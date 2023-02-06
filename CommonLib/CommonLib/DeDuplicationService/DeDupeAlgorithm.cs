using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;
using System.Runtime.Serialization;
using com.ivp.refmaster.common;
using com.ivp.rad.utils;
using com.ivp.commom.commondal;
using com.ivp.refmaster.deduplication;

namespace com.ivp.common
{
    public enum MatchTypes
    {
        ExactMatch = 1,
        ApproximateMatch
    }

    public enum ToleranceType
    {
        None = 0,
        Absolute = 1,
        Percentage = 2,
        Days = 3,
        Weeks = 4,
        Months = 5,
        Years = 6,
        Hours = 7,
        Minutes = 8,
        Seconds = 9
    }

    /// <summary>
    /// Jaro Winkler Algorithm
    /// </summary>
    internal class JaroWinklerDistanceAlgorithm
    {
        private static readonly double mWeightThreshold = 0.7;
        private static readonly int mNumChars = 4;

        public static double GetMatchProbability(string string1, string string2)
        {
            int lLen1 = string1.Length;
            int lLen2 = string2.Length;
            if (lLen1 == 0)
                return lLen2 == 0 ? 1.0 : 0.0;

            int lSearchRange = Math.Max(0, Math.Max(lLen1, lLen2) / 2 - 1);

            // default initialized to false
            bool[] lMatched1 = new bool[lLen1];
            bool[] lMatched2 = new bool[lLen2];

            int lNumCommon = 0;
            for (int i = 0; i < lLen1; ++i)
            {
                int lStart = Math.Max(0, i - lSearchRange);
                int lEnd = Math.Min(i + lSearchRange + 1, lLen2);
                for (int j = lStart; j < lEnd; ++j)
                {
                    if (lMatched2[j]) continue;
                    if (string1[i] != string2[j])
                        continue;
                    lMatched1[i] = true;
                    lMatched2[j] = true;
                    ++lNumCommon;
                    break;
                }
            }
            if (lNumCommon == 0) return 0.0;

            int lNumHalfTransposed = 0;
            int k = 0;
            for (int i = 0; i < lLen1; ++i)
            {
                if (!lMatched1[i]) continue;
                while (!lMatched2[k]) ++k;
                if (string1[i] != string2[k])
                    ++lNumHalfTransposed;
                ++k;
            }
            int lNumTransposed = lNumHalfTransposed / 2;

            double lNumCommonD = lNumCommon;
            double lWeight = (lNumCommonD / lLen1
                             + lNumCommonD / lLen2
                             + (lNumCommon - lNumTransposed) / lNumCommonD) / 3.0;

            if (lWeight <= mWeightThreshold) return lWeight;
            int lMax = Math.Min(mNumChars, Math.Min(string1.Length, string2.Length));
            int lPos = 0;
            while (lPos < lMax && string1[lPos] == string2[lPos])
                ++lPos;
            if (lPos == 0) return lWeight;
            return lWeight + 0.1 * lPos * (1.0 - lWeight);

        }
    }

    public class DeDupAlgo
    {
        public static double GetMatchProbability(string string1, string string2)
        {
            StringBuilder strbuild = new StringBuilder();
            bool compareWhole = false;

            int minLength = 0;
            int maxLength = 0;
            double netProbability = 0;

            List<string> lststr1 = GetTokens(string1);
            List<string> lststr2 = GetTokens(string2);

            bool isAbbreviation1 = false;
            bool isAbbreviation2 = false;

            if (lststr1 != null && lststr2 == null)
                isAbbreviation1 = isAbbreviation(lststr1);

            else if (lststr1 == null && lststr2 != null)
                isAbbreviation2 = isAbbreviation(lststr2);

            else if (lststr1 == null && lststr2 == null)
                compareWhole = true;

            else
            {
                isAbbreviation1 = isAbbreviation(lststr1);
                isAbbreviation2 = isAbbreviation(lststr2);
            }

            if (isAbbreviation1 && isAbbreviation2)
            {
                lststr1.ForEach(x => strbuild.Append(x));
                string1 = strbuild.ToString();
                strbuild.Clear();
                lststr2.ForEach(x => strbuild.Append(x));
                string2 = strbuild.ToString();
                compareWhole = true;
            }

            else if (isAbbreviation1 && !isAbbreviation2)
            {
                lststr1.ForEach(x => strbuild.Append(x));
                string1 = strbuild.ToString();
                strbuild.Clear();

                if (lststr2 != null)
                {
                    lststr2.ForEach(x => strbuild.Append(x[0]));
                    string2 = strbuild.ToString();
                }
                compareWhole = true;
            }

            else if (!isAbbreviation1 && isAbbreviation2)
            {
                lststr2.ForEach(x => strbuild.Append(x));
                string2 = strbuild.ToString();
                strbuild.Clear();

                if (lststr1 != null)
                {
                    lststr1.ForEach(x => strbuild.Append(x[0]));
                    string1 = strbuild.ToString();
                }
                compareWhole = true;
            }
            else
            {
                compareWhole = true;
                if (lststr1 != null && lststr2 != null)
                {
                    compareWhole = false;
                    if (lststr1.Count < lststr2.Count)
                    {
                        minLength = lststr1.Count;
                        maxLength = lststr2.Count;
                    }
                    else
                    {
                        minLength = lststr2.Count;
                        maxLength = lststr1.Count;
                    }
                }
            }

            if (compareWhole)
            {
                double dist = JaroWinklerDistanceAlgorithm.GetMatchProbability(string1.ToLower(), string2.ToLower());
                if (isAbbreviation1 || isAbbreviation2)
                    dist *= 0.9;
                return dist;
            }
            else
            {
                for (int c = 0; c < minLength; c++)
                    netProbability += JaroWinklerDistanceAlgorithm.GetMatchProbability(lststr1[c].ToLower(), lststr2[c].ToLower());

                return (netProbability / maxLength);
            }
        }

        private static List<string> GetTokens(string str)
        {
            List<string> lststr = null;

            if (str.Contains(' '))
                lststr = str.Split(' ').AsEnumerable().Where(x => !string.IsNullOrEmpty(x)).ToList();

            else if (str.Contains('.'))
                lststr = str.Split('.').AsEnumerable().Where(x => !string.IsNullOrEmpty(x)).ToList();

            else if (str.Contains(". "))
            {
                str = str.Replace(". ", ".");
                lststr = str.Split('.').AsEnumerable().Where(x => !string.IsNullOrEmpty(x)).ToList();
            }
            return lststr;
        }

        private static bool isAbbreviation(List<string> lststr)
        {
            bool flag = true;
            foreach (string str in lststr)
            {
                if (str.Length != 1)
                    flag = false;
            }
            return flag;
        }
    }

    public class DeDupeWrapper
    {
        private string UserName { set; get; }
        private DeDupeConfig config = null;
        private HashSet<string> lstColumnstoFetch = new HashSet<string>();
        private Dictionary<string, Dictionary<string, Dictionary<string, MergeAttributeInfo>>> dictPercentVsSecIdVsAttrVsValues = new Dictionary<string, Dictionary<string, Dictionary<string, MergeAttributeInfo>>>();
        private Dictionary<string, KeyValuePair<MatchTypes, int>> lstCriteriaAttributes = new Dictionary<string, KeyValuePair<MatchTypes, int>>();
        private Dictionary<string, Attribute> attrNameVsConfigAttr = new Dictionary<string, Attribute>();
        private HashSet<string> hshAttributes = new HashSet<string> { "sec_id" };
        private Dictionary<string, DeDupeAttributeInfo> dictAttributeNameVsDeDupeAttributeInfo = null;
        private Dictionary<string, string> dictDisplayNameVsAttributeName = null;
        private Dictionary<int, string> dictIdVsSectype = null;

        public static Dictionary<int, Dictionary<string, string>> hshSecTypeVsAttributesInfo = null;

        private void SM_InitializeColumnsToFetch()
        {
            XElement root = null;
            DirectoryInfo di = Directory.GetParent(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);
            if (File.Exists(di.Parent.FullName + @"\CommonUI\ConfigFiles\DeDuplicationConfig.xml"))
                root = XElement.Load(di.Parent.FullName + @"\CommonUI\ConfigFiles\DeDuplicationConfig.xml");

            else if (File.Exists(di.FullName + @"\CommonUI\ConfigFiles\DeDuplicationConfig.xml"))
                root = XElement.Load(di.FullName + @"\CommonUI\ConfigFiles\DeDuplicationConfig.xml");
            else
                throw new Exception("Config File not found");

            if (root != null)
            {
                IEnumerable<XElement> ixe = root.Elements("attribute");
                if (ixe.Count() > 0)
                {
                    foreach (XElement xe in ixe)
                    {
                        lstColumnstoFetch.Add(xe.Attribute("display_name").Value);
                    }
                }
            }
        }

        private void LoadWorkflowDuplicateCheckConfig(int typeId)
        {
            XElement root = null;
            DirectoryInfo di = Directory.GetParent(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);
            if (File.Exists(di.Parent.FullName + @"\CommonUI\ConfigFiles\WorkflowDeDuplicationConfig.xml"))
                root = XElement.Load(di.Parent.FullName + @"\CommonUI\ConfigFiles\WorkflowDeDuplicationConfig.xml");

            else if (File.Exists(di.FullName + @"\CommonUI\ConfigFiles\WorkflowDeDuplicationConfig.xml"))
                root = XElement.Load(di.FullName + @"\CommonUI\ConfigFiles\WorkflowDeDuplicationConfig.xml");

            else if (File.Exists(di.FullName + @"\ConfigFiles\WorkflowDeDuplicationConfig.xml"))
                root = XElement.Load(di.FullName + @"\ConfigFiles\WorkflowDeDuplicationConfig.xml");
            else
                throw new Exception("Config File not found");

            if (root != null)
            {
                IEnumerable<XElement> ixe = null;
                var toShow = root.Element("attributes_to_show");
                if (toShow != null)
                {
                    ixe = toShow.Elements("attribute");
                    if (ixe.Count() > 0)
                    {
                        foreach (XElement xe in ixe)
                        {
                            lstColumnstoFetch.Add(xe.Attribute("display_name").Value);
                        }
                    }
                }
                var temp = root.Element("duplicate_check");
                if (temp != null)
                {
                    ixe = temp.Elements("security_type");
                    if (ixe.Count() > 0)
                    {
                        var sectypeName = Convert.ToString(CommonDALWrapper.ExecuteSelectQuery(string.Format("SELECT sectype_name FROM IVPSecMaster.dbo.ivp_secm_sectype_master WHERE is_active = 1 AND sectype_id = {0}", typeId), ConnectionConstants.SecMaster_Connection).Tables[0].Rows[0][0]);
                        foreach (XElement sectypeLevel in ixe)
                        {
                            var name = sectypeLevel.Attribute("name").Value;
                            if (name.Equals(sectypeName, StringComparison.OrdinalIgnoreCase))
                            {
                                config = new DeDupeConfig();
                                config.matchConfidence = Convert.ToInt32(sectypeLevel.Attribute("matchConfidence").Value);
                                config.moduleId = 3;
                                config.sectypeId = new int[] { typeId };
                                config.attrList = new List<Attribute>();

                                var attributeNameVsId = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT td.attribute_id, td.display_name
                                FROM IVPSecMaster.dbo.ivp_secm_attribute_details ad
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_table st ON ad.sectype_table_id = st.sectype_table_id
                                INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details td ON ad.attribute_id = td.attribute_id
                                WHERE template_id = IVPSecMaster.dbo.SECM_GetUserTemplate({0},'SYSTEM') AND td.display_name <>'Underlying Security ID'", typeId), ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["display_name"]), y => Convert.ToInt32(y["attribute_id"]));

                                ixe = sectypeLevel.Elements("attribute");
                                if (ixe.Count() > 0)
                                {
                                    foreach (XElement attributeLevel in ixe)
                                    {
                                        var attributeName = attributeLevel.Attribute("name").Value;
                                        if (attributeNameVsId.ContainsKey(attributeName))
                                        {
                                            var matchType = attributeLevel.Attribute("matchType").Value;
                                            var toleranceType = attributeLevel.Attribute("toleranceType").Value;
                                            MatchTypes mt;
                                            ToleranceType tt;

                                            if (!Enum.TryParse(matchType, out mt))
                                                throw new Exception("Invalid match type");
                                            var q = Enum.GetNames(typeof(ToleranceType)).Where(x => x.Equals(toleranceType, StringComparison.OrdinalIgnoreCase));
                                            if (q.Count() > 0)
                                                Enum.TryParse(q.First(), out tt);
                                            else
                                                throw new Exception("Invalid tolerance type");

                                            config.attrList.Add(new Attribute { attributeId = attributeNameVsId[attributeName], matchPercentage = Convert.ToInt32(attributeLevel.Attribute("matchPercentage").Value), matchTypeId = (int)mt, toleranceValue = Convert.ToInt32(attributeLevel.Attribute("tolerance").Value), toleranceOptionSelected = Enum.GetName(typeof(ToleranceType), tt) });
                                        }
                                    }
                                }
                            }
                        }
                        if (config == null)
                            throw new Exception("Security Type is not configured for DeDuplication Check");
                    }
                }
            }
        }

