using com.ivp.commom.commondal;
using com.ivp.common.srmdwhjob;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
//using static com.ivp.common.srmdwhjob.SRMDownstreamConfiguration;

namespace com.ivp.common.srmdownstreamcontroller
{
    public class SRMDownstreamStatusAndScheduler
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMDownstreamStatusAndScheduler");
        public static readonly Dictionary<string, string> DaysOfWeekStringToNumber = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Sunday", "1" }, { "Monday", "2" }, { "Tuesday", "4" }, { "Wednesday", "8" }, { "Thursday", "16" }, { "Friday", "32" }, { "Saturday", "64" } };
        public static readonly Dictionary<string, string> DaysOfWeekNumberToString = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "0", "Sunday" }, { "1", "Monday" }, { "2", "Tuesday" }, { "3", "Wednesday" }, { "4", "Thursday" }, { "5", "Friday" }, { "6", "Saturday" } };

        public static List<string> ExtractDaysOfWeek(string scheduledJob)
        {
            List<string> dayOfWeek = new List<string>();
            int valDaysOfWeek = Convert.ToInt32(scheduledJob);
            int count = 0;

            int fixedVal = 64;
            int itemNumber = 6;
            while (valDaysOfWeek > 0)
            {
                if (valDaysOfWeek >= fixedVal)
                {
                    dayOfWeek.Add(itemNumber.ToString());
                    count++;
                    valDaysOfWeek -= fixedVal;
                }
                fixedVal = fixedVal / 2;
                itemNumber -= 1;
            }
            dayOfWeek.Reverse();
            return dayOfWeek;
        }

        public static List<DownstreamSyncReportMessage> GetReportStatusMessage(List<int> blockStatusId, string resultDateTimeFormat)
        {
            mLogger.Error("GetReportStatusMessage -----> START");
            List<DownstreamSyncReportMessage> messages = new List<DownstreamSyncReportMessage>();
            DownstreamSyncReportMessage result;
            try
            {
                string query = "SELECT detail.block_status_id, detail.message,detail.created_on FROM dbo.ivp_srm_dwh_downstream_block_status_detail detail where is_active = 1 AND detail.block_status_id IN (" + string.Join(",", blockStatusId) + ") order by detail.block_status_id,detail.created_on";
                DataTable dt = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    result = new DownstreamSyncReportMessage();
                    result.block_status_id = int.Parse(row["block_status_id"].ToString());
                    result.message = row["message"].ToString();
                    result.created_on = ((DateTime)row["created_on"]).ToString(resultDateTimeFormat);
                    messages.Add(result);
                }
            }
            catch (Exception e)
            {
                mLogger.Error("GetReportStatusMessage -----> Error " + e.ToString());
                throw e;
            }
            mLogger.Error("GetReportStatusMessage -----> end");
            return messages;
        }
        public static List<DownstreamSyncStatusResult> GetAllDownstreamSyncStatus(string startDate, string endDate, string dateFormat, string resultDateTimeFormat, string selDateOption,string CustomRadioOption)
        {
            mLogger.Error("GetAllDownstreamSyncStatus -----> START");
            List<DownstreamSyncStatusResult> results;
            //DateTime? dtStart = null;
            //DateTime? dtEnd = null;
            string dtStart = null;
            string dtEnd = null;

            switch (selDateOption)
            {
                case "0": // Today
                    dtStart = DateTime.Now.ToString("MM-dd-yyy");
                    //whereConditions = "where cast([start_time] as datetime) >= cast('" + txtStartDate.Text + "' as datetime) and cast (start_time as datetime) <= cast('" + txtEndDate.Text + " 23:59:59" + "' as datetime)";
                    // changing 'enddate check' to starttime check for enabling inprogress task to be visible on task status screen.
                    break;
                case "1": // Since Yesterday
                    dtStart = DateTime.Now.AddDays(-1).ToString("MM-dd-yyy");
                    break;
                case "2":// This Week
                    dtStart = startDate;
                    break;
                case "3": // Anytime
                    break;
                case "4": // Custom Date
                    if (CustomRadioOption == "0") // From
                        dtStart = startDate;
                    else if (CustomRadioOption == "1") // Between
                    {
                        dtStart = startDate;

                        dtEnd = endDate;
                    }
                    else if (CustomRadioOption == "2") // Prior 
                        dtEnd = endDate;
                    break;
                default:
                    break;

            }

            //if (!string.IsNullOrEmpty(startDate))
            //{
            //    dtStart = DateTime.ParseExact(startDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
            //}

            //if (!string.IsNullOrEmpty(endDate))
            //{
            //    DateTime dt = DateTime.ParseExact(endDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
            //    dtEnd = dt.AddDays(1);
            //}
            try
            {
                string dtquery = "";
                bool isSec = SRMDownstreamConfiguration.CheckDatabaseExists("IVPSecMaster");
                string query = string.Format(@"
                                SELECT	task_name.setup_name        AS 'setup_name', 
                                setup.setup_id, 
                                setup.setup_status_id, 
                                setup.failure_reason        AS 'setup_failure_message', 
                                setup.status                AS 'setup_status', 
                                setup.start_time            AS 'setup_start_time', 
                                setup.end_time              AS 'setup_end_time', 
                                block_status.block_status_id, 
                                block_status.status         AS 'report_status', 
                                block_status.start_time     AS 'report_start_time', 
                                block_status.end_time       AS 'report_end_time', 
                                block_status.failure_reason AS 'report_failure_message',
                                {0}

                                FROM dbo.ivp_srm_dwh_downstream_setup_status setup         
		                        INNER JOIN dbo.ivp_srm_dwh_downstream_master task_name 
                                on (task_name.setup_id = setup.setup_id)       
		                        LEFT JOIN dbo.ivp_srm_dwh_downstream_block_status block_status
                                ON (setup.setup_status_id = block_status.setup_status_id AND block_status.is_active = 1)
                                LEFT join dbo.ivp_srm_dwh_downstream_details downstream_detail
                                ON(downstream_detail.setup_id = setup.setup_id AND downstream_detail.block_id = block_status.block_id AND downstream_detail.is_active = 1)

                                LEFT JOIN dbo.ivp_refm_report refm_report
                                ON refm_report.report_id = downstream_detail.report_id and refm_report.is_active = 1
                                {1}
                                WHERE setup.is_active = 1 and task_name.is_active = 1",
                                isSec? @"
                                CASE 
                                    WHEN downstream_detail.is_ref = 1 THEN refm_report.report_name 
                                    ELSE secm_report.report_name 
                                END                         AS 'report_name' ": " refm_report.report_name AS 'report_name' ",
                                isSec? " LEFT JOIN IVPSecMaster.dbo.ivp_secm_report_setup secm_report on secm_report.report_setup_id = downstream_detail.report_id AND secm_report.is_active = 1" : "");
                //if (dtStart != null || dtEnd != null)
                //{

                //    if (dtStart != null)
                //    {
                //        dtquery += " AND setup.start_time>='" + dtStart.Value.ToString("yyyy-MM-dd") + "'";
                //    }
                //    if (dtEnd != null)
                //    {
                //        dtquery += " AND ((setup.end_time<'" + dtEnd.Value.ToString("yyyy-MM-dd") + "' AND setup.end_time is not null) OR setup.end_time is null)";
                //    }

                //}

                string whereConditions = "";
                if (dtStart != null || dtEnd != null)
                {
                    whereConditions = " and ( 1=1 ";
                    if (dtStart != null)
                    {
                        whereConditions += "AND (cast(setup.end_time as datetime)>= cast ('" + dtStart + "' as datetime) OR setup.end_time is null)";
                    }
                    if (dtEnd != null)
                    {
                        whereConditions += "AND cast(setup.start_time as date)<= cast ('" + dtEnd + "' as date) ";
                    }
                    whereConditions += ") ";
                }

                query += whereConditions;
                query += @"
                        ORDER BY setup.start_time DESC, block_status.start_time";

                DataTable dt = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                results = new List<DownstreamSyncStatusResult>();

                DownstreamSyncStatusResult result = new DownstreamSyncStatusResult();
                result.reports = new List<DownstreamSyncReportResult>();
                result.setup_status_id = -1;
                DownstreamSyncReportResult reportResult;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (result.setup_status_id != -1 && Int32.Parse(row["setup_status_id"].ToString()) != result.setup_status_id)
                        {
                            results.Add(result);
                            result = new DownstreamSyncStatusResult();
                            result.setup_status_id = Int32.Parse(row["setup_status_id"].ToString());
                            result.SetupName = row["setup_name"].ToString();
                            result.StartTime = ((DateTime)row["setup_start_time"]).ToString(resultDateTimeFormat);
                            if (row["setup_end_time"] != DBNull.Value)
                                result.EndTime = ((DateTime)row["setup_end_time"]).ToString(resultDateTimeFormat);
                            else
                                result.EndTime = "";
                            result.Status = row["setup_status"].ToString();
                            result.reports = new List<DownstreamSyncReportResult>();
                            if (row["setup_failure_message"] != DBNull.Value)
                                result.FailureMessage = row["setup_failure_message"].ToString();
                            else
                                result.FailureMessage = "";
                        }

                        if (result.setup_status_id == -1)
                        {
                            result.setup_status_id = Int32.Parse(row["setup_status_id"].ToString());
                            result.SetupName = row["setup_name"].ToString();
                            result.StartTime = ((DateTime)row["setup_start_time"]).ToString(resultDateTimeFormat);
                            if (row["setup_end_time"] != DBNull.Value)
                                result.EndTime = ((DateTime)row["setup_end_time"]).ToString(resultDateTimeFormat);
                            else
                                result.EndTime = "";
                            result.Status = row["setup_status"].ToString();
                            if (row["setup_failure_message"] != DBNull.Value)
                                result.FailureMessage = row["setup_failure_message"].ToString();
                            else
                                result.FailureMessage = "";
                        }

                        if (row["block_status_id"] != DBNull.Value)
                        {
                            reportResult = new DownstreamSyncReportResult();
                            reportResult.BlockId = Int32.Parse(row["block_status_id"].ToString());
                            reportResult.ReportName = row["report_name"].ToString();
                            if (row["report_start_time"] != DBNull.Value)
                                reportResult.StartTime = ((DateTime)row["report_start_time"]).ToString(resultDateTimeFormat);
                            else
                                result.StartTime = "";
                            if (row["report_end_time"] != DBNull.Value)
                                reportResult.EndTime = ((DateTime)row["report_end_time"]).ToString(resultDateTimeFormat);
                            else
                                reportResult.EndTime = "";
                            reportResult.Status = row["report_status"].ToString();
                            result.reports.Add(reportResult);
                        }
                    }

                    results.Add(result);
                }
                //query = @"SELECT task_name.setup_name        AS 'setup_name', 
                //                    setup.setup_id, setup.setup_status_id,
                //                    setup.failure_reason        AS 'setup_failure_message', 
                //                    setup.status                AS 'setup_status', 
                //                    setup.start_time            AS 'setup_start_time', 
                //                    setup.end_time              AS 'setup_end_time', 
                //                    setup.guid  
                //                   FROM ivprefmaster.dbo.ivp_srm_dwh_downstream_master task_name
                //       Inner JOIN ivprefmaster.dbo.ivp_srm_dwh_downstream_setup_status setup 
                //                    ON setup.setup_id = task_name.setup_id 
                //          where setup_status_id NOT IN (Select distinct(setup_status_id) from ivprefmaster.dbo.ivp_srm_dwh_downstream_block_status)";
                //if (dtStart != null || dtEnd != null)
                //{
                //    query += " AND ";
                //    if (dtStart != null)
                //    {
                //        query += " setup.start_time>='" + dtStart.Value.ToString("yyyy-MM-dd") + "'";
                //    }
                //    if (dtEnd != null)
                //    {
                //        if (dtStart != null)
                //            query += " AND ";
                //        query += " setup.end_time<='" + dtEnd.Value.ToString("yyyy-MM-dd") + "'";
                //    }
                //}
                //dt = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                //foreach (DataRow row in dt.Rows)
                //{
                //    result = new DownstreamSyncStatusResult();
                //    result.setup_status_id = Int32.Parse(row["setup_status_id"].ToString());
                //    result.SetupName = row["setup_name"].ToString();
                //    result.Guid = row["guid"].ToString();
                //    result.StartTime = ((DateTime)row["setup_start_time"]).ToString(resultDateTimeFormat);
                //    result.EndTime = ((DateTime)row["setup_end_time"]).ToString(resultDateTimeFormat);
                //    result.Status = row["setup_status"].ToString();
                //    results.Add(result);
                //}
            }

            catch (Exception e)
            {

                mLogger.Error("GetAllDownstreamSyncStatus -----> Error " + e.ToString());
                throw;
            }
            mLogger.Error("GetAllDownstreamSyncStatus -----> end");
            return results;

        }

        public static DownstreamSchedulerInfo GetSchedulerData(string selectedSystemName)
        {
            List<string> configNames = new List<string>();
            configNames.Add(selectedSystemName);
            var schedulingInfo = SRMDownstreamStatusAndScheduler.GetDWHSchedulingConfiguration(configNames, null).Tables[0];
            foreach (var row in schedulingInfo.AsEnumerable())
            {
                DownstreamSchedulerInfo result = new DownstreamSchedulerInfo();
                result.RecurrenceType = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Recurrence_Type]);
                result.StartDate = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Start_Date]);
                result.StartTime = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Start_Time]);
                if (row[SRM_DownstreamSync_ColumnNames.End_Date] != DBNull.Value)
                    result.EndDate = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.End_Date]);
                if (row[SRM_DownstreamSync_ColumnNames.Recurrence_Pattern] != DBNull.Value)
                    result.RecurrencePattern = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Recurrence_Pattern]);
                if (row[SRM_DownstreamSync_ColumnNames.Interval] != DBNull.Value)
                    result.Interval = Convert.ToInt32(row[SRM_DownstreamSync_ColumnNames.Interval]);
                if (row[SRM_DownstreamSync_ColumnNames.Number_of_Recurrences] != DBNull.Value)
                    result.NumberOfRecurrence = Convert.ToInt32(row[SRM_DownstreamSync_ColumnNames.Number_of_Recurrences]);
                if (row[SRM_DownstreamSync_ColumnNames.Time_Interval_of_Recurrence] != DBNull.Value)
                    result.TimeInterval = Convert.ToInt32(row[SRM_DownstreamSync_ColumnNames.Time_Interval_of_Recurrence]);
                if (row[SRM_DownstreamSync_ColumnNames.Never_End_Job] != DBNull.Value)
                    result.NoEndDate = Convert.ToBoolean(row[SRM_DownstreamSync_ColumnNames.Never_End_Job]);
                if (row[SRM_DownstreamSync_ColumnNames.Days_Of_Week] != DBNull.Value && result.RecurrencePattern != null && result.RecurrencePattern == "weekly")
                    result.DaysOfWeek = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Days_Of_Week]);
                return result;
            }
            return null;
            //throw new Exception("No Scheduler Info for system name: " + selectedSystemName);
        }

        public static ObjectTable getObjectTableFor(DownstreamSchedulerInfo info)
        {
            if (info == null)
                return null;
            ObjectTable SetupTable = new ObjectTable();
            SetupTable.Columns.Add("SetupId", typeof(int));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.Setup_Name, typeof(string));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.Recurrence_Type, typeof(string));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.Start_Date, typeof(DateTime));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.Recurrence_Pattern, typeof(string));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.Interval, typeof(int));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.End_Date, typeof(DateTime));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.Number_of_Recurrences, typeof(int));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.Start_Time, typeof(DateTime));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.Time_Interval_of_Recurrence, typeof(int));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.Never_End_Job, typeof(bool));
            SetupTable.Columns.Add(SRM_DownstreamSync_ColumnNames.Days_Of_Week, typeof(string));
            SetupTable.Columns.Add(SRM_DownstreamSync_SpecialColumnNames.Sync_Status, typeof(string));
            SetupTable.Columns.Add(SRM_DownstreamSync_SpecialColumnNames.Remarks, typeof(string));
            SetupTable.Columns.Add("next_schedule_time", typeof(DateTime));

            var row = SetupTable.NewRow();
            row[SRM_DownstreamSync_ColumnNames.Setup_Name] = info.SetupName;
            row[SRM_DownstreamSync_ColumnNames.Recurrence_Type] = info.RecurrenceType;
            row[SRM_DownstreamSync_ColumnNames.Start_Date] = info.StartDate;
            row[SRM_DownstreamSync_ColumnNames.Recurrence_Pattern] = info.RecurrencePattern;
            row[SRM_DownstreamSync_ColumnNames.Interval] = info.Interval;
            row[SRM_DownstreamSync_ColumnNames.End_Date] = info.EndDate;
            row[SRM_DownstreamSync_ColumnNames.Number_of_Recurrences] = info.NumberOfRecurrence;
            row[SRM_DownstreamSync_ColumnNames.Start_Time] = info.StartTime;
            row[SRM_DownstreamSync_ColumnNames.Time_Interval_of_Recurrence] = info.TimeInterval;
            row[SRM_DownstreamSync_ColumnNames.Never_End_Job] = info.NoEndDate;
            row[SRM_DownstreamSync_ColumnNames.Days_Of_Week] = info.DaysOfWeek;
            SetupTable.Rows.Add(row);
            return SetupTable;
        }
        public static void ValidateandSaveDWHScheduling(ObjectTable excelInfo, HashSet<string> invalidSetups, string userName, RDBConnectionManager con, bool isSync,string dateFormat)
        {
            mLogger.Error("ValidateandSaveDWHScheduling -----> START");

            try
            {
                if (con == null)
                    throw new Exception("Connection cannot be null");
                if (invalidSetups == null)
                    invalidSetups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                Dictionary<string, int> existingSetupNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                HashSet<string> existingSetupInSheet = new HashSet<string>();

                Dictionary<string, DWHSchedulingInfo> existingSetupNameVsInfo = new Dictionary<string, DWHSchedulingInfo>();

                #region COLLECTION GENERATION
                HashSet<string> recurrenceTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Recurring", "Non-Recurring" };
                HashSet<string> recurrencePatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "daily", "weekly", "monthly" };
                Dictionary<string, string> dependencyRelations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "OR", "O" }, { "AND", "&" } };
                ObjectTable existingInfo = GetDWHSchedulingConfiguration(new List<string>(), con, true).Tables[0];

                foreach (var row in existingInfo.AsEnumerable())
                {
                    var setupName = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Setup_Name]);
                    existingSetupNames.Add(setupName, Convert.ToInt32(row["setup_id"]));
                }

                foreach (var row in existingInfo.AsEnumerable())
                {
                    var setupName = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Setup_Name]);
                    var recurrenceType = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Recurrence_Type]);
                    var startDateString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Start_Date]);
                    var endDateString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.End_Date]);
                    var startTimeString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Start_Time]);
                    var recurrencePattern = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Recurrence_Pattern]);
                    var intervalString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Interval]);
                    var noOfRecurrenceString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Number_of_Recurrences]);
                    var timeIntervalOfRecurrenceString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Time_Interval_of_Recurrence]);
                    var neverEndJobString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Never_End_Job]);
                    var daysOfWeekString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Days_Of_Week]);

                    DateTime startDate, endDate = default(DateTime);
                    TimeSpan startTime;
                    bool neverEndJob = false;
                    int interval = 0, noOfRecurrence = 0, timeInterval = 0, daysOfWeek = 0;

                    if (!(DateTime.TryParseExact(startDateString, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, @"MM/dd/yyyy hh\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, @"MM/dd/yyyy HH\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "MM/dd/yyyy hh:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "MM/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "M/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate)))
                    {
                    }
                    startDate = startDate.Date;

                    if ((string.IsNullOrWhiteSpace(intervalString) || !Int32.TryParse(intervalString, out interval)) && interval < 0)
                    {
                    }
                    if (!string.IsNullOrWhiteSpace(endDateString) && !(DateTime.TryParseExact(endDateString, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, @"MM/dd/yyyy hh\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, @"MM/dd/yyyy HH\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "MM/dd/yyyy hh:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "MM/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "M/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate)))
                    {
                    }
                    if ((string.IsNullOrWhiteSpace(noOfRecurrenceString) || !Int32.TryParse(noOfRecurrenceString, out noOfRecurrence)) && noOfRecurrence < 0)
                    {
                    }
                    if (!TimeSpan.TryParseExact(startTimeString, @"hh\:mm\:ss", null, out startTime))
                    {
                    }
                    if ((string.IsNullOrWhiteSpace(timeIntervalOfRecurrenceString) || !Int32.TryParse(timeIntervalOfRecurrenceString, out timeInterval)) && timeInterval < 0)
                    {
                    }
                    if (!string.IsNullOrWhiteSpace(neverEndJobString) && !Boolean.TryParse(neverEndJobString, out neverEndJob))
                    {
                    }

                    if (!string.IsNullOrWhiteSpace(daysOfWeekString))
                    {
                        var days = daysOfWeekString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (days.Length > 0)
                        {
                            var invalidDays = days.Where(x => !DaysOfWeekStringToNumber.ContainsKey(x));
                            if (invalidDays.Count() > 0)
                            {
                            }
                            else
                                daysOfWeek = Enumerable.Sum(days.Select(x => Convert.ToInt32(DaysOfWeekStringToNumber[x])));
                        }
                    }

                    var setupInfo = new DWHSchedulingInfo { ConfigName = setupName, EndDate = endDate, Interval = interval, NeverEndJob = neverEndJob, NumberOfRecurrences = noOfRecurrence, RecurrencePattern = recurrencePattern, RecurrenceType = recurrenceType, StartDate = startDate, StartTime = startTime, TimeIntervalOfRecurrence = timeInterval, DaysOfWeek = daysOfWeek };

                    if (existingSetupNames.ContainsKey(setupName))
                    {
                        var SetupId = existingSetupNames[setupName].ToString();
                        existingSetupNameVsInfo.Add(SetupId, setupInfo);
                    }
                }

                #endregion
                StringBuilder builder = new StringBuilder();

                #region EXCEL INFO PROCESSING
                foreach (var row in excelInfo.AsEnumerable())
                {
                    bool hasErrors = false;
                    var setupName = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Setup_Name]);
                    var status = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Sync_Status]);
                    if (!invalidSetups.Contains(setupName) && string.IsNullOrWhiteSpace(status))
                    {
                        var recurrenceType = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Recurrence_Type]);
                        var startDateString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Start_Date]);
                        var endDateString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.End_Date]);
                        var startTimeString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Start_Time]);
                        var recurrencePattern = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Recurrence_Pattern]);
                        var intervalString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Interval]);
                        var noOfRecurrenceString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Number_of_Recurrences]);
                        var timeIntervalOfRecurrenceString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Time_Interval_of_Recurrence]);
                        var neverEndJobString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Never_End_Job]);
                        var daysOfWeekString = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Days_Of_Week]);

                        DateTime startDate = default(DateTime), endDate = default(DateTime);
                        TimeSpan startTime = default(TimeSpan);
                        bool neverEndJob = false;
                        int interval = 0, noOfRecurrence = 0, timeInterval = 0, daysOfWeek = 0;

                        if (String.IsNullOrWhiteSpace(setupName))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Setup Name cannot be blank.";
                        }

                        if (!recurrenceTypes.Contains(recurrenceType))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Recurrence type is invalid.";
                        }

                        if (!hasErrors && recurrenceType.Equals("Non-Recurring", StringComparison.OrdinalIgnoreCase))
                        {
                            endDateString = string.Empty;
                            recurrencePattern = string.Empty;
                            intervalString = string.Empty;
                            noOfRecurrenceString = string.Empty;
                            timeIntervalOfRecurrenceString = string.Empty;
                        }

                        if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && !(DateTime.TryParseExact(startDateString, dateFormat, null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, dateFormat.Split(' ')[0], null, System.Globalization.DateTimeStyles.None, out startDate)||DateTime.TryParseExact(startDateString, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, @"MM/dd/yyyy hh\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, @"MM/dd/yyyy HH\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "MM/dd/yyyy hh:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "MM/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "M/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate)))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Start Date parsing failed.";
                        }
                        if (!hasErrors && recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && startDate > startDate.Date)
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Start Date cannot have time component.";
                        }
                        if (!hasErrors && recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && recurrencePattern.Equals("Weekly", StringComparison.OrdinalIgnoreCase) && String.IsNullOrWhiteSpace(daysOfWeekString))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Select atleast one day of the week";
                        }
                        if (!TimeSpan.TryParseExact(startTimeString, @"hh\:mm\:ss", null, out startTime))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Start Time is invalid.";
                        }
                        if (!hasErrors && recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && new DateTime(startDate.Year, startDate.Month, startDate.Day, startTime.Hours, startTime.Minutes, startTime.Seconds) <= DateTime.Now)
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Start Date has to be greater than current time.";
                        }
                        if (!string.IsNullOrWhiteSpace(recurrencePattern) && !recurrencePatterns.Contains(recurrencePattern))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Recurrence pattern is invalid.";
                        }
                        if ((string.IsNullOrWhiteSpace(intervalString) || !Int32.TryParse(intervalString, out interval)) && interval < 0)
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Interval is invalid.";
                        }
                        if (!(TimeSpan.TryParseExact(startTimeString, @"hh\:mm\:ss", null, out startTime)|| (dateFormat.Split(' ').Length>=2&&TimeSpan.TryParseExact(startTimeString,dateFormat.Split(' ')[1], null, out startTime))))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Start Time is invalid.";
                        }
                        if (!string.IsNullOrWhiteSpace(endDateString) && !(DateTime.TryParseExact(endDateString, dateFormat, null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, dateFormat.Split(' ')[0], null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, @"MM/dd/yyyy hh\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, @"MM/dd/yyyy HH\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "MM/dd/yyyy hh:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "MM/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "M/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate)))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " End Date is invalid.";
                        }
                        if (string.IsNullOrWhiteSpace(startDateString) || !(DateTime.TryParseExact(startDateString, dateFormat, null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, @"MM/dd/yyyy hh\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, @"MM/dd/yyyy HH\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "MM/dd/yyyy hh:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "MM/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "M/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate)))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Start Date is invalid.";
                        }
                        if ((string.IsNullOrWhiteSpace(noOfRecurrenceString) || !Int32.TryParse(noOfRecurrenceString, out noOfRecurrence)) && noOfRecurrence < 0)
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Number Of Recurrence is invalid.";
                        }

                        if ((string.IsNullOrWhiteSpace(timeIntervalOfRecurrenceString) || !Int32.TryParse(timeIntervalOfRecurrenceString, out timeInterval)) && timeInterval < 0)
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Time Interval Of Recurrence is invalid.";
                        }
                        if (!string.IsNullOrWhiteSpace(neverEndJobString) && !Boolean.TryParse(neverEndJobString, out neverEndJob))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Never End Job is invalid.";
                        }
                        if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && !neverEndJob && endDate == default(DateTime))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Either set Never End Job to true or mention the End Date.";
                        }
                        if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && !neverEndJob && endDate != default(DateTime) && endDate.AddDays(1) < new DateTime(startDate.Year, startDate.Month, startDate.Day, startTime.Hours, startTime.Minutes, startTime.Seconds))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " End Date should not be less than Start date.";
                        }
                        if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(daysOfWeekString))
                        {
                            var days = daysOfWeekString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (days.Length > 0)
                            {
                                var invalidDays = days.Where(x => !DaysOfWeekStringToNumber.ContainsKey(char.ToUpper(x[0])+x.ToLower().Substring(1)));
                                if (invalidDays.Count() > 0)
                                {
                                    hasErrors = true;
                                    row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " There are invalid days in Days Of Week.";
                                }
                                else
                                    daysOfWeek = Enumerable.Sum(days.Select(x => Convert.ToInt32(DaysOfWeekStringToNumber[x])));
                            }
                        }

                        if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && (noOfRecurrence <= 0 || interval <= 0 || timeInterval <= 0))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Invalid (Number Of Recurrences OR Interval OR Time Interval Of Recurrence).";
                        }

                        if (!hasErrors && recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && recurrencePattern.Equals("daily", StringComparison.OrdinalIgnoreCase) && noOfRecurrence > 0 && timeInterval > 0 && startDate.Add(startTime).AddMinutes(noOfRecurrence * timeInterval).Date == startDate.Date.AddDays(1))
                        {
                            hasErrors = true;
                            row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]) + SRM_DownstreamSync_Separators.Remarks_Separator + " Cannot schedule the setup since all the recurrences of this setup will not end on start date.";
                        }

                        if (!hasErrors)
                        {
                            var setupInfo = new DWHSchedulingInfo { ConfigName = setupName, EndDate = endDate, Interval = interval, NeverEndJob = neverEndJob, NumberOfRecurrences = noOfRecurrence, RecurrencePattern = recurrencePattern, RecurrenceType = recurrenceType, StartDate = startDate, StartTime = startTime, TimeIntervalOfRecurrence = timeInterval, DaysOfWeek = daysOfWeek };

                            if (existingSetupNames.ContainsKey(setupName))
                            {
                                var SetupId = existingSetupNames[setupName].ToString();
                                existingSetupNameVsInfo[SetupId] = setupInfo;
                                existingSetupInSheet.Add(SetupId);
                            }
                        }
                        else
                        {
                            row[SRM_DownstreamSync_SpecialColumnNames.Sync_Status] = SRM_DownstreamSync_MigrationStatus.Failed;
                            invalidSetups.Add(setupName);
                        }
                    }
                    else if (status == SRM_DownstreamSync_MigrationStatus.Failed)
                    {
                        invalidSetups.Add(setupName);
                    }
                    if (hasErrors)
                    {
                        builder.AppendLine(setupName);
                        builder.Append(Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Remarks]));
                        throw new Exception(builder.ToString());
                    }
                }


                #endregion


                #region POPULATE SCHEDULING INFO
                ObjectTable SetupTable = new ObjectTable();
                SetupTable.Columns.Add("SetupId", typeof(int));
                SetupTable.Columns.Add("SetupName", typeof(string));
                SetupTable.Columns.Add("RecurrenceType", typeof(string));
                SetupTable.Columns.Add("StartDate", typeof(DateTime));
                SetupTable.Columns.Add("RecurrencePattern", typeof(string));
                SetupTable.Columns.Add("Interval", typeof(int));
                SetupTable.Columns.Add("EndDate", typeof(DateTime));
                SetupTable.Columns.Add("NumberOfRecurrences", typeof(int));
                SetupTable.Columns.Add("StartTime", typeof(DateTime));
                SetupTable.Columns.Add("TimeIntervalOfRecurrence", typeof(int));
                SetupTable.Columns.Add("NeverEndJob", typeof(bool));
                SetupTable.Columns.Add("DaysOfWeek", typeof(int));

                foreach (var setupLevel in existingSetupNameVsInfo)
                {
                    if (!existingSetupInSheet.Contains(setupLevel.Key))
                        continue;
                    var ndr = SetupTable.NewRow();
                    ndr["SetupId"] = setupLevel.Key;
                    ndr["SetupName"] = setupLevel.Value.ConfigName;
                    ndr["RecurrenceType"] = setupLevel.Value.RecurrenceType;

                    if (setupLevel.Value.StartDate != default(DateTime))
                        ndr["StartDate"] = setupLevel.Value.StartDate;

                    ndr["RecurrencePattern"] = setupLevel.Value.RecurrencePattern;
                    ndr["Interval"] = setupLevel.Value.Interval;

                    if (setupLevel.Value.EndDate != default(DateTime))
                        ndr["EndDate"] = setupLevel.Value.EndDate;

                    ndr["NumberOfRecurrences"] = setupLevel.Value.NumberOfRecurrences;

                    if (setupLevel.Value.StartTime != default(TimeSpan) && setupLevel.Value.StartDate != default(DateTime))
                        ndr["StartTime"] = new DateTime(setupLevel.Value.StartDate.Year, setupLevel.Value.StartDate.Month, setupLevel.Value.StartDate.Day, setupLevel.Value.StartTime.Hours, setupLevel.Value.StartTime.Minutes, setupLevel.Value.StartTime.Seconds);

                    ndr["TimeIntervalOfRecurrence"] = setupLevel.Value.TimeIntervalOfRecurrence;
                    ndr["NeverEndJob"] = setupLevel.Value.NeverEndJob;
                    ndr["DaysOfWeek"] = setupLevel.Value.DaysOfWeek;

                    SetupTable.Rows.Add(ndr);
                }

                #endregion

                #region SYNC

                if (SetupTable.Rows.Count > 0)
                {
                    var guid = Guid.NewGuid().ToString().Replace("-", "_");
                    var setupTableName = "dbo.[setupTable_" + guid + "]";

                    try
                    {
                        CommonDALWrapper.ExecuteQuery(string.Format(@"CREATE TABLE {0}(SetupId INT, SetupName VARCHAR(MAX),  RecurrenceType VARCHAR(MAX), StartDate DATETIME, RecurrencePattern VARCHAR(MAX), Interval INT, EndDate DATETIME, NumberOfRecurrences INT, StartTime DATETIME, TimeIntervalOfRecurrence INT, NeverEndJob BIT, DaysOfWeek INT)", setupTableName), CommonQueryType.Insert, con);
                        CommonDALWrapper.ExecuteBulkUploadObject(setupTableName, SetupTable, con);
                        CommonDALWrapper.ExecuteQuery(string.Format(@"EXEC dbo.SRM_DWH_SaveDownstreamSchedulerInfo '{0}', '{1}'", setupTableName, userName), CommonQueryType.Insert, con);
                        foreach (var roww in excelInfo.AsEnumerable())
                        {
                            if (string.IsNullOrWhiteSpace(Convert.ToString(roww[SRM_DownstreamSync_SpecialColumnNames.Sync_Status])))
                            {
                                roww[SRM_DownstreamSync_SpecialColumnNames.Sync_Status] = SRM_DownstreamSync_MigrationStatus.Passed;
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        mLogger.Error("ValidateandSaveDWHScheduling -----> SYNC ERROR : " + ee.ToString());
                        var message = ee.Message;

                        foreach (var row in excelInfo.Rows)
                        {
                            if (string.IsNullOrWhiteSpace(Convert.ToString(row[SRM_DownstreamSync_SpecialColumnNames.Sync_Status])))
                            {
                                row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = message;
                                row[SRM_DownstreamSync_SpecialColumnNames.Sync_Status] = SRM_DownstreamSync_MigrationStatus.Failed;
                            }
                        }

                        throw;
                    }
                    finally
                    {
                        CommonDALWrapper.ExecuteQuery(string.Format(@"IF (OBJECT_ID('{0}') IS NOT NULL) DROP TABLE {0};", setupTableName), CommonQueryType.Delete, con);
                    }
                }
                #endregion
            }
            catch (Exception ee)
            {
                mLogger.Error("ValidateandSaveDWHScheduling -----> ERROR : " + ee.ToString());
                if (!isSync)
                    throw;
            }
            finally
            {
                mLogger.Error("ValidateandSaveDWHScheduling -----> END");
            }
        }

        public static ObjectSet GetDWHSchedulingConfiguration(List<string> configNames, RDBConnectionManager con, bool forSync = false)
        {
            bool openedConnection = false;
            mLogger.Error("GetDWHSchedulingConfiguration -----> START");
            if (forSync && con == null)
                throw new Exception("Connection Object is null for sync");
            try
            {
                if (con == null)
                {
                    openedConnection = true;
                    con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, false, IsolationLevel.ReadCommitted);
                }
                var ds = CommonDALWrapper.ExecuteSelectQueryObject(string.Format(@"DECLARE @setup_names VARCHAR(MAX) = '{0}'

                DECLARE @setup TABLE (setup_id INT)
                IF(ISNULL(@setup_names,'') = '')
                BEGIN
	                INSERT INTO @setup
	                SELECT DISTINCT ct.setup_id
                    FROM dbo.ivp_srm_dwh_downstream_master ct
                    WHERE ct.is_active = 1
                END
                ELSE
                BEGIN
	                INSERT INTO @setup
	                SELECT setup_id
	                FROM dbo.ivp_srm_dwh_downstream_master ct
	                INNER JOIN dbo.REFM_GetList2Table(@setup_names,',') c ON ct.setup_name = c.item
	                WHERE is_active = 1
                END

                SELECT {1} ct.setup_name AS [Setup Name], CASE WHEN sj.recurrence_type = 1 THEN 'Recurring' ELSE 'Non-Recurring' END AS [Recurrence Type], sj.next_schedule_time AS [Start Date], sj.recurrence_pattern AS [Recurrence Pattern], 
	                CASE WHEN recurrence_pattern = 'daily' THEN day_interval WHEN recurrence_pattern = 'weekly' THEN week_interval WHEN recurrence_pattern = 'monthly' THEN month_interval END AS [Interval], 
	                end_date AS [End Date], no_of_recurrences AS [Number of Recurrences], CAST(start_time AS TIME) AS [Start Time], time_interval_of_recurrence AS [Time Interval of Recurrence], no_end_date AS [Never End Job],
                    days_of_week
                FROM dbo.ivp_srm_dwh_downstream_master ct
                INNER JOIN @setup c ON ct.setup_id = c.setup_id
                LEFT JOIN dbo.ivp_srm_dwh_downstream_scheduled_jobs sj ON (ct.setup_id = sj.setup_id AND sj.is_active = 1)
                ORDER BY ct.setup_name", (configNames != null && configNames.Count > 0 ? string.Join(",", configNames) : string.Empty), "ct.setup_id,next_schedule_time,"), con);

                ds.Tables[0].Columns.Add("Days Of Week", typeof(string));

                foreach (var row in ds.Tables[0].AsEnumerable())
                {
                    var endDate = Convert.ToString(row["End Date"]);
                    if (!string.IsNullOrWhiteSpace(endDate))
                        row["End Date"] = Convert.ToDateTime(row["End Date"]).ToString(@"MM/dd/yyyy");

                    var startDate = Convert.ToString(row["Start Date"]);
                    if (!string.IsNullOrWhiteSpace(startDate))
                    {
                        var startDt = Convert.ToDateTime(row["Start Date"]);
                        if (startDt < DateTime.Now)
                            row["Recurrence Type"] = "Non-Recurring";
                        else
                        {
                            row["Start Date"] = startDt.ToString(@"MM/dd/yyyy");
                            row["Start Time"] = startDt.TimeOfDay.ToString(@"hh\:mm\:ss");
                        }
                    }

                    var daysOfWeek = Convert.ToString(row["days_of_week"]);

                    if (!string.IsNullOrWhiteSpace(daysOfWeek))
                    {
                        List<string> lstdaysOfWeek = ExtractDaysOfWeek(daysOfWeek);
                        daysOfWeek = string.Join(",", lstdaysOfWeek.Where(y => DaysOfWeekNumberToString.ContainsKey(y)).Select(x => DaysOfWeekNumberToString[x]));
                        row["Days Of Week"] = daysOfWeek;
                    }

                    var recurrenceType = Convert.ToString(row["Recurrence Type"]);
                    if (recurrenceType.Equals("Non-Recurring", StringComparison.OrdinalIgnoreCase))
                    {
                        //row["Start Date"] = DBNull.Value;
                        row["Recurrence Pattern"] = DBNull.Value;
                        row["Interval"] = DBNull.Value;
                        row["End Date"] = DBNull.Value;
                        row["Number of Recurrences"] = DBNull.Value;
                        //row["Start Time"] = DBNull.Value;
                        row["Time Interval of Recurrence"] = DBNull.Value;
                        row["Never End Job"] = DBNull.Value;
                        row["Days Of Week"] = DBNull.Value;
                    }
                }

                ds.Tables[0].Columns.Remove("days_of_week");
                ds.Tables[0].TableName = "Schedule Information";
                if (!forSync)
                {
                    ds.Tables[0].Columns.Remove("setup_id");
                    ds.Tables[0].Columns.Remove("next_schedule_time");
                }
                if (!forSync)
                    con.CommitTransaction();

                return ds;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetDWHSchedulingConfiguration -----> ERROR : " + ex.ToString());
                throw;
            }
            finally
            {
                if (openedConnection)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(con);
                mLogger.Error("GetDWHSchedulingConfiguration -----> END");
            }
        }

        public static void DeleteDWHSchedulingInfo(string systemName, RDBConnectionManager con)
        {
            mLogger.Error("DeleteDWHSchedulingInfo -----> START");
            bool openedConnection = false;
            if (con == null)
            {
                mLogger.Error("DeleteDWHSchedulingInfo -----> Opening New Connection");
                openedConnection = true;
                con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, false, IsolationLevel.ReadCommitted);
            }
            string query = "Delete from dbo.ivp_srm_dwh_downstream_scheduled_jobs where setup_id IN (select setup_id from  dbo.ivp_srm_dwh_downstream_master where setup_name='" + systemName.Trim() + "')";
            try
            {
                CommonDALWrapper.ExecuteSelectQueryObject(query, con);
            }
            catch (Exception ex)
            {
                mLogger.Error("DeleteDWHSchedulingInfo -----> ERROR : " + ex.ToString());
                throw;
            }
            finally
            {
                if (openedConnection)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(con);
                mLogger.Error("DeleteDWHSchedulingInfo -----> END");
            }

        }
    }

    [DataContract]
    public class DownstreamSyncReportMessage
    {
        [DataMember]
        public int block_status_id { get; set; }
        [DataMember]
        public string message { get; set; }
        [DataMember]
        public string created_on { get; set; }
    }
    [DataContract]
    public class DownstreamSyncStatusResult
    {
        [DataMember]
        public int setup_status_id { get; set; }
        [DataMember]
        public string SetupName { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string StartTime { get; set; }
        [DataMember]
        public string EndTime { get; set; }
        [DataMember]
        public string FailureMessage { get; set; }
        [DataMember]
        public List<DownstreamSyncReportResult> reports { get; set; }
    }

    [DataContract]
    public class DownstreamSyncReportResult
    {
        [DataMember]
        public int BlockId { get; set; }
        [DataMember]
        public string ReportName { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string StartTime { get; set; }
        [DataMember]
        public string EndTime { get; set; }

    }


    [DataContract]
    public class DownstreamSchedulerInfo
    {
        [DataMember]
        public int TimeInterval { get; set; }
        [DataMember]
        public String RecurrenceType { get; set; }
        [DataMember]
        public string RecurrencePattern { get; set; }
        [DataMember]
        public int NumberOfRecurrence { get; set; }
        [DataMember]
        public string SetupName { get; set; }
        [DataMember]
        public string SetupDescription { get; set; }
        [DataMember]
        public string StartDate { get; set; }
        [DataMember]
        public string StartTime { get; set; }
        [DataMember]
        public string EndDate { get; set; }
        [DataMember]
        public bool NoEndDate { get; set; }
        [DataMember]
        public int Interval { get; set; }
        [DataMember]
        public String DaysOfWeek { get; set; }

    }

    public class DWHSchedulingInfo
    {
        public int ConfigId { get; set; }
        public string ConfigName { get; set; }
        public string RecurrenceType { get; set; }
        public DateTime StartDate { get; set; }
        public string RecurrencePattern { get; set; }
        public int Interval { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfRecurrences { get; set; }
        public int DaysOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public int TimeIntervalOfRecurrence { get; set; }
        public bool NeverEndJob { get; set; }
    }
}
