using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public class RMTimeSeriesDBManager
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMTimeSeriesDBManager");
        private RHashlist mList = null;
        private RDBConnectionManager mDBCon = null;

        public RMTimeSeriesDBManager(RDBConnectionManager conMgr)
        {
            this.mDBCon = conMgr;
        }

        public ObjectSet GetTimeSeriesTaskConfiguration(List<int> entityTypeIds, int moduleID)
        {
            try
            {
                mLogger.Debug("RMTimeSeriesDBManager -> GetTimeSeriesTaskConfiguration -> Start");

                string entity_type_ids = string.Empty;
                ObjectSet oSet = null;

                if (entityTypeIds != null && entityTypeIds.Count > 0)
                {
                    entity_type_ids = string.Join(",", entityTypeIds);
                }

                string queryText = "EXEC [IVPRefMasterVendor].[dbo].[REFM_GetTimeSeriesTaskData] '" + entity_type_ids + "', " + moduleID + " ";
                string source = string.Empty;


                if (this.mDBCon != null)
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                else
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMasterVendor_Connection);

                return oSet;
            }
            catch
            {
                mLogger.Debug("RMTimeSeriesDBManager -> GetTimeSeriesTaskConfiguration -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMTimeSeriesDBManager -> GetTimeSeriesTaskConfiguration -> End");
            }
        }
    }
}
