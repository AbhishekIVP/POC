using com.ivp.commom.commondal;
using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.notificationmanager;
using com.ivp.rad.transport;
using com.ivp.rad.utils;
using com.ivp.srmcommon;
using DWHAdapter;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace com.ivp.common.srmdwhjob
{
    public static class SRMDWHStatic
    {
        public static object lockObject = new object();
        public static object lockObjectForStatusUpdate = new object();
        public static object lockObjectForSecAndRefParallelRun = new object();

        public static ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, Dictionary<int, SRMJobInfo>>>> cachedReportIdVsInfo = new ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, Dictionary<int, SRMJobInfo>>>>();
        public static ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, List<int>>>> cachedModuleVsReportsProcessed = new ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, List<int>>>>();
        public static ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, List<int>>>> cachedModuleVsLegReports = new ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, List<int>>>>();
        public static ConcurrentDictionary<string, ConcurrentDictionary<string, DateTime>> cachedLoadingTimeForStagingTable = new ConcurrentDictionary<string, ConcurrentDictionary<string, DateTime>>();
        public static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> cachedDownstreamConnectionName = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        public static ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<int, List<DataRow>>>> cachedDictBlockTypeVsData = new ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<int, List<DataRow>>>>();

        public static ConcurrentDictionary<string, HashSet<int>> RunningSetups = new ConcurrentDictionary<string, HashSet<int>>();
        public static ConcurrentDictionary<string, HashSet<int>> RunningRollData = new ConcurrentDictionary<string, HashSet<int>>();
        public static ConcurrentDictionary<string, Dictionary<int, RollingSetupInfo>> SetupVsLastRolledTime = new ConcurrentDictionary<string, Dictionary<int, RollingSetupInfo>>();

        public static ConcurrentDictionary<int, List<DWHAdapterDetails>> dctSetupIdVsDWHAdapterDetails = new ConcurrentDictionary<int, List<DWHAdapterDetails>>();

        public static ConcurrentDictionary<string, Dictionary<int, bool>> dctSetupIdVsParallelTriggered = new ConcurrentDictionary<string, Dictionary<int, bool>>();
        public static ConcurrentDictionary<string, Dictionary<int, HashSet<string>>> dctSetupIdVsModuleStatus = new ConcurrentDictionary<string, Dictionary<int, HashSet<string>>>();
        public static ConcurrentDictionary<string, Dictionary<int, DWHStatus>> dctSetupIdVsGlobalStatus = new ConcurrentDictionary<string, Dictionary<int, DWHStatus>>();
    }
    public class SRMDWHJob
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMDWHJob");
        public static string SetLookupColumnNamesInDWHViews = SRMCommon.GetConfigFromDB("SetLookupColumnNamesInDWHViews");
        public static string RunRefDWHTasksInParallel = SRMCommon.GetConfigFromDB("RunRefDWHTasksInParallel");
        public static string AllowECinColumnsInDWHViews = SRMCommon.GetConfigFromDB("AllowECinColumnsInDWHViews");
        public static string RefSecLookupAttributesWithSecId = SRMCommon.GetConfigFromDB("IgnoreSecurityLookupMassagingInRefReport");
        public static bool SkipCreateDWHViews = string.IsNullOrEmpty(SRMCommon.GetConfigFromDB("DWHSkipViewCreation")) ? false : bool.Parse(SRMCommon.GetConfigFromDB("DWHSkipViewCreation"));

        public static bool RunRefSecDWHInParallel = string.IsNullOrEmpty(SRMCommon.GetConfigFromDB("RunRefSecDWHInParallel")) ? false : bool.Parse(SRMCommon.GetConfigFromDB("RunRefSecDWHInParallel"));
        public static bool CreateMappingTableAndIntraDayView = string.IsNullOrEmpty(SRMCommon.GetConfigFromDB("DWHCreateMappingTableAndIntradayView")) ? false : bool.Parse(SRMCommon.GetConfigFromDB("DWHCreateMappingTableAndIntradayView"));

        const string REFM_CONSTANT = "REFM";
        const string SECM_CONSTANT = "SECM";

        private static object lockObject = new object();

        public string ExecuteDWHTaskJob(string clientName, int setupId, string userName, int setupStatusId)
        {
            //Thread.Sleep(5000);
            List<SRMDWHBlockStatus> blockStatus = new List<SRMDWHBlockStatus>();
            return ExecuteDWHTask(clientName, setupId, userName, setupStatusId, string.Empty, false, blockStatus);
        }

        public string ExecuteDWHTask(string clientName, int setupId, string userName, int setupStatusId, string blockExecuted, bool inTransaction, List<SRMDWHBlockStatus> blockStatus, string errorMessage = "", string cacheKey = "")
        {
            mLogger.Debug(string.Format("Start -> ExecuteDWHTask for clientName : {0} and setupId : {1} and setupStatusId : {2} and blockExecuted : {3} and transaction : {4} and with error Message : {5}", clientName, setupId, setupStatusId, blockExecuted, inTransaction, errorMessage));

            bool waitForWorkerResponse = false;

            if (blockStatus.Count > 0)
            {
                lock (SRMDWHStatic.lockObjectForStatusUpdate)
                {
                    foreach (var block in blockStatus)
                    {
                        mLogger.Debug("Is block NULL : " + (block == null ? "true" : "false"));
                        InsertUpdateStatusForBlock(block.BlockStatusId, 0, block.status, userName, block.ErrorMessage, block.BlockStatusId, block.EndTime);
                        if (block.status == EXECUTION_STATUS.FAILED)
                        {
                            SRMJobInfo jobInfo = null;

                            if (SRMDWHStatic.cachedReportIdVsInfo[clientName].ContainsKey(cacheKey))
                            {
                                if (SRMDWHStatic.cachedReportIdVsInfo[clientName][cacheKey].ContainsKey(REFM_CONSTANT))
                                {
                                    jobInfo = SRMDWHStatic.cachedReportIdVsInfo[clientName][cacheKey][REFM_CONSTANT].Where(x => x.Value.BlockId == block.BlockId).Select(x => x.Value).FirstOrDefault();
                                    if (jobInfo == null)
                                    {
                                        if (SRMDWHStatic.cachedReportIdVsInfo[clientName][cacheKey].ContainsKey(SECM_CONSTANT))
                                        {
                                            jobInfo = SRMDWHStatic.cachedReportIdVsInfo[clientName][cacheKey][SECM_CONSTANT].Where(x => x.Value.BlockId == block.BlockId).Select(x => x.Value).FirstOrDefault();
                                        }
                                    }
                                }
                                if (SRMDWHStatic.cachedReportIdVsInfo[clientName][cacheKey].ContainsKey(SECM_CONSTANT))
                                {
                                    jobInfo = SRMDWHStatic.cachedReportIdVsInfo[clientName][cacheKey][SECM_CONSTANT].Where(x => x.Value.BlockId == block.BlockId).Select(x => x.Value).FirstOrDefault();
                                }
                            }
                            if (jobInfo != null)
                                SendSubscriptionMail(jobInfo, block.ErrorMessage);
                        }
                    }
                    blockStatus = new List<SRMDWHBlockStatus>();
                }
            }

            if (!RunRefSecDWHInParallel)
            {
                if (string.IsNullOrEmpty(cacheKey))
                {
                    cacheKey = Guid.NewGuid().ToString();

                    if (!SRMDWHStatic.cachedReportIdVsInfo.ContainsKey(clientName))
                        SRMDWHStatic.cachedReportIdVsInfo.TryAdd(clientName, new ConcurrentDictionary<string, Dictionary<string, Dictionary<int, SRMJobInfo>>>());

                    if (!SRMDWHStatic.cachedModuleVsReportsProcessed.ContainsKey(clientName))
                        SRMDWHStatic.cachedModuleVsReportsProcessed.TryAdd(clientName, new ConcurrentDictionary<string, Dictionary<string, List<int>>>());

                    if (!SRMDWHStatic.cachedModuleVsLegReports.ContainsKey(clientName))
                        SRMDWHStatic.cachedModuleVsLegReports.TryAdd(clientName, new ConcurrentDictionary<string, Dictionary<string, List<int>>>());

                    if (!SRMDWHStatic.cachedLoadingTimeForStagingTable.ContainsKey(clientName))
                        SRMDWHStatic.cachedLoadingTimeForStagingTable.TryAdd(clientName, new ConcurrentDictionary<string, DateTime>());

                    if (!SRMDWHStatic.cachedDownstreamConnectionName.ContainsKey(clientName))
                        SRMDWHStatic.cachedDownstreamConnectionName.TryAdd(clientName, new ConcurrentDictionary<string, string>());

                    if (!SRMDWHStatic.cachedDictBlockTypeVsData.ContainsKey(clientName))
                        SRMDWHStatic.cachedDictBlockTypeVsData.TryAdd(clientName, new ConcurrentDictionary<string, Dictionary<int, List<DataRow>>>());

                    if (!SRMDWHStatic.SetupVsLastRolledTime.ContainsKey(clientName))
                        SRMDWHStatic.SetupVsLastRolledTime.TryAdd(clientName, new Dictionary<int, RollingSetupInfo>());

                    if (!SRMDWHStatic.RunningRollData.ContainsKey(clientName))
                        SRMDWHStatic.RunningRollData.TryAdd(clientName, new HashSet<int>());

                    if (!SRMDWHStatic.RunningSetups.ContainsKey(clientName))
                        SRMDWHStatic.RunningSetups.TryAdd(clientName, new HashSet<int>());

                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, REFM_CONSTANT + ":nts", ref errorMessage, inTransaction, ref waitForWorkerResponse);

                    if (!waitForWorkerResponse)
                    {
                        if (string.IsNullOrEmpty(errorMessage))
                            ExecuteDWHTask(clientName, setupId, userName, setupStatusId, REFM_CONSTANT + ":nts", false, blockStatus, errorMessage, cacheKey);
                        else
                            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                    }
                }

                else if (blockExecuted.Equals(REFM_CONSTANT + ":nts") && string.IsNullOrEmpty(errorMessage))
                {
                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, SECM_CONSTANT + ":nts", ref errorMessage, inTransaction, ref waitForWorkerResponse);

                    if (!waitForWorkerResponse)
                    {
                        if (string.IsNullOrEmpty(errorMessage))
                            ExecuteDWHTask(clientName, setupId, userName, setupStatusId, SECM_CONSTANT + ":nts", true, blockStatus, errorMessage, cacheKey);
                        else
                            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                    }

                }

                else if (blockExecuted.Equals(SECM_CONSTANT + ":nts") && string.IsNullOrEmpty(errorMessage))
                {
                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, REFM_CONSTANT + ":ts", ref errorMessage, inTransaction, ref waitForWorkerResponse);

                    if (!waitForWorkerResponse)
                    {
                        if (string.IsNullOrEmpty(errorMessage) && inTransaction)
                            ExecuteDWHTask(clientName, setupId, userName, setupStatusId, SECM_CONSTANT + ":nts", false, blockStatus, errorMessage, cacheKey);

                        else if (string.IsNullOrEmpty(errorMessage) && !inTransaction)
                            ExecuteDWHTask(clientName, setupId, userName, setupStatusId, REFM_CONSTANT + ":ts", true, blockStatus, errorMessage, cacheKey);

                        else if (!string.IsNullOrEmpty(errorMessage))
                            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                    }

                }

                else if (blockExecuted.Equals(REFM_CONSTANT + ":ts") && string.IsNullOrEmpty(errorMessage))
                {
                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, SECM_CONSTANT + ":ts", ref errorMessage, inTransaction, ref waitForWorkerResponse);

                    if (!waitForWorkerResponse)
                    {
                        if (string.IsNullOrEmpty(errorMessage) && inTransaction)
                            ExecuteDWHTask(clientName, setupId, userName, setupStatusId, REFM_CONSTANT + ":ts", false, blockStatus, errorMessage, cacheKey);

                        else if (string.IsNullOrEmpty(errorMessage) && !inTransaction)
                            ExecuteDWHTask(clientName, setupId, userName, setupStatusId, SECM_CONSTANT + ":ts", false, blockStatus, errorMessage, cacheKey);

                        else if (!string.IsNullOrEmpty(errorMessage))
                            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                    }
                }

                else if (blockExecuted.Equals(SECM_CONSTANT + ":ts") && string.IsNullOrEmpty(errorMessage))
                {
                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, REFM_CONSTANT + ":daily", ref errorMessage, inTransaction, ref waitForWorkerResponse);

                    if (!waitForWorkerResponse)
                    {
                        if (string.IsNullOrEmpty(errorMessage))
                            ExecuteDWHTask(clientName, setupId, userName, setupStatusId, REFM_CONSTANT + ":daily", false, blockStatus, errorMessage, cacheKey);
                        else
                            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                    }

                }

                else if (blockExecuted.Equals(REFM_CONSTANT + ":daily") && string.IsNullOrEmpty(errorMessage))
                {
                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, SECM_CONSTANT + ":daily", ref errorMessage, inTransaction, ref waitForWorkerResponse);

                    if (!waitForWorkerResponse)
                    {
                        if (string.IsNullOrEmpty(errorMessage))
                            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "passed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                        else
                            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                    }
                }

                else if (blockExecuted.Equals(SECM_CONSTANT + ":daily") && string.IsNullOrEmpty(errorMessage))
                {
                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "passed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                }

                else if (!string.IsNullOrEmpty(errorMessage))
                {
                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(cacheKey))
                {
                    lock (SRMDWHStatic.lockObjectForSecAndRefParallelRun)
                    {
                        if (!SRMDWHStatic.dctSetupIdVsParallelTriggered.ContainsKey(clientName))
                            SRMDWHStatic.dctSetupIdVsParallelTriggered.TryAdd(clientName, new Dictionary<int, bool>());

                        if (!SRMDWHStatic.dctSetupIdVsModuleStatus.ContainsKey(clientName))
                            SRMDWHStatic.dctSetupIdVsModuleStatus.TryAdd(clientName, new Dictionary<int, HashSet<string>>());

                        if (!SRMDWHStatic.dctSetupIdVsGlobalStatus.ContainsKey(clientName))
                            SRMDWHStatic.dctSetupIdVsGlobalStatus.TryAdd(clientName, new Dictionary<int, DWHStatus>());


                        if (!SRMDWHStatic.dctSetupIdVsParallelTriggered[clientName].ContainsKey(setupId))
                            SRMDWHStatic.dctSetupIdVsParallelTriggered[clientName].Add(setupId, false);

                        if (!SRMDWHStatic.dctSetupIdVsModuleStatus[clientName].ContainsKey(setupId))
                            SRMDWHStatic.dctSetupIdVsModuleStatus[clientName].Add(setupId, new HashSet<string>());

                        if (!SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName].ContainsKey(setupId))
                            SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName].Add(setupId, new DWHStatus());

                        ////
                        if (!SRMDWHStatic.cachedReportIdVsInfo.ContainsKey(clientName))
                            SRMDWHStatic.cachedReportIdVsInfo.TryAdd(clientName, new ConcurrentDictionary<string, Dictionary<string, Dictionary<int, SRMJobInfo>>>());

                        if (!SRMDWHStatic.cachedModuleVsReportsProcessed.ContainsKey(clientName))
                            SRMDWHStatic.cachedModuleVsReportsProcessed.TryAdd(clientName, new ConcurrentDictionary<string, Dictionary<string, List<int>>>());

                        if (!SRMDWHStatic.cachedModuleVsLegReports.ContainsKey(clientName))
                            SRMDWHStatic.cachedModuleVsLegReports.TryAdd(clientName, new ConcurrentDictionary<string, Dictionary<string, List<int>>>());

                        if (!SRMDWHStatic.cachedLoadingTimeForStagingTable.ContainsKey(clientName))
                            SRMDWHStatic.cachedLoadingTimeForStagingTable.TryAdd(clientName, new ConcurrentDictionary<string, DateTime>());

                        if (!SRMDWHStatic.cachedDownstreamConnectionName.ContainsKey(clientName))
                            SRMDWHStatic.cachedDownstreamConnectionName.TryAdd(clientName, new ConcurrentDictionary<string, string>());

                        if (!SRMDWHStatic.cachedDictBlockTypeVsData.ContainsKey(clientName))
                            SRMDWHStatic.cachedDictBlockTypeVsData.TryAdd(clientName, new ConcurrentDictionary<string, Dictionary<int, List<DataRow>>>());

                        if (!SRMDWHStatic.SetupVsLastRolledTime.ContainsKey(clientName))
                            SRMDWHStatic.SetupVsLastRolledTime.TryAdd(clientName, new Dictionary<int, RollingSetupInfo>());

                        if (!SRMDWHStatic.RunningRollData.ContainsKey(clientName))
                            SRMDWHStatic.RunningRollData.TryAdd(clientName, new HashSet<int>());

                        if (!SRMDWHStatic.RunningSetups.ContainsKey(clientName))
                            SRMDWHStatic.RunningSetups.TryAdd(clientName, new HashSet<int>());
                        ///

                    }

                    cacheKey = Guid.NewGuid().ToString();
                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, REFM_CONSTANT + ":nts", ref errorMessage, inTransaction, ref waitForWorkerResponse);

                    if (!waitForWorkerResponse)
                    {
                        SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(REFM_CONSTANT + ":nts");

                        if (string.IsNullOrEmpty(errorMessage))
                            ExecuteDWHTask(clientName, setupId, userName, setupStatusId, REFM_CONSTANT + ":nts", false, blockStatus, errorMessage, cacheKey);
                        else
                            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                    }
                }

                else if (blockExecuted.Equals(REFM_CONSTANT + ":nts") && string.IsNullOrEmpty(errorMessage))
                {
                    SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(REFM_CONSTANT + ":nts");

                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, SECM_CONSTANT + ":nts", ref errorMessage, inTransaction, ref waitForWorkerResponse);

                    if (!waitForWorkerResponse)
                    {
                        SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(SECM_CONSTANT + ":nts");

                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            lock (SRMDWHStatic.lockObjectForSecAndRefParallelRun)
                                SRMDWHStatic.dctSetupIdVsParallelTriggered[clientName][setupId] = true;
                            ExecuteDWHSubTaskInParallel(clientName, setupId, userName, setupStatusId, cacheKey);
                        }
                        else
                            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                    }
                }
                else
                {
                    lock (SRMDWHStatic.lockObjectForSecAndRefParallelRun)
                    {
                        if (!SRMDWHStatic.dctSetupIdVsParallelTriggered[clientName][setupId])
                        {
                            SRMDWHStatic.dctSetupIdVsParallelTriggered[clientName][setupId] = true;
                            ExecuteDWHSubTaskInParallel(clientName, setupId, userName, setupStatusId, cacheKey);
                        }


                        if (blockExecuted.Equals(REFM_CONSTANT + ":nts"))
                            SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(REFM_CONSTANT + ":nts");
                        else if (blockExecuted.Equals(SECM_CONSTANT + ":nts") && inTransaction)
                            SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(SECM_CONSTANT + ":nts");
                        else if (blockExecuted.Equals(SECM_CONSTANT + ":nts") && !inTransaction)
                            SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(REFM_CONSTANT + ":ts:true");
                        else if (blockExecuted.Equals(REFM_CONSTANT + ":ts") && inTransaction)
                            SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(REFM_CONSTANT + ":ts:false");
                        else if (blockExecuted.Equals(REFM_CONSTANT + ":ts") && !inTransaction)
                            SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(SECM_CONSTANT + ":ts:true");
                        else if (blockExecuted.Equals(SECM_CONSTANT + ":ts"))
                            SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(SECM_CONSTANT + ":ts:false");
                        else if (blockExecuted.Equals(REFM_CONSTANT + ":daily"))
                            SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(REFM_CONSTANT + ":daily");
                        else if (blockExecuted.Equals(SECM_CONSTANT + ":daily"))
                            SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(SECM_CONSTANT + ":daily");

                        SetFailureStatusInDictionary(setupId, userName, setupStatusId, cacheKey, errorMessage, clientName);

                        if (SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Count == 8)//CheckStatus
                        {
                            if (!string.IsNullOrEmpty(SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName][setupId].errorMessage))
                                ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, inTransaction, ref waitForWorkerResponse);
                            else
                                ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "passed", ref errorMessage, inTransaction, ref waitForWorkerResponse);

                            if (SRMDWHStatic.dctSetupIdVsParallelTriggered[clientName].ContainsKey(setupId))
                                SRMDWHStatic.dctSetupIdVsParallelTriggered[clientName].Remove(setupId);

                            if (SRMDWHStatic.dctSetupIdVsModuleStatus[clientName].ContainsKey(setupId))
                                SRMDWHStatic.dctSetupIdVsModuleStatus[clientName].Remove(setupId);

                            if (SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName].ContainsKey(setupId))
                                SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName].Remove(setupId);
                        }
                    }
                }
            }
            mLogger.Debug(string.Format("End -> ExecuteDWHTask for clientName : {0} and setupId : {1} and setupStatusId : {2} and blockExecuted : {3} and transaction : {4} and with error Message : {5}", clientName, setupId, setupStatusId, blockExecuted, inTransaction, errorMessage));

            return errorMessage;
        }

        public void ExecuteDWHSubTaskInParallel(string clientName, int setupId, string userName, int setupStatusId, string cacheKey)
        {
            string errorMessage = string.Empty; bool waitForWorkerResponse = false;

            //Ref TS and Daily Combination
            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, REFM_CONSTANT + ":ts", ref errorMessage, true, ref waitForWorkerResponse);
            if (!waitForWorkerResponse)
            {
                SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(REFM_CONSTANT + ":ts:true");
                if (!string.IsNullOrEmpty(errorMessage))
                    SetFailureStatusInDictionary(setupId, userName, setupStatusId, cacheKey, errorMessage, clientName);
            }


            //Sec TS and Daily Combination
            errorMessage = string.Empty; waitForWorkerResponse = false;
            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, SECM_CONSTANT + ":ts", ref errorMessage, true, ref waitForWorkerResponse);
            if (!waitForWorkerResponse)
            {
                SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(SECM_CONSTANT + ":ts:true");
                if (!string.IsNullOrEmpty(errorMessage))
                    SetFailureStatusInDictionary(setupId, userName, setupStatusId, cacheKey, errorMessage, clientName);
            }

            //Ref TS Only
            errorMessage = string.Empty; waitForWorkerResponse = false;
            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, REFM_CONSTANT + ":ts", ref errorMessage, false, ref waitForWorkerResponse);
            if (!waitForWorkerResponse)
            {
                SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(REFM_CONSTANT + ":ts:false");
                if (!string.IsNullOrEmpty(errorMessage))
                    SetFailureStatusInDictionary(setupId, userName, setupStatusId, cacheKey, errorMessage, clientName);
            }

            //Sec TS Only
            errorMessage = string.Empty; waitForWorkerResponse = false;
            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, SECM_CONSTANT + ":ts", ref errorMessage, false, ref waitForWorkerResponse);
            if (!waitForWorkerResponse)
            {
                SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(SECM_CONSTANT + ":ts:false");
                if (!string.IsNullOrEmpty(errorMessage))
                    SetFailureStatusInDictionary(setupId, userName, setupStatusId, cacheKey, errorMessage,clientName);
            }

            //Ref Daily Only            
            errorMessage = string.Empty; waitForWorkerResponse = false;
            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, REFM_CONSTANT + ":daily", ref errorMessage, false, ref waitForWorkerResponse);
            if (!waitForWorkerResponse)
            {
                SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(REFM_CONSTANT + ":daily");
                if (!string.IsNullOrEmpty(errorMessage))
                    SetFailureStatusInDictionary(setupId, userName, setupStatusId, cacheKey, errorMessage, clientName);
            }

            //Sec Daily Only            
            errorMessage = string.Empty; waitForWorkerResponse = false;
            ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, SECM_CONSTANT + ":daily", ref errorMessage, false, ref waitForWorkerResponse);
            if (!waitForWorkerResponse)
            {
                SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Add(SECM_CONSTANT + ":daily");
                if (!string.IsNullOrEmpty(errorMessage))
                    SetFailureStatusInDictionary(setupId, userName, setupStatusId, cacheKey, errorMessage, clientName);
            }

            if (SRMDWHStatic.dctSetupIdVsModuleStatus[clientName][setupId].Count == 8)//CheckStatus
            {
                if (!string.IsNullOrEmpty(SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName][setupId].errorMessage))
                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "failed", ref errorMessage, false, ref waitForWorkerResponse);
                else
                    ExecuteDWHSubTask(clientName, setupId, userName, setupStatusId, cacheKey, "passed", ref errorMessage, false, ref waitForWorkerResponse);

                if (SRMDWHStatic.dctSetupIdVsParallelTriggered[clientName].ContainsKey(setupId))
                    SRMDWHStatic.dctSetupIdVsParallelTriggered[clientName].Remove(setupId);

                if (SRMDWHStatic.dctSetupIdVsModuleStatus[clientName].ContainsKey(setupId))
                    SRMDWHStatic.dctSetupIdVsModuleStatus[clientName].Remove(setupId);

                if (SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName].ContainsKey(setupId))
                    SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName].Remove(setupId);
            }
        }

        private static void SetFailureStatusInDictionary(int setupId, string userName, int setupStatusId, string cacheKey, string errorMessage, string clientName)
        {
            if (!string.IsNullOrEmpty(SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName][setupId].errorMessage))
                errorMessage = SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName][setupId].errorMessage + Environment.NewLine + errorMessage;

            SRMDWHStatic.dctSetupIdVsGlobalStatus[clientName][setupId] = new DWHStatus()
            {
                setupStatusId = setupStatusId,
                errorMessage = errorMessage,
                cacheKey = cacheKey,
                userName = userName
            };
        }

        public string ExecuteDWHSubTask(string clientName, int setupId, string userName, int setupStatusId, string cacheKey, string blockToExecute, ref string errorMessage, bool inTransaction, ref bool waitForWorkerResponse)
        {
            mLogger.Debug(string.Format("Start -> ExecuteDWHSubTask for clientName : {0} and setupId : {1} and setupStatusId : {2} and blockToExecute : {3} and transaction : {4} and with error Message : {5}", clientName, setupId, setupStatusId, blockToExecute, inTransaction, errorMessage));

            string downstreamConnectionName = string.Empty;
            Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo = new Dictionary<string, Dictionary<int, SRMJobInfo>>();
            Dictionary<string, List<int>> moduleVsReportsProcessed = new Dictionary<string, List<int>>();
            Dictionary<string, List<int>> moduleVsLegReports = new Dictionary<string, List<int>>();
            Dictionary<int, List<DataRow>> dictBlockTypeVsData = new Dictionary<int, List<DataRow>>();
            DateTime loadingTimeForStagingTable = DateTime.Now;

            if (!string.IsNullOrEmpty(cacheKey))
            {
                if (SRMDWHStatic.cachedDownstreamConnectionName.ContainsKey(clientName) && SRMDWHStatic.cachedDownstreamConnectionName[clientName].ContainsKey(cacheKey))
                    downstreamConnectionName = SRMDWHStatic.cachedDownstreamConnectionName[clientName][cacheKey];

                if (SRMDWHStatic.cachedReportIdVsInfo.ContainsKey(clientName) && SRMDWHStatic.cachedReportIdVsInfo[clientName].ContainsKey(cacheKey))
                    reportIdVsInfo = SRMDWHStatic.cachedReportIdVsInfo[clientName][cacheKey];

                if (SRMDWHStatic.cachedModuleVsReportsProcessed.ContainsKey(clientName) && SRMDWHStatic.cachedModuleVsReportsProcessed[clientName].ContainsKey(cacheKey))
                    moduleVsReportsProcessed = SRMDWHStatic.cachedModuleVsReportsProcessed[clientName][cacheKey];

                if (SRMDWHStatic.cachedModuleVsLegReports.ContainsKey(clientName) && SRMDWHStatic.cachedModuleVsLegReports[clientName].ContainsKey(cacheKey))
                    moduleVsLegReports = SRMDWHStatic.cachedModuleVsLegReports[clientName][cacheKey];

                if (SRMDWHStatic.cachedLoadingTimeForStagingTable.ContainsKey(clientName) && SRMDWHStatic.cachedLoadingTimeForStagingTable[clientName].ContainsKey(cacheKey))
                    loadingTimeForStagingTable = SRMDWHStatic.cachedLoadingTimeForStagingTable[clientName][cacheKey];

                if (SRMDWHStatic.cachedDictBlockTypeVsData.ContainsKey(clientName) && SRMDWHStatic.cachedDictBlockTypeVsData[clientName].ContainsKey(cacheKey))
                    dictBlockTypeVsData = SRMDWHStatic.cachedDictBlockTypeVsData[clientName][cacheKey];
            }

            try
            {
                if (blockToExecute.Equals(REFM_CONSTANT + ":nts"))
                {
                    mLogger.Debug("Starting for setup Id : " + setupId);
                    bool canRun = CheckIfProcessCanExecute(true, setupId, ref downstreamConnectionName, false);
                    if (!canRun)
                    {
                        errorMessage = "Cannot run as next run has already been passed";
                        mLogger.Debug("Cannot run as next run has already been passed. End for setup : " + setupId);
                        InsertUpdateStatusForSetup(setupId, EXECUTION_STATUS.NOT_PROCESSED, userName, errorMessage, setupStatusId);
                        return errorMessage;
                    }

                    while (canRun)
                    {
                        bool sleep = false;
                        lock (SRMDWHStatic.lockObject)
                        {
                            if (SRMDWHStatic.RunningRollData[clientName].Contains(setupId))
                                sleep = true;
                        }
                        if (sleep)
                            Thread.Sleep(10000);
                        else
                            canRun = false;
                    }

                    ClearStagingTables(downstreamConnectionName);

                    RollDataPerSetup(clientName, setupId);

                    loadingTimeForStagingTable = DateTime.Now;

                    setupStatusId = InsertUpdateStatusForSetup(setupId, EXECUTION_STATUS.INPROGRESS, userName, string.Empty, setupStatusId);
                    mLogger.Debug("Starting for setup with status Id: " + setupStatusId);
                    if (setupStatusId != -1)
                    {
                        string sqlQuery = @"SELECT	mas.setup_id ,mas.setup_name, mas.connection_name,
		                        det.block_id, det.block_type_id , det.report_id,det.is_ref , mas.calendar_id , mas.effective_from_date , det.start_date , det.end_date , 
		                        det.custom_start_date , det.custom_end_date,det.table_name , det.batch_size , det.require_knowledge_date_reporting , 
		                        det.require_time_in_ts_report , det.require_deleted_asset_types, det.require_lookup_massaging_start_date , 
		                        det.require_lookup_massaging_current_knowledge_date, det.cc_assembly_name , det.cc_class_name , det.cc_method_name ,
		                        det.queue_name , det.failure_email_id		 
                        FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master mas
                        INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                        ON (mas.setup_id = det.setup_id AND mas.is_active = 1 AND det.is_active = 1)
                        WHERE mas.setup_id = " + setupId + "";

                        DataSet dwhJobDetails = CommonDALWrapper.ExecuteSelectQuery(sqlQuery, ConnectionConstants.RefMaster_Connection);

                        if (dwhJobDetails != null && dwhJobDetails.Tables.Count > 0 && dwhJobDetails.Tables[0].Rows.Count > 0)
                        {

                            downstreamConnectionName = Convert.ToString(dwhJobDetails.Tables[0].Rows[0]["connection_name"]);
                            SRMDWHStatic.cachedDownstreamConnectionName[clientName].TryAdd(cacheKey, downstreamConnectionName);
                            dictBlockTypeVsData = dwhJobDetails.Tables[0].AsEnumerable().GroupBy(x => Convert.ToInt32(x["block_type_id"])).ToDictionary(x => x.Key, y => y.ToList());
                            SRMDWHStatic.cachedDictBlockTypeVsData[clientName].TryAdd(cacheKey, dictBlockTypeVsData);

                            loadingTimeForStagingTable = DateTime.Now;
                            SRMDWHStatic.cachedLoadingTimeForStagingTable[clientName].TryAdd(cacheKey, loadingTimeForStagingTable);

                            GetLegReports(ref moduleVsLegReports);
                            SRMDWHStatic.cachedModuleVsLegReports[clientName].TryAdd(cacheKey, moduleVsLegReports);

                            if (dictBlockTypeVsData.Keys.Contains((int)BLOCK_TYPES.NTS_REPORT))
                            {
                                ExecuteReportsInJob(dictBlockTypeVsData[(int)BLOCK_TYPES.NTS_REPORT].Where(x => Convert.ToBoolean(x["is_ref"])).ToList(), BLOCK_TYPES.NTS_REPORT, setupStatusId, loadingTimeForStagingTable, downstreamConnectionName, userName, moduleVsReportsProcessed, reportIdVsInfo, moduleVsLegReports, cacheKey, ref waitForWorkerResponse);
                                UpdateObjectInCache(clientName, cacheKey, reportIdVsInfo, moduleVsReportsProcessed);
                            }
                        }
                    }
                }
                else if (blockToExecute.Equals(SECM_CONSTANT + ":nts"))
                {
                    if (dictBlockTypeVsData.Keys.Contains((int)BLOCK_TYPES.NTS_REPORT))
                    {
                        ExecuteReportsInJob(dictBlockTypeVsData[(int)BLOCK_TYPES.NTS_REPORT].Where(x => !Convert.ToBoolean(x["is_ref"])).ToList(),
                            BLOCK_TYPES.NTS_REPORT, setupStatusId, loadingTimeForStagingTable, downstreamConnectionName, userName, moduleVsReportsProcessed, reportIdVsInfo, moduleVsLegReports, cacheKey, ref waitForWorkerResponse);

                        UpdateObjectInCache(clientName, cacheKey, reportIdVsInfo, moduleVsReportsProcessed);
                    }

                }
                else if (blockToExecute.Equals(REFM_CONSTANT + ":ts") && inTransaction)
                {
                    if (dictBlockTypeVsData.Keys.Contains((int)BLOCK_TYPES.DAILY_REPORT) && dictBlockTypeVsData.Keys.Contains((int)BLOCK_TYPES.TS_REPORT))
                    {
                        ExecuteReportsInJobInTransaction(dictBlockTypeVsData[(int)BLOCK_TYPES.DAILY_REPORT].Where(x => Convert.ToBoolean(x["is_ref"])).ToList(), dictBlockTypeVsData[(int)BLOCK_TYPES.TS_REPORT].Where(x => Convert.ToBoolean(x["is_ref"])).ToList(), downstreamConnectionName, setupStatusId, loadingTimeForStagingTable, userName, ref moduleVsReportsProcessed, ref reportIdVsInfo, moduleVsLegReports, cacheKey, ref waitForWorkerResponse);

                        UpdateObjectInCache(clientName, cacheKey, reportIdVsInfo, moduleVsReportsProcessed);
                    }
                }

                else if (blockToExecute.Equals(REFM_CONSTANT + ":ts") && !inTransaction)
                {
                    if (dictBlockTypeVsData.Keys.Contains((int)BLOCK_TYPES.TS_REPORT))
                    {
                        ExecuteReportsInJob(dictBlockTypeVsData[(int)BLOCK_TYPES.TS_REPORT].Where(x => Convert.ToBoolean(x["is_ref"])).ToList(),
                        BLOCK_TYPES.TS_REPORT, setupStatusId, loadingTimeForStagingTable, downstreamConnectionName, userName, moduleVsReportsProcessed, reportIdVsInfo, moduleVsLegReports, cacheKey, ref waitForWorkerResponse);

                        UpdateObjectInCache(clientName, cacheKey, reportIdVsInfo, moduleVsReportsProcessed);
                    }
                }

                else if (blockToExecute.Equals(SECM_CONSTANT + ":ts") && inTransaction)
                {
                    if (dictBlockTypeVsData.Keys.Contains((int)BLOCK_TYPES.DAILY_REPORT) && dictBlockTypeVsData.Keys.Contains((int)BLOCK_TYPES.TS_REPORT))
                    {
                        ExecuteReportsInJobInTransaction(dictBlockTypeVsData[(int)BLOCK_TYPES.DAILY_REPORT].Where(x => !Convert.ToBoolean(x["is_ref"])).ToList(), dictBlockTypeVsData[(int)BLOCK_TYPES.TS_REPORT].Where(x => !Convert.ToBoolean(x["is_ref"])).ToList(), downstreamConnectionName, setupStatusId, loadingTimeForStagingTable, userName, ref moduleVsReportsProcessed, ref reportIdVsInfo, moduleVsLegReports, cacheKey, ref waitForWorkerResponse);

                        UpdateObjectInCache(clientName, cacheKey, reportIdVsInfo, moduleVsReportsProcessed);
                    }
                }

                else if (blockToExecute.Equals(SECM_CONSTANT + ":ts") && !inTransaction)
                {
                    if (dictBlockTypeVsData.Keys.Contains((int)BLOCK_TYPES.TS_REPORT))
                    {
                        ExecuteReportsInJob(dictBlockTypeVsData[(int)BLOCK_TYPES.TS_REPORT].Where(x => !Convert.ToBoolean(x["is_ref"])).ToList(),
                        BLOCK_TYPES.TS_REPORT, setupStatusId, loadingTimeForStagingTable, downstreamConnectionName, userName, moduleVsReportsProcessed, reportIdVsInfo, moduleVsLegReports, cacheKey, ref waitForWorkerResponse);

                        UpdateObjectInCache(clientName, cacheKey, reportIdVsInfo, moduleVsReportsProcessed);
                    }
                }

                else if (blockToExecute.Equals(REFM_CONSTANT + ":daily"))
                {
                    if (dictBlockTypeVsData.Keys.Contains((int)BLOCK_TYPES.DAILY_REPORT))
                    {
                        ExecuteReportsInJob(dictBlockTypeVsData[(int)BLOCK_TYPES.DAILY_REPORT].Where(x => Convert.ToBoolean(x["is_ref"])).ToList(), BLOCK_TYPES.DAILY_REPORT, setupStatusId,
                    loadingTimeForStagingTable, downstreamConnectionName, userName, moduleVsReportsProcessed, reportIdVsInfo, moduleVsLegReports, cacheKey, ref waitForWorkerResponse);

                        UpdateObjectInCache(clientName, cacheKey, reportIdVsInfo, moduleVsReportsProcessed);
                    }
                }

                else if (blockToExecute.Equals(SECM_CONSTANT + ":daily"))
                {
                    if (dictBlockTypeVsData.Keys.Contains((int)BLOCK_TYPES.DAILY_REPORT))
                    {
                        ExecuteReportsInJob(dictBlockTypeVsData[(int)BLOCK_TYPES.DAILY_REPORT].Where(x => !Convert.ToBoolean(x["is_ref"])).ToList(), BLOCK_TYPES.DAILY_REPORT, setupStatusId,
                    loadingTimeForStagingTable, downstreamConnectionName, userName, moduleVsReportsProcessed, reportIdVsInfo, moduleVsLegReports, cacheKey, ref waitForWorkerResponse);

                        UpdateObjectInCache(clientName, cacheKey, reportIdVsInfo, moduleVsReportsProcessed);
                    }
                }


                else if (blockToExecute.Equals("passed"))
                {
                    try
                    {
                        DropOldStagingTables(downstreamConnectionName);
                        if (!SkipCreateDWHViews)
                            ReCreateViews(setupId, downstreamConnectionName, loadingTimeForStagingTable, string.Empty);
                        InsertUpdateStatusForSetup(setupId, EXECUTION_STATUS.PASSED, userName, string.Empty, setupStatusId);
                        //RollDataPerSetup(setupId, downstreamConnectionName);
                        lock (SRMDWHStatic.lockObject)
                        {
                            mLogger.Error("Removing RunningSetups for setup Id : " + setupId + " and clientName : " + clientName);
                            SRMDWHStatic.RunningSetups[clientName].Remove(setupId);
                        }
                        CheckIfProcessCanExecute(false, setupId, ref downstreamConnectionName, false);
                        ClearCacheEntries(clientName, cacheKey);
                    }
                    catch (Exception ex)
                    {
                        FailSyncProcess(clientName, setupId, userName, setupStatusId, ex.Message, downstreamConnectionName, cacheKey);
                    }
                }
                else if (blockToExecute.Equals("failed"))
                {
                    FailSyncProcess(clientName, setupId, userName, setupStatusId, errorMessage, downstreamConnectionName, cacheKey);
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error in " + blockToExecute + " with transaction : " + inTransaction + " : " + ex.Message;
                mLogger.Error("ExecuteDWHSubTask in " + blockToExecute + " with transaction : " + inTransaction + " : ");
                mLogger.Error(ex);
            }
            mLogger.Debug(string.Format("End -> ExecuteDWHSubTask for clientName : {0} and setupId : {1} and setupStatusId : {2} and blockToExecute : {3} and transaction : {4} and with error Message : {5}", clientName, setupId, setupStatusId, blockToExecute, inTransaction, errorMessage));

            return errorMessage;
        }

        private static void ClearCacheEntries(string clientName, string cacheKey)
        {
            if (!string.IsNullOrEmpty(cacheKey))
            {
                string dummyString = string.Empty;
                DateTime dummyDate = DateTime.Now;
                Dictionary<string, Dictionary<int, SRMJobInfo>> dummyDict = new Dictionary<string, Dictionary<int, SRMJobInfo>>();
                Dictionary<string, List<int>> dummyDict2 = new Dictionary<string, List<int>>();
                Dictionary<int, List<DataRow>> dummyDict3 = new Dictionary<int, List<DataRow>>();

                if (SRMDWHStatic.cachedDownstreamConnectionName.ContainsKey(clientName) && SRMDWHStatic.cachedDownstreamConnectionName[clientName].ContainsKey(cacheKey))
                    SRMDWHStatic.cachedDownstreamConnectionName[clientName].TryRemove(cacheKey, out dummyString);

                if (SRMDWHStatic.cachedReportIdVsInfo.ContainsKey(clientName) && SRMDWHStatic.cachedReportIdVsInfo[clientName].ContainsKey(cacheKey))
                    SRMDWHStatic.cachedReportIdVsInfo[clientName].TryRemove(cacheKey, out dummyDict);

                if (SRMDWHStatic.cachedModuleVsReportsProcessed.ContainsKey(clientName) && SRMDWHStatic.cachedModuleVsReportsProcessed[clientName].ContainsKey(cacheKey))
                    SRMDWHStatic.cachedModuleVsReportsProcessed[clientName].TryRemove(cacheKey, out dummyDict2);

                if (SRMDWHStatic.cachedModuleVsLegReports.ContainsKey(clientName) && SRMDWHStatic.cachedModuleVsLegReports[clientName].ContainsKey(cacheKey))
                    SRMDWHStatic.cachedModuleVsLegReports[clientName].TryRemove(cacheKey, out dummyDict2);

                if (SRMDWHStatic.cachedLoadingTimeForStagingTable.ContainsKey(clientName) && SRMDWHStatic.cachedLoadingTimeForStagingTable[clientName].ContainsKey(cacheKey))
                    SRMDWHStatic.cachedLoadingTimeForStagingTable[clientName].TryRemove(cacheKey, out dummyDate);

                if (SRMDWHStatic.cachedDictBlockTypeVsData.ContainsKey(clientName) && SRMDWHStatic.cachedDictBlockTypeVsData[clientName].ContainsKey(cacheKey))
                    SRMDWHStatic.cachedDictBlockTypeVsData[clientName].TryRemove(cacheKey, out dummyDict3);
            }
        }

        private string FailSyncProcess(string clientName, int setupId, string userName, int setupStatusId, string errorMessage, string downstreamConnectionName, string cacheKey)
        {
            InsertUpdateStatusForSetup(setupId, EXECUTION_STATUS.FAILED, userName, errorMessage, setupStatusId);
            //RollDataPerSetup(setupId, downstreamConnectionName);
            lock (SRMDWHStatic.lockObject)
            {
                mLogger.Error("Removing RunningSetups for setup Id : " + setupId + " and clientName : " + clientName);
                SRMDWHStatic.RunningSetups[clientName].Remove(setupId);
            }
            CheckIfProcessCanExecute(false, setupId, ref downstreamConnectionName, false);
            ClearCacheEntries(clientName, cacheKey);

            return downstreamConnectionName;
        }

        private static void UpdateObjectInCache(string clientName, string cacheKey, Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo, Dictionary<string, List<int>> moduleVsReportsProcessed)
        {
            if (!SRMDWHStatic.cachedReportIdVsInfo[clientName].ContainsKey(cacheKey))
                SRMDWHStatic.cachedReportIdVsInfo[clientName].TryAdd(cacheKey, new Dictionary<string, Dictionary<int, SRMJobInfo>>());
            SRMDWHStatic.cachedReportIdVsInfo[clientName][cacheKey] = reportIdVsInfo;

            if (!SRMDWHStatic.cachedModuleVsReportsProcessed[clientName].ContainsKey(cacheKey))
                SRMDWHStatic.cachedModuleVsReportsProcessed[clientName].TryAdd(cacheKey, new Dictionary<string, List<int>>());
            SRMDWHStatic.cachedModuleVsReportsProcessed[clientName][cacheKey] = moduleVsReportsProcessed;
        }

        internal static void SendSubscriptionMail(SRMJobInfo jobInfo, string errorMsg)
        {
            if (!string.IsNullOrEmpty(jobInfo.FailureEmailId))
            {
                mLogger.Debug("SubscriptionMail: SendMail -> Start");
                string emailTo = string.Empty;
                RNotificationInfo notificationInfo = null;
                RMailContent mailContent = null;
                try
                {
                    notificationInfo = new RNotificationInfo();
                    mailContent = new RMailContent();

                    notificationInfo.NoOfRetry = Convert.ToInt32(SRMCommon.GetConfigFromDB("NoOfRetry"));
                    notificationInfo.NotificationFrequency = Convert.ToInt32(SRMCommon.GetConfigFromDB("MailNotificationFrequency"));
                    notificationInfo.TransportName = SRMCommon.GetConfigFromDB("MailTransportName");


                    mailContent.To = string.Join(";", jobInfo.FailureEmailId.Split(',').ToArray());
                    mailContent.From = RConfigReader.GetConfigAppSettings("FromEmailIDForApp");
                    mailContent.Subject = "Failure Downstream Sync Job ";
                    StringBuilder mailBody = new StringBuilder();
                    mailBody.Append("<table><tr><td><b>Setup Name</b></td><td>" + jobInfo.SetupName + "</td></tr>");
                    mailBody.Append("<tr><td><b>Block Type</b></td><td>" + jobInfo.BlockType + "</td></tr>");
                    mailBody.Append("<tr><td><b>Table Name</b></td><td>" + jobInfo.TableName + "</td></tr>");
                    mailBody.Append("<tr><td><b>Error</b></td><td>" + errorMsg + "</td></tr>");
                    mailBody.Append("</table>");
                    mailContent.Body = mailBody.ToString();
                    mailContent.IsBodyHTML = true;

                    notificationInfo.NotificationMessage = mailContent;
                    if (mailContent.To.Length > 0)
                    {
                        System.Threading.Thread taskThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SendMailInThread));
                        taskThread.Start(notificationInfo);
                    }

                }
                catch (Exception ex)
                {
                    mLogger.Error("Error in subscription mail...");
                    mLogger.Error(ex.ToString());
                }
                finally { mLogger.Debug("SubscriptionMail: SendMail -> End"); }
            }
        }

        internal static void SendMailInThread(object objNotificationInfo)
        {
            RNotificationManager manager = new RNotificationManager();
            RNotificationInfo notificationInfo = (RNotificationInfo)objNotificationInfo;
            manager.GenerateNotification(notificationInfo);
        }

        public string DownloadViewDefinition(string setupName, out string errorMessage)
        {
            string folderLocation = string.Empty;
            errorMessage = string.Empty;
            List<string> errorMessages = new List<string>();
            string sqlQuery = "SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master WHERE setup_name = '" + setupName + "' AND is_active = 1";
            DataSet dwhSetup = CommonDALWrapper.ExecuteSelectQuery(sqlQuery, ConnectionConstants.RefMaster_Connection);
            if (dwhSetup != null && dwhSetup.Tables.Count > 0 && dwhSetup.Tables[0].Rows.Count > 0)
            {
                string downstreamConnectionName = Convert.ToString(dwhSetup.Tables[0].Rows[0]["connection_name"]);
                int setupId = Convert.ToInt32(dwhSetup.Tables[0].Rows[0]["setup_id"]);

                ///System.Web.Hosting.HostingEnvironment.MapPath("~/") + 
                folderLocation = AppDomain.CurrentDomain.BaseDirectory + "DWHSQLViewFiles\\" + setupName + "\\" + DateTime.Now.ToString("yyyy-MM-dd--hh--mm--ss") + "\\";
                if (!Directory.Exists(folderLocation))
                    Directory.CreateDirectory(folderLocation);
                errorMessages = DownloadViews(setupId, downstreamConnectionName, folderLocation);
            }

            if (errorMessages.Count > 0)
                errorMessage = string.Join(",", errorMessages);
            return folderLocation;
        }

        private List<string> DownloadViews(int setupId, string downstreamConnectionName, string folderLocation)
        {
            List<string> errorMessages = new List<string>();
            string dependentInstruments = string.Empty;
            DateTime minDate = new DateTime(1990, 1, 1);

            string query = string.Format(@"
                        CREATE TABLE #temp2(Security VARCHAR(MAX), sectype_id INT, RequireIntraDayChanges BIT, RequireNTSView BIT)
                        CREATE TABLE #temp(Entity VARCHAR(MAX), entity_type_id INT, RequireIntraDayChanges BIT, RequireNTSView BIT)

                        DECLARE @setupId INT = {0}

                        INSERT INTO #temp2(Security,sectype_id,RequireIntraDayChanges, RequireNTSView)
                        SELECT DISTINCT 'Security',0,0,0
                        FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                            WHERE det.is_active = 1 AND det.is_ref = 0 AND setup_id = @setupId

                        UPDATE #temp2
                        SET #temp2.RequireNTSView = 1
                        FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                        WHERE det.is_active = 1 AND det.is_ref = 0 AND setup_id = @setupId AND det.block_type_id = 1 AND table_name NOT LIKE '%[_]leg[_]%'

                        UPDATE #temp2
                        SET #temp2.RequireIntraDayChanges = 1
                        FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                        WHERE det.is_active = 1 AND det.is_ref = 0 AND setup_id = @setupId AND det.block_type_id = 2 AND det.require_time_in_ts_report = 1

                        INSERT INTO #temp(Entity,entity_type_id,RequireIntraDayChanges, RequireNTSView)
                        SELECT DISTINCT etyp.entity_display_name,etyp.entity_type_id,0,0
                        FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                        ON (det.report_id = rep.report_id AND rep.is_active = 1 AND det.is_active = 1 AND det.is_ref = 1)
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                        ON (rep.report_id = emap.report_id AND emap.is_active = 1)
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                        ON (etyp.entity_type_id = emap.dependent_id) 
                        WHERE setup_id = @setupId

                        UPDATE tt
                        SET tt.RequireNTSView = 1
                        FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                        ON (det.report_id = rep.report_id AND rep.is_active = 1 AND det.is_active = 1 AND det.is_ref = 1 AND det.block_type_id = 1)
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                        ON (rep.report_id = emap.report_id AND emap.is_active = 1)
                        INNER JOIN #temp tt ON (emap.dependent_id = tt.entity_type_id)
                        WHERE setup_id = @setupId AND rep.report_name NOT LIKE '% leg %'

                        UPDATE tt
                        SET tt.RequireIntraDayChanges = 1
                        FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                        ON (det.report_id = rep.report_id AND rep.is_active = 1 AND det.is_active = 1 AND det.is_ref = 1 AND det.block_type_id = 2 AND det.require_time_in_ts_report = 1)
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                        ON (rep.report_id = emap.report_id AND emap.is_active = 1)
                        INNER JOIN #temp tt ON (emap.dependent_id = tt.entity_type_id)
                        WHERE setup_id = @setupId AND rep.report_name NOT LIKE '% leg %'

                        SELECT * FROM #temp2
                        SELECT * FROM #temp

                        DROP TABLE #temp
                        DROP TABLE #temp2 ", setupId);

            ObjectSet ds = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                bool requireIntraDayChanges = Convert.ToBoolean(ds.Tables[0].Rows[0]["RequireIntraDayChanges"]);
                bool requireNTSView = Convert.ToBoolean(ds.Tables[0].Rows[0]["RequireNTSView"]);
                try
                {
                    CheckForModificationsAndReCreateView("Security", minDate.ToString("yyyyMMdd HH:mm:ss.fff"), downstreamConnectionName, setupId,
                        requireIntraDayChanges, requireNTSView, dependentInstruments, folderLocation);
                }
                catch (Exception ex)
                {
                    errorMessages.Add(ex.Message);
                }
            }
            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (ObjectRow or in ds.Tables[1].Rows)
                {
                    bool requireIntraDayChanges = Convert.ToBoolean(or["RequireIntraDayChanges"]);
                    bool requireNTSView = Convert.ToBoolean(or["RequireNTSView"]);
                    try
                    {
                        CheckForModificationsAndReCreateView(Convert.ToString(or[0]), minDate.ToString("yyyyMMdd HH:mm:ss.fff"),
                            downstreamConnectionName, setupId, requireIntraDayChanges, requireNTSView, dependentInstruments, folderLocation);
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add(ex.Message);
                    }
                }
            }
            return errorMessages;
        }

        private void ReCreateViews(int setupId, string downstreamConnectionName, DateTime loadingTimeForStagingTable, string folderLocation)
        {
            List<string> errorMessages = new List<string>();
            bool isSecMasterExists = SRMDownstreamConfiguration.CheckDatabaseExists("IVPSecMaster");
            string query = string.Format("SELECT TOP 1 last_run_time FROM dbo.ivp_srm_dwh_last_successful_run_time WHERE setup_id = {0}", setupId);
            ObjectSet ds = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                loadingTimeForStagingTable = Convert.ToDateTime(ds.Tables[0].Rows[0][0]);

            query = string.Format(@"
                                CREATE TABLE #temp2(Security VARCHAR(MAX), sectype_id INT, RequireIntraDayChanges BIT, RequireNTSView BIT, DependentInstruments VARCHAR(MAX))
                                CREATE TABLE #temp(Entity VARCHAR(MAX), entity_type_id INT, RequireIntraDayChanges BIT, RequireNTSView BIT, DependentInstruments VARCHAR(MAX))

                                INSERT INTO #temp2(Security,sectype_id,RequireIntraDayChanges, RequireNTSView)
                                SELECT DISTINCT 'Security',0,0,0
                                FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                    WHERE det.is_active = 1 AND det.is_ref = 0 AND setup_id = {0}

                                UPDATE #temp2
                                SET #temp2.RequireNTSView = 1
                                FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                WHERE det.is_active = 1 AND det.is_ref = 0 AND setup_id = {0} AND det.block_type_id = 1  AND table_name NOT LIKE '%[_]leg[_]%'

                                UPDATE #temp2
                                SET #temp2.RequireIntraDayChanges = 1
                                FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                WHERE det.is_active = 1 AND det.is_ref = 0 AND setup_id = {0} AND det.block_type_id = 2 AND det.require_time_in_ts_report = 1 
                                " +
                                (isSecMasterExists ? @"
                                    SELECT *
                                    INTO #temp4 FROM (
                                    SELECT DISTINCT 'Security' AS Security,etyp.entity_display_name AS Dependents
                                        FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id 
                                        INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det ON 
	                                    (det.report_id = rs.report_setup_id AND det.is_ref = 0 AND det.is_active = 1  AND det.setup_id = 2)
	                                    INNER JOIN IVPSecMaster..ivp_secm_report_configuration rc on rs.report_setup_id = rc.report_setup_id
                                        CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(rc.attributes_to_show,'|') tab
                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attribute_mapping ram 
	                                        ON ra.report_attribute_id = ram.report_attribute_id AND ISNULL(ram.attribute_id,0) > 0
	                                        AND 
	                                        CASE WHEN tab.item LIKE 'A:%' OR tab.item LIKE 'S:%' 
		                                        THEN SUBSTRING(SUBSTRING(tab.item,3,LEN(tab.item)-2),1,CHARINDEX(':',SUBSTRING(tab.item,3,LEN(tab.item)-2))-1) 
		                                        WHEN tab.item LIKE 'R::%' 
		                                        THEN REPLACE(tab.item,'R::','') 
	                                        END = ra.report_attribute_id
                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_reference_attribute_mapping ramm
                                        ON (ram.attribute_id = ramm.attribute_id AND ram.reference_attribute_id = '-1')
	                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
	                                    ON (etyp.entity_type_id = ramm.reference_type_id)
                                    UNION ALL
                                    SELECT DISTINCT 'Security' AS Security,etyp.entity_display_name AS Dependents 
                                        FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id 
	                                    INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det ON 
	                                    (det.report_id = rs.report_setup_id AND det.is_ref = 0 AND det.is_active = 1  AND det.setup_id = 2)
                                        INNER JOIN IVPSecMaster..ivp_secm_report_configuration rc on rs.report_setup_id = rc.report_setup_id
                                        CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(rc.attributes_to_show,'|') tab
                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_additional_leg_attribute_mapping ram 
	                                        ON ra.report_attribute_id = ram.report_attribute_id 
	                                        AND 
	                                        CASE WHEN tab.item LIKE 'A:%' OR tab.item LIKE 'S:%' 
		                                        THEN SUBSTRING(SUBSTRING(tab.item,3,LEN(tab.item)-2),1,CHARINDEX(':',SUBSTRING(tab.item,3,LEN(tab.item)-2))-1) 
		                                        WHEN tab.item LIKE 'R::%' 
		                                        THEN REPLACE(tab.item,'R::','') 
	                                        END = ra.report_attribute_id
                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs_reference_attribute_mapping ramm
                                        ON (ram.attribute_id = ramm.attribute_id AND ram.reference_attribute_id = '-1')
	                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
	                                    ON (etyp.entity_type_id = ramm.reference_type_id)) tt4

	                                SELECT Security, Dependents = 
                                            STUFF((SELECT ', ' + Dependents
                                                    FROM #temp4 b 
                                                    WHERE b.Security = a.Security 
                                                    FOR XML PATH('')), 1, 2, '')
	                                    INTO #temp5
                                        FROM #temp4 a
                                        GROUP BY Security

	                                UPDATE tt
                                    SET tt.DependentInstruments = tt4.Dependents
                                    FROM #temp2 tt
                                    INNER JOIN #temp5 tt4
                                    ON (tt.Security = tt4.Security) " : ""
                                ) + @"
                                INSERT INTO #temp(Entity,entity_type_id,RequireIntraDayChanges, RequireNTSView)
                                SELECT DISTINCT etyp.entity_display_name,etyp.entity_type_id,0,0
                                FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                                ON (det.report_id = rep.report_id AND rep.is_active = 1 AND det.is_active = 1 AND det.is_ref = 1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                                ON (rep.report_id = emap.report_id AND emap.is_active = 1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                                ON (etyp.entity_type_id = emap.dependent_id) 
                                WHERE setup_id = {0}

                                UPDATE tt
                                SET tt.RequireNTSView = 1
                                FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                                ON (det.report_id = rep.report_id AND rep.is_active = 1 AND det.is_active = 1 AND det.is_ref = 1 AND det.block_type_id = 1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                                ON (rep.report_id = emap.report_id AND emap.is_active = 1)
                                INNER JOIN #temp tt ON (emap.dependent_id = tt.entity_type_id)
                                WHERE setup_id = {0} AND rep.report_name NOT LIKE '% leg %'

                                UPDATE tt
                                SET tt.RequireIntraDayChanges = 1
                                FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                                ON (det.report_id = rep.report_id AND rep.is_active = 1 AND det.is_active = 1 AND det.is_ref = 1 AND det.block_type_id = 2 AND det.require_time_in_ts_report = 1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                                ON (rep.report_id = emap.report_id AND emap.is_active = 1)
                                INNER JOIN #temp tt ON (emap.dependent_id = tt.entity_type_id)
                                WHERE setup_id = {0} AND rep.report_name NOT LIKE '% leg %'

                                SELECT DISTINCT etyp.entity_display_name, etyp2.entity_display_name AS Dependents
                                INTO #temp3
                                FROM 
                                IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                                ON (det.report_id = rep.report_id AND rep.is_active = 1 AND det.is_active = 1 AND det.is_ref = 1 
                                    AND det.setup_id = {0})
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                                ON (rep.report_id = emap.report_id AND emap.is_active = 1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                                ON (etyp.entity_type_id = emap.dependent_id AND etyp.is_active = 1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_configuration conf
                                ON (rep.report_id = conf.report_id AND conf.is_active = 1 AND conf.attribute_to_show NOT LIKE '%-%')
                                CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowID(attribute_to_show,'|') tab
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute_map map
                                ON(map.report_attribute_id = tab.item AND map.is_active = 1 AND map.lookup_attribute_id IS NULL)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute_lookup eal
                                ON (map.dependent_attribute_id = eal.child_entity_attribute_id AND eal.is_active = 1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp2
                                ON (eal.parent_entity_type_id = etyp2.entity_type_id AND etyp2.is_active = 1)

                                INSERT INTO #temp3
                                SELECT DISTINCT tt.entity_display_name,'Security'
                                FROM #temp3 tt
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                                ON (etyp.entity_display_name = tt.entity_display_name)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                                ON (emap.dependent_id = etyp.entity_type_id)
                                INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                ON (det.report_id = emap.report_id)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute ra
                                ON (ra.report_id = emap.report_id)
                                WHERE det.setup_id = {0} AND ra.attribute_data_type = 'SECURITY_LOOKUP'

                                SELECT entity_display_name, Dependents = 
                                    STUFF((SELECT ', ' + Dependents
                                            FROM #temp3 b 
                                            WHERE b.entity_display_name = a.entity_display_name 
                                            FOR XML PATH('')), 1, 2, '')
	                            INTO #temp6
                                FROM #temp3 a
                                GROUP BY entity_display_name

                                UPDATE tt
                                SET tt.DependentInstruments = tt4.Dependents
                                FROM #temp tt
                                INNER JOIN #temp6 tt4
                                ON (tt.entity = tt4.entity_display_name)

                                SELECT * FROM #temp2
                                SELECT * FROM #temp

                                DROP TABLE #temp
                                DROP TABLE #temp2
                                DROP TABLE #temp3
                                " +
                                (isSecMasterExists ? @"
                                    DROP TABLE #temp4
                                    DROP TABLE #temp5" : ""
                                ) + @"
                                DROP TABLE #temp6", setupId);
            ds = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                bool requireIntraDayChanges = Convert.ToBoolean(ds.Tables[0].Rows[0]["RequireIntraDayChanges"]);
                bool requireNTSView = Convert.ToBoolean(ds.Tables[0].Rows[0]["RequireNTSView"]);
                string dependentInstruments = Convert.ToString(ds.Tables[0].Rows[0]["DependentInstruments"]).Trim();
                try
                {
                    CheckForModificationsAndReCreateView("Security", loadingTimeForStagingTable.ToString("yyyyMMdd HH:mm:ss.fff"),
                        downstreamConnectionName, setupId, requireIntraDayChanges, requireNTSView, dependentInstruments, string.Empty);
                }
                catch (Exception ex)
                {
                    errorMessages.Add(ex.Message);
                }
            }
            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (ObjectRow or in ds.Tables[1].Rows)
                {
                    bool requireIntraDayChanges = Convert.ToBoolean(or["RequireIntraDayChanges"]);
                    bool requireNTSView = Convert.ToBoolean(or["RequireNTSView"]);
                    string dependentInstruments = Convert.ToString(or["DependentInstruments"]).Trim();
                    try
                    {
                        CheckForModificationsAndReCreateView(Convert.ToString(or[0]), loadingTimeForStagingTable.ToString("yyyyMMdd HH:mm:ss.fff"),
                            downstreamConnectionName, setupId, requireIntraDayChanges, requireNTSView, dependentInstruments, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add(ex.Message);
                    }
                }
            }
            if (errorMessages.Count > 0)
                throw new Exception(String.Join(",", errorMessages));
        }

        private void CheckForModificationsAndReCreateView(string instrumentName, string lastSuccessTime, string downstreamConnectionName, int setupId, bool requireIntraDayChanges,
            bool requireNTSView, string dependentInstruments, string folderLocation)
        {
            try
            {
                string instrumentNameLocal = instrumentName.ToLower().Replace(" ", "_");
                string query = string.Empty;
                ObjectSet ds1 = null;

                if (string.IsNullOrEmpty(folderLocation))
                {
                    query = @"IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ivp_srm_dwh_view_tables' AND TABLE_SCHEMA = 'dimension')
                                BEGIN
                                    DROP TABLE dimension.ivp_srm_dwh_view_tables
                                END
                                CREATE TABLE dimension.ivp_srm_dwh_view_tables(table_name VARCHAR(1000))";
                    SRMDWHJobExtension.ExecuteQueryObject(downstreamConnectionName, query, SRMDBQueryType.SELECT);
                    ObjectTable ot = new ObjectTable();
                    ot.Columns.Add(new ObjectColumn("table_name", typeof(System.String)));
                    ObjectRow or = ot.NewRow();
                    or[0] = "ivp_polaris_" + instrumentNameLocal + "_non_time_series";
                    ot.Rows.Add(or);

                    or = ot.NewRow();
                    or[0] = "ivp_polaris_" + instrumentNameLocal + "_time_series";
                    ot.Rows.Add(or);

                    or = ot.NewRow();
                    or[0] = "ivp_polaris_" + instrumentNameLocal + "_daily";
                    ot.Rows.Add(or);

                    if (!string.IsNullOrEmpty(dependentInstruments))
                    {
                        List<string> dependents = dependentInstruments.Split(',').ToList();
                        foreach (var dd in dependents)
                        {
                            or = ot.NewRow();
                            or[0] = "ivp_polaris_" + dd.ToLower().Replace(" ", "_") + "_non_time_series";
                            ot.Rows.Add(or);

                            or = ot.NewRow();
                            or[0] = "ivp_polaris_" + dd.ToLower().Replace(" ", "_") + "_time_series";
                            ot.Rows.Add(or);

                            or = ot.NewRow();
                            or[0] = "ivp_polaris_" + dd.ToLower().Replace(" ", "_") + "_daily";
                            ot.Rows.Add(or);
                        }
                    }

                    SRMDWHJobExtension.ExecuteBulkCopyObject(downstreamConnectionName, "dimension.ivp_srm_dwh_view_tables", ot);

                    query = string.Format(@"
                                        SELECT TOP 1 1 FROM sys.tables tab
                                        INNER JOIN sys.schemas sch
                                        ON (tab.schema_id = sch.schema_id)
                                        INNER JOIN dimension.ivp_srm_dwh_view_tables tab2
                                        ON (tab.name = tab2.table_name)
                                        AND sch.name = 'dimension'
                                        AND modify_date > '{0}'
                                        DROP TABLE dimension.ivp_srm_dwh_view_tables", lastSuccessTime);

                    ds1 = SRMDWHJobExtension.ExecuteQueryObject(downstreamConnectionName, query, SRMDBQueryType.SELECT);
                }

                if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    GenerateViews(instrumentName, downstreamConnectionName, setupId, requireIntraDayChanges, requireNTSView, instrumentNameLocal, true, false, folderLocation);
                }
                if (!string.IsNullOrEmpty(folderLocation))
                    GenerateViews(instrumentName, downstreamConnectionName, setupId, requireIntraDayChanges, requireNTSView, instrumentNameLocal, false, true, folderLocation);
            }
            catch (Exception ex)
            {
                mLogger.Error("View creation failed for " + instrumentName + " Exception : " + ex);
                throw new Exception("View creation failed for " + instrumentName + " : " + ex.Message + "; ");
            }
        }

        private static void GenerateViews(string instrumentName, string downstreamConnectionName, int setupId, bool requireIntraDayChanges, bool requireNTSView,
            string instrumentNameLocal, bool executeViewDefinition, bool downloadViewDefinition, string folderLocation)
        {
            string viewQuery = string.Empty;
            string query;
            bool requireIntraDayChangesLocal = requireIntraDayChanges;
            bool setLookupColumns = true, allowECInColumns = true;
            string surrogateColumnLength = "NUMERIC(19,0)";
            if (!string.IsNullOrEmpty(SetLookupColumnNamesInDWHViews))
            {
                if (SetLookupColumnNamesInDWHViews.Equals("0") || SetLookupColumnNamesInDWHViews.Equals("false"))
                    setLookupColumns = false;
            }
            if (!string.IsNullOrEmpty(AllowECinColumnsInDWHViews))
            {
                if (AllowECinColumnsInDWHViews.Equals("0") || AllowECinColumnsInDWHViews.Equals("false"))
                    allowECInColumns = false;
            }

            if (!requireIntraDayChanges)
            {
                // If TS table is created new, it will have surrogate values for lookups instead of master_ids;
                requireIntraDayChangesLocal = CanAlterTable(downstreamConnectionName, "ivp_polaris_" + instrumentNameLocal + "_time_series");
            }

            if (instrumentNameLocal.Equals("security"))
                query = string.Format("EXEC dbo.SRM_Polaris_Create_Security_View {0},'{1}',{2},{3},{4}", setupId, surrogateColumnLength, setLookupColumns, allowECInColumns, requireIntraDayChangesLocal);
            else
                query = string.Format("EXEC dbo.SRM_Polaris_Create_Entity_View {0},'{1}','{2}',{3},{4},{5}", setupId, instrumentName, surrogateColumnLength, setLookupColumns, allowECInColumns, requireIntraDayChangesLocal);

            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                viewQuery = Convert.ToString(ds.Tables[0].Rows[0][0]);
                viewQuery = viewQuery.Substring(3, viewQuery.Length - 5);

                if (executeViewDefinition)
                    ExecuteViewInTransaction(downstreamConnectionName, viewQuery, setupId);

                if (downloadViewDefinition)
                    File.AppendAllText(folderLocation + instrumentName + "_daily", viewQuery);
            }

            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                viewQuery = Convert.ToString(ds.Tables[1].Rows[0][0]);
                viewQuery = viewQuery.Substring(3, viewQuery.Length - 5);

                if (executeViewDefinition)
                    ExecuteViewInTransaction(downstreamConnectionName, viewQuery, setupId);

                if (downloadViewDefinition)
                    File.AppendAllText(folderLocation + instrumentName + "_daily_wc", viewQuery);
            }

            if(requireIntraDayChanges && CreateMappingTableAndIntraDayView)
            {
                //Drop Intraday Views If Exists and Create New if IntraDay bit = true
                if (instrumentNameLocal.Equals("security"))
                    query = string.Format("EXEC dbo.SRM_Polaris_Create_Security_IntraDay_View {0},'{1}',{2},{3},{4}", setupId, surrogateColumnLength, setLookupColumns, allowECInColumns, requireIntraDayChanges);
                else
                    query = string.Format("EXEC dbo.SRM_Polaris_Create_Entity_IntraDay_View {0},'{1}','{2}',{3},{4},{5}", setupId, instrumentName, surrogateColumnLength, setLookupColumns, allowECInColumns, requireIntraDayChanges);

                ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    viewQuery = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    viewQuery = viewQuery.Substring(3, viewQuery.Length - 5);

                    if (executeViewDefinition)
                        ExecuteViewInTransaction(downstreamConnectionName, viewQuery, setupId);

                    if (downloadViewDefinition)
                        File.AppendAllText(folderLocation + instrumentName + "_intraday_daily", viewQuery);
                }

                if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    viewQuery = Convert.ToString(ds.Tables[1].Rows[0][0]);
                    viewQuery = viewQuery.Substring(3, viewQuery.Length - 5);

                    if (executeViewDefinition && requireIntraDayChanges)
                        ExecuteViewInTransaction(downstreamConnectionName, viewQuery, setupId);

                    if (downloadViewDefinition && requireIntraDayChanges)
                        File.AppendAllText(folderLocation + instrumentName + "_intraday_daily_wc", viewQuery);
                }
            }

            if (requireNTSView)
            {
                if (instrumentNameLocal.Equals("security"))
                    query = string.Format("EXEC dbo.SRM_Polaris_Create_Security_NTS_View {0},'{1}',{2},{3},{4}", setupId, surrogateColumnLength, setLookupColumns, allowECInColumns, requireIntraDayChanges);
                else
                    query = string.Format("EXEC dbo.SRM_Polaris_Create_Entity_NTS_View {0},'{1}','{2}',{3},{4},{5}", setupId, instrumentName, surrogateColumnLength, setLookupColumns, allowECInColumns, requireIntraDayChanges);

                ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    viewQuery = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    viewQuery = viewQuery.Substring(3, viewQuery.Length - 5);

                    if (executeViewDefinition)
                        ExecuteViewInTransaction(downstreamConnectionName, viewQuery, setupId);

                    if (downloadViewDefinition)
                        File.AppendAllText(folderLocation + instrumentName + "_nts", viewQuery);
                }
            }
        }

        private static void ExecuteViewInTransaction(string downstreamConnectionName, string viewQuery, int setupId)
        {
            RDBConnectionManager con = RDALAbstractFactory.DBFactory.GetConnectionManager(downstreamConnectionName);
            con.IsolationLevel = IsolationLevel.RepeatableRead;
            con.UseTransaction = true;
            try
            {
                var spliittedList = viewQuery.Split(new string[] { "GO\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                SRMDWHJobExtension.ExecuteQueryObject(con, spliittedList[0], SRMDBQueryType.SELECT);
                SRMDWHJobExtension.ExecuteQueryObject(con, spliittedList[1], SRMDBQueryType.SELECT);

                if (SRMDWHStatic.dctSetupIdVsDWHAdapterDetails.ContainsKey(setupId))
                {
                    foreach (var adapterInfo in SRMDWHStatic.dctSetupIdVsDWHAdapterDetails[setupId])
                    {
                        DWHError? failedResponse = DWHAdapterController.SyncViewsSchema(adapterInfo.AdapterId, con, viewQuery);

                        if (failedResponse.HasValue)
                        {
                            mLogger.Error(adapterInfo.AdapterName + " - Exception : " + failedResponse.Value.Message);
                            throw new Exception(adapterInfo.AdapterName + " - Exception : " + failedResponse.Value.Message);
                        }
                    }
                }
                con.CommitTransaction();
            }
            catch (Exception ex)
            {
                if (con != null)
                    con.RollbackTransaction();
                throw;
            }
            finally
            {
                if (con != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(con);
            }
        }

        private void RollDataPerSetup(string clientName, int setupId)
        {
            RollingInfo rollingInfo;
            bool isValid = false;
            lock (SRMDWHStatic.lockObject)
            {
                if (SRMDWHJobQueue.dctrollingQueue[clientName].TryGetValue(setupId, out rollingInfo))
                    isValid = true;
            }
            if (isValid)
                RollData(clientName, rollingInfo);
        }

        private void ExecuteReportsInJobInTransaction(List<DataRow> dailyReports, List<DataRow> tsReports, string downstreamConnectionName, int setupStatusId, DateTime loadingTimeForStagingTable, string userName, ref Dictionary<string, List<int>> moduleVsReportsProcessed, ref Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfoGlobal, Dictionary<string, List<int>> moduleVsLegReports, string cacheKey, ref bool waitForWorkerResponse)
        {
            List<int> refReportIdsForParallel = new List<int>();
            List<int> secReportIdsForParallel = new List<int>();
            List<int> refReportIdsForSequential = new List<int>();
            List<int> secReportIdsForSequential = new List<int>();
            Dictionary<string, Dictionary<int, List<int>>> tsReportVsDailyReports = new Dictionary<string, Dictionary<int, List<int>>>();
            Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo = new Dictionary<string, Dictionary<int, SRMJobInfo>>();

            foreach (var row in tsReports)
            {
                SRMJobInfo job;
                int reportId = Convert.ToInt32(row["report_id"]);
                string module = Convert.ToBoolean(row["is_ref"]) == true ? REFM_CONSTANT : SECM_CONSTANT;

                if (!reportIdVsInfoGlobal.ContainsKey(module))
                    reportIdVsInfoGlobal.Add(module, new Dictionary<int, SRMJobInfo>());

                if (!reportIdVsInfo.ContainsKey(module))
                    reportIdVsInfo.Add(module, new Dictionary<int, SRMJobInfo>());

                if (!reportIdVsInfoGlobal[module].ContainsKey(reportId))
                {
                    PopulateJobDetails(downstreamConnectionName, BLOCK_TYPES.TS_REPORT, row, setupStatusId, loadingTimeForStagingTable, out job);
                    //job.EffectiveStartDateForReport = effectiveFromDate;
                    job.UserName = userName;

                    if (moduleVsLegReports.ContainsKey(module) && moduleVsLegReports[module].Contains(job.ReportId) && !job.BlockType.Equals(BLOCK_TYPES.TS_REPORT))
                        job.IsLegReport = true;

                    reportIdVsInfoGlobal[module].Add(reportId, job);

                    reportIdVsInfo[module].Add(reportId, job);
                }
            }

            foreach (var row in dailyReports)
            {
                SRMJobInfo job;
                int reportId = Convert.ToInt32(row["report_id"]);
                string module = Convert.ToBoolean(row["is_ref"]) == true ? REFM_CONSTANT : SECM_CONSTANT;

                if (reportIdVsInfoGlobal.ContainsKey(module) && reportIdVsInfoGlobal[module].ContainsKey(reportId))
                {
                    job = reportIdVsInfoGlobal[module][reportId];
                }
                else
                {
                    PopulateJobDetails(downstreamConnectionName, BLOCK_TYPES.DAILY_REPORT, row, setupStatusId, loadingTimeForStagingTable, out job);
                    job.UserName = userName;

                    if (moduleVsLegReports.ContainsKey(module) && moduleVsLegReports[module].Contains(job.ReportId) && !job.BlockType.Equals(BLOCK_TYPES.TS_REPORT))
                        job.IsLegReport = true;

                    if (!reportIdVsInfoGlobal.ContainsKey(module))
                        reportIdVsInfoGlobal.Add(module, new Dictionary<int, SRMJobInfo>());
                    reportIdVsInfoGlobal[module].Add(reportId, job);

                    if (!reportIdVsInfo.ContainsKey(module))
                        reportIdVsInfo.Add(module, new Dictionary<int, SRMJobInfo>());
                    reportIdVsInfo[module].Add(reportId, job);

                }
                if (!string.IsNullOrEmpty(job.TimeSeriesReportName) && reportIdVsInfoGlobal[module].ContainsKey(job.TimeSeriesReportId))
                {
                    if (!tsReportVsDailyReports.ContainsKey(module))
                        tsReportVsDailyReports.Add(module, new Dictionary<int, List<int>>());

                    if (!tsReportVsDailyReports[module].ContainsKey(job.TimeSeriesReportId))
                        tsReportVsDailyReports[module].Add(job.TimeSeriesReportId, new List<int>());

                    tsReportVsDailyReports[module][job.TimeSeriesReportId].Add(reportId);
                }
            }

            foreach (var tsDailyReportInfo in tsReportVsDailyReports)
            {
                string module = tsDailyReportInfo.Key;
                foreach (var tsDailyReports in tsDailyReportInfo.Value)
                {
                    int tsReportId = tsDailyReports.Key;

                    if (!moduleVsReportsProcessed.ContainsKey(module))
                        moduleVsReportsProcessed.Add(module, new List<int>());

                    moduleVsReportsProcessed[module].Add(tsReportId);
                    moduleVsReportsProcessed[module].AddRange(tsDailyReports.Value);

                    bool reportInLastExtraction = reportIdVsInfoGlobal[module][tsReportId].DateType.Equals(DWHDateType.LastExtractionDate);
                    if (reportInLastExtraction)
                    {
                        foreach (var dailyReportId in tsDailyReports.Value)
                        {
                            reportInLastExtraction = reportInLastExtraction && reportIdVsInfoGlobal[module][dailyReportId].DateType.Equals(DWHDateType.LastExtractionDate);
                        }
                    }


                    if (module.Equals(REFM_CONSTANT))
                    {
                        if (reportInLastExtraction || (!string.IsNullOrEmpty(RunRefDWHTasksInParallel) && RunRefDWHTasksInParallel.Equals("true", StringComparison.OrdinalIgnoreCase)))
                        {
                            refReportIdsForParallel.Add(tsReportId);
                            refReportIdsForParallel.AddRange(tsDailyReports.Value);
                            //Check for both TS and Daily report as they can have different start dates
                        }
                        else
                        {
                            refReportIdsForSequential.Add(tsReportId);
                            refReportIdsForSequential.AddRange(tsDailyReports.Value);
                        }
                    }
                    else
                    {
                        {
                            secReportIdsForSequential.Add(tsReportId);
                            secReportIdsForSequential.AddRange(tsDailyReports.Value);
                        }
                    }
                }
            }

            if (refReportIdsForParallel.Count > 0)
            {
                ObjectSet modifiedDataInReports = FetchModifiedEntities(refReportIdsForParallel, reportIdVsInfo[REFM_CONSTANT]);
                refReportIdsForParallel.Clear();
                if (modifiedDataInReports != null && modifiedDataInReports.Tables.Count > 0 && modifiedDataInReports.Tables[0].Rows.Count > 0)
                {
                    var reportIdVsRecords = modifiedDataInReports.Tables[0].AsEnumerable().GroupBy(x => Convert.ToInt32(x["report_id"])).ToDictionary(x => x.Key, y => y.GroupBy(z => Convert.ToBoolean(z["isTSUpdate"])).ToDictionary(z => z.Key, zz => zz.Select(zzz => Convert.ToString(zzz["entity_code"])).ToHashSet()));

                    foreach (int reportId in reportIdVsRecords.Keys)
                    {
                        refReportIdsForParallel.Add(reportId);

                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntities = new HashSet<string>();
                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = new HashSet<string>();
                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = new HashSet<string>();

                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntities = new HashSet<string>();
                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = new HashSet<string>();
                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = new HashSet<string>();


                        if (reportIdVsRecords[reportId].ContainsKey(true))
                        {
                            reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = reportIdVsRecords[reportId][true];
                            reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = reportIdVsRecords[reportId][true];
                        }
                        if (reportIdVsRecords[reportId].ContainsKey(false))
                        {
                            reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = reportIdVsRecords[reportId][false];
                            reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = reportIdVsRecords[reportId][false];
                        }
                    }
                }
            }
            if (refReportIdsForSequential.Count > 0)
            {
                ObjectSet modifiedDataInReports = FetchModifiedEntities(refReportIdsForSequential, reportIdVsInfo[REFM_CONSTANT]);
                refReportIdsForSequential.Clear();
                if (modifiedDataInReports != null && modifiedDataInReports.Tables.Count > 0 && modifiedDataInReports.Tables[0].Rows.Count > 0)
                {
                    var reportIdVsRecords = modifiedDataInReports.Tables[0].AsEnumerable().GroupBy(x => Convert.ToInt32(x["report_id"])).ToDictionary(x => x.Key, y => y.GroupBy(z => Convert.ToBoolean(z["isTSUpdate"])).ToDictionary(z => z.Key, zz => zz.Select(zzz => Convert.ToString(zzz["entity_code"])).ToHashSet()));

                    foreach (int reportId in reportIdVsRecords.Keys)
                    {
                        refReportIdsForSequential.Add(reportId);

                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntities = new HashSet<string>();
                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = new HashSet<string>();
                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = new HashSet<string>();

                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntities = new HashSet<string>();
                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = new HashSet<string>();
                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = new HashSet<string>();

                        if (reportIdVsRecords[reportId].ContainsKey(true))
                        {
                            reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = reportIdVsRecords[reportId][true];
                            reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = reportIdVsRecords[reportId][true];
                        }
                        if (reportIdVsRecords[reportId].ContainsKey(false))
                        {
                            reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = reportIdVsRecords[reportId][false];
                            reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = reportIdVsRecords[reportId][false];
                        }
                    }
                }
            }

            if (refReportIdsForParallel.Count > 0 || refReportIdsForSequential.Count > 0 || secReportIdsForSequential.Count > 0 || secReportIdsForParallel.Count > 0)
            {
                int id = InsertInfoForSetupExecution(setupStatusId, reportIdVsInfo, refReportIdsForParallel, refReportIdsForSequential, secReportIdsForSequential, secReportIdsForParallel);

                if (refReportIdsForParallel.Count > 0 || refReportIdsForSequential.Count > 0)
                    CallWorkerProcess(REFM_CONSTANT, id, BLOCK_TYPES.TS_REPORT, true, reportIdVsInfo[REFM_CONSTANT].FirstOrDefault().Value.SetupName.Replace(" ", "_"), cacheKey, ref waitForWorkerResponse);

                if (secReportIdsForSequential.Count > 0 || secReportIdsForParallel.Count > 0)
                    CallWorkerProcess(SECM_CONSTANT, id, BLOCK_TYPES.TS_REPORT, true, reportIdVsInfo[SECM_CONSTANT].FirstOrDefault().Value.SetupName.Replace(" ", "_"), cacheKey, ref waitForWorkerResponse);
            }
        }


        public static void RollData(string clientName, RollingInfo rollingInfo)
        {
            mLogger.Error("RollData -> START");
            try
            {
                mLogger.Error(rollingInfo);

                var isSuccess = RollDataForDailyTables(rollingInfo);
                if (isSuccess)
                    isSuccess = UpdateLookupIdsInTSTables(rollingInfo);

                if (isSuccess)
                {
                    DateTime now = DateTime.Now;
                    DateTime nextDay = DateTime.Today.AddDays(1);
                    string nextDayString = nextDay.ToString("yyyy-MM-dd HH:mm:ss");

                    while(rollingInfo.T_plus_1 && now >= rollingInfo.TimeToRun && now.Date == rollingInfo.TimeToRun.Date)
                    {
                        mLogger.Error(string.Format("RollData -> Current Time : {0}. Waiting till {1}", now.ToString("yyyy-MM-dd HH:mm:ss"), nextDayString));
                        Thread.Sleep(10000);
                        now = DateTime.Now;
                    }

                    DateTime rollingTime;
                    lock (SRMDWHStatic.lockObject)
                    {
                        now = DateTime.Now;
                        var rollingSetupInfo = SRMDWHStatic.SetupVsLastRolledTime[clientName][rollingInfo.SetupId];

                        if (rollingInfo.T_plus_1 && DateTime.Today == rollingInfo.TimeToRun.Date)
                        {
                            rollingTime = now;
                            rollingSetupInfo.LastRollingTime = rollingTime;
                        }
                        else
                        {
                            rollingTime = now.AddDays(-1);
                            rollingSetupInfo.LastRollingTime = rollingTime;
                        }
                        SRMDWHStatic.SetupVsLastRolledTime[clientName][rollingInfo.SetupId] = rollingSetupInfo;
                    }

                    var rollingTimeString = rollingTime.ToString("yyyyMMdd HH:mm:ss.fff");
                    mLogger.Error(string.Format("Setting Last Rolling Time For SetupId : {0} As Time : {1}", rollingInfo.SetupId, rollingTimeString));

                    string sqlQuery = string.Format("INSERT INTO dbo.ivp_srm_dwh_rolling_time (setup_id,rolling_time) VALUES({0},'{1}')", rollingInfo.SetupId, rollingTimeString);
                    CommonDALWrapper.ExecuteQuery(sqlQuery, CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("Error in rolling data..");
                mLogger.Error(ee.ToString());
            }
            finally
            {
                lock (SRMDWHStatic.lockObject)
                {
                    if (SRMDWHStatic.RunningRollData.ContainsKey(clientName))
                    {
                        mLogger.Error("Removing RunningRollData for setup Id : " + rollingInfo.SetupId + " and clientName : " + clientName);
                        SRMDWHStatic.RunningRollData[clientName].Remove(rollingInfo.SetupId);
                    }
                    if (SRMDWHJobQueue.dctrollingQueue.ContainsKey(clientName))
                        SRMDWHJobQueue.dctrollingQueue[clientName].Remove(rollingInfo.SetupId);
                }
                mLogger.Error("RollData -> END");
            }
        }

        private static bool UpdateLookupIdsInTSTables(RollingInfo rollingInfo)
        {
            DateTime currentTime = DateTime.Now;
            var today = rollingInfo.TimeToRun.Date;
            var nextDay = today.AddDays(1);
            int[] processors = new int[Environment.ProcessorCount];
            string query = @"SELECT MAS.table_name, MAS.is_leg
		                FROM (
				                SELECT DISTINCT CASE WHEN PARSENAME(identifier_key,1) LIKE '%[_]staging' THEN  REPLACE(PARSENAME(identifier_key,1),'_staging','') ELSE PARSENAME(identifier_key,1) END AS table_name,
                                    CASE WHEN PARSENAME(identifier_key,2) = 'dimension' AND CHARINDEX('_leg_',PARSENAME(identifier_key,1)) > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS is_leg 
				                FROM taskmanager.ivp_polaris_core_secref_extract_table_column_mapping  
				                WHERE identifier_key NOT IN ('ref_tables_default_columns','security_tables_default_columns')
			                ) MAS
		                INNER JOIN INFORMATION_SCHEMA.TABLES t ON MAS.table_name = t.TABLE_NAME AND t.TABLE_SCHEMA = 'dimension'";

            var dt = SRMDWHJobExtension.ExecuteQueryObject(rollingInfo.ConnectionName, query, SRMDBQueryType.SELECT).Tables[0];

            ConcurrentQueue<KeyValuePair<string, bool>> queueTables = new ConcurrentQueue<KeyValuePair<string, bool>>(dt.AsEnumerable().Select(x => new KeyValuePair<string, bool>(Convert.ToString(x["table_name"]), Convert.ToBoolean(x["is_leg"]))));
            bool failed = false;
            Parallel.ForEach(processors, processorId =>
            {
                KeyValuePair<string, bool> tableVsIsLeg;

                while (queueTables.TryDequeue(out tableVsIsLeg))
                {
                    mLogger.Error(string.Format("Rolling Table : {0} -> START", tableVsIsLeg.Key));
                    RDBConnectionManager con = null;

                    string tempTableName = null;
                    try
                    {
                        con = RDALAbstractFactory.DBFactory.GetConnectionManager(rollingInfo.ConnectionName);
                        con.IsolationLevel = IsolationLevel.RepeatableRead;
                        con.UseTransaction = true;

                        var mainTableName = string.Format("[dimension].[{0}]", tableVsIsLeg.Key);
                        var querySB = new StringBuilder();
                        querySB.AppendFormat(@"DECLARE @columns VARCHAR(MAX) = '',
                                    @sql VARCHAR(MAX) = ''

                                    SELECT @columns += '[' + COLUMN_NAME + '],'
                                    FROM INFORMATION_SCHEMA.COLUMNS
                                    WHERE TABLE_NAME = '{0}' AND COLUMN_NAME NOT IN('master_id', 'id') AND DATA_TYPE = 'BIGINT' AND TABLE_SCHEMA = 'dimension'

                                    SELECT DATA_TYPE
                                    FROM INFORMATION_SCHEMA.COLUMNS
                                    WHERE TABLE_NAME = '{0}' AND COLUMN_NAME = 'id' AND TABLE_SCHEMA = 'dimension'

                                    SELECT NEWID()

                                    IF(LEN(@columns) > 0)
                                    BEGIN
                                        SELECT @columns = SUBSTRING(@columns, 1, LEN(@columns) - 1)
                                        SELECT 1 FROM {1} WITH (TABLOCKX) WHERE 1 = 2
                                      
                                        SELECT @sql = 'SELECT id, ' + @columns + ' FROM {1} WHERE [effective_end_date] IS NULL'
                                        EXEC(@sql)
                                    END", tableVsIsLeg.Key, mainTableName);

                        var dsTemp = SRMDWHJobExtension.ExecuteQueryObject(con, querySB.ToString(), SRMDBQueryType.SELECT);

                        if (dsTemp.Tables.Count > 2 && dsTemp.Tables[3].Rows.Count > 0)
                        {
                            var data = dsTemp.Tables[3];
                            if (data.Rows.Count > 0)
                            {
                                var dayToRollTo = today;
                                if (rollingInfo.T_plus_1)
                                    dayToRollTo = nextDay;

                                var dateStr = dayToRollTo.ToString("yyyyMMdd");
                                foreach (var row in data.Rows)
                                {
                                    foreach (var col in data.Columns)
                                    {
                                        if (col.ColumnName == "id")
                                            continue;

                                        var value = Convert.ToString(row[col.ColumnName]);
                                        if (!string.IsNullOrEmpty(value) && value.Length > 8)
                                        {
                                            var masterId = value.Substring(8, value.Length - 8);
                                            row[col.ColumnName] = dateStr + masterId;
                                        }
                                    }
                                }

                                var idDatatype = Convert.ToString(dsTemp.Tables[0].Rows[0][0]);
                                tempTableName = string.Format("[taskmanager].[{0}]", Convert.ToString(dsTemp.Tables[1].Rows[0][0]));

                                var setColumns = new StringBuilder();
                                var createColumns = new StringBuilder();
                                foreach (var col in data.Columns)
                                {
                                    if (col.ColumnName != "id")
                                    {
                                        setColumns.Append("mainTab.[").Append(col.ColumnName).Append("] = tab.[").Append(col.ColumnName).Append("],");
                                        createColumns.Append("[").Append(col.ColumnName).Append("] BIGINT,");
                                    }
                                }
                                setColumns.Append("mainTab.[loading_time] = '").Append(currentTime.ToString("yyyyMMdd HH:mm:ss.fff")).Append("',");

                                var q = string.Format("CREATE TABLE {0} ([id] {1}, {2})", tempTableName, idDatatype, createColumns.Remove(createColumns.Length - 1, 1).ToString());

                                SRMDWHJobExtension.ExecuteQueryObject(rollingInfo.ConnectionName, q, SRMDBQueryType.INSERT);
                                SRMDWHJobExtension.ExecuteBulkCopyObject(con, tempTableName, data);

                                q = string.Format(@"UPDATE mainTab
                                                        SET {0}
                                                        FROM {1} mainTab
                                                        INNER JOIN {2} tab ON mainTab.id = tab.id", setColumns.Remove(setColumns.Length - 1, 1).ToString(), mainTableName, tempTableName);

                                SRMDWHJobExtension.ExecuteQueryObject(con, q, SRMDBQueryType.INSERT);

                                if (SRMDWHStatic.dctSetupIdVsDWHAdapterDetails.TryGetValue(rollingInfo.SetupId, out List<DWHAdapterDetails> adapters))
                                {
                                    foreach (var info in adapters)
                                    {
                                        var tsMetaData = new TSAndDailyMetadata(new TableSyncMetadata[0], new TableSyncMetadata[] { new TableSyncMetadata(tableVsIsLeg.Value, false, currentTime, mainTableName) });
                                        var error = DWHAdapterController.SyncDailyTableData(info.AdapterId, rollingInfo.ConnectionName, tsMetaData, con);

                                        if (error.HasValue)
                                            throw new Exception(error.Value.ErrorMessage);
                                    }
                                }
                            }
                        }

                        con.CommitTransaction();
                    }
                    catch (Exception ee)
                    {
                        con.RollbackTransaction();
                        mLogger.Error(string.Format("Rolling Table : {0} -> Exception -> {1}", tableVsIsLeg.Key, ee.ToString()));

                        lock (lockObject)
                            failed = true;
                    }
                    finally
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(con);

                        if (tempTableName != null)
                        {
                            try
                            {
                                SRMDWHJobExtension.ExecuteQueryObject(rollingInfo.ConnectionName, string.Format("IF(OBJECT_ID('{0}') IS NOT NULL) DROP TABLE {0}", tempTableName), SRMDBQueryType.INSERT);
                            }
                            catch { }
                        }
                        mLogger.Error(string.Format("Rolling Table : {0} -> END", tableVsIsLeg.Key));
                    }
                }
            });

            return !failed;
        }

        private static bool RollDataForDailyTables(RollingInfo rollingInfo)
        {
            string query;
            DateTime today, nextDay;
            int[] processors;
            DateTime currentTime = DateTime.Now;

            query = @"SELECT PARSENAME(t1.table_name,1) AS staging_table, REPLACE(REPLACE(REPLACE(t1.table_name,'_staging',''),'references','dimension'),'taskmanager','dimension') AS daily_table, t1.is_leg, t1.surrogate_table_name AS master_table, '[dimension].[' + REPLACE(PARSENAME(t1.table_name,1),'_staging','') + ']' AS table_name_with_schema
                    FROM[dimension].[ivp_srm_dwh_tables_for_loading] t1
                    INNER JOIN INFORMATION_SCHEMA.TABLES t ON t.TABLE_NAME = PARSENAME(REPLACE(REPLACE(REPLACE(t1.table_name, '_staging', ''), 'references', 'dimension'), 'taskmanager', 'dimension'), 1) AND t.TABLE_SCHEMA = 'dimension'
                    WHERE t1.is_active = 1 AND t1.table_type = 'DAILY'";
            var dtTemp = SRMDWHJobExtension.ExecuteQueryObject(rollingInfo.ConnectionName, query, SRMDBQueryType.SELECT).Tables[0];

            ConcurrentQueue<KeyValuePair<string, Tuple<bool, string, string, string>>> queue = new ConcurrentQueue<KeyValuePair<string, Tuple<bool, string, string, string>>>();
            foreach (var row in dtTemp.Rows)
            {
                queue.Enqueue(new KeyValuePair<string, Tuple<bool, string, string, string>>(Convert.ToString(row["daily_table"]), new Tuple<bool, string, string, string>(Convert.ToBoolean(row["is_leg"]), Convert.ToString(row["master_table"]), Convert.ToString(row["staging_table"]), Convert.ToString(row["table_name_with_schema"]))));
            }

            query = string.Format(@"SELECT PARSENAME(table_name,1) AS table_name, require_deleted_asset_types
                    FROM dbo.ivp_srm_dwh_downstream_details bd
                    INNER JOIN dbo.ivp_srm_dwh_downstream_block_type bt ON bd.block_type_id = bt.block_type_id
                    WHERE bt.block_type_name = 'DAILY' AND setup_id = {0}", rollingInfo.SetupId);

            var stagingTableVsRequireDeletedInstruments = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["table_name"]), y => Convert.ToBoolean(y["require_deleted_asset_types"]));

            bool failed = false;
            today = rollingInfo.TimeToRun.Date;
            nextDay = today.AddDays(1);
            processors = new int[Environment.ProcessorCount];
            Parallel.ForEach(processors, processorId =>
            {
                KeyValuePair<string, Tuple<bool, string, string, string>> tableLevel;
                while (queue.TryDequeue(out tableLevel))
                {
                    mLogger.Error(string.Format("Rolling Table : {0} -> START", tableLevel.Key));

                    var requireDeletedInstruments = false;
                    stagingTableVsRequireDeletedInstruments.TryGetValue(tableLevel.Value.Item3, out requireDeletedInstruments);

                    StringBuilder querySB = new StringBuilder($@"DECLARE @today DATE = '{(rollingInfo.T_plus_1 ? nextDay.ToString("yyyyMMdd") : today.ToString("yyyyMMdd"))}'

                        IF(OBJECT_ID('tempdb..#tempdd') IS NOT NULL)
                            DROP TABLE #tempdd").Append(Environment.NewLine);

                    querySB.AppendFormat("SELECT 1 FROM {0} WITH (TABLOCKX) WHERE 1 = 2", tableLevel.Key).Append(Environment.NewLine);

                    if (requireDeletedInstruments)
                    {
                        querySB.AppendFormat(@"SELECT d2.[master_id], MAX(d2.[Effective Date]) AS [Effective Date]
                                    INTO #tempdd
							        FROM {0} d2
                                    INNER JOIN {1} m ON d2.master_id = m.id AND m.is_active = 1
							        GROUP BY d2.[master_id]", tableLevel.Key, tableLevel.Value.Item2).Append(Environment.NewLine);
                    }
                    else
                    {
                        querySB.AppendFormat(@"SELECT d2.[master_id], MAX(d2.[Effective Date]) AS [Effective Date]
                                    INTO #tempdd
							        FROM {0} d2
							        GROUP BY d2.[master_id]", tableLevel.Key).Append(Environment.NewLine);
                    }

                    querySB.AppendFormat(@"
                        SELECT d.*
				        FROM {0} d
				        INNER JOIN #tempdd MAS ON d.[master_id] = MAS.[master_id] AND d.[Effective Date] = MAS.[Effective Date]
				        WHERE d.[Effective Date] <= @today {1}

				        SELECT COLUMN_NAME, DATA_TYPE
				        FROM INFORMATION_SCHEMA.COLUMNS
				        WHERE TABLE_NAME = PARSENAME('{0}',1) AND TABLE_SCHEMA = 'dimension'", tableLevel.Key, tableLevel.Value.Item1 ? "AND d.is_leg_deleted = 0" : string.Empty);

                    RDBConnectionManager con = null;
                    try
                    {
                        con = RDALAbstractFactory.DBFactory.GetConnectionManager(rollingInfo.ConnectionName);
                        con.IsolationLevel = IsolationLevel.RepeatableRead;
                        con.UseTransaction = true;

                        var ds = SRMDWHJobExtension.ExecuteQueryObject(con, querySB.ToString(), SRMDBQueryType.SELECT);

                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            var dayToRollTo = today;
                            if (rollingInfo.T_plus_1)
                                dayToRollTo = nextDay;

                            var currentData = ds.Tables[1];
                            var columnInfo = ds.Tables[2].AsEnumerable().ToDictionary(x => Convert.ToString(x["COLUMN_NAME"]).ToLower(), y => Convert.ToString(y["DATA_TYPE"]).ToLower());
                            var futureData = currentData.Clone();

                            for (var rowCounter = 0; rowCounter < currentData.Rows.Count; rowCounter++)
                            {
                                var currentRow = currentData.Rows[rowCounter];
                                var effectiveDate = Convert.ToDateTime(currentRow["Effective Date"]);
                                var diff = dayToRollTo.Subtract(effectiveDate).Days;
                                for (var dateCounter = 1; dateCounter <= diff; dateCounter++)
                                {
                                    effectiveDate = effectiveDate.AddDays(1);
                                    var dateString = effectiveDate.ToString("yyyyMMdd");
                                    var newRow = futureData.NewRow();
                                    foreach (var column in currentData.Columns)
                                    {
                                        var columnName = column.ColumnName.ToLower();
                                        string datatype = null;
                                        columnInfo.TryGetValue(columnName, out datatype);

                                        if (columnName == "effective date")
                                            newRow[columnName] = effectiveDate;

                                        else if (datatype == "bigint")
                                        {
                                            var value = Convert.ToString(currentRow[columnName]);
                                            if (!string.IsNullOrEmpty(value) && value.Length > 8)
                                            {
                                                var masterId = value.Substring(8, value.Length - 8);
                                                newRow[columnName] = dateString + masterId;
                                            }
                                            else
                                                newRow[columnName] = currentRow[columnName];
                                        }
                                        else
                                            newRow[columnName] = currentRow[columnName];
                                    }
                                    newRow["loading_time"] = currentTime;
                                    futureData.Rows.Add(newRow);
                                }
                            }

                            SRMDWHJobExtension.ExecuteBulkCopyObject(con, tableLevel.Key, futureData);

                            if (SRMDWHStatic.dctSetupIdVsDWHAdapterDetails.TryGetValue(rollingInfo.SetupId, out List<DWHAdapterDetails> adapters))
                            {
                                foreach (var info in adapters)
                                {
                                    var tsMetaData = new TSAndDailyMetadata(new TableSyncMetadata[] { new TableSyncMetadata(tableLevel.Value.Item1, false, currentTime, tableLevel.Value.Item4) }, new TableSyncMetadata[0]);
                                    var error = DWHAdapterController.SyncDailyTableData(info.AdapterId, rollingInfo.ConnectionName, tsMetaData, con);

                                    if (error.HasValue)
                                        throw new Exception(error.Value.ErrorMessage);
                                }
                            }
                        }

                        con.CommitTransaction();
                    }
                    catch (Exception ee)
                    {
                        con.RollbackTransaction();
                        mLogger.Error(string.Format("Rolling Table : {0} -> Exception -> {1}", tableLevel.Key, ee.ToString()));

                        lock (lockObject)
                            failed = true;
                    }
                    finally
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(con);
                        mLogger.Error(string.Format("Rolling Table : {0} -> END", tableLevel.Key));
                    }
                }
            });

            return !failed;
        }

        public static void ExecuteDWHAdapters(SRMJobInfo job, RDBConnectionManager con)
        {
            string tableName = job.TableName.Replace("[taskmanager]", "[dimension]").Replace("[references]", "[dimension]").Replace("_staging]", "]");
            FailedSyncMetadata? failedResponse = null;

            if (SRMDWHStatic.dctSetupIdVsDWHAdapterDetails.ContainsKey(job.SetupId))
            {
                foreach (var adapterInfo in SRMDWHStatic.dctSetupIdVsDWHAdapterDetails[job.SetupId])
                {
                    if (job.BlockType == BLOCK_TYPES.NTS_REPORT)
                    {
                        TableSyncMetadata metadata = new TableSyncMetadata(job.IsLegReport, job.DateType == DWHDateType.None, job.LoadingTimeForStagingTable, tableName);
                        failedResponse = DWHAdapterController.SyncNTSTableData(adapterInfo.AdapterId, job.DownstreamSQLConnectionName, metadata, null);
                    }
                    else if (job.BlockType == BLOCK_TYPES.TS_REPORT)
                    {
                        TableSyncMetadata[] tSMetadata = new TableSyncMetadata[0];

                        string legText = "securities_leg_";
                        if (job.IsRef)
                            legText = tableName.Replace("[dimension].[ivp_polaris_", "").Replace("_time_series]", "_leg_");
                        string query = string.Format(@"SELECT DISTINCT identifier_key FROM taskmanager.ivp_polaris_core_secref_extract_table_column_mapping 
                                        WHERE identifier_key like '%{0}%'", legText);
                        ObjectSet os = SRMDWHJobExtension.ExecuteQueryObject(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                        if (os != null && os.Tables.Count > 0 && os.Tables[0].Rows.Count > 0)
                        {
                            ObjectTable ot = os.Tables[0];
                            tSMetadata = new TableSyncMetadata[ot.Rows.Count + 1];
                            int counter = 0;
                            foreach (ObjectRow or in ot.Rows)
                            {
                                string legTableName = Convert.ToString(or[0]);
                                tSMetadata[counter] = new TableSyncMetadata(true, job.DateType == DWHDateType.None, job.LoadingTimeForStagingTable, legTableName);
                                counter++;
                            }
                            tSMetadata[counter] = new TableSyncMetadata(false, job.DateType == DWHDateType.None, job.LoadingTimeForStagingTable, tableName);
                        }
                        else
                        {
                            tSMetadata = new TableSyncMetadata[1];
                            tSMetadata[0] = new TableSyncMetadata(false, job.DateType == DWHDateType.None, job.LoadingTimeForStagingTable, tableName);
                        }

                        TableSyncMetadata[] dailyMetadata = new TableSyncMetadata[1];
                        tableName = tableName.Replace("_time_series]", "_daily]");
                        dailyMetadata[0] = new TableSyncMetadata(false, job.DateType == DWHDateType.None, job.LoadingTimeForStagingTable, tableName);

                        TSAndDailyMetadata metadata = new TSAndDailyMetadata(dailyMetadata, tSMetadata);
                        failedResponse = DWHAdapterController.SyncDailyTableData(adapterInfo.AdapterId, job.DownstreamSQLConnectionName, metadata, con);
                    }
                    else
                    {
                        TableSyncMetadata[] tSMetadata = new TableSyncMetadata[0];

                        TableSyncMetadata[] dailyMetadata = new TableSyncMetadata[1];
                        dailyMetadata[0] = new TableSyncMetadata(false, job.DateType == DWHDateType.None, job.LoadingTimeForStagingTable, tableName);

                        TSAndDailyMetadata metadata = new TSAndDailyMetadata(dailyMetadata, tSMetadata);

                        failedResponse = DWHAdapterController.SyncDailyTableData(adapterInfo.AdapterId, job.DownstreamSQLConnectionName, metadata, con);
                    }
                    if (failedResponse.HasValue)
                    {
                        mLogger.Error(adapterInfo.AdapterName + " - Exception : " + failedResponse.Value.ErrorMessage);
                        throw new Exception(adapterInfo.AdapterName + " - Exception : " + failedResponse.Value.ErrorMessage);
                    }
                }
            }
        }

        public void CreateObjects(string downstreamConnectionName)
        {
            mLogger.Debug("CreateObjects -> Start");
            mLogger.Debug("Create/Update Objects in DB");
            string directoryLocation = AppDomain.CurrentDomain.BaseDirectory + "\\DWHScripts";
            new CommonDALScriptsExecutor().RunDBScriptsFromFolder(directoryLocation, downstreamConnectionName);
            mLogger.Debug("CreateObjects -> End");
        }

        public bool CheckIfProcessCanExecute(bool input, int setupId, ref string downstreamConnectionName, bool clearWaiting)
        {
            if (setupId == 0 || string.IsNullOrEmpty(downstreamConnectionName))
            {
                string sqlQuery = "SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master WHERE setup_id = '" + setupId + "' AND is_active = 1";

                DataSet dwhSetup = CommonDALWrapper.ExecuteSelectQuery(sqlQuery, ConnectionConstants.RefMaster_Connection);

                if (dwhSetup != null && dwhSetup.Tables.Count > 0 && dwhSetup.Tables[0].Rows.Count > 0)
                {
                    downstreamConnectionName = Convert.ToString(dwhSetup.Tables[0].Rows[0]["connection_name"]);

                    sqlQuery = "EXEC dbo.SRM_DWH_CanExecutePipe " + setupId + "," + input + "," + clearWaiting;
                    if (input)
                        return Convert.ToBoolean(CommonDALWrapper.ExecuteSelectQuery(sqlQuery, ConnectionConstants.RefMaster_Connection).Tables[0].Rows[0][0]);
                    else
                        CommonDALWrapper.ExecuteSelectQuery(sqlQuery, ConnectionConstants.RefMaster_Connection);
                }
                return false;
            }
            else
            {
                string sqlQuery = "EXEC dbo.SRM_DWH_CanExecutePipe " + setupId + "," + input + "," + clearWaiting;
                CommonDALWrapper.ExecuteSelectQuery(sqlQuery, ConnectionConstants.RefMaster_Connection);
                return false;
            }
        }


        internal int InsertUpdateStatusForSetup(int setupId, EXECUTION_STATUS status, string userName, string failure_reason, int statusId)
        {
            if (status == EXECUTION_STATUS.QUEUED || (status == EXECUTION_STATUS.INPROGRESS && statusId == -1))
            {
                string query = @"
                            INSERT INTO [dbo].[ivp_srm_dwh_downstream_setup_status] (setup_id,status,failure_reason,is_active,created_by,start_time)
                            OUTPUT INSERTED.setup_status_id
                            VALUES (" + setupId + ",'" + status + "','" + failure_reason + "',1,'" + userName + "',GETDATE())";
                return Convert.ToInt32(CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0].Rows[0][0]);
            }
            else if (status == EXECUTION_STATUS.INPROGRESS)
            {
                string query = "UPDATE [dbo].[ivp_srm_dwh_downstream_setup_status] SET status = '" + status + "',start_time = GETDATE() WHERE setup_status_id = " + statusId;
                CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            }
            else
            {
                if (failure_reason.Length > 0)
                {
                    int maxLength = failure_reason.Length - 1;
                    if (maxLength > 8000)
                    {
                        maxLength = 8000;
                        failure_reason = failure_reason.Substring(0, maxLength);
                    }
                    failure_reason = failure_reason.Replace("'", "''");
                }
                string query = "UPDATE [dbo].[ivp_srm_dwh_downstream_setup_status] SET status = '" + status + "',failure_reason = '" + failure_reason + "', end_time = GETDATE() WHERE setup_status_id = " + statusId;
                CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            }

            if (status == EXECUTION_STATUS.PASSED)
            {
                string query = string.Format(@"
                                IF EXISTS(SELECT TOP 1 1 FROM dbo.ivp_srm_dwh_last_successful_run_time WHERE setup_id = {0})
                                BEGIN
	                                UPDATE dbo.ivp_srm_dwh_last_successful_run_time SET last_run_time = GETDATE() WHERE setup_id = {0}
                                END
                                ELSE
                                BEGIN
	                                INSERT INTO dbo.ivp_srm_dwh_last_successful_run_time(setup_id, last_run_time) VALUES ({0},GETDATE())
                                END", setupId);
                CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            }
            return statusId;
        }

        private bool ExecuteReportsInJob(List<DataRow> dataRows, BLOCK_TYPES blockType, int setupStatusId,
                                        DateTime loadingTimeForStagingTable, string downstreamConnectionName, string userName, Dictionary<string, List<int>> moduleVsReportsProcessed, Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfoGlobal, Dictionary<string, List<int>> moduleVsLegReports, string cacheKey, ref bool waitForWorkerResponse)
        {
            List<int> refReportIdsForParallel = new List<int>();
            List<int> secReportIdsForParallel = new List<int>();
            List<int> refReportIdsForSequential = new List<int>();
            List<int> secReportIdsForSequential = new List<int>();
            Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo = new Dictionary<string, Dictionary<int, SRMJobInfo>>();
            bool isPassed = true;


            foreach (var row in dataRows)
            {
                SRMJobInfo job;
                int reportId = Convert.ToInt32(row["report_id"]);
                string module = Convert.ToBoolean(row["is_ref"]) == true ? REFM_CONSTANT : SECM_CONSTANT;
                if (moduleVsReportsProcessed.ContainsKey(module) && moduleVsReportsProcessed[module].Contains(reportId))
                    continue;

                if (reportIdVsInfoGlobal.ContainsKey(module) && reportIdVsInfoGlobal[module].ContainsKey(reportId))
                {
                    job = reportIdVsInfoGlobal[module][reportId];
                }
                else
                {
                    PopulateJobDetails(downstreamConnectionName, blockType, row, setupStatusId, loadingTimeForStagingTable, out job);
                    job.UserName = userName;

                    if (moduleVsLegReports.ContainsKey(module) && moduleVsLegReports[module].Contains(job.ReportId) && !job.BlockType.Equals(BLOCK_TYPES.TS_REPORT))
                        job.IsLegReport = true;
                }

                if (job.IsRef)
                {
                    if (!reportIdVsInfo.ContainsKey(REFM_CONSTANT))
                        reportIdVsInfo.Add(REFM_CONSTANT, new Dictionary<int, SRMJobInfo>() { });
                    reportIdVsInfo[REFM_CONSTANT].Add(job.ReportId, job);

                    if (!reportIdVsInfoGlobal.ContainsKey(REFM_CONSTANT))
                        reportIdVsInfoGlobal.Add(REFM_CONSTANT, new Dictionary<int, SRMJobInfo>() { });

                    if (!reportIdVsInfoGlobal[REFM_CONSTANT].ContainsKey(job.ReportId))
                        reportIdVsInfoGlobal[REFM_CONSTANT].Add(job.ReportId, job);

                    if (job.DateType.Equals(DWHDateType.LastExtractionDate) || (!string.IsNullOrEmpty(RunRefDWHTasksInParallel) && RunRefDWHTasksInParallel.Equals("true", StringComparison.OrdinalIgnoreCase)))
                        refReportIdsForParallel.Add(job.ReportId);
                    else
                        refReportIdsForSequential.Add(job.ReportId);
                }
                else
                {

                    if (!reportIdVsInfo.ContainsKey(SECM_CONSTANT))
                        reportIdVsInfo.Add(SECM_CONSTANT, new Dictionary<int, SRMJobInfo>() { });
                    reportIdVsInfo[SECM_CONSTANT].Add(job.ReportId, job);

                    if (!reportIdVsInfoGlobal.ContainsKey(SECM_CONSTANT))
                        reportIdVsInfoGlobal.Add(SECM_CONSTANT, new Dictionary<int, SRMJobInfo>() { });

                    if (!reportIdVsInfoGlobal[SECM_CONSTANT].ContainsKey(job.ReportId))
                        reportIdVsInfoGlobal[SECM_CONSTANT].Add(job.ReportId, job);

                    if (job.DateType.Equals(DWHDateType.LastExtractionDate))
                        secReportIdsForParallel.Add(job.ReportId);
                    else
                        secReportIdsForSequential.Add(job.ReportId);
                }
            }

            if (refReportIdsForParallel.Count > 0)
            {
                ObjectSet modifiedDataInReports = FetchModifiedEntities(refReportIdsForParallel, reportIdVsInfo[REFM_CONSTANT]);
                refReportIdsForParallel.Clear();
                if (modifiedDataInReports != null && modifiedDataInReports.Tables.Count > 0 && modifiedDataInReports.Tables[0].Rows.Count > 0)
                {
                    var reportIdVsRecords = modifiedDataInReports.Tables[0].AsEnumerable().GroupBy(x => Convert.ToInt32(x["report_id"])).ToDictionary(x => x.Key, y => y.GroupBy(z => Convert.ToBoolean(z["isTSUpdate"])).ToDictionary(z => z.Key, zz => zz.Select(zzz => Convert.ToString(zzz["entity_code"])).ToHashSet()));

                    foreach (int reportId in reportIdVsRecords.Keys)
                    {
                        refReportIdsForParallel.Add(reportId);

                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntities = new HashSet<string>();
                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = new HashSet<string>();
                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = new HashSet<string>();

                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntities = new HashSet<string>();
                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = new HashSet<string>();
                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = new HashSet<string>();

                        if (reportIdVsRecords[reportId].ContainsKey(true))
                        {
                            reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = reportIdVsRecords[reportId][true];
                            reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = reportIdVsRecords[reportId][true];
                        }
                        if (reportIdVsRecords[reportId].ContainsKey(false))
                        {
                            reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = reportIdVsRecords[reportId][false];
                            reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = reportIdVsRecords[reportId][false];
                        }
                    }
                }
            }
            if (refReportIdsForSequential.Count > 0)
            {
                ObjectSet modifiedDataInReports = FetchModifiedEntities(refReportIdsForSequential, reportIdVsInfo[REFM_CONSTANT]);
                refReportIdsForSequential.Clear();
                if (modifiedDataInReports != null && modifiedDataInReports.Tables.Count > 0 && modifiedDataInReports.Tables[0].Rows.Count > 0)
                {
                    var reportIdVsRecords = modifiedDataInReports.Tables[0].AsEnumerable().GroupBy(x => Convert.ToInt32(x["report_id"])).ToDictionary(x => x.Key, y => y.GroupBy(z => Convert.ToBoolean(z["isTSUpdate"])).ToDictionary(z => z.Key, zz => zz.Select(zzz => Convert.ToString(zzz["entity_code"])).ToHashSet()));

                    foreach (int reportId in reportIdVsRecords.Keys)
                    {
                        refReportIdsForSequential.Add(reportId);

                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntities = new HashSet<string>();
                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = new HashSet<string>();
                        reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = new HashSet<string>();

                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntities = new HashSet<string>();
                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = new HashSet<string>();
                        reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = new HashSet<string>();


                        if (reportIdVsRecords[reportId].ContainsKey(true))
                        {
                            reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = reportIdVsRecords[reportId][true];
                            reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithTSUpdates = reportIdVsRecords[reportId][true];
                        }
                        if (reportIdVsRecords[reportId].ContainsKey(false))
                        {
                            reportIdVsInfo[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = reportIdVsRecords[reportId][false];
                            reportIdVsInfoGlobal[REFM_CONSTANT][reportId].ModifiedEntitiesWithManualUpdates = reportIdVsRecords[reportId][false];
                        }
                    }
                }
            }

            if (refReportIdsForParallel.Count > 0 || refReportIdsForSequential.Count > 0 || secReportIdsForSequential.Count > 0 || secReportIdsForParallel.Count > 0)
            {
                int id = InsertInfoForSetupExecution(setupStatusId, reportIdVsInfo, refReportIdsForParallel, refReportIdsForSequential, secReportIdsForSequential, secReportIdsForParallel);

                if (refReportIdsForParallel.Count > 0 || refReportIdsForSequential.Count > 0)
                    CallWorkerProcess(REFM_CONSTANT, id, blockType, false, reportIdVsInfo[REFM_CONSTANT].FirstOrDefault().Value.SetupName.Replace(" ", "_"), cacheKey, ref waitForWorkerResponse);

                if (secReportIdsForSequential.Count > 0 || secReportIdsForParallel.Count > 0)
                    CallWorkerProcess(SECM_CONSTANT, id, blockType, false, reportIdVsInfo[SECM_CONSTANT].FirstOrDefault().Value.SetupName.Replace(" ", "_"), cacheKey, ref waitForWorkerResponse);
            }
            return isPassed;
        }

        private void GetLegReports(ref Dictionary<string, List<int>> moduleVsLegReports)
        {
            bool isSecMasterExists = SRMDownstreamConfiguration.CheckDatabaseExists("IVPSecMaster");
            string query = @"                        
                        SELECT DISTINCT rep.report_id FROM IVPRefMaster.dbo.ivp_refm_report rep
                            INNER JOIN IVPRefMaster.dbo.ivp_refm_report_configuration conf
                            ON (rep.report_id = conf.report_id)
                            INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute_map ramap
                            ON (rep.report_id = ramap.report_id)
                            INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat
                            ON (ramap.dependent_attribute_id = eat.entity_attribute_id)
                            INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                            ON (eat.entity_type_id = etyp.entity_type_id)
                            WHERE etyp.structure_type_id = 3 AND rep.is_active = 1 AND conf.is_active = 1 AND ramap.is_active = 1 AND eat.is_active = 1" +

                        (isSecMasterExists ? @"
                        SELECT rs.report_setup_id
                            FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attribute_mapping ram ON ra.report_attribute_id = ram.report_attribute_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_details ad ON ram.attribute_id = ad.attribute_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_table st ON ad.sectype_table_id = st.sectype_table_id
                            WHERE priority < 0 AND ad.is_active = 1 AND rs.is_active = 1
                            UNION
                            SELECT rs.[report_setup_id]
                            FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_additional_leg_attribute_mapping ram ON ra.report_attribute_id = ram.report_attribute_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs_attribute_details ad ON ram.attribute_id = ad.attribute_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs al ON ad.additional_legs_id = al.additional_legs_id
                            WHERE ad.is_active = 1 AND al.is_active = 1 AND rs.is_active = 1" : "");

            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                moduleVsLegReports.Add(REFM_CONSTANT, ds.Tables[0].AsEnumerable().Select(x => Convert.ToInt32(x["report_id"])).ToList());
            }
            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                moduleVsLegReports.Add(SECM_CONSTANT, ds.Tables[1].AsEnumerable().Select(x => Convert.ToInt32(x["report_setup_id"])).ToList());
            }
        }

        private int InsertInfoForSetupExecution(int setupStatusId, Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo, List<int> refReportIdsForParallel, List<int> refReportIdsForSequential, List<int> secReportIdsForSequential, List<int> secReportIdsForParallel)
        {
            string reportInfo = JsonConvert.SerializeObject(reportIdVsInfo);
            string query = string.Format(@"
                    INSERT INTO [dbo].[ivp_srm_dwh_downstream_execution_info] (setup_status_id,report_info,ref_report_id_parallel,ref_report_id_sequential,sec_report_id_sequential,sec_report_id_parallel,is_active,created_by,created_on) 
                      OUTPUT INSERTED.id
                      VALUES ({0},'{1}','{2}','{3}','{4}','{5}',1,'{6}',GETDATE())", setupStatusId, reportInfo,
                string.Join(",", refReportIdsForParallel),
                string.Join(",", refReportIdsForSequential),
                string.Join(",", secReportIdsForSequential),
                string.Join(",", secReportIdsForParallel),
                "SYSTEM");
            return Convert.ToInt32(CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0].Rows[0][0]);
        }

        private static ObjectSet FetchModifiedEntities(List<int> refReportIds, Dictionary<int, SRMJobInfo> reportIdVsInfo)
        {
            string tableName = "[ivp_reportIdVsStartDate" + Guid.NewGuid() + "]";
            string sqlQuery = "CREATE TABLE " + tableName + " (report_id INT, start_date DATETIME, end_date DATETIME)";
            ObjectSet os = CommonDALWrapper.ExecuteSelectQueryObject(sqlQuery, ConnectionConstants.RefMaster_Connection);
            ObjectTable ot = new ObjectTable();
            ot.Columns.Add(new ObjectColumn("report_id", typeof(System.Int32)));
            ot.Columns.Add(new ObjectColumn("start_date", typeof(System.DateTime)));
            ot.Columns.Add(new ObjectColumn("end_date", typeof(System.DateTime)));
            foreach (var id in refReportIds)
            {
                ObjectRow or = ot.NewRow();
                or[0] = id;
                or[1] = DateTime.ParseExact(reportIdVsInfo[id].StartDate, "yyyyMMdd HH:mm:ss.fff", null);
                or[2] = DateTime.ParseExact(reportIdVsInfo[id].EndDate, "yyyyMMdd HH:mm:ss.fff", null);
                ot.Rows.Add(or);
            }
            CommonDALWrapper.ExecuteBulkUploadObject(tableName, ot, ConnectionConstants.RefMaster_Connection);

            sqlQuery = string.Format(@"
                    
                    CREATE TABLE #modifiedReportEntities(report_id INT , entity_type_name VARCHAR(MAX),entity_code VARCHAR(10),start_date DATETIME,modified_entity_codes VARCHAR(MAX),end_date DATETIME)
                    CREATE TABLE #modifiedEntities(entity_code VARCHAR(20),report_id INT,entity_type_name VARCHAR(1000), isTSUpdate BIT)

                    INSERT INTO #modifiedReportEntities (report_id, entity_type_name ,entity_code,start_date,end_date)
                    SELECT report.report_id, etyp.entity_type_name, etyp.entity_code, report.start_date, report.end_date
                    FROM IVPRefMaster.dbo.ivp_refm_report_entity_map map
                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                    ON(map.dependent_id = etyp.entity_type_id AND map.is_active = 1)
                    INNER JOIN {0} report
                    ON(report.report_id = map.report_id)

                    SELECT DISTINCT tab.report_id,ts.entity_code,et.entity_type_id,entity_type_name,structure_type_id
                    INTO #ts_EC
                    FROM {0} tab
                    INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute ra
                    ON (ra.report_id = tab.report_id)
                    INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute_map ramap
                    ON (ra.report_attribute_id = ramap.report_attribute_id)
                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute ea
                    ON (ea.entity_attribute_id = ramap.dependent_attribute_id AND ea.is_active = 1)
                    INNER JOIN IVPRefMaster.dbo.ivp_Refm_entity_Type ET
                    ON (ET.entity_type_id = ea.entity_type_id AND et.is_active = 1)
                    INNER JOIN IVPRefMaster_Archive.dbo.ivp_refm_time_series_history ts
                    ON (ts.entity_attribute_id = ea.entity_attribute_id AND ts.knowledge_date > tab.start_date AND ts.knowledge_date <= tab.end_date)
                    WHERE ra.is_active = 1 AND ramap.is_active = 1 AND ts.is_active = 1

                    SELECT DISTINCT entity_type_id,entity_type_name
                    INTO #legETs
                    FROM #ts_EC
                    WHERE structure_type_id = 3

                    DECLARE @sql VARCHAR(MAX) = ''

                    SELECT @sql += CHAR(13) +
                    'UPDATE ts
                    SET entity_code = tab.master_entity_code
                    FROM #ts_EC ts
                    INNER JOIN IVPRefMaster_Archive.dbo.[' + entity_type_name+ '] tab
                    ON (ts.entity_code = tab.entity_code AND ts.entity_type_id = ' + CAST(entity_type_id AS VARCHAR(MAX)) + ')'
                    FROM #legETs
                    PRINT (@sql)
                    EXEC (@sql)

                    SELECT @sql = ''
                    SELECT @sql += CHAR(13) +
                    'INSERT INTO #modifiedEntities
                    SELECT DISTINCT entity_code ,''' + CAST(report_id AS VARCHAR) + ''',''' + entity_type_name + ''',0 FROM IVPRefMaster_Archive.dbo.'+ entity_type_name +' WHERE knowledge_date > '''+ CONVERT(VARCHAR,start_date,121) +''' AND
                    knowledge_date <= '''+ CONVERT(VARCHAR,end_date,121) +'''  AND knowledge_date = loading_time '
                    FROM #modifiedReportEntities

                    EXEC(@sql)

                    DELETE tab
                    FROM #modifiedEntities tab
                    INNER JOIN #ts_EC ts
                    ON (tab.entity_code = ts.entity_code AND tab.report_id = ts.report_id)

                    INSERT INTO #modifiedEntities
                    SELECT entity_code,report_id,entity_type_name,1
                    FROM #ts_EC

                    SELECT report_id,entity_code,isTSUpdate FROM #modifiedEntities

                    DROP TABLE #ts_EC
                    DROP TABLE #modifiedEntities
                    DROP TABLE {0}
                    DROP TABLE #modifiedReportEntities
                    DROP TABLE #legETs", tableName);

            ObjectSet modifiedDataInReports = CommonDALWrapper.ExecuteSelectQueryObject(sqlQuery, ConnectionConstants.RefMaster_Connection);
            return modifiedDataInReports;
        }

        private static void PopulateJobDetails(string downstreamConnectionName, BLOCK_TYPES blockType, DataRow row, int setupStatusId, DateTime loadingTimeForStagingTable, out SRMJobInfo job)
        {
            string startDate = "19000101 00:00:00.000";

            job = new SRMJobInfo()
            {
                SetupName = Convert.ToString(row["setup_name"]),
                ReportId = Convert.ToInt32(row["report_id"]),
                BlockId = Convert.ToInt32(row["block_id"]),
                BlockType = blockType,
                StartDate = startDate,
                EndDate = RCalenderUtils.FormatDate(loadingTimeForStagingTable, "yyyyMMdd HH:mm:ss.fff"),
                Calendar = Convert.ToInt32(row["calendar_id"]),
                IsRef = Convert.ToBoolean(row["is_ref"]),
                DownstreamSQLConnectionName = downstreamConnectionName,
                SetupId = Convert.ToInt32(row["setup_id"]),
                TableName = Convert.ToString(row["table_name"]),
                BackupTableName = Convert.ToString(row["table_name"]).Substring(0, Convert.ToString(row["table_name"]).Length - 1) + "_backup]",
                BatchSize = Convert.ToInt32(row["batch_size"]),
                RequireKnowledgeDateReporting = Convert.ToBoolean(row["require_knowledge_date_reporting"]),
                RequireTimeTSReports = !string.IsNullOrEmpty(Convert.ToString(row["require_time_in_ts_report"])) ? Convert.ToBoolean(row["require_time_in_ts_report"]) : false,
                RequireDeletedRows = Convert.ToBoolean(row["require_deleted_asset_types"]),
                RequireReferenceMassagingOnStartDate = Convert.ToBoolean(row["require_lookup_massaging_start_date"]),
                RequireReferenceMassagingOnCurrentKnowledgeDate = !Convert.ToBoolean(row["require_lookup_massaging_current_knowledge_date"]),
                CustomClassAssemblyName = Convert.ToString(row["cc_assembly_name"]),
                CustomClassName = Convert.ToString(row["cc_class_name"]),
                CustomClassMethodName = Convert.ToString(row["cc_method_name"]),
                QueueName = Convert.ToString(row["queue_name"]),
                FailureEmailId = Convert.ToString(row["failure_email_id"]),
                SetupStatusId = setupStatusId,
                EffectiveStartDateForReport = Convert.ToDateTime(row["effective_from_date"]),
                RequireBatching = true,
                DateType = (DWHDateType)Enum.Parse(typeof(DWHDateType), Convert.ToString(row["start_date"]), true),
                LoadingTimeForStagingTable = loadingTimeForStagingTable,
                UnderlyingAttributes = new HashSet<string>()
            };
            if (Convert.ToString(row["start_date"]).Equals(((int)DWHDateType.LastExtractionDate).ToString()))
            {
                string query = string.Format(@"
                            SELECT last_run_date FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_reporting_time WHERE setup_id = {0} AND block_id = {1}", job.SetupId, job.BlockId);

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DateTime blockStartDate = Convert.ToDateTime(ds.Tables[0].Rows[0][0]);
                    job.StartDate = RCalenderUtils.FormatDate(blockStartDate, "yyyyMMdd HH:mm:ss.fff");
                }
                else
                    job.DateType = DWHDateType.None;
            }
            if ((job.BlockType == BLOCK_TYPES.TS_REPORT || job.BlockType == BLOCK_TYPES.DAILY_REPORT) && job.DateType == DWHDateType.None)
            {
                job.TableName = job.TableName.Replace("[taskmanager]", "[dimension]").Replace("[references]", "[dimension]");
                job.BackupTableName = job.BackupTableName.Replace("[taskmanager]", "[dimension]").Replace("[references]", "[dimension]");
            }
            if (job.IsRef)
            {
                string query = string.Format(@" DECLARE @dependentId INT , @reportId INT = {0}
                                                SELECT DISTINCT attr.report_attribute_name FROM 
                                                IVPRefMaster.dbo.ivp_refm_report_attribute attr
                                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_configuration conf
												ON (attr.report_id = conf.report_id)
												CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowID(attribute_to_show,'|') tab
                                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute_map map
                                                ON(attr.report_attribute_id = map.report_attribute_id AND attr.report_attribute_id = tab.item)
                                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat
                                                ON(eat.entity_attribute_id = map.dependent_attribute_id)
                                                WHERE attr.report_id = @reportId AND eat.data_type = 'LOOKUP' AND lookup_attribute_id IS NULL
                                                AND map.is_active = 1 AND attr.is_active = 1 AND eat.is_active = 1

                                                SELECT DISTINCT attr.report_attribute_name, attr.report_attribute_id FROM 
                                                IVPRefMaster.dbo.ivp_refm_report_attribute attr
                                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_configuration conf
												ON (attr.report_id = conf.report_id)
												CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowID(attribute_to_show,'|') tab
                                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute_map map
                                                ON(attr.report_attribute_id = map.report_attribute_id AND attr.report_attribute_id = tab.item)
                                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat
                                                ON(eat.entity_attribute_id = map.dependent_attribute_id)
                                                WHERE attr.report_id = @reportId AND eat.data_type = 'SECURITY_LOOKUP' AND lookup_attribute_id IS NULL
                                                AND map.is_active = 1 AND attr.is_active = 1 AND eat.is_active = 1
                                            
                                                SELECT @dependentId = dependent_id FROM IVPRefMaster.dbo.ivp_refm_report_entity_map WHERE report_id = @reportId

                                                SELECT TOP 1 rep.report_name FROM IVPRefMaster.dbo.ivp_refm_report rep
                                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                                                ON (rep.report_id = emap.report_id)
                                                INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
												ON(det.report_id = rep.report_id)
                                                WHERE emap.dependent_id = @dependentId AND rep.is_active = 1 AND emap.is_active = 1
                                                AND rep.report_id != @reportId AND rep.report_name LIKE '%Non[ ]Time[ ]Series' AND rep.report_name NOT LIKE '%[ ]leg[ ]%'
                                                AND det.is_ref = 1 AND det.is_active = 1 AND det.setup_id = {1}

                                                SELECT TOP 1 rep.report_name,rep.report_id FROM IVPRefMaster.dbo.ivp_refm_report rep
                                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                                                ON (rep.report_id = emap.report_id)
                                                INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
												ON(det.report_id = rep.report_id)
                                                WHERE emap.dependent_id = @dependentId AND rep.is_active = 1 AND emap.is_active = 1
                                                AND rep.report_id != @reportId AND rep.report_name LIKE '%Time[ ]Series' AND rep.report_name NOT LIKE '%Non[ ]Time[ ]Series' 
                                                AND rep.report_name NOT LIKE '%[ ]leg[ ]%'
                                                AND det.is_ref = 1 AND det.is_active = 1 AND det.setup_id = {1}

                                                SELECT report_name FROM IVPRefMaster.dbo.ivp_refm_report WHERE report_id = {0} 

                                                SELECT @dependentId", job.ReportId, job.SetupId);
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    job.ReferenceAttributes = new HashSet<string>(ds.Tables[0].AsEnumerable().Select(x => Convert.ToString(x["report_attribute_name"])));

                if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    HashSet<int> attributesWithSecId = new HashSet<int>();
                    if (!string.IsNullOrEmpty(RefSecLookupAttributesWithSecId))
                    {
                        attributesWithSecId = new HashSet<int>(RefSecLookupAttributesWithSecId.Split(',').Select(int.Parse));
                    }

                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        int reportAttributeId = Convert.ToInt32(dr["report_attribute_id"]);
                        string reportAttributeName = Convert.ToString(dr["report_attribute_name"]);
                        if (attributesWithSecId.Contains(reportAttributeId))
                            job.UnderlyingAttributes.Add(reportAttributeName);
                    }
                }

                if (ds != null && ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                    job.NonTimeSeriesReportName = Convert.ToString(ds.Tables[2].Rows[0][0]);

                if (ds != null && ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                {
                    job.TimeSeriesReportName = Convert.ToString(ds.Tables[3].Rows[0][0]);
                    job.TimeSeriesReportId = Convert.ToInt32(ds.Tables[3].Rows[0][1]);
                    if (job.BlockType == BLOCK_TYPES.DAILY_REPORT)
                        job.TimeSeriesTableNameWithSchema = "[dimension].[ivp_polaris_" + job.TimeSeriesReportName.ToLower().Replace(" ", "_") + "]";
                }

                if (ds != null && ds.Tables.Count > 4 && ds.Tables[4].Rows.Count > 0)
                {
                    job.ReportName = Convert.ToString(ds.Tables[4].Rows[0][0]);
                }

                if (ds != null && ds.Tables.Count > 5 && ds.Tables[5].Rows.Count > 0)
                    job.InstrumentTypeId = Convert.ToInt32(ds.Tables[5].Rows[0][0]);
            }
            else
            {
                string query = string.Format(@"SELECT TOP 1 rep.report_name,rep.report_setup_id FROM IVPSecMaster.dbo.ivp_secm_report_setup rep
                                INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                ON (rep.report_setup_id = det.report_id)
                                WHERE rep.is_active = 1 AND det.is_active = 1
                                AND det.setup_id = {0} AND rep.report_name = 'Security Time Series' AND det.is_ref = 0

                                SELECT TOP 1 rep.report_name FROM IVPSecMaster.dbo.ivp_secm_report_setup rep
                                INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                ON (rep.report_setup_id = det.report_id)
                                WHERE rep.is_active = 1 AND det.is_active = 1
                                AND det.setup_id = {0} AND rep.report_name = 'Security Non Time Series' AND det.is_ref = 0  

                                SELECT DISTINCT report_name,report_attribute_name
                                FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id 
                                INNER JOIN IVPSecMaster..ivp_secm_report_configuration rc on rs.report_setup_id = rc.report_setup_id
                                CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(rc.attributes_to_show,'|') tab
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attribute_mapping ram 
	                                ON ra.report_attribute_id = ram.report_attribute_id AND ISNULL(ram.attribute_id,0) > 0
	                                AND 
	                                CASE WHEN tab.item LIKE 'A:%' OR tab.item LIKE 'S:%' 
		                                THEN SUBSTRING(SUBSTRING(tab.item,3,LEN(tab.item)-2),1,CHARINDEX(':',SUBSTRING(tab.item,3,LEN(tab.item)-2))-1) 
		                                WHEN tab.item LIKE 'R::%' 
		                                THEN REPLACE(tab.item,'R::','') 
	                                END = ra.report_attribute_id
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_reference_attribute_mapping ramm
                                ON (ram.attribute_id = ramm.attribute_id AND ram.reference_attribute_id = '-1')
                                WHERE rs.report_setup_id = {1}
                                UNION ALL
                                SELECT DISTINCT report_name,report_attribute_name
                                FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id 
                                INNER JOIN IVPSecMaster..ivp_secm_report_configuration rc on rs.report_setup_id = rc.report_setup_id
                                CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(rc.attributes_to_show,'|') tab
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_report_additional_leg_attribute_mapping ram 
	                                ON ra.report_attribute_id = ram.report_attribute_id 
	                                AND 
	                                CASE WHEN tab.item LIKE 'A:%' OR tab.item LIKE 'S:%' 
		                                THEN SUBSTRING(SUBSTRING(tab.item,3,LEN(tab.item)-2),1,CHARINDEX(':',SUBSTRING(tab.item,3,LEN(tab.item)-2))-1) 
		                                WHEN tab.item LIKE 'R::%' 
		                                THEN REPLACE(tab.item,'R::','') 
	                                END = ra.report_attribute_id
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs_reference_attribute_mapping ramm
                                ON (ram.attribute_id = ramm.attribute_id AND ram.reference_attribute_id = '-1')
                                WHERE rs.report_setup_id = {1}

                                SELECT DISTINCT report_name,report_attribute_name
                                FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id 
                                INNER JOIN IVPSecMaster..ivp_secm_report_configuration rc on rs.report_setup_id = rc.report_setup_id
                                CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(rc.attributes_to_show,'|') tab
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attribute_mapping ram 
	                                ON ra.report_attribute_id = ram.report_attribute_id AND ISNULL(ram.attribute_id,0) > 0
	                                AND 
	                                CASE WHEN tab.item LIKE 'A:%' OR tab.item LIKE 'S:%' 
		                                THEN SUBSTRING(SUBSTRING(tab.item,3,LEN(tab.item)-2),1,CHARINDEX(':',SUBSTRING(tab.item,3,LEN(tab.item)-2))-1) 
		                                WHEN tab.item LIKE 'R::%' 
		                                THEN REPLACE(tab.item,'R::','') 
	                                END = ra.report_attribute_id
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_details ad
                                ON (ram.attribute_id = ad.attribute_id AND ram.reference_attribute_id = '-1')
                                WHERE rs.report_setup_id = {1} AND ad.attribute_name = 'underlying_sec_id'
                                UNION ALL
                                SELECT DISTINCT report_name,report_attribute_name
                                FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id 
                                INNER JOIN IVPSecMaster..ivp_secm_report_configuration rc on rs.report_setup_id = rc.report_setup_id
                                CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(rc.attributes_to_show,'|') tab
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_report_additional_leg_attribute_mapping ram 
	                                ON ra.report_attribute_id = ram.report_attribute_id 
	                                AND 
	                                CASE WHEN tab.item LIKE 'A:%' OR tab.item LIKE 'S:%' 
		                                THEN SUBSTRING(SUBSTRING(tab.item,3,LEN(tab.item)-2),1,CHARINDEX(':',SUBSTRING(tab.item,3,LEN(tab.item)-2))-1) 
		                                WHEN tab.item LIKE 'R::%' 
		                                THEN REPLACE(tab.item,'R::','') 
	                                END = ra.report_attribute_id
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs_attribute_details ad
                                ON (ram.attribute_id = ad.attribute_id AND ram.reference_attribute_id = '-1')
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs_attribute_type ast ON ad.attribute_type_id = ast.attribute_type_id
                                WHERE rs.report_setup_id = {1} AND display_data_type = 'SECURITY'

                                SELECT report_name FROM IVPSecMaster.dbo.ivp_secm_report_setup WHERE report_setup_id = {1}", job.SetupId, job.ReportId);

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.SecMaster_Connection);

                job.InstrumentTypeId = -1;

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    job.TimeSeriesReportName = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    job.TimeSeriesReportId = Convert.ToInt32(ds.Tables[0].Rows[0][1]);

                    if (job.BlockType == BLOCK_TYPES.DAILY_REPORT)
                        job.TimeSeriesTableNameWithSchema = "[dimension].[ivp_polaris_" + job.TimeSeriesReportName.ToLower().Replace(" ", "_") + "]";
                }

                if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    job.NonTimeSeriesReportName = Convert.ToString(ds.Tables[1].Rows[0][0]);

                if (ds != null && ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                    job.ReferenceAttributes = new HashSet<string>(ds.Tables[2].AsEnumerable().Select(x => Convert.ToString(x["report_attribute_name"])));

                if (ds != null && ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                    job.UnderlyingAttributes = new HashSet<string>(ds.Tables[3].AsEnumerable().Select(x => Convert.ToString(x["report_attribute_name"])));

                if (ds != null && ds.Tables.Count > 4 && ds.Tables[4].Rows.Count > 0)
                    job.ReportName = Convert.ToString(ds.Tables[4].Rows[0][0]);
            }
            if (job.BlockType == BLOCK_TYPES.DAILY_REPORT || (job.BlockType == BLOCK_TYPES.TS_REPORT && job.RequireTimeTSReports))
            {
                job.ReferenceDailyAttributes = new HashSet<string>(GetReferenceAttributesWithDailyReport(job.IsRef, job.ReportId, job.SetupId));
            }

            SRMDownstreamSyncReportsAvailable reportInfo = new SRMDownstreamSyncReportsAvailable()
            {
                Module = job.IsRef ? "Ref" : "Sec",
                Name = job.ReportName,
                Details = new SRMDownstreamSyncReportDetails()
                {
                    StartDateValue = job.DateType.ToString(),
                    TableName = job.TableName
                }
            };

            string errorMessage = SRMDownstreamConfiguration.ValidateReportTableName(reportInfo);

            if (!string.IsNullOrEmpty(errorMessage))
                throw new Exception("Error while validating report : " + errorMessage);

            if (!job.TableName.Equals(reportInfo.Details.TableName, StringComparison.OrdinalIgnoreCase) &&
                !((job.BlockType == BLOCK_TYPES.TS_REPORT || job.BlockType == BLOCK_TYPES.DAILY_REPORT) && job.DateType == DWHDateType.None))
            {
                job.TableName = reportInfo.Details.TableName;
                SRMDownstreamConfiguration conf = new SRMDownstreamConfiguration();

                ObjectSet os = conf.GetConfiguredReports(new List<int> { job.SetupId }, out errorMessage, "MM/dd/yyyy");

                if (!string.IsNullOrEmpty(errorMessage))
                    throw new Exception("Error while getting configured system ReportName - " + job.ReportName + ": " + errorMessage);

                conf.StartSync(os, out errorMessage, "SYSTEM", "MM/dd/yyyy");

                if (!string.IsNullOrEmpty(errorMessage))
                    throw new Exception("Error while saving system ReportName - " + job.ReportName + ": " + errorMessage);
            }
        }

        private static List<string> GetReferenceAttributesWithDailyReport(bool isRefM, int reportId, int setupId)
        {
            ObjectSet os = new ObjectSet();
            List<string> refAttributesWithDailyReport = new List<string>();
            if (isRefM)
            {
                string sqlQuery = string.Format(@"DECLARE @dailyAlias VARCHAR(100) = 'Daily'

                    DECLARE @reportId INT = '{0}'
                    CREATE TABLE #reportInfo(id INT IDENTITY,report_name VARCHAR(1000),report_attribute_name VARCHAR(1000),data_type VARCHAR(500),parent_entity_type_id INT,table_alias VARCHAR(50),parent_report_name VARCHAR(1000),
	                    column_alias VARCHAR(1000), join_condition VARCHAR(MAX))

                    INSERT INTO #reportInfo
                    SELECT DISTINCT rep.report_name,ra.report_attribute_name,eat.data_type,look.parent_entity_type_id, 'TAB_DAILY',null,ra.report_attribute_name,null
                    FROM IVPRefMaster.dbo.ivp_refm_report rep
                    INNER JOIN IVPRefMaster.dbo.ivp_refm_report_configuration con
                    ON(rep.report_id = con.report_id AND con.is_active = 1 AND rep.report_id = @reportId)
                    CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(con.attribute_to_show, '|') tab
                
				    INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute ra
                    ON(ra.report_id = rep.report_id AND CAST(ra.report_attribute_id AS VARCHAR) = tab.item)
                    INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute_map map
                    ON(ra.report_attribute_id = map.report_attribute_id)
                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat
                    ON(eat.entity_attribute_id = map.dependent_attribute_id)
                    LEFT JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute_lookup look
                    ON(look.child_entity_attribute_id = eat.entity_attribute_id)
                    WHERE rep.report_type_id IN (1, 6) AND rep.is_active = 1 AND rep.is_legacy_report = 0 AND eat.data_type = 'LOOKUP'


                    UPDATE r
                        SET r.parent_report_name = rep.report_name , r.data_type = 'VARCHAR'
                        FROM #reportInfo r
		                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                        ON(emap.dependent_id = r.parent_entity_type_id AND emap.is_active = 1)
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
                        ON(rep.report_id = emap.report_id AND rep.is_active = 1)

					    INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
					    ON (det.report_id = rep.report_id  AND det.is_active = 1 AND det.is_ref = 1 AND det.setup_id = {1})
					    INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_type blt
					    ON (blt.block_type_id = det.block_type_id  AND blt.block_type_name = 'DAILY')
                
                        WHERE rep.report_name like '%' + @dailyAlias + '' AND r.data_type = 'LOOKUP' AND (r.report_name like '%'+ @dailyAlias +'' OR r.report_name LIKE '%Time Series%') AND rep.report_name not like '% leg %'

                        SELECT report_attribute_name FROM #reportInfo WHERE parent_report_name IS NOT NULL

		                DROP TABLE #reportInfo", reportId, setupId);
                os = CommonDALWrapper.ExecuteSelectQueryObject(sqlQuery, ConnectionConstants.RefMaster_Connection);
            }
            else
            {
                string sqlQuery = string.Format(@"DECLARE @dailyAlias VARCHAR(100) = 'Daily'

                CREATE TABLE #reportInfo(id INT IDENTITY,report_name VARCHAR(1000),report_attribute_name VARCHAR(1000),data_type VARCHAR(500),parent_entity_type_id INT,table_alias VARCHAR(50),parent_report_name VARCHAR(1000),
	                column_alias VARCHAR(1000),join_condition VARCHAR(MAX))

                INSERT INTO #reportInfo
                SELECT DISTINCT report_name,report_attribute_name,
                CASE WHEN ramm.reference_attribute_id IS NOT NULL THEN 'LOOKUP' ELSE ra.data_type_name END,
                ramm.reference_type_id,
				'TAB_DAILY',null,ra.report_attribute_name,null
                  FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                  INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id 
                  INNER JOIN IVPSecMaster..ivp_secm_report_configuration rc on rs.report_setup_id = rc.report_setup_id
                  CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(rc.attributes_to_show,'|') tab
                  INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attribute_mapping ram 
						ON ra.report_attribute_id = ram.report_attribute_id AND ISNULL(ram.attribute_id,0) > 0
						AND 
						CASE WHEN tab.item LIKE 'A:%' OR tab.item LIKE 'S:%' 
							THEN SUBSTRING(SUBSTRING(tab.item,3,LEN(tab.item)-2),1,CHARINDEX(':',SUBSTRING(tab.item,3,LEN(tab.item)-2))-1) 
							WHEN tab.item LIKE 'R::%' 
							THEN REPLACE(tab.item,'R::','') 
						END = ra.report_attribute_id
                  LEFT JOIN IVPSecMaster.dbo.ivp_secm_reference_attribute_mapping ramm
                  ON (ram.attribute_id = ramm.attribute_id AND ram.reference_attribute_id = '-1' AND ISNULL(reference_type_id,-1) > 0)
                  WHERE report_name IN ('Security Daily','security time series')
				  UNION ALL
				  SELECT DISTINCT report_name,report_attribute_name,
					CASE WHEN ramm.reference_attribute_id IS NOT NULL THEN 'LOOKUP' ELSE ra.data_type_name END,
					ramm.reference_type_id,'TAB_DAILY',null,ra.report_attribute_name,null
                  FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                  INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id 
                  INNER JOIN IVPSecMaster..ivp_secm_report_configuration rc on rs.report_setup_id = rc.report_setup_id
                  CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(rc.attributes_to_show,'|') tab
                  INNER JOIN IVPSecMaster.dbo.ivp_secm_report_additional_leg_attribute_mapping ram 
						ON ra.report_attribute_id = ram.report_attribute_id 
						AND 
						CASE WHEN tab.item LIKE 'A:%' OR tab.item LIKE 'S:%' 
							THEN SUBSTRING(SUBSTRING(tab.item,3,LEN(tab.item)-2),1,CHARINDEX(':',SUBSTRING(tab.item,3,LEN(tab.item)-2))-1) 
							WHEN tab.item LIKE 'R::%' 
							THEN REPLACE(tab.item,'R::','') 
						END = ra.report_attribute_id
                  LEFT JOIN IVPSecMaster.dbo.ivp_secm_additional_legs_reference_attribute_mapping ramm
                  ON (ram.attribute_id = ramm.attribute_id AND ram.reference_attribute_id = '-1' AND ISNULL(reference_type_id,-1) > 0)
                  WHERE report_name IN ('Security Daily','security time series')

                UPDATE r
		                SET r.parent_report_name = rep.report_name , r.data_type = 'VARCHAR'
		                FROM #reportInfo r
		                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
		                ON (emap.dependent_id = r.parent_entity_type_id AND emap.is_active = 1)
		                INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep
		                ON (rep.report_id = emap.report_id AND rep.is_active = 1)

		                INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
					    ON (det.report_id = rep.report_id  AND det.is_active = 1 AND det.is_ref = 1 AND det.setup_id = {0})
					    INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_type blt
					    ON (blt.block_type_id = det.block_type_id  AND blt.block_type_name = 'DAILY')

		                WHERE rep.report_name like '%'+ @dailyAlias +'' AND r.data_type = 'LOOKUP' AND (r.report_name like '%'+ @dailyAlias +'' OR r.report_name LIKE '%Time Series%') AND rep.report_name not like '% leg %'
		
		                SELECT report_attribute_name FROM #reportInfo WHERE parent_report_name IS NOT NULL
		
		                DROP TABLE #reportInfo", setupId);

                os = CommonDALWrapper.ExecuteSelectQueryObject(sqlQuery, ConnectionConstants.RefMaster_Connection);
            }
            if (os != null && os.Tables.Count > 0 && os.Tables[0].Rows.Count > 0)
            {
                refAttributesWithDailyReport = os.Tables[0].AsEnumerable().Select(x => Convert.ToString(x["report_attribute_name"])).ToList<string>();
            }
            else
                refAttributesWithDailyReport = new List<string>();

            return refAttributesWithDailyReport;
        }

        private void CallWorkerProcess(string module, int executionInfoId, BLOCK_TYPES blockType, bool runLoaderInTransaction, string setupName, string cacheKey, ref bool waitForWorkerResponse)
        {
            waitForWorkerResponse = true;
            try
            {
                mLogger.Debug("SRMDWHJobWorker Path : " + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SRMDWHJobWorker.exe"));
                ProcessStartInfo processInfo = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SRMDWHJobWorker.exe"), module + " " + executionInfoId + " " + blockType + " " + runLoaderInTransaction + " " + setupName + " " + cacheKey);

                processInfo.CreateNoWindow = false;
                processInfo.UseShellExecute = false;
                processInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardInput = true;
                processInfo.RedirectStandardOutput = true;

                Process process = Process.Start(processInfo);
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(process_Exited);

            }
            catch (Exception ex)
            {
                mLogger.Error("Unable to CallWorkerProcess :: " + ex);

            }
        }

        private static bool WorkerProcessStatus(Process process, int setupStatusId)
        {
            List<Task<bool>> tasks = new List<Task<bool>>();

            tasks.Add(Task.Factory.StartNew((param) =>
            {
                var processLocal = (Process)param;
                processLocal.WaitForExit();
                var exitCode = processLocal.ExitCode;
                if (exitCode != 0)
                    return false;
                return true;

            }, process));

            tasks.Add(Task.Factory.StartNew(() =>
            {
                return ResponseFromPolling(setupStatusId, process.Id);
            }));

            var t = Task.WhenAny(tasks).Result;
            return t.Result;
        }

        private static bool ResponseFromPolling(int setupStatusId, int id)
        {
            while (true)
            {
                HashSet<int> runningProcesses = new HashSet<int>(Process.GetProcessesByName("SRMDWHJobWorker").Select(x => x.Id));

                if (!runningProcesses.Contains(id))
                    return false;
                else
                {
                    Thread.Sleep(100000);
                }

            }
        }

        void process_Exited(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(((Process)sender).StandardError.ReadToEnd().Trim()))
                {
                    mLogger.Error("SRMDWHJobWorker : " + ((Process)sender).StandardError.ReadToEnd().Trim());
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMDWHJobWorker error in new process exited : " + ex);
            }

        }

        public int InsertUpdateStatusForBlock(int setupStatusId, int blockId, EXECUTION_STATUS status, string userName, string failure_reason, int statusId, DateTime endTime)
        {
            if (statusId != -1 && status == EXECUTION_STATUS.INPROGRESS)
                return -1;
            if (statusId == -1 && status == EXECUTION_STATUS.INPROGRESS)
            {
                string query = @"INSERT INTO [dbo].[ivp_srm_dwh_downstream_block_status] (setup_status_id,block_id,status,failure_reason,is_active,start_time,process_id) OUTPUT INSERTED.block_status_id
                    VALUES (" + setupStatusId + "," + blockId + ",'" + status + "','" + failure_reason + "',1,GETDATE()," + Process.GetCurrentProcess().Id + ")";
                return Convert.ToInt32(CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0].Rows[0][0]);
            }
            else
            {
                if (failure_reason.Length > 0)
                {
                    int maxLength = failure_reason.Length - 1;
                    if (maxLength > 8000)
                    {
                        maxLength = 8000;
                        failure_reason = failure_reason.Substring(0, maxLength);
                    }
                    failure_reason = failure_reason.Replace("'", "''");
                }

                string query = "UPDATE [dbo].[ivp_srm_dwh_downstream_block_status] SET status = '" + status + "',failure_reason = '" + failure_reason + "', end_time = '" + endTime.ToString("yyyyMMdd HH:mm:ss.fff") + "' WHERE block_status_id = " + statusId + " AND status != 'FAILED'";
                CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Update, ConnectionConstants.RefMaster_Connection);
                return statusId;
            }
        }


        public void DropOldStagingTables(string downstreamConnectionName)
        {
            string noOfDays = SRMCommon.GetConfigFromDB("DWHDaysToPreserveStagingTable");
            if (string.IsNullOrEmpty(noOfDays))
                noOfDays = "7";
            string query = string.Format("EXEC [dimension].[SRM_DWH_Drop_Old_Staging_Tables] {0}", Convert.ToInt32(noOfDays));
            SRMDWHJobExtension.ExecuteQueryObject(downstreamConnectionName, query, SRMDBQueryType.SELECT);

            query = string.Format("DELETE FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_execution_info WHERE created_on < DATEADD(DAY,-{0},GETDATE())", Convert.ToInt32(noOfDays));
            CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Delete, ConnectionConstants.RefMaster_Connection);
        }


        public void ClearStagingTables(string downstreamConnectionName)
        {
            string query = "EXEC [dimension].[SRM_DWH_Clear_Staging_Tables]";
            SRMDWHJobExtension.ExecuteQueryObject(downstreamConnectionName, query, SRMDBQueryType.SELECT);
        }


        public static bool CanAlterTable(string downstreamSQLConnectionName, string mainTableName)
        {
            var A9deploymentDate = Convert.ToDateTime(CommonDALWrapper.ExecuteSelectQuery("SELECT create_date FROM sys.tables WHERE name = 'ivp_srm_dwh_downstream_master'", ConnectionConstants.RefMaster_Connection).Tables[0].Rows[0][0]);

            string query2 = string.Format(@"SELECT create_date FROM sys.tables WHERE name = PARSENAME('{0}',1) AND schema_id = SCHEMA_ID('dimension')", mainTableName);
            var queryOutput = SRMDWHJobExtension.ExecuteQuery(downstreamSQLConnectionName, query2, SRMDBQueryType.SELECT).Tables[0];
            if (queryOutput.Rows.Count > 0)
            {
                var tableCreationDate = Convert.ToDateTime(queryOutput.Rows[0][0]);
                if (tableCreationDate > A9deploymentDate)
                    return true;
            }

            return false;
        }


        public static void ExitCallback()
        {
            try
            {
                string query = "SELECT DISTINCT connection_name FROM dbo.ivp_srm_dwh_downstream_master WHERE is_active = 1";
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string dbName = Convert.ToString(dr[0]);
                        CheckForHostedSetup(dbName, true, false, false);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void CheckForHostedSetup(string dbName, bool updateEndTime, bool updateStartTime, bool throwOverlapException)
        {
            RDBConnectionManager con = SRMDWHJobExtension.GetConnectionManager(dbName, true, IsolationLevel.RepeatableRead);
            bool isFailed = false;
            try
            {
                string query = "SELECT 1 FROM dimension.ivp_srm_dwh_server_setup_details(TABLOCKX) WHERE 1 = 2";
                SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.SELECT);

                //         Path    ServerName      DB      Continue
                //         1            1            1         1
                //         1            1            0         1
                //         0            1            1         0
                //         0            0            0         1
                //         0            1            0         1
                //         0            0            1         0
                //         1            0            1         0
                //         1            0            0         1

                string localExePath = DALWrapperAppend.Replace(AppDomain.CurrentDomain.BaseDirectory) + "\\" + "SRMDWHService.exe";
                string localServerName = System.Net.Dns.GetHostName();
                string localPolarisDBInstance = con.DataBaseName;
                var list = con.ConnectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.Contains('='));
                Dictionary<string, string> dictDB = list.ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1], StringComparer.OrdinalIgnoreCase);
                if (dictDB.ContainsKey("Data Source"))
                    localPolarisDBInstance = dictDB["Data Source"] + ";" + localPolarisDBInstance;
                query = "SELECT * FROM dimension.ivp_srm_dwh_server_setup_details WHERE end_time IS NULL";
                DataSet ds = SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.SELECT);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string exePath = Convert.ToString(dr["exe_path"]);
                    string serverName = Convert.ToString(dr["server_name"]);
                    string polarisDBInstance = Convert.ToString(dr["db_instance"]);

                    mLogger.Debug("localExePath : " + localExePath + " and exePath : " + exePath);
                    mLogger.Debug("localServerName : " + localServerName + " and serverName : " + serverName);
                    mLogger.Debug("localPolarisDBInstance : " + localPolarisDBInstance + " and polarisDBInstance : " + polarisDBInstance);

                    if ((exePath.Equals(localExePath, StringComparison.OrdinalIgnoreCase) && serverName.Equals(localServerName, StringComparison.OrdinalIgnoreCase) && polarisDBInstance.Equals(localPolarisDBInstance, StringComparison.OrdinalIgnoreCase))
                        ||
                        (exePath.Equals(localExePath, StringComparison.OrdinalIgnoreCase) && !serverName.Equals(localServerName, StringComparison.OrdinalIgnoreCase) && !polarisDBInstance.Equals(localPolarisDBInstance, StringComparison.OrdinalIgnoreCase))
                        ||
                        (exePath.Equals(localExePath, StringComparison.OrdinalIgnoreCase) && serverName.Equals(localServerName, StringComparison.OrdinalIgnoreCase) && !polarisDBInstance.Equals(localPolarisDBInstance, StringComparison.OrdinalIgnoreCase))
                        ||
                        (!exePath.Equals(localExePath, StringComparison.OrdinalIgnoreCase) && !serverName.Equals(localServerName, StringComparison.OrdinalIgnoreCase) && !polarisDBInstance.Equals(localPolarisDBInstance, StringComparison.OrdinalIgnoreCase))
                        ||
                        (!exePath.Equals(localExePath, StringComparison.OrdinalIgnoreCase) && serverName.Equals(localServerName, StringComparison.OrdinalIgnoreCase) && !polarisDBInstance.Equals(localPolarisDBInstance, StringComparison.OrdinalIgnoreCase))
                        )
                    {
                        if (updateEndTime)
                        {
                            query = string.Format("UPDATE dimension.ivp_srm_dwh_server_setup_details SET end_time = GETDATE() WHERE end_time IS NULL");
                            SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.INSERT);
                        }

                        if (updateStartTime)
                        {
                            query = string.Format(@"INSERT INTO dimension.ivp_srm_dwh_server_setup_details(exe_path,server_name,db_instance,start_time) 
                                    VALUES ('{0}','{1}','{2}',GETDATE())", localExePath, localServerName, localPolarisDBInstance);
                            SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.INSERT);
                        }
                    }
                    else if (throwOverlapException)
                        throw new Exception("Cannot host multiple setups on the same SQL Instance : " + dbName + ". Overlapping with server : " + serverName);
                }
                else if (updateStartTime)
                {
                    query = string.Format(@"INSERT INTO dimension.ivp_srm_dwh_server_setup_details(exe_path,server_name,db_instance,start_time) 
                                    VALUES ('{0}','{1}','{2}',GETDATE())", localExePath, localServerName, localPolarisDBInstance);
                    SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.INSERT);
                }
            }
            catch (Exception ex)
            {
                isFailed = true;
                throw;
            }
            finally
            {
                if (con != null)
                {
                    if (isFailed)
                        con.RollbackTransaction();
                    else
                        con.CommitTransaction();
                    RDALAbstractFactory.DBFactory.PutConnectionManager(con);
                }
            }
        }
    }



    public static class SRMDWHJobQueue
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMDWHJobQueue");
        private static ConcurrentDictionary<string, Dictionary<int, ConcurrentQueue<KeyValuePair<int, string>>>> setupQueue = new ConcurrentDictionary<string, Dictionary<int, ConcurrentQueue<KeyValuePair<int, string>>>>();
        public static ConcurrentDictionary<string, Dictionary<int, RollingInfo>> dctrollingQueue = new ConcurrentDictionary<string, Dictionary<int, RollingInfo>>();
        static SRMDWHJobQueue()
        {
            string scheduledJobInterval = "3000";
            System.Timers.Timer mTimer = new System.Timers.Timer(double.Parse(scheduledJobInterval));
            mTimer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            mTimer.Start();

            System.Timers.Timer mTimer2 = new System.Timers.Timer(double.Parse(scheduledJobInterval));
            mTimer2.Elapsed += new ElapsedEventHandler(timer_Elapsed_rolling);
            mTimer2.Start();
        }

        public static int Enqueue(string clientName, int setupId, string userName)
        {
            lock (SRMDWHStatic.lockObject)
            {
                mLogger.Error("Enqueue Setup Id : " + setupId);
                int setupStatusId = new SRMDWHJob().InsertUpdateStatusForSetup(setupId, EXECUTION_STATUS.QUEUED, userName, string.Empty, -1);
                if (!setupQueue.ContainsKey(clientName))
                    setupQueue.TryAdd(clientName, new Dictionary<int, ConcurrentQueue<KeyValuePair<int, string>>>());
                if (!setupQueue[clientName].ContainsKey(setupId))
                    setupQueue[clientName].Add(setupId, new ConcurrentQueue<KeyValuePair<int, string>>());

                setupQueue[clientName][setupId].Enqueue(new KeyValuePair<int, string>(setupStatusId, userName));

                return setupStatusId;
            }
        }

        public static void EnqueueRolling(string clientName, RollingInfo rollingInfo)
        {
            lock (SRMDWHStatic.lockObject)
            {
                mLogger.Error("EnqueueRolling Setup Id : " + rollingInfo.SetupId);
                if (!dctrollingQueue.ContainsKey(clientName))
                    dctrollingQueue.TryAdd(clientName, new Dictionary<int, RollingInfo>());
                if (!dctrollingQueue[clientName].ContainsKey(rollingInfo.SetupId))
                    dctrollingQueue[clientName].Add(rollingInfo.SetupId, rollingInfo);
            }
        }

        public static void Dequeue(string clientName, out Dictionary<int, KeyValuePair<int, string>> dctSetupIdVsSetusStatusIdVsUserName)
        {
            dctSetupIdVsSetusStatusIdVsUserName = new Dictionary<int, KeyValuePair<int, string>>();
            KeyValuePair<int, string> kvp = new KeyValuePair<int, string>();
            List<int> keysToRemove = new List<int>();

            lock (SRMDWHStatic.lockObject)
            {
                if (!setupQueue.ContainsKey(clientName))
                    setupQueue.TryAdd(clientName, new Dictionary<int, ConcurrentQueue<KeyValuePair<int, string>>>());
                if (!SRMDWHStatic.RunningSetups.ContainsKey(clientName))
                    SRMDWHStatic.RunningSetups.TryAdd(clientName, new HashSet<int>());
                if (!SRMDWHStatic.RunningRollData.ContainsKey(clientName))
                    SRMDWHStatic.RunningRollData.TryAdd(clientName, new HashSet<int>());

                foreach (int setupId in setupQueue[clientName].Keys)
                {
                    if ((!SRMDWHStatic.RunningSetups[clientName].Contains((setupId)) && (!SRMDWHStatic.RunningRollData[clientName].Contains((setupId)))))
                    {
                        mLogger.Error("Adding RunningSetups for setup Id : " + setupId + " and clientName : " + clientName);

                        SRMDWHStatic.RunningSetups[clientName].Add(setupId);
                        setupQueue[clientName][setupId].TryDequeue(out kvp);
                        dctSetupIdVsSetusStatusIdVsUserName.Add(setupId, kvp);
                        if (setupQueue[clientName][setupId].Count == 0)
                            keysToRemove.Add(setupId);
                    }
                }
                foreach (var setupId in keysToRemove)
                    setupQueue[clientName].Remove(setupId);
            }
        }

        public static List<RollingInfo> DequeueRolling(string clientName)
        {
            List<RollingInfo> kstRollingInfo = new List<RollingInfo>();
            List<int> keysToRemove = new List<int>();
            lock (SRMDWHStatic.lockObject)
            {
                if (!dctrollingQueue.ContainsKey(clientName))
                    dctrollingQueue.TryAdd(clientName, new Dictionary<int, RollingInfo>());

                foreach (int setupId in dctrollingQueue[clientName].Keys)
                {
                    if ((!SRMDWHStatic.RunningSetups[clientName].Contains((setupId)) && (!SRMDWHStatic.RunningRollData[clientName].Contains((setupId)))))
                    {
                        mLogger.Error("Adding RunningRollData for setup Id : " + setupId + " and clientName : " + clientName);

                        SRMDWHStatic.RunningRollData[clientName].Add(setupId);
                        kstRollingInfo.Add(dctrollingQueue[clientName][setupId]);
                        keysToRemove.Add(setupId);
                    }
                }
                foreach (int key in keysToRemove)
                    dctrollingQueue[clientName].Remove(key);
            }
            return kstRollingInfo;
        }

        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SRMMTConfig.MethodCallPerClient(timer_ElapsedMT);
        }

        private static void timer_ElapsedMT(string clientName)
        {
            try
            {
                Dictionary<int, KeyValuePair<int, string>> dctSetupIdVsSetusStatusIdVsUserName = new Dictionary<int, KeyValuePair<int, string>>();
                Dequeue(clientName, out dctSetupIdVsSetusStatusIdVsUserName);

                Parallel.ForEach(dctSetupIdVsSetusStatusIdVsUserName.Keys, setupId =>
                {
                    mLogger.Error("Triggerting for setup Id : " + setupId);

                    new SRMDWHJob().ExecuteDWHTaskJob(clientName, setupId, dctSetupIdVsSetusStatusIdVsUserName[setupId].Value, dctSetupIdVsSetusStatusIdVsUserName[setupId].Key);
                    
                });

                lock (SRMDWHStatic.lockObjectForStatusUpdate)
                    CheckForRunningJobs();
            }
            catch (Exception ee)
            {
                mLogger.Error("ERROR in Polling thread for DWH Queue : " + ee.ToString());
            }
        }

        private static void CheckForRunningJobs()
        {
            Dictionary<int, Dictionary<int, HashSet<int>>> setupIdVsStatusIdVsProcessId = new Dictionary<int, Dictionary<int, HashSet<int>>>();
            string query = @"SELECT DISTINCT bs.setup_status_id, det.setup_id, process_id, bs.block_status_id
                                FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_status bs
                                INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                ON(bs.block_id = det.block_id)
	                            INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_setup_status st
	                            ON (st.setup_status_id = bs.setup_status_id)	
                                WHERE st.status = 'INPROGRESS' AND bs.status = 'INPROGRESS' AND det.is_active = 1";
            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Dictionary<int, HashSet<int>> dict = new Dictionary<int, HashSet<int>>();
                HashSet<int> lst = new HashSet<int>();
                HashSet<int> lst2 = new HashSet<int>();
                Dictionary<int, HashSet<int>> dictSetupStatusIdVsBlockStatusId = new Dictionary<int, HashSet<int>>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    int setupId = Convert.ToInt32(dr["setup_id"]);
                    int setupStatusId = Convert.ToInt32(dr["setup_status_id"]);
                    int processId = Convert.ToInt32(dr["process_id"]);
                    int blockStatusId = Convert.ToInt32(dr["block_status_id"]);

                    if (!setupIdVsStatusIdVsProcessId.TryGetValue(setupId, out dict))
                    {
                        dict = new Dictionary<int, HashSet<int>>();
                        setupIdVsStatusIdVsProcessId.Add(setupId, dict);
                    }

                    if (!dict.TryGetValue(setupStatusId, out lst))
                    {
                        lst = new HashSet<int>();
                        dict.Add(setupStatusId, lst);
                    }
                    if (!lst.Contains(processId))
                        lst.Add(processId);

                    if (!dictSetupStatusIdVsBlockStatusId.TryGetValue(setupStatusId, out lst2))
                    {
                        lst2 = new HashSet<int>();
                        dictSetupStatusIdVsBlockStatusId.Add(setupStatusId, lst2);
                    }
                    if (!lst2.Contains(blockStatusId))
                        lst2.Add(blockStatusId);
                }

                HashSet<int> runningProcesses = new HashSet<int>(Process.GetProcessesByName("SRMDWHJobWorker").Select(x => x.Id));
                SRMDWHJob job = new SRMDWHJob();
                DateTime endTime = DateTime.Now;
                foreach (var setupIdKVP in setupIdVsStatusIdVsProcessId)
                {
                    bool isKilled = false;
                    string emptyString = string.Empty;
                    foreach (var kvp in setupIdKVP.Value)
                    {
                        List<int> killedProcesses = kvp.Value.Except(runningProcesses.ToList()).ToList();
                        if (killedProcesses.Count > 0)
                        {
                            isKilled = true;
                            if (dictSetupStatusIdVsBlockStatusId.ContainsKey(kvp.Key))
                            {
                                foreach (var blockStatusId in dictSetupStatusIdVsBlockStatusId[kvp.Key])
                                    job.InsertUpdateStatusForBlock(0, 0, EXECUTION_STATUS.FAILED, emptyString, "Process killed", blockStatusId, endTime);
                            }
                            job.InsertUpdateStatusForSetup(setupIdKVP.Key, EXECUTION_STATUS.FAILED, emptyString, "Process killed", kvp.Key);
                        }
                    }
                    if (isKilled)
                        job.CheckIfProcessCanExecute(false, setupIdKVP.Key, ref emptyString, false);
                }
            }
        }

        private static void timer_Elapsed_rolling(object sender, ElapsedEventArgs e)
        {
            SRMMTConfig.MethodCallPerClient(timer_Elapset_rollingMT);
        }

        private static void timer_Elapset_rollingMT(string clientName)
        {
            try
            {
                var lstRollingInfo = DequeueRolling(clientName);

                Parallel.ForEach(lstRollingInfo, rollingInfo =>
                {
                    SRMDWHJob.RollData(clientName, rollingInfo);
                });
            }
            catch (Exception ee)
            {
                mLogger.Error("ERROR in Polling thread for DWH Rolling Queue : " + ee.ToString());
            }
        }
    }
}