using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.controls.xruleeditor;
using com.ivp.rad.controls.xruleeditor.grammar;
using com.ivp.rad.culturemanager;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.RRadWorkflow;
using com.ivp.rad.RUserManagement;
using com.ivp.rad.viewmanagement;
using com.ivp.rad.xruleengine;
using com.ivp.srmcommon;
using Irony.Parsing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace com.ivp.common.migration
{
    public class SRMWorkflowMigrationController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMWorkflowMigrationController");
        private Dictionary<string, int> workFlowNameDBSet = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, int> dicWorkflowActionTypeNameVsID = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        //private static ConcurrentDictionary<string, Dictionary<string, HashSet<string>>> workflowTypeVsInstrumentTypes = new ConcurrentDictionary<string, Dictionary<string, HashSet<string>>>(StringComparer.OrdinalIgnoreCase);

        private HashSet<string> pendingRequestWorkflows = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, int> radWorkflowsNameVsId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, WorkflowInfo> dictDelta_ValidWorkflowNameVWorkflowInfo = new Dictionary<string, WorkflowInfo>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, WorkflowInfo> dictDB_WorkflowNameVWorkflowInfo = new Dictionary<string, WorkflowInfo>(StringComparer.OrdinalIgnoreCase);

        //private static ConcurrentDictionary<string, Dictionary<string, string>> inValidWorkflowName = new ConcurrentDictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        private HashSet<string> validWorkflowsSyncSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        //private static ConcurrentDictionary<string, HashSet<string>> validRemovedTypeWorkflowsSyncSet = new ConcurrentDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, Dictionary<string, string>> inValidWorkflowNameVsSheetNameVsError = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, HashSet<string>> dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, List<string>> dictDB_WorkflowNameVSInstrumentTypeNames = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, HashSet<string>> dict_WorkflowNameVSRuleInfo = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        //private static ConcurrentDictionary<string, Dictionary<string, HashSet<string>>> dictDB_WorkflowNameVSRuleInfo = new ConcurrentDictionary<string, Dictionary<string, HashSet<string>>>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, HashSet<string>> dict_RadTemplateVSStates = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, Dictionary<string, List<string>>> workflowNameVsAttribute = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, Dictionary<string, HashSet<RuleStateInfo>>> workflowNameVsRuleInfoKeyVsStageDataSet = new Dictionary<string, Dictionary<string, HashSet<RuleStateInfo>>>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, Dictionary<string, HashSet<RuleStateInfo>>> dictDB_workflowNameVsRuleInfoKeyVsStageDataSet = new Dictionary<string, Dictionary<string, HashSet<RuleStateInfo>>>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, List<string>> workflowNameVsTypeId = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, SecurityTypeMasterInfo> sectypeTypeVsAttributesInfo;
        private Dictionary<string, AttrInfo> commonAttributeNameVsInfo;
        private Dictionary<string, RMEntityDetailsInfo> entityTypeVsAttributesInfo;

        private Dictionary<string, HashSet<string>> dict_WorkflowNameVSRuleInfoForActions = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, HashSet<string>> dict_RadTemplateVSActions = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, Dictionary<string, Dictionary<string, ActionDataDetails>>> dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet = new Dictionary<string, Dictionary<string, Dictionary<string, ActionDataDetails>>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, Dictionary<string, Dictionary<string, ActionDataDetails>>> workflowNameVsRuleInfoKeyVsActionNameActionDataSet = new Dictionary<string, Dictionary<string, Dictionary<string, ActionDataDetails>>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<int, string> AttributeIdVsAttributeName = new Dictionary<int, string> {
            { -1,"Security Type"},{ -2,"Security ID"},{ -3,"Requested By"},{ -4,"Requested Time"},{-5,"Approved By"},{-6,"Approved At"},{-7,"Approver Comments"},{-8,"Rejected By"},{-9,"Rejected Time"},{-10,"Rejection Comments"},{-11,"Cancelled By"},{-12,"Cancellation Time"},{-13,"Cancellation Comments"},{-14,"Entity Type"},{-15,"Entity Code"}
        };
        private Dictionary<string, int> AttributeNameVsAttributeId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) {
            { "Security Type",-1},{"Security ID", -2},{"Requested By", -3},{ "Requested Time",-4},{"Approved By",-5},{"Approved At",-6},{"Approver Comments",-7},{"Rejected By",-8},{"Rejected Time",-9},{"Rejection Comments",-10},{"Cancelled By",-11},{"Cancellation Time",-12},{"Cancellation Comments",-13},{"Entity Type",-14},{"Entity Code",-15}
        };
        private Dictionary<string, HashSet<string>> actionNameVsAllowedCustomAttributes = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase) {
            { "initiates",new HashSet<string>(StringComparer.OrdinalIgnoreCase){"Requested By","Requested Time","Security Type","Security ID","Entity Type","Entity Code" } },{ "approve",new HashSet<string>(StringComparer.OrdinalIgnoreCase){"Approved By","Approved At", "Approver Comments", "Security Type", "Security ID", "Entity Type", "Entity Code" } },{ "reject", new HashSet<string>(StringComparer.OrdinalIgnoreCase){ "Rejected By","Rejected Time","Rejection Comments", "Security Type", "Security ID", "Entity Type", "Entity Code" } },{ "cancel", new HashSet<string>(StringComparer.OrdinalIgnoreCase){ "Cancelled By","Cancellation Time","Cancellation Comments", "Security Type", "Security ID", "Entity Type", "Entity Code" } } };
        private Dictionary<string, HashSet<string>> WorkflowNameVsAttributes = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, Dictionary<int, string>> workflowNameVsRulePriorityVsRadTemplateName = new Dictionary<string, Dictionary<int, string>>(StringComparer.OrdinalIgnoreCase);
        private ObjectRow previousRow;
        private ActionDataDetails pAction = new ActionDataDetails();
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> workflowNameVsRulePriorityVsActionNameVsBulkSubject = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);


        public ObjectSet GetAllWorkflowsConfiguration(int moduleId, List<int> workflowIds)
        {
            try
            {
                SRMWorkflowMigrationController obj = new SRMWorkflowMigrationController();
                mLogger.Debug("WorkflowController::GetAllWorkflowsConfiguration - START");
                ObjectSet refConfigDS = null;
                ObjectSet secConfigDS = null;

                string selectedWorkflowIds;
                if (workflowIds == null || workflowIds.Count == 0)
                    selectedWorkflowIds = "";
                else
                    selectedWorkflowIds = string.Join("|", workflowIds);

                if (moduleId == 3)
                {
                    secConfigDS = CommonDALWrapper.ExecuteSelectQueryObject(@"EXEC IVPSecMaster.dbo.SRM_FetchCompleteWorkflowConfiguration @workflowIds = '" + selectedWorkflowIds + "'", ConnectionConstants.SecMaster_Connection);

                    ObjectTable EmailConfiguration = AddEmailConfigurationSheet(moduleId, secConfigDS);
                    secConfigDS.Tables.Add(EmailConfiguration);

                    secConfigDS = RenameWorkflowConfigurationTables(secConfigDS);
                    return secConfigDS;
                }
                else
                {
                    refConfigDS = CommonDALWrapper.ExecuteSelectQueryObject(@"EXEC IVPRefMaster.dbo.SRM_FetchCompleteWorkflowConfiguration @workflowIds = '" + selectedWorkflowIds + "', @module_id = " + moduleId + ";", ConnectionConstants.RefMaster_Connection);

                    ObjectTable EmailConfiguration = AddEmailConfigurationSheet(moduleId, refConfigDS);
                    refConfigDS.Tables.Add(EmailConfiguration);

                    refConfigDS = RenameWorkflowConfigurationTables(refConfigDS);
                    return refConfigDS;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("WorkflowController::GetAllWorkflowsConfiguration - ERROR - " + ex.ToString());
            }
            finally
            {
                mLogger.Debug("WorkflowController::GetAllWorkflowsConfiguration - END");
            }
            return null;
        }

        public ObjectTable AddEmailConfigurationSheet(int moduleId, ObjectSet configDs)
        {
            ObjectTable EmailConfiguration = new ObjectTable("Email Configuration");
            EmailConfiguration.Columns.Add("Workflow Name", typeof(string));
            EmailConfiguration.Columns.Add("Rule Priority", typeof(int));
            EmailConfiguration.Columns.Add("Include Action", typeof(string));
            EmailConfiguration.Columns.Add("Action Name", typeof(string));
            EmailConfiguration.Columns.Add("Keep Application URL In Footer", typeof(string));
            EmailConfiguration.Columns.Add("Send Consolidated Email For Bulk Action", typeof(string));
            EmailConfiguration.Columns.Add("Keep Creator In CC", typeof(string));
            EmailConfiguration.Columns.Add("To", typeof(string));
            EmailConfiguration.Columns.Add("Subject", typeof(string));
            EmailConfiguration.Columns.Add("Bulk Subject", typeof(string));
            EmailConfiguration.Columns.Add("Mail Body Content", typeof(string));
            EmailConfiguration.Columns.Add("Data Section Attributes", typeof(string));

            List<WorkflowDetails> inputDetails = new List<WorkflowDetails>();

            foreach (ObjectRow row in configDs.Tables[4].Rows)
            {
                WorkflowDetails input = new WorkflowDetails();
                input.WorkflowName = Convert.ToString(row["Workflow Name"]);
                if (String.IsNullOrEmpty(Convert.ToString(row["Rule Priority"])))
                    input.RulePriority = -1;
                else
                    input.RulePriority = int.Parse(Convert.ToString(row["Rule Priority"]));

                inputDetails.Add(input);

                string workflowName = input.WorkflowName;
                int rulePriority = input.RulePriority;
                if (!workflowNameVsRulePriorityVsRadTemplateName.ContainsKey(workflowName))
                    workflowNameVsRulePriorityVsRadTemplateName.Add(workflowName, new Dictionary<int, string>());
                if (!workflowNameVsRulePriorityVsRadTemplateName[workflowName].ContainsKey(rulePriority))
                    workflowNameVsRulePriorityVsRadTemplateName[workflowName].Add(rulePriority, Convert.ToString(row["Rad Workflow Template Name"]));

            }


            List<WorkflowEmailState> result = new List<WorkflowEmailState>();
            result = GetWorkflowEmailActions(moduleId, inputDetails);


            Dictionary<string, Dictionary<int, List<ActionDataDetails>>> resultDict = new Dictionary<string, Dictionary<int, List<ActionDataDetails>>>();


            foreach (var r in result)
            {
                if (!resultDict.ContainsKey(r.WorkflowName))
                {
                    resultDict.Add(r.WorkflowName, new Dictionary<int, List<ActionDataDetails>>());
                }
                if (!resultDict[r.WorkflowName].ContainsKey(r.RulePriority))
                {
                    resultDict[r.WorkflowName].Add(r.RulePriority, new List<ActionDataDetails>());
                }
                resultDict[r.WorkflowName][r.RulePriority] = r.Actions;
            }

            foreach (var input in inputDetails)
            {
                if (resultDict.ContainsKey(input.WorkflowName))
                {
                    var value = resultDict[input.WorkflowName];
                    if (value.ContainsKey(input.RulePriority))
                    {
                        List<ActionDataDetails> res = value[input.RulePriority];
                        foreach (var r in res)
                        {
                            if (r.DataSectionAttributes == null)
                            {
                                var row = EmailConfiguration.NewRow();
                                row["Workflow Name"] = input.WorkflowName;
                                if (input.RulePriority != -1)
                                    row["Rule Priority"] = input.RulePriority;
                                else
                                    row["Rule Priority"] = DBNull.Value;
                                row["Include Action"] = (Convert.ToBoolean(r.CheckBoxForEachAction)) ? "Yes" : "No";
                                row["Action Name"] = r.ActionName;
                                row["Keep Application URL In Footer"] = (Convert.ToBoolean(r.KeepApplicationURLInTheFooter)) ? "Yes" : "No";
                                row["Send Consolidated Email For Bulk Action"] = (Convert.ToBoolean(r.SendConsolidatedEmailForBulkAction)) ? "Yes" : "No";
                                row["Keep Creator In CC"] = (Convert.ToBoolean(r.KeepCreatorInCC)) ? "Yes" : "No";
                                row["To"] = r.To;
                                row["Subject"] = r.Subject;
                                row["Bulk Subject"] = r.BulkSubject;
                                row["Mail Body Content"] = r.MailBodyContent;
                                row["Data Section Attributes"] = "";
                                EmailConfiguration.Rows.Add(row);
                            }
                            else
                            {
                                for (int i = 0; i < r.DataSectionAttributes.Count; i++)
                                {
                                    var row = EmailConfiguration.NewRow();
                                    row["Workflow Name"] = input.WorkflowName;
                                    if (input.RulePriority != -1)
                                        row["Rule Priority"] = input.RulePriority;
                                    else
                                        row["Rule Priority"] = DBNull.Value;
                                    row["Include Action"] = (Convert.ToBoolean(r.CheckBoxForEachAction)) ? "Yes" : "No";
                                    row["Action Name"] = r.ActionName;
                                    row["Keep Application URL In Footer"] = (Convert.ToBoolean(r.KeepApplicationURLInTheFooter)) ? "Yes" : "No";
                                    row["Send Consolidated Email For Bulk Action"] = (Convert.ToBoolean(r.SendConsolidatedEmailForBulkAction)) ? "Yes" : "No";
                                    row["Keep Creator In CC"] = (Convert.ToBoolean(r.KeepCreatorInCC)) ? "Yes" : "No";
                                    row["To"] = r.To;
                                    row["Subject"] = r.Subject;
                                    row["Bulk Subject"] = r.BulkSubject;
                                    row["Mail Body Content"] = r.MailBodyContent;
                                    row["Data Section Attributes"] = AttributeIdVsAttributeName[Convert.ToInt32(r.DataSectionAttributes[i])];


                                    EmailConfiguration.Rows.Add(row);
                                }
                            }

                        }
                    }
                }
            }
            return EmailConfiguration;
        }

        public List<CommonMigrationSelectionInfo> SRMGetSelectableWorkflows(int moduleID)
        {
            List<CommonMigrationSelectionInfo> lstSelectableworkflows = new List<CommonMigrationSelectionInfo>();
            CommonMigrationSelectionInfo workflow = null;
            DataSet ds = null;

            string query = string.Format(@"SELECT
                                        ins.workflow_instance_name AS[Text],
                                        ins.workflow_instance_id AS[Value]
                                        FROM[IVPRefMaster].[dbo].[ivp_srm_workflow_instance] ins
                                        JOIN[IVPRefMaster].[dbo].[ivp_srm_modules] module
                                        ON ins.module_id = module.module_id AND module.module_id = {0} AND ins.is_active = 1
	                                    ORDER BY 1", moduleID);

            ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    workflow = new CommonMigrationSelectionInfo();
                    workflow.Text = Convert.ToString(row["Text"]);
                    workflow.Value = Convert.ToInt32(row["Value"]);
                    lstSelectableworkflows.Add(workflow);
                });
            }

            return lstSelectableworkflows;
        }

        private ObjectSet RenameWorkflowConfigurationTables(ObjectSet configDS)
        {
            try
            {
                mLogger.Debug("WorkflowController::RenameWorkflowConfigurationTables - START");
                if (configDS != null && configDS.Tables.Count > 0)
                {
                    foreach (ObjectRow dr in configDS.Tables[0].Rows)
                    {
                        int index = dr.Field<int>("id");
                        string configName = dr.Field<string>("configName");
                        configDS.Tables[index].TableName = configName;
                    }

                    configDS.Tables.RemoveAt(0);
                }
                return configDS;
            }
            catch (Exception ex)
            {
                mLogger.Error("WorkflowController::RenameWorkflowConfigurationTables- ERROR - " + ex.ToString());
            }
            finally
            {
                mLogger.Debug("WorkflowController::RenameWorkflowConfigurationTables - END");
                reInitializeVariables();
            }
            return null;
        }

        //Validation
        public void SRMWorkflowModeler_Sync(ObjectSet objSetFromDB, ObjectSet objSetDiff, int moduleId, string userName, out string errorMsg)
        {

            errorMsg = string.Empty;
            bool canInsert = (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflows)) ? (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Mapping) ? (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes) ? (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Template_Mapping) ? (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Data_Validation_Checks) ? (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Email_Configuration) ? true : false) : false) : false) : false) : false) : false;

            bool sheet4Exists = objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Template_Mapping) ? true : false;
            bool sheet5Exists = objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Data_Validation_Checks) ? true : false;

            mLogger.Debug("WorkflowController -> SRMWorkflowModeler_Sync -> Start");
            try
            {
                if (sheet4Exists && !sheet5Exists)
                    throw new Exception(SRM_WorkFlow_SheetNames.Data_Validation_Checks + " sheet is also required to process workflow rules.");
                else if (!sheet4Exists && sheet5Exists)
                    throw new Exception(SRM_WorkFlow_SheetNames.Workflow_Template_Mapping + " sheet is also required to process workflow rules.");

                #region COLLECTION GENERATION  FOR VALIDATION
                fillDictionaryForValidation(objSetFromDB, moduleId, userName);
                #endregion

                #region Sheet1

                if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflows))
                {
                    foreach (ObjectRow row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflows].Rows)
                    {
                        //Already in Sync 
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                            continue;

                        WorkflowInfo obj = new WorkflowInfo();

                        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                        string workflowType = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Type]).ToLower().Trim();
                        string sheetName = SRM_WorkFlow_SheetNames.Workflows;
                        string remarks = string.Empty;

                        // If Object row's sync status is Failed.
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                        {
                            //remarks = Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim();
                            addErrorToRow(row, sheetName, workflowName, remarks);
                        }
                        else
                        {
                            // If Object row's Workflow Type is valid.
                            if (!dicWorkflowActionTypeNameVsID.ContainsKey(workflowType))
                            {
                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Workflow_Type_Name);
                            }
                            else if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("update", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // Workflow update case.   
                                if (pendingRequestWorkflows.Contains(workflowName, StringComparer.OrdinalIgnoreCase))
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Has_Pending_Request);
                                }
                                else
                                {
                                    if (dictDB_WorkflowNameVWorkflowInfo[workflowName].WorkflowActionTypeName.Equals(workflowType, StringComparison.OrdinalIgnoreCase))
                                    {
                                        dictDB_WorkflowNameVWorkflowInfo[workflowName].WorkflowActionTypeName = workflowType;
                                        dictDB_WorkflowNameVWorkflowInfo[workflowName].WorkflowActionTypeId = dicWorkflowActionTypeNameVsID[workflowType];
                                        validWorkflowsSyncSet.Add(workflowName);
                                    }
                                    else
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Type_Cannot_Be_Changed_For_Existing_Workflow);
                                    }
                                }
                            }
                            // Workflow insert case.
                            else if (canInsert && row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                if (workFlowNameDBSet.ContainsKey(workflowName))
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Name_Already_Exists);
                                }
                                else
                                {
                                    obj.isSave = true;
                                    obj.ModuleId = moduleId;
                                    obj.WorkflowActionTypeName = workflowType;
                                    obj.WorkflowActionTypeId = dicWorkflowActionTypeNameVsID[workflowType];
                                    obj.UserName = userName;
                                    dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName] = obj;
                                }
                            }
                            else
                            {
                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                            }
                        }
                    }

                }
                #endregion

                #region Sheet2
                if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Mapping))
                {
                    #region VALIDATION AND CONSTRUCTION OF CUSTOM DICTIONARY FOR FURTHER VALIDATION
                    foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflow_Mapping].Rows)
                    {
                        //Already in Sync 
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                        string sheetName = SRM_WorkFlow_SheetNames.Workflow_Mapping;
                        WorkflowInfo dictionaryName = new WorkflowInfo();
                        string instrument_type = string.Empty;
                        string remarks = string.Empty;
                        bool hasError = false;

                        // If Object row's sync status is Failed.
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                        {
                            //remarks = Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim();
                            addErrorToRow(row, sheetName, workflowName, remarks);
                        }
                        else if (inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
                        {
                            addErrorToRow(row, sheetName, workflowName, SRMMigrationStatus.Not_Processed);
                        }
                        else
                        {
                            if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("update", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // Workflow update case.   
                                if (pendingRequestWorkflows.Contains(workflowName, StringComparer.OrdinalIgnoreCase))
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Has_Pending_Request);
                                    hasError = true;
                                }
                                else
                                {
                                    dictionaryName = dictDB_WorkflowNameVWorkflowInfo[workflowName];
                                    validWorkflowsSyncSet.Add(workflowName);
                                }
                            }
                            else if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // Workflow insert case.
                                if (canInsert)
                                {
                                    if (!dictDelta_ValidWorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                        hasError = true;
                                    }
                                    if (workFlowNameDBSet.ContainsKey(workflowName))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Name_Already_Exists);
                                        hasError = true;
                                    }
                                    else
                                    {
                                        if (!hasError)
                                            dictionaryName = dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName];
                                    }
                                }
                                else
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                    hasError = true;
                                }
                            }

                            //Process row if no error has occured
                            if (!hasError && (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName)))
                            {
                                string workflowTypeName = dictionaryName.WorkflowActionTypeName.Trim().ToLower();
                                string key;

                                if (dictionaryName.TypeIds == null)
                                    dictionaryName.TypeIds = new List<string>();

                                if (moduleId == 3)
                                {
                                    instrument_type = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Security_Type]).Trim().ToLower();
                                    key = instrument_type + "<$>" + workflowTypeName;

                                    if (sectypeTypeVsAttributesInfo.ContainsKey(instrument_type))
                                    {
                                        //Make [InstrumentType<$>WorkflowTypeVSWorkflowNames] dictionary
                                        if (dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames.ContainsKey(key))
                                            dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[key].Add(workflowName);
                                        else
                                        {
                                            dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames.Add(key, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                                            dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[key].Add(workflowName);
                                        }



                                        //dictionaryName.TypeIds.Add(Convert.ToString(sectypeTypeVsAttributesInfo[instrument_type].SectypeId));
                                        dictionaryName.TypeName = instrument_type;
                                        //dictionaryName.TypeName = string.IsNullOrEmpty(dictionaryName.TypeName) ? instrument_type : dictionaryName.TypeName + "|" + instrument_type;

                                        if (!workflowNameVsTypeId.ContainsKey(workflowName))
                                            workflowNameVsTypeId.Add(workflowName, new List<string> { Convert.ToString(sectypeTypeVsAttributesInfo[instrument_type].SectypeId) });
                                        else
                                            workflowNameVsTypeId[workflowName].Add(Convert.ToString(sectypeTypeVsAttributesInfo[instrument_type].SectypeId));

                                    }
                                    else
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Security_Type_Mapped + ": '" + instrument_type + "'");
                                        workflowNameVsTypeId.Remove(workflowName);
                                        hasError = true;
                                    }

                                }
                                // If module is other than SecMaster
                                else if (moduleId != 3)
                                {
                                    instrument_type = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Entity_Type]).Trim().ToLower();
                                    key = instrument_type + "<$>" + workflowTypeName;

                                    if (entityTypeVsAttributesInfo.ContainsKey(instrument_type))
                                    {
                                        //Make [InstrumentType<$>WorkflowTypeVSWorkflowNames] dictionary
                                        if (dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames.ContainsKey(key))
                                            dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[key].Add(workflowName);
                                        else
                                        {
                                            dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames.Add(key, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                                            dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[key].Add(workflowName);
                                        }

                                        //dictionaryName.TypeIds.Add(Convert.ToString(entityTypeVsAttributesInfo[instrument_type].EntityTypeID));
                                        dictionaryName.TypeName = instrument_type;
                                        //dictionaryName.TypeName = string.IsNullOrEmpty(dictionaryName.TypeName) ? instrument_type : dictionaryName.TypeName + "|" + instrument_type;

                                        if (!workflowNameVsTypeId.ContainsKey(workflowName))
                                            workflowNameVsTypeId.Add(workflowName, new List<string> { Convert.ToString(entityTypeVsAttributesInfo[instrument_type].EntityTypeID) });
                                        else
                                            workflowNameVsTypeId[workflowName].Add(Convert.ToString(entityTypeVsAttributesInfo[instrument_type].EntityTypeID));

                                    }
                                    else
                                    {   //Invalid Entity Type
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Entity_Type_Mapped + ": '" + instrument_type + "'");
                                        workflowNameVsTypeId.Remove(workflowName);
                                        hasError = true;
                                    }
                                }

                                //Remove all workflows from dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames
                                if (hasError)
                                {
                                    foreach (var item in dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames)
                                    {
                                        string instrumentType = item.Key;
                                        if (dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[instrumentType].Contains(workflowName))
                                            dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[instrumentType].Remove(workflowName);
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region EXCEL COLLECTION
                    //Append Existing Workflow For Sectypes 
                    foreach (var item in dictDB_WorkflowNameVSInstrumentTypeNames)
                    {
                        string workflowName = item.Key;
                        string workflowTypeName = string.Empty;
                        string instrumentTypeName = string.Empty;

                        if (!workflowNameVsTypeId.ContainsKey(workflowName))
                        {
                            foreach (var instrument_workflowType_Key in dictDB_WorkflowNameVSInstrumentTypeNames[workflowName])
                            {
                                if (dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames.ContainsKey(instrument_workflowType_Key))
                                    dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[instrument_workflowType_Key].Add(workflowName);
                            }
                        }
                    }
                    #endregion

                    #region VALIDATION THROUGH ABOVE DICTIONARY
                    HashSet<string> workflowNameSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    //Proccess sheet again for errors
                    foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflow_Mapping].Rows)
                    {
                        //Already in Sync 
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                        string sheetName = SRM_WorkFlow_SheetNames.Workflow_Mapping;
                        WorkflowInfo dictionaryName = new WorkflowInfo();
                        string instrument_type = string.Empty;
                        string remarks = string.Empty;
                        bool hasError = false;

                        // If Object row's sync status is Failed.
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                        {
                            remarks = Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim();
                            //addErrorToRow(row, sheetName, workflowName, remarks);
                        }
                        else if (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
                        {
                            if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("update", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                dictionaryName = dictDB_WorkflowNameVWorkflowInfo[workflowName];
                            }
                            else if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // Workflow insert case.
                                dictionaryName = dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName];
                            }


                            string workflowTypeName = dictionaryName.WorkflowActionTypeName.Trim().ToLower();
                            string key;

                            if (moduleId == 3)
                            {
                                instrument_type = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Security_Type]).Trim().ToLower();
                                key = instrument_type + "<$>" + workflowTypeName;

                                if (dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[key].Count() > 1)
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Security_Type_Mapped_In_More_Than_One_Workflow + ": '" + instrument_type + "'");
                                    hasError = true;
                                }
                                if (!workflowNameSet.Contains(workflowName))
                                {
                                    workflowNameSet.Add(workflowName);
                                    dictionaryName.TypeIds = workflowNameVsTypeId[workflowName];
                                }

                            }
                            // If module is other than SecMaster
                            else if (moduleId != 3)
                            {
                                instrument_type = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Entity_Type]).Trim().ToLower();
                                key = instrument_type + "<$>" + workflowTypeName;

                                if (dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[key].Count() > 1)
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Entity_Type_Mapped_In_More_Than_One_Workflow + ": '" + instrument_type + "'");
                                    hasError = true;
                                }

                                if (!workflowNameSet.Contains(workflowName))
                                {
                                    workflowNameSet.Add(workflowName);
                                    dictionaryName.TypeIds = workflowNameVsTypeId[workflowName];
                                }

                            }

                            if (hasError)
                            {
                                foreach (var item in dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames)
                                {
                                    string instrumentType = item.Key;
                                    if (dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[instrumentType].Contains(workflowName))
                                        dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[instrumentType].Remove(workflowName);
                                }
                            }

                        }
                    }
                    #endregion

                }
                #endregion

                #region Sheet3
                if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes))
                {

                    #region VALIDATION AND CONSTRUCTION OF CUSTOM DICTIONARY FOR FURTHER VALIDATION
                    foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes].Rows)
                    {
                        ////Already in Sync 
                        //if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                        //    continue;

                        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                        string sheetName = SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes;
                        string attributeName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Attribute_Name]).ToLower().Trim();
                        string isPrimary = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Is_Primary_Attribute]).ToLower().Trim();
                        WorkflowInfo dictionaryName = new WorkflowInfo();
                        string instrument_type = string.Empty;
                        string remarks = string.Empty;
                        bool hasError = false;

                        // If Object row's sync status is Failed.
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                        {
                            //remarks = Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim();
                            addErrorToRow(row, sheetName, workflowName, remarks);
                        }
                        else if (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
                        {
                            if (dictDB_WorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                            {
                                // Workflow update case.   
                                if (pendingRequestWorkflows.Contains(workflowName, StringComparer.OrdinalIgnoreCase))
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Has_Pending_Request);
                                    hasError = true;
                                }
                                else
                                {
                                    dictionaryName = dictDB_WorkflowNameVWorkflowInfo[workflowName];
                                    validWorkflowsSyncSet.Add(workflowName);
                                }
                            }
                            else if (dictDelta_ValidWorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                            {
                                // Workflow insert case.
                                if (canInsert)
                                {
                                    if (!dictDelta_ValidWorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                        hasError = true;
                                    }
                                    if (workFlowNameDBSet.ContainsKey(workflowName))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Name_Already_Exists);
                                        hasError = true;
                                    }
                                    else
                                    {
                                        if (!hasError)
                                            dictionaryName = dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName];
                                    }
                                }
                                else
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                    hasError = true;
                                }
                            }
                            else
                            {
                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                hasError = true;
                            }

                            if (!hasError && !inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
                            {
                                int typeId = dictionaryName.TypeIds.Count;
                                string typeName = string.Empty;

                                if (typeId == 1)
                                    typeName = dictionaryName.TypeName.Trim();

                                //SecMaster Module
                                if (moduleId == 3)
                                {
                                    if (typeId == 1)
                                    {
                                        //Check in Sectype Specific Attrs
                                        if (sectypeTypeVsAttributesInfo.ContainsKey(typeName))
                                        {
                                            if (sectypeTypeVsAttributesInfo[typeName].AttributeInfo.MasterAttrs.ContainsKey(attributeName))
                                            {
                                                //add here	
                                                if (!WorkflowNameVsAttributes.ContainsKey(workflowName))
                                                {
                                                    WorkflowNameVsAttributes.Add(workflowName, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                                                }
                                                if (!WorkflowNameVsAttributes[workflowName].Contains(attributeName))
                                                {
                                                    WorkflowNameVsAttributes[workflowName].Add(attributeName);
                                                }
                                                if (Convert.ToBoolean(isPrimary))
                                                {
                                                    string primaryAttrID = Convert.ToString(sectypeTypeVsAttributesInfo[typeName].AttributeInfo.MasterAttrs[attributeName].AttrId);

                                                    if (!workflowNameVsAttribute.ContainsKey(workflowName))
                                                    {
                                                        workflowNameVsAttribute.Add(workflowName, new Dictionary<string, List<string>>());
                                                        workflowNameVsAttribute[workflowName].Add("Primary", new List<string> { primaryAttrID });
                                                    }
                                                    else
                                                    {
                                                        if (!workflowNameVsAttribute[workflowName].ContainsKey("Primary"))
                                                            workflowNameVsAttribute[workflowName].Add("Primary", new List<string> { primaryAttrID });
                                                        else
                                                            workflowNameVsAttribute[workflowName]["Primary"].Add(primaryAttrID);
                                                    }
                                                }
                                                else
                                                {
                                                    string otherAttrID = Convert.ToString(sectypeTypeVsAttributesInfo[typeName].AttributeInfo.MasterAttrs[attributeName].AttrId);

                                                    if (!workflowNameVsAttribute.ContainsKey(workflowName))
                                                    {
                                                        workflowNameVsAttribute.Add(workflowName, new Dictionary<string, List<string>>());
                                                        workflowNameVsAttribute[workflowName].Add("Other", new List<string> { otherAttrID });
                                                    }
                                                    else
                                                    {
                                                        if (!workflowNameVsAttribute[workflowName].ContainsKey("Other"))
                                                            workflowNameVsAttribute[workflowName].Add("Other", new List<string> { otherAttrID });
                                                        else
                                                            workflowNameVsAttribute[workflowName]["Other"].Add(otherAttrID);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //SecType Attribute not present
                                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Attribute_Is_Mapped_For + ": '" + typeName + "'");
                                                hasError = true;
                                            }
                                        }
                                        else
                                        {
                                            //SecType Name not found
                                            addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Security_Type_Mapped);
                                            hasError = true;
                                        }
                                    }
                                    else
                                    {
                                        //Check in Common Attrs
                                        if (commonAttributeNameVsInfo.ContainsKey(attributeName))
                                        {
                                            //add here	
                                            if (!WorkflowNameVsAttributes.ContainsKey(workflowName))
                                            {
                                                WorkflowNameVsAttributes.Add(workflowName, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                                            }
                                            if (!WorkflowNameVsAttributes[workflowName].Contains(attributeName))
                                            {
                                                WorkflowNameVsAttributes[workflowName].Add(attributeName);
                                            }
                                            if (Convert.ToBoolean(isPrimary))
                                            {
                                                string primaryAttrID = Convert.ToString(commonAttributeNameVsInfo[attributeName].AttrId);

                                                if (!workflowNameVsAttribute.ContainsKey(workflowName))
                                                {
                                                    workflowNameVsAttribute.Add(workflowName, new Dictionary<string, List<string>>());
                                                    workflowNameVsAttribute[workflowName].Add("Primary", new List<string> { primaryAttrID });
                                                }
                                                else
                                                {
                                                    if (!workflowNameVsAttribute[workflowName].ContainsKey("Primary"))
                                                        workflowNameVsAttribute[workflowName].Add("Primary", new List<string> { primaryAttrID });
                                                    else
                                                        workflowNameVsAttribute[workflowName]["Primary"].Add(primaryAttrID);
                                                }
                                            }
                                            else
                                            {
                                                string otherAttrID = Convert.ToString(commonAttributeNameVsInfo[attributeName].AttrId);

                                                if (!workflowNameVsAttribute.ContainsKey(workflowName))
                                                {
                                                    workflowNameVsAttribute.Add(workflowName, new Dictionary<string, List<string>>());
                                                    workflowNameVsAttribute[workflowName].Add("Other", new List<string> { otherAttrID });
                                                }
                                                else
                                                {
                                                    if (!workflowNameVsAttribute[workflowName].ContainsKey("Other"))
                                                        workflowNameVsAttribute[workflowName].Add("Other", new List<string> { otherAttrID });
                                                    else
                                                        workflowNameVsAttribute[workflowName]["Other"].Add(otherAttrID);
                                                }

                                            }
                                        }
                                        else
                                        {
                                            //Common Attribute not present
                                            addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Common_Attribute_Is_Mapped);
                                            hasError = true;
                                        }

                                    }
                                }
                                //Modules Other Than SecMaster
                                else
                                {
                                    if (typeId == 1)
                                    {
                                        //Check in Entity Type Specific Attrs
                                        if (entityTypeVsAttributesInfo.ContainsKey(typeName))
                                        {
                                            if (entityTypeVsAttributesInfo[typeName].Attributes.ContainsKey(attributeName))
                                            {
                                                if (!WorkflowNameVsAttributes.ContainsKey(workflowName))
                                                {
                                                    WorkflowNameVsAttributes.Add(workflowName, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                                                }
                                                if (!WorkflowNameVsAttributes[workflowName].Contains(attributeName))
                                                {
                                                    WorkflowNameVsAttributes[workflowName].Add(attributeName);
                                                }
                                                if (Convert.ToBoolean(isPrimary))
                                                {
                                                    string primaryAttrID = Convert.ToString(entityTypeVsAttributesInfo[typeName].Attributes[attributeName].EntityAttributeID);

                                                    if (!workflowNameVsAttribute.ContainsKey(workflowName))
                                                    {
                                                        workflowNameVsAttribute.Add(workflowName, new Dictionary<string, List<string>>());
                                                        workflowNameVsAttribute[workflowName].Add("Primary", new List<string> { primaryAttrID });
                                                    }
                                                    else
                                                    {
                                                        if (!workflowNameVsAttribute[workflowName].ContainsKey("Primary"))
                                                            workflowNameVsAttribute[workflowName].Add("Primary", new List<string> { primaryAttrID });
                                                        else
                                                            workflowNameVsAttribute[workflowName]["Primary"].Add(primaryAttrID);
                                                    }
                                                }
                                                else
                                                {
                                                    string otherAttrID = Convert.ToString(entityTypeVsAttributesInfo[typeName].Attributes[attributeName].EntityAttributeID);

                                                    if (!workflowNameVsAttribute.ContainsKey(workflowName))
                                                    {
                                                        workflowNameVsAttribute.Add(workflowName, new Dictionary<string, List<string>>());
                                                        workflowNameVsAttribute[workflowName].Add("Other", new List<string> { otherAttrID });
                                                    }
                                                    else
                                                    {
                                                        if (!workflowNameVsAttribute[workflowName].ContainsKey("Other"))
                                                            workflowNameVsAttribute[workflowName].Add("Other", new List<string> { otherAttrID });
                                                        else
                                                            workflowNameVsAttribute[workflowName]["Other"].Add(otherAttrID);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //SecType Attribute not present
                                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Attribute_Is_Mapped_For + ": '" + typeName + "'");
                                                hasError = true;
                                            }

                                        }
                                        else
                                        {
                                            //Entity Type Attribute not present
                                            addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Entity_Type_Mapped + ": '" + typeName + "'");
                                        }
                                    }
                                    else
                                    {
                                        //Mapped to more than one entity type
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_can_be_mapped_to_only_one_Instrument_Type);
                                    }
                                }
                            }
                        }
                        //Remove from collection, if error
                        if (hasError)
                        {
                            workflowNameVsTypeId.Remove(workflowName);
                        }
                    }
                    #endregion

                    #region VALIDATION THROUGH ABOVE DICTIONARY

                    foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes].Rows)
                    {

                        //Already in Sync 
                        //if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                        //    continue;

                        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                        string sheetName = SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes;
                        WorkflowInfo dictionaryName = new WorkflowInfo();
                        string remarks = string.Empty;
                        bool hasError = false;

                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                        {
                            remarks = Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim();
                            //addErrorToRow(row, sheetName, workflowName, remarks);
                        }
                        else if (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName) && workflowNameVsAttribute.ContainsKey(workflowName))
                        {
                            //if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("update", StringComparison.OrdinalIgnoreCase) != -1)
                            //{
                            //    dictionaryName = dictDB_WorkflowNameVWorkflowInfo[workflowName];
                            //}
                            //else if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1)
                            //{
                            //    // Workflow insert case.
                            //    dictionaryName = dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName];
                            //}

                            if (dictDB_WorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                            {
                                // Workflow update case.   
                                if (pendingRequestWorkflows.Contains(workflowName, StringComparer.OrdinalIgnoreCase))
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Has_Pending_Request);
                                    hasError = true;
                                }
                                else
                                {
                                    dictionaryName = dictDB_WorkflowNameVWorkflowInfo[workflowName];
                                    validWorkflowsSyncSet.Add(workflowName);
                                }
                            }
                            else if (dictDelta_ValidWorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                            {
                                // Workflow insert case.
                                if (canInsert)
                                {
                                    if (!dictDelta_ValidWorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                        hasError = true;
                                    }
                                    if (workFlowNameDBSet.ContainsKey(workflowName))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Name_Already_Exists);
                                        hasError = true;
                                    }
                                    else
                                    {
                                        if (!hasError)
                                            dictionaryName = dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName];
                                    }
                                }
                                else
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                    hasError = true;
                                }
                            }
                            else
                            {
                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                hasError = true;
                            }

                            if (!hasError && !inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
                            {

                                if (!workflowNameVsAttribute[workflowName].ContainsKey("Primary"))
                                    workflowNameVsAttribute[workflowName].Add("Primary", new List<string>());
                                if (!workflowNameVsAttribute[workflowName].ContainsKey("Other"))
                                    workflowNameVsAttribute[workflowName].Add("Other", new List<string>());

                                List<string> primaryAttr = workflowNameVsAttribute[workflowName]["Primary"];
                                List<string> otherAttr = workflowNameVsAttribute[workflowName]["Other"];


                                if (primaryAttr.Count() > 1)
                                {
                                    remarks += SRMMigrationSeparators.Remarks_Separator + SRMErrorMessages.More_Than_One_Primary_Attribute_Are_Mapped;
                                    hasError = true;
                                }
                                else if (workflowNameVsAttribute[workflowName]["Primary"].Count() == 0)
                                {
                                    remarks += SRMMigrationSeparators.Remarks_Separator + SRMErrorMessages.No_Primary_Attribute_Is_Mapped;
                                    hasError = true;
                                }

                                if (workflowNameVsAttribute[workflowName]["Other"].Count() > 4)
                                {
                                    remarks += SRMMigrationSeparators.Remarks_Separator + SRMErrorMessages.More_Than_Four_Attribute_Are_Mapped_In_Other_Display_Attributes;
                                    hasError = true;
                                }

                                if (!hasError && primaryAttr.Count == 1 && otherAttr.Count > 0)
                                {
                                    foreach (var attrID in otherAttr)
                                    {
                                        if (attrID.Trim().Equals(Convert.ToString(primaryAttr[0]), StringComparison.OrdinalIgnoreCase))
                                            hasError = true;
                                    }

                                    if (hasError)
                                        remarks += SRMMigrationSeparators.Remarks_Separator + SRMErrorMessages.Same_Attribute_Cannot_Be_Selected_As_Primary_Display_Attribute_And_Other_Display_Attributes;

                                }

                                if (hasError)
                                    addErrorToRow(row, sheetName, workflowName, remarks);
                                else
                                {
                                    dictionaryName.PrimaryAttributeIds = workflowNameVsAttribute[workflowName]["Primary"];
                                    dictionaryName.OtherAttributeIds = workflowNameVsAttribute[workflowName]["Other"];

                                    // Workflow Proccessed Successfully
                                    workflowNameVsAttribute.Remove(workflowName);
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region Sheet4
                if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Template_Mapping))
                {
                    #region VALIDATION AND CONSTRUCTION OF CUSTOM DICTIONARY FOR FURTHER VALIDATION
                    foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflow_Template_Mapping].Rows)
                    {

                        //Already in Sync 
                        //if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                        //    continue;

                        bool isDefaultSet = false;
                        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                        WorkflowInfo dictionaryName = new WorkflowInfo();
                        string sheetName = SRM_WorkFlow_SheetNames.Workflow_Template_Mapping;
                        bool hasError = false;
                        string remarks = string.Empty;


                        // If Object row's sync status is Failed.
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                        {
                            //remarks = Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim();
                            addErrorToRow(row, sheetName, workflowName, remarks);
                        }
                        else if (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
                        {

                            if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("update", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // Workflow update case.   
                                if (pendingRequestWorkflows.Contains(workflowName, StringComparer.OrdinalIgnoreCase))
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Has_Pending_Request);
                                    hasError = true;
                                }
                                else
                                {
                                    dictionaryName = dictDB_WorkflowNameVWorkflowInfo[workflowName];
                                    validWorkflowsSyncSet.Add(workflowName);
                                }
                            }
                            else if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // Workflow insert case.
                                if (canInsert)
                                {
                                    if (!dictDelta_ValidWorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                        hasError = true;
                                    }
                                    if (workFlowNameDBSet.ContainsKey(workflowName))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Name_Already_Exists);
                                        hasError = true;
                                    }
                                    else
                                    {
                                        if (!hasError)
                                            dictionaryName = dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName];
                                    }
                                }
                                else
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                    hasError = true;
                                }
                            }
                        }

                        //Process row if no error has occured
                        if (!hasError && (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName)))
                        {
                            string is_Rule_Configured = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Is_Rule_Configured]).Trim();
                            string is_Default_Workflow = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Is_Default_Workflow]).Trim();
                            string radTemplateName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rad_Workflow_Template_Name]).Trim();
                            string ruleText = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rule_Text]).Trim();
                            string priority = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rule_Priority]).Trim();

                            if (radWorkflowsNameVsId.ContainsKey(radTemplateName))
                            {
                                #region Fill dicitonary of radTemplateVsStates

                                if (!dict_RadTemplateVSStates.ContainsKey(radTemplateName))
                                {
                                    var workflowInfo = new RWorkFlowService().GetWorkflowInfo(radTemplateName);
                                    dynamic parsedworkflowInfo = JObject.Parse(workflowInfo);
                                    HashSet<string> hashRADTemplateStage = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                                    foreach (var stateInfo in parsedworkflowInfo.WorkflowStates)
                                    {
                                        string stateName = Convert.ToString(stateInfo.StateName);

                                        bool toSkipState = false;
                                        switch (stateName.ToUpper())
                                        {
                                            case "END":
                                            case "FAILED":
                                                toSkipState = true;
                                                break;
                                            default:
                                                break;
                                        }
                                        if (!toSkipState)
                                        {
                                            hashRADTemplateStage.Add(stateName);
                                        }
                                    }

                                    dict_RadTemplateVSStates.Add(radTemplateName, hashRADTemplateStage);
                                }
                                #endregion

                                if (!dict_RadTemplateVSActions.ContainsKey(radTemplateName))
                                {
                                    RWorkFlowInfo workflowInfo = new RWorkFlowService().GetWorkflowInfoByIdorName(0, radTemplateName);
                                    HashSet<string> hashRADTemplateAction = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                    hashRADTemplateAction.Add("Creator initiates the request");
                                    foreach (WorkflowState state in workflowInfo.WorkflowStates)
                                    {
                                        foreach (StateActionInfo action in state.StateActions)
                                        {
                                            hashRADTemplateAction.Add(state.StateName + " - " + action.Action);
                                        }
                                    }
                                    hashRADTemplateAction.Add("Creator cancels the request");

                                    dict_RadTemplateVSActions.Add(radTemplateName, hashRADTemplateAction);
                                }




                                //WorkflowRulesInfo rulesObj = new WorkflowRulesInfo();

                                if (!is_Rule_Configured.Equals(is_Default_Workflow, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!isDefaultSet && is_Default_Workflow.Equals("yes", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (string.IsNullOrEmpty(ruleText) && string.IsNullOrEmpty(priority))
                                        {
                                            //rulesObj.SRMWorkFlowIsDefault = true;
                                            //rulesObj.SRMWorkFlowRuleText = ruleText;
                                            //rulesObj.WorkflowRuleSetID = "-1";
                                            //rulesObj.WorkFlowRulePriority = "-1";
                                            //rulesObj.RadWorkFlowInstanceName = radTemplateName;
                                            //rulesObj.RadWorkFlowInstanceID = Convert.ToString(radWorkflowsNameVsId[radTemplateName]);
                                            isDefaultSet = true;

                                            //dictionaryName.WorkflowRulesInfo.Add(rulesObj);
                                        }
                                        else
                                        {
                                            addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Rule_Text_And_Priority_Cannot_Be_Assigned_To_Default_Rule);
                                            hasError = true;
                                        }

                                    }
                                    else if (is_Default_Workflow.Equals("no", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (!string.IsNullOrEmpty(priority))
                                        {
                                            int ValidPriority;
                                            if (Int32.TryParse(priority, out ValidPriority))
                                            {
                                                if (ValidPriority > 0)
                                                {
                                                    if (!string.IsNullOrEmpty(ruleText))
                                                    {
                                                        //rulesObj.SRMWorkFlowIsDefault = false;
                                                        //rulesObj.SRMWorkFlowRuleText = ruleText;
                                                        //rulesObj.WorkflowRuleSetID = "";
                                                        //rulesObj.WorkFlowRulePriority = priority;
                                                        //rulesObj.RadWorkFlowInstanceName = radTemplateName;
                                                        //rulesObj.RadWorkFlowInstanceID = Convert.ToString(radWorkflowsNameVsId[radTemplateName]);

                                                        //dictionaryName.WorkflowRulesInfo.Add(rulesObj);
                                                    }
                                                    else
                                                    {
                                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Rule_Text_Cannot_Be_Empty);
                                                        hasError = true;
                                                    }
                                                }
                                                else
                                                {
                                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Priority_Should_be_Greater_Than_Zero);
                                                    hasError = true;
                                                }
                                            }
                                            else
                                            {
                                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Incorrect_Priority);
                                                hasError = true;
                                            }

                                        }
                                        else
                                        {   //Priority is not given.
                                            addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Priority_Can_Not_Be_Empty);
                                            hasError = true;
                                        }
                                    }
                                    else
                                    {
                                        //Default rule is already set.
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Default_Rule_Is_Already_Set);
                                        hasError = true;
                                    }
                                }
                                else
                                {
                                    //Is_Default_Workflow and Is_Rule_Configured can't be yes/no simultaneously.
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Is_Default_Workflow_And_Is_Rule_Configured_Error);
                                    hasError = true;
                                }
                            }
                            else
                            {   //Invalid RAD template
                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_RAD_Template_Is_Mapped);
                                hasError = true;
                            }

                            //Add RuleInfoObj into workflowNameVsRuleInfo Collection 
                            if (!hasError && (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName)))
                            {
                                if (!dict_WorkflowNameVSRuleInfo.ContainsKey(workflowName))
                                {
                                    dict_WorkflowNameVSRuleInfo.Add(workflowName, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                                    dict_WorkflowNameVSRuleInfo[workflowName].Add(is_Default_Workflow + "<$>" + ruleText + "<$>" + (is_Default_Workflow.Equals("yes", StringComparison.OrdinalIgnoreCase) ? "-1" : priority) + "<$>" + radTemplateName);
                                }
                                else
                                    dict_WorkflowNameVSRuleInfo[workflowName].Add(Convert.ToString(is_Default_Workflow) + "<$>" + ruleText + "<$>" + (is_Default_Workflow.Equals("yes", StringComparison.OrdinalIgnoreCase) ? "-1" : priority) + "<$>" + radTemplateName);


                                if (!dict_WorkflowNameVSRuleInfoForActions.ContainsKey(workflowName))
                                {
                                    dict_WorkflowNameVSRuleInfoForActions.Add(workflowName, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                                    if (is_Default_Workflow.Equals("yes", StringComparison.OrdinalIgnoreCase))
                                    {
                                        string priorityInternal = "-1";
                                        dict_WorkflowNameVSRuleInfoForActions[workflowName].Add(priorityInternal);
                                    }
                                    else
                                        dict_WorkflowNameVSRuleInfoForActions[workflowName].Add(priority);
                                }
                                else
                                    dict_WorkflowNameVSRuleInfoForActions[workflowName].Add(priority);
                            }

                        }
                    }
                    #endregion

                    #region VALIDATION THROUGH ABOVE DICTIONARY
                    foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflow_Template_Mapping].Rows)
                    {

                        //Already in Sync 
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                        string sheetName = SRM_WorkFlow_SheetNames.Workflow_Template_Mapping;
                        HashSet<string> prioritySet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        bool hasError = false, isDefaultSet = false;
                        string remarks = string.Empty;

                        // If Object row's sync status is Failed.
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                        {
                            remarks = Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim();
                            //addErrorToRow(row, sheetName, workflowName, remarks);
                        }
                        else if (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
                        {
                            foreach (var ruleInfo in dict_WorkflowNameVSRuleInfo[workflowName])
                            {
                                string[] ruleInfoSplit = ruleInfo.Split(new string[] { "<$>" }, StringSplitOptions.None);
                                bool isDefault = ruleInfoSplit[0].Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false;
                                string priority = ruleInfoSplit[2];

                                //Check for multiple default rules
                                if (isDefault && !isDefaultSet)
                                    isDefaultSet = true;
                                else
                                {
                                    if (isDefaultSet && isDefault)
                                    {
                                        row[SRMSpecialColumnNames.Remarks] += SRMMigrationSeparators.Remarks_Separator + SRMErrorMessages.Default_Rule_Is_Already_Set;
                                        hasError = true;
                                    }
                                }

                                //Check for multiple rules with same priority 
                                if (!prioritySet.Contains(priority))
                                    prioritySet.Add(priority);
                                else
                                {
                                    row[SRMSpecialColumnNames.Remarks] += SRMMigrationSeparators.Remarks_Separator + SRMErrorMessages.Priority_Should_be_Unique;
                                    hasError = true;
                                }
                            }
                            if (hasError)
                                addErrorToRow(row, sheetName, workflowName, remarks, false);
                        }
                    }
                    #endregion
                }
                #endregion

                #region Sheet5
                if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Data_Validation_Checks))
                {
                    #region
                    foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Data_Validation_Checks].Rows)
                    {

                        //Already in Sync 
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                        string sheetName = SRM_WorkFlow_SheetNames.Data_Validation_Checks;
                        WorkflowInfo dictionaryName = new WorkflowInfo();
                        string remarks = string.Empty;
                        bool hasError = false;

                        // If Object row's sync status is Failed.
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                        {
                            //remarks = Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim();
                            addErrorToRow(row, sheetName, workflowName, remarks);
                        }
                        else if (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
                        {

                            if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("update", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // Workflow update case.   
                                if (pendingRequestWorkflows.Contains(workflowName, StringComparer.OrdinalIgnoreCase))
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Has_Pending_Request);
                                    hasError = true;
                                }
                                else
                                {
                                    dictionaryName = dictDB_WorkflowNameVWorkflowInfo[workflowName];
                                    validWorkflowsSyncSet.Add(workflowName);
                                }
                            }
                            else if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // Workflow insert case.
                                if (canInsert)
                                {
                                    if (!dictDelta_ValidWorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                        hasError = true;
                                    }
                                    if (workFlowNameDBSet.ContainsKey(workflowName))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Name_Already_Exists);
                                        hasError = true;
                                    }
                                    else
                                    {
                                        if (!hasError)
                                            dictionaryName = dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName];
                                    }
                                }
                                else
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                    hasError = true;
                                }
                            }
                        }

                        //Process row if no error has occured
                        if (!hasError && (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName)))
                        {
                            string is_Default_Workflow = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Is_Default_Workflow]).Trim();
                            string ruleText = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rule_Text]).Trim();
                            string priority = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rule_Priority]).Trim();
                            string radTemplateName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rad_Workflow_Template_Name]).Trim();

                            string keyInfo = is_Default_Workflow + "<$>" + ruleText + "<$>" + (is_Default_Workflow.Equals("yes", StringComparison.OrdinalIgnoreCase) ? "-1" : priority) + "<$>" + radTemplateName;


                            if (dict_RadTemplateVSStates.ContainsKey(radTemplateName))
                            {
                                RuleStateInfo info = new RuleStateInfo();

                                string stage = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Stage]);
                                if (stage.Equals("Workflow Initiation", StringComparison.OrdinalIgnoreCase))
                                    stage = "Start";
                                if (dict_RadTemplateVSStates[radTemplateName].Contains(stage))
                                {
                                    info.stateName = stage;
                                    info.mandatoryData = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Mandatory]).ToLower() == "yes" ? true : false;
                                    info.uniquenessValidation = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Uniqueness]).ToLower() == "yes" ? true : false;
                                    info.primaryKeyValidation = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Primary_Key]).ToLower() == "yes" ? true : false;
                                    info.validations = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Validations]).ToLower() == "yes" ? true : false;
                                    info.alerts = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Alerts]).ToLower() == "yes" ? true : false;

                                    if (moduleId == 3)
                                    {
                                        info.basketValidation = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Basket_Validations]).ToLower() == "yes" ? true : false;
                                        info.basketAlert = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Basket_Alerts]).ToLower() == "yes" ? true : false;
                                    }
                                    else
                                    {
                                        info.basketValidation = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Group_Validation]).ToLower() == "yes" ? true : false;
                                        info.basketAlert = false;
                                    }

                                    //Make custom dictionary
                                    if (dict_WorkflowNameVSRuleInfo[workflowName].Contains(keyInfo))
                                    {
                                        if (!workflowNameVsRuleInfoKeyVsStageDataSet.ContainsKey(workflowName))
                                        {
                                            // workflowNameVsRuleInfoKeyVsStageDataSet.Add(workflowName, new Dictionary<string, HashSet<RuleStateInfo>> { { keyInfo, new HashSet<RuleStateInfo> { info } } });
                                            workflowNameVsRuleInfoKeyVsStageDataSet.Add(workflowName, new Dictionary<string, HashSet<RuleStateInfo>>(StringComparer.OrdinalIgnoreCase));
                                            workflowNameVsRuleInfoKeyVsStageDataSet[workflowName].Add(keyInfo, new HashSet<RuleStateInfo> { info });
                                        }
                                        else
                                        {
                                            if (!workflowNameVsRuleInfoKeyVsStageDataSet[workflowName].ContainsKey(keyInfo))
                                                workflowNameVsRuleInfoKeyVsStageDataSet[workflowName].Add(keyInfo, new HashSet<RuleStateInfo> { info });
                                            else
                                                workflowNameVsRuleInfoKeyVsStageDataSet[workflowName][keyInfo].Add(info);
                                        }
                                    }
                                    else
                                    {
                                        addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Rule_Does_Not_Exist_In_Template_Mapping_Sheet);
                                        hasError = true;
                                    }
                                }
                                else
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Stage_For_RAD_Template);
                                    hasError = true;
                                }
                            }
                            else
                            {
                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_RAD_Template_Is_Mapped);
                                hasError = true;
                            }

                        }
                    }
                    #endregion

                    #region FILL COLLECTION TO SAVE/UPDATE 
                    foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Data_Validation_Checks].Rows)
                    {
                        //Already in Sync 
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                        string sheetName = SRM_WorkFlow_SheetNames.Data_Validation_Checks;
                        WorkflowInfo dictionaryName = new WorkflowInfo();
                        string instrument_type = string.Empty;
                        string remarks = string.Empty;
                        bool hasError = false;

                        // If Object row's sync status is Failed.
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                        {
                            remarks = Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim();
                            //addErrorToRow(row, sheetName, workflowName, remarks);
                        }
                        else if ((!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName)) && workflowNameVsRuleInfoKeyVsStageDataSet.ContainsKey(workflowName))
                        {
                            if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("update", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                dictionaryName = dictDB_WorkflowNameVWorkflowInfo[workflowName];
                            }
                            else if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // Workflow insert case.
                                dictionaryName = dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName];
                            }

                            List<WorkflowRulesInfo> WorkflowRulesInfoList = new List<WorkflowRulesInfo>();

                            foreach (var item in workflowNameVsRuleInfoKeyVsStageDataSet[workflowName])
                            {
                                WorkflowRulesInfo ruleInfo = new WorkflowRulesInfo();
                                string[] splitItems = item.Key.Split(new string[] { "<$>" }, StringSplitOptions.None);
                                string radTemplate = splitItems[3];
                                bool isValidNumberOfStates = dict_RadTemplateVSStates[radTemplate].Count == workflowNameVsRuleInfoKeyVsStageDataSet[workflowName][item.Key].Count() ? true : false;

                                if (isValidNumberOfStates)
                                {
                                    ruleInfo.SRMWorkFlowIsDefault = splitItems[0].Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false;
                                    ruleInfo.SRMWorkFlowRuleText = splitItems[0].Equals("yes", StringComparison.OrdinalIgnoreCase) ? "" : splitItems[1];
                                    ruleInfo.WorkflowRuleSetID = splitItems[0].Equals("yes", StringComparison.OrdinalIgnoreCase) ? "-1" : "";
                                    ruleInfo.WorkFlowRulePriority = splitItems[0].Equals("yes", StringComparison.OrdinalIgnoreCase) ? "-1" : splitItems[2];
                                    ruleInfo.RadWorkFlowInstanceName = splitItems[3];
                                    ruleInfo.RadWorkFlowInstanceID = Convert.ToString(radWorkflowsNameVsId[splitItems[3]]);
                                    ruleInfo.RuleStateInfo = workflowNameVsRuleInfoKeyVsStageDataSet[workflowName][item.Key].ToList();

                                    WorkflowRulesInfoList.Add(ruleInfo);
                                }
                                else
                                {
                                    remarks = SRMErrorMessages.RAD_Template_has_invalid_number_of_states;
                                    hasError = true;
                                    break;
                                }
                            }

                            if (hasError)
                                addErrorToRow(row, sheetName, workflowName, remarks);
                            else
                            {
                                dictionaryName.WorkflowRulesInfo = WorkflowRulesInfoList;
                                // Workflow Proccessed Successfully
                                workflowNameVsRuleInfoKeyVsStageDataSet.Remove(workflowName);
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region Sheet6
                if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Email_Configuration))
                {
                    #region
                    foreach (var wfName in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Email_Configuration].Rows.AsEnumerable().GroupBy(x => new { WorkflowName = Convert.ToString(x["Workflow Name"]), RulePriority = Convert.ToString(x["Rule Priority"]), ActionName = Convert.ToString(x["Action Name"]) }))
                    {
                        string actionName = wfName.Key.ActionName;
                        bool isMandatoryAttributesUploaded = true;
                        HashSet<string> dataSectionAttributesList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        HashSet<string> missingMandatoryAttributeList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var row in wfName)
                        {
                            string includeAction = Convert.ToString(row["Include Action"]);
                            if (includeAction.ToLower().Equals("yes"))
                            {
                                string dataSectionAttribute = Convert.ToString(row["Data Section Attributes"]);
                                dataSectionAttributesList.Add(dataSectionAttribute);
                            }
                        }
                        if (dataSectionAttributesList.Count > 0)
                        {
                            if (moduleId == 3)
                            {
                                if (!dataSectionAttributesList.Contains("Security Type", StringComparer.OrdinalIgnoreCase))
                                {
                                    missingMandatoryAttributeList.Add("Security Type");
                                    isMandatoryAttributesUploaded = false;
                                }
                                if (!dataSectionAttributesList.Contains("Security ID", StringComparer.OrdinalIgnoreCase))
                                {
                                    missingMandatoryAttributeList.Add("Security ID");
                                    isMandatoryAttributesUploaded = false;
                                }
                            }
                            else
                            {
                                if (!dataSectionAttributesList.Contains("Entity Type", StringComparer.OrdinalIgnoreCase))
                                {
                                    missingMandatoryAttributeList.Add("Entity Type");
                                    isMandatoryAttributesUploaded = false;
                                }
                                if (!dataSectionAttributesList.Contains("Entity Code", StringComparer.OrdinalIgnoreCase))
                                {
                                    missingMandatoryAttributeList.Add("Entity Code");
                                    isMandatoryAttributesUploaded = false;
                                }
                            }
                        }
                        if (missingMandatoryAttributeList.Count > 0)
                        {
                            foreach (var row in wfName)
                                addErrorToRow(row, SRM_WorkFlow_SheetNames.Email_Configuration, wfName.Key.WorkflowName, "Mandatory attributes - " + string.Join(",",missingMandatoryAttributeList) + " are not present", true);
                        }
                        if (isMandatoryAttributesUploaded)
                        {
                            foreach (var row in wfName)
                            {
                                string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                                string sheetName = SRM_WorkFlow_SheetNames.Email_Configuration;
                                if (WorkflowNameVsAttributes.ContainsKey(workflowName))
                                {
                                    string dataSectionAttribute = Convert.ToString(row["Data Section Attributes"]);
                                    HashSet<string> availableAttributes = new HashSet<string>(WorkflowNameVsAttributes[workflowName], StringComparer.OrdinalIgnoreCase);
                                    HashSet<string> allowedCustomAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                    foreach (string actionSubname in actionNameVsAllowedCustomAttributes.Keys)
                                    {
                                        if (actionName.ToLower().Contains(actionSubname.ToLower()))
                                        {
                                            allowedCustomAttributes = actionNameVsAllowedCustomAttributes[actionSubname];
                                            break;
                                        }
                                    }
                                    availableAttributes.UnionWith(allowedCustomAttributes);
                                    string includeAction = Convert.ToString(row["Include Action"]);
                                    if (!includeAction.ToLower().Equals("no") && !availableAttributes.Contains(dataSectionAttribute))
                                    {
                                        addErrorToRow(row, sheetName, workflowName, "Invalid attribute mapped", true);
                                        break;
                                    }
                                }
                                if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                                    continue;
                                string remarks = string.Empty;
                                bool hasError = false;

                                // If Object row's sync status is Failed.
                                if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                                {
                                    addErrorToRow(row, sheetName, workflowName, remarks);
                                }
                                else if (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
                                {
                                    if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("update", StringComparison.OrdinalIgnoreCase) != -1)
                                    {
                                        // Workflow update case.   
                                        if (pendingRequestWorkflows.Contains(workflowName, StringComparer.OrdinalIgnoreCase))
                                        {
                                            addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Has_Pending_Request);
                                            hasError = true;
                                        }
                                        else
                                        {
                                            validWorkflowsSyncSet.Add(workflowName);
                                        }
                                    }
                                    else if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1)
                                    {
                                        // Workflow insert case.
                                        if (canInsert)
                                        {
                                            if (!dictDelta_ValidWorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                                            {
                                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                                hasError = true;
                                            }
                                            if (workFlowNameDBSet.ContainsKey(workflowName))
                                            {
                                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Name_Already_Exists);
                                                hasError = true;
                                            }

                                        }
                                        else
                                        {
                                            addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                                            hasError = true;
                                        }
                                    }
                                }
                                else if (pendingRequestWorkflows.Contains(workflowName, StringComparer.OrdinalIgnoreCase))
                                {
                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Workflow_Has_Pending_Request);
                                    hasError = true;
                                }

                                if (!hasError && (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName)))
                                {
                                    string priority = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rule_Priority]).Trim();
                                    if (String.IsNullOrEmpty(priority))
                                        priority = "-1";

                                    string radTemplateName = "";

                                    string keyInfo = workflowName + "<$>" + priority;

                                    if (workflowNameVsRulePriorityVsRadTemplateName[workflowName].ContainsKey(Convert.ToInt32(priority)))
                                    {
                                        radTemplateName = workflowNameVsRulePriorityVsRadTemplateName[workflowName][Convert.ToInt32(priority)];
                                        if (dict_RadTemplateVSActions.ContainsKey(radTemplateName))
                                        {
                                            ActionDataDetails info = new ActionDataDetails();

                                            string action = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Action_Name]);

                                            if (dict_RadTemplateVSActions[radTemplateName].Contains(action))
                                            {
                                                if (dict_WorkflowNameVSRuleInfoForActions[workflowName].Contains(priority))
                                                {
                                                    if (!workflowNameVsRuleInfoKeyVsActionNameActionDataSet.ContainsKey(workflowName))
                                                    {
                                                        workflowNameVsRuleInfoKeyVsActionNameActionDataSet.Add(workflowName, new Dictionary<string, Dictionary<string, ActionDataDetails>>());
                                                    }

                                                    if (!workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName].ContainsKey(keyInfo))
                                                        workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName].Add(keyInfo, new Dictionary<string, ActionDataDetails>());

                                                    if (!workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][keyInfo].ContainsKey(action))
                                                        workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][keyInfo].Add(action, GetActionDataDetailsForObjectRow(moduleId, row));
                                                    else
                                                        UpdateActionDataSectionAttributesForObjectRow(workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][keyInfo][action], Convert.ToString(row["Data Section Attributes"]), moduleId, workflowName, row, workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][keyInfo]);
                                                }
                                                else
                                                {
                                                    addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Rule_Does_Not_Exist_In_Template_Mapping_Sheet);
                                                    hasError = true;
                                                }
                                            }
                                            else
                                            {
                                                addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_Action_For_RAD_Template);
                                                hasError = true;
                                            }
                                        }
                                        else
                                        {
                                            addErrorToRow(row, sheetName, workflowName, SRMErrorMessages.Invalid_RAD_Template_Is_Mapped);
                                            hasError = true;
                                        }
                                    }
                                    else
                                    {
                                        addErrorToRow(row, sheetName, workflowName, "Rule Priority does not exist in Template Mapping sheet");
                                        hasError = true;
                                    }

                                }
                            }
                        }
                    }
                    #endregion
                    #region FILL COLLECTION TO SAVE/UPDATE 
                    foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Email_Configuration].Rows)
                    {
                        //Already in Sync 
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                        string sheetName = SRM_WorkFlow_SheetNames.Email_Configuration;
                        WorkflowInfo dictionaryName = new WorkflowInfo();
                        string instrument_type = string.Empty;
                        string remarks = string.Empty;
                        bool hasError = false;

                        // If Object row's sync status is Failed.
                        if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Failed, StringComparison.OrdinalIgnoreCase))
                        {
                            remarks = Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim();
                        }
                        else if ((!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName)) && workflowNameVsRuleInfoKeyVsActionNameActionDataSet.ContainsKey(workflowName))
                        {
                            if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("update", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                dictionaryName = dictDB_WorkflowNameVWorkflowInfo[workflowName];
                            }
                            else if (row[SRMSpecialColumnNames.Delta_Action].ToString().IndexOf("insert", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                // Workflow insert case.
                                dictionaryName = dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName];
                            }

                            List<WorkflowRulesInfo> WorkflowRulesInfoList = new List<WorkflowRulesInfo>();

                            foreach (var item in workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName])
                            {
                                //  string radTemplate = "";
                                WorkflowRulesInfo ruleInfo = new WorkflowRulesInfo();
                                string[] splitItems = item.Key.Split(new string[] { "<$>" }, StringSplitOptions.None);

                                string radTemplate = workflowNameVsRulePriorityVsRadTemplateName[splitItems[0]][Convert.ToInt32(splitItems[1])];

                                bool isValidNumberOfActions = dict_RadTemplateVSActions[radTemplate].Count == workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][item.Key].Count() ? true : false;

                                if (isValidNumberOfActions)
                                {
                                    ruleInfo.WorkFlowRulePriority = splitItems[1];
                                    foreach (var value in workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][item.Key])
                                    {
                                        ruleInfo.ActionStateInfo.Add(workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][item.Key][value.Key]);
                                    }

                                    WorkflowRulesInfoList.Add(ruleInfo);
                                }
                                else
                                {
                                    remarks = SRMErrorMessages.RAD_Template_has_invalid_number_of_actions;
                                    hasError = true;
                                    break;
                                }
                            }

                            if (hasError)
                                addErrorToRow(row, sheetName, workflowName, remarks);
                            else
                            {
                                dictionaryName.WorkflowRulesInfoForActions = WorkflowRulesInfoList;
                                // Workflow Proccessed Successfully
                                workflowNameVsRuleInfoKeyVsActionNameActionDataSet.Remove(workflowName);
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                //Sync
                syncWorkflow(moduleId);

                List<SelectedActionsInfo> InputList = new List<SelectedActionsInfo>();
                foreach (var item in validWorkflowsSyncSet)
                {

                    string workflowName = item;
                    foreach (WorkflowRulesInfo workflowRulesInfo in dictDB_WorkflowNameVWorkflowInfo[workflowName].WorkflowRulesInfoForActions)
                    {
                        List<ActionDataDetails> list = new List<ActionDataDetails>();
                        SelectedActionsInfo input = new SelectedActionsInfo();
                        input.RulePriority = Convert.ToInt32(workflowRulesInfo.WorkFlowRulePriority);
                        input.WorkflowName = item;
                        foreach (var actions in workflowRulesInfo.ActionStateInfo)
                        {
                            if (actions.CheckBoxForEachAction)
                                list.Add(actions);
                        }
                        input.SaveConfigurationForActions = list.ToArray();
                        InputList.Add(input);
                    }

                }
                List<SaveResponse> saveResponse = new List<SaveResponse>();
                saveResponse = SaveWorkflowEmailAction(InputList);


            }
            catch (Exception ex)
            {
                mLogger.Error("WorkflowController -> SRMWorkflowModeler_Sync -> Error : " + ex.ToString());
                errorMsg = ex.ToString();
            }
            finally
            {
                fillDeltaSheetStatus(ref objSetDiff);
                mLogger.Debug("WorkflowController -> SRMWorkflowModeler_Sync -> End : ");
            }
        }

        //Sync Definition
        private void syncWorkflow(int moduleID)
        {
            string Message = "";
            long InstanceId = 0;
            string remarks = string.Empty;
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            HashSet<string> inValidInsertWorkflow = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                #region UPDATE EXISTING WORKFLOWS
                foreach (var item in validWorkflowsSyncSet)
                {
                    string workflowName = item;

                    DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_SaveUpdateWorkflow] '{0}', {1}, {2}, {3}, '{4}', {5}, '{6}', '{7}', '{8}'", workflowName,
                        moduleID, dictDB_WorkflowNameVWorkflowInfo[workflowName].WorkflowActionTypeId, dictDB_WorkflowNameVWorkflowInfo[workflowName].isSave, dictDB_WorkflowNameVWorkflowInfo[workflowName].UserName, dictDB_WorkflowNameVWorkflowInfo[workflowName].InstanceId, string.Join("|", dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeIds), string.Join("|", dictDB_WorkflowNameVWorkflowInfo[workflowName].PrimaryAttributeIds), string.Join("|", dictDB_WorkflowNameVWorkflowInfo[workflowName].OtherAttributeIds)), con);

                    foreach (DataRow dr in result.Tables[0].Rows)
                    {
                        Message = Convert.ToString(dr["message"]);
                        InstanceId = Convert.ToInt64(dr["InstanceId"]);
                    }

                    if (InstanceId != 0)
                    {
                        try
                        {
                            //Save Rules
                            saveWorkFlowRules(dictDB_WorkflowNameVWorkflowInfo[workflowName], InstanceId, con);
                        }
                        catch (Exception ee)
                        {
                            throw new Exception(ee.Message.ToString() + "<$>" + workflowName);
                        }
                    }
                    else
                        throw new Exception(Message + "<$>" + workflowName);
                }
                #endregion

                #region VALIDATE INSERT WORKFLOWS
                foreach (var item in dictDelta_ValidWorkflowNameVWorkflowInfo)
                {
                    string workflowName = item.Key;
                    bool hasError = false;

                    if (dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].WorkflowActionTypeId == 0)
                        hasError = true;
                    else if (dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].TypeIds == null || dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].TypeIds.Count == 0)
                        hasError = true;
                    else if (dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].PrimaryAttributeIds == null || dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].PrimaryAttributeIds.Count == 0)
                        hasError = true;
                    else if (dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].WorkflowRulesInfo == null || dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].WorkflowRulesInfo.Count == 0)
                        hasError = true;
                    if (dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].WorkflowRulesInfo != null)
                    {
                        foreach (var ruleStateInfo in dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].WorkflowRulesInfo)
                        {
                            if (ruleStateInfo.RuleStateInfo == null || ruleStateInfo.RuleStateInfo.Count == 0)
                            {
                                hasError = true;
                                break;
                            }
                        }
                    }

                    if (hasError)
                    {
                        inValidInsertWorkflow.Add(workflowName);
                        //Add workflow to Invalid Workflow Dictionary along with remarks
                        if (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
                        {
                            inValidWorkflowNameVsSheetNameVsError.Add(workflowName, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                            inValidWorkflowNameVsSheetNameVsError[workflowName].Add(SRMErrorMessages.Incomplete_Insert_Workflow, SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets);
                        }
                    }

                }
                #endregion

                #region INSERT NEW WORKFLOWS
                foreach (var item in dictDelta_ValidWorkflowNameVWorkflowInfo)
                {
                    string workflowName = item.Key;

                    if (inValidInsertWorkflow.Contains(workflowName))
                        continue;

                    if (dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].OtherAttributeIds == null)
                        dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].OtherAttributeIds = new List<string>();

                    DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_SaveUpdateWorkflow] '{0}', {1}, {2}, {3}, '{4}', {5}, '{6}', '{7}', '{8}'", workflowName,
                        moduleID, dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].WorkflowActionTypeId, dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].isSave, dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].UserName, dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].InstanceId, string.Join("|", dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].TypeIds), string.Join("|", dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].PrimaryAttributeIds), string.Join("|", dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName].OtherAttributeIds)), con);

                    foreach (DataRow dr in result.Tables[0].Rows)
                    {
                        Message = Convert.ToString(dr["message"]);
                        InstanceId = Convert.ToInt64(dr["InstanceId"]);
                    }

                    if (InstanceId != 0)
                    {
                        try
                        {//Save Rules
                            saveWorkFlowRules(dictDelta_ValidWorkflowNameVWorkflowInfo[workflowName], InstanceId, con);
                        }
                        catch (Exception ee)
                        {
                            throw new Exception(ee.Message.ToString() + "<$>" + workflowName);
                        }
                    }
                    else
                        throw new Exception(Message + "<$>" + workflowName);
                }
                #endregion

                #region VALIDATE FOR INSTRUMENT TYPE
                remarks = validateForInstrumentType(moduleID, con);
                if (string.IsNullOrEmpty(remarks))
                {
                    con.CommitTransaction();
                }
                else
                {
                    throw new Exception(remarks);
                }
                #endregion
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
                if (!string.IsNullOrEmpty(remarks))
                {
                    string[] rem = remarks.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (rem.Length > 1)
                    {
                        inValidWorkflowNameVsSheetNameVsError.Add(SRMErrorMessages.Error_While_Inserting_Workflow, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                        inValidWorkflowNameVsSheetNameVsError[SRMErrorMessages.Error_While_Inserting_Workflow].Add(rem[0], rem[1]);
                    }
                }
                else
                {
                    inValidWorkflowNameVsSheetNameVsError.Add(SRMErrorMessages.Error_While_Inserting_Workflow, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                    inValidWorkflowNameVsSheetNameVsError[SRMErrorMessages.Error_While_Inserting_Workflow].Add("dbError", ee.Message.ToString());
                    //throw new Exception(ee.ToString());
                }

            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }
        }

        private void saveWorkFlowRules(WorkflowInfo workflowRulesInfo, long workflowInstanceId, RDBConnectionManager con)
        {
            try
            {
                //Delete workflow rules as well as states for instance_id
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"
                            DELETE rdetails FROM IVPRefMaster.dbo.ivp_srm_workflow_rules wrules
                            INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_rules_details rdetails
                            ON rdetails.rule_mapping_id = wrules.rule_mapping_id
                            WHERE wrules.workflow_instance_id = {0}

                            DELETE bad FROM IVPRefMaster.dbo.ivp_srm_workflow_email_body_attribute_details bad
	                        INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_email_configuration wec
	                        on bad.action_mapping_id = wec.action_mapping_id
                            INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_rules wrules
                            on wec.rule_mapping_details_id = wrules.rule_mapping_id
	                        WHERE wrules.workflow_instance_id={0}


	                        DELETE wec FROM IVPRefMaster.dbo.ivp_srm_workflow_email_configuration wec
                            INNER  JOIN IVPRefMaster.dbo.ivp_srm_workflow_rules wrules
                            on wec.rule_mapping_details_id = wrules.rule_mapping_id
	                        WHERE  wrules.workflow_instance_id={0}

                            DELETE FROM IVPRefMaster.dbo.ivp_srm_workflow_rules WHERE workflow_instance_id = {0}", workflowInstanceId), con);

                RADXRuleGrammarInfo info = SRMWorkflowPrepareRuleGrammar.prepareGrammarInfo(string.Join("|", workflowRulesInfo.TypeIds), workflowRulesInfo.ModuleId);
                RADXRuleHelperClass objRADXRuleHelperClass = new RADXRuleHelperClass();
                Parser parser = (Parser)objRADXRuleHelperClass.GetRuleParser(info);
                RXRuleInfo objRXRuleInfo = new RXRuleInfo();

                foreach (WorkflowRulesInfo ruleInfo in workflowRulesInfo.WorkflowRulesInfo)
                {
                    int ruleSetID = -1;
                    RDBConnectionManager ruleSaveConnection = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMasterVendor_Connection, true, IsolationLevel.RepeatableRead);

                    try
                    {
                        if (!ruleInfo.SRMWorkFlowIsDefault)
                        {
                            string ruleText = ruleInfo.SRMWorkFlowRuleText.Trim();
                            RXRuleEditorResponse response = objRADXRuleHelperClass.GetRuleClass(ruleText, parser);
                            RXRuleController objRXRuleController = new RXRuleController();
                            objRXRuleController.DBConnectionId = RADConfigReader.GetConfigAppSettings(ConnectionConstants.RefMasterVendor_Connection);


                            objRXRuleInfo.RuleSetID = 0;
                            objRXRuleInfo.RuleType = RXRuleType.Conditional;
                            objRXRuleInfo.RuleName = "WorkFlow Rule" + (workflowInstanceId + 1);
                            objRXRuleInfo.RulePriority = Convert.ToInt32(ruleInfo.WorkFlowRulePriority);
                            objRXRuleInfo.RuleText = ruleText;
                            objRXRuleInfo.CreatedBy = workflowRulesInfo.UserName;
                            objRXRuleInfo.ClassText = response.ClassText;
                            objRXRuleInfo.ClassDetails = response.ClassDocument;
                            objRXRuleInfo.CreatedOn = DateTime.Now;
                            objRXRuleInfo.RuleState = true;
                            objRXRuleInfo.RuleID = 0;
                            objRXRuleInfo.DBOperationType = RXOperationType.Insert;
                            objRXRuleInfo.IsActive = true;
                            objRXRuleInfo.RuleExecutionMode = RXRuleExecutionMode.Priority;

                            ruleSetID = objRXRuleController.SaveRule(objRXRuleInfo, ruleSaveConnection);
                            ruleSaveConnection.CommitTransaction();

                            //Get coulmns used
                            string colUsed = string.Empty;
                            Dictionary<int, List<string>> colsUsed = new RXRuleExecutor() { DBConnectionId = objRXRuleController.DBConnectionId }.GetColumnNameForRulesets(new List<int>() { ruleSetID });

                            if (colsUsed != null && colsUsed.Keys.Count > 0 && colsUsed[ruleSetID].Count > 0)
                                colUsed = "|" + string.Join("|", colsUsed[ruleSetID].ToArray()) + "|";

                            ruleInfo.attributes_in_rule = colUsed;
                        }

                        ruleInfo.WorkflowRuleSetID = Convert.ToString(ruleSetID);

                        //SAVE RULE IN DB
                        ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"
                            INSERT INTO IVPRefMaster.dbo.ivp_srm_workflow_rules(workflow_instance_id,attributes_in_rule,rule_set_id,rad_workflow_id,priority) VALUES({0},'{1}',{2},{3},{4});", workflowInstanceId, ruleInfo.attributes_in_rule, ruleSetID, Convert.ToInt32(ruleInfo.RadWorkFlowInstanceID), Convert.ToInt32(ruleInfo.WorkFlowRulePriority)), con);

                        //Save Rule State Info
                        saveWorkFlowRuleStates(ruleSetID, ruleInfo.RuleStateInfo, workflowInstanceId, con);
                    }
                    catch (Exception e)
                    {
                        ruleSaveConnection.RollbackTransaction();
                        throw new Exception("Invalid Rule. Error while saving rule");
                    }
                    finally
                    {
                        CommonDALWrapper.PutConnectionManager(ruleSaveConnection);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
            }
        }

        private void saveWorkFlowRuleStates(int ruleSetID, List<RuleStateInfo> workflowRuleStateInfo, long workflowInstanceId, RDBConnectionManager con)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"
                        Declare @tempRuleDetailsTable table(rule_set_id INT, stage_name VARCHAR(MAX), apply_mandatory BIT, apply_uniqueness BIT, apply_primary BIT, apply_validation BIT, apply_alert BIT, apply_basket_validation BIT, apply_basket_alert BIT)
                        INSERT INTO @tempRuleDetailsTable (rule_set_id,stage_name, apply_mandatory, apply_uniqueness, apply_primary, apply_validation, apply_alert, apply_basket_validation, apply_basket_alert)VALUES");


            foreach (var itemState in workflowRuleStateInfo)
            {
                sb.Append(@"(").Append(ruleSetID);
                sb.Append(",'").Append(Convert.ToString(itemState.stateName.First().ToString().ToUpper()) + Convert.ToString(itemState.stateName).Substring(1).ToLower());
                sb.Append("',").Append(Convert.ToBoolean(itemState.mandatoryData) ? 1 : 0);
                sb.Append(",").Append(Convert.ToBoolean(itemState.uniquenessValidation) ? 1 : 0);
                sb.Append(",").Append(Convert.ToBoolean(itemState.primaryKeyValidation) ? 1 : 0);
                sb.Append(",").Append(Convert.ToBoolean(itemState.validations) ? 1 : 0);
                sb.Append(",").Append(Convert.ToBoolean(itemState.alerts) ? 1 : 0);
                sb.Append(",").Append(Convert.ToBoolean(itemState.basketValidation) ? 1 : 0);
                sb.Append(",").Append(Convert.ToBoolean(itemState.basketAlert) ? 1 : 0);
                sb.Append(@"),");
            }

            sb = sb.Remove(sb.Length - 1, 1); sb.Append(";");

            sb.Append(@"
                        INSERT INTO IVPRefMaster.dbo.ivp_srm_workflow_rules_details(stage_name,tb.apply_mandatory,apply_uniqueness,apply_primary,apply_validation,apply_alert,apply_basket_validation,apply_basket_alert,rule_mapping_id)
                            SELECT tb.stage_name,tb.apply_mandatory,tb.apply_uniqueness,tb.apply_primary,tb.apply_validation,tb.apply_alert,tb.apply_basket_validation, tb.apply_basket_alert,trt.rule_mapping_id
                            from @tempRuleDetailsTable tb
                            INNER JOIN @tempRulesTable trt
                        ON tb.rule_set_id = trt.rule_set_id
                    ");

            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"
                    Declare @tempRulesTable table(rule_set_id INT, rule_mapping_id INT)
                   
                    INSERT INTO @tempRulesTable(rule_set_id, rule_mapping_id)
	                       Select rule_set_id, rule_mapping_id from IVPRefMaster.dbo.ivp_srm_workflow_rules where workflow_instance_id = {0}

                    {1}
                    ", workflowInstanceId, sb.ToString()), con);

        }

        private void addErrorToRow(ObjectRow objRow, string sheetName, string workflowName, string errorMsg, bool setRemarksColumn = true)
        {
            string remarks = errorMsg.Trim();

            // Remove workflow from Valid Workflow Dictionary (Insert)
            if (dictDelta_ValidWorkflowNameVWorkflowInfo.ContainsKey(workflowName))
                dictDelta_ValidWorkflowNameVWorkflowInfo.Remove(workflowName);

            // Remove workflow from Valid Workflow Dictionary (Update)
            if (validWorkflowsSyncSet.Contains(workflowName))
                validWorkflowsSyncSet.Remove(workflowName);

            //Remove
            if (dict_WorkflowNameVSRuleInfo.ContainsKey(workflowName))
                dict_WorkflowNameVSRuleInfo.Remove(workflowName);

            if (dict_WorkflowNameVSRuleInfoForActions.ContainsKey(workflowName))
                dict_WorkflowNameVSRuleInfoForActions.Remove(workflowName);

            ////Remove
            //if (workflowNameVsAttribute.ContainsKey(workflowName))
            //    workflowNameVsAttribute.Remove(workflowName);

            //Remove
            if (workflowNameVsRuleInfoKeyVsStageDataSet.ContainsKey(workflowName))
                workflowNameVsRuleInfoKeyVsStageDataSet.Remove(workflowName);

            if (workflowNameVsRuleInfoKeyVsActionNameActionDataSet.ContainsKey(workflowName))
                workflowNameVsRuleInfoKeyVsActionNameActionDataSet.Remove(workflowName);


            //Add workflow to Invalid Workflow Dictionary along with remarks
            if (!inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflowName))
            {
                inValidWorkflowNameVsSheetNameVsError.Add(workflowName, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                inValidWorkflowNameVsSheetNameVsError[workflowName].Add(sheetName, remarks);
            }
            else if (inValidWorkflowNameVsSheetNameVsError[workflowName].ContainsKey(sheetName))
            {
                if (!inValidWorkflowNameVsSheetNameVsError[workflowName][sheetName].Equals(remarks, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(remarks))
                    inValidWorkflowNameVsSheetNameVsError[workflowName][sheetName] = inValidWorkflowNameVsSheetNameVsError[workflowName][sheetName] + SRMMigrationSeparators.Remarks_Separator + remarks;
            }

            //Fill Remarks Column from ObjectRow
            if (setRemarksColumn)
            {
                if (objRow != null && objRow.Table.Columns.Contains(SRMSpecialColumnNames.Remarks))
                {
                    objRow[SRMSpecialColumnNames.Remarks] += remarks;
                }
            }
            ////Set Sync status as Not Proccessed
            //if (objRow != null && objRow.Table.Columns.Contains(SRMSpecialColumnNames.Sync_Status))
            //{
            //    objRow[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
            //}
        }

        //private static void addErrorToRow1(ObjectRow objRow, string workflowName, string errorMsg)
        //{
        //    string remarks = errorMsg.Trim();
        //    //isError = true;

        //    // Remove workflow from Valid Workflow Dictionary
        //    if (dictDelta_ValidWorkflowNameVWorkflowInfo[clientName].ContainsKey(workflowName))
        //        dictDelta_ValidWorkflowNameVWorkflowInfo[clientName].Remove(workflowName);

        //    //Add workflow to Invalid Workflow Dictionary along with remarks
        //    if (!inValidWorkflowName.ContainsKey(workflowName))
        //        inValidWorkflowName.Add(workflowName, remarks);
        //    else if (!inValidWorkflowName[workflowName].Equals(remarks, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(remarks))
        //        inValidWorkflowName[workflowName] = inValidWorkflowName[workflowName] + SRMMigrationSeparators.Remarks_Separator + remarks;

        //    //Empty Remarks Column from ObjectRow
        //    if (objRow != null && objRow.Table.Columns.Contains(SRMSpecialColumnNames.Remarks))
        //    {
        //        objRow[SRMSpecialColumnNames.Remarks] = "";
        //    }
        //    //Set Sync status as Not Proccessed
        //    if (objRow != null && objRow.Table.Columns.Contains(SRMSpecialColumnNames.Sync_Status))
        //    {
        //        objRow[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
        //    }
        //}

        private void fillDictionaryForValidation(ObjectSet objSetFromDB, int moduleID, string userName)
        {
            #region FETCH FROM DB
            DataSet dbSet = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @moduleID INT = {0}
                        SELECT DISTINCT winst.workflow_instance_name, workflow_instance_id FROM IVPRefMaster.dbo.ivp_srm_workflow_instance winst WHERE  winst.is_active = 1

                        SELECT DISTINCT(winst.workflow_instance_name)  FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq 
                        INNER JOIN  IVPRefMaster.dbo.ivp_srm_workflow_instance winst
                        ON winst.workflow_instance_id = rq.workflow_instance_id AND rq.is_active = 1 AND winst.module_id = @moduleID

                        SELECT * from [IVPRefMaster].[dbo].[ivp_srm_workflow_action_type] where module_id = @moduleID

                        IF(@moduleID = 3)
	                        BEGIN
		                        SELECT DISTINCT
                                ins.workflow_instance_name AS [workflow_name],
			                        st.sectype_name AS [Instrument_Type_Name],
			                        action.workflow_action_type_name AS [Workflow_Type] 
		                        FROM [IVPRefMaster].[dbo].[ivp_srm_workflow_mapping] map
		                        JOIN [IVPRefMaster].[dbo].[ivp_srm_workflow_instance] ins 
		                        ON map.workflow_instance_id = ins.workflow_instance_id AND ins.is_active = 1 AND map.is_active = 1
		                        JOIN [IVPRefMaster].[dbo].[ivp_srm_workflow_action_type] action
		                        ON ins.workflow_action_type_id = action.workflow_action_type_id AND ins.is_active = 1
		                        JOIN [IVPRefMaster].[dbo].[ivp_srm_modules] module
		                        ON ins.module_id = module.module_id
		                        JOIN [IVPSecMaster].[dbo].[ivp_secm_sectype_master] st
		                        ON module.module_id = 3 AND map.type_id = st.sectype_id 
		                        ORDER BY 1,2 
	                        END
                        ELSE
                        BEGIN
		                        SELECT
                                ins.workflow_instance_name AS [workflow_name],
		                        et.entity_display_name AS [Instrument_Type_Name],
		                        action.workflow_action_type_name AS [Workflow_Type]
		                        FROM[IVPRefMaster].[dbo].[ivp_srm_workflow_mapping] map
		                        JOIN [IVPRefMaster].[dbo].[ivp_srm_workflow_instance] ins
		                        ON map.workflow_instance_id = ins.workflow_instance_id AND ins.is_active = 1 AND map.is_active = 1
		                        JOIN[IVPRefMaster].[dbo].[ivp_srm_workflow_action_type] action
		                        ON ins.workflow_action_type_id = action.workflow_action_type_id AND ins.is_active = 1
		                        JOIN[dbo].[ivp_srm_modules] module
		                        ON ins.module_id = module.module_id
		                        JOIN[IVPRefMaster].[dbo].[ivp_refm_entity_type] et
		                        ON module.module_id = @moduleID AND map.type_id = et.entity_type_id
		                        ORDER BY 1,2 
                        END", moduleID), ConnectionConstants.RefMaster_Connection);

            if (dbSet.Tables.Count > 0)
            {
                if (dbSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dbSet.Tables[0].Rows)
                    {
                        workFlowNameDBSet.Add(Convert.ToString(dr["workflow_instance_name"]).ToLower(), Convert.ToInt32(dr["workflow_instance_id"]));
                    }
                }
                if (dbSet.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in dbSet.Tables[1].Rows)
                    {
                        pendingRequestWorkflows.Add(Convert.ToString(dr["workflow_instance_name"]).ToLower());
                    }
                }
                if (dbSet.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow dr in dbSet.Tables[2].Rows)
                    {
                        dicWorkflowActionTypeNameVsID.Add(Convert.ToString(dr["workflow_action_type_name"]).ToLower(), Convert.ToInt32(dr["workflow_action_type_id"]));
                    }
                }
                if (dbSet.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow dr in dbSet.Tables[3].Rows)
                    {
                        string workflowName = Convert.ToString(dr["workflow_name"]).ToLower().Trim();
                        string instrumentType = Convert.ToString(dr["Instrument_Type_Name"]).ToLower().Trim();
                        string workflowType = Convert.ToString(dr["Workflow_Type"]).ToLower().Trim();

                        if (dictDB_WorkflowNameVSInstrumentTypeNames.ContainsKey(workflowName))
                            dictDB_WorkflowNameVSInstrumentTypeNames[workflowName].Add(instrumentType + "<$>" + workflowType);
                        else
                            dictDB_WorkflowNameVSInstrumentTypeNames.Add(workflowName, new List<string> { instrumentType + "<$>" + workflowType });
                    }
                }
            }
            #endregion

            #region COLLECTION FOR VALIDATION

            //Instrument Type Specifc Or Common Attributes
            if (moduleID == 3)
            {
                SMCommonController smCmnObj = new SMCommonController();
                sectypeTypeVsAttributesInfo = new Dictionary<string, SecurityTypeMasterInfo>(smCmnObj.GetSectypeAttributes(false), StringComparer.OrdinalIgnoreCase);
                commonAttributeNameVsInfo = new Dictionary<string, AttrInfo>(smCmnObj.FetchCommonAttributes(false), StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                RMModelerController rmCmnObj = new RMModelerController();
                entityTypeVsAttributesInfo = new Dictionary<string, RMEntityDetailsInfo>(rmCmnObj.GetEntityTypeDetails(moduleID), StringComparer.OrdinalIgnoreCase);
            }

            // RAD Templates
            radWorkflowsNameVsId = new RWorkFlowService().GetAllWorkFLows().ToDictionary(x => x.WorkflowName.ToLower().Trim(), y => y.WorkflowID, StringComparer.OrdinalIgnoreCase);
            #endregion

            #region DB OBJECT COLLECTION
            //DB Object Collection 

            #region SHEET_WORKFLOWS

            if (objSetFromDB.Tables.Contains(SRM_WorkFlow_SheetNames.Workflows))
            {
                foreach (ObjectRow row in objSetFromDB.Tables[SRM_WorkFlow_SheetNames.Workflows].Rows)
                {
                    WorkflowInfo obj = new WorkflowInfo();
                    string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
                    string workflowType = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Type]).ToLower();

                    obj.ModuleId = moduleID;
                    obj.InstanceId = workFlowNameDBSet[workflowName];
                    obj.WorkflowActionTypeName = workflowType;
                    obj.WorkflowActionTypeId = dicWorkflowActionTypeNameVsID[workflowType];
                    obj.UserName = userName;
                    obj.isSave = false;
                    dictDB_WorkflowNameVWorkflowInfo[workflowName] = obj;
                }
            }
            #endregion

            #region SHEET_WORKFLOW_MAPPING

            if (objSetFromDB.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Mapping))
            {
                foreach (var row in objSetFromDB.Tables[SRM_WorkFlow_SheetNames.Workflow_Mapping].Rows)
                {
                    string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
                    string instrument_type = string.Empty;

                    if (dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeIds == null)
                        dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeIds = new List<string>();

                    // If module is SecMaster
                    if (moduleID == 3)
                    {
                        instrument_type = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Security_Type]).Trim().ToLower();
                        dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeIds.Add(Convert.ToString(sectypeTypeVsAttributesInfo[instrument_type].SectypeId));
                        dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeName = instrument_type;
                    }
                    // If module is other than SecMaster
                    else
                    {
                        instrument_type = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Entity_Type]).Trim().ToLower();
                        dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeIds.Add(Convert.ToString(entityTypeVsAttributesInfo[instrument_type].EntityTypeID));
                        dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeName = instrument_type;
                    }
                }
            }
            #endregion

            #region SHEET_WORKFLOW_INBOX_ATTRIBUTES

            if (objSetFromDB.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes))
            {
                foreach (var row in objSetFromDB.Tables[SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes].Rows)
                {
                    string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
                    string attributeName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Attribute_Name]).ToLower().Trim();
                    string isPrimary = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Is_Primary_Attribute]).ToLower().Trim();
                    int typeId = dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeIds.Count();
                    string typeName = string.Empty;

                    if (dictDB_WorkflowNameVWorkflowInfo[workflowName].PrimaryAttributeIds == null)
                        dictDB_WorkflowNameVWorkflowInfo[workflowName].PrimaryAttributeIds = new List<string>();
                    if (dictDB_WorkflowNameVWorkflowInfo[workflowName].OtherAttributeIds == null)
                        dictDB_WorkflowNameVWorkflowInfo[workflowName].OtherAttributeIds = new List<string>();

                    if (typeId == 1)
                        typeName = dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeName.Trim();

                    // SecMaster Module
                    if (moduleID == 3)
                    {
                        if (typeId == 1)
                        {
                            // Sectype Specific Attrs
                            if (Convert.ToBoolean(isPrimary))
                                dictDB_WorkflowNameVWorkflowInfo[workflowName].PrimaryAttributeIds.Add(Convert.ToString(sectypeTypeVsAttributesInfo[typeName].AttributeInfo.MasterAttrs[attributeName].AttrId));
                            else
                            {
                                //otherAttributeIds.Add(Convert.ToString(sectypeTypeVsAttributesInfo[typeName].AttributeInfo.MasterAttrs[attributeName].AttrId));
                                dictDB_WorkflowNameVWorkflowInfo[workflowName].OtherAttributeIds.Add(Convert.ToString(sectypeTypeVsAttributesInfo[typeName].AttributeInfo.MasterAttrs[attributeName].AttrId));
                            }
                        }
                        else
                        {
                            // Common Attrs     
                            if (Convert.ToBoolean(isPrimary))
                                dictDB_WorkflowNameVWorkflowInfo[workflowName].PrimaryAttributeIds.Add(Convert.ToString(commonAttributeNameVsInfo[attributeName].AttrId));
                            else
                            {
                                //otherAttributeIds.Add(Convert.ToString(commonAttributeNameVsInfo[attributeName].AttrId));
                                dictDB_WorkflowNameVWorkflowInfo[workflowName].OtherAttributeIds.Add(Convert.ToString(commonAttributeNameVsInfo[attributeName].AttrId));
                            }
                        }
                    }
                    //Modules Other Than SecMaster
                    else
                    {
                        //Check in Entity Type Specific Attrs
                        if (Convert.ToBoolean(isPrimary))
                            dictDB_WorkflowNameVWorkflowInfo[workflowName].PrimaryAttributeIds.Add(Convert.ToString(entityTypeVsAttributesInfo[typeName].Attributes[attributeName].EntityAttributeID));
                        else
                        {
                            //otherAttributeIds.Add(Convert.ToString(entityTypeVsAttributesInfo[typeName].Attributes[attributeName].EntityAttributeID));
                            dictDB_WorkflowNameVWorkflowInfo[workflowName].OtherAttributeIds.Add(Convert.ToString(entityTypeVsAttributesInfo[typeName].Attributes[attributeName].EntityAttributeID));
                        }
                    }


                }
            }
            #endregion

            #region SHEET_WORKFLOW_TEMPLATE_MAPPING
            //if (objSetFromDB.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Template_Mapping))
            //{
            //    foreach (var row in objSetFromDB.Tables[SRM_WorkFlow_SheetNames.Workflow_Template_Mapping].Rows.AsEnumerable())
            //    {
            //        string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
            //        string is_Rule_Configured = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Is_Rule_Configured]).Trim();
            //        string radTemplateName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rad_Workflow_Template_Name]).Trim();
            //        string ruleText = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rule_Text]).Trim();
            //        string priority = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rule_Priority]).Trim();
            //        bool is_Default_Workflow = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Is_Default_Workflow]).Trim().Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false;

            //        if (!dict_WorkflowNameVSRuleInfo.ContainsKey(workflowName))
            //            dictDB_WorkflowNameVSRuleInfo.Add(workflowName, new HashSet<string> { Convert.ToString(is_Default_Workflow).ToLower() + "<$>" + ruleText.ToLower() + "<$>" + priority + "<$>" + radTemplateName.ToLower() });
            //        else
            //            dictDB_WorkflowNameVSRuleInfo[workflowName].Add(Convert.ToString(is_Default_Workflow).ToLower() + "<$>" + ruleText.ToLower() + "<$>" + priority + "<$>" + radTemplateName.ToLower());

            //    }
            //}
            #endregion

            #region SHEET_WORKFLOW_DATA_VALIDATION_CHECKS

            if (objSetFromDB.Tables.Contains(SRM_WorkFlow_SheetNames.Data_Validation_Checks))
            {
                foreach (var row in objSetFromDB.Tables[SRM_WorkFlow_SheetNames.Data_Validation_Checks].Rows)
                {

                    string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();

                    string is_Default_Workflow = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Is_Default_Workflow]).Trim();
                    string ruleText = is_Default_Workflow.Equals("yes") ? "" : Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rule_Text]).Trim();
                    string priority = is_Default_Workflow.Equals("yes") ? "-1" : Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rule_Priority]).Trim();
                    string radTemplateName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Rad_Workflow_Template_Name]).ToLower();

                    string keyInfo = is_Default_Workflow + "<$>" + ruleText + "<$>" + (is_Default_Workflow.Equals("yes", StringComparison.OrdinalIgnoreCase) ? "-1" : priority) + "<$>" + radTemplateName;

                    RuleStateInfo info = new RuleStateInfo();

                    string stage = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Stage]).Trim();
                    if (stage.Equals("Workflow Initiation", StringComparison.OrdinalIgnoreCase))
                        stage = "Start";
                    info.stateName = stage;
                    info.mandatoryData = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Mandatory]).ToLower() == "yes" ? true : false;
                    info.uniquenessValidation = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Uniqueness]).ToLower() == "yes" ? true : false;
                    info.primaryKeyValidation = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Primary_Key]).ToLower() == "yes" ? true : false;
                    info.validations = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Validations]).ToLower() == "yes" ? true : false;
                    info.alerts = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Alerts]).ToLower() == "yes" ? true : false;

                    if (moduleID == 3)
                    {
                        info.basketValidation = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Basket_Validations]).ToLower() == "yes" ? true : false;
                        info.basketAlert = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Basket_Alerts]).ToLower() == "yes" ? true : false;
                    }
                    else
                    {
                        info.basketValidation = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Group_Validation]).ToLower() == "yes" ? true : false;
                        info.basketAlert = false;
                    }

                    //Make custom dictionary

                    if (!dictDB_workflowNameVsRuleInfoKeyVsStageDataSet.ContainsKey(workflowName))
                    {
                        //dictDB_workflowNameVsRuleInfoKeyVsStageDataSet.Add(workflowName, new Dictionary<string, HashSet<RuleStateInfo>> { { keyInfo, new HashSet<RuleStateInfo> { info } } });
                        dictDB_workflowNameVsRuleInfoKeyVsStageDataSet.Add(workflowName, new Dictionary<string, HashSet<RuleStateInfo>>(StringComparer.OrdinalIgnoreCase));
                        dictDB_workflowNameVsRuleInfoKeyVsStageDataSet[workflowName].Add(keyInfo, new HashSet<RuleStateInfo> { info });
                    }
                    else
                    {
                        if (!dictDB_workflowNameVsRuleInfoKeyVsStageDataSet[workflowName].ContainsKey(keyInfo))
                            dictDB_workflowNameVsRuleInfoKeyVsStageDataSet[workflowName].Add(keyInfo, new HashSet<RuleStateInfo> { info });
                        else
                            dictDB_workflowNameVsRuleInfoKeyVsStageDataSet[workflowName][keyInfo].Add(info);
                    }

                }


                foreach (var row in objSetFromDB.Tables[SRM_WorkFlow_SheetNames.Data_Validation_Checks].Rows)
                {
                    string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
                    List<WorkflowRulesInfo> WorkflowRulesInfoList = new List<WorkflowRulesInfo>();

                    if (dictDB_workflowNameVsRuleInfoKeyVsStageDataSet.ContainsKey(workflowName))
                    {
                        foreach (var item in dictDB_workflowNameVsRuleInfoKeyVsStageDataSet[workflowName])
                        {
                            WorkflowRulesInfo ruleInfo = new WorkflowRulesInfo();
                            string[] splitItems = item.Key.Split(new string[] { "<$>" }, StringSplitOptions.None);
                            string radTemplate = splitItems[3];

                            ruleInfo.SRMWorkFlowIsDefault = splitItems[0].Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false;
                            ruleInfo.SRMWorkFlowRuleText = splitItems[1];
                            ruleInfo.WorkflowRuleSetID = splitItems[0].Equals("yes", StringComparison.OrdinalIgnoreCase) ? "-1" : "";
                            ruleInfo.WorkFlowRulePriority = splitItems[2];
                            ruleInfo.RadWorkFlowInstanceName = splitItems[3];
                            ruleInfo.RadWorkFlowInstanceID = Convert.ToString(radWorkflowsNameVsId[splitItems[3]]);
                            ruleInfo.RuleStateInfo = dictDB_workflowNameVsRuleInfoKeyVsStageDataSet[workflowName][item.Key].ToList();

                            WorkflowRulesInfoList.Add(ruleInfo);

                        }
                        dictDB_WorkflowNameVWorkflowInfo[workflowName].WorkflowRulesInfo = WorkflowRulesInfoList;
                        // Workflow Proccessed Successfully
                        dictDB_workflowNameVsRuleInfoKeyVsStageDataSet.Remove(workflowName);
                    }
                }
            }
            #endregion

            #region SHEET_WORKFLOW_EMAIL_CONFIGURATION
            if (objSetFromDB.Tables.Contains(SRM_WorkFlow_SheetNames.Email_Configuration))
            {
                foreach (var row in objSetFromDB.Tables[SRM_WorkFlow_SheetNames.Email_Configuration].Rows)
                {
                    string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]).Trim();
                    int priority;
                    if (row[SRM_WorkFlow_ColumnNames.Rule_Priority] == DBNull.Value)
                        priority = -1;
                    else
                        priority = Convert.ToInt32(row[SRM_WorkFlow_ColumnNames.Rule_Priority]);

                    string keyInfo = workflowName + "<$>" + priority;
                    string action = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Action_Name]).Trim();


                    //Make custom dictionary

                    if (!dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet.ContainsKey(workflowName))
                    {
                        dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet.Add(workflowName, new Dictionary<string, Dictionary<string, ActionDataDetails>>());
                    }

                    if (!dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName].ContainsKey(keyInfo))
                        dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName].Add(keyInfo, new Dictionary<string, ActionDataDetails>());

                    if (!dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][keyInfo].ContainsKey(action))
                        dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][keyInfo].Add(action, GetActionDataDetailsForObjectRow(moduleID, row, false));
                    else
                        UpdateActionDataSectionAttributesForObjectRow(dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][keyInfo][action], Convert.ToString(row["Data Section Attributes"]), moduleID, workflowName, row, null, false);
                }

                foreach (var row in objSetFromDB.Tables[SRM_WorkFlow_SheetNames.Email_Configuration].Rows)
                {
                    string workflowName = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
                    string actionName = Convert.ToString(row["Action Name"]);
                    List<WorkflowRulesInfo> WorkflowRulesInfoList = new List<WorkflowRulesInfo>();

                    if (dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet.ContainsKey(workflowName))
                    {
                        foreach (var item in dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName])
                        {
                            WorkflowRulesInfo ruleInfo = new WorkflowRulesInfo();
                            string[] splitItems = item.Key.Split(new string[] { "<$>" }, StringSplitOptions.None);
                            ruleInfo.WorkFlowRulePriority = splitItems[1];
                            foreach (var value in dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][item.Key])
                            {
                                ruleInfo.ActionStateInfo.Add(dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet[workflowName][item.Key][value.Key]);
                            }

                            WorkflowRulesInfoList.Add(ruleInfo);

                        }
                        dictDB_WorkflowNameVWorkflowInfo[workflowName].WorkflowRulesInfoForActions = WorkflowRulesInfoList;
                        // Workflow Proccessed Successfully
                        dictDB_workflowNameVsRuleInfoKeyVsActionNameActionDataSet.Remove(workflowName);
                    }
                }

            }
            #endregion
            #endregion
        }

        private string validateForInstrumentType(int moduleID, RDBConnectionManager con)
        {
            string remarks = string.Empty;

            DataSet dbSet = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @moduleID INT = {0}
                        IF(@moduleID = 3)
	                        BEGIN
		                        SELECT DISTINCT
                                ins.workflow_instance_name AS [workflow_name],
			                        st.sectype_name AS [Instrument_Type_Name],
			                        action.workflow_action_type_name AS [Workflow_Type] 
		                        FROM [IVPRefMaster].[dbo].[ivp_srm_workflow_mapping] map
		                        JOIN [IVPRefMaster].[dbo].[ivp_srm_workflow_instance] ins 
		                        ON map.workflow_instance_id = ins.workflow_instance_id AND ins.is_active = 1 AND map.is_active = 1
		                        JOIN [IVPRefMaster].[dbo].[ivp_srm_workflow_action_type] action
		                        ON ins.workflow_action_type_id = action.workflow_action_type_id AND ins.is_active = 1
		                        JOIN [IVPRefMaster].[dbo].[ivp_srm_modules] module
		                        ON ins.module_id = module.module_id
		                        JOIN [IVPSecMaster].[dbo].[ivp_secm_sectype_master] st
		                        ON module.module_id = 3 AND map.type_id = st.sectype_id 
		                        ORDER BY 1,2 
	                        END
                        ELSE
                        BEGIN
		                        SELECT
                                ins.workflow_instance_name AS [workflow_name],
		                        et.entity_display_name AS [Instrument_Type_Name],
		                        action.workflow_action_type_name AS [Workflow_Type]
		                        FROM[IVPRefMaster].[dbo].[ivp_srm_workflow_mapping] map
		                        JOIN[dbo].[ivp_srm_workflow_instance] ins
		                        ON map.workflow_instance_id = ins.workflow_instance_id AND ins.is_active = 1 AND map.is_active = 1
		                        JOIN[IVPRefMaster].[dbo].[ivp_srm_workflow_action_type] action
		                        ON ins.workflow_action_type_id = action.workflow_action_type_id AND ins.is_active = 1
		                        JOIN[dbo].[ivp_srm_modules] module
		                        ON ins.module_id = module.module_id
		                        JOIN[IVPRefMaster].[dbo].[ivp_refm_entity_type] et
		                        ON module.module_id = @moduleID AND map.type_id = et.entity_type_id
		                        ORDER BY 1,2 
                        END", moduleID), con);

            if (dbSet.Tables.Count > 0)
            {
                dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

                if (dbSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dbSet.Tables[0].Rows)
                    {
                        string workflowName = Convert.ToString(dr["workflow_name"]).ToLower().Trim();
                        string instrumentType = Convert.ToString(dr["Instrument_Type_Name"]).ToLower().Trim();
                        string workflowType = Convert.ToString(dr["Workflow_Type"]).ToLower().Trim();
                        string key = instrumentType + "<$>" + workflowType;


                        if (dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames.ContainsKey(key))
                        {
                            //dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[key].Add(workflowName);
                            remarks = dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[key].First() + '|' + instrumentType;
                            break;
                        }
                        else
                        {
                            dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames.Add(key, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                            dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames[key].Add(workflowName);
                        }
                    }
                }
            }

            return remarks;
        }

        private void reInitializeVariables()
        {

            workFlowNameDBSet = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            dicWorkflowActionTypeNameVsID = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            //workflowTypeVsInstrumentTypes = new ConcurrentDictionary<string,Dictionary<string, HashSet<string>>>(StringComparer.OrdinalIgnoreCase);

            pendingRequestWorkflows = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            radWorkflowsNameVsId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            dictDelta_ValidWorkflowNameVWorkflowInfo = new Dictionary<string, WorkflowInfo>(StringComparer.OrdinalIgnoreCase);
            dictDB_WorkflowNameVWorkflowInfo = new Dictionary<string, WorkflowInfo>(StringComparer.OrdinalIgnoreCase);

            //inValidWorkflowName = new ConcurrentDictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            validWorkflowsSyncSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            //validRemovedTypeWorkflowsSyncSet = new ConcurrentDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            inValidWorkflowNameVsSheetNameVsError = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            dictProcessed_instrumentType_WorkflowTypeVSWorkflowNames = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            dictDB_WorkflowNameVSInstrumentTypeNames = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            dict_WorkflowNameVSRuleInfo = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            //dictDB_WorkflowNameVSRuleInfo = new ConcurrentDictionary<string, Dictionary<string, HashSet<string>>>(StringComparer.OrdinalIgnoreCase);

            dict_RadTemplateVSStates = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            workflowNameVsAttribute = new Dictionary<string, Dictionary<string, List<string>>>();

            workflowNameVsRuleInfoKeyVsStageDataSet = new Dictionary<string, Dictionary<string, HashSet<RuleStateInfo>>>(StringComparer.OrdinalIgnoreCase);

            dictDB_workflowNameVsRuleInfoKeyVsStageDataSet = new Dictionary<string, Dictionary<string, HashSet<RuleStateInfo>>>(StringComparer.OrdinalIgnoreCase);

            workflowNameVsTypeId = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        }

        private void fillDeltaSheetStatus(ref ObjectSet objSetDiff)
        {
            bool isAPIError = false;
            string errorMsg = string.Empty;
            string workflowName = string.Empty;
            bool isAPIErrorProcessed = false;
            bool isMissingWorkflowMappingProccessed = false;

            if (inValidWorkflowNameVsSheetNameVsError.ContainsKey(SRMErrorMessages.Error_While_Inserting_Workflow))
            {
                isAPIError = true;
                if (inValidWorkflowNameVsSheetNameVsError[SRMErrorMessages.Error_While_Inserting_Workflow].ContainsKey("dbError"))
                {
                    string[] str = inValidWorkflowNameVsSheetNameVsError[SRMErrorMessages.Error_While_Inserting_Workflow]["dbError"].Split(new string[] { "<$>" }, StringSplitOptions.RemoveEmptyEntries);

                    if (str.Length == 2)
                    {
                        errorMsg = str[0];
                        workflowName = str[1];

                    }
                }
                else
                {
                    foreach (var item in inValidWorkflowNameVsSheetNameVsError[SRMErrorMessages.Error_While_Inserting_Workflow])
                    {
                        workflowName = item.Key;
                        errorMsg = SRMErrorMessages.Workflow_Already_Exists_for + ": '" + item.Value + "'";
                    }
                }
            }

            #region SHEET_WORKFLOWS
            if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflows))
            {
                string errorInSheet = string.Empty;

                foreach (ObjectRow row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflows].Rows)
                {
                    string workflow = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);

                    if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                    {
                        row[SRMSpecialColumnNames.Remarks] = string.Empty;
                    }
                    else
                    {
                        if (isAPIError)
                        {
                            if (workflow.Equals(workflowName, StringComparison.OrdinalIgnoreCase) && !isAPIErrorProcessed)
                            {
                                row[SRMSpecialColumnNames.Remarks] = (Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim().Equals(errorMsg.Trim(), StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(errorMsg.Trim())) ? Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim() : Convert.ToString(row[SRMSpecialColumnNames.Remarks]).ToString() + SRMMigrationSeparators.Remarks_Separator + errorMsg.Trim();
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                errorInSheet = SRM_WorkFlow_SheetNames.Workflows;
                                isAPIErrorProcessed = true;
                            }
                            else if (errorInSheet.Equals(SRM_WorkFlow_SheetNames.Workflows, StringComparison.OrdinalIgnoreCase))
                            {
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                            }
                            else
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
                        }
                        else if (inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflow))
                        {
                            if (!isMissingWorkflowMappingProccessed && inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRMErrorMessages.Incomplete_Insert_Workflow))
                            {
                                row[SRMSpecialColumnNames.Remarks] = (Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim().Equals(errorMsg.Trim(), StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(errorMsg.Trim())) ? Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim() : Convert.ToString(row[SRMSpecialColumnNames.Remarks]).ToString() + SRMMigrationSeparators.Remarks_Separator + SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets;
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow].Add("processed", "");
                                isMissingWorkflowMappingProccessed = true;
                            }
                            else if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRM_WorkFlow_SheetNames.Workflows))
                            {
                                row[SRMSpecialColumnNames.Remarks] = (Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim().Equals(errorMsg.Trim(), StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(errorMsg.Trim())) ? Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim() : Convert.ToString(row[SRMSpecialColumnNames.Remarks]).ToString() + SRMMigrationSeparators.Remarks_Separator + inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Workflows];
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow].Add("processed", "");
                            }
                            else
                            {
                                row[SRMSpecialColumnNames.Remarks] = (Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim().Equals(errorMsg.Trim(), StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(errorMsg.Trim())) ? Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim() : Convert.ToString(row[SRMSpecialColumnNames.Remarks]).ToString() + SRMMigrationSeparators.Remarks_Separator + "Error in sheet : " + inValidWorkflowNameVsSheetNameVsError[workflow].Keys.First();
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                            }
                        }
                        else if (!Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                            row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Passed;
                    }
                }
            }
            #endregion

            #region SHEET_WORKFLOW_MAPPING
            if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Mapping))
            {
                string errorInSheet = string.Empty;

                foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflow_Mapping].Rows.AsEnumerable())
                {
                    string workflow = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
                    string remarks = string.Empty;
                    string status = string.Empty;
                    bool isProccessed = false;

                    if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                    {
                        row[SRMSpecialColumnNames.Remarks] = string.Empty;
                    }
                    else
                    {
                        if (isAPIError)
                        {
                            if (workflow.Equals(workflowName, StringComparison.OrdinalIgnoreCase) && !isAPIErrorProcessed)
                            {
                                row[SRMSpecialColumnNames.Remarks] = (Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim().Equals(errorMsg.Trim(), StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(errorMsg.Trim())) ? Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim() : Convert.ToString(row[SRMSpecialColumnNames.Remarks]).ToString() + SRMMigrationSeparators.Remarks_Separator + errorMsg.Trim();
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                errorInSheet = SRM_WorkFlow_SheetNames.Workflow_Mapping;
                                isAPIErrorProcessed = true;
                            }
                            else if (errorInSheet.Equals(SRM_WorkFlow_SheetNames.Workflow_Mapping, StringComparison.OrdinalIgnoreCase))
                            {
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                            }
                            else
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
                        }
                        else if (inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflow))
                        {
                            if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey("processed"))
                            {
                                remarks = "";
                                status = SRMMigrationStatus.Not_Processed;

                            }
                            else if (!isMissingWorkflowMappingProccessed && inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRMErrorMessages.Incomplete_Insert_Workflow))
                            {
                                remarks = SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets;
                                status = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow].Add("processed", "");
                                isMissingWorkflowMappingProccessed = true;
                            }
                            else if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRM_WorkFlow_SheetNames.Workflow_Mapping))
                            {
                                remarks = inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Workflow_Mapping];
                                if (remarks.Equals("processed"))
                                    remarks = string.Empty;
                                status = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Workflow_Mapping] = "processed";
                            }
                            else
                            {
                                foreach (var item in inValidWorkflowNameVsSheetNameVsError[workflow])
                                {
                                    if (inValidWorkflowNameVsSheetNameVsError[workflow][item.Key].Equals("processed", StringComparison.OrdinalIgnoreCase))
                                    {
                                        isProccessed = true;
                                        break;
                                    }
                                }

                                if (isProccessed)
                                    status = SRMMigrationStatus.Not_Processed;
                                else
                                {
                                    remarks = "Error in sheet : " + inValidWorkflowNameVsSheetNameVsError[workflow].Keys.First();
                                    status = SRMMigrationStatus.Failed;
                                }
                            }
                        }
                        else
                            status = SRMMigrationStatus.Passed;


                        if (!(Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase) &&
                             status.Equals(SRMMigrationStatus.Passed, StringComparison.OrdinalIgnoreCase)) && !isAPIError)
                        {
                            if (remarks.IndexOf("Error in sheet", StringComparison.OrdinalIgnoreCase) != -1 || !isMissingWorkflowMappingProccessed)
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) || (remarks.IndexOf(Convert.ToString(row[SRMSpecialColumnNames.Remarks]), StringComparison.OrdinalIgnoreCase) == -1))
                                    row[SRMSpecialColumnNames.Remarks] += SRMMigrationSeparators.Remarks_Separator + remarks;
                            }

                            row[SRMSpecialColumnNames.Sync_Status] = status;
                        }
                    }

                }
            }
            #endregion

            #region SHEET_Workflow_Inbox_Attributes
            if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes))
            {
                string errorInSheet = string.Empty;

                foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes].Rows.AsEnumerable())
                {
                    string workflow = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
                    string remarks = string.Empty;
                    string status = string.Empty;
                    bool isProccessed = false;

                    if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                    {
                        row[SRMSpecialColumnNames.Remarks] = string.Empty;
                    }
                    else
                    {
                        if (isAPIError)
                        {
                            if (workflow.Equals(workflowName, StringComparison.OrdinalIgnoreCase) && !isAPIErrorProcessed)
                            {
                                row[SRMSpecialColumnNames.Remarks] = (Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim().Equals(errorMsg.Trim(), StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(errorMsg.Trim())) ? Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim() : Convert.ToString(row[SRMSpecialColumnNames.Remarks]).ToString() + SRMMigrationSeparators.Remarks_Separator + errorMsg.Trim();
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                errorInSheet = SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes;
                                isAPIErrorProcessed = true;
                            }
                            else if (errorInSheet.Equals(SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes, StringComparison.OrdinalIgnoreCase))
                            {
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                            }
                            else
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
                        }
                        else if (inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflow))
                        {
                            if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey("processed"))
                            {
                                remarks = "";
                                status = SRMMigrationStatus.Not_Processed;

                            }
                            else if (!isMissingWorkflowMappingProccessed && inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRMErrorMessages.Incomplete_Insert_Workflow))
                            {
                                remarks = SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets;
                                status = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow].Add("processed", "");
                                isMissingWorkflowMappingProccessed = true;
                            }
                            else if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes))
                            {
                                remarks = inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes];
                                if (remarks.Equals("processed"))
                                    remarks = string.Empty;
                                status = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes] = "processed";
                            }
                            else
                            {
                                foreach (var item in inValidWorkflowNameVsSheetNameVsError[workflow])
                                {
                                    if (inValidWorkflowNameVsSheetNameVsError[workflow][item.Key].Equals("processed", StringComparison.OrdinalIgnoreCase))
                                    {
                                        isProccessed = true;
                                        break;
                                    }
                                }

                                if (isProccessed)
                                    status = SRMMigrationStatus.Not_Processed;
                                else
                                {
                                    remarks = "Error in sheet : " + inValidWorkflowNameVsSheetNameVsError[workflow].Keys.First();
                                    status = SRMMigrationStatus.Failed;
                                }
                            }
                        }
                        else
                            status = SRMMigrationStatus.Passed;

                        if (!(Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase) &&
                            status.Equals(SRMMigrationStatus.Passed, StringComparison.OrdinalIgnoreCase)) && !isAPIError)
                        {
                            if (remarks.IndexOf("Error in sheet", StringComparison.OrdinalIgnoreCase) != -1 || !isMissingWorkflowMappingProccessed)
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) || (remarks.IndexOf(Convert.ToString(row[SRMSpecialColumnNames.Remarks]), StringComparison.OrdinalIgnoreCase) == -1))
                                    row[SRMSpecialColumnNames.Remarks] += SRMMigrationSeparators.Remarks_Separator + remarks;
                            }

                            row[SRMSpecialColumnNames.Sync_Status] = status;
                        }

                    }
                }
            }
            #endregion

            #region SHEET_Workflow_Template_Mapping
            if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Workflow_Template_Mapping))
            {
                string errorInSheet = string.Empty;

                foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Workflow_Template_Mapping].Rows.AsEnumerable())
                {
                    string workflow = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
                    string remarks = string.Empty;
                    string status = string.Empty;
                    bool isProccessed = false;

                    if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                    {
                        row[SRMSpecialColumnNames.Remarks] = string.Empty;
                    }
                    else
                    {
                        if (isAPIError)
                        {
                            if (workflow.Equals(workflowName, StringComparison.OrdinalIgnoreCase) && !isAPIErrorProcessed)
                            {
                                row[SRMSpecialColumnNames.Remarks] = (Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim().Equals(errorMsg.Trim(), StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(errorMsg.Trim())) ? Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim() : Convert.ToString(row[SRMSpecialColumnNames.Remarks]).ToString() + SRMMigrationSeparators.Remarks_Separator + errorMsg.Trim();
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                errorInSheet = SRM_WorkFlow_SheetNames.Workflow_Template_Mapping;
                                isAPIErrorProcessed = true;
                            }
                            else if (errorInSheet.Equals(SRM_WorkFlow_SheetNames.Workflow_Template_Mapping, StringComparison.OrdinalIgnoreCase))
                            {
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                            }
                            else
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
                        }
                        else if (inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflow))
                        {
                            if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey("processed"))
                            {
                                remarks = "";
                                status = SRMMigrationStatus.Not_Processed;

                            }
                            else if (!isMissingWorkflowMappingProccessed && inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRMErrorMessages.Incomplete_Insert_Workflow))
                            {
                                remarks = SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets;
                                status = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow].Add("processed", "");
                                isMissingWorkflowMappingProccessed = true;
                            }
                            else if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRM_WorkFlow_SheetNames.Workflow_Template_Mapping))
                            {
                                remarks = inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Workflow_Template_Mapping];
                                if (remarks.Equals("processed"))
                                    remarks = string.Empty;
                                status = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Workflow_Template_Mapping] = "processed";
                            }
                            else
                            {
                                foreach (var item in inValidWorkflowNameVsSheetNameVsError[workflow])
                                {
                                    if (inValidWorkflowNameVsSheetNameVsError[workflow][item.Key].Equals("processed", StringComparison.OrdinalIgnoreCase))
                                    {
                                        isProccessed = true;
                                        break;
                                    }
                                }

                                if (isProccessed)
                                    status = SRMMigrationStatus.Not_Processed;
                                else
                                {
                                    remarks = "Error in sheet : " + inValidWorkflowNameVsSheetNameVsError[workflow].Keys.First();
                                    status = SRMMigrationStatus.Failed;
                                }
                            }
                        }
                        else
                            status = SRMMigrationStatus.Passed;

                        if (!(Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase) &&
                            status.Equals(SRMMigrationStatus.Passed, StringComparison.OrdinalIgnoreCase)) && !isAPIError)
                        {
                            if (remarks.IndexOf("Error in sheet", StringComparison.OrdinalIgnoreCase) != -1 || !isMissingWorkflowMappingProccessed)
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) || (remarks.IndexOf(Convert.ToString(row[SRMSpecialColumnNames.Remarks]), StringComparison.OrdinalIgnoreCase) == -1))
                                    row[SRMSpecialColumnNames.Remarks] += SRMMigrationSeparators.Remarks_Separator + remarks;
                            }

                            row[SRMSpecialColumnNames.Sync_Status] = status;
                        }
                    }
                }
            }
            #endregion

            #region SHEET_Workflow_Data_Validation_Checks
            if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Data_Validation_Checks))
            {
                string errorInSheet = string.Empty;

                foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Data_Validation_Checks].Rows.AsEnumerable())
                {
                    string workflow = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
                    string remarks = string.Empty;
                    string status = string.Empty;
                    bool isProccessed = false;

                    if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                    {
                        row[SRMSpecialColumnNames.Remarks] = string.Empty;
                    }
                    else
                    {
                        if (isAPIError)
                        {
                            if (workflow.Equals(workflowName, StringComparison.OrdinalIgnoreCase) && !isAPIErrorProcessed)
                            {
                                row[SRMSpecialColumnNames.Remarks] = (Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim().Equals(errorMsg.Trim(), StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(errorMsg.Trim())) ? Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim() : Convert.ToString(row[SRMSpecialColumnNames.Remarks]).ToString() + SRMMigrationSeparators.Remarks_Separator + errorMsg.Trim();
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                errorInSheet = SRM_WorkFlow_SheetNames.Data_Validation_Checks;
                                isAPIErrorProcessed = true;
                            }
                            else if (errorInSheet.Equals(SRM_WorkFlow_SheetNames.Data_Validation_Checks, StringComparison.OrdinalIgnoreCase))
                            {
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                            }
                            else
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
                        }
                        else if (inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflow))
                        {
                            if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey("processed"))
                            {
                                remarks = "";
                                status = SRMMigrationStatus.Not_Processed;

                            }
                            else if (!isMissingWorkflowMappingProccessed && inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRMErrorMessages.Incomplete_Insert_Workflow))
                            {
                                remarks = SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets;
                                status = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow].Add("processed", "");
                                isMissingWorkflowMappingProccessed = true;
                            }
                            else if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRM_WorkFlow_SheetNames.Data_Validation_Checks))
                            {
                                remarks = inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Data_Validation_Checks];
                                if (remarks.Equals("processed"))
                                    remarks = string.Empty;
                                status = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Data_Validation_Checks] = "processed";
                            }
                            else
                            {
                                foreach (var item in inValidWorkflowNameVsSheetNameVsError[workflow])
                                {
                                    if (inValidWorkflowNameVsSheetNameVsError[workflow][item.Key].Equals("processed", StringComparison.OrdinalIgnoreCase))
                                    {
                                        isProccessed = true;
                                        break;
                                    }
                                }

                                if (isProccessed)
                                    status = SRMMigrationStatus.Not_Processed;
                                else
                                {
                                    remarks = "Error in sheet : " + inValidWorkflowNameVsSheetNameVsError[workflow].Keys.First();
                                    status = SRMMigrationStatus.Failed;
                                }
                            }
                        }
                        else
                            status = SRMMigrationStatus.Passed;

                        if (!(Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase) &&
                                               status.Equals(SRMMigrationStatus.Passed, StringComparison.OrdinalIgnoreCase)) && !isAPIError)
                        {
                            if (remarks.IndexOf("Error in sheet", StringComparison.OrdinalIgnoreCase) != -1 || !isMissingWorkflowMappingProccessed)
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) || (remarks.IndexOf(Convert.ToString(row[SRMSpecialColumnNames.Remarks]), StringComparison.OrdinalIgnoreCase) == -1))
                                    row[SRMSpecialColumnNames.Remarks] += SRMMigrationSeparators.Remarks_Separator + remarks;
                            }
                            row[SRMSpecialColumnNames.Sync_Status] = status;
                        }
                    }
                }

            }
            #endregion

            #region SHEET_Workflow_Email_Configuration
            if (objSetDiff.Tables.Contains(SRM_WorkFlow_SheetNames.Email_Configuration))
            {
                string errorInSheet = string.Empty;

                foreach (var row in objSetDiff.Tables[SRM_WorkFlow_SheetNames.Email_Configuration].Rows.AsEnumerable())
                {
                    string workflow = Convert.ToString(row[SRM_WorkFlow_ColumnNames.Workflow_Name]);
                    string remarks = string.Empty;
                    string status = string.Empty;
                    bool isProccessed = false;

                    if (Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase))
                    {
                        row[SRMSpecialColumnNames.Remarks] = string.Empty;
                    }
                    else
                    {
                        if (isAPIError)
                        {
                            if (workflow.Equals(workflowName, StringComparison.OrdinalIgnoreCase) && !isAPIErrorProcessed)
                            {
                                row[SRMSpecialColumnNames.Remarks] = (Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim().Equals(errorMsg.Trim(), StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(errorMsg.Trim())) ? Convert.ToString(row[SRMSpecialColumnNames.Remarks]).Trim() : Convert.ToString(row[SRMSpecialColumnNames.Remarks]).ToString() + SRMMigrationSeparators.Remarks_Separator + errorMsg.Trim();
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                errorInSheet = SRM_WorkFlow_SheetNames.Email_Configuration;
                                isAPIErrorProcessed = true;
                            }
                            else if (errorInSheet.Equals(SRM_WorkFlow_SheetNames.Email_Configuration, StringComparison.OrdinalIgnoreCase))
                            {
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                            }
                            else
                                row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Not_Processed;
                        }
                        else if (inValidWorkflowNameVsSheetNameVsError.ContainsKey(workflow))
                        {
                            if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey("processed"))
                            {
                                remarks = "";
                                status = SRMMigrationStatus.Not_Processed;

                            }
                            else if (!isMissingWorkflowMappingProccessed && inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRMErrorMessages.Incomplete_Insert_Workflow))
                            {
                                remarks = SRMErrorMessages.Workflow_Mapping_Does_Not_Exist_In_All_Sheets;
                                status = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow].Add("processed", "");
                                isMissingWorkflowMappingProccessed = true;
                            }
                            else if (inValidWorkflowNameVsSheetNameVsError[workflow].ContainsKey(SRM_WorkFlow_SheetNames.Email_Configuration))
                            {
                                remarks = inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Email_Configuration];
                                if (remarks.Equals("processed"))
                                    remarks = string.Empty;
                                status = SRMMigrationStatus.Failed;
                                inValidWorkflowNameVsSheetNameVsError[workflow][SRM_WorkFlow_SheetNames.Email_Configuration] = "processed";
                            }
                            else
                            {
                                foreach (var item in inValidWorkflowNameVsSheetNameVsError[workflow])
                                {
                                    if (inValidWorkflowNameVsSheetNameVsError[workflow][item.Key].Equals("processed", StringComparison.OrdinalIgnoreCase))
                                    {
                                        isProccessed = true;
                                        break;
                                    }
                                }

                                if (isProccessed)
                                    status = SRMMigrationStatus.Not_Processed;
                                else
                                {
                                    remarks = "Error in sheet : " + inValidWorkflowNameVsSheetNameVsError[workflow].Keys.First();
                                    status = SRMMigrationStatus.Failed;
                                }
                            }
                        }
                        else
                            status = SRMMigrationStatus.Passed;

                        if (!(Convert.ToString(row[SRMSpecialColumnNames.Sync_Status]).Equals(SRMMigrationStatus.Already_In_Sync, StringComparison.OrdinalIgnoreCase) &&
                                               status.Equals(SRMMigrationStatus.Passed, StringComparison.OrdinalIgnoreCase)) && !isAPIError)
                        {
                            //if (remarks.IndexOf("Error in sheet", StringComparison.OrdinalIgnoreCase) != -1 || !isMissingWorkflowMappingProccessed)
                            //{
                            //    if (string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) || (remarks.IndexOf(Convert.ToString(row[SRMSpecialColumnNames.Remarks]), StringComparison.OrdinalIgnoreCase) == -1))
                            //        row[SRMSpecialColumnNames.Remarks] += SRMMigrationSeparators.Remarks_Separator + remarks;
                            //}
                            row[SRMSpecialColumnNames.Sync_Status] = status;
                        }
                    }
                }

            }
            #endregion

        }

        private ActionDataDetails GetActionDataDetails(DataRow actionDataRow)
        {
            ActionDataDetails actionDataDetails = new ActionDataDetails();
            actionDataDetails.RadWorkflowId = Convert.ToInt32(actionDataRow["rad_workflow_id"]);
            actionDataDetails.RuleSetId = Convert.ToInt32(actionDataRow["rule_set_id"]);
            actionDataDetails.ActionName = Convert.ToString(actionDataRow["action_name"]);
            if (actionDataRow["application_url_in_footer"] != DBNull.Value)
                actionDataDetails.KeepApplicationURLInTheFooter = Convert.ToBoolean(actionDataRow["application_url_in_footer"]);
            else
                actionDataDetails.KeepApplicationURLInTheFooter = false;
            if (actionDataRow["consolidated_email_for_bulk_action"] != DBNull.Value)
                actionDataDetails.SendConsolidatedEmailForBulkAction = Convert.ToBoolean(actionDataRow["consolidated_email_for_bulk_action"]);
            else
                actionDataDetails.SendConsolidatedEmailForBulkAction = false;
            if (actionDataRow["keep_creator_in_cc"] != DBNull.Value)
                actionDataDetails.KeepCreatorInCC = Convert.ToBoolean(actionDataRow["keep_creator_in_cc"]);
            else
                actionDataDetails.KeepCreatorInCC = false;

            actionDataDetails.To = Convert.ToString(actionDataRow["to_Details"]);
            actionDataDetails.Subject = Convert.ToString(actionDataRow["subject_details"]);
            actionDataDetails.BulkSubject = Convert.ToString(actionDataRow["bulk_subject_details"]);
            actionDataDetails.MailBodyTitle = "";
            actionDataDetails.MailBodyContent = Convert.ToString(actionDataRow["mail_body_content"]);
            actionDataDetails.DataSectionAttributes.Add(Convert.ToString(actionDataRow["attribute_id"]));
            if (actionDataRow["display_name"] != DBNull.Value)
            {
                if (!AttributeIdVsAttributeName.ContainsKey(Convert.ToInt32(actionDataRow["attribute_id"])))
                    AttributeIdVsAttributeName.Add(Convert.ToInt32(actionDataRow["attribute_id"]), Convert.ToString(actionDataRow["display_name"]));
            }

            return actionDataDetails;
        }

        private ActionDataDetails GetActionDataDetailsForObjectRow(int moduleID, ObjectRow actionDataRow, bool whileUploading = true)
        {
            ActionDataDetails action = new ActionDataDetails();
            try
            {
                if (whileUploading)
                {
                    bool isError = false;
                    string workflowName = Convert.ToString(actionDataRow["Workflow Name"]);

                    int typeId = dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeIds.Count();
                    string typeName = string.Empty;
                    if (typeId == 1)
                        typeName = dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeName.Trim();


                    if (actionDataRow["Include Action"] != DBNull.Value)
                    {
                        if (Convert.ToString(actionDataRow["Include Action"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                            action.CheckBoxForEachAction = true;
                        else
                            action.CheckBoxForEachAction = false;
                    }
                    else
                        action.CheckBoxForEachAction = false;

                    action.ActionName = Convert.ToString(actionDataRow["Action Name"]);

                    if (actionDataRow["Keep Application URL In Footer"] != DBNull.Value)
                    {
                        if (Convert.ToString(actionDataRow["Keep Application URL In Footer"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                            action.KeepApplicationURLInTheFooter = true;
                        else
                            action.KeepApplicationURLInTheFooter = false;
                    }
                    else
                        action.KeepApplicationURLInTheFooter = false;

                    if (actionDataRow["Send Consolidated Email For Bulk Action"] != DBNull.Value)
                    {
                        if (Convert.ToString(actionDataRow["Send Consolidated Email For Bulk Action"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                            action.SendConsolidatedEmailForBulkAction = true;
                        else
                            action.SendConsolidatedEmailForBulkAction = false;
                    }
                    else
                        action.SendConsolidatedEmailForBulkAction = false;

                    if (actionDataRow["Keep Creator In CC"] != DBNull.Value)
                    {
                        if (Convert.ToString(actionDataRow["Keep Creator In CC"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                            action.KeepCreatorInCC = true;
                        else
                            action.KeepCreatorInCC = false;
                    }
                    else
                        action.KeepCreatorInCC = false;

                    if (action.CheckBoxForEachAction)
                    {
                        Regex regex = new Regex(@"^((\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)*([,])*)*$");
                        Match match = regex.Match(Convert.ToString(actionDataRow["To"]));
                        if (match.Success)
                            action.To = Convert.ToString(actionDataRow["To"]);
                        else
                        {
                            addErrorToRow(actionDataRow, SRM_WorkFlow_SheetNames.Email_Configuration, workflowName, "Invalid email id configured");
                        }
                    }
                    else
                        action.To = Convert.ToString(actionDataRow["To"]);

                    action.Subject = Convert.ToString(actionDataRow["Subject"]);

                    if (action.SendConsolidatedEmailForBulkAction)
                    {
                        action.BulkSubject = Convert.ToString(actionDataRow["Bulk Subject"]);
                    }
                    else
                    {
                        string rulePriority = Convert.ToString(actionDataRow["Rule Priority"]);
                        if (rulePriority == "")
                            rulePriority = "-1";
                        string configuredBulkSubject = workflowNameVsRulePriorityVsActionNameVsBulkSubject[workflowName][rulePriority][action.ActionName];
                        string modifiedBulkSubject = Convert.ToString(actionDataRow["Bulk Subject"]);
                        if (configuredBulkSubject != modifiedBulkSubject)
                            addErrorToRow(actionDataRow, SRM_WorkFlow_SheetNames.Email_Configuration, workflowName, SRMErrorMessages.Bulk_Action_Not_Configured);
                        else
                            action.BulkSubject = modifiedBulkSubject;
                    }

                    action.MailBodyTitle = "";
                    action.MailBodyContent = Convert.ToString(actionDataRow["Mail Body Content"]);
                    string DataSectionAttribute = Convert.ToString(actionDataRow["Data Section Attributes"]);
                    if (action.CheckBoxForEachAction && DataSectionAttribute != "")
                    {
                        if (WorkflowNameVsAttributes.ContainsKey(workflowName))
                        {
                            if (!WorkflowNameVsAttributes[workflowName].Contains(DataSectionAttribute, StringComparer.InvariantCultureIgnoreCase) && !AttributeNameVsAttributeId.ContainsKey(DataSectionAttribute))
                            {
                                addErrorToRow(actionDataRow, SRM_WorkFlow_SheetNames.Email_Configuration, workflowName, "Invalid attribute mapped");
                                isError = true;
                            }
                        }
                        if (!isError)
                        {
                            if (AttributeNameVsAttributeId.ContainsKey(DataSectionAttribute) && !action.DataSectionAttributes.Contains(Convert.ToString(AttributeNameVsAttributeId[DataSectionAttribute])))
                                action.DataSectionAttributes.Add(Convert.ToString(AttributeNameVsAttributeId[DataSectionAttribute]));
                            else
                            {
                                if (moduleID == 3)
                                {
                                    if (typeId == 1)
                                    {
                                        action.DataSectionAttributes.Add(Convert.ToString(sectypeTypeVsAttributesInfo[typeName].AttributeInfo.MasterAttrs[DataSectionAttribute].AttrId));
                                    }
                                    else
                                        action.DataSectionAttributes.Add(Convert.ToString(commonAttributeNameVsInfo[DataSectionAttribute].AttrId));
                                }
                                else
                                {
                                    action.DataSectionAttributes.Add(Convert.ToString(entityTypeVsAttributesInfo[typeName].Attributes[DataSectionAttribute].EntityAttributeID));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (action.CheckBoxForEachAction && DataSectionAttribute == "")
                        {
                            addErrorToRow(actionDataRow, SRM_WorkFlow_SheetNames.Email_Configuration, workflowName, "No data section attribute added.");
                        }
                    }
                }
                else
                {
                    string workflowName = Convert.ToString(actionDataRow["Workflow Name"]);
                    action.ActionName = Convert.ToString(actionDataRow["Action Name"]);
                    if (actionDataRow["Include Action"] != DBNull.Value)
                    {
                        if (Convert.ToString(actionDataRow["Include Action"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                            action.CheckBoxForEachAction = true;
                        else
                            action.CheckBoxForEachAction = false;
                    }
                    else
                        action.CheckBoxForEachAction = false;

                    action.ActionName = Convert.ToString(actionDataRow["Action Name"]);

                    if (actionDataRow["Keep Application URL In Footer"] != DBNull.Value)
                    {
                        if (Convert.ToString(actionDataRow["Keep Application URL In Footer"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                            action.KeepApplicationURLInTheFooter = true;
                        else
                            action.KeepApplicationURLInTheFooter = false;
                    }
                    else
                        action.KeepApplicationURLInTheFooter = false;

                    if (actionDataRow["Send Consolidated Email For Bulk Action"] != DBNull.Value)
                    {
                        if (Convert.ToString(actionDataRow["Send Consolidated Email For Bulk Action"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                            action.SendConsolidatedEmailForBulkAction = true;
                        else
                            action.SendConsolidatedEmailForBulkAction = false;
                    }
                    else
                        action.SendConsolidatedEmailForBulkAction = false;

                    if (actionDataRow["Keep Creator In CC"] != DBNull.Value)
                    {
                        if (Convert.ToString(actionDataRow["Keep Creator In CC"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                            action.KeepCreatorInCC = true;
                        else
                            action.KeepCreatorInCC = false;
                    }
                    else
                        action.KeepCreatorInCC = false;

                    action.To = Convert.ToString(actionDataRow["To"]);
                    action.Subject = Convert.ToString(actionDataRow["Subject"]);
                    action.BulkSubject = Convert.ToString(actionDataRow["Bulk Subject"]);
                    action.MailBodyTitle = "";
                    action.MailBodyContent = Convert.ToString(actionDataRow["Mail Body Content"]);
                    string DataSectionAttribute = Convert.ToString(actionDataRow["Data Section Attributes"]);
                    if (!String.IsNullOrEmpty(DataSectionAttribute))
                    {
                        if (AttributeNameVsAttributeId.ContainsKey(DataSectionAttribute))
                            action.DataSectionAttributes.Add(Convert.ToString(AttributeNameVsAttributeId[DataSectionAttribute]));
                        else
                        {
                            int numberOfInstrumentTypes = dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeIds.Count();
                            string instrumentTypeName = string.Empty;
                            if (numberOfInstrumentTypes == 1)
                                instrumentTypeName = dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeName.Trim();
                            if (moduleID == 3)
                            {
                                if (numberOfInstrumentTypes == 1)
                                {
                                    action.DataSectionAttributes.Add(Convert.ToString(sectypeTypeVsAttributesInfo[instrumentTypeName].AttributeInfo.MasterAttrs[DataSectionAttribute].AttrId));
                                }
                                else
                                    action.DataSectionAttributes.Add(Convert.ToString(commonAttributeNameVsInfo[DataSectionAttribute].AttrId));
                            }
                            else
                            {
                                action.DataSectionAttributes.Add(Convert.ToString(entityTypeVsAttributesInfo[instrumentTypeName].Attributes[DataSectionAttribute].EntityAttributeID));
                            }
                        }
                    }

                    string rulePriority = Convert.ToString(actionDataRow["Rule Priority"]);
                    if (rulePriority == "")
                        rulePriority = "-1";
                    string actionName = Convert.ToString(actionDataRow["Action Name"]);
                    string bulkSubject = Convert.ToString(actionDataRow["Bulk Subject"]);
                    if (!workflowNameVsRulePriorityVsActionNameVsBulkSubject.ContainsKey(workflowName))
                        workflowNameVsRulePriorityVsActionNameVsBulkSubject.Add(workflowName, new Dictionary<string, Dictionary<string, string>>());
                    if (!workflowNameVsRulePriorityVsActionNameVsBulkSubject[workflowName].ContainsKey(rulePriority))
                        workflowNameVsRulePriorityVsActionNameVsBulkSubject[workflowName].Add(rulePriority, new Dictionary<string, string>());
                    if (!workflowNameVsRulePriorityVsActionNameVsBulkSubject[workflowName][rulePriority].ContainsKey(actionName))
                        workflowNameVsRulePriorityVsActionNameVsBulkSubject[workflowName][rulePriority].Add(actionName, bulkSubject);

                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            return action;
        }

        private void UpdateActionDataSectionAttributes(ActionDataDetails actionDataDetails, String dataSectionAttributes, string AttributeDisplayName = null)
        {
            if (AttributeDisplayName != "")
            {
                if (!(AttributeIdVsAttributeName.ContainsKey(Convert.ToInt32(dataSectionAttributes))))
                    AttributeIdVsAttributeName.Add(Convert.ToInt32(dataSectionAttributes), Convert.ToString(AttributeDisplayName));
            }
            if (actionDataDetails != null && actionDataDetails.DataSectionAttributes != null)
                actionDataDetails.DataSectionAttributes.Add(dataSectionAttributes);
        }

        private void UpdateActionDataSectionAttributesForObjectRow(ActionDataDetails actionDataDetails, String dataSectionAttributes, int moduleID, string workflowName, ObjectRow row, Dictionary<string, ActionDataDetails> actionDetailsDictionary, bool whileUploading = true)
        {
            try
            {
                if (whileUploading)
                {
                    bool isError = false;
                    string actionName = Convert.ToString(row["Action Name"]);

                    if (actionDetailsDictionary != null && actionName == actionDataDetails.ActionName && actionDataDetails.CheckBoxForEachAction)
                    {
                        isError = ValidateMultipleRowsForSameAction(actionDetailsDictionary, row, actionName, workflowName);
                    }


                    if (!isError && actionDataDetails.CheckBoxForEachAction)
                    {
                        int numberOfInstrumentTypes = dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeIds.Count;
                        string typeName = string.Empty;
                        if (numberOfInstrumentTypes == 1)
                            typeName = dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeName;

                        if (actionDataDetails != null && actionDataDetails.DataSectionAttributes != null)
                        {
                            if (WorkflowNameVsAttributes.ContainsKey(workflowName))
                            {
                                if (!WorkflowNameVsAttributes[workflowName].Contains(dataSectionAttributes, StringComparer.InvariantCultureIgnoreCase) && !AttributeNameVsAttributeId.ContainsKey(dataSectionAttributes))
                                {
                                    addErrorToRow(row, SRM_WorkFlow_SheetNames.Email_Configuration, workflowName, "Invalid attribute mapped");
                                    isError = true;
                                }
                            }
                            if (!isError)
                            {
                                if (AttributeNameVsAttributeId.ContainsKey(dataSectionAttributes) && !actionDataDetails.DataSectionAttributes.Contains(Convert.ToString(AttributeNameVsAttributeId[dataSectionAttributes])))
                                    actionDataDetails.DataSectionAttributes.Add(Convert.ToString(AttributeNameVsAttributeId[dataSectionAttributes]));
                                else
                                {
                                    if (moduleID == 3)
                                    {
                                        if (numberOfInstrumentTypes == 1)
                                        {
                                            actionDataDetails.DataSectionAttributes.Add(Convert.ToString(sectypeTypeVsAttributesInfo[typeName].AttributeInfo.MasterAttrs[dataSectionAttributes].AttrId));
                                        }
                                        else
                                            actionDataDetails.DataSectionAttributes.Add(Convert.ToString(commonAttributeNameVsInfo[dataSectionAttributes].AttrId));
                                    }
                                    else
                                    {
                                        actionDataDetails.DataSectionAttributes.Add(Convert.ToString(entityTypeVsAttributesInfo[typeName].Attributes[dataSectionAttributes].EntityAttributeID));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (AttributeNameVsAttributeId.ContainsKey(dataSectionAttributes))
                        actionDataDetails.DataSectionAttributes.Add(Convert.ToString(AttributeNameVsAttributeId[dataSectionAttributes]));
                    else
                    {
                        int numberOfInstrumentTypes = dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeIds.Count;
                        string instrumentTypeName = string.Empty;
                        if (numberOfInstrumentTypes == 1)
                            instrumentTypeName = dictDB_WorkflowNameVWorkflowInfo[workflowName].TypeName;
                        if (moduleID == 3)
                        {
                            if (numberOfInstrumentTypes == 1)
                            {
                                actionDataDetails.DataSectionAttributes.Add(Convert.ToString(sectypeTypeVsAttributesInfo[instrumentTypeName].AttributeInfo.MasterAttrs[dataSectionAttributes].AttrId));
                            }
                            else
                                actionDataDetails.DataSectionAttributes.Add(Convert.ToString(commonAttributeNameVsInfo[dataSectionAttributes].AttrId));
                        }
                        else
                        {
                            actionDataDetails.DataSectionAttributes.Add(Convert.ToString(entityTypeVsAttributesInfo[instrumentTypeName].Attributes[dataSectionAttributes].EntityAttributeID));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
        }

        public List<WorkflowEmailState> GetWorkflowEmailActions(int moduleID, List<WorkflowDetails> workflowDetails)
        {
            mLogger.Debug("GetWorkflowEmailActions - Start");
            string guid = Guid.NewGuid().ToString();
            string workflowDetailsTableName = "IVPRefMaster.dbo.[workflowDetailsTable_" + guid + "]";
            try
            {
                ObjectTable workflowDetailsTable = new ObjectTable();
                workflowDetailsTable.Columns.Add("WorkflowName", typeof(string));
                workflowDetailsTable.Columns.Add("RulePriority", typeof(int));
                foreach (var workflow in workflowDetails)
                {
                    var row = workflowDetailsTable.NewRow();
                    row["WorkflowName"] = workflow.WorkflowName;
                    row["RulePriority"] = workflow.RulePriority;
                    workflowDetailsTable.Rows.Add(row);
                }

                CommonDALWrapper.ExecuteQuery(string.Format(@"CREATE TABLE {0}(WorkflowName varchar(max),RulePriority int)", workflowDetailsTableName), CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
                CommonDALWrapper.ExecuteBulkUploadObject(workflowDetailsTableName, workflowDetailsTable, ConnectionConstants.RefMaster_Connection);

                DataSet ds = null;
                if (moduleID == 3)
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery(DALWrapperAppend.Replace(string.Format(@"SELECT DISTINCT ec.action_name, wr.rad_workflow_id,wdtn.WorkflowName,wdtn.RulePriority,wr.rule_mapping_id,wr.rule_set_id, ec.application_url_in_footer, ec.consolidated_email_for_bulk_action, 
                ec.keep_creator_in_cc, ec.to_Details, ec.subject_details, ec.bulk_subject_details, ec.mail_body_title, ec.mail_body_content, ad.attribute_id, td.display_name,ad.detail_id
                from IVPRefMaster.dbo.ivp_srm_workflow_instance wi
				inner join IVPRefMaster.dbo.ivp_srm_workflow_rules wr
				on wi.workflow_instance_id=wr.workflow_instance_id and wi.is_active=1
                left JOIN IVPRefMaster.dbo.ivp_srm_workflow_email_configuration ec
                ON wr.rule_mapping_id = ec.rule_mapping_details_id
                left join IVPRefMaster.dbo.ivp_srm_workflow_email_body_attribute_details ad
                on ec.action_mapping_id = ad.action_mapping_id
                left join IVPSecMaster.dbo.ivp_secm_template_details td
				on ad.attribute_id=td.attribute_id and td.is_active=1
                inner join {0} wdtn
                on wdtn.WorkflowName=wi.workflow_instance_name and wdtn.RulePriority=wr.priority
                order by ad.detail_id", workflowDetailsTableName)), ConnectionConstants.RefMaster_Connection);
                }
                else
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery(DALWrapperAppend.Replace(string.Format(@"SELECT DISTINCT ec.action_name, wr.rad_workflow_id,wdtn.WorkflowName,wdtn.RulePriority,wr.rule_mapping_id,wr.rule_set_id, ec.application_url_in_footer, ec.consolidated_email_for_bulk_action, 
                ec.keep_creator_in_cc, ec.to_Details, ec.subject_details, ec.bulk_subject_details, ec.mail_body_title, ec.mail_body_content, ad.attribute_id, td.display_name,ad.detail_id
                from IVPRefMaster.dbo.ivp_srm_workflow_instance wi
				inner join IVPRefMaster.dbo.ivp_srm_workflow_rules wr
				on wi.workflow_instance_id=wr.workflow_instance_id and wi.is_active=1
                left JOIN IVPRefMaster.dbo.ivp_srm_workflow_email_configuration ec
                ON wr.rule_mapping_id = ec.rule_mapping_details_id
                left join IVPRefMaster.dbo.ivp_srm_workflow_email_body_attribute_details ad
                on ec.action_mapping_id = ad.action_mapping_id
                left join IVPRefMaster.dbo.ivp_refm_entity_attribute td
				on ad.attribute_id=td.entity_attribute_id and td.is_active=1
                inner join {0} wdtn
                on wdtn.WorkflowName=wi.workflow_instance_name and wdtn.RulePriority=wr.priority
                order by ad.detail_id", workflowDetailsTableName)), ConnectionConstants.RefMaster_Connection);
                }

                Dictionary<string, Dictionary<int, Dictionary<string, ActionDataDetails>>> workflowNameVsRulePriorityVsActionNameVsActionDetails = new Dictionary<string, Dictionary<int, Dictionary<string, ActionDataDetails>>>();

                string workFlowName, actionName;
                int rulePriority;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    workFlowName = Convert.ToString(row["WorkflowName"]);
                    if (!workflowNameVsRulePriorityVsActionNameVsActionDetails.ContainsKey(workFlowName))
                        workflowNameVsRulePriorityVsActionNameVsActionDetails.Add(workFlowName, new Dictionary<int, Dictionary<string, ActionDataDetails>>());

                    rulePriority = Convert.ToInt32(row["RulePriority"]);
                    if (!workflowNameVsRulePriorityVsActionNameVsActionDetails[workFlowName].ContainsKey(rulePriority))
                        workflowNameVsRulePriorityVsActionNameVsActionDetails[workFlowName].Add(rulePriority, new Dictionary<string, ActionDataDetails>());

                    actionName = Convert.ToString(row["action_name"]);
                    if (!workflowNameVsRulePriorityVsActionNameVsActionDetails[workFlowName][rulePriority].ContainsKey(actionName))
                        workflowNameVsRulePriorityVsActionNameVsActionDetails[workFlowName][rulePriority].Add(actionName, GetActionDataDetails(row));
                    else
                        UpdateActionDataSectionAttributes(workflowNameVsRulePriorityVsActionNameVsActionDetails[workFlowName][rulePriority][actionName], Convert.ToString(row["attribute_id"]), Convert.ToString(row["display_name"]));

                }

                List<WorkflowEmailState> result = new List<WorkflowEmailState>();

                foreach (WorkflowDetails workflow in workflowDetails)
                {
                    string WorkflowName = workflow.WorkflowName;
                    int RulePriority = workflow.RulePriority;
                    int RadWorkflowId = 0;
                    int RuleSetId = 0;


                    Dictionary<int, Dictionary<string, ActionDataDetails>> ValuesAgainstWorkflowName = new Dictionary<int, Dictionary<string, ActionDataDetails>>();
                    if (workflowNameVsRulePriorityVsActionNameVsActionDetails.ContainsKey(workflow.WorkflowName))
                        ValuesAgainstWorkflowName = workflowNameVsRulePriorityVsActionNameVsActionDetails[WorkflowName];

                    Dictionary<string, ActionDataDetails> ValuesAgainstRulePriority = new Dictionary<string, ActionDataDetails>(); ;
                    if (ValuesAgainstWorkflowName.ContainsKey(workflow.RulePriority))
                        ValuesAgainstRulePriority = ValuesAgainstWorkflowName[RulePriority];

                    ActionDataDetails ValueAgainstActionName = new ActionDataDetails();
                    var value = ValuesAgainstRulePriority.Values;

                    foreach (ActionDataDetails v in value)
                    {
                        RadWorkflowId = v.RadWorkflowId;
                        RuleSetId = v.RuleSetId;
                    }

                    WorkflowEmailState workflowEmailState = new WorkflowEmailState();

                    RWorkFlowInfo workflowInfo = new RWorkFlowService().GetWorkflowInfoByIdorName(RadWorkflowId, null);
                    HashSet<string> workflowNameList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    workflowNameList.Add("Creator initiates the request");
                    foreach (WorkflowState state in workflowInfo.WorkflowStates)
                    {
                        foreach (StateActionInfo action in state.StateActions)
                        {
                            workflowNameList.Add(state.StateName + " - " + action.Action);
                        }
                    }
                    workflowNameList.Add("Creator cancels the request");

                    workflowEmailState.Actions = new List<ActionDataDetails>();
                    workflowEmailState.RulePriority = Convert.ToInt32(workflow.RulePriority);
                    workflowEmailState.RuleSetId = RuleSetId;
                    workflowEmailState.WorkflowName = WorkflowName;
                    foreach (ActionDataDetails entry in value)
                    {
                        string actionNameInternal = entry.ActionName;
                        if (workflowNameList.Contains(actionNameInternal))
                        {
                            ActionDataDetails rs = new ActionDataDetails();
                            if (rs.DataSectionAttributes.Count <= 0) //fetch from the first attribute common for all action
                            {
                                rs.ActionName = entry.ActionName;
                                rs.CheckBoxForEachAction = true;
                                rs.KeepApplicationURLInTheFooter = entry.KeepApplicationURLInTheFooter;
                                rs.SendConsolidatedEmailForBulkAction = entry.SendConsolidatedEmailForBulkAction;
                                rs.KeepCreatorInCC = entry.KeepCreatorInCC;
                                rs.To = entry.To;
                                rs.Subject = entry.Subject;
                                rs.BulkSubject = entry.BulkSubject;
                                rs.MailBodyTitle = "";
                                rs.MailBodyContent = entry.MailBodyContent;
                            }
                            foreach (string i in entry.DataSectionAttributes)
                                rs.DataSectionAttributes.Add(i);

                            workflowEmailState.Actions.Add(rs);
                            workflowNameList.Remove(actionNameInternal);
                        }
                    }
                    if (workflowNameList.Count != 0)
                    {
                        foreach (var row in workflowNameList)
                        {
                            ActionDataDetails rs = new ActionDataDetails();
                            rs.ActionName = row;
                            rs.CheckBoxForEachAction = false;
                            rs.KeepApplicationURLInTheFooter = false;
                            rs.SendConsolidatedEmailForBulkAction = false;
                            rs.KeepCreatorInCC = false;
                            rs.To = "";
                            rs.Subject = "";
                            rs.BulkSubject = "";
                            rs.MailBodyTitle = "";
                            if (moduleID == 3)
                                rs.MailBodyContent = "Please be informed that there is a status change for the following security";
                            else
                                rs.MailBodyContent = "Please be informed that there is a status change for the following entity";
                            rs.DataSectionAttributes = null;
                            workflowEmailState.Actions.Add(rs);
                        }
                    }

                    result.Add(workflowEmailState);

                }

                return result;
            }
            catch (Exception ee)
            {
                mLogger.Error("GetWorkflowEmailActions - Exception - " + ee.ToString());
                return new List<WorkflowEmailState>();
            }
            finally
            {
                mLogger.Debug("GetWorkflowEmailActions - End");
                CommonDALWrapper.ExecuteQuery(string.Format(@"IF (OBJECT_ID('{0}') IS NOT NULL) DROP TABLE {0}", workflowDetailsTableName), CommonQueryType.Delete, ConnectionConstants.RefMaster_Connection);
            }
        }

        public List<SaveResponse> SaveWorkflowEmailAction(List<SelectedActionsInfo> selectedActionsInfo)
        {
            mLogger.Debug("SaveWorkflowEmailAction - Start");
            List<SaveResponse> res = new List<SaveResponse>();
            SaveResponse m = new SaveResponse();
            string Message = "";
            string guid = Guid.NewGuid().ToString();
            string workflowNameAndRulePriorityTableName = "IVPRefMaster.dbo.[workflowNameAndRulePriorityTable_" + guid + "]";
            string workflowEmailConfigurationTableName = "IVPRefMaster.dbo.[workflowEmailConfigurationTable_" + guid + "]";
            string workflowEmailBodyAttributeDetailsTableName = "IVPRefMaster.dbo.[workflowEmailBodyAttributeDetailsTable_" + guid + "]";
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                ObjectTable workflowNameAndRulePriority = new ObjectTable();
                ObjectTable workflowEmailConfigurationTable = new ObjectTable();
                ObjectTable workflowEmailBodyAttributeDetails = new ObjectTable();

                workflowNameAndRulePriority.Columns.Add("workflowname_and_rule_priority_id", typeof(int));
                workflowNameAndRulePriority.Columns.Add("workflow_name", typeof(string));
                workflowNameAndRulePriority.Columns.Add("rule_priority", typeof(int));

                workflowEmailConfigurationTable.Columns.Add("workflowname_and_rule_priority_id", typeof(int));
                workflowEmailConfigurationTable.Columns.Add("application_url_in_footer", typeof(bool));
                workflowEmailConfigurationTable.Columns.Add("consolidated_email_for_bulk_action", typeof(bool));
                workflowEmailConfigurationTable.Columns.Add("keep_creator_in_cc", typeof(bool));
                workflowEmailConfigurationTable.Columns.Add("action_name", typeof(string));
                workflowEmailConfigurationTable.Columns.Add("to_Details", typeof(string));
                workflowEmailConfigurationTable.Columns.Add("subject_details", typeof(string));
                workflowEmailConfigurationTable.Columns.Add("bulk_subject_details", typeof(string));
                workflowEmailConfigurationTable.Columns.Add("mail_body_title", typeof(string));
                workflowEmailConfigurationTable.Columns.Add("mail_body_content", typeof(string));

                workflowEmailBodyAttributeDetails.Columns.Add("workflowname_and_rule_priority_id", typeof(int));
                workflowEmailBodyAttributeDetails.Columns.Add("action_name", typeof(string));
                workflowEmailBodyAttributeDetails.Columns.Add("mail_body_attributes", typeof(int));

                int id = 1;
                foreach (SelectedActionsInfo selectedAction in selectedActionsInfo)
                {
                    if (selectedAction != null)
                    {
                        id++;
                        var wrp = workflowNameAndRulePriority.NewRow();
                        wrp["workflowname_and_rule_priority_id"] = id;
                        wrp["workflow_name"] = selectedAction.WorkflowName;
                        wrp["rule_priority"] = selectedAction.RulePriority;
                        workflowNameAndRulePriority.Rows.Add(wrp);

                        foreach (var action in selectedAction.SaveConfigurationForActions)
                        {
                            var ndr = workflowEmailConfigurationTable.NewRow();
                            ndr["workflowname_and_rule_priority_id"] = id;
                            ndr["application_url_in_footer"] = action.KeepApplicationURLInTheFooter;
                            ndr["consolidated_email_for_bulk_action"] = action.SendConsolidatedEmailForBulkAction;
                            ndr["keep_creator_in_cc"] = action.KeepCreatorInCC;
                            ndr["action_name"] = action.ActionName;
                            ndr["to_Details"] = action.To;
                            ndr["subject_details"] = action.Subject;
                            ndr["bulk_subject_details"] = action.BulkSubject;
                            ndr["mail_body_title"] = "";
                            ndr["mail_body_content"] = action.MailBodyContent;

                            foreach (var attr in action.DataSectionAttributes)
                            {
                                if (!String.IsNullOrEmpty(attr))
                                {
                                    var dsa = workflowEmailBodyAttributeDetails.NewRow();
                                    dsa["workflowname_and_rule_priority_id"] = id;
                                    dsa["action_name"] = action.ActionName;
                                    dsa["mail_body_attributes"] = Convert.ToInt32(attr);
                                    workflowEmailBodyAttributeDetails.Rows.Add(dsa);
                                }
                            }

                            workflowEmailConfigurationTable.Rows.Add(ndr);
                        }

                    }
                }


                CommonDALWrapper.ExecuteQuery(string.Format(@"CREATE TABLE {0}(workflowname_and_rule_priority_id int,workflow_name VARCHAR(MAX),rule_priority int)", workflowNameAndRulePriorityTableName), CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);

                CommonDALWrapper.ExecuteQuery(string.Format(@"CREATE TABLE {0}(workflowname_and_rule_priority_id int,application_url_in_footer bit,consolidated_email_for_bulk_action bit,keep_creator_in_cc bit,action_name VARCHAR(MAX),to_Details VARCHAR(MAX),
                    subject_details VARCHAR(MAX), bulk_subject_details VARCHAR(MAX), mail_body_title VARCHAR(MAX),mail_body_content VARCHAR(MAX))", workflowEmailConfigurationTableName), CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);

                CommonDALWrapper.ExecuteQuery(string.Format(@"CREATE TABLE {0}(workflowname_and_rule_priority_id int,action_name VARCHAR(MAX),mail_body_attributes int)", workflowEmailBodyAttributeDetailsTableName), CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);

                CommonDALWrapper.ExecuteBulkUploadObject(workflowNameAndRulePriorityTableName, workflowNameAndRulePriority, ConnectionConstants.RefMaster_Connection);
                CommonDALWrapper.ExecuteBulkUploadObject(workflowEmailConfigurationTableName, workflowEmailConfigurationTable, ConnectionConstants.RefMaster_Connection);
                CommonDALWrapper.ExecuteBulkUploadObject(workflowEmailBodyAttributeDetailsTableName, workflowEmailBodyAttributeDetails, ConnectionConstants.RefMaster_Connection);

                DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_SaveUpdateWorkflowEmailConfiguration] '{0}', '{1}','{2}'", workflowNameAndRulePriorityTableName, workflowEmailConfigurationTableName, workflowEmailBodyAttributeDetailsTableName), con);

                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    Message = Convert.ToString(dr["message"]);
                    m.Message = Message;
                }
                con.CommitTransaction();
                res.Add(m);

                return res;
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
                Message = ee.Message;

                m.Message = Message;
                res.Add(m);

                return res;
            }
            finally
            {
                mLogger.Debug("SaveWorkflowEmailAction - End");
                CommonDALWrapper.ExecuteQuery(string.Format(@"IF (OBJECT_ID('{0}') IS NOT NULL) DROP TABLE {0}", workflowNameAndRulePriorityTableName), CommonQueryType.Delete, con);
                CommonDALWrapper.ExecuteQuery(string.Format(@"IF (OBJECT_ID('{0}') IS NOT NULL) DROP TABLE {0}", workflowEmailConfigurationTableName), CommonQueryType.Delete, con);
                CommonDALWrapper.ExecuteQuery(string.Format(@"IF (OBJECT_ID('{0}') IS NOT NULL) DROP TABLE {0}", workflowEmailBodyAttributeDetailsTableName), CommonQueryType.Delete, con);
                CommonDALWrapper.PutConnectionManager(con);
            }

        }

        private Boolean ValidateMultipleRowsForSameAction(Dictionary<string, ActionDataDetails> actionDetailsDictionary, ObjectRow row, string actionName, string workflowName)
        {
            bool isError = false;
            var KeepApplicationURLInTheFooter = false;
            var SendConsolidatedEmailForBulkAction = false;
            var KeepCreatorInCC = false;
            ActionDataDetails actionDetails = actionDetailsDictionary[actionName];

            if (Convert.ToString(row["Keep Application URL In Footer"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                KeepApplicationURLInTheFooter = true;
            else
                KeepApplicationURLInTheFooter = false;

            if (Convert.ToString(row["Send Consolidated Email For Bulk Action"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                SendConsolidatedEmailForBulkAction = true;
            else
                SendConsolidatedEmailForBulkAction = false;

            if (Convert.ToString(row["Keep Creator In CC"]).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                KeepCreatorInCC = true;
            else
                KeepCreatorInCC = false;

            if (actionDetails.KeepApplicationURLInTheFooter != KeepApplicationURLInTheFooter || actionDetails.SendConsolidatedEmailForBulkAction != SendConsolidatedEmailForBulkAction || actionDetails.KeepCreatorInCC != KeepCreatorInCC || actionDetails.Subject != Convert.ToString(row["Subject"]) || actionDetails.BulkSubject != Convert.ToString(row["Bulk Subject"]) || /*actionDetails.MailBodyTitle != Convert.ToString(row["Mail Body Title"]) ||*/ actionDetails.MailBodyContent != Convert.ToString(row["Mail Body Content"]))
            {
                addErrorToRow(row, SRM_WorkFlow_SheetNames.Email_Configuration, workflowName, "Same action cannot have multiple configurations.");
                isError = true;
            }
            return isError;
        }

        public class SaveResponse
        {
            public string Message { get; set; }
        }

        public class SelectedActionsInfo
        {
            public string WorkflowName { get; set; }
            public int RulePriority { get; set; }
            public ActionDataDetails[] SaveConfigurationForActions { get; set; } = new ActionDataDetails[] { };

        }


        public class WorkflowInfo
        {
            public int InstanceId { get; set; }
            public string Name { get; set; }
            public int ModuleId { get; set; }
            public int WorkflowActionTypeId { get; set; }
            public string WorkflowActionTypeName { get; set; }
            public List<string> TypeIds { get; set; }
            public string TypeName { get; set; }
            public List<string> PrimaryAttributeIds { get; set; }
            public List<string> OtherAttributeIds { get; set; }
            public string UserName { get; set; }
            public bool isSave { get; set; }
            public List<WorkflowRulesInfo> WorkflowRulesInfo { get; set; }
            public List<WorkflowRulesInfo> WorkflowRulesInfoForActions { get; set; }

        }
        public class WorkflowRulesInfo
        {
            public string WorkflowRuleSetID { get; set; }
            public string RadWorkFlowInstanceID { get; set; }
            public string RadWorkFlowInstanceName { get; set; }
            public string moduleId { get; set; }
            public string WorkFlowRulePriority { get; set; }
            public bool SRMWorkFlowRuleState { get; set; }
            public string SRMWorkFlowRuleText { get; set; }
            public bool SRMWorkFlowIsDefault { get; set; }
            public string workflowInstanceId { get; set; }
            public string attributes_in_rule { get; set; }
            public List<RuleStateInfo> RuleStateInfo { get; set; }
            public List<ActionDataDetails> ActionStateInfo { get; set; } = new List<ActionDataDetails>();

        }
        public class RuleStateInfo
        {
            public string stateName { get; set; }
            public bool mandatoryData { get; set; }
            public bool uniquenessValidation { get; set; }
            public bool primaryKeyValidation { get; set; }
            public bool validations { get; set; }
            public bool alerts { get; set; }
            public bool basketValidation { get; set; }
            public bool basketAlert { get; set; }
        }
        public class WorkflowEmailState
        {
            public int RuleSetId { get; set; }
            public List<ActionDataDetails> Actions { get; set; }
            public int RulePriority { get; set; }
            public string WorkflowName { get; set; }
        }

        public class ActionDataDetails
        {
            public int RuleSetId { get; set; }
            public bool CheckBoxForEachAction { get; set; }
            public int RadWorkflowId { get; set; }
            public bool KeepApplicationURLInTheFooter { get; set; }
            public bool SendConsolidatedEmailForBulkAction { get; set; }
            public bool KeepCreatorInCC { get; set; }
            public string ActionName { get; set; }
            public string To { get; set; }
            public string Subject { get; set; }
            public string BulkSubject { get; set; }
            public string MailBodyTitle { get; set; }
            public string MailBodyContent { get; set; }
            public List<string> DataSectionAttributes { get; set; } = new List<string>();

        }
        public class WorkflowDetails
        {
            public string WorkflowName { get; set; }
            public int RulePriority { get; set; }

        }

    }
}
