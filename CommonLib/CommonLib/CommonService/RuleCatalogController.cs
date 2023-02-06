using com.ivp.commom.commondal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonService
{
    public class RuleCatalogController
    {
        Dictionary<string, List<string>> attributeVsDepending = null;
        public string GetRules(SRMModule module, SRMTabType tab, string ruleType, List<int> types, string userName)
        {
            string returnObject = string.Empty;
            DataTable dtResult = null;
            List<SRMRuleType> ruleTypes = new List<SRMRuleType>();
            GetRuleTypes(tab, ruleType, ruleTypes);

            switch (module)
            {
                case SRMModule.SECURITY_MASTER:
                    dtResult = GetSecMRules(tab, ruleTypes, types);
                    break;
                case SRMModule.REFERENCE_MASTER:
                case SRMModule.PARTY_MASTER:
                case SRMModule.FUND_MASTER:
                    dtResult = GetRefRules(tab, ruleTypes, types, userName, (int)Enum.Parse(typeof(SRMModule), module.ToString()));
                    break;
                case SRMModule.CORPORATE_ACTIONS:
                    dtResult = GetCorpActionRules(tab, ruleTypes, types);
                    break;
            }
            returnObject = ConvertRulesToObject(tab, dtResult, module);
            return returnObject;
        }

        private string ConvertRulesToObject(SRMTabType tab, DataTable dtResult, SRMModule module)
        {
            dynamic ret = new JObject();
            JArray rules = new JArray();
            switch (tab)
            {
                case SRMTabType.UPSTREAM:
                    var col = dtResult.Columns.Cast<DataColumn>().Where(s => s.ColumnName.Contains("Types"));
                    if (col.Count() > 0 && col.FirstOrDefault().ColumnName != "Security Types")
                        col.FirstOrDefault().ColumnName = "Security Types";

                    foreach (var attrLevel in dtResult.AsEnumerable().GroupBy(x => new { attrName = Convert.ToString(x["Feed Attribute Name"]), typeName = Convert.ToString(x["Feed Name"]), ruleType = Convert.ToString(x["Rule Type"]) }))
                    {
                        dynamic attrLevelObj = new JObject();
                        if (attrLevel.Key.attrName == string.Empty)
                            attrLevelObj.attrName = null;
                        else
                            attrLevelObj.attrName = attrLevel.Key.attrName;

                        attrLevelObj.typeName = attrLevel.Key.typeName;
                        attrLevelObj.ruletypes = new JObject();
                        attrLevelObj.section = tab.ToString();
                        attrLevelObj.basketName = null;

                        HashSet<string> depending = new HashSet<string>();
                        HashSet<string> dependent = new HashSet<string>();
                        HashSet<string> sectypes = new HashSet<string>();

                        JArray arr = new JArray();
                        foreach (var row in attrLevel)
                        {
                            var dep = Convert.ToString(row["Dependent Attributes"]);
                            if (!string.IsNullOrEmpty(dep))
                            {
                                foreach (var depp in dep.Split(','))
                                    dependent.Add(depp);
                            }
                            var depg = Convert.ToString(row["Depending Attributes"]);
                            if (!string.IsNullOrEmpty(depg))
                            {
                                foreach (var deppg in depg.Split(','))
                                    depending.Add(deppg);
                            }

                            var sect = Convert.ToString(row["Security Types"]);
                            if (!string.IsNullOrEmpty(sect))
                            {
                                foreach (var sectt in sect.Split(','))
                                    sectypes.Add(sectt);
                            }

                            dynamic obj = new JObject();
                            obj.ruleGroupName = null;

                            if (attrLevel.Key.ruleType == "API/FTP Filter" || attrLevel.Key.ruleType == "Sectype Rule Filter"
                                || attrLevel.Key.ruleType == "Entity Type Rule Filter" || attrLevel.Key.ruleType == "Entity Feed Transformation")
                                obj.ruleName = Convert.ToString(row["Rule Name"]) + " (" + sect + ")";
                            else
                                obj.ruleName = Convert.ToString(row["Rule Name"]);

                            obj.priority = Convert.ToInt32(row["Priority"]);
                            obj.ruleText = Convert.ToString(row["Rule"]);
                            obj.ruleState = Convert.ToString(row["Rule State"]);
                            obj.isCommonRule = false;
                            obj.commonRuleSectypes = null;
                            arr.Add(obj);
                        }
                        if (attrLevel.Key.ruleType == "Transformation" || attrLevel.Key.ruleType == "Feed Transformation" || attrLevel.Key.ruleType == "Entity Feed Transformation")
                        {
                            if (dependent.Count > 0)
                                attrLevelObj.dependent = string.Join(",", dependent.OrderBy(x => x));
                            else
                                attrLevelObj.dependent = "NONE";
                            if (depending.Count > 0)
                                attrLevelObj.depending = string.Join(",", depending.OrderBy(x => x));
                            else
                                attrLevelObj.depending = "NONE";
                        }
                        else
                        {
                            attrLevelObj.dependent = null;
                            attrLevelObj.depending = null;
                        }
                        if (sectypes.Count > 0)
                            attrLevelObj.sectypes = string.Join(",", sectypes);
                        else
                            attrLevelObj.sectypes = null;

                        attrLevelObj.ruletypes = new JArray { new JObject(new JProperty("ruleTypeName", attrLevel.Key.ruleType), new JProperty("attrRules", arr)) };

                        rules.Add(attrLevelObj);
                    }
                    break;
                case SRMTabType.DOWNSTREAM:
                    var coll = dtResult.Columns.Cast<DataColumn>().Where(s => s.ColumnName.Contains("Types"));
                    if (coll.Count() > 0 && coll.FirstOrDefault().ColumnName != "Security Types")
                        coll.FirstOrDefault().ColumnName = "Security Types";

                    foreach (var attrLevel in dtResult.AsEnumerable().GroupBy(x => new { attrName = Convert.ToString(x["Report Attribute Name"]), typeName = Convert.ToString(x["Report Name"]), ruleType = Convert.ToString(x["Rule Type"]) }))
                    {
                        dynamic attrLevelObj = new JObject();
                        if (attrLevel.Key.attrName == string.Empty)
                            attrLevelObj.attrName = null;
                        else
                            attrLevelObj.attrName = attrLevel.Key.attrName;

                        attrLevelObj.typeName = attrLevel.Key.typeName;
                        attrLevelObj.section = tab.ToString();
                        attrLevelObj.basketName = null;

                        HashSet<string> depending = new HashSet<string>();
                        HashSet<string> dependent = new HashSet<string>();
                        HashSet<string> sectypes = new HashSet<string>();

                        JArray arr = new JArray();
                        foreach (var row in attrLevel)
                        {
                            var dep = Convert.ToString(row["Dependent Attributes"]);
                            if (!string.IsNullOrEmpty(dep))
                            {
                                foreach (var depp in dep.Split(','))
                                    dependent.Add(depp);
                            }
                            var depg = Convert.ToString(row["Depending Attributes"]);
                            if (!string.IsNullOrEmpty(depg))
                            {
                                foreach (var deppg in depg.Split(','))
                                    depending.Add(deppg);
                            }

                            var sect = Convert.ToString(row["Security Types"]);
                            if (!string.IsNullOrEmpty(sect))
                            {
                                foreach (var sectt in sect.Split(','))
                                    sectypes.Add(sectt);
                            }

                            dynamic obj = new JObject();
                            obj.ruleGroupName = null;
                            obj.ruleName = Convert.ToString(row["Rule Name"]);
                            obj.priority = Convert.ToInt32(row["Priority"]);
                            obj.ruleText = Convert.ToString(row["Rule"]);
                            obj.ruleState = Convert.ToString(row["Rule State"]);
                            obj.isCommonRule = false;
                            obj.commonRuleSectypes = null;
                            arr.Add(obj);
                        }
                        if (attrLevel.Key.ruleType == "Transformation")
                        {
                            if (dependent.Count > 0)
                                attrLevelObj.dependent = string.Join(",", dependent.OrderBy(x => x));
                            else
                                attrLevelObj.dependent = "NONE";
                            if (depending.Count > 0)
                                attrLevelObj.depending = string.Join(",", depending.OrderBy(x => x));
                            else
                                attrLevelObj.depending = "NONE";
                        }
                        else
                        {
                            attrLevelObj.dependent = null;
                            attrLevelObj.depending = null;
                        }
                        if (sectypes.Count > 0)
                            attrLevelObj.sectypes = string.Join(",", sectypes);
                        else
                            attrLevelObj.sectypes = null;
                        attrLevelObj.ruletypes = new JArray { new JObject(new JProperty("ruleTypeName", attrLevel.Key.ruleType), new JProperty("attrRules", arr)) };
                        rules.Add(attrLevelObj);
                    }
                    break;
                case SRMTabType.MODELER:
                    var colll = dtResult.Columns.Cast<DataColumn>().Where(s => s.ColumnName.Contains("Type Name"));
                    if (colll.Count() > 0 && colll.FirstOrDefault().ColumnName != "Security Type Name")
                        colll.FirstOrDefault().ColumnName = "Security Type Name";
                    if (!dtResult.Columns.Contains("Rule Group Name"))
                        dtResult.Columns.Add("Rule Group Name", typeof(string));

                    foreach (var attrLevel in dtResult.AsEnumerable().GroupBy(x => new { attrName = Convert.ToString(x["Attribute Name"]), typeName = Convert.ToString(x["Security Type Name"]), BasketName = Convert.ToString(x["Dependent Attributes"]) }).OrderBy(y => y.Key.attrName))
                    {
                        dynamic attrLevelObj = new JObject();
                        attrLevelObj.sectypes = null;
                        attrLevelObj.section = tab.ToString();
                        if (attrLevel.Key.attrName == string.Empty)
                            attrLevelObj.attrName = null;
                        else
                            attrLevelObj.attrName = attrLevel.Key.attrName;

                        if (attrLevel.Key.BasketName == string.Empty)
                            attrLevelObj.basketName = null;
                        else
                            attrLevelObj.basketName = attrLevel.Key.BasketName;

                        HashSet<string> depending = new HashSet<string>();
                        HashSet<string> dependent = new HashSet<string>();

                        attrLevelObj.typeName = attrLevel.Key.typeName;
                        JArray ruleTypes = new JArray();

                        foreach (var grp in attrLevel.GroupBy(w => Convert.ToString(w["Rule Type"])))
                        {
                            dynamic ruleType = new JObject();
                            ruleType.ruleTypeName = grp.Key;

                            JArray arr = new JArray();
                            foreach (var row in grp)
                            {
                                var dep = Convert.ToString(row["Dependent Attributes"]);
                                if (!string.IsNullOrEmpty(dep))
                                {
                                    foreach (var depp in dep.Split(','))
                                        dependent.Add(depp);
                                }
                                var depg = Convert.ToString(row["Depending Attributes"]);
                                if (!string.IsNullOrEmpty(depg))
                                {
                                    foreach (var deppg in depg.Split(','))
                                        depending.Add(deppg);
                                }

                                dynamic obj = new JObject();
                                obj.ruleGroupName = null;

                                string r = string.Empty;
                                var rulegroupName = Convert.ToString(row["Rule Group Name"]);
                                var ruleName = Convert.ToString(row["Rule Name"]);
                                if (!string.IsNullOrEmpty(rulegroupName))
                                    r = rulegroupName + " : ";
                                if (!string.IsNullOrEmpty(ruleName))
                                    r += ruleName;

                                obj.ruleName = r;
                                obj.priority = Convert.ToInt32(row["Priority"]);
                                obj.ruleText = Convert.ToString(row["Rule"]);
                                obj.ruleState = Convert.ToString(row["Rule State"]);

                                if (module == SRMModule.SECURITY_MASTER)
                                {
                                    var sectypes = Convert.ToString(row["Security Type Name"]);
                                    if (sectypes.StartsWith("Common Rule"))
                                    {
                                        obj.isCommonRule = true;
                                        obj.commonRuleSectypes = sectypes.Split('-')[1];
                                    }
                                    else
                                    {
                                        obj.isCommonRule = false;
                                        obj.commonRuleSectypes = null;
                                    }
                                }
                                else
                                {
                                    obj.isCommonRule = false;
                                    obj.commonRuleSectypes = null;
                                }
                                arr.Add(obj);
                            }
                            ruleType.attrRules = arr;
                            ruleTypes.Add(ruleType);
                        }
                        attrLevelObj.ruletypes = ruleTypes;


                        if (dependent.Count > 0)
                            attrLevelObj.dependent = string.Join(",", dependent.OrderBy(x => x));
                        else
                            attrLevelObj.dependent = "NONE";
                        if (depending.Count > 0)
                            attrLevelObj.depending = string.Join(",", depending.OrderBy(x => x));
                        else
                            attrLevelObj.depending = "NONE";

                        rules.Add(attrLevelObj);
                    }
                    break;
            }
            ret.rules = rules;
            return JsonConvert.SerializeObject(ret);
        }

        private static void GetRuleTypes(SRMTabType tab, string ruleType, List<SRMRuleType> ruleTypes)
        {
            switch (ruleType)
            {
                case "Transformation":
                    if (tab == SRMTabType.UPSTREAM || tab == SRMTabType.DOWNSTREAM)
                        ruleTypes.Add(SRMRuleType.Transformation);
                    break;
                case "Validation":
                    if (tab == SRMTabType.UPSTREAM)
                        ruleTypes.Add(SRMRuleType.Validation);
                    break;
                case "API/FTP Filter":
                    if (tab == SRMTabType.UPSTREAM)
                        ruleTypes.Add(SRMRuleType.APIFTPFilter);
                    break;
                case "Rule Filter":
                    if (tab == SRMTabType.UPSTREAM)
                        ruleTypes.Add(SRMRuleType.RuleFilter);
                    break;
                case "Filter":
                    if (tab == SRMTabType.DOWNSTREAM)
                        ruleTypes.Add(SRMRuleType.Validation);
                    break;
                case "Attribute Level":
                    if (tab == SRMTabType.MODELER)
                    {
                        ruleTypes.Add(SRMRuleType.Mnemonic);
                        ruleTypes.Add(SRMRuleType.Transformation);
                        ruleTypes.Add(SRMRuleType.Validation);
                        ruleTypes.Add(SRMRuleType.Alerts);
                    }
                    break;
                case "Basket Level":
                    if (tab == SRMTabType.MODELER)
                        ruleTypes.Add(SRMRuleType.BasketValidation);
                    break;
                case "Conditional Filter":
                    if (tab == SRMTabType.MODELER)
                        ruleTypes.Add(SRMRuleType.ConditionalFilter);
                    break;
            }
        }

        DataTable GetRefRules(SRMTabType tab, List<SRMRuleType> ruleTypes, List<int> types, string user_name, int module_id)
        {
            List<int> refRuleTypes = new List<int>();
            string typesString = "NULL";
            if (types != null && types.Count > 0)
                typesString = "'" + string.Join(",", types) + "'";

            if (ruleTypes != null && ruleTypes.Count > 0)
            {
                foreach (var ruleType in ruleTypes)
                {
                    switch (tab)
                    {
                        case SRMTabType.UPSTREAM:
                            switch (ruleType)
                            {
                                case SRMRuleType.Transformation:
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.UPSTREAM_TRANSFORMATION.ToString()));
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.UPSTREAM_ENTITY_TRANSFORMATION.ToString()));
                                    break;
                                case SRMRuleType.Validation:
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.UPSTREAM_VALIDATION.ToString()));
                                    break;
                                case SRMRuleType.RuleFilter:
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.UPSTREAM_FILTER.ToString()));
                                    break;
                                case SRMRuleType.APIFTPFilter:
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.UPSTREAM_API_FTP_FILTER.ToString()));
                                    break;
                            }
                            break;
                        case SRMTabType.DOWNSTREAM:
                            switch (ruleType)
                            {
                                case SRMRuleType.Transformation:
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.DOWNSTREAM_TRANSFORMATION.ToString()));
                                    break;
                                case SRMRuleType.Validation:
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.DOWNSTREAM_FILTER.ToString()));
                                    break;
                            }
                            break;
                        case SRMTabType.MODELER:
                            switch (ruleType)
                            {
                                case SRMRuleType.Transformation:
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.MODELER_TRANSFORMATION.ToString()));
                                    break;
                                case SRMRuleType.Validation:
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.MODELER_VALIDATION.ToString()));
                                    break;
                                case SRMRuleType.Alerts:
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.MODELER_ALERT.ToString()));
                                    break;
                                case SRMRuleType.BasketValidation:
                                    refRuleTypes.Add((int)Enum.Parse(typeof(RMRuleTypes), RMRuleTypes.MODELER_GROUP_VALIDATION.ToString()));
                                    break;
                            }
                            break;
                    }
                }
            }

            string ruleTypeString = "NULL";
            if (refRuleTypes != null && refRuleTypes.Count > 0)
                ruleTypeString = string.Join(",", refRuleTypes);
            var dt = CommonDALWrapper.ExecuteSelectQuery(string.Format("EXEC IVPRefMaster.dbo.REFM_GetRulesForCatalogue @module_type = '{0}', @rule_type_ids = '{1}', @types = {2}, @user_name = '{3}',@module_id={4}", tab.ToString(), ruleTypeString, typesString, user_name, module_id), ConnectionConstants.RefMaster_Connection).Tables[0];

            if (tab == SRMTabType.UPSTREAM)
            {
                if (dt.Rows.Count > 0)
                {
                    var merged = dt.Clone();
                    var feedTransformation = dt.AsEnumerable().GroupBy(x => Convert.ToString(x["Rule Type"])).ToDictionary(c => c.Key, d => d.CopyToDataTable());
                    foreach (var ruleTypeLevel in feedTransformation)
                    {
                        var dtt = ruleTypeLevel.Value;
                        if (dtt.Rows.Count > 0)
                        {
                            FillDependingAttributes(tab, ref dtt);
                            merged.Merge(dtt);
                        }
                    }
                    dt = merged;
                }
            }
            else
            {
                if (dt.Rows.Count > 0)
                    FillDependingAttributes(tab, ref dt);
            }
            return dt;
        }

        DataTable GetSecMRules(SRMTabType tab, List<SRMRuleType> ruleTypes, List<int> types)
        {
            StringBuilder ruleTypesString = new StringBuilder("NULL");
            string typesString = "NULL";
            string task_in_module = "";
            int temp = 0;
            if (ruleTypes != null && ruleTypes.Count > 0)
            {
                ruleTypesString = ruleTypesString.Clear().Append("'");
                foreach (var ruleType in ruleTypes)
                {
                    switch (tab)
                    {
                        case SRMTabType.UPSTREAM:
                            switch (ruleType)
                            {
                                case SRMRuleType.Transformation:
                                    temp = 4;
                                    break;
                                case SRMRuleType.Validation:
                                    temp = 1;
                                    break;
                                case SRMRuleType.RuleFilter:
                                    temp = 2;
                                    task_in_module += "u" + ",";
                                    break;
                                case SRMRuleType.APIFTPFilter:
                                    temp = 2;
                                    task_in_module += "n" + ",";
                                    break;
                            }
                            break;
                        case SRMTabType.DOWNSTREAM:
                            switch (ruleType)
                            {
                                case SRMRuleType.Transformation:
                                    temp = 4;
                                    break;
                                case SRMRuleType.Validation:
                                    temp = 2;
                                    break;
                            }
                            break;
                        case SRMTabType.MODELER:
                            switch (ruleType)
                            {
                                case SRMRuleType.Mnemonic:
                                    temp = 1;
                                    break;
                                case SRMRuleType.Transformation:
                                    temp = 2;
                                    break;
                                case SRMRuleType.Validation:
                                    temp = 3;
                                    break;
                                case SRMRuleType.Alerts:
                                    temp = 4;
                                    break;
                                case SRMRuleType.BasketValidation:
                                    temp = 5;
                                    break;
                                case SRMRuleType.ConditionalFilter:
                                    temp = 6;
                                    break;
                            }
                            break;
                    }
                    if (temp == 0)
                        continue;
                    //throw new Exception("Invalid Rule Type");
                    ruleTypesString.Append(temp).Append(",");
                }
                ruleTypesString.Remove(ruleTypesString.Length - 1, 1);
                ruleTypesString.Append("'");
            }

            if (types != null && types.Count > 0)
                typesString = "'" + string.Join(",", types) + "'";
            if (!string.IsNullOrEmpty(task_in_module))
                task_in_module = "'" + task_in_module.Substring(0, task_in_module.Length - 1) + "'";
            else
                task_in_module = "NULL";

            var dt = CommonDALWrapper.ExecuteSelectQuery(string.Format("EXEC IVPSecMaster.dbo.SECM_GetRulesForCatalogue @module_type = '{0}', @rule_type_ids = {1}, @types = {2}, @task_in_modules = {3}",
                            tab.ToString(), ruleTypesString.ToString(), typesString, task_in_module), ConnectionConstants.SecMaster_Connection).Tables[0];

            if (dt.Rows.Count > 0)
                FillDependingAttributes(tab, ref dt);
            return dt;
        }

        DataTable GetCorpActionRules(SRMTabType tab, List<SRMRuleType> ruleTypes, List<int> types)
        {
            StringBuilder ruleTypesString = new StringBuilder("NULL");
            string typesString = "NULL";
            string task_in_module = "NULL";
            int temp = 0;
            if (ruleTypes != null && ruleTypes.Count > 0)
            {
                ruleTypesString = ruleTypesString.Clear().Append("'");
                foreach (var ruleType in ruleTypes)
                {
                    switch (tab)
                    {
                        case SRMTabType.UPSTREAM:
                            switch (ruleType)
                            {
                                case SRMRuleType.Transformation:
                                    temp = 4;
                                    break;
                                case SRMRuleType.Validation:
                                    temp = 1;
                                    break;
                                case SRMRuleType.RuleFilter:
                                    temp = 2;
                                    task_in_module += "u" + ",";
                                    break;
                                case SRMRuleType.APIFTPFilter:
                                    temp = 2;
                                    task_in_module += "n" + ",";
                                    break;
                            }
                            break;
                    }
                    if (temp == 0)
                        throw new Exception("Invalid Rule Type");
                    ruleTypesString.Append(temp).Append(",");
                }
                ruleTypesString.Remove(ruleTypesString.Length - 1, 1);
                ruleTypesString.Append("'");
            }
            if (types != null && types.Count > 0)
                typesString = "'" + string.Join(",", types) + "'";
            if (task_in_module != "NULL")
                task_in_module = task_in_module.Substring(0, task_in_module.Length - 1);

            var dt = CommonDALWrapper.ExecuteSelectQuery(string.Format("EXEC IVPCorpAction.dbo.CA_GetRulesForCatalogue @module_type = '{0}', @rule_type_ids = {1}, @types = {2}, @task_in_modules = {3}", tab.ToString(), ruleTypesString.ToString(), typesString, task_in_module), ConnectionConstants.SecMaster_Connection).Tables[0];
            if (dt.Rows.Count > 0)
                FillDependingAttributes(tab, ref dt);
            return dt;
        }

        void FillDependingAttributes(SRMTabType tab, ref DataTable dt)
        {
            Dictionary<string, HashSet<string>> attributeVsFinalDepending = new Dictionary<string, HashSet<string>>();
            var groupBy = dt.AsEnumerable().Where(e => e["Rule State"] != DBNull.Value && Convert.ToBoolean(e["Rule State"])).GroupBy(x => Convert.ToString(x["Dependent Attributes"]));
            attributeVsDepending = groupBy.ToDictionary(a => a.Key, b => b.Where(c => !string.IsNullOrEmpty(Convert.ToString(c["Depending Attributes"]))).SelectMany(d => Convert.ToString(d["Depending Attributes"]).Split(',')).ToList());
            var attributeVsRows = groupBy.ToDictionary(a => a.Key, b => b.ToList());
            var attributeVsDependent = new Dictionary<string, HashSet<string>>();

            foreach (var attrLevel in attributeVsDepending)
            {
                List<string> t = new List<string>();
                foreach (var attr in attrLevel.Value)
                {
                    var attributesToIgnore = new HashSet<string> { attrLevel.Key };
                    t.AddRange(CheckDependent(ref attributesToIgnore, attr));
                }
                attributeVsFinalDepending.Add(attrLevel.Key, new HashSet<string>(t));
            }

            foreach (var row in dt.AsEnumerable())
            {
                row["Depending Attributes"] = DBNull.Value;
            }

            foreach (var attrLevel in attributeVsFinalDepending)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var dependingAttr in attrLevel.Value)
                {
                    if (!attributeVsDependent.ContainsKey(dependingAttr))
                        attributeVsDependent[dependingAttr] = new HashSet<string>();
                    attributeVsDependent[dependingAttr].Add(attrLevel.Key);

                    sb.Append(dependingAttr).Append(",");
                }

                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);

                if (attributeVsRows.ContainsKey(attrLevel.Key))
                {
                    foreach (var row in attributeVsRows[attrLevel.Key])
                        row["Depending Attributes"] = sb.ToString();
                }
            }

            string columnName = string.Empty;
            switch (tab)
            {
                case SRMTabType.UPSTREAM:
                    columnName = "Feed Attribute Name";
                    break;
                case SRMTabType.DOWNSTREAM:
                    columnName = "Report Attribute Name";
                    break;
                case SRMTabType.MODELER:
                    columnName = "Attribute Name";
                    break;
            }

            bool isBasket = dt.AsEnumerable().Any(x => Convert.ToString(x["Rule Type"]).StartsWith("Basket"));
            foreach (var row in dt.AsEnumerable())
            {
                var attr = Convert.ToString(row[columnName]);
                if (attributeVsDependent.ContainsKey(attr))
                    row["Dependent Attributes"] = string.Join(",", attributeVsDependent[attr]);
                else if (!isBasket)
                    row["Dependent Attributes"] = DBNull.Value;
            }
        }

        List<string> CheckDependent(ref HashSet<string> attributesToIgnore, string attribute)
        {
            List<string> ret = new List<string>();
            if (!attributesToIgnore.Contains(attribute))
            {
                if (attributeVsDepending.ContainsKey(attribute))
                {
                    foreach (var attr in attributeVsDepending[attribute])
                    {
                        if (!attributesToIgnore.Contains(attr))
                        {
                            attributesToIgnore.Add(attribute);
                            ret.AddRange(CheckDependent(ref attributesToIgnore, attr));
                        }
                    }
                }
                ret.Add(attribute);
            }
            return ret;
        }

        public string GetRuleCatalogueLeftPane(SRMModule module, SRMTabType tab, string userName, List<int> typeIds)
        {
            string returnJSON = string.Empty;
            switch (module)
            {
                case SRMModule.SECURITY_MASTER:
                case SRMModule.CORPORATE_ACTIONS:
                    returnJSON = GetSecMAndCorpRuleCatalogueLeftPane(module, tab, userName, typeIds);
                    break;
                case SRMModule.REFERENCE_MASTER:
                case SRMModule.PARTY_MASTER:
                case SRMModule.FUND_MASTER:
                    returnJSON = GetRefMAndPartyFundRuleCatalogueLeftPane(module, tab, userName, typeIds);
                    break;
            }
            return returnJSON;
        }

        private string GetSecMAndCorpRuleCatalogueLeftPane(SRMModule module, SRMTabType tab, string userName, List<int> typeIds)
        {
            dynamic ret = new JObject();
            ret.Section = tab.ToString();
            ret.SectionId = (int)tab;

            switch (tab)
            {
                case SRMTabType.UPSTREAM:
                    if (module == SRMModule.SECURITY_MASTER)
                    {
                        ret.List = new JArray(CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT DISTINCT fs.feed_id, vendor_name + ' - ' + feed_name AS feed_name, vt.vendor_type
                            FROM IVPSecMasterVendor.dbo.ivp_secmv_feed_summary fs
                            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_vendor_master vm ON fs.vendor_id = vm.vendor_id
                            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_vendor_type vt ON vm.vendor_type_id = vt.vendor_type_id
							INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_sectype_feed_mapping_master sfm ON fs.feed_id = sfm.feed_id
							INNER JOIN IVPSecMaster.dbo.SECM_GetList2Table('{0}',',') tab ON sfm.sectype_id = tab.item
                            WHERE fs.is_active = 1 AND vm.is_active = 1 AND sfm.is_active = 1", string.Join(",", typeIds)), ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().OrderBy(q => Convert.ToString(q["feed_name"])).Select(x => new JObject(new JProperty("TypeId", Convert.ToInt32(x["feed_id"])), new JProperty("Type", Convert.ToString(x["feed_name"])), new JProperty("ParentType", Convert.ToString(x["vendor_type"])), new JProperty("RuleTypes", new JArray("Transformation", "Validation", "API/FTP Filter", "Rule Filter")))));
                    }
                    else if (module == SRMModule.CORPORATE_ACTIONS)
                    {
                        ret.List = new JArray(CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT DISTINCT fs.feed_id, vendor_name + '-' + feed_name AS feed_name, vt.vendor_type
                            FROM IVPCorpActionVendor.dbo.ivp_cav_feed_summary fs
                            INNER JOIN IVPCorpActionVendor.dbo.ivp_cav_vendor_master vm ON fs.vendor_id = vm.vendor_id
                            INNER JOIN IVPCorpActionVendor.dbo.ivp_cav_vendor_type vt ON vm.vendor_type_id = vt.vendor_type_id
							INNER JOIN IVPCorpActionVendor.dbo.ivp_cav_corpaction_type_feed_mapping_master sfm ON fs.feed_id = sfm.feed_id
							INNER JOIN IVPSecMaster.dbo.SECM_GetList2Table('{0}',',') tab ON sfm.corpaction_type_id = tab.item
                            WHERE fs.is_active = 1 AND vm.is_active = 1 AND sfm.is_active = 1", string.Join(",", typeIds)), ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().OrderBy(q => Convert.ToString(q["feed_name"])).Select(x => new JObject(new JProperty("TypeId", Convert.ToInt32(x["feed_id"])), new JProperty("Type", Convert.ToString(x["feed_name"])), new JProperty("ParentType", Convert.ToString(x["vendor_type"])), new JProperty("RuleTypes", new JArray("Transformation", "Validation", "API/FTP Filter", "Rule Filter")))));
                    }
                    break;
                case SRMTabType.DOWNSTREAM:
                    if (module == SRMModule.SECURITY_MASTER)
                    {
                        ret.List = new JArray(CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT DISTINCT report_setup_id, repository_name + ' - ' + report_name AS report_name, report_type
                            FROM IVPSecMaster.dbo.ivp_secm_report_setup rs
						    CROSS APPLY IVPSecMaster.dbo.SECM_GetList2Table(rs.dependent_id,'|') tab
                            INNER JOIN IVPSecMaster.dbo.SECM_GetList2Table('{0}',',') tab2 ON tab.item = tab2.item
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_repository rr ON rs.repository_id = rr.repository_id
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_reports r ON rs.report_id = r.report_id
                            WHERE rs.is_active = 1 AND rr.is_active = 1 ", string.Join(",", typeIds)), ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().OrderBy(q => Convert.ToString(q["report_name"])).Select(x => new JObject(new JProperty("TypeId", Convert.ToInt32(x["report_setup_id"])), new JProperty("Type", Convert.ToString(x["report_name"])), new JProperty("ParentType", Convert.ToString(x["report_type"])), new JProperty("RuleTypes", new JArray("Transformation", "Filter")))));
                    }
                    break;
                case SRMTabType.MODELER:
                    if (module == SRMModule.SECURITY_MASTER)
                    {
                        ret.List = new JArray(CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT DISTINCT sectype_Id, sectype_name
                            FROM IVPSecMaster.dbo.SECM_GetUserSectypes('{0}') sm
						    INNER JOIN IVPSecMaster.dbo.SECM_GetList2Table('{1}',',') tab ON tab.item = sm.sectype_id", userName, string.Join(",", typeIds)), ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().OrderBy(q => Convert.ToString(q["sectype_name"])).Select(x => new JObject(new JProperty("TypeId", Convert.ToInt32(x["sectype_Id"])), new JProperty("Type", Convert.ToString(x["sectype_name"])), new JProperty("ParentType", null), new JProperty("RuleTypes", new JArray("Attribute Level", "Basket Level", "Conditional Filter")))));
                    }
                    break;
            }

            return JsonConvert.SerializeObject(ret);
        }

        private string GetRefMAndPartyFundRuleCatalogueLeftPane(SRMModule module, SRMTabType tab, string userName, List<int> typeIds)
        {
            dynamic ret = new JObject();
            ret.Section = tab.ToString();
            ret.SectionId = (int)tab;

            switch (tab)
            {
                case SRMTabType.UPSTREAM:
                    ret.List = new JArray(CommonDALWrapper.ExecuteSelectQuery(@"SELECT DISTINCT fsum.feed_summary_id,ds.data_source_name + ' - ' + fsum.feed_name AS feed_name 
                                FROM IVPRefMasterVendor.dbo.ivp_refm_data_source ds
                                INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_feed_summary fsum
                                ON(ds.data_source_id = fsum.data_source_id AND ds.is_active = 1 AND fsum.is_active = 1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type_feed_mapping fmap
                                ON (fmap.feed_summary_id = fsum.feed_summary_id)
                                INNER JOIN IVPRefMaster.dbo.REFM_GetUserEntityTypes('" + userName + @"'," + (int)Enum.Parse(typeof(SRMModule), module.ToString()) + @") uet
                                ON (uet.entity_type_id = fmap.entity_type_id) 
                                INNER JOIN IVPRefMaster.dbo.REFM_GetList2Table('" + string.Join(",", typeIds) + @"',',') tab
                                ON (tab.item = uet.entity_type_id) ", ConnectionConstants.RefMasterVendor_Connection).Tables[0].AsEnumerable().OrderBy(q => Convert.ToString(q["feed_name"])).Select(x => new JObject(new JProperty("TypeId", Convert.ToInt32(x["feed_summary_id"])), new JProperty("Type", Convert.ToString(x["feed_name"])), new JProperty("ParentType", null), new JProperty("RuleTypes", new JArray("Transformation", "Validation", "API/FTP Filter", "Rule Filter")))));
                    break;
                case SRMTabType.DOWNSTREAM:
                    ret.List = new JArray(CommonDALWrapper.ExecuteSelectQuery(@"SELECT DISTINCT rep.report_id,repo.report_repository_name + ' - ' + rep.report_name AS report_name,typ.report_type AS report_type
                                FROM IVPRefMaster.dbo.ivp_refm_report_repository_master repo
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report rep            	                
                                ON (repo.report_repository_id = rep.report_repository_id AND rep.is_active = 1 AND repo.is_active = 1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_type typ
                                ON (typ.report_type_id = rep.report_type_id)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_report_entity_map rmap
                                ON (rmap.report_id = rep.report_id)
                                INNER JOIN IVPRefMaster.dbo.REFM_GetUserEntityTypes('" + userName + @"'," + (int)Enum.Parse(typeof(SRMModule), module.ToString()) + @") utyp
                                ON (utyp.entity_type_id = rmap.dependent_id) 
                                INNER JOIN IVPRefMaster.dbo.REFM_GetList2Table('" + string.Join(",", typeIds) + @"',',') tab
                                ON (tab.item = utyp.entity_type_id) ", ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().OrderBy(q => Convert.ToString(q["report_name"])).Select(x => new JObject(new JProperty("TypeId", Convert.ToInt32(x["report_id"])), new JProperty("Type", Convert.ToString(x["report_name"])), new JProperty("ParentType", Convert.ToString(x["report_type"])), new JProperty("RuleTypes", new JArray("Transformation", "Filter")))));

                    break;
                case SRMTabType.MODELER:
                    ret.List = new JArray(CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT entity_type_id, entity_display_name FROM IVPRefMaster.dbo.REFM_GetUserEntityTypes('{0}',{1}) uet
                                INNER JOIN IVPRefMaster.dbo.REFM_GetList2Table('" + string.Join(",", typeIds) + @"',',') tab
                                ON (tab.item = uet.entity_type_id) ", userName, (int)Enum.Parse(typeof(SRMModule), module.ToString())), ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().OrderBy(q => Convert.ToString(q["entity_display_name"])).Select(x => new JObject(new JProperty("TypeId", Convert.ToInt32(x["entity_type_id"])), new JProperty("Type", Convert.ToString(x["entity_display_name"])), new JProperty("ParentType", null), new JProperty("RuleTypes", new JArray("Attribute Level", "Basket Level")))));
                    break;
            }

            return JsonConvert.SerializeObject(ret);
        }
    }

    public enum SRMTabType
    {
        UPSTREAM = 0,
        DOWNSTREAM,
        MODELER
    }

    public enum SRMRuleType
    {
        Mnemonic = 0,
        Transformation,
        Validation,
        Alerts,
        BasketValidation,
        RuleFilter,
        APIFTPFilter,
        ConditionalFilter
    }

    public enum SRMModule
    {
        SECURITY_MASTER = 3,
        REFERENCE_MASTER = 6,
        CORPORATE_ACTIONS = 9,
        PARTY_MASTER = 20,
        FUND_MASTER = 18
    }

    public enum RMRuleTypes
    {
        UPSTREAM_VALIDATION = 1,
        UPSTREAM_FILTER = 2,
        UPSTREAM_API_FTP_FILTER = 3,
        UPSTREAM_TRANSFORMATION = 4,
        UPSTREAM_ENTITY_TRANSFORMATION = 5,
        DOWNSTREAM_TRANSFORMATION = 6,
        DOWNSTREAM_FILTER = 7,
        MODELER_VALIDATION = 8,
        MODELER_TRANSFORMATION = 9,
        MODELER_GROUP_VALIDATION = 10,
        MODELER_ALERT = 11,
    }
}
