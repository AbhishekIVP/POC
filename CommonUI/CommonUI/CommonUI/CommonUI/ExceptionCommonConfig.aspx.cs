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
    public partial class ExceptionCommonConfig : BasePage
    {
        protected override void OnPreRender(EventArgs e)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "ExceptionCommonConfig.init('" + serializer.Serialize(getExceptionConfigInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeExceptionCommonConfig", script, true);
            base.OnPreRender(e);
        }

        protected ExceptionCommonConfigInfo getExceptionConfigInfo (){
            ExceptionCommonConfigInfo info = new ExceptionCommonConfigInfo();
            if (Request.QueryString != null && !string.IsNullOrEmpty(Request.QueryString["moduleId"]) && !string.IsNullOrEmpty(Request.QueryString["instrumentTypeId"]))
            {
                info.moduleId = Int32.Parse(Request.QueryString["moduleId"]);
                info.instrumentTypeId = Int32.Parse(Request.QueryString["instrumentTypeId"]);
            }
            else {
                info.moduleId = -1;
                info.instrumentTypeId = -1;
            }
            info.UserName = SessionInfo.LoginName;
            return info;
        }
    }
    public class ExceptionCommonConfigInfo {
        public int moduleId { get; set; }
        public int instrumentTypeId { get; set; }
        public string UserName { get; set; }
    }
}