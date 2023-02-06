using com.ivp.commom.commondal;
using com.ivp.common.migration;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.controls.xruleeditor.grammar;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.RRadWorkflow;
using com.ivp.rad.viewmanagement;
using com.ivp.rad.xruleengine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;



namespace com.ivp.common.CommonUI.CommonUI.BaseUserControls.Service
{
    /// <summary>
    /// Summary description for WorkflowService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WorkflowService : System.Web.Services.WebService
    {
        public static Dictionary<string, Dictionary<string, WorkflowRulesInfo>> WorkflowRulesDetails = new Dictionary<string, Dictionary<string, WorkflowRulesInfo>>();
        public static Dictionary<string, DateTime> WorkflowRulesDetailsTimeStamp = new Dictionary<string, DateTime>();
        static System.Timers.Timer timer = new System.Timers.Timer(30000);

        private static IRLogger mLogger = RLogFactory.CreateLogger("WorkflowService");
        public WorkflowService()
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }


        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<string> guidsToDelete = new List<string>();
            var now = DateTime.Now;
            foreach (var kvp in WorkflowRulesDetailsTimeStamp)
            {
                if ((now - kvp.Value).TotalSeconds >= 1800)
                {
                    guidsToDelete.Add(kvp.Key);
                }
            }

            foreach (var guid in guidsToDelete)
            {
                WorkflowRulesDetails.Remove(guid);
                WorkflowRulesDetailsTimeStamp.Remove(guid);
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Workflow[] GetAllWorkflows()
        {
            List<Workflow> workflowDetails = new List<Workflow>();

            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataSet result = CommonDALWrapper.ExecuteSelectQuery("exec [IVPRefMaster].[dbo].[SRM_GetAllWorkflows]", con);

                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    workflowDetails.Add(new Workflow
                    {
                        InstanceId = Convert.ToInt32(dr["workflow_instance_id"]),
                        Name = Convert.ToString(dr["workflow_instance_name"]),
                        ModuleId = Convert.ToInt32(dr["module_id"]),
                        ModuleName = Convert.ToString(dr["module_name"]),
                        WorkflowActionTypeId = Convert.ToInt32(dr["workflow_action_type_id"]),
                        WorkflowActionTypeName = Convert.ToString(dr["workflow_action_type_name"]),
                        WorkflowIsCreate = Convert.ToBoolean(dr["workflow_is_create"]),
                        WorkflowIsUpdate = Convert.ToBoolean(dr["workflow_is_update"]),
                        WorkflowIsTimeSeries = Convert.ToBoolean(dr["workflow_is_time_series"]),
                        RaiseForNonEmptyValue = Convert.ToBoolean(dr["raise_for_non_empty_value"]),
                        WorkflowIsDelete = Convert.ToBoolean(dr["workflow_is_delete"]),
                        TypeIds = Convert.ToString(dr["TypeIds"]),
                        PrimaryAttributeIds = Convert.ToString(dr["PrimaryAttributeIds"]),
                        OtherAttributeIds = Convert.ToString(dr["OtherAttributeIds"])
                    });
                }

                con.CommitTransaction();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }

