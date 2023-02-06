using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;

namespace com.ivp.common
{
    public class WorkflowGridInput
    {
        public bool getAllRequests { get; set; }
        public bool getMyRequests { get; set; }
        public bool getRejectedRequests { get; set; }
        public bool getRequestsPending { get; set; }
        public string userName { get; set; }
        public string rejectedRequestsDivId { get; set; }
        public string requestsPendingDivId { get; set; }
        public string userSessionIdentifier { get; set; }
        public string longDateFormat { get; set; }
        public string shortDateFormat { get; set; }
        public string sectypeIds { get; set; }
        public string securityId { get; set; }
        public string selectedTab { get; set; }
    }

    public class WorkflowGridResetterInput
    {
        public string userName { get; set; }
        public string gridId { get; set; }
        public string cacheKey { get; set; }
        public string gridTypeStr { get; set; }
        public string userSessionIdentifier { get; set; }
        public bool isGrouped { get; set; }
        public string selectedTab { get; set; }
    }

    public static class WorkflowConstants
    {
        public const string STATUS_ID = "status_id";
        public const string QUEUE_ID = "queue_id";
        public const string IS_APPROVED = "is_approved";
        public const string IS_REJECTED = "is_rejected";
        public const string IS_DELETED = "is_deleted";
        public const string IS_SUPPRESSED = "is_suppressed";
        public const string ACTION_BY = "action_by";
        public const string ACTION_ON = "action_on";
        public const string REMARKS = "remarks";
        public const string WORKFLOW_SEQUENCE = "workflow_sequence";
        public const string IS_ACTIVE = "is_active";
    }

    public enum GridType
    {
        RGrid = 0,
        RADGrid = 1,
        RADXGrid = 2,
        RADXLGrid = 3,
        RADNEOGrid = 4
    }

    public enum WorkflowActionEnum
    {
        APPROVED = 0,
        REJECTED = 1,
        DELETED = 2,
        SUPPRESSED = 3,
        REVOKED = 4,
        REJECTEDTOLAST = 5,
        INITIAL_REQUEST = 6,
        INTERMEDIATE_REQUEST = 7
    }

    public enum WorkflowGridType
    {
        MYREQUESTS = 0,
        REJECTEDREQUESTS,
        REQUESTSPENDING,
        ALLREQUESTS
    }

    public class WorkflowMailInfo
    {
        //public string Sectype { set; get; }
        //public string SecurityName { set; get; }
        //public List<string> SecIds { get; set; }
        public string RequestedBy { set; get; }
        public DateTime RequestedTime { set; get; }
        public string RequestedByMailId { set; get; }
        public string ActionBy { set; get; }
        public DateTime ActionTime { set; get; }
        public string Comments { set; get; }
        public Dictionary<string, string> UserNameVsMailId { set; get; }
        public Dictionary<string, Dictionary<string, string>> GroupNameVsUserNameVsMailId { set; get; }
        public string Uri { set; get; }
        public string UpdateType { get; set; }
        public WorkflowActionEnum Action { get; set; }
        public DataTable TableInfo { get; set; }
    }

    public class WorkflowMailConfigInfo
    {
        public string[] MailSegment;
        public string FromEmailIdForWorkflow;
        public string NoOfRetry;
        public string NotificationFrequency;
        public string TransportName;
    }

    public class WorkflowDateRangePerAttribute
    {
        public bool ContainsNormalUpdate;
        public Dictionary<int, WorkflowDateRangeComparer> TimeSeriesRanges;
    }

    public class WorkflowDateRangeComparer
    {
        public WorkflowDateRange DateRange;
        public int LeftChild;
        public int RightChild;
        public bool HasLeftChild;
        public bool HasRightChild;
    }

    public class WorkflowDateRange
    {
        public DateTime? StartDate;
        public DateTime? EndDate;
    }

    [DataContract]
    public class WorkflowHandlerResponseInfo
    {
        [DataMember]
        public bool IsSuccess { set; get; }
        [DataMember]
        public string FailureMessage { set; get; }
        [DataMember]
        public List<string> secIds { set; get; }
    }

    [DataContract]
    public class WorkflowImpactedSecuritiesResponse
    {
        [DataMember]
        public string guid { set; get; }
        [DataMember]
        public bool IsActionComplete { set; get; }
        [DataMember]
        public bool IsImpactedCheck { set; get; }
        [DataMember]
        public string ImpactedHTML { set; get; }
        [DataMember]
        public List<int> statusIds { set; get; }
        [DataMember]
        public WorkflowHandlerResponseInfo handlerResponse { set; get; }
    }

