using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.srmdwhjob
{
    public class SRMDWHInfoClass
    {

    }

    public enum BLOCK_TYPES
    {
        [Description("Non Time Series")]
        NTS_REPORT = 1,
        [Description("Time Series")]
        TS_REPORT = 2,
        [Description("Daily")]
        DAILY_REPORT = 6,
        NTS_LOADER,
        TS_LOADER,
        DAILY_LOADER
    }

    [DataContract]
    public enum EXECUTION_STATUS
    {
        [Description("INPROGRESS")]
        INPROGRESS,

        [Description("FAILED")]
        FAILED,

        [Description("PASSED")]
        PASSED,

        [Description("NOT PROCESSED")]
        NOT_PROCESSED,

        [Description("QUEUED")]
        QUEUED,

        [Description("LOADER EXECUTION PENDING")]
        LOADER_EXECUTION_PENDING,

        [Description("LOADER EXECUTION INPROGRESS")]
        LOADER_EXECUTION_INPROGRESS
    }

    public class SRMJobInfo
    {
        public SRMJobInfo()
        {
            IsPassed = true;
            ModifiedEntities = new HashSet<string>();
            ModifiedEntitiesWithManualUpdates = new HashSet<string>();
            ModifiedEntitiesWithTSUpdates = new HashSet<string>();
            InstrumentsInTSWithTSUpdates = new HashSet<string>();
            InstrumentsInTSWithManualUpdate = new HashSet<string>();
        }
        public BLOCK_TYPES BlockType { get; set; }
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public int BlockId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Calendar { get; set; }
        public bool IsRef { get; set; }
        //public bool IsLeg { get; set; }
        public string DownstreamSQLConnectionName { get; set; }
        public int SetupId { get; set; }
        public string TableName { get; set; }
        public bool RequireBatching { get; set; }
        public int BatchSize { get; set; }
        public bool RequireKnowledgeDateReporting { get; set; }
        public bool RequireTimeTSReports { get; set; }
        public bool RequireDeletedRows { get; set; }
        public bool RequireReferenceMassagingOnStartDate { get; set; }
        public bool RequireReferenceMassagingOnCurrentKnowledgeDate { get; set; }
        public string CustomClassAssemblyName { get; set; }
        public string CustomClassName { get; set; }
        public string CustomClassMethodName { get; set; }
        public string QueueName { get; set; }
        public string FailureEmailId { get; set; }
        public HashSet<string> ReferenceAttributes { get; set; }
        public HashSet<string> ReferenceDailyAttributes { get; set; }
        public string SetupName { get; set; }
        public HashSet<string> ModifiedEntities { get; set; }
        public HashSet<string> ModifiedEntitiesWithTSUpdates { get; set; }
        public HashSet<string> ModifiedEntitiesWithManualUpdates { get; set; }
        public int SetupStatusId { get; set; }
        public int BlockStatusId { get; set; }
        public string UserName { get; set; }
        public string NonTimeSeriesReportName { get; set; }
        public string TimeSeriesReportName { get; set; }
        public int TimeSeriesReportId { get; set; }
        public string TimeSeriesTableNameWithSchema { get; set; }
        public DateTime? EffectiveStartDateForReport { get; set; }
        public DateTime LoadingTimeForStagingTable { get; set; }
        public DWHDateType DateType { get; set; }
        public bool FirstBatch { get; set; }
        public bool IsLegReport { get; set; }
        public string BackupTableName { get; set; }
        public HashSet<string> UnderlyingAttributes { get; set; }
        public bool IsPassed { get; set; }
        public DWHDateType TSReportDateType { get; set; }
        public HashSet<string> InstrumentsInTSWithManualUpdate { get; set; }
        public HashSet<string> InstrumentsInTSWithTSUpdates { get; set; }
        public int InstrumentTypeId { get; set; }
    }

    public enum DWHDateType
    {
        None,
        Today = 1,
        Yesterday = 2,
        LastBusinessDay = 3,
        T_Minus_N = 4,
        CustomDate = 5,
        Now = 6,
        FirstBusinessDayOfMonth = 7,
        FirstBusinessDayOfYear = 8,
        LastBusinessDayOfMonth = 9,
        LastBusinessDayOfYear = 10,
        LastBusinessDayOfPreviousMonth_Plus_N = 11,
        LastBusinessDayOfPreviousYear_Plus_N = 12,
        FirstBusinessDayOfMonth_Plus_N = 13,
        FirstBusinessDayOfYear_Plus_N = 14,
        LastExtractionDate = 100
    }

    [DataContract]
    public class SRMDWHInputInfo
    {
        [DataMember]
        public string SetupName { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public bool WaitForResponse { get; set; }
    }

    [DataContract]
    public class SRMDWHOutputInfo
    {
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public string FailureReason { get; set; }
    }

    [DataContract]
    public class SRMDWHSyncOutputInfo
    {
        [DataMember]
        public int SetupStatusId { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public string FailureReason { get; set; }
    }

    [DataContract]
    public class SRMDWHSyncStatus
    {
        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string StartTime { get; set; }

        [DataMember]
        public string EndTime { get; set; }

        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public string FailureReason { get; set; }
    }

    [DataContract]
    public class SRMDWHInternalInfo
    {
        //(int setupId, string userName, int setupStatusId, string blockExecuted, bool inTransaction, string errorMessage = "", string cacheKey = "")
        [DataMember]
        public int SetupId { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public int SetupStatusId { get; set; }

        [DataMember]
        public string BlockExecuted { get; set; }

        [DataMember]
        public bool InTransaction { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public string CacheKey { get; set; }

        [DataMember]
        public string SetupName { get; set; }

        [DataMember]
        public List<SRMDWHBlockStatus> BlockStatus { get; set; }
    }

    [DataContract]
    public class SRMDWHBlockStatus
    {
        [DataMember]
        public int BlockStatusId { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public EXECUTION_STATUS status { get; set; }

        [DataMember]
        public DateTime EndTime { get; set; }

        [DataMember]
        public int BlockId { get; set; }
    }

    public struct RollingSetupInfo
    {
        public int SetupId { get; set; }
        public string SetupName { get; set; }
        public string ConnectionName { get; set; }
        public DateTime? LastRollingTime { get; set; }
    }

    public struct RollingInfo
    {
        public int SetupId { get; set; }
        public string ConnectionName { get; set; }
        public bool T_plus_1 { get; set; }
        public DateTime TimeToRun { get; set; }

        public override string ToString()
        {
            return string.Format("SetupId : {0}, ConnectionName : {1}, T_plus_1 : {2}, TimeToRun : {3}", SetupId, ConnectionName, T_plus_1, TimeToRun);
        }
    }

    public struct DWHAdapterDetails
    {
        public int SetupId { get; set; }
        public int AdapterId { get; set; }
        public string AdapterName { get; set; }
        public string ClassName { get; set; }
        public string AssemblyPath { get; set; }
    }

    public struct DWHStatus
    {
        public int setupStatusId { get; set; }

        public string cacheKey { get; set; }

        public string userName { get; set; }
        public string errorMessage { get; set; }
    }
}
