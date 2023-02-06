using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    /// <summary>
    /// Class having common info properties
    /// </summary>
    [DataContract]
    public abstract class RMBaseInfo
    {
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public string LastModifiedBy { get; set; }
        [DataMember]
        public DateTime LastModifiedOn { get; set; }
    }

    public class RMColumnInfo
    {
        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
        [DefaultValue(false)]
        public bool Nulable { get; set; }

        public RMDBDataTypes DataType { get; set; }

        public string Length { get; set; }

        public string DefaultValue { get; set; }
        [DefaultValue(false)]
        public bool IsIdentity { get; set; }

        [DefaultValue(false)]
        public bool InPrimaryKey { get; set; }

        public bool IsUnique { get; set; }
        [DefaultValue("")]
        public string Calculated { get; set; }

        [DefaultValue(false)]
        public bool IsExisting { get; set; }

        [DefaultValue(false)]
        public bool IsInternalLookup { get; set; }
    }

    public class RMCommonColumnInfo
    {
        public string ColumnName { get; set; }
        public Type DataType { get; set; }
        public object DefaultValue { get; set; }
    }

    public enum EMModule
    {
        AllSystems = 0,
        RefData = 6,
        Funds = 18,
        Parties = 20
    }

    public enum EMInputType
    {
        Id,
        DisplayName,
        RealName
    }

    public enum EMOutputType
    {
        Id,
        DisplayName,
        RealName
    }

    public enum EMDataType
    {
        DataSet,
        ObjectSet,
        DataTable,
        ObjectTable
    }

    public enum EMDateType
    {
        Both = 0,
        StartDate = 1,
        EndDate = 2
    }

    [DataContract]
    public class EMDateTypeInfo
    {
        [DataMember]
        public string Value;
        [DataMember]
        public string CustomValue;
        public int Id;
        [DefaultValue("MM/dd/yyyy")]
        public string DateFormat;
    }
    [DataContract]
    public class EMRuleInfo
    {
        [DataMember]
        public string Type;
        [DataMember]
        public string Name;
        [DataMember]
        public string Text;
        [DataMember]
        public int Priority;
        [DataMember]
        public bool State;
        public int TypeId;
    }

    public enum EMCallingInterface
    {
        UI = 0,
        API = 1,
        Sync = 2
    }
    
    /// <summary>
    /// Descriptions are rad rule type ids
    /// </summary>
    public enum RMRuleType
    {
        [Description("1")]
        UPSTREAM_VALIDATION = 1,
        [Description("2")]
        UPSTREAM_FILTER = 2,
        [Description("2")]
        UPSTREAM_API_FTP_FILTER = 3,
        [Description("4")]
        UPSTREAM_TRANSFORMATION = 4,
        [Description("4")]
        UPSTREAM_ENTITY_TRANSFORMATION = 5,
        [Description("4")]
        DOWNSTREAM_TRANSFORMATION = 6,
        [Description("2")]
        DOWNSTREAM_FILTER = 7,
        [Description("1")]
        MODELER_VALIDATION = 8,
        [Description("4")]
        MODELER_TRANSFORMATION = 9,
        [Description("10")]
        MODELER_GROUP_VALIDATION = 10,
        [Description("1")]
        MODELER_ALERT = 11,
    }

    public enum EMDateTypeEnum
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// Todays date. 
        /// Independent of Calendar
        /// </summary>
        Todays = 1,

        /// <summary>
        /// Yesterdays date. 
        /// Independent of Calendar.
        /// </summary>
        Yesterday = 2,

        /// <summary>
        /// Last business date.
        /// Dependent on Calendar
        /// </summary>
        LastBusinessDay = 3,

        /// <summary>
        /// Difference of business date and the number of days given by the user.
        /// Dependent on Calendar
        /// </summary>
        T_Minus_N = 4,

        /// <summary>
        /// Custom date in case user wants to provide the custom date format.
        /// Independent of Calendar.
        /// </summary>
        CustomDate = 5,

        /// <summary>
        /// Gets the current date and time.
        /// Independent of Calendar.
        /// </summary>
        Now = 6,

        /// <summary>
        /// Gets the starting business date of month.
        /// Dependent of Calendar.
        /// </summary>
        FirstBusinessDayOfMonth = 7,

        /// <summary>
        /// Gets the starting business date of year.
        /// Dependent of Calendar.
        /// </summary>
        FirstBusinessDayOfYear = 8,

        /// <summary>
        /// Gets the last business date of month.
        /// Dependent of Calendar.
        /// </summary>
        LastBusinessDayOfMonth = 9,

        /// <summary>
        /// Gets the last business date of year.
        /// Dependent of Calendar.
        /// </summary>
        LastBusinessDayOfYear = 10,

        /// <summary>
        /// Gets the last business date of year.
        /// Dependent of Calendar.
        /// </summary>
        LastBusinessDayOfPreviousMonth_Plus_N = 11,
        LastBusinessDayOfPreviousYear_Plus_N = 12,
        FirstBusinessDayOfMonth_Plus_N = 13,
        FirstBusinessDayOfYear_Plus_N = 14,


        /// <summary>
        /// Specifically added for Reporting
        /// </summary>
        LastExtractionDate = 100,
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMTimeSeriesTaskInfo : RMBaseInfo
    {
        public RMTimeSeriesTaskInfo()
        {
            RadFileInfo = new RADFilePropertyInfo();
        }

        public int TimeSeriesTaskID { get; set; }
        public int EntityTypeID { get; set; }
        public int UniqueAttributeId { get; set; }
        public RADFilePropertyInfo RadFileInfo { get; set; }
        public string DateFormat { get; set; }
    }

    public class EMEntityDataFilter
    {
        public string EntityType { get; set; }
        public HashSet<string> Attributes { get; set; }
        public string PrimaryAttributeName { get; set; }
        public HashSet<string> PrimaryAttributeValues { get; set; }
        public DataTable OutputData { get; set; }
        public HashSet<string> FinalPrimaryAttributeValues { get; set; }
    }

    public class EMEntityDataFilterForArchive
    {
        public string EntityType { get; set; }
        public HashSet<string> Attributes { get; set; }
        public string PrimaryAttributeName { get; set; }
        public DataTable OutputData { get; set; }
        public List<EMEntityArchiveDataInfo> AttributeValues { get; set; }
    }

    public class EMEntityArchiveDataInfo
    {
        public HashSet<string> PrimaryAttributeValues { get; set; }
        public DateTime KnowledgeDate { get; set; }
        public DateTime EffectiveDate { get; set; }
    }

    public class RMReconciledAttributeInfo
    {
        public int AttributeId { get; set; }
        public string AttributeRealName { get; set; }
        public string AttributeDisplayName { get; set; }
        public object DBValue { get; set; }
        public object NewValue { get; set; }
    }

    public class RMReconciledLegInfo
    {
        public int LegEntityTypeId { get; set; }
        public string LegDisplayName { get; set; }
        public string LegEntityTypeName { get; set; }
    }

    public class RMReconciledResponseInfo
    {
        public List<RMReconciledAttributeInfo> Attributes { get; set; }

        public List<RMReconciledLegInfo> Legs { get; set; }
    }

}
