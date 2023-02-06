using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using com.ivp.commom.commondal;
using com.ivp.rad.common;
using Newtonsoft.Json;

namespace CommonService
{
   public class DBUtilityService
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("Common.DBUtilityService");
        private static Timer timer = new Timer(30000);

        //public DBUtilityService()
        //{
        //    timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        //    timer.Start();
        //}

        static DBUtilityService()
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }
        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            mLogger.Debug("timer_Elapsed->Start running all Service");
            try
            {
                DeadLockTrace();
                RecentExpensiveQueries();
            }
            catch (Exception ex)
            {
                mLogger.Error("ERROR in timer_Elapsed" + ex.ToString());
            }
            mLogger.Debug("timer_Elapsed->End running all Service");
        }
        static void DeadLockTrace()
        {
            mLogger.Debug("DeadLockTrace->Start running all DeadLock Service");
            try
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMaster.dbo.Refm_GetDeadLock"), ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            mLogger.Debug("DeadLockTrace->End running all DeadLock Service");
        }

        static void RecentExpensiveQueries()
        {
            mLogger.Debug("RecentExpensiveQueries->Start running all RecentExpensiveQueries Service");
            try
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMaster.dbo.Refm_GetExpensiveQueries"), ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            mLogger.Debug("RecentExpensiveQueries->End running all RecentExpensiveQueries Service");
        }
    }
}
