using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using com.ivp.refmaster.common;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.ivp.common
{
    public class SRMWorkflowServiceController
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMWorkflowServiceController");
        public SRMWorkflowRequest GetWorkflowRequests(string UserName, int ModuleId)
        {
            mLogger.Debug("GetWorkflowRequests -> Start");
            SRMWorkflowRequest responseObj = new SRMWorkflowRequest();
            WorkflowsCountResponse countResponse = SRMWorkflowController.GetWorkflowsCountPerModule(UserName, ModuleId.ToString());
            responseObj.IsSuccess = countResponse.IsSuccess;
            responseObj.FailureReason = countResponse.FailureReason;
            responseObj.WorkflowRequests = new List<SRMWorkflowsRequests>();

            if (responseObj.IsSuccess)
            {
                foreach (var countsInfo in countResponse.WorkflowsCounts)
                {
                    SRMWorkflowsRequests requestsObj = new SRMWorkflowsRequests();
                    requestsObj.ModuleName = countsInfo.ModuleName;
                    requestsObj.WorkflowCountsPerState = new List<SRMWorkflowRequestStateData>();
                    foreach (var perStateInfo in countsInfo.WorkflowCountsPerState)
                    {
                        SRMWorkflowRequestStateData stateObj = new SRMWorkflowRequestStateData();
                        stateObj.WorkflowState = perStateInfo.WorkflowState;
                        stateObj.CountInfo = new List<SRMWorkflowTypeRequestInfo>();
                        foreach (var perStateCount in perStateInfo.CountInfo)
                        {
                            SRMWorkflowTypeRequestInfo obj = new SRMWorkflowTypeRequestInfo();
                            obj.WorkflowType = perStateCount.WorkflowType;
                            obj.Count = perStateCount.Count;
                            stateObj.CountInfo.Add(obj);
                        }
                        requestsObj.WorkflowCountsPerState.Add(stateObj);
                    }
                    responseObj.WorkflowRequests.Add(requestsObj);
                }
            }
            mLogger.Debug("GetWorkflowRequests -> End");
            return responseObj;
        }

        public SRMWorkflowRequestDetails GetWorkflowRequestsDetails(SRMWorkflowRequestDetailInfo requestInfo, int ModuleId)
        {
            mLogger.Debug("GetWorkflowRequestsDetails -> Start");
            SRMWorkflowRequestDetails responseInfo = new SRMWorkflowRequestDetails();
            List<SRMWorkflowRequestDetail> lstResponse = new List<SRMWorkflowRequestDetail>();
            List<WorkflowQueueData> result = new List<WorkflowQueueData>();
            List<string> errorMessages = new List<string>();
            try
            {
                WorkflowStatusType workflowState = WorkflowStatusType.MY_REQUESTS;

                #region Validations                
                if (string.IsNullOrEmpty(requestInfo.WorkflowState))
                    errorMessages.Add("WorkflowState cannot be blank");

                if (string.IsNullOrEmpty(requestInfo.WorkflowType))
                    errorMessages.Add("WorkflowType cannot be blank");

                if (string.IsNullOrEmpty(requestInfo.DateFormat))
                    errorMessages.Add("DateFormat cannot be blank");

                if (string.IsNullOrEmpty(requestInfo.DateTimeFormat))
                    errorMessages.Add("DateTimeFormat cannot be blank");

                if (string.IsNullOrEmpty(requestInfo.UserName))
                    errorMessages.Add("UserName cannot be blank");

                if (!string.IsNullOrEmpty(requestInfo.WorkflowType) && !requestInfo.WorkflowType.SRMEqualWithIgnoreCase("create") && !requestInfo.WorkflowType.SRMEqualWithIgnoreCase("update"))
                    errorMessages.Add("Invalid WorkflowState");

                if (!string.IsNullOrEmpty(requestInfo.WorkflowState) && !(Enum<WorkflowStatusType>.TryParse(requestInfo.WorkflowState, true, out workflowState)))
                    errorMessages.Add("Invalid WorkflowState");

                if (errorMessages.Count > 0)
                {
                    mLogger.Debug("GetWorkflowRequestsDetails -> Validation Error :" + string.Join(",", errorMessages));
                    SRMWorkflowRequestDetail responseObj = new SRMWorkflowRequestDetail();
                    lstResponse.Add(responseObj);
                    responseInfo.IsSuccess = false;
                    responseInfo.FailureReason = string.Join(",", errorMessages);
                    return responseInfo;
                }
                #endregion
                //requestInfo.UserName = RMCommonUtilsGDPR.GetUserLoginNameFromUserName(requestInfo.UserName);

                result = SRMWorkflowController.GetWorkflowQueueData(ModuleId, requestInfo.UserName, requestInfo.WorkflowType, workflowState.ToString(), requestInfo.DateFormat, requestInfo.DateTimeFormat);
                foreach (WorkflowQueueData info in result)
                {
                    SRMWorkflowRequestDetail responseObj = new SRMWorkflowRequestDetail();
                    responseObj.InstrumentId = info.InstrumentId;
                    responseObj.InstrumentName = info.TypeName;
                    responseObj.RequestedBy = info.RequestedBy;
                    responseObj.RequestedOn = info.RequestedOn.ToString(requestInfo.DateTimeFormat);
                    responseObj.EffectiveFrom = info.EffectiveStartDate.HasValue ? info.EffectiveStartDate.Value.ToString(requestInfo.DateFormat) : string.Empty;
                    if (info.PossibleActions.SRMContainsWithIgnoreCase("Edit and Approve"))
                    {
                        info.PossibleActions.Remove("Edit and Approve");
                        info.PossibleActions.Add("Approve");
                    }
                    responseObj.ActionsAvailable = (new HashSet<string>(info.PossibleActions)).ToList();
                    responseObj.PrimaryAttribute = new List<SMRWorkflowAttributeInfo>();
                    responseObj.SecondaryAttribute = new List<SMRWorkflowAttributeInfo>();
                    if (workflowState.GetDescription().SRMEqualWithIgnoreCase("Rejected Requests") || workflowState.GetDescription().SRMEqualWithIgnoreCase("My Requests"))
                        responseObj.ActionsAvailable = new List<string>() { "Cancel" };
                    foreach (var attrInfo in info.WorkflowAttributes)
                    {
                        if (attrInfo.isPrimary)
                        {
                            responseObj.PrimaryAttribute.Add(new SMRWorkflowAttributeInfo() { Name = attrInfo.AttributeName, Value = attrInfo.AttributeValue });
                        }
                        else
                        {
                            responseObj.SecondaryAttribute.Add(new SMRWorkflowAttributeInfo() { Name = attrInfo.AttributeName, Value = attrInfo.AttributeValue });
                        }
                    }
                    lstResponse.Add(responseObj);
                    responseInfo.IsSuccess = true;
                    responseInfo.SRMWorkflowRequestDetail = lstResponse;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowRequestsDetails -> Exception : " + ex);
                SRMWorkflowRequestDetail responseObj = new SRMWorkflowRequestDetail();
                lstResponse.Add(responseObj);
                responseInfo.IsSuccess = false;
                responseInfo.FailureReason = ex.Message;
                return responseInfo;
            }
            finally
            {
                mLogger.Debug("GetWorkflowRequestsDetails -> End");
            }
            return responseInfo;
        }

        public SRMWorkflowAction PerformWorkflowAction(SRMWorkflowActionInfo requestInfo, int ModuleId)
        {
            mLogger.Debug("PerformWorkflowAction -> Start");
            SRMWorkflowAction responseObj = new SRMWorkflowAction();
            List<WorkflowActionInfo> workflowActionInfo = new List<WorkflowActionInfo>();
            List<WorkflowActionInfo> actionInfo = new List<WorkflowActionInfo>();
            WorkflowActionInfo action = new WorkflowActionInfo();
            List<string> errorMessages = new List<string>();

            try
            {
                #region Validations
                if (string.IsNullOrEmpty(requestInfo.Action))
                    errorMessages.Add("Action cannot be blank");

                if (string.IsNullOrEmpty(requestInfo.InstrumentId))
                    errorMessages.Add("InstrumentId cannot be blank");

                if (string.IsNullOrEmpty(requestInfo.UserName))
                    errorMessages.Add("UserName cannot be blank");

                if (string.IsNullOrEmpty(requestInfo.Comment) && !string.IsNullOrEmpty(requestInfo.Action) && (requestInfo.Action.SRMEqualWithIgnoreCase("reject") ||
                        requestInfo.Action.SRMEqualWithIgnoreCase("cancel")))
                    errorMessages.Add("Comment cannot be blank");
                #endregion

                if (errorMessages.Count > 0)
                {
                    responseObj.Status = "FAILED";
                    responseObj.Message = string.Join(",", errorMessages);
                    return responseObj;
                }

                //requestInfo.UserName = RMCommonUtilsGDPR.GetUserLoginNameFromUserName(requestInfo.UserName);

                action.InstrumentID = requestInfo.InstrumentId;
                if (ModuleId != 3)
                {
                    DataSet ds = null;
                    string query = string.Format(" EXEC dbo.SRM_GetWorkflowDetailsForTakeAction @instrument_id = '{0}' , @module_id = {1}", action.InstrumentID, ModuleId);
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        action.QueueID = Convert.ToInt32(dr["queue_id"]);
                        action.RadInstanceID = Convert.ToInt32(dr["rad_workflow_instance_id"]);
                        action.WorkflowInstanceID = Convert.ToInt32(dr["workflow_instance_id"]);
                        action.TypeID = Convert.ToInt32(dr["entity_type_id"]);
                        action.TypeName = Convert.ToString(dr["entity_display_name"]);
                        action.RequestedOn = Convert.ToDateTime(dr["requested_on"]);
                        string workflow_type = Convert.ToString(dr["workflow_action_type_name"]);
                        if (workflow_type.StartsWith("Create"))
                            action.WorkflowType = WorkflowType.CREATE;
                        else
                            action.WorkflowType = WorkflowType.UPDATE;

                        actionInfo.Add(action);
                    }

                    if (requestInfo.Action == "Re-Trigger")
                        workflowActionInfo = SRMWorkflowController.ReTriggerWorkflow(actionInfo, requestInfo.UserName, requestInfo.Comment);
                    workflowActionInfo = SRMWorkflowController.TakeAction(actionInfo, requestInfo.Action, requestInfo.UserName, requestInfo.Comment, ModuleId, true).actionOutput;
                    if (workflowActionInfo[0].isPassed)
                    {
                        responseObj.Status = "Passed";
                    }

                    else
                    {
                        responseObj.Status = "Failed";
                        responseObj.Message = workflowActionInfo[0].ErrorMessage;
                    }
                }
                else
                {
                    responseObj.Status = "FAILED";
                    responseObj.Message = "Not Supported";
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("PerformWorkflowAction -> Exception : " + ex);
            }
            finally
            {
                mLogger.Debug("PerformWorkflowAction -> End");
            }
            return responseObj;
        }

        public SRMWorkflowActionHistoryResponseInfo GetWorkflowActionHistory(SRMWorkflowActionHistoryRequestInfo requestInfo, int ModuleId)
        {
            SRMWorkflowActionHistoryResponseInfo responseInfo = new SRMWorkflowActionHistoryResponseInfo();

            try
            {
                if (requestInfo != null)
                {
                    if (string.IsNullOrEmpty(requestInfo.DateFormat))
                    {
                        requestInfo.DateFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    }

                    if (string.IsNullOrEmpty(requestInfo.DateTimeFormat))
                    {
                        requestInfo.DateTimeFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern;
                    }

                    if (string.IsNullOrEmpty(requestInfo.TimeFormat))
                    {
                        requestInfo.TimeFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern;
                    }
                    //requestInfo.UserName = RMCommonUtilsGDPR.GetUserLoginNameFromUserName(requestInfo.UserName);
                    if (ModuleId != 3)
                    {
                        RMEntityTypeInfo mainEntityTypeInfo = GetEntityTypeInfoByName(requestInfo.TypeName, null, (Modules)ModuleId, InputType.DISPLAYNAME);
                        if (mainEntityTypeInfo != null && mainEntityTypeInfo.EntityTypeID > 0 && ModuleId == mainEntityTypeInfo.ModuleID)
                        {
                            if (!RMCommonUtils.CheckEntityTypeAccess(mainEntityTypeInfo.EntityTypeID, requestInfo.UserName))
                            {
                                responseInfo.ErrorMessage = "You dont have access to this entity type.";
                            }
                            else
                            {
                                if (requestInfo.InstrumentId != null && requestInfo.InstrumentId.Count > 0 && !string.IsNullOrEmpty(requestInfo.UserName))
                                {
                                    IEnumerable<string> distinctInstrumentIds = requestInfo.InstrumentId.Distinct();

                                    List<string> activeInstruments = GetValidEntitiesFromGivenList(distinctInstrumentIds, mainEntityTypeInfo.EntityTypeID, ModuleId, mainEntityTypeInfo.EntityTypeName);

                                    foreach (string instrumentId in distinctInstrumentIds)
                                    {
                                        if (activeInstruments.Contains(instrumentId))
                                        {
                                            try
                                            {
                                                List<WorkflowActionHistoryPerRequest> objInstrumentResponse = SRMWorkflowController.GetWorkflowActionHistoryByInstrument(instrumentId, ModuleId, requestInfo.DateFormat, requestInfo.DateTimeFormat, requestInfo.TimeFormat, requestInfo.UserName);

                                                List<SRMWorkflowActionHistoryInstanceInfo> apiInstrumentResponse = new List<SRMWorkflowActionHistoryInstanceInfo>();

                                                if (objInstrumentResponse != null && objInstrumentResponse.Count > 0)
                                                {
                                                    foreach (var instanceInfo in objInstrumentResponse)
                                                    {
                                                        SRMWorkflowActionHistoryInstanceInfo apiInstanceInfo = new SRMWorkflowActionHistoryInstanceInfo();

                                                        apiInstanceInfo.RequestedBy = instanceInfo.RequestedBy;
                                                        apiInstanceInfo.RequestedOn = instanceInfo.RequestedOn;

                                                        var pendingAction = instanceInfo.Actions.Where(x => string.IsNullOrEmpty(x.ActionDate)).FirstOrDefault();
                                                        if (pendingAction != null)
                                                        {
                                                            SRMWorkflowActionHistoryActionInfo apiActionInfo = new SRMWorkflowActionHistoryActionInfo();

                                                            apiActionInfo.ActionDate = pendingAction.ActionDate;
                                                            apiActionInfo.ActionTime = pendingAction.ActionTime;
                                                            apiActionInfo.ActionTakenBy = pendingAction.UserName;
                                                            apiActionInfo.Remarks = pendingAction.Remarks;
                                                            apiActionInfo.WorkflowAction = pendingAction.Status;
                                                            apiActionInfo.WorkflowStage = pendingAction.LevelName;

                                                            foreach (var attributeInfo in pendingAction.Attributes)
                                                            {
                                                                SRMWorkflowActionHistoryAttributeAuditInfo apiAttributeInfo = new SRMWorkflowActionHistoryAttributeAuditInfo();

                                                                apiAttributeInfo.AttributeName = attributeInfo.AttributeName;
                                                                apiAttributeInfo.KnowledgeDate = attributeInfo.KnowledgeDate;
                                                                apiAttributeInfo.PrimaryAttribute = attributeInfo.PrimaryAttribute;
                                                                apiAttributeInfo.LegName = attributeInfo.TypeName;
                                                                apiAttributeInfo.ModifiedBy = attributeInfo.UserName;
                                                                apiAttributeInfo.NewValue = attributeInfo.NewValue;
                                                                apiAttributeInfo.OldValue = attributeInfo.OldValue;

                                                                apiActionInfo.Attributes.Add(apiAttributeInfo);
                                                            }

                                                            apiInstanceInfo.Actions.Add(apiActionInfo);

                                                            instanceInfo.Actions.Remove(pendingAction);
                                                        }
                                                        //instanceInfo.Actions = instanceInfo.Actions.OrderByDescending(x => DateTime.ParseExact(x.ActionDate, requestInfo.DateFormat, null)).ThenByDescending(x=> TimeSpan.ParseExact(x.ActionTime,requestInfo.TimeFormat,null)).ToList();

                                                        instanceInfo.Actions = instanceInfo.Actions.OrderByDescending(x => Convert.ToDateTime(x.ActualActionTime)).ToList();


                                                        foreach (var actionInfo in instanceInfo.Actions)
                                                        {
                                                            SRMWorkflowActionHistoryActionInfo apiActionInfo = new SRMWorkflowActionHistoryActionInfo();

                                                            apiActionInfo.ActionDate = actionInfo.ActionDate;
                                                            apiActionInfo.ActionTime = actionInfo.ActionTime;
                                                            apiActionInfo.ActionTakenBy = actionInfo.UserName;
                                                            apiActionInfo.Remarks = actionInfo.Remarks;
                                                            apiActionInfo.WorkflowAction = actionInfo.Status;
                                                            apiActionInfo.WorkflowStage = actionInfo.LevelName;

                                                            foreach (var attributeInfo in actionInfo.Attributes)
                                                            {
                                                                SRMWorkflowActionHistoryAttributeAuditInfo apiAttributeInfo = new SRMWorkflowActionHistoryAttributeAuditInfo();

                                                                apiAttributeInfo.AttributeName = attributeInfo.AttributeName;
                                                                apiAttributeInfo.KnowledgeDate = attributeInfo.KnowledgeDate;
                                                                apiAttributeInfo.PrimaryAttribute = attributeInfo.PrimaryAttribute;
                                                                apiAttributeInfo.LegName = attributeInfo.TypeName;
                                                                apiAttributeInfo.ModifiedBy = attributeInfo.UserName;
                                                                apiAttributeInfo.NewValue = attributeInfo.NewValue;
                                                                apiAttributeInfo.OldValue = attributeInfo.OldValue;

                                                                apiActionInfo.Attributes.Add(apiAttributeInfo);
                                                            }

                                                            apiInstanceInfo.Actions.Add(apiActionInfo);
                                                        }

                                                        apiInstrumentResponse.Add(apiInstanceInfo);
                                                    }
                                                }

                                                responseInfo.InstrumentWorkflowInfo.Add(new SRMWorkflowActionHistoryInstrumentInfo() { InstrumentId = instrumentId, WorkflowInstances = apiInstrumentResponse, IsSuccess = true });
                                            }
                                            catch (Exception ex)
                                            {
                                                mLogger.Error("GetWorkflowActionHistory -> Exception : " + ex);
                                                responseInfo.InstrumentWorkflowInfo.Add(new SRMWorkflowActionHistoryInstrumentInfo() { InstrumentId = instrumentId, WorkflowInstances = null, IsSuccess = false, ErrorMessage = ex.Message.ToString() });
                                            }
                                        }
                                        else
                                        {
                                            responseInfo.InstrumentWorkflowInfo.Add(new SRMWorkflowActionHistoryInstrumentInfo() { InstrumentId = instrumentId, WorkflowInstances = null, IsSuccess = false, ErrorMessage = "Invalid Entity Code." });
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            responseInfo.ErrorMessage = "Entity Type Name Doesn't Exist";
                        }
                    }
                    else
                    {
                        responseInfo.ErrorMessage = "Not Supported";
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowActionHistory -> Exception : " + ex);
            }
            finally
            {
                mLogger.Debug("GetWorkflowActionHistory -> End");
            }
            return responseInfo;
        }

        private List<string> GetValidEntitiesFromGivenList(IEnumerable<string> instrumentsToCheck, int EntityTypeId, int ModuleId, string EntityTypeTable)
        {
            List<string> validInstruments = new List<string>();
            try
            {
                if (instrumentsToCheck != null && instrumentsToCheck.Count() > 0)
                {
                    DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"Select entity_code FROM IVPRefMaster.dbo." + EntityTypeTable + @" WHERE entity_code IN ('" + string.Join("','", instrumentsToCheck) + @"')

                                                                    Select instrument_id
                                                                    FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
                                                                    INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_instance wi
                                                                        ON rq.workflow_instance_id = wi.workflow_instance_id
                                                                    WHERE wi.is_active = 1 AND rq.type_id=" + EntityTypeId + @" AND wi.module_id = " + (int)ModuleId + @" AND instrument_id IN ('" + string.Join("','", instrumentsToCheck) + @"')", ConnectionConstants.RefMaster_Connection);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        validInstruments = ds.Tables[0].AsEnumerable().Select(x => Convert.ToString(x["entity_code"])).ToList();
                    }

                    if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        List<string> validInstrumentsInWF = ds.Tables[1].AsEnumerable().Select(x => Convert.ToString(x["instrument_id"])).ToList();

                        validInstruments = validInstruments.Union(validInstrumentsInWF).Distinct().ToList();
                    }
                }

                return validInstruments;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private RMEntityTypeInfo GetEntityTypeInfoByName(string entityTypeDisplayName, rad.dal.RDBConnectionManager cnMgr = null, Modules module = Modules.ALL, InputType inputType = InputType.DISPLAYNAME)
        {
            mLogger.Debug("GetEntityTypeInfoByName -> Start");
            RMEntityTypeInfo objEntityTypeInfo = null;
            try
            {
                Assembly RefmControllerAssembly = Assembly.Load("RefMController");
                Type objType = RefmControllerAssembly.GetType("com.ivp.refmaster.controller.RMEntityTypeController");
                object obj = Activator.CreateInstance(objType);
                MethodInfo methodInfo = objType.GetMethod("GetEntityTypeInfoByName");
                objEntityTypeInfo = (RMEntityTypeInfo)methodInfo.Invoke(obj, new object[] { entityTypeDisplayName, cnMgr, module, inputType });
            }
            catch (Exception ex)
            {
                mLogger.Error("GetEntityTypeInfoByName -> Exception : " + ex);
                throw;
            }
            finally
            {
                mLogger.Debug("GetEntityTypeInfoByName -> End");
            }
            return objEntityTypeInfo;
        }

        public SRMWorkflowInstument GetWorkflowInstrumentDetails(SRMWorkflowInstrumentInfo inputInfo)
        {
            mLogger.Debug("GetWorkflowInstrumentDetails -> Start");
            SRMWorkflowInstument responseObj = new SRMWorkflowInstument();
            responseObj.IsSuccess = true;
            responseObj.Legs = new List<SRMWorkflowInstrumentLegDetails>();
            responseObj.Tabs = new List<SRMWorkflowInstrumentTabDetails>();
            try
            {
                //inputInfo.UserName = RMCommonUtilsGDPR.GetUserLoginNameFromUserName(inputInfo.UserName);

                DataSet entityInfo = CommonDALWrapper.ExecuteSelectQuery(string.Format("SELECT entity_type_id,entity_type_name FROM dbo.ivp_refm_entity_type WHERE entity_code = '{0}'", inputInfo.InstrumentId.Substring(0, 4)), ConnectionConstants.RefMaster_Connection);

                int entityTypeId = Convert.ToInt32(entityInfo.Tables[0].Rows[0][0]);
                string entityTypeName = Convert.ToString(entityInfo.Tables[0].Rows[0][1]);

                int templateId = Convert.ToInt32(CommonDALWrapper.ExecuteSelectQuery(string.Format("SELECT dbo.REFM_GetUserTemplate({0},'{1}','Update')", entityTypeId, inputInfo.UserName), ConnectionConstants.RefMaster_Connection).Tables[0].Rows[0][0]);

                DataSet dsXMLData = FetchUIXMLData(entityTypeId, templateId);
                var dictETvsTabVsAttr = dsXMLData.Tables[2].AsEnumerable().GroupBy(x => Convert.ToString(x["entity_type_name"])).ToDictionary(x => x.Key,
                    y => y.GroupBy(z => Convert.ToString(z["entity_tab_name"])).ToDictionary(z => z.Key, p => p.ToList<DataRow>()));

                DataSet dsDBData = FetchDBData(inputInfo, entityTypeId, templateId);

                //List<SRMWorkflowInstrumentTabDetails> lstTabs = new List<SRMWorkflowInstrumentTabDetails>();
                //List<SRMWorkflowInstrumentLegDetails> lstLegs = new List<SRMWorkflowInstrumentLegDetails>();

                foreach (var etName in dictETvsTabVsAttr.Keys)
                {
                    if (etName.Equals(entityTypeName))
                    {
                        foreach (var tab in dictETvsTabVsAttr[etName].Keys)
                        {
                            SRMWorkflowInstrumentTabDetails tabInfo = new SRMWorkflowInstrumentTabDetails();
                            tabInfo.TabName = tab;
                            tabInfo.SubTabs = new List<SRMWorkflowInstrumentSubTabDetails>();
                            SRMWorkflowInstrumentSubTabDetails subTabInfo = new SRMWorkflowInstrumentSubTabDetails();
                            subTabInfo.SubTabName = tab;
                            subTabInfo.Attributes = new List<SMRWorkflowAttributeInfo>();

                            foreach (var attrRow in dictETvsTabVsAttr[etName][tab])
                            {
                                bool isVisible = Convert.ToBoolean(attrRow["is_visible"]);
                                if (isVisible)
                                {
                                    SMRWorkflowAttributeInfo attrInfo = new SMRWorkflowAttributeInfo();
                                    attrInfo.DataType = Convert.ToString(attrRow["data_type"]);
                                    attrInfo.Name = Convert.ToString(attrRow["display_name"]);
                                    attrInfo.Value = Convert.ToString(dsDBData.Tables[etName].Rows[0][Convert.ToString(attrRow["attribute_name"])]);
                                    if (attrInfo.DataType.SRMEqualWithIgnoreCase("datetime") && !string.IsNullOrEmpty(attrInfo.Value))
                                        attrInfo.Value = Convert.ToDateTime(dsDBData.Tables[etName].Rows[0][Convert.ToString(attrRow["attribute_name"])]).ToString(inputInfo.DateFormat);
                                    subTabInfo.Attributes.Add(attrInfo);
                                }
                            }
                            tabInfo.SubTabs.Add(subTabInfo);
                            responseObj.Tabs.Add(tabInfo);
                        }

                    }
                    else
                    {
                        //foreach (var leg in dictETvsTabVsAttr[etName].Keys)
                        //{
                        SRMWorkflowInstrumentLegDetails legInfo = new SRMWorkflowInstrumentLegDetails();
                        legInfo.LegRows = new List<SRMWorkflowInstrumentLegRowDetails>();
                        legInfo.LegName = Convert.ToString(dictETvsTabVsAttr[etName].Keys.FirstOrDefault());

                        foreach (var legRow in dsDBData.Tables[etName].AsEnumerable())
                        {
                            SRMWorkflowInstrumentLegRowDetails row = new SRMWorkflowInstrumentLegRowDetails();
                            row.LegRowId = Convert.ToString(legRow["entity_code"]);
                            row.Attributes = new List<SMRWorkflowAttributeInfo>();

                            foreach (var attrRow in dictETvsTabVsAttr[etName][legInfo.LegName])
                            {
                                SMRWorkflowAttributeInfo legAttrInfo = new SMRWorkflowAttributeInfo();
                                legAttrInfo.DataType = Convert.ToString(attrRow["data_type"]);
                                legAttrInfo.Name = Convert.ToString(attrRow["display_name"]);
                                legAttrInfo.Value = Convert.ToString(legRow[Convert.ToString(attrRow["attribute_name"])]);
                                if (legAttrInfo.DataType.SRMEqualWithIgnoreCase("datetime") && !string.IsNullOrEmpty(legAttrInfo.Value))
                                    legAttrInfo.Value = Convert.ToDateTime(legRow[Convert.ToString(attrRow["attribute_name"])]).ToString(inputInfo.DateFormat);

                                row.Attributes.Add(legAttrInfo);
                            }
                            legInfo.LegRows.Add(row);
                        }
                        responseObj.Legs.Add(legInfo);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Debug("GetWorkflowInstrumentDetails -> Exception :" + ex);
                responseObj.IsSuccess = false;
                responseObj.FailureReason = ex.Message;
            }
            finally
            {
                mLogger.Debug("GetWorkflowInstrumentDetails -> End");
            }
            return responseObj;
        }

        private static DataSet FetchDBData(SRMWorkflowInstrumentInfo inputInfo, int entityTypeId, int templateId)
        {
            RMEntityUpdationInfo objEntityUpdation = new RMEntityUpdationInfo();
            objEntityUpdation.entityTypeId = entityTypeId;
            objEntityUpdation.entityCode = inputInfo.InstrumentId;
            objEntityUpdation.templateId = templateId;

            Assembly RefmControllerAssembly = Assembly.Load("RefMController");
            Type objType = RefmControllerAssembly.GetType("com.ivp.refmaster.controller.RMEntityUpdationController");
            object obj = Activator.CreateInstance(objType);
            MethodInfo methodInfo = objType.GetMethod("GetEntityInfoForUpdationWithLookupMassaging");
            DataSet dsDBData = (DataSet)methodInfo.Invoke(obj, new object[] { objEntityUpdation, true, false, true });
            return dsDBData;
        }

        private static DataSet FetchUIXMLData(int entityTypeId, int templateId)
        {
            RHashlist mHList = new RHashlist();
            mHList.Add(RMTableRefmEntityType.ENTITY_TYPE_ID, entityTypeId);
            mHList.Add("template_id", templateId);
            mHList.Add(
                "dependantETList",
                new XElement(
                    "etIds",
                    from depEt in new List<int>()
                    select new XElement("etId", depEt)
                ).ToString()
            );
            mHList = RMDalWrapper.ExecuteProcedure(RMQueryConstantsXmlGeneration.REFM_GETUIXMLFROMDATA,
                mHList, RMDBConnectionEnum.RefMDBConnectionId);
            DataSet dsXML = (DataSet)mHList[0];
            return dsXML;
        }

        public SRMFileDataInfo GetWorkflowAttributeFileData(SRMFileDataRequestInfo requestInfo)
        {
            mLogger.Debug("GetWorkflowAttributeFileData -> Start");
            SRMFileDataInfo responseInfo = new SRMFileDataInfo();
            responseInfo.IsSuccess = true;
            string fileRealName = string.Empty;

            try
            {
                DataSet entityInfo = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT etyp.entity_type_id,entity_type_name,eat.attribute_name,eat.is_encrypted FROM dbo.ivp_refm_entity_type etyp
                        INNER JOIN dbo.ivp_refm_entity_attribute eat
                        ON(eat.entity_type_id = etyp.entity_type_id) WHERE entity_code = '{0}' AND eat.display_name = '{1}'", requestInfo.InstrumentId.Substring(0, 4), requestInfo.AttributeName), ConnectionConstants.RefMaster_Connection);

                int entityTypeId = Convert.ToInt32(entityInfo.Tables[0].Rows[0][0]);
                string entityTypeName = Convert.ToString(entityInfo.Tables[0].Rows[0][1]);
                string attributeName = Convert.ToString(entityInfo.Tables[0].Rows[0][2]);
                bool isEncrypted = Convert.ToBoolean(entityInfo.Tables[0].Rows[0][3]);

                string query = string.Empty;
                if (isEncrypted)
                    query = string.Format("EXEC IVPRefMaster.dbo.REFM_OpenEncryptionKey; SELECT dbo.REFM_Decrypt({1}) FROM IVPRefMaster.dbo.{2} WHERE entity_code = '{0}'", requestInfo.InstrumentId, attributeName, entityTypeName.Replace("ivp_refmd_", "ivp_refmw_"));
                else
                    query = string.Format("SELECT {1} FROM IVPRefMaster.dbo.{2} WHERE entity_code = '{0}'", requestInfo.InstrumentId, attributeName, entityTypeName.Replace("ivp_refmd_", "ivp_refmw_"));

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                fileRealName = Convert.ToString(ds.Tables[0].Rows[0][0]);

                if (!string.IsNullOrEmpty(fileRealName))
                {
                    Dictionary<string, RMFileDataInfo> dict = RMCommonUtils.GetFileInfoByRealName(new HashSet<string>() { fileRealName });
                    responseInfo.FileRealName = dict[fileRealName].FileRealName;
                    responseInfo.FileDisplayName = dict[fileRealName].FileDisplayName;
                    responseInfo.FileData = dict[fileRealName].FileData;
                }
            }
            catch(Exception ex)
            {
                mLogger.Debug("GetWorkflowAttributeFileData -> Exception :" + ex);
                responseInfo.IsSuccess = false;
                responseInfo.FailureReason = ex.Message;
            }
            finally
            {
                mLogger.Debug("GetWorkflowAttributeFileData -> End");
            }
            return responseInfo;
        }
    }
}