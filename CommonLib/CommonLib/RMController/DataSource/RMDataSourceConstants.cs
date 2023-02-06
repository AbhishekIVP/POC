using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public static class RMDataSourceConstants
    {
        #region Static Table Names
        public const string TABLE_DATA_SOURCES = "Data Sources";
        public const string TABLE_FEED_FIELDS = "Feed Fields";
        public const string TABLE_FEED_MAPPING = "Feed Mapping";
        public const string TABLE_FEED_RULES = "Feed Rules";
        public const string TABLE_ENTITY_TYPE_FEED_MAPPING = "Entity Type Mapping";
        public const string TABLE_ENTITY_TYPE_FEED_RULES = "Entity Type Rules";
        public const string TABLE_BULK_LICENSE_SETUP = "Bulk License Setup";
        public const string TABLE_API_LICENSE_SETUP= "API License Setup";
        public const string TABLE_FTP_LICENSE_SETUP = "FTP License Setup";
        public const string TABLE_CUSTOM_CLASSES = "Custom Classes";
        #endregion

        #region Static Column Names

        //Data Source Definition
        public const string Data_Source_ID = "data_source_id";
        public const string FEED_ID = "feed_summary_id";
        public const string FILE_ID = "file_id";
        public const string FILE_TYPE_ID = "file_type_id";
        public const string Data_Source_Name = "Data Source";
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
        public const string Feed_Field_ID = "field_id";
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
        public const string Feed_Mapping_Details_ID = "feed_mapping_detail_id";
        public const string Mapped_Column_ID = "mapped_col_id";
        public const string Mapped_Column_Name = "Mapped Column Name";
        public const string Primary_Column_Name = "Primary Column Name";
        public const string Map_Name = "Map Name";
        public const string Map_State = "Map State";

        //Entity Type Mapping
        public const string Date_Format = "Date Format";
        public const string Update_Blank = "Update Blank";

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

        //Feed Entity Type Mapping
        public const string Lookup_Type_Ref_Sec = "lookup_type_ref_sec";
        public const string Is_Insert = "Is Insert";
        public const string Is_Update = "Is Update";

        //Rule Types
        public const string Feed_Rule_Validation = "Feed Validation";
        public const string Feed_Rule_Transformation = "Feed Transformation";
        public const string Feed_Rule_Entity_Transformation = "Entity Feed Transformation";
        public const string Filter_Rule = "Filter Rule";
        public const string Request_Filter_Rule = "Request Filter Rule";

        #endregion

    }



    public class RMTableRefmVFeedSummary : RMDBCommonConstantsInfo
    {
        public const string FEED_SUMMARY_ID = "feed_summary_id";
        public const string DATA_SOURCE_ID = "data_source_id";
        public const string FEED_NAME = "feed_name";
        public const string FEED_TYPE_ID = "feed_type_id";
        public const string RAD_FILE_ID = "rad_file_id";
        public const string DB_PROVIDER = "db_provider";
        public const string CONNECTION_STRING = "connection_string";
        public const string COLUMN_QUERY = "column_query";
        public const string IS_COMPLETE = "is_complete";
        public const string FILE_NAME = "file_name";
        public const string FILE_TYPE = "file_type";
        public const string ROW_DELIMITER = "row_delimiter";
        public const string RECORD_LENGTH = "record_length";
        public const string FIELD_DELIMITER = "field_delimiter";
        public const string COMMENT_CHAR = "comment_char";
        public const string SINGLE_ESCAPE = "single_escape";
        public const string PAIRED_ESCAPE = "paired_escape";
        public const string ROOT_X_PATH = "root_xpath";
        public const string RECORD_X_PATH = "record_xpath";
        public const string EXCLUDE_REGEX = "exclude_regex";
        public const string FILE_DATE = "file_date";
        public const string FIELD_ID = "field_id";
        public const string FIELD_COUNT = "field_count";
        public const string FILE_ID = "file_id";
        public const string FIELD_NAME = "field_name";
        public const string FIELD_DESCRIPTION = "field_description";
        public const string START_INDEX = "start_index";
        public const string END_INDEX = "end_index";
        public const string FIELD_POSITION = "field_position";
        public const string MANDATORY = "mandatory";
        public const string PERSISTENCE = "persistency";
        public const string VALIDATION = "validation";
        public const string ALLOW_TRIM = "allow_trim";
        public const string FIELD_X_PATH = "field_x_path";
        public const string REMOVE_WHITE_SPACES = "remove_white_space";
        public const string IS_BULK_LOADED = "is_bulk_loaded";
        public const string IS_ENCRYPTED = "is_encrypted";
        public const string IS_PII = "is_pii";
    }

    public class RMTableRefmVLoadingTaskDetails : RMDBCommonConstantsInfo
    {
        public const string LOADING_DETAILS_ID = "loading_details_id";
        public const string TASK_MASTER_ID = "task_master_id";
        public const string LAST_RUN_DATE = "last_run_date";
        public const string BULK_FILE_PATH = "bulk_file_path";
        public const string DIFFERENCE_FILE_PATH = "difference_file_path";
        public const string CUSTOM_CALL_EXISTS = "custom_call_exists";
        public const string BULK_FILE_DATE_TYPE = "bulk_file_date_type";
        public const string BULK_FILE_DATE = "bulk_file_date";
        public const string DIFF_FILE_DATE_TYPE = "diff_file_date_type";
        public const string DIFF_FILE_DATE = "diff_file_date";
        public const string LAST_INSTANCE_FILE_NAME = "last_instance_file_name";
        public const string LAST_INSTANCE_FILE_DATE = "last_instance_file_date";
        public const string LAST_RUN_INSTANCE_ID = "last_run_instance_id";
        public const string DIFF_FILE_DATE_DAYS = "diff_file_date_days";
        public const string BULK_FILE_DATE_DAYS = "bulk_file_date_days";
    }

    public class RMDBCommonConstantsInfo
    {
        public const string IS_ACTIVE = "is_active";
        public const string CREATED_BY = "created_by";
        public const string CREATED_ON = "created_on";
        public const string LAST_MODIFIED_BY = "last_modified_by";
        public const string LAST_MODIFIED_ON = "last_modified_on";

    }

    public class RMTableRefmVFeedFieldDetail : RMDBCommonConstantsInfo
    {
        public const string FEED_FIELD_DETAILS_ID = "feed_field_details_id";
        public const string RAD_FIELD_ID = "rad_field_id";
        public const string IS_BULK = "is_bulk";
        public const string IS_FTP = "is_ftp";
        public const string IS_API = "is_api";
        public const string IS_PRIMARY = "is_primary";
        public const string FEED_SUMMARY_ID = "feed_summary_id";
        public const string IS_UNIQUE = "is_unique";
    }

    public class RMTableRefmVFeedMapping
    {
        public const string FEED_MAPPING_DETAIL_ID = "feed_mapping_detail_id";
        public const string FEED_SUMMARY_ID = "feed_summary_id";
        public const string PRIMARY_COL_ID = "primary_col_id";
        public const string MAPPED_COL_ID = "mapped_col_id";
        public const string MAP_ID = "map_id";
        public const string MAP_STATE = "map_state";
        public const string FILE_ID = "file_id";
        public const string FIELD_NAME = "field_name";
        public const string MAPPING_NAME = "mapping_name";
        public const string MAPPED_COL_NAME = "mapped_col_name";
        public const string PRIMARY_COL_NAME = "primary_col_name";
    }

    
}
