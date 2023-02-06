using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.transport;
using System.IO;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.common;
using com.ivp.srmcommon;

namespace com.ivp.srm.vendorapi.bloomberg
{
    internal class RCorpActFtpHandler
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        public void GetCorpAction(RCorpActInfo corpActInfo, string appenTime, ref Dictionary<string, string> dictHeaders)
        {
            IRTransport transport = null;
            RFTPContent objftpContent = null;
            try
            {
                mLogger.Debug("RCorpActFtpHandler:GetCorpAction=>start getting corp action");
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                          (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                workingDirectory = SRMMTCommon.GetClientFilePath(workingDirectory);
                string requestFileName = RCorpActionConstant.REQ + appenTime + RCorpActionConstant.DOTREQ;
                string responseFileName = RCorpActionConstant.RES + appenTime + RCorpActionConstant.DOTRES;
                string outputFileName = RCorpActionConstant.DECRYPT + appenTime + RCorpActionConstant.DOTTXT;
                RCorpActUtils.WriteSecInfoOnFile(corpActInfo, requestFileName, responseFileName, workingDirectory, ref dictHeaders);
                RBbgUtils.InsertBBGAuditHeaders(appenTime, dictHeaders);
                //get transport and send file
                transport = new RTransportManager().GetTransport(corpActInfo.TransportName);

                objftpContent = new RFTPContent();
                objftpContent.FileName = Path.Combine(workingDirectory, requestFileName);
                objftpContent.FolderName = RCorpActionConstant.BACKSLASH;
                objftpContent.Action = FTPAction.Upload;

                string ftpFolderName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.FTPFOLDERNAME);
                if (!string.IsNullOrEmpty(ftpFolderName))
                    objftpContent.FolderName = ftpFolderName;

                object[] ftpArgs = { objftpContent };
                //send file to bloomberg
                transport.SendMessage(null, ftpArgs);
                //recieve response, parse the file and return the xml doc.
                corpActInfo.RequestIdentifier = appenTime;
                if (corpActInfo.ImmediateResponse)
                    GetResponse(corpActInfo);
                mLogger.Debug("RCorpActFtpHandler:GetCorpAction=>End getting corp action");
            }
            catch (Exception ex)
            {
                corpActInfo.HasError = true;
                mLogger.Error("RCorpActFtpHandler:GetCorpAction=>" + ex.ToString());
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

        public void GetResponse(RCorpActInfo corpActInfo)
        {
            RFTPContent ftpContent = null;
            IRTransport transport = null;
            try
            {
                mLogger.Debug("RCorpActFtpHandler:GetResponse=>start getting corp action response");
                transport = new RTransportManager().GetTransport(corpActInfo.TransportName);
                string appenTime = corpActInfo.RequestIdentifier;
                // fileName = appenTime;
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                          (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                workingDirectory = SRMMTCommon.GetClientFilePath(workingDirectory);
                string requestFileName = RCorpActionConstant.REQ + appenTime + RCorpActionConstant.DOTREQ;
                string responseFileName = RCorpActionConstant.RES + appenTime + RCorpActionConstant.DOTRES;
                string outputFileName = RCorpActionConstant.DECRYPT + appenTime + RCorpActionConstant.DOTTXT;

                ftpContent = new RFTPContent();
                ftpContent.FileName = responseFileName;
                ftpContent.FolderName = workingDirectory;
                ftpContent.UsePrivateKey = true;

                string ftpFolderName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.FTPFOLDERNAME);
                if (!string.IsNullOrEmpty(ftpFolderName))
                    ftpContent.FileName = Path.Combine(ftpFolderName, ftpContent.FileName);

                object[] ftpArg = { ftpContent };
                transport.ReceiveMessage(null, ftpArg);
                outputFileName = RCorpActUtils.DecryptOutputFile(workingDirectory, responseFileName, outputFileName, corpActInfo);
                //parse the file to generate xml
                RCorpActUtils.GetXML(Path.Combine(workingDirectory, outputFileName), corpActInfo);
                mLogger.Debug("RCorpActFtpHandler:GetResponse=>end getting corp action response");
            }
            catch (Exception ex)
            {
                corpActInfo.HasError = true;
                mLogger.Error("RCorpActFtpHandler:GetResponse=>" + ex.ToString());
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
