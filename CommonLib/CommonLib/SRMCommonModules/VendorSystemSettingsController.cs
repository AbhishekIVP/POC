using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using com.ivp.commom.commondal;
using com.ivp.rad.cryptography;
using com.ivp.rad.common;

namespace SRMCommonModules
{
    public class VendorSystemSettingsController
    {

        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMCommonModules.VendorSystemSettingsController");
        //Vendor Management Data
        //gives the list of preferences
        public VendorManagementDataSourceType GetVendorManagementData(int preferenceId, int vendorId)
        {
            VendorManagementDataSourceType result = new VendorManagementDataSourceType();

            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMasterVendor.dbo.RAD_GetVendorPreferenceData {0}, {1}", preferenceId, vendorId), ConnectionConstants.RefMasterVendor_Connection);

            //preferences part

            foreach (var item in ds.Tables[0].AsEnumerable())
            {
                result.VendorPreferences.Add(new VendorManagementPreferences(Convert.ToInt32(item["vendor_management_id"]), Convert.ToString(item["vendor_management_name"])));
            }


            //fixed table
            foreach (var grp in ds.Tables[1].AsEnumerable().GroupBy(x => new { requestType = Convert.ToString(x["request_type_name"]) }))
            {
                List<ConfigurationsDataType> lst = null;
                switch (grp.Key.requestType.ToLower())
                {
                    case "ftp":
                        lst = result.Configurations.FTP;
                        break;
                    case "sapi":
                        lst = result.Configurations.SAPI;
                        break;
                    case "heavy":
                        lst = result.Configurations.HeavyLite;
                        break;
                    case "globalapi":
                        lst = result.Configurations.GlobalApi;
                        break;
                    default:
                        //some error handling
                        break;
                }

                foreach (DataRow dr in grp)
                {
                    string keyName = Convert.ToString(dr["key_name"]);
                    lst.Add(new ConfigurationsDataType(keyName, (keyName.Equals("password", StringComparison.OrdinalIgnoreCase) ? "" : Convert.ToString(dr["key_value"]))));
                }
            }


            /* ??
            result.Headers.FTP = new List<HeadersRequestType>();
            result.Headers.HeavyLite = new List<HeadersRequestType>();
            */

            foreach (var grp in ds.Tables[2].AsEnumerable().GroupBy(x => new { requestType = Convert.ToString(x["request_type_name"]) }))
            {
                List<HeadersRequestType> lst = null;
                switch (grp.Key.requestType.ToLower())
                {
                    case "ftp":
                        lst = result.Headers.FTP;
                        break;
                    case "heavy":
                        lst = result.Headers.HeavyLite;
                        break;
                    default:
                        break;
                }
                lst.AddRange(grp.GroupBy(x => Convert.ToString(x["header_type_name"])).Select(x => new HeadersRequestType { requestType = x.Key, value = x.Select(y => new HeadersRequestTypeDataType(Convert.ToString(y["header_name"]), Convert.ToString(y["header_value"]))).ToList() }).ToList());
            }

            foreach (var grp in ds.Tables[3].AsEnumerable().GroupBy(x => new { requestType = Convert.ToString(x["request_type_name"]) }))
            {
                List<HeadersRequestType> lst = null;
                switch (grp.Key.requestType.ToLower())
                {
                    case "ftp":
                        lst = result.Headers.FTP;
                        break;
                    case "heavy":
                        lst = result.Headers.HeavyLite;
                        break;
                    default:
                        break;
                }
                foreach (DataRow dr in grp)
                {
                    string headerType = Convert.ToString(dr["header_type_name"]);
                    if (lst.Count == 0 || lst.Where(x => x.requestType.Equals(headerType, StringComparison.OrdinalIgnoreCase)).Count() == 0)
                    {
                        lst.Add(new HeadersRequestType() { requestType = headerType, value = new List<HeadersRequestTypeDataType>() });
                    }
                }
            }

            return result;
        }

