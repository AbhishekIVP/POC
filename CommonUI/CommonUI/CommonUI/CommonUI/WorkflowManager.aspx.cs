using com.ivp.rad.ExcelCreator;
using com.ivp.rad.excellibrary;
using com.ivp.rad.viewmanagement;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.GAC;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class WorkflowManager : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            UserName.Value = SessionInfo.LoginName;
            hdnGuid.Value = Guid.NewGuid().ToString().Replace('-', '_');

            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //string script = "workflowManger.preInit('" + serializer.Serialize(new { UserName = SessionInfo.LoginName}) + "');";
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeWorkflowManager", script, true);
            //ClientScript.RegisterStartupScript(GetType(), "scriptToInitializeWorkflowManager", script, true);
        }

        protected void ExportWorkflow_Click(object sender, EventArgs e)
        {
            DataSet config = WorkflowController.GetAllWorkflowsConfiguration();

            byte[] excelBinary = SRMCommon.GetExcelByteArray(config);

            if (excelBinary != null)
            {
                string instanceName = SRMCommon.GetConfigFromDB("InstanceName");
                string fileName = string.Format("SRM Workflow Configuration - {0}.xls", instanceName);
                Response.Clear();
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(excelBinary);
                Response.End();
            }
        }
    }
}