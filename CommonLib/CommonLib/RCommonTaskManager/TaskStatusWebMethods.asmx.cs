using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using System.ServiceModel;
using System.Data;
//using com.ivp.rad.RCommonTaskManager;
using com.ivp.rad.RCTMUtils;
using System.ServiceModel.Activation;
using System.Runtime.Serialization;
using com.ivp.rad.common;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using com.ivp.srmcommon;

namespace com.ivp.rad.RCommonTaskManager
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class TaskStatusWebMethods : System.Web.Services.WebService, ICTMServiceCallback
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RCTMService");
        [WebMethod]
        public string DeleteStatus(string taskStatusId, string clientName)
        {

            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            DuplexChannelFactory<ICTMService> dupFactory = null; ICTMService clientProxy = null;
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;

                mLogger.Debug("CTM: deleting status with status id" + taskStatusId);
                TaskStatusWebMethods cbk = new TaskStatusWebMethods();
                dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
                dupFactory.Open();
                clientProxy = dupFactory.CreateChannel();
                try
                {
                    clientProxy.RemoveTaskFromRunningChains(Convert.ToInt32(taskStatusId), getGuidFromTaskStatusId(taskStatusId), clientName);
                }
                catch (Exception ex)
                {
                    mLogger.Error("Unable to remove task from running chains" + ex);
                }

                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");

                htParams.Add("task_status_id", taskStatusId);
                dbConnection.ExecuteQuery("CTM:DeleteTaskStatusByTaskStatusId", htParams);
                mLogger.Debug("task status deleted from db");

                return "true";
            }

            catch (Exception ex)
            {
                mLogger.Error("CTM:Error while deleting status: " + ex.ToString());
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }
        [WebMethod]
        public string UndoTask(string taskStatusId, string clientName)
        {

            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            DuplexChannelFactory<ICTMService> dupFactory = null; ICTMService clientProxy = null;
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;

                mLogger.Debug("CTM: task undo with status id" + taskStatusId);
                TaskStatusWebMethods cbk = new TaskStatusWebMethods();
                dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
                dupFactory.Open();
                clientProxy = dupFactory.CreateChannel();
                if (clientProxy.CTMUndoTask(Convert.ToInt32(taskStatusId), clientName) == true)
                {
                    //{
                    //  dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");

                    //htParams.Add("task_status_id", taskStatusId);
                    //dbConnection.ExecuteQuery("CTM:DeleteTaskStatusByTaskStatusId", htParams);
                    return "true";
                }
                else
                {
                    return "false";
                }
            }

            catch (Exception ex)
            {
                mLogger.Error("CTM:Error while deleting status: " + ex.ToString());
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }

        private string getGuidFromTaskStatusId(string taskStatusId)
        {
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            mLogger.Debug("CTM: fetching guid from task status id with taskstatusid :" + taskStatusId);
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                htParams = new RHashlist();
                htParams.Add("task_status_id", taskStatusId);

                return dbConnection.ExecuteQuery("CTM:GetGuidByTaskStatusId", htParams).Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                mLogger.Error("CTM:Error while fetching guid from task status id :" + ex.ToString());
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }

        [WebMethod]
        public void RetriggerTask(string flowId, string username, string option, DateTime? asOfDate, string clientName)
        {
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;

                mLogger.Debug("CTM:retriggering task with flow id " + flowId + " and username " + username);

                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                htParams.Add("flow_id", flowId);
                int chainId = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetChainIdByFlowId", htParams).Tables[0].Rows[0][0]);

                DuplexChannelFactory<ICTMService> dupFactory = null;
                ICTMService clientProxy = null;

                TaskStatusWebMethods cbk = new TaskStatusWebMethods();
                dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
                dupFactory.Open();
                clientProxy = dupFactory.CreateChannel();
                if (option == "0")//"Trigger from this task")
                {
                    clientProxy.TriggerTaskInChain(chainId, Convert.ToInt32(flowId), username, asOfDate, clientName);
                }
                else if (option == "1")//"Trigger only this task")
                {
                    clientProxy.TriggerSingleTaskInChain(chainId, Convert.ToInt32(flowId), username, asOfDate, clientName);
                }
                else if (option == "2")//Trigger all tasks from begining of chain
                {
                    clientProxy.TriggerChain(chainId, false, username, asOfDate, clientName);
                }


            }
            catch (Exception ex)
            {
                mLogger.Error("CTM:Error while retriggering task " + ex.ToString());
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }

        [WebMethod]
        public string getGanttChartData(string flowId, string startTime, string endTime, string isGlobalTaskStatus, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;

            if (startTime == "0") { if (isGlobalTaskStatus == "False") { startTime = DateTime.Now.AddDays(-20).ToShortDateString(); } else { startTime = DateTime.Now.AddDays(-10).ToShortDateString(); } }
            if (endTime == "0") { endTime = DateTime.Now.AddDays(1).ToShortDateString(); }
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            mLogger.Debug("CTM:fetching data for gantt chart with flow id " + flowId + " start time " + startTime + " end time " + endTime + " isglobaltaskstatus" + isGlobalTaskStatus);
            try
            {
                DataTable table = null;
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                if (isGlobalTaskStatus == "False")
                {
                    htParams.Add("flow_id", flowId);
                    htParams.Add("start_time", startTime);
                    htParams.Add("end_time", endTime);
                    table = dbConnection.ExecuteQuery("CTM:GetTaskStatusByFlowIdStartTimeEndTime", htParams).Tables[0];
                }
                else
                {
                    htParams.Add("start_time", startTime);
                    htParams.Add("end_time", endTime);
                    table = dbConnection.ExecuteQuery("CTM:GetAllTaskStatusByStartTimeEndTime", htParams).Tables[0];
                }
                List<TaskStatusModel> taskStatuses = new List<TaskStatusModel>();

                foreach (DataRow rdr in table.Rows)
                {
                    taskStatuses.Add(new TaskStatusModel()
                    {
                        task_type_name = rdr["task_type_name"].ToString(),
                        task_name = rdr["task_name"].ToString(),
                        //task_status_id = Convert.ToInt32(rdr["task_status_id"]),
                        //flow_id = (int)rdr["flow_id"],
                        end_time = Convert.ToDateTime(rdr["end_time"]),
                        //environment_variables = rdr["environment_variables"].ToString(),
                        //log_description = rdr["log_description"].ToString(),
                        start_time = Convert.ToDateTime(rdr["start_time"]),
                        status = rdr["status"].ToString()
                    });
                }

                var obj = new System.Web.Script.Serialization.JavaScriptSerializer();
                obj.MaxJsonLength = Int32.MaxValue;
                return obj.Serialize(taskStatuses);
            }
            catch (Exception ex)
            {
                mLogger.Error("CTM: error while fetching gantt chart data : " + ex.ToString());
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }


        [WebMethod]
        public bool CompareDate(string startDate, string endDate, bool setServerDate = false)
        {
            if (setServerDate)
                endDate = DateTime.Now.ToString();
            DateTime startDt, endDt;
            int result = 0;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                startDt = Convert.ToDateTime(startDate);
                endDt = Convert.ToDateTime(endDate);
                result = DateTime.Compare(startDt, endDt);
            }
            return ((result > 0) ? true : false);
        }

        [WebMethod]
        public string GetTaskStatusInfo(string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;

            mLogger.Error("GetTaskStatusInfo ==> Start");
            SRMTaskStatusInfo objTaskStatusInfo = new SRMTaskStatusInfo();
            objTaskStatusInfo.TaskType = BindTaskType();
            objTaskStatusInfo.TaskStatus = BindSRMTaskStatus();
            var obj = new System.Web.Script.Serialization.JavaScriptSerializer();
            obj.MaxJsonLength = Int32.MaxValue;
            //mLogger.Error("CTM: Fetching GetTaskStatusInfo: " + obj.Serialize(objTaskStatusInfo));

            return obj.Serialize(objTaskStatusInfo);
        }

        private List<TaskStatusDetails> BindSRMTaskStatus()
        {
            try
            {
                List<TaskStatusDetails> l = new List<TaskStatusDetails>();
                //l.Add(new TaskStatusDetails
                //{
                //    TaskStatusId = 0,
                //    TaskStatusName = SMTaskStatusTypes.ALL.ToString()
                //});
                l.Add(new TaskStatusDetails
                {
                    TaskStatusId = 1,
                    TaskStatusName = SMTaskStatusTypes.Failed.ToString()
                });
                l.Add(new TaskStatusDetails
                {
                    TaskStatusId = 2,
                    TaskStatusName = SMTaskStatusTypes.Inprogress.ToString()
                });
                l.Add(new TaskStatusDetails
                {
                    TaskStatusId = 3,
                    TaskStatusName = SMTaskStatusTypes.Passed.ToString()
                });
                l.Add(new TaskStatusDetails
                {
                    TaskStatusId = 4,
                    TaskStatusName = SMTaskStatusTypes.Queued.ToString()
                });


                return l;
            }
            catch (Exception ex) { throw ex; }
            finally
            {

            }
        }

        public List<TaskTypeDetails> BindTaskType()
        {
            List<TaskTypeDetails> lstObj = new List<TaskTypeDetails>();

            DataTable dt = DALAdapter.GetSRMTaskTypesForTaskStatus();
            if (dt != null && dt.Rows.Count > 0)
            {
                //lstObj.Add(new TaskTypeDetails
                //{
                //    TaskTypeDetailsId = "ALL",
                //    TaskTypeDetailsName = "ALL"
                //});
                foreach (DataRow dr in dt.AsEnumerable().OrderBy(x => x.Field<string>("task_type_name")))
                {
                    lstObj.Add(new TaskTypeDetails
                    {
                        TaskTypeDetailsId = dr.Field<string>("task_type_name"),
                        TaskTypeDetailsName = Convert.ToString(dr["task_type_name"])
                    });
                }
            }
            return lstObj;
        }

        void ICTMServiceCallback.triggerTask(TaskInfo taskInfo, string guid,string clientName)
        {
            throw new NotImplementedException();
        }

        public List<string> getSubscriberList(string a, string b, string clientName)
        {
            throw new NotImplementedException();
        }
        public List<string> getCalendarList(string a, string b, string clientName)
        {
            throw new NotImplementedException();
        }
        public Boolean deleteStatusFromClient(string assemblyLocation, string className, int clientTaskStatusId, string clientName)
        {
            throw new NotImplementedException();
        }
        Boolean ICTMServiceCallback.UndoTask(string assemblyLocation, string className, int clientTaskStatusId, string clientName)
        {
            throw new NotImplementedException();
        }
        public List<int> getUnsyncdTasksClientTaskStatusIds(string assemblyLocation, string className, List<int> list, string clientName)
        {
            throw new NotImplementedException();
        }
        public string keepAlive(string clientName)
        {
            throw new NotImplementedException();
        }

        void ICTMServiceCallback.flowsAdded(string assemblyLocation, string className, List<int> clientTaskStatusId, string clientName)
        {
            throw new NotImplementedException();
        }

        void ICTMServiceCallback.flowsDeleted(string assemblyLocation, string className, List<int> clientTaskStatusId, string clientName)
        {
            throw new NotImplementedException();
        }
        List<string> ICTMServiceCallback.getPrivilegeList(string assemblyLocation, string className, string pageId, string username, string clientName)
        {
            throw new NotImplementedException();
        }

        public DataTable SyncStatus(string assemblyLocation, string className, List<int> ctmStatusId, string clientName)
        {
            throw new NotImplementedException();
        }

        public void KillInprogressTask(string assemblyLocation, string className, List<int> ctmStatusId, string clientName)
        {
            throw new NotImplementedException();
        }


        List<int> ICTMServiceCallback.isSecureToTrigger(string assemblyLocation, string className, int taskMasterId, string clientName)
        {
            throw new NotImplementedException();
        }
    }
    public enum SMTaskStatusTypes
    {
        ALL = 0,
        /// <summary>
        /// Success Staus
        /// </summary>
        Failed = 1,
        /// <summary>
        /// Failed Status
        /// </summary>
        Inprogress = 2,
        /// <summary>
        /// INPROGRESS status
        /// </summary>
        Passed = 3,
        /// <summary>
        /// NOTPROCESSED status
        /// </summary>
        Queued = 4
    }
    public class SRMTaskStatusInfo
    {
        public List<TaskTypeDetails> TaskType { get; set; }
        public List<TaskStatusDetails> TaskStatus { get; set; }
    }
    public class TaskTypeDetails
    {
        public string TaskTypeDetailsId { get; set; }
        public string TaskTypeDetailsName { get; set; }
    }
    public class TaskStatusDetails
    {
        public int TaskStatusId { get; set; }
        public string TaskStatusName { get; set; }
    }
}
