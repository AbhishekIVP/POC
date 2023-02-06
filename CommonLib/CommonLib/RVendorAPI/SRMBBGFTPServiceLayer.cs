using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using com.ivp.srm.vendorapi.BBFTPServiceLayer;
using System.Diagnostics;

namespace com.ivp.srm.vendorapi
{
    public class SRMBBGFTPServiceLayer
    {
        static com.ivp.rad.common.IRLogger mLogger = RLogFactory.CreateLogger("SRMBBGFTPServiceLayer");
        public static bool RegisterUploadFile(SMFTPFileInfo fileInfo, out string message)
        {
            SRMBbgFtpApiClient cl = null;
            try
            {
                mLogger.Error("RegisteringUploadFile -> " + fileInfo.Identifier);
                //mLogger.Error("Calling Method -> " + new StackTrace().ToString());
                cl = new SRMBbgFtpApiClient();
                return cl.RegisterUploadFile(out message, fileInfo);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                if (cl != null)
                    cl.Close();
            }
        }

        public static bool RegisterDownloadFile(SMFTPFileInfo fileInfo, out string message)
        {
            SRMBbgFtpApiClient cl = null;
            try
            {
                mLogger.Error("RegisteringDownloadFile -> " + fileInfo.Identifier);
                //mLogger.Error("Calling Method -> " + new StackTrace().ToString());
                cl = new SRMBbgFtpApiClient();
                return cl.RegisterDownloadFile(out message, fileInfo);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                if (cl != null)
                    cl.Close();
            }
        }

        public static bool GetUploadFileResponse(string identifier, out string message)
        {
            SRMBbgFtpApiClient cl = null;
            try
            {
                mLogger.Error("GetUploadFileResponse -> " + identifier);
                //mLogger.Error("Calling Method -> " + new StackTrace().ToString());
                cl = new SRMBbgFtpApiClient();
                return cl.GetUploadFileResponse(out message, identifier);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                if (cl != null)
                    cl.Close();
            }
        }

        public static bool GetDownloadFileResponse(string identifier, out string message)
        {
            SRMBbgFtpApiClient cl = null;
            try
            {
                mLogger.Error("GetDownloadFileResponse -> " + identifier);
                //mLogger.Error("Calling Method -> " + new StackTrace().ToString());
                cl = new SRMBbgFtpApiClient();
                return cl.GetDownloadFileResponse(out message, identifier);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                if (cl != null)
                    cl.Close();
            }
        }
    }
}
