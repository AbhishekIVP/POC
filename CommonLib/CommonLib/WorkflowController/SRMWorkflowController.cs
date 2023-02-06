using com.ivp.commom.commondal;
using com.ivp.common.lookupdatamassage;
using com.ivp.common.migration;
using com.ivp.rad.common;
using com.ivp.rad.culturemanager;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.notificationmanager;
using com.ivp.rad.RRadWorkflow;
using com.ivp.rad.RUserManagement;
using com.ivp.rad.transport;
using com.ivp.rad.utils;
using com.ivp.rad.viewmanagement;
using com.ivp.refmaster.common;
using com.ivp.srmcommon;
using com.ivp.SRMCommonModules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
//using com.ivp.refmaster.controller;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace com.ivp.common
{
    public class SRMWorkFlowCheckException : Exception
    {
        #region constructors
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="SMCoreException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SRMWorkFlowCheckException(string message)
            : base(message)
        {
            //do nothing
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="SMCoreException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public SRMWorkFlowCheckException(string message, Exception ex)
            : base(message, ex)
        {
            //do nothing 
        }
        #endregion
    }

    public static class SRMWorkflowController
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMWorkflowController");
        private static string FromEmailId = SRMCommon.GetConfigFromDB("FromEmailIdForWorkflow");
        private static string TransportName = SRMCommon.GetConfigFromDB("MailTransportName");
        static List<WorkflowDivisionByModule> ModuleWiseInfo = new List<WorkflowDivisionByModule>();
        const string startState = "start";
        const string endState = "end";
        const string failedState = "failed";
        const string cancelAction = "cancel";
        const string rejectAction = "reject";
        const string approveAction = "approve";
        const string editAndApproveAction = "edit and approve";
        const string creatorCancelsTheRequest = "Creator cancels the request";

        /// <summary>
        /// Method to initiate a workflow
        /// </summary>
        /// <param name="inputInfo"></param>
        /// <param name="userName"></param>
        /// <param name="conMgr"></param>
        /// <returns></returns>
        public static List<SRMEventInfo> InitiateWorkflow(ref SRMWorkflowResponse inputInfo, List<WorkflowQueueInfo> queueInfo, string userName, bool raiseWorkflow, RDBConnectionManager conMgr = null, Dictionary<SRMEventActionType, SRMEventPreferenceInfo> dictEventConfiguration = null, DateTime? timestamp = null)
        {
            if (timestamp == null)
                timestamp = DateTime.Now;
            Dictionary<int, SRMEventInfo> dictRadWorkflowInstanceIdVsEventsInfo = new Dictionary<int, SRMEventInfo>();
            List<WorkflowActionInfo> actionInfoForCreateEmailStructure = new List<WorkflowActionInfo>();
            string usersInfoFromRAD = null;
            try
            {
                mLogger.Debug("InitiateWorkflow -> Start");

                int typeId = queueInfo.First().TypeID;
                string typeName = string.Empty;

                DataSet dsModuleDetails = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT CAST((CASE WHEN wi.workflow_action_type_id IN (1,6,11,16) THEN 1 ELSE 0 END) AS BIT) AS is_create, module_id
                FROM [IVPRefMaster].[dbo].[ivp_srm_workflow_instance] wi
                WHERE wi.workflow_instance_id = {0} AND wi.is_active = 1", inputInfo.WorkflowInstanceId), ConnectionConstants.RefMaster_Connection);

                bool isCreate = false;
                int moduleId = 3;

                if (dsModuleDetails != null && dsModuleDetails.Tables.Count > 0 && dsModuleDetails.Tables[0].Rows.Count > 0)
                {
                    isCreate = Convert.ToBoolean(dsModuleDetails.Tables[0].Rows[0]["is_create"]);
                    moduleId = Convert.ToInt32(dsModuleDetails.Tables[0].Rows[0]["module_id"]);
                }

                DataSet dsTypeName = null;
                if (moduleId == 3)
                    dsTypeName = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @type_id INT = {0}

	                    SELECT tmas.sectype_name AS [type_name]
	                    FROM IVPSecMaster.dbo.ivp_secm_sectype_master tmas
	                    WHERE tmas.sectype_id = @type_id AND tmas.is_active = 1", typeId), ConnectionConstants.RefMaster_Connection);
                else
                    dsTypeName = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @type_id INT = {0}

	                    SELECT tmas.entity_display_name AS [type_name]
	                    FROM IVPRefMaster.dbo.ivp_refm_entity_type tmas
	                    WHERE tmas.entity_type_id = @type_id AND tmas.is_active = 1", typeId), ConnectionConstants.RefMaster_Connection);

                if (dsTypeName != null && dsTypeName.Tables.Count > 0 && dsTypeName.Tables[0].Rows.Count > 0)
                    typeName = Convert.ToString(dsTypeName.Tables[0].Rows[0][0]);

                if (dictEventConfiguration == null)
                    dictEventConfiguration = SRMEventController.GetEventConfiguration(new SRMEventConfigurationInputInfo { instrumentTypeId = typeId, moduleId = moduleId }).DictConfiguration;

                Dictionary<string, WorkflowQueueInfo> dictInstrumentIdVsqueueInfo = new Dictionary<string, WorkflowQueueInfo>();
                if (queueInfo != null && queueInfo.Count > 0)
                    dictInstrumentIdVsqueueInfo = queueInfo.Where(x => !string.IsNullOrEmpty(x.InstrumentID)).GroupBy(x => x.InstrumentID).ToDictionary(x => x.Key, y => y.First());

                var usersdic = GetMassagedUser();
                var workflowInstanceId = inputInfo.WorkflowInstanceId;
                Queue<string> instrumentQueue = new Queue<string>();

                SRMEventPreferenceInfo eventPreferenceInfo = dictEventConfiguration[SRMEventActionType.Workflow_Request_Initiate];

                Dictionary<string, int> instrumentIdVsInstanceId = new Dictionary<string, int>();
                Dictionary<string, int> instrumentIdVsRuleMappingId = new Dictionary<string, int>();

                foreach (var kvp in inputInfo.InstrumentIDVsRADWorkflowInfo)
                {
                    if (!instrumentIdVsRuleMappingId.ContainsKey(kvp.Key))
                        instrumentIdVsRuleMappingId.Add(kvp.Key, kvp.Value.RuleMappingId);

                }

                HashSet<int> workflowIdList = new HashSet<int>();
                foreach (var kvp in inputInfo.InstrumentIDVsRADWorkflowInfo)
                {
                    workflowIdList.Add(kvp.Value.RadWorkflowId);
                }

                Dictionary<int, Dictionary<string, HashSet<string>>> workflowIdVsStateNameVsListOfMappedGroups = new Dictionary<int, Dictionary<string, HashSet<string>>>();
                workflowIdVsStateNameVsListOfMappedGroups = GetListOfGroupsMapped(workflowIdList);

                Dictionary<int, KeyValuePair<int, string>> RADWorkflowInstanceIdVsWorkflowIdVsCurrentState = new Dictionary<int, KeyValuePair<int, string>>();

                Action<List<TriggerInfo>> callback = info =>
                {
                    try
                    {

                        //new SRMRADWorkflow().MassageUserNamesTriggerInfo(info);
                        if (!raiseWorkflow)
                            throw new SRMWorkFlowCheckException("Initiate Workflow -> Exception in callback.");
                        else
                        {
                            Queue<string> myQueue2 = new Queue<string>(instrumentQueue);
                            foreach (var i in info)
                            {
                                var instrumentId = myQueue2.Dequeue();
                                instrumentIdVsInstanceId[instrumentId] = i.InstanceId;
                            }
                            foreach (var q in queueInfo)
                            {
                                if (instrumentIdVsInstanceId.ContainsKey(q.InstrumentID))
                                    q.RadInstanceID = instrumentIdVsInstanceId[q.InstrumentID];

                                if (instrumentIdVsRuleMappingId.ContainsKey(q.InstrumentID))
                                    q.RuleMappingID = instrumentIdVsRuleMappingId[q.InstrumentID];
                            }

                            
                            actionInfoForCreateEmailStructure = queueInfo.Select(ai => new WorkflowActionInfo() { InstrumentID = ai.InstrumentID, RadInstanceID = ai.RadInstanceID, WorkflowInstanceID = workflowInstanceId, TypeID = ai.TypeID, RuleMappingID = ai.RuleMappingID }).ToList();

                            foreach (var id in info)
                            {
                                if (!RADWorkflowInstanceIdVsWorkflowIdVsCurrentState.ContainsKey(id.InstanceId))
                                    RADWorkflowInstanceIdVsWorkflowIdVsCurrentState.Add(id.InstanceId, new KeyValuePair<int, string>(id.workflowId, id.currentState));
                            }

                        }
                    }
                    catch (Exception er)
                    {
                        mLogger.Error(er.ToString());
                        throw;
                    }
                };

                List<TriggerInfo> lstTriggerInfo = new List<TriggerInfo>();
                TriggerInfo tInfo = null;

                bool instanceReturned = false;
                Dictionary<int, List<int>> workFlowIdVsInstanceID = new Dictionary<int, List<int>>();

                Dictionary<string, SRMWorkflowInstanceInfo> instrumentInfo = inputInfo.InstrumentIDVsRADWorkflowInfo;

                string approveForSkipWorkflow = SRMCommon.GetConfigFromDB("ApproveForSkipWorkflow");
                string approveActionName = "Edit and Approve";
                if (!string.IsNullOrEmpty(approveForSkipWorkflow) && approveForSkipWorkflow.Equals("true", StringComparison.OrdinalIgnoreCase))
                    approveActionName = "Approve";

                foreach (var i in instrumentInfo.Keys)
                {
                    tInfo = new TriggerInfo();
                    tInfo.workflowId = instrumentInfo[i].RadWorkflowId;
                    tInfo.user = userName;
                    tInfo.action = approveActionName;
                    if (tInfo.workflowId != -1)
                    {
                        if (!workFlowIdVsInstanceID.ContainsKey(tInfo.workflowId))
                            workFlowIdVsInstanceID.Add(tInfo.workflowId, null);

                        lstTriggerInfo.Add(tInfo);
                    }

                    instrumentQueue.Enqueue(i);
                }

                try
                {
                    instanceReturned = new SRMRADWorkflow().TriggerWorkFlow(lstTriggerInfo, callback);
                }
                catch (SRMWorkFlowCheckException ex)
                {
                    mLogger.Error(ex.ToString());
                }
                catch (Exception ex)
                {
                    throw;
                }

                if (instanceReturned || !raiseWorkflow)
                {
                    //added
                    if (raiseWorkflow)
                    {
                        if (actionInfoForCreateEmailStructure.Count > 0)
                        {
                            Dictionary<int,string> RADWorkflowInstanceIdVsRADUsers = GetUsersDetailsFromRAD(RADWorkflowInstanceIdVsWorkflowIdVsCurrentState, workflowIdVsStateNameVsListOfMappedGroups);
                            CreateEmailStructure(actionInfoForCreateEmailStructure, userName, moduleId, true, false, "", RADWorkflowInstanceIdVsRADUsers, null, conMgr, timestamp);
                        }
                    }


                    var dictRadWorkflowIdVsTriggerInfo = lstTriggerInfo.GroupBy(x => x.workflowId).ToDictionary(o => o.Key, p => { var t = p.First(); return new { IsFinalApproval = (t.currentState != null && t.currentState.ToLower() == endState), IsWorkflowRaised = (t.InstanceId > 0) }; });

                    foreach (KeyValuePair<string, SRMWorkflowInstanceInfo> kvp in inputInfo.InstrumentIDVsRADWorkflowInfo)
                    {
                        if (dictRadWorkflowIdVsTriggerInfo.ContainsKey(kvp.Value.RadWorkflowId))
                        {
                            var ti = dictRadWorkflowIdVsTriggerInfo[kvp.Value.RadWorkflowId];
                            if (raiseWorkflow)
                            {
                                kvp.Value.RadWorkflowInstanceId = instrumentIdVsInstanceId[kvp.Key];

                                if (eventPreferenceInfo.IsSecurityLevel || (eventPreferenceInfo.LstAttributeLevel != null && eventPreferenceInfo.LstAttributeLevel.Count > 0) || (eventPreferenceInfo.LstLegLevel != null && eventPreferenceInfo.LstLegLevel.Count > 0))
                                {
                                    var eventInfo = new SRMEventInfo
                                    {
                                        Action = SRMEventActionType.Workflow_Request_Initiate,
                                        ID = kvp.Key,
                                        Key = kvp.Key,
                                        User = userName,
                                        Module = SRMEventModule.Securities,
                                        Type = typeName,
                                        IsCreate = isCreate,
                                        TimeStamp = timestamp.Value,
                                        //Initiator = userName
                                    };

                                    if (Enum.IsDefined(typeof(SRMEventModule), moduleId))
                                        eventInfo.Module = (SRMEventModule)moduleId;
                                    if (dictInstrumentIdVsqueueInfo.ContainsKey(kvp.Key))
                                    {
                                        eventInfo.EffectiveStartDate = dictInstrumentIdVsqueueInfo[kvp.Key].EffectiveStartDate;
                                        eventInfo.EffectiveEndDate = dictInstrumentIdVsqueueInfo[kvp.Key].EffectiveEndDate;
                                    }

                                    dictRadWorkflowInstanceIdVsEventsInfo[kvp.Value.RadWorkflowInstanceId] = eventInfo;
                                }
                            }
                            kvp.Value.IsFinalApproval = ti.IsFinalApproval;
                            kvp.Value.IsWorkflowRaised = ti.IsWorkflowRaised;
                        }
                        else
                        {
                            kvp.Value.RadWorkflowInstanceId = -1;
                            kvp.Value.IsFinalApproval = false;
                            kvp.Value.IsWorkflowRaised = false;
                        }
                    }
                }

                if (dictRadWorkflowInstanceIdVsEventsInfo != null && dictRadWorkflowInstanceIdVsEventsInfo.Count > 0)
                {
                    var workflowPendingInfo = new SRMRADWorkflow().GetPendingInfoForInstances(dictRadWorkflowInstanceIdVsEventsInfo.Keys.ToList(), string.Empty);

                    foreach (var workflowLevel in workflowPendingInfo)
                    {
                        if (dictRadWorkflowInstanceIdVsEventsInfo.ContainsKey(workflowLevel.InstanceID))
                            dictRadWorkflowInstanceIdVsEventsInfo[workflowLevel.InstanceID].PendingAt = workflowLevel.GroupsMapped;
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("InitiateWorkflow -> Error: " + ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("InitiateWorkflow -> End");
            }
            return dictRadWorkflowInstanceIdVsEventsInfo.Values.ToList();
        }

        private static Dictionary<int, Dictionary<string, HashSet<string>>> GetListOfGroupsMapped(HashSet<int> workflowId)
        {
            mLogger.Debug("GetListOfGroupsMapped --> Start");
            Dictionary<int, Dictionary<string, HashSet<string>>> workflowIdVsStateNameVsListOfMappedGroups = new Dictionary<int, Dictionary<string, HashSet<string>>>();
            RWorkFlowService rWorkFlowService = new RWorkFlowService();
            foreach (int wId in workflowId)
            {
                if (!workflowIdVsStateNameVsListOfMappedGroups.ContainsKey(wId))
                    workflowIdVsStateNameVsListOfMappedGroups.Add(wId, new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase));
                RWorkFlowInfo workflowInfo = rWorkFlowService.GetWorkflowInfoById(wId);
                foreach (WorkflowState stateInfo in workflowInfo.WorkflowStates)
                {
                    if (!workflowIdVsStateNameVsListOfMappedGroups[wId].ContainsKey(stateInfo.StateName))
                        workflowIdVsStateNameVsListOfMappedGroups[wId].Add(stateInfo.StateName, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                    foreach (StateActionInfo stateActionInfo in stateInfo.StateActions)
                    {
                        foreach (UserNGroups mappedUsers in stateActionInfo.MappedUsers)
                        {
                            foreach (string name in mappedUsers.Name.Keys)
                            {
                                workflowIdVsStateNameVsListOfMappedGroups[wId][stateInfo.StateName].Add(name);
                            }
                        }
                    }
                }
            }
            mLogger.Debug("GetListOfGroupsMapped --> End");
            return workflowIdVsStateNameVsListOfMappedGroups;
        }

        static XElement LoadWorkflowMailConfig(string node)
        {
            var serverPath = RConfigReader.GetServerPhysicalPath();
            var filePath = @"ConfigFiles\WorkflowEmailTemplate.xml";
            XElement xe = XElement.Load(serverPath + filePath);
            return xe.Element(node);
        }

        static Dictionary<string, string> GetMassagedUser()
        {
            Dictionary<string, string> allUsers = new Dictionary<string, string>();

            RUserManagementService objUserController = new RUserManagementService();
            List<RUserInfo> radUsers = objUserController.GetAllUsersGDPR();

            foreach (var info in radUsers)
            {
                if (info.UserName == "admin")
                    allUsers.Add(info.UserName, info.FullName + " (" + info.UserName + ")");
                else
                    allUsers.Add(info.UserLoginName, info.FullName + " (" + info.UserName + ")");
            }
            return allUsers;
        }

        public static WorkflowRetriggerResponse ReTriggerWorkflow(List<WFRetriggerInfo> inputInfo, string userName, string remarks = "", RDBConnectionManager conMgr = null)
        {
            var guid = Guid.NewGuid().ToString();
            var workflowDetailsTableName = "IVPRefMaster.dbo.[workflowDetailsTable_" + guid + "]";
            try
            {
                mLogger.Debug("ReTriggerWorkflow -> Start");
                var usersdic = GetMassagedUser();
                var instrumentID = inputInfo.First().InstrumentID;
                var instanceIdVSInstrumentId = inputInfo.ToDictionary(a => a.RadInstanceID, b => b.InstrumentID);
                DateTime timestamp = DateTime.Now;
                var now = timestamp.ToString(@"MM-dd-yyyy hh\:mm\:ss\.fff");
                List<TriggerInfo> lstTriggerInfo = new List<TriggerInfo>();
                TriggerInfo tInfo = null;
                bool retriggered = false;
                List<string> instruments = new List<string>();

                Dictionary<int, KeyValuePair<int, SRMEventInfo>> dictRadWorkflowInstanceIdVsEventsInfo = new Dictionary<int, KeyValuePair<int, SRMEventInfo>>();
                int moduleIdMain = 3;

                List<WorkflowActionInfo> actionInfoForCreateEmailStructure = new List<WorkflowActionInfo>();
                string usersInfoFromRAD = null;
                var moduleId = 3;

                HashSet<int> workflowIdList = new HashSet<int>();
                foreach (WFRetriggerInfo wFRetriggerInfo in inputInfo)
                {
                    workflowIdList.Add(wFRetriggerInfo.RadWorkflowID);
                }

                Dictionary<int, Dictionary<string, HashSet<string>>> workflowIdVsStateNameVsListOfMappedGroups = new Dictionary<int, Dictionary<string, HashSet<string>>>();
                workflowIdVsStateNameVsListOfMappedGroups = GetListOfGroupsMapped(workflowIdList);

                Dictionary<int, KeyValuePair<int, string>> rADWorkflowInstanceIdVsWorkflowIdVsCurrentState = new Dictionary<int, KeyValuePair<int, string>>();

                Action<List<TriggerInfo>> callback = lstInfo =>
                {
                    try
                    {

                        DataTable workflowDetailsTable = new DataTable();
                        workflowDetailsTable.Columns.Add("instrument_id", typeof(string));
                        workflowDetailsTable.Columns.Add("rad_workflow_instance_id", typeof(int));
                        foreach (var workflow in inputInfo)
                        {
                            var row = workflowDetailsTable.NewRow();
                            row["instrument_id"] = workflow.InstrumentID;
                            row["rad_workflow_instance_id"] = workflow.RadInstanceID;
                            workflowDetailsTable.Rows.Add(row);
                        }
                        
                        CommonDALWrapper.ExecuteQuery(string.Format(@"CREATE TABLE {0}(instrument_id varchar(max),rad_workflow_instance_id int)", workflowDetailsTableName), CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
                        CommonDALWrapper.ExecuteBulkUpload(workflowDetailsTableName, workflowDetailsTable, ConnectionConstants.RefMaster_Connection);

                        DataTable dtInternal = null;
                        dtInternal = CommonDALWrapper.ExecuteSelectQuery(DALWrapperAppend.Replace(string.Format(@"select a.workflow_instance_id,a.instrument_id,a.rad_workflow_instance_id from IVPRefMaster.dbo.ivp_srm_workflow_requests_queue a
                            inner join {0} b 
                            on a.rad_workflow_instance_id = b.rad_workflow_instance_id and a.instrument_id=b.instrument_id", workflowDetailsTableName)), ConnectionConstants.RefMaster_Connection).Tables[0];
                        Dictionary<string, Dictionary<int, int>> instrumentIdVsRadWorkflowInstanceIdVsWorkflowInstanceId = new Dictionary<string, Dictionary<int, int>>();
                        foreach (DataRow row in dtInternal.Rows)
                        {
                            string instrumentId = Convert.ToString(row["instrument_id"]);
                            if (!instrumentIdVsRadWorkflowInstanceIdVsWorkflowInstanceId.ContainsKey(instrumentId))
                                instrumentIdVsRadWorkflowInstanceIdVsWorkflowInstanceId.Add(instrumentId, new Dictionary<int, int>());

                            int radWorkflowInstanceId = Convert.ToInt32(row["rad_workflow_instance_id"]);
                            int workflowInstanceId = Convert.ToInt32(row["workflow_instance_id"]);
                            if (!instrumentIdVsRadWorkflowInstanceIdVsWorkflowInstanceId[instrumentId].ContainsKey(radWorkflowInstanceId))
                                instrumentIdVsRadWorkflowInstanceIdVsWorkflowInstanceId[instrumentId].Add(radWorkflowInstanceId, workflowInstanceId);
                        }

                        foreach (var info in inputInfo)
                        {
                            if (string.IsNullOrEmpty(Convert.ToString(info.WorkflowInstanceId)))
                            {
                                info.WorkflowInstanceId = instrumentIdVsRadWorkflowInstanceIdVsWorkflowInstanceId[info.InstrumentID][info.RadInstanceID];
                            }
                        }


                        actionInfoForCreateEmailStructure = inputInfo.Select(ai => new WorkflowActionInfo() { InstrumentID = ai.InstrumentID, RadInstanceID = ai.RadInstanceID, WorkflowInstanceID = ai.WorkflowInstanceId }).ToList();

                        foreach (var id in lstInfo)
                        {
                            if (!rADWorkflowInstanceIdVsWorkflowIdVsCurrentState.ContainsKey(id.InstanceId))
                                rADWorkflowInstanceIdVsWorkflowIdVsCurrentState.Add(id.InstanceId, new KeyValuePair<int, string>(id.workflowId, id.currentState));
                        }

                    }
                    catch (Exception er)
                    {
                        mLogger.Error(er.ToString());
                        throw new Exception(er.Message, er);
                    }
                };



                Dictionary<int, WorkflowDetailsInfo> dictInstanceVsWorkflowID = SRMWorkflowDBHandler.GetWorkflowDetailsVsRADWorkflowInstanceID(inputInfo.Select(i => i.RadInstanceID).Distinct().ToList<int>(), conMgr);

                if (dictInstanceVsWorkflowID != null)
                {
                    moduleIdMain = dictInstanceVsWorkflowID.Values.First().moduleId;
                    foreach (KeyValuePair<int, WorkflowDetailsInfo> kvp in dictInstanceVsWorkflowID)
                    {
                        inputInfo.Where(i => i.RadInstanceID == kvp.Key).ToList().ForEach(ii =>
                        {
                            ii.RadWorkflowID = kvp.Value.RadWorkflowId;
                        });

                        var eventInfo = new SRMEventInfo
                        {
                            Action = SRMEventActionType.Workflow_Request_Initiate,
                            ID = instanceIdVSInstrumentId[kvp.Key],
                            Key = instanceIdVSInstrumentId[kvp.Key],
                            User = userName,
                            Module = SRMEventModule.Securities,
                            Type = kvp.Value.typeName,
                            IsCreate = kvp.Value.isCreate,
                            TimeStamp = timestamp,
                            //Initiator = userName
                        };

                        if (Enum.IsDefined(typeof(SRMEventModule), kvp.Value.moduleId))
                            eventInfo.Module = (SRMEventModule)kvp.Value.moduleId;

                        eventInfo.EffectiveStartDate = kvp.Value.EffectiveStartDate;
                        eventInfo.EffectiveEndDate = kvp.Value.EffectiveEndDate;

                        dictRadWorkflowInstanceIdVsEventsInfo[kvp.Key] = new KeyValuePair<int, SRMEventInfo>(kvp.Value.typeId, eventInfo);
                    }
                }

                inputInfo.ForEach(i =>
                {
                    tInfo = new TriggerInfo();
                    tInfo.ReTrigger = true;
                    tInfo.InstanceId = i.RadInstanceID;
                    tInfo.workflowId = i.RadWorkflowID;
                    tInfo.comments = remarks;
                    tInfo.user = userName;
                    lstTriggerInfo.Add(tInfo);
                });

                retriggered = new SRMRADWorkflow().ReTriggerWorkFlow(lstTriggerInfo, callback);

                WorkflowRetriggerResponse response = new WorkflowRetriggerResponse() { isSuccess = retriggered };

                if (retriggered)
                {
                    //added
                    if (actionInfoForCreateEmailStructure.Count > 0)
                    {
                        Dictionary<int, string> RADWorkflowInstanceIdVsRADUsers = GetUsersDetailsFromRAD(rADWorkflowInstanceIdVsWorkflowIdVsCurrentState, workflowIdVsStateNameVsListOfMappedGroups);
                        CreateEmailStructure(actionInfoForCreateEmailStructure, userName, moduleId, false, true, remarks, RADWorkflowInstanceIdVsRADUsers, null, conMgr);
                    }

                    if (dictRadWorkflowInstanceIdVsEventsInfo != null && dictRadWorkflowInstanceIdVsEventsInfo.Count > 0)
                    {
                        var workflowPendingInfo = new SRMRADWorkflow().GetPendingInfoForInstances(dictRadWorkflowInstanceIdVsEventsInfo.Keys.ToList(), string.Empty);

                        foreach (var workflowLevel in workflowPendingInfo)
                        {
                            if (dictRadWorkflowInstanceIdVsEventsInfo.ContainsKey(workflowLevel.InstanceID))
                                dictRadWorkflowInstanceIdVsEventsInfo[workflowLevel.InstanceID].Value.PendingAt = workflowLevel.GroupsMapped;
                        }
                    }

                    if (conMgr == null)
                    {
                        foreach (var typeLevel in dictRadWorkflowInstanceIdVsEventsInfo.Values.GroupBy(x => x.Key))
                        {
                            SRMEventController.RaiseEvent(new SRMRaiseEventsInputInfo { instrumentTypeId = typeLevel.Key, eventInfo = typeLevel.Select(x => x.Value).ToList(), moduleId = moduleIdMain });
                        }
                    }
                    else
                        response.objSRMEventsInfo = dictRadWorkflowInstanceIdVsEventsInfo.Values.GroupBy(x => x.Key).ToDictionary(x => x.Key, y => y.Select(a => a.Value).ToList());
                }

                return response;
            }
            catch (Exception ex)
            {
                mLogger.Error("ReTriggerWorkflow -> Error: " + ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("ReTriggerWorkflow -> End");
                string query = @"IF (OBJECT_ID('" + workflowDetailsTableName + "') IS NOT NULL) DROP TABLE " + workflowDetailsTableName;
                if (conMgr == null)
                    CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Delete, ConnectionConstants.RefMaster_Connection);
                else
                    CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Delete, conMgr);
            }
        }

        public static List<WorkflowActionInfo> ReTriggerWorkflow(List<WorkflowActionInfo> actionInfo, string userName, string remarks = "")
        {
            try
            {
                mLogger.Debug("SRMWorkflowController-ReTriggerWorkflow-2 -> Start");

                List<WorkflowActionInfo> response = new List<WorkflowActionInfo>();
                var inputInfo = actionInfo.Select(ai => new WFRetriggerInfo() { InstrumentID = ai.InstrumentID, RadInstanceID = ai.RadInstanceID, WorkflowInstanceId = ai.WorkflowInstanceID });

                if (inputInfo != null && inputInfo.Count() > 0)
                {
                    foreach (var info in inputInfo)
                    {
                        var result = ReTriggerWorkflow(new List<WFRetriggerInfo>() { info }, userName, remarks);

                        response.Add(new WorkflowActionInfo()
                        {
                            InstrumentID = info.InstrumentID,
                            RadInstanceID = info.RadInstanceID,
                            isPassed = result.isSuccess,
                            Exceptions = new List<WFApprovalExceptionInfo>()
                        });
                    }

                }

                return response;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                mLogger.Debug("SRMWorkflowController-ReTriggerWorkflow-2 -> END");
            }
        }

        public static WorkflowInitiateResponse InitiateWorkflow(List<WorkflowQueueInfo> queueInfo, SRMWorkflowResponse inputInfo, string userName, bool checkWorkflowRaised, RDBConnectionManager conMgr = null, Dictionary<SRMEventActionType, SRMEventPreferenceInfo> dictEventConfiguration = null, DateTime? timestamp = null)
        {
            WorkflowInitiateResponse response = new WorkflowInitiateResponse();

            if (inputInfo != null && inputInfo.InstrumentIDVsRADWorkflowInfo != null && inputInfo.InstrumentIDVsRADWorkflowInfo.Count > 0)
            {
                Dictionary<string, WorkflowQueueInfo> queueInfoDict = queueInfo.ToDictionary(x => x.InstrumentID);
                foreach (var kvp in inputInfo.InstrumentIDVsRADWorkflowInfo)
                {
                    string instrument = kvp.Key;

                    if (queueInfoDict.ContainsKey(instrument))
                    {
                        queueInfoDict[instrument].IsFinalApproval = kvp.Value.IsFinalApproval;
                    }
                }
            }


            response.queueInfo = new List<WorkflowQueueInfo>(queueInfo);
            Dictionary<string, WorkflowQueueInfo> responseDict = response.queueInfo.ToDictionary(x => x.InstrumentID);
            List<string> instrumentsInWorkflow = new List<string>();

            if (checkWorkflowRaised)
                instrumentsInWorkflow = CheckInstrumentsInWorkflow(inputInfo.InstrumentIDVsRADWorkflowInfo.Keys.ToList(), inputInfo.WorkflowInstanceId).Keys.ToList();

            if (instrumentsInWorkflow != null && instrumentsInWorkflow.Count > 0)
            {
                foreach (var instrument in instrumentsInWorkflow)
                {
                    inputInfo.InstrumentIDVsRADWorkflowInfo.Remove(instrument);

                    responseDict[instrument].IsPassed = false;
                    responseDict[instrument].Message = "Instrument already in workflow";
                }

                List<WorkflowQueueInfo> queueInfoToRemove = queueInfo.Where(x => instrumentsInWorkflow.Contains(x.InstrumentID)).ToList();
                foreach (var item in queueInfoToRemove)
                {
                    queueInfo.Remove(item);
                }
            }

            response.objSRMEventsInfo = InitiateWorkflow(ref inputInfo, queueInfo, userName, true, conMgr, dictEventConfiguration, timestamp);

            if (!inputInfo.isPassed)
                throw new Exception(inputInfo.message);

            foreach (var item in queueInfo)
            {
                item.RadInstanceID = inputInfo.InstrumentIDVsRADWorkflowInfo[item.InstrumentID].RadWorkflowInstanceId;
                item.RuleMappingID = inputInfo.InstrumentIDVsRADWorkflowInfo[item.InstrumentID].RuleMappingId;
            }

            GenerateRequestQueue(queueInfo, conMgr);

            return response;
        }

        public static Dictionary<string, WorkflowInstrumentInfo> CheckInstrumentsInWorkflow(List<string> lstInstruments, int WorkflowInstanceId, bool checkByRadInstanceId = false, int moduleId = 0)
        {
            XElement xmlInstruments = new XElement("instruments", lstInstruments.Select(x => new XElement("instrument", new XAttribute("id", x))));
            XmlDocument xDocInstruments = new XmlDocument();
            xDocInstruments.Load(xmlInstruments.CreateReader());

            return CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @workflow_instance_id INT = {0}, @xml XML = '{1}'

            DECLARE @idoc INT;

            EXEC sp_xml_preparedocument @idoc OUTPUT, @xml;

            SELECT id
            INTO #instrument_ids
            FROM OPENXML(@idoc,'/instruments/instrument',2)
            WITH (id VARCHAR(MAX) '@id');

            EXEC sp_xml_removedocument @idoc;

            SELECT rque.instrument_id, rque.rad_workflow_instance_id, rque.queue_id, rque.rule_mapping_id
            FROM #instrument_ids instruments
            INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rque
            ON rque.{2} = instruments.id {3} AND is_active = 1" +
            (moduleId == 0 ? string.Empty :
            @"INNER JOIN[IVPRefMaster].[dbo].[ivp_srm_workflow_instance] wi
            ON wi.workflow_instance_id = rque.workflow_instance_id WHERE wi.module_id = " + moduleId) + @"

            DROP TABLE #instrument_ids", WorkflowInstanceId, xDocInstruments.InnerXml, checkByRadInstanceId ? "rad_workflow_instance_id" : "instrument_id", checkByRadInstanceId || WorkflowInstanceId == 0 ? "" : "AND rque.workflow_instance_id = @workflow_instance_id"), ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["instrument_id"]), y => new WorkflowInstrumentInfo { RadInstanceID = Convert.ToInt32(y["rad_workflow_instance_id"]), QueueID = Convert.ToInt32(y["queue_id"]), RuleMappingID = Convert.ToInt32(y["rule_mapping_id"]) });
        }

        private static Dictionary<string, int> GetInstrumentVsInstance(List<string> instruments, List<int> instances, RDBConnectionManager conMgr = null)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = 0; i < instruments.Count; i++)
            {
                dict.Add(instruments[i], instances[i]);
            }
            return dict;
        }

        /// <summary>
        /// Method to insert request queue when a workflow is initiated
        /// </summary>
        /// <param name="queueInfo">Queue info per instrument</param>
        /// <param name="conMgr"></param>
        public static void GenerateRequestQueue(List<WorkflowQueueInfo> queueInfo, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("GenerateRequestQueue -> Start");
                string instrumentXML = SRMWorkflowUtils.GetInstrumentXML(queueInfo);
                SRMWorkflowDBHandler.InsertRequestQueue(instrumentXML, conMgr);
                mLogger.Debug("GenerateRequestQueue -> End");
            }
            catch (Exception ex)
            {
                mLogger.Error("GenerateRequestQueue -> Error: " + ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Method to get action history of a particular workflow request
        /// </summary>
        /// <param name="radInstanceID">Rad instance id</param>
        /// <param name="moduleID">Module ID</param>
        /// <returns></returns>
        public static List<WorkflowActionHistoryPerRequest> GetWorkflowActionHistoryByInstance(int radInstanceID, int moduleID, string dateFormat, string dateTimeFormat, string timeFormat, string userName)
        {
            List<WorkflowActionHistory> actionHistory = new List<WorkflowActionHistory>();
            WorkflowActionHistoryPerRequest currentInstanceHistory = new WorkflowActionHistoryPerRequest();
            List<WorkflowActionHistoryPerRequest> actionHistoryPerRequest = new List<WorkflowActionHistoryPerRequest>();
            try
            {
                mLogger.Debug("GetWorkflowActionHistoryByInstance -> Start");
                mLogger.Debug("Instance ID: " + radInstanceID.ToString());
                mLogger.Debug("Module ID: " + moduleID.ToString());
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                Dictionary<int, DateTime> dictAuditLevel = new Dictionary<int, DateTime>();
                string statusUser = userName;
                DateTime pendingDate = DateTime.Now;
                string requestedBy = string.Empty;
                string requestedOn = string.Empty;
                DateTime requestDateTime = DateTime.Now;
                DateTime auditStartDate = DateTime.Now;
                DateTime auditEndDate = DateTime.Now;
                string workflowType = string.Empty;

                Dictionary<string, string> ActionVsHistoryDisplay = new Dictionary<string, string>();
                ActionVsHistoryDisplay = SRMWorkflowUtils.PopulateActionDictionary();
                workflowType = SRMWorkflowDBHandler.GetWorkflowTypeByRadInstanceID(radInstanceID);

                WorkflowActionHistory history = null;
                int level = 1;

                Dictionary<string, string> dictAllUsers = new SRMRADWorkflow().GetAllUserNames();

                List<RWorkFlowInstanceInfo> rInfo = new SRMRADWorkflow().GetWorkflowInstanceAudit(radInstanceID);

                if (rInfo != null && rInfo.Count > 0)
                {

                    rInfo = rInfo.OrderBy(r => r.ActionDate).ToList();

                    WorkflowInitiatedInfo initiatedInfo = SRMWorkflowDBHandler.GetWorkflowInitiatedInfo(radInstanceID);

                    if (initiatedInfo != null)
                    {
                        currentInstanceHistory.InstanceID = radInstanceID;
                        currentInstanceHistory.RequestedBy = SRMCommonRAD.GetUserDisplayNameFromUserName(initiatedInfo.InitiatedBy);
                        currentInstanceHistory.RequestedOn = initiatedInfo.InitiatedOn.ToString();
                        startDate = initiatedInfo.InitiatedOn;
                    }
                    else
                    {
                        mLogger.Debug("Initiation Info not found");
                    }

                    //startDate = rInfo.Min(r => r.ActionDate);
                    endDate = rInfo.Max(r => r.ActionDate);

                    dictAuditLevel.Add(level, startDate);

                    rInfo.OrderBy(r => r.ActionDate).ToList().ForEach(r =>
                    {
                        level++;
                        dictAuditLevel.Add(level, r.ActionDate);
                    });

                    level = 0;

                    rInfo.Where(ri => ActionVsHistoryDisplay.ContainsKey(ri.ActionPerformed.ToLower())).ToList().ForEach(r =>
                    {
                        level++;
                        history = new WorkflowActionHistory();
                        history.ActualActionTime = r.ActionDate;
                        history.Status = ActionVsHistoryDisplay[r.ActionPerformed.ToLower()];
                        if (dictAllUsers != null && dictAllUsers.Keys.Contains(r.ActionPerformedBy))
                            history.UserName = dictAllUsers[r.ActionPerformedBy];
                        else
                            history.UserName = r.ActionPerformedBy;
                        history.Remarks = (r.Comment != null && r.Comment.Count > 0) ? r.Comment[0].comment : string.Empty;
                        history.ActionDate = r.ActionDate.ToShortDateString();
                        history.LevelName = r.CurrentState;
                        history.ActionLevel = level;
                        history.AuditLevel = level;
                        history.ActionTime = r.ActionDate.ToShortTimeString();
                        history.Attributes = new List<AttributeAudit>();
                        actionHistory.Add(history);
                    });


                }
                else
                    mLogger.Debug("No instance audit returned from rad.");

                List<RWorkFlowInstanceInfo> pendingInfo = new SRMRADWorkflow().GetActionOfInstanceForUser(new List<int>() { radInstanceID }, statusUser);
                if (pendingInfo != null && pendingInfo.Count > 0 && pendingInfo[0].CurrentState != null && pendingInfo[0].CurrentState.ToLower() != endState && pendingInfo[0].CurrentState.ToLower() != failedState)
                {
                    endDate = pendingDate;
                    level++;
                    history = new WorkflowActionHistory();
                    history.Status = ActionVsHistoryDisplay["pending"];
                    history.LevelName = pendingInfo[0].CurrentState;
                    history.AuditLevel = level;
                    history.ActualActionTime = DateTime.Now;
                    history.Attributes = new List<AttributeAudit>();
                    actionHistory.Add(history);

                    int newLevel = 1;
                    if (dictAuditLevel != null && dictAuditLevel.Keys.Count > 0)
                    {
                        newLevel = dictAuditLevel.Keys.Max(k => k) + 1;
                    }

                    dictAuditLevel.Add(newLevel, pendingDate);
                }



                //Setting Audit History
                if (actionHistory != null && actionHistory.Count > 0)
                {
                    //Start Setting audit start and end date per action
                    if (dictAuditLevel != null)
                    {
                        actionHistory.ForEach(ah =>
                        {
                            if (dictAuditLevel.ContainsKey(ah.AuditLevel + 1))
                            {
                                ah.AuditStartDate = dictAuditLevel[ah.AuditLevel];
                                ah.AuditEndDate = dictAuditLevel[ah.AuditLevel + 1];
                            }
                        });
                    }
                    //End Setting audit start and end date per action


                    List<AttributeAudit> attributeAudit = GetAttributeAudit(radInstanceID, moduleID, dateFormat, dateTimeFormat, timeFormat, userName, startDate,
                        endDate, dictAuditLevel, workflowType);
                    if (attributeAudit != null)
                    {
                        actionHistory.ForEach(ah =>
                        {
                            ah.Attributes.AddRange(attributeAudit.Where(a => a.AuditLevel == ah.AuditLevel).ToList());
                        });
                    }


                }

            }
            catch (Exception ex)
            {
                mLogger.Debug("GetWorkflowActionHistoryByInstance -> Error: " + ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetWorkflowActionHistoryByInstance -> End");
            }

            currentInstanceHistory.Actions = actionHistory;
            actionHistoryPerRequest.Add(currentInstanceHistory);

            //var sessionInfo = new RCommon().SessionInfo;
            //var dFormat = sessionInfo.CultureInfo.ShortDateFormat;
            //var dateTimeFormat = sessionInfo.CultureInfo.LongDateFormat;

            var systemDateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            var systemDateTimeFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern;

            foreach (var obj in actionHistoryPerRequest)
            {
                if (!string.IsNullOrEmpty(obj.RequestedOn))
                {
                    DateTime dt;
                    DateTime.TryParseExact(obj.RequestedOn, systemDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out dt);
                    obj.RequestedOn = dt.ToString(dateTimeFormat);
                }

                foreach (var obj2 in obj.Actions)
                {
                    if (!string.IsNullOrEmpty(obj2.ActionDate))
                    {
                        DateTime dt;
                        DateTime.TryParseExact(obj2.ActionDate, systemDateFormat, null, System.Globalization.DateTimeStyles.None, out dt);
                        obj2.ActionDate = dt.ToString(dateFormat);
                    }
                }
            }

            return actionHistoryPerRequest;
        }


        public static List<WorkflowActionHistoryPerRequest> GetWorkflowActionHistoryByInstrument(string instrumentID, int moduleID, string dateFormat, string dateTimeFormat, string timeFormat, string userName)
        {
            List<int> instances = new List<int>();
            List<WorkflowActionHistory> finalActionHistory = new List<WorkflowActionHistory>();
            List<WorkflowActionHistoryPerRequest> ah = new List<WorkflowActionHistoryPerRequest>();
            List<WorkflowActionHistoryPerRequest> historyPerRequest = new List<WorkflowActionHistoryPerRequest>();
            instances = SRMWorkflowDBHandler.GetInstancesByInstrumentID(instrumentID, moduleID);

            if (instances != null && instances.Count > 0)
            {
                instances.OrderBy(i => i).ToList().ForEach(ins =>
                {
                    ah = GetWorkflowActionHistoryByInstance(ins, moduleID, dateFormat, dateTimeFormat, timeFormat, userName);
                    if (ah != null && ah.Count > 0)
                    {
                        historyPerRequest.AddRange(ah);
                    }
                });
            }
            return historyPerRequest;
        }

        public static List<AttributeVsValue> GetAttributeVsValue(List<int> instances, int moduleID, string dateFormat, string dateTimeFormat)
        {
            List<AttributeVsValue> attrVsValue = new List<AttributeVsValue>();

            if (moduleID == 3)
            {
                attrVsValue = SMGetAttributeVsValue(instances, dateFormat, dateTimeFormat);
            }
            else
            {
                attrVsValue = RPFMGetAttributeVsValue(instances, dateFormat);
            }

            return attrVsValue;
        }

        private static List<AttributeVsValue> PrepareAttributeVsValueCollection(List<AttributeVsValue> attrVsValue, DataSet ds, string dateFormat, int moduleId, string dateTimeFormat = null)
        {
            mLogger.Debug("Prepare AttributeVsValue Collection ---> Start");
            if (moduleId == 3)
            {
                if (ds != null && ds.Tables.Count > 1 && ds.Tables[ds.Tables.Count - 1].Rows.Count > 0)
                {
                    Dictionary<int, Dictionary<int, Dictionary<string, KeyValuePair<string, HashSet<bool>>>>> workflowVsAttributeDetails = ds.Tables[ds.Tables.Count - 1].AsEnumerable().GroupBy(x => Convert.ToInt32(x["workflow_instance_id"])).ToDictionary(x => x.Key, y => y.GroupBy(a => Convert.ToInt32(a["type_id"])).ToDictionary(typ => typ.Key, tdet => tdet.GroupBy(attr => Convert.ToString(attr["display_name"])).ToDictionary(attr => attr.Key, adet => new KeyValuePair<string, HashSet<bool>>(Convert.ToString(adet.First()["data_type_name"]), new HashSet<bool>(adet.Select(prim => Convert.ToBoolean(prim["is_primary"])))))));
                    for (int index = 0; index < ds.Tables.Count - 1; index++)
                    {
                        attrVsValue.AddRange(ds.Tables[index].AsEnumerable().Select(x => workflowVsAttributeDetails[Convert.ToInt32(x["workflow_instance_id"])][Convert.ToInt32(x["type_id"])].Select(attr => attr.Value.Value.Select(prim => new AttributeVsValue
                        {
                            InstanceID = Convert.ToInt32(x["rad_workflow_instance_id"]),
                            AttributeName = attr.Key,
                            //AttributeValue = attr.Value.Key.SRMEqualWithIgnoreCase("DATE") ? !string.IsNullOrEmpty(Convert.ToString(x[attr.Key])) ? Convert.ToDateTime(x[attr.Key]).ToString(dateFormat) : Convert.ToString(x[attr.Key]) : Convert.ToString(x[attr.Key]), isPrimary = prim })).SelectMany(a => a)).SelectMany(b => b));
                            AttributeValue = !string.IsNullOrEmpty(Convert.ToString(x[attr.Key])) ? attr.Value.Key.SRMEqualWithIgnoreCase("DATE") ? Convert.ToDateTime(x[attr.Key]).ToString(dateFormat) : attr.Value.Key.SRMEqualWithIgnoreCase("DATETIME") ? Convert.ToDateTime(x[attr.Key]).ToString(dateTimeFormat) : Convert.ToString(x[attr.Key]) : Convert.ToString(x[attr.Key]),
                            isPrimary = prim
                        })).SelectMany(a => a)).SelectMany(b => b));
                    }
                }

                try
                {
                    //sec lookup massaging
                    Dictionary<int, RMAttributeValueInfoOptimized> lookupDict = new Dictionary<int, RMAttributeValueInfoOptimized>();
                    Dictionary<string, Dictionary<string, string>> dictSecIdVsAttributeVsInfo = new Dictionary<string, Dictionary<string, string>>();
                    Dictionary<string, string> dispVsRealName = new Dictionary<string, string>();

                    if (ds != null && ds.Tables.Count == 2)
                    {
                        foreach (var row in ds.Tables[1].AsEnumerable())
                        {
                            var objectRowInAttributeIdList = row;
                            string attributeTypeName = Convert.ToString(objectRowInAttributeIdList["attribute_type_name"]);
                            if (attributeTypeName.ToLower() == "reference")
                            {
                                int entityTypeId = Convert.ToInt32(objectRowInAttributeIdList["reference_type_id"]);
                                string entityAttrName = Convert.ToString(objectRowInAttributeIdList["reference_attribute_name"]);
                                string newValue = Convert.ToString(row["value"]);
                                if (!lookupDict.ContainsKey(entityTypeId))
                                {
                                    HashSet<string> temp = new HashSet<string>();
                                    lookupDict.Add(entityTypeId, new RMAttributeValueInfoOptimized() { EntityTypeId = entityTypeId, AttributeList = new HashSet<string>() { entityAttrName }, EntityCodeList = temp });
                                    if (!string.IsNullOrEmpty(newValue))
                                    {
                                        temp.Add(newValue);
                                    }
                                }
                                else
                                {
                                    lookupDict[entityTypeId].AttributeList.Add(entityAttrName);
                                    if (!string.IsNullOrEmpty(newValue))
                                    {
                                        lookupDict[entityTypeId].EntityCodeList.Add(newValue);
                                    }
                                }
                                if (!dispVsRealName.ContainsKey(Convert.ToString(row["display_name"])))
                                {
                                    dispVsRealName.Add(Convert.ToString(row["display_name"]), Convert.ToString(row["attribute_name"]));
                                }
                            }
                        }
                    }

                    //lookup massage sec
                    if (lookupDict != null && lookupDict.Count > 0)
                    {
                        Assembly SecMasterRefDataInterfaceLayer = Assembly.Load("SecMasterRefDataInterfaceLayer");
                        Type smRefDataMassage = SecMasterRefDataInterfaceLayer.GetType("com.ivp.secm.refdatainterfacelayer.SMRefDataMassage");
                        MethodInfo getMasssagedData = smRefDataMassage.GetMethod("GetMasssagedData");
                        var smRefDataMassageObj = Activator.CreateInstance(smRefDataMassage);
                        Dictionary<int, RMAttributeValueInfoOptimized> tempo = new Dictionary<int, RMAttributeValueInfoOptimized>();
                        List<RMAttributeValueInfoOptimized> lookupDictList = (List<RMAttributeValueInfoOptimized>)getMasssagedData.Invoke(smRefDataMassageObj, new object[] { lookupDict.Values.ToList() });
                        Parallel.ForEach(lookupDict, kvp =>
                        {
                            RMAttributeValueInfoOptimized obj = lookupDictList.Where(x => x.EntityTypeId == kvp.Key).FirstOrDefault();
                            lock (((System.Collections.IDictionary)tempo).SyncRoot)
                            {
                                tempo[kvp.Key] = obj;
                            }
                        });
                        lookupDict = tempo;

                        foreach (var kvp in lookupDict)
                        {
                            foreach (var item in kvp.Value.ResultSet.AsEnumerable())
                            {
                                string eCode = Convert.ToString(item["entity_code"]);
                                Dictionary<string, string> attrInfoVsValue = new Dictionary<string, string>();
                                dictSecIdVsAttributeVsInfo.Add(eCode, attrInfoVsValue);
                                foreach (var item1 in kvp.Value.AttributeList)
                                {
                                    attrInfoVsValue.Add(item1, Convert.ToString(item[item1]));
                                }
                            }
                        }

                        foreach (var row in ds.Tables[1].AsEnumerable())
                        {
                            var objectRowInAttributeIdList = row;
                            string attributeTypeName = Convert.ToString(objectRowInAttributeIdList["attribute_type_name"]);

                            if (attributeTypeName.ToLower() == "reference")
                            {
                                string displayName = Convert.ToString(row["display_name"]);
                                string entityAttrName = Convert.ToString(objectRowInAttributeIdList["reference_attribute_name"]);
                                var lstOfRows = attrVsValue.Where(x => x.AttributeName.Equals(displayName));
                                if (lstOfRows != null && lstOfRows.Count() > 0)
                                {
                                    foreach (var item in lstOfRows)
                                    {
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(item.AttributeValue))
                                            {
                                                item.AttributeValue = dictSecIdVsAttributeVsInfo[item.AttributeValue][entityAttrName];
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                DataTable dtAVS = null;
                DataTable dtAttributes = null;
                if (ds != null && ds.Tables.Count > 1 && ds.Tables[0] != null && ds.Tables[1] != null)
                {
                    dtAVS = ds.Tables[0];

                    dtAttributes = ds.Tables[1];

                    Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                    Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMWorkFlowControllerNew");
                    MethodInfo GetEntityAudit = objType.GetMethod("MassageAttributesForWorkflowInbox");

                    object obj = Activator.CreateInstance(objType);
                    dtAVS = (DataTable)GetEntityAudit.Invoke(obj, new object[] { dtAVS, dtAttributes, dateFormat });

                    attrVsValue = SRMWorkflowUtils.RPFMGetAttributeValueFromDatatable(dtAVS);
                }
            }
            mLogger.Debug("Prepare AttributeVsValue Collection ---> End");
            return attrVsValue;
        }
        private static List<AttributeVsValue> SMGetAttributeVsValue(List<int> instances, string dateFormat, string dateTimeFormat)
        {
            List<AttributeVsValue> attrVsValue = new List<AttributeVsValue>();

            XElement xmlInstruments = new XElement("instances", instances.Select(x => new XElement("instance", new XAttribute("id", x))));
            XmlDocument xDocInstruments = new XmlDocument();
            xDocInstruments.Load(xmlInstruments.CreateReader());

            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_GetWorkflowAttributesToShow '{0}'", xDocInstruments.InnerXml), ConnectionConstants.RefMaster_Connection);

            attrVsValue = PrepareAttributeVsValueCollection(attrVsValue, ds, dateFormat, 3, dateTimeFormat);

            return attrVsValue;
        }

        private static List<AttributeVsValue> RPFMGetAttributeVsValue(List<int> instances, string dateFormat)
        {
            List<AttributeVsValue> attrVsValue = new List<AttributeVsValue>();
            DataSet dsAVS = null;

            dsAVS = SRMWorkflowDBHandler.RPFMGetAttributeValuesForWorflowInbox(instances);

            attrVsValue = PrepareAttributeVsValueCollection(attrVsValue, dsAVS, dateFormat, 6);

            return attrVsValue;
        }


        private static List<AttributeAudit> GetAttributeAudit(int instanceID, int moduleID, string dateFormat, string dateTimeFormat, string timeFormat, string userName, DateTime startDate, DateTime endDate, Dictionary<int, DateTime> auditLevels, string workflowType)
        {
            List<AttributeAudit> attrVsValue = new List<AttributeAudit>();

            if (moduleID == 3)
            {
                attrVsValue = SMGetAttributeAudit(instanceID, dateFormat, dateTimeFormat, timeFormat, startDate, endDate, null, null, userName, auditLevels, moduleID, workflowType);
            }
            else
            {
                attrVsValue = RPFMGetAttributeAudit(instanceID, dateFormat, dateTimeFormat, startDate, endDate, null, null, userName, auditLevels, moduleID, workflowType);
            }

            return attrVsValue;
        }


        private static List<AttributeAudit> SMGetAttributeAudit(int instanceID, string dateFormat, string dateTimeFormat, string timeFormat, DateTime? startKnowledgeDate, DateTime? endKnowledgeDate, DateTime? startLoadingTime, DateTime? endLoadingTime, string userName, Dictionary<int, DateTime> auditLevels, int moduleID, string workflowType)
        {
            List<AttributeAudit> attrVsValue = new List<AttributeAudit>();
            string instrumentID = string.Empty;

            instrumentID = SRMWorkflowDBHandler.GetInstrumentIDByInstanceID(instanceID);
            attrVsValue.AddRange(SMGetAttributeAuditPerInstance(instrumentID, dateFormat, dateTimeFormat, timeFormat, startKnowledgeDate, endKnowledgeDate, startLoadingTime, endLoadingTime, userName, auditLevels, moduleID, workflowType));

            return attrVsValue;
        }

        private static List<AttributeAudit> RPFMGetAttributeAudit(int instanceID, string dateFormat, string dateTimeFormat, DateTime? startKnowledgeDate, DateTime? endKnowledgeDate, DateTime? startLoadingTime, DateTime? endLoadingTime, string userName, Dictionary<int, DateTime> auditLevels, int moduleID, string workflowType)
        {
            List<AttributeAudit> attrVsValue = new List<AttributeAudit>();
            string instrumentID = string.Empty;

            instrumentID = SRMWorkflowDBHandler.GetInstrumentIDByInstanceID(instanceID);
            attrVsValue.AddRange(RPFMGetAttributeAuditPerInstance(instrumentID, dateFormat, dateTimeFormat, startKnowledgeDate, endKnowledgeDate, startLoadingTime, endLoadingTime, userName, auditLevels, moduleID, workflowType));

            return attrVsValue;
        }

        private static List<AttributeAudit> SMGetAttributeAuditPerInstance(string instrumentID, string dateFormat, string dateTimeFormat, string timeFormat, DateTime? startKnowledgeDate, DateTime? endKnowledgeDate, DateTime? startLoadingTime, DateTime? endLoadingTime, string userName, Dictionary<int, DateTime> auditLevels, int moduleID, string workflowType)
        {
            List<AttributeAudit> attrVsValue = new List<AttributeAudit>();
            DataTable dtAudit = new DataTable();

            Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
            Type objType = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMWorkFlowController");
            MethodInfo GetSecurityAuditForWorkflow = objType.GetMethod("GetSecurityAuditForWorkflow");

            object obj = Activator.CreateInstance(objType);
            dtAudit = (DataTable)GetSecurityAuditForWorkflow.Invoke(obj, new object[] { instrumentID, userName, startKnowledgeDate, endKnowledgeDate, workflowType });

            var attributeIdVsDatatype = CommonDALWrapper.ExecuteSelectQuery(@"SELECT attribute_id, ast.data_type_name 
                FROM IVPSecMaster.dbo.ivp_secm_attribute_details ad
                INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_subtype ast ON ad.attribute_subtype_id = ast.attribute_subtype_id
                WHERE data_type_name IN ('DATE', 'DATETIME')", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToInt32(x["attribute_id"]), y => Convert.ToString(y["data_type_name"]).ToUpper());

            //var sessionInfo = new RCommon().SessionInfo;
            //var dateFormat = sessionInfo.CultureInfo.ShortDateFormat;
            //var dateTimeFormat = sessionInfo.CultureInfo.LongDateFormat;

            var systemDateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            var systemDateTimeFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern;
            var systemTimeFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern;

            string systemDateTimeFormatWithoutTT = systemDateTimeFormat;
            string systemTimeFormatWithoutTT = systemTimeFormat;

            if (systemDateTimeFormat.Contains("tt"))
                systemDateTimeFormatWithoutTT = systemDateTimeFormatWithoutTT.Remove(systemDateTimeFormat.Length - 3, 3);
            if (systemTimeFormat.Contains("tt"))
                systemTimeFormatWithoutTT = systemTimeFormatWithoutTT.Remove(systemTimeFormat.Length - 3, 3);

            foreach (var row in dtAudit.AsEnumerable())
            {
                var attrId = Convert.ToString(row["attribute_id"]);
                if (!string.IsNullOrEmpty(attrId))
                {
                    var attributeId = Convert.ToInt32(row["attribute_id"]);
                    string dataType = null;

                    if (attributeIdVsDatatype.TryGetValue(attributeId, out dataType))
                    {
                        var oldValue = Convert.ToString(row["Old Value"]);
                        var newValue = Convert.ToString(row["New Value"]);
                        switch (dataType)
                        {
                            case "DATE":
                                if (!string.IsNullOrEmpty(oldValue))
                                {
                                    DateTime tempDateTime;
                                    DateTime.TryParseExact(oldValue, systemDateFormat, null, System.Globalization.DateTimeStyles.None, out tempDateTime);
                                    row["Old Value"] = tempDateTime.ToString(dateFormat);
                                }
                                if (!string.IsNullOrEmpty(newValue))
                                {
                                    DateTime tempDateTime;
                                    DateTime.TryParseExact(newValue, systemDateFormat, null, System.Globalization.DateTimeStyles.None, out tempDateTime);
                                    row["New Value"] = tempDateTime.ToString(dateFormat);
                                }
                                break;
                            case "DATETIME":
                                if (!string.IsNullOrEmpty(oldValue))
                                {
                                    DateTime tempDateTime;
                                    if (DateTime.TryParseExact(oldValue, systemDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out tempDateTime) || DateTime.TryParseExact(oldValue, systemDateTimeFormatWithoutTT, null, System.Globalization.DateTimeStyles.None, out tempDateTime))
                                        row["Old Value"] = tempDateTime.ToString(dateTimeFormat);
                                }
                                if (!string.IsNullOrEmpty(newValue))
                                {
                                    DateTime tempDateTime;
                                    if (DateTime.TryParseExact(newValue, systemDateTimeFormat, null, System.Globalization.DateTimeStyles.None, out tempDateTime) || DateTime.TryParseExact(newValue, systemDateTimeFormatWithoutTT, null, System.Globalization.DateTimeStyles.None, out tempDateTime))
                                        row["New Value"] = tempDateTime.ToString(dateTimeFormat);
                                }
                                break;
                            case "TIME":
                                if (!string.IsNullOrEmpty(oldValue))
                                {
                                    TimeSpan tempTime;
                                    if (TimeSpan.TryParseExact(oldValue, systemTimeFormat, null, out tempTime) || TimeSpan.TryParseExact(oldValue, systemTimeFormatWithoutTT, null, out tempTime))
                                        row["Old Value"] = tempTime.ToString(timeFormat);
                                }
                                if (!string.IsNullOrEmpty(newValue))
                                {
                                    TimeSpan tempTime;
                                    if (TimeSpan.TryParseExact(newValue, systemTimeFormat, null, out tempTime) || TimeSpan.TryParseExact(newValue, systemTimeFormatWithoutTT, null, out tempTime))
                                        row["New Value"] = tempTime.ToString(timeFormat);
                                }
                                break;
                        }
                    }
                }
            }

            if (dtAudit.Columns.Contains("attribute_id"))
                dtAudit.Columns.Remove("attribute_id");

            //Added By Dhruv to Massage Modified By
            Dictionary<string, string> allUsers = new Dictionary<string, string>();

            RUserManagementService objUserController = new RUserManagementService();
            List<RUserInfo> radUsers = objUserController.GetAllUsersGDPR();

            foreach (var info in radUsers)
            {
                allUsers.Add(info.UserLoginName, info.FullName + " (" + info.UserName + ")");
                allUsers.Add("TS:" + info.UserLoginName, "TS:" + info.FullName + " (" + info.UserName + ")");
                allUsers.Add("TS Inserted : " + info.UserLoginName, "TS Inserted : " + info.FullName + " (" + info.UserName + ")");
            }

            dtAudit.AsEnumerable().Where(x => allUsers.ContainsKey(x.Field<string>("Modified By"))).ToList().ForEach(x => x.SetField<string>("Modified By", allUsers[x.Field<string>("Modified By")]));

            if (dtAudit != null && dtAudit.Rows.Count > 0)
                attrVsValue = SRMWorkflowUtils.BreakAuditLevels(dtAudit, auditLevels, "Knowledge Date", moduleID, dateTimeFormat);
            //attrVsValue = SRMWorkflowUtils.ParseActionHistoryAudit(dtAudit, "Knowledge Date", moduleID, startKnowledgeDate.Value, endKnowledgeDate.Value);


            return attrVsValue;
        }

        private static List<AttributeAudit> RPFMGetAttributeAuditPerInstance(string instrument, string dateFormat, string dateTimeFormat, DateTime? startKnowledgeDate, DateTime? endKnowledgeDate, DateTime? startLoadingTime, DateTime? endLoadingTime, string userName, Dictionary<int, DateTime> auditLevels, int moduleID, string workflowType)
        {
            List<AttributeAudit> attrVsValue = new List<AttributeAudit>();
            DataTable dtAudit = null;

            //var sessionInfo = new RCommon().SessionInfo;
            //var dFormat = sessionInfo.CultureInfo.ShortDateFormat;
            //var dateTimeFormat = sessionInfo.CultureInfo.LongDateFormat;                        

            //RMEntityAuditPerAttributeInfo input = new RMEntityAuditPerAttributeInfo();
            //input.EntityCodes.Add(instrument);
            //input.entityAttributeIds = new List<int>() { -1 };
            //input.StartKnowledgeDate = startKnowledgeDate;
            //input.EndKnowledgeDate = endKnowledgeDate;
            //input.StartLoadingTime = startLoadingTime;
            //input.EndLoadingTime = endLoadingTime;

            Assembly RefMControllerAssembly = Assembly.Load("RefMController");
            Type auditInputType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMEntityAuditPerAttributeInfo");
            object objAuditInfo = Activator.CreateInstance(auditInputType);

            PropertyInfo[] properties = objAuditInfo.GetType().GetProperties();
            foreach (PropertyInfo propinfo in properties)
            {
                if (propinfo.Name.Equals("EntityCodes"))
                    propinfo.SetValue(objAuditInfo, new List<string> { instrument }, null);
                else if (propinfo.Name.Equals("StartLoadingTime"))
                    propinfo.SetValue(objAuditInfo, startLoadingTime, null);
                else if (propinfo.Name.Equals("EndLoadingTime"))
                    propinfo.SetValue(objAuditInfo, endLoadingTime, null);
                else if (propinfo.Name.Equals("StartKnowledgeDate"))
                    propinfo.SetValue(objAuditInfo, startKnowledgeDate, null);
                else if (propinfo.Name.Equals("EndKnowledgeDate"))
                    propinfo.SetValue(objAuditInfo, endKnowledgeDate, null);
                else if (propinfo.Name.Equals("entityAttributeIds"))
                    propinfo.SetValue(objAuditInfo, new List<int> { -1 }, null);
                else if (propinfo.Name.Equals("EntityTypeID"))
                    propinfo.SetValue(objAuditInfo, -1, null);

            }


            Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMEntityAuditTrail");
            MethodInfo GetEntityAudit = objType.GetMethod("GetAuditHistory");

            object obj = Activator.CreateInstance(objType);
            dtAudit = (DataTable)GetEntityAudit.Invoke(obj, new object[] { objAuditInfo, "", dateFormat, true, workflowType, false, true, false, false });

            if (dtAudit != null && dtAudit.Rows.Count > 0)
                attrVsValue = SRMWorkflowUtils.BreakAuditLevels(dtAudit, auditLevels, "Knowledge Date", moduleID, dateTimeFormat);

            return attrVsValue;
        }

        /// <summary>
        /// Method to get workflow inbox data based on type of workflow and status
        /// </summary>
        /// <param name="moduleID">Module ID</param>
        /// <param name="userName">User Name</param>
        /// <param name="workflowType">Type of Workflow (Create/Update etc.)</param>
        /// <param name="statusType">Status to get the data for (Pending at user, raised by user etc.)</param>
        /// <returns></returns>
        public static List<WorkflowQueueData> GetWorkflowQueueData(int moduleID, string userName, string workflowType, string statusType, string dateFormat, string dateTimeFormat)
        {
            List<WorkflowQueueData> queueData = new List<WorkflowQueueData>();
            try
            {
                mLogger.Debug("GetWorkflowQueueData -> Start");
                if (string.IsNullOrEmpty(dateTimeFormat))
                {
                    var cultureInfo = new RCommon().SessionInfo.CultureInfo;
                    dateTimeFormat = cultureInfo.ShortDateFormat + " " + cultureInfo.LongTimePattern;
                }
                List<RWorkFlowInstanceInfo> radReturnedInstances = null;
                DataTable dtQueueData = null;
                List<int> instances = new List<int>();

                mLogger.Debug("Module ID: " + moduleID.ToString());
                mLogger.Debug("User Name: " + userName);
                mLogger.Debug("Workflow Type: " + workflowType);
                mLogger.Debug("Status: " + statusType);

                if (statusType == WorkflowStatusType.PENDING_AT_ME.ToString())
                {
                    radReturnedInstances = new SRMRADWorkflow().GetAllWOrkFlowInstances(WorkFlowStatus.pending, userName, null);//pending at me

                    if (radReturnedInstances != null)
                    {
                        instances.AddRange(radReturnedInstances.Select(r => r.InstanceID).Distinct().ToList());
                    }
                }

                radReturnedInstances = null;

                if (statusType == WorkflowStatusType.MY_REQUESTS.ToString())
                {
                    radReturnedInstances = new SRMRADWorkflow().GetAllWOrkFlowInstances(WorkFlowStatus.started, userName, null); //my requests

                    if (radReturnedInstances != null)
                    {
                        instances.AddRange(radReturnedInstances.Select(r => r.InstanceID).Distinct().ToList());
                    }

                    List<int> rejectedInstances = new SRMRADWorkflow().GetFinalWorkflowInstanceStateStatedByUser(userName, "Failed", moduleID); //my requests

                    if (rejectedInstances != null)
                    {
                        rejectedInstances.ForEach(rej =>
                        {
                            if (instances.Contains(rej))
                                instances.Remove(rej);
                        });
                    }

                }

                radReturnedInstances = null;

                if (statusType == WorkflowStatusType.REJECTED_REQUESTS.ToString())
                {
                    instances = new SRMRADWorkflow().GetFinalWorkflowInstanceStateStatedByUser(userName, "Failed", moduleID); //rejected requests
                }

                instances = FilterRadWorkflowInstanceIdsByUsername(instances, userName);

                if (instances != null && instances.Count > 0)
                {
                    mLogger.Debug("Getting workflow data for instruments.");
                    dtQueueData = SRMWorkflowDBHandler.GetWorkflowQueueData(moduleID, instances, workflowType);
                }

                if (dtQueueData != null && dtQueueData.Rows.Count > 0)
                {
                    mLogger.Debug("Preparing final workflow data.");
                    queueData = SRMWorkflowUtils.PrepareWorkflowQueueData(dtQueueData, userName, moduleID, dateFormat, dateTimeFormat);
                }

            }
            catch (Exception ex)
            {
                mLogger.Debug("GetWorkflowQueueData -> Error: " + ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetWorkflowQueueData -> End");
            }
            return queueData;
        }

        /// <summary>
        /// Method to get workflow inbox landing page data (count per workflow type and status) for a user
        /// </summary>
        /// <param name="moduleID">Module</param>
        /// <param name="userName">User Name</param>
        /// <returns></returns>
        public static List<SRMWorkflowCountPerType> GetWorkflowStatusCount(int moduleID, string userName)
        {
            List<SRMWorkflowCountPerType> countInfo = new List<SRMWorkflowCountPerType>();
            try
            {
                mLogger.Debug("GetWorkflowStatusCount -> Start");
                SRMWorkflowCountPerType info = null;
                List<RWorkFlowInstanceInfo> radReturnedInstances = null;
                List<int> rejectedInstances = null;
                Dictionary<int, List<int>> countPerType = new Dictionary<int, List<int>>();
                countPerType.Add(0, new List<int>()); //pending at me
                countPerType.Add(1, new List<int>()); //my requests
                countPerType.Add(2, new List<int>()); //rejected

                radReturnedInstances = new SRMRADWorkflow().GetAllWOrkFlowInstances(WorkFlowStatus.pending, userName, null); //pending at me

                if (radReturnedInstances != null)
                {
                    countPerType[0].AddRange(radReturnedInstances.Select(r => r.InstanceID).Distinct().ToList());
                }

                radReturnedInstances = null;
                radReturnedInstances = new SRMRADWorkflow().GetAllWOrkFlowInstances(WorkFlowStatus.started, userName, null); //my requests

                if (radReturnedInstances != null)
                {
                    countPerType[1].AddRange(radReturnedInstances.Where(ri => ri.CurrentState != null && ri.CurrentState.ToLower() != endState).Select(r => r.InstanceID).Distinct().ToList());
                }

                radReturnedInstances = null;
                rejectedInstances = new SRMRADWorkflow().GetFinalWorkflowInstanceStateStatedByUser(userName, "Failed", moduleID); //my requests

                if (rejectedInstances != null)
                {
                    countPerType[2].AddRange(rejectedInstances.Distinct());
                }

                if (countPerType[2] != null)
                {
                    countPerType[2].ForEach(rej =>
                    {
                        if (countPerType[1].Contains(rej))
                            countPerType[1].Remove(rej);
                    });
                }

                var allInstanceIds = countPerType.SelectMany(kvp => kvp.Value).Distinct().ToList();
                if (allInstanceIds.Count > 0)
                {
                    allInstanceIds = FilterRadWorkflowInstanceIdsByUsername(allInstanceIds, userName);

                    mLogger.Info("allInstanceIds count =" + allInstanceIds.Count);


                    HashSet<int> allInstanceIdsHashset = new HashSet<int>(allInstanceIds);

                    mLogger.Info("Parallel.ForEach start");

                    Parallel.ForEach(countPerType, countKVP =>
                    {
                        countKVP.Value.RemoveAll(instanceId => !allInstanceIdsHashset.Contains(instanceId));
                    });


                }

                Dictionary<int, WorkflowType> workflowTypes = null;
                workflowTypes = SRMWorkflowUtils.SetWorkflowTypeDictionary(workflowTypes);

                for (int i = 0; i < 2; i++)
                {
                    info = new SRMWorkflowCountPerType();
                    info.workflowType = workflowTypes[i].ToString();
                    countInfo.Add(info);
                }

                foreach (KeyValuePair<int, List<int>> kvp in countPerType)
                {
                    int status = kvp.Key;
                    List<int> instances = kvp.Value;

                    DataTable dtCount = bulkUploadInstances(instances, moduleID);

                    if (dtCount != null)
                    {
                        dtCount.AsEnumerable().ToList().ForEach(d =>
                        {
                            int count = Convert.ToInt32(d["instance_count"]);
                            SRMWorkflowCountPerType cpt = new SRMWorkflowCountPerType();
                            string wfType = Convert.ToString(d["workflow_type"]).ToUpper();
                            cpt = countInfo.Where(c => c.workflowType == wfType).FirstOrDefault();
                            if (cpt != null)
                            {
                                if (status == 0)
                                    cpt.pendingCount = count;
                                if (status == 1)
                                    cpt.myRequestsCount = count;
                                if (status == 2)
                                    cpt.rejectedCount = count;
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Debug("GetWorkflowStatusCount -> Error: " + ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetWorkflowStatusCount -> End");
            }
            return countInfo;
        }

        public static DataTable bulkUploadInstances(List<int> radWorkflowInstanceIds, int moduleID)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "_") + "_" + new Random().Next(1, 10000).ToString();
            string workflowRequestsTableNameOrg = "[workflowRequests_" + guid.ToString() + "]";
            string workflowRequestsTableName = "IVPRefMaster.dbo." + workflowRequestsTableNameOrg;
            DataTable dtCount = null;
            try
            {
                mLogger.Debug("SRMWorkflowController -> bulkUploadInstances -> Start");

                DataTable dt = new DataTable();
                dt.Columns.Add("instance_id", typeof(int));

                if (radWorkflowInstanceIds != null && radWorkflowInstanceIds.Count > 0)
                {
                    foreach (var instanceId in radWorkflowInstanceIds)
                    {
                        DataRow dr = dt.NewRow();
                        dr["instance_id"] = instanceId;
                        dt.Rows.Add(dr);
                    }
                }

                CommonDALWrapper.ExecuteSelectQuery(string.Format(@"CREATE TABLE {0} (instance_id INT)", workflowRequestsTableName), ConnectionConstants.RefMaster_Connection);

                CommonDALWrapper.ExecuteBulkUpload(workflowRequestsTableName, dt, ConnectionConstants.RefMaster_Connection);

                dtCount = SRMWorkflowDBHandler.GetWorkflowCountPerType(moduleID, workflowRequestsTableName);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                try
                {
                    CommonDALWrapper.ExecuteSelectQuery("DROP TABLE " + workflowRequestsTableName, ConnectionConstants.RefMaster_Connection);
                }
                catch { }

                mLogger.Debug("SRMWorkflowController -> bulkUploadInstances -> End");
            }
            return dtCount;
        }
        /// <summary>
        /// Method to check what the result of an action will be
        /// </summary>
        /// <param name="actionInfo"></param>
        /// <param name="actionName"></param>
        /// <param name="userName"></param>
        /// <param name="remarks"></param>
        /// <param name="moduleID"></param>
        /// <param name="conMgr"></param>
        /// <returns></returns>
        public static List<WorkflowActionInfo> CheckActionOutput(List<WorkflowActionInfo> actionInfo, string actionName, string userName, string remarks, int moduleID, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("CheckActionOutput -> Start");
                List<TriggerInfo> lstTriggerInfo = new List<TriggerInfo>();
                TriggerInfo tInfo = null;
                List<int> finalInstances = new List<int>();
                bool actionCompleted = false;

                Action<List<TriggerInfo>> callback = info =>
                {
                    new SRMRADWorkflow().MassageUserNamesTriggerInfo(info);
                    throw new SRMWorkFlowCheckException("Check Action Only.");
                };

                actionInfo.ForEach(action =>
                {
                    tInfo = new TriggerInfo();
                    tInfo.action = actionName;
                    tInfo.user = userName;
                    tInfo.comments = remarks;
                    tInfo.InstanceId = action.RadInstanceID;
                    lstTriggerInfo.Add(tInfo);
                });

                try
                {
                    actionCompleted = new SRMRADWorkflow().MoveWorkflow(lstTriggerInfo, callback);
                }
                catch (SRMWorkFlowCheckException ex)
                {
                    actionCompleted = true;
                    mLogger.Error(ex.ToString());
                }
                catch (Exception ex)
                {
                    throw;
                }

                var setState = from rad in lstTriggerInfo.AsEnumerable()
                               join act in actionInfo.AsEnumerable()
                               on rad.InstanceId equals act.RadInstanceID
                               select SetRadInstanceData(act, rad.previousState, rad.currentState, rad.ErrorMessage);

                setState.Count();


                return actionInfo;
            }
            catch (Exception ex)
            {
                mLogger.Error("CheckActionOutput -> Error: " + ex.ToString());
                actionInfo.ForEach(a =>
                {
                    a.isPassed = false;
                    a.ErrorMessage = ex.Message;
                });
                return actionInfo;
            }
            finally
            {
                mLogger.Debug("CheckActionOutput -> End");
            }
        }

        /// <summary>
        /// Method to perform an action (Approve, reject etc.)
        /// </summary>
        /// <param name="actionInfo"></param>
        /// <param name="actionName"></param>
        /// <param name="userName"></param>
        /// <param name="remarks"></param>
        /// <param name="moduleID"></param>
        /// <param name="conMgr"></param>
        /// <returns></returns>
        public static WorkflowTakeActionResponse TakeAction(List<WorkflowActionInfo> actionInfo, string actionName, string userName, string remarks, int moduleID, bool performFinalApproval = true, RDBConnectionManager conMgr = null, Dictionary<int, Dictionary<SRMEventActionType, SRMEventPreferenceInfo>> dictEventConfiguration = null)
        {
            //WorkflowActionOutput output = new WorkflowActionOutput() { isPassed = true, message = string.Empty };
            List<WorkflowActionInfo> actionOutput = new List<WorkflowActionInfo>();
            string guid = Guid.NewGuid().ToString();
            string RADWorkflowInstanceIdTableName = "IVPRefMaster.dbo.[RADWorkflowInstanceIdTable_" + guid + "]";
            try
            {
                mLogger.Debug("TakeAction -> Start");
                mLogger.Debug("action name- " + actionName);
                DateTime timestamp = DateTime.Now;
                var usersdic = GetMassagedUser();
                List<TriggerInfo> lstTriggerInfo = new List<TriggerInfo>();
                List<WorkflowActionInfo> actionInfoForRAD = new List<WorkflowActionInfo>();
                List<WorkflowActionInfo> finalActions = new List<WorkflowActionInfo>();
                List<WorkflowActionInfo> intermediateActions = new List<WorkflowActionInfo>();
                TriggerInfo tInfo = null;
                List<int> finalInstances = new List<int>();
                bool actionCompleted = false;
                List<WorkflowActionInfo> finalApprovalCU = null;
                List<WorkflowActionInfo> intermediateApprovals = null;
                actionOutput.AddRange(actionInfo);
                actionInfoForRAD.AddRange(actionInfo);
                List<WorkflowActionInfo> failedApprovals = new List<WorkflowActionInfo>();
                List<WorkflowActionInfo> cancelledByInitiator = new List<WorkflowActionInfo>();
                List<WorkflowActionInfo> cancelledToEndStage = new List<WorkflowActionInfo>();
                List<int> cancelledInstances = new List<int>();
                Dictionary<int, List<string>> cancelledInstrumentsPerType = new Dictionary<int, List<string>>();
                bool isUpdate = actionInfo[0].WorkflowType == WorkflowType.UPDATE ? true : false;
                List<int> instanceId = new List<int>();
                List<WorkflowActionInfo> actionInfoForCreateEmailStructure = new List<WorkflowActionInfo>();
                string usersInfoFromRAD = null;

                Dictionary<int, KeyValuePair<int, SRMEventInfo>> dictRadWorkflowInstanceIdVsEventsInfo = new Dictionary<int, KeyValuePair<int, SRMEventInfo>>();

                var radInstanceIdVsInstrumentId = actionInfo.ToDictionary(x => x.RadInstanceID, y => y.InstrumentID);

                SRMEventActionType eventType = SRMEventActionType.Workflow_Request_Approve;
                if (actionName.Equals(rejectAction, StringComparison.OrdinalIgnoreCase))
                    eventType = SRMEventActionType.Workflow_Request_Reject;
                else if (actionName.Equals(cancelAction, StringComparison.OrdinalIgnoreCase))
                    eventType = SRMEventActionType.Workflow_Request_Cancel;

                if (dictEventConfiguration == null)
                    dictEventConfiguration = new Dictionary<int, Dictionary<SRMEventActionType, SRMEventPreferenceInfo>>();

                Dictionary<string, WorkflowDetailsInfo> instrumentVsworkflowinstanceInfo = SRMWorkflowDBHandler.GetWorkflowDetailsVsInstrumentID(actionInfo.Select(e => e.RadInstanceID).ToList(), conMgr);

                foreach (var action in actionInfo)
                {
                    action.WorkflowInstanceID = instrumentVsworkflowinstanceInfo[action.InstrumentID].WorkflowInstanceId;
                }

                if (actionName.ToLower() == cancelAction)
                {
                    mLogger.Debug("Cancel Action Perfomed");
                    cancelledByInitiator = actionInfo.Where(a => !string.IsNullOrEmpty(a.RequestedBy) && a.RequestedBy.ToLower() == userName).ToList();
                    if (cancelledByInitiator != null && cancelledByInitiator.Count > 0)
                    {
                        mLogger.Debug("cancelled by initiator");
                        actionInfoForRAD = actionInfoForRAD.Except(cancelledByInitiator).ToList();
                        actionInfo = actionInfo.Except(cancelledByInitiator).ToList();
                        //actionOutput = actionOutput.Except(cancelledByInitiator).ToList();
                    }

                }

                DataTable RADWorkflowInstanceId = new DataTable();
                RADWorkflowInstanceId.Columns.Add("rad_workflow_instance_id", typeof(int));
                if (actionInfo.Count > 0)
                {
                    foreach (WorkflowActionInfo inf in actionInfo)
                    {
                        var row = RADWorkflowInstanceId.NewRow();
                        row["rad_workflow_instance_id"] = inf.RadInstanceID;
                        RADWorkflowInstanceId.Rows.Add(row);
                    }
                }
                if(cancelledByInitiator.Count>0)
                {
                    foreach(WorkflowActionInfo inf in cancelledByInitiator)
                    {
                        var row = RADWorkflowInstanceId.NewRow();
                        row["rad_workflow_instance_id"] = inf.RadInstanceID;
                        RADWorkflowInstanceId.Rows.Add(row);
                    }
                }
                CommonDALWrapper.ExecuteQuery(string.Format(@"CREATE TABLE {0}(rad_workflow_instance_id int)", RADWorkflowInstanceIdTableName), CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
                CommonDALWrapper.ExecuteBulkUpload(RADWorkflowInstanceIdTableName, RADWorkflowInstanceId, ConnectionConstants.RefMaster_Connection);

                DataTable dt = CommonDALWrapper.ExecuteSelectQuery(DALWrapperAppend.Replace(string.Format(@"select wr.rad_workflow_id,rwi.rad_workflow_instance_id from {0} rwi     inner join IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq on rq.rad_workflow_instance_id = rwi.rad_workflow_instance_id
                    inner join IVPRefMaster.dbo.ivp_srm_workflow_rules wr on wr.rule_mapping_id = rq.rule_mapping_id where rq.is_active=1", RADWorkflowInstanceIdTableName)), ConnectionConstants.RefMaster_Connection).Tables[0];

                HashSet<int> workflowIdSet = new HashSet<int>();
                Dictionary<int, int> RADWorkflowInstanceIdVsWorkflowId = new Dictionary<int, int>();
                foreach (DataRow row in dt.Rows)
                {
                    int workflowId = Convert.ToInt32(row["rad_workflow_id"]);
                    int radWorkflowInstanceId = Convert.ToInt32(row["rad_workflow_instance_id"]);
                    workflowIdSet.Add(workflowId);

                    if (!RADWorkflowInstanceIdVsWorkflowId.ContainsKey(radWorkflowInstanceId))
                        RADWorkflowInstanceIdVsWorkflowId.Add(radWorkflowInstanceId, workflowId);

                }

                Action<List<TriggerInfo>> testCallBack = info =>
                {
                    new SRMRADWorkflow().MassageUserNamesTriggerInfo(info);
                    throw new SRMWorkFlowCheckException("Checking action output before taking action.");
                };

                Action<List<TriggerInfo>> callBack = info =>
                {
                };

                Dictionary<int, Dictionary<string, HashSet<string>>> workflowIdVsStateNameVsListOfMappedGroups = new Dictionary<int, Dictionary<string, HashSet<string>>>();
                workflowIdVsStateNameVsListOfMappedGroups = GetListOfGroupsMapped(workflowIdSet);

                Dictionary<int, KeyValuePair<int, string>> RADWorkflowInstanceIdVsWorkflowIdVsCurrentState = new Dictionary<int, KeyValuePair<int, string>>();
                Dictionary<int, KeyValuePair<int, string>> RADWorkflowInstanceIdVsWorkflowIdVsCurrentStateForCancel = new Dictionary<int, KeyValuePair<int, string>>();

                Action<List<TriggerInfo>> passCallBack = info =>
                {
                    try
                    {
                        actionInfoForCreateEmailStructure = (from first in actionInfo join second in info on first.RadInstanceID equals second.InstanceId select first).ToList();

                        if (actionInfoForCreateEmailStructure.Count > 0)
                        {

                            var workflowInstanceIdVsRadInstanceIds = new Dictionary<int, List<TriggerInfo>>();
                            var tempCol = info.ToDictionary(x => x.InstanceId);
                            foreach (var inf in actionInfoForCreateEmailStructure)
                            {
                                if (!workflowInstanceIdVsRadInstanceIds.ContainsKey(inf.WorkflowInstanceID))
                                    workflowInstanceIdVsRadInstanceIds[inf.WorkflowInstanceID] = new List<TriggerInfo>();
                                if (tempCol.ContainsKey(inf.RadInstanceID))
                                    workflowInstanceIdVsRadInstanceIds[inf.WorkflowInstanceID].Add(tempCol[inf.RadInstanceID]);
                            }

                            foreach (var id in info)
                            {
                                if (!RADWorkflowInstanceIdVsWorkflowIdVsCurrentState.ContainsKey(id.InstanceId))
                                    RADWorkflowInstanceIdVsWorkflowIdVsCurrentState.Add(id.InstanceId, new KeyValuePair<int, string>(RADWorkflowInstanceIdVsWorkflowId[id.InstanceId], id.currentState));
                            }

                        }
                    }
                    catch (SRMWorkFlowCheckException er)
                    {
                        mLogger.Error(er.ToString());
                    }
                    catch (Exception er)
                    {
                        mLogger.Error(er.ToString());
                        throw new Exception(er.Message, er);
                    }
                };

                actionInfo.ForEach(action =>
                {
                    tInfo = new TriggerInfo();
                    tInfo.action = actionName;
                    tInfo.user = userName;
                    tInfo.comments = remarks;
                    tInfo.InstanceId = action.RadInstanceID;
                    lstTriggerInfo.Add(tInfo);
                });

                try
                {
                    actionCompleted = new SRMRADWorkflow().MoveWorkflow(lstTriggerInfo, testCallBack);
                }
                catch (SRMWorkFlowCheckException ex)
                {
                    try
                    {
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        serializer.MaxJsonLength = Int32.MaxValue;
                        mLogger.Debug("actionInfo -> Output From Rad in catch: " + serializer.Serialize(lstTriggerInfo));
                    }
                    catch (Exception exx) { }

                    mLogger.Error(ex.ToString());
                }
                catch (Exception ex)
                {
                    throw;
                }


                var setState = from rad in lstTriggerInfo.AsEnumerable()
                               join act in actionInfo.AsEnumerable()
                               on rad.InstanceId equals act.RadInstanceID

                               select SetRadInstanceData(act, rad.previousState, rad.currentState, rad.ErrorMessage);

                setState.Count();

                foreach (var act in actionInfo.Where(a => string.IsNullOrEmpty(a.ErrorMessage) && string.IsNullOrEmpty(a.NextState)))
                {
                    act.isPassed = false;
                    act.ErrorMessage = "Current State provided by RAD after moving workflow is null or empty";
                }

                if (actionName.ToLower() == cancelAction)
                {
                    cancelledToEndStage = actionInfo.Where(a => string.IsNullOrEmpty(a.ErrorMessage) && a.NextState.ToLower() == endState && (a.WorkflowType == WorkflowType.CREATE || a.WorkflowType == WorkflowType.UPDATE)).ToList();

                }
                else
                {
                    try
                    {
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        serializer.MaxJsonLength = Int32.MaxValue;
                        mLogger.Debug("actionInfo -> Output From Rad after modifying: " + serializer.Serialize(actionInfo));
                    }
                    catch (Exception ex) { }

                    finalApprovalCU = actionInfo.Where(a => string.IsNullOrEmpty(a.ErrorMessage) && a.NextState.ToLower() == endState && (a.WorkflowType == WorkflowType.CREATE || a.WorkflowType == WorkflowType.UPDATE)).ToList();
                }
                //added
                foreach (var action in actionInfo)
                {
                    action.StateNameAndActionName = action.CurrentState + " - " + actionName;
                }

                if (cancelledToEndStage != null && cancelledToEndStage.Count > 0)
                {
                    mLogger.Debug("Cancelled to end stage");

                    CreateEmailStructure(cancelledToEndStage, userName, moduleID, false, false, remarks, null, actionName, conMgr);

                    cancelledToEndStage.Select(c => c.TypeID).Distinct().ToList().ForEach(can =>
                    {
                        cancelledInstrumentsPerType.Add(can, cancelledToEndStage.Where(c => c.TypeID == can).Select(cc => cc.InstrumentID).Distinct().ToList());

                        InactivateRequestQueue(cancelledToEndStage.Select(f => f.RadInstanceID).Distinct().ToList(), conMgr);

                        if (moduleID == 3) //Sec Master
                        {
                            Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
                            Type objType = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMWorkFlowController");
                            MethodInfo SMCancelWorkflowAction = objType.GetMethod("SMCancelWorkflowAction");

                            object obj = Activator.CreateInstance(objType);
                            SMCancelWorkflowAction.Invoke(obj, new object[] { cancelledInstrumentsPerType, userName });

                        }
                        else //Others
                        {
                            Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                            Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMWorkFlowControllerNew");
                            MethodInfo CancelRequest = objType.GetMethod("RemoveEntitiesFromWorkflow");

                            object obj = Activator.CreateInstance(objType);
                            CancelRequest.Invoke(obj, new object[] { cancelledInstrumentsPerType, moduleID, userName, timestamp, null });
                        }
                    });

                }

                if (finalApprovalCU != null && finalApprovalCU.Count > 0)
                {
                    mLogger.Debug("Final Approval Exists");

                    if (performFinalApproval) //Final Approval
                    {
                        if (moduleID == 3) //SecMaster
                        {
                            finalApprovalCU = SMApprovalCreateUpdate(finalApprovalCU, userName, actionName, remarks, true, conMgr);
                        }
                        else //Ref-Party-Fund
                        {
                            finalApprovalCU = RPFMApproval(finalApprovalCU, userName, actionName, remarks, isUpdate, moduleID, conMgr, true);
                        }

                        finalInstances = finalApprovalCU.Select(f => f.RadInstanceID).Distinct().ToList();

                        if (finalInstances != null)
                        {
                            finalActions = actionInfoForRAD.Where(ar => finalInstances.Contains(ar.RadInstanceID)).ToList();
                            finalActions.ForEach(fac =>
                            {
                                actionInfoForRAD.Remove(fac);
                            });
                        }
                    }

                    else
                    {
                        InactivateRequestQueue(finalApprovalCU.Select(f => f.RadInstanceID).Distinct().ToList(), conMgr);
                    }
                }

                //Start Vikas Bhat, 28-June-2018, Configurable rules and validations execution on intermediate stage move
                if (performFinalApproval && (actionName.ToLower() == approveAction || actionName.ToLower() == editAndApproveAction))
                {
                    List<WFStageRuleConfigurationInfo> ruleConfigs = SRMWorkflowDBHandler.GetRuleConfigInfoByQueueID(actionInfo.Select(a => a.QueueID).Distinct().ToList(), conMgr);

                    //Code to fetch rule config info per workflow instance.

                    if (ruleConfigs != null && ruleConfigs.Any(r => r.IsAnythingConfigured))
                    {
                        mLogger.Debug("Intermediate stage rules configured.");

                        intermediateApprovals = (from act in actionInfo.Where(a => !finalInstances.Contains(a.RadInstanceID)
                                                 && string.IsNullOrEmpty(a.ErrorMessage)
                                                 && (a.WorkflowType == WorkflowType.CREATE || a.WorkflowType == WorkflowType.UPDATE))
                                                 join rc in ruleConfigs
                                                 on act.QueueID equals rc.QueueID
                                                 where act.CurrentState.ToLower().Trim() == rc.CurrentStage.ToLower().Trim()
                                                 && rc.IsAnythingConfigured
                                                 select act).Distinct().ToList<WorkflowActionInfo>();

                        intermediateApprovals.Count();

                        if (intermediateApprovals != null && intermediateApprovals.Count > 0) //Check if there are any requests for intermediate approval.
                        {
                            mLogger.Debug("Intermediate Approval Count: " + intermediateApprovals.Count.ToString());

                            if (ruleConfigs != null)
                            {
                                var setRuleConfig = from app in intermediateApprovals
                                                    join rc in ruleConfigs
                                                    on app.QueueID equals rc.QueueID
                                                    where app.CurrentState.ToLower() == rc.CurrentStage.ToLower()
                                                    select SetRuleConfigurations(app, rc);

                                setRuleConfig.Count();

                            }

                            if (moduleID == 3) //SecMaster
                            {
                                intermediateApprovals = SMApprovalCreateUpdate(intermediateApprovals, userName, actionName, remarks, false, conMgr);
                            }
                            else //Ref-Party-Fund
                            {
                                intermediateApprovals = RPFMApproval(intermediateApprovals, userName, actionName, remarks, isUpdate, moduleID, conMgr, false);
                            }

                            List<int> intermediateInstances = intermediateApprovals.Select(f => f.RadInstanceID).Distinct().ToList();

                            if (intermediateInstances != null && intermediateInstances.Count > 0)
                            {
                                intermediateActions = actionInfoForRAD.Where(ar => intermediateInstances.Contains(ar.RadInstanceID)).ToList();
                                intermediateActions.ForEach(fac =>
                                {
                                    actionInfoForRAD.Remove(fac);
                                });
                            }

                            mLogger.Debug("Intermediate approval completed.");
                        }
                        else
                            mLogger.Debug("Intermediate Approval Count: 0");
                    }

                }
                //End Vikas Bhat, 28-June-2018


                if (cancelledByInitiator != null && cancelledByInitiator.Count > 0)
                {
                    mLogger.Debug("Cancelled By Initiator Instances: " + string.Join(",", cancelledByInitiator.Select(ci => ci.RadInstanceID).ToList()));


                    //Start cancel action

                    lstTriggerInfo = new List<TriggerInfo>();
                    //  List<int> instanceId = new List<int>();

                    cancelledByInitiator.ForEach(action =>
                    {
                        tInfo = new TriggerInfo();
                        tInfo.action = actionName;
                        tInfo.user = userName;
                        tInfo.nextState = "End";
                        tInfo.comments = remarks;
                        tInfo.InstanceId = action.RadInstanceID;
                        lstTriggerInfo.Add(tInfo);

                        instanceId.Add(action.RadInstanceID);

                        if (!RADWorkflowInstanceIdVsWorkflowIdVsCurrentStateForCancel.ContainsKey(action.RadInstanceID))
                            RADWorkflowInstanceIdVsWorkflowIdVsCurrentStateForCancel.Add(action.RadInstanceID, new KeyValuePair<int, string>(RADWorkflowInstanceIdVsWorkflowId[action.RadInstanceID], "End"));
                    });

                    Dictionary<int, string> RADWorkflowInstanceIdVsRADUsersForCancel = GetUsersDetailsFromRAD(RADWorkflowInstanceIdVsWorkflowIdVsCurrentStateForCancel, workflowIdVsStateNameVsListOfMappedGroups);

                    bool cancelled = new SRMRADWorkflow().CancelWorkflowRequest(lstTriggerInfo);

                    //Removing entries from staging tables
                    if (cancelled)
                    {
                        CreateEmailStructure(cancelledByInitiator, userName, moduleID, false, false, remarks, RADWorkflowInstanceIdVsRADUsersForCancel, creatorCancelsTheRequest, conMgr);

                        mLogger.Debug("cancelled and removing entry from staging table");
                        foreach (var aInfo in cancelledByInitiator)
                        {
                            if (instrumentVsworkflowinstanceInfo.ContainsKey(aInfo.InstrumentID))
                            {
                                var wInfo = instrumentVsworkflowinstanceInfo[aInfo.InstrumentID];

                                if (!dictEventConfiguration.ContainsKey(wInfo.typeId))
                                    dictEventConfiguration[wInfo.typeId] = SRMEventController.GetEventConfiguration(new SRMEventConfigurationInputInfo { instrumentTypeId = wInfo.typeId, moduleId = moduleID }).DictConfiguration;
                                SRMEventPreferenceInfo eventPreferenceInfo = dictEventConfiguration[wInfo.typeId][eventType];

                                if (eventPreferenceInfo.IsSecurityLevel || (eventPreferenceInfo.LstAttributeLevel != null && eventPreferenceInfo.LstAttributeLevel.Count > 0) || (eventPreferenceInfo.LstLegLevel != null && eventPreferenceInfo.LstLegLevel.Count > 0))
                                {
                                    var eventInfo = new SRMEventInfo
                                    {
                                        Action = eventType,
                                        ID = aInfo.InstrumentID,
                                        Key = aInfo.InstrumentID,
                                        User = userName,
                                        Module = SRMEventModule.Securities,
                                        Type = wInfo.typeName,
                                        IsCreate = wInfo.isCreate,
                                        TimeStamp = timestamp,
                                        Initiator = usersdic.ContainsKey(wInfo.Initiator) ? usersdic[wInfo.Initiator] : wInfo.Initiator
                                    };

                                    if (Enum.IsDefined(typeof(SRMEventModule), moduleID))
                                        eventInfo.Module = (SRMEventModule)moduleID;

                                    eventInfo.EffectiveStartDate = wInfo.EffectiveStartDate;
                                    eventInfo.EffectiveEndDate = wInfo.EffectiveEndDate;

                                    dictRadWorkflowInstanceIdVsEventsInfo[aInfo.RadInstanceID] = new KeyValuePair<int, SRMEventInfo>(wInfo.typeId, eventInfo);
                                }
                            }
                        }

                        cancelledInstrumentsPerType = new Dictionary<int, List<string>>();

                        cancelledByInitiator.Select(c => c.TypeID).Distinct().ToList().ForEach(can =>
                        {
                            cancelledInstrumentsPerType.Add(can, cancelledByInitiator.Where(c => c.TypeID == can).Select(cc => cc.InstrumentID).Distinct().ToList());

                            InactivateRequestQueue(cancelledByInitiator.Select(f => f.RadInstanceID).Distinct().ToList(), conMgr);

                            if (moduleID == 3) //Sec Master
                            {
                                Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
                                Type objType = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMWorkFlowController");
                                MethodInfo SMCancelWorkflowAction = objType.GetMethod("SMCancelWorkflowAction");

                                object obj = Activator.CreateInstance(objType);
                                SMCancelWorkflowAction.Invoke(obj, new object[] { cancelledInstrumentsPerType, userName });

                            }
                            else //Others
                            {
                                Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                                Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMWorkFlowControllerNew");
                                MethodInfo CancelRequest = objType.GetMethod("RemoveEntitiesFromWorkflow");

                                object obj = Activator.CreateInstance(objType);
                                CancelRequest.Invoke(obj, new object[] { cancelledInstrumentsPerType, moduleID, userName, timestamp, null });
                            }
                        });
                    }

                    //End cancel action

                }


                //Finally Moving Workflow to Rad
                lstTriggerInfo = new List<TriggerInfo>();

                actionInfoForRAD.ForEach(action =>
                {
                    tInfo = new TriggerInfo();
                    tInfo.action = actionName;
                    tInfo.user = userName;
                    tInfo.comments = remarks;
                    tInfo.InstanceId = action.RadInstanceID;
                    lstTriggerInfo.Add(tInfo);
                });

                mLogger.Debug("just above MoveWorkflow");
                actionCompleted = new SRMRADWorkflow().MoveWorkflow(lstTriggerInfo, passCallBack);

                var setPassed = from aOut in actionOutput.AsEnumerable().Where(a => a.isPassed && a.Exceptions.Count == 0)
                                join aIn in actionInfo.AsEnumerable().Where(aa => !string.IsNullOrEmpty(aa.TypeName))
                                on aOut.RadInstanceID equals aIn.RadInstanceID
                                select SetExceptionForPassed(aOut, aIn.TypeName);

                setPassed.Count();

                var response = new WorkflowTakeActionResponse { actionOutput = actionOutput };

                if (actionCompleted)
                {
                    //added.
                    if (actionInfoForCreateEmailStructure.Count > 0 && cancelledToEndStage.Count == 0)
                    {
                        Dictionary<int, string> RADWorkflowInstanceIdVsRADUsers = GetUsersDetailsFromRAD(RADWorkflowInstanceIdVsWorkflowIdVsCurrentState, workflowIdVsStateNameVsListOfMappedGroups);
                        CreateEmailStructure(actionInfoForCreateEmailStructure, userName, moduleID, false, false, remarks, RADWorkflowInstanceIdVsRADUsers, actionName, conMgr);
                    }
                    foreach (var aInfo in actionInfoForRAD)
                    {
                        if (instrumentVsworkflowinstanceInfo.ContainsKey(aInfo.InstrumentID))
                        {
                            var wInfo = instrumentVsworkflowinstanceInfo[aInfo.InstrumentID];

                            if (!dictEventConfiguration.ContainsKey(wInfo.typeId))
                                dictEventConfiguration[wInfo.typeId] = SRMEventController.GetEventConfiguration(new SRMEventConfigurationInputInfo { instrumentTypeId = wInfo.typeId, moduleId = moduleID }).DictConfiguration;
                            SRMEventPreferenceInfo eventPreferenceInfo = dictEventConfiguration[wInfo.typeId][eventType];

                            if (actionName.Equals(approveAction, StringComparison.OrdinalIgnoreCase) || actionName.Equals(editAndApproveAction, StringComparison.OrdinalIgnoreCase) || eventPreferenceInfo.IsSecurityLevel || (eventPreferenceInfo.LstAttributeLevel != null && eventPreferenceInfo.LstAttributeLevel.Count > 0) || (eventPreferenceInfo.LstLegLevel != null && eventPreferenceInfo.LstLegLevel.Count > 0))
                            {
                                var eventInfo = new SRMEventInfo
                                {
                                    Action = eventType,
                                    ID = aInfo.InstrumentID,
                                    Key = aInfo.InstrumentID,
                                    User = userName,
                                    Module = SRMEventModule.Securities,
                                    Type = wInfo.typeName,
                                    IsCreate = wInfo.isCreate,
                                    TimeStamp = timestamp,
                                    Initiator = usersdic.ContainsKey(wInfo.Initiator) ? usersdic[wInfo.Initiator] : wInfo.Initiator
                                };

                                if (Enum.IsDefined(typeof(SRMEventModule), moduleID))
                                    eventInfo.Module = (SRMEventModule)moduleID;

                                eventInfo.EffectiveStartDate = wInfo.EffectiveStartDate;
                                eventInfo.EffectiveEndDate = wInfo.EffectiveEndDate;

                                dictRadWorkflowInstanceIdVsEventsInfo[aInfo.RadInstanceID] = new KeyValuePair<int, SRMEventInfo>(wInfo.typeId, eventInfo);
                            }
                        }
                    }
                }

                if (!actionName.Equals(cancelAction, StringComparison.OrdinalIgnoreCase) && dictRadWorkflowInstanceIdVsEventsInfo != null && dictRadWorkflowInstanceIdVsEventsInfo.Count > 0)
                {
                    if (dictRadWorkflowInstanceIdVsEventsInfo != null && dictRadWorkflowInstanceIdVsEventsInfo.Count > 0)
                    {
                        var workflowPendingInfo = new SRMRADWorkflow().GetPendingInfoForInstances(dictRadWorkflowInstanceIdVsEventsInfo.Keys.ToList(), string.Empty);

                        foreach (var workflowLevel in workflowPendingInfo)
                        {
                            if (dictRadWorkflowInstanceIdVsEventsInfo.ContainsKey(workflowLevel.InstanceID))
                                dictRadWorkflowInstanceIdVsEventsInfo[workflowLevel.InstanceID].Value.PendingAt = workflowLevel.GroupsMapped;
                        }
                    }
                }

                if (conMgr == null)
                {
                    foreach (var typeLevel in dictRadWorkflowInstanceIdVsEventsInfo.Values.GroupBy(x => x.Key))
                    {
                        SRMEventController.RaiseEvent(new SRMRaiseEventsInputInfo { instrumentTypeId = typeLevel.Key, eventInfo = typeLevel.Select(x => x.Value).ToList(), moduleId = moduleID });
                    }
                }
                else
                    response.objSRMEventsInfo = dictRadWorkflowInstanceIdVsEventsInfo.Values.GroupBy(x => x.Key).ToDictionary(x => x.Key, y => y.Select(a => a.Value).ToList());

                return response;
            }
            catch (Exception ex)
            {
                mLogger.Error("TakeAction -> Error: " + ex.ToString());
                actionOutput.ForEach(a =>
                {
                    a.isPassed = false;
                    a.ErrorMessage = ex.Message;
                });
                return new WorkflowTakeActionResponse { actionOutput = actionOutput };
            }
            finally
            {
                
                mLogger.Error("TakeAction -> End");
                string query = @"IF (OBJECT_ID('" + RADWorkflowInstanceIdTableName + "') IS NOT NULL) DROP TABLE " + RADWorkflowInstanceIdTableName;
                if (conMgr == null)
                    CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Delete, ConnectionConstants.RefMaster_Connection);
                else
                    CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Delete, conMgr);
            }
        }


        private static bool SetRuleConfigurationsForInitialization(SRMWorkflowInstanceInfo instanceInfo, WFStageRuleConfigurationInfo ruleConfigInfo)
        {
            instanceInfo.RuleConfigurations = ruleConfigInfo;

            return true;
        }


        private static bool SetRuleConfigurations(WorkflowActionInfo actionInfo, WFStageRuleConfigurationInfo ruleConfigInfo)
        {
            actionInfo.RuleConfigurations.ExecuteAlertRules = ruleConfigInfo.ExecuteAlertRules;
            actionInfo.RuleConfigurations.ExecuteBasketAlertRules = ruleConfigInfo.ExecuteBasketAlertRules;
            actionInfo.RuleConfigurations.ExecuteGroupValidationRules = ruleConfigInfo.ExecuteGroupValidationRules;
            actionInfo.RuleConfigurations.ExecuteMandatoryCheck = ruleConfigInfo.ExecuteMandatoryCheck;
            actionInfo.RuleConfigurations.ExecuteMnemonicsRules = ruleConfigInfo.ExecuteMnemonicsRules;
            actionInfo.RuleConfigurations.ExecutePrimaryCheck = ruleConfigInfo.ExecutePrimaryCheck;
            actionInfo.RuleConfigurations.ExecuteRestrictedChars = ruleConfigInfo.ExecuteRestrictedChars;
            actionInfo.RuleConfigurations.ExecuteTransformationRules = ruleConfigInfo.ExecuteTransformationRules;
            actionInfo.RuleConfigurations.ExecuteUniquenessCheck = ruleConfigInfo.ExecuteUniquenessCheck;
            actionInfo.RuleConfigurations.ExecuteValidationRules = ruleConfigInfo.ExecuteValidationRules;
            actionInfo.RuleConfigurations.IsAnythingConfigured = ruleConfigInfo.IsAnythingConfigured;

            return true;
        }

        private static bool SetExceptionForPassed(WorkflowActionInfo actionInfo, string typeName)
        {
            //actionInfo.Exceptions = new List<WFApprovalExceptionInfo>();
            actionInfo.Exceptions.Add(new WFApprovalExceptionInfo() { AttributeName = string.Empty, isAlertOnly = false, Exception = string.Empty, TypeName = typeName });
            return true;
        }


        private static void InactivateRequestQueue(List<int> instanceIDs, RDBConnectionManager mDBConMgr = null)
        {
            SRMWorkflowDBHandler.InactiveRequestQueue(instanceIDs, mDBConMgr);
        }

        /// <summary>
        /// Final Approval for Security Date
        /// </summary>
        /// <param name="actionInfo"></param>
        /// <param name="conMgr"></param>
        /// <returns></returns>
        private static List<WorkflowActionInfo> SMApprovalCreateUpdate(List<WorkflowActionInfo> actionInfo, string userName, string actionName, string remarks, bool isFinalApproval, RDBConnectionManager conMgr = null)
        {
            Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterDashboardService");
            Type objType = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMWorkflowSecurityUpdator");
            MethodInfo SaveSecurities = objType.GetMethod("SaveSecurities");

            object obj = Activator.CreateInstance(objType);
            return (List<WorkflowActionInfo>)SaveSecurities.Invoke(obj, new object[] { actionInfo, userName, actionName, remarks, isFinalApproval });
        }

        /// <summary>
        /// Final Approval for ref/party/fund
        /// </summary>
        /// <param name="actionInfo"></param>
        /// <param name="conMgr"></param>
        /// <returns></returns>
        private static List<WorkflowActionInfo> RPFMApproval(List<WorkflowActionInfo> actionInfo, string userName, string actionName, string remarks, bool isUpdate, int moduleID, RDBConnectionManager conMgr = null, bool isFinalApproval = true)
        {
            try
            {
                mLogger.Debug("RPFMApproval -> Start");

                mLogger.Debug("Is Final Approval: " + isFinalApproval.ToString());

                Dictionary<string, DateTime> entityCodeVsEffectiveDate = new Dictionary<string, DateTime>();
                Dictionary<string, string> entityCodeVsRequester = new Dictionary<string, string>();
                Dictionary<string, int> instrumentVsInstanceID = new Dictionary<string, int>();
                DateTime approvalDate = DateTime.Now;

                actionInfo.ForEach(a =>
                {
                    entityCodeVsEffectiveDate.Add(a.InstrumentID, a.EffectiveStartDate ?? DateTime.Now);
                    entityCodeVsRequester.Add(a.InstrumentID, a.RequestedBy);
                    instrumentVsInstanceID.Add(a.InstrumentID, a.RadInstanceID);
                });

                Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMWorkFlowControllerNew");
                MethodInfo FinalApproval = objType.GetMethod("SaveApprovedEntities");

                object obj = Activator.CreateInstance(objType);
                List<WFInstrumentApprovalInfo> approvalOutput = (List<WFInstrumentApprovalInfo>)FinalApproval.Invoke(obj, new object[] { actionInfo, entityCodeVsEffectiveDate, instrumentVsInstanceID, approvalDate, userName, remarks, actionName, isUpdate, moduleID, false, entityCodeVsRequester, isFinalApproval });

                if (approvalOutput != null)
                {
                    var setFAOutput = from app in approvalOutput.AsEnumerable()
                                      join act in actionInfo.AsEnumerable()
                                      on app.InstrumentID equals act.InstrumentID
                                      select SetFinalApprovalOutput(act, app.isPassed, app.Exceptions);

                    setFAOutput.Count();
                }

                return actionInfo;
            }
            catch (Exception ex)
            {
                mLogger.Debug("RPFMApproval -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RPFMApproval -> End");
            }
        }

        private static bool SetFinalApprovalOutput(WorkflowActionInfo info, bool isPassed, List<WFApprovalExceptionInfo> Exceptions)
        {
            info.isPassed = isPassed;
            info.Exceptions = Exceptions;
            return true;
        }


        /// <summary>
        /// Method to reconcile the output of a final approval action(validations raised, error etc.)
        /// </summary>
        /// <param name="actionInfo"></param>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        private static DataTable ReconcileApprovalValidations(List<WorkflowActionInfo> actionInfo, int moduleID)
        {
            DataTable dtExceptions = new DataTable();
            ModuleWiseInfo = SRMWorkflowUtils.PopulateModuleWiseInfo(ModuleWiseInfo);
            string instrumentColumnName = ModuleWiseInfo.Where(m => m.ModuleID == moduleID).Select(mm => mm.InstrumentColumnName).FirstOrDefault();

            dtExceptions.Columns.Add(new DataColumn() { ColumnName = instrumentColumnName, DataType = typeof(string) });
            dtExceptions.Columns.Add(new DataColumn() { ColumnName = "Attribute", DataType = typeof(string) });
            dtExceptions.Columns.Add(new DataColumn() { ColumnName = "Message", DataType = typeof(string) });
            DataRow row = null;

            actionInfo.ForEach(act =>
            {
                act.Exceptions.ForEach(ex =>
                {
                    row = dtExceptions.NewRow();
                    row[instrumentColumnName] = act.InstrumentID;
                    row["Attribute"] = ex.AttributeName;
                    row["Message"] = ex.Exception;

                    dtExceptions.Rows.Add(row);
                });

            });

            return dtExceptions;
        }

        /// <summary>
        /// Method to get all the actions that a user can do on an instance
        /// </summary>
        /// <param name="instances"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static Dictionary<int, List<string>> GetPossibleActions(List<int> instances, string userName)
        {
            Dictionary<int, List<string>> dict = new Dictionary<int, List<string>>();

            new SRMRADWorkflow().GetActionOfInstanceForUser(instances, userName).ForEach(a =>
            {
                dict.Add(a.InstanceID, a.Actions);
            });

            return dict;
        }

        private static bool SetState(WorkflowActionInfo actionInfo, string state)
        {
            actionInfo.NextState = state;
            return true;
        }

        private static bool SetRadInstanceData(WorkflowActionInfo actionInfo, string currentState, string nextState, string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                actionInfo.isPassed = false;
                actionInfo.ErrorMessage = errorMessage;
            }
            else
            {
                actionInfo.CurrentState = currentState;
                actionInfo.NextState = nextState;
            }
            return true;
        }

        public static SRMWorkflowResponse GetRADWorkflowForInstruments(int typeId, int moduleId, bool isUpdateCase, string instrumentIdColumnName, string userName, DataTable dt, bool RaiseRequest = true, RDBConnectionManager connManager = null, Dictionary<SRMEventActionType, SRMEventPreferenceInfo> dictEventConfiguration = null, DateTime? timestamp = null, Dictionary<string, HashSet<string>> updatedAttributes = null)
        {
            SRMWorkflowResponse result = new SRMWorkflowResponse();
            mLogger.Debug("GetRADWorkflowForInstruments ==> Start");

            string ATTRIBUTES_UPDATED = "Attributes Updated";
            result.message = string.Empty;
            try
            {
                bool raiseWorkflow = false;

                DataTable dtRules = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT wrul.*
                FROM IVPRefMaster.dbo.ivp_srm_workflow_instance inst
                INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_action_type atyp
                ON inst.workflow_action_type_id = atyp.workflow_action_type_id AND atyp.workflow_action_type_id IN ({0}) AND atyp.module_id = {1} AND inst.is_active = 1
                INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_mapping wmap
                ON inst.workflow_instance_id = wmap.workflow_instance_id AND wmap.type_id = {2} AND wmap.is_active = 1
                INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_rules wrul
                ON inst.workflow_instance_id = wrul.workflow_instance_id", isUpdateCase ? "2,7,12,17" : "1,6,11,16", moduleId, typeId), ConnectionConstants.RefMaster_Connection).Tables[0];

                if (dtRules == null || dtRules.Rows.Count == 0)
                    result.isWorkflowConfigured = false;
                else
                {
                    result.isWorkflowConfigured = true;

                    if (dtRules.AsEnumerable().Select(x => Convert.ToString(x["workflow_instance_id"])).Distinct().Count() > 1)
                        throw new Exception("Multiple workflows configured");


                    result.WorkflowInstanceId = Convert.ToInt32(dtRules.Rows[0]["workflow_instance_id"]);

                    List<string> attributesInRule = dtRules.AsEnumerable().Select(x => x["attributes_in_rule"] != DBNull.Value ? Convert.ToString(x["attributes_in_rule"]).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>()).SelectMany(x => x).Distinct().ToList();
                    attributesInRule.Add(instrumentIdColumnName);

                    if (dt != null && dt.Rows.Count > 0 && !dt.Columns.Contains(ATTRIBUTES_UPDATED) && moduleId != 3)
                    {
                        attributesInRule.Add(ATTRIBUTES_UPDATED);
                        Dictionary<string, string> updatedAttributesConcatenated = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        if (updatedAttributes != null && updatedAttributes.Count > 0)
                        {
                            updatedAttributesConcatenated = updatedAttributes.ToDictionary(x => x.Key, y => "#" + string.Join("#", y.Value) + "#", StringComparer.OrdinalIgnoreCase);
                            mLogger.Debug("updatedAttributesConcatenated - " + string.Join(",", updatedAttributesConcatenated.Select(x => x.Key + " - [" + x.Value + "]")));
                        }
                        else
                            mLogger.Debug("updatedAttributes is empty");

                        dt.Columns.Add(ATTRIBUTES_UPDATED, typeof(string));
                        foreach (DataRow dr in dt.Rows)
                        {
                            string instrumentId = Convert.ToString(dr[instrumentIdColumnName]);
                            if (updatedAttributesConcatenated.ContainsKey(instrumentId))
                                dr[ATTRIBUTES_UPDATED] = updatedAttributesConcatenated[instrumentId];
                            else
                                dr[ATTRIBUTES_UPDATED] = "#";
                        }
                    }

                    DataTable dtData = dt.Copy();

                    List<string> lstColumnsToRemove = new List<string>();
                    foreach (DataColumn dc in dtData.Columns)
                    {
                        if (!attributesInRule.Contains(dc.ColumnName))
                            lstColumnsToRemove.Add(dc.ColumnName);
                    }
                    if (lstColumnsToRemove.Count > 0)
                    {
                        foreach (var column in lstColumnsToRemove)
                        {
                            dtData.Columns.Remove(column);
                        }
                    }

                    if (!dtData.Columns.Contains("is_valid"))
                        dtData.Columns.Add("is_valid", typeof(string));
                    if (!dtData.Columns.Contains("rule_name"))
                        dtData.Columns.Add("rule_name", typeof(string));

                    DataRow drDefaultRule = dtRules.AsEnumerable().Where(x => Convert.ToInt32(x["priority"]) == -1).FirstOrDefault();
                    if (dtRules.Rows.Count > 1 && drDefaultRule != null)
                    {
                        int maxPriority = dtRules.AsEnumerable().Max(x => Convert.ToInt32(x["priority"]));

                        drDefaultRule["priority"] = maxPriority + 1;
                    }

                    #region MASSAGE LOOKUP DATA

                    DataTable refAttributes = new DataTable();
                    Dictionary<string, DataRow> lookupAttrVsDataRow = new Dictionary<string, DataRow>();
                    Dictionary<string, DataRow> seclookupAttrVsDataRow = new Dictionary<string, DataRow>();

                    if (moduleId != 3)
                    {
                        refAttributes = RMCommonUtils.GetEntityTypeLookupDetails(typeId);
                        var seclookupDetails = RMCommonUtils.GetAllAttributeByEntityTypeId(typeId);

                        if (seclookupDetails != null && seclookupDetails.Tables.Count > 0 && seclookupDetails.Tables[0].Rows.Count > 0)
                        {
                            seclookupAttrVsDataRow = seclookupDetails.Tables[0].AsEnumerable().Where(x => Convert.ToString(x["Data Type"]) == "SECURITY_LOOKUP").ToDictionary(x => Convert.ToString(x["attribute_name"]), y => y);
                        }
                    }
                    else
                    {
                        refAttributes = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT adet.attribute_name AS childAttributeRealName, CAST(0 AS BIT) AS isSecurityLookup, refmap.reference_attribute_name AS parentAttribute, etyp.entity_type_name AS parentEntityType
                        FROM IVPSecMaster.dbo.ivp_secm_sectype_master tmas
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_template_master temas
                        ON tmas.sectype_id = temas.sectype_id AND tmas.is_active = 1 AND temas.is_active = 1 AND temas.dependent_id = 'SYSTEM' AND tmas.sectype_id = {0}
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details tdet
                        ON temas.template_id = tdet.template_id AND tdet.is_active = 1
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_details adet
                        ON adet.attribute_id = tdet.attribute_id AND adet.is_active = 1
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_table ttab
                        ON adet.sectype_table_id = ttab.sectype_table_id AND (ttab.priority = 1 OR ttab.sectype_id = 0)
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_reference_attribute_mapping refmap
                        ON adet.attribute_id = refmap.attribute_id
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                        ON refmap.reference_type_id = etyp.entity_type_id AND etyp.is_active = 1", typeId), ConnectionConstants.SecMaster_Connection).Tables[0];
                    }

                    if (refAttributes != null && refAttributes.Rows.Count > 0)
                    {
                        lookupAttrVsDataRow = refAttributes.AsEnumerable().ToDictionary(x => Convert.ToString(x["childAttributeRealName"]), y => y);
                    }

                    RMLookupDataMassageInfo objRMLookupDataMassageInfo = new RMLookupDataMassageInfo() { IsArchive = false, IsAuditView = false, InputType = RMLookupInputType.REALNAME, IsEntityCodeToValues = true };
                    List<RMLookupAttributeInfo> lstLookupAttributeInfo = new List<RMLookupAttributeInfo>();
                    List<RMLookupAttributeInfo> lstSecLookupAttributeInfo = new List<RMLookupAttributeInfo>();


                    foreach (var attr in attributesInRule)
                    {
                        if (lookupAttrVsDataRow.ContainsKey(attr))
                        {
                            DataRow dr = lookupAttrVsDataRow[attr];
                            bool isSecLookup = Convert.ToBoolean(dr["isSecurityLookup"]);

                            RMLookupAttributeInfo info = new RMLookupAttributeInfo();
                            info.AttributeRealName = attr;
                            info.MappedColumns = new List<string>() { attr };
                            info.ParentAttributeRealName = Convert.ToString(dr["parentAttribute"]);
                            info.ParentRealName = Convert.ToString(dr["parentEntityType"]);

                            if (!isSecLookup)
                                lstLookupAttributeInfo.Add(info);
                            else
                            {
                                info.IsSecurityLookup = true;
                                DataRow drr = seclookupAttrVsDataRow[attr];
                                info.ParentAttributeId = Convert.ToInt32(drr["lookup_attribute_id"]);
                                info.ParentId = Convert.ToInt32(Convert.ToInt32(drr["lookup_entity_type"]));
                                lstSecLookupAttributeInfo.Add(info);
                            }
                        }
                    }

                    if (lstLookupAttributeInfo.Count > 0)
                        new RMLookupDataMassage().MassageLookupData(dtData, objRMLookupDataMassageInfo, lstLookupAttributeInfo);

                    if (lstSecLookupAttributeInfo.Count > 0)
                        new RMLookupDataMassage().MassageSecurityLookup(dtData, new RMLookupDataMassageInfo() { IsArchive = false, IsAuditView = false, InputType = RMLookupInputType.ID, IsEntityCodeToValues = true }, lstSecLookupAttributeInfo);
                    #endregion

                    Dictionary<string, List<DataRow>> InstrumentIdVsRows = dtData.AsEnumerable().GroupBy(x => Convert.ToString(x[instrumentIdColumnName])).ToDictionary(x => x.Key, y => y.ToList());

                    foreach (DataRow drRule in dtRules.AsEnumerable().OrderBy(x => Convert.ToInt32(x["priority"])))
                    {
                        int ruleSetId = Convert.ToInt32(drRule["rule_set_id"]);
                        int ruleMappingId = Convert.ToInt32(drRule["rule_mapping_id"]);
                        int radWorkflowId = Convert.ToInt32(drRule["rad_workflow_id"]);

                        if (dtData.Rows.Count == 0 || InstrumentIdVsRows == null || InstrumentIdVsRows.Count == 0)
                            break;

                        List<string> lstFilteredInstruments = new List<string>();
                        if (ruleSetId > 0)
                        {
                            SRMWorkflowRuleExecutor objSRMWorkflowRuleExecutor = new SRMWorkflowRuleExecutor();
                            SRMRuleInfo objSRMRuleInfo = new SRMRuleInfo();

                            objSRMRuleInfo.InputInformation = dtData;
                            objSRMRuleInfo.RuleSetID = ruleSetId;
                            objSRMWorkflowRuleExecutor.ExecuteXRule(objSRMRuleInfo, "Workflow", typeId, moduleId);

                            lstFilteredInstruments.AddRange(objSRMRuleInfo.OutputInformation.AsEnumerable().Where(x => Convert.ToString(x["is_valid"]).Equals("Passed", StringComparison.OrdinalIgnoreCase)).Select(x => Convert.ToString(x[instrumentIdColumnName])).Distinct());
                        }
                        else
                            lstFilteredInstruments.AddRange(InstrumentIdVsRows.Keys);

                        if (lstFilteredInstruments.Count > 0)
                        {
                            foreach (string instrumentId in lstFilteredInstruments)
                            {
                                raiseWorkflow = true;

                                result.InstrumentIDVsRADWorkflowInfo[instrumentId] = new SRMWorkflowInstanceInfo { RadWorkflowId = radWorkflowId, RuleMappingId = ruleMappingId };
                                foreach (DataRow dr in InstrumentIdVsRows[instrumentId])
                                {
                                    try
                                    {
                                        dtData.Rows.Remove(dr);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                InstrumentIdVsRows.Remove(instrumentId);
                            }
                        }
                    }

                    if (result.InstrumentIDVsRADWorkflowInfo != null && result.InstrumentIDVsRADWorkflowInfo.Count > 0)
                    {
                        List<WFStageRuleConfigurationInfo> ruleConfigs = SRMWorkflowDBHandler.GetRuleConfigInfoForInitiation(result.InstrumentIDVsRADWorkflowInfo.Select(a => a.Value.RuleMappingId).Distinct().ToList(), connManager);

                        if (ruleConfigs != null)
                        {
                            var setRuleConfig = from app in result.InstrumentIDVsRADWorkflowInfo.Values
                                                join rc in ruleConfigs
                                                on app.RuleMappingId equals rc.RuleMappingID
                                                where startState.ToLower() == rc.CurrentStage.ToLower()
                                                select SetRuleConfigurationsForInitialization(app, rc);

                            setRuleConfig.Count();

                        }
                    }

                    foreach (string instrumentId in InstrumentIdVsRows.Keys)
                    {
                        result.InstrumentIDVsRADWorkflowInfo[instrumentId] = new SRMWorkflowInstanceInfo { RadWorkflowId = -1 };
                    }

                    if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains(ATTRIBUTES_UPDATED) && attributesInRule.Contains(ATTRIBUTES_UPDATED))
                    {
                        dt.Columns.Remove(ATTRIBUTES_UPDATED);
                    }
                }
                result.isPassed = true;

                if (raiseWorkflow)
                {
                    List<WorkflowQueueInfo> queueInfoInternal = new List<WorkflowQueueInfo>();
                    foreach (var kvp in result.InstrumentIDVsRADWorkflowInfo)
                    {
                        var info = new WorkflowQueueInfo()
                        {
                            InstrumentID = kvp.Key,
                            RequestedBy = userName,
                            WorkflowInstanceID = result.WorkflowInstanceId,
                            TypeID = typeId
                        };
                        if (timestamp.HasValue)
                            info.RequestedOn = timestamp.Value;
                        queueInfoInternal.Add(info);
                    }

                    result.objSRMEventsInfo = InitiateWorkflow(ref result, queueInfoInternal, userName, RaiseRequest, connManager, dictEventConfiguration, timestamp);
                }
            }
            catch (Exception ex)
            {
                result.isPassed = false;
                result.message = ex.ToString();
                result.InstrumentIDVsRADWorkflowInfo = new Dictionary<string, SRMWorkflowInstanceInfo>();

                mLogger.Error(ex.ToString());
            }
            finally
            {
                mLogger.Debug("GetRADWorkflowForInstruments ==> End");
            }
            return result;
        }

        public static SRMWorkflowResponse GetRADWorkflowStageForInstruments(int typeId, int moduleId, bool isUpdateCase, string userName, List<string> lstInstruments, string actionName)
        {
            SRMWorkflowResponse result = new SRMWorkflowResponse();
            mLogger.Debug("GetRADWorkflowStageForInstruments ==> Start");

            result.message = string.Empty;
            try
            {
                DataTable dtInstances = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT inst.workflow_instance_id
                FROM IVPRefMaster.dbo.ivp_srm_workflow_instance inst
                INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_action_type atyp
                ON inst.workflow_action_type_id = atyp.workflow_action_type_id AND atyp.workflow_action_type_id IN ({0}) AND atyp.module_id = {1} AND inst.is_active = 1
                INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_mapping wmap
                ON inst.workflow_instance_id = wmap.workflow_instance_id AND wmap.type_id = {2} AND wmap.is_active = 1", isUpdateCase ? "2,7,12,17" : "1,6,11,16", moduleId, typeId), ConnectionConstants.RefMaster_Connection).Tables[0];

                if (dtInstances == null || dtInstances.Rows.Count == 0)
                    result.isWorkflowConfigured = false;
                else
                {
                    result.isWorkflowConfigured = true;

                    if (dtInstances.AsEnumerable().Select(x => Convert.ToString(x["workflow_instance_id"])).Distinct().Count() > 1)
                        throw new Exception("Multiple workflows configured");

                    result.WorkflowInstanceId = Convert.ToInt32(dtInstances.Rows[0]["workflow_instance_id"]);

                    result.InstrumentIDVsRADWorkflowInfo = lstInstruments.ToDictionary(x => x, y => new SRMWorkflowInstanceInfo { IsWorkflowRaised = false });

                    List<WorkflowActionInfo> actionInfo = CheckInstrumentsInWorkflow(lstInstruments, result.WorkflowInstanceId).Select(x => new WorkflowActionInfo { InstrumentID = x.Key, RadInstanceID = x.Value.RadInstanceID, QueueID = x.Value.QueueID, RuleMappingID = x.Value.RuleMappingID, isPassed = true }).ToList();

                    if (!string.IsNullOrEmpty(actionName))
                    {
                        if (actionName.Equals("startžretrigger", StringComparison.OrdinalIgnoreCase))
                            actionInfo.ForEach(x => x.CurrentState = "Start");
                        else
                            actionInfo = CheckActionOutput(actionInfo, actionName, userName, string.Empty, moduleId);
                    }
                    else
                    {
                        actionInfo = CheckActionOutput(actionInfo, approveAction, userName, string.Empty, moduleId);

                        var actionInfoRemaining = actionInfo.Where(x => string.IsNullOrEmpty(x.CurrentState));
                        if (actionInfoRemaining != null && actionInfoRemaining.Count() > 0)
                            CheckActionOutput(actionInfoRemaining.ToList(), editAndApproveAction, userName, string.Empty, moduleId);

                        actionInfo.ToList().ForEach(x => x.NextState = string.Empty);
                    }

                    List<WFStageRuleConfigurationInfo> ruleConfigs = SRMWorkflowDBHandler.GetRuleConfigInfoByQueueID(actionInfo.Select(a => a.QueueID).Distinct().ToList());
                    Dictionary<int, Dictionary<string, WFStageRuleConfigurationInfo>> dictQueueIdVsRuleConfigurations = new Dictionary<int, Dictionary<string, WFStageRuleConfigurationInfo>>();
                    if (ruleConfigs != null)
                    {
                        dictQueueIdVsRuleConfigurations = ruleConfigs.GroupBy(x => x.QueueID).ToDictionary(x => x.Key, y => y.ToDictionary(m => m.CurrentStage, n => n, StringComparer.OrdinalIgnoreCase));
                    }

                    foreach (var item in actionInfo)
                    {
                        if (!item.isPassed)
                            continue;

                        WFStageRuleConfigurationInfo objRuleConfigurations = new WFStageRuleConfigurationInfo();
                        if (dictQueueIdVsRuleConfigurations != null && dictQueueIdVsRuleConfigurations.Count > 0 && dictQueueIdVsRuleConfigurations.ContainsKey(item.QueueID) && dictQueueIdVsRuleConfigurations[item.QueueID].ContainsKey(item.CurrentState))
                            objRuleConfigurations = dictQueueIdVsRuleConfigurations[item.QueueID][item.CurrentState];

                        result.InstrumentIDVsRADWorkflowInfo[item.InstrumentID] = new SRMWorkflowInstanceInfo { RadWorkflowInstanceId = item.RadInstanceID, IsWorkflowRaised = false, IsFinalApproval = !string.IsNullOrEmpty(item.NextState) && item.NextState.Equals(endState, StringComparison.OrdinalIgnoreCase), State = item.NextState, RuleConfigurations = objRuleConfigurations, RuleMappingId = item.RuleMappingID };
                    }
                }
                result.isPassed = true;
            }
            catch (Exception ex)
            {
                result.isPassed = false;
                result.message = ex.ToString();
                result.InstrumentIDVsRADWorkflowInfo = new Dictionary<string, SRMWorkflowInstanceInfo>();

                mLogger.Error(ex.ToString());
            }
            finally
            {
                mLogger.Debug("GetRADWorkflowStageForInstruments ==> End");
            }
            return result;
        }


        public static List<SRMWorkflowCountPerType> GetWorkflowCountPerType(int moduleID, string userName)
        {
            List<SRMWorkflowCountPerType> lstWorkFlow = new List<SRMWorkflowCountPerType>();
            List<RWorkFlowInstanceInfo> lstInfo = new SRMRADWorkflow().GetActionOfInstanceForUser(null, userName);

            lstInfo.ForEach(wInfo =>
            {
                wInfo.ActionInfo.ForEach(ai =>
                {

                });
            });

            return lstWorkFlow;
        }

        public static List<WorkflowActionHistory> GetWorkflowActionHistory(int moduleId, int radWorkflowInstanceId)
        {
            List<WorkflowActionHistory> res = new List<WorkflowActionHistory>();
            WorkflowActionHistory item = new WorkflowActionHistory();
            item.ActionDate = DateTime.Now.ToShortDateString().ToString();
            item.ActionLevel = 1;
            item.ActionTime = DateTime.Now.TimeOfDay.ToString();
            item.UserName = "admin";
            item.Status = "Approved";
            item.Remarks = "42";

            AttributeAudit attritem = new AttributeAudit();
            attritem.AttributeName = "name";
            attritem.OldValue = "42";
            attritem.NewValue = "6942";
            item.Attributes.Add(attritem);
            attritem.AttributeName = "gevge";
            attritem.OldValue = "1142";
            attritem.NewValue = "622942";
            item.Attributes.Add(attritem);

            res.Add(item);

            WorkflowActionHistory item2 = new WorkflowActionHistory();

            item2.ActionDate = DateTime.Now.ToShortDateString().ToString();
            item2.ActionLevel = 2;
            item2.ActionTime = DateTime.Now.TimeOfDay.ToString();
            item2.UserName = "admin2";
            item2.Status = "Approved";
            item2.Remarks = "4222222";
            attritem.AttributeName = "ndadsame";
            attritem.OldValue = "1sdd142";
            attritem.NewValue = "62sdsd2942";
            item2.Attributes.Add(attritem);
            attritem.AttributeName = "gffdsfevge";
            attritem.OldValue = "aa";
            attritem.NewValue = "bbb";
            item2.Attributes.Add(attritem);

            res.Add(item2);
            res.Add(item2);
            res.Add(item2);
            res.Add(item2);
            res.Add(item2);
            res.Add(item2);
            res.Add(item2);
            res.Add(item2);
            res.Add(item2);
            res.Add(item2);
            res.Add(item2);
            res.Add(item2);


            WorkflowActionHistory item3 = new WorkflowActionHistory();

            item3.ActionDate = DateTime.Now.ToShortDateString().ToString();
            item3.ActionLevel = 3;
            item3.ActionTime = DateTime.Now.TimeOfDay.ToString();
            item3.UserName = "admfdssfin2";
            item3.Status = "Rejected";
            item3.Remarks = "42222gf22";

            res.Add(item3);


            WorkflowActionHistory item4 = new WorkflowActionHistory();

            item4.ActionDate = DateTime.Now.ToShortDateString().ToString();
            item4.ActionLevel = 2;
            item4.ActionTime = DateTime.Now.TimeOfDay.ToString();
            item4.UserName = "admin3";
            item4.Status = "Pending";
            item4.Remarks = "4222222";
            attritem.AttributeName = "ndadsame";
            attritem.OldValue = "1sdd142";
            attritem.NewValue = "62sdsd2942";
            item4.Attributes.Add(attritem);
            attritem.AttributeName = "gffdsfevge";
            attritem.OldValue = "aa";
            attritem.NewValue = "bbb";
            item4.Attributes.Add(attritem);

            res.Add(item4);

            return res;
        }

        //Added By Dhruv (Used in Create Update codebehind)
        public static bool IsInstrumentUnderWorkflow(string instrumentId, int moduleId)
        {
            mLogger.Debug("SRMWorkflowController -> IsInstrumentUnderWorkflow -> Start");
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(instrumentId))
                {
                    DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT queue_id FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
                                                                                     INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_instance wi
                                                                                         ON wi.workflow_instance_id = rq.workflow_instance_id AND wi.module_id = " + moduleId + @"
                                                                                     WHERE rq.is_active=1 AND instrument_id='" + instrumentId + @"'"), ConnectionConstants.RefMaster_Connection);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMWorkflowController -> IsInstrumentUnderWorkflow -> Exception ->" + ex.ToString());
            }
            finally
            {
                mLogger.Debug("SRMWorkflowController -> IsInstrumentUnderWorkflow -> End");
            }
            return result;
        }

        //Added By Dhruv to use on Search Security Screen, to check if security can be cloned (Context Menu).
        //NOTE : This method expects that all instrumentIds have SAME MODULEID.
        public static Dictionary<string, bool> AreInstrumentsUnderWorkflow(string instrumentIds, int moduleId)
        {
            mLogger.Debug("SRMWorkflowController -> AreInstrumentsUnderWorkflow -> Start");
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            try
            {
                if (!string.IsNullOrEmpty(instrumentIds))
                {
                    DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"CREATE TABLE #listOfInstrumentIds (instrumentId VARCHAR(20), isUnderWorkflow BIT);

                                                                                     INSERT INTO #listOfInstrumentIds(instrumentId)
                                                                                         SELECT item FROM dbo.REFM_GetList2Table('" + instrumentIds + @"',',')
																					
																					CREATE NONCLUSTERED INDEX final_Dates_sec_id
																					ON #listOfInstrumentIds (instrumentId)

                                                                                     UPDATE loi
                                                                                     SET isUnderWorkflow = CASE WHEN rq.queue_id IS NOT NULL THEN 1 ELSE 0 END
                                                                                     FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
                                                                                     INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_instance wi
                                                                                         ON wi.workflow_instance_id=rq.workflow_instance_id AND wi.module_id=" + moduleId + @"
                                                                                     RIGHT JOIN #listOfInstrumentIds loi
                                                                                         ON rq.instrument_id = loi.instrumentId AND rq.is_active=1

                                                                                     Select instrumentId, isUnderWorkflow FROM #listOfInstrumentIds

																					 DROP TABLE #listOfInstrumentIds"), ConnectionConstants.RefMaster_Connection);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            result = ds.Tables[0].AsEnumerable().ToDictionary(key => key.Field<string>("instrumentId"), value => value.Field<bool>("isUnderWorkflow"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMWorkflowController -> AreInstrumentsUnderWorkflow -> Exception ->" + ex.ToString());
            }
            finally
            {
                mLogger.Debug("SRMWorkflowController -> AreInstrumentsUnderWorkflow -> End");
            }
            return result;
        }

        private static List<int> FilterRadWorkflowInstanceIdsByUsername(List<int> radWorkflowInstanceIds, string username, RDBConnectionManager conMgr = null)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "_") + "_" + new Random().Next(1, 10000).ToString();
            string workflowRequestsTableNameOrg = "[workflowRequests_" + guid.ToString() + "]";
            string workflowRequestsTableName = "IVPRefMaster.dbo." + workflowRequestsTableNameOrg;

            try
            {
                mLogger.Debug("SRMWorkflowController -> FilterRadWorkflowInstanceIdsByUsername -> Start");

                DataTable dt = new DataTable();
                dt.Columns.Add("instance_id", typeof(int));

                if (radWorkflowInstanceIds != null && radWorkflowInstanceIds.Count > 0)
                {
                    foreach (var instanceId in radWorkflowInstanceIds)
                    {
                        DataRow dr = dt.NewRow();
                        dr["instance_id"] = instanceId;
                        dt.Rows.Add(dr);
                    }
                }

                CommonDALWrapper.ExecuteSelectQuery(string.Format(@"CREATE TABLE {0} (instance_id INT)", workflowRequestsTableName), ConnectionConstants.RefMaster_Connection);

                CommonDALWrapper.ExecuteBulkUpload(workflowRequestsTableName, dt, ConnectionConstants.RefMaster_Connection);

                string query = string.Format("exec [IVPRefMaster].[dbo].[SRM_FilterRadInstanceIdsByUsername] '{0}', '{1}'", workflowRequestsTableName, username);
                DataSet resultDS = null;

                if (conMgr == null)
                    resultDS = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                else
                    resultDS = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                if (resultDS != null && resultDS.Tables.Count > 0 && resultDS.Tables[0].Rows.Count > 0)
                {
                    var instanceIds = resultDS.Tables[0].AsEnumerable().Select(dr => dr.Field<int>("rad_workflow_instance_id"));
                    radWorkflowInstanceIds.Clear();
                    radWorkflowInstanceIds.AddRange(instanceIds);
                }

                return radWorkflowInstanceIds;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                try
                {
                    CommonDALWrapper.ExecuteSelectQuery("DROP TABLE " + workflowRequestsTableName, ConnectionConstants.RefMaster_Connection);
                }
                catch { }

                mLogger.Debug("SRMWorkflowController -> FilterRadWorkflowInstanceIdsByUsername -> End");
            }
        }

        public static List<SRMWorkflowRequestsCounts> GetWorkflowCounts(SRMWorkflowRequestsCountInfo info, List<int> typeIdsFilter)
        {
            List<SRMWorkflowRequestsCounts> counts = new List<SRMWorkflowRequestsCounts>();

            foreach (WorkflowStatusType statusType in info.StatusTypeCountsRequired)
            {
                var requestsCount = new SRMWorkflowRequestsCounts(statusType);

                var radInstanceIds = GetRadWorkflowInstanceIds(statusType, info.Username, info.StartDate, info.EndDate, info.ModuleId);
                radInstanceIds = FilterRadWorkflowInstanceIdsByUsername(radInstanceIds, info.Username);

                if (radInstanceIds.Count > 0)
                {
                    DataTable reqCount = GetWorkflowRequestsInfo(radInstanceIds, info.ModuleId);
                    foreach (DataRow dr in reqCount.Rows)
                    {
                        if (typeIdsFilter == null || typeIdsFilter.Contains(dr.Field<int>("type_id")))
                            requestsCount.AddToTypeCount(dr.Field<string>("type_name"), dr.Field<int>("type_id"), dr.Field<int>("cnt"));
                    }
                }

                counts.Add(requestsCount);
            }

            return counts;
        }

        private static List<int> GetRadWorkflowInstanceIds(WorkflowStatusType statusType, string username, DateTime? startDate, DateTime? endDate, int moduleId)
        {
            List<int> radInstanceIds = new List<int>();
            List<RWorkFlowInstanceInfo> radInstanceInfos = null;

            List<int> rejectedReqInstanceIds = new SRMRADWorkflow().GetFinalWorkflowInstanceStateStatedByUser(username, "Failed", moduleId);

            if (statusType == WorkflowStatusType.PENDING_AT_ME)
                radInstanceInfos = new SRMRADWorkflow().GetAllWOrkFlowInstances(WorkFlowStatus.pending, username, null);
            else if (statusType == WorkflowStatusType.MY_REQUESTS)
                radInstanceInfos = new SRMRADWorkflow().GetAllWOrkFlowInstances(WorkFlowStatus.started, username, null);
            else if (statusType == WorkflowStatusType.REJECTED_REQUESTS)
                radInstanceIds = rejectedReqInstanceIds;

            if (radInstanceInfos != null && radInstanceInfos.Count > 0)
            {
                if (startDate != null && endDate != null)
                    radInstanceInfos = radInstanceInfos.Where(r => r.ActionDate.Date >= startDate.Value.Date && r.ActionDate.Date <= endDate.Value.Date).ToList();
                else if (startDate != null && endDate == null)
                    radInstanceInfos = radInstanceInfos.Where(r => r.ActionDate.Date >= startDate.Value.Date).ToList();
                else if (startDate == null && endDate != null)
                    radInstanceInfos = radInstanceInfos.Where(r => r.ActionDate.Date <= endDate.Value.Date).ToList();
            }

            if ((statusType == WorkflowStatusType.PENDING_AT_ME || statusType == WorkflowStatusType.MY_REQUESTS) && radInstanceInfos != null && radInstanceInfos.Count() > 0)
            {
                var filteredInstanceIds = radInstanceInfos.Where(i => !rejectedReqInstanceIds.Contains(i.InstanceID)).Select(i => i.InstanceID);
                if (filteredInstanceIds != null)
                    radInstanceIds = filteredInstanceIds.Distinct().ToList();
            }

            return radInstanceIds;
        }

        private static DataTable GetWorkflowRequestsInfo(List<int> radInstanceIds, int moduleId, RDBConnectionManager conMgr = null)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("instance_id", typeof(int));

            if (radInstanceIds != null && radInstanceIds.Count > 0)
            {
                foreach (var instanceId in radInstanceIds)
                {
                    DataRow dr = dt.NewRow();
                    dr["instance_id"] = instanceId;
                    dt.Rows.Add(dr);
                }
            }

            var guid = Guid.NewGuid().ToString().Replace("-", "_") + "_" + new Random().Next(1, 10000).ToString();
            string workflowRequestsTableNameOrg = "[workflowRequests_" + guid.ToString() + "]";
            string workflowRequestsTableName = "IVPRefMaster.dbo." + workflowRequestsTableNameOrg;
            try
            {
                CommonDALWrapper.ExecuteSelectQuery(string.Format(@"CREATE TABLE {0} (instance_id INT)", workflowRequestsTableName), ConnectionConstants.RefMaster_Connection);

                CommonDALWrapper.ExecuteBulkUpload(workflowRequestsTableName, dt, ConnectionConstants.RefMaster_Connection);

                string query = string.Empty;
                if (moduleId == 3)
                {
                    query = @"
                    select sm.sectype_id as type_id, sm.sectype_name as type_name, count(*) cnt
                    from [IVPRefMaster].[dbo].[ivp_srm_workflow_requests_queue] req
                    join [IVPRefMaster].[dbo].[ivp_srm_workflow_instance] ins
                    on req.workflow_instance_id = ins.workflow_instance_id and ins.module_id = {0}
                    INNER JOIN {1} wrids
                    ON wrids.instance_id = req.rad_workflow_instance_id
                    join [IVPSecMaster].[dbo].[ivp_secm_sectype_master] sm 
                    on req.type_id = sm.sectype_id
                    where req.is_active = 1
                    group by sm.sectype_id, sm.sectype_name
                ";
                }
                else if (moduleId == 6 || moduleId == 18 || moduleId == 20)
                {
                    query = @"
                    select et.entity_type_id as type_id, et.entity_display_name as type_name, count(*) cnt
                    from [IVPRefMaster].[dbo].[ivp_srm_workflow_requests_queue] req
                    join [IVPRefMaster].[dbo].[ivp_srm_workflow_instance] ins
                    on req.workflow_instance_id = ins.workflow_instance_id and ins.module_id = {0}
                    INNER JOIN {1} wrids
                    ON wrids.instance_id = req.rad_workflow_instance_id
                    join [IVPRefMaster].[dbo].[ivp_refm_entity_type] et
                    on req.type_id = et.entity_type_id
                    where req.is_active = 1
                    group by et.entity_type_id, et.entity_display_name
                ";
                }

                query = string.Format(query, moduleId, workflowRequestsTableName);

                DataSet resultDS;
                if (conMgr == null)
                    resultDS = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                else
                    resultDS = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                if (resultDS != null && resultDS.Tables.Count > 0)
                    return resultDS.Tables[0];
            }
            catch (Exception ee)
            {
                mLogger.Error(ee.ToString());
                throw;
            }
            finally
            {
                try
                {
                    CommonDALWrapper.ExecuteSelectQuery("DROP TABLE " + workflowRequestsTableName, ConnectionConstants.RefMaster_Connection);
                }
                catch { }
            }

            return null;
        }

        public static string GetRequestedByForRequest(string instrumentId, int radWorkflowInstanceId, int moduleId)
        {
            string requested_by = "";

            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT requested_by FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
                                                                         INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_instance wi
                                                                             ON wi.workflow_instance_id = rq.workflow_instance_id AND wi.module_id = " + moduleId + @"
                                                                         WHERE rq.is_active=1 AND instrument_id='" + instrumentId + @"' AND rad_workflow_instance_id = " + radWorkflowInstanceId), ConnectionConstants.SecMaster_Connection);

            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    requested_by = Convert.ToString(ds.Tables[0].Rows[0]["requested_by"]);
                }
            }

            return requested_by;
        }

        #region UniquenessFailure
        public static List<KeyValuePair<string, List<SRMDuplicateInfo>>> RM_MassageUniquenessInfo(UniquenessFailureInfo uniquenessInfo)
        {
            Dictionary<string, List<SRMDuplicateInfo>> finalInfo = new Dictionary<string, List<SRMDuplicateInfo>>();

            SRMDuplicateInfo dupInfo = new SRMDuplicateInfo();
            finalInfo.Add(uniquenessInfo.UniqueKeyName, new List<SRMDuplicateInfo> { dupInfo });
            dupInfo.LstSaveDuplicateAttributesInKeyId = uniquenessInfo.AttributeNamesVsValues
                .Select(kvp => new SRMDuplicateAttributeInfo()
                {
                    AttributeDisplayName = kvp.Key,
                    AttributeValue = kvp.Value,
                    AttributeDataType = uniquenessInfo.AttributeNamesVsDataType[kvp.Key]
                })
                .ToList();
            Dictionary<string, string> duplicateEntityInfo = new Dictionary<string, string>();

            var typeVsEntityCodes = RMGetEntityTypeVsEntityCodesList(uniquenessInfo.SecIds.Select(s => s.SecurityId));
            foreach (var entity in uniquenessInfo.SecIds)
            {
                var typeVsEcKVP = typeVsEntityCodes.Where(kvp => kvp.Value.Contains(entity.SecurityId)).FirstOrDefault();
                duplicateEntityInfo.Add(entity.SecurityId, typeVsEcKVP.Key + "|" + "|" + GetUniquenessFailureTextFromEnum(entity.FailureType));
            }
            dupInfo.LstDuplicateSecurityInfo = duplicateEntityInfo.ToList();
            return finalInfo.ToList();
        }

        private static Dictionary<string, List<string>> RMGetEntityTypeVsEntityCodesList(IEnumerable<string> entities)
        {
            Dictionary<string, List<string>> entityTypeVsEntityCodes;

            Dictionary<string, List<string>> ecVsEntities = entities.GroupBy(ec => ec.Substring(0, 4)).ToDictionary(key => key.Key, value => value.ToList());
            string query = string.Format(@"
                select et.entity_code, et.entity_type_name, et.entity_display_name 
                from IVPRefMaster.dbo.ivp_refm_entity_type et
                join IVPRefMaster.dbo.REFM_GetList2Table('{0}',',') tbl
                on et.entity_code = tbl.item
            ", string.Join(",", ecVsEntities.Keys));
            DataSet etInfoDS = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            entityTypeVsEntityCodes = (
                    from kvp in ecVsEntities
                    join etInfo in etInfoDS.Tables[0].AsEnumerable()
                    on kvp.Key equals etInfo.Field<string>("entity_code")
                    select new { etName = etInfo.Field<string>("entity_display_name"), entities = kvp.Value }
                ).ToDictionary(key => key.etName, value => value.entities);

            return entityTypeVsEntityCodes;
        }

        private static string GetUniquenessFailureTextFromEnum(SMUniquenessFailureType uniqueFailureType)
        {
            switch (uniqueFailureType)
            {
                case SMUniquenessFailureType.GOLDEN:
                    return "Active";
                case SMUniquenessFailureType.WORKFLOW:
                    return "In Workflow";
                case SMUniquenessFailureType.DRAFT:
                    return "Draft";
                case SMUniquenessFailureType.USERDATA:
                    return "User Data";
            }
            return uniqueFailureType.ToString();
        }

        public static List<KeyValuePair<string, List<SRMDuplicateInfo>>> SM_MassageUniquenessInfo(UniquenessFailureInfo uniquenessInfo)
        {
            Assembly SecMasterDashboardServiceAss = Assembly.Load("SecMasterDashboardService");
            Type SecMasterDashboardService = SecMasterDashboardServiceAss.GetType("com.ivp.secm.api.ui.SecMasterDashboardService");
            MethodInfo SMPrepareUniquenessFailurePopupInfo = SecMasterDashboardService.GetMethod("SMPrepareUniquenessFailurePopupInfo");

            object dashboardServiceObj = Activator.CreateInstance(SecMasterDashboardService);
            List<UniquenessFailureInfo> failureInfoParam = new List<UniquenessFailureInfo>() { uniquenessInfo };
            var dupLst = (List<KeyValuePair<string, List<SRMDuplicateInfo>>>)SMPrepareUniquenessFailurePopupInfo.Invoke(
                dashboardServiceObj,
                new object[] { failureInfoParam }
            );

            return dupLst;
        }

        #endregion UniquenessFailure

        public static WorkflowsCountResponse GetWorkflowsCountPerModule(string userName, string moduleId)
        {
            Dictionary<int, WorkflowsCounts> moduleVsResponse = new Dictionary<int, WorkflowsCounts>();
            WorkflowsCountResponse FinalResponse = new WorkflowsCountResponse();
            int moduleid = 0;

            #region validation
            //if (string.IsNullOrEmpty(userName))
            //{
            //    FinalResponse.IsSuccess = false;
            //    FinalResponse.FailureReason = "User name can not be empty.";
            //    return FinalResponse;
            //}
            //validate username
            //List<RUserInfo> lstUserInfo = new RUserManagementService().GetAllUsersGDPR();

            //var Isvalid = from user in lstUserInfo
            //              where user.UserName == userName
            //              select user;

            //if (Isvalid.Count() < 1)
            //{
            //    FinalResponse.IsSuccess = false;
            //    FinalResponse.FailureReason = "Invalid user name.";
            //    return FinalResponse;
            //}

            if (string.IsNullOrEmpty(moduleId))
                moduleid = 0;
            else if (!int.TryParse(moduleId, out moduleid))
            {
                //FinalResponse.IsSuccess = false;
                //FinalResponse.FailureReason = "Invalid ModuleId.";
                //return FinalResponse;
            }
            #endregion

            try
            {
                //Parallel.ForEach(Enum.GetValues(typeof(SRMModules)).Cast<int>(), module =>
                foreach (var module in Enum.GetValues(typeof(WFSRMModules)).Cast<int>())
                {
                    //int module = 3;
                    if (module != Convert.ToInt32(WorkflowsCountApiConstants.ALL_SYSTEMS_MODULE) && module != Convert.ToInt32(WorkflowsCountApiConstants.CORP_ACTION_MODULE))
                    {
                        if (moduleid != 0 && (module != moduleid))
                        {
                            continue;
                        }
                        var CountsData = GetWorkflowStatusCount(module, userName);

                        WorkflowsCounts moduleCount = new WorkflowsCounts();
                        moduleCount.ModuleName = Enum.GetName(typeof(WFSRMModules), module);
                        WorkflowCountStateData pending = new WorkflowCountStateData();
                        pending.WorkflowState = WorkflowsCountApiConstants.PENDING_AT_ME;
                        pending.CountInfo = new List<WorkflowTypeCountInfo>();
                        WorkflowCountStateData rejected = new WorkflowCountStateData();
                        rejected.WorkflowState = WorkflowsCountApiConstants.REJECTED_REQUESTS;
                        rejected.CountInfo = new List<WorkflowTypeCountInfo>();
                        WorkflowCountStateData myRequests = new WorkflowCountStateData();
                        myRequests.WorkflowState = WorkflowsCountApiConstants.MY_REQUESTS;
                        myRequests.CountInfo = new List<WorkflowTypeCountInfo>();

                        if (CountsData != null)
                        {
                            foreach (var data in CountsData)
                            {
                                // fill data for pending at me
                                WorkflowTypeCountInfo typeCount_pending = new WorkflowTypeCountInfo();
                                typeCount_pending.WorkflowType = data.workflowType;
                                typeCount_pending.Count = data.pendingCount;
                                pending.CountInfo.Add(typeCount_pending);

                                // fill data for rejected at me
                                WorkflowTypeCountInfo typeCount_rej = new WorkflowTypeCountInfo();
                                typeCount_rej.WorkflowType = data.workflowType;
                                typeCount_rej.Count = data.rejectedCount;
                                rejected.CountInfo.Add(typeCount_rej);

                                // fill data for myrequests at me
                                WorkflowTypeCountInfo typeCount_myreq = new WorkflowTypeCountInfo();
                                typeCount_myreq.WorkflowType = data.workflowType;
                                typeCount_myreq.Count = data.myRequestsCount;
                                myRequests.CountInfo.Add(typeCount_myreq);
                            }

                        }
                        //res.WorkflowsCounts = new List<WorkflowsCounts>() { moduleCount };
                        List<WorkflowCountStateData> stateData = new List<WorkflowCountStateData>();
                        stateData.Add(pending);
                        stateData.Add(rejected);
                        stateData.Add(myRequests);
                        moduleCount.WorkflowCountsPerState = stateData;
                        moduleVsResponse.Add(module, moduleCount);

                    }
                }
                // });
                var modulesData = moduleVsResponse.OrderBy(x => x.Key);
                FinalResponse.WorkflowsCounts = new List<WorkflowsCounts>();

                foreach (var data in modulesData)
                {
                    FinalResponse.WorkflowsCounts.Add(data.Value);
                }
                FinalResponse.IsSuccess = true;
                FinalResponse.FailureReason = "";
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                FinalResponse.IsSuccess = false;
                FinalResponse.FailureReason = ex.Message;
            }
            return FinalResponse;
        }

        public static void CreateEmailStructure(List<WorkflowActionInfo> actionInfo, string userName, int moduleId, bool isInitiate, bool isRetrigger, string remarks, Dictionary<int, string> RADWorkflowInstanceIdVsRADUsers, string actionName, RDBConnectionManager conMgr, DateTime? timeStamp = null)
        {
            try
            {
                mLogger.Debug("SRMWorkflowController -> CreateEmailStructure -> Start");
                string guid = CommonDALWrapper.ExecuteSelectQuery(DALWrapperAppend.Replace(string.Format(@"select NEWID()")), ConnectionConstants.RefMaster_Connection).Tables[0].Rows[0][0].ToString();
                bool isCancel = false;
                string noOfRetry = RConfigReader.GetConfigAppSettings("NoOfRetry");
                string mailFrom = RConfigReader.GetConfigAppSettings("FromEmailIdForWorkflow");
                string transportName = RConfigReader.GetConfigAppSettings("MailTransportName");
                string instanceName = RConfigReader.GetConfigAppSettings("InstanceName");
                string staticURLFromRAD = string.Empty;
                int moduleIdForEmailFooter = -1;
                try
                {
                    staticURLFromRAD = GenerateStaticURLFromRAD(moduleId);
                    moduleIdForEmailFooter = moduleId;
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (ex.Message.Equals("module details is empty", StringComparison.OrdinalIgnoreCase))
                        {
                            int moduleIdFromAppSettings = Convert.ToInt32(RConfigReader.GetConfigAppSettings("ModuleId"));
                            if (moduleId == 18 || moduleId == 20)
                            {
                                try
                                {
                                    staticURLFromRAD = GenerateStaticURLFromRAD(moduleId, 6);
                                    moduleIdForEmailFooter = 6;
                                }
                                catch (Exception e)
                                {
                                    if (e.Message.Equals("module details is empty", StringComparison.OrdinalIgnoreCase) && moduleId != moduleIdFromAppSettings && moduleIdFromAppSettings != 6)
                                    {
                                        staticURLFromRAD = GenerateStaticURLFromRAD(moduleId, moduleIdFromAppSettings);
                                        moduleIdForEmailFooter = moduleIdFromAppSettings;
                                    }
                                }
                            }
                            else if (moduleId != moduleIdFromAppSettings)
                            {
                                staticURLFromRAD = GenerateStaticURLFromRAD(moduleId, moduleIdFromAppSettings);
                                moduleIdForEmailFooter = moduleIdFromAppSettings;
                            }
                        }
                    }
                    catch { }
                }

                if (actionName == creatorCancelsTheRequest)
                    isCancel = true;

                RUserManagementService group = new RUserManagementService();
                string userCultureName = "";
                if (userName == "admin")
                {
                    List<string> userNameList = new List<string> { userName };
                    userCultureName = group.GetUsersDetailGDPR(userNameList)[0].CultureName;
                }
                else
                {
                    userCultureName = group.GetUserDetailByIdGDPR(userName).CultureName;
                }
                RCultureInfo res = RCultureController.GetCultureInfo(userCultureName);
                string dateFormat = res.ShortDateFormat;
                string dateTimeFormat = res.LongDateFormat;

                Dictionary<int, string> InstrumentTypeIdVsName = GetInstrumentTypeIdVsTypeName(moduleId);

                Dictionary<int, List<WorkflowActionInfo>> workflowInstanceIdVsWorkflowActionList = new Dictionary<int, List<WorkflowActionInfo>>();
                HashSet<int> workflowInstanceIds = new HashSet<int>();

                DataTable instrumentIdsTable = new DataTable();
                instrumentIdsTable.Columns.Add("InstrumentId", typeof(string));
                instrumentIdsTable.Columns.Add("RADWorkflowInstanceId", typeof(int));
                foreach (var action in actionInfo)
                {
                    if (!workflowInstanceIdVsWorkflowActionList.ContainsKey(action.WorkflowInstanceID))
                        workflowInstanceIdVsWorkflowActionList.Add(action.WorkflowInstanceID, new List<WorkflowActionInfo>());
                    workflowInstanceIdVsWorkflowActionList[action.WorkflowInstanceID].Add(action);

                    workflowInstanceIds.Add(action.WorkflowInstanceID);

                    DataRow row = instrumentIdsTable.NewRow();
                    row["InstrumentId"] = action.InstrumentID;
                    row["RADWorkflowInstanceId"] = action.RadInstanceID;
                    instrumentIdsTable.Rows.Add(row);
                }


                Dictionary<string, Dictionary<string, Dictionary<string, SRMWorkflowMigrationController.ActionDataDetails>>> workflowNameVsRulePriorityVsActionNameVsActionDetails = new Dictionary<string, Dictionary<string, Dictionary<string, SRMWorkflowMigrationController.ActionDataDetails>>>(StringComparer.OrdinalIgnoreCase);
                workflowNameVsRulePriorityVsActionNameVsActionDetails = GetActionDetails(moduleId, workflowInstanceIds.ToList<int>());

                DataSet dataSetFromRequestQueueTable = null;
                if (isInitiate)
                    dataSetFromRequestQueueTable = PrepareDataSetCollection(instrumentIdsTable, isInitiate, actionInfo, userName, guid, conMgr, timeStamp);
                else
                    dataSetFromRequestQueueTable = PrepareDataSetCollection(instrumentIdsTable, isInitiate, null, null, guid, conMgr, null);

                Dictionary<int, PriorityAndRequestedOnAndRequestedBy> RADWorkflowInstanceIdVsStruct = RADWorkflowInstanceIdVsStructCollection(dataSetFromRequestQueueTable);

                Dictionary<int, string> workflowInstanceIdVsWorkflowInstanceName = WorkflowInstanceIdVsWorkflowInstanceName(dataSetFromRequestQueueTable);

                foreach (KeyValuePair<int, List<WorkflowActionInfo>> workflow in workflowInstanceIdVsWorkflowActionList)
                {
                    if (isInitiate || isRetrigger)
                        actionName = "Creator initiates the request";

                    List<int> RadWorkflowInstanceIdForFinalApprovalList = new List<int>();
                    List<int> RadWorkflowInstanceIdList = new List<int>();
                    DataTable workflowDetailsTable = new DataTable();
                    workflowDetailsTable.Columns.Add("workflow_instance_id", typeof(int));
                    workflowDetailsTable.Columns.Add("rad_workflow_instance_id", typeof(int));
                    workflowDetailsTable.Columns.Add("instrument_id", typeof(string));
                    workflowDetailsTable.Columns.Add("type_id", typeof(int));

                    //workflow.key-> WorkflowInstanceId
                    if (workflowInstanceIdVsWorkflowActionList.ContainsKey(workflow.Key))
                    {
                        foreach (WorkflowActionInfo wf in workflowInstanceIdVsWorkflowActionList[workflow.Key])
                        {
                            if (wf.NextState == "End" && (actionName.ToLower().Contains("approve") || isRetrigger || isInitiate))
                            {
                                RadWorkflowInstanceIdForFinalApprovalList.Add(wf.RadInstanceID);
                            }
                            else
                            {
                                RadWorkflowInstanceIdList.Add(wf.RadInstanceID);
                            }

                            DataRow row1 = workflowDetailsTable.NewRow();
                            row1["workflow_instance_id"] = wf.WorkflowInstanceID;
                            row1["rad_workflow_instance_id"] = wf.RadInstanceID;
                            row1["instrument_id"] = wf.InstrumentID;
                            row1["type_id"] = wf.TypeID;
                            workflowDetailsTable.Rows.Add(row1);
                        }


                        string workflowDetailsTableName = "IVPRefMaster.dbo.[workflowDetailsTableInternal_" + guid + "]";
                        string query = @"CREATE TABLE " + workflowDetailsTableName + "(workflow_instance_id int, rad_workflow_instance_id int, instrument_id varchar(max), type_id int)";
                        if (conMgr == null)
                        {
                            CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
                            CommonDALWrapper.ExecuteBulkUpload(workflowDetailsTableName, workflowDetailsTable, ConnectionConstants.RefMaster_Connection);
                        }
                        else
                        {
                            CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Insert, conMgr);
                            CommonDALWrapper.ExecuteBulkUpload(workflowDetailsTableName, workflowDetailsTable, conMgr);
                        }

                        //call db proc to get attribute values
                        List<AttributeVsValue> attrVsValueCombined = GetAttributesAndValues(moduleId, RadWorkflowInstanceIdForFinalApprovalList, RadWorkflowInstanceIdList, workflowDetailsTableName, dateFormat, dateTimeFormat, conMgr, isInitiate);


                        // collection of RADWorkflowInstanceIdVsAttributeNameVsAttributeValue from the output of called proc to store data.
                        Dictionary<int, Dictionary<string, string>> RADWorkflowInstanceIdVsAttributeNameVsAttributeValue = new Dictionary<int, Dictionary<string, string>>();
                        Dictionary<int, string> RADWorkdlowInstanceIdVsPrimaryAttributeName = new Dictionary<int, string>();
                        foreach (AttributeVsValue v in attrVsValueCombined)
                        {
                            if (!RADWorkflowInstanceIdVsAttributeNameVsAttributeValue.ContainsKey(v.InstanceID))
                                RADWorkflowInstanceIdVsAttributeNameVsAttributeValue.Add(v.InstanceID, new Dictionary<string, string>());
                            if (!RADWorkflowInstanceIdVsAttributeNameVsAttributeValue[v.InstanceID].ContainsKey(v.AttributeName))
                                RADWorkflowInstanceIdVsAttributeNameVsAttributeValue[v.InstanceID].Add(v.AttributeName, v.AttributeValue);


                            if (!RADWorkdlowInstanceIdVsPrimaryAttributeName.ContainsKey(v.InstanceID) && v.isPrimary)
                                RADWorkdlowInstanceIdVsPrimaryAttributeName.Add(v.InstanceID, v.AttributeName);

                        }


                        string workflowInstanceName = null;
                        if (workflowInstanceIdVsWorkflowInstanceName.ContainsKey(workflow.Key))
                            workflowInstanceName = workflowInstanceIdVsWorkflowInstanceName[workflow.Key];

                        Dictionary<SRMWorkflowMigrationController.ActionDataDetails, List<InstrumentIdAndRadInstanceId>> SelectedActionVsListOfInstrumentIdAndRadInstanceId = new Dictionary<SRMWorkflowMigrationController.ActionDataDetails, List<InstrumentIdAndRadInstanceId>>();
                        SRMWorkflowMigrationController.ActionDataDetails selectedAction = new SRMWorkflowMigrationController.ActionDataDetails();

                        foreach (WorkflowActionInfo value in workflow.Value)
                        {
                            int rulePriority = -1;
                            if (RADWorkflowInstanceIdVsStruct.ContainsKey(value.RadInstanceID))
                            {
                                rulePriority = RADWorkflowInstanceIdVsStruct[value.RadInstanceID].priority;
                            }
                            if (!isInitiate && !isRetrigger && !isCancel)
                                actionName = value.StateNameAndActionName;

                            actionName = actionName.ToUpper();
                            mLogger.Debug("action name- " + actionName);

                            if (workflowNameVsRulePriorityVsActionNameVsActionDetails.ContainsKey(workflowInstanceName) && workflowNameVsRulePriorityVsActionNameVsActionDetails[workflowInstanceName].ContainsKey(Convert.ToString(rulePriority)) && workflowNameVsRulePriorityVsActionNameVsActionDetails[workflowInstanceName][Convert.ToString(rulePriority)].ContainsKey(actionName))
                            {
                                if (workflowNameVsRulePriorityVsActionNameVsActionDetails[workflowInstanceName][Convert.ToString(rulePriority)][actionName].CheckBoxForEachAction)
                                {
                                    selectedAction = workflowNameVsRulePriorityVsActionNameVsActionDetails[workflowInstanceName][Convert.ToString(rulePriority)][actionName];
                                    if (!SelectedActionVsListOfInstrumentIdAndRadInstanceId.ContainsKey(selectedAction))
                                        SelectedActionVsListOfInstrumentIdAndRadInstanceId.Add(selectedAction, new List<InstrumentIdAndRadInstanceId>());

                                    InstrumentIdAndRadInstanceId obj1 = new InstrumentIdAndRadInstanceId();
                                    obj1.InstrumentId = value.InstrumentID;
                                    obj1.RadInstanceId = value.RadInstanceID;
                                    SelectedActionVsListOfInstrumentIdAndRadInstanceId[selectedAction].Add(obj1);
                                }
                            }
                        }

                        Dictionary<string, string> userdic = GetMassagedUser();
                        string user = userdic.ContainsKey(userName) ? userdic[userName] : userName;
                        int InstrumentsCount = 0;
                        string RequiredInstrumentId = null;
                        int RequiredRADWorkflowInstanceId = -1;
                        string PrimaryAttributeName = null;
                        List<int> RADWorkflowInstanceIdList = new List<int>();
                        DataTable EmailAttachmentExcel = new DataTable();
                        bool PopulateDataInDataTable = false;

                        foreach (var v in SelectedActionVsListOfInstrumentIdAndRadInstanceId)
                        {
                            RADWorkflowInstanceIdList = new List<int>();
                            InstrumentsCount = v.Value.Count;
                            if (InstrumentsCount > 100)
                                PopulateDataInDataTable = true;

                            StringBuilder html = new StringBuilder("");
                            if (v.Key.SendConsolidatedEmailForBulkAction)
                            {
                                html.Append(CreateMailHeader(v.Key.MailBodyTitle, v.Key.MailBodyContent, v.Key.DataSectionAttributes, true, InstrumentsCount, EmailAttachmentExcel));
                            }

                            foreach (InstrumentIdAndRadInstanceId value in v.Value)
                            {
                                var row = EmailAttachmentExcel.NewRow();
                                RADWorkflowInstanceIdList.Add(value.RadInstanceId);

                                if (InstrumentsCount == 1)
                                {
                                    RequiredInstrumentId = value.InstrumentId;
                                    RequiredRADWorkflowInstanceId = value.RadInstanceId;
                                }

                                if (!v.Key.SendConsolidatedEmailForBulkAction)
                                {
                                    html.Clear();
                                    html.Append(CreateMailHeader(v.Key.MailBodyTitle, v.Key.MailBodyContent, v.Key.DataSectionAttributes, false, InstrumentsCount));
                                }

                                if (RADWorkflowInstanceIdVsAttributeNameVsAttributeValue.ContainsKey(value.RadInstanceId))
                                {
                                    if (!PopulateDataInDataTable)
                                    {
                                        html.Append("<tr>");
                                    }

                                    Dictionary<string, string> attrNameVsattrValue = RADWorkflowInstanceIdVsAttributeNameVsAttributeValue[value.RadInstanceId];

                                    for (int i = 0; i < v.Key.DataSectionAttributes.Count; i++)
                                    {
                                        string attribute = v.Key.DataSectionAttributes[i];
                                        if (attrNameVsattrValue.ContainsKey(attribute))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + attrNameVsattrValue[attribute] + "</td>");
                                            else
                                                row[attribute] = attrNameVsattrValue[attribute];
                                        }
                                        else if (attribute.Equals("Security Type", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (RADWorkflowInstanceIdVsStruct.ContainsKey(value.RadInstanceId))
                                            {
                                                PriorityAndRequestedOnAndRequestedBy structValue = RADWorkflowInstanceIdVsStruct[value.RadInstanceId];
                                                string typeName = InstrumentTypeIdVsName[structValue.typeID];
                                                if (!PopulateDataInDataTable)
                                                    html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + typeName + "</td>");
                                                else
                                                    row[attribute] = typeName;
                                            }
                                        }
                                        else if (attribute.Equals("Security ID", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + value.InstrumentId + "</td>");
                                            else
                                                row[attribute] = value.InstrumentId;
                                        }
                                        else if (attribute.Equals("Requested By", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (RADWorkflowInstanceIdVsStruct.ContainsKey(value.RadInstanceId))
                                            {
                                                PriorityAndRequestedOnAndRequestedBy structValue = RADWorkflowInstanceIdVsStruct[value.RadInstanceId];
                                                if (!PopulateDataInDataTable)
                                                    html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + user + "</td>");
                                                else
                                                    row[attribute] = user;
                                            }
                                        }
                                        else if (attribute.Equals("Requested Time", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (RADWorkflowInstanceIdVsStruct.ContainsKey(value.RadInstanceId))
                                            {
                                                PriorityAndRequestedOnAndRequestedBy structValue = RADWorkflowInstanceIdVsStruct[value.RadInstanceId];
                                                if (!PopulateDataInDataTable)
                                                    html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + structValue.requestedOn.TimeOfDay + "</td>");
                                                else
                                                    row[attribute] = structValue.requestedOn.TimeOfDay;
                                            }
                                        }
                                        else if (attribute.Equals("Approved By", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + user + "</td>");
                                            else
                                                row[attribute] = user;
                                        }
                                        else if (attribute.Equals("Approved At", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + DateTime.Now + "</td>");
                                            else
                                                row[attribute] = DateTime.Now;
                                        }
                                        else if (attribute.Equals("Approver Comments", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + remarks + "</td>");
                                            else
                                                row[attribute] = remarks;
                                        }
                                        else if (attribute.Equals("Rejected By", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + user + "</td>");
                                            else
                                                row[attribute] = user;
                                        }
                                        else if (attribute.Equals("Rejected Time", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + DateTime.Now + "</td>");
                                            else
                                                row[attribute] = DateTime.Now;
                                        }
                                        else if (attribute.Equals("Rejection Comments", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + remarks + "</td>");
                                            else
                                                row[attribute] = remarks;
                                        }
                                        else if (attribute.Equals("Cancelled By", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + user + "</td>");
                                            else
                                                row[attribute] = user;
                                        }
                                        else if (attribute.Equals("Cancellation Time", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + DateTime.Now + "</td>");
                                            else
                                                row[attribute] = DateTime.Now;
                                        }
                                        else if (attribute.Equals("Cancellation Comments", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + remarks + "</td>");
                                            else
                                                row[attribute] = remarks;
                                        }
                                        else if (attribute.Equals("Entity Type", StringComparison.OrdinalIgnoreCase))
                                        {
                                            PriorityAndRequestedOnAndRequestedBy structValue = RADWorkflowInstanceIdVsStruct[value.RadInstanceId];
                                            string typeName = InstrumentTypeIdVsName[structValue.typeID];
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + typeName + "</td>");
                                            else
                                                row[attribute] = typeName;
                                        }
                                        else if (attribute.Equals("Entity Code", StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (!PopulateDataInDataTable)
                                                html.Append("<td style='border:1px solid #bbbbbb;text-align:center;font-size:13px;padding:2px 5px 2px 5px;'>" + value.InstrumentId + "</td>");
                                            else
                                                row[attribute] = value.InstrumentId;
                                        }

                                    }
                                    if (!PopulateDataInDataTable)
                                        html.Append("</tr>");

                                }

                                if (RADWorkdlowInstanceIdVsPrimaryAttributeName.ContainsKey(value.RadInstanceId))
                                {
                                    PrimaryAttributeName = RADWorkdlowInstanceIdVsPrimaryAttributeName[value.RadInstanceId];
                                }
                                if (!v.Key.SendConsolidatedEmailForBulkAction)
                                {
                                    html.Append(CreateMailFooter(v.Key.SendConsolidatedEmailForBulkAction, v.Key.KeepApplicationURLInTheFooter, value.InstrumentId, value.RadInstanceId, 1, moduleId, staticURLFromRAD, moduleIdForEmailFooter, RadWorkflowInstanceIdForFinalApprovalList));
                                    SendSubscriptionMail(v.Key.To, v.Key.KeepCreatorInCC, v.Key.SendConsolidatedEmailForBulkAction, v.Key.Subject, v.Key.BulkSubject, RADWorkflowInstanceIdVsRADUsers, PrimaryAttributeName, html.ToString(), noOfRetry, mailFrom, transportName, 0, instanceName, RADWorkflowInstanceIdList, userName);
                                }
                                if (PopulateDataInDataTable)
                                    EmailAttachmentExcel.Rows.Add(row.ItemArray);
                            }

                            if (v.Key.SendConsolidatedEmailForBulkAction)
                            {
                                if (InstrumentsCount == 1)
                                {
                                    html.Append(CreateMailFooter(v.Key.SendConsolidatedEmailForBulkAction, v.Key.KeepApplicationURLInTheFooter, RequiredInstrumentId, RequiredRADWorkflowInstanceId, InstrumentsCount, moduleId, staticURLFromRAD, moduleIdForEmailFooter, RadWorkflowInstanceIdForFinalApprovalList));
                                    SendSubscriptionMail(v.Key.To, v.Key.KeepCreatorInCC, v.Key.SendConsolidatedEmailForBulkAction, v.Key.Subject, v.Key.BulkSubject, RADWorkflowInstanceIdVsRADUsers, PrimaryAttributeName, html.ToString(), noOfRetry, mailFrom, transportName, InstrumentsCount, instanceName, RADWorkflowInstanceIdList, userName);
                                }
                                else
                                {
                                    html.Append(CreateMailFooter(v.Key.SendConsolidatedEmailForBulkAction, v.Key.KeepApplicationURLInTheFooter, null, RequiredRADWorkflowInstanceId, InstrumentsCount, moduleId, staticURLFromRAD, moduleIdForEmailFooter, RadWorkflowInstanceIdForFinalApprovalList));
                                    SendSubscriptionMail(v.Key.To, v.Key.KeepCreatorInCC, v.Key.SendConsolidatedEmailForBulkAction, v.Key.Subject, v.Key.BulkSubject, RADWorkflowInstanceIdVsRADUsers, "", html.ToString(), noOfRetry, mailFrom, transportName, InstrumentsCount, instanceName, RADWorkflowInstanceIdList, userName, EmailAttachmentExcel);
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                mLogger.Debug("SRMWorkflowController -> CreateEmailStructure -> End");
            }
        }

        private static void UpdateActionDataSectionAttributesForObjectRow(SRMWorkflowMigrationController.ActionDataDetails actionDataDetails, string dataSectionAttributes)
        {
            if (actionDataDetails != null && actionDataDetails.DataSectionAttributes != null)
                actionDataDetails.DataSectionAttributes.Add(dataSectionAttributes);
        }

        private static SRMWorkflowMigrationController.ActionDataDetails GetActionDataDetailsForObjectRow(ObjectRow row)
        {
            SRMWorkflowMigrationController.ActionDataDetails actionDataDetails = new SRMWorkflowMigrationController.ActionDataDetails();
            if (Convert.ToString(row["Include Action"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                actionDataDetails.CheckBoxForEachAction = true;
            else
                actionDataDetails.CheckBoxForEachAction = false;
            actionDataDetails.ActionName = Convert.ToString(row["Action Name"]);
            if (Convert.ToString(row["Keep Application URL In Footer"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                actionDataDetails.KeepApplicationURLInTheFooter = true;
            else
                actionDataDetails.KeepApplicationURLInTheFooter = false;
            if (Convert.ToString(row["Send Consolidated Email For Bulk Action"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                actionDataDetails.SendConsolidatedEmailForBulkAction = true;
            else
                actionDataDetails.SendConsolidatedEmailForBulkAction = false;
            if (Convert.ToString(row["Keep Creator In CC"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                actionDataDetails.KeepCreatorInCC = true;
            else
                actionDataDetails.KeepCreatorInCC = false;
            actionDataDetails.To = Convert.ToString(row["To"]);
            actionDataDetails.Subject = Convert.ToString(row["Subject"]);
            actionDataDetails.BulkSubject = Convert.ToString(row["Bulk Subject"]);
            //actionDataDetails.MailBodyTitle = Convert.ToString(row["Mail Body Title"]);
            actionDataDetails.MailBodyTitle = "";
            actionDataDetails.MailBodyContent = Convert.ToString(row["Mail Body Content"]);
            actionDataDetails.DataSectionAttributes.Add(Convert.ToString(row["Data Section Attributes"]));

            return actionDataDetails;
        }

        internal static void SendSubscriptionMail(string To, bool KeepCreatorInCC, bool sendConsolidatedEmailForBulkAction, string subject, string bulkSubject, Dictionary<int,string> RADWorkflowInstanceIdVsRADUsers, string PrimaryAttributeName, string body, string noOfRetry, string mailFrom, string transportName, int count, string instanceName, List<int> rwid, string userName, DataTable EmailAttachmentExcel = null)
        {
            mLogger.Debug("SMTaskSubscriptionMail: SendMail -> Start");

            string emailTo = string.Empty;
            RNotificationInfo notificationInfo = null;
            RMailContent mailContent = null;
            try
            {
                notificationInfo = new RNotificationInfo();
                mailContent = new RMailContent();

                notificationInfo.NoOfRetry = Convert.ToInt32(noOfRetry);
                notificationInfo.NotificationFrequency = 5;
                notificationInfo.TransportName = transportName;

                StringBuilder UsersDetails = new StringBuilder();
                bool UsersFromRAD = false;
                if (rwid.Count>0)
                {
                    foreach (var id in rwid)
                    {
                        if (RADWorkflowInstanceIdVsRADUsers!=null && RADWorkflowInstanceIdVsRADUsers.ContainsKey(id))
                        {
                            UsersDetails.Append(RADWorkflowInstanceIdVsRADUsers[id]);
                            UsersDetails.Append(",");
                        }
                    }
                    UsersFromRAD = true;
                }
                if (To.Length > 0)
                {
                    if (UsersFromRAD)
                    {
                        UsersDetails.Append(To);
                    }
                    else
                        UsersDetails.Append(To);
                }

                mailContent.To = UsersDetails.ToString();


                mailContent.From = mailFrom;

                if (KeepCreatorInCC)
                {
                    RUserManagementService group = new RUserManagementService();
                    string userEmailId = string.Empty;
                    if (userName == "admin")
                    {
                        userEmailId = group.GetUserInfoGDPR(userName).EmailId;
                    }
                    else
                    {
                        userEmailId = group.GetUserDetailByIdGDPR(userName).EmailId;
                    }

                    mailContent.CC = userEmailId;

                }
                else
                    mailContent.CC = "";

                if (!sendConsolidatedEmailForBulkAction || count == 1)
                {
                    if (subject.Contains("@instance"))
                        subject = subject.Replace("@instance", instanceName);
                    if (subject.Contains("@primaryAttribute"))
                        subject = subject.Replace("@primaryAttribute", PrimaryAttributeName);

                    mailContent.Subject = subject;
                }
                else
                {
                    if (bulkSubject.Contains("@instance"))
                        bulkSubject = bulkSubject.Replace("@instance", instanceName);
                    if (bulkSubject.Contains("@requestCount"))
                        bulkSubject = bulkSubject.Replace("@requestCount", Convert.ToString(count));

                    mailContent.Subject = bulkSubject;
                }

                mailContent.Body = body;
                if (count > 100)
                {
                    DataSet EmailAttachmentExcelDataSet = new DataSet();
                    EmailAttachmentExcelDataSet.Tables.Add(EmailAttachmentExcel);


                    byte[] byteData = SRMCommon.GetExcelByteArrayNew(EmailAttachmentExcelDataSet);
                    string fileFullName = "Workflow Requests" + "." + "xlsx";
                    string basePath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                    string tempFileLocation = "WorkflowEmailFiles\\" + userName + "\\" + DateTime.Now.ToString("yyyy-MM-dd--hh--mm--ss") + "\\"; // + randomly generated suffix
                    string filePath = basePath + tempFileLocation;
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    filePath += "\\" + fileFullName;
                    SRMCommon.DeleteFile(filePath);

                    //write file
                    File.WriteAllBytes(filePath, byteData);

                    ArrayList attachments = new ArrayList();

                    attachments.Add(filePath);
                    mailContent.MailAttachment = attachments;

                }

                mailContent.IsBodyHTML = true;

                notificationInfo.NotificationMessage = mailContent;

                if (mailContent.To.Length > 0)
                {
                    System.Threading.Thread taskThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SendMailInThread));
                    taskThread.Start(notificationInfo);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally { mLogger.Debug("SMTaskSubscriptionMail: SendMail -> End"); }
        }

        internal static void SendMailInThread(object objNotificationInfo)
        {
            RNotificationManager manager = new RNotificationManager();
            RNotificationInfo notificationInfo = (RNotificationInfo)objNotificationInfo;
            manager.GenerateNotification(notificationInfo);
        }

        internal static string CreateMailHeader(string MailBodyTitle, string MailBodyContent, List<string> DataSectionAttributes, bool isBulkRequest, int count, DataTable EmailAttachmentExcel = null)
        {
            mLogger.Debug("Start->  CreateMailHeader method");
            StringBuilder htmlInternal = new StringBuilder("");
            if (isBulkRequest && count > 1)
            {
                if (MailBodyContent.IndexOf("security", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    int index = MailBodyContent.IndexOf("security", StringComparison.OrdinalIgnoreCase);
                    MailBodyContent = MailBodyContent.Remove(index + 7, 1).Insert(index + 7, "ies");
                }
                if (MailBodyContent.IndexOf("entity", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    int index = MailBodyContent.IndexOf("entity", StringComparison.OrdinalIgnoreCase);
                    MailBodyContent = MailBodyContent.Remove(index + 5, 1).Insert(index + 5, "ies");
                }

            }
            htmlInternal.Append("<Body>");
            htmlInternal.Append("<div style='width:92%;border:#e3e0db;background-color:#e5e2e263;border-radius:7px;'>");
            htmlInternal.Append("<div style='display:inline-block;margin-left:3%;margin-top:3%;font-size:14px;color:black;white-space:pre-wrap;'>" + MailBodyContent + "</div><br/>");
            if (count <= 100)
            {
                htmlInternal.Append("<div style='overflow:auto;margin-left:3%;margin-top:3%;position:relative;max-height:450px;'>");
                htmlInternal.Append("<table style='border-collapse: collapse;margin-right:2%;width:92%;'>");
                htmlInternal.Append("<tr style='background-color:#e3e0db;position:sticky;top:0;z-index:1;'>");
                for (var i = 0; i < DataSectionAttributes.Count; i++)
                {
                    htmlInternal.Append("<th style='border-collapse: collapse;border:1px solid #bbbbbb;padding:3px 5px 3px 5px;width:fit-content;font-size:14px'>" + DataSectionAttributes[i] + "</th>");
                }
                htmlInternal.Append("</tr>");
            }
            else
            {
                for (var i = 0; i < DataSectionAttributes.Count; i++)
                {
                    EmailAttachmentExcel.Columns.Add(DataSectionAttributes[i], typeof(string));
                }
            }

            mLogger.Debug("End->  CreateMailHeader method");
            return htmlInternal.ToString();
        }

        internal static string CreateMailFooter(bool SendConsolidatedEmailForBulkAction, bool KeepApplicationURLInTheFooter, string instrumentId, int RADWorkflowInstanceId, int count, int moduleId, string staticURLFromRAD, int moduleIdForEmailFooter, List<int> RadWorkflowInstanceIdForFinalApprovalList)
        {
            mLogger.Debug("Start->  CreateMailFooter method");
            StringBuilder htmlInternal = new StringBuilder("");
            string url = String.Empty;
            string urlText = null;
            if (count <= 100)
                htmlInternal.Append("</table></div>");
            htmlInternal.Append("<br/><br/><br/>");
            bool isSecOrNot = moduleId == 3 ? true : false;

            if (KeepApplicationURLInTheFooter)
            {
                url = GetApplicationURL(SendConsolidatedEmailForBulkAction, instrumentId, RADWorkflowInstanceId, count, isSecOrNot, staticURLFromRAD, moduleIdForEmailFooter, RadWorkflowInstanceIdForFinalApprovalList);

                if (count == 1)
                {
                    if (Convert.ToInt32(moduleId) == 3)
                        urlText = "View Security";
                    else
                        urlText = "View Entity";
                }
                else
                {
                    urlText = "Workflow Inbox";

                }
                htmlInternal.Append("<div style='padding:5px 0px 5px 0px;background-color:#e3e0db;text-align:center;width:100%;'><span>Application Link : </span><a href = '" + url + "'>" + urlText + "</a>");
            }

            htmlInternal.Append("</div></div></Body>");
            mLogger.Debug("End->  CreateMailFooter method");
            return htmlInternal.ToString();
        }

        public static Dictionary<string, Dictionary<string, Dictionary<string, SRMWorkflowMigrationController.ActionDataDetails>>> GetActionDetails(int moduleId, List<int> workflowInstanceIds)
        {
            mLogger.Debug("Start->  GetActionDetails method");
            Dictionary<string, Dictionary<string, Dictionary<string, SRMWorkflowMigrationController.ActionDataDetails>>> workflowNameVsRulePriorityVsActionNameVsActionDetails = new Dictionary<string, Dictionary<string, Dictionary<string, SRMWorkflowMigrationController.ActionDataDetails>>>(StringComparer.OrdinalIgnoreCase);


            ObjectSet result = null;
            SRMWorkflowMigrationController obj = new SRMWorkflowMigrationController();
            result = obj.GetAllWorkflowsConfiguration(moduleId, workflowInstanceIds);
            mLogger.Debug("Returned from GetAllWorkflowsConfiguration method");

            string workflowName = null;
            foreach (ObjectRow row in result.Tables[5].Rows)
            {
                workflowName = Convert.ToString(row["Workflow Name"]);
                if (!workflowNameVsRulePriorityVsActionNameVsActionDetails.ContainsKey(workflowName))
                    workflowNameVsRulePriorityVsActionNameVsActionDetails.Add(workflowName, new Dictionary<string, Dictionary<string, SRMWorkflowMigrationController.ActionDataDetails>>(StringComparer.OrdinalIgnoreCase));
                string rulePriority = null;
                if (Convert.ToString(row["Rule Priority"]) == "")
                    rulePriority = "-1";
                else
                    rulePriority = Convert.ToString(row["Rule Priority"]);
                if (!workflowNameVsRulePriorityVsActionNameVsActionDetails[workflowName].ContainsKey(rulePriority))
                    workflowNameVsRulePriorityVsActionNameVsActionDetails[workflowName].Add(rulePriority, new Dictionary<string, SRMWorkflowMigrationController.ActionDataDetails>(StringComparer.OrdinalIgnoreCase));
                string actionNameInternal = Convert.ToString(row["Action Name"]);
                actionNameInternal = actionNameInternal.ToUpper();
                if (!workflowNameVsRulePriorityVsActionNameVsActionDetails[workflowName][rulePriority].ContainsKey(actionNameInternal))
                    workflowNameVsRulePriorityVsActionNameVsActionDetails[workflowName][rulePriority].Add(actionNameInternal, GetActionDataDetailsForObjectRow(row));
                else
                    UpdateActionDataSectionAttributesForObjectRow(workflowNameVsRulePriorityVsActionNameVsActionDetails[workflowName][rulePriority][actionNameInternal], Convert.ToString(row["Data Section Attributes"]));
            }

            mLogger.Debug("End->  GetActionDetails method");
            return workflowNameVsRulePriorityVsActionNameVsActionDetails;
        }

        internal static Dictionary<int, string> GetInstrumentTypeIdVsTypeName(int moduleId)
        {
            mLogger.Debug("Entered in GetInstrumentTypeIdVsTypeName method with moduleId- " + moduleId);
            Dictionary<int, string> result = new Dictionary<int, string>();
            try
            {
                if (moduleId == 3)
                {
                    DataTable SecTypeIdVsNameDataTable = CommonDALWrapper.ExecuteSelectQuery(DALWrapperAppend.Replace(string.Format(@"select sectype_id,sectype_name from IVPSecMaster.dbo.ivp_secm_sectype_master order by sectype_id")), ConnectionConstants.SecMaster_Connection).Tables[0];

                    foreach (DataRow dr in SecTypeIdVsNameDataTable.Rows)
                    {
                        if (!result.ContainsKey(Convert.ToInt32(dr["sectype_id"])))
                            result.Add(Convert.ToInt32(dr["sectype_id"]), Convert.ToString(dr["sectype_name"]));
                    }
                }

                else
                {
                    DataTable EntityCodeVsEntityTypeName = CommonDALWrapper.ExecuteSelectQuery(DALWrapperAppend.Replace(string.Format(@"select entity_type_id,entity_display_name from IVPRefMaster.dbo.ivp_refm_entity_type")), ConnectionConstants.RefMaster_Connection).Tables[0];

                    foreach (DataRow dr in EntityCodeVsEntityTypeName.Rows)
                    {
                        if (!result.ContainsKey(Convert.ToInt32(dr["entity_type_id"])))
                            result.Add(Convert.ToInt32(dr["entity_type_id"]), Convert.ToString(dr["entity_display_name"]));
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                mLogger.Debug("End-> in GetInstrumentTypeIdVsTypeName method");
            }
            return result;

        }

        private static string GenerateStaticURLFromRAD(int moduleId, int modifiedModuleId = -1)
        {
            string url = string.Empty;
            DataSet moduleDetails = null;
            RUserController rUserController = new RUserController();
            if (modifiedModuleId == -1)
                moduleDetails = rUserController.GetModuleDetails(Convert.ToInt32(moduleId));
            else
                moduleDetails = rUserController.GetModuleDetails(Convert.ToInt32(modifiedModuleId));
            if (moduleDetails.Tables[0].Rows.Count > 0)
            {
                url = Convert.ToString(moduleDetails.Tables[0].Rows[0]["module_url"]);
            }
            else
            {
                mLogger.Debug("module details is empty");
                throw new Exception("module details is empty");
            }
            return url;
        }

        public static string GetApplicationURL(bool sendConsolidatedEmailForBulkAction, string instrumentId, int RADWorkflowInstanceId, int count, bool isSecOrNot, string staticURLFromRAD, int moduleId, List<int> RadWorkflowInstanceIdForFinalApprovalList)
        {
            string url = string.Empty;
            if (!string.IsNullOrEmpty(staticURLFromRAD))
            {
                if (sendConsolidatedEmailForBulkAction && count > 1)
                    url = staticURLFromRAD + "?identifier=SRM_WorkflowInbox&moduleId=" + moduleId;
                else
                {
                    if (isSecOrNot)
                    {
                        int isSecurityUnderWorkflow = 1;
                        if (RadWorkflowInstanceIdForFinalApprovalList.Contains(RADWorkflowInstanceId))
                            url = staticURLFromRAD + "?secId=" + instrumentId;
                        else
                            url = staticURLFromRAD + "?secId=" + instrumentId + "&isSecurityUnderWorkflow=" + isSecurityUnderWorkflow;

                    }
                    else
                    {
                        if (RadWorkflowInstanceIdForFinalApprovalList.Contains(RADWorkflowInstanceId))
                            url = staticURLFromRAD + "?entityCode=" + instrumentId + "&pageIdentifier=UpdateEntityFromSearch";
                        else
                            url = staticURLFromRAD + "?entityCode=" + instrumentId + "&pageIdentifier=WorkflowEntity";
                    }
                }
            }
            return url;
        }


        private static Dictionary<int, PriorityAndRequestedOnAndRequestedBy> RADWorkflowInstanceIdVsStructCollection(DataSet ds1)
        {
            mLogger.Debug("Start->  RADWorkflowInstanceIdVsStructCollection method");
            //rad_workflow_instance_idvsstruct obj=priority,req_on,req-By,type_id
            Dictionary<int, PriorityAndRequestedOnAndRequestedBy> RADWorkflowInstanceIdVsStruct = new Dictionary<int, PriorityAndRequestedOnAndRequestedBy>();
            foreach (DataRow row in ds1.Tables[0].Rows)
            {
                PriorityAndRequestedOnAndRequestedBy priorityAndRequestedOnAndRequestedBy = new PriorityAndRequestedOnAndRequestedBy();
                priorityAndRequestedOnAndRequestedBy.priority = Convert.ToInt32(row["priority"]);
                priorityAndRequestedOnAndRequestedBy.requestedOn = Convert.ToDateTime(row["requested_on"]);
                priorityAndRequestedOnAndRequestedBy.requestedBy = Convert.ToString(row["requested_by"]);
                priorityAndRequestedOnAndRequestedBy.typeID = Convert.ToInt32(row["type_id"]);

                int RADWorkflowInstanceId = Convert.ToInt32(row["rad_workflow_instance_id"]);



                if (!RADWorkflowInstanceIdVsStruct.ContainsKey(RADWorkflowInstanceId))
                    RADWorkflowInstanceIdVsStruct.Add(RADWorkflowInstanceId, priorityAndRequestedOnAndRequestedBy);
            }
            mLogger.Debug("End->  RADWorkflowInstanceIdVsStructCollection method");
            return RADWorkflowInstanceIdVsStruct;
        }

        private static Dictionary<int, string> WorkflowInstanceIdVsWorkflowInstanceName(DataSet ds1)
        {
            mLogger.Debug("Start->  WorkflowInstanceIdVsWorkflowInstanceName method");
            //collection of WorkflowInstanceIdVSWorkflowInstanceName
            Dictionary<int, string> workflowInstanceIdVsWorkflowInstanceName = new Dictionary<int, string>();
            foreach (DataRow row in ds1.Tables[1].Rows)
            {
                int workflowInstanceId = Convert.ToInt32(row["workflow_instance_id"]);
                string workflowInstanceName = Convert.ToString(row["workflow_instance_name"]);
                if (!workflowInstanceIdVsWorkflowInstanceName.ContainsKey(workflowInstanceId))
                    workflowInstanceIdVsWorkflowInstanceName.Add(workflowInstanceId, workflowInstanceName);
            }
            mLogger.Debug("End->  WorkflowInstanceIdVsWorkflowInstanceName method");
            return workflowInstanceIdVsWorkflowInstanceName;
        }

        private static DataSet PrepareDataSetCollection(DataTable instrumentIdsTable, bool isInitiate, List<WorkflowActionInfo> actionInfo, string userName, string guid, RDBConnectionManager conMgr, DateTime? timeStamp = null)
        {
            mLogger.Debug("Prepare Data Set Collection-->Start");
            DataSet ds1 = null;

            string instrumentIdsTableName = "IVPRefMaster.dbo.[instrumentIdsTable_" + guid + "]";
            CommonDALWrapper.ExecuteQuery(string.Format(@"CREATE TABLE {0}(InstrumentId varchar(max),RADWorkflowInstanceId int)", instrumentIdsTableName), CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
            CommonDALWrapper.ExecuteBulkUpload(instrumentIdsTableName, instrumentIdsTable, ConnectionConstants.RefMaster_Connection);
            try
            {
                if (!isInitiate)
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select rq.workflow_instance_id,wr.priority,rq.rad_workflow_instance_id,rq.requested_on, rq.requested_by,rq.type_id from IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq inner join " + instrumentIdsTableName + " ins on rq.rad_workflow_instance_id = ins.RADWorkflowInstanceId inner join IVPRefMaster.dbo.ivp_srm_workflow_rules wr on rq.rule_mapping_id = wr.rule_mapping_id");
                    query.Append("select distinct workflow_instance_id,workflow_instance_name from IVPRefMaster.dbo.ivp_srm_workflow_instance  where is_active=1");
                    if (conMgr == null)
                        ds1 = CommonDALWrapper.ExecuteSelectQuery(query.ToString(), ConnectionConstants.RefMaster_Connection);
                    else
                        ds1 = CommonDALWrapper.ExecuteSelectQuery(query.ToString(), conMgr);

                }
                else
                {
                    DataTable workflowDetailsTable = new DataTable();
                    workflowDetailsTable.Columns.Add("workflow_instance_id", typeof(int));
                    workflowDetailsTable.Columns.Add("type_id", typeof(int));
                    workflowDetailsTable.Columns.Add("instrument_id", typeof(string));
                    workflowDetailsTable.Columns.Add("requested_by", typeof(string));
                    workflowDetailsTable.Columns.Add("requested_on", typeof(DateTime));
                    workflowDetailsTable.Columns.Add("rad_workflow_instance_id", typeof(int));
                    workflowDetailsTable.Columns.Add("rule_mapping_id", typeof(int));

                    foreach (var info in actionInfo)
                    {
                        var row = workflowDetailsTable.NewRow();
                        row["workflow_instance_id"] = info.WorkflowInstanceID;
                        row["type_id"] = info.TypeID;
                        row["instrument_id"] = info.InstrumentID;
                        row["requested_by"] = userName;
                        row["requested_on"] = timeStamp;
                        row["rad_workflow_instance_id"] = info.RadInstanceID;
                        row["rule_mapping_id"] = info.RuleMappingID;
                        workflowDetailsTable.Rows.Add(row);
                    }

                    string workflowDetailsTableName = "IVPRefMaster.dbo.[workflowDetailsTable_" + guid + "]";
                    CommonDALWrapper.ExecuteQuery(string.Format(@"CREATE TABLE {0}(workflow_instance_id int, type_id int, instrument_id varchar(max), requested_by varchar(max), requested_on datetime, rad_workflow_instance_id int, rule_mapping_id int)", workflowDetailsTableName), CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
                    CommonDALWrapper.ExecuteBulkUpload(workflowDetailsTableName, workflowDetailsTable, ConnectionConstants.RefMaster_Connection);

                    ds1 = CommonDALWrapper.ExecuteSelectQuery(DALWrapperAppend.Replace(string.Format(@"select rq.workflow_instance_id,wr.priority,rq.rad_workflow_instance_id,rq.requested_on, rq.requested_by,rq.type_id from {0} rq inner join IVPRefMaster.dbo.ivp_srm_workflow_rules wr on rq.rule_mapping_id = wr.rule_mapping_id

                      select distinct workflow_instance_id,workflow_instance_name from IVPRefMaster.dbo.ivp_srm_workflow_instance  where is_active=1 ", workflowDetailsTableName)), ConnectionConstants.RefMaster_Connection);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                string query = @"IF (OBJECT_ID('" + instrumentIdsTableName + "') IS NOT NULL) DROP TABLE " + instrumentIdsTableName;
                if (conMgr == null)
                    CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Delete, ConnectionConstants.RefMaster_Connection);
                else
                    CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Delete, conMgr);
            }
            mLogger.Debug("Prepare Data Set Collection-->End");
            return ds1;
        }

        private static List<AttributeVsValue> GetAttributesAndValues(int moduleId, List<int> RadWorkflowInstanceIdForFinalApproval, List<int> RadWorkflowInstanceId, string workflowDetailsTableName, string dateFormat, string dateTimeFormat, RDBConnectionManager conMgr, bool isInitiate)
        {
            mLogger.Debug("GetAttributesAndValues method --> Start");
            List<AttributeVsValue> attrVsValue = new List<AttributeVsValue>();
            List<AttributeVsValue> attrVsValueCombined = new List<AttributeVsValue>();
            try
            {
                DataSet dsForFinalStage = new DataSet();
                DataSet dsForIntermediateStage = new DataSet();

                if (moduleId == 3)
                {
                    XmlDocument xDocInstruments = new XmlDocument();
                    if (RadWorkflowInstanceIdForFinalApproval.Count > 0)
                    {
                        XElement xmlInstruments = new XElement("instances", RadWorkflowInstanceIdForFinalApproval.Select(x => new XElement("instance", new XAttribute("id", x))));
                        xDocInstruments.Load(xmlInstruments.CreateReader());
                        string query = @"EXEC IVPSecMaster.dbo.SECM_GetWorkflowAttributesToShow '" + xDocInstruments.InnerXml + "'," + 1 + ",'" + workflowDetailsTableName + "'";
                        if (conMgr == null)
                            dsForFinalStage = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                        else
                            dsForFinalStage = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                    }
                    else if (RadWorkflowInstanceId.Count > 0)
                    {
                        XElement xmlInstruments = new XElement("instances", RadWorkflowInstanceId.Select(x => new XElement("instance", new XAttribute("id", x))));
                        xDocInstruments.Load(xmlInstruments.CreateReader());
                        string query = @"EXEC IVPSecMaster.dbo.SECM_GetWorkflowAttributesToShow '" + xDocInstruments.InnerXml + "'," + 0 + ",'" + workflowDetailsTableName + "'";
                        if (conMgr == null)
                            dsForIntermediateStage = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                        else
                            dsForIntermediateStage = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                    }
                    attrVsValue = PrepareAttributeVsValueCollection(attrVsValue, dsForIntermediateStage, dateFormat, moduleId, dateTimeFormat);
                    attrVsValueCombined = PrepareAttributeVsValueCollection(attrVsValueCombined, dsForFinalStage, dateFormat, moduleId, dateTimeFormat);
                }
                else
                {

                    if (RadWorkflowInstanceIdForFinalApproval.Count > 0)
                    {
                        string RadWorkflowInstanceIds = "";
                        if (RadWorkflowInstanceIdForFinalApproval != null && RadWorkflowInstanceIdForFinalApproval.Count > 0)
                            RadWorkflowInstanceIds = string.Join(",", RadWorkflowInstanceIdForFinalApproval);

                        string query = @"EXEC IVPRefMaster.dbo.REFM_GetAttributeValuesForWorkflow '" + RadWorkflowInstanceIds + "', " + 1 + ", '" + workflowDetailsTableName + "'";

                        if (conMgr == null)
                            dsForFinalStage = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                        else
                            dsForFinalStage = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                    }
                    else if (RadWorkflowInstanceId.Count > 0)
                    {
                        string RadWorkflowInstanceIds = "";
                        if (RadWorkflowInstanceId != null && RadWorkflowInstanceId.Count > 0)
                            RadWorkflowInstanceIds = string.Join(",", RadWorkflowInstanceId);

                        string query = @"EXEC IVPRefMaster.dbo.REFM_GetAttributeValuesForWorkflow '" + RadWorkflowInstanceIds + "', " + 0 + ", '" + workflowDetailsTableName + "'";

                        if (conMgr == null)
                            dsForIntermediateStage = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                        else
                            dsForIntermediateStage = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                    }
                    attrVsValue = PrepareAttributeVsValueCollection(attrVsValue, dsForIntermediateStage, dateFormat, moduleId, dateTimeFormat);
                    attrVsValueCombined = PrepareAttributeVsValueCollection(attrVsValueCombined, dsForFinalStage, dateFormat, moduleId, dateTimeFormat);
                }
                attrVsValueCombined.AddRange(attrVsValue);

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                string query = @"IF (OBJECT_ID('" + workflowDetailsTableName + "') IS NOT NULL) DROP TABLE " + workflowDetailsTableName;
                if (conMgr == null)
                    CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Delete, ConnectionConstants.RefMaster_Connection);
                else
                    CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Delete, conMgr);
            }
            mLogger.Debug(" GetAttributesAndValues method --> End");
            return attrVsValueCombined;
        }

        private static Dictionary<int,string> GetUsersDetailsFromRAD(Dictionary<int, KeyValuePair<int, string>> RADWorkflowInstanceIdVsWorkflowIdVsCurrentState = null, Dictionary<int, Dictionary<string, HashSet<string>>> workflowIdVsStateNameVsListOfMappedGroups = null)
        {
            mLogger.Debug("GetUsersDetailsFromRAD method --> Start");
            RUserManagementService group = new RUserManagementService();
            Dictionary<int, string> RADWorkflowInstanceIdVsRADUsers = new Dictionary<int, string>();
            try
            {
                foreach (var rwid in RADWorkflowInstanceIdVsWorkflowIdVsCurrentState.Keys)
                {
                    StringBuilder usersInfoFromRAD = new StringBuilder();
                    HashSet<string> uIFRAD = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    KeyValuePair<int, string> WorkflowIdVsCurrentState = RADWorkflowInstanceIdVsWorkflowIdVsCurrentState[rwid];
                    int WorkflowId = WorkflowIdVsCurrentState.Key;
                    string currentState = WorkflowIdVsCurrentState.Value;
                    if (workflowIdVsStateNameVsListOfMappedGroups.ContainsKey(WorkflowId) && workflowIdVsStateNameVsListOfMappedGroups[WorkflowId].ContainsKey(currentState))
                    {
                        HashSet<string> ListOfMappedGroups = workflowIdVsStateNameVsListOfMappedGroups[WorkflowId][currentState];
                        foreach (string groupName in ListOfMappedGroups)
                        {
                            List<RUserInfo> result = group.GetGroupDetailGDPR(groupName);
                            foreach (RUserInfo r in result)
                            {
                                if (!string.IsNullOrEmpty(r.EmailId))
                                    uIFRAD.Add(r.EmailId);
                            }
                        }
                        if (usersInfoFromRAD.Length > 0)
                            usersInfoFromRAD.Append(",");
                        foreach (string id in uIFRAD)
                        {
                            usersInfoFromRAD.Append(id);
                            usersInfoFromRAD.Append(",");
                        }
                        if (usersInfoFromRAD.Length > 0 && usersInfoFromRAD.ToString().EndsWith(","))
                        {
                            usersInfoFromRAD.Length--;
                        }
                        if (!RADWorkflowInstanceIdVsRADUsers.ContainsKey(rwid))
                            RADWorkflowInstanceIdVsRADUsers.Add(rwid, usersInfoFromRAD.ToString());
                    }
                }
 
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                mLogger.Debug("GetUsersDetailsFromRAD method --> End");
            }
            return RADWorkflowInstanceIdVsRADUsers;
        }

        struct PriorityAndRequestedOnAndRequestedBy
        {
            public int priority;
            public DateTime requestedOn;
            public string requestedBy;
            public int typeID;
        }

        public class InstrumentIdAndRadInstanceId
        {
            public string InstrumentId { get; set; }
            public int RadInstanceId { get; set; }
        }
        public class AttributeNameVsAttributeValueVsIsPrimary
        {
            public string AttributeName { get; set; }
            public string AttributeValue { get; set; }
            public bool IsPrimary { get; set; }
        }
    }
}
