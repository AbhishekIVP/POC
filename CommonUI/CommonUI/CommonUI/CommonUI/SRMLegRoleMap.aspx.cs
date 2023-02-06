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
    public partial class SRMLegRoleMap : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            int typeId = string.IsNullOrEmpty(Convert.ToString(Request.QueryString["typeId"]))? -1: Convert.ToInt32(Request.QueryString["typeId"]) ;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string JSScript = "srmLegRoleMap.Initializer('" + typeId + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SRMLegRoleMap", JSScript, true);
        }
    }
}