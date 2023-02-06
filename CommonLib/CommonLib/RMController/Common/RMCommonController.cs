using com.ivp.commom.commondal;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.rad.data;
using com.ivp.rad.RCommonTaskManager;
using System.Reflection;
using com.ivp.rad.common;
using System.Xml;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.controls.xruleeditor;
using com.ivp.rad.controls.xruleeditor.grammar;
using com.ivp.rad.xruleengine;
using com.ivp.srmcommon;
using System.Xml.Linq;
using com.ivp.common.lookupdatamassage;
using com.ivp.SRMCommonModules;
using System.IO;

namespace com.ivp.common
{
    public class RMCommonController
    {
        Dictionary<int, int> RMRuleTypeVsRADRuleType = new Dictionary<int, int>();

        static IRLogger mLogger = RLogFactory.CreateLogger("RMCommonController");
        public RDBConnectionManager OpenNewConnection(string connectionID, bool useTransaction = true)
        {
            RDBConnectionManager mDBCon = null;

            string mDBConnectionId = string.Empty;
            mDBConnectionId = RConfigReader.GetConfigAppSettings
                        (connectionID);
            mDBCon = RDALAbstractFactory.DBFactory.GetConnectionManager
                            (mDBConnectionId);
            mDBCon.UseTransaction = useTransaction;

            return mDBCon;
        }

        public void CommitTransaction(RDBConnectionManager mDBCon)
        {
            if (mDBCon != null)
                mDBCon.CommitTransaction();
        }

        public List<string> GetAttributeDataTypes()
        {
            return new RMCommonDBManager().GetAttributeDataTypes().Tables[0].AsEnumerable().Select(x => Convert.ToString(x["data_type_name"])).ToList();
        }

        public Dictionary<int, string> GetDateTypes(EMDateType dateType = EMDateType.Both)
        {
            return new RMCommonDBManager().GetDateTypes(dateType).Tables[0].AsEnumerable().ToDictionary(x => x.Field<int>("date_type_id"), y => y.Field<string>("date_display_name"));
        }


        public void AddColumnsToObjectTable(ObjectTable table, List<RMCommonColumnInfo> columns)
        {
            columns.ForEach(col =>
            {
                if (!table.Columns.Contains(col.ColumnName))
                {
                    ObjectColumn objCol = new ObjectColumn(col.ColumnName, col.DataType);
                    objCol.DefaultValue = col.DefaultValue;
                    table.Columns.Add(objCol);
                }

            });
        }

        public void RemoveColumnsFromObjectTable(ObjectTable table, List<string> columns)
        {
            columns.ForEach(col =>
            {

                if (table.Columns.Contains(col))
                {
                    table.Columns.Remove(col);
                }

            });
        }

        public void RMSaveTaskInCTM(string taskName, OperationType opType, RDBConnectionManager dbConnection)
        {
            Assembly RefMControllerAssembly = Assembly.Load("RefMController");
            Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMCTMController");
            MethodInfo GetEntityAudit = objType.GetMethod("SaveTaskInCTM", BindingFlags.Static | BindingFlags.Public);
            object obj = Activator.CreateInstance(objType);
            GetEntityAudit.Invoke(obj, new object[] { taskName, opType, dbConnection });
        }

        public List<RMColumnInfo> GetDefaultColumns(RMDynamicTableType tableType)
        {
            //if (INIT == false)
            Dictionary<RMDynamicTableType, List<RMColumnInfo>> columns = LoadStaticColumns();
            return columns[tableType];
        }

        private Dictionary<RMDynamicTableType, List<RMColumnInfo>> LoadStaticColumns()
        {
            mLogger.Debug("RMDynamicDBConfigLoader : LoadStaticColumns -> Start");
            Dictionary<RMDynamicTableType, List<RMColumnInfo>> staticColumnConfig =
            new Dictionary<RMDynamicTableType, List<RMColumnInfo>>();
            RMDynamicTableType nodeName;
            RMColumnInfo colInfo = null;
            string serverPath = string.Empty;
            string filePath = string.Empty;
            XmlDocument xmlDoc = null;
            List<RMColumnInfo> columnNodes = null;
            XmlNodeList nodeList = null;
            XmlNodeList xNodeList = null;
            try
            {
                xmlDoc = RConfigurationManager.GetConfigDocument("refmasterconfig", "RefM_DynamicDBConfig");
                nodeList = xmlDoc.ChildNodes[1].ChildNodes;
                foreach (XmlNode xnode in nodeList)
                {
                    switch (xnode.Name)
                    {
                        case "enitytype":
                            nodeName = RMDynamicTableType.EntityType;
                            break;
                        case "historyentitytype":
                            nodeName = RMDynamicTableType.HistoryEntityType;
                            break;
                        case "feed":
                            nodeName = RMDynamicTableType.Feed;
                            break;
                        default:
                            nodeName = RMDynamicTableType.EntityType;
                            break;
                    }

                    xNodeList = xnode.ChildNodes;
                    columnNodes = new List<RMColumnInfo>();
                    foreach (XmlNode node in xNodeList)
                    {
                        colInfo = new RMColumnInfo();
                        colInfo.ColumnName = node.Attributes["name"].Value;
                        colInfo.DisplayName = node.Attributes["name"].Value;
                        colInfo.Nulable = node.Attributes["nulable"].Value == "true" ? true : false;
                        switch (node.Attributes["datatype"].Value)
                        {
                            case "VARCHAR":
                                colInfo.DataType = RMDBDataTypes.VARCHAR;
                                break;
                            case "INT":
                                colInfo.DataType = RMDBDataTypes.INT;
                                break;
                            case "DECIMAL":
                                colInfo.DataType = RMDBDataTypes.DECIMAL;
                                break;
                            case "DATETIME":
                                colInfo.DataType = RMDBDataTypes.DATETIME;
                                break;
                            case "BIT":
                                colInfo.DataType = RMDBDataTypes.BIT;
                                break;
                            case "VARCHARMAX":
                                colInfo.DataType = RMDBDataTypes.VARCHARMAX;
                                break;
                        }
                        colInfo.Length = node.Attributes["length"].Value;
                        colInfo.DefaultValue = node.Attributes["defaultvalue"].Value;
                        colInfo.IsIdentity = node.Attributes["idenity"].Value == "true" ? true : false;
                        colInfo.InPrimaryKey = node.Attributes["inprimarykey"].Value == "true" ? true : false;
                        colInfo.IsUnique = node.Attributes["isunique"].Value == "true" ? true : false;
                        colInfo.Calculated = node.Attributes["calculated"].Value;
                        columnNodes.Add(colInfo);
                    }
                    staticColumnConfig[nodeName] = columnNodes;
                }

            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                xmlDoc = null;
                nodeList = null;
                xNodeList = null;
                mLogger.Debug("RMDynamicDBConfigLoader : LoadStaticColumns -> End");
            }
            return staticColumnConfig;
        }


