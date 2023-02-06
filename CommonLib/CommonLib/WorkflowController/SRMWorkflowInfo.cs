using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using com.ivp.SRMCommonModules;

namespace com.ivp.common
{

    public class WFRequestInfo
    {
        public int WorkflowInstanceID { get; set; }
        public WorkflowType WorkflowType { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedOn { get; set; }
        public DateTime? EffectiveFromDate { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public int RadWorkFlowID { get; set; }

    }

    public class WorkflowInstrumentInfo
    {
        public int QueueID { get; set; }
        public string InstrumentID { get; set; }
        public int RadInstanceID { get; set; }
        public int RuleMappingID { get; set; }
    }

    public class WorkflowActionInfo
    {
        public WorkflowActionInfo()
        {
            isPassed = true;
            ErrorMessage = null;
            Exceptions = new List<WFApprovalExceptionInfo>();
            RuleConfigurations = new RuleConfigurations();
        }
        public int QueueID { get; set; }
        public string InstrumentID { get; set; }
        public int RadInstanceID { get; set; }
        public int RuleMappingID { get; set; }
        public string NextState { get; set; }
        public string CurrentState { get; set; }
        public bool isPassed { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public int TypeID { get; set; }
        public int WorkflowInstanceID { get; set; }
        public string RequestedBy { get; set; }
        public DateTime? RequestedOn { get; set; }
        public WorkflowType WorkflowType { get; set; }
        public string ClonedFrom { get; set; }
        public bool CopyTimeSeries { get; set; }
        public bool DeleteExisting { get; set; }
        public string TypeName { get; set; }
        public List<WFApprovalExceptionInfo> Exceptions { get; set; }
        public RuleConfigurations RuleConfigurations { get; set; }

        public string StateNameAndActionName { get; set; }

    }

    public class RuleConfigurations
    {
        public bool ExecuteTransformationRules { get; set; }
        public bool ExecuteMnemonicsRules { get; set; }
        public bool ExecuteValidationRules { get; set; }
        public bool ExecuteGroupValidationRules { get; set; }
        public bool ExecuteAlertRules { get; set; }
        public bool ExecuteRestrictedChars { get; set; }
        public bool ExecuteUniquenessCheck { get; set; }
        public bool ExecuteMandatoryCheck { get; set; }
        public bool ExecutePrimaryCheck { get; set; }
        public bool ExecuteBasketAlertRules { get; set; }
        public bool IsAnythingConfigured { get; set; }
    }

    public class WFStageRuleConfigurationInfo
    {
        public WFStageRuleConfigurationInfo()
        {
            ExecuteTransformationRules = true;
            ExecuteRestrictedChars = true;
            IsAnythingConfigured = true;
            ExecuteMnemonicsRules = true;
        }
        public int WorkflowInstanceID { get; set; }
        public int RadInstanceID { get; set; }
        public string InstrumentID { get; set; }
        public int RadWorkflowID { get; set; }
        public int RuleMappingID { get; set; }
        public int TypeID { get; set; }
        public int QueueID { get; set; }
        public string CurrentStage { get; set; }
        public string NextStage { get; set; }
        public bool ExecuteTransformationRules { get; set; }
        public bool ExecuteMnemonicsRules { get; set; }
        public bool ExecuteValidationRules { get; set; }
        public bool ExecuteGroupValidationRules { get; set; }
        public bool ExecuteAlertRules { get; set; }
        public bool ExecuteRestrictedChars { get; set; }
        public bool ExecuteUniquenessCheck { get; set; }
        public bool ExecuteMandatoryCheck { get; set; }
        public bool ExecutePrimaryCheck { get; set; }
        public bool ExecuteBasketAlertRules { get; set; }
        public bool IsAnythingConfigured { get; set; }
        public bool isDraft { get; set; }
    }

    public class WFApprovalExceptionInfo
    {
        public WFApprovalExceptionInfo()
        {
            isAlertOnly = false;
            ExceptionType = WorkflowExceptionType.NONE;
        }
        public string LegName { get; set; }
        public string AttributeName { get; set; }
        public string Exception { get; set; }
        public bool isAlertOnly { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public WorkflowExceptionType ExceptionType { get; set; }
        public UniquenessFailureInfo UniquenessInfo { get; set; }

    }

    public class SRMDuplicateInfo
    {
        public List<SRMDuplicateAttributeInfo> LstSaveDuplicateAttributesInKeyId { get; set; }
        public List<string> LstSaveDuplicateAttrValuesId { get; set; }

        public List<KeyValuePair<string, string>> LstDuplicateSecurityInfo { get; set; }

        public List<int> LstSaveDuplicateRowId { get; set; }
        public string DuplicateLegName { get; set; }
        public List<int> LstSavePrimaryAttr { get; set; }
        public List<int> SelectedSectypes { get; set; }
        public bool IsMaster { get; set; }
    }

    public class SRMDuplicateAttributeInfo
    {
        public string AttributeDisplayName { get; set; }
        public string AttributeValue { get; set; }
        public string AttributeDataType { get; set; }
    }

    public enum WorkflowExceptionType
    {
        NONE = -1,
        UNIQUENESS = 0,
        MANDATORY = 1,
        PRIMARY = 2,
        VALIDATION_RULE = 3,
        GROUP_VALIDATION_RULE = 4,
        ALERT_RULE = 5,
        RESTRICTED_CHARS = 6,
        NOT_PENDING = 7,
        UNHANDLED = 8
    }

    public class WorkflowActionOutput
    {
        public bool isPassed { get; set; }
        public string message { get; set; }
    }

    public class WFRetriggerInfo
    {
        public string InstrumentID { get; set; }
        public int RadInstanceID { get; set; }
        public int RadWorkflowID { get; set; }

        public int WorkflowInstanceId { get; set; }
    }

    public class SRMWorkflowResponse
    {
        public SRMWorkflowResponse()
        {
            objSRMEventsInfo = new List<SRMEventInfo>();
            InstrumentIDVsRADWorkflowInfo = new Dictionary<string, SRMWorkflowInstanceInfo>(StringComparer.OrdinalIgnoreCase);
            isPassed = true;
        }
        public bool isPassed { get; set; }
        public string message { get; set; }
        public bool isWorkflowConfigured { get; set; }
        public int WorkflowInstanceId { get; set; }
        public Dictionary<string, SRMWorkflowInstanceInfo> InstrumentIDVsRADWorkflowInfo { get; set; }
        public List<SRMEventInfo> objSRMEventsInfo { get; set; }
    }

    public class SRMWorkflowInstanceInfo
    {
        public int RuleMappingId { get; set; }
        public int RadWorkflowId { get; set; }
        public int RadWorkflowInstanceId { get; set; }
        public bool IsFinalApproval { get; set; }
        public bool IsWorkflowRaised { get; set; }
        public string State { get; set; }
        public WFStageRuleConfigurationInfo RuleConfigurations { get; set; }
    }

    public class WorkflowInitiatedInfo
    {
        public string InitiatedBy { get; set; }
        public DateTime InitiatedOn { get; set; }
    }

    public class WorkflowActionHistoryPerRequest
    {
        public int InstanceID { get; set; }
        public string RequestedBy { get; set; }
        public string RequestedOn { get; set; }
        public List<WorkflowActionHistory> Actions { get; set; }
    }

    public class WorkflowQueueData
    {
        public WorkflowQueueData()
        {
            WorkflowAttributes = new List<AttributeVsValue>();
            PossibleActions = new List<string>();
        }
        public int QueueId { get; set; }
        public int RadInstanceId { get; set; }
        public string InstrumentId { get; set; }
        public string TypeName { get; set; }
        public int TypeID { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public string RequestedBy { get; set; }
        public string RequestedByReal { get; set; }
        public DateTime RequestedOn { get; set; }
        public List<AttributeVsValue> WorkflowAttributes { get; set; }
        public List<string> PossibleActions { get; set; }

        public int WorkflowInstanceID { get; set; }
        public string ClonedFrom { get; set; }
        public bool DeleteExisting { get; set; }
        public bool CopyTimeSeries { get; set; }
    }

    public class AttributeVsValue
    {
        public int InstanceID { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public bool isPrimary { get; set; }
    }

    public class AttributeAudit
    {
        public string AttributeName { get; set; }
        public string TypeName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int InstanceID { get; set; }
        public int AuditLevel { get; set; }
        public string PrimaryAttribute { get; set; }
        public string LegID { get; set; }
        public string KnowledgeDate { get; set; }
        public string UserName { get; set; }
    }


    public class WorkflowQueueInfo
    {
        public WorkflowQueueInfo()
        {
            IsPassed = true;
            IsFinalApproval = false;
        }
        public bool IsPassed { get; set; }
        public string Message { get; set; }
        public string InstrumentID { get; set; }
        public int RadInstanceID { get; set; }
        public int WorkflowInstanceID { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedOn { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public int TypeID { get; set; }
        public string ClonedFrom { get; set; }
        public bool DeleteExisting { get; set; }
        public bool CopyTimeSeries { get; set; }
        public bool IsFinalApproval { get; set; }
        public int RuleMappingID { get; set; }
    }

    public class WorkflowInitiateResponse
    {
        public WorkflowInitiateResponse()
        {
            queueInfo = new List<WorkflowQueueInfo>();
            objSRMEventsInfo = new List<SRMEventInfo>();
        }
        public List<WorkflowQueueInfo> queueInfo { get; set; }
        public List<SRMEventInfo> objSRMEventsInfo { get; set; }
    }

    public class WorkflowRetriggerResponse
    {
        public WorkflowRetriggerResponse()
        {
            objSRMEventsInfo = new Dictionary<int, List<SRMEventInfo>>();
        }
        public bool isSuccess { get; set; }
        public Dictionary<int, List<SRMEventInfo>> objSRMEventsInfo { get; set; }
    }

    public class WorkflowTakeActionResponse
    {
        public WorkflowTakeActionResponse()
        {
            actionOutput = new List<WorkflowActionInfo>();
            objSRMEventsInfo = new Dictionary<int, List<SRMEventInfo>>();
        }
        public List<WorkflowActionInfo> actionOutput { get; set; }
        public Dictionary<int, List<SRMEventInfo>> objSRMEventsInfo { get; set; }
    }

    public class WorkflowDetailsInfo
    {
        public int typeId { get; set; }
        public string typeName { get; set; }
        public int moduleId { get; set; }
        public bool isCreate { get; set; }
        public int RadWorkflowId { get; set; }
        public int WorkflowInstanceId { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public string Initiator { get; set; }
    }

    public class WFInstrumentApprovalInfo
    {
        public WFInstrumentApprovalInfo()
        {
            isPassed = true;
            Exceptions = new List<WFApprovalExceptionInfo>();
        }

        public string InstrumentID { get; set; }
        public bool isPassed { get; set; }
        public string CurrentStage { get; set; }
        public List<WFApprovalExceptionInfo> Exceptions { get; set; }
    }

    public class WorkflowActionHistory
    {
        public WorkflowActionHistory()
        {

            this.Attributes = new List<AttributeAudit>();
        }

        public string UserName { get; set; }
        public string ActionDate { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string ActionTime { get; set; }
        public int ActionLevel { get; set; }
        public string LevelName { get; set; }
        public List<AttributeAudit> Attributes { get; set; }
        public int AuditLevel { get; set; }
        public DateTime ActualActionTime { get; set; }
        public DateTime AuditStartDate { get; set; }
        public DateTime AuditEndDate { get; set; }

    }

    public class WorkflowDivisionByModule
    {
        public int ModuleID { get; set; }
        public string InstrumentColumnName { get; set; }
    }




    [DataContract]
    public class SRMWorkflowCountPerType
    {
        public SRMWorkflowCountPerType()
        {
            pendingCount = 0;
            myRequestsCount = 0;
            rejectedCount = 0;
        }

        public SRMWorkflowCountPerType(string workflowType, int pendingCount, int myRequestsCount, int rejectedCount)
        {
            this.workflowType = workflowType;
            this.pendingCount = pendingCount;
            this.myRequestsCount = myRequestsCount;
            this.rejectedCount = rejectedCount;
        }

        [DataMember]
        public string workflowType { get; set; }

        [DataMember]
        public int pendingCount { get; set; }

        [DataMember]
        public int myRequestsCount { get; set; }

        [DataMember]
        public int rejectedCount { get; set; }
    }

    public class SRMWorklowRequestCountPerType
    {
        public SRMWorklowRequestCountPerType()
        {
            RequestCount = 0;
        }
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public int RequestCount { get; set; }

    }

    public class SRMWorkflowRequestsCounts
    {
        public WorkflowStatusType StatusType { get; set; }
        public int TotalRequestsCount { get; set; }
        public List<SRMWorklowRequestCountPerType> RequestsCountPerInstrumentType { get; set; }

        public SRMWorkflowRequestsCounts(WorkflowStatusType statusType)
        {
            StatusType = statusType;
            RequestsCountPerInstrumentType = new List<SRMWorklowRequestCountPerType>();
            TotalRequestsCount = 0;
        }

        public void AddToTypeCount(string typeName, int typeId, int count)
        {
            var req = RequestsCountPerInstrumentType.Find(r => r.TypeID == typeId);
            if (req != null)
            {
                req.RequestCount += count;
            }
            else
            {
                SRMWorklowRequestCountPerType rq = new SRMWorklowRequestCountPerType();
                rq.TypeID = typeId;
                rq.TypeName = typeName;
                rq.RequestCount = count;
                RequestsCountPerInstrumentType.Add(rq);
            }

            TotalRequestsCount += count;
        }
    }

    public class SRMWorkflowRequestsCountInfo
    {
        public int ModuleId { get; set; }
        public string Username { get; set; }
        public string DateFormat { get; set; }
        public List<WorkflowStatusType> StatusTypeCountsRequired { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SRMWorkflowRequestsCountInfo()
        {
            DateFormat = System.Globalization.CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
        }
    }

    [Serializable]
    public class UniquenessFailureInfo
    {
        public string UniqueKeyName { get; set; }

        public List<SMUniquenessFailureSecurityInfo> SecIds { get; set; }

        public Dictionary<string, string> AttributeNamesVsValues { get; set; }

        public Dictionary<string, string> AttributeNamesVsValuesMassaged { get; set; }

        public Dictionary<int, Dictionary<string, string>> DuplicateRows { get; set; }

        public string LegName { get; set; }

        public List<int> SelectedSectypes { get; set; }
        public bool IsMaster { get; set; }
        public Dictionary<string, string> AttributeNamesVsDataType { get; set; }
    }

    public enum SMUniquenessFailureType
    {
        GOLDEN = 0,
        WORKFLOW = 1,
        DRAFT = 2,
        USERDATA = 3
    }

    public class SMUniquenessFailureSecurityInfo
    {
        public string SecurityId { get; set; }

        public SMUniquenessFailureType FailureType { get; set; }
    }

    public enum WorkflowType
    {
        CREATE = 0,
        UPDATE = 1,
        ATTRIBUTE = 2,
        LEG = 3,
        DELETE = 4
    }

    public enum WorkflowAction
    {
        APPROVE = 0,
        REJECT = 1,
        EDIT = 2
    }

    public enum WorkflowStatusType
    {
        [Description("Pending At Me")]
        PENDING_AT_ME,
        [Description("My Requests")]
        MY_REQUESTS,
        [Description("Rejected Requests")]
        REJECTED_REQUESTS
    }
    public enum WFSRMModules
    {
        [EnumMember]
        AllSystems = -1,
        [EnumMember]
        Securities = 3,
        [EnumMember]
        RefData = 6,
        [EnumMember]
        CorpAction = 9,
        [EnumMember]
        Funds = 18,
        [EnumMember]
        Parties = 20
    }
    public class WorkflowsCountApiConstants
    {

        public const string PENDING_AT_ME = "Pending at me";
        public const string REJECTED_REQUESTS = "Rejected requests";
        public const string MY_REQUESTS = "My requests";
        public const string ALL_SYSTEMS_MODULE = "-1";
        public const string CORP_ACTION_MODULE = "9";
        public const string SECMASTER_MODULE_ID = "3";

    }
    [DataContract]
    public class WorkflowsCountInfo
    {
        [DataMember]
        public string UserName;
    }
    [DataContract]
    public class WorkflowsCountResponse
    {
        [DataMember]
        public List<WorkflowsCounts> WorkflowsCounts;
        [DataMember]
        public string FailureReason;
        [DataMember]
        public bool IsSuccess;
    }
    [DataContract]
    public class WorkflowsCounts
    {
        [DataMember]
        public string ModuleName;
        [DataMember]
        public List<WorkflowCountStateData> WorkflowCountsPerState;
    }
    [DataContract]
    public class WorkflowCountStateData
    {
        [DataMember]
        public string WorkflowState;
        [DataMember]
        public List<WorkflowTypeCountInfo> CountInfo;
    }
    [DataContract]
    public class WorkflowTypeCountInfo
    {
        [DataMember]
        public string WorkflowType;
        [DataMember]
        public int Count;
    }
}
