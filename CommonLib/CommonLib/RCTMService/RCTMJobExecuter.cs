using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using System.Transactions;
using System.Data;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.dal;

namespace com.ivp.rad.RCTMService
{
    class RCTMJobExecuter
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RCTMJobExecuter");
        static string DBConnectionId = "radDB";// RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
        object locker = new object();
        RCTMDal objSchedulerDal = new RCTMDal();

        RCommonTaskManager.CTMService service;
        public RCTMJobExecuter(RCommonTaskManager.CTMService service)
        {
            this.service = service;

        }

        /// <summary>
        /// Runs the scheduled jobs.
        /// </summary>
        public void RunScheduledJobs(string clientName)
        {
            mLogger.Debug("RscheduledJobExecutor :RunScheduledJobs ->Start running all schedulable jobs");
            DataSet dsJobs = null;
            int jobID;
            int i = -1;
            RCTMScheduledJobInfo jobInfoScheduled = null;
            jobInfoScheduled = new RCTMScheduledJobInfo();
            dsJobs = new DataSet();
            try
            {
                lock (locker)
                {
                    dsJobs = objSchedulerDal.GetAllRunnableJobs();
                    if (dsJobs != null && dsJobs.Tables.Count > 0)
                        mLogger.Error("RscheduledJobExecutor :RunScheduledJobs -> " + dsJobs.Tables[0].Rows.Count + " Jobs to run.");
                    if (dsJobs != null && (dsJobs.Tables.Count > 0 && dsJobs.Tables[0].Rows.Count > 0))
                    {
                        foreach (DataRow dr in dsJobs.Tables[0].Rows)
                        {
                            mLogger.Error("running job id=> " + dr["job_id"]);
                            if (dr["running_job_id"].Equals(System.DBNull.Value) || string.IsNullOrEmpty(Convert.ToString(dr["running_job_id"])))
                            {
                                jobID = Convert.ToInt32(dr["job_id"]);
                                CallProcess(jobID, clientName);
                            }
                            //else if (int.TryParse(Convert.ToString(dr["running_job_id"]), out i))
                            //{
                            //    if (((DateTime)dr["prev_start_time"]).AddDays(1) > ((DateTime)dr["next_schedule_time"]))
                            //    {
                            //        //mLogger.Debug("begin notifying for stale job for id=>" + i);
                            //        //RCTMScheduledJobInfo jobInfo = objSchedulerDal.GetScheduledJobById(i);
                            //        //objSchedulerDal.UpdateNextScheduledTime(jobInfo, true);
                            //        //StringBuilder mailBody = new StringBuilder();
                            //        //mailBody.Append("Hi, <br/><br/><br/> Cosmos Scheduler Service delayed jobs:<br/><br/><br/> <table border=\"1\" cellpadding=\"1\" cellspacing=\"0\" style=\"text-align:center;border-color:#ccc;\"><tr><th style=\"padding:1px 4px;\">Job Id</th><th style=\"padding:1px 4px;\">Job Name</th><th style=\"padding:1px 4px;\">Next Schedule Time</th><th style=\"padding:1px 4px;\">Recurrence Type</th></tr>");

                            //        //mailBody.Append("<tr><td style=\"padding:1px 4px;\">" + dr["job_id"] + "</td><td  style=\"padding:1px 4px;\">" + dr["job_name"] + "</td><td style=\"padding:1px 4px;\">" + dr["next_schedule_time"] + "</td><td style=\"padding:1px 4px;\">" + dr["recurrence_pattern"] + "</td></tr>");
                            //        //mailBody.Append("</table><br/><br/><br/> Thanks,<br/> Cosmos Product Team.");
                            //        //NotifyUserForStaleMail(mailBody.ToString());
                            //        //mLogger.Debug("end notifying for stale job");
                            //    }
                            //    else
                            //    {
                            //        mLogger.Debug("begin call process for already running task");
                            //        CallProcess(Convert.ToInt32(dr["job_id"]));
                            //        mLogger.Debug("end call process for already running task");
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                //throw new RSchedulerException(ex.Message.ToString(), ex);
            }
            mLogger.Debug("RscheduledJobExecutor :RunScheduledJobs ->End running all schedulable jobs");
        }

        /// <summary>
        /// Calls the process.
        /// </summary>
        /// <param name="jobID">The job ID.</param>
        private void CallProcess(int jobID,string clientName)
        {
            //utsav RCommonTaskManager.CTMService service = new RCommonTaskManager.CTMService();
            service.TriggerScheduledChain(jobID, clientName);
            //service.TriggerChain(jobID, true);
        }
        /// <summary>
        /// updates the job for next scheduled time
        /// </summary>
        public void UpdateNextScheduledTime()
        {
            mLogger.Debug("RscheduledJobExecutor :UpdateNextScheduledTime ->Start updating next scheduled time");
            //TransactionScope tranScope = null;
            RDBConnectionManager dbconn = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            try
            {
               // TransactionOptions tranOptions = new TransactionOptions();
                //Set Transaction Isolation Level//
               // tranOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
               // tranOptions.Timeout = new TimeSpan();
                dbconn.UseTransaction = true;
                List<string> parameters = new List<string>();
                //using (tranScope = new TransactionScope(TransactionScopeOption.RequiresNew, tranOptions))
                {
                    DataSet getSheduledJobs = objSchedulerDal.GetAllJobsToBeUpdated();
                    if (getSheduledJobs != null)
                        mLogger.Debug("RscheduledJobExecutor :UpdateNextScheduledTime ->job to be updated " + getSheduledJobs.Tables[0].Rows.Count);
                    if (getSheduledJobs != null && (getSheduledJobs.Tables.Count > 0 && getSheduledJobs.Tables[0].Rows.Count > 0))
                    {
                        foreach (DataRow scheduledJob in getSheduledJobs.Tables[0].Rows)
                        {
                            mLogger.Debug("RscheduledJobExecutor :UpdateNextScheduledTime ->updating job id " + scheduledJob["job_id"].ToString());
                            parameters = DevelopPatternAndReturnParamToUpdate(scheduledJob, scheduledJob["recurrence_pattern"].ToString());
                            if (parameters.Count > 0)
                            {
                                UpdateJob(parameters, Convert.ToInt32(scheduledJob["job_id"]), dbconn);
                            }
                        }
                    }
                    dbconn.CommitTransaction();
                    // tranScope.Complete();
                }
            }
            catch (Exception ex)
            {
                // tranScope.Dispose();
                dbconn.RollbackTransaction();
                mLogger.Error("RscheduledJobExecutor-> On Service start various Next Scheduled times were not updated.");
                mLogger.Error(ex);
            }
            finally
            {
                mLogger.Debug("RscheduledJobExecutor :UpdateNextScheduledTime -> End" + " " +
                                                        "updating next scheduled time");
                if (dbconn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbconn);
            }
        }

        /// <summary>
        /// Updates the Job.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="jobId">The job id.</param>
        private void UpdateJob(List<string> parameters, int jobId, RDBConnectionManager dbConnection)
        {
            objSchedulerDal.UpdateJobForNextScheduleTime(parameters, jobId, dbConnection);
        }

        /// <summary>
        /// Develops the pattern and return param to update.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <param name="recPattern">The rec pattern.</param>
        /// <returns></returns>
        private List<string> DevelopPatternAndReturnParamToUpdate(DataRow scheduledJob, string recPattern)
        {
            mLogger.Debug("RSchedulerDAL :UpdateNextScheduledTime ->Start updating next scheduled time for jobid " + scheduledJob["job_id"]);
            List<string> parameters = new List<string>();
            DateTime nextScheduledTime = new DateTime();
            bool noEndDate = Convert.ToBoolean(scheduledJob["no_end_date"]);
            DateTime jobStartTime = Convert.ToDateTime(scheduledJob["start_time"]);
            nextScheduledTime = Convert.ToDateTime(scheduledJob["next_schedule_time"]);
            DateTime scheduledTime = nextScheduledTime;
            int hours = jobStartTime.Hour;
            int minutes = jobStartTime.Minute;
            int second = jobStartTime.Second;
            int numberOfRuns = Convert.ToInt32(scheduledJob["no_of_runs"]);
            int timeIntervalOfRecurrence = Convert.ToInt32(scheduledJob["time_interval_of_recurrence"]);
            int noOfRecurrences = Convert.ToInt32(scheduledJob["no_of_recurrences"]);
            if (nextScheduledTime.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                for (int interval = numberOfRuns - 1; interval > 0; interval--)
                {
                    nextScheduledTime += new TimeSpan(0, timeIntervalOfRecurrence, 0);

                    if (nextScheduledTime > DateTime.Now)
                    {
                        TimeSpan startTimeTicks = new TimeSpan(hours, minutes, second);
                        TimeSpan nextScheduletimeTicks = new TimeSpan(nextScheduledTime.Hour, nextScheduledTime.Minute, nextScheduledTime.Second);
                        if (nextScheduletimeTicks > startTimeTicks)
                        {
                            if (noEndDate)
                            {
                                parameters.Add(nextScheduledTime.ToString());
                                parameters.Add(interval.ToString());
                                return parameters;
                            }
                            else if (nextScheduledTime <= Convert.ToDateTime(scheduledJob["end_date"]))
                            {
                                parameters.Add(nextScheduledTime.ToString());
                                parameters.Add(interval.ToString());
                                return parameters;
                            }
                        }
                    }
                }
            }
            nextScheduledTime = scheduledTime;


            switch (recPattern.ToLower())
            {
                case "daily":
                    if (Convert.ToInt32(scheduledJob["day_interval"]) > 0)
                    {

                        while (nextScheduledTime < DateTime.Now)
                        {
                            nextScheduledTime = Convert.ToDateTime(Convert.ToDateTime(nextScheduledTime).ToShortDateString()) + new TimeSpan(Convert.ToInt32(scheduledJob["day_interval"]), hours, minutes, second);
                            if (nextScheduledTime < DateTime.Now && nextScheduledTime.ToShortDateString() == DateTime.Now.ToShortDateString() && noOfRecurrences > 1)
                            {
                                while (noOfRecurrences > 0)
                                {
                                    noOfRecurrences--;
                                    nextScheduledTime += new TimeSpan(0, timeIntervalOfRecurrence, 0);
                                    if (nextScheduledTime > DateTime.Now)
                                        break;
                                }
                            }
                        }
                    }
                    break;

                case "weekly":
                    if (Convert.ToInt32(scheduledJob["week_interval"]) > 0)
                    {
                        while (nextScheduledTime < DateTime.Now)
                        {
                            List<string> dayOfWeek = ExtractDaysOfWeek(scheduledJob["days_of_week"].ToString());
                            string scheduledDay = Convert.ToDateTime(nextScheduledTime).DayOfWeek.ToString();
                            DaysOfWeek scheduledDayValue = (DaysOfWeek)Enum.Parse(typeof(DaysOfWeek), scheduledDay.ToLower());

                            for (int day = 0; day < dayOfWeek.Count; day++)
                            {
                                if (!dayOfWeek.Contains(Convert.ToInt32(scheduledDayValue).ToString()))
                                {
                                    if (Convert.ToInt32(dayOfWeek[day]) > (Convert.ToInt32(scheduledDayValue)))
                                    {
                                        int noOfDayToAdd = Convert.ToInt32(dayOfWeek[day]) - (Convert.ToInt32(scheduledDayValue));
                                        nextScheduledTime = Convert.ToDateTime(Convert.ToDateTime(nextScheduledTime).ToShortDateString()) + new TimeSpan(noOfDayToAdd, hours, minutes, second);
                                        if (nextScheduledTime < DateTime.Now && nextScheduledTime.ToShortDateString() == DateTime.Now.ToShortDateString() && noOfRecurrences > 1)
                                        {
                                            while (noOfRecurrences > 0)
                                            {
                                                nextScheduledTime += new TimeSpan(0, timeIntervalOfRecurrence, 0);
                                                noOfRecurrences--;
                                                if (nextScheduledTime > DateTime.Now)
                                                    break;
                                            }
                                        }
                                        break;
                                    }
                                    if (Convert.ToInt32(dayOfWeek[day]) < (Convert.ToInt32(scheduledDayValue)) && day == dayOfWeek.Count - 1)
                                    {
                                        int noOfDayToSub = (Convert.ToInt32(scheduledDayValue)) - Convert.ToInt32(dayOfWeek[0]);
                                        nextScheduledTime = Convert.ToDateTime(Convert.ToDateTime(nextScheduledTime).ToShortDateString()).Subtract(new TimeSpan(noOfDayToSub, 0, 0, 0));
                                        nextScheduledTime = nextScheduledTime.AddDays(Convert.ToInt32(scheduledJob["week_interval"]) * 7);
                                        nextScheduledTime = nextScheduledTime + new TimeSpan(hours, minutes, second);
                                        if (nextScheduledTime < DateTime.Now && nextScheduledTime.ToShortDateString() == DateTime.Now.ToShortDateString() && noOfRecurrences > 1)
                                        {
                                            while (noOfRecurrences > 0)
                                            {
                                                nextScheduledTime += new TimeSpan(0, timeIntervalOfRecurrence, 0);
                                                noOfRecurrences--;
                                                if (nextScheduledTime > DateTime.Now)
                                                    break;
                                            }
                                        }
                                        break;
                                    }
                                }
                                if (Convert.ToInt32(dayOfWeek[day]).Equals(Convert.ToInt32(scheduledDayValue)))
                                {
                                    if (day < dayOfWeek.Count - 1)
                                    {
                                        int noOfDayToAdd = Convert.ToInt32(dayOfWeek[day + 1]) - Convert.ToInt32(dayOfWeek[day]);
                                        nextScheduledTime = Convert.ToDateTime(Convert.ToDateTime(nextScheduledTime).ToShortDateString()) + new TimeSpan(noOfDayToAdd, hours, minutes, second);
                                        if (nextScheduledTime < DateTime.Now && nextScheduledTime.ToShortDateString() == DateTime.Now.ToShortDateString() && noOfRecurrences > 1)
                                        {
                                            while (noOfRecurrences > 0)
                                            {
                                                nextScheduledTime += new TimeSpan(0, timeIntervalOfRecurrence, 0);
                                                noOfRecurrences--;
                                                if (nextScheduledTime > DateTime.Now)
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        nextScheduledTime = Convert.ToDateTime(Convert.ToDateTime(nextScheduledTime).ToShortDateString()).Subtract(new TimeSpan(Convert.ToInt32(dayOfWeek[day]) - Convert.ToInt32(dayOfWeek[0]), 0, 0, 0));
                                        nextScheduledTime = nextScheduledTime.AddDays(Convert.ToInt32(scheduledJob["week_interval"]) * 7);
                                        nextScheduledTime = nextScheduledTime + new TimeSpan(hours, minutes, second);
                                        if (nextScheduledTime < DateTime.Now && nextScheduledTime.ToShortDateString() == DateTime.Now.ToShortDateString() && noOfRecurrences > 1)
                                        {
                                            while (noOfRecurrences > 0)
                                            {
                                                nextScheduledTime += new TimeSpan(0, timeIntervalOfRecurrence, 0);
                                                noOfRecurrences--;
                                                if (nextScheduledTime > DateTime.Now)
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (scheduledTime == nextScheduledTime)
                                break;
                        }
                    }
                    break;
                case "monthly":
                    if (Convert.ToInt32(scheduledJob["month_interval"]) > 0)
                    {
                        while (nextScheduledTime < DateTime.Now)
                        {
                            nextScheduledTime = Convert.ToDateTime(Convert.ToDateTime(nextScheduledTime).ToShortDateString()).AddMonths(Convert.ToInt32(scheduledJob["month_interval"]));
                            nextScheduledTime = nextScheduledTime + new TimeSpan(hours, minutes, second);
                            if (nextScheduledTime < DateTime.Now && nextScheduledTime.ToShortDateString() == DateTime.Now.ToShortDateString() && noOfRecurrences > 1)
                            {
                                while (noOfRecurrences > 0)
                                {
                                    nextScheduledTime += new TimeSpan(0, timeIntervalOfRecurrence, 0);
                                    noOfRecurrences--;
                                    if (nextScheduledTime > DateTime.Now)
                                        break;
                                }
                            }
                        }
                    }
                    break;
            }
            if (nextScheduledTime > DateTime.Now)
            {
                if (noEndDate)
                {
                    parameters.Add(nextScheduledTime.ToString());
                    parameters.Add(noOfRecurrences.ToString());
                }
                else if (nextScheduledTime <= Convert.ToDateTime(scheduledJob["end_date"]))
                {
                    parameters.Add(nextScheduledTime.ToString());
                    parameters.Add(noOfRecurrences.ToString());
                }
            }
            mLogger.Debug("RSchedulerDAL :UpdateNextScheduledTime ->ended updating next scheduled time for jobid " + scheduledJob["job_id"]);
            return parameters;
        }

        /// <summary>
        /// Extracts the days of week.
        /// </summary>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <returns></returns>
        internal static List<string> ExtractDaysOfWeek(string scheduledJob)
        {
            List<string> dayOfWeek = new List<string>();
            int valDaysOfWeek = Convert.ToInt32(scheduledJob);
            int count = 0;

            int fixedVal = 64;
            int itemNumber = 6;
            while (valDaysOfWeek > 0)
            {
                if (valDaysOfWeek >= fixedVal)
                {
                    dayOfWeek.Add(itemNumber.ToString());
                    count++;
                    valDaysOfWeek -= fixedVal;
                }
                fixedVal = fixedVal / 2;
                itemNumber -= 1;
            }
            dayOfWeek.Reverse();
            return dayOfWeek;
        }
    }
    /// <summary>
    /// Class for Days of Week
    /// </summary>
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

    public class RCTMScheduledJobInfo
    {
        private int _jobId = 0;
        private int _schedulableJobId;
        private string _jobName = null;
        private string _jobDescription = null;
        private DateTime _startTime;
        private bool _noEndDate;
        private DateTime _startDate;
        private DateTime _endDate = new DateTime();
        private bool _recurrenceType = false;
        private string _recurrancePattern = null;
        private int _daysInterval = 0;
        private int _weekInterval = 0;
        private int _monthInterval = 0;
        private int _daysOfWeek = 0;
        private int _noOfRecurrences = 0;
        private int _timeIntervalOfRecurrence = 0;
        private DateTime _creationTime;
        private DateTime _modificationTime;
        private bool _isActive = false;
        //private string _nextScheduleTime = null;
        private DateTime _nextScheduleTime;
        private int _noOfRuns = 0;
        private string _createdBy = String.Empty;
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the job ID.
        /// </summary>
        public int JobID
        {
            get { return _jobId; }
            set { _jobId = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [recurrance type].
        /// </summary>
        public bool RecurrenceType
        {
            get { return _recurrenceType; }
            set { _recurrenceType = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [no end date].
        /// </summary>
        /// <value><c>true</c> if [no end date]; otherwise, <c>false</c>.</value>
        public bool NoEndDate
        {
            get { return _noEndDate; }
            set { _noEndDate = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        public DateTime CreationTime
        {
            get { return _creationTime; }
            set { _creationTime = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the modification time.
        /// </summary>
        /// <value>The modification time.</value>
        public DateTime ModificationTime
        {
            get { return _modificationTime; }
            set { _modificationTime = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the recurrence pattern.
        /// </summary>
        public string RecurrencePattern
        {
            get { return _recurrancePattern; }
            set { _recurrancePattern = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the days interval.
        /// </summary>
        public int DaysInterval
        {
            get { return _daysInterval; }
            set { _daysInterval = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the no of recurrences.
        /// </summary>
        /// <value>The no of recurrences.</value>
        public int NoOfRecurrences
        {
            get { return _noOfRecurrences; }
            set { _noOfRecurrences = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the time interval of recurrence.
        /// </summary>
        /// <value>The time interval of recurrence.</value>
        public int TimeIntervalOfRecurrence
        {
            get { return _timeIntervalOfRecurrence; }
            set { _timeIntervalOfRecurrence = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the daysof week.
        /// </summary>
        public int DaysofWeek
        {
            get { return _daysOfWeek; }
            set { _daysOfWeek = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the week interval.
        /// </summary>
        /// <value>The week interval.</value>
        public int WeekInterval
        {
            get { return _weekInterval; }
            set { _weekInterval = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the month interval.
        /// </summary>
        public int MonthInterval
        {
            get { return _monthInterval; }
            set { _monthInterval = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the schedulable job id.
        /// </summary>
        public int SchedulableJobId
        {
            get { return _schedulableJobId; }
            set { _schedulableJobId = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the job.
        /// </summary>
        public string JobName
        {
            get { return _jobName; }
            set { _jobName = value; }
        }
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the job description.
        /// </summary>
        public string JobDescription
        {
            get { return _jobDescription; }
            set { _jobDescription = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the next schedule time.
        /// </summary>
        /// <value>The next schedule time.</value>
        public DateTime NextScheduleTime
        {
            get { return _nextScheduleTime; }
            set { _nextScheduleTime = value; }
        }
        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the no of runs remaining.
        /// </summary>
        public int NoOfRuns
        {
            get { return _noOfRuns; }
            set { _noOfRuns = value; }
        }
        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        public string CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }
        //------------------------------------------------------------------------------------------
    }
}
