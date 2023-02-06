using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.culturemanager;
using com.ivp.rad.viewmanagement;
using com.ivp.rad.utils;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class UniquenessSetup : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo('" + serializer.Serialize(SessionInfo.CultureInfo) + "', '" + serializer.Serialize(RCultureController.GetServerCultureInfo()) + "'); uniquenessSetup.Initializer('" + serializer.Serialize(GetUniquenessSetupControlsInfo()) + "','" + serializer.Serialize(GetUniquenessSetupJSInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeUniquenessSetup", script, true);
            base.OnPreRender(e);
        }

        public UniquenessSetupControlsInfo GetUniquenessSetupControlsInfo()
        {
            UniquenessSetupControlsInfo controlInfo = new UniquenessSetupControlsInfo();

            controlInfo.PanelTopId = panelTop.ClientID;
            controlInfo.PanelMiddleId = panelMiddle.ClientID;
            controlInfo.PanelBottomId = panelBottom.ClientID;
            controlInfo.ModalSuccessId = modalSuccess.BehaviorID;
            controlInfo.ModalErrorId = modalError.BehaviorID;
            controlInfo.LblSuccessId = lblSuccess.ClientID;
            controlInfo.LblErrorId = lblError.ClientID;

            //Delete Popup
            controlInfo.ModalDeleteId = modalDelete.BehaviorID;
            controlInfo.LblDeleteId = lblDelete.ClientID;
            controlInfo.BtnDeleteYes = btnDeleteYES.ClientID;
            controlInfo.BtnDeleteNo = btnDeleteNO.ClientID;

            return controlInfo;
        }

        public UniquenessSetupJSInfo GetUniquenessSetupJSInfo()
        {
            UniquenessSetupJSInfo info = new UniquenessSetupJSInfo();
            info.UserName = SessionInfo.LoginName;
            //info.InstanceName = RConfigReader.GetConfigAppSettings("InstanceName");
            return info;
        }

    }

    public class UniquenessSetupControlsInfo
    {
        public string PanelTopId { get; set; }
        public string PanelMiddleId { get; set; }
        public string PanelBottomId { get; set; }
        public string ModalSuccessId { get; set; }
        public string ModalErrorId { get; set; }
        public string LblSuccessId { get; set; }
        public string LblErrorId { get; set; }
        //Delete Popup
        public string ModalDeleteId { get; set; }
        public string LblDeleteId { get; set; }
        public string BtnDeleteYes { get; set; }
        public string BtnDeleteNo { get; set; }
    }
    public class UniquenessSetupJSInfo
    {
        public string UserName { get; set; }
        //public string InstanceName { get; set; }
    }
}