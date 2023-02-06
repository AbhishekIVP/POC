using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public class RMPrioritizationConstants
    {
        #region Static Table Names
        public const string TABLE_ENTITY_TYPE_CONFIG = "Entity Type";
        public const string TABLE_ATTRIBUTE_PRIORITIZATION = "Attributes";
        public const string TABLE_DATA_SOURCE_MERGING = "Data Source Merging";
        #endregion

        #region Static Column Names       
        public const string ENTITY_DISPLAY_NAME = "Entity Type Name";
        public const string ENTITY_TYPE_ID = "entity_type_id";
        public const string LEG_NAME = "Leg Name";
        public const string LEG_ENTITY_TYPE_ID = "leg_entity_type_id";
        public const string FIRST_PRIORITY_VENDOR_EXCEPTION = "First Priority Vendor Exception Configured";
        public const string DELETE_PREVIOUS_EXCEPTION = "Delete Previous Exception";
        public const string APPLY_UI_CALCULATED_RULES = "Run Calculated Rules";
        public const string NO_VENDOR_VALUE_EXCEPTION = "All Vendor value Missing Exception Configured";
        public const string UNIQUE_KEY_NAME = "Unique Key Name";
        public const string UNIQUE_KEY_ID = "unique_key_id";
        public const string ATTRIBUTE_ID = "entity_attribute_id";
        //public const string TAB_NAME = "Tab Name";
        public const string PRIORITY = "Priority";
        public const string DATA_SOURCE_NAME = "Data Source Name";
        public const string DATA_SOURCE_ID = "data_source_id";
        public const string IS_INSERT = "Is Insert";
        public const string ATTRIBUTE_DISPLAY_NAME = "Attribute Name";
        #endregion
    }
}
