using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using com.ivp.rad.viewmanagement;
using com.ivp.rad.utils;
using System.Web.Script.Serialization;

namespace com.ivp.secm.ui
{
    public partial class SMMergeDuplicate : BasePage
    {

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            #region    JS VARIABLES
            string username = SessionInfo.LoginName;
            string presetValue = string.Empty;
            string productName = RConfigReader.GetConfigAppSettings("ProductName");
            #endregion

            if (Request.QueryString["identifier"] != null && Request.QueryString["identifier"] == "Dashboard")
            {
                if (Request.QueryString["sessionIdentifier"] != null)
                {
                    string sessionIdentifier = Request.QueryString["sessionIdentifier"];
                    presetValue = Convert.ToString(HttpContext.Current.Session[sessionIdentifier]);
                }
            }

            string jsQuery = "smMergeDuplicate._username = '" + username + "'; smMergeDuplicate._productName = '" + productName.ToLower() + "'; smMergeDuplicate._presetValue = '" + presetValue + "'; ";
            jsQuery += "smMergeDuplicate.preInit()";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SMMergeDupeSessionVariables", jsQuery, true);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            //#region    JS VARIABLES
            //    string username = SessionInfo.LoginName;
            //string presetValue = string.Empty;
            //    string productName = RConfigReader.GetConfigAppSettings("ProductName");
            //#endregion

            // if (Request.QueryString["identifier"] != null && Request.QueryString["identifier"] == "Dashboard")
            //    {
            //        if (Request.QueryString["sessionIdentifier"] != null)
            //        {
            //            string sessionIdentifier = Request.QueryString["sessionIdentifier"];
            //            presetValue = Convert.ToString(HttpContext.Current.Session[sessionIdentifier]);
            //        }
            //    }

            //    string jsQuery = "smMergeDuplicate._username = '" + username + "'; smMergeDuplicate._productName = '"+productName.ToLower()+"'; smMergeDuplicate._presetValue = '"+presetValue+"'; ";
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "SMMergeDupeSessionVariables", jsQuery, true);

        }

    }
}