        internal DeDupeWrapper(int config_master_id, string userName, int moduleId)
        {
            if (moduleId == 3)
                SM_InitializeColumnsToFetch();
            this.UserName = userName;
            if (config_master_id != 0)
                config = DeDupeController.GetConfig(config_master_id);
        }

        internal DeDupeWrapper(DeDupeConfig config, string userName, int moduleId)
        {
            if (moduleId == 3)
                SM_InitializeColumnsToFetch();
            this.UserName = userName;
            this.config = config;
        }

        public DeDupeWrapper(string userName, int moduleId, int typeId)
        {
            if (moduleId == 3)
                LoadWorkflowDuplicateCheckConfig(typeId);
            this.UserName = userName;
        }

        public int MatchConfidence
        {
            get
            {
                if (config != null)
                    return config.matchConfidence;
                else
                    return 0;
            }
        }

        private bool CheckToleranceDateTime(ToleranceType tolType, int tolValue, object baseValue, object value)
        {
            bool returnStatus = false;

            DateTime baseDatetime = (!string.IsNullOrWhiteSpace(Convert.ToString(baseValue)) ? Convert.ToDateTime(baseValue) : default(DateTime));
            DateTime valDateTime = (!string.IsNullOrWhiteSpace(Convert.ToString(value)) ? Convert.ToDateTime(value) : default(DateTime));
            Int32 toleranceVal = Convert.ToInt32(tolValue);

            switch (tolType)
            {
                case ToleranceType.Days:
                    //if ((baseDatetime.AddDays(toleranceVal) > valDateTime) && (baseDatetime.AddDays(toleranceVal * -1) < valDateTime))
                    if (Math.Abs((baseDatetime - valDateTime).Days) <= toleranceVal)
                    {
                        returnStatus = true;
                    }
                    else
                    {
                        returnStatus = false;
                    }
                    break;
                case ToleranceType.Months:
                    if ((baseDatetime.AddMonths(toleranceVal) >= valDateTime) && (baseDatetime.AddMonths(toleranceVal * -1) <= valDateTime))
                    {
                        returnStatus = true;
                    }
                    else
                    {
                        returnStatus = false;
                    }
                    break;
                case ToleranceType.Years:
                    if ((baseDatetime.AddYears(toleranceVal) >= valDateTime) && (baseDatetime.AddYears(toleranceVal * -1) <= valDateTime))
                    {
                        returnStatus = true;
                    }
                    else
                    {
                        returnStatus = false;
                    }
                    break;
                case ToleranceType.Hours:
                    //if ((baseDatetime.AddHours(toleranceVal) > valDateTime) && (baseDatetime.AddHours(toleranceVal * -1) < valDateTime))
                    if (Math.Abs((baseDatetime - valDateTime).Hours) <= toleranceVal)
                    {
                        returnStatus = true;
                    }
                    else
                    {
                        returnStatus = false;
                    }
                    break;
                case ToleranceType.Minutes:
                    //if ((baseDatetime.AddMinutes(toleranceVal) > valDateTime) && (baseDatetime.AddMinutes(toleranceVal * -1) < valDateTime))
                    if (Math.Abs((baseDatetime - valDateTime).Minutes) <= toleranceVal)
                    {
                        returnStatus = true;
                    }
                    else
                    {
                        returnStatus = false;
                    }
                    break;
                case ToleranceType.Seconds:
                    //if ((baseDatetime.AddSeconds(toleranceVal) > valDateTime) && (baseDatetime.AddSeconds(toleranceVal * -1) < valDateTime))
                    if (Math.Abs((baseDatetime - valDateTime).Seconds) <= toleranceVal)
                    {
                        returnStatus = true;
                    }
                    else
                    {
                        returnStatus = false;
                    }
                    break;
                case ToleranceType.Weeks:
                    //if ((baseDatetime.AddDays(7 * toleranceVal) > valDateTime) && (baseDatetime.AddDays(toleranceVal * 7 * -1) < valDateTime))
                    if (Math.Ceiling((double)Math.Abs((baseDatetime - valDateTime).Days / 7)) <= toleranceVal)
                    {
                        returnStatus = true;
                    }
                    else
                    {
                        returnStatus = false;
                    }
                    break;
            }
            return returnStatus;
        }

        private bool CheckTolerance(int toleranceValue, ToleranceType toleranceOption, string dataType, object baseValue, object value)
        {
            bool returnStatus = false;
            switch (dataType)
            {
                case "NUMERIC":
                    Decimal baseVal = (!string.IsNullOrWhiteSpace(Convert.ToString(baseValue)) ? Convert.ToDecimal(baseValue) : 0);
                    Decimal val = (!string.IsNullOrWhiteSpace(Convert.ToString(value)) ? Convert.ToDecimal(value) : 0);
                    Decimal tolVal = Convert.ToDecimal(toleranceValue);
                    if (toleranceOption == ToleranceType.Percentage)
                    {
                        if (baseVal == 0)
                        {
                            if (val != 0)
                                return false;
                            else
                                return true;
                        }
                        else if (((Math.Abs(baseVal - val) / baseVal) * 100) <= tolVal)
                            returnStatus = true;
                        else
                            returnStatus = false;
                    }
                    else if (toleranceOption == ToleranceType.Absolute)
                    {
                        if (Math.Abs(baseVal - val) <= tolVal)
                            returnStatus = true;
                        else
                            returnStatus = false;
                    }
                    break;
                case "DATETIME":
                case "DATE":
                case "TIME":
                    returnStatus = CheckToleranceDateTime(toleranceOption, toleranceValue, baseValue, value);
                    break;
            }
            return returnStatus;
        }

