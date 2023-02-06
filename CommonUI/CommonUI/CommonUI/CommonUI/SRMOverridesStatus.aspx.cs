using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using com.ivp.secm.ui;
using System.Web.Script.Serialization;
using System.Configuration;
using com.ivp.rad.utils;
using com.ivp.rad.controls.xlgrid.service;
using com.ivp.rad.viewmanagement;
using System.Globalization;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMOverridesStatus : BasePage
    {
        private string screenName = string.Empty;
        private string productName = string.Empty;
        private string selectedProduct = "secmaster";
        private OverrideStatusInfo info;
        private int moduleId;
        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //Session["SetOveridesSecIDsOrEntityCodes"] = new string[2] { "ISIV00001", "ISIV00002" };
            //Session["SetOveridesSecIDsOrEntityCodes"] = new List<string>() { "EQST000001|1", "EQST000002|1" };
            string JSScript = "var os_serverSideValues = " + serializer.Serialize(GetOverrideStatusInfo()) + "; overrideStatus._entityTypeID= " + (serializer.Serialize(this.Request.QueryString["entityTypeId"])) + ";";
            if (screenName.Equals("search"))
            {
                JSScript += " overrideStatus._selectedSecIDsOrEntityCodesFromSearch = " + (serializer.Serialize(GetSecIDsOrEntityCodes())) + ";";
            }

            JSScript += " var OverridesStatus = overrideStatus.init();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "OverrideStatus", JSScript, true);
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            string s = this.Request.QueryString["screenName"];
            screenName = (s == null ? string.Empty : s);

            string urlProductName = this.Request.QueryString["productName"];
            string urlSelectedProduct = this.Request.QueryString["product"]; // For selecting secmaster if this screen is opened from RM Dashboard
            if (!String.IsNullOrEmpty(urlSelectedProduct))
            {
                selectedProduct = urlSelectedProduct;
            }

            if (!String.IsNullOrEmpty(urlProductName) && !String.IsNullOrEmpty(screenName))
            {
                productName = urlProductName;
            }
            else
            {
                productName = RConfigReader.GetConfigAppSettings("ProductName");
            }
            string moduleidFromQueryString = this.Request.QueryString["moduleId"];
            if (!String.IsNullOrEmpty(moduleidFromQueryString))
            {
                moduleId = Convert.ToInt32(moduleidFromQueryString);
            }
        }

        //private OverrideStatusControlInfo GetOverrideStatusControlInfo()
        //{
        //    OverrideStatusControlInfo controlInfo = new OverrideStatusControlInfo();
        //    controlInfo.TopPanel = panelTop.ClientID;
        //    controlInfo.MiddlePanel = panelMiddle.ClientID;
        //    controlInfo.BottomPanel = panelBottom.ClientID;
        //    return controlInfo;
        //}
        //getting the same from the master page



        private OverrideStatusInfo GetOverrideStatusInfo()
        {
            info = new OverrideStatusInfo();
            info.Username = SessionInfo.LoginName;
            info.CultureShortDateFormat = SessionInfo.CultureInfo.ShortDateFormat;
            info.CultureLongDateFormat = SessionInfo.CultureInfo.LongDateFormat;
            info.FromScreenName = screenName == null ? "" : screenName;
            info.ProductName = productName;
            info.SelectedProduct = selectedProduct;
            info.ServerLongDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
            info.ServerShortDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            info.ModuleID = moduleId;
            return info;
        }

        private List<string> GetSecIDsOrEntityCodes()
        {
            if (!string.IsNullOrEmpty(info.FromScreenName))
            {
                if (!String.IsNullOrEmpty(Convert.ToString(this.Request.QueryString["searchGridCachekey"])))
                {
                    List<string> entCodes = null;
                    entCodes = GetEntityCodes(Convert.ToString(this.Request.QueryString["searchGridCachekey"]));
                    if (entCodes.Count == 0)
                    {
                        entCodes = GetEntityCodesFromQuery(Convert.ToString(this.Request.QueryString["entityCode"]).Split(','));
                    }
                    return entCodes;
                }
                else
                {
                    return GetSecIDs((String[])Session["SetOveridesSecIDsOrEntityCodes"]);
                }
            }
            else
            {
                return null;
            }
        }

        private List<string> GetSecIDs(String[] secids)
        {
            List<string> secIDList = new List<string>();
            foreach (string item in secids)
            {
                secIDList.Add(item);
            }
            return secIDList;
        }

        private List<string> GetEntityCodesFromQuery(String[] entityCode)
        {
            List<string> entCode = new List<string>();
            foreach (string item in entityCode)
            {
                entCode.Add(item);
            }
            return entCode;
        }

        private List<string> GetEntityCodes(string cacheKey)
        {
            RADXLGridService service = new RADXLGridService();
            List<string> entityCodes = service.GetCheckedRowKeys(cacheKey);
            return entityCodes;
        }
    }

    //public class OverrideStatusControlInfo
    //{
    //    public string TopPanel { get; set; }
    //    public string MiddlePanel { get; set; }
    //    public string BottomPanel { get; set; }
    //}

    public class OverrideStatusInfo
    {
        public string Username { get; set; }
        public string FromScreenName { get; set; }
        public string ProductName { get; set; }
        public string SelectedProduct { get; set; }
        public string CultureShortDateFormat { get; set; }
        public string CultureLongDateFormat { get; set; }
        public string ServerShortDateFormat { get; set; }
        public string ServerLongDateFormat { get; set; }
        public int ModuleID { get; set; }
    }
}
