using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.srmcommon
{
    [DataContract]
    public class SRFPMAttributeMetaDataInputObject
    {
        [DataMember]
        public int ModuleId { get; set; }
        [DataMember]
        public int TypeId { get; set; }
        [DataMember]
        public int AttributeId { get; set; }
        [DataMember]
        public bool IsAdditionalLeg { get; set; }
    }

    [DataContract]
    public class SRFPMAttributeMetaDataOutputObject
    {
        // Common Properties - SRFPM
        [DataMember]
        public string DataType { get; set; }
        [DataMember]
        public string DataLength { get; set; }
        [DataMember]
        public string Tags { get; set; }
        [DataMember]
        public bool IsCloneable { get; set; }
        [DataMember]
        public int IsPrimary { get; set; } // -1 for Non-Basket Attributes. 0 & 1 for Basket Attributes.
        [DataMember]
        public string ReferenceEntityTypeName { get; set; } // For Ref Lookup Attributes
        [DataMember]
        public string SecurityTypeName { get; set; } // For Sec Lookup Attributes
        [DataMember]
        public string AttributeDescription { get; set; }
        [DataMember]
        public string VendorPriority { get; set; }


        // Ref/Fund/Party Only Properties
        [DataMember]
        public string DefaultValue { get; set; }
        [DataMember]
        public string RestrictedCharacters { get; set; }
        [DataMember]
        public bool IsPII { get; set; }
        [DataMember]
        public bool IsEncrypted { get; set; }
    }

    [DataContract]
    public enum SRMModules
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
    [DataContract]
    public class ExceptionsConfig
    {
        [DataMember]
        public int moduleId;
        [DataMember]
        public int instrumentTypeId;
        [DataMember]
        public bool FirstVendorValueMissing;
        [DataMember]
        public bool Alert;
        [DataMember]
        public bool Duplicate;
        [DataMember]
        public bool InvalidData;
        [DataMember]
        public bool NoVendorValue;
        [DataMember]
        public bool RefDataMissing;
        [DataMember]
        public bool ShowAsException;
        [DataMember]
        public bool Validation;
        [DataMember]
        public bool VendorMismatch;
        [DataMember]
        public bool ValueChange;
        [DataMember]
        public bool UnderlierMissing;
    }
}
