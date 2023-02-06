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
    public class SRMVendorManagement
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorManagement");
        public ObjectSet GetVendorSystemSettingsConfiguration(List<int> vendorManagementIds = null,bool isVisible = true,bool isSync = false,RDBConnectionManager mDBCon = null)
        {
            try
            {
                mLogger.Debug("SRMVendorManagement -> GetVendorSystemSettingsConfiguration -> Start");
                ObjectSet oSet = new SRMVendorManagementDBManager(mDBCon).DownloadVendorSystemConfiguration(vendorManagementIds, isVisible);

                if (oSet != null && oSet.Tables.Count > 0)
                {
                    oSet.Tables[0].TableName = SRMVendorManagementConstants.TABLE_CONFIGURATIONS;
                    oSet.Tables[1].TableName = SRMVendorManagementConstants.TABLE_HEADERS;

                    if (!isSync)
                    {
                        RemoveUnNecessaryColumns(oSet);
                    }
                }

                return oSet;
            }
            catch
            {
                mLogger.Debug("SRMVendorManagement -> GetVendorSystemSettingsConfiguration -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("SRMVendorManagement -> GetVendorSystemSettingsConfiguration -> End");
            }


        }

        private void RemoveUnNecessaryColumns(ObjectSet oSet)
        {
            Dictionary<int, List<string>> dictColumnsToRemove = new Dictionary<int, List<string>>();

            //Populate columns to remove
            dictColumnsToRemove.Add(0, new List<string>() { SRMVendorManagementConstants.VENDOR_ID, SRMVendorManagementConstants.VENDOR_MANAGEMENT_ID, SRMVendorManagementConstants.REQUEST_TYPE_ID });
            dictColumnsToRemove.Add(1, new List<string>() { SRMVendorManagementConstants.VENDOR_ID, SRMVendorManagementConstants.VENDOR_MANAGEMENT_ID, SRMVendorManagementConstants.REQUEST_TYPE_ID, SRMVendorManagementConstants.HEADER_TYPE_ID });

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
