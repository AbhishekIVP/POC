using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;

namespace com.ivp.rad.RCTMUtils
{
    [Serializable]
    public class TriggerAsOfDateInfo
    {
        public string customValue { get; set; }

        public DateTime? triggerDate { get; set; }
    }
    [DataContract]
    [Serializable]
    public class TaskInfo
    {
        public TaskInfo()
        {
            MailSubscribeId = new Dictionary<TaskStatus, MailDetails>();
        }
        [DataMember]
        public TriggerAsOfDateInfo triggerAsOfDateInfo { get; set; }
        //[DataMember]
        //public DateTime? triggerAsOfDate { get; set; }
        [DataMember]
        public string TaskName { get; set; }
        [DataMember]
        public int TaskMasterId { get; set; }
        [DataMember]
        public int FlowID { get; set; }
        [DataMember]
        public int TimeOut { get; set; }
        [DataMember]
        public string ChainName { get; set; }
        [DataMember]
        public string ChainGUID { get; set; }
        [DataMember]
        public bool IsLastTaskInChain { get; set; }            
        [DataMember]
        public Dictionary<TaskStatus, MailDetails> ChainMailSubscribeId { get; set; }
        [DataMember]
        public Dictionary<TaskStatus,MailDetails> MailSubscribeId { get; set; }
        [DataMember]
        public bool IsReRunOnFail { get; set; }
        [DataMember]
        public int RetryCount { get; set; }
        [DataMember]
        public int RetryInterval { get; set; }
        [DataMember]
        public string ExtraInfo { get; set; }
        [DataMember]
        public int ProcessId { get; set; }
        [DataMember]
        public TaskStatusInfo Status { get; set; }
        [DataMember]
        public string AssemblyLocation { get; set; }
        [DataMember]
        public string ClassName { get; set; }
        [DataMember]
        public string CustomClassAssembly { get; set; }
        [DataMember]
        public string CustomClassClassName { get; set; }
        [DataMember]
        public object DependantOnId { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public int ChainId { get; set; }
        [DataMember]
        public bool ProceedOnFail { get; set; }
        [DataMember]
        public bool IsMuted { get; set; }
        [DataMember]
        public int OnFailRunTask { get; set; }
        [DataMember]
        public int ModuleId { get; set; }
        [DataMember]
        public int RegisteredModuleId{get;set;}
        [DataMember]
        public string CalendarName { get; set; }
        [DataMember]
        public string TaskTypeName { get; set; }
        [DataMember]
        public string ModuleName { get; set; }
        [DataMember]
        public string IsUndoSupported { get; set; }
        [DataMember]
        public int TaskSummaryId { get; set; }
		[DataMember]
	    public bool isService { get; set; }
        [DataMember]
        public string AdditionalInfo { get; set; }
        [DataMember]
        public string[] Subscribers { get; set; }
        [DataMember]
        public int SecondInstanceWait { get; set; }
        [DataMember]
        public DateTime? TaskEndTime { get; set; }
        [DataMember]
        public string ClientName { get; set; }

        private PropertyInfo[] _PropertyInfos = null;
        public override string ToString()
        {
            var sb = new StringBuilder();
            try
            {
                if (_PropertyInfos == null)
                    _PropertyInfos = this.GetType().GetProperties();



                foreach (var info in _PropertyInfos)
                {
                    if (info != null && info.Name == "MailSubscribeId") { foreach (var mail in MailSubscribeId) { sb.Append(mail + ";"); } }
                    if(info!=null && info.GetValue(this,null)!=null)
                    sb.Append(info.Name + ": " +info.GetValue(this, null).ToString()+";");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "Error in TaskInfo.toString";
                throw;
            }
        }
    }

    [DataContract]
    [Serializable]
    public class MailDetails
    {
        public MailDetails()
        {
            MailIds = new List<string>();
        }
        [DataMember]
        public List<String> MailIds { get; set; }
        [DataMember]
        public string MailSubject { get; set; }
        [DataMember]
        public string mailBody { get; set; }

        private PropertyInfo[] _PropertyInfos = null;

