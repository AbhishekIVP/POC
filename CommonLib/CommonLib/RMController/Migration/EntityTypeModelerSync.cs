using com.ivp.common.SecMaster;
using com.ivp.rad.common;
using com.ivp.rad.data;
using com.ivp.rad.RUserManagement;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.Migration
{
    public class EntityTypeModelerSync
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("EntityTypeModelerSync");
        public Dictionary<string, RMEntityDetailsInfo> entityDetailsInfo = new Dictionary<string, RMEntityDetailsInfo>();
        public Dictionary<string, SecurityTypeMasterInfo> secTypeDetailsInfo = new Dictionary<string, SecurityTypeMasterInfo>();
        public Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo = new Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>>();
        public Dictionary<string, AttrInfo> commonAttributes = new Dictionary<string, AttrInfo>();
        public Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo = new Dictionary<string, RMEntityDetailsInfo>();

        public void StartSync(int moduleId, ObjectSet deltaSet, string userName, DateTime syncDateTime, string dateFormat, out string errorMessage)
        {
            mLogger.Debug("EntityTypeModelerSync : SyncEntityTypeModeler -> Start");
            try
            {
                errorMessage = string.Empty;
                ObjectTable sourceTable = null;
                ObjectTable dbTable = null;
                ObjectSet dbEntityTypeData = new RMModelerController().GetModelerConfigurationDetails(moduleId, null, true, false); ;
                ObjectSet dbAllModulesEntityTypeData = new RMModelerController().GetModelerConfigurationDetails(0, null, true, false); ;
                IEnumerable<ObjectRow> currentRows = null;

                allModulesEntityDetailsInfo = new RMModelerController().GetEntityTypeDetails(0, null, dbAllModulesEntityTypeData);
                entityDetailsInfo = new RMModelerController().GetEntityTypeDetails(moduleId, null, dbEntityTypeData);
                secTypeDetailsInfo = new RMSectypeController().GetSectypeAttributes(false);
                entityTypeVstemplateInfo = new RMModelerController().GetTemplateDetails(dbEntityTypeData);
                commonAttributes = new RMSectypeController().FetchCommonAttributes(false);
                Dictionary<string, string> hshUsers = SRMCommonRAD.GetAllDisplayNamevsUsersLoginName();
                List<RUserInfo> userInfo = new RUserManagementService().GetAllUsersGDPR();
                List<string> hshGroups = SRMCommonRAD.GetAllGroups();

                #region Entity Type Definition
                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_DEFINITION) && deltaSet.Tables[RMModelerConstants.TABLE_DEFINITION] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_DEFINITION].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_DEFINITION];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_DEFINITION];

                    List<string> failedEntityTypes = sourceTable.AsEnumerable().Where(r => Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(x => Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME])).ToList();

                    FailAllDependencies(deltaSet, failedEntityTypes);

                    new RMEntityTypeModelerMigration(dbEntityTypeData).SyncEntityTypes(deltaSet, dbTable, sourceTable, userName, moduleId, entityDetailsInfo, entityTypeVstemplateInfo, allModulesEntityDetailsInfo, hshUsers, hshGroups);

                }
                #endregion

                #region Master Attributes
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_MASTER_ATTIRBUTES) && deltaSet.Tables[RMModelerConstants.TABLE_MASTER_ATTIRBUTES] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_MASTER_ATTIRBUTES].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_MASTER_ATTIRBUTES];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_MASTER_ATTIRBUTES];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    List<string> failedEntityTypes = sourceTable.AsEnumerable().Where(r => Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(x => Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME])).ToList();

                    if (failedEntityTypes != null)
                    {
                        foreach (var entity in failedEntityTypes)
                        {
                            new RMEntityTypeModelerMigration(dbEntityTypeData).FailAllDependencies(deltaSet, entity, null, true, false, false, false, false, false, false, false, false, false, false);
                        }
                    }

                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncMasterAttributes(currentRows, deltaSet, dbTable, sourceTable, entityDetailsInfo, secTypeDetailsInfo, dateFormat, userName, syncDateTime, commonAttributes, allModulesEntityDetailsInfo);
                    }
                }

                #endregion

                #region Leg Definition
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_LEGS_CONFIGURATION) && deltaSet.Tables[RMModelerConstants.TABLE_LEGS_CONFIGURATION] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_LEGS_CONFIGURATION].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_LEGS_CONFIGURATION];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_LEGS_CONFIGURATION];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    List<string> failedEntityTypes = sourceTable.AsEnumerable().Where(r => Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(x => Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME])).ToList();

                    if (failedEntityTypes != null)
                    {
                        foreach (var entity in failedEntityTypes)
                        {
                            new RMEntityTypeModelerMigration(dbEntityTypeData).FailAllDependencies(deltaSet, entity, null, true, true, false, false, false, false, false, false, false, false, false);
                        }
                    }
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncLegConfiguration(currentRows, deltaSet, dbTable, sourceTable, userName, moduleId, entityDetailsInfo, entityTypeVstemplateInfo, allModulesEntityDetailsInfo);
                    }
                }
                #endregion

                #region Leg Attributes
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_LEG_ATTRIBUTES) && deltaSet.Tables[RMModelerConstants.TABLE_LEG_ATTRIBUTES] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_LEG_ATTRIBUTES].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_LEG_ATTRIBUTES];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_LEG_ATTRIBUTES];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
                    List<string> failedEntityTypes = sourceTable.AsEnumerable().Where(r => Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(x => Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME])).ToList();

                    if (failedEntityTypes != null)
                    {
                        foreach (var entity in failedEntityTypes)
                        {
                            new RMEntityTypeModelerMigration(dbEntityTypeData).FailAllDependencies(deltaSet, entity, null, true, true, true, false, false, false, false, false, false, false, false);
                        }
                    }
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncLegAttributes(currentRows, deltaSet, dbTable, sourceTable, entityDetailsInfo, secTypeDetailsInfo, dateFormat, userName, syncDateTime, commonAttributes, allModulesEntityDetailsInfo);
                    }
                }
                #endregion

                #region Unique Keys
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_UNIQUE_KEYS) && deltaSet.Tables[RMModelerConstants.TABLE_UNIQUE_KEYS] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_UNIQUE_KEYS].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_UNIQUE_KEYS];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_UNIQUE_KEYS];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncUniqueKeys(currentRows, deltaSet, dbTable, sourceTable, userName, entityDetailsInfo);
                    }
                }
                #endregion

                #region Attribute Rules
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_ATTRIBUTE_RULES) && deltaSet.Tables[RMModelerConstants.TABLE_ATTRIBUTE_RULES] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_ATTRIBUTE_RULES].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_ATTRIBUTE_RULES];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_ATTRIBUTE_RULES];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncAttributeRules(currentRows, deltaSet, dbTable, sourceTable, userName, entityDetailsInfo);
                    }
                }
                #endregion Basket Rules
                #region
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_BASKET_RULES) && deltaSet.Tables[RMModelerConstants.TABLE_BASKET_RULES] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_BASKET_RULES].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_BASKET_RULES];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_BASKET_RULES];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncBasketRules(currentRows, deltaSet, dbTable, sourceTable, userName, entityDetailsInfo);
                    }
                }
                #endregion
                #region Template

                #region Layout Details
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_LAYOUTS) && deltaSet.Tables[RMModelerConstants.TABLE_LAYOUTS] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_LAYOUTS].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_LAYOUTS];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_LAYOUTS];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    var failedLayouts = sourceTable.AsEnumerable().Where(r => Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(x => new {
                        entity = Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME])
                                                                              ,
                        layout = Convert.ToString(x[RMModelerConstants.Layout_Name])
                    });

                    if (failedLayouts != null)
                    {
                        foreach (var index in failedLayouts)
                        {
                            new RMEntityTypeModelerMigration(dbEntityTypeData).FailAllDependencies(deltaSet, index.entity, index.layout, true, true, true, true, true, true, true, false, false, false, true);
                        }
                    }
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncLayouts(currentRows, deltaSet, dbTable, sourceTable, userName, entityTypeVstemplateInfo, entityDetailsInfo, hshUsers, hshGroups);
                    }
                }
                #endregion

                #region Layout Preference
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_USER_GROUP_LAYOUT_PRIORITY) && deltaSet.Tables[RMModelerConstants.TABLE_USER_GROUP_LAYOUT_PRIORITY] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_USER_GROUP_LAYOUT_PRIORITY].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_USER_GROUP_LAYOUT_PRIORITY];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_USER_GROUP_LAYOUT_PRIORITY];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncLayoutPreference(currentRows, deltaSet, dbTable, sourceTable, userName, entityTypeVstemplateInfo, entityDetailsInfo, hshUsers, userInfo);
                    }
                }
                #endregion

                #region Tab Management
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_TAB_MANAGEMENT) && deltaSet.Tables[RMModelerConstants.TABLE_TAB_MANAGEMENT] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_TAB_MANAGEMENT].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_TAB_MANAGEMENT];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_TAB_MANAGEMENT];
                    var failedLayouts = sourceTable.AsEnumerable().Where(r => Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(x => new {
                        entity = Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME])
                                                                              ,
                        layout = Convert.ToString(x[RMModelerConstants.Layout_Name])
                    });

                    if (failedLayouts != null)
                    {
                        foreach (var index in failedLayouts)
                        {
                            new RMEntityTypeModelerMigration(dbEntityTypeData).FailAllDependencies(deltaSet, index.entity, index.layout, true, true, true, true, true, true, true, true, false, true, true);
                        }
                    }
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncTabs(currentRows, deltaSet, dbTable, sourceTable, userName, entityTypeVstemplateInfo, entityDetailsInfo);
                    }
                }

                #endregion
                #region Layout Attribute Management
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT) && deltaSet.Tables[RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncAttributeManagement(currentRows, deltaSet, dbTable, sourceTable, userName, entityTypeVstemplateInfo, entityDetailsInfo);
                    }
                }
                #endregion
                #endregion

                #region Leg Display Order
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_LEG_ORDER) && deltaSet.Tables[RMModelerConstants.TABLE_LEG_ORDER] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_LEG_ORDER].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_LEG_ORDER];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_LEG_ORDER];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncLegOrder(currentRows, deltaSet, dbTable, sourceTable, userName, entityTypeVstemplateInfo, entityDetailsInfo);
                    }
                }
                #endregion

                #region Page Configuration
                currentRows = null;

                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION) && deltaSet.Tables[RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncAttributeConfiguration(currentRows, deltaSet, dbTable, sourceTable, userName, entityDetailsInfo);
                    }
                }
                #endregion
                #region Exception Preferences
                currentRows = null;
                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_EXCEPTION_PREFERENCES) && deltaSet.Tables[RMModelerConstants.TABLE_EXCEPTION_PREFERENCES] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_EXCEPTION_PREFERENCES].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_EXCEPTION_PREFERENCES];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_EXCEPTION_PREFERENCES];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncExceptionPreferences(currentRows, deltaSet, dbTable, sourceTable, userName, entityDetailsInfo, moduleId);
                    }
                }

                #endregion
                #region Action Notifications
                currentRows = null;
                if (deltaSet.Tables.Contains(RMModelerConstants.TABLE_ACTION_NOTIFICATIONS) && deltaSet.Tables[RMModelerConstants.TABLE_ACTION_NOTIFICATIONS] != null
                        && deltaSet.Tables[RMModelerConstants.TABLE_ACTION_NOTIFICATIONS].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMModelerConstants.TABLE_ACTION_NOTIFICATIONS];
                    dbTable = dbEntityTypeData.Tables[RMModelerConstants.TABLE_ACTION_NOTIFICATIONS];
                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
                    if (currentRows != null)
                    {
                        new RMEntityTypeModelerMigration(dbEntityTypeData).SyncActionNotifications(currentRows, deltaSet, dbTable, sourceTable, userName, entityDetailsInfo, moduleId);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Debug("EntityTypeModelerSync : SyncEntityTypeModeler -> Error" + ex.Message);
                errorMessage = ex.Message;
                //throw;
            }
            finally
            {
                mLogger.Debug("EntityTypeModelerSync : SyncEntityTypeModeler -> End");
            }
        }

        private void FailAllDependencies(ObjectSet deltaSet, List<string> failedEntityTypes)
        {
            Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
            foreach (ObjectTable table in deltaSet.Tables)
            {
                dependencies.Add(table.TableName, null);

                dependencies[table.TableName] =
                table.AsEnumerable()
                    .Where
                    (
                        t => (failedEntityTypes.SRMContainsWithIgnoreCase(RMCommonStatic.ConvertToLower(t[RMModelerConstants.ENTITY_DISPLAY_NAME])) && string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(t[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(t[RMCommonConstants.MIGRATION_COL_STATUS])))
                    );
            }

            new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ET_NOT_PROCESSED, true);
        }

        //private void FailAllAttributeDependencies(ObjectSet deltaSet, List<string> failedAttributes)
        //{
        //    Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
        //    foreach (ObjectTable table in deltaSet.Tables)
        //    {
        //        dependencies.Add(table.TableName, null);

        //        dependencies[table.TableName] =
        //        table.AsEnumerable()
        //            .Where
        //            (
        //                t => (failedAttributes.Contains(RMCommonStatic.ConvertToLower(t[RMModelerConstants.Attribute_Name])))
        //            );
        //    }

        //    new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ATTR_NOT_PROCESSED, true);
        //}
    }
}