        public string AlterTable(string database, string tableName, List<RMColumnInfo> columns, DataSet objectInfo)
        {
            mLogger.Debug("RMDynamicDB : AlterTable -> Start");
            if (columns == null || columns.Count == 0)
                return null;

            StringBuilder sql = new StringBuilder();
            sql.Append("USE " + database + ";");
            StringBuilder indexSql = new StringBuilder();
            string cols = string.Empty;
            StringBuilder column = new StringBuilder();
            StringBuilder constraints = new StringBuilder();
            StringBuilder pk_constraints = new StringBuilder();

            List<string> primaryColNames = null;
            List<string> uniqueColNames = null;
            List<string> staticColumns = new List<string>() { "id", "entity_code", "last_modified_by", "loading_time", "is_active", "is_latest", "instance_id", "knowledge_date", "created_by", "is_deleted" };
            try
            {
                primaryColNames = new List<string>();
                uniqueColNames = new List<string>();

                foreach (RMColumnInfo cInfo in columns)
                {
                    cols = new RMCommonDBManager().GetColumn(cInfo);
                    if (cInfo.DataType == RMDBDataTypes.LOOKUP)
                    {
                        if (objectInfo.Tables.Count > 2 && objectInfo.Tables[2].Rows.Count != 0)
                        {
                            string indexName = "IX_" + tableName + "_" + cInfo.ColumnName;
                            bool cindex = objectInfo.Tables[2].Select().ToList().Exists(row => row["index_name"].ToString().ToUpper() == indexName.ToUpper());

                            if (!cindex && !cInfo.IsInternalLookup)
                                indexSql.Append("CREATE NONCLUSTERED INDEX [" + indexName + "]  ON [dbo].[" + tableName + "] ([" + cInfo.ColumnName + "]);");
                        }
                        if (cInfo.IsInternalLookup)
                        {
                            string lookupindexName = "IX_" + tableName;
                            bool lookupindex = objectInfo.Tables[2].Select().ToList().Exists(row => row["index_name"].ToString().ToUpper() == lookupindexName.ToUpper());

                            if (!lookupindex)
                            {
                                if (database.Equals("IVPRefMaster"))
                                {
                                    indexSql.Append("CREATE CLUSTERED INDEX [" + lookupindexName + "]  ON [dbo].[" + tableName + "] (" + cInfo.ColumnName + " ASC) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF);");
                                }
                                else if (database.Equals("IVPRefMaster_Archive"))
                                {
                                    indexSql.Append("CREATE CLUSTERED INDEX [" + lookupindexName + "]  ON [dbo].[" + tableName + "] (" + cInfo.ColumnName + " ASC, [loading_time] DESC,[knowledge_date] DESC) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF);");
                                }
                            }

                            if (database.Equals("IVPRefMaster_Archive"))
                            {
                                string indexName = "IX_" + tableName + "_" + cInfo.ColumnName;
                                bool cindex = objectInfo.Tables[2].Select().ToList().Exists(row => row["index_name"].ToString().ToUpper() == indexName.ToUpper());

                                if (!cindex)
                                {
                                    indexSql.Append("CREATE NONCLUSTERED INDEX [" + indexName + "]  ON [dbo].[" + tableName + "] ([" + cInfo.ColumnName + "]);");
                                }
                            }
                        }
                    }

                    if (column.Length > 0)
                        column.Append(", ").Append(cols);
                    else
                        column.Append(cols);

                    if (cInfo.IsIdentity)
                    {
                        column.Append(" IDENTITY(1,1)");
                    }
                    if (cInfo.Calculated != null && cInfo.Calculated != "")
                    {
                    }
                    if (cInfo.Calculated == null || cInfo.Calculated == "")
                    {
                    }
                    if (cInfo.InPrimaryKey && !cInfo.IsExisting)
                    {
                        primaryColNames.Add(cInfo.ColumnName);
                    }
                    else
                    {
                        if ((staticColumns.Contains(cInfo.ColumnName) && (!cInfo.Nulable)) || (!cInfo.Nulable && (database.ToLower() != "ivprefmaster_archive")))
                        {
                            column.Append(" NOT NULL");
                        }
                        else
                        {
                            column.Append(" NULL ");
                        }
                        if (cInfo.IsUnique)
                        {
                            //column.Append(" UNIQUE ");
                        }
                    }
                    if (!cInfo.IsExisting)
                        sql.Append(" ALTER TABLE " + tableName + " ADD " + column + "  ");
                    column.Clear();

                }
                if (database.Equals("IVPRefMaster_Archive"))
                {
                    if (objectInfo.Tables.Count > 2 && objectInfo.Tables[2].Rows.Count != 0)
                    {
                        bool kd = false, lt = false, ec = false;
                        string index_ec = "IX_" + tableName + "_EC";
                        string index_kd = "IX_" + tableName + "_KD";
                        string index_lt = "IX_" + tableName + "_LT";

                        ec = objectInfo.Tables[2].Select().ToList().Exists(row => row["index_name"].ToString().ToUpper() == index_ec.ToUpper());
                        kd = objectInfo.Tables[2].Select().ToList().Exists(row => row["index_name"].ToString().ToUpper() == index_kd.ToUpper());
                        lt = objectInfo.Tables[2].Select().ToList().Exists(row => row["index_name"].ToString().ToUpper() == index_lt.ToUpper());

                        if (!ec)
                            indexSql.Append("CREATE NONCLUSTERED INDEX [" + index_ec + "] ON [" + tableName + "] (entity_code); ");

                        if (!kd)
                            indexSql.Append("CREATE NONCLUSTERED INDEX [" + index_kd + "] ON [" + tableName + "] (knowledge_date); ");

                        if (!lt)
                            indexSql.Append("CREATE NONCLUSTERED INDEX [" + index_lt + "] ON [" + tableName + "] (loading_time); ");
                    }
                }
                if (pk_constraints.Length > 5)
                    column.Append(", " + pk_constraints);

                sql.Append("  " + column + "  ");

                if (constraints.Length > 5)
                {
                    sql.Append(" " + constraints);
                    constraints.Clear();
                }
                sql.Append(indexSql);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMDynamicDB : AlterTable -> End");
            }
            return sql.ToString();
        }

        public string CreateTable(string database, string tableName, List<RMColumnInfo> columns, DataSet objectInfo)
        {
            mLogger.Debug("RMDynamicDB : CreateTable -> Start");

            if (columns == null || columns.Count == 0)
                return null;

            StringBuilder sql = new StringBuilder();
            sql.Append("USE " + database + ";");
            StringBuilder indexSql = new StringBuilder();
            string cols = string.Empty;
            StringBuilder column = new StringBuilder();
            List<string> primaryColNames = null;
            List<string> uniqueColNames = null;
            bool createPrimaryKey = true;
            List<string> staticColumns = new List<string>() { "id", "entity_code", "last_modified_by", "loading_time", "is_active", "is_latest", "instance_id", "knowledge_date", "created_by", "is_deleted" };
            try
            {
                primaryColNames = new List<string>();
                uniqueColNames = new List<string>();
                StringBuilder constraints = new StringBuilder();
                StringBuilder pk_constraints = new StringBuilder();

                //Add various columns to the table. 
                foreach (RMColumnInfo cInfo in columns)
                {
                    cols = new RMCommonDBManager().GetColumn(cInfo);
                    if (column.Length > 0)
                        column.Append(", ").Append(cols);
                    else
                        column.Append(cols);
                    //Set Default Constraints for staticColumns  updated                     
                    if ((staticColumns.Contains(cInfo.ColumnName) || database.ToLower().Contains("ivprefmaster_archive") || database.Contains("Vendor")) && !string.IsNullOrWhiteSpace(cInfo.DefaultValue))
                    {
                        constraints.Append(" ALTER TABLE " + tableName + " ADD ");
                        constraints.AppendLine(" CONSTRAINT DF" + tableName + cInfo.ColumnName + " DEFAULT '" + cInfo.DefaultValue + "' FOR [" + cInfo.ColumnName + "]");
                    }
                    if (cInfo.IsIdentity)
                    {
                        column.Append(" IDENTITY(1,1)");
                    }
                    if (cInfo.InPrimaryKey)
                    {
                        primaryColNames.Add(cInfo.ColumnName);
                    }
                    else
                    {
                        if ((staticColumns.Contains(cInfo.ColumnName) && (!cInfo.Nulable)) || (!cInfo.Nulable && (database.ToLower() != "ivprefmaster_archive")))
                        {
                            column.Append(" NOT NULL");
                        }
                        else
                        {
                            column.Append(" NULL ");
                        }
                        if (cInfo.IsUnique)
                        {
                            //column.Append(" UNIQUE ");
                        }
                    }

                    if (cInfo.Calculated != null && cInfo.Calculated != "")
                    {

                    }
                    if (cInfo.Calculated == null || cInfo.Calculated == "")
                    {

                    }
                    //CREATE NONCLUSTERED INDEX on columns 
                    if (cInfo.DataType == RMDBDataTypes.LOOKUP && !cInfo.IsInternalLookup)
                        indexSql.Append("CREATE NONCLUSTERED INDEX [IX_" + tableName + "_" + cInfo.ColumnName + "]  ON [dbo].[" + tableName + "] ([" + cInfo.ColumnName + "]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF);");
                    if (cInfo.IsInternalLookup)
                    {
                        createPrimaryKey = false;
                        if (database.Equals("IVPRefMaster"))
                        {
                            indexSql.Append("CREATE CLUSTERED INDEX [IX_" + tableName + "]  ON [dbo].[" + tableName + "] (" + cInfo.ColumnName + " ASC) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF);");
                        }
                        else if (database.Equals("IVPRefMaster_Archive"))
                        {
                            indexSql.Append("CREATE NONCLUSTERED INDEX [IX_" + tableName + "_" + cInfo.ColumnName + "]  ON [dbo].[" + tableName + "] (" + cInfo.ColumnName + ") WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF);");
                            indexSql.Append("CREATE CLUSTERED INDEX [IX_" + tableName + "]  ON [dbo].[" + tableName + "] (" + cInfo.ColumnName + " ASC, [loading_time] DESC,[knowledge_date] DESC) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF);");
                        }
                    }
                }
                if (primaryColNames.Count > 0)
                {

                    foreach (string name in primaryColNames)
                    {
                        indexSql.Append("CREATE NONCLUSTERED INDEX [IX_" + tableName + "_" + name + "]  ON [dbo].[" + tableName + "] ([" + name + "]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF);");
                    }

                    //End Vikas Bhat, 16-Dec-2016
                }

                if (database.Equals("IVPRefMaster") && createPrimaryKey)
                {
                    pk_constraints.Append(" CONSTRAINT  [PK_" + tableName + "] PRIMARY KEY CLUSTERED ([entity_code]) ");
                }
                else if (database.Equals("IVPRefMaster_Archive") && createPrimaryKey)
                {
                    pk_constraints.Append(" CONSTRAINT  [PK_" + tableName + "] PRIMARY KEY CLUSTERED ([entity_code] ASC, [loading_time] DESC,[knowledge_date] DESC) ");
                }

                //Create NonClustered Index on KD & LT in Archive Database Entity Tables
                if (database.Equals("IVPRefMaster_Archive"))
                {
                    indexSql.Append("CREATE NONCLUSTERED INDEX [IX_" + tableName + "_EC] ON [dbo].[" + tableName + "] (entity_code) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF);");
                    indexSql.Append("CREATE NONCLUSTERED INDEX [IX_" + tableName + "_KD] ON [dbo].[" + tableName + "] (knowledge_date) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF);");
                    indexSql.Append("CREATE NONCLUSTERED INDEX [IX_" + tableName + "_LT] ON [dbo].[" + tableName + "] (loading_time) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF);");
                }
                column.Append(", " + pk_constraints);
                sql.Append("CREATE TABLE " + tableName + " ( " + column + " ) ");
                if (constraints.Length > 5)
                {
                    sql.Append(" " + constraints);
                    constraints.Clear();
                }
                sql.Append(indexSql);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());

            }
            finally
            {
                mLogger.Debug("RMDynamicDB : CreateTable -> End");
            }
            return sql.ToString();
        }

