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
	public partial class CASourcePrioritization : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            base.Page_Load(sender, e);

        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "caSourcePrioritization.Initializer('" + serializer.Serialize(GetCASP_ControlsInfo()) + "','" + serializer.Serialize(GetCASP_JSInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeCASourcePrioritization", script, true);
            base.OnPreRender(e);
        }

        public CASP_ControlsInfo GetCASP_ControlsInfo()
        {
            CASP_ControlsInfo controlInfo = new CASP_ControlsInfo();

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

        public CASP_JSInfo GetCASP_JSInfo()
        {
            CASP_JSInfo info = new CASP_JSInfo();
            info.UserName = SessionInfo.LoginName;

            return info;
        }
    }
    
    public class CASP_ControlsInfo
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
    public class CASP_JSInfo
    {
        public string UserName { get; set; }
    }
}