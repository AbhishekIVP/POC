using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.utils;
using com.ivp.srm.dwhdownstream;
using com.ivp.srmcommon;
using DWHAdapter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Timers;
using System.Xml;
using System.Xml.Linq;

namespace com.ivp.common.srmdwhjob
{
    public class SRMDWHService
    {
        static bool RunScheduler = true;
        static IRLogger mLogger;
        static SRMDWHJobExecuter mJobExecutor = null;
        public static ConcurrentDictionary<string, bool> ErrorOccurredWhileTriggeringScheduledDWHJob = new ConcurrentDictionary<string, bool>();
        

        public static void Main(string[] args)
        {
            CultureInfo culture = null;
            culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            try
            {
                string loggingPath = string.Empty;
                mLogger = SRMMTCommon.CreateJobLogger(SRMMTConfig.isMultiTenantEnabled, string.Empty, "SRMDWHService", string.Empty, out loggingPath, string.Empty);
                SRMDWHService dwh = new SRMDWHService();
                mLogger.Debug("SRMDWHService:Ctor-->>SRMDWHService started");

                mLogger.Debug("SRMDWHService:Ctor-->>created new instance of mJobExecuter");

                //Thread.Sleep(40000);

                mJobExecutor = new SRMDWHJobExecuter();

                SRMMTConfig.MethodCallPerClientSequentially(SetInfo);

                dwh.HostService();

                DWHAdapterController.RegisterDWHAdapter(0, true);

                string scheduledJobInterval = RConfigReader.GetConfigAppSettings("ScheduledJobInterval");
                scheduledJobInterval = scheduledJobInterval ?? "20000";
                System.Timers.Timer mTimer = new System.Timers.Timer(double.Parse(scheduledJobInterval));
                mTimer.Elapsed += new ElapsedEventHandler(dwh.timer_Elapsed);
                mTimer.Start();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMDWHService:ctor-->>Error while creating new instance of SRMDWHService");
                mLogger.Error(ex);
                Environment.ExitCode = 1;
            }
        }

        private static void SetInfo(string clientName)
        {
            SRMDWHService dwh = new SRMDWHService();
            if (!ErrorOccurredWhileTriggeringScheduledDWHJob.ContainsKey(clientName))
                ErrorOccurredWhileTriggeringScheduledDWHJob.TryAdd(clientName, false);

            if (!SRMDWHStatic.SetupVsLastRolledTime.ContainsKey(clientName))
                SRMDWHStatic.SetupVsLastRolledTime.TryAdd(clientName, new Dictionary<int, RollingSetupInfo>());

            string query = @"SELECT DISTINCT connection_name FROM dbo.ivp_srm_dwh_downstream_master WHERE is_active = 1 
                                SELECT mas.setup_id, mas.setup_name, mas.connection_name, MAX(rolling_time) AS rolling_time
                                    FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master mas
                                    LEFT JOIN IVPRefMaster.dbo.ivp_srm_dwh_rolling_time rol
                                    ON (mas.setup_id = rol.setup_id)
                                    WHERE mas.is_active = 1
                                    GROUP BY mas.setup_id,mas.setup_name, mas.connection_name";
            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string dbName = Convert.ToString(dr[0]);

                    mLogger.Error($"DBNAME : {dbName}");
                    new SRMDWHJob().CreateObjects(dbName);
                    SRMDWHJob.CheckForHostedSetup(dbName, true, true, true);
                }
            }
            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    DateTime? lastRollTime = null;
                    if (!string.IsNullOrEmpty(Convert.ToString(dr["rolling_time"])))
                        lastRollTime = Convert.ToDateTime(dr["rolling_time"]);


