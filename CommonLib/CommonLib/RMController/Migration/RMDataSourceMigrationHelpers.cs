using com.ivp.rad.common;
using com.ivp.rad.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.common;
using com.ivp.rad.dal;
using com.ivp.commom.commondal;
using com.ivp.srmcommon;
using System.Data;
using com.ivp.srm.vendorapi;
using System.Collections;
using System.Xml.Linq;
using com.ivp.common.SecMaster;

namespace com.ivp.common.Migration
{
    public partial class RMDataSourceMigrationSync
    {
        private void AddAttributeToXMLNode(XElement element, string value, string text, string key, string name, string vendorPropName)
        {
            element.Add(new XAttribute("value", value));
            element.Add(new XAttribute("text", text));
            element.Add(new XAttribute("key", key));
            element.Add(new XAttribute("name", name));
            element.Add(new XAttribute("vendorpropertyname", vendorPropName));
        }

        private bool GetBooleanValue(string boolString)
        {
            if (boolString.ToLower() == "1" || boolString.ToLower() == "true")
                return true;
            else
                return false;
        }

        private void FailAllDependencies(ObjectSet deltaSet)
        {
            Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
            foreach (ObjectTable table in deltaSet.Tables)
            {
                dependencies.Add(table.TableName, null);

                dependencies[table.TableName] =
                table.AsEnumerable()
                    .Where
                    (
                        t => (!processedDataSourceVsFeed.Keys.SRMContainsWithIgnoreCase(RMCommonStatic.ConvertToLower(t[RMDataSourceConstants.Data_Source_Name])))
                        ||
                        (
                            processedDataSourceVsFeed.Keys.SRMContainsWithIgnoreCase(RMCommonStatic.ConvertToLower(t[RMDataSourceConstants.Data_Source_Name]))
                            &&
                            !processedDataSourceVsFeed[RMCommonStatic.ConvertToLower(t[RMDataSourceConstants.Data_Source_Name])].SRMContainsWithIgnoreCase(RMCommonStatic.ConvertToLower(t[RMDataSourceConstants.Feed_Name]))
                        )
                    );
            }

            new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.DS_FEED_NOT_PROCESSED, true);

        }

