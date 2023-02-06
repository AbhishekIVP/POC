using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.transport;
using System.IO;
using com.ivp.srmcommon;

namespace com.ivp.srm.vendorapi.Bloomberg.Fundamentals
{
    internal class RBbgFundamentalFTPHandler
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RBbgFundamentalFTPHandler");


        internal void GetFundamental(RBbgFundamentalInfo secInfo, string appenTime)
        {
            IRTransport transport = null;
            RFTPContent objftpContent = null;
            try
            {
                mLogger.Debug("RBbgFundamentalFTPHandler:GetFundamental=>start getting fundamental data");
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                          (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                string requestFileName = RCorpActionConstant.REQ + appenTime + RCorpActionConstant.DOTREQ;
                string responseFileName = RCorpActionConstant.RES + appenTime + RCorpActionConstant.DOTRES;
                RBbgFundamentalUtils.WriteSecInfoOnFile(secInfo, requestFileName, responseFileName, workingDirectory);
                //get transport and send file
                transport = new RTransportManager().GetTransport(secInfo.TransportName);

                objftpContent = new RFTPContent();
                objftpContent.FileName = Path.Combine(workingDirectory, requestFileName);
                objftpContent.FolderName = RCorpActionConstant.BACKSLASH;
                objftpContent.Action = FTPAction.Upload;

                string ftpFolderName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.FTPFOLDERNAME);
                if (!string.IsNullOrEmpty(ftpFolderName))
                    objftpContent.FolderName = ftpFolderName;

                object[] ftpArgs = { objftpContent };
                //send file to bloomberg
                transport.SendMessage(null, ftpArgs);
                //recieve response, parse the file and return the xml doc.
                if (secInfo.ImmediateRequest)
                    GetResponse(secInfo.TransportName, appenTime, secInfo);
                mLogger.Debug("RBbgFundamentalFTPHandler:GetFundamental =>End getting fundamental data");
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgFundamentalFTPHandler:GetFundamental=>" + ex.ToString());
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

        internal void GetResponse(string transportName, string requestIdentifier, RBbgFundamentalInfo secInfo)
        {
            RFTPContent ftpContent = null;
            IRTransport transport = null;
            try
            {
                mLogger.Debug("RBbgFundamentalFTPHandler:GetResponse=>start getting fundamental data response");
                transport = new RTransportManager().GetTransport(transportName);
                string appenTime = requestIdentifier;
                // fileName = appenTime;
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                          (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                string requestFileName = RCorpActionConstant.REQ + appenTime + RCorpActionConstant.DOTREQ;
                string responseFileName = RCorpActionConstant.RES + appenTime + RCorpActionConstant.DOTRES;
                string outputFileName = RCorpActionConstant.DECRYPT + appenTime + RCorpActionConstant.DOTTXT;

                workingDirectory = SRMMTCommon.GetClientFilePath(workingDirectory);
                ftpContent = new RFTPContent();
                ftpContent.FileName = responseFileName;
                ftpContent.FolderName = workingDirectory;
                ftpContent.UsePrivateKey = true;

                string ftpFolderName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.FTPFOLDERNAME);
                if (!string.IsNullOrEmpty(ftpFolderName))
                    ftpContent.FileName = Path.Combine(ftpFolderName, ftpContent.FileName);

                object[] ftpArg = { ftpContent };
                transport.ReceiveMessage(null, ftpArg);
                RBbgFundamentalUtils.DecryptOutputFile(workingDirectory, responseFileName, outputFileName, secInfo);
                //parse the file to generate xml
                RBbgFundamentalUtils.GetData(Path.Combine(workingDirectory, outputFileName), secInfo);
                mLogger.Debug("RBbgFundamentalFTPHandler:GetResponse=>end getting fundamental data response");
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgFundamentalFTPHandler:GetResponse=>" + ex.ToString());
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
