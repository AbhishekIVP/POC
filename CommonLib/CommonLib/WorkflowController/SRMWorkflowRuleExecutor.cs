using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.xruleengine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{

    public enum SRMRuleType
    {
        /// <summary>
        /// Validation
        /// </summary>
        VALIDATION = 1,
        /// <summary>
        /// Filter
        /// </summary>
        FILTER = 2,
        /// <summary>
        /// Transformation
        /// </summary>
        TRANSFORMATION = 3
    }

    public class SRMRuleInfo
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the rule set ID.
        /// </summary>
        /// <value>The rule set ID.</value>
        public int RuleSetID { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of the rule.
        /// </summary>
        /// <value>The type of the rule.</value>
        public SRMRuleType RuleType { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the dict rule names.
        /// </summary>
        /// <value>The dict rule names.</value>
        public Dictionary<string, string> dictRuleNames { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the input information.
        /// </summary>
        /// <value>The input information.</value>
        public DataTable InputInformation { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the output information.
        /// </summary>
        /// <value>The output information.</value>
        public DataTable OutputInformation { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the rule set ID.
        /// </summary>
        /// <value>The rule set ID.</value>
        public List<int> RuleSetIDs { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets whether failed rule name is required or not.
        /// </summary>
        /// <value>The rule set ID.</value>
        public bool RequireRuleName { get; set; }
    }

    public class SRMWorkflowRuleExecutor
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMWorkflowController");
        RXRuleExecutionInfo ruleExecutionInfo = null;

        public void ExecuteXRule(SRMRuleInfo baseInfo, string ruleType, int typeId, int moduleId)
        {
            DataSet dsInput = null;
            mLogger.Debug("SRMWorkflowRuleExecutor : ExecuteRule " + baseInfo.RuleType + " -> Start");
            try
            {
                dsInput = new DataSet();
                ruleExecutionInfo = new RXRuleExecutionInfo();

                if (baseInfo.InputInformation.DataSet == null)
                    dsInput.Tables.Add(baseInfo.InputInformation);
                else
                    dsInput = baseInfo.InputInformation.DataSet;

                ruleExecutionInfo.InputDataset = dsInput;
                ruleExecutionInfo.ruleSetID = baseInfo.RuleSetID;
                ruleExecutionInfo.ruleInfoColumnName = "rule_name";

                if (!string.IsNullOrEmpty(ruleType))
                {
                    Dictionary<string, string> modMapping = new Dictionary<string, string>();
                    DataSet customOperations = null;
                    customOperations = RMCommonController.PrepareCustomOperationsInfo(ruleType);
                    foreach (DataRow row in customOperations.Tables[0].Rows)
                    {
                        string assemblyName = AppDomain.CurrentDomain.BaseDirectory + row.Field<string>("assembly_name") + ".dll";
                        if (!File.Exists(assemblyName))
                            assemblyName = AppDomain.CurrentDomain.BaseDirectory + "bin\\" + row.Field<string>("assembly_name") + ".dll";
                        modMapping.Add(row.Field<string>("name"), assemblyName);
                    }

                    ruleExecutionInfo.hiddenValue = typeId + "||" + moduleId;
                    ruleExecutionInfo.modelMapping = modMapping;
                }

                if (baseInfo.RuleSetID != 0)
                {
                    RXRuleExecutor objRXRuleExecutor = new RXRuleExecutor();
                    objRXRuleExecutor.DBConnectionId = RADConfigReader.GetConfigAppSettings(ConnectionConstants.RefMasterVendor_Connection);
                    objRXRuleExecutor.ExecuteRule(ruleExecutionInfo);
                    baseInfo.OutputInformation = ruleExecutionInfo.InputDataset.Tables[0];
                    for (int count = 1; count < ruleExecutionInfo.InputDataset.Tables.Count; count++)
                    {
                        ruleExecutionInfo.InputDataset.Tables.RemoveAt(count);
                    }
                }

                //dsInput.Tables.RemoveAt(0);
                mLogger.Debug("SRMWorkflowRuleExecutor : ExecuteRule " + baseInfo.RuleType + " -> End");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new Exception(ex.Message, ex);
            }
            finally { }
        }
    }
}