                    SRMDWHStatic.SetupVsLastRolledTime[clientName].Add(Convert.ToInt32(dr["setup_id"]), new RollingSetupInfo()
                    {
                        ConnectionName = Convert.ToString(dr["connection_name"]),
                        SetupId = Convert.ToInt32(dr["setup_id"]),
                        SetupName = Convert.ToString(dr["setup_name"]),
                        LastRollingTime = lastRollTime
                    });
                }
            }

            FailRunningDWHSyncProcess();

            var isSuccess = dwh.UpdateNextScheduledTime();
            if (!isSuccess)
                ErrorOccurredWhileTriggeringScheduledDWHJob[clientName] = true;


        }

        private static void FailRunningDWHSyncProcess()
        {
            string sqlQuery = @"SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master WHERE is_active = 1; 
                            UPDATE IVPRefMaster.dbo.ivp_srm_dwh_downstream_setup_status SET status = 'FAILED' WHERE status IN ('INPROGRESS','QUEUED')";
            DataSet dwhSetup = CommonDALWrapper.ExecuteSelectQuery(sqlQuery, ConnectionConstants.RefMaster_Connection);

            SRMDWHJob job = new SRMDWHJob();

            if (dwhSetup != null && dwhSetup.Tables.Count > 0 && dwhSetup.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dwhSetup.Tables[0].Rows)
                {
                    int setupId = Convert.ToInt32(dr["setup_id"]);
                    string downstreamConnectionName = Convert.ToString(dr["connection_name"]);
                    job.CheckIfProcessCanExecute(false, setupId, ref downstreamConnectionName, true);
                    job.ClearStagingTables(downstreamConnectionName);
                }
            }
        }


        private void HostService()
        {
            ServiceHost host = new ServiceHost(typeof(SRMDWHAPI));
            host.Open();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                SRMMTConfig.MethodCallPerClient(timer_ElapsetMT);
            }
            catch (Exception ee)
            {
                mLogger.Error("ERROR in Polling timer_Elapsed thread" + ee.ToString());
            }
        }

        private static void timer_ElapsetMT(string clientName)
        {
            if (ErrorOccurredWhileTriggeringScheduledDWHJob[clientName])
            {
                var isSuccess = new SRMDWHService().UpdateNextScheduledTime();
                if (isSuccess)
                    ErrorOccurredWhileTriggeringScheduledDWHJob[clientName] = false;
            }
            mJobExecutor.RunScheduledJobs(clientName);
            timer_Elapsed_rolling(clientName);
        }

        private static void timer_Elapsed_rolling(string clientName)
        {
            try
            {
                string DWHRollingStartTime = SRMCommon.GetConfigFromDB("DWHRollingStartTime");

                string configTime = "11:45 PM";
                if (!string.IsNullOrEmpty(DWHRollingStartTime))
                    configTime = DWHRollingStartTime;
                DateTime currentTime = DateTime.Now;
                DateTime configuredTime;

                var yesterdayDate = currentTime.Date.AddDays(-1);
                foreach (var info in SRMDWHStatic.SetupVsLastRolledTime[clientName])
                {
                    bool rollTillNextDay = true;

                    if (info.Value.LastRollingTime.HasValue && info.Value.LastRollingTime.Value.Date.AddDays(1) <= yesterdayDate)
                    {
                        rollTillNextDay = false;
                        SRMDWHJobQueue.EnqueueRolling(clientName, new RollingInfo
                        {
                            SetupId = info.Value.SetupId,
                            ConnectionName = info.Value.ConnectionName,
                            TimeToRun = DateTime.Now,
                            T_plus_1 = false
                        });
                    }

                    if (rollTillNextDay)
                    {
                        if (DateTime.TryParse(configTime, out configuredTime) && currentTime < configuredTime)
                            mLogger.Debug(string.Format("Cannot roll data for setup : {0} since the current time ({1}) is less than : {2}", info.Value.SetupName, currentTime.TimeOfDay, configTime));
                        else
                        {
                            SRMDWHJobQueue.EnqueueRolling(clientName, new RollingInfo
                            {
                                SetupId = info.Value.SetupId,
                                ConnectionName = info.Value.ConnectionName,
                                TimeToRun = configuredTime,
                                T_plus_1 = true
                            });
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("ERROR in Polling timer_Elapsed_rolling thread" + ee.ToString());
            }
        }

        private bool UpdateNextScheduledTime()
        {
            try
            {
                mLogger.Debug("SRMDWHService-->> Update Next scheduled times on scheduler restart");
                mJobExecutor.UpdateNextScheduledTime();
                mLogger.Debug("SRMDWHService-->> Updated Next scheduled times on scheduler restart");
                return true;
            }
            catch (Exception ee)
            {
                mLogger.Error("ERROR while updating next scheduled time : " + ee.ToString());
                return false;
            }
        }

    }
}
