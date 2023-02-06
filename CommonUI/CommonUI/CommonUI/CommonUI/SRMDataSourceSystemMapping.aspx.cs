using System;
using com.ivp.rad.culturemanager;
using com.ivp.rad.viewmanagement;
using com.ivp.rad.utils;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMDataSourceSystemMapping : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string script = "com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo('" + serializer.Serialize(SessionInfo.CultureInfo) + "', '" +
            //     serializer.Serialize(RCultureController.GetServerCultureInfo()) + "');";
            int moduleId = 0; //6 for ref, 3 for sec
            int typeId = 0; //17 for sec, 2 for ref
            int templateId = 0;
            int setFirstScreenBit = 0;

            if (!string.IsNullOrEmpty(Request.QueryString["moduleId"]))
            {
                moduleId = Convert.ToInt32(Request.QueryString["moduleId"]);
            }
            //type id
            if (!string.IsNullOrEmpty(Request.QueryString["typeId"]))
            {
                typeId = Convert.ToInt32(Request.QueryString["typeId"]);
            }
            //template id
            if (!string.IsNullOrEmpty(Request.QueryString["templateId"]))
            {
                templateId = Convert.ToInt32(Request.QueryString["templateId"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["setFirstScreenBit"]))
            {
                setFirstScreenBit = Convert.ToInt32(Request.QueryString["setFirstScreenBit"]);
            }
            string script = "var DataSourceToSystemWidget = dataSourceToSystemWidget.init('" + moduleId + "','" + typeId + "','" + templateId + "','" + setFirstScreenBit + "');";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
    this.Page.GetHashCode().ToString() + this.Page.ClientID, script, true);
        }
    }
}