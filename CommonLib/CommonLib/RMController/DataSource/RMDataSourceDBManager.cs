using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.common;
using com.ivp.commom.commondal;
using System.Data;
using System.Xml;
using com.ivp.rad.utils;
using com.ivp.rad.RUserManagement;
using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.configurationmanagement;
using System.Text.RegularExpressions;

namespace com.ivp.common
{
    public class RMDataSourceDBManager
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMDataSourceDBManager");
        private RHashlist mList = null;
        private RDBConnectionManager mDBCon = null;
        public const string RM_VENDOR_DB = "IVPRefMasterVendor";
        public const string RM_VENDOR_ARCHIVE_DB = "IVPRefMasterVendor_Archive";

        public RMDataSourceDBManager(RDBConnectionManager conMgr)
        {
            this.mDBCon = conMgr;
        }

        public ObjectSet GetFeedDetailsByFeedID(int feedSummaryID)
        {
            mLogger.Debug("RMDataSourceDBManager -> GetAllFeedAndFileTypes -> Start");
            ObjectSet ds = new ObjectSet();

            try
            {
                string query = " EXEC IVPRefMasterVendor.dbo.REFM_GetFeedDetailsByFeedID " + feedSummaryID + " ";

                if (mDBCon == null)
                {
                    ds = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMasterVendor_Connection);
                }
                else
                {
                    ds = CommonDALWrapper.ExecuteSelectQueryObject(query, mDBCon);
                }

                return ds;
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> GetAllFeedAndFileTypes -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> GetAllFeedAndFileTypes -> End");
                if (mList != null)
                    mList.Dispose();
            }

        }

        public ObjectSet GetDataSourceConfiguration(List<int> feeds, int moduleID, bool getRawData, bool getAllFeeds)
        {
            try
            {
                mLogger.Debug("RMDataSourceDBManager -> DownloadDataSourceConfiguration -> Start");

                string feed_ids = string.Empty;
                ObjectSet oSet = null;

                if (feeds != null && feeds.Count > 0)
                {
                    feed_ids = string.Join(",", feeds);
                }

                string queryText = "EXEC [dbo].[REFM_FetchDataSourceConfiguration] '" + feed_ids + "', " + moduleID + ", " + getAllFeeds + " ";
                string source = string.Empty;


                if (this.mDBCon != null)
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                else
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMasterVendor_Connection);



                return oSet;
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> DownloadDataSourceConfiguration -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> DownloadDataSourceConfiguration -> End");
            }
        }

        public ObjectTable GetFeedRuleDetailsByRuleType(int feedSummaryID, int ruleTypeID)
        {
            try
            {
                ObjectTable table = null;
                ObjectSet oSet = null;

                string queryText = " EXEC IVPRefMasterVendor.dbo.REFM_GetFeedRuleDataByRuleType " + feedSummaryID + ", " + ruleTypeID + " ";

                if (this.mDBCon != null)
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                else
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMasterVendor_Connection);

                if (oSet != null && oSet.Tables.Count > 0)
                    table = oSet.Tables[0];

                return table;
            }
            catch
            {
                throw;
            }

        }

        public int GetEntityTypeFeedMappingID(int feedSummaryID, int entityTypeID)
        {
            try
            {
                int mappingID = -1;
                ObjectSet oSet = null;

                string queryText = " SELECT TOP 1 entity_type_feed_mapping_id FROM IVPRefMaster.dbo.ivp_refm_entity_type_feed_mapping " +
                                   " WHERE feed_summary_id = " + feedSummaryID + " AND entity_type_id = " + entityTypeID + " AND is_active = 1 ";

                if (this.mDBCon != null)
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                else
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMasterVendor_Connection);

                if (oSet != null && oSet.Tables.Count > 0 && oSet.Tables[0] != null && oSet.Tables[0].Rows.Count > 0)
                    mappingID = Convert.ToInt32(oSet.Tables[0].Rows[0]["entity_type_feed_mapping_id"]);

                return mappingID;
            }
            catch
            {
                throw;
            }
        }



        public bool CheckTaskNameExists(string feedName, string taskName, int taskTypeID)
        {
            mLogger.Debug("RMDataSourceDBManager -> CheckTaskNameExists -> Error");
            bool exists = false;
            DataSet ds = null;
            try
            {
                string query = "EXEC IVPRefMasterVendor.dbo.REFM_GetTaskDetailsByName '" + taskName.Replace("'", "''") + "', '"
                    + feedName.Replace("'", "''") + "', " + taskTypeID + " ";

                if (this.mDBCon != null)
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, this.mDBCon);
                else
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMasterVendor_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    exists = true;

                return exists;
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> CheckTaskNameExists -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> CheckTaskNameExists -> End");
            }

        }

        public string GetFeedLoadingTaskDetails(string feedName, string taskName, int taskTypeID)
        {
            mLogger.Debug("RMDataSourceDBManager -> CheckTaskNameExists -> Error");
            string result = string.Empty;
            DataSet ds = null;
            try
            {
                string query = "EXEC IVPRefMasterVendor.dbo.REFM_GetTaskDetailsByName '" + taskName.Replace("'", "''") + "', '"
                    + feedName.Replace("'", "''") + "', 2 ";

                if (this.mDBCon != null)
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, this.mDBCon);
                else
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMasterVendor_Connection);

                if (ds != null && ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[1].Rows[0];
                    result = Convert.ToString(row["task_master_id"]) + "|" + Convert.ToString(row["loading_details_id"]);
                }

                return result;
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> CheckTaskNameExists -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> CheckTaskNameExists -> End");
            }

        }

        public DataSet GetTaskSummaryByDependentID(int dependentID, string taskType)
        {
            mLogger.Debug("RMDataSourceDBManager :GetTaskSummaryByDependentID->Start getting task master ID");
            RHashlist htParams = null;
            DataSet dsResult = null;
            try
            {
                htParams = new RHashlist();
                dsResult = new DataSet();
                htParams.Add("dependent_id", dependentID);
                htParams.Add("task_type_id", taskType);

                if (this.mDBCon != null)
                    dsResult = CommonDALWrapper.ExecuteSelectQuery("RefMVendor:GetTaskMasterIDByDependentID",
                                                                   htParams, this.mDBCon);
                else
                    dsResult = CommonDALWrapper.ExecuteSelectQuery("RefMVendor:GetTaskMasterIDByDependentID",
                                                                       htParams, ConnectionConstants.RefMasterVendor_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager :GetTaskSummaryByDependentID->End getting task master ID");
                if (htParams != null)
                    htParams = null;
            }
            return dsResult;
        }

        public void CreateAPIFTPTaskDetails(int taskMasterID, string vendorDetails, string userName, int feedSummaryID, int licenseTypeID)
        {
            mLogger.Debug("RMDataSourceDBManager :CreateAPIFTPTaskDetails->Start creating task details");
            RHashlist htParams = null;
            try
            {
                htParams = new RHashlist();
                htParams.Add("task_master_id", taskMasterID);
                htParams.Add("vendor_details", vendorDetails);
                htParams.Add("created_by", userName);
                htParams.Add("last_modified_by", userName);
                htParams.Add("feed_summary_id", feedSummaryID);
                htParams.Add("license_type_id", licenseTypeID);

                if (this.mDBCon != null)
                    CommonDALWrapper.ExecuteQuery("RefMVendor:CreateAPIFTPTaskDetails"
                                           , htParams, this.mDBCon);
                else
                    CommonDALWrapper.ExecuteQuery("RefMVendor:CreateAPIFTPTaskDetails"
                                               , htParams, ConnectionConstants.RefMasterVendor_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager :CreateAPIFTPTaskDetails->End creating task details");
                if (htParams != null)
                    htParams = null;
            }
        }

        public void UpdateAPIFTPTaskDetails(int taskMasterID, string vendorDetails, string userName)
        {
            mLogger.Debug("RMDataSourceDBManager :CreateAPIFTPTaskDetails->Start creating task details");
            RHashlist htParams = null;
            try
            {
                htParams = new RHashlist();
                htParams.Add("vendor_details", vendorDetails);
                htParams.Add("last_modified_by", userName);
                htParams.Add("task_master_id", taskMasterID);

                if (this.mDBCon != null)
                    CommonDALWrapper.ExecuteQuery("RefMVendor:UpdateAPIFTPTaskDetailsByID", htParams, this.mDBCon);
                else
                    CommonDALWrapper.ExecuteQuery("RefMVendor:UpdateAPIFTPTaskDetailsByID", htParams, ConnectionConstants.RefMasterVendor_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager :CreateAPIFTPTaskDetails->End creating task details");
                if (htParams != null)
                    htParams = null;
            }
        }


        public bool AddUpdateLoadingTask(RMLoadTaskInfo objLoadingTaskInfo, RMCustomClassInfo[] objCustomClassInfo,
           RMTaskInfo objTaskSummaryInfo, int TaskMasterID, int TaskDetailsID)
        {
            mLogger.Debug("Start->RMLoadingTaskController : AddUpdateLoadingTask");
            bool isSuccess = false;
            bool isConnectionCreated = false;
            RDBConnectionManager vendorConnection = null;
            if (this.mDBCon != null)
                vendorConnection = this.mDBCon;
            else
            {
                vendorConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(RConfigReader.GetConfigAppSettings
                         (ConnectionConstants.RefMasterVendor_Connection));
                isConnectionCreated = true;
            }
            try
            {
                if (TaskDetailsID < 0)
                {
                    objTaskSummaryInfo.DependentId = objLoadingTaskInfo.FeedSummaryID;
                    int taskMasterID = AddTaskSummaryDetails(objTaskSummaryInfo);
                    objLoadingTaskInfo.TaskMasterID = taskMasterID;
                    int taskDetailsID = AddLoadingTaskDetails(objLoadingTaskInfo, vendorConnection);

                    if (objCustomClassInfo != null)
                    {
                        foreach (RMCustomClassInfo customClassInfo in objCustomClassInfo)
                        {
                            customClassInfo.CreatedBy = objLoadingTaskInfo.LastModifiedBy;
                            customClassInfo.LastModifiedBy = objLoadingTaskInfo.LastModifiedBy;
                            customClassInfo.TaskDetailsId = taskDetailsID;
                            customClassInfo.TaskMasterId = taskMasterID;

                            AddCustomClass(customClassInfo,
                                vendorConnection);
                        }
                    }
                }
                else
                {
                    objTaskSummaryInfo.TaskMasterId = TaskMasterID;
                    objTaskSummaryInfo.DependentId = objLoadingTaskInfo.FeedSummaryID;
                    UpdateTaskSummaryDetails(objTaskSummaryInfo, vendorConnection);

                    objLoadingTaskInfo.TaskMasterID = TaskMasterID;
                    UpdateLoadingTaskDetails(objLoadingTaskInfo, vendorConnection);

                    DeleteCustomClass(TaskMasterID, TaskDetailsID,
                        vendorConnection);
                    if (objCustomClassInfo != null)
                    {
                        foreach (RMCustomClassInfo customClassInfo in objCustomClassInfo)
                        {
                            customClassInfo.CreatedBy = objLoadingTaskInfo.LastModifiedBy;
                            customClassInfo.LastModifiedBy = objLoadingTaskInfo.LastModifiedBy;
                            customClassInfo.TaskDetailsId = TaskDetailsID;
                            customClassInfo.TaskMasterId = TaskMasterID;

                            AddCustomClass(customClassInfo,
                                vendorConnection);
                        }
                    }
                }
                if (isConnectionCreated)
                    vendorConnection.CommitTransaction();
                isSuccess = true;
            }
            catch (Exception ex)
            {
                if (isConnectionCreated)
                    vendorConnection.RollbackTransaction();
                mLogger.Error(ex.ToString());
                throw new Exception(ex.ToString(), ex);
            }
            finally
            {
                if (isConnectionCreated)
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(vendorConnection);
                    vendorConnection = null;
                }
                if (mList != null)
                    mList.Dispose();
                mLogger.Debug("End->RMLoadingTaskController : AddUpdateLoadingTask");
            }
            return isSuccess;
        }

        private int AddLoadingTaskDetails(RMLoadTaskInfo objLoadingTaskInfo,
            RDBConnectionManager vendorConnection)
        {
            mLogger.Debug("Start->RMLoadingTaskController : AddLoadingTaskDetails");
            mList = new RHashlist();
            DataSet dsLoading = null;

            try
            {
                mList.Add(RMTableRefmVLoadingTaskDetails.TASK_MASTER_ID,
                    objLoadingTaskInfo.TaskMasterID);
                mList.Add("feed_summary_id",
                    objLoadingTaskInfo.FeedSummaryID);
                mList.Add(RMTableRefmVLoadingTaskDetails.BULK_FILE_PATH,
                    objLoadingTaskInfo.BulkFilePath);
                mList.Add(RMTableRefmVLoadingTaskDetails.DIFFERENCE_FILE_PATH,
                    objLoadingTaskInfo.DifferenceFilePath);
                mList.Add(RMTableRefmVLoadingTaskDetails.CUSTOM_CALL_EXISTS,
                    objLoadingTaskInfo.CustomCallExist);
                mList.Add(RMTableRefmVLoadingTaskDetails.DIFF_FILE_DATE_TYPE,
                    objLoadingTaskInfo.DiffFileDateType);
                mList.Add(RMTableRefmVLoadingTaskDetails.DIFF_FILE_DATE,
                    objLoadingTaskInfo.DiffFileDate);
                mList.Add(RMTableRefmVLoadingTaskDetails.BULK_FILE_DATE_TYPE,
                    objLoadingTaskInfo.BulkFileDateType);
                mList.Add(RMTableRefmVLoadingTaskDetails.BULK_FILE_DATE,
                    objLoadingTaskInfo.BulkFileDate);
                mList.Add(RMTableRefmVLoadingTaskDetails.CREATED_BY,
                    objLoadingTaskInfo.CreatedBy);
                mList.Add(RMTableRefmVLoadingTaskDetails.LAST_MODIFIED_BY,
                    objLoadingTaskInfo.LastModifiedBy);
                mList.Add(RMTableRefmVLoadingTaskDetails.DIFF_FILE_DATE_DAYS,
                                    objLoadingTaskInfo.DiffFileDateDays);
                mList.Add(RMTableRefmVLoadingTaskDetails.BULK_FILE_DATE_DAYS,
                    objLoadingTaskInfo.BulkFileDateDays);

                dsLoading = vendorConnection.ExecuteQuery("REFMVendor:InsertLoadingTaskDetail",
                    mList, true);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new Exception(ex.ToString(), ex);
            }
            finally
            {
                if (mList != null)
                    mList.Dispose();
                mLogger.Debug("End->RMLoadingTaskController : AddLoadingTaskDetails");
            }

            if (dsLoading != null && dsLoading.Tables.Count > 0 && dsLoading.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(dsLoading.Tables[0].Rows[0][0]);
            }
            return -1;
        }

        private void UpdateLoadingTaskDetails(RMLoadTaskInfo objLoadingTaskInfo,
            RDBConnectionManager vendorConnection)
        {
            mLogger.Debug("Start->RMLoadingTaskController : UpdateLoadingTaskDetails");
            mList = new RHashlist();

            try
            {
                mList.Add(RMTableRefmVLoadingTaskDetails.LOADING_DETAILS_ID,
                    objLoadingTaskInfo.FeedLoadingDetailsID);
                mList.Add(RMTableRefmVLoadingTaskDetails.TASK_MASTER_ID,
                    objLoadingTaskInfo.TaskMasterID);
                mList.Add("feed_summary_id",
                    objLoadingTaskInfo.FeedSummaryID);
                mList.Add(RMTableRefmVLoadingTaskDetails.BULK_FILE_PATH,
                    objLoadingTaskInfo.BulkFilePath);
                mList.Add(RMTableRefmVLoadingTaskDetails.DIFFERENCE_FILE_PATH,
                    objLoadingTaskInfo.DifferenceFilePath);
                mList.Add(RMTableRefmVLoadingTaskDetails.CUSTOM_CALL_EXISTS,
                    objLoadingTaskInfo.CustomCallExist);
                mList.Add(RMTableRefmVLoadingTaskDetails.DIFF_FILE_DATE_TYPE,
                    objLoadingTaskInfo.DiffFileDateType);
                mList.Add(RMTableRefmVLoadingTaskDetails.DIFF_FILE_DATE,
                    objLoadingTaskInfo.DiffFileDate);
                mList.Add(RMTableRefmVLoadingTaskDetails.BULK_FILE_DATE_TYPE,
                    objLoadingTaskInfo.BulkFileDateType);
                mList.Add(RMTableRefmVLoadingTaskDetails.BULK_FILE_DATE,
                    objLoadingTaskInfo.BulkFileDate);
                mList.Add(RMTableRefmVLoadingTaskDetails.LAST_MODIFIED_BY,
                    objLoadingTaskInfo.LastModifiedBy);
                mList.Add(RMTableRefmVLoadingTaskDetails.DIFF_FILE_DATE_DAYS,
                    objLoadingTaskInfo.DiffFileDateDays);
                mList.Add(RMTableRefmVLoadingTaskDetails.BULK_FILE_DATE_DAYS,
                    objLoadingTaskInfo.BulkFileDateDays);

                vendorConnection.ExecuteQuery("REFMVendor:UpdateLoadingTaskDetail",
                    mList, true);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new Exception(ex.ToString(), ex);
            }
            finally
            {
                if (mList != null)
                    mList.Dispose();
                mLogger.Debug("End->RMLoadingTaskController : UpdateLoadingTaskDetails");
            }
        }

        public int AddTaskSummaryDetails(RMTaskInfo objTaskSummaryInfo, RDBConnectionManager vendorConnection = null)
        {
            mLogger.Debug("Start->Insert_ivp_refm_task_summary");
            mList = new RHashlist();
            DataSet ds = null;
            bool connectionCreated = false;
            string originalDBName = string.Empty;
            try
            {
                mList.Add("task_name", objTaskSummaryInfo.TaskName);
                mList.Add("task_description",
                    objTaskSummaryInfo.TaskDescription);
                mList.Add("task_type_id", objTaskSummaryInfo.TaskTypeId);
                mList.Add("dependent_id", objTaskSummaryInfo.DependentId);
                mList.Add(RMDBCommonConstantsInfo.CREATED_BY, objTaskSummaryInfo.CreatedBy);
                mList.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objTaskSummaryInfo.CreatedBy);
                if (vendorConnection == null)
                {
                    vendorConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(ConnectionConstants.RefMasterVendor_Connection));
                    connectionCreated = true;
                }
                else
                {
                    originalDBName = vendorConnection.DataBaseName;
                }

                ds = CommonDALWrapper.ExecuteSelectQuery("REFM:Insert_ivp_refm_task_summary", mList, vendorConnection);
                CommonDALWrapper.ExecuteSelectQuery(DALWrapperAppend.Replace("USE IVPSRMTaskManager"), vendorConnection);
                new RMCommonController().RMSaveTaskInCTM(objTaskSummaryInfo.TaskName, rad.RCommonTaskManager.OperationType.Add, vendorConnection);
                if (!string.IsNullOrEmpty(originalDBName))
                {
                    CommonDALWrapper.ExecuteSelectQuery("USE " + originalDBName, vendorConnection);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message);
                throw ex;
            }
            finally
            {
                if (connectionCreated && vendorConnection != null)
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(vendorConnection);
                }
                if (mList != null)
                    mList.Dispose();      
                mLogger.Debug("End->Insert_ivp_refm_task_summary");
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            return -1;
        }

        public bool UpdateTaskSummaryDetails(RMTaskInfo objTaskSummaryInfo, RDBConnectionManager vendorConnection = null)
        {
            mLogger.Debug("Start->Update_ivp_refm_task_summary");
            bool isSuccess = false;
            mList = new RHashlist();
            try
            {
                mList.Add("task_master_id", objTaskSummaryInfo.TaskMasterId);
                mList.Add("task_name", objTaskSummaryInfo.TaskName);
                mList.Add("task_description", objTaskSummaryInfo.TaskDescription);
                mList.Add("task_type_id", objTaskSummaryInfo.TaskTypeId);
                mList.Add("dependent_id", objTaskSummaryInfo.DependentId);
                mList.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objTaskSummaryInfo.CreatedBy);

                if (vendorConnection == null)
                    CommonDALWrapper.ExecuteQuery("REFM:Update_ivp_refm_task_summary", mList, ConnectionConstants.RefMasterVendor_Connection);
                else
                    CommonDALWrapper.ExecuteQuery("REFM:Update_ivp_refm_task_summary", mList, vendorConnection);

                new RMCommonController().RMSaveTaskInCTM(objTaskSummaryInfo.TaskName, rad.RCommonTaskManager.OperationType.Update,null);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message);
                throw ex;
            }
            finally
            {
                if (mList != null)
                    mList.Dispose();
                mLogger.Debug("End->Update_ivp_refm_task_summary");
            }
            return isSuccess;
        }

        public bool AddCustomClass(RMCustomClassInfo customClassInfo,
            RDBConnectionManager connectionManager = null, bool requireMTHandling = false)
        {
            mLogger.Debug("Start->Insert_ivp_refm_custom_class");
            bool isSuccess = false;
            mList = new RHashlist();
            try
            {
                mList.Add("class_name", customClassInfo.ClassName);
                if(requireMTHandling)
                    mList.Add("assembly_path", Regex.Replace(customClassInfo.AssemblyPath, "IVP", "Æ", RegexOptions.IgnoreCase));
                else
                    mList.Add("assembly_path", customClassInfo.AssemblyPath);
                mList.Add("call_type", customClassInfo.CallType);
                mList.Add("class_type", customClassInfo.ClassType);
                mList.Add("task_master_id", customClassInfo.TaskMasterId);
                mList.Add("task_details_id", customClassInfo.TaskDetailsId);
                mList.Add("exec_sequence", customClassInfo.ExecSequence);
                mList.Add(RMDBCommonConstantsInfo.CREATED_BY, customClassInfo.CreatedBy);
                mList.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, customClassInfo.CreatedBy);

                if (connectionManager != null)
                    connectionManager.ExecuteQuery("REFM:Insert_ivp_refm_custom_class", mList,
                        true);
                else
                    CommonDALWrapper.ExecuteSelectQuery("REFM:Insert_ivp_refm_custom_class", mList, ConnectionConstants.RefMasterVendor_Connection);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message);
                throw ex;
            }
            finally
            {
                if (mList != null)
                    mList.Dispose();
                mLogger.Debug("End->Insert_ivp_refm_custom_class");
            }
            return isSuccess;
        }

        public bool DeleteCustomClass(int taskMasterId, int taskDetailsId,
            RDBConnectionManager connectionManager)
        {
            mLogger.Debug("Start->DeleteCustomClass");
            bool isSuccess = false;
            mList = new RHashlist();
            try
            {
                mList.Add("task_master_id", taskMasterId);
                mList.Add("task_details_id", taskDetailsId);

                connectionManager.ExecuteQuery("REFMVendor:DeleteCustomClass",
                    mList, true);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message);
                throw ex;
            }
            finally
            {
                if (mList != null)
                    mList.Dispose();
                mLogger.Debug("End->DeleteCustomClass");
            }
            return isSuccess;
        }

        public void InsertNewLicenseDetails(List<RMLicenseSetupInfo> objInfoList)
        {
            mLogger.Debug("Start->InsertNewLicenseDetails");
            RHashlist mlist;

            try
            {
                foreach (RMLicenseSetupInfo objLicenseSetupInfo in objInfoList)
                {
                    mlist = new RHashlist();
                    mlist.Add("feed_summary_id", objLicenseSetupInfo.FeedSummaryId);
                    mlist.Add("license_type_id", objLicenseSetupInfo.LicenseTypeId);
                    mlist.Add("created_by", objLicenseSetupInfo.CreatedBy);
                    mlist.Add("last_modified_by", objLicenseSetupInfo.LastModifiedBy);

                    if (this.mDBCon != null)
                        CommonDALWrapper.ExecuteSelectQuery("RefMVendor:InsertNewLicenseDetails", mlist, this.mDBCon);
                    else
                        CommonDALWrapper.ExecuteSelectQuery("RefMVendor:InsertNewLicenseDetails", mlist, ConnectionConstants.RefMasterVendor_Connection);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("End-->InsertNewLicenseDetails");
            }
        }

        public DataSet GetFieldsByFeedSummaryId(int feedSummaryID)
        {
            mLogger.Debug("RMCommonDBManager : GetFieldsByFeedSummaryId -> Start");
            RHashlist hList = null;
            try
            {
                hList = new RHashlist();
                hList.Add("feed_summary_id", feedSummaryID);

                if (this.mDBCon == null)
                    return CommonDALWrapper.ExecuteSelectQuery("REFM:SelectFeedFieldDetail", hList, ConnectionConstants.RefMasterVendor_Connection);
                else
                    return CommonDALWrapper.ExecuteSelectQuery("REFM:SelectFeedFieldDetail", hList, mDBCon);
            }
            catch (Exception ex)
            {
                mLogger.Error("RMCommonDBManager : GetFieldsByFeedSummaryId -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RMCommonDBManager : GetFieldsByFeedSummaryId -> End");
                if (hList != null)
                {
                    hList.Clear();
                    hList = null;
                }
            }
        }

        public void DeleteFeedEntityTypeMapping(int feedSummaryID, List<int> entityTypeIds, RDBConnectionManager conMgr)
        {
            mLogger.Debug("RMDataSourceDBManager: DeleteFeedEntityTypeMapping -> Start");

            try
            {
                string entityTypes = string.Empty;
                if (entityTypeIds != null && entityTypeIds.Count > 0)
                {
                    entityTypes = string.Join(",", entityTypeIds);
                }
                string query = " EXEC IVPRefMasterVendor.dbo.REFM_DeleteFeedEntityTypeMapping " + feedSummaryID + ", '" + entityTypes + "' ";

                if (conMgr != null)
                {
                    CommonDALWrapper.ExecuteSelectQuery(query, conMgr);
                }
                else
                {
                    CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                }
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMDataSourceDBManager: DeleteFeedEntityTypeMapping -> Error: " + ex.Message);
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager: DeleteFeedEntityTypeMapping -> End");
            }

        }

        public Dictionary<int, int> AddEntityTypeFeedMapping(RMEntityTypeFeedMapping objEFMappingInfo)
        {
            mLogger.Debug("RMDataSourceDBManager:AddEntityTypeFeedMapping -> Start");
            Dictionary<int, int> entityTypeFeedMappingIDs = new Dictionary<int, int>();
            DataSet ds = null;
            try
            {
                string query = "EXEC [IVPRefMaster].[dbo].[REFM_AddEntityTypeFeedMapping] " + objEFMappingInfo.FeedSummaryID + ", " + objEFMappingInfo.EntityTypeID + ", " + objEFMappingInfo.ReplaceExisting + ", " + objEFMappingInfo.IsMasterUpdateOnly + ", '" + objEFMappingInfo.CreatedBy + "', '" + objEFMappingInfo.LastModifiedBy + "' ";

                if (this.mDBCon != null)
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, this.mDBCon);
                else
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMasterVendor_Connection);

                ModifyFeedMappingView(0, objEFMappingInfo.FeedSummaryID, 0, false);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    ds.Tables[0].AsEnumerable().ToList().ForEach(d =>
                    {
                        entityTypeFeedMappingIDs.Add(Convert.ToInt32(d["entity_type_id"]), Convert.ToInt32(d["entity_type_feed_mapping_id"]));
                    });
                }

                return entityTypeFeedMappingIDs;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager:AddEntityTypeFeedMapping -> End");
            }
        }

        public void UpdateEntityTypeFeedMapping(RMEntityTypeFeedMapping objEFMappingInfo, RDBConnectionManager conMgr)
        {
            mLogger.Debug("RMDataSourceDBManager:UpdateEntityTypeFeedMapping -> Start");
            try
            {
                string query = "EXEC IVPRefMaster.dbo.REFM_UpdateEntityTypeFeedMapping " + objEFMappingInfo.FeedSummaryID + ", " + objEFMappingInfo.EntityTypeID + ", " + objEFMappingInfo.ReplaceExisting + ", " + objEFMappingInfo.IsMasterUpdateOnly + ", '" + objEFMappingInfo.LastModifiedBy + "' ";

                if (conMgr != null)
                    CommonDALWrapper.ExecuteSelectQuery(query, conMgr);
                else
                    CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager:UpdateEntityTypeFeedMapping -> End");
            }
        }

        public ObjectTable GetAllDataSources()
        {
            mLogger.Debug("RMDataSourceManager -> GetAllDataSources -> Start");
            try
            {
                string query = " SELECT data_source_id, data_source_name FROM IVPRefMasterVendor.dbo.ivp_refm_data_source WHERE is_active = 1 ";
                ObjectSet oSet = null;

                if (this.mDBCon != null)
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(query, this.mDBCon);
                else
                    oSet = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMasterVendor_Connection);

                if (oSet != null && oSet.Tables.Count > 0)
                    return oSet.Tables[0];

                return null;
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMDataSourceManager -> GetAllDataSources -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMDataSourceManager -> GetAllDataSources -> End");
            }
        }

        public void SaveAllAttributesMappedForEntity(DataTable dtEFMappedDetailsToInsert, List<RMEntityTypeFeedMapping> lstobjEFMappingInfo,
            List<int> lstEntityType, int feedSummaryId, int entityTypeId,
            RMEntityFeedAttributeLookup[] objEntityFeedAttLookupInfo, RMEntityFeedAttributeLookup[] objSecLookupInfo = null)
        {
            mLogger.Debug("RMEntityTypeFeedMappingDetailsController:SaveAllAttributesMappedForEntity -> Start Savin All mapped Attributes to feed fields");
            try
            {
                RHashlist htParams = new RHashlist();
                string xmlInfo = string.Empty;
                string secLookupXML = string.Empty;
                bool isConnectionCreated = false;
                xmlInfo = GetXMLForInfo(objEntityFeedAttLookupInfo, entityTypeId);

                if (objSecLookupInfo != null)
                    secLookupXML = GetXMLForInfo(objSecLookupInfo, entityTypeId);

                RDBConnectionManager connectionManager;

                if (this.mDBCon == null)
                {
                    isConnectionCreated = true;
                    string mDBConnectionId = RConfigReader.GetConfigAppSettings(ConnectionConstants.RefMaster_Connection.ToString());
                    connectionManager = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                    connectionManager.UseTransaction = true;
                }
                else
                {
                    connectionManager = this.mDBCon;
                }
                try
                {
                    {
                        InsertAttributePriorityFromEntityFeedMapping(xmlInfo, connectionManager);

                        if (!string.IsNullOrEmpty(secLookupXML))
                            InsertSecLookupDataForFeed(secLookupXML, connectionManager);

                        foreach (RMEntityTypeFeedMapping objEFMappingInfo in lstobjEFMappingInfo)
                        {
                            UpdateEntityTypeFeedMapping(objEFMappingInfo, connectionManager);
                            DeleteAllAttributesMappedForEntity(feedSummaryId, objEFMappingInfo.EntityTypeID, connectionManager);
                        }

                        DataRow[] drUnMappedRows = dtEFMappedDetailsToInsert.Select("field_id=0");
                        foreach (DataRow row in drUnMappedRows)
                        {
                            dtEFMappedDetailsToInsert.Rows.Remove(row);
                        }
                        dtEFMappedDetailsToInsert.AcceptChanges();

                        CommonDALWrapper.ExecuteBulkUpload("ivp_refm_entity_type_feed_mapping_details", dtEFMappedDetailsToInsert, connectionManager);

                        CreateFeedFieldMappingView(feedSummaryId, entityTypeId, connectionManager);

                        if (isConnectionCreated)
                        {
                            connectionManager.CommitTransaction();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (isConnectionCreated && connectionManager != null)
                    {
                        connectionManager.RollbackTransaction();
                    }
                    throw ex;
                }
                finally
                {
                    if (isConnectionCreated)
                    {
                        if (connectionManager != null)
                        {
                            RDALAbstractFactory.DBFactory.PutConnectionManager(connectionManager);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMEntityTypeFeedMappingDetailsController:SaveAllAttributesMappedForEntity -> End Deleting All mapped Attributes to feed fields");
            }
        }

        private void CreateFeedFieldMappingView(int feedSummaryID, int entityTypeID, RDBConnectionManager conMgr)
        {
            mLogger.Debug("RMDataSourceManager -> CreateFeedFieldMappingView -> Start");
            try
            {
                CommonDALWrapper.ExecuteSelectQuery(" EXEC REFM_CreateFeedMappingView " + feedSummaryID + ", " + entityTypeID + " ", conMgr);
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMDataSourceManager -> CreateFeedFieldMappingView -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMDataSourceManager -> CreateFeedFieldMappingView -> End");
            }
        }

        public void DeleteAllAttributesMappedForEntity(int feedSummaryId, int entityTypeID, RDBConnectionManager conMgr)
        {
            mLogger.Debug("RMEntityTypeFeedMappingDetailsController:DeleteAllAttributesMappedForEntity -> Start Deleting All mapped Attributes to feed fields");
            try
            {
                string query = "EXEC IVPRefMaster.dbo.REFM_DeleteAllAttributesMappedForEntity " + feedSummaryId + ", " + entityTypeID + " ";

                if (conMgr != null)
                    CommonDALWrapper.ExecuteSelectQuery(query, conMgr);
                else
                    CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMEntityTypeFeedMappingDetailsController:DeleteAllAttributesMappedForEntity -> End Deleting All mapped Attributes to feed fields");
            }
        }

        public Boolean InsertAttributePriorityFromEntityFeedMapping(string xmlInfo, RDBConnectionManager conn)
        {
            mLogger.Debug("Start->InsertAttributePriorityFromEntityFeedMapping");
            RHashlist mlist = new RHashlist();
            Boolean result;
            DataSet dsResult = new DataSet();
            try
            {
                mlist.Add("xmlAttributePriorityInfo", xmlInfo);
                dsResult = (DataSet)CommonDALWrapper.ExecuteProcedure("REFM:InsertAttributePriorityFromMapping",
                    mlist, conn)["DataSet"];
                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                    result = Convert.ToBoolean(dsResult.Tables[0].Rows[0]["is_success"]);
                else
                    result = false;
            }
            catch (Exception ex)
            {
                result = false;
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("End-->InsertAttributePriorityFromEntityFeedMapping");
            }
            return result;
        }

        public Boolean InsertSecLookupDataForFeed(string xmlInfo, RDBConnectionManager conn)
        {
            mLogger.Debug("Start->InsertSecLookupDataForFeed");
            RHashlist mlist = new RHashlist();
            Boolean result;
            DataSet dsResult = new DataSet();
            try
            {
                mlist.Add("xmlAttributePriorityInfo", xmlInfo);
                dsResult = (DataSet)CommonDALWrapper.ExecuteProcedure("REFM:InsertUpdateSecurityLookupForFeed", mlist, conn)["DataSet"];

                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                    result = Convert.ToBoolean(dsResult.Tables[0].Rows[0]["is_success"]);
                else
                    result = false;
            }
            catch (Exception ex)
            {
                result = false;
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("End-->InsertSecLookupDataForFeed");
            }
            return result;
        }

        private string GetXMLForInfo(RMEntityFeedAttributeLookup[] objEntityFeedAttLookupInfo, int entityTypeId)
        {
            StringBuilder xmlFlowInfo = new StringBuilder();
            xmlFlowInfo.Append("<AttributePriorityInfo>");

            foreach (RMEntityFeedAttributeLookup objEFAttLookup in objEntityFeedAttLookupInfo)
            {
                xmlFlowInfo.Append("<EntityAttributeLookup>");
                xmlFlowInfo.Append("<attribute_lookup_id>" + objEFAttLookup.AttributeLookupId + "</attribute_lookup_id>");
                xmlFlowInfo.Append("<feed_summary_id>" + objEFAttLookup.FeedSummaryId + "</feed_summary_id>");
                xmlFlowInfo.Append("<parent_entity_attribute_name>" + objEFAttLookup.ParentEntityAttributeName + "</parent_entity_attribute_name>");
                xmlFlowInfo.Append("<is_active>" + objEFAttLookup.IsActive + "</is_active>");
                xmlFlowInfo.Append("<created_by>" + objEFAttLookup.CreatedBy + "</created_by>");
                xmlFlowInfo.Append("<last_modified_by>" + objEFAttLookup.LastModifiedBy + "</last_modified_by>");
                xmlFlowInfo.Append("</EntityAttributeLookup>");
            }


            xmlFlowInfo.Append("</AttributePriorityInfo>");

            return xmlFlowInfo.ToString();


        }

        public int AddDatasource(RMDatasourceInfo datasourceInfo)
        {
            mLogger.Debug("RMDataSourceDBManager -> AddDatasource -> Start");
            mList = new RHashlist();
            int result = -1;
            DataSet ds = null;
            try
            {
                mList.Add("data_source_name", datasourceInfo.DatasourceName);
                mList.Add("description", datasourceInfo.DatasourceDescription);
                mList.Add("created_by", datasourceInfo.CreatedBy);
                mList.Add("last_modified_by", datasourceInfo.CreatedBy);
                mList.Add("account_id", datasourceInfo.AccountID);
                if (this.mDBCon == null)
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery("RefMVendor:AddDatasource", mList, ConnectionConstants.RefMasterVendor_Connection);
                }
                else
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery("RefMVendor:AddDatasource", mList, this.mDBCon);
                }

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result = Convert.ToInt32(ds.Tables[0].Rows[0]["data_source_id"].ToString());
                }

            }
            catch (Exception controllerEx)
            {
                mLogger.Debug("RMDataSourceDBManager -> AddDatasource -> Error");
                mLogger.Error(controllerEx.Message);
                result = -1;
                throw;
            }
            finally
            {
                if (mList != null)
                    mList.Dispose();
                mLogger.Debug("RMDataSourceDBManager -> AddDatasource -> End");
            }

            return result;
        }


        public bool UpdateDatasource(RMDatasourceInfo datasourceInfo)
        {
            mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> Start");
            mList = new RHashlist();
            bool result = false;
            try
            {
                ModifyFeedMappingView(0, 0, datasourceInfo.DatasourceID, true);

                mList.Add("data_source_id", datasourceInfo.DatasourceID);
                mList.Add("data_source_name", datasourceInfo.DatasourceName);
                mList.Add("data_source_description", datasourceInfo.DatasourceDescription);
                mList.Add("last_modified_by", datasourceInfo.LastModifiedBy);

                if (mDBCon == null)
                {
                    CommonDALWrapper.ExecuteQuery("RefMVendor:UpdateDatasource", mList, ConnectionConstants.RefMasterVendor_Connection);
                }

                else
                {
                    CommonDALWrapper.ExecuteQuery("RefMVendor:UpdateDatasource", mList,
                        mDBCon);
                }

                ModifyFeedMappingView(0, 0, datasourceInfo.DatasourceID, false);
                result = true;
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> End");
            }
            return result;
        }

        public int AddFeed(RMFeedSummaryInfo objFeedSummaryInfo)
        {
            mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> Start");
            int feedSummaryID = -1;
            mList = new RHashlist();
            mList.Add("feed_name", objFeedSummaryInfo.FeedName);
            mList.Add("data_source_id", objFeedSummaryInfo.DataSourceID);
            mList.Add("feed_type_id", objFeedSummaryInfo.FeedTypeID);
            mList.Add("created_by", objFeedSummaryInfo.CreatedBy);
            mList.Add("last_modified_by", objFeedSummaryInfo.LastModifiedBy);
            mList.Add("is_bulk_loaded", objFeedSummaryInfo.IsBulkLoaded);


            try
            {
                if (mDBCon == null)
                {
                    feedSummaryID = Convert.ToInt32(CommonDALWrapper.ExecuteSelectQuery("REFM:AddFeed", mList, ConnectionConstants.RefMasterVendor_Connection).Tables[0].Rows[0]["feed_summary_id"]);
                }
                else
                {
                    feedSummaryID = Convert.ToInt32(CommonDALWrapper.ExecuteSelectQuery("REFM:AddFeed", mList, mDBCon).Tables[0].Rows[0]["feed_summary_id"]);
                }
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> End");
                if (mList != null)
                    mList.Dispose();
            }
            return feedSummaryID;
        }

        public int GetFeedSummaryIdByName(string feedName)
        {

            int feedId;
            DataSet ds = new DataSet();
            mLogger.Debug("RMDataSourceDBManager -> GetFeedSummaryIdByName -> Start");
            mList = new RHashlist();
            mList.Add("feed_name", feedName);

            try
            {
                if (mDBCon == null)
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery("REFM:GetFeedSummaryIdByFeedName", mList, ConnectionConstants.RefMasterVendor_Connection);
                }
                else
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery("REFM:GetFeedSummaryIdByFeedName", mList, mDBCon);
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    feedId = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                }
                else feedId = 0;
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> GetFeedSummaryIdByName -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> GetFeedSummaryIdByName -> End");
                if (mList != null)
                    mList.Dispose();
            }

            return feedId;
        }

        public DataTable GetAllRadMappings()
        {
            DataSet ds = new DataSet();
            DataTable dt = null;
            mLogger.Debug("RMDataSourceDBManager -> GetAllRadMappings -> Start");
            string query = string.Empty;
            try
            {
                query = " SELECT mapping_summary_id, mapping_name FROM IVPRAD.dbo.ivp_rad_mapping_summary WHERE is_active = 1 ORDER BY mapping_name ";
                if (mDBCon == null)
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMasterVendor_Connection);
                }
                else
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
                }
                if (ds != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> GetAllRadMappings -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> GetAllRadMappings -> End");
                if (mList != null)
                    mList.Dispose();
            }

            return dt;
        }

        public ObjectSet GetAllFeedAndFileTypes()
        {
            mLogger.Debug("RMDataSourceDBManager -> GetAllFeedAndFileTypes -> Start");
            ObjectSet ds = new ObjectSet();

            try
            {
                string query = " SELECT feed_type_id,feed_type_name FROM ivp_refm_feed_type WHERE is_active=1; SELECT * FROM [IVPRefMasterVendor].[dbo].ivp_rad_file_type WHERE is_active = 1";

                if (mDBCon == null)
                {
                    ds = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMasterVendor_Connection);
                }
                else
                {
                    ds = CommonDALWrapper.ExecuteSelectQueryObject(query, mDBCon);
                }

                return ds;
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> GetAllFeedAndFileTypes -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager -> GetAllFeedAndFileTypes -> End");
                if (mList != null)
                    mList.Dispose();
            }

        }

        public void ModifyFeedMappingView(int entityTypeID, int feedSummaryID, int dataSourceID, bool dropView)
        {
            try
            {
                mLogger.Debug("RMCommonUtils: ModifyFeedMappingView -> Start");

                string query = " EXEC IVPRefMaster.dbo.REFM_ModifyFeedMappingViews " + dataSourceID + ", " + feedSummaryID + ", " + entityTypeID + ", " + dropView + " ";

                if (mDBCon != null)
                    CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
                else
                    CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);


            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("RMCommonUtils: ModifyFeedMappingView -> End");
            }
        }

        public bool AddUpdateFeedSummary(RADFilePropertyInfo objRADFilePropertyInfo,
            RMFeedSummaryInfo objFeedSummaryInfo, RADFileFieldDetailsInfo[] objRADFileFieldDetailsInfo,
            RMFeedFieldDetailsInfo[] objRefMFeedFieldDetailsInfo)
        {
            bool isSuccess = true;
            bool isConnectionCreated = false;
            mLogger.Debug("Start->Insert Feed Summary");

            if (mDBCon == null)
            {
                isConnectionCreated = true;
                mDBCon = RDALAbstractFactory.DBFactory.GetConnectionManager(RConfigReader.GetConfigAppSettings(ConnectionConstants.RefMasterVendor_Connection.ToString()));
            }
            try
            {
                int[] ids = null;

                if (objRADFilePropertyInfo.FileId <= 0)
                    ids = AddFileProperty(objRADFilePropertyInfo, objFeedSummaryInfo,
                        mDBCon);
                else
                    UpdateFileProperty(objRADFilePropertyInfo, objFeedSummaryInfo,
                        mDBCon);
                for (int i = 0; i < objRADFileFieldDetailsInfo.Length; i++)
                {
                    if (objRADFileFieldDetailsInfo[i].FieldId <= 0)
                    {
                        objRADFileFieldDetailsInfo[i].CreatedBy = objRADFilePropertyInfo.CreatedBy;
                        objRADFileFieldDetailsInfo[i].LastModifiedBy =
                            objRADFilePropertyInfo.LastModifiedBy;
                        if (ids == null)
                        {
                            ids = new int[2];
                            ids[0] = objFeedSummaryInfo.FeedSummaryID;
                            ids[1] = objRADFilePropertyInfo.FileId;
                        }
                        AddFileFieldDetails(objRADFileFieldDetailsInfo[i],
                            objRefMFeedFieldDetailsInfo[i], ids, mDBCon);
                    }
                    else
                    {
                        objRADFileFieldDetailsInfo[i].CreatedBy = objRADFilePropertyInfo.CreatedBy;
                        objRADFileFieldDetailsInfo[i].LastModifiedBy =
                            objRADFilePropertyInfo.LastModifiedBy;
                        UpdateFileFieldDetails(objRADFileFieldDetailsInfo[i],
                            objRefMFeedFieldDetailsInfo[i], mDBCon);
                    }
                }
                ManageFeedTable(objFeedSummaryInfo.FeedSummaryID, mDBCon);
                ManageFeedTablesForEncryption(objFeedSummaryInfo.FeedSummaryID);
                CreateFeedViewWithFieldName(objFeedSummaryInfo.FeedSummaryID, mDBCon);


                if (isConnectionCreated)
                    mDBCon.CommitTransaction();
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> Error");
                if (isConnectionCreated)
                    mDBCon.RollbackTransaction();
                throw;
            }
            finally
            {
                if (isConnectionCreated)
                {
                    if (mDBCon != null)
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                }
                mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> Error");
            }
            return isSuccess;
        }

        public void ManageFeedTablesForEncryption(int feedSummaryId)
        {
            mLogger.Debug("RMDynamicDB : ManageFeedTablesForEncryption -> Start");
            RHashlist htParams = null;

            try
            {
                htParams = new RHashlist();
                htParams.Add("feed_summary_id", feedSummaryId.ToString());
                CommonDALWrapper.ExecuteProcedure("REFMV:ManageFeedTablesForEncryption", htParams, this.mDBCon);
            }

            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }

            finally
            {
                mLogger.Debug("RMDynamicDB : ManageFeedTablesForEncryption -> End");
            }
        }

        public void CreateFeedViewWithFieldName(int feedSummaryId, RDBConnectionManager conMgr)
        {
            RHashlist htParams = null;
            try
            {
                DataSet mappingId = new DataSet();
                htParams = new RHashlist();
                htParams.Add("feedSummaryID", feedSummaryId.ToString());
                CommonDALWrapper.ExecuteProcedure("REFVendor:CreateViewOnFeedWithFieldName", htParams, conMgr);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {

                htParams.Clear();
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("RMFeedMappingController:GetFieldID -> End Adding Feed Details");
            }
        }

        public void ManageFeedTable(int feedSummaryId, RDBConnectionManager connectionManager)
        {
            mLogger.Debug("RMDynamicDB : ManageFeedTable -> Start");
            string feedTableName = string.Empty;
            List<RMColumnInfo> defaultColumns = null;
            List<RMColumnInfo> dynamicColumns = null;
            List<RMColumnInfo> columns = null;
            try
            {
                feedTableName = GetVendorArchiveFeedTable(feedSummaryId);
                defaultColumns = new List<RMColumnInfo>();
                defaultColumns = new RMCommonController().GetDefaultColumns(RMDynamicTableType.Feed);
                dynamicColumns = new List<RMColumnInfo>();
                dynamicColumns = GetDynamicFeedColumns(feedSummaryId);
                columns = new List<RMColumnInfo>();
                columns.AddRange(defaultColumns);
                columns.AddRange(dynamicColumns);
                string tableSql = ManageTable(RM_VENDOR_DB, feedTableName, columns);
                string archiveTableSql = ManageHistoryFeedTable(feedSummaryId);

                if (!string.IsNullOrEmpty(tableSql) || !string.IsNullOrEmpty(archiveTableSql))
                    connectionManager.ExecuteQuery(DALWrapperAppend.Replace(archiveTableSql + tableSql + " USE IVPRefMasterVendor;"), RQueryType.Insert);
            }

            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }

            finally
            {
                mLogger.Debug("RMDynamicDB : ManageFeedTable -> End");
            }
        }

        public string ManageHistoryFeedTable(int feedSummaryId)
        {
            mLogger.Debug("RMDynamicDB : ManageFeedTable -> Start");
            string feedTableName = string.Empty;
            List<RMColumnInfo> defaultColumns = null;
            List<RMColumnInfo> dynamicColumns = null;
            List<RMColumnInfo> columns = null;
            try
            {
                feedTableName = GetVendorArchiveFeedTable(feedSummaryId);
                defaultColumns = new List<RMColumnInfo>();
                defaultColumns = new RMCommonController().GetDefaultColumns(RMDynamicTableType.Feed);
                dynamicColumns = new List<RMColumnInfo>();
                dynamicColumns = GetDynamicHistoryFeedColumns(feedSummaryId);
                columns = new List<RMColumnInfo>();
                columns.AddRange(defaultColumns);
                columns.AddRange(dynamicColumns);
                string tableSql = ManageTable(RM_VENDOR_ARCHIVE_DB, feedTableName, columns);
                return tableSql;
            }

            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }

            finally
            {
                mLogger.Debug("RMDynamicDB : ManageFeedTable -> End");
            }
        }

        private string ManageTable(string database, string tableName, List<RMColumnInfo> columns)
        {
            mLogger.Debug("RMDynamicDB : ManageTable -> Start");
            string sql = string.Empty;

            try
            {
                DataSet objectInfo = new DataSet();
                objectInfo = new RMCommonDBManager(null).GetTableOrViewDetailsInfo(database, tableName);

                if (objectInfo.Tables.Count > 1 && objectInfo.Tables[1].Rows.Count != 0)
                {
                    List<string> staticColumns = new List<string>() { "id", "entity_code", "last_modified_by", "loading_time", "is_active", "is_latest", "instance_id", "knowledge_date", "created_by", "is_deleted" };

                    //Table already exists in database                    
                    foreach (DataRow dr in objectInfo.Tables[1].Rows)
                    {
                        if (staticColumns.Contains(dr["COLUMN_NAME"].ToString().ToLower()) && columns.Any(x => x.ColumnName.Equals(dr["COLUMN_NAME"].ToString(), StringComparison.InvariantCultureIgnoreCase)))
                        {
                            columns.Remove(columns.Where(cl => cl.ColumnName.Equals(dr["COLUMN_NAME"].ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault());
                        }
                        else if (columns.Any(cl => cl.ColumnName.Equals(dr["COLUMN_NAME"].ToString(), StringComparison.InvariantCultureIgnoreCase)))
                        {
                            columns.Where(cl => cl.ColumnName.Equals(dr["COLUMN_NAME"].ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().IsExisting = true;
                        }
                    }

                    sql = new RMCommonController().AlterTable(database, tableName, columns, objectInfo);
                }
                else
                {
                    //Table does not exist in database  
                    sql = new RMCommonController().CreateTable(database, tableName, columns, objectInfo);
                }
                //------------------------------------------------------------------------
                if (!string.IsNullOrEmpty(sql))
                {
                    sql = sql.Replace("\t", " ");
                    sql = sql.Replace("\r", " ");
                    sql = sql.Replace("\n", " ");
                }
                return sql;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new Exception(ex.Message);
            }
            finally
            {
                mLogger.Debug("RMDynamicDB : ManageTable -> End");
            }
        }

        private List<RMColumnInfo> GetDynamicFeedColumns(int feedId)
        {
            mLogger.Debug("RMDynamicDB : ManageEntityTypeTable -> Start");
            DataSet dsFields = null;
            List<RMColumnInfo> dynamicColumns = null;
            try
            {
                dynamicColumns = new List<RMColumnInfo>();
                dsFields = new RMDataSourceDBManager(this.mDBCon).GetFieldsByFeedSummaryId(feedId);

                RMColumnInfo objDynamicColumns = null;
                foreach (DataRow dr in dsFields.Tables[0].Rows)
                {
                    objDynamicColumns = new RMColumnInfo();
                    objDynamicColumns.ColumnName = GetFeedColumnName((int)dr[RMTableRefmVFeedFieldDetail.RAD_FIELD_ID]);
                    if ((bool)dr[RMTableRefmVFeedFieldDetail.IS_PRIMARY])
                    {
                        objDynamicColumns.DataType = RMDBDataTypes.VARCHAR;
                        objDynamicColumns.Length = "8000";
                    }
                    else
                    {
                        objDynamicColumns.DataType = RMDBDataTypes.VARCHARMAX;
                        objDynamicColumns.Length = "";
                    }
                    objDynamicColumns.DefaultValue = "";
                    objDynamicColumns.IsUnique = false;
                    objDynamicColumns.InPrimaryKey = (bool)dr[RMTableRefmVFeedFieldDetail.IS_PRIMARY];
                    objDynamicColumns.Nulable = true;
                    dynamicColumns.Add(objDynamicColumns);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                mLogger.Debug("RMDynamicDB : ManageEntityTypeTable -> End");
            }
            return dynamicColumns;
        }

        private List<RMColumnInfo> GetDynamicHistoryFeedColumns(int feedId)
        {
            mLogger.Debug("RMDynamicDB : ManageEntityTypeTable -> Start");
            DataSet dsFields = null;
            List<RMColumnInfo> dynamicColumns = null;
            try
            {
                dynamicColumns = new List<RMColumnInfo>();
                dsFields = new RMDataSourceDBManager(mDBCon).GetFieldsByFeedSummaryId(feedId);

                RMColumnInfo objDynamicColumns = null;
                foreach (DataRow dr in dsFields.Tables[0].Rows)
                {
                    objDynamicColumns = new RMColumnInfo();
                    objDynamicColumns.ColumnName = GetFeedColumnName((int)dr[RMTableRefmVFeedFieldDetail.RAD_FIELD_ID]);
                    objDynamicColumns.DataType = RMDBDataTypes.VARCHARMAX;
                    objDynamicColumns.Length = "";
                    objDynamicColumns.DefaultValue = "";
                    objDynamicColumns.IsUnique = false;
                    objDynamicColumns.InPrimaryKey = false;
                    objDynamicColumns.Nulable = true;
                    dynamicColumns.Add(objDynamicColumns);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                mLogger.Debug("RMDynamicDB : ManageEntityTypeTable -> End");
            }
            return dynamicColumns;
        }

        public string GetFeedColumnName(int fieldId)
        {
            return "Field_" + fieldId;
        }

        public string GetVendorArchiveFeedTable(int feedID)
        {
            return GetVendorFeedTable(feedID);
        }

        public string GetVendorFeedTable(int feedID)
        {
            return RMCommonConstants.TABLE_START_NAME + "Feed_" + feedID.ToString();
        }

        private void AddFileFieldDetails(RADFileFieldDetailsInfo objRADFileFieldDetailsInfo,
            RMFeedFieldDetailsInfo objRefMFeedFieldDetailsInfo, int[] ids, RDBConnectionManager connectionManager)
        {
            mLogger.Debug("Start->Insert File Summary");
            mList = new RHashlist();
            RHashlist mListOutput = new RHashlist();
            try
            {
                int feedSummaryId = ids[0];
                int fileId = ids[1];

                mList.Add(RMTableRefmVFeedSummary.FILE_ID, fileId);
                mList.Add(RMTableRefmVFeedSummary.FIELD_NAME, objRADFileFieldDetailsInfo.FieldName);
                mList.Add(RMTableRefmVFeedSummary.FIELD_DESCRIPTION,
                    objRADFileFieldDetailsInfo.FieldDescription);
                mList.Add(RMTableRefmVFeedSummary.START_INDEX,
                    objRADFileFieldDetailsInfo.StartIndex);
                mList.Add(RMTableRefmVFeedSummary.END_INDEX, objRADFileFieldDetailsInfo.EndIndex);
                mList.Add(RMTableRefmVFeedSummary.FIELD_POSITION,
                    objRADFileFieldDetailsInfo.FieldPosition);
                mList.Add(RMTableRefmVFeedSummary.MANDATORY, objRADFileFieldDetailsInfo.Mandatory);
                mList.Add(RMTableRefmVFeedSummary.PERSISTENCE,
                    objRADFileFieldDetailsInfo.Persistency);
                mList.Add(RMTableRefmVFeedSummary.VALIDATION,
                    objRADFileFieldDetailsInfo.Validation);
                mList.Add(RMTableRefmVFeedSummary.ALLOW_TRIM, objRADFileFieldDetailsInfo.AllowTrim);
                mList.Add(RMTableRefmVFeedSummary.FIELD_X_PATH,
                    objRADFileFieldDetailsInfo.FieldXPath);
                mList.Add(RMTableRefmVFeedSummary.REMOVE_WHITE_SPACES,
                    objRADFileFieldDetailsInfo.RemoveWhiteSpaces);
                mList.Add(RMTableRefmVFeedSummary.IS_ENCRYPTED,
                    objRADFileFieldDetailsInfo.Encrypted);
                mList.Add(RMDBCommonConstantsInfo.CREATED_BY, objRADFileFieldDetailsInfo.CreatedBy);
                mList.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY,
                    objRADFileFieldDetailsInfo.LastModifiedBy);
                mList.Add(RMTableRefmVFeedFieldDetail.FEED_SUMMARY_ID,
                    feedSummaryId);
                mList.Add(RMTableRefmVFeedFieldDetail.IS_BULK,
                    objRefMFeedFieldDetailsInfo.IsBulk);
                mList.Add(RMTableRefmVFeedFieldDetail.IS_FTP,
                    objRefMFeedFieldDetailsInfo.IsFTP);
                mList.Add(RMTableRefmVFeedFieldDetail.IS_API,
                    objRefMFeedFieldDetailsInfo.IsAPI);
                mList.Add(RMTableRefmVFeedFieldDetail.IS_PRIMARY,
                    objRefMFeedFieldDetailsInfo.IsPrimary);
                mList.Add(RMTableRefmVFeedFieldDetail.IS_UNIQUE,
                    objRefMFeedFieldDetailsInfo.IsUnique);
                mList.Add(RMTableRefmVFeedSummary.IS_PII,
                    objRADFileFieldDetailsInfo.IsPII);
                connectionManager.ExecuteProcedure
                     ("RefMVendor:InsertFeedFieldDetails", mList);
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("End->Insert File Summary");
            }
        }

        public void UpdateFileFieldDetails(RADFileFieldDetailsInfo objRADFileFieldDetailsInfo,
            RMFeedFieldDetailsInfo objRefMFeedFieldDetailsInfo, RDBConnectionManager connectionManager)
        {
            mLogger.Debug("Start->Update Feed Field Summary");
            mList = new RHashlist();
            RHashlist mListOutput = new RHashlist();
            try
            {
                mList.Add(RMTableRefmVFeedSummary.FIELD_ID, objRADFileFieldDetailsInfo.FieldId);
                mList.Add(RMTableRefmVFeedSummary.FILE_ID, objRADFileFieldDetailsInfo.FileId);
                mList.Add(RMTableRefmVFeedSummary.FIELD_NAME, objRADFileFieldDetailsInfo.FieldName);
                mList.Add(RMTableRefmVFeedSummary.FIELD_DESCRIPTION,
                    objRADFileFieldDetailsInfo.FieldDescription);
                mList.Add(RMTableRefmVFeedSummary.START_INDEX,
                    objRADFileFieldDetailsInfo.StartIndex);
                mList.Add(RMTableRefmVFeedSummary.END_INDEX, objRADFileFieldDetailsInfo.EndIndex);
                mList.Add(RMTableRefmVFeedSummary.FIELD_POSITION,
                    objRADFileFieldDetailsInfo.FieldPosition);
                mList.Add(RMTableRefmVFeedSummary.MANDATORY, objRADFileFieldDetailsInfo.Mandatory);
                mList.Add(RMTableRefmVFeedSummary.PERSISTENCE,
                    objRADFileFieldDetailsInfo.Persistency);
                mList.Add(RMTableRefmVFeedSummary.VALIDATION,
                    objRADFileFieldDetailsInfo.Validation);
                mList.Add(RMTableRefmVFeedSummary.ALLOW_TRIM, objRADFileFieldDetailsInfo.AllowTrim);
                mList.Add(RMTableRefmVFeedSummary.FIELD_X_PATH,
                    objRADFileFieldDetailsInfo.FieldXPath);
                mList.Add(RMTableRefmVFeedSummary.REMOVE_WHITE_SPACES,
                    objRADFileFieldDetailsInfo.RemoveWhiteSpaces);

                mList.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY,
                    objRADFileFieldDetailsInfo.LastModifiedBy);
                mList.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_ON,
                    objRADFileFieldDetailsInfo.LastModifiedOn.ToString("yyyy-MM-dd HH:mm:ss"));

                connectionManager.ExecuteQuery
                    ("REFM:UpdateRADFileFieldDetails", mList, true);

                mList.Clear();
                mList = null;
                mList = new RHashlist();

                mList.Add(RMTableRefmVFeedFieldDetail.FEED_FIELD_DETAILS_ID,
                    objRefMFeedFieldDetailsInfo.FeedFieldDetailsId);
                mList.Add(RMTableRefmVFeedFieldDetail.FEED_SUMMARY_ID,
                    objRefMFeedFieldDetailsInfo.FeedSummaryId);
                mList.Add(RMTableRefmVFeedSummary.RAD_FILE_ID,
                    objRefMFeedFieldDetailsInfo.RadFieldId);
                mList.Add(RMTableRefmVFeedFieldDetail.IS_BULK,
                    objRefMFeedFieldDetailsInfo.IsBulk);
                mList.Add(RMTableRefmVFeedFieldDetail.IS_FTP,
                    objRefMFeedFieldDetailsInfo.IsFTP);
                mList.Add(RMTableRefmVFeedFieldDetail.IS_API,
                    objRefMFeedFieldDetailsInfo.IsAPI);
                mList.Add(RMTableRefmVFeedFieldDetail.IS_PRIMARY,
                    objRefMFeedFieldDetailsInfo.IsPrimary);
                mList.Add(RMTableRefmVFeedFieldDetail.LAST_MODIFIED_BY,
                    objRADFileFieldDetailsInfo.LastModifiedBy);
                mList.Add(RMTableRefmVFeedFieldDetail.IS_UNIQUE,
                    objRefMFeedFieldDetailsInfo.IsUnique);
                mList.Add(RMTableRefmVFeedSummary.IS_ENCRYPTED,
                    objRADFileFieldDetailsInfo.Encrypted);
                mList.Add(RMTableRefmVFeedSummary.IS_PII,
                    objRADFileFieldDetailsInfo.IsPII);
                connectionManager.ExecuteQuery
                     ("REFM:UpdateRefMFeedFieldDetails", mList, true);
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("End->Insert Feed Field Summary");
            }
        }

        private int[] AddFileProperty(RADFilePropertyInfo objRADFilePropInfo,
            RMFeedSummaryInfo objRefMFileInfo, RDBConnectionManager connectionManager)
        {
            mLogger.Debug("Start->Insert File Summary");
            mList = new RHashlist();
            RHashlist mListOutput = new RHashlist();
            try
            {
                mList.Add(RMTableRefmVFeedSummary.FEED_NAME, objRADFilePropInfo.FeedName);
                mList.Add(RMTableRefmVFeedSummary.FILE_TYPE, objRADFilePropInfo.FileType);
                mList.Add(RMTableRefmVFeedSummary.ROW_DELIMITER, objRADFilePropInfo.RowDelimiter);
                mList.Add(RMTableRefmVFeedSummary.RECORD_LENGTH, objRADFilePropInfo.RecordLength);
                mList.Add(RMTableRefmVFeedSummary.FIELD_DELIMITER, objRADFilePropInfo.FieldDelimiter);
                mList.Add(RMTableRefmVFeedSummary.COMMENT_CHAR, objRADFilePropInfo.CommentChar);
                mList.Add(RMTableRefmVFeedSummary.SINGLE_ESCAPE, objRADFilePropInfo.SingleEscape);
                mList.Add(RMTableRefmVFeedSummary.PAIRED_ESCAPE, objRADFilePropInfo.PairedEscape);
                mList.Add(RMTableRefmVFeedSummary.ROOT_X_PATH, objRADFilePropInfo.RootXPath);
                mList.Add(RMTableRefmVFeedSummary.RECORD_X_PATH, objRADFilePropInfo.RecordXPath);
                mList.Add(RMTableRefmVFeedSummary.EXCLUDE_REGEX, objRADFilePropInfo.ExcludeRegEx);
                mList.Add(RMTableRefmVFeedSummary.FILE_DATE, RCalenderUtils.ConvertDateToISOFormat(objRADFilePropInfo.FileDate, RDateLengthFormat.Long));
                mList.Add(RMTableRefmVFeedSummary.FIELD_COUNT, objRADFilePropInfo.FieldCount);
                mList.Add(RMDBCommonConstantsInfo.CREATED_BY, objRADFilePropInfo.CreatedBy);
                mList.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objRADFilePropInfo.LastModifiedBy);
                mList.Add(RMTableRefmVFeedSummary.DATA_SOURCE_ID, objRefMFileInfo.DataSourceID);
                mList.Add(RMTableRefmVFeedSummary.FEED_TYPE_ID, objRefMFileInfo.FeedTypeID);
                mList.Add(RMTableRefmVFeedSummary.DB_PROVIDER, objRefMFileInfo.DBProvider);
                mList.Add(RMTableRefmVFeedSummary.CONNECTION_STRING, objRefMFileInfo.ConnectionString);
                mList.Add(RMTableRefmVFeedSummary.COLUMN_QUERY, objRefMFileInfo.ColumnQuery);
                mList.Add(RMTableRefmVFeedSummary.IS_COMPLETE, objRefMFileInfo.IsComplete);
                mList.Add(RMTableRefmVFeedSummary.FEED_SUMMARY_ID, objRefMFileInfo.FeedSummaryID);

                mListOutput = connectionManager.ExecuteProcedure
                    ("RefMVendor:InsertFeedDetails", mList);
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("End->Insert File Summary");
            }
            DataSet ds = (DataSet)mListOutput[0];
            int[] ids = new int[2];
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                ids[0] = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                ids[1] = Convert.ToInt32(ds.Tables[0].Rows[0][1]);
                return ids;
            }
            return null;
        }

        private void UpdateFileProperty(RADFilePropertyInfo objRADFilePropInfo,
                    RMFeedSummaryInfo objRefMFileInfo, RDBConnectionManager connectionManager)
        {
            mLogger.Debug("Start->Insert File Summary");
            mList = new RHashlist();
            RHashlist mListOutput = new RHashlist();
            try
            {
                mList.Add(RMTableRefmVFeedSummary.FILE_ID, objRADFilePropInfo.FileId);
                mList.Add(RMTableRefmVFeedSummary.FEED_NAME, objRADFilePropInfo.FeedName);
                mList.Add(RMTableRefmVFeedSummary.FILE_TYPE, objRADFilePropInfo.FileType);
                mList.Add(RMTableRefmVFeedSummary.ROW_DELIMITER, objRADFilePropInfo.RowDelimiter);
                mList.Add(RMTableRefmVFeedSummary.RECORD_LENGTH, objRADFilePropInfo.RecordLength);
                mList.Add(RMTableRefmVFeedSummary.FIELD_DELIMITER, objRADFilePropInfo.FieldDelimiter);
                mList.Add(RMTableRefmVFeedSummary.COMMENT_CHAR, objRADFilePropInfo.CommentChar);
                mList.Add(RMTableRefmVFeedSummary.SINGLE_ESCAPE, objRADFilePropInfo.SingleEscape);
                mList.Add(RMTableRefmVFeedSummary.PAIRED_ESCAPE, objRADFilePropInfo.PairedEscape);
                mList.Add(RMTableRefmVFeedSummary.ROOT_X_PATH, objRADFilePropInfo.RootXPath);
                mList.Add(RMTableRefmVFeedSummary.RECORD_X_PATH, objRADFilePropInfo.RecordXPath);
                mList.Add(RMTableRefmVFeedSummary.EXCLUDE_REGEX, objRADFilePropInfo.ExcludeRegEx);
                mList.Add(RMTableRefmVFeedSummary.FILE_DATE, RCalenderUtils.ConvertDateToISOFormat(objRADFilePropInfo.FileDate, RDateLengthFormat.Long));
                mList.Add(RMTableRefmVFeedSummary.FIELD_COUNT, objRADFilePropInfo.FieldCount);
                mList.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objRADFilePropInfo.LastModifiedBy);

                connectionManager.ExecuteQuery
                   ("REFM:UpdateRADFileProperty", mList, true);

                mList.Clear();
                mList = null;
                mList = new RHashlist();

                mList.Add(RMTableRefmVFeedSummary.FEED_SUMMARY_ID, objRefMFileInfo.FeedSummaryID);
                mList.Add(RMTableRefmVFeedSummary.FEED_NAME, objRefMFileInfo.FeedName);
                mList.Add(RMTableRefmVFeedSummary.DATA_SOURCE_ID, objRefMFileInfo.DataSourceID);
                mList.Add(RMTableRefmVFeedSummary.FEED_TYPE_ID, objRefMFileInfo.FeedTypeID);
                mList.Add(RMTableRefmVFeedSummary.RAD_FILE_ID, objRefMFileInfo.RadFileID);
                mList.Add(RMTableRefmVFeedSummary.DB_PROVIDER, objRefMFileInfo.DBProvider);
                mList.Add(RMTableRefmVFeedSummary.CONNECTION_STRING, objRefMFileInfo.ConnectionString);
                mList.Add(RMTableRefmVFeedSummary.COLUMN_QUERY, objRefMFileInfo.ColumnQuery);
                mList.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objRADFilePropInfo.LastModifiedBy);
                mList.Add(RMTableRefmVFeedSummary.IS_COMPLETE, objRefMFileInfo.IsComplete);

                connectionManager.ExecuteQuery
                    ("REFM:UpdateRefMFeedSummary", mList, true);
            }
            catch
            {
                mLogger.Debug("RMDataSourceDBManager -> UpdateDatasource -> Error");
                throw;
            }
            finally
            {
                if (mList != null)
                    mList.Dispose();
                mLogger.Debug("End->Insert File Summary");
            }
        }


        public void AddUpdateFeedMappingDetails(List<RMFeedMappingInfo> feedMappingInfo, int fileID, int feedSummaryID)
        {
            mLogger.Debug("Start->Insert Feed Summary");
            bool isConnectionCreated = false;

            if (fileID <= 0 && feedSummaryID > 0)
            {
                fileID = GetFileIdFromFeedSummaryID(feedSummaryID, mDBCon);
            }

            Dictionary<string, int> newUnmappedColumnDictionary = new Dictionary<string, int>();

            if (mDBCon == null)
            {
                isConnectionCreated = true;
                mDBCon = RDALAbstractFactory.DBFactory.GetConnectionManager(RConfigReader.GetConfigAppSettings(ConnectionConstants.RefMasterVendor_Connection.ToString()));
            }
            try
            {
                RMFeedMappingInfo objFeedMappingInfo;
                for (int i = 0; i < feedMappingInfo.Count; i++)
                {
                    objFeedMappingInfo = feedMappingInfo[i];
                    if ((objFeedMappingInfo.PrimaryColumnId < 0) && (newUnmappedColumnDictionary.ContainsKey(objFeedMappingInfo.PrimaryColumnName) == false))
                    {
                        objFeedMappingInfo.PrimaryColumnId = GetFieldID(objFeedMappingInfo, fileID, objFeedMappingInfo.PrimaryColumnName, mDBCon);
                        newUnmappedColumnDictionary[objFeedMappingInfo.PrimaryColumnName] = objFeedMappingInfo.PrimaryColumnId;
                    }
                    if ((objFeedMappingInfo.MappedColumnId <= 0) && (newUnmappedColumnDictionary.ContainsKey(objFeedMappingInfo.MappedColumnName) == false))
                    {
                        objFeedMappingInfo.MappedColumnId = GetFieldID(objFeedMappingInfo, fileID, objFeedMappingInfo.MappedColumnName, mDBCon);
                        newUnmappedColumnDictionary[objFeedMappingInfo.MappedColumnName] = objFeedMappingInfo.MappedColumnId;
                    }
                }

                for (int i = 0; i < feedMappingInfo.Count; i++)
                {
                    objFeedMappingInfo = feedMappingInfo[i];

                    if ((objFeedMappingInfo.PrimaryColumnId < 0) && (newUnmappedColumnDictionary.ContainsKey(objFeedMappingInfo.PrimaryColumnName) == true))
                    {
                        objFeedMappingInfo.PrimaryColumnId = newUnmappedColumnDictionary[objFeedMappingInfo.PrimaryColumnName];
                    }
                    if ((objFeedMappingInfo.MappedColumnId <= 0) && (newUnmappedColumnDictionary.ContainsKey(objFeedMappingInfo.MappedColumnName) == true))
                    {
                        objFeedMappingInfo.MappedColumnId = newUnmappedColumnDictionary[objFeedMappingInfo.MappedColumnName];
                    }

                    if (objFeedMappingInfo.FeedMappingDetailId < 0)
                    {

                        AddFeedMappingDetails(objFeedMappingInfo, fileID, mDBCon);
                    }
                    else
                        UpdateFeedMappingDetails(objFeedMappingInfo, fileID, mDBCon);
                }

                ManageFeedTable(feedSummaryID, mDBCon);
                ManageFeedTablesForEncryption(feedSummaryID);
                CreateFeedViewWithFieldName(feedSummaryID, mDBCon);

                if (isConnectionCreated)
                    mDBCon.CommitTransaction();
            }
            catch (Exception ex)
            {
                if (isConnectionCreated)
                    mDBCon.RollbackTransaction();
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                if (isConnectionCreated)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
            }

        }

        private int GetFileIdFromFeedSummaryID(int feedSummaryID, RDBConnectionManager conn = null)
        {
            mLogger.Debug("RMDataSourceDBManager: GetFileIdFromFeedSummaryID -> Start");
            int fileID = -1;
            try
            {
                DataSet ds = null;
                string query = " SELECT rad_file_id FROM IVPRefMasterVendor.dbo.ivp_refm_feed_summary WHERE feed_summary_id = " + feedSummaryID + " ";

                if (conn != null)
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, conn);
                else
                    ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMasterVendor_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    fileID = Convert.ToInt32(ds.Tables[0].Rows[0]["rad_file_id"]);

                return fileID;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("RMDataSourceDBManager: GetFileIdFromFeedSummaryID -> End");
            }
        }

        private void AddFeedMappingDetails(RMFeedMappingInfo objFeedMappingInfo, int fileID, RDBConnectionManager conn)
        {
            mLogger.Debug("RMFeedMappingController:AddFeedMappingDetails -> Start Adding Feed Mapping Details");
            RHashlist htParams = new RHashlist();
            try
            {
                DataSet mappingId = new DataSet();
                htParams.Add(RMTableRefmVFeedMapping.FEED_SUMMARY_ID, objFeedMappingInfo.FeedSummaryId);
                htParams.Add(RMTableRefmVFeedMapping.PRIMARY_COL_ID, objFeedMappingInfo.PrimaryColumnId);
                htParams.Add(RMTableRefmVFeedMapping.MAP_ID, objFeedMappingInfo.MapId);
                htParams.Add(RMDBCommonConstantsInfo.CREATED_BY, objFeedMappingInfo.CreatedBy);
                htParams.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objFeedMappingInfo.LastModifiedBy);
                htParams.Add(RMTableRefmVFeedMapping.MAPPED_COL_ID, objFeedMappingInfo.MappedColumnId);
                htParams.Add(RMTableRefmVFeedMapping.MAP_STATE, objFeedMappingInfo.MapState);
                CommonDALWrapper.ExecuteSelectQuery("REFMVendor:InsertFeedMappingDetail", htParams, conn);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {

                htParams.Clear();
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("RMFeedMappingController:AddFeedMappingDetails -> End Adding Feed Mapping Details");
            }
        }

        private void UpdateFeedMappingDetails(RMFeedMappingInfo objFeedMappingInfo, int fileID, RDBConnectionManager conn)
        {
            mLogger.Debug("RMFeedMappingController:UpdateFeedMappingDetails -> Start Update Feed Mapping Details");
            RHashlist htParams = new RHashlist();
            try
            {
                htParams.Add(RMTableRefmVFeedMapping.FEED_MAPPING_DETAIL_ID, objFeedMappingInfo.FeedMappingDetailId);
                htParams.Add(RMTableRefmVFeedMapping.PRIMARY_COL_ID, objFeedMappingInfo.PrimaryColumnId);
                htParams.Add(RMTableRefmVFeedMapping.MAP_ID, objFeedMappingInfo.MapId);
                htParams.Add(RMTableRefmVFeedMapping.MAP_STATE, objFeedMappingInfo.MapState);
                htParams.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objFeedMappingInfo.LastModifiedBy);
                htParams.Add(RMTableRefmVFeedMapping.FIELD_NAME, objFeedMappingInfo.MappedColumnName);
                CommonDALWrapper.ExecuteQuery("RefMVendor:UpdateFeedMappingDetails", htParams, conn);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                htParams.Clear();
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("RMFeedMappingController:UpdateFeedMappingDetails -> End Update Feed Mapping Details");
            }
        }

        private int GetFieldID(RMFeedMappingInfo objFeedMappingInfo, int fileID, string ColumnName, RDBConnectionManager conn)
        {
            RHashlist htParams = new RHashlist();
            try
            {
                DataSet mappingId = new DataSet();
                htParams.Add(RMDBCommonConstantsInfo.CREATED_BY, objFeedMappingInfo.CreatedBy);
                htParams.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objFeedMappingInfo.LastModifiedBy);
                htParams.Add("file_id", fileID);
                htParams.Add("field_name", ColumnName);
                mappingId = CommonDALWrapper.ExecuteSelectQuery("REFMVendor:InsertFileFieldDetails", htParams, conn);
                return (int)mappingId.Tables[0].Rows[0].ItemArray[0];
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {

                htParams.Clear();
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("RMFeedMappingController:GetFieldID -> End Adding Feed Details");
            }

        }

    }
}
