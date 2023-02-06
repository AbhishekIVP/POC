using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public class RMModelerInfo
    {
    }

    public class RMEntityTypeInfo : RMBaseInfo
    {

        #region Properties
        [DataMember]
        public int EntityTypeID { get; set; }
        [DataMember]
        public int StructureTypeID { get; set; }
        //[DataMember]
        //public int EntityGroupID { get; set; }
        [DataMember]
        public bool VisibleOutsideGroup { get; set; }
        [DataMember]
        public int DerivedFromEntityTypeID { get; set; }
        [DataMember]
        public bool IsOneToOne { get; set; }
        [DataMember]
        public bool HasParent { get; set; }
        [DataMember]
        public string EntityTypeName { get; set; }
        [DataMember]
        public string EntityDisplayName { get; set; }
        [DataMember]
        public string EntityCode { get; set; }
        [DataMember]
        public int AccountId { get; set; }
        [DataMember]
        public string EntityTypeViewName { get; set; }
        [DataMember]
        public string EntityTypeViewNameWithRealColumnName { get; set; }
        [DataMember]
        public bool IsVector { get; set; }
        [DataMember]
        public string Tags { get; set; }
        [DataMember]
        public string AllowedUsers { get; set; }
        [DataMember]
        public string AllowedGroups { get; set; }
        [DataMember]
        public int ModuleID { get; set; }

        #endregion

        //public string LastModifiedBy { get; set; }
        //public string CreatedBy { get; set; }

    }

    public class RMEntityAttributeInfo : RMBaseInfo
    {        
        public int EntityAttributeID { get; set; }        
        public int EntityTypeID { get; set; }
        public string AttributeName { get; set; }        
        public string DisplayName { get; set; }        
        public bool IsNullable { get; set; }
        public RMDBDataTypes DataType { get; set; }
        public string DataTypeLength { get; set; }        
        public string DefaultValue { get; set; }        
        public bool IsUnique { get; set; }        
        public bool IsSearchView { get; set; }        
        public bool IsPrimary { get; set; }        
        public bool IsClonable { get; set; }        
        public int? SearchViewPosition { get; set; }        
        public bool IsInternal { get; set; }
        public int LookupEntityTypeID { get; set; }
        public int LookupAttributeID { get; set; }
        public string LookupEntityTypeName { get; set; }
        public string LookupAttributeName { get; set; }
        public string Tags { get; set; }
        public string RestrictedChars { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsPII { get; set; }
        public string LookupDisplayAttributes { get; set; }
        public string RealDataType { get; set; }
        public int AttributeLookupIdentityColumn { get; set; }

        public bool DisplayMetaData { get; set; }
        public string AttributeDescription { get; set; }
        //public bool IsActive { get; set; }        
        //public string CreatedBy { get; set; }        
        //public DateTime CreatedOn { get; set; }        
        //public string LastModifiedBy { get; set; }
        //public DateTime LastModifiedOn { get; set; }
    }

    public class RMEntityDetailsInfo
    {
        public int EntityTypeID { get; set; }
        public string EntityTypeName { get; set; }
        public string EntityDisplayName { get; set; }
        public Dictionary<string, RMEntityAttributeInfo> Attributes { get; set; }
        public Dictionary<string, RMEntityDetailsInfo> Legs { get; set; }
    }

    [DataContract]
    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMUniquenessSetupKeyInfo
    {
        public RMUniquenessSetupKeyInfo()
        {
            this.SelectedLeg = new RMUniquenessSetupLegInfo();
            this.SelectedAttributes = new List<RMUniquenessSetupAttributeInfo>();
            //this.SelectedLegAttributes = new List<RMUniquenessSetupAttributeInfo>();
        }

        [DataMember]
        public int EntityTypeID { get; set; }
        [DataMember]
        public int KeyID { get; set; }
        [DataMember]
        public string KeyName { get; set; }
        [DataMember]
        public bool IsLeg { get; set; }
        [DataMember]
        public bool IsAcrossEntities { get; set; }
        [DataMember]
        public RMUniquenessSetupLegInfo SelectedLeg { get; set; }
        [DataMember]
        public List<RMUniquenessSetupAttributeInfo> SelectedAttributes { get; set; }
        [DataMember]
        public bool CheckInDrafts { get; set; }
        [DataMember]
        public bool CheckInWorkflows { get; set; }

        [DataMember]
        public bool NullAsUnique { get; set; }
        //[DataMember]
        //public List<RMUniquenessSetupAttributeInfo> SelectedLegAttributes { get; set; }
    }

    [DataContract]
    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMUniquenessSetupLegInfo
    {
        [DataMember]
        public int LegID { get; set; }
        [DataMember]
        public string LegName { get; set; }
    }

    [DataContract]
    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMUniquenessSetupAttributeInfo
    {
        [DataMember]
        public int AttributeID { get; set; }
        [DataMember]
        public string AttributeName { get; set; }
    }

    //Layout info
    public class RMEntityTypeTemplateDetails
    {
        public string TemplateName { get; set; }
        public int TemplateId { get; set; }
        public Dictionary<string,RMEntityTabDetails> TabNameVsId { get; set; }

    }
    public class RMEntityTabDetails
    {
        public int tabNameId { get; set; }
        public int tabOrder { get; set; }
        public string tabName { get; set; }
    }

    public class RMTemplateInfo :RMBaseInfo
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public int EntityTypeId { get; set; }
        public int TemplateTypeId { get; set; }
        public string DependentId { get; set; }
        public string EntityTypeDisplayName { get; set; }
        public string EntityStates { get; set; }
    }

    public class RMTemplatePreferenceInfo : RMBaseInfo
    {
        public int TemplateId { get; set; }
        public string UserName { get; set; }
    }
    public enum action_type
    {
        insert = 1,
        update = 2,
        delete = 3
    }

    public class RMTabManagementInfo : RMBaseInfo
    {
        //public RMTabManagementInfo();

        public string TabAttributeXML { get; set; }
        public int TabEntityTypeId { get; set; }
        public int TabId { get; set; }
        public int TabIndex { get; set; }
        public string TabName { get; set; }
        public int TabNameId { get; set; }
        public int TemplateId { get; set; }
    }
    public class RMTableRefmTabManagement
    {
        public const string ENTITY_TAB_ID = "entity_tab_id";
        public const string ENTITY_TAB_NAME_ID = "entity_tab_name_id";
        public const string ENTITY_TAB_NAME = "entity_tab_name";
        public const string TAB_INDEX = "tab_index";
    }

    //Layout Attribute Management

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMAttributeManagementInfo : RMBaseInfo
    {
        //public RMAttributeManagementInfo();

        public int EntityTypeId { get; set; }
        public int TemplateId { get; set; }
        public string UIXml { get; set; }
    }

    //Page Configuration

    public class RMManageAttributeConfigurationInfo
    {
        //public RMManageAttributeConfigurationInfo();

        public string AttributeIds { get; set; }
        public string EntityTypeId { get; set; }
        public string FunctionalityIdentifier { get; set; }
        public string PageIdentifier { get; set; }
    }

    //Attribute display configuration
    public class RMAttributeDisplayConfigurationInfo
    {
        public int AttributeId { get; set; }
        public bool AppendPercentage { get; set; }
        public bool ShowEntityCode { get; set; }
        public int OrderByAttributeId { get; set; }
        public string OrderByAttributeName { get; set; }
        public bool AllowCommaFormatting { get; set; }
        public bool AppendMultiplier { get; set; }
    }
    public enum RMDBDataTypes
    {
        VARCHAR = 1,
        INT = 2,
        DECIMAL = 3,
        DATETIME = 4,
        BIT = 5,
        VARCHARMAX = 6,
        LOOKUP = 7,
        FILE = 8,
        LINK = 9,
        SECURITY_LOOKUP = 10
    }
}
