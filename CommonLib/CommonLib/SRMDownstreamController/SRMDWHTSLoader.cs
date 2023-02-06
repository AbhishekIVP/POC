using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.srmdwhjob
{
    public class SRMDWHTSLoader
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMDWHTSLoader");
        private String tempLegtableSchema = String.Empty;

        public List<string> Begin(SRMJobInfo tsReportJob, RDBConnectionManager con, List<string> tablesToBeDropped)
        {
            bool isConnectionMade = false;
            if (con == null)
            {
                isConnectionMade = true;
                con = SRMDWHJobExtension.GetConnectionManager(tsReportJob.DownstreamSQLConnectionName, true, IsolationLevel.RepeatableRead);
            }

            SRMStagingTableModel stagingTableModel = new SRMStagingTableModel();
            try
            {
                string query = string.Format(@" DECLARE @db VARCHAR(500) = DB_NAME()
                                        select id, table_name, created_on, isRefM, surrogate_table_name
	                                        from [dimension].[ivp_srm_dwh_tables_for_loading]
	                                        where is_active=1 and table_type = 'TS' AND table_name = '['+ @db + '].{0}';
	
	                                        select m.table_name, c.name as column_name, t.name as datatype, c.precision, c.scale, c.max_length
	                                        from sys.columns c
	                                        inner join sys.types t
	                                        on c.system_type_id = t.system_type_id
	                                        inner join [dimension].[ivp_srm_dwh_tables_for_loading] m
	                                        on c.object_id = object_id(m.table_name)
	                                        where m.is_active=1 and m.table_type = 'TS' and m.table_name = '['+ @db + '].{0}'", tsReportJob.TableName);

                DataSet ds = SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        stagingTableModel.Id = Convert.ToInt32(dr["id"]);
                        stagingTableModel.StagingTableName = Convert.ToString(dr["table_name"]);
                        stagingTableModel.CreatedOn = Convert.ToDateTime(dr["created_on"]);
                        stagingTableModel.IsActive = true;
                        stagingTableModel.IsRefM = Convert.ToBoolean(dr["isRefM"]);
                        stagingTableModel.SurrogateTableName = Convert.ToString(dr["surrogate_table_name"]);

                        stagingTableModel.columns = new List<SRMStagingTableModel.PStagingTableColumns>();
                        DataRow[] columnInfoRows = ds.Tables[1].AsEnumerable().Where(r => r.Field<string>("table_name") == stagingTableModel.StagingTableName).ToArray();
                        foreach (DataRow columnInfoRow in columnInfoRows)
                        {
                            SRMStagingTableModel.PStagingTableColumns columnInfo = new SRMStagingTableModel.PStagingTableColumns();
                            columnInfo.ColumnName = Convert.ToString(columnInfoRow["column_name"]);
                            columnInfo.DbColumnDatatype = new SRMDWHDataManager().GetDbColumnDatatype(Convert.ToString(columnInfoRow["datatype"]), Convert.ToInt32(columnInfoRow["precision"]), Convert.ToInt32(columnInfoRow["scale"]), Convert.ToInt32(columnInfoRow["max_length"]));
                            stagingTableModel.columns.Add(columnInfo);
                        }
                    }

                    mLogger.Debug("Start Processing Staging Table : " + stagingTableModel.StagingTableName);
                    tablesToBeDropped = ProcessSnapshotsForStage(stagingTableModel, tsReportJob, con, isConnectionMade, tablesToBeDropped);
                    mLogger.Debug("End Processing Staging Table : " + stagingTableModel.StagingTableName);
                }
                if (isConnectionMade)
                    con.CommitTransaction();
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                if (isConnectionMade)
                    con.RollbackTransaction();
                throw;
            }
            finally
            {
                if (isConnectionMade)
                    CommonDALWrapper.PutConnectionManager(con);
            }
            return tablesToBeDropped;
        }

        public List<string> ProcessSnapshotsForStage(SRMStagingTableModel stage, SRMJobInfo tsReportJob, RDBConnectionManager con, bool isConnectionMade, List<string> tablesToBeDropped)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<string> elapsedTimeList = new List<string>();
            long totalTime = 0;
            bool isFullLoad = tsReportJob.DateType.Equals(DWHDateType.None);
            bool createTempTSTable = false, dropMainTSTable = false;
            if (isFullLoad)
                createTempTSTable = true;
            List<string> tempTableNames = new List<string>();

            try
            {
                string uniqueGuid = Guid.NewGuid().ToString().Replace(SRMDWHDataLoaderTaskCommonConstants.HYFEN, SRMDWHDataLoaderTaskCommonConstants.UNDERSCORE);
                String stageTableNameWithoutSchema = stage.StagingTableName.Split('[')[3].Replace("]", "");
                String stagingTableSchemaName = stage.StagingTableName.Split('[')[2].Replace("].", "");
                String stagingTableWithSchema = "[" + stagingTableSchemaName + "].[" + stageTableNameWithoutSchema + "]";
                String LogTableName = stage.StagingLogTableName;
                String StagingTableName = stage.StagingTableName;
                String MainTableName = stage.TimeSeriesTableName;
                String SurrogateTableName = stage.SurrogateTableName;
                String MainTableNameWithoutDB = stage.TimeSeriesTableName.Split('.')[1] + "." + stage.TimeSeriesTableName.Split('.')[2];
                String StagingTableWithoutDB = stage.StagingTableName.Split('.')[1] + "." + stage.StagingTableName.Split('.')[2];

                if (isFullLoad)
                {
                    MainTableName = stage.TimeSeriesTableName.Replace("_time_series]","_time_series_temp_main]");
                    MainTableNameWithoutDB = MainTableNameWithoutDB.Replace("_time_series]", "_time_series_temp_main]");
                }

                List<DateTime> snapshots = new SRMDWHDataManager().GetSnapshotListForStagingTable(stage.StagingTableName, tsReportJob.DownstreamSQLConnectionName, con);

                SRMDWHExecutor objSecuritiesDataLoaderTaskExecuter = new SRMDWHExecutor(con);
                SRMDWHDataManager stageDataManager = new SRMDWHDataManager();

                mLogger.Debug("Get leg table information and create temporary table to store this data");
                DataTable dtLegInfo = GetLegTables(stage, isFullLoad);
                string TempLegInfoTable = objSecuritiesDataLoaderTaskExecuter.CreateTable(SRMDWHDataLoaderTaskCommonConstants.TEMP_TABLE_LEG_INFO, tempLegtableSchema.Trim(','), tsReportJob.DownstreamSQLConnectionName, uniqueGuid);
                tempTableNames.Add(TempLegInfoTable);
                tempTableNames.Add("[temp].temp_extract_columns_" + uniqueGuid);
                tempTableNames.Add("[temp].temp_deleted_master_" + uniqueGuid);                
                logTime(elapsedTimeList, String.Format("Temporary leg information table created successfully. Table Name: {0}", TempLegInfoTable), watch, ref totalTime);

                mLogger.Debug(String.Format("Bulk Copy leg information in temporary lef info table: {0}", TempLegInfoTable));
                SRMDWHJobExtension.ExecuteBulkCopy(tsReportJob.DownstreamSQLConnectionName, TempLegInfoTable, dtLegInfo);
                logTime(elapsedTimeList, "Bulk copy leg information in temporary lef info table successfull", watch, ref totalTime);

                mLogger.Debug("Synchronize attributes for column mappings");
                objSecuritiesDataLoaderTaskExecuter.SynchronizeAttributes(stageTableNameWithoutSchema, TempLegInfoTable, stageTableNameWithoutSchema, stagingTableSchemaName, stage.IsRefM, tsReportJob.DownstreamSQLConnectionName, uniqueGuid);
                logTime(elapsedTimeList, "Synchronize attributes for column mappings successfull", watch, ref totalTime);

                mLogger.Debug("Synchronize tables as per the column mappings");
                objSecuritiesDataLoaderTaskExecuter.SynchronizeTables(LogTableName, stagingTableWithSchema, MainTableName, stage.IsRefM, tsReportJob.DownstreamSQLConnectionName, uniqueGuid, tsReportJob.RequireTimeTSReports);
                logTime(elapsedTimeList, "Synchronize tables as per the column mappings successfull", watch, ref totalTime);

                mLogger.Debug("Synchronize tables indexes");
                objSecuritiesDataLoaderTaskExecuter.SynchronizeIndexes(MainTableNameWithoutDB, stage.IsRefM, false, tsReportJob.DownstreamSQLConnectionName, uniqueGuid);
                logTime(elapsedTimeList, "Synchronize tables indexes", watch, ref totalTime);

                mLogger.Debug("Create modify leg table schema");
                objSecuritiesDataLoaderTaskExecuter.CreateModifyLegTableSchema(TempLegInfoTable, stage.IsRefM, tsReportJob.DownstreamSQLConnectionName, uniqueGuid, tsReportJob.RequireTimeTSReports);
                logTime(elapsedTimeList, "Create modify leg table schema successful", watch, ref totalTime);

                mLogger.Debug("Synchronise index leg table schema");
                objSecuritiesDataLoaderTaskExecuter.SynchronizeIndexesOnLegs(dtLegInfo, stage.IsRefM, tsReportJob.DownstreamSQLConnectionName, uniqueGuid);
                logTime(elapsedTimeList, "Synchronise index leg table schema successful", watch, ref totalTime);

                //foreach (DateTime snap in snapshots)
                for (int i = 0; i < snapshots.Count; i++)
                {
                    if (i > 0)
                        createTempTSTable = false;
                    if (i == snapshots.Count - 1)
                        dropMainTSTable = true;
                    
                    mLogger.Debug("Start Precessing Staging Table : " + stage.StagingTableName + snapshots[i]);
                    ProcessStageSnapshot(stage, snapshots[i].ToString("yyyyMMdd HH:mm:ss.fff"), tsReportJob.DownstreamSQLConnectionName, dtLegInfo, con, watch, elapsedTimeList, totalTime, TempLegInfoTable, uniqueGuid, isConnectionMade,isFullLoad, createTempTSTable, dropMainTSTable);
                    mLogger.Debug("Start Precessing Staging Table : " + stage.StagingTableName + snapshots[i]);
                }
            }
            catch (Exception ex)
            {
                mLogger.Debug("Inside catch");
                mLogger.Debug("Error : " + ex.ToString());
                mLogger.Debug("Error stack Trace : " + ex.StackTrace);

                throw;
            }
            finally
            {
                mLogger.Debug("");
                mLogger.Debug("#########################################################################");
                foreach (string s in elapsedTimeList)
                    mLogger.Debug(s);
                mLogger.Debug("#########################################################################");
                TimeSpan t = TimeSpan.FromMilliseconds(totalTime);
                string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                        t.Hours,
                                        t.Minutes,
                                        t.Seconds,
                                        t.Milliseconds);
                mLogger.Debug("TOTAL TIME : " + answer);
                mLogger.Debug("#########################################################################");
            }
            return tempTableNames;
        }

        public void ProcessStageSnapshot(SRMStagingTableModel stage, string snap, string DBConnectionId, DataTable dtLegInfo, RDBConnectionManager con, Stopwatch watch, List<string> elapsedTimeList, long totalTime, string TempLegInfoTable, string uniqueGuid, bool isConnectionMade, bool isFullLoad, bool createTempTSTable, bool dropMainTSTable)
        {
            logTime(elapsedTimeList, (stage.StagingTableName + " # " + snap), watch, ref totalTime);

            SRMDWHExecutor objSecuritiesDataLoaderTaskExecuter = new SRMDWHExecutor(con);
            SRMDWHDataManager stageDataManager = new SRMDWHDataManager();

            DateTime dtNow = DateTime.Now;

            String stageTableNameWithoutSchema = stage.StagingTableName.Split('[')[3].Replace("]", "");
            String stagingTableSchemaName = stage.StagingTableName.Split('[')[2].Replace("].", "");
            String stagingTableWithSchema = "[" + stagingTableSchemaName + "].[" + stageTableNameWithoutSchema + "]";
            String LogTableName = stage.StagingLogTableName;
            String StagingTableName = stage.StagingTableName;
            String MainTableName = stage.TimeSeriesTableName;
            String SurrogateTableName = stage.SurrogateTableName;
            String MainTableNameWithoutDB = stage.TimeSeriesTableName.Split('.')[1] + "." + stage.TimeSeriesTableName.Split('.')[2];
            String StagingTableWithoutDB = stage.StagingTableName.Split('.')[1] + "." + stage.StagingTableName.Split('.')[2];

            if (isFullLoad)
            {
                MainTableName = stage.TimeSeriesTableName.Replace("_time_series]", "_time_series_temp_main]");
                MainTableNameWithoutDB = MainTableNameWithoutDB.Replace("_time_series]", "_time_series_temp_main]");
            }

            mLogger.Debug("Populate staging archive and time series data");
            objSecuritiesDataLoaderTaskExecuter.PopulateData(LogTableName, stagingTableWithSchema, MainTableName, stage.IsDeltaExtract, stage.SofDeleteSecurity, stage.IsRefM, snap, uniqueGuid, isFullLoad, createTempTSTable, dropMainTSTable);
            logTime(elapsedTimeList, "Populate staging archive and time series data successful", watch, ref totalTime);

            //mLogger.Debug("Populate master ids in master time series data");
            //UpdateMainDataWithMasterId(MainTableName, stage.SurrogateTableName, snap, stage.IsRefM, StagingTableWithoutDB, isLeg: false, pConnection: con);
            //logTime(elapsedTimeList, "Populate master ids in master time series data successful", watch, ref totalTime);

            mLogger.Debug("Populate securities leg table data");
            objSecuritiesDataLoaderTaskExecuter.PopulateLegTableData(TempLegInfoTable, stage.StagingTableName, stage.IsDeltaExtract, stage.SofDeleteSecurity, stage.IsRefM, snap, uniqueGuid, isFullLoad, createTempTSTable, dropMainTSTable);
            logTime(elapsedTimeList, "Populate securities leg table data successful", watch, ref totalTime);


            mLogger.Debug("Populate master ids in leg table data");
            UpdateLegDataWithMasterId(dtLegInfo, snap, stage.IsRefM, con, isFullLoad);
            logTime(elapsedTimeList, "Populate master ids in leg table data successful", watch, ref totalTime);

            if (!isConnectionMade)
            {
                mLogger.Debug("Delete stage data");
                objSecuritiesDataLoaderTaskExecuter.DeleteDataFromStagingTable(stage.StagingTableName, snap);
            }
            else
            {
                DateTime loadingTime = DateTime.ParseExact(snap, "yyyyMMdd HH:mm:ss.fff", null);
                //string query = "SELECT * INTO " + (stage.StagingTableName.Substring(0, stage.StagingTableName.Length - 1) + loadingTime.ToString("dd_MM_yyyy_HH_mm_ss") + "]") + " FROM " + stage.StagingTableName;

                string query = string.Format(@"
                                DECLARE @tableName VARCHAR(1000) = '{0}'
                                DECLARE @newTableName VARCHAR(1000) = PARSENAME('{1}',1)                                
                                					            
                                DECLARE @primaryKeyName VARCHAR(1000) = ''
                                DECLARE @primaryKeyNewName VARCHAR(1000) = '{2}' 
				            
                                SELECT @primaryKeyName = ind.name
                                FROM sys.indexes ind INNER JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
                                INNER JOIN sys.columns col 	ON ic.object_id = col.object_id and ic.column_id = col.column_id 
                                INNER JOIN sys.tables t ON ind.object_id = t.object_id
                                WHERE t.name = PARSENAME(@tableName,1) AND is_primary_key = 1
				
                                IF(ISNULL(@primaryKeyName,'')<>'')
                                BEGIN
	                                SELECT @primaryKeyNewName = @primaryKeyName + @primaryKeyNewName
	                                SELECT @primaryKeyName = PARSENAME(@tableName,2)+ '.' + @primaryKeyName
					
	                                EXEC sp_rename @primaryKeyName , @primaryKeyNewName
	                                
                                END 

                                EXEC sp_rename @tableName , @newTableName

                                ", stage.StagingTableName,
                                (stage.StagingTableName.Substring(0, stage.StagingTableName.Length - 1) + loadingTime.ToString("dd_MM_yyyy_HH_mm_ss") + "]"),
                                loadingTime.ToString("dd_MM_yyyy_HH_mm_ss"));

                SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.SELECT);
            }

        }

        private void UpdateLegDataWithMasterId(DataTable dtLegInfo, string snap, bool isRefM, RDBConnectionManager con, bool isFullLoad)
        {
            string query = string.Format(@"UPDATE taskmanager.ivp_polaris_core_secref_extract_table_column_mapping
                    SET identifier_key = REPLACE(identifier_key,'_temp_leg]',']')");

            if (isFullLoad)
                SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.SELECT);

            query = string.Empty;

            foreach (DataRow dr in dtLegInfo.Rows)
            {
                mLogger.Debug("Populating surrogate ids in Leg " + dr["LegFullTableName"].ToString());
    
                dr["LegFullTableName"] = Convert.ToString(dr["LegFullTableName"]).Replace("_temp_leg", "");
                
                query += string.Format(@"UPDATE T SET T.[master_id] = S.[{5}] from {0} T LEFT OUTER JOIN {1} S ON S.[{2}] = T.[{4}] 
                    where T.loading_time = '{3}'", dr["LegFullTableName"].ToString(), isRefM ? "dimension.ivp_srm_dwh_entity_type_master" : "dimension.ivp_polaris_security_master", isRefM ? "entity_code" : "security id" , snap, isRefM ? "entity_code" : "security_id",isRefM ? "master_id" : "id");
                SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.SELECT);
            }
        }
        
        private void logTime(List<string> record, string message, Stopwatch watch, ref long totals)
        {
            mLogger.Debug(message);
            long t = watch.ElapsedMilliseconds;
            totals = totals + t;
            record.Add(t + "  -  " + message);
            watch.Restart();
        }

        private DataTable GetLegTables(SRMStagingTableModel stage, bool isFullLoad)
        {
            String dbCols = String.Empty;
            String extractCol = String.Empty;
            String colName = String.Empty;
            String extractColWithoutSurrogate = String.Empty;
            String colNameWithoutSurrogate = String.Empty;
            String colType = String.Empty;

            // To store leg table names.
            Hashtable hsTables = new Hashtable();

            // DataTable to store securities leg information.
            DataTable dtLegInfo = new DataTable();

            dtLegInfo.Columns.Add("LegTableName", typeof(System.String));
            dtLegInfo.Columns.Add("LegDBCol", typeof(System.String));
            dtLegInfo.Columns.Add("LegExtractCol", typeof(System.String));
            dtLegInfo.Columns.Add("LegFullTableName", typeof(System.String));
            dtLegInfo.Columns.Add("IfNotExistCondition", typeof(System.String));
            dtLegInfo.Columns.Add("column_name", typeof(System.String));
            dtLegInfo.Columns.Add("column_type", typeof(System.String));
            dtLegInfo.Columns.Add("LegExtractColWithoutSurrogate", typeof(System.String));
            dtLegInfo.Columns.Add("column_name_without_surrogate", typeof(System.String));

            // Get leg table names.
            foreach (SRMStagingTableModel.PStagingTableColumns dc in stage.columns)
            {
                if (dc.ColumnName.Contains("-"))
                {
                    string tableName;
                    String[] splits = dc.ColumnName.Split('-');

                    if (splits[0].Trim() == "EC") continue;

                    tableName = splits[0].Trim();

                    if (hsTables[tableName] == null)
                    {
                        hsTables[tableName] = String.Empty;
                    }
                }
            }

            // Get leg table column and data type info for leg tables
            //TODO: check column data type length
            foreach (String key in hsTables.Keys)
            {
                mLogger.Debug("Leg table name key: " + key);
                foreach (SRMStagingTableModel.PStagingTableColumns dc in stage.columns)
                {
                    if (dc.ColumnName.StartsWith(key + "-") || dc.ColumnName.StartsWith(key + " -"))
                    {
                        String[] splits = dc.ColumnName.Split('-');
                        if (splits[1].Trim() == "EC")
                        {
                            dbCols += "[" + splits[1].Trim() + "-" + splits[2].Trim() + "]" + " " + dc.DbColumnDatatype + "|";
                            extractCol += "[" + dc.ColumnName + "],";
                            colName += "[" + splits[1].Trim() + "-" + splits[2].Trim() + "]" + ",";
                            extractColWithoutSurrogate += "[" + dc.ColumnName + "],";
                            colNameWithoutSurrogate += "[" + splits[1].Trim() + "-" + splits[2].Trim() + "]" + ",";
                            colType += dc.DbColumnDatatype + ";";

                        }
                        else
                        {
                            dbCols += "[" + splits[1].Trim() + "]" + " " + dc.DbColumnDatatype + "|";
                            extractCol += "[" + dc.ColumnName + "],";
                            colName += "[" + splits[1].Trim() + "]" + ",";
                            extractColWithoutSurrogate += "[" + dc.ColumnName + "],";
                            colNameWithoutSurrogate += "[" + splits[1].Trim() + "]" + ",";
                            colType += dc.DbColumnDatatype + ";";

                        }
                    }
                }

                DataRow dr = dtLegInfo.NewRow();

                string LegTablePrefix = null;
                if (stage.IsRefM)
                {
                    string masterEntityName = stage.StagingTableName.Split('[')[3].TrimEnd(']').Replace("ivp_polaris_", "").Replace("_staging", "").Replace("_time_series", "").ToLower().Replace(" ", "");
                    LegTablePrefix = string.Format(SRMDWHDataLoaderTaskCommonConstants.REF_LEG_TABLE_PREFIX, masterEntityName);
                }
                else
                {
                    LegTablePrefix = SRMDWHDataLoaderTaskCommonConstants.SEC_LEG_TABLE_PREFIX;
                }

                dr[0] = key;
                dr[1] = dbCols + "[Row Id] varchar(1000)";
                dr[2] = extractCol + "[Row Id]";
                dr[3] = "[" + SRMDWHDataLoaderTaskCommonConstants.DEFAULT_REF_SCHEMA_PREFIX + "]" + SRMDWHDataLoaderTaskCommonConstants.DOT + "[" + LegTablePrefix + "_" + key.Replace(" ", "_").ToLower() + "_time_series" + "]";
                if(isFullLoad)
                    dr[3] = "[" + SRMDWHDataLoaderTaskCommonConstants.DEFAULT_REF_SCHEMA_PREFIX + "]" + SRMDWHDataLoaderTaskCommonConstants.DOT + "[" + LegTablePrefix + "_" + key.Replace(" ", "_").ToLower() + "_time_series" + "_temp_leg]";
                dr[4] = "if not exists (select * from sys.tables where object_id = object_id('" + dr[3].ToString() + "'))";
                dr[5] = colName + "[Row Id]";
                dr[6] = colType + "varchar(1000)";
                dr[7] = extractColWithoutSurrogate + "[Row Id]";
                dr[8] = colNameWithoutSurrogate + "[Row Id]";

                dtLegInfo.Rows.Add(dr);

                dbCols = String.Empty;
                extractCol = String.Empty;
                colName = String.Empty;
                colType = String.Empty;
                extractColWithoutSurrogate = String.Empty;
                colNameWithoutSurrogate = String.Empty;
            }

            // Get leg info table schema
            tempLegtableSchema = "";
            foreach (DataColumn dc in dtLegInfo.Columns)
            {
                tempLegtableSchema = tempLegtableSchema + "[" + dc.ColumnName + "] " + GetDbColType(Convert.ToString(dc.DataType)) + ",";
            }

            hsTables = null;
            return dtLegInfo;
        }

        private String GetDbColType(String cSharpType)
        {
            String dbType = String.Empty;

            switch (cSharpType)
            {
                case "System.String":
                    dbType = "varchar(max)";
                    break;
                case "System.Double":
                case "System.Decimal":
                    dbType = "numeric(30,10)";
                    break;
                case "System.Bool":
                    dbType = "bit";
                    break;
                case "System.Int32":
                    dbType = "int";
                    break;
                case "System.DateTime":
                    dbType = "datetime";
                    break;
                case "System.TimeSpan":
                    dbType = "time";
                    break;
                default:
                    dbType = "varchar(max)";
                    break;
            }
            return dbType;
        }
    }

    public class SRMDWHExecutor
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMDWHExecutor");
        private String _tempLoadFullTableName;
        private String _tempTableName;
        private RDBConnectionManager _connection;

        internal SRMDWHExecutor(RDBConnectionManager pConnection)
        {
            _connection = pConnection;
        }

        internal String CreateTable(String tablePrefix, String Schema, string downstreamConnectionName, string uniqueGuid)
        {
            try
            {
                _tempTableName = tablePrefix + "_" + uniqueGuid;
                _tempLoadFullTableName = SRMDWHDataLoaderTaskCommonConstants.TEMP_TABLE_SCHEMA + "." + _tempTableName;

                string query = string.Format("EXEC {0} '{1}','{2}'", SRMDWHDataLoaderTaskDBConstants.PROC_CREATE_TEMP_TABLE, _tempLoadFullTableName, Schema);
                SRMDWHJobExtension.ExecuteQuery(downstreamConnectionName, query, SRMDBQueryType.SELECT);
                return _tempLoadFullTableName;
            }
            finally
            {

            }
        }
        internal void SynchronizeAttributes(String TempTableName, String TempLegInfoTableName, String UserDefinedTableName, String SchemaName, bool isRefM, string downstreamConnectionName, string uniqueGuid)
        {
            try
            {
                string query = string.Format("EXEC {0} '{1}','{2}','{3}','{4}',{5},'{6}'", SRMDWHDataLoaderTaskDBConstants.PROC_SYNCHRONIZE_ATTRIBUTES, TempTableName, TempLegInfoTableName, UserDefinedTableName, SchemaName, isRefM, uniqueGuid);
                SRMDWHJobExtension.ExecuteQuery(downstreamConnectionName, query, SRMDBQueryType.SELECT);
            }
            finally
            {

            }

        }

        /// <summary>
        /// Method calls procedure to synchronize security data tables with extract schema.
        /// </summary>
        internal void SynchronizeTables(String LogTableName, String StagingTableName, String MainTableName, bool isRefM, string downstreamConnectionName, string uniqueGuid, bool requireTimeTSReports)
        {
            try
            {
                string query = string.Format("EXEC {0} '{1}','{2}','{3}',{4},'{5}',{6}", SRMDWHDataLoaderTaskDBConstants.PROC_SYNCHRONIZE_TABLES, LogTableName, StagingTableName, MainTableName, isRefM, uniqueGuid, requireTimeTSReports);
                SRMDWHJobExtension.ExecuteQuery(downstreamConnectionName, query, SRMDBQueryType.SELECT);
            }
            finally
            {

            }
        }

        internal void SynchronizeIndexes(String tableName, bool isRefM, bool isLeg, string downstreamConnectionName, string uniqueGuid)
        {
            try
            {
                string query = string.Format("EXEC {0} '{1}',{2},{3},'{4}'", SRMDWHDataLoaderTaskDBConstants.PROC_SYNCHRONISE_INDEXES, tableName, isRefM, isLeg, uniqueGuid);

                SRMDWHJobExtension.ExecuteQuery(downstreamConnectionName, query, SRMDBQueryType.SELECT);
            }
            finally
            {

            }
        }

        internal void SynchronizeIndexesOnLegs(DataTable dtLegInfo, bool isRef, string downstreamConnectionName, string uniqueGuid)
        {
            foreach (DataRow dr in dtLegInfo.Rows)
            {
                mLogger.Debug("Synchronize indexes in Leg " + dr["LegFullTableName"].ToString());
                SynchronizeIndexes(dr["LegFullTableName"].ToString(), isRef, true, downstreamConnectionName, uniqueGuid);
            }
        }


        /// <summary>
        /// Method calls procedure to populate security data into tables from extract.
        /// </summary>
        internal void PopulateData(String LogTableName, String StagingTableName, String MainTableName, Boolean IsDeltaExtract, Boolean SoftDeleteSecurity, Boolean isRefM, string snapshot, string uniqueGuid, bool isFullLoad, bool createTempTSTable, bool dropMainTSTable)
        {
            string query = string.Format("EXEC {0} '{1}','{2}','{3}',{4},{5},{6},'{7}','{8}',{9},{10},{11}", SRMDWHDataLoaderTaskDBConstants.PROC_POPULATE_DATA, LogTableName, StagingTableName, MainTableName, IsDeltaExtract, SoftDeleteSecurity, isRefM, snapshot, uniqueGuid, isFullLoad, createTempTSTable, dropMainTSTable);
            CommonDALWrapper.ExecuteSelectQuery(query, _connection);
        }

        /// <summary>
        /// Methos calls procedure to create or modify leg table's schema based on sec master extract.
        /// </summary>
        /// <param name="tmpLegInfoTableName">DataTable with customized leg data info</param>
        internal void CreateModifyLegTableSchema(String tmpLegInfoTableName, Boolean isRefM, string downstreamConnectionName, string uniqueGuid, bool requireIntraDayChanges)
        {
            try
            {
                string query = string.Format("EXEC {0} '{1}',{2}, '{3}',{4}", SRMDWHDataLoaderTaskDBConstants.PROC_CREATE_MODIFY_LEG_TABLE_SCHEMA, tmpLegInfoTableName, isRefM, uniqueGuid, requireIntraDayChanges);
                SRMDWHJobExtension.ExecuteQuery(downstreamConnectionName, query, SRMDBQueryType.SELECT);
            }
            finally
            {

            }
        }


        /// <summary>
        /// Method calls procedure to populate le table data from extract.
        /// </summary>
        /// <param name="tmpLegInfoTableName">Leg info table name.</param>
        /// <param name="tmpExtractTableName">Extract data table name.</param>
        internal void PopulateLegTableData(String tmpLegInfoTableName, String tmpExtractTableName, Boolean IsDeltaExtract, Boolean SoftDeleteEntity, Boolean isRefM, string snapshot, string uniqueGuid, bool isFullLoad, bool createTempTSTable, bool dropMainTSTable)
        {
            string query = string.Format("EXEC {0} '{1}','{2}',{3},{4},{5},'{6}','{7}',{8},{9},{10}", SRMDWHDataLoaderTaskDBConstants.PROC_POPULATE_LEG_TABLE_DATA, tmpLegInfoTableName, tmpExtractTableName, IsDeltaExtract, SoftDeleteEntity, isRefM, snapshot, uniqueGuid, isFullLoad, createTempTSTable, dropMainTSTable);
            CommonDALWrapper.ExecuteSelectQuery(query, _connection);
        }

        internal void DropTable(String tableName, string downstreamConnectionName)
        {
            mLogger.Debug("Calling func DropTable");
            try
            {

                var tableNames = tableName.Split('.');
                string query = string.Format(@"IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{1}' AND TABLE_SCHEMA = '{0}')
	                                        DROP TABLE {2}", tableNames[0], tableNames[1], tableName);
                mLogger.Debug("query" + query);
                SRMDWHJobExtension.ExecuteQuery(downstreamConnectionName, query.ToString(), SRMDBQueryType.SELECT);
            }
            catch (Exception ex)
            {
                throw;
            }
            mLogger.Debug("END func DropTable");
        }
        internal DataSet GetStagingTableList(string downstreamConnectionName, string stagingTableName)
        {

            string query = string.Format("EXEC {0} '{1}'", SRMDWHDataLoaderTaskDBConstants.PROC_GET_STAGING_TABLE_LIST, stagingTableName);
            return CommonDALWrapper.ExecuteSelectQuery(query, _connection);

        }
        internal DataTable GetSnapshotListForStagingTable(string stagingTableName)
        {
            string query = string.Format("EXEC {0} '{1}'", SRMDWHDataLoaderTaskDBConstants.PROC_GET_SNAPSHOT_LIST_FOR_STAGING_TABLE, stagingTableName);
            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, _connection);

            return ds.Tables[0];
        }

        internal void DeleteDataFromStagingTable(string stageTableName, string snapshot)
        {
            try
            {
                string query = string.Format("EXEC {0} '{1}','{2}'", SRMDWHDataLoaderTaskDBConstants.PROC_DELETE_SNAPSHOT_FROM_STAGE, stageTableName, snapshot);
                CommonDALWrapper.ExecuteSelectQuery(query, _connection);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class SRMStagingTableModel
    {

        public class PStagingTableColumns
        {
            public string ColumnName;

            public string DbColumnDatatype;
        }


        public SRMStagingTableModel()
        {
            IsDeltaExtract = true;
            SofDeleteSecurity = true;
        }

        public int Id;

        public string StagingTableName;

        public string StagingLogTableName
        {
            get { return StagingTableName.TrimEnd(']') + "_log]"; }
        }

        public string SurrogateTableName;

        public DateTime CreatedOn;

        public bool IsActive;

        public bool IsRefM;

        public bool IsDeltaExtract;

        public bool SofDeleteSecurity;
        public string TimeSeriesTableName
        {
            get
            {

                if (IsRefM)
                    return StagingTableName.ToLower().Replace(" ", "_").Replace("_staging]", "]").Replace("[taskmanager]", "[dimension]").Replace("[references]", "[dimension]");
                else
                    return StagingTableName.ToLower().Replace(" ", "_").Replace("_staging]", "]").Replace("[taskmanager]", "[dimension]");
            }
        }

        public List<PStagingTableColumns> columns;

    }

    public class SRMDWHDataManager
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMDWHDataManager");
        private RDBConnectionManager _connection;

        public List<DateTime> GetSnapshotListForStagingTable(string stagingTableName, string downstreamConnectionName, RDBConnectionManager _connection)
        {
            try
            {
                SRMDWHExecutor objSecuritiesDataLoaderTaskExecuter = new SRMDWHExecutor(_connection);
                DataTable dt = objSecuritiesDataLoaderTaskExecuter.GetSnapshotListForStagingTable(stagingTableName);
                List<DateTime> retVal = dt.AsEnumerable().Select(r => ((DateTime)r["loading_time"])).ToList();
                retVal.Sort();
                return retVal;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public string GetDbColumnDatatype(string datatype, int precision, int scale, int maxLength)
        {
            switch (datatype)
            {
                case "numeric":
                case "decimal":
                    return datatype + "(" + precision + "," + scale + ")";
                case "varchar":
                case "nvarchar":
                case "char":
                    if (maxLength == -1)
                        return datatype + "(max)";
                    else
                        return datatype + "(" + maxLength + ")";
                default:
                    return datatype;
            }
        }


        public bool isLookUpColumn(string datatype, int maxLength)
        {
            if ((datatype.ToLower() == "varchar" || datatype.ToLower() == "nvarchar") && (maxLength == 100))
                return true;

            return false;
        }

    }

    class SRMDWHDataLoaderTaskCommonConstants
    {
        internal const String FILE_PATH = "File Path";
        internal const String TABLE_NAME = "Table Name";
        internal const String SCHEMA_NAME = "Schema Name";
        internal const String IS_DELTA_EXTARCT = "Is Delta Extract";
        internal const String SOFT_DELETE_SECURITY = "Soft Delete Security";
        internal const String TEMP_TABLE_PREFIX = "ivp_polaris_securities_data_loader_task_temp";
        internal const String TEMP_TABLE_LEG_INFO = "ivp_polaris_leg_info_temp";
        internal const String SEC_LEG_TABLE_PREFIX = "ivp_polaris_securities_leg";
        internal const String REF_LEG_TABLE_PREFIX = "ivp_polaris_{0}_leg";
        internal const String TEMP_TABLE_SCHEMA = "temp";
        internal const char UNDERSCORE = '_';
        internal const char HYFEN = '-';
        internal const char DOT = '.';
        internal const String DEFAULT_REF_SCHEMA_PREFIX = "dimension";
        internal const String CONTINUE_EXECUTION = "Continue Execution";
        internal const String DB_CONNECTION_ID = "Database Connection Id";
    }

    /// <summary>
    /// Lists all the Db constants.
    /// </summary>
    class SRMDWHDataLoaderTaskDBConstants
    {
        internal const String PROC_CREATE_TEMP_TABLE = "[commons].[sp_ivp_polaris_core_secref_task_create_temp_table_with_schema]";
        internal const String PROC_SYNCHRONIZE_ATTRIBUTES = "[taskmanager].[sp_ivp_polaris_core_secref_synchronize_attributes]";
        internal const String PROC_SYNCHRONIZE_TABLES = "[taskmanager].[sp_ivp_polaris_core_secref_synchronize_tables]";
        internal const String PROC_POPULATE_DATA = "[taskmanager].[sp_ivp_polaris_core_secref_populate_time_series_data]";
        internal const String PROC_CREATE_MODIFY_LEG_TABLE_SCHEMA = "[taskmanager].[sp_ivp_polaris_core_secref_create_modify_leg_table_schema]";
        internal const String PROC_POPULATE_LEG_TABLE_DATA = "[taskmanager].[sp_ivp_polaris_core_secref_populate_leg_table_data]";

        internal const String PROC_GET_STAGING_TABLE_LIST = "[taskmanager].[sp_ivp_polaris_core_secref_get_staging_table_list]";
        internal const String PROC_GET_SNAPSHOT_LIST_FOR_STAGING_TABLE = "[taskmanager].[sp_ivp_polaris_core_secref_get_snapshot_list_for_staging_table]";
        internal const String PROC_POPULATE_SURROGATE_IDS = "[taskmanager].[sp_ivp_polaris_core_secref_populate_surrogate_ids]";
        internal const String PROC_DELETE_SNAPSHOT_FROM_STAGE = "[taskmanager].[sp_ivp_polaris_core_secref_delete_snapshot_from_stage]";
        internal const String PROC_SYNCHRONISE_INDEXES = "[taskmanager].[sp_ivp_polaris_core_secref_synchronise_indexes]";
        internal const String PROC_SYNCHRONISE_SURROGATE_VIEW = "[taskmanager].[sp_ivp_polaris_core_secref_synchronise_surrogate_view]";

    }

}
