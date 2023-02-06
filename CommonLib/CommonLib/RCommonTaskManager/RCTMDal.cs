using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using System.Data;
using System.Configuration;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;

namespace com.ivp.rad.RCTMService
{
    class RCTMDal
    {
        static object objLock = new object();
        static IRLogger mLogger = RLogFactory.CreateLogger("RSchedulerDAL");
        //static string "CTMDB" = RADConfigReader.GetConfigAppSettings("CTMDB");
        //RHashlist htParams = null;
        //        RDBConnectionManager dbConnection = null;

        /// <summary>
        /// Gets all runnable jobs.
        /// </summary>
        /// <returns></returns>
        internal DataSet GetAllRunnableJobs(int interval)
        {
            RHashlist htParams = new RHashlist();
            RDBConnectionManager dbConnection = null;
            lock (objLock)
            {
                mLogger.Debug("RSchedulerDAL :GetAllScheduledJobs ->Start getting all scheduled jobs");
                DataSet dsTemp = null;
                string scheduledJobInterval = ConfigurationManager.AppSettings["ScheduledJobInterval"];
                int timeInt = 0;
                try
                {
                    if (scheduledJobInterval != string.Empty)
                        timeInt = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(scheduledJobInterval) * -3).Minutes;

                    if (interval != 0)
                        timeInt = interval;

                    dsTemp = new DataSet();

                    //dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                    dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                    htParams.Add("mInterval", timeInt);
                    dsTemp = dbConnection.ExecuteQuery("RAD:GetRunnableJobs", htParams, true);
                }
                catch (RDALException rdEx)
                {
                    mLogger.Error(rdEx.ToString());
                    throw rdEx;
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.Message.ToString());
                    throw ex;
                }
                finally
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                    if (htParams != null)
                        htParams = null;
                }
                mLogger.Debug("RSchedulerDAL :GetAllScheduledJobs ->End getting all scheduled jobs");
                return dsTemp;
            }
        }

        /// <summary>
        /// Gets the scheduled job by id.
        /// </summary>
        internal RCTMScheduledJobInfo GetScheduledJobById(int jobID)
        {
            RHashlist htParams = new RHashlist();
            RDBConnectionManager dbConnection = null;
            lock (objLock)
            {
                mLogger.Debug("RSchedulerDAL :GetScheduledJobById ->Start getting scheduled job by ID");
                RCTMScheduledJobInfo objScheduledInfo = null;
                DataSet dsJobInfo = null;
                try
                {
                    htParams = new RHashlist();
                    htParams.Add("job_id", jobID);

                    dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                    dsJobInfo = dbConnection.ExecuteQuery("RAD:GetScheduledJobById", htParams, true);
                }
                catch (RDALException rdEx)
                {
                    mLogger.Error(rdEx.ToString());
                    throw rdEx;
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.Message.ToString());
                    throw ex;
                }
                finally
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                    if (htParams != null)
                        htParams = null;
                }
                if (dsJobInfo != null && dsJobInfo.Tables[0].Rows.Count > 0)
                {
                    objScheduledInfo = new RCTMScheduledJobInfo();
                    objScheduledInfo.JobID = (int)dsJobInfo.Tables[0].Rows[0]["job_id"];
                    objScheduledInfo.SchedulableJobId = (int)dsJobInfo.Tables[0].Rows[0]
                                                                                ["schedulable_job_id"];
                    objScheduledInfo.JobName = (string)dsJobInfo.Tables[0].Rows[0]["job_name"];
                    objScheduledInfo.JobDescription = ReplaceNullString(dsJobInfo.Tables[0].Rows[0]
                                                                                ["job_description"]);
                    objScheduledInfo.CreationTime = (DateTime)dsJobInfo.Tables[0].Rows[0]
                                                                                ["creation_time"];
                    objScheduledInfo.ModificationTime = (DateTime)dsJobInfo.Tables[0].Rows[0]
                                                                                ["modification_time"];
                    objScheduledInfo.DaysInterval = ReplaceNullInteger(dsJobInfo.Tables[0].Rows[0]
                                                                                ["day_interval"]);
                    objScheduledInfo.WeekInterval = ReplaceNullInteger(dsJobInfo.Tables[0].Rows[0]
                                                                                ["week_interval"]);
                    objScheduledInfo.MonthInterval = ReplaceNullInteger(dsJobInfo.Tables[0].Rows[0]
                                                                                ["month_interval"]);
                    objScheduledInfo.DaysofWeek = ReplaceNullInteger(dsJobInfo.Tables[0].Rows[0]
                                                                                ["days_of_week"]);
                    objScheduledInfo.StartDate = (DateTime)dsJobInfo.Tables[0].Rows[0]
                                                                                ["start_date"];
                    objScheduledInfo.EndDate = string.IsNullOrEmpty(dsJobInfo.Tables[0].Rows[0]
                        ["end_date"].ToString()) ? new DateTime() : Convert.ToDateTime(dsJobInfo.Tables[0].Rows[0]["end_date"].ToString());
                    objScheduledInfo.StartTime = (DateTime)dsJobInfo.Tables[0].Rows[0]["start_time"];
                    objScheduledInfo.NoEndDate = ReplaceNullBool(dsJobInfo.Tables[0].Rows[0]
                                                                                ["no_end_date"]);
                    objScheduledInfo.RecurrenceType = ReplaceNullBool(dsJobInfo.Tables[0].Rows[0]
                                                                                ["recurrence_type"]);
                    objScheduledInfo.RecurrencePattern = ReplaceNullString(dsJobInfo.Tables[0].Rows[0]
                                                                                ["recurrence_pattern"]);
                    //objScheduledInfo.NextScheduleTime = ReplaceNullString(dsJobInfo.Tables[0].Rows[0]
                    //                                                            ["next_schedule_time"]);
                    objScheduledInfo.NextScheduleTime = (DateTime)dsJobInfo.Tables[0].Rows[0]
                                                                                ["next_schedule_time"];
                    objScheduledInfo.TimeIntervalOfRecurrence = ReplaceNullInteger(dsJobInfo.Tables[0].
                                                                Rows[0]["time_interval_of_recurrence"]);
                    objScheduledInfo.NoOfRecurrences = ReplaceNullInteger(dsJobInfo.Tables[0].Rows[0]
                                                                                ["no_of_recurrences"]);
                    objScheduledInfo.NoOfRuns = ReplaceNullInteger(dsJobInfo.Tables[0].Rows[0]
                                                                                ["no_of_runs"]);
                    objScheduledInfo.CreatedBy = ReplaceNullString(dsJobInfo.Tables[0].Rows[0]["Created_by"]);
                }

                mLogger.Debug("RSchedulerDAL :GetScheduledJobById ->End getting scheduled job by ID");

                return objScheduledInfo;
            }
        }

        /// <summary>
        /// Updates the next scheduled time.
        /// </summary>
        /// <param name="jobInfo">The job info.</param>
        /// <param name="resetCon">if set to <c>true</c> [reset con].</param>
        internal void UpdateNextScheduledTime(RCTMScheduledJobInfo jobInfo, bool resetCon)
        {
            mLogger.Debug("RSchedulerDAL :GetNextScheduledTime ->Start getting the next scheduled" +
                                                                            " time for the job");
            RHashlist htParamNoOfRuns = null;
            DataSet dsTemp = null;
            bool recurrenceType = false;
            string recurrancePattern = null;
            DateTime nextScheduledTime = jobInfo.NextScheduleTime;
            int dayInterval = 0;
            int weekInterval = 0;
            int monthInterval = 0;
            //int totalDays = 0;
            TimeSpan diffResult;
            bool res = false;
            RHashlist htRes = new RHashlist();
            RHashlist htParams = new RHashlist();
            RDBConnectionManager dbConnection = null;

            try
            {
                lock (objLock)
                {
                    htParams = new RHashlist();
                    htParamNoOfRuns = new RHashlist();
                    recurrenceType = (bool)jobInfo.RecurrenceType;
                    recurrancePattern = jobInfo.RecurrencePattern;
                    dayInterval = (int)jobInfo.DaysInterval;
                    weekInterval = (int)jobInfo.WeekInterval;
                    monthInterval = (int)jobInfo.MonthInterval;
                    if (dbConnection != null)
                        RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                    dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                    if (!jobInfo.RecurrenceType)
                    {
                        SetNextScheduledTime(jobInfo, jobInfo.NextScheduleTime, dbConnection);
                        return;
                    }

                    if (jobInfo.NoOfRuns > 0)
                    {
                        nextScheduledTime = nextScheduledTime.AddMinutes(Convert.ToDouble
                                                                (jobInfo.TimeIntervalOfRecurrence));
                        htParams.Clear();
                        htParams.Add("next_scheduled_time", nextScheduledTime);
                        htParams.Add("job_id", jobInfo.JobID);
                        dsTemp = dbConnection.ExecuteQuery("RAD:updateNoOfRuns", htParams, true);
                        diffResult = nextScheduledTime.Subtract(jobInfo.NextScheduleTime);
                        if (!(diffResult.Days >= 1))
                        {
                            res = CheckActualNoOfRuns(jobInfo, dbConnection);
                            if (res)
                                goto Update;
                            else
                            {
                                SetNextScheduledTime(jobInfo, nextScheduledTime, dbConnection);
                                return;
                            }
                        }
                    }

                Update:
                    if (recurrenceType.Equals(true))
                    {
                        TimeSpan startTime;
                        string actualDate = null;
                        actualDate = DateTime.Parse(jobInfo.NextScheduleTime.ToString()).
                                                                                 ToShortDateString();
                        startTime = jobInfo.StartTime.TimeOfDay;
                        nextScheduledTime = DateTime.Parse(actualDate + " " + startTime.ToString());
                        switch (recurrancePattern.ToLower())
                        {
                            case "daily":
                                nextScheduledTime = nextScheduledTime.AddDays(Convert.ToDouble(jobInfo.DaysInterval));
                                break;
                            case "weekly":
                                int hours = jobInfo.StartTime.Hour;
                                int minutes = jobInfo.StartTime.Minute;
                                int second = jobInfo.StartTime.Second;

                                //string[] dayOfWeek = jobInfo.DaysofWeek.ToString().Split(',');
                                List<string> dayOfWeek = RCTMJobExecuter.ExtractDaysOfWeek(jobInfo.DaysofWeek.ToString());
                                string scheduledDay = Convert.ToDateTime(jobInfo.NextScheduleTime).DayOfWeek.ToString();
                                DaysOfWeek scheduledDayValue = (DaysOfWeek)Enum.Parse(typeof(DaysOfWeek), scheduledDay.ToLower());
                                for (int day = 0; day < dayOfWeek.Count; day++)
                                {
                                    if (Convert.ToInt32(dayOfWeek[day]).Equals(Convert.ToInt32(scheduledDayValue)))
                                    {
                                        if (day < dayOfWeek.Count - 1)
                                        {
                                            int noOfDayToAdd = Convert.ToInt32(dayOfWeek[day + 1]) - Convert.ToInt32(dayOfWeek[day]);
                                            nextScheduledTime = Convert.ToDateTime(Convert.ToDateTime(jobInfo.NextScheduleTime).ToShortDateString()) + new TimeSpan(noOfDayToAdd, hours, minutes, second);
                                        }
                                        else
                                        {
                                            nextScheduledTime = Convert.ToDateTime(Convert.ToDateTime(jobInfo.NextScheduleTime).ToShortDateString()).Subtract(new TimeSpan(Convert.ToInt32(dayOfWeek[day]) - Convert.ToInt32(dayOfWeek[0]), 0, 0, 0));
                                            nextScheduledTime = nextScheduledTime.AddDays(weekInterval * 7);
                                            nextScheduledTime = nextScheduledTime + new TimeSpan(hours, minutes, second);
                                        }
                                    }
                                }
                                break;
                            case "monthly":

                                //int year = jobInfo.StartDate.Year;
                                //int month = jobInfo.StartDate.Month;
                                //totalDays = System.DateTime.DaysInMonth(year, month);
                                nextScheduledTime = nextScheduledTime.AddMonths(jobInfo.MonthInterval);
                                break;
                            case "default":
                                throw new Exception("Enter correct Recurrance Pattern");
                        }
                        htParamNoOfRuns.Clear();
                        htParamNoOfRuns.Add("job_id", jobInfo.JobID);
                        dsTemp = dbConnection.ExecuteQuery("RAD:ActualNoOfRuns",
                                                                    htParamNoOfRuns, true);
                    }

                    if (!jobInfo.NoEndDate)
                    {
                        if (nextScheduledTime.Date.CompareTo(jobInfo.EndDate.Date) > 0)
                        {
                            jobInfo.NextScheduleTime = jobInfo.StartTime;
                            SetNextScheduledTime(jobInfo, jobInfo.NextScheduleTime, dbConnection);
                            return;
                        }
                        else
                        {
                            SetNextScheduledTime(jobInfo, nextScheduledTime, dbConnection);
                            return;
                        }

                    }
                    else
                    {
                        SetNextScheduledTime(jobInfo, nextScheduledTime, dbConnection);
                    }
                }
            }

            catch (RDALException rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw rdEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message.ToString());
                throw ex;
            }
            finally
            {
                if (htParams != null)
                    htParams = null;
                if (htParamNoOfRuns != null)
                    htParamNoOfRuns = null;
            }
            mLogger.Debug("RSchedulerDAL :GetNextScheduledTime ->End getting the next scheduled" +
                                                                        " time for the job");
        }

        internal void UpdateJobInfo(int jobId)
        {
            RHashlist htParams = new RHashlist();
            RDBConnectionManager dbConnection = null;
            mLogger.Debug("RSchedulerDAL:UpdateJobInfo -> Start updating next scheduled time for job id " + jobId);
            lock (objLock)
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                dbConnection.UseTransaction = true;
                RCTMScheduledJobInfo jobInfo = GetScheduledJobById(jobId, dbConnection);
                try
                {
                    AddRunningJob(jobInfo, dbConnection);
                    UpdateNextScheduledTime(jobInfo, dbConnection);
                    dbConnection.CommitTransaction();
                    mLogger.Debug("RSchedulerDAL:UpdateJobInfo -> COmpleted updating next scheduled time for job id " + jobId);
                }
                catch (Exception ex)
                {
                    dbConnection.RollbackTransaction();
                    throw ex;
                }
                finally
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                }
            }
        }

        internal void UpdateNextScheduledTime(RCTMScheduledJobInfo jobInfo, RDBConnectionManager dbConnection)
        {
            mLogger.Debug("RSchedulerDAL :GetNextScheduledTime ->Start getting the next scheduled" +
                                                                            " time for the job");
            RHashlist htParamNoOfRuns = null;
            DataSet dsTemp = null;
            bool recurrenceType = false;
            string recurrancePattern = null;
            DateTime nextScheduledTime = jobInfo.NextScheduleTime;
            int dayInterval = 0;
            int weekInterval = 0;
            int monthInterval = 0;
            //int totalDays = 0;
            TimeSpan diffResult;
            bool res = false;
            RHashlist htRes = new RHashlist();
            RHashlist htParams = new RHashlist();
            //RDBConnectionManager dbConnection = null;
            try
            {
                htParams = new RHashlist();
                htParamNoOfRuns = new RHashlist();
                recurrenceType = (bool)jobInfo.RecurrenceType;
                recurrancePattern = jobInfo.RecurrencePattern;
                dayInterval = (int)jobInfo.DaysInterval;
                weekInterval = (int)jobInfo.WeekInterval;
                monthInterval = (int)jobInfo.MonthInterval;

                if (!jobInfo.RecurrenceType)
                {
                    SetNextScheduledTime(jobInfo, jobInfo.NextScheduleTime, dbConnection);
                    return;
                }

                if (jobInfo.NoOfRuns > 0)
                {
                    nextScheduledTime = nextScheduledTime.AddMinutes(Convert.ToDouble
                                                            (jobInfo.TimeIntervalOfRecurrence));
                    htParams.Clear();
                    htParams.Add("next_scheduled_time", nextScheduledTime);
                    htParams.Add("job_id", jobInfo.JobID);
                    dsTemp = dbConnection.ExecuteQuery("RAD:updateNoOfRuns", htParams, true);
                    diffResult = nextScheduledTime.Subtract(jobInfo.NextScheduleTime);
                    if (!(diffResult.Days >= 1))
                    {
                        res = CheckActualNoOfRuns(jobInfo, dbConnection);
                        if (res)
                            goto Update;
                        else
                        {
                            SetNextScheduledTime(jobInfo, nextScheduledTime, dbConnection);
                            return;
                        }
                    }
                }

            Update:
                if (recurrenceType.Equals(true))
                {
                    TimeSpan startTime;
                    string actualDate = null;
                    actualDate = DateTime.Parse(jobInfo.NextScheduleTime.ToString()).
                                                                             ToShortDateString();
                    startTime = jobInfo.StartTime.TimeOfDay;
                    nextScheduledTime = DateTime.Parse(actualDate + " " + startTime.ToString());
                    switch (recurrancePattern.ToLower())
                    {
                        case "daily":
                            nextScheduledTime = nextScheduledTime.AddDays(Convert.ToDouble(jobInfo.DaysInterval));
                            break;
                        case "weekly":
                            int hours = jobInfo.StartTime.Hour;
                            int minutes = jobInfo.StartTime.Minute;
                            int second = jobInfo.StartTime.Second;

                            //string[] dayOfWeek = jobInfo.DaysofWeek.ToString().Split(',');
                            List<string> dayOfWeek = RCTMJobExecuter.ExtractDaysOfWeek(jobInfo.DaysofWeek.ToString());
                            string scheduledDay = Convert.ToDateTime(jobInfo.NextScheduleTime).DayOfWeek.ToString();
                            DaysOfWeek scheduledDayValue = (DaysOfWeek)Enum.Parse(typeof(DaysOfWeek), scheduledDay.ToLower());
                            for (int day = 0; day < dayOfWeek.Count; day++)
                            {
                                if (Convert.ToInt32(dayOfWeek[day]).Equals(Convert.ToInt32(scheduledDayValue)))
                                {
                                    if (day < dayOfWeek.Count - 1)
                                    {
                                        int noOfDayToAdd = Convert.ToInt32(dayOfWeek[day + 1]) - Convert.ToInt32(dayOfWeek[day]);
                                        nextScheduledTime = Convert.ToDateTime(Convert.ToDateTime(jobInfo.NextScheduleTime).ToShortDateString()) + new TimeSpan(noOfDayToAdd, hours, minutes, second);
                                    }
                                    else
                                    {
                                        nextScheduledTime = Convert.ToDateTime(Convert.ToDateTime(jobInfo.NextScheduleTime).ToShortDateString()).Subtract(new TimeSpan(Convert.ToInt32(dayOfWeek[day]) - Convert.ToInt32(dayOfWeek[0]), 0, 0, 0));
                                        nextScheduledTime = nextScheduledTime.AddDays(weekInterval * 7);
                                        nextScheduledTime = nextScheduledTime + new TimeSpan(hours, minutes, second);
                                    }
                                }
                            }
                            break;
                        case "monthly":

                            //int year = jobInfo.StartDate.Year;
                            //int month = jobInfo.StartDate.Month;
                            //totalDays = System.DateTime.DaysInMonth(year, month);
                            nextScheduledTime = nextScheduledTime.AddMonths(jobInfo.MonthInterval);
                            break;
                        case "default":
                            throw new Exception("Enter correct Recurrance Pattern");
                    }
                    htParamNoOfRuns.Clear();
                    htParamNoOfRuns.Add("job_id", jobInfo.JobID);
                    dsTemp = dbConnection.ExecuteQuery("RAD:ActualNoOfRuns",
                                                                htParamNoOfRuns, true);
                }

                if (!jobInfo.NoEndDate)
                {
                    if (nextScheduledTime.Date.CompareTo(jobInfo.EndDate.Date) > 0)
                    {
                        jobInfo.NextScheduleTime = jobInfo.StartTime;
                        SetNextScheduledTime(jobInfo, jobInfo.NextScheduleTime, dbConnection);
                        return;
                    }
                    else
                    {
                        SetNextScheduledTime(jobInfo, nextScheduledTime, dbConnection);
                        return;
                    }

                }
                else
                {
                    SetNextScheduledTime(jobInfo, nextScheduledTime, dbConnection);
                }
            }

            catch (RDALException rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new Exception();//RSchedulerException(rdEx.Message, rdEx);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message.ToString());
                throw new Exception();// RSchedulerException(ex.Message.ToString(), ex);
            }
            finally
            {
                if (htParams != null)
                    htParams = null;
                if (htParamNoOfRuns != null)
                    htParamNoOfRuns = null;
            }
            mLogger.Debug("RSchedulerDAL :GetNextScheduledTime ->End getting the next scheduled" +
                                                                        " time for the job");
        }

        internal void AddRunningJob(RCTMScheduledJobInfo jobInfo, RDBConnectionManager dbConnection)
        {
            mLogger.Debug("Start->InsertSchedulerRunningJobs");
            RHashlist htParams = new RHashlist();
            //RDBConnectionManager dbConnection = null;
            RHashlist hList = new RHashlist();
            hList.Add("running_job_id", jobInfo.JobID);
            hList.Add("start_time", jobInfo.NextScheduleTime);
            hList.Add("job_status", "STARTED");
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                dbConnection.ExecuteQuery("RAD:InsertSchedulerRunningJobs", hList, true);
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new Exception();// RSchedulerException(dalEx.ToString(), dalEx);
            }
            finally
            {
                hList.Clear();
                hList = null;
                mLogger.Debug("End->InsertSchedulerRunningJobs");
            }
        }

        internal RCTMScheduledJobInfo GetScheduledJobById(int jobID, RDBConnectionManager dbConnection)
        {
            RHashlist htParams = new RHashlist();
            // RDBConnectionManager dbConnection = null;
            mLogger.Debug("RSchedulerDAL :GetScheduledJobById ->Start getting scheduled job by ID");
            RCTMScheduledJobInfo objScheduledInfo = null;
            DataSet dsJobInfo = null;
            try
            {
                htParams = new RHashlist();
                htParams.Add("job_id", jobID);
                dsJobInfo = dbConnection.ExecuteQuery("RAD:GetScheduledJobById", htParams, true);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message.ToString());
                throw ex;
            }
            finally
            {
                if (htParams != null)
                    htParams = null;
            }
            if (dsJobInfo != null && dsJobInfo.Tables[0].Rows.Count > 0)
                objScheduledInfo = FillScheduledInfo(objScheduledInfo, dsJobInfo.Tables[0].Rows[0]);

            mLogger.Debug("RSchedulerDAL :GetScheduledJobById ->End getting scheduled job by ID");

            return objScheduledInfo;
        }

        internal RCTMScheduledJobInfo FillScheduledInfo(RCTMScheduledJobInfo objScheduledInfo, DataRow dsJobInfo)
        {
            objScheduledInfo = new RCTMScheduledJobInfo();
            objScheduledInfo.JobID = (int)dsJobInfo["job_id"];
            objScheduledInfo.SchedulableJobId = (int)dsJobInfo["schedulable_job_id"];
            objScheduledInfo.JobName = (string)dsJobInfo["job_name"];
            objScheduledInfo.JobDescription = ReplaceNullString(dsJobInfo["job_description"]);
            objScheduledInfo.CreationTime = (DateTime)dsJobInfo["creation_time"];
            objScheduledInfo.ModificationTime = (DateTime)dsJobInfo["modification_time"];
            objScheduledInfo.DaysInterval = ReplaceNullInteger(dsJobInfo["day_interval"]);
            objScheduledInfo.WeekInterval = ReplaceNullInteger(dsJobInfo["week_interval"]);
            objScheduledInfo.MonthInterval = ReplaceNullInteger(dsJobInfo["month_interval"]);
            objScheduledInfo.DaysofWeek = ReplaceNullInteger(dsJobInfo["days_of_week"]);
            objScheduledInfo.StartDate = (DateTime)dsJobInfo["start_date"];
            objScheduledInfo.EndDate = string.IsNullOrEmpty(dsJobInfo["end_date"].ToString()) ? new DateTime() : Convert.ToDateTime(dsJobInfo["end_date"].ToString());
            objScheduledInfo.StartTime = (DateTime)dsJobInfo["start_time"];
            objScheduledInfo.NoEndDate = ReplaceNullBool(dsJobInfo["no_end_date"]);
            objScheduledInfo.RecurrenceType = ReplaceNullBool(dsJobInfo["recurrence_type"]);
            objScheduledInfo.RecurrencePattern = ReplaceNullString(dsJobInfo["recurrence_pattern"]);
            objScheduledInfo.NextScheduleTime = (DateTime)dsJobInfo["next_schedule_time"];
            objScheduledInfo.TimeIntervalOfRecurrence = ReplaceNullInteger(dsJobInfo["time_interval_of_recurrence"]);
            objScheduledInfo.NoOfRecurrences = ReplaceNullInteger(dsJobInfo["no_of_recurrences"]);
            objScheduledInfo.NoOfRuns = ReplaceNullInteger(dsJobInfo["no_of_runs"]);
            return objScheduledInfo;
        }
        /// <summary>
        /// Calculate the next Scheduled timwe of the job.
        /// </summary>
        private void SetNextScheduledTime(RCTMScheduledJobInfo jobInfo, DateTime nextScheduledTime, RDBConnectionManager dbConnection)
        {
            RHashlist htParams = new RHashlist();
            // RDBConnectionManager dbConnection = null;
            try
            {
                htParams.Clear();
                htParams.Add("next_schedule_time", nextScheduledTime);
                htParams.Add("job_id", jobInfo.JobID);
                htParams.Add("modification_time", DateTime.Now);
                dbConnection.ExecuteQuery("RAD:updateNextScheduleTime", htParams, true);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Checks the actual no of runs.
        /// </summary>
        /// <param name="jobInfo">The job info.</param>
        /// <returns></returns>
        private bool CheckActualNoOfRuns(RCTMScheduledJobInfo jobInfo, RDBConnectionManager dbConnection)
        {
            RHashlist htParams = new RHashlist();
            // RDBConnectionManager dbConnection = null;
            DataSet ds = null;
            bool result = false;
            ds = new DataSet();
            htParams.Clear();
            htParams.Add("job_id", jobInfo.JobID);
            ds = dbConnection.ExecuteQuery("RAD:CheckActualNoOfRuns", htParams, true);
            if ((ds != null) && (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0))
            {
                int val = int.Parse(ds.Tables[0].Rows[0]["no_of_runs"].ToString());
                if (val == 0)
                    result = true;
                else
                    result = false;
            }
            return result;
        }

        /// <summary>
        /// Gets all jobs to be updated.
        /// </summary>
        /// <returns></returns>
        internal DataSet GetAllJobsToBeUpdated()
        {
            lock (objLock)
            {
                RHashlist htParams = new RHashlist();
                RDBConnectionManager dbConnection = null;
                mLogger.Debug("RSchedulerDAL :GetAllJobsToBeUpdated ->Start getting all scheduled jobs to be updated for next scheduled time");
                DataSet dsTemp = null;
                try
                {
                    dsTemp = new DataSet();
                    dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                    dsTemp = dbConnection.ExecuteQuery("RAD:GetAllJobsToBeUpdated", htParams, true);
                }
                catch (RDALException rdEx)
                {
                    mLogger.Error(rdEx.ToString());
                    throw rdEx;
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.Message.ToString());
                    throw ex;
                }
                finally
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                    if (htParams != null)
                        htParams = null;
                }
                mLogger.Debug("RSchedulerDAL :GetAllScheduledJobs ->End getting all scheduled jobs");
                return dsTemp;
            }
        }

        /// <summary>
        /// Updates the job for next schedule time.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="jobId">The job id.</param>
        internal void UpdateJobForNextScheduleTime(List<string> parameters, int jobId)
        {
            lock (objLock)
            {
                RHashlist htParams = new RHashlist();
                RDBConnectionManager dbConnection = null;
                mLogger.Debug("RSchedulerDAL:UpdateJobForNextScheduleTime -> Start modifying the job for next scheduled time");
                try
                {
                    htParams = new RHashlist();
                    dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                    htParams.Add("NextScheduleTime", Convert.ToDateTime(parameters[0]));
                    htParams.Add("NoOfRuns", Convert.ToInt32(parameters[1]));
                    htParams.Add("JobId", jobId);

                    dbConnection.ExecuteQuery("RAD:ModifyScheduledJobByJobIDForNextScheduledTime", htParams, true);
                    htParams.Clear();
                }
                catch (RDALException rdEx)
                {
                    mLogger.Error(rdEx.ToString());
                    throw rdEx;
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.Message.ToString());
                    throw ex;
                }
                finally
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                    if (htParams != null)
                        htParams = null;
                }
                mLogger.Debug("RSchedulerDAL:UpdateJobForNextScheduleTime -> End modifying the job");
            }
        }

        /// <summary>
        /// Updates the job for next schedule time.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="jobId">The job id.</param>
        internal void UpdateJobForNextScheduleTime(List<string> parameters, int jobId, RDBConnectionManager dbConnection)
        {
            lock (objLock)
            {
                RHashlist htParams = new RHashlist();
               // RDBConnectionManager dbConnection = null;
                mLogger.Debug("RSchedulerDAL:UpdateJobForNextScheduleTime -> Start modifying the job for next scheduled time");
                try
                {
                    htParams = new RHashlist();
                   // dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager("CTMDB");
                    htParams.Add("NextScheduleTime", Convert.ToDateTime(parameters[0]));
                    htParams.Add("NoOfRuns", Convert.ToInt32(parameters[1]));
                    htParams.Add("JobId", jobId);

                    dbConnection.ExecuteQuery("RAD:ModifyScheduledJobByJobIDForNextScheduledTime", htParams, true);
                    htParams.Clear();
                }
                catch (RDALException rdEx)
                {
                    mLogger.Error(rdEx.ToString());
                    throw rdEx;
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.Message.ToString());
                    throw ex;
                }
                finally
                {
                   // RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                    if (htParams != null)
                        htParams = null;
                }
                mLogger.Debug("RSchedulerDAL:UpdateJobForNextScheduleTime -> End modifying the job");
            }
        }

        #region class Private methods
        /// <summary>
        /// Replaces the null string.
        /// </summary>
        private string ReplaceNullString(object obj)
        {
            if (obj.ToString() == "")
                return "";
            return obj.ToString();
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Replaces the null integer.
        /// </summary>
        private int ReplaceNullInteger(object obj)
        {
            if (obj.ToString() == "")
                return 0;
            return Convert.ToInt32(obj);
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Replaces the null bool.
        /// </summary>
        private bool ReplaceNullBool(object obj)
        {
            if (obj.ToString() == "")
                return false;
            return Convert.ToBoolean(obj);
        }
        #endregion
    }
}
