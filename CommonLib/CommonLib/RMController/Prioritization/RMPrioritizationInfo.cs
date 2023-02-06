using com.ivp.rad.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public class RMPrioritizationMigrationInfo
    {
        public string EntityDisplayName { get; set; }
        public int EntityTypeId { get; set; }
        public bool FirstPriorityVendorException { get; set; }
        public bool DeletePreviousException { get; set; }
        public bool AllVendorValueMissingException { get; set; }
        public bool RunCalculatedRules { get; set; }
        public ObjectRow row { get; set; }
        public List<RMEntityPrioritizationInfo> AttributeInfo { get; set; }

        public RMPrioritizationMapping MappingInfo{ get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMEntityPrioritizationInfo : RMBaseInfo
    {
        public int EntityAttributeId { get; set; }
        public List<RMDatasourcePriorityInfo> dspriorityInfoObjectArrayList { get; set; }
        public ObjectRow row { get; set; }
    }

    public class RMDatasourcePriorityInfo
    {
        public int Priority { get; set; }
        public int DataSourceId { get; set; }        
    }

    public class RMPrioritizationMapping
    {
        public int DataSouceId {get;set; }
        public int EntityAttributeId {get;set; }
        public bool IsInsert { get; set; }
        public ObjectRow row { get; set; }        
    }

    public class RMPrioritizationDataSourceMapping
    {
        public RMPrioritizationDataSourceMapping()
        {
            DataSources = new List<string>();
        }
        public string EntityTypeName { get; set; }
        public int EntityTypeId { get; set; }
        public List<string> DataSources { get; set; }
        public Dictionary<string,RMPrioritizationDataSourceMapping> Legs { get; set; }
    }
}