        public VendorManagementDataSourceType SaveVendorManagementData(VendorManagementInputInfo inputData)
        {
            RRSAEncrDecr encDec = new RRSAEncrDecr();
            VendorManagementDataSourceType result = new VendorManagementDataSourceType() { Status = true };

            try
            {
                string guid = Guid.NewGuid().ToString().Replace("-", "_");

                DataTable dtHeaders = new DataTable();
                dtHeaders.Columns.Add("header_name");
                dtHeaders.Columns.Add("header_value");
                dtHeaders.Columns.Add("request_type_name");
                dtHeaders.Columns.Add("header_type_name");
                dtHeaders.Columns.Add("vendor_id");

                DataTable dtConfiguration = new DataTable();
                dtConfiguration.Columns.Add("key_name");
                dtConfiguration.Columns.Add("key_value");
                dtConfiguration.Columns.Add("request_type_name");
                dtConfiguration.Columns.Add("vendor_id");

                if (inputData != null && inputData.Configurations != null && inputData.Configurations.Count > 0)
                {
                    foreach (var config in inputData.Configurations)
                    {
                        if (config.FTP != null && config.FTP.Count > 0)
                        {
                            foreach (var item in config.FTP)
                            {
                                if (item.labelName.Equals("password", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!string.IsNullOrEmpty(item.value))
                                    {
                                        try
                                        {
                                            item.value = encDec.DoDecrypt(item.value);
                                        }
                                        catch (Exception ex)
                                        {
                                            mLogger.Error("Error while decrypting password => " + ex.ToString());
                                        }
                                        try
                                        {
                                            item.value = encDec.DoEncrypt(item.value);
                                        }
                                        catch (Exception ex)
                                        {
                                            mLogger.Error("Error while encrypting password => " + ex.ToString());
                                        }
                                    }
                                    else
                                        continue;
                                }
                                DataRow dr = dtConfiguration.NewRow();
                                dr["key_name"] = item.labelName;
                                dr["key_value"] = item.value;
                                dr["request_type_name"] = VendorManagementRequestType.FTP.ToString();
                                dr["vendor_id"] = config.VendorId;

                                dtConfiguration.Rows.Add(dr);
                            }
                        }
                        if (config.SAPI != null && config.SAPI.Count > 0)
                        {
                            foreach (var item in config.SAPI)
                            {
                                if (item.labelName.Equals("password", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!string.IsNullOrEmpty(item.value))
                                    {
                                        try
                                        {
                                            item.value = encDec.DoDecrypt(item.value);
                                        }
                                        catch (Exception ex)
                                        {
                                            mLogger.Error("Error while decrypting password => " + ex.ToString());
                                        }
                                        try
                                        {
                                            item.value = encDec.DoEncrypt(item.value);
                                        }
                                        catch (Exception ex)
                                        {
                                            mLogger.Error("Error while encrypting password => " + ex.ToString());
                                        }
                                    }
                                    else
                                        continue;
                                }
                                DataRow dr = dtConfiguration.NewRow();
                                dr["key_name"] = item.labelName;
                                dr["key_value"] = item.value;
                                dr["request_type_name"] = VendorManagementRequestType.SAPI.ToString();
                                dr["vendor_id"] = config.VendorId;

                                dtConfiguration.Rows.Add(dr);
                            }
                        }
                        if (config.GlobalApi != null && config.GlobalApi.Count > 0)
                        {
                            foreach (var item in config.GlobalApi)
                            {
                                if (item.labelName.Equals("password", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!string.IsNullOrEmpty(item.value))
                                    {
                                        try
                                        {
                                            item.value = encDec.DoDecrypt(item.value);
                                        }
                                        catch (Exception ex)
                                        {
                                            mLogger.Error("Error while decrypting password => " + ex.ToString());
                                        }
                                        try
                                        {
                                            item.value = encDec.DoEncrypt(item.value);
                                        }
                                        catch (Exception ex)
                                        {
                                            mLogger.Error("Error while encrypting password => " + ex.ToString());
                                        }
                                    }
                                    else
                                        continue;
                                }
                                DataRow dr = dtConfiguration.NewRow();
                                dr["key_name"] = item.labelName;
                                dr["key_value"] = item.value;
                                dr["request_type_name"] = VendorManagementRequestType.GlobalAPI.ToString();
                                dr["vendor_id"] = config.VendorId;

                                dtConfiguration.Rows.Add(dr);
                            }
                        }
                        if (config.HeavyLite != null && config.HeavyLite.Count > 0)
                        {
                            foreach (var item in config.HeavyLite)
                            {
                                if (item.labelName.Equals("password", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!string.IsNullOrEmpty(item.value))
                                    {
                                        try
                                        {
                                            item.value = encDec.DoDecrypt(item.value);
                                        }
                                        catch (Exception ex)
                                        {
                                            mLogger.Error("Error while decrypting password => " + ex.ToString());
                                        }
                                        try
                                        {
                                            item.value = encDec.DoEncrypt(item.value);
                                        }
                                        catch (Exception ex)
                                        {
                                            mLogger.Error("Error while encrypting password => " + ex.ToString());
                                        }
                                    }
                                    else
                                        continue;
                                }
                                DataRow dr = dtConfiguration.NewRow();
                                dr["key_name"] = item.labelName;
                                dr["key_value"] = item.value;
                                dr["request_type_name"] = VendorManagementRequestType.Heavy.ToString();
                                dr["vendor_id"] = config.VendorId;

                                dtConfiguration.Rows.Add(dr);
                            }
                        }
                    }
                }

                if (inputData != null && inputData.Headers != null && inputData.Headers.Count > 0)
                {
                    foreach (var header in inputData.Headers)
                    {
                        if (header.FTP != null && header.FTP.Count > 0)
                        {
                            foreach (var headers in header.FTP)
                            {
                                foreach (var item in headers.value)
                                {
                                    DataRow dr = dtHeaders.NewRow();
                                    dr["header_name"] = item.headerName;
                                    dr["header_value"] = item.headerValue;
                                    dr["request_type_name"] = VendorManagementRequestType.FTP.ToString();
                                    dr["header_type_name"] = headers.requestType;
                                    dr["vendor_id"] = header.VendorId;

                                    dtHeaders.Rows.Add(dr);
                                }
                            }
                        }
                        if (header.HeavyLite != null && header.HeavyLite.Count > 0)
                        {
                            foreach (var headers in header.HeavyLite)
                            {
                                foreach (var item in headers.value)
                                {
                                    DataRow dr = dtHeaders.NewRow();
                                    dr["header_name"] = item.headerName;
                                    dr["header_value"] = item.headerValue;
                                    dr["request_type_name"] = VendorManagementRequestType.Heavy.ToString();
                                    dr["header_type_name"] = headers.requestType;
                                    dr["vendor_id"] = header.VendorId;

                                    dtHeaders.Rows.Add(dr);
                                }
                            }
                        }
                    }
                }

                try
                {
                    CommonDALWrapper.ExecuteSelectQuery(string.Format(@"CREATE TABLE IVPRefMasterVendor.dbo.Vendor_Config_{0} (key_name VARCHAR(200), key_value VARCHAR(MAX), request_type_name VARCHAR(200), vendor_id INT);
                    CREATE TABLE IVPRefMasterVendor.dbo.Vendor_Headers_{0} (header_name VARCHAR(200), header_value VARCHAR(MAX), request_type_name VARCHAR(200), header_type_name VARCHAR(200), vendor_id INT);", guid), ConnectionConstants.RefMasterVendor_Connection);
                    CommonDALWrapper.ExecuteBulkUpload("IVPRefMasterVendor.dbo.Vendor_Config_" + guid, dtConfiguration, ConnectionConstants.RefMasterVendor_Connection);
                    CommonDALWrapper.ExecuteBulkUpload("IVPRefMasterVendor.dbo.Vendor_Headers_" + guid, dtHeaders, ConnectionConstants.RefMasterVendor_Connection);
                    DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMasterVendor.dbo.RAD_SaveVendorPreferenceData '{0}', {1}, '{2}', {3}, '{4}'", guid, inputData.VendorManagementId, inputData.VendorManagementName, inputData.VendorManagementToCloneFromID, inputData.UserName), ConnectionConstants.RefMasterVendor_Connection);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[ds.Tables.Count - 1].Columns.Contains("status"))
                        {
                            result.Status = false;
                            result.Message = Convert.ToString(ds.Tables[ds.Tables.Count - 1].Rows[0]["message"]);
                        }
                        else
                        {
                            result.VendorManagementId = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                            foreach (var item in ds.Tables[1].AsEnumerable())
                            {
                                result.VendorPreferences.Add(new VendorManagementPreferences(Convert.ToInt32(item["vendor_management_id"]), Convert.ToString(item["vendor_management_name"])));
                            }

                            VendorManagementDataSourceType getHeadersAndConfigurations = GetVendorManagementData(result.VendorManagementId, 1);
                            result.Headers = getHeadersAndConfigurations.Headers;
                            result.Configurations = getHeadersAndConfigurations.Configurations;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DROP TABLE IVPRefMasterVendor.dbo.Vendor_Config_{0}
                    DROP TABLE IVPRefMasterVendor.dbo.Vendor_Headers_{0}", guid), ConnectionConstants.RefMasterVendor_Connection);
                }
            }
            catch (Exception exx)
            {
                result.Status = false;
                result.Message = exx.ToString();
            }

            return result;
        }

        public VendorManagementDataSourceType DeleteVendorManagementData(VendorManagementDeleteData inputData)
        {
            VendorManagementDataSourceType result = new VendorManagementDataSourceType();
            result.VendorManagementId = inputData.VendorManagementId;
            result.Status = true;
            try
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPRefMasterVendor.dbo.RAD_DeleteVendorPreference {0}, '{1}'", inputData.VendorManagementId, inputData.UserName), ConnectionConstants.RefMasterVendor_Connection);

                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[ds.Tables.Count - 1].Columns.Contains("status"))
                    {
                        result.Status = false;
                        result.Message = Convert.ToString(ds.Tables[ds.Tables.Count - 1].Rows[0]["message"]);
                    }
                    else
                    {
                        result.VendorManagementId = inputData.VendorManagementId;

                        foreach (var item in ds.Tables[0].AsEnumerable())
                        {
                            result.VendorPreferences.Add(new VendorManagementPreferences(Convert.ToInt32(item["vendor_management_id"]), Convert.ToString(item["vendor_management_name"])));
                        }
                        VendorManagementDataSourceType getHeadersAndConfigurations = GetVendorManagementData(result.VendorManagementId, 1);
                        result.Headers = getHeadersAndConfigurations.Headers;
                        result.Configurations = getHeadersAndConfigurations.Configurations;
                    }
                }
            }
            catch (Exception exxx)
            {
                result.Status = false;
                result.Message = exxx.ToString();
            }
            return result;
        }
    }
}
