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
    public class RMPrioritizationDBManager
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMPrioritizationDBManager");
        private RDBConnectionManager mDBCon = null;

        public RMPrioritizationDBManager(RDBConnectionManager conMgr)
        {
            this.mDBCon = conMgr;
        }
        public ObjectSet GetPrioritizationConfiguration(int moduleId, string entityTypes)
        {
            try
            {
                mLogger.Debug("RMPrioritizationDBManager -> GetPrioritizationConfiguration -> Start");

                ObjectSet oSet = null;
                string queryText = "EXEC [dbo].[REFM_GetEntityTypePrioritization] " + moduleId + ",'" + entityTypes + "'";

                if (this.mDBCon != null)
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                else
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);

                return oSet;
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMDataSourceDBManager -> GetPrioritizationConfiguration -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> GetPrioritizationConfiguration -> End");
            }
        }

        public ObjectTable GetEntityTypeVendors(List<int> entity_type_ids)
        {
            mLogger.Debug("RMDataSourceDBManager -> GetEntityTypeVendors -> Start");
            try
            {
                ObjectSet oSet = null;
                string entityTypeIds = "";
                string queryText = string.Empty;

                if (entity_type_ids != null)
                    entityTypeIds = string.Join(",", entity_type_ids);

                queryText = " EXEC REFM_GetEntityTypeVendors '" + entityTypeIds + "' ";

                if (this.mDBCon != null)
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                else
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);

                if (oSet != null && oSet.Tables.Count > 0)
                    return oSet.Tables[0];

                return null;
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMDataSourceDBManager -> GetEntityTypeVendors -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> GetEntityTypeVendors -> End");
            }
        }

        public ObjectSet GetPossibleDataSourceMapping(int moduleId)
        {

            try
            {
                mLogger.Debug("RMPrioritizationDBManager -> GetPossibleDataSourceMapping -> Start");

                ObjectSet oSet = null;
                string queryText = @"SELECT 
                                        ISNULL(cetyp.entity_display_name, etyp.entity_display_name) AS[Entity Type Name],
	                                    ISNULL(cetyp.entity_type_id, etyp.entity_type_id) AS[entity_type_id],
	                                    CASE WHEN cetyp.entity_display_name IS NOT NULL THEN etyp.entity_display_name END AS[Leg Name],
	                                    CASE WHEN cetyp.entity_display_name IS NOT NULL THEN etyp.entity_type_id END AS[leg_entity_type_id],
	                                    ds.data_source_name AS[Data Source Name],
                                        ds.data_source_id AS[data_source_id]
                                    FROM IVPRefMaster.dbo.ivp_refm_entity_type_feed_mapping map
                                    INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_feed_summary fsum
                                    ON(map.feed_summary_id = fsum.feed_summary_id AND map.is_active = 1 AND fsum.is_active = 1)
                                    INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_data_source ds
                                    ON(ds.data_source_id = fsum.data_source_id AND ds.is_active = 1)
                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                                    ON(etyp.entity_type_id = map.entity_type_id AND etyp.is_active = 1 AND module_id = " + moduleId + @")
                                    LEFT JOIN IVPRefMaster.dbo.ivp_refm_entity_type cetyp
                                    ON(cetyp.entity_type_id = etyp.derived_from_entity_type_id)
                                    ORDER BY[Entity Type Name],[Leg Name]";

                if (this.mDBCon != null)
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                else
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);

                return oSet;
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMDataSourceDBManager -> GetPossibleDataSourceMapping -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> GetPossibleDataSourceMapping -> End");
            }
        }
    }
}
