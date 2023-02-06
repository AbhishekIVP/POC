using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public class RMCommonConstants
    {

        #region Common Constants

        public const string TABLE_START_NAME = "ivp_refmd_";
        public const string TASK_MASTER_ID = "task_master_id";
        public const string TASK_NAME = "task_name";
        #endregion

        #region Migration Specific Constants

        public const string MIGRATION_COL_REMARKS = "Remarks";
        public const string MIGRATION_COL_STATUS = "Status";
        public const string MIGRATION_COL_ACTION = "Action";

        public const string MIGRATION_SUCCESS = "Success";
        public const string MIGRATION_IN_SYNC = "Already in Sync";
        public const string MIGRATION_FAILED = "Failure";
        public const string MIGRATION_NOT_PROCESSED = "Not Processed";

        public const string MIGRATION_REMARKS_SEPARATOR = "\n";

        public const string ATTR_NOT_PROCESSED = "Attribute not processed";
        public const string ET_NOT_PROCESSED = "Entity type not processed";
        public const string PRIORITIZATION_NOT_PROCESSED = "Prioritization not processed";
        public const string DS_FEED_NOT_PROCESSED = "Data Source/Feed not processed.";
        public const string DS_FEED_NOT_PRESENT = "Data Source/Feed not present.";
        public const string DS_NOT_PROCESSED = "Data Source not processed.";
        public const string FEED_NOT_PROCESSED = "Feed not processed.";
        public const string FEED_MAPPING_NOT_PROCESSED = "Feed mapping not processed.";
        public const string FEED_FIELD_NOT_PROCESSED = "Feed Field(s) not processed.";
        public const string FEED_FIELD_INVALID = "Invalid field name.";
        public const string FEED_ALREADY_EXISTS = "Feed name already exists.";
        public const string INVALID_FEED_FILE_TYPE = "Invalid Feed/File Type";

        public const string LEG_NOT_PROCESSED = "Leg not processed";
        public const string LAYOUT_NOT_PROCESSED = "Layout not processed.";
        public const string TAB_NOT_PROCESSED = "Tab not processed.";
        public const string PII_NOT_SET = "Attribute has to be set as encrypted if PII is set as true.";
        public const string DATATYPE_CANNOT_BE_UPDATED = "Data Type cannot be updated.";
        public const string INVALID_LOOKUP_DISPLAY_ATTRS = "Lookup attribute must be a part of selected lookup display attributes.";

        public const string RECORD_DELIMITER_MISSING = "Record Delimiter is missing.";
        public const string FIELD_DELIMITER_MISSING = "Field Delimiter is missing.";
        public const string ROOT_XPATH_MISSING = "Root XPath is missing.";
        public const string RECORD_XPATH_MISSING = "Record XPath is missing.";
        public const string RECORD_DELIMITER_LENGTH_MISSING = "Record Delimiter/Record Length is missing.";
        public const string INVALID_COMMENT_CHAR = "Invalid Comment Char.";
        public const string INVALID_TAB_ORDER = "Invalid Tab Order.";

        public const string SERVER_TYPE_MISSING = "Server Type is missing.";
        public const string CONNECTION_STRING_MISSING = "Connection String is missing.";
        public const string LOADING_CRITERIA_MISSING = "Loading Criteria is missing.";


        public const string FEED_TYPE_CHANGED = "Feed Type cannot be changed.";

        public const string FEED_FIELDS_NOT_PRESENT = "No feed fields present.";
        public const string FEED_FIELDS_DUPLICATE = "Duplicate feed fields.";
        public const string FIELD_POSITION_DUPLICATE = "Duplicate feed field position.";
        public const string FIELD_POSITION_INVALID = "Invalid feed field position.";
        public const string PRIMARY_FIELD_CHANGED = "Primary field(s) cannot be modified.";
        public const string PRIMARY_FIELD_NOT_PRESENT = "Primary field not present.";
        public const string START_INDEX_EMPTY = "Start Index is empty.";
        public const string END_INDEX_EMPTY = "End Index is empty.";
        public const string FIELD_POSITION_EMPTY = "Field Position is empty.";
        public const string FIELD_XPATH_EMPTY = "Field XPath is empty.";
        public const string PII_NOT_ENCRYPTED = "PII field should be encrypted.";
        public const string LOOKUP_NOT_MAPPED = "Lookup entity type/attribute not mapped.";

        public const string FEED_MAP_PRIM_COL_NOT_PRESENT = "Primary Column not present.";
        public const string RAD_MAP_NAME_NOT_PRESENT = "Map Name not present.";

        public const string RULE_PRIORITY_DUPLICATE = "Duplicate rule priority.";
        public const string RULE_NAME_DUPLICATE = "Duplicate rule name.";

        public const string ENTITY_TYPE_NOT_FOUND = "Entity Type not found";
        public const string ENTITY_ATTRIBUTE_NOT_FOUND = "Attribute not found.";
        public const string ENTITY_TYPE_LEG_NOT_FOUND = "Entity type/leg not found.";
        public const string ENTITY_TYPE_LEG_WITHOUT_PRIMARY = "Leg without primary cannot be mapped.";

        public const string ENTITY_TYPE_NOT_MAPPED = "Entity type mapping is invalid.";
        public const string LEG_NOT_FOUND = "Leg not found.";

        public const string INVALID_LOOKUP_DETAILS = "Invalid lookup details.";
        public const string NO_PRIMARY_FIELD = "Feed has no primary field(s)";

        public const string INSERT_UPDATE_BOTH_FALSE = "Both Is Insert and Is Update cannot be false.";

        public const string PRIMARY_NOT_MAPPED = "All primary fields not mapped.";
        public const string INVALID_PRIMARY_FIELD_MAPPING = "Invalid primary field mapping.";
        public const string INVALID_FIELD_ATTRIBUTE_MAPPING = "Invalid field - attribute mapping.";
        public const string MULTIPLE_PRIMARY_FIELD_MAPPING = "Primary field cannot be mapped with more than one attribute.";
        public const string LEG_ATTRIBUTE_IN_MASTER_FEED = "Leg attributes cannot be mapped in master feed.";
        public const string NO_MASTER_IN_LEG_FEED = "Atleast one master attribute needs to be mapped in leg feed.";
        public const string MORE_THAN_ONE_LEG_IN_FEED = "Cannot map two different legs in a feed.";

        public const string EMPTY_DATE_FORMAT = "Date Format cannot be empty.";

        //License setup
        public const string TASK_ALREADY_EXISTS = "Task already exists.";
        public const string BULK_FILE_PATH_MISSING = "Bulk File path is missing.";
        public const string INVALID_BULK_FILE_DATE_TYPE = "Invalid Bulk File Date Type.";
        public const string INVALID_BULK_FILE_DATE = "Invalid Bulk File Date.";
        public const string INVALID_BULK_FILE_BUSINESS_DAYS = "Invalid Bulk Business Days.";

        public const string INVALID_DIFF_FILE_DATE_TYPE = "Invalid Diff File Date Type.";
        public const string INVALID_DIFF_FILE_DATE = "Invalid Diff File Date.";
        public const string INVALID_DIFF_FILE_BUSINESS_DAYS = "Invalid Diff Business Days.";

        public const string INVALID_API_VENDOR_TYPE = "Invalid Vendor Type.";
        public const string INVALID_API_REQUEST_TYPE = "Invalid Request Type.";
        public const string INVALID_API_VENDOR_IDENTIFIER = "Invalid Vendor Identifier.";
        public const string INVALID_API_MARKET_SECTOR = "Invalid Market Sector.";
        public const string INVALID_API_ASSET_TYPE = "Invalid Asset Type.";

        public const string DIFFERENT_VENDOR_TYPES = "Request and Response Vendor Types cannot be different.";
        public const string INVALID_REQ_VENDOR_TYPE = "Invalid Request Vendor Type.";
        public const string INVALID_REQ_TRANSPORT_TYPE = "Invalid Request Transport Type.";
        public const string INVALID_REQ_VENDOR_IDENTIFIER = "Invalid Request Vendor Identifier.";
        public const string INVALID_REQ_MARKET_SECTOR = "Invalid Request Market Sector.";
        public const string INVALID_REQ_ASSET_TYPE = "Invalid Request Asset Type.";
        public const string INVALID_REQ_OUTGOING_FTP = "Invalid Request Outgoing FTP.";
        public const string INVALID_REQ_DATA_REQUEST = "Invalid Data Request Type.";

        public const string INVALID_RES_VENDOR_TYPE = "Invalid Response Vendor Type.";
        public const string INVALID_RES_TRANSPORT_TYPE = "Invalid Response Transport Type.";
        public const string INVALID_RES_VENDOR_IDENTIFIER = "Invalid Response Vendor Identifier.";
        public const string INVALID_RES_INCOMING_FTP = "Invalid Response Incoming FTP.";
        public const string INVALID_RES_DATA_REQUEST = "Invalid Response Data Request Type.";

        //Custom Class
        public const string LOADING_TASK_NOT_EXISTS = "Loading Task doesn't exist.";
        public const string INVALID_SEQUENCE_NUMBER = "Sequence Number should be greater than zero.";
        public const string INVALID_CALL_TYPE = "Invalid Call Type.";
        public const string INVALID_CLASS_TYPE = "Invalid Class Type.";
        public const string ASSEMBLY_PATH_EMPTY = "Assembly Path cannot be empty.";
        public const string CUSTOM_CLASS_NOT_PROCESSED = "Custom Class not processed.";
        #endregion
        
        #region Prioritization
        public const string ATTRIBUTE_PRIORITIES_NOT_FOUND = "Attribute priorities not found.";
        public const string ENTITY_TYPE_FEED_MAPPING_NOT_EXISTS = "Entity Type Feed mapping does not exist.";
        public const string INVALID_DATA_SOURCE_MAPPED = "Invalid Data Source mapped.";
        public const string INVALID_DATA_SOURCE = "Invalid Data Source.";
        public const string INVALID_MERGING_KEY = "Invalid Unique Key.";
        public const string INCORRECT_DATA_SOURCE_MAPPING = "Incorrect Data Source mapping.";
        public const string DUPLICATE_DATA_SOURCE_MAPPING = "Cannot map same data source in more than one priority.";
        public const string ATTRIBUTE_MERGING_EXCEEDING_DATASOURCES = "Attribute merging details cannot be more than datasources configured.";
        public const string ATTRIBUTE_MERGING_LESS_THAN_ONE = "Atleast two data sources are needed for merging.";


        public const string DATA_SOURCE_NOT_FOUND = "Data Source not found";
        #endregion

        #region Time Series

        public const string ENTITY_TYPE_NAME = "Entity Type Name";
        public const string PRIMARY_ATTRIBUTE = "Primary Attribute";
        public const string FILE_TYPE = "File Type";
        public const string DATE_FORMAT = "Date Format";
        public const string FILE_PATH = "File Path";
        public const string INVALID_DATE_FORMAT = "Invalid Date Format.";
        public const string INVALID_FILE_TYPE = "Invalid File Type.";

        #endregion

    }
}