            return workflowDetails.ToArray();
        }



        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public WorkFlowPendingRequest[] GetAllWorkfowPendingRequest()
        {
            List<WorkFlowPendingRequest> workflowsRequest = new List<WorkFlowPendingRequest>();
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT distinct(workflow_instance_id) AS 'workflow_instance_id' FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue WHERE is_active = 1"), con);


                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    workflowsRequest.Add(new WorkFlowPendingRequest
                    {
                        workflow_instance_id = Convert.ToInt32(dr["workflow_instance_id"])
                    });
                }
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }

            return workflowsRequest.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public WorkFlowPendingRequest[] GetAllRADWorkfowPendingRequest()
        {
            List<WorkFlowPendingRequest> workflowsRequest = new List<WorkFlowPendingRequest>();
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT DISTINCT ru.rad_workflow_id
                    FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
                    INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_rules ru on rq.workflow_instance_id = ru.workflow_instance_id
                    where rq.is_active = 1"), con);


                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    workflowsRequest.Add(new WorkFlowPendingRequest
                    {
                        workflow_instance_id = Convert.ToInt32(dr["rad_workflow_id"])
                    });
                }
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }

            return workflowsRequest.ToArray();
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetSpecificWorkfowPendingRequest(int workflowInstanceId)
        {
            bool message = false;
            string InstanceType = "WorkflowInstance";

            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                message = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"IF EXISTS(SELECT 1 FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue WHERE workflow_instance_id = {0} AND is_active = 1)
                                                                            SELECT CAST(1 AS BIT) AS 'has_pending_request'
                                                                            ELSE
                                                                            SELECT CAST(0 AS BIT) AS 'has_pending_request'
                                                                            ", workflowInstanceId), con).Tables[0].AsEnumerable().Select(x => x.Field<bool>("has_pending_request")).FirstOrDefault();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }

            return new JavaScriptSerializer().Serialize(message);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetSpecificRADWorkfowPendingRequest(int radWorkflowInstanceId)
        {
            bool message = false;
            string InstanceType = "RADInstance";

            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                message = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"IF EXISTS(SELECT 1
                                                                            FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
                                                                            INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_rules ru on rq.workflow_instance_id = ru.workflow_instance_id
                                                                            where ru.rad_workflow_id = {0} AND rq.is_active = 1)
                                                                            SELECT CAST(1 AS BIT) AS 'has_pending_request'
                                                                            ELSE
                                                                            SELECT CAST(0 AS BIT) AS 'has_pending_request'
                                                                            ", radWorkflowInstanceId), con).Tables[0].AsEnumerable().Select(x => x.Field<bool>("has_pending_request")).FirstOrDefault();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }
            return new JavaScriptSerializer().Serialize(message);
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public WorkflowModule[] GetWorkflowModules()
        {
            List<WorkflowModule> workflowModules = new List<WorkflowModule>();

            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataSet result = CommonDALWrapper.ExecuteSelectQuery("select * from [IVPRefMaster].[dbo].[ivp_srm_modules]", con);

                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    workflowModules.Add(new WorkflowModule
                    {
                        ModuleId = Convert.ToInt32(dr["module_id"]),
                        ModuleName = Convert.ToString(dr["module_name"])
                    });
                }

                con.CommitTransaction();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }

            return workflowModules.ToArray();
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public WorkflowActionType[] GetWorkflowActionTypes()
        {
            List<WorkflowActionType> workflowActionTypes = new List<WorkflowActionType>();

            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataSet result = CommonDALWrapper.ExecuteSelectQuery("select * from [IVPRefMaster].[dbo].[ivp_srm_workflow_action_type]", con);

                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    workflowActionTypes.Add(new WorkflowActionType
                    {
                        ActionTypeId = Convert.ToInt32(dr["workflow_action_type_id"]),
                        ActionTypeName = Convert.ToString(dr["workflow_action_type_name"]),
                        ModuleId = Convert.ToInt16(dr["module_id"])
                    });
                }

                con.CommitTransaction();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }

            return workflowActionTypes.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Object InsertWorkflow(Workflow workflow, List<RuleStateDataCollection> rsc)
        {
            string Message = "";
            long InstanceId = 0;
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                //'{0}', { 1}, { 2}, { 3}, '{4}', { 5}, '{6}', '{7}', '{8}'", "", 0, 0, false, userName,workflowInstanceId,types,primaryAttribute,otherAttributes ), con);
                DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_SaveUpdateWorkflow] '{0}', {1}, {2}, {3}, '{4}', {5}, '{6}', '{7}', '{8}'", workflow.Name,
                    workflow.ModuleId, workflow.WorkflowActionTypeId, true, workflow.UserName, workflow.InstanceId, workflow.TypeIds, workflow.PrimaryAttributeIds, workflow.OtherAttributeIds), con);

                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    Message = Convert.ToString(dr["message"]);
                    InstanceId = Convert.ToInt64(dr["InstanceId"]);
                }
                btn_SaveUpdateWorkFlowWithRulesClick(workflow.Guid, Convert.ToInt32(InstanceId), con, true);


                if (InstanceId != 0)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append(@"    
                                 Declare @tempRuleDetailsTable table(rule_set_id INT, stage_name VARCHAR(MAX), apply_mandatory BIT, apply_uniqueness BIT, apply_primary BIT, apply_validation BIT, apply_alert BIT, apply_basket_validation BIT, apply_basket_alert BIT)
                                 INSERT INTO @tempRuleDetailsTable (rule_set_id,stage_name, apply_mandatory, apply_uniqueness, apply_primary, apply_validation, apply_alert, apply_basket_validation, apply_basket_alert)VALUES");

                    foreach (var item in rsc)
                    {
                        foreach (var itemState in item.statesRuleState)
                        {
                            sb.Append(@"(").Append(item.rulesetid);
                            sb.Append(",'").Append(Convert.ToString(itemState.stateName));
                            sb.Append("',").Append(Convert.ToBoolean(itemState.mandatoryData) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.uniquenessValidation) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.primaryKeyValidation) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.validations) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.alerts) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.basketValidation) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.basketAlert) ? 1 : 0);
                            sb.Append(@"),");
                        }
                    }
                    sb = sb.Remove(sb.Length - 1, 1); sb.Append(";");

                    sb.Append(@"
                        INSERT INTO IVPRefMaster.dbo.ivp_srm_workflow_rules_details(stage_name, apply_mandatory, apply_uniqueness, apply_primary, apply_validation, apply_alert, apply_basket_validation, apply_basket_alert, rule_mapping_id) 
                        SELECT tb.stage_name, tb.apply_mandatory, tb.apply_uniqueness, tb.apply_primary, tb.apply_validation, tb.apply_alert, tb.apply_basket_validation, tb.apply_basket_alert, trt.rule_mapping_id
                        FROM @tempRuleDetailsTable tb 
                        INNER JOIN @tempRulesTable trt
                        ON trt.rule_set_id = tb.rule_set_id 
                    ");

                    DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"
                    Declare @tempRulesTable table(rule_set_id INT, rule_mapping_id INT)
                    INSERT INTO @tempRulesTable(rule_set_id, rule_mapping_id)
	                       Select rule_set_id, rule_mapping_id from IVPRefMaster.dbo.ivp_srm_workflow_rules where workflow_instance_id = {0}

                    {1}
                    ", InstanceId, sb.ToString()), con);
                }
                con.CommitTransaction();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
                Message = ee.Message;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }

            return new
            {
                Message = Message,
                InstanceId = InstanceId,
                name = workflow.Name,
                moduleId = workflow.ModuleId,
                moduleName = workflow.ModuleName,
                workflowActionTypeId = workflow.WorkflowActionTypeId,
                workflowActionTypeName = workflow.WorkflowActionTypeName,
                TypeIds = workflow.TypeIds,
                PrimaryAttributeIds = workflow.PrimaryAttributeIds,
                OtherAttributeIds = workflow.OtherAttributeIds
            };
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Type[] GetAllTypes(int moduleId, string userName)
        {
            List<Type> types = new List<Type>();

            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_GetWorkflowTypeDetails] '{0}', {1}, {2}, {3}", "Type", moduleId, 0, userName), con);

                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    types.Add(new Type
                    {
                        TypeId = Convert.ToInt32(dr["type_id"]),
                        TypeName = Convert.ToString(dr["type_name"])
                    });
                }

                con.CommitTransaction();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }


            return types.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public WorkflowAttribute[] GetPrimaryAttributes(int moduleId, string typeIds, bool includeTypeAttribute)
        {
            List<WorkflowAttribute> attributes = new List<WorkflowAttribute>();

            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                if (includeTypeAttribute)
                {
                    DataSet sectTypeSpecificAttributes = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_GetWorkflowTypeDetails] '{0}', {1}, {2}", "Primary Attribute", moduleId, typeIds.Substring(0, typeIds.IndexOf("|"))), con);

                    foreach (DataRow dr in sectTypeSpecificAttributes.Tables[0].Rows)
                    {
                        attributes.Add(new WorkflowAttribute
                        {
                            TypeId = Convert.ToInt32(typeIds.Substring(0, typeIds.IndexOf("|"))),
                            AttributeId = Convert.ToInt32(dr["attribute_id"]),
                            AttributeName = Convert.ToString(dr["display_name"]),
                        });
                    }
                }
                else
                {
                    DataSet commonAttributes = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_GetWorkflowTypeDetails] '{0}', {1}", "Common Attribute", moduleId), con);

                    foreach (DataRow dr in commonAttributes.Tables[0].Rows)
                    {
                        attributes.Add(new WorkflowAttribute
                        {
                            TypeId = Convert.ToInt32(dr["type_id"]),
                            AttributeId = Convert.ToInt32(dr["attribute_id"]),
                            AttributeName = Convert.ToString(dr["display_name"])
                        });
                    }
                }


                con.CommitTransaction();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }


            return attributes.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public WorkflowAttribute[] GetOtherAttributes(int moduleId, string typeIds, bool includeTypeAttribute)
        {
            List<WorkflowAttribute> attributes = new List<WorkflowAttribute>();

            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                if (includeTypeAttribute)
                {
                    DataSet sectTypeSpecificAttributes = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_GetWorkflowTypeDetails] '{0}', {1}, {2}", "Primary Attribute", moduleId, typeIds.Substring(0, typeIds.IndexOf("|"))), con);

                    foreach (DataRow dr in sectTypeSpecificAttributes.Tables[0].Rows)
                    {
                        attributes.Add(new WorkflowAttribute
                        {
                            TypeId = Convert.ToInt32(typeIds.Substring(0, typeIds.IndexOf("|"))),
                            AttributeId = Convert.ToInt32(dr["attribute_id"]),
                            AttributeName = Convert.ToString(dr["display_name"]),
                        });
                    }
                }
                else
                {
                    DataSet commonAttributes = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_GetWorkflowTypeDetails] '{0}', {1}", "Common Attribute", moduleId), con);

                    foreach (DataRow dr in commonAttributes.Tables[0].Rows)
                    {
                        attributes.Add(new WorkflowAttribute
                        {
                            TypeId = Convert.ToInt32(dr["type_id"]),
                            AttributeId = Convert.ToInt32(dr["attribute_id"]),
                            AttributeName = Convert.ToString(dr["display_name"])
                        });
                    }
                }


                con.CommitTransaction();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }


            return attributes.ToArray();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Workflow GetWorkflowDetails(int instanceId, string Guid)
        {
            Workflow workflow = new Workflow();

            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_GetAllWorkflows] {0}", instanceId), con);
                //DataSet result = CommonDALWrapper.ExecuteSelectQuery("select * from ivp_srm_workflow_instance where workflow_instance_id = "+ instanceId, con);

                //    var workflow = {
                //"instanceId": $(this).data('instanceid'), name: $(this).data('name'), moduleId: $(this).data('moduleid'), moduleName: $(this).data('modulename'), workflowActionTypeId: $(this).data('workflowactiontypeid'),
                //workflowActionTypeName: $(this).data('workflowactiontypename'), workflowIsCreate: $(this).data('workflowiscreate'), workflowIsUpdate: $(this).data('workflowisupdate'), workflowIsTimeSeries: $(this).data('workflowistimeseries'),
                //raiseForNonEmptyValue: $(this).data('raisefornonemptyvalue'), workflowIsDelete: $(this).data('workflowisdelete')
                //}
                if (result.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = result.Tables[0].Rows[0];
                    workflow.InstanceId = Convert.ToInt32(dr["workflow_instance_id"]);
                    workflow.Name = Convert.ToString(dr["workflow_instance_name"]);
                    workflow.ModuleId = Convert.ToInt32(dr["module_id"]);
                    workflow.ModuleName = Convert.ToString(dr["module_name"]);
                    workflow.WorkflowActionTypeId = Convert.ToInt32(dr["workflow_action_type_id"]);
                    workflow.WorkflowActionTypeName = Convert.ToString(dr["workflow_action_type_name"]);
                    workflow.WorkflowIsCreate = Convert.ToBoolean(dr["workflow_is_create"]);
                    workflow.WorkflowIsUpdate = Convert.ToBoolean(dr["workflow_is_update"]);
                    workflow.WorkflowIsTimeSeries = Convert.ToBoolean(dr["workflow_is_time_series"]);
                    workflow.RaiseForNonEmptyValue = Convert.ToBoolean(dr["raise_for_non_empty_value"]);
                    workflow.WorkflowIsDelete = Convert.ToBoolean(dr["workflow_is_delete"]);
                    workflow.TypeIds = Convert.ToString(dr["TypeIds"]);
                    workflow.PrimaryAttributeIds = Convert.ToString(dr["PrimaryAttributeIds"]);
                    workflow.OtherAttributeIds = Convert.ToString(dr["PrimaryAttributeIds"]);
                }

                con.CommitTransaction();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }


            return workflow;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public WorkflowRulesInfoCollection[] GetAllWorkflowsRules(int instanceId, int moduleId, string Guid)
        {
            List<WorkflowRulesInfoCollection> rulesCollection = new List<WorkflowRulesInfoCollection>();
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            string colUsed, ruleSetId, rad_workflow_id, priority;
            try
            {

                DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @instance_id INT = {0}
DECLARE @module_id INT = {1}

	SELECT r.rad_workflow_id, r.rule_set_id, x.rule_id, x.rule_name, x.rule_text, r.priority, x.rule_state, r.attributes_in_rule, CAST(0 AS BIT) AS 'is_default'
	FROM IVPRefMaster.dbo.ivp_srm_workflow_rules r
	INNER JOIN IVPRefMasterVendor.dbo.ivp_rad_xrule x ON r.rule_set_id = x.rule_set_id AND r.workflow_instance_id = @instance_id AND x.is_active = 1
	WHERE r.workflow_instance_id = @instance_id
	UNION
	SELECT rad_workflow_id AS 'rad_workflow_id', rule_set_id AS 'rule_set_id', null AS 'rule_id', null AS 'rule_name', null AS 'rule_text', priority AS 'priority', 1 AS 'rule_state', attributes_in_rule AS 'attributes_in_rule', CAST(1 AS BIT) AS 'is_default'
	FROM IVPRefMaster.dbo.ivp_srm_workflow_rules 
	WHERE workflow_instance_id = @instance_id AND rule_set_id = -1
	ORDER BY rule_name", instanceId, moduleId), con);

                if (result.Tables[0].Rows.Count > 0)
                {
                    var radWorkflowsIdVsName = new RWorkFlowService().GetAllWorkFLows().ToDictionary(x => x.WorkflowID, y => y.WorkflowName);

                    foreach (DataRow dr in result.Tables[0].Rows)
                    {
                        var radWorkflowId = Convert.ToString(dr["rad_workflow_id"]);
                        var RuleSet_ID = Convert.ToString(dr["rule_set_id"]);
                        var RadWorkFlowInstance_Name = radWorkflowsIdVsName[Convert.ToInt32(radWorkflowId)];
                        var module_Id = Convert.ToString(moduleId);
                        var WorkFlowRule_Priority = Convert.ToString(dr["priority"]);
                        var SRMWorkFlowRuleState_ID = RuleSet_ID + "_imgChangeState";
                        var SRMWorkFlow_RuleState = Convert.ToBoolean(dr["rule_state"]);
                        var SRMWorkFlowRuleUpdate_ID = RuleSet_ID + "_imgUpdateState";
                        var SRMWorkFlowRuleDelete_ID = RuleSet_ID + "_imgDeleteState";
                        var SRMWorkFlowRule_Text = (Convert.ToString(dr["rule_text"]).Trim().Replace("\\", "\\\\")).Replace("END", "").Replace("{", "").Replace("}", "");
                        var SRMWorkFlow_IsDefault = Convert.ToBoolean(dr["is_default"]);

                        rulesCollection.Add(new WorkflowRulesInfoCollection()
                        {
                            WorkflowRuleSetID = RuleSet_ID,
                            RadWorkFlowInstanceID = radWorkflowId,
                            RadWorkFlowInstanceName = RadWorkFlowInstance_Name,
                            moduleId = module_Id,
                            WorkFlowRuleName = Convert.ToString(dr["rule_name"]),
                            WorkFlowRulePriority = WorkFlowRule_Priority,
                            SRMWorkFlowRuleStateID = SRMWorkFlowRuleState_ID,
                            SRMWorkFlowRuleState = SRMWorkFlow_RuleState,
                            SRMWorkFlowRuleUpdateID = SRMWorkFlowRuleUpdate_ID,
                            SRMWorkFlowRuleDeleteID = SRMWorkFlowRuleDelete_ID,
                            SRMWorkFlowRuleText = SRMWorkFlowRule_Text,
                            SRMWorkFlowIsDefault = SRMWorkFlow_IsDefault
                        });

                        colUsed = Convert.ToString(dr["attributes_in_rule"]);
                        ruleSetId = Convert.ToString(dr["rule_set_id"]);
                        rad_workflow_id = Convert.ToString(dr["rad_workflow_id"]);
                        priority = Convert.ToString(dr["priority"]);

                        //Save rules data in collection
                        saveWorkFlowRuleDetailsCollection(RuleSet_ID, radWorkflowId, RadWorkFlowInstance_Name, module_Id, WorkFlowRule_Priority, SRMWorkFlow_RuleState, SRMWorkFlowRule_Text, SRMWorkFlow_IsDefault, colUsed, Guid, "ADD");
                    }
                }

                con.CommitTransaction();

            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }


            return rulesCollection.ToArray();
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Object UpdateWorkflow(string workflowInstanceId, string types, string primaryAttribute, string otherAttributes, string userName, string Guid, string moduleId, string actionTypeId, List<RuleStateDataCollection> rsc)
        {
            string Message = "";
            long InstanceId = 0;
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataSet result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"exec [IVPRefMaster].[dbo].[SRM_SaveUpdateWorkflow] '{0}', {1}, {2}, {3}, '{4}', {5}, '{6}', '{7}', '{8}'", "", Convert.ToInt32(moduleId), Convert.ToInt32(actionTypeId), false, userName, Convert.ToInt32(workflowInstanceId), types, primaryAttribute, otherAttributes), con);

                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    Message = Convert.ToString(dr["message"]);
                    InstanceId = Convert.ToInt32(dr["InstanceId"]);
                }

                //btn_SaveUpdateWorkFlowWithRulesClick(Guid, Convert.ToInt32(workflowInstanceId), con, true);
                if (InstanceId != 0 && rsc.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append(@"    
                                 Declare @tempRuleDetailsTable table(rule_set_id INT, stage_name VARCHAR(MAX), apply_mandatory BIT, apply_uniqueness BIT, apply_primary BIT, apply_validation BIT, apply_alert BIT, apply_basket_validation BIT, apply_basket_alert BIT)
                                 INSERT INTO @tempRuleDetailsTable (rule_set_id,stage_name, apply_mandatory, apply_uniqueness, apply_primary, apply_validation, apply_alert, apply_basket_validation, apply_basket_alert)VALUES");

                    foreach (var item in rsc)
                    {
                        foreach (var itemState in item.statesRuleState)
                        {
                            sb.Append(@"(").Append(item.rulesetid);
                            sb.Append(",'").Append(Convert.ToString(itemState.stateName));
                            sb.Append("',").Append(Convert.ToBoolean(itemState.mandatoryData) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.uniquenessValidation) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.primaryKeyValidation) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.validations) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.alerts) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.basketValidation) ? 1 : 0);
                            sb.Append(",").Append(Convert.ToBoolean(itemState.basketAlert) ? 1 : 0);
                            sb.Append(@"),");
                        }
                    }
                    sb = sb.Remove(sb.Length - 1, 1); sb.Append(";");

                    sb.Append(@"
                        DELETE wrd
                        FROM IVPRefMaster.dbo.ivp_srm_workflow_rules_details wrd
                        INNER JOIN @tempRulesTable trt
                        ON trt.rule_mapping_id = wrd.rule_mapping_id
                        INNER JOIN @tempRuleDetailsTable tb
                        ON tb.rule_set_id = trt.rule_set_id

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
                    ", InstanceId, sb.ToString()), con);
                }

                con.CommitTransaction();
            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
                Message = ee.Message;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }

            return new
            {
                Message = Message,
                InstanceId = workflowInstanceId,
                primaryAttribute = primaryAttribute,
                otherAttributes = otherAttributes,
                types = types
            };
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string saveWorkFlowRuleDetailsCollection(string WorkflowRuleSetID, string RadWorkFlowInstanceID, string RadWorkFlowInstanceName, string moduleId, string WorkFlowRulePriority, bool SRMWorkFlowRuleState, string SRMWorkFlowRuleText, bool SRMWorkFlowIsDefault, string attributes_in_rule, string Guid, string action)
        {
            string key = "";
            WorkflowRulesInfo obj = new WorkflowRulesInfo();
            string status = "";
            action = action.ToUpper();
            try
            {
                if (action.ToLower().Equals("add") || action.ToLower().Equals("update"))
                {
                    key = WorkflowRuleSetID;
                    obj.WorkflowRuleSetID = WorkflowRuleSetID;
                    obj.RadWorkFlowInstanceID = RadWorkFlowInstanceID;
                    obj.RadWorkFlowInstanceName = RadWorkFlowInstanceName;
                    obj.moduleId = moduleId;
                    obj.SRMWorkFlowRuleText = SRMWorkFlowRuleText.Trim().Replace("\\", "\\\\").Replace("END", "").Replace("{", "").Replace("}", "");
                    obj.WorkFlowRulePriority = WorkFlowRulePriority;
                    obj.SRMWorkFlowRuleStateID = WorkflowRuleSetID + "_imgChangeState";
                    obj.SRMWorkFlowRuleState = SRMWorkFlowRuleState;
                    obj.SRMWorkFlowRuleUpdateID = WorkflowRuleSetID + "_imgUpdateState";
                    obj.SRMWorkFlowRuleDeleteID = WorkflowRuleSetID + "_imgDeleteState";
                    obj.SRMWorkFlowIsDefault = SRMWorkFlowIsDefault;
                    obj.attributes_in_rule = attributes_in_rule;
                }
                switch (action)
                {
                    case "ADD":
                        if (!WorkflowRulesDetails.ContainsKey(Guid))
                            WorkflowRulesDetails[Guid] = new Dictionary<string, WorkflowRulesInfo>();

                        if (!WorkflowRulesDetailsTimeStamp.ContainsKey(Guid))
                            WorkflowRulesDetailsTimeStamp[Guid] = DateTime.Now;

                        if (!WorkflowRulesDetails[Guid].ContainsKey(key))
                            WorkflowRulesDetails[Guid][key] = obj;
                        else
                            return "FAILURE";
                        break;
                    case "UPDATE":
                        if (WorkflowRulesDetails[Guid].ContainsKey(key))
                            WorkflowRulesDetails[Guid].Remove(key);
                        WorkflowRulesDetails[Guid][key] = obj;
                        WorkflowRulesDetailsTimeStamp[Guid] = DateTime.Now;
                        break;
                    case "DELETE":
                        key = WorkflowRuleSetID;
                        WorkflowRulesDetails[Guid].Remove(key);
                        WorkflowRulesDetailsTimeStamp[Guid] = DateTime.Now;
                        break;
                }


            }
            catch (Exception ex) { }
            return status;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<WorkflowRulesInfo> getWorkFlowRuleDetails(string Guid)
        {
            List<WorkflowRulesInfo> objRulesInfo = new List<WorkflowRulesInfo>();
            try
            {
                if (WorkflowRulesDetails.ContainsKey(Guid))
                {
                    foreach (var key in WorkflowRulesDetails[Guid])
                    {
                        objRulesInfo.Add(new WorkflowRulesInfo() { WorkflowRuleSetID = key.Value.WorkflowRuleSetID, RadWorkFlowInstanceID = key.Value.RadWorkFlowInstanceID, RadWorkFlowInstanceName = key.Value.RadWorkFlowInstanceName, moduleId = key.Value.moduleId, SRMWorkFlowRuleText = key.Value.SRMWorkFlowRuleText, WorkFlowRulePriority = key.Value.WorkFlowRulePriority, SRMWorkFlowRuleState = key.Value.SRMWorkFlowRuleState, SRMWorkFlowIsDefault = key.Value.SRMWorkFlowIsDefault, attributes_in_rule = key.Value.attributes_in_rule, SRMWorkFlowRuleStateID = key.Value.SRMWorkFlowRuleStateID, SRMWorkFlowRuleUpdateID = key.Value.SRMWorkFlowRuleUpdateID, SRMWorkFlowRuleDeleteID = key.Value.SRMWorkFlowRuleDeleteID });
                    }
                }
                if (objRulesInfo.Count > 0)
                    return objRulesInfo;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public WorkflowRulesInfo GetWorkflowRuleDataToUpdate(string ruleSetID, string guid)
        {
            WorkflowRulesInfo objRulesInfo = new WorkflowRulesInfo();

            if (WorkflowRulesDetails.ContainsKey(guid))
            {
                foreach (var key in WorkflowRulesDetails[guid])
                {
                    if (key.Equals(ruleSetID))
                    {
                        objRulesInfo = (new WorkflowRulesInfo() { WorkflowRuleSetID = key.Value.WorkflowRuleSetID, RadWorkFlowInstanceID = key.Value.RadWorkFlowInstanceID, RadWorkFlowInstanceName = key.Value.RadWorkFlowInstanceName, moduleId = key.Value.moduleId, SRMWorkFlowRuleText = key.Value.SRMWorkFlowRuleText, WorkFlowRulePriority = key.Value.WorkFlowRulePriority, SRMWorkFlowRuleState = key.Value.SRMWorkFlowRuleState, SRMWorkFlowIsDefault = key.Value.SRMWorkFlowIsDefault, attributes_in_rule = key.Value.attributes_in_rule, SRMWorkFlowRuleStateID = key.Value.SRMWorkFlowRuleStateID, SRMWorkFlowRuleUpdateID = key.Value.SRMWorkFlowRuleUpdateID, SRMWorkFlowRuleDeleteID = key.Value.SRMWorkFlowRuleDeleteID });
                    }
                }
                return objRulesInfo;
            }
            else
                return null;
        }
        public void DeleteAllRulesFromCollection(string Guid)
        {
            WorkflowRulesDetails.Remove(Guid);
            WorkflowRulesDetailsTimeStamp.Remove(Guid);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Object DeleteRuleFromDataBaseandCollection(string Guid, string ruleset_id, bool isDefault, bool isUpdate, int workFlowInstanceID)
        {

            string Message = "";
            int rule_id = 0;
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMasterVendor_Connection, true, IsolationLevel.RepeatableRead);
            RXRuleController objRXRuleController = new RXRuleController();
            objRXRuleController.DBConnectionId = RADConfigReader.GetConfigAppSettings(ConnectionConstants.RefMasterVendor_Connection);
            DataSet result = new DataSet();

            try
            {
                if (!isDefault && !isUpdate)
                {
                    result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT rule_id , 'Passed' AS [message] from IVPRefMastervendor.dbo.ivp_rad_xrule where rule_set_id = {0}", ruleset_id), con);

                    foreach (DataRow dr in result.Tables[0].Rows)
                    {
                        rule_id = Convert.ToInt32(dr["rule_id"]);
                        Message = Convert.ToString(dr["message"]);
                    }
                    objRXRuleController.DeleteRule(Convert.ToInt32(rule_id), con);
                    con.CommitTransaction();
                    saveWorkFlowRuleDetailsCollection(ruleset_id, "", "", "", "", false, "", false, "", Guid, "DELETE");
                }
                else if (!isDefault && isUpdate)
                {
                    result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"IF NOT EXISTS(SELECT 1 FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
					INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_rules wr ON rq.workflow_instance_id = wr.workflow_instance_id
					WHERE wr.rule_set_id = {0} AND rq.is_active = 1)
					BEGIN
					SELECT rule_id , 'Passed' AS [message] from IVPRefMastervendor.dbo.ivp_rad_xrule where rule_set_id = {0}
					END
					ELSE
					SELECT '-2' AS [rule_id], 'Failed' AS [message]", ruleset_id), con);

                    foreach (DataRow dr in result.Tables[0].Rows)
                    {
                        rule_id = Convert.ToInt32(dr["rule_id"]);
                        Message = Convert.ToString(dr["message"]);
                    }

                    if (Convert.ToInt32(rule_id) != -2)
                    {
                        objRXRuleController.DeleteRule(Convert.ToInt32(rule_id), con);
                        con.CommitTransaction();

                        RXRuleSetInfo RuleSetInfo = objRXRuleController.GetRuleSetInfo(Convert.ToInt32(ruleset_id));
                        if (RuleSetInfo != null && RuleSetInfo.Rules != null && RuleSetInfo.Rules.Count == 0)
                            saveWorkFlowRuleDetailsCollection(ruleset_id, "", "", "", "", false, "", false, "", Guid, "DELETE");
                    }
                }
                else if (isDefault)
                    saveWorkFlowRuleDetailsCollection("-1", "", "", "", "", false, "", false, "", Guid, "DELETE");

                if (workFlowInstanceID != 0)
                {
                    //btn_SaveUpdateWorkFlowWithRulesClick(Guid, workFlowInstanceID, null, false);

                    result = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @rule_mapping_id INT 
					SELECT @rule_mapping_id = rule_mapping_id FROM IVPRefMaster.dbo.ivp_srm_workflow_rules WHERE workflow_instance_id = {0} AND rule_set_id = {1}
 
					DELETE wrkdetails FROM IVPRefMaster.dbo.ivp_srm_workflow_rules_details wrkdetails WHERE wrkdetails.rule_mapping_id = @rule_mapping_id
					DELETE wrkrules FROM IVPRefMaster.dbo.ivp_srm_workflow_rules wrkrules WHERE wrkrules.rule_mapping_id = @rule_mapping_id ", workFlowInstanceID, ruleset_id), con);
                    con.CommitTransaction();
                }

            }
            catch (Exception ee)
            {
                con.RollbackTransaction();
                Message = ee.Message;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
                result = null;
            }
            return new
            {
                Message = Message,
            };

        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Object btn_SaveRuleInRADWorkFlowClick(string hdnRuleClassInfoValue, string module, string ruleInfo, string Guid, string rad_workflow_id, bool updateCase, int workFlowInstanceID) //  priority|ruleState|ruleId|rulesetID
        {
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMasterVendor_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                int ruleSetId = -1;
                int ruleId = 0;
                bool ruleState = true;

                RXRuleController objRXRuleController = new RXRuleController();
                RXRuleInfo mObjRuleInfo = new RXRuleInfo();

                var radWorkflowsIdVsName = new RWorkFlowService().GetAllWorkFLows().ToDictionary(x => x.WorkflowID, y => y.WorkflowName);
                var RadWorkFlowInstance_Name = radWorkflowsIdVsName[Convert.ToInt32(rad_workflow_id)];

                objRXRuleController.DBConnectionId = RADConfigReader.GetConfigAppSettings(ConnectionConstants.RefMasterVendor_Connection);

                if (!string.IsNullOrEmpty(hdnRuleClassInfoValue))
                {
                    string[] ruleClassInfo = Server.HtmlDecode(hdnRuleClassInfoValue).Split(new[] { "||$$||" }, StringSplitOptions.None);


                    //string ruleName = Convert.ToString(ruleInfo.Split('|')[0]);
                    int priority = Convert.ToInt32(ruleInfo.Split('|')[0]);

                    string action = updateCase ? "UPDATE" : "ADD";

                    ///Final string to be added to the database as rule_text                
                    string finalRuleString = ruleClassInfo[2];

                    if (updateCase)
                    {
                        ruleSetId = Convert.ToInt32(ruleInfo.Split('|')[1]);

                        ruleState = true; // Convert.ToBoolean(ruleInfo.Split('|')[3]);
                                          //ruleId = Convert.ToInt32(ruleInfo.Split('|')[4]);
                        ruleId = Convert.ToInt32(CommonDALWrapper.ExecuteSelectQuery(string.Format(@"IF NOT EXISTS(SELECT 1 FROM IVPRefMaster.dbo.ivp_srm_workflow_requests_queue rq
						INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_rules wr ON rq.workflow_instance_id = wr.workflow_instance_id
						WHERE wr.rule_set_id = {0} AND rq.is_active = 1)
						BEGIN
						SELECT rule_id , 'Passed' AS [message] from IVPRefMastervendor.dbo.ivp_rad_xrule where rule_set_id = {0}
						END
						ELSE
						SELECT '-2' AS [rule_id], 'Failed' AS [message]", ruleSetId), con).Tables[0].AsEnumerable().Select(x => x.Field<int>("rule_id")).FirstOrDefault());

                        mObjRuleInfo.RuleSetID = ruleSetId;
                    }
                    else
                        mObjRuleInfo.RuleSetID = 0;

                    mObjRuleInfo.RuleType = RXRuleType.Conditional;
                    mObjRuleInfo.RuleName = "WorkFlow Rule";
                    mObjRuleInfo.RulePriority = priority;
                    mObjRuleInfo.RuleText = finalRuleString;
                    mObjRuleInfo.CreatedBy = new RCommon().SessionInfo.LoginName;

                    if (!string.IsNullOrEmpty(ruleClassInfo[0]))
                    {
                        mObjRuleInfo.ClassText = new System.Text.StringBuilder(ruleClassInfo[0]);
                        mObjRuleInfo.ClassDetails = System.Xml.Linq.XDocument.Parse(ruleClassInfo[1]);
                    }
                    mObjRuleInfo.CreatedOn = DateTime.Now;

                    if (updateCase)
                    {
                        mObjRuleInfo.RuleState = true;
                        mObjRuleInfo.RuleID = Convert.ToInt32(ruleId);
                        mObjRuleInfo.DBOperationType = RXOperationType.Update;
                        updateCase = true;
                    }
                    else
                    {
                        mObjRuleInfo.RuleState = true;
                        mObjRuleInfo.RuleID = 0;
                        mObjRuleInfo.DBOperationType = RXOperationType.Insert;
                    }
                    mObjRuleInfo.IsActive = true;
                    mObjRuleInfo.RuleExecutionMode = RXRuleExecutionMode.Priority;


                    ruleSetId = objRXRuleController.SaveRule(mObjRuleInfo, con);
                    con.CommitTransaction();

                    //Get coulmns used
                    string colUsed = string.Empty;
                    Dictionary<int, List<string>> colsUsed = new RXRuleExecutor() { DBConnectionId = objRXRuleController.DBConnectionId }.GetColumnNameForRulesets(new List<int>() { ruleSetId });

                    if (colsUsed != null && colsUsed.Keys.Count > 0 && colsUsed[ruleSetId].Count > 0)
                    {
                        colUsed = "|" + string.Join("|", colsUsed[ruleSetId].ToArray()) + "|";
                    }
                    // Save Rule Data In Collection
                    if (!updateCase)
                        saveWorkFlowRuleDetailsCollection(Convert.ToString(ruleSetId), rad_workflow_id, RadWorkFlowInstance_Name, module, Convert.ToString(priority), mObjRuleInfo.RuleState, finalRuleString, false, colUsed, Guid, "ADD");
                    else
                        saveWorkFlowRuleDetailsCollection(Convert.ToString(ruleSetId), rad_workflow_id, RadWorkFlowInstance_Name, module, Convert.ToString(priority), mObjRuleInfo.RuleState, finalRuleString, false, colUsed, Guid, "UPDATE");
                    // saveWorkFlowRuleDetailsCollection(colUsed, Convert.ToString(ruleSetId), rad_workflow_id, Convert.ToString(priority), Guid, action);

                    if (workFlowInstanceID != 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("INSERT INTO IVPRefMaster.dbo.ivp_srm_workflow_rules_details(stage_name, apply_mandatory, apply_uniqueness, apply_primary, apply_validation, apply_alert, apply_basket_validation, apply_basket_alert, rule_mapping_id) VALUES");
                        var workflowInfo = new RWorkFlowService().GetWorkflowInfo(RadWorkFlowInstance_Name);
                        dynamic workflowName = JObject.Parse(workflowInfo);
                        foreach (var row in workflowName.WorkflowStates)
                        {
                            if (Convert.ToString(row.StateName).ToLower() != "end")
                                sb.Append("('").Append(row.StateName).Append("',0,0,0,0,0,0,0,@rule_mapping_id),");
                        }

                        sb = sb.Remove(sb.Length - 1, 1);
                        sb.Append(";");


                        if (updateCase)
                        {
                            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @rule_mapping_id INT, @radWorkFlowID INT
								SELECT @rule_mapping_id = rule_mapping_id, @radWorkFlowID = rad_workflow_id FROM IVPRefMaster.dbo.ivp_srm_workflow_rules WHERE workflow_instance_id = {0} AND rule_set_id = {1}
 
								UPDATE IVPRefMaster.dbo.ivp_srm_workflow_rules 
								SET attributes_in_rule = '{2}', rule_set_id = {1}, rad_workflow_id = {3}, [priority] = {4}
								WHERE rule_mapping_id = @rule_mapping_id
								
								IF(@radWorkFlowID <> {3})
								BEGIN
									DELETE rd FROM IVPRefMaster.dbo.ivp_srm_workflow_rules_details rd WHERE rule_mapping_id = @rule_mapping_id 
								
									{5}
								END ", workFlowInstanceID, ruleSetId, colUsed, Convert.ToInt32(rad_workflow_id), Convert.ToInt32(priority), sb), con);
                        }
                        else
                        {
                            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @rule_mapping_id INT 
 
								INSERT INTO IVPRefMaster.dbo.ivp_srm_workflow_rules(workflow_instance_id, attributes_in_rule, rule_set_id, rad_workflow_id, [priority] )
								VALUES({0},'{2}',{1},{3},{4})
 
								SELECT @rule_mapping_id = rule_mapping_id FROM IVPRefMaster.dbo.ivp_srm_workflow_rules WHERE workflow_instance_id = {0} AND rule_set_id = {1}
								
								{5}
								", workFlowInstanceID, ruleSetId, colUsed, Convert.ToInt32(rad_workflow_id), Convert.ToInt32(priority), sb), con);
                        }
                        con.CommitTransaction();
                    }
                }
                else
                {
                    if (WorkflowRulesDetails.ContainsKey(Guid))
                    {
                        if (WorkflowRulesDetails[Guid].ContainsKey(Convert.ToString(ruleSetId)))
                            saveWorkFlowRuleDetailsCollection(Convert.ToString(ruleSetId), null, null, null, " - 1", true, "Default WorkFlow", true, null, Guid, "DELETE");
                    }
                    saveWorkFlowRuleDetailsCollection(Convert.ToString(ruleSetId), rad_workflow_id, RadWorkFlowInstance_Name, module, "-1", true, "Default WorkFlow", true, null, Guid, "ADD");
                    //  saveWorkFlowRuleDetailsCollection(Convert.ToString(ruleSetId), rad_workflow_id, "-1", Guid, "ADD");     // Save Rule Data In Collection for Default
                    if (workFlowInstanceID != 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        string s1 = "";
                        if (!updateCase) //  case when add default attribute for new wf or existing wf
                        {
                            btn_SaveUpdateWorkFlowWithRulesClick(Guid, workFlowInstanceID, con, false);

                            s1 = string.Format(@"INSERT INTO IVPRefMaster.dbo.ivp_srm_workflow_rules(workflow_instance_id, rule_set_id, rad_workflow_id, [priority] )
                                VALUES({0},{1},{2},{3});", workFlowInstanceID, ruleSetId, Convert.ToInt32(rad_workflow_id), "-1");
                        }

                        sb.Append(@"INSERT INTO IVPRefMaster.dbo.ivp_srm_workflow_rules_details(stage_name, apply_mandatory, 
                            apply_uniqueness, apply_primary, apply_validation, apply_alert, apply_basket_validation, apply_basket_alert, rule_mapping_id) VALUES");
                        var workflowInfo = new RWorkFlowService().GetWorkflowInfo(RadWorkFlowInstance_Name);
                        dynamic workflowName = JObject.Parse(workflowInfo);
                        foreach (var row in workflowName.WorkflowStates)
                        {
                            if (Convert.ToString(row.StateName).ToLower() != "end")
                                sb.Append("('").Append(row.StateName).Append("',0,0,0,0,0,0,0,@rule_mapping_id),");
                        }

                        sb = sb.Remove(sb.Length - 1, 1);
                        sb.Append(";");

                        DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @rule_mapping_id INT, @radWorkFlowID INT; {3}
					    SELECT @rule_mapping_id = rule_mapping_id, @radWorkFlowID = rad_workflow_id FROM IVPRefMaster.dbo.ivp_srm_workflow_rules WHERE workflow_instance_id = {0} AND rule_set_id = -1;
                        {4}
							 
						    UPDATE IVPRefMaster.dbo.ivp_srm_workflow_rules 
						    SET rad_workflow_id = {1}
						    WHERE rule_mapping_id = @rule_mapping_id
 
					    IF(@radWorkFlowID <> {1})
					    BEGIN
						    DELETE rd FROM IVPRefMaster.dbo.ivp_srm_workflow_rules_details rd WHERE rule_mapping_id = @rule_mapping_id 
								
					    {2}
					    END ", workFlowInstanceID, Convert.ToInt32(rad_workflow_id), sb, s1, (!updateCase ? sb.ToString() : "")), con);

                        con.CommitTransaction();
                    }
                }

            }

            catch (Exception ee)
            {
                con.RollbackTransaction();
                throw ee;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }
            return new
            {
                Message = workFlowInstanceID,
            };
        }

        protected void btn_SaveUpdateWorkFlowWithRulesClick(string Guid, int workflowInstanceId, RDBConnectionManager dbConnectionObj, bool toDeleteCollection)
        {
            bool setTransaction = false;
            bool commitTransaction = false;

            if (dbConnectionObj == null)
            {
                commitTransaction = true;
                setTransaction = true;
                dbConnectionObj = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMasterVendor_Connection, true, IsolationLevel.RepeatableRead);
            }
            try
            {
                List<WorkflowRulesInfo> lstWorkflowRules = new List<WorkflowRulesInfo>();
                DataTable workflow_rule_details = new DataTable();
                workflow_rule_details.Columns.Add("workflow_instance_id", typeof(int));
                workflow_rule_details.Columns.Add("attributes_in_rule", typeof(string));
                workflow_rule_details.Columns.Add("rule_set_id", typeof(int));
                workflow_rule_details.Columns.Add("rad_workflow_id", typeof(int));
                workflow_rule_details.Columns.Add("priority", typeof(int));

                lstWorkflowRules = getWorkFlowRuleDetails(Guid);
                foreach (var item in lstWorkflowRules)
                {
                    workflow_rule_details.Rows.Add(Convert.ToInt32(workflowInstanceId), item.attributes_in_rule, Convert.ToInt32(item.WorkflowRuleSetID), Convert.ToInt32(item.RadWorkFlowInstanceID), Convert.ToInt32(item.WorkFlowRulePriority));
                }
                var sb = new StringBuilder();
                foreach (var row in workflow_rule_details.AsEnumerable())
                {
                    sb.Append("INSERT INTO @tab VALUES(").Append(Convert.ToString(row["workflow_instance_id"])).Append(");");
                }

                if (toDeleteCollection && workflow_rule_details.Rows.Count > 0) // there are another rules
                {
                    CommonDALWrapper.ExecuteQuery(DALWrapperAppend.Replace(string.Format(@"DECLARE @tab TABLE(workflow_instance_id INT)
                        {0}

                        DELETE c
                        FROM IVPRefMaster.dbo.ivp_srm_workflow_rules c
                        INNER JOIN @tab tab ON c.workflow_instance_id = tab.workflow_instance_id", sb.ToString())), CommonQueryType.Delete, dbConnectionObj);

                    CommonDALWrapper.ExecuteBulkUpload("IVPRefMaster.dbo.ivp_srm_workflow_rules", workflow_rule_details, dbConnectionObj);
                    setTransaction = true;
                }
                //Delete from WorkFlow collection
                if (toDeleteCollection)
                    DeleteAllRulesFromCollection(Guid);

                if (setTransaction && commitTransaction)
                    dbConnectionObj.CommitTransaction();
            }
            catch (Exception ee)
            {
                if (setTransaction)
                    dbConnectionObj.RollbackTransaction();
                throw new Exception(ee.Message, ee);
            }
            finally
            {
                if (setTransaction && commitTransaction)
                    CommonDALWrapper.PutConnectionManager(dbConnectionObj);
            }
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetWorkFlowRuleStateDataForUpdate(int ruleSetId, int instanceId, string templatename)
        {
            RDBConnectionManager dbConnectionObj = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMasterVendor_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(DALWrapperAppend.Replace(string.Format(@"SELECT rd.stage_name, wr.rule_set_id, rd.apply_mandatory, rd.apply_uniqueness, rd.apply_primary,rd.apply_validation, rd.apply_alert, rd.apply_basket_validation, rd.apply_basket_alert from IVPRefMaster.dbo.ivp_srm_workflow_rules wr
                INNER JOIN IVPRefMaster.dbo.ivp_srm_workflow_rules_details rd
                ON wr.rule_mapping_id = rd.rule_mapping_id
                WHERE wr.workflow_instance_id = {0} and wr.rule_set_id = {1}
                ORDER BY rd.stage_name
            ", instanceId, ruleSetId)), dbConnectionObj);

                RuleStateDataCollection rsdc = new RuleStateDataCollection();

                var workflowInfo = new RWorkFlowService().GetWorkflowInfo(templatename);
                dynamic workflowName = JObject.Parse(workflowInfo);
                HashSet<string> workflowNameList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var row in workflowName.WorkflowStates)
                {
                    workflowNameList.Add(Convert.ToString(row.StateName));
                }

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("stage_name", typeof(string));
                    dt.Columns.Add("rule_set_id", typeof(int));
                    dt.Columns.Add("apply_mandatory", typeof(bool));
                    dt.Columns.Add("apply_uniqueness", typeof(bool));
                    dt.Columns.Add("apply_primary", typeof(bool));
                    dt.Columns.Add("apply_validation", typeof(bool));
                    dt.Columns.Add("apply_alert", typeof(bool));
                    dt.Columns.Add("apply_basket_validation", typeof(bool));
                    dt.Columns.Add("apply_basket_alert", typeof(bool));

                    foreach (var row in workflowName.WorkflowStates)
                    {
                        string stateName = Convert.ToString(row.StateName);
                        bool toSkipState = false;
                        switch (stateName.ToUpper())
                        {
                            case "END":
                            case "FAILED":
                                toSkipState = true;
                                break;
                        }
                        if (!toSkipState)
                        {
                            dt.Rows.Add(stateName, ruleSetId, false, false, false, false, false, false, false);
                        }
                    }

                    if (ds.Tables.Count == 1)
                        ds.Tables[0].Merge(dt);
                    else
                        ds.Tables.Add(dt);
                }

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (var stateObj in ds.Tables[0].AsEnumerable().GroupBy(x => new { ruleSetID = Convert.ToString(x["rule_set_id"]) }))
                    {
                        rsdc.rulesetid = Convert.ToInt32(stateObj.Key.ruleSetID);
                        rsdc.statesRuleState = new List<RuleStateInfo>();
                        List<RuleStateInfo> rsi = new List<RuleStateInfo>();
                        foreach (var dr in stateObj)
                        {
                            if (workflowNameList.Contains(Convert.ToString(dr["stage_name"])))
                            {
                                string stateName = Convert.ToString(dr["stage_name"]);
                                bool toSkipState = false;

                                switch (stateName.ToUpper())
                                {
                                    case "END":
                                    case "FAILED":
                                        toSkipState = true;
                                        break;
                                }
                                if (!toSkipState)
                                {
                                    RuleStateInfo rs = new RuleStateInfo();
                                    rs.stateName = stateName;
                                    rs.mandatoryData = Convert.ToBoolean(dr["apply_mandatory"]);
                                    rs.uniquenessValidation = Convert.ToBoolean(dr["apply_uniqueness"]);
                                    rs.primaryKeyValidation = Convert.ToBoolean(dr["apply_primary"]);
                                    rs.validations = Convert.ToBoolean(dr["apply_validation"]);
                                    rs.alerts = Convert.ToBoolean(dr["apply_alert"]);
                                    rs.basketValidation = Convert.ToBoolean(dr["apply_basket_validation"]);
                                    rs.basketAlert = Convert.ToBoolean(dr["apply_basket_alert"]);

                                    rsi.Add(rs);
                                }
                            }
                        }
                        rsdc.statesRuleState = rsi;
                    }

                    return new JavaScriptSerializer().Serialize(rsdc);
                }
                else
                    return null;
            }
            catch (Exception ee)
            {
                dbConnectionObj.RollbackTransaction();
                throw ee;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(dbConnectionObj);
            }
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<SRMWorkflowMigrationController.WorkflowEmailState> GetWorkflowEmailActions(int moduleId,List<SRMWorkflowMigrationController.WorkflowDetails> workflowDetails)
        {
            SRMWorkflowMigrationController obj = new SRMWorkflowMigrationController();
            return obj.GetWorkflowEmailActions(moduleId,workflowDetails);
        }
       
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<SRMWorkflowMigrationController.SaveResponse> SaveWorkflowEmailAction(List<SRMWorkflowMigrationController.SelectedActionsInfo> selectedActionsInfo)
        {
            SRMWorkflowMigrationController obj = new SRMWorkflowMigrationController();
            return obj.SaveWorkflowEmailAction(selectedActionsInfo);
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetStateRuleData(int ruleSetId, string templatename)
        {
            var workflowInfo = new RWorkFlowService().GetWorkflowInfo(templatename);
            RuleStateDataCollection sdc = new RuleStateDataCollection();
            sdc.rulesetid = ruleSetId;
            sdc.statesRuleState = new List<RuleStateInfo>();
            dynamic workflowName = JObject.Parse(workflowInfo);

            foreach (var row in workflowName.WorkflowStates)
            {
                RuleStateInfo info = new RuleStateInfo();
                string stateName = Convert.ToString(row.StateName);
                bool toSkipState = false;
                switch (stateName.ToUpper())
                {
                    case "END":
                    case "FAILED":
                        toSkipState = true;
                        break;
                }
                if (!toSkipState)
                {
                    info.stateName = row.StateName;
                    info.mandatoryData = false;
                    info.uniquenessValidation = false;
                    info.primaryKeyValidation = false;
                    info.validations = false;
                    info.alerts = false;
                    info.basketValidation = false;
                    info.basketAlert = false;
                    sdc.statesRuleState.Add(info);
                }

            }
            return new JavaScriptSerializer().Serialize(sdc);
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<string> GetStageNameFromRAD(string templatename)
        {
            var workflowInfo = new RWorkFlowService().GetWorkflowInfo(templatename);
            dynamic workflowName = JObject.Parse(workflowInfo);
            List<string> workflowNameList = new List<string>();

            foreach (var row in workflowName.WorkflowStates)
            {
                string statename = Convert.ToString(row.StateName);
                bool toSkipState = false;
                switch (statename.ToUpper())
                {
                    case "END":
                    case "FAILED":
                        toSkipState = true;
                        break;
                }
                if (!toSkipState)
                    workflowNameList.Add(Convert.ToString(row.StateName));
            }

            return workflowNameList;
        }

    }


    [DataContract]
    public class Workflow
    {
        [DataMember]
        public int InstanceId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ModuleId { get; set; }
        [DataMember]
        public string ModuleName { get; set; }
        [DataMember]
        public int WorkflowActionTypeId { get; set; }
        [DataMember]
        public string WorkflowActionTypeName { get; set; }
        [DataMember]
        public bool WorkflowIsCreate { get; set; }
        [DataMember]
        public bool WorkflowIsUpdate { get; set; }
        [DataMember]
        public bool WorkflowIsTimeSeries { get; set; }
        [DataMember]
        public bool RaiseForNonEmptyValue { get; set; }
        [DataMember]
        public bool WorkflowIsDelete { get; set; }
        [DataMember]
        public string TypeIds { get; set; }
        [DataMember]
        public string PrimaryAttributeIds { get; set; }
        [DataMember]
        public string OtherAttributeIds { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Guid { get; set; }

    }

    [DataContract]
    public class WorkflowModule
    {
        [DataMember]
        public int ModuleId { get; set; }
        [DataMember]
        public string ModuleName { get; set; }
    }

    [DataContract]
    public class WorkflowActionType
    {
        [DataMember]
        public int ActionTypeId { get; set; }
        [DataMember]
        public string ActionTypeName { get; set; }
        [DataMember]
        public int ModuleId { get; set; }
    }

    [DataContract]
    public class Type
    {
        [DataMember]
        public int TypeId { get; set; }
        [DataMember]
        public string TypeName { get; set; }
    }

    [DataContract]
    public class WorkFlowPendingRequest
    {
        [DataMember]
        public int workflow_instance_id { get; set; }

    }

    public class WorkflowAttribute
    {
        public int TypeId { get; set; }
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
    }

    public class WorkflowRulesInfo
    {
        public string WorkflowRuleSetID { get; set; }
        public string RadWorkFlowInstanceID { get; set; }
        public string RadWorkFlowInstanceName { get; set; }
        public string moduleId { get; set; }
        public string WorkFlowRulePriority { get; set; }
        public string SRMWorkFlowRuleStateID { get; set; }
        public bool SRMWorkFlowRuleState { get; set; }
        public string SRMWorkFlowRuleUpdateID { get; set; }
        public string SRMWorkFlowRuleDeleteID { get; set; }
        public string SRMWorkFlowRuleText { get; set; }
        public bool SRMWorkFlowIsDefault { get; set; }
        public string workflowInstanceId { get; set; }
        public string attributes_in_rule { get; set; }

    }
    [DataContract]
    public class WorkflowRulesInfoCollection
    {
        [DataMember]
        public string WorkflowRuleSetID;
        [DataMember]
        public string RadWorkFlowInstanceID;
        [DataMember]
        public string RadWorkFlowInstanceName;
        [DataMember]
        public string moduleId;
        [DataMember]
        public string WorkFlowRuleName;
        [DataMember]
        public string WorkFlowRulePriority;
        [DataMember]
        public string SRMWorkFlowRuleStateID;
        [DataMember]
        public bool SRMWorkFlowRuleState;
        [DataMember]
        public string SRMWorkFlowRuleUpdateID;
        [DataMember]
        public string SRMWorkFlowRuleDeleteID;
        [DataMember]
        public string SRMWorkFlowRuleText;
        [DataMember]
        public bool SRMWorkFlowIsDefault;
    }

    //classes for rule popup to define rule/checks at stage level.

    [DataContract]
    public class RuleStateInfo
    {
        [DataMember]
        public string stateName { get; set; }
        [DataMember]
        public bool mandatoryData { get; set; }
        [DataMember]
        public bool uniquenessValidation { get; set; }
        [DataMember]
        public bool primaryKeyValidation { get; set; }
        [DataMember]
        public bool validations { get; set; }
        [DataMember]
        public bool alerts { get; set; }
        [DataMember]
        public bool basketValidation { get; set; }
        [DataMember]
        public bool basketAlert { get; set; }
    }


    [DataContract]
    public class RuleStateDataCollection
    {
        [DataMember]
        public int rulesetid { get; set; }
        [DataMember]
        public List<RuleStateInfo> statesRuleState { get; set; }
    }

}
