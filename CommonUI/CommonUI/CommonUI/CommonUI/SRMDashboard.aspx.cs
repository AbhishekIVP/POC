using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.utils;
using com.ivp.rad.viewmanagement;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMDashboard : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "srmDashboard.Init('" + serializer.Serialize(GetSRMDashboardInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeSRMDashboard", script, true);
            base.OnPreRender(e);
        }

        public SRMDashboardInfo GetSRMDashboardInfo()
        {
            string IsDemoBuild = RConfigReader.GetConfigAppSettings("IsDemoBuild");

            SRMDashboardInfo objSRMDashboardInfo = new SRMDashboardInfo();
            objSRMDashboardInfo.UserName = SessionInfo.LoginName;
            objSRMDashboardInfo.IsDemoBuild = ((!string.IsNullOrEmpty(IsDemoBuild) && IsDemoBuild.Equals("true", StringComparison.OrdinalIgnoreCase)) ? true : false);
            return objSRMDashboardInfo;
        }
    }

    public class SRMDashboardInfo
    {
        public string UserName { get; set; }
        public bool IsDemoBuild { get; set; }
    }
}