        public void IdentifyDuplicates(DataTable dtRes, int countOfSecuritiesToRunFor = 0)
        {
            try
            {
                int lowerLimit = 0;
                if (countOfSecuritiesToRunFor == 0)
                    lowerLimit = 0;
                else
                    lowerLimit = dtRes.Rows.Count - 1 - countOfSecuritiesToRunFor;

                #region FOR DEBUGGING
                //List<DataRow> rowsToRemove = new List<DataRow>();
                //for (var i = 0; i < dtRes.Rows.Count; i++)
                //{
                //    var x = dtRes.Rows[i];
                //    if (Convert.ToString(x["sec_id"]) != "REDE000007" && i != dtRes.Rows.Count - 1)
                //        rowsToRemove.Add(x);
                //}

                //foreach (var row in rowsToRemove)
                //    dtRes.Rows.Remove(row);
                //lowerLimit = 0;
                #endregion

                for (int i = dtRes.Rows.Count - 1; i > lowerLimit; i--)
                {
                    DataRow drT = dtRes.Rows[i];

                    Dictionary<string, MergeAttributeInfo> dicTempSource = new Dictionary<string, MergeAttributeInfo>();
                    foreach (KeyValuePair<string, KeyValuePair<MatchTypes, int>> kvp in lstCriteriaAttributes)
                    {
                        DeDupeAttributeInfo ai = dictAttributeNameVsDeDupeAttributeInfo[kvp.Key];
                        string sourceAttrValue = string.Empty;
                        if (ai.Datatype.Equals("DATE", StringComparison.OrdinalIgnoreCase) || ai.Datatype.Equals("TIME", StringComparison.OrdinalIgnoreCase) || ai.Datatype.Equals("DATETIME", StringComparison.OrdinalIgnoreCase))
                            sourceAttrValue = GetFormattedValue(Convert.ToString(drT[kvp.Key]), ai.Datatype).Key;
                        else
                            sourceAttrValue = Convert.ToString(drT[kvp.Key]);

                        dicTempSource.Add(ai.DisplayName, new MergeAttributeInfo { ReferenceAttributeName = ai.ReferenceAttributeName, ReferenceTypeId = ai.ReferenceTypeId, Value = sourceAttrValue });
                    }
                    foreach (string attr in lstColumnstoFetch)
                    {
                        if (attr.Equals("Security Type", StringComparison.OrdinalIgnoreCase))
                        {
                            dicTempSource.Add(attr, new MergeAttributeInfo { ReferenceAttributeName = string.Empty, ReferenceTypeId = 0, Value = dictIdVsSectype[Convert.ToInt32(drT["sectype_id"])] });
                        }
                        else if (!attr.Equals("Security Id", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!dicTempSource.ContainsKey(attr))
                            {
                                DeDupeAttributeInfo ai = dictAttributeNameVsDeDupeAttributeInfo[dictDisplayNameVsAttributeName[attr]];
                                string sourceAttrValue = string.Empty;
                                if (ai.Datatype.Equals("DATE", StringComparison.OrdinalIgnoreCase) || ai.Datatype.Equals("TIME", StringComparison.OrdinalIgnoreCase) || ai.Datatype.Equals("DATETIME", StringComparison.OrdinalIgnoreCase))
                                    sourceAttrValue = GetFormattedValue(Convert.ToString(drT[dictDisplayNameVsAttributeName[attr]]), ai.Datatype).Key;
                                else
                                    sourceAttrValue = Convert.ToString(drT[dictDisplayNameVsAttributeName[attr]]);

                                dicTempSource.Add(attr, new MergeAttributeInfo { ReferenceAttributeName = ai.ReferenceAttributeName, ReferenceTypeId = ai.ReferenceTypeId, Value = sourceAttrValue });
                            }
                        }
                    }

                    Parallel.ForEach(dtRes.AsEnumerable().Take(i), drTemp =>
                    {
                        Dictionary<string, MergeAttributeInfo> dicTempDestination = new Dictionary<string, MergeAttributeInfo>();
                        Dictionary<string, double> dicTempBool = new Dictionary<string, double>();
                        bool isMatch = false;

                        foreach (KeyValuePair<string, KeyValuePair<MatchTypes, int>> kvp in lstCriteriaAttributes)
                        {
                            string sourceAttrValue = Convert.ToString(drT[kvp.Key]);
                            string destAttrValue = Convert.ToString(drTemp[kvp.Key]);
                            string datatype = dictAttributeNameVsDeDupeAttributeInfo[kvp.Key].Datatype;
                            int toleranceValue = -1;
                            bool isToleranceSatisfied = false;
                            ToleranceType toleranceOption = ToleranceType.None;

                            if (attrNameVsConfigAttr.ContainsKey(kvp.Key))
                            {
                                toleranceValue = attrNameVsConfigAttr[kvp.Key].toleranceValue;
                                if (attrNameVsConfigAttr[kvp.Key].toleranceOptionSelected != null)
                                    toleranceOption = (ToleranceType)Enum.Parse(typeof(ToleranceType), attrNameVsConfigAttr[kvp.Key].toleranceOptionSelected);
                            }

                            if (datatype.Equals("DATE", StringComparison.OrdinalIgnoreCase))
                            {
                                KeyValuePair<string, string> valuekvp = GetFormattedValue(sourceAttrValue, datatype, destAttrValue);
                                sourceAttrValue = valuekvp.Key;
                                destAttrValue = valuekvp.Value;
                                if (CheckTolerance(toleranceValue, toleranceOption, datatype, sourceAttrValue, destAttrValue))
                                {
                                    isToleranceSatisfied = true;
                                }
                            }
                            else if (datatype.Equals("DATETIME", StringComparison.OrdinalIgnoreCase))
                            {
                                KeyValuePair<string, string> valuekvp = GetFormattedValue(sourceAttrValue, datatype, destAttrValue);
                                sourceAttrValue = valuekvp.Key;
                                destAttrValue = valuekvp.Value;
                                if (CheckTolerance(toleranceValue, toleranceOption, datatype, sourceAttrValue, destAttrValue))
                                {
                                    isToleranceSatisfied = true;
                                }
                            }
                            else if (datatype.Equals("TIME", StringComparison.OrdinalIgnoreCase))
                            {
                                KeyValuePair<string, string> valuekvp = GetFormattedValue(sourceAttrValue, datatype, destAttrValue);
                                sourceAttrValue = valuekvp.Key;
                                destAttrValue = valuekvp.Value;
                                if (CheckTolerance(toleranceValue, toleranceOption, datatype, sourceAttrValue, destAttrValue))
                                {
                                    isToleranceSatisfied = true;
                                }
                            }
                            else if (datatype.Equals("NUMERIC", StringComparison.OrdinalIgnoreCase))
                            {
                                if (CheckTolerance(toleranceValue, toleranceOption, datatype, sourceAttrValue, destAttrValue))
                                {
                                    isToleranceSatisfied = true;
                                }
                            }
                            else
                            {
                                isToleranceSatisfied = false;
                            }

                            if (kvp.Value.Key == MatchTypes.ExactMatch)
                            {
                                if (sourceAttrValue.Equals(destAttrValue, StringComparison.OrdinalIgnoreCase))
                                {
                                    lock (((IDictionary)dicTempBool).SyncRoot)
                                    {
                                        dicTempBool.Add(kvp.Key, 1);
                                    }
                                    isMatch = true;
                                }
                                else
                                {
                                    lock (((IDictionary)dicTempBool).SyncRoot)
                                    {
                                        dicTempBool.Add(kvp.Key, 0);
                                    }
                                }
                            }
                            else
                            {
                                lock (((IDictionary)dicTempBool).SyncRoot)
                                {
                                    if (isToleranceSatisfied && !datatype.Equals("STRING", StringComparison.OrdinalIgnoreCase))
                                    {
                                        dicTempBool.Add(kvp.Key, 1);
                                        isMatch = true;
                                    }
                                    else if (datatype.Equals("STRING", StringComparison.OrdinalIgnoreCase))
                                    {
                                        double stringMatch = DeDupAlgo.GetMatchProbability(sourceAttrValue, destAttrValue);
                                        dicTempBool.Add(kvp.Key, stringMatch);
                                        if (stringMatch > 0.0)
                                        {
                                            isMatch = true;
                                        }
                                    }
                                }
                            }

                            lock (((IDictionary)dicTempDestination).SyncRoot)
                            {
                                DeDupeAttributeInfo ai = dictAttributeNameVsDeDupeAttributeInfo[kvp.Key];
                                dicTempDestination.Add(ai.DisplayName, new MergeAttributeInfo { ReferenceAttributeName = ai.ReferenceAttributeName, ReferenceTypeId = ai.ReferenceTypeId, Value = destAttrValue });
                            }
                        }

                        foreach (string attr in lstColumnstoFetch)
                        {
                            if (attr.Equals("Security Type", StringComparison.OrdinalIgnoreCase))
                            {
                                lock (((IDictionary)dicTempDestination).SyncRoot)
                                {
                                    dicTempDestination.Add(attr, new MergeAttributeInfo { ReferenceAttributeName = string.Empty, ReferenceTypeId = 0, Value = dictIdVsSectype[Convert.ToInt32(drTemp["sectype_id"])] });
                                }
                            }
                            else if (!attr.Equals("Security Id", StringComparison.OrdinalIgnoreCase))
                            {
                                lock (((IDictionary)dicTempDestination).SyncRoot)
                                {
                                    if (!dicTempDestination.ContainsKey(attr))
                                    {
                                        DeDupeAttributeInfo ai = dictAttributeNameVsDeDupeAttributeInfo[dictDisplayNameVsAttributeName[attr]];
                                        string destAttrValue = string.Empty;
                                        if (ai.Datatype.Equals("DATE", StringComparison.OrdinalIgnoreCase) || ai.Datatype.Equals("TIME", StringComparison.OrdinalIgnoreCase) || ai.Datatype.Equals("DATETIME", StringComparison.OrdinalIgnoreCase))
                                            destAttrValue = GetFormattedValue(Convert.ToString(drTemp[dictDisplayNameVsAttributeName[attr]]), ai.Datatype).Key;
                                        else
                                            destAttrValue = Convert.ToString(drTemp[dictDisplayNameVsAttributeName[attr]]);

                                        dicTempDestination.Add(attr, new MergeAttributeInfo { ReferenceAttributeName = ai.ReferenceAttributeName, ReferenceTypeId = ai.ReferenceTypeId, Value = destAttrValue });
                                    }
                                }
                            }
                        }

                        if (isMatch)
                        {
                            double percentage = 0;
                            foreach (KeyValuePair<string, KeyValuePair<MatchTypes, int>> kvp in lstCriteriaAttributes)
                            {
                                lock (((IDictionary)dicTempBool).SyncRoot)
                                {
                                    if (dicTempBool.ContainsKey(kvp.Key))
                                        percentage += dicTempBool[kvp.Key] * kvp.Value.Value;
                                }
                            }
                            string sourceSecId = Convert.ToString(drT["sec_id"]);
                            string destinationSecId = Convert.ToString(drTemp["sec_id"]);
                            decimal percent = (decimal)percentage;

                            if (percent >= config.matchConfidence && sourceSecId != destinationSecId)
                            {
                                string key = percent.ToString("#.##") + "%_" + sourceSecId;
                                lock (((IDictionary)dictPercentVsSecIdVsAttrVsValues).SyncRoot)
                                {
                                    if (!dictPercentVsSecIdVsAttrVsValues.ContainsKey(key))
                                        dictPercentVsSecIdVsAttrVsValues.Add(key, new Dictionary<string, Dictionary<string, MergeAttributeInfo>>());

                                    if (!dictPercentVsSecIdVsAttrVsValues[key].ContainsKey(sourceSecId))
                                        dictPercentVsSecIdVsAttrVsValues[key].Add(sourceSecId, dicTempSource);

                                    lock (((IDictionary)dicTempDestination).SyncRoot)
                                    {
                                        if (!dictPercentVsSecIdVsAttrVsValues[key].ContainsKey(destinationSecId))
                                            dictPercentVsSecIdVsAttrVsValues[key].Add(destinationSecId, dicTempDestination);
                                    }
                                }
                            }
                        }
                    });
                }

                Dictionary<string, Dictionary<string, SortInfo>> dictPercentVsSecIds = new Dictionary<string, Dictionary<string, SortInfo>>();
                Dictionary<string, Dictionary<string, Dictionary<string, MergeAttributeInfo>>> tempdic = new Dictionary<string, Dictionary<string, Dictionary<string, MergeAttributeInfo>>>();
                Parallel.ForEach(dictPercentVsSecIdVsAttrVsValues, kvp =>
                {
                    string[] arr = kvp.Key.Split('%');
                    string key = arr[0] + "%  ";
                    HashSet<string> hshSecIds = new HashSet<string>();
                    foreach (KeyValuePair<string, Dictionary<string, MergeAttributeInfo>> kvp1 in kvp.Value)
                    {
                        hshSecIds.Add(kvp1.Key);
                        foreach (KeyValuePair<string, MergeAttributeInfo> kvp2 in kvp1.Value)
                        {
                            if (kvp2.Key == "Security Name")
                                key += kvp2.Value.Value + ", ";
                        }
                    }
                    key = key.Substring(0, key.Length - 2);
                    key = key + arr[1];

                    lock (((IDictionary)tempdic).SyncRoot)
                    {
                        tempdic[key] = kvp.Value;
                    }
                    lock (((IDictionary)dictPercentVsSecIds).SyncRoot)
                    {
                        if (!dictPercentVsSecIds.ContainsKey(arr[0]))
                            dictPercentVsSecIds[arr[0]] = new Dictionary<string, SortInfo>();
                        if (!dictPercentVsSecIds[arr[0]].ContainsKey(arr[1]))
                            dictPercentVsSecIds[arr[0]][arr[1]] = new SortInfo { hshSecIds = hshSecIds, Key = key };
                    }
                });

                dictPercentVsSecIdVsAttrVsValues = tempdic;
                foreach (KeyValuePair<string, Dictionary<string, SortInfo>> kvo in dictPercentVsSecIds)
                {
                    foreach (KeyValuePair<string, SortInfo> kvoo in kvo.Value)
                    {
                        foreach (KeyValuePair<string, SortInfo> kvoo1 in kvo.Value)
                        {
                            if (kvoo.Key != kvoo1.Key && kvoo1.Value.hshSecIds.Intersect(kvoo.Value.hshSecIds).Count() == kvoo.Value.hshSecIds.Count)
                            {
                                dictPercentVsSecIdVsAttrVsValues.Remove(kvoo.Value.Key);
                                break;
                            }
                        }
                    }
                }
                dictPercentVsSecIdVsAttrVsValues = dictPercentVsSecIdVsAttrVsValues.OrderByDescending(x => Convert.ToDecimal(x.Key.Split('%')[0])).ToDictionary(x => x.Key, y => y.Value);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, MergeAttributeInfo>>> GetDuplicates(string secId = null, Dictionary<string, string> attributeNameVsValue = null)
        {
            try
            {
                DataTable dtAttributes = DeDupeController.GetAttributes();
                Dictionary<int, DataRow> dictAttributesById = new Dictionary<int, DataRow>();
                Dictionary<string, DataRow> dictCommonAttributesByName = new Dictionary<string, DataRow>();
                dictAttributeNameVsDeDupeAttributeInfo = new Dictionary<string, DeDupeAttributeInfo>();
                dictDisplayNameVsAttributeName = new Dictionary<string, string>();

                dictIdVsSectype = DeDupeController.GetSectypes(UserName).AsEnumerable().ToDictionary(x => Convert.ToInt32(x["sectype_id"]), y => Convert.ToString(y["sectype_name"]));
                HashSet<int> hshSectypeIds = new HashSet<int>(config.sectypeId);
                foreach (DataRow dr in dtAttributes.Rows)
                {
                    int sectypeId = Convert.ToInt32(dr["sectype_id"]);
                    if (sectypeId == 0 || hshSectypeIds.Contains(sectypeId))
                    {
                        dictAttributesById.Add(Convert.ToInt32(dr["attribute_id"]), dr);
                        string displayName = Convert.ToString(dr["display_name"]);
                        string attributeName = Convert.ToString(dr["attribute_name"]);
                        string referenceTypeId = Convert.ToString(dr["reference_type_id"]);
                        string referenceAttributeName = Convert.ToString(dr["reference_attribute_name"]);

                        if (Convert.ToInt32(dr["sectype_id"]) == 0)
                            dictCommonAttributesByName.Add(displayName, dr);

                        if (!dictAttributeNameVsDeDupeAttributeInfo.ContainsKey(attributeName))
                        {
                            dictAttributeNameVsDeDupeAttributeInfo.Add(attributeName, new DeDupeAttributeInfo { Datatype = Convert.ToString(dr["data_type_name"]), DisplayName = displayName, ReferenceAttributeName = referenceAttributeName, ReferenceTypeId = (!string.IsNullOrEmpty(referenceTypeId) ? Convert.ToInt32(referenceTypeId) : 0) });
                        }
                        if (!dictDisplayNameVsAttributeName.ContainsKey(displayName))
                        {
                            dictDisplayNameVsAttributeName.Add(displayName, attributeName);
                        }
                    }
                }

                foreach (Attribute al in config.attrList)
                {
                    MatchTypes mt = (MatchTypes)Enum.Parse(typeof(MatchTypes), Convert.ToString(al.matchTypeId));
                    string attName = Convert.ToString(dictAttributesById[al.attributeId]["attribute_name"]);
                    if (!lstCriteriaAttributes.ContainsKey(attName))
                    {
                        lstCriteriaAttributes.Add(attName, new KeyValuePair<MatchTypes, int>(mt, al.matchPercentage));
                    }
                    hshAttributes.Add(attName);
                    if (!attrNameVsConfigAttr.ContainsKey(attName))
                    {
                        attrNameVsConfigAttr.Add(attName, al);
                    }
                }

                foreach (string attr in lstColumnstoFetch)
                {
                    if (!attr.Equals("Security Type", StringComparison.OrdinalIgnoreCase) && !attr.Equals("Security Id", StringComparison.OrdinalIgnoreCase))
                    {
                        if (dictCommonAttributesByName.ContainsKey(attr))
                        {
                            hshAttributes.Add(Convert.ToString(dictCommonAttributesByName[attr]["attribute_name"]));
                        }
                    }
                }

                DataTable dtRes = DeDupeController.GetSecurities(hshAttributes, hshSectypeIds, UserName);

                if (secId != null)
                {
                    var q = dtRes.AsEnumerable().Where(x => Convert.ToString(x["sec_id"]) == secId);
                    if (q.Count() > 0)
                        dtRes.Rows.Remove(q.First());

                    var ndr = dtRes.NewRow();
                    foreach (var attr in hshAttributes)
                    {
                        if (attr.Equals("sec_id", StringComparison.OrdinalIgnoreCase))
                            ndr[attr] = secId;
                        else if (attributeNameVsValue.ContainsKey(attr) && !string.IsNullOrEmpty(attributeNameVsValue[attr]))
                            ndr[attr] = attributeNameVsValue[attr];
                    }
                    dtRes.Rows.Add(ndr);
                }

                Dictionary<int, RMAttributeValueInfoOptimized> entitytypeidVsobject = new Dictionary<int, RMAttributeValueInfoOptimized>();

                foreach (DataRow dr in dtRes.Rows)
                {
                    foreach (DataColumn dc in dtRes.Columns)
                    {
                        if (dictAttributeNameVsDeDupeAttributeInfo.ContainsKey(dc.ColumnName))
                        {
                            string attrValue = Convert.ToString(dr[dc.ColumnName]);
                            DeDupeAttributeInfo ai = dictAttributeNameVsDeDupeAttributeInfo[dc.ColumnName];
                            if (ai.ReferenceTypeId != 0)
                            {
                                if (!entitytypeidVsobject.ContainsKey(ai.ReferenceTypeId))
                                    entitytypeidVsobject[ai.ReferenceTypeId] = new RMAttributeValueInfoOptimized { EntityTypeId = ai.ReferenceTypeId, AttributeList = new HashSet<string> { ai.ReferenceAttributeName }, EntityCodeList = new HashSet<string> { attrValue } };
                                else
                                {
                                    RMAttributeValueInfoOptimized obj = entitytypeidVsobject[ai.ReferenceTypeId];
                                    obj.EntityCodeList.Add(attrValue);
                                    obj.AttributeList.Add(ai.ReferenceAttributeName);
                                }
                            }
                        }
                    }
                }

                //List<RMAttributeValueInfoOptimized> refdata = new SMRefDataMassage().GetMasssagedData(entitytypeidVsobject.Values.ToList());
                //Dictionary<int, RMAttributeValueInfoOptimized> tempo = new Dictionary<int, RMAttributeValueInfoOptimized>();
                //Parallel.ForEach(entitytypeidVsobject, kvp =>
                //{
                //    RMAttributeValueInfoOptimized obj = refdata.Where(x => x.EntityTypeId == kvp.Key).FirstOrDefault();
                //    lock (((IDictionary)tempo).SyncRoot)
                //    {
                //        tempo[kvp.Key] = obj;
                //    }
                //});
                //entitytypeidVsobject = tempo;
                Assembly SecMasterCommonsAssembly = Assembly.Load("SecMasterCommons");
                Type ObjTypeDeDupeExtension = SecMasterCommonsAssembly.GetType("com.ivp.secm.commons.SMDeDupeExtension");
                var deDupeExtensionObj = Activator.CreateInstance(ObjTypeDeDupeExtension);
                MethodInfo getDuplicatesExtension = ObjTypeDeDupeExtension.GetMethod("GetDuplicatesExtension", BindingFlags.Public | BindingFlags.Static);
                entitytypeidVsobject = (Dictionary<int, RMAttributeValueInfoOptimized>)getDuplicatesExtension.Invoke(ObjTypeDeDupeExtension, new object[] { entitytypeidVsobject });

                foreach (DataRow dr in dtRes.Rows)
                {
                    foreach (DataColumn dc in dtRes.Columns)
                    {
                        if (dictAttributeNameVsDeDupeAttributeInfo.ContainsKey(dc.ColumnName))
                        {
                            DeDupeAttributeInfo ai = dictAttributeNameVsDeDupeAttributeInfo[dc.ColumnName];
                            if (ai.ReferenceTypeId != 0)
                            {
                                string attrValue = Convert.ToString(dr[dc.ColumnName]);
                                RMAttributeValueInfoOptimized objref = entitytypeidVsobject[ai.ReferenceTypeId];
                                if (objref.ResultSet != null && !string.IsNullOrEmpty(attrValue))
                                {
                                    IEnumerable<DataRow> whereClause = objref.ResultSet.AsEnumerable().Where(x => Convert.ToString(x["entity_code"]) == attrValue);
                                    if (whereClause.Count() > 0)
                                        dr[dc.ColumnName] = Convert.ToString(whereClause.FirstOrDefault()[ai.ReferenceAttributeName]);
                                }
                            }
                        }
                    }
                }

                IdentifyDuplicates(dtRes, (!string.IsNullOrEmpty(secId) ? 1 : 0));

                return dictPercentVsSecIdVsAttrVsValues;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, MergeAttributeInfo>>> GetEntityDuplicates()
        {
            try
            {
                int entity_type_id = 0;
                if (config.sectypeId != null && config.sectypeId.Length == 1)
                    entity_type_id = config.sectypeId[0];
                DataTable dtAttributes = DeDupeController.GetEntityAttributes(entity_type_id);

                Dictionary<int, DataRow> dictAttributesById = new Dictionary<int, DataRow>();
                Dictionary<string, DataRow> dictCommonAttributesByName = new Dictionary<string, DataRow>();
                dictAttributeNameVsDeDupeAttributeInfo = new Dictionary<string, DeDupeAttributeInfo>();
                dictDisplayNameVsAttributeName = new Dictionary<string, string>();

                DataTable dtEntityTypes = RMCommonUtils.GetAllEntityTypesForUser(UserName);
                string entityTableName = Convert.ToString(dtEntityTypes.Select("entity_type_id=" + entity_type_id).FirstOrDefault()["entity_type_name"]);

                dictIdVsSectype = dtEntityTypes.AsEnumerable().ToDictionary(x => Convert.ToInt32(x["entity_type_id"]), y => Convert.ToString(y["entity_display_name"]));
                HashSet<int> hshSectypeIds = new HashSet<int>(config.sectypeId);
                foreach (DataRow dr in dtAttributes.Rows)
                {
                    int sectypeId = Convert.ToInt32(dr["entity_type_id"]);
                    if (sectypeId == 0 || hshSectypeIds.Contains(sectypeId))
                    {
                        dictAttributesById.Add(Convert.ToInt32(dr["entity_attribute_id"]), dr);
                        string displayName = Convert.ToString(dr["display_name"]);
                        string attributeName = Convert.ToString(dr["attribute_name"]);
                        string referenceTypeId = Convert.ToString(dr["lookup_entity_type"]);
                        string referenceAttributeName = Convert.ToString(dr["lookup_attribute_id"]);

                        //if (Convert.ToInt32(dr["entity_type_id"]) == 0)
                        dictCommonAttributesByName.Add(displayName, dr);

                        dictAttributeNameVsDeDupeAttributeInfo.Add(attributeName, new DeDupeAttributeInfo { Datatype = Convert.ToString(dr["Original Data Type"]), DisplayName = displayName, ReferenceAttributeName = referenceAttributeName, ReferenceTypeId = (!string.IsNullOrEmpty(referenceTypeId) ? Convert.ToInt32(referenceTypeId) : 0) });
                        dictDisplayNameVsAttributeName.Add(displayName, attributeName);
                    }
                }

                hshAttributes.Remove("sec_id");
                hshAttributes.Add("entity_code");

                foreach (Attribute al in config.attrList)
                {
                    MatchTypes mt = (MatchTypes)Enum.Parse(typeof(MatchTypes), Convert.ToString(al.matchTypeId));
                    string attName = Convert.ToString(dictAttributesById[al.attributeId]["attribute_name"]);
                    lstCriteriaAttributes.Add(attName, new KeyValuePair<MatchTypes, int>(mt, al.matchPercentage));
                    hshAttributes.Add(attName);
                    attrNameVsConfigAttr.Add(attName, al);
                }

                //Start Change to fetch only the attributes selected in criteria along with entity code

                //lstColumnstoFetch = dtAttributes.AsEnumerable().Select(x => Convert.ToString(x["display_name"])).ToHashSet();

                lstColumnstoFetch = new HashSet<string>();
                foreach (DataRow dr in dtAttributes.Rows)
                {
                    int attrid = Convert.ToInt32(dr["entity_attribute_id"]);
                    string attributename = Convert.ToString(dr["display_name"]);
                    if (config.attrList.Any(x => x.attributeId == attrid))
                        lstColumnstoFetch.Add(attributename);
                }
                //End Change to fetch only the attributes selected in criteria along with entity code

                foreach (string attr in lstColumnstoFetch)
                {
                    if (!attr.Equals("Security Type", StringComparison.OrdinalIgnoreCase) && !attr.Equals("Security Id", StringComparison.OrdinalIgnoreCase))
                        hshAttributes.Add(Convert.ToString(dictCommonAttributesByName[attr]["attribute_name"]));
                }

                DataTable dtRes = DeDupeController.GetEntities(hshAttributes, entity_type_id);
                Dictionary<int, RMAttributeValueInfoOptimized> entitytypeidVsobject = new Dictionary<int, RMAttributeValueInfoOptimized>();

                /*
                foreach (DataRow dr in dtRes.Rows)
                {
                    foreach (DataColumn dc in dtRes.Columns)
                    {
                        if (dictAttributeNameVsDeDupeAttributeInfo.ContainsKey(dc.ColumnName))
                        {
                            string attrValue = Convert.ToString(dr[dc.ColumnName]);
                            DeDupeAttributeInfo ai = dictAttributeNameVsDeDupeAttributeInfo[dc.ColumnName];
                            if (ai.ReferenceTypeId != 0)
                            {
                                if (!entitytypeidVsobject.ContainsKey(ai.ReferenceTypeId))
                                    entitytypeidVsobject[ai.ReferenceTypeId] = new RMAttributeValueInfoOptimized { EntityTypeId = ai.ReferenceTypeId, AttributeList = new HashSet<string> { ai.ReferenceAttributeName }, EntityCodeList = new HashSet<string> { attrValue } };
                                else
                                {
                                    RMAttributeValueInfoOptimized obj = entitytypeidVsobject[ai.ReferenceTypeId];
                                    obj.EntityCodeList.Add(attrValue);
                                    obj.AttributeList.Add(ai.ReferenceAttributeName);
                                }
                            }
                        }
                    }
                }

                List<RMAttributeValueInfoOptimized> refdata = new SMRefDataMassage().GetMasssagedData(entitytypeidVsobject.Values.ToList());
                Dictionary<int, RMAttributeValueInfoOptimized> tempo = new Dictionary<int, RMAttributeValueInfoOptimized>();
                Parallel.ForEach(entitytypeidVsobject, kvp =>
                {
                    RMAttributeValueInfoOptimized obj = refdata.Where(x => x.EntityTypeId == kvp.Key).FirstOrDefault();
                    lock (((IDictionary)tempo).SyncRoot)
                    {
                        tempo[kvp.Key] = obj;
                    }
                });
                entitytypeidVsobject = tempo;

                foreach (DataRow dr in dtRes.Rows)
                {
                    foreach (DataColumn dc in dtRes.Columns)
                    {
                        if (dictAttributeNameVsDeDupeAttributeInfo.ContainsKey(dc.ColumnName))
                        {
                            DeDupeAttributeInfo ai = dictAttributeNameVsDeDupeAttributeInfo[dc.ColumnName];
                            if (ai.ReferenceTypeId != 0)
                            {
                                string attrValue = Convert.ToString(dr[dc.ColumnName]);
                                RMAttributeValueInfoOptimized objref = entitytypeidVsobject[ai.ReferenceTypeId];
                                if (objref.ResultSet != null && !string.IsNullOrEmpty(attrValue))
                                {
                                    IEnumerable<DataRow> whereClause = objref.ResultSet.AsEnumerable().Where(x => Convert.ToString(x["entity_code"]) == attrValue);
                                    if (whereClause.Count() > 0)
                                        dr[dc.ColumnName] = Convert.ToString(whereClause.FirstOrDefault()[ai.ReferenceAttributeName]);
                                }
                            }
                        }
                    }
                }
                
                */

                IdentifyDuplicates(dtRes);

                return dictPercentVsSecIdVsAttrVsValues;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private KeyValuePair<string, string> GetFormattedValue(string sourceValue, string datatype, string destValue = null)
        {
            string sour = sourceValue, dest = destValue;
            if (datatype.Equals("DATE", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(sourceValue))
                    sour = Convert.ToDateTime(sourceValue).ToShortDateString();
                if (!string.IsNullOrEmpty(destValue))
                    dest = Convert.ToDateTime(destValue).ToShortDateString();
            }
            else if (datatype.Equals("DATETIME", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(sourceValue))
                    sour = Convert.ToDateTime(sourceValue).ToString(@"MM-dd-yyyy hh\:mm\:ss");
                if (!string.IsNullOrEmpty(destValue))
                    dest = Convert.ToDateTime(destValue).ToString(@"MM-dd-yyyy hh\:mm\:ss");
            }

            else if (datatype.Equals("TIME", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(sourceValue))
                    sour = Convert.ToDateTime(sourceValue).ToString(@"hh\:mm\:ss");
                if (!string.IsNullOrEmpty(destValue))
                    dest = Convert.ToDateTime(destValue).ToString(@"hh\:mm\:ss");
            }

            return new KeyValuePair<string, string>(sour, dest);
        }
    }

    public static class DeDupeController
    {
        internal static Assembly SecMasterServiceAPIAssembly = null;
        internal static Assembly SecMasterCoreAssembly = null;
        internal static Assembly SecMasterUpstreamAssembly = null;
        internal static Assembly SecMasterRefDataInterfaceLayerAssembly = null;
        internal static Assembly SecMasterCommonsAssembly = null;

        internal static List<DeDupeListItem> GetDeDuplicateModuleList()
        {
            try
            {
                //DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT module_id, module_name FROM  IVPSecMaster.dbo.ivp_secm_dupes_module_type", ConnectionConstants.SecMaster_Connection);
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT module_id, module_name FROM  IVPRefMaster.dbo.ivp_refm_dupes_module_type", ConnectionConstants.RefMaster_Connection);
                List<DeDupeListItem> moduleList = new List<DeDupeListItem>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DeDupeListItem temp = new DeDupeListItem();
                    temp.text = Convert.ToString(row["module_name"]);
                    temp.value = Convert.ToString(row["module_id"]);
                    moduleList.Add(temp);
                }
                return moduleList;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static List<DeDupeListItem> GetDeDuplicateMatchTypeList()
        {
            try
            {
                //DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT match_type_id, match_type_name FROM  IVPSecMaster.dbo.ivp_secm_dupes_match_type", ConnectionConstants.SecMaster_Connection);
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT match_type_id, match_type_name FROM  IVPRefMaster.dbo.ivp_refm_dupes_match_type", ConnectionConstants.RefMaster_Connection);
                List<DeDupeListItem> moduleList = new List<DeDupeListItem>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DeDupeListItem temp = new DeDupeListItem();
                    temp.text = Convert.ToString(row["match_type_name"]);
                    temp.value = Convert.ToString(row["match_type_id"]);
                    moduleList.Add(temp);
                }
                return moduleList;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static List<DeDupeListItem> GetAllSectypes(string userName)
        {
            try
            {
                List<DeDupeListItem> lstSectypes = new List<DeDupeListItem>();
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("select * from IVPSecMaster.dbo.SECM_GetUserSectypes('" + userName + "')", ConnectionConstants.SecMaster_Connection);
                DataTable dtSectype = ds.Tables[0].AsEnumerable().OrderBy(x => x["sectype_name"]).CopyToDataTable();
                foreach (DataRow row in dtSectype.Rows)
                {
                    var text = Convert.ToString(row["sectype_name"]);
                    var value = Convert.ToString(row["sectype_id"]);
                    DeDupeListItem obj = new DeDupeListItem();
                    obj.text = text;
                    obj.value = value;
                    lstSectypes.Add(obj);
                }
                return lstSectypes;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static List<DeDupeListItem> GetAllEntityTypes(string userName, int ModuleId = 0)
        {

            try
            {
                List<DeDupeListItem> lstEnttypes = new List<DeDupeListItem>();

                DataTable dtEnttype = RMCommonUtils.GetAllEntityTypesForUser(userName, ModuleId);
                foreach (DataRow row in dtEnttype.Rows)
                {
                    var value = Convert.ToString(row["entity_type_id"]);
                    var text = Convert.ToString(row["entity_display_name"]);
                    DeDupeListItem obj = new DeDupeListItem();
                    obj.text = text;
                    obj.value = value;
                    lstEnttypes.Add(obj);
                }
                return lstEnttypes;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static string[] GetEntityTypeAttributesForDeDupe(int entityTypeId)
        {

            try
            {
                List<DeDupeListItem> lstEnttypes = new List<DeDupeListItem>();

                DataTable dt = RMCommonUtils.GetAllEntityTypeAttributeDetails(entityTypeId).Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.AsEnumerable().Select(x => x.Field<int>("entity_attribute_id") + "&&" + x.Field<string>("Data Type") + "|" + x.Field<string>("Display Name")).ToArray();
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static bool CheckEntityHasDependency(List<string> entityCodes)
        {
            return true; //DeDupeDBController.CheckEntityHasDependency(entityCodes);
        }

        private static DeDupeConfig CreateRMConfigObject(DataRow[] rows)
        {
            try
            {
                if (rows != null && rows.Length > 0)
                {
                    DeDupeConfig config = new DeDupeConfig();

                    for (int i = 0; i < rows.Length; i++)
                    {
                        DataRow dr = rows[i];
                        if (i == 0)
                        {
                            config.sectypeId = Convert.ToString(dr["entity_type_id"]).Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                            config.moduleId = Convert.ToInt32(dr["module_id"]);
                            config.matchConfidence = Convert.ToInt32(dr["match_confidence"]);
                            config.configName = Convert.ToString(dr["config_name"]);
                            config.attrList = new List<Attribute>();
                        }
                        if (!String.IsNullOrEmpty(Convert.ToString(dr["attribute_tolerance_type"])) && Convert.ToInt32(dr["attribute_tolerance_value"]) != 0)
                        {
                            config.attrList.Add(new Attribute { attributeId = Convert.ToInt32(dr["attribute_id"]), matchPercentage = Convert.ToInt32(dr["match_percentage"]), matchTypeId = Convert.ToInt32(dr["match_type_id"]), toleranceValue = Convert.ToInt32(dr["attribute_tolerance_value"]), toleranceOptionSelected = Convert.ToString(dr["attribute_tolerance_type"]) });
                        }
                        else
                        {
                            config.attrList.Add(new Attribute { attributeId = Convert.ToInt32(dr["attribute_id"]), matchPercentage = Convert.ToInt32(dr["match_percentage"]), matchTypeId = Convert.ToInt32(dr["match_type_id"]) });
                        }
                    }
                    return config;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return null;
        }

        private static DeDupeConfig CreateConfigObject(DataRow[] rows)
        {
            try
            {
                if (rows != null && rows.Length > 0)
                {
                    DeDupeConfig config = new DeDupeConfig();

                    for (int i = 0; i < rows.Length; i++)
                    {
                        DataRow dr = rows[i];
                        if (i == 0)
                        {
                            config.sectypeId = Convert.ToString(dr["sectype_id"]).Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                            config.moduleId = Convert.ToInt32(dr["module_id"]);
                            config.matchConfidence = Convert.ToInt32(dr["match_confidence"]);
                            config.configName = Convert.ToString(dr["config_name"]);
                            config.attrList = new List<Attribute>();
                        }
                        if (!String.IsNullOrEmpty(Convert.ToString(dr["attribute_tolerance_type"])) && Convert.ToInt32(dr["attribute_tolerance_value"]) != 0)
                        {
                            config.attrList.Add(new Attribute { attributeId = Convert.ToInt32(dr["attribute_id"]), matchPercentage = Convert.ToInt32(dr["match_percentage"]), matchTypeId = Convert.ToInt32(dr["match_type_id"]), toleranceValue = Convert.ToInt32(dr["attribute_tolerance_value"]), toleranceOptionSelected = Convert.ToString(dr["attribute_tolerance_type"]) });
                        }
                        else
                        {
                            config.attrList.Add(new Attribute { attributeId = Convert.ToInt32(dr["attribute_id"]), matchPercentage = Convert.ToInt32(dr["match_percentage"]), matchTypeId = Convert.ToInt32(dr["match_type_id"]) });
                        }
                    }
                    return config;
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return null;
        }

        internal static DeDupeConfig GetConfig(int dupes_master_id)
        {
            try
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT cm.dupes_master_id, cm.module_id, cm.sectype_id, cm.match_confidence, cm.config_name, cd.details_id, cd.match_type_id, cd.match_percentage, cd.attribute_tolerance_value,cd.attribute_tolerance_type FROM IVPSecMaster.dbo.ivp_secm_dupes_config_master cm INNER JOIN IVPSecMaster.dbo.ivp_secm_dupes_config_details cd ON cm.dupes_master_id = cd.dupes_master_id WHERE cm.dupes_master_id = " + dupes_master_id, ConnectionConstants.SecMaster_Connection);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataRow[] drArr = new DataRow[ds.Tables[0].Rows.Count];
                    ds.Tables[0].Rows.CopyTo(drArr, 0);
                    return CreateConfigObject(drArr);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return null;
        }

        internal static string SaveConfig(DeDupeConfig config)
        {

            try
            {

                Dictionary<int, DeDupeConfig> filtersList = GetFiltersList();

                //check for duplicate entries in saved filter list

                foreach (KeyValuePair<int, DeDupeConfig> entry in filtersList)
                {
                    if (config.configName.ToLower() == "")
                    {
                        throw new Exception("Please enter a valid preset name");
                    }
                    else if (config.configName.ToLower() == entry.Value.configName.ToLower())
                    {
                        throw new Exception("Preset name already exists");
                    }
                }


                CommonDALWrapper.ExecuteQuery("INSERT INTO IVPSecMaster.dbo.ivp_secm_dupes_config_master VALUES(" + config.moduleId + ",'" + String.Join(",", config.sectypeId) + "'," + config.matchConfidence + ",'" + config.configName + "')", CommonQueryType.Insert, ConnectionConstants.SecMaster_Connection);

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT IDENT_CURRENT( 'IVPSecMaster.dbo.ivp_secm_dupes_config_master' )", ConnectionConstants.SecMaster_Connection);
                int insertedRow = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                foreach (Attribute row in config.attrList)
                {
                    CommonDALWrapper.ExecuteQuery("INSERT INTO IVPSecMaster.dbo.ivp_secm_dupes_config_details VALUES(" + row.attributeId + "," + row.matchTypeId + "," + row.matchPercentage + ", " + insertedRow + ",'" + row.toleranceValue + "'," + Convert.ToInt32(row.toleranceOptionSelected) + ")", CommonQueryType.Insert, ConnectionConstants.SecMaster_Connection);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "success";
        }

        internal static string SaveRMPreset(DeDupeConfig config)
        {


            try
            {

                Dictionary<int, DeDupeConfig> filtersList = GetRMFiltersList();

                //check for duplicate entries in saved filter list


                foreach (KeyValuePair<int, DeDupeConfig> entry in filtersList)
                {
                    if (config.configName.ToLower() == "")
                    {
                        throw new Exception("Please enter a valid preset name");
                    }
                    else if (config.configName.ToLower() == entry.Value.configName.ToLower())
                    {
                        throw new Exception("Preset name already exists");
                    }

                }



                CommonDALWrapper.ExecuteQuery("INSERT INTO IVPRefMaster.dbo.ivp_refm_dupes_config_master VALUES(" + config.moduleId + ",'" + String.Join(",", config.sectypeId) + "'," + config.matchConfidence + ",'" + config.configName + "')", CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);

                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT IDENT_CURRENT( 'IVPRefMaster.dbo.ivp_refm_dupes_config_master' )", ConnectionConstants.RefMaster_Connection);
                int insertedRow = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                foreach (Attribute row in config.attrList)
                {
                    CommonDALWrapper.ExecuteQuery("INSERT INTO IVPRefMaster.dbo.ivp_refm_dupes_config_details VALUES(" + row.attributeId + "," + row.matchTypeId + "," + row.matchPercentage + ", " + insertedRow + ",'" + row.toleranceValue + "'," + Convert.ToInt32(row.toleranceOptionSelected) + ")", CommonQueryType.Insert, ConnectionConstants.RefMaster_Connection);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "success";
        }

        public static DataTable GetAttributes()
        {
            try
            {
                StringBuilder query = new StringBuilder(@"SELECT DISTINCT ad.attribute_id, ad.attribute_name, td.display_name, st.sectype_id, ass.data_type_name, ram.reference_type_id, ram.reference_attribute_name FROM IVPSecMaster.dbo.ivp_secm_sectype_table st
                INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_details ad ON st.sectype_table_id = ad.sectype_table_id
                INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details td ON td.attribute_id = ad.attribute_id
                INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_subtype ass ON ass.attribute_subtype_id = ad.attribute_subtype_id
                LEFT JOIN IVPSecMaster.dbo.ivp_secm_reference_attribute_mapping ram ON ram.attribute_id = ad.attribute_id
                WHERE ((sectype_id = 0 AND priority BETWEEN 1 AND 3) OR (sectype_id <> 0 AND priority BETWEEN 0 AND 1))");

                return CommonDALWrapper.ExecuteSelectQuery(query.ToString(), ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static DataTable GetEntityAttributes(int entity_type_id)
        {
            try
            {
                return RMCommonUtils.GetEntityTypeAttributeInfo(entity_type_id);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static DataTable GetSecurities(HashSet<string> hshAttributes, HashSet<int> sectypeIds, string userName)
        {
            try
            {
                StringBuilder query = null;
                query = new StringBuilder(@"DECLARE @select_list VARCHAR(MAX) = '[").Append(string.Join("],[", hshAttributes)).Append("]';");
                query.Append(@"DECLARE @query VARCHAR(MAX) = 'SELECT tab.';
                SELECT @query = @query + REPLACE(@select_list,',',',tab.') + ',tab.sectype_id FROM (';
                SELECT @query = @query + 'SELECT ' + @select_list + ',' + CAST(sectype_id AS VARCHAR(MAX)) + ' AS [sectype_id] FROM ' + fully_qualified_table_name + ' UNION ALL '
                FROM IVPSecMaster.dbo.ivp_secm_sectype_table");
                if (!sectypeIds.Contains(0))
                    query.Append(" WHERE sectype_id IN (").Append(string.Join(",", sectypeIds)).Append(") AND");
                else
                    query.Append(" WHERE sectype_id IN (SELECT sectype_id FROM IVPSecMaster.dbo.SECM_GetUserSectypes('").Append(userName).Append("')) AND");
                query.Append(@" priority = 1 and sectype_id != 0; SELECT @query = SUBSTRING(@query,0,LEN(@query)-9);SELECT @query = @query + ')tab INNER JOIN IVPSecMaster.dbo.ivp_secm_sec_master sm ON (sm.sec_id = tab.sec_id AND is_active = 1)';
                EXEC(@query);");

                return CommonDALWrapper.ExecuteSelectQuery(query.ToString(), ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static DataTable GetEntities(HashSet<string> hshAttributes, int entityTypeId)
        {
            try
            {
                string columns = string.Join(",", hshAttributes);
                return RMCommonUtils.GetEntityValues(columns, entityTypeId);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static DataTable GetSectypes(string userName)
        {
            try
            {
                //Loaded Assemblies in this function as this is the first method that is called from UI
                SecMasterServiceAPIAssembly = Assembly.Load("SecMasterServiceAPI");
                SecMasterCoreAssembly = Assembly.Load("SecMasterCore");
                SecMasterUpstreamAssembly = Assembly.Load("SecMasterUpstream");
                SecMasterRefDataInterfaceLayerAssembly = Assembly.Load("SecMasterRefDataInterfaceLayer");
                SecMasterCommonsAssembly = Assembly.Load("SecMasterCommons");

                return CommonDALWrapper.ExecuteSelectQuery("SELECT sectype_id, sectype_name FROM IVPSecMaster.dbo.SECM_GetUserSectypes('" + userName + "');", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static Dictionary<int, DeDupeConfig> GetFiltersList()
        {
            try
            {
                Dictionary<int, DeDupeConfig> filterResponse = new Dictionary<int, DeDupeConfig>();
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT cm.dupes_master_id, cm.config_name, cm.module_id, cm.sectype_id, cm.match_confidence, cd.attribute_id, cd.match_percentage, cd.match_type_id, cd.attribute_tolerance_type, cd.attribute_tolerance_value  FROM [dbo].[ivp_secm_dupes_config_master] cm INNER JOIN [dbo].[ivp_secm_dupes_config_details] cd ON cm.dupes_master_id = cd.dupes_master_id INNER JOIN [dbo].[ivp_secm_dupes_match_type] mt ON mt.match_type_id = cd.match_type_id ", ConnectionConstants.SecMaster_Connection);
                Dictionary<int, DataRow[]> dictMasterVsRows = ds.Tables[0].AsEnumerable().GroupBy(z => Convert.ToInt32(z["dupes_master_id"])).ToDictionary(x => x.Key, y => y.ToArray());

                foreach (KeyValuePair<int, DataRow[]> kvp in dictMasterVsRows)
                {
                    DeDupeConfig configObj = CreateConfigObject(kvp.Value);
                    filterResponse.Add(kvp.Key, configObj);
                }

                return filterResponse;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static Dictionary<int, DeDupeConfig> GetRMFiltersList(string userName = "admin", int ModuleId = 0)
        {
            try
            {
                Dictionary<int, DeDupeConfig> filterResponse = new Dictionary<int, DeDupeConfig>();
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@" SELECT cm.dupes_master_id, cm.config_name, cm.module_id, cm.entity_type_id, cm.match_confidence, cd.attribute_id, cd.match_percentage, cd.match_type_id, cd.attribute_tolerance_type, cd.attribute_tolerance_value  FROM IVPRefMaster.[dbo].[ivp_refm_dupes_config_master] cm 
                     INNER JOIN IVPRefMaster.dbo.REFM_GetUserEntityTypes('" + userName + @"'," + ModuleId + @") US 
                     ON US.entity_type_id = cm.entity_type_id 
                    INNER JOIN IVPRefMaster.[dbo].[ivp_refm_dupes_config_details] cd ON cm.dupes_master_id = cd.dupes_master_id INNER JOIN IVPRefMaster.[dbo].[ivp_refm_dupes_match_type] mt ON mt.match_type_id = cd.match_type_id ", ConnectionConstants.RefMaster_Connection);
                Dictionary<int, DataRow[]> dictMasterVsRows = ds.Tables[0].AsEnumerable().GroupBy(z => Convert.ToInt32(z["dupes_master_id"])).ToDictionary(x => x.Key, y => y.ToArray());

                foreach (KeyValuePair<int, DataRow[]> kvp in dictMasterVsRows)
                {
                    DeDupeConfig configObj = CreateRMConfigObject(kvp.Value);
                    filterResponse.Add(kvp.Key, configObj);
                }

                return filterResponse;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        internal static Dictionary<string, Dictionary<string, string>> GetSecurityAttributeValues(List<string> sectypeNames, List<string> secIds, string username)
        {
            Dictionary<string, Dictionary<string, string>> securityAttrData = new Dictionary<string, Dictionary<string, string>>();
            return securityAttrData;
            //Type ObjTypeSecMasterService = SecMasterServiceAPIAssembly.GetType("com.ivp.secm.api.SecMasterService");
            //Type ObjTypeAttributeRequestInfo = SecMasterServiceAPIAssembly.GetType("com.ivp.secm.api.Info.AttributeRequestInfo");
            //Type ObjTypeSecurityDetailsRequestInfo = SecMasterServiceAPIAssembly.GetType("com.ivp.secm.api.Info.SecurityDetailsRequestInfo");
            //Type ObjTypeSecurityDetailsResponseInfo = SecMasterServiceAPIAssembly.GetType("com.ivp.secm.api.Info.SecurityDetailsResponseInfo");

            //var serviceObj = Activator.CreateInstance(ObjTypeSecMasterService); //new SecMasterService();
            //var attrRequestInfoObj = Activator.CreateInstance(ObjTypeAttributeRequestInfo);//new AttributeRequestInfo();
            //var propInfo = ObjTypeAttributeRequestInfo.GetProperty("SecurityTypes");
            //propInfo.SetValue(attrRequestInfoObj, sectypeNames, null);
            //propInfo = ObjTypeAttributeRequestInfo.GetProperty("UserName");
            //propInfo.SetValue(attrRequestInfoObj, username, null);
            //propInfo = ObjTypeAttributeRequestInfo.GetProperty("RequirePrimaryAttributeOnly");
            //propInfo.SetValue(attrRequestInfoObj, false, null);
            ////attrRequestInfoObj.SecurityTypes = sectypeNames;
            ////attrRequestInfoObj.UserName = username;
            ////attrRequestInfoObj.RequirePrimaryAttributeOnly = false;
            //MethodInfo getAttributes = ObjTypeSecMasterService.GetMethod("GetAttributes");
            //var attrList = getAttributes.Invoke(serviceObj, new object[] { attrRequestInfoObj }); //serviceObj.GetAttributes(attrRequestInfoObj);

            //var securityDetailsRequestInfoObj = Activator.CreateInstance(ObjTypeSecurityDetailsRequestInfo);
            ////SecurityDetailsRequestInfo secDetailsRequestInfo = new SecurityDetailsRequestInfo();
            //propInfo = ObjTypeSecurityDetailsRequestInfo.GetProperty("SecurityTypes");
            //propInfo.SetValue(securityDetailsRequestInfoObj, sectypeNames, null);
            //propInfo = ObjTypeSecurityDetailsRequestInfo.GetProperty("UserName");
            //propInfo.SetValue(securityDetailsRequestInfoObj, username, null);
            //propInfo = ObjTypeSecurityDetailsRequestInfo.GetProperty("RequiredAttributes");
            //propInfo.SetValue(securityDetailsRequestInfoObj, attrList.MasterAttributes.Where(x => !x.Name.Contains(":")).Select(x => x.Name).ToList(), null);
            //propInfo = ObjTypeSecurityDetailsRequestInfo.GetProperty("Legs");
            //propInfo.SetValue(securityDetailsRequestInfoObj, new List<string>(), null);
            //propInfo = ObjTypeSecurityDetailsRequestInfo.GetProperty("DateTimeFormat");
            //propInfo.SetValue(securityDetailsRequestInfoObj, "MM/dd/yyyy HH:mm", null);
            //propInfo = ObjTypeSecurityDetailsRequestInfo.GetProperty("DateFormat");
            //propInfo.SetValue(securityDetailsRequestInfoObj, "MM/dd/yyyy", null);
            //propInfo = ObjTypeSecurityDetailsRequestInfo.GetProperty("TimeFormat");
            //propInfo.SetValue(securityDetailsRequestInfoObj, "hh:mmhh:mm:ss", null);
            //propInfo = ObjTypeSecurityDetailsRequestInfo.GetProperty("PrimaryAttribute");
            //propInfo.SetValue(securityDetailsRequestInfoObj, "security id", null);
            //propInfo = ObjTypeSecurityDetailsRequestInfo.GetProperty("PrimaryAttributeValues");
            //propInfo.SetValue(securityDetailsRequestInfoObj, secIds, null);
            //propInfo = ObjTypeSecurityDetailsRequestInfo.GetProperty("UnderlyingAttribute");
            //propInfo.SetValue(securityDetailsRequestInfoObj, "security id", null);
            ////secDetailsRequestInfo.SecurityTypes = sectypeNames;
            ////secDetailsRequestInfo.UserName = username;
            ////secDetailsRequestInfo.RequiredAttributes = attrList.MasterAttributes.Where(x => !x.Name.Contains(":")).Select(x => x.Name).ToList();
            ////secDetailsRequestInfo.Legs = new List<string>();
            ////secDetailsRequestInfo.DateTimeFormat = "MM/dd/yyyy HH:mm";
            ////secDetailsRequestInfo.DateFormat = "MM/dd/yyyy";
            ////secDetailsRequestInfo.TimeFormat = "hh:mmhh:mm:ss";
            ////secDetailsRequestInfo.PrimaryAttribute = "security id";
            ////secDetailsRequestInfo.PrimaryAttributeValues = secIds;
            ////secDetailsRequestInfo.UnderlyingAttribute = "security id";

            ////secDetailsRequestInfo.SecurityTypes

            //MethodInfo getSecurityDetails = ObjTypeSecMasterService.GetMethod("GetSecurityDetails").MakeGenericMethod(ObjTypeSecurityDetailsResponseInfo);
            //var secDetails = getSecurityDetails.Invoke(serviceObj, new object[] { securityDetailsRequestInfoObj }); //serviceObj.GetSecurityDetails(secDetailsRequestInfo);
            //var castedList = ((IEnumerable)secDetails).Cast<object>().ToList();
            //var masterAttr = ((IEnumerable)item.GetType().GetProperty("MasterAttributes")).Cast<object>().ToList();

            //Dictionary<string, Dictionary<string, string>> responseDict = new Dictionary<string, Dictionary<string, string>>();
            //foreach (var item in castedList)
            //{
            //    string primaryAttrValue = Convert.ToString(item.GetType().GetProperty("PrimaryAttributeValue").GetValue(castedList, null));
            //    if (!responseDict.ContainsKey(primaryAttrValue))
            //    {
            //        responseDict.Add(primaryAttrValue, new Dictionary<string, string>());
            //    }
            //    else
            //    {
            //        Dictionary<string, string> masterAttr = new Dictionary<string, string>();
            //        var mAttr = ((IEnumerable)item.GetType().GetProperty("MasterAttributes")).Cast<object>().ToList();
            //        foreach (var attr in mAttr)
            //        {
            //            string name = Convert.ToString(attr.GetType().GetProperty("Name").GetValue(mAttr, null));
            //            string value = Convert.ToString(attr.GetType().GetProperty("Value").GetValue(mAttr, null));
            //            if (name != "Security Id") {
            //                masterAttr.Add(name, value);
            //            }
            //        }
            //        responseDict[primaryAttrValue] = masterAttr;
            //    }
            //}

            //return castedList.GroupBy(x => x.GetType().GetProperty("PrimaryAttributeValue").GetValue(castedList, null)).ToDictionary(x => x.Key, y => y.First().MasterAttributes.Where(x => !x.Name.Equals("Security Id")).OrderBy(x => x.Name).ToDictionary(x => x.Name, x => x.Value));
        }

        internal static MergeResponse GetSecurityData(string[] secIds)
        {
            Type ObjTypeDeDupeExtension = SecMasterCommonsAssembly.GetType("com.ivp.secm.commons.SMDeDupeExtension");
            var deDupeExtensionObj = Activator.CreateInstance(ObjTypeDeDupeExtension);
            MethodInfo getSecurityData = ObjTypeDeDupeExtension.GetMethod("GetSecurityData", BindingFlags.Public | BindingFlags.Static);
            return (MergeResponse)getSecurityData.Invoke(deDupeExtensionObj, new object[] { secIds });
        }

        internal static MergeResponse GetDupeEntityData(string[] secIds)
        {
            DataSet dsDupeData = GetDupeEntityDetails(secIds);

            DataTable dtData = dsDupeData.Tables[0];

            Dictionary<string, List<MergeAttributeInfo>> dictEntityCodeVsAttributeInfo = null;
            Dictionary<int, List<string>> dictentityTypeVsEntityCodes = null;
            List<string> columns = null;
            List<int> entity_type_ids = null;
            MergeResponse response = null;
            MergeAttributeInfo MergedInfo = null;
            List<MergeAttributeInfo> lstMergedinfo = new List<MergeAttributeInfo>();


            if (dtData != null && dtData.Rows.Count > 0)
            {
                dictEntityCodeVsAttributeInfo = new Dictionary<string, List<MergeAttributeInfo>>();
                dictentityTypeVsEntityCodes = new Dictionary<int, List<string>>();
                columns = new List<string>();
                entity_type_ids = new List<int>();
                response = new MergeResponse();

                foreach (DataRow dr in dtData.Rows)
                {
                    int entity_type = Convert.ToInt32(dr["entity_type_id"]);
                    string entity_code = Convert.ToString(dr["entity_code"]);
                    bool isHighlighted = Convert.ToInt32(dr["row_id"]) == 1 ? true : false;
                    string value = Convert.ToString(dr["value"]);

                    List<MergeAttributeInfo> lstinfo = new List<MergeAttributeInfo>();
                    MergeAttributeInfo info = new MergeAttributeInfo();


                    if (dictEntityCodeVsAttributeInfo.Keys.Contains(entity_code))
                    {
                        lstinfo = dictEntityCodeVsAttributeInfo[entity_code];
                        info.IsHighlighted = isHighlighted;
                        info.Value = value;
                        lstinfo.Add(info);
                        dictEntityCodeVsAttributeInfo[entity_code] = lstinfo;
                    }
                    else
                    {
                        info.IsHighlighted = isHighlighted;
                        info.Value = value;
                        lstinfo.Add(info);
                        dictEntityCodeVsAttributeInfo.Add(entity_code, lstinfo);
                    }

                    List<string> lstCodes = new List<string>();

                    if (dictentityTypeVsEntityCodes.Keys.Contains(entity_type))
                    {
                        lstCodes = dictentityTypeVsEntityCodes[entity_type];
                        if (!lstCodes.Contains(entity_code))
                            lstCodes.Add(entity_code);
                        dictentityTypeVsEntityCodes[entity_type] = lstCodes;
                    }

                    else
                    {
                        if (!lstCodes.Contains(entity_code))
                            lstCodes.Add(entity_code);
                        dictentityTypeVsEntityCodes.Add(entity_type, lstCodes);
                    }

                }

                //Start Adding Merged Values
                List<string> lstMergedValues = dtData.AsEnumerable().Where(x => Convert.ToInt32(x["row_id"]) == 1).Select(y => Convert.ToString(y["value"])).ToList();
                foreach (string str in lstMergedValues)
                {
                    MergedInfo = new MergeAttributeInfo();
                    MergedInfo.IsHighlighted = false;
                    MergedInfo.Value = str;
                    lstMergedinfo.Add(MergedInfo);
                }

                dictEntityCodeVsAttributeInfo.Add("Merged", lstMergedinfo);
                //End Adding Merged Values


                HashSet<string> cols = new HashSet<string>(dtData.AsEnumerable().Select(x => x["attribute_name"].ToString()));
                columns = cols.ToList();
                columns.ToList();

                HashSet<int> ids = new HashSet<int>(dtData.AsEnumerable().Select(x => Convert.ToInt32(x["entity_type_id"])));
                entity_type_ids = ids.ToList();
                entity_type_ids.Sort();

                response.data = dictEntityCodeVsAttributeInfo;
                response.columns = columns;
                response.sectypeIds = entity_type_ids;
                response.sectypeVsSecid = dictentityTypeVsEntityCodes;

            }

            return response;
        }

        private static DataSet GetDupeEntityDetails(string[] entityCodes)
        {
            RHashlist mlist = new RHashlist();
            string entity_codes = string.Join(",", entityCodes);

            mlist.Add("entity_codes", entity_codes);

            DataSet dsResult = (DataSet)RMDalWrapper.ExecuteProcedure(RMQueryConstantDeDuplication.REFM_GET_DUPE_ATTRIBUTE_INFO, mlist, RMDBConnectionEnum.RefMDBConnectionId)["DataSet"];

            return dsResult;
        }

        internal static string GenerateDataForCreateEntity(bool isCreate, int sectypeId, string secId, Dictionary<string, Dictionary<string, string>> attributeData, Dictionary<string, string> legNameVsSecId, List<string> deleteSecurities, bool copyTS)
        {
            string unique = Guid.NewGuid().ToString();

            return unique;
        }

        internal static string GenerateDataForCreateUpdate(bool isCreate, int sectypeId, string secId, Dictionary<string, Dictionary<string, string>> attributeData, Dictionary<string, string> legNameVsSecId, List<string> deleteSecurities, bool copyTS)
        {
            try
            {
                Type ObjTypeDeDupeExtension = SecMasterCommonsAssembly.GetType("com.ivp.secm.commons.SMDeDupeExtension");
                var deDupeExtensionObj = Activator.CreateInstance(ObjTypeDeDupeExtension);
                MethodInfo generateDataForCreateUpdate = ObjTypeDeDupeExtension.GetMethod("GenerateDataForCreateUpdate", BindingFlags.Public | BindingFlags.Static);
                return (string)generateDataForCreateUpdate.Invoke(deDupeExtensionObj, new object[] { isCreate, sectypeId, secId, attributeData, legNameVsSecId, deleteSecurities, copyTS });
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        internal static Dictionary<string, List<DeDupeListItem>> GetToleranceOptions()
        {
            Dictionary<string, List<DeDupeListItem>> toleranceOptionsList = new Dictionary<string, List<DeDupeListItem>>();
            //DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT attr_datatype_name, COUNT(attr_datatype_name) AS 'attr_datatype_count' FROM IVPSecMaster.dbo.ivp_secm_dupes_attribute_tolerance_type GROUP BY attr_datatype_name;  SELECT attr_datatype_name, attribute_type_id, attribute_type_name FROM IVPSecMaster.dbo.ivp_secm_dupes_attribute_tolerance_type", ConnectionConstants.SecMaster_Connection);
            DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT attr_datatype_name, COUNT(attr_datatype_name) AS 'attr_datatype_count' FROM IVPRefMaster.dbo.ivp_refm_dupes_attribute_tolerance_type GROUP BY attr_datatype_name;  SELECT attr_datatype_name, attribute_type_id, attribute_type_name FROM IVPRefMaster.dbo.ivp_refm_dupes_attribute_tolerance_type", ConnectionConstants.RefMaster_Connection);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                List<DeDupeListItem> tempList = new List<DeDupeListItem>();
                string attrDataType = Convert.ToString(row["attr_datatype_name"]);
                foreach (DataRow dr in ds.Tables[1].AsEnumerable().Where(x => x.Field<string>("attr_datatype_name") == attrDataType))
                {
                    string txt = Convert.ToString(dr["attribute_type_name"]);
                    string val = Convert.ToString(dr["attribute_type_id"]);
                    tempList.Add(new DeDupeListItem { text = txt, value = val });
                }
                toleranceOptionsList.Add(attrDataType, tempList);
            }
            return toleranceOptionsList;
        }

        internal static void CopyTimeSeriesAndDeleteSecuritiesCallback(string unique, string secId)
        {
            Type ObjTypeDeDupeExtension = SecMasterCommonsAssembly.GetType("com.ivp.secm.commons.SMDeDupeExtension");
            var deDupeExtensionObj = Activator.CreateInstance(ObjTypeDeDupeExtension);
            MethodInfo copyTimeSeriesAndDeleteSecuritiesCallback = ObjTypeDeDupeExtension.GetMethod("CopyTimeSeriesAndDeleteSecuritiesCallback", BindingFlags.Public | BindingFlags.Static);
            copyTimeSeriesAndDeleteSecuritiesCallback.Invoke(deDupeExtensionObj, new object[] { unique, secId });
        }

        internal static Dictionary<int, string> GetLegNames(int sectypeId)
        {
            Dictionary<int, string> legNamesList = new Dictionary<int, string>();
            DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT sectype_table_id, table_display_name FROM [ivpsecmaster].[dbo].[ivp_secm_sectype_table] (NOLOCK) WHERE priority != 1 AND sectype_id = " + sectypeId + ";", ConnectionConstants.SecMaster_Connection);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                legNamesList.Add(Convert.ToInt32(item["sectype_table_id"]), Convert.ToString(item["table_display_name"]));
            }
            return legNamesList;
        }

        internal static Dictionary<int, string> GetEntityTypeLegNames(int sectypeId)
        {
            Dictionary<int, string> legNamesList = new Dictionary<int, string>();
            DataSet ds = CommonDALWrapper.ExecuteSelectQuery("SELECT entity_type_id, entity_display_name FROM IVPRefMaster.dbo.ivp_refm_entity_type WHERE derived_from_entity_type_id = " + sectypeId + " AND is_active = 1 AND structure_type_id = 3 ;", ConnectionConstants.RefMaster_Connection);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                legNamesList.Add(Convert.ToInt32(item["entity_type_id"]), Convert.ToString(item["entity_display_name"]));
            }
            return legNamesList;
        }

        internal static Dictionary<string, List<KeyValuePair<int, string>>> GetOverrideAttributesForSectypes(string secIds)
        {
            Dictionary<string, List<KeyValuePair<int, string>>> responseList = new Dictionary<string, List<KeyValuePair<int, string>>>();
            try
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("select sec_id, attribute_id, attribute_value from IVPSecMaster.dbo.ivp_secm_overridden_attributes where sec_id IN ('" + secIds + "') order by sec_id", ConnectionConstants.SecMaster_Connection);
                responseList = ds.Tables[0].AsEnumerable().GroupBy(x => x.Field<string>("sec_id")).ToDictionary(x => x.Key, y => y.Select(p => new KeyValuePair<int, string>(p.Field<int>("attribute_id"), p.Field<string>("attribute_value"))).ToList());
            }
            catch (Exception ex)
            {
            }
            return responseList;
        }
    }

    internal class SortInfo
    {
        public HashSet<string> hshSecIds { get; set; }
        public string Key { get; set; }
    }

    public class DeDupeAttributeInfo
    {
        //DeDupeAttributeInfo();

        public int AttributeId { get; set; }
        public string Datatype { get; set; }
        public string DisplayName { get; set; }
        public string RealName { get; set; }
        public string ReferenceAttributeName { get; set; }
        public int ReferenceTypeId { get; set; }
    }

    public static class DuplicationInfo
    {
        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> AttributeData { get; set; }
        public static Dictionary<string, bool> CopyTS { get; set; }
        public static Dictionary<string, DataSet> CreateUpdateInput { get; set; }
        public static Dictionary<string, List<string>> DeleteSecurities { get; set; }
        public static Dictionary<string, bool> IsCreate { get; set; }
        public static Dictionary<string, Dictionary<string, string>> LegVsSecId { get; set; }
        public static Dictionary<string, Dictionary<string, DeDupeAttributeInfo>> MasterDisplayNameVsAttributeInfo { get; set; }
        public static Dictionary<string, Dictionary<string, Dictionary<string, KeyValuePair<int, string>>>> MultiInfoLegAttributes { get; set; }
        public static Dictionary<string, int> SecTypeId { get; set; }
        public static Dictionary<string, Dictionary<string, Dictionary<string, KeyValuePair<int, string>>>> SingleInfoAttributes { get; set; }
        public static Dictionary<string, string> UserName { get; set; }
        public static Dictionary<string, Dictionary<string, List<KeyValuePair<int, string>>>> OverrideAttributesInfo { get; set; } //<unique, <sec_id, <attribute_id, attribute_value>>>
    }

    [DataContract]
    public class MergeAttributeInfo
    {
        [DataMember]
        public bool IsHighlighted { get; set; }
        [DataMember]
        public string Value { get; set; }
        public int ReferenceTypeId { get; set; }
        public string ReferenceAttributeName { get; set; }
    }

    [DataContract]
    public class MergeResponse
    {
        [DataMember]
        public Dictionary<string, List<MergeAttributeInfo>> data { get; set; }
        [DataMember]
        public List<string> columns { get; set; }
        [DataMember]
        public List<int> sectypeIds { get; set; }
        [DataMember]
        public Dictionary<int, List<string>> sectypeVsSecid { get; set; }
    }
}
