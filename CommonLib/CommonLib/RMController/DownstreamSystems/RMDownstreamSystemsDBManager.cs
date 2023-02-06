using com.ivp.commom.commondal;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.DownstreamSystems
{
   public class RMDownstreamSystemsDBManager
    {
        public static Dictionary<string, int> GetSytemNamesVsIdFromDb(string username)
        {
            DataSet ds = null;

            string query = @"SELECT report_system_name AS [Text], report_system_id AS [Value] from IVPRefMaster.dbo.ivp_refm_report_system_configuration where is_active = 1";


            ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            Dictionary<string, int> systemNamevsID = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    systemNamevsID.Add(Convert.ToString(row["Text"]), Convert.ToInt32(row["Value"]));
                });
            }

            return systemNamevsID;
        }
        public static Dictionary<string, Dictionary<int, int>> GetSystemReportMapping(Dictionary<string, Dictionary<string, int>> systemVsEntityTypeVsReportmapid)
        {
            DataSet ds = null;
            string query = @"select DSYS.report_system_name,REP.report_id, RMAP.report_system_map_id,ET.entity_display_name
                    from
                    IVPRefMaster.dbo.ivp_refm_report_system_configuration DSYS 
                    INNER JOIN IVPRefMaster.dbo.ivp_refm_report_system_report_map RMAP ON ( DSYS.report_system_id = RMAP.report_system_id AND DSYS.is_active = 1 AND RMAP.is_active = 1)
                    INNER JOIN IVPRefMaster.dbo.ivp_refm_report REP ON (REP.report_id = RMAP.report_id AND REP.is_active = 1)
                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map AS REM ON (REM.report_id = REP.report_id AND REM.is_active = 1)
                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type AS ET ON (ET.entity_type_id = REM.dependent_id AND ET.is_active = 1)
                    order by DSYS.report_system_name";

            ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            Dictionary<string, Dictionary<int, int>> systemNamevsreportmapID = new Dictionary<string, Dictionary<int, int>>(StringComparer.OrdinalIgnoreCase);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    string sys = Convert.ToString(row["report_system_name"]);
                    if (!systemNamevsreportmapID.ContainsKey(sys))
                        systemNamevsreportmapID.Add(Convert.ToString(row["report_system_name"]), new Dictionary<int, int>() { });
                    if(!systemNamevsreportmapID[sys].ContainsKey(Convert.ToInt32(row["report_id"]))) //if a report is mapped with 2 ET's
                        systemNamevsreportmapID[sys].Add(Convert.ToInt32(row["report_id"]), Convert.ToInt32(row["report_system_map_id"]));

                    if (!systemVsEntityTypeVsReportmapid.ContainsKey(sys))
                        systemVsEntityTypeVsReportmapid.Add(Convert.ToString(row["report_system_name"]), new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { });

                    if (!systemVsEntityTypeVsReportmapid[sys].ContainsKey(Convert.ToString(row["entity_display_name"])))
                    systemVsEntityTypeVsReportmapid[sys].Add(Convert.ToString(row["entity_display_name"]), Convert.ToInt32(row["report_system_map_id"]));

                });
            }

            return systemNamevsreportmapID;
        }

        public static void GetReportsDataFromDB(Dictionary<string, int> reportnameVsId, Dictionary<string, List<string>> reportVsEntity)
        {
            string q = @"select REP.report_id,REP.report_name,ET.entity_display_name,ET.entity_type_id from IVPRefMaster.dbo.ivp_refm_report REP inner join IVPRefMaster.dbo.ivp_refm_report_type rt on(REP.report_type_id = rt.report_type_id AND REP.is_active = 1 AND rt.is_active = 1)
                            INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map AS REM ON(REM.report_id = REP.report_id AND REM.is_active = 1)
                            INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type AS ET ON(ET.entity_type_id = REM.dependent_id AND ET.is_active = 1) where rt.report_type_id = 1";

            var ds = CommonDALWrapper.ExecuteSelectQuery(q, ConnectionConstants.RefMaster_Connection);
            foreach (var row in ds.Tables[0].AsEnumerable())
            {
                string repName = row.Field<string>("report_name");
                if (!reportnameVsId.ContainsKey(repName))
                 reportnameVsId.Add(repName, row.Field<int>("report_id"));
                if(!reportVsEntity.ContainsKey(repName))
                    reportVsEntity.Add(repName, new List<string>() { });

                reportVsEntity[repName].Add(row.Field<string>("entity_display_name"));
            }
        }
    }
}
