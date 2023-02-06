using com.ivp.commom.commondal;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.utils;
using com.ivp.srmcommon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.SecMaster
{
    public class RMSectypeController
    {

        private Dictionary<string, List<ObjectRow>> GetSecurityLookupInfo(int secTypeId)
        {
            Dictionary<string, List<ObjectRow>> dictEntityTypeVsSecurityLookupMetaDataRows = new Dictionary<string, List<ObjectRow>>();
            ObjectTable dtSecurityLookupMetaData = new ObjectTable();

            string query = string.Format(@"SELECT DISTINCT entityList.entity_type_id, entityList.derived_from_entity_type_id,entityList.entity_type_name,
                            secLookup.parent_security_type_id,secLookup.child_entity_attribute_name,entityList.module_id
		                    FROM IVPRefMaster.dbo.ivp_refm_entity_type AS entityList
		                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute AS entityAttribute ON entityList.entity_type_id = entityAttribute.entity_type_id
		                    INNER JOIN IVPRefMaster.dbo.ivp_refm_security_attribute_lookup AS secLookup ON secLookup.child_entity_type_name = entityList.entity_type_name
		                    WHERE entityAttribute.data_type = 'SECURITY_LOOKUP' AND secLookup.parent_security_type_id IN (0,{0}) AND entityList.is_active = 1",
                            secTypeId);

            dtSecurityLookupMetaData = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection).Tables[0];

            foreach (ObjectRow dr in dtSecurityLookupMetaData.Rows)
            {
                string entityTypeName = Convert.ToString(dr["entity_type_name"]);
                List<ObjectRow> lst = null;
                if (!dictEntityTypeVsSecurityLookupMetaDataRows.TryGetValue(entityTypeName, out lst))
                {
                    lst = new List<ObjectRow>();
                    dictEntityTypeVsSecurityLookupMetaDataRows.Add(entityTypeName, lst);
                }
                lst.Add(dr);

            }

            return dictEntityTypeVsSecurityLookupMetaDataRows;
        }

        private void InsertSecIdsIntoTable(string securityTableName, string[] securityIds)
        {

            string createTableQuery = String.Format(@"CREATE TABLE {0}(sec_id varchar(100))
                                                    CREATE NONCLUSTERED INDEX IDX_{0}_sec_id ON {0}(sec_id)",
                                                    securityTableName);

            CommonDALWrapper.ExecuteSelectQuery(createTableQuery, ConnectionConstants.RefMaster_Connection);

            ObjectTable dtSecurityId = new ObjectTable();
            dtSecurityId.TableName = "SecurityList";
            dtSecurityId.Columns.Add("sec_id", typeof(string));

            foreach (var secId in securityIds)
            {
                dtSecurityId.Rows.Add(secId);
            }

            CommonDALWrapper.ExecuteBulkUploadObject(securityTableName, dtSecurityId, ConnectionConstants.RefMaster_Connection);
        }

        //Below function added based upon the fogbugz case id 678079
        public ObjectTable GetEntityCodeOfSecurityLookupAttribute(int secTypeId, string[] securityIds)
        {

            string securityTableName = Guid.NewGuid().ToString().Replace('-', '_');
            securityTableName = "securityId_" + securityTableName; // table name should not start with digit

            Dictionary<string, List<ObjectRow>> dictEntityTypeMetaDataRows = GetSecurityLookupInfo(secTypeId);
            InsertSecIdsIntoTable(securityTableName, securityIds);

            ObjectTable outputTable = new ObjectTable();
            outputTable.Columns.Add("security_id", typeof(string));
            outputTable.Columns.Add("entity_code", typeof(string));
            outputTable.Columns.Add("leg_entity_code", typeof(string));
            outputTable.Columns.Add("module_id", typeof(int));


            try
            {
                ConcurrentQueue<string> entityQueue = new ConcurrentQueue<string>(dictEntityTypeMetaDataRows.Keys);
                int[] threadsToProcess = new int[Environment.ProcessorCount];
                object objectLock = new object();
                Parallel.ForEach(threadsToProcess, i =>
                {

                    while (entityQueue.Count > 0)
                    {
                        string currentEntityTypeName;
                        if (entityQueue.TryDequeue(out currentEntityTypeName))
                        {
                            string execQuery = string.Empty;
                            string entityTableName = currentEntityTypeName;

                            foreach (ObjectRow dr in dictEntityTypeMetaDataRows[currentEntityTypeName])
                            {
                                string attrName = Convert.ToString(dr["child_entity_attribute_name"]);
                                int moduleId = Convert.ToInt32(dr["module_id"]);
                                int derivedFromEntityTypeId = Convert.ToInt32(dr["derived_from_entity_type_id"]);

                                if (derivedFromEntityTypeId == 0)
                                {
                                    execQuery += string.Format(@" SELECT {0} AS security_id, A.entity_code, NULL AS leg_entity_code, {1} AS module_id
                                                              FROM IVPRefMaster.dbo.{2} AS A 
                                                              INNER JOIN {3} AS B ON A.{0} = B.sec_id union",
                                                          attrName, moduleId, entityTableName, securityTableName);

                                }
                                else
                                {
                                    execQuery += string.Format(@" SELECT {0} AS security_id, A.entity_code, A.master_entity_code AS leg_entity_code, {1}  AS module_id
                                                              FROM IVPRefMaster.dbo.{2} AS A 
                                                              INNER JOIN {3} AS B on A.{0} = B.sec_id union",
                                                          attrName, moduleId, entityTableName, securityTableName);
                                }

                            }
                            execQuery = execQuery.Substring(0, execQuery.Length - 6);
                            ObjectTable returnOutput = CommonDALWrapper.ExecuteSelectQueryObject(execQuery, ConnectionConstants.RefMaster_Connection).Tables[0];
                            lock (objectLock)
                            {
                                foreach (ObjectRow curRow in returnOutput.Rows)
                                {
                                    outputTable.ImportRow(curRow);
                                }
                            }
                        }
                    }

                });
            }
            finally
            {
                // delete the created table
                CommonDALWrapper.ExecuteSelectQuery(string.Format(@"drop table {0}", securityTableName), ConnectionConstants.RefMaster_Connection);
            }

            return outputTable;
        }

        public bool IsSecMasterAvailable
        {
            get
            {
                string modulesStr = RConfigReader.GetConfigAppSettings("AVAILABLE_MODULES");
                string productName = RConfigReader.GetConfigAppSettings("ProductName");
                if (!string.IsNullOrEmpty(modulesStr))
                {
                    var modules = modulesStr.Split(',').ToList();
                    //if (modules.Contains("3") || (modules.Contains("-1") && !string.IsNullOrEmpty(productName) && productName.Equals("secmaster",StringComparison.OrdinalIgnoreCase)))
                    if (modules.Contains("3") || (modules.Contains("-1") && !string.IsNullOrEmpty(productName) && productName.Equals("secmaster", StringComparison.OrdinalIgnoreCase)))
                        return true;
                }
                else if (!string.IsNullOrEmpty(productName) && productName.Equals("secmaster", StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            }
        }

        public Dictionary<string, SecurityTypeMasterInfo> GetSectypeAttributes(bool requireCurveAttributes)
        {
            if (IsSecMasterAvailable)
                return new RMSectypeAPI().GetSectypeAttributes(requireCurveAttributes);
            else
                return new Dictionary<string, SecurityTypeMasterInfo>();
        }

        public void MassageSecTypeAndAttributes(IEnumerable<ObjectRow> dtInput, SRMSecTypeMassageInputInfo inputInfo, RDBConnectionManager mDBCon = null)
        {
            if (IsSecMasterAvailable)
                new RMSectypeAPI().MassageSecTypeAndAttributes(dtInput, inputInfo, mDBCon);
        }

        public Dictionary<string, AttrInfo> FetchCommonAttributes(bool requireCurveAttributes)
        {
            if (IsSecMasterAvailable)
                return new RMSectypeAPI().FetchCommonAttributes(requireCurveAttributes);
            else
                return new Dictionary<string, AttrInfo>();
        }
    }
}
