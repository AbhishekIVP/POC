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
using com.ivp.rad.controls.xruleeditor.grammar;

namespace com.ivp.common.Migration
{
    public partial class RMDataSourceMigrationSync
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMDataSourceMigrationSync");
        ObjectSet objExistingFeeds = null;
        const string COL_IS_INSERT = "@isInsert";
        HashSet<string> existingDataSources = new HashSet<string>();
        HashSet<string> processedDataSources = new HashSet<string>();
        Dictionary<string, int> dictDataSources = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, HashSet<string>> existingDataSourceVsFeed = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, HashSet<string>> processedDataSourceVsFeed = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, HashSet<string>> processedFeedVsFields = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Entry method for syncing data source configurations
        /// </summary>
        /// <param name="source"></param>
        public void StartSync(ObjectSet deltaSet, int moduleID, string userName, DateTime syncDateTime, out string errorMessage)
        {
            mLogger.Debug("RMDataSourceMigrationSync : SyncDataSourceConfiguration -> Start");
            try
            {
                errorMessage = string.Empty;
                IEnumerable<ObjectRow> currentRows = null;
                objExistingFeeds = new RMDataSourceControllerNew().GetDataSourceConfiguration(null, -1, true, true);
                ObjectTable sourceTable = null;
                List<string> failedFeedNames = new List<string>();

                SetExistingDataSourceInfo(objExistingFeeds);
                Dictionary<string, RMEntityDetailsInfo> moduleEntityTypeDetails = new RMModelerController().GetEntityTypeDetails(moduleID, null);
                Dictionary<string, RMEntityDetailsInfo> allEntityTypeDetails = new RMModelerController().GetEntityTypeDetails(0, null);

                ObjectTable dbTableDataSources = objExistingFeeds.Tables[RMDataSourceConstants.TABLE_DATA_SOURCES];
                ObjectTable dbTableFeedFields = objExistingFeeds.Tables[RMDataSourceConstants.TABLE_FEED_FIELDS];
                ObjectTable dbTableFeedMapping = objExistingFeeds.Tables[RMDataSourceConstants.TABLE_FEED_MAPPING];

                #region Sheet 1 - Feed Summary
                //Sheet 1 - Data Sources
                if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_DATA_SOURCES) && deltaSet.Tables[RMDataSourceConstants.TABLE_DATA_SOURCES] != null
                    && deltaSet.Tables[RMDataSourceConstants.TABLE_DATA_SOURCES].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMDataSourceConstants.TABLE_DATA_SOURCES];


                    #region Check Failed Rows
                    sourceTable.AsEnumerable().Where(r => Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).ToList().ForEach(failedRow =>
                    {
                        string failedDataSourceName = Convert.ToString(failedRow[RMDataSourceConstants.Data_Source_Name]);
                        string failedFeedName = Convert.ToString(failedRow[RMDataSourceConstants.Feed_Name]);

                        if (!failedFeedNames.SRMContainsWithIgnoreCase(failedFeedName))
                            failedFeedNames.Add(failedFeedName);

                        if (processedDataSources.Contains(failedDataSourceName))
                            processedDataSources.Remove(failedDataSourceName);

                        foreach (KeyValuePair<string, HashSet<string>> kvp in processedDataSourceVsFeed)
                        {
                            if (processedDataSourceVsFeed.ContainsKey(failedDataSourceName) && processedDataSourceVsFeed[failedDataSourceName].Contains(failedFeedName))
                            {
                                processedDataSourceVsFeed[failedDataSourceName].Remove(failedFeedName);
                            }
                        }

                    });

                    //FailAllDependencies(deltaSet);

                    #endregion

                    #region Add Static Columns

