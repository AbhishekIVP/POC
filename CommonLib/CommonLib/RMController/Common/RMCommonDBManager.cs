using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.rad.culturemanager;
using com.ivp.srmcommon;

namespace com.ivp.common
{
    public class RMCommonDBManager
    {
        public RDBConnectionManager mDBCon = null;
        public RMCommonDBManager(RDBConnectionManager connectionManager = null)
        {
            this.mDBCon = connectionManager;
        }

        static IRLogger mLogger = RLogFactory.CreateLogger("RMCommonDBManager");

        internal DataSet GetAttributeDataTypes()
        {
            try
            {
                DataSet dsResult;
                string query = "Select data_type_name FROM IVPRefMaster.dbo.ivp_refm_attribute_type";
                if (this.mDBCon != null)
                {
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(query, this.mDBCon);
                }
                else
                {
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                }
                return dsResult;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetAllReportTypes -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        internal DataSet GetDateTypes(EMDateType dateType = EMDateType.Both)
        {
            try
            {
                DataSet dsResult;
                string query = "SELECT date_type_id,date_display_name FROM IVPRefMaster.dbo.ivp_refm_date_type";
                if(dateType == EMDateType.StartDate)
                {
                    query += " WHERE is_start_date_type = 1";
                }
                else if (dateType == EMDateType.EndDate)
                {
                    query += " WHERE is_end_date_type = 1";
                }
                if (this.mDBCon != null)
                {
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(query, this.mDBCon);
                }
                else
                {
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                }
                return dsResult;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetAllReportTypes -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        public DataSet GetTableOrViewDetailsInfo(string database, string tableName)
        {
            RHashlist mList = null;
            DataSet objectInfo = new DataSet();
            mLogger.Debug("RMCommonUtils->GetTableOrViewDetailsInfo-> start");
            try
            {
                mList = new RHashlist();
                mList.Add("dbname", database);
                mList.Add("objname", tableName);
                if (this.mDBCon == null)
                {
                    objectInfo = (DataSet)CommonDALWrapper.ExecuteProcedure("REFM:GetTableOrViewDetailsInfo", mList, ConnectionConstants.RefMaster_Connection)["DataSet"];
                }

                else
                {
                    objectInfo = (DataSet)CommonDALWrapper.ExecuteProcedure("REFM:GetTableOrViewDetailsInfo", mList, mDBCon)["DataSet"];
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            mLogger.Debug("RMCommonUtils->GetTableOrViewDetailsInfo-> End");

            return objectInfo;
        }
       

        public string GetColumn(RMColumnInfo cInfo)
        {
            string cols = string.Empty;
            switch (cInfo.DataType)
            {
                case RMDBDataTypes.VARCHARMAX:
                    cols = "[varchar](max)";
                    break;
                case RMDBDataTypes.VARCHAR:
                    string len = cInfo.Length;
                    cols = "[varchar] ( " + len + " )";
                    break;
                case RMDBDataTypes.INT:
                    cols = " [int] ";
                    break;
                case RMDBDataTypes.DECIMAL:
                    cols = "[decimal] (" + cInfo.Length.Split(',')[0] + ", " + cInfo.Length.Split(',')[1] + " )";
                    break;
                case RMDBDataTypes.DATETIME:
                    cols = "[datetime]";
                    break;
                case RMDBDataTypes.BIT:
                    cols = "[bit]";
                    break;
                case RMDBDataTypes.LOOKUP:
                    cols = "[varchar](100)";
                    break;
                case RMDBDataTypes.FILE:
                    cols = "[varchar](8000)";
                    break;
            }
            cols = "[" + cInfo.ColumnName + "] " + cols;
            return cols;
        }

        public void ChangeDBForConnection(string dbName)
        {
            if (this.mDBCon != null)
            {
                CommonDALWrapper.ExecuteQuery("USE " + dbName, CommonQueryType.Update, this.mDBCon);
            }
        }

        public List<string> GetAllCultureDateFormats()
        {
            mLogger.Debug("RMCommonDBManager: GetAllCultureDateFormats -> Start");
            try
            {
                List<string> dateTimeFormats = new List<string>();
                DataTable dt = RCultureController.GetAllCultures(false);
                Dictionary<string, string> datetimeDictionary = new Dictionary<string, string>();
                dt.AsEnumerable().ToList<DataRow>().ForEach(dataRow =>
                {
                    if (dateTimeFormats.Contains(dataRow["Short Date Format"].ToString()) == false &&
                        dataRow["Short Date Format"].ToString().Contains("'г.'") == false)
                    {
                        dateTimeFormats.Add(dataRow["Short Date Format"].ToString());
                    }
                });
                dateTimeFormats.Sort((first, second) => { return first.Length > second.Length ? 1 : (first.Length == second.Length ? 0 : -1); });

                return dateTimeFormats;
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMCommonDBManager: GetAllCultureDateFormats -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RMCommonDBManager: GetAllCultureDateFormats -> End");
            }
        }

        #region Attribute Meta Data

        public SRFPMAttributeMetaDataOutputObject RFPM_GetAttributeMetaData(SRFPMAttributeMetaDataInputObject InputObject)
        {
            SRFPMAttributeMetaDataOutputObject result = new SRFPMAttributeMetaDataOutputObject();
            try
            {
                DataSet ds = null;
                if (this.mDBCon == null)
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery("exec IVPRefMaster.dbo.REFM_GetAttributeMetaData " + InputObject.TypeId + ", " + InputObject.AttributeId + ", " + InputObject.ModuleId, ConnectionConstants.RefMaster_Connection);
                }

                else
                {
                    ds = CommonDALWrapper.ExecuteSelectQuery("exec IVPRefMaster.dbo.REFM_GetAttributeMetaData " + InputObject.TypeId + ", " + InputObject.AttributeId + ", " + InputObject.ModuleId, mDBCon);
                }

                if (ds != null && ds.Tables.Count > 1)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        string dataLength = string.Empty;
                        //var defaultValueDateTime;

                        if (Convert.ToString(dr["data_type"]).ToUpper() == "DECIMAL")
                        {
                            var attrLength = Convert.ToString(dr["data_type_length"]).Split(',');
                            var firstDigit = Convert.ToInt32(attrLength[0]) - Convert.ToInt32(attrLength[1]);
                            dataLength = Convert.ToString(firstDigit) + '.' + Convert.ToString(attrLength[1]);
                        }
                        else
                            dataLength = Convert.ToString(dr["data_type_length"]);

                        if (Convert.ToString(dr["data_type"]).ToUpper() == "DATETIME" && !string.IsNullOrEmpty(Convert.ToString(dr["default_value"])))
                        {
                            DateTime dt;
                            DateTime.TryParseExact(Convert.ToString(dr["default_value"]), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out dt);
                            result.DefaultValue = dt.ToShortDateString().ToString(System.Globalization.CultureInfo.GetCultureInfo(System.Globalization.CultureInfo.CurrentCulture.ToString()));
                        }
                        else
                            result.DefaultValue = string.IsNullOrEmpty(Convert.ToString(dr["default_value"])) ? "" : Convert.ToString(dr["default_value"]);

                        result.AttributeDescription = Convert.ToString(dr["attribute_description"]);
                        result.DataType = Convert.ToString(dr["data_type"]);
                        result.DataLength = Convert.ToString(dataLength);
                        result.IsCloneable = Convert.ToBoolean(dr["is_clonable"]);
                        result.IsPII = Convert.ToBoolean(dr["is_pii"]);
                        result.IsEncrypted = Convert.ToBoolean(dr["is_encrypted"]);
                        result.ReferenceEntityTypeName = string.IsNullOrEmpty(Convert.ToString(dr["Lookup Type"])) ? "" : Convert.ToString(dr["Lookup Type"]);


                        if (Convert.ToString(dr["data_type"]).ToUpper() == "SECURITY_LOOKUP")
                        {
                            result.SecurityTypeName = Convert.ToInt32(dr["security_id"]) == 0 ? "All Security Types" : GetSecurityType(Convert.ToInt32(dr["security_id"]));
                        }
                        else
                            result.SecurityTypeName = "";
                        result.RestrictedCharacters = string.IsNullOrEmpty(Convert.ToString(dr["restricted_chars"])) ? "" : Convert.ToString(dr["restricted_chars"]);
                        result.IsPrimary = Convert.ToInt32(dr["primary_attribute"]);
                        result.Tags = Convert.ToString(dr["Tags"]);
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        List<string> vendorPriority = ds.Tables[1].AsEnumerable().Select(x => Convert.ToString(x["data_source_name"])).Distinct().ToList();
                        result.VendorPriority = string.Join(",", vendorPriority);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                mLogger.Error("RMCommonDBManager -> RFPM_GetAttributeMetaData -> Error : " + ex.ToString());
                throw ex;
            }
        }
        private string GetSecurityType(int sectypeId)
        {
            string sectype = string.Empty;
            sectype = CommonDALWrapper.ExecuteSelectQuery("select sectype_name from IVPSecMaster.dbo.ivp_secm_sectype_master where sectype_id =" + sectypeId + " and is_active = 1;", ConnectionConstants.SecMaster_Connection).Tables[0].Rows[0][0].ToString();
            return sectype;

        }
        #endregion Attribute Meta Data

    }
}