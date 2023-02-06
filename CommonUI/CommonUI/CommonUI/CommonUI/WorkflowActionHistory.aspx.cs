using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.culturemanager;
using com.ivp.rad.viewmanagement;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class WorkflowActionHistory : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);


        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            //Changed to give ShorDate Format to Workflow Methods in Common Lib
            //string script = "workflowActionHistory.initializer('" + serializer.Serialize(GetWorkflowActionHistoryInfo()) + "'); ";
            string script = "com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo('" + serializer.Serialize(SessionInfo.CultureInfo) + "', '" + serializer.Serialize(RCultureController.GetServerCultureInfo()) + "'); workflowActionHistory.Initializer('" + serializer.Serialize(GetWorkflowActionHistoryInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "workflowActionHistory", script, true);
            base.OnPreRender(e);
        }

        public WorkflowActionHistoryInfo GetWorkflowActionHistoryInfo() {
            WorkflowActionHistoryInfo obj = new WorkflowActionHistoryInfo();
            obj.instrumentId = !string.IsNullOrEmpty(Convert.ToString(Request.QueryString["instrumentId"])) ? Convert.ToString(Request.QueryString["instrumentId"]) : "@$@";
            obj.radWorkflowInstanceId = !string.IsNullOrEmpty(Convert.ToString(Request.QueryString["radWorkflowInstanceId"])) ? Convert.ToInt32(Request.QueryString["radWorkflowInstanceId"]) : -1;
            obj.userName = SessionInfo.LoginName;
            obj.moduleId = !string.IsNullOrEmpty(Convert.ToString(Request.QueryString["moduleId"])) ? Convert.ToInt32(Request.QueryString["moduleId"]) : -1;

            return obj;
        }

        
    }

    public class WorkflowActionHistoryInfo
    {
        public string userName { get; set; }
        public string instrumentId { get; set; }

        public int radWorkflowInstanceId { get; set; }

        public int moduleId { get; set; }
    }
}