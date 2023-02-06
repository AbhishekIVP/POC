using com.ivp.commom.commondal;
using com.ivp.common.srmdwhjob;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DWHAdapter
{
    public interface IDWHAdapter
    {
        FailedSyncMetadata? SyncMasterTableData(string sQLServerConnectionName, MasterTableMetaData metadata);
        FailedSyncMetadata? SyncDailyTableData(string sQLServerConnectionName, TSAndDailyMetadata metadata, RDBConnectionManager connectionManager);
        FailedSyncMetadata? SyncNTSTableData(string sQLServerConnectionName, TableSyncMetadata metadata, RDBConnectionManager connectionManager);
        FailedSyncMetadata? DeleteInactiveInstruments(string masterTableName, string tableName);
        DWHError? SyncViewsSchema(RDBConnectionManager connectionManager, string viewDefinition);
        DWHError? SyncTableSchema(string tableDefinition);
        DWHError? SyncMasterTables(string sQLServerConnectionName);
    }

    public static class DWHAdapterController
    {
        static IRLogger logger = RLogFactory.CreateLogger("DWHAdapterController");
        static Dictionary<int, IDWHAdapter> Adapters = new Dictionary<int, IDWHAdapter>();
        public static void Register(int adapterId, IDWHAdapter adapter)
        {
            Adapters[adapterId] = adapter;
            logger.Error($"Registered Adapter with ID: {adapterId}");
        }

        static IDWHAdapter GetAdapter(int adapterId)
        {
            if (Adapters.ContainsKey(adapterId))
                return Adapters[adapterId];
            else
                throw new ArgumentException("Adapter is not registered.");
        }

        public static FailedSyncMetadata? SyncDailyTableData(int adapterId, string sQLServerConnectionName, TSAndDailyMetadata metadata, RDBConnectionManager connectionManager)
        {
            var adapter = GetAdapter(adapterId);
            return adapter.SyncDailyTableData(sQLServerConnectionName, metadata, connectionManager);
        }

        public static FailedSyncMetadata? SyncNTSTableData(int adapterId, string sQLServerConnectionName, TableSyncMetadata metadata, RDBConnectionManager connectionManager)
        {
            var adapter = GetAdapter(adapterId);
            return adapter.SyncNTSTableData(sQLServerConnectionName, metadata, connectionManager);
        }

        public static FailedSyncMetadata? DeleteInactiveInstruments(int adapterId, string masterTableName, string tableName)
        {
            var adapter = GetAdapter(adapterId);
            return adapter.DeleteInactiveInstruments(masterTableName, tableName);
        }

        public static DWHError? SyncViewsSchema(int adapterId, RDBConnectionManager connectionManager, string viewDefinition)
        {
            var adapter = GetAdapter(adapterId);
            return adapter.SyncViewsSchema(connectionManager, viewDefinition);
        }

        public static DWHError? SyncTableSchema(int adapterId, string tableDefinition)
        {
            var adapter = GetAdapter(adapterId);
            return adapter.SyncTableSchema(tableDefinition);
        }

        public static DWHError? SyncMasterTables(int adapterId, string sQLServerConnectionName)
        {
            var adapter = GetAdapter(adapterId);
            return adapter.SyncMasterTables(sQLServerConnectionName);
        }

        public static FailedSyncMetadata? SyncMasterTableData(int adapterId, string sQLServerConnectionName, MasterTableMetaData metadata)
        {
            var adapter = GetAdapter(adapterId);
            return adapter.SyncMasterTableData(sQLServerConnectionName, metadata);
        }

        private static void LoadDWHAdapterDetails(int setupId)
        {
            SRMDWHStatic.dctSetupIdVsDWHAdapterDetails = new ConcurrentDictionary<int, List<DWHAdapterDetails>>();

            string query = String.Empty;
            if (setupId > 0)
                query = string.Format(@"SELECT * FROM dbo.ivp_srm_dwh_adapter_master WHERE setup_id = {0}", setupId);
            else
                query = "SELECT * FROM dbo.ivp_srm_dwh_adapter_master";

            ObjectSet os = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection);

            if (os != null && os.Tables.Count > 0 && os.Tables[0].Rows.Count > 0)
            {
                var ot = os.Tables[0];
                List<DWHAdapterDetails> lstAdapterDetails;
                foreach (ObjectRow or in ot.Rows)
                {
                    int setup_id = Convert.ToInt32(or["setup_id"]);
                    int adapter_id = Convert.ToInt32(or["adapter_id"]);
                    string adapter_name = Convert.ToString(or["adapter_name"]);
                    string assembly_path = Convert.ToString(or["assembly_path"]);
                    string class_name = Convert.ToString(or["class_name"]);

                    if (!SRMDWHStatic.dctSetupIdVsDWHAdapterDetails.TryGetValue(setup_id, out lstAdapterDetails))
                    {
                        lstAdapterDetails = new List<DWHAdapterDetails>();
                        SRMDWHStatic.dctSetupIdVsDWHAdapterDetails.TryAdd(setup_id, lstAdapterDetails);
                    }
                    lstAdapterDetails.Add(new DWHAdapterDetails()
                    {
                        AdapterName = adapter_name,
                        AssemblyPath = assembly_path,
                        ClassName = class_name,
                        SetupId = setup_id,
                        AdapterId = adapter_id
                    });
                }
            }
        }

        private static Dictionary<int, string> GetSetupVsConnectionDetails()
        {
            var setupIdVsConnectionName = new Dictionary<int, string>();
            string query = "SELECT DISTINCT setup_id, connection_name FROM dbo.ivp_srm_dwh_downstream_master WHERE is_active = 1";

            ObjectSet os = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection);

            if (os != null && os.Tables.Count > 0)
            {
                var ot = os.Tables[0];
                foreach (ObjectRow or in ot.Rows)
                {
                    int setupId = Convert.ToInt32(or["setup_id"]);
                    var connectionName = Convert.ToString(or["connection_name"]);
                    setupIdVsConnectionName.Add(setupId, connectionName);
                }
            }
            return setupIdVsConnectionName;
        }

        public static void RegisterDWHAdapter(int setupId, bool loadMasterTables)
        {
            Dictionary<int, string> setupIdVsConnectionName = null;
            LoadDWHAdapterDetails(setupId);

            if (loadMasterTables)
                setupIdVsConnectionName = GetSetupVsConnectionDetails();

            foreach (var info in SRMDWHStatic.dctSetupIdVsDWHAdapterDetails)
            {
                foreach (var adapterInfo in info.Value)
                {
                    Assembly assembly = null;

                    if (File.Exists(adapterInfo.AssemblyPath))
                        assembly = Assembly.LoadFile(adapterInfo.AssemblyPath);
                    else
                    {
                        FileInfo dllFile = null;

                        if (adapterInfo.AssemblyPath.Contains("bin\\"))
                            dllFile = new FileInfo(adapterInfo.AssemblyPath);
                        else
                            dllFile = new FileInfo("bin\\" + adapterInfo.AssemblyPath);

                        assembly = Assembly.LoadFile(dllFile.FullName);
                    }

                    Type objType = null;
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.FullName.Equals(adapterInfo.ClassName, StringComparison.OrdinalIgnoreCase))
                        {
                            objType = type;
                            break;
                        }
                    }
                    MethodInfo initAdapter = objType.GetMethod("InitAdapter", BindingFlags.Static | BindingFlags.Public);
                    initAdapter.Invoke(null, new object[] { adapterInfo.AdapterId });

                    if (loadMasterTables)
                        SyncMasterTables(adapterInfo.AdapterId, setupIdVsConnectionName[info.Key]);
                }
            }
        }
    }

    public struct TSAndDailyMetadata
    {
        public TableSyncMetadata[] DailyMetadata { get; private set; }
        public TableSyncMetadata[] TSMetadata { get; private set; }

        public TSAndDailyMetadata(TableSyncMetadata[] dailyMetadata, TableSyncMetadata[] tSMetadata)
        {
            DailyMetadata = dailyMetadata;
            TSMetadata = tSMetadata;
        }
    }

    public struct TableSyncMetadata
    {
        public bool IsLegTable { get; private set; }
        public bool IsNoneToNow { get; private set; }
        public DateTime LoadingTime { get; private set; }
        public string TableName { get; private set; }

        public TableSyncMetadata(bool isLegTable, bool isNoneToNow, DateTime loadingTime, string tableName)
        {
            IsLegTable = isLegTable;
            IsNoneToNow = isNoneToNow;
            LoadingTime = loadingTime;
            TableName = tableName;
        }
    }

    public struct FailedSyncMetadata
    {
        public string ErrorMessage { get; private set; }

        public FailedSyncMetadata(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }

    public struct DWHError
    {
        public string Message { get; private set; }

        public DWHError(string message)
        {
            Message = message;
        }
    }

    public struct MasterTableMetaData
    {
        public string TableName { get; private set; }
        public bool IsRefM { get; private set; }
        public bool IsNoneToNow { get; private set; }
        public DateTime LoadingTime { get; private set; }

        public MasterTableMetaData(string tableName, bool isRefM, bool isNoneToNow, DateTime loadingTime)
        {
            TableName = tableName;
            IsRefM = isRefM;
            IsNoneToNow = isNoneToNow;
            LoadingTime = loadingTime;
        }
    }
}
