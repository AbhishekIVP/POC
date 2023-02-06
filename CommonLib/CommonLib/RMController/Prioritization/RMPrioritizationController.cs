using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public class RMPrioritizationController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMPrioritizationDBManager");
        public ObjectSet GetPrioritizationConfiguration(int moduleId, List<int> entityTypes, bool requireIdColumns)
        {

            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            ObjectSet osPrioritization = new ObjectSet();
            ObjectSet osConfiguration = new RMPrioritizationDBManager(null).GetPrioritizationConfiguration(moduleId, string.Join(",", entityTypes));
            if (osConfiguration != null && osConfiguration.Tables.Count > 0)
            {
                if (!requireIdColumns)
                {
                    List<string> colsToRemove = new List<string>() { RMModelerConstants.ENTITY_TYPE_ID };
                    new RMCommonController().RemoveColumnsFromObjectTable(osConfiguration.Tables[0], colsToRemove);

                    colsToRemove = new List<string>() { RMPrioritizationConstants.ENTITY_TYPE_ID, RMModelerConstants.MAIN_ENTITY_TYPE_ID, RMPrioritizationConstants.UNIQUE_KEY_ID, RMPrioritizationConstants.DATA_SOURCE_ID };
                    new RMCommonController().RemoveColumnsFromObjectTable(osConfiguration.Tables[1], colsToRemove);
                    new RMCommonController().RemoveColumnsFromObjectTable(osConfiguration.Tables[2], colsToRemove);
                }
                osPrioritization.Tables.Add(osConfiguration.Tables[0].Copy());
                osPrioritization.Tables[0].TableName = RMPrioritizationConstants.TABLE_ENTITY_TYPE_CONFIG;
                osPrioritization.Tables.Add(PreparePrioritizationTable(osConfiguration.Tables[1]));
                osPrioritization.Tables[1].TableName = RMPrioritizationConstants.TABLE_ATTRIBUTE_PRIORITIZATION;
                osPrioritization.Tables.Add(osConfiguration.Tables[2].Copy());
                osPrioritization.Tables[2].TableName = RMPrioritizationConstants.TABLE_DATA_SOURCE_MERGING;
                return osPrioritization;
            }
            return null;
        }
        public ObjectTable PreparePrioritizationTable(ObjectTable sourceTable)
        {
            ObjectTable targetTable = new ObjectTable();
            int maxPriority = Convert.ToInt32(sourceTable.AsEnumerable().Max(row => row[RMPrioritizationConstants.PRIORITY]));

            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPrioritizationConstants.ENTITY_DISPLAY_NAME, DataType = typeof(String), DefaultValue = string.Empty });
            //columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPrioritizationConstants.TAB_NAME, DataType = typeof(String), DefaultValue = string.Empty });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPrioritizationConstants.ATTRIBUTE_DISPLAY_NAME, DataType = typeof(String), DefaultValue = string.Empty });
            for (int index = 1; index <= maxPriority; index++)
            {
                columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPrioritizationConstants.PRIORITY + " " + index, DataType = typeof(String), DefaultValue = string.Empty });
            }
            new RMCommonController().AddColumnsToObjectTable(targetTable, columnsToAdd);

            sourceTable.AsEnumerable().ToList().ForEach(row =>
            {
                string entityTypeName = Convert.ToString(row[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]);
                string attributeName = Convert.ToString(row[RMPrioritizationConstants.ATTRIBUTE_DISPLAY_NAME]);
                //string tabName = Convert.ToString(row[RMPrioritizationConstants.TAB_NAME]);
                string priority = Convert.ToString(row[RMPrioritizationConstants.PRIORITY]);
                string dataSourceName = Convert.ToString(row[RMPrioritizationConstants.DATA_SOURCE_NAME]);

                IEnumerable<ObjectRow> rowFound = targetTable.AsEnumerable().Where(r => Convert.ToString(r[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName)
                                     && Convert.ToString(r[RMPrioritizationConstants.ATTRIBUTE_DISPLAY_NAME]).SRMEqualWithIgnoreCase(attributeName));
                if (rowFound.Count() > 0)
                {
                    rowFound.First()[RMPrioritizationConstants.PRIORITY + " " + priority] = dataSourceName;
                }
                else
                {
                    ObjectRow newRow = targetTable.NewRow();
                    newRow[RMPrioritizationConstants.ENTITY_DISPLAY_NAME] = entityTypeName;
                    newRow[RMPrioritizationConstants.ATTRIBUTE_DISPLAY_NAME] = attributeName;
                    //newRow[RMPrioritizationConstants.TAB_NAME] = tabName;
                    newRow[RMPrioritizationConstants.PRIORITY + " " + priority] = dataSourceName;
                    targetTable.Rows.Add(newRow);
                }
            });

            return targetTable;
        }

        public Dictionary<string, List<string>> GetEntityTypeMappedVendors(List<int> entity_type_ids, RDBConnectionManager mDBCon = null)
        {
            mLogger.Debug("RMPrioritizationController -> GetEntityTypeMappedVendors -> Start");
            try
            {
                Dictionary<string, List<string>> vendorMappings = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                ObjectTable table = new RMPrioritizationDBManager(mDBCon).GetEntityTypeVendors(entity_type_ids);

                if (table != null && table.Rows.Count > 0)
                {
                    foreach (ObjectRow row in table.Rows)
                    {
                        string entityTypeName = Convert.ToString(row["entity_display_name"]);
                        string dataSourceName = Convert.ToString(row["data_source_name"]);

                        if (!vendorMappings.Keys.SRMContainsWithIgnoreCase(entityTypeName))
                        {
                            vendorMappings.Add(entityTypeName, new List<string>());
                        }
                        vendorMappings[entityTypeName].Add(dataSourceName);
                    }
                }

                return vendorMappings;
            }
            catch
            {
                mLogger.Debug("RMPrioritizationController -> GetEntityTypeMappedVendors -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMPrioritizationController -> GetEntityTypeMappedVendors -> End");
            }
        }

        public Dictionary<string, RMPrioritizationDataSourceMapping> GetEntityTypeDataSourceMapping(int moduleId, ref Dictionary<string, int> dataSourceNamevsId)
        {
            Dictionary<string, RMPrioritizationDataSourceMapping> mappingDetails = new Dictionary<string, RMPrioritizationDataSourceMapping>();
            dataSourceNamevsId = new Dictionary<string, int>();

            ObjectSet osMapping = new RMPrioritizationDBManager(null).GetPossibleDataSourceMapping(moduleId);
            if (osMapping != null && osMapping.Tables.Count > 0 && osMapping.Tables[0] != null && osMapping.Tables[0].Rows.Count > 0)
            {
                foreach (ObjectRow row in osMapping.Tables[0].Rows)
                {
                    string entityTypeName = Convert.ToString(row[RMPrioritizationConstants.ENTITY_DISPLAY_NAME]);
                    int entityTypeId = Convert.ToInt32(row[RMPrioritizationConstants.ENTITY_TYPE_ID]);
                    string legName = Convert.ToString(row[RMPrioritizationConstants.LEG_NAME]);
                    int legEntityTypeId = string.IsNullOrEmpty(legName) ? -1 : Convert.ToInt32(row[RMPrioritizationConstants.LEG_ENTITY_TYPE_ID]);
                    string dataSourceName = Convert.ToString(row[RMPrioritizationConstants.DATA_SOURCE_NAME]);
                    int dataSourceId = Convert.ToInt32(row[RMPrioritizationConstants.DATA_SOURCE_ID]);

                    if (!dataSourceNamevsId.ContainsKey(dataSourceName))
                        dataSourceNamevsId.Add(dataSourceName, dataSourceId);

                    if (mappingDetails.ContainsKey(entityTypeName))
                    {
                        RMPrioritizationDataSourceMapping info = mappingDetails[entityTypeName];
                        if (!string.IsNullOrEmpty(legName))
                        {
                            if (!info.Legs.ContainsKey(legName))
                                info.Legs.Add(legName, new RMPrioritizationDataSourceMapping() { EntityTypeId = legEntityTypeId, EntityTypeName = legName, DataSources = new List<string>() });

                            info.Legs[legName].DataSources.Add(dataSourceName);
                        }
                        else
                            info.DataSources.Add(dataSourceName);
                    }
                    else
                    {
                        RMPrioritizationDataSourceMapping info = new RMPrioritizationDataSourceMapping();
                        info.EntityTypeName = entityTypeName;
                        info.EntityTypeId = entityTypeId;
                        if (!string.IsNullOrEmpty(legName))
                        {
                            info.Legs.Add(legName, new RMPrioritizationDataSourceMapping()
                            {
                                EntityTypeId = legEntityTypeId,
                                EntityTypeName = legName,
                                DataSources = new List<string>() { dataSourceName }
                            });
                        }
                        else
                            info.DataSources.Add(dataSourceName);

                        mappingDetails.Add(entityTypeName, info);
                    }
                }
            }
            return mappingDetails;
        }
    }
}
