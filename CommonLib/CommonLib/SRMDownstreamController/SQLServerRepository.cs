using com.ivp.common.srmdwhjob;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DWHAdapter;
using com.ivp.rad.common;
using com.ivp.rad.dal;

namespace DWHAdapter
{
    public class SQLServerRepository
    {
        static IRLogger logger = RLogFactory.CreateLogger("SQLServerRepository");
        public static SQLServerColumnMetaData[] GetTableSchema(string sqlConnectionName, string tableName)
        {
            string query = $@"SELECT COLUMN_NAME, 
                CASE WHEN DATA_TYPE = 'VARCHAR' THEN 'VARCHAR(' + CASE WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN 'MAX' ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR(MAX)) END + ')'
	                WHEN DATA_TYPE IN ('NUMERIC','DECIMAL') THEN DATA_TYPE + '(' + CAST(NUMERIC_PRECISION AS VARCHAR(MAX)) + ',' + CAST(NUMERIC_SCALE AS VARCHAR(MAX)) + ')'
	                ELSE DATA_TYPE
	                END AS DATA_TYPE
                FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = PARSENAME('{tableName}',1) AND TABLE_SCHEMA = 'dimension'";

            var schema = SRMDWHJobExtension.ExecuteQuery(sqlConnectionName, query, SRMDBQueryType.SELECT).Tables[0];

            var schemaMetadata = new SQLServerColumnMetaData[schema.Rows.Count];
            for (var counter = 0; counter < schema.Rows.Count; counter++)
            {
                var row = schema.Rows[counter];
                schemaMetadata[counter] = new SQLServerColumnMetaData(row.Field<string>("COLUMN_NAME"), row.Field<string>("DATA_TYPE"));
            }

            return schemaMetadata;
        }

        public static MasterTableMetaData[] GetMasterTableNames(string sqlConnectionName)
        {
            string query = $@"SELECT DISTINCT PARSENAME(surrogate_table_name,1) AS [master_table], isRefM
                FROM dimension.ivp_srm_dwh_tables_for_loading l
				INNER JOIN INFORMATION_SCHEMA.TABLES t ON PARSENAME(l.surrogate_table_name,1) = t.TABLE_NAME AND t.TABLE_SCHEMA = 'dimension'
                WHERE is_active = 1";

            var dt = SRMDWHJobExtension.ExecuteQuery(sqlConnectionName, query, SRMDBQueryType.SELECT).Tables[0];

            var masterTables = new MasterTableMetaData[dt.Rows.Count];
            for (var counter = 0; counter < dt.Rows.Count; counter++)
            {
                var row = dt.Rows[counter];
                masterTables[counter] = new MasterTableMetaData($"[dimension].[{row.Field<string>("master_table")}]", row.Field<bool>("isRefM"), true, DateTime.Now);
            }

            return masterTables;
        }

        public static string GetViewDefinition(RDBConnectionManager connectionManager, string viewDefinitionStatement)
        {
            var viewNameStartIndex = viewDefinitionStatement.IndexOf("ME  = '");

            var viewNameTemp = viewDefinitionStatement.Substring(viewNameStartIndex + 7);
            var viewNameEndIndex = viewNameTemp.IndexOf("'");
            var viewName = viewNameTemp.Substring(0, viewNameEndIndex);

            string query = $"SELECT OBJECT_DEFINITION (OBJECT_ID('dimension.{viewName}')) AS viewDefinition";

            return SRMDWHJobExtension.ExecuteQuery(connectionManager, query, SRMDBQueryType.SELECT).Tables[0].Rows[0].Field<string>(0);
        }

        public static string GetGUID(string sqlConnectionName)
        {
            string query = "SELECT NEWID()";

            return SRMDWHJobExtension.ExecuteQuery(sqlConnectionName, query, SRMDBQueryType.SELECT).Tables[0].Rows[0].Field<Guid>(0).ToString().Replace("-", "_");
        }