    public class SMWorkflowSubscriptionInfo
    {
        private Dictionary<string, string> _mailToDic = new Dictionary<string, string>();
        private string _mailTo = string.Empty;
        private string _mailCc = string.Empty;
        private string _mailBcc = string.Empty;
        private string _mailFrom = string.Empty;
        private string _mailSubject = string.Empty;
        private string _mailBody = string.Empty;
        private bool _appendToMail = false;

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the mail to.
        /// </summary>
        /// <value>The mail to.</value>
        public string MailTo
        {
            get { return _mailTo; }
            set { _mailTo = value; }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the mail to.
        /// </summary>
        /// <value>The mail to.</value>
        public Dictionary<string, string> MailToDictionary
        {
            get { return _mailToDic; }
            set { _mailToDic = value; }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the mail cc.
        /// </summary>
        /// <value>The mail cc.</value>
        public string MailCc
        {
            get { return _mailCc; }
            set { _mailCc = value; }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the mail BCC.
        /// </summary>
        /// <value>The mail BCC.</value>
        public string MailBcc
        {
            get { return _mailBcc; }
            set { _mailBcc = value; }
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the custom mail info.
        /// </summary>
        /// <value>Custom mail Info.</value>
        //public SMCustomMailInfo CustomMailInfo
        //{
        //    get { return _objCustomMailInfo; }
        //    set { _objCustomMailInfo = value; }
        //}

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets if info contains customized e-mail info
        /// </summary>
        /// <value>true if custom mail info is given else false</value>
        //public bool IsCustomMail
        //{
        //    get { return _isCustomMail; }
        //    set { _isCustomMail = value; }

        //}


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets if cutomized info given is to be appended to the default mail
        /// </summary>
        /// <value>true if custom mail is to be appended to the default mail generated
        /// false if custom mail has to override the default generated mail
        /// </value>
        /// 
        public bool AppendToMail
        {
            get
            {
                return _appendToMail;
            }

            set { _appendToMail = value; }
        }

        //------------------------------------------------------------------------------------------
        // internal because we want these to be accessible only within files in the same assembly
        /// <summary>
        /// Gets or sets the mail from.
        /// </summary>
        /// <value>The mail from.</value>
        internal string MailFrom
        {
            get { return _mailFrom; }
            set { _mailFrom = value; }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the mail subject.
        /// </summary>
        /// <value>The mail subject.</value>
        internal string MailSubject
        {
            get { return _mailSubject; }
            set { _mailSubject = value; }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the mail body.
        /// </summary>
        /// <value>The mail body.</value>
        internal string MailBody
        {
            get { return _mailBody; }
            set { _mailBody = value; }
        }

    }

    public class SMWorkflowCachedInfo
    {
        public DataSet DsImpactedSecurities { get; set; }
        public DataSet DsAlreadyPassedSecurities { get; set; }
        public Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, string>>>> normalUserVssecTypeVsSecIdVsAttributeIdVsValue { get; set; }
        public Dictionary<string, Dictionary<int, Dictionary<DateTime, Dictionary<string, Dictionary<string, string>>>>> TSuserVssecTypeVsSecIdVsAttributeIdVsValue { get; set; }
        public Dictionary<string, Dictionary<int, Dictionary<KeyValuePair<DateTime, DateTime>, Dictionary<string, Dictionary<string, string>>>>> OnlyTSuserVssecTypeVsSecIdVsAttributeIdVsValue { get; set; }
        public Dictionary<string, Dictionary<int, List<SMSecurityWorkflowAttributes>>> UserVsLstRequestAttributes { get; set; }
    }

    public class SMWorkflowImpactedSecuritiesResponse
    {
        public Dictionary<int, List<SMSecurityWorkflowAttributes>> DictSectypeIdVsLstRequestAttributes { get; set; }
        public DataSet DsImpactedSecurities { get; set; }
    }
    public class SMSecurityWorkflowAttributes
    {
        public string SecId { get; set; }
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string DisplayName { get; set; }
        public string AttributeValue { get; set; }
        public string EffectiveFromDate { get; set; }
        public string EffectiveToDate { get; set; }
    }

    public class SMWorkflowSecuritiesValidationResponse
    {
        public DataTable ValidationResult { set; get; }
        public List<SMSecurityWorkflowAttributes> lstRaisedRequests { set; get; }
        public DataSet OtherInfo { set; get; }
    }

    public class RMFinalApprovalEntity
    {
        public string EntityCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> FailureReasons { get; set; }
        public bool hasAlert { get; set; }
        public string AttributeRealName { get; set; }
        public string AttributeDisplayName { get; set; }
        public string RuleName { get; set; }

        public RMFinalApprovalEntity()
        {
            hasAlert = false;
            AttributeRealName = string.Empty;
            AttributeDisplayName = string.Empty;
            RuleName = string.Empty;
        }
    }

    public class RMProcessedEntityInfo
    {
        public string EntityCode { get; set; }
        public int StatusID { get; set; }
        public bool isProcessed { get; set; }
        public RMProcessedEntityInfo()
        {
            StatusID = 0;
            isProcessed = false;
        }
    }
}
