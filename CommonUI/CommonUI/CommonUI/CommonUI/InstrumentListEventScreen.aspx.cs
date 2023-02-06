using com.ivp.rad.viewmanagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class InstrumentListEventScreen : BasePage
    {
        
        protected override void OnPreRender(EventArgs e)
        {
            string UserName = SessionInfo.LoginName;
            string identifier = Request.QueryString["identifier"];
            string script = "instrumentListEventScreen.init('{username:\"" + UserName+"\"}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeInstrumentListEventScreen", script, true);
            base.OnPreRender(e);
        }
}
}