        public static bool WriteFileForSQLQuery(string sqlConnectionName, string query, string outputFilePath, RDBConnectionManager connectionManager)
        {
            try
            {
                logger.Debug("WriteFileForSQLQuery -> START");
                logger.Debug($"WriteFileForSQLQuery -> Query -> {query}");

                if (File.Exists(outputFilePath))
                    File.Delete(outputFilePath);

                var hasRecordsToProcess = false;

                if (connectionManager is null)
                {
                    var sqlConnection = SRMDWHJobExtension.GetConnectionManager(sqlConnectionName, false, IsolationLevel.ReadCommitted);
                    try
                    {
                        hasRecordsToProcess = RunInConnection(query, outputFilePath, sqlConnection);
                    }
                    finally
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(sqlConnection);
                    }
                }
                else
                    hasRecordsToProcess = RunInConnection(query, outputFilePath, connectionManager);

                if (!hasRecordsToProcess)
                {
                    if (File.Exists(outputFilePath))
                        File.Delete(outputFilePath);
                }

                return hasRecordsToProcess;
            }
            finally
            {
                logger.Debug("WriteFileForSQLQuery -> END");
            }
        }

        private static bool RunInConnection(string query, string outputFilePath, RDBConnectionManager connectionManager)
        {
            bool hasRecordsToProcess = false;
            connectionManager.ExecuteSelectQuery(query, (reader) =>
            {
                using (var fs = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (GZipStream compressionStream = new GZipStream(fs, CompressionMode.Compress))
                    {
                        using (var writer = new StreamWriter(compressionStream))
                        {
                            var batchSizeToFlush = 10000;
                            var rowCounter = 0;

                            var schema = reader.GetSchemaTable();
                            var counter = schema.Rows.Count;
                            var totalColumns = counter - 1;

                            Dictionary<int, string> columnOrdinalVsDatatype = new Dictionary<int, string>();

                            foreach (DataRow column in schema.Rows)
                            {
                                counter--;

                                var columnName = column.Field<string>("ColumnName");
                                columnOrdinalVsDatatype.Add(column.Field<int>("ColumnOrdinal"), column.Field<string>("DataTypeName"));

                                writer.Write(columnName);

                                if (counter > 0)
                                    writer.Write(',');
                                else
                                    writer.Write(Environment.NewLine);
                            }
                            writer.Flush();

                            while (reader.Read())
                            {
                                hasRecordsToProcess = true;
                                for (var c = 0; c <= totalColumns; c++)
                                {
                                    if (!reader.IsDBNull(c))
                                    {
                                        writer.Write('\"');

                                        if (columnOrdinalVsDatatype[c].Equals("datetime", StringComparison.OrdinalIgnoreCase))
                                            writer.Write(reader.GetDateTime(c).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                        else if (columnOrdinalVsDatatype[c].Equals("date", StringComparison.OrdinalIgnoreCase))
                                            writer.Write(reader.GetDateTime(c).ToString("yyyy-MM-dd"));
                                        else if (columnOrdinalVsDatatype[c].Equals("varchar", StringComparison.OrdinalIgnoreCase))
                                            writer.Write(reader[c].ToString().Replace("\"", "\"\""));
                                        else
                                            writer.Write(reader[c].ToString());

                                        writer.Write('\"');
                                    }
                                    else
                                        writer.Write("null");

                                    if (c == totalColumns)
                                        writer.Write(Environment.NewLine);
                                    else
                                        writer.Write(',');
                                }

                                rowCounter++;

                                if (rowCounter == batchSizeToFlush)
                                {
                                    rowCounter = 0;
                                    writer.Flush();
                                }
                            }
                            writer.Flush();
                            writer.Close();
                        }
                    }
                    fs.Close();
                }
            });

            return hasRecordsToProcess;
        }
    }

    public struct SQLServerColumnMetaData
    {
        public string Name { get; private set; }
        public string Datatype { get; private set; }

        internal SQLServerColumnMetaData(string name, string datatype)
        {
            Name = name;
            Datatype = datatype;
        }
    }
}
