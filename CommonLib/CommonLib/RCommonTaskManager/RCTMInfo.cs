using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using com.ivp.rad.dal;
using com.ivp.rad.RCTMUtils;
using com.ivp.rad.utils;
using System.Data;
using com.ivp.rad.common;
using System.ServiceModel;
using System.Collections.Concurrent;

namespace com.ivp.rad.RCommonTaskManager
{
    public class RCTMQueuedTaskInfo
    {
        public int FlowId { get; set; }
        public string ChainGuid { get; set; }
        public string EnvironmentVariables { get; set; }
        public string UserName { get; set; }
        public DateTime? AsOfDate { get; set; }
        public DateTime EndTime { get; set; }
        public int CTMStatusId { get; set; }
        public string[] Subscribers { get; set; }
        public string TaskName { get; set; }
        public DateTime StartTime { get; set; }
        public int TaskSummaryId { get; set; }
    }

    public class RCTMRunningTaskInfo
    {
        public int TaskSummaryId { get; set; }
    }

    [ServiceContract(CallbackContract = typeof(ICTMServiceCallback), SessionMode = SessionMode.Required)]
    public interface ICTMService
    {

        [OperationContract]
        string clientKeepAlive();

        [OperationContract(IsOneWay = true)]
        void RegisterClient(int moduleId,string clientName);

        [OperationContract]
        string TriggerChain(int chainId, bool isservice, string username, DateTime? asOfDate, string clientName);

        [OperationContract(IsOneWay = false)]
        string TriggerTaskInChain(int chainId, int FlowId, string username, DateTime? asOfDate, string clientName, string passedGuid = null);

        [OperationContract(IsOneWay = false)]
        string TriggerTaskInChainFromExternalSystem(int chainId, int FlowId, string username, DateTime? asOfDate, string clientName, string passedGuid = null);

        [OperationContract(IsOneWay = false)]
        string TriggerSingleTaskInChain(int chainId, int FlowId, string username, DateTime? asOfDate, string clientName);

        [OperationContract(IsOneWay = true)]
        void TaskCompleted(TaskInfo taskInfo, string guid, string clientName);

        [OperationContract(IsOneWay = false)]
        bool isAlive(string clientName);

        [OperationContract(IsOneWay = false)]
        Boolean RemoveTaskFromRunningChains(int taskStatusId, string guid, string clientName);

        [OperationContract(IsOneWay = false)]
        Boolean CTMUndoTask(int taskStatusId, string clientName);


        [OperationContract(IsOneWay = false)]
        Dictionary<string, List<string>> GetSubscriberList(int moduleId,string clientName);

        [OperationContract(IsOneWay = false)]
        List<string> GetCalendarList(int moduleId, string clientName);

        [OperationContract(IsOneWay = false)]
        List<int> GetUnsyncdTasksClientTaskStatusIds(string clientName);

        [OperationContract(IsOneWay = false)]
        List<string> GetPrivilegeList(string username, string clientName);

        [OperationContract(IsOneWay = true)]
        void flowAdded(List<int> task_summary_ids, string clientName);
        [OperationContract(IsOneWay = true)]
        void flowDeleted(List<int> list, string clientName);
    }

    [ServiceContract]
    public interface ICTMServiceCallback
    {
        //[OperationContract(IsOneWay = true)]
        ////void TriggerTask(int flowId);
        //void TriggerTask(int taskSummaryId,string guid,string environmentVariables);

        [OperationContract(IsOneWay = true)]
        void triggerTask(TaskInfo taskInfo, string guid, string clientName);

        [OperationContract(IsOneWay = false)]
        List<string> getSubscriberList(string assemblyLocation, string className, string clientName);

        [OperationContract(IsOneWay = false)]
        List<string> getCalendarList(string assemblyLocation, string className, string clientName);

        [OperationContract(IsOneWay = false)]
        Boolean deleteStatusFromClient(string assemblyLocation, string className, int clientTaskStatusId, string clientName);

        [OperationContract(IsOneWay = false)]
        Boolean UndoTask(string assemblyLocation, string className, int clientTaskStatusId, string clientName);

        [OperationContract(IsOneWay = false)]
        List<int> getUnsyncdTasksClientTaskStatusIds(string assemblyLocation, string className, List<int> clientTaskStatusIds, string clientName);

