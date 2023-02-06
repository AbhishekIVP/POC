using com.ivp.rad.utils;
using com.ivp.rad.viewmanagement;
using com.ivp.secm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMDownstreamSyncStatus : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
        }

        protected override void OnPreRender(EventArgs e) {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "syncStatus.init('" + serializer.Serialize(GetSRMDownstreamSyncStatusInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeCDODashboard", script, true);
            base.OnPreRender(e);
        }
        public SRMDownstreamSyncStatusInfo GetSRMDownstreamSyncStatusInfo()
        {
            SRMDownstreamSyncStatusInfo objDownstreamSyncStatusInfo = new SRMDownstreamSyncStatusInfo();
            objDownstreamSyncStatusInfo.UserName = SessionInfo.LoginName;
            objDownstreamSyncStatusInfo.LongDateFormat = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.LongDateFormat, SMDateTimeOptions.DATETIME);
            objDownstreamSyncStatusInfo.ShortDateFormat = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.ShortDateFormat, SMDateTimeOptions.DATE);
            objDownstreamSyncStatusInfo.RADLongDateFormat = SessionInfo.CultureInfo.LongDateFormat;
            objDownstreamSyncStatusInfo.RADShortDateFormat = SessionInfo.CultureInfo.ShortDateFormat;
            return objDownstreamSyncStatusInfo;
        }
    }
    public class SRMDownstreamSyncStatusInfo
    {
        public string UserName { get; set; }
        public string RADShortDateFormat { get; set; }
        public string ServerShortDateFormat { get; set; }
        public string LongDateFormat { get; set; }
        public string ShortDateFormat { get; set; }
        public string RADLongDateFormat { get; set; }
    }
}