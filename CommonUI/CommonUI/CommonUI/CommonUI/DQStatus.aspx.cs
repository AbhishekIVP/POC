using com.ivp.rad.culturemanager;
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
    public partial class DQStatus : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo('" + serializer.Serialize(SessionInfo.CultureInfo) + "', '" + serializer.Serialize(RCultureController.GetServerCultureInfo()) + "'); Initializer();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeDashboard", script, true);
        }
    }
}