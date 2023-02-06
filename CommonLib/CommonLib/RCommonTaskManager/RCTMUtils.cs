using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.utils;
using com.ivp.rad.dal;
using System.Data.SqlClient;
using System.Data;
using com.ivp.rad.RCTMUtils;
using com.ivp.rad.common;
using System.Diagnostics;

namespace com.ivp.rad.RCommonTaskManager
{
    public class RCTMUtils
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RCTMService");


        public static RHashlist fillHtParams(object ob, List<string> excludeList)
        {
            RHashlist htParams = new RHashlist();
            foreach (var prop in ob.GetType().GetProperties())
            {
                if (excludeList != null && excludeList.Contains(prop.Name) == false)
                    htParams.Add(prop.Name, prop.GetValue(ob, null));
            }
            return htParams;
        }

        public bool SyncUp(List<TaskSummaryInfo> taskInfos, bool isDelta, RDBConnectionManager dbConnection)
        {
            if (dbConnection == null)
            {
                return SyncUp(taskInfos, isDelta);
            }
            else
            {
                RHashlist htParams = null;
                try
                {
                    mLogger.Debug("Syncing up");
                    foreach (var task in taskInfos)
                    {

                        switch (task.type)
                        {
                            case OperationType.Add:
                                htParams = new RHashlist();
                                htParams.Add("task_master_id", task.task_master_id);
                                htParams.Add("task_name", task.task_name);
                                htParams.Add("task_description", task.task_description);
                                htParams.Add("created_by", task.created_by);
                                htParams.Add("created_on", task.created_on);
                                htParams.Add("last_modified_by", task.last_modified_by);
                                htParams.Add("last_modified_on", task.last_modified_on);
                                htParams.Add("module_name", task.module_name);
                                htParams.Add("module_id", task.module_id);
                                htParams.Add("task_type_name", task.task_type_name);
                                htParams.Add("registered_module_id", task.registered_module_id);
                                htParams.Add("is_visible_on_ctm", task.is_visible_on_ctm);
                                htParams.Add("is_retryable", task.is_retryable);
                                htParams.Add("is_undoable", task.is_undoable);
                                //htParams = fillHtParams(task, new List<string>() { "type", "task_summary_id", "task_type_id", "_PropertyInfos" });
                                dbConnection.ExecuteQuery("CTM:InsertTaskSummary", htParams);
                                break;
                            case OperationType.Update:
                                htParams = new RHashlist();
                                htParams.Add("task_master_id", task.task_master_id);
                                htParams.Add("task_name", task.task_name);
                                htParams.Add("task_description", task.task_description);
                                htParams.Add("last_modified_by", task.last_modified_by);
                                htParams.Add("last_modified_on", task.last_modified_on);
                                htParams.Add("module_name", task.module_name);
                                htParams.Add("module_id", task.module_id);
                                htParams.Add("task_type_name", task.task_type_name);
                                htParams.Add("registered_module_id", task.registered_module_id);
                                //htParams.Add("is_visible_on_ctm", task.is_visible_on_ctm);
                                // htParams.Add("is_retryable", task.is_retryable);
                                //htParams = fillHtParams(task, new List<string>() { "type", "task_summary_id", "task_type_id", "created_by", "created_on", "_PropertyInfos" });
                                dbConnection.ExecuteQuery("CTM:UpdateTaskSummaryByTaskMasterIdModuleId", htParams);
                                break;
                            case OperationType.Delete:
                                htParams = new RHashlist();
                                htParams.Add("task_master_id", task.task_master_id);
                                htParams.Add("module_id", task.module_id);
                                //dbConnection.ExecuteQuery("CTM:DeleteTaskSummaryByTaskMasterIdModuleId", htParams);
                                dbConnection.ExecuteQuery(string.Format(@"DECLARE @EncodedUser VARBINARY(128);SET @EncodedUser = CONVERT(VARBINARY(128),CONVERT(CHAR(128), '{2}')); SET CONTEXT_INFO @EncodedUser; DELETE FROM [ivp_rad_task_summary] WHERE [task_master_id] = {0} and [module_id]={1}", task.task_master_id, task.module_id, task.last_modified_by), RQueryType.Select);
                                break;

                        }
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error("error while syncing up :");
                    mLogger.Error(ex.ToString());
                    //return null;
                    throw;
                }
                finally
                {
                    if (htParams != null)
                        htParams = null;
                }
                return true;
            }
        }

        public bool SyncUp(List<TaskSummaryInfo> taskInfos, bool isDelta)
        {
            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            bool result = true;
            try
            {
                result = SyncUp(taskInfos, isDelta, dbConnection);
            }
            catch (Exception ex)
            {
                mLogger.Error("error while syncing up :");
                mLogger.Error(ex.ToString());
                //return null;
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
            return result;
        }

        public void DeleteStatusByClientTaskStatusId(int moduleId, int clientTaskStatusId)
        {
            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            RHashlist htParams = null;
            try
            {
                htParams = new RHashlist();
                htParams.Add("module_id", moduleId);
                htParams.Add("client_task_status_id", clientTaskStatusId);
                dbConnection.ExecuteQuery("CTM:DeleteStatusByClientTaskStatusIdModuleId", htParams);
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }

        }

        public int AddTaskStatus(TaskStatusInfo taskStatusInfo, Flow flow)
        {
            //int taskSummary = DALAdapter.getTaskSummaryByTaskMasterId(clientTaskMasterId, moduleId);

            ChainedTask chainedTask = new ChainedTask();
            chainedTask.is_active = true;
            chainedTask.calendar_name = "NYSE";
            chainedTask.scheduled_job_id = -1;
            chainedTask.created_by = "System Generated";
            chainedTask.created_on = taskStatusInfo.start_time;
            chainedTask.last_modified_by = "System Generated";
            int ChainID = DALAdapter.addNewChain(chainedTask);

            flow.chain_id = ChainID;
            int flowId = DALAdapter.addFlow(flow, "", chainedTask.created_by);
            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            RHashlist htParams = new RHashlist();
            try
            {
                mLogger.Debug("Adding task status to db >> " + taskStatusInfo);
                string status = taskStatusInfo.Status == TaskStatus.FAILED ? "FAILED" : taskStatusInfo.Status == TaskStatus.PASSED ? "PASSED" : "INPROGRESS";
                string log_description = taskStatusInfo.TaskLog != null ? taskStatusInfo.TaskLog : "";
                string environment_variables = taskStatusInfo.environmentVariables != null ? taskStatusInfo.environmentVariables : "";
                int client_task_status_id = taskStatusInfo.clientStatusId;
                string start_time = taskStatusInfo.start_time != DateTime.MinValue ? taskStatusInfo.start_time.ToString() : DateTime.Now.ToString();
                string end_time = taskStatusInfo.end_time != DateTime.MinValue ? taskStatusInfo.end_time.ToString() : null;

                mLogger.Debug("Task status successfully added");

                DataSet ds = dbConnection.ExecuteQuery(string.Format(@"INSERT INTO ivp_rad_task_status (flow_id, status, log_description, environment_variables, client_task_status_id, start_time, end_time, process_id) 
                    VALUES({0}, '{1}', '{2}', '{3}', {4}, '{5}', '{6}', {7}); SELECT IDENT_CURRENT('ivp_rad_task_status');", flowId, status, log_description, environment_variables, client_task_status_id, start_time, end_time, Process.GetCurrentProcess().Id), RQueryType.Select);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }
                else
                    mLogger.Debug("task status id not found");

                return -1;
                //return Convert.ToInt32(dbConnection.ExecuteQuery("CTM:InsertTaskStatus", htParams).Tables[0].Rows[0][0]);

            }
            catch (Exception ex)
            {
                mLogger.Error("AddTaskStatus>>Error while adding task status with clientTaskMasterId:" + flow.task_master_id);
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }



        }
        public int AddTaskStatus(int clientTaskMasterId, int moduleId, TaskStatusInfo taskStatusInfo)
        {

            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            try
            {
                mLogger.Debug("fetching flow id with task master id:" + clientTaskMasterId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                htParams = new RHashlist();
                htParams.Add("task_master_id", clientTaskMasterId);
                htParams.Add("module_id", moduleId);
                int flowId = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetTopFlowIdByTaskMasterId", htParams).Tables[0].Rows[0][0]);
                mLogger.Debug("got flow id >> " + flowId);

                mLogger.Debug("Adding task status to db >> " + taskStatusInfo);
                htParams = new RHashlist();
                htParams.Add("flow_id", flowId);
                htParams.Add("status", taskStatusInfo.Status == TaskStatus.FAILED ? "FAILED" : taskStatusInfo.Status == TaskStatus.PASSED ? "PASSED" : "INPROGRESS");
                htParams.Add("log_description", taskStatusInfo.TaskLog != null ? taskStatusInfo.TaskLog : "");
                htParams.Add("environment_variables", taskStatusInfo.environmentVariables != null ? taskStatusInfo.environmentVariables : "");
                htParams.Add("client_task_status_id", taskStatusInfo.clientStatusId);
                htParams.Add("start_time", taskStatusInfo.start_time != DateTime.MinValue ? taskStatusInfo.start_time.ToString() : DateTime.Now.ToString());
                htParams.Add("end_time", taskStatusInfo.end_time != DateTime.MinValue ? taskStatusInfo.end_time.ToString() : null);

                mLogger.Debug("Task status successfully added");
                return Convert.ToInt32(dbConnection.ExecuteQuery("CTM:InsertTaskStatus", htParams).Tables[0].Rows[0][0]);




            }
            catch (Exception ex)
            {
                mLogger.Error("AddTaskStatus>>Error while adding task status with clientTaskMasterId:" + clientTaskMasterId);
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }

        public List<int> getAllConfiguredTasksClientTaskMasterIds(int module_id)
        {
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = null;
            mLogger.Debug("fetching all configured tasks module id by module id " + module_id);
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                htParams = new RHashlist();
                htParams.Add("module_id", module_id);
                var table = dbConnection.ExecuteQuery("CTM:GetConfiguredTaskMasterIdsByModuleId", htParams).Tables[0];
                List<int> tmp = new List<int>();
                foreach (DataRow row in table.Rows)
                {
                    tmp.Add(Convert.ToInt32(row[0]));
                }
                return tmp;
            }
            catch (Exception ex)
            {
                mLogger.Error("Error while fetching configured task master ids in module id " + module_id);
                mLogger.Error(ex);
                throw;

            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }

        }

        public void UpdateTaskStatus(TaskStatusInfo taskStatusInfo)
        {
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = null;
            mLogger.Debug("Updating task status with task status info " + taskStatusInfo);
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                htParams = new RHashlist();
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
                mLogger.Error("UpdateTaskStatus>>Error while trying to update task status : " + taskStatusInfo.ToString());
                mLogger.Error(ex);

            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }


        }

        public void AddClientStatusId(int CTMStatusId, int clientStatusId)
        {

            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            RHashlist htParams = null;
            try
            {
                htParams = new RHashlist();

                mLogger.Debug("Adding client status id with ctmstatus id : " + CTMStatusId + " and client status id" + clientStatusId);
                htParams.Add("task_status_id", CTMStatusId);
                htParams.Add("client_task_status_id", clientStatusId);
                dbConnection.ExecuteQuery("CTM:AddClientTaskStatusId", htParams);
            }
            catch (Exception ex)
            {
                mLogger.Error("Error while adding client task status id :" + ex.ToString());

                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }

        }

        public List<TaskStatusInfo> getTaskStatusByChainGuid(string chainGuid)
        {
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            List<TaskStatusInfo> tmp = new List<TaskStatusInfo>();
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");

                htParams.Add("chain_guid", chainGuid);
                DataTable dt = dbConnection.ExecuteQuery("CTM:GetTaskStatusByChainGuid", htParams).Tables[0];
                foreach (DataRow rdr in dt.Rows)
                {
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
                    tmp.Add(new TaskStatusInfo()
                    {
                        chain_guid = rdr["chain_guid"] != DBNull.Value ? rdr["chain_guid"].ToString() : null,
                        environmentVariables = rdr["environment_variables"] != DBNull.Value ? rdr["environment_variables"].ToString() : null,
                        Status = tmpStatus,
                        StatusId = rdr["client_task_status_id"] != DBNull.Value ? Convert.ToInt32(rdr["client_task_status_id"]) : 0,
                        TaskLog = rdr["log_description"] != DBNull.Value ? rdr["log_description"].ToString() : null,
                        start_time = rdr["start_time"] != DBNull.Value ? Convert.ToDateTime(rdr["start_time"]) : DateTime.MinValue,
                        end_time = rdr["end_time"] != DBNull.Value ? Convert.ToDateTime(rdr["end_time"]) : DateTime.MinValue,
                        clientStatusId = rdr["client_task_status_id"] != DBNull.Value ? Convert.ToInt32(rdr["client_task_status_id"]) : 0
                    });
                }

                return tmp;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }

        }
        internal static List<chainInfo> GetAllToBeWatched()
        {
            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            RHashlist htParams = new RHashlist(); List<chainInfo> tmpChains = new List<chainInfo>();
            try
            {
                DataTable dt = dbConnection.ExecuteQuery("CTM:GetAllTasksToBeWatched", null).Tables[0];


                //SqlConnection con = new SqlConnection(dbConnection.ConnectionString); con.Open();
                //SqlCommand cmd = new SqlCommand("select  distinct chain_id from ivp_rad_flow where (dependent_on_id = NULL OR ltrim(rtrim(dependent_on_id)) ='') and (is_muted=0) and chain_id in(	select chain_id from ivp_rad_chained_tasks 	where replace(ltrim(rtrim(filewatcher_info)),'|','') != '' and filewatcher_info is not NULL 	)",con);
                //SqlDataReader rdr = cmd.ExecuteReader();
                //while (rdr.Read())
                foreach (DataRow rdr in dt.Rows)
                {
                    tmpChains.Add(new chainInfo(Convert.ToInt32(rdr["chain_id"])));
                }
            }
            catch (Exception ex)
            {
                //mLogger.Error("CTMService:GetTaskInfoFromFlowId>>Error while getting task info with flow id" + flowId);
                //mLogger.Error(ex);
                //return null;
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
            return tmpChains;
        }

        public static DataSet GetFailedTasks(DateTime startDate, DateTime endDate)
        {
            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            RHashlist htParams = null;
            try
            {
                mLogger.Debug("begin GetFailedTasks");
                htParams = new RHashlist();
                htParams.Add("startDate", startDate);
                htParams.Add("endDate", endDate);
                DataSet failedTask = dbConnection.ExecuteQuery("CTM:GetFailedTask", htParams);
                mLogger.Debug("end GetFailedTasks");
                return failedTask;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }

        public static DataSet GetTaskInProgress(DateTime startDate, DateTime endDate, int durationInMinute)
        {
            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            RHashlist htParams = null;
            try
            {
                mLogger.Debug("begin GetTaskInProgress");
                htParams = new RHashlist();
                htParams.Add("startDate", startDate);
                htParams.Add("endDate", endDate);
                htParams.Add("duration", durationInMinute);
                DataSet failedTask = dbConnection.ExecuteQuery("CTM:GetTaskInProgress", htParams);
                mLogger.Debug("end GetTaskInProgress");
                return failedTask;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }

        public static DataSet GetMissedScheduledTask()
        {
            RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            RHashlist htParams = null;
            try
            {
                mLogger.Debug("begin GetMissedScheduledTask");
                htParams = new RHashlist();
                DataSet failedTask = dbConnection.ExecuteQuery("CTM:GetMissedScheduledTask", htParams);
                mLogger.Debug("end GetMissedScheduledTask");
                return failedTask;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }

        public void deleteTask(int[] clientTaskMasterIds, int moduleId, string username)
        {

            List<int> flowIds = new List<int>();
            foreach (int clientTaskMasterId in clientTaskMasterIds)
            {
                flowIds.AddRange(DALAdapter.getFlowIdsByClientTaskMasterId(clientTaskMasterId, moduleId));
            }
            DALAdapter.hardDeleteFlows(flowIds, username);
            DALAdapter.hardDeleteTaskSummaryByClientTaskMasterIds(clientTaskMasterIds, moduleId, username);
        }
    }
}