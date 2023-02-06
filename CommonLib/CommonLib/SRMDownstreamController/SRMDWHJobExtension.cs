using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace com.ivp.common.srmdwhjob
{
    public class SRMDWHJobExtension
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMDWHJobExtension");
        static string batchSizeConfig = Convert.ToString(RADConfigReader.GetConfigAppSettings("SRMDWHBulkUploadBatchSize"));
        public static DataSet ExecuteQuery(string connectionName, string query, SRMDBQueryType queryType)
        {
            query = DALWrapperAppend.Replace(query);

            DataSet ds = new DataSet();
            RDBConnectionManager con = RDALAbstractFactory.DBFactory.GetConnectionManager(connectionName);

            try
            {
                RQueryType radQueryType = (RQueryType)Enum.Parse(typeof(RQueryType), queryType.ToString(), true);
                ds = con.ExecuteQuery(query, radQueryType);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(con);
            }

            return ds;
        }

        public static DataSet ExecuteQuery(RDBConnectionManager con, string query, SRMDBQueryType queryType)
        {
            query = DALWrapperAppend.Replace(query);
            DataSet ds = new DataSet();

            try
            {
                RQueryType radQueryType = (RQueryType)Enum.Parse(typeof(RQueryType), queryType.ToString(), true);
                ds = con.ExecuteQuery(query, radQueryType);
            }
            catch (Exception ex)
            {
                throw;
            }
            return ds;
        }

        public static ObjectSet ExecuteQueryObject(RDBConnectionManager con, string query, SRMDBQueryType queryType)
        {
            query = DALWrapperAppend.Replace(query);
            ObjectSet ds = new ObjectSet();

            try
            {
                RQueryType radQueryType = (RQueryType)Enum.Parse(typeof(RQueryType), queryType.ToString(), true);
                ds = con.ExecuteQueryObject(query, radQueryType);
            }
            catch (Exception ex)
            {
                throw;
            }
            return ds;
        }

        public static ObjectSet ExecuteQueryObject(string connectionName, string query, SRMDBQueryType queryType)
        {
            query = DALWrapperAppend.Replace(query);

            ObjectSet ds = new ObjectSet();
            RDBConnectionManager con = RDALAbstractFactory.DBFactory.GetConnectionManager(connectionName);

            try
            {
                RQueryType radQueryType = (RQueryType)Enum.Parse(typeof(RQueryType), queryType.ToString(), true);
                ds = con.ExecuteQueryObject(query, radQueryType);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(con);
            }

            return ds;
        }

        public static ObjectTable ConvertDataTableToObjectTable(DataTable dt, int start, int end)
        {
            try
            {
                ObjectTable objTable = new ObjectTable();

                //Copy Table Name
                objTable.TableName = dt.TableName;

                //Copy Schema
                foreach (DataColumn dc in dt.Columns)
                {
                    ObjectColumn objColumn = new ObjectColumn(dc.ColumnName, dc.DataType);
                    objTable.Columns.Add(objColumn);
                }

                List<DataRow> lstRowsToDelete = new List<DataRow>();
                //Copy Data
                for (var i = start; i < end; i++)
                {
                    DataRow dr = dt.Rows[0];
                    ObjectRow objRow = objTable.NewRow();

                    foreach (DataColumn dc in dt.Columns)
                    {
                        objRow[dc.ColumnName] = dr[dc.ColumnName];
                    }

                    objTable.Rows.Add(objRow);
                    dt.Rows.Remove(dr);
                }
                return objTable;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void ExecuteBulkCopy(string connectionName, string tableName, DataTable dtTable)
        {
            tableName = DALWrapperAppend.Replace(tableName);

            RDBConnectionManager mDbConn = null;

            try
            {
                var columnMapping = new Dictionary<string, string>();

                var dataTableColumnNames = dtTable.Columns.Cast<DataColumn>().ToDictionary(x => x.ColumnName, y => y.ColumnName, StringComparer.OrdinalIgnoreCase);

                var query = string.Format(@"SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = PARSENAME('{0}', 1) AND TABLE_SCHEMA = PARSENAME('{0}', 2)", tableName);

                var sqlTableColumnNames = ExecuteQueryObject(connectionName, query, SRMDBQueryType.SELECT).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["COLUMN_NAME"]), y => Convert.ToString(y["COLUMN_NAME"]));
                foreach (var column in sqlTableColumnNames)
                {
                    if (dataTableColumnNames.ContainsKey(column.Key))
                        columnMapping.Add(dataTableColumnNames[column.Key], column.Key);
                }

                int batchSize = 5000;
                if (!string.IsNullOrEmpty(batchSizeConfig))
                    batchSize = Convert.ToInt32(batchSizeConfig);

                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(connectionName);
                mDbConn.ExecuteBulkCopy(tableName, dtTable, batchSize, columnMapping);
            }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw;
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
            }
        }

        public static void ExecuteBulkCopyObject(string connectionName, string tableName, ObjectTable dtTable, Dictionary<string, string> columnMappings = null)
        {
            tableName = DALWrapperAppend.Replace(tableName);

            RDBConnectionManager mDbConn = null;

            try
            {
                if (columnMappings == null)
                {
                    columnMappings = new Dictionary<string, string>();

                    var dataTableColumnNames = dtTable.Columns.ToDictionary(x => x.ColumnName, y => y.ColumnName, StringComparer.OrdinalIgnoreCase);

                    var query = string.Format(@"SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = PARSENAME('{0}', 1) AND TABLE_SCHEMA = PARSENAME('{0}', 2)", tableName);

                    var sqlTableColumnNames = ExecuteQueryObject(connectionName, query, SRMDBQueryType.SELECT).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["COLUMN_NAME"]), y => Convert.ToString(y["COLUMN_NAME"]));
                    foreach (var column in sqlTableColumnNames)
                    {
                        if (dataTableColumnNames.ContainsKey(column.Key))
                            columnMappings.Add(dataTableColumnNames[column.Key], column.Key);
                    }
                }
                int batchSize = 5000;
                if (!string.IsNullOrEmpty(batchSizeConfig))
                    batchSize = Convert.ToInt32(batchSizeConfig);

                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(connectionName);
                mDbConn.ExecuteBulkCopyObject(tableName, dtTable, true, batchSize, columnMappings);
            }
            catch (Exception ex)
            {
                mLogger.Error("Error :: " + ex.ToString());
                LogBulkCopyTableDataForFailure(dtTable);
                throw;
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
            }
        }

        public static void ExecuteBulkCopyObject(RDBConnectionManager con, string tableName, ObjectTable dtTable, Dictionary<string, string> columnMappings = null)
        {
            tableName = DALWrapperAppend.Replace(tableName);

            try
            {
                if (columnMappings == null)
                {
                    columnMappings = new Dictionary<string, string>();

                    var dataTableColumnNames = dtTable.Columns.ToDictionary(x => x.ColumnName, y => y.ColumnName, StringComparer.OrdinalIgnoreCase);

                    var query = string.Format(@"SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = PARSENAME('{0}', 1) AND TABLE_SCHEMA = PARSENAME('{0}', 2)", tableName);

                    var sqlTableColumnNames = ExecuteQueryObject(con, query, SRMDBQueryType.SELECT).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["COLUMN_NAME"]), y => Convert.ToString(y["COLUMN_NAME"]));
                    foreach (var column in sqlTableColumnNames)
                    {
                        if (dataTableColumnNames.ContainsKey(column.Key))
                            columnMappings.Add(dataTableColumnNames[column.Key], column.Key);
                    }
                }

                int batchSize = 5000;
                if (!string.IsNullOrEmpty(batchSizeConfig))
                    batchSize = Convert.ToInt32(batchSizeConfig);

                con.ExecuteBulkCopyObject(tableName, dtTable, true, batchSize, columnMappings);
            }
            catch (Exception ex)
            {
                mLogger.Error("Error :: " + ex.ToString());
                LogBulkCopyTableDataForFailure(dtTable);
                throw;
            }
        }

        static void LogBulkCopyTableDataForFailure(ObjectTable dtTable)
        {
            using (StringWriter writer = new StringWriter())
            {
                try
                {
                    var ddt = dtTable.ConvertToDataTable();
                    if (string.IsNullOrEmpty(ddt.TableName))
                        ddt.TableName = "Bulk Copy Table Data";

                    ddt.WriteXml(writer, XmlWriteMode.WriteSchema);
                    mLogger.Error(writer.ToString());
                }
                catch (Exception ex)
                {
                    mLogger.Error("Exception : " + ex.ToString());
                }
                finally
                {
                    try
                    {
                        writer.Close();
                    }
                    catch { }
                }
            }
        }

        public static RDBConnectionManager GetConnectionManager(string connectionName, bool useTransaction, IsolationLevel isolationLevel)
        {
            mLogger.Debug("Start-> GetConnectionManager");
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(connectionName);

                if (useTransaction)
                {
                    mDbConn.UseTransaction = true;
                    mDbConn.IsolationLevel = isolationLevel;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("End-> GetConnectionManager");
            }
            return mDbConn;
        }

        public static string GetConnectionString(string connectionName)
        {
            mLogger.Debug("Start-> GetConnectionString");
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(connectionName);
                return mDbConn.ConnectionString;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End-> GetConnectionString");
            }
        }
    }

    public enum SRMDBQueryType
    {
        SELECT = 0, INSERT, DELETE
    }

}
