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
    public class RMTimeSeriesController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMTimeSeriesController");

        public ObjectSet GetTimeSeriesTaskConfiguration(List<int> entityTypeIds, int moduleID, bool getRawData, RDBConnectionManager mDBCon = null)
        {
            try
            {
                mLogger.Debug("RMTimeSeriesController -> GetTimeSeriesTaskConfiguration -> Start");
                ObjectSet oSet = new RMTimeSeriesDBManager(mDBCon).GetTimeSeriesTaskConfiguration(entityTypeIds, moduleID);

                if (oSet != null && oSet.Tables.Count > 0)
                {
                    oSet.Tables[0].TableName = "Definition";
                }

                if (!getRawData)
                {
                    RemoveUnNecessaryColumns(oSet);
                }

                return oSet;
            }
            catch
            {
                mLogger.Debug("RMTimeSeriesController -> GetTimeSeriesTaskConfiguration -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMTimeSeriesController -> GetTimeSeriesTaskConfiguration -> End");
            }
        }


        private void RemoveUnNecessaryColumns(ObjectSet oSet)
        {
            Dictionary<int, List<string>> dictColumnsToRemove = new Dictionary<int, List<string>>();

            //Populate columns to remove
            dictColumnsToRemove.Add(0, new List<string>() { RMModelerConstants.ENTITY_TYPE_ID, "entity_display_name", "leg_name", "task_name", "time_series_id" });

            foreach (KeyValuePair<int, List<string>> kvp in dictColumnsToRemove)
            {
                kvp.Value.ForEach(col =>
                {
                    if (oSet.Tables[kvp.Key].Columns.Contains(col))
                        oSet.Tables[kvp.Key].Columns.Remove(col);
                });
            }
        }
    }
}
