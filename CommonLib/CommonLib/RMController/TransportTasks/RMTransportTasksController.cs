using com.ivp.commom.commondal;
using com.ivp.common.Migration;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.rad.utils;
using com.ivp.rad.raduicontrols;
using System.Reflection;
using System.Globalization;

namespace com.ivp.common.TransportTasks
{
    public class RMTransportTasksController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMController");
        public ObjectSet RMTransportTasksDownload(int moduleID, List<int> taskIds, out string errorMsg)
        {
            mLogger.Debug("RMTransportTasksController : RMTransportTasksDownload() --> Start");
            RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
            errorMsg = "";
            try
            {
                DataSet dsResult = null;
                if (taskIds != null && taskIds.Count != 0)
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC [dbo].[REFM_TransportTaskSetup_Download] '{0}'", string.Join(",", taskIds)), mDBCon);
                else
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(@"EXEC [dbo].[REFM_TransportTaskSetup_Download] ''", mDBCon);
                if (dsResult != null && dsResult.Tables.Count > 1)
                {
                    IDictionary<string, string> dateTypes = null;
                    dateTypes = (IDictionary<string, string>)com.ivp.rad.raduicontrols.RUIConfigLoader.GetSetupConfiguration("CommonDateTypes");
                    foreach (var row in dsResult.Tables[0].AsEnumerable())
                    {
                        string dateType = dateTypes[Convert.ToString(row["File Date Type"])];
                        row["File Date Type"] = dateType;
                        if (Convert.ToBoolean(row["Use Default path"]) == true)
                            row["Local File Location"] = "";
                    }
                    //var AllGroups = SRMCommonRAD.GetAllGroups();
                    dsResult.Tables[0].TableName = "Definition";
                    if (dsResult.Tables.Count > 1)
                        dsResult.Tables[1].TableName = "Custom Classes";
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
                mLogger.Debug("RMTransportTasksController : RMTransportTasksDownload() --> End");
            }
        }


        public void RMTransportTasks_Sync(ObjectSet objSetFromDB, ObjectSet objSetDiff, string userName, string dateformat, out string errorMsg)
        {
            mLogger.Debug("RMTransportTasksController : RMTransportTasks_Sync() --> Start");
            errorMsg = "";
            try
            {
                Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                Type objTransportTypeControllerType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMTransportTaskController");
                object objTransportTypeController = Activator.CreateInstance(objTransportTypeControllerType);

                MethodInfo GetTransports = objTransportTypeControllerType.GetMethod("GetRefMTransports");

                DataSet transportTypesds = (DataSet)GetTransports.Invoke(objTransportTypeController, null);
                List<string> transportTypes = transportTypesds.Tables[0].AsEnumerable().Select(x => Convert.ToString(x["Name"])).ToList();

                #region sheet 1
                try
                {
                    mLogger.Debug("SMMigrationController : SMTransportTasks_Sync():sheet 1 --> Start");
                    RDBConnectionManager mDbConn_Vendor = null;

                    if (objSetDiff.Tables.Contains(RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition))
                    {
                        Dictionary<string, List<ObjectRow>> tasksVSTransportsInDB = objSetFromDB.Tables[RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition].AsEnumerable().GroupBy(x => Convert.ToString(x["Task Name"])).ToDictionary(y => y.Key, y => y.ToList(), StringComparer.OrdinalIgnoreCase);
                        List<ObjectRow> validRowsToProcess = objSetDiff.Tables[RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition].AsEnumerable().Where(row => string.IsNullOrEmpty(Convert.ToString(row[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList();
                        Dictionary<string, int> taskNameVsID = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);


                        if (validRowsToProcess != null && validRowsToProcess.Count > 0)
                        {
                            var DsTaskTransports = SRMCommon.ConvertDataSetToObjectSet(TransportTasksDBManager.getIdsFromDb(taskNameVsID));
                            Dictionary<string, List<ObjectRow>> taskVsTransport = validRowsToProcess.GroupBy(x => Convert.ToString(x["Task Name"]), StringComparer.OrdinalIgnoreCase).ToDictionary(y => y.Key, y => y.ToList(), StringComparer.OrdinalIgnoreCase);
                            Dictionary<string, List<ObjectRow>> taskVsTransportRowInDb = null;
                            if (DsTaskTransports != null && DsTaskTransports.Tables.Count > 0)
                                taskVsTransportRowInDb = DsTaskTransports.Tables[0].AsEnumerable().GroupBy(x => Convert.ToString(x["task_name"]), StringComparer.OrdinalIgnoreCase).ToDictionary(y => y.Key, y => y.ToList(), StringComparer.OrdinalIgnoreCase);

                            //get valid file date types
                            var dateTypeIDVsName = RUIConfigLoader.GetSetupConfiguration("CommonDateTypes").ToDictionary(x => x.Key, y => y.Value.ToUpper());
                            List<string> dateTypes = dateTypeIDVsName.Values.ToList();

                            var objTaskSumamryControllerType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMTaskSummaryController");
                            object objTaskSumamryController = Activator.CreateInstance(objTaskSumamryControllerType);
                            MethodInfo GetTaskSummaryByTaskType = objTaskSumamryControllerType.GetMethod("GetTaskSummaryByTaskType");
                            DataSet dtTaskSummaryResult = (DataSet)GetTaskSummaryByTaskType.Invoke(objTaskSumamryController, new object[] { RMTransportTasksConstants.TASK_TYPE_ID });
                            DataTable dtTaskSummary = dtTaskSummaryResult.Tables[0];
                            RMTaskInfo objTaskSummaryInfo = null;

                            foreach (var task in taskVsTransport)
                            {
                                try
                                {
                                    RMTransportTaskInfo objTransportTaskInfo = null;
                                    List<RMTransportTaskInfo> lstValidRowsTransportCreateInfo = new List<RMTransportTaskInfo>();
                                    Dictionary<string, List<ObjectRow>> TaskVsCustomClassesRow = null;
                                    List<TransportTaskPk> LstTransportTasksPK = new List<TransportTaskPk>();
                                    Dictionary<TransportTaskPk, List<int>> transportPkVsCCId = new Dictionary<TransportTaskPk, List<int>>();
                                    ObjectSet TaskCustomClassesDs = null;
                                    string taskDesc = Convert.ToString(task.Value[0]["Description"]);
                                    //**start transaction
                                    mDbConn_Vendor = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMasterVendor_Connection, true, System.Data.IsolationLevel.RepeatableRead);

                                    objTaskSummaryInfo = new RMTaskInfo();

                                    objTaskSummaryInfo.LastModifiedBy = userName;
                                    objTaskSummaryInfo.TaskDescription = string.IsNullOrEmpty(taskDesc) ? string.Empty : Convert.ToString(task.Value[0]["Description"]);
                                    objTaskSummaryInfo.TaskName = task.Key;
                                    objTaskSummaryInfo.CreatedBy = userName;
                                    objTaskSummaryInfo.DependentId = 0;
                                    objTaskSummaryInfo.TaskTypeId = RMTransportTasksConstants.TASK_TYPE_ID;

                                    //task does not exist ==> create task
                                    if (!taskNameVsID.ContainsKey(task.Key))
                                    {
                                        int taskMasterId = new RMDataSourceDBManager(mDbConn_Vendor).AddTaskSummaryDetails(objTaskSummaryInfo, mDbConn_Vendor);
                                        taskNameVsID.Add(objTaskSummaryInfo.TaskName, taskMasterId);

                                    }
                                    else //else update UpdateTaskSummaryDetails
                                    {
                                        objTaskSummaryInfo.TaskMasterId = taskNameVsID[task.Key];
                                        new RMDataSourceDBManager(mDbConn_Vendor).UpdateTaskSummaryDetails(objTaskSummaryInfo, mDbConn_Vendor);
                                        //***get cc wrt pk FROM DB
                                        TaskCustomClassesDs = SRMCommon.ConvertDataSetToObjectSet(TransportTasksDBManager.getTaskCustomClasses(taskNameVsID[task.Key], mDbConn_Vendor));
                                        if (TaskCustomClassesDs != null && TaskCustomClassesDs.Tables.Count > 0 && TaskCustomClassesDs.Tables[0].Rows.Count > 0)
                                        {
                                            TaskVsCustomClassesRow = TaskCustomClassesDs.Tables[0].AsEnumerable().GroupBy(x => Convert.ToString(x["Task Name"])).ToDictionary(y => y.Key, row => row.ToList());
                                            int indexToAdd = -1;
                                            TaskVsCustomClassesRow[task.Key].ForEach(
                                                row =>
                                                {
                                                    var ObjTransportTaskPk = new TransportTaskPk();
                                                    ObjTransportTaskPk.taskName = Convert.ToString(row["Task Name"]);
                                                    ObjTransportTaskPk.transportType = Convert.ToString(row["Transport type"]).ToUpper();
                                                    ObjTransportTaskPk.RemoteFile = Convert.ToString(row["Remote File Name"]);
                                                    ObjTransportTaskPk.RemoteFileLoc = Convert.ToString(row["Remote File Location"]);
                                                    ObjTransportTaskPk.LocalFile = Convert.ToString(row["Local File Name"]);
                                                    LstTransportTasksPK.Add(ObjTransportTaskPk);
                                                    int index = -1;
                                                    //if(!LstTransportTaskPkContainsObj(TransportPKVsCustomClassId.Keys.ToList(),ObjTransportTaskPk))
                                                    //    TransportPKVsCustomClassId.Add(ObjTransportTaskPk, new List<int>() { });
                                                    if (!TransportTasksDBManager.LstTransportTaskPkContainsObj(transportPkVsCCId.Keys.ToList(), ObjTransportTaskPk, ref index))
                                                    { transportPkVsCCId.Add(ObjTransportTaskPk, new List<int>() { }); indexToAdd++; index = indexToAdd; }
                                                    transportPkVsCCId[transportPkVsCCId.Keys.ElementAt(index)].Add(row.Field<int>("custom_class_id"));
                                                });

                                        }
                                    }
                                    bool hasErr = false; string TransportTypeForSync = null;
                                    string defaultPath = RConfigReader.GetFullDirectoryPath("DefaultFilePath");
                                    foreach (var row in task.Value)
                                    {
                                        if (Convert.ToString(row["Description"]).ToLower() != taskDesc.ToLower())
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Task Description of task for this transport is not in sync with original task!" }, false);
                                            hasErr = true;
                                            break;
                                        }
                                        if (!transportTypes.SRMContainsWithIgnoreCase(Convert.ToString(row["Transport Type"])))
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Invalid Transport Type!" }, false);
                                            hasErr = true;
                                            break;
                                        }
                                        else
                                        {
                                            TransportTypeForSync = transportTypes.First(x => x.ToLower() == Convert.ToString(row["Transport Type"]).ToLower());
                                        }
                                        string FileDateType = Convert.ToString(row["File Date Type"]).ToUpper();
                                        if (string.IsNullOrEmpty(FileDateType))
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : File Date Type Can not be empty! (use value None)" }, false);
                                            hasErr = true;
                                            break;
                                        }
                                        else if (!string.IsNullOrEmpty(FileDateType) && !dateTypes.Contains(FileDateType))
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Invalid File Date Type!" }, false);
                                            hasErr = true;
                                            break;
                                        }
                                        if (Convert.ToBoolean(row["Use Default path"]) == true)
                                        {
                                            if (!string.IsNullOrEmpty(Convert.ToString(row["Local File Location"])))
                                            {
                                                //if ((!Convert.ToString(row["Local File Location"]).EndsWith("\\") && Convert.ToString(row["Local File Location"]) + "\\" != defaultPath) || (Convert.ToString(row["Local File Location"]).EndsWith("\\") && Convert.ToString(row["Local File Location"]) != defaultPath))
                                                //{
                                                //    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Invalid Default path in Local File Location !" }, false);
                                                //    hasErr = true;
                                                //    break;
                                                //}
                                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Local File location should be blank if use default path is set to true!" }, false);
                                                hasErr = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(Convert.ToString(row["Local File Location"])))
                                            {
                                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Local File Location can not be Empty!" }, false);
                                                hasErr = true;
                                                break;
                                            }
                                        }
                                        string GPGKeyUserName = Convert.ToString(row["GPG Key User Name"]); string gpgPhrase = Convert.ToString(row["GPG Key PassPhrase"]);
                                        if ((string.IsNullOrEmpty(GPGKeyUserName) && !string.IsNullOrEmpty(gpgPhrase)) ||
                                            (!string.IsNullOrEmpty(GPGKeyUserName) && string.IsNullOrEmpty(gpgPhrase))
                                            )
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : GPG Key User Name OR GPG Key Phrase can not be empty!" }, false);
                                            hasErr = true;
                                            break;
                                        }
                                        var drArr = dtTaskSummary.AsEnumerable().Select(r => r.Field<int>("task_master_id") == taskNameVsID[task.Key]);

                                        //Create info for transport
                                        objTransportTaskInfo = new RMTransportTaskInfo();



                                        if (!string.IsNullOrEmpty(FileDateType))
                                        {
                                            if (FileDateType == "T-N")
                                            {
                                                if (Convert.ToString(row["Custom Value File Date type"]) == string.Empty)
                                                {
                                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Custom Value File Date type can not be empty!" }, false);
                                                    hasErr = true;
                                                    break;
                                                }
                                                else
                                                {
                                                    int x;
                                                    string rowVal = Convert.ToString(row["Custom Value File Date type"]);
                                                    if (int.TryParse(rowVal, out x) && x > 0)
                                                        objTransportTaskInfo.FileDateDays = Convert.ToInt32(row["Custom Value File Date type"]);
                                                    else
                                                    {
                                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Custom Value File Date type must be a number!" }, false);
                                                        hasErr = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (FileDateType == "CUSTOM")
                                            {
                                                DateTime date = DateTime.Now;
                                                if (!string.IsNullOrEmpty(Convert.ToString(row["Custom Value File Date type"])))
                                                {
                                                    try
                                                    {
                                                        date = DateTime.ParseExact(Convert.ToString(row["Custom Value File Date type"]).Split(' ')[0], dateformat, CultureInfo.InvariantCulture);

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Custom Date format should be " + dateformat }, false);
                                                        hasErr = true;
                                                    }
                                                    if (hasErr == true)
                                                        break;
                                                }
                                                else
                                                {
                                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Custom Value File Date type can not be empty!" }, false);
                                                    hasErr = true;
                                                    break;
                                                }
                                                objTransportTaskInfo.CustomDate = date.Date.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(Convert.ToString(row["Custom Value File Date type"])))
                                                {
                                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.Definition + " : Custom Value File Date type should be empty!" }, false);
                                                    hasErr = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (Convert.ToBoolean(row["Use Default path"]) == true)
                                        {
                                            //if (defaultPath.EndsWith("\\"))
                                            //    objTransportTaskInfo.LocalFileLocation = defaultPath.Remove(defaultPath.Length - 1);
                                            //else
                                            objTransportTaskInfo.LocalFileLocation = "";
                                        }
                                        else
                                        {
                                            string localFileLoc = Convert.ToString(row["Local File Location"]);
                                            if (localFileLoc.EndsWith("\\"))
                                                objTransportTaskInfo.LocalFileLocation = localFileLoc.Remove(localFileLoc.Length - 1);
                                            else
                                                objTransportTaskInfo.LocalFileLocation = localFileLoc;
                                        }
                                        string remoteFileLoc = Convert.ToString(row["Remote File Location"]);
                                        if (remoteFileLoc.EndsWith("\\"))
                                            objTransportTaskInfo.RemoteFileLocation = remoteFileLoc.Remove(remoteFileLoc.Length - 1);
                                        else
                                            objTransportTaskInfo.RemoteFileLocation = remoteFileLoc;

                                        objTransportTaskInfo.TransportName = TransportTypeForSync;
                                        objTransportTaskInfo.TransportMasterId = taskNameVsID[task.Key];
                                        objTransportTaskInfo.RemoteFileName = Convert.ToString(row["Remote File Name"]);
                                        //objTransportTaskInfo.RemoteFileLocation = Convert.ToString(row["Remote File Location"]);
                                        objTransportTaskInfo.LocalFileName = Convert.ToString(row["Local File Name"]);

                                        objTransportTaskInfo.UseDefaultPath = Convert.ToBoolean(row["Use Default path"]);
                                        objTransportTaskInfo.ExtractAll = Convert.ToBoolean(row["Extract All Files"]);
                                        objTransportTaskInfo.GpgUserName = Convert.ToString(row["GPG Key User Name"]);
                                        objTransportTaskInfo.GpgPassPhrase = Convert.ToString(row["GPG key PassPhrase"]);
                                        objTransportTaskInfo.CreatedBy = userName;
                                        objTransportTaskInfo.FileDateType = dateTypeIDVsName.First(x => x.Value == FileDateType).Key;
                                        objTransportTaskInfo.State = Convert.ToString(row["State"]).ToLower() == "on" ? true : false;

                                        lstValidRowsTransportCreateInfo.Add(objTransportTaskInfo);

                                    }
                                    if (hasErr == true)
                                    {
                                        foreach (var row in task.Value)
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { }, false);

                                        }
                                        continue;
                                    }
                                    //delete all transports of this task from db if exists
                                    if (taskVsTransportRowInDb != null && taskVsTransportRowInDb.ContainsKey(task.Key) && taskVsTransportRowInDb[task.Key].Count > 0)
                                    {
                                        taskVsTransportRowInDb[task.Key].ForEach(row =>
                                        {
                                            var mList = new RHashlist();
                                            if (row != null && row["transport_details_id"] != System.DBNull.Value)
                                            {
                                                int transportDetailsId = row.Field<int>("transport_details_id");
                                                mList.Add("transport_details_id", transportDetailsId);
                                                CommonDALWrapper.ExecuteQuery("REFM:Delete_ivp_refm_transport_task_details", mList, mDbConn_Vendor);
                                            }
                                        }
                                     );
                                    }
                                    //CREATE TRANSPORTS
                                    MethodInfo AddTransportTaskDetails = objTransportTypeControllerType.GetMethod("AddTransportTaskDetails");

                                    lstValidRowsTransportCreateInfo.ForEach(
                                        obj =>
                                        {
                                            var dsTransport = new DataSet();

                                            int transportDetailsIdRtrnVal = (int)AddTransportTaskDetails.Invoke(objTransportTypeController, new Object[] { obj, mDbConn_Vendor });
                                            //update dependendent id
                                            if (mDbConn_Vendor != null && transportDetailsIdRtrnVal != -1)
                                            {
                                                MethodInfo UpdateDependentIDMethod = objTaskSumamryControllerType.GetMethod("UpdateDependentId");
                                                UpdateDependentIDMethod.Invoke(objTaskSumamryController, new object[] { transportDetailsIdRtrnVal, taskNameVsID[task.Key], mDbConn_Vendor });
                                                //CommonDALWrapper.ExecuteQuery("REFMVendor:UpdateDepIdTaskSummary", mList, mDbConn_Vendor);

                                                //if(obj.State == false)
                                                //    {
                                                //        MethodInfo ChangeState = objTransportTypeControllerType.GetMethod("ChangeStateofTask");
                                                //        ChangeState.Invoke(objTransportTypeController, new object[] { transportDetailsIdRtrnVal, mDbConn_Vendor });
                                                //    }
                                                TransportTaskPk TransportFromSheet = new TransportTaskPk();
                                                TransportFromSheet.taskName = task.Key;
                                                TransportFromSheet.RemoteFile = obj.RemoteFileName;
                                                TransportFromSheet.RemoteFileLoc = obj.RemoteFileLocation;
                                                TransportFromSheet.LocalFile = obj.LocalFileName;
                                                TransportFromSheet.transportType = obj.TransportName;

                                                int index = -1;
                                                if (TransportTasksDBManager.LstTransportTaskPkContainsObj(transportPkVsCCId.Keys.ToList(), TransportFromSheet, ref index))
                                                {
                                                    transportPkVsCCId[transportPkVsCCId.Keys.ElementAt(index)].ForEach(cc =>
                                                    {
                                                        CommonDALWrapper.ExecuteSelectQuery(@"Update IVPRefMasterVendor.dbo.ivp_refm_custom_class SET is_active = 1,task_details_id = " + transportDetailsIdRtrnVal + " WHERE custom_class_id = " + cc, mDbConn_Vendor);

                                                    });
                                                }
                                            }
                                        }
                                        );
                                    //**commit trancation
                                    if (mDbConn_Vendor != null && hasErr == false)
                                    {
                                        task.Value.ForEach(row =>
                                                new RMCommonMigration().SetPassedRow(row, string.Empty)
                                        );
                                        mDbConn_Vendor.CommitTransaction();
                                    }
                                    else
                                        if (mDbConn_Vendor != null && hasErr == true)
                                        mDbConn_Vendor.RollbackTransaction();
                                }
                                catch (Exception ex)
                                {
                                    //**rollback
                                    if (mDbConn_Vendor != null)
                                        mDbConn_Vendor.RollbackTransaction();
                                    mLogger.Error(ex.ToString());
                                    errorMsg = ex.Message.ToString();

                                }
                                finally
                                {
                                    if (mDbConn_Vendor != null)
                                    {
                                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn_Vendor);
                                        mDbConn_Vendor = null;
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

                }
                finally
                {

                    mLogger.Debug("RMTransportTasksController : RMTransportTasks_Sync() : sheet1 --> End");
                }
                #endregion sheet1
                #region sheet2

                mLogger.Debug("SMMigrationController : RMTransportTasks_Sync() : sheet2 --> Start");
                if (objSetDiff.Tables.Contains(RMTransportTasksConstants.RM_TransportTask_SheetNames.CustomClasses))
                {
                    //RMCustomClassController objCustomClassController = null;
                    RMCustomClassInfo objCustomClassInfo = null;


                    string message = string.Empty;
                    try
                    {
                        List<ObjectRow> validRowsToProcess = objSetDiff.Tables[RMTransportTasksConstants.RM_TransportTask_SheetNames.CustomClasses].AsEnumerable().Where(row => string.IsNullOrEmpty(Convert.ToString(row[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(row[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList();

                        RDBConnectionManager connManager = null;
                        if (validRowsToProcess != null && validRowsToProcess.Count > 0)
                        {
                            var PKVsValidRows = validRowsToProcess.GroupBy(x => new { task_name = Convert.ToString(x["Task Name"]).ToLower(), transport = Convert.ToString(x["Transport Type"]).ToUpper(), rf_name = Convert.ToString(x["Remote File Name"]).ToLower(), rf_loc = Convert.ToString(x["Remote File Location"]).ToLower(), lf_name = Convert.ToString(x["Local File Name"]).ToLower() });
                            Dictionary<string, int> taskNameVsID = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                            var DsTaskTransports = SRMCommon.ConvertDataSetToObjectSet(TransportTasksDBManager.getTaskAndTransportIdsFromDB(taskNameVsID));
                            if (taskNameVsID.Count == 0 || DsTaskTransports.Tables[0].Rows.Count < 1)
                            {
                                foreach (var pkvsRows in PKVsValidRows) //task + transport wise
                                {
                                    foreach (var row in pkvsRows)
                                    {
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.CustomClasses + " : Transport task does not exists to create custom class! " }, false);
                                    }
                                }
                                return;
                            }

                            Dictionary<string, List<ObjectRow>> taskVsTransportRowInDb = null;
                            if (DsTaskTransports != null && DsTaskTransports.Tables.Count > 0)
                                taskVsTransportRowInDb = DsTaskTransports.Tables[0].AsEnumerable().GroupBy(x => Convert.ToString(x["task_name"]), StringComparer.OrdinalIgnoreCase).ToDictionary(y => y.Key, y => y.ToList(), StringComparer.OrdinalIgnoreCase);

                            foreach (var pkvsRows in PKVsValidRows)
                            {
                                var pkValidRows = pkvsRows.Key;
                                if (!taskNameVsID.ContainsKey(pkValidRows.task_name))
                                {
                                    foreach (var row in pkvsRows)
                                    {
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.CustomClasses + " : Task Name does not exists! " }, false);

                                    }
                                    continue;
                                }
                                var pkVsTrasnportINDB = taskVsTransportRowInDb[pkValidRows.task_name].GroupBy(x => new { task_name = Convert.ToString(x["task_name"]).ToLower(), transport = Convert.ToString(x["transport_name"]).ToUpper(), rf_name = Convert.ToString(x["remote_file_name"]).ToLower(), rf_loc = Convert.ToString(x["remote_file_location"]).ToLower(), lf_name = Convert.ToString(x["local_file_name"]).ToLower() });


                                bool validTransport = false;
                                foreach (var pk in pkVsTrasnportINDB)
                                {
                                    if (pk.Key.transport.ToUpper() == pkValidRows.transport
                                     && pk.Key.rf_name.ToLower() == pkValidRows.rf_name
                                     && pk.Key.rf_loc.ToLower() == pkValidRows.rf_loc
                                     && pk.Key.lf_name.ToLower() == pkValidRows.lf_name)
                                    {
                                        validTransport = true;
                                        break;
                                    }
                                }
                                if (validTransport == false)
                                {
                                    foreach (var row in pkvsRows)
                                    {
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.CustomClasses + " : Transport does not exists! " }, false);
                                    }
                                    continue;
                                }

                                Dictionary<ObjectRow, TransportTaskPk> ValidCustomClassInfoRows2 = new Dictionary<ObjectRow, TransportTaskPk>();
                                List<string> customClassesAlreadyAdded = new List<string>();
                                //***get cc wrt pk FROM DB
                                DataSet dsCustomClasses = TransportTasksDBManager.getTaskCustomClasses(taskNameVsID[pkValidRows.task_name]);
                                //**transport pk VS Details ID
                                Dictionary<TransportTaskPk, int> transportPkVsId = new Dictionary<TransportTaskPk, int>();

                                taskVsTransportRowInDb[pkValidRows.task_name].ForEach(
                                  a =>
                                  {
                                      var PKObj = new TransportTaskPk();
                                      PKObj.taskName = pkValidRows.task_name;
                                      PKObj.transportType = Convert.ToString(a["transport_name"]).ToUpper();
                                      PKObj.RemoteFile = Convert.ToString(Convert.ToString(a["remote_file_name"]));
                                      PKObj.RemoteFileLoc = Convert.ToString(a["remote_file_location"]);
                                      PKObj.LocalFile = Convert.ToString(a["local_file_name"]);
                                      int i = 0;
                                      if (!TransportTasksDBManager.LstTransportTaskPkContainsObj(transportPkVsId.Keys.ToList(), PKObj, ref i))
                                          transportPkVsId.Add(PKObj, Convert.ToInt32(a["transport_details_id"]));
                                  }
                                  );

                                bool hasErr = false;
                                foreach (var row in pkvsRows) //ON SHEET
                                {
                                    // validate task name
                                    if (!taskNameVsID.ContainsKey(Convert.ToString(row["Task Name"])))
                                    {
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.CustomClasses + " : Task Name does not exists! " }, false);
                                        hasErr = true;
                                        break;
                                    } //validate transport
                                    if (!transportTypes.SRMContainsWithIgnoreCase(Convert.ToString(row["Transport Type"])))
                                    {
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.CustomClasses + " Transport Type does not exists! " }, false);
                                        hasErr = true;
                                        break;
                                    }
                                    if (Convert.ToString(row["Class Type"]).ToUpper() == "CUSTOM CLASS" && (row["Assembly Path"] == System.DBNull.Value || string.IsNullOrEmpty(Convert.ToString(row["Assembly Path"]))))
                                    {
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.CustomClasses + " : Assembly Path Can not be empty! " }, false);
                                        hasErr = true;
                                        break;
                                    }
                                    //create pk obj to verify complete PK
                                    var ObjTransportTaskPk = new TransportTaskPk();
                                    ObjTransportTaskPk.taskName = Convert.ToString(row["Task Name"]);
                                    ObjTransportTaskPk.transportType = Convert.ToString(row["Transport type"]).ToUpper();
                                    ObjTransportTaskPk.RemoteFile = Convert.ToString(row["Remote File Name"]);
                                    ObjTransportTaskPk.RemoteFileLoc = Convert.ToString(row["Remote File Location"]);
                                    ObjTransportTaskPk.LocalFile = Convert.ToString(row["Local File Name"]);
                                    int i = 0;
                                    if (!TransportTasksDBManager.LstTransportTaskPkContainsObj(transportPkVsId.Keys.ToList(), ObjTransportTaskPk, ref i)) // if transport pk is not correct
                                    {
                                        foreach (var r in pkvsRows)
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { RMTransportTasksConstants.RM_TransportTask_SheetNames.CustomClasses + " : Invalid Data ! Remote File Name/Remote File Location/Local File Name is not found. " }, false);

                                        }
                                        hasErr = true;
                                        break;
                                    }


                                    ValidCustomClassInfoRows2.Add(row, ObjTransportTaskPk);
                                }

                                try
                                {
                                    if (hasErr == true)
                                    {
                                        foreach (var row in pkvsRows)
                                        {
                                            new RMCommonMigration().SetFailedRow(row, new List<string>() { }, false);

                                        }
                                        continue;
                                    }
                                    //**start transaction
                                    connManager = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMasterVendor_Connection, true, System.Data.IsolationLevel.RepeatableRead);
                                    //**delete all cc if exists
                                    if (dsCustomClasses != null && dsCustomClasses.Tables.Count > 0 && dsCustomClasses.Tables[0].Rows.Count > 0) // no cc mapped
                                    {
                                        foreach (var cc in dsCustomClasses.Tables[0].AsEnumerable().GroupBy(x => new { task_name = Convert.ToString(x["Task Name"]), transport = Convert.ToString(x["Transport Type"]), rf_name = Convert.ToString(x["Remote File Name"]), rf_loc = Convert.ToString(x["Remote File Location"]), lf_name = Convert.ToString(x["Local File Name"]) }).AsEnumerable())
                                        {
                                            if (cc.Key.transport.ToUpper() == pkValidRows.transport
                                          && cc.Key.rf_name.ToLower() == pkValidRows.rf_name
                                          && cc.Key.rf_loc.ToLower() == pkValidRows.rf_loc
                                          && cc.Key.lf_name.ToLower() == pkValidRows.lf_name) // dlt cc of that transport only
                                            {
                                                foreach (var a in cc)
                                                {
                                                    int customClassId = Convert.ToInt32(a.Field<int>("custom_class_id"));
                                                    var mList = new RHashlist();

                                                    mList.Add("custom_class_id", customClassId);

                                                    //RMDalWrapper.ExecuteQuery(RMQueryConstantsCustomClass.DELETE_CUSTOM_CLASS, mList,
                                                    //connManager);

                                                    CommonDALWrapper.ExecuteQuery("REFM:Delete_ivp_refm_custom_class", mList, connManager);

                                                }
                                            }
                                        }
                                    }
                                    foreach (var rowVal in ValidCustomClassInfoRows2)
                                    {
                                        var row = rowVal.Key;
                                        objCustomClassInfo = new RMCustomClassInfo();
                                        //objCustomClassController = new RMCustomClassController();
                                        objCustomClassInfo.ClassName = Convert.ToString(row["Script/Class Name"]);
                                        objCustomClassInfo.AssemblyPath = Convert.ToString(row["Assembly Path"]);
                                        objCustomClassInfo.CallType = ((RMTransportTasksConstants.CallType)Enum.Parse(typeof(RMTransportTasksConstants.CallType), Convert.ToString(row["Call Type"]).ToUpper())).ToString();

                                        objCustomClassInfo.TaskMasterId = taskNameVsID[Convert.ToString(row["Task Name"])];
                                        objCustomClassInfo.TaskDetailsId = TransportTasksDBManager.ReturnTransportId(transportPkVsId, rowVal.Value);
                                        objCustomClassInfo.ExecSequence = Convert.ToInt32(row["Sequence Number"]);
                                        objCustomClassInfo.CreatedBy = userName;
                                        objCustomClassInfo.LastModifiedBy = userName;

                                        switch (Convert.ToString(row["Class Type"]).ToUpper())
                                        {
                                            case "SCRIPT EXECUTABLE":
                                                objCustomClassInfo.ClassType = "1";
                                                break;
                                            case "CUSTOM CLASS":
                                                objCustomClassInfo.ClassType = "2";
                                                break;
                                        }

                                        new RMDataSourceDBManager(connManager).AddCustomClass(objCustomClassInfo, connManager,true);
                                        new RMCommonMigration().SetPassedRow(row, string.Empty);
                                    }


                                    if (connManager != null)
                                        connManager.CommitTransaction();

                                }
                                catch (Exception ex)
                                {
                                    if (connManager != null)
                                        connManager.RollbackTransaction();
                                    mLogger.Error(ex.ToString());
                                    errorMsg = ex.Message.ToString();

                                }
                                finally
                                {
                                    if (connManager != null)
                                    {
                                        RDALAbstractFactory.DBFactory.PutConnectionManager(connManager);
                                        connManager = null;
                                    }
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error(ex.ToString());
                        errorMsg = ex.Message.ToString();
                        // throw;
                    }
                    finally
                    {
                        mLogger.Debug("SMMigrationController : SMTransportTasks_Sync() : sheet2 --> End");
                    }
                }
                #endregion sheet2
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                errorMsg = ex.Message.ToString();
                throw ex;
            }
            finally
            {
                mLogger.Debug("SMMigrationController : SMTransportTasks_Sync()  --> End");
            }
        }



    }
}
