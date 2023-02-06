using com.ivp.rad.common;
using com.ivp.rad.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using com.ivp.srmcommon;
using com.ivp.commom.commondal;
using com.ivp.common.Migration;
using com.ivp.rad.dal;
using SRMCommonModules;

namespace com.ivp.common
{
    public class SRMVendorSettingsController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorSettingsController");
        Dictionary<string, Dictionary<string, List<string>>> dictVendorVSRequestTypeVsConfigKey = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, Dictionary<string, List<string>>> dictVendorVSRequestTypeVsHeaderType = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);
        public List<string> ValidatePreferenceName(string prefName)
        {
            List<string> errormssgs = new List<string>();

            if (prefName == null || prefName.Trim() == string.Empty)
            {
                errormssgs.Add("Preference Name cannot be left blank.");
            }

            return errormssgs;
        }

        public List<string> ValidateHeaderName(string headerName)
        {
            List<string> errormssgs = new List<string>();

            if (headerName == null || headerName.Trim() == string.Empty)
            {
                errormssgs.Add("Header Name cannot be left blank.");
            }

            return errormssgs;
        }

        public Dictionary<string, Dictionary<string, List<string>>> GetConfigurationKeyValues()
        {
            var result = new SRMVendorManagementDBManager().GetPossibleKeys();

            foreach (ObjectRow row in result.Tables[0].AsEnumerable())
            {
                var vendorName = Convert.ToString(row["vendor_name"]);
                var requestType = Convert.ToString(row["request_type_name"]);
                var keyName = Convert.ToString(row["key_name"]);

                if (!dictVendorVSRequestTypeVsConfigKey.ContainsKey(vendorName))
                    dictVendorVSRequestTypeVsConfigKey.Add(vendorName, new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase));

                if (!dictVendorVSRequestTypeVsConfigKey[vendorName].ContainsKey(requestType))
                    dictVendorVSRequestTypeVsConfigKey[vendorName].Add(requestType, new List<string>());

                if (!dictVendorVSRequestTypeVsConfigKey[vendorName][requestType].Contains(keyName))
                    dictVendorVSRequestTypeVsConfigKey[vendorName][requestType].Add(keyName);


            }

            return dictVendorVSRequestTypeVsConfigKey;
        }

        public Dictionary<string, Dictionary<string, List<string>>> GetHeaderTypeValues()
        {
            var result = new SRMVendorManagementDBManager().GetHeaderTypeValues();

            foreach (ObjectRow row in result.Tables[0].AsEnumerable())
            {
                var vendorName = Convert.ToString(row["vendor_name"]);
                var requestType = Convert.ToString(row["request_type_name"]);
                var headerType = Convert.ToString(row["header_type_name"]);

                if (!dictVendorVSRequestTypeVsHeaderType.ContainsKey(vendorName))
                    dictVendorVSRequestTypeVsHeaderType.Add(vendorName, new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase));

                if (!dictVendorVSRequestTypeVsHeaderType[vendorName].ContainsKey(requestType))
                    dictVendorVSRequestTypeVsHeaderType[vendorName].Add(requestType, new List<string>());

                if (!dictVendorVSRequestTypeVsHeaderType[vendorName][requestType].Contains(headerType))
                    dictVendorVSRequestTypeVsHeaderType[vendorName][requestType].Add(headerType);


            }

            return dictVendorVSRequestTypeVsHeaderType;
        }

        public List<string> ValidateKeys(string vendorName, string requestType, string key, Dictionary<string, Dictionary<string, List<string>>> dictVendorVSRequestTypeVsConfigKey)
        {
            List<string> errormssgs = new List<string>();

            if (!dictVendorVSRequestTypeVsConfigKey.ContainsKey(vendorName))
                errormssgs.Add("Invalid Vendor.");

            else if (!dictVendorVSRequestTypeVsConfigKey[vendorName].ContainsKey(requestType))
                errormssgs.Add("Invalid Request Type.");

            else if (!dictVendorVSRequestTypeVsConfigKey[vendorName][requestType].Contains(key))
                errormssgs.Add("Invalid Configuration Key.");

            return errormssgs;
        }

        public List<string> ValidateHeaders(string vendorName, string requestType, string headertype, Dictionary<string, Dictionary<string, List<string>>> dictVendorVSRequestTypeVsHeaderType)
        {
            List<string> errormssgs = new List<string>();

            if (!dictVendorVSRequestTypeVsHeaderType.ContainsKey(vendorName))
                errormssgs.Add("Invalid Vendor.");

            else if (!dictVendorVSRequestTypeVsHeaderType[vendorName].ContainsKey(requestType))
                errormssgs.Add("Invalid Request Type.");

            else if (!dictVendorVSRequestTypeVsHeaderType[vendorName][requestType].Contains(headertype))
                errormssgs.Add("Invalid Header Type.");

            return errormssgs;
        }

        public void SyncVendor(ObjectSet deltaSet, string username)
        {
            mLogger.Debug("SRMVendorSettingsController : SyncVendor -> Start");
            try
            {
                ObjectTable sourceTable = null;
                ObjectSet dbVendorSystemSettingsData = new SRMVendorManagement().GetVendorSystemSettingsConfiguration(null, false);

                dictVendorVSRequestTypeVsConfigKey = GetConfigurationKeyValues();
                dictVendorVSRequestTypeVsHeaderType = GetHeaderTypeValues();

                Dictionary<string, bool> PrefVsIsInsert = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

                var PreferenceNameVsIdTable = CommonDALWrapper.ExecuteSelectQueryObject("select distinct vendor_management_name,vendor_management_id from IVPRefMasterVendor.dbo.ivp_rad_vendor_management_master", ConnectionConstants.RefMasterVendor_Connection);

                Dictionary<string, int> PreferenceNameVsId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var row in PreferenceNameVsIdTable.Tables[0].AsEnumerable())
                {
                    if (!PreferenceNameVsId.ContainsKey(Convert.ToString(row["vendor_management_name"])))
                        PreferenceNameVsId.Add(Convert.ToString(row["vendor_management_name"]), Convert.ToInt32(row["vendor_management_id"]));
                }

                Dictionary<string, int> VendorNameVsId = CommonDALWrapper.ExecuteSelectQueryObject("SELECT * from IVPRefMasterVendor.dbo.ivp_rad_vendor_management_vendor_type", ConnectionConstants.RefMasterVendor_Connection).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["vendor_name"]), y => Convert.ToInt32(y["vendor_id"]));

                Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>> dictPrefVsVendorVsReqestVsConfigs = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(StringComparer.OrdinalIgnoreCase);

                Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>> dictPrefVsVendorVsReqestVsHeaders = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>>(StringComparer.OrdinalIgnoreCase);

                foreach (string pref in PreferenceNameVsId.Keys)
                {
                    if (!dictPrefVsVendorVsReqestVsConfigs.ContainsKey(pref))
                        dictPrefVsVendorVsReqestVsConfigs.Add(pref, new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase));

                    dictPrefVsVendorVsReqestVsConfigs[pref] = dictVendorVSRequestTypeVsConfigKey.ToDictionary(x => x.Key, y => y.Value.ToDictionary(s => s.Key, t => t.Value.ToDictionary(u => u, v => string.Empty),StringComparer.OrdinalIgnoreCase));

                    if (!dictPrefVsVendorVsReqestVsHeaders.ContainsKey(pref))
                        dictPrefVsVendorVsReqestVsHeaders.Add(pref, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(StringComparer.OrdinalIgnoreCase));

                    dictPrefVsVendorVsReqestVsHeaders[pref] = dictVendorVSRequestTypeVsHeaderType.ToDictionary(x => x.Key, y => y.Value.ToDictionary(s => s.Key, t => t.Value.ToDictionary(u => u, v => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))));

                }

                foreach (var table in dbVendorSystemSettingsData.Tables)
                {
                    if (table.TableName.SRMEqualWithIgnoreCase(SRMVendorManagementConstants.TABLE_CONFIGURATIONS))
                    {
                        foreach (ObjectRow row in table.AsEnumerable())
                        {
                            string preferenceName = Convert.ToString(row[SRMVendorManagementConstants.PREFERENCE_NAME]);
                            string vendorName = Convert.ToString(row[SRMVendorManagementConstants.VENDOR_NAME]);
                            string requestType = Convert.ToString(row[SRMVendorManagementConstants.REQUEST_TYPE]);
                            string configKeys = Convert.ToString(row[SRMVendorManagementConstants.CONFIGURATION_KEY]);
                            string values = Convert.ToString(row[SRMVendorManagementConstants.CONFIGURATION_VALUE]);

                            dictPrefVsVendorVsReqestVsConfigs[preferenceName][vendorName][requestType][configKeys] = values;
                        }
                    }
                    if (table.TableName.SRMEqualWithIgnoreCase(SRMVendorManagementConstants.TABLE_HEADERS))
                    {
                        foreach (ObjectRow row in table.AsEnumerable())
                        {
                            string preferenceName = Convert.ToString(row[SRMVendorManagementConstants.PREFERENCE_NAME]);
                            string vendorName = Convert.ToString(row[SRMVendorManagementConstants.VENDOR_NAME]);
                            string requestType = Convert.ToString(row[SRMVendorManagementConstants.REQUEST_TYPE]);
                            string headerType = Convert.ToString(row[SRMVendorManagementConstants.HEADER_TYPE_NAME]);
                            string headerName = Convert.ToString(row[SRMVendorManagementConstants.HEADER_NAME]);
                            string headerValue = Convert.ToString(row[SRMVendorManagementConstants.HEADER_VALUE]);

                            dictPrefVsVendorVsReqestVsHeaders[preferenceName][vendorName][requestType][headerType].Add(headerName, headerValue);

                        }
                    }
                }


                List<string> failedPreferences = new List<string>();
                if (deltaSet.Tables.Contains(SRMVendorManagementConstants.TABLE_CONFIGURATIONS))
                    failedPreferences = deltaSet.Tables[SRMVendorManagementConstants.TABLE_CONFIGURATIONS].AsEnumerable().Where(r => Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Select(x => Convert.ToString(x[SRMVendorManagementConstants.PREFERENCE_NAME])).ToList();

                bool isValid = true;
                Dictionary<string, Dictionary<string, Dictionary<string, string>>> DefaultConfig = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);
                //Dictionary<string, Dictionary<string, Dictionary<string, string>>> DefaultConfig = dictPrefVsVendorVsReqestVsConfigs["Default"].ToDictionary(x => x.Key, y => y.Value.ToDictionary(s => s.Key, t => t.Value.ToDictionary(a => a.Key, b => string.Empty), StringComparer.OrdinalIgnoreCase));

                var defcon = CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT DISTINCT ventype.vendor_name AS [Vendor Name],
                vreq.request_type_name AS[Request Type],
                vdet.key_name AS[Configuration Key], vdet.key_value AS[Value],is_visible
                FROM IVPRefMasterVendor.dbo.ivp_rad_vendor_management_master vmas
                INNER JOIN IVPRefMasterVendor.dbo.ivp_rad_vendor_management_details vdet
                ON vmas.vendor_management_id = vdet.vendor_management_id
                INNER JOIN IVPRefMasterVendor.dbo.ivp_rad_vendor_management_vendor_type ventype
                ON vdet.vendor_id = ventype.vendor_id
                INNER JOIN IVPRefMasterVendor.dbo.ivp_rad_vendor_management_request_type vreq
                ON vreq.request_type_id = vdet.request_type_id
                WHERE ISNULL(vdet.key_value, '') <> '' AND vdet.vendor_management_id = 1
                ORDER BY[Vendor Name],[Request Type],[Configuration Key]", ConnectionConstants.RefMasterVendor_Connection).Tables[0];

                List<string> lstNotVisibleConfigKeys = defcon.AsEnumerable().Where(x => !Convert.ToBoolean(x["is_visible"])).Select(y => Convert.ToString(y[SRMVendorManagementConstants.CONFIGURATION_KEY])).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                foreach (var row in defcon.AsEnumerable())
                {
                    //string preferenceName = Convert.ToString(row[SRMVendorManagementConstants.PREFERENCE_NAME]);
                    string vendorName = Convert.ToString(row[SRMVendorManagementConstants.VENDOR_NAME]);
                    string requestType = Convert.ToString(row[SRMVendorManagementConstants.REQUEST_TYPE]);
                    string configKeys = Convert.ToString(row[SRMVendorManagementConstants.CONFIGURATION_KEY]);
                    string values = Convert.ToString(row[SRMVendorManagementConstants.CONFIGURATION_VALUE]);

                    if (!DefaultConfig.ContainsKey(vendorName))
                        DefaultConfig.Add(vendorName, new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase));

                    if (!DefaultConfig[vendorName].ContainsKey(requestType))
                        DefaultConfig[vendorName].Add(requestType, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

                    DefaultConfig[vendorName][requestType].Add(configKeys, values);

                }
                

                #region Configurations Sheet
                if (deltaSet.Tables.Contains(SRMVendorManagementConstants.TABLE_CONFIGURATIONS) && deltaSet.Tables[SRMVendorManagementConstants.TABLE_CONFIGURATIONS] != null
                        && deltaSet.Tables[SRMVendorManagementConstants.TABLE_CONFIGURATIONS].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[SRMVendorManagementConstants.TABLE_CONFIGURATIONS];
                    //dbTable = dbVendorSystemSettingsData.Tables[SRMVendorManagementConstants.TABLE_CONFIGURATIONS];

                    var group = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS]))).GroupBy(x => Convert.ToString(x[SRMVendorManagementConstants.PREFERENCE_NAME]));


                    foreach (var prefGroup in group)
                    {

                        string preferenceName = prefGroup.Key;
                        var vendorGroup = prefGroup.GroupBy(x => Convert.ToString(x[SRMVendorManagementConstants.VENDOR_NAME]));
                        bool isInsert = false;
                        if (!failedPreferences.Contains(preferenceName))
                        {
                            foreach (var row in vendorGroup.ToList())
                            {
                                string vendorName = row.Key;
                                foreach (var innerrow in row)
                                {
                                    string requestType = Convert.ToString(innerrow[SRMVendorManagementConstants.REQUEST_TYPE]);
                                    string configKeys = Convert.ToString(innerrow[SRMVendorManagementConstants.CONFIGURATION_KEY]);
                                    string values = Convert.ToString(innerrow[SRMVendorManagementConstants.CONFIGURATION_VALUE]);
                                    List<string> errormssgs = new List<string>();

                                    errormssgs.AddRange(ValidatePreferenceName(preferenceName));
                                    errormssgs.AddRange(ValidateKeys(vendorName, requestType, configKeys, dictVendorVSRequestTypeVsConfigKey));
                                    if (lstNotVisibleConfigKeys.Contains(configKeys))
                                        errormssgs.Add("Value for this configuration key cannot be added.");

                                    if (errormssgs.Count() > 0)
                                    {
                                        new RMCommonMigration().SetFailedRow(innerrow, errormssgs, false);
                                        isValid = false;
                                    }
                                    else
                                    {
                                        if (dictPrefVsVendorVsReqestVsConfigs.ContainsKey(preferenceName))
                                        {
                                            if (PrefVsIsInsert.ContainsKey(preferenceName))
                                                PrefVsIsInsert[preferenceName] = false;


                                        }

                                        else if (!dictPrefVsVendorVsReqestVsConfigs.ContainsKey(preferenceName))
                                        {
                                            if (!PrefVsIsInsert.ContainsKey(preferenceName))
                                            {
                                                PrefVsIsInsert.Add(preferenceName, true);
                                                isInsert = true;

                                            }

                                            dictPrefVsVendorVsReqestVsConfigs[preferenceName] = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>(DefaultConfig,StringComparer.OrdinalIgnoreCase);

                                            //dictPrefVsVendorVsReqestVsHeaders[preferenceName] = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(StringComparer.OrdinalIgnoreCase);
                                            dictPrefVsVendorVsReqestVsHeaders[preferenceName] = dictVendorVSRequestTypeVsHeaderType.ToDictionary(x => x.Key, y => y.Value.ToDictionary(s => s.Key, t => t.Value.ToDictionary(u => u, v => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))));

                                        }

                                        dictPrefVsVendorVsReqestVsConfigs[preferenceName][vendorName][requestType][configKeys] = values;
                                    }

                                }

                            }
                            if (isValid)
                            {

                                VendorManagementInputInfo inputInfo = new VendorManagementInputInfo();

                                if (isInsert == true)
                                {
                                    inputInfo.VendorManagementId = 0;
                                    inputInfo.VendorManagementToCloneFromID = 1;
                                }
                                else if (isInsert == false)
                                {
                                    inputInfo.VendorManagementId = PreferenceNameVsId[preferenceName];
                                    inputInfo.VendorManagementToCloneFromID = 1;
                                }
                                inputInfo.VendorManagementName = preferenceName;
                                inputInfo.UserName = username;

                                inputInfo.Configurations = dictPrefVsVendorVsReqestVsConfigs[preferenceName].Select(x =>
                                {
                                    string vendorName = x.Key;
                                    var dd = new ConfigurationsType
                                    {
                                        VendorId = VendorNameVsId[vendorName],
                                        FTP = x.Value.Where(y => y.Key.Equals("FTP")).SelectMany(a => a.Value).Select(z => new ConfigurationsDataType(z.Key, z.Value)).ToList(),
                                        SAPI = x.Value.Where(y => y.Key.Equals("SAPI")).SelectMany(a => a.Value).Select(z => new ConfigurationsDataType(z.Key, z.Value)).ToList(),
                                        HeavyLite = x.Value.Where(y => y.Key.Equals("Heavy")).SelectMany(a => a.Value).Select(z => new ConfigurationsDataType(z.Key, z.Value)).ToList(),
                                        GlobalApi = x.Value.Where(y => y.Key.Equals("GlobalAPI")).SelectMany(a => a.Value).Select(z => new ConfigurationsDataType(z.Key, z.Value)).ToList()

                                    };

                                    return dd;
                                }).ToList();
                                inputInfo.Headers = dictPrefVsVendorVsReqestVsHeaders[preferenceName].Select(x =>
                                {
                                    string vendorName = x.Key;
                                    var headers = new HeadersType
                                    {
                                        VendorId = VendorNameVsId[vendorName],
                                        FTP = x.Value.Where(y => y.Key.Equals("FTP")).SelectMany(a => a.Value).Select(z => new HeadersRequestType
                                        {
                                            requestType = z.Key,
                                            value = z.Value.Select(l => new HeadersRequestTypeDataType(l.Key, l.Value)).ToList()

                                        }).ToList(),
                                        HeavyLite = x.Value.Where(y => y.Key.Equals("Heavy")).SelectMany(a => a.Value).Select(z => new HeadersRequestType
                                        {
                                            requestType = z.Key,
                                            value = z.Value.Select(l => new HeadersRequestTypeDataType(l.Key, l.Value)).ToList()

                                        }).ToList()
                                    };
                                    return headers;
                                }).ToList();

                                try
                                {
                                    var Output = new VendorSystemSettingsController().SaveVendorManagementData(inputInfo);
                                    if (!PreferenceNameVsId.ContainsKey(preferenceName))
                                        PreferenceNameVsId.Add(preferenceName, Output.VendorManagementId);
                                }
                                catch (Exception ex)
                                {
                                    isValid = false;
                                }

                            }
                            if (isValid)
                            {
                                foreach (var outerrow in group.ToList())
                                {
                                    foreach (var row in outerrow.ToList())
                                        new RMCommonMigration().SetPassedRow(row, string.Empty);
                                }

                            }
                            else
                            {
                                foreach (var outerrow in group.ToList())
                                {
                                    foreach (var row in outerrow.ToList())
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { }, false);
                                }
                                if (!failedPreferences.Contains(preferenceName))
                                    failedPreferences.Add(preferenceName);
                            }
                        }
                        else
                        {
                            foreach (var outerrow in group.ToList())
                            {
                                foreach (var row in outerrow.ToList())
                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { }, false);
                            }
                        }

                    }

                }
                #endregion


                #region Headers Sheet

                if (deltaSet.Tables.Contains(SRMVendorManagementConstants.TABLE_HEADERS) && deltaSet.Tables[SRMVendorManagementConstants.TABLE_HEADERS] != null
                            && deltaSet.Tables[SRMVendorManagementConstants.TABLE_HEADERS].Rows.Count > 0)
                {
                    sourceTable = deltaSet.Tables[SRMVendorManagementConstants.TABLE_HEADERS];
                    //dbTable = dbVendorSystemSettingsData.Tables[SRMVendorManagementConstants.TABLE_HEADERS];


                    var group = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS]))).GroupBy(x => Convert.ToString(x[SRMVendorManagementConstants.PREFERENCE_NAME]));

                    foreach (var prefGroup in group)
                    {
                        List<string> errormssgs = new List<string>();
                        string preferenceName = prefGroup.Key;
                        bool isInsert = false;

                        if (!failedPreferences.Contains(preferenceName))
                        {
                            var vendorGroup = prefGroup
                                .GroupBy(x => new
                                {
                                    VendorName = Convert.ToString(x[SRMVendorManagementConstants.VENDOR_NAME]),
                                    RequestType = Convert.ToString(x[SRMVendorManagementConstants.REQUEST_TYPE]),
                                    HeaderType = Convert.ToString(x[SRMVendorManagementConstants.HEADER_TYPE_NAME])
                                });

                            foreach (var row in vendorGroup.ToList())
                            {
                                string vendorName = row.Key.VendorName;
                                string requestType = row.Key.RequestType;
                                string headerType = row.Key.HeaderType;
                                dictPrefVsVendorVsReqestVsHeaders[preferenceName][vendorName][requestType][headerType] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                                foreach (var innerrow in row)
                                {

                                    string headerName = Convert.ToString(innerrow[SRMVendorManagementConstants.HEADER_NAME]);
                                    string headerValue = Convert.ToString(innerrow[SRMVendorManagementConstants.HEADER_VALUE]);

                                    errormssgs.AddRange(ValidatePreferenceName(preferenceName));
                                    errormssgs.AddRange(ValidateHeaders(vendorName, requestType, headerType, dictVendorVSRequestTypeVsHeaderType));

                                    if (errormssgs.Count() > 0)
                                    {
                                        new RMCommonMigration().SetFailedRow(innerrow, errormssgs, false);
                                        isValid = false;
                                    }
                                    else
                                    {
                                        if (dictPrefVsVendorVsReqestVsHeaders.ContainsKey(preferenceName))
                                        {
                                            if (PrefVsIsInsert.ContainsKey(preferenceName))
                                                PrefVsIsInsert[preferenceName] = false;

                                            if (dictPrefVsVendorVsReqestVsHeaders[preferenceName][vendorName][requestType][headerType].ContainsKey(headerName))
                                                dictPrefVsVendorVsReqestVsHeaders[preferenceName][vendorName][requestType][headerType][headerName] = string.Empty;

                                            dictPrefVsVendorVsReqestVsHeaders[preferenceName][vendorName][requestType][headerType][headerName] = headerValue;
                                        }

                                        else if (!dictPrefVsVendorVsReqestVsHeaders.ContainsKey(preferenceName))
                                        {
                                            if (!PrefVsIsInsert.ContainsKey(preferenceName))
                                                PrefVsIsInsert.Add(preferenceName, true);
                                            isInsert = true;
                                            dictPrefVsVendorVsReqestVsHeaders.Add(preferenceName, new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(StringComparer.OrdinalIgnoreCase));

                                            dictPrefVsVendorVsReqestVsHeaders[preferenceName][vendorName][requestType][headerType].Add(headerName, headerValue);

                                        }

                                    }

                                }

                            }
                            if (isValid)
                            {

                                VendorManagementInputInfo inputInfo = new VendorManagementInputInfo();

                                if (isInsert == true)
                                {
                                    inputInfo.VendorManagementId = 0;
                                    inputInfo.VendorManagementToCloneFromID = 1;
                                }
                                else if (isInsert == false)
                                {
                                    inputInfo.VendorManagementId = PreferenceNameVsId[preferenceName];
                                    inputInfo.VendorManagementToCloneFromID = 1;

                                }
                                inputInfo.VendorManagementName = preferenceName;
                                inputInfo.UserName = username;

                                inputInfo.Configurations = dictPrefVsVendorVsReqestVsConfigs[preferenceName].Select(x =>
                                {
                                    string vendorName = x.Key;
                                    var dd = new ConfigurationsType
                                    {
                                        VendorId = VendorNameVsId[vendorName],
                                        FTP = x.Value.Where(y => y.Key.Equals("FTP")).SelectMany(a => a.Value).Select(z => new ConfigurationsDataType(z.Key, z.Value)).ToList(),
                                        SAPI = x.Value.Where(y => y.Key.Equals("SAPI")).SelectMany(a => a.Value).Select(z => new ConfigurationsDataType(z.Key, z.Value)).ToList(),
                                        HeavyLite = x.Value.Where(y => y.Key.Equals("Heavy")).SelectMany(a => a.Value).Select(z => new ConfigurationsDataType(z.Key, z.Value)).ToList(),
                                        GlobalApi = x.Value.Where(y => y.Key.Equals("GlobalAPI")).SelectMany(a => a.Value).Select(z => new ConfigurationsDataType(z.Key, z.Value)).ToList()

                                    };

                                    return dd;
                                }).ToList();

                                inputInfo.Headers = dictPrefVsVendorVsReqestVsHeaders[preferenceName].Select(x =>
                               {
                                   string vendorName = x.Key;
                                   var headers = new HeadersType
                                   {
                                       VendorId = VendorNameVsId[vendorName],
                                       FTP = x.Value.Where(y => y.Key.Equals("FTP")).SelectMany(a => a.Value).Select(z => new HeadersRequestType
                                       {
                                           requestType = z.Key,
                                           value = z.Value.Select(l => new HeadersRequestTypeDataType(l.Key, l.Value)).ToList()

                                       }).ToList(),
                                       HeavyLite = x.Value.Where(y => y.Key.Equals("Heavy")).SelectMany(a => a.Value).Select(z => new HeadersRequestType
                                       {
                                           requestType = z.Key,
                                           value = z.Value.Select(l => new HeadersRequestTypeDataType(l.Key, l.Value)).ToList()

                                       }).ToList()
                                   };
                                   return headers;
                               }).ToList();
                                try
                                {
                                    var Output = new VendorSystemSettingsController().SaveVendorManagementData(inputInfo);
                                }
                                catch (Exception e)
                                {
                                    isValid = false;
                                }

                            }
                            if (isValid)
                            {
                                foreach (var outerrow in group.ToList())
                                {
                                    foreach (var row in outerrow.ToList())
                                        new RMCommonMigration().SetPassedRow(row, string.Empty);
                                }
                            }
                            else
                            {
                                foreach (var outerrow in group.ToList())
                                {
                                    foreach (var row in outerrow.ToList())
                                        new RMCommonMigration().SetFailedRow(row, new List<string>() { }, false);
                                }
                            }
                        }
                        else
                        {
                            foreach (var outerrow in group.ToList())
                            {
                                foreach (var row in outerrow.ToList())
                                    FailAllDependencies(deltaSet, failedPreferences); ;
                            }
                        }

                    }

                }

                #endregion


            }
            catch (Exception ex)
            {
                mLogger.Debug("SRMVendorSettingsController : SyncVendor -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("SRMVendorSettingsController : SyncVendor -> End");
            }
        }

        private void FailAllDependencies(ObjectSet deltaSet, List<string> failedPreferences)
        {
            Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>(StringComparer.OrdinalIgnoreCase);

            foreach (ObjectTable table in deltaSet.Tables)
            {
                dependencies.Add(table.TableName, null);

                dependencies[table.TableName] =
                table.AsEnumerable()
                    .Where
                    (
                        t => (failedPreferences.SRMContainsWithIgnoreCase(RMCommonStatic.ConvertToLower(t[SRMVendorManagementConstants.PREFERENCE_NAME])) && string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(t[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(t[RMCommonConstants.MIGRATION_COL_STATUS])))
                    );
            }

            new RMCommonMigration().FailDependencies(dependencies, "Preference not processed.", true);
        }

    }
}
