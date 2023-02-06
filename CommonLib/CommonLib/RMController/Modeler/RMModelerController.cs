using com.ivp.common;
using com.ivp.common.Migration;
using com.ivp.common.SecMaster;
using com.ivp.rad.data;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.ivp.common
{
    public class RMModelerController
    {
        public XElement CreateXmlInfoFromAttributeInfo(List<RMEntityAttributeInfo> lstAttributesInfo, int entityTypeID)
        {
            try
            {
                XElement sbAtInfo = new XElement("root", from item in lstAttributesInfo.Where(x => x != null)
                                                         select new XElement("pair", new XElement("eai", Convert.ToString(item.EntityAttributeID)),
                                                                new XElement("eti", entityTypeID),
                                                                new XElement("an", item.AttributeName),
                                                                new XElement("dn", item.DisplayName),
                                                                new XElement("isn", item.IsNullable),
                                                                new XElement("dt", Convert.ToString(item.DataType)),
                                                                //new XElement("isu", item.IsPrimary ? true : item.IsUnique),
                                                                new XElement("dtl", string.IsNullOrEmpty(item.DataTypeLength) ? DBNull.Value.ToString() : item.DataTypeLength),
                                                                new XElement("dv", item.DefaultValue == null ? string.Empty : item.DefaultValue),
                                                                new XElement("isv", item.IsSearchView),
                                                                new XElement("lmb", item.LastModifiedBy),
                                                                new XElement("pa", item.IsPrimary),
                                                                new XElement("isc", item.IsClonable),
                                                                new XElement("ise", item.IsEncrypted),
                                                                new XElement("ispii", item.IsPII),
                                                                new XElement("dimeta", item.DisplayMetaData),
                                                                new XElement("svp", item.SearchViewPosition > 0 ? item.SearchViewPosition.Value.ToString() : DBNull.Value.ToString()),
                                                                new XElement("lt", Convert.ToInt16(item.IsInternal).ToString()),
                                                                new XElement("pet", (!string.IsNullOrEmpty(Convert.ToString(item.LookupEntityTypeID)) && item.LookupEntityTypeID > 0) ? item.LookupEntityTypeID.ToString() : null),
                                                                new XElement("pat", (!string.IsNullOrEmpty(Convert.ToString(item.LookupAttributeID))) ? item.LookupAttributeID.ToString() : null),
                                                                new XElement("tags", item.Tags),
                                                                new XElement("restricted_chars", !string.IsNullOrEmpty(item.RestrictedChars) ? item.RestrictedChars : null),
                                                                new XElement("attribute_description", !string.IsNullOrEmpty(item.AttributeDescription) ? item.AttributeDescription : null),
                                                                new XElement("lookup_display_attributes", !string.IsNullOrEmpty(item.LookupDisplayAttributes) ? item.LookupDisplayAttributes : null)
                                                                )
                                                            );

                return sbAtInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public XElement CreateLookupXmlInfoFromAttributeInfo(List<RMEntityAttributeInfo> lstAttributesInfo, string entityTypeName, Dictionary<string, RMEntityDetailsInfo> entityDetailsInfo)
        {
            try
            {
                XElement sbAtInfo = new XElement("root", from item in lstAttributesInfo.Where(x => x != null)
                                                         select new XElement("pair", new XElement("eai", Convert.ToString(entityDetailsInfo[entityTypeName].Attributes[item.DisplayName].EntityAttributeID)),
                                                                new XElement("eti", entityDetailsInfo[entityTypeName].EntityTypeID),
                                                                new XElement("etn", entityDetailsInfo[entityTypeName].EntityTypeName),
                                                                new XElement("an", item.AttributeName),
                                                                new XElement("pet", entityDetailsInfo[entityDetailsInfo[entityTypeName].Attributes[item.DisplayName].LookupEntityTypeName].EntityTypeID),
                                                                new XElement("pat", entityDetailsInfo[entityDetailsInfo[entityTypeName].Attributes[item.DisplayName].LookupEntityTypeName].Attributes[entityDetailsInfo[entityTypeName].Attributes[item.DisplayName].LookupAttributeName].EntityAttributeID),
                                                                new XElement("petn", entityDetailsInfo[entityDetailsInfo[entityTypeName].Attributes[item.DisplayName].LookupEntityTypeName].EntityTypeName),
                                                                new XElement("patn", entityDetailsInfo[entityDetailsInfo[entityTypeName].Attributes[item.DisplayName].LookupEntityTypeName].Attributes[entityDetailsInfo[entityTypeName].Attributes[item.DisplayName].LookupAttributeName].AttributeName)
                                                                )
                                                            );

                return sbAtInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<string, RMEntityDetailsInfo> GetEntityTypeDetails(int moduleId, List<int> entityType = null, ObjectSet dbEntityTypeData = null, bool requireTemplateDetails = true)
        {
            Dictionary<string, RMEntityDetailsInfo> entityDetails = new Dictionary<string, RMEntityDetailsInfo>(StringComparer.OrdinalIgnoreCase);
            ObjectSet osEntityDetails = null;
            if (dbEntityTypeData == null)
                osEntityDetails = GetModelerConfigurationDetails(moduleId, entityType, true, !requireTemplateDetails);
            else
                osEntityDetails = dbEntityTypeData;

            ObjectTable otEtDetails = osEntityDetails.Tables[RMModelerConstants.TABLE_DEFINITION];
            ObjectTable otAttrDetails = osEntityDetails.Tables[RMModelerConstants.TABLE_MASTER_ATTIRBUTES];
            ObjectTable legDetails = osEntityDetails.Tables[RMModelerConstants.TABLE_LEGS_CONFIGURATION];
            ObjectTable legAttrDetails = osEntityDetails.Tables[RMModelerConstants.TABLE_LEG_ATTRIBUTES];

            otEtDetails.AsEnumerable().ToList().ForEach(row => {
                if (!entityDetails.ContainsKey(Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME])))
                    entityDetails.Add(Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]), new RMEntityDetailsInfo()
                    {
                        EntityDisplayName = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]),
                        EntityTypeID = Convert.ToInt32(row[RMModelerConstants.ENTITY_TYPE_ID]),
                        EntityTypeName = Convert.ToString(row[RMModelerConstants.ENTITY_TYPE_NAME]),
                        Legs = new Dictionary<string, RMEntityDetailsInfo>(StringComparer.OrdinalIgnoreCase),
                        Attributes = new Dictionary<string, RMEntityAttributeInfo>(StringComparer.OrdinalIgnoreCase)
                    });
            });

            otAttrDetails.AsEnumerable().ToList().ForEach(row => {
                if (entityDetails.ContainsKey(Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME])))
                {
                    string dataType = Convert.ToString(row[RMModelerConstants.Data_Type]);
                    RMDBDataTypes type = GetDataType(dataType);

                    entityDetails[Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME])].Attributes.Add(Convert.ToString(row[RMModelerConstants.Attribute_Name]), new RMEntityAttributeInfo()
                    {
                        DisplayName = Convert.ToString(row[RMModelerConstants.Attribute_Name]),
                        EntityAttributeID = Convert.ToInt32(row[RMModelerConstants.ENTITY_ATTRIBUTE_ID]),
                        AttributeName = Convert.ToString(row[RMModelerConstants.Attribute_Real_Name]),
                        LookupAttributeName = Convert.ToString(row[RMModelerConstants.Lookup_Attribute]),
                        LookupEntityTypeName = Convert.ToString(row[RMModelerConstants.Lookup_Type]),
                        LookupAttributeID = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Lookup_Attribute_Id])) ? Convert.ToInt32(row[RMModelerConstants.Lookup_Attribute_Id]) : -1,
                        LookupEntityTypeID = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Lookup_Type_Id])) ? Convert.ToInt32(row[RMModelerConstants.Lookup_Type_Id]) : -1,
                        IsNullable = !Convert.ToBoolean(row[RMModelerConstants.Is_Mandatory]),
                        AttributeLookupIdentityColumn = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Attribute_lookup_identity])) ? Convert.ToInt32(row[RMModelerConstants.Attribute_lookup_identity]) : -1,
                        DataType = type
                    });
                }
            });

            legDetails.AsEnumerable().ToList().ForEach(row =>
            {
                if (entityDetails.ContainsKey(Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME])))
                {
                    entityDetails[Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME])].Legs.Add(Convert.ToString(row[RMModelerConstants.Leg_Name]),
                        new RMEntityDetailsInfo() { Attributes = new Dictionary<string, RMEntityAttributeInfo>(StringComparer.OrdinalIgnoreCase), EntityDisplayName = Convert.ToString(row[RMModelerConstants.Leg_Name]), EntityTypeID = Convert.ToInt32(row[RMModelerConstants.LEG_ENTITY_TYPE_ID]), EntityTypeName = Convert.ToString(row[RMModelerConstants.LEG_REAL_NAME]) });
                }
            });

            legAttrDetails.AsEnumerable().ToList().ForEach(row => {
                if (entityDetails.ContainsKey(Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME])))
                {
                    string dataType = Convert.ToString(row[RMModelerConstants.Data_Type]);
                    RMDBDataTypes type = GetDataType(dataType);

                    entityDetails[Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME])].Legs[Convert.ToString(row[RMModelerConstants.Leg_Name])].Attributes.Add(Convert.ToString(row[RMModelerConstants.Attribute_Name]), new RMEntityAttributeInfo()
                    {

                        DisplayName = Convert.ToString(row[RMModelerConstants.Attribute_Name]),
                        EntityAttributeID = Convert.ToInt32(row[RMModelerConstants.ENTITY_ATTRIBUTE_ID]),
                        AttributeName = Convert.ToString(row[RMModelerConstants.Attribute_Real_Name]),
                        LookupAttributeName = Convert.ToString(row[RMModelerConstants.Lookup_Attribute]),
                        LookupEntityTypeName = Convert.ToString(row[RMModelerConstants.Lookup_Type]),
                        LookupAttributeID = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Lookup_Attribute_Id])) ? Convert.ToInt32(row[RMModelerConstants.Lookup_Attribute_Id]) : -1,
                        LookupEntityTypeID = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Lookup_Type_Id])) ? Convert.ToInt32(row[RMModelerConstants.Lookup_Type_Id]) : -1,
                        IsNullable = !Convert.ToBoolean(row[RMModelerConstants.Is_Mandatory]),
                        AttributeLookupIdentityColumn = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Attribute_lookup_identity])) ? Convert.ToInt32(row[RMModelerConstants.Attribute_lookup_identity]) : -1,
                        DataType = type,
                        IsPrimary = Convert.ToBoolean(row[RMModelerConstants.Is_Primary])
                    });
                }
            });

            return entityDetails;
        }

        private RMDBDataTypes GetDataType(string dataType)
        {
            RMDBDataTypes type = RMDBDataTypes.VARCHAR;
            switch (dataType.ToUpper())
            {
                case RMAttributeSetupConstants.DATA_TYPE_VARCHAR:
                    type = RMDBDataTypes.VARCHAR;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_VARCHAR_MAX:
                    type = RMDBDataTypes.VARCHARMAX;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_INT:
                    type = RMDBDataTypes.INT;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_DATETIME:
                    type = RMDBDataTypes.DATETIME;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_BIT:
                    type = RMDBDataTypes.BIT;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_LOOKUP:
                    type = RMDBDataTypes.LOOKUP;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_SECURITYLOOKUP:
                case RMAttributeSetupConstants.DATA_TYPE_SECURITY_LOOKUP:
                    type = RMDBDataTypes.SECURITY_LOOKUP;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_FILE:
                    type = RMDBDataTypes.FILE;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_DECIMAL:
                    type = RMDBDataTypes.DECIMAL;
                    break;
            }

            return type;
        }

        public Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> GetTemplateDetails(ObjectSet dbData)
        {
            Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo = new Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>>();
            Dictionary<string, RMEntityTypeTemplateDetails> templateInfo = new Dictionary<string, RMEntityTypeTemplateDetails>();
            //Dictionary<string, int> tabInfo = new Dictionary<string, int>();
            Dictionary<string, RMEntityTabDetails> tabInfo = new Dictionary<string, RMEntityTabDetails>();
            List<int> templateIds = new List<int>();

            ObjectTable otTab = dbData.Tables[RMModelerConstants.TABLE_TAB_MANAGEMENT];
            templateIds = otTab.AsEnumerable().Select(x => Convert.ToInt32(x[RMModelerConstants.TEMPLATE_ID])).ToList<int>();
            ObjectTable allTabs = new RMModelerDBManager().GetAllTabsForTempate(string.Join(",", templateIds));

            allTabs.AsEnumerable().ToList().ForEach(row =>
            {
                string entityTypeName = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]);
                string layoutName = Convert.ToString(row[RMModelerConstants.Layout_Name]);
                int layoutId = Convert.ToInt32(row[RMModelerConstants.TEMPLATE_ID]);
                string tabName = Convert.ToString(row[RMModelerConstants.Tab_Name]);
                int tabId = Convert.ToInt32(row[RMModelerConstants.TAB_NAME_ID]);
                int tabOrder = Convert.ToInt32(row[RMModelerConstants.Tab_Order]);

                if (!entityTypeVstemplateInfo.ContainsKey(entityTypeName))
                {
                    templateInfo = new Dictionary<string, RMEntityTypeTemplateDetails>();
                    entityTypeVstemplateInfo.Add(entityTypeName, templateInfo);
                }

                else
                    templateInfo = entityTypeVstemplateInfo[entityTypeName];

                if (!templateInfo.ContainsKey(layoutName))
                {
                    //tabInfo = new Dictionary<string, int>();
                    tabInfo = new Dictionary<string, RMEntityTabDetails>();
                    templateInfo.Add(layoutName, new RMEntityTypeTemplateDetails()
                    {
                        TemplateName = layoutName,
                        TemplateId = layoutId,
                        TabNameVsId = tabInfo
                    });

                }
                else
                    tabInfo = templateInfo[layoutName].TabNameVsId;

                if (!tabInfo.ContainsKey(tabName))
                {
                    tabInfo.Add(tabName, new RMEntityTabDetails()
                    {
                        tabName = tabName,
                        tabNameId = tabId,
                        tabOrder = tabOrder
                    });
                }
            });

            //TAB_ID
            return entityTypeVstemplateInfo;
        }

        public ObjectSet GetModelerConfigurationDetails(int moduleId, List<int> entityType = null, bool is_sync = false, bool runSelected = false)
        {
            ObjectSet dsExcel = new RMModelerDBManager().GetModelerConfiguration(moduleId, entityType, runSelected);

            var objRUser = new rad.RUserManagement.RUserManagementService();
            Dictionary<string, string> hshUsers = SRMCommonRAD.GetAllUsersLoginNamevsDisplayName();
            //objRUser.GetAllUsersGDPR().ToDictionary(y => y.UserLoginName, x => x.FullName + '(' + x.UserName + ')', StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> hshGroups = objRUser.GetAllGroupsGDPR().ToDictionary(x => x.GroupName, y => y.GroupName);
            List<string> invalidTemplates = new List<string>();

            foreach (ObjectRow row in dsExcel.Tables[0].AsEnumerable().ToList())
            {

                List<string> lstUserNames = new List<string>();
                List<string> lstGroups = new List<string>();
                string userName = Convert.ToString(row["Allowed Users"]);
                string groups = Convert.ToString(row["Allowed Groups"]);
                if (!string.IsNullOrEmpty(userName))
                {
                    lstUserNames = userName.Split(',').ToList();
                    List<string> massageusers = new List<string>();
                    lstUserNames.ForEach(z =>
                    {
                        if (hshUsers.ContainsKey(z.Trim()))
                            massageusers.Add(hshUsers[z.Trim()]);
                    });

                    if (massageusers.Count > 0)
                        row["Allowed Users"] = string.Join(",", massageusers);
                    else
                        row["Allowed Users"] = string.Empty;
                }
                if (!string.IsNullOrEmpty(groups))
                {
                    lstGroups = groups.Split(',').ToList();
                    List<string> massagegroups = new List<string>();
                    lstGroups.ForEach(z =>
                    {
                        if (hshGroups.ContainsKey(z.Trim()))
                            massagegroups.Add(hshGroups[z.Trim()]);
                    });

                    if (massagegroups.Count > 0)
                        row["Allowed Groups"] = string.Join(",", massagegroups);
                    else
                        row["Allowed Groups"] = string.Empty;
                }

            }

            foreach (ObjectRow row in dsExcel.Tables[1].AsEnumerable().Where(r => !string.IsNullOrEmpty(Convert.ToString(r["Attribute Length"])) && Convert.ToString(r["Data Type"]).Equals("DECIMAL")).ToList())
            {
                var attrLength = Convert.ToString(row["Attribute Length"]).Split(',');
                var firstDigit = Convert.ToInt32(attrLength[0]) - Convert.ToInt32(attrLength[1]);
                var actualLength = Convert.ToString(firstDigit) + '.' + Convert.ToString(attrLength[1]);
                row["Attribute Length"] = actualLength;
            }

            foreach (ObjectRow row in dsExcel.Tables[1].AsEnumerable().Where(r => !string.IsNullOrEmpty(Convert.ToString(r["Default Value"])) && Convert.ToString(r["Data Type"]).Equals("DATETIME")).ToList())
            {
                //dateFormat = "MM/dd/yyyy";
                string actualDate = string.Empty;
                var DBDateValue = Convert.ToString(row["Default Value"]);
                string year = DBDateValue.Substring(0, 4);
                string month = DBDateValue.Substring(4, 2);
                string day = DBDateValue.Substring(6, 2);
                actualDate = month + '/' + day + '/' + year;
                row["Default Value"] = actualDate;
            }

            foreach (ObjectRow row in dsExcel.Tables[3].AsEnumerable().Where(r => !string.IsNullOrEmpty(Convert.ToString(r["Attribute Length"])) && Convert.ToString(r["Data Type"]).Equals("DECIMAL")).ToList())
            {
                var attrLength = Convert.ToString(row["Attribute Length"]).Split(',');
                var firstDigit = Convert.ToInt32(attrLength[0]) - Convert.ToInt32(attrLength[1]);
                var actualLength = Convert.ToString(firstDigit) + '.' + Convert.ToString(attrLength[1]);
                row["Attribute Length"] = actualLength;
            }

            foreach (ObjectRow row in dsExcel.Tables[3].AsEnumerable().Where(r => !string.IsNullOrEmpty(Convert.ToString(r["Default Value"])) && Convert.ToString(r["Data Type"]).Equals("DATETIME")).ToList())
            {
                //dateFormat = "MM/dd/yyyy";
                string actualDate = string.Empty;
                var DBDateValue = Convert.ToString(row["Default Value"]);
                string year = DBDateValue.Substring(0, 4);
                string month = DBDateValue.Substring(4, 2);
                string day = DBDateValue.Substring(6, 2);
                actualDate = month + '/' + day + '/' + year;
                row["Default Value"] = actualDate;
            }
            SRMSecTypeMassageInputInfo inputInfo = new SRMSecTypeMassageInputInfo();
            inputInfo.SecTypeIDColumn = "security_id";
            inputInfo.SecTypeAttributeIDColumn = "security_attribute_id";
            inputInfo.SecTypeDisplayNameColumn = "Lookup Type";
            inputInfo.SecTypeAttributeDisplayNameColumn = "Lookup Attribute";

            new RMSectypeController().MassageSecTypeAndAttributes(dsExcel.Tables[1].AsEnumerable().Where(r => Convert.ToString(r["Data Type"]).Equals("SECURITY_LOOKUP")), inputInfo);
            new RMSectypeController().MassageSecTypeAndAttributes(dsExcel.Tables[3].AsEnumerable().Where(r => Convert.ToString(r["Data Type"]).Equals("SECURITY_LOOKUP")), inputInfo);

            inputInfo = new SRMSecTypeMassageInputInfo();
            inputInfo.SecTypeIDColumn = "security_id";
            inputInfo.SecTypeAttributeIDColumn = "lookup_display_attributes";
            inputInfo.SecTypeDisplayNameColumn = "Lookup Type";
            inputInfo.SecTypeAttributeDisplayNameColumn = "Lookup Display Attributes";

            new RMSectypeController().MassageSecTypeAndAttributes(dsExcel.Tables[1].AsEnumerable().Where(r => !string.IsNullOrEmpty(Convert.ToString(r["lookup_display_attributes"])) && Convert.ToString(r["Data Type"]).Equals("SECURITY_LOOKUP")), inputInfo);
            new RMSectypeController().MassageSecTypeAndAttributes(dsExcel.Tables[3].AsEnumerable().Where(r => !string.IsNullOrEmpty(Convert.ToString(r["lookup_display_attributes"])) && Convert.ToString(r["Data Type"]).Equals("SECURITY_LOOKUP")), inputInfo);

            dsExcel.Tables[0].TableName = RMModelerConstants.TABLE_DEFINITION;
            dsExcel.Tables[1].TableName = RMModelerConstants.TABLE_MASTER_ATTIRBUTES;
            dsExcel.Tables[2].TableName = RMModelerConstants.TABLE_LEGS_CONFIGURATION;
            dsExcel.Tables[3].TableName = RMModelerConstants.TABLE_LEG_ATTRIBUTES;

            if (!runSelected)
            {

                foreach (ObjectRow dr in dsExcel.Tables[7].AsEnumerable())
                {
                    var username = Convert.ToString(dr["Dependent Name"]);
                    if (Convert.ToString(dr["is_user"]).Equals("1"))
                    {
                        if (hshUsers.ContainsKey(username))
                        {
                            dr["Dependent Name"] = hshUsers[username];
                        }
                        else
                        {
                            if (!invalidTemplates.Contains(Convert.ToString(dr["Layout Name"])))
                                invalidTemplates.Add(Convert.ToString(dr["Layout Name"]));
                        }

                    }
                    else if (Convert.ToString(dr["is_user"]).Equals("0"))
                    {
                        if (hshGroups.ContainsKey(username))
                        {
                            dr["Dependent Name"] = hshGroups[username];
                        }
                        else
                        {
                            if (!invalidTemplates.Contains(Convert.ToString(dr["Layout Name"])))
                                invalidTemplates.Add(Convert.ToString(dr["Layout Name"]));
                        }
                    }
                }
                dsExcel.Tables[7].Columns.Remove(dsExcel.Tables[7].Columns["is_user"]);

                foreach (ObjectRow dr in dsExcel.Tables[8].AsEnumerable())
                {
                    string userName = Convert.ToString(dr["User Name"]);
                    if (!string.IsNullOrEmpty(userName))
                    {
                        if (hshUsers.ContainsKey(userName))
                            dr["User Name"] = hshUsers[userName];
                        else
                        {
                            if (!invalidTemplates.Contains(Convert.ToString(dr["Group Level Layout"])))
                                invalidTemplates.Add(Convert.ToString(dr["Group Level Layout"]));
                        }
                    }
                }

                dsExcel.Tables[4].TableName = RMModelerConstants.TABLE_UNIQUE_KEYS;
                dsExcel.Tables[5].TableName = RMModelerConstants.TABLE_ATTRIBUTE_RULES;
                dsExcel.Tables[6].TableName = RMModelerConstants.TABLE_BASKET_RULES;
                dsExcel.Tables[7].TableName = RMModelerConstants.TABLE_LAYOUTS;
                dsExcel.Tables[8].TableName = RMModelerConstants.TABLE_USER_GROUP_LAYOUT_PRIORITY;
                dsExcel.Tables[9].TableName = RMModelerConstants.TABLE_TAB_MANAGEMENT;
                dsExcel.Tables[10].TableName = RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT;
                dsExcel.Tables[11].TableName = RMModelerConstants.TABLE_LEG_ORDER;
                dsExcel.Tables[12].TableName = RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION;
                dsExcel.Tables[13].TableName = RMModelerConstants.TABLE_EXCEPTION_PREFERENCES;
                dsExcel.Tables[14].TableName = RMModelerConstants.TABLE_ACTION_NOTIFICATIONS;


                foreach (ObjectTable table in dsExcel.Tables.AsEnumerable().Where(r => r.TableName.Equals(RMModelerConstants.TABLE_LAYOUTS) || r.TableName.Equals(RMModelerConstants.TABLE_TAB_MANAGEMENT) || r.TableName.Equals(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT) || r.TableName.Equals(RMModelerConstants.TABLE_LEG_ORDER)))
                {
                    List<ObjectRow> rowsToDelete = new List<ObjectRow>();
                    foreach (var row in table.AsEnumerable())
                    {
                        if (invalidTemplates.Contains(row[RMModelerConstants.Layout_Name]))
                            rowsToDelete.Add(row);
                    }

                    foreach (ObjectRow delRow in rowsToDelete)
                        table.Rows.Remove(delRow);
                }
            }

            //CASE : method called for downloading modeler configuration
            if (!is_sync)
            {
                //removing columns not needed in final downloaded excel.
                List<string> colsToRemove = new List<string>()
                {   RMModelerConstants.ENTITY_TYPE_ID,
                    RMModelerConstants.ENTITY_TYPE_NAME,
                    RMModelerConstants.ENTITY_ATTRIBUTE_ID,
                    RMModelerConstants.Attribute_Real_Name,
                    RMModelerConstants.SECURITY_ID,
                    RMModelerConstants.SECURITY_ATTRIBUTE_ID,
                    RMModelerConstants.LOOKUP_DISP_ATTR,
                    RMModelerConstants.Lookup_Type_Id,
                    RMModelerConstants.Lookup_Attribute_Id,
                    RMModelerConstants.Lookup_Attribute_Real_Name,
                    RMModelerConstants.Attribute_lookup_identity,
                    RMModelerConstants.MASTER_ENTITY_TYPE_ID,
                    RMModelerConstants.LEG_ENTITY_TYPE_ID,
                    RMModelerConstants.LEG_REAL_NAME,
                    RMModelerConstants.UNIQUE_KEY_ID,
                    RMModelerConstants.TEMPLATE_ID,
                    RMModelerConstants.TAB_NAME_ID,
                    RMModelerConstants.Rule_ID,
                    RMModelerConstants.CONFIG_MASTER_ID,
                    RMModelerConstants.Order_By_Attribute_Id,
                    RMModelerConstants.Order_By_Attribute_Name
                };

                for (int i = 0; i < dsExcel.Tables.Count; i++)
                    new RMCommonController().RemoveColumnsFromObjectTable(dsExcel.Tables[i], colsToRemove);
            }

            return dsExcel;
        }
    }
}
