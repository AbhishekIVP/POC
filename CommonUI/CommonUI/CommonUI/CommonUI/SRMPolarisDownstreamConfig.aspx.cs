using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.common;
using com.ivp.rad.culturemanager;
using com.ivp.rad.RUserManagement;
using com.ivp.rad.transport;
using com.ivp.rad.viewmanagement;
using com.ivp.secm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMPolarisDownstreamConfig : BasePage
    {
        RTransportConfigController objTransportConfigController = null;
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMPolarisDownstreamConfig");
        public class SRMDownstreamSyncInfo
        {
            public string CustomJQueryDateTimeFormat { get; set; }
            public string CustomJQueryDateFormat { get; set; }
            public string ServerShortDateFormat { get; set; }
            public string ServerLongDateFormat { get; set; }
        }
        private SRMDownstreamSyncInfo GetCommonStatusInfo()
        {
            SRMDownstreamSyncInfo securityInfo = new SRMDownstreamSyncInfo();
            securityInfo.CustomJQueryDateTimeFormat = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.LongDateFormat, SMDateTimeOptions.DATETIME);
            securityInfo.CustomJQueryDateFormat = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.ShortDateFormat, SMDateTimeOptions.DATE);
            securityInfo.ServerShortDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            securityInfo.ServerLongDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern;
            return securityInfo;
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void OnPreRender(EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "polarisdownstream.Initializer('" + serializer.Serialize(GetCommonStatusInfo()) + "','" + JsonConvert.SerializeObject(BindTransportTypes()) + "', '" + JsonConvert.SerializeObject(BindCalendarTypes()) + "','" + GetSubscriberList() + "');";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitialize", script, true);
        }
        private Dictionary<int, string> BindCalendarTypes()
        {
            mLogger.Debug("SRMPolarisDownstreamConfig : BindCalendarTypes -> Start");
            RCalendarController objCalController = null;
            try
            {
                objCalController = new RCalendarController();
                DataSet ds = objCalController.GetAllCalendarsSorted();

                Dictionary<int, string> calendarIdVsCalendarName = new Dictionary<int, string>();
                calendarIdVsCalendarName = ds.Tables[0].AsEnumerable()
                                          .ToDictionary<DataRow, int, string>(row => row.Field<int>("calendar_id"),
                                           row => row.Field<string>("calendar_name"));
                return calendarIdVsCalendarName;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMPolarisDownstreamConfig : BindCalendarTypes -> Error " + ex.Message);
                throw ex;
            }
            finally
            {
                objCalController = null;
                mLogger.Debug("SRMPolarisDownstreamConfig : BindCalendarTypes -> End");
            }

        }
        public string GetSubscriberList()
        {
            mLogger.Debug("SRMPolarisDownstreamConfig : GetSubscriberList -> Start");
            Dictionary<string, List<string>> lstEmailId = new Dictionary<string, List<string>>();
            try
            {
                RUserManagementService objUserController = new RUserManagementService();
                List<RUserInfo> dsUsers = objUserController.GetAllUsersGDPR();
                if (dsUsers != null && dsUsers.Count > 0)
                    lstEmailId = dsUsers.Where(r => !string.IsNullOrEmpty(r.EmailId)).GroupBy(y => y.EmailId).ToDictionary(a => a.Key, t => t.Select(x => x.FullName).ToList());
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMPolarisDownstreamConfig : GetSubscriberList -> Error " + ex.Message);
                throw new Exception(ex.Message, ex);
            }
            mLogger.Debug("SRMPolarisDownstreamConfig : GetSubscriberList -> End");
            return new JavaScriptSerializer().Serialize(lstEmailId);
        }
        protected Dictionary<int, string> BindTransportTypes()
        {
            mLogger.Debug("SRMPolarisDownstreamConfig : BindTransportTypes -> Start");

            try
            {
                objTransportConfigController = new RTransportConfigController();
                DataSet ds = RTransportConfigLoader.GetAllTransports();
                var filteredTransportsRows = ds.Tables[0].AsEnumerable()
                                                              .Where(r => r.Field<string>("transport_type_name").Equals("MSMQ", StringComparison.OrdinalIgnoreCase) || r.Field<string>("transport_type_name").Equals("WMQ", StringComparison.OrdinalIgnoreCase) || r.Field<string>("transport_type_name").Equals("RabbitMQ", StringComparison.OrdinalIgnoreCase) || r.Field<string>("transport_type_name").Equals("KafkaMQ", StringComparison.OrdinalIgnoreCase));

                Dictionary<int, string> transportDetailsIdVsTransportName = new Dictionary<int, string>();
                if (filteredTransportsRows.Any())
                {
                    DataTable filteredTransportsDt = filteredTransportsRows.CopyToDataTable();
                    transportDetailsIdVsTransportName = filteredTransportsDt.AsEnumerable()
                                                        .ToDictionary<DataRow, int, string>(row => row.Field<int>("transport_details_id"),
                                                        row => row.Field<string>("transport_name"));
                }
                return transportDetailsIdVsTransportName;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMPolarisDownstreamConfig : BindTransportTypes -> Error " + ex.Message);
                throw ex;
            }
            finally
            {
                objTransportConfigController = null;
                mLogger.Debug("SRMPolarisDownstreamConfig : BindTransportTypes -> End");
            }
        }
    }
}