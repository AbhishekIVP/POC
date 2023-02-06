using DWHAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace SnowflakeAdapter
{
    internal class SFRepository
    {
        public const string SCHEMA_NAME = "dimension";

        internal static string BuildCreateTableStatement(string tableName, bool isTransient, string[] clusteredKeyColumns, SQLServerColumnMetaData[] columnMetaData)
        {
            StringBuilder builder = new StringBuilder("CREATE ");

            if (isTransient)
                builder.Append("TRANSIENT ");

            builder
                .Append("TABLE ")
                .Append(tableName)
                .AppendLine(" (");

            foreach (SQLServerColumnMetaData column in columnMetaData)
            {
                builder
                    .AppendLine()
                    .Append("\"")
                    .Append(column.Name)
                    .Append("\" ");

                if (column.Datatype.Equals("bit", StringComparison.OrdinalIgnoreCase))
                    builder.Append("boolean");
                else
                {
                    var datatypeLower = column.Datatype.ToLower();
                    if (datatypeLower.Contains("varchar"))
                        builder.Append("varchar");
                    else if (datatypeLower.Contains("numeric") || datatypeLower.Contains("decimal"))
                        builder.Append("number");
                    else
                        builder.Append(column.Datatype);
                }
                builder.Append(",");
            }

            builder = builder.Remove(builder.Length - 1, 1);
            builder.Append(")");

            if (clusteredKeyColumns != null && clusteredKeyColumns.Length > 0)
            {
                builder
                    .Append(" CLUSTER BY (");

                foreach (var column in clusteredKeyColumns)
                    builder.Append("\"").Append(column).Append("\",");

                builder = builder.Remove(builder.Length - 1, 1);
                builder.Append(")");
            }

            return builder.ToString();
        }

        internal static string BuildMergeQueryStatement(string sourceTableName, string targetTableName, string[] joinColumns, SQLServerColumnMetaData[] columnMetaData)
        {
            var joinCondition = new StringBuilder();
            var setClause = new StringBuilder();
            var selectClause = new StringBuilder();
            var valuesClause = new StringBuilder();

            for (var counter = 0; counter < joinColumns.Length; counter++)
            {
                joinCondition.Append("target.\"").Append(joinColumns[counter]).Append("\" = source.\"").Append(joinColumns[counter]).Append("\"");
                if (counter < joinColumns.Length - 1)
                    joinCondition.Append(" AND ");
            }

            for (var counter = 0; counter < columnMetaData.Length; counter++)
            {
                setClause.Append("target.\"").Append(columnMetaData[counter].Name).Append("\" = source.\"").Append(columnMetaData[counter].Name).Append("\"");
                selectClause.Append("\"").Append(columnMetaData[counter].Name).Append("\"");
                valuesClause.Append("source.\"").Append(columnMetaData[counter].Name).Append("\"");

                if (counter < columnMetaData.Length - 1)
                {
                    setClause.Append(",");
                    selectClause.Append(",");
                    valuesClause.Append(",");
                }
            }

            var query = $@"MERGE INTO {targetTableName} AS target USING {sourceTableName} AS source ON {joinCondition.ToString()}
                  WHEN MATCHED THEN UPDATE SET {setClause.ToString()}
                  WHEN NOT MATCHED THEN INSERT ({selectClause.ToString()}) VALUES ({valuesClause.ToString()});";

            return query;
        }

        internal static string BuildDeleteStatement(string sourceTableName, string targetTableName, string[] joinColumns, string[][] disparateJoinColumns, string[] whereClause)
        {
            var joinCondition = new StringBuilder();

            if (joinColumns != null && joinColumns.Length > 0)
            {
                for (var counter = 0; counter < joinColumns.Length; counter++)
                {
                    joinCondition.Append("source.\"").Append(joinColumns[counter]).Append("\" = target.\"").Append(joinColumns[counter]).Append("\"");
                    if (counter < joinColumns.Length - 1)
                        joinCondition.Append(" AND ");
                }
            }

            if (disparateJoinColumns != null && disparateJoinColumns.Length > 0)
            {
                for (var counter = 0; counter < disparateJoinColumns.Length; counter++)
                {
                    joinCondition.Append("source.\"").Append(disparateJoinColumns[counter][0]).Append("\" = target.\"").Append(disparateJoinColumns[counter][1]).Append("\"");
                    if (counter < disparateJoinColumns.Length - 1)
                        joinCondition.Append(" AND ");
                }
            }

            if (whereClause != null && whereClause.Length > 0)
            {
                joinCondition.Append(" AND ");

                for (var counter = 0; counter < whereClause.Length; counter++)
                {
                    joinCondition.Append(whereClause[counter]);
                    if (counter < whereClause.Length - 1)
                        joinCondition.Append(" AND ");
                }
            }

            var query = $@"DELETE FROM {targetTableName} AS target
                USING {sourceTableName} AS source 
                WHERE {joinCondition};";

            return query;
        }

        internal static string BuildInsertStatement(string sourceTableName, string targetTableName, SQLServerColumnMetaData[] columnMetaData)
        {
            var selectClause = new StringBuilder();

            for (var counter = 0; counter < columnMetaData.Length; counter++)
            {
                selectClause.Append("\"").Append(columnMetaData[counter].Name).Append("\"");

                if (counter < columnMetaData.Length - 1)
                    selectClause.Append(",");
            }

            var query = $@"INSERT INTO {targetTableName} ({selectClause.ToString()}) 
                SELECT {selectClause.ToString()}
                FROM {sourceTableName};";

            return query;
        }

        internal static string BuildInsertStatementFromStagingTable(string sourceTableName, string targetTableName, SQLServerColumnMetaData[] columnMetaData)
        {
            var selectClause = new StringBuilder();
            var indexedSelectClause = new StringBuilder();

            for (var counter = 0; counter < columnMetaData.Length; counter++)
            {
                selectClause.Append("\"").Append(columnMetaData[counter].Name).Append("\"");
                indexedSelectClause.Append("$").Append(counter + 1).Append(" AS \"").Append(columnMetaData[counter].Name).Append("\"");

                if (counter < columnMetaData.Length - 1)
                {
                    selectClause.Append(",");
                    indexedSelectClause.Append(",");
                }
            }

            var query = $@"INSERT INTO {targetTableName} ({selectClause}) 
                SELECT {indexedSelectClause}
                FROM {sourceTableName};";

            return query;
        }

        internal static string BuildAlterTableStatementFromSQLServerStatement(string sqlServerStatement)
        {
            StringBuilder sb = new StringBuilder();

            var tempStatement = sqlServerStatement.Replace("[", "\"").Replace("]", "\"");

            var arr = tempStatement.ToCharArray();
            var newArr = new StringBuilder();

            bool isOpenQuotes = false;
            for (var c = 0; c < arr.Length; c++)
            {
                if (arr[c] == '\"')
                    isOpenQuotes = !isOpenQuotes;

                if (isOpenQuotes && arr[c] == ' ')
                    newArr.Append("žŸž");
                else
                    newArr.Append(arr[c]);
            }

            var splittedString = newArr.ToString().Split(new string[] { " ", "(", ")" }, StringSplitOptions.RemoveEmptyEntries);

            bool isColumnNameNext = false;
            bool isDatatypeNext = false;
            bool ignoreRemaining = false;
            foreach (var str in splittedString)
            {
                if (ignoreRemaining)
                    continue;

                if (str.ToLower() == "add" || str.ToLower() == "column")
                    isColumnNameNext = true;
                else
                {
                    if (isColumnNameNext)
                    {
                        isColumnNameNext = false;
                        isDatatypeNext = true;
                    }
                    else if (isDatatypeNext)
                    {
                        isDatatypeNext = false;
                        if (str.Equals("varchar", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("varchar");
                            ignoreRemaining = true;
                        }
                        else if (str.Equals("numeric", StringComparison.OrdinalIgnoreCase) || str.Equals("decimal", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("number");
                            ignoreRemaining = true;
                        }
                        else if (str.Equals("bit", StringComparison.OrdinalIgnoreCase))
                        {
                            sb.Append("boolean").Append(" ");
                        }
                        else
                            sb.Append(str).Append(" ");

                        continue;
                    }
                }
                sb.Append(str).Append(" ");
            }

            sb.Replace("žŸž", " ");

            return sb.ToString();
        }

        internal static string BuildCreateViewStatementFromSQLServerStatement(string sqlServerStatement)
        {
            var viewStatement = sqlServerStatement.Replace("CREATE VIEW", "CREATE OR REPLACE VIEW");
            return viewStatement
                .Replace("[dimension]", "\"dimension\"")
                .Replace('[', '\"')
                .Replace(']', '\"');
        }

        internal static string BuildDropTableStatement(string tableName)
        {
            var query = $@"DROP TABLE IF EXISTS {tableName};";

            return query;
        }

        internal static string BuildCreateStageStatement(string stageName, string fileFormatName)
        {
            return $@"CREATE STAGE ""{stageName}""
            FILE_FORMAT='""{fileFormatName}""';";
        }

        internal static string BuildFileFormatStatement(string fileFormatName, string pairedEscape, string singleEscape)
        {
            pairedEscape = singleEscape.Equals(pairedEscape, StringComparison.OrdinalIgnoreCase) ? "NONE" : pairedEscape;
            return $@"CREATE FILE FORMAT ""{fileFormatName}""
                TYPE = CSV
                RECORD_DELIMITER = '{Environment.NewLine}'
                NULL_IF = ('NULL', 'null') 
                ESCAPE = {(!string.IsNullOrEmpty(singleEscape) ? "'" + singleEscape + "'" : "'NONE'")} 
                FIELD_DELIMITER = ','
                FIELD_OPTIONALLY_ENCLOSED_BY = '{pairedEscape}'
                SKIP_HEADER = 1
                COMPRESSION = GZIP
                EMPTY_FIELD_AS_NULL = TRUE";
        }

        internal static string BuildCopyTableStatement(string destinationTableName, string stagingTableName, string fileFormatName, string fileName)
        {
            return $@"COPY INTO {destinationTableName}
              from '@{stagingTableName}'
              file_format = (format_name = '{fileFormatName}' , error_on_column_count_mismatch=false)
              pattern = '{fileName}'
              on_error = 'skip_file';";
        }

        internal static string BuildRenameTableStatement(string destinationTableName, string stagingTableName)
        {
            return $@"ALTER TABLE IF EXISTS {stagingTableName} RENAME TO {destinationTableName}";
        }
    }
}
