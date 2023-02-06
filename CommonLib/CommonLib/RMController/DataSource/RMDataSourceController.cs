using com.ivp.commom.commondal;
using com.ivp.common.SecMaster;
using com.ivp.rad.common;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.utils;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace com.ivp.common
{
    public class RMDataSourceControllerNew
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMController");

        public ObjectSet GetDataSourceConfiguration(List<int> feeds, int moduleID, bool getRawData, bool getAllFeeds, RDBConnectionManager mDBCon = null)
        {
            try
            {
                mLogger.Debug("RMController -> DownloadDataSourceConfiguration -> Start");
                ObjectSet oSet = new RMDataSourceDBManager(mDBCon).GetDataSourceConfiguration(feeds, moduleID, getRawData, getAllFeeds);

                if (oSet != null && oSet.Tables.Count > 0)
                {
                    oSet.Tables[0].TableName = RMDataSourceConstants.TABLE_DATA_SOURCES;
                    oSet.Tables[1].TableName = RMDataSourceConstants.TABLE_FEED_FIELDS;
                    oSet.Tables[2].TableName = RMDataSourceConstants.TABLE_FEED_MAPPING;
                    oSet.Tables[3].TableName = RMDataSourceConstants.TABLE_FEED_RULES;
                    oSet.Tables[4].TableName = RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_MAPPING;
                    oSet.Tables[5].TableName = RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_RULES;
                    oSet.Tables[6].TableName = RMDataSourceConstants.TABLE_BULK_LICENSE_SETUP;
                    oSet.Tables[7].TableName = RMDataSourceConstants.TABLE_CUSTOM_CLASSES;
                    oSet.Tables[8].TableName = RMDataSourceConstants.TABLE_API_LICENSE_SETUP;
                    oSet.Tables[9].TableName = RMDataSourceConstants.TABLE_FTP_LICENSE_SETUP;

                    MassageSecurityLookupsForDataSourceConfig(oSet.Tables[4]);
                    SetLoadingTaskDateTypes(oSet.Tables[6], new List<string>() { "Bulk File Date Type", "Difference File Date Type" });
                    ParseFTPTaskInfo(oSet.Tables[8], "vendor_details", "control", RMLicenseTaskType.Import);
                    ParseFTPTaskInfo(oSet.Tables[9], "request_vendor_details", "control", RMLicenseTaskType.Request);
                    ParseFTPTaskInfo(oSet.Tables[9], "response_vendor_details", "control", RMLicenseTaskType.Response);

                    if (!getRawData)
                    {
                        RemoveUnNecessaryColumns(oSet);
                    }
                }

                return oSet;
            }
            catch
            {
                mLogger.Debug("RMController -> DownloadDataSourceConfiguration -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMController -> DownloadDataSourceConfiguration -> End");
            }
        }

        public Dictionary<int, string> GetAllDataSources(RDBConnectionManager mDBCon = null)
        {
            mLogger.Debug("RMController -> GetAllDataSources -> Start");
            try
            {
                Dictionary<int, string> dictDS = new Dictionary<int, string>();
                ObjectTable table = new RMDataSourceDBManager(mDBCon).GetAllDataSources();

                if (table != null && table.Rows.Count > 0)
                {
                    table.AsEnumerable().ToList().ForEach(row =>
                    {
                        dictDS.Add(Convert.ToInt32(row["data_source_id"]), Convert.ToString(row["data_source_name"]));
                    });
                }

                return dictDS;
            }
            catch
            {
                mLogger.Debug("RMController -> GetAllDataSources -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMController -> GetAllDataSources -> End");
            }
        }

        public List<int> GetFeedRuleIDsByRuleType(int feedSummaryID, int ruleTypeID, RDBConnectionManager mDBCon = null)
        {
            try
            {
                ObjectTable table = new RMDataSourceDBManager(mDBCon).GetFeedRuleDetailsByRuleType(feedSummaryID, ruleTypeID);
                List<int> lstRuleIds = null;

                if (table != null && table.Rows.Count > 0)
                {
                    lstRuleIds = table.AsEnumerable().Select(x => Convert.ToInt32(x["rule_id"])).Distinct().ToList();
                }

                return lstRuleIds;
            }
            catch
            {
                throw;
            }
        }



        public Dictionary<string, RMFeedInfoDetailed> GetFeedDetailsPerFeed(List<int> feeds, int moduleID, bool getRawData, RDBConnectionManager mDBCon = null)
        {
            Dictionary<string, RMFeedInfoDetailed> feedInfo = new Dictionary<string, RMFeedInfoDetailed>();
            ObjectSet osFeeds = GetDataSourceConfiguration(feeds, moduleID, getRawData, true, mDBCon);

            if (osFeeds != null && osFeeds.Tables.Count > 0)
            {
                osFeeds.Tables[RMDataSourceConstants.TABLE_DATA_SOURCES].AsEnumerable()
                    .Where(r => !string.IsNullOrEmpty(Convert.ToString(r[RMDataSourceConstants.Feed_Name]).Trim()))
                    .ToList().ForEach(t =>
                    {
                        feedInfo.Add(Convert.ToString(t[RMDataSourceConstants.Feed_Name]).Trim(), new RMFeedInfoDetailed()
                        {
                            DataSourceID = Convert.ToInt32(t[RMDataSourceConstants.Data_Source_ID]),
                            FeedID = !string.IsNullOrEmpty(Convert.ToString(t[RMDataSourceConstants.FEED_ID])) ? Convert.ToInt32(t[RMDataSourceConstants.FEED_ID]) : -1,
                            FeedName = Convert.ToString(t[RMDataSourceConstants.Feed_Name]).Trim(),
                            FileTypeID = !string.IsNullOrEmpty(Convert.ToString(t[RMDataSourceConstants.FILE_TYPE_ID])) ? Convert.ToInt32(t[RMDataSourceConstants.FILE_TYPE_ID]) : -1
                        });
                    });

                osFeeds.Tables[RMDataSourceConstants.TABLE_FEED_FIELDS].AsEnumerable()
                    .Where(r => !string.IsNullOrEmpty(Convert.ToString(r[RMDataSourceConstants.Feed_Name]).Trim()))
                    .ToList().ForEach(t =>
                    {
                        string feedName = Convert.ToString(t[RMDataSourceConstants.Feed_Name]);
                        RMFeedInfoDetailed fInfo = feedInfo.Where(f => f.Key.SRMEqualWithIgnoreCase(feedName)).Select(f => f.Value).FirstOrDefault();

                        if (fInfo != null)
                        {
                            RMFeedFieldInfo fieldInfo = new RMFeedFieldInfo();
                            fieldInfo.FieldID = !string.IsNullOrEmpty(Convert.ToString(t[RMDataSourceConstants.Feed_Field_ID])) ? Convert.ToInt32(t[RMDataSourceConstants.Feed_Field_ID]) : -1;
                            fieldInfo.FieldName = Convert.ToString(t[RMDataSourceConstants.Feed_Field_Name]).Trim();
                            fieldInfo.IsPrimary = !string.IsNullOrEmpty(Convert.ToString(t[RMModelerConstants.Is_Primary])) ? Convert.ToBoolean(t[RMModelerConstants.Is_Primary]) : false;

                            fInfo.FeedFields.Add(fieldInfo);
                        }
                    });

                osFeeds.Tables[RMDataSourceConstants.TABLE_ENTITY_TYPE_FEED_MAPPING].AsEnumerable()
                    .Where(r => !string.IsNullOrEmpty(Convert.ToString(r[RMDataSourceConstants.Feed_Name]).Trim()))
                    .ToList().ForEach(t =>
                 {
                     string feedName = Convert.ToString(t[RMDataSourceConstants.Feed_Name]);

                     RMFeedInfoDetailed fInfo = feedInfo.Where(f => f.Key.SRMEqualWithIgnoreCase(feedName)).Select(f => f.Value).FirstOrDefault();

                     if (fInfo != null)
                     {
                         RMFeedEntityTypeMappingInfo mappingInfo = new RMFeedEntityTypeMappingInfo();
                         mappingInfo.AttributeName = Convert.ToString(t[RMModelerConstants.Attribute_Name]).Trim();
                         mappingInfo.FieldName = Convert.ToString(t[RMDataSourceConstants.Feed_Field_Name]).Trim();
                         mappingInfo.LookupType = Convert.ToString(t[RMModelerConstants.Lookup_Type]).Trim();
                         mappingInfo.LookupAttribute = Convert.ToString(t[RMModelerConstants.Lookup_Attribute]).Trim();

                         fInfo.FieldAttributeMappings.Add(mappingInfo);
                     }
                 });
            }
            return feedInfo;
        }

        public void GetAllFeedAndFileDetailsForMigration(Dictionary<string, int> feedTypes, Dictionary<string, int> fileTypes, Dictionary<string, List<string>> feedVsFileTypes, RDBConnectionManager mDBCon = null)
        {
            ObjectSet oSet = new RMDataSourceDBManager(mDBCon).GetAllFeedAndFileTypes();

            if (oSet != null && oSet.Tables.Count > 1)
            {
                oSet.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    string feedTypeName = RMCommonStatic.ConvertToLower(Convert.ToString(row["feed_type_name"]));
                    feedTypes.Add(feedTypeName, Convert.ToInt32(row["feed_type_id"]));

                    if (RMCommonStatic.ConvertToLower(row["feed_type_name"]).SRMEqualWithIgnoreCase("Load From DB"))
                    {
                        feedVsFileTypes.Add(feedTypeName, oSet.Tables[1].AsEnumerable().Where(r => Convert.ToInt32(r["file_type_id"]) == 4).Select(rr => RMCommonStatic.ConvertToLower(rr["file_type_name"])).Distinct().ToList());
                    }
                    else
                    {
                        feedVsFileTypes.Add(feedTypeName, oSet.Tables[1].AsEnumerable().Where(r => Convert.ToInt32(r["file_type_id"]) < 4).Select(rr => RMCommonStatic.ConvertToLower(rr["file_type_name"])).Distinct().ToList());
                    }

                });

                oSet.Tables[1].AsEnumerable().ToList().ForEach(row =>
                {
                    string fileTypeName = RMCommonStatic.ConvertToLower(Convert.ToString(row["file_type_name"]));
                    fileTypes.Add(fileTypeName, Convert.ToInt32(row["file_type_id"]));
                });

            }
        }

        public RMFeedInfoDetailed GetDetailedFeedInfoByID(int feedSummaryID, RDBConnectionManager mDBCon)
        {
            try
            {
                RMFeedInfoDetailed feedInfo = null;

                ObjectSet oSet = new RMDataSourceDBManager(mDBCon).GetFeedDetailsByFeedID(feedSummaryID);

                if (oSet != null && oSet.Tables.Count > 0 && oSet.Tables[0] != null && oSet.Tables[0].Rows.Count > 0)
                {
                    feedInfo = new RMFeedInfoDetailed();
                    feedInfo.FeedName = Convert.ToString(oSet.Tables[0].Rows[0]["feed_name"]); ;
                    feedInfo.FeedID = feedSummaryID;
                    feedInfo.DataSourceID = Convert.ToInt32(oSet.Tables[0].Rows[0]["data_source_id"]);
                    feedInfo.FileID = Convert.ToInt32(oSet.Tables[0].Rows[0]["file_id"]);
                    feedInfo.FileName = Convert.ToString(oSet.Tables[0].Rows[0]["file_name"]);

                    if (oSet.Tables[1] != null && oSet.Tables[1].Rows.Count > 0)
                    {
                        feedInfo.FeedFields = new List<RMFeedFieldInfo>();
                        foreach (ObjectRow row in oSet.Tables[1].Rows)
                        {
                            RMFeedFieldInfo ffi = new RMFeedFieldInfo();
                            ffi.FieldID = Convert.ToInt32(row["field_id"]);
                            ffi.FeedFieldDetailsID = Convert.ToInt32(row["feed_field_details_id"]);
                            ffi.FieldName = Convert.ToString(row["field_name"]);
                            feedInfo.FeedFields.Add(ffi);
                        }
                    }
                }

                return feedInfo;
            }
            catch
            {
                throw;
            }

        }

        public Dictionary<int, string> GetAllRadMappings(RDBConnectionManager mDBCon)
        {
            try
            {
                Dictionary<int, string> dictMappings = new Dictionary<int, string>();
                DataTable dt = new RMDataSourceDBManager(mDBCon).GetAllRadMappings();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        dictMappings.Add(Convert.ToInt32(row["mapping_summary_id"]), Convert.ToString(row["mapping_name"]));
                    }
                }

                return dictMappings;
            }
            catch
            {
                throw;
            }
        }

        private void RemoveUnNecessaryColumns(ObjectSet oSet)
        {
            Dictionary<int, List<string>> dictColumnsToRemove = new Dictionary<int, List<string>>();

            //Populate columns to remove
            dictColumnsToRemove.Add(0, new List<string>() { RMDataSourceConstants.Data_Source_ID, RMDataSourceConstants.FEED_ID, RMDataSourceConstants.FILE_TYPE_ID });
            dictColumnsToRemove.Add(1, new List<string>() { RMDataSourceConstants.Feed_Field_ID });
            dictColumnsToRemove.Add(2, new List<string>() { RMDataSourceConstants.Feed_Mapping_Details_ID, RMDataSourceConstants.FEED_ID, RMDataSourceConstants.FILE_ID, RMDataSourceConstants.Mapped_Column_ID });
            dictColumnsToRemove.Add(3, new List<string>() { RMModelerConstants.Rule_Type_ID, RMModelerConstants.Rule_ID });
            dictColumnsToRemove.Add(5, new List<string>() { RMModelerConstants.Rule_ID });

            foreach (KeyValuePair<int, List<string>> kvp in dictColumnsToRemove)
            {
                kvp.Value.ForEach(col =>
                {
                    if (oSet.Tables[kvp.Key].Columns.Contains(col))
                        oSet.Tables[kvp.Key].Columns.Remove(col);
                });
            }
        }

        private void MassageSecurityLookupsForDataSourceConfig(ObjectTable dt)
        {
            if (dt != null)
            {
                new RMSectypeController().MassageSecTypeAndAttributes(dt.AsEnumerable().Where(d => Convert.ToString(d["data_type"]).SRMEqualWithIgnoreCase("SECURITY_LOOKUP")), new SRMSecTypeMassageInputInfo() { SecTypeAttributeRealNameColumn = "parent_security_attribute_name", SecTypeIDColumn = "parent_security_type_id", SecTypeDisplayNameColumn = "Lookup Type", SecTypeAttributeDisplayNameColumn = "Lookup Attribute" });

                if (dt.Columns.Contains("data_type"))
                    dt.Columns.Remove("data_type");
                if (dt.Columns.Contains("parent_security_type_id"))
                    dt.Columns.Remove("parent_security_type_id");
                if (dt.Columns.Contains("parent_security_attribute_name"))
                    dt.Columns.Remove("parent_security_attribute_name");
            }
        }


        private bool SetSecurityTypeDetails(DataRow row, string secTypeName, string secAttributeName)
        {
            row["Parent Security Type"] = secTypeName;
            row["Parent Security Attribute"] = secAttributeName;
            return true;
        }

        private void ParseFTPTaskInfo(ObjectTable dt, string column, string tagName, RMLicenseTaskType taskType)
        {
            if (dt != null)
            {

                Dictionary<string, string> dictControlPropertyMapping = new Dictionary<string, string>();
                PopulateControlPropMapping(dictControlPropertyMapping, taskType);

                List<RMLicenseKeyPropertyMapping> mappings = new List<RMLicenseKeyPropertyMapping>();
                foreach (KeyValuePair<string, string> kvp in dictControlPropertyMapping)
                {
                    RMLicenseKeyPropertyMapping map = new RMLicenseKeyPropertyMapping();
                    map.KeyName = kvp.Key;
                    map.PropertyName = kvp.Value;
                    mappings.Add(map);
                }

                XmlDocument doc = new XmlDocument();
                dt.AsEnumerable().Where(r => !string.IsNullOrEmpty(Convert.ToString(r[column]))).ToList().ForEach(row =>
                {
                    doc = new XmlDocument();
                    doc.LoadXml(Convert.ToString(row[column]));
                    XmlNodeList nodes = doc.GetElementsByTagName(tagName);

                    foreach (XmlNode node in nodes)
                    {
                        XmlAttribute xmlAttr = node.Attributes["key"];
                        string value = node.Attributes["text"].Value;
                        string columnName = mappings.Where(m => m.KeyName.Trim().Equals(xmlAttr.Value.Trim(), StringComparison.OrdinalIgnoreCase)).Select(m => m.PropertyName).FirstOrDefault();

                        if (!string.IsNullOrEmpty(columnName))
                            row[columnName] = value;
                    }
                });

                dt.Columns.Remove(column);
            }
        }


        private void PopulateControlPropMapping(Dictionary<string, string> dict, RMLicenseTaskType taskType)
        {
            if (taskType == RMLicenseTaskType.Import)
            {
                dict.Add("taskname", "Import Task Name");
                dict.Add("taskdescription", "Import Task Description");
                dict.Add("apitype", "Request Type");
                dict.Add("vendor", "Vendor Type");
                dict.Add("MarketSector", "Market Sector/Asset Type");
                dict.Add("assettype", "Market Sector/Asset Type");
                dict.Add("vendoridentifier", "Vendor Identifier");
            }
            else if (taskType == RMLicenseTaskType.Request)
            {
                dict.Add("tasknamerequest", "Request Task Name");
                dict.Add("taskdescriptionrequest", "Request Task Description");
                dict.Add("transporttyperequest", "Request Transport Type");
                dict.Add("vendor", "Request Vendor Type");
                dict.Add("outgoingftprequest", "Request Outgoing FTP");
                dict.Add("vendoridentifierrequest", "Request Vendor Identifier");
                dict.Add("marketsectorrequest", "Request Market Sector/Asset Type");
                dict.Add("assettyperequest", "Request Market Sector/Asset Type");
                dict.Add("datarequesttype", "Data Request Type");
            }
            else if (taskType == RMLicenseTaskType.Response)
            {
                dict.Add("tasknameresponse", "Response Task Name");
                dict.Add("taskdescriptionresponse", "Response Task Description");
                dict.Add("transporttyperesponse", "Response Transport Type");
                dict.Add("vendor", "Response Vendor Type");
                dict.Add("incomingftpresponse", "Response Incoming FTP");
                dict.Add("vendoridentifierrequest", "Request Vendor Identifier");
                dict.Add("marketsectorrequest", "Request Market Sector/Asset Type");
                dict.Add("assettyperesponse", "Request Market Sector/Asset Type");
                //dict.Add("datarequesttype", "Response Data Request Type");
            }
        }

        private void SetLoadingTaskDateTypes(ObjectTable dt, List<string> columns)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                IDictionary<string, string> dateTypes = new Dictionary<string, string>();
                dateTypes = ConfigurationLoader("RefM_RefMasterControllerConfig")["CommonDateTypes"];
                dateTypes.Remove("100");
                dateTypes.Add("-1", "");

                columns.ForEach(col =>
                {
                    var setDateType = from tab in dt.AsEnumerable()
                                      join typ in dateTypes.AsEnumerable()
                                      on Convert.ToString(tab[col]) equals typ.Key
                                      select AssignDateType(tab, col, typ.Value);

                    setDateType.Count();
                });

                dt.AsEnumerable().Where(r => !string.IsNullOrEmpty(Convert.ToString(r[RMDataSourceConstants.Bulk_File_Date]))).ToList().ForEach(row =>
                {
                    row[RMDataSourceConstants.Bulk_File_Date] = DateTime.ParseExact(Convert.ToString(row[RMDataSourceConstants.Bulk_File_Date]), "yyyyMMdd", null).ToString("MM/dd/yyyy");
                });

                dt.AsEnumerable().Where(r => !string.IsNullOrEmpty(Convert.ToString(r[RMDataSourceConstants.Difference_File_Date]))).ToList().ForEach(row =>
                {
                    row[RMDataSourceConstants.Difference_File_Date] = DateTime.ParseExact(Convert.ToString(row[RMDataSourceConstants.Difference_File_Date]), "yyyyMMdd", null).ToString("MM/dd/yyyy");
                });
            }
        }

        private bool AssignDateType(ObjectRow row, string col, string value)
        {
            row[col] = value;
            return true;
        }

        public IDictionary<string, IDictionary<string, string>> ConfigurationLoader(string configKey)
        {
            IDictionary<string, IDictionary<string, string>> mWizardConfig =
                                              new Dictionary<string, IDictionary<string, string>>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc = RConfigurationManager.GetConfigDocument("refmasterconfig", configKey);
            XmlNodeList nodeList = xmlDoc.FirstChild.NextSibling.ChildNodes;
            mWizardConfig = IterateChildNodes(nodeList);
            return mWizardConfig;
        }

        private IDictionary<string, IDictionary<string, string>> IterateChildNodes(XmlNodeList nodeList)
        {
            IDictionary<string, IDictionary<string, string>> mWizardConfig = new Dictionary<string, IDictionary<string, string>>();
            Dictionary<string, string> dictChildren = null;
            string nodeName = null;
            foreach (XmlNode node in nodeList)
            {
                dictChildren = new Dictionary<string, string>();
                nodeName = node.Name;
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    dictChildren[childNode.Attributes[0].InnerText] = childNode.InnerText;
                }
                mWizardConfig[nodeName] = dictChildren;
            }

            return mWizardConfig;
        }


        public Dictionary<int, string> GetEntityTypeKeysForMerging(int entityTypeID)
        {
            Dictionary<int, string> dict = null;

            Assembly RefMControllerAssembly = Assembly.Load("RefMController");
            Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMEntityPrioritizationController");
            MethodInfo GetAttributes = objType.GetMethod("GetEntityTypeKeysForDSMerging");
            object obj = Activator.CreateInstance(objType);
            dict = (Dictionary<int, string>)GetAttributes.Invoke(obj, new object[] { entityTypeID });

            return dict;
        }


        public bool SaveEntityPrioritization(List<RMEntityPrioritizationInfo> list, int mainEntityTypeId, string distinctEntityTypeId, string xmlinfo, string createdBy, bool firstPriorityChkBox, bool deletePrevExceptionChkBox, bool noVendorValueFoundExceptionChkBox, bool appplyUICalculationRule, RDBConnectionManager mDBCon = null)
        {
            bool isPassed = false;

            Assembly RefMControllerAssembly = Assembly.Load("RefMController");
            Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMEntityPrioritizationController");
            MethodInfo SavePrioritization = objType.GetMethod("SaveAllDataSourcePriorityAndAttributeMap");
            object obj = Activator.CreateInstance(objType);
            isPassed = (bool)SavePrioritization.Invoke(obj, new object[] { list, mainEntityTypeId, distinctEntityTypeId, xmlinfo, createdBy, firstPriorityChkBox, deletePrevExceptionChkBox, noVendorValueFoundExceptionChkBox, appplyUICalculationRule, mDBCon });

            return isPassed;
        }
    }
}
