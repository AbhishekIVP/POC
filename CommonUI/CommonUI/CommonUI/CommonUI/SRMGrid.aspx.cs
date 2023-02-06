using com.ivp.rad.utils;
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
    public partial class SRMGrid : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            int moduleId = Convert.ToInt32(Request.QueryString["moduleId"]);
            int templateId = Convert.ToInt32(Request.QueryString["templateId"]);
            string JSScript = "srmGrid.Initializer('" + templateId + "','" + moduleId + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SRMGrid", JSScript, true);
        }
    }
}