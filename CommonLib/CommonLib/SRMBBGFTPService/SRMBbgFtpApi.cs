using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.transport;
using com.ivp.rad.transport.sftp;
using com.ivp.srm.vendorapi;
using WinSCP;

namespace com.ivp.srm.bbgftpservice
{
    [DataContract]
    public class SMFTPFileInfo
    {
        [DataMember]
        public int VendorPreferenceId { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string FolderName { get; set; }
        [DataMember]
        public string Identifier { get; set; }
        [DataMember]
        public bool IsError { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }

        public TimeSpan EllapsedTime { get; set; }
        public int RetryCount { get; set; }
        public TimeSpan PollingStartTime { get; set; }

    }

    [ServiceContract(Namespace = "com.ivp.srm.bbgftpservice", Name = "ISRMBbgFtpApi")]
    public interface ISRMBbgFtpApi
    {
        [OperationContract]
        bool RegisterUploadFile(SMFTPFileInfo fileInfo, out string message);

        [OperationContract]
        bool RegisterDownloadFile(SMFTPFileInfo fileInfo, out string message);

        [OperationContract]
        bool GetUploadFileResponse(string identifier, out string message);

        [OperationContract]
        bool GetDownloadFileResponse(string identifier, out string message);
    }

    class SRMBbgFtpApi : ISRMBbgFtpApi
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMBbgFtpApi");

        static RSFTPTransportConfig transportConfig;

        static SessionOptions sessionOptions;

        static decimal maxRetryCount = 0;
        static decimal mPollingTimeout = 60000;
        static decimal mPolllingInterval = 10000;

        static SRMBbgFtpApi()
        {
            string transportName = ConfigurationManager.AppSettings["BBGTransportName"];
            if (string.IsNullOrWhiteSpace(transportName))
                throw new Exception("BBGTransportName not set in appSettings.config");

            transportConfig = (RSFTPTransportConfig)RTransportConfigLoader.GetTransportConfig(transportName);

            mPollingTimeout = transportConfig.PollingTimeout * 60000;
            mPolllingInterval = transportConfig.PollingInterval * 60000;

            maxRetryCount = Math.Ceiling(mPollingTimeout / mPolllingInterval);

            mLogger.Error("Polling Timeout-> " + mPollingTimeout);
            mLogger.Error("Polling Interval-> " + mPolllingInterval);
            mLogger.Error("Max Retry Count-> " + maxRetryCount);
            mLogger.Error("Host-> " + transportConfig.Host);
            mLogger.Error("Port-> " + transportConfig.PortNumber);
            mLogger.Error("User Name-> " + transportConfig.UserName);

            string SFTPPrivateKeyPath = ConfigurationManager.AppSettings["SFTPPrivateKeyPath"];
            if (string.IsNullOrWhiteSpace(SFTPPrivateKeyPath))
            {
                sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = transportConfig.Host,
                    UserName = transportConfig.UserName,
                    Password = transportConfig.Password,
                    PortNumber = transportConfig.PortNumber,
                    GiveUpSecurityAndAcceptAnySshHostKey = true,
                };
            }
            else
            {
                sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = transportConfig.Host,
                    UserName = transportConfig.UserName,
                    PortNumber = transportConfig.PortNumber,
                    SshPrivateKeyPath = SFTPPrivateKeyPath,
                    SshPrivateKeyPassphrase = transportConfig.Password,
                    GiveUpSecurityAndAcceptAnySshHostKey = true
                };
            }

            Thread taskThreadUpload = new Thread(new ThreadStart(RunUploadTask));
            taskThreadUpload.Start();

