using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel.Activation;
using System.Data;
using com.ivp.commom.commondal;
using System.Web.Script.Serialization;
using com.ivp.rad.controls.neogrid.client.info;
using com.ivp.rad.controls.neogrid.service;
using com.ivp.common;
using com.ivp.rad.common;
using System.Drawing;
using System.Xml.Linq;
using System.Reflection;
using System.Web;
using com.ivp.refmaster.common;
using com.ivp.rad;
using com.ivp.rad.controls.neogrid;
using Newtonsoft.Json;
using com.ivp.rad.cryptography;
using com.ivp.rad.utils;
using com.ivo.rad.RGridWriterToolKit;
using System.IO;
using System.Globalization;
using com.ivp.rad.controls.xruleeditor.grammar;
using SRMModelerController;
using com.ivp.rad.RUserManagement;
using com.ivp.srmcommon;
using System.Management;
using System.Threading.Tasks;
using System.ServiceProcess;
using Microsoft.Web.Administration;
using com.ivp.rad.data;
using SRMCommonModules;
using com.ivp.rad.viewmanagement;
using com.ivp.rad.BusinessCalendar;
using com.ivp.common.srmdwhjob;
//using static com.ivp.srmcommon.SRMCommonInfo;
using com.ivp.common.srmdownstreamcontroller;
using com.ivp.SRMCommonModules;
using com.ivp.rad.dal;
using System.Runtime.Serialization.Json;
using System.Xml;

//User Anayltics
using com.ivp.rad.UserAnalytics;
using com.ivp.rad.configurationmanagement;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace CommonService
{
    [DataContract]
    public class RuleTypes
    {
        [DataMember]
        public int RuleId { get; set; }
        [DataMember]
        public string RuleName { get; set; }
    }

    [DataContract]
    public class EntityTypes
    {
        [DataMember]
        public int EntityTypeId { get; set; }
        [DataMember]
        public string EntityTypeName { get; set; }
    }

    [DataContract]
    public class SecurityTypes
    {
        [DataMember]
        public int SecurityTypeId { get; set; }
        [DataMember]
        public string SecurityTypeName { get; set; }
    }

    [DataContract]
    public class SRMRuleCatalogFilterInfo
    {
        [DataMember]
        public List<SecurityTypes> Security { get; set; }

        [DataMember]
        public List<RuleTypes> Rules { get; set; }

        [DataMember]
        public List<EntityTypes> Entity { get; set; }

    }

    [DataContract]
    public class SRMRuleCatalogOutput
    {
        [DataMember]
        public bool Status { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string SuccessMessage { get; set; }
        [DataMember]
        public bool DataValidated { get; set; }
        [DataMember]
        public string Guid { get; set; }
        [DataMember]
        public bool HasGrid { get; set; }
        [DataMember]
        public string gridID { get; set; }
        [DataMember]
        public string currentPageId { get; set; }
        [DataMember]
        public string viewKey { get; set; }
        [DataMember]
        public string sessionIdentifier { get; set; }
        [DataMember]
        public int rowCount { get; set; }
        [DataMember]
        public string customInfoScript { get; set; }
        [DataMember]
        public string FilePath { get; set; }
    }


    [DataContract]
    public class GridData
    {
        [DataMember]
        public string gridID { get; set; }
        [DataMember]
        public string currentPageId { get; set; }
        [DataMember]
        public string viewKey { get; set; }
        [DataMember]
        public string sessionIdentifier { get; set; }
        [DataMember]
        public string customInfoScript { get; set; }
        [DataMember]
        public string gridData { get; set; }
        [DataMember]
        public bool isMyRequestGrid { get; set; }
        [DataMember]
        public bool isAllRequestGrid { get; set; }
        [DataMember]
        public string usersGroups { get; set; }
        [DataMember]
        public bool isDataAvailable { get; set; }
        [DataMember]
        public int rowCount { get; set; }
    }

    [DataContract]
    public class ResetGridOutput
    {
        [DataMember]
        public string gridID { get; set; }
        [DataMember]
        public string currentPageId { get; set; }
        [DataMember]
        public string viewKey { get; set; }
        [DataMember]
        public string sessionIdentifier { get; set; }
        [DataMember]
        public bool status { get; set; }
    }

    [DataContract]
    public class ViewLogOutput
    {
        [DataMember]
        public string html { get; set; }
        [DataMember]
        public string htmlTop { get; set; }
        [DataMember]
        public bool isGridAvailable { get; set; }
    }

    [DataContract]
    public class ListItems
    {
        [DataMember]
        public string value { get; set; }
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string isSelected { get; set; }
    }

    [DataContract]
    public class WorkFlowAttributeInfo
    {
        [DataMember]
        public string attrName { get; set; }
        [DataMember]
        public int attrID { get; set; }
    }

    [DataContract]
    public class WorkflowInfo
    {
        [DataMember]
        public string workflowName { get; set; }
        [DataMember]
        public int workflowID { get; set; }
        [DataMember]
        public WorkflowData workflowData { get; set; }
        [DataMember]
        public string moduleType { get; set; }
        [DataMember]
        public int workflowType { get; set; }
    }

    [DataContract]
    public class WorkflowData
    {
        [DataMember]
        public string entityTypeID { get; set; }
        [DataMember]
        public string sectypeIDs { get; set; }
        [DataMember]
        public List<WorkFlowAttributeInfo> attributeInfo { get; set; }
        [DataMember]
        public List<WorkflowUsersGroups> usersInfo { get; set; }
        [DataMember]
        public List<WorkflowUsersGroups> groupInfo { get; set; }
        [DataMember]
        public bool applyTimeSeries { get; set; }
        [DataMember]
        public bool applyOnCreate { get; set; }
        [DataMember]
        public bool applyOnBlankToNonblank { get; set; }
    }

    [DataContract]
    public class WorkflowUsersGroups
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int level { get; set; }
    }

    [DataContract]
    public class OverrideStatusRowInfo
    {
        [DataMember]
        public string typeID { get; set; }
        [DataMember]
        public string attributeID { get; set; }
        [DataMember]
        public string ruleID { get; set; }
        [DataMember]
        public string tableName { get; set; }
        [DataMember]
        public string attributeDisplayName { get; set; }
        [DataMember]
        public string entityCode { get; set; }
    }

    [DataContract]
    public class SMSearchOverrideInfo
    {
        [DataMember]
        public string customRowInfo { get; set; }
        [DataMember]
        public List<AttributeInfo> attributeList { get; set; }
        [DataMember]
        public List<string> gridRowIds { get; set; }

    }

    [DataContract]
    public class AttributeInfo
    {
        [DataMember]
        public string attributeName { get; set; }
        [DataMember]
        public string attributeID { get; set; }
        [DataMember]
        public string attributeExpiry { get; set; }
    }

    [DataContract]
    public class ScreenerDataInfo
    {
        [DataMember]
        public Dictionary<string, string> securityData { get; set; }
        [DataMember]
        public string securityHistoricalLastPriceData { get; set; }
        [DataMember]
        public string securityHistoricalVolumneData { get; set; }
    }


    /// <summary>
    /// test data for graphs for dqm
    /// </summary>

    [DataContract]
    public class DqmChartDate
    {
        public DqmChartDate(int year, int month, int day)
        {
            this.year = year;
            this.month = month;
            this.day = day;
        }
        [DataMember]
        public int year { get; set; }
        [DataMember]
        public int month { get; set; }
        [DataMember]
        public int day { get; set; }
    }



    [DataContract]
    public class DqmChartSeriesData
    {
        [DataMember]
        public string chartName { get; set; }
        [DataMember]
        public Dictionary<DqmChartDate, float> chartData { get; set; }
        [DataMember]
        public string color { get; set; }

    }

    [DataContract]
    public class DqmChartData
    {
        // above chartseries data how?
        [DataMember]
        public int statusBit { get; set; }
        [DataMember]
        public DqmChartSeriesData seriesData { get; set; }
        [DataMember]
        public string info1 { get; set; }
        [DataMember]
        public string subInfo1 { get; set; }
    }

    /// <summary>
    /// end of
    /// test data for graphs for dqm
    /// </summary>
    /// 


    //service for data to display in Chains
    [DataContract]
    public class DqmChainData
    {
        [DataMember]
        public int statusBit;
        [DataMember]
        public string ChainName;
        [DataMember]
        public string ChainTime; //??
        [DataMember]
        public string ChainDate; //??
        [DataMember]
        public string ChainAvgDuration;
        [DataMember]
        public string ChainDuration;
        [DataMember]
        public string State;
        [DataMember]
        public int WarningCount;
        [DataMember]
        public List<DqmTaskDetails> TaskDetails;
    }

    [DataContract]
    public class DqmTaskDetails
    {
        public DqmTaskDetails(string TaskName, string TaskType, int duration, int ModuleId)
        {
            this.Duration = duration;
            this.TaskName = TaskName;
            this.TaskType = TaskType;
            this.ModuleId = ModuleId;
        }

        [DataMember]
        public string TaskName { get; set; }
        [DataMember]
        public string TaskType { get; set; }
        [DataMember]
        public int Duration { get; set; }     //write a data type that appends min to a integer probably
        [DataMember]
        public int ModuleId { get; set; }
    }


    [DataContract]
    public class ExternalSystemInfo
    {
        [DataMember]
        public List<string> ExternalSystemNameAndIds { get; set; }
        [DataMember]
        public bool PostToPrivilege { get; set; }
    }

    [DataContract]
    public class ExternalSystemInputInfo
    {
        [DataMember]
        public string selectedTab { get; set; }
        [DataMember]
        public string userName { get; set; }
        //[DataMember]
        //public string moduleId { get; set; }
    }

    [DataContract]
    public class DownstreamPostInputInfo
    {
        [DataMember]
        public string selectedTab { get; set; }
        [DataMember]
        public string userName { get; set; }
        [DataMember]
        public List<string> SecIds { get; set; }
        [DataMember]
        public List<int> systemIds { get; set; }
    }

    public class LeftMenuStructure
    {
        public List<LeftMenuItem> MenuItems { get; set; }
    }

    public class LeftMenuItem
    {
        public LeftMenuItem()
        {
            Children = new List<LeftMenuItem>();
        }
        public int MenuID { get; set; }
        public string Text { get; set; }
        public string Img { get; set; }
        public string Href { get; set; }
        public string Fragment { get; set; }
        public bool IgnorePriviledge { get; set; }
        public List<LeftMenuItem> Children { get; set; }
    }

    [DataContract]
    public class ApiMonitoringApiCallsInputInfo
    {
        [DataMember]
        public List<string> selectedUrl { get; set; }

        [DataMember]
        public List<string> selectedMethod { get; set; }

        [DataMember]
        public List<string> selectedClientMachine { get; set; }

        [DataMember]
        public string selectedStartDate { get; set; }

        [DataMember]
        public string selectedEndDate { get; set; }


    }

    [DataContract]
    public class ApiMonitoringApiCallsOutputInfoItem
    {
        public ApiMonitoringApiCallsOutputInfoItem()
        {
            this.ChainDetailsData = new ApiMonitoringApiCallDetailsData();
        }


        [DataMember]
        public string ApiUniqueId { get; set; }

        [DataMember]
        public string ChainName { get; set; }

        [DataMember]
        public string ChainURL { get; set; }

        [DataMember]
        public string ChainMethod { get; set; }

        [DataMember]
        public string ChainClientMachine { get; set; }

        [DataMember]
        public string ChainClientIP { get; set; }

        [DataMember]
        public int ChainPort { get; set; }

        [DataMember]
        public string ChainStartDateTime { get; set; }

        [DataMember]
        public string ChainEndDateTime { get; set; }

        [DataMember]
        public List<int> ChainTimeTaken { get; set; }

        [DataMember]
        public string ChainDetailsDataFormatRequest { get; set; }

        [DataMember]
        public string ChainDetailsDataFormatResponse { get; set; }

        [DataMember]
        public ApiMonitoringApiCallDetailsData ChainDetailsData { get; set; }

        [DataMember]
        public int ThreadId { get; set; }
    }

    [DataContract]
    public class ApiMonitoringApiCallDetailsData
    {
        [DataMember]
        public string RequestHeaderData { get; set; }

        [DataMember]
        public string RequestBodyDataFileLocation { get; set; }

        [DataMember]
        public string RequestBodyDataFileName { get; set; }

        //[DataMember]
        //public string RequestBodyDataFileContentType { get; set; }

        //[DataMember]
        //public bool RequestBodyDataToShow { get; set; }

        [DataMember]
        public string RequestBodyErrorMsg { get; set; }

        [DataMember]
        public string ResponseHeaderData { get; set; }

        [DataMember]
        public string ResponseBodyDataFileLocation { get; set; }

        [DataMember]
        public string ResponseBodyDataFileName { get; set; }

        //[DataMember]
        //public bool ResponseBodyDataToShow { get; set; }

        [DataMember]
        public string ResponseBodyErrorMsg { get; set; }
    }

    [DataContract]
    public class ApiMonitoringFilterOutputInfoItem
    {
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string value { get; set; }
    }

    [DataContract]
    public class ApiMonitoringFiltersOutput
    {
        [DataMember]
        public List<ApiMonitoringFilterOutputInfoItem> URLFiltersData { get; set; }
        [DataMember]
        public List<ApiMonitoringFilterOutputInfoItem> MethodFiltersData { get; set; }
        [DataMember]
        public List<ApiMonitoringFilterOutputInfoItem> ClientMachineFiltersData { get; set; }
    }

    [DataContract]
    public class ApiMonitoringFileContentInput
    {
        [DataMember]
        public string BodyFileLocation { get; set; }
        [DataMember]
        public string RequestOrResponse { get; set; }
        [DataMember]
        public string DataFormat { get; set; }
    }

    [DataContract]
    public class ApiMonitoringFileContentOutput
    {
        [DataMember]
        public string BodyFileContent { get; set; }
        [DataMember]
        public string RequestOrResponse { get; set; }
        [DataMember]
        public string DataFormat { get; set; }
        [DataMember]
        public bool BodyDataToShow { get; set; }
        [DataMember]
        public string BodyErrorMsg { get; set; }
        [DataMember]
        public bool FileExist { get; set; }
    }

    [DataContract]
    public class ApiMonitoringApiCallLogInput
    {
        [DataMember]
        public int ApiCallId { get; set; }
    }

    [DataContract]
    public class ApiMonitoringApiCallLogOutputItem
    {
        [DataMember]
        public int ApiCallDetailsId { get; set; }
        [DataMember]
        public string LogTime { get; set; }
        [DataMember]
        public string LogMessage { get; set; }
    }

    [DataContract]
    public class ApiMonitoringApiCallLogOutput
    {
        public ApiMonitoringApiCallLogOutput()
        {
            this.ApiCallLogItemsList = new List<ApiMonitoringApiCallLogOutputItem>();
        }

        [DataMember]
        public List<ApiMonitoringApiCallLogOutputItem> ApiCallLogItemsList { get; set; }
    }

    //[DataContract]
    //public class SectypeInfo
    //{
    //    [DataMember]
    //    public int SectypeID { get; set; }
    //    [DataMember]
    //    public string SectypeName { get; set; }
    //}

    //[DataContract]
    //public class UniquenessSetupKeyInfo
    //{
    //    public UniquenessSetupKeyInfo()
    //    {
    //        this.SelectedLeg = new UniquenessSetupLegInfo();
    //        this.SelectedAttributes = new List<UniquenessSetupAttributeInfo>();
    //        this.SelectedLegAttributes = new List<UniquenessSetupAttributeInfo>();
    //    }

    //    [DataMember]
    //    public int KeyID { get; set; }
    //    [DataMember]
    //    public string KeyName { get; set; }
    //    [DataMember]
    //    public bool IsMaster { get; set; }
    //    [DataMember]
    //    public bool IsAcrossSecurities { get; set; }
    //    [DataMember]
    //    public List<int> SelectedSectypes { get; set; }
    //    [DataMember]
    //    public UniquenessSetupLegInfo SelectedLeg { get; set; }
    //    [DataMember]
    //    public List<UniquenessSetupAttributeInfo> SelectedAttributes { get; set; }
    //    [DataMember]
    //    public List<UniquenessSetupAttributeInfo> SelectedLegAttributes { get; set; }
    //    [DataMember]
    //    public bool CheckInDrafts { get; set; }
    //    [DataMember]
    //    public bool CheckInWorkflows { get; set; }

    //    [DataMember]
    //    public bool NullAsUnique { get; set; }
    //}

    //[DataContract]
    //public class UniquenessSetupLegInfo
    //{
    //    [DataMember]
    //    public string LegIDs { get; set; }
    //    [DataMember]
    //    public string LegName { get; set; }
    //    [DataMember]
    //    public string AreAdditionalLegs { get; set; }
    //}

    //[DataContract]
    //public class UniquenessSetupAttributeInfo
    //{
    //    [DataMember]
    //    public string AttributeIDs { get; set; }
    //    [DataMember]
    //    public string AttributeName { get; set; }
    //    [DataMember]
    //    public string AreAdditionalLegAttributes { get; set; }
    //}

    //[DataContract]
    //public class UniquenessSetupCommonMasterAttributesOutputInfo
    //{
    //    public UniquenessSetupCommonMasterAttributesOutputInfo()
    //    {
    //        this.CommonMasterAttributesList = new List<UniquenessSetupAttributeInfo>();
    //    }

    //    [DataMember]
    //    public List<UniquenessSetupAttributeInfo> CommonMasterAttributesList { get; set; }
    //    [DataMember]
    //    public int KeyID { get; set; }
    //}

    //[DataContract]
    //public class UniquenessSetupCommonLegAttributesOutputInfo
    //{
    //    public UniquenessSetupCommonLegAttributesOutputInfo()
    //    {
    //        this.CommonLegAttributesList = new List<UniquenessSetupAttributeInfo>();
    //    }

    //    [DataMember]
    //    public List<UniquenessSetupAttributeInfo> CommonLegAttributesList { get; set; }
    //    [DataMember]
    //    public int KeyID { get; set; }
    //}

    [DataContract]
    public class UniquenessSetupFileDownloadInfo
    {
        [DataMember]
        public string isSuccess { get; set; }
        [DataMember]
        public string guidString { get; set; }
        //[DataMember]
        //public string fileName { get; set; }
        //[DataMember]
        //public string fullFilePath { get; set; }
    }

    //[DataContract]
    //public class UniquenessSetupOutputObject
    //{
    //    [DataMember]
    //    public int status { get; set; }
    //    [DataMember]
    //    public string message { get; set; }
    //    [DataMember]
    //    public List<KeyValuePair<string, List<SRMDuplicateInfo>>> uniquenessFailurePopupInfo { get; set; }
    //}

    [DataContract]
    public class CommonModuleTypeDetails
    {
        private int v1;
        private string v2;

        public CommonModuleTypeDetails(int moduleId, string displayName)
        {
            this.moduleId = moduleId;
            this.displayName = displayName;
        }

        //public CommonModuleTypeDetails(int v1, string v2)GetCommonModuleTypeDetails
        //{
        //    this.v1 = v1;
        //    this.v2 = v2;
        //}

        [DataMember]
        public int moduleId { get; set; }
        [DataMember]
        public string displayName { get; set; }

    }

    [DataContract]
    public class CASPM_CorpActionType
    {
        [DataMember]
        public int CorpActionTypeID { get; set; }
        [DataMember]
        public string CorpActionTypeName { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string IsPrioritized { get; set; }
    }

    [DataContract]
    public class CASP_VendorData
    {
        [DataMember]
        public int VendorID { get; set; }
        [DataMember]
        public string VendorName { get; set; }
        [DataMember]
        public int VendorTypeID { get; set; }
    }

    [DataContract]
    public class CASP_AttributeInfo
    {
        public CASP_AttributeInfo()
        {
            this.AttributeVendorList = new List<CASP_VendorData>();
        }

        [DataMember]
        public int AttributeID { get; set; }
        [DataMember]
        public string AttributeDisplayName { get; set; }
        [DataMember]
        public List<CASP_VendorData> AttributeVendorList { get; set; }
    }

    [DataContract]
    public class CASP_TabInfo
    {
        public CASP_TabInfo()
        {
            this.TabAttributesDetails = new List<CASP_AttributeInfo>();
            //this.TabColumnNames = new List<string>();
        }

        [DataMember]
        public int TabID { get; set; }
        [DataMember]
        public string TabName { get; set; }
        [DataMember]
        public int TabPriority { get; set; }
        [DataMember]
        public List<CASP_AttributeInfo> TabAttributesDetails { get; set; }
        //[DataMember]
        //public List<string> TabColumnNames { get; set; }
    }

    [DataContract]
    public class CASP_GetDataOutputObject
    {
        public CASP_GetDataOutputObject()
        {
            this.VendorData = new List<CASP_VendorData>();
            this.TabData = new List<CASP_TabInfo>();
        }

        [DataMember]
        public string CorpActionTypeName { get; set; }
        [DataMember]
        public List<CASP_VendorData> VendorData { get; set; }
        [DataMember]
        public List<CASP_TabInfo> TabData { get; set; }
    }


    public class DataSourceAndSystemMappingInfo
    {
        public DataSourceAndSystemMappingInfo()
        {
            dataSourceInfoWidgetList = new List<DataSourceInfoWidget>();
            systemInfoWidgetList = new List<SystemInfoWidget>();
        }

        [DataMember]
        public List<DataSourceInfoWidget> dataSourceInfoWidgetList { get; set; }

        [DataMember]
        public List<SystemInfoWidget> systemInfoWidgetList { get; set; }

        [DataMember]
        public string entityTypeDisplayName;
    }

    [DataContract]
    public class DataSourceInfoWidget
    {
        public DataSourceInfoWidget(string dataSourceName, int dataSourceId)
        {
            this.dataSourceName = dataSourceName;
            this.dataSourceId = dataSourceId;
        }
        [DataMember]
        public string dataSourceName { get; set; }
        [DataMember]
        public int dataSourceId { get; set; }
    }

    [DataContract]
    public class SystemInfoWidget
    {
        public SystemInfoWidget(int report_type_id, string systemInfoName, int systemInfoId)
        {
            this.reportTypeId = report_type_id;
            this.systemInfoName = systemInfoName;
            this.systemInfoId = systemInfoId;
        }
        [DataMember]
        public int reportTypeId;
        [DataMember]
        public string systemInfoName;
        [DataMember]
        public int systemInfoId;
    }
    class RMBaseReportClass
    {
        public int CalendarId { get; set; }
        public string DataSetName { get; set; }
        public string EndDate { get; set; }
        public string EntityCodes { get; set; }
        public int InstanceId { get; set; }
        public bool IsExtractionReport { get; set; }
        public bool IsLegacyReport { get; set; }
        public bool IsReportForExternalSystem { get; set; }
        public string KnowledgeDate { get; set; }
        public string LongDateFormat { get; set; }
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public int ReportTypeId { get; set; }
        public string StartDate { get; set; }
        public bool UpdateExtractionTime { get; set; }
    }
    #region WorkflowVariables


    [DataContract]
    public class SRMWorkflowInboxActionType
    {
        public SRMWorkflowInboxActionType(string actionName, int actionCount)
        {
            this.actionName = actionName;
            this.actionCount = actionCount;
        }

        [DataMember]
        public string actionName { get; set; }

        [DataMember]

        public int actionCount { get; set; }
    }


    enum SRMWorkflowInboxSubModuleType
    {
        CREATE = 0,
        UPDATE = 1,
        ATTRIBUTE = 2,
        LEG = 3,
        DELETE = 4
    }

    enum SRMWorkflowInboxActionName
    {
        PENDING_AT_ME = 0,
        REJECTED = 1,
        MY_REQUEST = 2
    }

    #endregion

    #region db locks variables
    [DataContract]
    public class DBLocksInfo
    {
        [DataMember]
        public int rowCount { get; set; }

        [DataMember]

        public bool ifLockExistsCurrently { get; set; }

        [DataMember]
        public string userName { get; set; }

        [DataMember]
        public string machineName { get; set; }

        [DataMember]
        public string firstAttempt { get; set; }

        [DataMember]
        public string whenAcquired { get; set; }

        [DataMember]
        public string whenReleased { get; set; }

        [DataMember]
        public string identifier { get; set; }

        [DataMember]
        public string stackTrace { get; set; }

        [DataMember]
        public int processId { get; set; }

        [DataMember]
        public string processName { get; set; }

    }

    [DataContract]
    public class Process
    {
        [DataMember]
        public string VictimId { get; set; }

        [DataMember]
        public string ProcessId { get; set; }

        [DataMember]
        public string OwnerId { get; set; }

        [DataMember]
        public string LockMode { get; set; }

        [DataMember]
        public string LogUsed { get; set; }
        [DataMember]
        public string Priority { get; set; }
        [DataMember]
        public string WaitResource { get; set; }
        [DataMember]
        public int WaitTime { get; set; }
        [DataMember]
        public string TransactionName { get; set; }
        [DataMember]
        public string LastTransacted { get; set; }
        [DataMember]
        public string Xdes { get; set; }
        [DataMember]
        public int SchedulerId { get; set; }
        [DataMember]
        public int Kpid { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public int Spid { get; set; }
        [DataMember]
        public int Transcount { get; set; }
        [DataMember]
        public string LastBatchStarted { get; set; }
        [DataMember]
        public string LastBatchCompleted { get; set; }
        [DataMember]
        public string LastAttention { get; set; }
        [DataMember]
        public string IsolationLevel { get; set; }
        [DataMember]
        public string XactId { get; set; }
        [DataMember]
        public string LockTimeOut { get; set; }
        [DataMember]
        public string InputBuff { get; set; }
        [DataMember]
        public int nodeCount { get; set; }

        [DataMember]
        public int Sbid { get; set; }

        [DataMember]
        public int Ecid { get; set; }
    }

    [DataContract]
    public class Resource
    {
        [DataMember]
        public string HobtId { get; set; }
        [DataMember]
        public string AssociatedObjectId { get; set; }
        [DataMember]
        public string IndexName { get; set; }
        [DataMember]
        public string ObjectName { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string ProcessIdwaiter { get; set; }
        [DataMember]
        public string Mode { get; set; }
    }

    [DataContract]
    public class Deadlock
    {
        [DataMember]
        public string DeadLockTimeStamp { get; set; }

        [DataMember]
        public string DeadLockXMLData { get; set; }
    }

    [DataContract]
    public class DeadLockResponseReceived
    {
        [DataMember]
        public List<Process> Processes { get; set; }

        [DataMember]
        public List<Resource> Resources { get; set; }
    }

    [DataContract]

    public class QueryStats
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string QueryText { get; set; }

        [DataMember]
        public long ExecutionCount { get; set; }

        [DataMember]
        public string CreationTime { get; set; }

        [DataMember]
        public string LastExecutionTime { get; set; }

        [DataMember]
        public long LastWorkerTime { get; set; }

        [DataMember]
        public long MinWorkerTime { get; set; }

        [DataMember]
        public long MaxWorkerTime { get; set; }

        [DataMember]
        public long LastPhysicalReads { get; set; }

        [DataMember]
        public long MinPhysicalReads { get; set; }

        [DataMember]
        public long MaxPhysicalReads { get; set; }

        [DataMember]
        public long LastLogicalWrites { get; set; }

        [DataMember]
        public long MinLogicalWrites { get; set; }

        [DataMember]
        public long MaxLogicalWrites { get; set; }

        [DataMember]
        public long LastLogicalReads { get; set; }

        [DataMember]
        public long MinLogicalReads { get; set; }

        [DataMember]
        public long MaxLogicalReads { get; set; }

        [DataMember]
        public long LastElapsedTime { get; set; }

        [DataMember]
        public long MinElapsedTime { get; set; }

        [DataMember]
        public long MaxElapsedTime { get; set; }

        [DataMember]
        public string QueryPlan { get; set; }

    }


    #endregion

    #region Migration Utiltiy Info
    [DataContract]

    public class MigrationFeaturesInfoUI
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }
    }

    [DataContract]
    public class MigrationModuleCheckboxInfo
    {
        public MigrationModuleCheckboxInfo()
        {
            infoList = new List<MigrationModuleCheckboxInfo>();
        }
        public MigrationModuleCheckboxInfo(int id, string name)
        {
            infoList = new List<MigrationModuleCheckboxInfo>();
            this.id = id;
            this.name = name;
        }

        [DataMember]
        public string name { get; set; }
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public List<MigrationModuleCheckboxInfo> infoList { get; set; }
    }

    [DataContract]
    public class PrivilegeInfo
    {

        [DataMember]
        public bool result { get; set; }
        [DataMember]
        public string name { get; set; }
    }
    #endregion


    [AspNetCompatibilityRequirements
    (RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CommonService : RServiceBaseClass, ICommonService
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("CommonService.CommonService");
        string SECMASTER = "SecMaster";
        string REFMASTER = "RefMaster";
        private const string DISPLAY_NAME = "display_name";
        private const string ATTRIBUTE_NAME = "attribute_name";
        private const string DATA_TYPE_NAME = "data_type_name";
        private const string STRING = "STRING";
        private const string NUMERIC = "NUMERIC";
        private const string DECIMAL = "DECIMAL";
        private const string INT = "INT";
        private const string VARCHAR = "VARCHAR";
        private const string VARCHARMAX = "VARCHARMAX";
        private const string DATE = "DATE";
        private const string DATETIME = "DATETIME";
        private const string TIME = "TIME";
        private const string BOOLEAN = "BOOLEAN";
        private const string BIT = "BIT";
        private const string LOOKUP = "LOOKUP";
        private const string ATTRIBUTE_TYPE_NAME = "attribute_type_name";
        private const string REFERENCE = "Reference";
        private const string REFERENCE_TYPE_ID = "reference_type_id";
        private const string MAS_ATTRIBUTE_NAME = "mas_attribute_name";
        private const string REFERENCE_ATTRIBUTE_NAME = "reference_attribute_name";
        private const string REFERENCE_ATTRIBUTE_DISPLAY = "reference_attribute_display";
        private const string REFERENCE_DATA_TYPE = "reference_data_type";
        static object lockObj = new object();
        List<RMProcessedEntityInfo> rmProcessInfo = new List<RMProcessedEntityInfo>();

        //public CommonService()
        //{
        //    if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Headers != null)
        //    {
        //        var headers = HttpContext.Current.Request.Headers;
        //        if (!string.IsNullOrEmpty(headers["ClientName"]))
        //        {
        //            RMultiTanentClientUtil.ClientName = "CA";
        //            RMultiTanentUtil.ClientName = "CA";
        //        }
        //    }
        //}

        public RADXRuleGrammarInfo PrepareRuleGrammarInfo(string type_ids, int moduleId)
        {
            DataTable dt = null;
            List<string> lstReferenceAttributeName = null;
            var type_idsArray = type_ids.Split(',');
            if (type_idsArray.Length > 1)
                dt = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_GetWorkflowTypeDetails] '{0}', {1}", "Common Attribute", moduleId), ConnectionConstants.RefMaster_Connection).Tables[0];
            else
                dt = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_GetWorkflowTypeDetails] '{0}', {1}, {2}", "Primary Attribute", moduleId, type_idsArray[0]), ConnectionConstants.RefMaster_Connection).Tables[0];

            string ATTRIBUTES_UPDATED = "Attributes Updated";
            if (moduleId != 3)
            {
                DataRow dr = dt.NewRow();
                dr[DISPLAY_NAME] = ATTRIBUTES_UPDATED;
                dr[ATTRIBUTE_NAME] = ATTRIBUTES_UPDATED;
                dr[DATA_TYPE_NAME] = "VARCHAR";
                dt.Rows.Add(dr);
            }

            //if (moduleId == 3)
            //    lstReferenceAttributeName = dt.AsEnumerable().Where(x => !string.IsNullOrEmpty(Convert.ToString(x["reference_type_id"]))).Select(y => Convert.ToString(y["attribute_name"])).ToList();
            //else
            //    lstReferenceAttributeName = dt.AsEnumerable().Where(x => !string.IsNullOrEmpty(Convert.ToString(x["reference_type_id"]))).Select(y => Convert.ToString(y["attribute_name"])).ToList();

            return PrepareRuleGrammarInfoInternal(dt, lstReferenceAttributeName,moduleId, type_ids);
        }

        public RADXRuleGrammarInfo PrepareRuleGrammarInfoInternal(DataTable dtAttribute, List<string> lstReferenceAttributeName,int moduleId,string entityTypeId)
        {
            List<RADXRuleColumnInfo> listColumnInfo = new List<RADXRuleColumnInfo>();
            List<RADXRuleCustomOpParamsInfo> paramaInfo = new List<RADXRuleCustomOpParamsInfo>();
            if (dtAttribute != null && (dtAttribute.Rows.Count > 0))
            {
                Dictionary<string, string> attrVsDataType = new Dictionary<string, string>();
                if(moduleId != 3)
                {
                    DataTable refAttributes = new DataTable();
                    refAttributes = RMCommonUtils.GetAllAttributesForEntityTypeAndLegs(Convert.ToInt32(entityTypeId), new RCommon().SessionInfo.LoginName);
                    
                    if (refAttributes != null && refAttributes.Rows.Count > 0)
                    {
                        attrVsDataType = refAttributes.AsEnumerable().Where(r => Convert.ToInt32(r["entity_type_id"]) == Convert.ToInt32(entityTypeId)).ToDictionary(x => Convert.ToString(x["Column Name"]), y => Convert.ToString(y["real_data_type"]));
                    }
                }
                RADXRuleColumnInfo columnInfo = null;
                RADXRuleCustomOpParamsInfo prmInfo = null;
                foreach (DataRow dr in dtAttribute.Rows)
                {
                    columnInfo = new RADXRuleColumnInfo();
                    prmInfo = new RADXRuleCustomOpParamsInfo();
                    columnInfo.ColumnName = dr[DISPLAY_NAME].ToString();
                    columnInfo.actualColumnName = dr[ATTRIBUTE_NAME].ToString();

                    prmInfo.ActualColumnName = columnInfo.actualColumnName;
                    prmInfo.IsInput = true;
                    prmInfo.UserDefinedColumnName = columnInfo.ColumnName;

                    columnInfo.ColumnType = RADXRuleColumnType.Both;

                    columnInfo.IsRhsColumn = true;
                    columnInfo.IsRhsEnum = false;
                    columnInfo.IsRhsUserInput = true;

                    var dataType = Convert.ToString(dr[DATA_TYPE_NAME]).Trim();
                    if(dataType == LOOKUP && moduleId != 3 && attrVsDataType.ContainsKey(columnInfo.actualColumnName))
                    {
                        dataType = attrVsDataType[columnInfo.actualColumnName].Trim().ToUpper();
                    }
                    switch (dataType)
                    {
                        case NUMERIC:
                        case DECIMAL:
                        case INT:
                            columnInfo.DataType = RADXRuleDataType.Numeric;
                            break;
                        case STRING:
                        case VARCHAR:
                        case VARCHARMAX:
                        case BOOLEAN:
                            columnInfo.DataType = RADXRuleDataType.Text;
                            break;
                        case BIT:
                            columnInfo.DataType = RADXRuleDataType.Text;
                            break;
                        case DATETIME:
                        case DATE:
                            columnInfo.DataType = RADXRuleDataType.DateTime;
                            break;
                        case TIME:
                            columnInfo.DataType = RADXRuleDataType.Time;
                            break;
                        case LOOKUP:
                            columnInfo.DataType = RADXRuleDataType.Text;
                            break;
                    }
                    prmInfo.DataType = columnInfo.DataType;

                    //columnInfo.ActionSideApplicability = RADXRuleColumnApplicability.BOTH;
                    columnInfo.ExpressionSideApplicability = RADXRuleColumnApplicability.BOTH;
                    paramaInfo.Add(prmInfo);
                    listColumnInfo.Add(columnInfo);
                }
            }

            RADXRuleGrammarInfo info = new RADXRuleGrammarInfo();
            info.RuleType = com.ivp.rad.controls.xruleeditor.RADXRuleType.ConditionalRule;
            info.Columns = listColumnInfo;
            info.ElseCount = 1;
            info.ThenCount = 1;
            info.IsDataTypeAvailable = true;

            RMCommonController.PrepareModalsInfo(info, paramaInfo, "Workflow");

            return info;
        }

        public ExternalSystemInfo GetExternalSystemInfo(ExternalSystemInputInfo inputObject)
        {
            ExternalSystemInfo res = new ExternalSystemInfo();

            if (inputObject.selectedTab == SECMASTER)
            {
                List<string> externalSystems = new List<string>();
                DataTable systems = WorkflowController.SMGetDownStreamSystems(inputObject.userName);

                if (systems != null && systems.Rows.Count > 0)
                {
                    externalSystems.Add("-1<@>Select All");

                    systems.AsEnumerable().ToList().ForEach(sys =>
                    {
                        externalSystems.Add(Convert.ToString(sys["system_id"]) + "<@>" + Convert.ToString(sys["system_name"]) + "<@>" + (Convert.ToBoolean(sys["real_time_posting"]) ? "true" : "false"));
                    });
                }

                res.ExternalSystemNameAndIds = externalSystems;
                res.PostToPrivilege = true;
            }
            else if (inputObject.selectedTab == REFMASTER)
            {
                List<string> externalSystems = new List<string>();
                DataTable systems = null;// WorkflowController.RMGetDownStreamSystems(0);

                if (systems != null && systems.Rows.Count > 0)
                {
                    externalSystems.Add("-1<@>Select All");

                    systems.AsEnumerable().ToList().ForEach(sys =>
                    {
                        externalSystems.Add(Convert.ToString(sys["report_system_id"]) + "<@>" + Convert.ToString(sys["report_system_name"]) + "<@>true");
                    });
                }

                res.ExternalSystemNameAndIds = externalSystems; //List of systems (System Id<@>System Name<@>Default selected)
                res.PostToPrivilege = true;
            }
            return res;
        }

        public void PostToDownstream(DownstreamPostInputInfo inputObject)
        {
            if (inputObject.selectedTab == SECMASTER)
            {
                WorkflowController.SMPostToDownstream(inputObject.systemIds, inputObject.SecIds, inputObject.userName);
            }
            else if (inputObject.selectedTab == REFMASTER)
            {
                WorkflowController.RMPostToDownstream(inputObject.systemIds, inputObject.SecIds, inputObject.userName, false);
            }
        }

        public List<GridData> GetGridData(WorkflowGridInput inputObject)
        {
            mLogger.Debug("GetGridData -> Start");
            List<GridData> listGridData = new List<GridData>();
            GridData objGridData = null;

            DataSet dsRejectedRequests = new DataSet();
            DataSet dsRequestsPending = new DataSet();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            DataSet dsWorkflowStatus = null;

            if (inputObject.selectedTab == SECMASTER)
            {
                dsWorkflowStatus = WorkflowController.GetWorkflowStatusData(inputObject);

                PopulatePageConfiguredAttributes(ref dsWorkflowStatus, inputObject.userName);

                MassageDateTimeData(ref dsWorkflowStatus, inputObject);

                WorkflowController.PerformRefDataMassage(ref dsWorkflowStatus, DateTime.Now.ToString());
            }
            else
            {
                dsWorkflowStatus = WorkflowController.GetRMWorkflowStatusData(inputObject);
            }

            DataTable dtMyRequestsOriginal = dsWorkflowStatus.Tables[0];
            DataTable dtRejectedRequests = dsWorkflowStatus.Tables[1];
            DataTable dtRequestsPending = dsWorkflowStatus.Tables[2];
            DataTable dtAllRequestsOriginal = dsWorkflowStatus.Tables[3];

            if (inputObject.getMyRequests)
            {
                WorkflowStatusDataHandler.ClearGridDataForGridType(inputObject.userSessionIdentifier, WorkflowGridType.MYREQUESTS);
                if (dtMyRequestsOriginal != null && dtMyRequestsOriginal.Rows.Count > 0)
                {
                    foreach (var item in dtMyRequestsOriginal.AsEnumerable().GroupBy(x => new { row_id = Convert.ToInt32(x["row_id"]) }))
                    {
                        string userNames = string.Join(",", item.Select(x => Convert.ToString(x["workflow_group_user_name"])));

                        foreach (var row in item)
                        {
                            row["workflow_group_user_name"] = userNames;
                        }
                    }
                    DataTable dtMyRequests = dtMyRequestsOriginal.AsEnumerable().GroupBy(x => Convert.ToInt32(x["row_id"])).Select(x => x.First()).CopyToDataTable();

                    List<DataSet> lstData = new List<DataSet>();
                    foreach (var item in dtMyRequests.AsEnumerable().GroupBy(x => new { workflow_group_user_name = Convert.ToString(x["workflow_group_user_name"]) }))
                    {
                        DataSet dsData = new DataSet();
                        DataTable dtData = new DataTable();
                        dtData = dtMyRequests.Clone();
                        foreach (DataRow dr in item)
                        {
                            dtData.Rows.Add(dr.ItemArray);
                        }
                        dsData.Tables.Add(dtData);
                        dtData.TableName = item.Key.workflow_group_user_name;

                        lstData.Add(dsData);
                    }

                    foreach (var dsData in lstData)
                    {
                        string guid = "s" + Guid.NewGuid().ToString().Replace('-', '_');
                        objGridData = new GridData();
                        objGridData.gridID = guid + "_MyRequestsDiv";
                        objGridData.currentPageId = guid + "_123";
                        objGridData.viewKey = guid + "_789";
                        objGridData.sessionIdentifier = guid;
                        objGridData.isMyRequestGrid = true;
                        objGridData.isAllRequestGrid = false;
                        objGridData.isDataAvailable = true;
                        objGridData.usersGroups = dsData.Tables[0].TableName;
                        objGridData.rowCount = dsData.Tables[0].Rows.Count;
                        dsData.Tables[0].TableName = "DataTable";

                        List<CustomRowDataInfo> CustomRowDataInfo = GetCustomRowDataInfo(dsData, inputObject.selectedTab, inputObject.userSessionIdentifier, Convert.ToString(WorkflowGridType.MYREQUESTS), objGridData.gridID);

                        string customInfoScript = serializer.Serialize(CustomRowDataInfo);
                        objGridData.customInfoScript = customInfoScript;

                        GridInfo gridInfo = GetGridInfo(inputObject.userName, inputObject.selectedTab);

                        gridInfo.ViewKey = objGridData.viewKey;
                        gridInfo.GridId = objGridData.gridID;
                        gridInfo.CurrentPageId = objGridData.currentPageId;
                        gridInfo.SessionIdentifier = objGridData.sessionIdentifier;
                        gridInfo.CustomRowsDataInfo = CustomRowDataInfo;
                        gridInfo.DateFormat = inputObject.shortDateFormat;

                        RADNeoGridService service = new RADNeoGridService();
                        service.SaveGridDataInCache(dsData.Tables[0], inputObject.userName, objGridData.gridID, objGridData.currentPageId, objGridData.viewKey, objGridData.sessionIdentifier, false, gridInfo);
                        WorkflowStatusDataHandler.SetGridData(inputObject.userSessionIdentifier, WorkflowGridType.MYREQUESTS, objGridData.gridID, dsData);

                        listGridData.Add(objGridData);
                    }
                }
                else
                {
                    objGridData = new GridData();
                    objGridData.isMyRequestGrid = true;
                    objGridData.isAllRequestGrid = false;
                    objGridData.isDataAvailable = false;

                    listGridData.Add(objGridData);
                }
            }

            if (inputObject.getRejectedRequests)
            {
                WorkflowStatusDataHandler.ClearGridDataForGridType(inputObject.userSessionIdentifier, WorkflowGridType.REJECTEDREQUESTS);
                dsWorkflowStatus.Tables.Remove(dtRejectedRequests);
                dsRejectedRequests.Tables.Add(dtRejectedRequests);

                if (dsRejectedRequests != null && dsRejectedRequests.Tables.Count > 0 && dsRejectedRequests.Tables[0].Rows.Count > 0)
                {
                    string guid = "s" + Guid.NewGuid().ToString().Replace('-', '_');
                    objGridData = new GridData();
                    objGridData.gridID = inputObject.rejectedRequestsDivId;
                    objGridData.currentPageId = guid + "_123";
                    objGridData.viewKey = guid + "_789";
                    objGridData.sessionIdentifier = guid;
                    objGridData.isMyRequestGrid = false;
                    objGridData.isAllRequestGrid = false;
                    objGridData.isDataAvailable = true;
                    objGridData.rowCount = dsRejectedRequests.Tables[0].Rows.Count;
                    dsRejectedRequests.Tables[0].TableName = "DataTable";

                    List<CustomRowDataInfo> CustomRowDataInfo = GetCustomRowDataInfo(dsRejectedRequests, inputObject.selectedTab, inputObject.userSessionIdentifier, Convert.ToString(WorkflowGridType.REJECTEDREQUESTS), objGridData.gridID);

                    string customInfoScript = serializer.Serialize(CustomRowDataInfo);
                    objGridData.customInfoScript = customInfoScript;

                    GridInfo gridInfo = GetGridInfo(inputObject.userName, inputObject.selectedTab);

                    gridInfo.ViewKey = objGridData.viewKey;
                    gridInfo.GridId = objGridData.gridID;
                    gridInfo.CurrentPageId = objGridData.currentPageId;
                    gridInfo.SessionIdentifier = objGridData.sessionIdentifier;
                    gridInfo.CustomRowsDataInfo = CustomRowDataInfo;
                    gridInfo.DateFormat = inputObject.shortDateFormat;

                    RADNeoGridService service = new RADNeoGridService();
                    service.SaveGridDataInCache(dsRejectedRequests.Tables[0], inputObject.userName, objGridData.gridID, objGridData.currentPageId, objGridData.viewKey, objGridData.sessionIdentifier, false, gridInfo);
                    WorkflowStatusDataHandler.SetGridData(inputObject.userSessionIdentifier, WorkflowGridType.REJECTEDREQUESTS, objGridData.gridID, dsRejectedRequests);

                    listGridData.Add(objGridData);
                }
                else
                {
                    objGridData = new GridData();
                    objGridData.gridID = inputObject.rejectedRequestsDivId;
                    objGridData.isMyRequestGrid = false;
                    objGridData.isAllRequestGrid = false;
                    objGridData.isDataAvailable = false;
                    objGridData.rowCount = 0;

                    listGridData.Add(objGridData);
                }
            }

            if (inputObject.getRequestsPending)
            {
                WorkflowStatusDataHandler.ClearGridDataForGridType(inputObject.userSessionIdentifier, WorkflowGridType.REQUESTSPENDING);
                dsWorkflowStatus.Tables.Remove(dtRequestsPending);
                dsRequestsPending.Tables.Add(dtRequestsPending);

                if (dsRequestsPending != null && dsRequestsPending.Tables.Count > 0 && dsRequestsPending.Tables[0].Rows.Count > 0)
                {
                    string guid = "s" + Guid.NewGuid().ToString().Replace('-', '_');
                    objGridData = new GridData();
                    objGridData.gridID = inputObject.requestsPendingDivId;
                    objGridData.currentPageId = guid + "_123";
                    objGridData.viewKey = guid + "_789";
                    objGridData.sessionIdentifier = guid;
                    objGridData.isMyRequestGrid = false;
                    objGridData.isAllRequestGrid = false;
                    objGridData.isDataAvailable = true;
                    objGridData.rowCount = dsRequestsPending.Tables[0].Rows.Count;
                    dsRequestsPending.Tables[0].TableName = "DataTable";

                    List<CustomRowDataInfo> CustomRowDataInfo = GetCustomRowDataInfo(dsRequestsPending, inputObject.selectedTab, inputObject.userSessionIdentifier, Convert.ToString(WorkflowGridType.REQUESTSPENDING), objGridData.gridID);

                    string customInfoScript = serializer.Serialize(CustomRowDataInfo);
                    objGridData.customInfoScript = customInfoScript;

                    GridInfo gridInfo = GetGridInfo(inputObject.userName, inputObject.selectedTab);

                    gridInfo.ViewKey = objGridData.viewKey;
                    gridInfo.GridId = objGridData.gridID;
                    gridInfo.CurrentPageId = objGridData.currentPageId;
                    gridInfo.SessionIdentifier = objGridData.sessionIdentifier;
                    gridInfo.CustomRowsDataInfo = CustomRowDataInfo;
                    gridInfo.DateFormat = inputObject.shortDateFormat;

                    RADNeoGridService service = new RADNeoGridService();
                    service.SaveGridDataInCache(dsRequestsPending.Tables[0], inputObject.userName, objGridData.gridID, objGridData.currentPageId, objGridData.viewKey, objGridData.sessionIdentifier, false, gridInfo);
                    WorkflowStatusDataHandler.SetGridData(inputObject.userSessionIdentifier, WorkflowGridType.REQUESTSPENDING, objGridData.gridID, dsRequestsPending);

                    listGridData.Add(objGridData);
                }
                else
                {
                    objGridData = new GridData();
                    objGridData.gridID = inputObject.requestsPendingDivId;
                    objGridData.isMyRequestGrid = false;
                    objGridData.isAllRequestGrid = false;
                    objGridData.isDataAvailable = false;
                    objGridData.rowCount = 0;

                    listGridData.Add(objGridData);
                }
            }

            if (inputObject.getAllRequests)
            {
                WorkflowStatusDataHandler.ClearGridDataForGridType(inputObject.userSessionIdentifier, WorkflowGridType.ALLREQUESTS);
                if (dtAllRequestsOriginal != null && dtAllRequestsOriginal.Rows.Count > 0)
                {
                    foreach (var item in dtAllRequestsOriginal.AsEnumerable().GroupBy(x => new { row_id = Convert.ToInt32(x["row_id"]) }))
                    {
                        string userNames = string.Join(",", item.Select(x => Convert.ToString(x["workflow_group_user_name"])));

                        foreach (var row in item)
                        {
                            row["workflow_group_user_name"] = userNames;
                        }
                    }
                    DataTable dtAllRequests = dtAllRequestsOriginal.AsEnumerable().GroupBy(x => Convert.ToInt32(x["row_id"])).Select(x => x.First()).CopyToDataTable();

                    List<DataSet> lstData = new List<DataSet>();
                    foreach (var item in dtAllRequests.AsEnumerable().GroupBy(x => new { workflow_group_user_name = Convert.ToString(x["workflow_group_user_name"]) }))
                    {
                        DataSet dsData = new DataSet();
                        DataTable dtData = new DataTable();
                        dtData = dtAllRequests.Clone();
                        foreach (DataRow dr in item)
                        {
                            dtData.Rows.Add(dr.ItemArray);
                        }
                        dsData.Tables.Add(dtData);
                        dtData.TableName = item.Key.workflow_group_user_name;

                        lstData.Add(dsData);
                    }

                    foreach (var dsData in lstData)
                    {
                        string guid = "s" + Guid.NewGuid().ToString().Replace('-', '_');
                        objGridData = new GridData();
                        objGridData.gridID = guid + "_AllRequestsDiv";
                        objGridData.currentPageId = guid + "_123";
                        objGridData.viewKey = guid + "_789";
                        objGridData.sessionIdentifier = guid;
                        objGridData.isMyRequestGrid = false;
                        objGridData.isAllRequestGrid = true;
                        objGridData.isDataAvailable = true;
                        objGridData.usersGroups = dsData.Tables[0].TableName;
                        objGridData.rowCount = dsData.Tables[0].Rows.Count;
                        dsData.Tables[0].TableName = "DataTable";

                        List<CustomRowDataInfo> CustomRowDataInfo = GetCustomRowDataInfo(dsData, inputObject.selectedTab, inputObject.userSessionIdentifier, Convert.ToString(WorkflowGridType.ALLREQUESTS), objGridData.gridID);

                        string customInfoScript = serializer.Serialize(CustomRowDataInfo);
                        objGridData.customInfoScript = customInfoScript;

                        GridInfo gridInfo = GetGridInfo(inputObject.userName, inputObject.selectedTab);

                        gridInfo.ViewKey = objGridData.viewKey;
                        gridInfo.GridId = objGridData.gridID;
                        gridInfo.CurrentPageId = objGridData.currentPageId;
                        gridInfo.SessionIdentifier = objGridData.sessionIdentifier;
                        gridInfo.CustomRowsDataInfo = CustomRowDataInfo;
                        gridInfo.DateFormat = inputObject.shortDateFormat;

                        RADNeoGridService service = new RADNeoGridService();
                        service.SaveGridDataInCache(dsData.Tables[0], inputObject.userName, objGridData.gridID, objGridData.currentPageId, objGridData.viewKey, objGridData.sessionIdentifier, false, gridInfo);
                        WorkflowStatusDataHandler.SetGridData(inputObject.userSessionIdentifier, WorkflowGridType.ALLREQUESTS, objGridData.gridID, dsData);

                        listGridData.Add(objGridData);
                    }
                }
                else
                {
                    objGridData = new GridData();
                    objGridData.isMyRequestGrid = false;
                    objGridData.isAllRequestGrid = true;
                    objGridData.isDataAvailable = false;

                    listGridData.Add(objGridData);
                }
            }

            mLogger.Debug("GetGridData -> End");
            return listGridData;
        }

        private List<HiddenColumnInfo> GetColumnsToHide(bool IsGrouped)
        {
            if (IsGrouped)
                return new List<HiddenColumnInfo>() {
                    new HiddenColumnInfo { ColumnName = "queue_id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "row_id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "row_keys", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "workflow_group_user_name", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Security", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Security Id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Security Type", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Security Name", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Requested On", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "data_type_name", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "sectype_table_id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "attribute_id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "entity_type_id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Entity", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Entity Code", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Entity Type", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Entity Name", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "entity_type_table", isDefault = true }
                };
            else
                return new List<HiddenColumnInfo>() {
                    new HiddenColumnInfo { ColumnName = "queue_id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "row_id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "row_keys", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "workflow_group_user_name", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Security", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "data_type_name", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "sectype_table_id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "attribute_id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "entity_type_id", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "Entity", isDefault = true },
                    new HiddenColumnInfo { ColumnName = "entity_type_table", isDefault = true }
                };
        }

        private GridInfo GetGridInfo(string userName, string moduleName, bool isGrouped = true)
        {
            List<HiddenColumnInfo> columnToHide = GetColumnsToHide(isGrouped);
            GridInfo gridInfo = new GridInfo
            {
                SelectRecordsAcrossAllPages = null,
                CacheGriddata = true,
                UserId = userName,
                Height = "350px",
                ColumnsWithoutClientSideFunctionality = new List<string>(),
                RequireEditGrid = false,
                RequireEditableRow = false,
                IdColumnName = "row_id",
                KeyColumns = new Dictionary<KeyType, string> { { KeyType.RowKey, "row_keys" } },
                TableName = "DataTable",
                PageSize = 50,
                RequirePaging = false,
                RequireInfiniteScroll = true,
                CollapseAllGroupHeader = false,
                //GridTheme = Theme.MasterChildGridTheme,
                DoNotExpand = false,
                ItemText = "Number of Requests",
                DoNotRearrangeColumn = true,
                RequireGrouping = true,
                RequireFilter = true,
                RequireSort = true,
                RequireMathematicalOperations = false,
                RequireSelectedRows = true,
                RequireExportToExcel = true,
                RequireSearch = true,
                RequireFreezeColumns = false,
                RequireHideColumns = true,
                RequireColumnSwap = true,
                RequireGroupExpandCollapse = true,
                RequireResizing = true,
                RequireLayouts = false,
                RequireGroupHeaderCheckbox = true,
                RequireRuleBasedColoring = false,
                RequireExportToPdf = false,
                ShowRecordCountOnHeader = false,
                ShowAggregationOnHeader = true,
                ColumnsNotToSum = new List<string>() { "row_id", "queue_id", "sectype_table_id", "attribute_id", "entity_type_id" },
                CheckBoxInfo = new CheckBoxInfo(),
                RaiseGridCallBackBeforeExecute = "",
                RaiseGridRenderComplete = "workflowstatus.gridRenderComplete",
                DefaultGroupedAndSortedColumns = new List<SortInfo>(),
                ColumnsToHide = columnToHide,
                CustomFormatInfoClientSide = new Dictionary<string, FormatterInfo>()
            };

            if (isGrouped)
            {
                if (moduleName == SECMASTER)
                {
                    gridInfo.DefaultGroupedAndSortedColumns = new List<SortInfo>() { new SortInfo { ColumnName = "Security", IsGrouped = true } };
                    gridInfo.CustomFormatInfoClientSide = new Dictionary<string, FormatterInfo>() { { "Security", new FormatterInfo { AssemblyName = "CommonService", FormatString = "{0:C2}", ClassName = "CommonService.HeaderFormatter" } } };
                }
                else
                {
                    gridInfo.DefaultGroupedAndSortedColumns = new List<SortInfo>() { new SortInfo { ColumnName = "Entity", IsGrouped = true } };
                    gridInfo.CustomFormatInfoClientSide = new Dictionary<string, FormatterInfo>() { { "Entity", new FormatterInfo { AssemblyName = "CommonService", FormatString = "{0:C2}", ClassName = "CommonService.RMHeaderFormatter" } } };
                }
            }
            return gridInfo;
        }

        private void MassageDateTimeData(ref DataSet ds, WorkflowGridInput inputObject)
        {
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int index = 0; index < ds.Tables.Count - 1; index++)
                {
                    DataTable dt = ds.Tables[index];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string dataType = Convert.ToString(dr["data_type_name"]).ToLower();
                            string attrVal = Convert.ToString(dr["Attribute Value"]);
                            string attrValOld = Convert.ToString(dr["Old Attribute Value"]);
                            switch (dataType)
                            {
                                case "date":
                                    if (!string.IsNullOrEmpty(attrVal))
                                        dr["Attribute Value"] = Convert.ToDateTime(attrVal).ToString(inputObject.shortDateFormat);
                                    if (!string.IsNullOrEmpty(attrValOld))
                                        dr["Old Attribute Value"] = Convert.ToDateTime(attrValOld).ToString(inputObject.shortDateFormat);
                                    break;
                                case "datetime":
                                    if (!string.IsNullOrEmpty(attrVal))
                                        dr["Attribute Value"] = Convert.ToDateTime(attrVal).ToString(inputObject.longDateFormat);
                                    if (!string.IsNullOrEmpty(attrValOld))
                                        dr["Old Attribute Value"] = Convert.ToDateTime(attrValOld).ToString(inputObject.longDateFormat);
                                    break;
                                case "time":
                                    if (!string.IsNullOrEmpty(attrVal))
                                        dr["Attribute Value"] = TimeSpan.Parse(attrVal).ToString(@"hh\:mm\:ss");
                                    if (!string.IsNullOrEmpty(attrValOld))
                                        dr["Old Attribute Value"] = TimeSpan.Parse(attrValOld).ToString(@"hh\:mm\:ss");
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private DataTable GetLegDataFromQueueID(int queueID, DataTable dt)
        {
            mLogger.Debug("CommonService -> GetLegJSONFromQueueID -> Start");
            string jsonData = string.Empty;
            DataSet ds = null;
            DataTable dtAttributes = null;
            string displayName = string.Empty;
            string internalColumn = string.Empty;

            try
            {
                ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMaster.dbo.REFM_GetLegDataJsonForWorkflow '{0}'", queueID), ConnectionConstants.RefMaster_Connection);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    jsonData = Convert.ToString(ds.Tables[0].Rows[0][0]);
                }
                if (!string.IsNullOrEmpty(jsonData))
                {
                    dt = (DataTable)(JsonConvert.DeserializeObject(jsonData, typeof(DataTable)));

                    DataTable tempTable = new DataTable();
                    dt.Columns.Cast<DataColumn>().ToList().ForEach(col =>
                        {
                            DataColumn column = new DataColumn();
                            column.ColumnName = col.ColumnName;
                            column.DataType = typeof(System.String);
                            tempTable.Columns.Add(column);
                        });
                    foreach (DataRow dr in dt.Rows)
                    {
                        tempTable.Rows.Add(dr.ItemArray);
                    }
                    dt = tempTable;

                }
                if (dt != null && ds != null && ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    dtAttributes = ds.Tables[1];
                    dt.Columns.Cast<DataColumn>().ToList().ForEach(col =>
                    {
                        DataRow dataRow = dtAttributes.AsEnumerable().Where(x => Convert.ToString(x["attribute_name"]) == col.ColumnName).FirstOrDefault();
                        if (dataRow != null)
                        {

                            string dataType = Convert.ToString(dataRow["data_type"]);
                            bool isInternal = Convert.ToBoolean(dataRow["is_internal"]);
                            displayName = Convert.ToString(dataRow["display_name"]);
                            if (isInternal)
                                internalColumn = displayName;
                            if (dataType.Equals("LOOKUP") && !isInternal)
                            {
                                string parentEntityType = Convert.ToString(dataRow["parent_entity_type_name"]);
                                string parentAttribute = Convert.ToString(dataRow["parent_entity_attribute_name"]);
                                List<string> lookupEntityCodes = new List<string>();
                                foreach (DataRow row in dt.Rows)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(row[col.ColumnName]).Trim()))
                                        lookupEntityCodes.Add("'" + Convert.ToString(row[col.ColumnName]) + "'");
                                }
                                DataSet dsLookupData = null;
                                if (lookupEntityCodes.Count > 0)
                                {
                                    dsLookupData = CommonDALWrapper.ExecuteSelectQuery(@"SELECT entity_code,[" + parentAttribute + "] FROM [IVPRefMaster].[dbo].[" + parentEntityType + @"] 
                                                    WHERE entity_code IN (" + string.Join(",", lookupEntityCodes) + ")", ConnectionConstants.RefMaster_Connection);
                                }
                                if (dsLookupData != null && dsLookupData.Tables.Count > 0 && dsLookupData.Tables[0] != null && dsLookupData.Tables[0].Rows.Count > 0)
                                {
                                    dsLookupData.Tables[0].Rows.Cast<DataRow>().ToList().ForEach(row =>
                                    {
                                        string entityCode = Convert.ToString(row[0]);
                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            if (Convert.ToString(dr[col.ColumnName]).Equals(entityCode))
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(row[1])))
                                                    dr[col.ColumnName] = Convert.ToString(row[1]);
                                            }
                                        }
                                    });

                                }
                            }
                            if (dataType.Equals("DATETIME"))
                            {
                                //col.DataType = typeof(System.String);
                                foreach (DataRow dr in dt.Rows)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(dr[col.ColumnName])))
                                    {
                                        dr[col.ColumnName] = Convert.ToDateTime(dr[col.ColumnName]).ToString("MM/dd/yyyy");
                                    }
                                }
                            }
                            col.ColumnName = displayName;
                        }
                        //displayName = dtAttributes.AsEnumerable().Where(x => Convert.ToString(x["attribute_name"]) == col.ColumnName).Select(y => Convert.ToString(y["display_name"])).FirstOrDefault();

                    });
                    if (!string.IsNullOrEmpty(internalColumn))
                        dt.Columns.Remove(internalColumn);
                }
                if (dt != null)
                {
                    DataColumn flagCol = new DataColumn();
                    flagCol.ColumnName = "Action Type";
                    flagCol.DataType = typeof(string);

                    dt.Columns.Add(flagCol);

                    dt.AsEnumerable().ToList().ForEach(x => x["Action Type"] = "Added");

                    dt.AsEnumerable().Where(y => !string.IsNullOrEmpty(Convert.ToString(y["entity_code"]))).ToList().ForEach(x => x["Action Type"] = "Modified");
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetLegJSONFromQueueID -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetLegJSONFromQueueID -> End");
            }

            return dt;
        }

        public GridData GetLegData(int queueId, string userName)
        {
            DataTable dt = new DataTable();

            //Start Vikas Bhat, 05-Aug-2017
            //Get Leg Data in Tabular Form from JSON 
            dt = GetLegDataFromQueueID(queueId, dt);
            //End Vikas Bhat, 05-Aug-2017

            if (!dt.Columns.Contains("row_id"))
                dt.Columns.Add("row_id");
            if (!dt.Columns.Contains("row_keys"))
                dt.Columns.Add("row_keys");

            int index = 0;
            foreach (DataRow dr in dt.Rows)
            {
                dr["row_id"] = index;
                dr["row_keys"] = index++;
            }

            GridData objGridData = new GridData();

            string guid = "s" + Guid.NewGuid().ToString().Replace('-', '_');
            objGridData = new GridData();
            objGridData.gridID = guid + "_gridId";
            objGridData.currentPageId = guid + "_123";
            objGridData.viewKey = guid + "_789";
            objGridData.sessionIdentifier = guid;
            dt.TableName = "DataTable";

            #region Prepare Grid Info
            GridInfo gridInfo = new GridInfo
            {
                SelectRecordsAcrossAllPages = null,
                CacheGriddata = true,
                UserId = userName,
                Height = "350px",
                ColumnsWithoutClientSideFunctionality = new List<string>(),
                RequireEditGrid = false,
                RequireEditableRow = false,
                IdColumnName = "row_id",
                KeyColumns = new Dictionary<KeyType, string> { { KeyType.RowKey, "row_keys" } },
                TableName = "DataTable",
                PageSize = 50,
                RequirePaging = false,
                RequireInfiniteScroll = true,
                CollapseAllGroupHeader = false,
                //GridTheme = Theme.MasterChildGridTheme,
                DoNotExpand = false,
                ItemText = "Number of Records",
                DoNotRearrangeColumn = true,
                RequireGrouping = true,
                RequireFilter = true,
                RequireSort = true,
                RequireMathematicalOperations = false,
                RequireSelectedRows = true,
                RequireExportToExcel = true,
                RequireSearch = true,
                RequireFreezeColumns = false,
                RequireHideColumns = true,
                RequireColumnSwap = true,
                RequireGroupExpandCollapse = true,
                RequireResizing = true,
                RequireLayouts = false,
                RequireGroupHeaderCheckbox = true,
                RequireRuleBasedColoring = false,
                RequireExportToPdf = false,
                ShowRecordCountOnHeader = false,
                ShowAggregationOnHeader = true,
                ColumnsNotToSum = new List<string>(),
                CheckBoxInfo = null,
                RaiseGridCallBackBeforeExecute = "",
                RaiseGridRenderComplete = "workflowstatus.gridRenderComplete",
                DefaultGroupedAndSortedColumns = new List<SortInfo>(),
                ColumnsToHide = new List<HiddenColumnInfo>() { new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "row_keys", isDefault = true }, new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "entity_code", isDefault = true } },
                CustomFormatInfoClientSide = new Dictionary<string, FormatterInfo>()
            };
            #endregion

            gridInfo.ViewKey = objGridData.viewKey;
            gridInfo.GridId = objGridData.gridID;
            gridInfo.CurrentPageId = objGridData.currentPageId;
            gridInfo.SessionIdentifier = objGridData.sessionIdentifier;

            RADNeoGridService service = new RADNeoGridService();
            service.SaveGridDataInCache(dt, userName, objGridData.gridID, objGridData.currentPageId, objGridData.viewKey, objGridData.sessionIdentifier, false, gridInfo);

            return objGridData;
        }

        public ResetGridOutput ResetGridData(WorkflowGridResetterInput inputObject)
        {
            GridInfo gridInfo = GetGridInfo(inputObject.userName, inputObject.selectedTab, inputObject.isGrouped);

            ResetGridOutput objResetGridOutput = new ResetGridOutput();
            string guid = "s" + Guid.NewGuid().ToString().Replace('-', '_');

            objResetGridOutput.gridID = guid + "_" + inputObject.gridTypeStr + "Div";
            objResetGridOutput.currentPageId = guid + "_123";
            objResetGridOutput.viewKey = guid + "_789";
            objResetGridOutput.sessionIdentifier = guid;
            try
            {
                WorkflowGridType gridType = (WorkflowGridType)Enum.Parse(typeof(WorkflowGridType), inputObject.gridTypeStr);
                DataSet dsGridData = WorkflowStatusDataHandler.GetGridData(inputObject.userSessionIdentifier, gridType, inputObject.gridId);

                WorkflowStatusDataHandler.ClearGridDataForGridId(inputObject.userSessionIdentifier, gridType, inputObject.gridId);
                WorkflowStatusDataHandler.SetGridData(inputObject.userSessionIdentifier, gridType, objResetGridOutput.gridID, dsGridData);

                List<CustomRowDataInfo> CustomRowDataInfo = GetCustomRowDataInfo(dsGridData, inputObject.selectedTab, inputObject.userSessionIdentifier, string.Empty, string.Empty, false);

                gridInfo.ViewKey = objResetGridOutput.viewKey;
                gridInfo.GridId = objResetGridOutput.gridID;
                gridInfo.CurrentPageId = objResetGridOutput.currentPageId;
                gridInfo.SessionIdentifier = objResetGridOutput.sessionIdentifier;
                gridInfo.CustomRowsDataInfo = CustomRowDataInfo;

                if (dsGridData != null)
                {
                    RADNeoGridService service = new RADNeoGridService();
                    service.SaveGridDataInCache(dsGridData.Tables[0], inputObject.userName, objResetGridOutput.gridID, objResetGridOutput.currentPageId, objResetGridOutput.viewKey, objResetGridOutput.sessionIdentifier, false, gridInfo);
                    objResetGridOutput.status = true;
                }
                else
                {
                    objResetGridOutput.status = false;
                }
            }
            catch (Exception ex)
            {
                objResetGridOutput.status = false;
            }
            return objResetGridOutput;
        }

        public void ClearGridDataForUser(string userSessionIdentifier)
        {
            WorkflowStatusDataHandler.ClearGridDataForCacheKey(userSessionIdentifier);
        }

        public ViewLogOutput GetWorkflowRequestLog(int queueId, bool getPending, string moduleName)
        {
            ViewLogOutput objViewLogOutput = new ViewLogOutput();
            objViewLogOutput.isGridAvailable = false;
            StringBuilder result = new StringBuilder();
            StringBuilder resultTop = new StringBuilder();
            bool isHistoryAvailable = false;

            result.Append("<div class='workflowEmptyLogDiv'>No action performed on request</div>");

            bool status = true;
            DataSet ds = null;

            if (moduleName == REFMASTER)
                ds = WorkflowController.GetRMWorkflowRequestLog(queueId, getPending, out status);
            else
                ds = WorkflowController.GetWorkflowRequestLog(queueId, getPending, out status);

            Dictionary<int, List<string>> workflowLevelVsUsers = new Dictionary<int, List<string>>();

            if (status)
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    try
                    {
                        if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[1].Rows)
                            {
                                int workflowSequence = Convert.ToInt32(dr[WorkflowConstants.WORKFLOW_SEQUENCE]);
                                if (!workflowLevelVsUsers.ContainsKey(workflowSequence))
                                    workflowLevelVsUsers[workflowSequence] = new List<string>();
                                workflowLevelVsUsers[workflowSequence].Add(Convert.ToString(dr["workflow_group_user_name"]));
                            }
                        }

                        result.Length = 0;
                        resultTop.Length = 0;
                        result.Append("<div class='workflowViewLogContainer'><div class='workflowViewLogDiv'>");
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            if (!Convert.ToBoolean(dr[WorkflowConstants.IS_ACTIVE]))
                            {
                                isHistoryAvailable = true;
                                string action = "";
                                string actionClass = "";
                                string leftStatusClass = string.Empty;
                                if (Convert.ToBoolean(dr[WorkflowConstants.IS_APPROVED]))
                                {
                                    action = "APPROVED";
                                    actionClass = "workflowLogApproved";
                                    leftStatusClass = " fa fa-check";
                                }
                                else if (Convert.ToBoolean(dr[WorkflowConstants.IS_REJECTED]))
                                {
                                    action = "REJECTED";
                                    actionClass = "workflowLogRejected";
                                    leftStatusClass = " fa fa-times";
                                }
                                else if (Convert.ToBoolean(dr[WorkflowConstants.IS_DELETED]))
                                {
                                    action = "DELETED";
                                    actionClass = "workflowLogDeleted";
                                }
                                else if (Convert.ToBoolean(dr[WorkflowConstants.IS_SUPPRESSED]))
                                {
                                    action = "SUPPRESSED";
                                    actionClass = "workflowLogSuppressed";
                                }
                                else
                                {
                                    action = "PENDING";
                                    actionClass = "workflowLogPending";
                                }
                                string actionBy = (dr[WorkflowConstants.ACTION_BY] != DBNull.Value) ? Convert.ToString(dr[WorkflowConstants.ACTION_BY]) : "";
                                DateTime? actionOn = (dr[WorkflowConstants.ACTION_ON] != DBNull.Value) ? (DateTime?)Convert.ToDateTime(dr[WorkflowConstants.ACTION_ON]) : null;
                                string remarks = (dr[WorkflowConstants.REMARKS] != DBNull.Value) ? Convert.ToString(dr[WorkflowConstants.REMARKS]) : "";
                                string workflowSequence = Convert.ToInt32(dr[WorkflowConstants.WORKFLOW_SEQUENCE]).ToString("00");

                                result.Append("<div class='workflowLogActionDivs ");
                                result.Append(actionClass);
                                result.Append("'><div class='workflowLogLeftStatus");
                                result.Append(leftStatusClass);
                                result.Append("'></div><div class='workflowLogInner'><div class='workflowLogAction'>");
                                result.Append(action);
                                result.Append("</div><div class='workflowLogActionBy'><div class='workflowLogActionByUpper'>Action by</div><div class='workflowLogActionByLower' title='");
                                result.Append(actionBy);
                                result.Append("'>");
                                result.Append(actionBy);
                                result.Append("</div></div><div class='workflowLogRemarks'><div class='workflowLogRemarksUpper'>Remarks</div><div class='workflowLogRemarksLower' title='");
                                result.Append(remarks);
                                result.Append("'>");
                                result.Append(remarks);
                                result.Append("</div></div><div class='workflowLogWorkflowLevel'><div class='workflowLogWorkflowLevelText'>Workflow Level</div><div class='workflowLogWorkflowLevelCount'>");
                                result.Append(workflowSequence);
                                result.Append("</div></div></div><div class='workflowLogActionOn'><div class='workflowLogActionDate'>");
                                result.Append(((actionOn == null) ? "" : ((DateTime)actionOn).ToString("MM/dd/yyyy")));
                                result.Append("</div><div class='workflowLogActionTime'>");
                                result.Append(((actionOn == null) ? "" : ((DateTime)actionOn).ToString("HH:mm:ss")));
                                result.Append("</div></div></div>");
                            }
                            else
                            {
                                int workflowSequence = Convert.ToInt32(dr[WorkflowConstants.WORKFLOW_SEQUENCE]);
                                if (workflowLevelVsUsers != null && workflowLevelVsUsers.Count > 0 && workflowLevelVsUsers.ContainsKey(workflowSequence) && workflowLevelVsUsers[workflowSequence].Count > 0)
                                {
                                    isHistoryAvailable = true;
                                    string workflowGroupsUsersShown = string.Empty;
                                    string workflowGroupsUsersHidden = string.Empty;
                                    string actionClass = "workflowLogCurrentLevel";
                                    bool isNextLevel = Convert.ToBoolean(dr["next_levels"]);
                                    bool isHidden = false;
                                    int index = 0;
                                    foreach (string user in workflowLevelVsUsers[workflowSequence])
                                    {
                                        if (index < 3)
                                        {
                                            workflowGroupsUsersShown += user + ", ";
                                        }
                                        else
                                        {
                                            isHidden = true;
                                            workflowGroupsUsersHidden += user + ",";
                                        }
                                        index++;
                                    }
                                    if (workflowGroupsUsersShown.Length > 0)
                                        workflowGroupsUsersShown = workflowGroupsUsersShown.Substring(0, workflowGroupsUsersShown.Length - 2);
                                    if (workflowGroupsUsersHidden.Length > 0)
                                        workflowGroupsUsersHidden = workflowGroupsUsersHidden.Substring(0, workflowGroupsUsersHidden.Length - 1);
                                    if (isNextLevel)
                                        actionClass = "workflowLogNextLevels";

                                    result.Append("<div class='workflowLogActionDivs ");
                                    result.Append(actionClass);
                                    result.Append("'><div class='workflowLogLeftStatus'></div><div class='workflowLogInner'><div class='workflowLogUsers'>Pending at ");
                                    result.Append(workflowGroupsUsersShown);
                                    //resultTop.Append("<div class='workflowLogPendingDiv'>Pending at ");
                                    //resultTop.Append(workflowGroupsUsersShown);
                                    if (isHidden)
                                    {
                                        result.Append("<div class='WorkflowMoreUsers' users='");
                                        result.Append(workflowGroupsUsersHidden);
                                        result.Append("'>+ ");
                                        result.Append(workflowLevelVsUsers[workflowSequence].Count - 3);
                                        result.Append(" more</div>");
                                        //resultTop.Append("<div class='WorkflowMoreUsers' users='");
                                        //resultTop.Append(workflowGroupsUsersHidden);
                                        //resultTop.Append("'>+ ");
                                        //resultTop.Append(workflowLevelVsUsers[workflowSequence].Count - 3);
                                        //resultTop.Append(" more</div>");
                                    }
                                    result.Append("</div></div></div>");
                                    //resultTop.Append("</div>");
                                }
                            }
                        }
                        result.Append("</div></div>");
                        if (!isHistoryAvailable)
                        {
                            result.Length = 0;
                            result.Append("<div class='workflowEmptyLogDiv'>No action performed on request</div>");
                        }
                        else
                            objViewLogOutput.isGridAvailable = true;
                    }
                    catch (Exception ex)
                    {
                        result.Length = 0;
                        result.Append("<div class='workflowEmptyLogDiv'>Error occured while preparing log : " + ex.ToString() + "</div>");
                    }
                }
            }
            else
            {
                result.Length = 0;
                result.Append("<div class='workflowEmptyLogDiv'>Error occured while fetching data</div>");
            }
            objViewLogOutput.html = result.ToString(); //"<div class='panelHead'>Action History</div>" + result.ToString();
            objViewLogOutput.htmlTop = resultTop.ToString();
            return objViewLogOutput;
        }

        private List<CustomRowDataInfo> GetCustomRowDataInfo(DataSet ds, string moduleName, string userSessionIdentifier, string gridType, string gridId, bool isFirstTimeBind = true)
        {
            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            List<CustomCellDatainfo> lstCustomCellDataInfo = new List<CustomCellDatainfo>();
            CustomRowDataInfo objCustomRowDataInfo = null;
            DataTable dt = ds.Tables[0];
            if (!dt.Columns.Contains("row_keys"))
                dt.Columns.Add("row_keys");

            string guid = "s" + Guid.NewGuid().ToString().Replace('-', '_');

            string entityRealName = "security_id";
            string entityColumn = "Security";
            string entityIdColumn = "Security Id";
            string entityNameColumn = "Security Name";
            if (moduleName == REFMASTER)
            {
                entityRealName = "entity_code";
                entityColumn = "Entity";
                entityIdColumn = "Entity Code";
                entityNameColumn = "Entity Name";
            }

            foreach (DataRow dr in dt.Rows)
            {
                string SecurityId = Convert.ToString(dr[entityIdColumn]);
                string row_id = Convert.ToString(dr["row_id"]);
                string queueId = Convert.ToString(dr["Action History"]);

                if (isFirstTimeBind)
                    dr[entityColumn] = dr[entityNameColumn] + "|" + dr[entityColumn] + "|" + userSessionIdentifier + "|" + gridType + "|" + gridId;

                objCustomRowDataInfo = new CustomRowDataInfo();
                dr["row_keys"] = row_id + "|" + SecurityId + "|" + Convert.ToString(dr["Attribute"]) + "|" + Convert.ToString(dr["Effective Start Date"]) + "|" + Convert.ToString(dr["Effective To Date"]);
                lstCustomCellDataInfo = new List<CustomCellDatainfo>();
                objCustomRowDataInfo.RowID = row_id;

                lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = entityIdColumn, NewChild = "<div id='divSecurityId_" + SecurityId + "_" + row_id + "_" + guid + "' class='secIdClick' queueIds='" + queueId + "' gridType='" + gridType + "' style='text-decoration:underline;cursor:pointer;color:#48a3dd;' secId='" + SecurityId + "'>" + SecurityId + "</div>" });

                lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Action History", NewChild = "<div class='workflowViewLogClick' queueId='" + queueId + "'>View Action History</div>" });
                objCustomRowDataInfo.Attribute.Add(entityRealName, SecurityId);
                if (moduleName == REFMASTER && Convert.ToString(dr["Attribute"]).Equals("Leg Level", StringComparison.OrdinalIgnoreCase))
                {
                    lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Attribute Value", NewChild = "<div class='workflowAttributeValueClick' queueId='" + queueId + "'>View leg data</div>" });
                }

                objCustomRowDataInfo.Cells = lstCustomCellDataInfo;
                lstCustomRowDataInfo.Add(objCustomRowDataInfo);
            }
            return lstCustomRowDataInfo;
        }

        private void PopulatePageConfiguredAttributes(ref DataSet ds, string userName)
        {
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int index = 0; index < ds.Tables.Count - 1; index++)
                //foreach (DataTable dt in ds.Tables)
                {
                    DataTable dt = ds.Tables[index];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        Assembly SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
                        Type objTypeSMBulkSecurityController = SecMasterCoreAssembly.GetType("com.ivp.secm.core.SMBulkSecurityController");
                        MethodInfo getPageConfiguredSecurityDetails = objTypeSMBulkSecurityController.GetMethod("GetPageConfiguredSecurityDetails");

                        var secIds = dt.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>("Security Id"))).GroupBy(x => x.Field<string>("Security Id")).ToDictionary(key => key.Key, value => value.ToList(), StringComparer.OrdinalIgnoreCase);

                        var objSMBulkSecurityController = Activator.CreateInstance(objTypeSMBulkSecurityController); //new SMBulkSecurityController();
                        DataSet securityDetailConfAttrs = (DataSet)getPageConfiguredSecurityDetails.Invoke(objSMBulkSecurityController, new object[] { string.Join("|", secIds.Keys), "|", "Workflow", "Workflow Inbox" }); //new SMBulkSecurityController().GetPageConfiguredSecurityDetails(string.Join("|", secIds.Keys), "|", "Workflow", "Workflow Inbox");
                        if (securityDetailConfAttrs != null && securityDetailConfAttrs.Tables.Count > 0 && securityDetailConfAttrs.Tables[0] != null)// && securityDetailConfAttrs.Tables[0].Rows.Count > 0
                        {
                            int secIdColumnPosition = dt.Columns["Security Type"].Ordinal;
                            foreach (DataColumn dcCol in securityDetailConfAttrs.Tables[0].Columns)
                            {
                                if (!dcCol.ColumnName.Equals("Security Id", StringComparison.OrdinalIgnoreCase) && !dcCol.ColumnName.Equals("sec_id", StringComparison.OrdinalIgnoreCase) && !dcCol.ColumnName.Equals("Security Type", StringComparison.OrdinalIgnoreCase) && !dcCol.ColumnName.Equals("sectype_id", StringComparison.OrdinalIgnoreCase) && !dt.Columns.Contains(dcCol.ColumnName))
                                {
                                    dt.Columns.Add(dcCol.ColumnName, dcCol.DataType);
                                    secIdColumnPosition++;
                                    dt.Columns[dcCol.ColumnName].SetOrdinal(secIdColumnPosition);
                                }
                            }

                            foreach (DataRow drRow in securityDetailConfAttrs.Tables[0].Rows)
                            {
                                string secId = drRow.Field<string>("sec_id");
                                if (secIds.ContainsKey(secId))
                                {
                                    foreach (var rows in secIds[secId])
                                    {
                                        foreach (DataColumn dcCol in securityDetailConfAttrs.Tables[0].Columns)
                                        {
                                            if (!dcCol.ColumnName.Equals("Security Id", StringComparison.OrdinalIgnoreCase) && !dcCol.ColumnName.Equals("sec_id", StringComparison.OrdinalIgnoreCase) && !dcCol.ColumnName.Equals("Security Type", StringComparison.OrdinalIgnoreCase) && !dcCol.ColumnName.Equals("sectype_id", StringComparison.OrdinalIgnoreCase))
                                            {
                                                rows[dcCol.ColumnName] = drRow[dcCol.ColumnName];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<string> GetCheckedRowKeys(string cacheKey, int gridType)
        {
            List<string> lstResult = null;
            GridType gridTypeE = (GridType)gridType;
            switch (gridTypeE)
            {
                case GridType.RADNEOGrid:
                    RADNeoGridService neoGridService = new RADNeoGridService();
                    lstResult = neoGridService.GetCheckedRowKeys(cacheKey);
                    break;
            }
            return lstResult == null ? new List<string>() : lstResult;
        }

        private string RMGetImpactedHTML(List<RMFinalApprovalEntity> response)
        {
            string finalHTML = string.Empty;

            if (response != null && response.Any(x => !x.IsSuccess || x.hasAlert))
            {
                //objResponse.IsImpactedCheck = true;
                HashSet<string> passedEntities = new HashSet<string>();
                List<string> procEntities = new List<string>();

                response.ForEach(r =>
                {
                    if (!response.Any(rr => rr.EntityCode == r.EntityCode && (!rr.IsSuccess && !rr.hasAlert)))
                        passedEntities.Add(r.EntityCode);
                });

                StringBuilder result = new StringBuilder();

                //result.Append("<div style='float: right; padding-right: 13px; margin-top: 9px;'><input type='button' id='btnPerformAction' class='button' tooltip='Click to Process' value='Process' /></div>");

                //Start Passed Entities
                if (passedEntities != null && passedEntities.Count > 0)
                {
                    if (rmProcessInfo != null && rmProcessInfo.Count > 0)
                    {
                        passedEntities.ToList().ForEach(pe =>
                        {
                            if (rmProcessInfo.Any(p => p.EntityCode == pe))
                                rmProcessInfo.Where(pp => pp.EntityCode == pe).ToList().ForEach(t => { t.isProcessed = true; });
                        });
                    }

                    result.Append("<div style='float: right; padding-right: 13px; margin-top: 9px;'><input type='button' id='btnPerformActionRM' class='button' tooltip='Click to Process' value='Process' /></div>");

                    result.Append("<div class='panelHead workflowImpactedSectionsHeader'>Passed Entities</div>");

                    result.Append("<div class='workflowImpactedScrollContainer'><div class='workflowImpactedTableContainer'><table id='workflowImpactedTablePassed' style='width: 100%; border-collapse: collapse; table-layout: fixed; margin-top: 5px; border: none;' border='0' cellspacing='0'><thead><tr class='header'>");

                    result.Append("<th title='Entity Code'>Entity Code</th><th title='Attribute'>Attribute</th><th title='Alert Message'>Alert Message</th>");

                    result.Append("</tr></thead><tbody>");
                    string rowClass = "normalRow";

                    response.Where(x => passedEntities.Contains(x.EntityCode)).OrderBy(x => x.EntityCode).ToList().ForEach(res =>
                    {
                        if (!procEntities.Contains(res.EntityCode) || (res.FailureReasons != null && res.FailureReasons.Count > 0))
                        {
                            procEntities.Add(res.EntityCode);
                            result.Append("<tr class='");
                            result.Append(rowClass);
                            result.Append("'>");

                            result.Append("<td title='" + res.EntityCode + "'>" + res.EntityCode + "</td>");
                            result.Append("<td title='" + res.AttributeDisplayName + "'>" + res.AttributeDisplayName + "</td>");

                            if (res.FailureReasons != null && !string.IsNullOrEmpty(res.FailureReasons[0]))
                                result.Append("<td title='" + res.FailureReasons[0].Replace("'", "''") + "'>" + res.FailureReasons[0] + "</td>");
                            else
                                result.Append("<td></td>");

                            //if (res.hasAlert)
                            //    result.Append("<td title='Alert'>Alert</td>");
                            //else
                            //    result.Append("<td></td>");


                            result.Append("</tr>");
                            if (rowClass == "normalRow")
                                rowClass = "alternatingRow";
                            else
                                rowClass = "normalRow";
                        }

                    });

                    result.Append("</tbody></table></div></div>");
                }
                //End Passed Entities

                //Start Failed Entities
                if (passedEntities == null || response.Select(resp => resp.EntityCode).Distinct().Count() > passedEntities.Count)
                {
                    result.Append("<div class='panelHead workflowImpactedSectionsHeader'>Failed Entities</div>");

                    result.Append("<div class='workflowImpactedScrollContainer'><div class='workflowImpactedTableContainer'><table id='workflowImpactedTablePassed' style='width: 100%; border-collapse: collapse; table-layout: fixed; margin-top: 5px; border: none;' border='0' cellspacing='0'><thead><tr class='header'>");

                    result.Append("<th title='Entity Code'>Entity Code</th><th title='Attribute'>Attribute</th><th title='Failure Reason'>Failure Reason</th><th title='Type'>Failure Type</th>");

                    result.Append("</tr></thead><tbody>");
                    string rowClass = "normalRow";

                    response.Where(x => !passedEntities.Contains(x.EntityCode) && (!x.IsSuccess || (x.IsSuccess && x.hasAlert))).OrderBy(x => x.EntityCode).ToList().ForEach(res =>
                    {

                        result.Append("<tr class='");
                        result.Append(rowClass);
                        result.Append("'>");

                        result.Append("<td title='" + res.EntityCode + "'>" + res.EntityCode + "</td>");
                        result.Append("<td title='" + res.AttributeDisplayName + "'>" + res.AttributeDisplayName + "</td>");

                        if (res.FailureReasons != null && !string.IsNullOrEmpty(res.FailureReasons[0]))
                            result.Append("<td title='" + res.FailureReasons[0].Replace("'", "''") + "'>" + res.FailureReasons[0] + "</td>");
                        else
                            result.Append("<td></td>");

                        if (!res.hasAlert)
                            result.Append("<td title='Validation'>Validation</td>");
                        else
                            result.Append("<td title='Alert'>Alert</td>");


                        result.Append("</tr>");
                        if (rowClass == "normalRow")
                            rowClass = "alternatingRow";
                        else
                            rowClass = "normalRow";

                    });

                    result.Append("</tbody></table></div></div>");
                }
                //End Failed Entities

                finalHTML = result.ToString();
            }

            return finalHTML;
        }

        public WorkflowImpactedSecuritiesResponse WorkflowImpactedSecuritiesCheck(List<string> rowKeys, string userName, string remarks, int action, string moduleName)
        {
            WorkflowImpactedSecuritiesResponse objResponse = new WorkflowImpactedSecuritiesResponse() { IsActionComplete = false, IsImpactedCheck = false };

            WorkflowActionEnum actionType = (WorkflowActionEnum)action;

            try
            {
                if (moduleName == REFMASTER)
                {
                    List<int> statusIds = new List<int>();
                    objResponse.IsActionComplete = true;
                    objResponse.IsImpactedCheck = true;
                    objResponse.guid = Guid.NewGuid().ToString();
                    objResponse.handlerResponse = new WorkflowHandlerResponseInfo();
                    objResponse.handlerResponse.IsSuccess = true;
                    objResponse.handlerResponse.FailureMessage = string.Empty;

                    rowKeys.ForEach(key =>
                    {
                        if (key.Split('|').Length > 1)
                            statusIds.Add(Convert.ToInt32(key.Split('|')[0]));
                    });

                    if (statusIds != null && statusIds.Count > 0)
                    {
                        rmProcessInfo = new List<RMProcessedEntityInfo>();
                        Dictionary<WorkflowHandlerResponseInfo, List<RMFinalApprovalEntity>> RMResponse = WorkflowController.RMWorkflowRequestHandler(statusIds, userName, remarks, action, null, rmProcessInfo);
                        if (RMResponse != null)
                        {
                            WorkflowHandlerResponseInfo rmHandlerResponse = RMResponse.Keys.FirstOrDefault();
                            objResponse.handlerResponse = rmHandlerResponse;

                            if (RMResponse[rmHandlerResponse] != null && RMResponse[rmHandlerResponse].Count > 0)
                            {
                                List<RMFinalApprovalEntity> impactResponse = RMResponse[rmHandlerResponse];
                                objResponse.ImpactedHTML = RMGetImpactedHTML(impactResponse);
                                if (!string.IsNullOrEmpty(objResponse.ImpactedHTML))
                                {
                                    objResponse.IsImpactedCheck = true;
                                    objResponse.IsActionComplete = false;

                                    List<int> finalStatusIds = new List<int>();
                                    if (statusIds != null && rmProcessInfo != null && rmProcessInfo.Any(r => r.isProcessed))
                                    {
                                        statusIds.ForEach(id =>
                                        {
                                            if (rmProcessInfo.Any(r => r.StatusID == id && r.isProcessed))
                                                finalStatusIds.Add(id);
                                        });
                                    }

                                    objResponse.statusIds = finalStatusIds;
                                }
                                else
                                {
                                    objResponse.IsImpactedCheck = false;
                                    objResponse.IsActionComplete = true;
                                    objResponse.statusIds = statusIds;
                                }
                            }
                        }

                        //objResponse.handlerResponse = WorkflowController.RMWorkflowRequestHandler(statusIds, userName, remarks, action);
                    }
                }
                else
                {
                    if (actionType == WorkflowActionEnum.APPROVED)
                    {
                        bool containsNormalUpdate = true;
                        WorkflowDateRange objWorkflowDateRange = null;
                        List<string> IgnoredKeysList = new List<string>();
                        Dictionary<string, WorkflowDateRangePerAttribute> DictRequestDataPerSecurityAttribute = new Dictionary<string, WorkflowDateRangePerAttribute>();

                        objResponse.statusIds = new List<int>();
                        objResponse.handlerResponse = new WorkflowHandlerResponseInfo() { FailureMessage = "" };
                        foreach (string request in rowKeys)
                        {
                            string[] requestItems = request.Split('|');

                            objResponse.statusIds.Add(Convert.ToInt32(requestItems[0]));

                            string SecurityAttributeKey = requestItems[1] + "|" + requestItems[2];
                            if (IgnoredKeysList.Contains(SecurityAttributeKey))
                                continue;

                            objWorkflowDateRange = new WorkflowDateRange();
                            if (!DictRequestDataPerSecurityAttribute.ContainsKey(SecurityAttributeKey))
                            {
                                DictRequestDataPerSecurityAttribute[SecurityAttributeKey] = new WorkflowDateRangePerAttribute() { ContainsNormalUpdate = false, TimeSeriesRanges = null };
                            }

                            objWorkflowDateRange.StartDate = null;
                            objWorkflowDateRange.EndDate = DateTime.Today;
                            bool normalUpdate = true;
                            if (!string.IsNullOrEmpty(requestItems[3]))
                            {
                                normalUpdate = false;
                                objWorkflowDateRange.StartDate = Convert.ToDateTime(requestItems[3]);
                            }
                            if (!string.IsNullOrEmpty(requestItems[4]))
                            {
                                normalUpdate = false;
                                objWorkflowDateRange.EndDate = Convert.ToDateTime(requestItems[4]);
                            }
                            if (normalUpdate)
                            {
                                containsNormalUpdate = normalUpdate;
                                if (DictRequestDataPerSecurityAttribute[SecurityAttributeKey].ContainsNormalUpdate)
                                {
                                    objResponse.IsActionComplete = true;
                                    objResponse.handlerResponse.IsSuccess = false;
                                    objResponse.handlerResponse.FailureMessage += string.Format("More than one normal update for security : {0} and attribute : {1}</br>", requestItems[1], requestItems[2]);
                                    IgnoredKeysList.Add(SecurityAttributeKey);
                                }
                                else
                                {
                                    DictRequestDataPerSecurityAttribute[SecurityAttributeKey].ContainsNormalUpdate = normalUpdate;
                                }
                            }
                            else
                            {
                                if (DictRequestDataPerSecurityAttribute[SecurityAttributeKey].TimeSeriesRanges == null)
                                    DictRequestDataPerSecurityAttribute[SecurityAttributeKey].TimeSeriesRanges = new Dictionary<int, WorkflowDateRangeComparer>();

                                if (!AddWorkflowDateRange(DictRequestDataPerSecurityAttribute[SecurityAttributeKey].TimeSeriesRanges, objWorkflowDateRange))
                                {
                                    objResponse.IsActionComplete = true;
                                    objResponse.handlerResponse.IsSuccess = false;
                                    objResponse.handlerResponse.FailureMessage += string.Format("Time series range overlapping for security : {0} and attribute : {1}</br>", requestItems[1], requestItems[2]);
                                    IgnoredKeysList.Add(SecurityAttributeKey);
                                }
                            }
                        }
                        if (objResponse.IsActionComplete)
                            return objResponse;
                        if (containsNormalUpdate)
                        {
                            string guid;
                            DataSet dsImpacted = WorkflowController.GetImpactedSecurities(objResponse.statusIds, userName, out guid);
                            objResponse.guid = guid;

                            if (dsImpacted != null && dsImpacted.Tables.Count > 1 && (dsImpacted.Tables[0].Rows.Count > 0 || dsImpacted.Tables[1].Rows.Count > 0))
                            {
                                objResponse.IsImpactedCheck = true;

                                StringBuilder result = new StringBuilder();
                                if (dsImpacted.Tables[0].Rows.Count > 0)
                                {
                                    result.Append("<div style='float: right; padding-right: 13px; margin-top: 9px;'><input type='button' id='btnPerformAction' class='button' tooltip='Click to Process' value='Process' /></div>");
                                    if (dsImpacted.Tables[1].Rows.Count > 0)
                                        result.Append("<div class='panelHead workflowImpactedSectionsHeader'>Passed Attributes</div>");
                                    else
                                        result.Append("<div class='panelHead workflowImpactedSectionsHeader' style='visibility: hidden;'>Passed Attributes</div>");
                                    dsImpacted.Tables[0].Columns["Attribute/Leg Name"].ColumnName = "Attribute";
                                    int OldValueOrdinal = dsImpacted.Tables[0].Columns["Old Value"].Ordinal;
                                    int NewValueOrdinal = dsImpacted.Tables[0].Columns["New Value"].Ordinal;
                                    dsImpacted.Tables[0].Columns["Old Value"].SetOrdinal(NewValueOrdinal);
                                    dsImpacted.Tables[0].Columns["New Value"].SetOrdinal(OldValueOrdinal);
                                    result.Append("<div class='workflowImpactedScrollContainer'><div class='workflowImpactedTableContainer'><table id='workflowImpactedTablePassed' style='width: 100%; border-collapse: collapse; table-layout: fixed; margin-top: 5px; border: none;' border='0' cellspacing='0'><thead><tr class='header'>");
                                    foreach (DataColumn dc in dsImpacted.Tables[0].Columns)
                                    {
                                        if (!(dc.ColumnName.Equals("Attribute Id") || dc.ColumnName.Equals("Sectype Id") || dc.ColumnName.Equals("Attribute Value")))
                                        {
                                            result.Append("<th title='");
                                            result.Append(dc.ColumnName);
                                            result.Append("'>");
                                            result.Append(dc.ColumnName);
                                            result.Append("</th>");
                                        }
                                    }
                                    result.Append("</tr></thead><tbody>");
                                    string rowClass = "normalRow";
                                    foreach (DataRow dr in dsImpacted.Tables[0].Rows)
                                    {
                                        result.Append("<tr class='");
                                        result.Append(rowClass);
                                        result.Append("'>");
                                        foreach (DataColumn dc in dsImpacted.Tables[0].Columns)
                                        {
                                            if (!(dc.ColumnName.Equals("Attribute Id") || dc.ColumnName.Equals("Sectype Id") || dc.ColumnName.Equals("Attribute Value")))
                                            {
                                                string value = Convert.ToString(dr[dc.ColumnName]);
                                                result.Append("<td title='");
                                                result.Append(value);
                                                result.Append("'");
                                                if (dc.ColumnName.Equals("Security Id"))
                                                {
                                                    result.Append(" class='secIdClick' secId='");
                                                    result.Append(value);
                                                    result.Append("'");
                                                }
                                                result.Append(">");
                                                result.Append(value);
                                                result.Append("</td>");
                                            }
                                        }
                                        result.Append("</tr>");
                                        if (rowClass == "normalRow")
                                            rowClass = "alternatingRow";
                                        else
                                            rowClass = "normalRow";
                                    }
                                    result.Append("</tbody></table></div></div>");
                                }

                                if (dsImpacted.Tables[1].Rows.Count > 0)
                                {
                                    if (dsImpacted.Tables[0].Rows.Count > 0)
                                        result.Append("<div class='panelHead workflowImpactedSectionsHeader'>Failed Attributes</div>");
                                    dsImpacted.Tables[1].Columns["Attribute/Leg Name"].ColumnName = "Attribute";
                                    int OldValueOrdinal = dsImpacted.Tables[1].Columns["Old Value"].Ordinal;
                                    int NewValueOrdinal = dsImpacted.Tables[1].Columns["New Value"].Ordinal;
                                    dsImpacted.Tables[1].Columns["Old Value"].SetOrdinal(NewValueOrdinal);
                                    dsImpacted.Tables[1].Columns["New Value"].SetOrdinal(OldValueOrdinal);
                                    result.Append("<div class='workflowImpactedScrollContainer'><div class='workflowImpactedTableContainer'><table id='workflowImpactedTableFailed' style='width: 100%; border-collapse: collapse; table-layout: fixed; margin-top: 5px; border: none;' border='0' cellspacing='0'><thead><tr class='header'>");
                                    foreach (DataColumn dc in dsImpacted.Tables[1].Columns)
                                    {
                                        if (!(dc.ColumnName.Equals("Attribute Id") || dc.ColumnName.Equals("Sectype Id") || dc.ColumnName.Equals("Attribute Value")))
                                        {
                                            result.Append("<th title='");
                                            result.Append(dc.ColumnName);
                                            result.Append("'>");
                                            result.Append(dc.ColumnName);
                                            result.Append("</th>");
                                        }
                                    }
                                    result.Append("</tr></thead><tbody>");
                                    string rowClass = "normalRow";
                                    foreach (DataRow dr in dsImpacted.Tables[1].Rows)
                                    {
                                        result.Append("<tr class='");
                                        result.Append(rowClass);
                                        result.Append("'>");
                                        foreach (DataColumn dc in dsImpacted.Tables[1].Columns)
                                        {
                                            if (!(dc.ColumnName.Equals("Attribute Id") || dc.ColumnName.Equals("Sectype Id") || dc.ColumnName.Equals("Attribute Value")))
                                            {
                                                string value = Convert.ToString(dr[dc.ColumnName]);
                                                result.Append("<td title='");
                                                result.Append(value);
                                                result.Append("'");
                                                if (dc.ColumnName.Equals("Security Id"))
                                                {
                                                    result.Append(" class='secIdClick' secId='");
                                                    result.Append(value);
                                                    result.Append("'");
                                                }
                                                result.Append(">");
                                                result.Append(value);
                                                result.Append("</td>");
                                            }
                                        }
                                        result.Append("</tr>");
                                        if (rowClass == "normalRow")
                                            rowClass = "alternatingRow";
                                        else
                                            rowClass = "normalRow";
                                    }
                                    result.Append("</tbody></table></div></div>");
                                }

                                objResponse.ImpactedHTML = result.ToString();
                            }
                            else
                            {
                                objResponse.IsActionComplete = true;
                                objResponse.handlerResponse = WorkflowRequestHandler(objResponse.statusIds, userName, remarks, action, objResponse.guid);
                                objResponse.guid = string.Empty;
                            }
                        }
                        else
                        {
                            objResponse.IsActionComplete = true;
                            objResponse.handlerResponse = WorkflowRequestHandler(objResponse.statusIds, userName, remarks, action, string.Empty);
                        }
                    }
                    else
                    {
                        objResponse.statusIds = rowKeys.Select(x => Convert.ToInt32(x.Split('|')[0])).ToList();
                        objResponse.IsActionComplete = true;
                        objResponse.handlerResponse = WorkflowRequestHandler(objResponse.statusIds, userName, remarks, action, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> WorkflowImpactedSecurities -> Error: " + ex);
                objResponse.IsActionComplete = false;
                objResponse.IsImpactedCheck = false;
                objResponse.handlerResponse = new WorkflowHandlerResponseInfo();
                objResponse.handlerResponse.IsSuccess = false;
                objResponse.handlerResponse.FailureMessage = ex.Message;
            }
            finally
            {
                mLogger.Debug("CommonService -> WorkflowImpactedSecurities -> End");
            }

            return objResponse;
        }

        private bool AddWorkflowDateRange(Dictionary<int, WorkflowDateRangeComparer> timeSeriesRanges, WorkflowDateRange objWorkflowDateRange)
        {
            int index = timeSeriesRanges.Count;
            if (index > 0)
            {
                return AddWorkflowDateRangeInner(timeSeriesRanges, objWorkflowDateRange, 0);
            }
            else
            {
                timeSeriesRanges[index] = new WorkflowDateRangeComparer() { DateRange = objWorkflowDateRange, HasLeftChild = false, HasRightChild = false, LeftChild = 0, RightChild = 0 };
                return true;
            }
        }

        private bool AddWorkflowDateRangeInner(Dictionary<int, WorkflowDateRangeComparer> timeSeriesRanges, WorkflowDateRange objWorkflowDateRange, int index)
        {
            WorkflowDateRangeComparer dateRangeParent = timeSeriesRanges[index];

            if (dateRangeParent.DateRange.StartDate > objWorkflowDateRange.EndDate)
            {
                if (dateRangeParent.HasLeftChild)
                {
                    return AddWorkflowDateRangeInner(timeSeriesRanges, objWorkflowDateRange, dateRangeParent.LeftChild);
                }
                else
                {
                    int indexElement = timeSeriesRanges.Count;
                    dateRangeParent.HasLeftChild = true;
                    dateRangeParent.LeftChild = indexElement;
                    timeSeriesRanges[indexElement] = new WorkflowDateRangeComparer() { DateRange = objWorkflowDateRange, HasLeftChild = false, HasRightChild = false, LeftChild = 0, RightChild = 0 };
                    return true;
                }
            }
            else if (objWorkflowDateRange.StartDate > dateRangeParent.DateRange.EndDate)
            {
                if (dateRangeParent.HasRightChild)
                {
                    return AddWorkflowDateRangeInner(timeSeriesRanges, objWorkflowDateRange, dateRangeParent.RightChild);
                }
                else
                {
                    int indexElement = timeSeriesRanges.Count;
                    dateRangeParent.HasRightChild = true;
                    dateRangeParent.RightChild = indexElement;
                    timeSeriesRanges[indexElement] = new WorkflowDateRangeComparer() { DateRange = objWorkflowDateRange, HasLeftChild = false, HasRightChild = false, LeftChild = 0, RightChild = 0 };
                    return true;
                }
            }
            else //if (dateRangeParent.DateRange.StartDate <= objWorkflowDateRange.EndDate && objWorkflowDateRange.StartDate <= dateRangeParent.DateRange.EndDate)
            {
                return false;
            }
        }

        public WorkflowHandlerResponseInfo WorkflowRequestHandler(List<int> statusIds, string userName, string remarks, int action, string guid)
        {
            return WorkflowController.WorkflowRequestHandler(statusIds, userName, HttpUtility.HtmlEncode(remarks), action, guid);
        }

        public Dictionary<WorkflowHandlerResponseInfo, List<RMFinalApprovalEntity>> RMWorkflowRequestHandler(List<int> statusIds, string userName, string remarks, int action, string guid)
        {
            return WorkflowController.RMWorkflowRequestHandler(statusIds, userName, remarks, action, null, null, true);

        }

        public List<ListItems> GetAllSectypes(List<string> selectedSectypes)
        {
            List<ListItems> lstObj = new List<ListItems>();
            DataTable dt = WorkflowController.GetAllSecTypeNames().Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ListItems temp = new ListItems();
                    string value = Convert.ToString(dr["sectype_id"]);
                    string text = Convert.ToString(dr["sectype_name"]);
                    temp.value = value;
                    temp.text = text;
                    if (selectedSectypes != null && selectedSectypes.Contains(value))
                    {
                        temp.isSelected = "true";
                    }
                    else
                    {
                        temp.isSelected = "false";
                    }
                    lstObj.Add(temp);
                }
            }
            return lstObj;
        }

        public List<ListItems> GetAllEntityTypes(List<string> selectedEntityTypes)
        {
            List<ListItems> lstObj = new List<ListItems>();
            DataTable dt = WorkflowController.GetAllEntityTypes().Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ListItems temp = new ListItems();
                    string value = Convert.ToString(dr["entity_type_id"]);
                    string text = Convert.ToString(dr["entity_display_name"]);
                    temp.value = value;
                    temp.text = text;
                    if (selectedEntityTypes != null && selectedEntityTypes.Contains(value))
                    {
                        temp.isSelected = "true";
                    }
                    else
                    {
                        temp.isSelected = "false";
                    }
                    lstObj.Add(temp);
                }
            }
            return lstObj;
        }

        public List<WorkFlowAttributeInfo> GetAllAttributesBasedOnSectypeSelection(string secTypeIds, string userName)
        {
            List<WorkFlowAttributeInfo> lstObj = new List<WorkFlowAttributeInfo>();
            DataSet ds = WorkflowController.GetAttributeBasedOnSecTypeSelection(secTypeIds, userName);
            DataTable dt = ds.Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WorkFlowAttributeInfo temp = new WorkFlowAttributeInfo();
                    temp.attrID = Convert.ToInt32(dr["attribute_id"]);
                    temp.attrName = Convert.ToString(dr["display_name"]);
                    lstObj.Add(temp);
                }
            }
            if (ds.Tables.Count > 1)
            {
                DataTable dt1 = ds.Tables[1];
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt1.Rows)
                    {
                        WorkFlowAttributeInfo temp = new WorkFlowAttributeInfo();
                        temp.attrID = Convert.ToInt32(dr["attribute_id"]);
                        temp.attrName = Convert.ToString(dr["display_name"]);
                        lstObj.Add(temp);
                    }
                }
            }
            return lstObj;
        }

        public List<WorkFlowAttributeInfo> GetAttributeBasedOnEntityTypeSelection(string entityTypeId)
        {
            List<WorkFlowAttributeInfo> lstObj = new List<WorkFlowAttributeInfo>();
            DataSet ds = WorkflowController.GetAttributeBasedOnEntityTypeSelection(entityTypeId);
            DataTable dt = ds.Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WorkFlowAttributeInfo temp = new WorkFlowAttributeInfo();
                    temp.attrID = Convert.ToInt32(dr["entity_attribute_id"]);
                    temp.attrName = Convert.ToString(dr["attribute_display_name"]);
                    lstObj.Add(temp);
                }
            }
            return lstObj;
        }

        public List<WorkFlowAttributeInfo> GetLegBasedOnEntityTypeSelection(string entityTypeId)
        {
            List<WorkFlowAttributeInfo> lstObj = new List<WorkFlowAttributeInfo>();
            DataSet ds = WorkflowController.GetEntityTypeLegs(Convert.ToInt32(entityTypeId));
            DataTable dt = ds.Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WorkFlowAttributeInfo temp = new WorkFlowAttributeInfo();
                    temp.attrID = Convert.ToInt32(dr["entity_type_id"]);
                    temp.attrName = Convert.ToString(dr["entity_display_name"]);
                    lstObj.Add(temp);
                }
            }
            return lstObj;
        }

        public List<WorkflowUsersGroups> GetAllUsersList()
        {
            List<WorkflowUsersGroups> tempList = new List<WorkflowUsersGroups>();
            DataSet ds = WorkflowController.GetAllUsers();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                WorkflowUsersGroups temp = new WorkflowUsersGroups();
                temp.name = Convert.ToString(dr["user_login_name"]) + "|" + Convert.ToString(dr["first_name"]) + "|" + Convert.ToString(dr["last_name"]);
                temp.level = 0;
                tempList.Add(temp);
            }
            return tempList;
        }

        public List<WorkflowUsersGroups> GetAllGroupsList()
        {
            List<WorkflowUsersGroups> tempList = new List<WorkflowUsersGroups>();
            DataSet ds = WorkflowController.GetAllGroups();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                WorkflowUsersGroups temp = new WorkflowUsersGroups();
                temp.name = Convert.ToString(dr["group_name"]);
                temp.level = 0;
                tempList.Add(temp);
            }
            return tempList;
        }

        public List<ListItems> RMGetWorkflowType()
        {
            List<ListItems> lstObj = new List<ListItems>();
            DataTable dt = WorkflowController.RMGetWorkflowType().Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ListItems temp = new ListItems();
                    string value = Convert.ToString(dr["workflow_type_id"]);
                    string text = Convert.ToString(dr["workflow_type_name"]);
                    temp.value = value;
                    temp.text = text;
                    lstObj.Add(temp);
                }
            }
            return lstObj;
        }

        public string SMSaveWorkflow(WorkflowInfo info, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank)
        {
            List<string> sectypes = info.workflowData.sectypeIDs.Split(',').ToList();
            Dictionary<int, List<KeyValuePair<string, bool>>> tempUserGroupInfo = new Dictionary<int, List<KeyValuePair<string, bool>>>(); //true for user and false for group

            //For Users
            foreach (WorkflowUsersGroups item in info.workflowData.usersInfo)
            {
                var level = item.level;
                var name = item.name;

                if (level != 0)
                {
                    if (tempUserGroupInfo.ContainsKey(level))
                    {
                        tempUserGroupInfo[level].Add(new KeyValuePair<string, bool>(name, true));
                    }
                    else
                    {
                        List<KeyValuePair<string, bool>> tempList = new List<KeyValuePair<string, bool>>();
                        KeyValuePair<string, bool> tempObj = new KeyValuePair<string, bool>(name, true);
                        tempList.Add(tempObj);
                        tempUserGroupInfo.Add(level, tempList);
                    }
                }
            }
            //For Groups
            foreach (WorkflowUsersGroups item in info.workflowData.groupInfo)
            {
                var level = item.level;
                var name = item.name;

                if (level != 0)
                {
                    if (tempUserGroupInfo.ContainsKey(level))
                    {
                        tempUserGroupInfo[level].Add(new KeyValuePair<string, bool>(name, false));
                    }
                    else
                    {
                        List<KeyValuePair<string, bool>> tempList = new List<KeyValuePair<string, bool>>();
                        KeyValuePair<string, bool> tempObj = new KeyValuePair<string, bool>(name, false);
                        tempList.Add(tempObj);
                        tempUserGroupInfo.Add(level, tempList);
                    }
                }
            }

            XDocument doc = new XDocument(
                                new XElement("workflowInfo", new XAttribute("sectype_ids", String.Join(",", sectypes)), new XAttribute("attribute_ids", String.Join(",", info.workflowData.attributeInfo.Select(x => x.attrID))),
                                        tempUserGroupInfo.Select(x => new XElement("level", new XAttribute("sequence", Convert.ToString(x.Key)), new XAttribute("users", String.Join(",", x.Value.Where(y => y.Value).Select(y => y.Key).ToArray<string>())), new XAttribute("groups", String.Join(",", String.Join(",", x.Value.Where(y => !y.Value).Select(y => y.Key).ToArray<string>()))))
                                                )
                                            )
                                        );

            return WorkflowController.SMSaveWorkflow(doc, workflowName, isCreate, applyTimeSeries, username, applyBlankToNonBlank);
        }

        public string RMSaveWorkflow(WorkflowInfo info, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank)
        {
            List<string> entityTypes = info.workflowData.entityTypeID.Split(',').ToList();
            Dictionary<int, List<KeyValuePair<string, bool>>> tempUserGroupInfo = new Dictionary<int, List<KeyValuePair<string, bool>>>(); //true for user and false for group

            //For Users
            foreach (WorkflowUsersGroups item in info.workflowData.usersInfo)
            {
                var level = item.level;
                var name = item.name;

                if (level != 0)
                {
                    if (tempUserGroupInfo.ContainsKey(level))
                    {
                        tempUserGroupInfo[level].Add(new KeyValuePair<string, bool>(name, true));
                    }
                    else
                    {
                        List<KeyValuePair<string, bool>> tempList = new List<KeyValuePair<string, bool>>();
                        KeyValuePair<string, bool> tempObj = new KeyValuePair<string, bool>(name, true);
                        tempList.Add(tempObj);
                        tempUserGroupInfo.Add(level, tempList);
                    }
                }
            }
            //For Groups
            foreach (WorkflowUsersGroups item in info.workflowData.groupInfo)
            {
                var level = item.level;
                var name = item.name;

                if (level != 0)
                {
                    if (tempUserGroupInfo.ContainsKey(level))
                    {
                        tempUserGroupInfo[level].Add(new KeyValuePair<string, bool>(name, false));
                    }
                    else
                    {
                        List<KeyValuePair<string, bool>> tempList = new List<KeyValuePair<string, bool>>();
                        KeyValuePair<string, bool> tempObj = new KeyValuePair<string, bool>(name, false);
                        tempList.Add(tempObj);
                        tempUserGroupInfo.Add(level, tempList);
                    }
                }
            }

            XDocument doc = null;
            if (info.workflowType == 1 || info.workflowType == 3)
            {
                doc = new XDocument(
                                    new XElement("workflowInfo", new XAttribute("sectype_ids", String.Join(",", entityTypes)), new XAttribute("attribute_ids", String.Join(",", info.workflowData.attributeInfo.Select(x => x.attrID))),
                                            tempUserGroupInfo.Select(x => new XElement("level", new XAttribute("sequence", Convert.ToString(x.Key)), new XAttribute("users", String.Join(",", x.Value.Where(y => y.Value).Select(y => y.Key).ToArray<string>())), new XAttribute("groups", String.Join(",", String.Join(",", x.Value.Where(y => !y.Value).Select(y => y.Key).ToArray<string>()))))
                                                    )
                                                )
                                            );
            }
            else
            {
                doc = new XDocument(
                                    new XElement("workflowInfo", new XAttribute("sectype_ids", String.Join(",", entityTypes)),
                                            tempUserGroupInfo.Select(x => new XElement("level", new XAttribute("sequence", Convert.ToString(x.Key)), new XAttribute("users", String.Join(",", x.Value.Where(y => y.Value).Select(y => y.Key).ToArray<string>())), new XAttribute("groups", String.Join(",", String.Join(",", x.Value.Where(y => !y.Value).Select(y => y.Key).ToArray<string>()))))
                                                    )
                                                )
                                            );
            }

            return WorkflowController.RMSaveWorkflow(doc, workflowName, isCreate, applyTimeSeries, username, applyBlankToNonBlank, info.workflowType);
        }

        public List<WorkflowInfo> RMGetAllWorkflows()
        {
            List<WorkflowInfo> finalWorkflowObj = new List<WorkflowInfo>();
            try
            {
                DataSet dsRM = WorkflowController.RMGetAllWorkflows();
                foreach (DataRow instance in dsRM.Tables[0].Rows)
                {
                    WorkflowInfo temp = new WorkflowInfo();
                    temp.moduleType = "REFMASTER";
                    temp.workflowType = Convert.ToInt32(instance["workflow_type_id"]);
                    temp.workflowName = instance.Field<string>("workflow_instance_name");
                    temp.workflowID = instance.Field<int>("workflow_instance_id");
                    temp.workflowData = new WorkflowData();
                    temp.workflowData.applyOnCreate = instance.Field<bool>("workflow_is_create");
                    temp.workflowData.applyTimeSeries = instance.Field<bool>("workflow_is_time_series");
                    temp.workflowData.applyOnBlankToNonblank = instance.Field<bool>("raise_for_non_empty_value");
                    temp.workflowData.entityTypeID = String.Join(",", dsRM.Tables[1].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id")).Select(x => Convert.ToString(x.Field<int>("entity_type_id"))).Distinct().ToArray<string>());


                    if (temp.workflowType == 1)
                    {
                        //Attributes
                        temp.workflowData.attributeInfo = new List<WorkFlowAttributeInfo>();
                        foreach (DataRow attribute in dsRM.Tables[1].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id") && x.Field<int>("entity_type_id") == Convert.ToInt32(temp.workflowData.entityTypeID)))
                        {
                            WorkFlowAttributeInfo tempAttr = new WorkFlowAttributeInfo();
                            tempAttr.attrID = attribute.Field<int>("attribute_id");
                            tempAttr.attrName = attribute.Field<string>("entity_display_name");
                            temp.workflowData.attributeInfo.Add(tempAttr);
                        }
                    }

                    if (temp.workflowType == 3)
                    {
                        //Legs
                        temp.workflowData.attributeInfo = new List<WorkFlowAttributeInfo>();
                        foreach (DataRow attribute in dsRM.Tables[1].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id") && x.Field<int>("entity_type_id") == Convert.ToInt32(temp.workflowData.entityTypeID)))
                        {
                            WorkFlowAttributeInfo tempAttr = new WorkFlowAttributeInfo();
                            tempAttr.attrID = attribute.Field<int>("attribute_id");
                            tempAttr.attrName = attribute.Field<string>("leg_name");
                            temp.workflowData.attributeInfo.Add(tempAttr);
                        }
                    }

                    temp.workflowData.usersInfo = new List<WorkflowUsersGroups>();
                    temp.workflowData.groupInfo = new List<WorkflowUsersGroups>();
                    foreach (DataRow userOrGroup in dsRM.Tables[2].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id")))
                    {
                        if (userOrGroup.Field<string>("workflow_group_user_mapping_type").ToLower() == "user")
                        {
                            //User Info
                            WorkflowUsersGroups uTemp = new WorkflowUsersGroups();
                            uTemp.name = userOrGroup.Field<string>("workflow_group_user_name");
                            uTemp.level = userOrGroup.Field<int>("workflow_sequence");
                            temp.workflowData.usersInfo.Add(uTemp);
                        }
                        else
                        {
                            //Group Info
                            WorkflowUsersGroups gTemp = new WorkflowUsersGroups();
                            gTemp.name = userOrGroup.Field<string>("workflow_group_user_name");
                            gTemp.level = userOrGroup.Field<int>("workflow_sequence");
                            temp.workflowData.groupInfo.Add(gTemp);
                        }
                    }
                    finalWorkflowObj.Add(temp);
                }
            }
            catch (Exception e)
            {

            }
            return finalWorkflowObj;
        }

        public List<WorkflowInfo> SMGetAllWorkflows(bool setRefWorkflow)
        {
            List<WorkflowInfo> finalWorkflowObj = new List<WorkflowInfo>();
            try
            {
                DataSet ds = WorkflowController.SMGetAllWorkflows();
                foreach (DataRow instance in ds.Tables[0].Rows)
                {
                    WorkflowInfo temp = new WorkflowInfo();
                    temp.moduleType = "SECMASTER";
                    temp.workflowType = 1;
                    temp.workflowName = instance.Field<string>("workflow_instance_name");
                    temp.workflowID = instance.Field<int>("workflow_instance_id");
                    temp.workflowData = new WorkflowData();
                    temp.workflowData.applyOnCreate = instance.Field<bool>("workflow_is_create");
                    temp.workflowData.applyTimeSeries = instance.Field<bool>("workflow_is_time_series");
                    temp.workflowData.applyOnBlankToNonblank = instance.Field<bool>("raise_for_non_empty_value");
                    temp.workflowData.sectypeIDs = String.Join(",", ds.Tables[1].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id")).Select(x => Convert.ToString(x.Field<int>("sectype_id"))).Distinct().ToArray<string>());

                    //Attributes
                    temp.workflowData.attributeInfo = new List<WorkFlowAttributeInfo>();
                    foreach (DataRow attribute in ds.Tables[1].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id") && x.Field<int>("sectype_id") == Convert.ToInt32(temp.workflowData.sectypeIDs.Split(',')[0])))
                    {
                        WorkFlowAttributeInfo tempAttr = new WorkFlowAttributeInfo();
                        tempAttr.attrID = attribute.Field<int>("attribute_id");
                        tempAttr.attrName = attribute.Field<string>("display_name");
                        temp.workflowData.attributeInfo.Add(tempAttr);
                    }

                    temp.workflowData.usersInfo = new List<WorkflowUsersGroups>();
                    temp.workflowData.groupInfo = new List<WorkflowUsersGroups>();
                    foreach (DataRow userOrGroup in ds.Tables[2].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id")))
                    {
                        if (userOrGroup.Field<string>("workflow_group_user_mapping_type").ToLower() == "user")
                        {
                            //User Info
                            WorkflowUsersGroups uTemp = new WorkflowUsersGroups();
                            uTemp.name = userOrGroup.Field<string>("workflow_group_user_name");
                            uTemp.level = userOrGroup.Field<int>("workflow_sequence");
                            temp.workflowData.usersInfo.Add(uTemp);
                        }
                        else
                        {
                            //Group Info
                            WorkflowUsersGroups gTemp = new WorkflowUsersGroups();
                            gTemp.name = userOrGroup.Field<string>("workflow_group_user_name");
                            gTemp.level = userOrGroup.Field<int>("workflow_sequence");
                            temp.workflowData.groupInfo.Add(gTemp);
                        }
                    }
                    finalWorkflowObj.Add(temp);
                }
                if (setRefWorkflow)
                {
                    DataSet dsRM = WorkflowController.RMGetAllWorkflows();
                    foreach (DataRow instance in dsRM.Tables[0].Rows)
                    {
                        WorkflowInfo temp = new WorkflowInfo();
                        temp.moduleType = "REFMASTER";
                        temp.workflowType = Convert.ToInt32(instance["workflow_type_id"]);
                        temp.workflowName = instance.Field<string>("workflow_instance_name");
                        temp.workflowID = instance.Field<int>("workflow_instance_id");
                        temp.workflowData = new WorkflowData();
                        temp.workflowData.applyOnCreate = instance.Field<bool>("workflow_is_create");
                        temp.workflowData.applyTimeSeries = instance.Field<bool>("workflow_is_time_series");
                        temp.workflowData.applyOnBlankToNonblank = instance.Field<bool>("raise_for_non_empty_value");
                        temp.workflowData.entityTypeID = String.Join(",", dsRM.Tables[1].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id")).Select(x => Convert.ToString(x.Field<int>("entity_type_id"))).Distinct().ToArray<string>());


                        if (temp.workflowType == 1)
                        {
                            //Attributes
                            temp.workflowData.attributeInfo = new List<WorkFlowAttributeInfo>();
                            foreach (DataRow attribute in dsRM.Tables[1].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id") && x.Field<int>("entity_type_id") == Convert.ToInt32(temp.workflowData.entityTypeID)))
                            {
                                WorkFlowAttributeInfo tempAttr = new WorkFlowAttributeInfo();
                                tempAttr.attrID = attribute.Field<int>("attribute_id");
                                tempAttr.attrName = attribute.Field<string>("entity_display_name");
                                temp.workflowData.attributeInfo.Add(tempAttr);
                            }
                        }

                        if (temp.workflowType == 3)
                        {
                            //Legs
                            temp.workflowData.attributeInfo = new List<WorkFlowAttributeInfo>();
                            foreach (DataRow attribute in dsRM.Tables[1].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id") && x.Field<int>("entity_type_id") == Convert.ToInt32(temp.workflowData.entityTypeID)))
                            {
                                WorkFlowAttributeInfo tempAttr = new WorkFlowAttributeInfo();
                                tempAttr.attrID = attribute.Field<int>("attribute_id");
                                tempAttr.attrName = attribute.Field<string>("leg_name");
                                temp.workflowData.attributeInfo.Add(tempAttr);
                            }
                        }

                        temp.workflowData.usersInfo = new List<WorkflowUsersGroups>();
                        temp.workflowData.groupInfo = new List<WorkflowUsersGroups>();
                        foreach (DataRow userOrGroup in dsRM.Tables[2].AsEnumerable().Where(x => x.Field<int>("workflow_instance_id") == instance.Field<int>("workflow_instance_id")))
                        {
                            if (userOrGroup.Field<string>("workflow_group_user_mapping_type").ToLower() == "user")
                            {
                                //User Info
                                WorkflowUsersGroups uTemp = new WorkflowUsersGroups();
                                uTemp.name = userOrGroup.Field<string>("workflow_group_user_name");
                                uTemp.level = userOrGroup.Field<int>("workflow_sequence");
                                temp.workflowData.usersInfo.Add(uTemp);
                            }
                            else
                            {
                                //Group Info
                                WorkflowUsersGroups gTemp = new WorkflowUsersGroups();
                                gTemp.name = userOrGroup.Field<string>("workflow_group_user_name");
                                gTemp.level = userOrGroup.Field<int>("workflow_sequence");
                                temp.workflowData.groupInfo.Add(gTemp);
                            }
                        }
                        finalWorkflowObj.Add(temp);
                    }
                }
            }
            catch (Exception e)
            {

            }
            return finalWorkflowObj;
        }

        public string SMUpdateWorkflow(WorkflowInfo info, int instanceId, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank)
        {
            List<string> sectypes = info.workflowData.sectypeIDs.Split(',').ToList();
            Dictionary<int, List<KeyValuePair<string, bool>>> tempUserGroupInfo = new Dictionary<int, List<KeyValuePair<string, bool>>>(); //true for user and false for group

            //For Users
            foreach (WorkflowUsersGroups item in info.workflowData.usersInfo)
            {
                var level = item.level;
                var name = item.name;

                if (level != 0)
                {
                    if (tempUserGroupInfo.ContainsKey(level))
                    {
                        tempUserGroupInfo[level].Add(new KeyValuePair<string, bool>(name, true));
                    }
                    else
                    {
                        List<KeyValuePair<string, bool>> tempList = new List<KeyValuePair<string, bool>>();
                        KeyValuePair<string, bool> tempObj = new KeyValuePair<string, bool>(name, true);
                        tempList.Add(tempObj);
                        tempUserGroupInfo.Add(level, tempList);
                    }
                }
            }
            //For Groups
            foreach (WorkflowUsersGroups item in info.workflowData.groupInfo)
            {
                var level = item.level;
                var name = item.name;

                if (level != 0)
                {
                    if (tempUserGroupInfo.ContainsKey(level))
                    {
                        tempUserGroupInfo[level].Add(new KeyValuePair<string, bool>(name, false));
                    }
                    else
                    {
                        List<KeyValuePair<string, bool>> tempList = new List<KeyValuePair<string, bool>>();
                        KeyValuePair<string, bool> tempObj = new KeyValuePair<string, bool>(name, false);
                        tempList.Add(tempObj);
                        tempUserGroupInfo.Add(level, tempList);
                    }
                }
            }

            XDocument doc = new XDocument(
                                new XElement("workflowInfo", new XAttribute("sectype_ids", String.Join(",", sectypes)), new XAttribute("attribute_ids", String.Join(",", info.workflowData.attributeInfo.Select(x => x.attrID))),
                                        tempUserGroupInfo.Select(x => new XElement("level", new XAttribute("sequence", Convert.ToString(x.Key)), new XAttribute("users", String.Join(",", x.Value.Where(y => y.Value).Select(y => y.Key).ToArray<string>())), new XAttribute("groups", String.Join(",", String.Join(",", x.Value.Where(y => !y.Value).Select(y => y.Key).ToArray<string>()))))
                                                )
                                            )
                                        );

            return WorkflowController.SMUpdateWorkflow(doc, instanceId, workflowName, isCreate, applyTimeSeries, username, applyBlankToNonBlank);
        }

        public string RMUpdateWorkflow(WorkflowInfo info, int instanceId, string workflowName, bool isCreate, bool applyTimeSeries, string username, bool applyBlankToNonBlank)
        {
            List<string> entityTypes = info.workflowData.entityTypeID.Split(',').ToList();
            Dictionary<int, List<KeyValuePair<string, bool>>> tempUserGroupInfo = new Dictionary<int, List<KeyValuePair<string, bool>>>(); //true for user and false for group

            //For Users
            foreach (WorkflowUsersGroups item in info.workflowData.usersInfo)
            {
                var level = item.level;
                var name = item.name;

                if (level != 0)
                {
                    if (tempUserGroupInfo.ContainsKey(level))
                    {
                        tempUserGroupInfo[level].Add(new KeyValuePair<string, bool>(name, true));
                    }
                    else
                    {
                        List<KeyValuePair<string, bool>> tempList = new List<KeyValuePair<string, bool>>();
                        KeyValuePair<string, bool> tempObj = new KeyValuePair<string, bool>(name, true);
                        tempList.Add(tempObj);
                        tempUserGroupInfo.Add(level, tempList);
                    }
                }
            }
            //For Groups
            foreach (WorkflowUsersGroups item in info.workflowData.groupInfo)
            {
                var level = item.level;
                var name = item.name;

                if (level != 0)
                {
                    if (tempUserGroupInfo.ContainsKey(level))
                    {
                        tempUserGroupInfo[level].Add(new KeyValuePair<string, bool>(name, false));
                    }
                    else
                    {
                        List<KeyValuePair<string, bool>> tempList = new List<KeyValuePair<string, bool>>();
                        KeyValuePair<string, bool> tempObj = new KeyValuePair<string, bool>(name, false);
                        tempList.Add(tempObj);
                        tempUserGroupInfo.Add(level, tempList);
                    }
                }
            }

            XDocument doc = null;
            if (info.workflowType == 1 || info.workflowType == 3)
            {
                doc = new XDocument(
                                    new XElement("workflowInfo", new XAttribute("sectype_ids", String.Join(",", entityTypes)), new XAttribute("attribute_ids", String.Join(",", info.workflowData.attributeInfo.Select(x => x.attrID))),
                                            tempUserGroupInfo.Select(x => new XElement("level", new XAttribute("sequence", Convert.ToString(x.Key)), new XAttribute("users", String.Join(",", x.Value.Where(y => y.Value).Select(y => y.Key).ToArray<string>())), new XAttribute("groups", String.Join(",", String.Join(",", x.Value.Where(y => !y.Value).Select(y => y.Key).ToArray<string>()))))
                                                    )
                                                )
                                            );
            }
            else
            {
                doc = new XDocument(
                                    new XElement("workflowInfo", new XAttribute("sectype_ids", String.Join(",", entityTypes)),
                                            tempUserGroupInfo.Select(x => new XElement("level", new XAttribute("sequence", Convert.ToString(x.Key)), new XAttribute("users", String.Join(",", x.Value.Where(y => y.Value).Select(y => y.Key).ToArray<string>())), new XAttribute("groups", String.Join(",", String.Join(",", x.Value.Where(y => !y.Value).Select(y => y.Key).ToArray<string>()))))
                                                    )
                                                )
                                            );
            }
            return WorkflowController.RMUpdateWorkflow(doc, instanceId, workflowName, isCreate, applyTimeSeries, username, applyBlankToNonBlank, info.workflowType);
        }

        public bool RemoveKey(string guid)
        {
            return WorkflowController.RemoveKey(guid);
        }

        //Overrides Methods -> Start
        public string GetOverridesDataSM(string username, string divID, string currPageID, string viewKey, string sessionID, string dateFormat)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            com.ivp.rad.controls.neogrid.service.RADNeoGridService service = new com.ivp.rad.controls.neogrid.service.RADNeoGridService();
            DataSet ds = OverridesController.GetOverridesDataSM(username);
            string customRowDataInfo = string.Empty;
            int rowCount = 0;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(ds.Tables[0].Rows[0]["status"]) == 1 && ds.Tables.Count > 1)
            {
                rowCount = ds.Tables[0].Rows.Count;
                ds.Tables[1].TableName = "overrideStatusTable";
                serializer.MaxJsonLength = Int32.MaxValue;

                com.ivp.rad.controls.neogrid.client.info.GridInfo gridInfo = new com.ivp.rad.controls.neogrid.client.info.GridInfo();
                gridInfo.CheckBoxInfo = new com.ivp.rad.controls.neogrid.client.info.CheckBoxInfo();
                gridInfo.SelectRecordsAcrossAllPages = true;
                gridInfo.ViewKey = viewKey;
                gridInfo.CacheGriddata = true;
                gridInfo.GridId = divID;
                gridInfo.CurrentPageId = currPageID;
                gridInfo.SessionIdentifier = sessionID;
                gridInfo.UserId = username;
                List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo> columnsToHide = new List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo>();
                //columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "is_updated_today", isDefault = true });
                //columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "is_verified_corpaction", isDefault = true });
                //columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "corpaction_type_id", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "attributeID", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "message", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "status", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "row_keys", isDefault = true });
                gridInfo.ColumnsToHide = columnsToHide;
                gridInfo.Height = "400px";
                gridInfo.ColumnsNotToSum = new List<string>();
                gridInfo.RequireEditGrid = false;
                gridInfo.IdColumnName = "row_id";
                gridInfo.TableName = "overrideStatusTable";
                gridInfo.PageSize = 200;
                gridInfo.RequirePaging = false;
                gridInfo.RequireInfiniteScroll = true;
                gridInfo.DoNotExpand = false;
                gridInfo.ItemText = "Number of Records";
                gridInfo.DoNotRearrangeColumn = true;
                gridInfo.RequireGrouping = true;
                gridInfo.RequireGroupExpandCollapse = true;
                gridInfo.RequireSelectedRows = true;
                gridInfo.RequireExportToExcel = true;
                gridInfo.RequireSearch = true;
                gridInfo.RequireResizing = true;
                gridInfo.RequireLayouts = false;
                gridInfo.DateFormat = dateFormat;
                gridInfo.DataSetDateTimeFormat = new Dictionary<string, string>() { { "Overridden On", dateFormat }, { "Expiring On", new RCommon().SessionInfo.CultureInfo.ShortDateFormat } };
                gridInfo.RequireFilter = true;
                gridInfo.CssExportRows = "xlneoexportToExcel";
                gridInfo.CustomRowsDataInfo = GetCustomRowDataInfoOverrideStatusSM(ds, username);
                gridInfo.ColumnDateFormatMapping = new Dictionary<string, string>() { { "Overridden On", dateFormat }, { "Expiring On", new RCommon().SessionInfo.CultureInfo.ShortDateFormat } };
                gridInfo.ColumnNotToExport = new List<string> { "attributeID", "row_keys" };

                //Set data in grid cache
                service.SaveGridDataInCache(ds.Tables[1], username, divID, currPageID, viewKey, sessionID, false, gridInfo);
            }
            return rowCount.ToString();
        }

        public SMSearchOverrideInfo GetOverridesSecutiyDataSM(string username, string divID, string currPageID, string viewKey, string sessionID, List<string> selectedSecIds, string dateFormat)
        {
            SMSearchOverrideInfo returnObj = new SMSearchOverrideInfo();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            com.ivp.rad.controls.neogrid.service.RADNeoGridService service = new com.ivp.rad.controls.neogrid.service.RADNeoGridService();
            DataSet ds = OverridesController.GetOverridesSecutiyDataSM(username, selectedSecIds, true);
            List<string> gridRowIds = new List<string>();
            //returnObj.customRowInfo = serializer.Serialize(GetCustomRowDataInfoOverrideSecutiyDataSM(ref ds, ref gridRowIds));
            returnObj.gridRowIds = gridRowIds;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(ds.Tables[0].Rows[0]["status"]) == 1 && ds.Tables.Count > 1)
            {
                ds.Tables[1].TableName = "overrideStatusTable";
                serializer.MaxJsonLength = Int32.MaxValue;

                com.ivp.rad.controls.neogrid.client.info.GridInfo gridInfo = new com.ivp.rad.controls.neogrid.client.info.GridInfo();
                gridInfo.CheckBoxInfo = new com.ivp.rad.controls.neogrid.client.info.CheckBoxInfo();
                Dictionary<string, Dictionary<string, string>> chbInfo = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> chbRowInfo = new Dictionary<string, string>();
                chbRowInfo.Add("checked", "checked");

                gridInfo.SelectRecordsAcrossAllPages = true;
                gridInfo.ViewKey = viewKey;
                gridInfo.CacheGriddata = true;
                gridInfo.GridId = divID;
                gridInfo.CurrentPageId = currPageID;
                gridInfo.SessionIdentifier = sessionID;
                gridInfo.UserId = username;
                List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo> columnsToHide = new List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo>();
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "row_keys", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "attribute_id", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "sectype_id", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "attribute_name", isDefault = true });
                gridInfo.ColumnsToHide = columnsToHide;
                gridInfo.Height = "400px";
                gridInfo.ColumnsNotToSum = new List<string>();
                gridInfo.RequireEditGrid = false;
                gridInfo.IdColumnName = "row_id";
                gridInfo.TableName = "overrideStatusTable";
                gridInfo.PageSize = 200;
                gridInfo.RequirePaging = false;
                gridInfo.RequireInfiniteScroll = true;
                gridInfo.DoNotExpand = false;
                gridInfo.ItemText = "Number of Records";
                gridInfo.DoNotRearrangeColumn = true;
                gridInfo.RequireGrouping = true;
                gridInfo.RequireGroupExpandCollapse = true;
                gridInfo.RequireSelectedRows = true;
                gridInfo.RequireExportToExcel = true;
                gridInfo.RequireSearch = true;
                gridInfo.RequireResizing = true;
                gridInfo.RequireLayouts = false;
                gridInfo.DateFormat = dateFormat;
                gridInfo.RequireFilter = true;
                gridInfo.CssExportRows = "xlneoexportToExcel";
                gridInfo.RaiseGridCallBackBeforeExecute = "";
                gridInfo.CustomRowsDataInfo = GetCustomRowDataInfoOverrideSecutiyDataSM(ref ds, ref gridRowIds, username);
                foreach (string i in gridRowIds)
                {
                    chbInfo.Add(i, chbRowInfo);
                }

                gridInfo.CheckBoxInfo.ItemAttribute = chbInfo;
                //Set data in grid cache
                service.SaveGridDataInCache(ds.Tables[1], username, divID, currPageID, viewKey, sessionID, false, gridInfo);
                //service.SaveGridDataInCache(ds, username, divID, currPageID, viewKey, sessionID, false);

                returnObj.attributeList = new List<AttributeInfo>();
                foreach (DataRow row in ds.Tables[2].Rows)
                {
                    AttributeInfo tempInfo = new AttributeInfo();
                    tempInfo.attributeID = Convert.ToString(row["attribute_id"]);
                    tempInfo.attributeName = Convert.ToString(row["Attribute Name"]);
                    //if (!String.IsNullOrEmpty(Convert.ToString(row["rule_override_timeperiod"])))
                    //    tempInfo.attributeExpiry = Convert.ToString(row["rule_override_timeperiod"]);
                    //else if (!String.IsNullOrEmpty(Convert.ToString(row["vendor_override_timeperiod"])))
                    //    tempInfo.attributeExpiry = Convert.ToString(row["vendor_override_timeperiod"]);
                    //else
                    tempInfo.attributeExpiry = "Never";

                    returnObj.attributeList.Add(tempInfo);
                }
            }
            return returnObj;
        }

        public string GetOverridesDataRM(string username, string divID, string currPageID, string viewKey, string sessionID, string dateFormat, int ModuleId, string dateFormatLong)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            com.ivp.rad.controls.neogrid.service.RADNeoGridService service = new com.ivp.rad.controls.neogrid.service.RADNeoGridService();
            DataSet ds = OverridesController.GetOverridesDataRM(username, dateFormat, dateFormatLong, ModuleId);
            string customRowDataInfo = string.Empty;//serializer.Serialize(GetCustomRowDataInfoOverrideStatusRM(ref ds));
            int rowCount = 0;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                rowCount = ds.Tables[0].Rows.Count;
                ds.Tables[0].TableName = "overrideStatusTable";
                serializer.MaxJsonLength = Int32.MaxValue;

                com.ivp.rad.controls.neogrid.client.info.GridInfo gridInfo = new com.ivp.rad.controls.neogrid.client.info.GridInfo();
                gridInfo.CheckBoxInfo = new com.ivp.rad.controls.neogrid.client.info.CheckBoxInfo();
                gridInfo.SelectRecordsAcrossAllPages = true;
                gridInfo.ViewKey = viewKey;
                gridInfo.CacheGriddata = true;
                gridInfo.GridId = divID;
                gridInfo.CurrentPageId = currPageID;
                gridInfo.SessionIdentifier = sessionID;
                gridInfo.UserId = username;
                List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo> columnsToHide = new List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo>();
                //columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "is_updated_today", isDefault = true });
                //columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "is_verified_corpaction", isDefault = true });
                //columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "corpaction_type_id", isDefault = true });
                //columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "has_exceptions", isDefault = true });
                //columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "template_id", isDefault = true });
                //columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "created_on", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "row_keys", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "locked_until", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "last_modified_on", isDefault = true });
                gridInfo.ColumnsToHide = columnsToHide;
                gridInfo.Height = "400px";
                gridInfo.ColumnsNotToSum = new List<string>();
                gridInfo.RequireEditGrid = false;
                gridInfo.IdColumnName = "row_id";
                gridInfo.TableName = "overrideStatusTable";
                gridInfo.PageSize = 200;
                gridInfo.RequirePaging = false;
                gridInfo.RequireInfiniteScroll = true;
                gridInfo.DoNotExpand = false;
                gridInfo.ItemText = "Number of Records";
                gridInfo.DoNotRearrangeColumn = true;
                gridInfo.RequireGrouping = true;
                gridInfo.RequireGroupExpandCollapse = true;
                gridInfo.RequireSelectedRows = true;
                gridInfo.RequireExportToExcel = true;
                gridInfo.RequireSearch = true;
                gridInfo.RequireResizing = true;
                gridInfo.RequireLayouts = false;
                gridInfo.DateFormat = dateFormat;
                gridInfo.RequireFilter = true;
                gridInfo.CssExportRows = "xlneoexportToExcel";
                gridInfo.CustomRowsDataInfo = GetCustomRowDataInfoOverrideStatusRM(ref ds, username, ModuleId);

                //Set data in grid cache
                service.SaveGridDataInCache(ds.Tables[0], username, divID, currPageID, viewKey, sessionID, false, gridInfo);
                //service.SaveGridDataInCache(ds, username, divID, currPageID, viewKey, sessionID, false);
            }
            return rowCount.ToString();
        }

        public SMSearchOverrideInfo GetOverridesEntityDataRM(string username, string divID, string currPageID, string viewKey, string sessionID, List<string> selectedEntityIds, int entityTypeID, string dateFormat)
        {
            SMSearchOverrideInfo returnObj = new SMSearchOverrideInfo();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            com.ivp.rad.controls.neogrid.service.RADNeoGridService service = new com.ivp.rad.controls.neogrid.service.RADNeoGridService();
            DataSet ds = OverridesController.GetOverridesEntityDataRM(selectedEntityIds, entityTypeID);
            List<string> gridRowIds = new List<string>();
            //returnObj.customRowInfo = serializer.Serialize(GetCustomRowDataInfoOverrideEntityDataRM(ref ds, ref gridRowIds));
            returnObj.gridRowIds = gridRowIds;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].TableName = "overrideStatusTable";
                serializer.MaxJsonLength = Int32.MaxValue;

                com.ivp.rad.controls.neogrid.client.info.GridInfo gridInfo = new com.ivp.rad.controls.neogrid.client.info.GridInfo();
                gridInfo.CheckBoxInfo = new com.ivp.rad.controls.neogrid.client.info.CheckBoxInfo();
                Dictionary<string, Dictionary<string, string>> chbInfo = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> chbRowInfo = new Dictionary<string, string>();
                chbRowInfo.Add("checked", "checked");
                gridInfo.SelectRecordsAcrossAllPages = true;
                gridInfo.ViewKey = viewKey;
                gridInfo.CacheGriddata = true;
                gridInfo.GridId = divID;
                gridInfo.CurrentPageId = currPageID;
                gridInfo.SessionIdentifier = sessionID;
                gridInfo.UserId = username;
                List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo> columnsToHide = new List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo>();
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "TableIndex", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "entity_type_id", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "entity_display_name", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "Is Active", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "id", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "loading_time", isDefault = true });
                columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "row_keys", isDefault = true });
                gridInfo.ColumnsToHide = columnsToHide;
                gridInfo.Height = "400px";
                gridInfo.ColumnsNotToSum = new List<string>();
                gridInfo.RequireEditGrid = false;
                gridInfo.IdColumnName = "row_id";
                gridInfo.TableName = "overrideStatusTable";
                gridInfo.PageSize = 200;
                gridInfo.RequirePaging = false;
                gridInfo.RequireInfiniteScroll = true;
                gridInfo.DoNotExpand = false;
                gridInfo.ItemText = "Number of Records";
                gridInfo.DoNotRearrangeColumn = true;
                gridInfo.RequireGrouping = true;
                gridInfo.RequireGroupExpandCollapse = true;
                gridInfo.RequireSelectedRows = true;
                gridInfo.RequireExportToExcel = true;
                gridInfo.RequireSearch = true;
                gridInfo.RequireResizing = true;
                gridInfo.RequireLayouts = false;
                gridInfo.DateFormat = dateFormat;
                gridInfo.RequireFilter = true;
                gridInfo.CssExportRows = "xlneoexportToExcel";
                gridInfo.CustomRowsDataInfo = GetCustomRowDataInfoOverrideEntityDataRM(ds, ref gridRowIds, username);

                //Set data in grid cache
                service.SaveGridDataInCache(ds.Tables[0], username, divID, currPageID, viewKey, sessionID, false, gridInfo);
                //service.SaveGridDataInCache(ds, username, divID, currPageID, viewKey, sessionID, false);
                foreach (string i in gridRowIds)
                {
                    chbInfo.Add(i, chbRowInfo);
                }

                gridInfo.CheckBoxInfo.ItemAttribute = chbInfo;
                returnObj.attributeList = new List<AttributeInfo>();
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    AttributeInfo tempInfo = new AttributeInfo();
                    tempInfo.attributeID = Convert.ToString(row["entity_attribute_id"]);
                    tempInfo.attributeName = Convert.ToString(row["display_name"]);
                    //if (!String.IsNullOrEmpty(Convert.ToString(row["rule_override_timeperiod"])))
                    //    tempInfo.attributeExpiry = Convert.ToString(row["rule_override_timeperiod"]);
                    //else if (!String.IsNullOrEmpty(Convert.ToString(row["vendor_override_timeperiod"])))
                    //    tempInfo.attributeExpiry = Convert.ToString(row["vendor_override_timeperiod"]);
                    //else
                    //    tempInfo.attributeExpiry = "Never";

                    returnObj.attributeList.Add(tempInfo);
                }
            }
            return returnObj;
        }

        private static List<CustomRowDataInfo> GetCustomRowDataInfoOverrideStatusSM(DataSet ds, string username)
        {
            List<string> lstPriv = new List<string>();
            lstPriv.Add("Monitor Overrides - View Security");
            lstPriv.Add("Monitor Overrides - Edit Security");

            var lstPrivInfo = new CommonService().CheckControlPrivilegeForUserMultiple(lstPriv, username);
            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            List<CustomCellDatainfo> lstCustomCellDataInfo = new List<CustomCellDatainfo>();
            CustomRowDataInfo objCustomRowDataInfo = null;
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[1];
                if (!ds.Tables[1].Columns.Contains("row_keys"))
                    ds.Tables[1].Columns.Add("row_keys");

                RUserManagementService objUserController = new RUserManagementService();
                List<RUserInfo> radUsers = objUserController.GetAllUsersGDPR();

                Dictionary<string, string> dictUserLoginNamevsFullName = radUsers.ToDictionary(x => x.UserLoginName, y => y.FullName + " (" + y.UserName + ")");



                foreach (DataRow dr1 in dt.Rows)
                {
                    if (dt.Columns.Contains("Overridden By"))
                    {
                        string user = Convert.ToString(dr1["Overridden By"]);
                        if (!string.IsNullOrEmpty(user) && dictUserLoginNamevsFullName.ContainsKey(user))
                            dr1["Overridden By"] = dictUserLoginNamevsFullName[user];

                    }

                    objCustomRowDataInfo = new CustomRowDataInfo();
                    dr1["row_keys"] = Convert.ToString(dr1["row_id"]) + "||" + Convert.ToString(dr1["Security ID"]) + "||" + Convert.ToString(dr1["attributeID"]);
                    lstCustomCellDataInfo = new List<CustomCellDatainfo>();
                    objCustomRowDataInfo.RowID = Convert.ToString(dr1["row_id"]);

                    if (lstPrivInfo != null && (lstPrivInfo[0].result || lstPrivInfo[1].result))
                        lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Security ID", NewChild = "<div id='" + Convert.ToString(dr1["row_id"]) + "_divSecurityId" + Convert.ToString(dr1["Security ID"]) + "' style='text-decoration:underline;cursor:pointer;color:#00BFF0;'>" + Convert.ToString(dr1["Security ID"]) + "</div>" });
                    else
                        lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Security ID", NewChild = "<div id='" + Convert.ToString(dr1["row_id"]) + "_divSecurityId" + Convert.ToString(dr1["Security ID"]) + "' >" + Convert.ToString(dr1["Security ID"]) + "</div>" });

                    //if (String.IsNullOrEmpty(dr1["Expiring On"].ToString()))
                    //{
                    //    dr1["Expiring On"] = "Never";
                    //}

                    objCustomRowDataInfo.Attribute.Add("securityid", Convert.ToString(dr1["Security ID"]));
                    objCustomRowDataInfo.Cells = lstCustomCellDataInfo;
                    lstCustomRowDataInfo.Add(objCustomRowDataInfo);
                }
            }
            return lstCustomRowDataInfo;
        }

        private static List<CustomRowDataInfo> GetCustomRowDataInfoOverrideSecutiyDataSM(ref DataSet ds, ref List<string> gridRowIds, string username)
        {
            List<string> lstPriv = new List<string>();
            lstPriv.Add("Monitor Overrides - View Security");
            lstPriv.Add("Monitor Overrides - Edit Security");

            var lstPrivInfo = new CommonService().CheckControlPrivilegeForUserMultiple(lstPriv, username);

            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            List<CustomCellDatainfo> lstCustomCellDataInfo = new List<CustomCellDatainfo>();
            CustomRowDataInfo objCustomRowDataInfo = null;
            if (ds.Tables.Count > 1)
            {
                DataTable dt = ds.Tables[1];
                if (!ds.Tables[1].Columns.Contains("row_keys"))
                    ds.Tables[1].Columns.Add("row_keys");

                foreach (DataRow dr1 in dt.Rows)
                {
                    objCustomRowDataInfo = new CustomRowDataInfo();
                    dr1["row_keys"] = Convert.ToString(dr1["Security ID"]) + "|" + Convert.ToString(dr1["sectype_id"]);
                    lstCustomCellDataInfo = new List<CustomCellDatainfo>();
                    objCustomRowDataInfo.RowID = Convert.ToString(dr1["row_id"]);


                    if (lstPrivInfo != null && (lstPrivInfo[0].result || lstPrivInfo[1].result))
                        lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Security ID", NewChild = "<div id='divSecurityId" + Convert.ToString(dr1["Security ID"]) + "' style='text-decoration:underline;cursor:pointer;color:#00BFF0;'>" + Convert.ToString(dr1["Security ID"]) + "</div>" });
                    else
                        lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Security ID", NewChild = "<div id='divSecurityId" + Convert.ToString(dr1["Security ID"]) + "'>" + Convert.ToString(dr1["Security ID"]) + "</div>" });
                    objCustomRowDataInfo.Attribute.Add("securityid", Convert.ToString(dr1["Security ID"]));
                    objCustomRowDataInfo.Cells = lstCustomCellDataInfo;
                    lstCustomRowDataInfo.Add(objCustomRowDataInfo);

                    gridRowIds.Add(Convert.ToString(dr1["row_id"]));
                }
            }
            return lstCustomRowDataInfo;
        }

        private static List<CustomRowDataInfo> GetCustomRowDataInfoOverrideEntityDataRM(DataSet ds, ref List<string> gridRowIds, string username)
        {
            List<string> lstPriv = new List<string>();
            lstPriv.Add("RM - Monitor Overrides - View Entity");
            lstPriv.Add("RM - Monitor Overrides - Edit Entity");

            var lstPrivInfo = new CommonService().CheckControlPrivilegeForUserMultiple(lstPriv, username);
            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            List<CustomCellDatainfo> lstCustomCellDataInfo = new List<CustomCellDatainfo>();
            CustomRowDataInfo objCustomRowDataInfo = null;
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (!ds.Tables[0].Columns.Contains("row_keys"))
                    ds.Tables[0].Columns.Add("row_keys");
                if (!ds.Tables[0].Columns.Contains("row_id"))
                    ds.Tables[0].Columns.Add("row_id");

                int i = 0;
                foreach (DataRow dr1 in dt.Rows)
                {
                    objCustomRowDataInfo = new CustomRowDataInfo();
                    dr1["row_id"] = (++i).ToString() + "_" + Convert.ToString(dr1["Entity Code"]);
                    dr1["row_keys"] = Convert.ToString(dr1["row_id"]) + "||" + Convert.ToString(dr1["Entity Code"]);
                    lstCustomCellDataInfo = new List<CustomCellDatainfo>();
                    objCustomRowDataInfo.RowID = Convert.ToString(dr1["row_id"]);

                    if (lstPrivInfo != null && (lstPrivInfo[0].result || lstPrivInfo[1].result))
                        lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Entity Code", NewChild = "<div id='divEntityCode" + Convert.ToString(dr1["Entity Code"]) + "' style='text-decoration:underline;cursor:pointer;color:#00BFF0;'>" + Convert.ToString(dr1["Entity Code"]) + "</div>" });
                    else
                        lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Entity Code", NewChild = "<div id='divEntityCode" + Convert.ToString(dr1["Entity Code"]) + "' >" + Convert.ToString(dr1["Entity Code"]) + "</div>" });
                    objCustomRowDataInfo.Attribute.Add("entityCode", Convert.ToString(dr1["Entity Code"]));
                    objCustomRowDataInfo.Cells = lstCustomCellDataInfo;
                    lstCustomRowDataInfo.Add(objCustomRowDataInfo);

                    gridRowIds.Add(Convert.ToString(dr1["row_id"]));
                }
            }
            return lstCustomRowDataInfo;
        }

        private static List<CustomRowDataInfo> GetCustomRowDataInfoOverrideStatusRM(ref DataSet ds, string username, int moduleId)
        {
            List<string> lstPriv = new List<string>();
            switch (moduleId)
            {
                case 6:
                    lstPriv.Add("RM - Monitor Overrides - View Entity");
                    lstPriv.Add("RM - Monitor Overrides - Edit Entity");
                    break;
                case 18:
                    lstPriv.Add("FM - Monitor Overrides - View Entity");
                    lstPriv.Add("FM - Monitor Overrides - Edit Entity");
                    break;
                case 20:
                    lstPriv.Add("PM - Monitor Overrides - View Entity");
                    lstPriv.Add("PM - Monitor Overrides - Edit Entity");
                    break;
                default:
                    lstPriv.Add("RM - Monitor Overrides - View Entity");
                    lstPriv.Add("RM - Monitor Overrides - Edit Entity");
                    break;
            }

            var lstPrivInfo = new CommonService().CheckControlPrivilegeForUserMultiple(lstPriv, username);

            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            List<CustomCellDatainfo> lstCustomCellDataInfo = new List<CustomCellDatainfo>();
            CustomRowDataInfo objCustomRowDataInfo = null;
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                if (!ds.Tables[0].Columns.Contains("row_keys"))
                    ds.Tables[0].Columns.Add("row_keys");

                foreach (DataRow dr1 in dt.Rows)
                {
                    objCustomRowDataInfo = new CustomRowDataInfo();
                    dr1["row_keys"] = Convert.ToString(dr1["row_id"]) + "||" + Convert.ToString(dr1["Entity Code"]) + "||" + Convert.ToString(dr1["Attribute Name"]);
                    lstCustomCellDataInfo = new List<CustomCellDatainfo>();
                    objCustomRowDataInfo.RowID = Convert.ToString(dr1["row_id"]);

                    if (lstPrivInfo != null && (lstPrivInfo[0].result || lstPrivInfo[1].result))
                        lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Entity Code", NewChild = "<div id='divEntityCode" + Convert.ToString(dr1["Entity Code"]) + "' style='text-decoration:underline;cursor:pointer;color:#00BFF0;'>" + Convert.ToString(dr1["Entity Code"]) + "</div>" });
                    else
                        lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Entity Code", NewChild = "<div id='divEntityCode" + Convert.ToString(dr1["Entity Code"]) + "' >" + Convert.ToString(dr1["Entity Code"]) + "</div>" });

                    objCustomRowDataInfo.Attribute.Add("entitycode", Convert.ToString(dr1["Entity Code"]));
                    objCustomRowDataInfo.Cells = lstCustomCellDataInfo;
                    lstCustomRowDataInfo.Add(objCustomRowDataInfo);
                }
            }
            return lstCustomRowDataInfo;
        }

        public OverrideStatusRowInfo OverrideStatusGetCheckedRowsData(string cacheKey)
        {
            com.ivp.rad.controls.neogrid.service.RADNeoGridService service = new com.ivp.rad.controls.neogrid.service.RADNeoGridService();
            List<System.Xml.Linq.XElement> xml = service.GetCheckedRows(cacheKey);
            OverrideStatusRowInfo rowInfo = new OverrideStatusRowInfo();

            if (xml.Count > 0)
            {
                for (int index = 0; index < xml.Count; index++)
                {
                    if (xml[index].Elements("row_id").Any())
                        rowInfo.ruleID = rowInfo.ruleID + "|" + xml[index].Descendants("row_id").FirstOrDefault().Value;
                    else
                        rowInfo.ruleID = "";
                    if (xml[index].Elements("Security_x0020_ID").Any())
                        rowInfo.typeID = rowInfo.typeID + "|" + xml[index].Descendants("Security_x0020_ID").FirstOrDefault().Value;
                    else
                        rowInfo.typeID = "";
                    if (xml[index].Elements("attributeID").Any())
                        rowInfo.attributeID = rowInfo.attributeID + "|" + xml[index].Descendants("attributeID").FirstOrDefault().Value;
                    else
                        rowInfo.attributeID = "";
                    if (xml[index].Elements("Attribute_x0020_Name").Any())
                        rowInfo.attributeDisplayName = rowInfo.attributeDisplayName + "|" + xml[index].Descendants("Attribute_x0020_Name").FirstOrDefault().Value;
                    else
                        rowInfo.attributeDisplayName = "";
                    if (xml[index].Elements("Entity_x0020_Code").Any())
                        rowInfo.entityCode = rowInfo.entityCode + "|" + xml[index].Descendants("Entity_x0020_Code").FirstOrDefault().Value;
                    else
                        rowInfo.entityCode = "";
                    if (xml[index].Elements("table_name").Any())
                        rowInfo.tableName = rowInfo.tableName + "|" + xml[index].Descendants("table_name").FirstOrDefault().Value;
                    else
                        rowInfo.tableName = "";
                }
            }
            return rowInfo;
        }

        public List<string> OverrideDataGetCheckedRowsData(string cacheKey, string productName)
        {
            com.ivp.rad.controls.neogrid.service.RADNeoGridService service = new com.ivp.rad.controls.neogrid.service.RADNeoGridService();
            List<System.Xml.Linq.XElement> xml = service.GetCheckedRows(cacheKey);
            List<string> secIDs = new List<string>();

            if (xml.Count > 0)
            {
                for (int index = 0; index < xml.Count; index++)
                {
                    if (productName.ToLower().Equals("secmaster"))
                    {
                        if (xml[index].Elements("Security_x0020_ID").Any())
                            secIDs.Add(xml[index].Descendants("row_keys").FirstOrDefault().Value);
                    }
                    else if (productName.ToLower().Equals("refmaster"))
                    {
                        if (xml[index].Elements("Entity_x0020_Code").Any())
                            secIDs.Add(xml[index].Descendants("row_keys").FirstOrDefault().Value);
                    }
                }
            }
            return secIDs;
        }

        public string SMDeleteOverride(Dictionary<int, string> deleteInfo)
        {
            return OverridesController.SMDeleteOverride(deleteInfo);
        }

        public string RMDeleteOverride(string username, List<RMEntityOverrideInfo> attrInfo)
        {
            try
            {
                RMAttributeOverride obj = new RMAttributeOverride();
                DataSet outputDS = obj.SaveAttributeOverride(attrInfo, username);
                string response = string.Empty;
                if (outputDS.Tables.Count > 0)
                {
                    if (outputDS.Tables[0].Rows.Count == 1)
                    {
                        response = outputDS.Tables[0].Rows[0]["status"] + "|" + outputDS.Tables[0].Rows[0]["message"];
                    }
                    else
                    {
                        response = "0|No Response from Procedure";
                    }
                }
                else
                {
                    response = "0|No Response from Procedure";
                }
                return response;
            }
            catch (Exception ex)
            {
                return "0|" + ex.Message.ToString();
            }
        }

        public string SaveBulkOverrideSM(string username, string uniqueId, Dictionary<string, List<SMOverrideAttributesInfo>> attrInfo)
        {
            return OverridesController.SMSaveOverride(username, uniqueId, attrInfo);
        }

        public string SaveBulkOverrideRM(string username, List<RMEntityOverrideInfo> attrInfo)
        {
            try
            {

                //No need

                //foreach (RMEntityOverrideInfo item in attrInfo)
                //{
                //    List<string> lockedUntillLst = item.Attributes.Select(x => x.LockedUntil).ToList();
                //    foreach (string s in lockedUntillLst)
                //    {
                //        if (!string.IsNullOrEmpty(s))
                //        {
                //            DateTime d = DateTime.ParseExact(s, "yyyy-M-d", CultureInfo.InvariantCulture);
                //            foreach (RMAttributeOverrideInfo attr in item.Attributes)
                //            {
                //                if (!string.IsNullOrEmpty(attr.LockedUntil) && (attr.LockedUntil.Equals(s)))
                //                    attr.LockedUntil = d.ToString("M/d/yyyy");
                //            }
                //        }
                //    }
                //}

                RMAttributeOverride obj = new RMAttributeOverride();
                DataSet outputDS = obj.SaveAttributeOverride(attrInfo, username);
                string response = string.Empty;
                if (outputDS.Tables.Count > 0)
                {
                    if (outputDS.Tables[0].Rows.Count == 1)
                    {
                        response = outputDS.Tables[0].Rows[0]["status"] + "|" + outputDS.Tables[0].Rows[0]["message"];
                    }
                    else
                    {
                        response = "0|No Response from Procedure";
                    }
                }
                else
                {
                    response = "0|No Response from Procedure";
                }
                return response;
            }
            catch (Exception ex)
            {
                return "0|" + ex.Message.ToString();
            }
        }
        //Overrides Methods -> End

        //trying chartData for DQM
        public DqmChartData GetChartData()
        {
            //add try catch/status bit 
            //dummy data


            try
            {
                DqmChartData dqmChartData = new DqmChartData();
                dqmChartData.seriesData = new DqmChartSeriesData();
                dqmChartData.info1 = "Info";
                dqmChartData.subInfo1 = "Subinfo";
                dqmChartData.seriesData.chartName = "Chart 1";
                dqmChartData.seriesData.chartName = "Blue";
                dqmChartData.seriesData.chartData = new Dictionary<DqmChartDate, float>();
                dqmChartData.seriesData.chartData.Add(new DqmChartDate(2017, 1, 3), 23);
                dqmChartData.seriesData.chartData.Add(new DqmChartDate(2017, 1, 4), 24);
                dqmChartData.seriesData.chartData.Add(new DqmChartDate(2017, 1, 5), 25);
                dqmChartData.seriesData.chartData.Add(new DqmChartDate(2017, 1, 6), 26);
                dqmChartData.seriesData.chartData.Add(new DqmChartDate(2017, 1, 7), 27);
                dqmChartData.seriesData.chartData.Add(new DqmChartDate(2017, 1, 8), 28);
                dqmChartData.seriesData.chartData.Add(new DqmChartDate(2017, 1, 9), 29);
                dqmChartData.seriesData.chartData.Add(new DqmChartDate(2017, 1, 10), 30);
                dqmChartData.seriesData.chartData.Add(new DqmChartDate(2017, 1, 11), 31);
                dqmChartData.statusBit = 1;
                return dqmChartData;
            }
            catch (Exception ex)
            {
                DqmChartData dqmChartData = new DqmChartData();
                dqmChartData.statusBit = 0;
                return dqmChartData;
            }

        }


        //Task details in dqm page
        public List<DqmChainData> GetChainData()
        {

            DqmChainData dqmChainData = new DqmChainData();
            dqmChainData.statusBit = 1;
            dqmChainData.ChainAvgDuration = "34 mins";
            dqmChainData.ChainDate = "6/1/2017";
            dqmChainData.ChainTime = "04.25 AM";
            dqmChainData.ChainDuration = "35 mins 33 secs";
            dqmChainData.State = "passed";
            dqmChainData.WarningCount = 0;
            dqmChainData.ChainName = "Name - Equities SOD Refresh";
            dqmChainData.TaskDetails = new List<DqmTaskDetails>();
            dqmChainData.TaskDetails.Add(new DqmTaskDetails("Loading Task for Geneva Data Load", "Loading", 36, 3));
            dqmChainData.TaskDetails.Add(new DqmTaskDetails("FLOW_Equity Common Stock", "Update Core", 36, 3));
            dqmChainData.TaskDetails.Add(new DqmTaskDetails("FLOW_Corporate Bond", "Update Core", 36, 3));

            DqmChainData dqmChainData2 = new DqmChainData();
            dqmChainData2.statusBit = 1;
            dqmChainData2.ChainAvgDuration = "20min";
            dqmChainData2.ChainDate = "6/1/2017";
            dqmChainData2.ChainTime = "03.55 AM";
            dqmChainData2.ChainDuration = "30 min";
            dqmChainData2.State = "passed";
            dqmChainData2.WarningCount = 0;
            dqmChainData2.ChainName = "Name - Securities in Position";
            dqmChainData2.TaskDetails = new List<DqmTaskDetails>();
            dqmChainData2.TaskDetails.Add(new DqmTaskDetails("Equity Common Stock Feed - Request", "Request", 36, 3));
            dqmChainData2.TaskDetails.Add(new DqmTaskDetails("Equity Common Stock Feed - Response", "Response", 36, 3));
            dqmChainData2.TaskDetails.Add(new DqmTaskDetails("ADR Feed - Request", "Request", 36, 3));

            List<DqmChainData> dqmChainDataList = new List<DqmChainData>();
            dqmChainDataList.Add(dqmChainData);
            dqmChainDataList.Add(dqmChainData2);
            return dqmChainDataList;

        }

        ////Vendor Management Data
        ////gives the list of preferences
        public VendorManagementDataSourceType GetVendorManagementData(int preferenceId, int vendorId)
        {
            return new VendorSystemSettingsController().GetVendorManagementData(preferenceId, vendorId);
        }


        public VendorManagementDataSourceType SaveVendorManagementData(VendorManagementInputInfo inputData)
        {
            return new VendorSystemSettingsController().SaveVendorManagementData(inputData);
        }

        public VendorManagementDataSourceType DeleteVendorManagementData(VendorManagementDeleteData inputData)
        {
            return new VendorSystemSettingsController().DeleteVendorManagementData(inputData);
        }



        #region QuantModule

        public void GetQuantData(string username, string encodedQuery, char tokenSeparator, char paramSeparator, char functionStart, char functionEnd, char valueIdentifier, string divId, string curPageId, string viewKey, string sessionId, char querySeparator)
        {
            RADNeoGridService service = new RADNeoGridService();
            DataTable dt = SRMQuantController.GetQueryResults(encodedQuery, tokenSeparator, paramSeparator, functionStart, functionEnd, valueIdentifier, querySeparator);
            dt.TableName = "TEST";
            dt.Columns["Security Name"].SetOrdinal(0);
            if (dt != null)
            {
                GridInfo gridInfo = new GridInfo();
                gridInfo.CheckBoxInfo = null;
                gridInfo.SelectRecordsAcrossAllPages = true;
                gridInfo.ViewKey = viewKey;
                gridInfo.CacheGriddata = true;
                gridInfo.GridId = divId;
                gridInfo.CurrentPageId = curPageId;
                gridInfo.SessionIdentifier = sessionId;
                gridInfo.UserId = username;
                gridInfo.Height = "400px";
                gridInfo.ColumnsNotToSum = new List<string>();
                gridInfo.RequireEditGrid = false;
                gridInfo.IdColumnName = "row_id";
                gridInfo.TableName = "TEST";
                gridInfo.PageSize = 200;
                gridInfo.RequirePaging = true;
                gridInfo.RightAlignHeaderForNumerics = false;
                gridInfo.DoNotExpand = false;
                gridInfo.ItemText = "Number of Records";
                gridInfo.DoNotRearrangeColumn = true;
                gridInfo.RequireGrouping = true;
                gridInfo.RequireGroupExpandCollapse = true;
                gridInfo.RequireSelectedRows = true;
                gridInfo.RequireExportToExcel = true;
                gridInfo.RequireSearch = true;
                gridInfo.RequireResizing = true;
                gridInfo.RequireLayouts = false;
                gridInfo.DateFormat = "yyyy/MM/dd";
                gridInfo.RequireFilter = true;
                gridInfo.CssExportRows = "xlneoexportToExcel";
                gridInfo.CustomRowsDataInfo = GetCustomRowDataInfoQuantDataSM(dt);
                gridInfo.ColumnWidths = new Dictionary<string, string>() { { "Security Name", "225px" } };

                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.DataType == typeof(decimal) && dc.ColumnName.ToLower() != "Moneyness".ToLower())
                    {
                        gridInfo.GridCustomFormatMapping.Add(dc.ColumnName, new com.ivp.rad.controls.neogrid.Formatters.CustomFormatter()
                        {
                            DataType = com.ivp.rad.controls.neogrid.Formatters.FormatterDataType.Number,
                            DecimalPlaces = 2,
                            NegativeValue = com.ivp.rad.controls.neogrid.Formatters.NegativeValue.DEFAULT
                        });
                    }
                }

                gridInfo.GridCustomFormatMapping.Add("Moneyness", new com.ivp.rad.controls.neogrid.Formatters.CustomFormatter() { DataType = com.ivp.rad.controls.neogrid.Formatters.FormatterDataType.Percentage, DecimalPlaces = 2 });

                service.SaveGridDataInCache(dt, username, divId, curPageId, viewKey, sessionId, false, gridInfo);
            }
        }

        private static List<CustomRowDataInfo> GetCustomRowDataInfoQuantDataSM(DataTable dt)
        {
            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            List<CustomCellDatainfo> lstCustomCellDataInfo = new List<CustomCellDatainfo>();
            CustomRowDataInfo objCustomRowDataInfo = null;
            if (dt.Rows.Count > 0)
            {
                //DataTable dt = ds.Tables[0];
                foreach (DataRow dr1 in dt.Rows)
                {
                    objCustomRowDataInfo = new CustomRowDataInfo();
                    //dr1["row_keys"] = Convert.ToString(dr1["row_id"]) + "||" + Convert.ToString(dr1["Security ID"]) + "||" + Convert.ToString(dr1["attributeID"]) + "||" + Convert.ToString(dr1["table_name"]);
                    lstCustomCellDataInfo = new List<CustomCellDatainfo>();
                    objCustomRowDataInfo.RowID = Convert.ToString(dr1["row_id"]);

                    lstCustomCellDataInfo.Add(new CustomCellDatainfo { ColumnName = "Security Name", NewChild = "<div id='divSecurityName" + Convert.ToString(dr1["Security Name"]) + "' style='text-decoration:underline;cursor:pointer;color:#00BFF0;'>" + Convert.ToString(dr1["Security Name"]) + "</div>" });

                    //if (String.IsNullOrEmpty(dr1["Expires On"].ToString()))
                    //{
                    //    dr1["Expires On"] = "Never";
                    //}

                    objCustomRowDataInfo.Attribute.Add("SecurityName", Convert.ToString(dr1["Security Name"]));
                    objCustomRowDataInfo.Cells = lstCustomCellDataInfo;
                    lstCustomRowDataInfo.Add(objCustomRowDataInfo);
                }
            }
            return lstCustomRowDataInfo;
        }

        public List<SRMQuantFilterInfo> GetQuantIntellisenseDataSM()
        {
            return SRMQuantController.GetQuantIntellisenseData();
        }

        public List<SRMQuantSavedSearches> GetQuantSavedSearches(string userName)
        {
            return SRMQuantController.GetQuantSavedSearches(userName);
        }

        public SRMQuantResponse InsertUpdateQuantSavedSearch(string searchID, string userName, string searchName, string searchQuery, string searchEncodedQuery)
        {
            return SRMQuantController.InsertUpdateQuantSavedSearch(searchID, userName, searchName, searchQuery, searchEncodedQuery);
        }

        public SRMQuantResponse DeleteQuantSavedSearch(string searchID, string userName)
        {
            return SRMQuantController.DeleteQuantSavedSearch(searchID, userName);
        }

        public ScreenerDataInfo GetSecurityData(string secName)
        {
            ScreenerDataInfo methodResponse = new ScreenerDataInfo();
            try
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"DECLARE @secName VARCHAR(MAX);
                                                    DECLARE @secID VARCHAR(MAX);
                                                    SET @secID = '';
                                                    SET @secName = '" + secName + @"';
                                                    
                                                    SELECT @secID = sec_id FROM [ivpsecmaster].[dbo].[SecType_equity_common_stock] WHERE security_name = '" + secName + @"'
                                                    
                                                    SELECT [Security Name], [Issuer], [Shares Outstanding], [High Price], [Low Price], [Close Price], [Bid Price], [Ask Price], [Volume], [GICS Sector], [GICS Industry Group], [GICS Industry], [GICS Sub Industry] FROM [IVPSecMaster].[dbo].[rmvwEquityCommonStock] WHERE [Security Id] = @secID

                                                    SELECT [Effective Start Date], [Last Price], Volume FROM IVPSecMaster.dbo.equities_screener_data WHERE [Security Id] = @secID ORDER BY [Effective Start Date]
                                                    ", ConnectionConstants.SecMaster_Connection);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 1)
                {
                    methodResponse.securityData = new Dictionary<string, string>();
                    foreach (DataColumn dcol in ds.Tables[0].Columns)
                    {
                        string colName = dcol.ColumnName;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string val = string.Empty;
                            try
                            {
                                val = Convert.ToDecimal(dr[colName]).ToString("G29");
                            }
                            catch (Exception e)
                            {
                                val = Convert.ToString(dr[colName]);
                            }

                            if (!methodResponse.securityData.ContainsKey(colName))
                                methodResponse.securityData.Add(colName, val);
                        }
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        StringBuilder finalStringPrice = new StringBuilder();
                        finalStringPrice.AppendLine("[");

                        StringBuilder finalStringVolume = new StringBuilder();
                        finalStringVolume.AppendLine("[");

                        foreach (DataRow dr in ds.Tables[1].Rows)
                        {
                            DateTime date = Convert.ToDateTime((Convert.ToString(dr["Effective Start Date"])));
                            string effStartDate = (Convert.ToInt64((DateTime.UtcNow.Date.AddDays(-(DateTime.Today - date).Days) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds)).ToString();
                            try
                            {
                                finalStringPrice.Append("[").Append(effStartDate).Append(", ").Append(Convert.ToInt32(dr["Last Price"])).AppendLine("],");
                                finalStringVolume.Append("[").Append(effStartDate).Append(", ").Append(Convert.ToInt32(dr["Volume"])).AppendLine("],");
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        finalStringPrice.Length = finalStringPrice.Length - 3;
                        finalStringPrice.AppendLine();

                        finalStringPrice.AppendLine("]");

                        methodResponse.securityHistoricalLastPriceData = finalStringPrice.ToString();

                        finalStringVolume.Length = finalStringVolume.Length - 3;
                        finalStringVolume.AppendLine();

                        finalStringVolume.AppendLine("]");

                        methodResponse.securityHistoricalVolumneData = finalStringVolume.ToString();
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return methodResponse;
        }

        public List<SRMQuantFilterInfo> GetReferenceData(string attributeName)
        {
            List<string> values = SRMQuantController.GetReferenceData(attributeName);
            List<SRMQuantFilterInfo> data = new List<SRMQuantFilterInfo>();

            foreach (var val in values)
                data.Add(new SRMQuantFilterInfo() { FilterName = val });

            return data;
        }
        #endregion

        public List<PrivilegeInfo> CheckControlPrivilegeForUserMultiple(List<string> privilegeName, string userName)
        {
            List<PrivilegeInfo> result = new List<PrivilegeInfo>();
            if (privilegeName == null)
                return result;
            privilegeName.ForEach(
                x => result.Add(new PrivilegeInfo { result = CheckControlPrivilegeForUser(x, userName), name = x })
                );

            return result;
        }

        public bool CheckControlPrivilegeForUser(string privilegeName, string userName)
        {
            mLogger.Debug("CommonService : CheckControlPrivilegeForUser -> Start");
            DataSet dsResult = null;
            try
            {
                if (userName == "admin")
                    return true;
                dsResult = (DataSet)CommonDALWrapper.ExecuteSelectQuery("EXEC IVPRefMaster.dbo.SRM_CheckControlPrivilegeForUser @privilege_name = '" + privilegeName + "', @user_name = '" + userName + "'",
                    ConnectionConstants.RefMaster_Connection);
                return Convert.ToBoolean(dsResult.Tables[0].Rows[0][0]);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return false;
            }
            finally { mLogger.Debug("CommonService: CommonCheckControlPrivilegeForUser -> End"); }
        }

        #region ApiMonitoring

        public List<ApiMonitoringApiCallsOutputInfoItem> GetApiCallsData(ApiMonitoringApiCallsInputInfo InputObject)
        {
            mLogger.Debug("CommonService -> GetApiCallsData -> Start");

            List<ApiMonitoringApiCallsOutputInfoItem> result = new List<ApiMonitoringApiCallsOutputInfoItem>();

            try
            {
                //Find length of Filters
                int selected_URL_Length = InputObject.selectedUrl.Count;
                int selected_Method_Length = InputObject.selectedMethod.Count;
                int selected_Client_Machine_Length = InputObject.selectedClientMachine.Count;

                //Create XML from the selected Multi-Select filters
                XDocument url_xml = new XDocument(
                                  new XElement("ListOfURL",
                                        Enumerable.Range(0, selected_URL_Length).Select(i => new XElement("ListItem", new XAttribute("ItemName", InputObject.selectedUrl[i]))))
                                    );
                XDocument method_xml = new XDocument(
                                  new XElement("ListOfMethod",
                                        Enumerable.Range(0, selected_Method_Length).Select(i => new XElement("ListItem", new XAttribute("ItemName", InputObject.selectedMethod[i]))))
                                    );
                XDocument client_machine_xml = new XDocument(
                                  new XElement("ListOfClientMachine",
                                        Enumerable.Range(0, selected_Client_Machine_Length).Select(i => new XElement("ListItem", new XAttribute("ItemName", InputObject.selectedClientMachine[i]))))
                                    );

                string start_date_time = null;
                start_date_time = InputObject.selectedStartDate != "null" ? InputObject.selectedStartDate : start_date_time;

                string end_date_time = null;
                end_date_time = InputObject.selectedEndDate != "null" ? InputObject.selectedEndDate : end_date_time;

                string sysShortFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                //ServerFormat obj = new ServerFormat();
                //string start_date = "";
                //string end_date = "";
                DateTime startDateDT, endDateDT;
                if (!string.IsNullOrEmpty(start_date_time))
                {
                    DateTime.TryParseExact(start_date_time, new RCommon().SessionInfo.CultureInfo.ShortDateFormat, CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.ToString()), DateTimeStyles.None, out startDateDT);
                    start_date_time = startDateDT.ToString(sysShortFormat);
                }

                if (!string.IsNullOrEmpty(end_date_time))
                {
                    DateTime.TryParseExact(end_date_time, new RCommon().SessionInfo.CultureInfo.ShortDateFormat, CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.ToString()), DateTimeStyles.None, out endDateDT);
                    end_date_time = endDateDT.ToString(sysShortFormat);
                }


                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMaster.dbo.SRM_GetAPICallsData '{0}', '{1}', '{2}', '{3}', '{4}'", url_xml, method_xml, client_machine_xml, start_date_time, end_date_time), ConnectionConstants.RefMaster_Connection);


                //Populate the data into result variable
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ApiMonitoringApiCallsOutputInfoItem item = new ApiMonitoringApiCallsOutputInfoItem();

                        //item.text = Convert.ToString(ds.Tables[0].Rows[i][0]);
                        //item.value = Convert.ToString(ds.Tables[0].Rows[i][0]);
                        item.ApiUniqueId = Convert.ToString(ds.Tables[0].Rows[i][0]);
                        item.ChainName = "API_Call_" + item.ApiUniqueId;
                        item.ChainURL = Convert.ToString(ds.Tables[0].Rows[i][1]);
                        item.ChainMethod = Convert.ToString(ds.Tables[0].Rows[i][2]);
                        item.ChainClientMachine = Convert.ToString(ds.Tables[0].Rows[i][3]);
                        item.ChainClientIP = Convert.ToString(ds.Tables[0].Rows[i][4]);
                        item.ChainPort = Convert.ToInt32(ds.Tables[0].Rows[i][5]);
                        var tempStartDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i][6]);
                        item.ChainStartDateTime = tempStartDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff");

                        if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[i]["thread_id"])))
                            item.ThreadId = Convert.ToInt32(ds.Tables[0].Rows[i]["thread_id"]);

                        // Handling Null Exception for End Date Time and accordingly compute ChainTimeTaken
                        if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[i][7])))
                        {
                            var tempEndDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i][7]);
                            item.ChainEndDateTime = tempEndDateTime.ToString("MM/dd/yyyy HH:mm:ss.fff");

                            TimeSpan time_taken = Convert.ToDateTime(ds.Tables[0].Rows[i][7]) - Convert.ToDateTime(ds.Tables[0].Rows[i][6]);

                            item.ChainTimeTaken = new List<int>();
                            item.ChainTimeTaken.Add(time_taken.Hours);           //Hour
                            item.ChainTimeTaken.Add(time_taken.Minutes);         //Minutes
                            item.ChainTimeTaken.Add(time_taken.Seconds);         //Seconds
                            item.ChainTimeTaken.Add(time_taken.Milliseconds);    //Milli - Seconds

                            //item.ChainTimeTaken = Convert.ToString(Convert.ToDateTime(ds.Tables[0].Rows[i][7]) - Convert.ToDateTime(ds.Tables[0].Rows[i][6]));
                        }
                        else
                        {
                            item.ChainEndDateTime = "NA";

                            item.ChainTimeTaken = new List<int>();
                            item.ChainTimeTaken.Add(-1);
                            item.ChainTimeTaken.Add(-1);
                            item.ChainTimeTaken.Add(-1);
                            item.ChainTimeTaken.Add(-1);

                        }

                        // JSON or XML -- Request Body
                        if (Convert.ToString(ds.Tables[0].Rows[i][8]) == "False")
                        {
                            item.ChainDetailsDataFormatRequest = "XML";
                        }
                        else if (Convert.ToString(ds.Tables[0].Rows[i][8]) == "True")
                        {
                            item.ChainDetailsDataFormatRequest = "JSON";
                        }

                        //Create Object to add Details
                        //item.ChainDetailsData = new ApiMonitoringApiCallDetailsData();        - No need. Done in Constructor.

                        //RequestHeaderData is a NON-NULL Attribute in DB. So, no need to check for NULL Exception
                        item.ChainDetailsData.RequestHeaderData = Convert.ToString(ds.Tables[0].Rows[i][9]);


                        // Handling Null Exception for Request Body
                        if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[i][10])))
                        {
                            item.ChainDetailsData.RequestBodyDataFileLocation = Convert.ToString(ds.Tables[0].Rows[i][10]);
                            item.ChainDetailsData.RequestBodyDataFileName = Path.GetFileName(item.ChainDetailsData.RequestBodyDataFileLocation);

                            //item.ChainDetailsData.RequestBodyDataToShow = false;

                            //try
                            //{
                            //    if (System.IO.File.Exists(item.ChainDetailsData.RequestBodyDataFileLocation))
                            //    {
                            //        FileInfo f1 = new FileInfo(item.ChainDetailsData.RequestBodyDataFileLocation);

                            //        //FileSize is in Bytes
                            //        var fileSize = f1.Length;

                            //        //If fileSize is less than or equal to 1MB, then we render the data on UI.
                            //        if (fileSize <= 1000000)
                            //        {
                            //            item.ChainDetailsData.RequestBodyDataToShow = true;
                            //        }
                            //        else
                            //        {
                            //            item.ChainDetailsData.RequestBodyErrorMsg = "The Request Body is too Large to display. Please Download the file.";
                            //        }
                            //    }
                            //    else
                            //    {
                            //        item.ChainDetailsData.RequestBodyErrorMsg = "The Request Body File Not Found at specified location.";
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    mLogger.Error("CommonService -> GetApiCallsData -> Exception ->" + ex.ToString());
                            //}
                        }
                        else
                        {
                            item.ChainDetailsData.RequestBodyDataFileLocation = "";
                            item.ChainDetailsData.RequestBodyDataFileName = "";
                            //item.ChainDetailsData.RequestBodyDataToShow = false;
                            item.ChainDetailsData.RequestBodyErrorMsg = "No Request Body available. Please check server logs for more details.";
                        }


                        // Handling Null Exception for Response Header
                        if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[i][11])))
                        {
                            item.ChainDetailsData.ResponseHeaderData = Convert.ToString(ds.Tables[0].Rows[i][11]);
                        }
                        else
                        {
                            item.ChainDetailsData.ResponseHeaderData = "";
                        }


                        // Handling Null Exception for Response Body
                        if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[i][12])))
                        {
                            item.ChainDetailsData.ResponseBodyDataFileLocation = Convert.ToString(ds.Tables[0].Rows[i][12]);
                            item.ChainDetailsData.ResponseBodyDataFileName = Path.GetFileName(item.ChainDetailsData.ResponseBodyDataFileLocation);

                            //item.ChainDetailsData.ResponseBodyDataToShow = false;

                            //try
                            //{
                            //    if (System.IO.File.Exists(item.ChainDetailsData.ResponseBodyDataFileLocation))
                            //    {
                            //        FileInfo f1 = new FileInfo(item.ChainDetailsData.ResponseBodyDataFileLocation);

                            //        //FileSize is in Bytes
                            //        var fileSize = f1.Length;

                            //        //If fileSize is less than or equal to 1MB, then we render the data on UI.
                            //        if (fileSize <= 1000000)
                            //        {
                            //            item.ChainDetailsData.ResponseBodyDataToShow = true;
                            //        }
                            //        else
                            //        {
                            //            item.ChainDetailsData.ResponseBodyErrorMsg = "The Response Body is too Large to display. Please Download the file.";
                            //        }
                            //    }
                            //    else
                            //    {
                            //        item.ChainDetailsData.ResponseBodyErrorMsg = "The Response Body File Not Found at specified location.";
                            //    }
                            //}
                            //catch (Exception ex)
                            //{
                            //    mLogger.Error("CommonService -> GetApiCallsData -> Exception ->" + ex.ToString());
                            //}
                        }
                        else
                        {
                            item.ChainDetailsData.ResponseBodyDataFileLocation = "";
                            item.ChainDetailsData.ResponseBodyDataFileName = "";
                            //item.ChainDetailsData.ResponseBodyDataToShow = false;
                            item.ChainDetailsData.ResponseBodyErrorMsg = "No Response Body available. Please check server logs for more details.";
                        }

                        // JSON or XML -- Response Body
                        if (Convert.ToString(ds.Tables[0].Rows[i][13]) == "False")
                        {
                            item.ChainDetailsDataFormatResponse = "XML";
                        }
                        else if (Convert.ToString(ds.Tables[0].Rows[i][13]) == "True")
                        {
                            item.ChainDetailsDataFormatResponse = "JSON";
                        }


                        result.Add(item);
                    }

                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetApiCallsData -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetApiCallsData -> End");
            }
            return result;
        }

        public ApiMonitoringFiltersOutput GetFiltersData()
        {
            mLogger.Debug("CommonService -> GetFiltersData -> Start");

            ApiMonitoringFiltersOutput result = new ApiMonitoringFiltersOutput();

            try
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMaster.dbo.SRM_GetAPIFiltersData"), ConnectionConstants.RefMaster_Connection);

                //Populate URL Filters Data
                List<ApiMonitoringFilterOutputInfoItem> inner_result1 = new List<ApiMonitoringFilterOutputInfoItem>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ApiMonitoringFilterOutputInfoItem item = new ApiMonitoringFilterOutputInfoItem();

                        item.text = Convert.ToString(ds.Tables[0].Rows[i][0]);
                        item.value = Convert.ToString(ds.Tables[0].Rows[i][0]);

                        inner_result1.Add(item);
                    }


                }

                result.URLFiltersData = inner_result1;

                //Populate Method Filters Data
                List<ApiMonitoringFilterOutputInfoItem> inner_result2 = new List<ApiMonitoringFilterOutputInfoItem>();

                if (ds != null && ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        ApiMonitoringFilterOutputInfoItem item = new ApiMonitoringFilterOutputInfoItem();

                        item.text = Convert.ToString(ds.Tables[1].Rows[i][0]);
                        item.value = Convert.ToString(ds.Tables[1].Rows[i][0]);

                        inner_result2.Add(item);
                    }
                }

                result.MethodFiltersData = inner_result2;


                //Populate Client Machine Filters Data
                List<ApiMonitoringFilterOutputInfoItem> inner_result3 = new List<ApiMonitoringFilterOutputInfoItem>();

                if (ds != null && ds.Tables.Count > 2 && ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                    {
                        ApiMonitoringFilterOutputInfoItem item = new ApiMonitoringFilterOutputInfoItem();

                        item.text = Convert.ToString(ds.Tables[2].Rows[i][0]);
                        item.value = Convert.ToString(ds.Tables[2].Rows[i][0]);

                        inner_result3.Add(item);
                    }
                }

                result.ClientMachineFiltersData = inner_result3;
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetFiltersData -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetFiltersData -> End");
            }

            return result;
        }

        public ApiMonitoringFileContentOutput GetFileContent(ApiMonitoringFileContentInput InputObject)
        {
            mLogger.Debug("CommonService -> GetFileContent -> Start");

            ApiMonitoringFileContentOutput result = new ApiMonitoringFileContentOutput();

            try
            {
                result.DataFormat = InputObject.DataFormat;
                result.RequestOrResponse = InputObject.RequestOrResponse;

                if (System.IO.File.Exists(InputObject.BodyFileLocation))
                {
                    result.FileExist = true;

                    FileInfo f1 = new FileInfo(InputObject.BodyFileLocation);

                    //FileSize is in Bytes
                    var fileSize = f1.Length;

                    //If fileSize is less than or equal to 1MB, then we render the data on UI.
                    if (fileSize <= 1000000)
                    {
                        result.BodyDataToShow = true;

                        // Handling Null Exception for Body File Location
                        //if (!string.IsNullOrEmpty(InputObject.BodyFileLocation))
                        //{
                        //Read Body File Data
                        result.BodyFileContent = System.IO.File.ReadAllText(InputObject.BodyFileLocation);
                        //}
                        //else
                        //{
                        //    result.BodyFileContent = "";
                        //}

                    }
                    else
                    {
                        result.BodyErrorMsg = "The " + result.RequestOrResponse + " Body is too Large to display. Please Download the file.";
                    }
                }
                else
                {
                    result.FileExist = false;
                    result.BodyErrorMsg = "The " + result.RequestOrResponse + " Body File Not Found at specified location.";
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetFileContent -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetFileContent -> End");
            }
            return result;
        }

        public ApiMonitoringApiCallLogOutput GetApiCallLogData(ApiMonitoringApiCallLogInput InputObject)
        {
            mLogger.Debug("CommonService -> GetApiCallLogData -> Start");

            ApiMonitoringApiCallLogOutput result = new ApiMonitoringApiCallLogOutput();

            try
            {
                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMaster.dbo.SRM_GetAPICallLogData '{0}'", InputObject.ApiCallId), ConnectionConstants.RefMaster_Connection);

                //Populate the data into result variable
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ApiMonitoringApiCallLogOutputItem item = new ApiMonitoringApiCallLogOutputItem();

                        item.ApiCallDetailsId = Convert.ToInt32(ds.Tables[0].Rows[i][0]);

                        var tempLogTime = Convert.ToDateTime(ds.Tables[0].Rows[i][1]);
                        item.LogTime = tempLogTime.ToString("MM/dd/yyyy HH:mm:ss.fff");

                        item.LogMessage = Convert.ToString(ds.Tables[0].Rows[i][2]);

                        result.ApiCallLogItemsList.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetApiCallLogData -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetApiCallLogData -> End");
            }

            return result;
        }

        #endregion


        #region UniquenessSetup

        public List<SectypeInfo> UniquenessSetupGetAllSectypes()
        {
            return new UniquenessSetupController().UniquenessSetupGetAllSectypes();
        }

        public List<UniquenessSetupKeyInfo> GetUniqueKeysForSelectedSectypes(List<int> selectedSectypes)
        {
            return new UniquenessSetupController().GetUniqueKeysForSelectedSectypes(selectedSectypes);
        }

        //Handled in UI Itself
        /*public List<UniquenessSetupKeyInfo> SearchUniqueKeys(List<int> selectedSectypes, string searchString)
        {
            mLogger.Debug("CommonService -> SearchUniqueKeys -> Start");

            List<UniquenessSetupKeyInfo> result = new List<UniquenessSetupKeyInfo>();

            try
            {
                string selectedSectypesString = "";

                if (selectedSectypes.Count > 0)
                {
                    selectedSectypesString = Convert.ToString(selectedSectypes[0]);

                    if (selectedSectypes.Count > 1)
                    {
                        for (int i = 1; i < selectedSectypes.Count; i++)
                            selectedSectypesString += ("," + Convert.ToString(selectedSectypes[i]));
                    }
                }

                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_SearchUniqueKeys '{0}', '{1}'", selectedSectypesString, searchString), ConnectionConstants.SecMaster_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    result = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Key_Name = x.Field<string>("Key_Name"), Is_Master = x.Field<bool>("Is_Master"), Is_Across_Securities = x.Field<bool>("Is_Across_Securities") }).Select(x => new UniquenessSetupKeyInfo() { KeyID = Convert.ToInt32(x.Key.Key_ID), KeyName = Convert.ToString(x.Key.Key_Name), IsMaster = Convert.ToBoolean(x.Key.Is_Master), IsAcrossSecurities = Convert.ToBoolean(x.Key.Is_Across_Securities), SelectedSectypes = x.Select(y => Convert.ToInt32(y["Sectype_ID"])).ToList() }).ToList();
                    //result = dt.AsEnumerable().GroupBy(x => x.Field<string>("Key_ID")).Select(x => new UniquenessSetupKeyInfo() { KeyID = Convert.ToInt32(x.Key) }).ToList();

                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].IsMaster == true)
                        {
                            result[i].SelectedAttributes = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Attribute_Name = x.Field<string>("Attribute_Name") }).Where(pp => Convert.ToInt32(pp.Key.Key_ID) == result[i].KeyID).Select(x => new UniquenessSetupAttributeInfo() { AttributeName = x.Key.Attribute_Name, AttributeIDs = string.Join(",", x.Select(y => Convert.ToString(y["Attribute_ID"]))), AreAdditionalLegAttributes = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).ToList();
                        }
                        else
                        {
                            result[i].SelectedLeg = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Leg_Name = x.Field<string>("Leg_Name") }).Where(pp => Convert.ToInt32(pp.Key.Key_ID) == result[i].KeyID).Select(x => new UniquenessSetupLegInfo() { LegName = x.Key.Leg_Name, LegIDs = string.Join(",", x.Select(y => Convert.ToString(y["Additional_Leg_ID"]))), AreAdditionalLegs = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).First();

                            result[i].SelectedLegAttributes = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Leg_Name = x.Field<string>("Leg_Name"), Leg_Attribute_Name = x.Field<string>("Attribute_Name") }).Where(pp => Convert.ToInt32(pp.Key.Key_ID) == result[i].KeyID).Select(x => new UniquenessSetupAttributeInfo() { AttributeName = x.Key.Leg_Attribute_Name, AttributeIDs = string.Join(",", x.Select(y => Convert.ToString(y["Attribute_ID"]))), AreAdditionalLegAttributes = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> SearchUniqueKeys -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> SearchUniqueKeys -> End");
            }

            return result;
        }
        */

        public List<UniquenessSetupLegInfo> GetCommonLegsForSelectedSectypes(List<int> selectedSectypes)
        {
            return new UniquenessSetupController().GetCommonLegsForSelectedSectypes(selectedSectypes);
        }

        public UniquenessSetupCommonMasterAttributesOutputInfo GetCommonMasterAttributesForSelectedSectypes(string userName, List<int> selectedSectypes, int KeyID)
        {
            return new UniquenessSetupController().GetCommonMasterAttributesForSelectedSectypes(userName, selectedSectypes, KeyID);
        }

        public UniquenessSetupCommonLegAttributesOutputInfo GetCommonLegAttributesForSelectedLegName(UniquenessSetupLegInfo InputObject, int KeyID)
        {
            return new UniquenessSetupController().GetCommonLegAttributesForSelectedLegName(InputObject, KeyID);
        }

        public UniquenessSetupOutputObject CreateUniqueKey(string userName, UniquenessSetupKeyInfo InputObject)
        {
            return new UniquenessSetupController().CreateUniqueKey(userName, InputObject);
        }

        public UniquenessSetupOutputObject UpdateUniqueKey(string userName, UniquenessSetupKeyInfo InputObject)
        {
            return new UniquenessSetupController().UpdateUniqueKey(userName, InputObject);
        }

        public int DeleteUniqueKey(string userName, int keyID)
        {
            return new UniquenessSetupController().DeleteUniqueKey(userName, keyID);
        }

        public UniquenessSetupFileDownloadInfo DownloadAllUniqueKeys()
        {
            mLogger.Debug("CommonService -> DownloadAllUniqueKeys -> Start ");
            DataSet dsResult = null;
            UniquenessSetupFileDownloadInfo result = new UniquenessSetupFileDownloadInfo();
            try
            {
                dsResult = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"
                    DECLARE @final_key_table TABLE (Key_ID INT, [Key_Name] VARCHAR(max), Is_Master BIT, Is_Across_Securities BIT, Level_ID INT, Attribute_ID INT, Additional_Leg_ID INT, Sectype_ID INT, Is_Additional_Leg BIT, Attribute_Name VARCHAR(max), Leg_Name VARCHAR(max), Sectype_Name VARCHAR(max), Check_In_Drafts BIT, Check_In_Workflows BIT, Null_As_Unique BIT)

                    INSERT INTO @final_key_table(Key_ID, Key_Name, Is_Master, Is_Across_Securities, Level_ID, Attribute_ID, Additional_Leg_ID, Sectype_ID, Is_Additional_Leg, Check_In_Drafts, Check_In_Workflows, Null_As_Unique)
                        SELECT kmas.key_id, key_name, is_master, is_across_securities, level_id, attribute_id, additional_leg_id, sectype_id, is_additional_leg, check_in_drafts, check_in_workflows, null_as_unique
                        FROM [IVPSecMaster].[dbo].[ivp_secm_unique_key_master] kmas (NOLOCK)
                        INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_unique_key_details] kdet (NOLOCK)
                            ON  kdet.key_id = kmas.key_id
                        WHERE kmas.is_active=1

                    --System Level Unique Keys : Added with Sectypes
                    INSERT INTO @final_key_table(Key_ID, Key_Name, Is_Master, Is_Across_Securities, Level_ID, Attribute_ID, Additional_Leg_ID, Sectype_ID, Is_Additional_Leg, Attribute_Name, Leg_Name, Check_In_Drafts, Check_In_Workflows, Null_As_Unique)
                        SELECT fkt.Key_ID, fkt.Key_Name, fkt.Is_Master, fkt.Is_Across_Securities, fkt.Level_ID, fkt.Attribute_ID, fkt.Additional_Leg_ID, smas.sectype_id, fkt.Is_Additional_Leg, fkt.Attribute_Name, fkt.Leg_Name, fkt.Check_In_Drafts, fkt.Check_In_Workflows, fkt.Null_As_Unique
                        FROM @final_key_table fkt
                        JOIN IVPSecMaster.dbo.ivp_secm_sectype_master smas
                            ON fkt.Sectype_ID = -1
                        WHERE smas.is_complete=1 AND smas.is_active=1

                    --Removed the -1 entry for System Level Unique Keys
                    DELETE FROM @final_key_table
                    WHERE Sectype_ID = -1

                    -- Fetch Leg Names
                    UPDATE fkt
                        SET Leg_Name = aleg.additional_legs_name
                    FROM @final_key_table fkt
                    INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_additional_legs] aleg (NOLOCK)
                        ON aleg.additional_legs_id=fkt.Additional_Leg_ID
                    WHERE fkt.Is_Master=0

                    --Fetch Attribute Names for Additional Leg Attributes
                    UPDATE fkt
                        SET Attribute_Name = aladet.display_name
                    FROM @final_key_table fkt
                    INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_additional_legs_attribute_details] aladet (NOLOCK)
                        ON aladet.attribute_id = fkt.Attribute_ID AND fkt.Is_Additional_Leg=1
                    WHERE fkt.Is_Master=0
        
                    --Fetch Attribute Names for Non-Additional Leg Attributes
                    UPDATE fkt
                        SET Attribute_Name = tdet.display_name
                    FROM @final_key_table fkt
                    INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_details] adet (NOLOCK)
                        ON adet.attribute_id=fkt.Attribute_ID
                    INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_template_master] tmas (NOLOCK)
                        ON (tmas.sectype_id = fkt.Sectype_ID AND tmas.is_active = 1 AND tmas.dependent_id='SYSTEM')
                    INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_template_details] tdet (NOLOCK)
                        ON (tmas.template_id = tdet.template_id AND tdet.attribute_id = adet.attribute_id AND tdet.is_active = 1)
                    WHERE fkt.Is_Master=1 AND fkt.Is_Additional_Leg=0

                    --Fetch Security Type Names
                    UPDATE fkt
                    SET Sectype_Name = smas.sectype_name
                    FROM @final_key_table fkt
                    INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master smas
                        ON smas.sectype_id = fkt.Sectype_ID

                    -- Select statement
                    SELECT CASE WHEN Is_Master=1 THEN 'Attribute Level' ELSE 'Leg Level' END AS [Level], [Key_Name] AS [Key Name], Sectype_Name AS [Security Type], Leg_Name AS [Leg Name], Attribute_Name AS [Attribute Name], CASE WHEN Is_Across_Securities=1 THEN 'Yes' ELSE 'No' END AS [Is Across Securities], CASE WHEN Check_In_Drafts=1 THEN 'Yes' ELSE 'No' END AS [Check In Drafts], CASE WHEN Check_In_Workflows=1 THEN 'Yes' ELSE 'No' END AS [Check In Workflows], CASE WHEN Null_As_Unique=1 THEN 'Yes' ELSE 'No' END AS [Consider Null As Unique]
                    FROM @final_key_table
                    Order By [Level], [Key Name], [Security Type], [Leg Name], [Attribute Name]"), ConnectionConstants.SecMaster_Connection);

                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                {
                    result.guidString = Guid.NewGuid().ToString();
                    dsResult.Tables[0].TableName = "Unique Keys";

                    File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/" + result.guidString + ".xlsx"), SRMCommon.GetExcelByteArray(dsResult));

                    result.isSuccess = "1" + "@&@" + "Successfully retrieved the Unique Keys from DB.";
                }
                else
                {
                    result.isSuccess = "0" + "@&@" + "No Unique Keys exist in the system.";
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> DownloadAllUniqueKeys -> Exception ->" + ex.ToString());
                result.isSuccess = "0" + "@&@" + ex.ToString();
            }
            finally
            {
                mLogger.Debug("CommonService -> DownloadAllUniqueKeys -> End");
            }

            return result;
        }

        #endregion


        #region CASourcePrioritization

        //public List<CASP_VendorData> CASP_GetAllVendorsForCorpActionType(int corpActionTypeID)
        //{
        //    mLogger.Debug("CommonService -> CASP_GetAllVendors -> Start");
        //    List<CASP_VendorData> result = new List<CASP_VendorData>();
        //    try
        //    {                
        //    }
        //    catch(Exception ex)
        //    {
        //        mLogger.Error("CommonService -> CASP_GetAllVendors -> Exception ->" + ex.ToString());
        //    }
        //    finally
        //    {
        //        mLogger.Debug("CommonService -> CASP_GetAllVendors -> End");
        //    }
        //    return result;
        //}

        public List<CASPM_CorpActionType> CASPM_GetCorpActionTypesData()
        {
            mLogger.Debug("CommonService -> CASP_GetCorpActionTypesData -> Start");
            List<CASPM_CorpActionType> result = new List<CASPM_CorpActionType>();
            try
            {
                DataSet dsCorpActionTypes = CommonDALWrapper.ExecuteSelectQuery(@"USE IVPCorpActionVendor
                                                                                    SELECT  mas.corpaction_type_id, mas.corpaction_type_name AS corpaction_type, 
                                                                                        CASE WHEN 
                                                                                        (
                                                                                            SELECT count(*) 
                                                                                            FROM [IVPCorpActionVendor].[dbo].[ivp_cav_corpaction_type_feed_mapping_master] (NOLOCK) 
                                                                                            WHERE corpaction_type_id = mas.corpaction_type_id AND corpaction_type_feed_mapping_id IN 
                                                                                            (
                                                                                                SELECT corpaction_type_feed_mapping_id 
                                                                                                FROM [IVPCorpActionVendor].[dbo].[ivp_cav_corpaction_type_feed_mapping_master] (NOLOCK) 
                                                                                                WHERE corpaction_type_id = mas.corpaction_type_id
                                                                                            )
                                                                                        ) > 0 THEN 'TRUE' ELSE 'FALSE' END AS [status],
                                                                                        CASE WHEN 
                                                                                        (
                                                                                            SELECT DISTINCT template_id 
                                                                                            FROM [IVPCorpAction].[dbo].[ivp_ca_template_details] (NOLOCK) 
                                                                                            WHERE template_id = tem.template_id AND selected_vendor_id IS NOT NULL 
                                                                                        ) >= 1 THEN 'Set' ELSE 'Not Set' END AS IsPrioritized 
                                                                                    FROM [IVPCorpAction].[dbo].[ivp_ca_corpaction_type_master] AS mas (NOLOCK) 
                                                                                    INNER JOIN [IVPCorpAction].[dbo].[ivp_ca_template_master] AS tem (NOLOCK) 
                                                                                        ON tem.corpaction_type_id = mas.corpaction_type_id
                                                                                    WHERE mas.is_active = 'TRUE' AND tem.is_active = 'TRUE' AND tem.dependent_id = 'SYSTEM' ORDER BY corpaction_type", ConnectionConstants.CorpActionVendor_Connection);

                if (dsCorpActionTypes != null && dsCorpActionTypes.Tables.Count > 0 && dsCorpActionTypes.Tables[0] != null)
                {
                    int length = dsCorpActionTypes.Tables[0].Rows.Count;

                    for (int i = 0; i < length; i++)
                    {
                        CASPM_CorpActionType item = new CASPM_CorpActionType();
                        item.CorpActionTypeID = Convert.ToInt32(dsCorpActionTypes.Tables[0].Rows[i][0]);
                        item.CorpActionTypeName = Convert.ToString(dsCorpActionTypes.Tables[0].Rows[i][1]);
                        item.Status = Convert.ToString(dsCorpActionTypes.Tables[0].Rows[i][2]);
                        item.IsPrioritized = Convert.ToString(dsCorpActionTypes.Tables[0].Rows[i][3]);
                        result.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> CASP_GetCorpActionTypesData -> Exception ->" + ex.ToString());
            }
            finally
            {
                mLogger.Debug("CommonService -> CASP_GetCorpActionTypesData -> End");
            }
            return result;
        }

        public CASP_GetDataOutputObject CASP_GetDataForCorpActionType(int corpActionTypeID)
        {
            mLogger.Debug("CommonService -> CASP_GetData -> Start");
            CASP_GetDataOutputObject result = new CASP_GetDataOutputObject();

            try
            {
                //Populated in CASP_Get_Vendor_Data region, but used in CASP_Get_Tab_Data to add vendor data to each priority in each attribute.
                Dictionary<int, CASP_VendorData> vendorListDict = new Dictionary<int, CASP_VendorData>();

                #region CASP_Get_Corp_Action_Type_Name
                DataSet dsCorpActionTypeName = CommonDALWrapper.ExecuteSelectQuery(@"SELECT corpaction_type_name FROM IVPCorpAction.dbo.ivp_ca_corpaction_type_master WHERE corpaction_type_id = " + corpActionTypeID, ConnectionConstants.CorpActionVendor_Connection);

                if (dsCorpActionTypeName != null && dsCorpActionTypeName.Tables.Count > 0 && dsCorpActionTypeName.Tables[0] != null)
                {
                    result.CorpActionTypeName = Convert.ToString(dsCorpActionTypeName.Tables[0].Rows[0][0]);
                }

                #endregion

                #region CASP_Get_Vendor_Data
                DataSet dsAllVendors = CommonDALWrapper.ExecuteSelectQuery(@"SELECT DISTINCT vmas.vendor_id as Vendor_ID, vmas.vendor_name as Vendor_Name, vmas.vendor_type_id as Vendor_Type_ID 
                                                                   FROM IVPCorpActionVendor.dbo.ivp_cav_corpaction_type_feed_mapping_master AS fmapmas (NOLOCK) 
                                                                   INNER JOIN [IVPCorpActionVendor].[dbo].[ivp_cav_feed_summary] AS fsum (NOLOCK) 
                                                                        ON fmapmas.feed_id = fsum.feed_id 
                                                                   INNER JOIN [IVPCorpActionVendor].[dbo].[ivp_cav_vendor_master] AS vmas (NOLOCK) 
                                                                        ON fsum.vendor_id = vmas.vendor_id 
                                                                   INNER JOIN [IVPCorpActionVendor].[dbo].[ivp_cav_vendor_type] AS vtyp (NOLOCK) 
                                                                        ON vtyp.vendor_type_id = vmas.vendor_type_id 
                                                                   WHERE fmapmas.corpaction_type_id = " + corpActionTypeID + @" AND fmapmas.is_active = 'true' AND vtyp.vendor_type = 'CorpAction Data Provider'",
                                                               ConnectionConstants.CorpActionVendor_Connection);

                if (dsAllVendors != null && dsAllVendors.Tables.Count > 0 && dsAllVendors.Tables[0] != null)
                {
                    DataRow drVendorCompareAndShow = dsAllVendors.Tables[0].NewRow();
                    drVendorCompareAndShow["Vendor_Name"] = "Vendor Mismatch";
                    drVendorCompareAndShow["Vendor_ID"] = -2;
                    drVendorCompareAndShow["Vendor_Type_ID"] = 2;
                    dsAllVendors.Tables[0].Rows.Add(drVendorCompareAndShow);

                    DataRow drDBValue = dsAllVendors.Tables[0].NewRow();
                    drDBValue["Vendor_Name"] = "Value Changed";
                    drDBValue["Vendor_ID"] = -3;
                    drDBValue["Vendor_Type_ID"] = 2;
                    dsAllVendors.Tables[0].Rows.Add(drDBValue);

                    DataRow drVendor = dsAllVendors.Tables[0].NewRow();
                    drVendor["Vendor_Name"] = "Show as Exception";
                    drVendor["Vendor_ID"] = 0;
                    drVendor["Vendor_Type_ID"] = 2;
                    dsAllVendors.Tables[0].Rows.Add(drVendor);

                    DataRow drSelect = dsAllVendors.Tables[0].NewRow();
                    drSelect["Vendor_Name"] = "--Select One--";
                    drSelect["Vendor_ID"] = -9;
                    drSelect["Vendor_Type_ID"] = "2";
                    dsAllVendors.Tables[0].Rows.InsertAt(drSelect, 0);


                    int length = dsAllVendors.Tables[0].Rows.Count;

                    for (int i = 0; i < length; i++)
                    {
                        CASP_VendorData item = new CASP_VendorData();
                        item.VendorID = Convert.ToInt32(dsAllVendors.Tables[0].Rows[i][0]);
                        item.VendorName = Convert.ToString(dsAllVendors.Tables[0].Rows[i][1]);
                        item.VendorTypeID = Convert.ToInt32(dsAllVendors.Tables[0].Rows[i][2]);

                        result.VendorData.Add(item);

                        vendorListDict.Add(item.VendorID, item);
                    }
                }
                #endregion

                #region CASP_Get_Tab_Data

                DataSet dsAttributeLevelPriority = CommonDALWrapper.ExecuteSelectQuery(@"USE IVPCorpActionVendor
                                                                             SELECT DISTINCT tdet.attribute_id as AttributeID, 
                                                                                        (
                                                                                            CASE WHEN (catab.corpaction_type_id<>0) 
                                                                                                    THEN table_display_name + '-' + tdet.display_name 
                                                                                                 ELSE tdet.display_name 
                                                                                            END
                                                                                        ) as AttributeDisplayName,
                                                                                        fully_qualified_table_name,
                                                                                        table_display_name as TableDisplayName,
                                                                                        catab.table_id as TableID,
                                                                                        tdet.selected_vendor_id as SelectedVendorID,
                                                                                        catab.priority as Priority,
                                                                                        catab.underlyer_exists 
                                                                             FROM [IVPCorpAction].[dbo].ivp_ca_template_master tmas (NOLOCK) 
                                                                             INNER JOIN [IVPCorpAction].[dbo].ivp_ca_template_details tdet (NOLOCK) 
                                                                                 ON (tmas.template_id = tdet.template_id AND tmas.dependent_id = 'SYSTEM') 
                                                                             INNER JOIN [IVPCorpAction].[dbo].ivp_ca_attribute_details adet (NOLOCK) 
                                                                                 ON (adet.attribute_id = tdet.attribute_id) 
                                                                             INNER JOIN  [IVPCorpAction].[dbo].ivp_ca_corpaction_type_table catab (NOLOCK) 
                                                                                 ON (catab.table_id = adet.table_id) 
                                                                             WHERE tmas.corpaction_type_id = " + corpActionTypeID + @" AND adet.attribute_name <> 'cleared_approval' 
                                                                             ORDER BY AttributeDisplayName ",

                                                                   ConnectionConstants.CorpActionVendor_Connection);


                if (dsAttributeLevelPriority != null && dsAttributeLevelPriority.Tables.Count > 0 && dsAttributeLevelPriority.Tables[0] != null && dsAttributeLevelPriority.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = dsAttributeLevelPriority.Tables[0];

                    result.TabData = dt.AsEnumerable().GroupBy(x => new { TabID = x.Field<int>("TableID"), TabName = x.Field<string>("TableDisplayName"), TabPriority = x.Field<int>("Priority") }).Select(x => new CASP_TabInfo() { TabID = Convert.ToInt32(x.Key.TabID), TabName = Convert.ToString(x.Key.TabName), TabPriority = Convert.ToInt32(x.Key.TabPriority), TabAttributesDetails = x.Select(y => new CASP_AttributeInfo() { AttributeID = y.Field<int>("AttributeID"), AttributeDisplayName = y.Field<string>("AttributeDisplayName"), AttributeVendorList = string.IsNullOrEmpty(y.Field<string>("SelectedVendorID")) ? new List<CASP_VendorData>() : y.Field<string>("SelectedVendorID").Split(',').Select(sValue => vendorListDict[Convert.ToInt32(sValue.Trim())]).ToList() }).ToList() }).ToList();

                }

                #endregion

            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> CASP_GetData -> Exception ->" + ex.ToString());
            }
            finally
            {
                mLogger.Debug("CommonService -> CASP_GetData -> End");
            }
            return result;
        }

        #endregion


        #region DraftsStatus
        public string GetDraftsDataSM(string username, string divID, string currPageID, string viewKey, string sessionID, string dateFormat, string jsonGridInfo, bool isFromDashboard, string sectypeList, bool isUserSpecific)
        {
            mLogger.Debug("Start ==> GetDraftsDataSM");
            int rowCount = 0;
            try
            {
                Assembly SMControllerAssembly = Assembly.Load("SecMasterCore");
                Type smSecurityInfo = null;
                smSecurityInfo = SMControllerAssembly.GetType("com.ivp.secm.core.SMSecurityInfo");
                object smSecurityInfoObj = null;
                smSecurityInfoObj = Activator.CreateInstance(smSecurityInfo);
                PropertyInfo[] secinfo = smSecurityInfoObj.GetType().GetProperties();
                foreach (PropertyInfo propinfo in secinfo)
                {
                    if (propinfo.Name.Equals("CreatedBy"))
                        propinfo.SetValue(smSecurityInfoObj, username, null);
                    if (propinfo.Name.Equals("DraftType"))
                        propinfo.SetValue(smSecurityInfoObj, 0, null);
                    if (isFromDashboard)
                    {
                        if (propinfo.Name.Equals("SecTypeIdList"))
                            propinfo.SetValue(smSecurityInfoObj, sectypeList, null);
                        if (propinfo.Name.Equals("IsUserSpecific"))
                            propinfo.SetValue(smSecurityInfoObj, isUserSpecific, null);
                        if (propinfo.Name.Equals("UserName"))
                            propinfo.SetValue(smSecurityInfoObj, username, null);
                    }
                }

                Type smSecurityController = SMControllerAssembly.GetType("com.ivp.secm.core.SMSecurityController");
                MethodInfo getDrafts = smSecurityController.GetMethod("GetDrafts");
                var smSecurityControllerObj = Activator.CreateInstance(smSecurityController);
                DataSet dsDrafts = (DataSet)getDrafts.Invoke(smSecurityControllerObj, new object[] { smSecurityInfoObj });

                //SMSecurityInfo objSMSecurityInfo = new SMSecurityInfo();
                //objSMSecurityInfo.CreatedBy = username;
                //objSMSecurityInfo.DraftType = DraftType.Manual;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ////returns a single row
                GridInfo gridInfo = serializer.Deserialize<GridInfo>(jsonGridInfo);
                //DataSet dsDrafts = new SMSecurityController().GetDrafts(objSMSecurityInfo);
                RADNeoGridService service = new RADNeoGridService();

                string customRowDataInfo = string.Empty;

                if (dsDrafts.Tables.Count > 0 && dsDrafts.Tables[0] != null && dsDrafts.Tables[0].Rows.Count > 0)
                {
                    rowCount = dsDrafts.Tables.Count;

                    //binding
                    dsDrafts.Tables[0].TableName = "draftsTable";
                    serializer.MaxJsonLength = Int32.MaxValue;
                    //com.ivp.rad.controls.neogrid.client.info.GridInfo gridInfo = new com.ivp.rad.controls.neogrid.client.info.GridInfo();

                    //gridInfo.CheckBoxInfo = new com.ivp.rad.controls.neogrid.client.info.CheckBoxInfo();
                    //gridInfo.SelectRecordsAcrossAllPages = true;
                    //gridInfo.ViewKey = viewKey;
                    //gridInfo.CacheGriddata = true;
                    //gridInfo.GridId = divID;
                    //gridInfo.CurrentPageId = currPageID;
                    //gridInfo.SessionIdentifier = sessionID;
                    //gridInfo.UserId = username;
                    List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo> columnsToHide = new List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo>();
                    ////columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "is_updated_today", isDefault = true });
                    ////columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "is_verified_corpaction", isDefault = true });
                    ////columnsToHide.Add(new rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "corpaction_type_id", isDefault = true });
                    //columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "sectype_id", isDefault = true });
                    //columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "template_id", isDefault = true });
                    //columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "sectype_description", isDefault = true });
                    //columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "row_keys", isDefault = true });
                    //gridInfo.ColumnsToHide = columnsToHide;
                    //gridInfo.Height = "400px";
                    //gridInfo.ColumnsNotToSum = new List<string>();
                    //gridInfo.RequireEditGrid = false;
                    //gridInfo.IdColumnName = "sec_id";
                    //gridInfo.TableName = "draftsTable";
                    //gridInfo.PageSize = 200;
                    //gridInfo.RequirePaging = true;
                    //gridInfo.RequireInfiniteScroll = true;
                    //gridInfo.DoNotExpand = false;
                    //gridInfo.ItemText = "Security Drafts";
                    //gridInfo.DoNotRearrangeColumn = true;
                    //gridInfo.RequireGrouping = true;
                    //gridInfo.RequireGroupExpandCollapse = true;
                    //gridInfo.RequireSelectedRows = true;
                    //gridInfo.RequireExportToExcel = true;
                    //gridInfo.RequireSearch = true;
                    //gridInfo.RequireResizing = true;
                    //gridInfo.RequireLayouts = false;

                    //gridInfo.RequireFilter = true;
                    //gridInfo.CssExportRows = "xlneoexportToExcel";
                    //gridInfo.HideFooter = false;
                    //gridInfo.RequireSort = true;
                    //gridInfo.RequireMathematicalOperations = false;
                    //gridInfo.RequireFreezeColumns = false;
                    //gridInfo.ExcelSheetName = "Security Drafts";

                    //gridInfo.ColumnDateFormatMapping.Add("Effective Date", dateFormat);

                    DataSet dsTemp = new DataSet("ROOT");
                    dsTemp.Tables.Add(dsDrafts.Tables[0].Copy());
                    dsTemp.Tables[0].Columns["sectype_name"].ColumnName = "Security Type";

                    dsTemp.Tables[0].Columns.Add("row_keys");
                    dsTemp.Tables[0].Columns.Add("Security Id", typeof(string));
                    dsTemp.Tables[0].Columns["Security Type"].SetOrdinal(0);
                    dsTemp.Tables[0].Columns["Security Id"].SetOrdinal(1);
                    foreach (DataRow dr in dsTemp.Tables[0].Rows)
                    {
                        dr["Security Id"] = dr["sec_id"];
                    }

                    List<string> privilegeList = new List<string>();
                    privilegeList.Add("Edit Security Drafts");
                    privilegeList.Add("View Security Drafts");

                    bool isClickableRow = false;
                    CheckControlPrivilegeForUserMultiple(privilegeList, username).ForEach(x => isClickableRow = isClickableRow || x.result);
                    if (isClickableRow)
                        gridInfo.CustomRowsDataInfo = GetCustomRowDataInfoDraftsSM(dsTemp);


                    service.SaveGridDataInCache(dsTemp.Tables[0], username, divID, currPageID, viewKey, sessionID, false, gridInfo);
                }
            }

            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                rowCount = 0;
            }
            finally
            {
                mLogger.Debug("End ==> GetDraftsDataSM");
            }
            return rowCount.ToString();
        }

        public string GetDraftsDataRM(string username, string divID, string currPageID, string viewKey, string sessionID, string dateFormat, int ModuleId, string jsonGridInfo)
        {
            mLogger.Debug("Start ==> GetDraftsDataRM");
            int rowCount = 0;
            try
            {
                Assembly RefMController = Assembly.Load("RefMController");
                Type rmDraftController = null;
                rmDraftController = RefMController.GetType("com.ivp.refmaster.controller.draft.RMDraftController");
                //object typeData = null;
                //typeData = Activator.CreateInstance(rmDraftController);

                MethodInfo getAllActiveDrafts = rmDraftController.GetMethod("GetAllActiveDrafts");
                var rmDraftControllerObj = Activator.CreateInstance(rmDraftController);
                DataSet dsDrafts = (DataSet)getAllActiveDrafts.Invoke(rmDraftControllerObj, new object[] { username, ModuleId });

                JavaScriptSerializer serializer = new JavaScriptSerializer();

                GridInfo gridInfo = serializer.Deserialize<GridInfo>(jsonGridInfo);
                string customRowDataInfo = string.Empty;
                RADNeoGridService service = new RADNeoGridService();
                //int rowCount = 0;


                if (dsDrafts.Tables.Count > 0 && dsDrafts.Tables[0] != null && dsDrafts.Tables[0].Rows.Count > 0)
                {
                    rowCount = dsDrafts.Tables.Count;

                    //binding
                    dsDrafts.Tables[0].TableName = "draftsTable";
                    serializer.MaxJsonLength = Int32.MaxValue;

                    //adsdasd
                    DataSet dsTemp = new DataSet("ROOT");
                    dsTemp.Tables.Add(dsDrafts.Tables[0].Copy());
                    //dsTemp.Tables[0].Columns["entity_code"].ColumnName = "Entity Code";
                    //dsTemp.Tables[0].Columns["entity_name"].ColumnName = "Entity Name";
                    dsTemp.Tables[0].Columns["created_on"].ColumnName = "Created On";
                    dsTemp.Tables[0].Columns["created_by"].ColumnName = "Created By";
                    dsTemp.Tables[0].Columns["last_modified_on"].ColumnName = "Updated On";
                    dsTemp.Tables[0].Columns["effective_start_date"].ColumnName = "Effective Start Date";
                    dsTemp.Tables[0].Columns["entity_type_name"].ColumnName = "Entity Type Name";

                    dsTemp.Tables[0].Columns.Add("Entity Code", typeof(string));
                    foreach (DataRow dr in dsTemp.Tables[0].Rows)
                    {
                        dr["Entity Code"] = dr["entity_code"];
                    }
                    //dsTemp.Tables[0].Columns.Add("row_keys");

                    dsTemp.Tables[0].Columns["Entity Code"].SetOrdinal(0);
                    dsTemp.Tables[0].Columns["Entity Type Name"].SetOrdinal(1);
                    dsTemp.Tables[0].Columns["Created On"].SetOrdinal(2);
                    dsTemp.Tables[0].Columns["Created By"].SetOrdinal(3);
                    dsTemp.Tables[0].Columns["Updated On"].SetOrdinal(4);
                    dsTemp.Tables[0].Columns["Effective Start Date"].SetOrdinal(5);

                    //dsTemp.Tables[0].Columns["Security Id"].SetOrdinal(1);
                    //foreach (DataRow dr in dsTemp.Tables[0].Rows)
                    //{
                    //    dr["Security Id"] = dr["sec_id"];
                    //}
                    List<string> privilegeList = new List<string>();

                    switch (ModuleId)
                    {
                        case (int)SRMModules.RefData:
                            privilegeList.Add("RM - Drafts - View Entity");
                            privilegeList.Add("RM - Drafts - Edit Entity");
                            break;
                        case (int)SRMModules.Funds:
                            privilegeList.Add("FM - Drafts - View Entity");
                            privilegeList.Add("FM - Drafts - Edit Entity");
                            break;
                        case (int)SRMModules.Parties:
                            privilegeList.Add("PM - Drafts - View Entity");
                            privilegeList.Add("PM - Drafts - Edit Entity");
                            break;
                    }
                    bool isClickableRow = false;
                    CheckControlPrivilegeForUserMultiple(privilegeList, username).ForEach(x => isClickableRow = isClickableRow || x.result);
                    if (isClickableRow)
                        gridInfo.CustomRowsDataInfo = GetCustomRowDataInfoDraftsRM(dsTemp);

                    gridInfo.ColumnDateFormatMapping.Add("Effective Start Date", dateFormat);
                    service.SaveGridDataInCache(dsTemp.Tables[0], username, divID, currPageID, viewKey, sessionID, false, gridInfo);
                }

                //return rowCount.ToString();
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                rowCount = 0;
            }
            finally
            {
                mLogger.Debug("End ==> GetDraftsDataRM");
            }
            return rowCount.ToString();
        }

        private static string GetText(object data)
        {
            string stringData = Convert.ToString(data);
            if (string.IsNullOrWhiteSpace(stringData))
                return "&nbsp;";
            else
                return stringData;
        }

        public string DraftsStatusGetCheckedRowsData(string cacheKey)
        {
            com.ivp.rad.controls.neogrid.service.RADNeoGridService service = new com.ivp.rad.controls.neogrid.service.RADNeoGridService();
            List<System.Xml.Linq.XElement> xml = service.GetCheckedRows(cacheKey);
            string result = "";
            // OverrideStatusRowInfo rowInfo = new OverrideStatusRowInfo();

            if (xml.Count > 0)
            {
                for (int index = 0; index < xml.Count; index++)
                {
                    if (xml[index].Elements("Security_x0020_Id").Any())
                        result += xml[index].Descendants("Security_x0020_Id").FirstOrDefault().Value + "|";
                    else if (xml[index].Elements("Entity_x0020_Code").Any() && xml[index].Elements("entity_type_id").Any())
                    {
                        result += xml[index].Descendants("Entity_x0020_Code").FirstOrDefault().Value + "_" +
                 xml[index].Descendants("entity_type_id").FirstOrDefault().Value + "|";
                    }
                }
            }
            return result;
        }

        public string SMDeleteDrafts(List<string> deleteInfo, string username)
        {

            mLogger.Debug("Start ==> SMDeleteDrafts");
            string result = "success";
            try
            {
                // List<string> draftsList = new JavaScriptSerializer().Deserialize<List<string>>(deleteInfo);

                Assembly SMControllerAssembly = Assembly.Load("SecMasterCore");

                //sm controller
                Type smSecurityController = SMControllerAssembly.GetType("com.ivp.secm.core.SMSecurityController");
                MethodInfo deleteSecurityDrafts = smSecurityController.GetMethod("DeleteSecurityDrafts");
                var smSecurityControllerObj = Activator.CreateInstance(smSecurityController);

                //sm security info
                Type smSecurityInfo = null;
                smSecurityInfo = SMControllerAssembly.GetType("com.ivp.secm.core.SMSecurityInfo");

                //SMSecurityController objSecurityController = new SMSecurityController();
                //SMSecurityInfo objSecurityInfo = null;

                foreach (string secId in deleteInfo)
                {
                    if (!string.IsNullOrEmpty(secId))
                    {
                        object smSecurityInfoObj = null;
                        smSecurityInfoObj = Activator.CreateInstance(smSecurityInfo);

                        PropertyInfo[] secinfo = smSecurityInfoObj.GetType().GetProperties();
                        foreach (PropertyInfo propinfo in secinfo)
                        {
                            if (propinfo.Name.Equals("CreatedBy"))
                                propinfo.SetValue(smSecurityInfoObj, username, null);
                            if (propinfo.Name.Equals("SecID"))
                                propinfo.SetValue(smSecurityInfoObj, secId, null);
                        }

                        //objSecurityInfo = new SMSecurityInfo();
                        //objSecurityInfo.SecID = secId;
                        //objSecurityInfo.CreatedBy = username;
                        //objSecurityController.DeleteSecurityDrafts(objSecurityInfo);
                        deleteSecurityDrafts.Invoke(smSecurityControllerObj, new object[] { smSecurityInfoObj });
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                mLogger.Debug("End ==> SMDeleteDrafts");
            }


            return result;
        }

        public string RMDeleteDrafts(List<string> deleteInfo, string username)
        {
            mLogger.Debug("Start ==> RMDeleteDrafts");
            string result = "success";

            try
            {
                //deleteInfo contains both entity type id + entity code separated by underscore
                Assembly RefMController = Assembly.Load("RefMController");

                Type rmDraftController = null;
                rmDraftController = RefMController.GetType("com.ivp.refmaster.controller.draft.RMDraftController");
                MethodInfo removeDrafts = rmDraftController.GetMethod("RemoveDrafts");
                var rmDraftControllerObj = Activator.CreateInstance(rmDraftController);

                foreach (string ent in deleteInfo)
                {
                    string[] entityData = ent.Split('_');
                    //entity code is 0, entity type is 1 in entityData
                    removeDrafts.Invoke(rmDraftControllerObj, new object[] { Convert.ToInt32(entityData[1]), entityData[0], DateTime.Now, username, null });
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                mLogger.Debug("End ==> RMDeleteDrafts");
            }

            return result;

        }

        private static List<CustomRowDataInfo> GetCustomRowDataInfoDraftsSM(DataSet ds)
        {
            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            List<CustomCellDatainfo> lstCustomCellDataInfo = new List<CustomCellDatainfo>();
            CustomRowDataInfo objCustomRowDataInfo = null;
            DataTable dt = ds.Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                RUserManagementService objUserController = new RUserManagementService();
                List<RUserInfo> radUsers = objUserController.GetAllUsersGDPR();

                Dictionary<string, string> dictUserLoginNamevsFullName = radUsers.ToDictionary(x => x.UserLoginName, y => y.FullName + " (" + y.UserName + ")");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Columns.Contains("Created By"))
                    {
                        string user = Convert.ToString(dt.Rows[i]["Created By"]);
                        if (!string.IsNullOrEmpty(user) && dictUserLoginNamevsFullName.ContainsKey(user))
                            dt.Rows[i]["Created By"] = dictUserLoginNamevsFullName[user];

                    }

                    string effectiveDate = dt.Rows[i].IsNull("Effective Date") ? string.Empty : dt.Rows[i].Field<DateTime>("Effective Date").ToString("yyyyMMdd");
                    var v = string.IsNullOrEmpty(effectiveDate) ? Convert.ToString(dt.Rows[i]["sec_id"]) : Convert.ToString(dt.Rows[i]["sec_id"]) + "¦" + effectiveDate;

                    objCustomRowDataInfo = new CustomRowDataInfo();
                    objCustomRowDataInfo.Attribute.Add("cloned_from", Convert.ToString(dt.Rows[i]["cloned_from"]));
                    objCustomRowDataInfo.Attribute.Add("delete_existing", Convert.ToString(dt.Rows[i]["delete_existing"]));
                    objCustomRowDataInfo.Attribute.Add("copy_time_series", Convert.ToString(dt.Rows[i]["copy_time_series"]));
                    CustomCellDatainfo cellSecurityId = new CustomCellDatainfo();
                    cellSecurityId.ColumnName = "Security Id";
                    bool isSecuityEditable = true;
                    //TODO is security editable checks based on user privilege
                    if (isSecuityEditable)
                    {
                        if (!Convert.ToString(GetText(dt.Rows[i]["sec_id"])).Equals("&nbsp;"))
                            cellSecurityId.NewChild = "<span id=divSecurityId" + GetText(dt.Rows[i]["sec_id"]) + " id=\"lblSecId_" + GetText(dt.Rows[i]["sec_id"]) + "\" style='text-decoration:underline;cursor:pointer;color:#48a3dd;' cloned_from='" + Convert.ToString(dt.Rows[i]["cloned_from"]) + "' delete_existing='" + Convert.ToString(dt.Rows[i]["delete_existing"]) + "' copy_time_series='" + Convert.ToString(dt.Rows[i]["copy_time_series"]) + "' effective_date='" + effectiveDate + "'>" + Convert.ToString(GetText(dt.Rows[i]["sec_id"])) + "</span>";
                    }
                    else
                    {
                        cellSecurityId.NewChild = "<span id=divSecurityId_" + GetText(dt.Rows[i]["sec_id"]) + " id=\"lblSecId_" + GetText(dt.Rows[i]["sec_id"]) + "\"  cloned_from='" + Convert.ToString(dt.Rows[i]["cloned_from"]) + "' delete_existing='" + Convert.ToString(dt.Rows[i]["delete_existing"]) + "' copy_time_series='" + Convert.ToString(dt.Rows[i]["copy_time_series"]) + "' effective_date='" + effectiveDate + "'>" + GetText(dt.Rows[i]["sec_id"]) + "</span>";
                    }
                    objCustomRowDataInfo.Cells.Add(cellSecurityId);

                    objCustomRowDataInfo.RowID = Convert.ToString(dt.Rows[i]["sec_id"]);
                    dt.Rows[i]["row_keys"] = v;

                    objCustomRowDataInfo.Attribute.Add("sec_id", v);
                    lstCustomRowDataInfo.Add(objCustomRowDataInfo);
                }
            }
            return lstCustomRowDataInfo;
        }

        private static List<CustomRowDataInfo> GetCustomRowDataInfoDraftsRM(DataSet ds)
        {
            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            List<CustomCellDatainfo> lstCustomCellDataInfo = new List<CustomCellDatainfo>();
            CustomRowDataInfo objCustomRowDataInfo = null;
            DataTable dt = ds.Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //effectiveDate=" + effectiveDate + " entity_type_id=" + GetText(dt.Rows[i]["entity_type_id"]) + 
                    objCustomRowDataInfo = new CustomRowDataInfo();
                    CustomCellDatainfo cellEntityCode = new CustomCellDatainfo();
                    cellEntityCode.ColumnName = "Entity Code";
                    if (!Convert.ToString(GetText(dt.Rows[i]["Entity Code"])).Equals("&nbsp;") && !Convert.ToString(GetText(dt.Rows[i]["entity_type_id"])).Equals("&nbsp;"))
                    {
                        var effectiveDate = Convert.ToDateTime(dt.Rows[i]["Effective Start Date"]).ToString("yyyyMMdd");

                        cellEntityCode.NewChild = "<span effectiveDate=" + effectiveDate + " entity_type_id=" + GetText(dt.Rows[i]["entity_type_id"]) + " id =\"divEntityCode" + GetText(dt.Rows[i]["Entity Code"]) + "\" style='text-decoration:underline;cursor:pointer;color:#48a3dd;' >" + Convert.ToString(GetText(dt.Rows[i]["Entity Code"])) + "</span>";
                    }


                    objCustomRowDataInfo.Cells.Add(cellEntityCode);

                    objCustomRowDataInfo.RowID = Convert.ToString(dt.Rows[i]["entity_code"]);
                    //dt.Rows[i]["row_keys"] = v;

                    objCustomRowDataInfo.Attribute.Add("entity_code", Convert.ToString(dt.Rows[i]["entity_code"]));
                    lstCustomRowDataInfo.Add(objCustomRowDataInfo);
                }
            }
            return lstCustomRowDataInfo;
        }



        #endregion

        #region CommonModuleTabs

        //Common Module Tabs
        public List<CommonModuleTypeDetails> GetCommonModuleTypeDetails(string userName, string pageIdentifier, string baseModule, string currentModule)
        {
            int count = 0;
            List<CommonModuleTypeDetails> result = new List<CommonModuleTypeDetails>();
            string availableModules = RConfigReader.GetConfigAppSettings("AVAILABLE_MODULES");
            //string productName = RConfigReader.GetConfigAppSettings("ProductName");
            List<int> lstModulesWithPrivilege = CommonDALWrapper.ExecuteSelectQuery("EXEC IVPRefMaster.dbo.SRM_GetAvailableModulesForPage '" + pageIdentifier + "','" + userName + "'", ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().Select(x => x.Field<int>("module_id")).ToList();

            if (string.IsNullOrEmpty(availableModules) || availableModules == "-1")
            {
                if (baseModule.Equals("refmaster", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentModule) && currentModule.ToLower().Equals("partymaster"))
                    {
                        foreach (var val in Enum.GetValues(typeof(PMModules)).Cast<PMModules>())
                        {
                            if (lstModulesWithPrivilege.Contains(Convert.ToInt32(Enum.Parse(typeof(SRMModules), val.ToString()))))
                            {
                                count++;
                                if (count == 1)
                                    result.Add(new CommonModuleTypeDetails(-1, "AllSystems"));
                                result.Add(new CommonModuleTypeDetails(Convert.ToInt32(Enum.Parse(typeof(SRMModules), val.ToString())), val.ToString()));
                            }
                        }
                    }
                    else
                    {
                        foreach (var val in Enum.GetValues(typeof(RMModules)).Cast<RMModules>())
                        {
                            if (lstModulesWithPrivilege.Contains(Convert.ToInt32(Enum.Parse(typeof(SRMModules), val.ToString()))))
                            {
                                count++;
                                if (count == 1)
                                    result.Add(new CommonModuleTypeDetails(-1, "AllSystems"));
                                result.Add(new CommonModuleTypeDetails(Convert.ToInt32(Enum.Parse(typeof(SRMModules), val.ToString())), val.ToString()));
                                //result.Add(new CommonModuleTypeDetails(Convert.ToInt32(val), val.ToString()));
                            }
                        }
                    }
                }
                else if (baseModule.Equals("secmaster", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var val in Enum.GetValues(typeof(SRMModules)).Cast<SRMModules>())
                    {
                        if (lstModulesWithPrivilege.Contains(Convert.ToInt32(val)))
                        {
                            count++;
                            if (count == 1)
                                result.Add(new CommonModuleTypeDetails(-1, "AllSystems"));
                            result.Add(new CommonModuleTypeDetails(Convert.ToInt32(val), val.ToString()));
                        }

                    }
                }
            }
            else
            {
                List<string> lstAvailableModules = availableModules.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (baseModule.Equals("refmaster", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(currentModule) && currentModule.ToLower().Equals("partymaster"))
                    {
                        foreach (var val in Enum.GetValues(typeof(PMModules)).Cast<PMModules>())
                        {
                            if (lstModulesWithPrivilege.Contains(Convert.ToInt32(Enum.Parse(typeof(SRMModules), val.ToString()))))
                            {
                                count++;
                                if (count == 1)
                                    result.Add(new CommonModuleTypeDetails(-1, "AllSystems"));
                                result.Add(new CommonModuleTypeDetails(Convert.ToInt32(Enum.Parse(typeof(SRMModules), val.ToString())), val.ToString()));
                                //result.Add(new CommonModuleTypeDetails(Convert.ToInt32(Enum.Parse(typeof(SRMModules), val)), Enum.Parse(typeof(PMModules), val).ToString()));
                            }

                        }
                    }
                    else
                    {
                        foreach (var val in Enum.GetValues(typeof(RMModules)).Cast<RMModules>())
                        {
                            if (lstModulesWithPrivilege.Contains(Convert.ToInt32(Enum.Parse(typeof(SRMModules), val.ToString()))))
                            {
                                count++;
                                if (count == 1)
                                    result.Add(new CommonModuleTypeDetails(-1, "AllSystems"));
                                result.Add(new CommonModuleTypeDetails(Convert.ToInt32(Enum.Parse(typeof(SRMModules), val.ToString())), val.ToString()));
                            }

                        }
                    }
                }
                else if (baseModule.Equals("secmaster", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string val in lstAvailableModules)
                    {
                        if (lstModulesWithPrivilege.Contains(Convert.ToInt32(val)))
                        {
                            count++;
                            if (count == 1)
                                result.Add(new CommonModuleTypeDetails(-1, "AllSystems"));
                            result.Add(new CommonModuleTypeDetails(Convert.ToInt32(Enum.Parse(typeof(SRMModules), val)), Enum.Parse(typeof(SRMModules), val).ToString()));
                        }
                    }
                }
            }
            if (result.Count == 1 || (pageIdentifier.Equals("ExceptionManager") && result.Count == 1))
            {
                result.RemoveAt(0);
            }
            return result;
        }
        #endregion

        #region downstreamstatus
        public SMDownstreamStatusInfo GetDownstreamStatusInfo(string productName, string userName, bool isTempFilterApplied, int moduleId)
        {
            SMDownstreamStatusInfo objDownstreamStatusInfo = new SMDownstreamStatusInfo();
            //   objDownstreamStatusInfo.DownstreamStatusConfig = GetDownstreamStatusConfig(userName);

            //if (!isTempFilterApplied)
            //    selectedExternalSystems = objSMDashboardBasicInfo.DashboardConfig[0].SelectedExternalSystems;
            //  objDownstreamStatusInfo.DownstreamStatusConfig = GetDownstreamStatusConfig(userName);
            objDownstreamStatusInfo.ExternalSystems = GetExternalSystems(moduleId);
            objDownstreamStatusInfo.TaskStatus = BindTaskStatus();
            objDownstreamStatusInfo.Status = BindStatus();

            return objDownstreamStatusInfo;
        }
        public List<ExternalSystemDetails> GetExternalSystems(int moduleId)
        {
            List<ExternalSystemDetails> lstObj = new List<ExternalSystemDetails>();
            DataTable dtSec = null;
            DataTable dtRefDownstreamSystems = null;
            if (moduleId == 3 || moduleId == 9)
            {
                Assembly SMUpstreamAssembly = Assembly.Load("SecMasterUpstream");
                Type SMBulkEditController = null;
                SMBulkEditController = SMUpstreamAssembly.GetType("com.ivp.secm.upstream.SMBulkEditController");
                object SMBulkEditControllerObj = null;
                SMBulkEditControllerObj = Activator.CreateInstance(SMBulkEditController);
                MethodInfo getExternalSystems = SMBulkEditController.GetMethod("GetExternalSystems");
                //DataTable dt = new SMBulkEditController().GetExternalSystems();
                dtSec = (DataTable)getExternalSystems.Invoke(SMBulkEditControllerObj, new object[] { });
                if (dtSec != null && dtSec.Rows.Count > 0)
                {
                    lstObj.Add(new ExternalSystemDetails
                    {
                        ExternalSystemId = 0,
                        ExternalSystemName = "All systems"
                    });
                    foreach (DataRow dr in dtSec.AsEnumerable().OrderBy(x => x.Field<string>("system_name")))
                    {
                        lstObj.Add(new ExternalSystemDetails
                        {
                            ExternalSystemId = dr.Field<int>("system_id"),
                            ExternalSystemName = Convert.ToString(dr["system_name"])
                        });
                    }
                }
            }

            else
            {
                Assembly RefMAPI = Assembly.Load("RefMAPI");
                Type RMRefMasterAPI = null;
                RMRefMasterAPI = RefMAPI.GetType("com.ivp.refmaster.refmasterwebservices.RMRefMasterAPI");
                object RMRefMasterAPIObj = null;
                RMRefMasterAPIObj = Activator.CreateInstance(RMRefMasterAPI);
                MethodInfo getEntityDownstreamSystems = RMRefMasterAPI.GetMethod("GetEntityDownstreamSystems");
                dtRefDownstreamSystems = (DataTable)getEntityDownstreamSystems.Invoke(RMRefMasterAPIObj, new object[] { });

                List<string> l = new List<string>();

                for (int i = 0; i < dtRefDownstreamSystems.Rows.Count; i++)
                {
                    if (!(l.Contains(dtRefDownstreamSystems.Rows[i]["report_system_name"].ToString(), StringComparer.CurrentCultureIgnoreCase)))
                        l.Add(dtRefDownstreamSystems.Rows[i]["report_system_name"].ToString());
                }
                if (dtRefDownstreamSystems != null && dtRefDownstreamSystems.Rows.Count > 0)
                {
                    lstObj.Add(new ExternalSystemDetails
                    {
                        ExternalSystemId = 0,
                        ExternalSystemName = "All systems"
                    });
                    foreach (DataRow dr in dtRefDownstreamSystems.AsEnumerable().OrderBy(x => x.Field<string>("report_system_name")))
                    {
                        lstObj.Add(new ExternalSystemDetails
                        {
                            ExternalSystemId = dr.Field<int>("report_system_id"),
                            ExternalSystemName = Convert.ToString(dr["report_system_name"])
                        });
                    }

                }

            }

            return lstObj;
        }
        private List<TaskStatusDetails> BindTaskStatus()
        {
            try
            {
                List<TaskStatusDetails> l = new List<TaskStatusDetails>();
                l.Add(new TaskStatusDetails
                {
                    TaskStatusId = -1,
                    TaskStatusName = SMCommonStatusTypes.Any.ToString()
                });
                l.Add(new TaskStatusDetails
                {
                    TaskStatusId = 1,
                    TaskStatusName = SMCommonStatusTypes.Success.ToString()
                });
                l.Add(new TaskStatusDetails
                {
                    TaskStatusId = 2,
                    TaskStatusName = SMCommonStatusTypes.Failure.ToString()
                });
                l.Add(new TaskStatusDetails
                {
                    TaskStatusId = 3,
                    TaskStatusName = SMCommonStatusTypes.In_Progress.ToString().Replace('_', ' ')
                });
                l.Add(new TaskStatusDetails
                {
                    TaskStatusId = 4,
                    TaskStatusName = SMCommonStatusTypes.Not_Processed.ToString().Replace('_', ' ')
                });
                l.Add(new TaskStatusDetails
                {
                    TaskStatusId = (int)SMCommonStatusTypes.Queued,
                    TaskStatusName = SMCommonStatusTypes.Queued.ToString().Replace('_', ' ')
                });


                return l;
            }
            catch (Exception ex) { throw ex; }
            finally
            {

            }
        }

        private List<StatusDetails> BindStatus()
        {
            try
            {
                List<StatusDetails> l = new List<StatusDetails>();
                l.Add(new StatusDetails
                {
                    StatusId = 1,
                    StatusName = SMStatus.Most_Recent.ToString().Replace('_', ' ')
                });
                l.Add(new StatusDetails
                {
                    StatusId = 2,
                    StatusName = SMStatus.All.ToString()
                });

                return l;
            }
            catch (Exception ex) { throw ex; }
            finally
            {

            }
        }
        #endregion

        #region datasourceandsystemmapping

        public DataSourceAndSystemMappingInfo GetDataSourceAndSystemMappingInfo(int type_id, int module_id)
        {
            DataSourceAndSystemMappingInfo obj = new DataSourceAndSystemMappingInfo();

            if (module_id == 3)
            {
                //DataSourceAndSystemMappingInfo obj = new DataSourceAndSystemMappingInfo();
                //DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"select sectype_name from  IVPSecMaster.dbo.ivp_secm_sectype_master where sectype_id=" + type_id, ConnectionConstants.SecMaster_Connection);
                //obj.entityTypeDisplayName = Convert.ToString(ds.Tables[0].Rows[0]["sectype_name"]);

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"select distinct smaster.sectype_name,vm.vendor_id,vm.vendor_name
                                                                from IVPSecMaster.dbo.ivp_secm_sectype_master smaster
                                                                LEFT JOIN IVPSecMasterVendor.dbo.ivp_secmv_sectype_feed_mapping_master fmap 

                                                                ON (smaster.sectype_id=fmap.sectype_id AND smaster.is_active=1)
                                                                LEFT JOIN IVPSecMasterVendor.dbo.ivp_secmv_feed_summary fsum
                                                                ON (fsum.feed_id=fmap.feed_id AND fsum.is_active=1)
                                                       
                                                                LEFT JOIN IVPSecMasterVendor.dbo.ivp_secmv_vendor_master vm
                                                                ON(vm.vendor_id=fsum.vendor_id AND vm.is_active=1)
                                                                WHERE smaster.sectype_id =" + type_id, ConnectionConstants.SecMaster_Connection);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    obj.entityTypeDisplayName = Convert.ToString(ds.Tables[0].Rows[0]["sectype_name"]);


                    if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["vendor_name"])))
                    {
                        obj.dataSourceInfoWidgetList.AddRange(ds.Tables[0].AsEnumerable().Select(x => new DataSourceInfoWidget(x.Field<string>("vendor_name"), x.Field<int>("vendor_id"))));

                    }
                }

                ds = CommonDALWrapper.ExecuteSelectQuery(@"select DISTINCT ds.system_id,rs.report_name
                                                        from IVPSecMaster.dbo.ivp_secm_downstream_system ds 
                                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_system_mapping smap ON(ds.system_id=smap.system_id AND ds.is_active=1)
                                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON(rs.report_setup_id=smap.report_setup_id AND rs.is_active=1)
                                                        CROSS APPLY IVPSecMaster.dbo.SECM_GetList2Table(rs.dependent_id,'|') tab 
                                                        WHERE tab.item = '" + type_id + @"'", ConnectionConstants.SecMaster_Connection);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    obj.systemInfoWidgetList.AddRange(ds.Tables[0].AsEnumerable().Select(x => new SystemInfoWidget(0, x.Field<string>("report_name"), x.Field<int>("system_id"))));
                }
            }
            else if (module_id == 6 || module_id == 18 || module_id == 20)
            {

                // DataSourceAndSystemMappingInfo obj = new DataSourceAndSystemMappingInfo();

                //DataSet ds = CommonDALWrapper.ExecuteSelectQuery("select entity_display_name from IVPRefMaster.dbo.ivp_refm_entity_type where entity_type_id = " + type_id + " ", ConnectionConstants.RefMaster_Connection);
                //obj.entityTypeDisplayName = Convert.ToString(ds.Tables[0].Rows[0]["entity_display_name"]);

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT distinct rentity.entity_display_name,ds.data_source_id,ds.data_source_name as data_source_name
                                                                FROM IVPRefMaster.dbo.ivp_refm_entity_type rentity
                                                                left join IVPRefMaster.dbo.ivp_refm_entity_type_feed_mapping fmap
                                                                ON(rentity.entity_type_id=fmap.entity_type_id AND fmap.is_active = 1)
                                                                left JOIN IVPRefMasterVendor.dbo.ivp_refm_feed_summary fsum
                                                                ON(fsum.feed_summary_id = fmap.feed_summary_id AND fsum.is_active = 1)
                                                                left JOIN IVPRefMasterVendor.dbo.ivp_refm_data_source ds
                                                                ON(ds.data_source_id = fsum.data_source_id AND ds.is_active = 1)
                                                                WHERE rentity.entity_type_id = " + type_id, ConnectionConstants.RefMaster_Connection);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    obj.entityTypeDisplayName = Convert.ToString(ds.Tables[0].Rows[0]["entity_display_name"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["data_source_name"])))
                    {
                        obj.dataSourceInfoWidgetList.AddRange(ds.Tables[0].AsEnumerable().Select(x => new DataSourceInfoWidget(x.Field<string>("data_source_name"), x.Field<int>("data_source_id"))));

                    }
                }
                ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT rep.report_type_id,rsconf.report_system_name,rsconf.report_system_id
                                                        FROM IVPRefMaster.dbo.ivp_refm_report_system_report_map rmap
                                                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report_system_configuration rsconf
                                                        ON (rsconf.report_system_id = rmap.report_system_id AND rsconf.is_active = 1)
                                                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                                                        ON (rmap.report_id = rep.report_id AND rmap.is_active = 1 AND rep.is_active = 1)
                                                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map remp
                                                        ON (remp.report_id = rep.report_id AND remp.is_active = 1 AND remp.dependent_id = " + type_id + @" AND rep.report_type_id IN (1,2,5,6))", ConnectionConstants.RefMaster_Connection);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    obj.systemInfoWidgetList.AddRange(ds.Tables[0].AsEnumerable().Select(x => new SystemInfoWidget(x.Field<int>("report_type_id"), x.Field<string>("report_system_name"), x.Field<int>("report_system_id"))));
                }
            }
            else
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"select vm.vendor_id,vm.vendor_name,cmaster.corpaction_type_name
                                                                from IVPCorpActionVendor.dbo.ivp_cav_corpaction_type_feed_mapping_master fmap 
                                                                INNER JOIN IVPCorpActionVendor.dbo.ivp_cav_feed_summary fsum
                                                                ON (fsum.feed_id=fmap.feed_id AND fsum.is_active=1)
                                                                INNER JOIN IVPCorpAction.dbo.ivp_ca_corpaction_type_master cmaster
                                                                ON (cmaster.corpaction_type_id=fmap.corpaction_type_id AND cmaster.is_active=1)
                                                                INNER JOIN IVPCorpActionVendor.dbo.ivp_cav_vendor_master vm
                                                                ON(vm.vendor_id=fsum.vendor_id AND vm.is_active=1)
                                                                WHERE fmap.corpaction_type_id = " + type_id, ConnectionConstants.CorpAction_Connection);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    obj.entityTypeDisplayName = Convert.ToString(ds.Tables[0].Rows[0]["corpaction_type_name"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["vendor_name"])))
                    {
                        obj.dataSourceInfoWidgetList.AddRange(ds.Tables[0].AsEnumerable().Select(x => new DataSourceInfoWidget(x.Field<string>("vendor_name"), x.Field<int>("vendor_id"))));
                    }
                }
            }
            return obj;

        }


        public string GetDataSourceTable_Slider(int type_id, int data_source_id, int module_id, int template_id)
        {
            DataSet feed_data_ds = null;
            string attribute_name = null;

            if (module_id == 3)
            {
                feed_data_ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT DISTINCT fmap.feed_id,fsum.feed_name,fdetails.field_name,temp.display_name,fmapdetails.feed_field_id
                                        FROM IVPSecMasterVendor.dbo.ivp_secmv_sectype_feed_mapping_master fmap 
                                        INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_feed_summary fsum
                                        ON (fsum.feed_id=fmap.feed_id)
                                        INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_sectype_feed_mapping_details fmapdetails 
                                        ON (fmapdetails.sectype_feed_mapping_id=fmap.sectype_feed_mapping_id)
                                        INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_feed_details fd
                                        ON(fd.feed_field_id=fmapdetails.feed_field_id AND fd.is_active=1)
                                        INNER JOIN IVPSecMaster.dbo.ivp_rad_file_field_details fdetails 
                                        ON (fdetails.field_id=fd.field_id)
                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details temp 
                                        ON (temp.attribute_id=fmapdetails.sectype_attribute_id AND temp.template_id= " + template_id + @" )
                                        INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_vendor_master vm
                                        ON(vm.vendor_id=fsum.vendor_id)
                                        WHERE fmap.sectype_id =  " + type_id + @" AND vm.vendor_id = " + data_source_id + @" ORDER BY fsum.feed_name,temp.display_name", ConnectionConstants.SecMaster_Connection);

            }
            else if (module_id == 9)
            {
                feed_data_ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT fmap.feed_id,fsum.feed_name,fdetails.field_name,temp.display_name,fmapdetails.feed_field_id
                                                                    FROM IVPCorpActionVendor.dbo.ivp_cav_corpaction_type_feed_mapping_master fmap
                                                                    INNER JOIN IVPCorpActionVendor.dbo.ivp_cav_feed_summary fsum
                                                                    ON (fsum.feed_id=fmap.feed_id AND fsum.is_active=1)
                                                                    INNER JOIN IVPCorpActionVendor.dbo.ivp_cav_corpaction_type_feed_mapping_details fmapdetails
                                                                    ON (fmapdetails.corpaction_type_feed_mapping_id = fmap.corpaction_type_feed_mapping_id AND fmapdetails.is_active=1)
                                                                    INNER JOIN IVPCorpActionVendor.dbo.ivp_cav_feed_details fd 
                                                                    ON (fd.feed_field_id=fmapdetails.feed_field_id AND fd.is_active=1)
                                                                    INNER JOIN IVPCorpAction.dbo.ivp_rad_file_field_details fdetails 
                                                                    ON (fdetails.field_id=fd.field_id AND fdetails.is_active=1)
                                                                    INNER JOIN IVPCorpAction.dbo.ivp_ca_template_details temp 
                                                                    ON (temp.attribute_id=fmapdetails.corpaction_type_attribute_id AND temp.is_active=1)
                                                                    INNER JOIN IVPCorpActionVendor.dbo.ivp_cav_vendor_master vm
                                                                    ON(vm.vendor_id=fsum.vendor_id AND vm.is_active=1)
                                                                    WHERE fmap.corpaction_type_id =  " + type_id + @" AND vm.vendor_id = " + data_source_id + @" ORDER BY fsum.feed_name,temp.display_name", ConnectionConstants.CorpAction_Connection);

            }
            else
            {
                feed_data_ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT fsum.feed_summary_id,fsum.feed_name,fielddetail.field_name,attr.display_name,fsum.data_source_id
                                                                    FROM 
                                                                    IVPRefMaster.dbo.ivp_refm_entity_type_feed_mapping fmap 
                                                                    INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_feed_summary fsum
                                                                    ON (fsum.feed_summary_id = fmap.feed_summary_id AND fsum.is_active = 1)
                                                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type_feed_mapping_details fmapdetail
                                                                    ON(fmapdetail.entity_type_feed_mapping_id=fmap.entity_type_feed_mapping_id AND fmapdetail.is_active=1)
                                                                    INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_feed_field_details fd
                                                                    ON(fd.rad_field_id=fmapdetail.field_id AND fd.is_active=1)
                                                                    INNER JOIN IVPRefMasterVendor.dbo.ivp_rad_file_field_details fielddetail
                                                                    ON(fielddetail.field_id=fd.rad_field_id AND fielddetail.is_active=1)
                                                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute attr
                                                                    ON(attr.entity_attribute_id=fmapdetail.entity_attribute_id AND attr.is_active=1)
                                                                    INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_data_source ds
                                                                    ON (ds.data_source_id = fsum.data_source_id AND ds.is_active = 1)
                                                                    WHERE fmap.entity_type_id= " + type_id + @" AND ds.data_source_id = " + data_source_id + @" ORDER BY fsum.feed_name,attr.attribute_name", ConnectionConstants.RefMaster_Connection);

            }

            DataTable result = new DataTable();

            if (feed_data_ds != null && feed_data_ds.Tables.Count > 0 && feed_data_ds.Tables[0] != null && feed_data_ds.Tables[0].Rows.Count > 0)
            {
                result.Columns.Add("Feed_Name", typeof(string));
                result.Columns.Add("Field_Name", typeof(string));
                result.Columns.Add("Attribute_Name", typeof(string));

                foreach (DataRow dr in feed_data_ds.Tables[0].AsEnumerable())
                {
                    if (module_id == 3 || module_id == 9)
                    {
                        attribute_name = Convert.ToString(dr["display_name"]);
                    }
                    else
                    {
                        attribute_name = Convert.ToString(dr["display_name"]);
                    }

                    result.Rows.Add(Convert.ToString(dr["feed_name"]), string.IsNullOrEmpty(Convert.ToString(dr["field_name"])) ? string.Empty : Convert.ToString(dr["field_name"]), attribute_name);
                }
            }

            return JsonConvert.SerializeObject(result);
        }
        /* public string GetDataSourceTable_Slider1(int entity_type_id, int data_source_id)
         {

             Dictionary<int, string> dictFieldInfo = new Dictionary<int, string>();
             DataSet feedIdList_ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT fsum.feed_summary_id,fsum.feed_name FROM IVPRefMaster.dbo.ivp_refm_entity_type_feed_mapping fmap
                                                                         INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_feed_summary fsum
                                                                         ON (fsum.feed_summary_id = fmap.feed_summary_id AND fsum.is_active = 1)
                                                                         INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_data_source ds
                                                                         ON (ds.data_source_id = fsum.data_source_id AND ds.is_active = 1)
                                                                         WHERE fmap.entity_type_id = " + entity_type_id + " AND fmap.is_active = 1 AND ds.data_source_id = " + data_source_id
                                                                         , ConnectionConstants.RefMaster_Connection);
             DataTable result = new DataTable();
             if (feedIdList_ds != null && feedIdList_ds.Tables.Count > 0 && feedIdList_ds.Tables[0] != null && feedIdList_ds.Tables[0].Rows.Count > 0)
             {

                 result.Columns.Add("Feed_Name", typeof(string));
                 result.Columns.Add("Field_Name", typeof(string));
                 result.Columns.Add("Attribute_Name", typeof(string));

                 foreach (DataRow dr in feedIdList_ds.Tables[0].AsEnumerable())
                 {
                     int feed_summary_id = Convert.ToInt32(dr[0]);
                     string feed_name = Convert.ToString(dr[1]);

                     Dictionary<string, object> data = new RMEntityTypeFeedMappingDetailsController().GetAllEntityFeedMappingDetailsData(entity_type_id, feed_summary_id);
                     if (data.ContainsKey("FeedFieldData"))
                     {
                         foreach (RMFeedFieldDetailsForMappingInfo info in (List<RMFeedFieldDetailsForMappingInfo>)data["FeedFieldData"])
                         {
                             if (dictFieldInfo.ContainsKey(info.FieldId))
                                 dictFieldInfo[info.FieldId] = info.FieldName;
                             else
                                 dictFieldInfo.Add(info.FieldId, info.FieldName);
                         }
                     }
                     if (data.ContainsKey("EntityFeedMappingData"))
                     {
                         foreach (RMEntityAttributesFeedFieldsMappingInfo info in (List<RMEntityAttributesFeedFieldsMappingInfo>)data["EntityFeedMappingData"])
                         {
                             result.Rows.Add(feed_name, dictFieldInfo.ContainsKey(info.FieldId) ? dictFieldInfo[info.FieldId] : string.Empty, info.EntityAttributeDisplayName);
                         }
                     }
                 }
             }
             return result.AsJson();
         }
         */
        public string GetReportData(int type_id, int report_system_id, int module_id, int report_type_id, int template_id)
        {

            if (module_id == 3)
            {
                DataSet report_ds = CommonDALWrapper.ExecuteSelectQuery(@" SELECT DISTINCT rs.report_name,attr.report_attribute_name, td.display_name
                                                                        from IVPSecMaster.dbo.ivp_secm_report_setup rs 
                                                                        CROSS APPLY IVPSecMaster.dbo.SECM_GetList2Table(rs.dependent_id,'|') tab
                                                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_system_mapping rsm ON rs.report_setup_id = rsm.report_setup_id
                                                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attributes attr
                                                                        ON(attr.report_setup_id=rs.report_setup_id)
                                                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attribute_mapping amap
                                                                        ON(amap.report_attribute_id=attr.report_attribute_id)
                                                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details td
                                                                        ON(td.attribute_id=amap.attribute_id AND td.template_id= " + template_id + @" ) 
                                                                        WHERE rsm.system_id = " + report_system_id + " AND rs.is_active = 1 AND tab.item = '" + type_id + @"' ORDER BY td.display_name", ConnectionConstants.SecMaster_Connection);

                DataTable result = new DataTable();
                if (report_ds != null && report_ds.Tables.Count > 0 && report_ds.Tables[0] != null && report_ds.Tables[0].Rows.Count > 0)
                {

                    result.Columns.Add("Report Name", typeof(string));
                    result.Columns.Add("Report Attribute Name", typeof(string));
                    result.Columns.Add("Attribute Name", typeof(string));
                    foreach (DataRow dr in report_ds.Tables[0].AsEnumerable())
                    {
                        result.Rows.Add(dr["report_name"], dr["report_attribute_name"], dr["display_name"]);

                    }

                }
                return JsonConvert.SerializeObject(result);
            }
            else
            {

                if (report_type_id == 2 || report_type_id == 5)
                {
                    DataSet report_ds = CommonDALWrapper.ExecuteSelectQuery(@" SELECT rep.report_name,eat.display_name FROM
                                                                        IVPRefMaster.dbo.ivp_refm_report_system_report_map rmap
                                                                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report_system_configuration rsyconf
                                                                        ON(rmap.report_system_id=rsyconf.report_system_id AND rsyconf.is_active = 1)
                                                                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report_configuration rconf
                                                                        ON(rconf.report_id=rmap.report_id AND rconf.is_active=1)
                                                                        CROSS APPLY IVPRefMaster.dbo.REFM_GetList2Table(rconf.attribute_to_show,'|') tab
                                                                        INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat ON (tab.item = eat.entity_attribute_id)
                                                                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                                                                        ON (rep.report_id = rconf.report_id AND rep.is_active=1)
                                                                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map remap
                                                                        ON(remap.report_id=rep.report_id AND remap.is_active=1) WHERE rep.report_type_id IN (1,6) AND rsyconf.report_system_id = " + report_system_id +
                                                                        @" AND remap.dependent_id= " + type_id +
                                                                        @" AND rconf.is_active = 1 AND eat.is_active = 1 AND rep.is_active = 1 ORDER BY eat.display_name", ConnectionConstants.RefMaster_Connection);

                    DataTable result1 = new DataTable();
                    if (report_ds != null && report_ds.Tables.Count > 0 && report_ds.Tables[0] != null && report_ds.Tables[0].Rows.Count > 0)
                    {

                        result1.Columns.Add("Report Name", typeof(string));
                        result1.Columns.Add("Attribute Name", typeof(string));
                        foreach (DataRow dr in report_ds.Tables[0].AsEnumerable())
                        {
                            result1.Rows.Add(dr["report_name"], dr["displayName"]);

                        }


                    }
                    return JsonConvert.SerializeObject(result1);
                }

                else
                {
                    DataSet report_ds = CommonDALWrapper.ExecuteSelectQuery(@" SELECT 
                                                                            rep.report_name,rattr.report_attribute_name,eat.display_name 
                                                                            FROM IVPRefMaster.dbo.ivp_refm_report_system_configuration rsconf
                                                                            INNER JOIN IVPRefMaster.dbo.ivp_refm_report_system_report_map rmap
                                                                            ON (rmap.report_system_id = rsconf.report_system_id AND rsconf.is_active = 1 AND rmap.is_active = 1)
                                                                            INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                                                                            ON (rep.report_id = rmap.report_id AND rep.is_active = 1)
                                                                            INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute rattr
                                                                            ON (rattr.report_id = rep.report_id AND rattr.is_active = 1)
                                                                            INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute_map ramap
                                                                            ON (ramap.report_attribute_id = rattr.report_attribute_id AND ramap.is_active = 1)
                                                                            INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat
                                                                            ON (eat.entity_attribute_id = ramap.dependent_attribute_id AND eat.is_active = 1)
                                                                            WHERE rep.report_type_id IN (1,6) AND rsconf.report_system_id = " + report_system_id + " AND eat.entity_type_id = " + type_id + @" ORDER BY eat.display_name", ConnectionConstants.RefMaster_Connection);

                    DataTable result2 = new DataTable();
                    //  int result1_=0, result2_=0;

                    if (report_ds != null && report_ds.Tables.Count > 0 && report_ds.Tables[0] != null && report_ds.Tables[0].Rows.Count > 0)
                    {
                        result2.Columns.Add("Report Name", typeof(string));
                        result2.Columns.Add("Report Attribute Name", typeof(string));
                        result2.Columns.Add("Attribute Name", typeof(string));

                        foreach (DataRow dr in report_ds.Tables[0].AsEnumerable())//first feed id in dr
                        {
                            result2.Rows.Add(dr["report_name"], dr["report_attribute_name"], dr["display_name"]);

                        }
                    }
                    return JsonConvert.SerializeObject(result2);
                }
            }
        }


        #endregion

        #region Rules Catalog

        public Dictionary<int, string> GetRuleCatalogFilterDataForModuleID(int ModuleID, string UserName)
        {
            mLogger.Debug("CommonService -> GetRuleCatalogFilterDataForModuleID -> Start");
            Dictionary<int, string> result = new Dictionary<int, string>();

            try
            {
                if (ModuleID == 3)
                {
                    result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT sectype_id, sectype_name FROM IVPSecMaster.dbo.SECM_GetUserSectypes('{0}') ORDER BY sectype_name", UserName), ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().ToDictionary(key => key.Field<int>("sectype_id"), value => value.Field<string>("sectype_name"));
                }
                else if (ModuleID == 9)
                {
                    result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT corpaction_type_id, corpaction_type_name FROM IVPCorpAction.dbo.ivp_ca_corpaction_type_master WHERE is_active=1 ORDER BY corpaction_type_name"), ConnectionConstants.CorpAction_Connection).Tables[0].AsEnumerable().ToDictionary(key => key.Field<int>("corpaction_type_id"), value => value.Field<string>("corpaction_type_name"));
                }
                else if (ModuleID == 6 || ModuleID == 18 || ModuleID == 20)
                {
                    Assembly RefMCommon = Assembly.Load("RefMCommon");
                    Type rmCommonUtils = null;
                    rmCommonUtils = RefMCommon.GetType("com.ivp.refmaster.common.RMCommonUtils");

                    MethodInfo getAllEntityTypesForUser = rmCommonUtils.GetMethod("GetAllEntityTypesForUser");
                    //var rmCommonUtilsObj = Activator.CreateInstance(rmCommonUtils);
                    DataTable dsEntityTypes = (DataTable)getAllEntityTypesForUser.Invoke(null, new object[] { UserName, ModuleID });

                    result = dsEntityTypes.AsEnumerable().ToDictionary(key => key.Field<int>("entity_type_id"), value => value.Field<string>("entity_display_name"));
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetRuleCatalogFilterDataForModuleID -> Exception ->" + ex.ToString());
            }
            finally
            {
                mLogger.Debug("CommonService -> GetRuleCatalogFilterDataForModuleID -> End");
            }

            return result;
        }

        public string GetRulesForModuleIDAndTabID(int ModuleID, int TabID, string UserName, List<int> TypeIDs)
        {
            mLogger.Debug("CommonService -> GetRulesForModuleIDAndTabID -> Start");
            string result = "";

            try
            {
                RuleCatalogController GetRulesCatalog = new RuleCatalogController();
                result = GetRulesCatalog.GetRuleCatalogueLeftPane((SRMModule)ModuleID, (SRMTabType)TabID, UserName, TypeIDs);
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetRulesForModuleIDAndTabID -> Exception ->" + ex.ToString());
            }
            finally
            {
                mLogger.Debug("CommonService -> GetRulesForModuleIDAndTabID -> End");
            }

            return result;
        }

        //BELOW Function NOT Used Now
        public SRMRuleCatalogFilterInfo GetRuleCatalogFilterInfo(string userName, string moduleName)
        {
            SRMRuleCatalogFilterInfo objSRMRuleCatalogFilterInfo = new SRMRuleCatalogFilterInfo();

            if (moduleName.Equals("SECURITY_MASTER", StringComparison.OrdinalIgnoreCase))
            {
                objSRMRuleCatalogFilterInfo.Security = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT sectype_id, sectype_name FROM IVPSecMaster.dbo.SECM_GetUserSectypes('{0}') ORDER BY sectype_name", userName), ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => new SecurityTypes { SecurityTypeId = Convert.ToInt32(x["sectype_id"]), SecurityTypeName = Convert.ToString(x["sectype_name"]) }).ToList(); ;
            }
            else
            {
                objSRMRuleCatalogFilterInfo.Entity = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT entity_type_id, entity_display_name FROM IVPRefMaster.dbo.REFM_GetUserEntityTypes('{0}', 0) ORDER BY entity_display_name", userName), ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().Select(x => new EntityTypes { EntityTypeId = Convert.ToInt32(x["entity_type_id"]), EntityTypeName = Convert.ToString(x["entity_display_name"]) }).ToList(); ;

            }

            objSRMRuleCatalogFilterInfo.Rules = new List<RuleTypes>();
            foreach (SRMRuleType type in Enum.GetValues(typeof(SRMRuleType)))
            {
                objSRMRuleCatalogFilterInfo.Rules.Add(new RuleTypes { RuleId = Convert.ToInt32(type), RuleName = type.ToString() });
            }
            objSRMRuleCatalogFilterInfo.Rules = objSRMRuleCatalogFilterInfo.Rules.OrderBy(x => x.RuleName).ToList();
            return objSRMRuleCatalogFilterInfo;
        }

        public string GetRules(int module, int tab, string ruleType, int typeId, string userName)
        {
            return new RuleCatalogController().GetRules((SRMModule)module, (SRMTabType)tab, ruleType, new List<int> { typeId }, userName);
        }

        private static void SaveFailureDataInCacheRuleCatalog(DataTable failureDataTable, SRMRuleCatalogOutput objSRMRuleCatalogOutput, string userName)
        {
            GridInfo gridInfo = new GridInfo
            {
                SelectRecordsAcrossAllPages = null,
                CacheGriddata = true,
                UserId = userName,
                Height = "350px",
                ColumnsWithoutClientSideFunctionality = new List<string>(),
                ColumnsNotToSum = new List<string>(),
                RequireEditGrid = false,
                RequireEditableRow = false,
                IdColumnName = "row_id",
                KeyColumns = new Dictionary<KeyType, string> { { KeyType.RowKey, "row_keys" } },
                TableName = "DataTable",
                PageSize = 50,
                RequirePaging = false,
                RequireInfiniteScroll = true,
                CollapseAllGroupHeader = false,
                //GridTheme = Theme.MasterChildGridTheme,
                DoNotExpand = false,
                ItemText = "Number of Rows",
                DoNotRearrangeColumn = true,
                RequireGrouping = true,
                RequireFilter = true,
                RequireSort = true,
                RequireMathematicalOperations = false,
                RequireSelectedRows = false,
                RequireExportToExcel = true,
                RequireSearch = true,
                RequireFreezeColumns = false,
                RequireHideColumns = true,
                RequireColumnSwap = true,
                RequireGroupExpandCollapse = true,
                RequireResizing = true,
                RequireLayouts = false,
                RequireGroupHeaderCheckbox = false,
                RequireRuleBasedColoring = false,
                RequireExportToPdf = false,
                ShowRecordCountOnHeader = false,
                ShowAggregationOnHeader = true,
                CheckBoxInfo = null,
                RaiseGridCallBackBeforeExecute = "",
                RaiseGridRenderComplete = "",
                DefaultGroupedAndSortedColumns = new List<SortInfo>(),
                ColumnsToHide = new List<HiddenColumnInfo>() { new HiddenColumnInfo { ColumnName = "row_id", isDefault = true }, new HiddenColumnInfo { ColumnName = "row_keys", isDefault = true } },
                CustomFormatInfoClientSide = new Dictionary<string, FormatterInfo>()
            };

            string gridGuid = "s" + Guid.NewGuid().ToString().Replace('-', '_');

            objSRMRuleCatalogOutput.HasGrid = true;
            objSRMRuleCatalogOutput.gridID = gridGuid;
            objSRMRuleCatalogOutput.currentPageId = gridGuid + "_123";
            objSRMRuleCatalogOutput.viewKey = gridGuid + "_789";
            objSRMRuleCatalogOutput.sessionIdentifier = gridGuid;
            objSRMRuleCatalogOutput.rowCount = failureDataTable.Rows.Count;

            List<CustomRowDataInfo> CustomRowDataInfo = new List<CustomRowDataInfo>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            CustomRowDataInfo = GetCustomRowDataInfoRuleCatalog(failureDataTable);
            //string customInfoScript = serializer.Serialize(CustomRowDataInfo);
            //objSRMRuleCatalogOutput.customInfoScript = customInfoScript;

            failureDataTable.TableName = "DataTable";

            gridInfo.ViewKey = objSRMRuleCatalogOutput.viewKey;
            gridInfo.GridId = objSRMRuleCatalogOutput.gridID;
            gridInfo.CurrentPageId = objSRMRuleCatalogOutput.currentPageId;
            gridInfo.SessionIdentifier = objSRMRuleCatalogOutput.sessionIdentifier;
            gridInfo.CustomRowsDataInfo = CustomRowDataInfo;

            RADNeoGridService service = new RADNeoGridService();
            service.SaveGridDataInCache(failureDataTable, userName, objSRMRuleCatalogOutput.gridID, objSRMRuleCatalogOutput.currentPageId, objSRMRuleCatalogOutput.viewKey, objSRMRuleCatalogOutput.sessionIdentifier, false, gridInfo);
        }

        private static List<CustomRowDataInfo> GetCustomRowDataInfoRuleCatalog(DataTable dt)
        {
            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            List<CustomCellDatainfo> lstCustomCellDataInfo = new List<CustomCellDatainfo>();
            //CustomRowDataInfo objCustomRowDataInfo = null;
            if (!dt.Columns.Contains("row_keys"))
                dt.Columns.Add("row_keys");
            if (!dt.Columns.Contains("row_id"))
                dt.Columns.Add("row_id");

            int p = 0;
            foreach (DataRow dr in dt.Rows)
            {
                dr["row_id"] = p;
                dr["row_keys"] = p++;
                string row_id = Convert.ToString(dr["row_id"]);

                //objCustomRowDataInfo.Cells = lstCustomCellDataInfo;
                //lstCustomRowDataInfo.Add(objCustomRowDataInfo);
            }
            return lstCustomRowDataInfo;
        }
        #endregion

        #region landingpg
        public string GetLandingPageInfo(int moduleId)
        {
            DataSet ds;
            if (moduleId == 3)
                ds = CommonDALWrapper.ExecuteSelectQuery("EXEC [IVPSecmaster].[dbo].[SECM_GetAttrDetailsForLandingPage]", ConnectionConstants.SecMaster_Connection);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery("EXEC [IVPRefMaster].[dbo].[REFM_GetAttrDetailsForLandingPage] " + moduleId, ConnectionConstants.RefMaster_Connection);
            if (ds == null || ds.Tables.Count == 0)
                return null;
            return JsonConvert.SerializeObject(ds.Tables[0]);
        }

        public string GetRoleData(int typeId)
        {
            DataSet ds;
            ds = CommonDALWrapper.ExecuteSelectQuery(@"select entity_display_name from IVPRefMaster.dbo.ivp_refm_entity_type where entity_type_id =" + typeId + @";
select entity_display_name from IVPRefMaster.dbo.ivp_refm_entity_type where derived_from_entity_type_id =" + typeId + @" AND is_active = 1 AND structure_type_id = 3;
select entity_display_name from IVPRefMaster.dbo.ivp_refm_entity_type where derived_from_entity_type_id =" + typeId + @" AND is_active = 1 AND structure_type_id = 5;", ConnectionConstants.RefMaster_Connection);
            return JsonConvert.SerializeObject(ds);
        }

        #endregion

        #region Workflow

        public List<WorkflowActionInfo> TakeAction(List<WorkflowActionInfo> actionInfo, string actionName, string userName, string remarks, int moduleID, bool performFinalApproval = true)
        {
            if (actionName == "Re-Trigger")
                return SRMWorkflowController.ReTriggerWorkflow(actionInfo, userName, remarks);
            return SRMWorkflowController.TakeAction(actionInfo, actionName, userName, remarks, moduleID, performFinalApproval).actionOutput;
        }

        public bool ReTriggerWorkflow(List<WFRetriggerInfo> inputInfo, string userName, string remarks)
        {
            return SRMWorkflowController.ReTriggerWorkflow(inputInfo, userName, remarks).isSuccess;
        }

        public string BindGridForActionInfo(string username, string divID, string currPageID, string viewKey, string sessionID, string dateFormat, int ModuleId, string jsonGridInfo, List<WorkflowActionInfo> actionInfo)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            GridInfo gridInfo = serializer.Deserialize<GridInfo>(jsonGridInfo);
            RADNeoGridService service = new RADNeoGridService();

            DataTable dt = new DataTable("WorkflowActionErrorInfo");
            DataColumn instrumentIdDC;
            DataColumn instrumentName;
            if (ModuleId == 3)
            {
                instrumentIdDC = new DataColumn("Security Id");
                instrumentName = new DataColumn("Security Type Name");
            }
            else
            {
                instrumentIdDC = new DataColumn("Entity Code");
                instrumentName = new DataColumn("Entity Type Name");
            }
            DataColumn attributeNameDC = new DataColumn("Attribute Name");
            DataColumn legNameDC = new DataColumn("Leg Name");
            DataColumn exceptionDC = new DataColumn("Exception");
            DataColumn alertsDC = new DataColumn("Is Alert");
            DataColumn statusDC = new DataColumn("Status");
            DataColumn idColumn = new DataColumn("ID");


            dt.Columns.Add(instrumentIdDC);
            dt.Columns.Add(instrumentName);
            dt.Columns.Add(attributeNameDC);
            //dt.Columns.Add(legNameDC);
            dt.Columns.Add(exceptionDC);
            dt.Columns.Add(alertsDC);
            dt.Columns.Add(statusDC);
            dt.Columns.Add(idColumn);

            int id = 0;
            foreach (var ai in actionInfo)
            {
                foreach (var ex in ai.Exceptions)
                {
                    DataRow dr = dt.NewRow();
                    dr[idColumn] = id++;
                    dr[statusDC] = ai.isPassed ? "Passed" : "Failed";
                    dr[instrumentIdDC] = ai.InstrumentID;
                    dr[instrumentName] = ex.TypeName;
                    dr[attributeNameDC] = ex.AttributeName;
                    //dr[legNameDC] = ex.LegName;
                    dr[exceptionDC] = ex.Exception;
                    dr[alertsDC] = ex.isAlertOnly;
                    if (ex.UniquenessInfo != null)
                    {
                        List<KeyValuePair<string, List<SRMDuplicateInfo>>> info = null;
                        if (ModuleId == (int)SRMModules.Securities)
                            info = SRMWorkflowController.SM_MassageUniquenessInfo(ex.UniquenessInfo);
                        else if (ModuleId == (int)SRMModules.RefData || ModuleId == (int)SRMModules.Parties || ModuleId == (int)SRMModules.Funds)
                            info = SRMWorkflowController.RM_MassageUniquenessInfo(ex.UniquenessInfo);
                        dr[exceptionDC] = "<span class=\"grid-error-link\" onclick=\"SRMWorkflowInboxDetails.ShowUniquenessInfoPopup(event);\" data-uniqueness-info='" + serializer.Serialize(info) + "'>" + ex.Exception + "</span>";
                    }
                    dt.Rows.Add(dr);
                }
            }

            service.SaveGridDataInCache(dt, username, divID, currPageID, viewKey, sessionID, false, gridInfo);

            return null;
        }

        public List<WorkflowActionInfo> CheckActionOutput(List<WorkflowActionInfo> actionInfo, string actionName, string userName, string remarks, int moduleID)
        {
            return SRMWorkflowController.CheckActionOutput(actionInfo, actionName, userName, remarks, moduleID);
        }

        public List<WorkflowActionHistoryPerRequest> GetWorkflowActionHistoryByInstance(int instanceID, int moduleID, string dateFormat, string dateTimeFormat, string timeFormat, string userName)
        {
            return SRMWorkflowController.GetWorkflowActionHistoryByInstance(instanceID, moduleID, dateFormat, dateTimeFormat, timeFormat, userName);
        }

        public List<WorkflowActionHistoryPerRequest> GetWorkflowActionHistoryByInstrument(string instrumentID, int moduleID, string dateFormat, string dateTimeFormat, string timeFormat, string userName)
        {
            return SRMWorkflowController.GetWorkflowActionHistoryByInstrument(instrumentID, moduleID, dateFormat, dateTimeFormat, timeFormat, userName);
        }

        public List<SRMWorkflowCountPerType> GetWorkFlowInboxData(int moduleId, string userName)
        {
            List<SRMWorkflowCountPerType> result = new List<SRMWorkflowCountPerType>();

            result = SRMWorkflowController.GetWorkflowStatusCount(moduleId, userName);

            return result;

        }

        public List<WorkflowQueueData> GetWorkFlowQueueData(int moduleId, string userName, string workflowType, string statusType, string dateFormat)
        {
            List<WorkflowQueueData> result = new List<WorkflowQueueData>();

            result = SRMWorkflowController.GetWorkflowQueueData(moduleId, userName, workflowType, statusType, dateFormat, string.Empty);

            return result;
        }

        public List<SRMWorkflowRequestsCounts> GetWorkflowCounts(SRMWorkflowRequestsCountInfo info, List<int> typeIdsFilter)
        {
            var result = SRMWorkflowController.GetWorkflowCounts(info, typeIdsFilter);
            return result;
        }

        public List<WorkflowActionHistory> GetWorkflowActionHistory(int moduleId, int radWorkflowInstanceId)
        {
            return SRMWorkflowController.GetWorkflowActionHistory(moduleId, radWorkflowInstanceId);

        }

        #endregion

        #region associations pg
        public string Derivatives(int secTypeId)
        {

            SRMModelerController.SRMModelerController c = new SRMModelerController.SRMModelerController();
            return JsonConvert.SerializeObject(c.Derivatives(secTypeId));


        }

        public List<SRMAccess> GetRuleAccessDetails(int typeId, int moduleId)
        {

            SRMModelerController.SRMModelerController c = new SRMModelerController.SRMModelerController();
            return c.GetRuleAccessDetails(typeId, moduleId);


        }

        public string DerivativesLeft(int secTypeId)
        {

            SRMModelerController.SRMModelerController c = new SRMModelerController.SRMModelerController();
            return JsonConvert.SerializeObject(c.DerivativesLeft(secTypeId));


        }

        public string ConstituentRight(int secTypeId)
        {

            SRMModelerController.SRMModelerController c = new SRMModelerController.SRMModelerController();
            return JsonConvert.SerializeObject(c.ConstituentRight(secTypeId));


        }

        public List<string> GetAllowedGroups(int typeId, int moduleId)
        {

            SRMModelerController.SRMModelerController c = new SRMModelerController.SRMModelerController();
            return c.GetAllowedGroups(typeId, moduleId);


        }
        public List<SRMTemplateMaster> GetTemplateInfo(int typeId, int moduleId)
        {

            SRMModelerController.SRMModelerController c = new SRMModelerController.SRMModelerController();
            return c.GetTemplateInfo(typeId, moduleId);


        }
        public List<string> GetAllowedUsers(int typeId, int moduleId)
        {

            SRMModelerController.SRMModelerController c = new SRMModelerController.SRMModelerController();
            return c.GetAllowedUsers(typeId, moduleId);


        }

        public string ConstituentRightMost(int secTypeId)
        {

            SRMModelerController.SRMModelerController cc = new SRMModelerController.SRMModelerController();
            return JsonConvert.SerializeObject(cc.ConstituentRightMost(secTypeId));



        }
        public string SecTypeNameText(int secTypeId)
        {

            SRMModelerController.SRMModelerController cc = new SRMModelerController.SRMModelerController();
            return JsonConvert.SerializeObject(cc.SecTypeNameText(secTypeId));



        }

        //public SRMRuleCatalogOutput GetRules(string module, string tab, List<string> ruleTypes, List<int> types, string userName)
        //{
        //    SRMRuleCatalogOutput objSRMRuleCatalogOutput = new SRMRuleCatalogOutput();
        //    RuleCatalogController getRulesCatalog = new RuleCatalogController();
        //    List<SRMRuleType> rulesTypesSrm = null;
        //    if (ruleTypes != null)
        //    {
        //        rulesTypesSrm = new List<SRMRuleType>();
        //        foreach (string s in ruleTypes)
        //        {
        //            SRMRuleType objSRMRuleType;
        //            if (Enum.TryParse(s, true, out objSRMRuleType))
        //                rulesTypesSrm.Add(objSRMRuleType);
        //        }
        //    }
        //    SRMTabType moduleSubTab;
        //    Enum.TryParse(tab, true, out moduleSubTab);
        //    SRMModule moduleName;
        //    Enum.TryParse(module, true, out moduleName);
        //    DataTable getRules = new DataTable();

        //    getRules = getRulesCatalog.GetRules(moduleName, moduleSubTab, rulesTypesSrm, types, userName);

        //    ////Set values for existing rows  
        //    SaveFailureDataInCacheRuleCatalog(getRules, objSRMRuleCatalogOutput, "admin");

        //    return objSRMRuleCatalogOutput;
        //}
        #endregion

        #region accesssss pg
        public string GetTemplateDetails(int templateId, int moduleId)
        {
            SRMModelerController.SRMModelerController cc = new SRMModelerController.SRMModelerController();
            return JsonConvert.SerializeObject(cc.GetTemplateDetails(templateId, moduleId));
        }
        #endregion

        #region DB locks

        public DBLocksInfo GetDBLocksData(string divID, string currPageID, string viewKey, string sessionID, string jsonGridInfo, string startDate, string endDate)
        {
            mLogger.Error("GetDBLocksData ==> Start");
            DBLocksInfo result = new DBLocksInfo();

            try
            {
                string StartDate = null, EndDate = null;
                if (startDate != null)
                    StartDate = Convert.ToDateTime(startDate).ToString("yyyyMMdd");
                if (endDate != null)
                    EndDate = Convert.ToDateTime(endDate).ToString("yyyyMMdd");

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                GridInfo gridInfo = serializer.Deserialize<GridInfo>(jsonGridInfo);
                RADNeoGridService service = new RADNeoGridService();

                string whereQuery = string.Empty;
                //where query
                if (StartDate != null && EndDate != null)
                {
                    if (StartDate != EndDate)
                        whereQuery = "where " + "time_of_first_lock_attempt >= '" + StartDate + "' AND time_of_first_lock_attempt < DATEADD(dd,1,'" + EndDate + "')";
                    else
                        whereQuery = "where " + "time_of_first_lock_attempt >= '" + StartDate + "'";
                }
                else if (StartDate != null)
                {
                    whereQuery = "where " + "time_of_first_lock_attempt >= '" + StartDate + "'";
                }
                else if (EndDate != null)
                {
                    whereQuery = "where " + "time_of_first_lock_attempt <= '" + EndDate + "'";
                }

                string query = "SELECT id,user_name as [User Name], machine_name AS [Machine Name], CAST(time_of_first_lock_attempt as varchar) 'First attempt', CAST(time_when_lock_acquired as varchar) 'When acquired',CAST(time_when_lock_released as varchar) 'When released', identifier as Identifier, source_of_lock, process_id as [Process Id], process_name as [Process Name] from IVPSecMaster.dbo.ivp_secm_data_upload_manager_locks " + whereQuery + " ";

                //query for current lock
                query += "SELECT TOP 1 id,user_name as [User Name], machine_name AS [Machine Name], CAST(time_of_first_lock_attempt as varchar) 'First attempt', CAST(time_when_lock_acquired as varchar) 'When acquired',CAST(time_when_lock_released as varchar) 'When released', identifier as Identifier, source_of_lock, process_id as [Process Id], process_name as [Process Name] from IVPSecMaster.dbo.ivp_secm_data_upload_manager_locks " + (string.IsNullOrEmpty(whereQuery) ? "WHERE " : whereQuery + " AND ") + " time_of_first_lock_attempt IS NOT NULL AND time_when_lock_released IS NULL AND time_when_lock_acquired IS NOT NULL ORDER BY time_when_lock_acquired";

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.SecMaster_Connection);

                //GDPR username massaging
                RUserManagementService objUserController = new RUserManagementService();
                List<RUserInfo> radUsers = objUserController.GetAllUsersGDPR();

                Dictionary<string, string> dictUserLoginNamevsFullName = radUsers.ToDictionary(x => x.UserLoginName, y => y.FullName + " (" + y.UserName + ")");

                foreach (DataRow dr1 in ds.Tables[0].Rows)
                {
                    if (ds.Tables[0].Columns.Contains("User Name"))
                    {
                        string user = Convert.ToString(dr1["User Name"]);
                        if (!string.IsNullOrEmpty(user) && dictUserLoginNamevsFullName.ContainsKey(user))
                            dr1["User Name"] = dictUserLoginNamevsFullName[user];
                    }
                }

                List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo> columnsToHide = new List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo>();

                if (ds.Tables[0].Columns.Contains("id"))
                {
                    columnsToHide.Add(new com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo() { ColumnName = "id", isDefault = true });
                    columnsToHide.Add(new HiddenColumnInfo() { ColumnName = "source_of_lock", isDefault = true });
                }

                gridInfo.ColumnsToHide = columnsToHide;
                ds.Tables[0].Columns.Add("Stack Trace", typeof(string));

                //customRowInfo for hyperlink 
                gridInfo.CustomRowsDataInfo = GetCustomRowDataInfoDBLocks(ds);

                service.SaveGridDataInCache(ds.Tables[0], "admin", divID, currPageID, viewKey, sessionID, false, gridInfo);

                result.rowCount = ds.Tables[0].Rows.Count;

                //row on which current lock is
                if (ds.Tables[1].Rows.Count > 0)
                {
                    result.ifLockExistsCurrently = true;
                    result.userName = Convert.ToString(ds.Tables[1].Rows[0]["User Name"]);
                    result.machineName = Convert.ToString(ds.Tables[1].Rows[0]["Machine Name"]);
                    result.firstAttempt = Convert.ToString(ds.Tables[1].Rows[0]["First attempt"]);
                    result.whenAcquired = Convert.ToString(ds.Tables[1].Rows[0]["When acquired"]);
                    result.whenReleased = Convert.ToString(ds.Tables[1].Rows[0]["When released"]);
                    result.identifier = Convert.ToString(ds.Tables[1].Rows[0]["Identifier"]);
                    result.stackTrace = Convert.ToString(ds.Tables[1].Rows[0]["source_of_lock"]);
                    result.processId = !string.IsNullOrEmpty(Convert.ToString(ds.Tables[1].Rows[0]["Process Id"])) ? Convert.ToInt32(ds.Tables[1].Rows[0]["Process Id"]) : 0;
                    result.processName = Convert.ToString(ds.Tables[1].Rows[0]["Process Name"]);
                }

                return result;
            }
            catch (Exception ex)
            {
                result.rowCount = 0;
                return result;
            }
        }

        private static List<CustomRowDataInfo> GetCustomRowDataInfoDBLocks(DataSet ds)
        {
            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            List<CustomCellDatainfo> lstCustomCellDataInfo = new List<CustomCellDatainfo>();
            CustomRowDataInfo objCustomRowDataInfo = null;
            DataTable dt = ds.Tables[0];
            string logInfo;
            //string abc = "abcsfkd";
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    objCustomRowDataInfo = new CustomRowDataInfo();
                    CustomCellDatainfo cellLogInfo = new CustomCellDatainfo();

                    cellLogInfo.ColumnName = "Stack Trace";

                    string colorTypeForHyperLinkText = "#48a3dd";

                    //ignore old entries that didn't have process name, process id etc
                    if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])) && string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Process Id"])) && string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Process Name"])))
                    {
                        //do nothing
                    }

                    //row color changed
                    //red color
                    else if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["When acquired"])) && string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["When released"])))
                    {
                        objCustomRowDataInfo.StyleAttribute.Add("background", "#cc7386");
                        objCustomRowDataInfo.StyleAttribute.Add("color", "white");
                        colorTypeForHyperLinkText = "white";
                    }

                    //blue color
                    else if (string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["When acquired"])) && string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["When released"])))
                    {
                        objCustomRowDataInfo.StyleAttribute.Add("background", "#d6e4ec");
                        objCustomRowDataInfo.StyleAttribute.Add("color", "inherit");
                    }

                    //set clickable error message
                    if (!string.IsNullOrEmpty(Convert.ToString(GetText(dt.Rows[i]["source_of_lock"]))))
                    {
                        dt.Rows[i]["Stack Trace"] = Convert.ToString(dt.Rows[i]["source_of_lock"]).Substring(0, 50) + "...";
                        cellLogInfo.NewChild = "<span class='DataUploadClickablePopup' message=\"" + Convert.ToString(dt.Rows[i]["source_of_lock"]) + "\" } style='text-decoration:underline;cursor:pointer;color:" + colorTypeForHyperLinkText + ";' >" + dt.Rows[i]["Stack Trace"] + "</span>";
                    }
                    objCustomRowDataInfo.Cells.Add(cellLogInfo);

                    objCustomRowDataInfo.RowID = Convert.ToString(dt.Rows[i]["id"]);

                    lstCustomRowDataInfo.Add(objCustomRowDataInfo);

                }
            }
            return lstCustomRowDataInfo;
        }

        #endregion

        #region Task manager   
        public Dictionary<string, ProcessInfo> FetchTaskManagerValues()
        {
            Dictionary<string, ProcessInfo> dictProcIdVSInfo = new Dictionary<string, ProcessInfo>();
            try
            {
                //Process[] processlist = Process.GetProcesses();
                //Parallel.ForEach(processlist, theprocess =>
                //{
                //    lock (lockObj)
                //    {
                //        dictProcIdVSInfo[theprocess.ProcessName] = tempMethod(theprocess);
                //    }
                //});

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfProc_Process");
                Parallel.ForEach(searcher.Get().Cast<ManagementObject>(), obj =>
                {
                    //if (obj["IDProcess"].ToString() == Process.GetCurrentProcess().Id.ToString())

                    var usage = obj["PercentProcessorTime"];
                    var name = obj["Name"];
                    if (!string.IsNullOrEmpty(Convert.ToString(name)))
                    {

                        lock (lockObj)
                        {
                            dictProcIdVSInfo[Convert.ToString(name)] = calculateCPU(Convert.ToInt32(usage));
                        }
                    }
                });

                string query = "Select * From Win32_Process";// Where ProcessID = " + Process.GetCurrentProcess().Id;
                searcher = new ManagementObjectSearcher(query);
                Parallel.ForEach(searcher.Get().Cast<ManagementObject>(), obj =>
                {
                    //if (obj["IDProcess"].ToString() == Process.GetCurrentProcess().Id.ToString())
                    string processName = Convert.ToString(obj["Name"]);
                    lock (lockObj)
                    {
                        if (!string.IsNullOrEmpty(processName))
                        {
                            ProcessInfo pinfo = null;
                            if (dictProcIdVSInfo.TryGetValue(processName, out pinfo))
                            {
                                try
                                {
                                    pinfo.memoryUsed = Convert.ToInt32(obj["WorkingSetSize"]);
                                    pinfo.processId = Convert.ToInt32(obj["ProcessId"]);
                                    pinfo.processName = Convert.ToString(obj["Name"]);
                                    string[] argList = new string[] { string.Empty, string.Empty };
                                    obj.InvokeMethod("GetOwner", argList);
                                    if (!(string.IsNullOrEmpty(argList[0]) && string.IsNullOrEmpty(argList[1])))
                                        pinfo.userName = argList[1] + "\\" + argList[0];
                                    else
                                        pinfo.userName = null;
                                }
                                catch (Exception e) { }

                            }
                            else
                            {
                                try
                                {
                                    pinfo = new ProcessInfo();
                                    pinfo.memoryUsed = Convert.ToInt32(obj["WorkingSetSize"]);
                                    pinfo.processId = Convert.ToInt32(obj["ProcessId"]);
                                    pinfo.processName = Convert.ToString(obj["Name"]);
                                    string[] argList = new string[] { string.Empty, string.Empty };
                                    obj.InvokeMethod("GetOwner", argList);
                                    if (!(string.IsNullOrEmpty(argList[0]) && string.IsNullOrEmpty(argList[1])))
                                        pinfo.userName = argList[1] + "\\" + argList[0];
                                    else
                                        pinfo.userName = null;
                                    dictProcIdVSInfo[processName] = pinfo;
                                }
                                catch (Exception e) { }

                            }
                        }
                    }

                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dictProcIdVSInfo;
        }

        public Dictionary<string, servicesInfo> FetchSystemServicesValues()
        {
            Dictionary<string, servicesInfo> dictserviceNameVSInfo = new Dictionary<string, servicesInfo>();
            try
            {
                //ServiceController[] servicesLst = ServiceController.GetServices();
                //Parallel.ForEach(servicesLst, theservice =>
                //{
                //    lock (lockObj)
                //    {
                //        dictProcIdVSInfo[theprocess.ProcessName] = tempMethod(theprocess);
                //    }
                //});
                string XMLPath = "com.ivp.common.CommonUI.CommonUI.ConfigFiles.secmasterServicesConfig.xml";
                Assembly asm = AppDomain.CurrentDomain.Load("CommonUI");
                XElement xml = XElement.Load(asm.GetManifestResourceStream(XMLPath));
                List<string> lstServices = xml.Descendants("Service").Select(node => node.Attribute("Name").Value).ToList();

                foreach (ServiceController service in ServiceController.GetServices())
                {
                    mLogger.Debug("First Service Name:" + service.ServiceName);
                    string currentserviceName = string.Empty;
                    servicesInfo servobj = new servicesInfo();
                    string serviceName = service.ServiceName;
                    string serviceDisplayName = service.DisplayName;
                    string serviceType = service.ServiceType.ToString();
                    string status = service.Status.ToString();
                    using (ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + serviceName + "'"))
                    {
                        mLogger.Debug(" Second Service Name:" + service.ServiceName + "\n");
                        wmiService.Get();
                        string currentserviceExePath = wmiService["PathName"].ToString();

                        if (currentserviceExePath.StartsWith("\""))
                        { currentserviceExePath = currentserviceExePath.Substring(1); }
                        if (currentserviceExePath.EndsWith("\""))
                        { currentserviceExePath = currentserviceExePath.Substring(0, (currentserviceExePath.Length - 1)); }

                        //  string formattedPath = Path.GetInvalidFileNameChars().Aggregate(currentserviceExePath, (current, c) => current.Replace(c.ToString(), string.Empty));
                        try
                        {
                            if (currentserviceExePath.Contains("SecMasterService"))
                                currentserviceName = Path.GetFileName(currentserviceExePath);
                            else
                                currentserviceName = Path.GetFileName(currentserviceExePath);
                            mLogger.Debug(" First currentserviceName Name:" + currentserviceName + "\n");
                        }
                        catch (Exception e)
                        {
                            //servobj.serviceName = Convert.ToString(wmiService["DisplayName"]);
                            //servobj.path = currentserviceExePath;
                            //if (!dictserviceNameVSInfo.ContainsKey(currentserviceName))
                            //    dictserviceNameVSInfo.Add(currentserviceName, servobj);
                            mLogger.Debug("In Catch :" + service.ServiceName + "\n");
                        }
                        mLogger.Debug(" Third Service Name:" + service.ServiceName + "\n");
                        //servobj.serviceName = Convert.ToString(wmiService["DisplayName"]);
                        //servobj.path = currentserviceExePath;
                        //if (!dictserviceNameVSInfo.ContainsKey(currentserviceName))
                        //    dictserviceNameVSInfo.Add(currentserviceName, servobj);
                        if (currentserviceName == "SecMasterService")
                        {
                            string adsds = "efefd";
                        }
                        if (lstServices.Contains(currentserviceName))
                        {

                            servobj.path = currentserviceExePath;
                            servobj.processId = Convert.ToInt32(wmiService["ProcessId"]);
                            servobj.serviceName = Convert.ToString(wmiService["DisplayName"]);
                            servobj.status = Convert.ToString(wmiService["State"]);
                            if (!dictserviceNameVSInfo.ContainsKey(currentserviceName))
                                dictserviceNameVSInfo.Add(currentserviceName, servobj);
                            mLogger.Debug(" Fourth Service Name:" + service.ServiceName + "\n");
                        }
                        mLogger.Debug(" Fifth Service Name:" + service.ServiceName + "\n");
                    }
                }


                //foreach (ServiceController service in ServiceController.GetServices())
                //{
                //    string serviceName = service.ServiceName;
                //    string serviceDisplayName = service.DisplayName;
                //    string serviceType = service.ServiceType.ToString();
                //    string status = service.Status.ToString();

                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dictserviceNameVSInfo;
        }

        public string OnRestartService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(10000);

                if (service.Status.ToString() == "Running")
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(10000 - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                return "success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string onStartStopService(string serviceName, string existingStatus)
        {
            ServiceController service = new ServiceController(serviceName);
            int millisec1 = Environment.TickCount;
            TimeSpan timeout = TimeSpan.FromMilliseconds(1000);

            try
            {
                if (existingStatus.ToLower().Equals("running") || existingStatus.ToLower().Equals("started"))
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
                else if (existingStatus.ToLower().Equals("stopped"))
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                return "success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string startAppPool()
        {
            using (ServerManager s = new ServerManager())
            {
                Application application = null;
                foreach (Site site in s.Sites)
                {

                    application = site.Applications.Where(x => x.Path.Equals(AppDomain.CurrentDomain.BaseDirectory, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (application != null)
                        break;
                }
                ApplicationPool pool = s.ApplicationPools[application.ApplicationPoolName.Trim()];
                if (pool != null)
                {
                    try
                    {
                        pool.Start();
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                    return "success";
                }
                else
                    return "Application Pool is null";
            }
        }
        public string stopAppPool()
        {
            using (ServerManager s = new ServerManager())
            {
                Application application = null;
                foreach (Site site in s.Sites)
                {

                    application = site.Applications.Where(x => x.Path.Equals(AppDomain.CurrentDomain.BaseDirectory, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (application != null)
                        break;
                }
                ApplicationPool pool = s.ApplicationPools[application.ApplicationPoolName.Trim()];
                if (pool != null)
                {
                    try
                    {
                        pool.Stop();
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                    return "success";
                }
                else
                    return "Application Pool is null";
            }
        }
        public ProcessInfo calculateCPU(int usage)
        {
            ProcessInfo pinfo = new ProcessInfo();
            pinfo.CPUUsage = usage;
            return pinfo;
        }

        #endregion

        #region TimeFormat

        public ServerFormat GetServerFormat()
        {
            com.ivp.rad.viewmanagement.RSessionInfo sessionInfo = new RCommon().SessionInfo;

            ServerFormat obj = new ServerFormat();
            obj.longFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
            obj.shortFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            obj.clientShortFormat = sessionInfo.CultureInfo.ShortDateFormat;
            obj.clientLongFormat = sessionInfo.CultureInfo.LongDateFormat;

            return obj;
        }
        //public ServerFormat GetServerFormat(string startDate, string endDate, string format, string shortformat)
        //{
        //    string syslongFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
        //    string sysShortFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        //    ServerFormat obj = new ServerFormat();
        //    bool flagStartShort = false, flagEndShort = false;
        //    string start_date = "";
        //    string end_date = "";
        //    DateTime startDateDT, endDateDT;
        //    if (DateTime.TryParseExact(startDate, format, CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.ToString()), DateTimeStyles.None, out startDateDT))
        //        flagStartShort = false;
        //    else if (DateTime.TryParseExact(startDate, shortformat, CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.ToString()), DateTimeStyles.None, out startDateDT))
        //        flagStartShort = true;

        //    if (DateTime.TryParseExact(endDate, shortformat, CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.ToString()), DateTimeStyles.None, out endDateDT))
        //        flagEndShort = true;
        //   obj.serverFormat = sysShortFormat;
        //    obj.startDateOriginal = startDateDT.ToString(sysShortFormat);
        //    obj.endDateOriginal = endDateDT.ToString(sysShortFormat);

        //    //if (DateTime.TryParseExact(endDate, format, CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.ToString()), DateTimeStyles.None, out endDateDT))
        //    //    flagEndShort = false;
        //    //else

        //    if (!flagStartShort && flagEndShort)
        //    {
        //        TimeSpan time = new TimeSpan(0, 23, 59, 59);
        //        endDateDT = endDateDT.Add(time);
        //        end_date = endDateDT.ToString(syslongFormat);
        //        start_date = startDateDT.ToString(syslongFormat);
        //    }
        //    else if (flagStartShort && flagEndShort)
        //    {
        //        TimeSpan time = new TimeSpan(0, 23, 59, 59);
        //        TimeSpan time1 = new TimeSpan(0, 0, 0, 0);

        //        startDateDT = startDateDT.Add(time1);
        //        endDateDT = endDateDT.Add(time);
        //        end_date = endDateDT.ToString(syslongFormat);
        //        start_date = startDateDT.ToString(syslongFormat);
        //    }
        //    //else if (!flagStartShort && !flagEndShort)
        //    //{
        //    //    start_date = startDateDT.ToString(syslongFormat);
        //    //    end_date = endDateDT.ToString(syslongFormat);
        //    //}
        //    //else if (flagStartShort && flagEndShort)
        //    //{
        //    //    start_date = startDateDT.ToString(sysShortFormat);
        //    //    end_date = endDateDT.ToString(sysShortFormat);
        //    //}

        //    obj.startDate = start_date;
        //    obj.endDate = end_date;

        //    return obj;
        //}

        public string GetClientDateFromServerDate(string date, string format)
        {
            return Convert.ToDateTime(date).ToString(format);
        }
        #endregion


        #region SRM Migration Utility

        public List<MigrationFeaturesInfoUI> GetMigrationUtilityModulesList(string username, int moduleId)
        {
            List<MigrationFeaturesInfoUI> result = new List<MigrationFeaturesInfoUI>();
            try
            {
                mLogger.Debug("GetMigrationUtilityModulesList: Start");
                var objModuleIdVsFeaturesList = new SRMCommonMigration().GetFeaturesList(moduleId);
                foreach (var item in objModuleIdVsFeaturesList)
                {
                    result.Add(new MigrationFeaturesInfoUI
                    {
                        id = Convert.ToInt32(item),
                        name = Enum<MigrationFeatureEnum>.GetDescription(item)
                    });
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("GetMigrationUtilityModulesList: " + ex.Message);
            }
            finally
            {
                mLogger.Debug("GetMigrationUtilityModulesList: End");
            }
            return result;
        }

        public string GetSelectableItemsForMigration(int moduleID, int feature, string userName)
        {
            return JsonConvert.SerializeObject(new SRMCommonMigration().GetSelectableItemsForMigration(moduleID, feature, userName));
        }

        public string DownloadMigrationConfiguration(List<int> features, List<object> selectedItems, int moduleID, string userName, bool isExcelFile)
        {
            string errorMessage = string.Empty;
            try
            {
                string instanceName = SRMCommon.GetConfigFromDB("InstanceName");
                mLogger.Debug("DownloadMigrationConfiguration: Start");
                SRMCommonMigration objSRMCommonMigration = new SRMCommonMigration();
                List<MigrationFeatureInfo> featuresList = new List<MigrationFeatureInfo>();
                Dictionary<string, ObjectSet> workbookNameVsObjectSet = new Dictionary<string, ObjectSet>();

                features.ForEach(
                    x => featuresList.Add(new MigrationFeatureInfo { FeatureEnum = (MigrationFeatureEnum)x, ModuleID = moduleID })
                    );

                objSRMCommonMigration.DownloadMigrationConfiguration(featuresList, selectedItems, moduleID, userName, SRMMigrationUserAction.Download, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    throw new Exception(errorMessage);

                var featureEnumVsWorkbookName = objSRMCommonMigration.GetEnumValueVsFeatureDisplayName();
                foreach (var item in featuresList)
                {
                    if (item.TargetSet == null)
                        throw new Exception("Unable to download file.");
                    if (item.TargetSet.Tables.Count > 0 && item.TargetSet.Tables.First().Rows.Count > 0)
                        workbookNameVsObjectSet.Add(GetModuleNameForWorkbookVsModuleId(moduleID) + " - " + featureEnumVsWorkbookName[Convert.ToInt32(item.FeatureEnum)] + " - " + instanceName, item.TargetSet);
                }

                if (workbookNameVsObjectSet.Count == 0)
                {
                    throw new Exception("Cannot download file" + (features.Count() > 1 ? "s" : "") + " as no configuration exists in it.");
                }

                string zipFileNameBase = GetModuleNameForWorkbookVsModuleId(moduleID) + " - " + "Configurations" + " - " + instanceName;

                string filePath = SRMCommon.WriteMultipleFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/"), workbookNameVsObjectSet, moduleID, false, false, userName, !isExcelFile, zipFileNameBase);

                if (string.IsNullOrEmpty(filePath))
                    throw new Exception("File download error");

                return filePath.Replace("\\", "\\\\");
            }
            catch (Exception ex)
            {
                mLogger.Error("DownloadMigrationConfiguration: " + ex.Message);
                return "ž" + ex.Message;
            }
            finally
            {
                mLogger.Debug("DownloadMigrationConfiguration: End");
            }
        }

        public DownloadViewConfiguration DownloadViewConfigurationForDWH(string systemName)
        {
            DownloadViewConfiguration downloadViewConfiguration = new DownloadViewConfiguration();
            string errorMessage = string.Empty;
            try
            {
                mLogger.Debug("DownloadViewConfiguration: Start");
                SRMDWHJob job = new SRMDWHJob();
                string location = job.DownloadViewDefinition(systemName, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                    throw new Exception(errorMessage);

                SRMCommonZip objSRMCommonZip = new SRMCommonZip();
                string zipFileName = systemName + ".7z";
                try
                {
                    if (SRMCommon.CopyDirectoryContents(location, location+ "FilesToBeZipped")){
                        objSRMCommonZip.Zip(location+ "FilesToBeZipped"+"\\\\", location  + zipFileName);
                    }
                
                }
                catch (Exception ex)
                {
                    mLogger.Error("DownloadViewConfiguration: " + ex.Message);
                    downloadViewConfiguration.errorMessage = ex.Message;
                }

                string downloadLocation = location + zipFileName;
                downloadViewConfiguration.downloadLocation = downloadLocation.Replace("\\", "\\\\");
            }
            catch (Exception ex)
            {
                mLogger.Error("DownloadViewConfiguration: " + ex.Message);
                downloadViewConfiguration.errorMessage = ex.Message;
            }
            finally
            {
                mLogger.Debug("DownloadViewConfiguration: End");
            }
            return downloadViewConfiguration;
        }
        public string UploadMigrationConfiguration(List<int> features, bool isSync, bool requireMissing, int moduleID, string userName, string dateFormat, string dateTimeFormat, string timeFormat, string filePath, bool isExcel, bool isBulkUpload)
        {
            string errorMsg = string.Empty;
            try
            {
                mLogger.Debug("UploadMigrationConfiguration: Start");

                string instanceName = SRMCommon.GetConfigFromDB("InstanceName");

                string downloadFilePath = string.Empty;
                string zipFileNameBase = GetModuleNameForWorkbookVsModuleId(moduleID) + " - " + "Configurations" + " - " + instanceName;
                Dictionary<string, ObjectSet> workbookNameVsObjectSet = new Dictionary<string, ObjectSet>();

                List<MigrationFeatureInfo> featuresList = new List<MigrationFeatureInfo>();
                if (string.IsNullOrEmpty(filePath))
                    throw new Exception("File Path not provided");

                filePath.Replace("\\", "\\\\");

                //zip -> convert to files and then pass info down


                List<MigrationFeatureEnum> featureEnumList = new List<MigrationFeatureEnum>();
                Dictionary<MigrationFeatureEnum, Tuple<string, string>> featureEnumVsFilePathVsFileExtension = new Dictionary<MigrationFeatureEnum, Tuple<string, string>>();

                SRMCommonMigration objMigration = null;
                if (isBulkUpload)
                {
                    //extract file to a location
                    string targetExtractFileLocation = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "MigrationUtilityFiles\\" + userName + "\\" + DateTime.Now.ToString("yyyy-MM-dd--hh--mm--ss") + "\\ExtractedFiles";
                    FileInfo fileExtensionUploaded = new FileInfo(filePath);
                    if (fileExtensionUploaded.Extension == ".7z")
                    {
                        new SRMCommonZip().UnZip(filePath, targetExtractFileLocation);
                        var directories = Directory.GetDirectories(targetExtractFileLocation);
                        if (directories.Length != 1)
                            throw new Exception("Extra folders provided in 7zip file");
                        DirectoryInfo dir = new DirectoryInfo(directories.First());
                        FileInfo[] files = dir.GetFiles();
                        featureEnumVsFilePathVsFileExtension = new SRMCommonMigration().ValidateFileInfoAndGetFileEnumMapping(files, moduleID);
                    }
                    else
                    {
                        FileInfo[] files = new FileInfo[] { new FileInfo(filePath) };
                        featureEnumVsFilePathVsFileExtension = new SRMCommonMigration().ValidateFileInfoAndGetFileEnumMapping(files, moduleID);
                        isBulkUpload = false;
                    }
                    featureEnumList.AddRange(featureEnumVsFilePathVsFileExtension.Keys);

                    objMigration = new SRMCommonMigration(featureEnumList, moduleID);

                    foreach (var item in featureEnumVsFilePathVsFileExtension)
                    {
                        var extension = item.Value.Item2;
                        var extractedFilePath = item.Value.Item1;

                        ObjectSet sourceSet = null;
                        string originalFilePath = extractedFilePath;
                        FileInfo fi = new FileInfo(filePath);
                        string newFilePath = originalFilePath;
                        if (newFilePath.Length > 256)
                        {
                            newFilePath = fi.DirectoryName + "\\" + fi.Name.GetHashCode().ToString() + fi.Extension.ToString();
                            fi.MoveTo(newFilePath);
                        }

                        if (extension == ".xml")
                        {
                            featuresList.Add(new MigrationFeatureInfo { FeatureEnum = item.Key, ModuleID = moduleID, SourceSet = objMigration.getObjectSetFromXML(newFilePath, userName, item.Key) });
                        }
                        else if (extension == ".xlsx" || extension == ".xls")
                        {
                            featuresList.Add(new MigrationFeatureInfo { FeatureEnum = item.Key, ModuleID = moduleID, SourceSet = objMigration.getObjectSetFromExcel(newFilePath, userName, item.Key) });
                        }
                        else
                        {
                            throw new Exception("Unsupported File Type uploaded for " + item.Key.GetDescription());
                        }
                        if (newFilePath.Length > 256)
                        {
                            fi.MoveTo(originalFilePath);
                        }
                    }

                }
                else
                {
                    foreach (var x in features)
                    {
                        featureEnumList.Add((MigrationFeatureEnum)x);
                    }

                    objMigration = new SRMCommonMigration(featureEnumList, moduleID);

                    foreach (var x in features)
                    {
                        {
                            ObjectSet sourceSet = null;
                            string originalFilePath = filePath;
                            FileInfo fi = new FileInfo(filePath);
                            string newFilePath = originalFilePath;
                            string fileExtension = fi.Extension.ToString();
                            if (newFilePath.Length > 256)
                            {
                                newFilePath = fi.DirectoryName + "\\" + fi.Name.GetHashCode().ToString() + fi.Extension.ToString();
                                fi.MoveTo(newFilePath);
                            }

                            if (fileExtension == ".xml")
                                sourceSet = objMigration.getObjectSetFromXML(newFilePath, userName, (MigrationFeatureEnum)x);
                            else if (fileExtension == ".xlsx" || fileExtension == ".xls")
                                sourceSet = objMigration.getObjectSetFromExcel(newFilePath, userName, (MigrationFeatureEnum)x);
                            else
                            {
                                throw new Exception("Unsupported File Type uploaded for " + ((MigrationFeatureEnum)x).GetDescription());
                            }
                            if (newFilePath.Length > 256)
                            {
                                fi.MoveTo(originalFilePath);
                            }
                            if (sourceSet == null || sourceSet.Tables.Count == 0)
                            {
                                throw new Exception(((MigrationFeatureEnum)x).GetDescription() + " : Unable to parse the uploaded file. Please make sure the sheet names are as expected by the application.");
                            }
                            featuresList.Add(new MigrationFeatureInfo { FeatureEnum = (MigrationFeatureEnum)x, ModuleID = moduleID, SourceSet = sourceSet });
                        }
                    }
                }


                objMigration.UploadMigrationConfiguration(featuresList, isSync, requireMissing, moduleID, userName, dateFormat, dateTimeFormat, timeFormat, out errorMsg);

                if (!string.IsNullOrEmpty(errorMsg))
                    throw new Exception(errorMsg);

                //prepare for download
                var featureEnumVsWorkbookName = objMigration.GetEnumValueVsFeatureDisplayName();

                List<MigrationFeatureErrorInfo> errorInfoList = new List<MigrationFeatureErrorInfo>();
                //upload case
                if (isSync)
                {
                    foreach (var item in featuresList)
                    {
                        //string fileName = GetModuleNameForWorkbookVsModuleId(moduleID) + " " + featureEnumVsWorkbookName[Convert.ToInt32(item.FeatureEnum)];
                        string fileName = GetModuleNameForWorkbookVsModuleId(moduleID) + " - " + featureEnumVsWorkbookName[Convert.ToInt32(item.FeatureEnum)] + " - " + instanceName;
                        workbookNameVsObjectSet.Add(fileName, item.DeltaSet);
                        if (item.DeltaSet == null)
                            throw new Exception("Error parsing uploaded file");
                        MigrationFeatureErrorInfo objMigrationFeatureErrorInfo = new MigrationFeatureErrorInfo();
                        objMigrationFeatureErrorInfo.ErrorMsg = item.ErrorMsg;
                        objMigrationFeatureErrorInfo.SyncStatus = item.SyncStatus;
                        objMigrationFeatureErrorInfo.FeatureDisplayName = item.FeatureEnum.GetDescription();
                        objMigrationFeatureErrorInfo.FeatureEnum = item.FeatureEnum;
                        objMigrationFeatureErrorInfo.FileName = fileName;
                        objMigrationFeatureErrorInfo.ModuleID = moduleID;
                        objMigrationFeatureErrorInfo.IsDownloadable = item.IsDownloadable;
                        errorInfoList.Add(objMigrationFeatureErrorInfo);
                    }

                    downloadFilePath = SRMCommon.WriteMultipleFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/"), workbookNameVsObjectSet, moduleID, false, true, userName, !isExcel, zipFileNameBase);
                    downloadFilePath = downloadFilePath.Replace("\\", "\\\\");
                    //.Replace("." + x.FileName.Split('.').Last(), "")
                    if (isBulkUpload)
                    {
                        string basePathForUnZippedFiles = Path.GetDirectoryName(downloadFilePath) + "\\FilesToBeZipped" + "\\";
                        errorInfoList.ForEach(x => x.FilePath = basePathForUnZippedFiles + x.FileName + " - Upload Result." + (isExcel ? "xlsx" : "xml"));
                        //errorInfoList.Add(new MigrationFeatureErrorInfo { FilePath = downloadFilePath, FileName = Path.GetFileName(downloadFilePath), FeatureDisplayName = MigrationFeatureEnum.All_Features.GetDescription(), FeatureEnum = MigrationFeatureEnum.All_Features });
                    }
                    else
                    {
                        errorInfoList.First().FilePath = downloadFilePath;
                    }
                    return downloadFilePath + "Ÿ" + JsonConvert.SerializeObject(errorInfoList);
                }

                //download diff
                else
                {
                    bool downloadDiffIsEmpty = true;
                    foreach (var item in featuresList)
                    {
                        //if (item.DeltaSet == null && featuresList.Count == 1)
                        //    throw new Exception("Download diff objectset not provided");

                        if (item.DeltaSet.Tables.Count == 0)
                        {
                            downloadDiffIsEmpty = downloadDiffIsEmpty && true;
                        }
                        else
                        {
                            downloadDiffIsEmpty = downloadDiffIsEmpty && false;
                        }

                        if (item.DeltaSet != null && item.DeltaSet.Tables.Count > 0)
                        {
                            workbookNameVsObjectSet.Add(GetModuleNameForWorkbookVsModuleId(moduleID) + " - " + featureEnumVsWorkbookName[Convert.ToInt32(item.FeatureEnum)] + " - " + instanceName, item.DeltaSet);
                        }
                        //if (item.DeltaSet == null)
                        //    throw new Exception("Download diff objectset not provided");
                    }
                    if (downloadDiffIsEmpty)
                        return "¡" + "DownloadDiffError";
                    downloadFilePath = SRMCommon.WriteMultipleFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/"), workbookNameVsObjectSet, moduleID, true, false, userName, !isExcel, zipFileNameBase);
                    return downloadFilePath.Replace("\\", "\\\\");
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("UploadMigrationConfiguration:" + ex.Message);
                return "ž" + ex.Message;
            }
            finally
            {
                mLogger.Debug("UploadMigrationConfiguration: End");
            }
        }

        public static string GetModuleNameForWorkbookVsModuleId(int moduleId)
        {
            string result = "";
            try
            {
                switch (moduleId)
                {
                    case 3: result = SRMModuleNames.Security_Master.GetDescription(); break;
                    case 6: result = SRMModuleNames.Reference_Master.GetDescription(); break;
                    case 18: result = SRMModuleNames.Fund_Master.GetDescription(); break;
                    case 20: result = SRMModuleNames.Party_Master.GetDescription(); break;
                    default: throw new Exception("GetModuleNameForWorkbookVsModuleId : Wrong module Id supplied");
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        #endregion

        #region Attribute Meta data

        public SRFPMAttributeMetaDataOutputObject GetAttributeMetaData(SRFPMAttributeMetaDataInputObject InputObject)
        {
            SRFPMAttributeMetaDataOutputObject result = null;

            try
            {
                if (InputObject != null)
                {
                    if (InputObject.ModuleId == 3)
                    {
                        //SEC
                        //SMDBManager objSMDBManager = new SMDBManager();
                        //result = objSMDBManager.SM_GetAttributeMetaData(InputObject);
                    }
                    else
                    {
                        //REF

                        result = new RMCommonDBManager().RFPM_GetAttributeMetaData(InputObject);



                    }
                }
                else
                {
                    throw new Exception("Invalid Input to GetAttributeMetaData");
                }

                return result;
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetAttributeMetadata -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {

            }
        }

        #endregion Attribute Meta data

        #region Report Viewer
        public string SecM_GetReportConfig(int reportSetupID, string loginName)
        {
            DataTable ReportDt = null; //string NoDataFound;
            mLogger.Debug("CommonService -> GetDataForReportViewer -> Start");
            try
            {
                //objSMReportViewController.GetDataForReport(objSMReportViewInfo);
                Assembly SecMasterReportAssembly = Assembly.Load("SecMasterReportController");
                Type objTypeReportViewController = SecMasterReportAssembly.GetType("com.ivp.secm.reportcontroller.SMReportViewController");
                MethodInfo getReportData = objTypeReportViewController.GetMethod("GetDataForReport");
                var objReportViewController = Activator.CreateInstance(objTypeReportViewController);

                Type objTypeReportViewInfoClass = SecMasterReportAssembly.GetType("com.ivp.secm.reportcontroller.SMReportViewInfo");
                var objTypeReportViewInfo = Activator.CreateInstance(objTypeReportViewInfoClass);

                PropertyInfo propInfo = objTypeReportViewInfoClass.GetProperty("ReportSetupId");
                propInfo.SetValue(objTypeReportViewInfo, reportSetupID, null);

                propInfo = objTypeReportViewInfoClass.GetProperty("LoginName");
                propInfo.SetValue(objTypeReportViewInfo, loginName, null);

                DataSet ReportDs = (DataSet)getReportData.Invoke(objReportViewController, new object[] { objTypeReportViewInfo });
                if (ReportDs != null || ReportDs.Tables.Count > 0)
                {
                    ReportDt = ReportDs.Tables[0];
                    return JsonConvert.SerializeObject(ReportDt);

                }
                else return "";

            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetDataForReportViewer -> Exception ->" + ex.ToString());
            }
            finally
            {
                mLogger.Debug("CommonService -> GetDataForReportViewer -> End");
            }
            return null;
        }

        public string GetReportLevel()
        {
            IDictionary<string, string> ReportLevelList = null;
            mLogger.Debug("CommonService -> GetReportLevel -> Start");
            try
            {
                Assembly ReportLevelAssembly = Assembly.Load("SecMasterCommons");
                Type objTypeReportLevelViewController = ReportLevelAssembly.GetType("com.ivp.secm.commons.SMSetupConfigLoader");
                MethodInfo getReportLevelData = objTypeReportLevelViewController.GetMethod("GetSetupConfiguration", BindingFlags.Public | BindingFlags.Static);
                var objReportViewController = Activator.CreateInstance(objTypeReportLevelViewController);

                ReportLevelList = (IDictionary<string, string>)getReportLevelData.Invoke(objReportViewController, new object[] { "RepositoryType" });
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetReportLevel -> Exception ->" + ex.ToString());
            }
            finally
            {
                mLogger.Debug("CommonService -> GetReportLevel -> End");
            }
            return JsonConvert.SerializeObject(ReportLevelList);
        }

        public string GetReportType(int selectedReportLevelID, int moduleID)
        {
            mLogger.Debug("CommonService -> GetReportType -> Start");

            DataTable ReportTypeDT = null;
            try
            {
                if (moduleID == 3)
                {
                    Assembly SecMasterReportAssembly = Assembly.Load("SecMasterReportController");
                    Type objTypeReportViewController = SecMasterReportAssembly.GetType("com.ivp.secm.reportcontroller.SMReportViewController");
                    MethodInfo getReportType = objTypeReportViewController.GetMethod("GetReportType");
                    var objReportViewController = Activator.CreateInstance(objTypeReportViewController);

                    ReportTypeDT = (DataTable)getReportType.Invoke(objReportViewController, new object[] { selectedReportLevelID });
                    ReportTypeDT.Columns["report_type"].ColumnName = "text";
                    ReportTypeDT.Columns["report_id"].ColumnName = "value";

                }
                else
                {
                    Assembly RefMasterReportAssembly = Assembly.Load("RefMController");
                    Type objTypeReportRespositoryController = RefMasterReportAssembly.GetType("com.ivp.refmaster.controller.RMReportRepositoryController");
                    MethodInfo getReportType = objTypeReportRespositoryController.GetMethod("GetReportTypes");
                    var objReportRespositoryController = Activator.CreateInstance(objTypeReportRespositoryController);

                    var TypeDs = (DataSet)getReportType.Invoke(objReportRespositoryController, new object[] { });
                    ReportTypeDT = TypeDs.Tables[0];
                    ReportTypeDT.Columns["Report Type Name"].ColumnName = "text";
                    ReportTypeDT.Columns["Report Type ID"].ColumnName = "value";

                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetReportType -> Exception ->" + ex.ToString());
            }
            finally
            {
                mLogger.Debug("CommonService -> GetReportType -> End");
            }
            return JsonConvert.SerializeObject(ReportTypeDT);
        }

        public string GetReportNamesForReportId(int selectedReportLevelID, int selectedReportID, string loginName, int moduleID)
        {
            mLogger.Debug("CommonService -> GetReportNamesForReportId -> Start");

            DataTable ReportNamesDt = null;
            try
            {
                if (moduleID == 3)
                {
                    Assembly SecMasterReportAssembly = Assembly.Load("SecMasterReportController");
                    Type objTypeReportViewController = SecMasterReportAssembly.GetType("com.ivp.secm.reportcontroller.SMReportViewController");
                    MethodInfo getReportNamesForReportId = objTypeReportViewController.GetMethod("GetReportNamesForReportId");
                    var objReportViewController = Activator.CreateInstance(objTypeReportViewController);

                    Type objTypeReportViewInfoClass = SecMasterReportAssembly.GetType("com.ivp.secm.reportcontroller.SMReportViewInfo");
                    var objTypeReportViewInfo = Activator.CreateInstance(objTypeReportViewInfoClass);

                    PropertyInfo propInfo = objTypeReportViewInfoClass.GetProperty("ReportLevel");
                    propInfo.SetValue(objTypeReportViewInfo, selectedReportLevelID, null);

                    propInfo = objTypeReportViewInfoClass.GetProperty("ReportId");
                    propInfo.SetValue(objTypeReportViewInfo, selectedReportID, null);

                    propInfo = objTypeReportViewInfoClass.GetProperty("LoginName");
                    propInfo.SetValue(objTypeReportViewInfo, loginName, null);

                    DataSet ReportNameDs = (DataSet)getReportNamesForReportId.Invoke(objReportViewController, new object[] { objTypeReportViewInfo });
                    ReportNamesDt = ReportNameDs.Tables[0];
                    ReportNamesDt.Columns["report_setup_id"].ColumnName = "value";
                }
                else
                {
                    Assembly RefMasterReportAssembly = Assembly.Load("RefMController");
                    Type objTypeReportViewerController = RefMasterReportAssembly.GetType("com.ivp.refmaster.controller.RMReportViewerController");
                    MethodInfo getReportByType = objTypeReportViewerController.GetMethod("GetReportByReportType");
                    var objReportViewerController = Activator.CreateInstance(objTypeReportViewerController);

                    var NameDs = (DataSet)getReportByType.Invoke(objReportViewerController, new object[] { selectedReportID });
                    ReportNamesDt = NameDs.Tables[0];
                    ReportNamesDt.Columns["report_id"].ColumnName = "value";
                }
                ReportNamesDt.Columns["report_name"].ColumnName = "text";
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetReportNamesForReportId -> Exception ->" + ex.ToString());
            }
            finally
            {
                mLogger.Debug("CommonService -> GetReportNamesForReportId -> End");
            }
            return JsonConvert.SerializeObject(ReportNamesDt);
        }
        public string Refm_GetFeedBrowserReport_Instances(int reportID, string longDateFormat)
        {
            Assembly RefMasterReportAssembly = Assembly.Load("RefMController");
            Type objTypeReportViewerController = RefMasterReportAssembly.GetType("com.ivp.refmaster.controller.RMReportViewerController");
            MethodInfo getReportInstances = objTypeReportViewerController.GetMethod("GetInstancesForReport");
            var objReportViewerController = Activator.CreateInstance(objTypeReportViewerController);
            DataSet dsInstances = (DataSet)getReportInstances.Invoke(objReportViewerController, new object[] { reportID });
            DataTable dtInstances = new DataTable();
            dtInstances.Columns.Add("start_time");
            dtInstances.Columns.Add("task_instance_id");

            if (dsInstances != null && dsInstances.Tables.Count > 0 && dsInstances.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsInstances.Tables[0].Rows.Count; i++)
                {
                    DataRow drInstance = dtInstances.NewRow();
                    DataRow dr = dsInstances.Tables[0].Rows[i];
                    drInstance["start_time"] = dr["task_instance_id"] + ". " +
                        RCalenderUtils.FormatDate(DateTime.Parse(dr["start_time"].ToString()),
                           longDateFormat);
                    drInstance["task_instance_id"] = dr["task_instance_id"];
                    dtInstances.Rows.Add(drInstance);
                }
                return JsonConvert.SerializeObject(dtInstances);
            }
            else
                return "There are no instances loaded.";


        }
        public string Refm_GetAuditHistory_Dates(int reportID, int ReportTypeId, string shortDateFormat)
        {
            Assembly RefMasterReportAssembly = Assembly.Load("RefMController");
            Type objTypeReportConfigController = RefMasterReportAssembly.GetType("com.ivp.refmaster.controller.RMReportingConfigureController");
            MethodInfo getReportConfig = objTypeReportConfigController.GetMethod("GetReportConfigurationInfoById");
            var objReportConfigController = Activator.CreateInstance(objTypeReportConfigController);
            RMReportConfigurationInfo rcInfo = (RMReportConfigurationInfo)getReportConfig.Invoke(objReportConfigController, new object[] { reportID });
            DataTable DTAuditHistoryDates = new DataTable();
            DTAuditHistoryDates.Columns.Add("knowledgeDate");
            DTAuditHistoryDates.Columns.Add("startDate");
            DTAuditHistoryDates.Columns.Add("endDate");

            if (rcInfo != null)
            {
                DataRow row = DTAuditHistoryDates.NewRow();
                row[0] = DateTime.Now.ToString(shortDateFormat);
                if (rcInfo.startDate == 100) // Case for last extraction date
                {
                    if (string.IsNullOrEmpty(rcInfo.LastExtractionDate))
                    {
                        row[1] = RCalenderUtils.ConvertISOFormatToDate("19000101 00:00:00", RDateLengthFormat.Long).ToString(shortDateFormat); // let the min sql date be default
                    }
                    else
                    {
                        row[1] = RCalenderUtils.FormatDate(DateTime.Parse(rcInfo.LastExtractionDate),
                           shortDateFormat);
                    }
                    row[2] = RCalenderUtils.FormatDate(DateTime.Now, shortDateFormat);
                }
                else
                {
                    //**RMBASE REPORT
                    Type objTypeBaseReportInfo = null;
                    object BaseReportObj = null;
                    Assembly RefMasterReporting = Assembly.Load("RefMReporting");
                    Type DateTypeEnumType = RefMasterReporting.GetType("com.ivp.refmaster.reporting.RMReportingDateTypeEnum");
                    Type RefMBaseReport_Type = RefMasterReporting.GetType("com.ivp.refmaster.reporting.RMBaseReport");
                    // object ReportingDateTypeEnum = Activator.CreateInstance(DateTypeEnumType);
                    if (ReportTypeId == 5)
                    {
                        objTypeBaseReportInfo = RefMasterReporting.GetType("com.ivp.refmaster.reporting.RMEntityTypeReport");
                        //BaseReportObj = Activator.CreateInstance(objTypeBaseReportInfo);
                        //RefGenerateReport = objTypeBaseReportInfo.GetMethod("GenerateReport");
                    }
                    else if (ReportTypeId == 6)
                    {
                        objTypeBaseReportInfo = RefMasterReporting.GetType("com.ivp.refmaster.reporting.RMAcrossEntityTypeReport");

                    }
                    BaseReportObj = Activator.CreateInstance(objTypeBaseReportInfo);

                    MethodInfo RefGetReportDate = RefMBaseReport_Type.GetMethod("GetReportDate", new[] { typeof(int), DateTypeEnumType, typeof(string), typeof(bool) });
                    var enumValueStartDate = Enum.ToObject(DateTypeEnumType, rcInfo.startDate);
                    string startDate = (string)RefGetReportDate.Invoke(BaseReportObj, new object[] { rcInfo.calendarId, enumValueStartDate, rcInfo.customStartDate, true });
                    if (!string.IsNullOrEmpty(startDate))
                        row[1] = DateTime.ParseExact
                            (startDate, new string[] { "yyyyMMdd HH:mm:ss.fff", "yyyyMMdd" }, null, DateTimeStyles.None).ToString(shortDateFormat);
                    else
                        row[1] = string.Empty;
                    var enumValueEndDate = Enum.ToObject(DateTypeEnumType, rcInfo.endDate);
                    string endDate = (string)RefGetReportDate.Invoke(BaseReportObj, new object[] { rcInfo.calendarId, enumValueEndDate, rcInfo.customEndDate, false });
                    if (!string.IsNullOrEmpty(endDate))
                        row[2] = DateTime.ParseExact
                            (endDate, new string[] { "yyyyMMdd HH:mm:ss.fff", "yyyyMMdd" }, null, DateTimeStyles.None).ToString(shortDateFormat);
                    else
                        row[2] = string.Empty;

                }
                DTAuditHistoryDates.Rows.Add(row);
                return JsonConvert.SerializeObject(DTAuditHistoryDates);
            }
            else
            {
                return "The report is not configured.";
            }

            //return JsonConvert.SerializeObject(dtInstances);

        }
        public string LoadReportSec(int ReportID, int ReportSetupID, string UserName, string EnvironmentCulture) //get datatable reportId = reportTypeId
        {
            mLogger.Debug("CommonService -> LoadReport -> Start");

            DataSet FinalReportDs = null;
            try
            {
                Assembly SecMasterReportAssembly = Assembly.Load("SecMasterReporting");
                // Type objTypeBaseReportController = SecMasterReportAssembly.GetType("com.ivp.secm.reporting.SMBaseReport");
                //MethodInfo createReport = objTypeBaseReportController.GetMethod("CreateReport");
                //var BaseReportController = Activator.CreateInstance(objTypeBaseReportController);
                //  Type objTypeReportInfo = SecMasterReportAssembly.GetType("com.ivp.secm.reportcontroller.SMBaseReportInfo");
                Type objTypeBaseReportInfo = SecMasterReportAssembly.GetType("com.ivp.secm.reporting.SMBaseReportInfo");
                var BaseReportObj = Activator.CreateInstance(objTypeBaseReportInfo);
                PropertyInfo propInfo = objTypeBaseReportInfo.GetProperty("ReportID");
                propInfo.SetValue(BaseReportObj, ReportID, null);

                propInfo = objTypeBaseReportInfo.GetProperty("ReportSetupID");
                propInfo.SetValue(BaseReportObj, ReportSetupID, null);

                propInfo = objTypeBaseReportInfo.GetProperty("IsPerSecType");
                propInfo.SetValue(BaseReportObj, false, null);
                propInfo = objTypeBaseReportInfo.GetProperty("PerSecTypeIndex");
                propInfo.SetValue(BaseReportObj, 0, null);
                propInfo = objTypeBaseReportInfo.GetProperty("ContainsReportObject");
                propInfo.SetValue(BaseReportObj, false, null);
                propInfo = objTypeBaseReportInfo.GetProperty("UserName");
                propInfo.SetValue(BaseReportObj, UserName, null);
                propInfo = objTypeBaseReportInfo.GetProperty("EnvironmentCulture");
                propInfo.SetValue(BaseReportObj, EnvironmentCulture, null);

                //var ReportFinalObj = Convert.ChangeType(reportObj, objTypeReportInfo);

                Type ReportingMgrType = SecMasterReportAssembly.GetType("com.ivp.secm.reporting.SMReportingManager");
                var ReportingMgrController = Activator.CreateInstance(ReportingMgrType);
                MethodInfo createReport = ReportingMgrType.GetMethod("CreateReport");
                //int reportSetupID = reportObj.ReportSetupID;
                var objBaseReport = createReport.Invoke(ReportingMgrType, new object[] { ReportSetupID });
                MethodInfo GetReport = objBaseReport.GetType().GetMethod("GetReportData", new[] { typeof(System.Object) });

                //object baseobj = Convert.ChangeType(objBaseReport, typeof(System.Object));
                //object basereport = new object();

                //var props = objTypeBaseReportInfo.GetProperties();
                //foreach (var prop in props)
                //{
                //    basereport.GetType().GetProperties().ToList().Add(prop);
                //}

                FinalReportDs = (DataSet)GetReport.Invoke(objBaseReport, new object[] { BaseReportObj });
                //if (FinalReportDs != null || FinalReportDs.Tables.Count > 0)
                //    return null;
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> LoadReport -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> LoadReport -> End");
            }
            if (FinalReportDs == null || FinalReportDs.Tables.Count < 1 || FinalReportDs.Tables[0].Rows.Count < 1)
                return "No Data Available For Selected Combination.";
            else return (JsonConvert.SerializeObject(FinalReportDs.Tables[0]));

        }
        public string LoadReportRef(string ReportID_reportTypeID, int instanceID, string reportName, string knowledgeDate, string startDate, string EndDate, string shortLongDatePatterns) //get datatable reportId = reportTypeId
        {
            mLogger.Debug("CommonService -> LoadReport -> Start");

            DataSet FinalReportDs = null;
            try
            {
                string shortDateFormat = shortLongDatePatterns.Split('|')[0];
                string longdateFormat = shortLongDatePatterns.Split('|')[1];
                int ReportId = Convert.ToInt32(ReportID_reportTypeID.Split('|')[0]);
                int ReportTypeId = Convert.ToInt32(ReportID_reportTypeID.Split('|')[1]);
                Assembly RefMasterReportAssembly = Assembly.Load("RefMController");
                Type objTypeReportViewerController = RefMasterReportAssembly.GetType("com.ivp.refmaster.controller.RMReportViewerController");
                MethodInfo getReportDisplayData = objTypeReportViewerController.GetMethod("GetReportDisplayData");
                var objReportViewerController = Activator.CreateInstance(objTypeReportViewerController);
                Assembly RefMasterReporting = Assembly.Load("RefMReporting");

                //**RMBASE REPORT
                Type objTypeBaseReportInfo = null;//RefMasterReporting.GetType("com.ivp.refmaster.reporting.RMBaseReport");
                //var BaseReportObj = Activator.CreateInstance(objTypeBaseReportInfo);
                object BaseReportObj = null;
                MethodInfo RefGenerateReport = null;
                //****CALL 1 GetReportDisplayData()***
                var dsResult = (DataSet)getReportDisplayData.Invoke(objReportViewerController, new object[] { ReportId });

                if (dsResult == null)
                    return "Empty Data Set Returned,  Please check the configuration of report.";
                //int ReportTypeId = ReportTypeID;

                RMBaseReportClass objBaseReport = new RMBaseReportClass();
                MemoryStream memStream = null;

                int DependentId = Convert.ToInt32(dsResult.Tables[0].Rows[0]["dependent_id"]);

                Type objTypeReportingManagerControllerType = RefMasterReporting.GetType("com.ivp.refmaster.reporting.RMReportingManager");
                MethodInfo RefCreateReport = objTypeReportingManagerControllerType.GetMethod("CreateReport", new[] { typeof(System.Object) });
                var objTypeReportingManagerController = Activator.CreateInstance(objTypeReportingManagerControllerType);

                if (ReportTypeId == 4) // feed browser 
                {
                    objTypeBaseReportInfo = RefMasterReporting.GetType("com.ivp.refmaster.reporting.RMFeedTypeReport");
                    //BaseReportObj = Activator.CreateInstance(objTypeBaseReportInfo);
                    //RefGenerateReport = objTypeBaseReportInfo.GetMethod("GenerateReport");
                    if (instanceID != 0)
                    { objBaseReport.InstanceId = instanceID; }
                    else
                        return "There are no instances loaded";
                }
                else if (ReportTypeId == 2 || ReportTypeId == 5)
                {
                    objTypeBaseReportInfo = RefMasterReporting.GetType("com.ivp.refmaster.reporting.RMEntityTypeReport");
                    //BaseReportObj = Activator.CreateInstance(objTypeBaseReportInfo);
                    //RefGenerateReport = objTypeBaseReportInfo.GetMethod("GenerateReport");
                }
                else if (ReportTypeId == 1 || ReportTypeId == 6)
                {
                    objTypeBaseReportInfo = RefMasterReporting.GetType("com.ivp.refmaster.reporting.RMAcrossEntityTypeReport");

                }
                BaseReportObj = Activator.CreateInstance(objTypeBaseReportInfo);
                RefGenerateReport = objTypeBaseReportInfo.GetMethod("GenerateReport");
                //****CALL 2 createReport***

                //object Createdreport = RefCreateReport.Invoke(objTypeReportingManagerController, new object[] { ReportID });
                //Convert.ChangeType(Createdreport, BaseReportObj.GetType());
                //BaseReportObj = Createdreport;
                //var b = Convert.ChangeType(rep, BaseReportObj.GetType());
                objBaseReport.ReportId = ReportId;
                objBaseReport.ReportName = System.Text.RegularExpressions.Regex.Replace(reportName, " ", "_");
                objBaseReport.ReportTypeId = ReportTypeId;

                objBaseReport.LongDateFormat = longdateFormat;

                //Audit History Reports -- start
                if (!string.IsNullOrEmpty(startDate))
                    objBaseReport.StartDate = RCalenderUtils.FormatDate(startDate, shortDateFormat, "yyyyMMdd");
                else
                    objBaseReport.StartDate = "";
                string endDate = null;
                if (!string.IsNullOrEmpty(EndDate))
                {
                    endDate = RCalenderUtils.FormatDate(EndDate, shortDateFormat, "yyyyMMdd");
                    System.Globalization.Calendar cal = new GregorianCalendar();
                    DateTime endDatePlusOne = cal.AddDays(RCalenderUtils.ConvertISOFormatToDate(endDate, RDateLengthFormat.Short), 1);
                    objBaseReport.EndDate = RCalenderUtils.ConvertDateToISOFormat(endDatePlusOne, RDateLengthFormat.Short);
                }
                else
                    objBaseReport.EndDate = "";
                if (!string.IsNullOrEmpty(knowledgeDate))
                    objBaseReport.KnowledgeDate = RCalenderUtils.FormatDate(knowledgeDate, shortDateFormat, "yyyyMMdd");
                // --end

                objBaseReport.DataSetName = reportName;


                //MethodInfo RefGenerateReport = objTypeBaseReportInfo.GetMethod("GenerateReport");


                PropertyInfo propInfo = objTypeBaseReportInfo.GetProperty("ReportTypeId");
                propInfo.SetValue(BaseReportObj, ReportTypeId, null);
                propInfo = objTypeBaseReportInfo.GetProperty("DataSetName");
                propInfo.SetValue(BaseReportObj, objBaseReport.DataSetName, null);
                propInfo = objTypeBaseReportInfo.GetProperty("InstanceId");
                propInfo.SetValue(BaseReportObj, objBaseReport.InstanceId, null);
                propInfo = objTypeBaseReportInfo.GetProperty("ReportId");
                propInfo.SetValue(BaseReportObj, ReportId, null);
                propInfo = objTypeBaseReportInfo.GetProperty("ReportName");
                propInfo.SetValue(BaseReportObj, objBaseReport.ReportName, null);
                propInfo = objTypeBaseReportInfo.GetProperty("LongDateFormat");
                propInfo.SetValue(BaseReportObj, objBaseReport.LongDateFormat, null);
                propInfo = objTypeBaseReportInfo.GetProperty("StartDate");
                propInfo.SetValue(BaseReportObj, objBaseReport.StartDate, null);
                propInfo = objTypeBaseReportInfo.GetProperty("EndDate");
                propInfo.SetValue(BaseReportObj, objBaseReport.EndDate, null);
                propInfo = objTypeBaseReportInfo.GetProperty("KnowledgeDate");
                propInfo.SetValue(BaseReportObj, objBaseReport.KnowledgeDate, null);

                //****CALL 3 GenerateReport()***

                memStream = (MemoryStream)RefGenerateReport.Invoke(BaseReportObj, new object[] { "SSRS" });
                if (memStream == null)
                {
                    return "There was an error processing the report. Please check the configuration of report.";
                }
                MethodInfo RefGetReportData = objTypeBaseReportInfo.GetMethod("GetReportData");
                DataSet dsReport = (DataSet)RefGetReportData.Invoke(BaseReportObj, new object[] { });
                if (memStream == null || dsReport == null || dsReport.Tables.Count < 1 || dsReport.Tables[0].Rows.Count < 1)
                    return "There is no data in the report with respect to selected instance.";
                if (dsReport.Tables[0].Columns.Contains("entity_code") == true)
                    dsReport.Tables[0].Columns["entity_code"].ColumnName = "Entity Code";
                if (dsReport.Tables[0].Columns.Contains("last_modified_by") == true)
                    dsReport.Tables[0].Columns["last_modified_by"].ColumnName = "Last Modified By";
                if (dsReport.Tables[0].Columns.Contains("last_modified_on") == true)
                    //dsReport.Tables[0].Columns["last_modified_on"].ColumnName = "Last Modified On";
                    dsReport.Tables[0].Columns["last_modified_on"].ColumnName = "Effective Date";
                if (dsReport.Tables[0].Columns.Contains("Entity_Type") == true)
                    dsReport.Tables[0].Columns["Entity_Type"].ColumnName = "Entity Type";

                if (ReportTypeId == 5)
                    dsReport.Tables[0].TableName = "Entity Type Audit History Report";
                if (ReportTypeId == 6)
                    dsReport.Tables[0].TableName = "Across Entity Type Audit History Report";
                if (dsReport.Tables[0].Columns.Contains("is_active") == true)
                    dsReport.Tables[0].Columns.Remove("is_active");
                if (dsReport.Tables[0].Columns.Contains("is_deleted") == true)
                    dsReport.Tables[0].Columns.Remove("is_deleted");
                if (dsReport.Tables[0].Rows.Count > 0 && (ReportTypeId == 5 || ReportTypeId == 6))
                {
                    IEnumerable<DataRow> q;
                    if (dsReport.Tables[0].Columns.Contains("isArchiveRecord"))
                        q = dsReport.Tables[0].AsEnumerable().Where(dr => Convert.ToBoolean(dr["isArchiveRecord"]) == true || Convert.ToBoolean(dr["isTimeSeriesRecord"]) == true); //.CopyToDataTable();
                    else
                        q = dsReport.Tables[0].AsEnumerable();

                    if (dsReport.Tables[0].Columns.Contains("Effective Start Date"))
                        q = q.OrderBy(dr => dr["entity code"].ToString()).ThenBy(dr => Convert.ToDateTime(dr["Effective Start Date"]));
                    else if (dsReport.Tables[0].Columns.Contains("Effective Date"))
                        q = q.OrderBy(dr => dr["entity code"].ToString()).ThenBy(dr => Convert.ToDateTime(dr["Effective Date"]));
                    else if (dsReport.Tables[0].Columns.Contains("loading time"))
                        q = q.OrderBy(dr => dr["entity code"].ToString()).ThenBy(dr => Convert.ToDateTime(dr["loading time"]));
                    else
                        q = q.OrderBy(dr => dr["entity code"].ToString());

                    DataTable dt = q.CopyToDataTable();
                    dt.TableName = dsReport.Tables[0].TableName;
                    dsReport.Tables.Remove(dsReport.Tables[0]);
                    dt.TableName = reportName;
                    dsReport.Tables.Add(dt.Copy());

                }
                FinalReportDs = dsReport;
                // return null;
                //    FinalReportDs = objBaseReport.GetReportData();

            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> LoadReport -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> LoadReport -> End");
            }
            if (FinalReportDs == null || FinalReportDs.Tables.Count < 1)
                return null;
            else return (JsonConvert.SerializeObject(FinalReportDs.Tables[0]));

        }
        public string GetReportHeader(int ReportSetupID, string moduleID, string reportName) //get datatable reportId = reportTypeId
        {
            int moduleId = Convert.ToInt32(moduleID);
            DataTable dtHeader = null;
            if (moduleId == 3)
                dtHeader = CommonDALWrapper.ExecuteSelectQuery(@"select report_header from IVPSecMaster.dbo.ivp_secm_report_configuration where report_setup_id = " + ReportSetupID, ConnectionConstants.SecMaster_Connection).Tables[0];
            else if (moduleId == 6)
                dtHeader = CommonDALWrapper.ExecuteSelectQuery(@"select report_header from IVPRefMaster.dbo.ivp_refm_report_configuration where report_id =" + ReportSetupID, ConnectionConstants.RefMaster_Connection).Tables[0];

            if (dtHeader != null && dtHeader.Rows.Count > 0 && !string.IsNullOrEmpty(Convert.ToString(dtHeader.Rows[0][0])))
                return Convert.ToString(dtHeader.Rows[0][0]);
            else return reportName;
        }
        #endregion

        #region DownstreamSync
        //public string GetSetupNames()
        //{
        //    RHashlist mHList = new RHashlist();
        //    try
        //    {
        //        DataSet setupDS = CommonDALWrapper.ExecuteSelectQuery("select setup_name from  IVPRefMaster.dbo.ivp_srm_dwh_downstream_master", ConnectionConstants.RefMaster_Connection);
        //        string s = string.Join(",", blockTypeDS.Tables[0].Rows.OfType<DataRow>().Select(r => r[0].ToString()));
        //        return s;
        //    }
        //    catch (Exception Ex)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        if (mHList != null)
        //        {
        //            mHList.Clear();
        //            mHList = null;
        //        }
        //    }
        //}
        public string GetBlockTypes()
        {
            return new SRMDownstreamConfiguration().GetBlockTypes();
        }

        public List<SRMDownstreamSyncInfo> GetAllConfigDataInitial(string selectedSystemName)
        {
            return new SRMDownstreamConfiguration().GetAllConfigDataInitial(selectedSystemName);
        }

        public SRMDownstreamSyncSetupDetails GetSelectedConnectionDetails(string connectionName)
        {
            return new SRMDownstreamConfiguration().GetSelectedConnectionDetails(connectionName);
        }
        public DownstreamSchedulerInfo GetSchedulerData(string selectedSystemName)
        {
            return SRMDownstreamStatusAndScheduler.GetSchedulerData(selectedSystemName);
        }

        public string SRMDownstreamSyncSaveReports(SRMDownstreamSyncInfo Systems, bool IsNewSystem, DownstreamSchedulerInfo scheduleInfo,string dateFormat)
        {
            string errorMessage = string.Empty;
            new SRMDownstreamConfiguration().SRMDownstreamSyncSaveReports(Systems, IsNewSystem, out errorMessage, SRMDownstreamStatusAndScheduler.getObjectTableFor(scheduleInfo),"",dateFormat);
            //if (!string.IsNullOrEmpty(errorMessage))
            // throw new Exception(errorMessage);
            return errorMessage;
        }
        public void SRMDownstreamSyncTriggerReports(string SetupName)
        {
            //new SRMDWHJob().ExecuteDWHTaskJob(SetupName, new RCommon().SessionInfo.LoginName);
            //SRMDWHJobQueue.Enqueue(SetupName, new RCommon().SessionInfo.LoginName);
            string exeConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SRMDWHService.exe.config");
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(exeConfigPath);
            string url = xDoc.GetElementsByTagName("baseAddresses")[0]["add"].GetAttribute("baseAddress");

            SRMDWHInputInfo inputInfo = new SRMDWHInputInfo() { SetupName = SetupName, UserName = new RCommon().SessionInfo.UserName, WaitForResponse = false };

            var serObject = new DataContractJsonSerializer(typeof(SRMDWHInputInfo));
            var memStream = new MemoryStream();
            serObject.WriteObject(memStream, inputInfo);
            memStream.Position = 0;
            byte[] requestBodyBytes = new ASCIIEncoding().GetBytes(new StreamReader(memStream).ReadToEnd());
            //string url = "http://localhost/SRMDWHServiceQA8.2MT2";
            SRMCommon.InvokeRestAPI(url, new Dictionary<string, string>(), requestBodyBytes, "TriggerDWHSync", "POST");
        }
        public string SRMDownstreamSyncAddNewSystem(SRMDownstreamSyncSetupDetails SetupDetails, string SystemName, string EffectiveDate, int CalendarType)
        {
            string errorMessage = string.Empty;
            new SRMDownstreamConfiguration().SRMDownstreamSyncAddNewSystem(SetupDetails, SystemName, EffectiveDate, CalendarType, out errorMessage);
            return errorMessage;
        }
        public List<string> SRMDownstreamSyncGetExistingConnections()
        {
            return new SRMDownstreamConfiguration().SRMDownstreamSyncGetExistingConnections();
        }


        #endregion DownstreamSync
        public string GetIPAddress()
        {
            string ip = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Request.ServerVariables != null)
            {
                ip = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
                if (string.IsNullOrEmpty(ip))
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }

        public List<DownstreamSyncStatusResult> GetAllDownstreamSyncStatus(string startDate, string endDate, string dateFormat, string resultDateTimeFormat, string selDateOption, string CustomRadioOption)
        {
            return SRMDownstreamStatusAndScheduler.GetAllDownstreamSyncStatus(startDate, endDate, dateFormat, resultDateTimeFormat, selDateOption, CustomRadioOption);
        }

        public List<DownstreamSyncReportMessage> GetReportStatusMessage(List<int> blockStatusId, string resultDateTimeFormat)
        {
            return SRMDownstreamStatusAndScheduler.GetReportStatusMessage(blockStatusId, resultDateTimeFormat);
        }
        #region Exception Common Config
        public ExceptionsConfig getExceptionConfigDetails(int moduleId, int instrumentTypeId)
        {
            return SRMCommon.getExceptionConfigDetails(moduleId, instrumentTypeId);
        }

        public string saveExceptionConfigDetails(List<ExceptionsConfig> configs, string user)
        {
            return SRMCommon.saveExceptionConfigDetails(configs, user);
        }

        #endregion

        #region Event Config
        public List<SRMEventInstrumentTypesConfigList> GetEventInstrumentTypesConfigList(List<int> moduleIds)
        {
            return SRMEventController.GetEventInstrumentTypesConfigList(moduleIds);
        }

        public void DeleteEventSettingForInstrumentType(int moduleId, int instrumentTypeId)
        {
            SRMEventController.DeleteEventSettingForInstrumentType(moduleId, instrumentTypeId);
        }
        public SRMEventsAllLevelActions GetEventConfigForInstrumentType(int moduleId, int instrumentTypeId)
        {
            return SRMEventController.GetEventConfigForInstrumentType(moduleId, instrumentTypeId);
        }

        public List<string> getTransportNames()
        {
            return SRMEventController.getTransportNames();
        }

        public void SaveEventInstrumentTypesConfigList(int moduleId, int instrumentTypeId, SRMEventsAllLevelActions config, string username)
        {
            SRMEventController.SaveEventConfigForInstrumentType(moduleId, instrumentTypeId, config, username);
        }

        public string ExecuteQuery(string query)
        {
            RDBConnectionManager con = RDALAbstractFactory.DBFactory.GetConnectionManager("radDB");
            try
            {
                DataSet ds = con.ExecuteQuery(query, RQueryType.Select);
                return JsonConvert.SerializeObject(ds);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(con);
            }
        }

        /// <summary>
        /// To Display The DeadLock Graph
        /// </summary>
        /// <returns></returns>
        public string GetDeadLockGraphData(string deadLockTime, string dateFormat)
        {
            mLogger.Debug("CommonService -> GetDeadLockGraphData -> Start");
            DateTime? lockDate = null;
            if (!string.IsNullOrEmpty(deadLockTime))
            {
                lockDate = DateTime.ParseExact(deadLockTime, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                string json = "";
                string query = "SELECT * FROM ivp_refm_deadlock where DeadLockTimeStamp = '" + lockDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' ";
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                DataTable dt = ds.Tables[0];
                List<Process> Processes = new List<Process>();
                Process pr;
                List<Resource> Resources = new List<Resource>();
                Resource rs;
                if (ds != null && ds.Tables.Count > 0 && dt != null && dt.Rows.Count > 0)
                {
                    XElement deadlockgraphRoot = XElement.Parse(Convert.ToString(dt.Rows[0]["DeadLockXMLData"]));
                    var dl = deadlockgraphRoot.Element("data").Element("value").Element("deadlock");
                    var processList = dl.Element("process-list");
                    var resourceList = dl.Element("resource-list");
                    var nodeCount = processList.Elements("process").Count();
                    var VictimId = dl.Element("victim-list").Element("victimProcess").Attribute("id").Value;
                    foreach (var process in processList.Elements("process"))
                    {
                        pr = new Process();
                        pr.ProcessId = process.Attribute("id").Value;
                        pr.LogUsed = process.Attribute("logused").Value;
                        pr.OwnerId = process.Attribute("ownerId").Value;
                        pr.Priority = process.Attribute("priority").Value;
                        pr.LockMode = process.Attribute("lockMode").Value;
                        pr.WaitResource = process.Attribute("waitresource").Value;
                        pr.WaitTime = Convert.ToInt32(process.Attribute("waittime").Value);
                        pr.TransactionName = process.Attribute("transactionname").Value;
                        pr.LastTransacted = process.Attribute("lasttranstarted").Value;
                        pr.Xdes = process.Attribute("XDES").Value;
                        pr.SchedulerId = Convert.ToInt32(process.Attribute("schedulerid").Value);
                        pr.Kpid = Convert.ToInt32(process.Attribute("kpid").Value);
                        pr.Status = process.Attribute("status").Value;
                        pr.Spid = Convert.ToInt32(process.Attribute("spid").Value);
                        pr.Sbid = Convert.ToInt32(process.Attribute("sbid").Value);
                        pr.Ecid = Convert.ToInt32(process.Attribute("ecid").Value);
                        pr.Transcount = Convert.ToInt32(process.Attribute("trancount").Value);
                        pr.LastBatchStarted = process.Attribute("lastbatchstarted").Value;
                        pr.LastBatchCompleted = process.Attribute("lastbatchcompleted").Value;
                        pr.LastAttention = process.Attribute("lastattention").Value;
                        pr.IsolationLevel = process.Attribute("isolationlevel").Value;
                        pr.XactId = process.Attribute("xactid").Value;
                        pr.LockTimeOut = process.Attribute("lockTimeout").Value;
                        pr.nodeCount = processList.Elements("process").Count();
                        foreach (var input in process.Elements("inputbuf"))
                        {
                            pr.InputBuff = input.Value;
                        }

                        Processes.Add(pr);
                    }
                    foreach (var resourse in resourceList.Elements("keylock"))
                    {
                        rs = new Resource();
                        rs.HobtId = resourse.Attribute("hobtid").Value;
                        rs.IndexName = resourse.Attribute("indexname").Value;
                        rs.AssociatedObjectId = resourse.Attribute("associatedObjectId").Value;
                        rs.ObjectName = resourse.Attribute("objectname").Value;
                        rs.Id = resourse.Attribute("id").Value;
                        rs.Mode = resourse.Attribute("mode").Value;
                        var waiterList = resourse.Element("waiter-list");
                        foreach (var waiter in waiterList.Elements("waiter"))
                        {
                            rs.ProcessIdwaiter = waiter.Attribute("id").Value;
                        }
                        Resources.Add(rs);
                    }
                    json = JsonConvert.SerializeObject(new DeadLockResponseReceived { Processes = Processes, Resources = Resources });
                }
                return json;
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetDeadLockGraphData -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetDeadLockGraphData -> End");
            }
        }


        /// <summary>
        /// To Identify and Get The Deadlock Data
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dateFormat"></param>
        /// <param name="resultDateTimeFormat"></param>
        /// <returns></returns>
        public string GetDeadLockData(string startDate, string endDate, string dateFormat, string resultDateTimeFormat)
        {
            mLogger.Debug("CommonService -> GetDeadLockData -> Start");
            DateTime? dtStart = null;
            DateTime? dtEnd = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                dtStart = DateTime.ParseExact(startDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime dt = DateTime.ParseExact(endDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                dtEnd = dt.AddDays(1);
            }
            try
            {
                string json = "";
                string query = "SELECT * FROM ivp_refm_deadlock";

                if (dtStart != null || dtEnd != null)
                {
                    query += " where";
                    if (dtStart != null)
                    {
                        query += " DeadLockTimeStamp>= '" + dtStart.Value.ToString("yyyy-MM-dd") + "' ";
                    }
                    if (dtEnd != null)
                    {
                        if (dtStart != null)
                            query += " AND ";
                        query += " DeadLockTimeStamp<='" + dtEnd.Value.ToString("yyyy-MM-dd") + "' ";
                    }
                }
                query += " Order by DeadLockTimeStamp";

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                DataTable dt = ds.Tables[0];
                List<Deadlock> Data = new List<Deadlock>();
                Deadlock d;
                if (ds != null && ds.Tables.Count > 0 && dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        d = new Deadlock();
                        d.DeadLockTimeStamp = ((DateTime)row["DeadLockTimeStamp"]).ToString(resultDateTimeFormat);
                        d.DeadLockXMLData = row["DeadLockXMLData"].ToString();
                        Data.Add(d);
                    }
                    json = JsonConvert.SerializeObject(Data);
                }
                mLogger.Debug("CommonService -> GetDeadLockData -> End");
                return json;
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetDeadLockData -> Exception ->" + ex.ToString());
                throw ex;
            }
        }


        /// <summary>
        /// To Get the Activity Monitor Details
        /// </summary>
        public string GetActivityMonitorDetails()
        {
            mLogger.Debug("CommonService -> GetActivityMonitorDetails -> Start");
            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"DECLARE @Table TABLE(SPID INT,Status VARCHAR(MAX),LOGIN VARCHAR(MAX), HostName VARCHAR(MAX),BlkBy VARCHAR(MAX),DBName VARCHAR(MAX),Command VARCHAR(MAX),CPUTime INT,DiskIO INT,LastBatch VARCHAR(MAX),ProgramName VARCHAR(MAX),SPID_1 INT,REQUESTID INT)
                                                               INSERT INTO @Table EXEC sp_who2
                                                               SELECT t.*, sqltext.Text
                                                               FROM @Table t
                                                               INNER JOIN sys.dm_exec_requests r ON t.SPID = r.session_id
                                                               CROSS APPLY sys.dm_exec_sql_text(sql_handle) sqltext
                                                               WHERE t.Command <> 'Awaiting Command' ", ConnectionConstants.RefMaster_Connection);

            DataTable dt = new DataTable();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {

                dt.Columns.Add("SPID", typeof(int));
                dt.Columns.Add("Status", typeof(string));
                dt.Columns.Add("LOGIN", typeof(string));
                dt.Columns.Add("HostName", typeof(string));
                dt.Columns.Add("BlkBy", typeof(string));
                dt.Columns.Add("DBName", typeof(string));
                dt.Columns.Add("Command", typeof(string));
                dt.Columns.Add("CPUTime", typeof(int));
                dt.Columns.Add("DiskIO", typeof(int));
                dt.Columns.Add("LastBatch", typeof(string));
                dt.Columns.Add("ProgramName", typeof(string));
                dt.Columns.Add("SPID_1", typeof(int));
                dt.Columns.Add("REQUESTID", typeof(int));
                dt.Columns.Add("Text", typeof(string));
                foreach (DataRow dr in ds.Tables[0].AsEnumerable())
                {
                    dt.Rows.Add(dr["SPID"], dr["Status"], dr["LOGIN"], dr["HostName"], dr["BlkBy"],
                                    dr["DBName"], dr["Command"], dr["CPUTime"], dr["DiskIO"], dr["LastBatch"],
                                    dr["ProgramName"], dr["SPID_1"], dr["REQUESTID"], dr["Text"]);

                }
            }
            mLogger.Debug("CommonService -> GetActivityMonitorDetails -> End");
            return JsonConvert.SerializeObject(dt);
        }


        /// <summary>
        /// To Log the Most Recent expensive Queries
        /// </summary>
        public string GetExpensiveQuery(string startDate, string endDate, string dateFormat, string resultDateTimeFormat)
        {
            mLogger.Debug("CommonService -> GetExpensiveQuery -> Start");
            DateTime? dtStart = null;
            DateTime? dtEnd = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                dtStart = DateTime.ParseExact(startDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                dtEnd = DateTime.ParseExact(endDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
            try
            {
                string json = "";
                string query = "SELECT * FROM ivp_refm_recent_expensive_query";

                if (dtStart != null || dtEnd != null)
                {
                    query += " where";
                    if (dtStart != null)
                    {
                        query += " LastExecutionTime>= '" + dtStart.Value.ToString("yyyy-MM-dd") + "' ";
                    }
                    if (dtEnd != null)
                    {
                        if (dtStart != null)
                            query += " AND ";
                        query += " LastExecutionTime<='" + dtEnd.Value.ToString("yyyy-MM-dd") + "' ";
                    }
                }
                query += " Order by LastExecutionTime";

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                DataTable dt = ds.Tables[0];
                List<QueryStats> Data = new List<QueryStats>();
                QueryStats q;
                if (ds != null && ds.Tables.Count > 0 && dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        q = new QueryStats();
                        q.Id = Int64.Parse(row["Id"].ToString());
                        q.QueryText = row["QueryText"].ToString();
                        q.ExecutionCount = Int64.Parse(row["ExecutionCount"].ToString());
                        q.CreationTime = ((DateTime)row["CreationTime"]).ToString(resultDateTimeFormat);
                        q.LastExecutionTime = ((DateTime)row["LastExecutionTime"]).ToString(resultDateTimeFormat);
                        q.LastWorkerTime = Int64.Parse(row["LastWorkerTime"].ToString());
                        q.MinWorkerTime = Int64.Parse(row["MinWorkerTime"].ToString());
                        q.MaxWorkerTime = Int64.Parse(row["MaxWorkerTime"].ToString());
                        q.LastPhysicalReads = Int64.Parse(row["Last_PhysicalReads"].ToString());
                        q.MinPhysicalReads = Int64.Parse(row["Min_PhysicalReads"].ToString());
                        q.MaxPhysicalReads = Int64.Parse(row["Max_PhysicalReads"].ToString());
                        q.LastLogicalWrites = Int64.Parse(row["Last_LogicalWrites"].ToString());
                        q.MinLogicalWrites = Int64.Parse(row["Min_LogicalWrites"].ToString());
                        q.MaxLogicalWrites = Int64.Parse(row["Max_LogicalWrites"].ToString());
                        q.LastLogicalReads = Int64.Parse(row["Last_LogicalReads"].ToString());
                        q.MinLogicalReads = Int64.Parse(row["Min_LogicalReads"].ToString());
                        q.MaxLogicalReads = Int64.Parse(row["Max_LogicalReads"].ToString());
                        q.LastElapsedTime = Int64.Parse(row["Last_ElapsedTime"].ToString());
                        q.MinElapsedTime = Int64.Parse(row["Min_ElapsedTime"].ToString());
                        q.MaxElapsedTime = Int64.Parse(row["Max_ElapsedTime"].ToString());
                        q.QueryPlan = row["QueryPlan"].ToString();
                        Data.Add(q);
                    }
                    json = JsonConvert.SerializeObject(Data);
                }
                mLogger.Debug("CommonService -> GetExpensiveQuery -> End");
                return json;
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetExpensiveQuery -> Exception ->" + ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// To Get The Query Text Data
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string GetQueryTextData(long Id)
        {
            mLogger.Debug("CommonService -> GetQueryTextData -> Start");
            try
            {
                string json = "";
                string query = "SELECT QueryText FROM ivp_refm_recent_expensive_query where Id = " + Id;
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                DataTable dt = ds.Tables[0];
                List<QueryStats> qs = new List<QueryStats>();
                QueryStats qp;
                if (ds != null && ds.Tables.Count > 0 && dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        qp = new QueryStats();
                        qp.QueryText = row["QueryText"].ToString();
                        //qp.QueryPlan = row["QueryPlan"].ToString();
                        qs.Add(qp);
                    }
                    json = JsonConvert.SerializeObject(qs);
                }
                return json;
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetQueryTextData -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetQueryTextData -> End");
            }
        }


        public string DownloadQueryPlanFile(long Id, string QueryPlan, string LastExecutionTime)
        {
            mLogger.Debug("CommonService -> DownloadQueryPlanFile -> Start");
            try
            {
                string basePath = System.Web.Hosting.HostingEnvironment.MapPath("~/"); //@"C:\Downloads\\";
                string tempFileLocation = "SqlQueryPlanUtilityFiles\\" + "\\" + DateTime.Now.ToString("yyyy-MM-dd--hh--mm--ss") + "\\";
                // Regex FileDatePattern = new Regex("[/:]");
                //string FormatedDate= FileDatePattern.Replace(LastExecutionTime,"-");
                // LastExecutionTime = System.Text.RegularExpressions.Regex.Replace(LastExecutionTime,@"[/:]","-");
                LastExecutionTime = LastExecutionTime.Replace("/", "-").Replace(":", "--");
                string fileName = "ShowPlan" + "_" + LastExecutionTime;
                string filePath = SRMCommon.WriteSqlPlanFile(basePath + tempFileLocation, fileName, "sqlplan", QueryPlan);

                return filePath;
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> DownloadQueryPlanFile -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> DownloadQueryPlanFile -> End");
            }
        }

        public string DownloadDeadLockXML(string DeadLockTimeStamp, string DeadLockXMLData)
        {
            mLogger.Debug("CommonService -> DownloadDeadLockXML -> Start");
            try
            {
                string basePath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                string tempFileLocation = "DeadLockUtilityFiles\\" + "\\" + DateTime.Now.ToString("yyyy-MM-dd--hh--mm--ss") + "\\";
                DeadLockTimeStamp = DeadLockTimeStamp.Replace("/", "-").Replace(":", "--");
                string fileName = "DeadlockGraph" + DeadLockTimeStamp;

                string filePath = SRMCommon.WriteXDLFile(basePath + tempFileLocation, fileName, "xml", DeadLockXMLData);

                return filePath;
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> DownloadDeadLockXML -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> DownloadDeadLockXML -> End");
            }
        }

        #endregion

        public List<VersionInfo> GetHotfixes(bool allDeployedVersions, int moduleId)
        {
            List<VersionInfo> versionInfos = new List<VersionInfo>();

            DataTable table = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[GetHotfixDetails] '{0}'", allDeployedVersions),
                ConnectionConstants.RefMaster_Connection).Tables[0];
            if (table.Rows.Count == 0 && allDeployedVersions)
                throw new Exception("Build information not present");
            if (table.Rows.Count == 0 && !allDeployedVersions)
                return null;
            VersionInfo vInfo = null;
            HotfixInfo hfInfo = null;
            if (allDeployedVersions)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (vInfo == null || vInfo.Version != row["Version"].ToString())
                    {
                        if (vInfo != null)
                            versionInfos.Add(vInfo);
                        vInfo = new VersionInfo();
                        vInfo.Version = row["Version"].ToString();
                        vInfo.SMVersion = row["SMVersion"].ToString();
                        vInfo.SMVersion = !vInfo.SMVersion.Trim().Contains("(") ? vInfo.SMVersion : vInfo.SMVersion.Insert(vInfo.SMVersion.IndexOf('('), " ");
                        vInfo.RMVersion = row["RMVersion"].ToString();
                        vInfo.RMVersion = !vInfo.RMVersion.Trim().Contains("(") ? vInfo.RMVersion : vInfo.RMVersion.Insert(vInfo.RMVersion.IndexOf('('), " ");
                        vInfo.RADVersion = row["RADVersion"].ToString().Split('(')[0];
                        vInfo.DeploymentDate = row["DeploymentDate"].ToString().Split(' ')[0];
                        vInfo.Hotfixes = new List<HotfixInfo>();
                    }
                    if (row["HotfixId"] == DBNull.Value)
                    {
                        vInfo.Hotfixes = null;
                        continue;
                    }
                    hfInfo = new HotfixInfo();
                    hfInfo.HotfixId = row["HotfixId"].ToString();
                    hfInfo.CaseId = row["CaseId"].ToString();
                    hfInfo.PublishDate = row["HFPublishDate"].ToString();
                    hfInfo.DeploymentDate = row["HFDeploymentDate"].ToString();
                    hfInfo.Version = row["Version"].ToString();
                    vInfo.Hotfixes.Add(hfInfo);
                }
                versionInfos.Add(vInfo);
            }
            else
            {
                vInfo = new VersionInfo();
                vInfo.Hotfixes = new List<HotfixInfo>();
                versionInfos.Add(vInfo);
                foreach (DataRow row in table.Rows)
                {
                    hfInfo = new HotfixInfo();
                    hfInfo.HotfixId = row["HotfixId"].ToString();
                    hfInfo.CaseId = row["CaseId"].ToString();
                    hfInfo.PublishDate = row["HFPublishDate"].ToString();
                    hfInfo.DeploymentDate = row["HFDeploymentDate"].ToString();
                    hfInfo.Version = row["Version"].ToString();
                    vInfo.Hotfixes.Add(hfInfo);
                }
            }
            return versionInfos;
        }

        public string GetUserGroupLayoutPriorityReport(string guid, string userName, string shortDateFormat, string gridDivId, int moduleId)
        {
            try
            {
                mLogger.Debug("CommonService -> GetUserGroupLayoutPriorityReport -> Start");
                //string q = "EXEC IVPSecMaster.dbo.SECM_GetUserGroupLayoutPriority";
                //q = "select * from ivpsecmaster.dbo.ivp_secm_sectype_master";

                //DataSet dataset = CommonDALWrapper.ExecuteSelectQuery(q, ConnectionConstants.SecMaster_Connection);
                //DataTable table = dataset.Tables[0].Copy();
                DataTable table;
                if (moduleId == 3)
                    table = com.ivp.common.SMCommonController.GetUserGroupLayoutPriority();
                else
                    table = com.ivp.common.RMCommonController.GetUserGroupLayoutPriority(moduleId);
                table.Columns.Add("row_id");
                int i = 0;
                foreach (DataRow row in table.Rows)
                    row["row_id"] = i++;

                Dictionary<string, string> colNameMap = new Dictionary<string, string>();
                i = 0;
                for (int colIndex = 0; colIndex < table.Columns.Count; colIndex++)
                {
                    if (table.Columns[colIndex].ColumnName != "row_id")
                        colNameMap.Add("a" + i++, table.Columns[colIndex].ColumnName);
                }
                mLogger.Debug("CommonService -> GetUserGroupLayoutPriorityReport -> Creating gridInfo");
                com.ivp.rad.controls.neogrid.service.RADNeoGridService service = new com.ivp.rad.controls.neogrid.service.RADNeoGridService();
                GridInfo gridInfo = new GridInfo();
                gridInfo.ViewKey = guid;
                gridInfo.CacheGriddata = true;
                gridInfo.GridId = gridDivId;
                gridInfo.CurrentPageId = guid;
                gridInfo.SessionIdentifier = guid;
                gridInfo.UserId = userName;
                List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo> columnsToHide = new List<com.ivp.rad.controls.neogrid.client.info.HiddenColumnInfo>();
                gridInfo.ColumnsToHide = columnsToHide;
                gridInfo.RequireEditGrid = false;
                gridInfo.IdColumnName = "row_id";
                //gridInfo.TableName = "overrideStatusTable";
                gridInfo.RequirePaging = true;
                gridInfo.RequireInfiniteScroll = true;
                gridInfo.DateFormat = shortDateFormat.ToUpper();
                gridInfo.RequireViews = false;
                gridInfo.RaiseGridCallBackBeforeExecute = "";
                //gridInfo.Height = "400px";
                gridInfo.TableName = "Table";
                gridInfo.PageSize = 200;
                gridInfo.RequirePaging = false;
                gridInfo.DoNotExpand = false;
                gridInfo.DoNotRearrangeColumn = true;
                gridInfo.RequireGrouping = true;
                gridInfo.RequireGroupExpandCollapse = true;
                gridInfo.RequireSelectedRows = true;
                gridInfo.RequireExportToExcel = true;
                gridInfo.RequireSearch = true;
                gridInfo.RequireResizing = true;
                gridInfo.RequireLayouts = false;
                gridInfo.RequireFilter = true;
                gridInfo.CssExportRows = "xlneoexportToExcel";

                mLogger.Debug("CommonService -> GetUserGroupLayoutPriorityReport -> Saving Datatable and gridInfo in cache");
                service.SaveGridDataInCache(table, userName, gridDivId, guid, guid, guid, false, gridInfo);
                return table.Rows.Count.ToString();
            }
            catch (Exception e)
            {
                mLogger.Error("CommonService -> GetUserGroupLayoutPriorityReport -> Error: "+e.ToString());
                throw e;
            }
            finally {
                mLogger.Debug("CommonService -> GetUserGroupLayoutPriorityReport -> Start");
            }
        }

        public void SaveUserScreenVisitInfo(string identifier, string url, string uniqueId, string uniqueText,string screenName,string userName)
        {
            mLogger.Debug("CommonService -> SaveUserScreenVisitInfo -> Start");

            //To avoid the second hit from the createTab method
            if(identifier==""&& url =="")
            {
                return;
            }

            StringBuilder screen = new StringBuilder();
            String ip = String.Empty;
            String _userName = String.Empty;
            StringBuilder module = new StringBuilder();

            screen.Append(screenName);
            if (identifier.Equals("ViewReport"))
            {
                screen.Clear();
                screen.Append("ViewReport - " + screenName);
                module.Append("SecMaster");
            }
            else if (identifier.Contains("CA") || identifier.Contains("Corp"))
            {
                module.Append("SecMaster");
                if (screenName == "" || screenName == null)
                {
                    screen.Append(identifier);
                }
            }
            else if (url.Contains("RefMasterLite") || url.Contains("RefMasterUI") ||  (
                !url.Contains("RefMasterUI") && !url.StartsWith("RMHomeInternal") && !url.Contains("UserAnalytics")))
            {
                module.Append("SecMaster");
            }
            else
            {
                module.Append("RefMaster");
            }

            if(screen.Length<1 )
            {
                screen.Append(identifier);
            }

            //Set the Ip Address
            ip = GetIPAddress();

            //Set the userName
            _userName = userName;

            mLogger.Debug("CommonService -> SaveUserScreenVisitInfo -> Creating rUserAnalyticsObject and saving the info in the DB");
            try
            {
                RUserAnalyticsController rUserAnalyticsController = new RUserAnalyticsController();
                RUserAnalyticsInfo userAnalyticsInfo = new RUserAnalyticsInfo();
                List<RUserAnalyticsInfo> userAnalyticsInfoList = new List<RUserAnalyticsInfo>();
                RCommon rCommon = new RCommon();
                RemoteEndpointMessageProperty oRemoteEndpointMessageProperty = (RemoteEndpointMessageProperty)OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name];

                userAnalyticsInfo.tab = screen.ToString();

                string clientName;
                if (SRMMTConfig.isMultiTenantEnabled)
                    clientName = SRMMTConfig.GetClientName();
                else
                    clientName = RConfigReader.GetConfigAppSettings("ClientName");


                if (SRMMTConfig.isMultiTenantEnabled)
                {
                    userAnalyticsInfo.product_name = SRMCommon.GetConfigFromDB("ProductName");
                }
                else
                {
                    userAnalyticsInfo.product_name = RConfigReader.GetConfigAppSettings("ProductName");
                }


                if (rCommon.SessionInfo != null)
                {
                    userAnalyticsInfo.user_id = rCommon.SessionInfo.LoginName;
                }
                if(HttpContext.Current.Session != null)
                {
                    userAnalyticsInfo.rad_session_id = HttpContext.Current.Session.SessionID;
                }  
                
                string clientMachine = string.Empty;
                 userAnalyticsInfo.client_ip = oRemoteEndpointMessageProperty.Address;//ip;
                try
                {
                    clientMachine = Dns.GetHostEntry(oRemoteEndpointMessageProperty.Address).HostName;
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                    clientMachine = ip;
                }

                string instanceName;
                if (SRMMTConfig.isMultiTenantEnabled)
                    instanceName = SRMCommon.GetConfigFromDB("InstanceName");
                else
                    instanceName = RConfigReader.GetConfigAppSettings("InstanceName");

                userAnalyticsInfo.client_name = clientName;
                userAnalyticsInfo.environment = instanceName;     
                userAnalyticsInfo.page = screen.ToString();
                userAnalyticsInfo.time_stamp = DateTime.Now.ToString(@"yyyy-MM-dd HH\:mm\:ss\.fff");
                userAnalyticsInfo.URL = url;
                userAnalyticsInfo.user_action_tag = null;
                userAnalyticsInfo.bundle_id = null;
                userAnalyticsInfoList.Add(userAnalyticsInfo);
                string radDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                rUserAnalyticsController.Save(userAnalyticsInfoList, radDBConnectionId);
            }
            catch (Exception e)
            {
                mLogger.Error(e.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("CommonService -> SaveUserScreenVisitInfo -> End");
            }

            return;               
        }


        [DataContract]
        public class VersionInfo
        {
            [DataMember]
            public string Version { get; set; }
            [DataMember]
            public string SMVersion { get; set; }
            [DataMember]
            public string RMVersion { get; set; }
            [DataMember]
            public string RADVersion { get; set; }
            [DataMember]
            public string DeploymentDate { get; set; }
            [DataMember]
            public List<HotfixInfo> Hotfixes { get; set; }
        }

        [DataContract]
        public class HotfixInfo
        {
            [DataMember]
            public string HotfixId { get; set; }
            [DataMember]
            public string CaseId { get; set; }
            [DataMember]
            public string Version { get; set; }
            [DataMember]
            public string PublishDate { get; set; }
            [DataMember]
            public string DeploymentDate { get; set; }
        }
        public enum SRMModules
        {
            AllSystems = -1,
            Securities = 3,
            RefData = 6,
            CorpAction = 9,
            Funds = 18,
            Parties = 20
        }

        public enum RMModules
        {
            AllSystems,
            RefData,
            Funds,
            Parties
        }

        public enum PMModules
        {
            AllSystems,
            Parties,
            Funds,
            RefData
        }
        public enum SMCommonStatusTypes
        {
            Any = 0,
            /// <summary>
            /// Success Staus
            /// </summary>
            Success = 1,
            /// <summary>
            /// Failed Status
            /// </summary>
            Failure = 2,
            /// <summary>
            /// INPROGRESS status
            /// </summary>
            In_Progress = 3,
            /// <summary>
            /// NOTPROCESSED status
            /// </summary>
            Not_Processed = 4,
            Queued = 5
        }
        public enum SMStatus
        {
            /// <summary>
            /// Success Staus
            /// </summary>
            Most_Recent = 1,
            /// <summary>
            /// Failed Status
            /// </summary>
            All = 2
        }
    }

    public class GridWriterService : BasePage, IGridWriterclientInterface
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("Common.GridWriterService");

        public List<CategoryAttribute> GetAttributesByCategory()
        {
            mLogger.Error("Start ==> GetAttributesByCategory");
            throw new NotImplementedException();
        }

        public RGridCustomInfo GetCustomGridInfo(RGridCustomInfo gridCustomInfo)
        {
            mLogger.Error("Start ==> GetCustomGridInfo");
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetDataPrivilegeMapping(string user, Dictionary<string, string> dataset)
        {
            mLogger.Error("Start ==> GetDataPrivilegeMapping");
            throw new NotImplementedException();
        }

        public Dictionary<string, Dictionary<string, List<string>>> GetDataPrivilegeMapping(string user, List<string> dataset)
        {
            mLogger.Error("Start ==> GetDataPrivilegeMapping");
            throw new NotImplementedException();
        }

        public string GetLeftMenuJson()
        {
            mLogger.Debug("Start ==> GetLeftMenuJson");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            LeftMenuStructure result = new LeftMenuStructure() { MenuItems = new List<LeftMenuItem>() };
            try
            {
                XElement MenuXml = null;

                string moduleId = Convert.ToString(Session["ModuleId"]);

                if (!string.IsNullOrEmpty(moduleId))
                {
                    if (moduleId.Equals("3"))
                        MenuXml = XElement.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\TopMenuXml\LeftMenu.xml"));
                    else if (moduleId.Equals("20"))
                        MenuXml = XElement.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\PMLeftMenu.xml"));
                    else if (moduleId.Equals("18"))
                        MenuXml = XElement.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\FMLeftMenu.xml"));
                    else if (moduleId.Equals("6"))
                        MenuXml = XElement.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\RMLeftMenu.xml"));
                }
                else
                    MenuXml = XElement.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\EMLeftMenu.xml"));

                IEnumerable<XElement> sections = MenuXml.Elements("section");
                foreach (XElement section in sections)
                {
                    LeftMenuItem item = createSubMenu(section);
                    if (item != null)
                        result.MenuItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("End ==> GetLeftMenuJson");
            }
            return serializer.Serialize(result);
        }

        private LeftMenuItem createSubMenu(XElement section)
        {
            IEnumerable<XElement> headers = section.Elements("header");
            IEnumerable<XElement> bodies = section.Elements("body");

            LeftMenuItem menuItem = new LeftMenuItem();
            if (headers.Count() > 0)
            {
                menuItem.Text = headers.FirstOrDefault().Value;
            }
            if (bodies.Count() > 0)
            {
                XElement body = bodies.FirstOrDefault();
                List<LeftMenuItem> lstChildren = new List<LeftMenuItem>();
                IEnumerable<XElement> sections = body.Elements("section");
                if (sections.Count() > 0)
                {
                    foreach (XElement sec in sections)
                    {
                        LeftMenuItem item = createSubMenu(sec);
                        if (item != null)
                            lstChildren.Add(item);
                    }
                }
                else
                {
                    IEnumerable<XElement> items = body.Elements("item");
                    items = items.Concat(body.Elements("Item"));
                    if (items.Count() > 0)
                    {
                        foreach (XElement item in items)
                        {
                            if (!string.IsNullOrEmpty(menuItem.Text))
                            {
                                LeftMenuItem submenuItem = new LeftMenuItem { Text = item.Attribute("name").Value, Href = item.Attribute("source").Value };

                                lstChildren.Add(submenuItem);
                            }
                            else
                            {
                                menuItem.Text = item.Attribute("name").Value;
                                menuItem.Href = item.Attribute("source").Value;
                            }
                        }
                    }
                }
                menuItem.Children = lstChildren;
            }
            if (string.IsNullOrEmpty(menuItem.Href) && menuItem.Children.Count == 0)
                return null;
            else
                return menuItem;
        }
    }

    public class SMDownstreamStatusInfo
    {
        [DataMember]
        public List<ConfigDetails> DownstreamStatusConfig { get; set; }
        [DataMember]
        public List<ExternalSystemDetails> ExternalSystems { get; set; }
        [DataMember]
        public List<TaskStatusDetails> TaskStatus { get; set; }
        public List<StatusDetails> Status { get; set; }
    }

    [DataContract]
    public class ConfigDetails
    {

        [DataMember]
        public string SelectedSystemNames { get; set; }

        [DataMember]
        public string SelectedTaskStatus { get; set; }

    }
    [DataContract]
    public class ExternalSystemDetails
    {
        [DataMember]
        public int ExternalSystemId { get; set; }
        [DataMember]
        public string ExternalSystemName { get; set; }
    }
    public class TaskStatusDetails
    {
        [DataMember]
        public int TaskStatusId { get; set; }
        [DataMember]
        public string TaskStatusName { get; set; }
    }
    public class StatusDetails
    {
        [DataMember]
        public int StatusId { get; set; }
        [DataMember]
        public string StatusName { get; set; }
    }
    //enum VendorManagementRequestType
    //{
    //    SAPI = 0,
    //    FTP = 1,
    //    GlobalAPI = 2,
    //    Heavy = 3
    //}

    class HeaderFormatter : IRADFormatter
    {
        public HeaderFormatter()
        {

        }
        private string _stringF;
        #region IRADFormatter Members

        public string FormatString
        {
            get
            {
                return "{0:C}";
            }
            set
            {
                _stringF = value;
            }
        }

        #endregion

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            //if (typeof(IRADFormatter).Equals(formatType)) return this;
            //return null;
            return this;
        }

        #endregion

        #region ICustomFormatter Members

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var rowData = Convert.ToString(arg).Split('|');
            string status = rowData[3];
            string guid = "s" + Guid.NewGuid().ToString().Replace('-', '_');
            return String.Format("<span class='workflowGridHeaderSection'><span class='workflowGridHeaderSecId secIdClick' id='{2}divSecurityId{0}' secId='{0}'>{0}</span><span class='workflowGridHeaderSecname'> : {4}</span> [ <span class='workflowGridHeaderSectype'>{1}</span> ]<span class='workflowGridHeaderRequestedOnText' style='color:#4C4C4C;'>Requested On: {3}</span></span>", rowData[1], rowData[2], guid, rowData[3], rowData[0]); //, rowData[4], ((rowData[4].Equals("Approved", StringComparison.OrdinalIgnoreCase)) ? "workflowGridHeaderApproved" : "workflowGridHeaderRejected"));<span class='workflowGridHeaderStatus {5}'>Request Status : {4}</span>
        }

        public object Formatter
        {
            get;
            set;
        }

        public object HiddenData
        {
            get;
            set;
        }

        public string ValueString
        {
            get;
            set;
        }

        public string rowAttributes
        {
            get;
            set;
        }

        public string ColumnName
        {
            get;
            set;
        }

        public Color BGColor
        {
            get;
            set;
        }

        public Color FGColor
        {
            get;
            set;
        }
        #endregion
    }

    class RMHeaderFormatter : IRADFormatter
    {
        public RMHeaderFormatter()
        {

        }
        private string _stringF;
        #region IRADFormatter Members

        public string FormatString
        {
            get
            {
                return "{0:C}";
            }
            set
            {
                _stringF = value;
            }
        }

        #endregion

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            //if (typeof(IRADFormatter).Equals(formatType)) return this;
            //return null;
            return this;
        }

        #endregion

        #region ICustomFormatter Members

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string groupingData = Convert.ToString(arg);
            var rowData = groupingData.Split('|');
            string status = rowData[3];
            string guid = "s" + Guid.NewGuid().ToString().Replace('-', '_');

            string cacheKey = rowData[rowData.Length - 3];
            string gridTypeStr = rowData[rowData.Length - 2];
            string gridId = rowData[rowData.Length - 1];
            WorkflowGridType gridType = (WorkflowGridType)Enum.Parse(typeof(WorkflowGridType), gridTypeStr);

            string queueIds = WorkflowStatusDataHandler.GetQueueIds(cacheKey, gridType, gridId, groupingData);

            return String.Format("<span class='workflowGridHeaderSection'><span class='workflowGridHeaderSecId secIdClick' id='{2}divSecurityId{0}' secId='{0}' queueIds='{5}' gridType='{6}'>{0}</span><span class='workflowGridHeaderSecname'> : {4}</span> [ <span class='workflowGridHeaderSectype'>{1}</span> ]<span class='workflowGridHeaderRequestedOnText' style='color:#4C4C4C;'>Requested On: {3}</span></span>", rowData[1], rowData[2], guid, rowData[3], rowData[0], queueIds, gridTypeStr); //, rowData[4], ((rowData[4].Equals("Approved", StringComparison.OrdinalIgnoreCase)) ? "workflowGridHeaderApproved" : "workflowGridHeaderRejected"));<span class='workflowGridHeaderStatus {5}'>Request Status : {4}</span>
        }

        public object Formatter
        {
            get;
            set;
        }

        public object HiddenData
        {
            get;
            set;
        }

        public string ValueString
        {
            get;
            set;
        }

        public string rowAttributes
        {
            get;
            set;
        }

        public string ColumnName
        {
            get;
            set;
        }

        public Color BGColor
        {
            get;
            set;
        }

        public Color FGColor
        {
            get;
            set;
        }
        #endregion
    }

    //class UniqueCompare : IEqualityComparer<DataRow>
    //{
    //    public bool Equals(DataRow x, DataRow y)
    //    {
    //        return x["Additional_Leg_ID"].Equals(y["Additional_Leg_ID"]);
    //    }

    //    public int GetHashCode(DataRow obj)
    //    {
    //        return base.GetHashCode();
    //    }

    //}
    public class ProcessInfo
    {
        public int processId { get; set; }
        public string processStatus { get; set; }
        public float CPUUsage { get; set; }
        public long memoryUsed { get; set; }
        public string userName { get; set; }
        public string processName { get; set; }
    }
    public class servicesInfo
    {
        public string serviceName { get; set; }
        public string path { get; set; }
        public string status { get; set; }
        public int processId { get; set; }
    }

    [DataContract]
    public class ServerFormat
    {
        [DataMember]
        public string longFormat;
        [DataMember]
        public string shortFormat;
        [DataMember]
        public string clientLongFormat;
        [DataMember]
        public string clientShortFormat;
        [DataMember]
        public string serverFormat;
    }

    public struct DownloadViewConfiguration
    {
        public string errorMessage;
        public string downloadLocation;
    }
}
