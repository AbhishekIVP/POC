using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.viewmanagement;
using System.Web.Script.Serialization;
using com.ivp.rad.utils;

namespace Reconciliation.CommonUI
{
    public partial class WorkflowSetup : BasePage
    {
        private string productName;
        private WorkflowSetupInfo info;
        private Boolean isDemoBuild; 
        private Boolean showRefWorkflow; 

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string JSScript = "var workflowSetup_info = " + serializer.Serialize(GetWorkflowSetupInfo()) + ";";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "WorkflowSetup", JSScript, true);
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            productName = RConfigReader.GetConfigAppSettings("ProductName");
            isDemoBuild = Convert.ToBoolean(RConfigReader.GetConfigAppSettings("IsDemoBuild"));
        }

        private WorkflowSetupInfo GetWorkflowSetupInfo()
        {
            info = new WorkflowSetupInfo();
            info.Username = SessionInfo.LoginName;
            info.CultureShortDateFormat = SessionInfo.CultureInfo.ShortDateFormat;
            info.ProductName = productName;
            info.IsDemoBuild = isDemoBuild;
            info.ShowRefWorkflow = showRefWorkflow;
            return info;
        }
    }

    class WorkflowSetupInfo
    {
        public string Username { get; set; }
        public string ProductName { get; set; }
        public string CultureShortDateFormat { get; set; }
        public Boolean ShowRefWorkflow { get; set; }
        public Boolean IsDemoBuild { get; set; }
    }
}