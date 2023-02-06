using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.transport;
using System.IO;
using com.ivp.srmcommon;

namespace com.ivp.srm.vendorapi.bloomberg
{
    class RbbgHistoryFtpHandler
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RbbgHistoryFtpHandler");
        internal void GetHistory(RBbgHistoryInfo historyInfo, string appenTime, ref Dictionary<string, string> dictHeaders)
        {
            IRTransport transport = null;
            RFTPContent objftpContent = null;
            try
            {
                mLogger.Debug("RbbgHistoryFtpHandler:GetCorpAction=>start getting history data");
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                          (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                workingDirectory = SRMMTCommon.GetClientFilePath(workingDirectory);
                string requestFileName = RCorpActionConstant.REQ + appenTime + RCorpActionConstant.DOTREQ;
                string responseFileName = RCorpActionConstant.RES + appenTime + RCorpActionConstant.DOTRES;
                RBbgHistoryUtils.WriteSecInfoOnFile(historyInfo, requestFileName, responseFileName, workingDirectory, ref dictHeaders);
                RBbgUtils.InsertBBGAuditHeaders(appenTime, dictHeaders);
                //get transport and send file
                transport = new RTransportManager().GetTransport(historyInfo.TransportName);

                objftpContent = new RFTPContent();
                objftpContent.FileName = Path.Combine(workingDirectory, requestFileName);
                objftpContent.FolderName = RCorpActionConstant.BACKSLASH;
                objftpContent.Action = FTPAction.Upload;

                string ftpFolderName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.FTPFOLDERNAME);
                if (!string.IsNullOrEmpty(ftpFolderName))
                    objftpContent.FolderName = ftpFolderName;

                object[] ftpArgs = { objftpContent };
                //send file to bloomberg
                transport.SendMessage(null, ftpArgs);
                //recieve response, parse the file and return the xml doc.
                GetResponse(historyInfo.TransportName, appenTime, historyInfo);
                mLogger.Debug("RbbgHistoryFtpHandler:GetCorpAction=>End getting history data");
            }
            catch (Exception ex)
            {
                mLogger.Error("RbbgHistoryFtpHandler:GetCorpAction=>" + ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                if (transport != null)
                    transport = null;
                if (objftpContent != null)
                    objftpContent = null;
            }
        }

        private void GetResponse(string transportName, string requestIdentifier, RBbgHistoryInfo historyInfo)
        {
            RFTPContent ftpContent = null;
            IRTransport transport = null;
            try
            {
                mLogger.Debug("RbbgHistoryFtpHandler:GetResponse=>start getting history data response");
                transport = new RTransportManager().GetTransport(transportName);
                string appenTime = requestIdentifier;
                // fileName = appenTime;
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                          (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId,RVendorConstant.WORKINGDIRECTORY));
                workingDirectory = SRMMTCommon.GetClientFilePath(workingDirectory);
                string requestFileName = RCorpActionConstant.REQ + appenTime + RCorpActionConstant.DOTREQ;
                string responseFileName = RCorpActionConstant.RES + appenTime + RCorpActionConstant.DOTRESDOTGZ;
                string outputFileName = RCorpActionConstant.DECRYPT + appenTime;

                ftpContent = new RFTPContent();
                ftpContent.FileName = responseFileName;
                ftpContent.FolderName = workingDirectory;
                ftpContent.UsePrivateKey = true;

                string ftpFolderName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.FTPFOLDERNAME);
                if (!string.IsNullOrEmpty(ftpFolderName))
                    ftpContent.FileName = Path.Combine(ftpFolderName, ftpContent.FileName);

                object[] ftpArg = { ftpContent };
                transport.ReceiveMessage(null, ftpArg);
                outputFileName = RBbgHistoryUtils.DecryptOutputFile(workingDirectory, responseFileName, outputFileName, historyInfo);
                //parse the file to generate xml
                RBbgHistoryUtils.GetData(Path.Combine(workingDirectory, outputFileName + RCorpActionConstant.DOTTXT), historyInfo);
                string xml = RBbgHistoryUtils.PrepareXML(historyInfo, appenTime);
                RBbgHistoryUtils.UpdateHistoryDetails(xml, appenTime);
                mLogger.Debug("RbbgHistoryFtpHandler:GetResponse=>end getting history data response");
            }
            catch (Exception ex)
            {
                mLogger.Error("RbbgHistoryFtpHandler:GetResponse=>" + ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                if (transport != null)
                    transport = null;
                if (ftpContent != null)
                    ftpContent = null;
            }
        }
    }
}