        [OperationContract]
        string keepAlive(string clientName);

        [OperationContract(IsOneWay = false)]
        List<string> getPrivilegeList(string assemblyLocation, string className, string pageId, string username, string clientName);

        [OperationContract(IsOneWay = true)]
        void flowsAdded(string assemblyLocation, string className, List<int> list, string clientName);

        [OperationContract(IsOneWay = true)]
        void flowsDeleted(string assemblyLocation, string className, List<int> list, string clientName);

        [OperationContract(IsOneWay = false)]
        List<int> isSecureToTrigger(string assemblyLocation, string className, int taskMasterId, string clientName);

        [OperationContract(IsOneWay = false)]
        DataTable SyncStatus(string assemblyLocation, string className, List<int> ctmStatusId, string clientName);

        [OperationContract(IsOneWay = false)]
        void KillInprogressTask(string assemblyLocation, string className, List<int> ctmStatusId, string clientName);
    }

    public class chainInfo
    {
        public int chainId { get; set; }
        static IRLogger mLogger = RLogFactory.CreateLogger("RCTMService");
        //List<flowId>
        public List<int> completedTasks = new List<int>();
        public List<TaskInfo> runningTasks = new List<TaskInfo>();
        //static RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
        //static RHashlist htParams = null;
        public chainInfo(int chainId)
        {
            RDBConnectionManager dbConnection = null;
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                htParams.Add("chain_id", chainId);

                DataSet ds = dbConnection.ExecuteQuery("CTM:GetChainByChainId", htParams);
                mLogger.Debug("chain info XML" + ds.GetXml());
                DataRow rdr = ds.Tables[0].Rows[0];
                this.max_parallel_instances_allowed = rdr["max_parallel_instances_allowed"] != DBNull.Value ? Convert.ToInt32(rdr["max_parallel_instances_allowed"]) : 0;
                this.chainId = chainId;
                this.allow_parallel = rdr["allow_parallel"] != DBNull.Value ? Convert.ToBoolean(rdr["allow_parallel"]) : false;
                if (rdr["filewatcher_info"] != DBNull.Value)
                {
                    string[] fileWatcherDetails = rdr["filewatcher_info"].ToString().Split(new char[] { '|' });//,StringSplitOptions.RemoveEmptyEntries);
                    this.filewatcherInfo = new FilewatcherInfo() { SearchFolder = fileWatcherDetails[0], FileRegex = fileWatcherDetails[1] };
                }

                if (rdr["subscribe_id"] != DBNull.Value)
                {
                    MailDetails = new Dictionary<com.ivp.rad.RCTMUtils.TaskStatus, MailDetails>();
                    if (rdr["subscribe_id"].ToString().Split(new char[] { '|' })[0] != null && string.IsNullOrEmpty(rdr["subscribe_id"].ToString().Split(new char[] { '|' })[0].Trim()) == false)
                    {
                        MailDetails[com.ivp.rad.RCTMUtils.TaskStatus.PASSED] = new MailDetails();
                        if (rdr["subscribe_id"].ToString().Split(new char[] { '|' }).Length >= 3 && string.IsNullOrEmpty(rdr["subscribe_id"].ToString().Split(new char[] { '|' })[2].Trim()) == false)
                        { MailDetails[com.ivp.rad.RCTMUtils.TaskStatus.PASSED].MailSubject = System.Web.HttpUtility.UrlDecode(rdr["subscribe_id"].ToString().Split(new char[] { '|' })[2]); }
                        if (rdr["subscribe_id"].ToString().Split(new char[] { '|' }).Length >= 4 && string.IsNullOrEmpty(rdr["subscribe_id"].ToString().Split(new char[] { '|' })[3].Trim()) == false)
                        { MailDetails[com.ivp.rad.RCTMUtils.TaskStatus.PASSED].mailBody = System.Web.HttpUtility.UrlDecode(rdr["subscribe_id"].ToString().Split(new char[] { '|' })[3]); }
                        foreach (string str in (rdr["subscribe_id"].ToString().Split(new char[] { '|' })[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)))
                        {
                            MailDetails[com.ivp.rad.RCTMUtils.TaskStatus.PASSED].MailIds.Add(str);
                        }

                    }
                    if (string.IsNullOrEmpty(rdr["subscribe_id"].ToString().Split(new char[] { '|' })[1].Trim()) == false)
                    {
                        MailDetails[com.ivp.rad.RCTMUtils.TaskStatus.FAILED] = new MailDetails();
                        if (rdr["subscribe_id"].ToString().Split(new char[] { '|' }).Length >= 5 && string.IsNullOrEmpty(rdr["subscribe_id"].ToString().Split(new char[] { '|' })[4].Trim()) == false)
                        {
                            MailDetails[com.ivp.rad.RCTMUtils.TaskStatus.FAILED].MailSubject = System.Web.HttpUtility.UrlDecode(rdr["subscribe_id"].ToString().Split(new char[] { '|' })[4]);
                        }
                        if (rdr["subscribe_id"].ToString().Split(new char[] { '|' }).Length >= 6 && string.IsNullOrEmpty(rdr["subscribe_id"].ToString().Split(new char[] { '|' })[5].Trim()) == false)
                        {
                            MailDetails[com.ivp.rad.RCTMUtils.TaskStatus.FAILED].mailBody = System.Web.HttpUtility.UrlDecode(rdr["subscribe_id"].ToString().Split(new char[] { '|' })[5]);
                        }
                        foreach (string str in (rdr["subscribe_id"].ToString().Split(new char[] { '|' })[1].Split(new char[] { ',' })))
                        {
                            MailDetails[com.ivp.rad.RCTMUtils.TaskStatus.FAILED].MailIds.Add(str);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CTMService:ctor>>Error while trying to create new instance of chainInfo with chainId:" + chainId);
                mLogger.Error(ex);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
        }
        public int max_parallel_instances_allowed { get; set; }
        public bool allow_parallel { get; set; }
        public bool IsScheduled { get; set; }
        public Boolean IsService { get; set; }

        public Dictionary<com.ivp.rad.RCTMUtils.TaskStatus, MailDetails> MailDetails { get; set; }
        public FilewatcherInfo filewatcherInfo { get; set; }

        private PropertyInfo[] _PropertyInfos = null;

        public override string ToString()
        {
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                sb.AppendLine(info.Name + ": " + (info.GetValue(this, null) == null ? "null" : info.GetValue(this, null).ToString()));
            }

            return sb.ToString();
        }

    }

    public class FilewatcherInfo
    {
        public string SearchFolder { get; set; }
        public string FileRegex { get; set; }

        private PropertyInfo[] _PropertyInfos = null;

        public override string ToString()
        {
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                sb.AppendLine(info.Name + ": " + (info.GetValue(this, null) == null ? "null" : info.GetValue(this, null).ToString()));
            }

            return sb.ToString();
        }
    }

