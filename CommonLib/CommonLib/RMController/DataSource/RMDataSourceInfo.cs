using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public enum RMLicenseTaskType
    {
        Import = 0,
        Request = 1,
        Response = 2
    }

    public class RMLicenseKeyPropertyMapping
    {
        public string KeyName { get; set; }
        public string PropertyName { get; set; }
    }

    public class RMFeedInfoForMigration
    {
        public int FeedID { get; set; }
        public string FeedName { get; set; }
    }

    public class RMDataSourceInfoForMigration
    {
        public int DataSourceID { get; set; }
        public string DataSourceName { get; set; }
        public string DataSourceDescription { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMDatasourceInfo : RMBaseInfo
    {
        public int DatasourceID { get; set; }
        public string DatasourceName { get; set; }
        public string DatasourceDescription { get; set; }
        public int AccountID { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMFeedSummaryInfo : RMBaseInfo
    {
        public int FeedSummaryID { get; set; }
        public string FeedName { get; set; }
        public int DataSourceID { get; set; }
        public int FeedTypeID { get; set; }
        public int RadFileID { get; set; }
        public string DBProvider { get; set; }
        public string ConnectionString { get; set; }
        public string ColumnQuery { get; set; }
        public bool IsComplete { get; set; }
        public bool IsBulkLoaded { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RADFilePropertyInfo : RMBaseInfo
    {
        public int FileId { get; set; }
        public string FeedName { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string RowDelimiter { get; set; }
        public int RecordLength { get; set; }
        public char FieldDelimiter { get; set; }
        public string CommentChar { get; set; }
        public string SingleEscape { get; set; }
        public string PairedEscape { get; set; }
        public string RootXPath { get; set; }
        public string RecordXPath { get; set; }
        public string ExcludeRegEx { get; set; }
        public DateTime FileDate { get; set; }
        public int FieldCount { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RADFileFieldDetailsInfo : RMBaseInfo
    {
        public int FieldId { get; set; }
        public int FileId { get; set; }
        public string FieldName { get; set; }
        public string FieldDescription { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string FieldPosition { get; set; }
        public bool Mandatory { get; set; }
        public bool Persistency { get; set; }
        public bool Validation { get; set; }
        public bool AllowTrim { get; set; }
        public string FieldXPath { get; set; }
        public bool RemoveWhiteSpaces { get; set; }
        public bool Encrypted { get; set; }
        public bool IsPII { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMFeedFieldDetailsInfo : RMBaseInfo
    {
        public int FeedFieldDetailsId { get; set; }
        public int FeedSummaryId { get; set; }
        public int RadFieldId { get; set; }
        public bool IsBulk { get; set; }
        public bool IsFTP { get; set; }
        public bool IsAPI { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsUnique { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsPII { get; set; }

    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMFeedMappingInfo : RMBaseInfo
    {
        public int FeedMappingDetailId { get; set; }
        public int FeedSummaryId { get; set; }
        public int PrimaryColumnId { get; set; }
        public int MappedColumnId { get; set; }
        public int MapId { get; set; }
        public Boolean MapState { get; set; }
        public string PrimaryColumnName { get; set; }
        public string MappedColumnName { get; set; }
        public string MappingName { get; set; }
    }


    public class RMFeedInfoDetailed
    {
        public RMFeedInfoDetailed()
        {
            FeedFields = new List<RMFeedFieldInfo>();
            FieldAttributeMappings = new List<RMFeedEntityTypeMappingInfo>();
        }
        public string FeedName { get; set; }
        public int FeedID { get; set; }
        public int FileID { get; set; }
        public string FileName { get; set; }
        public int FileTypeID { get; set; }
        public int DataSourceID { get; set; }
        public List<RMFeedFieldInfo> FeedFields { get; set; }
        public List<RMFeedEntityTypeMappingInfo> FieldAttributeMappings { get; set; }
    }

    public class RMFeedFieldInfo
    {
        public int FieldID { get; set; }
        public string FieldName { get; set; }
        public int FeedFieldDetailsID { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class RMFeedEntityTypeMappingInfo
    {
        public string AttributeName { get; set; }
        public string FieldName { get; set; }
        public string LookupType { get; set; }
        public string LookupAttribute { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMEntityTypeFeedMapping : RMBaseInfo
    {
        public RMEntityTypeFeedMapping()
        {
            EntityAttributeFeedFieldMap = new List<RMEntityTypeFeedMappingDetailsInfo>();
        }
        public int EntityFeedMappingID { get; set; }
        public int FeedSummaryID { get; set; }
        public int EntityTypeID { get; set; }
        public bool ReplaceExisting { get; set; }
        public bool IsMasterUpdateOnly { get; set; }
        public List<RMEntityTypeFeedMappingDetailsInfo> EntityAttributeFeedFieldMap { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMEntityTypeFeedMappingDetailsInfo : RMBaseInfo
    {
        public int EntityTypeFeedMappingDetailsId { get; set; }
        public int EntityTypeFeedMappingId { get; set; }
        public int EntityAttributeId { get; set; }
        public int FieldId { get; set; }
        public string DataFormat { get; set; }
        public string UpdateBlank { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMEntityFeedAttributeLookup : RMBaseInfo
    {
        public int EntityFeedAttributeLookupId { get; set; }
        public int AttributeLookupId { get; set; }
        public int FeedSummaryId { get; set; }
        public string ParentEntityAttributeName { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public sealed class RMLoadTaskInfo : RMBaseInfo
    {
        public int FeedLoadingDetailsID { get; set; }
        public int TaskMasterID { get; set; }
        public int FeedSummaryID { get; set; }
        public DateTime LastRunDate { get; set; }
        public int BulkFileDateType { get; set; }
        public object BulkFileDate { get; set; }
        public int DiffFileDateType { get; set; }
        public object DiffFileDate { get; set; }
        public string BulkFilePath { get; set; }
        public string DifferenceFilePath { get; set; }
        public bool CustomCallExist { get; set; }
        public bool IsBulk { get; set; }
        public int DiffFileDateDays { get; set; }
        public int BulkFileDateDays { get; set; }
        public DateTime LastInstanceFileDate { get; set; }
        public string LastInstanceFileName { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMCustomClassInfo : RMBaseInfo
    {
        public int CustomClassId { get; set; }
        
        public string ClassName { get; set; }
        public string CallType { get; set; }
        public string AssemblyPath { get; set; }
        public string ClassType { get; set; }
        public int ExecSequence { get; set; }
        public int TaskMasterId { get; set; }
        public int TaskDetailsId { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMTaskInfo : RMBaseInfo
    {
        public int TaskMasterId { get; set; }
        public int TaskStatusId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int TaskTypeId { get; set; }
        public int DependentId { get; set; }
        public int ScheduledJobId { get; set; }
        public int TriggerType { get; set; }
        public int DependentPreTask { get; set; }
        public string LogDescription { get; set; }
        public string Status { get; set; }
        public int TaskInstanceID { get; set; }
        public string LoginName { get; set; }
        public int CalenderID { get; set; }
        public int FlowID { get; set; }

        public string Retry { get; set; }
        public int CTMStatusID { get; set; }

    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMLicenseSetupInfo : RMBaseInfo
    {
        public int LicenseFeedMappingId { get; set; }

        public int FeedSummaryId { get; set; }

        public int LicenseTypeId { get; set; }
    }

    public enum RMDynamicTableType
    {
        EntityType = 1,
        Feed = 2,
        HistoryEntityType = 3
    }
}
