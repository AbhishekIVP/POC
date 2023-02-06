using com.ivp.commom.commondal;
using com.ivp.rad.dal;
//using com.ivp.rad.util.types.list;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.TransportTasks
{
    class TransportTasksDBManager
    {
        public static DataSet getIdsFromDb(Dictionary<string, int> taskNameVsID)
        {
            var ds = CommonDALWrapper.ExecuteSelectQuery(@"
            SELECT  TS.task_name,TS.task_master_id, TFD.transport_name, TFD.transport_details_id, TFD.remote_file_name, TFD.remote_file_location,TFD.local_file_name
            FROM IVPRefMasterVendor.dbo.ivp_refm_task_summary TS LEFT JOIN 
            IVPRefMasterVendor.dbo.ivp_refm_transport_task_details TFD ON (TFD.transport_master_id= TS.task_master_id AND TS.is_active = 1 AND TFD.is_active = 1)
            INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_task_type TT ON (TT.task_type_id = TS.task_type_id AND TS.is_active = 1)
             WHERE TT.task_type_id = 1", ConnectionConstants.RefMasterVendor_Connection);

            foreach (var row in ds.Tables[0].AsEnumerable())
            {
                if (taskNameVsID != null && !taskNameVsID.ContainsKey(row.Field<string>("task_name")))
                    taskNameVsID.Add(row.Field<string>("task_name"), row.Field<int>("task_master_id"));
                //if (transportNameVSId != null)
                //    transportNameVSId.Add(row.Field<string>("transport_name"), row.Field<int>("transport_details_id"));
            }
            return ds;
        }
        public static DataSet getTaskAndTransportIdsFromDB(Dictionary<string, int> taskNameVsID)
        {
            var ds = CommonDALWrapper.ExecuteSelectQuery(@"
            SELECT  TS.task_name,TS.task_master_id, TFD.transport_name, TFD.transport_details_id, TFD.remote_file_name, TFD.remote_file_location,TFD.local_file_name
            FROM IVPRefMasterVendor.dbo.ivp_refm_task_summary TS INNER JOIN 
            IVPRefMasterVendor.dbo.ivp_refm_transport_task_details TFD ON (TFD.transport_master_id= TS.task_master_id AND TS.is_active = 1 AND TFD.is_active = 1)
            INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_task_type TT ON (TT.task_type_id = TS.task_type_id AND TS.is_active = 1)
             WHERE TT.task_type_id = 1", ConnectionConstants.RefMasterVendor_Connection);

            foreach (var row in ds.Tables[0].AsEnumerable())
            {
                if (taskNameVsID != null && !taskNameVsID.ContainsKey(row.Field<string>("task_name")))
                    taskNameVsID.Add(row.Field<string>("task_name"), row.Field<int>("task_master_id"));
                //if (transportNameVSId != null)
                //    transportNameVSId.Add(row.Field<string>("transport_name"), row.Field<int>("transport_details_id"));
            }
            return ds;
        }
        public static DataSet getTaskCustomClasses(int taskId, RDBConnectionManager conn = null)
        {
            string q = @"SELECT TS.task_name AS [Task Name], TFD.transport_name AS [Transport Type],
TFD.remote_file_name AS [Remote File Name],
 TFD.remote_file_location AS [Remote File Location],
TFD.local_file_name AS [Local File Name],
TFD.transport_details_id, CC.custom_class_id
FROM IVPRefMasterVendor.dbo.ivp_refm_task_summary TS INNER JOIN 
IVPRefMasterVendor.dbo.ivp_refm_transport_task_details TFD ON (TFD.transport_master_id = TS.task_master_id AND TS.is_active = 1 AND TS.is_active = 1)
INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_custom_class CC ON (CC.task_master_id = TFD.transport_master_id AND cc.is_active = 1 AND CC.task_details_id = TFD.transport_details_id)
                        WHERE TS.is_active = 1 AND TS.task_master_id = " + taskId;
            DataSet ds = null;
            if (conn == null)
                ds = CommonDALWrapper.ExecuteSelectQuery(q, ConnectionConstants.RefMasterVendor_Connection);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(q, conn);
            return ds;
        }
        public static bool LstTransportTaskPkContainsObj(List<TransportTaskPk> lst, TransportTaskPk obj, ref int index)
        {
            bool Contains = false;
            foreach (var x in lst)
            {
                index++;
                if (x.taskName.ToLower() == obj.taskName.ToLower() && x.transportType.ToUpper() == obj.transportType.ToUpper() && x.RemoteFile.ToLower() == obj.RemoteFile.ToLower()
                && x.RemoteFileLoc.ToLower() == obj.RemoteFileLoc.ToLower() && x.LocalFile.ToLower() == obj.LocalFile.ToLower())
                {
                    Contains = true;
                    break;
                }

            }
            return Contains;
        }
        public static int ReturnTransportId(Dictionary<TransportTaskPk, int> transportPkVsId, TransportTaskPk obj)
        {
            int rtrnTransportId = -1;
            foreach (var pair in transportPkVsId)
            {
                var x = pair.Key;
                if (x.taskName.ToLower() == obj.taskName.ToLower() && x.transportType.ToUpper() == obj.transportType.ToUpper() && x.RemoteFile.ToLower() == obj.RemoteFile.ToLower()
                 && x.RemoteFileLoc.ToLower() == obj.RemoteFileLoc.ToLower() && x.LocalFile.ToLower() == obj.LocalFile.ToLower())
                {
                    rtrnTransportId = pair.Value;
                    break;
                }
            }
            return rtrnTransportId;
        }
    }
}
