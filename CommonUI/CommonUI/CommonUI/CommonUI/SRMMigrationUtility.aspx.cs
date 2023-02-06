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
    public partial class SRMMigrationUtility : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!IsPostBack)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string JSScript = "var SRMMigrationUtility = srmMigrationUtility.Initializer('" + SessionInfo.LoginName + "','" + serializer.Serialize(SessionInfo.CultureInfo) + "');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "SRMMigrationUtility", JSScript, true);
            }
        }
           }
}