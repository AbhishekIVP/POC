using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public class RMModelerConstants
    {
        public const string TABLE_START_NAME = "ivp_refmd_";
        public const string PARENT_ENTITY_TYPE_ID = "parent_entity_type_id";
        public const string ENTITY_TYPE_ID = "entity_type_id";
        public const string MAIN_ENTITY_TYPE_ID = "entity_type_id";
        public const string STRUCTURE_TYPE_ID = "structure_type_id";
        public const string ENTITY_GROUP_ID = "entity_group_id";
        public const string VISIBLE_GROUP_ID = "visible_outside_group";
        public const string DERIVED_FROM_ENTITY_TYPE = "derived_from_entity_type_id";
        public const string IS_ONE_TO_ONE = "is_one_to_one";
        public const string HAS_PARENT = "has_parent";
        public const string Attribute_Real_Name = "attribute_name";
        public const string Lookup_Type_Id = "lookup_entity_id";
        public const string Lookup_Attribute_Id = "lookup_attribute_id";
        public const string SECURITY_ID = "security_id";
        public const string SECURITY_ATTRIBUTE_ID = "security_attribute_id";
        public const string LOOKUP_DISP_ATTR = "lookup_display_attributes";
        public const string Attribute_lookup_identity = "attribute_lookup_id";
        public const string Lookup_Attribute_Real_Name = "lookup_attribute_real_name";

        public const string ENTITY_DISPLAY_NAME = "Entity Type Name";
        public const string LEG_REAL_NAME = "leg_real_name";
        public const string ENTITY_TYPE_NAME = "entity_type_name";
        public const string ENTITY_VIEW_NAME = "entity_view_name";
        public const string ENTITY_VIEW_NAME_REAL_COLUMN_NAME = "entity_view_name_real_column_name";
        public const string ENTITY_CODE = "entity_code";
        public const string ENTITY_GROUP_NAME = "entity_group_name";
        public const string VISIBLE_OUTSIDE_GROUP = "visible_outside_group";
        public const string CREATED_BY = "created_by";
        public const string LAST_MODIFIED_BY = "last_modified_by";
        public const string ACCOUNT_ID = "account_id";
        public const string CHILD_ENTITY_TYPE_NAME = "child_entity_type_name";
        public const string IS_VECTOR = "isvector";
        public const string IS_READ_ONLY = "is_read_only";
        public const string MASTER_ENTITY_TYPE_ID = "master_entity_type_id";
        public const string LEG_ENTITY_TYPE_ID = "leg_entity_type_id";
        public const string ALLOWED_USERS = "allowed_users";
        public const string ALLOWED_GROUPS = "allowed_groups";
        public const string USER_NAME = "userName";
        public const string MODULE_ID = "module_id";
        public const string ENTITY_ATTRIBUTE_ID = "entity_attribute_id";
        public const string UNIQUE_KEY_ID = "unique_key_id";
        //public const string LAYOUT_ID = "template_id";
        public const string TAB_ID = "tab_id";        
        public const string TAB_NAME_ID = "entity_tab_name_id";
        public const string CALCULATED_FIELD_RULE = "CALCULATED FIELD";
        public const string ALERT_RULE = "ALERT";
        public const string VALIDATION_RULE = "VALIDATION";
        public const string GROUP_VALIDATION_RULE = "GROUP VALIDATIONS";


        public const string TEMPLATE_ID = "template_id";
        public const string USER = "user";
        public const string GROUP = "group";
        public const string SYSTEM = "system";

        public const string PAGE_IDENTIFIER = "page_identifier";
        public const string FUNCTIONALITY_IDENTIFIER = "functionality_identifier";
        public const string ATTRIBUTE_IDS = "attribute_ids";
        public const string CONFIG_MASTER_ID = "config_master_id";

        #region static table names 

        public const string TABLE_DEFINITION = "Definition";
        public const string TABLE_MASTER_ATTIRBUTES = "Master Attributes";
        public const string TABLE_LEGS_CONFIGURATION = "Legs Configuration";
        public const string TABLE_LEG_ATTRIBUTES = "Leg Attributes";
        public const string TABLE_UNIQUE_KEYS = "Unique Keys";
        public const string TABLE_ATTRIBUTE_RULES = "Attribute Rules";
        public const string TABLE_BASKET_RULES = "Basket Rules";
        public const string TABLE_LAYOUTS = "Layouts";
        public const string TABLE_TAB_MANAGEMENT = "Tab Management";
        public const string TABLE_ATTRIBUTE_MANAGEMENT = "Attribute Management";
        public const string TABLE_LEG_ORDER = "Leg Order";
        public const string TABLE_ATTRIBUTE_CONFIGURATION = "Page Configuration";
        public const string TABLE_EXCEPTION_PREFERENCES = "Exception Preferences";
        public const string TABLE_ACTION_NOTIFICATIONS = "Action Notifications";
        public const string TABLE_USER_GROUP_LAYOUT_PRIORITY = "User Group Layout Priority";

        #endregion

        #region static column names

        //Definition, Master and Leg Sheets
        //public const string Entity_Type_Name = "Entity Type Name";
        public const string Tags = "Tags";
        public const string Allowed_Users = "Allowed Users";
        public const string Allowed_Groups = "Allowed Groups";

        public const string Attribute_Name = "Attribute Name";
        public const string Data_Type = "Data Type";
        public const string Attribute_Length = "Attribute Length";
        public const string Lookup_Type = "Lookup Type";
        public const string Lookup_Attribute = "Lookup Attribute";
        public const string Default_Value = "Default Value";
        public const string Search_View_Position = "Search View Position";
        public const string Is_Mandatory = "Is Mandatory";
        public const string Visible_In_Search = "Visible In Search";
        public const string Is_Cloneable = "Is Cloneable";
        public const string Is_Encrypted = "Is Encrypted";
        public const string Is_PII = "PII";
        public const string Restricted_Characters = "Restricted Characters";
        public const string Lookup_Display_Attributes = "Lookup Display Attributes";
        public const string Show_Entity_Code = "Show Entity Code";
        public const string Order_By_Attribute = "Order by Attribute";
        public const string Comma_Formatting = "Comma Formatting";
        public const string Show_Percentage = "Show Percentage";
        public const string Show_Multiple = "Show Multiple";
        public const string Order_By_Attribute_Id = "order_by_attribute_id";
        public const string Order_By_Attribute_Name = "order_by_attribute_name";
        public const string Display_Meta_Data = "Display meta-data info";
        public const string Attribute_Description = "Attribute Description";

        public const string Leg_Name = "Leg Name";
        public const string Is_Primary = "Is Primary";

        //Unique Keys Sheet
        public const string Level = "Level";
        public const string Key_Name = "Key Name";
        public const string Is_Across_Entities = "Is Across Entities";
        public const string Check_In_Drafts = "Check In Drafts";
        public const string Check_In_Workflows = "Check In Workflows";
        public const string Consider_Null_As_Unique = "Consider Null As Unique";

        //Rules Sheet
        public const string Rule_Type = "Rule Type";
        public const string Rule_Name = "Rule Name";
        public const string Rule_Text = "Rule Text";
        public const string Priority = "Priority";
        public const string Rule_State = "Rule State";
        public const string Rule_Type_ID = "rule_type_id";
        public const string Rule_ID = "rule_id";
        public const string Rule_Set_ID = "rule_set_id";

        //Layout, Tab and Attribute Mgmt Sheet
        public const string Layout_Name = "Layout Name";
        public const string Dependent_Name = "Dependent Name";
        public const string Layout_Type = "Layout Type";
        public const string Entity_States = "Entity States";

        public const string Tab_Name = "Tab Name";
        public const string Tab_Order = "Tab Order";

        public const string Panel = "Panel";
        public const string Order = "Order";
        public const string Is_Visible = "Is Visible";
        public const string Is_Read_Only = "Is Read Only";

        //Leg order sheet
        public const string Display_Order = "Display Order";

        //Page Configuration Sheet
        public const string Page_Identifier = "Page Identifier";
        public const string Functionality_Identifier = "Functionality Identifier";

        //Exception Preferences Sheet
        public const string Duplicates = "Duplicates";
        public const string VENDOR_MISMATCH = "Vendor Mismatch";
        public const string REF_DATA_MISSING = "Ref Data Missing";
        public const string NO_VENDOR_VALUE = "No Vendor Value";
        public const string VALUE_CHANGED = "Value Changed";
        public const string SHOW_AS_EXCEPTION = "Show As Exception";
        public const string VENDOR_VALUE_MISSING = "1st Vendor Value Missing";
        public const string VALIDATION = "Validations";
        public const string INVALID_DATA = "Invalid Data";
        public const string ALERTS = "Alerts";

        //Event action notifications sheets
        public const string Queues = "Queues";
        public const string Actions = "Requested Actions";
        public const string Action_Level = "Action Level";

        //Layout pref sheet
        public const string User_Name = "User Name";
        public const string Group_Level_Layout = "Group Level Layout";

        public static string TABLE_LEGS_ATTRIBUTEs { get; internal set; }
        #endregion

    }

}
