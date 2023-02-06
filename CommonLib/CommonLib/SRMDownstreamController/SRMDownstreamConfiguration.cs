using com.ivp.commom.commondal;
using com.ivp.common.srmdownstreamcontroller;
using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.transport;
using com.ivp.rad.utils;
using com.ivp.rad.viewmanagement;
using com.ivp.srmcommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace com.ivp.common.srmdwhjob
{
    public class SRM_DownstreamSync_Separators
    {
        public const string Remarks_Separator = "\n";
    }
    public class SRM_DownstreamSync_MigrationStatus
    {
        public const string Passed = "Success";
        public const string Failed = "Failure";
        public const string Not_Processed = "Not Processed";
        public const string Already_In_Sync = "Already in Sync";
    }
    public class SRM_DownstreamSync_SheetNames
    {
        public const string Downstream_Sync_Setup = "Downstream Sync Setup";
        public const string Downstream_Sync_Configuration = "Downstream Sync Configuration";
        public const string Downstream_Sync_Scheduler = "Downstream Sync Scheduler";
    }
    public class SRM_DownstreamSync_ColumnNames
    {
        public const string Setup_Name = "Setup Name";
        public const string Connection_Name = "Connection Name";
        public const string Calendar = "Calendar";
        public const string Effective_From_Date = "Effective From Date";
        public const string Report_Name = "Report Name";
        public const string Block_Type_Name = "Block Type";
        public const string Module = "Module";
        public const string Start_Date = "Start Date";
        //public const string Custom_Start_Date = "Custom Start Date";
        public const string End_Date = "End Date";
        //public const string Custom_End_Date = "Custom End Date";
        public const string Table_Name = "Table Name";
        public const string Batch_Size = "Batch Size";
        public const string Require_Knowledge_Date_Reporting = "Require Knowledge Date Reporting";
        public const string Require_Time_in_TS_Report = "Require IntraDay Changes";
        public const string Require_Deleted_Asset_Types = "Require Deleted Entities";
        public const string Require_Lookup_Massaging_Start_Date = "Attribute value massaging based on Start Date";
        public const string Require_Lookup_Massaging_Current_Knowledge_Date = "Attribute value massaging based on Knowledge Date";
        public const string CC_Assembly_Name = "Custom Class Assembly Name";
        public const string CC_Class_Name = "Custom Class Name";
        public const string CC_Method_Name = "Custom Class Method Name";
        public const string Queue_Name = "Queue Name";
        public const string Failure_Email_Id = "Failure Email Id";

        public const string Recurrence_Type = "Recurrence Type";
        public const string Recurrence_Pattern = "Recurrence Pattern";
        public const string Interval = "Interval";
        public const string Number_of_Recurrences = "Number of Recurrences";
        public const string Start_Time = "Start Time";
        public const string Time_Interval_of_Recurrence = "Time Interval of Recurrence";
        public const string Never_End_Job = "Never End Job";
        public const string Days_Of_Week = "Days Of Week";
    }

    public class SRM_DownstreamSync_SpecialColumnNames
    {
        public const string Remarks = "Remarks";
        public const string Sync_Status = "Status";
    }
    public enum StartDateTypesEnum
    {
        None,
        //Today = 1,
        //Yesterday = 2,
        //LastBusinessDay = 3,
        //T_Minus_N = 4,
        //CustomDate = 5,
        //Now = 6,
        //FirstBusinessDayOfMonth = 7,
        //FirstBusinessDayOfYear = 8,
        //LastBusinessDayOfMonth = 9,
        //LastBusinessDayOfYear = 10,
        //LastBusinessDayOfPreviousMonth_Plus_N = 11,
        //LastBusinessDayOfPreviousYear_Plus_N = 12,
        //FirstBusinessDayOfMonth_Plus_N = 13,
        //FirstBusinessDayOfYear_Plus_N = 14,
        LastExtractionDate = 100
    }
    public enum EndDateTypesEnum
    {
        //None,
        //Today = 1,
        //Yesterday = 2,
        //LastBusinessDay = 3,
        //T_Minus_N = 4,
        //CustomDate = 5,
        Now = 6,
        //FirstBusinessDayOfMonth = 7,
        //FirstBusinessDayOfYear = 8,
        //LastBusinessDayOfMonth = 9,
        //LastBusinessDayOfYear = 10,
        //LastBusinessDayOfPreviousMonth_Plus_N = 11,
        //LastBusinessDayOfPreviousYear_Plus_N = 12,
        //FirstBusinessDayOfMonth_Plus_N = 13,
        //FirstBusinessDayOfYear_Plus_N = 14,
        //LastExtractionDate = 100
    }
    public class DateTypeInfo
    {
        public string Value;
        public string CustomValue;
        public int Id;
        public string DateFormat;
    }
    [DataContract]
    public class SRMDownstreamSyncInfo
    {
        public SRMDownstreamSyncInfo()
        {
            this.SetupDetails = new SRMDownstreamSyncSetupDetails();
            this.BlockType = new List<SRMDownstreamSyncBlockType>();
        }
        [DataMember]
        public string SystemName { get; set; }

        [DataMember]
        public SRMDownstreamSyncSetupDetails SetupDetails { get; set; }

        [DataMember]
        public List<SRMDownstreamSyncBlockType> BlockType { get; set; }
    }
    [DataContract]
    public class SRMDownstreamSyncSetupDetails
    {
        [DataMember]
        public string ServerName { get; set; }

        [DataMember]
        public string DbName { get; set; }

        [DataMember]
        public string RealDbName { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string CalendarName { get; set; }
        [DataMember]
        public string EffectiveDate { get; set; }
    }
    [DataContract]
    public class SRMDownstreamSyncBlockType
    {
        public SRMDownstreamSyncBlockType()
        {
            this.ReportsAvailable = new List<SRMDownstreamSyncReportsAvailable>();
        }

        [DataMember]
        public string BlockTypeName { get; set; }
        [DataMember]
        public int BlockTypeId { get; set; }
        [DataMember]
        public bool IsBlockTypeConfigured { get; set; }

        [DataMember]
        public List<SRMDownstreamSyncReportsAvailable> ReportsAvailable { get; set; }

    }
    [DataContract]
    public class SRMDownstreamSyncReportsAvailable
    {
        public SRMDownstreamSyncReportsAvailable()
        {
            this.Details = new SRMDownstreamSyncReportDetails();
        }
        [DataMember]
        public string Module { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string text { get; set; }

        [DataMember]
        public int value { get; set; }

        [DataMember]
        public bool IsReportConfigured { get; set; }

        [DataMember]
        public SRMDownstreamSyncReportDetails Details { get; set; }
    }
    [DataContract]
    public class SRMDownstreamSyncReportDetails
    {
        [DataMember]
        public int? BatchSize { get; set; }

        [DataMember]
        public string CCAssemblyName { get; set; }

        [DataMember]
        public string CCClassName { get; set; }

        [DataMember]
        public string CCMethodName { get; set; }
        [DataMember]
        public string CustomStartDate { get; set; }

        [DataMember]
        public string CustomEndDate { get; set; }

        [DataMember]
        public string EndDateValue { get; set; }

        [DataMember]
        public string FailureEmail { get; set; }

        [DataMember]
        public string QueueName { get; set; }

        [DataMember]
        public bool RequireKnowledgeDateReporting { get; set; }

        [DataMember]
        public bool? RequireTimeInTSReport { get; set; }

        [DataMember]
        public bool RequireDeletedAssetTypes { get; set; }

        [DataMember]
        public bool RequireLookupMassagingStartDate { get; set; }

        [DataMember]
        public bool RequireLookupMassagingCurrentKnowledgeDate { get; set; }

        [DataMember]
        public string StartDateValue { get; set; }

        [DataMember]
        public string TableName { get; set; }
        public SRMDownstreamSyncReportDetails()
        {
            RequireKnowledgeDateReporting = false;
            RequireTimeInTSReport = false;
            RequireDeletedAssetTypes = false;
            RequireLookupMassagingCurrentKnowledgeDate = false;
            RequireLookupMassagingStartDate = false;
        }

    }

    public class SRMDownstreamConfiguration
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMDownstreamConfiguration");
        public string GetBlockTypes()
        {
            RHashlist mHList = new RHashlist();
            try
            {
                DataSet blockTypeDS = CommonDALWrapper.ExecuteSelectQuery("SELECT block_type_name FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_type where block_type_name Not In('Yearly','Quarterly','Monthly')", ConnectionConstants.RefMaster_Connection);
                string s = string.Join(",", blockTypeDS.Tables[0].Rows.OfType<DataRow>().Select(r => r[0].ToString()));
                return s;
            }
            catch (Exception Ex)
            {
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
            }
        }


        public List<SRMDownstreamSyncInfo> GetAllConfigDataInitial(string selectedSystemName)
        {
            List<SRMDownstreamSyncInfo> result = new List<SRMDownstreamSyncInfo>();
            try
            {
                string query = "select * from  IVPRefMaster.dbo.ivp_srm_dwh_downstream_master WHERE is_active = 1";
                DataSet setupDS = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                if (setupDS != null && setupDS.Tables.Count > 0 && setupDS.Tables[0] != null && setupDS.Tables[0].Rows.Count > 0)
                {
                    List<int> setupIds = new List<int>();

                    if (String.IsNullOrEmpty(selectedSystemName)) { selectedSystemName = Convert.ToString(setupDS.Tables[0].Rows[0]["setup_name"]); }
                    string currentCalendarName = null;
                    DateTime currentEffectiveDate = new DateTime();
                    var calendars = GetAllCalendars();
                    foreach (DataRow row in setupDS.Tables[0].Rows)
                    {
                        if (row["setup_name"].ToString().Equals(selectedSystemName))
                        {
                            var calendarId = Convert.ToInt32(row["calendar_id"].ToString());
                            currentCalendarName = calendars.ContainsKey(calendarId) ? calendars[calendarId] : "";
                            currentEffectiveDate = Convert.ToDateTime(row["effective_from_date"]);
                            break;
                        }
                    }

                    var systemDict = GetSystemNameVsConnectionDetailsDict(setupDS, true, selectedSystemName, setupIds);
                    DataSet blockTypeDS = CommonDALWrapper.ExecuteSelectQuery("SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_type where block_type_name Not In('Yearly','Quarterly','Monthly')", ConnectionConstants.RefMaster_Connection);


                    Dictionary<int, string> blockTypeIdVsName = new Dictionary<int, string>();
                    var reportsAvailableDict = SetReportsAvailableDict(blockTypeDS, blockTypeIdVsName);

                    var subString1 = "";
                    var substring2 = "";
                    if (CheckDatabaseExists("IVPSecMaster"))
                    {
                        subString1 = " CASE WHEN is_ref = 1 THEN rep.report_name ELSE srep.report_name END AS report_name";
                        substring2 = " LEFT JOIN IVPSecMaster.dbo.ivp_secm_report_setup srep ON(srep.report_setup_id = det.report_id AND det.is_Ref = 0 AND srep.is_active = 1) ";
                    }
                    else
                    {
                        subString1 = " CASE WHEN is_ref = 1 THEN rep.report_name END AS report_name";
                        substring2 = "";
                    }
                    if (blockTypeDS != null && blockTypeDS.Tables.Count > 0 && blockTypeDS.Tables[0] != null && blockTypeDS.Tables[0].Rows.Count > 0)
                    {
                        string q = string.Format(@"SELECT mas.setup_id ,mas.setup_name, mas.connection_name,mas.calendar_id,mas.effective_from_date, det.block_id, 
                        det.block_type_id, bl.block_type_name, det.report_id, 
                        {0}, 
                        det.is_ref , det.start_date , 
                        det.end_date ,det.table_name , det.batch_size ,det.require_time_in_ts_report , det.require_knowledge_date_reporting,
                        det.require_deleted_asset_types, det.require_lookup_massaging_start_date ,det.require_lookup_massaging_current_knowledge_date, det.cc_assembly_name , 
                        det.cc_class_name , det.cc_method_name ,det.queue_name , det.failure_email_id 
                        FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master mas 
                        INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det ON(mas.setup_id = det.setup_id) 
                        INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_type bl ON(bl.block_type_id = det.block_type_id)
                        LEFT JOIN IVPRefMaster.dbo.ivp_refm_report rep ON(rep.report_id = det.report_id AND det.is_Ref = 1 AND rep.is_active = 1 ) 
                        {1}
                        WHERE mas.setup_name = '{2}' AND mas.is_active = 1 AND det.is_active = 1 ORDER BY setup_id,block_type_id ", subString1, substring2, selectedSystemName);
                        ObjectTable reportsConfiguredDT = CommonDALWrapper.ExecuteSelectQueryObject(q, ConnectionConstants.RefMaster_Connection).Tables[0];

                        if (reportsConfiguredDT != null && reportsConfiguredDT.Rows.Count > 0)
                        {
                            SRMDownstreamSyncSetupDetails setupDetailsData = new SRMDownstreamSyncSetupDetails();
                            List<SRMDownstreamSyncBlockType> blockTypeList = new List<SRMDownstreamSyncBlockType>();

                            for (int j = 0; j < reportsConfiguredDT.Rows.Count; j++)
                            {
                                ObjectRow dataRow = reportsConfiguredDT.Rows[j];
                                var currentSetupName = Convert.ToString(dataRow["setup_name"]);
                                var currentBlockTypeId = Convert.ToInt32(dataRow["block_type_id"]);
                                var currentModule = Convert.ToBoolean(dataRow["is_ref"]);
                                var currentReportId = Convert.ToInt32(dataRow["report_id"]);
                                var currentReportName = Convert.ToString(dataRow["report_name"]);
                                if (selectedSystemName.Equals(currentSetupName, StringComparison.OrdinalIgnoreCase))
                                {
                                    bool containsSystem = result.Any(item => item.SystemName.Equals(selectedSystemName, StringComparison.OrdinalIgnoreCase));
                                    if (!containsSystem)
                                    {
                                        if (systemDict.ContainsKey(currentSetupName))
                                        {
                                            SRMDownstreamSyncInfo setup = new SRMDownstreamSyncInfo();
                                            var setupDetails = systemDict[currentSetupName];
                                            setup.SystemName = currentSetupName;
                                            setupDetailsData.DbName = setupDetails["id"];
                                            setupDetailsData.RealDbName = setupDetails["Initial Catalog"];
                                            setupDetailsData.ServerName = setupDetails["Data Source"];
                                            setupDetailsData.UserName = String.Empty;
                                            setupDetailsData.CalendarName = currentCalendarName;
                                            setupDetailsData.Password = String.Empty;
                                            setupDetailsData.EffectiveDate = currentEffectiveDate.ToString("MM/dd/yyyy");
                                            setup.SetupDetails = setupDetailsData;
                                            result.Add(setup);
                                        }
                                    }
                                    var currentBlockTypeName = Convert.ToString(dataRow["block_type_name"]);
                                    var existingBlockType = blockTypeList.Where(p => p.BlockTypeId == currentBlockTypeId).ToList();

                                    SRMDownstreamSyncReportsAvailable reportsAvailable = new SRMDownstreamSyncReportsAvailable();
                                    SRMDownstreamSyncReportDetails configuredDetails = new SRMDownstreamSyncReportDetails();

                                    reportsAvailable.Module = Convert.ToBoolean(dataRow["is_Ref"]) ? "Ref" : "Sec";
                                    reportsAvailable.Id = currentReportId;
                                    reportsAvailable.Name = Convert.ToString(dataRow["report_name"]);
                                    reportsAvailable.text = Convert.ToString(dataRow["report_name"]);
                                    reportsAvailable.value = currentReportId;
                                    reportsAvailable.IsReportConfigured = Convert.ToBoolean(1);
                                    configuredDetails = setConfiguredProperties(reportsConfiguredDT, j, true);
                                    reportsAvailable.Details = configuredDetails;

                                    if (existingBlockType.Count == 0)
                                    {
                                        SRMDownstreamSyncBlockType blockType = new SRMDownstreamSyncBlockType();
                                        List<SRMDownstreamSyncReportsAvailable> reportsAvailableList = new List<SRMDownstreamSyncReportsAvailable>();
                                        blockType.BlockTypeName = currentBlockTypeName;
                                        blockType.BlockTypeId = currentBlockTypeId;

                                        if (reportsAvailable.IsReportConfigured)
                                        {
                                            blockType.IsBlockTypeConfigured = true;
                                        }

                                        reportsAvailableList.Add(reportsAvailable);
                                        blockType.ReportsAvailable = reportsAvailableList;

                                        blockTypeList.Add(blockType);
                                        result[0].BlockType = blockTypeList;
                                    }
                                    else
                                    {
                                        var existingBlockTypeData = existingBlockType[0];
                                        if (reportsAvailable.IsReportConfigured)
                                        {
                                            existingBlockTypeData.IsBlockTypeConfigured = true;
                                        }

                                        existingBlockTypeData.ReportsAvailable.Add(reportsAvailable);
                                    }
                                    if (reportsAvailableDict.ContainsKey(currentBlockTypeId))
                                    {
                                        if (reportsAvailableDict[currentBlockTypeId].ContainsKey(currentModule))
                                        {
                                            if (reportsAvailableDict[currentBlockTypeId][currentModule].ContainsKey(currentReportName))
                                            {
                                                reportsAvailableDict[currentBlockTypeId][currentModule].Remove(currentReportName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //no reports configured
                            if (systemDict.ContainsKey(selectedSystemName))
                            {
                                SRMDownstreamSyncInfo setup = new SRMDownstreamSyncInfo();
                                SRMDownstreamSyncSetupDetails setupDetailsData = new SRMDownstreamSyncSetupDetails();
                                var setupDetails = systemDict[selectedSystemName];
                                setup.SystemName = selectedSystemName;
                                setupDetailsData.DbName = setupDetails["id"];
                                setupDetailsData.RealDbName = setupDetails["Initial Catalog"];
                                setupDetailsData.ServerName = setupDetails["Data Source"];
                                setupDetailsData.UserName = String.Empty;
                                setupDetailsData.Password = String.Empty;
                                setupDetailsData.CalendarName = currentCalendarName;
                                setupDetailsData.EffectiveDate = currentEffectiveDate.ToString("MM/dd/yyyy");
                                setup.SetupDetails = setupDetailsData;
                                result.Add(setup);
                            }
                        }
                        if (reportsAvailableDict.Count > 0)
                            foreach (var modict in reportsAvailableDict)
                            {
                                var currentBlockTypeId = modict.Key;
                                var existingBlockType = new List<SRMDownstreamSyncBlockType>();
                                var blockTypeExists = result[0].BlockType.Any(p => p.BlockTypeId == currentBlockTypeId);
                                if (blockTypeExists)
                                {
                                    existingBlockType = result[0].BlockType.Where(p => p.BlockTypeId == currentBlockTypeId).ToList();
                                }
                                if (existingBlockType.Count > 0)
                                {
                                    foreach (var dict in modict.Value)
                                    {
                                        foreach (var rdict in dict.Value)
                                        {
                                            SRMDownstreamSyncReportsAvailable reportsAvailable = new SRMDownstreamSyncReportsAvailable();
                                            reportsAvailable.Module = Convert.ToBoolean(dict.Key) ? "Ref" : "Sec";
                                            reportsAvailable.Id = rdict.Value;
                                            reportsAvailable.Name = rdict.Key;
                                            reportsAvailable.text = rdict.Key; ;
                                            reportsAvailable.value = rdict.Value;
                                            reportsAvailable.IsReportConfigured = Convert.ToBoolean(0);
                                            existingBlockType[0].ReportsAvailable.Add(reportsAvailable);
                                        }
                                    }
                                }
                                else
                                {
                                    SRMDownstreamSyncBlockType blockType = new SRMDownstreamSyncBlockType();
                                    var currentBlockTypeName = blockTypeIdVsName[currentBlockTypeId];
                                    blockType.BlockTypeName = currentBlockTypeName;
                                    blockType.BlockTypeId = currentBlockTypeId;
                                    foreach (var dict in modict.Value)
                                    {
                                        foreach (var rdict in dict.Value)
                                        {
                                            SRMDownstreamSyncReportsAvailable reportsAvailable = new SRMDownstreamSyncReportsAvailable();
                                            reportsAvailable.Module = Convert.ToBoolean(dict.Key) ? "Ref" : "Sec";
                                            reportsAvailable.Id = rdict.Value;
                                            reportsAvailable.Name = rdict.Key;
                                            reportsAvailable.text = rdict.Key;
                                            reportsAvailable.value = rdict.Value;
                                            reportsAvailable.IsReportConfigured = Convert.ToBoolean(0);
                                            blockType.ReportsAvailable.Add(reportsAvailable);
                                        }
                                    }
                                    result[0].BlockType.Add(blockType);
                                }
                            }

                        if (systemDict.Count > 0)
                            foreach (var system in systemDict)
                            {
                                if (system.Key != selectedSystemName)
                                {
                                    SRMDownstreamSyncInfo SetupNameAndDbOnly = new SRMDownstreamSyncInfo();
                                    SetupNameAndDbOnly.SystemName = Convert.ToString(system.Key);
                                    SetupNameAndDbOnly.SetupDetails.DbName = Convert.ToString(system.Value["id"]);
                                    result.Add(SetupNameAndDbOnly);
                                }
                            }
                    }
                    else
                    {
                        //no Setup exists
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetAllConfigDataInitial -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetAllConfigDataInitial -> End");
            }
            return result;
        }
        public List<string> SRMDownstreamSyncGetExistingConnections()
        {
            List<string> connectionIds = new List<string>();
            var dalConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RConfigReader.GetConfigAppSettings("DalConfigurationPath"));
            XmlDocument doc = new XmlDocument();
            doc.Load(dalConfigPath);

            XmlNodeList connectionNodes = doc.GetElementsByTagName("connection");
            //selected setup name with details
            foreach (XmlNode connectionNode in connectionNodes)
            {
                connectionIds.Add(connectionNode.SelectSingleNode("id").InnerText.ToString());
            }

            return connectionIds;
        }
        public Dictionary<string, Dictionary<string, string>> GetSystemNameVsConnectionDetailsDict(DataSet setupDS, bool fromUI, string selectedSystemName, List<int> SetupIds)
        {
            Dictionary<string, Dictionary<string, string>> systemDict = new Dictionary<string, Dictionary<string, string>>();

            for (int i = 0; i < setupDS.Tables[0].Rows.Count; i++)
            {
                // ConfigManager case
                if (SetupIds.Any() && SetupIds.Count > 0)
                {
                    foreach (int item in SetupIds)
                    {
                        if (Convert.ToInt32(setupDS.Tables[0].Rows[i]["setup_id"]) == item)
                        {
                            systemDict = GetConnectionDetails(setupDS.Tables[0].Rows[i], systemDict);
                        }
                    }
                }
                else if (SetupIds.Any() == false && fromUI == false)
                {
                    systemDict = GetConnectionDetails(setupDS.Tables[0].Rows[i], systemDict);
                }
                //UI case
                else
                {
                    if (Convert.ToString(setupDS.Tables[0].Rows[i]["setup_name"]).Equals(selectedSystemName, StringComparison.OrdinalIgnoreCase))
                    {
                        systemDict = GetConnectionDetails(setupDS.Tables[0].Rows[i], systemDict);
                    }
                    else
                    {
                        //to add available setup names
                        var connectionDictNameOnly = new Dictionary<string, string>();
                        connectionDictNameOnly.Add("id", Convert.ToString(setupDS.Tables[0].Rows[i]["connection_name"]));
                        systemDict.Add(Convert.ToString(setupDS.Tables[0].Rows[i]["setup_name"]), connectionDictNameOnly);
                    }
                }
            }
            return systemDict;
        }
        public Dictionary<string, Dictionary<string, string>> GetConnectionDetails(DataRow setupDR, Dictionary<string, Dictionary<string, string>> systemDict)
        {
            XmlNodeList connectionNodes = GetDalConfig().GetElementsByTagName("connection");
            //selected setup name with details
            foreach (XmlNode connectionNode in connectionNodes)
            {
                string a = connectionNode.SelectSingleNode("id").InnerText.ToString().Trim();
                if (a.Equals(Convert.ToString(setupDR["connection_name"]), StringComparison.OrdinalIgnoreCase))
                {
                    var connString = connectionNode.SelectSingleNode("connstring").InnerText.ToString();
                    var connectionDict = connString.Split(';').Where(x => x.Contains('=')).Select(x => x.Split('='))
                        .Where(x => x.Length > 1 && !String.IsNullOrEmpty(x[0].Trim()) && !String.IsNullOrEmpty(x[1].Trim()))
                        .ToDictionary(x => x[0].Trim(), x => x[1].Trim());
                    connectionDict.Add("id", a);
                    systemDict.Add(Convert.ToString(setupDR["setup_name"]), connectionDict);
                }
            }
            return systemDict;
        }
        public XmlDocument GetDalConfig()
        {
            var dalConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RConfigReader.GetConfigAppSettings("DalConfigurationPath"));
            XmlDocument doc = new XmlDocument();
            doc.Load(dalConfigPath);
            return doc;
        }
        public SRMDownstreamSyncSetupDetails GetSelectedConnectionDetails(string connectionName)
        {
            XmlNodeList connectionNodes = GetDalConfig().GetElementsByTagName("connection");
            Dictionary<string, string> connectionDict = new Dictionary<string, string>();
            //selected setup name with details
            foreach (XmlNode connectionNode in connectionNodes)
            {
                string a = connectionNode.SelectSingleNode("id").InnerText.ToString().Trim();
                if (a.Equals(connectionName, StringComparison.OrdinalIgnoreCase))
                {
                    var connString = connectionNode.SelectSingleNode("connstring").InnerText.ToString();
                    connectionDict = connString.Split(';').Where(x => x.Contains('=')).Select(x => x.Split('='))
                       .Where(x => x.Length > 1 && !String.IsNullOrEmpty(x[0].Trim()) && !String.IsNullOrEmpty(x[1].Trim()))
                       .ToDictionary(x => x[0].Trim(), x => x[1].Trim());
                    connectionDict.Add("id", a);
                }
            }
            SRMDownstreamSyncSetupDetails clonedSystemConnectionDetails = new SRMDownstreamSyncSetupDetails();
            clonedSystemConnectionDetails.DbName = connectionDict["id"];
            clonedSystemConnectionDetails.RealDbName = connectionDict["Initial Catalog"];
            clonedSystemConnectionDetails.ServerName = connectionDict["Data Source"];
            clonedSystemConnectionDetails.UserName = connectionDict["User ID"];
            clonedSystemConnectionDetails.Password = connectionDict["password"];
            return clonedSystemConnectionDetails;
        }


        public Dictionary<int, Dictionary<bool, Dictionary<string, int>>> SetReportsAvailableDict(DataSet blockTypeDS, Dictionary<int, string> blockTypeIdVsName)
        {
            Dictionary<int, Dictionary<bool, Dictionary<string, int>>> reportsAvailableDict = new Dictionary<int, Dictionary<bool, Dictionary<string, int>>>();


            if (blockTypeDS != null && blockTypeDS.Tables.Count > 0 && blockTypeDS.Tables[0] != null && blockTypeDS.Tables[0].Rows.Count > 0)
            {
                Dictionary<bool, Dictionary<int, string>> moduleDict = new Dictionary<bool, Dictionary<int, string>>();

                for (int j = 0; j < blockTypeDS.Tables[0].Rows.Count; j++)
                {
                    var currentBlockTypeId = Convert.ToInt32(blockTypeDS.Tables[0].Rows[j]["block_type_id"]);
                    var currentBlockTypName = Convert.ToString(blockTypeDS.Tables[0].Rows[j]["block_type_name"]);
                    blockTypeIdVsName.Add(currentBlockTypeId, currentBlockTypName);

                    string query = "";
                    string querysec = "";
                    switch (currentBlockTypeId)
                    {
                        case 1:
                            query = @"SELECT rep.report_id,rep.report_name ,
                                    CASE WHEN rep.report_name LIKE '% leg %' THEN 1 ELSE 0 END AS 'Is Leg Report'
                                    FROM IVPRefMaster.dbo.ivp_refm_report rep
                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_report_configuration conf
                                    ON(rep.report_id = conf.report_id)
                                    WHERE rep.is_active = 1 AND conf.is_active = 1
                                    AND((rep.report_type_id = 1 AND rep.report_name like '%Non Time Series') OR (rep.report_type_id = 1 AND rep.report_name LIKE '% leg %'))";

                            querysec = @"SELECT report_name, report_setup_id as 'report_id'
                                    FROM IVPSecMaster.dbo.ivp_secm_report_setup
                                    WHERE is_active = 1
                                    AND ((report_id = 16 AND report_name like '%Non Time Series') OR (report_id = 16 AND report_name LIKE '% leg %'))";
                            break;
                        case 2:
                            query = @"SELECT rep.report_id,rep.report_name ,
                                    CASE WHEN rep.report_name LIKE '% leg %' THEN 1 ELSE 0 END AS 'Is Leg Report'
                                    FROM IVPRefMaster.dbo.ivp_refm_report rep
                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_report_configuration conf
                                    ON (rep.report_id = conf.report_id)
                                    WHERE rep.report_type_id = 6 AND rep.is_active = 1 AND conf.is_active = 1
                                    AND rep.report_name like '%Time Series' AND conf.Is_DWH_Extract = 1";
                            querysec = @"SELECT report_name, report_setup_id as 'report_id'
                                    FROM IVPSecMaster.dbo.ivp_secm_report_setup
                                    WHERE report_id = 11 AND is_active = 1
                                    AND report_name like '%Time Series'";
                            break;
                        case 6:
                            query = @"SELECT rep.report_id,rep.report_name ,
                                    CASE WHEN rep.report_name LIKE '% leg %' THEN 1 ELSE 0 END AS 'Is Leg Report'
                                    FROM IVPRefMaster.dbo.ivp_refm_report rep
                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_report_configuration conf
                                    ON (rep.report_id = conf.report_id)
                                    WHERE  rep.is_active = 1 AND conf.is_active = 1
                                    AND ((rep.report_type_id = 6 AND rep.report_name like '%Daily') OR (rep.report_type_id = 6 AND rep.report_name LIKE '% leg %')) AND conf.Is_DWH_Extract = 0";
                            querysec = @"SELECT report_name, report_setup_id as 'report_id'
                                    FROM IVPSecMaster.dbo.ivp_secm_report_setup
                                    WHERE is_active = 1
                                    AND ((report_id IN (11,16) AND report_name like '%Daily') OR (report_id IN (11,16) AND report_name LIKE '% leg %')) AND report_name NOT LIKE '%Time Series%'";
                            break;
                        default:
                            query = "SELECT report_id, report_name FROM IVPRefMaster.dbo.ivp_refm_report WHERE is_active = 1";
                            querysec = "SELECT report_id, report_name  FROM IVPSecMaster.dbo.ivp_secm_report_setup WHERE is_active = 1";
                            break;
                    }

                    DataTable reportsAvailableRefDT = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                    SetDictionary(reportsAvailableDict, reportsAvailableRefDT, currentBlockTypeId, true);


                    if (CheckDatabaseExists("IVPSecMaster"))
                    {
                        DataTable reportsAvailableSecDT = CommonDALWrapper.ExecuteSelectQuery(querysec, ConnectionConstants.RefMaster_Connection).Tables[0];
                        SetDictionary(reportsAvailableDict, reportsAvailableSecDT, currentBlockTypeId, false);
                    }
                }
            }
            return reportsAvailableDict;
        }

        public Dictionary<int, Dictionary<bool, Dictionary<string, int>>> SetDictionary(Dictionary<int, Dictionary<bool, Dictionary<string, int>>> reportsAvailableDict, DataTable reportsAvailableDT, int currentBlockTypeId, bool isRef)
        {
            if (reportsAvailableDT != null && reportsAvailableDT.Rows.Count > 0)
            {
                Dictionary<int, string> reportsDict = new Dictionary<int, string>();
                for (int k = 0; k < reportsAvailableDT.Rows.Count; k++)
                {
                    var currentReportName = Convert.ToString(reportsAvailableDT.Rows[k]["report_name"]);
                    var isRefReport = isRef ? Convert.ToBoolean(1) : Convert.ToBoolean(0);
                    if (!reportsAvailableDict.ContainsKey(currentBlockTypeId))
                        reportsAvailableDict[currentBlockTypeId] = new Dictionary<bool, Dictionary<string, int>>();

                    if (!reportsAvailableDict[currentBlockTypeId].ContainsKey(isRefReport))
                        reportsAvailableDict[currentBlockTypeId][isRefReport] = new Dictionary<string, int>();
                    if (!reportsAvailableDict[currentBlockTypeId][isRefReport].ContainsKey(currentReportName))
                        reportsAvailableDict[currentBlockTypeId][isRefReport].Add(currentReportName, Convert.ToInt32(reportsAvailableDT.Rows[k]["report_id"]));
                }
            }
            return reportsAvailableDict;
        }
        public SRMDownstreamSyncReportDetails setConfiguredProperties(ObjectTable reportsConfiguredDT, int j, bool forUI)
        {
            //var calendarIdVsName = GetAllCalendars();
            SRMDownstreamSyncReportDetails configuredDetails = new SRMDownstreamSyncReportDetails();
            ObjectRow dataRow = reportsConfiguredDT.Rows[j];

            if (forUI)
            {
                var startDate = dataRow["start_date"] != DBNull.Value ? Convert.ToInt32(dataRow["start_date"]) : 0;
                configuredDetails.StartDateValue = Convert.ToString((StartDateTypesEnum)startDate);
            }
            else
                configuredDetails.StartDateValue = dataRow["start_date"] != DBNull.Value ? Convert.ToString(dataRow["start_date"]) : "";

            if (forUI)
            {
                var endDate = dataRow["end_date"] != DBNull.Value ? Convert.ToInt32(dataRow["end_date"]) : 0;
                configuredDetails.EndDateValue = Convert.ToString((EndDateTypesEnum)endDate);
            }
            else
                configuredDetails.EndDateValue = dataRow["end_date"] != DBNull.Value ? Convert.ToString(dataRow["end_date"]) : "";
            //configuredDetails.CustomStartDate = dataRow["custom_start_date"] != DBNull.Value ? Convert.ToString(dataRow["custom_start_date"]) : "";
            //configuredDetails.CustomEndDate = dataRow["custom_end_date"] != DBNull.Value ? Convert.ToString(dataRow["custom_end_date"]) : "";
            configuredDetails.TableName = dataRow["table_name"] != DBNull.Value ? Convert.ToString(dataRow["table_name"]) : "";
            configuredDetails.BatchSize = dataRow["batch_size"] != DBNull.Value ? Convert.ToInt32(dataRow["batch_size"]) : 0;
            if (dataRow["require_time_in_ts_report"].Equals(""))
            {
                configuredDetails.RequireTimeInTSReport = false;
            }
            else
                //  configuredDetails.RequireTimeInTSReport = Convert.ToBoolean(dataRow["require_time_in_ts_report"]) ;
                configuredDetails.RequireTimeInTSReport = dataRow["require_time_in_ts_report"] != DBNull.Value ? Convert.ToBoolean(dataRow["require_time_in_ts_report"]) : false;

            configuredDetails.RequireKnowledgeDateReporting = dataRow["require_knowledge_date_reporting"] != DBNull.Value ? Convert.ToBoolean(dataRow["require_knowledge_date_reporting"]) : false;
            configuredDetails.RequireDeletedAssetTypes = dataRow["require_deleted_asset_types"] != DBNull.Value ? Convert.ToBoolean(dataRow["require_deleted_asset_types"]) : false;
            configuredDetails.RequireLookupMassagingStartDate = dataRow["require_lookup_massaging_start_date"] != DBNull.Value && dataRow["require_lookup_massaging_start_date"].ToString() != "" ? Convert.ToBoolean(dataRow["require_lookup_massaging_start_date"]) : false;
            configuredDetails.RequireLookupMassagingCurrentKnowledgeDate = dataRow["require_lookup_massaging_current_knowledge_date"] != DBNull.Value && dataRow["require_lookup_massaging_start_date"].ToString() != "" ? Convert.ToBoolean(dataRow["require_lookup_massaging_current_knowledge_date"]) : false;
            configuredDetails.CCAssemblyName = dataRow["cc_assembly_name"] != DBNull.Value ? Convert.ToString(dataRow["cc_assembly_name"]) : "";
            configuredDetails.CCClassName = dataRow["cc_class_name"] != DBNull.Value ? Convert.ToString(dataRow["cc_class_name"]) : "";
            configuredDetails.CCMethodName = dataRow["cc_method_name"] != DBNull.Value ? Convert.ToString(dataRow["cc_method_name"]) : "";
            configuredDetails.QueueName = dataRow["queue_name"] != DBNull.Value ? Convert.ToString(dataRow["queue_name"]) : "";
            configuredDetails.FailureEmail = dataRow["failure_email_id"] != DBNull.Value ? Convert.ToString(dataRow["failure_email_id"]) : "";
            return configuredDetails;

        }
        public void SRMDownstreamSyncAddNewSystem(SRMDownstreamSyncSetupDetails SetupDetails, string SystemName, string EffectiveDate, int CalendarType, out string errorMessage)
        {
            try
            {
                errorMessage = string.Empty;
                bool IsNewSystem = true;
                if (string.IsNullOrEmpty(SystemName) || string.IsNullOrEmpty(SetupDetails.DbName) || string.IsNullOrEmpty(EffectiveDate) || CalendarType == 0)
                {
                    errorMessage = "Provide all details required to add new setup";
                }
                else
                {
                    DataSet result = CommonDALWrapper.ExecuteSelectQuery(
                        string.Format(@"exec [IVPRefMaster].[dbo].[REFM_DownstreamSyncSaveReportConfiguration]  '{0}', '{1}', '{2}', '', '{3}', '{4}' ,'{5}'", IsNewSystem, SystemName, SetupDetails.DbName, new RCommon().SessionInfo.LoginName, CalendarType, EffectiveDate),
                      ConnectionConstants.RefMaster_Connection);
                    //DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"IF NOT EXISTS(SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master WHERE setup_name = '{0}' )Insert into IVPRefMaster.dbo.ivp_srm_dwh_downstream_master(setup_name,connection_name,is_active,created_by,created_on, last_modified_by, last_modified_on,SysStartTime,SysEndTime,calendar_id,effective_from_date) values( '{0}', '{1}', 1, '{2}', getdate(), '{3}', getdate(), default, default,{4},'{5}')",
                    //    SystemName, SetupDetails.DbName, new RCommon().SessionInfo.LoginName, new RCommon().SessionInfo.LoginName, CalendarType, DateTime.ParseExact(EffectiveDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture)), ConnectionConstants.RefMaster_Connection);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
            }
        }
        //private static bool IsConnectionValid(string connectionString)
        //{
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            return true;
        //        }
        //        catch (Exception Ex)
        //        {
        //            return false;
        //        }
        //    }
        //}
        //public string GetConnectionString(string connectionName)
        //{
        //    XmlNodeList connectionNodes = GetDalConfig().GetElementsByTagName("connection");
        //    string connString = "";
        //    foreach (XmlNode connectionNode in connectionNodes)
        //    {
        //        string a = connectionNode.SelectSingleNode("id").InnerText.ToString().Trim();
        //        if (a.Equals(connectionName, StringComparison.OrdinalIgnoreCase))
        //        {
        //            connString = connectionNode.SelectSingleNode("connstring").InnerText.ToString();
        //            break;
        //            //dataSource = (connString.Split(';').Select(x => x.Split('='))
        //            //   .Where(x => x.Length > 1 && !String.IsNullOrEmpty(x[0].Trim()) && !String.IsNullOrEmpty(x[1].Trim()))
        //            //   .ToDictionary(x => x[0].Trim(), x => x[1].Trim()))["Data Source"];
        //        }
        //    }
        //    return connString;
        //}
        public static bool CheckDatabaseExists(string databaseName)
        {
            string query = "SELECT TOP 1 1 FROM sys.databases WHERE name = '" + databaseName + "'";
            var ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
            //using (var connection = new SqlConnection(connectionString))
            //{
            //    using (var command = new SqlCommand($"SELECT db_id('{databaseName}')", connection))
            //    {
            //        connection.Open();
            //        return (command.ExecuteScalar() != DBNull.Value);
            //    }
            //}
        }

        public static string ValidateReportTableName(SRMDownstreamSyncReportsAvailable report)
        {

            string module;
            if (report.Module == "Ref")
            {
                module = "references";
            }
            else
            {
                module = "taskmanager";
            }
            var idealTableName = "[" + module + "].[ivp_polaris_" + report.Name.ToLower().Replace(' ', '_') + "_staging]";
            if (!string.IsNullOrEmpty(report.Details.TableName))
            {
                if (report.Details.StartDateValue.Trim().Equals(DWHDateType.None.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    report.Details.TableName = idealTableName;
                    return null;
                }
                if (report.Details.TableName.ToLower() != idealTableName)
                {
                    return "Report Name Changed: " + report.Name + ". Run full load to resolve it.";
                }
            }
            else
            {
                report.Details.TableName = idealTableName;
            }
            return null;
        }


        public void SRMDownstreamSyncSaveReports(SRMDownstreamSyncInfo Sys, bool IsNewSystem, out string errorMessage, ObjectTable schedulerInfo, string userName = "", string dateFormat = "")
        {
            if (string.IsNullOrEmpty(userName))
                userName = new RCommon().SessionInfo.LoginName;
            if (string.IsNullOrEmpty(dateFormat))
                dateFormat = new RCommon().SessionInfo.CultureInfo.LongDateFormat;
            List<String> Exceptions = new List<string>();
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(Sys.SystemName))
            {
                Exceptions.Add("- System Name -> System Name Required");
            }
            if (string.IsNullOrEmpty(Sys.SetupDetails.DbName))
            {
                Exceptions.Add("- Connection Name Required for");
            }
            else if (!SRMDownstreamSyncGetExistingConnections().Contains(Sys.SetupDetails.DbName, StringComparer.OrdinalIgnoreCase))
            {
                Exceptions.Add("- Inavlid Connection Name Provided");
            }
            if (string.IsNullOrEmpty(Sys.SetupDetails.CalendarName))
            {
                Sys.SetupDetails.CalendarName = "NYSE";
            }
            var calendarIdVsName = GetAllCalendars();
            int calendarId = calendarIdVsName.FirstOrDefault(x => x.Value.Equals(Sys.SetupDetails.CalendarName, StringComparison.OrdinalIgnoreCase)).Key;

            if (calendarId == 0)
            {
                Exceptions.Add("- Invalid Calendar Name Provided");
            }
            if (string.IsNullOrEmpty(Sys.SetupDetails.EffectiveDate))
            {
                Exceptions.Add("- Effective Date Required");
            }

            foreach (var blockType in Sys.BlockType)
            {
                foreach (var report in blockType.ReportsAvailable)
                {
                    try
                    {
                        if (blockType.BlockTypeId == 2 && blockType.BlockTypeName == "Time Series")
                        {
                            if (report.Details.RequireTimeInTSReport == null)
                            {
                                report.Details.RequireTimeInTSReport = true;
                            }
                        }
                        else
                        {
                            report.Details.RequireTimeInTSReport = null;
                        }
                        //Module Validations
                        if (String.IsNullOrEmpty(report.Module))
                        {
                            Exceptions.Add("- Invalid Module Provided for " + report.Name);
                            break;
                        }

                        //Table Name Validations
                        var message = ValidateReportTableName(report);
                        if (message != null)
                            Exceptions.Add(message);

                        //Batch Size Validations
                        if (report.Details.BatchSize == null)
                        {
                            report.Details.BatchSize = 1000;
                        }
                        if (report.Details.BatchSize < 0)
                        {
                            Exceptions.Add("- Invalid Batch Size for " + report.Name);
                        }


                        //Custom Class Validations
                        if (!(!string.IsNullOrEmpty(report.Details.CCAssemblyName) && !string.IsNullOrEmpty(report.Details.CCClassName) && !string.IsNullOrEmpty(report.Details.CCMethodName)))
                        {
                            if ((string.IsNullOrEmpty(report.Details.CCAssemblyName) && string.IsNullOrEmpty(report.Details.CCClassName) && string.IsNullOrEmpty(report.Details.CCMethodName))) { }
                            else
                            {
                                Exceptions.Add("- Insufficient Custom Class Details Provided for " + report.Name);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(report.Details.CCAssemblyName) && !string.IsNullOrEmpty(report.Details.CCClassName) && !string.IsNullOrEmpty(report.Details.CCMethodName))
                            {
                                try
                                {
                                    Assembly testAssembly = Assembly.LoadFile(report.Details.CCAssemblyName);

                                    var t = (from type in testAssembly.GetTypes() where type.Name == report.Details.CCAssemblyName && type.GetMethods().Any(m => m.Name == report.Details.CCMethodName) select type).FirstOrDefault();

                                    if (t == null)
                                    {
                                        Exceptions.Add("- Invalid Custom Class Details Provided for " + report.Name);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Exceptions.Add("- Invalid Custom Class Details Provided for " + report.Name);
                                }
                            }
                        }
                        //Failure Email Validations
                        if (!string.IsNullOrEmpty(report.Details.FailureEmail))
                        {
                            //if (report.Details.FailureEmail.Contains(","))
                            //{
                            string[] failureEmailList = report.Details.FailureEmail.Split(',');
                            foreach (var item in failureEmailList)
                            {
                                try
                                {
                                    var validEmail = new System.Net.Mail.MailAddress(item);
                                    if (!(validEmail.Address == item))
                                    {
                                        Exceptions.Add("- Invalid Failure Email Provided for " + report.Name);
                                        return;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Exceptions.Add("- Invalid Failure Email Provided for " + report.Name);
                                }
                            }
                            //}
                            //else
                            //{
                            //    try
                            //    {
                            //        var validEmail = new System.Net.Mail.MailAddress(report.Details.FailureEmail);
                            //        if (!(validEmail.Address == report.Details.FailureEmail))
                            //        {
                            //            Exceptions.Add("- Invalid Failure Email Provided for " + report.Name);
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Exceptions.Add("- Invalid Failure Email Provided for " + report.Name);
                            //    }

                            //}
                        }

                        //Queue Name Validations

                        DataSet allTransports = RTransportConfigLoader.GetAllTransports();
                        List<string> QueueNames = allTransports.Tables[0].AsEnumerable()
                                                              .Where(r => r.Field<string>("transport_type_name").Equals("MSMQ", StringComparison.OrdinalIgnoreCase) || r.Field<string>("transport_type_name").Equals("WMQ", StringComparison.OrdinalIgnoreCase) || r.Field<string>("transport_type_name").Equals("RabbitMQ", StringComparison.OrdinalIgnoreCase) || r.Field<string>("transport_type_name").Equals("KafkaMQ", StringComparison.OrdinalIgnoreCase))
                                                              .Select(row => row.Field<string>("transport_name")).ToList();

                        List<string> items = null;
                        if (!String.IsNullOrEmpty(report.Details.QueueName))
                        {
                            items = report.Details.QueueName.Split(',').ToList();

                            foreach (var item in items ?? Enumerable.Empty<string>())
                            {
                                if (!QueueNames.Contains(item, StringComparer.OrdinalIgnoreCase))
                                {
                                    Exceptions.Add("- Invalid Queue Name Provided for " + blockType.BlockTypeName + "_" + report.Module + "_" + report.Name);
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(report.Details.StartDateValue))
                        {
                            report.Details.StartDateValue = Enum.GetName(typeof(StartDateTypesEnum), 100);
                        }
                        if (report.Details.EndDateValue == null)
                        {
                            report.Details.EndDateValue = Enum.GetName(typeof(EndDateTypesEnum), 6);
                        }

                        string error = ValidateReportConfigurationDates(report.Details.StartDateValue, report.Details.EndDateValue, report.Details.CustomStartDate, report.Details.CustomEndDate, "yyyyMMdd", calendarId);

                        if (!string.IsNullOrEmpty(error))
                        {
                            Exceptions.Add("- " + error + " " + blockType.BlockTypeName + "_" + report.Module + "_" + report.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.ToString();
                    }
                }
            }
            if (Exceptions.Count == 0)
            {
                string stagingTableName = null;
                var con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
                try
                {
                    var table = CreateDataTable<SRMDownstreamSyncBlockType, SRMDownstreamSyncReportsAvailable>(Sys.BlockType, "ReportsAvailable", Sys.SystemName);
                    DateTime effDate;
                    if (!(DateTime.TryParseExact(Sys.SetupDetails.EffectiveDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out effDate)
                        || DateTime.TryParseExact(Sys.SetupDetails.EffectiveDate, dateFormat.Split(' ')[0], null, System.Globalization.DateTimeStyles.None, out effDate)
                        || DateTime.TryParseExact(Sys.SetupDetails.EffectiveDate.Split(' ')[0], dateFormat.Split(' ')[0], null, System.Globalization.DateTimeStyles.None, out effDate)))
                    {
                        throw new Exception("Error parsing effective start date: " + Sys.SystemName);
                    }

                    stagingTableName = BulkUploadConfiguredReports(table);
                    DataSet result = CommonDALWrapper.ExecuteSelectQuery(
                        string.Format(@"exec [IVPRefMaster].[dbo].[REFM_DownstreamSyncSaveReportConfiguration]  '{0}', '{1}', '{2}', '{3}', '{4}',{5},'{6}'", IsNewSystem,
                            Sys.SystemName, Sys.SetupDetails.DbName, stagingTableName, userName, calendarId, effDate),
                        con);
                    if (result.Tables[0].Rows.Count == 0)
                        throw new Exception("Downstream Sync Save Report returned no setup id");
                    int setup_id = Convert.ToInt32(result.Tables[0].Rows[0]["setup_id"].ToString());
                    if (schedulerInfo != null)
                        SRMDownstreamStatusAndScheduler.ValidateandSaveDWHScheduling(schedulerInfo, null, userName, con, false, dateFormat);
                    else
                    {
                        SRMDownstreamStatusAndScheduler.DeleteDWHSchedulingInfo(Sys.SystemName, con);
                    }

                    const string REFM_CONSTANT = "REFM";
                    const string SECM_CONSTANT = "SECM";

                    SRMDWHJob SRMDWHJob = new SRMDWHJob();
                    SRMDWHJob.CreateObjects(Sys.SetupDetails.DbName);
                    Dictionary<string, List<int>> moduleVsLegReports = new Dictionary<string, List<int>>();
                    GetLegReports(ref moduleVsLegReports);



                    string query = "TRUNCATE TABLE [dimension].[ivp_srm_dwh_tables_for_loading]";
                    DataSet ds = SRMDWHJobExtension.ExecuteQuery(Sys.SetupDetails.DbName, query, SRMDBQueryType.SELECT);
                    DataSet blockTypeDS = CommonDALWrapper.ExecuteSelectQuery("SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_type where block_type_name Not In('Yearly','Quarterly','Monthly')", ConnectionConstants.RefMaster_Connection);

                    Dictionary<int, string> blockTypeIdVsName = new Dictionary<int, string>();
                    var reportsAvailableDict = SetReportsAvailableDict(blockTypeDS, blockTypeIdVsName);
                    foreach (var blockType in Sys.BlockType)
                    {
                        if (blockType.BlockTypeId == 1 || blockType.BlockTypeId == 2 || blockType.BlockTypeId == 6)
                        {
                            foreach (var report in blockType.ReportsAvailable)
                            {
                                bool isRef = report.Module == "Ref" ? true : false;
                                if (reportsAvailableDict[blockType.BlockTypeId][isRef].ContainsKey(report.Name))
                                {
                                    var reportId = reportsAvailableDict[blockType.BlockTypeId][isRef][report.Name];
                                    report.Id = reportId;
                                }
                                int reportType = 0;
                                int IsLegReport = 0;
                                var masterReportName = "";
                                if (blockType.BlockTypeId == 1 || blockType.BlockTypeId == 6)
                                {
                                    if (report.Module == "Ref")
                                    {
                                        if (moduleVsLegReports.ContainsKey(REFM_CONSTANT) && moduleVsLegReports[REFM_CONSTANT].Contains(report.Id))
                                        {
                                            IsLegReport = 1;
                                            masterReportName = Regex.Split(report.Name, " leg ", RegexOptions.IgnoreCase)[0] + " Non Time Series";
                                        }
                                    }
                                    else
                                    {
                                        if (moduleVsLegReports.ContainsKey(SECM_CONSTANT) && moduleVsLegReports[SECM_CONSTANT].Contains(report.Id))
                                        {
                                            IsLegReport = 1;
                                            masterReportName = "Security Non Time Series";
                                        }
                                    }
                                }

                                if (blockType.BlockTypeId == 2)
                                {
                                    reportType = 1;
                                    string queryStr;
                                    if (report.Module == "Ref")
                                    {
                                        queryStr = string.Format(
                                        @"
                                        DECLARE @dependentId INT ,  @reportId INT = {0}

                                        SELECT @dependentId = dependent_id FROM IVPRefMaster.dbo.ivp_refm_report_entity_map WHERE report_id = @reportId

                                        SELECT TOP 1 rep.report_name,rep.report_id FROM IVPRefMaster.dbo.ivp_refm_report rep
                                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map emap
                                        ON (rep.report_id = emap.report_id)
                                        INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                        ON(det.report_id = rep.report_id)
                                        WHERE emap.dependent_id = @dependentId AND rep.is_active = 1 AND emap.is_active = 1
                                        AND rep.report_id != @reportId AND rep.report_name LIKE '%Daily'
                                        AND rep.report_name NOT LIKE '%[ ]leg[ ]%'
                                        AND det.is_ref = 1 AND det.is_active = 1 AND det.setup_id = {1}", report.Id, setup_id);
                                    }
                                    else if (CheckDatabaseExists("IVPSecMaster"))
                                    {
                                        queryStr = string.Format(
                                       @"
                                        SELECT rep.report_name,rep.report_setup_id FROM IVPSecMaster.dbo.ivp_secm_report_setup rep
                                        INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det
                                        ON (rep.report_setup_id = det.report_id)
                                        WHERE rep.is_active = 1 AND det.is_active = 1
                                        AND det.setup_id = {0} AND rep.report_name = 'Security Daily' AND det.is_ref = 0", setup_id);
                                    }
                                    else
                                    {
                                        throw new Exception("Only Ref module is supported");
                                    }
                                    DataTable dailyReport = CommonDALWrapper.ExecuteSelectQuery(queryStr, con).Tables[0];
                                    if (dailyReport.Rows.Count == 0)
                                    {
                                        query = string.Format(@"exec [dimension].[SRM_DWH_InsertInStaticTable]  '{0}', {1}, {2}, {3}, '{4}'",
                                            report.Module == "Ref" ? report.Name.Substring(0, report.Name.IndexOf("Time Series")) + "Daily" : "Security Daily",
                                            report.Module == "Ref" ? 1 : 0,
                                            0,
                                            2,
                                            "");
                                        DataSet staticAddition = SRMDWHJobExtension.ExecuteQuery(Sys.SetupDetails.DbName, query, SRMDBQueryType.SELECT);
                                    }
                                }
                                else if (blockType.BlockTypeId == 6)
                                {
                                    reportType = 2;
                                }
                                query = string.Format(@"exec [dimension].[SRM_DWH_InsertInStaticTable]  '{0}', {1}, {2}, {3}, '{4}'", report.Name, report.Module == "Ref" ? 1 : 0, IsLegReport, reportType, masterReportName);
                                DataSet res = SRMDWHJobExtension.ExecuteQuery(Sys.SetupDetails.DbName, query, SRMDBQueryType.SELECT);
                            }
                        }
                    }
                    con.CommitTransaction();
                    if (IsNewSystem)
                    {
                        string clientName = SRMMTConfig.GetClientName();
                        if (!SRMDWHStatic.SetupVsLastRolledTime.ContainsKey(clientName))
                            SRMDWHStatic.SetupVsLastRolledTime.TryAdd(clientName, new Dictionary<int, RollingSetupInfo>());
                        SRMDWHStatic.SetupVsLastRolledTime[clientName].Add(setup_id, new RollingSetupInfo()
                        {
                            ConnectionName = Sys.SetupDetails.DbName,
                            SetupId = setup_id,
                            SetupName = Sys.SystemName
                        });
                    }
                }
                catch (Exception e)
                {
                    mLogger.Error(e.ToString());
                    con.RollbackTransaction();
                    errorMessage = "Error occured while saving report\n" + e.ToString();
                }
                finally
                {
                    try
                    {
                        if (stagingTableName != null)
                            CommonDALWrapper.ExecuteQuery("IF(OBJECT_ID('[IVPRefMaster].[dbo].[" + stagingTableName + "]') IS NOT NULL) DROP TABLE [IVPRefMaster].[dbo].[" + stagingTableName + "]", CommonQueryType.Delete, ConnectionConstants.RefMaster_Connection);
                    }
                    catch (Exception e)
                    {
                        mLogger.Error(e.ToString());
                    }
                    CommonDALWrapper.PutConnectionManager(con);
                }
            }
            else
            {
                foreach (var a in Exceptions)
                {
                    mLogger.Error(a);
                    errorMessage += a;
                    errorMessage += "<br />";
                }
                mLogger.Error(errorMessage);
            }
        }
        private void GetLegReports(ref Dictionary<string, List<int>> moduleVsLegReports)
        {
            const string REFM_CONSTANT = "REFM";
            const string SECM_CONSTANT = "SECM";
            bool isSec = CheckDatabaseExists("IVPSecMaster");
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
                            WHERE etyp.structure_type_id = 3 AND rep.is_active = 1 AND conf.is_active = 1 AND ramap.is_active = 1 AND eat.is_active = 1";
            if (isSec)
                query += @" SELECT rs.report_setup_id
                            FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attribute_mapping ram ON ra.report_attribute_id = ram.report_attribute_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_details ad ON ram.attribute_id = ad.attribute_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_table st ON ad.sectype_table_id = st.sectype_table_id
                            WHERE priority< 0 AND ad.is_active = 1 AND rs.is_active = 1
                            UNION
                            SELECT rs.[report_setup_id]
                            FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON ra.report_setup_id = rs.report_setup_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_additional_leg_attribute_mapping ram ON ra.report_attribute_id = ram.report_attribute_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs_attribute_details ad ON ram.attribute_id = ad.attribute_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs al ON ad.additional_legs_id = al.additional_legs_id
                            WHERE ad.is_active = 1 AND al.is_active = 1 AND rs.is_active = 1";

            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                moduleVsLegReports.Add(REFM_CONSTANT, ds.Tables[0].AsEnumerable().Select(x => Convert.ToInt32(x["report_id"])).ToList());
            }
            if (isSec && ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                moduleVsLegReports.Add(SECM_CONSTANT, ds.Tables[1].AsEnumerable().Select(x => Convert.ToInt32(x["report_setup_id"])).ToList());
            }
        }
        public string BulkUploadConfiguredReports(DataTable dt)
        {
            string tableName = "DownstreamSyncStagingTable" + Guid.NewGuid().ToString();
            try
            {
                mLogger.Debug("BulkUploadConfiguredReports -> End");

                var query = new List<string>();
                query.Add("CREATE TABLE [IVPRefMaster].[dbo].[" + tableName + "] (setup_name VARCHAR(500), report_id INT, BlockTypeName VARCHAR(500), is_ref BIT, Name VARCHAR(1000), BatchSize INT, CCAssemblyName VARCHAR(500), CCClassName VARCHAR(1000), CCMethodName VARCHAR(500), CustomStartDate VARCHAR(100), CustomEndDate VARCHAR(100), EndDate INT, FailureEmail VARCHAR(200), QueueName VARCHAR(100), RequireKnowledgeDateReporting BIT, RequireTimeInTSReport BIT, RequireDeletedAssetTypes BIT,  RequireLookupMassagingStartDate BIT, RequireLookupMassagingCurrentKnowledgeDate BIT,  StartDate INT, TableName VARCHAR(200) )");

                CommonDALWrapper.ExecuteSelectQuery(string.Join(Environment.NewLine, query), ConnectionConstants.RefMaster_Connection);

                CommonDALWrapper.ExecuteBulkUpload("[IVPRefMaster].[dbo].[" + tableName + "]", dt, ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                mLogger.Debug("BulkUploadConfiguredReports -> End");
            }
            return tableName;
        }
        private string ValidateReportConfigurationDates(string startDate, string endDate, string customStartDate, string customEndDate, string dateFormat, int calendarId)
        {
            bool isValid;
            string errorMsg = string.Empty;
            StartDateTypesEnum startDateEnum;
            EndDateTypesEnum endDateEnum;
            DateTypeInfo startDateInfo = new DateTypeInfo() { Value = startDate, CustomValue = customStartDate, DateFormat = dateFormat };
            DateTypeInfo endDateInfo = new DateTypeInfo() { Value = endDate, CustomValue = customEndDate, DateFormat = dateFormat };

            isValid = Enum.TryParse(startDateInfo.Value, true, out startDateEnum);
            if (isValid)
            {
                startDateInfo.Id = (int)Enum.Parse(typeof(StartDateTypesEnum), startDateInfo.Value);
                if (!string.IsNullOrEmpty(endDateInfo.Value))
                    isValid = Enum.TryParse(endDateInfo.Value, true, out endDateEnum);
            }
            else
            {
                errorMsg = "Invalid Start Date provided";
            }
            if (isValid)
            {
                if (!string.IsNullOrEmpty(endDateInfo.Value))
                    endDateInfo.Id = (int)Enum.Parse(typeof(EndDateTypesEnum), endDateInfo.Value);
                errorMsg = ValidateStartAndEndDates(startDateInfo, endDateInfo, calendarId);
            }
            else
            {
                errorMsg = "Invalid End Date provided";
            }
            if (string.IsNullOrEmpty(errorMsg))
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
            return errorMsg;
        }

        private string ValidateStartAndEndDates(DateTypeInfo startDate, DateTypeInfo endDate, int calendarId)
        {
            StartDateTypesEnum startDateEnum = (StartDateTypesEnum)startDate.Id;
            EndDateTypesEnum endDateEnum = (EndDateTypesEnum)endDate.Id;
            switch (startDateEnum)
            {
                //    case DateTypesEnum.LastBusinessDayOfPreviousMonth_Plus_N:
                //    case DateTypesEnum.LastBusinessDayOfPreviousYear_Plus_N:
                //    case DateTypesEnum.FirstBusinessDayOfMonth_Plus_N:
                //    case DateTypesEnum.FirstBusinessDayOfYear_Plus_N:
                //    case DateTypesEnum.T_Minus_N:
                //        int customStartDateTN;
                //        if (startDate.CustomValue == null || string.IsNullOrEmpty(startDate.CustomValue))
                //        {
                //            return "Custom start date cannot be blank for start date " + startDate.Value;
                //        }
                //        else if (!int.TryParse(startDate.CustomValue, out customStartDateTN))
                //        {
                //            return "Custom start date should be an integer for start date " + startDate.Value;
                //        }
                //        break;
                //    case DateTypesEnum.CustomDate:
                //        DateTime customStartDate;
                //        if (startDate.CustomValue == null || string.IsNullOrEmpty(startDate.CustomValue))
                //        {
                //            return "Custom start date cannot be blank for start date " + startDate.Value;
                //        }
                //        else if (!DateTime.TryParseExact(startDate.CustomValue, new string[] { startDate.DateFormat, startDate.DateFormat + " hh:mm:ss tt" }, null, System.Globalization.DateTimeStyles.None, out customStartDate))
                //        {
                //            return "Custom start date is not in a valid format (" + startDate.DateFormat + ") for start date " + startDate.Value;
                //        }
                //        else if (customStartDate > customStartDate.Date)
                //        {
                //            return "Custom start date is not in a valid format (" + startDate.DateFormat + ") for end date " + startDate.Value;
                //        }
                //        else if (customStartDate.Date > DateTime.Today.Date)
                //        {
                //            return "Start date cannot be greater than today's date";
                //        }
                //        else
                //        {
                //            startDate.DateFormat = "yyyyMMdd";
                //            startDate.CustomValue = customStartDate.ToString(startDate.DateFormat);
                //        }
                //        break;
                //    case DateTypesEnum.Today:
                //        if (endDateEnum != DateTypesEnum.CustomDate && endDateEnum != DateTypesEnum.Now)
                //        {
                //            return "End date can only be CustomDate or Now when start date is " + startDate.Value;
                //        }
                //        break;
                //    case DateTypesEnum.Yesterday:
                //        if (endDateEnum != DateTypesEnum.Today && endDateEnum != DateTypesEnum.Now)
                //        {
                //            return "End date can only be Today or Now when start date is " + startDate.Value;
                //        }
                //        break;
                //    case DateTypesEnum.LastBusinessDay:
                //        if (endDateEnum != DateTypesEnum.Today && endDateEnum != DateTypesEnum.Now && endDateEnum != DateTypesEnum.Yesterday && endDateEnum != DateTypesEnum.CustomDate)
                //        {
                //            return "End date can only be Today, Yesterday, Now or CustomDate when start date is " + startDate.Value;
                //        }
                //        break;
                //    case DateTypesEnum.Now:
                //    case DateTypesEnum.LastBusinessDayOfMonth:
                //    case DateTypesEnum.LastBusinessDayOfYear:
                //        return "Start date cannot be " + startDate.Value;
                //    case DateTypesEnum.FirstBusinessDayOfMonth:
                //    case DateTypesEnum.FirstBusinessDayOfYear:
                //        if (endDateEnum == DateTypesEnum.Yesterday)
                //        {
                //            return "End date cannot be Yesterday when start date is " + startDate.Value;
                //        }
                //        break;
                case StartDateTypesEnum.LastExtractionDate:
                    if (endDate != null && !string.IsNullOrEmpty(endDate.Value) && endDateEnum != EndDateTypesEnum.Now)
                    {
                        return "End date cannot be set when start date is " + startDate.Value;
                    }
                    break;
                default:
                    break;
            }
            //switch ((DateTypesEnum)endDate.Id)
            //{
            //    case DateTypesEnum.LastBusinessDayOfPreviousMonth_Plus_N:
            //    case DateTypesEnum.LastBusinessDayOfPreviousYear_Plus_N:
            //    case DateTypesEnum.FirstBusinessDayOfMonth_Plus_N:
            //    case DateTypesEnum.FirstBusinessDayOfYear_Plus_N:
            //    case DateTypesEnum.T_Minus_N:
            //        int customEndDateTN;
            //        if (endDate.CustomValue == null || string.IsNullOrEmpty(endDate.CustomValue))
            //        {
            //            return "Custom end date cannot be blank for end date " + endDate.Value;
            //        }
            //        else if (!int.TryParse(endDate.CustomValue, out customEndDateTN))
            //        {
            //            return "Custom end date should be an integer for end date " + endDate.Value;
            //        }
            //        break;
            //    case DateTypesEnum.CustomDate:
            //        DateTime customEndDate;
            //        if (endDate.CustomValue == null || string.IsNullOrEmpty(endDate.CustomValue))
            //        {
            //            return "Custom end date cannot be blank for end date " + endDate.Value;
            //        }
            //        else if (!DateTime.TryParseExact(endDate.CustomValue, new string[] { endDate.DateFormat, endDate.DateFormat + " hh:mm:ss tt" }, null, System.Globalization.DateTimeStyles.None, out customEndDate))
            //        {
            //            return "Custom end date is not in a valid format (" + endDate.DateFormat + ") for end date " + endDate.Value;
            //        }
            //        else if (customEndDate > customEndDate.Date)
            //        {
            //            return "Custom end date is not in a valid format (" + endDate.DateFormat + ") for end date " + endDate.Value;
            //        }
            //        else if (customEndDate.Date > DateTime.Today.Date)
            //        {
            //            return "End date cannot be greater than today's date";
            //        }
            //        else
            //        {
            //            endDate.DateFormat = "yyyyMMdd";
            //            endDate.CustomValue = customEndDate.ToString(endDate.DateFormat);
            //        }
            //        break;
            //    case DateTypesEnum.None:
            //        if (!string.IsNullOrEmpty(endDate.Value))
            //            return "End date cannot be None";
            //        break;
            //}

            //DateTime calculatedStartDate = GetReportDate(calendarId, startDate);
            //DateTime calculatedEndDate = GetReportDate(calendarId, endDate);
            //if (calculatedStartDate > calculatedEndDate && endDateEnum != DateTypesEnum.LastExtractionDate)
            //{
            //    return "Incorrect start and end date combination. Start date cannot be greater than end date";
            //}
            return string.Empty;
        }

        public DataTable CreateDataTable<TOuter, TInner>(IEnumerable<TOuter> list, string innerListPropertyName, string setupName)
        {
            PropertyInfo[] outerProperties = typeof(TOuter).GetProperties().Where(pi => pi.Name != innerListPropertyName).ToArray();
            PropertyInfo[] innerProperties = typeof(TInner).GetProperties();
            MethodInfo innerListGetter = typeof(TOuter).GetProperty(innerListPropertyName).GetMethod;
            Type type = typeof(TInner);
            IEnumerable<PropertyInfo> Iprop = FlattenType(type);
            // set up columns
            DataSet blockTypeDS = CommonDALWrapper.ExecuteSelectQuery("SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_type where block_type_name Not In('Yearly','Quarterly','Monthly')", ConnectionConstants.RefMaster_Connection);

            Dictionary<int, string> blockTypeIdVsName = new Dictionary<int, string>();
            var reportsAvailableDict = SetReportsAvailableDict(blockTypeDS, blockTypeIdVsName);
            string allcol = "";
            DataTable table = new DataTable();
            table.Columns.Add("setup_name");
            table.Columns.Add("report_id");
            foreach (PropertyInfo pi in outerProperties)
            {
                allcol += pi.Name;
                table.Columns.Add(pi.Name, Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType);
            }
            foreach (PropertyInfo pi in Iprop)
            {
                if (pi.Name.Equals("Module", StringComparison.OrdinalIgnoreCase))
                {
                    table.Columns.Add("Module");
                    table.Columns.Add("is_ref");
                }
                else

                if (pi.Name.Equals("StartDateValue", StringComparison.OrdinalIgnoreCase))
                {
                    table.Columns.Add("StartDate");
                }
                else if (pi.Name.Equals("EndDateValue", StringComparison.OrdinalIgnoreCase))
                {
                    table.Columns.Add("EndDate");
                }
                else
                {
                    table.Columns.Add(pi.Name, Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType);
                }
                allcol += pi.Name;

            }

            // iterate through outer items
            foreach (TOuter outerItem in list)
            {
                var innerList = innerListGetter.Invoke(outerItem, null) as IEnumerable<TInner>;
                if (innerList == null || innerList.Count() == 0)
                {
                    // outer item has no inner items
                    DataRow row = table.NewRow();
                    row["setup_name"] = setupName;
                    foreach (PropertyInfo pi in outerProperties)
                        row[pi.Name] = pi.GetValue(outerItem) ?? DBNull.Value;
                    table.Rows.Add(row);
                }
                else
                {
                    // iterate through inner items
                    foreach (object innerItem in innerList)
                    {
                        var currentBlockTypeId = 0;
                        string module = "";
                        bool isRef = false;
                        DataRow row = table.NewRow();
                        row["setup_name"] = setupName;
                        foreach (PropertyInfo pi in outerProperties)
                        {
                            if (pi.Name == "BlockTypeId")
                            {
                                currentBlockTypeId = Convert.ToInt32(pi.GetValue(outerItem));
                            }
                            row[pi.Name] = pi.GetValue(outerItem) ?? DBNull.Value;
                        }

                        foreach (PropertyInfo pi in Iprop)
                        {
                            if (pi.Name == "Module")
                            {
                                module = Convert.ToString(pi.GetValue(innerItem));
                                isRef = module == "Ref" ? true : false;
                                row["is_ref"] = module == "Ref" ? true : false;

                            }
                            if (pi.Name == "Name")
                            {
                                if (reportsAvailableDict[currentBlockTypeId][isRef].ContainsKey(Convert.ToString(pi.GetValue(innerItem))))
                                {
                                    var reportId = reportsAvailableDict[currentBlockTypeId][isRef][Convert.ToString(pi.GetValue(innerItem))];
                                    row["report_id"] = reportId;
                                }
                            }
                            try { row[pi.Name] = pi.GetValue(innerItem) ?? DBNull.Value; }
                            catch (Exception ex)
                            {
                                var prop = pi.Name;
                                var propValue = GetPropertyValue(innerItem, "Details." + prop);

                                if (pi.Name.Equals("StartDateValue", StringComparison.OrdinalIgnoreCase))
                                {
                                    var dateTypeNameVsId = GetAllDateTypes();
                                    row["StartDate"] = Convert.ToInt32(dateTypeNameVsId[Convert.ToString(propValue)]);
                                }
                                else if (pi.Name.Equals("EndDateValue", StringComparison.OrdinalIgnoreCase))
                                {
                                    var dateTypeNameVsId = GetAllDateTypes();
                                    row["EndDate"] = Convert.ToInt32(dateTypeNameVsId[Convert.ToString(propValue)]);
                                }
                                else
                                {
                                    row[pi.Name] = propValue ?? DBNull.Value;
                                }

                            }
                        }
                        table.Rows.Add(row);
                    }
                }
            }
            if (table.Columns.Contains("BlockTypeId"))
            {
                table.Columns.Remove("BlockTypeId");
            }
            if (table.Columns.Contains("Id"))
            {
                table.Columns.Remove("Id");
            }
            if (table.Columns.Contains("Module"))
            {
                table.Columns.Remove("Module");
            }
            if (table.Columns.Contains("IsBlockTypeConfigured"))
            {
                table.Columns.Remove("IsBlockTypeConfigured");
            }
            if (table.Columns.Contains("IsReportConfigured"))
            {
                table.Columns.Remove("IsReportConfigured");
            }
            if (table.Columns.Contains("text"))
            {
                table.Columns.Remove("text");
            }
            if (table.Columns.Contains("value"))
            {
                table.Columns.Remove("value");
            }
            return table;
        }

        public object GetPropertyValue(object obj, string propertyName)
        {
            foreach (var prop in propertyName.Split('.').Select(s => obj.GetType().GetProperty(s)))
                obj = prop.GetValue(obj, null);

            return obj;
        }

        public static IEnumerable<PropertyInfo> FlattenType(Type type)
        {
            var properties = type.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                if (info.PropertyType.Assembly.GetName().Name == "mscorlib")
                {
                    yield return info;
                }
                else
                {
                    foreach (var childInfo in FlattenType(info.PropertyType))
                    {
                        if (childInfo.PropertyType.Assembly.GetName().Name == "mscorlib")
                        {
                            yield return childInfo;
                        }
                        else
                        {
                            foreach (var childInfo1 in FlattenType(childInfo.PropertyType))
                            {
                                yield return childInfo1;
                            }
                        }
                    }
                }
            }
        }
        public Dictionary<int, string> GetAllCalendars()
        {
            Dictionary<int, string> calendarIdVsName = new Dictionary<int, string>();
            RCalendarController objCalController = new RCalendarController();
            DataSet ds = objCalController.GetAllCalendarsSorted();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                calendarIdVsName.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["calendar_id"]), Convert.ToString(ds.Tables[0].Rows[i]["calendar_name"]));
            }
            return calendarIdVsName;
        }
        public Dictionary<string, int> GetAllDateTypes()
        {
            Dictionary<string, int> dateTypeNameVsId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            dateTypeNameVsId.Add("None", 0);
            dateTypeNameVsId.Add("Today", 1);
            dateTypeNameVsId.Add("Yesterday", 2);
            dateTypeNameVsId.Add("LastBusinessDay", 3);
            dateTypeNameVsId.Add("T-n", 4);
            dateTypeNameVsId.Add("CustomDate", 5);
            dateTypeNameVsId.Add("Now", 6);
            dateTypeNameVsId.Add("FirstBusinessDayOfMonth", 7);
            dateTypeNameVsId.Add("FirstBusinessDayOfYear", 8);
            dateTypeNameVsId.Add("LastBusinessDayOfMonth", 9);
            dateTypeNameVsId.Add("LastBusinessDayOfYear", 10);
            dateTypeNameVsId.Add("LastBusinessDayOfPreviousMonth_Plus_N", 11);
            dateTypeNameVsId.Add("LastBusinessDayOfPreviousYear_Plus_N", 12);
            dateTypeNameVsId.Add("FirstBusinessDayOfMonth_Plus_N", 13);
            dateTypeNameVsId.Add("FirstBusinessDayOfYear_Plus_N", 14);
            dateTypeNameVsId.Add("LastExtractionDate", 100);
            return dateTypeNameVsId;
        }
        public static DataSet ConvertObjectSetToDataSet(ObjectSet objSet)
        {
            mLogger.Debug("SRMDownstreamConfiguration -> ConvertObjectSetToDataSet -> Start");
            try
            {
                DataSet ds = new DataSet();

                if (objSet != null)
                {
                    foreach (ObjectTable objTable in objSet.Tables)
                    {
                        DataTable dt = objTable.ConvertToDataTable();
                        dt.TableName = objTable.TableName;
                        ds.Tables.Add(dt);
                    }
                }

                return ds;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMDownstreamConfiguration -> ConvertObjectSetToDataSet -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMDownstreamConfiguration -> ConvertObjectSetToDataSet -> End");
            }

        }
        public void StartSync(ObjectSet deltaSet, out string errorMessage, string userName = "", string dateFormat = "")
        {
            try
            {
                errorMessage = string.Empty;
                ObjectTable setupObjectTable = null;
                ObjectTable configurationObjectTable = null;
                ObjectTable schedulerObjectTable = null;

                if (string.IsNullOrEmpty(userName))
                    userName = new RCommon().SessionInfo.LoginName;
                if (string.IsNullOrEmpty(dateFormat))
                    dateFormat = new RCommon().SessionInfo.CultureInfo.LongDateFormat;

                if (deltaSet.Tables.Contains(SRM_DownstreamSync_SheetNames.Downstream_Sync_Setup) && deltaSet.Tables[SRM_DownstreamSync_SheetNames.Downstream_Sync_Setup] != null
                        && deltaSet.Tables[SRM_DownstreamSync_SheetNames.Downstream_Sync_Setup].Rows.Count > 0)
                {
                    setupObjectTable = deltaSet.Tables[SRM_DownstreamSync_SheetNames.Downstream_Sync_Setup];
                }
                if (deltaSet.Tables.Contains(SRM_DownstreamSync_SheetNames.Downstream_Sync_Configuration) && deltaSet.Tables[SRM_DownstreamSync_SheetNames.Downstream_Sync_Configuration] != null
                        && deltaSet.Tables[SRM_DownstreamSync_SheetNames.Downstream_Sync_Configuration].Rows.Count > 0)
                {
                    configurationObjectTable = deltaSet.Tables[SRM_DownstreamSync_SheetNames.Downstream_Sync_Configuration];
                }
                if (deltaSet.Tables.Contains(SRM_DownstreamSync_SheetNames.Downstream_Sync_Scheduler) && deltaSet.Tables[SRM_DownstreamSync_SheetNames.Downstream_Sync_Scheduler] != null
                        && deltaSet.Tables[SRM_DownstreamSync_SheetNames.Downstream_Sync_Scheduler].Rows.Count > 0)
                {
                    schedulerObjectTable = deltaSet.Tables[SRM_DownstreamSync_SheetNames.Downstream_Sync_Scheduler];
                }

                SRMDownstreamSyncInfo Setup = new SRMDownstreamSyncInfo();
                var duplicateSystemName = setupObjectTable.AsEnumerable().GroupBy(r => r["Setup Name"].ToString(), StringComparer.OrdinalIgnoreCase).Where(gr => gr.Count() > 1).Select(gr => gr.Key).ToList();
                if (duplicateSystemName.Count() == 0)
                {
                    var duplicateConnectionName = setupObjectTable.AsEnumerable().GroupBy(r => r["Connection Name"]).Where(gr => gr.Count() > 1).Where(gr => gr.Count() > 1).Select(gr => gr.Key).ToList();
                    if (duplicateConnectionName.Count() == 0)
                    {
                        List<string> distinctSetupNames = (setupObjectTable.AsEnumerable().Select(x => x["Setup Name"].ToString()).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList());
                        Dictionary<string, string> setupNameVsConnectionNameDict = new Dictionary<string, string>();
                        string query = " Select setup_name,connection_name from IVPRefMaster.dbo.ivp_srm_dwh_downstream_master WHERE is_active = 1";
                        var setupNameConnectionNameTable = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                        var existingSetupNames = setupNameConnectionNameTable.Rows.OfType<DataRow>()
             .Select(dr => dr.Field<string>("setup_name")).ToList();
                        foreach (DataRow row in setupNameConnectionNameTable.Rows)
                        {
                            var setupName = Convert.ToString(row["setup_name"]).Trim().ToLower();
                            if (setupNameVsConnectionNameDict.ContainsKey(setupName))
                            {
                                errorMessage = "Multiple Setup with same name present in database";
                                return;
                            }
                            else
                                setupNameVsConnectionNameDict.Add(setupName, Convert.ToString(row["connection_name"]).ToLower());
                        }
                        var commonItems = distinctSetupNames.FindAll(elem => existingSetupNames.Contains(elem, StringComparer.CurrentCultureIgnoreCase));

                        Dictionary<string, SRMDownstreamSyncInfo> setupNameVsSetupInfo = new Dictionary<string, SRMDownstreamSyncInfo>(StringComparer.CurrentCultureIgnoreCase);
                        Dictionary<string, ObjectTable> setupNameVsSchedulerInfo = new Dictionary<string, ObjectTable>();
                        if (setupObjectTable != null && setupObjectTable.Rows.Count > 0)
                        {
                            for (int i = 0; i < setupObjectTable.Rows.Count; i++)
                            {
                                Setup = new SRMDownstreamSyncInfo();
                                ObjectRow row = setupObjectTable.Rows[i];
                                Setup.SystemName = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Setup_Name]).Trim();
                                Setup.SetupDetails.DbName = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Connection_Name]);
                                Setup.SetupDetails.CalendarName = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Calendar]);
                                Setup.SetupDetails.EffectiveDate = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Effective_From_Date]);
                                if (!setupNameVsConnectionNameDict.ContainsKey(Setup.SystemName.ToLower()) && setupNameVsConnectionNameDict.ContainsValue(Setup.SetupDetails.DbName.ToLower()))
                                {
                                    errorMessage = "Connection name for Setup " + Setup.SystemName + " already in use by Setup " + setupNameVsConnectionNameDict.FirstOrDefault(x => x.Value == Setup.SetupDetails.DbName).Key;
                                    return;
                                }
                                if (!setupNameVsSetupInfo.ContainsKey(Setup.SystemName))
                                    setupNameVsSetupInfo.Add(Setup.SystemName, Setup);
                            }
                        }
                        if (configurationObjectTable != null && configurationObjectTable.Rows.Count > 0)
                        {
                            ModifyObjectTableColumnNames(configurationObjectTable);
                            query = " SELECT * FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_type where block_type_name Not In('Yearly','Quarterly','Monthly')";
                            DataTable blockTypeIdVsName = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                            query = " select * from  IVPRefMaster.dbo.ivp_refm_report";
                            DataTable refReports = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                            DataTable secReports = null;
                            if (CheckDatabaseExists("IVPSecMaster"))
                            {
                                query = " select * from  IVPSecMaster.dbo.ivp_secm_report_setup";
                                secReports = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.SecMaster_Connection).Tables[0];
                            }
                            for (int j = 0; j < configurationObjectTable.Rows.Count; j++)
                            {
                                ObjectRow dataRow = configurationObjectTable.Rows[j];
                                var setupName = Convert.ToString(dataRow[SRM_DownstreamSync_ColumnNames.Setup_Name]);
                                var currentBlockTypeName = Convert.ToString(dataRow[SRM_DownstreamSync_ColumnNames.Block_Type_Name]);
                                int currentBlockTypeId = (from DataRow dr in blockTypeIdVsName.Rows where (string)dr["block_type_name"] == currentBlockTypeName select (int)dr["block_type_id"]).FirstOrDefault();
                                if (currentBlockTypeId == 0)
                                {
                                    errorMessage = "Invalid Block Type Provided.";
                                    return;
                                }
                                if (Convert.ToString(dataRow[SRM_DownstreamSync_ColumnNames.Setup_Name]) == "")
                                {
                                    errorMessage = "Setup Name Not Provided.";
                                    return;
                                }
                                if (Convert.ToString(dataRow[SRM_DownstreamSync_ColumnNames.Report_Name]) == "")
                                {
                                    errorMessage = "Report Name Not Provided.";
                                    return;
                                }
                                if (currentBlockTypeName != "Time Series" && !string.IsNullOrEmpty(Convert.ToString(dataRow["require_time_in_ts_report"])))
                                {
                                    errorMessage = "Require IntraDay Changes is allowed on Time Series Report only";
                                    return;
                                }

                                if (currentBlockTypeName != "Time Series" && !string.IsNullOrEmpty(Convert.ToString(dataRow["require_lookup_massaging_start_date"])))
                                {
                                    errorMessage = "Attribute value massaging based on Start Date is allowed on Time Series Report only";
                                    return;
                                }

                                if (currentBlockTypeName != "Time Series" && !string.IsNullOrEmpty(Convert.ToString(dataRow["require_lookup_massaging_current_knowledge_date"])))
                                {
                                    errorMessage = "Attribute value massaging based on Knowledge Date is allowed on Time Series Report only";
                                    return;
                                }
                                if (setupNameVsSetupInfo.ContainsKey(setupName))
                                {
                                    Setup = setupNameVsSetupInfo[setupName];
                                }

                                SRMDownstreamSyncReportsAvailable reportsAvailable = new SRMDownstreamSyncReportsAvailable();
                                SRMDownstreamSyncReportDetails configuredDetails = new SRMDownstreamSyncReportDetails();

                                reportsAvailable.Module = Convert.ToString(dataRow[SRM_DownstreamSync_ColumnNames.Module]);
                                reportsAvailable.Name = Convert.ToString(dataRow[SRM_DownstreamSync_ColumnNames.Report_Name]);
                                reportsAvailable.text = Convert.ToString(dataRow[SRM_DownstreamSync_ColumnNames.Report_Name]);
                                reportsAvailable.IsReportConfigured = Convert.ToBoolean(1);
                                if (secReports == null && reportsAvailable.Module.ToLower() == "sec")
                                {
                                    errorMessage = "Report Cant be Security Report.";
                                    return;
                                }
                                else
                                {
                                    bool found = false;
                                    DataTable tableForCheck;
                                    if (reportsAvailable.Module.ToLower() == "ref")
                                        tableForCheck = refReports;
                                    else tableForCheck = secReports;
                                    foreach (DataRow reportName in tableForCheck.Rows)
                                    {
                                        var report_name = Convert.ToString(reportName["report_name"]);
                                        if (reportsAvailable.Name.Equals(report_name, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            found = true;
                                            reportsAvailable.Name = report_name;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        errorMessage = "Report Name doesnt exist: " + reportsAvailable.Name + " in Setup: " + setupName;
                                        return;
                                    }
                                }

                                configuredDetails = setConfiguredProperties(configurationObjectTable, j, false);
                                reportsAvailable.Details = configuredDetails;
                                var existingBlockType = Setup.BlockType.Where(p => p.BlockTypeId == currentBlockTypeId).ToList();

                                if (existingBlockType.Count == 0)
                                {
                                    SRMDownstreamSyncBlockType blockType = new SRMDownstreamSyncBlockType();

                                    List<SRMDownstreamSyncReportsAvailable> reportsAvailableList = new List<SRMDownstreamSyncReportsAvailable>();
                                    blockType.BlockTypeName = currentBlockTypeName;
                                    blockType.BlockTypeId = currentBlockTypeId;
                                    if (reportsAvailable.IsReportConfigured)
                                    {
                                        blockType.IsBlockTypeConfigured = true;
                                    }
                                    configuredDetails = setConfiguredProperties(configurationObjectTable, j, false);
                                    reportsAvailable.Details = configuredDetails;
                                    reportsAvailableList.Add(reportsAvailable);
                                    blockType.ReportsAvailable = reportsAvailableList;
                                    if (setupNameVsSetupInfo.ContainsKey(setupName))
                                    {
                                        Setup = setupNameVsSetupInfo[setupName];
                                        Setup.BlockType.Add(blockType);
                                    }
                                }
                                else
                                {
                                    var existingBlockTypeData = existingBlockType[0];
                                    if (reportsAvailable.IsReportConfigured)
                                    {
                                        existingBlockTypeData.IsBlockTypeConfigured = true;
                                    }
                                    existingBlockTypeData.ReportsAvailable.Add(reportsAvailable);
                                }
                            }
                        }
                        if (schedulerObjectTable != null && schedulerObjectTable.Rows.Count > 0)
                        {
                            for (int i = 0; i < schedulerObjectTable.Rows.Count; i++)
                            {
                                ObjectRow row = schedulerObjectTable.Rows[i];
                                ObjectTable table = schedulerObjectTable.Clone();
                                table.ImportRow(row);
                                if (!table.Columns.Contains(SRM_DownstreamSync_SpecialColumnNames.Remarks))
                                    table.Columns.Add(new ObjectColumn(SRM_DownstreamSync_SpecialColumnNames.Remarks, typeof(string)));
                                if (!table.Columns.Contains(SRM_DownstreamSync_SpecialColumnNames.Sync_Status))
                                    table.Columns.Add(new ObjectColumn(SRM_DownstreamSync_SpecialColumnNames.Sync_Status, typeof(string)));
                                var setupName = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Setup_Name]);
                                if (!setupNameVsSchedulerInfo.ContainsKey(setupName.Trim().ToLower()))
                                    setupNameVsSchedulerInfo.Add(setupName.Trim().ToLower(), table);
                            }
                        }
                        foreach (SRMDownstreamSyncInfo setup in setupNameVsSetupInfo.Values)
                        {
                            ObjectTable schedulerInfo = null;
                            if (setupNameVsSchedulerInfo.ContainsKey(setup.SystemName.Trim().ToLower()))
                                schedulerInfo = setupNameVsSchedulerInfo[setup.SystemName.Trim().ToLower()];

                            if (commonItems.Contains(setup.SystemName, StringComparer.CurrentCultureIgnoreCase))
                            {
                                SRMDownstreamSyncSaveReports(setup, false, out errorMessage, schedulerInfo, userName, dateFormat);
                            }
                            else
                            {
                                SRMDownstreamSyncSaveReports(setup, true, out errorMessage, schedulerInfo, userName, dateFormat);
                            }
                        }
                        if (schedulerObjectTable != null)
                            for (int i = 0; i < schedulerObjectTable.Rows.Count; i++)
                            {
                                ObjectRow row = schedulerObjectTable.Rows[i];
                                var setupName = Convert.ToString(row[SRM_DownstreamSync_ColumnNames.Setup_Name]);
                                if (setupNameVsSchedulerInfo.ContainsKey(setupName.Trim().ToLower()))
                                {
                                    if (schedulerObjectTable.Columns.Contains(SRM_DownstreamSync_SpecialColumnNames.Remarks))
                                        row[SRM_DownstreamSync_SpecialColumnNames.Remarks] = setupNameVsSchedulerInfo[setupName.Trim().ToLower()].Rows[0][SRM_DownstreamSync_SpecialColumnNames.Remarks];

                                    if (schedulerObjectTable.Columns.Contains(SRM_DownstreamSync_SpecialColumnNames.Sync_Status))
                                        row[SRM_DownstreamSync_SpecialColumnNames.Sync_Status] = setupNameVsSchedulerInfo[setupName.Trim().ToLower()].Rows[0][SRM_DownstreamSync_SpecialColumnNames.Sync_Status];
                                }
                            }
                        if (errorMessage == "")
                        {
                            for (int i = 0; i < setupObjectTable.Rows.Count; i++)
                            {
                                ObjectRow row = setupObjectTable.Rows[i];
                                if (setupObjectTable.Columns.Contains(SRM_DownstreamSync_SpecialColumnNames.Sync_Status))
                                    if (row[SRM_DownstreamSync_SpecialColumnNames.Sync_Status].ToString() != SRM_DownstreamSync_MigrationStatus.Already_In_Sync) row[SRM_DownstreamSync_SpecialColumnNames.Sync_Status] = SRM_DownstreamSync_MigrationStatus.Passed;
                            }
                            for (int i = 0; i < configurationObjectTable.Rows.Count; i++)
                            {
                                ObjectRow row = configurationObjectTable.Rows[i];
                                if (configurationObjectTable.Columns.Contains(SRM_DownstreamSync_SpecialColumnNames.Sync_Status))
                                    if (row[SRM_DownstreamSync_SpecialColumnNames.Sync_Status].ToString() != SRM_DownstreamSync_MigrationStatus.Already_In_Sync) row[SRM_DownstreamSync_SpecialColumnNames.Sync_Status] = SRM_DownstreamSync_MigrationStatus.Passed;
                            }
                        }
                    }
                    else
                    {
                        errorMessage = "Duplicate Connection Name Provided.";
                    }
                }
                else
                {
                    errorMessage = "Duplicate System Name Provided.";
                }
                if (!string.IsNullOrEmpty(errorMessage))
                    throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
            }
        }

        public void ModifyObjectTableColumnNames(ObjectTable configurationObjectTable)
        {
            configurationObjectTable.Columns["Start Date"].ColumnName = "Start_Date";
            //configurationObjectTable.Columns["Custom Start Date"].ColumnName = "Custom_Start_Date";
            configurationObjectTable.Columns["End Date"].ColumnName = "End_Date";
            //configurationObjectTable.Columns["Custom End Date"].ColumnName = "Custom_End_Date";
            configurationObjectTable.Columns["Table Name"].ColumnName = "Table_Name";
            configurationObjectTable.Columns["Batch Size"].ColumnName = "Batch_Size";
            configurationObjectTable.Columns["Require Knowledge Date Reporting"].ColumnName = "require_knowledge_date_reporting";
            configurationObjectTable.Columns["Require IntraDay Changes"].ColumnName = "Require_Time_in_TS_Report";
            configurationObjectTable.Columns["Require Deleted Entities"].ColumnName = "Require_Deleted_Asset_Types";
            configurationObjectTable.Columns["Attribute value massaging based on Start Date"].ColumnName = "Require_Lookup_Massaging_Start_Date";
            configurationObjectTable.Columns["Attribute value massaging based on Knowledge Date"].ColumnName = "Require_Lookup_Massaging_Current_Knowledge_Date";
            configurationObjectTable.Columns["Custom Class Assembly Name"].ColumnName = "CC_Assembly_Name";
            configurationObjectTable.Columns["Custom Class Name"].ColumnName = "CC_Class_Name";
            configurationObjectTable.Columns["Custom Class Method Name"].ColumnName = "CC_Method_Name";
            configurationObjectTable.Columns["Queue Name"].ColumnName = "Queue_Name";
            configurationObjectTable.Columns["Failure Email Id"].ColumnName = "Failure_Email_Id";
        }
        public ObjectSet GetConfiguredReports(List<int> SetupIds, out string errorMsg, string dateformat = "")
        {
            mLogger.Debug("SRMDownstreamConfiguration : GetConfiguredReports() --> Start");
            RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
            errorMsg = "";
            DataSet osResult = new DataSet();
            ObjectSet result = new ObjectSet();
            DataTable configurationObjectTable = new DataTable();
            DataTable setupOt = new DataTable();
            DataTable schedulerOt = new DataTable();
            if (string.IsNullOrEmpty(dateformat))
                dateformat = new RCommon().SessionInfo.CultureInfo.LongDateFormat;
            try
            {
                if (SetupIds == null)
                {
                    SetupIds = new List<int>();
                }
                string selectedSystemName = null;
                string query = "select * from  IVPRefMaster.dbo.ivp_srm_dwh_downstream_master WHERE is_active = 1";
                DataSet setupDS = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

                if (setupDS != null && setupDS.Tables.Count > 0 && setupDS.Tables[0] != null && setupDS.Tables[0].Rows.Count > 0)
                {
                    Dictionary<string, Dictionary<string, string>> systemDict = GetSystemNameVsConnectionDetailsDict(setupDS, false, selectedSystemName, SetupIds);

                    setupOt.Columns.Add("Setup Name");
                    setupOt.Columns.Add("Connection Name");
                    setupOt.Columns.Add("Calendar");
                    setupOt.Columns.Add("Effective From Date");
                    bool isSec = CheckDatabaseExists("IVPSecMaster");
                    DataSet ds;
                    if (SetupIds != null && SetupIds.Count != 0)
                    {
                        configurationObjectTable = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT mas.setup_name as 'Setup Name', bl.block_type_name  as 'Block Type', 
                        {0}, 
                        det.is_ref , det.start_date as start_date_id, 
                        det.end_date  as end_date_id,det.table_name  as 'Table Name', det.batch_size as 'Batch Size',det.require_time_in_ts_report  as 'Require IntraDay Changes', det.require_knowledge_date_reporting as 'Require Knowledge Date Reporting',
                        det.require_deleted_asset_types  as 'Require Deleted Entities', det.require_lookup_massaging_start_date  as 'Attribute value massaging based on Start Date',det.require_lookup_massaging_current_knowledge_date as 'Attribute value massaging based on Knowledge Date', det.cc_assembly_name  as 'Custom Class Assembly Name', 
                        det.cc_class_name  as 'Custom Class Name', det.cc_method_name  as 'Custom Class Method Name',det.queue_name as 'Queue Name', det.failure_email_id as 'Failure Email Id'
                        FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master mas 
                        INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det ON(mas.setup_id = det.setup_id) 
                        INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_type bl ON(bl.block_type_id = det.block_type_id)
                        LEFT JOIN IVPRefMaster.dbo.ivp_refm_report rep ON(rep.report_id = det.report_id AND det.is_Ref = 1 AND rep.is_active = 1 ) 
                        {1}
                        WHERE mas.setup_id in (" + string.Join<int>(",", SetupIds) + ") AND mas.is_active = 1 AND det.is_active = 1 ORDER BY mas.setup_id,bl.block_type_id",
                        isSec ? "CASE WHEN is_ref = 1 THEN rep.report_name ELSE srep.report_name END as 'Report Name'" : "rep.report_name as 'Report Name'",
                        isSec ? "LEFT JOIN IVPSecMaster.dbo.ivp_secm_report_setup srep ON(srep.report_setup_id = det.report_id AND det.is_Ref = 0 AND srep.is_active = 1) " : " "), ConnectionConstants.RefMaster_Connection).Tables[0];
                        ds = CommonDALWrapper.ExecuteSelectQuery("SELECT ct.setup_name AS[Setup Name], CASE WHEN sj.recurrence_type = 1 THEN 'Recurring' ELSE 'Non-Recurring' END AS[Recurrence Type], sj.next_schedule_time AS[Start_Date],CASE WHEN sj.recurrence_type = 1 THEN sj.recurrence_pattern else null end AS [Recurrence Pattern] ,  CASE WHEN recurrence_pattern = 'daily' THEN day_interval WHEN recurrence_pattern = 'weekly' THEN week_interval WHEN recurrence_pattern = 'monthly' THEN month_interval END AS[Interval], end_date AS[End_Date], no_of_recurrences AS[Number of Recurrences], CAST(start_time AS TIME) AS[Start_Time], time_interval_of_recurrence AS[Time Interval of Recurrence], no_end_date AS[Never End Job], days_of_week FROM dbo.ivp_srm_dwh_downstream_master ct INNER JOIN dbo.ivp_srm_dwh_downstream_scheduled_jobs sj ON(ct.setup_id = sj.setup_id AND sj.is_active = 1) Where ct.is_active=1 AND sj.setup_id IN (" + string.Join<int>(",", SetupIds) + ") ORDER BY sj.setup_id ", ConnectionConstants.RefMaster_Connection);
                    }
                    else
                    {
                        configurationObjectTable = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT mas.setup_name as 'Setup Name', bl.block_type_name  as 'Block Type', 
                        {0},
                        det.is_ref , det.start_date as start_date_id, 
                        det.end_date  as end_date_id,det.table_name  as 'Table Name', det.batch_size as 'Batch Size',det.require_time_in_ts_report  as 'Require IntraDay Changes', det.require_knowledge_date_reporting as 'Require Knowledge Date Reporting',
                        det.require_deleted_asset_types  as 'Require Deleted Entities', det.require_lookup_massaging_start_date  as 'Attribute value massaging based on Start Date',det.require_lookup_massaging_current_knowledge_date as 'Attribute value massaging based on Knowledge Date', det.cc_assembly_name  as 'Custom Class Assembly Name', 
                        det.cc_class_name  as 'Custom Class Name', det.cc_method_name  as 'Custom Class Method Name',det.queue_name as 'Queue Name', det.failure_email_id as 'Failure Email Id'
                        FROM IVPRefMaster.dbo.ivp_srm_dwh_downstream_master mas 
                        INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_details det ON(mas.setup_id = det.setup_id) 
                        INNER JOIN IVPRefMaster.dbo.ivp_srm_dwh_downstream_block_type bl ON(bl.block_type_id = det.block_type_id)
                        LEFT JOIN IVPRefMaster.dbo.ivp_refm_report rep ON(rep.report_id = det.report_id AND det.is_Ref = 1 AND rep.is_active = 1 ) 
                        {1}
                        Where mas.is_active = 1 AND det.is_active = 1 ORDER BY mas.setup_id,bl.block_type_id",
                        isSec ? "CASE WHEN is_ref = 1 THEN rep.report_name ELSE srep.report_name END as 'Report Name'" : "rep.report_name as 'Report Name'",
                        isSec ? "LEFT JOIN IVPSecMaster.dbo.ivp_secm_report_setup srep ON(srep.report_setup_id = det.report_id AND det.is_Ref = 0 AND srep.is_active = 1) " : " "), ConnectionConstants.RefMaster_Connection).Tables[0];
                        ds = CommonDALWrapper.ExecuteSelectQuery("SELECT ct.setup_name AS[Setup Name], CASE WHEN sj.recurrence_type = 1 THEN 'Recurring' ELSE 'Non-Recurring' END AS[Recurrence Type], sj.next_schedule_time AS[Start_Date],CASE WHEN sj.recurrence_type = 1 THEN sj.recurrence_pattern else null end AS [Recurrence Pattern] ,  CASE WHEN recurrence_pattern = 'daily' THEN day_interval WHEN recurrence_pattern = 'weekly' THEN week_interval WHEN recurrence_pattern = 'monthly' THEN month_interval END AS[Interval], end_date AS[End_Date], no_of_recurrences AS[Number of Recurrences], CAST(start_time AS TIME) AS[Start_Time], time_interval_of_recurrence AS[Time Interval of Recurrence], no_end_date AS[Never End Job], days_of_week FROM dbo.ivp_srm_dwh_downstream_master ct INNER JOIN dbo.ivp_srm_dwh_downstream_scheduled_jobs sj ON(ct.setup_id = sj.setup_id AND sj.is_active = 1) Where ct.is_active=1 ORDER BY sj.setup_id ", ConnectionConstants.RefMaster_Connection);
                    }
                    schedulerOt = ds.Tables[0];
                    ds.Tables.Remove(schedulerOt);
                    schedulerOt.TableName = SRM_DownstreamSync_SheetNames.Downstream_Sync_Scheduler;

                    //   configurationObjectTable.Columns.Add("Calendar");
                    configurationObjectTable.Columns.Add("Start Date");
                    configurationObjectTable.Columns.Add("End Date");
                    configurationObjectTable.Columns.Add("Module");


                    schedulerOt.Columns.Add(SRM_DownstreamSync_ColumnNames.Start_Date);
                    schedulerOt.Columns.Add(SRM_DownstreamSync_ColumnNames.End_Date);
                    schedulerOt.Columns.Add(SRM_DownstreamSync_ColumnNames.Start_Time);
                    schedulerOt.Columns.Add(SRM_DownstreamSync_ColumnNames.Days_Of_Week);
                    DataTable dt = setupDS.Tables[0];
                    Dictionary<string, string> setupNameVsDetails = new Dictionary<string, string>();
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {

                        setupNameVsDetails.Add(Convert.ToString(dt.Rows[j]["setup_name"]), Convert.ToString(dt.Rows[j]["calendar_id"]) + "|" + ((DateTime)dt.Rows[j]["effective_from_date"]).ToString(dateformat));
                    }

                    foreach (var system in systemDict)
                    {
                        var currentSetupName = Convert.ToString(system.Key);

                        //foreach (var dict in system.Value)
                        //{
                        //}
                        var calendarIdVsName = GetAllCalendars();
                        //dataRow["Calendar"] = calendarIdVsName.ContainsKey(calendarId) ? calendarIdVsName[calendarId] : "";

                        if (systemDict.ContainsKey(currentSetupName))
                        {
                            DataRow dataRow = setupOt.NewRow();

                            var setupDetails = systemDict[system.Key];
                            dataRow["Setup Name"] = currentSetupName;
                            dataRow["Connection Name"] = setupDetails["id"];
                            int calendarId = Convert.ToInt16(setupNameVsDetails[currentSetupName].Split('|')[0]);
                            if (calendarIdVsName.ContainsKey(calendarId))
                            {
                                dataRow["Calendar"] = calendarIdVsName[calendarId];
                            }
                            else
                            {
                                errorMsg = "Invalid Calendar Provided.";
                            }
                            dataRow["Effective From Date"] = setupNameVsDetails[currentSetupName].Split('|').Last();
                            setupOt.Rows.Add(dataRow);
                        }
                    }
                    for (int j = 0; j < configurationObjectTable.Rows.Count; j++)
                    {
                        DataRow dataRow = configurationObjectTable.Rows[j];

                        var startDateId = dataRow["start_date_id"] != DBNull.Value ? Convert.ToInt32(dataRow["start_date_id"]) : 0;
                        dataRow["Start Date"] = Convert.ToString((StartDateTypesEnum)startDateId);

                        if (!dataRow["Block Type"].ToString().Equals("Time Series", StringComparison.OrdinalIgnoreCase))
                        {
                            dataRow["Attribute value massaging based on Start Date"] = DBNull.Value;
                            dataRow["Attribute value massaging based on Knowledge Date"] = DBNull.Value;
                        }

                        //var calendarId = dataRow["calendar_id"] != DBNull.Value ? Convert.ToInt32(dataRow["calendar_id"]) : 0;
                        //var calendarIdVsName = GetAllCalendars();
                        //dataRow["Calendar"] = calendarIdVsName.ContainsKey(calendarId) ? calendarIdVsName[calendarId] : "";

                        var endDateId = dataRow["end_date_id"] != DBNull.Value ? Convert.ToInt32(dataRow["end_date_id"]) : 0;
                        dataRow["End Date"] = Convert.ToString((EndDateTypesEnum)endDateId);

                        var module = Convert.ToBoolean(dataRow["is_ref"]) ? "Ref" : "Sec";
                        dataRow["Module"] = Convert.ToString(module);

                    }
                    configurationObjectTable.Columns.Remove("start_date_id");
                    // configurationObjectTable.Columns.Remove("calendar_id");
                    configurationObjectTable.Columns.Remove("end_date_id");
                    configurationObjectTable.Columns.Remove("is_ref");
                    // osResult.Tables.Add(setupOt);
                    for (int j = 0; j < schedulerOt.Rows.Count; j++)
                    {
                        DataRow dataRow = schedulerOt.Rows[j];
                        if (dataRow["Start_Date"] != DBNull.Value)
                        {
                            dataRow[SRM_DownstreamSync_ColumnNames.Start_Date] = ((DateTime)dataRow["Start_Date"]).ToString(dateformat.Split(' ')[0]);
                            dataRow[SRM_DownstreamSync_ColumnNames.Start_Time] = ((DateTime)dataRow["Start_Date"]).TimeOfDay.ToString(dateformat.Split(' ')[1].Replace(":", "\\:"));
                        }
                        if (dataRow["End_Date"] != DBNull.Value)
                            dataRow[SRM_DownstreamSync_ColumnNames.End_Date] = ((DateTime)dataRow["End_Date"]).ToString(dateformat);
                        else
                            dataRow[SRM_DownstreamSync_ColumnNames.End_Date] = "";
                        if (dataRow["days_of_week"] != DBNull.Value)
                            dataRow[SRM_DownstreamSync_ColumnNames.Days_Of_Week] = string.Join(",", SRMDownstreamStatusAndScheduler.ExtractDaysOfWeek(Convert.ToString(dataRow["days_of_week"])).Where(y => SRMDownstreamStatusAndScheduler.DaysOfWeekNumberToString.ContainsKey(y)).Select(x => SRMDownstreamStatusAndScheduler.DaysOfWeekNumberToString[x]));

                    }

                    schedulerOt.Columns.Remove("Start_Date");
                    schedulerOt.Columns.Remove("End_Date");
                    schedulerOt.Columns.Remove("Start_Time");
                    schedulerOt.Columns.Remove("days_of_week");
                }

                osResult.Merge(setupOt);
                osResult.Merge(configurationObjectTable);
                osResult.Tables.Add(schedulerOt);

                osResult.Tables[0].TableName = "Downstream Sync Setup";
                if (osResult.Tables.Count > 1)
                    osResult.Tables[1].TableName = "Downstream Sync Configuration";
                if (osResult.Tables.Count > 2)
                    osResult.Tables[2].TableName = SRM_DownstreamSync_SheetNames.Downstream_Sync_Scheduler;
                if (osResult != null)
                    return SRMCommon.ConvertDataSetToObjectSet(osResult);
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
        #region SRMDownstreamSyncVariables

        #endregion SRMDownstreamSync

    }
}
