using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Text;
using com.ivp.rad.RCTMUtils;

//[Serializable]
//public class TriggerAsOfDateInfo {


//    public string customValue { get; set; }
//    public DateTime? triggerDate { get; set; }
//}

public class ChainedTask
{
    public string chain_name { get; set; }
    public string calendar_name { get; set; }
    public int chain_id { get; set; }
    public string created_by { get; set; }
    public DateTime created_on { get; set; }
    public bool is_active { get; set; }
    public string last_modified_by { get; set; }
    public int scheduled_job_id { get; set; }
    public int max_parallel_instances_allowed { get; set; }
    public bool allow_parallel { get; set; }
    public string filewatcher_info { get; set; }
    public TriggerAsOfDateInfo triggerAsOfDateInfo { get; set; }
    private PropertyInfo[] _PropertyInfos = null;
    public int chain_second_instance_wait { get; set; }
    public int chain_last_run_status { get; set; }
    public string nextScheduleTime { get; set; }
    public string inprogress_subscribers { get; set; }
    public override string ToString()
    {
        if (_PropertyInfos == null)
            _PropertyInfos = this.GetType().GetProperties();

        var sb = new StringBuilder();

        foreach (var info in _PropertyInfos)
        {
            sb.AppendLine(info.Name + ": " + (info.GetValue(this, null) == null ? "null" : info.GetValue(this, null).ToString()));
        }

        return sb.ToString();
    }
}

public class Flow
{
    public int chain_id { get; set; }
    public string chain_name { get; set; }
    public string dependant_on_id { get; set; }
    public int fail_number_retry { get; set; }
    public long fail_retry_duration { get; set; }
    public int flow_id { get; set; }
    public bool is_muted { get; set; }
    public int module_id { get; set; }
    public string module_name { get; set; }
    public int? on_fail_run_task { get; set; }
    public bool proceed_on_fail { get; set; }
    public bool rerun_on_fail { get; set; }
    public string subscribe_id { get; set; }
    public string task_name { get; set; }
    public string task_type_name { get; set; }
    public long timeout { get; set; }
    public string trigger_type { get; set; }
    public bool undo_supported { get; set; }
    public int task_summary_id { get; set; }
    public string configure_page_url { get; set; }
    public bool is_visible_on_ctm { get; set; }
    public bool is_retryable { get; set; }
    public int registered_module_id { get; set; }
    public TaskStatusInfo lastRunTaskStatusInfo { get; set; }
    public String lastRunTaskStatus { get; set; }

    public int task_master_id { get; set; }
    private PropertyInfo[] _PropertyInfos = null;
    public TriggerAsOfDateInfo triggerAsOfDateInfo { get; set; }
    public int task_time_out { get; set; }
    public int task_second_instance_wait { get; set; }
    public string task_wait_subscription_id { get; set; }
    public override string ToString()
    {
        if (_PropertyInfos == null)
            _PropertyInfos = this.GetType().GetProperties();

        var sb = new StringBuilder();

        foreach (var info in _PropertyInfos)
        {
            sb.AppendLine(info.Name + ": " + (info.GetValue(this, null) == null ? "null" : info.GetValue(this, null).ToString()));
        }

        return sb.ToString();
    }
}

public class TaskSummary
{
    public int task_summary_id { get; set; }
    public string created_by { get; set; }
    public DateTime created_on { get; set; }
    public string last_modified_by { get; set; }
    public DateTime last_modified_on { get; set; }
    public string task_description { get; set; }
    public string task_name { get; set; }
    public int task_type_id { get; set; }
    public string task_type_name { get; set; }
    public bool undo_supported { get; set; }
    private PropertyInfo[] _PropertyInfos = null;
    public override string ToString()
    {
        if (_PropertyInfos == null)
            _PropertyInfos = this.GetType().GetProperties();

        var sb = new StringBuilder();

        foreach (var info in _PropertyInfos)
        {
            sb.AppendLine(info.Name + ": " + (info.GetValue(this, null) == null ? "null" : info.GetValue(this, null).ToString()));
        }

        return sb.ToString();
    }
}

public class RScheduledJobInfoWrapper
{
    public string CreatedBy { get; set; }
    public string CreationTime { get; set; }
    public int DaysInterval { get; set; }
    public int DaysofWeek { get; set; }
    public string EndDate { get; set; }
    public bool IsActive { get; set; }
    public string JobDescription { get; set; }
    public int JobID { get; set; }
    public string JobName { get; set; }
    public string ModificationTime { get; set; }
    public int MonthInterval { get; set; }
    public string NextScheduleTime { get; set; }
    public bool NoEndDate { get; set; }
    public int NoOfRecurrences { get; set; }
    public int NoOfRuns { get; set; }
    public string RecurrencePattern { get; set; }
    public bool RecurrenceType { get; set; }
    public int SchedulableJobId { get; set; }
    public string StartDate { get; set; }
    public string StartTime { get; set; }
    public int TimeIntervalOfRecurrence { get; set; }
    public int WeekInterval { get; set; }
}