        private void FailDependentSheets(ObjectSet deltaSet, List<string> feeds, List<string> includeSheetNames, string errorMessage)
        {
            Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();

            if (feeds != null && feeds.Count > 0)
            {
                foreach (ObjectTable table in deltaSet.Tables)
                {
                    if (table != null && includeSheetNames.SRMContainsWithIgnoreCase(table.TableName))
                    {
                        dependencies.Add(table.TableName, null);

                        dependencies[table.TableName] =
                        table.AsEnumerable()
                            .Where
                            (
                                t => string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS]))
                                && feeds.SRMContainsWithIgnoreCase(Convert.ToString(t[RMDataSourceConstants.Feed_Name]))
                            );
                    }
                }

                new RMCommonMigration().FailDependencies(dependencies, errorMessage, true);
            }
        }

        private void FailFeedDependencies(ObjectSet deltaSet, string feedName, string dataSourceName)
        {
            Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();

            foreach (ObjectTable table in deltaSet.Tables)
            {
                dependencies.Add(table.TableName, null);

                dependencies[table.TableName] =
                table.AsEnumerable()
                    .Where
                    (
                        t => string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS]))
                        && Convert.ToString(t[RMDataSourceConstants.Data_Source_Name]).SRMEqualWithIgnoreCase(dataSourceName)
                        && Convert.ToString(t[RMDataSourceConstants.Feed_Name]).SRMEqualWithIgnoreCase(feedName)
                    );
            }

            new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.FEED_NOT_PROCESSED, true);

        }

        private void FailFeedMappingDependencies(List<ObjectTable> tables, string feedName, string dataSourceName, string entityTypeName)
        {
            Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
            foreach (ObjectTable table in tables)
            {
                if (table != null)
                {
                    dependencies.Add(table.TableName, null);

                    dependencies[table.TableName] =
                    table.AsEnumerable()
                        .Where
                        (
                            t => string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS]))
                            && Convert.ToString(t[RMDataSourceConstants.Data_Source_Name]).SRMEqualWithIgnoreCase(dataSourceName)
                            && Convert.ToString(t[RMDataSourceConstants.Feed_Name]).SRMEqualWithIgnoreCase(feedName)
                            && Convert.ToString(t[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName)
                        );
                }
            }

            new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.FEED_MAPPING_NOT_PROCESSED, true);

        }

        private void FailFeedFieldDependencies(ObjectSet deltaSet, List<string> tableNames, string feedName, string dataSourceName, string fieldName)
        {
            Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
            foreach (ObjectTable table in deltaSet.Tables.Where(t => tableNames.SRMContainsWithIgnoreCase(t.TableName)))
            {
                dependencies.Add(table.TableName, null);

                dependencies[table.TableName] =
                table.AsEnumerable()
                    .Where
                    (
                        t => string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS]))
                        && Convert.ToString(t[RMDataSourceConstants.Data_Source_Name]).SRMEqualWithIgnoreCase(dataSourceName)
                        && Convert.ToString(t[RMDataSourceConstants.Feed_Name]).SRMEqualWithIgnoreCase(feedName)
                        && Convert.ToString(t[RMDataSourceConstants.Feed_Field_Name]).SRMEqualWithIgnoreCase(fieldName)
                    );
            }

            new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.FEED_FIELD_NOT_PROCESSED, true);

        }

        private void FailDataSourceDependencies(ObjectSet deltaSet, string dataSourceName)
        {
            Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
            foreach (ObjectTable table in deltaSet.Tables)
            {
                dependencies.Add(table.TableName, null);

                dependencies[table.TableName] =
                table.AsEnumerable()
                    .Where
                    (
                        t => string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS]))
                        && Convert.ToString(t[RMDataSourceConstants.Data_Source_Name]).SRMEqualWithIgnoreCase(dataSourceName)
                    );
            }

            new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.DS_NOT_PROCESSED, true);

        }

        private void FailDataSourceGroup(List<ObjectRow> rows, List<string> errors, bool setNotProcessed)
        {
            rows.ForEach(row =>
            {
                new RMCommonMigration().SetFailedRow(row, errors, setNotProcessed);
            });
        }

        private bool CheckDataSourceAndFeedExists(string dataSourceName, string feedName)
        {
            if (!processedDataSourceVsFeed.Keys.SRMContainsWithIgnoreCase(dataSourceName)
                ||
                (
                processedDataSourceVsFeed.Keys.SRMContainsWithIgnoreCase(dataSourceName)
                && !processedDataSourceVsFeed[dataSourceName].SRMContainsWithIgnoreCase(feedName)
                )
               )
                return false;


            return true;


        }

        private void SetExistingDataSourceInfo(ObjectSet dbSet)
        {
            dbSet.Tables[RMDataSourceConstants.TABLE_DATA_SOURCES].AsEnumerable().ToList().ForEach(row =>
            {
                existingDataSources.Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name]));
                processedDataSources.Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name]));

                if (!existingDataSourceVsFeed.ContainsKey(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name])))
                    existingDataSourceVsFeed.Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name]), new HashSet<string>());

                existingDataSourceVsFeed[RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name])].Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Name]));

                if (!processedDataSourceVsFeed.ContainsKey(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name])))
                    processedDataSourceVsFeed.Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name]), new HashSet<string>());

                processedDataSourceVsFeed[RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name])].Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Name]));

                if (!dictDataSources.Keys.SRMContainsWithIgnoreCase(Convert.ToString(row[RMDataSourceConstants.Data_Source_Name])))
                    dictDataSources.Add(Convert.ToString(row[RMDataSourceConstants.Data_Source_Name]), Convert.ToInt32(row[RMDataSourceConstants.Data_Source_ID]));

            });

            dbSet.Tables[RMDataSourceConstants.TABLE_FEED_FIELDS].AsEnumerable().ToList().ForEach(row =>
            {
                if (!processedFeedVsFields.ContainsKey(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Name])))
                {
                    processedFeedVsFields.Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Name]), new HashSet<string>());
                }
                processedFeedVsFields[RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Name])].Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Field_Name]));
            });

        }

        private void RefreshExistingDataSourceInfo(ObjectSet dbSet)
        {
            dbSet.Tables[RMDataSourceConstants.TABLE_DATA_SOURCES].AsEnumerable().ToList().ForEach(row =>
            {
                processedDataSources.Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name]));

                if (!processedDataSourceVsFeed.ContainsKey(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name])))
                    processedDataSourceVsFeed.Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name]), new HashSet<string>());

                processedDataSourceVsFeed[RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name])].Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Name]));
            });

            dbSet.Tables[RMDataSourceConstants.TABLE_FEED_FIELDS].AsEnumerable().ToList().ForEach(row =>
            {
                if (!processedFeedVsFields.ContainsKey(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Name])))
                {
                    processedFeedVsFields.Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Name]), new HashSet<string>());
                }
                processedFeedVsFields[RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Name])].Add(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Feed_Field_Name]));
            });

        }

        private List<ObjectRow> GetFeedFieldsForSync(ObjectSet deltaSet, string feedName, string dataSourceName)
        {
            List<ObjectRow> feedFieldRows = new List<ObjectRow>();

            if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_FEED_FIELDS) && deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_FIELDS] != null)
            {
                feedFieldRows = deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_FIELDS].AsEnumerable()
                    .Where(r => string.IsNullOrEmpty(Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]))
                    && RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Data_Source_Name]).SRMEqualWithIgnoreCase(dataSourceName)
                    && RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Name]).SRMEqualWithIgnoreCase(feedName)
                    ).ToList();
            }

            return feedFieldRows;
        }

        private List<ObjectRow> MergeExistingFeedFields(List<ObjectRow> currentRows, ObjectTable dbTable, string feedName, string dataSourceName)
        {
            List<string> fieldsInFile = currentRows.Select(c => RMCommonStatic.ConvertToLower(c[RMDataSourceConstants.Feed_Field_Name])).Distinct().ToList();

            List<ObjectRow> dbRows = dbTable.AsEnumerable()
                .Where(r => RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Data_Source_Name]).SRMEqualWithIgnoreCase(dataSourceName)
                    && RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Name]).SRMEqualWithIgnoreCase(feedName)
                    && !fieldsInFile.SRMContainsWithIgnoreCase(RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Field_Name]))
                    ).ToList();

            if (dbRows != null)
                currentRows.AddRange(dbRows);

            return dbTable.AsEnumerable()
                .Where(r => RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Data_Source_Name]).SRMEqualWithIgnoreCase(dataSourceName)
                    && RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Name]).SRMEqualWithIgnoreCase(feedName)).ToList();
        }

        private List<string> CheckFileProperties(int fileTypeID, ObjectRow row)
        {
            List<string> errors = new List<string>();
            switch (fileTypeID)
            {
                case 0:
                    ValidateFixedWidthFeed(row);
                    break;
                case 1:
                    errors = ValidateDelimitedFeed(row);
                    break;
                case 2:
                    errors = ValidateXMLFeed(row);
                    break;
                case 3:
                    break;
                case 4:
                    ValidateLoadFromDBFeed(row);
                    break;
                default:
                    break;
            }

            return errors;
        }

        private bool IsFeedTypeChanged(string feedName, string feedType, ObjectTable dbTable)
        {
            if (dbTable.AsEnumerable().Any(d => RMCommonStatic.ConvertToLower(d[RMDataSourceConstants.Feed_Name]).SRMEqualWithIgnoreCase(feedName)
            && !string.IsNullOrEmpty(Convert.ToString(d[RMDataSourceConstants.Feed_Type])) &&
            RMCommonStatic.ConvertToLower(d[RMDataSourceConstants.Feed_Type]).SRMEqualWithIgnoreCase(feedType) == false))
                return true;

            return false;
        }

        private bool IsFeedAndFileTypeValid(string feedType, string fileType, Dictionary<string, List<string>> feedVsFileTypes)
        {
            bool isValid = true;
            if (!feedVsFileTypes.ContainsKey(RMCommonStatic.ConvertToLower(feedType)))
                isValid = false;

            else
            {
                List<string> lst = feedVsFileTypes.Where(f => f.Key.SRMEqualWithIgnoreCase(feedType)).Select(t => t.Value).FirstOrDefault();
                if (!(lst != null && lst.Count > 0 && lst.SRMContainsWithIgnoreCase(fileType)))
                    isValid = false;
            }

            return isValid;
        }

        private bool AssignExistingDataSource(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMDataSourceConstants.Data_Source_ID] = Convert.ToInt32(dbRow[RMDataSourceConstants.Data_Source_ID]);
            return true;
        }

        private bool AssignExistingDataSourceAndFeedID(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[RMDataSourceConstants.FEED_ID] = Convert.ToInt32(dbRow[RMDataSourceConstants.FEED_ID]);
            sourceRow[RMDataSourceConstants.Data_Source_ID] = Convert.ToInt32(dbRow[RMDataSourceConstants.Data_Source_ID]);
            return true;
        }

        private bool AssignExistingFeedData(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMDataSourceConstants.Data_Source_ID] = Convert.ToInt32(dbRow[RMDataSourceConstants.Data_Source_ID]);
            return true;
        }

        private void ConvertToDataTableFromInfo(RMEntityTypeFeedMappingDetailsInfo objInfo, DataTable dtEFMappedDetails)
        {
            DataRow dr;
            dr = dtEFMappedDetails.NewRow();
            dr["entity_type_feed_mapping_id"]
                = objInfo.EntityTypeFeedMappingId;
            dr[RMModelerConstants.ENTITY_ATTRIBUTE_ID]
                = objInfo.EntityAttributeId;
            dr[RMDataSourceConstants.Feed_Field_ID]
                = objInfo.FieldId;
            dr["data_format"]
                = objInfo.DataFormat;
            dr[RMDBCommonConstantsInfo.CREATED_BY]
                = objInfo.CreatedBy;
            dr[RMDBCommonConstantsInfo.LAST_MODIFIED_BY]
                = objInfo.LastModifiedBy;
            dr[RMDBCommonConstantsInfo.CREATED_ON] = DateTime.Now;
            dr[RMDBCommonConstantsInfo.LAST_MODIFIED_ON] = DateTime.Now;

            dr["update_blank"] = objInfo.UpdateBlank == "true" ? 1 : 0;

            dtEFMappedDetails.Rows.Add(dr);
        }
    }
}
