using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.RCommonTaskManager;
using com.ivp.rad.RUserManagement;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.reporting
{
    public class EMReportingTaskController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("EMReportingTaskController");
        DataSet dsResult = null;
        DataSet dsReportDetails = null;
        RDBConnectionManager mDBCon;

        private Dictionary<string, List<string>> GetColumnsToRemoveForDownloadFromReportingTaskMetadata()
        {
            Dictionary<string, List<string>> dctTableIndexVsColumnsToRemove = new Dictionary<string, List<string>>();
            dctTableIndexVsColumnsToRemove.Add(EMReportingConstants.REPORTING_TASK_CONFIGURATION, new List<string>() { "task_master_id", "report_id", "report_system_id", "calendar_id", "start_date_type", "end_date_type", "publication_format_id", "extraction_format_id", "extraction_file_date_type", "subscription_format_id" });
            return dctTableIndexVsColumnsToRemove;
        }

        private Dictionary<int, string> GetTableIndexAndNameMappingForReportingTaskMetadata()
        {
            Dictionary<int, string> dctTableIndexVsName = new Dictionary<int, string>();
            dctTableIndexVsName.Add(0, EMReportingConstants.REPORTING_TASK_CONFIGURATION);
            return dctTableIndexVsName;
        }

        private void RenameTableNamesForReportingTaskMetadataForObjectSet(ref ObjectSet reportMetadata)
        {
            Dictionary<int, string> dctTableIndexVsName = GetTableIndexAndNameMappingForReportingTaskMetadata();
            foreach (KeyValuePair<int, string> tableInfo in dctTableIndexVsName)
            {
                reportMetadata.Tables[tableInfo.Key].TableName = tableInfo.Value;
            }
        }

        private void RenameTableNamesForReportingTaskMetadataForDataSet(ref DataSet reportMetadata)
        {
            Dictionary<int, string> dctTableIndexVsName = GetTableIndexAndNameMappingForReportingTaskMetadata();
            foreach (KeyValuePair<int, string> tableInfo in dctTableIndexVsName)
            {
                reportMetadata.Tables[tableInfo.Key].TableName = tableInfo.Value;
            }
        }

        private ObjectSet RemoveIdColumnsForDownloadFromReportingTaskMetadata(ObjectSet reportMetadata)
        {
            Dictionary<string, List<string>> dctTableIndexVsColumnsToRemove = GetColumnsToRemoveForDownloadFromReportingTaskMetadata();
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

        private DataSet RemoveIdColumnsForDownloadFromReportingTaskMetadata(DataSet reportMetadata)
        {
            Dictionary<string, List<string>> dctTableIndexVsColumnsToRemove = GetColumnsToRemoveForDownloadFromReportingTaskMetadata();
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

        public ObjectSet GetReportingTaskMetadata(List<int> lstTasks, string userName, int moduleId, bool removeInternalColumns)
        {
            try
            {
                ObjectSet reportMetaData = null;
                if (lstTasks == null)
                    lstTasks = new List<int>();
                var result = new EMReportingDBManager().GetReportingTaskMetadata(lstTasks.Cast<object>().ToList(), userName, (EMModule)moduleId, EMInputType.Id, EMDataType.ObjectSet);
                if (result != null)
                {
                    reportMetaData = (ObjectSet)result;
                    RenameTableNamesForReportingTaskMetadataForObjectSet(ref reportMetaData);
                    if (removeInternalColumns)
                        reportMetaData = RemoveIdColumnsForDownloadFromReportingTaskMetadata(reportMetaData);
                    UpdateEmailIdsForReportingTaskMetaData(ref reportMetaData);
                }
            
                return reportMetaData;
            }
            catch (Exception ex)
            {
                mLogger.Debug("GetReportingTaskMetadataForDownload - > Error -> " + ex.ToString());
                throw;
            }
        }

        private void UpdateEmailIdsForReportingTaskMetaData(ref ObjectSet reportMetaData)
        {
            RUserManagementService objRUserManagementService = new RUserManagementService();
            Dictionary<string, string> dctLoginNamevsEmail = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            List<RUserInfo> lstUserInfo = objRUserManagementService.GetAllUsersGDPR();
            if (lstUserInfo != null && lstUserInfo.Count > 0)
            {
                dctLoginNamevsEmail = lstUserInfo.Where(x => !string.IsNullOrEmpty(x.EmailId)).ToDictionary(y => y.UserLoginName, x => SRMCommonRAD.GetUserDisplayNameWithEmailFromInfo(x));
            }
            foreach (ObjectRow dr in reportMetaData.Tables[EMReportingConstants.REPORTING_TASK_CONFIGURATION].Rows)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dr["Email Ids"])))
                {
                    List<string> lstUserIds = Convert.ToString(dr["Email Ids"]).Split((new string[] { "," }), StringSplitOptions.RemoveEmptyEntries).ToList();
                    string lstEmails = string.Empty;
                    foreach (string userId in lstUserIds)
                    {
                        if (dctLoginNamevsEmail.ContainsKey(userId))
                        {
                            lstEmails += dctLoginNamevsEmail[userId] + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(lstEmails))
                    {
                        lstEmails = lstEmails.TrimEnd(new char[] { ',' });
                    }
                    dr["Email Ids"] = lstEmails;
                }
            }
        }

        public ObjectSet GetAllReportingTasks(string userName, int moduleId)
        {
            try
            {
                ObjectSet reportingTasks = null;
                var result = new EMReportingDBManager().GetAllReportingTasks(userName, (EMModule)moduleId, EMDataType.ObjectSet);
                if (result != null)
                {
                    reportingTasks = (ObjectSet)result;
                }
                return reportingTasks;
            }
            catch (Exception ex)
            {
                mLogger.Debug("GetAllReportingTasks - > Error -> " + ex.ToString());
                throw;
            }
        }

        public List<string> GetPublicationQueues()
        {
            List<string> lstQueues = new List<string>();
            DataSet dsTransportTypes = new EMReportingDBManager().GetTransportByTransportType(new List<string>() { "MSMQ" });
            if (dsTransportTypes != null && dsTransportTypes.Tables.Count > 0 && dsTransportTypes.Tables[0] != null)
            {
                lstQueues = dsTransportTypes.Tables[0].AsEnumerable().Select(x => x.Field<string>("transport_name")).ToList();
            }
            return lstQueues;
        }

        public List<string> GetEmailTransports()
        {
            List<string> lstQueues = new List<string>();
            DataSet dsTransportTypes = new EMReportingDBManager().GetTransportByTransportType(new List<string>() { "SMTP" });
            if (dsTransportTypes != null && dsTransportTypes.Tables.Count > 0 && dsTransportTypes.Tables[0] != null)
            {
                lstQueues = dsTransportTypes.Tables[0].AsEnumerable().Select(x => x.Field<string>("transport_name")).ToList();
            }
            return lstQueues;
        }

        public List<string> GetFileExtractionTransports()
        {
            List<string> lstQueues = new List<string>();
            DataSet dsTransportTypes = new EMReportingDBManager().GetTransportByTransportType(new List<string>() { "WFT", "FTP", "SFTP" });
            if (dsTransportTypes != null && dsTransportTypes.Tables.Count > 0 && dsTransportTypes.Tables[0] != null)
            {
                lstQueues = dsTransportTypes.Tables[0].AsEnumerable().Select(x => x.Field<string>("transport_name")).ToList();
            }
            return lstQueues;
        }

        public List<string> GetEmailFileExtractionTransports()
        {
            List<string> lstQueues = new List<string>();
            DataSet dsTransportTypes = new EMReportingDBManager().GetTransportByTransportType(new List<string>() { "WFT" });
            if (dsTransportTypes != null && dsTransportTypes.Tables.Count > 0 && dsTransportTypes.Tables[0] != null)
            {
                lstQueues = dsTransportTypes.Tables[0].AsEnumerable().Select(x => x.Field<string>("transport_name")).ToList();
            }
            return lstQueues;
        }

        public List<string> GetPublicationFileFormats()
        {
            List<string> lstFormats = new List<string>();
            DataSet dsFileFormats = new EMReportingDBManager().GetReportFileFormats();
            if (dsFileFormats != null && dsFileFormats.Tables.Count > 0 && dsFileFormats.Tables[0] != null)
            {
                lstFormats = dsFileFormats.Tables[0].Select(" report_file_format NOT IN ('PDF','Excel') ").Select(x => x.Field<string>("report_file_format")).Distinct().ToList();
            }
            return lstFormats;
        }

        public List<string> GetReportFileFormats()
        {
            List<string> lstFormats = new List<string>();
            DataSet dsFileFormats = new EMReportingDBManager().GetReportFileFormats();
            if (dsFileFormats != null && dsFileFormats.Tables.Count > 0 && dsFileFormats.Tables[0] != null)
            {
                lstFormats = dsFileFormats.Tables[0].AsEnumerable().Select(x => x.Field<string>("report_file_format")).Distinct().ToList();
            }
            return lstFormats;
        }

        public List<string> GetReportFileDelimiters()
        {
            List<string> lstFormats = new List<string>();
            DataSet dsFileFormats = new EMReportingDBManager().GetReportFileFormats();
            if (dsFileFormats != null && dsFileFormats.Tables.Count > 0 && dsFileFormats.Tables[0] != null)
            {
                lstFormats = dsFileFormats.Tables[0].AsEnumerable().Where(x => x.Field<string>("report_file_format").Equals("Delimited", StringComparison.OrdinalIgnoreCase)).Select(x => x.Field<string>("report_delimiter_char")).Distinct().ToList();
            }
            return lstFormats;
        }

        public Boolean ValidateAndSaveReportingTask(RMReportingTaskInfo objRMReportingTaskInfo, out string message)
        {
            dsReportDetails = new DataSet();
            dsResult = new DataSet();
            try
            {
                ValidateCommonDetails(objRMReportingTaskInfo);
                if ((objRMReportingTaskInfo.objRMEmailTaskInfo == null || (objRMReportingTaskInfo.objRMEmailTaskInfo != null && (objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds == null || (objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds != null && objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds.Count == 0))))
                    && (objRMReportingTaskInfo.objRMReportExtractionInfo == null || (objRMReportingTaskInfo.objRMReportExtractionInfo != null && string.IsNullOrEmpty(objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName)))
                    && (objRMReportingTaskInfo.objRMReportPublicationInfo == null || (objRMReportingTaskInfo.objRMReportPublicationInfo != null && (objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues == null || (objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues != null && objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues.Count == 0)))))
                {
                    if (!objRMReportingTaskInfo.UsedForDirectDownstreamPosting)
                        throw new Exception("Atleast one of the task/direct posting must be configured");
                }
                else if (objRMReportingTaskInfo.UsedForDirectDownstreamPosting)
                {
                    throw new Exception("Either tasks or direct downstream posting can be configured");
                }

                //if ((objRMReportingTaskInfo.UsedForDirectDownstreamPosting && (Convert.ToString(objRMReportingTaskInfo.ReportSystem).Equals("--Select One--") || Convert.ToString(objRMReportingTaskInfo.ReportSystem).Equals(""))) ||
                //    (!objRMReportingTaskInfo.UsedForDirectDownstreamPosting && !(Convert.ToString(objRMReportingTaskInfo.ReportSystem).Equals("--Select One--") || Convert.ToString(objRMReportingTaskInfo.ReportSystem).Equals(""))))
                //{
                //    throw new Exception("Direct downstream posting configuration is incomplete");
                //}

                if (objRMReportingTaskInfo.objRMEmailTaskInfo != null)
                    ValidateEmailDetails(objRMReportingTaskInfo);
                if (objRMReportingTaskInfo.objRMReportExtractionInfo != null)
                    ValidateExtractionDetails(objRMReportingTaskInfo);

                dsResult = new EMReportingDBManager().SaveReportingTask(objRMReportingTaskInfo, dsReportDetails);
                string operationType = Convert.ToString(dsResult.Tables[0].Rows[0][0]);
                if (string.IsNullOrEmpty(operationType))
                    operationType = string.Empty;
                OperationType otype = operationType.Equals("INSERT") ? OperationType.Add : OperationType.Update;

                new RMCommonController().RMSaveTaskInCTM(objRMReportingTaskInfo.ReportingTaskName, otype, null);

                if (dsResult.Tables[0].Rows[0][0].ToString().Equals("1"))
                {
                    message = dsResult.Tables[0].Rows[0][1].ToString();
                    return true;
                }
                else
                    throw new Exception(dsResult.Tables[0].Rows[0][1].ToString());
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        private static void ValidateExtractionDetails(RMReportingTaskInfo objRMReportingTaskInfo)
        {
            if (((objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileLocation != null && objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileLocation != string.Empty) ||
                (objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileName != null && objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileName != string.Empty)) &&
                (objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName == null || objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName == string.Empty ||
                    objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName.ToLower().Trim() == "--select one--"))
                throw new Exception("Extraction Type must be selected for extraction");

            if (((objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName != null && objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName != string.Empty && objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName.ToLower().Trim() != "--select one--") ||
                (objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileName != null && objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileName != string.Empty)) &&
                (objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileLocation == null || objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileLocation == string.Empty))
                throw new Exception("Remote File Location cannot be empty for extraction");

            if (((objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName != null && objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName != string.Empty && objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName.ToLower().Trim() != "--select one--") ||
                (objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileLocation != null && objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileLocation != string.Empty)) &&
                (objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileName == null || objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileName == string.Empty))
                throw new Exception("Remote File Name cannot be empty for extraction");
        }

        private static void ValidateEmailDetails(RMReportingTaskInfo objRMReportingTaskInfo)
        {
            if (objRMReportingTaskInfo.objRMEmailTaskInfo.EmailTransportTypeName != null &&
                objRMReportingTaskInfo.objRMEmailTaskInfo.EmailTransportTypeName.ToLower().Trim() != "--select one--" &&
                objRMReportingTaskInfo.objRMEmailTaskInfo.EmailTransportTypeName.ToLower().Trim() != "" &&
                objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds.Count == 0)
                throw new Exception("No Email Id selected for mailing.");
            if (objRMReportingTaskInfo.objRMEmailTaskInfo.FileTransportTypeName != null && objRMReportingTaskInfo.objRMEmailTaskInfo.FileTransportTypeName.ToLower().Trim() != "--select one--"
                && objRMReportingTaskInfo.objRMEmailTaskInfo.FileTransportTypeName.ToLower().Trim() != "")
                if (objRMReportingTaskInfo.objRMEmailTaskInfo.FileTransportLocation == null || objRMReportingTaskInfo.objRMEmailTaskInfo.FileTransportLocation == "")
                    throw new Exception("Transport location cannot be empty for selected file transport name.");

        }

        private void ValidateCommonDetails(RMReportingTaskInfo objRMReportingTaskInfo)
        {
            RCalendarController objCalController = new RCalendarController();

            if (objRMReportingTaskInfo.ReportNames.Count == 0)
                throw new Exception("You must select atleast one report.");

            if (objRMReportingTaskInfo.ReportingTaskName.Equals(string.Empty))
                throw new Exception("Task Name cannot be empty.");

            if (!new EMReportingDBManager().IsReportingTaskNameUnique(objRMReportingTaskInfo))
                throw new Exception("Reporting Task name is not unique.");

            dsReportDetails = new EMReportingDBManager().GetDataForReportingTaskControls();
            if (((DataSet)objCalController.GetAllCalendarsSorted()).Tables[0].Select("calendar_name = '" + objRMReportingTaskInfo.CalendarName + "'").Count() > 0)
                objRMReportingTaskInfo.CalendarId = ((DataSet)objCalController.GetAllCalendarsSorted()).Tables[0].Select("calendar_name = '" + objRMReportingTaskInfo.CalendarName + "'").Select(x => x.Field<int>("calendar_id")).First();
            else
                throw new Exception("You must Select a Calendar");

            if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.NOW)
                throw new Exception("'Start Date' cannot be 'Now'");

            if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.CUSTOMDATE && objRMReportingTaskInfo.CustomStartDateValue == null)
                throw new Exception("'Custom Start Date' cannot be empty when 'Start Date' is 'Custom'");

            if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.T_MINUS_N && objRMReportingTaskInfo.CustomStartDateDays < 0)
                throw new Exception("'Enter Business Days' cannot be empty when 'Start Date' is 'T-N'");

            if (objRMReportingTaskInfo.StartDate != RMReportingTaskStartDate.LASTEXTRACTIONDATE && objRMReportingTaskInfo.StartDate != RMReportingTaskStartDate.LASTSUCCESSFULPUSHTIME && objRMReportingTaskInfo.EndDate == RMReportingTaskEndDate.NONE)
                throw new Exception("'End Date' cannot be 'None'");

            if (objRMReportingTaskInfo.EndDate == RMReportingTaskEndDate.CUSTOMDATE && objRMReportingTaskInfo.CustomEndDateValue == null)
                throw new Exception("'Custom End Date' cannot be empty when 'End Date' is 'Custom'");

            if (objRMReportingTaskInfo.EndDate == RMReportingTaskEndDate.T_MINUS_N && objRMReportingTaskInfo.CustomEndDateDays < 0)
                throw new Exception("'Enter Business Days' cannot be empty when 'End Date' is 'T-N'");

            if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.LASTSUCCESSFULPUSHTIME)
            {
                string invalidReports = CheckReportTypeForLastSuccessfulPushTime(objRMReportingTaskInfo);
                if (!invalidReports.Equals(string.Empty))
                    throw new Exception("LastSuccessfulPushTime is valid only for Across Entity Type reports.");
            }
            if (objRMReportingTaskInfo.RequireReportAttributeLevelAudit == true)
            {
                string invalidReports = CheckReportTypeForReportAttributeLevelAudit(objRMReportingTaskInfo);
                if (!invalidReports.Equals(string.Empty))
                    throw new Exception("Report Attribute Level Audit can be true only for Across Entity Type reports.");
            }

            if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.LASTSUCCESSFULPUSHTIME && (objRMReportingTaskInfo.ReportSystem.ToLower().Trim().Equals("--select one--") || string.IsNullOrEmpty(objRMReportingTaskInfo.ReportSystem)))
                throw new Exception("Report System must be selected when Start Date is 'LastSuccessfulPushTime'");

            if (objRMReportingTaskInfo.UsedForDirectDownstreamPosting && (objRMReportingTaskInfo.ReportSystem.ToLower().Trim().Equals("--select one--") || string.IsNullOrEmpty(objRMReportingTaskInfo.ReportSystem)))
                throw new Exception("Report System must be selected when direct downstream posting is configured");

            if (!(objRMReportingTaskInfo.ReportSystem.ToLower().Trim().Equals("--select one--") || string.IsNullOrEmpty(objRMReportingTaskInfo.ReportSystem)))
            {
                if (objRMReportingTaskInfo.StartDate != RMReportingTaskStartDate.LASTSUCCESSFULPUSHTIME && !objRMReportingTaskInfo.UsedForDirectDownstreamPosting)
                {
                    throw new Exception("Report System can only be selected when direct downstream posting is configured or Start Date is 'LastSuccessfulPushTime'");
                }
            }

            if (objRMReportingTaskInfo.StartDate != RMReportingTaskStartDate.LASTSUCCESSFULPUSHTIME && objRMReportingTaskInfo.StartDate != RMReportingTaskStartDate.LASTEXTRACTIONDATE)
                ValidateReportDates(objRMReportingTaskInfo);
            if (objRMReportingTaskInfo.objRMEmailTaskInfo != null)
                ValidateEmailTransport(objRMReportingTaskInfo.objRMEmailTaskInfo);

        }

        private string CheckReportTypeForLastSuccessfulPushTime(RMReportingTaskInfo objRMReportingTaskInfo)
        {
            mLogger.Debug("RMReportingTaskController: CheckReportTypeForAttributeLevelAudit -> Start ");
            try
            {
                string reportNames = string.Join(",", dsReportDetails.Tables[0].Select("report_type NOT IN('Across Entity Type') AND report_name IN ('" + string.Join("','", objRMReportingTaskInfo.ReportNames.ToArray()) + "')").Select(x => x.Field<string>("report_name")).ToArray());
                //htParams = new RHashlist();
                //htParams.Add(RMReportingTaskInfo.REPORT_NAMES, string.Join(",", objRMReportingTaskInfo.ReportNames.ToArray()));
                //dsResult = RMDalWrapper.ExecuteSelectQuery(RMQueryConstantsRMReportSetupController.CheckReportTypeForAttributeLevelAudit,
                //    htParams, RMDBConnectionEnum.RefMDBConnectionId);
                //return dsResult;
                return reportNames;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                mLogger.Debug("RMReportingTaskController: CheckReportTypeForAttributeLevelAudit -> End ");
            }
        }
        private string CheckReportTypeForReportAttributeLevelAudit(RMReportingTaskInfo objRMReportingTaskInfo)
        {
            mLogger.Debug("EMReportingTaskController: CheckReportTypeForReportAttributeLevelAudit -> Start ");
            try
            {
                var sb = new StringBuilder();
                var hshReportNames = new HashSet<string>(objRMReportingTaskInfo.ReportNames);
                foreach (DataRow report in dsReportDetails.Tables[0].Rows)
                {
                    var reportName = report.Field<string>("report_name");
                    if (report.Field<string>("report_type") != "Across Entity Type" && hshReportNames.Contains(reportName))
                    {
                        sb.Append(reportName).Append(",");
                    }
                }
                if (sb.Length > 0)
                    sb = sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                mLogger.Debug("EMReportingTaskController: CheckReportTypeForReportAttributeLevelAudit -> End ");
            }
        }
        private static void ValidateEmailTransport(RMEmailTaskInfo objRMEmailTaskInfo)
        {
            bool isValid = false;
            if (objRMEmailTaskInfo.EmailTransportTypeName.ToLower().Trim() != "--select one--" && objRMEmailTaskInfo.EmailTransportTypeName.Trim() != "")
            {
                if (objRMEmailTaskInfo.EmailIds.Count == 0 ||
                    (objRMEmailTaskInfo.EmailIds.Count == 1 && objRMEmailTaskInfo.EmailIds.First().Trim() == ""))
                {
                    throw new Exception("Email Id cannot be empty when Email Transport type is selected");
                }
            }

            if (objRMEmailTaskInfo.EmailIds.Count > 1 ||
                    (objRMEmailTaskInfo.EmailIds.Count == 1 && objRMEmailTaskInfo.EmailIds.First().Trim() != ""))
            {
                if (objRMEmailTaskInfo.EmailTransportTypeName.ToLower().Trim() == "--select one--" || objRMEmailTaskInfo.EmailTransportTypeName.Trim() == "")
                {
                    throw new Exception("Email Transport type name cannot be empty when any Email Id is selected");
                }
            }

            if (objRMEmailTaskInfo.FileTransportLocation.Trim() != "")
            {
                if (objRMEmailTaskInfo.EmailIds.Count == 0 ||
                    (objRMEmailTaskInfo.EmailIds.Count == 1 && objRMEmailTaskInfo.EmailIds.First().Trim() == ""))
                {
                    throw new Exception("Email Id cannot be empty when Email Transport type is selected");
                }
                if (objRMEmailTaskInfo.FileTransportTypeName.Trim() == "" || objRMEmailTaskInfo.FileTransportTypeName.ToLower().Trim() == "--select one--")
                {
                    throw new Exception("File Transport Type cannot be empty when File Transport Location is given");
                }
            }

            if (objRMEmailTaskInfo.FileTransportTypeName.Trim() != "" && objRMEmailTaskInfo.FileTransportTypeName.ToLower().Trim() != "--select one--")
            {
                if (objRMEmailTaskInfo.EmailIds.Count == 0 ||
                    (objRMEmailTaskInfo.EmailIds.Count == 1 && objRMEmailTaskInfo.EmailIds.First().Trim() == ""))
                {
                    throw new Exception("Email Id cannot be empty when Email Transport type is selected");
                }
                if (objRMEmailTaskInfo.FileTransportLocation.Trim() == "")
                {
                    throw new Exception("File Transport Location cannot be empty when File Transport Type is selected");
                }
            }
        }

        private static void ValidateReportDates(RMReportingTaskInfo objRMReportingTaskInfo)
        {
            RCalendarDateInfo objStartCalendarDateInfo = new RCalendarDateInfo();
            RCalendarDateInfo objEndCalendarDateInfo = new RCalendarDateInfo();
            DateTime tempStartDate = DateTime.Now;
            DateTime tempEndDate = DateTime.Now;

            objStartCalendarDateInfo.CalendarDateType = (RCalendarEnums)(int)objRMReportingTaskInfo.StartDate;
            objEndCalendarDateInfo.CalendarDateType = (RCalendarEnums)(int)objRMReportingTaskInfo.EndDate;

            if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.CUSTOMDATE)
            {
                objStartCalendarDateInfo.CalendarDate = objRMReportingTaskInfo.CustomStartDateValue;
            }
            else
            {
                objStartCalendarDateInfo.CalendarDate = DateTime.Today;
            }


            objStartCalendarDateInfo.DateFormat = objRMReportingTaskInfo.SessionDateFormat;

            if (objRMReportingTaskInfo.EndDate == RMReportingTaskEndDate.CUSTOMDATE)
            {
                objEndCalendarDateInfo.CalendarDate = objRMReportingTaskInfo.CustomEndDateValue;
            }
            else
            {
                objEndCalendarDateInfo.CalendarDate = DateTime.Today;
            }
            objEndCalendarDateInfo.DateFormat = objRMReportingTaskInfo.SessionDateFormat;


            //IF T-N
            if (objStartCalendarDateInfo.CalendarDateType.ToString().ToLower().Equals(RCalendarEnums.T_Minus_N.ToString().ToLower()))
                objStartCalendarDateInfo.NumberOfDays = objRMReportingTaskInfo.CustomStartDateDays;

            //IF T-N
            if (objEndCalendarDateInfo.CalendarDateType.ToString().ToLower().Equals(RCalendarEnums.T_Minus_N.ToString().ToLower()))
                objEndCalendarDateInfo.NumberOfDays = objRMReportingTaskInfo.CustomEndDateDays;

            //Calender Details
            objStartCalendarDateInfo.CalendarID = objRMReportingTaskInfo.CalendarId;
            objEndCalendarDateInfo.CalendarID = objRMReportingTaskInfo.CalendarId;

            //SHOW ERROR IF VALIDATION FAILS
            if (!RCalenderUtils.ValidateBusinessDate(objStartCalendarDateInfo, objEndCalendarDateInfo))
                throw new Exception("Start Date Should Be Less Than Equal To End Date");


        }

        public void SyncReportingTasks(ObjectSet inputData, ObjectSet diffData, ObjectSet dbData, string dateFormat, string userName, int moduleId)
        {
            RMReportingTaskInfo objRMReportingTaskInfo = new RMReportingTaskInfo();
            if (diffData != null)
            {
                RUserManagementService objRUserManagementService = new RUserManagementService();
                Dictionary<string, string> dctLoginNamevsEmail = new Dictionary<string, string>();
                List<RUserInfo> lstUserInfo = objRUserManagementService.GetAllUsersGDPR();
                if (lstUserInfo != null && lstUserInfo.Count > 0)
                {
                    dctLoginNamevsEmail = lstUserInfo.Where(x => !string.IsNullOrEmpty(x.EmailId)).ToDictionary(y => y.UserLoginName, x => SRMCommonRAD.GetUserDisplayNameWithEmailFromInfo(x));
                }
                ObjectSet dsTasks = GetAllReportingTasks(userName, moduleId);
                Dictionary<string, int> dctTasks = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                if (dsTasks != null && dsTasks.Tables.Count > 0 && dsTasks.Tables[0] != null && dsTasks.Tables[0].Rows.Count > 0)
                {
                    dctTasks = dsTasks.Tables[0].AsEnumerable().ToDictionary(x => x.Field<string>(RMCommonConstants.TASK_NAME), y => y.Field<int>(RMCommonConstants.TASK_MASTER_ID));
                }

                foreach (ObjectRow row in diffData.Tables[0].Rows)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(row["Status"])))
                    {
                        bool isSuccess = true;
                        string errorMessage = string.Empty;
                        objRMReportingTaskInfo = ValidateAndPopulateReportingTaskInfo(row, dctLoginNamevsEmail, dctTasks, moduleId, dateFormat, userName, ref isSuccess, ref errorMessage);
                        if (isSuccess)
                        {
                            isSuccess = ValidateAndSaveReportingTask(objRMReportingTaskInfo, out errorMessage);
                        }
                        if (isSuccess)
                        {
                            row["Status"] = RMCommonConstants.MIGRATION_SUCCESS;
                        }
                        else
                        {
                            row["Status"] = RMCommonConstants.MIGRATION_FAILED;
                            row["Remarks"] = errorMessage;
                        }
                    }
                }
            }
        }

        private RMReportingTaskInfo ValidateAndPopulateReportingTaskInfo(ObjectRow row, Dictionary<string, string> dctLoginNamevsEmail, Dictionary<string, int> dctTasks, int moduleId, string dateFormat, string userName, ref bool isSuccess, ref string errorMessage)
        {
            RMReportingTaskInfo objRMReportingTaskInfo = new RMReportingTaskInfo();
            objRMReportingTaskInfo.SessionDateFormat = dateFormat;
            objRMReportingTaskInfo.UserName = userName;
            objRMReportingTaskInfo.ReportingTaskName = Convert.ToString(row["Task Name"]);
            if (dctTasks.ContainsKey(objRMReportingTaskInfo.ReportingTaskName))
            {
                objRMReportingTaskInfo.TaskMasterID = dctTasks[objRMReportingTaskInfo.ReportingTaskName];
            }
            else
            {
                objRMReportingTaskInfo.TaskMasterID = 0;
            }
            objRMReportingTaskInfo.ReportingTaskDescription = Convert.ToString(row["Task Description"]);
            if (!string.IsNullOrEmpty(Convert.ToString(row["Report"])))
            {
                List<string> lstModuleReports = new List<string>();
                ObjectSet result = new EMReportingController().GetAllReports(userName, moduleId);
                if (result != null && result.Tables.Count > 0 && result.Tables[0] != null)
                {
                    lstModuleReports = result.Tables[0].AsEnumerable().Select(x => x.Field<string>(EMReportingConstants.REPORT_NAME)).ToList();
                }
                if (lstModuleReports.Contains(Convert.ToString(row["Report"]).Trim().ToLower(), StringComparer.OrdinalIgnoreCase))
                {
                    objRMReportingTaskInfo.ReportNames = new List<string>() { Convert.ToString(row["Report"]) };
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Report provided does not exist in current module";
                    return null;
                }
            }
            else
            {
                isSuccess = false;
                errorMessage = "Report cannot be blank";
                return null;
            }
            objRMReportingTaskInfo.ReportSystem = Convert.ToString(row["Report System"]);
            if (Convert.ToString(row["Report Attribute Level Audit"]).Equals("yes", StringComparison.OrdinalIgnoreCase) || Convert.ToString(row["Report Attribute Level Audit"]).Equals("true", StringComparison.OrdinalIgnoreCase)) 
            {
                objRMReportingTaskInfo.RequireReportAttributeLevelAudit = true;
            }
            else if (Convert.ToString(row["Report Attribute Level Audit"]).Equals("no", StringComparison.OrdinalIgnoreCase) || Convert.ToString(row["Report Attribute Level Audit"]).Equals("false", StringComparison.OrdinalIgnoreCase)) 
            {
                objRMReportingTaskInfo.RequireReportAttributeLevelAudit = false;
            }
            else
            {
                isSuccess = false;
                errorMessage = "Allowed Report Attribute Level Audit Values are true/false/yes/no";
                return null;
            }

            objRMReportingTaskInfo.CalendarName = Convert.ToString(row["Calendar"]);
          

            if (!string.IsNullOrEmpty(Convert.ToString(row["Start Date"])))
            {
                string dateType = Convert.ToString(row["Start Date"]);
                if (dateType.SRMEqualWithIgnoreCase("T-n"))
                {
                    dateType = "T_Minus_N";
                }
                RMReportingTaskStartDate fileDateType;
                if (Enum.TryParse(dateType, true, out fileDateType))
                {
                    objRMReportingTaskInfo.StartDate = fileDateType;
                    int startDateType = Convert.ToInt32(objRMReportingTaskInfo.StartDate);
                    if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.CUSTOMDATE || startDateType == 4 || (startDateType >= 11 && startDateType <= 14))
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(row["Custom Value Start Date"])))
                        {
                            isSuccess = false;
                            errorMessage = "Custom Value Start Date cannot be blank when Start Date is " + Convert.ToString(row["Start Date"]);
                            return null;
                        }
                    }
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Invalid start date type provided";
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Convert.ToString(row["Custom Value Start Date"])))
            {
                string customDateValue = Convert.ToString(row["Custom Value Start Date"]);
                int dateDays;
                DateTime customDate;
                if (!string.IsNullOrEmpty(Convert.ToString(row["Start Date"])))
                {
                    int startDateType = Convert.ToInt32(objRMReportingTaskInfo.StartDate);
                    if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.CUSTOMDATE)
                    {
                        if (DateTime.TryParseExact(customDateValue, new string[] { dateFormat, dateFormat + " hh:mm:ss tt" }, null, System.Globalization.DateTimeStyles.None, out customDate))
                        {
                            objRMReportingTaskInfo.CustomStartDateValue = customDate;
                        }
                        else
                        {
                            isSuccess = false;
                            errorMessage = "Invalid custom value start date provided";
                            return null;
                        }
                    }
                    else if (startDateType == 4 || (startDateType >= 11 && startDateType <= 14))
                    {
                        if (int.TryParse(customDateValue, out dateDays))
                        {
                            if (dateDays > 0)
                            {
                                objRMReportingTaskInfo.CustomStartDateDays = dateDays;
                            }
                            else
                            {
                                isSuccess = false;
                                errorMessage = "Invalid custom value start date provided";
                                return null;
                            }
                        }
                        else
                        {
                            isSuccess = false;
                            errorMessage = "Invalid custom value start date provided";
                            return null;
                        }
                    }
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Custom value start date is not valid for blank start date";
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Convert.ToString(row["End Date"])))
            {
                if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.LASTEXTRACTIONDATE)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(row["End Date"])))
                    {
                        isSuccess = false;
                        errorMessage = "End Date should be blank if start date is LastExtractionDate";
                        return null;
                    }
                }
                else if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.LASTSUCCESSFULPUSHTIME)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(row["End Date"])))
                    {
                        isSuccess = false;
                        errorMessage = "End Date should be blank if start date is LastSuccessfulPushTime";
                        return null;
                    }
                }
                else
                {
                    string dateType = Convert.ToString(row["End Date"]);
                    if (dateType.SRMEqualWithIgnoreCase("T-n"))
                    {
                        dateType = "T_Minus_N";
                    }
                    RMReportingTaskEndDate fileDateType;
                    if (Enum.TryParse(dateType, true, out fileDateType))
                    {
                        objRMReportingTaskInfo.EndDate = fileDateType;
                        int endDateType = Convert.ToInt32(objRMReportingTaskInfo.EndDate);
                        if (objRMReportingTaskInfo.EndDate == RMReportingTaskEndDate.CUSTOMDATE || endDateType == 4 || (endDateType >= 11 && endDateType <= 14))
                        {
                            if (string.IsNullOrEmpty(Convert.ToString(row["Custom Value End Date"])))
                            {
                                isSuccess = false;
                                errorMessage = "Custom Value End Date cannot be blank when End Date is " + Convert.ToString(row["End Date"]);
                                return null;
                            }
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        errorMessage = "Invalid end date type provided";
                        return null;
                    }
                }
            }

            if (!string.IsNullOrEmpty(Convert.ToString(row["Custom Value End Date"])))
            {
                string customDateValue = Convert.ToString(row["Custom Value End Date"]);
                int dateDays;
                DateTime customDate;
                if (!string.IsNullOrEmpty(Convert.ToString(row["End Date"])))
                {
                    int endDateType = Convert.ToInt32(objRMReportingTaskInfo.EndDate);
                    if (objRMReportingTaskInfo.EndDate == RMReportingTaskEndDate.CUSTOMDATE)
                    {
                        if (DateTime.TryParseExact(customDateValue, new string[] { "MM/dd/yyyy", "MM/dd/yyyy hh:mm:ss tt" }, null, System.Globalization.DateTimeStyles.None, out customDate))
                        {
                            objRMReportingTaskInfo.CustomEndDateValue = customDate;
                        }
                        else
                        {
                            isSuccess = false;
                            errorMessage = "Invalid custom value end date provided";
                            return null;
                        }
                    }
                    else if (endDateType == 4 || endDateType >= 11 || endDateType <= 14)
                    {
                        if (int.TryParse(customDateValue, out dateDays))
                        {
                            if (dateDays > 0)
                            {
                                objRMReportingTaskInfo.CustomEndDateDays = dateDays;
                            }
                            else
                            {
                                isSuccess = false;
                                errorMessage = "Invalid custom value end date provided";
                                return null;
                            }
                        }
                        else
                        {
                            isSuccess = false;
                            errorMessage = "Invalid custom value end date provided";
                            return null;
                        }
                    }
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Custom value end date is not valid for blank end date";
                    return null;
                }
            }
            
            if (!string.IsNullOrEmpty(Convert.ToString(row["Send Real Time Updates"])))
            {
                objRMReportingTaskInfo.UsedForRealTimeUpdate = Convert.ToBoolean(row["Send Real Time Updates"]);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(row["Post To Downstream System"])))
            {
                objRMReportingTaskInfo.UsedForDirectDownstreamPosting = Convert.ToBoolean(row["Post To Downstream System"]);
            }
            objRMReportingTaskInfo.objRMReportPublicationInfo = new RMReportPublicationInfo();
            objRMReportingTaskInfo.objRMReportExtractionInfo = new RMReportExtractionInfo();
            objRMReportingTaskInfo.objRMEmailTaskInfo = new RMEmailTaskInfo();
            objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues = new List<string>();
            if (!string.IsNullOrEmpty(Convert.ToString(row["Publication Queues"])))
            {
                List<string> lstValidQueues = GetPublicationQueues();
                List<string> lstQueues = Convert.ToString(row["Publication Queues"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (string queueName in lstQueues)
                {
                    if (lstValidQueues.Contains(queueName.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        string dbQueueName = lstValidQueues.Where(x => x.Equals(queueName.Trim(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues.Add(dbQueueName);
                    }
                    else
                    {
                        isSuccess = false;
                        errorMessage = "Invalid queue name '" + queueName.Trim() + "'";
                        return null;
                    }
                }
                //objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues = lstQueues;
            }
            else
            {
                objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues = new List<string>();
            }

            if (!string.IsNullOrEmpty(Convert.ToString(row["Publication Format"])))
            {
                if (objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues.Count == 0)
                {
                    isSuccess = false;
                    errorMessage = "Publication Queues cannot be blank if Publication is configured";
                    return null;
                }
                RMReportingTaskFormat pubFormat;
                string publicationFormatName = Convert.ToString(row["Publication Format"]);
                string tempPubFormatName = string.Empty;
                objRMReportingTaskInfo.objRMReportPublicationInfo.ReportFileFormatDelimiter = Convert.ToString(row["Publication Delimiter"]);
                if (publicationFormatName.ToLower().Equals("delimited"))
                {
                    if (!string.IsNullOrEmpty(objRMReportingTaskInfo.objRMReportPublicationInfo.ReportFileFormatDelimiter))
                    {
                        if (objRMReportingTaskInfo.objRMReportPublicationInfo.ReportFileFormatDelimiter.Equals(","))
                            tempPubFormatName = "COMMA_" + publicationFormatName;
                        if (objRMReportingTaskInfo.objRMReportPublicationInfo.ReportFileFormatDelimiter.Equals("|"))
                            tempPubFormatName = "PIPE_" + publicationFormatName;
                    }
                    else
                    {
                        isSuccess = false;
                        errorMessage = "Publication Delimiter cannot be blank if Publication Format is Delimited";
                        return null;
                    }
                }
                else
                {
                    tempPubFormatName = publicationFormatName;
                }
                if (Enum.TryParse(tempPubFormatName, true, out pubFormat))
                {
                    objRMReportingTaskInfo.objRMReportPublicationInfo.ReportFileFormatName = publicationFormatName;
                    objRMReportingTaskInfo.objRMReportPublicationInfo.ReportFileFormat = pubFormat;
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Invalid publication format";
                    return null;
                }
            }
            else
            {
                if (objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues.Count > 0)
                {
                    isSuccess = false;
                    errorMessage = "Publication Format cannot be blank if Publication is configured";
                    return null;
                }
            }

            List<string> lstValidExtractionTransports = GetFileExtractionTransports();
            string dbTransportName = string.Empty;
            if (!string.IsNullOrEmpty(Convert.ToString(row["Extraction Transport"])))
            {
                if (lstValidExtractionTransports.Contains(Convert.ToString(row["Extraction Transport"]), StringComparer.OrdinalIgnoreCase))
                {
                    dbTransportName = lstValidExtractionTransports.Where(x => x.Equals(Convert.ToString(row["Extraction Transport"]), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Invalid Extraction Transport provided";
                    return null;
                }
            }
            objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName = dbTransportName;
            objRMReportingTaskInfo.objRMReportExtractionInfo.ReportFileFormatDelimiter = Convert.ToString(row["Remote File Delimiter"]);

            if (!string.IsNullOrEmpty(Convert.ToString(row["Remote File Format"])))
            {
                if (string.IsNullOrEmpty(objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName))
                {
                    isSuccess = false;
                    errorMessage = "Extraction Transport cannot be blank if Extraction is configured";
                    return null;
                }
                RMReportingTaskFormat fileFormat;
                string fileFormatName = Convert.ToString(row["Remote File Format"]);
                string tempFormatName = string.Empty;
                if (fileFormatName.ToLower().Equals("delimited"))
                {
                    if (!string.IsNullOrEmpty(objRMReportingTaskInfo.objRMReportExtractionInfo.ReportFileFormatDelimiter))
                    {
                        if (objRMReportingTaskInfo.objRMReportExtractionInfo.ReportFileFormatDelimiter.Equals(","))
                            tempFormatName = "COMMA_" + fileFormatName;
                        if (objRMReportingTaskInfo.objRMReportExtractionInfo.ReportFileFormatDelimiter.Equals("|"))
                            tempFormatName = "PIPE_" + fileFormatName;
                    }
                    else
                    {
                        isSuccess = false;
                        errorMessage = "Remote File Delimiter cannot be blank if Remote File Format is Delimited";
                        return null;
                    }
                }
                else
                {
                    tempFormatName = fileFormatName;
                }
                if (Enum.TryParse(tempFormatName, true, out fileFormat))
                {
                    objRMReportingTaskInfo.objRMReportExtractionInfo.ReportFileFormatName = fileFormatName;
                    objRMReportingTaskInfo.objRMReportExtractionInfo.ReportFileFormat = fileFormat;
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Invalid remote file format";
                    return null;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName))
                {
                    isSuccess = false;
                    errorMessage = "Remote File Format cannot be blank if Extraction is configured";
                    return null;
                }
            }

            objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileName = Convert.ToString(row["Remote File Name"]);
            objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileLocation = Convert.ToString(row["Remote File Location"]);

            if (!string.IsNullOrEmpty(Convert.ToString(row["Report File Date Type"])))
            {
                string dateType = Convert.ToString(row["Report File Date Type"]);
                if (dateType.SRMEqualWithIgnoreCase("T-n"))
                {
                    dateType = "T_Minus_N";
                }
                RMExtractionDate fileDateType;
                if (Enum.TryParse(dateType, true, out fileDateType))
                {
                    objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionDateType = fileDateType;
                    int extractionDateType = Convert.ToInt32(objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionDateType);
                    if (objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionDateType == RMExtractionDate.CUSTOMDATE || extractionDateType == 4 || (extractionDateType >= 11 && extractionDateType <= 14))
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(row["Custom Value Report File Date Type"])))
                        {
                            isSuccess = false;
                            errorMessage = "Custom Value Report File Date Type cannot be blank when Report File Date Type is " + Convert.ToString(row["Report File Date Type"]);
                            return null;
                        }
                    }
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Invalid report file date type provided";
                    return null;
                }
            }

            if (!string.IsNullOrEmpty(Convert.ToString(row["Custom Value Report File Date Type"])))
            {
                string customDateValue = Convert.ToString(row["Custom Value Report File Date Type"]);
                int dateDays;
                DateTime customDate;
                if (!string.IsNullOrEmpty(Convert.ToString(row["Report File Date Type"])))
                {
                    int startDateType = Convert.ToInt32(objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionDateType);
                    if (objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionDateType == RMExtractionDate.CUSTOMDATE)
                    {
                        if (DateTime.TryParseExact(customDateValue, new string[] { "MM/dd/yyyy", "MM/dd/yyyy hh:mm:ss tt" }, null, System.Globalization.DateTimeStyles.None, out customDate))
                        {
                            objRMReportingTaskInfo.objRMReportExtractionInfo.CustomExtractionDateValue = customDate;
                        }
                        else
                        {
                            isSuccess = false;
                            errorMessage = "Invalid custom value report file date type provided";
                            return null;
                        }
                    }
                    else if (startDateType == 4 || (startDateType >= 11 && startDateType <= 14))
                    {
                        if (int.TryParse(customDateValue, out dateDays))
                        {
                            if (dateDays > 0)
                            {
                                objRMReportingTaskInfo.objRMReportExtractionInfo.CustomExtractionDateDays = dateDays;
                            }
                            else
                            {
                                isSuccess = false;
                                errorMessage = "Invalid custom value report file date type provided";
                                return null;
                            }
                        }
                        else
                        {
                            isSuccess = false;
                            errorMessage = "Invalid custom value report file date type provided";
                            return null;
                        }
                    }
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Custom value report file date type is valid for the report file date type provided";
                    return null;
                }
            }
            if (!string.IsNullOrEmpty(Convert.ToString(row["Email Ids"])))
            {
                List<string> lstEmails = new List<string>();
                List<string> lstNamevsEmail = new List<string>();
                lstNamevsEmail = Convert.ToString(row["Email Ids"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (string name in lstNamevsEmail)
                {
                    if (dctLoginNamevsEmail.ContainsValue(name.Trim()))
                    {
                        lstEmails.Add(dctLoginNamevsEmail.Where(x => x.Value.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Key);
                    }
                    else
                    {
                        isSuccess = false;
                        errorMessage = "Invalid email id (" + name + ") provided";
                        return null;
                    }
                }
                objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds = lstEmails;
            }
            else
            {
                objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds = new List<string>();
            }

            List<string> lstValidEmailTransports = GetEmailTransports();
            string dbEmailTransportName = string.Empty;
            if (!string.IsNullOrEmpty(Convert.ToString(row["Email Transport"])))
            {
                if (objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds.Count == 0)
                {
                    isSuccess = false;
                    errorMessage = "Email Ids cannot be blank if Email is configured";
                    return null;
                }

                if (lstValidEmailTransports.Contains(Convert.ToString(row["Email Transport"]), StringComparer.OrdinalIgnoreCase))
                {
                    dbEmailTransportName = lstValidEmailTransports.Where(x => x.Equals(Convert.ToString(row["Email Transport"]), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Invalid Email Transport provided";
                    return null;
                }
            }
            else
            {
                if(objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds.Count > 0)
                {
                    isSuccess = false;
                    errorMessage = "Email Transport cannot be blank if Email is configured";
                    return null;
                }
            }

            objRMReportingTaskInfo.objRMEmailTaskInfo.EmailTransportTypeName = dbEmailTransportName;
            objRMReportingTaskInfo.objRMEmailTaskInfo.ReportFileFormatDelimiter = Convert.ToString(row["Email File Delimiter"]);

            if (!string.IsNullOrEmpty(Convert.ToString(row["Email File Format"])))
            {
                if(string.IsNullOrEmpty(objRMReportingTaskInfo.objRMEmailTaskInfo.EmailTransportTypeName))
                {
                    isSuccess = false;
                    errorMessage = "Email Transport cannot be blank if Email is configured";
                    return null;
                }
                 
                RMReportingTaskFormat fileFormat;
                string fileFormatName = Convert.ToString(row["Email File Format"]);
                string tempFormatName = string.Empty;
                if (fileFormatName.ToLower().Equals("delimited"))
                {
                    if (!string.IsNullOrEmpty(objRMReportingTaskInfo.objRMEmailTaskInfo.ReportFileFormatDelimiter))
                    {
                        if (objRMReportingTaskInfo.objRMEmailTaskInfo.ReportFileFormatDelimiter.Equals(","))
                            tempFormatName = "COMMA_" + fileFormatName;
                        if (objRMReportingTaskInfo.objRMEmailTaskInfo.ReportFileFormatDelimiter.Equals("|"))
                            tempFormatName = "PIPE_" + fileFormatName;
                    }
                    else
                    {
                        isSuccess = false;
                        errorMessage = "Email File Delimiter cannot be blank if Email File Format is Delimited";
                        return null;
                    }
                }
                else
                {
                    tempFormatName = fileFormatName;
                }
                if (Enum.TryParse(tempFormatName, true, out fileFormat))
                {
                    objRMReportingTaskInfo.objRMEmailTaskInfo.ReportFileFormatName = fileFormatName;
                    objRMReportingTaskInfo.objRMEmailTaskInfo.ReportFileFormat = fileFormat;
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Invalid email file format";
                    return null;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(objRMReportingTaskInfo.objRMEmailTaskInfo.EmailTransportTypeName))
                {
                    isSuccess = false;
                    errorMessage = "Email File Format cannot be blank if Email is configured";
                    return null;
                }
            }

            List<string> lstValidEmailFileTransports = GetEmailFileExtractionTransports();
            string dbEmailFileTransportName = string.Empty;
            if (!string.IsNullOrEmpty(Convert.ToString(row["Email File Transport"])))
            {
                if (lstValidEmailFileTransports.Contains(Convert.ToString(row["Email File Transport"]), StringComparer.OrdinalIgnoreCase))
                {
                    dbEmailFileTransportName = lstValidEmailFileTransports.Where(x => x.Equals(Convert.ToString(row["Email File Transport"]), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                }
                else
                {
                    isSuccess = false;
                    errorMessage = "Invalid Email File Transport provided";
                    return null;
                }
            }

            objRMReportingTaskInfo.objRMEmailTaskInfo.FileTransportTypeName = dbEmailFileTransportName;
            objRMReportingTaskInfo.objRMEmailTaskInfo.FileTransportLocation = Convert.ToString(row["Email File Location"]);



            return objRMReportingTaskInfo;

        }
    }
}
