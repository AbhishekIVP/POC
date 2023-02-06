using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using com.ivp.rad.dal;
using com.ivp.rad.common;
using com.ivp.rad.utils;
using com.ivp.commom.commondal;

namespace com.ivp.common
{
    public static class SRMWorkflowDBHandler
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMWorkflowDBHandler");
        private static RHashlist mHList = null;

        internal static void InsertRequestQueue(string InstrumentXML, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("InsertRequestQueue -> Start");
                string query = string.Empty;

                query = " EXEC IVPRefMaster.dbo.SRM_GenerateWorkflowRequestQueue '" + InstrumentXML + "' ";

                if (conMgr == null)
                    CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
                else
                    CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Insert, conMgr);

                mLogger.Debug("InsertRequestQueue -> End");
            }
            catch (Exception ex)
            {
                mLogger.Error("InsertRequestQueue -> Error: " + ex.Message);
                throw;
            }
        }

        internal static DataTable GetWorkflowCountPerType(int moduleID, string instancesTempTable, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("GetWorkflowCountPerType -> Start");
                string query = string.Empty;
                //string instanceIds = "-1";
                DataTable dt = null;

                //if (instances != null)
                //    instanceIds = string.Join(",", instances);

                query = " EXEC IVPRefMaster.dbo.SRM_GetWorkflowCountPerType " + moduleID + ", '" + instancesTempTable + "' ";

                if (conMgr == null)
                    dt = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                else
                    dt = CommonDALWrapper.ExecuteSelectQuery(query, conMgr).Tables[0];

                return dt;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowCountPerType -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("GetWorkflowCountPerType -> End");
            }
        }

        internal static string GetInstrumentIDByInstanceID(int radInstanceID)
        {
            try
            {
                mLogger.Debug("GetInstrumentIDByInstanceID -> Start");
                string query = string.Empty;
                DataTable dt = null;

                query = " SELECT instrument_id FROM ivp_srm_workflow_requests_queue WHERE rad_workflow_instance_id = " + radInstanceID + " ";

                dt = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];

                return Convert.ToString(dt.Rows[0]["instrument_id"]);
            }
            catch (Exception ex)
            {
                mLogger.Error("GetInstrumentIDByInstanceID -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("GetInstrumentIDByInstanceID -> End");
            }
        }

        internal static List<int> GetInstancesByInstrumentID(string instrumentID, int moduleID)
        {
            try
            {
                mLogger.Debug("GetInstancesByInstrumentID -> Start");
                List<int> instances = new List<int>();
                string query = string.Empty;
                DataTable dt = null;

                query = " SELECT DISTINCT RQ.rad_workflow_instance_id FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue RQ INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_instance WI " +
                          "  ON RQ.workflow_instance_id = WI.workflow_instance_id " +
                          "  WHERE RQ.instrument_id = '" + instrumentID + "' AND WI.module_id = " + moduleID + " AND WI.is_active=1";

                dt = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];

                if (dt != null)
                {
                    instances = dt.AsEnumerable().Select(d => Convert.ToInt32(d["rad_workflow_instance_id"])).Distinct().ToList();
                }

                return instances;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetInstancesByInstrumentID -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("GetInstancesByInstrumentID -> End");
            }
        }

        internal static void InactiveRequestQueue(List<int> instances, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("InactiveRequestQueue -> Start");
                string query = string.Empty;
                string instanceIds = "-1";

                if (instances != null && instances.Count > 0)
                    instanceIds = string.Join(",", instances);

                query = " UPDATE IVPRefMaster.dbo.ivp_srm_workflow_requests_queue SET is_active = 0 WHERE rad_workflow_instance_id  IN (" + instanceIds + ") ";

                if (conMgr == null)
                    CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                else
                    CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

            }
            catch (Exception ex)
            {
                mLogger.Error("InactiveRequestQueue -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("InactiveRequestQueue -> End");
            }
        }


        internal static Dictionary<int, WorkflowDetailsInfo> GetWorkflowDetailsVsRADWorkflowInstanceID(List<int> instanceIDs, RDBConnectionManager conMgr = null)
        {
            try
            {
                //conMgr = null;
                mLogger.Debug("GetWorkflowDetailsVsRADWorkflowInstanceID -> Start");
                string query = string.Empty;
                Dictionary<int, WorkflowDetailsInfo> dict = new Dictionary<int, WorkflowDetailsInfo>();
                DataTable dt = null;
                string instances = "-1";

                if (instanceIDs != null && instanceIDs.Count > 0)
                    instances = string.Join(",", instanceIDs);

                query = string.Format(@"IF EXISTS(SELECT * FROM tempdb.dbo.sysobjects WHERE ID = OBJECT_ID(N'tempdb..#temp_workflow_ids')) 
	                DROP TABLE #temp_workflow_ids

                DECLARE @instances VARCHAR(MAX) = '{0}'

                SELECT DISTINCT CAST(item AS INT) AS workflow_instance_id, rins.workflow_id
                INTO #temp_workflow_ids
                FROM IVPRefMaster.dbo.REFM_GetList2Table(@instances, ',') tids
				INNER JOIN IVPRAD.dbo.ivp_rad_workflow_instances rins
                ON rins.workflow_instance_id = CAST(tids.item AS INT)

                CREATE NONCLUSTERED INDEX temp_workflow_ids_workflow_instance_id
                ON #temp_workflow_ids (workflow_instance_id)

                DECLARE @module_id INT, @is_create BIT

                SELECT @module_id = tab.module_id, @is_create = tab.is_create
                FROM (SELECT TOP 1 CAST((CASE WHEN wins.workflow_action_type_id IN (1,6,11,16) THEN 1 ELSE 0 END) AS BIT) AS is_create, wins.module_id
                FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
                INNER JOIN #temp_workflow_ids tids
                ON rq.rad_workflow_instance_id = tids.workflow_instance_id
                INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_instance wins
                ON wins.workflow_instance_id = rq.workflow_instance_id)tab

                IF (@module_id = 3)
				BEGIN
					DECLARE @script VARCHAR(MAX) = '
                    SELECT rq.type_id, tmas.sectype_name AS type_name, ' + (CAST(@is_create AS VARCHAR)) + ' AS is_create, ' + (CAST(@module_id AS VARCHAR)) + ' AS module_id, tids.workflow_instance_id, tids.workflow_id, rq.effective_from_date, rq.effective_to_date
	                FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
	                INNER JOIN #temp_workflow_ids tids
	                ON rq.rad_workflow_instance_id = tids.workflow_instance_id
	                INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master tmas
	                ON tmas.sectype_id = rq.type_id'

					EXEC (@script)
				END
                ELSE
	                SELECT rq.type_id, tmas.entity_display_name AS type_name, @is_create AS is_create, @module_id AS module_id, tids.workflow_instance_id, tids.workflow_id, rq.effective_from_date, rq.effective_to_date
	                FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
	                INNER JOIN #temp_workflow_ids tids
	                ON rq.rad_workflow_instance_id = tids.workflow_instance_id
	                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type tmas
	                ON tmas.entity_type_id = rq.type_id


                IF EXISTS(SELECT * FROM tempdb.dbo.sysobjects WHERE ID = OBJECT_ID(N'tempdb..#temp_workflow_ids')) 
	                DROP TABLE #temp_workflow_ids", instances);

                if (conMgr == null)
                    dt = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                else
                    dt = CommonDALWrapper.ExecuteSelectQuery(query, conMgr).Tables[0];

                if (dt != null)
                {
                    dict = dt.AsEnumerable().ToDictionary<DataRow, int, WorkflowDetailsInfo>(row => Convert.ToInt32(row["workflow_instance_id"]), row => new WorkflowDetailsInfo { RadWorkflowId = Convert.ToInt32(row["workflow_id"]), moduleId = Convert.ToInt32(row["module_id"]), typeId = Convert.ToInt32(row["type_id"]), isCreate = Convert.ToBoolean(row["is_create"]), typeName = Convert.ToString(row["type_name"]), EffectiveStartDate = (row.IsNull("effective_from_date") ? null : (DateTime?)Convert.ToDateTime(row["effective_from_date"])), EffectiveEndDate = (row.IsNull("effective_to_date") ? null : (DateTime?)Convert.ToDateTime(row["effective_to_date"])) });
                }

                return dict;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowDetailsVsRADWorkflowInstanceID -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("GetWorkflowDetailsVsRADWorkflowInstanceID -> End");
            }
        }

        internal static Dictionary<string, WorkflowDetailsInfo> GetWorkflowDetailsVsInstrumentID(List<int> instanceIDs, RDBConnectionManager conMgr = null)
        {
            try
            {
                //conMgr = null;
                mLogger.Debug("GetWorkflowDetailsVsInstrumentID -> Start");
                string query = string.Empty;
                Dictionary<string, WorkflowDetailsInfo> dict = new Dictionary<string, WorkflowDetailsInfo>();

                string tableName = "IVPRefMaster.dbo.[" + Guid.NewGuid().ToString() + "]";
                DataTable dtt = null;
                try
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("rad_workflow_instance_id", typeof(long));
                    foreach (var id in instanceIDs)
                    {
                        dt.Rows.Add(id);
                    }

                    if (conMgr == null)
                    {
                        CommonDALWrapper.ExecuteQuery("CREATE TABLE " + tableName + "(rad_workflow_instance_id BIGINT)", CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
                        CommonDALWrapper.ExecuteBulkUpload(tableName, dt, ConnectionConstants.RefMaster_Connection);
                    }
                    else
                    {
                        CommonDALWrapper.ExecuteQuery("CREATE TABLE " + tableName + "(rad_workflow_instance_id BIGINT)", CommonQueryType.Insert, conMgr);
                        CommonDALWrapper.ExecuteBulkUpload(tableName, dt, conMgr);
                    }

                    query = string.Format(@"DECLARE @module_id INT, @is_create BIT

                    SELECT @module_id = tab.module_id, @is_create = tab.is_create
                    FROM (SELECT TOP 1 CAST((CASE WHEN wins.workflow_action_type_id IN (1,6,11,16) THEN 1 ELSE 0 END) AS BIT) AS is_create, wins.module_id
                    FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
                    INNER JOIN {0} tids
                    ON rq.rad_workflow_instance_id = tids.rad_workflow_instance_id
                    INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_instance wins
                    ON wins.workflow_instance_id = rq.workflow_instance_id)tab

                    IF (@module_id = 3)
					BEGIN
						DECLARE @script VARCHAR(MAX) = '
                        SELECT rq.instrument_id, rq.type_id, tmas.sectype_name AS type_name, ' + (CAST(@is_create AS VARCHAR)) + ' AS is_create, ' + (CAST(@module_id AS VARCHAR)) + ' AS module_id, rq.workflow_instance_id, rq.effective_from_date, rq.effective_to_date, rq.requested_by
	                    FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
	                    INNER JOIN {0} tids
	                    ON rq.rad_workflow_instance_id = tids.rad_workflow_instance_id
	                    INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master tmas
	                    ON tmas.sectype_id = rq.type_id'

						EXEC (@script)
					END
                    ELSE
	                    SELECT rq.instrument_id, rq.type_id, tmas.entity_display_name AS type_name, @is_create AS is_create, @module_id AS module_id, rq.workflow_instance_id, rq.effective_from_date, rq.effective_to_date, rq.requested_by
	                    FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
	                    INNER JOIN {0} tids
	                    ON rq.rad_workflow_instance_id = tids.rad_workflow_instance_id
	                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type tmas
	                    ON tmas.entity_type_id = rq.type_id", tableName);

                    if (conMgr == null)
                        dtt = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                    else
                        dtt = CommonDALWrapper.ExecuteSelectQuery(query, conMgr).Tables[0];
                }
                catch
                {
                    throw;
                }
                finally
                {
                    try
                    {
                        if (conMgr == null)
                            CommonDALWrapper.ExecuteQuery("IF(OBJECT_ID('" + tableName + "') IS NOT NULL) DROP TABLE " + tableName, CommonQueryType.Delete, ConnectionConstants.RefMaster_Connection);
                        else
                            CommonDALWrapper.ExecuteQuery("IF(OBJECT_ID('" + tableName + "') IS NOT NULL) DROP TABLE " + tableName, CommonQueryType.Delete, conMgr);
                    }
                    catch (Exception e)
                    {
                        mLogger.Error(e.ToString());
                    }
                }

                if (dtt != null)
                {
                    dict = dtt.AsEnumerable().ToDictionary<DataRow, string, WorkflowDetailsInfo>(row => Convert.ToString(row["instrument_id"]), row => new WorkflowDetailsInfo { typeId = Convert.ToInt32(row["type_id"]), isCreate = Convert.ToBoolean(row["is_create"]), typeName = Convert.ToString(row["type_name"]), WorkflowInstanceId = Convert.ToInt32(row["workflow_instance_id"]), EffectiveStartDate = (row.IsNull("effective_from_date") ? null : (DateTime?)Convert.ToDateTime(row["effective_from_date"])), EffectiveEndDate = (row.IsNull("effective_to_date") ? null : (DateTime?)Convert.ToDateTime(row["effective_to_date"])), Initiator = Convert.ToString(row["requested_by"]) });
                }

                return dict;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowDetailsVsInstrumentID -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("GetWorkflowDetailsVsInstrumentID -> End");
            }
        }

        internal static string GetWorkflowTypeByRadInstanceID(int radInstanceID, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("GetWorkflowTypeByRadInstanceID -> Start");
                string query = string.Empty;
                string workflowType = string.Empty;


                query = " SELECT TOP 1 " +
                        " CASE WHEN AT.workflow_action_type_id IN (1, 6, 11, 16) THEN 'Create' " +
                        " WHEN AT.workflow_action_type_id IN (2, 7, 12, 17) THEN 'Update' " +
                        " WHEN AT.workflow_action_type_id IN (3, 8, 13, 18) THEN 'Delete' " +
                        " WHEN AT.workflow_action_type_id IN (4, 9, 14, 19) THEN 'Attribute' " +
                        " ELSE 'Leg' END AS workflowType " +
                        " FROM ivp_srm_workflow_action_type AT " +
                        " INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_instance WI " +
                        " ON WI.workflow_action_type_id = AT.workflow_action_type_id " +
                        " INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_requests_queue RQ " +
                        " ON RQ.workflow_instance_id = WI.workflow_instance_id " +
                        " WHERE RQ.rad_workflow_instance_id = " + radInstanceID + " ";

                if (conMgr == null)
                    workflowType = Convert.ToString(CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0].Rows[0]["workflowType"]);
                else
                    workflowType = Convert.ToString(CommonDALWrapper.ExecuteSelectQuery(query, conMgr).Tables[0].Rows[0]["workflowType"]);

                return workflowType;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowTypeByRadInstanceID -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("GetWorkflowTypeByRadInstanceID -> End");
            }
        }

        internal static DataTable GetWorkflowQueueData(int moduleID, List<int> instances, string workflowType, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("GetWorkflowQueueData -> Start");
                string query = string.Empty;
                string instanceIds = "-1";
                DataTable dt = null;
                DataSet ds = null;

                if (instances != null && instances.Count > 0)
                    instanceIds = string.Join(",", instances);

                query = " EXEC IVPRefMaster.dbo.SRM_GetWorkflowInboxData " + moduleID + ", '" + instanceIds + "', '" + workflowType + "' ";

                if (conMgr == null)
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                else
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];

                return dt;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowQueueData -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("GetWorkflowQueueData -> End");
            }
        }

        internal static List<WFStageRuleConfigurationInfo> GetRuleConfigInfoForInitiation(List<int> lstRuleMappings, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("GetRuleConfigInfoForInitiation -> Start");
                List<WFStageRuleConfigurationInfo> lstConfig = new List<WFStageRuleConfigurationInfo>();
                WFStageRuleConfigurationInfo config = null;
                string query = string.Empty;
                DataSet ds = null;
                DataTable dtConfig = null;

                query = " EXEC IVPRefMaster.dbo.SRM_GetWorkflowRuleDetailsForInitiation '" + string.Join(",", lstRuleMappings) + "' ";

                if (conMgr == null)
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                else
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    dtConfig = ds.Tables[0];

                    dtConfig.AsEnumerable().ToList().ForEach(row =>
                    {
                        config = new WFStageRuleConfigurationInfo();

                        config.WorkflowInstanceID = Convert.ToInt32(row["workflow_instance_id"]);
                        config.WorkflowInstanceID = Convert.ToInt32(row["rad_workflow_id"]);
                        config.RuleMappingID = Convert.ToInt32(row["rule_mapping_id"]);
                        config.CurrentStage = Convert.ToString(row["stage_name"]);
                        config.ExecuteAlertRules = Convert.ToBoolean(row["apply_alert"]);
                        config.ExecuteBasketAlertRules = Convert.ToBoolean(row["apply_basket_alert"]);
                        config.ExecuteGroupValidationRules = Convert.ToBoolean(row["apply_basket_validation"]);
                        config.ExecuteMandatoryCheck = Convert.ToBoolean(row["apply_mandatory"]);
                        config.ExecutePrimaryCheck = Convert.ToBoolean(row["apply_primary"]);
                        config.ExecuteUniquenessCheck = Convert.ToBoolean(row["apply_uniqueness"]);
                        config.ExecuteValidationRules = Convert.ToBoolean(row["apply_validation"]);

                        lstConfig.Add(config);
                    });
                }

                return lstConfig;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetRuleConfigInfoForInitiation -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("GetRuleConfigInfoForInitiation -> End");
            }
        }

        public static List<WFStageRuleConfigurationInfo> GetRuleConfigInfoByQueueID(List<int> queue_ids, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("GetRuleConfigInfoByQueueID -> Start");
                List<WFStageRuleConfigurationInfo> lstConfig = new List<WFStageRuleConfigurationInfo>();
                WFStageRuleConfigurationInfo config = null;
                string query = string.Empty;
                DataSet ds = null;
                DataTable dtConfig = null;

                query = " EXEC IVPRefMaster.dbo.SRM_GetWorkflowRuleDetailsByQueueID '" + string.Join(",", queue_ids) + "' ";

                if (conMgr == null)
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                else
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    dtConfig = ds.Tables[0];

                    dtConfig.AsEnumerable().ToList().ForEach(row =>
                    {
                        config = new WFStageRuleConfigurationInfo();

                        config.QueueID = Convert.ToInt32(row["queue_id"]);
                        config.RadInstanceID = Convert.ToInt32(row["rad_workflow_instance_id"]);
                        config.WorkflowInstanceID = Convert.ToInt32(row["workflow_instance_id"]);
                        config.TypeID = Convert.ToInt32(row["type_id"]);
                        config.InstrumentID = Convert.ToString(row["instrument_id"]);
                        config.CurrentStage = Convert.ToString(row["stage_name"]);
                        config.ExecuteAlertRules = Convert.ToBoolean(row["apply_alert"]);
                        config.ExecuteBasketAlertRules = Convert.ToBoolean(row["apply_basket_alert"]);
                        config.ExecuteGroupValidationRules = Convert.ToBoolean(row["apply_basket_validation"]);
                        config.ExecuteMandatoryCheck = Convert.ToBoolean(row["apply_mandatory"]);
                        config.ExecutePrimaryCheck = Convert.ToBoolean(row["apply_primary"]);
                        config.ExecuteUniquenessCheck = Convert.ToBoolean(row["apply_uniqueness"]);
                        config.ExecuteValidationRules = Convert.ToBoolean(row["apply_validation"]);

                        lstConfig.Add(config);
                    });
                }

                return lstConfig;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetRuleConfigInfoByQueueID -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("GetRuleConfigInfoByQueueID -> End");
            }
        }

        internal static WorkflowInitiatedInfo GetWorkflowInitiatedInfo(int radInstanceID, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("GetWorkflowInitiatedInfo -> Start");
                WorkflowInitiatedInfo info = null;
                string query = string.Empty;
                DataSet ds = null;

                query = " EXEC IVPRefMaster.dbo.SRM_GetWorklowInitiateInfo '" + radInstanceID + "' ";

                if (conMgr == null)
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                else
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    info = new WorkflowInitiatedInfo();
                    info.InitiatedBy = Convert.ToString(ds.Tables[0].Rows[0]["requested_by"]);
                    info.InitiatedOn = Convert.ToDateTime(ds.Tables[0].Rows[0]["requested_on"]);
                }

                return info;
            }
            catch (Exception ex)
            {
                mLogger.Error("GetWorkflowInitiatedInfo -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("GetWorkflowInitiatedInfo -> End");
            }
        }

        internal static DataSet RPFMGetAttributeValuesForWorflowInbox(List<int> instances, RDBConnectionManager conMgr = null)
        {
            try
            {
                mLogger.Debug("RPFMGetAttributeValuesForWorflowInbox -> Start");
                string query = string.Empty;
                string instanceIds = "-1";
                DataSet ds = null;

                if (instances != null && instances.Count > 0)
                    instanceIds = string.Join(",", instances);

                query = " EXEC REFM_GetAttributeValuesForWorkflow '" + instanceIds + "' ";

                if (conMgr == null)
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                else
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, conMgr);

                return ds;
            }
            catch (Exception ex)
            {
                mLogger.Error("RPFMGetAttributeValuesForWorflowInbox -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RPFMGetAttributeValuesForWorflowInbox -> End");
            }
        }
    }
}
