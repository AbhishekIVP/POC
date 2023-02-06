using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.viewmanagement;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMRuleCatalogParent : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (Request.QueryString["id"] != null && !string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                hdnId.Value = Request.QueryString["id"];
            }
            if (Request.QueryString["moduleName"] != null && !string.IsNullOrEmpty(Request.QueryString["moduleName"]))
            {
                hdnModuleName.Value = Request.QueryString["moduleName"];
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "htmlBulkUploadGrid.Initializer('" + serializer.Serialize(GetHtmlBulkUploadGridControlsInfo()) + "','" + serializer.Serialize(GetHtmlBulkUploadGridJSInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeHtmlBulkUploadGrid", script, true);
            base.OnPreRender(e);
        }

        public HtmlBulkUploadGridControlsInfo GetHtmlBulkUploadGridControlsInfo()
        {
            HtmlBulkUploadGridControlsInfo controlInfo = new HtmlBulkUploadGridControlsInfo();

            //controlInfo.PanelTopId = panelTop.ClientID;
            //controlInfo.PanelMiddleId = panelMiddle.ClientID;
            controlInfo.PanelBottomId = panelBottom.ClientID;
            controlInfo.ModalSuccessId = modalSuccess.BehaviorID;
            controlInfo.ModalErrorId = modalError.BehaviorID;
            controlInfo.LblSuccessId = lblSuccess.ClientID;
            controlInfo.LblErrorId = lblError.ClientID;

            ////Delete Popup
            //controlInfo.ModalDeleteId = modalDelete.BehaviorID;
            //controlInfo.LblDeleteId = lblDelete.ClientID;
            //controlInfo.BtnDeleteYes = btnDeleteYES.ClientID;
            //controlInfo.BtnDeleteNo = btnDeleteNO.ClientID;

            return controlInfo;
        }

        public HtmlBulkUploadGridJSInfo GetHtmlBulkUploadGridJSInfo()
        {
            HtmlBulkUploadGridJSInfo info = new HtmlBulkUploadGridJSInfo();
            info.UserName = SessionInfo.LoginName;

            info.ModuleID = -10;
            info.TypeID = -10;

            if (Request.QueryString != null)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ModuleID"]))
                {
                    info.ModuleID = Convert.ToInt32(Request.QueryString["ModuleID"]);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["TypeID"]))
                {
                    info.TypeID = Convert.ToInt32(Request.QueryString["TypeID"]);
                }
            }

            return info;
        }
    }

    public class HtmlBulkUploadGridControlsInfo
    {
        //public string PanelTopId { get; set; }
        //public string PanelMiddleId { get; set; }
        public string PanelBottomId { get; set; }
        public string ModalSuccessId { get; set; }
        public string ModalErrorId { get; set; }
        public string LblSuccessId { get; set; }
        public string LblErrorId { get; set; }
        ////Delete Popup
        //public string ModalDeleteId { get; set; }
        //public string LblDeleteId { get; set; }
        //public string BtnDeleteYes { get; set; }
        //public string BtnDeleteNo { get; set; }
    }
    public class HtmlBulkUploadGridJSInfo
    {
        public string UserName { get; set; }
        public int ModuleID { get; set; }
        public int TypeID { get; set; }
    }
}