                    List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.Data_Source_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.FEED_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });
                    new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

                    #endregion

                    #region Syncing Start
                    SyncDataSources(deltaSet, dbTableDataSources, sourceTable, userName);

                    var assign = from src in sourceTable.AsEnumerable()
                                 join db in dbTableDataSources.AsEnumerable()
                                   on new { feed = RMCommonStatic.ConvertToLower(src[RMDataSourceConstants.Feed_Name]), map = RMCommonStatic.ConvertToLower(src[RMDataSourceConstants.Data_Source_Name]) }
                                 equals new { feed = RMCommonStatic.ConvertToLower(db[RMDataSourceConstants.Feed_Name]), map = RMCommonStatic.ConvertToLower(db[RMDataSourceConstants.Data_Source_Name]) }
                                 select AssignExistingDataSourceAndFeedID(db, src);

                    assign.Count();

                    SyncFeedSummary(deltaSet, dbTableDataSources, dbTableFeedFields, sourceTable, userName, syncDateTime);

                    #endregion

                    #region Remove Static Columns
                    //Remove static columns

                    List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMDataSourceConstants.Data_Source_ID, RMDataSourceConstants.FEED_ID };

                    new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);
                    #endregion

                    FailDependentSheets(deltaSet, failedFeedNames, deltaSet.Tables.Select(t => t.TableName).ToList(), RMCommonConstants.FEED_NOT_PROCESSED);
                    failedFeedNames = new List<string>();
                }
                #endregion


                #region Sheet 2 - Feed Fields

                currentRows = null;
                if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_FEED_FIELDS) && deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_FIELDS] != null
                    && deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_FIELDS].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_FIELDS];

                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    if (currentRows != null)
                    {
                        mLogger.Debug("Rows to process in Feed Fields - " + currentRows.Count().ToString());
                        SyncFeedFields(currentRows, deltaSet, dbTableFeedFields, dbTableDataSources, userName, syncDateTime);
                    }

                    failedFeedNames = sourceTable.AsEnumerable().Where(r => Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(r => Convert.ToString(r[RMDataSourceConstants.Feed_Name])).Distinct().ToList();

                    FailDependentSheets(deltaSet, failedFeedNames, new List<string>() { RMDataSourceConstants.TABLE_FEED_MAPPING, RMDataSourceConstants.TABLE_FEED_RULES, RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_MAPPING, RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_RULES, RMDataSourceConstants.TABLE_BULK_LICENSE_SETUP, RMDataSourceConstants.TABLE_CUSTOM_CLASSES, RMDataSourceConstants.TABLE_API_LICENSE_SETUP, RMDataSourceConstants.TABLE_FTP_LICENSE_SETUP }, RMCommonConstants.FEED_FIELD_NOT_PROCESSED);
                    failedFeedNames = new List<string>();

                }
                #endregion

                #region Refreshing Feed and field db info

                objExistingFeeds = new RMDataSourceControllerNew().GetDataSourceConfiguration(null, -1, true, true);
                RefreshExistingDataSourceInfo(objExistingFeeds);

                #endregion

                #region Sheet 3 - Feed Mapping

                currentRows = null;
                if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_FEED_MAPPING) && deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_MAPPING] != null
                    && deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_MAPPING].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_MAPPING];

                    #region Adding static columns

                    List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.Feed_Mapping_Details_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.FEED_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.FILE_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.Mapped_Column_ID, DataType = typeof(Int32), DefaultValue = -1 });

                    new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

                    #endregion


                    var assign = from src in sourceTable.AsEnumerable()
                                 join db in dbTableFeedMapping.AsEnumerable()
                                 on new { feed = RMCommonStatic.ConvertToLower(src[RMDataSourceConstants.Feed_Name]), map = RMCommonStatic.ConvertToLower(src[RMDataSourceConstants.Mapped_Column_Name]) }
                                 equals new { feed = RMCommonStatic.ConvertToLower(db[RMDataSourceConstants.Feed_Name]), map = RMCommonStatic.ConvertToLower(db[RMDataSourceConstants.Mapped_Column_Name]) }
                                 select SetFeedMappingDetailsInfo(src, db);

                    assign.Count();

                    assign = from src in sourceTable.AsEnumerable()
                             join db in dbTableDataSources.AsEnumerable()
                             on new { feed = RMCommonStatic.ConvertToLower(src[RMDataSourceConstants.Feed_Name]) }
                             equals new { feed = RMCommonStatic.ConvertToLower(db[RMDataSourceConstants.Feed_Name]) }
                             select SetFeedSummaryID(src, db);

                    assign.Count();

                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    if (currentRows != null)
                    {
                        mLogger.Debug("Rows to process in Feed Mapping - " + currentRows.Count().ToString());
                        SyncFeedMapping(currentRows, deltaSet, dbTableFeedFields, dbTableDataSources, userName, syncDateTime);
                        //SyncFeedFields(currentRows, deltaSet, dbTableFeedFields, dbTableDataSources, userName, syncDateTime);
                    }

                    #region Removing Static columns

                    List<string> colsToRemove = new List<string>() { RMDataSourceConstants.FEED_ID, RMDataSourceConstants.FILE_ID, RMDataSourceConstants.Mapped_Column_ID, RMDataSourceConstants.Feed_Mapping_Details_ID };

                    new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);

                    #endregion

                }
                #endregion

                objExistingFeeds = new RMDataSourceControllerNew().GetDataSourceConfiguration(null, -1, true, true);
                dbTableDataSources = objExistingFeeds.Tables[RMDataSourceConstants.TABLE_DATA_SOURCES];

                #region Sheet 4 - Feed Rules

                Dictionary<string, RMFeedInfoDetailed> feedDetailsInfo = new RMDataSourceControllerNew().GetFeedDetailsPerFeed(null, -1, true);

                currentRows = null;
                if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_FEED_RULES) && deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_RULES] != null
                    && deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_RULES].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMDataSourceConstants.TABLE_FEED_RULES];

                    #region Adding static columns

                    List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.Feed_Mapping_Details_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.FEED_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.FILE_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.Mapped_Column_ID, DataType = typeof(Int32), DefaultValue = -1 });

                    new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

                    #endregion


                    var assign = from src in sourceTable.AsEnumerable()
                                 join db in dbTableFeedMapping.AsEnumerable()
                                 on new { feed = RMCommonStatic.ConvertToLower(src[RMDataSourceConstants.Feed_Name]), map = RMCommonStatic.ConvertToLower(src[RMDataSourceConstants.Map_Name]) }
                                 equals new { feed = RMCommonStatic.ConvertToLower(db[RMDataSourceConstants.Feed_Name]), map = RMCommonStatic.ConvertToLower(db[RMDataSourceConstants.Map_Name]) }
                                 select SetFeedMappingDetailsInfo(src, db);

                    ValidateDataSourceAndFeed(sourceTable.AsEnumerable(), false);

                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    if (currentRows != null)
                    {
                        mLogger.Debug("Rows to process in Feed Rules - " + currentRows.Count().ToString());
                        SyncFeedRules(currentRows, userName, syncDateTime, feedDetailsInfo);
                        // SyncFeedFields(currentRows, deltaSet, dbTableFeedFields, dbTableDataSources, userName, syncDateTime);
                    }

                    #region Removing Static columns

                    List<string> colsToRemove = new List<string>() { RMDataSourceConstants.FEED_ID, RMDataSourceConstants.FILE_ID, RMDataSourceConstants.Mapped_Column_ID, RMDataSourceConstants.Feed_Mapping_Details_ID };

                    new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);

                    #endregion
                }
                #endregion

                #region Sheet 5 - Feed Entity Type Mapping

                currentRows = null;
                if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_MAPPING) && deltaSet.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_MAPPING] != null
                    && deltaSet.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_MAPPING].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_MAPPING];

                    #region Adding static columns

                    List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.FEED_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.Data_Source_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.Feed_Field_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.ENTITY_ATTRIBUTE_ID, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.Lookup_Type_Id, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.Lookup_Attribute_Id, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.Lookup_Attribute_Real_Name, DataType = typeof(string), DefaultValue = string.Empty });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.Attribute_lookup_identity, DataType = typeof(Int32), DefaultValue = -1 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMDataSourceConstants.Lookup_Type_Ref_Sec, DataType = typeof(Int32), DefaultValue = -1 });

                    new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

                    #endregion

                    ValidateDataSourceAndFeed(sourceTable.AsEnumerable(), false);

                    var assign = from src in sourceTable.AsEnumerable()
                                 join db in dbTableDataSources.AsEnumerable()
                                   on new { feed = RMCommonStatic.ConvertToLower(src[RMDataSourceConstants.Feed_Name]), map = RMCommonStatic.ConvertToLower(src[RMDataSourceConstants.Data_Source_Name]) }
                                 equals new { feed = RMCommonStatic.ConvertToLower(db[RMDataSourceConstants.Feed_Name]), map = RMCommonStatic.ConvertToLower(db[RMDataSourceConstants.Data_Source_Name]) }
                                 select AssignExistingDataSourceAndFeedID(db, src);

                    assign.Count();

                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    if (currentRows != null)
                    {
                        mLogger.Debug("Rows to process in Feed Entity Type Mapping - " + currentRows.Count().ToString());
                        SyncFeedEntityTypeMapping(deltaSet, currentRows, moduleEntityTypeDetails, allEntityTypeDetails, userName, syncDateTime, moduleID, feedDetailsInfo);
                    }

                    #region Removing Static columns

                    List<string> colsToRemove = new List<string>() { RMDataSourceConstants.FEED_ID, RMDataSourceConstants.Feed_Field_ID, RMModelerConstants.ENTITY_ATTRIBUTE_ID, RMModelerConstants.Lookup_Type_Id, RMModelerConstants.Lookup_Attribute_Id, RMModelerConstants.Lookup_Attribute_Real_Name, RMModelerConstants.Attribute_lookup_identity, RMDataSourceConstants.Lookup_Type_Ref_Sec, RMDataSourceConstants.Data_Source_ID };

                    new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);

                    #endregion

                    failedFeedNames = sourceTable.AsEnumerable().Where(r => Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(r => Convert.ToString(r[RMDataSourceConstants.Feed_Name])).Distinct().ToList();

                    FailDependentSheets(deltaSet, failedFeedNames, new List<string>() { RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_RULES, RMDataSourceConstants.TABLE_BULK_LICENSE_SETUP, RMDataSourceConstants.TABLE_CUSTOM_CLASSES, RMDataSourceConstants.TABLE_API_LICENSE_SETUP, RMDataSourceConstants.TABLE_FTP_LICENSE_SETUP }, RMCommonConstants.FEED_MAPPING_NOT_PROCESSED);
                    failedFeedNames = new List<string>();
                }
                #endregion

                #region Refreshing Feed and field db info

                objExistingFeeds = new RMDataSourceControllerNew().GetDataSourceConfiguration(null, -1, true, true);
                RefreshExistingDataSourceInfo(objExistingFeeds);

                #endregion

                #region Sheet 6 - Feed Entity Type Rules

                currentRows = null;
                if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_RULES) && deltaSet.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_RULES] != null
                    && deltaSet.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_RULES].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_RULES];

                    #region Adding static columns

                    List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();

                    new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

                    #endregion

                    //Add static data here

                    ValidateDataSourceAndFeed(sourceTable.AsEnumerable(), false);

                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    if (currentRows != null)
                    {
                        mLogger.Debug("Rows to process in Feed Entity Type Rules - " + currentRows.Count().ToString());
                        SyncFeedEntityTypeRules(currentRows, userName, syncDateTime, moduleEntityTypeDetails, feedDetailsInfo);
                    }

                    #region Removing Static columns

                    List<string> colsToRemove = new List<string>() { };

                    new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);

                    #endregion
                }
                #endregion

                #region Sheet 7 - Bulk License

                currentRows = null;
                if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_BULK_LICENSE_SETUP) && deltaSet.Tables[RMDataSourceConstants.TABLE_BULK_LICENSE_SETUP] != null
                    && deltaSet.Tables[RMDataSourceConstants.TABLE_BULK_LICENSE_SETUP].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMDataSourceConstants.TABLE_BULK_LICENSE_SETUP];

                    ValidateDataSourceAndFeed(sourceTable.AsEnumerable(), false);

                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    if (currentRows != null)
                    {
                        mLogger.Debug("Rows to process in Bulk License - " + currentRows.Count().ToString());
                        SyncBulkLicenseSetup(deltaSet, currentRows, moduleEntityTypeDetails, feedDetailsInfo, userName, syncDateTime, moduleID);
                    }

                }
                #endregion

                #region Sheet 8 - Bulk License Custom Classes

                currentRows = null;
                if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_CUSTOM_CLASSES) && deltaSet.Tables[RMDataSourceConstants.TABLE_CUSTOM_CLASSES] != null
                    && deltaSet.Tables[RMDataSourceConstants.TABLE_CUSTOM_CLASSES].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMDataSourceConstants.TABLE_CUSTOM_CLASSES];

                    ValidateDataSourceAndFeed(sourceTable.AsEnumerable(), false);

                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    if (currentRows != null)
                    {
                        mLogger.Debug("Rows to process in Custom classes - " + currentRows.Count().ToString());
                        SyncCustomClasses(deltaSet, currentRows, userName, syncDateTime, moduleID);
                    }

                }
                #endregion

                #region Sheet 9 - API License (Import Task)

                currentRows = null;
                if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_API_LICENSE_SETUP) && deltaSet.Tables[RMDataSourceConstants.TABLE_API_LICENSE_SETUP] != null
                    && deltaSet.Tables[RMDataSourceConstants.TABLE_API_LICENSE_SETUP].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMDataSourceConstants.TABLE_API_LICENSE_SETUP];

                    ValidateDataSourceAndFeed(sourceTable.AsEnumerable(), false);

                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    if (currentRows != null)
                    {
                        mLogger.Debug("Rows to process in API License - " + currentRows.Count().ToString());
                        SyncAPILicenseSetup(deltaSet, currentRows, feedDetailsInfo, userName, syncDateTime, moduleID);
                    }

                }
                #endregion

                #region Sheet 10 - FTP License (Request/Response Task)

                currentRows = null;
                if (deltaSet.Tables.Contains(RMDataSourceConstants.TABLE_FTP_LICENSE_SETUP) && deltaSet.Tables[RMDataSourceConstants.TABLE_FTP_LICENSE_SETUP] != null
                    && deltaSet.Tables[RMDataSourceConstants.TABLE_FTP_LICENSE_SETUP].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[RMDataSourceConstants.TABLE_FTP_LICENSE_SETUP];

                    ValidateDataSourceAndFeed(sourceTable.AsEnumerable(), false);

                    currentRows = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));

                    if (currentRows != null)
                    {
                        mLogger.Debug("Rows to process in API License - " + currentRows.Count().ToString());
                        SyncFTPLicenseSetup(deltaSet, currentRows, feedDetailsInfo, userName, syncDateTime, moduleID);
                    }

                }
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMDataSourceMigrationSync : SyncDataSourceConfiguration -> Start");
                errorMessage = ex.Message;
            }
            finally
            {
                mLogger.Debug("RMDataSourceMigrationSync : SyncDataSourceConfiguration -> End");
            }
        }

        private void SyncDataSources(ObjectSet deltaSet, ObjectTable existingDataSources, ObjectTable sourceTable, string userName)
        {
            List<string> processedDS = new List<string>();
            bool isInsert = false;
            string dataSourceName = string.Empty;
            string feedName = string.Empty;
            string dataSourceDescription = string.Empty;
            int dataSourceID = -1;
            RDBConnectionManager mDBCon = null;

            var assign = from db in existingDataSources.AsEnumerable()
                         join upl in sourceTable.AsEnumerable()
                         on RMCommonStatic.ConvertToLower(db[RMDataSourceConstants.Data_Source_Name]) equals RMCommonStatic.ConvertToLower(upl[RMDataSourceConstants.Data_Source_Name])
                         select AssignExistingDataSource(db, upl);

            assign.Count();


            sourceTable.AsEnumerable()
                .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList().ForEach(row =>
                {
                    #region Processing Data Sources

                    isInsert = false;
                    //if (!processedDataSources.SRMContainsWithIgnoreCase(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Data_Source_Name])))
                    //    isInsert = true;

                    mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);

                    try
                    {
                        //isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);
                        dataSourceName = Convert.ToString(row[RMDataSourceConstants.Data_Source_Name]);
                        dataSourceDescription = Convert.ToString(row[RMDataSourceConstants.Data_Source_Description]);
                        dataSourceID = Convert.ToInt32(row[RMDataSourceConstants.Data_Source_ID]);

                        if (dictDataSources.Keys.SRMContainsWithIgnoreCase(dataSourceName))
                        {
                            dataSourceID = dictDataSources[dataSourceName];
                        }
                        else
                            isInsert = true;

                        processedDS.Add(RMCommonStatic.ConvertToLower(dataSourceName));

                        if (isInsert) //Add DataSource
                        {
                            RMDatasourceInfo dsInfo = new RMDatasourceInfo();
                            dsInfo.DatasourceName = dataSourceName;
                            dsInfo.DatasourceDescription = dataSourceDescription;
                            dsInfo.CreatedBy = userName;
                            dsInfo.LastModifiedBy = userName;
                            dsInfo.AccountID = 0;
                            dataSourceID = new RMDataSourceDBManager(mDBCon).AddDatasource(dsInfo);

                            row[RMDataSourceConstants.Data_Source_ID] = dataSourceID;
                            //row[RMCommonConstants.MIGRATION_COL_STATUS] = RMCommonConstants.MIGRATION_SUCCESS;
                            processedDataSources.Add(dataSourceName);
                            processedDataSourceVsFeed.Add(dataSourceName, new HashSet<string>());

                            if (!dictDataSources.Keys.SRMContainsWithIgnoreCase(dataSourceName))
                                dictDataSources.Add(dataSourceName, dataSourceID);

                        }
                        else //Update DataSource
                        {
                            RMDatasourceInfo dsInfo = new RMDatasourceInfo();
                            dsInfo.DatasourceName = dataSourceName;
                            dsInfo.DatasourceDescription = dataSourceDescription;
                            dsInfo.DatasourceID = dataSourceID;
                            dsInfo.LastModifiedBy = userName;

                            new RMDataSourceDBManager(mDBCon).UpdateDatasource(dsInfo);
                        }

                        new RMCommonController().CommitTransaction(mDBCon);
                    }
                    catch (Exception ex)
                    {
                        if (mDBCon != null)
                            mDBCon.RollbackTransaction();

                        new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                        FailDataSourceDependencies(deltaSet, dataSourceName);
                    }
                    finally
                    {
                        if (mDBCon != null)
                        {
                            RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                            mDBCon = null;
                        }
                    }


                    #endregion

                    #region Processing Feed 
                    feedName = Convert.ToString(row[RMDataSourceConstants.Feed_Name]);
                    dataSourceName = Convert.ToString(row[RMDataSourceConstants.Data_Source_Name]);
                    //dataSourceID = Convert.ToInt32(row[RMDataSourceConstants.Data_Source_ID]);
                    dataSourceID = dictDataSources[dataSourceName];

                    mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                    try
                    {
                        bool isFeedDuplicate = false;
                        foreach (KeyValuePair<string, HashSet<string>> kvp in processedDataSourceVsFeed)
                        {
                            if (!kvp.Key.SRMEqualWithIgnoreCase(dataSourceName) && kvp.Value.SRMContainsWithIgnoreCase(feedName))
                                isFeedDuplicate = true;
                        }

                        if (!isFeedDuplicate)
                        {
                            if (new RMDataSourceDBManager(mDBCon).GetFeedSummaryIdByName(feedName) == 0) //Add Feed if same feed name not exists
                            {
                                int feedSummaryID = new RMDataSourceDBManager(mDBCon).AddFeed(new RMFeedSummaryInfo()
                                {
                                    FeedName = feedName,
                                    DataSourceID = dataSourceID,
                                    FeedTypeID = 0,
                                    CreatedBy = userName,
                                    LastModifiedBy = userName,
                                    IsBulkLoaded = false
                                });

                                if (!processedDataSourceVsFeed.ContainsKey(dataSourceName))
                                    processedDataSourceVsFeed.Add(dataSourceName, new HashSet<string>());

                                processedDataSourceVsFeed[dataSourceName].Add(feedName);

                                //row[RMCommonConstants.MIGRATION_COL_STATUS] = RMCommonConstants.MIGRATION_SUCCESS;
                                row[RMDataSourceConstants.FEED_ID] = feedSummaryID;
                            }
                        }
                        else
                        {
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.FEED_ALREADY_EXISTS }, false);
                            FailFeedDependencies(deltaSet, feedName, dataSourceName);
                        }

                        new RMCommonController().CommitTransaction(mDBCon);
                    }
                    catch (Exception ex)
                    {
                        if (mDBCon != null)
                            mDBCon.RollbackTransaction();

                        new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                        FailFeedDependencies(deltaSet, feedName, dataSourceName);
                    }
                    finally
                    {
                        if (mDBCon != null)
                        {
                            RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                            mDBCon = null;
                        }
                    }
                    #endregion

                });


        }

        private void SyncFeedSummary(ObjectSet deltaSet, ObjectTable existingDataSources, ObjectTable existingFeedFields, ObjectTable sourceTable, string userName, DateTime syncDateTime)
        {
            string dataSourceName = string.Empty;
            string feedName = string.Empty;
            string dataSourceDescription = string.Empty;
            string feedType = string.Empty;
            string fileType = string.Empty;
            string errorMessage = string.Empty;
            int feedTypeID = -1;
            int fileTypeID = -1;
            int dataSourceID = -1;
            int feedSummaryID = -1;
            RDBConnectionManager mDBCon = null;
            Dictionary<string, int> feedTypes = new Dictionary<string, int>();
            Dictionary<string, int> fileTypes = new Dictionary<string, int>();
            Dictionary<string, List<string>> feedVsFileTypes = new Dictionary<string, List<string>>();
            List<ObjectRow> feedFieldRows = null;

            new RMDataSourceControllerNew().GetAllFeedAndFileDetailsForMigration(feedTypes, fileTypes, feedVsFileTypes);

            if (!existingFeedFields.Columns.Contains(RMCommonConstants.MIGRATION_COL_STATUS))
                existingFeedFields.Columns.Add(RMCommonConstants.MIGRATION_COL_STATUS, typeof(string));

            if (!existingFeedFields.Columns.Contains(RMCommonConstants.MIGRATION_COL_REMARKS))
                existingFeedFields.Columns.Add(RMCommonConstants.MIGRATION_COL_REMARKS, typeof(string));

            sourceTable.AsEnumerable()
               .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS]))
               && !string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Type])))
               .ToList().ForEach(row =>
               {
                   mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                   try
                   {
                       List<string> errors = new List<string>();
                       feedFieldRows = null;
                       feedName = Convert.ToString(row[RMDataSourceConstants.Feed_Name]);
                       feedSummaryID = Convert.ToInt32(row[RMDataSourceConstants.FEED_ID]);
                       dataSourceID = Convert.ToInt32(row[RMDataSourceConstants.Data_Source_ID]);
                       dataSourceName = Convert.ToString(row[RMDataSourceConstants.Data_Source_Name]);
                       feedType = Convert.ToString(row[RMDataSourceConstants.Feed_Type]);
                       fileType = Convert.ToString(row[RMDataSourceConstants.Feed_File_Type]);
                       feedTypeID = feedTypes[RMCommonStatic.ConvertToLower(feedType)];
                       fileTypeID = fileTypes[RMCommonStatic.ConvertToLower(fileType)];
                       dataSourceID = dictDataSources[dataSourceName];

                       if (CheckDataSourceAndFeedExists(dataSourceName, feedName)) //Data Source and Feed should exist
                       {
                           if (!IsFeedTypeChanged(feedName, feedType, objExistingFeeds.Tables[RMDataSourceConstants.TABLE_DATA_SOURCES])) //Feed Type cannot be changed
                           {
                               if (IsFeedAndFileTypeValid(feedType, fileType, feedVsFileTypes)) //Feed type and corresponding file type should be valid.
                               {
                                   errors = CheckFileProperties(fileTypeID, row); //File properties should be valid
                                   if (errors == null || errors.Count <= 0)
                                   {
                                       feedFieldRows = GetFeedFieldsForSync(deltaSet, feedName, dataSourceName); //Feed fields should be present

                                       List<ObjectRow> existingFeedFieldRows = MergeExistingFeedFields(feedFieldRows, existingFeedFields, feedName, dataSourceName); //merging feed fields
                                       if (feedFieldRows != null && feedFieldRows.Count() > 0)
                                       {
                                           if (!ValidateFeedFields(feedFieldRows, existingFeedFieldRows, fileTypeID))  //validating feed fields
                                           {
                                               new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.FEED_FIELD_NOT_PROCESSED }, true);
                                               FailFeedDependencies(deltaSet, feedName, dataSourceName);
                                           }
                                           else //All passed
                                           {
                                               SaveFeedSummary(row, feedFieldRows, feedTypeID, fileTypeID, dataSourceID, feedSummaryID, feedName, userName, syncDateTime, mDBCon);
                                           }
                                       }
                                       else
                                       {
                                           new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.FEED_FIELDS_NOT_PRESENT }, false);
                                           FailFeedDependencies(deltaSet, feedName, dataSourceName);
                                       }

                                   }
                                   else
                                   {
                                       new RMCommonMigration().SetFailedRow(row, errors, false);
                                       FailFeedDependencies(deltaSet, feedName, dataSourceName);
                                   }
                               }
                               else
                               {
                                   new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.FEED_TYPE_CHANGED }, false);
                                   FailFeedDependencies(deltaSet, feedName, dataSourceName);

                               }

                           }
                           else
                           {
                               new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.FEED_TYPE_CHANGED }, false);
                               FailFeedDependencies(deltaSet, feedName, dataSourceName);
                           }
                       }
                       else
                       {
                           new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.DS_FEED_NOT_PROCESSED }, false);
                           FailFeedDependencies(deltaSet, feedName, dataSourceName);
                       }

                       new RMCommonController().CommitTransaction(mDBCon);
                   }
                   catch (Exception ex)
                   {
                       if (mDBCon != null)
                           mDBCon.RollbackTransaction();

                       new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                       FailFeedDependencies(deltaSet, feedName, dataSourceName);
                   }
                   finally
                   {
                       if (mDBCon != null)
                       {
                           RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                           mDBCon = null;
                       }
                   }
               });
        }

        private void SyncFeedFields(IEnumerable<ObjectRow> rowsToProcess, ObjectSet deltaSet, ObjectTable existingFeedFields, ObjectTable existingDataSources, string userName, DateTime syncDateTime)
        {
            RDBConnectionManager mDBCon = null;

            var groups = rowsToProcess.GroupBy(r => new { feed = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Name]), ds = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Data_Source_Name]) });

            if (!existingFeedFields.Columns.Contains(RMCommonConstants.MIGRATION_COL_STATUS))
                existingFeedFields.Columns.Add(RMCommonConstants.MIGRATION_COL_STATUS, typeof(string));

            if (!existingDataSources.Columns.Contains(RMCommonConstants.MIGRATION_COL_STATUS))
                existingDataSources.Columns.Add(RMCommonConstants.MIGRATION_COL_STATUS, typeof(string));

            if (!existingFeedFields.Columns.Contains(RMCommonConstants.MIGRATION_COL_REMARKS))
                existingFeedFields.Columns.Add(RMCommonConstants.MIGRATION_COL_REMARKS, typeof(string));

            if (!existingDataSources.Columns.Contains(RMCommonConstants.MIGRATION_COL_REMARKS))
                existingDataSources.Columns.Add(RMCommonConstants.MIGRATION_COL_REMARKS, typeof(string));

            foreach (var group in groups)
            {
                List<ObjectRow> feedFieldRows = group.ToList();
                if (feedFieldRows.Count > 0)
                {
                    mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                    string feedName = Convert.ToString(feedFieldRows[0][RMDataSourceConstants.Feed_Name]);
                    string dataSourceName = Convert.ToString(feedFieldRows[0][RMDataSourceConstants.Data_Source_Name]);
                    try
                    {
                        List<ObjectRow> existingFeedFieldRows = MergeExistingFeedFields(feedFieldRows, existingFeedFields, feedName, dataSourceName); //merging feed fields

                        ObjectRow feedRow = existingDataSources.AsEnumerable().Where(r => RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Name]).SRMEqualWithIgnoreCase(feedName) && RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Data_Source_Name]).SRMEqualWithIgnoreCase(dataSourceName)).FirstOrDefault();

                        if (feedRow != null)
                        {
                            int feedSummaryID = Convert.ToInt32(feedRow[RMDataSourceConstants.FEED_ID]);
                            int dataSourceID = Convert.ToInt32(feedRow[RMDataSourceConstants.Data_Source_ID]);
                            string feedType = Convert.ToString(feedRow[RMDataSourceConstants.Feed_Type]);
                            string fileType = Convert.ToString(feedRow[RMDataSourceConstants.Feed_File_Type]);
                            Dictionary<string, int> feedTypes = new Dictionary<string, int>();
                            Dictionary<string, int> fileTypes = new Dictionary<string, int>();
                            Dictionary<string, List<string>> feedVsFileTypes = new Dictionary<string, List<string>>();

                            new RMDataSourceControllerNew().GetAllFeedAndFileDetailsForMigration(feedTypes, fileTypes, feedVsFileTypes);

                            int feedTypeID = feedTypes[RMCommonStatic.ConvertToLower(feedType)];
                            int fileTypeID = fileTypes[RMCommonStatic.ConvertToLower(fileType)];

                            if (!ValidateFeedFields(feedFieldRows, existingFeedFieldRows, fileTypeID))  //validating feed fields
                            {
                                new RMCommonMigration().SetFailedRow(feedRow, new List<string>() { RMCommonConstants.FEED_FIELD_NOT_PROCESSED }, true);
                                FailFeedDependencies(deltaSet, feedName, dataSourceName);
                            }
                            else //All passed
                            {
                                SaveFeedSummary(feedRow, feedFieldRows, feedTypeID, fileTypeID, dataSourceID, feedSummaryID, feedName, userName, syncDateTime, mDBCon);
                            }
                        }
                        else
                        {
                            FailDataSourceGroup(feedFieldRows, new List<string>() { RMCommonConstants.DS_FEED_NOT_PRESENT }, false);
                        }

                        new RMCommonController().CommitTransaction(mDBCon);
                    }
                    catch (Exception ex)
                    {
                        if (mDBCon != null)
                            mDBCon.RollbackTransaction();

                        FailDataSourceGroup(feedFieldRows, new List<string>() { ex.Message }, false);
                        FailFeedDependencies(deltaSet, feedName, dataSourceName);
                    }
                    finally
                    {
                        if (mDBCon != null)
                        {
                            RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                            mDBCon = null;
                        }
                    }
                }
            }


        }

        private void SyncFeedMapping(IEnumerable<ObjectRow> rowsToProcess, ObjectSet deltaSet, ObjectTable existingFeedFields, ObjectTable existingDataSources, string userName, DateTime syncDateTime)
        {
            RDBConnectionManager mDBCon = null;
            RMFeedMappingInfo feedMappingInfo = null;
            List<string> errors = new List<string>();
            Dictionary<int, string> dictRadMappings = new RMDataSourceControllerNew().GetAllRadMappings(null);

            rowsToProcess.ToList().ForEach(row =>
            {
                mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                try
                {
                    feedMappingInfo = null;
                    int radMapID = -1;
                    string mapName = Convert.ToString(row[RMDataSourceConstants.Map_Name]);
                    errors = new List<string>();
                    errors = ValidateFeedMapping(row);

                    if (dictRadMappings.Any(d => d.Value.SRMEqualWithIgnoreCase(mapName)))
                    {
                        radMapID = dictRadMappings.Where(d => d.Value.SRMEqualWithIgnoreCase(mapName)).Select(d => d.Key).FirstOrDefault();
                    }
                    else
                    {
                        errors.Add(RMCommonConstants.RAD_MAP_NAME_NOT_PRESENT);
                    }

                    if (errors.Count <= 0) //Save feed mapping
                    {
                        int feedSummaryID = Convert.ToInt32(row[RMDataSourceConstants.FEED_ID]);
                        string fieldName = Convert.ToString(row[RMDataSourceConstants.Primary_Column_Name]);
                        int feedMappingDetailsID = Convert.ToInt32(row[RMDataSourceConstants.Feed_Mapping_Details_ID]);
                        int fileID = Convert.ToInt32(row[RMDataSourceConstants.FILE_ID]);
                        int fieldID = -1;
                        RMFeedInfoDetailed feedInfoDetailed = new RMDataSourceControllerNew().GetDetailedFeedInfoByID(feedSummaryID, mDBCon);

                        if (feedInfoDetailed != null && feedInfoDetailed.FeedFields.Any(f => f.FieldName.SRMEqualWithIgnoreCase(fieldName)))
                        {
                            fieldID = feedInfoDetailed.FeedFields.Where(f => f.FieldName.SRMEqualWithIgnoreCase(fieldName))
                            .Select(ff => ff.FieldID).FirstOrDefault();

                            feedMappingInfo = new RMFeedMappingInfo();
                            feedMappingInfo.CreatedBy = userName;
                            feedMappingInfo.CreatedOn = syncDateTime;
                            feedMappingInfo.FeedMappingDetailId = feedMappingDetailsID;
                            feedMappingInfo.FeedSummaryId = feedSummaryID;
                            feedMappingInfo.IsActive = true;
                            feedMappingInfo.LastModifiedBy = userName;
                            feedMappingInfo.LastModifiedOn = syncDateTime;
                            feedMappingInfo.MapId = radMapID;
                            feedMappingInfo.MappedColumnId = Convert.ToInt32(row[RMDataSourceConstants.Mapped_Column_ID]);
                            feedMappingInfo.MappedColumnName = Convert.ToString(row[RMDataSourceConstants.Mapped_Column_Name]) + "_Mapped";
                            feedMappingInfo.MappingName = mapName;
                            feedMappingInfo.MapState = GetBooleanValue(Convert.ToString(row[RMDataSourceConstants.Map_State]));
                            feedMappingInfo.PrimaryColumnId = fieldID;
                            feedMappingInfo.PrimaryColumnName = fieldName;

                            new RMDataSourceDBManager(mDBCon).AddUpdateFeedMappingDetails(new List<RMFeedMappingInfo>() { feedMappingInfo }, fileID, feedSummaryID);

                        }

                        new RMCommonMigration().SetPassedRow(row, string.Empty);
                    }
                    else
                    {
                        new RMCommonMigration().SetFailedRow(row, errors, false);
                    }

                    new RMCommonController().CommitTransaction(mDBCon);
                }
                catch (Exception ex)
                {
                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();

                    new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }


            });
        }

        private void SyncFeedRules(IEnumerable<ObjectRow> rowsToProcess, string userName, DateTime syncDateTime, Dictionary<string, RMFeedInfoDetailed> feedDetailsInfo)
        {
            RDBConnectionManager mDBCon = null;

            var groups = rowsToProcess.GroupBy(r => new { feed = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Name]), ds = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Data_Source_Name]), type = RMCommonStatic.ConvertToLower(r[RMModelerConstants.Rule_Type]) });

            foreach (var group in groups)
            {
                List<ObjectRow> feedRules = group.ToList();
                if (feedRules.Count > 0)
                {
                    mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                    string feedName = Convert.ToString(feedRules[0][RMDataSourceConstants.Feed_Name]);
                    string dataSourceName = Convert.ToString(feedRules[0][RMDataSourceConstants.Data_Source_Name]);
                    string ruleType = Convert.ToString(feedRules[0][RMModelerConstants.Rule_Type]);

                    int ruleTypeID = 4;
                    if (ruleType.SRMEqualWithIgnoreCase(RMDataSourceConstants.Feed_Rule_Validation))
                        ruleTypeID = 1;

                    int feedSummaryID = feedDetailsInfo[feedName].FeedID;

                    try
                    {
                        if (ValidateFeedRules(feedRules))
                        {
                            //Delete existing rules of the same type
                            List<int> existingRuleIds = new RMDataSourceControllerNew().GetFeedRuleIDsByRuleType(feedSummaryID, ruleTypeID, mDBCon);
                            if (existingRuleIds != null)
                                new RMCommonController().RMDeleteRulesByRuleID(existingRuleIds, mDBCon);

                            int ruleSetID = 0;
                            //Save rules
                            feedRules.ForEach(row =>
                            {
                                RADXRuleGrammarInfo ruleGrammarInfo = null;
                                string ruleName = Convert.ToString(row[RMModelerConstants.Rule_Name]);
                                string ruleText = Convert.ToString(row[RMModelerConstants.Rule_Text]);
                                int rulePriority = Convert.ToInt32(row[RMModelerConstants.Priority]);
                                bool ruleState = Convert.ToBoolean(row[RMModelerConstants.Rule_State]);

                                ruleSetID = new RMCommonController().RMSaveRule(feedSummaryID, feedSummaryID, ruleTypeID, ruleName, rulePriority, ruleText, ruleState, 0, ruleSetID, userName, ConnectionConstants.RefMasterVendor_Connection, ref ruleGrammarInfo, mDBCon);
                            });
                        }

                        feedRules.ForEach(f =>
                        {
                            new RMCommonMigration().SetPassedRow(f, string.Empty);
                        });

                        new RMCommonController().CommitTransaction(mDBCon);
                    }
                    catch (Exception ex)
                    {
                        if (mDBCon != null)
                            mDBCon.RollbackTransaction();

                        feedRules.ForEach(f =>
                        {
                            new RMCommonMigration().SetFailedRow(f, new List<string>() { ex.Message }, false);
                        });
                    }
                    finally
                    {
                        if (mDBCon != null)
                        {
                            RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                            mDBCon = null;
                        }
                    }
                }
            }
        }

        private List<string> SyncFeedEntityTypeMapping(ObjectSet deltaSet, IEnumerable<ObjectRow> rowsToProcess, Dictionary<string, RMEntityDetailsInfo> moduleEntityTypeDetails, Dictionary<string, RMEntityDetailsInfo> allEntityTypeDetails, string userName, DateTime syncDateTime, int moduleID, Dictionary<string, RMFeedInfoDetailed> feedDetailsInfo)
        {
            RDBConnectionManager mDBConVendor = null;
            RDBConnectionManager mDBCon = null;
            List<string> errors = new List<string>();
            Dictionary<string, SecurityTypeMasterInfo> secTypeDetails = new Dictionary<string, SecurityTypeMasterInfo>();
            Dictionary<string, AttrInfo> secTypeCommonAttributes = new Dictionary<string, AttrInfo>();

            secTypeDetails = new RMSectypeController().GetSectypeAttributes(false);
            secTypeCommonAttributes = new RMSectypeController().FetchCommonAttributes(false);

            var groups = rowsToProcess.GroupBy(r => new { feed = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Name]), ds = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Data_Source_Name]) });

            foreach (var group in groups)
            {
                List<ObjectRow> mappingRows = null;
                string feedName = string.Empty;
                string dataSourceName = string.Empty;
                int feedSummaryID = -1;
                int mainEntityTypeID = -1;
                int mappedEntityTypeID = -1;

                mappingRows = group.ToList();

                if (mappingRows != null && mappingRows.Count > 0)
                {
                    feedName = Convert.ToString(mappingRows[0][RMDataSourceConstants.Feed_Name]);
                    dataSourceName = Convert.ToString(mappingRows[0][RMDataSourceConstants.Data_Source_Name]);
                    feedSummaryID = Convert.ToInt32(mappingRows[0][RMDataSourceConstants.FEED_ID]);

                    var entityTypeGroups = mappingRows.GroupBy(r => new { feed = RMCommonStatic.ConvertToLower(r[RMModelerConstants.ENTITY_DISPLAY_NAME]) });

                    foreach (var etGroup in entityTypeGroups)
                    {
                        mDBConVendor = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                        mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                        string entityTypeName = string.Empty;
                        try
                        {
                            bool isGroupValid = true;
                            List<ObjectRow> entityTypeRows = etGroup.ToList();
                            bool isEntityTypeFailed = false;
                            mappedEntityTypeID = -1;
                            errors = new List<string>();
                            mainEntityTypeID = ValidateEntityTypeMappingDetails(entityTypeRows, moduleEntityTypeDetails, allEntityTypeDetails, secTypeDetails, secTypeCommonAttributes, feedName, feedDetailsInfo, ref isEntityTypeFailed, ref mappedEntityTypeID, ref entityTypeName);
                            errors.AddRange(ValidateFeedFieldAttributeMapping(entityTypeRows, feedName, feedDetailsInfo, ref isEntityTypeFailed));

                            if (isEntityTypeFailed)
                            {
                                isGroupValid = false;
                            }

                            if (!isGroupValid)
                            {
                                FailFeedMappingDependencies(new List<ObjectTable>() { deltaSet.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_MAPPING], deltaSet.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_RULES] }, feedName, dataSourceName, entityTypeName);
                            }
                            else
                            {
                                SaveFeedEntityTypeMapping(etGroup.ToList(), feedSummaryID, mainEntityTypeID, mappedEntityTypeID, moduleEntityTypeDetails, userName, syncDateTime, mDBConVendor, mDBCon);

                                etGroup.ToList().ForEach(row =>
                                {
                                    new RMCommonMigration().SetPassedRow(row, string.Empty);
                                });
                            }

                            new RMCommonController().CommitTransaction(mDBConVendor);
                            new RMCommonController().CommitTransaction(mDBCon);

                        }
                        catch (Exception ex)
                        {
                            if (mDBConVendor != null)
                                mDBConVendor.RollbackTransaction();

                            if (mDBCon != null)
                                mDBCon.RollbackTransaction();

                            mappingRows.ForEach(row =>
                            {
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                            });

                            FailFeedMappingDependencies(new List<ObjectTable>() { deltaSet.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_MAPPING], deltaSet.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_RULES] }, feedName, dataSourceName, entityTypeName);
                        }
                        finally
                        {
                            if (mDBConVendor != null)
                            {
                                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConVendor);
                                mDBConVendor = null;
                            }

                            if (mDBCon != null)
                            {
                                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                                mDBCon = null;
                            }
                        }
                    }

                }

            }

            return errors;
        }

        private void SyncFeedEntityTypeRules(IEnumerable<ObjectRow> rowsToProcess, string userName, DateTime syncDateTime, Dictionary<string, RMEntityDetailsInfo> entityTypeDetails, Dictionary<string, RMFeedInfoDetailed> feedDetailsInfo)
        {
            RDBConnectionManager mDBCon = null;

            var groups = rowsToProcess.GroupBy(r => new { feed = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Name]), ds = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Data_Source_Name]), et = RMCommonStatic.ConvertToLower(r[RMModelerConstants.ENTITY_DISPLAY_NAME]), type = RMCommonStatic.ConvertToLower(r[RMModelerConstants.Rule_Type]) });

            foreach (var group in groups)
            {
                List<ObjectRow> feedRules = group.ToList();
                if (feedRules.Count > 0)
                {
                    mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                    string feedName = Convert.ToString(feedRules[0][RMDataSourceConstants.Feed_Name]);
                    string dataSourceName = Convert.ToString(feedRules[0][RMDataSourceConstants.Data_Source_Name]);
                    string ruleType = Convert.ToString(feedRules[0][RMModelerConstants.Rule_Type]);
                    int ruleTypeID = 2;
                    if (ruleType.SRMEqualWithIgnoreCase(RMDataSourceConstants.Request_Filter_Rule))
                        ruleTypeID = 3;
                    else if (ruleType.SRMEqualWithIgnoreCase(RMDataSourceConstants.Feed_Rule_Entity_Transformation))
                        ruleTypeID = 5;

                    int feedSummaryID = feedDetailsInfo[feedName].FeedID;
                    int mappingID = -1;
                    int entityTypeID = -1;
                    try
                    {
                        if (ValidateFeedEntityTypeRules(feedRules, entityTypeDetails, feedSummaryID, ref mappingID, ref entityTypeID, mDBCon))
                        {
                            //Delete existing rules of the same type
                            List<int> existingRuleIds = new RMDataSourceControllerNew().GetFeedRuleIDsByRuleType(feedSummaryID, ruleTypeID, mDBCon);
                            if (existingRuleIds != null)
                                new RMCommonController().RMDeleteRulesByRuleID(existingRuleIds, mDBCon);

                            int ruleSetID = 0;
                            int dependentID = ruleTypeID != 3 ? feedSummaryID : entityTypeID;
                            //Save rules
                            feedRules.ForEach(row =>
                            {
                                RADXRuleGrammarInfo ruleGrammarInfo = null;
                                string ruleName = Convert.ToString(row[RMModelerConstants.Rule_Name]);
                                string ruleText = Convert.ToString(row[RMModelerConstants.Rule_Text]);
                                int rulePriority = Convert.ToInt32(row[RMModelerConstants.Priority]);
                                bool ruleState = Convert.ToBoolean(row[RMModelerConstants.Rule_State]);

                                ruleSetID = new RMCommonController().RMSaveRule(dependentID, mappingID, ruleTypeID, ruleName, rulePriority, ruleText, ruleState, 0, ruleSetID, userName, ConnectionConstants.RefMasterVendor_Connection, ref ruleGrammarInfo, mDBCon);
                            });
                        }

                        feedRules.ForEach(f =>
                        {
                            new RMCommonMigration().SetPassedRow(f, string.Empty);
                        });

                        new RMCommonController().CommitTransaction(mDBCon);
                    }
                    catch (Exception ex)
                    {
                        if (mDBCon != null)
                            mDBCon.RollbackTransaction();

                        feedRules.ForEach(f =>
                        {
                            new RMCommonMigration().SetFailedRow(f, new List<string>() { ex.Message }, false);
                        });
                    }
                    finally
                    {
                        if (mDBCon != null)
                        {
                            RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                            mDBCon = null;
                        }
                    }
                }
            }
        }

        private void SyncBulkLicenseSetup(ObjectSet deltaSet, IEnumerable<ObjectRow> rowsToProcess, Dictionary<string, RMEntityDetailsInfo> entityTypeDetails, Dictionary<string, RMFeedInfoDetailed> feedDetailsInfo, string userName, DateTime syncDateTime, int moduleID)
        {
            RDBConnectionManager mDBCon = null;

            IDictionary<string, string> dateTypes = new Dictionary<string, string>();
            dateTypes = new RMDataSourceControllerNew().ConfigurationLoader("RefM_RefMasterControllerConfig")["CommonDateTypes"];
            dateTypes.Remove("100");
            dateTypes.Add("-1", "");

            foreach (ObjectRow row in rowsToProcess)
            {
                mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                try
                {
                    string taskName = Convert.ToString(row[RMDataSourceConstants.Loading_Task_Name]).Trim();
                    string taskDescription = Convert.ToString(row[RMDataSourceConstants.Loading_Task_Description]).Trim();
                    string feedName = Convert.ToString(row[RMDataSourceConstants.Feed_Name]).Trim();

                    string bulkFilePath = Convert.ToString(row[RMDataSourceConstants.Bulk_File_Path]).Trim();
                    string bulkFileDateType = Convert.ToString(row[RMDataSourceConstants.Bulk_File_Date_Type]).Trim();
                    string bulkFileDate = Convert.ToString(row[RMDataSourceConstants.Bulk_File_Date]).Trim();
                    string bulkFileBusinessDays = Convert.ToString(row[RMDataSourceConstants.Bulk_File_Business_Days]).Trim();

                    string diffFilePath = Convert.ToString(row[RMDataSourceConstants.Difference_File_Path]).Trim();
                    string diffFileDateType = Convert.ToString(row[RMDataSourceConstants.Difference_File_Date_Type]).Trim();
                    string diffFileDate = Convert.ToString(row[RMDataSourceConstants.Difference_File_Date]).Trim();
                    string diffFileBusinessDays = Convert.ToString(row[RMDataSourceConstants.Difference_File_Business_Days]).Trim();

                    int fileTypeID = feedDetailsInfo[feedName].FileTypeID;
                    int feedSummaryID = feedDetailsInfo[feedName].FeedID;

                    bool isValid = SaveBulkLicenseSetup(row, feedSummaryID, feedName, fileTypeID, taskName, taskDescription, bulkFilePath, bulkFileDateType, bulkFileDate, bulkFileBusinessDays, diffFilePath, diffFileDateType, diffFileDate, diffFileBusinessDays, dateTypes, userName, syncDateTime, mDBCon);

                    if (isValid)
                        new RMCommonMigration().SetPassedRow(row, string.Empty);

                    new RMCommonController().CommitTransaction(mDBCon);
                }
                catch (Exception ex)
                {
                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();

                    new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }
            }
        }

        private void SyncCustomClasses(ObjectSet deltaSet, IEnumerable<ObjectRow> rowsToProcess, string userName, DateTime syncDateTime, int moduleID)
        {
            RDBConnectionManager mDBCon = null;

            var groups = rowsToProcess.GroupBy(r => new { feed = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Name]), ds = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Data_Source_Name]) });

            foreach (var group in groups.Where(g => g.ToList() != null && g.ToList().Count > 0))
            {
                int rowCount = 0;
                bool isGroupValid = true;
                try
                {
                    mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);

                    foreach (ObjectRow row in group.ToList())
                    {
                        rowCount++;
                        bool isValid = true;
                        int execSequence = -1;
                        int taskMasterID = -1;
                        int taskDetailsID = -1;
                        string taskName = Convert.ToString(row[RMDataSourceConstants.Loading_Task_Name]).Trim();
                        string feedName = Convert.ToString(row[RMDataSourceConstants.Feed_Name]).Trim();
                        string callType = Convert.ToString(row[RMDataSourceConstants.Call_Type]).Trim();
                        string classType = Convert.ToString(row[RMDataSourceConstants.Class_Type]).Trim();
                        string className = Convert.ToString(row[RMDataSourceConstants.Script_Or_Class_Name]).Trim();
                        string assembly = Convert.ToString(row[RMDataSourceConstants.Assembly_Path]).Trim();
                        execSequence = Convert.ToInt32(row[RMDataSourceConstants.Sequence_Number]);

                        string strTaskDetails = new RMDataSourceDBManager(mDBCon).GetFeedLoadingTaskDetails(feedName, taskName, 2);

                        if (!string.IsNullOrEmpty(strTaskDetails))
                        {
                            taskMasterID = Convert.ToInt32(strTaskDetails.Split('|')[0]);
                            taskDetailsID = Convert.ToInt32(strTaskDetails.Split('|')[1]);
                        }
                        else
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.LOADING_TASK_NOT_EXISTS }, false);
                        }

                        if (execSequence <= 0)
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_SEQUENCE_NUMBER }, false);
                        }

                        if (callType.SRMEqualWithIgnoreCase("Pre-Loading"))
                            callType = "PRE";
                        else if (callType.SRMEqualWithIgnoreCase("Post-Loading"))
                            callType = "POST";
                        else
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_CALL_TYPE }, false);
                        }

                        if (classType.SRMEqualWithIgnoreCase("Script Executable"))
                            classType = "1";
                        else if (classType.SRMEqualWithIgnoreCase("Custom Class"))
                            classType = "2";
                        else
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_CLASS_TYPE }, false);
                        }

                        if (classType == "2" && string.IsNullOrEmpty(assembly.Trim()))
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.ASSEMBLY_PATH_EMPTY }, false);
                        }

                        if (isValid && isGroupValid)
                        {
                            if (rowCount == 1)
                            {
                                new RMDataSourceDBManager(mDBCon).DeleteCustomClass(taskMasterID, taskDetailsID, mDBCon);
                            }

                            RMCustomClassInfo ccInfo = new RMCustomClassInfo() { AssemblyPath = assembly, CallType = callType, ClassType = classType, ClassName = className, CreatedBy = userName, CreatedOn = syncDateTime, ExecSequence = execSequence, IsActive = true, LastModifiedBy = userName, LastModifiedOn = syncDateTime, TaskMasterId = taskMasterID, TaskDetailsId = taskDetailsID };

                            new RMDataSourceDBManager(mDBCon).AddCustomClass(ccInfo, mDBCon);

                            new RMCommonMigration().SetPassedRow(row, string.Empty);
                        }
                        else
                            isGroupValid = false;

                    }

                    if (isGroupValid)
                    {
                        new RMCommonController().CommitTransaction(mDBCon);
                    }
                    else
                    {
                        group.ToList().Where(r => string.IsNullOrEmpty(Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList().ForEach(row =>
                        {
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.CUSTOM_CLASS_NOT_PROCESSED }, false);
                        });

                        mDBCon.RollbackTransaction();
                    }

                }
                catch (Exception ex)
                {
                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();

                    group.ToList().ForEach(row =>
                    {
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                    });
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }
            }
        }

        private void SyncAPILicenseSetup(ObjectSet deltaSet, IEnumerable<ObjectRow> rowsToProcess, Dictionary<string, RMFeedInfoDetailed> feedDetailsInfo, string userName, DateTime syncDateTime, int moduleID)
        {
            RDBConnectionManager mDBCon = null;

            foreach (ObjectRow row in rowsToProcess)
            {
                mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                try
                {
                    string nodeName = "control";
                    bool isValid = true;
                    string taskName = Convert.ToString(row[RMDataSourceConstants.Import_Task_Name]).Trim();
                    string taskDescription = Convert.ToString(row[RMDataSourceConstants.Import_Task_Description]).Trim();
                    string feedName = Convert.ToString(row[RMDataSourceConstants.Feed_Name]).Trim();
                    string vendorType = Convert.ToString(row[RMDataSourceConstants.Vendor_Type]).Trim();
                    string requestType = Convert.ToString(row[RMDataSourceConstants.Request_Type]).Trim();
                    string vendorIdentifier = Convert.ToString(row[RMDataSourceConstants.Vendor_Identifier]).Trim();
                    string marketSector = Convert.ToString(row[RMDataSourceConstants.Asset]).Trim();
                    RVendorType? rVendorType = null;
                    int feedSummaryID = feedDetailsInfo[feedName].FeedID;

                    XElement vendorXML = new XElement("api");

                    XElement controlElement = new XElement(nodeName);
                    AddAttributeToXMLNode(controlElement, taskName, taskName, "taskname", "Task Name", string.Empty);
                    vendorXML.Add(controlElement);

                    controlElement = new XElement(nodeName);
                    AddAttributeToXMLNode(controlElement, taskDescription, taskDescription, "taskdescription", "Task Description", string.Empty);
                    vendorXML.Add(controlElement);

                    if (new RMDataSourceDBManager(mDBCon).CheckTaskNameExists(feedName, taskName, 4))
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.TASK_ALREADY_EXISTS }, false);
                    }

                    if (vendorType.SRMEqualWithIgnoreCase("bloomberg"))
                    {
                        rVendorType = RVendorType.Bloomberg;
                    }
                    else if (vendorType.SRMEqualWithIgnoreCase("reuters"))
                    {
                        rVendorType = RVendorType.Reuters;
                    }

                    if (rVendorType != null)
                    {
                        controlElement = new XElement(nodeName);
                        AddAttributeToXMLNode(controlElement, vendorType, vendorType, "vendor", "Vendor Type", string.Empty);
                        vendorXML.Add(controlElement);

                        ArrayList lstRequestTypes = new VendorInterfaceAPI().GetRequestType(rVendorType.Value, RLicenseType.API);
                        ArrayList lstIdentifiers = null;
                        ArrayList lstMarketSectors = null;

                        if (rVendorType == RVendorType.Bloomberg)
                        {
                            lstIdentifiers = new VendorInterfaceAPI().GetBbgInstrumentIdType();
                            lstMarketSectors = new VendorInterfaceAPI().GetBbgMarketSector();
                        }
                        else
                        {
                            lstIdentifiers = new VendorInterfaceAPI().GetRReuterInstrumentIdType();
                            lstMarketSectors = new VendorInterfaceAPI().GetRReuterAssetTypes();
                        }

                        if (lstRequestTypes != null && lstRequestTypes.SRMContainsWithIgnoreCase(requestType))
                        {
                            controlElement = new XElement(nodeName);
                            AddAttributeToXMLNode(controlElement, requestType, requestType, "apitype", "API Type", "Request Type");
                            vendorXML.Add(controlElement);
                        }
                        else
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_API_REQUEST_TYPE }, false);
                        }


                        if (lstIdentifiers != null && lstIdentifiers.SRMContainsWithIgnoreCase(vendorIdentifier))
                        {
                            controlElement = new XElement(nodeName);
                            AddAttributeToXMLNode(controlElement, vendorIdentifier, vendorIdentifier, "vendoridentifier", "Vendor Identifier", "InstrumentIdentifierType");
                            vendorXML.Add(controlElement);
                        }
                        else
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_API_VENDOR_IDENTIFIER }, false);
                        }

                        if (lstMarketSectors != null && lstMarketSectors.SRMContainsWithIgnoreCase(marketSector))
                        {
                            controlElement = new XElement(nodeName);

                            if (rVendorType == RVendorType.Bloomberg)
                                AddAttributeToXMLNode(controlElement, marketSector, marketSector, "marketsector", "Market Sector", "MarketSector");
                            else
                                AddAttributeToXMLNode(controlElement, marketSector, marketSector, "assettype", "Asset Type", "AssetTypes");

                            vendorXML.Add(controlElement);
                        }
                        else
                        {
                            string invalidError = rVendorType == RVendorType.Bloomberg ? RMCommonConstants.INVALID_API_MARKET_SECTOR : RMCommonConstants.INVALID_API_ASSET_TYPE;
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { invalidError }, false);
                        }
                    }
                    else
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_API_VENDOR_TYPE }, false);
                    }

                    if (isValid)
                    {
                        SaveAPILicenseSetup(row, vendorXML, feedSummaryID, taskName, taskDescription, userName, syncDateTime, mDBCon);

                        new RMCommonMigration().SetPassedRow(row, string.Empty);
                    }

                    new RMCommonController().CommitTransaction(mDBCon);
                }
                catch (Exception ex)
                {
                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();

                    new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }
            }
        }

        private void SyncFTPLicenseSetup(ObjectSet deltaSet, IEnumerable<ObjectRow> rowsToProcess, Dictionary<string, RMFeedInfoDetailed> feedDetailsInfo, string userName, DateTime syncDateTime, int moduleID)
        {
            RDBConnectionManager mDBCon = null;
            List<string> lstDataRequestTypes = new List<string>() { "Current Data", "Historical Data", "Company Data" };

            foreach (ObjectRow row in rowsToProcess)
            {
                mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                try
                {
                    string nodeName = "control";
                    bool isValid = true;
                    string requestTaskName = Convert.ToString(row[RMDataSourceConstants.Request_Task_Name]).Trim();
                    string requestTaskDescription = Convert.ToString(row[RMDataSourceConstants.Request_Task_Description]).Trim();
                    string responseTaskName = Convert.ToString(row[RMDataSourceConstants.Response_Task_Name]).Trim();
                    string responseTaskDescription = Convert.ToString(row[RMDataSourceConstants.Response_Task_Description]).Trim();
                    string feedName = Convert.ToString(row[RMDataSourceConstants.Feed_Name]).Trim();
                    string requestVendorType = Convert.ToString(row[RMDataSourceConstants.Request_Vendor_Type]).Trim();
                    string requestTransportType = Convert.ToString(row[RMDataSourceConstants.Request_Transport_Type]).Trim();
                    string requestVendorIdentifier = Convert.ToString(row[RMDataSourceConstants.Request_Vendor_Identifier]).Trim();
                    string requestMarketSector = Convert.ToString(row[RMDataSourceConstants.Request_Market_Sector]).Trim();
                    string requestDataRequestType = Convert.ToString(row[RMDataSourceConstants.Request_Data_Request_Type]).Trim();
                    string requestOutgoingFTP = Convert.ToString(row[RMDataSourceConstants.Request_Outgoing_FTP]).Trim();

                    string responseVendorType = Convert.ToString(row[RMDataSourceConstants.Response_Vendor_Type]).Trim();
                    string responseTransportType = Convert.ToString(row[RMDataSourceConstants.Response_Transport_Type]).Trim();
                    //string responseDataRequestType = Convert.ToString(row[RMDataSourceConstants.Response_Data_Request_Type]).Trim();
                    string responseIncomingFTP = Convert.ToString(row[RMDataSourceConstants.Response_Incoming_FTP]).Trim();

                    RVendorType? rVendorTypeRequest = null;
                    RVendorType? rVendorTypeResponse = null;

                    int feedSummaryID = feedDetailsInfo[feedName].FeedID;

                    XElement requestXML = new XElement("request");
                    XElement responseXML = new XElement("response");

                    XElement controlElementRequest = null;
                    XElement controlElementResponse = null;

                    controlElementRequest = new XElement(nodeName);
                    AddAttributeToXMLNode(controlElementRequest, requestTaskName, requestTaskName, "tasknamerequest", "Task Name", string.Empty);
                    requestXML.Add(controlElementRequest);

                    controlElementRequest = new XElement(nodeName);
                    AddAttributeToXMLNode(controlElementRequest, requestTaskDescription, requestTaskDescription, "taskdescriptionrequest", "Task Description", string.Empty);
                    requestXML.Add(controlElementRequest);

                    controlElementResponse = new XElement(nodeName);
                    AddAttributeToXMLNode(controlElementResponse, responseTaskName, responseTaskName, "tasknameresponse", "Task Name", string.Empty);
                    responseXML.Add(controlElementResponse);

                    controlElementResponse = new XElement(nodeName);
                    AddAttributeToXMLNode(controlElementResponse, responseTaskDescription, responseTaskDescription, "taskdescriptionresponse", "Task Description", string.Empty);
                    responseXML.Add(controlElementResponse);

                    if (new RMDataSourceDBManager(mDBCon).CheckTaskNameExists(feedName, requestTaskName, 5))
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.TASK_ALREADY_EXISTS }, false);
                    }

                    if (new RMDataSourceDBManager(mDBCon).CheckTaskNameExists(feedName, responseTaskName, 6))
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.TASK_ALREADY_EXISTS }, false);
                    }

                    if (requestVendorType.SRMEqualWithIgnoreCase("bloomberg"))
                    {
                        rVendorTypeRequest = RVendorType.Bloomberg;
                    }
                    else if (requestVendorType.SRMEqualWithIgnoreCase("reuters"))
                    {
                        rVendorTypeRequest = RVendorType.Reuters;
                    }
                    if (responseVendorType.SRMEqualWithIgnoreCase("bloomberg"))
                    {
                        rVendorTypeResponse = RVendorType.Bloomberg;
                    }
                    else if (responseVendorType.SRMEqualWithIgnoreCase("reuters"))
                    {
                        rVendorTypeResponse = RVendorType.Reuters;
                    }

                    if (rVendorTypeRequest == null)
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_REQ_VENDOR_TYPE }, false);
                    }
                    if (rVendorTypeResponse == null)
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_RES_VENDOR_TYPE }, false);
                    }
                    if (rVendorTypeRequest != rVendorTypeResponse)
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.DIFFERENT_VENDOR_TYPES }, false);
                    }

                    if (isValid)
                    {
                        controlElementRequest = new XElement(nodeName);
                        AddAttributeToXMLNode(controlElementRequest, requestVendorType, requestVendorType, "vendor", "Vendor Type", string.Empty);
                        requestXML.Add(controlElementRequest);

                        controlElementResponse = new XElement(nodeName);
                        AddAttributeToXMLNode(controlElementResponse, responseVendorType, responseVendorType, "vendor", "Vendor Type", string.Empty);
                        responseXML.Add(controlElementResponse);

                        ArrayList lstTransportTypes = new VendorInterfaceAPI().GetRequestType(rVendorTypeRequest.Value, RLicenseType.FTP);
                        ArrayList lstIdentifiers = null;
                        ArrayList lstMarketSectors = null;
                        ArrayList lstFTPs = new VendorInterfaceAPI().GetAllTransportsNew();

                        if (rVendorTypeRequest == RVendorType.Bloomberg)
                        {
                            lstIdentifiers = new VendorInterfaceAPI().GetBbgInstrumentIdType();
                            lstMarketSectors = new VendorInterfaceAPI().GetBbgMarketSector();
                        }
                        else
                        {
                            lstIdentifiers = new VendorInterfaceAPI().GetRReuterInstrumentIdType();
                            lstMarketSectors = new VendorInterfaceAPI().GetRReuterAssetTypes();
                        }

                        if (lstTransportTypes != null && lstTransportTypes.SRMContainsWithIgnoreCase(requestTransportType))
                        {
                            controlElementRequest = new XElement(nodeName);
                            AddAttributeToXMLNode(controlElementRequest, requestTransportType, requestTransportType, "transporttyperequest", "Transport Type", "RequestType");
                            requestXML.Add(controlElementRequest);
                        }
                        else
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_REQ_TRANSPORT_TYPE }, false);
                        }

                        if (lstTransportTypes != null && lstTransportTypes.SRMContainsWithIgnoreCase(responseTransportType))
                        {
                            controlElementResponse = new XElement(nodeName);
                            AddAttributeToXMLNode(controlElementResponse, responseTransportType, responseTransportType, "transporttyperesponse", "Transport Type", "");
                            responseXML.Add(controlElementResponse);
                        }
                        else
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_RES_TRANSPORT_TYPE }, false);
                        }


                        if (lstIdentifiers != null && lstIdentifiers.SRMContainsWithIgnoreCase(requestVendorIdentifier))
                        {
                            controlElementRequest = new XElement(nodeName);
                            AddAttributeToXMLNode(controlElementRequest, requestVendorIdentifier, requestVendorIdentifier, "vendoridentifierrequest", "Vendor Identifier", "InstrumentIdentifierType");
                            requestXML.Add(controlElementRequest);
                        }
                        else
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_REQ_VENDOR_IDENTIFIER }, false);
                        }

                        if (lstMarketSectors != null && lstMarketSectors.SRMContainsWithIgnoreCase(requestMarketSector))
                        {
                            controlElementRequest = new XElement(nodeName);

                            if (rVendorTypeRequest == RVendorType.Bloomberg)
                                AddAttributeToXMLNode(controlElementRequest, requestMarketSector, requestMarketSector, "marketsectorrequest", "Market Sector", "MarketSector");
                            else
                                AddAttributeToXMLNode(controlElementRequest, requestMarketSector, requestMarketSector, "assettyperequest", "Asset Type", "AssetTypes");

                            requestXML.Add(controlElementRequest);
                        }
                        else
                        {
                            string invalidError = rVendorTypeRequest == RVendorType.Bloomberg ? RMCommonConstants.INVALID_REQ_MARKET_SECTOR : RMCommonConstants.INVALID_REQ_ASSET_TYPE;
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { invalidError }, false);
                        }

                        if (lstFTPs != null && lstFTPs.SRMContainsWithIgnoreCase(requestOutgoingFTP))
                        {
                            controlElementRequest = new XElement(nodeName);
                            AddAttributeToXMLNode(controlElementRequest, requestOutgoingFTP, requestOutgoingFTP, "outgoingftprequest", "Outgoing FTP", "TransportName");
                            requestXML.Add(controlElementRequest);
                        }
                        else
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_REQ_OUTGOING_FTP }, false);
                        }

                        if (lstFTPs != null && lstFTPs.SRMContainsWithIgnoreCase(responseIncomingFTP))
                        {
                            controlElementResponse = new XElement(nodeName);
                            AddAttributeToXMLNode(controlElementResponse, responseIncomingFTP, responseIncomingFTP, "incomingftpresponse", "Incoming FTP", "TransportName");
                            responseXML.Add(controlElementResponse);
                        }
                        else
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_RES_INCOMING_FTP }, false);
                        }

                        if (rVendorTypeRequest == RVendorType.Bloomberg && !string.IsNullOrEmpty(requestDataRequestType))
                        {
                            if (lstDataRequestTypes.SRMContainsWithIgnoreCase(requestDataRequestType))
                            {
                                controlElementRequest = new XElement(nodeName);

                                AddAttributeToXMLNode(controlElementRequest, lstDataRequestTypes.IndexOf(requestDataRequestType).ToString(), requestDataRequestType, "datarequesttype", "Data Request Type", "datarequesttype");
                                requestXML.Add(controlElementRequest);
                            }
                            else
                            {
                                isValid = false;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_REQ_DATA_REQUEST }, false);
                            }
                        }

                        //if (rVendorTypeResponse == RVendorType.Bloomberg && !string.IsNullOrEmpty(responseDataRequestType))
                        //{
                        //    if (lstDataRequestTypes.SRMContainsWithIgnoreCase(responseDataRequestType))
                        //    {
                        //        controlElementResponse = new XElement(nodeName);
                        //        AddAttributeToXMLNode(controlElementResponse, responseDataRequestType, responseDataRequestType, "datarequesttype", "Data Request Type", "datarequesttype");
                        //        responseXML.Add(controlElementResponse);
                        //    }
                        //    else
                        //    {
                        //        isValid = false;
                        //        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_RES_DATA_REQUEST }, false);
                        //    }
                        //}
                    }


                    if (isValid)
                    {
                        SaveFTPLicenseSetup(row, requestXML, responseXML, feedSummaryID, requestTaskName, requestTaskDescription, responseTaskName, responseTaskDescription, userName, syncDateTime, mDBCon);

                        new RMCommonMigration().SetPassedRow(row, string.Empty);
                    }

                    new RMCommonController().CommitTransaction(mDBCon);
                }
                catch (Exception ex)
                {
                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();

                    new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }
            }
        }


        private void SaveFTPLicenseSetup(ObjectRow row, XElement requestXML, XElement responseXML, int feedSummaryID, string requestTaskName, string requestTaskDescription, string responseTaskName, string responseTaskDescription, string userName, DateTime syncDateTime, RDBConnectionManager mDBCon)
        {
            try
            {
                #region Request Task
                int taskMasterID = -1;
                int taskTypeID = 5;
                bool isInsert = true;
                RMTaskInfo taskInfo = null;

                DataSet dsResult = null;

                dsResult = new RMDataSourceDBManager(mDBCon).GetTaskSummaryByDependentID(feedSummaryID, taskTypeID.ToString());

                if (dsResult != null && (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0))
                {
                    isInsert = false;
                    taskMasterID = Convert.ToInt32(dsResult.Tables[0].Rows[0]["task_master_id"]);
                }

                if (isInsert)
                {
                    new RMDataSourceDBManager(mDBCon).InsertNewLicenseDetails(new List<RMLicenseSetupInfo>() { new RMLicenseSetupInfo() { FeedSummaryId = feedSummaryID, CreatedBy = userName, CreatedOn = syncDateTime, IsActive = true, LastModifiedBy = userName, LastModifiedOn = syncDateTime, LicenseTypeId = 3, LicenseFeedMappingId = -1 } });

                    taskInfo = new RMTaskInfo() { TaskName = requestTaskName, TaskDescription = requestTaskDescription, TaskTypeId = taskTypeID, DependentId = feedSummaryID, CreatedBy = userName, LastModifiedBy = userName };

                    taskMasterID = new RMDataSourceDBManager(mDBCon).AddTaskSummaryDetails(taskInfo, mDBCon);

                    new RMDataSourceDBManager(mDBCon).CreateAPIFTPTaskDetails(taskMasterID, requestXML.ToString(), userName, feedSummaryID, 3);
                }
                else
                {
                    taskInfo = new RMTaskInfo() { TaskMasterId = taskMasterID, TaskName = requestTaskName, TaskDescription = requestTaskDescription, TaskTypeId = taskTypeID, DependentId = feedSummaryID, LastModifiedBy = userName, CreatedBy = userName };

                    new RMDataSourceDBManager(mDBCon).UpdateTaskSummaryDetails(taskInfo, mDBCon);

                    new RMDataSourceDBManager(mDBCon).UpdateAPIFTPTaskDetails(taskMasterID, requestXML.ToString(), userName);
                }

                #endregion

                #region Response Task

                taskMasterID = -1;
                taskTypeID = 6;
                isInsert = true;
                taskInfo = null;

                dsResult = null;

                dsResult = new RMDataSourceDBManager(mDBCon).GetTaskSummaryByDependentID(feedSummaryID, taskTypeID.ToString());

                if (dsResult != null && (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0))
                {
                    isInsert = false;
                    taskMasterID = Convert.ToInt32(dsResult.Tables[0].Rows[0]["task_master_id"]);
                }

                if (isInsert)
                {
                    taskInfo = new RMTaskInfo() { TaskName = responseTaskName, TaskDescription = responseTaskDescription, TaskTypeId = taskTypeID, DependentId = feedSummaryID, CreatedBy = userName, LastModifiedBy = userName };

                    taskMasterID = new RMDataSourceDBManager(mDBCon).AddTaskSummaryDetails(taskInfo, mDBCon);

                    new RMDataSourceDBManager(mDBCon).CreateAPIFTPTaskDetails(taskMasterID, responseXML.ToString(), userName, feedSummaryID, 3);
                }
                else
                {
                    taskInfo = new RMTaskInfo() { TaskMasterId = taskMasterID, TaskName = responseTaskName, TaskDescription = responseTaskDescription, TaskTypeId = taskTypeID, DependentId = feedSummaryID, LastModifiedBy = userName, CreatedBy = userName };

                    new RMDataSourceDBManager(mDBCon).UpdateTaskSummaryDetails(taskInfo, mDBCon);

                    new RMDataSourceDBManager(mDBCon).UpdateAPIFTPTaskDetails(taskMasterID, responseXML.ToString(), userName);
                }

                #endregion
            }
            catch
            {
                throw;
            }
        }

        private void SaveAPILicenseSetup(ObjectRow row, XElement vendorXML, int feedSummaryID, string taskName, string taskDescription, string userName, DateTime syncDateTime, RDBConnectionManager mDBCon)
        {
            try
            {
                int taskMasterID = -1;
                int taskTypeID = 4;
                bool isInsert = true;
                RMTaskInfo taskInfo = null;

                DataSet dsResult = new RMDataSourceDBManager(mDBCon).GetTaskSummaryByDependentID(feedSummaryID, taskTypeID.ToString());

                if (dsResult != null && (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0))
                {
                    isInsert = false;
                    taskMasterID = Convert.ToInt32(dsResult.Tables[0].Rows[0]["task_master_id"]);
                }

                if (isInsert)
                {
                    new RMDataSourceDBManager(mDBCon).InsertNewLicenseDetails(new List<RMLicenseSetupInfo>() { new RMLicenseSetupInfo() { FeedSummaryId = feedSummaryID, CreatedBy = userName, CreatedOn = syncDateTime, IsActive = true, LastModifiedBy = userName, LastModifiedOn = syncDateTime, LicenseTypeId = 2, LicenseFeedMappingId = -1 } });

                    taskInfo = new RMTaskInfo() { TaskName = taskName, TaskDescription = taskDescription, TaskTypeId = taskTypeID, DependentId = feedSummaryID, CreatedBy = userName, LastModifiedBy = userName };

                    taskMasterID = new RMDataSourceDBManager(mDBCon).AddTaskSummaryDetails(taskInfo, mDBCon);

                    new RMDataSourceDBManager(mDBCon).CreateAPIFTPTaskDetails(taskMasterID, vendorXML.ToString(), userName, feedSummaryID, 2);
                }
                else
                {
                    taskInfo = new RMTaskInfo() { TaskMasterId = taskMasterID, TaskName = taskName, TaskDescription = taskDescription, TaskTypeId = taskTypeID, DependentId = feedSummaryID, LastModifiedBy = userName, CreatedBy = userName };

                    new RMDataSourceDBManager(mDBCon).UpdateTaskSummaryDetails(taskInfo, mDBCon);

                    new RMDataSourceDBManager(mDBCon).UpdateAPIFTPTaskDetails(taskMasterID, vendorXML.ToString(), userName);
                }
            }
            catch
            {
                throw;
            }
        }

        private bool SaveBulkLicenseSetup(ObjectRow row, int feedSummaryID, string feedName, int fileTypeID, string taskName, string taskDescr, string bulkFilePath, string bulkFileDateType, string bulkFileDate, string bulkFileBusinessDays, string diffFilePath, string diffFileDateType, string diffFileDate, string diffFileBusinessDays, IDictionary<string, string> dateTypes, string userName, DateTime syncDateTime, RDBConnectionManager mDBCon)
        {
            try
            {
                bool isValid = true;
                bool isInsert = true;
                int bulkDays = 0;
                int diffDays = 0;
                int bulkDateType = -1;
                int diffDateType = -1;
                DateTime outDT;
                string strBulkFileDate = string.Empty;
                string strDiffFileDate = string.Empty;
                int taskMasterID = -1;
                int taskDetailsID = -1;

                #region Validations start
                if (new RMDataSourceDBManager(mDBCon).CheckTaskNameExists(feedName, taskName, 2))
                {
                    isValid = false;
                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.TASK_ALREADY_EXISTS }, false);
                }
                else
                {
                    string strTask = new RMDataSourceDBManager(mDBCon).GetFeedLoadingTaskDetails(feedName, taskName, 2);
                    if (!string.IsNullOrEmpty(strTask))
                    {
                        taskMasterID = Convert.ToInt32(strTask.Split('|')[0]);
                        taskDetailsID = Convert.ToInt32(strTask.Split('|')[1]);
                        isInsert = false;
                    }
                }

                if (fileTypeID != 4)
                {
                    #region Validating Bulk file properties
                    if (string.IsNullOrEmpty(bulkFilePath))
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.BULK_FILE_PATH_MISSING }, false);
                    }

                    //bulkDateType = dateTypes.Where(d => d.Value.SRMEqualWithIgnoreCase(bulkFileDateType)).Select(d => Convert.ToInt32(d.Key)).FirstOrDefault();
                    foreach (KeyValuePair<string, string> kvp in dateTypes)
                    {
                        if (kvp.Value.SRMEqualWithIgnoreCase(bulkFileDateType))
                        {
                            bulkDateType = Convert.ToInt16(kvp.Key);
                            break;
                        }
                    }

                    if (bulkDateType < 0)
                    {
                        isValid = false;
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_BULK_FILE_DATE_TYPE }, false);
                    }

                    if ((bulkDateType == 4 || (bulkDateType > 10 && bulkDateType < 15)))
                    {
                        if (!Int32.TryParse(bulkFileBusinessDays, out bulkDays))
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_BULK_FILE_BUSINESS_DAYS }, false);
                        }
                        else if (bulkDays < 0)
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_BULK_FILE_BUSINESS_DAYS }, false);
                        }
                    }

                    if (bulkDateType == 5)
                    {
                        if (!DateTime.TryParseExact(Convert.ToString(row[RMDataSourceConstants.Bulk_File_Date]), "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out outDT))
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_BULK_FILE_DATE }, false);
                        }
                        else
                            strBulkFileDate = outDT.ToString("yyyyMMdd");
                    }

                    #endregion

                    #region Validating diff file properties
                    if (!string.IsNullOrEmpty(diffFileDateType))
                    {
                        //diffDateType = dateTypes.Where(d => d.Value.SRMEqualWithIgnoreCase(diffFileDateType)).Select(d => Convert.ToInt32(d.Key)).FirstOrDefault();

                        foreach (KeyValuePair<string, string> kvp in dateTypes)
                        {
                            if (kvp.Value.SRMEqualWithIgnoreCase(diffFileDateType))
                            {
                                diffDateType = Convert.ToInt16(kvp.Key);
                                break;
                            }
                        }

                        if (diffDateType < 0)
                        {
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_DIFF_FILE_DATE_TYPE }, false);
                        }

                        if ((diffDateType == 4 || (diffDateType > 10 && diffDateType < 15)))
                        {
                            if (!Int32.TryParse(diffFileBusinessDays, out diffDays))
                            {
                                isValid = false;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_DIFF_FILE_BUSINESS_DAYS }, false);
                            }
                            else if (diffDays < 0)
                            {
                                isValid = false;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_DIFF_FILE_BUSINESS_DAYS }, false);
                            }
                        }

                        if (diffDateType == 5)
                        {
                            if (!DateTime.TryParseExact(Convert.ToString(row[RMDataSourceConstants.Difference_File_Date]), "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out outDT))
                            {
                                isValid = false;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_DIFF_FILE_DATE }, false);
                            }
                            else
                                strDiffFileDate = outDT.ToString("yyyyMMdd");
                        }
                    }
                    #endregion
                }

                #endregion

                if (isValid)
                {
                    RMLoadTaskInfo objLoadTaskInfo = new RMLoadTaskInfo();
                    objLoadTaskInfo.BulkFileDate = strBulkFileDate;
                    objLoadTaskInfo.BulkFileDateDays = bulkDays;
                    objLoadTaskInfo.BulkFileDateType = bulkDateType;
                    objLoadTaskInfo.BulkFilePath = bulkFilePath;
                    objLoadTaskInfo.CreatedBy = userName;
                    objLoadTaskInfo.CustomCallExist = false;
                    objLoadTaskInfo.DifferenceFilePath = diffFilePath;
                    objLoadTaskInfo.DiffFileDate = strDiffFileDate;
                    objLoadTaskInfo.DiffFileDateDays = diffDays;
                    objLoadTaskInfo.DiffFileDateType = diffDateType;
                    objLoadTaskInfo.FeedLoadingDetailsID = taskDetailsID;
                    objLoadTaskInfo.FeedSummaryID = feedSummaryID;
                    objLoadTaskInfo.IsActive = true;
                    objLoadTaskInfo.IsBulk = false;
                    objLoadTaskInfo.LastInstanceFileDate = DateTime.MinValue;
                    objLoadTaskInfo.LastInstanceFileName = string.Empty;
                    objLoadTaskInfo.LastModifiedBy = userName;
                    objLoadTaskInfo.LastModifiedOn = syncDateTime;
                    objLoadTaskInfo.LastRunDate = DateTime.MinValue;
                    objLoadTaskInfo.TaskMasterID = taskMasterID;


                    RMTaskInfo objTaskInfo = new RMTaskInfo();
                    objTaskInfo.CreatedBy = userName;
                    objTaskInfo.LastModifiedBy = userName;
                    objTaskInfo.TaskName = taskName;
                    objTaskInfo.TaskDescription = taskDescr;
                    objTaskInfo.TaskTypeId = 2;

                    if (isInsert)
                    {
                        new RMDataSourceDBManager(mDBCon).InsertNewLicenseDetails(new List<RMLicenseSetupInfo>() { new RMLicenseSetupInfo() { FeedSummaryId = feedSummaryID, CreatedBy = userName, CreatedOn = syncDateTime, IsActive = true, LastModifiedBy = userName, LastModifiedOn = syncDateTime, LicenseTypeId = 1, LicenseFeedMappingId = -1 } });
                    }

                    new RMDataSourceDBManager(mDBCon).AddUpdateLoadingTask(objLoadTaskInfo, null, objTaskInfo, taskMasterID, taskDetailsID);
                }

                return isValid;
            }
            catch
            {
                throw;
            }
        }

        private void SaveFeedEntityTypeMapping(List<ObjectRow> entityTypeRows, int feedSummaryID, int mainEntityTypeID, int mappedEntityTypeID, Dictionary<string, RMEntityDetailsInfo> entityTypeDetails, string userName, DateTime syncDateTime, RDBConnectionManager mDBConVendor, RDBConnectionManager mDBCon)
        {
            Dictionary<int, int> mappingIds = new Dictionary<int, int>();
            int entityTypeFeedMappingID = -1;
            List<int> entityTypeIds = new List<int>();
            string entityTypeName = Convert.ToString(entityTypeRows[0][RMModelerConstants.ENTITY_DISPLAY_NAME]);

            RMEntityDetailsInfo entityTypeInfo = entityTypeDetails.Where(e => e.Key.SRMEqualWithIgnoreCase(entityTypeName)).Select(e => e.Value).FirstOrDefault();

            if (entityTypeInfo != null)
            {
                entityTypeIds.Add(entityTypeInfo.EntityTypeID);

                if (entityTypeInfo.Legs != null)
                {
                    foreach (KeyValuePair<string, RMEntityDetailsInfo> kvp in entityTypeInfo.Legs)
                        entityTypeIds.Add(kvp.Value.EntityTypeID);
                }

                if (entityTypeIds.Count > 0)
                {
                    if (entityTypeIds.Count > 0)
                    {
                        //Deleting existing mapping for entity type (plus dependants)
                        new RMDataSourceDBManager(null).DeleteFeedEntityTypeMapping(feedSummaryID, entityTypeIds, mDBCon);

                        //Adding Feed Entity Type Mapping
                        RMEntityTypeFeedMapping feedMapping = new RMEntityTypeFeedMapping();
                        feedMapping.FeedSummaryID = feedSummaryID;
                        feedMapping.EntityTypeID = entityTypeInfo.EntityTypeID;
                        feedMapping.ReplaceExisting = false;
                        feedMapping.IsMasterUpdateOnly = false;
                        feedMapping.CreatedBy = userName;
                        feedMapping.LastModifiedBy = userName;

                        mappingIds = new RMDataSourceDBManager(mDBCon).AddEntityTypeFeedMapping(feedMapping);

                        if (mappingIds != null && mappingIds.Keys.Count > 0)
                        {
                            SaveEntityAttributeFeedFieldMapping(entityTypeRows, entityTypeInfo, mappingIds, feedSummaryID, entityTypeInfo.EntityTypeID, entityTypeIds, userName, syncDateTime, mDBConVendor, mDBCon);
                        }
                    }
                }

            }

            else
            {
                mLogger.Debug("SaveFeedEntityTypeMapping: Invalid Entity Type - " + entityTypeName);
            }
        }

        private void SaveEntityAttributeFeedFieldMapping(List<ObjectRow> entityTypeRows, RMEntityDetailsInfo entityTypeInfo, Dictionary<int, int> mappingIds, int feedSummaryID, int mainEntityTypeID, List<int> entityTypeIds, string userName, DateTime syncDateTime, RDBConnectionManager mDBConVendor, RDBConnectionManager mDBCon)
        {
            DataSet dsTableSchema = CommonDALWrapper.GetTableSchema("ivp_refm_entity_type_feed_mapping_details", ConnectionConstants.RefMaster_Connection);
            DataTable dtEFMappedDetails = dsTableSchema.Tables[0];
            List<RMEntityTypeFeedMappingDetailsInfo> lstFeedMappingDetails = new List<RMEntityTypeFeedMappingDetailsInfo>();
            RMEntityTypeFeedMappingDetailsInfo feedMappingDetail = null;
            List<RMEntityFeedAttributeLookup> lstRefLookups = new List<RMEntityFeedAttributeLookup>();
            RMEntityFeedAttributeLookup refLookup = null;
            List<RMEntityFeedAttributeLookup> lstSecLookups = new List<RMEntityFeedAttributeLookup>();
            RMEntityFeedAttributeLookup secLookup = null;
            List<RMEntityTypeFeedMapping> lstFeedMappings = new List<RMEntityTypeFeedMapping>();
            RMEntityTypeFeedMapping feedMapping = null;
            bool isInsert = true;
            bool isUpdate = true;
            int rowCount = 0;
            Dictionary<int, int> attrIdVsEntityTypeId = new Dictionary<int, int>();

            foreach (KeyValuePair<string, RMEntityAttributeInfo> kvp in entityTypeInfo.Attributes)
            {
                attrIdVsEntityTypeId.Add(kvp.Value.EntityAttributeID, entityTypeInfo.EntityTypeID);
            }

            if (entityTypeInfo.Legs != null)
            {
                foreach (KeyValuePair<string, RMEntityDetailsInfo> leg in entityTypeInfo.Legs)
                {
                    foreach (KeyValuePair<string, RMEntityAttributeInfo> kvp in leg.Value.Attributes)
                    {
                        attrIdVsEntityTypeId.Add(kvp.Value.EntityAttributeID, leg.Value.EntityTypeID);
                    }
                }
            }

            entityTypeRows.ForEach(row =>
            {
                rowCount++;
                int entityTypeFeedMappingID = -1;
                int entityTypeID = mainEntityTypeID;
                int entityAttributeID = Convert.ToInt32(row[RMModelerConstants.ENTITY_ATTRIBUTE_ID]);

                if (attrIdVsEntityTypeId.ContainsKey(entityAttributeID))
                {
                    entityTypeID = attrIdVsEntityTypeId[entityAttributeID];
                    if (mappingIds.ContainsKey(entityTypeID))
                        entityTypeFeedMappingID = mappingIds[entityTypeID];
                }

                int lookup_type_id = Convert.ToInt32(row[RMDataSourceConstants.Lookup_Type_Ref_Sec]);
                feedMappingDetail = new RMEntityTypeFeedMappingDetailsInfo();
                feedMappingDetail.CreatedBy = userName;
                feedMappingDetail.CreatedOn = syncDateTime;
                feedMappingDetail.DataFormat = Convert.ToString(row[RMDataSourceConstants.Date_Format]);
                feedMappingDetail.IsActive = true;
                feedMappingDetail.LastModifiedBy = userName;
                feedMappingDetail.LastModifiedOn = syncDateTime;
                feedMappingDetail.UpdateBlank = Convert.ToString(row[RMDataSourceConstants.Update_Blank]).ToLower();
                feedMappingDetail.EntityAttributeId = entityAttributeID;
                feedMappingDetail.EntityTypeFeedMappingDetailsId = -1;
                feedMappingDetail.EntityTypeFeedMappingId = entityTypeFeedMappingID;
                feedMappingDetail.FieldId = Convert.ToInt32(row[RMDataSourceConstants.Feed_Field_ID]);
                ConvertToDataTableFromInfo(feedMappingDetail, dtEFMappedDetails);

                if (rowCount == 1)
                {
                    isInsert = GetBooleanValue(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Is_Insert]));
                    isUpdate = GetBooleanValue(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Is_Update]));
                }

                if (lookup_type_id == 0) //ref lookup
                {
                    refLookup = new RMEntityFeedAttributeLookup();
                    refLookup.AttributeLookupId = Convert.ToInt32(row[RMModelerConstants.Attribute_lookup_identity]);
                    refLookup.CreatedBy = userName;
                    refLookup.EntityFeedAttributeLookupId = -1;
                    refLookup.FeedSummaryId = feedSummaryID;
                    refLookup.IsActive = true;
                    refLookup.LastModifiedBy = userName;
                    refLookup.ParentEntityAttributeName = Convert.ToString(row[RMModelerConstants.Lookup_Attribute_Real_Name]);
                    lstRefLookups.Add(refLookup);
                }

                else if (lookup_type_id == 1) //sec lookup
                {
                    secLookup = new RMEntityFeedAttributeLookup();
                    secLookup.AttributeLookupId = Convert.ToInt32(row[RMModelerConstants.Attribute_lookup_identity]);
                    secLookup.CreatedBy = userName;
                    secLookup.EntityFeedAttributeLookupId = -1;
                    secLookup.FeedSummaryId = feedSummaryID;
                    secLookup.IsActive = true;
                    secLookup.LastModifiedBy = userName;
                    secLookup.ParentEntityAttributeName = Convert.ToString(row[RMModelerConstants.Lookup_Attribute_Real_Name]);
                    lstSecLookups.Add(secLookup);
                }
            });


            entityTypeIds.ForEach(eid =>
            {
                feedMapping = new RMEntityTypeFeedMapping();
                feedMapping.FeedSummaryID = feedSummaryID;
                feedMapping.EntityTypeID = eid;
                feedMapping.IsMasterUpdateOnly = isUpdate;
                feedMapping.ReplaceExisting = isInsert;
                feedMapping.LastModifiedBy = userName;
                lstFeedMappings.Add(feedMapping);
            });

            new RMDataSourceDBManager(mDBCon).SaveAllAttributesMappedForEntity(dtEFMappedDetails, lstFeedMappings, entityTypeIds, feedSummaryID, mainEntityTypeID, lstRefLookups.ToArray(), lstSecLookups.ToArray());
        }

        private bool SetFeedMappingDetailsInfo(ObjectRow row, ObjectRow dbRow)
        {
            row[RMDataSourceConstants.Feed_Mapping_Details_ID] = dbRow[RMDataSourceConstants.Feed_Mapping_Details_ID];
            row[RMDataSourceConstants.FEED_ID] = dbRow[RMDataSourceConstants.FEED_ID];
            row[RMDataSourceConstants.FILE_ID] = dbRow[RMDataSourceConstants.FILE_ID];
            row[RMDataSourceConstants.Mapped_Column_ID] = dbRow[RMDataSourceConstants.Mapped_Column_ID];
            return true;
        }

        private bool SetFeedSummaryID(ObjectRow row, ObjectRow dbRow)
        {
            row[RMDataSourceConstants.FEED_ID] = dbRow[RMDataSourceConstants.FEED_ID];
            return true;
        }

        private void SaveFeedSummary(ObjectRow feedDetailsRow, List<ObjectRow> feedFieldRows, int feedTypeID, int fileTypeID, int dataSourceID, int feedSummaryID, string feedName, string userName, DateTime syncDateTime, RDBConnectionManager mDBCon)
        {
            try
            {
                RADFilePropertyInfo radFileProperty = new RADFilePropertyInfo();
                RMFeedSummaryInfo feedSummaryInfo = new RMFeedSummaryInfo();
                List<RADFileFieldDetailsInfo> lstRADFileFieldDetails = new List<RADFileFieldDetailsInfo>();
                RADFileFieldDetailsInfo radFFD = null;
                List<RMFeedFieldDetailsInfo> lstRMFeedFieldDetails = new List<RMFeedFieldDetailsInfo>();
                RMFeedFieldDetailsInfo rmFFD = null;
                RMFeedFieldInfo ffdi = null;

                RMFeedInfoDetailed feedInfoDetailed = new RMDataSourceControllerNew().GetDetailedFeedInfoByID(feedSummaryID, mDBCon);
                bool isFeedPresent = feedInfoDetailed != null;

                //Feed Summary
                feedSummaryInfo.ColumnQuery = fileTypeID == 4 ? Convert.ToString(feedDetailsRow[RMDataSourceConstants.Loading_Criteria]) : string.Empty;
                feedSummaryInfo.ConnectionString = fileTypeID == 4 ? Convert.ToString(feedDetailsRow[RMDataSourceConstants.Connection_String]) : string.Empty;
                feedSummaryInfo.CreatedBy = userName;
                feedSummaryInfo.CreatedOn = syncDateTime;
                feedSummaryInfo.DataSourceID = dataSourceID;
                feedSummaryInfo.DBProvider = fileTypeID == 4 ? Convert.ToString(feedDetailsRow[RMDataSourceConstants.Server_Type]) : string.Empty;
                feedSummaryInfo.FeedName = feedName;
                feedSummaryInfo.FeedSummaryID = feedSummaryID;
                feedSummaryInfo.FeedTypeID = feedTypeID;
                feedSummaryInfo.IsActive = true;
                feedSummaryInfo.IsBulkLoaded = true;
                feedSummaryInfo.IsComplete = true;
                feedSummaryInfo.LastModifiedBy = userName;
                feedSummaryInfo.LastModifiedOn = syncDateTime;
                feedSummaryInfo.RadFileID = isFeedPresent ? feedInfoDetailed.FileID : -1;

                //Rad File Properties
                radFileProperty.CommentChar = Convert.ToString(feedDetailsRow[RMDataSourceConstants.Comment_Char]);
                radFileProperty.CreatedBy = userName;
                radFileProperty.CreatedOn = DateTime.Now;
                radFileProperty.ExcludeRegEx = Convert.ToString(feedDetailsRow[RMDataSourceConstants.Exclude_Regex]);
                radFileProperty.FeedName = feedName;
                radFileProperty.FieldCount = feedFieldRows.Count;

                if (!string.IsNullOrEmpty(Convert.ToString(feedDetailsRow[RMDataSourceConstants.Field_Delimiter]).Trim()))
                    radFileProperty.FieldDelimiter = Convert.ToChar(feedDetailsRow[RMDataSourceConstants.Field_Delimiter]);

                radFileProperty.FileDate = DateTime.Now.Date;
                radFileProperty.FileId = isFeedPresent ? feedInfoDetailed.FileID : -1;
                radFileProperty.FileName = isFeedPresent ? feedInfoDetailed.FileName : string.Empty;
                radFileProperty.FileType = fileTypeID.ToString();
                radFileProperty.IsActive = true;
                radFileProperty.LastModifiedBy = userName;
                radFileProperty.LastModifiedOn = syncDateTime;
                radFileProperty.PairedEscape = Convert.ToString(feedDetailsRow[RMDataSourceConstants.Paired_Escape]);
                radFileProperty.RecordLength = (fileTypeID == 0 && !string.IsNullOrEmpty(Convert.ToString(feedDetailsRow[RMDataSourceConstants.Record_Length]))) ?
                    Convert.ToInt32(feedDetailsRow[RMDataSourceConstants.Record_Length]) : 0;
                radFileProperty.RecordXPath = fileTypeID == 2 ? Convert.ToString(feedDetailsRow[RMDataSourceConstants.Record_XPath]) : string.Empty;
                radFileProperty.RootXPath = fileTypeID == 2 ? Convert.ToString(feedDetailsRow[RMDataSourceConstants.Root_XPath]) : string.Empty;
                radFileProperty.RowDelimiter = Convert.ToString(feedDetailsRow[RMDataSourceConstants.Record_Delimiter]);
                radFileProperty.SingleEscape = Convert.ToString(feedDetailsRow[RMDataSourceConstants.Single_Escape]);


                feedFieldRows.ForEach(ffd =>
                {
                    //RAD File Field Details
                    radFFD = new RADFileFieldDetailsInfo();
                    string fieldName = Convert.ToString(ffd[RMDataSourceConstants.Feed_Field_Name]);
                    int fieldID = -1;
                    int feedFieldDetailsID = -1;
                    ffdi = null;
                    if (isFeedPresent && feedInfoDetailed.FeedFields != null)
                        ffdi = feedInfoDetailed.FeedFields.Find(f => f.FieldName.SRMEqualWithIgnoreCase(fieldName));

                    if (ffdi != null)
                    {
                        fieldID = ffdi.FieldID;
                        feedFieldDetailsID = ffdi.FeedFieldDetailsID;
                    }

                    radFFD.FieldId = fieldID;
                    radFFD.FieldName = fieldName;
                    radFFD.AllowTrim = GetBooleanValue(Convert.ToString(ffd[RMDataSourceConstants.Allow_Trim]));
                    radFFD.Encrypted = GetBooleanValue(Convert.ToString(ffd[RMModelerConstants.Is_Encrypted]));
                    radFFD.IsActive = true;
                    radFFD.IsPII = GetBooleanValue(Convert.ToString(ffd[RMModelerConstants.Is_PII]));
                    radFFD.Mandatory = GetBooleanValue(Convert.ToString(ffd[RMDataSourceConstants.Mandatory]));
                    radFFD.Persistency = GetBooleanValue(Convert.ToString(ffd[RMDataSourceConstants.Persistable]));
                    radFFD.RemoveWhiteSpaces = GetBooleanValue(Convert.ToString(ffd[RMDataSourceConstants.Remove_White_Spaces]));
                    radFFD.Validation = true;
                    radFFD.CreatedBy = userName;
                    radFFD.CreatedOn = syncDateTime;
                    radFFD.EndIndex = fileTypeID == 0 ? Convert.ToInt32(ffd[RMDataSourceConstants.End_Index]) : 0;
                    radFFD.StartIndex = fileTypeID == 0 ? Convert.ToInt32(ffd[RMDataSourceConstants.Start_Index]) : 0;
                    radFFD.FieldDescription = Convert.ToString(ffd[RMDataSourceConstants.Feed_Field_Description]);
                    radFFD.FieldPosition = (fileTypeID != 2 && fileTypeID != 4) ? Convert.ToString(Convert.ToInt32(ffd[RMDataSourceConstants.Position]) - 1) : "0";
                    radFFD.FieldXPath = (fileTypeID == 2) ? Convert.ToString(ffd[RMDataSourceConstants.X_Path]) : string.Empty;
                    radFFD.FileId = isFeedPresent ? feedInfoDetailed.FileID : -1;
                    radFFD.LastModifiedBy = userName;
                    radFFD.LastModifiedOn = syncDateTime;

                    lstRADFileFieldDetails.Add(radFFD);

                    //RM Feed Field Details
                    rmFFD = new RMFeedFieldDetailsInfo();
                    rmFFD.CreatedBy = userName;
                    rmFFD.CreatedOn = syncDateTime;
                    rmFFD.LastModifiedBy = userName;
                    rmFFD.LastModifiedOn = syncDateTime;
                    rmFFD.FeedFieldDetailsId = feedFieldDetailsID;
                    rmFFD.FeedSummaryId = feedSummaryID;
                    rmFFD.IsActive = true;
                    rmFFD.IsPII = GetBooleanValue(Convert.ToString(ffd[RMModelerConstants.Is_PII]));
                    rmFFD.IsAPI = GetBooleanValue(Convert.ToString(ffd[RMDataSourceConstants.Is_API]));
                    rmFFD.IsBulk = GetBooleanValue(Convert.ToString(ffd[RMDataSourceConstants.Is_Bulk]));
                    rmFFD.IsFTP = GetBooleanValue(Convert.ToString(ffd[RMDataSourceConstants.Is_FTP]));
                    rmFFD.IsPrimary = GetBooleanValue(Convert.ToString(ffd[RMModelerConstants.Is_Primary]));
                    rmFFD.IsUnique = GetBooleanValue(Convert.ToString(ffd[RMDataSourceConstants.Is_Unique]));
                    rmFFD.RadFieldId = fieldID;

                    lstRMFeedFieldDetails.Add(rmFFD);

                    if (!processedFeedVsFields.ContainsKey(RMCommonStatic.ConvertToLower(feedName)))
                        processedFeedVsFields.Add(RMCommonStatic.ConvertToLower(feedName), new HashSet<string>());

                    processedFeedVsFields[RMCommonStatic.ConvertToLower(feedName)].Add(fieldName);
                });

                bool isSaveSuccessful = new RMDataSourceDBManager(mDBCon).AddUpdateFeedSummary(radFileProperty, feedSummaryInfo, lstRADFileFieldDetails.ToArray(), lstRMFeedFieldDetails.ToArray());

                if (isSaveSuccessful)
                {
                    new RMCommonMigration().SetPassedRow(feedDetailsRow, string.Empty);

                    feedFieldRows.ForEach(row =>
                    {
                        new RMCommonMigration().SetPassedRow(row, string.Empty);
                    });
                }
            }
            catch
            {
                throw;
            }

        }

    }
}
