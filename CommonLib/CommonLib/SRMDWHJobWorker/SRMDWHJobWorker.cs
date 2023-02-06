using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using com.ivp.common.srmdwhjob;
using System.Reflection;
using System.Data;
using com.ivp.rad.common;
using System.Xml;
using com.ivp.rad.configurationmanagement;
using System.IO;
using com.ivp.rad.data;
using com.ivp.rad.transport;
using com.ivp.rad.utils;
using System.Collections.Concurrent;
using com.ivp.rad.notificationmanager;
using com.ivp.rad.dal;
using Newtonsoft.Json;
using com.ivp.commom.commondal;
using System.Xml.Linq;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using com.ivp.srmcommon;
using com.ivp.common.srmdownstreamcontroller;
using System.Collections.Specialized;
using com.ivp.rad.customclass;
using DWHAdapter;
using com.ivp.rad.common.MessageFormatters;
using System.Collections;

namespace com.ivp.common.srmdwhjobworker
{
    public class SRMDWHJobWorker
    {
        static IRLogger mLogger;

        static object lockObject = new object();
        static Dictionary<string, string> EntityCodeVsMasterId = new Dictionary<string, string>();
        static Dictionary<string, string> SecurityIdVsMasterId = new Dictionary<string, string>();
        static HashSet<string> PurgedInstruments = null;
        static HashSet<string> DeletedInstruments = null;
        static Dictionary<string, Tuple<string, DateTime>> SecIdVsSecurityKeyAndCreatedOn = null;
        static Dictionary<int, HashSet<string>> AdapterVsMasterTableUnderProcessing = new Dictionary<int, HashSet<string>>();

        static ConcurrentDictionary<int, object> dctInstrumentTypeVsObject = new ConcurrentDictionary<int, object>();

        static bool requireTSIdInLegDailyTable;
        static int loaderBatchSize;
        static bool createMappingTableAndIntradayView;

        const string REFM_CONSTANT = "REFM";
        const string SECM_CONSTANT = "SECM";

        static void Main(string[] args)
        {
            string errorMessage = string.Empty;
            Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo = new Dictionary<string, Dictionary<int, SRMJobInfo>>();
            string setupName = string.Empty, cacheKey = string.Empty;
            BLOCK_TYPES blockType = BLOCK_TYPES.NTS_REPORT;
            bool isRefm = false, runLoaderInTransaction = false;
            List<SRMDWHBlockStatus> blockStatus = new List<SRMDWHBlockStatus>();

            HashSet<int> reportIdsForParallel = new HashSet<int>();
            HashSet<int> reportIdsForSequential = new HashSet<int>();
            Dictionary<int, List<SRMJobInfo>> TSReportIdVsDailyJobInfos = new Dictionary<int, List<SRMJobInfo>>();
            HashSet<int> reportIdsToBeMovedToSequential = new HashSet<int>();

            try
            {
                CultureInfo culture = null;
                culture = CultureInfo.CreateSpecificCulture("en-US");
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                if (args[0].Equals("REFM", StringComparison.OrdinalIgnoreCase))
                    isRefm = true;
                int executionInfoId = Convert.ToInt32(args[1]);
                blockType = (BLOCK_TYPES)Enum.Parse(typeof(BLOCK_TYPES), Convert.ToString(args[2]), true);
                runLoaderInTransaction = Convert.ToBoolean(args[3]);
                setupName = Convert.ToString(args[4]);
                cacheKey = Convert.ToString(args[5]);

                SetLogger(isRefm, blockType, setupName);

                string value = SRMCommon.GetConfigFromDB("DWHRequireTSIdInLegDailyTable");
                if (!string.IsNullOrEmpty(value) && value.Equals("true", StringComparison.OrdinalIgnoreCase))
                    requireTSIdInLegDailyTable = true;
                else
                    requireTSIdInLegDailyTable = false;

                value = SRMCommon.GetConfigFromDB("DWHLoaderBatchSize");
                if (!(!string.IsNullOrEmpty(value) && int.TryParse(value, out loaderBatchSize)))
                    loaderBatchSize = 0;

                value = SRMCommon.GetConfigFromDB("DWHCreateMappingTableAndIntradayView");
                if (!string.IsNullOrEmpty(value) && value.Equals("true", StringComparison.OrdinalIgnoreCase))
                    createMappingTableAndIntradayView = true;
                else
                    createMappingTableAndIntradayView = false;

                GetReportsIdsForSequentialAndParallelRun(ref reportIdVsInfo, isRefm, runLoaderInTransaction, executionInfoId, ref reportIdsForParallel, ref reportIdsForSequential);

                SRMJobInfo tempInfo = reportIdVsInfo.Values.FirstOrDefault().Select(x => x.Value).FirstOrDefault();
                DWHAdapterController.RegisterDWHAdapter(tempInfo.SetupId, false);

                if (runLoaderInTransaction)
                {
                    string module = isRefm ? REFM_CONSTANT : SECM_CONSTANT;
                    TSReportIdVsDailyJobInfos = reportIdVsInfo[module].Values.AsEnumerable().Where(x => x.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT)).GroupBy(x => x.TimeSeriesReportId).ToDictionary(x => x.Key, y => y.ToList());
                }

                if (isRefm && !runLoaderInTransaction)
                {
                    if (reportIdsForParallel.Count > 0)
                    {
                        Dictionary<int, List<int>> entityTypeVsReports;
                        List<HashSet<int>> parallelJobSets;
                        GetEntityTypeExecutionAsPerDependencyGraph(reportIdVsInfo, reportIdsForParallel, out entityTypeVsReports, out parallelJobSets);

                        foreach (var set in parallelJobSets)
                        {
                            HashSet<int> lst = new HashSet<int>();

                            foreach (var entityType in set)
                            {
                                if (entityTypeVsReports.ContainsKey(entityType))
                                    lst.UnionWith(entityTypeVsReports[entityType]);
                            }
                            if (lst.Count > 0)
                            {
                                blockStatus = PerformWorkerJobWrapper(reportIdVsInfo, isRefm, blockStatus, lst);

                            }
                        }
                    }
                }
                else if (!isRefm && !runLoaderInTransaction)
                {
                    blockStatus = PerformWorkerJobWrapper(reportIdVsInfo, isRefm, blockStatus, reportIdsForParallel);

                    blockStatus.AddRange(PerformWorkerJobWrapperSeq(reportIdVsInfo, isRefm, blockStatus, reportIdsForSequential));
                }
                else
                {
                    blockStatus = ParallelReportExecution(reportIdVsInfo, isRefm, blockStatus, reportIdsForParallel, TSReportIdVsDailyJobInfos);

                    blockStatus.AddRange(SequentialReportExecution(reportIdVsInfo, isRefm, blockStatus, reportIdsForSequential, TSReportIdVsDailyJobInfos));
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("Error in Worker Process");
                mLogger.Error(ex);
                errorMessage = ex.Message;
            }
            finally
            {
                if (reportIdVsInfo.Count == 0)
                {
                    SRMDWHInternalInfo inputInfo = new SRMDWHInternalInfo()
                    {
                        BlockExecuted = string.Empty,
                        BlockStatus = new List<SRMDWHBlockStatus>(),
                        CacheKey = cacheKey,
                        ErrorMessage = errorMessage,
                        InTransaction = false,
                        SetupId = 0,
                        SetupStatusId = 0,
                        UserName = string.Empty,
                        SetupName = setupName
                    };
                    CallAPItoSendResponse(inputInfo);
                }
                else
                {
                    SRMJobInfo info = reportIdVsInfo.Values.FirstOrDefault().Select(x => x.Value).FirstOrDefault();
                    SendWorkerResponse(info.UserName, info.SetupId, info.SetupStatusId, blockType, isRefm, cacheKey, runLoaderInTransaction, blockStatus, errorMessage);
                }
            }
        }

        private static List<SRMDWHBlockStatus> SequentialReportExecution(Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo, bool isRefm, List<SRMDWHBlockStatus> blockStatus, HashSet<int> reportIdsForSequential, Dictionary<int, List<SRMJobInfo>> TSReportIdVsDailyJobInfos)
        {
            //TS/Daily Set
            //Case when TS does not have any data and daily has data.
            string module = isRefm ? REFM_CONSTANT : SECM_CONSTANT;
            HashSet<int> tsReportIds = TSReportIdVsDailyJobInfos.Keys.Intersect(reportIdsForSequential).ToHashSet();
            foreach (var reportId in reportIdsForSequential)
            {
                if (reportIdVsInfo[module][reportId].BlockType.Equals(BLOCK_TYPES.DAILY_REPORT) &&
                    !tsReportIds.Contains(reportIdVsInfo[module][reportId].TimeSeriesReportId))
                    tsReportIds.Add(reportIdVsInfo[module][reportId].TimeSeriesReportId);
            }
            if (reportIdsForSequential.Count > 0)
            {
                if (isRefm)
                {
                    Dictionary<int, List<int>> entityTypeVsReports;
                    List<HashSet<int>> parallelJobSets;
                    GetEntityTypeExecutionAsPerDependencyGraph(reportIdVsInfo, tsReportIds, out entityTypeVsReports, out parallelJobSets);

                    foreach (var set in parallelJobSets)
                    {
                        HashSet<int> lst = new HashSet<int>();

                        foreach (var entityType in set)
                        {
                            if (entityTypeVsReports.ContainsKey(entityType))
                                lst.UnionWith(entityTypeVsReports[entityType]);
                        }
                        if (lst.Count > 0)
                        {
                            blockStatus = PerformWorkerJobInTransactionInSequence(reportIdVsInfo, isRefm, blockStatus, TSReportIdVsDailyJobInfos, lst);

                        }
                    }
                }
                else
                    blockStatus = PerformWorkerJobInTransactionInSequence(reportIdVsInfo, isRefm, blockStatus, TSReportIdVsDailyJobInfos, tsReportIds);
            }

            return blockStatus;
        }

        private static List<SRMDWHBlockStatus> ParallelReportExecution(Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo, bool isRefm, List<SRMDWHBlockStatus> blockStatus, HashSet<int> reportIdsForParallel, Dictionary<int, List<SRMJobInfo>> TSReportIdVsDailyJobInfos)
        {
            //TS/Daily Set
            //Case when TS does not have any data and daily has data.
            string module = isRefm ? REFM_CONSTANT : SECM_CONSTANT;
            HashSet<int> tsReportIds = TSReportIdVsDailyJobInfos.Keys.Intersect(reportIdsForParallel).ToHashSet();
            foreach (var reportId in reportIdsForParallel)
            {
                if (reportIdVsInfo[module][reportId].BlockType.Equals(BLOCK_TYPES.DAILY_REPORT) &&
                    !tsReportIds.Contains(reportIdVsInfo[module][reportId].TimeSeriesReportId))
                    tsReportIds.Add(reportIdVsInfo[module][reportId].TimeSeriesReportId);
            }

            if (tsReportIds.Count > 0)
            {
                if (isRefm)
                {
                    Dictionary<int, List<int>> entityTypeVsReports;
                    List<HashSet<int>> parallelJobSets;
                    GetEntityTypeExecutionAsPerDependencyGraph(reportIdVsInfo, tsReportIds, out entityTypeVsReports, out parallelJobSets);

                    foreach (var set in parallelJobSets)
                    {
                        HashSet<int> lst = new HashSet<int>();

                        foreach (var entityType in set)
                        {
                            if (entityTypeVsReports.ContainsKey(entityType))
                                lst.UnionWith(entityTypeVsReports[entityType]);
                        }
                        if (lst.Count > 0)
                        {
                            blockStatus = PerformWorkerJobInTransactionInParallel(reportIdVsInfo, isRefm, blockStatus, TSReportIdVsDailyJobInfos, lst);

                        }
                    }
                }
                else
                    blockStatus = PerformWorkerJobInTransactionInParallel(reportIdVsInfo, isRefm, blockStatus, TSReportIdVsDailyJobInfos, tsReportIds);
            }

            return blockStatus;
        }

        private static void GetReportsIdsForSequentialAndParallelRun(ref Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo, bool isRefm, bool runLoaderInTransaction, int executionInfoId, ref HashSet<int> reportIdsForParallel, ref HashSet<int> reportIdsForSequential)
        {
            DataSet executionInfoDS = CommonDALWrapper.ExecuteSelectQuery("SELECT * FROM [dbo].[ivp_srm_dwh_downstream_execution_info] WHERE id = " + executionInfoId, ConnectionConstants.RefMaster_Connection);

            if (executionInfoDS != null && executionInfoDS.Tables.Count > 0 && executionInfoDS.Tables[0].Rows.Count > 0)
            {
                reportIdVsInfo = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, SRMJobInfo>>>(Convert.ToString(executionInfoDS.Tables[0].Rows[0]["report_info"]));
                if (isRefm)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(executionInfoDS.Tables[0].Rows[0]["ref_report_id_parallel"])))
                        reportIdsForParallel = Convert.ToString(executionInfoDS.Tables[0].Rows[0]["ref_report_id_parallel"]).Split(',').Select(Int32.Parse).ToHashSet();

