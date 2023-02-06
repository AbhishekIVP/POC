using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using com.ivp.common;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using com.ivp.refmaster.common;
using com.ivp.rad.controls.xruleeditor.grammar;
using SRMModelerController;
using SRMCommonModules;
//using static CommonService.CommonService;
//using static com.ivp.srmcommon.SRMCommonInfo;
using com.ivp.common.srmdownstreamcontroller;
using com.ivp.SRMCommonModules;
using com.ivp.srmcommon;
using com.ivp.common.srmdwhjob;
using System.Data;

namespace CommonService
{
    [ServiceContract(Namespace = "CommonService", Name = "CommonService")]
    public interface ICommonService
    {
        [OperationContract]
        ExternalSystemInfo GetExternalSystemInfo(ExternalSystemInputInfo inputObject);

        [OperationContract]
        void PostToDownstream(DownstreamPostInputInfo inputObject);

        [OperationContract]
        List<GridData> GetGridData(WorkflowGridInput inputObject);

        [OperationContract]
        GridData GetLegData(int queueId, string userName);

        [OperationContract]
        List<string> GetCheckedRowKeys(string cacheKey, int gridType);

        [OperationContract]
        WorkflowImpactedSecuritiesResponse WorkflowImpactedSecuritiesCheck(List<string> rowKeys, string userName, string remarks, int action, string moduleName);

        [OperationContract]
        WorkflowHandlerResponseInfo WorkflowRequestHandler(List<int> statusIds, string userName, string remarks, int action, string guid);

        [OperationContract]
        Dictionary<WorkflowHandlerResponseInfo, List<RMFinalApprovalEntity>> RMWorkflowRequestHandler(List<int> statusIds, string userName, string remarks, int action, string guid);

        [OperationContract]
        ResetGridOutput ResetGridData(WorkflowGridResetterInput inputObject);

        [OperationContract]
        void ClearGridDataForUser(string userSessionIdentifier);

