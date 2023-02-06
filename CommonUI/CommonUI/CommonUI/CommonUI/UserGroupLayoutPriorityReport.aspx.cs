using com.ivp.rad.culturemanager;
using com.ivp.rad.RUserManagement;
using com.ivp.rad.viewmanagement;
using com.ivp.secm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class UserGroupLayoutPriorityReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //string script = "com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo('" + serializer.Serialize(SessionInfo.CultureInfo) + "', '" + serializer.Serialize(RCultureController.GetServerCultureInfo()) + "'); srmLineageLandingPage.initializer('" + serializer.Serialize(GetSRMLineageJSInfo()) + "');";
            string script = "var UserGroupLayoutReportSessionInfo = " + serializer.Serialize(GetCommonStatusInfo()) + ";UserGroupLayoutReport.Initializer()";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "mandatoryDataReportScript", script, true);
        }
        private UserGroupLayoutReport GetCommonStatusInfo()
        {
            UserGroupLayoutReport securityInfo = new UserGroupLayoutReport();
            securityInfo.username = SessionInfo.LoginName;
            securityInfo.clientShortDate = SessionInfo.CultureInfo.ShortDateFormat;
            securityInfo.clientLongDate = SessionInfo.CultureInfo.LongDateFormat;
            securityInfo.clientTimeFormat = SessionInfo.CultureInfo.ShortTimePattern;
            securityInfo.guid = Guid.NewGuid().ToString();
            return securityInfo;
        }
    }
    public class UserGroupLayoutReport
    {
        public string username { get; set; }
        public string clientShortDate { get; set; }
        public string clientLongDate { get; set; }
        public string clientTimeFormat { get; set; }
        public string guid { get; set; }
    }
}