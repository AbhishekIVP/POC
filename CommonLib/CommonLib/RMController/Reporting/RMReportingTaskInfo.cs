using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.reporting
{
    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public enum RMReportingTaskFormat
    {
        /// <summary>
        /// Report in PIPE DELIMITED Format
        /// </summary>
        PIPE_DELIMITED = 1,
        /// <summary>
        /// Report in XML Format
        /// </summary>
        XML = 2,
        /// <summary>
        /// Report in COMMA DELIMITED Format
        /// </summary>
        COMMA_DELIMITED = 3,
        /// <summary>
        /// Report in PDF Format
        /// </summary>
        PDF = 4,
        /// <summary>
        /// Report in Excel Format
        /// </summary>
        EXCEL = 5
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public enum RMReportingTaskStartDate
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// None
        /// </summary>
        NONE = 0,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Todays date. 
        /// Independent of Calendar
        /// </summary>
        TODAYS = 1,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Yesterdays date. 
        /// Independent of Calendar.
        /// </summary>
        YESTERDAY = 2,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Last business date.
        /// Dependent on Calendar
        /// </summary>
        LASTBUSINESSDAY = 3,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Difference of business date and the number of days given by the user.
        /// Dependent on Calendar
        /// </summary>
        T_MINUS_N = 4,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Custom date in case user wants to provide the custom date format.
        /// Independent of Calendar.
        /// </summary>
        CUSTOMDATE = 5,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the current date and time.
        /// Independent of Calendar.
        /// </summary>
        NOW = 6,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the starting business date of month.
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFMONTH = 7,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the starting business date of year.
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFYEAR = 8,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of month.
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFMONTH = 9,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of year.
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFYEAR = 10,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of previous month  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFPREVIOUSMONTH_PLUS_N = 11,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of previous year  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFPREVIOUSYEAR_PLUS_N = 12,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the first business date of month  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFMONTH_PLUS_N = 13,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the first business date of year  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFYEAR_PLUS_N = 14,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last extraction date
        /// </summary>
        LASTEXTRACTIONDATE = 100,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last successfull push time
        /// </summary>
        LASTSUCCESSFULPUSHTIME = 101
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public enum RMReportingTaskEndDate
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// None
        /// </summary>
        NONE = 0,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Todays date. 
        /// Independent of Calendar
        /// </summary>
        TODAYS = 1,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Yesterdays date. 
        /// Independent of Calendar.
        /// </summary>
        YESTERDAY = 2,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Last business date.
        /// Dependent on Calendar
        /// </summary>
        LASTBUSINESSDAY = 3,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Difference of business date and the number of days given by the user.
        /// Dependent on Calendar
        /// </summary>
        T_MINUS_N = 4,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Custom date in case user wants to provide the custom date format.
        /// Independent of Calendar.
        /// </summary>
        CUSTOMDATE = 5,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the current date and time.
        /// Independent of Calendar.
        /// </summary>
        NOW = 6,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the starting business date of month.
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFMONTH = 7,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the starting business date of year.
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFYEAR = 8,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of month.
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFMONTH = 9,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of year.
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFYEAR = 10,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of previous month  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFPREVIOUSMONTH_PLUS_N = 11,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of previous year  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFPREVIOUSYEAR_PLUS_N = 12,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the first business date of month  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFMONTH_PLUS_N = 13,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the first business date of year  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFYEAR_PLUS_N = 14,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last extraction date
        /// </summary>
        LASTEXTRACTIONDATE = 100
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public enum RMExtractionDate
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// None
        /// </summary>
        NONE = 0,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Todays date. 
        /// Independent of Calendar
        /// </summary>
        TODAYS = 1,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Yesterdays date. 
        /// Independent of Calendar.
        /// </summary>
        YESTERDAY = 2,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Last business date.
        /// Dependent on Calendar
        /// </summary>
        LASTBUSINESSDAY = 3,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Difference of business date and the number of days given by the user.
        /// Dependent on Calendar
        /// </summary>
        T_MINUS_N = 4,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Custom date in case user wants to provide the custom date format.
        /// Independent of Calendar.
        /// </summary>
        CUSTOMDATE = 5,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the current date and time.
        /// Independent of Calendar.
        /// </summary>
        NOW = 6,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the starting business date of month.
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFMONTH = 7,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the starting business date of year.
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFYEAR = 8,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of month.
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFMONTH = 9,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of year.
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFYEAR = 10,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of previous month  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFPREVIOUSMONTH_PLUS_N = 11,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last business date of previous year  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        LASTBUSINESSDAYOFPREVIOUSYEAR_PLUS_N = 12,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the first business date of month  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFMONTH_PLUS_N = 13,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the first business date of year  and the number of days given by the user..
        /// Dependent of Calendar.
        /// </summary>
        FIRSTBUSINESSDAYOFYEAR_PLUS_N = 14,
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the last extraction date
        /// </summary>
        LASTEXTRACTIONDATE = 100
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMReportExtractionInfo
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the report file format.
        /// </summary>
        /// <value>The report file format.</value>
        public RMReportingTaskFormat ReportFileFormat { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of the extraction file date.
        /// </summary>
        /// <value>The type of the extraction file date.</value>
        public RMExtractionDate ExtractionDateType { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the custom extraction date value
        /// </summary>
        /// <value>The custom extraction date value</value>
        public DateTime CustomExtractionDateValue { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the custom extraction date days.
        /// </summary>
        /// <value>The custom extraction date days.</value>
        public int CustomExtractionDateDays { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the remote file.
        /// </summary>
        /// <value>The name of the remote file.</value>
        public string RemoteFileName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the remote file location.
        /// </summary>
        /// <value>The remote file location.</value>
        public string RemoteFileLocation { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of extraction type
        /// </summary>
        /// <value>The extraction type name.</value>
        public string ExtractionName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the report file format.
        /// </summary>
        /// <value>The name of the report file format.</value>
        public string ReportFileFormatName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the report file format delimiter.
        /// </summary>
        /// <value>The report file format delimiter.</value>
        public string ReportFileFormatDelimiter { get; set; }

    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMReportPublicationInfo
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the report file format.
        /// </summary>
        /// <value>The report file format.</value>
        public RMReportingTaskFormat ReportFileFormat { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of publication queues.
        /// </summary>
        /// <value>The publication queue names.</value>
        public List<string> PublicationQueues { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the report file format.
        /// </summary>
        /// <value>The name of the report file format.</value>
        public string ReportFileFormatName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the report file format delimiter.
        /// </summary>
        /// <value>The report file format delimiter.</value>
        public string ReportFileFormatDelimiter { get; set; }

    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMEmailTaskInfo
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the report file format.
        /// </summary>
        /// <value>The report file format.</value>
        public RMReportingTaskFormat ReportFileFormat { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the email Ids.
        /// </summary>
        /// <value>The email Ids</value>
        public List<string> EmailIds { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the extraction Type.
        /// </summary>
        /// <value>The name of the email extraction Type.</value>
        public string EmailTransportTypeName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the extraction Type.
        /// </summary>
        /// <value>The name of the file extraction Type.</value>
        public string FileTransportTypeName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the file transport location.
        /// </summary>
        /// <value>The file locaation.</value>
        public string FileTransportLocation { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the report file format.
        /// </summary>
        /// <value>The name of the report file format.</value>
        public string ReportFileFormatName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the report file format delimiter.
        /// </summary>
        /// <value>The report file format delimiter.</value>
        public string ReportFileFormatDelimiter { get; set; }
    }

    [Obfuscation(ApplyToMembers = true, Exclude = true)]
    public class RMReportingTaskInfo
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the extraction details ID.
        /// </summary>
        /// <value>The repoting task details ID.</value>
        public int ReportingTaskDetailsID { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the task master ID.
        /// </summary>
        /// <value>The task master ID.</value>
        public int TaskMasterID { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The Start Date</value>
        public RMReportingTaskStartDate StartDate { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the custom value for start date.
        /// </summary>
        /// <value>The Start Date custom value</value>
        public DateTime CustomStartDateValue { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the custom start date days.
        /// </summary>
        /// <value>The custom start date days.</value>
        public int CustomStartDateDays { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The End Date</value>
        public RMReportingTaskEndDate EndDate { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the custom value for end date.
        /// </summary>
        /// <value>The End Date custom value</value>
        public DateTime CustomEndDateValue { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the custom end date days.
        /// </summary>
        /// <value>The custom end date days.</value>
        public int CustomEndDateDays { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the reporting task.
        /// </summary>
        /// <value>The name of the reporting task.</value>
        public string ReportingTaskName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the description of the reporting task.
        /// </summary>
        /// <value>The description of the reporting task.</value>
        public string ReportingTaskDescription { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the report IDs for reports.
        /// </summary>
        /// <value>The report IDs.</value>
        public Dictionary<string, int> ReportSetupIDvsReportId { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the report setup IDs.
        /// </summary>
        /// <value>The report setup IDs.</value>
        public List<string> ReportSetupIDs { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the last run instance ID.
        /// </summary>
        /// <value>The last run instance ID.</value>
        public int LastRunInstanceID { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the last run date.
        /// </summary>
        /// <value>The last run date.</value>
        public DateTime? LastRunDate { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the external system ID
        /// </summary>
        /// <value>External system ID</value>
        public int ExternalSystemID { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the Security Level Audit check
        /// </summary>
        /// <value>Requires Security Level Audit or not</value>
        public bool RequireSecurityLevelAudit { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the object containing information for extraction task
        /// </summary>
        /// <value>The extraction task object.</value>
        public RMReportExtractionInfo objRMReportExtractionInfo { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the object containing information for publication task
        /// </summary>
        /// <value>The publication task object.</value>
        public RMReportPublicationInfo objRMReportPublicationInfo { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the object contaniing information for email task
        /// </summary>
        /// <value>The email task object.</value>
        public RMEmailTaskInfo objRMEmailTaskInfo { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the report names.
        /// </summary>
        /// <value>The report names.</value>
        public List<string> ReportNames { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the report system.
        /// </summary>
        /// <value>The report system.</value>
        public string ReportSystem { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the session date format.
        /// </summary>
        /// <value>The session date format.</value>
        public string SessionDateFormat { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the calendar.
        /// </summary>
        /// <value>The name of the calendar.</value>
        public string CalendarName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the calendar id.
        /// </summary>
        /// <value>The calendar id.</value>
        public int CalendarId { get; set; }

        public bool UsedForRealTimeUpdate { get; set; }

        public bool RealTimeUpdateAfterFlowTask { get; set; }

        public bool UsedForDirectDownstreamPosting { get; set; }
        public bool RequireReportAttributeLevelAudit { get; set; }

    }
}
