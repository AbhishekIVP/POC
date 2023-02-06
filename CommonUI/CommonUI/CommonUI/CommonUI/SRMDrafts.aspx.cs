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
    public partial class SRMDrafts : BasePage
    {

        private string screenName = string.Empty;
        private string productName = string.Empty;
        private string selectedProduct = "secmaster";
        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string JSScript = "var os_serverSideValues = " + serializer.Serialize(GetDraftsStatusInfo()) + ";" + "var DraftsStatus = draftsStatus.init();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "DraftsStatus", JSScript, true);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

        }

        private DraftsStatusInfo GetDraftsStatusInfo()
        {
            DraftsStatusInfo info = new DraftsStatusInfo();
            info.Username = SessionInfo.LoginName;
            info.CultureShortDateFormat = SessionInfo.CultureInfo.ShortDateFormat;
            info.CultureLongDateFormat = SessionInfo.CultureInfo.LongDateFormat;
            info.FromScreenName = screenName == null ? "" : screenName;
            info.ProductName = RConfigReader.GetConfigAppSettings("ProductName");
            info.SelectedProduct = selectedProduct;
            if (Request.QueryString["isUserSpecific"] != null)
                info.isUserSpecific = Convert.ToBoolean(Request.QueryString["isUserSpecific"]);
            info.sectypeList = Convert.ToString(Request.QueryString["secTypeIds"]);
            if (Request.QueryString["dashboard"] != null)
                info.isFromDashboard = true;
            return info;
        }

        public class DraftsStatusInfo
        {
            public string Username { get; set; }
            public string FromScreenName { get; set; }
            public string ProductName { get; set; }
            public string SelectedProduct { get; set; }
            public string CultureShortDateFormat { get; set; }
            public string CultureLongDateFormat { get; set; }
            public bool isFromDashboard { get; set; }
            public string sectypeList { get; set; }
            public bool isUserSpecific { get; set; }
        }
    }
}