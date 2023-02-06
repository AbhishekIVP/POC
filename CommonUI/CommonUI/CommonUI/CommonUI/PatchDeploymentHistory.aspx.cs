using com.ivp.commom.commondal;
using com.ivp.rad.viewmanagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class PatchDeploymentHistory : BasePage
    {
        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            PatchDeploymentHistoryData data = new PatchDeploymentHistoryData();
            if (Request.QueryString != null && !string.IsNullOrEmpty(Request.QueryString["moduleId"]))
            {
                data.moduleId = Int32.Parse(Request.QueryString["moduleId"]);
            }
            else
                data.moduleId = 3;
            string script = "patchDeploymentHistory.init('" + serializer.Serialize(data) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "patchDeploymentHistoryInitializer", script, true);
            base.OnPreRender(e);
        }
    }
    public class PatchDeploymentHistoryData
    {
        public int moduleId { get; set; }
    }
}