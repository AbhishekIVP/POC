using com.ivp.commom.commondal;
using com.ivp.common.srmdwhjob;
using com.ivp.rad.common;
using com.ivp.rad.RUserManagement;
using com.ivp.srmcommon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;

namespace com.ivp.srm.dwhdownstream
{
    public class SRMDWHAPI : ISRMDWHAPI
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMDWHAPI");

        public SRMDWHOutputInfo RollData(SRMDWHInputInfo inputInfo)
        {
            SRMDWHOutputInfo outputInfo = new SRMDWHOutputInfo() { IsSuccess = true, FailureReason = string.Empty };
            mLogger.Debug("SRMDWHAPI:RollData->Start ");
            try
            {
                mLogger.Error("API Name -> RollData");

                string clientName = SRMMTConfig.SetClientNameFromHeaders(false);

                string sqlQuery = "SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master WHERE setup_name = '" + inputInfo.SetupName + "' AND is_active = 1";
                DataSet dwhSetup = CommonDALWrapper.ExecuteSelectQuery(sqlQuery, ConnectionConstants.RefMaster_Connection);

                if (dwhSetup != null && dwhSetup.Tables.Count > 0 && dwhSetup.Tables[0].Rows.Count > 0)
                {
                    var rollingInfo = new RollingInfo
                    {
                        SetupId = Convert.ToInt32(dwhSetup.Tables[0].Rows[0]["setup_id"]),
                        ConnectionName = Convert.ToString(dwhSetup.Tables[0].Rows[0]["connection_name"]),
                        TimeToRun = DateTime.Now,
                        T_plus_1 = false
                    };

                    if (inputInfo.WaitForResponse)
                    {
                        bool canRun = true; int attempt = 0;
                        while (canRun)
                        {
                            attempt++;
                            if (attempt < 100)
                            {
                                bool sleep = true;
                                lock (SRMDWHStatic.lockObject)
                                {
                                    if (!SRMDWHStatic.RunningSetups[clientName].Contains((rollingInfo.SetupId)) && (!SRMDWHStatic.RunningRollData[clientName].Contains(rollingInfo.SetupId)))
                                    {
                                        sleep = false;
                                        mLogger.Error("Adding RunningRollData for setup Id : " + rollingInfo.SetupId + " and clientName : " + clientName) ;
                                        SRMDWHStatic.RunningRollData[clientName].Add(rollingInfo.SetupId);
                                    }
                                }
                                if (sleep)
                                    Thread.Sleep(10000);
                                else
                                {
                                    canRun = false;
                                    SRMDWHJob.RollData(clientName, rollingInfo);
                                }
                            }
                            else
                                canRun = false;
                        }
                        if (attempt == 100)
                        {
                            mLogger.Error("Cannot roll since it is waiting for more than 15 mins for setup : " + inputInfo.SetupName);
                            outputInfo.FailureReason = "Cannot roll since it is waiting for more than 15 mins for setup : " + inputInfo.SetupName;
                            outputInfo.IsSuccess = false;
                        }
                    }
                    else
                    {
                        SRMDWHJobQueue.EnqueueRolling(clientName, rollingInfo);
                    }
                }
                else
                {
                    mLogger.Error("Invalid Setup Name : " + inputInfo.SetupName + " received for RollData API");
                    outputInfo.FailureReason = "Invalid Setup Name : " + inputInfo.SetupName;
                    outputInfo.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                outputInfo.FailureReason = ex.Message;
                outputInfo.IsSuccess = false;
                mLogger.Error("Exception Encountered : RollData -> " + ex.ToString());
            }
            mLogger.Debug("SRMDWHAPI:RollData->End ");
            return outputInfo;
        }

        public SRMDWHSyncOutputInfo TriggerDWHSync(SRMDWHInputInfo inputInfo)
        {
            SRMDWHSyncOutputInfo outputInfo = new SRMDWHSyncOutputInfo() { IsSuccess = true, FailureReason = string.Empty };
            string errrorMessage = string.Empty;

            mLogger.Debug("SRMDWHAPI:TriggerDWHSync->Start ");
            try
            {
                string clientName = SRMMTConfig.SetClientNameFromHeaders(false);

                mLogger.Error("API Name -> TriggerDWHSync");
                mLogger.Error("User Name -> " + inputInfo.UserName);
                inputInfo.UserName = GetUserLoginNameFromUserName(inputInfo.UserName);

                string sqlQuery = "SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master WHERE setup_name = '" + inputInfo.SetupName + "' AND is_active = 1";
                DataSet dwhSetup = CommonDALWrapper.ExecuteSelectQuery(sqlQuery, ConnectionConstants.RefMaster_Connection);
                if (dwhSetup != null && dwhSetup.Tables.Count > 0 && dwhSetup.Tables[0].Rows.Count > 0)
                {
                    string downstreamConnectionName = Convert.ToString(dwhSetup.Tables[0].Rows[0]["connection_name"]);
                    int setupId = Convert.ToInt32(dwhSetup.Tables[0].Rows[0]["setup_id"]);

                    outputInfo.SetupStatusId = SRMDWHJobQueue.Enqueue(clientName,setupId, inputInfo.UserName);
                    mLogger.Debug("SRMDWHAPI:TriggerDWHSync->SetupStatusId : " + outputInfo.SetupStatusId + " and clientName : " + clientName);

                }
                else
                {
                    mLogger.Error("Invalid Setup Name : " + inputInfo.SetupName + " received for RollData API");
                    outputInfo.FailureReason = "Invalid Setup Name : " + inputInfo.SetupName;
                    outputInfo.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                outputInfo.FailureReason = ex.Message;
                outputInfo.IsSuccess = false;
                mLogger.Error("Exception Encountered : TriggerDWHSync -> " + ex.ToString());
            }
            mLogger.Debug("SRMDWHAPI:TriggerDWHSync->End ");
            return outputInfo;
        }

        public void GetWorkerResponse(SRMDWHInternalInfo inputInfo)
        {
            mLogger.Debug("SRMDWHAPI:GetWorkerResponse->Start for setup status id : " + inputInfo.SetupStatusId + " and block executed : " + inputInfo.BlockExecuted + " with transaction = " + inputInfo.InTransaction);

            mLogger.Debug("GetWorkerResponse Object : " + JsonConvert.SerializeObject(inputInfo));

            try
            {
                string clientName = SRMMTConfig.SetClientNameFromHeaders(false);

                if (!string.IsNullOrEmpty(inputInfo.SetupName))
                {
                    string query = string.Format(@"SELECT st.setup_id, st.setup_status_id , st.created_by,  mas.connection_name
                                    FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_setup_status st
                                    INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_master mas
                                    ON (st.setup_id = mas.setup_id AND mas.is_active = 1 AND st.is_active = 1)
                                    WHERE status = 'INPROGRESS' AND mas.setup_name = '{0}' ", inputInfo.SetupName);
                    DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        int setupId = Convert.ToInt32(ds.Tables[0].Rows[0]["setup_id"]);
                        int setupStatusId = Convert.ToInt32(ds.Tables[0].Rows[0]["setup_status_id"]);
                        string userName = Convert.ToString(ds.Tables[0].Rows[0]["created_by"]);
                        string connectionName = Convert.ToString(ds.Tables[0].Rows[0]["connection_name"]);
                        bool workerResponse = false;
                        string errorMessage = inputInfo.ErrorMessage;
                        new SRMDWHJob().ExecuteDWHSubTask(clientName,setupId, userName,setupStatusId,inputInfo.CacheKey, "failed", ref errorMessage, false,ref workerResponse);
                    }                    
                }
                else
                    new SRMDWHJob().ExecuteDWHTask(clientName, inputInfo.SetupId, inputInfo.UserName, inputInfo.SetupStatusId, inputInfo.BlockExecuted, inputInfo.InTransaction, inputInfo.BlockStatus, inputInfo.ErrorMessage, inputInfo.CacheKey);
            }
            catch (Exception ex)
            {
                mLogger.Error("Exception Encountered : GetWorkerResponse -> " + ex.ToString());
            }
            finally
            {
                mLogger.Debug("SRMDWHAPI:GetWorkerResponse->End ");

            }
        }

        public SRMDWHSyncStatus CheckSyncStatus(string setupStatusId)
        {
            mLogger.Debug("SRMDWHAPI:CheckSyncStatus->Start ");
            SRMDWHSyncStatus outputInfo = new SRMDWHSyncStatus() { IsSuccess = true, FailureReason = string.Empty };
            try
            {
                string clientName = SRMMTConfig.SetClientNameFromHeaders(false);

                string query = string.Format("SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_setup_status WHERE setup_status_id = {0}", setupStatusId);
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    outputInfo.Status = Convert.ToString(ds.Tables[0].Rows[0]["status"]);
                    outputInfo.StartTime = Convert.ToDateTime(ds.Tables[0].Rows[0]["start_time"]).ToString("yyyyMMdd HH:mm:ss.fff");
                    if (!string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["end_time"])))
                        outputInfo.EndTime = Convert.ToDateTime(ds.Tables[0].Rows[0]["end_time"]).ToString("yyyyMMdd HH:mm:ss.fff");
                    if (outputInfo.Status.Equals("FAILED"))
                    {
                        outputInfo.FailureReason = Convert.ToString(ds.Tables[0].Rows[0]["failure_reason"]);
                        outputInfo.IsSuccess = false;
                    }
                }
                else
                {
                    outputInfo.IsSuccess = false;
                    outputInfo.FailureReason = "Invalid Setup StatusId : " + setupStatusId;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("Exception Encountered : CheckSyncStatus -> " + ex.ToString());
                outputInfo.IsSuccess = false;
                outputInfo.FailureReason = ex.Message;
            }
            finally
            {
                mLogger.Debug("SRMDWHAPI:CheckSyncStatus->End ");
            }
            return outputInfo;
        }

        private string GetUserLoginNameFromUserName(string username)
        {
            RUserInfo objUserInfo = new RUserInfo();
            if (username.ToLower().Equals("admin"))
                return "admin";
            else
            {
                try
                {
                    objUserInfo = new RUserManagementService().GetUserInfoGDPR(username);
                    if (objUserInfo == null || string.IsNullOrEmpty(objUserInfo.UserLoginName))
                    {
                        return username;
                    }
                    else
                        return objUserInfo.UserLoginName;
                }
                catch (Exception ex)
                {
                    mLogger.Error("Error in fetching user details for - " + username + " Error : " + ex.ToString());
                }
                return username;
            }
        }
    }
}