using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.viewmanagement;
using System.Web.Script.Serialization;
namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class InstrumentEventConfig : BasePage
    {
        protected override void OnPreRender(EventArgs e)
        {
            InstrumentEventConfigData data = new InstrumentEventConfigData();
            data.username = SessionInfo.LoginName;
            data.instrumentTypeName = "";
            if (Request.QueryString != null && !string.IsNullOrEmpty(Request.QueryString["moduleId"])
                && !string.IsNullOrEmpty(Request.QueryString["instrumentTypeId"]) &&
                !string.IsNullOrEmpty(Request.QueryString["instrumentTypeName"]))
            {
                data.moduleId = Int32.Parse(Request.QueryString["moduleId"]);
                data.instrumentTypeId = Int32.Parse(Request.QueryString["instrumentTypeId"]);
                data.instrumentTypeName = Request.QueryString["instrumentTypeName"];
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "instrumentEventConfig.init('" + serializer.Serialize(data) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "instrumentEventConfigInitializer", script, true);
            base.OnPreRender(e);
        }

    }
    public class InstrumentEventConfigData
    {
        public int moduleId { get; set; }
        public int instrumentTypeId { get; set; }
        public string username { get; set; }
        public string instrumentTypeName { get; set; }
    }
}