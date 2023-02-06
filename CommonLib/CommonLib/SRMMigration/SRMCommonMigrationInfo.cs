using com.ivp.rad.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace com.ivp.srmcommon
{
    ////////////////////////////////////////////////////////
    // WRITE CLASSES TO POPULATE CONST STRINGS DATA BELOW //
    ////////////////////////////////////////////////////////

    // NOTE : Make a seperate region per featureName for clarity
    // NOTE : Each public const string is a Column Name (or Sheet Name) in that Sheet (or File)

    #region SM_Classes

    public class SM_SecurityTypeModeler_SheetNames
    {
        public const string Definition = "Definition";
        public const string Master_Attributes = "Master Attributes";
        public const string Baskets = "Baskets";
        public const string Basket_Attributes = "Basket Attributes";
        public const string Attribute_Level_Rules = "Attribute Level Rules";
        public const string Basket_Level_Rules = "Basket Level Rules";
        public const string Conditional_Filter = "Conditional Filter";
        public const string Defaults = "Defaults";
        public const string Layouts = "Layouts";
        public const string Layouts_Details = "Layouts Details";
        public const string Layouts_And_Permissions = "Layouts and Permissions";
        public const string User_Group_Layout_Priority = "User Group Layout Priority";
        public const string Exception_Preferences = "Exception Preferences";
        public const string Action_Notification = "Action Notification";
    }

    public class SM_CommonLegs_SheetNames
    {
        //public const string Common_Leg_Definition = "Definition";
        public const string Common_Leg_Attribute_Definition = "Attribute Definition";
        public const string Common_Leg_Templates = "Templates";
        public const string Common_Leg_Layout_and_Permissions = "Layout and Permissions";
        public const string Common_Leg_Security_Type_Mapping = "Layout Mapping";
        public const string Common_Leg_Attribute_Level_Rules = "Attribute Level Rules";
        public const string Common_Leg_Basket_Level_Rules = "Basket Level Rules";
        public const string User_Group_Layout_Priority = "User Group Layout Priority";
    }

    public class SM_LegsOrder_SheetNames
    {
        public const string Leg_Order = "Leg Order";
    }
    public class SM_CommonRules_SheetNames
    {
        public const string All_Common_Rules = "All Common Rules";
        public const string Configured_Security_Types = "Configured Sec Types";
    }

    public class SM_RealtimePreference_SheetNames
    {
        public const string PREFERENCE_SETUP = "Preference Setup";
        public const string PREFERENCE_DETAILS = "Preference Details";
        public const string BLOOMBERG_HEADERS = "Bloomberg Headers";
        public const string BASKET_PREFERENCE = "Basket Preference";
        public const string BASKET_PREFERENCE_MAPPING = "Basket Mapping";
        public const string UNDERLYER_DETECTOR_SETUP = "Underlyer Setup";
        public const string UNDERLYER_DETECTOR_RULES = "Underlyer Rules";
    }

    public class SM_DataSourcePrioritization_SheetNames
    {
        public const string SECURITY_TYPE = "Security Type";
        public const string ATTRIBUTES = "Attributes";
    }

    public class SM_ColumnNames
    {
        public const string Security_Type_Name = "Security Type Name";
        public const string Security_Type_Description = "Security Type Description";
        public const string Type = "Type";
        public const string Has_Master_Underlyer = "Has Master Underlyer";
        public const string Master_Underlyer_Sectype = "Master Underlyer Sectype";
        public const string Underlyer_Attribute = "Underlyer Attribute";
        public const string Tags = "Tags";
        public const string Creation_Date_Type = "Creation Date Type";
        public const string Creation_Date_Value = "Creation Date Value";
        public const string Allowed_Users = "Allowed Users";
        public const string Allowed_Groups = "Allowed Groups";
        public const string Attribute_Name = "Attribute Name";
        public const string Attribute_Description = "Attribute Description";
        public const string Data_Type = "Data Type";
        public const string Attribute_Length = "Attribute Length";
        public const string Reference_Type = "Reference Type";
        public const string Reference_Attribute = "Reference Attribute";
        public const string Reference_Display_Attribute = "Reference Display Attribute";
        public const string Reference_Leg_Attribute = "Reference Leg Attribute";
        public const string Is_Cloneable = "Is Cloneable";
        public const string Is_Mandatory = "Is Mandatory";
        public const string Leg_Name = "Leg Name";
        public const string Has_Constituent = "Has Constituent";
        public const string Constituent_Security_Type = "Constituent Security Type";
        public const string Primary_Attribute = "Primary Attribute";

        public const string Rule_Type = "Rule Type";
        public const string Attribute_Or_Leg_Name = "Attribute/Leg Name";
        public const string Rule_Group_Name = "Rule Group Name";
        public const string Rule_Name = "Rule Name";
        public const string Rule_Text = "Rule Text";
        public const string Priority = "Priority";
        public const string Rule_State = "Rule State";
        public const string Group_Name = "Group Name";
        public const string Attributes_In_Group = "Attributes In Group";

        public const string Default_Name = "Default Name";
        public const string Is_Default = "Is Default";
        public const string Description = "Description";
        public const string Default_Value = "Default Value";

        public const string Layout_Name = "Layout Name";
        public const string User_Or_Group = "User/Group";
        public const string Layout_Type = "Layout Type";

        public const string Tab_Name = "Tab Name";
        public const string Tab_Order = "Tab Order";
        public const string Sub_Tab_Name = "Sub Tab Name";
        public const string Sub_Tab_Order = "Sub Tab Order";

        public const string Leg_Or_Sub_Tab_Name = "Leg/Sub-Tab Name";
        public const string Panel = "Panel";
        public const string Order = "Order";
        public const string Is_Read_Only = "Is Read Only";
        public const string To_Show = "To Show";

        //public const string Common_Leg_Id = "Common Leg Id";
        public const string Multiple_Security_Types = "Security Type Name(s)";
        public const string Security_Or_Reference_Type = "Security/Reference Type";
        public const string Is_Primary = "Is Primary";

        public const string Template_Name = "Template Name";
        public const string Show_Created_On = "Show Created On";
        public const string Show_Created_By = "Show Created By";
        public const string Show_Last_Moodified_On = "Show Last Modified On";
        public const string Show_Last_Moodified_By = "Show Last Modified By";

        public const string Display_Order = "Display Order";

        public const string Can_Add = "Can Add";
        public const string Can_Update = "Can Update";
        public const string Can_Delete = "Can Delete";
        public const string Order_By_Clause = "Order By Clause";

        public const string Preference_Name = "Preference Name";
        public const string Preference_Description = "Preference Description";
        public const string Vendor_Type = "Vendor Type";
        public const string Request_Type = "Request Type";
        public const string Market_Sector = "Market Sector";
        public const string Data_Source = "Data Source";
        public const string Vendor_Identifier = "Vendor Identifier";
        public const string Sectype_Identifier = "Sectype Identifier";
        public const string Transport = "Transport";
        public const string Attribute_mapped_to_Market_Sector = "Attribute mapped to Market Sector";
        public const string Create_in_Background = "Create in Background";
        public const string Save_as_Draft = "Save as Draft";
        public const string Create_Underlier = "Create Underlier";
        public const string Create_constituent = "Create constituent";
        public const string Check_Existing = "Check Existing";
        public const string Check_Existing_attribute = "Check Existing attribute";
        public const string Security_Type = "Security Type";
        public const string Mapped_Bloomberg_Security_Types = "Mapped Bloomberg Security Types";
        public const string Prefered_Attributes = "Prefered Attributes";
        public const string Vendor_Management_Preference = "Vendor Management Preference";
        public const string Basket_Name = "Basket Name";
        public const string Header_Name = "Header Name";
        public const string Header_Value = "Header Value";
        public const string Exotic_field = "Exotic field";
        public const string Field_Name = "Field Name";
        public const string Mapped_Format = "Mapped Format";
        public const string Underlying_Attribute = "Underlying Attribute";
        public const string Is_Second_Fetch = "Is Second Fetch";
        public const string Fields_for_Second_Fetch = "Fields for Second Fetch";
        public const string Update_Blank = "Update Blank";

        public const string Chain_Name = "Chain Name";
        public const string Calendar_Name = "Calendar Name";
        public const string Recurrence_Type = "Recurrence Type";
        public const string Start_Date = "Start Date";
        public const string Recurrence_Pattern = "Recurrence Pattern";
        public const string Interval = "Interval";
        public const string End_Date = "End Date";
        public const string Number_of_Recurrences = "Number of Recurrences";
        public const string Start_Time = "Start Time";
        public const string Time_Interval_of_Recurrence = "Time Interval of Recurrence";
        public const string Never_End_Job = "Never End Job";
        public const string Days_Of_Week = "Days Of Week";
        public const string Task_Name = "Task Name";
        public const string Task_Type = "Task Type";
        public const string Module_Name = "Module Name";
        public const string Is_Muted = "Is Muted";
        public const string Proceed_On_Fail = "Proceed On Fail";
        public const string ReRun_on_Fail = "ReRun on Fail";
        public const string Retry_Duration = "Retry Duration";
        public const string Number_of_Retries = "Number of Retries";
        public const string On_Fail_Run_Task = "On Fail Run Task";
        public const string Time_Out = "Time Out";
        public const string Task_Instance_Wait = "Task Instance Wait";
        public const string Task_Wait_Subscription = "Task Wait Subscription";

        public const string On_Fail_Run_Task_Module_Name = "On Fail Run Task Module Name";
        public const string On_Fail_Run_Task_Task_Type = "On Fail Run Task Type";


        public const string Dependent_On_Module_Name = "Dependent On Module Name";
        public const string Dependent_On_Task_Type = "Dependent On Task Type";
        public const string Dependent_On_Task = "Dependent On Task";
        public const string Dependency_Relation = "Dependency Relation";
        public const string Subscription_Type = "Subscription Type";
        public const string To_Mail = "To Mail";
        public const string Subject = "Subject";
        public const string Body = "Body";

        public const string DATA_SOURCE = "Data Source";
        public const string DATA_SOURCE_DESCRIPTION = "Data Source Description";
        public const string DATA_SOURCE_TYPE = "Data Source Type";
        public const string FEED_NAME = "Feed Name";
        public const string FEED_SOURCE_TYPE = "Feed Source Type";
        public const string BLOOMBERG_LICENSE_CATEGORY = "Bloomberg License Category";
        public const string FILE_TYPE = "File Type";
        public const string RECORD_DELIMITER = "Record Delimiter";
        public const string FIELD_DELIMITER = "Field Delimiter";
        public const string COMMENT_CHAR = "Comment Char";
        public const string SINGLE_ESCAPE = "Single Escape";
        public const string PAIRED_ESCAPE = "Paired Escape";
        public const string EXCLUDE_REGEX = "Exclude Regex";
        public const string RECORD_LENGTH = "Record Length";
        public const string ROOT_XPATH = "Root XPath";
        public const string RECORD_XPATH = "Record XPath";
        public const string BLOOMBERG_BULK_FIELD = "Bloomberg Bulk Field";
        public const string SERVER_TYPE = "Server Type";
        public const string CONNECTION_STRING = "Connection String";
        public const string LOADING_CRITERIA = "Loading Criteria";
        public const string FIELD_NAME = "Field Name";
        public const string FIELD_DESCRIPTION = "Field Description";
        public const string IS_PRIMARY = "Is Primary";
        public const string START_INDEX = "Start Index";
        public const string END_INDEX = "End Index";
        public const string FIELD_POSITION = "Field Position";
        public const string FIELD_XPATH = "Field XPath";
        public const string MANDATORY = "Mandatory";
        public const string PERSISTABLE = "Persistable";
        public const string ALLOW_TRIM = "Allow Trim";
        public const string REMOVE_WHITE_SPACE = "Remove White Space";
        public const string IS_BLOOMBERG_MNEMONIC = "Is Bloomberg Mnemonic";
        public const string FOR_API = "For API";
        public const string FOR_FTP = "For FTP";
        public const string FOR_BULK = "For Bulk";
        public const string RULE_TYPE = "Rule Type";
        public const string RULE_NAME = "Rule Name";
        public const string RULE_TEXT = "Rule Text";
        public const string RULE_STATE = "Rule State";
        public const string PRIORITY = "Priority";
        public const string SECURITY_TYPE = "Security Type";
        public const string LEG_NAME = "Leg Name";
        public const string ATTRIBUTE_NAME = "Attribute Name";
        public const string REFERENCE_ATTRIBUTE = "Reference Attribute";
        public const string DATE_FORMAT = "Date Format";
        public const string LOADING_TASK_NAME = "Loading Task Name";
        public const string LOADING_TASK_DESCRIPTION = "Loading Task Description";
        public const string BULK_FILE_PATH = "Bulk File Path";
        public const string BULK_FILE_DATE_TYPE = "Bulk File Date Type";
        public const string BULK_FILE_DATE = "Bulk File Date";
        public const string BULK_FILE_BUSINESS_DAYS = "Bulk File Business Days";
        public const string DIFF_FILE_PATH = "Diff File Path";
        public const string DIFF_FILE_DATE_TYPE = "Diff File Date Type";
        public const string DIFF_FILE_DATE = "Diff File Date";
        public const string DIFF_FILE_BUSINESS_DAYS = "Diff File Business Days";
        public const string CHECK_EXISTING_SECURITY = "Check Existing Security";
        public const string CALL_TYPE = "Call Type";
        public const string CLASS_TYPE = "Class Type";
        public const string SCRIPT_CLASS_NAME = "Script-Class Name";
        public const string ASSEMBLY_PATH = "Assembly Path";
        public const string SEQUENCE_NUMBER = "Sequence Number";
        public const string IMPORT_TASK_NAME = "Import Task Name";
        public const string IMPORT_TASK_DESCRIPTION = "Import Task Description";
        public const string VENDOR_MANAGEMENT_PREFERENCE = "Vendor Management Preference";
        public const string REQUEST_TYPE = "Request Type";
        public const string VENDOR_TYPE = "Vendor Type";
        public const string VENDOR_IDENTIFIER = "Vendor Identifier";
        public const string VENDOR_IDENTIFIER_MAPPED_ATTRIBUTE = "Vendor Identifier Mapped Attribute";
        public const string MARKET_SECTOR = "Market Sector";
        public const string MARKET_SECTOR_MAPPED_ATTRIBUTE = "Market Sector Mapped Attribute";
        public const string REQUEST_TASK_NAME = "Request Task Name";
        public const string REQUEST_TASK_DESCRIPTION = "Request Task Description";
        public const string REQUEST_TRANSPORT_TYPE = "Request Transport Type";
        public const string OUTGOING_FTP = "Outgoing FTP";
        public const string BVAL_MAPPED_ATTRIBUTE = "BVAL Mapped Attribute";
        public const string RESPONSE_TASK_NAME = "Response Task Name";
        public const string RESPONSE_TASK_DESCRIPTION = "Response Task Description";
        public const string RESPONSE_TRANSPORT_TYPE = "Response Transport Type";
        public const string INCOMING_FTP = "Incoming FTP";
        public const string LICENSE_TYPE = "License Type";
        public const string HEADER_NAME = "Header Name";
        public const string HEADER_VALUE = "Header Value";
        public const string TASK_NAME = "Task Name";
        public const string TASK_DESCRIPTION = "Task Description";
        public const string APPLY_VALIDATION = "Apply Validation";
        public const string APPLY_UNIQUENESS = "Apply Uniqueness";
        public const string APPLY_ALERT = "Apply Alert";
        public const string DELETE_PREVIOUS_EXCEPTIONS = "Delete Previous Exceptions";
        public const string DELETE_PREVIOUS_EXCEPTIONS_CONSIDERED_SECURITIES = "Delete Previous Exceptions Of Considered Securities";
        public const string CALENDAR_NAME = "Calendar Name";
        public const string START_DATE_TYPE = "Start Date Type";
        public const string CUSTOM_START_DATE = "Custom Start Date";
        public const string START_DATE_BUSINESS_DAYS = "Start Date Business Days";
        public const string END_DATE_TYPE = "End Date Type";
        public const string CUSTOM_END_DATE = "Custom End Date";
        public const string END_DATE_BUSINESS_DAYS = "End Date Business Days";
        public const string UPDATE_BLANK = "Update Blank";

        public const string MASTER_DATA_SOURCE = "Master Data Source";
        public const string MASTER_FEED_NAME = "Master Feed Name";
        public const string MASTER_FEED_PRIMARY_FIELD = "Master Feed Primary Field";

        public const string SYSTEM_NAME = "System Name";
        public const string SYSTEM_SHORT_NAME = "Short Name";
        public const string CLASS_NAME = "Class Name";
        public const string VERSION = "Version";
        public const string AUDIT = "Audit";
        public const string REQUIRE_AUTOMATIC_POSTING_OF_UNDERLIER = "Require Automatic posting of Underlier/Constituents";
        public const string DEFAULT_SELECTED = "Default Selected";
        public const string GROUPS = "Groups";
        public const string USERS = "Users";
        public const string REPORT_NAME = "Report Name";
        public const string TRANSPORT_TYPE = "Transport Type";
        public const string REMOTE_FILE_NAME = "Remote File Name";
        public const string REMOTE_FILE_LOCATION = "Remote File Location";
        public const string LOCAL_FILE_NAME = "Local File Name";
        public const string LOCAL_FILE_LOCATION = "Local File Location";
        public const string USE_DEFAULT_PATH = "Use Default Path";
        public const string EXTRACT_ALL_FILES = "Extract All Files";
        public const string PGP_KEY_USERNAME = "PGP Key User Name";
        // public const string CALL_TYPE = "User Name";
        public const string PGP_KEY_PASSPHRASE = "PGP key PassPhrase";
        public const string FILE_DATE_TYPE = "File Date Type";
        public const string CUSTOM_VALUE_FILE_DATE_TYPE = "Custom Value File Date type";
        public const string STATE = "State";
        public const string SCRIPT_OR_CLASS_NAME = "Script/Class Name";
        public const string REQUIRE_REPORT_ATTRIBUTE_LEVEL_AUDIT = "Report Attribute Level Audit";

        public const string GPG_KEY_USERNAME = "GPG Key User Name";
        public const string GPG_KEY_PASSPHRASE = "GPG key PassPhrase";
        public const string REPORT_ATTRIBUTE = "Report Attribute";
        public const string ENTITY_TYPE_NAME = "Entity Type Name";
        public const string FILE_DATE_DAYS = "File Date Days";

        public const string Attribute_Type = "Attribute Type";

        public const string ENTITY_TYPES_FOR_CREATION = "Entity Types for Creation";
        public const string FIRST_PRIORITY_VENDOR_EXCEPTION_CONFIGURED = "First Priority Vendor Exception Configured";
        public const string DELETE_PREVIOUS_EXCEPTION = "Delete Previous Exception";
        public const string DELETE_PREVIOUS_EXCEPTION_CONSIDERED_SECURITIES = "Delete Previous Exception Of Considered Securities";
        public const string ALL_VENDOR_VALUE_MISSING_EXCEPTION_CONFIGURED = "All Vendor value Missing Exception Configured";
        public const string RUN_CALCULATED_RULES = "Run Calculated Rules";
        public const string FLUSH_AND_FILL_LEGS = "Flush and Fill Legs";
        public const string MERGE_LEGS = "Merge Legs";
        public const string OVERRIDE_CONSIDER_ALL_VENDORS = "Override Consider All Vendors";

        public const string Page_Identifier = "Page Identifier";
        public const string Functionality_Identifier = "Functionality Identifier";
        public const string Attribute = "Attribute";
        public const string Include_Security_Type = "Include Security Type";
        public const string Include_Security_Id = "Include Security Id";
        public const string Include_Last_Modified_By = "Include Last Modified By";
        public const string Include_Last_Modified_On = "Include Last Modified On";
        public const string Include_Created_By = "Include Created By";
        public const string Include_Created_On = "Include Created On";

        public const string REPORTS = "Reports";
        public const string REPORT_SYSTEM = "Report System";
        public const string AUDIT_LEVEL = "Audit Level";
        public const string CALENDAR = "Calendar";
        public const string POST_TO_DOWNSTREAM_SYSTEM = "Post To Downstream System";
        public const string SEND_REAL_TIME_UPDATES = "Send Real Time Updates";
        public const string SEND_REAL_TIME_UPDATES_IN_FLOW_TASK = "Send Real Time Updates In Flow Task";
        public const string PUBLICATION_QUEUES = "Publication Queues";
        public const string PUBLICATION_FORMAT = "Publication Format";
        public const string PUBLICATION_DELIMITER = "Publication Delimiter";
        public const string EXTRACTION_TRANSPORT_TYPE = "Extraction Transport Type";
        public const string EXTRACTION_REMOTE_FILE_LOCATION = "Extraction Remote File Location";
        public const string EXTRACTION_REMOTE_FILE_NAME = "Extraction Remote File Name";
        public const string EXTRACTION_REMOTE_FILE_FORMAT = "Extraction Remote File Format";
        public const string EXTRACTION_REMOTE_FILE_DELIMITER = "Extraction Remote File Delimiter";
        public const string EXTRACTION_REPORT_FILE_DATE_TYPE = "Extraction Report File Date Type";
        public const string EXTRACTION_REPORT_CUSTOM_FILE_DATE = "Extraction Report Custom File Date";
        public const string EXTRACTION_REPORT_FILE_DATE_BUSINESS_DAYS = "Extraction Report File Date Business Days";
        public const string EMAIL_IDS = "Email Ids";
        public const string EMAIL_TRANSPORT = "Email Transport";
        public const string EMAIL_FILE_FORMAT = "Email File Format";
        public const string EMAIL_FILE_DELIMITER = "Email File Delimiter";

        public const string EMAIL_REPORT_FILE_NAME = "Email Report File name";
        public const string EMAIL_REPORT_FILE_DATE_TYPE = "Email Report File Date Type";
        public const string EMAIL_REPORT_FILE_DATE_BUSINESS_DAYS = "Email Report File Date Business Days";
        public const string EMAIL_REPORT_CUSTOM_FILE_DATE = "Email Report Custom File Date";

        public const string ENCODING = "Encoding";
        public const string PROCESS_IN_BATCH = "Process In Batch";
        public const string BATCH_SIZE = "Batch Size";
        public const string IGNORE_SECURITY_KEY = "Ignore Security Key";
        public const string IGNORE_ARCHIVE_RECORDS = "Ignore Archive Records";
        public const string SHOW_ENTITY_CODE = "Show Entity Code";
        public const string COMMA_FORMATTING = "Comma Formatting";
        public const string SHOW_PERCENTAGE = "Show Percentage";
        public const string ORDER_BY_ATTRIBUTE = "Order by Attribute";
        public const string APPEND_MULTIPLIER = "Show Multiple";
        public const string Duplicates = "Duplicates";
        public const string VENDOR_MISMATCH = "Vendor Mismatch";
        public const string REF_DATA_MISSING = "Ref Data Missing";
        public const string NO_VENDOR_VALUE = "No Vendor Value";
        public const string VALUE_CHANGED = "Value Changed";
        public const string SHOW_AS_EXCEPTION = "Show As Exception";
        public const string VENDOR_VALUE_MISSING = "1st Vendor Value Missing";
        public const string VALIDATION = "Validations";
        public const string INVALID_DATA = "Invalid Data";
        public const string UNDERLIER_MISSING = "Underlier Missing";
        public const string ALERTS = "Alerts";
        public const string ACTION_LEVEL = "Action Level";
        public const string ACTIONS = "Requested Actions";
        public const string QUEUES = "QUEUES";
        public const string GROUP_LEVEL_LAYOUT = "Group Level Layout";
        public const string USER_NAME = "User Name";
    }

    public class SM_SecurityTypeType
    {
        public const string Vanilla_Structure = "Vanilla Structure";
        public const string Exotic_Structure = "Exotic Structure";
        public const string Vanilla_Structure_With_Leg = "Vanilla Structure With Leg";
    }

    public class SM_AttributesDataType
    {
        public const string String = "STRING";
        public const string Numeric = "NUMERIC";
        public const string Boolean = "BOOLEAN";
        public const string File = "FILE";
        public const string Date = "DATE";
        public const string DateTime = "DATETIME";
        public const string Time = "TIME";
        public const string Reference = "REFERENCE";
    }

    public class SM_CommonLegAttributesDataType
    {
        public const string String = "STRING";
        public const string Numeric = "NUMERIC";
        public const string Boolean = "BOOLEAN";
        public const string File = "FILE";
        public const string Date = "DATE";
        public const string DateTime = "DATETIME";
        public const string Time = "TIME";
        public const string Reference = "REFERENCE";
        public const string Security = "SECURITY";
        public const string Text = "TEXT";
    }

    public class SM_LegType
    {
        public const string SingleInfo = "Single";
        public const string MultiInfo = "Multiple";
        public const string Common = "Common";
    }

    public class SM_RuleTypes
    {
        public const string Mnemonic = "Mnemonic";
        public const string Calculated_Field = "Calculated Field";
        public const string Validation = "Validation";
        public const string Alert = "Alert";
        public const string Basket_Validation = "Basket Validation";
        public const string Basket_Alert = "Basket Alert";
        public const string Conditional_Filter = "Conditional Filter";
    }

    public class SM_Layout_Type
    {
        public const string System = "SYSTEM";
        public const string User = "USER";
        public const string Group = "GROUP";
    }

    public class SM_Layout_Panel
    {
        public const string Left = "LEFT";
        public const string Center = "CENTER";
        public const string Right = "RIGHT";
    }

    public class SRM_TaskManager_SheetNames
    {
        public const string Chain_Information = "Chain Information";
        public const string Task_Information = "Task Information";
        public const string Task_Dependency = "Task Dependency";
        public const string Chain_Subscription = "Chain Subscription";
        public const string Task_Subscription = "Task Subscription";
    }
    public class SM_DownstreamSystem_SheetNames
    {
        public const string Definition = "Definition";
        public const string Mapping = "Report Mapping";
    }
    public class SM_TransportTask_SheetNames
    {
        public const string Definition = "Definition";
        public const string CustomClasses = "Custom Classes";

    }

    public class SM_CommonAttributes_SheetNames
    {
        public const string Attribute_Definition = "Attribute Definition";
    }

    public class SM_CommonAttribute_AttributeType
    {
        public const string Identifiers = "Identifiers";
        public const string Security_Master_Information = "Security Master Information";
        public const string Reference_Data = "Reference Data";
    }

    public class SM_UniqueKeys_SheetNames
    {
        public const string Definition = "Definition";
        public const string Security_Types = "Security Types";
        public const string Legs = "Legs";
        public const string Attributes = "Attributes";
    }

    public class SM_UniqueKeys_ColumnNames
    {
        public const string Key_Name = "Key Name";
        public const string Level = "Level";
        public const string Is_Across_Securities = "Is Across Securities";
        public const string Check_In_Drafts = "Check In Drafts";
        public const string Check_In_Workflows = "Check In Workflows";
        public const string Consider_Null_As_Unique = "Consider Null As Unique";
        public const string Security_Type = "Security Type";
        public const string Attribute_Name = "Attribute Name";
        public const string Leg_Name = "Leg Name";
    }

    public class SM_UniqueKeys_LevelTypes
    {
        public const string Attribute_Level = "Attribute Level";
        public const string Leg_Level = "Leg Level";
    }

    public class SM_BooleanValues_YesNo
    {
        public const string Yes = "Yes";
        public const string No = "No";
    }

    public class SM_CommonConfiguration_SheetNames
    {
        public const string Configuration = "Configuration";
        public const string Advanced_Configuration = "Advanced Configuration";
    }

    public class SM_Downstream_Systems_Audit_Level
    {
        public const string Security_Level = "Security Level";
        public const string Report_Attribute_Level = "Report Attribute Level";
    }


    public class SM_DownstreamReports_SheetNames
    {
        public const string Report_Setup = "Report Setup";
        public const string Attribute_Mapping = "Attribute Mapping";
        public const string Report_Configuration = "Report Configuration";
        public const string Report_Rules = "Report Rules";
        public const string Report_Attribute_Order = "Report Attribute Order";
    }

    public class SM_DownstreamReports_ColumnNames
    {
        public const string Repository_Name = "Repository Name";
        public const string Repository_Description = "Repository Description";
        public const string Report_Name = "Report Name";
        public const string Report_Type = "Report Type";
        public const string Security_Type = "Security Type";
        public const string Insert_Text = "Insert Text";
        public const string Delete_Text = "Delete Text";
        public const string Update_Text = "Update Text";
        public const string Security_Type_SubInstruments_Legs = "Security Type/Sub-Instruments/Legs";
        public const string Attribute_Name = "Attribute Name";
        public const string Reference_Underlyer_Attribute_Name = "Reference/Underlyer Attribute Name";
        public const string Custom_Attribute_Name = "Custom Attribute Name";
        public const string Data_Type = "Data Type";
        public const string Report_Header = "Report Header";
        public const string MultiSheet_Report = "MultiSheet Report";
        public const string Expandable = "Expandable";
        public const string Show_Security_Type_Name = "Show Security Type Name";
        public const string Calender = "Calender";
        public const string Start_Date = "Start Date";
        public const string Custom_Value_Start_Date = "Custom Value Start Date";
        public const string End_Date = "End Date";
        public const string Custom_Value_End_Date = "Custom Value End Date";
        public const string Share_Report = "Share Report";
        public const string From_To_View = "From-To View";
        public const string Rule_Type = "Rule Type";
        public const string Rule_Name = "Rule Name";
        public const string Rule_Text = "Rule Text";
        public const string Priority = "Priority";
        public const string Rule_State = "Rule State";
        public const string Display_Order = "Display Order";
        public const string Column_Width = "Column Width";
        public const string Numeric_Format = "Numeric Format";
        public const string Rule_Set_ID = "rule_set_id";
    }

    public class SM_DownstreamReports_ReportType
    {
        public const string Multiple_Security_Type_Custom_Attributes = "Multiple Security Type Custom Attributes";
        public const string Multiple_Security_Type_Custom_Attributes_No_Audit = "Multiple Security Type Custom Attributes No Audit";
        public const string Attribute_Level_Audit_History_Report = "Attribute Level Audit History";
    }

    public class SM_DownstreamReports_DateType
    {
        public const string None = "None";
        public const string Todays = "Todays";
        public const string Yesterdays = "Yesterdays";
        public const string LastBusinessDays = "LastBusinessDays";
        public const string TminusN = "T-n";
        public const string Custom = "Custom";
        public const string Now = "Now";
        public const string FirstBusinessDayOfMonth = "FirstBusinessDayOfMonth";
        public const string FirstBusinessDayOfYear = "FirstBusinessDayOfYear";
        public const string LastBusinessDayOfMonth = "LastBusinessDayOfMonth";
        public const string LastBusinessDayOfYear = "LastBusinessDayOfYear";
        public const string LastExtractionDate = "LastExtractionDate";
        public const string LastSuccessfulPushTime = "LastSuccessfulPushTime";
    }

    public class SM_DownstreamReports_RuleType
    {
        public const string Filter_Rule = "Filter Rule";
        public const string Transformation_Rule = "Transformation Rule";
    }

    public class SM_DownstreamReports_Error
    {
        public const string Multiple_Report_Type_For_Same_Report = "Multiple Report Type For Same Report";
        public const string Report_Mapping_Does_Not_Exist_In_All_Sheets = "Incomplete report: Mapping does not exist in all sheets";
        public const string Invalid_End_Date_Custom = "'Custom Value End Date' can only be a DateTime with valid format as MM/DD/YYYY when 'End Date' is 'Custom'.";
        public const string Invalid_End_Date_TMINN = "'Custom Value End Date' can only be an Integer when 'End Date' is 'T_MINUS_N'.";
        public const string Invalid_Start_Date_Custom = "'Custom Value Start Date' can only be a DateTime with valid format as MM/DD/YYYY when 'Start Date' is 'Custom'.";
        public const string Invalid_Start_Date_TMINN = "'Custom Value Start Date' can only be an Integer when 'Start Date' is 'T_MINUS_N'.";
        public const string Rule_Name_Already_Exists = "Rule name already exists.";
        public const string Custom_Atribute_Name_And_DataType_Invalid_Error = "Custom attribute name or Data type is invalid for Attribute Level Audit History reports.";
        public const string Priority_Cannot_Be_Empty = "Priority cannot be empty.";
        public const string Priority_Should_be_Greater_Than_Zero = "Priority should be greater than zero.";
        public const string Incorrect_Priority = "Invalid priority";
        public const string Display_Order_Duplicate = "Display order mentioned multiple times for the same attribute.";
        public const string Display_Order_Should_be_Greater_Than_Zero = "Display order should be greater than zero.";
        public const string Display_Order_Can_Be_A_Integer_Only = "Display order can be a positive integer only.";
        public const string Multiple_Insert_Action_Identifier_Value_For_Same_Report = "Multiple insert action identifier value for same report";
        public const string Multiple_Delete_Action_Identifier_Value_For_Same_Report = "Multiple delete action identifier value for same report";
        public const string Multiple_Update_Action_Identifier_Value_For_Same_Report = "Multiple update action identifier value for same report";
        public const string Action_Identifier_Is_Invalid_For_Report = "Action identifier column values are invalid for this report";
        public const string Action_Identifier_Column_Cannot_Be_Empty_For_This_Report = "Action identifier column values cannot be empty for this report";

    }
    #endregion


    #region RM_Classes

    public class RM_EntityTypeModeler_SheetNames
    {
        public const string Definition = "Definition";
        public const string Master_Attributes = "Master Attributes";
        public const string Legs_Configuration = "Legs Configuration";
        public const string Leg_Attributes = "Leg Attributes";
        public const string Unique_Keys = "Unique Keys";
        public const string Attribute_Rules = "Attribute Rules";
        public const string Basket_Rules = "Basket Rules";
        public const string Layouts = "Layouts";
        public const string Tab_Management = "Tab Management";
        public const string Attribute_Management = "Attribute Management";
        public const string Leg_Order = "Leg Order";
        public const string Attribute_Configuration = "Page Configuration";
        public const string Exception_Preferences = "Exception Preferences";
        public const string Action_Notifications = "Action Notifications";
        public const string User_Group_Layout_Priority = "User Group Layout Priority";
    }

    public class RM_TimeSeriesTask_SheetNames
    {
        public const string Definition = "Definition";
    }

    public class RM_DataSource_SheetNames
    {
        public const string DataSourceAndFeedDefinition = "Data Sources";
        public const string Feed_Fields = "Feed Fields";
        public const string Feed_Mapping = "Feed Mapping";
        public const string Feed_Rules = "Feed Rules";
        public const string Entity_Type_Mapping = "Entity Type Mapping";
        public const string Entity_Type_Rules = "Entity Type Rules";
        public const string Bulk_License_Setup = "Bulk License Setup";
        public const string API_License_Setup = "API License Setup";
        public const string FTP_License_Setup = "FTP License Setup";
        public const string Custom_Classes = "Custom Classes";
    }

    public class SM_DataSource_SheetNames
    {
        public const string DATA_SOURCES = "Data Sources";
        public const string FEED_FIELDS = "Feed Fields";
        public const string FEED_RULES = "Feed Rules";
        public const string SECURITY_TYPE_MAPPING = "Security Type Mapping";
        public const string SECURITY_TYPE_RULES = "Security Type Rules";
        public const string BULK_LICENSE_SETUP = "Bulk License Setup";
        public const string BULK_LICENSE_CUSTOM_CLASS = "Custom Classes";
        public const string API_LICENSE_SETUP = "API License Setup";
        public const string FTP_LICENSE_SETUP = "FTP License Setup";
        public const string API_FTP_BLOOMBERG_HEADERS = "API-FTP BBG Headers";
        public const string LOAD_FROM_VENDOR_LICENSE_SETUP = "Load From Vendor Setup";
    }

    public class SM_ValidationTask_SheetNames
    {
        public const string DEFINITION = "Definition";
        public const string SECURITY_TYPE = "Security Type";
    }

    public class RM_Prioritization_SheetNames
    {
        public const string ENTITY_TYPE_CONFIGURATION = "Entity Type";
        public const string ATTRIBUTE_PRIORITIZATION = "Attributes";
        public const string DATA_SOURCE_MERGING = "Data Source Merging";
    }

    public class RM_RealTimePreference_SheetName
    {
        public const string PREFERENCE_SETUP = "Preference Setup";
    }

    public class RM_Migration_Constants
    {
        public const string Feed_Type_Manual = "Manual Setup";
        public const string Feed_Type_Load_From_DB = "Load From DB";
        public const string Feed_Type_File_Template = "File Template";

        public const string File_Type_Delimited = "Delimited";
        public const string File_Type_Excel = "Excel";
        public const string File_Type_XML = "Xml";
        public const string File_Type_DB = "DB";
        public const string File_Type_Fixed_Width = "Fixed Width";

        public const string Feed_Rule_Validation = "Feed Validation";
        public const string Feed_Rule_Transformation = "Feed Transformation";

        public const string Feed_Rule_Entity_Transformation = "Entity Feed Transformation";
        public const string Filter_Rule = "Filter Rule";
        public const string Request_Filter_Rule = "Request Filter Rule";

        public const string Vendor_Type_Bloomberg = "Bloomberg";
        public const string Vendor_Type_Reuters = "Reuters";

        public const string CC_Pre_Loading = "Pre-Loading";
        public const string CC_Post_Loading = "Post-Loading";
        public const string CC_Class_Type_Script_Executable = "Script Executable";
        public const string CC_Class_Type_Custom_Class = "Custom Class";
    }


    public class RM_ColumnNames
    {
        //Definition, Master and Leg Sheets
        public const string Entity_Type_Name = "Entity Type Name";
        public const string Entity_Type = "Entity Type";
        public const string Tags = "Tags";
        public const string Allowed_Users = "Allowed Users";
        public const string Allowed_Groups = "Allowed Groups";

        public const string Attribute_Name = "Attribute Name";
        public const string Unique_Key_Name = "Unique Key Name";
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
        //attribute description and display meta-data columns added
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

        //Layout preference sheet
        public const string Group_Level_Layout = "Group Level Layout";
        public const string User_Name = "User Name";

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


        //Data Source Definition
        public const string Data_Source = "Data Source";
        public const string Data_Source_Name = "Data Source Name";
        public const string Data_Source_Description = "Data Source Description";
        public const string Feed_Name = "Feed Name";
        public const string Feed_Type = "Feed Type";
        public const string Feed_File_Type = "File Type";
        public const string Record_Delimiter = "Record Delimiter";
        public const string Record_Length = "Record Length";
        public const string Field_Delimiter = "Field Delimiter";
        public const string Comment_Char = "Comment Char";
        public const string Single_Escape = "Single Escape";
        public const string Paired_Escape = "Paired Escape";
        public const string Exclude_Regex = "Exclude Regex";
        public const string Root_XPath = "Root XPath";
        public const string Record_XPath = "Record XPath";
        public const string Server_Type = "Server Type";
        public const string Connection_String = "Connection String";
        public const string Loading_Criteria = "Loading Criteria";

        //Feed Fields
        public const string Feed_Field_Name = "Field Name";
        public const string Feed_Field_Description = "Field Description";
        public const string Start_Index = "Start Index";
        public const string End_Index = "End Index";
        public const string Position = "Position";
        public const string Mandatory = "Mandatory";
        public const string Persistable = "Persistable";
        public const string Allow_Trim = "Allow Trim";
        public const string X_Path = "XPath";
        public const string Remove_White_Spaces = "Remove White Spaces";
        public const string Is_API = "Is API";
        public const string Is_FTP = "Is FTP";
        public const string Is_Bulk = "Is Bulk";
        public const string Is_Unique = "Is Unique";

        //Feed Mapping
        public const string Mapped_Column_Name = "Mapped Column Name";
        public const string Primary_Column_Name = "Primary Column Name";
        public const string Map_Name = "Map Name";
        public const string Map_State = "Map State";

        //Entity Type Mapping
        public const string Date_Format = "Date Format";
        public const string Update_Blank = "Update Blank";
        public const string Is_Insert = "Is Insert";
        public const string Is_Update = "Is Update";

        //Bulk License Setup
        public const string Loading_Task_Name = "Loading Task Name";
        public const string Loading_Task_Description = "Loading Task Description";
        public const string Bulk_File_Path = "Bulk File Path";
        public const string Bulk_File_Date_Type = "Bulk File Date Type";
        public const string Bulk_File_Date = "Bulk File Date";
        public const string Bulk_File_Business_Days = "Bulk File Business Days";
        public const string Difference_File_Path = "Difference File Path";
        public const string Difference_File_Date_Type = "Difference File Date Type";
        public const string Difference_File_Date = "Difference File Date";
        public const string Difference_File_Business_Days = "Difference File Business Days";

        //API License Setup
        public const string Import_Task_Name = "Import Task Name";
        public const string Import_Task_Description = "Import Task Description";
        public const string Vendor_Type = "Vendor Type";
        public const string Request_Type = "Request Type";
        public const string Vendor_Identifier = "Vendor Identifier";
        public const string Asset = "Market Sector/Asset Type";

        //FTP License Setup
        public const string Request_Task_Name = "Request Task Name";
        public const string Request_Task_Description = "Request Task Description";
        public const string Request_Vendor_Type = "Request Vendor Type";
        public const string Request_Transport_Type = "Request Transport Type";
        public const string Request_Vendor_Identifier = "Request Vendor Identifier";
        public const string Request_Outgoing_FTP = "Request Outgoing FTP";
        public const string Request_Market_Sector = "Request Market Sector/Asset Type";
        public const string Request_Data_Request_Type = "Data Request Type";
        public const string Response_Task_Name = "Response Task Name";
        public const string Response_Task_Description = "Response Task Description";
        public const string Response_Vendor_Type = "Response Vendor Type";
        public const string Response_Transport_Type = "Response Transport Type";
        public const string Response_Incoming_FTP = "Response Incoming FTP";
        //public const string Response_Data_Request_Type = "Response Data Request Type";

        //Feed Custom Classes
        public const string Call_Type = "Call Type";
        public const string Class_Type = "Class Type";
        public const string Script_Or_Class_Name = "Script/Class Name";
        public const string Assembly_Path = "Assembly Path";
        public const string Sequence_Number = "Sequence Number";

        //Reports
        public const string Repository_Name = "Repository Name";
        public const string Repository_Description = "Repository Description";
        public const string Report_Name = "Report Name";
        public const string Report_Type = "Report Type";
        public const string Legacy_Report = "Legacy Report";
        public const string Report_Attribute_Name = "Report Attribute Name";
        public const string Report_Attribute_Description = "Report Attribute Description";
        public const string Report_Header = "Report Header";
        public const string Multisheet_Report = "Multisheet Report";
        public const string Calendar = "Calendar";
        public const string Start_Date = "Start Date";
        public const string Custom_Value_Start_Date = "Custom Value Start Date";
        public const string End_Date = "End Date";
        public const string Custom_Value_End_Date = "Custom Value End Date";
        public const string Is_DWH_Extract = "Is DWH Extract";
        public const string Show_Entity_Codes = "Show Entity Codes";
        public const string Show_Display_Names = "Show Display Names";
        public const string Column_Width = "Column Width";
        public const string Decimal_Places = "Decimal Places";

        //Prioritization
        public const string First_Priority_Vendor_Exception = "First Priority Vendor Exception Configured";
        public const string Delete_Previous_Exception = "Delete Previous Exception";
        public const string All_Vendor_Missing_Exception = "All Vendor Value Missing Exception Configured";
        public const string Run_Calculated_Rules = "Run Calculated Rules";

        //Real Time Preference
        public const string Preference_Name = "Preference Name";
        public const string Preference_Description = "Preference Description";
        public const string Market_Sector = "Market Sector";
        public const string Transport = "Transport";
        public const string Data_Request_Type = "Data Request Type";
        public const string Asset_Type = "Asset Type";
        public const string Bloomberg = "Bloomberg";
        public const string Reuters = "Reuters";

        //Downstream Tasks
        public const string Task_Name = "Task Name";
        public const string Task_Description = "Task Description";
        public const string Reports = "Report";
        public const string Report_System = "Report System";
        public const string REQUIRE_REPORT_ATTRIBUTE_LEVEL_AUDIT = "Report Attribute Level Audit";
        public const string Direct_Downstream_Post = "Post To Downstream System";
     
        public const string Send_Realtime_Updates = "Send Real Time Updates";
        public const string Publication_Queues = "Publication Queues";
        public const string Publication_Format = "Publication Format";
        public const string Publication_Delimiter = "Publication Delimiter";
        public const string Extraction_Transport = "Extraction Transport";
        public const string Remote_File_Name = "Remote File Name";
        public const string Remote_File_Location = "Remote File Location";
        public const string Remote_File_Format = "Remote File Format";
        public const string Remote_File_Delimiter = "Remote File Delimiter";
        public const string Report_File_Date_Type = "Report File Date Type";
        public const string Custom_Value_Report_File_Date_Type = "Custom Value Report File Date Type";
        public const string Email_Ids = "Email Ids";
        public const string Email_Transport = "Email Transport";
        public const string Email_File_Format = "Email File Format";
        public const string Email_File_Delimiter = "Email File Delimiter";
        public const string Email_File_Transport = "Email File Transport";
        public const string Email_File_Location = "Email File Location";
      

        //Time Series Task
        public const string Primary_Attribute = "Primary Attribute";
        public const string File_Path = "File Path";
    }
    public class RM_Modeler_RuleTypes
    {
        public const string Validation = "Validation";
        public const string Calculated_Field = "Calculated Field";
        public const string Alert = "Alert";
        public const string Group_Validations = "Group Validation";
    }
    public class RM_Layout_Type
    {
        public const string System = "System";
        public const string User = "User";
        public const string Group = "Group";
    }

    public class RM_Layout_Entity_States
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Create_and_Update = "Create,Update";
    }
    public class RM_Layout_Panel
    {
        public const string Left = "Left";
        public const string Center = "Center";
        public const string Right = "Right";
    }

    public class RM_Page_Identifiers
    {
        public const string Entity_Identifier = "Entity Identifier";
        public const string Dashboard = "Dashboard";
        public const string Workflow = "Workflow";
    }

    public class RM_Functionality_Identifiers
    {
        public const string Duplicates = "Duplicates";
        public const string Override_Attributes = "Override Attributes";
        public const string Workflow_Inbox = "Workflow Inbox";
        public const string Entity_Tab = "Entity Tab";
    }

    #endregion

    #region SRM_Vendor_Management
    public class SRM_VendorManagement_SheetNames
    {
        public const string CONFIGURATIONS = "Configurations";
        public const string HEADERS = "Headers";
    }
    public class SRM_VendorManagement_ColumnNames
    {
        //Sheet 1 Configurations
        public const string VENDOR = "Vendor Name";
        public const string PREFERENCE_NAME = "Preference Name";
        public const string REQUEST_TYPE = "Request Type";
        public const string CONFIGURATION_KEY = "Configuration Key";
        public const string VALUE = "Value";

        //Sheet 2 Headers
        public const string HEADER_TYPE_NAME = "Header Type";
        public const string HEADER_NAME = "Header Name";
        public const string HEADER_VALUE = "Header Value";
    }

    public class SRM_VendorManagement_HeaderType
    {
        public const string GetData = "GetData";
        public const string BVAL = "BVAL";
        public const string GetActions = "GetActions";
        public const string GetCompany = "GetCompany";
        public const string GetFundamentals = "GetFundamentals";
        public const string GetHistory = "GetHistory";
        public const string Corpaction = "Corpaction";
        public const string Security = "Security";
        public const string FTP = "FTP";
    }
    #endregion


    #region SRMCommon_classes
    public class SRM_WorkFlow_SheetNames
    {
        public const string Workflows = "Workflows";
        public const string Workflow_Mapping = "Workflow Mapping";
        public const string Workflow_Inbox_Attributes = "Workflow Inbox Setup";
        public const string Workflow_Template_Mapping = "Template Mapping";
        public const string Data_Validation_Checks = "Data Validation Checks";
        public const string Email_Configuration = "Email Configuration";
    }

    public class SRM_WorkFlow_ColumnNames
    {
        public const string Module = "Module";
        public const string Workflow_Type = "Workflow Type";
        public const string Workflow_Name = "Workflow Name";
        public const string Security_Type = "Security Type";
        public const string Entity_Type = "Entity Type";
        public const string Attribute_Name = "Attribute Name";
        public const string Is_Primary_Attribute = "Is Primary Attribute";
        public const string Is_Rule_Configured = "Is Rule Configured";
        public const string Is_Default_Workflow = "Is Default Workflow";
        public const string Rule_Text = "Rule Text";
        public const string Rule_Priority = "Rule Priority";
        public const string Rad_Workflow_Template_Name = "Rad Workflow Template Name";
        public const string Stage = "Stage";
        public const string Mandatory = "Mandatory";
        public const string Uniqueness = "Uniqueness";
        public const string Primary_Key = "Primary Key";
        public const string Validations = "Validations";
        public const string Alerts = "Alerts";
        public const string Basket_Validations = "Basket Validation";
        public const string Basket_Alerts = "Basket Alert";
        public const string Group_Validation = "Group Validation";
        public const string Action_Name = "Action Name";
        public const string Include_Action = "Include Action";
        public const string Keep_Application_URL_In_Footer = "Keep Application URL In Footer";
        public const string Send_Consolidated_Email_For_Bulk_Action = "Send Consolidated Email For Bulk Action";
        public const string Keep_Creator_In_CC = "Keep Creator In CC";
        public const string To = "To";
        public const string Subject = "Subject";
        public const string Bulk_Subject = "Bulk Subject";
        public const string Mail_Body_Title = "Mail Body Title";
        public const string Mail_Body_Content = "Mail Body Content";
        public const string DataSectionAttributes = "Data Section Attributes";
    }

    public class SRM_WorkFlow_TF_Type
    {
        public const string True = "TRUE";
        public const string False = "FALSE";
    }

    public class SRM_WorkFlow_YN_Type
    {
        public const string Yes = "Yes";
        public const string No = "No";
    }

    public class ChainInfo
    {
        public int ChainId { get; set; }
        public string ChainName { get; set; }
        public string CalendarName { get; set; }
        public string RecurrenceType { get; set; }
        public DateTime StartDate { get; set; }
        public string RecurrencePattern { get; set; }
        public int Interval { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfRecurrences { get; set; }
        public int DaysOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public int TimeIntervalOfRecurrence { get; set; }
        public bool NeverEndJob { get; set; }
    }


    public class TaskInfo
    {
        public string ChainName { get; set; }
        public string TaskName { get; set; }
        public string TaskType { get; set; }
        public string ModuleName { get; set; }
        public bool IsMuted { get; set; }
        public bool ProceedOnFail { get; set; }
        public bool ReRunOnFail { get; set; }
        public int RetryDuration { get; set; }
        public int NumberOfRetries { get; set; }
        public int TimeOut { get; set; }
        public int TaskInstanceWait { get; set; }
        public string TaskWaitSubscription { get; set; }
        public List<TaskDependency> Dependencies { get; set; }
        public TaskInfoBase OnFailRunTask { get; set; }
    }

    public class TaskInfoBase
    {
        public string TaskName { get; set; }
        public string TaskType { get; set; }
        public string TaskModuleName { get; set; }
    }

    public class TaskDependency : TaskInfoBase
    {
        public string Relation { get; set; }
    }

    public class ChainSubscription
    {
        public string ChainName { get; set; }
        public string Type { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string body { get; set; }
    }

    public class TaskSubscription
    {
        public string ChainName { get; set; }
        public string TaskName { get; set; }
        public string TaskType { get; set; }
        public string ModuleName { get; set; }
        public string Type { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string body { get; set; }
    }

    public enum DaysOfWeek
    {
        sunday = 0,
        monday = 1,
        tuesday = 2,
        wednesday = 3,
        thursday = 4,
        friday = 5,
        saturday = 6
    }

    #endregion


    #region CommonClasses

    public class CommonMigrationSelectionInfo
    {
        public object Value { get; set; }
        public string Text { get; set; }

        public string AdditionalText { get; set; }
    }

    public class CommonSheetInfo
    {
        public CommonSheetInfo()
        {
            this.lstColumnInfo = new List<CommonColumnInfo>();
            this.lstPrimaryAttr = new List<string>();
            this.lstUniqueKeys = new List<SRMMigrationUniqueKeys>();
            this.lstGroupValidations = new List<SRMMigrationGroupValidations>();
        }

        public string sheetName { get; set; }
        public List<CommonColumnInfo> lstColumnInfo { get; set; }

        public List<string> lstPrimaryAttr { get; set; }
        public List<SRMMigrationUniqueKeys> lstUniqueKeys { get; set; }
        public List<SRMMigrationGroupValidations> lstGroupValidations { get; set; }
        public bool allowMultiplesAgainstPrimary { get; set; }
        public bool allowExtraColumnsInSheet { get; set; }
    }

    public class CommonColumnInfo
    {
        public CommonColumnInfo()
        {
            this.lstPossibleVal = new List<string>();
            this.lookupType = LookupType.NONE;
            this.acceptorRegex = null;
            this.rejectorRegex = null;
        }

        public string columnName { get; set; }
        public DataTypeName dataTypeName { get; set; }
        public string dataTypeLength { get; set; }
        public List<string> lstPossibleVal { get; set; }
        public LookupType lookupType { get; set; }
        public bool allowBlanks { get; set; }
        public string lookupColumnName { get; set; }
        public Regex acceptorRegex { get; set; }
        public Regex rejectorRegex { get; set; }
        public bool isCaseSensitive { get; set; }
    }

    public class SRMMigrationUniqueKeys
    {
        public SRMMigrationUniqueKeys()
        {
            this.lstUniqueColumns = new List<string>();
        }
        public List<string> lstUniqueColumns { get; set; }
    }

    public class SRMMigrationGroupValidations
    {
        public SRMMigrationGroupValidations()
        {
            this.lstGVColumns = new List<string>();
        }
        public List<string> lstGVColumns { get; set; }
    }

    ////////////////////////////////////////////////////
    // IMPORTANT (FeatureName Class - Constant Names) //
    // For Common   -   SRM_                          //
    // For Sec      -   SM_                           //
    // For Ref      -   RM_                           //
    ////////////////////////////////////////////////////
    public enum MigrationFeatureEnum
    {
        [Description("Common Attributes")]
        SM_CommonAttributes = 0,

        [Description("Security Type Modeler")]
        SM_SecurityTypeModeler = 1,

        [Description("Common Rules")]
        SM_CommonRules = 2,

        [Description("Unique Keys")]
        SM_UniqueKeys = 3,

        [Description("Source Interface Setup")]
        SM_DataSource = 4,

        [Description("Data Source Prioritization")]
        SM_DataSourcePrioritization = 5,

        [Description("Downstream Reports")]
        SM_DownstreamReports = 6,

        [Description("Downstream Tasks")]
        SM_DownstreamTasks = 7,

        [Description("Real Time Preferences")]
        SM_RealtimePreference = 8,

        [Description("Transport Tasks")]
        SM_TransportTasks = 9,

        [Description("Downstream Systems")]
        SM_DownstreamSystems = 10,

        [Description("Validation Tasks")]
        SM_ValidationTasks = 11,

        [Description("Workflow Setup")]
        SRM_WorkFlowModeler = 12,

        [Description("Common Configuration")]
        SM_CommonConfig = 13,

        [Description("Vendor System Settings")]
        SRM_VendorSettings = 14,

        [Description("Common Legs")]
        SM_CommonLegs = 15,

        [Description("Entity Type Modeler")]
        RM_EntityTypeModeler = 16,

        [Description("Source Interface Setup")]
        RM_DataSource = 17,

        [Description("Data Source Prioritization")]
        RM_Prioritization = 18,

        [Description("Downstream Reports")]
        RM_Reports = 19,

        [Description("Downstream Tasks")]
        RM_DownstreamTasks = 20,

        [Description("Real Time Preferences")]
        RM_RealtimePreference = 21,

        [Description("Transport Tasks")]
        RM_TransportTasks = 22,

        [Description("Downstream Systems")]
        RM_DownstreamSystems = 23,

        [Description("Validation Tasks")]
        RM_ValidationTasks = 24,

        [Description("Time Series Update Tasks")]
        RM_TimeSeriesUpdateTasks = 25,

        [Description("Task Manager")]
        SRM_TaskManager = 26,

        [Description("Leg Order")]
        SM_LegsOrder = 27,

        [Description("Downstream Sync")]
        SRM_DownstreamSync = 28,

    }

    public enum SRMModuleNames
    {
        [Description("SM")]
        Security_Master = 3,

        [Description("RM")]
        Reference_Master = 6,

        [Description("FM")]
        Fund_Master = 18,

        [Description("PM")]
        Party_Master = 20
    }

    public class CommonMigrationFeatureInfo
    {
        public MigrationFeatureEnum FeatureEnum { get; set; }
        public int ModuleID { get; set; }
        public string ErrorMsg { get; set; }
        public string SyncStatus { get; set; }

        public bool IsDownloadable { get; set; }
    }

    public class MigrationFeatureInfo : CommonMigrationFeatureInfo
    {
        public ObjectSet SourceSet { get; set; }
        public ObjectSet TargetSet { get; set; }
        public ObjectSet DeltaSet { get; set; }
    }

    public class MigrationFeatureErrorInfo : CommonMigrationFeatureInfo
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FeatureDisplayName { get; set; }
    }

    public class SRMErrorMessages
    {
        public const string Invalid_Data_Type = "Data Type is Invalid.";
        public const string Data_Precision_Incorrect = "Precision Limit reached.";
        public const string Value_Can_Not_Be_Empty = "Value can not be Empty.";
        public const string Incorrect_DateTime_Format = "Incorrect DateTime Format.";
        public const string Incorrect_Date_Format = "Incorrect Date Format.";
        public const string Incorrect_Boolean_Value = "Incorrect Boolean Value.";
        public const string Invalid_Data_In_Enum_Varchar = "Invalid Data.";
        public const string Invalid_User_In_Enum_Varchar = "Invalid User(s)";
        public const string Invalid_Group_In_Enum_Varchar = "Invalid Group(s)";
        public const string Lookup_Column_Does_Not_Exist = "Lookup Column Does Not Exist";

        public const string Meta_Data_Invalid_Data_Type_Length = "Static Meta-Data Data Type Length is Invalid.";
        public const string Meta_Data_Invalid_Data_Type = "Static Meta-Data Data Type is Invalid.";

        public const string Values_Should_Be_Same_For_Same = "Values Should Be Same For Same";
        public const string Entire_Row_Is_Duplicate = "Duplicate Row.";
        public const string Primary_Value_Is_Duplicate = "Duplicate Data";
        public const string Unique_Columns_Within_Primary_Are_Duplicate = "Duplicate Data.";
        public const string Remarks_Column_Exists = "Remarks Column Exists in the Data provided.";
        public const string Status_Column_Exists = "Status Column Exists in the Data provided.";
        public const string Incorrect_Priority = "Invalid Priority.";
        public const string Incorrect_RuleState = "Invalid Rule State.";
        public const string Incorrect_RuleType = "Invalid Rule Type.";
        public const string Invalid_Common_Attribute = "Attribute name does not exist.";
        public const string Priority_Can_Not_Be_Empty = "Priority can not be Empty.";
        public const string RuleState_Can_Not_Be_Empty = "Rule State can not be Empty.";
        public const string RuleType_Can_Not_Be_Empty = "Rule Type can not be Empty.";
        public const string Invalid_Sectype_Name = "Invalid Sectype Name";
        public const string Invalid_Creation_DateType = "Invalid Creation Date type.";
        public const string Duplicate_Rule_Name = "Rule name can not be Duplicate.";
        public const string Duplicate_Priority = "Priority can not be Duplicate.";
        public const string Sectype_Name_Can_Not_Be_Empty = "Security Type Name can not be Empty.";
        public const string Leg_Name_Can_Not_Be_Empty = "Leg Name can not be Empty.";
        public const string Invalid_Leg_Name = "Invalid Leg Name";

        #region SRM_Workflow_Error_Messages
        public const string Invalid_Module_In_Sheet = "Invalid Module";
        public const string Workflow_Name_Already_Exists = "Workflow Name Already Exists";
        public const string Workflow_Has_Pending_Request = "Workflow Has Pending Request";
        public const string Invalid_Workflow_Type_Name = "Invalid Workflow Type Name";
        public const string Common_Attribute_Can_Not_Be_Empty = "Common Attribute can not be Empty.";
        public const string Workflow_Already_Exists_for = "Workflow Already Exists for";
        public const string Invalid_Instrument_Type_Mapped = "Invalid Instrument Type Mapped";
        public const string Workflow_can_be_mapped_to_only_one_Instrument_Type = "Workflow can be mapped to only one Instrument Type";
        public const string Workflow_Mapping_Does_Not_Exist_In_All_Sheets = "Workflow mapping does not exist in all sheets";
        public const string More_Than_One_Primary_Attribute_Are_Mapped = "More than one primary attributes are mapped";
        public const string More_Than_Four_Attribute_Are_Mapped_In_Other_Display_Attributes = "More than four attributes are mapped in other display attributes";
        public const string No_Primary_Attribute_Is_Mapped = "No primary attribute is mapped";
        public const string Invalid_Attribute_Is_Mapped_For = "Invalid attribute is mapped for";
        public const string Invalid_Common_Attribute_Is_Mapped = "Invalid common attribute is mapped";
        public const string Invalid_RAD_Template_Is_Mapped = "Invalid RAD Template Is Mapped";
        public const string Invalid_Is_Default_Workflow_And_Is_Rule_Configured_Error = "Is Default Workflow and Is Rule Configured bits can't be yes/no simultaneously";
        public const string Default_Rule_Is_Already_Set = "Default Rule is already set";
        public const string Rule_Text_And_Priority_Cannot_Be_Assigned_To_Default_Rule = "Rule text and Priority cannot be assigned to default rule";
        public const string Mismatch_In_RAD_Template_In_Previous_And_Current_Sheet = "Mismatch in RAD Template in previous and current Sheet";
        public const string RAD_Template_has_invalid_number_of_states = "RAD Template has invalid number of states";
        public const string Invalid_isDefault_OR_RuleText = "Invalid isDefault OR RuleText in previous and current Sheet";
        public const string Invalid_Stage_For_RAD_Template = "Invalid Stage For RAD Template";
        public const string Error_While_Inserting_Workflow = "Error while saving workflow.";
        public const string Invalid_Security_Type_Mapped = "Invalid Security Type Mapped";
        public const string Invalid_Entity_Type_Mapped = "Invalid Entity Type Mapped";
        public const string Security_Type_Mapped_In_More_Than_One_Workflow = "Security type mapped in more than one workflow";
        public const string Entity_Type_Mapped_In_More_Than_One_Workflow = "Entity type mapped in more than one workflow";
        public const string Priority_Should_be_Greater_Than_Zero = "Priority should be greater than zero";
        public const string Priority_Should_be_Unique = "Priority should be unique";
        public const string Rule_Does_Not_Exist_In_Template_Mapping_Sheet = "Rule does not exist in template mapping sheet";
        public const string Rule_Text_Cannot_Be_Empty = "Rule text cannot be empty";
        public const string Incomplete_Insert_Workflow = "Incomplete workflow";
        public const string Same_Attribute_Cannot_Be_Selected_As_Primary_Display_Attribute_And_Other_Display_Attributes = "Same attribute cannot be selected as primary and other display attribute.";
        public const string Workflow_Type_Cannot_Be_Changed_For_Existing_Workflow = "Workflow Type cannot be changed for existing workflow.";

        public const string RAD_Template_has_invalid_number_of_actions = "RAD Template has invalid number of actions";
        public const string Invalid_Action_For_RAD_Template = "Invalid Action For RAD Template";
        public const string Bulk_Action_Not_Configured = "Bulk Subject cannot be modified if Send Consolidated Email For Bulk Action bit is set to No";


        #endregion
    }

    public enum DataTypeName
    {
        VARCHAR = 0,
        INT = 1,
        DECIMAL = 2,
        DATE = 3,
        DATETIME = 4,
        BIT = 5,
        ENUM_VARCHAR = 6
    }
    public enum LookupType
    {
        NONE = 0,
        SEC = 1,
        REF = 2,
        SEC_OR_REF = 3,
        RAD_USERS = 4,
        RAD_GROUPS = 5,
        RAD_USERS_OR_GROUPS = 6

    }
    public enum SRMRuleType
    {
        VALIDATION = 0,
        FILTER = 1,
        AGGREGATION = 2,
        PRE_TRANSFORMATION = 3,
        TOLERANCE = 4,
        POST_TRANSFORMATION = 5,
        MANUAL_AGGREGATION = 6,
        CONDITIONAL = 7,
        GROUPVALIDATION = 8
    }

    public enum SRMMigrationUserAction
    {
        Download = 0,
        Diff = 1,
        Sync = 2
    }

    #endregion

    #region SRMDownstreamSync
    public class SRM_DownstreamSync_SheetNames
    {
        public const string Downstream_Sync_Setup = "Downstream Sync Setup";
        public const string Downstream_Sync_Configuration = "Downstream Sync Configuration";
        public const string Downstream_Sync_Scheduler = "Downstream Sync Scheduler";
    }

    public class SRM_DownstreamSync_ColumnNames
    {
        public const string Setup_Name = "Setup Name";
        public const string Connection_Name = "Connection Name";
        public const string Calendar = "Calendar";
        public const string Effective_From_Date = "Effective From Date";
        public const string Report_Name = "Report Name";
        public const string Block_Type_Name = "Block Type";
        public const string Module = "Module";
        public const string Start_Date = "Start Date";
        //public const string Custom_Start_Date = "Custom Start Date";
        public const string End_Date = "End Date";
        //public const string Custom_End_Date = "Custom End Date";
        public const string Table_Name = "Table Name";
        public const string Batch_Size = "Batch Size";
        public const string Require_Knowledge_Date_Reporting = "Require Knowledge Date Reporting";
        public const string Require_Time_in_TS_Report = "Require IntraDay Changes";
        public const string Require_Deleted_Asset_Types = "Require Deleted Entities";
        public const string Require_Lookup_Massaging_Start_Date = "Attribute value massaging based on Start Date";
        public const string Require_Lookup_Massaging_Current_Knowledge_Date = "Attribute value massaging based on Knowledge Date";
        public const string CC_Assembly_Name = "Custom Class Assembly Name";
        public const string CC_Class_Name = "Custom Class Name";
        public const string CC_Method_Name = "Custom Class Method Name";
        public const string Queue_Name = "Queue Name";
        public const string Failure_Email_Id = "Failure Email Id";

        public const string Recurrence_Type = "Recurrence Type";
        public const string Recurrence_Pattern = "Recurrence Pattern";
        public const string Interval = "Interval";
        public const string Number_of_Recurrences = "Number of Recurrences";
        public const string Start_Time = "Start Time";
        public const string Time_Interval_of_Recurrence = "Time Interval of Recurrence";
        public const string Never_End_Job = "Never End Job";
        public const string Days_Of_Week = "Days Of Week";
    }
    public class SM_DownstreamSyncReports_DateType
    {
        public const string None = "None";
        public const string Today = "Today";
        public const string Yesterday = "Yesterday";
        public const string LastBusinessDay = "LastBusinessDay";
        public const string T_Minus_N = "T-n";
        public const string Custom = "Custom";
        public const string Now = "Now";
        public const string FirstBusinessDayOfMonth = "FirstBusinessDayOfMonth";
        public const string FirstBusinessDayOfYear = "FirstBusinessDayOfYear";
        public const string LastBusinessDayOfMonth = "LastBusinessDayOfMonth";
        public const string LastBusinessDayOfYear = "LastBusinessDayOfYear";
        public const string LastBusinessDayOfPreviousMonth_Plus_N = "LastBusinessDayOfPreviousMonth+n";
        public const string LastBusinessDayOfPreviousYear_Plus_N = "LastBusinessDayOfPreviousYear+n";
        public const string FirstBusinessDayOfMonth_Plus_N = "FirstBusinessDayOfMonth+n";
        public const string FirstBusinessDayOfYear_Plus_N = "FirstBusinessDayOfYear+n";
        public const string LastExtractionDate = "LastExtractionDate";
    }
    #endregion
}