        public void RMDeleteRulesByRuleID(List<int> ruleIds, RDBConnectionManager mDBCon = null)
        {
            try
            {
                //string queryText = string.Format(@"
                //                DELETE map
                //                FROM IVPRefMasterVendor.dbo.ivp_refm_rule_mapping_details map
                //                INNER JOIN IVPRefMaster.dbo.REFM_GetList2Table('{0}', ',') tab
                //                 ON (tab.item = map.rule_id)

                //                 DELETE * FROM IVPRefMasterVendor.dbo.ivp_refm_rule_mapping WHERE rad_rule_set_id IN(
                //                 SELECT map.rad_rule_set_id FROM IVPRefMasterVendor.dbo.ivp_refm_rule_mapping map
                //                 INNER JOIN IVPRefMasterVendor.dbo.ivp_rad_xrule rul
                //                 ON(map.rad_rule_set_id = rul.rule_set_id AND map.is_active = 1 AND rul.is_active = 1)
                //                 INNER JOIN IVPRefMaster.dbo.REFM_GetList2Table('{0}', ',') tab
                //                 ON(tab.item = rul.rule_id))",string.Join(",",ruleIds));

                //if(mDBCon == null)
                //    CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMasterVendor_Connection);
                //else
                //    CommonDALWrapper.ExecuteSelectQuery(queryText, mDBCon);

                RXRuleController objRXRuleController = new RXRuleController();

                ruleIds.ForEach(ruleID =>
                {
                    if (mDBCon == null)
                        objRXRuleController.DeleteRule(ruleID);
                    else
                        objRXRuleController.DeleteRule(ruleID, mDBCon);
                });

                //Delete from Refm table
            }
            catch
            {
                throw;
            }
        }

