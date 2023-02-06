using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Data;
using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.viewmanagement;
using com.ivp.secm;
using com.ivp.rad.utils;

namespace com.ivp.common.CommonUI.CommonUI
{

    public partial class SRMRealTimeStatus : BasePage
    {

        #region Declarations
        DataSet mDsSystems = null;

        public class SRMRealTimeStatusControlInfo
        {
            // public string DivExceptionDateId { get; set; }
            public string HdnExceptionDateId { get; set; }
            public string LabeldateselectiondivId { get; set; }
            // public string TxtTimerId { get; set; }
            public string BtnGetStatusId { get; set; }
            // public string HdnTimerId { get; set; }
            public string RblstDateId { get; set; }
            public string LblStartDateId { get; set; }
            public string TxtStartDateId { get; set; }
            public string LblStartDateErrorId { get; set; }
            public string LblEndDateId { get; set; }
            public string TxtEndDateId { get; set; }
            public string LblEndDateErrorId { get; set; }
            public string LblValidationSummaryId { get; set; }
            public string ProductName { get; set; }

            public string UserName { get; set; }
        }

        public class SRMRealTimeStatusInfo
        {
            public string CustomJQueryDateFormat { get; set; }
            public string CustomJQueryDateTimeFormat { get; set; }
        }
        #endregion

        #region INFO AND CONTROL IDS TO BE SEND TO SCRIPTS
        private SRMRealTimeStatusInfo GetCommonRealTimeStatusInfo()
        {
            SRMRealTimeStatusInfo securityInfo = new SRMRealTimeStatusInfo();
            securityInfo.CustomJQueryDateTimeFormat = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.LongDateFormat, SMDateTimeOptions.DATETIME);
            securityInfo.CustomJQueryDateFormat = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.ShortDateFormat, SMDateTimeOptions.DATE);
            return securityInfo;
        }
        private SRMRealTimeStatusControlInfo GetCommonRealTimeStatusControlInfo()
        {
            SRMRealTimeStatusControlInfo controlInfo = new SRMRealTimeStatusControlInfo();
            controlInfo.LabeldateselectiondivId = labeldateselectiondiv.ClientID;
            //controlInfo.DivExceptionDateId = divExceptionDate.ClientID;
            //controlInfo.HdnExceptionDateId = hdnExceptionDate.ClientID;
            //controlInfo.TxtTimerId = txtTimer.ClientID;
            controlInfo.BtnGetStatusId = btnGetStatus.ClientID;
            //controlInfo.HdnTimerId = hdnTimer.ClientID;
            //controlInfo.RblstDateId = rblstDate.ClientID;
            //controlInfo.LblStartDateId = lblStartDate.ClientID;
            controlInfo.TxtStartDateId = TextStartDate.ClientID;
            //controlInfo.LblStartDateErrorId = lblStartDateError.ClientID;
            //controlInfo.LblEndDateId = lblEndDate.ClientID;
            controlInfo.TxtEndDateId = TextEndDate.ClientID;
            //controlInfo.LblEndDateErrorId = lblEndDateError.ClientID;
            //controlInfo.LblValidationSummaryId = lblValidationSummary.ClientID;
            controlInfo.ProductName = RConfigReader.GetConfigAppSettings("ProductName");
            controlInfo.UserName = SessionInfo.LoginName;
            return controlInfo;
        }
        #endregion
        
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            hdnLongDateFormat.Value = SessionInfo.CultureInfo.LongDateFormat;

            if (!string.IsNullOrEmpty(Request.QueryString["typeId"]))
                hdnTypeId.Value = Request.QueryString["typeId"];

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "var objSMSSRMRealTimeStatusScreen = new SMSSRMRealTimeStatusScreen('" + serializer.Serialize(GetCommonRealTimeStatusInfo()) + "','" + serializer.Serialize(GetCommonRealTimeStatusControlInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), (this.GetType().GetHashCode() + 123).ToString(), script, true);

            bool isPostBack = IsPostBack;
            if (!isPostBack)
            {
                TextStartDate.Text = RCalenderUtils.FormatDate(DateTime.Today, SessionInfo.CultureInfo.LongDateFormat);
                //TextEndDate.Text = RCalenderUtils.FormatDate(DateTime.Now, SessionInfo.CultureInfo.LongDateFormat);
                TextEndDate.Text = (RCalenderUtils.FormatDate(DateTime.Now, SessionInfo.CultureInfo.ShortDateFormat)).Split(' ')[0];

            }
        }
    }
}