            Thread taskThreadDownload = new Thread(new ThreadStart(RunDownloadTask));
            taskThreadDownload.Start();
        }

        static Dictionary<string, SMFTPFileInfo> queuedUpload = new Dictionary<string, SMFTPFileInfo>(StringComparer.OrdinalIgnoreCase);
        static Dictionary<string, SMFTPFileInfo> queuedUploadResponse = new Dictionary<string, SMFTPFileInfo>(StringComparer.OrdinalIgnoreCase);

        static Dictionary<string, SMFTPFileInfo> queuedDownload = new Dictionary<string, SMFTPFileInfo>(StringComparer.OrdinalIgnoreCase);
        static Dictionary<string, SMFTPFileInfo> queuedDownloadResponse = new Dictionary<string, SMFTPFileInfo>(StringComparer.OrdinalIgnoreCase);

        static object lockObjUpload = new object();
        static object lockObjUploadResponse = new object();

        static object lockObjDownload = new object();
        static object lockObjDownloadResponse = new object();

        public int BBGQueueLimit
        {
            get
            {
                string limit = ConfigurationManager.AppSettings["BBGQueueLimit"];
                int intResult;
                if (!string.IsNullOrWhiteSpace(limit) && int.TryParse(limit, out intResult))
                {
                    return intResult;
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool RegisterUploadFile(SMFTPFileInfo fileInfo, out string message)
        {
            message = string.Empty;
            bool retValue = false;
            try
            {
                if (fileInfo == null)
                    throw new Exception("File info for BBG FTP Upload is null");

                if (string.IsNullOrWhiteSpace(fileInfo.Identifier))
                    throw new Exception("File identifier for BBG FTP Upload is null or empty");

                if (string.IsNullOrWhiteSpace(fileInfo.FileName))
                    throw new Exception("File name for BBG FTP Upload is null or empty");

                if (string.IsNullOrWhiteSpace(fileInfo.FolderName))
                    throw new Exception("Folder name for BBG FTP Upload is null or empty");

                lock (lockObjUpload)
                {
                    if (!queuedUpload.ContainsKey(fileInfo.Identifier))
                    {
                        if (BBGQueueLimit == 0 || queuedUpload.Count < BBGQueueLimit)
                        {
                            queuedUpload.Add(fileInfo.Identifier, fileInfo);
                            mLogger.Error("Registered Upload-> " + fileInfo.Identifier);
                        }
                    }

                    retValue = queuedUpload.ContainsKey(fileInfo.Identifier);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                retValue = false;
            }
            finally
            {
            }
            return retValue;
        }

        public bool RegisterDownloadFile(SMFTPFileInfo fileInfo, out string message)
        {
            message = string.Empty;
            bool retValue = false;
            try
            {
                if (fileInfo == null)
                    throw new Exception("File info for BBG FTP Download is null");

                if (string.IsNullOrWhiteSpace(fileInfo.Identifier))
                    throw new Exception("File identifier for BBG FTP Download is null or empty");

                if (string.IsNullOrWhiteSpace(fileInfo.FileName))
                    throw new Exception("File name for BBG FTP Download is null or empty");

                if (string.IsNullOrWhiteSpace(fileInfo.FolderName))
                    throw new Exception("Folder name for BBG FTP Download is null or empty");

                lock (lockObjDownload)
                {
                    if (!queuedDownload.ContainsKey(fileInfo.Identifier))
                    {
                        if (BBGQueueLimit == 0 || queuedDownload.Count < BBGQueueLimit)
                        {
                            queuedDownload.Add(fileInfo.Identifier, fileInfo);
                            mLogger.Error("Registered Download-> " + fileInfo.Identifier);
                        }
                    }

                    retValue = queuedDownload.ContainsKey(fileInfo.Identifier);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                retValue = false;
            }
            finally
            {
            }
            return retValue;
        }

        public bool GetUploadFileResponse(string identifier, out string message)
        {
            message = string.Empty;
            bool retValue = false;
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                    throw new Exception("File identifier for BBG FTP Upload is null or empty");

                lock (lockObjUploadResponse)
                {
                    if (queuedUploadResponse.ContainsKey(identifier))
                    {
                        var fileInfo = queuedUploadResponse[identifier];
                        retValue = !fileInfo.IsError;
                        message = fileInfo.ErrorMessage;
                        queuedUploadResponse.Remove(identifier);
                        mLogger.Error("Upload Response found-> " + identifier);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                retValue = false;
            }
            finally
            {
            }
            return retValue;
        }

        public bool GetDownloadFileResponse(string identifier, out string message)
        {
            message = string.Empty;
            bool retValue = false;
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                    throw new Exception("File identifier for BBG FTP Download is null or empty");

                lock (lockObjDownloadResponse)
                {
                    if (queuedDownloadResponse.ContainsKey(identifier))
                    {
                        var fileInfo = queuedDownloadResponse[identifier];
                        retValue = !fileInfo.IsError;
                        message = fileInfo.ErrorMessage;
                        queuedDownloadResponse.Remove(identifier);
                        mLogger.Error("Download Response found-> " + identifier);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                retValue = false;
            }
            finally
            {
            }
            return retValue;
        }

        static void RunUploadTask()
        {
            while (true)
            {
                var queuedUploadLocal = new Dictionary<string, SMFTPFileInfo>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var identifiersToRemove = new List<string>();
                    lock (lockObjUpload)
                    {
                        if (queuedUpload.Count > 0)
                        {
                            queuedUploadLocal = queuedUpload.ToDictionary(x => x.Key, y => y.Value, StringComparer.OrdinalIgnoreCase);
                        }
                    }

                    foreach (string identifier in queuedUploadLocal.Keys)
                    {
                        var fileInfo = queuedUploadLocal[identifier];
                        try
                        {
                            string workingDirectory = RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, fileInfo.VendorPreferenceId, "workingdirectory"));

                            string mFileToUpload = string.Empty;
                            mFileToUpload = fileInfo.FolderName;
                            mFileToUpload = mFileToUpload.Replace("\\", "//");
                            mFileToUpload = mFileToUpload.Insert(0, "//");

                            mLogger.Error("Send Message : " + identifier);

                            using (WinSCP.Session session = new WinSCP.Session())
                            {
                                session.Open(sessionOptions);
                                session.PutFiles(Path.Combine(workingDirectory, fileInfo.FileName), mFileToUpload);
                            }

                            identifiersToRemove.Add(identifier);
                        }
                        catch (Exception ex)
                        {
                            fileInfo.RetryCount++;
                            mLogger.Error("file identifier : " + identifier + " " + ex.Message.ToString());

                            if (fileInfo.RetryCount > maxRetryCount)
                            {
                                fileInfo.IsError = true;
                                fileInfo.ErrorMessage = "Failed to upload request file. Request timed out.";

                                mLogger.Error("Failed to upload request file. Request timed out. : " + identifier);

                                identifiersToRemove.Add(identifier);
                            }
                        }
                    }

                    if (identifiersToRemove.Count > 0)
                    {
                        lock (lockObjUpload)
                        {
                            lock (lockObjUploadResponse)
                            {
                                foreach (string identifier in identifiersToRemove)
                                {
                                    if (queuedUpload.ContainsKey(identifier))
                                    {
                                        if (!queuedUploadResponse.ContainsKey(identifier))
                                            queuedUploadResponse.Add(identifier, queuedUpload[identifier]);

                                        queuedUpload.Remove(identifier);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                }

                if (queuedUploadLocal.Count > 0)
                    Thread.Sleep((int)mPolllingInterval);
                else
                    Thread.Sleep(3000);
            }
        }

        static void RunDownloadTask()
        {
            while (true)
            {
                var queuedDownloadLocal = new Dictionary<string, SMFTPFileInfo>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var identifiersToRemove = new List<string>();
                    lock (lockObjDownload)
                    {
                        if (queuedDownload.Count > 0)
                        {
                            queuedDownloadLocal = queuedDownload.ToDictionary(x => x.Key, y => y.Value, StringComparer.OrdinalIgnoreCase);
                        }
                    }

                    foreach (string identifier in queuedDownloadLocal.Keys)
                    {
                        var fileInfo = queuedDownloadLocal[identifier];
                        try
                        {
                            string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                         (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, fileInfo.VendorPreferenceId, "workingdirectory"));

                            string mFileToDownload = fileInfo.FileName;
                            mFileToDownload = mFileToDownload.Replace("\\", "//");
                            if (!mFileToDownload.Trim().StartsWith("/"))
                                mFileToDownload = mFileToDownload.Insert(0, "/");

                            string mLocationToDownload = workingDirectory;

                            mLogger.Error("Receive Message : " + identifier);

                            using (WinSCP.Session session = new WinSCP.Session())
                            {
                                session.Open(sessionOptions);
                                if (!mLocationToDownload.EndsWith("\\"))
                                    mLocationToDownload = mLocationToDownload + "\\";
                                TransferOperationResult transferResult = session.GetFiles(mFileToDownload, mLocationToDownload);
                                transferResult.Check();
                                foreach (TransferEventArgs transfer in transferResult.Transfers)
                                {
                                    Console.WriteLine(string.Format("Download of {0} succeeded", transfer.FileName));
                                }
                            }

                            identifiersToRemove.Add(identifier);
                        }
                        catch (Exception ex)
                        {
                            fileInfo.RetryCount++;
                            mLogger.Error("file identifier : " + identifier + " " + ex.Message.ToString());

                            if (fileInfo.RetryCount > maxRetryCount)
                            {
                                fileInfo.IsError = true;
                                fileInfo.ErrorMessage = "Failed to download response file. Request timed out.";

                                mLogger.Error("Failed to download response file. Request timed out. : " + identifier);

                                identifiersToRemove.Add(identifier);
                            }
                        }
                    }

                    if (identifiersToRemove.Count > 0)
                    {
                        lock (lockObjDownload)
                        {
                            lock (lockObjDownloadResponse)
                            {
                                foreach (string identifier in identifiersToRemove)
                                {
                                    if (queuedDownload.ContainsKey(identifier))
                                    {
                                        if (!queuedDownloadResponse.ContainsKey(identifier))
                                            queuedDownloadResponse.Add(identifier, queuedDownload[identifier]);

                                        queuedDownload.Remove(identifier);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                }

                if (queuedDownloadLocal.Count > 0)
                    Thread.Sleep((int)mPolllingInterval);
                else
                    Thread.Sleep(3000);
            }
        }
    }
}
