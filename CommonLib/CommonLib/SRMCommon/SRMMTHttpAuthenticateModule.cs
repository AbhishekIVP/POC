using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Threading;
using com.ivp.rad.utils;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using System.Data;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace com.ivp.srmcommon.mtmodule
{
    public class SRMMTConfig
    {
        internal static string MultiTenantEnabled;// = RConfigReader.GetConfigAppSettings("MultiTenantEnabled");

        internal static bool isConfigMultiTenantRead;
        public static bool isMultiTenantEnabled
        {
            get
            {
                if (!isConfigMultiTenantRead)
                {
                    MultiTenantEnabled = RConfigReader.GetConfigAppSettings("MultiTenantEnabled");
                    if (!string.IsNullOrEmpty(MultiTenantEnabled) && MultiTenantEnabled.ToLower().Equals("true"))
                        MultiTenantEnabled = "true";
                    else
                        MultiTenantEnabled = "false";

                    isConfigMultiTenantRead = true;
                }
                return Convert.ToBoolean(MultiTenantEnabled);
            }
        }
        public static string ClientDBID = RConfigReader.GetConfigAppSettings("ClientDBID");

        public static string GetClientName()
        {
            string clientName = string.Empty;
            if (isMultiTenantEnabled)
                clientName = RMultiTanentUtil.ClientName;
            return clientName;
        }


        public static string SetClientNameFromHeaders(bool isTCP)
        {
            string clientName = string.Empty;
            clientName = SRMCommon.GetHeaderValue("ClientName", false, isTCP);
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
            { 
                RMultiTanentUtil.ClientName = clientName;
                RMultiTanentClientUtil.ClientName = clientName;
            }

            return clientName;
        }

        public static void MethodCallPerClient(Action<string, int> methodToBeCalled, int moduleId)
        {
            HashSet<string> clientNames = new HashSet<string>();

            if (SRMMTConfig.isMultiTenantEnabled)
            {
                RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(SRMMTConfig.ClientDBID);
                string query = "SELECT client_real_name FROM [dbo].[ivp_ctrl_client_master] WHERE is_active = 1";
                DataSet ds = dbConnection.ExecuteQuery(query, RQueryType.Select);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        clientNames.Add(Convert.ToString(dr[0]));
                }
            }
            else
                clientNames.Add(string.Empty);

            Parallel.ForEach(clientNames, clientName =>
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                {
                    RMultiTanentUtil.ClientName = clientName;
                    RMultiTanentClientUtil.ClientName = clientName;
                }
                methodToBeCalled(clientName, moduleId);
            });
        }

        public static void MethodCallPerClient(Action<string> methodToBeCalled)
        {
            HashSet<string> clientNames = new HashSet<string>();

            if (SRMMTConfig.isMultiTenantEnabled)
            {
                RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(SRMMTConfig.ClientDBID);
                string query = "SELECT client_real_name FROM [dbo].[ivp_ctrl_client_master] WHERE is_active = 1";
                DataSet ds = dbConnection.ExecuteQuery(query, RQueryType.Select);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        clientNames.Add(Convert.ToString(dr[0]));
                }
            }
            else
                clientNames.Add(string.Empty);

            Parallel.ForEach(clientNames, clientName =>
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                { 
                    RMultiTanentClientUtil.ClientName = clientName;
                    RMultiTanentUtil.ClientName = clientName;
                }

                methodToBeCalled(clientName);
            });
        }

    }
    public class SRMMTHttpAuthenticateModule : IHttpModule
    {
        bool isClientNameSet = false;
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {

            context.AuthenticateRequest += context_AuthenticateRequest;

            context.PreRequestHandlerExecute += Context_PreRequestHandlerExecute;
        }

        private void Context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            ValidateHttpRequest();
        }

        //void validateClientSession(string userId, string token, string clientName)
        //{
        //    bool isValidSession = true; // change it accordingly;
        //    // validate session by calling method here

        //    if (isValidSession)
        //    {
        //        //AA//RMultiTanentUtil.ClientName = clientName;
        //    }
        //    else
        //    {
        //        throw new Exception("Invalid Session");
        //    }
        //}

        void context_AuthenticateRequest(object sender, EventArgs e)
        {
            ValidateHttpRequest();
        }

        public void ValidateHttpRequest()
        {
            string clientName = string.Empty;
            string clientDisplayName = string.Empty;
            string timeZone = string.Empty;
            string token = string.Empty;
            string licensePath = string.Empty;
            //RMultiTanentUtil.ClientName = string.Empty;
            StringBuilder sb = new StringBuilder();

             if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["ClientName"] != null)
            {
                clientName = HttpContext.Current.Session["ClientName"].ToString();
                if (!string.IsNullOrEmpty(clientName) && !clientName.ToLower().Equals("null") && RMultiTanentUtil.ClientName != clientName)
                {
                    clientDisplayName = HttpContext.Current.Session["ClientDisplayName"].ToString();
                    timeZone = HttpContext.Current.Session["TimeZone"].ToString();
                    licensePath = HttpContext.Current.Session["LicensePath"].ToString();

                    RMultiTanentClientUtil.ClientName = clientName;
                    RMultiTanentUtil.ClientName = clientName;
                    RMultiTanentUtil.ClientDisplayName = clientDisplayName;
                    RMultiTanentUtil.TimeZone = timeZone;
                    RMultiTanentUtil.LicensePath = licensePath;

                }
            }
        }

        public static IPrincipal ProcessAuthentication()
        {
            IIdentity identity = new GenericIdentity("authenticated User");
            return (IPrincipal)identity;
        }
    }
}