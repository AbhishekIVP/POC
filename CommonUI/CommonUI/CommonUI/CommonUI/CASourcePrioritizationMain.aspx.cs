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
    public partial class CASourcePrioritizationMain : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "caSourcePrioritizationMain.Initializer('" + serializer.Serialize(GetCASPM_ControlsInfo()) + "','" + serializer.Serialize(GetCASPM_JSInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeCASourcePrioritizationMain", script, true);
            base.OnPreRender(e);
        }

        public CASPM_ControlsInfo GetCASPM_ControlsInfo()
        {
            CASPM_ControlsInfo controlInfo = new CASPM_ControlsInfo();

            controlInfo.PanelTopId = panelTop.ClientID;
            controlInfo.PanelMiddleId = panelMiddle.ClientID;
            controlInfo.PanelBottomId = panelBottom.ClientID;
            controlInfo.ModalSuccessId = modalSuccess.BehaviorID;
            controlInfo.ModalErrorId = modalError.BehaviorID;
            controlInfo.LblSuccessId = lblSuccess.ClientID;
            controlInfo.LblErrorId = lblError.ClientID;

            //Delete Popup
            //controlInfo.ModalDeleteId = modalDelete.BehaviorID;
            //controlInfo.LblDeleteId = lblDelete.ClientID;
            //controlInfo.BtnDeleteYes = btnDeleteYES.ClientID;
            //controlInfo.BtnDeleteNo = btnDeleteNO.ClientID;

            return controlInfo;
        }

        public CASPM_JSInfo GetCASPM_JSInfo()
        {
            CASPM_JSInfo info = new CASPM_JSInfo();
            info.UserName = SessionInfo.LoginName;

            return info;
        }
    }

    public class CASPM_ControlsInfo
    {
        public string PanelTopId { get; set; }
        public string PanelMiddleId { get; set; }
        public string PanelBottomId { get; set; }
        public string ModalSuccessId { get; set; }
        public string ModalErrorId { get; set; }
        public string LblSuccessId { get; set; }
        public string LblErrorId { get; set; }
        //Delete Popup
        //public string ModalDeleteId { get; set; }
        //public string LblDeleteId { get; set; }
        //public string BtnDeleteYes { get; set; }
        //public string BtnDeleteNo { get; set; }
    }
    public class CASPM_JSInfo
    {
        public string UserName { get; set; }
    }

}