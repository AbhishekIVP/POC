using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
//using ServiceReference1;
using System.Data;
using com.ivp.rad.scheduler;
using System.Text.RegularExpressions;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.common;
using com.ivp.rad.RCTMUtils;
using com.ivp.rad.RUserManagement;
using com.ivp.rad.RCommonTaskManager;

/// <summary>
/// Summary description for DALAdapter
/// </summary>
public class DALAdapter
{
    //static RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
    static string DBConnectionId = "CTMDB";//RADConfigReader.GetConfigAppSettings("CTMDBConnectionId");
    static IRLogger mLogger = RLogFactory.CreateLogger("RCTMServiceDALAdapter");
    //static RHashlist htParams = null;
    public static int GetLastRunStatus(int chainId)
    {
        RHashlist htParams = new RHashlist();
        RDBConnectionManager dbConnection = null;
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("chain_id", chainId);
            return Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetLastRunStatus", htParams).Tables[0].Rows[0][0]);
            //CTM:GetLastRunStatus
        }
        catch (Exception ex) { throw; }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }

    }

    public static int FindFlowIdOfHead(int chainId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = null;
        try
        {
            //mLogger.Debug("finding flow id of head with chain id : " + chainId);
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
            htParams = new RHashlist();
            htParams.Add("chain_id", chainId);
            int flowIdHead = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetHeadOfChainInFlowsByChainId", htParams).Tables[0].Rows[0][0]);
            //mLogger.Debug("flow id of head :" + flowIdHead);
            return flowIdHead;
        }
        catch (Exception ex)
        {
            //mLogger.Error("CTMService:FindFlowIdOfHead>>Error while finding flowId of first task in chain with chain id:" + chainId);
            //mLogger.Error(ex);
            return -1;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }

    }
    public static void updateFlows(Flow[] flows, string username)
    {
        try
        {
            RDBConnectionManager dbConnection = null;
            foreach (var flow in flows)
            {
                RHashlist htParams = new RHashlist();
                try
                {
                    dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);

                    dbConnection.ExecuteQuery(string.Format(@"update dbo.ivp_rad_flow set dependent_on_id = '{1}',timeout = {2},proceed_on_fail = {3},is_muted={4},rerun_on_fail = {5},fail_retry_duration = {6},fail_number_retry = {7},on_fail_run_task = {8},task_time_out = {9},task_second_instance_wait = {10}, task_wait_subscription_id = '{11}', last_modified_by='{12}', last_modified_on = GETDATE() where flow_id={0}", flow.flow_id, flow.dependant_on_id, flow.timeout, (flow.proceed_on_fail ? 1 : 0), (flow.is_muted ? 1 : 0), (flow.rerun_on_fail ? 1 : 0), flow.fail_retry_duration, flow.fail_number_retry, flow.on_fail_run_task, flow.task_time_out, flow.task_second_instance_wait, flow.task_wait_subscription_id, username), RQueryType.Update);
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
        }
        catch (Exception ex)
        {
            throw;
        }

    }

    public static Dictionary<int, ChainedTask> getAllChainInfo()
    {
        Dictionary<int, ChainedTask> result = new Dictionary<int, ChainedTask>();
        RDBConnectionManager dbConnection = null;
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);

            DataSet ds = dbConnection.ExecuteQuery(string.Format(@"EXEC dbo.RAD_GetChainDetailsForGrid"), RQueryType.Select);

            result = ds.Tables[0].AsEnumerable().ToDictionary(x => Convert.ToInt32(x["chain_id"]), y => new ChainedTask() { chain_id = Convert.ToInt32(y["chain_id"]), chain_last_run_status = Convert.ToInt32(y["chain_last_run_status"]), nextScheduleTime = (y["next_schedule_time"] != DBNull.Value && !y.IsNull("next_schedule_time") && !string.IsNullOrEmpty(Convert.ToString(y["next_schedule_time"]))) ? Convert.ToDateTime(y["next_schedule_time"]).ToString("yyyy-MM-dd HH:mm:ss") : null });
            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
        }
    }

    public static ChainedTask getChain(int chainId)
    {
        RHashlist htParams = new RHashlist();
        RDBConnectionManager dbConnection = null;
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("chain_id", chainId);

            DataSet ds = dbConnection.ExecuteQuery("CTM:GetChainByChainId", htParams);

            var chainTableRow = ds.Tables[0].Rows[0];
            ChainedTask taskDetail = new ChainedTask()
            {
                calendar_name = chainTableRow["calendar_name"] != DBNull.Value ? chainTableRow["calendar_name"].ToString() : "",
                chain_id = chainTableRow["chain_id"] != DBNull.Value ? Convert.ToInt32(chainTableRow["chain_id"]) : 0,
                created_by = chainTableRow["created_by"] != DBNull.Value ? chainTableRow["created_by"].ToString() : "",
                created_on = chainTableRow["created_on"] != DBNull.Value ? Convert.ToDateTime(chainTableRow["created_on"]) : new DateTime(),
                is_active = chainTableRow["is_active"] != DBNull.Value ? Convert.ToBoolean(chainTableRow["is_active"]) : true,
                last_modified_by = chainTableRow["last_modified_by"] != DBNull.Value ? chainTableRow["last_modified_by"].ToString() : "",
                scheduled_job_id = chainTableRow["scheduled_job_id"] != DBNull.Value ? Convert.ToInt32(chainTableRow["scheduled_job_id"]) : -1,
                allow_parallel = chainTableRow["allow_parallel"] != DBNull.Value ? Convert.ToBoolean(chainTableRow["allow_parallel"]) : false,
                max_parallel_instances_allowed = chainTableRow["max_parallel_instances_allowed"] != DBNull.Value ? Convert.ToInt32(chainTableRow["max_parallel_instances_allowed"]) : 0,
                filewatcher_info = chainTableRow["filewatcher_info"] != DBNull.Value ? chainTableRow["filewatcher_info"].ToString() : "",
                chain_name = chainTableRow["chain_name"] != DBNull.Value ? chainTableRow["chain_name"].ToString() : "",
                triggerAsOfDateInfo = new TriggerAsOfDateInfo()
                {
                    triggerDate = chainTableRow["trigger_as_of_date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(chainTableRow["trigger_as_of_date"]) : null,
                    customValue = chainTableRow["trigger_as_of_date_info"] != DBNull.Value ? (chainTableRow["trigger_as_of_date_info"]).ToString() : null,
                },
                chain_second_instance_wait = (chainTableRow["chain_second_instance_wait"] != DBNull.Value && !chainTableRow.IsNull("chain_second_instance_wait")) ? Convert.ToInt32(chainTableRow["chain_second_instance_wait"]) : 0,
                inprogress_subscribers = chainTableRow["inprogress_subscribers"] != DBNull.Value ? chainTableRow["inprogress_subscribers"].ToString() : "",
                //= chainTableRow["trigger_as_of_date"]!=DBNull.Value?Convert.ToDateTime( chainTableRow["trigger_as_of_date"]):null,


            };
            return taskDetail;
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

    public static int getTaskSummaryByTaskMasterId(int taskMasterId, int moduleId)
    {
        RDBConnectionManager dbConnection = null; RHashlist htParams = null;
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams = new RHashlist();

            htParams.Add("task_master_id ", taskMasterId);
            htParams.Add("module_id", moduleId);
            //htParams.Add("registered_module_id", flow.registered_module_id);
            //htParams.Add("is_visible_on_ctm", flow.is_visible_on_ctm); 
            return Convert.ToInt32((dbConnection.ExecuteQuery("CTM:GetTaskSummaryByTaskMasterId", htParams).Tables[0].Rows[0][0]));
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

    public static int addFlow(Flow flow, string prevDependantId, string username)
    {
        RDBConnectionManager dbConnection = null; RHashlist htParams = null;
        try
        {
            Object ChainId; if (flow.chain_id == 0) { ChainId = null; } else { ChainId = flow.chain_id; };
            mLogger.Debug("Adding flow :" + flow.ToString());
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams = new RHashlist();

            htParams.Add("task_summary_id ", flow.task_summary_id);
            htParams.Add("dependent_on_id ", prevDependantId);
            htParams.Add("chain_id ", ChainId);
            htParams.Add("timeout ", flow.timeout);
            htParams.Add("subscribe_id ", flow.subscribe_id);
            htParams.Add("proceed_on_fail ", flow.proceed_on_fail);
            htParams.Add("rerun_on_fail ", flow.rerun_on_fail);
            htParams.Add("fail_retry_duration ", flow.fail_retry_duration);
            htParams.Add("fail_number_retry ", flow.fail_number_retry);
            htParams.Add("on_fail_run_task ", flow.on_fail_run_task);
            htParams.Add("module_id ", flow.module_id);
            htParams.Add("module_name ", flow.module_name);
            htParams.Add("task_name ", flow.task_name);
            htParams.Add("task_type_name ", flow.task_type_name);
            htParams.Add("undo_supported", flow.undo_supported);
            htParams.Add("registered_module_id", flow.registered_module_id);
            htParams.Add("task_master_id", flow.task_master_id);
            htParams.Add("username", username);

            /*
				* declare @task_summary_id int, @task_name varchar(200), @task_type_name varchar (200),@registered_module_id int, @module_id, @module_name;
				* if {0} != 0   
				* begin  
				*  Select   @task_summary_id  = task_summary_id, @task_name = task_name, @task_type_name = task_type_name, @registered_module_id =registered_module_id, @module_id = module_id, @module_name=module_name from ivp_rad_task_summary where task_master_id = {16} and module_id = {10};  
				* end  
				* else 
				* begin   
				*  Select  @task_summary_id = {0} , @task_name = {12}, @task_type_name = {13} ,@registered_module_id ={15}, @module_id={10}, @module_name={11} 
				* end      
				*  insert into [RADCommonTaskManager].[dbo].[ivp_rad_flow](  task_summary_id,  dependent_on_id,chain_id,timeout,  subscribe_id,proceed_on_fail,rerun_on_fail,fail_retry_duration,  fail_number_retry,on_fail_run_task,module_id,module_name,task_name,task_type_name,undo_supported,registered_module_id) 
				*  values(@task_summary_id,{1},{2},{3},{4},{5},{6},{7},{8},{9},@module_id,@module_name,@task_name,@task_type_name,{14},@registered_module_id) select @@IDENTITY*/
            return Convert.ToInt32((dbConnection.ExecuteQuery("CTM:AddFlow", htParams).Tables[0].Rows[0][0]));
        }
        catch (Exception ex)
        {
            mLogger.Error("Error while adding flow : " + ex);
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
    }

    public static int addChain(ChainedTask chainedTask, Flow[] flows)
    {
        int currentChainId;
        try
        {
            RDBConnectionManager dbConnection = null; RHashlist htParams = null;

            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);

                currentChainId = Convert.ToInt32(dbConnection.ExecuteQuery(string.Format(@"insert into [ivp_rad_chained_tasks] (calendar_name,is_active,created_by,created_on,last_modified_by,scheduled_job_id,timeout,chain_name,chain_second_instance_wait,inprogress_subscribers,last_modified_on) values('{0}',{1},'{2}','{3}','{4}',{5},{6},'{7}',{8},'{9}',GETDATE()) select @@IDENTITY", chainedTask.calendar_name, (chainedTask.is_active ? 1 : 0), chainedTask.created_by, chainedTask.created_on, chainedTask.last_modified_by, chainedTask.scheduled_job_id, 0, chainedTask.chain_name, chainedTask.chain_second_instance_wait, chainedTask.inprogress_subscribers), RQueryType.Select).Tables[0].Rows[0][0]);
            }
            catch (Exception ex) { throw; }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
            string prevDependantId = "";
            foreach (Flow flow in flows)
            {
                flow.chain_id = currentChainId;
                prevDependantId = "&" + addFlow(flow, prevDependantId, chainedTask.last_modified_by).ToString();
                //try
                //{  dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
                //htParams = new RHashlist();
                //    htParams.Add("task_summary_id ", flow.task_summary_id);
                //    htParams.Add("dependent_on_id ", prevDependantId.ToString());
                //    htParams.Add("chain_id ", currentChainId);
                //    htParams.Add("timeout ", flow.timeout);
                //    htParams.Add("subscribe_id ", flow.subscribe_id);
                //    htParams.Add("proceed_on_fail ", flow.proceed_on_fail);
                //    htParams.Add("rerun_on_fail ", flow.rerun_on_fail);
                //    htParams.Add("fail_retry_duration ", flow.fail_retry_duration);
                //    htParams.Add("fail_number_retry ", flow.fail_number_retry);
                //    htParams.Add("on_fail_run_task ", flow.on_fail_run_task);
                //    htParams.Add("module_id ", flow.module_id);
                //    htParams.Add("module_name ", flow.module_name);
                //    htParams.Add("task_name ", flow.task_name);
                //    htParams.Add("task_type_name ", flow.task_type_name);
                //    htParams.Add("undo_supported", flow.undo_supported);
                //    //htParams.Add("registered_module_id", flow.registered_module_id);
                //    //htParams.Add("is_visible_on_ctm", flow.is_visible_on_ctm);
                //    prevDependantId = "&" + (dbConnection.ExecuteQuery("CTM:AddFlow", htParams).Tables[0].Rows[0][0]).ToString();
                //}
                //catch (Exception ex)
                //{
                //    throw;
                //}
                //finally
                //{
                //    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                //    if (htParams != null)
                //        htParams = null;
                //}
            }
        }
        catch (Exception ex)
        {
            throw;

        }

        return currentChainId;

    }

    public static List<int> getFlowIdsByClientTaskMasterId(int clientTaskMasterId, int moduleId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            List<int> flowIds = new List<int>();
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("task_master_id", clientTaskMasterId);
            htParams.Add("module_id", moduleId);
            foreach (DataRow row in dbConnection.ExecuteQuery("CTM:GetFlowIdsByClientTaskMasterId", htParams).Tables[0].Rows)
            {
                flowIds.Add(Convert.ToInt32(row[0]));
            }
            return flowIds;
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

    public static List<string> getChainNamesByClientTaskMasterId(int clientTaskMasterId, int moduleId)
    {
        RDBConnectionManager dbConnection = null;
        mLogger.Debug("getChainNamesByClientTaskMasterId -> Start");
        try
        {
            List<string> lstChains = new List<string>();

            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);

            DataTable dtChains = dbConnection.ExecuteQuery(string.Format(@"SELECT DISTINCT ctask.chain_name
            FROM dbo.ivp_rad_task_summary summ
            INNER JOIN dbo.ivp_rad_flow flow
            ON summ.task_summary_id = flow.task_summary_id AND summ.task_master_id = {0} AND summ.module_id = {1} AND flow.is_active = 1
            INNER JOIN dbo.ivp_rad_chained_tasks ctask
            ON ctask.chain_id = flow.chain_id AND ctask.is_active = 1", clientTaskMasterId, moduleId), RQueryType.Select).Tables[0];

            foreach (DataRow row in dtChains.Rows)
            {
                lstChains.Add(Convert.ToString(row["chain_name"]));
            }
            return lstChains;
        }
        catch (Exception ex)
        {
            mLogger.Error(ex.ToString());
            throw;
        }
        finally
        {
            mLogger.Debug("getChainNamesByClientTaskMasterId -> End");
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
        }
    }

    public static void hardDeleteTaskSummaryByClientTaskMasterIds(int[] clientTaskMasterIds, int moduleId, string username)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = null;
        foreach (int clientTaskMasterId in clientTaskMasterIds)
        {
            try
            {
                htParams = new RHashlist();
                mLogger.Error("Deleting for task_master_id" + clientTaskMasterId + " for module " + moduleId);
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
                htParams.Add("task_master_id", clientTaskMasterId);
                htParams.Add("module_id", moduleId);
                //dbConnection.ExecuteQuery("CTM:HardDeleteTaskSummaryByClientTaskMasterId", htParams);
                dbConnection.ExecuteQuery(string.Format(@"DECLARE @EncodedUser VARBINARY(128);SET @EncodedUser = CONVERT(VARBINARY(128),CONVERT(CHAR(128), '{2}')); SET CONTEXT_INFO @EncodedUser; Delete from ivp_rad_task_summary where task_master_id = {0} and module_id = {1}", clientTaskMasterId, moduleId, username), RQueryType.Select);

            }
            catch (Exception ex)
            {
                mLogger.Error("Error while deleting from task summary with task master id " + clientTaskMasterId);
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            }
        }
    }

    public static void hardDeleteTaskStatusByFlowId(int flowId)
    {

        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();

        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);



            htParams.Add("flow_id", flowId);
            dbConnection.ExecuteQuery("CTM:HardDeleteTaskStatusByFlowId", htParams);

        }
        catch (Exception ex)
        {
            mLogger.Error("Error while hardDeleteTaskStatusByFlowId with flow id " + flowId);
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
    }

    public static void hardDeleteFlows(List<int> flows, string username)
    {

        foreach (var flow in flows)
        {
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();

            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
                int flowId = Convert.ToInt32(flow);

                hardDeleteTaskStatusByFlowId(flowId);
                //var depOn = FindAllDependantsOn(flowId,getChainId(flowId));

                foreach (int depFlowId in FindAllDependantsOn(flowId, getChainId(flowId)))
                {
                    appendDependant(depFlowId, getDependants(flowId), flow, username);
                }

                htParams.Add("flow_id", flowId);
                dbConnection.ExecuteQuery(string.Format(@"DECLARE @EncodedUser VARBINARY(128);SET @EncodedUser = CONVERT(VARBINARY(128),CONVERT(CHAR(128), '{1}')); SET CONTEXT_INFO @EncodedUser; delete from ivp_rad_flow where flow_id = {0}", flowId, username), RQueryType.Select);
            }
            catch (Exception ex)
            {
                mLogger.Error("Error while hardDeleteFlows with flow id " + flow);
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }
    }


    public static void deleteFlows(List<int> flows, string username)
    {

        foreach (var flow in flows)
        {
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
                int flowId = Convert.ToInt32(flow);

                //var depOn = FindAllDependantsOn(flowId,getChainId(flowId));

                foreach (int depFlowId in FindAllDependantsOn(flowId, getChainId(flowId)))
                {
                    appendDependant(depFlowId, getDependants(flowId), flow, username);
                }
                htParams.Add("flow_id", flowId);
                dbConnection.ExecuteQuery(string.Format(@"update [ivp_rad_flow] set is_active=0, last_modified_by = '{1}' , last_modified_on = GETDATE() where flow_id={0}", flowId, username), RQueryType.Update);
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
    }

    private static void appendDependant(int depFlowId, string dependants, int deletedFlowId, string username)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            var deps = getDependants(depFlowId) + "," + dependants;
            if (deps.Contains(','))
            {
                var tmp = new List<String>(deps.Split(','));
                tmp.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                string deleted = "";
                foreach (String str in tmp)
                {
                    if (str.Substring(1) == deletedFlowId.ToString())
                    {
                        deleted = str;
                    }
                }
                tmp.Remove(deleted);
                deps = string.Join(",", tmp);
            }

            htParams.Add("flow_id", depFlowId);
            htParams.Add("dependent_on_id", deps);
            if (username != null)
                dbConnection.ExecuteQuery(string.Format(@"update ivp_rad_flow set dependent_on_id = '{1}', last_modified_by = '{2}' , last_modified_on = GETDATE() where flow_id = {0}", depFlowId, deps, username), RQueryType.Update);
            else
                dbConnection.ExecuteQuery(string.Format(@"update ivp_rad_flow set dependent_on_id = '{1}' where flow_id = {0}", depFlowId, deps), RQueryType.Update);
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

    private static string getDependants(int flowId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("flow_id", flowId);
            return dbConnection.ExecuteQuery("CTM:GetFlowByFlowId", htParams).Tables[0].Rows[0]["dependent_on_id"].ToString();
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

    public static Flow getFlowByFlowId(int flowId)
    {

        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {

            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("flow_id", flowId);
            return dataRowToFlow(dbConnection.ExecuteQuery("CTM:GetFlowByFlowId", htParams).Tables[0].Rows[0]);
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

    public static Flow getDeletedFlowByFlowId(int flowId)
    {

        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {

            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("flow_id", flowId);
            return dataRowToFlow(dbConnection.ExecuteQuery("CTM:GetDeletedFlowByFlowId", htParams).Tables[0].Rows[0]);
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

    private static int getChainId(int flowId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("flow_id", flowId);
            int chainId = -1;
            DataSet dsResult = dbConnection.ExecuteQuery("CTM:GetFlowByFlowId", htParams);
            if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0] != null && dsResult.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dsResult.Tables[0].Rows[0]["chain_id"])))
                chainId = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetFlowByFlowId", htParams).Tables[0].Rows[0]["chain_id"]);
            return chainId;
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

    private static List<int> FindAllDependantsOn(int flowId, int chainId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);

            htParams.Add("chain_id", chainId);
            DataTable st = dbConnection.ExecuteQuery("CTM:GetAllDependentsOn", htParams).Tables[0];

            List<int> dependantsOn = new List<int>();
            //SqlConnection con = new SqlConnection(conn); con.Open();
            //SqlCommand cmd = new SqlCommand("select flow_id , dependent_on_id from ivp_rad_flow where chainId = "+chainId, con);
            //SqlDataReader rdr = cmd.ExecuteReader();
            foreach (DataRow rdr in st.Rows)
            {
                int tmpFLowId = Convert.ToInt32(rdr[0]);
                string[] tmpDepArr = (rdr[1].ToString()).Split(new char[] { ',' });
                if (tmpDepArr.Length > 0)
                {
                    foreach (string tmpDep in tmpDepArr)
                    {
                        if (!string.IsNullOrWhiteSpace(tmpDep))
                        {
                            if (Convert.ToInt32(Regex.Replace(tmpDep, @"[&O]", "")) == flowId)
                            {
                                dependantsOn.Add(tmpFLowId);
                                break;

                            }
                        }

                    }
                }
            }
            return dependantsOn;
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

    public static Dictionary<string, TaskSummary[]> getAllChainableTasks()
    {
        RDBConnectionManager dbConnection = null;
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            Dictionary<string, TaskSummary[]> chainableTasks = new Dictionary<string, TaskSummary[]>();

            //DataSet dataSet = dbConnection.ExecuteQuery("CTM:GetAllTaskSummaries", null);
            DataRowCollection clients = dbConnection.ExecuteQuery("CTM:GetAvailableClientNames", null).Tables[0].Rows;
            foreach (DataRow row in clients)
            {
                RDBConnectionManager dbConnection2 = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                RHashlist htParams = new RHashlist();
                try
                {

                    htParams.Add("module_name", row["module_name"].ToString());
                    //chainableTasks[row["module_name"].ToString()] = ConvertToTaskSummaryArray(dbConnection2.ExecuteQuery(string.Format(@"select * from [ivp_rad_task_summary] where module_name= 
                    //    '{0}' and is_visible_on_ctm = 1 and task_type_name not in ('Create Security', 'Update Attributes','RealTime Security Creation/Updation') order by task_name", row["module_name"].ToString()), RQueryType.Select).Tables[0].Rows);

                    chainableTasks[row["module_name"].ToString()] = ConvertToTaskSummaryArray(dbConnection2.ExecuteQuery(string.Format(@"select ts1.* 
                                                                    from dbo.ivp_rad_task_summary ts1 
                                                                    inner join (
				                                                                    select max(task_summary_id) as id 
				                                                                    from dbo.ivp_rad_task_summary
				                                                                    where module_name=  '{0}' and is_visible_on_ctm = 1 and task_type_name not in ('Create Security', 'Update Attributes','RealTime Security Creation/Updation')
				                                                                    group by task_name, task_type_name
                                                                    ) mas ON ts1.task_summary_id = mas.id order by ts1.task_name", row["module_name"].ToString()), RQueryType.Select).Tables[0].Rows);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection2);
                    if (htParams != null)
                        htParams = null;
                }
            }

            return chainableTasks;

        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);

        }
    }

    private static TaskSummary[] ConvertToTaskSummaryArray(DataRowCollection dataRowCollection)
    {
        List<TaskSummary> taskSummaries = new List<TaskSummary>();
        foreach (DataRow row in dataRowCollection)
        {
            taskSummaries.Add(ConvertDataRowToTaskSummary(row));
        }
        return taskSummaries.ToArray();

    }

    private static TaskSummary ConvertDataRowToTaskSummary(DataRow row)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            //dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            //htParams.Add("task_type_id", row["task_type_id"].ToString());
            return new TaskSummary()
            {


                created_by = row["created_by"] != DBNull.Value ? row["created_by"].ToString() : "",
                created_on = row["created_on"] != DBNull.Value ? Convert.ToDateTime(row["created_on"]) : new DateTime(),
                last_modified_by = row["last_modified_by"] != DBNull.Value ? row["last_modified_by"].ToString() : "",
                task_description = row["task_description"] != DBNull.Value ? row["task_description"].ToString() : "",
                task_summary_id = row["task_summary_id"] != DBNull.Value ? Convert.ToInt32(row["task_summary_id"]) : -1,
                task_name = row["task_name"] != DBNull.Value ? row["task_name"].ToString() : "",
                //task_type_id = Convert.ToInt32(row["task_type_id"]),
                task_type_name = row["task_type_name"] != DBNull.Value ? row["task_type_name"].ToString() : ""//dbConnection.ExecuteQuery("CTM:GetTaskTypeNameByTaskTypeId", htParams).Tables[0].Rows[0][0].ToString()

            };
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

    public static List<Flow> getFlows()
    {
        RDBConnectionManager dbConnection = null;

        try
        {
            List<TaskStatusInfo> lastRunTaskStatuses = getAllLastRunTaskStatuses();
            Dictionary<int, TaskStatusInfo> dictlastRunTaskStatuses = new Dictionary<int, TaskStatusInfo>();
            if (lastRunTaskStatuses != null && lastRunTaskStatuses.Count > 0)
            {
                foreach (var item in lastRunTaskStatuses)
                {
                    dictlastRunTaskStatuses[item.flow_id] = item;
                }
            }

            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            DataSet dataSet = dbConnection.ExecuteQuery(string.Format(@"SELECT chain_name, a.flow_id, TASKSUMMARY.task_summary_id, dependent_on_id, a.chain_id, a.timeout, CASE WHEN ISNULL(a.subscribe_id,'|||||') <> '|||||' THEN a.subscribe_id ELSE NULL END AS subscribe_id, proceed_on_fail, is_muted, rerun_on_fail, fail_retry_duration, fail_number_retry, on_fail_run_task, a.module_id, a.module_name, TASKSUMMARY.task_name, TASKSUMMARY.task_type_name, url, is_retryable, is_visible_on_ctm, task_time_out, task_second_instance_wait, task_wait_subscription_id 
FROM dbo.ivp_rad_flow a  
LEFT JOIN dbo.ivp_rad_configure_screen_links b 
ON a.module_id = b.module_id AND b.task_type_name = a.task_type_name  
JOIN dbo.ivp_rad_chained_tasks c 
ON a.chain_id = c.chain_id   
JOIN dbo.ivp_rad_task_summary TASKSUMMARY 
ON a.task_summary_id = TASKSUMMARY.task_summary_id   
WHERE a.is_active=1 AND TASKSUMMARY.is_visible_on_ctm = 1 AND c.is_active = 1 AND TASKSUMMARY.task_type_name NOT IN ('Create Security', 'Update Attributes','RealTime Security Creation/Updation')
ORDER BY a.flow_id"), RQueryType.Select);
            List<Flow> flows = new List<Flow>();
            foreach (System.Data.DataRow item in dataSet.Tables[0].Rows)
            {
                Flow tmpFlow = dataRowToFlow(item);

                //List<TaskStatusInfo> tmp = lastRunTaskStatuses.FindAll(x => x.flow_id == tmpFlow.flow_id);
                if (dictlastRunTaskStatuses.ContainsKey(tmpFlow.flow_id))
                {
                    tmpFlow.lastRunTaskStatusInfo = dictlastRunTaskStatuses[tmpFlow.flow_id];
                    tmpFlow.lastRunTaskStatus = tmpFlow.lastRunTaskStatusInfo.Status.ToString();
                }
                flows.Add(tmpFlow);
            }



            return flows;
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);

        }
    }

    public static List<TaskStatusInfo> getAllLastRunTaskStatuses()
    {
        RDBConnectionManager dbConnection = null;

        try
        {
            mLogger.Debug("fetching all last run task statuses ==> Start");
            List<TaskStatusInfo> tmpLst = new List<TaskStatusInfo>();
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            System.Data.DataSet dataSet = dbConnection.ExecuteQuery(@"SELECT task_name,task_type_name,module_name,tab.task_status_id,chain_id,chain_guid,start_time,end_time,tab.flow_id,status,log_description,client_task_status_id FROM (
				SELECT flowIn.flow_id, task_name, task_type_name, module_name, chain_id, MAX(task_status_id) AS task_status_id
				FROM ivp_rad_flow flowIn
				INNER JOIN ivp_rad_task_status statusIn
				on flowIn.flow_id = statusIn.flow_id AND flowIn.is_active = 1
				GROUP BY flowIn.flow_id, task_name, task_type_name, module_name, chain_id
			) tab
			INNER JOIN ivp_rad_task_status status
			on tab.flow_id = status.flow_id AND status.task_status_id = tab.task_status_id", RQueryType.Select);
            foreach (System.Data.DataRow item in dataSet.Tables[0].Rows)
            {
                tmpLst.Add(TaskStatusInfo.dataRowToTaskStatusInfo(item));
            }
            return tmpLst;
        }
        catch (Exception ex)
        {
            mLogger.Error("Error while fetching all last run task statuses :" + ex);
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            mLogger.Debug("fetching all last run task statuses ==> End");
        }
    }

    public static Flow dataRowToFlow(DataRow item)
    {
        return new Flow()
        {
            chain_name = item["chain_name"] != DBNull.Value ? item["chain_name"].ToString() : "",
            flow_id = item["flow_id"] != DBNull.Value ? Convert.ToInt32(item["flow_id"]) : 0,
            task_summary_id = item["task_summary_id"] != DBNull.Value ? Convert.ToInt32(item["task_summary_id"]) : 0,
            dependant_on_id = item["dependent_on_id"] != DBNull.Value ? item["dependent_on_id"].ToString() : null,
            chain_id = item["chain_id"] != DBNull.Value ? Convert.ToInt32(item["chain_id"]) : 0,
            timeout = item["timeout"] != DBNull.Value ? Convert.ToInt64(item["timeout"]) : 0,
            subscribe_id = item["subscribe_id"] != DBNull.Value ? (item["subscribe_id"]).ToString() : null,
            proceed_on_fail = item["proceed_on_fail"] != DBNull.Value ? Convert.ToBoolean(item["proceed_on_fail"]) : false,
            is_muted = item["is_muted"] != DBNull.Value ? Convert.ToBoolean(item["is_muted"]) : false,
            rerun_on_fail = item["rerun_on_fail"] != DBNull.Value ? Convert.ToBoolean(item["rerun_on_fail"]) : false,
            fail_retry_duration = item["fail_retry_duration"] != DBNull.Value ? Convert.ToInt64(item["fail_retry_duration"]) : 0,
            fail_number_retry = item["fail_number_retry"] != DBNull.Value ? Convert.ToInt32(item["fail_number_retry"]) : 0,
            on_fail_run_task = item["on_fail_run_task"] != DBNull.Value ? Convert.ToInt32(item["on_fail_run_task"]) : 0,
            module_id = item["module_id"] != DBNull.Value ? Convert.ToInt32(item["module_id"]) : 0,
            module_name = item["module_name"] != DBNull.Value ? item["module_name"].ToString() : null,
            task_name = item["task_name"] != DBNull.Value ? item["task_name"].ToString() : null,
            task_type_name = item["task_type_name"] != DBNull.Value ? item["task_type_name"].ToString() : null,
            configure_page_url = item["url"] != DBNull.Value ? item["url"].ToString() : null,
            is_retryable = item["is_retryable"] != DBNull.Value ? Convert.ToBoolean(item["is_retryable"]) : true,
            is_visible_on_ctm = item["is_visible_on_ctm"] != DBNull.Value ? Convert.ToBoolean(item["is_visible_on_ctm"]) : true,
            task_time_out = (item["task_time_out"] != DBNull.Value && !item.IsNull("task_time_out")) ? Convert.ToInt32(item["task_time_out"]) : 0,
            task_second_instance_wait = (item["task_second_instance_wait"] != DBNull.Value && !item.IsNull("task_second_instance_wait")) ? Convert.ToInt32(item["task_second_instance_wait"]) : 0,
            task_wait_subscription_id = item["task_wait_subscription_id"] != DBNull.Value ? item["task_wait_subscription_id"].ToString() : null
            //calendar_name = item["calendar_name"].ToString()
        };
    }


    public static int getModuleId(string moduleName)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("module_name", moduleName);
            return Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetModuleIdByModuleName", htParams).Tables[0].Rows[0][0]);
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
    public static int getRegisteredModuleId(string moduleName)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("module_name", moduleName);
            return Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetRegisteredModuleIdByModuleName", htParams).Tables[0].Rows[0][0]);
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

    public static Dictionary<int, DataRow> getModuleInfoByRegisteredModuleId(int registeredModuleId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            return dbConnection.ExecuteQuery("select * from dbo.ivp_rad_module where registered_module_id = " + registeredModuleId, RQueryType.Select).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToInt32(x["module_id"]));
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

    public static void muteFlows(int[] flows, string username)
    {
        foreach (int flowId in flows)
        {
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
                htParams.Add("flow_id", flowId);
                dbConnection.ExecuteQuery(string.Format(@"update ivp_rad_flow set is_muted = 1, last_modified_by = '{1}' , last_modified_on = GETDATE() where flow_id={0}", flowId, username), RQueryType.Update);
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
    }
    public static void unmuteFlows(int[] flows, string username)
    {
        foreach (int flowId in flows)
        {
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
                htParams.Add("flow_id", flowId);
                dbConnection.ExecuteQuery(string.Format(@"update ivp_rad_flow set is_muted = 0, last_modified_by = '{1}' , last_modified_on = GETDATE() where flow_id={0}", flowId, username), RQueryType.Update);
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
    }

    public static void deleteChain(string chainId, string username)
    {
        RDBConnectionManager dbConnection = null;

        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            int chain_id = Convert.ToInt32(chainId);
            htParams.Add("chain_id", chainId);
            dbConnection.ExecuteQuery(string.Format(@"update [ivp_rad_flow] set is_active = 0, last_modified_by = '{1}' , last_modified_on = GETDATE() where chain_id={0};update [ivp_rad_chained_tasks] set is_active=0, last_modified_by = '{1}' , last_modified_on = GETDATE()  where chain_id={0}; UPDATE sj
SET is_active = 0, last_modified_by = '{1}', last_modified_on = ctask.last_modified_on
FROM [dbo].[ivp_rad_scheduled_jobs] sj
INNER JOIN [dbo].[ivp_rad_chained_tasks] ctask ON ctask.scheduled_job_id = sj.job_id
WHERE chain_id = {0}", chainId, username), RQueryType.Update);
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

    public static string[] GetCalendarNames()
    {
        RDBConnectionManager dbConnection = null;
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            return dbConnection.ExecuteQuery("CTM:GetCalendar", new RHashlist()).Tables[0].AsEnumerable().Select(row => row.Field<string>(0)).ToArray();
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);

        }
    }

    public static int getScheduledJobId(int chainId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("chain_id", chainId);
            string scheduledJobId = Convert.ToString(dbConnection.ExecuteQuery("CTM:GetScheduledJobIdByChainId", htParams).Tables[0].Rows[0][0]);

            if (string.IsNullOrEmpty(scheduledJobId))
                return 0;
            else
                return Convert.ToInt32(scheduledJobId);
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

    public static RScheduledJobInfoWrapper getScheduledJobInfo(int scheduledJobId)
    {
        RDBConnectionManager dbConnection = null;
        try
        {

            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            var objScheduledJobManager = new RScheduledJobManager();
            com.ivp.rad.scheduler.RScheduledJobInfo infoScheduledJob = (com.ivp.rad.scheduler.RScheduledJobInfo)objScheduledJobManager.GetJobById(scheduledJobId, dbConnection);

            RScheduledJobInfoWrapper RScheduledJobInfoObj = new RScheduledJobInfoWrapper();
            RScheduledJobInfoObj.CreatedBy = infoScheduledJob.CreatedBy;
            RScheduledJobInfoObj.CreationTime = Convert.ToDateTime(infoScheduledJob.CreationTime).ToString("yyyy-MM-dd HH:mm:ss");
            RScheduledJobInfoObj.DaysInterval = infoScheduledJob.DaysInterval;
            RScheduledJobInfoObj.DaysofWeek = infoScheduledJob.DaysofWeek;
            RScheduledJobInfoObj.EndDate = Convert.ToDateTime(infoScheduledJob.EndDate).ToString("yyyy-MM-dd HH:mm:ss");
            RScheduledJobInfoObj.IsActive = infoScheduledJob.IsActive;
            RScheduledJobInfoObj.JobDescription = infoScheduledJob.JobDescription;
            RScheduledJobInfoObj.JobID = infoScheduledJob.JobID;
            RScheduledJobInfoObj.JobName = infoScheduledJob.JobName;
            RScheduledJobInfoObj.ModificationTime = Convert.ToDateTime(infoScheduledJob.ModificationTime).ToString("yyyy-MM-dd HH:mm:ss");
            RScheduledJobInfoObj.MonthInterval = infoScheduledJob.MonthInterval;
            RScheduledJobInfoObj.NextScheduleTime = Convert.ToDateTime(infoScheduledJob.NextScheduleTime).ToString("yyyy-MM-dd HH:mm:ss");
            RScheduledJobInfoObj.NoEndDate = infoScheduledJob.NoEndDate;
            RScheduledJobInfoObj.NoOfRecurrences = infoScheduledJob.NoOfRecurrences;
            RScheduledJobInfoObj.NoOfRuns = infoScheduledJob.NoOfRuns;
            RScheduledJobInfoObj.RecurrencePattern = infoScheduledJob.RecurrencePattern;
            RScheduledJobInfoObj.RecurrenceType = infoScheduledJob.RecurrenceType;
            RScheduledJobInfoObj.SchedulableJobId = infoScheduledJob.SchedulableJobId;
            RScheduledJobInfoObj.StartDate = Convert.ToDateTime(infoScheduledJob.StartDate).ToString("yyyy-MM-dd HH:mm:ss");
            RScheduledJobInfoObj.StartTime = Convert.ToDateTime(infoScheduledJob.StartTime).ToString("yyyy-MM-dd HH:mm:ss");
            RScheduledJobInfoObj.TimeIntervalOfRecurrence = infoScheduledJob.TimeIntervalOfRecurrence;
            RScheduledJobInfoObj.WeekInterval = infoScheduledJob.WeekInterval;

            return RScheduledJobInfoObj;
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
        }
        //com.ivp.rad.scheduler.RScheduledJobInfo infoScheduledJob = new com.ivp.rad.scheduler.RScheduledJobInfo

    }

    public static void SubscribeFlow(int flowId, string subscribeString, string username)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("flow_id", flowId);
            htParams.Add("subscribe_id", subscribeString);
            dbConnection.ExecuteQuery(string.Format(@"UPDATE [ivp_rad_flow] SET [subscribe_id] = '{1}' , [last_modified_by] = '{2}' , [last_modified_on] = GETDATE() WHERE [flow_id] = {0}", flowId, subscribeString, username), RQueryType.Update);
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

    public static string GetChainSubscribeString(int chainId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("chain_id", chainId);
            return (dbConnection.ExecuteQuery("CTM:GetChainSubscribeString", htParams).Tables[0].Rows[0][0]).ToString();
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

    public static void SubscribeChain(int chainId, string subscribeString, string username)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("chain_id", chainId);
            htParams.Add("subscribe_id", subscribeString);
            dbConnection.ExecuteQuery(string.Format(@"UPDATE [ivp_rad_chained_tasks] SET [subscribe_id] = '{1}', [last_modified_by] = '{2}' , [last_modified_on] = GETDATE() WHERE [chain_id] = {0}", chainId, subscribeString, username), RQueryType.Update);
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
    public static string GetAssemblyInfoByFlowId(int flowId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("flow_id", flowId);
            return (dbConnection.ExecuteQuery("CTM:GetAssemblyInfoByFlowId", htParams).Tables[0].Rows[0][0]).ToString();
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
    public static void UpdateFlowSetAssemblyInfoByFlowId(int flowId, string assemblyInfo)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("flow_id", flowId);
            htParams.Add("assembly_info", assemblyInfo);
            dbConnection.ExecuteQuery("CTM:UpdateFlowSetAssemblyInfoByFlowId", htParams);
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
    public static int AddJobToScheduledJobs(string schedulerInfo, Array selectedTasks, string username)
    {

        try
        {
            mLogger.Debug("Adding job to scheduled jobs");
            var schedulerData = (new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(schedulerInfo)) as Dictionary<String, object>;
            mLogger.Debug("finished deserializing scheduler info");
            com.ivp.rad.scheduler.RScheduledJobInfo infoScheduledJob = new com.ivp.rad.scheduler.RScheduledJobInfo();

            infoScheduledJob.JobName = selectedTasks != null ? (selectedTasks.GetValue(0) as Dictionary<String, object>)["task_name"].ToString() : "CTM";
            infoScheduledJob.JobDescription = selectedTasks != null ? (selectedTasks.GetValue(0) as Dictionary<String, object>)["task_name"].ToString() : "CTM";
            infoScheduledJob.SchedulableJobId = 176;//Test Job
            infoScheduledJob.StartDate = Convert.ToDateTime(schedulerData["startDate"]);
            infoScheduledJob.StartTime = Convert.ToDateTime(schedulerData["startTime"]);
            infoScheduledJob.RecurrenceType = schedulerData["recurrenceType"].ToString().Equals("recurring") ? true : false;
            infoScheduledJob.CreatedBy = username;
            if (infoScheduledJob.RecurrenceType)
            {
                if (string.IsNullOrEmpty(schedulerData["endDate"].ToString()) == false)
                    infoScheduledJob.EndDate = Convert.ToDateTime(schedulerData["endDate"].ToString());
                infoScheduledJob.NoEndDate = Convert.ToBoolean(schedulerData["neverEndJob"]);
                infoScheduledJob.NoOfRecurrences = Convert.ToInt32(schedulerData["numberOfRecurrence"]);
                infoScheduledJob.TimeIntervalOfRecurrence = Convert.ToInt32(schedulerData["timeIntervalOfRecurrence"]);
                int interval = Convert.ToInt32(schedulerData["interval"]);
                infoScheduledJob.RecurrencePattern = Convert.ToString(schedulerData["recurrencePattern"]);
                switch (infoScheduledJob.RecurrencePattern)
                {
                    case "daily":
                        infoScheduledJob.DaysInterval = interval;
                        break;
                    case "weekly":
                        infoScheduledJob.WeekInterval = interval;
                        infoScheduledJob.DaysofWeek = Convert.ToInt32(schedulerData["daysOfWeek"]);
                        break;
                    case "monthly":
                        infoScheduledJob.MonthInterval = interval;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                infoScheduledJob.EndDate = new DateTime();
                infoScheduledJob.NoEndDate = false;
                infoScheduledJob.NoOfRecurrences = 0;
                infoScheduledJob.TimeIntervalOfRecurrence = 0;
                infoScheduledJob.RecurrencePattern = string.Empty;
            }

            var objScheduledJobManager = new RScheduledJobManager();
            mLogger.Debug("finished adding new job. updating db");

            return objScheduledJobManager.AddJob(infoScheduledJob, RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB"));
        }
        catch (Exception ex)
        {
            mLogger.Error("Error while AddJobToScheduledJobs:" + ex.ToString());
            throw;
        }
    }


    public static void UpdateChainScheduledJobId(int chainId, int scheduledJobId, string username)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        try
        {
            mLogger.Debug("Updating chain scheduled job id with chain id:" + chainId + " scheduled job id :" + scheduledJobId);
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("chain_id", chainId);
            htParams.Add("scheduled_job_id", scheduledJobId);
            dbConnection.ExecuteQuery("update ivp_rad_chained_tasks set scheduled_job_id = " + scheduledJobId + ", last_modified_on = GETDATE(), last_modified_by = '" + username + "' where chain_id = " + chainId, RQueryType.Update);
        }
        catch (Exception ex)
        {
            mLogger.Error("Error while updating chain" + ex);
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
    }


    public static void UpdateChain(int chainId, ChainedTask chainedTask, string username)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        mLogger.Debug("updateing chain with chain id " + chainId + " and chain info :" + chainedTask);
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            dbConnection.ExecuteQuery(string.Format(@"update ivp_rad_chained_tasks set calendar_name='{1}',allow_parallel={2},max_parallel_instances_allowed={3},filewatcher_info='{4}',trigger_as_of_date='{5}',trigger_as_of_date_info='{6}',chain_second_instance_wait={7}, inprogress_subscribers='{8}' , last_modified_by='{9}' ,last_modified_on = GETDATE() where chain_id={0}", chainId, chainedTask.calendar_name, (chainedTask.allow_parallel ? 1 : 0), chainedTask.max_parallel_instances_allowed, chainedTask.filewatcher_info, chainedTask.triggerAsOfDateInfo.triggerDate, chainedTask.triggerAsOfDateInfo.customValue.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Length == 0 ? null : chainedTask.triggerAsOfDateInfo.customValue, chainedTask.chain_second_instance_wait, chainedTask.inprogress_subscribers, username), RQueryType.Update);

        }
        catch (Exception ex)
        {
            mLogger.Error("Error while updating chain " + ex.ToString());
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
    }

    public static List<string> GetDistinctTaskNames()
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        List<string> tmp = new List<string>();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            //htParams.Add("flow_id", flowId);
            DataTable st = dbConnection.ExecuteQuery(@"SELECT DISTINCT task_type_name FROM dbo.ivp_rad_task_summary WHERE task_type_name NOT IN ('Time Series Correction', 'Create Security', 'Update Attributes','RealTime Security Creation/Updation','Undo')  UNION  
            SELECT DISTINCT task_type_name FROM dbo.ivp_rad_task_summary WHERE task_type_name = 'Time Series Correction' AND is_visible_on_ctm = 1 ORDER BY task_type_name", RQueryType.Select).Tables[0];

            foreach (DataRow rdr in st.Rows)
            {
                if (rdr[0].ToString().ToLower() != "create security" && rdr[0].ToString().ToLower() != "update attributes" && rdr[0].ToString().ToLower() != "realtime security creation/updation" && rdr[0].ToString().ToLower() != "undo")
                    tmp.Add(rdr[0].ToString());
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

    public static List<string> GetDistinctModuleNames()
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        List<string> tmp = new List<string>();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            //htParams.Add("flow_id", flowId);
            DataTable st = dbConnection.ExecuteQuery("CTM:GetDistinctModuleNames", null).Tables[0];
            foreach (DataRow rdr in st.Rows)
            {
                tmp.Add(rdr[0].ToString());
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
    public static List<int> GetAvailableModuleIds()
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        List<int> tmp = new List<int>();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            //htParams.Add("chain_id", chainId);
            DataTable st = dbConnection.ExecuteQuery("CTM:GetAvailableModuleIds", null).Tables[0];
            foreach (DataRow rdr in st.Rows)
            {
                tmp.Add(Convert.ToInt32(rdr[0]));
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
    public static string GetCurrentDateTimeFromServer(string longDateFormat)
    {
        string DateInClientFormat = null;
        try
        {
            DateTime Now = DateTime.Now;
            DateInClientFormat = Now.ToString(longDateFormat);
        }
        catch (Exception ex)
        {
            throw;
        }
        return DateInClientFormat;
    }
    public static List<int> GetAllModuleIdsByChainId(int chainId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        List<int> tmp = new List<int>();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams.Add("chain_id", chainId);
            DataTable st = dbConnection.ExecuteQuery("CTM:GetDistinctModuleIdsByChainId", htParams).Tables[0];
            foreach (DataRow rdr in st.Rows)
            {
                tmp.Add(Convert.ToInt32(rdr[0]));
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

    public static DataTable GetSRMTaskTypesForTaskStatus()
    {
        RDBConnectionManager dbConnection = null;
        DataTable st = new DataTable();

        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            DataTable dt = dbConnection.ExecuteQuery("CTM:TaskTypeNameTaskTypeId", null).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
        }
    }

    //public static int addTaskstoExistingChain(string chainId, Flow[] flows, int lastFlowId)
    //{
    //    int currentChainId;
    //    try
    //    {
    //        RDBConnectionManager dbConnection = null; 
    //        RHashlist htParams = null;

    //        try
    //        {
    //            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);

    //            htParams = new RHashlist();

    //            htParams.Add("chain_id", chainId);
    //            dbConnection.ExecuteQuery("CTM:DeactivateAllTasksInChain", htParams);
    //        }
    //        catch (Exception ex) { throw; }
    //        finally
    //        {
    //            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
    //            if (htParams != null)
    //                htParams = null;
    //        }
    //        string prevDependantId = "";
    //        foreach (Flow flow in flows)
    //        {

    //            try
    //            {
    //                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
    //                htParams = new RHashlist();
    //                htParams.Add("task_summary_id ", flow.task_summary_id);
    //                htParams.Add("dependent_on_id ", prevDependantId.ToString());
    //                htParams.Add("chain_id ", chainId);
    //                htParams.Add("timeout ", flow.timeout);
    //                htParams.Add("subscribe_id ", flow.subscribe_id);
    //                htParams.Add("proceed_on_fail ", flow.proceed_on_fail);
    //                htParams.Add("rerun_on_fail ", flow.rerun_on_fail);
    //                htParams.Add("fail_retry_duration ", flow.fail_retry_duration);
    //                htParams.Add("fail_number_retry ", flow.fail_number_retry);
    //                htParams.Add("on_fail_run_task ", flow.on_fail_run_task);
    //                htParams.Add("module_id ", flow.module_id);
    //                htParams.Add("module_name ", flow.module_name);
    //                htParams.Add("task_name ", flow.task_name);
    //                htParams.Add("task_type_name ", flow.task_type_name);
    //                htParams.Add("undo_supported", flow.undo_supported);
    //                prevDependantId = "&" + (dbConnection.ExecuteQuery("CTM:AddFlow", htParams).Tables[0].Rows[0][0]).ToString();
    //            }
    //            catch (Exception ex)
    //            {
    //                throw;
    //            }
    //            finally
    //            {
    //                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
    //                if (htParams != null)
    //                    htParams = null;
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw;

    //    }

    //    return Convert.ToInt32(chainId);
    //}

    internal static List<Flow> getAllTasksInChain(int chainId)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = null;
        try
        {
            List<Flow> tmp = new List<Flow>();
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams = new RHashlist();
            htParams.Add("chain_id ", chainId);
            DataTable table = dbConnection.ExecuteQuery("CTM:GetAllFlowsInChain", htParams).Tables[0];
            foreach (DataRow item in table.Rows)
            {
                tmp.Add(//(dataRowToFlow(item)));

                    new Flow()
                    {
                        chain_name = item["chain_name"] != DBNull.Value ? item["chain_name"].ToString() : "",
                        flow_id = item["flow_id"] != DBNull.Value ? Convert.ToInt32(item["flow_id"]) : 0,
                        task_summary_id = item["task_summary_id"] != DBNull.Value ? Convert.ToInt32(item["task_summary_id"]) : 0,
                        dependant_on_id = item["dependent_on_id"] != DBNull.Value ? item["dependent_on_id"].ToString() : null,
                        chain_id = item["chain_id"] != DBNull.Value ? Convert.ToInt32(item["chain_id"]) : 0,
                        timeout = item["timeout"] != DBNull.Value ? Convert.ToInt64(item["timeout"]) : 0,
                        subscribe_id = item["subscribe_id"] != DBNull.Value ? (item["subscribe_id"]).ToString() : null,
                        proceed_on_fail = item["proceed_on_fail"] != DBNull.Value ? Convert.ToBoolean(item["proceed_on_fail"]) : false,
                        is_muted = item["is_muted"] != DBNull.Value ? Convert.ToBoolean(item["is_muted"]) : false,
                        rerun_on_fail = item["rerun_on_fail"] != DBNull.Value ? Convert.ToBoolean(item["rerun_on_fail"]) : false,
                        fail_retry_duration = item["fail_retry_duration"] != DBNull.Value ? Convert.ToInt64(item["fail_retry_duration"]) : 0,
                        fail_number_retry = item["fail_number_retry"] != DBNull.Value ? Convert.ToInt32(item["fail_number_retry"]) : 0,
                        on_fail_run_task = item["on_fail_run_task"] != DBNull.Value ? Convert.ToInt32(item["on_fail_run_task"]) : 0,
                        module_id = item["module_id"] != DBNull.Value ? Convert.ToInt32(item["module_id"]) : 0,
                        module_name = item["module_name"] != DBNull.Value ? item["module_name"].ToString() : null,
                        task_name = item["task_name"] != DBNull.Value ? item["task_name"].ToString() : null,
                        task_type_name = item["task_type_name"] != DBNull.Value ? item["task_type_name"].ToString() : null,
                        //configure_page_url = item["url"] != DBNull.Value ? item["url"].ToString() : null
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

    public static void AddTasksToExistingChain(int chainId, List<Flow> flows, string username)//,int lastTaskId)
    {
        try
        {
            RDBConnectionManager dbConnection = null; RHashlist htParams = null;

            string prevDependantId = "";
            foreach (Flow flow in flows)
            {

                try
                {
                    dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
                    htParams = new RHashlist();
                    htParams.Add("task_summary_id ", flow.task_summary_id);
                    htParams.Add("dependent_on_id ", prevDependantId.ToString());
                    htParams.Add("chain_id ", chainId);
                    htParams.Add("timeout ", flow.timeout);
                    htParams.Add("subscribe_id ", flow.subscribe_id);
                    htParams.Add("proceed_on_fail ", flow.proceed_on_fail);
                    htParams.Add("rerun_on_fail ", flow.rerun_on_fail);
                    htParams.Add("fail_retry_duration ", flow.fail_retry_duration);
                    htParams.Add("fail_number_retry ", flow.fail_number_retry);
                    htParams.Add("on_fail_run_task ", flow.on_fail_run_task);
                    htParams.Add("module_id ", flow.module_id);
                    htParams.Add("module_name ", flow.module_name);
                    htParams.Add("task_name ", flow.task_name);
                    htParams.Add("task_type_name ", flow.task_type_name);
                    htParams.Add("undo_supported", flow.undo_supported);
                    htParams.Add("registered_module_id", flow.registered_module_id);
                    htParams.Add("task_master_id", flow.task_master_id);
                    htParams.Add("username", username);
                    prevDependantId = "&" + (dbConnection.ExecuteQuery("CTM:AddFlow", htParams).Tables[0].Rows[0][0]).ToString();
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
        }
        catch (Exception ex)
        {
            throw;

        }

    }

    //for CTM Issue 67335 and 102137 Start

    public static void UpdateDependentIds(Array selectedTasks, int chainId, string username)
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = null;
        Flow flowId = new Flow();
        string dependentId = string.Empty;
        List<Flow> flows = getAllTasksInChain(chainId);
        dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
        htParams = new RHashlist();
        foreach (var task in selectedTasks)
        {
            htParams = new RHashlist();
            flowId = new Flow();
            flowId = flows.Find(flow => flow.task_summary_id == Convert.ToInt32((task as Dictionary<string, object>)["task_summary_id"]));
            htParams.Add("flow_id", flowId.flow_id);
            htParams.Add("dependent_on_id", dependentId);
            dbConnection.ExecuteQuery("UPDATE ivp_rad_flow set dependent_on_id = '" + dependentId + "', last_modified_on = GETDATE(), last_modified_by = '" + username + "' where flow_id = " + flowId.flow_id, RQueryType.Update);

            dependentId = "&" + flowId.flow_id;
        }
    }

    //for CTM Issue 67335 and 102137 End

    internal static void removeOldScheduledJob(int chainId, string username)
    {
        RDBConnectionManager dbConnection = null; RHashlist htParams = null;
        try
        {
            mLogger.Debug("removing old scheduled job with chain id" + chainId);
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams = new RHashlist();
            htParams.Add("chain_id", chainId);
            //dbConnection.ExecuteQuery("CTM:RemoveOldScheduledJob", htParams);
            dbConnection.ExecuteQuery("update t1 set is_active = 0, last_modified_on = GETDATE(), last_modified_by = '" + username + "' from ivp_rad_scheduled_jobs t1  join ivp_rad_chained_tasks t2 on t1.job_id = t2.scheduled_job_id where t2.chain_id = " + chainId + ";  update [ivp_rad_chained_tasks] set scheduled_job_id = -1, last_modified_on = GETDATE(), last_modified_by = '" + username + "' where chain_id = " + chainId, RQueryType.Update);
        }
        catch (Exception ex)
        {
            mLogger.Error("error while removing old scheduled job:" + ex.ToString());
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
    }



    public static List<TaskStatusInfo> GetLastRunTaskStatusesByChainId(int chainId)
    {
        RDBConnectionManager dbConnection = null; RHashlist htParams = null;
        try
        {
            mLogger.Debug("fetching last run task status" + chainId);
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams = new RHashlist();
            htParams.Add("chain_id", chainId);
            DataTable dt = dbConnection.ExecuteQuery("CTM:GetLastRunTaskStatusesByChainId", htParams).Tables[0];
            List<TaskStatusInfo> tmp = new List<TaskStatusInfo>();
            foreach (DataRow rdr in dt.Rows)
            {
                tmp.Add(TaskStatusInfo.dataRowToTaskStatusInfo(rdr));
            }
            return tmp;
        }
        catch (Exception ex)
        {
            mLogger.Error("error while fetching lastt run task status" + ex.ToString());
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
    }

    public static int GetRegisteredModuleIdByModuleId(int moduleId)
    {
        RDBConnectionManager dbConnection = null; RHashlist htParams = null;
        try
        {
            mLogger.Debug("fetching registered module id by moduleid:" + moduleId);
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams = new RHashlist();
            htParams.Add("module_id", moduleId);
            return Convert.ToInt32(dbConnection.ExecuteQuery("CTM:GetRegisteredModuleIdByModuleId", htParams).Tables[0].Rows[0][0]);

        }
        catch (Exception ex)
        {
            mLogger.Error("error while fetching registered module id" + ex.ToString());
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
    }

    public static void saveConfiguredTask(Flow taskInfo)
    {
        RDBConnectionManager dbConnection = null; RHashlist htParams = null;
        try
        {
            mLogger.Debug("saving configured task with flow info :" + taskInfo.ToString());
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams = new RHashlist();
            htParams.Add("flow_id", taskInfo.flow_id);
            htParams.Add("timeout", taskInfo.timeout);
            htParams.Add("fail_retry_duration", taskInfo.fail_retry_duration);
            htParams.Add("fail_number_retry", taskInfo.fail_number_retry);
            htParams.Add("on_fail_run_task", taskInfo.on_fail_run_task == 0 ? null : taskInfo.on_fail_run_task);
            htParams.Add("proceed_on_fail", taskInfo.proceed_on_fail);
            htParams.Add("rerun_on_fail", taskInfo.rerun_on_fail);
            htParams.Add("is_muted", taskInfo.is_muted);
            //'update ivp_rad_flow set timeout = {1}, fail_retry_duration = {2}, fail_number_retry = {3},on_fail_run_task = {4},proceed_on_fail = {5},rerun_on_fail = {6},is_muted = {7} where flow_id = {0}'
            dbConnection.ExecuteQuery("CTM:SaveConfiguredTask", htParams);

        }
        catch (Exception ex)
        {
            mLogger.Error("error while saving task");
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
    }

    //TODO
    public static void setDependancyByFlowId(string flowId, string depStr)
    {
        RDBConnectionManager dbConnection = null; RHashlist htParams = null;
        try
        {

            mLogger.Debug("saving dependancy of  configured task with flow id :" + flowId + "and dependancy str " + depStr);
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams = new RHashlist();
            htParams.Add("flow_id", flowId);
            htParams.Add("dep", depStr);

            dbConnection.ExecuteQuery("CTM:UpdateDependancyByFLowId", htParams);

        }
        catch (Exception ex)
        {
            mLogger.Error("error while saving dependancy of task");
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
    }

    #region Group Privileges
    public static List<string> GetAllGroups()
    {
        RDBConnectionManager dbConnection = null;
        RHashlist htParams = new RHashlist();
        List<string> tmp = new List<string>();
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            //htParams.Add("chain_id", chainId);
            RGroupController group = new RGroupController();
            DataSet st = group.GetAllGroups();//dbConnection.ExecuteQuery("CTM:GetAllGroups", null).Tables[0];
            foreach (DataRow rdr in st.Tables[0].Rows)
            {
                tmp.Add(Convert.ToString(rdr[0]));
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

    #endregion
    public static int addNewChain(ChainedTask chainedTask)
    {
        int currentChainId;
        RDBConnectionManager dbConnection = null; RHashlist htParams = null;
        try
        {
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);

            htParams = new RHashlist();
            htParams.Add("calendar_name", chainedTask.calendar_name);
            htParams.Add("is_active", chainedTask.is_active);
            htParams.Add("created_by", chainedTask.created_by);
            htParams.Add("created_on", chainedTask.created_on);
            htParams.Add("last_modified_by", chainedTask.last_modified_by);
            htParams.Add("scheduled_job_id", chainedTask.scheduled_job_id);
            htParams.Add("timeout", 0);
            htParams.Add("chain_name", chainedTask.chain_name);
            //htParams.Add("group_privileges", chainedTask.group_privileges);
            currentChainId = Convert.ToInt32(dbConnection.ExecuteQuery("CTM:AddChain", htParams, true).Tables[0].Rows[0][0]);
        }
        catch (Exception ex) { throw; }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
        return currentChainId;
    }

    internal static KeyValuePair<int, int> getModuleIdClientTaskMasterIdKeyValueByTaskSummaryId(int taskSummaryId)
    {
        RDBConnectionManager dbConnection = null; RHashlist htParams = null;
        try
        {
            mLogger.Debug("fetching module id and taskmasterId where tasksummaryid:" + taskSummaryId);
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            htParams = new RHashlist();
            htParams.Add("task_summary_id", taskSummaryId);
            //select module_id , task_master_id from ivp_rad_task_summary where task_summary_id = {0}
            DataRow dr = dbConnection.ExecuteQuery("CTM:GetModuleIdClientTaskMasterIdByTaskSummaryId", htParams).Tables[0].Rows[0];
            return new KeyValuePair<int, int>(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]));

        }
        catch (Exception ex)
        {
            mLogger.Error("error while fetching registered module id" + ex.ToString());
            throw;
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
    }


    internal static string UpdateChainName(int chainId, string chainName, string username)
    {
        RDBConnectionManager dbConnection = null; RHashlist htParams = null;
        try
        {
            mLogger.Debug("Updating task group name from UI for chainId : " + chainId);
            dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            dbConnection.ExecuteQuery("UPDATE dbo.ivp_rad_chained_tasks SET chain_name = '" + chainName + "', last_modified_on = GETDATE(), last_modified_by = '" + username + "' WHERE chain_id = " + chainId + "", RQueryType.Update);

        }
        catch (Exception ex)
        {
            mLogger.Error("error while fetching registered module id" + ex.ToString());
            return ex.ToString();
        }
        finally
        {
            RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
            if (htParams != null)
                htParams = null;
        }
        return string.Empty;
    }
}

