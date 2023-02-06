using com.ivp.commom.commondal;
using com.ivp.common.Migration;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.utils;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.DownstreamSystems
{
    public class RMDownstreamSystemsController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMController");
        public ObjectSet RMDownstreamSystemsDownload(int moduleID, List<int> systemIds, out string errorMsg)
        {
            mLogger.Debug("RMDownstreamController : RMDownstreamSystemsDownload() --> Start");
            RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
            errorMsg = "";
            try
            {
                DataSet dsResult = null;
                if (systemIds != null && systemIds.Count != 0)
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC [dbo].[REFM_GetDownstreamSystems_DownloadData] '{0}'", string.Join(",", systemIds)), mDBCon);
                else
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(@"EXEC [dbo].[REFM_GetDownstreamSystems_DownloadData] ''", mDBCon);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "Definition";
                    if (dsResult.Tables.Count > 1)
                        dsResult.Tables[1].TableName = "Report Mapping";
                }
                if (dsResult != null)
                    return SRMCommon.ConvertDataSetToObjectSet(dsResult);
                else return null;

            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                errorMsg = ex.Message.ToString();
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMDownstreamController : RMDownstreamSystemsDownload() --> End");
            }
        }
        public void RMDownstreamSystems_Sync(ObjectSet objSetFromDB, ObjectSet objSetDiff, string userName, out string errorMsg)
        {
            errorMsg = "";

            mLogger.Debug("RMDownstreamController : RMDownstreamSystems_Sync() --> START");

            List<string> SystemNamesInDb = objSetFromDB.Tables["Definition"].AsEnumerable().Select(row => row.Field<string>("System Name")).ToList();

            #region validation sheet1
            try
            {
                if (objSetDiff.Tables.Contains("Definition"))
                {

                    List<ObjectRow> validRowsToProcess = objSetDiff.Tables["Definition"].AsEnumerable().Where(row => string.IsNullOrEmpty(Convert.ToString(row[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList();
                    if (validRowsToProcess != null && validRowsToProcess.Count > 0)
                    {
                        foreach (var row in validRowsToProcess)
                        {

                            RMReportSystemManagementInfo objReportSystemInfo = null;

                            objReportSystemInfo = new RMReportSystemManagementInfo();
                            objReportSystemInfo.CreatedBy = userName;
                            objReportSystemInfo.ReportSystemDescription = Convert.ToString(row["Description"]);
                            objReportSystemInfo.ReportSystemName = Convert.ToString(row["System Name"]);
                            objReportSystemInfo.AssemblyPath = Convert.ToString(row["Assembly Path"]);
                            objReportSystemInfo.ClassName = Convert.ToString(row["Class Name"]);
                            objReportSystemInfo.Version = Convert.ToString(row["Version"]);

                            if (Convert.ToString(row["Report Attribute Level Audit"]).Equals("yes", StringComparison.OrdinalIgnoreCase) || Convert.ToString(row["Report Attribute Level Audit"]).Equals("true", StringComparison.OrdinalIgnoreCase))
                            {
                                objReportSystemInfo.RequireReportAttributeLevelAudit = true;
                                new RMCommonMigration().SetPassedRow(row, string.Empty);
                            }
                            else if (Convert.ToString(row["Report Attribute Level Audit"]).Equals("no", StringComparison.OrdinalIgnoreCase) || Convert.ToString(row["Report Attribute Level Audit"]).Equals("false", StringComparison.OrdinalIgnoreCase))
                            {
                                objReportSystemInfo.RequireReportAttributeLevelAudit = false;
                                new RMCommonMigration().SetPassedRow(row, string.Empty);
                            }
                            else
                            {

                                new RMCommonMigration().SetFailedRow(row, new List<string>() {  " Allowed Report Attribute Level Audit Values are true/false/yes/no" }, false);

                                continue;
                            }

                            objReportSystemInfo.LastModifiedBy = userName;
                            Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                            Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMReportSystemManagementController");
                            object obj = Activator.CreateInstance(objType);
                            if (SystemNamesInDb != null && SystemNamesInDb.Find(x => x.Equals(Convert.ToString(row["System Name"]),StringComparison.OrdinalIgnoreCase)) == null) //Create new system
                            {
                                //Create/save system
                                MethodInfo method = objType.GetMethod("GetReportSystemInfoByName", BindingFlags.Public | BindingFlags.Static);

                                RMReportSystemManagementInfo tmpRsInfo = (RMReportSystemManagementInfo)method.Invoke(obj, new object[] { objReportSystemInfo.ReportSystemName });

                                if ((tmpRsInfo == null) || (!tmpRsInfo.ReportSystemName.ToUpper().Equals(objReportSystemInfo.ReportSystemName.ToUpper())))
                                {

                                    MethodInfo methodAdd = objType.GetMethod("AddReportSystem", BindingFlags.Public | BindingFlags.Static);

                                    bool rtrnval = (bool)methodAdd.Invoke(obj, new object[] { objReportSystemInfo });

                                    if (rtrnval)
                                        new RMCommonMigration().SetPassedRow(row, string.Empty);
                                }
                                else
                                {
                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RM_DownstreamSystem_SheetNames.Definition + " Report System Name Already Exists!" }, false);

                                }
                            } //create system end

                            else //update existing system
                            {
                                if (objReportSystemInfo != null)
                                {
                                    Dictionary<string, int> systemNameVsIdFromDB = RMDownstreamSystemsDBManager.GetSytemNamesVsIdFromDb(userName);
                                    objReportSystemInfo.ReportSystemId = systemNameVsIdFromDB[objReportSystemInfo.ReportSystemName];
                                    objReportSystemInfo.ReportSystemName = SystemNamesInDb.Find(x => x.Equals(Convert.ToString(row["System Name"]), StringComparison.OrdinalIgnoreCase));
                                    MethodInfo methodupdate = objType.GetMethod("UpdateReportSystem", BindingFlags.Public | BindingFlags.Static);

                                    bool rtrnval = (bool)methodupdate.Invoke(obj, new object[] { objReportSystemInfo });
                                    if (rtrnval)
                                        new RMCommonMigration().SetPassedRow(row, string.Empty);
                                    else
                                    {
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RM_DownstreamSystem_SheetNames.Definition + " Sytsem Updation failed!" }, false);

                                    }
                                }
                            

                            
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                errorMsg = ex.Message.ToString();
                //throw ex;
            }
            finally
            {
                mLogger.Debug("RMDownstreamController : RMDownstreamSystems_Sync() --> Sheet1 : End");
            }
            #endregion
            #region sheet2 Mapping
            try
            {
                mLogger.Debug("RMDownstreamController : RMDownstreamSystems_Sync() --> Sheet2 : START");

                if (!objSetDiff.Tables.Contains("Report Mapping"))
                    return;
                List<ObjectRow> validRowsToProcess = objSetDiff.Tables["Report Mapping"].AsEnumerable().Where(row => string.IsNullOrEmpty(Convert.ToString(row[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList();

                if (validRowsToProcess != null && validRowsToProcess.Count > 0)
                {
                    RDBConnectionManager connManager = null;
                    Dictionary<string, int> systemNameVsIdFromDB = RMDownstreamSystemsDBManager.GetSytemNamesVsIdFromDb(userName);
                    Dictionary<string, int> reportNameVsID = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    Dictionary<string, List<string>> reportNameVsEntity = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                    RMDownstreamSystemsDBManager.GetReportsDataFromDB(reportNameVsID, reportNameVsEntity);

                    Dictionary<string, Dictionary<string, int>> systemVsEntityTypesMapped = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);
                    Dictionary<string, Dictionary<int, int>> SystemVSReportIDVsReportMapID = RMDownstreamSystemsDBManager.GetSystemReportMapping(systemVsEntityTypesMapped);

                    if (systemNameVsIdFromDB != null && systemNameVsIdFromDB.Count > 0)
                    {
                        RMReportSystemMappingInfo rMappingInfo = null;

                        Dictionary<string, List<ObjectRow>> GroupedsystemVsReports2 = validRowsToProcess.GroupBy(
                         x => x.Field<string>("System Name")).ToDictionary(x => x.Key, row => row.ToList(),StringComparer.OrdinalIgnoreCase);
                        Dictionary<string, List<string>> SystemVsentityTypesMappedInRows = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                        foreach (var ItemsystemVsReports in GroupedsystemVsReports2)
                        {
                            int hasErr = 0; int duplicateET = 0;
                            List<RMReportSystemMappingInfo> mappingList = new List<RMReportSystemMappingInfo>();

                            if (!systemNameVsIdFromDB.ContainsKey(ItemsystemVsReports.Key))
                            {
                                ItemsystemVsReports.Value.ForEach(row =>
                                    {
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RM_DownstreamSystem_SheetNames.Mapping + " : System does not exists! " }, false);

                                    }
                                    );
                                continue;
                            }
                            try
                            {
                                List<ObjectRow> validMappedRows = new List<ObjectRow>();
                                List<ObjectRow> passesRows = new List<ObjectRow>();
                                List<int> mappedReportsInDb = null;
                                if (SystemVSReportIDVsReportMapID.ContainsKey(ItemsystemVsReports.Key) && SystemVSReportIDVsReportMapID[ItemsystemVsReports.Key] != null)
                                    mappedReportsInDb = SystemVSReportIDVsReportMapID[ItemsystemVsReports.Key].Values.ToList();
                                if (!SystemVsentityTypesMappedInRows.ContainsKey(ItemsystemVsReports.Key))
                                    SystemVsentityTypesMappedInRows.Add(ItemsystemVsReports.Key, new List<string>() { });
                                Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                                if (ItemsystemVsReports.Value != null)
                                {
                                    var MappedRows = ItemsystemVsReports.Value;
                                    List<string> EntityTypesAlreadyMapped = new List<string>();
                                    foreach (var row in MappedRows)
                                    {
                                        string reportName = row.Field<string>("Report Name");
                                        if (!reportNameVsEntity.ContainsKey(reportName))
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RM_DownstreamSystem_SheetNames.Mapping + " : Invalid Report Name " }, false);
                                            hasErr = 1;
                                            break;
                                        }
                                       foreach(var et in reportNameVsEntity[reportName])
                                        {
                                            if (!EntityTypesAlreadyMapped.Contains(et))
                                                EntityTypesAlreadyMapped.Add(et);
                                            else
                                            {
                                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RM_DownstreamSystem_SheetNames.Mapping + " : Entity type mapping for report : " + reportName + " is confliceted !" }, false);
                                                hasErr = 1; duplicateET = 1;
                                                break;

                                            }
                                        }

                                        if (duplicateET == 1)
                                            break;
                                        int reportId = reportNameVsID[row.Field<string>(1)];

                                        //get attr from db for this report
                                        
                                        Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMReportAttributeController");
                                        object obj = Activator.CreateInstance(objType);



                                        MethodInfo methodAdd = objType.GetMethod("GetReportAttribute");



                                        DataSet rtrnvalDS = (DataSet)methodAdd.Invoke(obj, new object[] { reportId });
                                        var ReportAttributesVSID = rtrnvalDS.Tables[0].AsEnumerable().ToDictionary(y => y.Field<string>("report_attribute_name").Trim(), x => x.Field<int>("report_attribute_id"),StringComparer.OrdinalIgnoreCase);

                                        if (string.IsNullOrEmpty(Convert.ToString(row["Report Attribute"])))
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RM_DownstreamSystem_SheetNames.Mapping + "Select Attribute for " + Convert.ToString(row["Report Name"]) }, false);

                                            hasErr = 1;
                                            break;
                                        }
                                        else if (!ReportAttributesVSID.ContainsKey(Convert.ToString(row["Report Attribute"]).Trim()))
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RM_DownstreamSystem_SheetNames.Mapping + "Invalid Report Attribute" }, false);
                                            hasErr = 1;
                                            break;
                                        }

                                        validMappedRows.Add(row);
                                        rMappingInfo = new RMReportSystemMappingInfo();
                                        rMappingInfo.CreatedBy = userName;
                                        rMappingInfo.ReportAttributeId = ReportAttributesVSID[Convert.ToString(row["Report Attribute"])];
                                        rMappingInfo.ReportId = reportId;
                                        rMappingInfo.ReportSystemId = systemNameVsIdFromDB[ItemsystemVsReports.Key];
                                        mappingList.Add(rMappingInfo);
                                        passesRows.Add(row);

                                    }

                                }
                                if (hasErr == 1)
                                {
                                    ItemsystemVsReports.Value.ForEach(row =>
                                    {
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() {}, false);

                                    });
                                    continue;
                                }
                                //start transaction
                                connManager = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                                // delete all reports
                                if (mappedReportsInDb != null && mappedReportsInDb.Count > 0)
                                {
                                     
                                     Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMReportSystemMappingController");
                                    object obj = Activator.CreateInstance(objType);



                                    MethodInfo method = objType.GetMethod("DeleteReportsMapping", BindingFlags.Public | BindingFlags.Static);



                                    method.Invoke(obj, new object[] { mappedReportsInDb, connManager });
                           

                                }
                                List<ObjectRow> failedRows = new List<ObjectRow>();
                                if (mappingList != null && mappingList.Count > 0)
                                {
                                    Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMReportSystemMappingController");
                                    object obj = Activator.CreateInstance(objType);



                                    MethodInfo method = objType.GetMethod("AddReportsMapping", BindingFlags.Public | BindingFlags.Static);



                                    bool rtrnval = (bool)method.Invoke(obj, new object[] { mappingList, connManager });

                                    if (rtrnval)
                                    {
                                        passesRows.ForEach(row =>
                                            new RMCommonMigration().SetPassedRow(row, string.Empty)
                                           );
                                    }
                                    else
                                        passesRows.ForEach(
                                             row => new RMCommonMigration().SetFailedRow(row, new List<string>(), false));
                                }
                                if (connManager != null)
                                    connManager.CommitTransaction();

                            }
                            catch (Exception ex)
                            {
                                if (connManager != null)
                                    connManager.RollbackTransaction();
                                errorMsg = ex.Message.ToString();
                            }
                            finally
                            {
                                if (connManager != null)
                                    RDALAbstractFactory.DBFactory.PutConnectionManager(connManager);

                            }
                        }
                    }
                    else
                        throw new Exception(" No downstream systems configured to map! ");

                }

            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                errorMsg = ex.Message.ToString();
                //throw ex;
            }
            finally
            {
                mLogger.Debug("RMDownstreamController : RMDownstreamSystems_Sync() --> Sheet2 : End");
            }
            #endregion sheet2
            mLogger.Debug("RMDownstreamController : RMDownstreamSystems_Sync() --> END");
        }
    }
}