    public class TaskSummaryInfo
    {
        public TaskSummaryInfo()
        {
            is_retryable = true;
            is_visible_on_ctm = true;

        }
        public int task_summary_id { get; set; }
        public int task_master_id { get; set; }
        public string task_name { get; set; }
        public string task_description { get; set; }
        public int task_type_id { get; set; }
        public string created_by { get; set; }
        public DateTime created_on { get; set; }
        public string last_modified_by { get; set; }
        public DateTime last_modified_on { get; set; }
        public string module_name { get; set; }
        public int module_id { get; set; }
        public int registered_module_id { get; set; }
        public string task_type_name { get; set; }
        public Boolean is_visible_on_ctm { get; set; }
        public Boolean is_retryable { get; set; }
        public Boolean is_undoable { get; set; }
        public OperationType type { get; set; }

        private PropertyInfo[] _PropertyInfos = null;

        public override string ToString()
        {
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                sb.AppendLine(info.Name + ": " + (info.GetValue(this, null) == null ? "null" : info.GetValue(this, null).ToString()));
            }

            return sb.ToString();
        }
    }

    public enum OperationType
    {
        Add,
        Update,
        Delete
    }

    public static class RCTMDic
    {
        public static Dictionary<string,Dictionary<int, SortedDictionary<DateTime, RCTMQueuedTaskInfo>>> WaitingQueue { get; set; } //TaskSummaryId Vs sorted by endtime and info
        public static Dictionary<string,Dictionary<int, List<int>>> RunningTasks { get; set; }
        public static object LockObject = new object();
    }
}
