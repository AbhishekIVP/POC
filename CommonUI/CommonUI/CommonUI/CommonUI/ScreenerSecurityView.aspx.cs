using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using com.ivp.rad.viewmanagement;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class ScreenerSecurityView : BasePage
    {
        private string securityName = string.Empty;

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string JSScript = "var sv_serverSideValue = { 'securityName' : '" + securityName + "' } ;";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ScreenerSecurityView", JSScript, true);
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            securityName = HttpUtility.UrlDecode(this.Request.QueryString["secName"]);
        }
    }
}