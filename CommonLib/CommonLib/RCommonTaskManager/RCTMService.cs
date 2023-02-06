
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;
using System.Text.RegularExpressions;
using com.ivp.rad.RCTMUtils;
using com.ivp.rad.RCTMService;
using System.Configuration;
using System.Timers;
using com.ivp.rad.common;
using System.Collections.Specialized;
using System.Net;
using System.Net.Mail;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using System.Data;
using System.Reflection;
using System.Threading;
using com.ivp.rad.viewmanagement;
using Newtonsoft.Json;
using System.IO;
using com.ivp.rad.transport;
using System.Xml.Linq;
using System.Diagnostics;
using System.Windows.Threading;
using System.Web;
using com.ivp.rad.RUserManagement;
using com.ivp.rad.BusinessCalendar;
using System.Collections.Concurrent;
using com.ivp.srmcommon;

namespace com.ivp.rad.RCommonTaskManager
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CTMService : RServiceBaseClass, ICTMService
    {
        public string clientKeepAlive()
        {
            return "OK";
        }
        public bool isAlive(string clientName)
        {
            return true;
        }

        //static RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
        //static RHashlist htParams = null;

        private RCTMJobExecuter mJobExecutor = null;
        static bool RunScheduler = true;
        
        private System.Timers.Timer mTimer = null;
        static IRLogger mLogger = RLogFactory.CreateLogger("RCTMService");
        //private RCTMFileWatcher filewatcher = null;

        //registeredModuleId, callbackchannel
        static Dictionary<string,Dictionary<int, ICTMServiceCallback>> registeredClients = new Dictionary<string, Dictionary<int, ICTMServiceCallback>>();


        static Dictionary<string, Dictionary<string, chainInfo>> runningChains = new Dictionary<string, Dictionary<string, chainInfo>>();
        public static ConcurrentDictionary<string, bool> ErrorOccurredWhileTriggeringScheduledJob = new ConcurrentDictionary<string, bool>();//false;

        public CTMService()
        {
            mLogger.Debug("CTMService:Ctor>>CTMService started");
            try
            {
                mLogger.Debug("CTMService:Ctor>>created new instance of mJobExecuter");
                mJobExecutor = new RCTMJobExecuter(this);


                string scheduledJobInterval = ConfigurationManager.AppSettings["ScheduledJobInterval"];
                string rs = ConfigurationManager.AppSettings["RunScheduler"];

                if (!string.IsNullOrEmpty(rs) && rs.Trim().Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    mLogger.Error("DO NOT RUN SCHEDULER !!!");
                    RunScheduler = false;
                }

                SRMMTConfig.MethodCallPerClient(UpdateNextScheduledTimeForMT);

                scheduledJobInterval = scheduledJobInterval ?? "20000";
                mTimer = new System.Timers.Timer(double.Parse(scheduledJobInterval));
                mTimer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                mTimer.Start();


            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:ctor>>Error while creating new instance of RCTMService");
                mLogger.Error(ex);
            }
        }

        private void UpdateNextScheduledTimeForMT(string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            if (!registeredClients.ContainsKey(clientName))
                registeredClients.Add(clientName, new Dictionary<int, ICTMServiceCallback>());
            if (!runningChains.ContainsKey(clientName))
                runningChains.Add(clientName, new Dictionary<string, chainInfo>());
            
            if (RCTMDic.RunningTasks == null)
                RCTMDic.RunningTasks = new Dictionary<string, Dictionary<int, List<int>>>();
            if (RCTMDic.WaitingQueue == null)
                RCTMDic.WaitingQueue = new Dictionary<string, Dictionary<int, SortedDictionary<DateTime, RCTMQueuedTaskInfo>>>();

            if (!RCTMDic.RunningTasks.ContainsKey(clientName))
                RCTMDic.RunningTasks.Add(clientName, new Dictionary<int, List<int>>());
            if (!RCTMDic.WaitingQueue.ContainsKey(clientName))
                RCTMDic.WaitingQueue.Add(clientName, new Dictionary<int, SortedDictionary<DateTime, RCTMQueuedTaskInfo>>());

            //Do not remove!!!!!!!!!!!!!!
            var isSuccess = UpdateNextScheduledTime();
            if (!isSuccess)
                ErrorOccurredWhileTriggeringScheduledJob.TryAdd(clientName, true);
            else
                ErrorOccurredWhileTriggeringScheduledJob.TryAdd(clientName, false);

            RetrieveQueues(clientName);
        }

        private void second_instance_timer_Elapsed(string clientName)
        {
            SRMMTConfig.SetClientName(clientName);
            mLogger.Error("second_instance_timer_Elapsed -----> Start");
            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            dbConnection.IsolationLevel = IsolationLevel.Serializable;

            try
            {
                DateTime now = DateTime.Now;
                List<RCTMQueuedTaskInfo> listTasksToTrigger = new List<RCTMQueuedTaskInfo>();
                List<int> lstTaskSummaryIdsToRemove = new List<int>();
                List<RCTMQueuedTaskInfo> lstTasksToMail = new List<RCTMQueuedTaskInfo>();
                HashSet<int> del = new HashSet<int>();
                List<int> runDel = new List<int>();

                lock (((ICollection)RCTMDic.RunningTasks).SyncRoot)
                {
                    List<int> statusIdsToCheck = new List<int>();
                    foreach (var kvp in RCTMDic.RunningTasks[clientName])
                    {
                        foreach (var id in kvp.Value)
                        {
                            statusIdsToCheck.Add(id);
                        }
                    }

                    HashSet<int> idsFoundInTaskStatus = new HashSet<int>();
                    if (statusIdsToCheck.Count > 0)
                        idsFoundInTaskStatus = new HashSet<int>(dbConnection.ExecuteQuery("SELECT task_status_id, status FROM dbo.ivp_rad_task_status WHERE task_status_id IN (" + string.Join(",", statusIdsToCheck) + ");", RQueryType.Select).Tables[0].AsEnumerable().Where(q => Convert.ToString(q["status"]).Equals("inprogress", StringComparison.OrdinalIgnoreCase)).Select(z => Convert.ToInt32(z["task_status_id"])));

                    foreach (var ii in statusIdsToCheck)
                    {
                        if (!idsFoundInTaskStatus.Contains(ii))
                            del.Add(ii);
                    }

                    foreach (var kvp in RCTMDic.RunningTasks[clientName])
                    {
                        foreach (var id in kvp.Value.ToArray())
                        {
                            if (del.Contains(id))
                            {
                                kvp.Value.Remove(id);
                            }
                        }
                        if (kvp.Value.Count == 0)
                            runDel.Add(kvp.Key);
                    }

                    foreach (var ir in runDel)
                        RCTMDic.RunningTasks[clientName].Remove(ir);
                }

                List<int> triggerabletaskSummaryIds = new List<int>();
                lock (((ICollection)RCTMDic.WaitingQueue).SyncRoot)
                {
                    foreach (KeyValuePair<int, SortedDictionary<DateTime, RCTMQueuedTaskInfo>> queueItem in RCTMDic.WaitingQueue[clientName])
                    {
                        List<DateTime> lstTasksToKill = new List<DateTime>();
                        foreach (KeyValuePair<DateTime, RCTMQueuedTaskInfo> item in queueItem.Value)
                        {
                            if (now > item.Key)
                            {
                                lstTasksToKill.Add(item.Key);
                                lstTasksToMail.Add(item.Value);
                            }
                        }

                        foreach (DateTime endTime in lstTasksToKill)
                        {
                            queueItem.Value.Remove(endTime);
                        }

                        if (queueItem.Value.Count == 0)
                            lstTaskSummaryIdsToRemove.Add(queueItem.Key);
                        else
                            triggerabletaskSummaryIds.Add(queueItem.Key);
                    }

                    foreach (int taskSummaryId in lstTaskSummaryIdsToRemove)
                    {
                        RCTMDic.WaitingQueue[clientName].Remove(taskSummaryId);
                    }
                }

                HashSet<int> taskSummaryIdsToTrigger = new HashSet<int>();
                lock (((ICollection)RCTMDic.RunningTasks).SyncRoot)
                {
                    foreach (int taskSummaryId in triggerabletaskSummaryIds)
                    {
                        if (!RCTMDic.RunningTasks[clientName].ContainsKey(taskSummaryId))
                        {
                            taskSummaryIdsToTrigger.Add(taskSummaryId);
                        }
                    }
                }

                lstTaskSummaryIdsToRemove = new List<int>();
                lock (((ICollection)RCTMDic.WaitingQueue).SyncRoot)
                {
                    foreach (KeyValuePair<int, SortedDictionary<DateTime, RCTMQueuedTaskInfo>> queueItem in RCTMDic.WaitingQueue[clientName])
                    {
                        if (taskSummaryIdsToTrigger.Contains(queueItem.Key))
                        {
                            var temp = queueItem.Value.FirstOrDefault().Value;
                            listTasksToTrigger.Add(temp);
                            queueItem.Value.Remove(temp.EndTime);
                        }
                        if (queueItem.Value.Count == 0)
                            lstTaskSummaryIdsToRemove.Add(queueItem.Key);
                    }
                    foreach (int taskSummaryId in lstTaskSummaryIdsToRemove)
                    {
                        RCTMDic.WaitingQueue[clientName].Remove(taskSummaryId);
                    }
                }

                mLogger.Error("second_instance_timer_Elapsed -----> Tasks to End Waiting (Status Ids): " + string.Join(",", lstTasksToMail.Select(x => x.CTMStatusId)));
                mLogger.Error("second_instance_timer_Elapsed -----> Tasks to Trigger (Status Ids): " + string.Join(",", listTasksToTrigger.Select(x => x.CTMStatusId)));

                Dictionary<string, List<KeyValuePair<string, DateTime>>> killedTasksUsersVsTaskInfo = new Dictionary<string, List<KeyValuePair<string, DateTime>>>();
                foreach (RCTMQueuedTaskInfo task in lstTasksToMail)
                {
                    foreach (string email in task.Subscribers)
                    {
                        if (!killedTasksUsersVsTaskInfo.ContainsKey(email))
                            killedTasksUsersVsTaskInfo[email] = new List<KeyValuePair<string, DateTime>>();
                        killedTasksUsersVsTaskInfo[email].Add(new KeyValuePair<string, DateTime>(task.TaskName, task.StartTime));
                    }
                    mLogger.Error("second_instance_timer_Elapsed -----> Syncing Task Status. Ending Wait (Status Id): " + task.CTMStatusId);
                    UpdateTaskStatus(new TaskStatusInfo { StatusId = task.CTMStatusId, Status = rad.RCTMUtils.TaskStatus.FAILED, end_time = DateTime.Now, TaskLog = "Instance Wait expired.", environmentVariables = task.EnvironmentVariables }, task.TaskSummaryId, clientName, dbConnection);

                }

                foreach (RCTMQueuedTaskInfo task in listTasksToTrigger)
                {
                    mLogger.Error("second_instance_timer_Elapsed -----> Triggering Task (Status Id): " + task.CTMStatusId);
                    TriggerTask(task.FlowId, task.ChainGuid, task.EnvironmentVariables, task.UserName, task.AsOfDate, clientName, task.CTMStatusId);
                }

                string fromMail = SRMCommon.GetConfigFromDB("FromEmailIdForTask");
                XElement mailTemplate = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + @"\ConfigFiles\CTMSubscription.xml").Element("InstanceTimeOutMailTemplate");
                StringBuilder mailBodyTemplate = new StringBuilder();
                string mailSubject = SRMCommon.GetConfigFromDB("InstanceName") + " " + mailTemplate.Element("Subject").Value;
                string tableTemp = "</br></br><table border=\"1\" style=\"border-collapse:collapse;\"><th style=\"padding:5px;\">Task Name</th><th style=\"padding:5px;\">Start Time</th>";
                StringBuilder mailBody = null;
                foreach (KeyValuePair<string, List<KeyValuePair<string, DateTime>>> kvp in killedTasksUsersVsTaskInfo)
                {
                    mailBody = new StringBuilder();
                    mailBody.Append(mailTemplate.Element("Salutation").Value).Append("</br>");
                    mailBody.Append(mailTemplate.Element("BodyHeader").Value);
                    mailBody.Append(tableTemp);
                    foreach (KeyValuePair<string, DateTime> info in kvp.Value)
                    {
                        mailBody.Append("<tr><td style=\"padding:5px;\">").Append(info.Key).Append("</td><td style=\"padding:5px;\">").Append(info.Value.ToString(@"MM-dd-yyyy hh:mm:ss")).Append("</td></tr>");
                    }
                    mailBody.Append("</table></br>");
                    mailBody.Append(mailTemplate.Element("BodyFooter").Value).Append("</br></br>");
                    mailBody.Append(mailTemplate.Element("Footnote").Value);
                    SendSubscriptionMail(kvp.Key, fromMail, null, mailBody.ToString(), mailSubject);
                }
                dbConnection.CommitTransaction();
            }
            catch (Exception ee)
            {
                dbConnection.RollbackTransaction();
                mLogger.Error("second_instance_timer_Elapsed : " + ee.ToString());
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
            mLogger.Error("second_instance_timer_Elapsed -----> End");
        }

        private void timeout_timer_Elapsed()
        {
            mLogger.Error("timeout_timer_Elapsed -----> Start");
            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            try
            {
                StringBuilder mailBody = null;
                string mailSubject;

                Dictionary<string, List<Tuple<int, string, DateTime>>> emailVsProcessId_TaskName_StartTime = new Dictionary<string, List<Tuple<int, string, DateTime>>>();
                List<int> statusIdsToProcess = new List<int>();
                DateTime now = DateTime.Now;
                DataSet ds = dbConnection.ExecuteQuery(@"SELECT task_status_id, process_id, ts.task_time_out, rf.task_wait_subscription_id, ct.inprogress_subscribers, tss.task_name, ts.start_time
                                    FROM dbo.ivp_rad_task_status ts
                                    INNER JOIN dbo.ivp_rad_flow rf ON (rf.flow_id = ts.flow_id AND (is_processed IS NULL OR is_processed = 0) AND status = 'INPROGRESS' AND process_id IS NOT NULL AND ts.task_time_out IS NOT NULL)
                                    INNER JOIN dbo.ivp_rad_chained_tasks ct ON ct.chain_id = rf.chain_id
                                    INNER JOIN dbo.ivp_rad_task_summary tss ON rf.task_summary_id = tss.task_summary_id 
                                    WHERE ts.start_time < ts.task_time_out AND GETDATE() > ts.task_time_out", RQueryType.Select);

                foreach (DataRow dr in ds.Tables[0].AsEnumerable())
                {
                    DateTime timeOut = Convert.ToDateTime(dr["task_time_out"]);

                    string[] subscribers = null;
                    int processId = Convert.ToInt32(dr["process_id"]);
                    int taskStatusId = Convert.ToInt32(dr["task_status_id"]);
                    String chainSubscribers = Convert.ToString(dr["inprogress_subscribers"]);
                    String taskSubscribers = Convert.ToString(dr["task_wait_subscription_id"]);
                    string taskName = Convert.ToString(dr["task_name"]);
                    DateTime startTime = Convert.ToDateTime(dr["start_time"]);

                    if (!string.IsNullOrEmpty(taskSubscribers))
                        subscribers = taskSubscribers.Split('|');
                    else if (!string.IsNullOrEmpty(chainSubscribers))
                        subscribers = chainSubscribers.Split('|');

                    statusIdsToProcess.Add(taskStatusId);

                    mLogger.Error("Time Out Subscribers : " + subscribers);
                    if (subscribers != null)
                    {
                        foreach (string email in subscribers)
                        {
                            if (!emailVsProcessId_TaskName_StartTime.ContainsKey(email))
                                emailVsProcessId_TaskName_StartTime[email] = new List<Tuple<int, string, DateTime>>();
                            emailVsProcessId_TaskName_StartTime[email].Add(new Tuple<int, string, DateTime>(processId, taskName, startTime));
                        }
                    }
                }

                string fromMail = SRMCommon.GetConfigFromDB("FromEmailIdForTask");
                XElement mailTemplate = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + @"\ConfigFiles\CTMSubscription.xml").Element("TaskTimeOutMailTemplate");
                StringBuilder mailBodyTemplate = new StringBuilder();
                mailSubject = SRMCommon.GetConfigFromDB("InstanceName") + " " + mailTemplate.Element("Subject").Value;
                string tableTemp = "</br></br><table border=\"1\" style=\"border-collapse:collapse;\"><th style=\"padding:5px;\">Task Name</th><th style=\"padding:5px;\">Process Id</th><th style=\"padding:5px;\">Start Time</th>";

                foreach (KeyValuePair<string, List<Tuple<int, string, DateTime>>> kvp in emailVsProcessId_TaskName_StartTime)
                {
                    mailBody = new StringBuilder();
                    mailBody.Append(mailTemplate.Element("Salutation").Value).Append("</br>");
                    mailBody.Append(mailTemplate.Element("BodyHeader").Value);
                    mailBody.Append(tableTemp);
                    foreach (Tuple<int, string, DateTime> ProcessId_TaskName_StartTime in kvp.Value)
                    {
                        mailBody.Append("<tr><td style=\"padding:5px;\">").Append(ProcessId_TaskName_StartTime.Item2).Append("</td><td style=\"padding:5px;\">").Append(ProcessId_TaskName_StartTime.Item1).Append("</td><td style=\"padding:5px;\">").Append(ProcessId_TaskName_StartTime.Item3.ToString(@"MM-dd-yyyy hh:mm:ss")).Append("</td></tr>");
                    }
                    mailBody.Append("</table></br>");
                    mailBody.Append(mailTemplate.Element("BodyFooter").Value).Append("</br></br>");
                    mailBody.Append(mailTemplate.Element("Footnote").Value);
                    SendSubscriptionMail(kvp.Key, fromMail, null, mailBody.ToString(), mailSubject);
                }

                if (statusIdsToProcess.Count > 0)
                {

                    dbConnection.ExecuteQuery(string.Format(@"DECLARE @tab TABLE(task_status_id INT); 
                                                        INSERT INTO @tab
                                                        SELECT item FROM dbo.REFM_GetList2Table('{0}',',');
                                                        UPDATE ts
                                                        SET ts.is_processed = 1
                                                        FROM dbo.ivp_rad_task_status ts
                                                        INNER JOIN @tab ta ON ta.task_status_id = ts.task_status_id", string.Join(",", statusIdsToProcess)), RQueryType.Update);
                }
                dbConnection.CommitTransaction();
            }
            catch (Exception ex)
            {
                mLogger.Error("timeout_timer_Elapsed  Error :" + ex.ToString());
                dbConnection.RollbackTransaction();
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
            mLogger.Error("timeout_timer_Elapsed -----> End");
        }

        private void sync_killed_tasks(string clientName)
        {
            SRMMTConfig.SetClientName(clientName);
            mLogger.Error("sync_killed_tasks -----> Start");
            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            try
            {
                DateTime now = DateTime.Now;
                HashSet<int> ctmProcesses = new HashSet<int>(Process.GetProcessesByName("CTMProcess").Select(x => x.Id));
                List<int> statusIdsForKilled = new List<int>();
                List<string> inserts = new List<string>();

                DataTable dtts = dbConnection.ExecuteQuery(@"SELECT ts.task_status_id, ts.start_time, ts.process_id, tss.task_summary_id, ts.flow_id, rf.chain_id, ts.chain_guid, tss.task_name FROM dbo.ivp_rad_task_status ts INNER JOIN dbo.ivp_rad_flow rf ON rf.flow_id = ts.flow_id INNER JOIN dbo.ivp_rad_task_summary tss ON tss.task_summary_id = rf.task_summary_id WHERE ts.status = 'INPROGRESS';", RQueryType.Select).Tables[0];
                List<DataRow> rowsWithProcessId = new List<DataRow>();
                HashSet<int> rowsWithoutProcessId = new HashSet<int>();
                List<int> ctmStatusIds = new List<int>();
                Dictionary<int, Tuple<int, int, string>> ctmStatusIdVsFlowIdWithChainIdandGuid = new Dictionary<int, Tuple<int, int, string>>();
                List<Tuple<int, int, string>> flowIdsToTrigger = new List<Tuple<int, int, string>>();
                Dictionary<KeyValuePair<string, int>, List<int>> guidVsCompletedTasks = new Dictionary<KeyValuePair<string, int>, List<int>>();
                HashSet<string> guids = new HashSet<string>();
                Dictionary<int, Tuple<string, int, string>> statusIdVsTaskInfo = new Dictionary<int, Tuple<string, int, string>>();
                Dictionary<int, TaskInfo> flowIdVsInfoForSyncedTasks = new Dictionary<int, TaskInfo>();

                foreach (DataRow drr in dtts.AsEnumerable())
                {
                    string chain_guid = Convert.ToString(drr["chain_guid"]);
                    int statusId = Convert.ToInt32(drr["task_status_id"]);
                    string processId = Convert.ToString(drr["process_id"]);

                    if (Math.Abs((now - Convert.ToDateTime(drr["start_time"])).TotalSeconds) > 120)
                    {
                        if (!string.IsNullOrEmpty(processId))
                        {
                            rowsWithProcessId.Add(drr);
                            ctmStatusIds.Add(statusId);
                            ctmStatusIdVsFlowIdWithChainIdandGuid.Add(statusId, new Tuple<int, int, string>(Convert.ToInt32(drr["flow_id"]), Convert.ToInt32(drr["chain_id"]), chain_guid));
                        }
                        else
                            rowsWithoutProcessId.Add(statusId);
                    }
                }

                lock (((ICollection)RCTMDic.WaitingQueue).SyncRoot)
                {
                    foreach (var queueItem in RCTMDic.WaitingQueue[clientName])
                    {
                        foreach (var kvp in queueItem.Value)
                        {
                            if (rowsWithoutProcessId.Contains(kvp.Value.CTMStatusId))
                            {
                                rowsWithoutProcessId.Remove(kvp.Value.CTMStatusId);
                            }
                        }
                    }
                }

                Dictionary<int, string> dictModuleVsAssemblyInfo = dbConnection.ExecuteQuery("SELECT * FROM dbo.ivp_rad_module", RQueryType.Select).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToInt32(x["module_id"]), y => Convert.ToString(y["assembly_info"]));
                Dictionary<int, DataRow> dictStatusIdVsRow = new Dictionary<int, DataRow>();
                HashSet<int> ctmstatusIdstoRemove = new HashSet<int>();

                if (ctmStatusIds.Count > 0)
                {
                    lock (((ICollection)registeredClients).SyncRoot)
                    {
                        foreach (var client in registeredClients[clientName])
                        {
                            try
                            {
                                if (client.Key == 3 || client.Key == 6)
                                {
                                    string[] arr = dictModuleVsAssemblyInfo[client.Key].Split('|');
                                    DataTable dtt = client.Value.SyncStatus(arr[0], arr[1], ctmStatusIds, clientName);
                                    foreach (DataRow dr in dtt.AsEnumerable())
                                    {
                                        int statusId = Convert.ToInt32(dr["ctm_status_id"]);
                                        DateTime endTime = Convert.ToDateTime(dr["end_time"]);
                                        if (!dictStatusIdVsRow.ContainsKey(statusId) && Math.Abs((now - endTime).TotalSeconds) > 120)
                                            dictStatusIdVsRow[statusId] = dr;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                mLogger.Error("Error while SyncStatus :" + ex.ToString());
                            }
                        }
                    }
                }

                foreach (DataRow dr in rowsWithProcessId)
                {
                    string processId = Convert.ToString(dr["process_id"]);
                    int statusId = Convert.ToInt32(dr["task_status_id"]);

                    if (dictStatusIdVsRow.ContainsKey(statusId))
                    {
                        Tuple<int, int, string> FlowIdWithChainIdandGuid = ctmStatusIdVsFlowIdWithChainIdandGuid[statusId];

                        if (!flowIdVsInfoForSyncedTasks.ContainsKey(FlowIdWithChainIdandGuid.Item1))
                            flowIdVsInfoForSyncedTasks.Add(FlowIdWithChainIdandGuid.Item1, GetTaskInfoFromFlowId(FlowIdWithChainIdandGuid.Item1));

                        DataRow dee = dictStatusIdVsRow[statusId];
                        string status = Convert.ToString(dee["status"]);
                        inserts.Add("(" + statusId + ", '" + status + "', '" + HttpUtility.HtmlEncode(Convert.ToString(dee["log_description"])) + "')");

                        if (status.Equals("PASSED", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(FlowIdWithChainIdandGuid.Item3))
                        {
                            guids.Add(FlowIdWithChainIdandGuid.Item3);
                            flowIdsToTrigger.Add(new Tuple<int, int, string>(FlowIdWithChainIdandGuid.Item1, FlowIdWithChainIdandGuid.Item2, FlowIdWithChainIdandGuid.Item3));
                        }

                        ctmstatusIdstoRemove.Add(statusId);
                    }
                    else if (!string.IsNullOrEmpty(processId) && !ctmProcesses.Contains(Convert.ToInt32(processId)))
                    {
                        statusIdsForKilled.Add(statusId);
                        statusIdVsTaskInfo.Add(statusId, new Tuple<string, int, string>(Convert.ToString(dr["task_name"]), Convert.ToInt32(processId), Convert.ToDateTime(dr["start_time"]).ToString(@"MM-dd-yyyy hh:mm:ss")));
                        ctmstatusIdstoRemove.Add(statusId);
                    }
                }

                lock (((ICollection)RCTMDic.RunningTasks).SyncRoot)
                {
                    List<int> tsId = new List<int>();
                    foreach (var runningTask in RCTMDic.RunningTasks[clientName])
                    {
                        foreach (int sId in runningTask.Value.ToArray())
                        {
                            if (ctmstatusIdstoRemove.Contains(sId))
                            {
                                runningTask.Value.Remove(sId);
                                mLogger.Error("CTMService: TriggerTask -> Remove in Running Task. CTM status Id :" + sId);
                            }
                        }

                        if (runningTask.Value.Count == 0)
                            tsId.Add(runningTask.Key);
                    }

                    foreach (var id in tsId)
                    {
                        RCTMDic.RunningTasks[clientName].Remove(id);
                        mLogger.Error("CTMService: TriggerTask -> Remove in Running Task. Summary Id :" + id);
                    }
                }

                mLogger.Error("sync_killed_tasks -----> Status Ids for which process was killed : " + string.Join(",", statusIdsForKilled));
                mLogger.Error("sync_killed_tasks -----> Status Ids for which Status is being synced : " + string.Join(",", dictStatusIdVsRow.Keys));
                mLogger.Error("sync_killed_tasks -----> Status Ids for which Status is being failed : " + string.Join(",", rowsWithoutProcessId));

                List<string> queries = new List<string>();
                if (statusIdsForKilled.Count > 0)
                {
                    queries.Add(string.Format(@"DECLARE @tab TABLE(task_status_id INT); 
                        INSERT INTO @tab
                        SELECT item FROM dbo.REFM_GetList2Table('{0}',',');

                        UPDATE ts
                        SET ts.is_processed = 1, ts.status = 'FAILED', ts.log_description = 'Task process was terminated abruptly. Please check the task logs for the details.', ts.end_time = GETDATE()
                        FROM dbo.ivp_rad_task_status ts
                        INNER JOIN @tab ta ON ta.task_status_id = ts.task_status_id;", string.Join(",", statusIdsForKilled)));

                    lock (((ICollection)registeredClients).SyncRoot)
                    {
                        foreach (var client in registeredClients[clientName])
                        {
                            try
                            {
                                if (client.Key == 3 || client.Key == 6)
                                {
                                    string[] arr = dictModuleVsAssemblyInfo[client.Key].Split('|');
                                    client.Value.KillInprogressTask(arr[0], arr[1], statusIdsForKilled, clientName);
                                }
                            }
                            catch (Exception ex)
                            {
                                mLogger.Error("Error while SyncStatus :" + ex.ToString());
                            }
                        }
                    }
                }

                if (inserts.Count > 0)
                {
                    queries.Add(string.Format(@"DECLARE @i_tab TABLE(task_status_id INT, status VARCHAR(MAX), log VARCHAR(MAX));  
INSERT INTO @i_tab(task_status_id, status, log) VALUES {0}
UPDATE ts
SET ts.is_processed = 1, ts.status = ta.status, ts.log_description = ta.log, ts.end_time = GETDATE()
FROM dbo.ivp_rad_task_status ts
INNER JOIN @i_tab ta ON ta.task_status_id = ts.task_status_id;", string.Join(",", inserts)));
                }

                if (rowsWithoutProcessId.Count > 0)
                {
                    queries.Add(string.Format(@"DECLARE @ptab TABLE(task_status_id INT); 
                        INSERT INTO @ptab
                        SELECT item FROM dbo.REFM_GetList2Table('{0}',',');

                        UPDATE ts
                        SET ts.is_processed = 1, ts.status = 'FAILED', ts.log_description = '', ts.end_time = GETDATE()
                        FROM dbo.ivp_rad_task_status ts
                        INNER JOIN @ptab ta ON ta.task_status_id = ts.task_status_id;", string.Join(",", rowsWithoutProcessId)));
                }

                if (queries.Count > 0)
                    dbConnection.ExecuteQuery(string.Join(" ", queries), RQueryType.Update);

                dbConnection.CommitTransaction();
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);


                //SEND MAIL
                StringBuilder mailBody = null;
                string fromMail = SRMCommon.GetConfigFromDB("FromEmailIdForTask");
                string toMail = SRMCommon.GetConfigFromDB("ToEmailIdForTaskProcessKilled");
                XElement mailTemplate = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + @"\ConfigFiles\CTMSubscription.xml").Element("ProcessKilledMailTemplate");
                StringBuilder mailBodyTemplate = new StringBuilder();
                string mailSubject = SRMCommon.GetConfigFromDB("InstanceName") + " " + mailTemplate.Element("Subject").Value;
                string tableTemp = "</br></br><table border=\"1\" style=\"border-collapse:collapse;\"><th style=\"padding:5px;\">Task Name</th><th style=\"padding:5px;\">Process Id</th><th style=\"padding:5px;\">Start Time</th>";
                string[] toMailIds = new string[0];

                if (!string.IsNullOrEmpty(toMail))
                    toMailIds = toMail.Split(',');

                if (statusIdVsTaskInfo.Count > 0)
                {
                    foreach (string emailId in toMailIds)
                    {
                        mailBody = new StringBuilder();
                        mailBody.Append(mailTemplate.Element("Salutation").Value).Append("</br>");
                        mailBody.Append(mailTemplate.Element("BodyHeader").Value);
                        mailBody.Append(tableTemp);
                        foreach (var kbvp in statusIdVsTaskInfo)
                        {
                            mailBody.Append("<tr><td style=\"padding:5px;\">").Append(kbvp.Value.Item1).Append("</td><td style=\"padding:5px;\">").Append(kbvp.Value.Item2).Append("</td><td style=\"padding:5px;\">").Append(kbvp.Value.Item3).Append("</td></tr>");
                        }
                        mailBody.Append("</table></br>");
                        mailBody.Append(mailTemplate.Element("BodyFooter").Value).Append("</br></br>");
                        mailBody.Append(mailTemplate.Element("Footnote").Value);
                        SendSubscriptionMail(emailId, fromMail, null, mailBody.ToString(), mailSubject);
                    }
                }


                //SUBSCRIPTION MAIL FOR SYNCED TASKS
                foreach (var kvp in flowIdVsInfoForSyncedTasks)
                {
                    if (kvp.Value.MailSubscribeId.ContainsKey(rad.RCTMUtils.TaskStatus.FAILED) && kvp.Value.MailSubscribeId[rad.RCTMUtils.TaskStatus.FAILED].MailIds != null && kvp.Value.MailSubscribeId[rad.RCTMUtils.TaskStatus.FAILED].MailIds.Count > 0)
                    {
                        foreach (var emailId in kvp.Value.MailSubscribeId[rad.RCTMUtils.TaskStatus.FAILED].MailIds)
                            SendSubscriptionMail(emailId, fromMail, null, kvp.Value.MailSubscribeId[rad.RCTMUtils.TaskStatus.FAILED].mailBody, kvp.Value.MailSubscribeId[rad.RCTMUtils.TaskStatus.FAILED].MailSubject);
                    }
                }


                //TRIGGER NEXT TASK ON SUCCESS
                flowIdsToTrigger = flowIdsToTrigger.GroupBy(x => x.Item2).ToDictionary(s => s.Key, g => g.ToList()).Select(z => z.Value.OrderByDescending(d => d.Item1).FirstOrDefault()).ToList();

                if (flowIdsToTrigger.Count > 0)
                {
                    dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");

                    guidVsCompletedTasks = dbConnection.ExecuteQuery(string.Format(@"DECLARE @tab TABLE (guid VARCHAR(MAX));

                    INSERT INTO @tab
                    SELECT item FROM dbo.REFM_GetList2Table('{0}',',')

                    SELECT ts.*, rf.chain_id
                    FROM dbo.ivp_rad_task_status ts
                    INNER JOIN @tab tb ON ts.chain_guid = tb.guid
                    INNER JOIN dbo.ivp_rad_flow rf ON ts.flow_id = rf.flow_id
                    WHERE status IN ('PASSED','FAILED') AND start_time > DATEADD(DAY, -2, DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), 0))", string.Join(",", guids)), RQueryType.Select).Tables[0].AsEnumerable().GroupBy(q => new KeyValuePair<string, int>(Convert.ToString(q["chain_guid"]), Convert.ToInt32(q["chain_id"]))).ToDictionary(x => x.Key, y => y.Select(s => Convert.ToInt32(s["flow_id"])).ToList());

                    lock (((ICollection)runningChains).SyncRoot)
                    {
                        foreach (var kvp in guidVsCompletedTasks)
                        {
                            if (!runningChains[clientName].ContainsKey(kvp.Key.Key))
                                runningChains[clientName].Add(kvp.Key.Key, new chainInfo(kvp.Key.Value));
                            runningChains[clientName][kvp.Key.Key].completedTasks = kvp.Value.ToList();
                        }
                    }

                    foreach (Tuple<int, int, string> kvp in flowIdsToTrigger)
                    {
                        mLogger.Error("Continuing batch for chain_id : " + kvp.Item2 + "after flow_id : " + kvp.Item1);

                        List<int> deps = FindAllDependantsOn(kvp.Item1, kvp.Item2);
                        if (deps != null && deps.Count > 0)
                        {
                            mLogger.Debug("found the following dependants>>" + string.Join(",", deps));
                            foreach (int depflowId in deps)
                            {
                                TaskInfo depcurrTask = GetTaskInfoFromFlowId(depflowId);
                                mLogger.Debug("flow id=>" + depflowId);
                                if (AllDependantsAreCompleteOrMuted(depflowId, kvp.Item3, clientName) == true)
                                {
                                    if (depcurrTask.IsMuted == false)
                                    {
                                        TriggerTaskInChain(kvp.Item2, depflowId, "System", null, kvp.Item3);
                                    }
                                    else
                                    {
                                        mLogger.Debug("skipping muted task with flow id >> " + depflowId);
                                        depcurrTask.Status = new TaskStatusInfo();
                                        depcurrTask.Status.Status = com.ivp.rad.RCTMUtils.TaskStatus.PASSED;
                                        TaskCompleted(depcurrTask, kvp.Item3, clientName);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                mLogger.Error("sync_killed_tasks Error : " + ex.ToString());
                dbConnection.RollbackTransaction();
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
            mLogger.Error("sync_killed_tasks -----> End");
        }

        public static void DumpQueues(string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Error("DumpQueues ----> Start");

            try
            {
                List<string> inserts = new List<string>();
                RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                dbConnection.IsolationLevel = IsolationLevel.Serializable;
                try
                {
                    mLogger.Error("DumpQueues : Dumping Running Tasks ----> Start");
                    lock (((ICollection)RCTMDic.RunningTasks).SyncRoot)
                    {
                        foreach (var taskSummaryId in RCTMDic.RunningTasks[clientName])
                        {
                            foreach (var statusId in taskSummaryId.Value)
                            {
                                inserts.Add("(" + taskSummaryId.Key + ", " + statusId + ")");
                            }
                        }
                        RCTMDic.RunningTasks[clientName].Clear();
                    }
                    if (inserts.Count > 0)
                        dbConnection.ExecuteQuery("TRUNCATE TABLE dbo.ivp_rad_running_tasks; INSERT INTO dbo.ivp_rad_running_tasks(task_summary_id, status_id) VALUES " + string.Join(",", inserts), RQueryType.Insert);
                    dbConnection.CommitTransaction();
                }
                catch (Exception ee)
                {
                    mLogger.Error("DumpQueues : Dumping Running Tasks -----> Error" + ee.ToString());
                    dbConnection.RollbackTransaction();
                }
                finally
                {
                    mLogger.Error("DumpQueues : Dumping Running Tasks ----> End");
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                }

                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                dbConnection.IsolationLevel = IsolationLevel.Serializable;

                try
                {
                    mLogger.Error("DumpQueues : Dumping Waiting Tasks ----> Start");
                    inserts = new List<string>();
                    lock (((ICollection)RCTMDic.WaitingQueue).SyncRoot)
                    {
                        foreach (var taskSummaryIds in RCTMDic.WaitingQueue[clientName])
                        {
                            foreach (var kvp in taskSummaryIds.Value)
                            {
                                RCTMQueuedTaskInfo info = kvp.Value;
                                inserts.Add("(" + info.FlowId + ", '" + info.ChainGuid + "', '" + info.EnvironmentVariables + "', '" + info.UserName + "', " + (info.AsOfDate == null ? "NULL" : "'" + info.AsOfDate + "'") + ", '" + info.EndTime + "', " + info.CTMStatusId + ", '" + info.StartTime + "', '" + string.Join("|", info.Subscribers) + "', '" + info.TaskName + "', " + info.TaskSummaryId + ")");
                            }
                        }
                        RCTMDic.WaitingQueue[clientName].Clear();
                    }
                    if (inserts.Count > 0)
                        dbConnection.ExecuteQuery("TRUNCATE TABLE dbo.ivp_rad_second_instance_queue; INSERT INTO dbo.ivp_rad_second_instance_queue (flow_id, chain_guid, environmentVariables, username, as_of_date, end_time, status_id, start_time, subscribers, task_name, task_summary_id) VALUES " + string.Join(",", inserts), RQueryType.Insert);
                    dbConnection.CommitTransaction();
                }
                catch (Exception ee)
                {
                    mLogger.Error("DumpQueues : Dumping Waiting Tasks -----> Error" + ee.ToString());
                    dbConnection.RollbackTransaction();
                }
                finally
                {
                    mLogger.Error("DumpQueues : Dumping Waiting Tasks ----> End");
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                }
            }
            catch (Exception e)
            {
                mLogger.Error("Dumping Queues Error -----> " + e.ToString());
            }
            mLogger.Error("Dumping Queues ----> End");
        }

        public static void RetrieveQueues(string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Error("RetrieveQueues ----> Start");

            try
            {
                RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                dbConnection.IsolationLevel = IsolationLevel.Serializable;
                try
                {
                    mLogger.Error("RetrieveQueues : Retrieving Running Tasks ----> Start");

                    //if (RCTMDic.RunningTasks == null)
                    //    RCTMDic.RunningTasks = new Dictionary<string, Dictionary<int, List<int>>>();
                    //if (!RCTMDic.RunningTasks.ContainsKey(clientName))
                    //    RCTMDic.RunningTasks.Add(clientName, new Dictionary<int, List<int>>());
                    RCTMDic.RunningTasks[clientName] = new Dictionary<int, List<int>>();

                    lock (((ICollection)RCTMDic.RunningTasks).SyncRoot)
                    {
                        DataTable dta = dbConnection.ExecuteQuery("SELECT task_summary_id, status_id, task_status_id, flow_id, status FROM dbo.ivp_rad_running_tasks rt LEFT JOIN dbo.ivp_rad_task_status ts ON rt.status_id = ts.task_status_id;", RQueryType.Select).Tables[0];
                        foreach (DataRow dr in dta.AsEnumerable())
                        {
                            int taskSummaryId = Convert.ToInt32(dr["task_summary_id"]);
                            string taskStatusId = Convert.ToString(dr["task_status_id"]);

                            if (!string.IsNullOrEmpty(taskStatusId) && Convert.ToString(dr["status"]).Equals("inprogress", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!RCTMDic.RunningTasks[clientName].ContainsKey(taskSummaryId))
                                    RCTMDic.RunningTasks[clientName][taskSummaryId] = new List<int>();
                                RCTMDic.RunningTasks[clientName][taskSummaryId].Add(Convert.ToInt32(taskStatusId));
                            }
                        }
                    }
                    dbConnection.CommitTransaction();
                }
                catch (Exception ee)
                {
                    mLogger.Error("RetrieveQueues : Retrieving Running Tasks -----> Error" + ee.ToString());
                    dbConnection.RollbackTransaction();
                }
                finally
                {
                    mLogger.Error("RetrieveQueues : Retrieving Running Tasks ----> End");
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                }

                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                dbConnection.IsolationLevel = IsolationLevel.Serializable;

                try
                {
                    mLogger.Error("RetrieveQueues : Retrieving Waiting Tasks ----> Start");
                    //if (RCTMDic.WaitingQueue == null)
                    //    RCTMDic.WaitingQueue = new Dictionary<string, Dictionary<int, SortedDictionary<DateTime, RCTMQueuedTaskInfo>>>();
                    //if (!RCTMDic.WaitingQueue.ContainsKey(clientName))
                    //    RCTMDic.WaitingQueue.Add(clientName, new Dictionary<int, SortedDictionary<DateTime, RCTMQueuedTaskInfo>>());
                    RCTMDic.WaitingQueue[clientName] = new Dictionary<int, SortedDictionary<DateTime, RCTMQueuedTaskInfo>>();

                    DataTable dt = dbConnection.ExecuteQuery("SELECT flow_id, chain_guid, environmentVariables, username, as_of_date, end_time, status_id, start_time, subscribers, task_name, task_summary_id FROM dbo.ivp_rad_second_instance_queue;", RQueryType.Select).Tables[0];
                    lock (((ICollection)RCTMDic.WaitingQueue).SyncRoot)
                    {
                        foreach (DataRow dr in dt.AsEnumerable())
                        {
                            int taskSummaryId = Convert.ToInt32(dr["task_summary_id"]);
                            DateTime endTime = Convert.ToDateTime(dr["end_time"]);
                            string asOfDate = Convert.ToString(dr["as_of_date"]);
                            string subscribers = Convert.ToString(dr["subscribers"]);

                            if (!RCTMDic.WaitingQueue[clientName].ContainsKey(taskSummaryId))
                                RCTMDic.WaitingQueue[clientName][taskSummaryId] = new SortedDictionary<DateTime, RCTMQueuedTaskInfo>();
                            if (!RCTMDic.WaitingQueue[clientName][taskSummaryId].ContainsKey(endTime))
                                RCTMDic.WaitingQueue[clientName][taskSummaryId][endTime] = new RCTMQueuedTaskInfo { ChainGuid = Convert.ToString(dr["chain_guid"]), CTMStatusId = Convert.ToInt32(dr["status_id"]), EndTime = endTime, EnvironmentVariables = Convert.ToString(dr["environmentVariables"]), FlowId = Convert.ToInt32(dr["flow_id"]), StartTime = Convert.ToDateTime(dr["start_time"]), Subscribers = (!string.IsNullOrEmpty(subscribers) ? subscribers.Split('|') : new string[0]), TaskName = Convert.ToString(dr["task_name"]), TaskSummaryId = taskSummaryId, UserName = Convert.ToString(dr["username"]) };

                            if (!string.IsNullOrEmpty(asOfDate))
                                RCTMDic.WaitingQueue[clientName][taskSummaryId][endTime].AsOfDate = Convert.ToDateTime(asOfDate);
                            else
                                RCTMDic.WaitingQueue[clientName][taskSummaryId][endTime].AsOfDate = null;
                        }
                    }
                    dbConnection.CommitTransaction();
                }
                catch (Exception ee)
                {
                    mLogger.Error("RetrieveQueues : Retrieving Waiting Tasks -----> Error" + ee.ToString());
                    dbConnection.RollbackTransaction();
                }
                finally
                {
                    mLogger.Error("RetrieveQueues : Retrieving Waiting Tasks ----> End");
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                }
            }
            catch (Exception e)
            {
                mLogger.Error("Retrieving Queues Error -----> " + e.ToString());
            }
            mLogger.Error("Retrieving Queues ----> End");
        }

        //checks for scheduled jobs and sends keepalive to all clients
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                SRMMTConfig.MethodCallPerClient(timer_ElapsedMT);                
            }
            catch (Exception ee)
            {
                mLogger.Error("ERROR in Polling thread" + ee.ToString());
            }
        }

        private void timer_ElapsedMT(string clientName)
        {
            if (RunScheduler)
            {
                if (ErrorOccurredWhileTriggeringScheduledJob[clientName])
                {
                    var isSuccess = UpdateNextScheduledTime();
                    if (isSuccess)
                        ErrorOccurredWhileTriggeringScheduledJob[clientName] = false;
                }
                mJobExecutor.RunScheduledJobs(clientName);
            }

            lock (((ICollection)registeredClients).SyncRoot)
            {
                foreach (var client in registeredClients[clientName])
                {
                    try
                    {
                        client.Value.keepAlive(clientName);
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("Error while keep alive :" + ex.ToString());
                        registeredClients[clientName].Remove(client.Key);
                    }
                }
            }

            sync_killed_tasks(clientName);
            second_instance_timer_Elapsed(clientName);
            timeout_timer_Elapsed();
        }

        //called by ctm ctor
        private bool UpdateNextScheduledTime()
        {
            try
            {
                mLogger.Debug("Scheduler-> Update Next scheduled times on scheduler restart");
                mJobExecutor.UpdateNextScheduledTime();
                mLogger.Debug("Scheduler-> Updated Next scheduled times on scheduler restart");
                return true;
            }
            catch (Exception ee)
            {
                mLogger.Error("ERROR while updating next scheduled time : " + ee.ToString());
                return false;
            }
        }

        //every client registers with CTM service on startup where the callback channel is stored in registeredClients dictionary
        public void RegisterClient(int registeredModuleId,string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Debug("CTMService: RegisterCLient -> Client with moduleid " + registeredModuleId + " registered with CTM Core Service at " + DateTime.Now);
            try
            {
                lock (((ICollection)registeredClients).SyncRoot)
                {
                    ICTMServiceCallback client = OperationContext.Current.GetCallbackChannel<ICTMServiceCallback>();
                    var curr = OperationContext.Current.InstanceContext;
                    curr.Faulted += new EventHandler(curr_Faulted);
                    registeredClients[clientName][registeredModuleId] = client;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService: RegisterCLient -> Error while registering client with moduleId " + registeredModuleId);
                mLogger.Error(ex);
            }
        }

        void curr_Faulted(object sender, EventArgs e)
        {

        }

        ///<summary>Called by UI or scheduler to trigger a chain from the start</summary>
        ///<param name="chainId">Chain id of the chain to be triggered</param>
        ///<param name="isService">'true' for tasks run by the scheduler or filewatcher else 'false'</param>
        ///<param name="username">The user who trigerred the tasks. Will be System for tasks run by the scheduler or filewatcher</param>
        public string TriggerChain(int chainId, bool isService, string username, DateTime? asOfDate,string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Debug("CTMService:TriggerChain>> triggering chain with chain id:" + chainId + " and isService:" + isService);
            try
            {
                //if (isLessThanMaxParallelRun(chainId) == true)
                //{
                Guid guid = Guid.NewGuid();
                string guidString = guid.ToString();
                lock (((ICollection)runningChains).SyncRoot)
                    runningChains[clientName][guidString] = new chainInfo(chainId) { IsService = isService };// { chainId = chainId };
                int headFlowId = FindFlowIdOfHead(chainId);
                if (isService == true)
                {
                    if (IsMuted(headFlowId) == true)//if head task is muted run all dependants
                    {
                        var deps = FindAllDependantsOn(headFlowId, chainId);
                        if (deps.Count > 0)
                        {
                            foreach (int flowTaskId in deps)
                            {
                                if (AllDependantsAreCompleteOrMuted(flowTaskId, guidString, clientName) == true)
                                {
                                    if (!IsMuted(flowTaskId))
                                        TriggerTask(flowTaskId, guidString, null, "System", asOfDate, clientName);
                                }
                            }
                        }
                        else
                        {//no deps found on muted head
                            lock (((ICollection)runningChains).SyncRoot)
                                runningChains[clientName].Remove(guidString);
                        }
                    }
                    else//head is not muted -> trigger head
                    {
                        TriggerTask(headFlowId, guid.ToString(), null, username, asOfDate, clientName);
                    }
                }
                else
                {//manually triggerred task -> trigger head task
                    if (!IsMuted(headFlowId))
                        TriggerTask(headFlowId, guid.ToString(), null, username, asOfDate, clientName);
                    else return "Unable to Trigger Task: Task is Muted";
                }
                //}
                //else
                //{
                //    return "Unable to Trigger Task: Max Parallel Run Limit Exceeded";
                //}
                return "Task Triggered Successfully";
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:TriggerChain>>Error while triggerring chain with chain id" + chainId + " and isservice:" + isService);
                mLogger.Error(ex);
                return "Error while triggering task";
            }
        }

        ///<summary>Chain is trigerred from the specified task given by flow id, all the tasks 
        ///following this task are executed normally after this tasks completes</summary>
        ///<param name="chainId">Chain id of the chain to be trigerred</param>
        ///<param name="FlowId">Flow id of the task from where to begin execution</param>
        ///<param name="username">The user who trigerred the task</param>
        public string TriggerTaskInChainFromExternalSystem(int chainId, int FlowId, string username, DateTime? asOfDate, string clientName,string passedGuid = null)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Debug("CTMService: TriggerTaskInChainFromExternalSystem>> Triggerring task in chain with chainId:" + chainId + " and flowId" + FlowId);
            try
            {
                string guid = string.Empty;
                if (passedGuid == null)
                    guid = Guid.NewGuid().ToString();
                else
                    guid = passedGuid;

                if (!IsMuted(FlowId))
                {
                    lock (((ICollection)runningChains).SyncRoot)
                        runningChains[clientName][guid] = new chainInfo(chainId);
                    TriggerTask(FlowId, guid, null, username, asOfDate, clientName);
                }
                else
                    return string.Empty;

                return guid;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService: TriggerTaskInChainFromExternalSystem>> Error while trying to TriggerTaskInChainFromExternalSystem with chain id:" + chainId + " and flowId:" + FlowId);
                mLogger.Error(ex);
                throw;
            }
        }

        ///<summary>Chain is trigerred from the specified task given by flow id, all the tasks 
        ///following this task are executed normally after this tasks completes</summary>
        ///<param name="chainId">Chain id of the chain to be trigerred</param>
        ///<param name="FlowId">Flow id of the task from where to begin execution</param>
        ///<param name="username">The user who trigerred the task</param>
        public string TriggerTaskInChain(int chainId, int FlowId, string username, DateTime? asOfDate, string clientName,string passedGuid = null)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Debug("CTMService: TriggerTaskInChain>> Triggerring task in chain with chainId:" + chainId + " and flowId" + FlowId);
            try
            {
                //if (isLessThanMaxParallelRun(chainId))
                //{
                if (!IsMuted(FlowId))
                {
                    string guid = string.Empty;
                    if (passedGuid == null)
                        guid = Guid.NewGuid().ToString();
                    else
                        guid = passedGuid;

                    lock (((ICollection)runningChains).SyncRoot)
                    {
                        if (!runningChains[clientName].ContainsKey(guid) || runningChains[clientName][guid].chainId != chainId)
                            runningChains[clientName][guid] = new chainInfo(chainId);// { chainId = chainId };
                    }

                    TriggerTask(FlowId, guid, null, username, asOfDate, clientName);
                }
                else return "Unable to Trigger Task: Task is Muted";
                //}
                //else return "Unable to Trigger Task: Max Parallel Run Limit Exceeded";
                return "Task Triggered Successfully";
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService: TriggerTaskInCahin>> Error while trying to TriggerTaskInChain with chain id:" + chainId + " and flowId:" + FlowId);
                mLogger.Error(ex);
                throw;
            }
        }

        ///<summary>A single task in chain is triggered manually, only this task in chain is executed</summary>
        ///<param name="chainId">Chain id of the chain to be trigerred</param>
        ///<param name="FlowId">Flow id of the task from where to begin execution</param>
        ///<param name="username">The user who trigerred the task</param>
        public string TriggerSingleTaskInChain(int chainId, int FlowId, string username, DateTime? asOfDate,string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Debug("CTMService: TriggerSingleTaskInChain>> Triggerring single task from a chain with chainId:" + chainId + " and flowId:" + FlowId);
            try
            {
                //if (isLessThanMaxParallelRun(chainId))
                if (!IsMuted(FlowId))
                    TriggerTask(FlowId, null, null, username, asOfDate, clientName);
                else return "Unable to Trigger Task: Task is Muted";
                //}
                //else return "Unable to Trigger Task: Max Parallel Run Limit Exceeded";

                return "Task Triggered Successfully";

            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService: TriggerSingleTaskInChain>> Error while trying to trigger single task in chain with chain id:" + chainId + " and flowId:" + FlowId);
                mLogger.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Triggers the specified task on the registered clien
        /// Called internally by :
        /// Triggerchain()
        /// TriggerTaskInChain -> called with null environmentVariables
        /// TriggerSingleTaskInChain ->called with null guid and null environmentvariables 
        /// TaskCompleted()
        /// </summary>
        /// <param name="flowId">Flow id of the task from where to begin execution</param>
        /// <param name="guid">Unique id generated for each instance of chain run</param>
        /// <param name="environmentVariables">Extra info</param>
        /// <param name="username">The user who trigerred the task</param>
        private void TriggerTask(int flowId, string guid, string environmentVariables, string username, DateTime? asOfDate, string clientName, int statusId = -1)
        {
            SRMMTConfig.SetClientName(clientName);

            TaskInfo info = null;
            RDBConnectionManager dbConnection = null;
            try
            {
                bool failTask = false;
                bool isWaiting = true;
                DateTime startTime;
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");

                mLogger.Error("CTMService: TriggerTask -> Triggering task with flowId:" + flowId + " guid: " + guid + " environmentVariables: " + environmentVariables);
                info = GetTaskInfoFromFlowId(flowId);

                if (info.IsActive == false)
                    throw new Exception("Task has been deleted");

                ICTMServiceCallback client = null;
                int registeredModuleId = GetRegisteredModuleIdByFlowId(flowId);// this is registeredmoduleid
                bool hasModuleRegistered = false;
                lock (((IDictionary)registeredClients).SyncRoot)
                {
                    hasModuleRegistered = registeredClients[clientName].ContainsKey(registeredModuleId);
                }
                if (!hasModuleRegistered)//client not registered with CTM
                {
                    if (statusId == -1)
                        statusId = AddTaskStatus(flowId, guid, username, out startTime);//add inprogress status in db with start time and no end time

                    info.Status = new TaskStatusInfo() { Status = com.ivp.rad.RCTMUtils.TaskStatus.INPROGRESS, StatusId = statusId, environmentVariables = environmentVariables, chain_guid = guid };
                    info.Status.Status = com.ivp.rad.RCTMUtils.TaskStatus.FAILED;
                    info.Status.TaskLog = "Error: Client not currently registered with CTM Service right now";
                    TaskCompleted(info, guid, clientName);
                    return;
                }

                else//client found in registered clients
                {
                    lock (((ICollection)registeredClients).SyncRoot)
                        client = registeredClients[clientName][registeredModuleId];
                    if (guid != null && runningChains[clientName].ContainsKey(guid))//guid is null when a 'single' task is trigerred from the UI
                    {
                        runningChains[clientName][guid].runningTasks.Add(GetTaskInfoFromFlowId(flowId));
                        info.isService = runningChains[clientName][guid].IsService;
                    }
                    if (info.isService && RCalenderUtils.IsHoliday(DateTime.Now, RCalenderUtils.GetCalendarIdByName(info.CalendarName)))
                    {
                        return;
                    }

                    if (statusId == -1)
                        statusId = AddTaskStatus(flowId, guid, username, out startTime);//add inprogress status in db with start time and no end time
                    else
                        startTime = Convert.ToDateTime(dbConnection.ExecuteQuery(@"SELECT start_time FROM dbo.ivp_rad_task_status WHERE task_status_id = " + statusId, RQueryType.Select).Tables[0].Rows[0]["start_time"]);

                    lock (RCTMDic.LockObject)
                    {
                        //Set time out value
                        isWaiting = IsWaiting(info, guid, environmentVariables, username, asOfDate, statusId, startTime, clientName,out failTask);

                        if (!isWaiting && !failTask)
                        {
                            info.ChainGUID = guid;
                            if (asOfDate != null)
                            {
                                info.triggerAsOfDateInfo.triggerDate = asOfDate;
                            }
                            info.ExtraInfo = "{\"username\":\"" + username + "\"}";
                            info.IsLastTaskInChain = FindAllDependantsOn(flowId, info.ChainId).Count > 0 ? false : true;
                            info.Status = new TaskStatusInfo() { Status = com.ivp.rad.RCTMUtils.TaskStatus.INPROGRESS, StatusId = statusId, environmentVariables = environmentVariables, chain_guid = guid };

                            bool forcedWait = false;
                            string[] arr = getAssemblyInfoByModuleId(info.ModuleId).Split('|');
                            lock (((ICollection)RCTMDic.RunningTasks).SyncRoot)
                            {
                                if (info.ModuleId == 3 || info.ModuleId == 6)
                                {
                                    List<int> taskSummaryIds = new List<int>();
                                    List<int> taskMasterIds = client.isSecureToTrigger(arr[0], arr[1], info.TaskMasterId, clientName);
                                    mLogger.Error("CTMService: TriggerTask -> TaskMasterIds from Client :" + string.Join(",", taskMasterIds));

                                    if (taskMasterIds.Count > 0)
                                        taskSummaryIds = dbConnection.ExecuteQuery(@"SELECT DISTINCT task_summary_id FROM dbo.ivp_rad_task_summary WHERE task_master_id IN (" + string.Join(",", taskMasterIds) + ") AND module_id = 3;", RQueryType.Select).Tables[0].AsEnumerable().Select(x => Convert.ToInt32(x["task_summary_id"])).ToList();

                                    if (!taskSummaryIds.Any(x => RCTMDic.RunningTasks[clientName].ContainsKey(x)))
                                    {
                                        if (!RCTMDic.RunningTasks[clientName].ContainsKey(info.TaskSummaryId))
                                            RCTMDic.RunningTasks[clientName].Add(info.TaskSummaryId, new List<int>());
                                        RCTMDic.RunningTasks[clientName][info.TaskSummaryId].Add(statusId);

                                        mLogger.Error("CTMService: TriggerTask -> Added in Running Task :" + info.TaskSummaryId);
                                        try
                                        {
                                            mLogger.Error("CTMService: TriggerTask -> Triggering task (Status Id): " + statusId);
                                            client.triggerTask(info, guid, clientName);
                                        }
                                        catch (Exception es)
                                        {
                                            List<int> ts = new List<int>();
                                            foreach (var rtask in RCTMDic.RunningTasks[clientName])
                                            {
                                                foreach (var statId in rtask.Value.ToArray())
                                                {
                                                    if (statId == statusId)
                                                    {
                                                        rtask.Value.Remove(statusId);
                                                        mLogger.Error("CTMService: TriggerTask -> Removing from Running Task (Status Id):" + statusId);
                                                    }
                                                }
                                                if (rtask.Value.Count == 0)
                                                    ts.Add(rtask.Key);
                                            }
                                            foreach (var tsId in ts)
                                            {
                                                RCTMDic.RunningTasks[clientName].Remove(tsId);
                                                mLogger.Error("CTMService: TriggerTask -> Remove in Running Task (Task Summary Id):" + tsId);
                                            }
                                        }
                                    }
                                    else
                                        forcedWait = true;
                                }
                                else
                                {
                                    if (!RCTMDic.RunningTasks[clientName].ContainsKey(info.TaskSummaryId))
                                        RCTMDic.RunningTasks[clientName].Add(info.TaskSummaryId, new List<int>());
                                    RCTMDic.RunningTasks[clientName][info.TaskSummaryId].Add(statusId);

                                    mLogger.Error("CTMService: TriggerTask -> Added in Running Task :" + info.TaskSummaryId);
                                    try
                                    {
                                        client.triggerTask(info, guid, clientName);
                                    }
                                    catch (Exception e)
                                    {
                                        List<int> ts = new List<int>();
                                        foreach (var rtask in RCTMDic.RunningTasks[clientName])
                                        {
                                            foreach (var statId in rtask.Value.ToArray())
                                            {
                                                if (statId == statusId)
                                                {
                                                    rtask.Value.Remove(statusId);
                                                    mLogger.Error("CTMService: TriggerTask -> Removing from Running Task (Status Id):" + statusId);
                                                }
                                            }
                                            if (rtask.Value.Count == 0)
                                                ts.Add(rtask.Key);
                                        }
                                        foreach (var tsId in ts)
                                        {
                                            RCTMDic.RunningTasks[clientName].Remove(tsId);
                                            mLogger.Error("CTMService: TriggerTask -> Remove in Running Task (Task Summary Id):" + tsId);
                                        }
                                        mLogger.Error("CTMService: TriggerTask -> Remove in Running Task :" + info.TaskSummaryId);
                                    }
                                }
                            }
                            if (forcedWait)
                                IsWaiting(info, guid, environmentVariables, username, asOfDate, statusId, startTime, clientName,out failTask, false, true);
                        }
                        else if (failTask)
                        {
                            throw new Exception("An earlier instance of the task is in progress.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService: TriggerTask-> Error while trying to trigger task");
                mLogger.Error(ex);
                if (statusId != -1)
                {
                    UpdateTaskStatus(new TaskStatusInfo()
                    {
                        Status = com.ivp.rad.RCTMUtils.TaskStatus.FAILED,
                        TaskLog = ex.Message,
                        StatusId = statusId,
                        environmentVariables = environmentVariables + "|Error:" + ex.Message + "|",
                        chain_guid = guid
                    }, info == null ? 0 : info.TaskSummaryId, clientName);
                }

            }
        }

        /// <summary>
        /// Trigerred by a client when the trigerred task completes. Triggers the next task to be run in the chain
        /// </summary>
        /// <param name="taskinfo">Task info object of the task completed on client. Will contain the updated status info.</param>
        /// <param name="guid">Unique id generated for each instance of chain run returned as it is by the client</param>
        public void TaskCompleted(TaskInfo taskinfo, string guid,string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Debug("CTMService: TaskCompleted >> TaskCompleted on client with taskinfo:" + taskinfo.ToString() + " and guid:" + guid);
            try
            {
                //update the status of competed task in db
                if (taskinfo.IsMuted == false)
                {
                    mLogger.Debug("Updating the status of completed task in db");
                    UpdateTaskStatus(taskinfo.Status, taskinfo.TaskSummaryId, clientName);
                    UpdateAdditionalInfoForTask(taskinfo.AdditionalInfo, taskinfo.Status.StatusId);
                }
                //completed task is in running chain
                if (guid != null && runningChains[clientName].ContainsKey(guid) == true)
                {
                    mLogger.Debug("found task in running chain");
                    chainInfo chainInfo = null;
                    int chainId = -1;
                    lock (((ICollection)runningChains).SyncRoot)
                    {
                        mLogger.Debug("updating runningChains col");
                        runningChains[clientName][guid].completedTasks.Add(taskinfo.FlowID);
                        chainInfo = runningChains[clientName][guid];
                        chainId = chainInfo.chainId;
                        mLogger.Debug("chain id=> " + chainId);
                    }

                    //if task passed or (task has failed AND proceed on fail = true)
                    if (taskinfo.Status != null && (taskinfo.Status.Status == com.ivp.rad.RCTMUtils.TaskStatus.PASSED || (taskinfo.Status.Status == com.ivp.rad.RCTMUtils.TaskStatus.FAILED && taskinfo.ProceedOnFail == true)))
                    {
                        mLogger.Debug("fetching dependents");
                        List<int> deps = FindAllDependantsOn(taskinfo.FlowID, chainId);
                        if (deps != null && deps.Count > 0)
                        {
                            mLogger.Debug("found the following dependants>>" + string.Join(",", deps));
                            foreach (int depflowId in deps)
                            {
                                TaskInfo depcurrTask = GetTaskInfoFromFlowId(depflowId);
                                mLogger.Debug("flow id=>" + depflowId);
                                if (AllDependantsAreCompleteOrMuted(depflowId, guid, clientName) == true)
                                {
                                    if (depcurrTask.IsMuted == false)
                                    {
                                        TriggerTask(depflowId, guid, null, "System", taskinfo.triggerAsOfDateInfo.triggerDate, clientName);
                                    }
                                    else
                                    {
                                        mLogger.Debug("skipping muted task with flow id >> " + depflowId);
                                        depcurrTask.Status = new TaskStatusInfo();
                                        depcurrTask.Status.Status = com.ivp.rad.RCTMUtils.TaskStatus.PASSED;
                                        TaskCompleted(depcurrTask, guid, clientName);
                                    }
                                }
                            }
                        }
                        else
                        {//task with no dependant ie last task in chain so remove chain from running chains
                            mLogger.Debug("no  dependents found");
                            lock (((ICollection)runningChains).SyncRoot)
                                runningChains[clientName].Remove(guid);

                            string assemblyInfo = getAssemblyInfoByModuleId(taskinfo.ModuleId);
                            if (taskinfo.ChainMailSubscribeId != null && taskinfo.ChainMailSubscribeId.ContainsKey(taskinfo.Status.Status))
                            {
                                MailDetails mailDetails = taskinfo.ChainMailSubscribeId[taskinfo.Status.Status];
                                //registeredClients[taskinfo.ModuleId].notifySubscribers(assemblyInfo.Split('|')[0], assemblyInfo.Split('|')[1], mailDetails, new RCTMUtils().getTaskStatusByChainGuid(taskinfo.ChainGUID));
                            }
                        }
                    }
                    else
                    {//task failed and proceed on fail = false
                        mLogger.Debug("task failed");

                        if (taskinfo.OnFailRunTask != 0 && taskinfo.OnFailRunTask != -1)
                        {
                            mLogger.Debug("on fail run task with flow id >>" + taskinfo.OnFailRunTask);
                            lock (((ICollection)runningChains).SyncRoot)
                            {
                                runningChains[clientName][guid].completedTasks.Add(taskinfo.FlowID);
                            }
                            TriggerTask(taskinfo.OnFailRunTask, guid, null, "System", taskinfo.triggerAsOfDateInfo.triggerDate, clientName);
                        }
                        else
                        {
                            lock (((ICollection)runningChains).SyncRoot)
                            {
                                runningChains[clientName].Remove(guid);

                                string assemblyInfo = getAssemblyInfoByModuleId(taskinfo.ModuleId);
                                MailDetails mailDetails = taskinfo.ChainMailSubscribeId[taskinfo.Status.Status];
                                //registeredClients[taskinfo.ModuleId].notifySubscribers(assemblyInfo.Split('|')[0], assemblyInfo.Split('|')[1], mailDetails, new RCTMUtils().getTaskStatusByChainGuid(taskinfo.ChainGUID));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService: Error on call to Task Completed by client with task info :" + taskinfo.ToString() + " and guid :" + guid);
                mLogger.Error(ex);
            }

        }

        /// <summary>
        /// Fetches all out of sync tasks from all registered clients 
        /// ie. tasks which are inprogress in CTM and have some other status in clients
        /// </summary>
        /// <returns>List of client task status ids</returns>
        public List<int> GetUnsyncdTasksClientTaskStatusIds(string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Debug("fetching unsyncd tasks clien task status ids");
            List<int> tmp = new List<int>();
            //lock (((ICollection)registeredClients).SyncRoot)
            //{
            //    foreach (ICTMServiceCallback registeredClient in registeredClients.Values)
            //    {
            //        int registeredModuleId = -1;
            //        try
            //        {
            //            registeredModuleId = registeredClients.First(x => x.Value == registeredClient).Key;
            //            Dictionary<int, DataRow> dicIdVsDr = DALAdapter.getModuleInfoByRegisteredModuleId(registeredModuleId);
            //            foreach (int moduleId in dicIdVsDr.Keys)
            //            {
            //                mLogger.Debug("Fetching unsyncd tasks client task status id from module id " + moduleId);
            //                String assemblyInfo = getAssemblyInfoByModuleId(moduleId);
            //                List<int> tmpOutOfSyncFlowIds = registeredClient.getUnsyncdTasksClientTaskStatusIds(assemblyInfo.Split('|')[0], assemblyInfo.Split('|')[1], getInprogressClientTaskStatusId(moduleId));
            //                tmp.AddRange(tmpOutOfSyncFlowIds);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            mLogger.Error("Error while fetching unsyncd tasks from module id" + registeredModuleId + " details:" + ex.ToString());
            //        }
            //    }
            //}
            mLogger.Debug("unsyncd client task status ids: " + tmp);
            return tmp;
        }

        private List<int> getInprogressClientTaskStatusId(int moduleId)
        {
            List<int> tmp = new List<int>();
            RDBConnectionManager dbConnection = null;// RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            try
            {
                mLogger.Debug("fetching inprogress client task status id from module id : " + moduleId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("module_id", moduleId);
                DataTable dt = dbConnection.ExecuteQuery("CTM:GetInProgressTasksByModuleId", htParams).Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        tmp.Add(Convert.ToInt32(row[0]));
                    }
                }
                mLogger.Debug("unsyncd client task status id : " + tmp);
                return tmp;

            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:getInprogressClientTaskStatusId failed for module id>>" + moduleId);
                mLogger.Error(ex);
                throw;
                //  return null;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }

        }

        public List<string> GetPrivilegeList(string username,string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            List<string> tmp = new List<string>();
            lock (((ICollection)registeredClients).SyncRoot)
            {
                foreach (ICTMServiceCallback registeredClient in registeredClients[clientName].Values)
                {
                    int moduleId = -1;
                    try
                    {
                        moduleId = registeredClients[clientName].First(x => x.Value == registeredClient).Key;
                        String assemblyInfo = getAssemblyInfoByModuleId(moduleId);
                        List<string> privilegeList = registeredClient.getPrivilegeList(assemblyInfo.Split('|')[0], assemblyInfo.Split('|')[1], "CommonTaskManager", username, clientName);
                        tmp.AddRange(privilegeList);
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("Error while fetching Privilege list from module id" + moduleId + " details:" + ex.ToString());
                    }
                }
            }
            return tmp.Distinct().ToList<string>();

        }

        public Boolean RemoveTaskFromRunningChains(int taskStatusId, string guid,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);

                mLogger.Debug("removing task from running chain with task status id " + taskStatusId + " and guid : " + guid);
                int moduleId = GetModuleIdFromTaskStatusId(taskStatusId);
                string assemblyInfo = getAssemblyInfoByModuleId(moduleId);
                int clientStatusId = getClientStatusIdFromStatusId(taskStatusId);
                if (clientStatusId == -1)
                {

                    return true;
                }
                if (registeredClients[clientName][moduleId].deleteStatusFromClient(assemblyInfo.Split('|')[0], assemblyInfo.Split('|')[1], clientStatusId, clientName) == true)
                {
                    try
                    {
                        if (runningChains[clientName].ContainsKey(guid))
                        {
                            runningChains[clientName][guid].runningTasks.RemoveAll(x => x.Status.StatusId == taskStatusId);
                        }

                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("Error while removing from running tasks" + ex);
                    }
                    mLogger.Debug("task successfully removed");
                    return true;
                }
                //db entry for the status is removed from TaskStatusWebMethods whenever this function returns true
                mLogger.Error("CTMService:Error while removing task from running chains with taskStatusId:" + taskStatusId + " and guid:" + guid);
                return false;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:Exception while removing task from running chains with taskStatusId:" + taskStatusId + " and guid:" + guid); mLogger.Error(ex);
                return false;
            }

        }

        public Boolean CTMUndoTask(int taskStatusId,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);

                mLogger.Debug("undo task  with task status id " + taskStatusId);
                int moduleId = GetModuleIdFromTaskStatusId(taskStatusId);
                string assemblyInfo = getAssemblyInfoByModuleId(moduleId);
                int clientStatusId = getClientStatusIdFromStatusId(taskStatusId);
                if (clientStatusId == -1)
                {
                    mLogger.Debug("no client task status id found for task status id " + taskStatusId);
                    return true;
                }
                if (registeredClients[clientName][moduleId].UndoTask(assemblyInfo.Split('|')[0], assemblyInfo.Split('|')[1], clientStatusId, clientName) == true)
                {

                    mLogger.Debug("task undo successful");
                    return true;
                }
                mLogger.Error("CTMService:Error while undo task  with taskStatusId:" + taskStatusId);
                return false;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:Exception while undo task  with taskStatusId:" + taskStatusId);
                mLogger.Error(ex);
                return false;
            }

        }

        private int GetModuleIdFromTaskStatusId(int taskStatusId)
        {
            RDBConnectionManager dbConnection = null;// RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            try
            {
                mLogger.Debug("fetching module id from task status id");
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("task_status_id", taskStatusId);
                int moduleId = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetModuleIdByTaskStatusId", htParams).Tables[0].Rows[0][0]);
                mLogger.Debug("module id : " + moduleId);
                return moduleId;
            }
            catch (Exception ex)
            {
                mLogger.Error("Error while fetching ModuleId From TaskStatusId ");
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }

        }


        int getClientStatusIdFromStatusId(int taskStatusId)
        {
            RDBConnectionManager dbConnection = null;
            try
            {
                mLogger.Debug("fetching client status id from status id " + taskStatusId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("task_status_id", taskStatusId);
                int clientTaskStatusId = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetClientStatusIdFromStatusId", htParams).Tables[0].Rows[0][0]);
                mLogger.Debug("client task status id : " + clientTaskStatusId);
                return clientTaskStatusId;
            }
            catch (Exception ex)
            {
                mLogger.Error("Error while fetching Client status id");
                mLogger.Error(ex);
                return -1; ;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }

        }

        private bool allTasksInChainAreComplete(int chainId, List<int> list)
        {
            RDBConnectionManager dbConnection = null;// RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            try
            {
                mLogger.Debug("checking if all tasks in chain are complete");
                List<int> tmp = new List<int>();
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("chain_id", chainId);
                DataTable dt = dbConnection.ExecuteQuery("CTM:GetAllFlowIdsByChainId", htParams).Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        tmp.Add(Convert.ToInt32(row[0]));
                    }
                }

                return list.OrderBy(x => x).SequenceEqual(tmp.OrderBy(x => x));

            }
            catch (Exception ex)
            {
                mLogger.Error("exception while checking if all tasks in chain are complete : " + ex.ToString());
                return false;
            }


        }

        string getAssemblyInfoByModuleId(int moduleId)
        {
            RDBConnectionManager dbConnection = null;
            try
            {
                mLogger.Debug("fetching assembly info by module id :" + moduleId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("module_id", moduleId);
                DataTable dt = dbConnection.ExecuteQuery("CTM:ModuleByModuleId", htParams).Tables[0];
                mLogger.Debug("assembly info : " + dt.Rows[0]["Assembly_info"].ToString());
                return dt.Rows[0]["Assembly_info"].ToString();
            }
            catch (Exception ex)
            {
                mLogger.Error("Error while fetching AssemblyInfo");
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }

        }

        public Dictionary<string, List<string>> GetSubscriberList(int moduleId,string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Debug("CTMService : GetEmailIds -> Start");
            Dictionary<string, List<string>> lstEmailId = new Dictionary<string, List<string>>();
            try
            {
                RUserManagementService objUserController = new RUserManagementService();
                List<RUserInfo> dsUsers = objUserController.GetAllUsersGDPR();
                if (dsUsers != null && dsUsers.Count > 0)
                    lstEmailId = dsUsers.Where(r => !string.IsNullOrEmpty(r.EmailId)).GroupBy(y => y.EmailId).ToDictionary(a => a.Key, t => t.Select(x => x.FullName).ToList());

                //lstEmailId = dsUsers.Tables[1].AsEnumerable().Select(x => x.Field<string>("email_id")).ToList();
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService : GetEmailIds -> Error " + ex.Message);
                throw new Exception(ex.Message, ex);
            }
            mLogger.Debug("CTMService : GetEmailIds -> End");
            return lstEmailId;
        }

        public List<string> GetCalendarList(int moduleId,string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            try
            {
                //mLogger.Debug("Fetching calendar list with module id "+ moduleId);
                //dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                //htParams = new RHashlist();
                //htParams.Add("module_id", moduleId);
                //DataTable dt = dbConnection.ExecuteQuery("CTM:GetRegisteredModuleByModuleId", htParams).Tables[0];
                String AssemblyInfo = getAssemblyInfoByModuleId(moduleId);
                int registeredModuleId = DALAdapter.GetRegisteredModuleIdByModuleId(moduleId);
                return registeredClients[clientName][registeredModuleId].getCalendarList(AssemblyInfo.Split('|')[0], AssemblyInfo.Split('|')[1], clientName);
            }
            catch (CommunicationObjectAbortedException ex)
            {
                mLogger.Error("CommunicationObjectAbortedException Error while fetching calendar list from client:" + ex.ToString());
                lock (((IDictionary)registeredClients).SyncRoot)
                {
                    registeredClients[clientName].Remove(moduleId);
                }
                mLogger.Error(ex);
                throw;


            }
            catch (Exception ex)
            {
                mLogger.Error("Error while fetching calendar list from client");
                mLogger.Error(ex);
                return new List<string>();
                //throw;

            }
        }

        internal void SendSubscriptionMail(string mailTo, string mailFrom, string mailCc, string mailBody, string mailSubject)
        {
            mLogger.Debug("CTM: SendSubscriptionMail -> Start");
            mLogger.Debug("CTM SendSubscriptionMail -> To: " + mailTo + " FROM : " + mailFrom + " mailCC : " + (mailCc ?? string.Empty) + " mailBody : " + mailBody + " mailSubject : " + mailSubject);
            try
            {
                RMailContent mailContent = new RMailContent();
                Dictionary<string, string> dictNotificationInfo = new Dictionary<string, string>();

                IEnumerable<XElement> ixe = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + @"\ConfigFiles\CTMSubscription.xml").Element("taskmanagernotification").Elements();
                foreach (XElement xe in ixe)
                {
                    if (!dictNotificationInfo.ContainsKey(xe.Name.ToString()))
                        dictNotificationInfo.Add(xe.Attribute("name").Value.ToString(), xe.Attribute("value").Value.ToString());
                }

                mailContent.To = mailTo;
                mailContent.From = mailFrom;
                if (mailCc != null)
                    mailContent.CC = mailCc;
                mailContent.Subject = mailSubject;
                mailContent.Body = mailBody;
                mailContent.IsBodyHTML = true;

                if (mailContent.To.Length > 0)
                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        try
                        {
                            IRTransport transConfig = new RTransportManager().GetTransport(dictNotificationInfo["TransportName"]);
                            transConfig.SendMessage(mailContent);
                        }
                        catch (Exception ex)
                        {
                            mLogger.Error(ex.ToString());
                        }
                    });
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new Exception(ex.Message, ex);
            }
            finally { mLogger.Debug("CTM: SendSubscriptionMail -> End"); }
        }

        private bool IsWaiting(TaskInfo info, string guid, string environmentVariables, string username, DateTime? as_of_date, int statusId, DateTime startTime, string clientName,out bool failTask, bool requireLock = true, bool forceWait = false)
        {
            SRMMTConfig.SetClientName(clientName);

            bool returnValue = false;
            bool updateWaitingQueue = false;
            DateTime now = DateTime.Now;
            failTask = false;

            mLogger.Error("IsWaiting -----> statusId :" + statusId);
            if (forceWait)
            {
                mLogger.Error("IsWaiting -----> Forced Waiting statusId :" + statusId);
                lock (((ICollection)RCTMDic.WaitingQueue).SyncRoot)
                {
                    RCTMQueuedTaskInfo tinfo = new RCTMQueuedTaskInfo { AsOfDate = as_of_date, ChainGuid = guid, EnvironmentVariables = environmentVariables, FlowId = info.FlowID, Subscribers = info.Subscribers, TaskName = info.TaskName, UserName = username, CTMStatusId = statusId, StartTime = startTime };
                    DateTime temp = now.AddHours(12);

                    if (!RCTMDic.WaitingQueue[clientName].ContainsKey(info.TaskSummaryId))
                        RCTMDic.WaitingQueue[clientName].Add(info.TaskSummaryId, new SortedDictionary<DateTime, RCTMQueuedTaskInfo>());
                    while (RCTMDic.WaitingQueue[clientName][info.TaskSummaryId].ContainsKey(temp))
                        temp = temp.AddMilliseconds(5);
                    tinfo.EndTime = temp;
                    RCTMDic.WaitingQueue[clientName][info.TaskSummaryId][temp] = tinfo;

                    mLogger.Error("IsWaiting -----> statusId :" + statusId + " added in WaitingQueue as forcewait");

                    returnValue = true;
                }
            }
            else
            {
                if (requireLock)
                {
                    mLogger.Error("IsWaiting -----> Trying Lock on RunningTasks.");
                    lock (((ICollection)RCTMDic.RunningTasks).SyncRoot)
                    {
                        mLogger.Error("IsWaiting -----> Acquired Lock on RunningTasks.");
                        mLogger.Error("RunningTasks ---- >" + string.Join(",", RCTMDic.RunningTasks[clientName].Keys) + "  Check for TaskSummaryId ----> " + info.TaskSummaryId);

                        if (!RCTMDic.RunningTasks[clientName].ContainsKey(info.TaskSummaryId))
                        {
                            mLogger.Error("IsWaiting -----> statusId :" + statusId + " is free to run");
                        }
                        else if (info.SecondInstanceWait > 0)
                            updateWaitingQueue = true;
                        else
                        {
                            failTask = true;
                            mLogger.Error("IsWaiting -----> statusId :" + statusId + " failed as no second instance wait.");
                        }
                    }
                }
                else
                {
                    if (!RCTMDic.RunningTasks[clientName].ContainsKey(info.TaskSummaryId))
                    {
                        mLogger.Error("IsWaiting -----> statusId :" + statusId + " is free to run");
                    }
                    else if (info.SecondInstanceWait > 0)
                        updateWaitingQueue = true;
                    else
                    {
                        failTask = true;
                        mLogger.Error("IsWaiting -----> statusId :" + statusId + " failed as no second instance wait.");
                    }
                }
                if (updateWaitingQueue)
                {
                    mLogger.Error("IsWaiting -----> SecOnd Isntance wait > 0.");
                    mLogger.Error("IsWaiting -----> Trying Lock on WaitingQueue.");
                    lock (((ICollection)RCTMDic.WaitingQueue).SyncRoot)
                    {
                        mLogger.Error("IsWaiting -----> Acquired Lock on WaitingQueue.");
                        RCTMQueuedTaskInfo tinfo = new RCTMQueuedTaskInfo { AsOfDate = as_of_date, ChainGuid = guid, EnvironmentVariables = environmentVariables, FlowId = info.FlowID, Subscribers = info.Subscribers, TaskName = info.TaskName, UserName = username, CTMStatusId = statusId, StartTime = startTime };
                        DateTime temp = now.AddMinutes(info.SecondInstanceWait);
                       
                        if (!RCTMDic.WaitingQueue[clientName].ContainsKey(info.TaskSummaryId))
                            RCTMDic.WaitingQueue[clientName].Add(info.TaskSummaryId, new SortedDictionary<DateTime, RCTMQueuedTaskInfo>());
                        while (RCTMDic.WaitingQueue[clientName][info.TaskSummaryId].ContainsKey(temp))
                            temp = temp.AddMilliseconds(5);
                        tinfo.EndTime = temp;
                        RCTMDic.WaitingQueue[clientName][info.TaskSummaryId][temp] = tinfo;
                        mLogger.Error("IsWaiting -----> statusId :" + statusId + " added in WaitingQueue as summaryid : " + info.TaskSummaryId + " already running.");
                        returnValue = true;
                    }
                }
            }

            return returnValue;
        }


        private Dictionary<string, string> deserializeJson(string jsonString)
        {
            try
            {
                mLogger.Debug("deserializingJSON with string" + jsonString);
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                //Dictionary<string, string> tmp = new Dictionary<string, string>();
                //jsonString = jsonString.Substring(1, jsonString.Length - 2);
                //foreach (string str in jsonString.Split(','))
                //{
                //    tmp[str.Split(':')[0]] = str.Split(':')[1];
                //}
                //mLogger.Debug("username in deserializing" + tmp["username"]);
                //return tmp;
                return values;
            }
            catch (Exception ex)
            {
                mLogger.Error("exception while deserialising JSON" + ex.ToString());
                Dictionary<string, string> tmp2 = new Dictionary<string, string>();
                tmp2["username"] = "userfetchfailed";
                return tmp2;
            }
        }

        private int AddTaskStatus(int flowId, string guid, string username, out DateTime startTime)
        {
            startTime = DateTime.Now;
            RDBConnectionManager dbConnection = null;// RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            try
            {
                mLogger.Debug("Adding Task status with flow id : " + flowId + " and guid" + guid);
                var dic = new Dictionary<string, string>(); dic.Add("username", username);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("flow_id", flowId);
                htParams.Add("start_time", startTime);
                htParams.Add("end_time", null);//htParams.Add("end_time", DateTime.Now);
                htParams.Add("status", "INPROGRESS");
                htParams.Add("chain_guid", guid);
                //htParams.Add("environment_variables", JsonConvert.SerializeObject(dic));
                return Convert.ToInt32(dbConnection.ExecuteQuery("CTM:AddTaskStatus", htParams).Tables[0].Rows[0][0]);
            }
            catch (Exception ex)
            {
                mLogger.Error("RCTMService: AddTaskStatus>>Error while adding task status with flowId:" + flowId + " and guid:" + guid);
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
        }

        public void TriggerScheduledChain(int scheduledJobID,string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            RDBConnectionManager dbConnection = null;// = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            mLogger.Debug("CTMService:TriggerScheduledChain>> triggerring scheduled chain with scheduledJobId:" + scheduledJobID);
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("scheduled_job_id", scheduledJobID);
                TriggerChain(Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetChainIdByScheduledJobId", htParams).Tables[0].Rows[0][0]), true, "System", DateTime.Now, clientName);
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:TrifferScheduledChain>>Error while trying to trigger scheduled chain with scheduled job id:" + scheduledJobID);
                mLogger.Error(ex);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
        }

        public void TriggerChainByFilewatcher(int chainId, string environmentVar, string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            mLogger.Debug("CTMService:TriggerChainByFilewatcher>> chainId:" + chainId + " environment variables" + environmentVar);
            try
            {
                if (isLessThanMaxParallelRun(chainId))
                {
                    Guid guid = Guid.NewGuid();
                    string guidString = guid.ToString();
                    lock (((ICollection)runningChains).SyncRoot)
                        runningChains[clientName][guidString] = new chainInfo(chainId);// { chainId = chainId };
                    int flowId = FindFlowIdOfHead(chainId);
                    if (IsMuted(flowId))
                    {
                        lock (((ICollection)runningChains).SyncRoot)
                            runningChains[clientName].Remove(guidString);
                    }
                    else
                        TriggerTask(flowId, guid.ToString(), null, "System", DateTime.Now, clientName);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:TriggerChainByFilewatcher>>Error while trying to trigger chain by filewatcher with chain id: " + chainId + " and environment variables : " + environmentVar);
                mLogger.Error(ex);
            }

        }

        private bool isLessThanMaxParallelRun(int chainId)
        {
            RDBConnectionManager dbConnection = null;
            mLogger.Debug("CTMService:CheckForMaxParallelRun>>Checking for max parrallel run limit for chain with chain id" + chainId);
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("chain_id", chainId);
                chainInfo tmpChain = new chainInfo(chainId);
                DataSet ds = dbConnection.ExecuteQuery("CTM:GetMaxParallelRunByChainId", htParams);

                int numberOfRunningChain = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetMaxParallelRunByChainId", htParams).Tables[0].Rows[0][0]);
                mLogger.Debug("number of running chains => " + numberOfRunningChain);
                mLogger.Debug("max chain parallel allowed => " + tmpChain.max_parallel_instances_allowed);
                if (numberOfRunningChain == 0)
                    return true;
                if (tmpChain.max_parallel_instances_allowed == 0)
                {
                    if (numberOfRunningChain >= 1)
                        return false;
                    else
                        return true;
                }
                else if (tmpChain.max_parallel_instances_allowed > 0)
                {
                    if (numberOfRunningChain >= tmpChain.max_parallel_instances_allowed)
                        return false;
                    else
                        return true;
                }
                return true;
                //}
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:Error while checking for max parallel run on chain with chain id:" + chainId);
                mLogger.Error(ex);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
            return true;
        }

        private void UpdateTaskStatus(TaskStatusInfo taskStatusInfo, int taskSummaryId, string clientName,RDBConnectionManager dbConnection = null)
        {
            SRMMTConfig.SetClientName(clientName);

            bool gotConnection = dbConnection == null ? false : true;
            mLogger.Debug("Updating task status with task status info " + taskStatusInfo);
            dbConnection = dbConnection ?? RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            try
            {
                lock (((ICollection)RCTMDic.RunningTasks).SyncRoot)
                {
                    List<int> ts = new List<int>();
                    foreach (var rtask in RCTMDic.RunningTasks[clientName])
                    {
                        foreach (var statId in rtask.Value.ToArray())
                        {
                            if (statId == taskStatusInfo.StatusId)
                            {
                                rtask.Value.Remove(taskStatusInfo.StatusId);
                                mLogger.Error("CTMService: UpdateTaskStatus -> Removing from Running Task (Status Id):" + taskStatusInfo.StatusId);
                            }
                        }
                        if (rtask.Value.Count == 0)
                            ts.Add(rtask.Key);
                    }
                    foreach (var tsId in ts)
                    {
                        RCTMDic.RunningTasks[clientName].Remove(tsId);
                        mLogger.Error("CTMService: UpdateTaskStatus -> Remove in Running Task (Task Summary Id):" + tsId);
                    }
                    mLogger.Error("CTMService: UpdateTaskStatus -> Remove in Running Task :" + taskSummaryId);
                }

                RHashlist htParams = new RHashlist();
                htParams.Add("task_status_id", taskStatusInfo.StatusId);
                htParams.Add("status", taskStatusInfo.Status);
                htParams.Add("end_time", DateTime.Now);
                htParams.Add("log_description", taskStatusInfo.TaskLog);
                htParams.Add("environment_variables", taskStatusInfo.environmentVariables);
                htParams.Add("client_task_status_id", taskStatusInfo.clientStatusId);
                dbConnection.ExecuteQuery("CTM:UpdateTaskStatus", htParams);

            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:UpdateTaskStatus>>Error while trying to update task status : " + taskStatusInfo.ToString());
                mLogger.Error(ex);
                throw ex;
            }
            finally
            {
                if (!gotConnection)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
        }

        private void UpdateAdditionalInfoForTask(string additionalInfo, int taskStatusId)
        {
            RDBConnectionManager dbConnection = null;
            mLogger.Debug("Updating addtional info for task with task_status_id : " + taskStatusId);
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            try
            {
                if (!string.IsNullOrEmpty(additionalInfo))
                    dbConnection.ExecuteQuery("UPDATE dbo.ivp_rad_task_status SET additional_info = '" + additionalInfo.Replace(@"\", @"\\").Replace("'", "\"") + "' WHERE task_status_id = " + taskStatusId + ";", RQueryType.Update);
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:UpdateTaskStatus>>Error while trying to update additional info for task with task_status_id : " + taskStatusId);
                mLogger.Error(ex);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
        }

        private bool AllDependantsAreCompleteOrMuted(int flowId, string guid,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);

                mLogger.Debug("Checking if all dependants are completed or muted");
                var deps = FindAllDependantsOf(flowId);
                foreach (var dep in deps)
                {
                    chainInfo chainInfo = null;
                    lock (((ICollection)runningChains).SyncRoot)
                        chainInfo = runningChains[clientName][guid];
                    if (!dep.Contains('O'))
                    {
                        if (chainInfo.completedTasks.Contains(Convert.ToInt32(dep.Substring(1))) == false && IsMuted(Convert.ToInt32(dep.Substring(1))) == false)
                        {
                            mLogger.Debug("All dependants are complete : false");
                            return false;
                        }
                    }
                }
                mLogger.Debug("All dependants are complete : true");
                return true;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:AllDependantsAreCompletedOrMuted>>Error while checking all dependants are completed or muted with flowId:" + flowId + " and guid:" + guid);
                mLogger.Error(ex);
                return false;
            }
        }

        private bool AllDependantsAreComplete(int flowId, string guid,string clientName)
        {
            try
            {
                SRMMTConfig.SetClientName(clientName);

                mLogger.Debug("Checking if all dependants are completed");
                bool flag = true;
                var deps = FindAllDependantsOf(flowId);
                foreach (var dep in deps)
                {
                    chainInfo chainInfo = null;
                    lock (((ICollection)runningChains).SyncRoot)
                        chainInfo = runningChains[clientName][guid];
                    //if (chainInfo.completedTasks.Contains(Convert.ToInt32(dep.Substring(1))) == false && runningChains[guid].IsService == true) { flag = false; break; }
                    if (chainInfo.completedTasks.Contains(Convert.ToInt32(dep.Substring(1))) == false) { flag = false; break; }
                }
                mLogger.Debug("All dependants are complete : " + flag);
                return flag;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:AllDependantsAreCompleted>>Error while checking all dependants are completed with flowId:" + flowId + " and guid:" + guid);
                mLogger.Error(ex);
                return false;
            }
        }

        private bool IsMuted(int flowId)
        {
            RDBConnectionManager dbConnection = null;
            try
            {
                mLogger.Debug("checking is muted with flowid: " + flowId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("flow_id", flowId);
                Boolean isMuted = Convert.ToBoolean(dbConnection.ExecuteQuery("CTM:GetIsMutedFromFLowByFlowId", htParams).Tables[0].Rows[0][0]);
                mLogger.Debug("ismuted :" + isMuted);
                return isMuted;

            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:IsMuted>>Error while checking is muted for task with flow id:" + flowId);
                mLogger.Error(ex);
                return false;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
        }

        private List<string> FindAllDependantsOf(int flowId)
        {
            RDBConnectionManager dbConnection = null;
            try
            {
                mLogger.Debug("finding all dependants of flowid: " + flowId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("flow_id", flowId);
                List<string> dependants = dbConnection.ExecuteQuery("CTM:GetDependentOnIdByFlowId", htParams).Tables[0].Rows[0][0].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                mLogger.Debug("found dependants : " + dependants);
                return dependants;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:FindAllDependantsOf>>Error while finding all dependants of task with flow id:" + flowId);
                mLogger.Error(ex);
                return null;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }

        }

        private List<int> FindAllDependantsOn(int flowId, int chainId)
        {
            RDBConnectionManager dbConnection = null;
            try
            {
                mLogger.Debug("checking dependents for flow id=> " + flowId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("chain_id", chainId);
                DataTable st = dbConnection.ExecuteQuery("CTM:GetAllDependentsOn", htParams).Tables[0];

                List<int> dependantsOn = new List<int>();
                foreach (DataRow rdr in st.Rows)
                {
                    int tmpFLowId = Convert.ToInt32(rdr[0]);
                    string[] tmpDepArr = (rdr[1].ToString()).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string tmpDep in tmpDepArr)
                    {
                        if (Convert.ToInt32(Regex.Replace(tmpDep, @"[&O]", "")) == flowId)
                        {
                            dependantsOn.Add(tmpFLowId);
                            break;

                        }

                    }
                }
                mLogger.Debug("completed checking dependents for flow id=> " + flowId + " dependants: " + dependantsOn);
                return dependantsOn;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:FindAllDependantsOn>>Error while finding all dependants on task with flow id:" + flowId + " and chain id:" + chainId);
                mLogger.Error(ex);
                return null;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
        }

        private TaskInfo GetTaskInfoFromFlowId(int flowId)
        {
            RDBConnectionManager dbConnection = null;
            try
            {
                string taskManagerSubscribers = SRMCommon.GetConfigFromDB("TaskManagerSubscribers");

                mLogger.Debug("Fetching task info from flow id " + flowId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("flow_id", flowId);
                DataRow rdr = dbConnection.ExecuteQuery("CTM:GetTaskInfoByFlowId", htParams).Tables[0].Rows[0];
                DateTime? triggerAsOfDate;
                if (rdr["trigger_as_of_date"] != DBNull.Value) { triggerAsOfDate = Convert.ToDateTime(rdr["trigger_as_of_date"]); } else { triggerAsOfDate = null; }
                string taskSubscribers = Convert.ToString(rdr["task_wait_subscription_id"]);
                string taskSecondInstanceWait = Convert.ToString(rdr["task_second_instance_wait"]);
                string taskTimeOut = Convert.ToString(rdr["task_time_out"]);
                //string chainSubscribers = Convert.ToString(rdr["inprogress_subscibers"]);

                TaskInfo taskInfo = new TaskInfo()
                {
                    FlowID = rdr["flow_id"] != DBNull.Value ? Convert.ToInt32(rdr["flow_id"]) : 0,
                    DependantOnId = rdr["dependent_on_id"] != DBNull.Value ? rdr["dependent_on_id"] : null,
                    IsActive = rdr["is_active"] != DBNull.Value ? Convert.ToBoolean(rdr["is_active"]) : false,
                    ChainId = rdr["chain_id"] != DBNull.Value ? Convert.ToInt32(rdr["chain_id"]) : 0,
                    TimeOut = rdr["timeout"] != DBNull.Value ? Convert.ToInt32(rdr["timeout"]) : 0,
                    ProceedOnFail = rdr["proceed_on_fail"] != DBNull.Value ? Convert.ToBoolean(rdr["proceed_on_fail"]) : false,
                    IsMuted = rdr["is_muted"] != DBNull.Value ? Convert.ToBoolean(rdr["is_muted"]) : false,
                    IsReRunOnFail = rdr["rerun_on_fail"] != DBNull.Value ? Convert.ToBoolean(rdr["rerun_on_fail"]) : false,
                    RetryInterval = rdr["fail_retry_duration"] != DBNull.Value ? Convert.ToInt32(rdr["fail_retry_duration"]) : 0,
                    RetryCount = rdr["fail_number_retry"] != DBNull.Value ? Convert.ToInt32(rdr["fail_number_retry"]) : 0,
                    OnFailRunTask = rdr["on_fail_run_task"] != DBNull.Value ? Convert.ToInt32(rdr["on_fail_run_task"]) : 0,
                    ModuleId = rdr["module_id"] != DBNull.Value ? Convert.ToInt32(rdr["module_id"]) : 0,
                    TaskName = rdr["task_name"] != DBNull.Value ? rdr["task_name"].ToString() : null,
                    TaskTypeName = rdr["task_type_name"] != DBNull.Value ? rdr["task_type_name"].ToString() : null,
                    ModuleName = rdr["module_name"] != DBNull.Value ? rdr["module_name"].ToString() : null,
                    IsUndoSupported = rdr["undo_supported"] != DBNull.Value ? rdr["undo_supported"].ToString() : null,
                    TaskSummaryId = rdr["task_summary_id"] != DBNull.Value ? Convert.ToInt32(rdr["task_summary_id"]) : 0,
                    CalendarName = rdr["calendar_name"] != DBNull.Value ? rdr["calendar_name"].ToString() : null,
                    TaskMasterId = rdr["task_master_id"] != DBNull.Value ? Convert.ToInt32(rdr["task_master_id"]) : 0,
                    RegisteredModuleId = rdr["registered_module_id"] != DBNull.Value ? Convert.ToInt32(rdr["registered_module_id"]) : 0,
                    ChainName = rdr["chain_name"] != DBNull.Value ? rdr["chain_name"].ToString() : null,
                    triggerAsOfDateInfo = new TriggerAsOfDateInfo()
                    {
                        triggerDate = rdr["trigger_as_of_date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(rdr["trigger_as_of_date"]) : null,
                        customValue = rdr["trigger_as_of_date_info"] != DBNull.Value ? (rdr["trigger_as_of_date_info"]).ToString() : null
                    },
                    // triggerAsOfDate, rdr["trigger_as_of_date_info"] != DBNull.Value ? rdr["trigger_as_of_date_info"].ToString() : null)

                    Subscribers = !string.IsNullOrEmpty(taskSubscribers) ? taskSubscribers.Split('|') : (string.IsNullOrEmpty(taskManagerSubscribers) ? new string[0] : taskManagerSubscribers.Split('|')),
                };

                if (!string.IsNullOrEmpty(taskSecondInstanceWait) && Convert.ToInt32(taskSecondInstanceWait) > 0)
                    taskInfo.SecondInstanceWait = Convert.ToInt32(taskSecondInstanceWait);
                else
                    taskInfo.SecondInstanceWait = 719;

                if (!string.IsNullOrEmpty(taskTimeOut) && Convert.ToInt32(taskTimeOut) > 0)
                    taskInfo.TaskEndTime = DateTime.Now.AddMinutes(Convert.ToInt32(taskTimeOut));
                else
                    taskInfo.TaskEndTime = null;

                taskInfo.MailSubscribeId = new Dictionary<rad.RCTMUtils.TaskStatus, MailDetails>();
                taskInfo.ChainMailSubscribeId = new Dictionary<rad.RCTMUtils.TaskStatus, MailDetails>();
                mLogger.Debug("fetching assembly info");
                String assemblyInfo = getAssemblyInfoByModuleId(Convert.ToInt32(rdr["module_id"]));
                //if (rdr["assembly_info"] != DBNull.Value)
                //{
                //    if (rdr["assembly_info"].ToString().Split(new char[] { '|' })[0] != null)
                //    {
                //        taskInfo.AssemblyLocation = rdr["assembly_info"].ToString().Split(new char[] { '|' })[0];
                //    }
                //    else { taskInfo.AssemblyLocation = ""; }
                //    if (rdr["assembly_info"].ToString().Split(new char[] { '|' }).Length >= 2 && rdr["assembly_info"].ToString().Split(new char[] { '|' })[1] != null)
                //    {
                //        taskInfo.ClassName = rdr["assembly_info"].ToString().Split(new char[] { '|' })[1];
                //    }

                //}
                if (String.IsNullOrEmpty(assemblyInfo) == false)
                {
                    if (assemblyInfo.ToString().Split(new char[] { '|' })[0] != null)
                    {
                        taskInfo.AssemblyLocation = assemblyInfo.ToString().Split(new char[] { '|' })[0];
                    }
                    else { taskInfo.AssemblyLocation = ""; }
                    if (assemblyInfo.ToString().Split(new char[] { '|' }).Length >= 2 && assemblyInfo.ToString().Split(new char[] { '|' })[1] != null)
                    {
                        taskInfo.ClassName = assemblyInfo.ToString().Split(new char[] { '|' })[1];
                    }

                }


                mLogger.Debug("fetching subscribtion info");
                string[] arr = null;
                string subscribeId = Convert.ToString(rdr["subscribe_id"]);
                if (!string.IsNullOrEmpty(subscribeId))
                {
                    arr = subscribeId.Split('|');
                    if (arr.Length > 0)
                    {
                        MailDetails mailDetail = new MailDetails();
                        mailDetail.MailIds = !string.IsNullOrEmpty(arr[0]) ? (arr[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>()) : new List<string>();
                        if (arr.Length >= 3 && !string.IsNullOrEmpty(arr[2]))
                        {
                            mailDetail.MailSubject = System.Web.HttpUtility.UrlDecode(arr[2]);
                        }
                        if (arr.Length >= 4 && !string.IsNullOrEmpty(arr[3]))
                        {
                            mailDetail.mailBody = System.Web.HttpUtility.UrlDecode(arr[3]);
                        }
                        taskInfo.MailSubscribeId[com.ivp.rad.RCTMUtils.TaskStatus.PASSED] = mailDetail;

                    }
                    if (arr.Length > 1)
                    {
                        MailDetails mailDetail = new MailDetails();
                        mailDetail.MailIds = !string.IsNullOrEmpty(arr[1]) ? (arr[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>()) : new List<string>();
                        if (arr.Length >= 5 && !string.IsNullOrEmpty(arr[4]))
                        {
                            mailDetail.MailSubject = System.Web.HttpUtility.UrlDecode(arr[4]);
                        }
                        if (arr.Length >= 6 && !string.IsNullOrEmpty(arr[5]))
                        {
                            mailDetail.mailBody = System.Web.HttpUtility.UrlDecode(arr[5]);
                        }
                        taskInfo.MailSubscribeId[com.ivp.rad.RCTMUtils.TaskStatus.FAILED] = mailDetail;
                    }
                }

                mLogger.Debug("fetching chain subscribe id");
                subscribeId = Convert.ToString(rdr["chain_subscribe_id"]);
                if (!string.IsNullOrEmpty(subscribeId))
                {
                    arr = subscribeId.Split('|');
                    if (arr.Length > 0)
                    {
                        MailDetails mailDetail = new MailDetails();
                        mailDetail.MailIds = !string.IsNullOrEmpty(arr[0]) ? (arr[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>()) : new List<string>();
                        if (arr.Length >= 3 && !string.IsNullOrEmpty(arr[2]))
                        {
                            mailDetail.MailSubject = System.Web.HttpUtility.UrlDecode(arr[2]);
                        }
                        if (arr.Length >= 4 && !string.IsNullOrEmpty(arr[3]))
                        {
                            mailDetail.mailBody = System.Web.HttpUtility.UrlDecode(arr[3]);
                        }
                        taskInfo.ChainMailSubscribeId[com.ivp.rad.RCTMUtils.TaskStatus.PASSED] = mailDetail;

                    }
                    if (arr[1] != null)
                    {
                        MailDetails mailDetail = new MailDetails();
                        mailDetail.MailIds = !string.IsNullOrEmpty(arr[1]) ? (arr[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>()) : new List<string>();
                        if (arr.Length >= 5 && !string.IsNullOrEmpty(arr[4]))
                        {
                            mailDetail.MailSubject = System.Web.HttpUtility.UrlDecode(arr[4]);
                        }
                        if (arr.Length >= 6 && !string.IsNullOrEmpty(arr[5]))
                        {
                            mailDetail.mailBody = System.Web.HttpUtility.UrlDecode(arr[5]);
                        }
                        taskInfo.ChainMailSubscribeId[com.ivp.rad.RCTMUtils.TaskStatus.FAILED] = mailDetail;
                    }
                }

                mLogger.Debug("task info fetched: " + taskInfo);
                return taskInfo;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:GetTaskInfoFromFlowId>>Error while getting task info with flow id" + flowId);
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
        }

        private int FindFlowIdOfHead(int chainId)
        {
            RDBConnectionManager dbConnection = null;
            try
            {
                mLogger.Debug("finding flow id of head with chain id : " + chainId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("chain_id", chainId);
                int flowIdHead = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetHeadOfChainInFlowsByChainId", htParams).Tables[0].Rows[0][0]);
                mLogger.Debug("flow id of head :" + flowIdHead);
                return flowIdHead;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:FindFlowIdOfHead>>Error while finding flowId of first task in chain with chain id:" + chainId);
                mLogger.Error(ex);
                return -1;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }

        }
        /// <summary>
        /// Returns moduleId not registered moduleId by flow id
        /// </summary>
        /// <param name="flowId">Flow Id of task</param>
        /// <returns>module id of the selected task</returns>
        private int GetModuleIdByFlowId(int flowId)
        {
            RDBConnectionManager dbConnection = null;
            try
            {
                mLogger.Debug("fetching module id by flow id " + flowId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("flow_id", flowId);
                int moduleId = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetModuleIdByFlowId", htParams).Tables[0].Rows[0][0]);
                mLogger.Debug("moduleId :" + moduleId);
                return moduleId;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:GetModuleIdByFlowId>>Error while trying to fetch moduleId with flowId:" + flowId);
                mLogger.Error(ex);
                return -1;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }

        }

        /// <summary>
        /// Returns registeredmoduleId not moduleId by flow id
        /// </summary>
        /// <param name="flowId">Flow Id of task</param>
        /// <returns>module id of the selected task</returns>
        private int GetRegisteredModuleIdByFlowId(int flowId)
        {
            RDBConnectionManager dbConnection = null;
            try
            {
                mLogger.Debug("fetching module id by flow id " + flowId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("flow_id", flowId);
                int registeredModuleId = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetRegisteredModuleIdByFlowId", htParams).Tables[0].Rows[0][0]);
                mLogger.Debug("moduleId :" + registeredModuleId);
                return registeredModuleId;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:GetModuleIdByFlowId>>Error while trying to fetch moduleId with flowId:" + flowId);
                mLogger.Error(ex);
                return -1;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }

        }

        public void flowAdded(List<int> task_summary_ids,string clientName)
        {
            //moduleId,List of client_task_master_id
            SRMMTConfig.SetClientName(clientName);

            Dictionary<int, List<int>> flowsAdded = new Dictionary<int, List<int>>();
            foreach (int taskSummaryId in task_summary_ids)
            {
                try
                {
                    KeyValuePair<int, int> ModuleIdClientTaskMasterId = DALAdapter.getModuleIdClientTaskMasterIdKeyValueByTaskSummaryId(taskSummaryId);
                    if (!flowsAdded.ContainsKey(ModuleIdClientTaskMasterId.Key))
                        flowsAdded.Add(ModuleIdClientTaskMasterId.Key, new List<int>());
                    flowsAdded[ModuleIdClientTaskMasterId.Key].Add(ModuleIdClientTaskMasterId.Value);
                }
                catch (Exception ex)
                {
                    mLogger.Error("Error in flow added while converting " + ex.ToString());
                }
            }

            foreach (int moduleId in flowsAdded.Keys)
            {
                try
                {
                    string assemblyInfo = getAssemblyInfoByModuleId(moduleId);
                    int registeredModuleId = DALAdapter.GetRegisteredModuleIdByModuleId(moduleId);
                    mLogger.Debug("flow added on module id = " + moduleId + "with client task master id : " + flowsAdded[moduleId]);
                    registeredClients[clientName][registeredModuleId].flowsAdded(assemblyInfo.Split('|')[0], assemblyInfo.Split('|')[1], flowsAdded[moduleId], clientName);
                }
                catch (Exception ex)
                {
                    mLogger.Error("Error in flow added " + ex.ToString());
                }
            }
        }

        public void flowDeleted(List<int> task_summary_ids,string clientName)
        {
            SRMMTConfig.SetClientName(clientName);

            //moduleId,List of client_task_master_id
            Dictionary<int, List<int>> flowsDeleted = new Dictionary<int, List<int>>();
            foreach (int taskSummaryId in task_summary_ids)
            {
                KeyValuePair<int, int> ModuleIdClientTaskMasterId = DALAdapter.getModuleIdClientTaskMasterIdKeyValueByTaskSummaryId(taskSummaryId);
                if (!flowsDeleted.ContainsKey(ModuleIdClientTaskMasterId.Key))
                    flowsDeleted.Add(ModuleIdClientTaskMasterId.Key, new List<int>());
                flowsDeleted[ModuleIdClientTaskMasterId.Key].Add(ModuleIdClientTaskMasterId.Value);
            }

            foreach (int moduleId in flowsDeleted.Keys)
            {
                try
                {
                    string assemblyInfo = getAssemblyInfoByModuleId(moduleId);
                    int registeredModuleId = DALAdapter.GetRegisteredModuleIdByModuleId(moduleId);
                    registeredClients[clientName][registeredModuleId].flowsDeleted(assemblyInfo.Split('|')[0], assemblyInfo.Split('|')[1], flowsDeleted[moduleId], clientName);
                }
                catch (Exception ex)
                {
                    mLogger.Error("Error in flow deleted " + ex.ToString());
                }
            }
        }

    }
}