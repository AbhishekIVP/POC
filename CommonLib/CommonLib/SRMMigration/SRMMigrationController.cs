using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.migration
{
    public static class SRMMigrationController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMMigrationController");
        static readonly Dictionary<string, string> DaysOfWeekStringToNumber = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "Sunday", "1" }, { "Monday", "2" }, { "Tuesday", "4" }, { "Wednesday", "8" }, { "Thursday", "16" }, { "Friday", "32" }, { "Saturday", "64" } };
        static readonly Dictionary<string, string> DaysOfWeekNumberToString = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "0", "Sunday" }, { "1", "Monday" }, { "2", "Tuesday" }, { "3", "Wednesday" }, { "4", "Thursday" }, { "5", "Friday" }, { "6", "Saturday" } };

        #region Migration Task Manager Methods

        public static ObjectSet GetTaskManagerConfiguration(List<string> chainNames, bool forSync = false)
        {
            mLogger.Error("GetTaskManagerConfiguration -----> START");
            RDBConnectionManager con = null;
            try
            {
                con = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                var emailVsDisplayName = SRMCommonRAD.GetUsersEmailVsDisplayName();

                var ds = con.ExecuteQueryObject(string.Format(@"DECLARE @chain_names VARCHAR(MAX) = '{0}'

                DECLARE @chain TABLE (chain_id INT)
                IF(ISNULL(@chain_names,'') = '')
                BEGIN
	                INSERT INTO @chain
	                SELECT DISTINCT ct.chain_id
                    FROM dbo.ivp_rad_chained_tasks ct
                    INNER JOIN dbo.ivp_rad_flow rf ON ct.chain_id = rf.chain_id
                    INNER JOIN dbo.ivp_rad_task_summary ts ON rf.task_summary_id = ts.task_summary_id
                    WHERE ct.is_active = 1 AND rf.is_active = 1 AND ts.is_visible_on_ctm = 1
                END
                ELSE
                BEGIN
	                INSERT INTO @chain
	                SELECT chain_id
	                FROM dbo.ivp_rad_chained_tasks ct
	                INNER JOIN dbo.REFM_GetList2Table(@chain_names,',') c ON ct.chain_name = c.item
	                WHERE is_active = 1
                END

                SELECT {1} chain_name AS [Chain Name], ct.calendar_name AS [Calendar Name], CASE WHEN sj.recurrence_type = 1 THEN 'Recurring' ELSE 'Non-Recurring' END AS [Recurrence Type], sj.next_schedule_time AS [Start Date], sj.recurrence_pattern AS [Recurrence Pattern], 
	                CASE WHEN recurrence_pattern = 'daily' THEN day_interval WHEN recurrence_pattern = 'weekly' THEN week_interval WHEN recurrence_pattern = 'monthly' THEN month_interval END AS [Interval], 
	                end_date AS [End Date], no_of_recurrences AS [Number of Recurrences], CAST(start_time AS TIME) AS [Start Time], time_interval_of_recurrence AS [Time Interval of Recurrence], no_end_date AS [Never End Job],
	                subscribe_id AS [Chain Subscription], days_of_week
                FROM dbo.ivp_rad_chained_tasks ct
                INNER JOIN @chain c ON ct.chain_id = c.chain_id
                LEFT JOIN dbo.ivp_rad_scheduled_jobs sj ON (ct.scheduled_job_id = sj.job_id AND sj.is_active = 1)
                ORDER BY ct.chain_name

                SELECT rf.flow_id, chain_name AS [Chain Name], rf.task_name AS [Task Name],  is_muted AS [Is Muted], proceed_on_fail AS [Proceed On Fail], rerun_on_fail AS [ReRun On Fail], fail_retry_duration AS [Retry Duration], 
	                fail_number_retry AS [Number Of Retries], on_fail_run_task AS [On Fail Run Task], task_time_out AS [Time Out], task_second_instance_wait AS [Task Instance Wait], 
	                task_wait_subscription_id AS [Task Wait Subscription], rf.subscribe_id AS [Task Subscription], rf.dependent_on_id, rf.task_type_name AS [Task Type], rf.module_name AS [Module Name]
                FROM dbo.ivp_rad_chained_tasks ct
                INNER JOIN @chain c ON ct.chain_id = c.chain_id
                INNER JOIN dbo.ivp_rad_flow rf ON ct.chain_id = rf.chain_id
                INNER JOIN dbo.ivp_rad_task_summary ts ON rf.task_summary_id = ts.task_summary_id
                WHERE rf.is_active = 1 AND is_visible_on_ctm = 1
                ORDER BY ct.chain_name, rf.flow_id", (chainNames != null && chainNames.Count > 0 ? string.Join(",", chainNames) : string.Empty), "ct.chain_id,next_schedule_time,"), RQueryType.Select);

                ds.Tables[0].Columns.Add("Days Of Week", typeof(string));

                ds.Tables[1].Columns.Add("On Fail Run Task Task", typeof(string));
                ds.Tables[1].Columns.Add("On Fail Run Task Type", typeof(string));
                ds.Tables[1].Columns.Add("On Fail Run Task Module Name", typeof(string));

                var taskDependency = new ObjectTable("Task Dependency");
                taskDependency.Columns.Add("Chain Name", typeof(string));
                taskDependency.Columns.Add("Task Name", typeof(string));
                taskDependency.Columns.Add("Task Type", typeof(string));
                taskDependency.Columns.Add("Module Name", typeof(string));
                taskDependency.Columns.Add("Dependent On Task", typeof(string));
                taskDependency.Columns.Add("Dependent On Task Type", typeof(string));
                taskDependency.Columns.Add("Dependent On Module Name", typeof(string));
                taskDependency.Columns.Add("Dependency Relation", typeof(string));

                var chainSubscription = new ObjectTable("Chain Subscription");
                chainSubscription.Columns.Add("Chain Name", typeof(string));
                chainSubscription.Columns.Add("Subscription Type", typeof(string));
                chainSubscription.Columns.Add("To Mail", typeof(string));
                chainSubscription.Columns.Add("Subject", typeof(string));
                chainSubscription.Columns.Add("Body", typeof(string));

                var taskSubscription = new ObjectTable("Task Subscription");
                taskSubscription.Columns.Add("Chain Name", typeof(string));
                taskSubscription.Columns.Add("Task Name", typeof(string));
                taskSubscription.Columns.Add("Task Type", typeof(string));
                taskSubscription.Columns.Add("Module Name", typeof(string));
                taskSubscription.Columns.Add("Subscription Type", typeof(string));
                taskSubscription.Columns.Add("To Mail", typeof(string));
                taskSubscription.Columns.Add("Subject", typeof(string));
                taskSubscription.Columns.Add("Body", typeof(string));

                var flowIdVsTaskName = new Dictionary<string, string>();

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
                        row["Start Date"] = DBNull.Value;
                        row["Recurrence Pattern"] = DBNull.Value;
                        row["Interval"] = DBNull.Value;
                        row["End Date"] = DBNull.Value;
                        row["Number of Recurrences"] = DBNull.Value;
                        row["Start Time"] = DBNull.Value;
                        row["Time Interval of Recurrence"] = DBNull.Value;
                        row["Never End Job"] = DBNull.Value;
                        row["Days Of Week"] = DBNull.Value;
                    }

                    string[] arr = null;
                    var subscribeId = Convert.ToString(row["Chain Subscription"]);
                    if (!string.IsNullOrWhiteSpace(subscribeId))
                    {
                        arr = subscribeId.Split('|');
                        if (arr.Length > 0)
                        {
                            var toEmailIds = arr[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (toEmailIds.Count() > 0)
                            {
                                var ndr = chainSubscription.NewRow();
                                ndr["Chain Name"] = Convert.ToString(row["Chain Name"]);

                                var toDisplayNames = string.Join(",", toEmailIds.Where(y => emailVsDisplayName.ContainsKey(y)).SelectMany(x => emailVsDisplayName[x]));
                                ndr["To Mail"] = toDisplayNames;
                                ndr["Subscription Type"] = "SUCCESS";
                                if (arr.Length >= 3 && !string.IsNullOrWhiteSpace(arr[2]))
                                    ndr["Subject"] = System.Web.HttpUtility.UrlDecode(arr[2]);
                                if (arr.Length >= 4 && !string.IsNullOrWhiteSpace(arr[3]))
                                    ndr["Body"] = System.Web.HttpUtility.UrlDecode(arr[3]);
                                chainSubscription.Rows.Add(ndr);
                            }

                        }
                        if (arr[1] != null)
                        {
                            var toEmailIds = arr[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (toEmailIds.Count() > 0)
                            {
                                var ndr = chainSubscription.NewRow();
                                ndr["Chain Name"] = Convert.ToString(row["Chain Name"]);

                                var toDisplayNames = string.Join(",", toEmailIds.Where(y => emailVsDisplayName.ContainsKey(y)).SelectMany(x => emailVsDisplayName[x]));
                                ndr["Subscription Type"] = "FAILURE";
                                ndr["To Mail"] = toDisplayNames;
                                if (arr.Length >= 5 && !string.IsNullOrWhiteSpace(arr[4]))
                                    ndr["Subject"] = System.Web.HttpUtility.UrlDecode(arr[4]);
                                if (arr.Length >= 6 && !string.IsNullOrWhiteSpace(arr[5]))
                                    ndr["Body"] = System.Web.HttpUtility.UrlDecode(arr[5]);
                                chainSubscription.Rows.Add(ndr);
                            }
                        }
                    }
                    else
                        continue;

                }

                foreach (var row in ds.Tables[1].AsEnumerable())
                {
                    var taskName = Convert.ToString(row["Task Name"]);
                    var taskType = Convert.ToString(row["Task Type"]);
                    var moduleName = Convert.ToString(row["Module Name"]);
                    var chainName = Convert.ToString(row["Chain Name"]);
                    var dependent = Convert.ToString(row["dependent_on_id"]);
                    var taskWaitSubscription = Convert.ToString(row["Task Wait Subscription"]);
                    var toEmailIdsForWaiting = taskWaitSubscription.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (toEmailIdsForWaiting.Count() > 0)
                    {
                        var toDisplayNames = string.Join(",", toEmailIdsForWaiting.Where(y => emailVsDisplayName.ContainsKey(y)).SelectMany(x => emailVsDisplayName[x]));
                        row["Task Wait Subscription"] = toDisplayNames;
                    }
                    else
                        row["Task Wait Subscription"] = DBNull.Value;

                    flowIdVsTaskName.Add(Convert.ToString(row["flow_id"]), taskName + "ž" + taskType + "ž" + moduleName);
                    if (!string.IsNullOrWhiteSpace(dependent))
                    {
                        var addRow = false;
                        var ndr2 = taskDependency.NewRow();
                        ndr2["Task Name"] = taskName;
                        ndr2["Task Type"] = taskType;
                        ndr2["Module Name"] = moduleName;
                        ndr2["Chain Name"] = chainName;
                        var arr2 = dependent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var dep in arr2)
                        {
                            var c = dep.FirstOrDefault();
                            switch (c)
                            {
                                case 'O':
                                    ndr2["Dependency Relation"] = "OR";
                                    break;
                                case '&':
                                    ndr2["Dependency Relation"] = "AND";
                                    break;
                            }
                            ndr2["Dependent On Task"] = dep.Remove(0, 1);
                            addRow = true;
                        }

                        if (addRow)
                            taskDependency.Rows.Add(ndr2);
                    }
                    else
                    {
                        var ndr2 = taskDependency.NewRow();
                        ndr2["Task Name"] = taskName;
                        ndr2["Task Type"] = taskType;
                        ndr2["Module Name"] = moduleName;
                        ndr2["Chain Name"] = chainName;
                        taskDependency.Rows.Add(ndr2);
                    }

                    string[] arr = null;
                    var subscribeId = Convert.ToString(row["Task Subscription"]);
                    if (!string.IsNullOrWhiteSpace(subscribeId))
                    {
                        arr = subscribeId.Split('|');
                        if (arr.Length > 0)
                        {
                            var toEmailIds = arr[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (toEmailIds.Count() > 0)
                            {
                                var ndr = taskSubscription.NewRow();
                                ndr["Chain Name"] = chainName;
                                ndr["Task Name"] = taskName;
                                ndr["Task Type"] = taskType;
                                ndr["Module Name"] = moduleName;

                                var toDisplayNames = string.Join(",", toEmailIds.Where(y => emailVsDisplayName.ContainsKey(y)).SelectMany(x => emailVsDisplayName[x]));
                                ndr["Subscription Type"] = "SUCCESS";
                                ndr["To Mail"] = toDisplayNames;
                                if (arr.Length >= 3 && !string.IsNullOrWhiteSpace(arr[2]))
                                    ndr["Subject"] = System.Web.HttpUtility.UrlDecode(arr[2]);
                                if (arr.Length >= 4 && !string.IsNullOrWhiteSpace(arr[3]))
                                    ndr["Body"] = System.Web.HttpUtility.UrlDecode(arr[3]);
                                taskSubscription.Rows.Add(ndr);
                            }

                        }
                        if (arr[1] != null)
                        {
                            var toEmailIds = arr[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (toEmailIds.Count() > 0)
                            {
                                var ndr = taskSubscription.NewRow();
                                ndr["Chain Name"] = chainName;
                                ndr["Task Name"] = taskName;
                                ndr["Task Type"] = taskType;
                                ndr["Module Name"] = moduleName;

                                var toDisplayNames = string.Join(",", toEmailIds.Where(y => emailVsDisplayName.ContainsKey(y)).SelectMany(x => emailVsDisplayName[x]));
                                ndr["Subscription Type"] = "FAILURE";
                                ndr["To Mail"] = toDisplayNames;
                                if (arr.Length >= 5 && !string.IsNullOrWhiteSpace(arr[4]))
                                    ndr["Subject"] = System.Web.HttpUtility.UrlDecode(arr[4]);
                                if (arr.Length >= 6 && !string.IsNullOrWhiteSpace(arr[5]))
                                    ndr["Body"] = System.Web.HttpUtility.UrlDecode(arr[5]);
                                taskSubscription.Rows.Add(ndr);
                            }
                        }
                    }
                    else
                        continue;

                }


                foreach (var row in ds.Tables[1].AsEnumerable())
                {
                    var onFailRunTask = Convert.ToString(row["On Fail Run Task"]);
                    if (!string.IsNullOrWhiteSpace(onFailRunTask))
                    {
                        string taskInfo;
                        if (flowIdVsTaskName.TryGetValue(onFailRunTask, out taskInfo))
                        {
                            var arr = taskInfo.Split('ž');
                            row["On Fail Run Task Task"] = arr[0];
                            row["On Fail Run Task Type"] = arr[1];
                            row["On Fail Run Task Module Name"] = arr[2];
                        }
                    }
                }

                HashSet<string> invalidChains = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var row in taskDependency.AsEnumerable())
                {
                    var dep = Convert.ToString(row["Dependent On Task"]);
                    if (!string.IsNullOrWhiteSpace(dep))
                    {
                        if (flowIdVsTaskName.ContainsKey(dep))
                        {
                            var taskName = flowIdVsTaskName[dep].Split('ž');
                            row["Dependent On Task"] = taskName[0];
                            row["Dependent On Task Type"] = taskName[1];
                            row["Dependent On Module Name"] = taskName[2];
                        }
                        else
                        {
                            invalidChains.Add(Convert.ToString(row["Chain Name"]));
                        }
                    }
                }

                List<ObjectRow> rowsToDelete = new List<ObjectRow>();
                foreach (var row in ds.Tables[0].AsEnumerable())
                {
                    var chainName = Convert.ToString(row["Chain Name"]);
                    if (invalidChains.Contains(chainName))
                        rowsToDelete.Add(row);
                }

                foreach (var row2 in rowsToDelete)
                    ds.Tables[0].Rows.Remove(row2);

                rowsToDelete = new List<ObjectRow>();
                foreach (var row in ds.Tables[1].AsEnumerable())
                {
                    var chainName = Convert.ToString(row["Chain Name"]);
                    if (invalidChains.Contains(chainName))
                        rowsToDelete.Add(row);
                }

                foreach (var row2 in rowsToDelete)
                    ds.Tables[1].Rows.Remove(row2);

                rowsToDelete = new List<ObjectRow>();
                foreach (var row in taskDependency.AsEnumerable())
                {
                    var chainName = Convert.ToString(row["Chain Name"]);
                    if (invalidChains.Contains(chainName))
                        rowsToDelete.Add(row);
                }

                foreach (var row2 in rowsToDelete)
                    taskDependency.Rows.Remove(row2);

                rowsToDelete = new List<ObjectRow>();
                foreach (var row in chainSubscription.AsEnumerable())
                {
                    var chainName = Convert.ToString(row["Chain Name"]);
                    if (invalidChains.Contains(chainName))
                        rowsToDelete.Add(row);
                }

                foreach (var row2 in rowsToDelete)
                    chainSubscription.Rows.Remove(row2);

                rowsToDelete = new List<ObjectRow>();
                foreach (var row in taskSubscription.AsEnumerable())
                {
                    var chainName = Convert.ToString(row["Chain Name"]);
                    if (invalidChains.Contains(chainName))
                        rowsToDelete.Add(row);
                }

                foreach (var row2 in rowsToDelete)
                    taskSubscription.Rows.Remove(row2);


                ds.Tables[0].Columns.Remove("days_of_week");
                ds.Tables[0].Columns.Remove("Chain Subscription");
                ds.Tables[0].TableName = "Chain Information";
                if (!forSync)
                {
                    ds.Tables[0].Columns.Remove("chain_id");
                    ds.Tables[0].Columns.Remove("next_schedule_time");
                }

                ds.Tables[1].Columns.Remove("Task Subscription");
                ds.Tables[1].Columns.Remove("dependent_on_id");
                ds.Tables[1].Columns.Remove("On Fail Run Task");
                ds.Tables[1].Columns["On Fail Run Task Task"].ColumnName = "On Fail Run Task";

                if (!forSync)
                    ds.Tables[1].Columns.Remove("flow_id");
                ds.Tables[1].TableName = "Task Information";


                ds.Tables.Add(taskDependency);
                //ds.Tables.Add(chainSubscription);
                ds.Tables.Add(taskSubscription);

                return ds;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetTaskManagerConfiguration -----> ERROR : " + ex.ToString());
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(con);
                mLogger.Error("GetTaskManagerConfiguration -----> END");
            }
        }

        static List<string> ExtractDaysOfWeek(string scheduledJob)
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

        static HashSet<string> GetExistingTaskInfo()
        {
            RDBConnectionManager con = null;
            try
            {
                con = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                HashSet<string> taskInfo = new HashSet<string>(con.ExecuteQueryObject(string.Format(@"SELECT task_summary_id, task_name, task_type_name, module_name FROM dbo.ivp_rad_task_summary WHERE task_type_name IN ('Import', 'Load from Vendor','Load Vendor', 'Reporting and Extraction', 'Request', 'Response','Security Activation', 'Time Series Correction', 'TimeSeriesUpdate', 'Transport', 'Update Master', 'EOD Golden Copy Validation')"), RQueryType.Select).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["task_name"]) + "ž" + Convert.ToString(x["task_type_name"]) + "ž" + Convert.ToString(x["module_name"])));
                return taskInfo;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(con);
            }
        }

        public static void ValidateandSaveTaskManager(ObjectSet excelInfo, ObjectSet existingInfo, string userName)
        {
            mLogger.Error("ValidateandSaveTaskManager -----> START");

            try
            {
                HashSet<string> invalidChains = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                Dictionary<string, int> existingChainNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                HashSet<string> existingChainsInSheet = new HashSet<string>();

                Dictionary<string, ChainInfo> existingchainNameVsInfo = new Dictionary<string, ChainInfo>();
                Dictionary<string, ChainInfo> newChainNameVsInfo = new Dictionary<string, ChainInfo>();
                Dictionary<string, List<string>> existingChainVsTaskOrder = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> newChainVsTaskOrder = new Dictionary<string, List<string>>();
                Dictionary<string, Dictionary<string, TaskInfo>> existingChainNameVsTaskVsInfo = new Dictionary<string, Dictionary<string, TaskInfo>>();
                Dictionary<string, Dictionary<string, TaskInfo>> existingChainNameVsTaskVsInfoToCompare = new Dictionary<string, Dictionary<string, TaskInfo>>();
                Dictionary<string, Dictionary<string, TaskInfo>> newChainNameVsTaskVsInfo = new Dictionary<string, Dictionary<string, TaskInfo>>();
                Dictionary<string, Dictionary<string, ChainSubscription>> existingChainVsSubscription = new Dictionary<string, Dictionary<string, ChainSubscription>>(); ;
                Dictionary<string, Dictionary<string, ChainSubscription>> newChainVsSubscription = new Dictionary<string, Dictionary<string, ChainSubscription>>();
                Dictionary<string, Dictionary<string, Dictionary<string, TaskSubscription>>> existingChainVsTaskVsSubscription = new Dictionary<string, Dictionary<string, Dictionary<string, TaskSubscription>>>();
                Dictionary<string, Dictionary<string, Dictionary<string, TaskSubscription>>> newChainVsTaskVsSubscription = new Dictionary<string, Dictionary<string, Dictionary<string, TaskSubscription>>>();

                #region COLLECTION GENERATION
                HashSet<string> existingCalendars = SRMCommonRAD.GetCalendarNames();
                HashSet<string> recurrenceTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Recurring", "Non-Recurring" };
                HashSet<string> recurrencePatterns = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "daily", "weekly", "monthly" };
                Dictionary<string, string> dependencyRelations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "OR", "O" }, { "AND", "&" } };
                HashSet<string> subscriptionTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "SUCCESS", "FAILURE" };
                var displayNamesVsEmail = SRMCommonRAD.GetUsersDisplayNameVsEmail();
                HashSet<string> existingTaskInfo = GetExistingTaskInfo();
                existingInfo = GetTaskManagerConfiguration(new List<string>(), true);

                foreach (var row in existingInfo.Tables[0].AsEnumerable())
                {
                    var chainName = Convert.ToString(row[SM_ColumnNames.Chain_Name]);
                    existingChainNames.Add(chainName, Convert.ToInt32(row["chain_id"]));
                }

                foreach (var row in existingInfo.Tables[SRM_TaskManager_SheetNames.Chain_Information].AsEnumerable())
                {
                    var chainName = Convert.ToString(row[SM_ColumnNames.Chain_Name]);
                    var calendarName = Convert.ToString(row[SM_ColumnNames.Calendar_Name]);
                    var recurrenceType = Convert.ToString(row[SM_ColumnNames.Recurrence_Type]);
                    var startDateString = Convert.ToString(row["next_schedule_time"]);
                    var endDateString = Convert.ToString(row[SM_ColumnNames.End_Date]);
                    var startTimeString = Convert.ToString(row[SM_ColumnNames.Start_Time]);
                    var recurrencePattern = Convert.ToString(row[SM_ColumnNames.Recurrence_Pattern]);
                    var intervalString = Convert.ToString(row[SM_ColumnNames.Interval]);
                    var noOfRecurrenceString = Convert.ToString(row[SM_ColumnNames.Number_of_Recurrences]);
                    var timeIntervalOfRecurrenceString = Convert.ToString(row[SM_ColumnNames.Time_Interval_of_Recurrence]);
                    var neverEndJobString = Convert.ToString(row[SM_ColumnNames.Never_End_Job]);
                    var daysOfWeekString = Convert.ToString(row[SM_ColumnNames.Days_Of_Week]);

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

                    var chainInfo = new ChainInfo { CalendarName = calendarName, ChainName = chainName, EndDate = endDate, Interval = interval, NeverEndJob = neverEndJob, NumberOfRecurrences = noOfRecurrence, RecurrencePattern = recurrencePattern, RecurrenceType = recurrenceType, StartDate = startDate, StartTime = startTime, TimeIntervalOfRecurrence = timeInterval, DaysOfWeek = daysOfWeek };

                    if (existingChainNames.ContainsKey(chainName)) //old chain
                    {
                        var chainId = existingChainNames[chainName].ToString();
                        existingchainNameVsInfo.Add(chainId, chainInfo);
                    }
                }

                foreach (var row in existingInfo.Tables[1].AsEnumerable())
                {
                    var chainName = Convert.ToString(row[SM_ColumnNames.Chain_Name]);
                    var taskName = Convert.ToString(row[SM_ColumnNames.Task_Name]);
                    var taskType = Convert.ToString(row[SM_ColumnNames.Task_Type]);
                    var isMutedString = Convert.ToString(row[SM_ColumnNames.Is_Muted]);
                    var proceedOnFailString = Convert.ToString(row[SM_ColumnNames.Proceed_On_Fail]);
                    var reRunOnFailString = Convert.ToString(row[SM_ColumnNames.ReRun_on_Fail]);
                    var retryDurationString = Convert.ToString(row[SM_ColumnNames.Retry_Duration]);
                    var noOfRetriesString = Convert.ToString(row[SM_ColumnNames.Number_of_Retries]);


                    var onFailRunTask = Convert.ToString(row[SM_ColumnNames.On_Fail_Run_Task]);
                    var onFailRunTaskType = Convert.ToString(row[SM_ColumnNames.On_Fail_Run_Task_Task_Type]);
                    var onFailRunTaskModule = Convert.ToString(row[SM_ColumnNames.On_Fail_Run_Task_Module_Name]);


                    var timeOutString = Convert.ToString(row[SM_ColumnNames.Time_Out]);
                    var taskInstanceWaitString = Convert.ToString(row[SM_ColumnNames.Task_Instance_Wait]);
                    var taskWaitSubscriptionString = Convert.ToString(row[SM_ColumnNames.Task_Wait_Subscription]);
                    var moduleName = Convert.ToString(row[SM_ColumnNames.Module_Name]);
                    var taskNameKey = taskName + "ž" + taskType + "ž" + moduleName;

                    bool isMuted = false, proceedOnFail = false, rerunOnFail = false;
                    int retryDuration = 0, noOfRetries = 0, timeOut = 0, taskInstanceWait = 0;
                    string taskWaitSubscription = null;

                    if (!string.IsNullOrWhiteSpace(isMutedString))
                        Boolean.TryParse(isMutedString, out isMuted);

                    if (!string.IsNullOrWhiteSpace(proceedOnFailString))
                        Boolean.TryParse(proceedOnFailString, out proceedOnFail);

                    if (!string.IsNullOrWhiteSpace(reRunOnFailString))
                        Boolean.TryParse(reRunOnFailString, out rerunOnFail);

                    if (!string.IsNullOrWhiteSpace(retryDurationString))
                        Int32.TryParse(retryDurationString, out retryDuration);

                    if (!string.IsNullOrWhiteSpace(noOfRetriesString))
                        Int32.TryParse(noOfRetriesString, out noOfRetries);

                    if (!string.IsNullOrWhiteSpace(timeOutString))
                        Int32.TryParse(timeOutString, out timeOut);

                    if (!string.IsNullOrWhiteSpace(taskInstanceWaitString))
                        Int32.TryParse(taskInstanceWaitString, out taskInstanceWait);

                    if (!string.IsNullOrWhiteSpace(taskWaitSubscriptionString))
                    {
                        var users = taskWaitSubscriptionString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (users.Count() > 0)
                        {
                            var q = users.Where(y => displayNamesVsEmail.ContainsKey(y));
                            if (q.Count() > 0)
                                taskWaitSubscription = string.Join(",", q.Select(x => displayNamesVsEmail[x]));
                        }
                    }

                    var taskInfo = new TaskInfo { ChainName = chainName, IsMuted = isMuted, NumberOfRetries = noOfRetries, OnFailRunTask = new srmcommon.TaskInfoBase { TaskName = onFailRunTask, TaskType = onFailRunTaskType, TaskModuleName = onFailRunTaskModule }, ProceedOnFail = proceedOnFail, ReRunOnFail = rerunOnFail, RetryDuration = retryDuration, TaskInstanceWait = taskInstanceWait, TaskName = taskName, TaskType = taskType, TaskWaitSubscription = taskWaitSubscription, TimeOut = timeOut, ModuleName = moduleName };

                    var chainId = existingChainNames[chainName].ToString();

                    Dictionary<string, TaskInfo> taskNameVsInfo = null;
                    if (!existingChainNameVsTaskVsInfo.TryGetValue(chainId, out taskNameVsInfo))
                    {
                        taskNameVsInfo = new Dictionary<string, TaskInfo>();
                        existingChainNameVsTaskVsInfo.Add(chainId, taskNameVsInfo);
                    }
                    taskNameVsInfo.Add(taskNameKey, taskInfo);

                    List<string> tasks = null;
                    if (!existingChainVsTaskOrder.TryGetValue(chainId, out tasks))
                    {
                        tasks = new List<string>();
                        existingChainVsTaskOrder[chainId] = tasks;
                    }
                    tasks.Add(taskNameKey);
                }

                foreach (var row in existingInfo.Tables[SRM_TaskManager_SheetNames.Task_Dependency].AsEnumerable())
                {
                    var chainName = Convert.ToString(row[SM_ColumnNames.Chain_Name]);
                    var taskName = Convert.ToString(row[SM_ColumnNames.Task_Name]);
                    var taskType = Convert.ToString(row[SM_ColumnNames.Task_Type]);
                    var moduleName = Convert.ToString(row[SM_ColumnNames.Module_Name]);
                    var taskNameKey = taskName + "ž" + taskType + "ž" + moduleName;
                    var dependentOnTask = Convert.ToString(row[SM_ColumnNames.Dependent_On_Task]);
                    var dependentOnTaskType = Convert.ToString(row[SM_ColumnNames.Dependent_On_Task_Type]);
                    var dependentOnModule = Convert.ToString(row[SM_ColumnNames.Dependent_On_Module_Name]);
                    var dependencyRelation = Convert.ToString(row[SM_ColumnNames.Dependency_Relation]);

                    var chainId = existingChainNames[chainName].ToString();

                    if (!string.IsNullOrWhiteSpace(dependentOnTask) && !string.IsNullOrWhiteSpace(dependencyRelation))
                    {
                        Dictionary<string, TaskInfo> taskNameVsInfo = null;
                        if (existingChainNameVsTaskVsInfo.TryGetValue(chainId, out taskNameVsInfo))
                        {
                            taskNameVsInfo[taskNameKey].Dependencies = taskNameVsInfo[taskNameKey].Dependencies ?? new List<TaskDependency>();
                            taskNameVsInfo[taskNameKey].Dependencies.Add(new srmcommon.TaskDependency { TaskName = dependentOnTask, Relation = dependencyRelation, TaskType = dependentOnTaskType, TaskModuleName = dependentOnModule });
                        }
                    }
                }
                #endregion

                #region CHAIN LEVEL
                if (excelInfo.Tables.Contains(SRM_TaskManager_SheetNames.Chain_Information))
                {
                    foreach (var row in excelInfo.Tables[SRM_TaskManager_SheetNames.Chain_Information].AsEnumerable())
                    {
                        bool hasErrors = false;
                        var chainName = Convert.ToString(row[SM_ColumnNames.Chain_Name]);
                        var status = Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]);
                        if (string.IsNullOrWhiteSpace(status))
                        {
                            var calendarName = Convert.ToString(row[SM_ColumnNames.Calendar_Name]);
                            var recurrenceType = Convert.ToString(row[SM_ColumnNames.Recurrence_Type]);
                            var startDateString = Convert.ToString(row[SM_ColumnNames.Start_Date]);
                            var endDateString = Convert.ToString(row[SM_ColumnNames.End_Date]);
                            var startTimeString = Convert.ToString(row[SM_ColumnNames.Start_Time]);
                            var recurrencePattern = Convert.ToString(row[SM_ColumnNames.Recurrence_Pattern]);
                            var intervalString = Convert.ToString(row[SM_ColumnNames.Interval]);
                            var noOfRecurrenceString = Convert.ToString(row[SM_ColumnNames.Number_of_Recurrences]);
                            var timeIntervalOfRecurrenceString = Convert.ToString(row[SM_ColumnNames.Time_Interval_of_Recurrence]);
                            var neverEndJobString = Convert.ToString(row[SM_ColumnNames.Never_End_Job]);
                            var daysOfWeekString = Convert.ToString(row[SM_ColumnNames.Days_Of_Week]);

                            DateTime startDate = default(DateTime), endDate = default(DateTime);
                            TimeSpan startTime = default(TimeSpan);
                            bool neverEndJob = false;
                            int interval = 0, noOfRecurrence = 0, timeInterval = 0, daysOfWeek = 0;

                            if (String.IsNullOrWhiteSpace(chainName))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name cannot be blank.";
                            }
                            if (!existingCalendars.Contains(calendarName))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Calendar does not exist.";
                            }
                            else
                                calendarName = existingCalendars.Where(x => x.Equals(calendarName, StringComparison.OrdinalIgnoreCase)).First();

                            if (!recurrenceTypes.Contains(recurrenceType))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Recurrence type is invalid.";
                            }
                            if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && !(DateTime.TryParseExact(startDateString, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, @"MM/dd/yyyy hh\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, @"MM/dd/yyyy HH\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "MM/dd/yyyy hh:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "MM/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "M/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate) || DateTime.TryParseExact(startDateString, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out startDate)))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Start Date parsing failed.";
                            }
                            if (!hasErrors && recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && startDate > startDate.Date)
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Start Date cannot have time component.";
                            }
                            if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && !TimeSpan.TryParseExact(startTimeString, @"hh\:mm\:ss", null, out startTime))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Start Time is invalid.";
                            }
                            if (!hasErrors && recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && new DateTime(startDate.Year, startDate.Month, startDate.Day, startTime.Hours, startTime.Minutes, startTime.Seconds) <= DateTime.Now)
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Start Date has to be greater than current time.";
                            }
                            if (!string.IsNullOrWhiteSpace(recurrencePattern) && !recurrencePatterns.Contains(recurrencePattern))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Recurrence pattern is invalid.";
                            }
                            if ((string.IsNullOrWhiteSpace(intervalString) || !Int32.TryParse(intervalString, out interval)) && interval < 0)
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Interval is invalid.";
                            }
                            if (!string.IsNullOrWhiteSpace(endDateString) && !(DateTime.TryParseExact(endDateString, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, @"MM/dd/yyyy hh\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, @"MM/dd/yyyy HH\:mm\:ss", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "MM/dd/yyyy hh:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "MM/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "M/dd/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate) || DateTime.TryParseExact(endDateString, "M/d/yyyy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out endDate)))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " End Date is invalid.";
                            }
                            if ((string.IsNullOrWhiteSpace(noOfRecurrenceString) || !Int32.TryParse(noOfRecurrenceString, out noOfRecurrence)) && noOfRecurrence < 0)
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Number Of Recurrence is invalid.";
                            }

                            if ((string.IsNullOrWhiteSpace(timeIntervalOfRecurrenceString) || !Int32.TryParse(timeIntervalOfRecurrenceString, out timeInterval)) && timeInterval < 0)
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Time Interval Of Recurrence is invalid.";
                            }
                            if (!string.IsNullOrWhiteSpace(neverEndJobString) && !Boolean.TryParse(neverEndJobString, out neverEndJob))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Never End Job is invalid.";
                            }
                            if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && !neverEndJob && endDate == default(DateTime))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Either set Never End Job to true or mention the End Date.";
                            }
                            if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && !neverEndJob && endDate != default(DateTime) && endDate < new DateTime(startDate.Year, startDate.Month, startDate.Day, startTime.Hours, startTime.Minutes, startTime.Seconds))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " End Date should not be less than Start date.";
                            }
                            if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(daysOfWeekString))
                            {
                                var days = daysOfWeekString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (days.Length > 0)
                                {
                                    var invalidDays = days.Where(x => !DaysOfWeekStringToNumber.ContainsKey(x));
                                    if (invalidDays.Count() > 0)
                                    {
                                        hasErrors = true;
                                        row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " There are invalid days in Days Of Week.";
                                    }
                                    else
                                        daysOfWeek = Enumerable.Sum(days.Select(x => Convert.ToInt32(DaysOfWeekStringToNumber[x])));
                                }
                            }

                            if (recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && (noOfRecurrence <= 0 || interval <= 0 || timeInterval <= 0))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Invalid (Number Of Recurrences OR Interval OR Time Interval Of Recurrence).";
                            }

                            if (!hasErrors && recurrenceType.Equals("Recurring", StringComparison.OrdinalIgnoreCase) && recurrencePattern.Equals("daily", StringComparison.OrdinalIgnoreCase) && noOfRecurrence > 0 && timeInterval > 0 && startDate.Add(startTime).AddMinutes((noOfRecurrence - 1) * timeInterval).Date == startDate.Date.AddDays(1))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Cannot schedule the chain since all the recurrences of this chain will not end on start date.";
                            }

                            if (!hasErrors)
                            {
                                var chainInfo = new ChainInfo { CalendarName = calendarName, ChainName = chainName, EndDate = endDate, Interval = interval, NeverEndJob = neverEndJob, NumberOfRecurrences = noOfRecurrence, RecurrencePattern = recurrencePattern, RecurrenceType = recurrenceType, StartDate = startDate, StartTime = startTime, TimeIntervalOfRecurrence = timeInterval, DaysOfWeek = daysOfWeek };

                                if (existingChainNames.ContainsKey(chainName)) //old chain
                                {
                                    var chainId = existingChainNames[chainName].ToString();
                                    existingchainNameVsInfo[chainId] = chainInfo;
                                    existingChainsInSheet.Add(chainId);
                                }
                                else //new chain
                                    newChainNameVsInfo.Add(chainName, chainInfo);
                            }
                            else
                            {
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                invalidChains.Add(chainName);
                            }
                        }
                        else if (status == SRMMigrationStatus.Failed)
                        {
                            invalidChains.Add(chainName);
                        }
                    }
                }
                #endregion

                #region TASK LEVEL
                if (excelInfo.Tables.Contains(SRM_TaskManager_SheetNames.Task_Information))
                {
                    var firstOccurredChain = new HashSet<string>();

                    foreach (var row in excelInfo.Tables[SRM_TaskManager_SheetNames.Task_Information].AsEnumerable())
                    {
                        var chainName = Convert.ToString(row[SM_ColumnNames.Chain_Name]);
                        var status = Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]);
                        if (string.IsNullOrWhiteSpace(status))
                        {
                            var hasErrors = false;
                            var taskName = Convert.ToString(row[SM_ColumnNames.Task_Name]);
                            var taskType = Convert.ToString(row[SM_ColumnNames.Task_Type]);
                            var moduleName = Convert.ToString(row[SM_ColumnNames.Module_Name]);
                            var isMutedString = Convert.ToString(row[SM_ColumnNames.Is_Muted]);
                            var proceedOnFailString = Convert.ToString(row[SM_ColumnNames.Proceed_On_Fail]);
                            var reRunOnFailString = Convert.ToString(row[SM_ColumnNames.ReRun_on_Fail]);
                            var retryDurationString = Convert.ToString(row[SM_ColumnNames.Retry_Duration]);
                            var noOfRetriesString = Convert.ToString(row[SM_ColumnNames.Number_of_Retries]);

                            var timeOutString = Convert.ToString(row[SM_ColumnNames.Time_Out]);
                            var taskInstanceWaitString = Convert.ToString(row[SM_ColumnNames.Task_Instance_Wait]);
                            var taskWaitSubscriptionString = Convert.ToString(row[SM_ColumnNames.Task_Wait_Subscription]);

                            var taskNameKey = taskName + "ž" + taskType + "ž" + moduleName;

                            bool isMuted = false, proceedOnFail = false, rerunOnFail = false;
                            int retryDuration = 0, noOfRetries = 0, timeOut = 0, taskInstanceWait = 0;
                            string taskWaitSubscription = null;

                            if (invalidChains.Contains(chainName))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
                            }
                            else
                            {
                                if (!existingChainNames.ContainsKey(chainName) && !newChainNameVsInfo.ContainsKey(chainName))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Invalid chain name.";
                                }
                                if (String.IsNullOrWhiteSpace(chainName))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name cannot be blank.";
                                }

                                if (!firstOccurredChain.Contains(chainName))
                                {
                                    firstOccurredChain.Add(chainName);
                                    int chainId = 0;
                                    if (existingChainNames.TryGetValue(chainName, out chainId))
                                    {
                                        string chIdString = chainId.ToString();
                                        existingChainNameVsTaskVsInfoToCompare[chIdString] = existingChainNameVsTaskVsInfo[chIdString];
                                        existingChainNameVsTaskVsInfo.Remove(chIdString);
                                        existingChainVsTaskOrder.Remove(chIdString);
                                    }
                                }

                                if (!existingTaskInfo.Contains(taskNameKey))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Task Name, Task Type or Module Name is invalid.";
                                }
                                if (!string.IsNullOrWhiteSpace(isMutedString) && !Boolean.TryParse(isMutedString, out isMuted))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Is Muted is invalid.";
                                }
                                if (!string.IsNullOrWhiteSpace(proceedOnFailString) && !Boolean.TryParse(proceedOnFailString, out proceedOnFail))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Proceed On Fail is invalid.";
                                }
                                if (!string.IsNullOrWhiteSpace(reRunOnFailString) && !Boolean.TryParse(reRunOnFailString, out rerunOnFail))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " ReRun On Fail is invalid.";
                                }
                                if (!string.IsNullOrWhiteSpace(retryDurationString) && (!Int32.TryParse(retryDurationString, out retryDuration) || retryDuration < 0))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Retry Duration is invalid.";
                                }
                                if (!string.IsNullOrWhiteSpace(noOfRetriesString) && (!Int32.TryParse(noOfRetriesString, out noOfRetries) || noOfRetries < 0))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Number of Retries is invalid.";
                                }
                                if (!string.IsNullOrWhiteSpace(timeOutString) && (!Int32.TryParse(timeOutString, out timeOut) || timeOut < 0))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Time Out is invalid.";
                                }
                                if (!string.IsNullOrWhiteSpace(taskInstanceWaitString) && (!Int32.TryParse(taskInstanceWaitString, out taskInstanceWait) || taskInstanceWait < 0))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Task Instance Wait is invalid.";
                                }
                                if (!string.IsNullOrWhiteSpace(taskWaitSubscriptionString))
                                {
                                    var users = taskWaitSubscriptionString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (users.Count() > 0)
                                    {
                                        var invalidUsers = users.Where(x => !displayNamesVsEmail.ContainsKey(x));
                                        if (invalidUsers.Count() > 0)
                                        {
                                            hasErrors = true;
                                            row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Task Wait Subscription has invalid users(" + string.Join(",", invalidUsers) + ").";
                                        }
                                        else
                                            taskWaitSubscription = string.Join(",", users.Select(x => displayNamesVsEmail[x]));
                                    }
                                }
                                if ((timeOut > 0 || taskInstanceWait > 0) && string.IsNullOrEmpty(taskWaitSubscription))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Please set Task Wait Subscription if Task Time Out or Task Instance Wait is set.";
                                }

                                if (hasErrors)
                                {
                                    row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                    invalidChains.Add(chainName);

                                    if (existingChainNames.ContainsKey(chainName)) //old chain
                                    {
                                        var chainId = existingChainNames[chainName].ToString();
                                        if (existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                                            existingChainNameVsTaskVsInfo.Remove(chainId);
                                        if (existingChainVsTaskOrder.ContainsKey(chainId))
                                            existingChainVsTaskOrder.Remove(chainId);
                                        if (existingChainsInSheet.Contains(chainId))
                                            existingChainsInSheet.Remove(chainId);

                                        if (existingchainNameVsInfo.ContainsKey(chainId))
                                            existingchainNameVsInfo.Remove(chainId);
                                    }
                                    else
                                    {
                                        if (newChainNameVsInfo.ContainsKey(chainName))
                                            newChainNameVsInfo.Remove(chainName);
                                        if (newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                            newChainNameVsTaskVsInfo.Remove(chainName);
                                        if (newChainVsTaskOrder.ContainsKey(chainName))
                                            newChainVsTaskOrder.Remove(chainName);
                                    }
                                }
                                else
                                {
                                    var taskInfo = new TaskInfo
                                    {
                                        ChainName = chainName,
                                        IsMuted = isMuted,
                                        NumberOfRetries = noOfRetries,
                                        //OnFailRunTask = onFailRunTaskString,
                                        ProceedOnFail = proceedOnFail,
                                        ReRunOnFail = rerunOnFail,
                                        RetryDuration = retryDuration,
                                        TaskInstanceWait = taskInstanceWait,
                                        TaskName = taskName,
                                        TaskType = taskType,
                                        ModuleName = moduleName,
                                        TaskWaitSubscription = taskWaitSubscription,
                                        TimeOut = timeOut,
                                        Dependencies = new List<srmcommon.TaskDependency>()
                                    };

                                    if (existingChainNames.ContainsKey(chainName)) //old chain
                                    {
                                        var chainId = existingChainNames[chainName].ToString();

                                        Dictionary<string, TaskInfo> taskNameVsInfo = null;
                                        if (!existingChainNameVsTaskVsInfo.TryGetValue(chainId, out taskNameVsInfo))
                                        {
                                            taskNameVsInfo = new Dictionary<string, TaskInfo>();
                                            existingChainNameVsTaskVsInfo.Add(chainId, taskNameVsInfo);
                                        }
                                        taskNameVsInfo.Add(taskNameKey, taskInfo);

                                        existingChainsInSheet.Add(chainId);

                                        List<string> tasks = null;
                                        if (!existingChainVsTaskOrder.TryGetValue(chainId, out tasks))
                                        {
                                            tasks = new List<string>();
                                            existingChainVsTaskOrder[chainId] = tasks;
                                        }
                                        tasks.Add(taskNameKey);
                                    }
                                    else //new chain
                                    {
                                        Dictionary<string, TaskInfo> taskNameVsInfo = null;
                                        if (!newChainNameVsTaskVsInfo.TryGetValue(chainName, out taskNameVsInfo))
                                        {
                                            taskNameVsInfo = new Dictionary<string, TaskInfo>();
                                            newChainNameVsTaskVsInfo.Add(chainName, taskNameVsInfo);
                                        }
                                        taskNameVsInfo.Add(taskNameKey, taskInfo);

                                        List<string> tasks = null;
                                        if (!newChainVsTaskOrder.TryGetValue(chainName, out tasks))
                                        {
                                            tasks = new List<string>();
                                            newChainVsTaskOrder[chainName] = tasks;
                                        }
                                        tasks.Add(taskNameKey);
                                    }
                                }
                            }
                        }
                        else if (status == SRMMigrationStatus.Failed)
                        {
                            invalidChains.Add(chainName);

                            if (existingChainNames.ContainsKey(chainName)) //old chain
                            {
                                var chainId = existingChainNames[chainName].ToString();
                                if (existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                                    existingChainNameVsTaskVsInfo.Remove(chainId);
                                if (existingChainVsTaskOrder.ContainsKey(chainId))
                                    existingChainVsTaskOrder.Remove(chainId);
                                if (existingChainsInSheet.Contains(chainId))
                                    existingChainsInSheet.Remove(chainId);

                                if (existingchainNameVsInfo.ContainsKey(chainId))
                                    existingchainNameVsInfo.Remove(chainId);
                            }
                            else
                            {
                                if (newChainNameVsInfo.ContainsKey(chainName))
                                    newChainNameVsInfo.Remove(chainName);
                                if (newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                    newChainNameVsTaskVsInfo.Remove(chainName);
                                if (newChainVsTaskOrder.ContainsKey(chainName))
                                    newChainVsTaskOrder.Remove(chainName);
                            }
                        }
                    }

                    foreach (var row in excelInfo.Tables[SRM_TaskManager_SheetNames.Task_Information].AsEnumerable())
                    {
                        var chainName = Convert.ToString(row[SM_ColumnNames.Chain_Name]);
                        var status = Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]);
                        if (string.IsNullOrWhiteSpace(status))
                        {
                            var hasErrors = false;
                            var taskName = Convert.ToString(row[SM_ColumnNames.Task_Name]);
                            var taskType = Convert.ToString(row[SM_ColumnNames.Task_Type]);
                            var moduleName = Convert.ToString(row[SM_ColumnNames.Module_Name]);

                            var onFailRunTask = Convert.ToString(row[SM_ColumnNames.On_Fail_Run_Task]);
                            var onFailRunTaskType = Convert.ToString(row[SM_ColumnNames.On_Fail_Run_Task_Task_Type]);
                            var onFailRunTaskModuleName = Convert.ToString(row[SM_ColumnNames.On_Fail_Run_Task_Module_Name]);

                            var taskNameKey = taskName + "ž" + taskType + "ž" + moduleName;
                            var onFailRunTaskKey = onFailRunTask + "ž" + onFailRunTaskType + "ž" + onFailRunTaskModuleName;

                            if (!String.IsNullOrWhiteSpace(chainName) && !invalidChains.Contains(chainName) && (existingChainNames.ContainsKey(chainName) || newChainNameVsInfo.ContainsKey(chainName)))
                            {
                                if (existingTaskInfo.Contains(taskNameKey))
                                {
                                    if (!String.IsNullOrWhiteSpace(onFailRunTask))
                                    {
                                        if (!existingTaskInfo.Contains(onFailRunTaskKey))
                                        {
                                            hasErrors = true;
                                            row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " On Fail Run Task Information is invalid.";
                                        }
                                        if (existingChainNames.ContainsKey(chainName)) //old chain
                                        {
                                            var chainId = existingChainNames[chainName].ToString();

                                            Dictionary<string, TaskInfo> taskNameVsInfo = null;
                                            if (existingChainNameVsTaskVsInfo.TryGetValue(chainId, out taskNameVsInfo))
                                            {
                                                if (!taskNameVsInfo.ContainsKey(onFailRunTaskKey))
                                                {
                                                    hasErrors = true;
                                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " On Fail Run Task is not part of the chain.";
                                                }
                                                else
                                                    taskNameVsInfo[taskNameKey].OnFailRunTask = new TaskInfoBase { TaskName = onFailRunTask, TaskType = onFailRunTaskType, TaskModuleName = onFailRunTaskModuleName };
                                            }
                                        }
                                        else
                                        {
                                            Dictionary<string, TaskInfo> taskNameVsInfo = null;
                                            if (newChainNameVsTaskVsInfo.TryGetValue(chainName, out taskNameVsInfo))
                                            {
                                                if (!taskNameVsInfo.ContainsKey(onFailRunTaskKey))
                                                {
                                                    hasErrors = true;
                                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " On Fail Run Task is not part of the chain.";
                                                }
                                                else
                                                    taskNameVsInfo[taskNameKey].OnFailRunTask = new TaskInfoBase { TaskName = onFailRunTask, TaskType = onFailRunTaskType, TaskModuleName = onFailRunTaskModuleName };
                                            }
                                        }
                                    }
                                }
                            }
                            if (hasErrors)
                            {
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                invalidChains.Add(chainName);

                                if (existingChainNames.ContainsKey(chainName)) //old chain
                                {
                                    var chainId = existingChainNames[chainName].ToString();
                                    if (existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                                        existingChainNameVsTaskVsInfo.Remove(chainId);
                                    if (existingChainVsTaskOrder.ContainsKey(chainId))
                                        existingChainVsTaskOrder.Remove(chainId);
                                    if (existingChainsInSheet.Contains(chainId))
                                        existingChainsInSheet.Remove(chainId);

                                    if (existingchainNameVsInfo.ContainsKey(chainId))
                                        existingchainNameVsInfo.Remove(chainId);
                                }
                                else
                                {
                                    if (newChainNameVsInfo.ContainsKey(chainName))
                                        newChainNameVsInfo.Remove(chainName);
                                    if (newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                        newChainNameVsTaskVsInfo.Remove(chainName);
                                    if (newChainVsTaskOrder.ContainsKey(chainName))
                                        newChainVsTaskOrder.Remove(chainName);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region TASK DEPENDENCY
                if (excelInfo.Tables.Contains(SRM_TaskManager_SheetNames.Task_Dependency))
                {
                    Dictionary<string, int> chainVsEmptyDependency = new Dictionary<string, int>();
                    foreach (var row in excelInfo.Tables[SRM_TaskManager_SheetNames.Task_Dependency].AsEnumerable())
                    {
                        var chainName = Convert.ToString(row[SM_ColumnNames.Chain_Name]);
                        var status = Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]);
                        var chainIdd = 0;
                        existingChainNames.TryGetValue(chainName, out chainIdd);

                        if (existingChainsInSheet.Contains(chainIdd.ToString()) || string.IsNullOrWhiteSpace(status))
                        {
                            var hasErrors = false;
                            var taskName = Convert.ToString(row[SM_ColumnNames.Task_Name]);
                            var taskType = Convert.ToString(row[SM_ColumnNames.Task_Type]);
                            var moduleName = Convert.ToString(row[SM_ColumnNames.Module_Name]);
                            var dependentOnModuleName = Convert.ToString(row[SM_ColumnNames.Dependent_On_Module_Name]);
                            var dependentOnTaskType = Convert.ToString(row[SM_ColumnNames.Dependent_On_Task_Type]);
                            var dependentOnTask = Convert.ToString(row[SM_ColumnNames.Dependent_On_Task]);
                            var dependencyRelation = Convert.ToString(row[SM_ColumnNames.Dependency_Relation]);
                            var taskNameKey = taskName + "ž" + taskType + "ž" + moduleName;
                            var dependentTaskKey = dependentOnTask + "ž" + dependentOnTaskType + "ž" + dependentOnModuleName;

                            if (invalidChains.Contains(chainName))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
                            }
                            else
                            {
                                if (String.IsNullOrWhiteSpace(chainName))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name cannot be blank.";
                                }
                                if (existingChainNames.ContainsKey(chainName)) //old chain
                                {
                                    var chainId = existingChainNames[chainName].ToString();

                                    if (!existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                                    {
                                        hasErrors = true;
                                        row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name is invalid.";
                                    }
                                    else if (!existingChainNameVsTaskVsInfo[chainId].ContainsKey(taskNameKey))
                                    {
                                        hasErrors = true;
                                        row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Task Name, Task Type or Module Name is invalid for the chain.";
                                    }
                                }
                                else
                                {
                                    if (!newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                    {
                                        hasErrors = true;
                                        row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name is invalid.";
                                    }
                                    else if (!newChainNameVsTaskVsInfo[chainName].ContainsKey(taskNameKey))
                                    {
                                        hasErrors = true;
                                        row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Task Name, Task Type or Module Name is invalid for the chain.";
                                    }
                                }


                                if (!string.IsNullOrWhiteSpace(dependencyRelation))
                                {
                                    if (!dependencyRelations.ContainsKey(dependencyRelation))
                                    {
                                        hasErrors = true;
                                        row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Dependency Relation is invalid.";
                                    }
                                }
                                else
                                {
                                    if (!chainVsEmptyDependency.ContainsKey(chainName))
                                        chainVsEmptyDependency[chainName] = 1;
                                    else
                                        chainVsEmptyDependency[chainName]++;
                                }

                                if (!string.IsNullOrWhiteSpace(dependentOnTask))
                                {
                                    if (existingChainNames.ContainsKey(chainName)) //old chain
                                    {
                                        var chainId = existingChainNames[chainName].ToString();

                                        if (!existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                                        {
                                            hasErrors = true;
                                            row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name is invalid.";
                                        }
                                        else if (!existingChainNameVsTaskVsInfo[chainId].ContainsKey(dependentTaskKey))
                                        {
                                            hasErrors = true;
                                            row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Dependency on Task Information is invalid.";
                                        }
                                    }
                                    else
                                    {
                                        if (!newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                        {
                                            hasErrors = true;
                                            row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name is invalid.";
                                        }
                                        else if (!newChainNameVsTaskVsInfo[chainName].ContainsKey(dependentTaskKey))
                                        {
                                            hasErrors = true;
                                            row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Dependency on Task Information is invalid.";
                                        }
                                    }
                                }
                                else if (string.IsNullOrWhiteSpace(dependentOnTask) && !string.IsNullOrWhiteSpace(dependencyRelation))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Dependent On Task cannot be empty if Dependency Relation is mentioned.";
                                }

                                if (chainVsEmptyDependency.ContainsKey(chainName) && chainVsEmptyDependency[chainName] > 1)
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " A chain cannot have more than one Head Task.";
                                }

                                if (hasErrors)
                                {
                                    row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                    invalidChains.Add(chainName);

                                    if (existingChainNames.ContainsKey(chainName)) //old chain
                                    {
                                        var chainId = existingChainNames[chainName].ToString();
                                        if (existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                                            existingChainNameVsTaskVsInfo.Remove(chainId);
                                        if (existingChainVsTaskOrder.ContainsKey(chainId))
                                            existingChainVsTaskOrder.Remove(chainId);
                                        if (existingChainsInSheet.Contains(chainId))
                                            existingChainsInSheet.Remove(chainId);

                                        if (existingchainNameVsInfo.ContainsKey(chainId))
                                            existingchainNameVsInfo.Remove(chainId);
                                    }
                                    else
                                    {
                                        if (newChainNameVsInfo.ContainsKey(chainName))
                                            newChainNameVsInfo.Remove(chainName);
                                        if (newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                            newChainNameVsTaskVsInfo.Remove(chainName);
                                        if (newChainVsTaskOrder.ContainsKey(chainName))
                                            newChainVsTaskOrder.Remove(chainName);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(dependentOnTask) && !string.IsNullOrWhiteSpace(dependencyRelation))
                                    {
                                        if (existingChainNames.ContainsKey(chainName)) //old chain
                                        {
                                            var chainId = existingChainNames[chainName].ToString();
                                            existingChainsInSheet.Add(chainId);

                                            Dictionary<string, TaskInfo> taskNameVsInfo = null;
                                            if (existingChainNameVsTaskVsInfo.TryGetValue(chainId, out taskNameVsInfo))
                                                taskNameVsInfo[taskNameKey].Dependencies.Add(new srmcommon.TaskDependency { TaskName = dependentOnTask, Relation = dependencyRelation, TaskType = dependentOnTaskType, TaskModuleName = dependentOnModuleName });
                                        }
                                        else //new chain
                                        {
                                            Dictionary<string, TaskInfo> taskNameVsInfo = null;
                                            if (newChainNameVsTaskVsInfo.TryGetValue(chainName, out taskNameVsInfo))
                                                taskNameVsInfo[taskNameKey].Dependencies.Add(new srmcommon.TaskDependency { TaskName = dependentOnTask, Relation = dependencyRelation, TaskType = dependentOnTaskType, TaskModuleName = dependentOnModuleName });
                                        }
                                    }
                                }
                            }
                        }
                        else if (status == SRMMigrationStatus.Failed)
                        {
                            invalidChains.Add(chainName);

                            if (existingChainNames.ContainsKey(chainName)) //old chain
                            {
                                var chainId = existingChainNames[chainName].ToString();
                                if (existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                                    existingChainNameVsTaskVsInfo.Remove(chainId);
                                if (existingChainVsTaskOrder.ContainsKey(chainId))
                                    existingChainVsTaskOrder.Remove(chainId);
                                if (existingChainsInSheet.Contains(chainId))
                                    existingChainsInSheet.Remove(chainId);

                                if (existingchainNameVsInfo.ContainsKey(chainId))
                                    existingchainNameVsInfo.Remove(chainId);
                            }
                            else
                            {
                                if (newChainNameVsInfo.ContainsKey(chainName))
                                    newChainNameVsInfo.Remove(chainName);
                                if (newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                    newChainNameVsTaskVsInfo.Remove(chainName);
                                if (newChainVsTaskOrder.ContainsKey(chainName))
                                    newChainVsTaskOrder.Remove(chainName);
                            }
                        }
                    }
                }
                #endregion

                #region CHAIN SUBSCRIPTION
                if (excelInfo.Tables.Contains(SRM_TaskManager_SheetNames.Chain_Subscription))
                {
                    Dictionary<string, int> chainVsEmptyDependency = new Dictionary<string, int>();
                    foreach (var row in excelInfo.Tables[SRM_TaskManager_SheetNames.Chain_Subscription].AsEnumerable())
                    {
                        if (string.IsNullOrWhiteSpace(Convert.ToString(row[SRMSpecialColumnNames.Sync_Status])))
                        {
                            var hasErrors = false;
                            var chainName = Convert.ToString(row[SM_ColumnNames.Chain_Name]);
                            var subscriptionType = Convert.ToString(row[SM_ColumnNames.Subscription_Type]);
                            var toMail = Convert.ToString(row[SM_ColumnNames.To_Mail]);
                            var subject = Convert.ToString(row[SM_ColumnNames.Subject]);
                            var body = Convert.ToString(row[SM_ColumnNames.Body]);
                            string toMailIds = null;

                            if (invalidChains.Contains(chainName))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
                            }
                            else
                            {
                                if (String.IsNullOrWhiteSpace(chainName))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name cannot be blank.";
                                }
                                if (!existingChainNames.ContainsKey(chainName) && !newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name is invalid.";
                                }
                                if (!subscriptionTypes.Contains(subscriptionType))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Subscription Type is invalid.";
                                }
                                if (!string.IsNullOrWhiteSpace(toMail))
                                {
                                    var users = toMail.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (users.Count() > 0)
                                    {
                                        var invalidUsers = users.Where(x => !displayNamesVsEmail.ContainsKey(x));
                                        if (invalidUsers.Count() > 0)
                                        {
                                            hasErrors = true;
                                            row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " To Mail Id has invalid users(" + string.Join(",", invalidUsers) + ").";
                                        }
                                        else
                                            toMailIds = string.Join(",", users.Select(x => displayNamesVsEmail[x]));
                                    }
                                }

                                if (hasErrors)
                                {
                                    row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                    invalidChains.Add(chainName);

                                    if (existingChainNames.ContainsKey(chainName)) //old chain
                                    {
                                        var chainId = existingChainNames[chainName].ToString();
                                        if (existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                                            existingChainNameVsTaskVsInfo.Remove(chainId);
                                        if (existingChainVsTaskOrder.ContainsKey(chainId))
                                            existingChainVsTaskOrder.Remove(chainId);
                                        if (existingChainsInSheet.Contains(chainId))
                                            existingChainsInSheet.Remove(chainId);

                                        if (existingchainNameVsInfo.ContainsKey(chainId))
                                            existingchainNameVsInfo.Remove(chainId);
                                    }
                                    else
                                    {
                                        if (newChainNameVsInfo.ContainsKey(chainName))
                                            newChainNameVsInfo.Remove(chainName);
                                        if (newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                            newChainNameVsTaskVsInfo.Remove(chainName);
                                        if (newChainVsTaskOrder.ContainsKey(chainName))
                                            newChainVsTaskOrder.Remove(chainName);
                                    }
                                }
                                else
                                {
                                    subscriptionType = subscriptionType.ToUpper();
                                    if (existingChainNames.ContainsKey(chainName)) //old chain
                                    {
                                        var chainId = existingChainNames[chainName].ToString();
                                        Dictionary<string, ChainSubscription> subs = null;
                                        ChainSubscription subsInfo = new srmcommon.ChainSubscription { body = body, ChainName = chainName, Subject = subject, To = toMailIds, Type = subscriptionType };
                                        if (!existingChainVsSubscription.TryGetValue(chainId, out subs))
                                        {
                                            subs = new Dictionary<string, ChainSubscription>();
                                            existingChainVsSubscription[chainId] = subs;
                                        }
                                        subs[subscriptionType] = subsInfo;
                                        existingChainsInSheet.Add(chainId);
                                    }
                                    else //new chain
                                    {
                                        Dictionary<string, ChainSubscription> subs = null;
                                        ChainSubscription subsInfo = new srmcommon.ChainSubscription { body = body, ChainName = chainName, Subject = subject, To = toMailIds, Type = subscriptionType };
                                        if (!newChainVsSubscription.TryGetValue(chainName, out subs))
                                        {
                                            subs = new Dictionary<string, ChainSubscription>();
                                            newChainVsSubscription[chainName] = subs;
                                        }
                                        subs[subscriptionType] = subsInfo;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region TASK SUBSCRIPTION
                if (excelInfo.Tables.Contains(SRM_TaskManager_SheetNames.Task_Subscription))
                {
                    Dictionary<string, int> chainVsEmptyDependency = new Dictionary<string, int>();
                    foreach (var row in excelInfo.Tables[SRM_TaskManager_SheetNames.Task_Subscription].AsEnumerable())
                    {
                        if (string.IsNullOrWhiteSpace(Convert.ToString(row[SRMSpecialColumnNames.Sync_Status])))
                        {
                            var hasErrors = false;
                            var chainName = Convert.ToString(row[SM_ColumnNames.Chain_Name]);
                            var taskName = Convert.ToString(row[SM_ColumnNames.Task_Name]);
                            var taskType = Convert.ToString(row[SM_ColumnNames.Task_Type]);
                            var moduleName = Convert.ToString(row[SM_ColumnNames.Module_Name]);
                            var subscriptionType = Convert.ToString(row[SM_ColumnNames.Subscription_Type]);
                            var toMail = Convert.ToString(row[SM_ColumnNames.To_Mail]);
                            var subject = Convert.ToString(row[SM_ColumnNames.Subject]);
                            var body = Convert.ToString(row[SM_ColumnNames.Body]);
                            var taskNameKey = taskName + "ž" + taskType + "ž" + moduleName;

                            string toMailIds = null;

                            if (invalidChains.Contains(chainName))
                            {
                                hasErrors = true;
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
                            }
                            else
                            {
                                if (String.IsNullOrWhiteSpace(chainName))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name cannot be blank.";
                                }
                                if (existingChainNames.ContainsKey(chainName))
                                {
                                    var chainId = existingChainNames[chainName].ToString();
                                    if (!existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                                    {
                                        hasErrors = true;
                                        row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name is invalid.";
                                    }
                                    else if (!existingChainNameVsTaskVsInfo[chainId].ContainsKey(taskNameKey))
                                    {
                                        hasErrors = true;
                                        row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Task Name is invalid for the chain.";
                                    }
                                }
                                else
                                {
                                    if (!newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                    {
                                        hasErrors = true;
                                        row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Chain Name is invalid.";
                                    }
                                    else if (!newChainNameVsTaskVsInfo[chainName].ContainsKey(taskNameKey))
                                    {
                                        hasErrors = true;
                                        row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Task Name is invalid for the chain.";
                                    }
                                }

                                if (!subscriptionTypes.Contains(subscriptionType))
                                {
                                    hasErrors = true;
                                    row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " Subscription Type is invalid.";
                                }
                                if (!string.IsNullOrWhiteSpace(toMail))
                                {
                                    var users = toMail.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (users.Count() > 0)
                                    {
                                        var invalidUsers = users.Where(x => !displayNamesVsEmail.ContainsKey(x));
                                        if (invalidUsers.Count() > 0)
                                        {
                                            hasErrors = true;
                                            row[SRMSpecialColumnNames.Remarks] = Convert.ToString(row[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + " To Mail Id has invalid users(" + string.Join(",", invalidUsers) + ").";
                                        }
                                        else
                                            toMailIds = string.Join(",", users.Select(x => displayNamesVsEmail[x]));
                                    }
                                }

                                if (hasErrors)
                                {
                                    row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                    invalidChains.Add(chainName);

                                    if (existingChainNames.ContainsKey(chainName)) //old chain
                                    {
                                        var chainId = existingChainNames[chainName].ToString();
                                        if (existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                                            existingChainNameVsTaskVsInfo.Remove(chainId);
                                        if (existingChainVsTaskOrder.ContainsKey(chainId))
                                            existingChainVsTaskOrder.Remove(chainId);
                                        if (existingChainsInSheet.Contains(chainId))
                                            existingChainsInSheet.Remove(chainId);

                                        if (existingchainNameVsInfo.ContainsKey(chainId))
                                            existingchainNameVsInfo.Remove(chainId);
                                    }
                                    else
                                    {
                                        if (newChainNameVsInfo.ContainsKey(chainName))
                                            newChainNameVsInfo.Remove(chainName);
                                        if (newChainNameVsTaskVsInfo.ContainsKey(chainName))
                                            newChainNameVsTaskVsInfo.Remove(chainName);
                                        if (newChainVsTaskOrder.ContainsKey(chainName))
                                            newChainVsTaskOrder.Remove(chainName);
                                    }
                                }
                                else
                                {
                                    subscriptionType = subscriptionType.ToUpper();
                                    if (existingChainNames.ContainsKey(chainName)) //old chain
                                    {
                                        var chainId = existingChainNames[chainName].ToString();

                                        Dictionary<string, Dictionary<string, TaskSubscription>> subs = null;
                                        Dictionary<string, TaskSubscription> tasksub = null;
                                        TaskSubscription subsInfo = new TaskSubscription { body = body, ChainName = chainName, Subject = subject, To = toMailIds, Type = subscriptionType, TaskName = taskName, TaskType = taskType, ModuleName = moduleName };
                                        if (!existingChainVsTaskVsSubscription.TryGetValue(chainId, out subs))
                                        {
                                            subs = new Dictionary<string, Dictionary<string, TaskSubscription>>();
                                            existingChainVsTaskVsSubscription[chainId] = subs;
                                        }
                                        if (!subs.TryGetValue(taskNameKey, out tasksub))
                                        {
                                            tasksub = new Dictionary<string, TaskSubscription>();
                                            subs[taskNameKey] = tasksub;
                                        }
                                        existingChainsInSheet.Add(chainId);
                                        tasksub[subscriptionType] = subsInfo;
                                    }
                                    else //new chain
                                    {
                                        Dictionary<string, Dictionary<string, TaskSubscription>> subs = null;
                                        Dictionary<string, TaskSubscription> tasksub = null;
                                        TaskSubscription subsInfo = new TaskSubscription { body = body, ChainName = chainName, Subject = subject, To = toMailIds, Type = subscriptionType, TaskName = taskName, TaskType = taskType, ModuleName = moduleName };
                                        if (!newChainVsTaskVsSubscription.TryGetValue(chainName, out subs))
                                        {
                                            subs = new Dictionary<string, Dictionary<string, TaskSubscription>>();
                                            newChainVsTaskVsSubscription[chainName] = subs;
                                        }
                                        if (!subs.TryGetValue(taskNameKey, out tasksub))
                                        {
                                            tasksub = new Dictionary<string, TaskSubscription>();
                                            subs[taskNameKey] = tasksub;
                                        }
                                        tasksub[subscriptionType] = subsInfo;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                foreach (var chainLevel in existingChainNameVsTaskVsInfoToCompare)
                {
                    Dictionary<string, TaskInfo> tasks = null;
                    if (existingChainNameVsTaskVsInfo.TryGetValue(chainLevel.Key, out tasks))
                    {
                        if (chainLevel.Value.Count == tasks.Count)
                        {
                            var hasBreak = false;
                            foreach (var taskLevel in chainLevel.Value)
                            {
                                if (!tasks.ContainsKey(taskLevel.Key))
                                    hasBreak = true;
                            }
                            if (!hasBreak)
                            {
                                foreach (var taskLevel in chainLevel.Value)
                                {
                                    TaskInfo tInfo = null;
                                    if (tasks.TryGetValue(taskLevel.Key, out tInfo) && (tInfo.Dependencies == null || tInfo.Dependencies.Count == 0))
                                        tInfo.Dependencies = taskLevel.Value.Dependencies;
                                }
                            }
                        }
                    }
                }

                foreach (var chainId in existingChainVsTaskOrder.Keys.ToArray())
                {
                    if (!existingChainsInSheet.Contains(chainId))
                    {
                        if (existingChainVsTaskOrder.ContainsKey(chainId))
                            existingChainVsTaskOrder.Remove(chainId);
                        if (existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                            existingChainNameVsTaskVsInfo.Remove(chainId);

                        if (existingChainVsSubscription.ContainsKey(chainId))
                            existingChainVsSubscription.Remove(chainId);
                        if (existingchainNameVsInfo.ContainsKey(chainId))
                            existingchainNameVsInfo.Remove(chainId);
                        if (existingChainVsTaskVsSubscription.ContainsKey(chainId))
                            existingChainVsTaskVsSubscription.Remove(chainId);
                    }
                }

                List<string> chainsToDelete = new List<string>();
                foreach (var chainLevel in existingChainNameVsTaskVsInfo)
                {
                    if (chainLevel.Value.Where(x => x.Value.Dependencies == null || x.Value.Dependencies.Count == 0).Count() > 1)
                        chainsToDelete.Add(chainLevel.Key);
                }
                foreach (var chainId in chainsToDelete)
                {
                    if (existingChainVsTaskOrder.ContainsKey(chainId))
                        existingChainVsTaskOrder.Remove(chainId);
                    if (existingChainNameVsTaskVsInfo.ContainsKey(chainId))
                        existingChainNameVsTaskVsInfo.Remove(chainId);
                    if (existingchainNameVsInfo.ContainsKey(chainId))
                        existingchainNameVsInfo.Remove(chainId);
                    if (existingChainVsSubscription.ContainsKey(chainId))
                        existingChainVsSubscription.Remove(chainId);
                    if (existingChainVsTaskVsSubscription.ContainsKey(chainId))
                        existingChainVsTaskVsSubscription.Remove(chainId);
                }

                chainsToDelete = new List<string>();
                foreach (var chainLevel in newChainNameVsTaskVsInfo)
                {
                    if (chainLevel.Value.Where(x => x.Value.Dependencies == null || x.Value.Dependencies.Count == 0).Count() > 1)
                        chainsToDelete.Add(chainLevel.Key);
                }
                foreach (var chainId in chainsToDelete)
                {
                    if (newChainVsTaskOrder.ContainsKey(chainId))
                        newChainVsTaskOrder.Remove(chainId);
                    if (newChainNameVsTaskVsInfo.ContainsKey(chainId))
                        newChainNameVsTaskVsInfo.Remove(chainId);
                    if (newChainNameVsInfo.ContainsKey(chainId))
                        newChainNameVsInfo.Remove(chainId);
                    if (newChainVsSubscription.ContainsKey(chainId))
                        newChainVsSubscription.Remove(chainId);
                    if (newChainVsTaskVsSubscription.ContainsKey(chainId))
                        newChainVsTaskVsSubscription.Remove(chainId);
                }

                #region CHAIN TABLE
                ObjectTable ChainTable = new ObjectTable();
                ChainTable.Columns.Add("ChainId", typeof(int));
                ChainTable.Columns.Add("ChainName", typeof(string));
                ChainTable.Columns.Add("CalendarName", typeof(string));
                ChainTable.Columns.Add("RecurrenceType", typeof(string));
                ChainTable.Columns.Add("StartDate", typeof(DateTime));
                ChainTable.Columns.Add("RecurrencePattern", typeof(string));
                ChainTable.Columns.Add("Interval", typeof(int));
                ChainTable.Columns.Add("EndDate", typeof(DateTime));
                ChainTable.Columns.Add("NumberOfRecurrences", typeof(int));
                ChainTable.Columns.Add("StartTime", typeof(DateTime));
                ChainTable.Columns.Add("TimeIntervalOfRecurrence", typeof(int));
                ChainTable.Columns.Add("NeverEndJob", typeof(bool));
                ChainTable.Columns.Add("DaysOfWeek", typeof(int));

                foreach (var chainLevel in existingchainNameVsInfo)
                {
                    var ndr = ChainTable.NewRow();
                    ndr["ChainId"] = chainLevel.Key;
                    ndr["ChainName"] = chainLevel.Value.ChainName;
                    ndr["CalendarName"] = chainLevel.Value.CalendarName;
                    ndr["RecurrenceType"] = chainLevel.Value.RecurrenceType;

                    if (chainLevel.Value.StartDate != default(DateTime))
                        ndr["StartDate"] = chainLevel.Value.StartDate;

                    ndr["RecurrencePattern"] = chainLevel.Value.RecurrencePattern;
                    ndr["Interval"] = chainLevel.Value.Interval;

                    if (chainLevel.Value.EndDate != default(DateTime))
                        ndr["EndDate"] = chainLevel.Value.EndDate;

                    ndr["NumberOfRecurrences"] = chainLevel.Value.NumberOfRecurrences;

                    if (chainLevel.Value.StartTime != default(TimeSpan) && chainLevel.Value.StartDate != default(DateTime))
                        ndr["StartTime"] = new DateTime(chainLevel.Value.StartDate.Year, chainLevel.Value.StartDate.Month, chainLevel.Value.StartDate.Day, chainLevel.Value.StartTime.Hours, chainLevel.Value.StartTime.Minutes, chainLevel.Value.StartTime.Seconds);

                    ndr["TimeIntervalOfRecurrence"] = chainLevel.Value.TimeIntervalOfRecurrence;
                    ndr["NeverEndJob"] = chainLevel.Value.NeverEndJob;
                    ndr["DaysOfWeek"] = chainLevel.Value.DaysOfWeek;

                    ChainTable.Rows.Add(ndr);
                }
                foreach (var chainLevel in newChainNameVsInfo)
                {
                    var ndr = ChainTable.NewRow();
                    ndr["ChainName"] = chainLevel.Key;
                    ndr["CalendarName"] = chainLevel.Value.CalendarName;
                    ndr["RecurrenceType"] = chainLevel.Value.RecurrenceType;

                    if (chainLevel.Value.StartDate != default(DateTime))
                        ndr["StartDate"] = chainLevel.Value.StartDate;

                    ndr["RecurrencePattern"] = chainLevel.Value.RecurrencePattern;
                    ndr["Interval"] = chainLevel.Value.Interval;

                    if (chainLevel.Value.EndDate != default(DateTime))
                        ndr["EndDate"] = chainLevel.Value.EndDate;

                    ndr["NumberOfRecurrences"] = chainLevel.Value.NumberOfRecurrences;

                    if (chainLevel.Value.StartTime != default(TimeSpan) && chainLevel.Value.StartDate != default(DateTime))
                        ndr["StartTime"] = new DateTime(chainLevel.Value.StartDate.Year, chainLevel.Value.StartDate.Month, chainLevel.Value.StartDate.Day, chainLevel.Value.StartTime.Hours, chainLevel.Value.StartTime.Minutes, chainLevel.Value.StartTime.Seconds);

                    ndr["TimeIntervalOfRecurrence"] = chainLevel.Value.TimeIntervalOfRecurrence;
                    ndr["NeverEndJob"] = chainLevel.Value.NeverEndJob;
                    ndr["DaysOfWeek"] = chainLevel.Value.DaysOfWeek;

                    ChainTable.Rows.Add(ndr);
                }
                #endregion

                #region TASK TABLE
                ObjectTable taskTable = new ObjectTable();
                taskTable.Columns.Add("ChainId", typeof(int));
                taskTable.Columns.Add("flow_id", typeof(int));
                taskTable.Columns.Add("ChainName", typeof(string));
                taskTable.Columns.Add("TaskName", typeof(string));
                taskTable.Columns.Add("TaskType", typeof(string));
                taskTable.Columns.Add("ModuleName", typeof(string));
                taskTable.Columns.Add("IsMuted", typeof(bool));
                taskTable.Columns.Add("ProceedOnFail", typeof(bool));
                taskTable.Columns.Add("ReRunOnFail", typeof(bool));
                taskTable.Columns.Add("RetryDuration", typeof(int));
                taskTable.Columns.Add("NumberOfRetries", typeof(int));
                taskTable.Columns.Add("OnFailRunTask", typeof(string));
                taskTable.Columns.Add("TimeOut", typeof(int));
                taskTable.Columns.Add("TaskInstanceWait", typeof(int));
                taskTable.Columns.Add("TaskWaitSubscription", typeof(string));
                taskTable.Columns.Add("Dependencies", typeof(string));
                foreach (var chainLevelOrder in existingChainVsTaskOrder)
                {
                    var chainLevel = existingChainNameVsTaskVsInfo[chainLevelOrder.Key];
                    foreach (var taskLevelOrder in chainLevelOrder.Value)
                    {
                        var taskLevel = chainLevel[taskLevelOrder];
                        var ndr = taskTable.NewRow();
                        ndr["ChainId"] = chainLevelOrder.Key;
                        ndr["ChainName"] = taskLevel.ChainName;
                        ndr["TaskName"] = taskLevel.TaskName;
                        ndr["TaskType"] = taskLevel.TaskType;
                        ndr["ModuleName"] = taskLevel.ModuleName;
                        ndr["IsMuted"] = taskLevel.IsMuted;
                        ndr["ProceedOnFail"] = taskLevel.ProceedOnFail;
                        ndr["ReRunOnFail"] = taskLevel.ReRunOnFail;
                        ndr["RetryDuration"] = taskLevel.RetryDuration;
                        ndr["NumberOfRetries"] = taskLevel.NumberOfRetries;
                        ndr["TimeOut"] = taskLevel.TimeOut;
                        ndr["TaskInstanceWait"] = taskLevel.TaskInstanceWait;
                        ndr["TaskWaitSubscription"] = taskLevel.TaskWaitSubscription;

                        if (taskLevel.Dependencies != null && taskLevel.Dependencies.Count > 0)
                            ndr["Dependencies"] = string.Join("ž", taskLevel.Dependencies.Select(y => dependencyRelations[y.Relation] + "Ÿ" + y.TaskName + "œ" + y.TaskType + "š" + y.TaskModuleName));

                        if (taskLevel.OnFailRunTask != null)
                            ndr["OnFailRunTask"] = taskLevel.OnFailRunTask.TaskName + "œ" + taskLevel.OnFailRunTask.TaskType + "š" + taskLevel.OnFailRunTask.TaskModuleName;
                        else
                            ndr["OnFailRunTask"] = DBNull.Value;

                        taskTable.Rows.Add(ndr);
                    }
                }
                foreach (var chainLevelOrder in newChainVsTaskOrder)
                {
                    var chainLevel = newChainNameVsTaskVsInfo[chainLevelOrder.Key];
                    foreach (var taskLevelOrder in chainLevelOrder.Value)
                    {
                        var taskLevel = chainLevel[taskLevelOrder];
                        var ndr = taskTable.NewRow();
                        ndr["ChainName"] = chainLevelOrder.Key;
                        ndr["TaskName"] = taskLevel.TaskName;
                        ndr["TaskType"] = taskLevel.TaskType;
                        ndr["ModuleName"] = taskLevel.ModuleName;
                        ndr["IsMuted"] = taskLevel.IsMuted;
                        ndr["ProceedOnFail"] = taskLevel.ProceedOnFail;
                        ndr["ReRunOnFail"] = taskLevel.ReRunOnFail;
                        ndr["RetryDuration"] = taskLevel.RetryDuration;
                        ndr["NumberOfRetries"] = taskLevel.NumberOfRetries;
                        ndr["TimeOut"] = taskLevel.TimeOut;
                        ndr["TaskInstanceWait"] = taskLevel.TaskInstanceWait;
                        ndr["TaskWaitSubscription"] = taskLevel.TaskWaitSubscription;

                        if (taskLevel.Dependencies != null && taskLevel.Dependencies.Count > 0)
                            ndr["Dependencies"] = string.Join("ž", taskLevel.Dependencies.Select(y => dependencyRelations[y.Relation] + "Ÿ" + y.TaskName + "œ" + y.TaskType + "š" + y.TaskModuleName));

                        if (taskLevel.OnFailRunTask != null)
                            ndr["OnFailRunTask"] = taskLevel.OnFailRunTask.TaskName + "œ" + taskLevel.OnFailRunTask.TaskType + "š" + taskLevel.OnFailRunTask.TaskModuleName;
                        else
                            ndr["OnFailRunTask"] = DBNull.Value;

                        taskTable.Rows.Add(ndr);
                    }
                }
                #endregion

                #region CHAIN SUBSCRIPTION TABLE
                ObjectTable chainSubTable = new ObjectTable();
                chainSubTable.Columns.Add("ChainId", typeof(int));
                chainSubTable.Columns.Add("ChainName", typeof(string));
                chainSubTable.Columns.Add("Subscription", typeof(string));

                foreach (var row in existingChainVsSubscription)
                {
                    var ndr = chainSubTable.NewRow();
                    ndr["ChainId"] = row.Key;

                    string[] arr = new string[6];
                    if (row.Value.ContainsKey("SUCCESS"))
                    {
                        var subInfo = row.Value["SUCCESS"];
                        arr[0] = subInfo.To;
                        arr[2] = subInfo.Subject;
                        arr[3] = subInfo.body;
                    }
                    if (row.Value.ContainsKey("FAILURE"))
                    {
                        var subInfo = row.Value["FAILURE"];
                        arr[1] = subInfo.To;
                        arr[4] = subInfo.Subject;
                        arr[5] = subInfo.body;
                    }

                    ndr["Subscription"] = string.Join("|", arr);

                    chainSubTable.Rows.Add(ndr);
                }
                foreach (var row in newChainVsSubscription)
                {
                    var ndr = chainSubTable.NewRow();
                    ndr["ChainName"] = row.Key;
                    string[] arr = new string[6];
                    if (row.Value.ContainsKey("SUCCESS"))
                    {
                        var subInfo = row.Value["SUCCESS"];
                        arr[0] = subInfo.To;
                        arr[2] = subInfo.Subject;
                        arr[3] = subInfo.body;
                    }
                    if (row.Value.ContainsKey("FAILURE"))
                    {
                        var subInfo = row.Value["FAILURE"];
                        arr[1] = subInfo.To;
                        arr[4] = subInfo.Subject;
                        arr[5] = subInfo.body;
                    }

                    ndr["Subscription"] = string.Join("|", arr);

                    chainSubTable.Rows.Add(ndr);
                }
                #endregion

                #region TASK SUBSCRIPTION TABLE
                ObjectTable taskSubTable = new ObjectTable();
                taskSubTable.Columns.Add("ChainId", typeof(int));
                taskSubTable.Columns.Add("ChainName", typeof(string));
                taskSubTable.Columns.Add("TaskName", typeof(string));
                taskSubTable.Columns.Add("TaskType", typeof(string));
                taskSubTable.Columns.Add("ModuleName", typeof(string));
                taskSubTable.Columns.Add("Subscription", typeof(string));

                foreach (var row in existingChainVsTaskVsSubscription)
                {
                    foreach (var roww in row.Value)
                    {
                        var ndr = taskSubTable.NewRow();
                        ndr["ChainId"] = row.Key;

                        string[] arr = new string[6];
                        if (roww.Value.ContainsKey("SUCCESS"))
                        {
                            var subInfo = roww.Value["SUCCESS"];
                            arr[0] = subInfo.To;
                            arr[2] = subInfo.Subject;
                            arr[3] = subInfo.body;
                            ndr["TaskName"] = subInfo.TaskName;
                            ndr["TaskType"] = subInfo.TaskType;
                            ndr["ModuleName"] = subInfo.ModuleName;
                        }
                        if (roww.Value.ContainsKey("FAILURE"))
                        {
                            var subInfo = roww.Value["FAILURE"];
                            arr[1] = subInfo.To;
                            arr[4] = subInfo.Subject;
                            arr[5] = subInfo.body;
                            ndr["TaskName"] = subInfo.TaskName;
                            ndr["TaskType"] = subInfo.TaskType;
                            ndr["ModuleName"] = subInfo.ModuleName;
                        }

                        ndr["Subscription"] = string.Join("|", arr);

                        taskSubTable.Rows.Add(ndr);
                    }
                }
                foreach (var row in newChainVsTaskVsSubscription)
                {
                    foreach (var roww in row.Value)
                    {
                        var ndr = taskSubTable.NewRow();
                        ndr["ChainName"] = row.Key;

                        string[] arr = new string[6];
                        if (roww.Value.ContainsKey("SUCCESS"))
                        {
                            var subInfo = roww.Value["SUCCESS"];
                            arr[0] = subInfo.To;
                            arr[2] = subInfo.Subject;
                            arr[3] = subInfo.body;
                            ndr["TaskName"] = subInfo.TaskName;
                            ndr["TaskType"] = subInfo.TaskType;
                            ndr["ModuleName"] = subInfo.ModuleName;
                        }
                        if (roww.Value.ContainsKey("FAILURE"))
                        {
                            var subInfo = roww.Value["FAILURE"];
                            arr[1] = subInfo.To;
                            arr[4] = subInfo.Subject;
                            arr[5] = subInfo.body;
                            ndr["TaskName"] = subInfo.TaskName;
                            ndr["TaskType"] = subInfo.TaskType;
                            ndr["ModuleName"] = subInfo.ModuleName;
                        }

                        ndr["Subscription"] = string.Join("|", arr);

                        taskSubTable.Rows.Add(ndr);
                    }
                }
                #endregion

                #region SYNC

                if (ChainTable.Rows.Count > 0)
                {
                    RDBConnectionManager con = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                    con.UseTransaction = true;
                    con.IsolationLevel = System.Data.IsolationLevel.RepeatableRead;

                    var masterId = 0;
                    var persists = false;
                    var guid = Guid.NewGuid().ToString();
                    var chainTableName = "IVPSRMTaskManager.dbo.[chainTable_" + guid + "]";
                    var taskTableName = "IVPSRMTaskManager.dbo.[taskTable_" + guid + "]";
                    var chainSubscriptionTableName = "IVPSRMTaskManager.dbo.[chainSubscription_" + guid + "]";
                    var taskSubscriptionTableName = "IVPSRMTaskManager.dbo.[taskSubscription_" + guid + "]";
                    var dependenciesTableName = "IVPSRMTaskManager.dbo.[dependencies_" + guid + "]";
                    var onFailTableName = "IVPSRMTaskManager.dbo.[onFail_" + guid + "]";

                    try
                    {
                        var row = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @master_id INT,
                                    @date DATETIME2,
                                    @persists BIT

                            IF EXISTS(SELECT 1 FROM sys.databases WHERE name = 'IVPSecMaster')
                            BEGIN
	                            SELECT @master_id = master_id, @persists = persist_tables FROM IVPSecmaster.dbo.ivp_secm_temp_table_config_master
	                            SELECT @date = GETDATE()

	                            IF(ISNULL(@master_id, 0) > 0)
	                                INSERT INTO IVPSecmaster.dbo.ivp_secm_temp_table_config_details VALUES(@master_id, '{0}', @date), (@master_id, '{1}', @date), (@master_id, '{2}', @date), (@master_id, '{3}', @date)
                            END

                            SELECT ISNULL(@master_id, 0) AS [master_id], CAST(ISNULL(@persists,0) AS BIT) AS persists", chainTableName, taskTableName, chainSubscriptionTableName, taskSubscriptionTableName), ConnectionConstants.RefMaster_Connection).Tables[0].Rows[0];
                        masterId = Convert.ToInt32(row["master_id"]);
                        persists = Convert.ToBoolean(row["persists"]);

                        CommonDALWrapper.ExecuteQuery(string.Format(@"CREATE TABLE {0}(ChainId INT, ChainName VARCHAR(MAX), CalendarName VARCHAR(MAX), RecurrenceType VARCHAR(MAX), StartDate DATETIME, RecurrencePattern VARCHAR(MAX), Interval INT, EndDate DATETIME, NumberOfRecurrences INT, StartTime DATETIME, TimeIntervalOfRecurrence INT, NeverEndJob BIT, DaysOfWeek INT)
	                CREATE TABLE {1}(id INT IDENTITY(1,1), ChainId INT, flow_id INT, ChainName VARCHAR(MAX), TaskName VARCHAR(MAX), TaskType VARCHAR(MAX), ModuleName VARCHAR(MAX), IsMuted BIT, ProceedOnFail BIT, ReRunOnFail BIT, RetryDuration INT, NumberOfRetries INT, TimeOut INT, TaskInstanceWait INT, TaskWaitSubscription VARCHAR(MAX), Dependencies VARCHAR(MAX), OnFailRunTask VARCHAR(MAX))
	                CREATE TABLE {2}(ChainId INT, ChainName VARCHAR(MAX), Subscription VARCHAR(MAX))
	                CREATE TABLE {3}(ChainId INT, ChainName VARCHAR(MAX), TaskName VARCHAR(MAX), TaskType VARCHAR(MAX), ModuleName VARCHAR(MAX), Subscription VARCHAR(MAX))
                    CREATE TABLE {4}(ChainId INT, FlowId INT, Dependencies VARCHAR(MAX))
                    CREATE TABLE {5}(ChainId INT, FlowId INT, onFail INT)", chainTableName, taskTableName, chainSubscriptionTableName, taskSubscriptionTableName, dependenciesTableName, onFailTableName), CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);

                        CommonDALWrapper.ExecuteBulkUploadObject(chainTableName, ChainTable, ConnectionConstants.RefMaster_Connection);
                        CommonDALWrapper.ExecuteBulkUploadObject(taskTableName, taskTable, ConnectionConstants.RefMaster_Connection);
                        CommonDALWrapper.ExecuteBulkUploadObject(chainSubscriptionTableName, chainSubTable, ConnectionConstants.RefMaster_Connection);
                        CommonDALWrapper.ExecuteBulkUploadObject(taskSubscriptionTableName, taskSubTable, ConnectionConstants.RefMaster_Connection);

                        var deletedTasksFromChain = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSRMTaskManager.dbo.SRM_SyncTaskManager '{0}','{1}','{2}','{3}','{4}'", chainTableName, taskTableName, chainSubscriptionTableName, taskSubscriptionTableName, userName), con).Tables[0];

                        Dictionary<int, List<int>> moduleIdVsDeletedTasks = new Dictionary<int, List<int>>();
                        foreach (DataRow dr in deletedTasksFromChain.Rows)
                        {
                            var moduleId = Convert.ToInt32(dr["module_id"]);
                            List<int> tasks = null;
                            if (!moduleIdVsDeletedTasks.TryGetValue(moduleId, out tasks))
                            {
                                tasks = new List<int>();
                                moduleIdVsDeletedTasks[moduleId] = tasks;
                            }
                            tasks.Add(Convert.ToInt32(dr["task_master_id"]));
                        }


                        Dictionary<string, KeyValuePair<int, int>> taskKeyVsModuleId = new Dictionary<string, KeyValuePair<int, int>>(StringComparer.OrdinalIgnoreCase);
                        var dtSum = CommonDALWrapper.ExecuteSelectQueryObject("SELECT task_name, task_type_name, module_id, module_name, task_master_id, task_summary_id FROM IVPSRMTaskManager.dbo.ivp_rad_task_summary ORDER BY task_summary_id DESC", con).Tables[0];

                        foreach (var x in dtSum.AsEnumerable())
                        {
                            var key = Convert.ToString(x["task_name"]) + "ž" + Convert.ToString(x["task_type_name"]) + "ž" + Convert.ToString(x["module_name"]);

                            if (!taskKeyVsModuleId.ContainsKey(key))
                                taskKeyVsModuleId.Add(key, new KeyValuePair<int, int>(Convert.ToInt32(x["module_id"]), Convert.ToInt32(x["task_master_id"])));
                        }

                        var chainAndTaskInfo = CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT ct.chain_name, ct.chain_id, rf.flow_id, rf.task_name, rf.task_type_name, rf.module_name
                        FROM IVPSRMTaskManager.dbo.ivp_rad_chained_tasks ct
                        INNER JOIN IVPSRMTaskManager.dbo.ivp_rad_flow rf ON ct.chain_id = rf.chain_id
                        INNER JOIN IVPSRMTaskManager.dbo.ivp_rad_task_summary ts ON rf.task_summary_id = ts.task_summary_id
                        WHERE ct.is_active = 1 AND rf.is_active = 1 AND is_visible_on_ctm = 1", con).Tables[0];

                        var ChainIdVsTaskKeyVsFlowId = chainAndTaskInfo.AsEnumerable().GroupBy(x => Convert.ToString(x["chain_id"])).ToDictionary(y => y.Key, z => z.GroupBy(a => Convert.ToString(a["task_name"]) + "ž" + Convert.ToString(a["task_type_name"]) + "ž" + Convert.ToString(a["module_name"])).ToDictionary(b => b.Key, c => c.Select(d => Convert.ToString(d["flow_id"])).First(), StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
                        var ChainNameVsId = chainAndTaskInfo.AsEnumerable().GroupBy(x => Convert.ToString(x["chain_name"])).ToDictionary(y => y.Key, z => z.Select(a => Convert.ToString(a["chain_id"])).First(), StringComparer.OrdinalIgnoreCase);

                        HashSet<int> SecmTasks = new HashSet<int>();
                        HashSet<int> RefmTasks = new HashSet<int>();

                        ObjectTable dependencies = new ObjectTable();
                        dependencies.Columns.Add("ChainId", typeof(int));
                        dependencies.Columns.Add("FlowId", typeof(int));
                        dependencies.Columns.Add("Dependencies", typeof(string));

                        ObjectTable onFail = new ObjectTable();
                        onFail.Columns.Add("ChainId", typeof(int));
                        onFail.Columns.Add("FlowId", typeof(int));
                        onFail.Columns.Add("onFail", typeof(string));

                        Dictionary<string, string> taskVsErrorMessageForOnFailRunTask = new Dictionary<string, string>();
                        Dictionary<string, string> taskVsErrorMessageForDependents = new Dictionary<string, string>();

                        foreach (var row2 in taskTable.AsEnumerable())
                        {
                            var key = Convert.ToString(row2["TaskName"]) + "ž" + Convert.ToString(row2["TaskType"]) + "ž" + Convert.ToString(row2["ModuleName"]);
                            var depends = Convert.ToString(row2["Dependencies"]);
                            var chainId = Convert.ToString(row2["ChainId"]);
                            var chainName = Convert.ToString(row2["ChainName"]);
                            var onFailRunTask = Convert.ToString(row2["OnFailRunTask"]);
                            var errorKey = chainName + "ž" + key;

                            if (!string.IsNullOrWhiteSpace(depends))
                            {
                                int cid = -1;
                                var ndr = dependencies.NewRow();
                                if (!string.IsNullOrEmpty(chainId))
                                    Int32.TryParse(chainId, out cid);
                                else if (!string.IsNullOrWhiteSpace(chainName))
                                {
                                    if (ChainNameVsId.ContainsKey(chainName))
                                        cid = Convert.ToInt32(ChainNameVsId[chainName]);
                                }
                                if (cid > 0)
                                {
                                    ndr["ChainId"] = cid;

                                    Dictionary<string, string> taskKeyVsFlowId = null;
                                    if (ChainIdVsTaskKeyVsFlowId.TryGetValue(cid.ToString(), out taskKeyVsFlowId))
                                    {
                                        string flowId = null;
                                        if (taskKeyVsFlowId.TryGetValue(key, out flowId))
                                            ndr["FlowId"] = Convert.ToInt32(flowId);

                                        var deps = depends.Split(new string[] { "ž" }, StringSplitOptions.RemoveEmptyEntries);
                                        if (deps.Length > 0)
                                        {
                                            StringBuilder sb = new StringBuilder();
                                            foreach (var de in deps)
                                            {
                                                var dtasks = de.Split(new string[] { "Ÿ", "œ", "š" }, StringSplitOptions.RemoveEmptyEntries);
                                                if (dtasks.Length == 4)
                                                {
                                                    var tk = dtasks[1] + "ž" + dtasks[2] + "ž" + dtasks[3];
                                                    flowId = null;
                                                    if (taskKeyVsFlowId.TryGetValue(tk, out flowId))
                                                        sb.Append(dtasks[0]).Append(flowId).Append(",");
                                                    else
                                                    {
                                                        string message = null;
                                                        taskVsErrorMessageForDependents.TryGetValue(errorKey, out message);
                                                        if (message == null)
                                                            message = string.Empty;
                                                        taskVsErrorMessageForDependents[errorKey] = message + " " + "Invalid Dependent On Task Info";
                                                    }
                                                }
                                            }
                                            if (sb.Length > 0)
                                            {
                                                ndr["Dependencies"] = sb.ToString();
                                                dependencies.Rows.Add(ndr);
                                            }
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(onFailRunTask))
                            {
                                int cid = -1;
                                var ndr = onFail.NewRow();
                                if (!string.IsNullOrEmpty(chainId))
                                    Int32.TryParse(chainId, out cid);
                                else if (!string.IsNullOrWhiteSpace(chainName))
                                {
                                    if (ChainNameVsId.ContainsKey(chainName))
                                        cid = Convert.ToInt32(ChainNameVsId[chainName]);
                                }
                                if (cid > 0)
                                {
                                    ndr["ChainId"] = cid;

                                    Dictionary<string, string> taskKeyVsFlowId = null;
                                    if (ChainIdVsTaskKeyVsFlowId.TryGetValue(cid.ToString(), out taskKeyVsFlowId))
                                    {
                                        string flowId = null;
                                        if (taskKeyVsFlowId.TryGetValue(key, out flowId))
                                            ndr["FlowId"] = Convert.ToInt32(flowId);

                                        StringBuilder sb = new StringBuilder();
                                        var dtasks = onFailRunTask.Split(new string[] { "œ", "š" }, StringSplitOptions.RemoveEmptyEntries);
                                        if (dtasks.Length == 3)
                                        {
                                            var tk = dtasks[0] + "ž" + dtasks[1] + "ž" + dtasks[2];
                                            string flowId2 = null;
                                            if (taskKeyVsFlowId.TryGetValue(tk, out flowId2))
                                            {
                                                if (Convert.ToInt32(flowId2) > Convert.ToInt32(flowId))
                                                {
                                                    ndr["onFail"] = Convert.ToInt32(flowId2);
                                                    onFail.Rows.Add(ndr);
                                                }
                                                else
                                                {
                                                    string message = null;
                                                    taskVsErrorMessageForOnFailRunTask.TryGetValue(errorKey, out message);
                                                    if (message == null)
                                                        message = string.Empty;
                                                    taskVsErrorMessageForOnFailRunTask[errorKey] = message + " " + "On Fail Run Task should be a task executed after this task.";
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            KeyValuePair<int, int> moduleIdVsTaskSummaryId;
                            if (taskKeyVsModuleId.TryGetValue(key, out moduleIdVsTaskSummaryId))
                            {
                                if (moduleIdVsTaskSummaryId.Key == 3)
                                    SecmTasks.Add(moduleIdVsTaskSummaryId.Value);
                                else if (moduleIdVsTaskSummaryId.Key == 6)
                                    RefmTasks.Add(moduleIdVsTaskSummaryId.Value);
                            }
                            else
                            {
                                string message = null;
                                taskVsErrorMessageForOnFailRunTask.TryGetValue(errorKey, out message);
                                if (message == null)
                                    message = string.Empty;
                                taskVsErrorMessageForOnFailRunTask[errorKey] = message + " " + "Invalid Task - " + Convert.ToString(row2["TaskName"]);
                            }
                        }

                        var hasErrors = false;
                        if (taskVsErrorMessageForOnFailRunTask.Count > 0)
                        {
                            foreach (var row2 in excelInfo.Tables[SRM_TaskManager_SheetNames.Task_Information].AsEnumerable())
                            {
                                var chainName = Convert.ToString(row2[SM_ColumnNames.Chain_Name]);
                                var taskName = Convert.ToString(row2[SM_ColumnNames.Task_Name]);
                                var taskType = Convert.ToString(row2[SM_ColumnNames.Task_Type]);
                                var moduleName = Convert.ToString(row2[SM_ColumnNames.Module_Name]);
                                var key = chainName + "ž" + taskName + "ž" + taskType + "ž" + moduleName;

                                string message = null;
                                if (taskVsErrorMessageForOnFailRunTask.TryGetValue(key, out message))
                                {
                                    hasErrors = true;
                                    row2[SRMSpecialColumnNames.Remarks] = Convert.ToString(row2[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + message;
                                }
                            }
                        }
                        if (taskVsErrorMessageForDependents.Count > 0)
                        {
                            foreach (var row2 in excelInfo.Tables[SRM_TaskManager_SheetNames.Task_Dependency].AsEnumerable())
                            {
                                var chainName = Convert.ToString(row2[SM_ColumnNames.Chain_Name]);
                                var taskName = Convert.ToString(row2[SM_ColumnNames.Task_Name]);
                                var taskType = Convert.ToString(row2[SM_ColumnNames.Task_Type]);
                                var moduleName = Convert.ToString(row2[SM_ColumnNames.Module_Name]);
                                var key = chainName + "ž" + taskName + "ž" + taskType + "ž" + moduleName;

                                string message = null;
                                if (taskVsErrorMessageForDependents.TryGetValue(key, out message))
                                {
                                    hasErrors = true;
                                    row2[SRMSpecialColumnNames.Remarks] = Convert.ToString(row2[SRMSpecialColumnNames.Remarks]) + SRMMigrationSeparators.Remarks_Separator + message;
                                }
                            }
                        }

                        if (hasErrors)
                            throw new Exception("Dummy");

                        if (dependencies.Rows.Count > 0)
                        {
                            CommonDALWrapper.ExecuteBulkUploadObject(dependenciesTableName, dependencies, con);
                            CommonDALWrapper.ExecuteQuery(string.Format(@"UPDATE rf
                                SET rf.dependent_on_id = t.Dependencies
                                FROM IVPSRMTaskManager.dbo.ivp_rad_flow rf
                                INNER JOIN {0} t ON rf.chain_id = t.ChainId AND rf.flow_id = t.FlowId
                                WHERE is_active = 1", dependenciesTableName), CommonQueryType.Update, con);
                        }

                        if (onFail.Rows.Count > 0)
                        {
                            CommonDALWrapper.ExecuteBulkUploadObject(onFailTableName, onFail, con);
                            CommonDALWrapper.ExecuteQuery(string.Format(@"UPDATE rf
                                SET rf.on_fail_run_task = t.onFail
                                FROM IVPSRMTaskManager.dbo.ivp_rad_flow rf
                                INNER JOIN {0} t ON rf.chain_id = t.ChainId AND rf.flow_id = t.FlowId
                                WHERE is_active = 1", onFailTableName), CommonQueryType.Update, con);
                        }

                        if (SecmTasks.Count > 0)
                        {
                            CommonDALWrapper.ExecuteQuery(string.Format(@"DECLARE @tasks TABLE (task_master_id INT)
                                DECLARE @task_ids VARCHAR(MAX) = '{0}'

                                INSERT INTO @tasks(task_master_id)
                                SELECT item
                                FROM IVPRefMaster.dbo.REFM_GetList2Table(@task_ids, ',')

                                DELETE ts
                                FROM IVPSecmasterVendor.dbo.ivp_secmv_flow ts
                                INNER JOIN @tasks t ON ts.task_master_id = t.task_master_id

                                DELETE ts
                                FROM IVPSecmasterVendor.dbo.ivp_secmv_flow ts
                                INNER JOIN IVPRefMaster.dbo.REFM_GetList2Table('{1}', ',') t ON ts.task_master_id = CAST(t.item AS INT)

                                INSERT INTO IVPSecmasterVendor.dbo.ivp_secmv_flow(specified_id, task_master_id, trigger_type, dependent_pretask, created_by, created_on, last_modified_by, last_modified_on)
                                SELECT - 1, task_master_id, 0, -1, 'admin', GETDATE(), 'admin', GETDATE()
                                FROM @tasks", string.Join(",", SecmTasks), moduleIdVsDeletedTasks.ContainsKey(3) ? string.Join(",", moduleIdVsDeletedTasks[3]) : string.Empty), CommonQueryType.Insert, con);
                        }

                        if (RefmTasks.Count > 0)
                        {
                            CommonDALWrapper.ExecuteQuery(string.Format(@"DECLARE @tasks TABLE (task_master_id INT)
                                DECLARE @task_ids VARCHAR(MAX) = '{0}'

                                INSERT INTO @tasks(task_master_id)
                                SELECT item
                                FROM IVPRefMaster.dbo.REFM_GetList2Table(@task_ids, ',')

                                DELETE ts
                                FROM IVPRefmasterVendor.dbo.ivp_refm_flow ts
                                INNER JOIN @tasks t ON ts.task_master_id = t.task_master_id

                                DELETE ts
                                FROM IVPRefmasterVendor.dbo.ivp_refm_flow ts
                                INNER JOIN IVPRefMaster.dbo.REFM_GetList2Table('{1}', ',') t ON ts.task_master_id = CAST(t.item AS INT)

                                INSERT INTO IVPRefmasterVendor.dbo.ivp_refm_flow (task_master_id,trigger_type,dependent_pretask,created_by,created_on,last_modified_by,last_modified_on,is_active)    
                                SELECT task_master_id,0,-1,'admin',GETDATE(),'admin',GETDATE(),1  
                                FROM  @tasks", string.Join(",", RefmTasks), moduleIdVsDeletedTasks.ContainsKey(6) ? string.Join(",", moduleIdVsDeletedTasks[6]) : string.Empty), CommonQueryType.Insert, con);
                        }

                        con.CommitTransaction();

                        foreach (var table in excelInfo.Tables)
                        {
                            foreach (var roww in table.Rows)
                            {
                                if (string.IsNullOrWhiteSpace(Convert.ToString(roww[SRMSpecialColumnNames.Sync_Status])))
                                {
                                    roww[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Passed;
                                }
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        mLogger.Error("ValidateandSaveTaskManager -----> SYNC ERROR : " + ee.ToString());
                        var message = ee.Message;
                        con.RollbackTransaction();

                        if (message != "Dummy")
                        {
                            foreach (var table in excelInfo.Tables)
                            {
                                foreach (var row in table.Rows)
                                {
                                    if (string.IsNullOrWhiteSpace(Convert.ToString(row[SRMSpecialColumnNames.Sync_Status])))
                                    {
                                        row[SRMSpecialColumnNames.Remarks] = message;
                                        row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (!persists && masterId > 0)
                        {
                            CommonDALWrapper.ExecuteQuery(string.Format(@"
                            IF (OBJECT_ID('{0}') IS NOT NULL) DROP TABLE {0};
                            IF (OBJECT_ID('{1}') IS NOT NULL) DROP TABLE {1};
                            IF (OBJECT_ID('{2}') IS NOT NULL) DROP TABLE {2};
                            IF (OBJECT_ID('{3}') IS NOT NULL) DROP TABLE {3};
                            IF (OBJECT_ID('{5}') IS NOT NULL) DROP TABLE {5};
                            IF (OBJECT_ID('{6}') IS NOT NULL) DROP TABLE {6};

                            DELETE FROM IVPSecmaster.dbo.ivp_secm_temp_table_config_details WHERE master_id = {4} AND fully_qualified_table_name IN ('{0}','{1}','{2}','{3}','{5}')", chainTableName, taskTableName, chainSubscriptionTableName, taskSubscriptionTableName, masterId, dependenciesTableName, onFailTableName), CommonQueryType.Delete, ConnectionConstants.RefMaster_Connection);
                        }

                        RDALAbstractFactory.DBFactory.PutConnectionManager(con);
                    }
                }
                #endregion
            }
            catch (Exception ee)
            {
                mLogger.Error("ValidateandSaveTaskManager -----> ERROR : " + ee.ToString());
            }
            finally
            {
                mLogger.Error("ValidateandSaveTaskManager -----> END");
            }
        }

        #endregion
    }
}
