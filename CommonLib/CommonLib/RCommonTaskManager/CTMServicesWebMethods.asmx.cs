
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.ServiceModel;
//using com.ivp.rad.RCommonTaskManager;
using com.ivp.rad.RCTMUtils;
using com.ivp.rad.scheduler;
using com.ivp.rad.dal;
using System.ServiceModel.Activation;
using com.ivp.rad.common;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using com.ivp.rad.RUserManagement;
using System.Data;
using com.ivp.rad.utils;
using com.ivp.srmcommon;

namespace com.ivp.rad.RCommonTaskManager
{
    /// <summary>
    /// Summary description for CTMService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CTMServicesWebMethods : System.Web.Services.WebService, ICTMServiceCallback
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("CTMServicesWebMethods");
        [WebMethod]
        public string GetDistinctModuleNames(string clientName)
        {
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;
                mLogger.Debug("CTM: fetching distinct module names");
                string distinctModuleNames = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(DALAdapter.GetDistinctModuleNames());
                mLogger.Debug("CTM fetched the following module ids " + distinctModuleNames);
                return distinctModuleNames;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTM: Error while fetching distinct module names " + ex.ToString());
                throw;

            }

        }
        [WebMethod]
        public string GetDistinctTaskNames(string clientName)
        {
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;
                mLogger.Debug("CTM: fetching distinct task names");
                String distinctTaskNames = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(DALAdapter.GetDistinctTaskNames());
                mLogger.Debug("CTM fetched the following task names " + distinctTaskNames);
                return distinctTaskNames;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTM: Error while fetching distinct task names " + ex.ToString());
                throw;
            }

        }
        [WebMethod]
        public string GetAllModuleIdsByChainId(String chainId,string clientName)
        {
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;
                mLogger.Debug("CTM: fetching module ids with chain id " + chainId);
                string tmp = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(DALAdapter.GetAllModuleIdsByChainId(Convert.ToInt32(chainId))); ;
                return tmp;
            }
            catch (Exception ex)
            {
                mLogger.Error("CTM: error while fetching module ids " + ex.ToString());
                throw;
            }
        }

        [WebMethod]
        public string GetSubscriberList(String moduleIds,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            mLogger.Debug("CTM: fetching subscriber list with module ids " + moduleIds);
            List<int> module_ids = (List<int>)new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(moduleIds, new List<int>().GetType());
            string tmp = "";
            //Dictionary<String, List<string>> subscriptionInfo = new Dictionary<String, List<string>>();
            Dictionary<string, List<string>> subscriptionInfo = new Dictionary<string, List<string>>();
            foreach (int moduleId in module_ids)
            {
                try
                {
                    DuplexChannelFactory<ICTMService> dupFactory = null;
                    ICTMService clientProxy = null;

                    CTMServicesWebMethods cbk = new CTMServicesWebMethods();
                    dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
                    dupFactory.Open();
                    clientProxy = dupFactory.CreateChannel();
                    Dictionary<string, List<string>> subscribetmp = clientProxy.GetSubscriberList(moduleId, clientName);
                    mLogger.Debug("CTM: subscriber list received from module id " + moduleId + " >> " + subscribetmp);
                    //subscriptionInfo[moduleId.ToString()] = subscribetmp;
                    subscriptionInfo = subscribetmp;
                }
                catch (Exception ex)
                {
                    mLogger.Error("Error while fetching subscriber list from module id " + moduleId + " Details: " + ex.ToString());

                }

            }

            return new JavaScriptSerializer().Serialize(subscriptionInfo);
        }
        [WebMethod]
        public string GetAvailableModuleIds(string clientName)
        {
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;
                mLogger.Debug("CTM: fetching all available module ids");
                return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(DALAdapter.GetAvailableModuleIds());
            }
            catch (Exception ex)
            {
                mLogger.Error("CTM:Error while fetching available module ids " + ex.ToString());
                throw;
            }
        }

        [WebMethod]
        public string GetCalendarList(string moduleIds,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            List<int> module_ids = (List<int>)new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(moduleIds, new List<int>().GetType());

            string tmp = "";
            foreach (int moduleId in module_ids)
            {
                try
                {
                    DuplexChannelFactory<ICTMService> dupFactory = null;
                    ICTMService clientProxy = null;

                    CTMServicesWebMethods cbk = new CTMServicesWebMethods();
                    //dupFactory = new DuplexChannelFactory<ICTMService>(cbk, new NetTcpBinding(), new EndpointAddress(new Uri("net.tcp://ws-uberi:8182/CTMService"), EndpointIdentity.CreateUpnIdentity("uberi@ivp.in")));
                    dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
                    dupFactory.Open();
                    clientProxy = dupFactory.CreateChannel();

                    foreach (String s in clientProxy.GetCalendarList(moduleId,clientName))
                    {
                        tmp += s + ",";
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error("Error while fetching calendar list from module id " + moduleId + "Details:" + ex.ToString());
                }
            }

            return tmp;
        }
        [WebMethod]
        public string GetCurrentDateTimeFromServer(string longDateFormat,string clientName)
        {
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;
                mLogger.Debug("CTM: GetCurrentDateTimeFromServer : SATRT");
                return DALAdapter.GetCurrentDateTimeFromServer(longDateFormat);
            }
            catch (Exception ex)
            {
                mLogger.Debug("CTM: GetCurrentDateTimeFromServer : END");
                throw;
            }
        }
        #region Group Privileges
        [WebMethod]
        public string GetAllGroups(string clientName)
        {
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;
                mLogger.Debug("CTM: fetching all Groups");
                return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(DALAdapter.GetAllGroups());
            }
            catch (Exception ex)
            {
                mLogger.Error("CTM:Error while fetching Groups " + ex.ToString());
                throw;
            }
        }
        #endregion


        [WebMethod]
        public ChainedTask getChainInfo(string chainId,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            var chai = DALAdapter.getChain(Convert.ToInt32(chainId));
            return chai;
        }

        [WebMethod]
        public string testTrig(TriggerAsOfDateInfo triggerAsOfDateInfo)
        {

            return "pass";
        }
        [WebMethod]
        public void updateChain(ChainedTask chainInfo, string username,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            DALAdapter.UpdateChain(chainInfo.chain_id, chainInfo, username);

        }

        [WebMethod]
        public string GetAssemblyInfoByFlowId(string flowId,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(DALAdapter.GetAssemblyInfoByFlowId(Convert.ToInt32(flowId)));
        }
        [WebMethod]
        public void UpdateFlowSetAssemblyInfoByFlowId(string flowId, string assemblyInfo,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            DALAdapter.UpdateFlowSetAssemblyInfoByFlowId(Convert.ToInt32(flowId), assemblyInfo);
        }
        [WebMethod]
        public void SubscribeChain(string chainId, string subscribeString, string username,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            DALAdapter.SubscribeChain(Convert.ToInt32(chainId), subscribeString, username);
        }
        [WebMethod]
        public string GetChainSubscribeString(string chainId,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(DALAdapter.GetChainSubscribeString(Convert.ToInt32(chainId)));
        }

        [WebMethod]
        public void SubscribeFlow(string flowId, string subscribeString, string username,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            DALAdapter.SubscribeFlow(Convert.ToInt32(flowId), subscribeString, username);
        }
        [WebMethod]
        public string getSchedulingInfo(string scheduledJobId,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(DALAdapter.getScheduledJobInfo(Convert.ToInt32(scheduledJobId)));
        }
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string getScheduledJobId(string chainId,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(DALAdapter.getScheduledJobId(Convert.ToInt32(chainId)));
        }
        [WebMethod]
        public void UnmuteFlows(string selectedFlows, string username,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            var flows = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(selectedFlows) as Array;
            //DALAdapter.muteFlows(flows.S);
            //System.Array.ConvertAll<string, int>(flows,Convert.ToInt32);
            int[] flws = flows.OfType<string>().ToArray<string>().Select(s => int.Parse(s)).ToArray();
            DALAdapter.unmuteFlows(flws, username);
        }
        [WebMethod]
        public string GetUpdatedNextScheduledTime(String chainId,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            DateTime? next_scheduled_time = null;
            var job = (new RScheduledJobManager()).GetJobById(
                DALAdapter.getChain(Convert.ToInt32(chainId)).scheduled_job_id, RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB")
                );
            if (job != null)
            {
                next_scheduled_time = ((RScheduledJobInfo)(job)).NextScheduleTime;
            }
            return next_scheduled_time.ToString();
        }
        [WebMethod]
        public string GetGridData(string username,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            mLogger.Debug("fetching grid data");

            RUserManagementService group = new RUserManagementService();
            //RUser user = group.GetUserDetailsGDPR(username);
            //List<string> userGroups = new List<string>();
            //userGroups = user.GroupNames;
            List<Flow> flows = DALAdapter.getFlows();
            flows = flows.Where(x => x.is_visible_on_ctm == true).ToList<Flow>();
            List<FlowChain> data = new List<FlowChain>();

            //List<string> flow_group_privileges = new List<string>();
            bool userFlag = true;
            List<Flow> parentFlows = flows.Where(x => x.dependant_on_id == "" || x.dependant_on_id == null).GroupBy(x => x.chain_id).OrderBy(x => x.First().chain_name).Select(x => x.First()).ToList();
            flows.RemoveAll(x => x.dependant_on_id == "" || x.dependant_on_id == null);

            Dictionary<int, ChainedTask> DictChainedTaskInfo = DALAdapter.getAllChainInfo();

            foreach (Flow flow in parentFlows)
            {
                //flow_group_privileges = flow.group_privileges.Split(',').ToList();
                //foreach (string userGroup in userGroups)
                //{
                //    if (flow_group_privileges.Contains(userGroup))
                //    {
                //        userFlag = true;
                //    }
                //}
                if (userFlag)
                {
                    string next_scheduled_time = DictChainedTaskInfo[flow.chain_id].nextScheduleTime;
                    if (string.IsNullOrEmpty(next_scheduled_time))
                        next_scheduled_time = null;

                    if (next_scheduled_time != null && Convert.ToDateTime(next_scheduled_time).Ticks < DateTime.Now.Ticks)
                    {
                        next_scheduled_time = null;
                    }

                    data.Add(new FlowChain()
                    {
                        flow_id = flow.flow_id,
                        chain_id = flow.chain_id,
                        module_name = flow.module_name,
                        task_name = flow.task_name,
                        task_type_name = flow.task_type_name,
                        //calendar_name = flow.calendar_name,
                        trigger_type = next_scheduled_time == null ? "Manual" : "Scheduled",
                        next_scheduled_time = next_scheduled_time,
                        is_muted = flow.is_muted,
                        subscribe_id = flow.subscribe_id,
                        configure_page_url = flow.configure_page_url,
                        module_id = flow.module_id,
                        chain_name = flow.chain_name,
                        chain_last_run_status = DictChainedTaskInfo[flow.chain_id].chain_last_run_status,
                        #region Group Privileges
                        //group_privileges = flow.group_privileges,
                        #endregion
                        triggerAsOfDateInfo = new TriggerAsOfDateInfo()
                        {
                            triggerDate = flow.triggerAsOfDateInfo == null ? null : flow.triggerAsOfDateInfo.triggerDate,
                            customValue = flow.triggerAsOfDateInfo == null ? null : flow.triggerAsOfDateInfo.customValue
                        }
                    });
                }
            }

            foreach (FlowChain heads in data)
            {
                var childs = getChildrenList(heads, flows);
                heads.children.AddRange(childs.GetRange(1, childs.Count - 1));
            }
            mLogger.Debug("fetching grid data done");
            var obj = new System.Web.Script.Serialization.JavaScriptSerializer();
            obj.MaxJsonLength = Int32.MaxValue;
            return obj.Serialize(data.ToArray<Flow>());
        }




        List<FlowChain> getChildrenList(FlowChain head, List<Flow> flows)
        {
            List<Flow> AddedNode = new List<Flow>();
            List<FlowChain> lst = new List<FlowChain>();
            findAllDepsAndAddToLst(head, flows, lst, AddedNode);
            return lst;
        }

        void findAllDepsAndAddToLst(Flow node, List<Flow> flows, List<FlowChain> lst, List<Flow> addedNodes)
        {
            if (addedNodes.Contains(node) == false)
            {
                lst.Add(new FlowChain()

                {
                    flow_id = node.flow_id,
                    chain_id = node.chain_id,
                    module_name = node.module_name,
                    task_name = node.task_name,
                    task_type_name = node.task_type_name,
                    //calendar_name = node.calendar_name,
                    trigger_type = node.trigger_type,
                    is_muted = node.is_muted,
                    subscribe_id = node.subscribe_id,
                    configure_page_url = node.configure_page_url,
                    module_id = node.module_id,
                    lastRunTaskStatusInfo = node.lastRunTaskStatusInfo,
                    lastRunTaskStatus = node.lastRunTaskStatus
                });
                addedNodes.Add(node);
            }
            List<Flow> deps = findAllDepOf(node, flows);

            if (deps.Count > 0)
            {
                foreach (Flow e in deps)
                {
                    findAllDepsAndAddToLst(e, flows, lst, addedNodes);
                }

            }
        }


        List<Flow> findAllDepOf(Flow node, List<Flow> flows)
        {

            List<Flow> deps = new List<Flow>();
            for (var i = 0; i < flows.Count; i++)
            {
                if (flows[i].dependant_on_id != "" || flows[i].dependant_on_id != null)
                {
                    String[] tmp = flows[i].dependant_on_id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        tmp[j] = tmp[j].Substring(1);
                    }

                    if (tmp.Contains((node.flow_id).ToString()) && flows[i].chain_id == node.chain_id)
                    {
                        deps.Add(flows[i]);
                    }
                }
            }
            return deps;
        }


        [WebMethod]
        public string GetChainableTasks(string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            mLogger.Debug("fetching chainable tasks");
            var obj = new System.Web.Script.Serialization.JavaScriptSerializer();
            obj.MaxJsonLength = Int32.MaxValue;
            return obj.Serialize(DALAdapter.getAllChainableTasks());
        }

        [WebMethod]
        public string GetFlowsJSON(string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            mLogger.Debug("fetching flow json");
            var obj = new System.Web.Script.Serialization.JavaScriptSerializer();
            obj.MaxJsonLength = Int32.MaxValue;
            return obj.Serialize(DALAdapter.getFlows());//flowsJSON;
        }

        [WebMethod]
        public string getCalendarNames(string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            mLogger.Debug("fetching calendarnames");
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(DALAdapter.GetCalendarNames());

        }

        [WebMethod]
        public void MuteFlows(string selectedFlows, string username, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            var flows = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(selectedFlows) as Array;
            //DALAdapter.muteFlows(flows.S);
            //System.Array.ConvertAll<string, int>(flows,Convert.ToInt32);
            int[] flws = flows.OfType<string>().ToArray<string>().Select(s => int.Parse(s)).ToArray();
            DALAdapter.muteFlows(flws, username);
        }

        [WebMethod]
        public void saveConfiguredTask(Flow taskInfo,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            DALAdapter.saveConfiguredTask(taskInfo);
        }

        [WebMethod]
        public void updateConfiguredDependencies(string dependancyDictionary,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            Dictionary<String, Object> dic = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(dependancyDictionary) as Dictionary<String, Object>;

            foreach (String key in dic.Keys)
            {
                String depStr = "";
                foreach (String str in dic[key] as Array)
                {
                    depStr += '&' + str.Substring(4) + ",";
                }
                depStr.TrimEnd(',');
                DALAdapter.setDependancyByFlowId(key.Substring(4), depStr);
            }


        }
        [WebMethod]
        public List<string> GetPrivilegeList(string username,string clientName)
        {
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;

                mLogger.Debug("fetching privilege list");
                DuplexChannelFactory<ICTMService> dupFactory = null;
                ICTMService clientProxy = null;
                CTMServicesWebMethods cbk = new CTMServicesWebMethods();
                dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
                dupFactory.Open();
                clientProxy = dupFactory.CreateChannel();
                return clientProxy.GetPrivilegeList(username,clientName);
            }
            catch (Exception ex)
            {
                mLogger.Error("Error while fetching privilege list Details:" + ex.ToString());
                throw;

            }
        }
        [WebMethod]
        public void configureTasks(string modifiedTasks, string username,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;

            var updateFlows = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(modifiedTasks) as Array;

            List<Flow> flows = new List<Flow>();

            foreach (var flow in updateFlows)
            {
                flows.Add(new Flow()
                {
                    dependant_on_id = ((flow as Dictionary<String, object>)["dependant_on_id"]).ToString(),
                    flow_id = Convert.ToInt32((flow as Dictionary<String, object>)["flow_id"]),
                    timeout = Convert.ToInt64((flow as Dictionary<String, object>)["timeout"]),
                    proceed_on_fail = Convert.ToBoolean((flow as Dictionary<String, object>)["proceed_on_fail"]),
                    is_muted = Convert.ToBoolean((flow as Dictionary<String, object>)["is_muted"].ToString()),
                    rerun_on_fail = Convert.ToBoolean((flow as Dictionary<String, object>)["rerun_on_fail"].ToString()),
                    fail_retry_duration = Convert.ToInt64((flow as Dictionary<String, object>)["fail_retry_duration"].ToString()),
                    fail_number_retry = Convert.ToInt32((flow as Dictionary<String, object>)["fail_number_retry"].ToString()),
                    on_fail_run_task = Convert.ToInt32((flow as Dictionary<String, object>)["on_fail_run_task"].ToString()),
                    task_time_out = Convert.ToInt32((flow as Dictionary<String, object>)["task_time_out"].ToString()),
                    task_second_instance_wait = Convert.ToInt32((flow as Dictionary<String, object>)["task_second_instance_wait"].ToString()),
                    task_wait_subscription_id = (flow as Dictionary<String, object>)["task_wait_subscription_id"].ToString()
                });

            }

            DALAdapter.updateFlows(flows.ToArray(), username);




        }

        [WebMethod]
        public string triggerChain(string chainId, string username, DateTime? asOfDate,string clientName)// string asOfDate)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;

            // string asOfDate = "";
            //ctmService.triggerChain(chainId);
            //ServiceReference1.CTMServiceClient ctm = new ServiceReference1.CTMServiceClient

            DuplexChannelFactory<ICTMService> dupFactory = null;
            ICTMService clientProxy = null;

            CTMServicesWebMethods cbk = new CTMServicesWebMethods();
            //dupFactory = new DuplexChannelFactory<ICTMService>(cbk, new NetTcpBinding(), new EndpointAddress(new Uri("net.tcp://ws-uberi:8182/CTMService"), EndpointIdentity.CreateUpnIdentity("uberi@ivp.in")));
            dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
            dupFactory.Open();
            clientProxy = dupFactory.CreateChannel();
            //DateTime triggerAsOfDate = DateTime.ParseExact(asOfDate, "MM/dd/yyyy", null);
            return clientProxy.TriggerChain(Convert.ToInt32(chainId), false, username, asOfDate, clientName);// triggerAsOfDate);


        }

        [WebMethod]
        public string triggerTaskInChain(string chainId, string flowId, string username, DateTime? asOfDate, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;

            //ctmService.triggerChain(chainId);
            //ServiceReference1.CTMServiceClient ctm = new ServiceReference1.CTMServiceClient
            //DateTime triggerAsOfDate = DateTime.ParseExact(asOfDate, "MM/dd/yyyy", null);
            DuplexChannelFactory<ICTMService> dupFactory = null;
            ICTMService clientProxy = null;

            CTMServicesWebMethods cbk = new CTMServicesWebMethods();
            //dupFactory = new DuplexChannelFactory<ICTMService>(cbk, new NetTcpBinding(), new EndpointAddress(new Uri("net.tcp://ws-uberi:8182/CTMService"), EndpointIdentity.CreateUpnIdentity("uberi@ivp.in")));
            dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
            dupFactory.Open();
            clientProxy = dupFactory.CreateChannel();
            return clientProxy.TriggerTaskInChain(Convert.ToInt32(chainId), Convert.ToInt32(flowId), username, asOfDate, clientName);// triggerAsOfDate);


        }

        [WebMethod]
        public string triggerSingleTaskInChain(string chainId, string flowId, string username, DateTime? asOfDate, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;

            //DateTime triggerAsOfDate = DateTime.ParseExact(asOfDate, "MM/dd/yyyy", null);
            DuplexChannelFactory<ICTMService> dupFactory = null;
            ICTMService clientProxy = null;

            CTMServicesWebMethods cbk = new CTMServicesWebMethods();
            //dupFactory = new DuplexChannelFactory<ICTMService>(cbk, new NetTcpBinding(), new EndpointAddress(new Uri("net.tcp://ws-uberi:8182/CTMService"), EndpointIdentity.CreateUpnIdentity("uberi@ivp.in")));
            dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
            dupFactory.Open();
            clientProxy = dupFactory.CreateChannel();
            return clientProxy.TriggerSingleTaskInChain(Convert.ToInt32(chainId), Convert.ToInt32(flowId), username, asOfDate, clientName);// triggerAsOfDate);
        }

        [WebMethod]
        public void triggerTask(string taskId, DateTime asOfDate, string clientName)
        {
            // ctmService.triggerTask(taskId);
        }
        [WebMethod]
        public void UpdateChainSchedulingInfo(string chainId, string schedulerInfo, string username, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            int scheduledJobId = 0;
            DALAdapter.removeOldScheduledJob(Convert.ToInt32(chainId), username);
            if (schedulerInfo != null && schedulerInfo.Trim().Equals("null") == false)
            {
                scheduledJobId = DALAdapter.AddJobToScheduledJobs(schedulerInfo, null, username);
            }

            if (scheduledJobId != 0)
                DALAdapter.UpdateChainScheduledJobId(Convert.ToInt32(chainId), scheduledJobId, username);
        }
        [WebMethod]
        public void UnscheduleJob(string chainId, string username, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;

            DALAdapter.removeOldScheduledJob(Convert.ToInt32(chainId), username);
        }

        [WebMethod]
        public string AddChain(string chainInfo, string selectedTasksInfo, string schedulerInfo, string username, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;

            var chain = (new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(chainInfo)) as Dictionary<String, object>;
            var selectedTasks = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(selectedTasksInfo) as Array;

            int scheduledJobId = 0;
            if (schedulerInfo != null && schedulerInfo.Trim().Equals("null") == false)
            {
                scheduledJobId = DALAdapter.AddJobToScheduledJobs(schedulerInfo, selectedTasks, username);
            }
            //var a = chain["calendar_name"].ToString();
            //var b = (scheduledJobId != 0 ? Convert.ToInt16(chain["scheduled_job_id"]) : scheduledJobId);


            ChainedTask chainedTask = new ChainedTask()
            {
                //calendar_id = Convert.ToInt16( chain["calendar_id"]),
                calendar_name = chain["calendar_name"].ToString(),
                chain_name = chain["chain_name"].ToString(),
                chain_second_instance_wait = Convert.ToInt32(chain["chain_second_instance_wait"]),
                scheduled_job_id = (scheduledJobId == 0 ? 0 : scheduledJobId),
                created_by = username, // "admin"
                created_on = DateTime.Now,
                is_active = true,
                //last_modified_by = "admin",
                last_modified_by = username,
            };

            List<Flow> flows = new List<Flow>();

            foreach (var flow in selectedTasks)
            {
                mLogger.Debug("Adding chain ut " + (flow as Dictionary<String, object>)["task_name"].ToString() + ">>" + (flow as Dictionary<String, object>)["module_name"].ToString() + ">>" + (flow as Dictionary<String, object>)["task_type_name"].ToString() + "\n");
                flows.Add(new Flow()
                {
                    module_name = (flow as Dictionary<String, object>)["module_name"].ToString(),
                    module_id = DALAdapter.getModuleId((flow as Dictionary<String, object>)["module_name"].ToString()),
                    task_name = (flow as Dictionary<String, object>)["task_name"].ToString(),
                    task_summary_id = Convert.ToInt32((flow as Dictionary<String, object>)["task_summary_id"]),
                    task_type_name = (flow as Dictionary<String, object>)["task_type_name"].ToString(),
                    registered_module_id = DALAdapter.getRegisteredModuleId((flow as Dictionary<String, object>)["module_name"].ToString())
                });
            }


            DuplexChannelFactory<ICTMService> dupFactory = null;
            ICTMService clientProxy = null;

            CTMServicesWebMethods cbk = new CTMServicesWebMethods();
            dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
            dupFactory.Open();
            clientProxy = dupFactory.CreateChannel();
            clientProxy.flowAdded(flows.Select(x => x.task_summary_id).ToList<int>(), clientName);//TriggerChain(Convert.ToInt32(chainId), false, username, asOfDate);// triggerAsOfDate);

            return DALAdapter.addChain(chainedTask, flows.ToArray()).ToString();
        }
        //delete>>flowId||add>>tasksummaryId
        Dictionary<String, List<int>> getAddTaskSummaryIdDeleteFlowIdDictionary(int chainId, System.Array selectedTasks)
        {
            Dictionary<String, List<int>> tmp = new Dictionary<string, List<int>>();
            tmp["Add"] = new List<int>();
            tmp["Delete"] = new List<int>();
            List<Flow> oldChain = DALAdapter.getAllTasksInChain(chainId);

            List<int> tmpSelectedTasksflows = new List<int>();
            foreach (var ob in selectedTasks)
            {
                tmpSelectedTasksflows.Add(Convert.ToInt32((ob as Dictionary<String, object>)["task_summary_id"]));
            }

            foreach (var ob in selectedTasks)
            {
                if (oldChain.Any(x => x.task_summary_id == Convert.ToInt32((ob as Dictionary<String, object>)["task_summary_id"])) == false)
                {

                    tmp["Add"].Add(Convert.ToInt32((ob as Dictionary<String, object>)["task_summary_id"]));
                }
            }
            //selectedTasks.bin
            foreach (Flow ob in oldChain)
            {
                if (tmpSelectedTasksflows.Contains(ob.task_summary_id) == false)
                //if (Convert.ToInt32((ar as Dictionary<String, object>)["flow_id"])==ob.flow_id  )
                {
                    tmp["Delete"].Add(ob.flow_id);
                }

            }
            return tmp;
        }

        public bool isLinearChain(List<Flow> flows)
        {
            for (var i = 0; i < flows.Count; i++)
            {
                var deps = findAllDepOf(flows[i], flows);
                if (deps.Count > 1) return false;
            }
            return true;
        }
        private void raiseFlowAddedEvent(int taskSummaryId, string clientName)
        {
            List<int> tmp = new List<int>();
            tmp.Add(taskSummaryId);
            raiseFlowsAddedEvent(tmp, clientName);
        }

        private void raiseFlowsAddedEvent(List<int> taskSummaryIds, string clientName)
        {
            DuplexChannelFactory<ICTMService> dupFactory = null;
            ICTMService clientProxy = null;
            CTMServicesWebMethods cbk = new CTMServicesWebMethods();
            dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
            dupFactory.Open();
            clientProxy = dupFactory.CreateChannel();
            clientProxy.flowAdded(taskSummaryIds, clientName);
        }
        private void raiseFlowsDeletedEvent(List<int> taskSummaryIds, string clientName)
        {
            DuplexChannelFactory<ICTMService> dupFactory = null;
            ICTMService clientProxy = null;
            CTMServicesWebMethods cbk = new CTMServicesWebMethods();
            dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
            dupFactory.Open();
            clientProxy = dupFactory.CreateChannel();
            clientProxy.flowDeleted(taskSummaryIds, clientName);
        }
        private void insertFlowInDBAt(Flow flow, int chainId, int index, string username, string clientName)
        {
            try
            {
                flow.chain_id = chainId;
                List<Flow> oldChain = DALAdapter.getAllTasksInChain(Convert.ToInt32(chainId));
                int currFlowId = -1;

                if (index != 0) //task has a prev
                {
                    Flow prev = oldChain[index - 1];
                    currFlowId = DALAdapter.addFlow(flow, "&" + prev.flow_id, username);
                    raiseFlowAddedEvent(flow.task_summary_id, clientName);
                }
                else
                {
                    DALAdapter.addFlow(flow, null, username);
                    raiseFlowAddedEvent(flow.task_summary_id, clientName);

                }

                //if (index + 1 < oldChain.Count) //task has a next
                //{
                //    Flow next = oldChain[index + 1];
                //    next.dependant_on_id = "&" + currFlowId;
                //    Flow[] tmp = { next };
                //    DALAdapter.updateFlows(tmp);
                //}

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [WebMethod]
        public void AddDeleteTasksToExistingChain(string chainId, string selectedTasksInfo, string username, string clientName)
        {
            try
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                    RMultiTanentUtil.ClientName = clientName;

                //var chain = (new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(chainInfo)) as Dictionary<String, object>;
                var selectedTasks = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(selectedTasksInfo) as Array;
                Dictionary<String, List<int>> dic = getAddTaskSummaryIdDeleteFlowIdDictionary(Convert.ToInt32(chainId), selectedTasks);
                if (dic["Delete"].Count > 0)
                {
                    DALAdapter.deleteFlows(dic["Delete"], username);
                    raiseFlowsDeletedEvent(dic["Delete"].Select(x => DALAdapter.getDeletedFlowByFlowId(x).task_summary_id).ToList<int>(), clientName);//TriggerChain(Convert.ToInt32(chainId), false, username, asOfDate);// triggerAsOfDate);
                }
                List<Flow> oldChain = DALAdapter.getAllTasksInChain(Convert.ToInt32(chainId));
                if (isLinearChain(oldChain))
                {
                    int i = 0;
                    foreach (var flow in selectedTasks)
                    {
                        if (dic["Add"].Contains(Convert.ToInt32((flow as Dictionary<String, Object>)["task_summary_id"])))
                        {
                            insertFlowInDBAt(new Flow()
                            {
                                module_name = (flow as Dictionary<String, object>)["module_name"].ToString(),
                                //module_id = DALAdapter.getRegisteredModuleId((flow as Dictionary<String, object>)["module_name"].ToString()),
                                module_id = DALAdapter.getModuleId((flow as Dictionary<String, object>)["module_name"].ToString()),
                                task_name = (flow as Dictionary<String, object>)["task_name"].ToString(),
                                task_summary_id = Convert.ToInt32((flow as Dictionary<String, object>)["task_summary_id"]),
                                task_type_name = (flow as Dictionary<String, object>)["task_type_name"].ToString()


                            }, Convert.ToInt32(chainId), i, username, clientName);
                        }
                        i++;
                    }
                    DALAdapter.UpdateDependentIds(selectedTasks, Convert.ToInt32(chainId), username);
                    return;
                }
                List<Flow> flowsToBeAdded = new List<Flow>();

                foreach (var flow in selectedTasks)
                {
                    if (dic["Add"].Contains(Convert.ToInt32((flow as Dictionary<String, Object>)["task_summary_id"])))
                    {
                        flowsToBeAdded.Add(new Flow()
                        {
                            module_name = (flow as Dictionary<String, object>)["module_name"].ToString(),
                            module_id = DALAdapter.getRegisteredModuleId((flow as Dictionary<String, object>)["module_name"].ToString()),
                            task_name = (flow as Dictionary<String, object>)["task_name"].ToString(),
                            task_summary_id = Convert.ToInt32((flow as Dictionary<String, object>)["task_summary_id"]),
                            task_type_name = (flow as Dictionary<String, object>)["task_type_name"].ToString()
                        });
                    }

                }

                if (flowsToBeAdded.Count > 0)
                {
                    DALAdapter.AddTasksToExistingChain(Convert.ToInt32(chainId), flowsToBeAdded, username);
                    DALAdapter.UpdateDependentIds(selectedTasks, Convert.ToInt32(chainId), username);
                    raiseFlowsAddedEvent(flowsToBeAdded.Select(x => x.task_summary_id).ToList<int>(), clientName);
                }

                //int scheduledJobId = 0;
                //if (schedulerInfo != null && schedulerInfo.Trim().Equals("null") == false)
                //{
                //    scheduledJobId = DALAdapter.AddJobToScheduledJobs(schedulerInfo, selectedTasks);
                //}
                //var a = chain["calendar_name"].ToString();
                //var b = (scheduledJobId != 0 ? Convert.ToInt16(chain["scheduled_job_id"]) : scheduledJobId);


                //ChainedTask chainedTask = new ChainedTask()
                //{
                //    //calendar_id = Convert.ToInt16( chain["calendar_id"]),
                //    calendar_name = chain["calendar_name"].ToString(),
                //    chain_name = chain["chain_name"].ToString(),
                //    scheduled_job_id = (scheduledJobId == 0 ? 0 : scheduledJobId),
                //    created_by = "admin",
                //    created_on = DateTime.Now,
                //    is_active = true,
                //    last_modified_by = "admin",
                //};

                //List<Flow> flows = new List<Flow>();

                //foreach (var flow in selectedTasks)
                //{
                //    flows.Add(new Flow()
                //    {
                //        module_name = (flow as Dictionary<String, object>)["module_name"].ToString(),
                //        module_id = DALAdapter.getRegisteredModuleId((flow as Dictionary<String, object>)["module_name"].ToString()),
                //        task_name = (flow as Dictionary<String, object>)["task_name"].ToString(),
                //        task_summary_id = Convert.ToInt32((flow as Dictionary<String, object>)["task_summary_id"]),
                //        task_type_name = (flow as Dictionary<String, object>)["task_type_name"].ToString()
                //    });
                //}


            }
            catch (Exception ex)
            {
                throw;

            }
            // return DALAdapter.addTaskstoExistingChain(chainId, flows.ToArray(),Convert.ToInt32(lastFlowId)).ToString();
        }



        [WebMethod]
        public void DeleteFlows(string[] flow_ids, string username, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;

            //int []tmp  = new int[flow_ids.Length];
            List<int> tmp = new List<int>();
            List<Flow> flows = new List<Flow>();
            foreach (String str in flow_ids)
            {
                tmp.Add(Convert.ToInt32(str));
                flows.Add(DALAdapter.getFlowByFlowId(Convert.ToInt32(str)));
            }

            DuplexChannelFactory<ICTMService> dupFactory = null;
            ICTMService clientProxy = null;
            CTMServicesWebMethods cbk = new CTMServicesWebMethods();
            dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
            dupFactory.Open();
            clientProxy = dupFactory.CreateChannel();
            clientProxy.flowDeleted(flows.Select(x => x.task_summary_id).ToList<int>(), clientName);//TriggerChain(Convert.ToInt32(chainId), false, username, asOfDate);// triggerAsOfDate);


            DALAdapter.deleteFlows(tmp, username);


        }

        [WebMethod]
        public void DeleteChain(string chainId, string username, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;

            List<Flow> flows = DALAdapter.getAllTasksInChain(Convert.ToInt32(chainId));
            DuplexChannelFactory<ICTMService> dupFactory = null;
            ICTMService clientProxy = null;
            CTMServicesWebMethods cbk = new CTMServicesWebMethods();
            dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
            dupFactory.Open();
            clientProxy = dupFactory.CreateChannel();
            clientProxy.flowDeleted(flows.Select(x => x.task_summary_id).ToList<int>(), clientName);//TriggerChain(Convert.ToInt32(chainId), false, username, asOfDate);// triggerAsOfDate);

            DALAdapter.deleteChain(chainId, username);
        }

        [WebMethod]
        public string GetLastRunTaskStatuses(string chainId, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            var obj = new System.Web.Script.Serialization.JavaScriptSerializer();
            obj.MaxJsonLength = Int32.MaxValue;
            return obj.Serialize(DALAdapter.GetLastRunTaskStatusesByChainId(Convert.ToInt32(chainId)));

        }

        [WebMethod]
        public Flow GetFlowInfoByFlowId(int flowId,string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            return DALAdapter.getFlowByFlowId(flowId);

        }

        [WebMethod]
        public string UpdateChainName(int chainId, string chainName, string username, string clientName)
        {
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                RMultiTanentUtil.ClientName = clientName;
            return DALAdapter.UpdateChainName(chainId, chainName, username);
        }

        #region not implemented

        void ICTMServiceCallback.triggerTask(TaskInfo taskInfo, string guid, string clientName)
        {
            throw new NotImplementedException();
        }

        List<string> ICTMServiceCallback.getSubscriberList(string assemblyLocation, string className, string clientName)
        {
            throw new NotImplementedException();
        }

        List<string> ICTMServiceCallback.getCalendarList(string assemblyLocation, string className, string clientName)
        {
            throw new NotImplementedException();
        }

        Boolean ICTMServiceCallback.deleteStatusFromClient(string assemblyLocation, string className, int clientTaskStatusId, string clientName)
        {
            throw new NotImplementedException();
        }
        Boolean ICTMServiceCallback.UndoTask(string assemblyLocation, string className, int clientTaskStatusId, string clientName)
        {
            throw new NotImplementedException();
        }
        public List<int> getUnsyncdTasksClientTaskStatusIds(string assemblyLocation, string className, List<int> list, string clientName) { throw new NotImplementedException(); }

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
        #endregion

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
    public class FlowChain : Flow
    {

        public List<FlowChain> children = new List<FlowChain>();

        public string next_scheduled_time { get; set; }

        public int chain_last_run_status { get; set; }
    }

    public class customComparerOnChainId : IEqualityComparer<Flow>
    {
        public bool Equals(Flow x, Flow y) { return (x.chain_id == y.chain_id); }
        public int GetHashCode(Flow obj) { return obj.chain_id.GetHashCode(); }
    }
}
