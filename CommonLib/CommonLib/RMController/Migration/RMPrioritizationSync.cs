using com.ivp.commom.commondal;
using com.ivp.common.Migration;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.ivp.common.Migration
{
    public partial class RMPrioritizationSync
    {
        List<string> failedEntityTypes = null;
        public RMPrioritizationSync()
        {
            failedEntityTypes = new List<string>();
        }

        static IRLogger mLogger = RLogFactory.CreateLogger("RMPrioritizationSync");
        public void StartSync(int moduleID, ObjectSet deltaSet, string userName, DateTime syncDateTime, string dateFormat, out string errorMessage)
        {
            mLogger.Debug("RMPrioritizationSync : SyncPrioritization -> Start");
            try
            {
                errorMessage = string.Empty;
                RDBConnectionManager mDBCon = null;
                RMPrioritizationController controller = new RMPrioritizationController();
                ObjectTable sourceTableEntityType = null;
                ObjectTable dbTableEntityType = null;
                ObjectTable sourceTableAttributes = null;
                ObjectTable dbTableAttributes = null;
                ObjectTable sourceTableMerging = null;
                ObjectTable dbTableMerging = null;
                Dictionary<string, int> dataSourceNameVsId = new Dictionary<string, int>();
                Dictionary<string, List<string>> entityTypeVendors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                Dictionary<int, string> dictAllDataSources = new RMDataSourceControllerNew().GetAllDataSources();
                int priorityColumnCount = -1;
                ObjectSet dbPrioritizationData = controller.GetPrioritizationConfiguration(moduleID, new List<int>(), true);

                //Validating priority columns
                errorMessage = ValidatePriorityColumns(deltaSet.Tables[RMPrioritizationConstants.TABLE_ATTRIBUTE_PRIORITIZATION], dbPrioritizationData.Tables[RMPrioritizationConstants.TABLE_ATTRIBUTE_PRIORITIZATION],  ref priorityColumnCount);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    Dictionary<string, RMEntityDetailsInfo> entityTypeDetails = new RMModelerController().GetEntityTypeDetails(moduleID, null);
                    entityTypeVendors = new RMPrioritizationController().GetEntityTypeMappedVendors(null, null);

                    failedEntityTypes = ValidateFailedMetaDataRows(deltaSet.Tables[RMPrioritizationConstants.TABLE_ENTITY_TYPE_CONFIG], deltaSet.Tables[RMPrioritizationConstants.TABLE_ATTRIBUTE_PRIORITIZATION], deltaSet.Tables[RMPrioritizationConstants.TABLE_DATA_SOURCE_MERGING]);

                    #region Entity type Configuration
                    if (deltaSet != null)
                    {
                        sourceTableEntityType = deltaSet.Tables[RMPrioritizationConstants.TABLE_ENTITY_TYPE_CONFIG];
                        dbTableEntityType = dbPrioritizationData.Tables[RMPrioritizationConstants.TABLE_ENTITY_TYPE_CONFIG];

                        sourceTableAttributes = deltaSet.Tables[RMPrioritizationConstants.TABLE_ATTRIBUTE_PRIORITIZATION];
                        dbTableAttributes = dbPrioritizationData.Tables[RMPrioritizationConstants.TABLE_ATTRIBUTE_PRIORITIZATION];

                        sourceTableMerging = deltaSet.Tables[RMPrioritizationConstants.TABLE_DATA_SOURCE_MERGING];
                        dbTableMerging = dbPrioritizationData.Tables[RMPrioritizationConstants.TABLE_DATA_SOURCE_MERGING];

                        if (sourceTableEntityType == null)
                        {
                            sourceTableEntityType = dbTableEntityType.Copy();
                            AddStaticColumns(sourceTableEntityType);
                        }
                        if (sourceTableAttributes == null)
                        {
                            sourceTableAttributes = dbTableAttributes.Copy();
                            AddStaticColumns(sourceTableAttributes);
                        }
                        if (sourceTableMerging == null)
                        {
                            sourceTableMerging = dbTableMerging.Copy();
                            AddStaticColumns(sourceTableMerging);
                        }

                        foreach (ObjectRow row in sourceTableEntityType.Rows.Where(r => !Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)))
                        {
                            bool isValid = true;
                            bool isAllInSync = true;
                            List<ObjectRow> attributeMappingRows = null;
                            List<ObjectRow> mergingRows = null;
                            List<string> dataSources = null;
                            List<RMEntityPrioritizationInfo> priorities = null;
                            RMEntityDetailsInfo entityTypeInfo = null;
                            string mergingXML = string.Empty;
                            string distinctEntityTypeIds = string.Empty;

                            mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection, true);
                            try
                            {
                                string entityTypeName = Convert.ToString(row[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]);
                                bool raiseFirstVendor = Convert.ToBoolean(row[RMPrioritizationConstants.FIRST_PRIORITY_VENDOR_EXCEPTION]);
                                bool raiseNoVendor = Convert.ToBoolean(row[RMPrioritizationConstants.NO_VENDOR_VALUE_EXCEPTION]);
                                bool deletePrevExceptions = Convert.ToBoolean(row[RMPrioritizationConstants.DELETE_PREVIOUS_EXCEPTION]);
                                bool applyUIRules = Convert.ToBoolean(row[RMPrioritizationConstants.APPLY_UI_CALCULATED_RULES]);

                                if (!Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_IN_SYNC))
                                    isAllInSync = false;

                                //Check entity type exists or not
                                if (!entityTypeDetails.Keys.SRMContainsWithIgnoreCase(entityTypeName) || entityTypeDetails[entityTypeName] == null)
                                {
                                    AddFailedEntityType(entityTypeName);
                                    isValid = false;
                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.ENTITY_TYPE_NOT_FOUND }, false);
                                }
                                else
                                {
                                    //Check entity type feed mapping exists or not
                                    if (entityTypeVendors.Keys.SRMContainsWithIgnoreCase(entityTypeName))
                                        dataSources = entityTypeVendors[entityTypeName];

                                    if (dataSources == null || dataSources.Count <= 0)
                                    {
                                        isValid = false;
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.ENTITY_TYPE_FEED_MAPPING_NOT_EXISTS }, false);
                                        AddFailedEntityType(entityTypeName);
                                    }
                                    else //Start Validating Attribute Priorities
                                    {
                                        entityTypeInfo = entityTypeDetails[entityTypeName];
                                        attributeMappingRows = sourceTableAttributes.AsEnumerable()
                                            .Where(c => Convert.ToString(c[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName)).ToList();

                                        if (attributeMappingRows == null || attributeMappingRows.Count <= 0)
                                        {
                                            isValid = false;
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.ATTRIBUTE_PRIORITIES_NOT_FOUND }, false);
                                            AddFailedEntityType(entityTypeName);
                                        }
                                        else
                                        {
                                            priorities = ValidateAttributePriorities(attributeMappingRows, entityTypeInfo, dataSources, dictAllDataSources, priorityColumnCount, userName, syncDateTime, out errorMessage, ref isValid, ref distinctEntityTypeIds, ref isAllInSync);
                                        }
                                    }
                                    //End Validating Attribute Priorities

                                    //Start Validating Merge Mapping

                                    if (sourceTableMerging != null)
                                    {
                                        mergingRows = sourceTableMerging.AsEnumerable()
                                                .Where(c => Convert.ToString(c[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName)).ToList();
                                    }

                                    Dictionary<int, string> dictMergableUniqueKeys = new RMDataSourceControllerNew().GetEntityTypeKeysForMerging(entityTypeInfo.EntityTypeID);

                                    if (mergingRows != null && mergingRows.Count > 0)
                                    {
                                        if (mergingRows.Count > dataSources.Count)
                                        {
                                            isValid = false;
                                            mergingRows.ForEach(mr => {
                                                new RMCommonMigration().
                                               SetFailedRow(mr, new List<string>() { RMCommonConstants.ATTRIBUTE_MERGING_EXCEEDING_DATASOURCES }, false);
                                            });
                                           
                                            AddFailedEntityType(entityTypeName);
                                        }
                                        if (mergingRows.Where(m => !string.IsNullOrEmpty(Convert.ToString(m[RMPrioritizationConstants.UNIQUE_KEY_NAME]))).Count() == 1)
                                        {
                                            isValid = false;

                                            mergingRows.ForEach(mr => {
                                                new RMCommonMigration().
                                               SetFailedRow(mr, new List<string>() { RMCommonConstants.ATTRIBUTE_MERGING_LESS_THAN_ONE }, false);
                                            });
                                            AddFailedEntityType(entityTypeName);
                                        }
                                    }

                                    mergingXML = ValidatePrioritizationMergeMapping(mergingRows, dictMergableUniqueKeys, dataSources, dictAllDataSources, userName, syncDateTime, out errorMessage, ref isValid, ref isAllInSync);

                                    //End Validating Merge Mapping
                                }

                                if (isValid && !isAllInSync)
                                {
                                    //Save Prioritization

                                    bool isSaved = new RMDataSourceControllerNew().SaveEntityPrioritization(priorities, entityTypeInfo.EntityTypeID, distinctEntityTypeIds, mergingXML, userName, raiseFirstVendor, deletePrevExceptions, raiseNoVendor, applyUIRules, mDBCon);

                                    new RMCommonMigration().SetPassedRow(row, string.Empty);

                                    if (attributeMappingRows != null)
                                        attributeMappingRows.ForEach(r =>
                                        {
                                            new RMCommonMigration().SetPassedRow(r, string.Empty);
                                        });

                                    if (mergingRows != null)
                                        mergingRows.ForEach(r =>
                                        {
                                            new RMCommonMigration().SetPassedRow(r, string.Empty);
                                        });

                                }
                                mDBCon.CommitTransaction();
                            }
                            catch (Exception ex)
                            {
                                if (mDBCon != null)
                                    mDBCon.RollbackTransaction();

                                new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                            }
                        }

                        FailAllDependencies(deltaSet, failedEntityTypes);
                        //new RMEntityTypeModelerMigration(dbEntityTypeData).SyncEntityTypes(deltaSet, dbTable, sourceTable, userName, moduleId, entityDetailsInfo);
                    }
                    #endregion

                    CheckUnProcessedRows(deltaSet.Tables[RMPrioritizationConstants.TABLE_ATTRIBUTE_PRIORITIZATION], deltaSet.Tables[RMPrioritizationConstants.TABLE_DATA_SOURCE_MERGING]);
                }
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMPrioritizationSync : SyncPrioritization -> Error: " + ex.Message);
                errorMessage = ex.Message;
            }
            finally
            {
                mLogger.Debug("RMPrioritizationSync : SyncPrioritization -> End");
            }
        }
        
    }
}
