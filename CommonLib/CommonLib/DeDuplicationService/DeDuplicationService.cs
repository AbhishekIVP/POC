using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections;
using System.Data;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web.SessionState;
using com.ivp.commom.commondal;
using com.ivp.rad.utils;

namespace com.ivp.common
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DeDuplicationService : RServiceBaseClass, IDeDuplicationService
    {
        public List<DeDupeListItem> GetAllSectypes(string userName)
        {
            return DeDupeController.GetAllSectypes(userName);
        }

        public List<DeDupeListItem> GetAllEntityTypes(string userName, int ModuleId = 0)
        {
            return DeDupeController.GetAllEntityTypes(userName, ModuleId);
        }

        public string[] GetEntityTypeAttributesForDeDupe(string entityTypeId)
        {
            return DeDupeController.GetEntityTypeAttributesForDeDupe(Convert.ToInt32(entityTypeId));
        }

        public string SavePreset(DeDupeConfig configValues)
        {
            return DeDupeController.SaveConfig(configValues);
        }

        public string SaveRMPreset(DeDupeConfig configValues)
        {
            return DeDupeController.SaveRMPreset(configValues);
        }

        public DeDupeConfig GetPreset(int dupes_master_id)
        {
            return DeDupeController.GetConfig(dupes_master_id);
        }

        public List<DeDupeListItem> GetDeDuplicateModuleList()
        {
            return DeDupeController.GetDeDuplicateModuleList();
        }

        public List<DeDupeListItem> GetDeDuplicateMatchTypeList()
        {
            return DeDupeController.GetDeDuplicateMatchTypeList();
        }

        public List<DeDuplicationResponseData> FindDupesData(int configId, DeDupeConfig configDetails, string userName)
        {
            List<DeDuplicationResponseData> responseDataList = new List<DeDuplicationResponseData>();

            DeDupeWrapper wrapperObject = null;
            if (configId != 0)
                wrapperObject = new DeDupeWrapper(configId, userName, 3);
            else
                wrapperObject = new DeDupeWrapper(configDetails, userName, 3);

            if (wrapperObject != null)
            {
                Dictionary<string, Dictionary<string, Dictionary<string, MergeAttributeInfo>>> result = wrapperObject.GetDuplicates();
                foreach (var row in result)
                {
                    string percent = row.Key.Split('_')[0];
                    DeDuplicationResponseData tempResponseObj = new DeDuplicationResponseData();
                    tempResponseObj.title = Convert.ToString(percent);
                    List<string> columnArray = row.Value.First().Value.Select(x => x.Key).ToList<string>();
                    tempResponseObj.colHeaders = columnArray;
                    tempResponseObj.checkboxRequired = true;
                    tempResponseObj.mergeSecurities = row.Value;
                    responseDataList.Add(tempResponseObj);
                }
            }
            return responseDataList;
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, MergeAttributeInfo>>> FindDupesData(string userName, int moduleId, int typeId, string secId, Dictionary<string, string> attributeNameVsValue, out int matchConfidenceConfigurationValue)
        {
            matchConfidenceConfigurationValue = 0;
            List<DeDuplicationResponseData> responseDataList = new List<DeDuplicationResponseData>();
            DeDupeWrapper wrapperObject = new DeDupeWrapper(userName, moduleId, typeId);

            if (wrapperObject != null)
            {
                if (attributeNameVsValue == null || attributeNameVsValue.Count == 0)
                {
                    matchConfidenceConfigurationValue = wrapperObject.MatchConfidence;

                    var dt = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DECLARE @secId VARCHAR(max) = '{0}',
		                                                            @sectypeId INT = 0,
		                                                            @tableName VARCHAR(max) = '',
		                                                            @sql VARCHAR(max) = ''

                                                            Select @sectypeId = sectype_id FROM IVPSecMaster.dbo.ivp_secm_sectype_master WHERE sectype_short_name = SUBSTRING(@secId, 1, 4)

                                                            Select @tableName = REPLACE(fully_qualified_table_name, '].[Sectype_', '].[Staging_Sectype_') FROM IVPSecMaster.dbo.ivp_secm_sectype_table WHERE sectype_id = @sectypeId AND [priority] = 1

                                                            SET @sql = 'SELECT * FROM ' + @tableName + 'WHERE sec_id = ''' + @secId + ''' '

                                                            EXEC(@sql)", secId), ConnectionConstants.SecMaster_Connection).Tables[0];
                    attributeNameVsValue = new Dictionary<string, string>();
                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        foreach (DataColumn col in dt.Columns)
                            attributeNameVsValue[col.ColumnName] = Convert.ToString(row[col.ColumnName]);
                    }
                }
                Dictionary<string, Dictionary<string, Dictionary<string, MergeAttributeInfo>>> result = wrapperObject.GetDuplicates(secId, attributeNameVsValue);
                return result;
            }
            return null;
        }

        public List<DeDuplicationResponseData> FindEntityDupesData(int configId, DeDupeConfig configDetails, string userName)
        {
            List<DeDuplicationResponseData> responseDataList = new List<DeDuplicationResponseData>();
            DataSet ds = null;

            DeDupeWrapper wrapperObject = null;
            if (configId != 0)
                wrapperObject = new DeDupeWrapper(configId, userName, 6);
            else
                wrapperObject = new DeDupeWrapper(configDetails, userName, 6);

            if (wrapperObject != null)
            {
                Dictionary<string, Dictionary<string, Dictionary<string, MergeAttributeInfo>>> result = wrapperObject.GetEntityDuplicates();
                foreach (var row in result)
                {
                    string titleString = string.Empty;
                    string percent = row.Key.Split('_')[0];
                    DeDuplicationResponseData tempResponseObj = new DeDuplicationResponseData();
                    tempResponseObj.title = Convert.ToString(percent);
                    List<string> columnArray = row.Value.First().Value.Select(x => x.Key).ToList<string>();
                    tempResponseObj.colHeaders = columnArray;
                    tempResponseObj.checkboxRequired = true;
                    tempResponseObj.mergeSecurities = row.Value;

                    if (tempResponseObj.mergeSecurities.Keys.Count > 0)
                    {
                        List<string> lstCodes = tempResponseObj.mergeSecurities.Keys.ToList<string>();
                        string codes = string.Join(",", lstCodes);

                        if (ds == null)
                        {
                            ds = CommonDALWrapper.ExecuteSelectQuery("EXEC REFM_GetDeDupeDisplayAttribute '', '" + codes + "'   ", ConnectionConstants.RefMaster_Connection);
                        }

                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            titleString = string.Join(", ", ds.Tables[0].AsEnumerable().Where(r => lstCodes.Contains(Convert.ToString(r["entity_code"])) && !string.IsNullOrEmpty(Convert.ToString(r["value"]))).Select(d => d["value"].ToString()).ToList());
                        }
                    }

                    tempResponseObj.title += titleString;

                    if (configDetails != null && configDetails.entityCodes != null && configDetails.entityCodes.Count > 0)
                    {
                        if (configDetails.entityCodes.Contains(tempResponseObj.mergeSecurities.Keys.FirstOrDefault()))
                            responseDataList.Add(tempResponseObj);
                    }
                    else
                        responseDataList.Add(tempResponseObj);
                }
            }

            return responseDataList;
        }

        public bool CheckEntityHasDependency(List<string> entityCodes)
        {
            return DeDupeController.CheckEntityHasDependency(entityCodes);
        }

        public Dictionary<int, DeDupeConfig> GetDeDuplicateFilterList()
        {
            return DeDupeController.GetFiltersList();
        }

        public Dictionary<int, DeDupeConfig> GetRMDeDuplicateFilterList(string userName, int ModuleId = 0)
        {
            return DeDupeController.GetRMFiltersList(userName, ModuleId);
        }

        public Dictionary<string, Dictionary<string, string>> GetSecurityAttributeValues(List<string> sectypeNames, List<string> secIds, string username)
        {
            return DeDupeController.GetSecurityAttributeValues(sectypeNames, secIds, username);
        }

        public MergeResponse GetSecurityData(string[] secIds)
        {
            return DeDupeController.GetSecurityData(secIds);
        }

        public MergeResponse GetDupeEntityData(string[] secIds)
        {
            return DeDupeController.GetDupeEntityData(secIds);
        }

        public string MergeSecurities(bool isCreate, int sectypeId, string secId, Dictionary<string, Dictionary<string, string>> attributeData, Dictionary<string, string> legNameVsSecId, List<string> deleteSecurities, bool copyTS)
        {
            return DeDupeController.GenerateDataForCreateUpdate(isCreate, sectypeId, secId, attributeData, legNameVsSecId, deleteSecurities, copyTS);
        }

        public Dictionary<string, List<DeDupeListItem>> GetToleranceOptions()
        {
            return DeDupeController.GetToleranceOptions();
        }

        public void CopyTimeSeriesAndDeleteSecuritiesCallback(string unique, string secId)
        {
            DeDupeController.CopyTimeSeriesAndDeleteSecuritiesCallback(unique, secId);
        }

        public Dictionary<int, string> GetLegNames(string sectypeId)
        {
            return DeDupeController.GetLegNames(Convert.ToInt32(sectypeId));
        }

        public Dictionary<int, string> GetEntityTypeLegNames(string sectypeId)
        {
            return DeDupeController.GetEntityTypeLegNames(Convert.ToInt32(sectypeId));
        }

        public string SetMergedEntityDetails(bool isCreate, int sectypeId, string secId, Dictionary<string, Dictionary<string, string>> attributeData, Dictionary<string, string> legNameVsSecId, List<string> deleteSecurities, bool copyTS, int deletionOption)
        {
            MergedEntityInfo info = new MergedEntityInfo();
            string sessionIdentifier = Guid.NewGuid().ToString();
            info.entityTypeID = sectypeId;
            info.entityCode = secId;
            info.AttributeData = attributeData;
            info.LegVsEntityCode = legNameVsSecId;
            info.DeleteEntities = deleteSecurities;
            info.CopyTS = copyTS;
            info.deletionOption = deletionOption;

            System.Web.HttpContext.Current.Session[sessionIdentifier] = info;
            return sessionIdentifier;
        }

        public string[] GetAttributeBasedOnSecTypeSelectionForDeDupe(string secTypeIds, string userName)
        {
            try
            {
                string[] sectypeArr = secTypeIds.Split(',');
                DataSet ds = null;
                if (sectypeArr.Length > 1 || (sectypeArr.Length == 1 && sectypeArr[0] == "-1"))
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT attr.attribute_id, attr.attribute_name, display_name, asub.data_type_name FROM [ivpsecmaster].[dbo].[ivp_secm_attribute_details] AS attr (NOLOCK) 
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_sectype_table] AS tbl (NOLOCK)
                                                                ON tbl.sectype_table_id = attr.sectype_table_id 
                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_subtype] asub (NOLOCK)
                                                                ON asub.attribute_subtype_id = attr.attribute_subtype_id
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_details] AS tem (NOLOCK) 
                                                                ON (attr.attribute_id = tem.attribute_id) 
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_master] AS mas (NOLOCK) 
                                                                ON (tem.template_id = mas.template_id AND mas.template_id = 0)
                                                                WHERE attr.attribute_subtype_id != 7 AND attr.is_active = 'TRUE' AND tem.is_active = 'TRUE' ORDER BY tem.display_name", ConnectionConstants.SecMaster_Connection);
                }
                else if (sectypeArr.Length == 1)
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT attr.attribute_id, attr.attribute_name, display_name, asub.data_type_name FROM [ivpsecmaster].[dbo].[ivp_secm_attribute_details] AS attr (NOLOCK) 
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_sectype_table] AS tbl (NOLOCK)
                                                                ON tbl.sectype_table_id = attr.sectype_table_id 
                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_subtype] asub (NOLOCK)
                                                                ON asub.attribute_subtype_id = attr.attribute_subtype_id
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_details] AS tem (NOLOCK) 
                                                                ON (attr.attribute_id = tem.attribute_id) 
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_master] AS mas (NOLOCK) 
                                                                ON (tem.template_id = mas.template_id AND mas.template_id = 0)
                                                                WHERE attr.attribute_subtype_id != 7 AND attr.is_active = 'TRUE' AND tem.is_active = 'TRUE'
                                                                UNION
                                                                SELECT DISTINCT ad.attribute_id, ad.attribute_name, td.display_name, asub.data_type_name FROM [IVPSecMaster].[dbo].[ivp_secm_attribute_details] ad
                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_sectype_table] st
                                                                ON st.sectype_table_id = ad.sectype_table_id
                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_subtype] asub
                                                                ON asub.attribute_subtype_id = ad.attribute_subtype_id
                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_template_details] td
                                                                ON td.attribute_id = ad.attribute_id 
                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_template_master] tm
                                                                ON tm.template_id = td.template_id
                                                                WHERE ad.attribute_subtype_id != 7 AND st.sectype_id IN ('" + secTypeIds + @"') AND st.[priority] > 0
                                                                ORDER BY tem.display_name ", ConnectionConstants.SecMaster_Connection);
                }
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].AsEnumerable().OrderBy(x => x["display_name"]).Select(x => x.Field<int>("attribute_id") + "&&" + x.Field<string>("data_type_name") + "|" + x.Field<string>("display_name")).ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception Ex)
            {
                return null;
            }
        }

        [Serializable]
        public class MergedEntityInfo
        {
            public int entityTypeID { get; set; }
            public string entityCode { get; set; }
            public Dictionary<string, Dictionary<string, string>> AttributeData { get; set; }
            public Dictionary<string, string> LegVsEntityCode { get; set; }
            public List<string> DeleteEntities { get; set; }
            public bool CopyTS { get; set; }
            public string uniqueid { get; set; }
            public int deletionOption { get; set; }
        }

    }
}