                    if (!string.IsNullOrEmpty(Convert.ToString(executionInfoDS.Tables[0].Rows[0]["ref_report_id_sequential"])))
                        reportIdsForSequential = Convert.ToString(executionInfoDS.Tables[0].Rows[0]["ref_report_id_sequential"]).Split(',').Select(Int32.Parse).ToHashSet();
                }
                else
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(executionInfoDS.Tables[0].Rows[0]["sec_report_id_parallel"])))
                        reportIdsForParallel = Convert.ToString(executionInfoDS.Tables[0].Rows[0]["sec_report_id_parallel"]).Split(',').Select(Int32.Parse).ToHashSet();

                    if (!string.IsNullOrEmpty(Convert.ToString(executionInfoDS.Tables[0].Rows[0]["sec_report_id_sequential"])))
                        reportIdsForSequential = Convert.ToString(executionInfoDS.Tables[0].Rows[0]["sec_report_id_sequential"]).Split(',').Select(Int32.Parse).ToHashSet();
                }
                if (isRefm && !runLoaderInTransaction)
                {
                    reportIdsForParallel.UnionWith(reportIdsForSequential);
                    reportIdsForSequential = new HashSet<int>();
                }
            }
        }

        private static List<SRMDWHBlockStatus> PerformWorkerJobInTransactionInSequence(Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo, bool isRefm, List<SRMDWHBlockStatus> blockStatus, Dictionary<int, List<SRMJobInfo>> TSReportIdVsDailyJobInfos, HashSet<int> tsReportIds)
        {
            foreach (int reportId in tsReportIds)
            {
                SRMJobInfo tsReportJob;
                if (isRefm)
                    tsReportJob = reportIdVsInfo[REFM_CONSTANT][reportId];
                else
                    tsReportJob = reportIdVsInfo[SECM_CONSTANT][reportId];

                PerformWorkerJobInTransaction(tsReportJob, TSReportIdVsDailyJobInfos[reportId], ref blockStatus);
            }

            return blockStatus;
        }

        private static List<SRMDWHBlockStatus> PerformWorkerJobInTransactionInParallel(Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo, bool isRefm, List<SRMDWHBlockStatus> blockStatus, Dictionary<int, List<SRMJobInfo>> TSReportIdVsDailyJobInfos, HashSet<int> tsReportIds)
        {
            ConcurrentQueue<int> reportQueue = new ConcurrentQueue<int>(tsReportIds);
            int[] threadsToProcess = new int[Environment.ProcessorCount];
            Parallel.ForEach(threadsToProcess, (i) =>
            {
                do
                {
                    int reportId;
                    if (reportQueue.TryDequeue(out reportId))
                    {
                        SRMJobInfo tsReportJob;
                        if (isRefm)
                            tsReportJob = reportIdVsInfo[REFM_CONSTANT][reportId];
                        else
                            tsReportJob = reportIdVsInfo[SECM_CONSTANT][reportId];

                        PerformWorkerJobInTransaction(tsReportJob, TSReportIdVsDailyJobInfos[reportId], ref blockStatus);
                    }
                }
                while (reportQueue.Count > 0);
            });
            return blockStatus;
        }

        private static void GetEntityTypeExecutionAsPerDependencyGraph(Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo, HashSet<int> reportIdsForParallel, out Dictionary<int, List<int>> entityTypeVsReports, out List<HashSet<int>> parallelJobSets)
        {
            ObjectSet ds = new ObjectSet();
            ds.Tables.Add(new ObjectTable());
            ds.Tables[0].Columns.Add(new ObjectColumn("report_id", typeof(System.Int32)));
            foreach (int report_id in reportIdsForParallel)
            {
                ObjectRow dr = ds.Tables[0].NewRow();
                dr[0] = report_id;
                ds.Tables[0].Rows.Add(dr);
            }

            string tableName = "[dbo].[dwh_" + Guid.NewGuid().ToString() + "]";
            string downstreamConnectionName = reportIdVsInfo[REFM_CONSTANT][reportIdsForParallel.First()].DownstreamSQLConnectionName;
            string sqlQuery = string.Format("CREATE TABLE {0}(report_id INT)", tableName);

            CommonDALWrapper.ExecuteSelectQueryObject(sqlQuery, ConnectionConstants.RefMaster_Connection);
            CommonDALWrapper.ExecuteBulkUploadObject(tableName, ds.Tables[0], ConnectionConstants.RefMaster_Connection);

            sqlQuery = string.Format(@"SELECT rep.report_id,  emap.dependent_id , rep.report_name
                                            FROM IVPRefMaster.dbo.ivp_refm_report rep
                                            INNER JOIN {0} inprep 
                                            ON (rep.report_id = inprep.report_id)
                                            INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                                            ON (emap.report_id = rep.report_id)
                                            WHERE emap.is_active = 1

                                            SELECT DISTINCT eal.child_entity_type_id, eal.parent_entity_type_id 
                                            FROM IVPRefMaster.dbo.ivp_refm_report rep
                                            INNER JOIN {0} inprep 
                                            ON (rep.report_id = inprep.report_id)
                                            INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute ra
                                            ON (ra.report_id = inprep.report_id)
                                            INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute_map ramap
                                            ON (ramap.report_attribute_id = ra.report_attribute_id AND ramap.report_id = ra.report_id)
                                            INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat
                                            ON (ramap.dependent_attribute_id = eat.entity_attribute_id)
                                            INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute_lookup eal
                                            ON (eal.child_entity_attribute_id = eat.entity_attribute_id)
                                            WHERE rep.is_active = 1 AND ra.is_active = 1 AND ramap.is_active = 1 AND eat.is_active = 1 AND eal.is_active = 1
                                            AND ramap.lookup_attribute_id IS NULL AND ra.attribute_data_type <> 'LOOKUP'

                                            DROP TABLE {0}", tableName);

            ds = CommonDALWrapper.ExecuteSelectQueryObject(sqlQuery, ConnectionConstants.RefMaster_Connection);
            Dictionary<int, List<int>> entityTypeVsLookupTypes = new Dictionary<int, List<int>>();
            entityTypeVsReports = new Dictionary<int, List<int>>();
            Dictionary<int, List<string>> entityTypeVsReportNames = new Dictionary<int, List<string>>();

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                entityTypeVsReports = ds.Tables[0].Rows.AsEnumerable().GroupBy(x => Convert.ToInt32(x["dependent_id"])).ToDictionary(x => x.Key, y => y.Select(z => Convert.ToInt32(z["report_id"])).ToList());
                entityTypeVsReportNames = ds.Tables[0].Rows.AsEnumerable().GroupBy(x => Convert.ToInt32(x["dependent_id"])).ToDictionary(x => x.Key, y => y.Select(z => Convert.ToString(z["report_name"])).ToList());
            }

            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                entityTypeVsLookupTypes = ds.Tables[1].Rows.AsEnumerable().GroupBy(x => Convert.ToInt32(x["child_entity_type_id"])).ToDictionary(x => x.Key, y => y.Select(z => Convert.ToInt32(z["parent_entity_type_id"])).ToList());
            }

            var noLookupEntityTypes = entityTypeVsReports.Keys.Except(entityTypeVsLookupTypes.Keys);
            foreach (var entityType in noLookupEntityTypes)
                entityTypeVsLookupTypes.Add(entityType, new List<int>());

            List<int> cyclicEntityTypes = null;
            parallelJobSets = GetEntityTypeExecutionOrder(entityTypeVsLookupTypes, out cyclicEntityTypes);
            if (cyclicEntityTypes.Count > 0)
            {
                HashSet<string> reportNames = new HashSet<string>();
                foreach (var et in cyclicEntityTypes)
                    if (entityTypeVsReportNames.ContainsKey(et))
                        reportNames.UnionWith(entityTypeVsReportNames[et]);

                throw new Exception("Cyclic dependencies exists for reports : " + string.Join(",", reportNames));
            }
        }

        private static List<SRMDWHBlockStatus> PerformWorkerJobWrapper(Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo, bool isRefm, List<SRMDWHBlockStatus> blockStatus, HashSet<int> lst)
        {
            ConcurrentQueue<int> reportQueue = new ConcurrentQueue<int>(lst.ToList());
            int[] threadsToProcess = new int[Environment.ProcessorCount];
            Parallel.ForEach(threadsToProcess, (i) =>
            {
                do
                {
                    int reportId;
                    if (reportQueue.TryDequeue(out reportId))
                    {
                        SRMJobInfo job = null;
                        if (isRefm)
                            job = reportIdVsInfo[REFM_CONSTANT][reportId];
                        else
                            job = reportIdVsInfo[SECM_CONSTANT][reportId];

                        PerformWorkerJob(job, ref blockStatus);
                    }
                }
                while (reportQueue.Count > 0);
            });
            return blockStatus;
        }

        private static List<SRMDWHBlockStatus> PerformWorkerJobWrapperSeq(Dictionary<string, Dictionary<int, SRMJobInfo>> reportIdVsInfo, bool isRefm, List<SRMDWHBlockStatus> blockStatus, HashSet<int> lst)
        {
            foreach (int reportId in lst)
            {
                SRMJobInfo job = null;
                if (isRefm)
                    job = reportIdVsInfo[REFM_CONSTANT][reportId];
                else
                    job = reportIdVsInfo[SECM_CONSTANT][reportId];

                PerformWorkerJob(job, ref blockStatus);
            }

            return blockStatus;
        }

        private static void SendWorkerResponse(string userName, int setupId, int setupStatusId, BLOCK_TYPES blockType, bool isRefm, string cacheKey, bool inTransaction, List<SRMDWHBlockStatus> blockStatus, string errorMessage)
        {

            if (Environment.ExitCode == 1 && string.IsNullOrEmpty(errorMessage))
                errorMessage = "Sync Failed";

            string blockExecuted = REFM_CONSTANT + ":nts";

            if (blockType.Equals(BLOCK_TYPES.NTS_REPORT) && !isRefm)
            {
                blockExecuted = SECM_CONSTANT + ":nts";
                inTransaction = true;
            }

            else if (blockType.Equals(BLOCK_TYPES.TS_REPORT) && isRefm && inTransaction)
            {
                blockExecuted = SECM_CONSTANT + ":nts";
                inTransaction = false;
            }
            else if (blockType.Equals(BLOCK_TYPES.TS_REPORT) && isRefm && !inTransaction)
            {
                blockExecuted = REFM_CONSTANT + ":ts";
                inTransaction = true;
            }
            else if (blockType.Equals(BLOCK_TYPES.TS_REPORT) && !isRefm && inTransaction)
            {
                blockExecuted = REFM_CONSTANT + ":ts";
                inTransaction = false;
            }
            else if (blockType.Equals(BLOCK_TYPES.TS_REPORT) && !isRefm && !inTransaction)
            {
                blockExecuted = SECM_CONSTANT + ":ts";
                inTransaction = false;
            }
            else if (blockType.Equals(BLOCK_TYPES.DAILY_REPORT) && isRefm)
            {
                blockExecuted = REFM_CONSTANT + ":daily";
                inTransaction = false;
            }
            else if (blockType.Equals(BLOCK_TYPES.DAILY_REPORT) && !isRefm)
            {
                blockExecuted = SECM_CONSTANT + ":daily";
                inTransaction = false;
            }

            SRMDWHInternalInfo inputInfo = new SRMDWHInternalInfo()
            {
                BlockExecuted = blockExecuted,
                CacheKey = cacheKey,
                ErrorMessage = errorMessage,
                InTransaction = inTransaction,
                SetupId = setupId,
                SetupStatusId = setupStatusId,
                UserName = userName,
                BlockStatus = blockStatus,
                SetupName = string.Empty
            };

            CallAPItoSendResponse(inputInfo);
        }

        private static void CallAPItoSendResponse(SRMDWHInternalInfo inputInfo)
        {
            mLogger.Debug("Send Worker Response Object : " + JsonConvert.SerializeObject(inputInfo));

            string exeConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SRMDWHService.exe.config");
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(exeConfigPath);
            string url = xDoc.GetElementsByTagName("baseAddresses")[0]["add"].GetAttribute("baseAddress");

            var serObject = new DataContractJsonSerializer(typeof(SRMDWHInternalInfo));
            var memStream = new MemoryStream();
            serObject.WriteObject(memStream, inputInfo);
            memStream.Position = 0;
            byte[] requestBodyBytes = new UTF8Encoding().GetBytes(new StreamReader(memStream).ReadToEnd());
            SRMCommon.InvokeRestAPI(url, new Dictionary<string, string>(), requestBodyBytes, "GetWorkerResponse", "POST");
        }

        private static void RenameNTNStagingToDimensionTable(SRMJobInfo job, bool requireMappingTableRename, RDBConnectionManager con, DataSet tsTablesToRename)
        {
            string finalTableNameOriginal = job.TableName.Replace("_staging]", "]");

            string query;
            if (job.BlockType == BLOCK_TYPES.TS_REPORT && job.RequireTimeTSReports && requireMappingTableRename && createMappingTableAndIntradayView)
            {
                query = string.Format(@"
                                    DECLARE @query VARCHAR(MAX) = ''
                                    DECLARE @stagingTableName VARCHAR(1000) = PARSENAME('{0}',1) + '_mapping'
                                    DECLARE @originalTableName VARCHAR(1000) = REPLACE(@stagingTableName,'_staging','')
                                    SELECT @stagingTableName = PARSENAME('{0}',2) + '.' + PARSENAME('{0}',1) + '_mapping'

				                    SELECT @query = 
				                    'IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '''+ @originalTableName +''' AND TABLE_SCHEMA = ''dimension'')
				                    BEGIN
						                    DROP TABLE dimension.[' + @originalTableName + ']
				                    END'
				                    EXEC(@query)
				                    EXEC sp_rename @stagingTableName , @originalTableName", job.TableName);
                if (con == null)
                    SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                else
                    SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.SELECT);

            }
            if (job.BlockType == BLOCK_TYPES.TS_REPORT)
            {
                query = @"DECLARE @query VARCHAR(MAX) = ''
                        DECLARE @stagingTableName VARCHAR(1000) = ''
                        DECLARE @originalTableName VARCHAR(1000) = '' ";

                foreach (DataRow dr in tsTablesToRename.Tables[0].Rows)
                {
                    string tempTableName = Convert.ToString(dr["dimension_table_name"]);
                    finalTableNameOriginal = tempTableName.Replace("_NTN]", "]");

                    query += String.Format(@"
                                    SELECT @query = ''
                                    SELECT @stagingTableName = '{0}'
                                    SELECT @originalTableName = PARSENAME('{1}',1)        

				                    SELECT @query = 
				                    'IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '''+ @originalTableName +''' AND TABLE_SCHEMA = ''dimension'')
				                    BEGIN
						                    DROP TABLE dimension.[' + @originalTableName + ']
				                    END'
				                    EXEC(@query)
				                    EXEC sp_rename @stagingTableName , @originalTableName", tempTableName, finalTableNameOriginal);
                }
            }
            else
                query = string.Format(@"
                                    DECLARE @query VARCHAR(MAX) = ''
                                    DECLARE @stagingTableName VARCHAR(1000) = '{0}'
                                    DECLARE @originalTableName VARCHAR(1000) = PARSENAME('{1}',1)                                    
				                    SELECT @query = 
				                    'IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '''+ @originalTableName +''' AND TABLE_SCHEMA = ''dimension'')
				                    BEGIN
						                    DROP TABLE dimension.[' + @originalTableName + ']
				                    END'
				                    EXEC(@query)
				                    EXEC sp_rename @stagingTableName , @originalTableName", job.TableName, finalTableNameOriginal);
            if (con == null)
                SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
            else
                SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.SELECT);
        }

        private static void ExecuteDailyNTNLoader(SRMJobInfo dailyJob)
        {
            string query;
            string indexAppendName = string.Empty;
            string surogateColumnName = dailyJob.TableName.Split('.')[1].Replace("[ivp_polaris_", "").Replace("_daily_staging]", "") + "_daily_surrogate_id";

            //Drop existing _bakforNTN table
            query = string.Format(@"SELECT NEWID()");
            DataSet ds = SRMDWHJobExtension.ExecuteQuery(dailyJob.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
            indexAppendName = Convert.ToString(ds.Tables[0].Rows[0][0]).Replace('-', '_');

            if (!dailyJob.IsLegReport)
            {
                query = string.Format(@"ALTER TABLE {0} ADD CONSTRAINT pk_{1} PRIMARY KEY NONCLUSTERED ({2})", dailyJob.TableName, indexAppendName, surogateColumnName);
                query = query + string.Format(@" CREATE NONCLUSTERED INDEX idx_{0} ON {1} (master_id)", indexAppendName, dailyJob.TableName);
                query = query + string.Format(@" CREATE UNIQUE NONCLUSTERED INDEX idx_1_{0} ON {1} ([Effective Date],master_id)", indexAppendName, dailyJob.TableName);
            }
            else if (dailyJob.IsLegReport)
            {
                query = string.Format(@"CREATE UNIQUE NONCLUSTERED INDEX idx_{0} ON {1} ({2}, [Row Id])", indexAppendName, dailyJob.TableName, surogateColumnName);
                query = query + string.Format(@" CREATE NONCLUSTERED INDEX idx_1_{0} ON {1} ([Effective Date],master_id)", indexAppendName, dailyJob.TableName);
                query = query + string.Format(@" CREATE NONCLUSTERED INDEX idx_2_{0} ON {1} ([Effective Date],is_leg_deleted)", indexAppendName, dailyJob.TableName);
            }
            SRMDWHJobExtension.ExecuteQuery(dailyJob.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
        }

        private static void ExecuteDWHAdapters(SRMJobInfo tsReportJob, List<SRMJobInfo> dailyReportJobs, HashSet<string> TSTableNames)
        {
            string tsTableName = tsReportJob.TableName.Replace("[taskmanager]", "[dimension]").Replace("[references]", "[dimension]").Replace("_staging]", "]");
            FailedSyncMetadata? failedResponse;
            foreach (var info in SRMDWHStatic.dctSetupIdVsDWHAdapterDetails)
            {
                foreach (var adapterInfo in info.Value)
                {
                    int counter = 0;
                    TableSyncMetadata[] tSMetadata = new TableSyncMetadata[0];

                    if (TSTableNames.Count > 0)
                    {
                        tSMetadata = new TableSyncMetadata[TSTableNames.Count];
                        counter = 0;
                        foreach (string tableName in TSTableNames)
                        {
                            bool isLegTable = false;
                            if (tableName.Contains("_leg_"))
                                isLegTable = true;
                            tSMetadata[counter] = new TableSyncMetadata(isLegTable, tsReportJob.DateType == DWHDateType.None, tsReportJob.LoadingTimeForStagingTable, tableName);
                            counter++;
                        }
                    }
                    else
                    {
                        string legText = "securities_leg_";
                        if (tsReportJob.IsRef)
                            legText = tsTableName.Replace("[dimension].[ivp_polaris_", "").Replace("_time_series]", "_leg_");
                        string query = string.Format(@"SELECT DISTINCT identifier_key FROM taskmanager.ivp_polaris_core_secref_extract_table_column_mapping 
                                    WHERE identifier_key like '%{0}%'", legText);
                        ObjectSet os = SRMDWHJobExtension.ExecuteQueryObject(tsReportJob.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                        if (os != null && os.Tables.Count > 0 && os.Tables[0].Rows.Count > 0)
                        {
                            ObjectTable ot = os.Tables[0];
                            tSMetadata = new TableSyncMetadata[ot.Rows.Count + 1];
                            counter = 0;

                            foreach (ObjectRow or in ot.Rows)
                            {
                                string legTableName = Convert.ToString(or[0]);
                                tSMetadata[counter] = new TableSyncMetadata(true, tsReportJob.DateType == DWHDateType.None, tsReportJob.LoadingTimeForStagingTable, legTableName);
                                counter++;
                            }
                            tSMetadata[counter] = new TableSyncMetadata(false, isNoneToNow: tsReportJob.DateType == DWHDateType.None, tsReportJob.LoadingTimeForStagingTable, tsTableName);
                        }
                        else
                        {
                            tSMetadata = new TableSyncMetadata[1];
                            tSMetadata[0] = new TableSyncMetadata(false, tsReportJob.DateType == DWHDateType.None, tsReportJob.LoadingTimeForStagingTable, tsTableName);
                        }
                    }

                    TableSyncMetadata[] dailyMetadata = new TableSyncMetadata[dailyReportJobs.Count];
                    counter = 0;
                    foreach (SRMJobInfo dailyjob in dailyReportJobs)
                    {
                        string dailyTableName = dailyjob.TableName.Replace("[taskmanager]", "[dimension]").Replace("[references]", "[dimension]").Replace("_staging]", "]");
                        dailyMetadata[counter] = new TableSyncMetadata(dailyjob.IsLegReport, dailyjob.DateType == DWHDateType.None, dailyjob.LoadingTimeForStagingTable, dailyTableName);
                        counter++;
                    }

                    TSAndDailyMetadata metadata = new TSAndDailyMetadata(dailyMetadata, tSMetadata);
                    failedResponse = DWHAdapterController.SyncDailyTableData(adapterInfo.AdapterId, tsReportJob.DownstreamSQLConnectionName, metadata, null);

                    if (failedResponse.HasValue)
                    {
                        mLogger.Error(adapterInfo.AdapterName + " - Exception : " + failedResponse.Value.ErrorMessage);
                        throw new Exception(adapterInfo.AdapterName + " - Exception : " + failedResponse.Value.ErrorMessage);
                    }
                }
            }
        }

        private static void InsertUpdateStatusForBlockInObject(int blockStatusId, EXECUTION_STATUS status, string failureMessage, DateTime endTime, int blockId, ref List<SRMDWHBlockStatus> blockStatus)
        {
            if (blockStatusId == 0)
                return;
            mLogger.Debug("Adding InsertUpdateStatusForBlockInObject for blockStatusId : " + blockStatusId + " AND blockId : " + blockId);
            if (failureMessage.Length > 0)
            {
                int maxLength = failureMessage.Length - 1;
                if (maxLength > 8000)
                {
                    maxLength = 8000;
                    failureMessage = failureMessage.Substring(0, maxLength);
                }
                failureMessage = failureMessage.Replace("'", "''");
            }
            lock (lockObject)
            {
                blockStatus.Add(new SRMDWHBlockStatus() { BlockStatusId = blockStatusId, status = status, ErrorMessage = failureMessage, EndTime = endTime, BlockId = blockId });
            }
        }

        private static void InsertUpdateStatusForBlock(int statusId, EXECUTION_STATUS status)
        {
            DateTime now = DateTime.Now;
            string query = "UPDATE [dbo].[ivp_srm_dwh_downstream_block_status] SET status = '" + status.GetDescription() + "' WHERE block_status_id = " + statusId + " AND status != 'FAILED'";
            if (status == EXECUTION_STATUS.FAILED)
                query = "UPDATE [dbo].[ivp_srm_dwh_downstream_block_status] SET end_time = '" + now.ToString("yyyyMMdd HH:mm:ss.fff") + "', status = '" + status.GetDescription() + "' WHERE block_status_id = " + statusId + " AND status != 'FAILED'";

            CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Update, ConnectionConstants.RefMaster_Connection);

        }

        private static void ProcessLoaderInTransaction(SRMJobInfo tsReportJob, List<SRMJobInfo> dailyReportJobs, HashSet<string> instrumentIds, List<int> blockIdsWithData, bool isLastBatchForDailyLoader,
            HashSet<string> currentBatchTSInstruments)
        {
            mLogger.Debug("ProcessLoaderInTransaction --> Start -- " + tsReportJob.TableName);

            string typeName = tsReportJob.IsRef ? "[Entity Type Name]" : "[Security Type Name]";
            string typeId = tsReportJob.IsRef ? "[entity_code]" : "[Security Id]";

            RDBConnectionManager con = null;

            List<string> tablesToBeDropped = new List<string>();
            try
            {
                ObjectSet ds = new ObjectSet();
                ds.Tables.Add(new ObjectTable());
                ds.Tables[0].Columns.Add(new ObjectColumn("instrument_id", typeof(System.String)));
                foreach (string instrumentId in instrumentIds)
                {
                    ObjectRow dr = ds.Tables[0].NewRow();
                    dr[0] = instrumentId;
                    ds.Tables[0].Rows.Add(dr);
                }
                string tableName = "[taskmanager].[" + Guid.NewGuid().ToString() + "]";
                tablesToBeDropped.Add(tableName);
                SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, string.Format("CREATE TABLE {0}(instrument_id VARCHAR(50)) CREATE NONCLUSTERED INDEX idx_1 ON {0}(instrument_id)", tableName), SRMDBQueryType.SELECT);
                SRMDWHJobExtension.ExecuteBulkCopyObject(tsReportJob.DownstreamSQLConnectionName, tableName, ds.Tables[0], new Dictionary<string, string> { { "instrument_id", "instrument_id" } });
                string query = string.Empty;

                con = RDALAbstractFactory.DBFactory.GetConnectionManager(tsReportJob.DownstreamSQLConnectionName);
                con.IsolationLevel = IsolationLevel.RepeatableRead;
                con.UseTransaction = true;


                if (blockIdsWithData.Contains(tsReportJob.BlockId))
                {
                    query = string.Format(@"DECLARE @sql VARCHAR(MAX) = ''
                                SELECT @sql += CASE WHEN @sql <> '' THEN ',' ELSE '' END + '[' + COLUMN_NAME + ']'
                                FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = PARSENAME('{3}',1) AND TABLE_SCHEMA = '{1}' AND COLUMN_NAME != 'loading_time'

                                IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('{0}',1) AND TABLE_SCHEMA = PARSENAME('{0}',2))
                                BEGIN
                                    DROP TABLE {0} 
                                END

                                DECLARE @sql2 VARCHAR(MAX) = ''
                                SELECT @sql2 += 
	                                CASE WHEN @sql2 <> '' THEN ',' ELSE '' END +
		                                '[' + COLUMN_NAME + '] ' + DATA_TYPE + 
		                                CASE WHEN DATA_TYPE = 'VARCHAR' AND CHARACTER_MAXIMUM_LENGTH != -1
				                                THEN ' (' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
			                                WHEN DATA_TYPE = 'VARCHAR'
				                                THEN ' (MAX)'
			                                WHEN DATA_TYPE IN ('NUMERIC','DECIMAL')
				                                THEN ' (' + CAST(NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(NUMERIC_SCALE AS VARCHAR)+ ')'
		                                ELSE '' 
		                                END
                                FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = PARSENAME('{3}',1) AND TABLE_SCHEMA = '{1}' ORDER BY ORDINAL_POSITION

                                SELECT @sql2 = 'CREATE TABLE {0} (' + @sql2 + ')'
                                EXEC (@sql2)

                                SELECT @sql = 'INSERT INTO {2} WITH (TABLOCKX) (' + @sql + ' ,[loading_time]) 
                                SELECT '+ @sql +',''{6}'' FROM {3} tab1
                                INNER JOIN {4} tab2 ON (tab1.{5} = tab2.instrument_id)'
                                EXEC(@sql)", tsReportJob.TableName, tsReportJob.IsRef ? "references" : "taskmanager", tsReportJob.TableName,
                                tsReportJob.BackupTableName, tableName, tsReportJob.IsRef ? "entity_code" : "[Security Id]", tsReportJob.LoadingTimeForStagingTable.ToString("yyyyMMdd HH:mm:ss.fff"));

                    SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);

                    StringBuilder sb1 = new StringBuilder();
                    sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_1 ON " + tsReportJob.TableName + "(" + typeName + ")");
                    sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_2 ON " + tsReportJob.TableName + "(" + typeId + ")");
                    sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_3 ON " + tsReportJob.TableName + "([isTimeSeriesRecord])");
                    sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_5 ON " + tsReportJob.TableName + "(" + typeId + ",[Effective Start Date])");
                    sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_6 ON " + tsReportJob.TableName + "(" + typeId + ",[Effective End Date])");
                    if (!tsReportJob.IsRef)
                        sb1.AppendLine("CREATE NONCLUSTERED INDEX idex_7 ON " + tsReportJob.TableName + "([Security Key])");

                    SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);

                    mLogger.Debug("Substep --> ExecuteLoader TS for table : " + tsReportJob.TableName + " started at : " + DateTime.Now);
                    new SRMDWHTSLoader().Begin(tsReportJob, con, tablesToBeDropped);
                    mLogger.Debug("Substep --> ExecuteLoader TS for table : " + tsReportJob.TableName + " completed at : " + DateTime.Now);
                }

                foreach (var dailyJob in dailyReportJobs)
                {
                    string surogateColumnName = dailyJob.TableName.Split('.')[1].Replace("[ivp_polaris_", "").Replace("_daily_staging]", "") + "_daily_surrogate_id";

                    if (dailyJob.ModifiedEntities != null && dailyJob.ModifiedEntities.Count > 0)
                    {
                        if (blockIdsWithData.Contains(dailyJob.BlockId))
                        {
                            query = string.Format(@"DECLARE @sql VARCHAR(MAX) = ''
                                SELECT @sql += CASE WHEN @sql <> '' THEN ',' ELSE '' END + '[' + COLUMN_NAME + ']'
                                FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = PARSENAME('{3}',1) AND TABLE_SCHEMA = '{1}' AND COLUMN_NAME != 'loading_time'
                    
                                IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('{0}',1) AND TABLE_SCHEMA = PARSENAME('{0}',2))
                                BEGIN
                                    DROP TABLE {0} 
                                END

                                DECLARE @sql2 VARCHAR(MAX) = ''
                                SELECT @sql2 += 
	                                CASE WHEN @sql2 <> '' THEN ',' ELSE '' END +
		                                '[' + COLUMN_NAME + '] ' + DATA_TYPE + 
		                                CASE WHEN DATA_TYPE = 'VARCHAR' AND CHARACTER_MAXIMUM_LENGTH != -1
				                                THEN ' (' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
			                                WHEN DATA_TYPE = 'VARCHAR'
				                                THEN ' (MAX)'
			                                WHEN DATA_TYPE IN ('NUMERIC','DECIMAL')
				                                THEN ' (' + CAST(NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(NUMERIC_SCALE AS VARCHAR)+ ')'
		                                ELSE '' 
		                                END + 
		                                CASE WHEN COLUMN_NAME IN ({7}) THEN ' NOT NULL ' ELSE '' END
                                FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = PARSENAME('{3}',1) AND TABLE_SCHEMA = '{1}' ORDER BY ORDINAL_POSITION

                                SELECT @sql2 = 'CREATE TABLE {0} (' + @sql2 + ')'
                                EXEC (@sql2)                                

                                SELECT @sql = '
                                INSERT INTO {2} WITH (TABLOCKX) (' + @sql + ' ,[loading_time]) 
                                SELECT '+ @sql +',''{6}'' FROM {3} tab1
                                INNER JOIN {4} tab2 ON (tab1.{5} = tab2.instrument_id)'
                                EXEC(@sql)", dailyJob.TableName, dailyJob.IsRef ? "references" : "taskmanager", dailyJob.TableName, dailyJob.BackupTableName, tableName, dailyJob.IsRef ? "entity_code" : "[Security Id]", tsReportJob.LoadingTimeForStagingTable.ToString("yyyyMMdd HH:mm:ss.fff"), "'" + surogateColumnName + "'");

                            SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);

                            StringBuilder sb1 = new StringBuilder();
                            sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_2 ON " + dailyJob.TableName + "(" + typeId + ")");

                            if (dailyJob.IsLegReport)
                                sb1.AppendLine("CREATE UNIQUE NONCLUSTERED INDEX idx_4 ON " + dailyJob.TableName + "([" + surogateColumnName + "],[Row Id])");

                            SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);

                            if (!dailyJob.IsLegReport)
                            {
                                sb1 = new StringBuilder();
                                sb1.AppendLine("ALTER TABLE " + dailyJob.TableName + " ADD CONSTRAINT pk_stg_" + surogateColumnName + "_" + DateTime.Now.ToString("yyyyMMdd_HH_mm_ss") + " PRIMARY KEY NONCLUSTERED (" + surogateColumnName + ")");
                                SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);
                            }

                            ExecuteNTSDailyLoader(dailyJob, isLastBatchForDailyLoader, true, con);
                        }
                    }
                }
                if (currentBatchTSInstruments != null)
                    PopulateDailyDataForTSIdUpdate(currentBatchTSInstruments, dailyReportJobs, tsReportJob, con);

                if (con != null)
                    con.CommitTransaction();
            }
            catch (Exception ex)
            {
                mLogger.Error("ProcessLoaderInTransaction --> Error -- " + tsReportJob.TableName);
                mLogger.Error(ex);
                if (con != null)
                    con.RollbackTransaction();
                throw;
            }
            finally
            {
                foreach (var tabName in tablesToBeDropped)
                    if (!string.IsNullOrEmpty(tabName))
                        SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, string.Format("IF OBJECT_ID('{0}') IS NOT NULL DROP TABLE {0}", tabName), SRMDBQueryType.SELECT);

                if (con != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(con);

                mLogger.Debug("ProcessLoaderInTransaction --> End -- " + tsReportJob.TableName);
            }
        }

        private static void PerformWorkerJobInTransaction(SRMJobInfo tsReportJob, List<SRMJobInfo> dailyReportJobs, ref List<SRMDWHBlockStatus> blockStatus)
        {
            string failureMessage = string.Empty;
            List<string> tablesToBeDropped = new List<string>();

            HashSet<string> instrumentsWithTSUpdate = new HashSet<string>();
            HashSet<string> instrumentsWithManualUpdate = new HashSet<string>();
            HashSet<string> instrumentsInDaily = new HashSet<string>();
            string securityEntityCode = tsReportJob.IsRef ? "entity_code" : "[Security Id]";
            List<int> blockIdsWithData = new List<int>();
            HashSet<string> TSTableNames = new HashSet<string>();
            try
            {
                ExtractData(tsReportJob, ref blockStatus);

                if (!tsReportJob.IsPassed)
                {
                    failureMessage = "Time Series Report Failed";
                    foreach (var job in dailyReportJobs)
                    {
                        if (job.IsPassed)
                        {
                            InsertStatusForBlockDetails(job.BlockStatusId, failureMessage, job.UserName);
                            InsertUpdateStatusForBlockInObject(job.BlockStatusId, EXECUTION_STATUS.FAILED, failureMessage, DateTime.Now, job.BlockId, ref blockStatus);
                        }
                    }
                    return;
                }
                else
                {
                    if (tsReportJob.DateType != DWHDateType.None)
                        DeleteInactiveInstruments(tsReportJob);

                    string query = string.Format(@" IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('{0}',1) AND TABLE_SCHEMA = PARSENAME('{0}',2))
                                BEGIN
	                                SELECT DISTINCT {1},isTimeSeriesRecord FROM {0}
                                END", tsReportJob.TableName, securityEntityCode);

                    if (tsReportJob.DateType == DWHDateType.None)
                    {
                        securityEntityCode = tsReportJob.IsRef ? "entity_code" : "security_id";

                        query = string.Format(@" IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('{0}',1) AND TABLE_SCHEMA = PARSENAME('{0}',2))
                                BEGIN
                                    SELECT DISTINCT {1} FROM {0}
                                END", tsReportJob.TableName, securityEntityCode);
                    }

                    if (tsReportJob.DateType != DWHDateType.None)
                        securityEntityCode = tsReportJob.IsRef ? "entity_code" : "Security Id";

                    ObjectSet instrumentsInTSOS = SRMDWHJobExtension.ExecuteQueryObject(tsReportJob.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                    if (instrumentsInTSOS != null && instrumentsInTSOS.Tables.Count > 0 && instrumentsInTSOS.Tables[0].Rows.Count > 0)
                    {
                        blockIdsWithData.Add(tsReportJob.BlockId);
                        if (tsReportJob.DateType != DWHDateType.None)
                        {
                            foreach (ObjectRow or in instrumentsInTSOS.Tables[0].Rows)
                            {
                                if (Convert.ToBoolean(or["isTimeSeriesRecord"]))
                                    instrumentsWithTSUpdate.Add(Convert.ToString(or[securityEntityCode]));
                                else
                                    instrumentsWithManualUpdate.Add(Convert.ToString(or[securityEntityCode]));
                            }
                        }
                        else
                            instrumentsWithTSUpdate = instrumentsInTSOS.Tables[0].AsEnumerable().Select(x => Convert.ToString(x[securityEntityCode])).ToHashSet();
                    }

                    foreach (var dailyReportJob in dailyReportJobs)
                    {
                        dailyReportJob.RequireTimeTSReports = tsReportJob.RequireTimeTSReports;
                        dailyReportJob.TSReportDateType = tsReportJob.DateType;
                        dailyReportJob.InstrumentsInTSWithManualUpdate = instrumentsWithManualUpdate;
                        dailyReportJob.InstrumentsInTSWithTSUpdates = instrumentsWithTSUpdate;

                        ExtractData(dailyReportJob, ref blockStatus);

                        if (!dailyReportJob.IsPassed)
                        {
                            failureMessage = "Daily Report Failed";
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(failureMessage))
                    {
                        foreach (var job in dailyReportJobs)
                        {
                            if (job.IsPassed)
                            {
                                InsertStatusForBlockDetails(job.BlockStatusId, failureMessage, job.UserName);
                                InsertUpdateStatusForBlockInObject(job.BlockStatusId, EXECUTION_STATUS.FAILED, failureMessage, DateTime.Now, job.BlockId, ref blockStatus);
                            }
                        }
                        InsertStatusForBlockDetails(tsReportJob.BlockStatusId, failureMessage, tsReportJob.UserName);
                        InsertUpdateStatusForBlockInObject(tsReportJob.BlockStatusId, EXECUTION_STATUS.FAILED, failureMessage, DateTime.Now, tsReportJob.BlockId, ref blockStatus);
                        return;
                    }
                    else
                    {
                        #region TSandDailyLoader
                        bool dailyReportsLED = true;

                        foreach (var dailyReportJob in dailyReportJobs)
                            if (dailyReportJob.DateType != DWHDateType.None)
                                DeleteInactiveInstruments(dailyReportJob);

                        InsertUpdateStatusForBlock(tsReportJob.BlockStatusId, EXECUTION_STATUS.LOADER_EXECUTION_INPROGRESS);
                        InsertStatusForBlockDetails(tsReportJob.BlockStatusId, "Loader Execution Started", tsReportJob.UserName);

                        foreach (var dailyReportJob in dailyReportJobs)
                        {
                            InsertUpdateStatusForBlock(dailyReportJob.BlockStatusId, EXECUTION_STATUS.LOADER_EXECUTION_INPROGRESS);
                            InsertStatusForBlockDetails(dailyReportJob.BlockStatusId, "Loader Execution Started", dailyReportJob.UserName);

                            if (dailyReportJob.DateType == DWHDateType.None)
                                dailyReportsLED = false;

                        }

                        query = string.Empty;
                        securityEntityCode = tsReportJob.IsRef ? "entity_code" : "[Security Id]";
                        foreach (var dailyJob in dailyReportJobs)
                        {
                            if (dailyJob.ModifiedEntities != null && dailyJob.ModifiedEntities.Count > 0)
                            {
                                string subQuery = string.Format("SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('{0}',1) AND TABLE_SCHEMA = PARSENAME('{0}',2)", dailyJob.TableName);
                                ObjectSet ds = SRMDWHJobExtension.ExecuteQueryObject(tsReportJob.DownstreamSQLConnectionName, subQuery, SRMDBQueryType.SELECT);
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    blockIdsWithData.Add(dailyJob.BlockId);
                                    if (string.IsNullOrEmpty(query))
                                        query = string.Format("SELECT DISTINCT {1} FROM {0} ", dailyJob.TableName, securityEntityCode);
                                    else
                                        query = query + " UNION " + string.Format("SELECT DISTINCT {1} FROM {0} ", dailyJob.TableName, securityEntityCode);
                                }
                            }
                        }

                        securityEntityCode = tsReportJob.IsRef ? "entity_code" : "Security Id";
                        ObjectSet instrumentsInDailyOS = null;
                        if (!string.IsNullOrEmpty(query))
                        {
                            instrumentsInDailyOS = SRMDWHJobExtension.ExecuteQueryObject(tsReportJob.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                        }
                        if (instrumentsInDailyOS != null && instrumentsInDailyOS.Tables.Count > 0 && instrumentsInDailyOS.Tables[0].Rows.Count > 0)
                            instrumentsInDaily = instrumentsInDailyOS.Tables[0].AsEnumerable().Select(x => Convert.ToString(x[securityEntityCode])).ToHashSet();

                        if (tsReportJob.DateType != DWHDateType.None && dailyReportsLED)
                        {
                            ExecuteLoaderInBatches(tsReportJob, dailyReportJobs, ref blockStatus, instrumentsWithTSUpdate, instrumentsWithManualUpdate, instrumentsInDaily, blockIdsWithData);
                        }
                        else
                        {
                            TSTableNames = ExecuteLoaderInSingleBatch(tsReportJob, dailyReportJobs, ref blockStatus, tablesToBeDropped, instrumentsWithTSUpdate, blockIdsWithData);
                        }

                        ExecuteDWHAdapters(tsReportJob, dailyReportJobs, TSTableNames);

                        //Set Passed Status
                        InsertStatusForBlockDetails(tsReportJob.BlockStatusId, "Loader Execution Completed", tsReportJob.UserName);
                        if (tsReportJob.IsPassed)
                        {
                            InsertUpdateStatusForBlock(tsReportJob.BlockStatusId, EXECUTION_STATUS.PASSED);
                            InsertUpdateStatusForBlockInObject(tsReportJob.BlockStatusId, EXECUTION_STATUS.PASSED, string.Empty, DateTime.Now, tsReportJob.BlockId, ref blockStatus);
                            UpdateLastRunDate(tsReportJob);
                        }

                        foreach (var dailyJob in dailyReportJobs)
                        {
                            InsertStatusForBlockDetails(dailyJob.BlockStatusId, "Loader Execution Completed", dailyJob.UserName);
                            if (dailyJob.IsPassed)
                            {
                                InsertUpdateStatusForBlock(dailyJob.BlockStatusId, EXECUTION_STATUS.PASSED);
                                InsertUpdateStatusForBlockInObject(dailyJob.BlockStatusId, EXECUTION_STATUS.PASSED, string.Empty, DateTime.Now, dailyJob.BlockId, ref blockStatus);
                                UpdateLastRunDate(dailyJob);
                            }
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;
                foreach (var job in dailyReportJobs)
                {
                    InsertStatusForBlockDetails(job.BlockStatusId, ex.ToString(), job.UserName);
                    InsertUpdateStatusForBlock(job.BlockStatusId, EXECUTION_STATUS.FAILED);
                    InsertUpdateStatusForBlockInObject(job.BlockStatusId, EXECUTION_STATUS.FAILED, ex.Message, DateTime.Now, job.BlockId, ref blockStatus);
                }
                InsertUpdateStatusForBlock(tsReportJob.BlockStatusId, EXECUTION_STATUS.FAILED);
                InsertStatusForBlockDetails(tsReportJob.BlockStatusId, ex.ToString(), tsReportJob.UserName);
                InsertUpdateStatusForBlockInObject(tsReportJob.BlockStatusId, EXECUTION_STATUS.FAILED, ex.Message, DateTime.Now, tsReportJob.BlockId, ref blockStatus);
            }
            finally
            {
                foreach (var tabName in tablesToBeDropped)
                    if (!string.IsNullOrEmpty(tabName))
                        SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, string.Format("IF OBJECT_ID('{0}') IS NOT NULL DROP TABLE {0}", tabName), SRMDBQueryType.SELECT);
            }
        }

        private static HashSet<string> ExecuteLoaderInSingleBatch(SRMJobInfo tsReportJob, List<SRMJobInfo> dailyReportJobs, ref List<SRMDWHBlockStatus> blockStatus, List<string> tablesToBeDropped,
            HashSet<string> instrumentsWithTSUpdate, List<int> blockIdsWithData)
        {
            RDBConnectionManager con = null;
            string typeName = tsReportJob.IsRef ? "[Entity Type Name]" : "[Security Type Name]";
            string typeId = tsReportJob.IsRef ? "[entity_code]" : "[Security Id]";
            DataSet tsTablesToRename = null;
            HashSet<string> TSTableNames = new HashSet<string>();
            try
            {
                con = RDALAbstractFactory.DBFactory.GetConnectionManager(tsReportJob.DownstreamSQLConnectionName);
                con.IsolationLevel = IsolationLevel.RepeatableRead;
                con.UseTransaction = true;

                if (tsReportJob.DateType == DWHDateType.None && blockIdsWithData.Contains(tsReportJob.BlockId))
                    tsTablesToRename = ExecuteTimeSeriesNTNLoader(tsReportJob);
                else if (blockIdsWithData.Contains(tsReportJob.BlockId))
                {
                    StringBuilder sb1 = new StringBuilder();
                    sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_1 ON " + tsReportJob.TableName + "(" + typeName + ")");
                    sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_2 ON " + tsReportJob.TableName + "(" + typeId + ")");
                    sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_3 ON " + tsReportJob.TableName + "([isTimeSeriesRecord])");
                    sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_5 ON " + tsReportJob.TableName + "(" + typeId + ",[Effective Start Date])");
                    sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_6 ON " + tsReportJob.TableName + "(" + typeId + ",[Effective End Date])");
                    if (!tsReportJob.IsRef)
                        sb1.AppendLine("CREATE NONCLUSTERED INDEX idex_7 ON " + tsReportJob.TableName + "([Security Key])");

                    SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);

                    mLogger.Debug("Substep --> ExecuteLoader TS for table : " + tsReportJob.TableName + " started at : " + DateTime.Now);
                    new SRMDWHTSLoader().Begin(tsReportJob, con, tablesToBeDropped);
                    mLogger.Debug("Substep --> ExecuteLoader TS for table : " + tsReportJob.TableName + " completed at : " + DateTime.Now);
                }

                foreach (SRMJobInfo dailyReportJob in dailyReportJobs)
                {
                    if (dailyReportJob.DateType == DWHDateType.None && blockIdsWithData.Contains(dailyReportJob.BlockId))
                        ExecuteDailyNTNLoader(dailyReportJob);
                    else if (blockIdsWithData.Contains(dailyReportJob.BlockId) && dailyReportJob.ModifiedEntities != null && dailyReportJob.ModifiedEntities.Count > 0)
                    {
                        string surogateColumnName = dailyReportJob.TableName.Split('.')[1].Replace("[ivp_polaris_", "").Replace("_daily_staging]", "") + "_daily_surrogate_id";

                        StringBuilder sb1 = new StringBuilder();
                        sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_2 ON " + dailyReportJob.TableName + "(" + typeId + ")");

                        if (dailyReportJob.IsLegReport)
                            sb1.AppendLine("CREATE UNIQUE NONCLUSTERED INDEX idx_4 ON " + dailyReportJob.TableName + "([" + surogateColumnName + "],[Row Id])");

                        SRMDWHJobExtension.ExecuteQuery(dailyReportJob.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);

                        if (!dailyReportJob.IsLegReport)
                        {
                            sb1 = new StringBuilder();
                            sb1.AppendLine("ALTER TABLE " + dailyReportJob.TableName + " ADD CONSTRAINT pk_stg_" + surogateColumnName + "_" + DateTime.Now.ToString("yyyyMMdd_HH_mm_ss") + " PRIMARY KEY NONCLUSTERED (" + surogateColumnName + ")");
                            SRMDWHJobExtension.ExecuteQuery(dailyReportJob.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);
                        }

                        ExecuteNTSDailyLoader(dailyReportJob, true, true, con);
                    }
                }


                if (tsReportJob.DateType == DWHDateType.None && blockIdsWithData.Contains(tsReportJob.BlockId) && dailyReportJobs.Any(x => x.DateType != DWHDateType.None))
                {
                    PopulateDailyDataForTSIdUpdate(instrumentsWithTSUpdate, dailyReportJobs.Where(x => x.DateType != DWHDateType.None).ToList(), tsReportJob, con);
                }

                foreach (var dailyJob in dailyReportJobs)
                {
                    if (dailyJob.DateType == DWHDateType.None && blockIdsWithData.Contains(dailyJob.BlockId))
                        RenameNTNStagingToDimensionTable(dailyJob, false, con, null);
                }

                if (tsReportJob.DateType == DWHDateType.None && blockIdsWithData.Contains(tsReportJob.BlockId)       && tsTablesToRename != null)
                {
                    RenameNTNStagingToDimensionTable(tsReportJob, true, con, tsTablesToRename);

                    foreach (DataRow dr in tsTablesToRename.Tables[0].Rows)
                    {
                        string tempTableName = Convert.ToString(dr["dimension_table_name"]);
                        string finalTableNameOriginal = tempTableName.Replace("_NTN]", "]");
                        TSTableNames.Add(finalTableNameOriginal);
                    }
                }

                con.CommitTransaction();

                return TSTableNames;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);

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

        private static void ExecuteLoaderInBatches(SRMJobInfo tsReportJob, List<SRMJobInfo> dailyReportJobs, ref List<SRMDWHBlockStatus> blockStatus, HashSet<string> instrumentsWithTSUpdate, HashSet<string> instrumentsWithManualUpdate, HashSet<string> instrumentsInDaily, List<int> blockIdsWithData)
        {
            bool isLastBatchForDailyLoader = false;
            string securityEntityCode = tsReportJob.IsRef ? "entity_code" : "[Security Id]";
            try
            {
                DropAndCreateBackupTableForBatches(tsReportJob, securityEntityCode);

                foreach (var dailyJob in dailyReportJobs)
                {
                    if (dailyJob.DateType != DWHDateType.None)
                    {
                        DropAndCreateBackupTableForBatches(dailyJob, securityEntityCode);
                    }
                }

                securityEntityCode = tsReportJob.IsRef ? "entity_code" : "Security Id";
                if (instrumentsWithManualUpdate.Count > 0)
                {
                    if (instrumentsWithTSUpdate.Count > 0)
                        instrumentsWithManualUpdate = instrumentsWithManualUpdate.Except(instrumentsWithTSUpdate).ToHashSet();

                    mLogger.Debug("Process Time Series = 0 records");
                    mLogger.Debug("InstrumentIds : " + string.Join(",", instrumentsWithManualUpdate));

                    InsertStatusForBlockDetails(tsReportJob.BlockStatusId, "Processing batch (Time Series = 0)", tsReportJob.UserName);
                    foreach (var dailyJob in dailyReportJobs)
                    {
                        if (blockIdsWithData.Contains(dailyJob.BlockId))
                            InsertStatusForBlockDetails(dailyJob.BlockStatusId, "Processing batch (Time Series = 0)", dailyJob.UserName);
                    }

                    instrumentsInDaily = instrumentsInDaily.Except(instrumentsWithManualUpdate).ToHashSet();
                    if (instrumentsInDaily.Count > 0)
                        isLastBatchForDailyLoader = false;

                    ProcessLoaderInTransaction(tsReportJob, dailyReportJobs, instrumentsWithManualUpdate, blockIdsWithData, isLastBatchForDailyLoader, null);
                }
                if (instrumentsWithTSUpdate.Count > 0)
                {
                    HashSet<string> instrumentsWithTSUpdateForTempDaily = new HashSet<string>(instrumentsWithTSUpdate);

                    instrumentsWithTSUpdate = instrumentsWithTSUpdate.Union(instrumentsInDaily).ToHashSet();

                    Queue<HashSet<string>> batches = new Queue<HashSet<string>>();
                    GetBatches(tsReportJob.BatchSize, instrumentsWithTSUpdate, batches);

                    int batchCounter = 0, totalbatcheCount = batches.Count;
                    if (batches.Count > 0)
                    {
                        do
                        {
                            HashSet<string> currentBatchInstruments = batches.Dequeue();
                            HashSet<string> currentBatchTSInstruments = currentBatchInstruments.Intersect(instrumentsWithTSUpdateForTempDaily).ToHashSet();

                            batchCounter++;
                            if (batchCounter == totalbatcheCount)
                            {
                                isLastBatchForDailyLoader = true;
                            }
                            mLogger.Debug("Processing batch (Time Series = 1) : " + (batchCounter) + " out of : " + totalbatcheCount + " for " + tsReportJob.TableName);
                            mLogger.Debug("InstrumentIds : " + string.Join(",", currentBatchInstruments));

                            InsertStatusForBlockDetails(tsReportJob.BlockStatusId, "Processing batch(Time Series = 1) : " + (batchCounter) + " out of : " + totalbatcheCount, tsReportJob.UserName);
                            foreach (var dailyJob in dailyReportJobs)
                            {
                                if (blockIdsWithData.Contains(dailyJob.BlockId))
                                    InsertStatusForBlockDetails(dailyJob.BlockStatusId, "Processing batch (Time Series = 1) : " + (batchCounter) + " out of : " + totalbatcheCount, dailyJob.UserName);
                            }

                            ProcessLoaderInTransaction(tsReportJob, dailyReportJobs, currentBatchInstruments, blockIdsWithData, isLastBatchForDailyLoader, currentBatchTSInstruments);
                        }
                        while (batches.Count > 0);
                    }
                }
                else
                {
                    Queue<HashSet<string>> batches = new Queue<HashSet<string>>();
                    GetBatches(tsReportJob.BatchSize, instrumentsInDaily, batches);

                    int batchCounter = 0, totalbatcheCount = batches.Count;
                    if (batches.Count > 0)
                    {
                        do
                        {
                            HashSet<string> currentBatchInstruments = batches.Dequeue();
                            batchCounter++;
                            if (batchCounter == totalbatcheCount)
                                isLastBatchForDailyLoader = true;
                            mLogger.Debug("Processing batch (Daily records) : " + (batchCounter) + " out of : " + totalbatcheCount + " for " + tsReportJob.TableName);
                            mLogger.Debug("InstrumentIds : " + string.Join(",", currentBatchInstruments));

                            InsertStatusForBlockDetails(tsReportJob.BlockStatusId, "Processing batch (Daily records) : " + (batchCounter) + " out of : " + totalbatcheCount, tsReportJob.UserName);
                            foreach (var dailyJob in dailyReportJobs)
                            {
                                if (blockIdsWithData.Contains(dailyJob.BlockId))
                                    InsertStatusForBlockDetails(dailyJob.BlockStatusId, "Processing batch (Daily records) : " + (batchCounter) + " out of : " + totalbatcheCount, dailyJob.UserName);
                            }

                            ProcessLoaderInTransaction(tsReportJob, dailyReportJobs, currentBatchInstruments, blockIdsWithData, isLastBatchForDailyLoader, null);
                        }
                        while (batches.Count > 0);
                    }
                }

                foreach (var dailyJob in dailyReportJobs)
                {
                    if (dailyJob.DateType == DWHDateType.None && dailyJob.IsLegReport)
                    {
                        ExecuteDailyNTNLoader(dailyJob);
                        RenameNTNStagingToDimensionTable(dailyJob, false, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);

                throw;
            }
        }

        private static string DropAndCreateBackupTableForBatches(SRMJobInfo job, string securityEntityCode)
        {
            string query = string.Format(@"
                            IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('{0}',1) AND TABLE_SCHEMA = PARSENAME('{0}',2))
                            BEGIN
	                            DROP TABLE {0}
                            END
                            IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('{1}',1) AND TABLE_SCHEMA = PARSENAME('{1}',2))
                            BEGIN                                
                                DECLARE @oldTableName VARCHAR(MAX) = PARSENAME('{1}',2) + '.' + PARSENAME('{1}',1)
                                DECLARE @newTableName VARCHAR(MAX) = PARSENAME('{0}',1)
                                EXEC sp_rename @oldTableName,@newTableName   
                                CREATE NONCLUSTERED INDEX idx_2 ON {0} ({2})
                            END", job.BackupTableName, job.TableName, securityEntityCode);
            SRMDWHJobExtension.ExecuteQueryObject(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
            return query;
        }

        private static void ExtractData(SRMJobInfo job, ref List<SRMDWHBlockStatus> blockStatus)
        {
            lock (SRMDWHStatic.lockObjectForStatusUpdate)
            {
                job.BlockStatusId = new SRMDWHJob().InsertUpdateStatusForBlock(job.SetupStatusId, job.BlockId, EXECUTION_STATUS.INPROGRESS, job.UserName, string.Empty, -1, DateTime.Now);
                InsertStatusForBlockDetails(job.BlockStatusId, "Fetching Report Data", job.UserName);
            }

            try
            {
                if (job.IsRef)
                    GetRefMasterDataAndPushToStagingTables(job);
                else
                    GetSecMasterDataAndPushToStagingTables(job);

                ExecuteCustomClass(job);

                PushMessageToQueue(job);

                InsertUpdateStatusForBlock(job.BlockStatusId, EXECUTION_STATUS.LOADER_EXECUTION_PENDING);
            }
            catch (Exception ex)
            {
                job.IsPassed = false;
                mLogger.Error(ex);

                InsertStatusForBlockDetails(job.BlockStatusId, ex.ToString(), job.UserName);
                InsertUpdateStatusForBlockInObject(job.BlockStatusId, EXECUTION_STATUS.FAILED, ex.Message, DateTime.Now, job.BlockId, ref blockStatus);

                Environment.ExitCode = 1;
            }
        }

        private static void PopulateDailyDataForTSIdUpdate(HashSet<string> instrumentsInTSUpdateOnly, List<SRMJobInfo> dailyReportJobs, SRMJobInfo tsReportJob, RDBConnectionManager con)
        {
            List<string> tablesToBeDropped = new List<string>();
            Dictionary<string, Dictionary<DateTime, int>> instIdVsDateVsTimeSeriesId = new Dictionary<string, Dictionary<DateTime, int>>();
            string tableName = string.Empty;
            ObjectTable dtInstIds = new ObjectTable();
            dtInstIds.Columns.Add("instrument_id", typeof(string));
            foreach (var instId in instrumentsInTSUpdateOnly)
                dtInstIds.Rows.Add(instId);

            SRMJobInfo masterDailyReportjob = dailyReportJobs.Where(x => !x.IsLegReport).FirstOrDefault();

            if (masterDailyReportjob == null)
                masterDailyReportjob = dailyReportJobs.FirstOrDefault();

            if (tsReportJob.DateType == DWHDateType.None)
            {
                //Get Data from Temp Dimension Table
                tableName = string.Empty;
                string tsDimensionTableName = tsReportJob.TableName.Replace("_staging]", "_NTN]");
                instIdVsDateVsTimeSeriesId = CreateTimeSeriesLookUpCollection(masterDailyReportjob, ref tableName, dtInstIds, null, masterDailyReportjob, tsDimensionTableName, false);
                tablesToBeDropped.Add(tableName);
            }
            else
            {
                //Get Data from Dimension Table
                instIdVsDateVsTimeSeriesId = CreateTimeSeriesLookUpCollection(masterDailyReportjob, ref tableName, dtInstIds, con, tsReportJob, string.Empty, false);
                tablesToBeDropped.Add(tableName);
            }

            if (instIdVsDateVsTimeSeriesId.Count > 0)
            {
                Dictionary<string, string> instrumentVsMasterId = new Dictionary<string, string>();

                if (tsReportJob.IsRef)
                    instrumentVsMasterId = EntityCodeVsMasterId;
                else
                    instrumentVsMasterId = SecurityIdVsMasterId;

                foreach (SRMJobInfo dailyReportJob in dailyReportJobs)
                {
                    if (!dailyReportJob.IsLegReport || (dailyReportJob.IsLegReport && requireTSIdInLegDailyTable))
                    {
                        string tempTable = dailyReportJob.TableName.Substring(0, dailyReportJob.TableName.Length - 1) + "_dummy]";
                        ObjectTable tempTableOT = new ObjectTable();
                        tempTableOT.Columns.Add(new ObjectColumn("time_series_id", typeof(System.Int32)));
                        tempTableOT.Columns.Add(new ObjectColumn("surrogate_id", typeof(System.Int64)));

                        foreach (string instrument in instrumentsInTSUpdateOnly)
                        {
                            DateTime startDate = tsReportJob.EffectiveStartDateForReport.Value.Date;
                            DateTime endDate = DateTime.Now.Date;

                            while (startDate <= endDate)
                            {
                                if (instrumentVsMasterId.ContainsKey(instrument))
                                {
                                    ObjectRow or = tempTableOT.NewRow();

                                    int timeSeriesId;
                                    Dictionary<DateTime, int> dateVsId = null;
                                    if (instIdVsDateVsTimeSeriesId.TryGetValue(instrument, out dateVsId))
                                    {
                                        if (dateVsId.TryGetValue(startDate, out timeSeriesId))
                                        {
                                            or[0] = timeSeriesId;
                                            or[1] = startDate.ToString("yyyyMMdd") + instrumentVsMasterId[instrument];
                                            tempTableOT.Rows.Add(or);
                                        }
                                    }
                                    startDate = startDate.AddDays(1);
                                }
                            }
                        }

                        if (tempTableOT.Rows.Count > 0)
                        {
                            string query = string.Format(@"
                                IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('{0}',1) AND TABLE_SCHEMA = PARSENAME('{0}',2))
                                    DROP TABLE {0}
                                CREATE TABLE {0} (time_series_id INT, surrogate_id BIGINT,INDEX cci CLUSTERED COLUMNSTORE)", tempTable);
                            SRMDWHJobExtension.ExecuteQuery(tsReportJob.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                            SRMDWHJobExtension.ExecuteBulkCopyObject(tsReportJob.DownstreamSQLConnectionName, tempTable, tempTableOT);

                            //Loader
                            string dimensionTableName = dailyReportJob.TableName.Replace("[references]", "[dimension]").Replace("[taskmanager]", "[dimension]").Replace("_staging]", "]");
                            string midName = dimensionTableName.Replace("[dimension].", "").Replace("[ivp_polaris_", "").Replace("]", "");
                            string surrogateColumnn = midName.ToLower() + "_surrogate_id";
                            string timeSeriesIdColumnn = "timeseries_" + midName.ToLower() + "_id";

                            query = String.Format(@" MERGE {0} as tab2 
                                                USING(SELECT * FROM {1}) AS tab
                                                ON({2})
                                                WHEN MATCHED THEN UPDATE SET {3};", dimensionTableName, tempTable, "tab2." + surrogateColumnn + " = tab.surrogate_id", "tab2." + timeSeriesIdColumnn + " = tab.time_series_id");
                            SRMDWHJobExtension.ExecuteQuery(con, query, SRMDBQueryType.SELECT);
                        }
                    }
                }
            }
        }

        private static void PerformWorkerJob(SRMJobInfo job, ref List<SRMDWHBlockStatus> blockStatus)
        {
            string typeName = job.IsRef ? "[Entity Type Name]" : "[Security Type Name]";
            string typeId = job.IsRef ? "[entity_code]" : "[Security Id]";

            string typeIdWithoutBrackets = job.IsRef ? "entity_code" : "Security Id";
            List<string> tablesToBeDropped = new List<string>();
            DataSet tsTablesToRename = null;
            lock (SRMDWHStatic.lockObjectForStatusUpdate)
            {
                job.BlockStatusId = new SRMDWHJob().InsertUpdateStatusForBlock(job.SetupStatusId, job.BlockId, EXECUTION_STATUS.INPROGRESS, job.UserName, string.Empty, -1, DateTime.Now);
                InsertStatusForBlockDetails(job.BlockStatusId, "Fetching Report Data", job.UserName);
            }

            try
            {
                if (job.IsRef)
                    GetRefMasterDataAndPushToStagingTables(job);
                else
                    GetSecMasterDataAndPushToStagingTables(job);

                ExecuteCustomClass(job);

                PushMessageToQueue(job);

                DeleteInactiveInstruments(job);

                string query = string.Format("SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('{0}',1) AND TABLE_SCHEMA = PARSENAME('{0}',2)", job.TableName);
                DataSet ds = SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    InsertStatusForBlockDetails(job.BlockStatusId, "Loader Execution Started", job.UserName);

                    if (job.BlockType == BLOCK_TYPES.NTS_REPORT)
                    {
                        StringBuilder sb1 = new StringBuilder();
                        sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_1 ON " + job.TableName + "(master_id)");
                        SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);

                        ExecuteNTSDailyLoader(job, true, false, null);
                        InsertStatusForBlockDetails(job.BlockStatusId, "Loader Execution Completed", job.UserName);
                    }
                    else if (job.BlockType == BLOCK_TYPES.TS_REPORT)
                    {
                        if (job.DateType == DWHDateType.None)
                        {
                            typeId = job.IsRef ? "[entity_code]" : "[Security_Id]";
                            query = string.Format("SELECT {0},MIN([effective_start_date]) AS [Effective Start Date] FROM {1} GROUP BY {0}", typeId, job.TableName);
                            typeId = job.IsRef ? "[entity_code]" : "[Security Id]";
                            typeIdWithoutBrackets = job.IsRef ? "entity_code" : "Security_Id";
                        }
                        else
                            query = string.Format("SELECT {0},MIN([Effective Start Date]) AS [Effective Start Date] FROM {1} GROUP BY {0}", typeId, job.TableName);

                        Dictionary<string, DateTime> dictTSInstrumentsVsESD = SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x[typeIdWithoutBrackets]), y => Convert.ToDateTime(y["Effective Start Date"]));

                        if (job.DateType == DWHDateType.None)
                        {
                            tsTablesToRename = ExecuteTimeSeriesNTNLoader(job);
                        }
                        else
                        {
                            StringBuilder sb1 = new StringBuilder();
                            sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_1 ON " + job.TableName + "(" + typeName + ")");
                            sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_2 ON " + job.TableName + "(" + typeId + ")");
                            sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_3 ON " + job.TableName + "([isTimeSeriesRecord])");
                            sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_4 ON " + job.TableName + "([isArchiveRecord])");
                            sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_5 ON " + job.TableName + "(" + typeId + ",[Effective Start Date])");
                            sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_6 ON " + job.TableName + "(" + typeId + ",[Effective End Date])");
                            if (!job.IsRef)
                                sb1.AppendLine("CREATE NONCLUSTERED INDEX idex_7 ON " + job.TableName + "([Security Key])");

                            SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);

                            new SRMDWHTSLoader().Begin(job, null, tablesToBeDropped);
                        }
                        InsertStatusForBlockDetails(job.BlockStatusId, "Loader Execution Completed", job.UserName);

                        InsertStatusForBlockDetails(job.BlockStatusId, "Populating Dummy Daily Table Started", job.UserName);

                        //Job Info TS to Daily
                        job.BlockType = BLOCK_TYPES.DAILY_REPORT;
                        job.TimeSeriesTableNameWithSchema = job.TableName.Replace("[references]", "[dimension]").Replace("[taskmanager]", "[dimension]").Replace("_staging]", "]");
                        job.TableName = job.TableName.Replace("_time_series_staging", "_daily_staging");
                        Dictionary<string, Dictionary<DateTime, int>> instIdVsDateVsTimeSeriesId = new Dictionary<string, Dictionary<DateTime, int>>();
                        //Create TimeSeries Lookup Collection
                        string securityEntityCode = job.IsRef ? "entity_code" : "Security Id";

                        HashSet<string> instruments = dictTSInstrumentsVsESD.Keys.ToHashSet();
                        if (instruments.Count > 0)
                        {
                            ObjectTable dtInstIds = new ObjectTable();
                            dtInstIds.Columns.Add("instrument_id", typeof(string));
                            foreach (var instId in instruments)
                                dtInstIds.Rows.Add(instId);

                            //Get Data from Dimension Table
                            string tableName = string.Empty;
                            string tsDimensionTableName = string.Empty;
                            bool requireColumnRename = true;

                            if (job.DateType == DWHDateType.None)
                            {
                                requireColumnRename = false;
                                tsDimensionTableName = job.TimeSeriesTableNameWithSchema.Replace("_series]", "_series_NTN]");
                            }
                            else
                                tsDimensionTableName = job.TimeSeriesTableNameWithSchema;

                            mLogger.Debug("tsDimensionTableName : " + tsDimensionTableName);
                            mLogger.Debug("jobInfo.TSReportDateType : " + job.TSReportDateType);
                            mLogger.Debug("requireColumnRename : " + requireColumnRename);

                            instIdVsDateVsTimeSeriesId = CreateTimeSeriesLookUpCollection(job, ref tableName, dtInstIds, null, tsReportJob: job, tsDimensionTableName, requireColumnRename);
                            tablesToBeDropped.Add(tableName);
                        }

                        string surogateColumnName = string.Empty, timeseriesColumnName = string.Empty;
                        query = string.Empty;
                        Dictionary<string, Tuple<string, DateTime>> secIdVsSecuritykey = new Dictionary<string, Tuple<string, DateTime>>();
                        ObjectTable dailyStagingTable = new ObjectTable(job.TableName);

                        surogateColumnName = job.TableName.Split('.')[1].Replace("[ivp_polaris_", "").Replace("_daily_staging]", "") + "_daily_surrogate_id";
                        timeseriesColumnName = "timeseries_" + job.TableName.Split('.')[1].Replace("[ivp_polaris_", "").Replace("_daily_staging]", "") + "_daily_id";

                        query = @"IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('" + job.TableName + @"',1))
                                BEGIN
                                    DROP TABLE " + job.TableName + @" 
                                END  
                                CREATE TABLE " + job.TableName + "(";
                        if (job.IsRef)
                        {
                            query = query + "[entity_code] VARCHAR(15),";
                            dailyStagingTable.Columns.Add(new ObjectColumn("entity_code", typeof(System.String)));
                        }
                        else
                        {
                            query = query + "[Security Id] VARCHAR(15),[Security Key] VARCHAR(15),";
                            dailyStagingTable.Columns.Add(new ObjectColumn("Security Id", typeof(System.String)));
                            dailyStagingTable.Columns.Add(new ObjectColumn("Security Key", typeof(System.String)));
                        }

                        query = query + @"[Effective Date] DATE,[master_id] INT,[" + surogateColumnName + "] BIGINT NOT NULL,[" + timeseriesColumnName + "] INT ,INDEX cci CLUSTERED COLUMNSTORE" +
                            ") ";

                        dailyStagingTable.Columns.Add(new ObjectColumn("Effective Date", typeof(System.DateTime)));
                        dailyStagingTable.Columns.Add(new ObjectColumn("master_id", typeof(System.Int32)));
                        dailyStagingTable.Columns.Add(new ObjectColumn(surogateColumnName, typeof(System.Int64)));
                        dailyStagingTable.Columns.Add(new ObjectColumn(timeseriesColumnName, typeof(System.Int32)));

                        Dictionary<string, string> instrumentIdVsMasterId = new Dictionary<string, string>();

                        if (job.IsRef)
                            instrumentIdVsMasterId = GetEntityCodeVsMasterId(job, dictTSInstrumentsVsESD.Keys.ToHashSet());
                        else
                        {
                            secIdVsSecuritykey = GetSecurityIdVsSecurityKey();
                            HashSet<string> securityKeys = new HashSet<string>();
                            foreach (var securityIdVsESD in dictTSInstrumentsVsESD)
                            {
                                if (secIdVsSecuritykey.ContainsKey(securityIdVsESD.Key))
                                    securityKeys.Add(secIdVsSecuritykey[securityIdVsESD.Key].Item1);
                                else
                                    securityKeys.Add(securityIdVsESD.Key);

                            }
                            instrumentIdVsMasterId = GetSecurityIdVsMasterId(job.DownstreamSQLConnectionName, securityKeys);

                        }

                        DateTime maxDate = DateTime.Now.Date;
                        if (job.IsRef)
                            maxDate = DateTime.ParseExact(job.EndDate, "yyyyMMdd HH:mm:ss.fff", null).Date;
                        else
                        {
                            string dateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + System.Text.RegularExpressions.Regex.Replace(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern, "(:ss|:s)", "$1.fff");
                            maxDate = DateTime.ParseExact(job.EndDate, dateFormat, null).Date;
                        }

                        foreach (var instrumentIdKvp in dictTSInstrumentsVsESD)
                        {
                            DateTime esd = instrumentIdKvp.Value.Date;
                            string effectiveDateString = esd.ToString("yyyyMMdd");

                            while (esd <= maxDate)
                            {
                                effectiveDateString = esd.ToString("yyyyMMdd");
                                ObjectRow or = dailyStagingTable.NewRow();
                                if (job.IsRef)
                                {
                                    or["entity_code"] = instrumentIdKvp.Key;
                                    or["master_id"] = instrumentIdVsMasterId[instrumentIdKvp.Key];
                                }
                                else
                                {
                                    or["Security Id"] = instrumentIdKvp.Key;
                                    if (secIdVsSecuritykey.ContainsKey(instrumentIdKvp.Key))
                                    {
                                        or["Security Key"] = secIdVsSecuritykey[instrumentIdKvp.Key].Item1;
                                        or["master_id"] = instrumentIdVsMasterId[secIdVsSecuritykey[instrumentIdKvp.Key].Item1];
                                    }
                                    else
                                    {
                                        or["Security Key"] = instrumentIdKvp.Key;
                                        or["master_id"] = instrumentIdVsMasterId[instrumentIdKvp.Key];
                                    }
                                }
                                or["Effective Date"] = esd;
                                or[surogateColumnName] = Convert.ToInt64(effectiveDateString + Convert.ToInt32(or["master_id"]));

                                mLogger.Debug("Setting TimeSeries ID for : " + instrumentIdKvp.Key + " and instIdVsDateVsTimeSeriesId.containsKey : " + instIdVsDateVsTimeSeriesId.ContainsKey(instrumentIdKvp.Key));
                                mLogger.Debug("instIdVsDateVsTimeSeriesId contains date : " + esd + " --> " + instIdVsDateVsTimeSeriesId[instrumentIdKvp.Key].ContainsKey(esd));
                                int timeSeriesId;
                                Dictionary<DateTime, int> dateVsId = null;
                                if (instIdVsDateVsTimeSeriesId.TryGetValue(instrumentIdKvp.Key, out dateVsId))
                                    if (dateVsId.TryGetValue(esd, out timeSeriesId))
                                        or[timeseriesColumnName] = timeSeriesId;

                                dailyStagingTable.Rows.Add(or);
                                esd = esd.AddDays(1);
                            }
                        }
                        SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                        SRMDWHJobExtension.ExecuteBulkCopyObject(job.DownstreamSQLConnectionName, job.TableName, dailyStagingTable);

                        if (job.DateType.Equals(DWHDateType.None))
                        {
                            string finalTableName = job.TableName.Replace("[references]", "[dimension]").Replace("[taskmanager]", "[dimension]").Replace("_staging]", "]");

                            query = string.Format(@"IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('{0}',1) AND TABLE_SCHEMA = PARSENAME('{0}',2))
                                    BEGIN
                                        DROP TABLE {0}
                                    END", finalTableName);
                            SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);

                            ExecuteDailyNTNLoader(job);
                            RenameNTNStagingToDimensionTable(job, false, null, null);
                            job.BlockType = BLOCK_TYPES.TS_REPORT;
                            RenameNTNStagingToDimensionTable(job, false, null, tsTablesToRename);
                        }
                        else
                            PerformWorkerJobDailyLoader(job, typeId);

                        //RDBConnectionManager conn = SRMDWHJobExtension.GetConnectionManager(job.DownstreamSQLConnectionName, true, IsolationLevel.RepeatableRead);
                        //SyncTimeSeriesIdInDailyTable(job, dictTSInstrumentsVsESD.Keys.ToList(), conn, null, string.Empty, string.Empty, tablesToBeDropped);
                        //conn.CommitTransaction();

                        //if (conn != null)
                        //    RDALAbstractFactory.DBFactory.PutConnectionManager(conn);

                        //Revert Job Info Daily to TS
                        job.TimeSeriesTableNameWithSchema = string.Empty;
                        job.BlockType = BLOCK_TYPES.TS_REPORT;
                        job.TableName = job.TableName.Replace("_daily_staging", "_time_series_staging");

                        InsertStatusForBlockDetails(job.BlockStatusId, "Populating Dummy Daily Table Completed", job.UserName);

                    }
                    else if (job.BlockType == BLOCK_TYPES.DAILY_REPORT)
                    {
                        if (job.DateType == DWHDateType.None)
                        {
                            ExecuteDailyNTNLoader(job);
                            RenameNTNStagingToDimensionTable(job, false, null, null);
                        }
                        else
                            PerformWorkerJobDailyLoader(job, typeId);
                        InsertStatusForBlockDetails(job.BlockStatusId, "Loader Execution Completed", job.UserName);
                    }
                }

                SRMDWHJob.ExecuteDWHAdapters(job, null);

                UpdateLastRunDate(job);
                InsertUpdateStatusForBlockInObject(job.BlockStatusId, EXECUTION_STATUS.PASSED, string.Empty, DateTime.Now, job.BlockId, ref blockStatus);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);

                InsertStatusForBlockDetails(job.BlockStatusId, ex.ToString(), job.UserName);
                InsertStatusForBlockDetails(job.BlockStatusId, "Sending Failure Subscription Mail", job.UserName);

                InsertUpdateStatusForBlockInObject(job.BlockStatusId, EXECUTION_STATUS.FAILED, ex.Message, DateTime.Now, job.BlockId, ref blockStatus);
                Environment.ExitCode = 1;
            }
            finally
            {
                foreach (var tabName in tablesToBeDropped)
                    if (!string.IsNullOrEmpty(tabName))
                        SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, string.Format("IF OBJECT_ID('{0}') IS NOT NULL DROP TABLE {0}", tabName), SRMDBQueryType.SELECT);
            }
        }

        private static DataSet ExecuteTimeSeriesNTNLoader(SRMJobInfo job)
        {
            string typeName = job.IsRef ? "[entity_type_name]" : "[security_type_name]";
            string typeId = job.IsRef ? "[entity_code]" : "[security_id]";
            StringBuilder sb1 = new StringBuilder();
            sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_1 ON " + job.TableName + "(" + typeName + ")");
            sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_3 ON " + job.TableName + "([master_id])");
            sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_4 ON " + job.TableName + "(" + typeId + ")");
            SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);

            string query = string.Format(@"
                BEGIN TRY
                    DECLARE @tsStagingTableName VARCHAR(1000) = '{0}'
                    DECLARE @isRef BIT = '{1}'
                    DECLARE @requireIntraDay BIT = '{2}'

                    DECLARE @sql VARCHAR(MAX) = '', @sql2 VARCHAR(MAX) = '',@rowCount INT, @rowIndex INT = 1, @entity_type_name VARCHAR(1000),@leg_name VARCHAR(1000), @dimension_table_name VARCHAR(1000),@isLeg BIT
                    DECLARE @tableName VARCHAR(1000) = '', @newTableName VARCHAR(1000) ='' , @primaryKeyName VARCHAR(1000) = '', @primaryKeyNewName VARCHAR(1000) = ''

                    --------------------------------------------Ref Variables-------------------------------------------
                    DECLARE @refStaticColumns VARCHAR(MAX) = '[id] ,[loading_time] ,[master_id] ,[action date] ,[effective start date] ,[effective end date] ,[modified_by] ,[modified_on] ,[created_on] ,[created_by] ,[entity_code] ,[entity_type_name] ,[is_active],[Row Id],'

                    DECLARE @refStaticColumnsWithDataType VARCHAR(MAX) = '[id] INT IDENTITY NOT NULL,[loading_time] DATETIME,[master_id] INT,[action_date] DATETIME,[effective_start_date] DATETIME,[effective_end_date] DATETIME,
		                    [modified_on] DATETIME,[modified_by] VARCHAR(200),[created_on] DATETIME,[created_by] VARCHAR(200),[entity_code] VARCHAR(15),[entity_type_name] VARCHAR(1000),[is_active] BIT,[Row Id] VARCHAR(1000),'

                    DECLARE @refStaticColumnsForInsert VARCHAR(MAX) = @refStaticColumns--REPLACE(@refStaticColumns,'[modified_on] ,[created_on] ,[created_by] ,[entity_code] ,[entity_type_name] ,[is_active],','[entity_code] ,[entity_type_name] ,')
                    SELECT @refStaticColumnsForInsert = REPLACE(@refStaticColumnsForInsert,'[action date] ,[effective start date] ,[effective end date] ,','[action_date] ,[effective_start_date] ,[effective_end_date] ,')

                    DECLARE @refStaticColumnsForSelect VARCHAR(MAX) = REPLACE(@refStaticColumns,',[action date] ,[effective start date] ,[effective end date]',',[action_date] ,[effective_start_date] ,[effective_end_date]')

                    --------------------------------------------Sec Variables-------------------------------------------
                    DECLARE @secStaticColumns VARCHAR(MAX) = '[id] ,[loading_time] ,[master_id] ,[action date] ,[effective start date] ,[effective end date] ,[modified_by] ,[modified_on] ,[created_on] ,[created_by] ,[security_id] ,[security_type_name] ,[is_active],[Row Id],[is_temp_row],[checksum],[security key],'

                    DECLARE @secStaticColumnsWithDataType VARCHAR(MAX) = '[id] INT IDENTITY NOT NULL,[loading_time] DATETIME,[master_id] INT,[action_date] DATETIME,[effective_start_date] DATETIME,[effective_end_date] DATETIME,
	                    [modified_by] VARCHAR(200),[modified_on] DATETIME,[created_on] DATETIME,[created_by] VARCHAR(200),[security_id] VARCHAR(15),[security_type_name] VARCHAR(1000),[is_active] BIT,[Row Id] VARCHAR(1000),
	                    [is_temp_row] BIT,[checksum] VARBINARY,[security key] VARCHAR(15),'

                    DECLARE @secStaticColumnsForInsert VARCHAR(MAX) = REPLACE(@secStaticColumns,'[modified_by] ,[modified_on] ,[created_on] ,[created_by] ,[security_id] ,[security_type_name] ,[is_active],','[modified_by] ,[security_id] ,[security_type_name] ,')
                    SELECT @secStaticColumnsForInsert = REPLACE(@secStaticColumnsForInsert,'[action date] ,[effective start date] ,[effective end date] ,','[action_date] ,[effective_start_date] ,[effective_end_date] ,')
                    SELECT @secStaticColumnsForInsert  = REPLACE(@secStaticColumnsForInsert ,'[is_temp_row],[checksum],','')

	                DECLARE @secStaticColumnsForSelect VARCHAR(MAX) = REPLACE(@secStaticColumns,',[action date] ,[effective start date] ,[effective end date]',',[action_date] ,[effective_start_date] ,[effective_end_date]')
                    SELECT @secStaticColumnsForSelect  = REPLACE(@secStaticColumnsForSelect ,'[is_temp_row],[checksum],','')
					SELECT @secStaticColumnsForSelect  = REPLACE(@secStaticColumnsForSelect ,',[created_on] ,[created_by]','')
					SELECT @secStaticColumnsForSelect  = REPLACE(@secStaticColumnsForSelect ,',[is_active]','')
                    SELECT @secStaticColumnsForSelect  = REPLACE(@secStaticColumnsForSelect ,',[modified_on]','')

                    IF(@requireIntraDay = 0)
                    BEGIN
	                    SELECT @refStaticColumnsWithDataType = REPLACE(@refStaticColumnsWithDataType,'[effective_start_date] DATETIME,[effective_end_date] DATETIME','[effective_start_date] DATE,[effective_end_date] DATE')
	                    SELECT @secStaticColumnsWithDataType = REPLACE(@secStaticColumnsWithDataType,'[effective_start_date] DATETIME,[effective_end_date] DATETIME','[effective_start_date] DATE,[effective_end_date] DATE')
                    END

                    CREATE TABLE #tableColumnsToIgnore(column_name VARCHAR(1000))
                    CREATE TABLE #tsTables(id INT IDENTITY,entity_type_name VARCHAR(1000),leg_name VARCHAR(1000),isLeg BIT, dimension_table_name VARCHAR(1000),is_leg_processed BIT)


                    INSERT INTO #tableColumnsToIgnore
                    SELECT Value from 
	                    commons.fn_ivp_polaris_delimited_to_table(
	                        'id,loading_time,master_id,Action_Date,Action Identifier,Effective_Start_Date,Effective_End_Date,modified_on,modified_by,created_on,created_by,entity_code,Entity_Type_Name,is_active,last_modified_by,isArchiveRecord,isTimeSeriesRecord,Row Id,Security_Id,Last Modified By,Security_Type_Name,security key,checksum,is_temp_row,',',') 

                    INSERT INTO #tsTables
                    SELECT DISTINCT 
	                    CASE WHEN {3} LIKE '%-%'
		                    THEN {3}
	                    ELSE 'MasterData' END,
	                    CASE WHEN {3} LIKE '%-%'
		                    THEN RTRIM(LTRIM(SUBSTRING({3},CHARINDEX('-',{3}) + 1,(LEN({3}) - CHARINDEX('-',{3})))))
	                    ELSE 'MasterData' END,
	       
	                    CASE WHEN {3} LIKE '%-%'
		                    THEN 1 ELSE 0 END,
	                    CASE WHEN {3} LIKE '%-%'
		                    THEN CASE WHEN @isRef = 1
			                    THEN '[dimension].[' + REPLACE(PARSENAME(@tsStagingTableName,1),'_time_series_staging','_leg_') + 
			                    REPLACE(LOWER(SUBSTRING({3},CHARINDEX('-',{3}) + 1,(LEN({3}) - CHARINDEX('-',{3})))) + '_time_series_NTN]',' ','_')
		                    ELSE
			                    '[dimension].[' + REPLACE(PARSENAME(@tsStagingTableName,1),'security_time_series_staging','securities_leg') + 
			                    REPLACE(LOWER(SUBSTRING({3},CHARINDEX('-',{3}) + 1,(LEN({3}) - CHARINDEX('-',{3})))) + '_time_series_NTN]',' ','_')
		                    END
	                    ELSE 
		                    '[dimension].[' + REPLACE(PARSENAME(@tsStagingTableName,1),'_time_series_staging','_time_series_NTN]') END,0

	                    FROM {0}

	                    SELECT @rowCount = @@ROWCOUNT
	
	                    PRINT ('---------------')

		                SELECT DISTINCT dimension_table_name FROM #tsTables
		
	                    IF EXISTS(SELECT TOP 1 1 FROM #tsTables WHERE isLeg = 1)
	                    BEGIN
		                    -- Only if leg exists
		                    WHILE(@rowIndex <= @rowCount)
		                    BEGIN
			                    SELECT @entity_type_name = entity_type_name from #tsTables WHERE id = @rowIndex
			                    SELECT @leg_name = leg_name from #tsTables WHERE id = @rowIndex
			                    SELECT @dimension_table_name = dimension_table_name from #tsTables WHERE id = @rowIndex
			                    SELECT @isLeg = isLeg from #tsTables WHERE id = @rowIndex

			                    IF(ISNULL(@entity_type_name,'') <> '')
			                    BEGIN
				
				                    -- Rename existing primary key
				                    SET @tableName = REPLACE(@dimension_table_name,'_NTN]',']')
                                    SET @newTableName = PARSENAME('{1}',1)                                
                                					            
                                    SET @primaryKeyName = ''
                                    SET @primaryKeyNewName = REPLACE(NEWID(),'-','_') 
				            
                                    SELECT @primaryKeyName = ind.name
                                    FROM sys.indexes ind INNER JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
                                    INNER JOIN sys.columns col 	ON ic.object_id = col.object_id and ic.column_id = col.column_id 
                                    INNER JOIN sys.tables t ON ind.object_id = t.object_id
                                    WHERE t.name = PARSENAME(@tableName,1) AND is_primary_key = 1
				

                                    SELECT @primaryKeyNewName = 
						                CASE WHEN @isLeg = 0 THEN 
							                REPLACE(REPLACE(PARSENAME(@dimension_table_name,1),'ivp_polaris_','pk_'),'_NTN','')
						                ELSE 
							                'pk_leg_' + REPLACE(PARSENAME(@dimension_table_name,1),'_NTN','')
						                END

                                    IF(ISNULL(@primaryKeyName,'')<>'' AND @primaryKeyName = @primaryKeyNewName)
                                    BEGIN
                                        SET @primaryKeyNewName = REPLACE(NEWID(),'-','_') 

	                                    SELECT @primaryKeyNewName = @primaryKeyName + @primaryKeyNewName
	                                    SELECT @primaryKeyName = PARSENAME(@tableName,2)+ '.' + @primaryKeyName
					
	                                    EXEC sp_rename @primaryKeyName , @primaryKeyNewName
	                                
                                    END 

                                    SELECT @primaryKeyName = 
						                CASE WHEN @isLeg = 0 THEN 
							                REPLACE(REPLACE(PARSENAME(@dimension_table_name,1),'ivp_polaris_','pk_'),'_NTN','')
						                ELSE 
							                'pk_leg_' + REPLACE(PARSENAME(@dimension_table_name,1),'_NTN','')
						                END
					                
				                    -- CREATE TABLE 
				                    SELECT @sql = '
							                    IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('''+ @dimension_table_name +''',1) AND TABLE_SCHEMA = ''dimension'')
								                    DROP TABLE '+@dimension_table_name + '

							                    CREATE TABLE' + @dimension_table_name + '(' + 
								                    CASE WHEN @isRef = 1
										                    THEN @refStaticColumnsWithDataType
									                    WHEN @isRef = 0 
										                    THEN @secStaticColumnsWithDataType
								                    END

				                    SELECT @sql += 
						                    CASE WHEN @isRef = 1 THEN '['+ REPLACE(col.COLUMN_NAME,@leg_name + '-','') + '] '
						                    ELSE '['+ REPLACE(col.COLUMN_NAME,@leg_name + ' - ','') + '] ' END+
						                    CASE WHEN DATA_TYPE = 'VARCHAR' THEN 'VARCHAR(' + CASE WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN 'MAX' ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR(MAX)) END + ')'
							                    WHEN DATA_TYPE IN ('NUMERIC','DECIMAL') THEN DATA_TYPE + '(' + CAST(NUMERIC_PRECISION AS VARCHAR(MAX)) + ',' + CAST(NUMERIC_SCALE AS VARCHAR(MAX)) + ')'
							                    ELSE DATA_TYPE
						                    END + ','
				                    FROM INFORMATION_SCHEMA.COLUMNS col
				                    LEFT JOIN #tableColumnsToIgnore colToIgnore
				                    ON (col.COLUMN_NAME = colToIgnore.column_name)
				                    WHERE col.TABLE_NAME = PARSENAME(@tsStagingTableName,1) AND TABLE_SCHEMA = PARSENAME(@tsStagingTableName,2)				
				                    AND ((@isLeg = 1 AND 
					                    (
						                    (col.COLUMN_NAME = @leg_name + '-' +  REPLACE(col.COLUMN_NAME,@leg_name + '-','') AND @isRef = 1) 
						                    OR 
						                    (col.COLUMN_NAME = @leg_name + ' - ' +  REPLACE(col.COLUMN_NAME,@leg_name + ' - ','') AND @isRef = 0)
					                    ))
				                    OR (@isLeg = 0 AND (CHARINDEX('-',col.COLUMN_NAME) <= 0 OR col.COLUMN_NAME LIKE 'EC-%')))
				                    AND colToIgnore.column_name IS NULL
				                    ORDER BY col.COLUMN_NAME
				
				                    SELECT @sql +=  ' INDEX cci CLUSTERED COLUMNSTORE, CONSTRAINT '+@primaryKeyName+' PRIMARY KEY (id))'

				                    IF NOT EXISTS(SELECT TOP 1 1 FROM #tsTables WHERE leg_name = @leg_name AND is_leg_processed = 1)
									BEGIN
										UPDATE #tsTables SET is_leg_processed = 1 WHERE leg_name = @leg_name

										PRINT (@sql)
										EXEC (@sql)
										PRINT ('---------------')

									END
				
				                    -- INSERT TABLE
				                    SELECT @sql2 = CASE WHEN @isRef = 1 
										                    THEN @refStaticColumnsForInsert
									                    WHEN @isRef = 0 
										                    THEN @secStaticColumnsForInsert
								                    END
			
				                    SELECT @sql2 += '['+ REPLACE(REPLACE(col.COLUMN_NAME,@leg_name + ' - ',''),@leg_name + '-','') + '] ' + ','
				                    FROM INFORMATION_SCHEMA.COLUMNS col
				                    LEFT JOIN #tableColumnsToIgnore colToIgnore
				                    ON (col.COLUMN_NAME = colToIgnore.column_name)
				                    WHERE col.TABLE_NAME = PARSENAME(@tsStagingTableName,1) AND TABLE_SCHEMA = PARSENAME(@tsStagingTableName,2)				
				                    AND ((@isLeg = 1 AND 
					                    (
						                    (col.COLUMN_NAME = @leg_name + '-' +  REPLACE(col.COLUMN_NAME,@leg_name + '-','') AND @isRef = 1) 
						                    OR 
						                    (col.COLUMN_NAME = @leg_name + ' - ' +  REPLACE(col.COLUMN_NAME,@leg_name + ' - ','') AND @isRef = 0)
					                    ))
				                    OR (@isLeg = 0 AND (CHARINDEX('-',col.COLUMN_NAME) <= 0 OR col.COLUMN_NAME LIKE 'EC-%')))
				                    AND colToIgnore.column_name IS NULL
				                    ORDER BY col.COLUMN_NAME

				                    -- Create TABLE and id column as primary key with identity insert on/off
				                    SELECT @sql2 = SUBSTRING(@sql2,0,LEN(@sql2))
				                    SELECT @sql2 = 'SET IDENTITY_INSERT ' + @dimension_table_name + ' ON ' + CHAR(13) + '
				                    INSERT INTO ' + @dimension_table_name + '(' + @sql2 + ')'
				                    PRINT (@sql2)
				                    ---- EXEC (@sql2)
				                    PRINT ('---------------')

				                    -- SELECT FROM TABLE 
				                    SELECT @sql = CASE WHEN @isRef = 1
										                    THEN @refStaticColumnsForSelect
									                    WHEN @isRef = 0 
										                    THEN @secStaticColumnsForSelect
									                END
			
				                    SELECT @sql += '['+ col.COLUMN_NAME + '] ' + ','
				                    FROM INFORMATION_SCHEMA.COLUMNS col
				                    LEFT JOIN #tableColumnsToIgnore colToIgnore
				                    ON (col.COLUMN_NAME = colToIgnore.column_name)
				                    WHERE col.TABLE_NAME = PARSENAME(@tsStagingTableName,1) AND TABLE_SCHEMA = PARSENAME(@tsStagingTableName,2)				
				                    AND ((@isLeg = 1 AND 
					                    (
						                    (col.COLUMN_NAME = @leg_name + '-' +  REPLACE(col.COLUMN_NAME,@leg_name + '-','') AND @isRef = 1) 
						                    OR 
						                    (col.COLUMN_NAME = @leg_name + ' - ' +  REPLACE(col.COLUMN_NAME,@leg_name + ' - ','') AND @isRef = 0)
					                    ))
				                    OR (@isLeg = 0 AND (CHARINDEX('-',col.COLUMN_NAME) <= 0 OR col.COLUMN_NAME LIKE 'EC-%')))
				                    AND colToIgnore.column_name IS NULL
				                    ORDER BY col.COLUMN_NAME

				                    SELECT @sql = SUBSTRING(@sql,0,LEN(@sql)) 
				                    SELECT @sql = 'SELECT ' + @sql + ' FROM ' + @tsStagingTableName + ' WHERE '+
					                    CASE WHEN @isLeg = 1 THEN '{3} = ''' + @entity_type_name + ''' '
						                    WHEN @isLeg = 0 THEN '{3} NOT LIKE ''%-%'' '
						                    END 
					                    +' ORDER BY id'
				                    SELECT @sql += CHAR(13) + ' SET IDENTITY_INSERT ' + @dimension_table_name + ' OFF'
				                    PRINT (@sql)
                                    SELECT @sql = @sql2 + CHAR(13) + @sql
				                    EXEC (@sql)
				                    PRINT ('---------------')

			                    END
			                    SELECT @rowIndex = @rowIndex + 1
		                    END
	                    END

	                    ELSE
	                    BEGIN
		                    -- If not leg exists, then rename master else
		                    PRINT ('No Leg')
		
		                    SELECT @entity_type_name = entity_type_name from #tsTables WHERE id = @rowIndex
		                    SELECT @leg_name = leg_name from #tsTables WHERE id = @rowIndex
		                    SELECT @dimension_table_name = dimension_table_name from #tsTables WHERE id = @rowIndex
		                    SELECT @isLeg = isLeg from #tsTables WHERE id = @rowIndex

		                    IF(ISNULL(@entity_type_name,'') <> '')
		                    BEGIN
				
			                    -- Rename existing primary key
			                    SET @tableName = REPLACE(@dimension_table_name,'_NTN]',']')
                                SET @newTableName = PARSENAME('{1}',1)                                
                                					            
                                SET @primaryKeyName = ''
                                SET @primaryKeyNewName =  REPLACE(NEWID(),'-','_') 
				            
			                    SELECT @primaryKeyName = ind.name
			                    FROM sys.indexes ind INNER JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
			                    INNER JOIN sys.columns col 	ON ic.object_id = col.object_id and ic.column_id = col.column_id 
			                    INNER JOIN sys.tables t ON ind.object_id = t.object_id
			                    WHERE t.name = PARSENAME(@tableName,1) AND is_primary_key = 1
				
			                    IF(ISNULL(@primaryKeyName,'')<>'' AND @primaryKeyName = REPLACE(REPLACE(PARSENAME(@tsStagingTableName,1),'ivp_polaris_','pk_'),'_staging',''))
			                    BEGIN
				                    SELECT @primaryKeyNewName = @primaryKeyName + @primaryKeyNewName
				                    SELECT @primaryKeyName = PARSENAME(@tableName,2)+ '.' + @primaryKeyName
					
				                    EXEC sp_rename @primaryKeyName , @primaryKeyNewName

			                    END 

				                SELECT @primaryKeyName = REPLACE(REPLACE(PARSENAME(@tsStagingTableName,1),'ivp_polaris_','pk_'),'_staging','')

		                    END
	                    END
	              
                    SELECT @sql = ''

                    IF EXISTS(SELECT TOP 1 1 FROM #tsTables WHERE isLeg = 1)
	                    BEGIN
			                PRINT('Rename not required')
		    
	                    END
                        ELSE
	                    BEGIN
		                    SELECT @sql += CHAR(13) + '
		                    
		                    IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '''+ PARSENAME(REPLACE(@tsStagingTableName,'_staging]','_NTN]'),1) +''' AND TABLE_SCHEMA = ''dimension'')
			                    DROP TABLE dimension.'+ PARSENAME(REPLACE(@tsStagingTableName,'_staging]','_NTN]'),1) +'

                            ALTER TABLE '+ @tsStagingTableName +' ADD CONSTRAINT ' + @primaryKeyName + ' PRIMARY KEY(id)

		                    EXEC sp_rename  ''dimension.' + PARSENAME(@tsStagingTableName,1) + ''',''' + REPLACE(PARSENAME(@tsStagingTableName,1),'_staging','_NTN''')

	                    END

                    PRINT(@sql)
                    EXEC (@sql)

                    DROP TABLE #tsTables
                    DROP TABLE #tableColumnsToIgnore
                END TRY
                BEGIN CATCH
	                DECLARE @error VARCHAR(MAX) = ERROR_MESSAGE()
	                RAISERROR(@error,16,1)
                END CATCH
                ", job.TableName, job.IsRef, job.RequireTimeTSReports, job.IsRef ? "entity_type_name" : "security_type_name");

            return SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
        }


        private static void PerformWorkerJobDailyLoader(SRMJobInfo job, string typeId)
        {
            string surogateColumnName = job.TableName.Split('.')[1].Replace("[ivp_polaris_", "").Replace("_daily_staging]", "") + "_daily_surrogate_id";
            StringBuilder sb1 = new StringBuilder();
            if (!job.DateType.Equals(DWHDateType.None))
            {
                sb1.AppendLine("CREATE NONCLUSTERED INDEX idx_2 ON " + job.TableName + "(" + typeId + ")");

                if (job.IsLegReport)
                    sb1.AppendLine("CREATE UNIQUE NONCLUSTERED INDEX idx_4 ON " + job.TableName + "([" + surogateColumnName + "],[Row Id])");

                SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);
            }

            if (!job.IsLegReport && !job.DateType.Equals(DWHDateType.None))
            {
                sb1 = new StringBuilder();
                sb1.AppendLine("ALTER TABLE " + job.TableName + " ADD CONSTRAINT pk_stg_" + surogateColumnName + "_" + DateTime.Now.ToString("yyyyMMdd_HH_mm_ss") + " PRIMARY KEY NONCLUSTERED (" + surogateColumnName + ")");
                SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);
            }

            ExecuteNTSDailyLoader(job, true, true, null);
        }

        private static void ExecuteNTSDailyLoader(SRMJobInfo job, bool requireStagingTableBackup, bool isDailyLoader, RDBConnectionManager conn)
        {
            string stagingTableNameWithoutSchema = job.TableName.Split('.')[1].Replace("[", "").Replace("]", "");
            bool isNoneToNowLoad = job.DateType.Equals(DWHDateType.None) ? true : false;
            string finalTableName = job.TableName.Replace("[references]", "[dimension]").Replace("[taskmanager]", "[dimension]").Replace("_staging]", "]");
            string finalTableNameOriginal = job.TableName.Replace("[references]", "[dimension]").Replace("[taskmanager]", "[dimension]").Replace("_staging]", "]");

            string indexAppendName = finalTableName.Replace("[dimension].", "").Replace("[ivp_polaris_", "").Replace("]", "");
            if (isNoneToNowLoad)
                finalTableName = job.TableName.Replace("[references]", "[dimension]").Replace("[taskmanager]", "[dimension]").Replace("_staging]", "_bakForNTN]");
            string identifierName = job.IsRef ? "entity_code" : "[Security Id]";
            string stagingTableSchemaName = job.IsRef ? "references" : "taskmanager";
            string surogateColumnName = string.Empty;
            if (job.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT))
                surogateColumnName = job.TableName.Split('.')[1].Replace("[ivp_polaris_", "").Replace("_daily_staging]", "") + "_daily_surrogate_id";
            string query = string.Empty;

            if (!isDailyLoader && isNoneToNowLoad)
            {
                query = string.Format(@"DECLARE @primaryKeyName VARCHAR(1000) = ''
				            DECLARE @primaryKeyNewName VARCHAR(1000) = '' 
				            
				            SELECT @primaryKeyName = ind.name
					            FROM sys.indexes ind INNER JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
					            INNER JOIN sys.columns col 	ON ic.object_id = col.object_id and ic.column_id = col.column_id 
					            INNER JOIN sys.tables t ON ind.object_id = t.object_id
					            WHERE t.name = PARSENAME('{0}',1) AND is_primary_key = 1
				
				            IF(ISNULL(@primaryKeyName,'')<>'')
				            BEGIN
					            SELECT @primaryKeyNewName = @primaryKeyName + '_bakForNTN'
					            SELECT @primaryKeyName = 'dimension.'+ @primaryKeyName
					
					            EXEC sp_rename @primaryKeyName , @primaryKeyNewName
				            END", finalTableNameOriginal);

                if (conn == null)
                    SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                else
                    SRMDWHJobExtension.ExecuteQuery(conn, query, SRMDBQueryType.SELECT);


                string notNullCoumns = string.Empty;
                if (job.BlockType.Equals(BLOCK_TYPES.NTS_REPORT) && job.IsLegReport)
                    notNullCoumns = "'master_id','Row Id'";
                else if (job.BlockType.Equals(BLOCK_TYPES.NTS_REPORT))
                    notNullCoumns = "'master_id'";
                else if (job.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT) && job.IsLegReport)
                    notNullCoumns = "'master_id','Row Id','Effective Date','" + surogateColumnName + "'";
                else if (job.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT))
                    notNullCoumns = "'master_id','Effective Date','" + surogateColumnName + "'";

                query = string.Format(@"DECLARE @sql VARCHAR(MAX) = ''
                    SELECT @sql = 'IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME(''{0}'',1) AND TABLE_SCHEMA = ''dimension'')
				    BEGIN
						    DROP TABLE {0}
				    END '
                    EXEC (@sql)
                    SELECT @sql = ''
                    SELECT @sql += 
	                    CASE WHEN @sql <> '' THEN ',' ELSE '' END +
		                    '[' + COLUMN_NAME + '] ' + DATA_TYPE + 
		                    CASE WHEN DATA_TYPE = 'VARCHAR' AND CHARACTER_MAXIMUM_LENGTH != -1
				                    THEN ' (' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
			                    WHEN DATA_TYPE = 'VARCHAR'
				                    THEN ' (MAX)'
			                    WHEN DATA_TYPE IN ('NUMERIC','DECIMAL')
				                    THEN ' (' + CAST(NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(NUMERIC_SCALE AS VARCHAR)+ ')'
		                    ELSE '' 
		                    END + 
		                    CASE WHEN COLUMN_NAME IN ({3}) THEN ' NOT NULL ' ELSE '' END
                    FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{1}' AND TABLE_SCHEMA = '{2}' ORDER BY ORDINAL_POSITION

                    

                    SELECT @sql = ' CREATE TABLE {0} (' + @sql + ',INDEX cci CLUSTERED COLUMNSTORE)'

                    EXEC (@sql)", finalTableName, stagingTableNameWithoutSchema, stagingTableSchemaName, notNullCoumns);

                DataSet ds = SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);

                query = string.Format(@"
                    DECLARE @sql VARCHAR(MAX) = ''

                    SELECT @sql += 
	                    CASE WHEN @sql <> '' THEN ',' ELSE '' END +
		                    '[' + COLUMN_NAME + '] ' 
                    FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{2}' AND TABLE_SCHEMA = '{3}' ORDER BY ORDINAL_POSITION

                    SELECT @sql = 'INSERT INTO {0} WITH (TABLOCKX) (' + @sql + ') 
                                    SELECT ' + @sql + ' FROM {1}'

                    EXEC (@sql)", finalTableName, job.TableName, stagingTableNameWithoutSchema, stagingTableSchemaName);

                if (job.BlockType.Equals(BLOCK_TYPES.NTS_REPORT) && job.IsLegReport)
                {
                    query = query + string.Format(" DELETE FROM {0} WHERE is_leg_deleted = 1", finalTableName);
                }

                if (conn == null)
                    SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                else
                    SRMDWHJobExtension.ExecuteQuery(conn, query, SRMDBQueryType.SELECT);
            }

            else
            {
                query = string.Empty;
                string joinCondition = "tab.master_id = tab2.master_id";
                if (job.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT) && job.IsLegReport)
                    joinCondition = "tab." + surogateColumnName + " = tab2." + surogateColumnName + " AND tab.[Row Id] = tab2.[Row Id]";
                else if (job.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT))
                    joinCondition = "tab." + surogateColumnName + " = tab2." + surogateColumnName;

                if (job.BlockType.Equals(BLOCK_TYPES.NTS_REPORT) && job.IsLegReport)
                {
                    query = string.Format(@" 
                        DELETE tab2 FROM {1} tab2
                        INNER JOIN {0} tab
                        ON (tab2.master_id = tab.master_id)
                        DELETE FROM {0} WHERE is_leg_deleted = 1", job.TableName, finalTableName);
                }

                query = query + string.Format(@" DECLARE @sql VARCHAR(MAX) = ''
                    DECLARE @sql2 VARCHAR(MAX) = ''
                    DECLARE @sql3 VARCHAR(MAX) = ''

                    SELECT @sql += 
	                    CASE WHEN @sql <> '' THEN ',' ELSE '' END +
		                    '[' + COLUMN_NAME + '] ' 
                    FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{4}' AND TABLE_SCHEMA = '{5}' ORDER BY ORDINAL_POSITION

                    SELECT @sql2 += 
	                    CASE WHEN @sql2 <> '' THEN ',' ELSE '' END +
		                    'tab.[' + COLUMN_NAME + '] ' 
                    FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{4}' AND TABLE_SCHEMA = '{5}' ORDER BY ORDINAL_POSITION

                    SELECT @sql3 += 
	                    CASE WHEN @sql3 <> '' THEN ',' ELSE '' END +
		                    'tab2.[' + COLUMN_NAME + ']  = tab.[' + COLUMN_NAME + '] ' 
                    FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{4}' AND TABLE_SCHEMA = '{5}' ORDER BY ORDINAL_POSITION


                    SELECT @sql = 'MERGE {0} AS tab2
                                   USING (SELECT * FROM {1}) AS tab
                                   ON ({2})
                                   WHEN MATCHED THEN UPDATE SET ' + @sql3 + '
                                   WHEN NOT MATCHED THEN INSERT ('+ @sql +')                    
                                   VALUES (' + @sql2 + ');'

                    EXEC (@sql)", finalTableName, job.TableName, joinCondition, identifierName, stagingTableNameWithoutSchema, stagingTableSchemaName);

                if (conn == null)
                    SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                else
                    SRMDWHJobExtension.ExecuteQuery(conn, query, SRMDBQueryType.SELECT);
            }

            if (!isDailyLoader && isNoneToNowLoad)
            {
                if (job.BlockType.Equals(BLOCK_TYPES.NTS_REPORT) && !job.IsLegReport)
                {
                    query = string.Format(@"ALTER TABLE {0} ADD CONSTRAINT pk_{1} PRIMARY KEY NONCLUSTERED (master_id)", finalTableName, indexAppendName);
                }
                else if (job.BlockType.Equals(BLOCK_TYPES.NTS_REPORT) && job.IsLegReport)
                {
                    query = string.Format(@"CREATE NONCLUSTERED INDEX idx_{0} ON {1} (master_id)", indexAppendName, finalTableName);
                }
                else if (job.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT) && !job.IsLegReport)
                {
                    query = string.Format(@"ALTER TABLE {0} ADD CONSTRAINT pk_{1} PRIMARY KEY NONCLUSTERED ({2})", finalTableName, indexAppendName, surogateColumnName);
                    query = query + string.Format(@" CREATE NONCLUSTERED INDEX idx_{0} ON {1} (master_id)", indexAppendName, finalTableName);
                    query = query + string.Format(@" CREATE UNIQUE NONCLUSTERED INDEX idx_1_{0} ON {1} ([Effective Date],master_id)", indexAppendName, finalTableName);
                }
                else if (job.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT) && job.IsLegReport)
                {
                    query = string.Format(@"CREATE UNIQUE NONCLUSTERED INDEX idx_{0} ON {1} ({2}, [Row Id])", indexAppendName, finalTableName, surogateColumnName);
                    query = query + string.Format(@" CREATE NONCLUSTERED INDEX idx_1_{0} ON {1} ([Effective Date],master_id)", indexAppendName, finalTableName);
                    query = query + string.Format(@" CREATE NONCLUSTERED INDEX idx_2_{0} ON {1} ([Effective Date],is_leg_deleted)", indexAppendName, finalTableName);
                }
                if (conn == null)
                    SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                else
                    SRMDWHJobExtension.ExecuteQuery(conn, query, SRMDBQueryType.SELECT);

                query = string.Format(@"
                                    DECLARE @query VARCHAR(MAX) = ''
                                    DECLARE @bakTableName VARCHAR(1000) = PARSENAME('{0}',1)
                                    DECLARE @originalTableName VARCHAR(1000) = PARSENAME('{1}',1)
                                    DECLARE @tableName VARCHAR(1000) = 'dimension.'+@bakTableName
				                    SELECT @query = 
				                    'IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '''+ @originalTableName +''' AND TABLE_SCHEMA = ''dimension'')
				                    BEGIN
						                    DROP TABLE dimension.[' + @originalTableName + ']
				                    END'
				                    EXEC(@query)
				                    EXEC sp_rename @tableName , @originalTableName", finalTableName, finalTableNameOriginal);
                if (conn == null)
                    SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                else
                    SRMDWHJobExtension.ExecuteQuery(conn, query, SRMDBQueryType.SELECT);
            }

            if (requireStagingTableBackup)
            {
                query = string.Format(@"
                                DECLARE @tableName VARCHAR(1000) = '{0}'
                                DECLARE @newTableName VARCHAR(1000) = PARSENAME('{1}',1)                                
                                					            
                                DECLARE @primaryKeyName VARCHAR(1000) = ''
                                DECLARE @primaryKeyNewName VARCHAR(1000) = '{2}' 
				            
                                SELECT @primaryKeyName = ind.name
                                FROM sys.indexes ind INNER JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
                                INNER JOIN sys.columns col 	ON ic.object_id = col.object_id and ic.column_id = col.column_id 
                                INNER JOIN sys.tables t ON ind.object_id = t.object_id
                                WHERE t.name = PARSENAME(@tableName,1) AND is_primary_key = 1
				
                                IF(ISNULL(@primaryKeyName,'')<>'')
                                BEGIN
	                                SELECT @primaryKeyNewName = @primaryKeyName + @primaryKeyNewName
	                                SELECT @primaryKeyName = PARSENAME(@tableName,2)+ '.' + @primaryKeyName
					
	                                EXEC sp_rename @primaryKeyName , @primaryKeyNewName
	                                
                                END 

                                EXEC sp_rename @tableName , @newTableName

                                ", job.TableName,
                                (job.TableName.Substring(0, job.TableName.Length - 1) + job.LoadingTimeForStagingTable.ToString("dd_MM_yyyy_HH_mm_ss") + "]"),
                                job.LoadingTimeForStagingTable.ToString("dd_MM_yyyy_HH_mm_ss"));
                if (conn == null)
                    SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                else
                    SRMDWHJobExtension.ExecuteQuery(conn, query, SRMDBQueryType.SELECT);
            }
        }

        private static void UpdateLastRunDate(SRMJobInfo job)
        {
            string query = string.Format(@"
                            DELETE FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_reporting_time WHERE setup_id = {0} AND block_id = {1}
                            INSERT INTO IVPRefMaster.dbo.ivp_srm_dwh_downstream_reporting_time VALUES ({0},{1},'{2}')", job.SetupId, job.BlockId, job.EndDate);

            CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
        }


        private static void DeleteInactiveInstruments(SRMJobInfo job)
        {
            if (!job.RequireDeletedRows && !job.DateType.Equals(DWHDateType.None))
            {
                InsertStatusForBlockDetails(job.BlockStatusId, "Deleting Inactive Instruments Started", job.UserName);
                DeleteInactiveRows(job);
                InsertStatusForBlockDetails(job.BlockStatusId, "Deleting Inactive Instruments Completed", job.UserName);
            }
        }

        private static void InsertStatusForBlockDetails(int blockStatusId, string message, string userName)
        {
            if (blockStatusId == 0)
                return;
            if (message.Length > 0)
            {
                int maxLength = message.Length - 1;
                if (maxLength > 8000)
                {
                    maxLength = 8000;
                    message = message.Substring(0, maxLength);
                }
                message = message.Replace("'", "''");
            }

            string query = @"INSERT INTO [dbo].[ivp_srm_dwh_downstream_block_status_detail] (block_status_id,message,is_active,created_on)
                            VALUES (" + blockStatusId + ",'" + message + "',1,GETDATE())";
            CommonDALWrapper.ExecuteQuery(query, CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
        }



        private static void GetSecMasterDataAndPushToStagingTables(SRMJobInfo job)
        {
            string dateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + System.Text.RegularExpressions.Regex.Replace(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern, "(:ss|:s)", "$1.fff");
            //Start and End Date to be in server format for SM
            job.StartDate = DateTime.ParseExact(job.StartDate, "yyyyMMdd HH:mm:ss.fff", null).ToString(dateFormat);
            job.EndDate = DateTime.ParseExact(job.EndDate, "yyyyMMdd HH:mm:ss.fff", null).ToString(dateFormat);

            if (job.BlockType == BLOCK_TYPES.NTS_REPORT || job.BlockType == BLOCK_TYPES.TS_REPORT)
            {
                Assembly SecmTaskManagerAssembly = Assembly.Load("SecMasterTaskManager");
                Type objType = SecmTaskManagerAssembly.GetType("com.ivp.secm.taskmanager.TaskHandlers.SMDWHController");
                MethodInfo ExecuteDWHReport = objType.GetMethod("ExecuteDWHReport", BindingFlags.Public | BindingFlags.Static);
                ExecuteDWHReport.Invoke(null, new object[] { job });
            }
            else
            {
                Assembly SecmFreqReportingAssembly = Assembly.Load("SecMasterFrequencyReport");

                Type objType = SecmFreqReportingAssembly.GetType("com.ivp.secm.SecMasterFrequencyReport.SecMasterFrequencyReport");
                object objTypeInstance = Activator.CreateInstance(objType);

                Type objTypeSMFrequencyReportInfo = SecmFreqReportingAssembly.GetType("com.ivp.secm.SecMasterFrequencyReport.SMFrequencyReportInfo");
                object objInstanceSMFrequencyReportInfo = Activator.CreateInstance(objTypeSMFrequencyReportInfo);

                PropertyInfo propInfo = objTypeSMFrequencyReportInfo.GetProperty("jobInfo");
                propInfo.SetValue(objInstanceSMFrequencyReportInfo, job, null);

                MethodInfo GetReportData = objType.GetMethod("Execute", new Type[] { objTypeSMFrequencyReportInfo });
                GetReportData.Invoke(objTypeInstance, new object[] { objInstanceSMFrequencyReportInfo });
            }
        }

        private static void GetRefMasterDataAndPushToStagingTables(SRMJobInfo job)
        {
            Assembly RMControllerAssembly = Assembly.Load("RefMController");
            object reportInfo;
            GetRefMasterReportInfoById(job, RMControllerAssembly, out reportInfo);
            DataSet dsReportData = null;
            bool syncTables = true;
            List<string> columnsToRemoveTimeComponent = new List<string>();
            if (!job.RequireTimeTSReports && job.BlockType.Equals(BLOCK_TYPES.TS_REPORT))
                columnsToRemoveTimeComponent = new List<string>() { "Effective Start Date", "Effective End Date" };
            if (job.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT))
                columnsToRemoveTimeComponent = new List<string>() { "Effective Date" };

            job.ModifiedEntities.UnionWith(job.ModifiedEntitiesWithManualUpdates);
            job.ModifiedEntities.UnionWith(job.ModifiedEntitiesWithTSUpdates);

            if (job.RequireBatching && job.BatchSize > 0)
            {
                if (job.ModifiedEntities.Count > 0)
                {
                    Queue<HashSet<string>> batches = new Queue<HashSet<string>>();
                    GetBatches(job.BatchSize, job.ModifiedEntities, batches);

                    int batchCounter = 0, totalBatchCount = batches.Count;
                    if (batches.Count > 0)
                    {
                        do
                        {
                            if (batchCounter == 0)
                                job.FirstBatch = true;
                            else
                                job.FirstBatch = false;

                            batchCounter++;

                            InsertStatusForBlockDetails(job.BlockStatusId, "Fetching Report Data for batch : " + (batchCounter) + " out of : " + totalBatchCount, job.UserName);

                            HashSet<string> currentBatchEntityCodes = batches.Dequeue();

                            dsReportData = GetRefMasterReportData(job, reportInfo, string.Join(",", currentBatchEntityCodes));
                            if (dsReportData != null && dsReportData.Tables.Count > 0 && dsReportData.Tables[0].Rows.Count > 0)
                            {
                                PushDataToStagingTables(dsReportData.Tables[0], columnsToRemoveTimeComponent, job, syncTables);
                                syncTables = false;
                            }
                        }
                        while (batches.Count > 0);
                    }
                }
            }
            else
            {
                if (job.ModifiedEntities.Count > 0)
                {
                    job.FirstBatch = true;
                    dsReportData = GetRefMasterReportData(job, reportInfo, string.Empty);
                    if (dsReportData != null && dsReportData.Tables.Count > 0 && dsReportData.Tables[0].Rows.Count > 0)
                        PushDataToStagingTables(dsReportData.Tables[0], columnsToRemoveTimeComponent, job, job.FirstBatch);
                }
            }
        }

        private static void GetBatches(int batchSize, HashSet<string> lst, Queue<HashSet<string>> batches)
        {
            var batchSizeCounter = 0;
            HashSet<string> temp = null;
            var totalEntityCounter = 0;
            foreach (var entityCode in lst)
            {
                if (batchSizeCounter == 0)
                    temp = new HashSet<string>();
                temp.Add(entityCode);
                batchSizeCounter++;
                totalEntityCounter++;

                if (batchSizeCounter == batchSize || totalEntityCounter == lst.Count)
                {
                    batchSizeCounter = 0;
                    batches.Enqueue(temp);
                }
            }
        }

        private static void ExecuteCustomClass(SRMJobInfo job)
        {
            if (!string.IsNullOrEmpty(job.CustomClassAssemblyName) && !string.IsNullOrEmpty(job.CustomClassMethodName)
                && !string.IsNullOrEmpty(job.CustomClassName))
            {
                try
                {
                    InsertStatusForBlockDetails(job.BlockStatusId, "Custom Class Execution Started", job.UserName);
                    mLogger.Debug("Execute Custom Class Start -> Assembly Name-" + job.CustomClassAssemblyName);
                    OrderedDictionary hCustomClassParam = new OrderedDictionary();
                    hCustomClassParam.Add("DownstreamSQLConnectionName", job.DownstreamSQLConnectionName);
                    hCustomClassParam.Add("StagingTableName", job.TableName);
                    hCustomClassParam.Add("IsRef", job.IsRef);
                    hCustomClassParam.Add("IsLegReport", job.IsLegReport);
                    hCustomClassParam.Add("BlockType", job.BlockType);
                    RCustomCallExecutor.ExecuteCustomCall(job.CustomClassAssemblyName, job.CustomClassMethodName, hCustomClassParam);
                    mLogger.Debug("Execute Custom Class End -> Assembly Name-" + job.CustomClassAssemblyName);
                    InsertStatusForBlockDetails(job.BlockStatusId, "Custom Class Execution Completed", job.UserName);
                }
                catch (Exception ex)
                {
                    InsertStatusForBlockDetails(job.BlockStatusId, "Custom Class Execution Exception : " + ex.Message, job.UserName);
                    mLogger.Error("Execute Custom Class Error -> Assembly Name -" + job.CustomClassAssemblyName);
                    mLogger.Error(ex);
                }
            }
        }

        private static void PushMessageToQueue(SRMJobInfo job)
        {
            if (!string.IsNullOrEmpty(job.QueueName))
            {
                try
                {
                    InsertStatusForBlockDetails(job.BlockStatusId, "Pushing to Message Queue Started", job.UserName);
                    mLogger.Debug("PushMessageToQueue Start -> QueueName-" + job.QueueName);
                    string postingText = "Data pushed to " + job.TableName;
                    IRTransport trans = new RTransportManager().GetTransport(job.QueueName);
                    trans.SendMessage(postingText.ToString());
                    mLogger.Debug("PushMessageToQueue End -> QueueName-" + job.QueueName);
                    InsertStatusForBlockDetails(job.BlockStatusId, "Pushing to Message Queue Completed", job.UserName);

                }
                catch (Exception ex)
                {
                    mLogger.Error("PushMessageToQueue Error -> QueueName -" + job.QueueName);
                    mLogger.Error(ex);
                    InsertStatusForBlockDetails(job.BlockStatusId, "Pushing to Message Queue Failed : " + ex.Message, job.UserName);
                }
            }
        }

        private static void SetLogger(bool isRefM, BLOCK_TYPES blockType, string setupName)
        {
            //Write in Manual file in case of error. and throw
            string fileName = string.Empty;
            List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
            foreach (XmlDocument loggerDoc in lstConfig)
            {
                XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
                    {
                        fileName = node.Attributes["value"].Value;
                    }
                }
            }

            string directoryName = System.IO.Directory.GetParent(fileName).FullName;
            directoryName = directoryName + "\\DWH Worker Log\\" + DateTime.Now.ToString("yyyyMMdd");
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            string jobLoggingPath = directoryName + "\\" + "DWHWorkerProcess" + "_" + setupName + "_" + (isRefM ? "RefM" : "SecM") + "_" + blockType + "_" + DateTime.Now.ToString("HH-mm-ss.fff") + ".log";

            foreach (XmlDocument loggerDoc in lstConfig)
            {
                XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
                foreach (XmlNode node in nodeList)
                {
                    if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
                        node.Attributes["value"].Value = jobLoggingPath;
                }
            }
            mLogger = RLogFactory.CreateLogger("SRMDWHJobWorker", lstConfig[0].InnerXml);
            mLogger.Debug("Logger Initialized");
        }

        private static DataSet GetRefMasterReportData(SRMJobInfo job, object reportInfo, string entityCodes)
        {
            DataSet dsReportData = null;
            try
            {
                mLogger.Debug("GetRefMasterReportData -> Start");
                Assembly RMReportingAssembly = Assembly.Load("RefMReporting");

                int reportTypeId = 0;
                if (job.BlockType == BLOCK_TYPES.NTS_REPORT)
                    reportTypeId = 1;
                else if (job.BlockType == BLOCK_TYPES.DAILY_REPORT || job.BlockType == BLOCK_TYPES.TS_REPORT)
                    reportTypeId = 6;

                Type objType = RMReportingAssembly.GetType("com.ivp.refmaster.reporting.RMAcrossEntityTypeReport");
                object objTypeInstance = Activator.CreateInstance(objType);
                MethodInfo GetReportData = objType.GetMethod("GetReportData");

                PropertyInfo propInfo = objType.GetProperty("ReportTypeId");
                propInfo.SetValue(objTypeInstance, reportTypeId, null);

                propInfo = objType.GetProperty("DataSetName");
                PropertyInfo pi = reportInfo.GetType().GetProperty("ReportName");
                String value = (String)(pi.GetValue(reportInfo, null));
                propInfo.SetValue(objTypeInstance, value, null);

                propInfo = objType.GetProperty("ReportName");
                pi = reportInfo.GetType().GetProperty("ReportName");
                value = (String)(pi.GetValue(reportInfo, null));
                propInfo.SetValue(objTypeInstance, value, null);

                propInfo = objType.GetProperty("IsDownstreamSyncingReport");
                propInfo.SetValue(objTypeInstance, true, null);

                propInfo = objType.GetProperty("DwhJobInfo");
                propInfo.SetValue(objTypeInstance, job, null);

                if (!string.IsNullOrEmpty(entityCodes))
                {
                    propInfo = objType.GetProperty("EntityCodes");
                    propInfo.SetValue(objTypeInstance, entityCodes, null);
                }
                dsReportData = (DataSet)GetReportData.Invoke(objTypeInstance, new object[] { });
                mLogger.Debug("GetRefMasterReportData -> End");
            }
            catch (Exception ex)
            {
                mLogger.Error("GetRefMasterReportData -> Error : " + ex);
                throw;
            }
            return dsReportData;
        }

        private static void GetRefMasterReportInfoById(SRMJobInfo job, Assembly RMControllerAssembly, out object reportInfo)
        {
            Type objType = RMControllerAssembly.GetType("com.ivp.refmaster.controller.RMReportRepositoryController");
            object obj = Activator.CreateInstance(objType);
            MethodInfo methodInfo = objType.GetMethod("GetReportInfoById");
            reportInfo = methodInfo.Invoke(obj, new object[] { job.ReportId });
        }



        #region PushData
        public static void PushDataToStagingTables(DataTable dataTable, List<string> columnNamesToRemoveTimeComponent, SRMJobInfo jobInfo, bool dropTables)
        {
            InsertStatusForBlockDetails(jobInfo.BlockStatusId, "Pushing Data to Staging Table", jobInfo.UserName);
            PushDataToStagingTables(SRMDWHJobExtension.ConvertDataTableToObjectTable(dataTable, 0, dataTable.Rows.Count), columnNamesToRemoveTimeComponent, jobInfo, dropTables);
        }

        public static void PushDataToStagingTables(ObjectTable dataTable, List<string> columnNamesToRemoveTimeComponent, SRMJobInfo jobInfo, bool dropTables)
        {
            mLogger.Debug("PushDataToStagingTables -> Start");
            List<string> tablesToBeDropped = new List<string>();

            jobInfo.ReferenceAttributes = jobInfo.ReferenceAttributes ?? new HashSet<string>();
            jobInfo.ReferenceDailyAttributes = jobInfo.ReferenceDailyAttributes ?? new HashSet<string>();

            var columnsInTable = new HashSet<string>(dataTable.Columns.Cast<ObjectColumn>().Select(x => x.ColumnName), StringComparer.OrdinalIgnoreCase);
            var refAttrsInTable = new HashSet<string>(jobInfo.ReferenceAttributes.Where(x => columnsInTable.Contains(x)), StringComparer.OrdinalIgnoreCase);
            var undAttrsInTable = new HashSet<string>(jobInfo.UnderlyingAttributes.Where(x => columnsInTable.Contains(x)), StringComparer.OrdinalIgnoreCase);

            if (!dctInstrumentTypeVsObject.ContainsKey(jobInfo.InstrumentTypeId))
                dctInstrumentTypeVsObject[jobInfo.InstrumentTypeId] = new object();
            object lck = dctInstrumentTypeVsObject[jobInfo.InstrumentTypeId];

            lock (lck)
                InsertMasterIdInDB(dataTable, jobInfo, refAttrsInTable, undAttrsInTable);

            string schemaName = jobInfo.IsRef ? "[references]" : "[taskmanager]";
            string destinationTableName = jobInfo.TableName.Replace(schemaName + ".", string.Empty);
            string destinationTableNameWithSchema = jobInfo.TableName;
            string typeName = jobInfo.IsRef ? "[Entity Type Name]" : "[Security Type Name]";
            string typeId = jobInfo.IsRef ? "[entity_code]" : "[Security Id]";
            string typeIdToPopulate = jobInfo.IsRef ? "entity_code" : "Security Id";
            string typeIdToPopulateMasterId = jobInfo.IsRef ? "entity_code" : "Security Key";
            string surrogateColumnn = dataTable.TableName.Replace(" ", "_").ToLower() + "_surrogate_id";
            string timeSeriesIdColumnn = "timeseries_" + dataTable.TableName.Replace(" ", "_").ToLower() + "_id";
            HashSet<string> columnNamesNotMassaged = new HashSet<string>();
            HashSet<string> masterIdsNotMassaged = new HashSet<string>();
            Dictionary<string, string> refAttrVsSurrogateColumnName = new Dictionary<string, string>();
            Dictionary<string, string> underlyingAttrVsSurrogateColumnName = new Dictionary<string, string>();

            Dictionary<string, string> dctTsTableColumnRename = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> dctTsTableColumnAdd = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            List<string> lstTSTableColumnDrop = new List<string>();
            List<ObjectRow> rowsToDelete = new List<ObjectRow>();

            HashSet<string> hiddenAttributes = null;
            var reportAttributeVsSchema = SRMDownstreamController.GetReportAttributesSchema(jobInfo.ReportId, jobInfo.IsRef, out hiddenAttributes);
            foreach (var attr in hiddenAttributes)
                reportAttributeVsSchema.Remove(attr);

            Dictionary<string, Dictionary<DateTime, int>> instIdVsDateVsTimeSeriesId = new Dictionary<string, Dictionary<DateTime, int>>();

            if (jobInfo.BlockType == BLOCK_TYPES.DAILY_REPORT && !string.IsNullOrEmpty(jobInfo.TimeSeriesReportName))
            {
                string securityEntityCode = jobInfo.IsRef ? "entity_code" : "Security Id";
                HashSet<string> instrumentsInDaily = dataTable.AsEnumerable().Select(x => Convert.ToString(x[securityEntityCode])).ToHashSet();
                if (jobInfo.InstrumentsInTSWithTSUpdates.Count > 0)
                {
                    HashSet<string> instruments = instrumentsInDaily.Intersect(jobInfo.InstrumentsInTSWithTSUpdates).ToHashSet();

                    instrumentsInDaily = instrumentsInDaily.Except(instruments).ToHashSet();

                    ObjectTable dtInstIds = new ObjectTable();
                    dtInstIds.Columns.Add("instrument_id", typeof(string));
                    foreach (var instId in instruments)
                        dtInstIds.Rows.Add(instId);

                    //Get Data from Dimension Table
                    string tableName = string.Empty;
                    string tsDimensionTableName = string.Empty;
                    bool requireColumnRename = false;
                    if (jobInfo.TSReportDateType == DWHDateType.None)
                        tsDimensionTableName = jobInfo.TimeSeriesTableNameWithSchema;
                    else
                    {
                        requireColumnRename = true;
                        if (jobInfo.IsRef)
                            tsDimensionTableName = jobInfo.TimeSeriesTableNameWithSchema.Replace("[dimension]", "[references]");
                        else
                            tsDimensionTableName = jobInfo.TimeSeriesTableNameWithSchema.Replace("[dimension]", "[taskmanager]");
                    }
                    tsDimensionTableName = tsDimensionTableName.Replace("_time_series]", "_time_series_staging]");

                    mLogger.Debug("tsDimensionTableName : " + tsDimensionTableName);
                    mLogger.Debug("jobInfo.TSReportDateType : " + jobInfo.TSReportDateType);
                    mLogger.Debug("requireColumnRename : " + requireColumnRename);

                    instIdVsDateVsTimeSeriesId = CreateTimeSeriesLookUpCollection(jobInfo, ref tableName, dtInstIds, null, jobInfo, tsDimensionTableName, requireColumnRename);
                    tablesToBeDropped.Add(tableName);
                }

                if (jobInfo.InstrumentsInTSWithManualUpdate.Count > 0)
                {
                    HashSet<string> instruments = instrumentsInDaily.Intersect(jobInfo.InstrumentsInTSWithManualUpdate).ToHashSet();

                    instrumentsInDaily = instrumentsInDaily.Except(instruments).ToHashSet();
                    tablesToBeDropped.AddRange(MergeTSLookupCollectionForManualAndTSUpdates(jobInfo, instruments, instIdVsDateVsTimeSeriesId));
                }

                if (instrumentsInDaily.Count > 0)
                {
                    ObjectTable dtInstIds = new ObjectTable();
                    dtInstIds.Columns.Add("instrument_id", typeof(string));
                    foreach (var instId in instrumentsInDaily)
                        dtInstIds.Rows.Add(instId);

                    string tsDimensionTableName = string.Empty;
                    if (jobInfo.DateType == DWHDateType.None)
                        tsDimensionTableName = jobInfo.TimeSeriesTableNameWithSchema;

                    string tableName = string.Empty;
                    Dictionary<string, Dictionary<DateTime, int>> dctData = CreateTimeSeriesLookUpCollection(jobInfo, ref tableName, dtInstIds, null, jobInfo, tsDimensionTableName, false);
                    tablesToBeDropped.Add(tableName);

                    foreach (var dctinstIdVsDateVsTimeSeriesId in dctData)
                    {
                        if (instIdVsDateVsTimeSeriesId.ContainsKey(dctinstIdVsDateVsTimeSeriesId.Key))
                        {
                            foreach (var dctDateVsTimeSeriesId in dctinstIdVsDateVsTimeSeriesId.Value)
                            {
                                if (instIdVsDateVsTimeSeriesId[dctinstIdVsDateVsTimeSeriesId.Key].ContainsKey(dctDateVsTimeSeriesId.Key))
                                    instIdVsDateVsTimeSeriesId[dctinstIdVsDateVsTimeSeriesId.Key][dctDateVsTimeSeriesId.Key] = dctDateVsTimeSeriesId.Value;
                                else
                                    instIdVsDateVsTimeSeriesId[dctinstIdVsDateVsTimeSeriesId.Key].Add(dctDateVsTimeSeriesId.Key, dctDateVsTimeSeriesId.Value);
                            }
                        }
                        else
                            instIdVsDateVsTimeSeriesId.Add(dctinstIdVsDateVsTimeSeriesId.Key, dctinstIdVsDateVsTimeSeriesId.Value);
                    }
                }
            }

            if (jobInfo.BlockType == BLOCK_TYPES.TS_REPORT && jobInfo.DateType == DWHDateType.None)
                PopulateTSColumnDictionary(dctTsTableColumnRename, dctTsTableColumnAdd, lstTSTableColumnDrop, jobInfo);

            if (!dataTable.Columns.Contains("loading_time"))
            {
                var dc = new ObjectColumn("loading_time", typeof(System.DateTime));
                dc.DefaultValue = jobInfo.LoadingTimeForStagingTable;
                dataTable.Columns.Add(dc);
            }

            if (jobInfo.BlockType.Equals(BLOCK_TYPES.NTS_REPORT) && !dataTable.Columns.Contains("is_deleted"))
            {
                var dc = new ObjectColumn("is_deleted", typeof(System.Boolean));
                dc.DefaultValue = false;
                dataTable.Columns.Add(dc);
            }

            if (!dataTable.Columns.Contains("master_id"))
            {
                var dc = new ObjectColumn("master_id", typeof(System.Int32));
                dataTable.Columns.Add(dc);
            }
            if (jobInfo.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT) && !dataTable.Columns.Contains(surrogateColumnn))
            {
                var dc = new ObjectColumn(surrogateColumnn, typeof(System.Int64));
                dataTable.Columns.Add(dc);
            }
            if (jobInfo.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT) && !dataTable.Columns.Contains(timeSeriesIdColumnn) && !string.IsNullOrEmpty(jobInfo.TimeSeriesReportName))
            {
                var dc = new ObjectColumn(timeSeriesIdColumnn, typeof(System.Int32));
                dataTable.Columns.Add(dc);
            }
            var tableSplit = destinationTableNameWithSchema.Split(new char[] { '[', '.' }, StringSplitOptions.RemoveEmptyEntries);
            var canAlterTable = SRMDWHJob.CanAlterTable(jobInfo.DownstreamSQLConnectionName, tableSplit[1].Replace("_staging]", "").Replace("]", ""));

            var processLookupFromDaily = jobInfo.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT) || (jobInfo.BlockType.Equals(BLOCK_TYPES.TS_REPORT) && (jobInfo.RequireTimeTSReports || canAlterTable));

            columnNamesToRemoveTimeComponent = columnNamesToRemoveTimeComponent ?? new List<string>();

            foreach (string attr in refAttrsInTable)
            {
                string newColumnName = (jobInfo.IsRef ? attr.Replace("EC-", "") : attr) + "_surrogate_id";
                if (jobInfo.BlockType.Equals(BLOCK_TYPES.TS_REPORT))
                    newColumnName = attr + "_surrogate_id";
                refAttrVsSurrogateColumnName.Add(attr, newColumnName);
                if (!dataTable.Columns.Contains(newColumnName))
                {
                    var dc = new ObjectColumn(newColumnName, typeof(System.Int64));
                    dataTable.Columns.Add(dc);
                }
            }
            foreach (string attr in undAttrsInTable)
            {
                string newColumnName = (jobInfo.IsRef ? attr.Replace("EC-", "") : attr) + "_surrogate_id";
                underlyingAttrVsSurrogateColumnName.Add(attr, newColumnName);
                if (!dataTable.Columns.Contains(newColumnName))
                {
                    var dc = new ObjectColumn(newColumnName, typeof(System.Int64));
                    dataTable.Columns.Add(dc);
                }
            }

            var today = DateTime.Today.ToString("yyyyMMdd");

            foreach (var row in dataTable.AsEnumerable())
            {
                string id;
                DateTime effectiveDate = default(DateTime);
                string effectiveDateString = null;

                if (processLookupFromDaily)
                {

                    if (jobInfo.BlockType == BLOCK_TYPES.DAILY_REPORT)
                    {
                        effectiveDate = Convert.ToDateTime(row["Effective Date"]).Date;
                        effectiveDateString = effectiveDate.ToString("yyyyMMdd");
                    }
                    else if (processLookupFromDaily)
                    {
                        if (row["Effective End Date"] != DBNull.Value)
                        {
                            effectiveDate = Convert.ToDateTime(row["Effective End Date"]).Date;
                            effectiveDateString = effectiveDate.ToString("yyyyMMdd");
                        }
                        else
                            effectiveDateString = today;
                    }
                }

                if (!jobInfo.RequireTimeTSReports)
                {
                    foreach (var column in columnNamesToRemoveTimeComponent)
                    {
                        if (row[column] != DBNull.Value)
                            row[column] = Convert.ToDateTime(row[column]).Date;
                    }
                }
                foreach (var column in refAttrsInTable)
                {
                    var value = Convert.ToString(row[column]);
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        bool foundValue = false;
                        if (processLookupFromDaily)
                        {
                            if (jobInfo.ReferenceDailyAttributes.Contains(column))
                            {
                                if (EntityCodeVsMasterId.TryGetValue(value, out id))
                                {
                                    foundValue = true;
                                    row[refAttrVsSurrogateColumnName[column]] = effectiveDateString + id;
                                }
                                else
                                    columnNamesNotMassaged.Add(column);
                            }
                        }

                        if (!foundValue)
                        {
                            if (EntityCodeVsMasterId.TryGetValue(value, out id))
                                row[refAttrVsSurrogateColumnName[column]] = id;
                            else
                                columnNamesNotMassaged.Add(column);
                        }
                    }
                }

                foreach (var column in undAttrsInTable)
                {
                    var value = Convert.ToString(row[column]);
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (processLookupFromDaily)
                        {
                            if (SecurityIdVsMasterId.TryGetValue(value, out id))
                                row[underlyingAttrVsSurrogateColumnName[column]] = effectiveDateString + id;
                            else
                                columnNamesNotMassaged.Add(column);
                        }
                        else
                        {
                            if (SecurityIdVsMasterId.TryGetValue(value, out id))
                                row[underlyingAttrVsSurrogateColumnName[column]] = id;
                            else
                                columnNamesNotMassaged.Add(column);
                        }
                    }
                }

                string entityCode = Convert.ToString(row[typeIdToPopulateMasterId]);
                string typeIdToPopulateTSId = Convert.ToString(row[typeIdToPopulate]);

                if ((jobInfo.IsRef && EntityCodeVsMasterId.TryGetValue(entityCode, out id)) || (!jobInfo.IsRef && SecurityIdVsMasterId.TryGetValue(entityCode, out id)))
                {
                    row["master_id"] = Convert.ToInt32(id);
                    if (jobInfo.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT))
                        row[surrogateColumnn] = Convert.ToInt64(effectiveDateString + id);
                }
                else
                    masterIdsNotMassaged.Add(entityCode);

                if (jobInfo.BlockType == BLOCK_TYPES.DAILY_REPORT && !string.IsNullOrEmpty(jobInfo.TimeSeriesReportName) &&
                    (!jobInfo.IsLegReport || (jobInfo.IsLegReport && requireTSIdInLegDailyTable)))
                {
                    int timeSeriesId;
                    Dictionary<DateTime, int> dateVsId = null;

                    if (instIdVsDateVsTimeSeriesId.TryGetValue(typeIdToPopulateTSId, out dateVsId))
                        if (dateVsId.TryGetValue(effectiveDate, out timeSeriesId))
                            row[timeSeriesIdColumnn] = timeSeriesId;
                }

                if (jobInfo.BlockType == BLOCK_TYPES.TS_REPORT && jobInfo.DateType == DWHDateType.None)
                {
                    //Delete Data with Action Identifier = D;
                    if (row["Action Identifier"].Equals("D"))
                        row.DeleteDeferred();
                }
            }

            dataTable.CommitDelete();

            if (dropTables)
            {
                var arr = destinationTableNameWithSchema.Split(new char[] { '[', '.' }, StringSplitOptions.RemoveEmptyEntries);
                var tableName = arr[1].Replace("_staging]", "");

                InsertStatusForBlockDetails(jobInfo.BlockStatusId, "Syncing Schema", jobInfo.UserName);

                string query = @"SELECT COLUMN_NAME, 
                    CASE WHEN DATA_TYPE = 'VARCHAR' THEN 'VARCHAR(' + CASE WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN 'MAX' ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR(MAX)) END + ')'
	                    WHEN DATA_TYPE IN ('NUMERIC','DECIMAL') THEN DATA_TYPE + '(' + CAST(NUMERIC_PRECISION AS VARCHAR(MAX)) + ',' + CAST(NUMERIC_SCALE AS VARCHAR(MAX)) + ')'
	                    ELSE DATA_TYPE
	                    END AS DATA_TYPE
                    FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName + "' AND TABLE_SCHEMA = 'dimension'";
                DataSet ds2 = SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                    SyncTableStructures(dataTable, ds2, "[dimension].[" + tableName + "]", jobInfo, reportAttributeVsSchema, false, dropTables, refAttrsInTable, processLookupFromDaily);

                if (dropTables && jobInfo.DateType == DWHDateType.None)
                {
                    string query2 = null;
                    if (jobInfo.BlockType == BLOCK_TYPES.TS_REPORT)
                    {
                        query2 = Environment.NewLine + string.Format(@"DELETE FROM taskmanager.ivp_polaris_core_secref_extract_table_column_mapping WHERE identifier_key ='[{1}].[{0}_staging]'", tableName, schemaName.Substring(1, schemaName.Length - 2));

                        SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, query2, SRMDBQueryType.DELETE);
                    }
                }

                query = @"SELECT COLUMN_NAME, 
                    CASE WHEN DATA_TYPE = 'VARCHAR' THEN 'VARCHAR(' + CASE WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN 'MAX' ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR(MAX)) END + ')'
	                    WHEN DATA_TYPE IN ('NUMERIC','DECIMAL') THEN DATA_TYPE + '(' + CAST(NUMERIC_PRECISION AS VARCHAR(MAX)) + ',' + CAST(NUMERIC_SCALE AS VARCHAR(MAX)) + ')'
	                    ELSE DATA_TYPE
	                    END AS DATA_TYPE
                    FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + destinationTableName.Substring(1, destinationTableName.Length - 2) + "' AND TABLE_SCHEMA = '" + schemaName.Substring(1, schemaName.Length - 2) + "'";
                DataSet ds = SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    SyncTableStructures(dataTable, ds, destinationTableNameWithSchema, jobInfo, reportAttributeVsSchema, true, dropTables, refAttrsInTable, processLookupFromDaily);
                }
                else
                {
                    StringBuilder sb1 = new StringBuilder("CREATE TABLE ").Append(destinationTableNameWithSchema).Append(" (");
                    if (jobInfo.BlockType == BLOCK_TYPES.TS_REPORT)
                    {
                        sb1.Append("[id] INT IDENTITY ");
                        if (jobInfo.DateType == DWHDateType.None)
                            sb1.Append("(1,1),");
                        else
                        {
                            //Get Value from dimension table in case of LED
                            int maxId = GetMaxIdFromTSTable(jobInfo);
                            sb1.Append("(" + maxId + ",1),");
                        }
                    }
                    foreach (ObjectColumn dc in dataTable.Columns)
                    {
                        if (!lstTSTableColumnDrop.Contains(dc.ColumnName))
                        {
                            if (dctTsTableColumnRename.ContainsKey(dc.ColumnName))
                                sb1.Append("[").Append(dctTsTableColumnRename[dc.ColumnName]);
                            else
                                sb1.Append("[").Append(dc.ColumnName);

                            sb1.Append("] ").Append(GetDBDataType(dc.ColumnName, reportAttributeVsSchema, refAttrsInTable, jobInfo, processLookupFromDaily));

                            if (surrogateColumnn == dc.ColumnName)
                                sb1.Append(" NOT NULL");
                            sb1.Append(",");
                        }
                    }

                    foreach (var kvp in dctTsTableColumnAdd)
                    {
                        if (!dataTable.Columns.Contains(kvp.Key))
                            sb1.Append("[").Append(kvp.Key).Append("] ").Append(kvp.Value).Append(",");
                    }

                    sb1.Remove(sb1.Length - 1, 1);
                    sb1.Append(",INDEX cci CLUSTERED COLUMNSTORE);");

                    SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, sb1.ToString(), SRMDBQueryType.SELECT);
                }
            }

            if (jobInfo.BlockType == BLOCK_TYPES.TS_REPORT && jobInfo.DateType == DWHDateType.None)
            {
                RenameColumnsInTSStagingTable(dataTable, dctTsTableColumnRename, dctTsTableColumnAdd, lstTSTableColumnDrop);
            }

            InsertStatusForBlockDetails(jobInfo.BlockStatusId, "Dumping data in Staging table", jobInfo.UserName);

            SRMDWHJobExtension.ExecuteBulkCopyObject(jobInfo.DownstreamSQLConnectionName, destinationTableNameWithSchema, dataTable);
            dataTable = null;

            foreach (var tabName in tablesToBeDropped)
                if (!string.IsNullOrEmpty(tabName))
                    SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, string.Format("IF OBJECT_ID('{0}') IS NOT NULL DROP TABLE {0}", tabName), SRMDBQueryType.SELECT);

            mLogger.Debug("Columns Not Massaged : " + string.Join(",", columnNamesNotMassaged));
            mLogger.Debug("Entity Codes Not Massaged : " + string.Join(",", masterIdsNotMassaged));
            mLogger.Debug("PushDataToStagingTables -> End");
        }

        private static int GetMaxIdFromTSTable(SRMJobInfo jobInfo)
        {
            string query = string.Format(@"
                DECLARE @tableName VARCHAR(1000) = '{0}'
                DECLARE @isRefM BIT = '{1}'

                DECLARE @legPrepend VARCHAR(1000) = 'securities_leg'
                DECLARE @sql VARCHAR(MAX)= ''

                IF(@isRefM =1)
                BEGIN
	                SELECT @legPrepend = REPLACE(REPLACE(PARSENAME(@tableName,1),'ivp_polaris_',''),'_time_series_staging','_leg')
                END

                CREATE TABLE #tables(table_name VARCHAR(1000))
                INSERT INTO #tables
                SELECT REPLACE(REPLACE(REPLACE(@tableName,'[references]','[dimension]'),'[taskmanager]','[dimension]'),'_staging]',']')

                INSERT INTO #tables
                SELECT DISTINCT identifier_key FROM taskmanager.ivp_polaris_core_secref_extract_table_column_mapping
                WHERE identifier_key LIKE '%'+@legPrepend+'%'

                --SELECT * FROM #tables

                SELECT @sql = 'SELECT MAX(ID) +1 AS ID FROM ('

                SELECT @sql += ' SELECT IDENT_CURRENT('''+ table_name +''') AS ID UNION'
                FROM #tables

                SELECT @sql = SUBSTRING(@sql,0,LEN(@sql) - 5) + ') tab'

                PRINT(@sql)
                EXEC(@sql)

                DROP TABLE #tables", jobInfo.TableName, jobInfo.IsRef);

            int maxId = Convert.ToInt32(SRMDWHJobExtension.ExecuteQueryObject(jobInfo.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT).Tables[0].Rows[0][0]);
            return maxId;
        }

        private static void RenameColumnsInTSStagingTable(ObjectTable dataTable, Dictionary<string, string> dctTsTableColumnRename, Dictionary<string, string> dctTsTableColumnAdd, List<string> lstTSTableColumnDrop)
        {
            foreach (var kvp in dctTsTableColumnRename)
            {
                if (dataTable.Columns.Contains(kvp.Key))
                    dataTable.Columns[kvp.Key].ColumnName = kvp.Value;
            }

            foreach (var column in lstTSTableColumnDrop)
            {
                if (dataTable.Columns.Contains(column))
                    dataTable.Columns.Remove(column);
            }

            foreach (var kvp in dctTsTableColumnAdd)
            {
                if (!dataTable.Columns.Contains(kvp.Key))
                    dataTable.Columns.Add(new ObjectColumn(kvp.Key, typeof(System.String)));
            }
        }

        private static void PopulateTSColumnDictionary(Dictionary<string, string> dctTsTableColumnRename, Dictionary<string, string> dctTsTableColumnAdd, List<string> lstTSTableColumnDrop, SRMJobInfo jobInfo)
        {
            dctTsTableColumnRename.Add("effective start date", "effective_start_date");
            dctTsTableColumnRename.Add("effective end date", "effective_end_date");
            dctTsTableColumnRename.Add("action date", "action_date");

            lstTSTableColumnDrop.Add("Action Identifier");
            lstTSTableColumnDrop.Add("isTimeSeriesRecord");
            lstTSTableColumnDrop.Add("isArchiveRecord");

            dctTsTableColumnAdd.Add("created_by", "VARCHAR(200)");
            dctTsTableColumnAdd.Add("created_on", "DATETIME");
            dctTsTableColumnAdd.Add("is_active", "BIT");
            dctTsTableColumnAdd.Add("modified_on", "DATETIME");

            if (jobInfo.IsRef)
            {
                dctTsTableColumnRename.Add("last_modified_by", "modified_by");
                dctTsTableColumnRename.Add("Entity Type Name", "entity_type_name");
            }
            else
            {
                dctTsTableColumnRename.Add("Security Id", "security_id");
                dctTsTableColumnRename.Add("Security Type Name", "security_type_name");

                dctTsTableColumnAdd.Add("checksum", "VARBINARY");
                dctTsTableColumnAdd.Add("is_temp_row", "BIT");
                dctTsTableColumnAdd.Add("modified_by", "VARCHAR(200)");
            }
        }

        private static List<string> MergeTSLookupCollectionForManualAndTSUpdates(SRMJobInfo dailyReportjob, HashSet<string> instruments, Dictionary<string, Dictionary<DateTime, int>> instIdVsDateVsTimeSeriesId)
        {
            List<string> tablesToBeDropped = new List<string>();

            if (instruments.Count > 0)
            {
                string tableName = string.Empty;
                ObjectTable dtInstIds = new ObjectTable();
                dtInstIds.Columns.Add("instrument_id", typeof(string));
                foreach (var instId in instruments)
                    dtInstIds.Rows.Add(instId);

                //Get Data from Dimension Table
                string tsDimensionTableName = string.Empty;
                if (dailyReportjob.DateType == DWHDateType.None)
                    tsDimensionTableName = dailyReportjob.TimeSeriesTableNameWithSchema;

                Dictionary<string, Dictionary<DateTime, int>> dctData = CreateTimeSeriesLookUpCollection(dailyReportjob, ref tableName, dtInstIds, null, dailyReportjob, tsDimensionTableName, false);
                tablesToBeDropped.Add(tableName);

                //Get Data from Temp Dimension Table
                tableName = string.Empty;
                //As daily staging table will be in dimension schema
                if (dailyReportjob.TSReportDateType != DWHDateType.None)
                {
                    if (dailyReportjob.IsRef)
                        tsDimensionTableName = dailyReportjob.TimeSeriesTableNameWithSchema.Replace("[dimension]", "[references]");
                    else
                        tsDimensionTableName = dailyReportjob.TimeSeriesTableNameWithSchema.Replace("[dimension]", "[taskmanager]");
                }
                else
                    tsDimensionTableName = dailyReportjob.TimeSeriesTableNameWithSchema;

                tsDimensionTableName = tsDimensionTableName.Replace("_time_series]", "_time_series_staging]");

                bool requireColumnRename = true;
                if (dailyReportjob.TSReportDateType == DWHDateType.None)
                    requireColumnRename = false;

                Dictionary<string, Dictionary<DateTime, int>> dctManualUpdate = CreateTimeSeriesLookUpCollection(dailyReportjob, ref tableName, dtInstIds, null, tsReportJob: dailyReportjob, tsDimensionTableName, requireColumnRename);
                tablesToBeDropped.Add(tableName);

                //Merge Manual update on Complete Data
                foreach (var dctinstIdVsDateVsTimeSeriesId in dctManualUpdate)
                {
                    if (dctData.ContainsKey(dctinstIdVsDateVsTimeSeriesId.Key))
                    {
                        foreach (var dctDateVsTimeSeriesId in dctinstIdVsDateVsTimeSeriesId.Value)
                        {
                            if (dctData[dctinstIdVsDateVsTimeSeriesId.Key].ContainsKey(dctDateVsTimeSeriesId.Key))
                                dctData[dctinstIdVsDateVsTimeSeriesId.Key][dctDateVsTimeSeriesId.Key] = dctDateVsTimeSeriesId.Value;
                            else
                                dctData[dctinstIdVsDateVsTimeSeriesId.Key].Add(dctDateVsTimeSeriesId.Key, dctDateVsTimeSeriesId.Value);
                        }
                    }
                    else
                        dctData.Add(dctinstIdVsDateVsTimeSeriesId.Key, dctinstIdVsDateVsTimeSeriesId.Value);
                }

                //Merge Current Collection on Complete Collection
                foreach (var dctinstIdVsDateVsTimeSeriesId in dctData)
                {
                    if (instIdVsDateVsTimeSeriesId.ContainsKey(dctinstIdVsDateVsTimeSeriesId.Key))
                    {
                        foreach (var dctDateVsTimeSeriesId in dctinstIdVsDateVsTimeSeriesId.Value)
                        {
                            if (instIdVsDateVsTimeSeriesId[dctinstIdVsDateVsTimeSeriesId.Key].ContainsKey(dctDateVsTimeSeriesId.Key))
                                instIdVsDateVsTimeSeriesId[dctinstIdVsDateVsTimeSeriesId.Key][dctDateVsTimeSeriesId.Key] = dctDateVsTimeSeriesId.Value;
                            else
                                instIdVsDateVsTimeSeriesId[dctinstIdVsDateVsTimeSeriesId.Key].Add(dctDateVsTimeSeriesId.Key, dctDateVsTimeSeriesId.Value);
                        }
                    }
                    else
                        instIdVsDateVsTimeSeriesId.Add(dctinstIdVsDateVsTimeSeriesId.Key, dctinstIdVsDateVsTimeSeriesId.Value);
                }
            }
            return tablesToBeDropped;
        }

        public static void InsertMasterIdInDB(ObjectTable dataTable, SRMJobInfo jobInfo, HashSet<string> refAttributesInTable, HashSet<string> underlyingAttributesInTable)
        {
            string tableName = null;
            ObjectTable instrumentTable = new ObjectTable();
            instrumentTable.Columns.Add("instrument_id", typeof(string));
            instrumentTable.Columns.Add("is_active", typeof(bool));
            instrumentTable.Columns.Add("is_deleted", typeof(bool));

            RDBConnectionManager conn = null;
            try
            {
                string instrumentIdColumnName = "entity_code";
                if (!jobInfo.IsRef)
                    instrumentIdColumnName = "Security Key";

                var populateSecurityKey = false;

                if (DeletedInstruments == null)
                {
                    lock (lockObject)
                    {
                        if (DeletedInstruments == null)
                        {
                            DeletedInstruments = GetDeletedInstruments(jobInfo.IsRef, out PurgedInstruments);
                            if (!jobInfo.IsRef)
                                SecIdVsSecurityKeyAndCreatedOn = GetSecurityIdVsSecurityKey();
                        }
                    }
                }

                Dictionary<string, Dictionary<string, bool>> securityKeyVsCurrentInstsDeletedOrNot = null;
                Dictionary<string, string> securityKeyVsMaxSecurity = null;

                HashSet<string> addedInstIds = new HashSet<string>();

                if (!jobInfo.IsRef)
                {
                    securityKeyVsCurrentInstsDeletedOrNot = new Dictionary<string, Dictionary<string, bool>>(StringComparer.OrdinalIgnoreCase);
                    securityKeyVsMaxSecurity = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    if (!dataTable.Columns.Contains("Security Key"))
                    {
                        populateSecurityKey = true;
                        dataTable.Columns.Add("Security Key", typeof(string));
                    }

                    foreach (var groupedOnSecurityKey in SecIdVsSecurityKeyAndCreatedOn.GroupBy(x => x.Value.Item1))
                    {
                        Dictionary<string, bool> insts = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                        securityKeyVsCurrentInstsDeletedOrNot[groupedOnSecurityKey.Key] = insts;

                        DateTime maxDate = default(DateTime);
                        string maxSecurityId = null;
                        foreach (var security in groupedOnSecurityKey)
                        {
                            if (maxDate < security.Value.Item2)
                            {
                                maxDate = security.Value.Item2;
                                maxSecurityId = security.Key;
                            }
                            insts.Add(security.Key, DeletedInstruments.Contains(security.Key));
                        }
                        securityKeyVsMaxSecurity[groupedOnSecurityKey.Key] = maxSecurityId;
                    }
                }

                HashSet<string> refLookups = new HashSet<string>();
                HashSet<string> secLookups = new HashSet<string>();
                bool deletedRows = false;
                var masterIdCollection = EntityCodeVsMasterId;
                if (!jobInfo.IsRef)
                    masterIdCollection = SecurityIdVsMasterId;

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    var row = dataTable.Rows[i];
                    string instIdSecondary = null;

                    foreach (var column in refAttributesInTable)
                    {
                        var id = Convert.ToString(row[column]);
                        if (!string.IsNullOrEmpty(id) && !EntityCodeVsMasterId.ContainsKey(id))
                            refLookups.Add(id);
                    }
                    foreach (var column in underlyingAttributesInTable)
                    {
                        var id = Convert.ToString(row[column]);
                        if (!string.IsNullOrEmpty(id) && !SecurityIdVsMasterId.ContainsKey(id))
                            secLookups.Add(id);
                    }

                    if (!jobInfo.IsRef)
                        instIdSecondary = Convert.ToString(row["Security Id"]);

                    if (populateSecurityKey)
                    {
                        var secId = instIdSecondary;

                        Tuple<string, DateTime> securityKeyAndCreatedOn = null;
                        if (SecIdVsSecurityKeyAndCreatedOn.TryGetValue(secId, out securityKeyAndCreatedOn))
                            row["Security Key"] = securityKeyAndCreatedOn.Item1;
                        else
                            row["Security Key"] = secId;
                    }

                    var instId = Convert.ToString(row[instrumentIdColumnName]);

                    if (!masterIdCollection.ContainsKey(instId) && !addedInstIds.Contains(instId))
                    {
                        var dr = instrumentTable.NewRow();
                        dr["instrument_id"] = instId;

                        var isInstrumentDeleted = DeletedInstruments.Contains(instId);
                        Dictionary<string, bool> insts = null;
                        if (securityKeyVsCurrentInstsDeletedOrNot != null && securityKeyVsCurrentInstsDeletedOrNot.TryGetValue(instId, out insts))
                            dr["is_active"] = insts.Any(x => !x.Value);
                        else
                            dr["is_active"] = !isInstrumentDeleted;
                        dr["is_deleted"] = isInstrumentDeleted;

                        instrumentTable.Rows.Add(dr);
                        addedInstIds.Add(instId);
                    }

                    if (!string.IsNullOrEmpty(instIdSecondary) && !masterIdCollection.ContainsKey(instIdSecondary) && !addedInstIds.Contains(instIdSecondary))
                    {
                        var dr = instrumentTable.NewRow();
                        dr["instrument_id"] = instIdSecondary;

                        var isInstrumentDeleted = DeletedInstruments.Contains(instIdSecondary);
                        dr["is_active"] = !isInstrumentDeleted;
                        dr["is_deleted"] = isInstrumentDeleted;

                        instrumentTable.Rows.Add(dr);
                        addedInstIds.Add(instIdSecondary);
                    }

                    if (!jobInfo.RequireDeletedRows)
                    {
                        string insttId = null;
                        if (jobInfo.IsRef)
                            insttId = Convert.ToString(row["entity_code"]);
                        else
                            insttId = Convert.ToString(row["Security Id"]);
                        if (DeletedInstruments.Contains(insttId))
                        {
                            deletedRows = true;
                            row.DeleteDeferred();
                        }
                    }
                    else if (!jobInfo.IsRef)
                    {
                        string securityId = Convert.ToString(row["Security Id"]);
                        string securityKey = Convert.ToString(row["Security Key"]);

                        string maxSecurityId = null;
                        if ((securityKeyVsMaxSecurity.TryGetValue(securityKey, out maxSecurityId) && maxSecurityId != securityId))
                        {
                            deletedRows = true;
                            row.DeleteDeferred();
                        }
                    }
                }

                if (deletedRows)
                    dataTable.CommitDelete();

                foreach (var instrument in PurgedInstruments)
                {
                    if (!masterIdCollection.ContainsKey(instrument) && !addedInstIds.Contains(instrument))
                    {
                        var dr = instrumentTable.NewRow();
                        dr["instrument_id"] = instrument;
                        dr["is_active"] = false;
                        dr["is_deleted"] = true;
                        instrumentTable.Rows.Add(dr);
                        addedInstIds.Add(instrument);
                    }
                }

                if (instrumentTable.Rows.Count > 0)
                {
                    Dictionary<string, MasterTableInfo> instrumentVsMasterTableInfo = new Dictionary<string, MasterTableInfo>();
                    foreach (var row in instrumentTable.Rows)
                    {
                        var instrument = Convert.ToString(row["instrument_id"]);
                        instrumentVsMasterTableInfo[instrument] = new MasterTableInfo("0", Convert.ToBoolean(row["is_active"]), Convert.ToBoolean(row["is_deleted"]));
                    }

                    string query = @"DECLARE @tabName VARCHAR(50) = '[dimension].[',
		                @sql VARCHAR(MAX)

                SELECT @tabName += CAST(NEWID() AS VARCHAR(MAX)) + ']';
                SELECT @sql = 'CREATE TABLE ' + @tabName + '(instrument_id VARCHAR(15) PRIMARY KEY)';
                EXEC(@sql)
                SELECT @tabName";
                    tableName = Convert.ToString(SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT).Tables[0].Rows[0][0]);

                    SRMDWHJobExtension.ExecuteBulkCopyObject(jobInfo.DownstreamSQLConnectionName, tableName, instrumentTable);
                    var instrumentsToSync = new List<string>();

                    try
                    {
                        conn = SRMDWHJobExtension.GetConnectionManager(jobInfo.DownstreamSQLConnectionName, false, IsolationLevel.ReadCommitted);

                        query = String.Format(@"DECLARE @table_name VARCHAR(200),
		                @isRefm BIT = '{0}',
		                @temp_inst_table VARCHAR(100) = '{1}',
		                @sql VARCHAR(MAX) = '',
		                @master_identifier_column VARCHAR(30),
                        @db_name VARCHAR(100),
                        @schema_name VARCHAR(20)

                        SELECT @db_name = DB_NAME()                    
                        SELECT @schema_name = CASE WHEN @isRefm = 1 THEN 'references' ELSE 'taskmanager' END

                        SELECT @table_name = PARSENAME(surrogate_table_name,1)
				        FROM dimension.ivp_srm_dwh_tables_for_loading
                        WHERE table_name = '[' + @db_name + '].[' + @schema_name + '].[' + PARSENAME('{2}',1) + ']' AND is_active = 1            
    
                        SELECT @master_identifier_column = CASE WHEN @isRefm = 1 THEN '[entity_code]' ELSE '[Security Id]' END

                        IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @table_name AND TABLE_SCHEMA = 'dimension')
                        BEGIN
                            SELECT @sql = 'SELECT id, ' + @master_identifier_column  + ', is_active, is_deleted
                                FROM dimension.[' + @table_name + '] tab 
							    INNER JOIN ' + @temp_inst_table + ' tab2 ON tab.' + @master_identifier_column + ' = tab2.instrument_id'
                        END
                        ELSE
                        BEGIN
                            SELECT @sql = 'SELECT CAST(NULL AS INT) AS [id], CAST(NULL AS VARCHAR(15)) AS ' + @master_identifier_column + ', CAST(NULL AS BIT) AS is_active, CAST(NULL AS BIT) AS is_deleted WHERE 1 = 2'
                        END
                        
                        EXEC(@sql)", jobInfo.IsRef.ToString().ToUpper(), tableName, jobInfo.TableName);


                        Dictionary<string, MasterTableInfo> instrumentVsMasterTableInfoFromTable = new Dictionary<string, MasterTableInfo>();

                        conn.ExecuteSelectQuery(query, (reader) =>
                        {
                            while (reader.Read())
                            {
                                instrumentVsMasterTableInfoFromTable[reader.GetString(1)] = new MasterTableInfo(reader.GetInt32(0).ToString(), reader.GetBoolean(2), reader.GetBoolean(3));
                            }
                        });

                        foreach (var instrument in instrumentVsMasterTableInfo)
                        {
                            if (instrumentVsMasterTableInfoFromTable.TryGetValue(instrument.Key, out MasterTableInfo info))
                            {
                                if (!instrument.Value.Equals(info))
                                    instrumentsToSync.Add(instrument.Key);
                                else
                                {
                                    lock (lockObject)
                                    {
                                        if (jobInfo.IsRef)
                                        {
                                            if (!EntityCodeVsMasterId.ContainsKey(instrument.Key))
                                                EntityCodeVsMasterId.Add(instrument.Key, info.Id);
                                        }
                                        else
                                        {
                                            if (!SecurityIdVsMasterId.ContainsKey(instrument.Key))
                                                SecurityIdVsMasterId.Add(instrument.Key, info.Id);
                                        }
                                    }
                                }
                            }
                            else
                                instrumentsToSync.Add(instrument.Key);
                        }

                    }
                    finally
                    {
                        if (conn != null)
                            RDALAbstractFactory.DBFactory.PutConnectionManager(conn);
                    }

                    if (instrumentsToSync.Count > 0)
                    {
                        var newInstrumentTable = instrumentTable.Clone();

                        var dc = new ObjectColumn("loading_time", typeof(DateTime));
                        dc.DefaultValue = jobInfo.LoadingTimeForStagingTable;
                        newInstrumentTable.Columns.Add(dc);

                        foreach (var instrument in instrumentsToSync)
                        {
                            var masterInfo = instrumentVsMasterTableInfo[instrument];
                            var dr = newInstrumentTable.NewRow();
                            dr["instrument_id"] = instrument;
                            dr["is_active"] = masterInfo.IsActive;
                            dr["is_deleted"] = masterInfo.IsDeleted;
                            newInstrumentTable.Rows.Add(dr);
                        }

                        instrumentTable = null;

                        query = $@"DECLARE @tabName VARCHAR(50) = '{tableName}',
		                @sql VARCHAR(MAX)

                        SELECT @sql = 'TRUNCATE TABLE ' + @tabName + ';';
                        EXEC(@sql);

                        SELECT @sql = 'ALTER TABLE ' + @tabName + 'ADD is_active BIT, is_deleted BIT, loading_time DATETIME';
                        EXEC(@sql)";
                        SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, query, SRMDBQueryType.INSERT);

                        SRMDWHJobExtension.ExecuteBulkCopyObject(jobInfo.DownstreamSQLConnectionName, tableName, newInstrumentTable);

                        conn = SRMDWHJobExtension.GetConnectionManager(jobInfo.DownstreamSQLConnectionName, true, IsolationLevel.Serializable);

                        query = string.Format(@"DECLARE @table_name VARCHAR(200),
		                @isRefm BIT = '{0}',
		                @temp_inst_table VARCHAR(100) = '{1}',
		                @sql VARCHAR(MAX) = '',
		                @master_identifier_column VARCHAR(30),
                        @db_name VARCHAR(100),
                        @schema_name VARCHAR(20),
                        @new_master_table BIT = 0

                        SELECT @db_name = DB_NAME()                    
                        SELECT @schema_name = CASE WHEN @isRefm = 1 THEN 'references' ELSE 'taskmanager' END

                        SELECT @table_name = PARSENAME(surrogate_table_name,1)
				        FROM dimension.ivp_srm_dwh_tables_for_loading
                        WHERE table_name = '[' + @db_name + '].[' + @schema_name + '].[' + PARSENAME('{2}',1) + ']' AND is_active = 1            

                        SELECT @master_identifier_column = CASE WHEN @isRefm = 1 THEN '[entity_code]' ELSE '[Security Id]' END

                        IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @table_name AND TABLE_SCHEMA = 'dimension')
                        BEGIN
                            SELECT @new_master_table = 1

	                        SELECT @sql = 'CREATE TABLE dimension.[' + @table_name + ']
					                        (id INT IDENTITY(1,1) NOT NULL,' + @master_identifier_column + ' VARCHAR(15) NOT NULL, is_active BIT, is_deleted BIT, loading_time DATETIME)
                                            CREATE CLUSTERED COLUMNSTORE INDEX IX_CCI ON [dimension].[' + @table_name + ']
                                            ALTER TABLE [dimension].[' + @table_name + '] ADD CONSTRAINT Pk_' + @table_name + ' PRIMARY KEY NONCLUSTERED (id)' + CHAR(13)

	                        EXEC(@sql)
	                        SELECT @sql = 'INSERT INTO dimension.[' + @table_name + '] WITH (TABLOCKX) (' + @master_identifier_column + ', is_active, is_deleted, loading_time)
					                        SELECT instrument_id, is_active, is_deleted, loading_time
					                        FROM ' + @temp_inst_table + CHAR(13)
                        END
                        ELSE
                        BEGIN
	                        SELECT @sql = 'INSERT INTO dimension.[' + @table_name + '] WITH (TABLOCKX) (' + @master_identifier_column + ', is_active, is_deleted, loading_time)
					                        SELECT instrument_id, tab.is_active, tab.is_deleted, tab.loading_time
					                        FROM ' + @temp_inst_table + ' tab
					                        LEFT JOIN dimension.[' + @table_name + '] mas ON tab.instrument_id = mas.' + @master_identifier_column + CHAR(13) + 
				                           'WHERE mas.id IS NULL' + CHAR(13) +

				                           'UPDATE mas
				                            SET mas.is_active = tab.is_active, mas.is_deleted = tab.is_deleted, mas.loading_time = tab.loading_time
				                            FROM ' + @temp_inst_table + ' tab
					                        INNER JOIN dimension.[' + @table_name + '] mas ON tab.instrument_id = mas.' + @master_identifier_column + CHAR(13)
                        END
                
                        SELECT @sql += CASE WHEN @isRefm = 1 THEN 'INSERT INTO dimension.ivp_srm_dwh_entity_type_master WITH (TABLOCKX) 
                            SELECT mas.entity_code, mas.id
				            FROM dimension.[' + @table_name + '] mas
					        LEFT JOIN dimension.ivp_srm_dwh_entity_type_master tab ON mas.' + @master_identifier_column + ' = tab.entity_code
                            WHERE tab.entity_code IS NULL' ELSE '' END
	            
                        EXEC(@sql)", jobInfo.IsRef.ToString().ToUpper(), tableName, jobInfo.TableName);

                        query += @"
                            SELECT @sql = 'SELECT id, ' + @master_identifier_column 
						+ ' FROM dimension.[' + @table_name + '] tab 
							INNER JOIN ' + @temp_inst_table + ' tab2 ON tab.' + @master_identifier_column + ' = tab2.instrument_id'
				           
                            EXEC(@sql)

                            SELECT 'dimension.[' + @table_name + ']' AS master_table_name
                
                            SELECT @new_master_table";

                        var os = SRMDWHJobExtension.ExecuteQueryObject(conn, query, SRMDBQueryType.SELECT);
                        conn.CommitTransaction();

                        var masterTableName = Convert.ToString(os.Tables[1].Rows[0][0]);
                        var isNewMasterTable = Convert.ToBoolean(os.Tables[2].Rows[0][0]);

                        lock (lockObject)
                        {
                            foreach (var row in os.Tables[0].Rows)
                            {
                                if (jobInfo.IsRef)
                                {
                                    var ec = Convert.ToString(row["entity_code"]);
                                    if (!EntityCodeVsMasterId.ContainsKey(ec))
                                        EntityCodeVsMasterId.Add(ec, Convert.ToString(row["id"]));
                                }
                                else
                                {
                                    var ec = Convert.ToString(row["Security Id"]);
                                    if (!SecurityIdVsMasterId.ContainsKey(ec))
                                        SecurityIdVsMasterId.Add(ec, Convert.ToString(row["id"]));
                                }
                            }
                        }
                        if (SRMDWHStatic.dctSetupIdVsDWHAdapterDetails.ContainsKey(jobInfo.SetupId))
                        {
                            foreach (var adapterInfo in SRMDWHStatic.dctSetupIdVsDWHAdapterDetails[jobInfo.SetupId])
                            {
                                bool processMasterTable = false;
                                lock (((ICollection)AdapterVsMasterTableUnderProcessing).SyncRoot)
                                {
                                    if (!AdapterVsMasterTableUnderProcessing.TryGetValue(adapterInfo.AdapterId, out HashSet<string> masterTables))
                                    {
                                        masterTables = new HashSet<string>();
                                        AdapterVsMasterTableUnderProcessing[adapterInfo.AdapterId] = masterTables;
                                    }
                                    if (!masterTables.Contains(masterTableName))
                                    {
                                        masterTables.Add(masterTableName);
                                        processMasterTable = true;
                                    }
                                }

                                var metadata = new MasterTableMetaData(masterTableName, jobInfo.IsRef, isNewMasterTable, jobInfo.LoadingTimeForStagingTable);

                                if (processMasterTable)
                                {
                                    FailedSyncMetadata? failedResponse = DWHAdapterController.SyncMasterTableData(adapterInfo.AdapterId, jobInfo.DownstreamSQLConnectionName, metadata);

                                    try
                                    {
                                        if (failedResponse.HasValue)
                                        {
                                            mLogger.Error(adapterInfo.AdapterName + " - Exception : " + failedResponse.Value.ErrorMessage);
                                            throw new Exception(adapterInfo.AdapterName + " - Exception : " + failedResponse.Value.ErrorMessage);
                                        }
                                    }
                                    finally
                                    {
                                        lock (((ICollection)AdapterVsMasterTableUnderProcessing).SyncRoot)
                                        {
                                            if (AdapterVsMasterTableUnderProcessing.TryGetValue(adapterInfo.AdapterId, out HashSet<string> masterTables))
                                            {
                                                if (masterTables.Contains(masterTableName))
                                                {
                                                    masterTables.Remove(masterTableName);
                                                    processMasterTable = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var entityCodeVsMasterIdTemp = GetEntityCodeVsMasterId(jobInfo, refLookups);
                var securityIdVsMasterIdTemp = GetSecurityIdVsMasterId(jobInfo.DownstreamSQLConnectionName, secLookups);

                lock (lockObject)
                {
                    foreach (var kvp in entityCodeVsMasterIdTemp)
                        EntityCodeVsMasterId[kvp.Key] = kvp.Value;
                    foreach (var kvp in securityIdVsMasterIdTemp)
                        SecurityIdVsMasterId[kvp.Key] = kvp.Value;
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("Exception -> " + ee.ToString());

                if (conn != null)
                    conn.RollbackTransaction();
                try
                {
                    mLogger.Error("Duplicates : " + string.Join(",", instrumentTable.AsEnumerable().GroupBy(x => Convert.ToString(x["instrument_id"])).Where(x => x.Count() > 1).Select(y => string.Join(Environment.NewLine, y.Select(z => Convert.ToString(z["instrument_id"]) + "ž" + Convert.ToString(z["is_active"]) + "ž" + Convert.ToString(z["is_deleted"]))))));
                }
                catch { }
                throw;
            }
            finally
            {
                if (conn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(conn);
                try
                {
                    if (tableName != null)
                        SRMDWHJobExtension.ExecuteQueryObject(jobInfo.DownstreamSQLConnectionName, string.Format("IF(OBJECT_ID('{0}') IS NOT NULL) DROP TABLE {0}", tableName), SRMDBQueryType.DELETE);
                }
                catch { }
            }
        }

        private static Dictionary<string, Dictionary<DateTime, int>> CreateTimeSeriesLookUpCollection(SRMJobInfo dailyJob, ref string tableName, ObjectTable dtInstIds, RDBConnectionManager con, SRMJobInfo tsReportJob, string tsDimTableNameWithSchema, bool requireColumnRename)
        {
            mLogger.Debug("CreateTimeSeriesLookUpCollection --> Start --> " + dailyJob.TableName + " : requireColumnRename : " + requireColumnRename);

            Dictionary<string, Dictionary<DateTime, int>> instIdVsDateVsTimeSeriesId = new Dictionary<string, Dictionary<DateTime, int>>();
            tableName = "[taskmanager].[inst_" + Guid.NewGuid().ToString().Replace("-", "_") + "]";
            try
            {
                string instColumn;
                if (dtInstIds != null)
                {
                    SRMDWHJobExtension.ExecuteQueryObject(dailyJob.DownstreamSQLConnectionName, "CREATE TABLE " + tableName + " (inst_id VARCHAR(50));", SRMDBQueryType.SELECT);
                    SRMDWHJobExtension.ExecuteBulkCopyObject(dailyJob.DownstreamSQLConnectionName, tableName, dtInstIds, new Dictionary<string, string>() { { "instrument_id", "inst_id" } });

                    SRMDWHJobExtension.ExecuteQuery(dailyJob.DownstreamSQLConnectionName, string.Format("CREATE NONCLUSTERED INDEX idx_1 ON {0}([inst_id])", tableName), SRMDBQueryType.SELECT);
                }

                if (dailyJob.IsRef)
                    instColumn = "entity_code";
                else
                    instColumn = "security_id";

                string query = string.Format(@"DECLARE @ts_table VARCHAR(MAX),
                            @isrefm BIT = '{4}',
                            @inst_table_name VARCHAR(100) = '{1}',
                            @sql VARCHAR(MAX) = '',
                            @db_name VARCHAR(100),
                            @ts_temp_table VARCHAR(500) = '{3}'

                    SELECT @db_name = DB_NAME()                    
                    SELECT @ts_table = @ts_temp_table

                    IF(ISNULL(@ts_temp_table,'') = '')
                    BEGIN
                        SELECT @ts_table = ts_table_name, @isRefm = isrefm
                        FROM dimension.ivp_srm_dwh_tables_for_loading
                        WHERE table_name = '[' + @db_name + '].{0}' AND is_active = 1

                        IF(ISNULL(@ts_table,'') = '')
                            RAISERROR('Cant find ts table',16,1)
                    END

                    SELECT @sql = ' SELECT id, {2},' 
                                + CASE WHEN @isrefm = 1 THEN 'entity_code' ELSE '{6}' END + ', {7} AS effective_start_date, {8} AS effective_end_date '
                                + ' FROM ' + @ts_table 
                                + CASE WHEN ISNULL(@inst_table_name,'') = '' THEN '' ELSE 'tab INNER JOIN ' + @inst_table_name + ' tab2 ON tab.' + CASE WHEN @isrefm = 1 THEN 'entity_code' ELSE '{6}' END + ' = tab2.inst_id' END
                                + ' WHERE ' + CASE WHEN @isrefm = 1 THEN '{9}' ELSE '{10}' END + ' NOT LIKE ''%-%'' {5}'
                                + ' ORDER BY ' + CASE WHEN @isrefm = 1 THEN 'entity_code' ELSE '{6}' END + ', {7}'
                    EXEC(@sql)

                    SELECT PARSENAME(@ts_table,1) AS ts_table", dailyJob.TableName, (dtInstIds != null ? tableName : string.Empty), "master_id", tsDimTableNameWithSchema, dailyJob.IsRef,
                    dailyJob.RequireTimeTSReports ? (" AND " +
                            (requireColumnRename ? "[Effective Start Date]" : "effective_start_date") + " >= ''" +
                                dailyJob.EffectiveStartDateForReport.Value.AddYears(-5).ToString("yyyyMMdd HH:mm:ss.fff") + "''") : String.Empty,
                    requireColumnRename ? "[Security Id]" : "security_id", requireColumnRename ? "[Effective Start Date]" : "effective_start_date",
                    requireColumnRename ? "[Effective End Date]" : "effective_end_date", requireColumnRename ? "[Entity Type Name]" : "entity_type_name",
                    requireColumnRename ? "[Security Type Name]" : "security_type_name");

                ObjectSet os = null;
                ObjectTable dt = null;
                if (con == null)
                    os = SRMDWHJobExtension.ExecuteQueryObject(dailyJob.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                else
                    os = SRMDWHJobExtension.ExecuteQueryObject(con, query, SRMDBQueryType.SELECT);

                dt = os.Tables[0];
                DateTime today = DateTime.Today;

                if (requireColumnRename && dt.Columns.Contains("Security Id"))
                    dt.Columns["Security Id"].ColumnName = "security_id";

                if (tsReportJob != null && tsReportJob.RequireTimeTSReports)
                {
                    ObjectTable mappingTable = new ObjectTable();
                    mappingTable.Columns.Add("surrogate_id", typeof(long));
                    mappingTable.Columns.Add("time_series_id", typeof(int));

                    var instVsDayVsDayBestId = new Dictionary<string, Dictionary<DateTime, Tuple<int, DateTime, bool>>>();
                    for (var rowCounter = 0; rowCounter < dt.Rows.Count; rowCounter++)
                    {
                        ObjectRow currentRow = dt.Rows[rowCounter];
                        ObjectRow nextRow = null;
                        string instIdNext = string.Empty;

                        var masterIdCurrent = Convert.ToString(currentRow["master_id"]);
                        var instIdCurrent = Convert.ToString(currentRow[instColumn]);
                        var idCurrent = Convert.ToInt32(currentRow["id"]);
                        var startDateCurrent = Convert.ToDateTime(currentRow["effective_start_date"]);
                        var endDateStringCurrent = Convert.ToString(currentRow["effective_end_date"]);
                        var day = startDateCurrent.Date;

                        if (rowCounter < dt.Rows.Count - 1)
                        {
                            nextRow = dt.Rows[rowCounter + 1];
                            instIdNext = Convert.ToString(nextRow[instColumn]);
                        }

                        Dictionary<DateTime, Tuple<int, DateTime, bool>> dayVsIdAndTimestamp = null;
                        if (!instVsDayVsDayBestId.TryGetValue(instIdCurrent, out dayVsIdAndTimestamp))
                        {
                            dayVsIdAndTimestamp = new Dictionary<DateTime, Tuple<int, DateTime, bool>>();
                            instVsDayVsDayBestId[instIdCurrent] = dayVsIdAndTimestamp;
                        }
                        dayVsIdAndTimestamp[day] = new Tuple<int, DateTime, bool>(idCurrent, startDateCurrent, false);

                        if (!string.IsNullOrEmpty(endDateStringCurrent))
                        {
                            var endDateCurrent = Convert.ToDateTime(endDateStringCurrent);
                            day = endDateCurrent.Date;

                            if (!instVsDayVsDayBestId.TryGetValue(instIdCurrent, out dayVsIdAndTimestamp))
                            {
                                dayVsIdAndTimestamp = new Dictionary<DateTime, Tuple<int, DateTime, bool>>();
                                instVsDayVsDayBestId[instIdCurrent] = dayVsIdAndTimestamp;
                            }
                            dayVsIdAndTimestamp[day] = new Tuple<int, DateTime, bool>(idCurrent, startDateCurrent, true);
                        }

                        //MAPPING TABLE
                        DateTime endDateCurr;
                        if (!string.IsNullOrEmpty(endDateStringCurrent))
                        {
                            if (instIdCurrent == instIdNext)
                                endDateCurr = Convert.ToDateTime(endDateStringCurrent).Date;
                            else
                                endDateCurr = Convert.ToDateTime(endDateStringCurrent).Date;
                        }
                        else
                            endDateCurr = today;

                        var dayDiff = (endDateCurr - startDateCurrent).Days;

                        for (var dateAddCounter = 0; dateAddCounter <= dayDiff; dateAddCounter++)
                        {
                            var tempDate = startDateCurrent.AddDays(dateAddCounter);

                            var ndr = mappingTable.NewRow();
                            ndr["surrogate_id"] = tempDate.ToString("yyyyMMdd") + masterIdCurrent;
                            ndr["time_series_id"] = idCurrent;
                            mappingTable.Rows.Add(ndr);
                        }
                    }

                    #region MAPPING TABLE
                    if (createMappingTableAndIntradayView)
                    {
                        var mappingTableName = "[dimension].[" + Convert.ToString(os.Tables[1].Rows[0][0]) + "_mapping]";
                        var mappingTempTableName = mappingTableName.Replace("_mapping]", "_mapping_temp]");
                        var queryTemp = string.Format(@"
                        IF(OBJECT_ID('{0}') IS NULL)
                        BEGIN
                            CREATE TABLE {0} ([surrogate_id] BIGINT NOT NULL, [time_series_id] INT)

                            SELECT CAST(0 AS BIT) AS table_exists
                        END
                        ELSE
                        BEGIN
                            IF(OBJECT_ID('{1}') IS NOT NULL)
                                DROP TABLE {1}
                            
                            CREATE TABLE {1} ([surrogate_id] BIGINT NOT NULL, [time_series_id] INT)
                            SELECT CAST(1 AS BIT) AS table_exists
                        END", mappingTableName, mappingTempTableName);

                        var tableExists = Convert.ToBoolean(SRMDWHJobExtension.ExecuteQueryObject(dailyJob.DownstreamSQLConnectionName, queryTemp, SRMDBQueryType.SELECT).Tables[0].Rows[0][0]);
                        SRMDWHJobExtension.ExecuteBulkCopyObject(dailyJob.DownstreamSQLConnectionName, tableExists ? mappingTempTableName : mappingTableName, mappingTable);

                        queryTemp = string.Format(@"CREATE CLUSTERED INDEX IX_1 ON {0} (surrogate_id)", tableExists ? mappingTempTableName : mappingTableName);
                        SRMDWHJobExtension.ExecuteQueryObject(dailyJob.DownstreamSQLConnectionName, queryTemp, SRMDBQueryType.INSERT);

                        if (tableExists)
                        {
                            queryTemp = string.Format(@"
                        DELETE main
                        FROM {0} main
                        INNER JOIN (SELECT DISTINCT [surrogate_id] FROM {1}) tab ON main.[surrogate_id] = tab.[surrogate_id]

                        INSERT INTO {0}([surrogate_id], [time_series_id])
                        SELECT [surrogate_id], [time_series_id]
                        FROM {1}

                        DROP TABLE {1}", mappingTableName, mappingTempTableName);

                            SRMDWHJobExtension.ExecuteQueryObject(dailyJob.DownstreamSQLConnectionName, queryTemp, SRMDBQueryType.INSERT);
                        }
                    }
                    #endregion

                    if (instVsDayVsDayBestId.Count > 0)
                    {
                        var newDt = dt.Clone();
                        newDt.Columns.Add("deleted_security", typeof(bool));

                        foreach (var instrumentLevel in instVsDayVsDayBestId)
                        {
                            foreach (var date in instrumentLevel.Value)
                            {
                                var ndr = newDt.NewRow();
                                ndr["id"] = date.Value.Item1;
                                ndr[instColumn] = instrumentLevel.Key;
                                ndr["effective_start_date"] = date.Value.Item2.Date;
                                ndr["deleted_security"] = date.Value.Item3;
                                newDt.Rows.Add(ndr);
                            }
                        }

                        dt = newDt.AsEnumerable().OrderBy(x => Convert.ToString(x[instColumn])).ThenBy(x => Convert.ToDateTime(x["effective_start_date"])).CopyToObjectTable();
                    }

                    bool deleteRows = false;

                    for (var rowCounter = 0; rowCounter < dt.Rows.Count; rowCounter++)
                    {
                        ObjectRow nextRow = null;
                        ObjectRow currentRow = dt.Rows[rowCounter];
                        string instIdNext = string.Empty;

                        var instIdCurrent = Convert.ToString(currentRow[instColumn]);
                        var idCurrent = Convert.ToInt32(currentRow["id"]);
                        var startDateCurrent = Convert.ToDateTime(currentRow["effective_start_date"]);

                        if (rowCounter < dt.Rows.Count - 1)
                        {
                            nextRow = dt.Rows[rowCounter + 1];
                            instIdNext = Convert.ToString(nextRow[instColumn]);
                        }

                        if (instIdCurrent == instIdNext)
                        {
                            var startDateNext = Convert.ToDateTime(nextRow["effective_start_date"]);
                            currentRow["effective_end_date"] = startDateNext;
                        }
                        else
                        {
                            if (Convert.ToBoolean(currentRow["deleted_security"]))
                            {
                                deleteRows = true;
                                currentRow.DeleteDeferred();
                            }
                            else
                                currentRow["effective_end_date"] = DBNull.Value;
                        }
                    }
                    if (deleteRows)
                        dt.CommitDelete();
                }

                for (var rowCounter = 0; rowCounter < dt.Rows.Count; rowCounter++)
                {
                    ObjectRow nextRow = null;
                    ObjectRow currentRow = dt.Rows[rowCounter];
                    string instIdNext = string.Empty;

                    var instIdCurrent = Convert.ToString(currentRow[instColumn]);
                    var idCurrent = Convert.ToInt32(currentRow["id"]);
                    var startDateCurrent = Convert.ToDateTime(currentRow["effective_start_date"]);
                    var endDateStringCurrent = Convert.ToString(currentRow["effective_end_date"]);

                    if (rowCounter < dt.Rows.Count - 1)
                    {
                        nextRow = dt.Rows[rowCounter + 1];
                        instIdNext = Convert.ToString(nextRow[instColumn]);
                    }

                    DateTime endDateCurrent;
                    if (!string.IsNullOrEmpty(endDateStringCurrent))
                    {
                        if (instIdCurrent == instIdNext)
                            endDateCurrent = Convert.ToDateTime(endDateStringCurrent).AddDays(-1);
                        else
                            endDateCurrent = Convert.ToDateTime(endDateStringCurrent);
                    }
                    else
                        endDateCurrent = today;

                    var dayDiff = (endDateCurrent - startDateCurrent).Days;

                    Dictionary<DateTime, int> dateVsId = null;
                    if (!instIdVsDateVsTimeSeriesId.TryGetValue(instIdCurrent, out dateVsId))
                    {
                        dateVsId = new Dictionary<DateTime, int>();
                        instIdVsDateVsTimeSeriesId[instIdCurrent] = dateVsId;
                    }

                    for (var dateAddCounter = 0; dateAddCounter <= dayDiff; dateAddCounter++)
                    {
                        var tempDate = startDateCurrent.AddDays(dateAddCounter);
                        try
                        {
                            dateVsId.Add(tempDate, idCurrent);
                        }
                        catch
                        {
                            mLogger.Error(string.Format("Duplicate for Inst : {0} Date : {1}", instIdCurrent, tempDate));

                            var dtReportData = dt.ConvertToDataTable();
                            StringWriter writer = new StringWriter();
                            if (string.IsNullOrEmpty(dtReportData.TableName))
                                dtReportData.TableName = "Table";

                            dtReportData.WriteXml(writer, XmlWriteMode.WriteSchema);
                            mLogger.Error("Data : " + writer.ToString());

                            throw;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                mLogger.Error("CreateTimeSeriesLookUpCollection --> Exception --> " + dailyJob.TableName + " ---> " + ex);
                throw;
            }
            finally
            {
                mLogger.Debug("CreateTimeSeriesLookUpCollection --> End --> " + dailyJob.TableName);
            }

            return instIdVsDateVsTimeSeriesId;
        }

        public static Dictionary<string, string> GetSecurityIdVsMasterId(string downstreamConnectionName, HashSet<string> instruments)
        {
            Dictionary<string, string> securityIdVsMasterId = new Dictionary<string, string>();

            if (instruments != null && instruments.Count > 0)
            {
                string tableName = "[references].[" + Guid.NewGuid().ToString() + "]";
                try
                {
                    var table = new ObjectTable();
                    table.Columns.Add("inst_id", typeof(string));
                    foreach (var instId in instruments)
                        table.Rows.Add(instId);

                    SRMDWHJobExtension.ExecuteQuery(downstreamConnectionName, string.Format("CREATE TABLE {0}(inst_id VARCHAR(15))", tableName), SRMDBQueryType.INSERT);
                    SRMDWHJobExtension.ExecuteBulkCopyObject(downstreamConnectionName, tableName, table);

                    string query = string.Format(@"SELECT mas.[Security Id], mas.id 
                        FROM dimension.ivp_polaris_security_master mas
                        INNER JOIN {0} tab ON mas.[Security Id] = tab.inst_id", tableName);

                    var dt = SRMDWHJobExtension.ExecuteQueryObject(downstreamConnectionName, query, SRMDBQueryType.SELECT).Tables[0];
                    foreach (var row in dt.Rows)
                        securityIdVsMasterId.Add(Convert.ToString(row["Security Id"]), Convert.ToString(row["id"]));
                }
                catch
                {
                    throw;
                }
                finally
                {
                    SRMDWHJobExtension.ExecuteQuery(downstreamConnectionName, string.Format("IF(OBJECT_ID('{0}') IS NOT NULL) DROP TABLE {0}", tableName), SRMDBQueryType.INSERT);
                }
            }
            return securityIdVsMasterId;
        }

        public static Dictionary<string, string> GetEntityCodeVsMasterId(SRMJobInfo jobInfo, HashSet<string> instruments)
        {
            Dictionary<string, string> entityCodeVsMasterId = new Dictionary<string, string>();

            if (instruments != null && instruments.Count > 0)
            {
                string tableName = "[references].[" + Guid.NewGuid().ToString() + "]";
                try
                {
                    var table = new ObjectTable();
                    table.Columns.Add("inst_id", typeof(string));
                    foreach (var instId in instruments)
                        table.Rows.Add(instId);

                    SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, string.Format("CREATE TABLE {0}(inst_id VARCHAR(15))", tableName), SRMDBQueryType.INSERT);
                    SRMDWHJobExtension.ExecuteBulkCopyObject(jobInfo.DownstreamSQLConnectionName, tableName, table);

                    string query = string.Format(@"SELECT mas.entity_code, mas.master_id 
                        FROM dimension.ivp_srm_dwh_entity_type_master mas
                        INNER JOIN {0} tab ON mas.entity_code = tab.inst_id", tableName);

                    var dt = SRMDWHJobExtension.ExecuteQueryObject(jobInfo.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT).Tables[0];
                    foreach (var row in dt.Rows)
                        entityCodeVsMasterId.Add(Convert.ToString(row["entity_code"]), Convert.ToString(row["master_id"]));
                }
                catch
                {
                    throw;
                }
                finally
                {
                    SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, string.Format("IF(OBJECT_ID('{0}') IS NOT NULL) DROP TABLE {0}", tableName), SRMDBQueryType.INSERT);
                }
            }

            return entityCodeVsMasterId;
        }


        private static void SyncTableStructures(ObjectTable currentDataTable, DataSet ds, string destinationTableNameWithSchema, SRMJobInfo jobInfo, Dictionary<string, string> reportAttributeVsSchema, bool isStaging, bool dropTables, HashSet<string> refAttributesInTable, bool processLookupFromDaily)
        {
            HashSet<string> dictDataTableColumns = new HashSet<string>(currentDataTable.Columns.Cast<ObjectColumn>().Select(x => x.ColumnName).Union(reportAttributeVsSchema.Select(x => x.Key)), StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> dictDBDataColumns = ds.Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["COLUMN_NAME"]), y => Convert.ToString(y["DATA_TYPE"]), StringComparer.OrdinalIgnoreCase);
            Dictionary<string, Dictionary<string, string>> DBTableNameVsColumnVsDatatype = null;
            Dictionary<string, Dictionary<string, string>> DatatableTableNameVsColumn = null;
            var tableSplit = destinationTableNameWithSchema.Split(new char[] { '[', '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (!isStaging && jobInfo.BlockType == BLOCK_TYPES.TS_REPORT)
            {
                string prefix = "ivp_polaris_";
                string midfix = "_leg_";
                string securities = "securities";
                string entityTypeName = null;
                DatatableTableNameVsColumn = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
                DBTableNameVsColumnVsDatatype = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

                if (jobInfo.IsRef)
                    entityTypeName = tableSplit[1].Replace("_staging]", "").Replace("ivp_polaris_", "").Replace("_time_series]", "").Replace("_time_series", "");

                HashSet<string> tableNames = new HashSet<string>();
                mLogger.Debug("dictDataTableColumns -> " + string.Join(",", dictDataTableColumns));
                foreach (var col in dictDataTableColumns)
                {
                    StringBuilder sb = new StringBuilder(prefix);
                    var arr = col.ToLower().Split('-');
                    if (arr.Length > 1)
                    {
                        var trimmed0 = arr[0].Trim();
                        if (trimmed0 != "ec")
                        {
                            var legName = trimmed0.Replace(" ", "_");
                            StringBuilder columnName = new StringBuilder(col.Length);

                            for (var c = 1; c < arr.Length; c++)
                            {
                                columnName.Append(arr[c]);
                                if (c < arr.Length - 1)
                                    columnName.Append("-");
                            }

                            if (jobInfo.IsRef)
                                sb.Append(entityTypeName).Append(midfix).Append(legName).Append("_time_series");
                            else
                                sb.Append(securities).Append(midfix).Append(legName).Append("_time_series");
                            var finalTableName = sb.ToString();

                            tableNames.Add(finalTableName);
                            Dictionary<string, string> columns = null;
                            if (!DatatableTableNameVsColumn.TryGetValue(finalTableName, out columns))
                            {
                                columns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                                DatatableTableNameVsColumn[finalTableName] = columns;
                            }
                            columns.Add(columnName.ToString(), col);
                        }
                    }
                }
                if (dropTables && jobInfo.DateType == DWHDateType.None)
                {
                    foreach (var tableName in tableNames)
                    {
                        string query2 = string.Format(@"DELETE FROM taskmanager.ivp_polaris_core_secref_extract_table_column_mapping 
                                                        where identifier_key ='[dimension].[{0}]'", tableName);
                        SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, query2, SRMDBQueryType.DELETE);
                    }
                }
                else
                {
                    string query = string.Format(@"SELECT TABLE_NAME, COLUMN_NAME, 
                    CASE WHEN DATA_TYPE = 'VARCHAR' THEN 'VARCHAR(' + CASE WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN 'MAX' ELSE CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR(MAX)) END + ')'
	                    WHEN DATA_TYPE IN ('NUMERIC','DECIMAL') THEN DATA_TYPE + '(' + CAST(NUMERIC_PRECISION AS VARCHAR(MAX)) + ',' + CAST(NUMERIC_SCALE AS VARCHAR(MAX)) + ')'
	                    ELSE DATA_TYPE
	                    END AS DATA_TYPE
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = 'dimension' AND TABLE_NAME IN ('{0}')", string.Join("','", tableNames));
                    DataSet ds2 = SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);
                    foreach (var row in ds2.Tables[0].AsEnumerable())
                    {
                        var tableName = Convert.ToString(row["TABLE_NAME"]);
                        Dictionary<string, string> columnVsDatatype = null;
                        if (!DBTableNameVsColumnVsDatatype.TryGetValue(tableName, out columnVsDatatype))
                        {
                            columnVsDatatype = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            DBTableNameVsColumnVsDatatype[tableName] = columnVsDatatype;
                        }
                        columnVsDatatype.Add(Convert.ToString(row["COLUMN_NAME"]), Convert.ToString(row["DATA_TYPE"]));
                    }
                }
            }

            if (!(dropTables && jobInfo.DateType == DWHDateType.None))
            {
                HashSet<string> systemColumnsToIgnore = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
                    "entity_code",
                    "Security Id",
                    "Security Key",
                    "Effective Start Date",
                    "Effective End Date",
                    "Security Type Name",
                    "Entity Type Name",
                    "last_modified_by",
                    "Action Date",
                    "isArchiveRecord",
                    "isTimeSeriesRecord",
                    "Action Identifier"
                };

                StringBuilder queryString = new StringBuilder();
                var canAlterDBTable = SRMDWHJob.CanAlterTable(jobInfo.DownstreamSQLConnectionName, tableSplit[1].Replace("_staging]", "").Replace("]", ""));
                foreach (var column in dictDataTableColumns)
                {
                    if (!systemColumnsToIgnore.Contains(column) && (isStaging || !(jobInfo.BlockType == BLOCK_TYPES.TS_REPORT && (column.Contains('-') && !column.StartsWith("EC-", StringComparison.OrdinalIgnoreCase)))))
                    {
                        string DBDatatype = null;
                        string reportBasedDBDataType = GetDBDataType(column, reportAttributeVsSchema, refAttributesInTable, jobInfo, processLookupFromDaily);
                        if (dictDBDataColumns.TryGetValue(column, out DBDatatype))
                        {
                            if (canAlterDBTable && !reportBasedDBDataType.Equals(DBDatatype, StringComparison.OrdinalIgnoreCase))
                            {
                                if (IsComparableDataType(DBDatatype, reportBasedDBDataType))
                                    queryString.Append("ALTER TABLE ").Append(destinationTableNameWithSchema).Append(" ALTER COLUMN [").Append(column).Append("] ").Append(reportBasedDBDataType).Append(Environment.NewLine);
                                else
                                    throw new Exception("Cannot Sync the datatype for " + column + " as it's datatype is changing from " + DBDatatype + " to " + reportBasedDBDataType);
                            }
                        }
                        else
                            queryString.Append("ALTER TABLE ").Append(destinationTableNameWithSchema).Append(" ADD [").Append(column).Append("] ").Append(reportBasedDBDataType).Append(Environment.NewLine);
                    }
                }

                if (!isStaging && jobInfo.BlockType == BLOCK_TYPES.TS_REPORT)
                {
                    foreach (var tableLevel in DatatableTableNameVsColumn)
                    {
                        Dictionary<string, string> columnVsdatatype = null;
                        if (DBTableNameVsColumnVsDatatype.TryGetValue(tableLevel.Key, out columnVsdatatype))
                        {
                            string DBTableName = "[dimension].[" + tableLevel.Key + "]";
                            canAlterDBTable = SRMDWHJob.CanAlterTable(jobInfo.DownstreamSQLConnectionName, DBTableName);
                            foreach (var column in tableLevel.Value)
                            {
                                string datatype = null;
                                string reportBasedDBDataType = GetDBDataType(column.Value, reportAttributeVsSchema, refAttributesInTable, jobInfo, processLookupFromDaily);
                                if (columnVsdatatype.TryGetValue(column.Key, out datatype))
                                {
                                    if (canAlterDBTable && !reportBasedDBDataType.Equals(datatype, StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (IsComparableDataType(datatype, reportBasedDBDataType))
                                            queryString.Append("ALTER TABLE ").Append(DBTableName).Append(" ALTER COLUMN [").Append(column.Key).Append("] ").Append(reportBasedDBDataType).Append(Environment.NewLine);
                                        else
                                            throw new Exception("Cannot Sync the datatype for " + column.Key + " as it's datatype is changing from " + datatype + " to " + reportBasedDBDataType);
                                    }
                                }
                                else
                                    queryString.Append("ALTER TABLE ").Append(DBTableName).Append(" ADD [").Append(column.Key).Append("] ").Append(reportBasedDBDataType).Append(Environment.NewLine);
                            }
                        }
                    }
                }
                if (queryString.Length > 0)
                {
                    if (!isStaging)
                    {
                        RDBConnectionManager con = RDALAbstractFactory.DBFactory.GetConnectionManager(jobInfo.DownstreamSQLConnectionName);
                        con.IsolationLevel = IsolationLevel.RepeatableRead;
                        con.UseTransaction = true;
                        try
                        {
                            SRMDWHJobExtension.ExecuteQuery(con, queryString.ToString(), SRMDBQueryType.SELECT);

                            foreach (var info in SRMDWHStatic.dctSetupIdVsDWHAdapterDetails)
                            {
                                foreach (var adapterInfo in info.Value)
                                {
                                    var error = DWHAdapterController.SyncTableSchema(adapterInfo.AdapterId, queryString.ToString());
                                    if (error.HasValue)
                                        throw new Exception(error.Value.Message);
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
                    else
                        SRMDWHJobExtension.ExecuteQuery(jobInfo.DownstreamSQLConnectionName, queryString.ToString(), SRMDBQueryType.SELECT);
                }
            }
        }

        private static bool IsComparableDataType(string oldDatatype, string newDatatype)
        {
            oldDatatype = oldDatatype.ToUpper();
            newDatatype = newDatatype.ToUpper();
            if (oldDatatype == "DATETIME" && newDatatype == "DATETIME")
                return true;
            else if (oldDatatype == "DATE" && newDatatype == "DATETIME")
                return true;
            else if (oldDatatype == "DATE" && newDatatype == "DATE")
                return true;
            else if (oldDatatype == "BIT" && newDatatype == "BIT")
                return true;
            else if (oldDatatype == "INT" && newDatatype == "INT")
                return true;
            else if (oldDatatype == "INT" && newDatatype == "BIGINT")
                return true;
            else if (oldDatatype == "BIGINT" && newDatatype == "BIGINT")
                return true;
            else if (oldDatatype.Contains("NUMERIC") && newDatatype.Contains("NUMERIC"))
                return true;
            else if (oldDatatype.Contains("DECIMAL") && newDatatype.Contains("DECIMAL"))
                return true;
            else if (oldDatatype.Contains("NUMERIC") && newDatatype.Contains("DECIMAL"))
                return true;
            else if (oldDatatype.Contains("DECIMAL") && newDatatype.Contains("NUMERIC"))
                return true;
            else if (oldDatatype.Contains("VARCHAR") && newDatatype.Contains("VARCHAR"))
                return true;
            else
                return false;
        }

        private static string GetDBDataType(string columnName, Dictionary<string, string> reportAttributeVsSourceDataTypeAndDecimalPlaces, HashSet<string> refAttributes, SRMJobInfo jobInfo, bool processLookupFromDaily)
        {
            string dbDataType = null;

            if (jobInfo.RequireTimeTSReports && (columnName.Equals("Effective Start Date", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Effective End Date", StringComparison.OrdinalIgnoreCase)))
                dbDataType = "DATETIME";

            else if (columnName.Equals("Effective Date", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Effective Start Date", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Effective End Date", StringComparison.OrdinalIgnoreCase))
                dbDataType = "DATE";

            else if (columnName.Equals("Last Modified On", StringComparison.OrdinalIgnoreCase) || columnName.Equals("loading_time", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Action Date", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Knowledge Date", StringComparison.OrdinalIgnoreCase) || columnName.Equals("created_on", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Created On", StringComparison.OrdinalIgnoreCase))
                dbDataType = "DATETIME";

            else if (columnName.Equals("entity_code", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Security Id", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Security Key", StringComparison.OrdinalIgnoreCase))
                dbDataType = "VARCHAR(15)";

            else if (columnName.Equals("Entity Type Name", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Security Type Name", StringComparison.OrdinalIgnoreCase))
                dbDataType = "VARCHAR(100)";

            else if (columnName.Equals("Action Identifier", StringComparison.OrdinalIgnoreCase) || columnName.Equals("last_modified_by", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Last Modified By", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Modified By", StringComparison.OrdinalIgnoreCase) || columnName.Equals("created_by", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Created By", StringComparison.OrdinalIgnoreCase))
                dbDataType = "VARCHAR(200)";

            else if (columnName.Equals("Entity_Type", StringComparison.OrdinalIgnoreCase) || columnName.Equals("Row Id", StringComparison.OrdinalIgnoreCase))
                dbDataType = "VARCHAR(500)";

            else if (columnName.Equals("isTimeSeriesRecord", StringComparison.OrdinalIgnoreCase) || columnName.Equals("is_active", StringComparison.OrdinalIgnoreCase) || columnName.Equals("isArchiveRecord", StringComparison.OrdinalIgnoreCase) || columnName.Equals("is_deleted", StringComparison.OrdinalIgnoreCase) || columnName.Equals("is_leg_deleted", StringComparison.OrdinalIgnoreCase))
                dbDataType = "BIT";

            else if (columnName.Contains("_surrogate_id") && processLookupFromDaily)
                dbDataType = "BIGINT";

            else if ((columnName.Contains("_surrogate_id") && !processLookupFromDaily) || columnName.Equals("master_id"))
                dbDataType = "INT";

            else if ((columnName.StartsWith("timeseries_") && jobInfo.BlockType.Equals(BLOCK_TYPES.DAILY_REPORT)))
                dbDataType = "INT";

            else if (refAttributes != null && refAttributes.Count > 0 && refAttributes.Contains(columnName))
                dbDataType = "VARCHAR(15)";

            if (dbDataType == null && !reportAttributeVsSourceDataTypeAndDecimalPlaces.TryGetValue(columnName, out dbDataType))
                throw new Exception(string.Format("Please map the report attribute {0} or remove it from Attributes To Show in report configuration.", columnName));

            return dbDataType;
        }

        private static void DeleteInactiveRows(SRMJobInfo job)
        {
            string query = string.Empty;
            if (job.BlockType == BLOCK_TYPES.NTS_REPORT)
            {
                query = string.Format(@"DECLARE @isNonTSExtract BIT = 1, @isRefMExtract BIT = '{1}'
                        DECLARE @stagingTableName VARCHAR(MAX) = '{0}'

                        DECLARE @masterTableName VARCHAR(MAX) = '', @tsTableName VARCHAR(MAX) = ''
                        DECLARE @query VARCHAR(MAX) = '', @tableName VARCHAR(MAX) = '', @identifierName VARCHAR(500)

                        SELECT @identifierName = CASE WHEN @isRefMExtract = 1 THEN '[entity_code]' ELSE '[Security Id]' END

                        SELECT 
	                        @stagingTableName = table_name,
	                        @masterTableName = surrogate_table_name, 
	                        @tsTableName = ts_table_name,
	                        @tableName = REPLACE(REPLACE(REPLACE(table_name , 'references','dimension'),'taskmanager','dimension'),'_staging','')
	                        FROM [dimension].ivp_srm_dwh_tables_for_loading WHERE table_type = 
	                        CASE WHEN @isNonTSExtract =1 THEN 'NONTS' ELSE 'DAILY' END
	                        AND is_active = 1 AND table_name LIKE '%'+ REPLACE(PARSENAME(@stagingTableName,1),'_','[_]') + '%'

	                        SELECT @query = '
                                    IF EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = PARSENAME('''+@tableName+''',1) AND TABLE_SCHEMA = PARSENAME('''+@tableName+''',2))
                                    BEGIN 
                                            DELETE TAB
                                            FROM '+ @tableName +' TAB
                                            INNER JOIN '+ @masterTableName + ' MTAB
                                            ON (TAB .' + @identifierName + ' = MTAB.' + @identifierName  + ')
                                            WHERE MTAB.is_deleted = 1 
                                    END'
	                        PRINT(@query)
	                        EXEC(@query)

                            SELECT @tableName AS [TableName], @masterTableName AS [MasterTableName]", job.TableName, job.IsRef);
            }
            else
            {
                query = string.Format(@"DECLARE @sql VARCHAR(MAX) = ''
                        DECLARE @stagingTableName VARCHAR(MAX) = '{0}' , @isRefm BIT = '{1}'

                        SELECT @sql = ''

                        SELECT @sql += CHAR(13) + 'IF EXISTS(SELECT TOP 1 1 FROM information_schema.tables WHERE table_name = PARSENAME(''' + REPLACE(REPLACE(REPLACE(table_name, '_staging', ''), 'references', 'dimension'), 'taskmanager', 'dimension') + ''',1))
                            BEGIN DELETE leg FROM  ' + REPLACE(REPLACE(REPLACE(table_name, '_staging', ''), 'references', 'dimension'), 'taskmanager', 'dimension') + ' leg INNER JOIN ' + surrogate_table_name + ' master  ON ' + ' (leg.' + CASE 
		                        WHEN isRefM = 1
			                        THEN 'entity_code'
		                        ELSE CASE WHEN table_type = 'TS' THEN '[Security_id]' ELSE '[Security Id]' END
		                        END + ' = master.' + CASE 
		                        WHEN isRefM = 1
			                        THEN 'entity_code'
		                        ELSE '[Security Id]'
		                        END + ')
                                WHERE master.is_deleted = 1
                            END'
                        FROM dimension.ivp_srm_dwh_tables_for_loading
                        WHERE is_active = 1
	                        AND table_type <> 'NONTS' AND table_name LIKE '%'+ REPLACE(PARSENAME(@stagingTableName,1),'_','[_]') + '%'

                        EXEC (@sql)

                        SELECT REPLACE(REPLACE(REPLACE(table_name, '_staging', ''), 'references', 'dimension'), 'taskmanager', 'dimension') AS [TableName],surrogate_table_name AS [MasterTableName]
                            FROM dimension.ivp_srm_dwh_tables_for_loading
                            WHERE is_active = 1 AND table_type <> 'NONTS' AND table_name LIKE '%'+ REPLACE(PARSENAME(@stagingTableName,1),'_','[_]') + '%' ", job.TableName, job.IsRef);
                if (job.BlockType == BLOCK_TYPES.TS_REPORT)
                {
                    query += string.Format(@"                                       
                        IF (OBJECT_ID('tempdb..#tsLegs') IS NOT NULL)
	                        DROP TABLE #tsLegs

                        CREATE TABLE #tsLegs (table_name VARCHAR(MAX),surrogate_table_name VARCHAR(MAX),isRefm BIT)

                        INSERT INTO #tsLegs (table_name)
                        SELECT DISTINCT identifier_key
                        FROM taskmanager.ivp_polaris_core_secref_extract_table_column_mapping map
                        INNER JOIN INFORMATION_SCHEMA.TABLES tab ON tab.TABLE_NAME = PARSENAME(map.identifier_key,1) AND tab.TABLE_SCHEMA = PARSENAME(map.identifier_key,2)
                        WHERE identifier_key LIKE '%[_]leg[_]%'	AND is_active = 1 AND 
                        identifier_key LIKE 
                        CASE WHEN @isRefm = 1 THEN '%'+ REPLACE(REPLACE(PARSENAME(@stagingTableName,1),'_time_series_staging',''),'_','[_]') + '%'
                        ELSE '%ivp_polaris_securities%' END

                        UPDATE #tsLegs
                        SET isRefm = 0
	                        ,surrogate_table_name = '[dimension].[ivp_polaris_security_master]'
                        WHERE table_name LIKE '%ivp_polaris_securities%'

                        UPDATE #tsLegs
                        SET isRefm = 1
	                        ,surrogate_table_name = '[dimension].[ivp_polaris_' + SUBSTRING(REPLACE(table_name, '[dimension].[ivp_polaris_', ''), 0, CHARINDEX('_leg', REPLACE(table_name, '[dimension].[ivp_polaris_', ''))) + '_master]'
                        WHERE isRefm IS NULL

                        SELECT @sql = ''

                        SELECT @sql += CHAR(13) + 'IF EXISTS(SELECT TOP 1 1 FROM information_schema.tables WHERE table_name = PARSENAME(''' + table_name + ''',1))
                            BEGIN DELETE leg FROM  ' + table_name + ' leg INNER JOIN ' + surrogate_table_name + ' master  ON ' + ' (leg.' + CASE 
		                        WHEN isRefM = 1
			                        THEN 'entity_code'
		                        ELSE '[Security_id]'
		                        END + ' = master.' + CASE 
		                        WHEN isRefM = 1
			                        THEN 'entity_code'
		                        ELSE '[Security Id]'
		                        END + ')
                                WHERE master.is_deleted = 1
                            END'
                        FROM #tsLegs

                        PRINT(@sql)
                        --SELECT @sql AS [processing-instruction(x)] FOR XML PATH('')
                        EXEC (@sql)

                        SELECT table_name AS [TableName], surrogate_table_name AS [MasterTableName] FROM #tsLegs
                        DROP TABLE #tsLegs");
                }
            }

            DataSet ds = SRMDWHJobExtension.ExecuteQuery(job.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);

            if (SRMDWHStatic.dctSetupIdVsDWHAdapterDetails.ContainsKey(job.SetupId))
            {
                foreach (DataTable dt in ds.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string tableName = Convert.ToString(dr["TableName"]);
                        string masterTableName = Convert.ToString(dr["MasterTableName"]);

                        foreach (var info in SRMDWHStatic.dctSetupIdVsDWHAdapterDetails)
                        {
                            foreach (var adapterInfo in info.Value)
                            {
                                DWHAdapterController.DeleteInactiveInstruments(adapterInfo.AdapterId, masterTableName, tableName);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        static Dictionary<string, Tuple<string, DateTime>> GetSecurityIdVsSecurityKey()
        {
            string query = @"SELECT sk.sec_id, sk.security_key, sm.created_on 
                    FROM IVPSecMaster.dbo.ivp_secm_security_keys sk
                    INNER JOIN IVPSecMaster.dbo.ivp_secm_sec_master sm ON sk.sec_id = sm.sec_id";
            return CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection)
                .Tables[0]
                .AsEnumerable()
                .ToDictionary(x => Convert.ToString(x["sec_id"]), y => new Tuple<string, DateTime>(Convert.ToString(y["security_key"]), Convert.ToDateTime(y["created_on"])));
        }

        static HashSet<string> GetDeletedInstruments(bool isRefM, out HashSet<string> purgedInstruments)
        {
            purgedInstruments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (isRefM)
            {
                string queryString = "SELECT deleted_identifier as entity_code FROM IVPRefMaster.dbo.ivp_refm_deletion_summary WHERE deletion_type_id = 1";
                return new HashSet<string>(CommonDALWrapper.ExecuteSelectQuery(queryString, ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["entity_code"])), StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                var hshDeleted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT sec_id FROM IVPSecMaster.dbo.ivp_secm_security_deletion_history; 
                    SELECT deletion_info 
                    FROM IVPSecMaster.dbo.ivp_secm_deletion_summary ds
                    LEFT JOIN IVPSecMaster.dbo.ivp_secm_sec_master sm ON (ds.deletion_info = sm.sec_id and sm.is_active = 1)
                    WHERE deletion_type = 'security' AND sm.sec_id IS NULL", ConnectionConstants.RefMaster_Connection);

                foreach (var row in ds.Tables[0].AsEnumerable())
                    hshDeleted.Add(Convert.ToString(row["sec_id"]));

                foreach (var row in ds.Tables[1].AsEnumerable())
                {
                    var purgedSecurity = Convert.ToString(row["deletion_info"]);
                    hshDeleted.Add(purgedSecurity);
                    purgedInstruments.Add(purgedSecurity);
                }

                return hshDeleted;
            }
        }

        public static List<HashSet<int>> GetEntityTypeExecutionOrder(Dictionary<int, List<int>> entityTypeVsLookUpTypes, out List<int> cyclicEntityTypes)
        {
            mLogger.Debug("GetEntityTypeExecutionOrder -> START");


            cyclicEntityTypes = new List<int>();
            List<HashSet<int>> hshExecutionOrder = new List<HashSet<int>>();
            List<EntityTypeNode> roots = new List<EntityTypeNode>();

            Dictionary<int, HashSet<int>> entityTypeVsRefferedByTypes = new Dictionary<int, HashSet<int>>();
            HashSet<int> hshEntityType = new HashSet<int>();
            foreach (var entityTypeLevel in entityTypeVsLookUpTypes)
            {
                HashSet<int> referredByTypes = null;
                foreach (var lookup in entityTypeLevel.Value)
                {
                    if (lookup != entityTypeLevel.Key)
                    {
                        if (!entityTypeVsRefferedByTypes.TryGetValue(lookup, out referredByTypes))
                        {
                            referredByTypes = new HashSet<int>();
                            entityTypeVsRefferedByTypes[lookup] = referredByTypes;
                            hshEntityType.Add(lookup);
                        }
                        referredByTypes.Add(entityTypeLevel.Key);
                    }
                }

                if (!entityTypeVsRefferedByTypes.TryGetValue(entityTypeLevel.Key, out referredByTypes))
                {
                    referredByTypes = new HashSet<int>();
                    entityTypeVsRefferedByTypes[entityTypeLevel.Key] = referredByTypes;
                    hshEntityType.Add(entityTypeLevel.Key);
                }
            }

            var emptyReferred = new HashSet<int>();
            foreach (var entity in entityTypeVsRefferedByTypes)
            {
                if (entity.Value.Count == 0)
                {
                    hshEntityType.Remove(entity.Key);
                    emptyReferred.Add(entity.Key);
                }
            }

            HashSet<int> addedInExecutionOrder = new HashSet<int>();
            var hasCyclicDependency = false;
            while (hshEntityType.Count > 0)
            {
                var entityTypesToRemove = new HashSet<int>();
                foreach (var entityType in hshEntityType)
                {
                    var foundInList = false;
                    foreach (var checkEntityType in hshEntityType)
                    {
                        if (checkEntityType != entityType)
                        {
                            if (!addedInExecutionOrder.Contains(checkEntityType) && entityTypeVsRefferedByTypes[checkEntityType].Contains(entityType))
                                foundInList = true;
                        }
                    }

                    if (!foundInList)
                    {
                        var maxLevel = 0;
                        for (var level = hshExecutionOrder.Count - 1; level > -1; level--)
                        {
                            var foundInLevel = false;
                            foreach (var entity in hshExecutionOrder[level])
                            {
                                if (entityTypeVsRefferedByTypes[entity].Contains(entityType))
                                {
                                    foundInLevel = true;
                                    break;
                                }
                            }

                            if (foundInLevel)
                            {
                                maxLevel = level + 1;
                                break;
                            }
                        }

                        if (hshExecutionOrder.Count - 1 < maxLevel)
                            hshExecutionOrder.Add(new HashSet<int>());
                        hshExecutionOrder[maxLevel].Add(entityType);

                        addedInExecutionOrder.Add(entityType);
                        entityTypesToRemove.Add(entityType);
                    }
                }

                if (entityTypesToRemove.Count == 0)
                {
                    hasCyclicDependency = true;
                    break;
                }
                else
                {
                    foreach (var entity in entityTypesToRemove)
                        hshEntityType.Remove(entity);
                }
            }

            if (!hasCyclicDependency)
            {
                hshExecutionOrder.Add(new HashSet<int>());
                foreach (var entity in emptyReferred)
                    hshExecutionOrder[hshExecutionOrder.Count - 1].Add(entity);
            }
            else
            {
                foreach (var entity in hshEntityType)
                    cyclicEntityTypes.Add(entity);
            }


            mLogger.Debug("GetEntityTypeExecutionOrder -> END");
            return hshExecutionOrder;
        }

        public static void SyncTimeSeriesIdInDailyTable(SRMJobInfo jobinfo, List<string> timeseriesSecurities, RDBConnectionManager con, SRMJobInfo tsReportJob, string dailyDimTableNameWithSchema, string tsDimTableNameWithSchema, List<string> tablesToBeDropped)
        {
            mLogger.Debug("SyncTimeSeriesIdInDailyTable -> START");
            try
            {

                if (!string.IsNullOrEmpty(jobinfo.TimeSeriesTableNameWithSchema))
                {
                    var arr = jobinfo.TableName.Split(new char[] { '.', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    var mainDailyTable = new StringBuilder("[").Append("dimension").Append("].[").Append(arr[1].Replace("_staging", "")).Append("]").ToString();

                    if (!string.IsNullOrEmpty(dailyDimTableNameWithSchema))
                        mainDailyTable = dailyDimTableNameWithSchema;

                    string instColumn = "entity_code";
                    if (!jobinfo.IsRef)
                        instColumn = "Security Key";

                    List<string> columns = new List<string>();
                    string query = string.Format(@"SELECT COLUMN_NAME 
                                FROM INFORMATION_SCHEMA.COLUMNS 
                                WHERE TABLE_NAME = PARSENAME('{0}',1) AND ((COLUMN_NAME LIKE 'timeseries[_]%' 
                                    AND COLUMN_NAME LIKE '%[_]daily[_]id') OR (COLUMN_NAME LIKE '%[_]daily[_]surrogate[_]id')) ", mainDailyTable);
                    if (con != null)
                        columns = con.ExecuteQueryObject(query, RQueryType.Select).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["COLUMN_NAME"])).ToList();
                    else
                        columns = SRMDWHJobExtension.ExecuteQueryObject(jobinfo.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["COLUMN_NAME"])).ToList();

                    var timeSeriesIdColumnn = columns.Where(x => x.Contains("timeseries")).First();
                    var surrogateIdColumnn = columns.Where(x => x.Contains("surrogate")).First();

                    ObjectTable dtInstIds = new ObjectTable();
                    dtInstIds.Columns.Add("instrument_id", typeof(string));
                    foreach (var instId in timeseriesSecurities)
                        dtInstIds.Rows.Add(instId);

                    string tableName = "[taskmanager].[" + Guid.NewGuid().ToString() + "]";
                    SRMDWHJobExtension.ExecuteQueryObject(jobinfo.DownstreamSQLConnectionName, string.Format("CREATE TABLE {0}([instrument_id] VARCHAR(15) PRIMARY KEY)", tableName), SRMDBQueryType.INSERT);
                    tablesToBeDropped.Add(tableName);
                    SRMDWHJobExtension.ExecuteBulkCopyObject(jobinfo.DownstreamSQLConnectionName, tableName, dtInstIds);

                    query = string.Format(@"DECLARE @table_name VARCHAR(200),
		                @isRefm BIT = '{0}',
		                @temp_inst_table VARCHAR(100) = '{1}',
		                @sql VARCHAR(MAX) = '',
		                @master_identifier_column VARCHAR(30),
                        @db_name VARCHAR(100),
                        @schema_name VARCHAR(20)


                        SELECT @db_name = DB_NAME()                    
                        SELECT @schema_name = CASE WHEN @isRefm = 1 THEN 'references' ELSE 'taskmanager' END

                        SELECT @table_name = PARSENAME(surrogate_table_name,1)
				        FROM dimension.ivp_srm_dwh_tables_for_loading
                        WHERE table_name = '[' + @db_name + '].[' + @schema_name + '].[' + PARSENAME('{2}',1) + ']' AND is_active = 1            

                        SELECT @master_identifier_column = CASE WHEN @isRefm = 1 THEN '[entity_code]' ELSE '[Security Id]' END
                        
                        IF(OBJECT_ID('tempdb..#master_ids') IS NOT NULL)
                            DROP TABLE #master_ids

	                    SELECT @sql = 'SELECT id
                                    INTO #master_ids
				                    FROM ' + @temp_inst_table + ' tab
					                INNER JOIN dimension.[' + @table_name + '] mas ON tab.instrument_id = mas.' + @master_identifier_column + '
                                    SELECT daily.[{5}] AS surrogate_id, daily.[Effective Date], daily.[{4}]
                                    FROM #master_ids tab 
                                    INNER JOIN {3} daily ON tab.[id] = daily.[master_id]

                                    DROP TABLE #master_ids ' 
                                    EXEC(@sql)
	                    ", jobinfo.IsRef.ToString().ToUpper(), tableName, jobinfo.TableName, mainDailyTable, instColumn, surrogateIdColumnn);

                    var instIdVsDateVsTimeSeriesId = CreateTimeSeriesLookUpCollection(jobinfo, ref tableName, dtInstIds, con, tsReportJob, tsDimTableNameWithSchema, false);
                    tablesToBeDropped.Add(tableName);

                    ObjectSet dailyDataDS = null;
                    if (con != null)
                        dailyDataDS = SRMDWHJobExtension.ExecuteQueryObject(con, query, SRMDBQueryType.SELECT);
                    else
                        dailyDataDS = SRMDWHJobExtension.ExecuteQueryObject(jobinfo.DownstreamSQLConnectionName, query, SRMDBQueryType.SELECT);

                    dailyDataDS.Tables[0].Columns.Add(timeSeriesIdColumnn, typeof(int));

                    foreach (var row in dailyDataDS.Tables[0].AsEnumerable())
                    {
                        var entityCode = Convert.ToString(row[instColumn]);
                        var effectiveDate = Convert.ToDateTime(row["Effective Date"]);

                        int timeSeriesId;
                        Dictionary<DateTime, int> dateVsId = null;
                        if (instIdVsDateVsTimeSeriesId.TryGetValue(entityCode, out dateVsId))
                            if (dateVsId.TryGetValue(effectiveDate, out timeSeriesId))
                                row[timeSeriesIdColumnn] = timeSeriesId;
                    }

                    tableName = "[taskmanager].[" + Guid.NewGuid().ToString() + "]";

                    SRMDWHJobExtension.ExecuteQueryObject(jobinfo.DownstreamSQLConnectionName, string.Format("CREATE TABLE {0}([surrogate_id] BIGINT, [{1}] INT)", tableName, timeSeriesIdColumnn), SRMDBQueryType.INSERT);
                    tablesToBeDropped.Add(tableName);
                    SRMDWHJobExtension.ExecuteBulkCopyObject(jobinfo.DownstreamSQLConnectionName, tableName, dailyDataDS.Tables[0], new Dictionary<string, string> { { "surrogate_id", "surrogate_id" }, { timeSeriesIdColumnn, timeSeriesIdColumnn } });

                    SRMDWHJobExtension.ExecuteQuery(jobinfo.DownstreamSQLConnectionName, string.Format("CREATE NONCLUSTERED INDEX idx_1 ON {0}([surrogate_id])", tableName), SRMDBQueryType.SELECT);

                    if (con != null)
                    {
                        SRMDWHJobExtension.ExecuteQueryObject(con, string.Format(@"UPDATE daily
                            SET daily.{0} = tab.{0}
                            FROM {1} daily
                            INNER JOIN {2} tab ON daily.[{3}] = tab.[surrogate_id]", timeSeriesIdColumnn, mainDailyTable, tableName, surrogateIdColumnn), SRMDBQueryType.DELETE);
                    }
                    else
                    {
                        SRMDWHJobExtension.ExecuteQueryObject(jobinfo.DownstreamSQLConnectionName, string.Format(@"UPDATE daily
                            SET daily.{0} = tab.{0}
                            FROM {1} daily
                            INNER JOIN {2} tab ON daily.[{3}] = tab.[surrogate_id]", timeSeriesIdColumnn, mainDailyTable, tableName, surrogateIdColumnn), SRMDBQueryType.DELETE);
                    }
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("SyncTimeSeriesIdInDailyTable -> ERROR : " + ee.ToString());
                throw;
            }
            finally
            {
                mLogger.Error("SyncTimeSeriesIdInDailyTable -> END");
            }
        }


    }

    struct EntityTypeNode
    {
        private int _entityTypeId;
        private List<EntityTypeNode> _childNodes;
        public int EntityTypeId { get { return _entityTypeId; } }
        public List<EntityTypeNode> ChildNodes { get { return _childNodes; } }
        public EntityTypeNode(int entityTypeId)
        {
            _entityTypeId = entityTypeId;
            _childNodes = new List<EntityTypeNode>();
        }
    }

    struct MasterTableInfo
    {
        public string Id { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDeleted { get; private set; }

        internal MasterTableInfo(string id, bool isActive, bool isDeleted)
        {
            Id = id;
            IsActive = isActive;
            IsDeleted = isDeleted;
        }

        public bool Equals(MasterTableInfo obj)
        {
            if (obj.IsActive == IsActive && obj.IsDeleted == IsDeleted)
                return true;

            return false;
        }
    }
}
