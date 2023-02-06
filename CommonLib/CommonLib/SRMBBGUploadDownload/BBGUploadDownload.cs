using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.transport;
using com.ivp.srmcommon;

namespace SRMBBGUploadDownload
{
    class BBGUploadDownload
    {
        static void Service_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exp = (Exception)e.ExceptionObject;
            System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt", "Exception Message: " + exp.ToString());
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Service_UnhandledException);
            try
            {
                //Thread.Sleep(20000);
                //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                if (args[0] == "bbgupload")
                {
                    BBGUploadProcess trg = new BBGUploadProcess(args[1], args[2], args[3], args[4], args[5]);
                    trg.RunTask();
                }
                else if (args[0] == "bbgdownload")
                {
                    BBGDownloadProcess trg = new BBGDownloadProcess(args[1], args[2], args[3], args[4], args[5], args[6]);
                    trg.RunTask();
                }

                Environment.Exit(1);
            }
            catch (RTransportException ex)
            {
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Environment.Exit(-1);
                //Process.GetCurrentProcess().Kill();
            }
            finally
            {
                if (!Process.GetCurrentProcess().HasExited)
                    Process.GetCurrentProcess().Kill();
            }
        }

        internal static void ConsoleWrite(string message)
        {
            Console.Out.Flush();
            Console.Write(message);
        }
    }

    class BBGUploadProcess
    {
        string fileName = string.Empty;
        static readonly object objLock = new object();
        static StreamWriter writer = null;
        static string xmlFileName = string.Empty;
        static IRLogger logger = null;

        string _uploadFileName;
        string _transportName;
        string _identifier;
        string _folderName;
        public BBGUploadProcess(string uploadFileName, string transportName, string identifier, string folderName, string clientName)
        {
            try
            {
                this._uploadFileName = uploadFileName;
                this._transportName = transportName;
                this._identifier = identifier;
                this._folderName = folderName;
                SRMMTConfig.SetClientName(clientName);
                string directoryName = GetFullFilePath(clientName);
                fileName = directoryName + "\\" + DateTime.Now.ToString("HH-mm-ss") + " PID " + Process.GetCurrentProcess().Id.ToString() + " request" + _identifier + ".log";
                CreateLog(fileName, "Start Process");
                CreateLog(fileName, "Finished decryption of object and begin deserilize the object.");
                CreateLog(fileName, "Finished deserilizatin of object.");
                CreateJobLogger(clientName);
                CreateLog(fileName, "Logger Intantiated and invoke Process constructor.");
            }
            catch (Exception ex)
            {
                CreateLog(fileName, ex.ToString());
                throw;
            }
        }

        internal void RunTask()
        {
            try
            {
                CreateLog(fileName, "Process constructor invoked and execute the job.");
                new RTransportManager().GetTransport(_transportName).SendMessage(null, new object[]
                {
                    new RFTPContent
                    {
                        FileName = _uploadFileName,
                        FolderName = string.IsNullOrEmpty(_folderName)?"//":_folderName,
                        Action = FTPAction.Upload
                    }
                });
            }
            catch (RTransportException ex)
            {
                CreateLog(fileName, ex.ToString());
                throw new RTransportException("Failed to upload request file. Request timed out.");
            }
            catch (Exception ex)
            {
                CreateLog(fileName, ex.ToString());
                throw;
            }
            finally
            {
                CreateLog(fileName, "method completed.");
                Dispose();
            }
        }

        private void Dispose()
        {
            if (writer != null)
                writer.Close();
            writer = null;
        }

        private string GetFullFilePath(string clientName)
        {
            List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
            foreach (XmlDocument loggerDoc in lstConfig)
            {
                XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
                        xmlFileName = node.Attributes["value"].Value;
                }
            }
            string directoryName = System.IO.Directory.GetParent(xmlFileName).FullName;
            directoryName = directoryName + "\\" + clientName + "\\Bloomberg FTP Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\Post Log";
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            return directoryName;
        }

        public void CreateLog(string fullFileName, string message)
        {
            try
            {
                lock (objLock)
                {
                    if (writer == null)
                        writer = File.AppendText(fullFileName);

                    writer.WriteLine("[TimeStamp:->" + DateTime.Now.ToString("MMddyyyyHH:mm:ss") + "] Message:->" + message);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("BBGUploadProcess", ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void CreateJobLogger(string clientName)
        {
            CreateLog(fileName, string.Format("Started created job specific logger"));
            string directoryName = System.IO.Directory.GetParent(xmlFileName).FullName;
            string jobLoggingFolder = System.IO.Directory.GetParent(xmlFileName).FullName + "\\" + clientName +  "\\Bloomberg FTP Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\Log";
            if (!Directory.Exists(jobLoggingFolder))
                Directory.CreateDirectory(jobLoggingFolder);
            string jobLoggingPath = DateTime.Now.ToString("HH-mm-ss") + " PID " + Process.GetCurrentProcess().Id.ToString() + " request" + _identifier + ".log";

            logger = SRMMTCommon.CreateJobLogger(SRMMTConfig.isMultiTenantEnabled, jobLoggingPath, "BBGUploadProcess", jobLoggingFolder, out jobLoggingPath, clientName);

            //string routingKey = string.Empty;
            //string customFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CustomConfigFiles\LoggerConfigRoutingKeys.xml");
            //if (File.Exists(customFilePath))
            //{
            //    XDocument rKeys = XDocument.Load(customFilePath);
            //    if (rKeys != null)
            //    {
            //        foreach (XElement ele in rKeys.Root.Descendants("RoutingKey"))
            //        {
            //            if (ele.Attribute("name").Value.Equals("BBGUploadProcess"))
            //            {
            //                routingKey = ele.Attribute("key").Value;
            //                break;
            //            }
            //        }
            //    }
            //}

            //List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
            //foreach (XmlDocument loggerDoc in lstConfig)
            //{
            //    XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
            //    foreach (XmlNode node in nodeList)
            //    {
            //        if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
            //            node.Attributes["value"].Value = jobLoggingPath;
            //    }
            //    if (!string.IsNullOrEmpty(routingKey))
            //    {
            //        XmlNodeList RoutingKeyList = loggerDoc.GetElementsByTagName("RoutingKey");
            //        if (RoutingKeyList != null)
            //        {
            //            foreach (XmlNode node in RoutingKeyList)
            //            {
            //                node.Attributes["value"].Value = routingKey;
            //            }
            //        }
            //    }
            //}
            //logger = RLogFactory.CreateLogger("BBGUploadProcess", lstConfig[0].InnerXml);
            CreateLog(fileName, string.Format("logger created"));
        }
    }

    class BBGDownloadProcess
    {
        string fileName = string.Empty;
        static readonly object objLock = new object();
        static StreamWriter writer = null;
        static string xmlFileName = string.Empty;
        static IRLogger logger = null;

        string _downloadFileName;
        string _downloadFolder;
        string _transportName;
        string _identifier;
        string _folderName;
        public BBGDownloadProcess(string downloadFileName, string downloadFolder, string transportName, string identifier, string folderName, string clientName)
        {
            try
            {
                this._downloadFileName = downloadFileName;
                this._downloadFolder = downloadFolder;
                this._transportName = transportName;
                this._identifier = identifier;
                this._folderName = folderName;
                SRMMTConfig.SetClientName(clientName);

                string directoryName = GetFullFilePath(clientName);
                fileName = directoryName + "\\" + DateTime.Now.ToString("HH-mm-ss") + " PID " + Process.GetCurrentProcess().Id.ToString() + " response" + _identifier + ".log";
                CreateLog(fileName, "Start Process");
                CreateLog(fileName, "Finished decryption of object and begin deserilize the object.");
                CreateLog(fileName, "Finished deserilizatin of object.");
                CreateJobLogger(clientName);
                CreateLog(fileName, "Logger Intantiated and invoke Process constructor.");
            }
            catch (Exception ex)
            {
                CreateLog(fileName, ex.ToString());
                throw;
            }
        }

        internal void RunTask()
        {
            try
            {
                CreateLog(fileName, "Process constructor invoked and execute the job.");
                new RTransportManager().GetTransport(_transportName).ReceiveMessage(null, new object[]
                {
                    new RFTPContent
                    {
                        FileName = string.IsNullOrEmpty(_folderName)?_downloadFileName:Path.Combine(_folderName, _downloadFileName),
                        FolderName = _downloadFolder,
                        UsePrivateKey = true
                    }
                });
            }
            catch (RTransportException ex)
            {
                CreateLog(fileName, ex.ToString());
                throw new RTransportException("Failed to download response file. Request timed out.");
            }
            catch (Exception ex)
            {
                CreateLog(fileName, ex.ToString());
                throw;
            }
            finally
            {
                CreateLog(fileName, "method completed.");
                Dispose();
            }
        }

        private void Dispose()
        {
            if (writer != null)
                writer.Close();
            writer = null;
        }

        private string GetFullFilePath(string clientName)
        {
            List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
            foreach (XmlDocument loggerDoc in lstConfig)
            {
                XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
                        xmlFileName = node.Attributes["value"].Value;
                }
            }
            string directoryName = System.IO.Directory.GetParent(xmlFileName).FullName;
            directoryName = directoryName + "\\" + clientName  + "\\Bloomberg FTP Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\Post Log";
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            return directoryName;
        }

        public void CreateLog(string fullFileName, string message)
        {
            try
            {
                lock (objLock)
                {
                    if (writer == null)
                        writer = File.AppendText(fullFileName);

                    writer.WriteLine("[TimeStamp:->" + DateTime.Now.ToString("MMddyyyyHH:mm:ss") + "] Message:->" + message);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("BBGDownloadProcess", ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void CreateJobLogger(string clientName)
        {
            CreateLog(fileName, string.Format("Started created job specific logger"));
            //string directoryName = System.IO.Directory.GetParent(xmlFileName).FullName;
            string jobLoggingFolder = System.IO.Directory.GetParent(xmlFileName).FullName + "\\" + clientName + "\\Bloomberg FTP Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\Log";
            if (!Directory.Exists(jobLoggingFolder))
                Directory.CreateDirectory(jobLoggingFolder);
            string jobLoggingPath = DateTime.Now.ToString("HH-mm-ss") + " PID " + Process.GetCurrentProcess().Id.ToString() + " response" + _identifier + ".log";

            logger = SRMMTCommon.CreateJobLogger(SRMMTConfig.isMultiTenantEnabled, jobLoggingPath, "BBGUploadProcess",  jobLoggingFolder, out jobLoggingPath, clientName);

            //string routingKey = string.Empty;
            //string customFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CustomConfigFiles\LoggerConfigRoutingKeys.xml");
            //if (File.Exists(customFilePath))
            //{
            //    XDocument rKeys = XDocument.Load(customFilePath);
            //    if (rKeys != null)
            //    {
            //        foreach (XElement ele in rKeys.Root.Descendants("RoutingKey"))
            //        {
            //            if (ele.Attribute("name").Value.Equals("BBGDownloadProcess"))
            //            {
            //                routingKey = ele.Attribute("key").Value;
            //                break;
            //            }
            //        }
            //    }
            //}

            //List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
            //foreach (XmlDocument loggerDoc in lstConfig)
            //{
            //    XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
            //    foreach (XmlNode node in nodeList)
            //    {
            //        if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
            //            node.Attributes["value"].Value = jobLoggingPath;
            //    }
            //    if (!string.IsNullOrEmpty(routingKey))
            //    {
            //        XmlNodeList RoutingKeyList = loggerDoc.GetElementsByTagName("RoutingKey");
            //        if (RoutingKeyList != null)
            //        {
            //            foreach (XmlNode node in RoutingKeyList)
            //            {
            //                node.Attributes["value"].Value = routingKey;
            //            }
            //        }
            //    }
            //}
            //logger = RLogFactory.CreateLogger("BBGDownloadProcess", lstConfig[0].InnerXml);
            CreateLog(fileName, string.Format("logger created"));
        }
    }
}
