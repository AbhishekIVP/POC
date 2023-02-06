using com.ivp.rad.controls.xruleeditor.grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.srmcommon
{
    public static class SRMWorkflowPrepareRuleGrammar
    {
        public static RADXRuleGrammarInfo prepareGrammarInfo(string typeIDs, int moduleID)
        {
            string[] processedString = typeIDs.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            string typeID = string.Join(",", processedString);

            Assembly CommonServiceAssembly = Assembly.Load("CommonService");
            Type CommonServiceType = CommonServiceAssembly.GetType("CommonService.CommonService");
            MethodInfo SyncMigrationConfiguration = CommonServiceType.GetMethod("PrepareRuleGrammarInfo");
            object CommonServiceObj = Activator.CreateInstance(CommonServiceType);
            RADXRuleGrammarInfo info = (RADXRuleGrammarInfo)SyncMigrationConfiguration.Invoke(CommonServiceObj, new object[] { typeID, moduleID });

            return info;
        }
    }
}
