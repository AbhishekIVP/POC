using com.ivp.rad.culturemanager;
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
    public partial class ApiMonitoring : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //string script = "com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo('" + serializer.Serialize(SessionInfo.CultureInfo) + "', '" + serializer.Serialize(RCultureController.GetServerCultureInfo()) + "'); apimonitoring.Initializer('" + serializer.Serialize(GetApiMonitoringControlsInfo()) + "');";
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeDashboard", script, true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo('" + serializer.Serialize(SessionInfo.CultureInfo) + "', '" + serializer.Serialize(RCultureController.GetServerCultureInfo()) + "'); apimonitoring.Initializer('" + serializer.Serialize(GetApiMonitoringControlsInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeDashboard", script, true);
            base.OnPreRender(e);
        }

        public ApiMonitoringControlsInfo GetApiMonitoringControlsInfo()
        {
            ApiMonitoringControlsInfo controlInfo = new ApiMonitoringControlsInfo();

            controlInfo.PanelTopId = panelTop.ClientID;
            controlInfo.PanelMiddleId = panelMiddle.ClientID;
            controlInfo.PanelBottomId = panelBottom.ClientID;
            //controlInfo.ModalSuccessId = modalSuccess.BehaviorID;
            controlInfo.ModalErrorId = modalError.BehaviorID;
            //controlInfo.LblSuccessId = lblSuccess.ClientID;
            controlInfo.LblErrorId = lblError.ClientID;

            return controlInfo;
        }
    }

    public class ApiMonitoringControlsInfo
    {
        public string PanelTopId { get; set; }

        public string PanelMiddleId { get; set; }

        public string PanelBottomId { get; set; }

        //public string ModalSuccessId { get; set; }

        public string ModalErrorId { get; set; }

        //public string LblSuccessId { get; set; }

        public string LblErrorId { get; set; }
    }
}