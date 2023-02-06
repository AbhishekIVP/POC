using com.ivp.rad.culturemanager;
using com.ivp.rad.viewmanagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMMaster : BaseMasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string script = "com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo('" + serializer.Serialize(SessionInfo.CultureInfo) + "', '" +
                   serializer.Serialize(RCultureController.GetServerCultureInfo()) + "');";
            string specificModuleId = string.Empty;
            string setCustomText = string.Empty;
            string selectedModuleToDisplay = string.Empty;

            //sessionModuleId for fund party login and making only RFM modules available
            //string sessionModuleId = Convert.ToString(Session["ModuleId"]);

            //selectedModuleToDisplay selects the moduleId selected from drilldowns
            if (!string.IsNullOrEmpty(Request.QueryString["moduleId"]))
            {
                selectedModuleToDisplay = Request.QueryString["moduleId"];
            }
            else {
                selectedModuleToDisplay = "0";
            }

            if (!string.IsNullOrEmpty(Request.QueryString["specificModuleId"]))
                specificModuleId = Request.QueryString["specificModuleId"];
            else
                specificModuleId = "-1";
            if (!string.IsNullOrEmpty(Request.QueryString["CustomText"]))
                setCustomText = Request.QueryString["CustomText"];
            else
                setCustomText = "";
            script += "var SRMProductTabs = CommonModuleTabs.SetSRMProductTabs('" + SessionInfo.LoginName + "','" + Request.QueryString["identifier"] + "','" + specificModuleId + "','" + setCustomText + "','" + selectedModuleToDisplay + "'); ";

            script += "var os_serverSideControls =" + serializer.Serialize(GetMasterControlInfo()) + ";";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
                this.Page.GetHashCode().ToString() + this.Page.ClientID, script, true);
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "SRMProductTabs", "var SRMProductTabs = CommonModuleTabs.SetSRMProductTabs();", true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "SRMProductTabs", "var SRMProductTabs = CommonModuleTabs.SetSRMProductTabs();", true);
            //base.OnPreRender(e);
        }


        private MasterControlInfo GetMasterControlInfo()
        {
            MasterControlInfo controlInfo = new MasterControlInfo();
            controlInfo.TopPanel = SRMPanelTop.ClientID;
            controlInfo.MiddlePanel = SRMPanelMiddle.ClientID;
            controlInfo.BottomPanel = SRMPanelBottom.ClientID;
            return controlInfo;
        }


    }

    public class MasterControlInfo
    {
        public string TopPanel { get; set; }
        public string MiddlePanel { get; set; }
        public string BottomPanel { get; set; }
    }


}