        public override string ToString()
        {
            var sb = new StringBuilder();
            try
            {
                if (_PropertyInfos == null)
                    _PropertyInfos = this.GetType().GetProperties();

                

                foreach (var info in _PropertyInfos)
                {
                    if (info != null && info.GetValue(this, null) != null)
                    sb.Append(info.Name + ": " + info.GetValue(this, null).ToString()+";" );
                }

                return sb.ToString();
            }
            catch(Exception ex){
                return "Error in MailDetails.toString()";
                throw;
            }
        }
    }
    [DataContract]
    [Serializable]
    public class TaskStatusInfo
    {   [DataMember]
        public int StatusId { get; set; }
        [DataMember]
        public TaskStatus Status { get; set; }
        [DataMember]
        public string TaskLog { get; set; }
        [DataMember]
        public string environmentVariables { get; set; }
        [DataMember]
        public string chain_guid { get; set; }
        [DataMember]
        public int clientStatusId { get; set; }
        private PropertyInfo[] _PropertyInfos = null;
        [DataMember]
        public DateTime start_time { get; set; }
        [DataMember]
        public DateTime end_time { get; set; }
        [DataMember]
        public string task_name { get; set; }
        [DataMember]
        public string task_type_name { get; set; }
        [DataMember]
        public string module_name { get; set; }
        [DataMember]
        public int flow_id { get; set; }
        public static TaskStatusInfo dataRowToTaskStatusInfo(System.Data.DataRow rdr){
            TaskStatus tmpStatus;
            if (rdr["status"] != DBNull.Value)
            {
                switch (rdr["status"].ToString().ToLower())
                {
                    case "inprogress": tmpStatus = TaskStatus.INPROGRESS; break;
                    case "failed": tmpStatus = TaskStatus.FAILED; break;
                    case "passed": tmpStatus = TaskStatus.PASSED; break;
                    default: tmpStatus = TaskStatus.FAILED; break;
                }
            }
            else
            {
                tmpStatus = TaskStatus.FAILED;
            }
            return new TaskStatusInfo() { 
            chain_guid = rdr["chain_guid"]!=DBNull.Value?rdr["chain_guid"].ToString():"",
            clientStatusId = rdr["client_task_status_id"] != DBNull.Value ? Convert.ToInt32(rdr["client_task_status_id"]):-1,
            end_time = rdr["end_time"] != DBNull.Value ?Convert.ToDateTime( rdr["end_time"]):new DateTime(),
            start_time = rdr["start_time"] != DBNull.Value ?Convert.ToDateTime(  rdr["start_time"]):new DateTime(),
            Status =tmpStatus,
            StatusId = rdr["task_status_id"] != DBNull.Value ? Convert.ToInt32(rdr["task_status_id"]):0,
            TaskLog = rdr["log_description"] != DBNull.Value ? rdr["log_description"].ToString() : "",
            task_name = rdr["task_name"] != DBNull.Value ? rdr["task_name"].ToString() : "",
            task_type_name = rdr["task_type_name"] != DBNull.Value ? rdr["task_type_name"].ToString() : "",
            module_name = rdr["module_name"] != DBNull.Value ? rdr["module_name"].ToString() : "",
            flow_id = rdr["flow_id"] != DBNull.Value ? Convert.ToInt32(rdr["flow_id"]) : 0
        
            };
        }

        public static TaskStatusInfo dataRowToTaskStatusInfoWOFlow(System.Data.DataRow rdr){
            TaskStatus tmpStatus;
            if (rdr["status"] != DBNull.Value)
            {
                switch (rdr["status"].ToString().ToLower())
                {
                    case "inprogress": tmpStatus = TaskStatus.INPROGRESS; break;
                    case "failed": tmpStatus = TaskStatus.FAILED; break;
                    case "passed": tmpStatus = TaskStatus.PASSED; break;
                    default: tmpStatus = TaskStatus.FAILED; break;
                }
            }
            else
            {
                tmpStatus = TaskStatus.FAILED;
            }
            return new TaskStatusInfo() { 
            chain_guid = rdr["chain_guid"]!=DBNull.Value?rdr["chain_guid"].ToString():"",
            clientStatusId = rdr["client_task_status_id"] != DBNull.Value ? Convert.ToInt32(rdr["client_task_status_id"]):-1,
            end_time = rdr["end_time"] != DBNull.Value ?Convert.ToDateTime( rdr["end_time"]):new DateTime(),
            start_time = rdr["start_time"] != DBNull.Value ?Convert.ToDateTime(  rdr["start_time"]):new DateTime(),
                Status =tmpStatus,
            StatusId = rdr["task_status_id"] != DBNull.Value ? Convert.ToInt32(rdr["task_status_id"]):0,
            TaskLog = rdr["log_description"] != DBNull.Value ? rdr["log_description"].ToString() : "",
            //task_name = rdr["task_name"] != DBNull.Value ? rdr["task_name"].ToString() : "",
            //task_type_name = rdr["task_name"] != DBNull.Value ? rdr["task_name"].ToString() : "",
            //module_name = rdr["task_name"] != DBNull.Value ? rdr["task_name"].ToString() : "",
        
            };
        }

        public override string ToString()
        {
            if (_PropertyInfos == null)
                _PropertyInfos = this.GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _PropertyInfos)
            {
                if (info != null && info.GetValue(this, null) != null)
                sb.AppendLine(info.Name + ": " + info.GetValue(this, null).ToString()+";");
            }

            return sb.ToString();
        }
    }

    [DataContract]
    [Serializable]
    public enum TaskStatus
    {
        [EnumMember]
        PASSED,
        [EnumMember]
        FAILED,
        [EnumMember]
        INPROGRESS
    }

    [Serializable]
    public class DataInfo
    {
        #region private member variables

        private byte[] _key;
        private byte[] _iv;
        private string _encryptedData;

        #endregion

        #region property
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the KEY.
        /// </summary>
        /// <value>The KEY.</value>
        public Byte[] KEY
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the InitializationVector.
        /// </summary>
        /// <value>The IV.</value>
        public Byte[] IV
        {
            get
            {
                return _iv;
            }
            set
            {
                _iv = value;
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the encrypted data.
        /// </summary>
        /// <value>The encrypted data.</value>
        public string EncryptedData
        {
            get
            {
                return _encryptedData;
            }
            set
            {
                _encryptedData = value;
            }
        }

        //------------------------------------------------------------------------------------------
        #endregion


    }

    [DataContract]
    [Serializable]
    public class TaskStatusModel
    {
        [DataMember]
        public int task_status_id { get; set; }
        [DataMember]
        public int flow_id { get; set; }
        [DataMember]
        public DateTime start_time { get; set; }
        [DataMember]
        public DateTime end_time { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string log_description { get; set; }
        [DataMember]
        public string environment_variables { get; set; }
        [DataMember]
        public string task_name { get; set; }
        [DataMember]
        public string task_type_name { get; set; }
        [DataMember]
        public object module_name { get; set; }
    }

}