        [OperationContract]
        ViewLogOutput GetWorkflowRequestLog(int queueId, bool getPending, string moduleName);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        RADXRuleGrammarInfo PrepareRuleGrammarInfo(string type_ids, int moduleId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<ListItems> GetAllSectypes(List<string> selectedSectypes);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<ListItems> GetAllEntityTypes(List<string> selectedEntityTypes);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkFlowAttributeInfo> GetAllAttributesBasedOnSectypeSelection(string secTypeIds, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkFlowAttributeInfo> GetAttributeBasedOnEntityTypeSelection(string entityTypeId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkFlowAttributeInfo> GetLegBasedOnEntityTypeSelection(string entityTypeId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkflowUsersGroups> GetAllUsersList();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkflowUsersGroups> GetAllGroupsList();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SMSaveWorkflow(WorkflowInfo info, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string RMSaveWorkflow(WorkflowInfo info, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkflowInfo> SMGetAllWorkflows(bool setRefWorkflow);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SMUpdateWorkflow(WorkflowInfo info, int instanceId, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string RMUpdateWorkflow(WorkflowInfo info, int instanceId, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool RemoveKey(string guid);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<ListItems> RMGetWorkflowType();


        //trying service for DQM charts
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        DqmChartData GetChartData();

        ////DQM Chain Data
        //[OperationContract]
        //[WebInvoke(Method = "POST",
        //   ResponseFormat = WebMessageFormat.Json,
        //   BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        //List<DqmChainData> GetChainData();

        [OperationContract]
        [WebInvoke(Method = "POST",
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        VendorManagementDataSourceType GetVendorManagementData(int preferenceId, int vendorId);

        [OperationContract]
        [WebInvoke(Method = "POST",
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        VendorManagementDataSourceType SaveVendorManagementData(VendorManagementInputInfo inputData);

        [OperationContract]
        [WebInvoke(Method = "POST",
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        VendorManagementDataSourceType DeleteVendorManagementData(VendorManagementDeleteData inputData);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkflowInfo> RMGetAllWorkflows();

        #region Overrides
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetOverridesDataSM(string username, string divID, string currPageID, string viewKey, string sessionID, string dateFormat);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        OverrideStatusRowInfo OverrideStatusGetCheckedRowsData(string cacheKey);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetOverridesDataRM(string username, string divID, string currPageID, string viewKey, string sessionID, string dateFormat, int ModuleId, string dateFormatLong);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        SMSearchOverrideInfo GetOverridesSecutiyDataSM(string username, string divID, string currPageID, string viewKey, string sessionID, List<string> selectedSecIds, string dateFormat);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        SMSearchOverrideInfo GetOverridesEntityDataRM(string username, string divID, string currPageID, string viewKey, string sessionID, List<string> selectedEntityIds, int entityTypeID, string dateFormat);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SaveBulkOverrideSM(string username, string uniqueId, Dictionary<string, List<SMOverrideAttributesInfo>> attrInfo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SaveBulkOverrideRM(string username, List<RMEntityOverrideInfo> attrInfo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SMDeleteOverride(Dictionary<int, string> deleteInfo);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string RMDeleteOverride(string username, List<RMEntityOverrideInfo> attrInfo);
        #endregion Overrides

        #region QuantModule
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SRMQuantFilterInfo> GetQuantIntellisenseDataSM();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void GetQuantData(string username, string encodedQuery, char tokenSeparator, char paramSeparator, char functionStart, char functionEnd, char valueIdentifier, string divId, string curPageId, string viewKey, string sessionId, char querySeparator);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SRMQuantSavedSearches> GetQuantSavedSearches(string userName);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        SRMQuantResponse InsertUpdateQuantSavedSearch(string searchID, string userName, string searchName, string searchQuery, string searchEncodedQuery);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        SRMQuantResponse DeleteQuantSavedSearch(string searchID, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ScreenerDataInfo GetSecurityData(string secName);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SRMQuantFilterInfo> GetReferenceData(string attributeName);
        #endregion

        #region ApiMonitoring

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<ApiMonitoringApiCallsOutputInfoItem> GetApiCallsData(ApiMonitoringApiCallsInputInfo InputObject);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ApiMonitoringFiltersOutput GetFiltersData();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ApiMonitoringFileContentOutput GetFileContent(ApiMonitoringFileContentInput InputObject);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ApiMonitoringApiCallLogOutput GetApiCallLogData(ApiMonitoringApiCallLogInput InputObject);
        #endregion

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool CheckControlPrivilegeForUser(string privilegeName, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<string> OverrideDataGetCheckedRowsData(string cacheKey, string productName);


        #region UniquenessSetup

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SectypeInfo> UniquenessSetupGetAllSectypes();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<UniquenessSetupKeyInfo> GetUniqueKeysForSelectedSectypes(List<int> selectedSectypes);

        //[OperationContract]
        //[WebInvoke(Method = "POST",
        //ResponseFormat = WebMessageFormat.Json,
        //BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        //List<UniquenessSetupKeyInfo> SearchUniqueKeys(List<int> selectedSectypes, string searchString);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<UniquenessSetupLegInfo> GetCommonLegsForSelectedSectypes(List<int> selectedSectypes);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        UniquenessSetupCommonMasterAttributesOutputInfo GetCommonMasterAttributesForSelectedSectypes(string userName, List<int> selectedSectypes, int KeyID);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        UniquenessSetupCommonLegAttributesOutputInfo GetCommonLegAttributesForSelectedLegName(UniquenessSetupLegInfo InputObject, int KeyID);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        UniquenessSetupOutputObject CreateUniqueKey(string userName, UniquenessSetupKeyInfo InputObject);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        UniquenessSetupOutputObject UpdateUniqueKey(string userName, UniquenessSetupKeyInfo InputObject);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int DeleteUniqueKey(string userName, int keyID);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        UniquenessSetupFileDownloadInfo DownloadAllUniqueKeys();
        #endregion

        #region CASourcePrioritization

        //[OperationContract]
        //[WebInvoke(Method = "POST",
        //ResponseFormat = WebMessageFormat.Json,
        //BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        //List<CASP_VendorData> CASP_GetAllVendorsForCorpActionType(int corpActionTypeID);

        //[OperationContract]
        //[WebInvoke(Method = "POST",
        //ResponseFormat = WebMessageFormat.Json,
        //BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        //List<CASP_TabInfo> CASP_GetAttributesDataForCorpActionType(int corpActionTypeID, int vendorsCount);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<CASPM_CorpActionType> CASPM_GetCorpActionTypesData();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        CASP_GetDataOutputObject CASP_GetDataForCorpActionType(int corpActionTypeID);

        #endregion

        #region Drafts
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetDraftsDataSM(string username, string divID, string currPageID, string viewKey, string sessionID, string dateFormat, string jsonGridInfo, bool isFromDashboard, string sectypeList, bool isUserSpecific);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetDraftsDataRM(string username, string divID, string currPageID, string viewKey, string sessionID, string dateFormat, int ModuleId, string jsonGridInfo);


        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string DraftsStatusGetCheckedRowsData(string cacheKey);


        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SMDeleteDrafts(List<string> deleteInfo, string username);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string RMDeleteDrafts(List<string> deleteInfo, string username);

        #endregion Drafts

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<CommonModuleTypeDetails> GetCommonModuleTypeDetails(string userName, string pageIdentifier, string baseModule, string currentModule);

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        SMDownstreamStatusInfo GetDownstreamStatusInfo(string productName, string userName, bool isTempFilterApplied, int moduleId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        DataSourceAndSystemMappingInfo GetDataSourceAndSystemMappingInfo(int type_id, int module_id);

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetDataSourceTable_Slider(int type_id, int data_source_id, int module_id, int template_id);

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetReportData(int type_id, int report_system_id, int module_id, int report_type_id, int template_id);

        [OperationContract]
        SRMRuleCatalogFilterInfo GetRuleCatalogFilterInfo(string userName, string moduleName);

        [OperationContract]
        string GetRules(int module, int tab, string ruleType, int typeId, string userName);


        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]

        Dictionary<int, string> GetRuleCatalogFilterDataForModuleID(int ModuleID, string UserName);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetRulesForModuleIDAndTabID(int ModuleID, int TabID, string UserName, List<int> TypeIDs);

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetLandingPageInfo(int moduleId);

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SRMWorkflowCountPerType> GetWorkFlowInboxData(int moduleId, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkflowQueueData> GetWorkFlowQueueData(int moduleId, string userName, string workflowType, string statusType, string dateFormat);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SRMWorkflowRequestsCounts> GetWorkflowCounts(SRMWorkflowRequestsCountInfo info, List<int> typeIdsFilter);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkflowActionHistoryPerRequest> GetWorkflowActionHistoryByInstance(int instanceID, int moduleID, string dateFormat, string dateTimeFormat, string timeFormat, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkflowActionHistoryPerRequest> GetWorkflowActionHistoryByInstrument(string instrumentID, int moduleID, string dateFormat, string dateTimeFormat, string timeFormat, string userName);

        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkflowActionInfo> TakeAction(List<WorkflowActionInfo> actionInfo, string actionName, string userName, string remarks, int moduleID, bool performFinalApproval = true);


        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool ReTriggerWorkflow(List<WFRetriggerInfo> inputInfo, string userName, string remarks);

        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkflowActionInfo> CheckActionOutput(List<WorkflowActionInfo> actionInfo, string actionName, string userName, string remarks, int moduleID);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<WorkflowActionHistory> GetWorkflowActionHistory(int moduleId, int radWorkflowInstanceId);


        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string ConstituentRight(int secTypeId);


        [OperationContract]
        [WebInvoke(Method = "POST",
      ResponseFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string Derivatives(int secTypeId);

        [OperationContract]
        [WebInvoke(Method = "POST",
      ResponseFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string DerivativesLeft(int secTypeId);

        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string ConstituentRightMost(int secTypeId);

        [OperationContract]
        [WebInvoke(Method = "POST",
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SecTypeNameText(int secTypeId);


        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetRoleData(int typeId);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetTemplateDetails(int templateId, int moduleId);

        [OperationContract]
        [WebInvoke(Method = "POST",
   ResponseFormat = WebMessageFormat.Json,
   BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SRMTemplateMaster> GetTemplateInfo(int typeId, int moduleId);


        [OperationContract]
        [WebInvoke(Method = "POST",
   ResponseFormat = WebMessageFormat.Json,
   BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<string> GetAllowedUsers(int typeId, int moduleId);


        [OperationContract]
        [WebInvoke(Method = "POST",
   ResponseFormat = WebMessageFormat.Json,
   BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<string> GetAllowedGroups(int typeId, int moduleId);

        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SRMAccess> GetRuleAccessDetails(int typeId, int moduleId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string BindGridForActionInfo(string username, string divID, string currPageID, string viewKey, string sessionID, string dateFormat, int ModuleId, string jsonGridInfo, List<WorkflowActionInfo> actionInfo);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        DBLocksInfo GetDBLocksData(string divID, string currPageID, string viewKey, string sessionID, string jsonGridInfo, string startDate, string endDate);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<MigrationFeaturesInfoUI> GetMigrationUtilityModulesList(string username, int moduleId);


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetSelectableItemsForMigration(int moduleID, int feature, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string DownloadMigrationConfiguration(List<int> features, List<object> selectedItems, int moduleID, string userName, bool isExcelFile);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]


        string UploadMigrationConfiguration(List<int> features, bool isSync, bool requireMissing, int moduleID, string userName, string dateFormat, string dateTimeFormat, string timeFormat, string filePath, bool isExcel, bool isBulkUpload);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        DownloadViewConfiguration DownloadViewConfigurationForDWH(string systemName);


        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<PrivilegeInfo> CheckControlPrivilegeForUserMultiple(List<string> privilegeName, string userName);

        [OperationContract]
        Dictionary<string, ProcessInfo> FetchTaskManagerValues();
        [OperationContract]
        string OnRestartService(string serviceName);
        [OperationContract]
        string onStartStopService(string serviceName, string existingStatus);
        [OperationContract]
        Dictionary<string, servicesInfo> FetchSystemServicesValues();
        [OperationContract]
        string stopAppPool();
        [OperationContract]
        string startAppPool();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServerFormat GetServerFormat();

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetClientDateFromServerDate(string date, string format);
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        SRFPMAttributeMetaDataOutputObject GetAttributeMetaData(SRFPMAttributeMetaDataInputObject InputObject);
        #region ReportViewer
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SecM_GetReportConfig(int reportSetupID, string loginName);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetReportLevel();
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetReportType(int selectedReportLevelID, int moduleID);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetReportNamesForReportId(int selectedReportLevelID, int selectedReportID, string loginName, int moduleID);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string Refm_GetFeedBrowserReport_Instances(int reportID, string longDateFormat);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string Refm_GetAuditHistory_Dates(int reportID, int ReportTypeId, string shortDateFormat);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string LoadReportRef(string ReportID_reportTypeID, int instanceID, string reportName, string knowledgeDate, string startDate, string EndDate, string shortLongDatePatterns);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string LoadReportSec(int ReportID, int ReportSetupID, string UserName, string EnvironmentCulture);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetReportHeader(int ReportSetupID, string moduleID, string reportName);

        #endregion

        #region SRMPolarisDownstreamConfig
        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetBlockTypes();

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SRMDownstreamSyncInfo> GetAllConfigDataInitial(string selectedSystemName);

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        SRMDownstreamSyncSetupDetails GetSelectedConnectionDetails(string connectionName);

        [OperationContract]
        [WebInvoke(Method = "POST",
      ResponseFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        DownstreamSchedulerInfo GetSchedulerData(string selectedSystemName);

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SRMDownstreamSyncSaveReports(SRMDownstreamSyncInfo Systems, bool IsNewSystem, DownstreamSchedulerInfo scheduleInfo, string dateFormat);

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void SRMDownstreamSyncTriggerReports(String SetupName);


        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<string> SRMDownstreamSyncGetExistingConnections();

        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string SRMDownstreamSyncAddNewSystem(SRMDownstreamSyncSetupDetails SetupDetails, string SystemName, string EffectiveDate, int CalendarType);
        #endregion

        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<DownstreamSyncStatusResult> GetAllDownstreamSyncStatus(string startDate, string endDate, string dateFormat, string resultDateTimeFormat, string selDateOption, string CustomRadioOption);


        [OperationContract]
        [WebInvoke(Method = "POST",
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<DownstreamSyncReportMessage> GetReportStatusMessage(List<int> blockStatusId, string resultDateTimeFormat);

        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ExceptionsConfig getExceptionConfigDetails(int moduleId, int instrumentTypeId);
        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string saveExceptionConfigDetails(List<ExceptionsConfig> configs, string user);

        #region Event Config
        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<SRMEventInstrumentTypesConfigList> GetEventInstrumentTypesConfigList(List<int> moduleIds);

        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void DeleteEventSettingForInstrumentType(int moduleId, int instrumentTypeId);

        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        SRMEventsAllLevelActions GetEventConfigForInstrumentType(int moduleId, int instrumentTypeId);
        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<string> getTransportNames();
        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void SaveEventInstrumentTypesConfigList(int moduleId, int instrumentTypeId, SRMEventsAllLevelActions config, string username);
        #endregion

        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string ExecuteQuery(string query);

        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<CommonService.VersionInfo> GetHotfixes(bool allDeployedVersions, int moduleId);

        [OperationContract]
        [WebInvoke(Method = "POST",
      ResponseFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetExpensiveQuery(string startDate, string endDate, string dateFormat, string resultDateTimeFormat);

        [OperationContract]
        [WebInvoke(Method = "POST",
      ResponseFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetDeadLockData(string startDate, string endDate, string dateFormat, string resultDateTimeFormat);

        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetDeadLockGraphData(string deadLockTime, string dateFormat);

        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetActivityMonitorDetails();

        [OperationContract]
        [WebInvoke(Method = "POST",
    ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetQueryTextData(long Id);

        [OperationContract]
        [WebInvoke(Method = "POST",
   ResponseFormat = WebMessageFormat.Json,
   BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string DownloadQueryPlanFile(long Id, string QueryPlan, string LastExecutionTime);

        [OperationContract]
        [WebInvoke(Method = "POST",
   ResponseFormat = WebMessageFormat.Json,
   BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string DownloadDeadLockXML(string DeadLockTimeStamp, string DeadLockXMLData);
        [OperationContract]
        [WebInvoke(Method = "POST",
 ResponseFormat = WebMessageFormat.Json,
 BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string GetUserGroupLayoutPriorityReport(string guid, string userName, string shortDateFormat, string gridDivId, int moduleId);


        [OperationContract]
        [WebInvoke(Method = "POST",
     ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void SaveUserScreenVisitInfo(string identifier, string url, string uniqueId, string uniqueText, string screenName,string userName);

    }
}
