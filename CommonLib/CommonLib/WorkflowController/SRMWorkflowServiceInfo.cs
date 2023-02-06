using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    class SRMWorkflowServiceInfo
    {
    }

    public class SRMWorkflowRequest
    {
        [DataMember]
        public List<SRMWorkflowsRequests> WorkflowRequests;

        [DataMember]
        public string FailureReason;

        [DataMember]
        public bool IsSuccess;
    }

    [DataContract]
    public class SRMWorkflowsRequests
    {
        [DataMember]
        public string ModuleName;

        [DataMember]
        public List<SRMWorkflowRequestStateData> WorkflowCountsPerState;
    }

    [DataContract]
    public class SRMWorkflowRequestStateData
    {
        [DataMember]
        public string WorkflowState;

        [DataMember]
        public List<SRMWorkflowTypeRequestInfo> CountInfo;
    }
    [DataContract]
    public class SRMWorkflowTypeRequestInfo
    {
        [DataMember]
        public string WorkflowType;

        [DataMember]
        public int Count;
    }

    [DataContract]
    public class SRMWorkflowRequestDetailInfo
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string WorkflowState { get; set; }

        [DataMember]
        public string WorkflowType { get; set; }

        [DataMember]
        public List<string> IntrumentName { get; set; }

        [DataMember]
        public string DateFormat { get; set; }

        [DataMember]
        public string DateTimeFormat { get; set; }
    }

    [DataContract]
    public class SRMWorkflowRequestDetails
    {
        [DataMember]
        public string FailureReason;

        [DataMember]
        public bool IsSuccess;

        [DataMember]
        public List<SRMWorkflowRequestDetail> SRMWorkflowRequestDetail { get; set; }
    }

    [DataContract]
    public class SRMWorkflowRequestDetail
    {
        [DataMember]
        public string InstrumentId { get; set; }

        [DataMember]
        public string InstrumentName { get; set; }

        [DataMember]
        public string EffectiveFrom { get; set; }

        [DataMember]
        public string RequestedBy { get; set; }

        [DataMember]
        public string RequestedOn { get; set; }

        [DataMember]
        public List<string> ActionsAvailable { get; set; }

        [DataMember]
        public List<SMRWorkflowAttributeInfo> PrimaryAttribute { get; set; }

        [DataMember]
        public List<SMRWorkflowAttributeInfo> SecondaryAttribute { get; set; }
    }

    [DataContract]
    public class SMRWorkflowAttributeInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string DataType { get; set; }
    }

    [DataContract]
    public class SRMWorkflowActionInfo
    {
        [DataMember]
        public string InstrumentId { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public class SRMWorkflowAction
    {
        [DataMember]
        public string InstrumentId { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class SRMWorkflowInstrumentInfo
    {
        [DataMember]
        public string InstrumentId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string DateFormat { get; set; }
    }

    [DataContract]
    public class SRMWorkflowInstument
    {
        [DataMember]
        public string InstrumentId { get; set; }

        [DataMember]
        public List<SRMWorkflowInstrumentTabDetails> Tabs { get; set; }

        [DataMember]
        public List<SRMWorkflowInstrumentLegDetails> Legs { get; set; }

        [DataMember]
        public string FailureReason { get; set; }

        [DataMember]
        public bool IsSuccess { get; set; }
    }

    [DataContract]
    public class SRMWorkflowInstrumentLegDetails
    {
        [DataMember]
        public string LegName { get; set; }

        [DataMember]
        public List<SRMWorkflowInstrumentLegRowDetails> LegRows { get; set; }
    }


    [DataContract]
    public class SRMWorkflowInstrumentLegRowDetails
    {
        [DataMember]
        public string LegRowId { get; set; }

        [DataMember]
        public List<SMRWorkflowAttributeInfo> Attributes { get; set; }
    }

    [DataContract]
    public class SRMWorkflowInstrumentTabDetails
    {
        [DataMember]
        public string TabName { get; set; }

        [DataMember]
        public List<SRMWorkflowInstrumentSubTabDetails> SubTabs { get; set; }
    }

    [DataContract]
    public class SRMWorkflowInstrumentSubTabDetails
    {
        [DataMember]
        public string SubTabName { get; set; }

        [DataMember]
        public List<SMRWorkflowAttributeInfo> Attributes { get; set; }
    }

    [DataContract]
    public class SRMWorkflowActionHistoryRequestInfo
    {
        public SRMWorkflowActionHistoryRequestInfo()
        {
            this.InstrumentId = new List<string>();
        }

        [DataMember]
        public List<string> InstrumentId { get; set; }
        [DataMember]
        public string TypeName { get; set; }
        [DataMember]
        public string DateFormat { get; set; }
        [DataMember]
        public string DateTimeFormat { get; set; }
        [DataMember]
        public string TimeFormat { get; set; }
        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public class SRMWorkflowActionHistoryResponseInfo
    {
        public SRMWorkflowActionHistoryResponseInfo()
        {
            this.InstrumentWorkflowInfo = new List<SRMWorkflowActionHistoryInstrumentInfo>();
        }

        [DataMember]
        public List<SRMWorkflowActionHistoryInstrumentInfo> InstrumentWorkflowInfo { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }        
    }

    [DataContract]
    public class SRMWorkflowActionHistoryInstrumentInfo
    {
        [DataMember]
        public string InstrumentId { get; set; }
        [DataMember]
        public List<SRMWorkflowActionHistoryInstanceInfo> WorkflowInstances { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }

    }

    [DataContract]
    public class SRMWorkflowActionHistoryInstanceInfo
    {
        public SRMWorkflowActionHistoryInstanceInfo()
        {
            this.Actions = new List<SRMWorkflowActionHistoryActionInfo>();
        }

        [DataMember]
        public List<SRMWorkflowActionHistoryActionInfo> Actions { get; set; }
        [DataMember]
        public string RequestedBy { get; set; }
        [DataMember]
        public string RequestedOn { get; set; }
    }

    [DataContract]
    public class SRMWorkflowActionHistoryActionInfo
    {
        public SRMWorkflowActionHistoryActionInfo()
        {
            this.Attributes = new List<SRMWorkflowActionHistoryAttributeAuditInfo>();
        }

        [DataMember]
        public string ActionDate { get; set; }
        [DataMember]
        public string ActionTime { get; set; }
        [DataMember]
        public List<SRMWorkflowActionHistoryAttributeAuditInfo> Attributes { get; set; }
        [DataMember]
        public string WorkflowStage { get; set; }
        [DataMember]
        public string Remarks { get; set; }
        [DataMember]
        public string WorkflowAction { get; set; }
        [DataMember]
        public string ActionTakenBy { get; set; }
    }

    [DataContract]
    public class SRMWorkflowActionHistoryAttributeAuditInfo
    {
        [DataMember]
        public string AttributeName { get; set; }
        [DataMember]
        public string KnowledgeDate { get; set; }
        [DataMember]
        public string PrimaryAttribute { get; set; }
        [DataMember]
        public string NewValue { get; set; }
        [DataMember]
        public string OldValue { get; set; }
        [DataMember]
        public string LegName { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
    }

    [DataContract]
    public class SRMFileDataInfo
    {
        [DataMember]
        public string FileRealName { get; set; }
        [DataMember]
        public string FileDisplayName { get; set; }
        [DataMember]
        public byte[] FileData { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public string FailureReason { get; set; }
    }

    [DataContract]
    public class SRMFileDataRequestInfo
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string InstrumentId { get; set; }
        [DataMember]
        public string AttributeName { get; set; }        
    }
}