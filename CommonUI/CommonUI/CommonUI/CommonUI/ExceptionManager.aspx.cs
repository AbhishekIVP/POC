using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.viewmanagement;
using System.Web.Script.Serialization;
using com.ivp.secm;
using com.ivp.rad.culturemanager;
using com.ivp.rad.utils;
using System.Globalization;

namespace com.ivp.common.exceptionManagerUI
{
    public partial class ExceptionManagerUI : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            //string productName = RConfigReader.GetConfigAppSettings("ProductName");
            //if (productName.ToLower().Equals("refmaster"))
            //{
            //    string scr = "$('#systemsExceptionButton').removeClass('selectedTabClass');$('#tabToggleDiv').css('display','none');$('#refMasterExceptionButton').addClass('selectedTabClass');";
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitialize", scr, true);
            //}

            //Added ModuleId
            //int ModuleID = 0;
            //if (!string.IsNullOrEmpty(Request.QueryString["ModuleID"]))
            //{
            //    ModuleID = Convert.ToInt32(Request.QueryString["ModuleID"]);
            //}

            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //string script = "exceptionManager.preInit('" + serializer.Serialize(GetExceptionManagerUIInfo()) + "');";
            ////string script = "exceptionManager.preInit('" + serializer.Serialize(GetExceptionManagerUIInfo(productName, ModuleID)) + "');";
            //script += "com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo('" + serializer.Serialize(SessionInfo.CultureInfo) + "', '" +
            //       serializer.Serialize(RCultureController.GetServerCultureInfo()) + "');";
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeExceptionManager", script, true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "exceptionManager.preInit('" + serializer.Serialize(GetExceptionManagerUIInfo()) + "');";            
            script += "com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo('" + serializer.Serialize(SessionInfo.CultureInfo) + "', '" +
                   serializer.Serialize(RCultureController.GetServerCultureInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeExceptionManager", script, true);
        }

        public ExceptionManagerUIInfo GetExceptionManagerUIInfo()
        {
            ExceptionManagerUIInfo objExceptionManagerUIInfo = new ExceptionManagerUIInfo();
            objExceptionManagerUIInfo.UserName = SessionInfo.LoginName;
            objExceptionManagerUIInfo.LongDateFormat = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.LongDateFormat, SMDateTimeOptions.DATETIME);
            objExceptionManagerUIInfo.ShortDateFormat = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.ShortDateFormat, SMDateTimeOptions.DATE);
            objExceptionManagerUIInfo.RADLongDateFormat = SessionInfo.CultureInfo.LongDateFormat;
            objExceptionManagerUIInfo.RADShortDateFormat = SessionInfo.CultureInfo.ShortDateFormat;
            objExceptionManagerUIInfo.CultureShortDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            //objExceptionManagerUIInfo.ProductName = productName;
            ////Added ModuleID
            //objExceptionManagerUIInfo.ModuleID = ModuleID;

            return objExceptionManagerUIInfo;
        }
    }

    public class ExceptionManagerUIInfo
    {
        public string UserName { get; set; }
        public string LongDateFormat { get; set; }
        public string ShortDateFormat { get; set; }
        public string RADShortDateFormat { get; set; }
        public string RADLongDateFormat { get; set; }
        public string ProductName { get; set; }
        public string CultureShortDateFormat { get; set; }
        //Added ModuleId
        //public int ModuleID { get; set; }

    }

    [Serializable]
    public class FilterInfoForException
    {
        public string ExceptionFilter { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionState { get; set; }
        public string ExceptionDate { get; set; }
        public string ExceptionSectype { get; set; }
        public string ExceptionAttribute { get; set; }
        public string CustomDateSelected { get; set; }
        public string SecurityIds { get; set; }
        public string DetailsType { get; set; }
        public bool ShowFilter { get; set; }
        public string ParentPageIdentifier { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string UserName { get; set; }
        public string TagName { get; set; }
        public string ExceptionEntityType { get; set; }
        public string EntityCodes { get; set; }
        public string viewUserSpecific { get; set; }
        public int ModuleId { get; set;  }
    }

    public enum SMExceptionDateType
    {
        Today = 0,
        SinceYesterday = 1,
        ThisWeek = 2,
        AnyTime = 3,
        CustomDate = 4
    }

    public enum SMExceptionDetailType
    {
        Details = 0,
        Count = 1
    }

    public enum SMExceptionFilterType
    {
        All = 15,
        UnSuppressed = 1,
        Suppressed = 2,
        Unresolved = 4,
        Resolved = 8

    }

    public enum RMExceptionDateType
    {
        Today = 0,
        SinceYesterday = 1,
        ThisWeek = 2,
        AnyTime = 3,
        CustomDate = 4
    }

    public enum RMExceptionDetailType
    {
        Details = 0,
        Count = 1
    }

    public enum RMExceptionFilterType
    {
        All = 15,
        UnSuppressed = 1,
        Suppressed = 2,
        Unresolved = 4,
        Resolved = 8

    }
}
