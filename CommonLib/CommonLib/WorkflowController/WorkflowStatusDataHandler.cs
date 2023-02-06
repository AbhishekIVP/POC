using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace com.ivp.common
{
    public static class WorkflowStatusDataHandler
    {
        private static Dictionary<string, Dictionary<string, Dictionary<string, GridData>>> dictGridData = new Dictionary<string, Dictionary<string, Dictionary<string, GridData>>>();

        public static void SetGridData(string cacheKey, WorkflowGridType gridType, string gridId, DataSet gridData)
        {
            string gridTypeStr = gridType.ToString();
            lock (((IDictionary)dictGridData).SyncRoot)
            {
                if (!dictGridData.ContainsKey(cacheKey))
                {
                    dictGridData[cacheKey] = new Dictionary<string, Dictionary<string, GridData>>();
                }
                if (!dictGridData[cacheKey].ContainsKey(gridTypeStr))
                {
                    dictGridData[cacheKey][gridTypeStr] = new Dictionary<string, GridData>();
                }

                dictGridData[cacheKey][gridTypeStr][gridId] = new GridData() { dsGridData = gridData };

                string entityColumn = string.Empty;
                if (gridData.Tables[0].Columns.Contains("Security"))
                    entityColumn = "Security";
                else if (gridData.Tables[0].Columns.Contains("Entity"))
                    entityColumn = "Entity";
                if (!string.IsNullOrEmpty(entityColumn))
                    dictGridData[cacheKey][gridTypeStr][gridId].dictQueueIds = gridData.Tables[0].AsEnumerable().GroupBy(x => Convert.ToString(x[entityColumn])).ToDictionary(x => x.Key, y => string.Join(",", y.Select(a => Convert.ToString(a["Action History"]))));
            }
        }

        public static DataSet GetGridData(string cacheKey, WorkflowGridType gridType, string gridId)
        {
            DataSet ds = null;
            string gridTypeStr = gridType.ToString();
            lock (((IDictionary)dictGridData).SyncRoot)
            {
                if (dictGridData.ContainsKey(cacheKey) && dictGridData[cacheKey].ContainsKey(gridTypeStr) && dictGridData[cacheKey][gridTypeStr].ContainsKey(gridId))
                {
                    ds = dictGridData[cacheKey][gridTypeStr][gridId].dsGridData;
                }
            }
            return ds;
        }

        public static string GetQueueIds(string cacheKey, WorkflowGridType gridType, string gridId, string groupingValue)
        {
            string queueIds = string.Empty;
            string gridTypeStr = gridType.ToString();
            lock (((IDictionary)dictGridData).SyncRoot)
            {
                if (dictGridData.ContainsKey(cacheKey) && dictGridData[cacheKey].ContainsKey(gridTypeStr) && dictGridData[cacheKey][gridTypeStr].ContainsKey(gridId) && dictGridData[cacheKey][gridTypeStr][gridId].dictQueueIds != null && dictGridData[cacheKey][gridTypeStr][gridId].dictQueueIds.ContainsKey(groupingValue))
                {
                    queueIds = dictGridData[cacheKey][gridTypeStr][gridId].dictQueueIds[groupingValue];
                }
            }
            return queueIds;
        }

        public static void ClearGridDataForCacheKey(string cacheKey)
        {
            lock (((IDictionary)dictGridData).SyncRoot)
            {
                if (dictGridData.ContainsKey(cacheKey))
                {
                    dictGridData.Remove(cacheKey);
                }
            }
        }

        public static void ClearGridDataForGridType(string cacheKey, WorkflowGridType gridType)
        {
            string gridTypeStr = gridType.ToString();
            lock (((IDictionary)dictGridData).SyncRoot)
            {
                if (dictGridData.ContainsKey(cacheKey) && dictGridData[cacheKey].ContainsKey(gridTypeStr))
                {
                    dictGridData[cacheKey].Remove(gridTypeStr);
                }
            }
        }

        public static void ClearGridDataForGridId(string cacheKey, WorkflowGridType gridType, string gridId)
        {
            string gridTypeStr = gridType.ToString();
            lock (((IDictionary)dictGridData).SyncRoot)
            {
                if (dictGridData.ContainsKey(cacheKey) && dictGridData[cacheKey].ContainsKey(gridTypeStr) && dictGridData[cacheKey][gridTypeStr].ContainsKey(gridId))
                {
                    dictGridData[cacheKey][gridTypeStr].Remove(gridId);
                }
            }
        }
    }

    class GridData
    {
        public DataSet dsGridData { get; set; }
        public Dictionary<string, string> dictQueueIds { get; set; }
    }
}