        public int RMSaveRule(int dependentID, int attributeID, int ruleTypeID, string ruleName, int priority, string ruleText, bool ruleState, int ruleId, int ruleSetId, string userName, string connectionId, ref RADXRuleGrammarInfo ruleGrammarInfo, RDBConnectionManager mDBCon = null)
        {
            string originalDBName = string.Empty;
            try
            {
                RXRuleType ruleType = GetRXRuleTypeByID(ruleTypeID);
                bool isSuccess = true;
                string errorMsg = string.Empty;

                if (ruleTypeID == 5) // entity type feed transformation rule
                    dependentID = attributeID;

                if (ruleGrammarInfo == null)
                    ruleGrammarInfo = PrepareRuleGrammarInfo(dependentID, attributeID, ruleTypeID, mDBCon);

                if (ruleTypeID <= 5)//feed rules
                    dependentID = attributeID;

                else if (ruleTypeID == 10) //Group Validation Rule
                    attributeID = dependentID;

                if (mDBCon != null)
                {
                    originalDBName = mDBCon.DataBaseName;

                    if (!originalDBName.Equals(DALWrapperAppend.Replace("IVPRefMasterVendor"), StringComparison.OrdinalIgnoreCase))
                    {
                        new RMCommonDBManager(mDBCon).ChangeDBForConnection(DALWrapperAppend.Replace("IVPRefMasterVendor"));
                    }
                }

                ruleSetId = SRMCommon.saveRule(ruleType, ruleName, priority, ruleText, ruleState, ruleId, ruleSetId, userName, ruleGrammarInfo, connectionId, mDBCon, out isSuccess, out errorMsg);

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    throw new Exception("Error while saving rule '" + ruleName + "' - " + errorMsg + ". Please check rule text.");
                }
                else
                {
                    SaveRuleInRefm(ruleId, ruleSetId, ruleTypeID, userName, dependentID, attributeID, mDBCon);
                    FetchDumpDependentsInRefm(ruleSetId, attributeID, connectionId, mDBCon);
                }
                if (!string.IsNullOrEmpty(originalDBName))
                {
                    new RMCommonDBManager(mDBCon).ChangeDBForConnection(originalDBName);
                }

                return ruleSetId;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(originalDBName))
                {
                    new RMCommonDBManager(mDBCon).ChangeDBForConnection(originalDBName);
                }
                throw;
            }
        }

        private RXRuleType GetRXRuleTypeByID(int ruleTypeID)
        {
            RXRuleType ruleType = (RXRuleType)Convert.ToInt32(((RMRuleType)ruleTypeID).GetDescription());
            return ruleType;
        }

        private RADXRuleGrammarInfo PrepareRuleGrammarInfo(int dependentID, int attributeID, int ruleTypeID, RDBConnectionManager mDBCon)
        {
            RADXRuleGrammarInfo ruleGrammarInfo = null;

            Assembly RefMControllerAssembly = Assembly.Load("RefMController");
            Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMRuleEngineController");
            MethodInfo GetRuleGrammar = objType.GetMethod("PrepareRuleGrammarInfo");
            object obj = Activator.CreateInstance(objType);
            ruleGrammarInfo = (RADXRuleGrammarInfo)GetRuleGrammar.Invoke(obj, new object[] { dependentID, attributeID, ruleTypeID, mDBCon });

            return ruleGrammarInfo;
        }

        private void SaveRuleInRefm(int ruleID, int ruleSetID, int ruleTypeID, string userName, int dependentID, int attributeID, RDBConnectionManager mDBCon = null)
        {
            Assembly RefMControllerAssembly = Assembly.Load("RefMController");
            Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMRuleEngineController");
            MethodInfo SaveRule = objType.GetMethod("RMSaveRule");
            object obj = Activator.CreateInstance(objType);
            SaveRule.Invoke(obj, new object[] { ruleID, ruleSetID, ruleTypeID, userName, dependentID, attributeID, mDBCon });
        }

        private void FetchDumpDependentsInRefm(int ruleSetID, int columnID, string connectionID, RDBConnectionManager mDBCon = null)
        {
            Assembly RefMControllerAssembly = Assembly.Load("RefMController");
            Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMRuleEngineController");
            MethodInfo FetchDump = objType.GetMethod("RMFetchAndDumpDependentAttributes");
            object obj = Activator.CreateInstance(objType);
            FetchDump.Invoke(obj, new object[] { ruleSetID, columnID, connectionID, mDBCon });
        }

        public List<EMEntityDataFilter> GetEntityDataByEntityCode(List<EMEntityDataFilter> lstEMEntityDataFilterInfo, EMInputType entityTypeInputType, EMInputType attributeInputType, EMOutputType attributeOutputType, bool validateDataType = false, bool fetchAllForNoInput = true)
        {
            if (lstEMEntityDataFilterInfo != null && lstEMEntityDataFilterInfo.Count > 0)
            {
                int inputType = 0;
                int infoCount = 0;
                bool isETInputandAttrOutputTypeSame = entityTypeInputType.ToString().Equals(attributeOutputType.ToString());
                bool isAttrInputVsOutputTypeSame = attributeInputType.ToString().Equals(attributeOutputType.ToString());

                RMModelerDBManager objRMModelerDBManager = new RMModelerDBManager();
                Dictionary<string, string> dctEntityTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                Dictionary<string, EMEntityDataFilter> dctInfo = new Dictionary<string, EMEntityDataFilter>(StringComparer.OrdinalIgnoreCase);
                Dictionary<string, Dictionary<string, string>> dctETvsAttributes = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

                string attributeInputColumnName = string.Empty;
                string attributeOutputColumnName = string.Empty;
                string entityTypeInputColumnName = string.Empty;
                string entityTypeColumnName = string.Empty;

                switch (entityTypeInputType)
                {
                    case EMInputType.RealName: entityTypeInputColumnName = "entity_type_name"; break;
                    case EMInputType.Id: entityTypeInputColumnName = "entity_type_id"; break;
                    case EMInputType.DisplayName: entityTypeInputColumnName = "entity_display_name"; break;
                }

                switch (attributeInputType)
                {
                    case EMInputType.RealName: attributeInputColumnName = "attribute_name"; break;
                    case EMInputType.Id: attributeInputColumnName = "entity_attribute_id"; break;
                    case EMInputType.DisplayName: attributeInputColumnName = "display_name"; break;
                }

                switch (attributeOutputType)
                {
                    case EMOutputType.RealName:
                        attributeOutputColumnName = "attribute_name";
                        entityTypeColumnName = "entity_type_name";
                        inputType = 0;
                        break;
                    case EMOutputType.Id:
                        attributeOutputColumnName = "entity_attribute_id";
                        entityTypeColumnName = "entity_type_id";
                        inputType = 2;
                        break;
                    case EMOutputType.DisplayName:
                        attributeOutputColumnName = "display_name";
                        entityTypeColumnName = "entity_display_name";
                        inputType = 1;
                        break;
                }

                dctEntityTypes = lstEMEntityDataFilterInfo.Select(x => x.EntityType).Distinct().ToDictionary(x => x, y => y, StringComparer.OrdinalIgnoreCase);
                dctInfo = lstEMEntityDataFilterInfo.ToDictionary(x => "R" + (infoCount++), y => y, StringComparer.OrdinalIgnoreCase);
                if (!isETInputandAttrOutputTypeSame)
                {
                    DataSet dsEntityType = new DataSet();
                    dsEntityType = objRMModelerDBManager.GetEntityTypes();
                    Dictionary<string, string> masterET = dsEntityType.Tables[0].AsEnumerable().Where(x => x.Field<bool>("is_master") == true).ToDictionary(x => Convert.ToString(x[entityTypeInputColumnName]), y => Convert.ToString(y[entityTypeColumnName]), StringComparer.OrdinalIgnoreCase);
                    Dictionary<string, string> legET = dsEntityType.Tables[0].AsEnumerable().Where(x => x.Field<bool>("is_master") == false).ToDictionary(x => Convert.ToString(x[entityTypeInputColumnName]), y => masterET[Convert.ToString(y["derived_from_entity_type_id"])], StringComparer.OrdinalIgnoreCase);
                    foreach (var ET in dctEntityTypes.Keys.ToList())
                    {
                        if (masterET.ContainsKey(ET))
                            dctEntityTypes[ET] = masterET[ET];
                        else if (legET.ContainsKey(ET))
                            dctEntityTypes[ET] = legET[ET];
                    }
                }

                DataSet dsAttributes = objRMModelerDBManager.GetAttributesForEntityType(dctEntityTypes.Keys.Select(x => Convert.ToInt32(x)).ToList(), string.Empty, EMModule.AllSystems);

                if (!isAttrInputVsOutputTypeSame)
                {
                    //DataSet dsAttributes = objRMModelerDBManager.GetAttributesForEntityType(dctEntityTypes.Keys.Select(x => Convert.ToInt32(x)).ToList(), string.Empty, EMModule.AllSystems);

                    foreach (DataRow row in dsAttributes.Tables[0].Rows)
                    {
                        if (!dctETvsAttributes.ContainsKey(Convert.ToString(row[entityTypeInputColumnName])))
                        {
                            dctETvsAttributes.Add(Convert.ToString(row[entityTypeInputColumnName]), new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                            dctETvsAttributes[Convert.ToString(row[entityTypeInputColumnName])].Add("entity_code", "entity_code");
                            dctETvsAttributes[Convert.ToString(row[entityTypeInputColumnName])].Add("Entity Code", "entity_code");
                        }

                        if (!dctETvsAttributes[Convert.ToString(row[entityTypeInputColumnName])].ContainsKey(Convert.ToString(row[attributeInputColumnName])))
                        {
                            dctETvsAttributes[Convert.ToString(row[entityTypeInputColumnName])].Add(Convert.ToString(row[attributeInputColumnName]), Convert.ToString(row[attributeOutputColumnName]));
                        }
                    }
                }
                else
                {
                    //DataSet dsAttributes = objRMModelerDBManager.GetAttributesForEntityType(dctEntityTypes.Keys.Select(x => Convert.ToInt32(x)).ToList(), string.Empty, EMModule.AllSystems);
                    Dictionary<string, Dictionary<string, DataRow>> dctETVsAttributeVsDataRow = new Dictionary<string, Dictionary<string, DataRow>>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataRow row in dsAttributes.Tables[0].Rows)
                    {
                        string entityTypeRealName = Convert.ToString(row[entityTypeInputColumnName]);
                        string attributeRealName = Convert.ToString(row[attributeInputColumnName]);
                        if (dctEntityTypes.ContainsKey(entityTypeRealName))
                        {
                            var entityTypes = dctEntityTypes.Where(x => x.Value.SRMEqualWithIgnoreCase(dctEntityTypes[entityTypeRealName]));
                            foreach (var et in entityTypes)
                            {
                                if (!dctETVsAttributeVsDataRow.ContainsKey(et.Key))
                                    dctETVsAttributeVsDataRow.Add(et.Key, new Dictionary<string, DataRow>());
                                if (!dctETVsAttributeVsDataRow[et.Key].ContainsKey(attributeRealName))
                                    dctETVsAttributeVsDataRow[et.Key].Add(attributeRealName, row);
                            }
                        }
                    }

                    foreach (var info in lstEMEntityDataFilterInfo)
                    {
                        if (!dctETvsAttributes.ContainsKey(info.EntityType))
                        {
                            dctETvsAttributes.Add(info.EntityType, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                            dctETvsAttributes[info.EntityType].Add("entity_code", "entity_code");
                            dctETvsAttributes[info.EntityType].Add("Entity Code", "entity_code");
                        }

                        foreach (var attr in info.Attributes)
                        {
                            if (!dctETvsAttributes[info.EntityType].ContainsKey(attr))
                            {
                                dctETvsAttributes[info.EntityType].Add(attr, attr);
                            }
                        }

                        //Start Vikas Bhat, 23-Dec-2019, Datatype validations for primary attributes (failed will be excluded from proc input)
                        if (validateDataType && info.PrimaryAttributeName != null && info.PrimaryAttributeValues != null && info.PrimaryAttributeValues.Count > 0 && dctETVsAttributeVsDataRow.ContainsKey(info.EntityType))
                        {
                            //string entityTypeRealName = dctEntityTypes[info.EntityType];
                            info.FinalPrimaryAttributeValues = PrimaryAttributeValuesDatatypeCheck(info.PrimaryAttributeName, dctETVsAttributeVsDataRow[info.EntityType], info.PrimaryAttributeValues);
                        }
                        else
                            info.FinalPrimaryAttributeValues = info.PrimaryAttributeValues;
                        //End Vikas Bhat, 23-Dec-2019, Datatype validations for primary attributes (failed will be excluded from proc input)
                    }
                }

                XElement entityTypeXML = new XElement("root", from item in dctInfo.Where(x => x.Value != null)
                                                              select new XElement("row",
                                                                 new XAttribute("ref", item.Key),
                                                                 new XAttribute("fa", (item.Value.PrimaryAttributeValues != null && item.Value.PrimaryAttributeValues.Count > 0 && (item.Value.FinalPrimaryAttributeValues == null || item.Value.FinalPrimaryAttributeValues.Count == 0)) || !fetchAllForNoInput ? "0" : "1"),
                                                                 new XAttribute("entity_type_id", dctEntityTypes[item.Value.EntityType]),
                                                                 new XAttribute("colsToFetch", string.Join(",", item.Value.Attributes.Select(x => dctETvsAttributes[item.Value.EntityType][x]))),
                                                                 new XAttribute("searchColumn", item.Value.PrimaryAttributeName.Contains(":") ? item.Value.PrimaryAttributeName : dctETvsAttributes[item.Value.EntityType][item.Value.PrimaryAttributeName]),
                                                                from val in item.Value.FinalPrimaryAttributeValues == null ? new HashSet<string>() : item.Value.FinalPrimaryAttributeValues
                                                                select
                                                              new XElement("value", val)
                                                     ));

                string mapping = "<root></root>";
                DataSet dsResult = new RMLookupDataMassage().GetData(entityTypeXML.ToString(), mapping, inputType, false, false, false, null);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    Dictionary<string, Dictionary<string, DataRow>> dctETvsSecLookupColumns = new Dictionary<string, Dictionary<string, DataRow>>(StringComparer.OrdinalIgnoreCase);

                    if (dsResult.Tables[dsResult.Tables.Count - 1] != null && dsResult.Tables[dsResult.Tables.Count - 1].Rows.Count > 0)
                    {
                        foreach (DataRow row in dsResult.Tables[dsResult.Tables.Count - 1].Rows)
                        {
                            if (!dctETvsSecLookupColumns.ContainsKey(Convert.ToString(row["ref"])))
                            {
                                dctETvsSecLookupColumns.Add(Convert.ToString(row["ref"]), new Dictionary<string, DataRow>());
                            }
                            dctETvsSecLookupColumns[Convert.ToString(row["ref"])].Add(Convert.ToString(row[attributeOutputColumnName]), row);
                        }

                    }

                    int count = 0;
                    foreach (var info in dctInfo)
                    {
                        if (dctETvsSecLookupColumns.ContainsKey(info.Key))
                        {
                            var secLookups = info.Value.Attributes.Intersect(dctETvsSecLookupColumns[info.Key].Keys, StringComparer.OrdinalIgnoreCase);
                            if (secLookups.Count() > 0)
                            {
                                List<RMLookupAttributeInfo> lstLookups = new List<RMLookupAttributeInfo>();
                                foreach (var attr in secLookups)
                                {
                                    lstLookups.Add(new RMLookupAttributeInfo()
                                    {
                                        IsSecurityLookup = true,
                                        ParentAttributeId = Convert.ToInt32(dctETvsSecLookupColumns[info.Key][attr]["parent_security_attribute_id"]),
                                        ParentId = Convert.ToInt32(Convert.ToInt32(dctETvsSecLookupColumns[info.Key][attr]["parent_security_type_id"])),
                                        MappedColumns = new List<string>() { attr }
                                    });
                                }
                                new RMLookupDataMassage().MassageSecurityLookup(dsResult.Tables[count], new RMLookupDataMassageInfo() { InputType = RMLookupInputType.ID, IsArchive = false, IsAuditView = false, IsEntityCodeToValues = true }, lstLookups);
                            }
                        }
                        info.Value.OutputData = dsResult.Tables[count].Copy();
                        count++;
                    }
                }
            }
            return lstEMEntityDataFilterInfo;
        }

        public DataTable GetMassagedEntityData(int entityTypeID, bool getDisplayNames, bool massageLookups, HashSet<string> entityCodes, bool fromWorkflow, RDBConnectionManager refMDBConnectionIdMgr = null)
        {
            mLogger.Debug("RMCommonController: GetMassagedEntityData -> Start");
            DataSet dsEntity = null;
            DataTable entityData = null;
            DataTable secLookups = null;
            bool filterEntityCode = false;
            try
            {
                string entityCodeXML = string.Empty;
                if (entityCodes != null && entityCodes.Count > 0)
                {
                    filterEntityCode = true;
                    entityCodeXML = "<root>";
                    foreach (string ec in entityCodes)
                    {
                        entityCodeXML = entityCodeXML + "<ec>" + ec + "</ec>";
                    }
                    entityCodeXML = entityCodeXML + "</root>";
                }

                string query = " EXEC REFM_GetMassagedEntityData " + entityTypeID + ", " + (getDisplayNames ? "1" : "0") + ", " +
                    (massageLookups ? "1" : "0") + ", " + (filterEntityCode ? "'" + entityCodeXML + "'" : "") + ", " + (fromWorkflow ? "1" : "0");

                if (refMDBConnectionIdMgr == null)
                    dsEntity = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                else
                    dsEntity = CommonDALWrapper.ExecuteSelectQuery(query, refMDBConnectionIdMgr);

                if (dsEntity != null && dsEntity.Tables.Count > 0)
                {
                    entityData = dsEntity.Tables[0];
                    secLookups = dsEntity.Tables[1];
                }

                return entityData;
            }

            catch (Exception ex)
            {
                mLogger.Error("RMCommonController: GetMassagedEntityData -> Error: " + ex.Message);
                throw ex;
            }

            finally
            {
                mLogger.Debug("RMCommonController: GetMassagedEntityData -> End");
            }
        }

        private HashSet<string> PrimaryAttributeValuesDatatypeCheck(string attributeName, Dictionary<string, DataRow> attributeNameVsDataRow, HashSet<string> valuesToCheck)
        {
            HashSet<string> primaryAttributeValues = new HashSet<string>();
            bool isCurve = attributeName.Contains(":");
            string attribute = attributeName;
            foreach (string value in valuesToCheck)
            {
                bool isPassed = true;
                if (!string.IsNullOrEmpty(value))
                {
                    string masterAttrValue = value;
                    if (isCurve)
                    {
                        masterAttrValue = value.Split(':')[0];
                        attribute = attributeName.Split(':')[0];
                    }
                    DataRow row = attributeNameVsDataRow.ContainsKey(attribute) ? attributeNameVsDataRow[attribute] : null;
                    if (row != null)
                    {
                        string dataType = Convert.ToString(row["real_data_type"]);
                        string dataLength = Convert.ToString(row["real_data_length"]);
                        isPassed = IsAttributeDataCheckPassed(masterAttrValue, dataType, dataLength);
                    }
                    if (isCurve && isPassed)
                    {
                        if (value.Contains(":"))
                        {
                            string legAttrValue = string.Empty;
                            legAttrValue = value.Substring(value.IndexOf(':') + 1);// value.Split(':')[1];
                            attribute = attributeName.Split(':')[1];
                            row = null;
                            row = attributeNameVsDataRow.ContainsKey(attribute) ? attributeNameVsDataRow[attribute] : null;
                            if (row != null)
                            {
                                string dataType = Convert.ToString(row["real_data_type"]);
                                string dataLength = Convert.ToString(row["real_data_length"]);
                                isPassed = IsAttributeDataCheckPassed(legAttrValue, dataType, dataLength);
                            }
                            if (isPassed && (!string.IsNullOrEmpty(masterAttrValue) || !string.IsNullOrEmpty(legAttrValue)))
                            {
                                primaryAttributeValues.Add(masterAttrValue + ":" + legAttrValue);
                            }
                        }
                    }
                    else if (isPassed && !string.IsNullOrEmpty(value))
                    {
                        primaryAttributeValues.Add(value);
                    }
                }
            }
            return primaryAttributeValues;
        }
        private bool IsAttributeDataCheckPassed(string value, string dataType, string dataLength)
        {
            bool isPassed = true;
            if (string.IsNullOrEmpty(value))
                return isPassed;
            switch (dataType.ToUpper())
            {
                case "INT":
                    int outInt;
                    isPassed = Int32.TryParse(value, out outInt);
                    break;
                case "DATETIME":
                    DateTime outDT;
                    isPassed = DateTime.TryParse(value, out outDT);
                    break;
                case "DECIMAL":
                    decimal outDec;
                    int prec = Convert.ToInt32(dataLength.Split(',')[0]);
                    int scale = Convert.ToInt32(dataLength.Split(',')[1]);
                    prec = prec - scale - 1;
                    if (value.Contains("."))
                    {
                        isPassed = (decimal.TryParse(value, out outDec) && (value.Split('.')[0].Length <= prec && value.Split('.')[1].Length <= scale));
                    }
                    else
                        isPassed = (decimal.TryParse(value, out outDec) && value.Length <= prec);
                    break;
                case "VARCHAR":
                    isPassed = value.Length <= Convert.ToInt32(dataLength);
                    break;
                default:
                    break;
            }
            return isPassed;
        }


        public List<EMEntityDataFilterForArchive> GetEntityArchiveDataByEntityCode(List<EMEntityDataFilterForArchive> lstEMEntityDataFilterInfo, EMInputType entityTypeInputType, EMInputType attributeInputType, EMOutputType attributeOutputType, bool requireAuditDateColumnsAsString, bool requireDayBestCopy)
        {
            if (lstEMEntityDataFilterInfo != null && lstEMEntityDataFilterInfo.Count > 0)
            {
                int inputType = 0;
                int infoCount = 0;
                bool isETInputandAttrOutputTypeSame = entityTypeInputType.ToString().Equals(attributeOutputType.ToString());
                bool isAttrInputVsOutputTypeSame = attributeInputType.ToString().Equals(attributeOutputType.ToString());

                RMModelerDBManager objRMModelerDBManager = new RMModelerDBManager();
                Dictionary<string, string> dctEntityTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                Dictionary<string, EMEntityDataFilterForArchive> dctInfo = new Dictionary<string, EMEntityDataFilterForArchive>(StringComparer.OrdinalIgnoreCase);
                Dictionary<string, Dictionary<string, string>> dctETvsAttributes = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

                string attributeInputColumnName = string.Empty;
                string attributeOutputColumnName = string.Empty;
                string entityTypeInputColumnName = string.Empty;
                string entityTypeColumnName = string.Empty;

                switch (entityTypeInputType)
                {
                    case EMInputType.RealName: entityTypeInputColumnName = "entity_type_name"; break;
                    case EMInputType.Id: entityTypeInputColumnName = "entity_type_id"; break;
                    case EMInputType.DisplayName: entityTypeInputColumnName = "entity_display_name"; break;
                }

                switch (attributeInputType)
                {
                    case EMInputType.RealName: attributeInputColumnName = "attribute_name"; break;
                    case EMInputType.Id: attributeInputColumnName = "entity_attribute_id"; break;
                    case EMInputType.DisplayName: attributeInputColumnName = "display_name"; break;
                }

                switch (attributeOutputType)
                {
                    case EMOutputType.RealName:
                        attributeOutputColumnName = "attribute_name";
                        entityTypeColumnName = "entity_type_name";
                        inputType = 0;
                        break;
                    case EMOutputType.Id:
                        attributeOutputColumnName = "entity_attribute_id";
                        entityTypeColumnName = "entity_type_id";
                        inputType = 2;
                        break;
                    case EMOutputType.DisplayName:
                        attributeOutputColumnName = "display_name";
                        entityTypeColumnName = "entity_display_name";
                        inputType = 1;
                        break;
                }

                dctEntityTypes = lstEMEntityDataFilterInfo.Select(x => x.EntityType).Distinct().ToDictionary(x => x, y => y, StringComparer.OrdinalIgnoreCase);
                dctInfo = lstEMEntityDataFilterInfo.ToDictionary(x => "R" + (infoCount++), y => y, StringComparer.OrdinalIgnoreCase);
                if (!isETInputandAttrOutputTypeSame)
                {
                    DataSet dsEntityType = new DataSet();
                    dsEntityType = objRMModelerDBManager.GetEntityTypes();
                    Dictionary<string, string> masterET = dsEntityType.Tables[0].AsEnumerable().Where(x => x.Field<bool>("is_master") == true).ToDictionary(x => Convert.ToString(x[entityTypeInputColumnName]), y => Convert.ToString(y[entityTypeColumnName]), StringComparer.OrdinalIgnoreCase);
                    Dictionary<string, string> legET = dsEntityType.Tables[0].AsEnumerable().Where(x => x.Field<bool>("is_master") == false).ToDictionary(x => Convert.ToString(x[entityTypeInputColumnName]), y => masterET[Convert.ToString(y["derived_from_entity_type_id"])], StringComparer.OrdinalIgnoreCase);
                    foreach (var ET in dctEntityTypes.Keys.ToList())
                    {
                        if (masterET.ContainsKey(ET))
                            dctEntityTypes[ET] = masterET[ET];
                        else if (legET.ContainsKey(ET))
                            dctEntityTypes[ET] = legET[ET];
                    }
                }

                if (!isAttrInputVsOutputTypeSame)
                {
                    DataSet dsAttributes = objRMModelerDBManager.GetAttributesForEntityType(dctEntityTypes.Keys.Select(x => Convert.ToInt32(x)).ToList(), string.Empty, EMModule.AllSystems);

                    foreach (DataRow row in dsAttributes.Tables[0].Rows)
                    {
                        if (!dctETvsAttributes.ContainsKey(Convert.ToString(row[entityTypeInputColumnName])))
                        {
                            dctETvsAttributes.Add(Convert.ToString(row[entityTypeInputColumnName]), new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                            dctETvsAttributes[Convert.ToString(row[entityTypeInputColumnName])].Add("entity_code", "entity_code");
                            dctETvsAttributes[Convert.ToString(row[entityTypeInputColumnName])].Add("Entity Code", "entity_code");
                        }

                        if (!dctETvsAttributes[Convert.ToString(row[entityTypeInputColumnName])].ContainsKey(Convert.ToString(row[attributeInputColumnName])))
                        {
                            dctETvsAttributes[Convert.ToString(row[entityTypeInputColumnName])].Add(Convert.ToString(row[attributeInputColumnName]), Convert.ToString(row[attributeOutputColumnName]));
                        }
                    }
                }
                else
                {
                    foreach (var info in lstEMEntityDataFilterInfo)
                    {
                        if (!dctETvsAttributes.ContainsKey(info.EntityType))
                        {
                            dctETvsAttributes.Add(info.EntityType, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                            dctETvsAttributes[info.EntityType].Add("entity_code", "entity_code");
                            dctETvsAttributes[info.EntityType].Add("Entity Code", "entity_code");
                        }

                        foreach (var attr in info.Attributes)
                        {
                            if (!dctETvsAttributes[info.EntityType].ContainsKey(attr))
                            {
                                dctETvsAttributes[info.EntityType].Add(attr, attr);
                            }
                        }
                    }
                }

                XElement entityTypeXML = new XElement("root", from item in dctInfo.Where(x => x.Value != null)
                                                              select new XElement("row",
                                                                 new XAttribute("ref", item.Key),
                                                                 new XAttribute("entity_type_id", dctEntityTypes[item.Value.EntityType]),
                                                                 new XAttribute("colsToFetch", string.Join(",", item.Value.Attributes.Select(x => dctETvsAttributes[item.Value.EntityType][x]))),
                                                                 new XAttribute("searchColumn", dctETvsAttributes[item.Value.EntityType][item.Value.PrimaryAttributeName]),
                                                                from info in item.Value.AttributeValues == null ? new List<EMEntityArchiveDataInfo>() : item.Value.AttributeValues
                                                                from val in info.PrimaryAttributeValues == null ? new HashSet<string>() : info.PrimaryAttributeValues
                                                                select
                                                              new XElement("value",
                                                              new XAttribute("val", val),
                                                              new XAttribute("ed", info.EffectiveDate),
                                                              new XAttribute("kd", info.KnowledgeDate)
                                                              )
                                                     ));

                string mapping = "<root></root>";
                DataSet dsResult = new RMLookupDataMassage().GetArchiveData(entityTypeXML.ToString(), mapping, inputType, false, false, false, requireAuditDateColumnsAsString, requireDayBestCopy, null);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    Dictionary<string, Dictionary<string, DataRow>> dctETvsSecLookupColumns = new Dictionary<string, Dictionary<string, DataRow>>(StringComparer.OrdinalIgnoreCase);
                    if (dsResult.Tables[dsResult.Tables.Count - 1] != null && dsResult.Tables[dsResult.Tables.Count - 1].Rows.Count > 0)
                    {
                        foreach (DataRow row in dsResult.Tables[dsResult.Tables.Count - 1].Rows)
                        {
                            if (!dctETvsSecLookupColumns.ContainsKey(Convert.ToString(row["ref"])))
                            {
                                dctETvsSecLookupColumns.Add(Convert.ToString(row["ref"]), new Dictionary<string, DataRow>());
                            }
                            dctETvsSecLookupColumns[Convert.ToString(row["ref"])].Add(Convert.ToString(row[attributeOutputColumnName]), row);
                        }

                    }

                    int count = 0;
                    foreach (var info in dctInfo)
                    {
                        if (dctETvsSecLookupColumns.ContainsKey(info.Key))
                        {
                            var secLookups = info.Value.Attributes.Intersect(dctETvsSecLookupColumns[info.Key].Keys, StringComparer.OrdinalIgnoreCase);
                            if (secLookups.Count() > 0)
                            {
                                foreach (var attr in secLookups)
                                {
                                    new RMLookupAttributeInfo()
                                    {
                                        IsSecurityLookup = true,
                                        ParentAttributeId = Convert.ToInt32(dctETvsSecLookupColumns[info.Key][attr]["parent_security_attribute_id"]),
                                        ParentId = Convert.ToInt32(Convert.ToInt32(dctETvsSecLookupColumns[info.Key][attr]["parent_security_type_id"])),
                                        MappedColumns = new List<string>() { attr }
                                    };
                                    new RMLookupDataMassage().MassageSecurityLookupForAudit(dsResult.Tables[count].AsEnumerable(),
                                                                                                new RMLookupDataMassageInfo()
                                                                                                {
                                                                                                    InputType = RMLookupInputType.ID,
                                                                                                    IsArchive = true,
                                                                                                    IsAuditView = false,
                                                                                                    IsEntityCodeToValues = true
                                                                                                },
                                                                                                new RMLookupAttributeInfo()
                                                                                                {
                                                                                                    IsSecurityLookup = true,
                                                                                                    ParentAttributeId = Convert.ToInt32(dctETvsSecLookupColumns[info.Key][attr]["parent_security_attribute_id"]),
                                                                                                    ParentId = Convert.ToInt32(Convert.ToInt32(dctETvsSecLookupColumns[info.Key][attr]["parent_security_type_id"])),
                                                                                                    MappedColumns = new List<string>() { attr }
                                                                                                });
                                }
                            }
                        }
                        info.Value.OutputData = dsResult.Tables[count].Copy();
                        count++;
                    }
                }
            }
            return lstEMEntityDataFilterInfo;
        }

        public Dictionary<int, Dictionary<string, RMReconciledResponseInfo>> ReconcileEntityData(Dictionary<int, IEnumerable<DataRow>> dictEntityData, HashSet<string> attributesToReconcile, EMInputType attributeInputType, int moduleId, bool fromWorkflow, int entityTypeID)
        {
            mLogger.Debug("RMCommonController: ReconcileEntityData -> Start");
            Dictionary<int, Dictionary<string, RMReconciledResponseInfo>> entityTypeIdVsEntityCodeVsModifiedAttributes = new Dictionary<int, Dictionary<string, RMReconciledResponseInfo>>();
            try
            {
                List<EMEntityDataFilter> lstEMEntityDataFilterInfo = new List<EMEntityDataFilter>();
                List<string> primaryValues = new List<string>();
                Dictionary<string, List<string>> masterECvsLegEC = new Dictionary<string, List<string>>();
                Dictionary<string, RMEntityDetailsInfo> entityTypeDetails = new RMModelerController().GetEntityTypeDetails(moduleId, new List<int>() { entityTypeID },null,false);

                foreach (var kvp in dictEntityData)
                {
                    bool isLeg = false;
                    Dictionary<string, RMReconciledResponseInfo> entityCodeVsModifiedAttributes = new Dictionary<string, RMReconciledResponseInfo>();
                    int entityTypeId = kvp.Key;
                    IEnumerable<DataRow> inputRows = kvp.Value;
                    EMEntityDataFilter input = new EMEntityDataFilter();
                    input.PrimaryAttributeValues = new HashSet<string>();
                    input.PrimaryAttributeName = "entity_code";
                    input.EntityType = entityTypeId.ToString();
                    input.Attributes = new HashSet<string>();

                    if (inputRows != null)
                    {
                        if (entityTypeId == entityTypeID)
                        {
                            foreach (DataRow row in inputRows)
                            {
                                string entityCode = Convert.ToString(row["entity_code"]);
                                input.PrimaryAttributeValues.Add(entityCode);
                                primaryValues.Add(entityCode);
                            }
                        }
                        else
                        {
                            isLeg = true;
                            input.PrimaryAttributeValues.UnionWith(primaryValues);

                            foreach (DataRow row in inputRows)
                            {
                                string MentityCode = Convert.ToString(row["master_entity_code"]);
                                if (!masterECvsLegEC.ContainsKey(MentityCode))
                                    masterECvsLegEC.Add(MentityCode, new List<string>());

                                string entityCode = Convert.ToString(row["entity_code"]);
                                masterECvsLegEC[MentityCode].Add(entityCode);

                            }
                        }
                    }

                    RMEntityDetailsInfo entityTypeInfo = null;

                    if (entityTypeDetails != null && entityTypeDetails.Count > 0)
                    {
                        string entityTypeDisplayName = entityTypeDetails.Keys.FirstOrDefault();
                        if (!string.IsNullOrEmpty(entityTypeDisplayName))
                            entityTypeInfo = entityTypeDetails[entityTypeDisplayName];

                        if (isLeg)
                        {
                            foreach (var legName in entityTypeInfo.Legs.Keys)
                            {
                                if (entityTypeInfo.Legs[legName].EntityTypeID == entityTypeId)
                                {
                                    entityTypeInfo = entityTypeInfo.Legs[legName];
                                    break;
                                }
                            }
                        }
                    }

                    if (entityTypeInfo != null)
                    {
                        string entityTypeRealName = entityTypeInfo.EntityTypeName;
                        var attributesInfo = entityTypeInfo.Attributes;
                        Dictionary<string, RMEntityAttributeInfo> dictAttributeInfo = entityTypeInfo.Attributes;
                        Dictionary<string, RMEntityAttributeInfo> finalAttributesInfo = dictAttributeInfo;

                        if (attributeInputType != EMInputType.DisplayName)
                        {
                            finalAttributesInfo = new Dictionary<string, RMEntityAttributeInfo>();
                            foreach (var attrKVP in dictAttributeInfo)
                            {
                                string attrKey = (attributeInputType == EMInputType.Id) ? attrKVP.Value.EntityAttributeID.ToString() : attrKVP.Value.AttributeName;

                                if (!finalAttributesInfo.ContainsKey(attrKey))
                                    finalAttributesInfo.Add(attrKey, attrKVP.Value);
                            }
                        }

                        foreach (var attrKvp in entityTypeInfo.Attributes)
                        {
                            input.Attributes.Add(attrKvp.Value.AttributeName);
                        }

                        if (input.PrimaryAttributeValues.Count > 0)
                        {
                            lstEMEntityDataFilterInfo.Add(input);

                            DataTable entityData = GetMassagedEntityData(entityTypeId, false, false, input.PrimaryAttributeValues, fromWorkflow);
                            Dictionary<string, DataRow> entityCodeVsDBData = new Dictionary<string, DataRow>();

                            if (entityData != null && entityData.Rows.Count > 0)
                            {
                                foreach (DataRow dbRow in entityData.Rows)
                                {
                                    string entityCode = Convert.ToString(dbRow["entity_code"]);
                                    if (!entityCodeVsDBData.ContainsKey(entityCode))
                                        entityCodeVsDBData.Add(entityCode, dbRow);

                                    if (isLeg)
                                    {
                                        string masterEntityCode = Convert.ToString(dbRow["master_entity_code"]);
                                        if (masterECvsLegEC.ContainsKey(masterEntityCode) && masterECvsLegEC[masterEntityCode].Contains(entityCode))
                                        {
                                            masterECvsLegEC[masterEntityCode].Remove(entityCode);
                                            if (masterECvsLegEC[masterEntityCode].Count == 0)
                                                masterECvsLegEC.Remove(masterEntityCode);
                                        }
                                        else
                                        {
                                            //Leg Deletion
                                            if (!entityCodeVsModifiedAttributes.ContainsKey(masterEntityCode))
                                                entityCodeVsModifiedAttributes.Add(masterEntityCode, new RMReconciledResponseInfo() { Attributes = new List<RMReconciledAttributeInfo>(), Legs = new List<RMReconciledLegInfo>() });
                                            entityCodeVsModifiedAttributes[masterEntityCode].Legs.Add(new RMReconciledLegInfo()
                                            {
                                                LegDisplayName = entityTypeInfo.EntityDisplayName,
                                                LegEntityTypeId = entityTypeInfo.EntityTypeID,
                                                LegEntityTypeName = entityTypeInfo.EntityTypeName
                                            });
                                        }
                                    }
                                }

                                foreach (DataRow row in inputRows)
                                {
                                    string entityCode = Convert.ToString(row["entity_code"]);
                                    if (entityCodeVsDBData.ContainsKey(entityCode))
                                    {
                                        DataRow dbRow = entityCodeVsDBData[entityCode];
                                        foreach (var attr in finalAttributesInfo)
                                        {
                                            string attrKey = attr.Key;
                                            if (attributesToReconcile == null || (attributesToReconcile != null && attributesToReconcile.SRMContainsWithIgnoreCase(attrKey)))
                                            {
                                                object dbValue = null;
                                                object newValue = null;
                                                bool isAttrValueChanged = IsAttributeDataChanged(row, dbRow, attrKey, attr.Value, out dbValue, out newValue);
                                                if (isAttrValueChanged)
                                                {
                                                    if (!isLeg)
                                                    {
                                                        if (!entityCodeVsModifiedAttributes.ContainsKey(entityCode))
                                                            entityCodeVsModifiedAttributes.Add(entityCode, new RMReconciledResponseInfo() { Attributes = new List<RMReconciledAttributeInfo>(), Legs = new List<RMReconciledLegInfo>() });

                                                        RMReconciledAttributeInfo reconciledInfo = new RMReconciledAttributeInfo();
                                                        reconciledInfo.AttributeId = attr.Value.EntityAttributeID;
                                                        reconciledInfo.AttributeRealName = attr.Value.AttributeName;
                                                        reconciledInfo.AttributeDisplayName = attr.Value.DisplayName;
                                                        reconciledInfo.DBValue = dbValue;
                                                        reconciledInfo.NewValue = newValue;

                                                        entityCodeVsModifiedAttributes[entityCode].Attributes.Add(reconciledInfo);
                                                    }
                                                    else
                                                    {
                                                        entityCode = Convert.ToString(row["master_entity_code"]);
                                                        if (!entityCodeVsModifiedAttributes.ContainsKey(entityCode))
                                                            entityCodeVsModifiedAttributes.Add(entityCode, new RMReconciledResponseInfo() { Attributes = new List<RMReconciledAttributeInfo>(), Legs = new List<RMReconciledLegInfo>() });

                                                        if (entityCodeVsModifiedAttributes[entityCode].Legs.Where(x => x.LegEntityTypeId == entityTypeInfo.EntityTypeID).Count() == 0)
                                                        {
                                                            entityCodeVsModifiedAttributes[entityCode].Legs.Add(new RMReconciledLegInfo()
                                                            {
                                                                LegDisplayName = entityTypeInfo.EntityDisplayName,
                                                                LegEntityTypeId = entityTypeInfo.EntityTypeID,
                                                                LegEntityTypeName = entityTypeInfo.EntityTypeName
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (isLeg)
                                    {
                                        entityCode = Convert.ToString(row["master_entity_code"]);
                                        if (!entityCodeVsModifiedAttributes.ContainsKey(entityCode))
                                            entityCodeVsModifiedAttributes.Add(entityCode, new RMReconciledResponseInfo() { Attributes = new List<RMReconciledAttributeInfo>(), Legs = new List<RMReconciledLegInfo>() });
                                        if (entityCodeVsModifiedAttributes[entityCode].Legs.Where(x => x.LegEntityTypeId == entityTypeInfo.EntityTypeID).Count() == 0)
                                        {
                                            entityCodeVsModifiedAttributes[entityCode].Legs.Add(new RMReconciledLegInfo()
                                            {
                                                LegDisplayName = entityTypeInfo.EntityDisplayName,
                                                LegEntityTypeId = entityTypeInfo.EntityTypeID,
                                                LegEntityTypeName = entityTypeInfo.EntityTypeName
                                            });
                                        }
                                    }
                                }
                            }
                            else if (isLeg)
                            {
                                foreach (DataRow row in inputRows)
                                {
                                    string entityCode = Convert.ToString(row["master_entity_code"]);
                                    if (!entityCodeVsModifiedAttributes.ContainsKey(entityCode))
                                        entityCodeVsModifiedAttributes.Add(entityCode, new RMReconciledResponseInfo() { Attributes = new List<RMReconciledAttributeInfo>(), Legs = new List<RMReconciledLegInfo>() });
                                    if (entityCodeVsModifiedAttributes[entityCode].Legs.Where(x => x.LegEntityTypeId == entityTypeInfo.EntityTypeID).Count() == 0)
                                    {
                                        entityCodeVsModifiedAttributes[entityCode].Legs.Add(new RMReconciledLegInfo()
                                        {
                                            LegDisplayName = entityTypeInfo.EntityDisplayName,
                                            LegEntityTypeId = entityTypeInfo.EntityTypeID,
                                            LegEntityTypeName = entityTypeInfo.EntityTypeName
                                        });
                                    }
                                }
                            }

                        }
                    }
                    if (entityCodeVsModifiedAttributes.Count > 0)
                    {
                        if (!entityTypeIdVsEntityCodeVsModifiedAttributes.ContainsKey(entityTypeID))
                            entityTypeIdVsEntityCodeVsModifiedAttributes.Add(entityTypeID, entityCodeVsModifiedAttributes);
                        else
                        {
                            foreach (var ec in entityCodeVsModifiedAttributes.Keys)
                            {
                                if (!entityTypeIdVsEntityCodeVsModifiedAttributes[entityTypeID].ContainsKey(ec))
                                    entityTypeIdVsEntityCodeVsModifiedAttributes[entityTypeID].Add(ec, entityCodeVsModifiedAttributes[ec]);
                                else
                                    entityTypeIdVsEntityCodeVsModifiedAttributes[entityTypeID][ec].Legs.AddRange(entityCodeVsModifiedAttributes[ec].Legs);
                            }
                        }
                    }
                }

                return entityTypeIdVsEntityCodeVsModifiedAttributes;
            }
            catch (Exception ex)
            {
                mLogger.Error("RMCommonController: ReconcileEntityData -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RMCommonController: ReconcileEntityData -> End");
            }
        }

        private bool IsAttributeDataChanged(DataRow inputRow, DataRow dbRow, string columnName, RMEntityAttributeInfo attrInfo, out object dbValue, out object newValue)
        {
            bool isChanged = false;
            dbValue = null;
            newValue = null;

            if (attrInfo != null)
            {
                //if (attrInfo.DataType != RMDBDataTypes.FILE)
                {
                    isChanged = CompareValues(Convert.ToString(dbRow[columnName]), Convert.ToString(inputRow[columnName]), attrInfo.DataType);
                    dbValue = dbRow[columnName];
                    newValue = inputRow[columnName];
                }
            }

            return isChanged;
        }

        private static bool CompareValues(string valueFromDB, string valueFromUI, RMDBDataTypes dataType)
        {
            bool isDataChanged = false;
            switch (dataType)
            {
                case RMDBDataTypes.FILE:
                case RMDBDataTypes.VARCHAR:
                case RMDBDataTypes.VARCHARMAX:
                case RMDBDataTypes.LOOKUP:
                case RMDBDataTypes.SECURITY_LOOKUP:
                    {
                        isDataChanged = valueFromDB != valueFromUI ? true : false;
                        break;
                    }
                case RMDBDataTypes.INT:
                    {
                        int num1;
                        int num2;
                        bool isIntFromDB = Int32.TryParse(valueFromDB, out num1);
                        bool isIntFromUI = Int32.TryParse(valueFromUI, out num2);
                        if (isIntFromDB && isIntFromUI)
                            isDataChanged = num1 != num2 ? true : false;
                        else
                            isDataChanged = valueFromDB != valueFromUI ? true : false;

                        break;
                    }
                case RMDBDataTypes.BIT:
                    {
                        bool bool1;
                        bool bool2;
                        bool isBoolFromDB = bool.TryParse(valueFromDB, out bool1);
                        bool isBoolFromUI = bool.TryParse(valueFromUI, out bool2);
                        if (isBoolFromDB && isBoolFromUI)
                            isDataChanged = bool1 != bool2 ? true : false;
                        else
                            isDataChanged = valueFromDB != valueFromUI ? true : false;
                        break;
                    }
                case RMDBDataTypes.DATETIME:
                    {
                        DateTime datetime1;
                        DateTime datetime2;
                        bool isBoolFromDB = DateTime.TryParse(valueFromDB, out datetime1);
                        bool isBoolFromUI = DateTime.TryParse(valueFromUI, out datetime2);
                        if (isBoolFromDB && isBoolFromUI)
                            isDataChanged = datetime1 != datetime2 ? true : false;
                        else
                            isDataChanged = valueFromDB != valueFromUI ? true : false;
                        //isDataTypeCheckRequired = Convert.ToDateTime(drDB[0][dc.ColumnName]) != Convert.ToDateTime(drPt[dc.ColumnName]) ? true : false;
                        break;
                    }
                case RMDBDataTypes.DECIMAL:
                    {
                        decimal dec1;
                        decimal dec2;
                        bool isIntFromDB = decimal.TryParse(valueFromDB, out dec1);
                        bool isIntFromUI = decimal.TryParse(valueFromUI, out dec2);
                        if (isIntFromDB && isIntFromUI)
                            isDataChanged = dec1 != dec2 ? true : false;
                        else
                            isDataChanged = valueFromDB != valueFromUI ? true : false;

                        break;
                    }
            }
            return isDataChanged;
        }

        public void RaiseEvents(Dictionary<int, DataTable> dictEntityAttributesInfo, string userName, int entityTypeId, SRMEventActionType eventAction, string entityCode, DateTime loadingTime, int moduleID, List<SRMEventInfo> lstEventInfo, DateTime? effectiveStartDate, List<string> pendingAt, string initiator, Dictionary<int, Dictionary<string, RMReconciledResponseInfo>> reconciledResponseInfo, Dictionary<string, string> attributeRealNameVsDisplayName, bool isCreate)
        {
            List<SRMEventInfo> lstInfo = new List<SRMEventInfo>();
            List<string> legs = new List<string>();
            string entityDisplayName = string.Empty;
            string query = string.Format("SELECT entity_type_id,entity_display_name FROM IVPRefMaster.dbo.ivp_refm_entity_type WHERE entity_type_id IN ({0})", string.Join(",", dictEntityAttributesInfo.Keys));
            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            Dictionary<int, string> entityTypeIdVsDisplayname = ds.Tables[0].AsEnumerable().ToDictionary(x => Convert.ToInt32(x[0]), y => Convert.ToString(y[1]));
            entityDisplayName = entityTypeIdVsDisplayname[entityTypeId];
            entityTypeIdVsDisplayname.Remove(entityTypeId);

            List<SRMEventAttributeDetails> attrList = new List<SRMEventAttributeDetails>();
            if (isCreate || eventAction == SRMEventActionType.Draft)
            {
                foreach (DataColumn dc in dictEntityAttributesInfo[entityTypeId].Columns)
                {
                    if (!dc.ColumnName.SRMEqualWithIgnoreCase("entity_code") && !string.IsNullOrEmpty(Convert.ToString(dictEntityAttributesInfo[entityTypeId].Rows[0][dc.ColumnName])))
                        attrList.Add(new SRMEventAttributeDetails() { Name = attributeRealNameVsDisplayName[dc.ColumnName], RealName = dc.ColumnName });
                }
                legs = entityTypeIdVsDisplayname.Count > 0 ? entityTypeIdVsDisplayname.Values.ToList<string>() : new List<string>() { };
            }
            else
            {
                if (reconciledResponseInfo.ContainsKey(entityTypeId) && reconciledResponseInfo[entityTypeId].ContainsKey(entityCode))
                {
                    foreach (var attrInfo in reconciledResponseInfo[entityTypeId][entityCode].Attributes)
                    {
                        attrList.Add(new SRMEventAttributeDetails() { Name = attrInfo.AttributeDisplayName, RealName = attrInfo.AttributeRealName });
                    }
                    foreach (var legInfo in reconciledResponseInfo[entityTypeId][entityCode].Legs)
                    {
                        legs.Add(legInfo.LegDisplayName);
                    }
                }
            }

            lstInfo.Add(new SRMEventInfo()
            {
                Action = eventAction,
                Type = entityDisplayName,
                Key = entityCode,
                ID = entityCode,
                Attributes = attrList,
                Module = (SRMEventModule)moduleID,
                IsCreate = isCreate,
                Legs = legs,
                TimeStamp = loadingTime,
                User = userName,
                EffectiveStartDate = effectiveStartDate,
                Initiator = initiator,
                PendingAt = pendingAt
            });
            if (lstEventInfo != null && lstEventInfo.Count > 0)
                lstInfo.AddRange(lstEventInfo);
            SRMEventController.RaiseEvent(new SRMRaiseEventsInputInfo() { eventInfo = lstInfo, instrumentTypeId = entityTypeId, moduleId = moduleID });
        }
		
		public static DataTable GetUserGroupLayoutPriority(int moduleId)
        {
            mLogger.Debug("GetUserGroupLayoutPriority -> Start");
            string tempTableName = "IVPRefMaster.dbo.tempUserLayoutReport_" + Guid.NewGuid().ToString().Replace("-", "_");
            try
            {
                var lstUsers = new com.ivp.rad.RUserManagement.RUserManagementService().GetAllUsersGDPR();
                DataTable otUser = new DataTable();
                otUser.Columns.Add("user_login_name");
                otUser.Columns.Add("user_name");
                foreach (var user in lstUsers)
                {
                    var orUser = otUser.NewRow();
                    orUser["user_login_name"] = user.UserLoginName;
                    orUser["user_name"] = user.UserName;
                    otUser.Rows.Add(orUser);
                }

                CommonDALWrapper.ExecuteSelectQuery("CREATE TABLE " + tempTableName + "(user_login_name VARCHAR(MAX), user_name VARCHAR(MAX))", ConnectionConstants.SecMaster_Connection);
                CommonDALWrapper.ExecuteBulkUpload(tempTableName, otUser, ConnectionConstants.SecMaster_Connection);

                var dtResult = CommonDALWrapper.ExecuteSelectQuery("EXEC IVPRefMaster.dbo.REFM_GetUserGroupLayoutPriority '" + tempTableName + "',"+ moduleId + "", ConnectionConstants.SecMaster_Connection).Tables[0];
                return dtResult;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                try
                {
                    CommonDALWrapper.ExecuteSelectQuery("IF EXISTS(SELECT 1 FROM IVPRefMaster.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('" + tempTableName + "',1)) DROP TABLE " + tempTableName, ConnectionConstants.SecMaster_Connection);
                }
                catch (Exception ex)
                {

                }

                mLogger.Debug("GetUserGroupLayoutPriority -> End");
            }
        }

        public static void PrepareModalsInfo(RADXRuleGrammarInfo info, List<RADXRuleCustomOpParamsInfo> paramInfo, string ruleType)
        {
            DataSet customOperations = null;

            try
            {
                customOperations = PrepareCustomOperationsInfo(ruleType);

                Dictionary<string, RADXRuleCustomOpInfo> customOpInfo = new Dictionary<string, RADXRuleCustomOpInfo>();
                info.RADXRuleCustomOpInfo = customOpInfo;
                foreach (DataRow dr in customOperations.Tables[0].Rows)
                {
                    List<RADXRuleCustomOpParams> sequenceInfo = new List<RADXRuleCustomOpParams>();

                    bool isInputColPresent = customOperations.Tables[1].AsEnumerable().Any(x => x.Field<int>("operation_id") == dr.Field<int>("operation_id") && !x.IsNull("input_columns"));

                    foreach (DataRow drInner in customOperations.Tables[1].AsEnumerable().Where(x => x.Field<int>("operation_id") == dr.Field<int>("operation_id")))
                    {
                        RADXRuleCustomOpParams sequence = new RADXRuleCustomOpParams();
                        sequence.DataType = (RADXRuleDataType)Enum.Parse(typeof(RADXRuleDataType), drInner.Field<string>("data_type"));
                        string inputCols = Convert.ToString(drInner.Field<string>("input_columns"));
                        if (!string.IsNullOrEmpty(inputCols))
                            sequence.inputColumns = inputCols.Split(',').ToList();

                        if (!isInputColPresent)
                            sequence.inputColumns = null;

                        sequence.IsNullable = Convert.ToBoolean(drInner.Field<bool>("is_nullable"));
                        sequence.IsUserEditAllowed = Convert.ToBoolean(drInner.Field<bool>("is_user_edit_allowed"));
                        sequence.Sequence = Convert.ToInt32(drInner.Field<int>("sequence"));
                        sequenceInfo.Add(sequence);
                    }

                    RADXRuleCustomOpInfo modelInfo = new RADXRuleCustomOpInfo();
                    string assemblyName = AppDomain.CurrentDomain.BaseDirectory + dr.Field<string>("assembly_name") + ".dll";

                    if (!File.Exists(assemblyName))
                        assemblyName = AppDomain.CurrentDomain.BaseDirectory + "bin\\" + dr.Field<string>("assembly_name") + ".dll";

                    modelInfo.AssemblyName = assemblyName;
                    modelInfo.ClassName = dr.Field<string>("class_name");
                    modelInfo.ParameterSequenceInfo = sequenceInfo;
                    modelInfo.ParametersInfo = paramInfo;
                    modelInfo.ExecutionMode = RADXRuleExecutionMode.Order;
                    modelInfo.Name = dr.Field<string>("name");
                    modelInfo.dataType = (RADXRuleDataType)Enum.Parse(typeof(RADXRuleDataType), dr.Field<string>("data_type"));
                    customOpInfo.Add(dr.Field<string>("name"), modelInfo);

                }
                info.RADXRuleCustomOpInfoJson = customOpInfo.Values.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static DataSet PrepareCustomOperationsInfo(string ruleType)
        {
            mLogger.Debug("RMRuleEngineController: PrepareCustomOperationsInfo -> Start");
            DataSet dsResult = null;
            try
            {
                dsResult = CommonDALWrapper.ExecuteSelectQuery("select * from [IVPRefMaster].[dbo].[ivp_refm_custom_operation_master] where rule_type = '" + ruleType.ToString() + "' select od.* from [IVPRefMaster].[dbo].[ivp_refm_custom_operation_master] om inner join  [IVPRefMaster].[dbo].[ivp_refm_custom_operation_details] od on om.operation_id = od.operation_id where rule_type = '" + ruleType.ToString() + "'", ConnectionConstants.RefMaster_Connection);
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RDALException(dalEx.Message, dalEx);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                mLogger.Debug("RMRuleEngineController: PrepareCustomOperationsInfo -> End");
            }

            return dsResult;
        }
    }

    public class RMCommonStatic
    {

        #region Common reusable methods

        public static string ConvertToLower(object ob)
        {
            if (string.IsNullOrEmpty(Convert.ToString(ob).Trim()))
                return string.Empty;
            else
                return ob.ToString().ToLower().Trim();
        }

        #endregion

    }
}
