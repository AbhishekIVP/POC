using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public class RMPreferenceDBManager
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMPreferenceDBManager");
        private RDBConnectionManager mDBCon = null;

        public RMPreferenceDBManager(RDBConnectionManager conMgr)
        {
            this.mDBCon = conMgr;
        }
        public ObjectSet GetRealTimePreferences(int moduleId, string entityTypeIds)
        {
            try
            {
                mLogger.Debug("RMPreferenceDBManager -> GetRealTimePreferences -> Start");

                ObjectSet oSet = null;
                string queryText = "EXEC [dbo].[REFM_GetEntityTypePreferences] " + moduleId + ",'" + entityTypeIds + "'";

                if (this.mDBCon != null)
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                else
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);

                return oSet;
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMPreferenceDBManager -> GetRealTimePreferences -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMPreferenceDBManager -> GetRealTimePreferences -> End");
            }
        }
    }
}
