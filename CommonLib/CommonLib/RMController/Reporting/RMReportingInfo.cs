using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.srmcommon;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace com.ivp.common.reporting
{
    #region ReportInfoForAPI

    [DataContract]
    public class EMReport
    {
        [DataMember]
        public string UserName;
        [DataMember]
        public EMReportRepositoryInfo Repository;
        [DataMember]
        public EMReportInfo Report;
        [DataMember]
        public List<EMRuleInfo> Rules;
        [DataMember]
        public List<EMReportMappingInfo> Mapping;
        [DataMember]
        public EMReportConfigurationInfo Configuration;
        [DataMember]
        public string DateFormat;
    }
    [DataContract]
    public class EMReportOutput
    {
        [DataMember]
        public bool IsSuccess;
        [DataMember]
        public List<string> Message;
    }

    [DataContract]
    public class EMReportRepositoryInfo
    {
        [DataMember]
        public string Name;
        [DataMember]
        public string Description;
        public int Id;
    }
    [DataContract]
    public class EMReportInfo
    {
        [DataMember]
        public string Name;
        [DataMember]
        public string Type;
        public int TypeId;
        public bool IsLegacy;
        public int Id;
    }
    [DataContract]
    public class EMReportConfigurationInfo
    {
        [DataMember]
        public string ReportHeader;
        [DataMember]
        public EMDateTypeInfo StartDate;
        [DataMember]
        public EMDateTypeInfo EndDate;
        [DataMember]
        public string Calendar;
        public int CalendarId;
        [DataMember]
        public bool IsFromToView;
        public bool ShowEntityCodes;
        public bool ShowDisplayNames;
        public bool IsMultiSheet;
        [DataMember]
        public List<EMReportAttributeConfigurationInfo> AttributeConfiguration;
    }
    [DataContract]
    public class EMReportMappingInfo
    {
        public int EntityTypeId;
        [DataMember]
        public string EntityType;
        [DataMember]
        public List<EMReportAttributeMappingInfo> Mapping;

    }
    [DataContract]
    public class EMReportAttributeMappingInfo
    {
        [DataMember]
        public EMReportAttributeInfo ReportAttribute;
        [DataMember]
        public string Attribute;
        [DataMember]
        public string LookupAttribute;
        public int AttributeId;
        public int LookupAttributeId;
        public string DataType;
        public string LookupDataType;
    }
    [DataContract]
    public class EMReportAttributeInfo
    {
        public int Id;
        [DataMember]
        public string Name;
        [DataMember]
        public string Description;
        [DataMember]
        public string DataType;
    }
    [DataContract]
    public class EMReportAttributeConfigurationInfo
    {
        public int AttributeId;
        public int LookupAttributeId;
        [DataMember]
        public string Attribute;
        [DataMember]
        public string LookupAttribute;
        [DataMember]
        public int ColumnWidth;
        [DataMember]
        public string Format;
        [DataMember]
        public int DisplayOrder;
        public string Parent;
        public string DataType;
    }

    public enum EMReportType
    {
        [Description("Entity Type")]
        ENTITY_TYPE_REPORT = 2,
        [Description("Entity Type Audit History Report")]
        ENTITY_TYPE_AUDIT_REPORT = 5,
        [Description("Across Entity Type")]
        ACROSS_ENTITY_TYPE_REPORT = 1,
        [Description("Across Entity Type Audit History Report")]
        ACROSS_ENTITY_TYPE_AUDIT_REPORT = 6
    }

    public enum EMReportRuleType
    {
        Transformation = 6,
        Filter = 7
    }

    #endregion
}
