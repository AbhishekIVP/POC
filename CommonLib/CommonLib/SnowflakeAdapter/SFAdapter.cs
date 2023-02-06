using com.ivp.rad.common;
using com.ivp.rad.dal;
using DWHAdapter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnowflakeAdapter
{

    public class SnowFlakeAdapter : IDWHAdapter
    {
        static object lockObject = new object();
        static SnowFlakeAdapter instance;
        static IRLogger logger = RLogFactory.CreateLogger("SnowFlakeAdapter");

        private SnowFlakeAdapter()
        {
            SFConnection connection = null;

            try
            {
                logger.Debug("SnowFlakeAdapter -> CTOR -> START");

                string query = $"CREATE SCHEMA IF NOT EXISTS \"{SFRepository.SCHEMA_NAME}\"";
                connection = new SFConnection();
                connection.ExecuteNonQuery(new string[] { query });
            }
            finally
            {
                connection?.Dispose();
                logger.Debug("SnowFlakeAdapter -> CTOR -> END");
            }
        }

        public static void InitAdapter(int adapterId)
        {
            lock (lockObject)
            {
                if (instance is null)
                {
                    instance = new SnowFlakeAdapter();
                    DWHAdapterController.Register(adapterId, instance);
                }
            }
        }

        public DWHError? SyncMasterTables(string sQLServerConnectionName)
        {
            logger.Debug("SyncMasterTables -> START");
            try
            {
                var now = DateTime.Now;
                var masterTables = SQLServerRepository.GetMasterTableNames(sQLServerConnectionName);
                Parallel.ForEach(masterTables, (metadata) =>
                {
                    SyncMasterTableData(sQLServerConnectionName, metadata);
                });

                return null;
            }
            catch (Exception e)
            {
                logger.Debug($"SyncMasterTables -> EXCEPTION -> {e}");
                return new DWHError(e.Message);
            }
            finally
            {
                logger.Debug("SyncMasterTables -> END");
            }
        }

        public FailedSyncMetadata? SyncDailyTableData(string sQLServerConnectionName, TSAndDailyMetadata metadata, RDBConnectionManager connectionManager)
        {
            logger.Debug("SyncDailyTableData -> START");

            List<string> transientTables = null;
            Dictionary<TableSyncMetadata, ValueTuple<SQLServerColumnMetaData[], string, string, string[]>> TSMetaVsInfoForSyncing = null;
            Dictionary<TableSyncMetadata, ValueTuple<SQLServerColumnMetaData[], string, string, string[]>> DailyMetaVsInfoForSyncing = null;

            SFConnection connection = null;
            try
            {
                DumpDatainTablesBeforeTransactionStarts(sQLServerConnectionName, metadata, connectionManager, out transientTables, out TSMetaVsInfoForSyncing, out DailyMetaVsInfoForSyncing);

                connection = new SFConnection(true);

                foreach (var metaLevel in TSMetaVsInfoForSyncing)
                {
                    var (columnMetaData, transientTableName, mainTableName, key) = metaLevel.Value;
                    SyncTableWithoutDumpingData(metaLevel.Key, key, connection, true, transientTableName, mainTableName, columnMetaData);
                }

                foreach (var metaLevel in DailyMetaVsInfoForSyncing)
                {
                    var (columnMetaData, transientTableName, mainTableName, key) = metaLevel.Value;
                    SyncTableWithoutDumpingData(metaLevel.Key, key, connection, false, transientTableName, mainTableName, columnMetaData);
                }

                connection?.Commit();
                return null;
            }
            catch (Exception e)
            {
                logger.Error($"SyncDailyTableData : Exception -> {e}");
                connection?.Rollback();
                return new FailedSyncMetadata(e.Message);
            }
            finally
            {
                connection?.Dispose();

                foreach (var table in transientTables)
                    DropTransientTable(table);

                logger.Debug("SyncDailyTableData -> END");
            }
        }

        private static void DumpDatainTablesBeforeTransactionStarts(string sQLServerConnectionName, TSAndDailyMetadata metadata, RDBConnectionManager connectionManager, out List<string> transientTables, out Dictionary<TableSyncMetadata, (SQLServerColumnMetaData[], string, string, string[])> TSMetaVsInfoForSyncing, out Dictionary<TableSyncMetadata, (SQLServerColumnMetaData[], string, string, string[])> DailyMetaVsInfoForSyncing)
        {
            var tsLegKey = new string[] { "master_id", "effective_start_date", "Row Id" };
            var tsKey = new string[] { "master_id", "effective_start_date" };

            var dailyKey = new string[] { "Effective Date", "master_id" };
            var dailyLegKey = new string[] { "Effective Date", "master_id", "Row Id" };

            var transientTablesTemp = new List<string>();

            var TSMetaVsInfoForSyncingTemp = new Dictionary<TableSyncMetadata, (SQLServerColumnMetaData[], string, string, string[])>();
            var DailyMetaVsInfoForSyncingTemp = new Dictionary<TableSyncMetadata, (SQLServerColumnMetaData[], string, string, string[])>();

            SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

            try
            {
                Parallel.ForEach(metadata.TSMetadata, (meta) =>
                {
                    var keys = meta.IsLegTable ? tsLegKey : tsKey;
                    CreateAndLoadTransientTable(sQLServerConnectionName, meta, connectionManager, keys, out SQLServerColumnMetaData[] columnMetaData, out bool hasRecords, out string transientTableName, out string mainTableName);

                    semaphore.Wait();

                    try
                    {
                        if (transientTableName != null)
                            transientTablesTemp.Add(transientTableName);
                        if (hasRecords)
                            TSMetaVsInfoForSyncingTemp[meta] = (columnMetaData, transientTableName, mainTableName, keys);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                Parallel.ForEach(metadata.DailyMetadata, (meta) =>
                {
                    var keys = meta.IsLegTable ? dailyLegKey : dailyKey;
                    CreateAndLoadTransientTable(sQLServerConnectionName, meta, connectionManager, keys, out SQLServerColumnMetaData[] columnMetaData, out bool hasRecords, out string transientTableName, out string mainTableName);

                    semaphore.Wait();

                    try
                    {
                        if (transientTableName != null)
                            transientTablesTemp.Add(transientTableName);
                        if (hasRecords)
                            DailyMetaVsInfoForSyncingTemp[meta] = (columnMetaData, transientTableName, mainTableName, keys);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }
            finally
            {
                semaphore.Dispose();
                transientTables = transientTablesTemp;
                TSMetaVsInfoForSyncing = TSMetaVsInfoForSyncingTemp;
                DailyMetaVsInfoForSyncing = DailyMetaVsInfoForSyncingTemp;
            }
        }

        public FailedSyncMetadata? SyncNTSTableData(string sQLServerConnectionName, TableSyncMetadata metadata, RDBConnectionManager connectionManager)
        {
            logger.Debug("SyncNTSTableData -> START");
            try
            {
                var keys = new string[] { "master_id" };
                SyncTable(sQLServerConnectionName, connectionManager, metadata, keys, metadata.IsLegTable);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error($"SyncNTSTableData : Table -> {metadata.TableName} Exception -> {ex}");

                return new FailedSyncMetadata(ex.Message);
            }
            finally
            {
                logger.Debug("SyncNTSTableData -> END");
            }
        }

        public FailedSyncMetadata? DeleteInactiveInstruments(string masterTableName, string tableName)
        {
            logger.Debug("DeleteInactiveInstruments -> START");
            SFConnection connection = null;
            try
            {
                (masterTableName, _) = CreateMainTableName(masterTableName);
                (tableName, _) = CreateMainTableName(tableName);

                var keys = new string[][] { new string[] { "id", "master_id" } };
                var deleteStatement = SFRepository.BuildDeleteStatement(masterTableName, tableName, null, keys, new string[] { "source.\"is_deleted\" = true" });

                connection = new SFConnection(true);
                connection.ExecuteNonQuery(new string[] { deleteStatement });
                connection?.Commit();
                return null;
            }
            catch (Exception ex)
            {
                connection?.Rollback();
                logger.Error($"DeleteInactiveInstruments : Table -> {tableName} Exception -> {ex}");

                return new FailedSyncMetadata(ex.Message);
            }
            finally
            {
                connection?.Dispose();
                logger.Debug("DeleteInactiveInstruments -> END");
            }
        }

        public FailedSyncMetadata? SyncMasterTableData(string sQLServerConnectionName, MasterTableMetaData metadata)
        {
            logger.Debug("SyncMasterTableData -> START");
            logger.Debug($"SyncMasterTableData -> Metadata : NoneToNow -> {metadata.IsNoneToNow} LoadingTime -> {metadata.LoadingTime} IsRefm -> {metadata.IsRefM} TableName -> {metadata.TableName}");

            try
            {
                var keys = new string[] { metadata.IsRefM ? "entity_code" : "Security Id" };
                var syncMetadata = new TableSyncMetadata(false, metadata.IsNoneToNow, metadata.LoadingTime, metadata.TableName);
                SyncTable(sQLServerConnectionName, null, syncMetadata, keys, false);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error($"SyncMasterTableData : Table -> {metadata.TableName} Exception -> {ex}");
                return new FailedSyncMetadata(ex.Message);
            }
            finally
            {
                logger.Debug("SyncMasterTableData -> END");
            }
        }

        public DWHError? SyncViewsSchema(RDBConnectionManager connectionManager, string viewDefinition)
        {
            logger.Debug("SyncViewsSchema -> START");
            SFConnection connection = null;
            try
            {
                var viewStatement = SQLServerRepository.GetViewDefinition(connectionManager, viewDefinition);
                viewStatement = SFRepository.BuildCreateViewStatementFromSQLServerStatement(viewStatement);

                connection = new SFConnection(true);
                connection.ExecuteNonQuery(new string[] { viewStatement });
                connection?.Commit();
                return null;
            }
            catch (Exception ex)
            {
                logger.Error("SyncViewsSchema : Exception -> " + ex.ToString());
                connection?.Rollback();
                return new DWHError(ex.Message);
            }
            finally
            {
                connection?.Dispose();
                logger.Debug("SyncViewsSchema -> END");
            }
        }

        public DWHError? SyncTableSchema(string tableDefinition)
        {
            logger.Debug("SyncTableSchema -> START");
            SFConnection connection = null;

            try
            {
                var splitAtAlter = tableDefinition.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string[] alterStatements = new string[splitAtAlter.Length];

                for (var counter = 0; counter < splitAtAlter.Length; counter++)
                {
                    var statement = SFRepository.BuildAlterTableStatementFromSQLServerStatement(splitAtAlter[counter]);
                    alterStatements[counter] = statement;
                }
                connection = new SFConnection(true);
                connection.ExecuteNonQuery(alterStatements);
                connection?.Commit();
                return null;
            }
            catch (Exception ex)
            {
                logger.Error("SyncTableSchema : Exception -> " + ex.ToString());
                connection?.Rollback();
                return new DWHError(ex.Message);
            }
            finally
            {
                connection?.Dispose();
                logger.Debug("SyncTableSchema -> END");
            }
        }

        private static void SyncTable(string sQLServerConnectionName, RDBConnectionManager connectionManager, TableSyncMetadata meta, string[] key, bool deleteInsertRecords)
        {
            string transientTableName = null;
            SFConnection connection = null;

            logger.Debug($"SyncTable -> IsLegTable : {meta.IsLegTable}, IsNoneToNow : {meta.IsNoneToNow}, TableName : {meta.TableName}");

            try
            {
                CreateAndLoadTransientTable(sQLServerConnectionName, meta, connectionManager, key, out SQLServerColumnMetaData[] columnMetaData, out bool hasRecords, out transientTableName, out string mainTableName);

                if (hasRecords)
                {
                    connection = new SFConnection(true);
                    if (meta.IsNoneToNow)
                    {
                        var dropTableCommand = SFRepository.BuildDropTableStatement(mainTableName);
                        var renameTableCommand = SFRepository.BuildRenameTableStatement(mainTableName, transientTableName);
                        connection.ExecuteNonQuery(new string[] { dropTableCommand, renameTableCommand });
                    }
                    else
                    {
                        string[] commands;
                        if (deleteInsertRecords)
                        {
                            var deleteCommand = SFRepository.BuildDeleteStatement(transientTableName, mainTableName, key, null, null);
                            var insertCommand = SFRepository.BuildInsertStatement(transientTableName, mainTableName, columnMetaData);
                            commands = new string[] { deleteCommand, insertCommand, };
                        }
                        else
                            commands = new string[] { SFRepository.BuildMergeQueryStatement(transientTableName, mainTableName, key, columnMetaData) };

                        connection.ExecuteNonQuery(commands);
                    }
                }

                connection?.Commit();
            }
            catch
            {
                connection?.Rollback();
                throw;
            }
            finally
            {
                connection?.Dispose();

                if (transientTableName != null)
                    DropTransientTable(transientTableName);
            }
        }

        private static void SyncTableWithoutDumpingData(TableSyncMetadata meta, string[] key, SFConnection connection, bool deleteInsertRecords, string transientTableName, string mainTableName, SQLServerColumnMetaData[] columnMetaData)
        {
            logger.Debug($"SyncTableWithoutDumpingData -> IsLegTable : {meta.IsLegTable}, IsNoneToNow : {meta.IsNoneToNow}, TableName : {meta.TableName}");

            if (meta.IsNoneToNow)
            {
                var dropTableCommand = SFRepository.BuildDropTableStatement(mainTableName);
                var renameTableCommand = SFRepository.BuildRenameTableStatement(mainTableName, transientTableName);
                connection.ExecuteNonQuery(new string[] { dropTableCommand, renameTableCommand });
            }
            else
            {
                string[] commands;
                if (deleteInsertRecords)
                {
                    var deleteCommand = SFRepository.BuildDeleteStatement(transientTableName, mainTableName, key, null, null);
                    var insertCommand = SFRepository.BuildInsertStatement(transientTableName, mainTableName, columnMetaData);
                    commands = new string[] { deleteCommand, insertCommand, };
                }
                else
                    commands = new string[] { SFRepository.BuildMergeQueryStatement(transientTableName, mainTableName, key, columnMetaData) };

                connection.ExecuteNonQuery(commands);
            }
        }

        private static void DropTransientTable(string transientTableName)
        {
            SFConnection connection = null;

            try
            {
                connection = new SFConnection(false);
                string dropTransientTableCommand = SFRepository.BuildDropTableStatement(transientTableName);
                connection.ExecuteNonQuery(new string[] { dropTransientTableCommand });
            }
            catch (Exception ee)
            {
                logger.Error($"DropTransientTable -> EXCEPTION -> {ee}");
            }
            finally
            {
                connection?.Dispose();
            }
        }

        private static void CreateAndLoadTransientTable(string sQLServerConnectionName, TableSyncMetadata metadata, RDBConnectionManager connectionManager, string[] keys, out SQLServerColumnMetaData[] columnMetaData, out bool proceedFurther, out string transientTableName, out string mainTableName)
        {
            columnMetaData = SQLServerRepository.GetTableSchema(sQLServerConnectionName, metadata.TableName);

            var (hasRecords, filePath, fileName, guid) = FetchDataAndWriteFile(sQLServerConnectionName, metadata, columnMetaData, connectionManager);

            var (mainTable, transientTable) = GetTableName(metadata, guid);
            transientTableName = transientTable;
            mainTableName = mainTable;

            if (hasRecords)
            {
                var createTableStatement = SFRepository.BuildCreateTableStatement(transientTableName, false, keys, columnMetaData);
                CopyDataToTransientTable(filePath, fileName, guid, transientTableName, createTableStatement, columnMetaData, keys);
            }

            proceedFurther = hasRecords;
        }

        private static (string, string) GetTableName(TableSyncMetadata metadata, string guid)
        {
            var (mainTableName, arr) = CreateMainTableName(metadata.TableName);
            string transientTableName = $"\"{arr[0]}\".\"{arr[1]}_{guid}\"";
            return (mainTableName, transientTableName);
        }

        private static (string, string[]) CreateMainTableName(string tableName)
        {
            var arr = tableName.Split(new string[] { "[", "]", "." }, StringSplitOptions.RemoveEmptyEntries);
            string mainTableName;
            if (arr.Length == 2)
                mainTableName = $"\"{arr[0]}\".\"{arr[1]}\"";
            else
                mainTableName = $"\"{arr[1]}\".\"{arr[2]}\"";

            return (mainTableName, arr);
        }

        private static (bool, string, string, string) FetchDataAndWriteFile(string sQLServerConnectionName, TableSyncMetadata metadata, SQLServerColumnMetaData[] columnMetaData, RDBConnectionManager connectionManager)
        {
            if (columnMetaData.Length > 0)
            {
                var selectColumns = new StringBuilder();
                foreach (var column in columnMetaData)
                    selectColumns.Append("[").Append(column.Name).Append("],");

                selectColumns = selectColumns.Remove(selectColumns.Length - 1, 1);

                var loadingTimeCheck = metadata.IsNoneToNow ? string.Empty : $"WHERE loading_time = @ld";

                var query = $@"DECLARE @ld DATETIME = CAST('{metadata.LoadingTime.ToString("yyyyMMdd HH:mm:ss.fffffff")}' AS DATETIME2)
                        SELECT {selectColumns}
                        FROM {metadata.TableName}
                        {loadingTimeCheck}";

                var (filePath, fileName, guid) = GetFilePath(sQLServerConnectionName);

                var hasRecords = SQLServerRepository.WriteFileForSQLQuery(sQLServerConnectionName, query, filePath, connectionManager);
                return (hasRecords, filePath, fileName, guid);
            }
            return (false, null, null, null);
        }

        private static (string, string, string) GetFilePath(string sQLServerConnectionName)
        {
            var guid = SQLServerRepository.GetGUID(sQLServerConnectionName);
            var directoryName = Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, "..\\SnowflakeFiles\\"));
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            var filePath = $"{directoryName}\\{guid}.csv";
            return (filePath, $"{guid}.csv", guid);
        }

        private static void CopyDataToTransientTable(string filePath, string fileName, string guid, string destinationTableName, string createTableStatement, SQLServerColumnMetaData[] columnMetaData, string[] keys)
        {
            SFConnection connection = null;

            try
            {
                connection = new SFConnection(false);

                string fileFormatName = $"FF_CSV_{guid}";
                string stageName = guid;

                string fileFormatCommand = SFRepository.BuildFileFormatStatement(fileFormatName, "\"", "");
                string createStageCommand = SFRepository.BuildCreateStageStatement(stageName, fileFormatName);

                string upload_enclosed_source = $@"file://{fileName}".Replace(@"\", @"/");
                string _upload = $@"PUT '{upload_enclosed_source}' @""{stageName}"";";

                string fileURL = $@"file://{filePath}".Replace(@"\", @"/");
                string putCommand = $@"PUT '{fileURL}' @""{stageName}"" auto_compress=false;";

                string mergeTableStatement = SFRepository.BuildInsertStatementFromStagingTable($"@\"{stageName}\"", destinationTableName, columnMetaData);

                connection.ExecuteNonQuery(new string[] { fileFormatCommand, createStageCommand, createTableStatement, putCommand, mergeTableStatement });
            }
            finally
            {
                connection?.Dispose();
            }
        }
    }
}
