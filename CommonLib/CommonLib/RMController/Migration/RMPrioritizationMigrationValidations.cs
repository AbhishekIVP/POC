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
        const string PRIORITY_SUFFIX = "Priority ";

        private List<string> ValidateFailedMetaDataRows(ObjectTable tableEntityType, ObjectTable tableAttributes, ObjectTable tableMerging)
        {
            mLogger.Debug("RMPrioritizationSync -> ValidateFailedMetaDataRows -> Start");
            List<string> failedEntityTypes = new List<string>();

            try
            {

                if (tableEntityType != null)
                    failedEntityTypes.AddRange(tableEntityType.AsEnumerable().Where(row => Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(row => Convert.ToString(row[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]).Trim()).Distinct().ToList());

                if (tableAttributes != null)
                    failedEntityTypes.AddRange(tableAttributes.AsEnumerable().Where(row => Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(row => Convert.ToString(row[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]).Trim()).Distinct().ToList());

                if (tableMerging != null)
                    failedEntityTypes.AddRange(tableMerging.AsEnumerable().Where(row => Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(row => Convert.ToString(row[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]).Trim()).Distinct().ToList());

                if (failedEntityTypes.Distinct().Count() > 0)
                {
                    mLogger.Debug("Failed Entity Type Count: " + failedEntityTypes.Distinct().Count().ToString());
                    FailPrioritizationDependencies(tableEntityType, failedEntityTypes);
                    FailPrioritizationDependencies(tableAttributes, failedEntityTypes);
                    FailPrioritizationDependencies(tableMerging, failedEntityTypes);
                }
                return failedEntityTypes;
            }
            catch
            {
                mLogger.Debug("RMPrioritizationSync -> ValidateFailedMetaDataRows -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMPrioritizationSync -> ValidateFailedMetaDataRows -> End");
            }
        }


        private void FailPrioritizationDependencies(ObjectTable table, List<string> failedEntityTypes)
        {
            if (table != null)
            {
                table.Rows.ToList().ForEach(row =>
                {
                    string entityTypeName = Convert.ToString(row[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]).Trim();

                    if (failedEntityTypes.SRMContainsWithIgnoreCase(entityTypeName) && string.IsNullOrEmpty(Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS])))
                    {
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.PRIORITIZATION_NOT_PROCESSED }, true);
                    }

                });
            }
        }

        private string ValidatePriorityColumns(ObjectTable table, ObjectTable dbTable, ref int priorityColumnCount)
        {
            string error = string.Empty;

            if (table == null)
            {
                table = dbTable.Copy();
            }

            List<string> priorityColumns = table.Columns.Where(c => c.ColumnName.StartsWith(PRIORITY_SUFFIX, StringComparison.OrdinalIgnoreCase)).Select(c => c.ColumnName.Trim()).ToList();

            priorityColumnCount = priorityColumns.Count;

            if (priorityColumns.Count <= 0)
                error = "Priority columns do not exist.";
            else
            {
                for (int i = 1; i <= priorityColumns.Count; i++)
                {
                    if (!priorityColumns.SRMContainsWithIgnoreCase(PRIORITY_SUFFIX + i.ToString()))
                        error = error + PRIORITY_SUFFIX + i.ToString() + " does not exist. ";
                }
            }

            return error;
        }


        public List<RMEntityPrioritizationInfo> ValidateAttributePriorities(List<ObjectRow> mappingRows, RMEntityDetailsInfo entityTypeInfo, List<string> dataSources, Dictionary<int, string> dictAllDS, int priorityColumnCount, string userName, DateTime syncDateTime, out string errorMessage, ref bool isValid, ref string distinctEntityTypeIds, ref bool isAllInSync)
        {
            List<RMEntityPrioritizationInfo> attributePriority = new List<RMEntityPrioritizationInfo>();
            RMEntityPrioritizationInfo pInfo = null;
            errorMessage = string.Empty;
            List<string> systemDS = dictAllDS.Where(d => d.Key <= 0).Select(d => d.Value).ToList();
            int maxCount = dataSources.Count;
            List<string> attributesInFile = new List<string>();
            distinctEntityTypeIds = entityTypeInfo.EntityTypeID.ToString() + "|";

            foreach (ObjectRow row in mappingRows)
            {
                bool isRowValid = true;
                pInfo = new RMEntityPrioritizationInfo();
                pInfo.CreatedBy = userName;
                pInfo.CreatedOn = syncDateTime;
                pInfo.IsActive = true;
                pInfo.LastModifiedBy = userName;
                pInfo.LastModifiedOn = syncDateTime;
                pInfo.row = null;
                pInfo.dspriorityInfoObjectArrayList = new List<RMDatasourcePriorityInfo>();
                int entityAttributeID = -1;
                int dataSourceID = -10;
                bool hasSystemDS = false;
                bool hasUserDS = false;
                bool hasDoNotUpdate = false;
                bool reachedMax = false;
                string attributeName = Convert.ToString(row[RMPrioritizationConstants.ATTRIBUTE_DISPLAY_NAME]).Trim();
                string entityTypeName = Convert.ToString(row[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]).Trim();
                attributesInFile.Add(attributeName);
                List<string> mappedDS = new List<string>();
                string mappedSystemDS = string.Empty;

                if (!Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_IN_SYNC))
                    isAllInSync = false;

                #region Attribute Validation
                if (entityTypeInfo.Attributes != null && entityTypeInfo.Attributes.Keys.SRMContainsWithIgnoreCase(attributeName))
                {
                    entityAttributeID = entityTypeInfo.Attributes[attributeName].EntityAttributeID;
                }

                if (entityTypeInfo.Legs != null)
                {
                    foreach (KeyValuePair<string, RMEntityDetailsInfo> leg in entityTypeInfo.Legs)
                    {
                        if (entityAttributeID > 0)
                            break;

                        if (leg.Value != null && leg.Value.Attributes != null && leg.Value.Attributes.Keys.SRMContainsWithIgnoreCase(attributeName))
                        {
                            entityAttributeID = leg.Value.Attributes[attributeName].EntityAttributeID;
                        }
                    }
                }

                if (entityAttributeID <= 0)
                {
                    isValid = false;
                    isRowValid = false;
                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.ENTITY_ATTRIBUTE_NOT_FOUND }, false);
                    AddFailedEntityType(entityTypeName);
                }
                else
                    pInfo.EntityAttributeId = entityAttributeID;

                #endregion Attribute Validation

                #region Data Source Validation

                for (int i = 1; i <= priorityColumnCount; i++)
                {
                    string colName = PRIORITY_SUFFIX + i.ToString();
                    string dataSourceName = Convert.ToString(row[colName]).Trim();
                    dataSourceID = -10;

                    if (i > maxCount && !reachedMax)
                    {
                        if (!string.IsNullOrEmpty(dataSourceName))
                        {
                            isValid = false;
                            isRowValid = false;
                            reachedMax = true;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { "Attribute cannot have priority more than " + maxCount.ToString() }, false);
                            AddFailedEntityType(entityTypeName);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(dataSourceName))
                        {
                            isValid = false;
                            isRowValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { colName + " cannot be empty." }, false);
                            AddFailedEntityType(entityTypeName);
                        }
                        else if (isRowValid)
                        {
                            if (dataSources.SRMContainsWithIgnoreCase(dataSourceName))
                            {
                                hasUserDS = true;
                                dataSourceID = GetDataSourceIdFromName(dataSourceName, dictAllDS);
                            }
                            else if (systemDS.SRMContainsWithIgnoreCase(dataSourceName))
                            {
                                dataSourceID = GetDataSourceIdFromName(dataSourceName, dictAllDS);
                                if (dataSourceID == -1)
                                    hasDoNotUpdate = true;
                                else
                                {
                                    hasSystemDS = true;
                                    if (string.IsNullOrEmpty(mappedSystemDS))
                                        mappedSystemDS = dataSourceName;
                                }
                            }
                            else
                            {
                                isValid = false;
                                isRowValid = false;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_DATA_SOURCE_MAPPED }, false);
                                AddFailedEntityType(entityTypeName);
                            }

                            if ((hasUserDS && hasSystemDS) || (hasDoNotUpdate && hasSystemDS) || (hasDoNotUpdate && dataSourceID > 0) || (hasSystemDS && mappedSystemDS != dataSourceName))
                            {
                                isValid = false;
                                isRowValid = false;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INCORRECT_DATA_SOURCE_MAPPING }, false);
                                AddFailedEntityType(entityTypeName);
                            }

                            if (mappedDS.SRMContainsWithIgnoreCase(dataSourceName) && dataSources.SRMContainsWithIgnoreCase(dataSourceName))
                            {
                                isValid = false;
                                isRowValid = false;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.DUPLICATE_DATA_SOURCE_MAPPING }, false);
                                AddFailedEntityType(entityTypeName);
                            }
                            else mappedDS.Add(dataSourceName);
                        }
                    }

                    if (isRowValid && dataSourceID >= -5)
                    {
                        pInfo.dspriorityInfoObjectArrayList.Add(new RMDatasourcePriorityInfo() { DataSourceId = dataSourceID, Priority = i });
                    }
                }

                if (isRowValid)
                    attributePriority.Add(pInfo);

                #endregion Data Source Validation

            }

            if (!isValid)
            {
                //mappingRows.Where(row => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMCommonConstants.MIGRATION_COL_REMARKS])))
                //    .ToList().ForEach(row => {
                //        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.PRIORITIZATION_NOT_PROCESSED }, true);
                //    });
            }
            else
            {
                #region Adding attributes not present in file
                if (entityTypeInfo.Attributes != null)
                {
                    foreach (KeyValuePair<string, RMEntityAttributeInfo> kvp in entityTypeInfo.Attributes)
                    {
                        if (!attributesInFile.SRMContainsWithIgnoreCase(kvp.Key))
                        {
                            pInfo = new RMEntityPrioritizationInfo();
                            pInfo.CreatedBy = userName;
                            pInfo.CreatedOn = syncDateTime;
                            pInfo.IsActive = true;
                            pInfo.LastModifiedBy = userName;
                            pInfo.LastModifiedOn = syncDateTime;
                            pInfo.row = null;
                            pInfo.EntityAttributeId = kvp.Value.EntityAttributeID;
                            pInfo.dspriorityInfoObjectArrayList = new List<RMDatasourcePriorityInfo>();

                            for (int i = 1; i <= maxCount; i++)
                            {
                                pInfo.dspriorityInfoObjectArrayList.Add(new RMDatasourcePriorityInfo() { DataSourceId = -1, Priority = i });
                            }
                            attributePriority.Add(pInfo);
                        }
                    }
                }

                if (entityTypeInfo.Legs != null)
                {
                    foreach (KeyValuePair<string, RMEntityDetailsInfo> leg in entityTypeInfo.Legs)
                    {
                        if (leg.Value != null && leg.Value.Attributes != null)
                        {
                            distinctEntityTypeIds = distinctEntityTypeIds + leg.Value.EntityTypeID.ToString() + "|";

                            foreach (KeyValuePair<string, RMEntityAttributeInfo> kvp in leg.Value.Attributes)
                            {
                                if (!attributesInFile.SRMContainsWithIgnoreCase(kvp.Key))
                                {
                                    pInfo = new RMEntityPrioritizationInfo();
                                    pInfo.CreatedBy = userName;
                                    pInfo.CreatedOn = syncDateTime;
                                    pInfo.IsActive = true;
                                    pInfo.LastModifiedBy = userName;
                                    pInfo.LastModifiedOn = syncDateTime;
                                    pInfo.row = null;
                                    pInfo.EntityAttributeId = kvp.Value.EntityAttributeID;
                                    pInfo.dspriorityInfoObjectArrayList = new List<RMDatasourcePriorityInfo>();

                                    for (int i = 1; i <= maxCount; i++)
                                    {
                                        pInfo.dspriorityInfoObjectArrayList.Add(new RMDatasourcePriorityInfo() { DataSourceId = -1, Priority = i });
                                    }
                                    attributePriority.Add(pInfo);
                                }
                            }
                        }
                    }
                }
                #endregion Adding attributes not present in file
            }

            return attributePriority;
        }

        public string ValidatePrioritizationMergeMapping(List<ObjectRow> mergingRows, Dictionary<int, string> dictMergableUniqueKeys, List<string> dataSources, Dictionary<int, string> dictAllDS, string userName, DateTime syncDateTime, out string errorMessage, ref bool isValid, ref bool isAllInSync)
        {
            string xmlString = string.Empty;
            errorMessage = string.Empty;
            List<string> passedDS = new List<string>();

            XElement mergingXML = new XElement("DatasourceAttributeMappingInfo");

            if (mergingRows != null)
            {
                foreach (ObjectRow row in mergingRows)
                {
                    if (!Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_IN_SYNC))
                        isAllInSync = false;

                    string entityTypeName = Convert.ToString(row[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]).Trim();
                    string dataSourceName = Convert.ToString(row[RMPrioritizationConstants.DATA_SOURCE_NAME]).Trim();
                    string uniqueKeyName = Convert.ToString(row[RMPrioritizationConstants.UNIQUE_KEY_NAME]).Trim();
                    bool isInsert = Convert.ToBoolean(row[RMPrioritizationConstants.IS_INSERT]);
                    int dataSourceID = -10;
                    int uniqueKeyId = -1;

                    if (!dataSources.SRMContainsWithIgnoreCase(dataSourceName))
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_DATA_SOURCE }, false);
                        AddFailedEntityType(entityTypeName);
                    }
                    else
                        dataSourceID = GetDataSourceIdFromName(dataSourceName, dictAllDS);

                    foreach (KeyValuePair<int, string> kvp in dictMergableUniqueKeys)
                    {
                        if (kvp.Value.SRMEqualWithIgnoreCase(uniqueKeyName))
                        {
                            uniqueKeyId = kvp.Key;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(uniqueKeyName) && uniqueKeyId <= 0)
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_MERGING_KEY }, false);
                        AddFailedEntityType(entityTypeName);
                    }

                    if (isValid)
                    {
                        passedDS.Add(dataSourceName);
                        AddMergingXMLToParentNode(mergingXML, uniqueKeyId, dataSourceID, userName, isInsert ? "true" : "false");
                    }
                }
            }

            dataSources.ForEach(ds =>
            {
                if (!passedDS.SRMContainsWithIgnoreCase(ds))
                {
                    AddMergingXMLToParentNode(mergingXML, -1, GetDataSourceIdFromName(ds, dictAllDS), userName, "false");
                }
            });

            xmlString = mergingXML.ToString();

            return xmlString;

        }

        private void CheckUnProcessedRows(ObjectTable attributeTable, ObjectTable mergingTable)
        {
            if (attributeTable != null)
            {
                attributeTable.AsEnumerable().Where(r => string.IsNullOrEmpty(Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).Trim()))
                    .ToList().ForEach(row =>
                {
                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.PRIORITIZATION_NOT_PROCESSED }, true);
                });

            }

            if (mergingTable != null)
            {
                mergingTable.AsEnumerable().Where(r => string.IsNullOrEmpty(Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).Trim()))
                    .ToList().ForEach(row =>
                {
                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.PRIORITIZATION_NOT_PROCESSED }, true);
                });

            }
        }


        private void AddMergingXMLToParentNode(XElement mergingXML, int uniqueKeyId, int dataSourceID, string userName, string isInsert)
        {
            XElement element = new XElement("DatasourceAttributePair");
            element.Add(new XElement("entity_attribute_id", uniqueKeyId > 0 ? uniqueKeyId : -1));
            element.Add(new XElement("data_source_id", dataSourceID));
            element.Add(new XElement("is_active", "true"));
            element.Add(new XElement("created_by", userName));
            element.Add(new XElement("last_modified_by", userName));
            element.Add(new XElement("is_insert", isInsert));

            mergingXML.Add(element);
        }

        private int GetDataSourceIdFromName(string dataSourceName, Dictionary<int, string> dict)
        {
            foreach (KeyValuePair<int, string> kvp in dict)
            {
                if (kvp.Value.SRMEqualWithIgnoreCase(dataSourceName))
                {
                    return kvp.Key;
                }
            }

            return -10;
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
                        t => (string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))
                        && failedEntityTypes.SRMContainsWithIgnoreCase(RMCommonStatic.ConvertToLower(t[RMPrioritizationConstants.ENTITY_DISPLAY_NAME])))
                    );
            }
            new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.PRIORITIZATION_NOT_PROCESSED, true);
        }

        private void AddFailedEntityType(string entityTypeName)
        {
            if (!failedEntityTypes.SRMContainsWithIgnoreCase(entityTypeName))
                failedEntityTypes.Add(entityTypeName);
        }

        private void AddStaticColumns(ObjectTable table)
        {
            if (!table.Columns.Contains(RMCommonConstants.MIGRATION_COL_STATUS))
                table.Columns.Add(RMCommonConstants.MIGRATION_COL_STATUS, typeof(string));

            if (!table.Columns.Contains(RMCommonConstants.MIGRATION_COL_REMARKS))
                table.Columns.Add(RMCommonConstants.MIGRATION_COL_REMARKS, typeof(string));
        }

    }
}
