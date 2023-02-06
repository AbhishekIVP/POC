using com.ivp.commom.commondal;
using com.ivp.common.SecMaster;
using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.common;
using com.ivp.rad.controls.xruleeditor.grammar;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.utils;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.ivp.common.reporting
{
    public class EMReportingController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("EMReportingController");
        RDBConnectionManager mDBCon;

        private Dictionary<string, List<string>> GetColumnsToRemoveForDownloadFromReportMetadata()
        {
            Dictionary<string, List<string>> dctTableIndexVsColumnsToRemove = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            dctTableIndexVsColumnsToRemove.Add(EMReportingConstants.REPORT_SETUP, new List<string>() { "report_repository_id", "report_id", "report_type_id", "entity_type_id" });
            dctTableIndexVsColumnsToRemove.Add(EMReportingConstants.ATTRIBUTE_MAPPING, new List<string>() { "report_id", "report_type_id", "dependent_id", "report_attribute_id", "dependent_attribute_id", "lookup_attribute_id" });
            dctTableIndexVsColumnsToRemove.Add(EMReportingConstants.REPORT_CONFIGURATION, new List<string>() { "report_id", "calendar_id", "start_date_type_id", "end_date_type_id" });
            dctTableIndexVsColumnsToRemove.Add(EMReportingConstants.REPORT_RULES, new List<string>() { "report_id", "rule_set_id", "rule_type_id" });
            dctTableIndexVsColumnsToRemove.Add(EMReportingConstants.REPORT_ATTRIBUTE_ORDER, new List<string>() { "report_id", "report_attribute_id", "lookup_attribute_id" });
            return dctTableIndexVsColumnsToRemove;
        }

        private Dictionary<int, string> GetTableIndexAndNameMappingForReportMetadata()
        {
            Dictionary<int, string> dctTableIndexVsName = new Dictionary<int, string>();
            dctTableIndexVsName.Add(0, EMReportingConstants.REPORT_SETUP);
            dctTableIndexVsName.Add(1, EMReportingConstants.ATTRIBUTE_MAPPING);
            dctTableIndexVsName.Add(2, EMReportingConstants.REPORT_RULES);
            dctTableIndexVsName.Add(3, EMReportingConstants.REPORT_CONFIGURATION);
            dctTableIndexVsName.Add(4, EMReportingConstants.REPORT_ATTRIBUTE_ORDER);
            return dctTableIndexVsName;
        }

        private void RenameTableNamesForReportMetadataForObjectSet(ref ObjectSet reportMetadata)
        {
            Dictionary<int, string> dctTableIndexVsName = GetTableIndexAndNameMappingForReportMetadata();
            foreach (KeyValuePair<int, string> tableInfo in dctTableIndexVsName)
            {
                reportMetadata.Tables[tableInfo.Key].TableName = tableInfo.Value;
            }
        }

        private void RenameTableNamesForReportMetadataForDataSet(ref DataSet reportMetadata)
        {
            Dictionary<int, string> dctTableIndexVsName = GetTableIndexAndNameMappingForReportMetadata();
            foreach (KeyValuePair<int, string> tableInfo in dctTableIndexVsName)
            {
                reportMetadata.Tables[tableInfo.Key].TableName = tableInfo.Value;
            }
        }

        private ObjectSet RemoveIdColumnsForDownloadFromReportMetadata(ObjectSet reportMetadata)
        {
            Dictionary<string, List<string>> dctTableIndexVsColumnsToRemove = GetColumnsToRemoveForDownloadFromReportMetadata();
            foreach (KeyValuePair<string, List<string>> tableInfo in dctTableIndexVsColumnsToRemove)
            {
                foreach (string columnName in tableInfo.Value)
                {
                    if (reportMetadata.Tables[tableInfo.Key].Columns.Contains(columnName))
                        reportMetadata.Tables[tableInfo.Key].Columns.Remove(columnName);
                }
            }
            return reportMetadata;
        }

        private DataSet RemoveIdColumnsForDownloadFromReportMetadata(DataSet reportMetadata)
        {
            Dictionary<string, List<string>> dctTableIndexVsColumnsToRemove = GetColumnsToRemoveForDownloadFromReportMetadata();
            foreach (KeyValuePair<string, List<string>> tableInfo in dctTableIndexVsColumnsToRemove)
            {
                foreach (string columnName in tableInfo.Value)
                {
                    if (reportMetadata.Tables[tableInfo.Key].Columns.Contains(columnName))
                        reportMetadata.Tables[tableInfo.Key].Columns.Remove(columnName);
                }
            }
            return reportMetadata;
        }

        public ObjectSet GetReportMetadata(List<int> lstReports, string userName, int moduleId, bool removeInternalColumns)
        {
            try
            {
                ObjectSet reportMetaData = null;
                if (lstReports == null)
                    lstReports = new List<int>();
                var result = new EMReportingDBManager().GetReportMetadata(lstReports.Cast<object>().ToList(), userName, (EMModule)moduleId, EMInputType.Id, EMDataType.ObjectSet);
                if (result != null)
                {
                    reportMetaData = (ObjectSet)result;
                    RenameTableNamesForReportMetadataForObjectSet(ref reportMetaData);
                    if (removeInternalColumns)
                        reportMetaData = RemoveIdColumnsForDownloadFromReportMetadata(reportMetaData);
                }
                return reportMetaData;
            }
            catch (Exception ex)
            {
                mLogger.Debug("GetReportMetadataForDownload - > Error -> " + ex.ToString());
                throw;
            }
        }

        public ObjectSet GetAllReports(string userName, int moduleId)
        {
            try
            {
                ObjectSet reportMetaData = null;
                var result = new EMReportingDBManager().GetAllReports(userName, (EMModule)moduleId, EMDataType.ObjectSet);
                if (result != null)
                {
                    reportMetaData = (ObjectSet)result;
                }
                return reportMetaData;
            }
            catch (Exception ex)
            {
                mLogger.Debug("GetAllReports - > Error -> " + ex.ToString());
                throw;
            }
        }

        public Dictionary<int, string> GetAllReportTypes()
        {
            try
            {
                return Enum<EMReportType>.ValueVsDescription;
                //DataSet dsResult = new EMReportingDBManager().GetAllReportTypes();
                //return dsResult.Tables[0].AsEnumerable().ToDictionary(x => x.Field<int>("Report Type ID"), y => y.Field<string>("Report Type Name"));
            }
            catch (Exception ex)
            {
                mLogger.Debug("GetAllReportTypes - > Error -> " + ex.ToString());
                throw;
            }
        }

        public List<string> GetReportAttributeDataTypes()
        {
            List<string> lstReportAttributeDataTypes = new RMCommonController().GetAttributeDataTypes();
            lstReportAttributeDataTypes.Remove("FILE");
            return lstReportAttributeDataTypes;
        }

        public Dictionary<int, string> GetReportDateTypes(EMDateType dateType = EMDateType.Both, bool forTask = false)
        {
            var dateTypes = new RMCommonController().GetDateTypes(dateType);
            if (!forTask && dateType != EMDateType.EndDate)
            {
                dateTypes.Remove((int)RMReportingTaskStartDate.LASTSUCCESSFULPUSHTIME);
            }
            return dateTypes;
        }

        public List<string> GetReportRuleTypes()
        {
            return Enum.GetNames(typeof(EMReportRuleType)).ToList();
        }

        public EMReportOutput SaveReport(EMReport reportInfo, EMCallingInterface callingInterface, EMModule module)
        {
            EMReportOutput objEMReportOutput = new EMReportOutput();
            try
            {
                if (callingInterface == EMCallingInterface.API)
                {
                    ValidateReport(reportInfo, callingInterface);
                }

                if (reportInfo != null)
                {
                    mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                    int reportId = reportInfo.Report.Id;
                    string userName = reportInfo.UserName;
                    DateTime updateTime = DateTime.Now;
                    objEMReportOutput.IsSuccess = true;
                    if (objEMReportOutput.IsSuccess)
                    {
                        objEMReportOutput = SaveReportRepository(reportInfo, callingInterface, module, updateTime);
                    }
                    if (objEMReportOutput.IsSuccess)
                    {
                        objEMReportOutput = SaveReportSetup(reportInfo, callingInterface, module, updateTime);
                    }
                    if (objEMReportOutput.IsSuccess)
                    {
                        objEMReportOutput = SaveReportAttributeMapping(reportInfo, callingInterface, module, updateTime);
                    }
                    if (objEMReportOutput.IsSuccess)
                    {
                        objEMReportOutput = SaveReportRules(reportInfo, callingInterface, module, updateTime);
                    }
                    if (objEMReportOutput.IsSuccess)
                    {
                        objEMReportOutput = SaveReportConfiguration(reportInfo, callingInterface, module, updateTime);
                    }
                    if (objEMReportOutput.IsSuccess)
                    {
                        objEMReportOutput = SaveReportAttributeConfiguration(reportInfo, callingInterface, module, updateTime);
                    }
                    if (mDBCon != null)
                    {
                        if (objEMReportOutput.IsSuccess)
                            mDBCon.CommitTransaction();
                        else
                            mDBCon.RollbackTransaction();
                    }
                }
                else
                {
                    objEMReportOutput.IsSuccess = false;
                    objEMReportOutput.Message = new List<string>() { "Report info not provided" };
                }
            }
            catch (Exception ex)
            {
                if (mDBCon != null)
                    mDBCon.RollbackTransaction();
                mLogger.Debug("SaveReport - > Error -> " + ex.ToString());
                objEMReportOutput.IsSuccess = false;
                objEMReportOutput.Message = new List<string>() { ex.Message };
            }
            finally
            {
                if (mDBCon != null)
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                    mDBCon = null;
                }
            }
            return objEMReportOutput;
        }

        private EMReportOutput SaveReportRepository(EMReport reportInfo, EMCallingInterface callingInterface, EMModule module, DateTime updateTime)
        {
            EMReportOutput objEMReportOutput = new EMReportOutput();
            objEMReportOutput.IsSuccess = true;
            if (reportInfo.Repository != null && !string.IsNullOrEmpty(reportInfo.Repository.Name))
            {
                DataSet dsResult = new DataSet();
                object objResult = new EMReportingDBManager(mDBCon).SaveReportRepository(reportInfo, updateTime, callingInterface, module, EMDataType.DataSet);
                if (objResult != null)
                {
                    dsResult = (DataSet)objResult;
                    if (dsResult.Tables.Count > 0 && dsResult.Tables[0] != null)
                    {
                        DataRow resultRow = dsResult.Tables[0].Rows[0];

                        reportInfo.Repository.Id = Convert.ToInt32(resultRow["repository_id"]);
                        objEMReportOutput.IsSuccess = Convert.ToBoolean(resultRow["Status"]);
                        objEMReportOutput.Message = new List<string>() { Convert.ToString(resultRow["Message"]) };
                    }
                    else
                    {
                        objEMReportOutput.IsSuccess = false;
                        objEMReportOutput.Message = new List<string>() { "Internal error occurred while saving report in database" };
                    }
                }
            }
            return objEMReportOutput;
        }

        private EMReportOutput SaveReportSetup(EMReport reportInfo, EMCallingInterface callingInterface, EMModule module, DateTime updateTime)
        {
            EMReportOutput objEMReportOutput = new EMReportOutput();
            objEMReportOutput.IsSuccess = true;
            if (reportInfo.Report != null && !string.IsNullOrEmpty(reportInfo.Report.Name) && reportInfo.Repository != null && !string.IsNullOrEmpty(Convert.ToString(reportInfo.Repository.Id)))
            {
                DataSet dsResult = new DataSet();
                object objResult = new EMReportingDBManager(mDBCon).SaveReportSetup(reportInfo, updateTime, callingInterface, module, EMDataType.DataSet);
                if (objResult != null)
                {
                    dsResult = (DataSet)objResult;
                    if (dsResult.Tables.Count > 0 && dsResult.Tables[0] != null)
                    {
                        DataRow resultRow = dsResult.Tables[0].Rows[0];

                        reportInfo.Report.Id = Convert.ToInt32(resultRow["report_id"]);
                        objEMReportOutput.IsSuccess = Convert.ToBoolean(resultRow["Status"]);
                        objEMReportOutput.Message = new List<string>() { Convert.ToString(resultRow["Message"]) };
                    }
                    else
                    {
                        objEMReportOutput.IsSuccess = false;
                        objEMReportOutput.Message = new List<string>() { "Internal error occurred while saving report in database" };
                    }
                }
            }
            return objEMReportOutput;
        }

        private EMReportOutput SaveReportAttributeMapping(EMReport reportInfo, EMCallingInterface callingInterface, EMModule module, DateTime updateTime)
        {
            EMReportOutput objEMReportOutput = new EMReportOutput();
            objEMReportOutput.IsSuccess = true;
            if (reportInfo.Mapping != null && reportInfo.Mapping.Count > 0 && reportInfo.Mapping.Any(x => x.Mapping != null && x.Mapping.Count > 0))
            {
                DataSet dsResult = new DataSet();
                string mappingXML = GetReportAttributeMappingXML(reportInfo);
                object objResult = new EMReportingDBManager(mDBCon).SaveReportAttributeMapping(reportInfo.Report.Id, mappingXML, reportInfo.UserName, updateTime, module, EMDataType.DataSet);
                if (objResult != null)
                {
                    dsResult = (DataSet)objResult;
                    if (dsResult.Tables.Count > 0 && dsResult.Tables[0] != null)
                    {
                        DataRow resultRow = dsResult.Tables[0].Rows[0];

                        objEMReportOutput.IsSuccess = Convert.ToBoolean(resultRow["Status"]);
                        objEMReportOutput.Message = new List<string>() { Convert.ToString(resultRow["Message"]) };
                    }
                    else
                    {
                        objEMReportOutput.IsSuccess = false;
                        objEMReportOutput.Message = new List<string>() { "Internal error occurred while saving report in database" };
                    }
                }
            }
            return objEMReportOutput;
        }

        private EMReportOutput SaveReportRules(EMReport reportInfo, EMCallingInterface callingInterface, EMModule module, DateTime updateTime)
        {
            EMReportOutput objEMReportOutput = new EMReportOutput();
            objEMReportOutput.IsSuccess = true;
            Dictionary<int, int> dctReportIdVsFilterRuleSetId = new Dictionary<int, int>();
            Dictionary<int, int> dctReportIdVsTransFormationRuleSetId = new Dictionary<int, int>();
            if (reportInfo.Rules != null && reportInfo.Rules.Count > 0)
            {
                new EMReportingDBManager(mDBCon).DeleteReportRules(reportInfo, updateTime, module, EMDataType.DataSet);
                foreach (EMRuleInfo ruleInfo in reportInfo.Rules)
                {
                    try
                    {
                        RADXRuleGrammarInfo ruleGrammarInfo = null;
                        int ruleSetID = 0;
                        //Transformation
                        if (ruleInfo.TypeId == 6)
                        {
                            if (dctReportIdVsTransFormationRuleSetId.ContainsKey(reportInfo.Report.Id))
                            {
                                ruleSetID = dctReportIdVsTransFormationRuleSetId[reportInfo.Report.Id];
                            }
                        }
                        //Filter
                        else if (ruleInfo.TypeId == 7)
                        {
                            if (dctReportIdVsFilterRuleSetId.ContainsKey(reportInfo.Report.Id))
                            {
                                ruleSetID = dctReportIdVsFilterRuleSetId[reportInfo.Report.Id];
                            }
                        }
                        ruleSetID = new RMCommonController().RMSaveRule(reportInfo.Report.Id, reportInfo.Report.Id, ruleInfo.TypeId, ruleInfo.Name, ruleInfo.Priority, ruleInfo.Text, ruleInfo.State, 0, ruleSetID, reportInfo.UserName, ConnectionConstants.RefMasterVendor_Connection, ref ruleGrammarInfo, mDBCon);
                        if (ruleInfo.TypeId == 6)
                        {
                            if (!dctReportIdVsTransFormationRuleSetId.ContainsKey(reportInfo.Report.Id))
                            {
                                dctReportIdVsTransFormationRuleSetId.Add(reportInfo.Report.Id, ruleSetID);
                            }
                        }
                        //Filter
                        else if (ruleInfo.TypeId == 7)
                        {
                            if (!dctReportIdVsFilterRuleSetId.ContainsKey(reportInfo.Report.Id))
                            {
                                dctReportIdVsFilterRuleSetId.Add(reportInfo.Report.Id, ruleSetID);
                            }
                        }
                    }
                    catch (Exception ruleEx)
                    {
                        objEMReportOutput.IsSuccess = false;
                        objEMReportOutput.Message = new List<string>() { ruleEx.ToString() };
                        break;
                    }
                }
            }
            return objEMReportOutput;
        }

        private EMReportOutput SaveReportConfiguration(EMReport reportInfo, EMCallingInterface callingInterface, EMModule module, DateTime updateTime)
        {
            EMReportOutput objEMReportOutput = new EMReportOutput();
            objEMReportOutput.IsSuccess = true;
            if (reportInfo.Configuration != null && reportInfo.Configuration.StartDate != null && reportInfo.Configuration.CalendarId > 0)
            {
                DataSet dsResult = new DataSet();
                object objResult = new EMReportingDBManager(mDBCon).SaveReportConfiguration(reportInfo, updateTime, module, EMDataType.DataSet);
                if (objResult != null)
                {
                    dsResult = (DataSet)objResult;
                    if (dsResult.Tables.Count > 0 && dsResult.Tables[0] != null)
                    {
                        DataRow resultRow = dsResult.Tables[0].Rows[0];

                        objEMReportOutput.IsSuccess = Convert.ToBoolean(resultRow["Status"]);
                        objEMReportOutput.Message = new List<string>() { Convert.ToString(resultRow["Message"]) };
                    }
                    else
                    {
                        objEMReportOutput.IsSuccess = false;
                        objEMReportOutput.Message = new List<string>() { "Internal error occurred while saving report in database" };
                    }
                }
            }
            return objEMReportOutput;
        }

        private EMReportOutput SaveReportAttributeConfiguration(EMReport reportInfo, EMCallingInterface callingInterface, EMModule module, DateTime updateTime)
        {
            EMReportOutput objEMReportOutput = new EMReportOutput();
            objEMReportOutput.IsSuccess = true;
            if (reportInfo.Configuration != null && reportInfo.Configuration.AttributeConfiguration != null && reportInfo.Configuration.AttributeConfiguration.Count > 0)
            {
                DataSet dsResult = new DataSet();
                string configurationXML = GetReportAttributeConfigurationXML(reportInfo);
                object objResult = new EMReportingDBManager(mDBCon).SaveReportAttributeConfiguration(reportInfo.Report.Id, configurationXML, reportInfo.UserName, updateTime, module, EMDataType.DataSet);
                if (objResult != null)
                {
                    dsResult = (DataSet)objResult;
                    if (dsResult.Tables.Count > 0 && dsResult.Tables[0] != null)
                    {
                        DataRow resultRow = dsResult.Tables[0].Rows[0];

                        objEMReportOutput.IsSuccess = Convert.ToBoolean(resultRow["Status"]);
                        objEMReportOutput.Message = new List<string>() { Convert.ToString(resultRow["Message"]) };
                    }
                    else
                    {
                        objEMReportOutput.IsSuccess = false;
                        objEMReportOutput.Message = new List<string>() { "Internal error occurred while saving report in database" };
                    }
                }
            }
            return objEMReportOutput;
        }

        private string GetReportAttributeMappingXML(EMReport reportInfo)
        {
            XElement xele = new XElement("mapping", from mapInfo in reportInfo.Mapping.Where(x => x.Mapping != null && x.Mapping.Count > 0)
                                                    from mapping in mapInfo.Mapping
                                                    select new XElement("map", new XAttribute("etid", mapInfo.EntityTypeId),
                                                                                new XAttribute("atid", mapping.AttributeId),
                                                                                new XAttribute("latid", mapping.LookupAttributeId),
                                                                                mapping.ReportAttribute == null || string.IsNullOrEmpty(mapping.ReportAttribute.Name) ? null : new XAttribute("ratname", mapping.ReportAttribute.Name),
                                                                                mapping.ReportAttribute == null || mapping.ReportAttribute.Id <= 0 ? null : new XAttribute("ratid", mapping.ReportAttribute.Id),
                                                                                mapping.ReportAttribute == null || string.IsNullOrEmpty(mapping.ReportAttribute.Description) ? null : new XAttribute("ratdesc", mapping.ReportAttribute.Description),
                                                                                mapping.ReportAttribute == null || string.IsNullOrEmpty(mapping.ReportAttribute.DataType) ? null : new XAttribute("dtyp", mapping.ReportAttribute.DataType)));
            return xele.ToString();
        }

        private string GetReportAttributeConfigurationXML(EMReport reportInfo)
        {
            XElement xele = new XElement("configuration", from confInfo in reportInfo.Configuration.AttributeConfiguration
                                                          select new XElement("attr", new XAttribute("atid", confInfo.AttributeId),
                                                                                      new XAttribute("atname", confInfo.Attribute),
                                                                                      new XAttribute("latid", confInfo.LookupAttributeId),
                                                                                      new XAttribute("do", confInfo.DisplayOrder),
                                                                                      new XAttribute("cw", confInfo.ColumnWidth),
                                                                                      new XAttribute("dp", confInfo.Format)));
            return xele.ToString();
        }

        public EMReportOutput ValidateReport(EMReport reportInfo, EMCallingInterface validationMode)
        {
            throw new NotImplementedException();
        }

        public void SyncReports(ObjectSet inputData, ObjectSet diffData, ObjectSet dbData, string dateFormat, string userName, int moduleId)
        {
            List<string> lstFailedReports = new List<string>();
            Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo = new Dictionary<string, EMReportOutput>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, EMReport> dctReportNamevsInfo = new Dictionary<string, EMReport>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, EMReportOutput> dctPassedReportNamevsOutputInfo = new Dictionary<string, EMReportOutput>(StringComparer.OrdinalIgnoreCase);

            dctFailedReportNamevsOutputInfo = GetAllFailedReports(diffData);
            dctReportNamevsInfo = ValidateAndPopulateReportInfo(inputData, diffData, dbData, dateFormat, userName, (EMModule)moduleId, dctFailedReportNamevsOutputInfo);

            PopulateReportOutputStatus(diffData, dctPassedReportNamevsOutputInfo, dctFailedReportNamevsOutputInfo, true);

            foreach (KeyValuePair<string, EMReport> reportInfo in dctReportNamevsInfo)
            {
                EMReportOutput output = SaveReport(reportInfo.Value, EMCallingInterface.Sync, (EMModule)moduleId);
                if (output.IsSuccess)
                {
                    dctPassedReportNamevsOutputInfo.Add(reportInfo.Key, output);
                }
                else
                {
                    dctFailedReportNamevsOutputInfo.Add(reportInfo.Key, output);
                }
            }
            PopulateReportOutputStatus(diffData, dctPassedReportNamevsOutputInfo, dctFailedReportNamevsOutputInfo, false);
        }

        private void PopulateReportOutputStatus(ObjectSet diffData, Dictionary<string, EMReportOutput> dctPassedReportNamevsOutputInfo, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, bool preSave)
        {
            PopulateReportOutputStatusForTable(diffData, dctPassedReportNamevsOutputInfo, dctFailedReportNamevsOutputInfo, EMReportingConstants.REPORT_SETUP, preSave);
            PopulateReportOutputStatusForTable(diffData, dctPassedReportNamevsOutputInfo, dctFailedReportNamevsOutputInfo, EMReportingConstants.ATTRIBUTE_MAPPING, preSave);
            PopulateReportOutputStatusForTable(diffData, dctPassedReportNamevsOutputInfo, dctFailedReportNamevsOutputInfo, EMReportingConstants.REPORT_RULES, preSave);
            PopulateReportOutputStatusForTable(diffData, dctPassedReportNamevsOutputInfo, dctFailedReportNamevsOutputInfo, EMReportingConstants.REPORT_CONFIGURATION, preSave);
            PopulateReportOutputStatusForTable(diffData, dctPassedReportNamevsOutputInfo, dctFailedReportNamevsOutputInfo, EMReportingConstants.REPORT_ATTRIBUTE_ORDER, preSave);
        }

        private void PopulateReportOutputStatusForTable(ObjectSet diffData, Dictionary<string, EMReportOutput> dctPassedReportNamevsOutputInfo, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, string tableName, bool preSave)
        {
            if (diffData.Tables[tableName] != null && diffData.Tables[tableName].Rows.Count > 0)
            {
                string reportName = string.Empty;
                foreach (ObjectRow row in diffData.Tables[tableName].Rows)
                {
                    reportName = Convert.ToString(row["Report Name"]);
                    if (dctPassedReportNamevsOutputInfo.ContainsKey(reportName))
                    {
                        row["Status"] = RMCommonConstants.MIGRATION_SUCCESS;
                    }
                    else if (dctFailedReportNamevsOutputInfo.ContainsKey(reportName) && string.IsNullOrEmpty(Convert.ToString(row["Status"])))
                    {

                        if (string.IsNullOrEmpty(Convert.ToString(row["Status"])))
                        {
                            if (preSave)
                            {
                                row["Status"] = RMCommonConstants.MIGRATION_NOT_PROCESSED;
                            }
                            else
                            {
                                row["Status"] = RMCommonConstants.MIGRATION_FAILED;
                                row["Remarks"] = string.Join(Environment.NewLine, dctFailedReportNamevsOutputInfo[reportName].Message);
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(row["Status"])) && !preSave)
                            row["Status"] = RMCommonConstants.MIGRATION_NOT_PROCESSED;
                        //row["Remarks"] = "No difference in report";
                    }
                }
            }
        }

        private Dictionary<string, EMReportOutput> GetAllFailedReports(ObjectSet diffData)
        {
            Dictionary<string, EMReportOutput> dctFailedReports = new Dictionary<string, EMReportOutput>(StringComparer.OrdinalIgnoreCase);
            AddFailedReports(diffData.Tables[EMReportingConstants.REPORT_SETUP], dctFailedReports);
            AddFailedReports(diffData.Tables[EMReportingConstants.ATTRIBUTE_MAPPING], dctFailedReports);
            AddFailedReports(diffData.Tables[EMReportingConstants.REPORT_CONFIGURATION], dctFailedReports);
            AddFailedReports(diffData.Tables[EMReportingConstants.REPORT_RULES], dctFailedReports);
            AddFailedReports(diffData.Tables[EMReportingConstants.REPORT_ATTRIBUTE_ORDER], dctFailedReports);
            return dctFailedReports;
        }

        private void AddFailedReports(ObjectTable reportData, Dictionary<string, EMReportOutput> dctFailedReports)
        {
            if (reportData != null && reportData.Rows.Count > 0)
            {
                var failedReports = reportData.AsEnumerable().Where(x => !string.IsNullOrEmpty(x.Field<string>("Status")) && x.Field<string>("Status").SRMEqualWithIgnoreCase("Failed"));
                if (failedReports != null && failedReports.Count() > 0)
                {
                    foreach (ObjectRow row in failedReports)
                    {
                        if (!dctFailedReports.ContainsKey(Convert.ToString(row["Report Name"])))
                            dctFailedReports.Add(Convert.ToString(row["Report Name"]), new EMReportOutput() { IsSuccess = false, Message = new List<string>() });
                        dctFailedReports[Convert.ToString(row["Report Name"])].Message.Add(Convert.ToString(row["Remarks"]));
                    }
                }
            }
        }

        private Dictionary<string, EMReport> ValidateAndPopulateReportInfo(ObjectSet inputData, ObjectSet diffData, ObjectSet dbData, string dateFormat, string userName, EMModule moduleId, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo)
        {
            Dictionary<string, EMReport> dctReportNamevsInfo = new Dictionary<string, EMReport>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, Dictionary<string, List<EMReportAttributeMappingInfo>>> dctReportNamevsETvsMapping = new Dictionary<string, Dictionary<string, List<EMReportAttributeMappingInfo>>>(StringComparer.OrdinalIgnoreCase);
            EMReport reportInfo = null;
            Dictionary<string, EMReportRepositoryInfo> dctRepositoryVsInfo = new Dictionary<string, EMReportRepositoryInfo>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, RMEntityDetailsInfo> dctModuleEntityTypeInfo = new RMModelerController().GetEntityTypeDetails((int)moduleId);
            Dictionary<string, RMEntityDetailsInfo> dctEntityTypeInfo = new RMModelerController().GetEntityTypeDetails(0);
            Dictionary<string, SecurityTypeMasterInfo> dctSecurityTypeInfo = new RMSectypeController().GetSectypeAttributes(false);
            Dictionary<string, int> dctCalendarNameVsId = SRMCommonRAD.GetCalendarNameVsId();
            Dictionary<string, List<EMReportMappingInfo>> dctReportVsAttributeMapping = new Dictionary<string, List<EMReportMappingInfo>>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, List<string>> dctReportNameVsET = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            #region Report Setup
            if (diffData != null)
            {

                foreach (KeyValuePair<string, RMEntityDetailsInfo> etInfo in dctModuleEntityTypeInfo)
                {
                    if (etInfo.Value.Legs != null)
                    {
                        foreach (KeyValuePair<string, RMEntityDetailsInfo> legInfo in etInfo.Value.Legs)
                        {
                            if (legInfo.Value.Attributes != null)
                            {
                                foreach (KeyValuePair<string, RMEntityAttributeInfo> attrInfo in legInfo.Value.Attributes)
                                {
                                    etInfo.Value.Attributes.Add(attrInfo.Key, attrInfo.Value);
                                    etInfo.Value.Attributes[attrInfo.Key].EntityTypeID = etInfo.Value.EntityTypeID;
                                }
                            }
                        }
                    }
                }

                ValidateAndPopulateReportSetup(diffData, dbData, userName, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, dctRepositoryVsInfo, dctModuleEntityTypeInfo, dctReportNameVsET, ref reportInfo);
                #endregion

                #region Attribute Mapping
                if (diffData.Tables.Contains(EMReportingConstants.ATTRIBUTE_MAPPING) && diffData.Tables[EMReportingConstants.ATTRIBUTE_MAPPING] != null && diffData.Tables[EMReportingConstants.ATTRIBUTE_MAPPING].Rows.Count > 0)
                    ValidateAndPopulateReportAttributeMapping(diffData, dbData, userName, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, dctReportNamevsETvsMapping, ref reportInfo, dctModuleEntityTypeInfo, dctEntityTypeInfo, dctSecurityTypeInfo, dctReportNameVsET);
                #endregion

                #region Report Configuration
                if (diffData.Tables.Contains(EMReportingConstants.REPORT_CONFIGURATION) && diffData.Tables[EMReportingConstants.REPORT_CONFIGURATION] != null && diffData.Tables[EMReportingConstants.REPORT_CONFIGURATION].Rows.Count > 0)
                    ValidateAndPopulateReportConfiguration(diffData, dbData, userName, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, ref reportInfo, dctCalendarNameVsId, dateFormat);
                #endregion

                #region Report Rules
                if (diffData.Tables.Contains(EMReportingConstants.REPORT_RULES) && diffData.Tables[EMReportingConstants.REPORT_RULES] != null && diffData.Tables[EMReportingConstants.REPORT_RULES].Rows.Count > 0)
                    ValidateAndPopulateReportRules(diffData, dbData, userName, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, ref reportInfo);
                #endregion

                #region Attribute Order
                if (diffData.Tables.Contains(EMReportingConstants.REPORT_ATTRIBUTE_ORDER) && diffData.Tables[EMReportingConstants.REPORT_ATTRIBUTE_ORDER] != null && diffData.Tables[EMReportingConstants.REPORT_ATTRIBUTE_ORDER].Rows.Count > 0)
                    ValidateAndPopulateReportAttributeOrder(diffData, dbData, userName, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, dctReportNamevsETvsMapping, ref reportInfo);
                #endregion
            }
            return dctReportNamevsInfo;
        }

        private void ValidateAndPopulateReportAttributeMapping(ObjectSet diffData, ObjectSet dbData, string userName, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, Dictionary<string, Dictionary<string, List<EMReportAttributeMappingInfo>>> dctReportNamevsETvsMapping, ref EMReport reportInfo, Dictionary<string, RMEntityDetailsInfo> dctModuleEntityTypeInfo, Dictionary<string, RMEntityDetailsInfo> dctEntityTypeInfo, Dictionary<string, SecurityTypeMasterInfo> dctSecurityTypeInfo, Dictionary<string, List<string>> dctReportNameVsET)
        {
            try
            {
                bool isValidRow;
                bool isAcrossETReport = false;
                EMReportAttributeMappingInfo attrMappingInfo = null;
                string attributeName = string.Empty;
                foreach (ObjectRow row in diffData.Tables[EMReportingConstants.ATTRIBUTE_MAPPING].Rows)
                {
                    string entityType = string.Empty;

                    isValidRow = IsValidRow(row, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo);
                    if (isValidRow)
                    {
                        reportInfo = SetReportInfo(dctReportNamevsInfo, reportInfo, row, userName, dbData.Tables[EMReportingConstants.REPORT_SETUP], false, dctFailedReportNamevsOutputInfo);
                        if (reportInfo == null)
                        {
                            isValidRow = false;
                        }
                    }
                    if (isValidRow)
                    {
                        entityType = Convert.ToString(row["Entity Type"]);
                        if (reportInfo.Report.Type.SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_REPORT.GetDescription()) || reportInfo.Report.Type.SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_AUDIT_REPORT.GetDescription()))
                        {
                            isAcrossETReport = true;
                        }
                        else
                        {
                            isAcrossETReport = false;
                        }
                        isValidRow = ValidateAndPopulateEntityTypeInfoForReportAttributeMapping(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, dctReportNamevsETvsMapping, reportInfo, dctModuleEntityTypeInfo, dctReportNameVsET, isValidRow, isAcrossETReport, row, entityType);
                    }
                    if (isValidRow)
                    {
                        attributeName = Convert.ToString(row["Attribute Name"]);
                        attrMappingInfo = new EMReportAttributeMappingInfo();
                        isValidRow = ValidateAndPopulateAttributeInfoForReportAttributeMapping(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, dctModuleEntityTypeInfo, isValidRow, attrMappingInfo, attributeName, row, entityType);
                    }
                    if (isValidRow)
                    {
                        isValidRow = ValidateAndPopulateLookupAttributeInfoForReportAttributeMapping(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, dctModuleEntityTypeInfo, dctEntityTypeInfo, dctSecurityTypeInfo, isValidRow, attrMappingInfo, attributeName, row, entityType);
                    }
                    if (isValidRow)
                    {
                        isValidRow = ValidateAndPopulateReportAttributeInfoForReportAttributeMapping(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, isValidRow, isAcrossETReport, attrMappingInfo, row);
                    }
                    if (isValidRow)
                    {
                        dctReportNamevsETvsMapping[reportInfo.Report.Name][Convert.ToString(row["Entity Type"])].Add(attrMappingInfo);
                    }
                    else
                    {
                        if (dctReportNamevsETvsMapping.ContainsKey(Convert.ToString(row["Report Name"])))
                        {
                            dctReportNamevsETvsMapping.Remove(reportInfo.Report.Name);
                        }
                    }
                }
                PopulateReportAttributeMappingInfo(diffData, dbData, userName, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, dctReportNamevsETvsMapping, dctModuleEntityTypeInfo);
                PopulateReportVsETVsAttributeMappingForDBReports(dbData, dctReportNamevsETvsMapping, dctModuleEntityTypeInfo, dctEntityTypeInfo, dctSecurityTypeInfo);
            }
            catch (Exception ex)
            {
                mLogger.Error("Error occurred in ValidateAndPopulateReportAttributeMapping - > " + ex.ToString());
                throw;
            }
        }

        private void PopulateReportVsETVsAttributeMappingForDBReports(ObjectSet dbData, Dictionary<string, Dictionary<string, List<EMReportAttributeMappingInfo>>> dctReportNamevsETvsMapping, Dictionary<string, RMEntityDetailsInfo> dctModuleEntityTypeInfo, Dictionary<string, RMEntityDetailsInfo> dctEntityTypeInfo, Dictionary<string, SecurityTypeMasterInfo> dctSecurityTypeInfo)
        {
            bool isAcrossETReport;
            List<string> lstReportsToBeIgnoredFromDBData = dctReportNamevsETvsMapping.Keys.ToList();
            Dictionary<string, string> dctReportvsType = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (dbData != null && dbData.Tables[EMReportingConstants.REPORT_SETUP] != null && dbData.Tables[EMReportingConstants.REPORT_SETUP].Rows.Count > 0)
            {
                dctReportvsType = dbData.Tables[EMReportingConstants.REPORT_SETUP].AsEnumerable().GroupBy(x => x.Field<string>("Report Name")).ToDictionary(x => x.Key, y => y.FirstOrDefault().Field<string>("Report Type"), StringComparer.OrdinalIgnoreCase);
            }

            foreach (ObjectRow row in dbData.Tables[EMReportingConstants.ATTRIBUTE_MAPPING].Rows)
            {
                string reportName = Convert.ToString(row["Report Name"]);
                if (!lstReportsToBeIgnoredFromDBData.Contains(reportName, StringComparer.OrdinalIgnoreCase))
                {
                    if (dctReportvsType[reportName].SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_REPORT.GetDescription()) || dctReportvsType[reportName].SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_AUDIT_REPORT.GetDescription()))
                    {
                        isAcrossETReport = true;
                    }
                    else
                    {
                        isAcrossETReport = false;
                    }
                    if (!dctReportNamevsETvsMapping.ContainsKey(reportName))
                        dctReportNamevsETvsMapping.Add(reportName, new Dictionary<string, List<EMReportAttributeMappingInfo>>(StringComparer.OrdinalIgnoreCase));
                    if (!dctReportNamevsETvsMapping[reportName].ContainsKey(Convert.ToString(row["Entity Type"])))
                        dctReportNamevsETvsMapping[reportName].Add(Convert.ToString(row["Entity Type"]), new List<EMReportAttributeMappingInfo>());
                    EMReportAttributeMappingInfo mappingInfo = new EMReportAttributeMappingInfo();
                    mappingInfo.Attribute = Convert.ToString(row["Attribute Name"]);
                    mappingInfo.AttributeId = Convert.ToInt32(row["dependent_attribute_id"]);
                    mappingInfo.DataType = dctModuleEntityTypeInfo[Convert.ToString(row["Entity Type"])].Attributes[mappingInfo.Attribute].DataType.ToString();

                    if (!string.IsNullOrEmpty(Convert.ToString(row["Lookup Attribute"])))
                    {
                        mappingInfo.LookupAttribute = Convert.ToString(row["Lookup Attribute"]);
                        if (mappingInfo.LookupAttribute.Equals("Entity Code", StringComparison.OrdinalIgnoreCase))
                        {
                            mappingInfo.LookupDataType = "VARCHAR";
                        }
                        else
                        {
                            if (mappingInfo.DataType.Equals("LOOKUP", StringComparison.OrdinalIgnoreCase))
                            {
                                mappingInfo.LookupDataType = dctEntityTypeInfo[dctModuleEntityTypeInfo[Convert.ToString(row["Entity Type"])].Attributes[mappingInfo.Attribute].LookupEntityTypeName].Attributes[mappingInfo.LookupAttribute].DataType.ToString();
                            }
                            //else if (mappingInfo.DataType.Equals("SECURITY_LOOKUP", StringComparison.OrdinalIgnoreCase))
                            //{
                            //    mappingInfo.LookupDataType = dctSecurityTypeInfo[dctModuleEntityTypeInfo[Convert.ToString(row["Entity Type"])].Attributes[mappingInfo.Attribute].LookupEntityTypeName].AttributeInfo.MasterAttrs[mappingInfo.LookupAttribute].AttrDataType;
                            //}
                        }
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(row["lookup_attribute_id"])))
                    {
                        mappingInfo.LookupAttributeId = Convert.ToInt32(row["lookup_attribute_id"]);
                    }
                    if (isAcrossETReport)
                    {
                        mappingInfo.ReportAttribute = new EMReportAttributeInfo();
                        mappingInfo.ReportAttribute.DataType = Convert.ToString(row["Data Type"]);
                        mappingInfo.ReportAttribute.Name = Convert.ToString(row["Report Attribute Name"]);
                        mappingInfo.ReportAttribute.Description = Convert.ToString(row["Report Attribute Description"]);
                        mappingInfo.ReportAttribute.Id = Convert.ToInt32(row["report_attribute_id"]);
                    }
                    dctReportNamevsETvsMapping[reportName][Convert.ToString(row["Entity Type"])].Add(mappingInfo);
                }
            }

            //return isAcrossETReport;
        }

        private void PopulateReportAttributeMappingInfo(ObjectSet diffData, ObjectSet dbData, string userName, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, Dictionary<string, Dictionary<string, List<EMReportAttributeMappingInfo>>> dctReportNamevsETvsMapping, Dictionary<string, RMEntityDetailsInfo> dctModuleEntityTypeInfo)
        {
            EMReport reportInfo = new EMReport();
            foreach (KeyValuePair<string, Dictionary<string, List<EMReportAttributeMappingInfo>>> rInfo in dctReportNamevsETvsMapping)
            {
                ObjectRow row = diffData.Tables[EMReportingConstants.ATTRIBUTE_MAPPING].NewRow();
                row["Report Name"] = rInfo.Key;
                reportInfo = SetReportInfo(dctReportNamevsInfo, reportInfo, row, userName, dbData.Tables[EMReportingConstants.REPORT_SETUP], false, dctFailedReportNamevsOutputInfo);
                if (reportInfo.Mapping == null)
                    reportInfo.Mapping = new List<EMReportMappingInfo>();

                foreach (KeyValuePair<string, List<EMReportAttributeMappingInfo>> etInfo in rInfo.Value)
                {
                    if (reportInfo.Mapping.Any(x => x.EntityType.Equals(etInfo.Key, StringComparison.OrdinalIgnoreCase)))
                    {
                        reportInfo.Mapping.Where(x => x.EntityType.Equals(etInfo.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Mapping.AddRange(etInfo.Value);
                    }
                    else
                    {
                        EMReportMappingInfo mappingInfo = new EMReportMappingInfo();
                        mappingInfo.EntityType = etInfo.Key;
                        mappingInfo.EntityTypeId = dctModuleEntityTypeInfo[etInfo.Key].EntityTypeID;
                        mappingInfo.Mapping = new List<EMReportAttributeMappingInfo>();
                        mappingInfo.Mapping.AddRange(etInfo.Value);
                        reportInfo.Mapping.Add(mappingInfo);
                    }
                }
            }
            //return reportInfo;
        }

        private bool ValidateAndPopulateReportAttributeInfoForReportAttributeMapping(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, bool isValidRow, bool isAcrossETReport, EMReportAttributeMappingInfo mappingInfo, ObjectRow row)
        {
            string errorMsg = string.Empty;

            if (isAcrossETReport)
            {
                List<string> lstRestrictedReportAttributeNames = new List<string>() { "Entity Type", "entity_type" };

                mappingInfo.ReportAttribute = new EMReportAttributeInfo();
                mappingInfo.ReportAttribute.DataType = Convert.ToString(row["Data Type"]);
                mappingInfo.ReportAttribute.Name = Convert.ToString(row["Report Attribute Name"]);
                mappingInfo.ReportAttribute.Description = Convert.ToString(row["Report Attribute Description"]);
                if (!string.IsNullOrEmpty(mappingInfo.ReportAttribute.Name) && lstRestrictedReportAttributeNames.Contains(mappingInfo.ReportAttribute.Name, StringComparer.OrdinalIgnoreCase))
                {
                    errorMsg = mappingInfo.ReportAttribute.Name + " is a restricted report attribute name";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }
                else if (reportInfo.Report.IsLegacy)
                {
                    if (!mappingInfo.ReportAttribute.DataType.Equals(mappingInfo.DataType, StringComparison.OrdinalIgnoreCase))
                    {
                        errorMsg = "Datatype mismatch between report attribute and mapped attribute";
                        MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                        isValidRow = false;
                    }
                }
                else
                {
                    if (mappingInfo.DataType.Equals("LOOKUP", StringComparison.OrdinalIgnoreCase))// || mappingInfo.DataType.Equals("SECURITY_LOOKUP", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!mappingInfo.ReportAttribute.DataType.Equals(mappingInfo.LookupDataType, StringComparison.OrdinalIgnoreCase))
                        {
                            errorMsg = "Datatype mismatch between report attribute and mapped attribute";
                            MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                            isValidRow = false;
                        }
                    }
                    else
                    {
                        if (!mappingInfo.ReportAttribute.DataType.Equals(mappingInfo.DataType, StringComparison.OrdinalIgnoreCase))
                        {
                            errorMsg = "Datatype mismatch between report attribute and mapped attribute";
                            MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                            isValidRow = false;
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Convert.ToString(row["Report Attribute Name"])))
                {
                    errorMsg = "Report attribute is valid only for across entity type reports";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }

                if (!string.IsNullOrEmpty(Convert.ToString(row["Report Attribute Description"])))
                {
                    errorMsg = "Report attribute description is valid only for across entity type reports";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }

                if (!string.IsNullOrEmpty(Convert.ToString(row["Data Type"])))
                {
                    errorMsg = "Datatype is valid only for across entity type reports";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }
            }

            return isValidRow;
        }

        private bool ValidateAndPopulateLookupAttributeInfoForReportAttributeMapping(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, Dictionary<string, RMEntityDetailsInfo> dctModuleEntityTypeInfo, Dictionary<string, RMEntityDetailsInfo> dctEntityTypeInfo, Dictionary<string, SecurityTypeMasterInfo> dctSecurityTypeInfo, bool isValidRow, EMReportAttributeMappingInfo mappingInfo, string attributeName, ObjectRow row, string entityType)
        {
            string errorMsg = string.Empty;
            if (!string.IsNullOrEmpty(Convert.ToString(row["Lookup Attribute"])))
            {
                if (reportInfo.Report.IsLegacy)
                {
                    errorMsg = "Lookup attribute is not valid for legacy report";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }
                else
                {
                    if (mappingInfo.DataType.ToUpper().Equals("LOOKUP"))
                    {
                        if (dctEntityTypeInfo[dctModuleEntityTypeInfo[entityType].Attributes[attributeName].LookupEntityTypeName].Attributes != null && dctEntityTypeInfo[dctModuleEntityTypeInfo[entityType].Attributes[attributeName].LookupEntityTypeName].Attributes.ContainsKey(Convert.ToString(row["Lookup Attribute"])))
                        {
                            mappingInfo.LookupAttribute = Convert.ToString(row["Lookup Attribute"]);
                            mappingInfo.LookupAttributeId = dctEntityTypeInfo[dctModuleEntityTypeInfo[entityType].Attributes[attributeName].LookupEntityTypeName].Attributes[Convert.ToString(row["Lookup Attribute"])].EntityAttributeID;
                            mappingInfo.LookupDataType = dctEntityTypeInfo[dctModuleEntityTypeInfo[entityType].Attributes[attributeName].LookupEntityTypeName].Attributes[Convert.ToString(row["Lookup Attribute"])].DataType.ToString();
                        }
                        else if (Convert.ToString(row["Lookup Attribute"]).Equals("Entity Code", StringComparison.OrdinalIgnoreCase))
                        {
                            mappingInfo.LookupAttribute = Convert.ToString(row["Lookup Attribute"]);
                            mappingInfo.LookupDataType = "VARCHAR";
                        }
                        else
                        {
                            errorMsg = "Lookup attribute '" + Convert.ToString(row["Lookup Attribute"]) + "' does not exist in parent entity type";
                            MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                            isValidRow = false;
                        }
                    }
                    //else if (mappingInfo.DataType.ToUpper().Equals("SECURITY_LOOKUP"))
                    //{
                    //    if (dctSecurityTypeInfo[dctModuleEntityTypeInfo[entityType].Attributes[attributeName].LookupEntityTypeName] != null &&
                    //        dctSecurityTypeInfo[dctModuleEntityTypeInfo[entityType].Attributes[attributeName].LookupEntityTypeName].AttributeInfo != null &&
                    //        dctSecurityTypeInfo[dctModuleEntityTypeInfo[entityType].Attributes[attributeName].LookupEntityTypeName].AttributeInfo.MasterAttrs != null
                    //        && dctSecurityTypeInfo[dctModuleEntityTypeInfo[entityType].Attributes[attributeName].LookupEntityTypeName].AttributeInfo.MasterAttrs.ContainsKey(Convert.ToString(row["Lookup Attribute"])))
                    //    {
                    //        mappingInfo.LookupAttribute = Convert.ToString(row["Lookup Attribute"]);
                    //        mappingInfo.LookupAttributeId = dctSecurityTypeInfo[dctModuleEntityTypeInfo[entityType].Attributes[attributeName].LookupEntityTypeName].AttributeInfo.MasterAttrs[Convert.ToString(row["Lookup Attribute"])].AttrId;
                    //        mappingInfo.LookupDataType = dctSecurityTypeInfo[dctModuleEntityTypeInfo[entityType].Attributes[attributeName].LookupEntityTypeName].AttributeInfo.MasterAttrs[Convert.ToString(row["Lookup Attribute"])].AttrDataType;
                    //    }
                    //    else if (Convert.ToString(row["Lookup Attribute"]).Equals("Security Id", StringComparison.OrdinalIgnoreCase))
                    //    {
                    //        mappingInfo.LookupAttribute = Convert.ToString(row["Lookup Attribute"]);
                    //        mappingInfo.LookupDataType = "VARCHAR";
                    //    }
                    //    else
                    //    {
                    //        errorMsg = "Lookup attribute '" + Convert.ToString(row["Lookup Attribute"]) + "' does not exist in parent security type";
                    //        MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    //        isValidRow = false;
                    //    }
                    //}
                    else
                    {
                        errorMsg = "Lookup attribute is not valid for a non lookup attribute";
                        MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                        isValidRow = false;
                    }
                }
            }

            return isValidRow;
        }

        private bool ValidateAndPopulateAttributeInfoForReportAttributeMapping(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, Dictionary<string, RMEntityDetailsInfo> dctModuleEntityTypeInfo, bool isValidRow, EMReportAttributeMappingInfo mappingInfo, string attributeName, ObjectRow row, string entityType)
        {
            string errorMsg = string.Empty;
            if (dctModuleEntityTypeInfo[entityType].Attributes.ContainsKey(attributeName))
            {
                mappingInfo.Attribute = attributeName;
                mappingInfo.AttributeId = Convert.ToInt32(dctModuleEntityTypeInfo[entityType].Attributes[attributeName].EntityAttributeID);
                mappingInfo.DataType = dctModuleEntityTypeInfo[entityType].Attributes[attributeName].DataType.ToString();
            }
            else
            {
                errorMsg = "Attribute does not exist in entity type";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }

            return isValidRow;
        }

        private bool ValidateAndPopulateEntityTypeInfoForReportAttributeMapping(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, Dictionary<string, Dictionary<string, List<EMReportAttributeMappingInfo>>> dctReportNamevsETvsMapping, EMReport reportInfo, Dictionary<string, RMEntityDetailsInfo> dctModuleEntityTypeInfo, Dictionary<string, List<string>> dctReportNameVsET, bool isValidRow, bool isAcrossETReport, ObjectRow row, string entityType)
        {
            string errorMsg = string.Empty;
            if (reportInfo.Report.Type.SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_REPORT.GetDescription()) || reportInfo.Report.Type.SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_AUDIT_REPORT.GetDescription()))
            {
                isAcrossETReport = true;
            }
            else
            {
                isAcrossETReport = false;
            }
            if (!dctReportNamevsETvsMapping.ContainsKey(reportInfo.Report.Name))
                dctReportNamevsETvsMapping.Add(reportInfo.Report.Name, new Dictionary<string, List<EMReportAttributeMappingInfo>>(StringComparer.OrdinalIgnoreCase));

            if (dctReportNameVsET[reportInfo.Report.Name].Contains(entityType, StringComparer.OrdinalIgnoreCase))
            {
                if (!dctReportNamevsETvsMapping[reportInfo.Report.Name].ContainsKey(entityType))
                    dctReportNamevsETvsMapping[reportInfo.Report.Name].Add(entityType, new List<EMReportAttributeMappingInfo>());
            }
            else
            {
                if (dctModuleEntityTypeInfo.ContainsKey(entityType))
                {
                    errorMsg = "Entity Type not mapped with this report";
                }
                else
                {
                    errorMsg = "Entity Type does not exist";
                }
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }

            return isValidRow;
        }

        private void ValidateAndPopulateReportAttributeOrder(ObjectSet diffData, ObjectSet dbData, string userName, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, Dictionary<string, Dictionary<string, List<EMReportAttributeMappingInfo>>> dctReportNamevsETvsMapping, ref EMReport reportInfo)
        {
            bool isValidRow;
            Dictionary<string, Dictionary<string, List<string>>> reportNameVsAttributeNameVsLookupAttribute = new Dictionary<string, Dictionary<string, List<string>>>();
            foreach (ObjectRow row in diffData.Tables[EMReportingConstants.REPORT_ATTRIBUTE_ORDER].Rows)
            {
                EMReportAttributeConfigurationInfo attrInfo = new EMReportAttributeConfigurationInfo();
                string reportName = Convert.ToString(row["Report Name"]);
                string attrName = Convert.ToString(row["Attribute Name"]);
                string lookupAttribute = Convert.ToString(row["Lookup Attribute"]);
                string errorMsg = string.Empty;
                bool isAcrossETReport = false;
                isValidRow = IsValidRow(row, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo);
                if (isValidRow)
                {
                    reportInfo = SetReportInfo(dctReportNamevsInfo, reportInfo, row, userName, dbData.Tables[EMReportingConstants.REPORT_SETUP], false, dctFailedReportNamevsOutputInfo);
                    if (reportInfo == null)
                    {
                        isValidRow = false;
                    }
                }
                if (isValidRow)
                {
                    if (reportInfo.Configuration == null)
                    {
                        reportInfo.Configuration = new EMReportConfigurationInfo();
                    }
                    if (reportInfo.Configuration.AttributeConfiguration == null)
                    {
                        reportInfo.Configuration.AttributeConfiguration = new List<EMReportAttributeConfigurationInfo>();
                    }
                    if (reportInfo.Report.Type.SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_REPORT.GetDescription()) || reportInfo.Report.Type.SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_AUDIT_REPORT.GetDescription()))
                    {
                        isAcrossETReport = true;
                    }
                    isValidRow = ValidateAndPopulateAttributeInfoForReportAttributeOrder(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, dctReportNamevsETvsMapping, reportInfo, row, attrInfo, reportName, attrName, isAcrossETReport, reportNameVsAttributeNameVsLookupAttribute);
                }
                if (isValidRow)
                {
                    isValidRow = ValidateAndPopulateLookupAttributeInfoForReportAttributeOrder(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, dctReportNamevsETvsMapping, reportInfo, row, attrInfo, reportName, attrName, lookupAttribute, isAcrossETReport);
                }
                if (isValidRow)
                {
                    isValidRow = ValidateAndPopulateDisplayOrderForReportAttributes(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, row, attrInfo);
                }
                if (isValidRow)
                {
                    isValidRow = ValidateAndPopulateColumnWidthForReportAttributes(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, attrInfo);
                }
                if (isValidRow)
                {
                    isValidRow = ValidateAndPopulateDecimalPlacesForReportAttributes(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, dctReportNamevsETvsMapping, row, attrInfo, reportName, attrName, lookupAttribute, isAcrossETReport);
                }
                if (isValidRow)
                {
                    reportInfo.Configuration.AttributeConfiguration.Add(attrInfo);
                }
            }
        }

        private bool ValidateAndPopulateDecimalPlacesForReportAttributes(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, Dictionary<string, Dictionary<string, List<EMReportAttributeMappingInfo>>> dctReportNamevsETvsMapping, ObjectRow row, EMReportAttributeConfigurationInfo attrInfo, string reportName, string attrName, string lookupAttribute, bool isAcrossETReport)
        {
            bool isValidRow;
            string errorMsg = string.Empty;
            int decimalPlaces = 0;
            bool isDecimal = false;
            attrInfo.Format = Convert.ToString(row["Decimal Places"]);
            if (isAcrossETReport)
            {
                if (dctReportNamevsETvsMapping[reportName].Any(x => x.Value.Any(y => y.ReportAttribute.Name.SRMEqualWithIgnoreCase(attrName) && y.DataType.SRMEqualWithIgnoreCase("DECIMAL"))))
                {
                    isDecimal = true;
                }
            }
            else
            {
                if (dctReportNamevsETvsMapping[reportName].Any(x => x.Value.Any(y => y.Attribute.SRMEqualWithIgnoreCase(attrName)
                    && ((string.IsNullOrEmpty(y.LookupAttribute) && string.IsNullOrEmpty(lookupAttribute))
                        || y.LookupAttribute.SRMEqualWithIgnoreCase(lookupAttribute))
                    && (y.DataType.SRMEqualWithIgnoreCase("DECIMAL") || (!string.IsNullOrEmpty(y.LookupDataType) && y.LookupDataType.SRMEqualWithIgnoreCase("DECIMAL"))))))
                {
                    isDecimal = true;
                }
            }

            if (isDecimal)
            {
                if (string.IsNullOrEmpty(attrInfo.Format))
                {
                    errorMsg = "Decimal places not provided for Decimal attribute";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }
                else
                {
                    if (int.TryParse(attrInfo.Format, out decimalPlaces))
                    {
                        if (decimalPlaces < 0 || decimalPlaces > 28)
                        {
                            errorMsg = "Decimal places should be between 0 and 28";
                            MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                            isValidRow = false;
                        }
                        else
                        {
                            isValidRow = true;
                        }
                    }
                    else
                    {
                        errorMsg = "Decimal places should be integer";
                        MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                        isValidRow = false;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(attrInfo.Format))
                {
                    isValidRow = true;
                }
                else
                {
                    errorMsg = "Decimal places is not valid for non decimal attribute";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }
            }
            return isValidRow;
        }

        private bool ValidateAndPopulateAttributeInfoForReportAttributeOrder(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, Dictionary<string, Dictionary<string, List<EMReportAttributeMappingInfo>>> dctReportNamevsETvsMapping, EMReport reportInfo, ObjectRow row, EMReportAttributeConfigurationInfo attrInfo, string reportName, string attrName, bool isAcrossETReport, Dictionary<string, Dictionary<string, List<string>>> reportNameVsAttributeNameVsLookupAttribute)
        {
            string errorMsg = string.Empty;
            bool isValidRow;
            int attributeId = 0;
            EMReportAttributeMappingInfo attrMapInfo = new EMReportAttributeMappingInfo();
            if (dctReportNamevsETvsMapping != null && dctReportNamevsETvsMapping.ContainsKey(reportName))
            {
                if (isAcrossETReport)
                {
                    if (dctReportNamevsETvsMapping[reportName].Any(y => y.Value.Any(z => z.ReportAttribute.Name.SRMEqualWithIgnoreCase(attrName))))
                    {
                        foreach (KeyValuePair<string, List<EMReportAttributeMappingInfo>> info in dctReportNamevsETvsMapping[reportName])
                        {
                            var result = info.Value.Where(z => z.ReportAttribute.Name.SRMEqualWithIgnoreCase(attrName));
                            if (result != null && result.Count() > 0)
                            {
                                attributeId = result.FirstOrDefault().ReportAttribute.Id;
                                break;
                            }
                        }
                        //attributeId = dctReportNamevsETvsMapping[reportName].Select(y => y.Value.Where(z => z.ReportAttribute.Name.SRMEqualWithIgnoreCase(attrName)).FirstOrDefault().ReportAttribute.Id).FirstOrDefault();
                        isValidRow = true;
                    }
                    else
                    {
                        isValidRow = false;
                    }
                }
                else
                {
                    if (dctReportNamevsETvsMapping[reportName].Any(y => y.Value.Any(z => z.Attribute.SRMEqualWithIgnoreCase(attrName))))
                    {
                        attributeId = dctReportNamevsETvsMapping[reportName].Select(y => y.Value.Where(z => z.Attribute.SRMEqualWithIgnoreCase(attrName)).FirstOrDefault().AttributeId).FirstOrDefault();
                        isValidRow = true;
                    }
                    else
                    {
                        isValidRow = false;
                    }
                }
                if (isValidRow)
                {
                    string lookupAttribute = Convert.ToString(row["Lookup Attribute"]);
                    if (!reportNameVsAttributeNameVsLookupAttribute.ContainsKey(reportName))
                        reportNameVsAttributeNameVsLookupAttribute.Add(reportName, new Dictionary<string, List<string>>(){ { attrName, new List<string>() { lookupAttribute } } });
                    else
                    {
                        if (reportNameVsAttributeNameVsLookupAttribute[reportName].ContainsKey(attrName))
                        {
                            if (reportNameVsAttributeNameVsLookupAttribute[reportName][attrName].Contains(lookupAttribute))
                                isValidRow = false;
                            else
                                reportNameVsAttributeNameVsLookupAttribute[reportName][attrName].Add(lookupAttribute);
                        }
                        else
                            reportNameVsAttributeNameVsLookupAttribute[reportName].Add(attrName, new List<string>());                                
                    }

                    if (!isValidRow)
                    {
                        errorMsg = "Attribute exists more than once in the report";
                        MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    }
                }
                if (isValidRow)
                {
                    attrInfo.Attribute = Convert.ToString(row["Attribute Name"]);
                    attrInfo.AttributeId = attributeId;
                }
                else
                {
                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        errorMsg = "Attribute not configured in report";
                        MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    }
                }
            }
            else
            {
                isValidRow = false;
                errorMsg = "Report does not exist";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
            }
            return isValidRow;
        }

        private bool ValidateAndPopulateLookupAttributeInfoForReportAttributeOrder(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, Dictionary<string, Dictionary<string, List<EMReportAttributeMappingInfo>>> dctReportNamevsETvsMapping, EMReport reportInfo, ObjectRow row, EMReportAttributeConfigurationInfo attrInfo, string reportName, string attrName, string lookupAttribute, bool isAcrossETReport)
        {
            bool isValidRow;
            string errorMsg = string.Empty;
            if (reportInfo.Report.IsLegacy || isAcrossETReport)
            {
                if (!string.IsNullOrEmpty(lookupAttribute))
                {
                    if (isAcrossETReport)
                        errorMsg = "Lookup attribute is not valid for Across Entity Type Reports";
                    else
                        errorMsg = "Lookup attribute is not valid for Legacy Reports";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }
                else
                {
                    isValidRow = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lookupAttribute))
                {
                    if (dctReportNamevsETvsMapping[reportName].Any(y => y.Value.Where(z => z.Attribute.SRMEqualWithIgnoreCase(attrName)).Any(x => x.LookupAttribute.SRMEqualWithIgnoreCase(lookupAttribute))))
                    {
                        attrInfo.LookupAttribute = Convert.ToString(row["Lookup Attribute"]);
                        attrInfo.LookupAttributeId = dctReportNamevsETvsMapping[reportName].Select(y => y.Value.Where(z => z.Attribute.SRMEqualWithIgnoreCase(attrName)).Where(x => x.LookupAttribute.SRMEqualWithIgnoreCase(lookupAttribute)).FirstOrDefault().LookupAttributeId).FirstOrDefault();
                        isValidRow = true;
                    }
                    else
                    {
                        errorMsg = "Combination of Attribute and Lookup Attribute is not valid.";
                        MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                        isValidRow = false;
                    }
                }
                else
                {
                    if (dctReportNamevsETvsMapping[reportName].Any(y => y.Value.Where(z => z.Attribute.SRMEqualWithIgnoreCase(attrName)).Any(x => string.IsNullOrEmpty(x.LookupAttribute))))
                        isValidRow = true;
                    else
                    {
                        errorMsg = "Combination of Attribute and Lookup Attribute is not valid.";
                        MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                        isValidRow = false;
                    }
                }
            }
            return isValidRow;
        }

        private bool ValidateAndPopulateColumnWidthForReportAttributes(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, ObjectRow row, EMReportAttributeConfigurationInfo attrInfo)
        {
            string errorMsg = string.Empty;
            bool isValidRow;
            if (Convert.ToInt32(row["Column Width"]) <= 0)
            {
                errorMsg = "Column width cannot be less than 1";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            else
            {
                attrInfo.ColumnWidth = Convert.ToInt32(row["Column Width"]);
                isValidRow = true;
            }

            return isValidRow;
        }

        private bool ValidateAndPopulateDisplayOrderForReportAttributes(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, ObjectRow row, EMReportAttributeConfigurationInfo attrInfo)
        {
            string errorMsg = string.Empty;
            bool isValidRow;
            if (Convert.ToInt32(row["Display Order"]) <= 0)
            {
                errorMsg = "Display Order cannot be less than 1";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            else if (reportInfo.Configuration.AttributeConfiguration.Any(x => x.DisplayOrder == Convert.ToInt32(row["Display Order"])))
            {
                errorMsg = "Another attribute with same display order already provided";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            else
            {
                attrInfo.DisplayOrder = Convert.ToInt32(row["Display Order"]);
                isValidRow = true;
            }

            return isValidRow;
        }

        private void ValidateAndPopulateReportConfiguration(ObjectSet diffData, ObjectSet dbData, string userName, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, ref EMReport reportInfo, Dictionary<string, int> dctCalendarNameVsId, string dateFormat)
        {
            bool isValidRow;
            foreach (ObjectRow row in diffData.Tables[EMReportingConstants.REPORT_CONFIGURATION].Rows)
            {
                string errorMsg = string.Empty;
                isValidRow = IsValidRow(row, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo);
                if (isValidRow)
                {
                    reportInfo = SetReportInfo(dctReportNamevsInfo, reportInfo, row, userName, dbData.Tables[EMReportingConstants.REPORT_SETUP], false, dctFailedReportNamevsOutputInfo);
                    if (reportInfo == null)
                    {
                        isValidRow = false;
                    }
                }
                if (isValidRow)
                {
                    reportInfo.Configuration = new EMReportConfigurationInfo();
                    reportInfo.Configuration.ReportHeader = Convert.ToString(row["Report Header"]);
                    reportInfo.Configuration.IsMultiSheet = Convert.ToBoolean(row["MultiSheet Report"]);
                    isValidRow = ValidateAndPopulateReportCalendarInfo(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, dctCalendarNameVsId, row);
                }
                if (isValidRow)
                {
                    isValidRow = ValidateAndPopulateReportConfigurationDates(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, row, dateFormat);
                }
                if (isValidRow)
                {
                    isValidRow = ValidateAndPopulateIsFromToViewForReportConfiguration(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, row);
                }
                if (isValidRow)
                {
                    isValidRow = ValidateAndPopulateLegacyReportInfo(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, row);
                }
            }
        }

        private bool ValidateAndPopulateReportCalendarInfo(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, Dictionary<string, int> dctCalendarNameVsId, ObjectRow row)
        {
            string errorMsg = string.Empty;
            bool isValidRow;
            reportInfo.Configuration.Calendar = Convert.ToString(row["Calendar"]);
            if (dctCalendarNameVsId.ContainsKey(reportInfo.Configuration.Calendar))
            {
                reportInfo.Configuration.CalendarId = dctCalendarNameVsId[reportInfo.Configuration.Calendar];
                isValidRow = true;
            }
            else
            {
                errorMsg = "Calendar doesn't exist in system";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }

            return isValidRow;
        }

        private bool ValidateAndPopulateReportConfigurationDates(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, ObjectRow row, string dateFormat)
        {
            bool isValidRow;
            string errorMsg = string.Empty;
            EMDateTypeEnum startDateEnum, endDateEnum;
            reportInfo.Configuration.StartDate = new EMDateTypeInfo() { Value = Convert.ToString(row["Start Date"]), CustomValue = Convert.ToString(row["Custom Value Start Date"]), DateFormat = dateFormat };
            reportInfo.Configuration.EndDate = new EMDateTypeInfo() { Value = Convert.ToString(row["End Date"]), CustomValue = Convert.ToString(row["Custom Value End Date"]), DateFormat = dateFormat };

            if (reportInfo.Configuration.StartDate != null && !string.IsNullOrEmpty(reportInfo.Configuration.StartDate.Value) && reportInfo.Configuration.StartDate.Value.SRMEqualWithIgnoreCase("T-n"))
            {
                reportInfo.Configuration.StartDate.Value = "T_Minus_N";
            }
            if (reportInfo.Configuration.EndDate != null && !string.IsNullOrEmpty(reportInfo.Configuration.EndDate.Value) && reportInfo.Configuration.EndDate.Value.SRMEqualWithIgnoreCase("T-n"))
            {
                reportInfo.Configuration.EndDate.Value = "T_Minus_N";
            }

            isValidRow = Enum.TryParse(reportInfo.Configuration.StartDate.Value, true, out startDateEnum);
            if (isValidRow)
            {
                reportInfo.Configuration.StartDate.Id = (int)Enum.Parse(typeof(EMDateTypeEnum), reportInfo.Configuration.StartDate.Value);
                if (!string.IsNullOrEmpty(reportInfo.Configuration.EndDate.Value))
                    isValidRow = Enum.TryParse(reportInfo.Configuration.EndDate.Value, true, out endDateEnum);
            }
            else
            {
                errorMsg = "Invalid Start Date provided";
            }
            if (isValidRow)
            {
                if (!string.IsNullOrEmpty(reportInfo.Configuration.EndDate.Value))
                    reportInfo.Configuration.EndDate.Id = (int)Enum.Parse(typeof(EMDateTypeEnum), reportInfo.Configuration.EndDate.Value);
                errorMsg = ValidateStartAndEndDates(reportInfo.Configuration.StartDate, reportInfo.Configuration.EndDate, reportInfo.Configuration.CalendarId);
            }
            else
            {
                errorMsg = "Invalid End Date provided";
            }
            if (string.IsNullOrEmpty(errorMsg))
            {
                isValidRow = true;
            }
            else
            {
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            return isValidRow;
        }

        private string ValidateStartAndEndDates(EMDateTypeInfo startDate, EMDateTypeInfo endDate, int calendarId)
        {
            EMDateTypeEnum startDateEnum = (EMDateTypeEnum)startDate.Id;
            EMDateTypeEnum endDateEnum = (EMDateTypeEnum)endDate.Id;
            switch (startDateEnum)
            {
                case EMDateTypeEnum.LastBusinessDayOfPreviousMonth_Plus_N:
                case EMDateTypeEnum.LastBusinessDayOfPreviousYear_Plus_N:
                case EMDateTypeEnum.FirstBusinessDayOfMonth_Plus_N:
                case EMDateTypeEnum.FirstBusinessDayOfYear_Plus_N:
                case EMDateTypeEnum.T_Minus_N:
                    int customStartDateTN;
                    if (startDate.CustomValue == null || string.IsNullOrEmpty(startDate.CustomValue))
                    {
                        return "Custom start date cannot be blank for start date " + startDate.Value;
                    }
                    else if (!int.TryParse(startDate.CustomValue, out customStartDateTN))
                    {
                        return "Custom start date should be an integer for start date " + startDate.Value;
                    }
                    break;
                case EMDateTypeEnum.CustomDate:
                    DateTime customStartDate;
                    if (startDate.CustomValue == null || string.IsNullOrEmpty(startDate.CustomValue))
                    {
                        return "Custom start date cannot be blank for start date " + startDate.Value;
                    }
                    else if (!DateTime.TryParseExact(startDate.CustomValue, new string[] { startDate.DateFormat, startDate.DateFormat + " hh:mm:ss tt" }, null, System.Globalization.DateTimeStyles.None, out customStartDate))
                    {
                        return "Custom start date is not in a valid format (" + startDate.DateFormat + ") for start date " + startDate.Value;
                    }
                    else if (customStartDate > customStartDate.Date)
                    {
                        return "Custom start date is not in a valid format (" + startDate.DateFormat + ") for end date " + startDate.Value;
                    }
                    else if (customStartDate.Date > DateTime.Today.Date)
                    {
                        return "Start date cannot be greater than today's date";
                    }
                    else
                    {
                        startDate.DateFormat = "yyyyMMdd";
                        startDate.CustomValue = customStartDate.ToString(startDate.DateFormat);
                    }
                    break;
                case EMDateTypeEnum.Todays:
                    if (endDateEnum != EMDateTypeEnum.CustomDate && endDateEnum != EMDateTypeEnum.Now)
                    {
                        return "End date can only be CustomDate or Now when start date is " + startDate.Value;
                    }
                    break;
                case EMDateTypeEnum.Yesterday:
                    if (endDateEnum != EMDateTypeEnum.Todays && endDateEnum != EMDateTypeEnum.Now)
                    {
                        return "End date can only be Today or Now when start date is " + startDate.Value;
                    }
                    break;
                case EMDateTypeEnum.LastBusinessDay:
                    if (endDateEnum != EMDateTypeEnum.Todays && endDateEnum != EMDateTypeEnum.Now && endDateEnum != EMDateTypeEnum.Yesterday && endDateEnum != EMDateTypeEnum.CustomDate)
                    {
                        return "End date can only be Today, Yesterday, Now or CustomDate when start date is " + startDate.Value;
                    }
                    break;
                case EMDateTypeEnum.Now:
                case EMDateTypeEnum.LastBusinessDayOfMonth:
                case EMDateTypeEnum.LastBusinessDayOfYear:
                    return "Start date cannot be " + startDate.Value;
                case EMDateTypeEnum.FirstBusinessDayOfMonth:
                case EMDateTypeEnum.FirstBusinessDayOfYear:
                    if (endDateEnum == EMDateTypeEnum.Yesterday)
                    {
                        return "End date cannot be Yesterday when start date is " + startDate.Value;
                    }
                    break;
                case EMDateTypeEnum.LastExtractionDate:
                    if (endDate != null && !string.IsNullOrEmpty(endDate.Value))
                    {
                        return "End date cannot be set when start date is " + startDate.Value;
                    }
                    break;
                default:
                    break;
            }
            switch ((EMDateTypeEnum)endDate.Id)
            {
                case EMDateTypeEnum.LastBusinessDayOfPreviousMonth_Plus_N:
                case EMDateTypeEnum.LastBusinessDayOfPreviousYear_Plus_N:
                case EMDateTypeEnum.FirstBusinessDayOfMonth_Plus_N:
                case EMDateTypeEnum.FirstBusinessDayOfYear_Plus_N:
                case EMDateTypeEnum.T_Minus_N:
                    int customEndDateTN;
                    if (endDate.CustomValue == null || string.IsNullOrEmpty(endDate.CustomValue))
                    {
                        return "Custom end date cannot be blank for end date " + endDate.Value;
                    }
                    else if (!int.TryParse(endDate.CustomValue, out customEndDateTN))
                    {
                        return "Custom end date should be an integer for end date " + endDate.Value;
                    }
                    break;
                case EMDateTypeEnum.CustomDate:
                    DateTime customEndDate;
                    if (endDate.CustomValue == null || string.IsNullOrEmpty(endDate.CustomValue))
                    {
                        return "Custom end date cannot be blank for end date " + endDate.Value;
                    }
                    else if (!DateTime.TryParseExact(endDate.CustomValue, new string[] { endDate.DateFormat, endDate.DateFormat + " hh:mm:ss tt" }, null, System.Globalization.DateTimeStyles.None, out customEndDate))
                    {
                        return "Custom end date is not in a valid format (" + endDate.DateFormat + ") for end date " + endDate.Value;
                    }
                    else if (customEndDate > customEndDate.Date)
                    {
                        return "Custom end date is not in a valid format (" + endDate.DateFormat + ") for end date " + endDate.Value;
                    }
                    else if (customEndDate.Date > DateTime.Today.Date)
                    {
                        return "End date cannot be greater than today's date";
                    }
                    else
                    {
                        endDate.DateFormat = "yyyyMMdd";
                        endDate.CustomValue = customEndDate.ToString(endDate.DateFormat);
                    }
                    break;
                case EMDateTypeEnum.None:
                    if (!string.IsNullOrEmpty(endDate.Value))
                        return "End date cannot be None";
                    break;
            }

            DateTime calculatedStartDate = GetReportDate(calendarId, startDate);
            DateTime calculatedEndDate = GetReportDate(calendarId, endDate);
            if (calculatedStartDate > calculatedEndDate && endDateEnum != EMDateTypeEnum.LastExtractionDate)
            {
                return "Incorrect start and end date combination. Start date cannot be greater than end date";
            }
            return string.Empty;
        }

        public DateTime GetReportDate(int calendarID, EMDateTypeInfo dateType)
        {
            RCalendarDateInfo dateInfo = null;
            DateTime dateStart = new DateTime();
            switch ((EMDateTypeEnum)dateType.Id)
            {
                case EMDateTypeEnum.CustomDate:
                    dateStart = DateTime.ParseExact(dateType.CustomValue, dateType.DateFormat, null);
                    break;

                case EMDateTypeEnum.FirstBusinessDayOfMonth:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.FirstBusinessDayOfMonth;
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.FirstBusinessDayOfYear:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.FirstBusinessDayOfYear;
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.LastBusinessDay:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.LastBusinessDay;
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.LastBusinessDayOfMonth:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.LastBusinessDayOfMonth;
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.LastBusinessDayOfYear:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.LastBusinessDayOfYear;
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.None:
                    //dateStart = null;
                    break;
                case EMDateTypeEnum.Now:
                    dateStart = DateTime.Now;
                    break;
                case EMDateTypeEnum.T_Minus_N:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.T_Minus_N;
                    dateInfo.NumberOfDays = Convert.ToInt32(dateType.CustomValue);
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.Todays:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.Today;
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.Yesterday:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.Yesterday;
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.LastBusinessDayOfPreviousMonth_Plus_N:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.LastBusinessDayOfPreviousMonth_Plus_N;
                    dateInfo.NumberOfDays = int.Parse(dateType.CustomValue);
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.LastBusinessDayOfPreviousYear_Plus_N:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.LastBusinessDayOfPreviousYear_Plus_N;
                    dateInfo.NumberOfDays = int.Parse(dateType.CustomValue);
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.FirstBusinessDayOfMonth_Plus_N:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.FirstBusinessDayOfMonth_Plus_N;
                    dateInfo.NumberOfDays = int.Parse(dateType.CustomValue);
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
                case EMDateTypeEnum.FirstBusinessDayOfYear_Plus_N:
                    dateInfo = new RCalendarDateInfo();
                    dateInfo.CalendarID = calendarID;
                    dateInfo.CalendarDateType = RCalendarEnums.FirstBusinessDayOfYear_Plus_N;
                    dateInfo.NumberOfDays = int.Parse(dateType.CustomValue);
                    dateStart = RCalenderUtils.GetDate(dateInfo);
                    break;
            }
            return dateStart;
        }

        private bool ValidateAndPopulateLegacyReportInfo(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, ObjectRow row)
        {
            string errorMsg = string.Empty;
            bool isValidRow;
            if (!reportInfo.Report.IsLegacy)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(row["Show Entity Codes"])))
                {
                    errorMsg = "Show Entity Codes is valid only for legacy reports";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }
                else if (!string.IsNullOrEmpty(Convert.ToString(row["Show Display Names"])))
                {
                    errorMsg = "Show Display Names is valid only for legacy reports";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }
                else
                {
                    isValidRow = true;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Convert.ToString(row["Show Entity Codes"])))
                {
                    errorMsg = "Show Entity Codes is mandatory for legacy reports";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }
                else if (string.IsNullOrEmpty(Convert.ToString(row["Show Display Names"])))
                {
                    errorMsg = "Show Display Names is mandatory for legacy reports";
                    MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                    isValidRow = false;
                }
                else
                {
                    isValidRow = true;
                }
            }
            if (isValidRow)
            {
                if (reportInfo.Report.IsLegacy)
                {
                    reportInfo.Configuration.ShowEntityCodes = Convert.ToBoolean(row["Show Entity Codes"]);
                    reportInfo.Configuration.ShowDisplayNames = Convert.ToBoolean(row["Show Display Names"]);
                }
            }
            return isValidRow;
        }

        private bool ValidateAndPopulateIsFromToViewForReportConfiguration(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, ObjectRow row)
        {
            string errorMsg = string.Empty;
            bool isValidRow;
            if (Convert.ToBoolean(row["Is DWH Extract"]) && !(reportInfo.Report.Type.SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_AUDIT_REPORT.GetDescription()) || reportInfo.Report.Type.SRMEqualWithIgnoreCase(EMReportType.ENTITY_TYPE_AUDIT_REPORT.GetDescription())))
            {
                errorMsg = "Is DWH Extract is valid only for audit reports";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            else
            {
                reportInfo.Configuration.IsFromToView = Convert.ToBoolean(row["Is DWH Extract"]);
                isValidRow = true;
            }
            return isValidRow;
        }

        private void ValidateAndPopulateReportRules(ObjectSet diffData, ObjectSet dbData, string userName, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, ref EMReport reportInfo)
        {
            bool isValidRow;
            string errorMsg = string.Empty;
            foreach (ObjectRow row in diffData.Tables[EMReportingConstants.REPORT_RULES].Rows)
            {
                isValidRow = IsValidRow(row, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo);
                if (isValidRow)
                {
                    reportInfo = SetReportInfo(dctReportNamevsInfo, reportInfo, row, userName, dbData.Tables[EMReportingConstants.REPORT_SETUP], false, dctFailedReportNamevsOutputInfo);
                    EMRuleInfo ruleInfo = new EMRuleInfo();
                    if (reportInfo == null)
                    {
                        isValidRow = false;
                    }
                    if (isValidRow)
                    {
                        isValidRow = ValidateAndPopulateReportRuleName(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, row, ruleInfo);
                    }
                    if (isValidRow)
                    {
                        ruleInfo.Type = Convert.ToString(row["Rule Type"]);
                        ruleInfo.TypeId = (int)Enum.Parse(typeof(EMReportRuleType), ruleInfo.Type);
                        ruleInfo.Text = Convert.ToString(row["Rule Text"]);
                        ruleInfo.State = Convert.ToBoolean(row["Rule State"]);
                        isValidRow = ValidateAndPopulateReportRulePriority(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, row, ruleInfo);
                    }
                    if (isValidRow)
                    {
                        if (reportInfo.Rules == null)
                            reportInfo.Rules = new List<EMRuleInfo>();
                        reportInfo.Rules.Add(ruleInfo);
                    }
                }
            }
        }

        private bool ValidateAndPopulateReportRulePriority(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, ObjectRow row, EMRuleInfo ruleInfo)
        {
            string errorMsg = string.Empty;
            bool isValidRow;
            if (Convert.ToInt32(row["Priority"]) <= 0)
            {
                errorMsg = "Priority cannot be less than 1";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            else if (reportInfo.Rules != null && reportInfo.Rules.Any(x => x.Type.SRMEqualWithIgnoreCase(ruleInfo.Type)) && reportInfo.Rules.Where(x => x.Type.SRMEqualWithIgnoreCase(ruleInfo.Type)).Any(x => x.Priority == Convert.ToInt32(row["Priority"])))
            {
                errorMsg = "Another rule with same priority already provided";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            else
            {
                ruleInfo.Priority = Convert.ToInt32(row["Priority"]);
                isValidRow = true;
            }
            return isValidRow;
        }

        private bool ValidateAndPopulateReportRuleName(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, ObjectRow row, EMRuleInfo ruleInfo)
        {
            string errorMsg = string.Empty;
            bool isValidRow;
            if (string.IsNullOrEmpty(Convert.ToString(row["Rule Name"])))
            {
                errorMsg = "Rule Name cannot be blank";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            else if (reportInfo.Rules != null && reportInfo.Rules.Any(x => x.Type.SRMEqualWithIgnoreCase(Convert.ToString(row["Rule Type"]))) && reportInfo.Rules.Where(x => x.Type.SRMEqualWithIgnoreCase(Convert.ToString(row["Rule Type"]))).Any(x => x.Name.Equals(Convert.ToString(row["Rule Name"]), StringComparison.OrdinalIgnoreCase)))
            {
                errorMsg = "Another rule with same name already provided";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            else
            {
                ruleInfo.Name = Convert.ToString(row["Rule Name"]);
                isValidRow = true;
            }
            return isValidRow;
        }

        private void ValidateAndPopulateReportSetup(ObjectSet diffData, ObjectSet dbData, string userName, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, Dictionary<string, EMReportRepositoryInfo> dctRepositoryVsInfo, Dictionary<string, RMEntityDetailsInfo> dctModuleEntityTypeInfo, Dictionary<string, List<string>> dctReportNameVsET, ref EMReport reportInfo)
        {
            try
            {
                bool isValidRow;
                if (diffData.Tables.Contains(EMReportingConstants.REPORT_SETUP) && diffData.Tables[EMReportingConstants.REPORT_SETUP] != null && diffData.Tables[EMReportingConstants.REPORT_SETUP].Rows.Count > 0)
                {
                    foreach (ObjectRow row in diffData.Tables[EMReportingConstants.REPORT_SETUP].Rows)
                    {
                        isValidRow = IsValidRow(row, dctFailedReportNamevsOutputInfo, dctReportNamevsInfo);
                        if (isValidRow)
                        {
                            reportInfo = SetReportInfo(dctReportNamevsInfo, reportInfo, row, userName, dbData.Tables[EMReportingConstants.REPORT_SETUP], true, dctFailedReportNamevsOutputInfo);
                            isValidRow = ValidateAndPopulateReportType(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, row);
                        }
                        if (isValidRow)
                        {
                            isValidRow = ValidateAndPopulateReportLegacyType(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, row);
                        }
                        if (isValidRow)
                        {
                            SetRepositoryInfo(dbData, reportInfo, dctRepositoryVsInfo, row);
                            isValidRow = ValidateAndPopulateEntityTypeForReportSetup(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, reportInfo, dctModuleEntityTypeInfo, isValidRow, row, dctReportNameVsET);
                        }
                    }
                }
                List<string> lstReportsToBeIgnoredFromDBData = dctReportNameVsET.Keys.ToList();

                foreach (ObjectRow row in dbData.Tables[EMReportingConstants.REPORT_SETUP].Rows)
                {
                    string reportName = Convert.ToString(row["Report Name"]);
                    if (!lstReportsToBeIgnoredFromDBData.Contains(reportName, StringComparer.OrdinalIgnoreCase))
                    {
                        if (!dctReportNameVsET.ContainsKey(reportName))
                        {
                            dctReportNameVsET.Add(reportName, new List<string>());
                        }
                        dctReportNameVsET[reportName].Add(Convert.ToString(row["Entity Type"]));
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("Error occurred in ValidateAndPopulateReportSetup - > " + ex.ToString());
                throw;
            }
        }

        private bool ValidateAndPopulateEntityTypeForReportSetup(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, Dictionary<string, RMEntityDetailsInfo> dctModuleEntityTypeInfo, bool isValidRow, ObjectRow row, Dictionary<string, List<string>> dctReportNameVsET)
        {
            string errorMsg = string.Empty;
            string entityTypeName = Convert.ToString(row["Entity Type"]);
            if (dctModuleEntityTypeInfo.ContainsKey(entityTypeName))
            {
                if (reportInfo.Mapping == null)
                    reportInfo.Mapping = new List<EMReportMappingInfo>();

                if (reportInfo.Report.Type.SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_REPORT.GetDescription()) || reportInfo.Report.Type.SRMEqualWithIgnoreCase(EMReportType.ACROSS_ENTITY_TYPE_AUDIT_REPORT.GetDescription()))
                {
                    if (dctReportNameVsET.ContainsKey(reportInfo.Report.Name) && dctReportNameVsET[reportInfo.Report.Name].Contains(entityTypeName))
                    {
                        errorMsg = "Cannot add same entity type multiple times in report";
                        MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                        isValidRow = false;
                    }
                    else
                    {
                        reportInfo.Mapping.Add(new EMReportMappingInfo() { EntityType = entityTypeName, EntityTypeId = dctModuleEntityTypeInfo[entityTypeName].EntityTypeID, Mapping = new List<EMReportAttributeMappingInfo>() });
                        if (dctReportNameVsET.ContainsKey(reportInfo.Report.Name))
                        {
                            dctReportNameVsET[reportInfo.Report.Name].Add(entityTypeName);
                        }
                        else
                        {
                            dctReportNameVsET.Add(reportInfo.Report.Name, new List<string>() { entityTypeName });
                        }
                    }
                }
                else
                {
                    if (reportInfo.Mapping.Count == 0)
                    {
                        reportInfo.Mapping.Add(new EMReportMappingInfo() { EntityType = entityTypeName, EntityTypeId = dctModuleEntityTypeInfo[entityTypeName].EntityTypeID, Mapping = new List<EMReportAttributeMappingInfo>() });
                        dctReportNameVsET.Add(reportInfo.Report.Name, new List<string>() { entityTypeName });
                    }
                    else
                    {
                        errorMsg = "Cannot add more than one entity type for report type - " + reportInfo.Report.Type;
                        MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                        isValidRow = false;
                    }
                }
            }
            else
            {
                errorMsg = "Entity type " + entityTypeName + " does not exist";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }

            return isValidRow;
        }

        private bool ValidateAndPopulateReportType(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, ObjectRow row)
        {
            bool isValidRow;
            if (reportInfo.Report.Id > 0 && reportInfo.Report.Type != row.Field<string>("Report Type"))
            {
                string errorMsg = "Cannot change report type";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            else
            {
                reportInfo.Report.Type = row.Field<string>("Report Type");
                reportInfo.Report.TypeId = (int)Enum<EMReportType>.Parse(reportInfo.Report.Type);
                isValidRow = true;
            }

            return isValidRow;
        }

        private bool ValidateAndPopulateReportLegacyType(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, ObjectRow row)
        {
            bool isValidRow;
            if (reportInfo.Report.Id > 0 && reportInfo.Report.IsLegacy != Convert.ToBoolean(row["Legacy Report"]))
            {
                string errorMsg = "Cannot change report legacy type";
                MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                isValidRow = false;
            }
            else
            {
                reportInfo.Report.IsLegacy = Convert.ToBoolean(row["Legacy Report"]);
                isValidRow = true;
            }
            return isValidRow;
        }

        private void MarkReportAsFailed(Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo, ObjectRow row, string errorMsg)
        {
            string reportName = Convert.ToString(row["Report Name"]);
            if (!dctFailedReportNamevsOutputInfo.ContainsKey(reportName))
            {
                dctFailedReportNamevsOutputInfo.Add(reportName, new EMReportOutput() { IsSuccess = false, Message = new List<string>() { errorMsg } });
                row["Status"] = "Failed";
                row["Remarks"] = dctFailedReportNamevsOutputInfo[reportName].Message[0];
            }
            if (dctReportNamevsInfo.ContainsKey(reportName))
                dctReportNamevsInfo.Remove(reportName);
        }

        private void SetRepositoryInfo(ObjectSet dbData, EMReport reportInfo, Dictionary<string, EMReportRepositoryInfo> dctRepositoryVsInfo, ObjectRow row)
        {
            string repositoryName = Convert.ToString(row["Repository Name"]);
            if (!dctRepositoryVsInfo.ContainsKey(repositoryName))
            {
                EMReportRepositoryInfo repositoryInfo = new EMReportRepositoryInfo();
                var repository = dbData.Tables[EMReportingConstants.REPORT_SETUP].AsEnumerable().Where(x => x.Field<string>("Repository Name").SRMEqualWithIgnoreCase(repositoryName));
                if (repository != null && repository.Count() > 0)
                {
                    repositoryInfo.Id = repository.FirstOrDefault().Field<int>("report_repository_id");
                }
                else
                {
                    repositoryInfo.Id = -1;
                }
                repositoryInfo.Name = repositoryName;
                repositoryInfo.Description = Convert.ToString(row["Repository Description"]);
                dctRepositoryVsInfo.Add(repositoryInfo.Name, repositoryInfo);
            }
            reportInfo.Repository = dctRepositoryVsInfo[repositoryName];
        }

        private void MarkRowNotProcessed(ObjectRow row)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(row["Status"])))
                row["Status"] = "Not Processed";
        }

        private bool IsValidRow(ObjectRow row, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo, Dictionary<string, EMReport> dctReportNamevsInfo)
        {
            string reportName = Convert.ToString(row["Report Name"]);

            if (!dctFailedReportNamevsOutputInfo.ContainsKey(reportName) && string.IsNullOrEmpty(Convert.ToString(row["Status"])))
            {
                return true;
            }
            else
            {
                if (string.IsNullOrEmpty(Convert.ToString(row["Status"])))
                {
                    MarkRowNotProcessed(row);
                }
                else if (!dctFailedReportNamevsOutputInfo.ContainsKey(reportName) && !string.IsNullOrEmpty(Convert.ToString(row["Remarks"])))
                {
                    dctFailedReportNamevsOutputInfo.Add(reportName, new EMReportOutput() { IsSuccess = false, Message = new List<string>() { Convert.ToString(row["Remarks"]) } });
                    if (dctReportNamevsInfo.ContainsKey(reportName))
                        dctReportNamevsInfo.Remove(reportName);
                }
                return false;
            }
        }

        private EMReport SetReportInfo(Dictionary<string, EMReport> dctReportNamevsInfo, EMReport reportInfo, ObjectRow row, string userName, ObjectTable dbData, bool allowCreation, Dictionary<string, EMReportOutput> dctFailedReportNamevsOutputInfo)
        {
            string reportName = Convert.ToString(row["Report Name"]);
            if (!dctReportNamevsInfo.ContainsKey(reportName))
            {
                if (dbData != null && dbData.Rows.Count > 0)
                {
                    var report = dbData.AsEnumerable().Where(x => x.Field<string>("Report Name").SRMEqualWithIgnoreCase(reportName));
                    if (report != null && report.Count() > 0)
                    {
                        reportInfo = new EMReport();
                        reportInfo.Report = new EMReportInfo();
                        reportInfo.Report.Name = reportName;
                        reportInfo.UserName = userName;
                        reportInfo.Report.Id = report.FirstOrDefault().Field<int>("report_id");
                        reportInfo.Report.Type = report.FirstOrDefault().Field<string>("Report Type");
                        reportInfo.Report.IsLegacy = Convert.ToBoolean(report.FirstOrDefault()["Legacy Report"]);
                        dctReportNamevsInfo.Add(reportName, reportInfo);
                    }
                    else
                    {
                        if (allowCreation)
                        {
                            reportInfo = new EMReport();
                            reportInfo.Report = new EMReportInfo();
                            reportInfo.Report.Name = reportName;
                            reportInfo.UserName = userName;
                            reportInfo.Report.Id = -1;
                            dctReportNamevsInfo.Add(reportInfo.Report.Name, reportInfo);
                        }
                        else
                        {
                            string errorMsg = "Report " + reportName + " does not exist";
                            MarkReportAsFailed(dctFailedReportNamevsOutputInfo, dctReportNamevsInfo, row, errorMsg);
                        }
                    }
                }
                else
                {
                    if (allowCreation)
                    {
                        reportInfo = new EMReport();
                        reportInfo.Report = new EMReportInfo();
                        reportInfo.Report.Name = reportName;
                        reportInfo.UserName = userName;
                        reportInfo.Report.Id = -1;
                        dctReportNamevsInfo.Add(reportInfo.Report.Name, reportInfo);
                    }
                }
            }
            else
                reportInfo = dctReportNamevsInfo[reportName];
            return reportInfo;
        }
    }
}
