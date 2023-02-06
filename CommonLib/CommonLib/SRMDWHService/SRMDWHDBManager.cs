using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.srmdwhjob
{
    class SRMDWHDBManager 
    {
        static object objLock = new object();
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMDWHDBManager");
        RHashlist htParams = null;

        /// <summary>
        /// Gets all runnable jobs.
        /// </summary>
        /// <returns></returns>
        internal DataSet GetAllRunnableJobs(int interval)
        {
            lock (objLock)
            {
                mLogger.Debug("SRMDWHDBManager :GetAllScheduledJobs ->Start getting all scheduled jobs");
                DataSet dsTemp = null;
                string scheduledJobInterval = RConfigReader.GetConfigAppSettings("ScheduledJobInterval");
                int timeInt = 0;
                try
                {
                    if (scheduledJobInterval != string.Empty)
                        timeInt = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(scheduledJobInterval) * -3).Minutes;

                    if (interval != 0)
                        timeInt = interval;

                    dsTemp = new DataSet();
                    htParams = new RHashlist();
                    htParams.Add("mInterval", timeInt);
                    dsTemp = CommonDALWrapper.ExecuteSelectQuery("SRM:GetRunnableJobs", htParams, ConnectionConstants.RefMaster_Connection);
                }
                catch (RDALException rdEx)
                {
                    mLogger.Error(rdEx.ToString());
                    throw;
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.Message.ToString());
                    throw;
                }
                finally
                {
                    if (htParams != null)
                        htParams = null;
                }
                mLogger.Debug("SRMDWHDBManager:GetAllScheduledJobs ->End getting all scheduled jobs");
                return dsTemp;
            }
        }

        internal DataSet GetAllJobsToBeUpdated()
        {
            lock (objLock)
            {
                mLogger.Debug("SRMDWHDBManager:GetAllJobsToBeUpdated ->Start getting all scheduled jobs to be updated for next scheduled time");
                DataSet dsTemp = null;
                try
                {
                    dsTemp = new DataSet();
                    dsTemp = CommonDALWrapper.ExecuteSelectQuery("SRM:GetAllJobsToBeUpdated", htParams, ConnectionConstants.RefMaster_Connection);
                }
                catch (RDALException rdEx)
                {
                    mLogger.Error(rdEx.ToString());
                    throw;
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.Message.ToString());
                    throw;
                }
                finally
                {
                    if (htParams != null)
                        htParams = null;
                }
                mLogger.Debug("SRMDWHDBManager:GetAllScheduledJobs ->End getting all scheduled jobs");
                return dsTemp;
            }
        }

        internal void UpdateJobInfo(int jobId)
        {
            RHashlist htParams = new RHashlist();
            RDBConnectionManager dbConnection = null;
            mLogger.Debug("SRMDWHDBManager:UpdateJobInfo -> Start updating next scheduled time for job id " + jobId);
            lock (objLock)
            {
                dbConnection = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.Serializable);
                
                SRMDWHScheduledJobInfo jobInfo = GetScheduledJobById(jobId, dbConnection);
                try
                {
                    //AddRunningJob(jobInfo, dbConnection);
                    UpdateNextScheduledTime(jobInfo, dbConnection);
                    dbConnection.CommitTransaction();
                    mLogger.Debug("SRMDWHDBManager:UpdateJobInfo -> Completed updating next scheduled time for job id " + jobId);
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

        internal SRMDWHScheduledJobInfo GetScheduledJobById(int jobID, RDBConnectionManager dbConnection)
        {
            RHashlist htParams = new RHashlist();
            mLogger.Debug("SRMDWHDBManager :GetScheduledJobById ->Start getting scheduled job by ID");
            SRMDWHScheduledJobInfo objScheduledInfo = null;
            DataSet dsJobInfo = null;
            try
            {
                htParams = new RHashlist();
                htParams.Add("job_id", jobID);
                dsJobInfo = dbConnection.ExecuteQuery("SRM:GetScheduledJobById", htParams, true);
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

            mLogger.Debug("SRMDWHDBManager :GetScheduledJobById ->End getting scheduled job by ID");

            return objScheduledInfo;
        }

        internal SRMDWHScheduledJobInfo FillScheduledInfo(SRMDWHScheduledJobInfo objScheduledInfo, DataRow dsJobInfo)
        {
            objScheduledInfo = new SRMDWHScheduledJobInfo();
            objScheduledInfo.JobID = (int)dsJobInfo["job_id"];
            //objScheduledInfo.SchedulableJobId = (int)dsJobInfo["schedulable_job_id"];
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

        internal void UpdateNextScheduledTime(SRMDWHScheduledJobInfo jobInfo, RDBConnectionManager dbConnection)
        {
            mLogger.Debug("SRMDWHDBManager :GetNextScheduledTime ->Start getting the next scheduled" +
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
                    SetNextScheduledTime(jobInfo, jobInfo.NextScheduleTime.AddMinutes(-1), dbConnection);
                    return;
                }

                if (jobInfo.NoOfRuns > 0)
                {
                    nextScheduledTime = nextScheduledTime.AddMinutes(Convert.ToDouble
                                                            (jobInfo.TimeIntervalOfRecurrence));
                    htParams.Clear();
                    htParams.Add("next_scheduled_time", nextScheduledTime);
                    htParams.Add("job_id", jobInfo.JobID);
                    dsTemp = dbConnection.ExecuteQuery("SRM:UpdateNoOfRuns", htParams, true);
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

                            List<string> dayOfWeek = SRMDWHJobExecuter.ExtractDaysOfWeek(jobInfo.DaysofWeek.ToString());
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
                    dsTemp = dbConnection.ExecuteQuery("SRM:ActualNoOfRuns",
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
            mLogger.Debug("SRMDWHDBManager :GetNextScheduledTime ->End getting the next scheduled" +
                                                                        " time for the job");
        }

        private bool CheckActualNoOfRuns(SRMDWHScheduledJobInfo jobInfo, RDBConnectionManager dbConnection)
        {
            RHashlist htParams = new RHashlist();
            DataSet ds = null;
            bool result = false;
            ds = new DataSet();
            htParams.Clear();
            htParams.Add("job_id", jobInfo.JobID);
            ds = dbConnection.ExecuteQuery("SRM:CheckActualNoOfRuns", htParams, true);
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

        private void SetNextScheduledTime(SRMDWHScheduledJobInfo jobInfo, DateTime nextScheduledTime, RDBConnectionManager dbConnection)
        {
            RHashlist htParams = new RHashlist();
            // RDBConnectionManager dbConnection = null;
            try
            {
                htParams.Clear();
                htParams.Add("next_schedule_time", nextScheduledTime);
                htParams.Add("job_id", jobInfo.JobID);
                htParams.Add("modification_time", DateTime.Now);
                dbConnection.ExecuteQuery("SRM:updateNextScheduleTime", htParams, true);
            }
            catch
            {

            }
        }

        //internal void UpdateJobForNextScheduleTime(List<string> parameters, int jobId)
        //{
        //    lock (objLock)
        //    {
        //        mLogger.Debug("SRMDWHDBManager:UpdateJobForNextScheduleTime -> Start modifying the job for next scheduled time");
        //        try
        //        {
        //            htParams = new RHashlist();
        //            //dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
        //            htParams.Add("NextScheduleTime", Convert.ToDateTime(parameters[0]));
        //            htParams.Add("NoOfRuns", Convert.ToInt32(parameters[1]));
        //            htParams.Add("JobId", jobId);

        //            //dbConnection.ExecuteQuery("RAD:ModifyScheduledJobByJobIDForNextScheduledTime", htParams, true);
        //            CommonDALWrapper.ExecuteSelectQuery("RAD:ModifyScheduledJobByJobIDForNextScheduledTime", htParams, DBConnectionId);

        //            htParams.Clear();
        //        }
        //        catch (RDALException rdEx)
        //        {
        //            mLogger.Error(rdEx.ToString());
        //            throw;
        //        }
        //        catch (Exception ex)
        //        {
        //            mLogger.Error(ex.Message.ToString());
        //            throw;
        //        }
        //        finally
        //        {
        //            //RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
        //            if (htParams != null)
        //                htParams = null;
        //        }
        //        mLogger.Debug("SRMDWHDBManager:UpdateJobForNextScheduleTime -> End modifying the job");
        //    }
        //}

        internal void UpdateJobForNextScheduleTime(List<string> parameters, int jobId, RDBConnectionManager dbConnection)
        {
            lock (objLock)
            {
                RHashlist htParams = new RHashlist();
                mLogger.Debug("SRMDWHDBManager:UpdateJobForNextScheduleTime -> Start modifying the job for next scheduled time");
                try
                {
                    htParams = new RHashlist();
                    htParams.Add("NextScheduleTime", Convert.ToDateTime(parameters[0]));
                    htParams.Add("NoOfRuns", Convert.ToInt32(parameters[1]));
                    htParams.Add("JobId", jobId);
                    CommonDALWrapper.ExecuteSelectQuery("SRM:ModifyScheduledJobByJobIDForNextScheduledTime", htParams, dbConnection);
                    htParams.Clear();
                }
                catch (RDALException rdEx)
                {
                    mLogger.Error(rdEx.ToString());
                    throw;
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.Message.ToString());
                    throw;
                }
                finally
                {
                    if (htParams != null)
                        htParams = null;
                }
                mLogger.Debug("SRMDWHDBManager:UpdateJobForNextScheduleTime -> End modifying the job");
            }
        }

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
    }
}
