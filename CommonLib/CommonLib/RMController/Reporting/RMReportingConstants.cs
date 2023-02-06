using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.reporting
{
    public class EMReportingConstants
    {
        public const string REPORT_NAME = "report_name";
        public const string REPOSITORY_NAME = "report_repository_name";
        public const string REPORT_ID = "report_id";
        public const string REPOSITORY_ID = "repository_id";
        public const string FILTER_RULE = "Filter Rule";
        public const string TRANSFORMATION_RULE = "Transformation Rule";
        public const string REPORT_SETUP = "Report Setup";
        public const string ATTRIBUTE_MAPPING = "Attribute Mapping";
        public const string REPORT_CONFIGURATION = "Report Configuration";
        public const string REPORT_RULES = "Report Rules";
        public const string REPORT_ATTRIBUTE_ORDER = "Report Attribute Order";
        public const string REPORTING_TASK_CONFIGURATION = "Configuration";
    }

    public partial class EMQueryConstants
    {
        public const string GET_REPORT_TYPE = "REFM:GetReportType";
        public const string SAVE_REPORTING_TASK = "REFMV:SaveReportingTask";
        public const string IS_REPORTING_TASK_NAME_UNIQUE = "REFMV:IsReportingTaskNameUnique";
        public const string GET_DATA_FOR_REPORTING_TASK_CONTROLS = "REFM:GetDataForReportingTaskControls";
    }

    public class RMReportingTaskConstants
    {
        public const string TASK_MASTER_ID = "task_master_id";
        public const string TASK_NAME = "task_name";
        public const string TASK_DESC = "task_desc";
        public const string TASK_TYPE = "task_type";
        public const string USER_NAME = "user_name";
        public const string REPORTING_DETAILS_ID = "reporting_details_id";
        public const string REPORT_SETUP_IDs = "report_setup_ids";
        public const string REPORT_NAMES = "report_names";
        public const string START_DATE_TYPE = "start_date_type";
        public const string CUSTOM_START_DATE = "custom_start_date";
        public const string START_DATE_DAYS = "start_date_days";
        public const string END_DATE_TYPE = "end_date_type";
        public const string CUSTOM_END_DATE = "custom_end_date";
        public const string END_DATE_DAYS = "end_date_days";
        public const string CALENDAR_ID = "calendar_id";
        public const string DOWNSTREAM_SYSTEM_ID = "downstream_system_id";
        public const string IS_AUDIT_SECURITY_LEVEL = "is_audit_security_level";

        public const string PUBLICATION_QUEUE_NAMES = "publication_queue_names";
        public const string PUBLICATION_FORMAT_ID = "publication_format_id";

        public const string EXTRACTION_TRANSPORT_NAME = "extraction_transport_name";
        public const string EXTRACTION_REMOTE_FILE_NAME = "extraction_remote_file_name";
        public const string EXTRACTION_REMOTE_FILE_LOCATION = "extraction_remote_file_location";
        public const string EXTRACTION_FILE_DATE_TYPE = "extraction_file_date_type";
        public const string EXTRACTION_FILE_DATE_DAYS = "extraction_file_date_days";
        public const string EXTRACTION_FORMAT_ID = "extraction_format_id";
        public const string EXTRACTION_FILE_DATE = "extraction_file_date";

        public const string SUBSCRIPTION_EMAIL_IDs = "subscription_email_ids";
        public const string EMAIL_TRANSPORT_NAME = "email_transport_name";
        public const string SUBSCRIPTION_FORMAT_ID = "subscription_format_id";
        public const string USED_FOR_REALTIME_UPDATE = "used_for_realtime_update";
        public const string REALTIME_UPDATE_AFTER_FLOW_TASK = "realtime_update_after_flow_task";
        public const string FILE_TRANSPORT_NAME = "file_transport_name";
        public const string FILE_TRANSPORT_LOCATION = "file_transport_location";

        public const string REPORT_DELIMITER_CHAR = "report_delimiter_char";
        public const string SPECIFIED_ID = "feed_id";
        public const string LAST_RUN_INSTANCE_ID = "last_run_instance_id";
        public const string LAST_RUN_DATE = "last_run_date";
        public const string REPORT_IDs = "report_ids";
        public const string USED_FOR_DIRECT_DOWNSTREAM_POSTING = "used_for_direct_downstream_posting";
        public const string IS_REPORT_ATTRIBUTE_LEVEL_AUDIT = "is_report_attribute_level_audit";
    }
}