/***************************************************************************************************
 * 
 *  This source forms a part of the IVP RAD Software System and is a copyright of 
 *  Indus Valley Partners (Pvt) Ltd.

 *  All rights are reserved with IVP. No part of this work may be reproduced, stored, 
 *  adopted or transmitted in any form or by any means including but not limiting to 
 *  electronic, mechanical, photographic, graphic, optic recording or otherwise, 
 *  translated in any language or computer language, without the prior written permission 
 *  of

 *  Indus Valley Partners (Pvt) Ltd
 *  Unit 7&8, Bldg 4
 *  Vardhman Trade Center
 *  Nehru Place Greens
 *  New Delhi - 19

 *  Copyright 2007-2008 Indus Valley Partners (Pvt) Ltd.
 * 
 * 
 * Change History
 * Version      Date            Author          Comments
 * -------------------------------------------------------------------------------------------------
 * 1            05-11-2008      Manoj           Initial Version
 * 1            05-11-2008      Nitin Saxena   Implemented Method.
 **************************************************************************************************/
using System.Collections.Generic;
using System.Data;
using System;
using com.ivp.rad.common;
using System.Text;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using com.ivp.srm.vendorapi.bloomberg;
using com.ivp.srm.vendorapi.reuters;
using System.Collections;
using com.ivp.rad.configurationmanagement;
using System.Linq;
using Microsoft.Win32;
using System.Configuration;
using com.ivp.rad.transport;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using com.ivp.rad.customclass;
using System.IO;
using com.ivp.commom.commondal;
using com.ivp.srm.vendorapi.ServiceReference1;
using System.Threading;

namespace com.ivp.srm.vendorapi
{
    /// <summary>
    /// Vendor Utils
    /// </summary>
    public class RVendorUtils
    {
        public static bool EnableBBGAudit
        {
            get
            {
                bool boolResult;
                return bool.TryParse(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, 1, RVendorConstant.ENABLE_BBG_AUDIT), out boolResult) ? boolResult : false;
            }
        }

        #region Member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        #endregion
        //------------------------------------------------------------------------------------------
        #region Public Methods
        /// <summary>
        /// Converts to data set for bulk list.
        /// </summary>
        public static DataTable ConvertToDataSetForBulkList(SecuritiesCollection securities, SecuritiesCollection unprocessed, RBbgSecurityInfo secInfo)
        {
            DataSet dsResult = null;
            DataTable dtResult = null;
            mLogger.Debug("Start -> converting security info into dataset");
            try
            {
                dsResult = new DataSet();
                dtResult = new DataTable();
                //dtResult.TableName = secInfo.BulkListMapId[index].ToString();

                foreach (string key in securities.Keys)
                {
                    dtResult.TableName = key.Split(',')[1];
                    break;
                }

                ICollection<string> keys = securities.Keys;
                int infoCount = 0;
                #region Table Schema
                if (securities != null && securities.Count > 0)
                {
                    DataColumn dcInstrument = new DataColumn("INSTRUMENT", Type.GetType("System.String"));
                    DataColumn dcIsValid = new DataColumn("is_valid", Type.GetType("System.String"));
                    dcIsValid.DefaultValue = RVendorStatusConstant.PASSED;
                    DataColumn dcFailureReason = new DataColumn("failure_reason", Type.GetType("System.String"));
                    dtResult.Columns.Add(dcInstrument);
                    dtResult.Columns.Add(dcIsValid);
                    dtResult.Columns.Add(dcFailureReason);
                    RBbgBulkListInfo bulkListInfo = RVendorUtils.GetBulkListInfo(Convert.ToInt32(dtResult.TableName));

                    infoCount = bulkListInfo.OutputFields.Split(',').Length;
                    mLogger.Error("column count=>" + infoCount);
                    for (int i = 0; i < infoCount; i++)
                    {
                        DataColumn dc1 = new DataColumn("dc" + i);
                        dc1.DataType = Type.GetType("System.String");
                        dtResult.Columns.Add(dc1);
                    }
                }
                #endregion

                #region Add Row to Table

                foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyValue in securities)
                {
                    int count = 0;
                    DataRow drRow = dtResult.NewRow();
                    if (secInfo.RequestType == RBbgRequestType.FTP)
                    {
                        List<string> matchingSec = new List<string>();
                        foreach (RBbgInstrumentInfo info in secInfo.Instruments)
                        {
                            if (keyValue.Key.StartsWith(info.InstrumentID))
                            {
                                matchingSec.Add(info.InstrumentID);
                                //drRow[count] = info.InstrumentID;
                                //break;
                            }
                        }
                        drRow[count] = GetMaxValue(matchingSec);
                    }
                    else if (secInfo.RequestType == RBbgRequestType.SAPI || secInfo.RequestType == RBbgRequestType.Heavy)
                    {
                        List<string> matchingSec = new List<string>();
                        foreach (RBbgInstrumentInfo info in secInfo.Instruments)
                        {
                            if (keyValue.Key.Contains(info.InstrumentID))
                            {
                                matchingSec.Add(info.InstrumentID);
                                //drRow[count] = info.InstrumentID;
                                //break;
                            }
                        }
                        drRow[count] = GetMaxValue(matchingSec);
                    }
                    // drRow[count] = secInfo.Instruments[0].InstrumentID;
                    count = 3;//skip Isvalid,ExceptionColumn
                    foreach (RVendorFieldInfo fldInfo in keyValue.Value.Values)
                    {
                        if (count < drRow.Table.Columns.Count)
                        {
                            drRow[count] = fldInfo.Value;
                            count++;
                        }
                    }
                    dtResult.Rows.Add(drRow);
                }


                //dsResult.Tables.Add(dtResult);
                #endregion
            }
            catch (RVendorException ex)
            {
                throw new RVendorException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End -> converting security info into dataset");
            }
            return dtResult;
        }

        public static List<List<string>> bulkConvert(string bulkListString)
        {
            List<List<string>> lst = new List<List<string>>();
            if (!string.IsNullOrWhiteSpace(bulkListString))
            {
                var contents = bulkListString.Split(';');
                int noOfFields = Convert.ToInt32(contents[3]);
                for (int index = 5; index < contents.Length; index = index + noOfFields * 2)
                {
                    List<string> lst2 = new List<string>();
                    int fieldCount = 0;
                    //bulkListdata.Append(primaryField + "|" + contents[index] + "|");

                    while (fieldCount < noOfFields)
                    {
                        string var = contents[index + fieldCount * 2];
                        if (RBbgUtils.IsInValidField(var))
                            lst2.Add(string.Empty);
                        else
                            lst2.Add(var);
                        //bulkListdata.Append(contents[index + fieldCount * 2]);
                        fieldCount++;
                        if (fieldCount >= noOfFields)
                        {


                            lst.Add(lst2);
                            lst2 = new List<string>();
                        }
                        //    bulkListdata.Append(Environment.NewLine);
                        //else
                        //    bulkListdata.Append("|");
                    }

                }
            }
            return lst;
        }

        /// <summary>
        /// Converts to data set.
        /// </summary>
        public static DataSet ConvertToDataSet(SecuritiesCollection normalizedResponse, SecuritiesCollection unProcessedResponse)
        {
            DataSet dsResult = null;
            DataTable dtResult = null;
            mLogger.Debug("Start -> converting security info into dataset");
            try
            {
                dsResult = new DataSet();
                dtResult = new DataTable("SecurityInfo");
                //--- Create Instrument Column,Default Columnn ---
                DataColumn dcInstrument = new DataColumn("INSTRUMENT", Type.GetType("System.String"));
                DataColumn dcIsValid = new DataColumn("is_valid", Type.GetType("System.String"));
                DataColumn dcFailureReason = new DataColumn("failure_reason", Type.GetType("System.String"));
                // ---Add Column to DataTable ---
                dtResult.Columns.Add(dcInstrument);
                dtResult.Columns.Add(dcIsValid);
                dtResult.Columns.Add(dcFailureReason);
                //--- Create Columns from security 
                foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyval in normalizedResponse)
                {
                    foreach (RVendorFieldInfo val in normalizedResponse[keyval.Key].Values)
                    {
                        DataColumn dc = new DataColumn(val.Name.ToUpper(), Type.GetType("System.String"));
                        dtResult.Columns.Add(dc);
                    }
                    break;
                }
                if (unProcessedResponse != null)
                {
                    foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyval in unProcessedResponse)
                    {
                        foreach (RVendorFieldInfo val in unProcessedResponse[keyval.Key].Values)
                        {
                            if (!dtResult.Columns.Contains(val.Name))
                            {
                                DataColumn dc = new DataColumn(val.Name.ToUpper(), Type.GetType("System.String"));
                                dtResult.Columns.Add(dc);
                            }
                        }
                        break;
                    }
                }
                if (normalizedResponse.Count > 0)
                {
                    foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyval in normalizedResponse)
                    {
                        DataRow drRow = dtResult.NewRow();
                        drRow["INSTRUMENT"] = keyval.Key;
                        drRow["is_valid"] = RVendorStatusConstant.PASSED;
                        foreach (var fieldInfo in normalizedResponse[keyval.Key].Values)
                        {
                            drRow[fieldInfo.Name] = fieldInfo.Value;
                        }
                        if (unProcessedResponse.Count > 0)
                        {
                            foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> key in unProcessedResponse)
                            {
                                if (drRow["INSTRUMENT"].ToString().Equals(key.Key))
                                {
                                    foreach (var fieldInfo in unProcessedResponse[keyval.Key].Values)
                                    {
                                        drRow[fieldInfo.Name] = fieldInfo.Value;
                                        drRow["failure_reason"] = fieldInfo.ExceptionMessage;
                                    }
                                    unProcessedResponse.Remove(key.Key);
                                    break;
                                }
                            }
                        }
                        dtResult.Rows.Add(drRow);
                    }
                }
                if (unProcessedResponse != null && unProcessedResponse.Count > 0)
                {
                    foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyval in unProcessedResponse)
                    {
                        DataRow drRow = dtResult.NewRow();
                        drRow["INSTRUMENT"] = keyval.Key;
                        foreach (var fieldInfo in unProcessedResponse[keyval.Key].Values)
                        {
                            drRow[fieldInfo.Name] = fieldInfo.Value;
                            drRow["failure_reason"] = fieldInfo.ExceptionMessage;
                        }
                        drRow["is_valid"] = RVendorStatusConstant.FAILED;
                        dtResult.Rows.Add(drRow);
                    }
                }
                dsResult.Tables.Add(dtResult);
            }
            catch (RVendorException ex)
            {
                throw new RVendorException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End -> converting security info into dataset");
            }
            return dsResult;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the bulk list output fields.
        /// </summary>
        public static List<string> GetBulkListOutputFields(int bulkListMapId)
        {
            mLogger.Debug("Start -> getting ouptut fields.");
            List<string> lstOutputFields = null;
            RHashlist htParams = null;
            DataSet dsResult = null;
            RDBConnectionManager mDBConn = null;
            try
            {
                htParams = new RHashlist();
                dsResult = new DataSet();
                lstOutputFields = new List<string>();
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                htParams.Add(RBbgBulkListConstant.BULKLISTMAPID, bulkListMapId);
                dsResult = mDBConn.ExecuteQuery("RAD:GetBulkListInfobyId", htParams);
                if (dsResult != null && (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0))
                {
                    string[] fields = dsResult.Tables[0].Rows[0][RBbgBulkListConstant.OUTPUTFIELDS].ToString().Split(',');
                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (!lstOutputFields.Contains(fields[i]))
                            lstOutputFields.Add(fields[i]);
                    }
                }
            }
            catch (RVendorException ex)
            {
                throw new RVendorException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new RVendorException(ex.Message);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                if (htParams != null)
                    htParams = null;
                if (dsResult != null)
                    dsResult = null;
                mLogger.Debug("Start -> getting ouptut fields.");
            }
            return lstOutputFields;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the bulk list output fields.
        /// </summary>
        public static DataSet GetBulkListRequestFields(RBbgMarketSector marketSector)
        {
            mLogger.Debug("Start -> getting ouptut fields.");
            RHashlist htParams = null;
            DataSet dsResult = null;
            RDBConnectionManager mDBConn = null;
            try
            {
                htParams = new RHashlist();
                dsResult = new DataSet();
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                htParams.Add(RBbgBulkListConstant.MARKETSECTOR, marketSector);
                dsResult = mDBConn.ExecuteQuery("RAD:GetBulkListFieldByMarketSector", htParams);
            }
            catch (RVendorException ex)
            {
                throw new RVendorException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new RVendorException(ex.Message);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("Start -> getting ouptut fields.");
            }
            return dsResult;
        }

        #endregion
        //------------------------------------------------------------------------------------------
        #region Internal Methods
        /// <summary>
        /// Removes the invalid fields.
        /// </summary>
        internal static SecuritiesCollection RemoveInvalidFields(SecuritiesCollection responseSecurities, ref SecuritiesCollection dictUnprocessedDict, bool requireNotAvailableInField)
        {
            #region "Normalize Response Securities"

            List<string> instrumentsToBeDeleted = new List<string>();
            bool fieldHasValue = false;
            foreach (string instrument in responseSecurities.Keys)
            {
                fieldHasValue = false;
                foreach (RVendorFieldInfo fldInfo in responseSecurities[instrument].Values)
                {
                    if (RBbgUtils.IsInValidField(fldInfo.Value))
                    {
                        fldInfo.Value = requireNotAvailableInField ? RVendorConstant.NOT_AVAILABLE : string.Empty;
                        fldInfo.Status = RVendorStatusConstant.FAILED;
                    }
                    if (fldInfo.Status == RVendorStatusConstant.PASSED && !fieldHasValue)
                    {
                        fieldHasValue = true;
                    }
                }
                if (!fieldHasValue)
                {
                    instrumentsToBeDeleted.Add(instrument);
                }
            }

            foreach (string inst in instrumentsToBeDeleted)
            {
                if (dictUnprocessedDict.ContainsKey(inst))
                {
                    Dictionary<string, RVendorFieldInfo> listVendorFieldInfo = responseSecurities[inst];
                    foreach (var fieldInfoKeyVal in responseSecurities[inst])
                    {
                        listVendorFieldInfo.Add(fieldInfoKeyVal.Key, fieldInfoKeyVal.Value);
                    }

                    dictUnprocessedDict[inst] = listVendorFieldInfo;
                }
                else
                    dictUnprocessedDict[inst] = responseSecurities[inst];
                responseSecurities.Remove(inst);

            }
            #endregion
            return responseSecurities;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts the history.
        /// </summary>
        /// <param name="info">The info.</param>
        internal static int InsertHistory(RVendorHistoryInfo info)
        {
            mLogger.Debug("Start->Insert into vendor History");
            RDBConnectionManager mDBConn = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
            RHashlist mList = new RHashlist();
            mList.Add("vendor_name", info.VendorName);
            mList.Add("request_type", info.RequestType);
            mList.Add("requested_instruments", info.VendorInstruments);
            mList.Add("requested_fields", info.VendorFields);
            mList.Add("request_identifier", info.requestIdentifier);
            mList.Add("time_stamp", info.TimeStamp);
            mList.Add("request_status", info.RequestStatus);
            mList.Add("user_login_name", info.UserLoginName);
            mList.Add("is_bulk_list", info.IsBulkList);
            mList.Add("module_id", info.ModuleId);
            // mList.Add("market_sector", info.MarketSector);
            // mList.Add("request_module", "Normal");
            try
            {
                return Convert.ToInt32(mDBConn.ExecuteQuery("RAD:InsertVendorHistory", mList, true).Tables[0].Rows[0]["ID"]);
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("End->Insert into vendor History");
            }
        }
        //------------------------------------------------------------------------------------------
        internal static int GetUUID(string userName)
        {
            mLogger.Debug("Start->getting uuid");
            RDBConnectionManager mDBConn = null;
            //RHashlist mList = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            DataSet dsResult = null;
            try
            {
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                //mList = new RHashlist();
                dsResult = new DataSet();
                //mList.Add(RBbgUUIDConstant.USERNAME, userName);
                dsResult = mDBConn.ExecuteQuery(string.Format(@"select *
                from dbo.ivp_rad_bbg_uuid_mapping uuid
                where uuid.is_active = 1 AND uuid.user_name = '{0}'", userName), RQueryType.Select);

                if (dsResult != null && (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0))
                    return Convert.ToInt32(dsResult.Tables[0].Rows[0][RBbgUUIDConstant.SAPIUUID]);
                else
                    return 0;
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("End->getting uuid");
            }
        }

        internal static string GetEMRSID(string userName)
        {
            mLogger.Debug("Start->getting uuid");
            RDBConnectionManager mDBConn = null;
            RHashlist mList = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            DataSet dsResult = null;
            try
            {
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                mList = new RHashlist();
                dsResult = new DataSet();
                mList.Add(RBbgUUIDConstant.USERNAME, userName);
                dsResult = mDBConn.ExecuteQuery("RAD:GetManagedBpipeEmrsid", mList, true);
                if (dsResult != null && (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0))
                    return dsResult.Tables[0].Rows[0][RBbgUUIDConstant.EMRSID].ToString();
                else
                    return "";
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("End->getting uuid");
            }
        }

        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the bulk list info.
        /// </summary>
        internal static RBbgBulkListInfo GetBulkListInfo(int bulkListMapId)
        {
            mLogger.Debug("Start->getting bulk list info by id");
            RDBConnectionManager mDBConn = null;
            RHashlist mList = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            DataSet dsResult = null;
            RBbgBulkListInfo bulkListInfo = null;
            try
            {
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                mList = new RHashlist();
                dsResult = new DataSet();
                mList.Add(RBbgBulkListConstant.BULKLISTMAPID, bulkListMapId);
                dsResult = mDBConn.ExecuteQuery("RAD:GetBulkListInfobyId", mList, true);
                if (dsResult != null && (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0))
                {
                    bulkListInfo = new RBbgBulkListInfo();
                    bulkListInfo.BulkListMapId = Convert.ToInt32(dsResult.Tables[0].
                                    Rows[0][RBbgBulkListConstant.BULKLISTMAPID].ToString());
                    bulkListInfo.RequestField = dsResult.Tables[0].
                                    Rows[0][RBbgBulkListConstant.REQUESTFIELD].ToString();
                    bulkListInfo.OutputFields = dsResult.Tables[0].
                                    Rows[0][RBbgBulkListConstant.OUTPUTFIELDS].ToString();
                    bulkListInfo.CreatedBy = dsResult.Tables[0].
                                    Rows[0][RBbgBulkListConstant.CREATEDBY].ToString();
                    bulkListInfo.MarketSector = dsResult.Tables[0].
                                    Rows[0][RBbgBulkListConstant.MARKETSECTOR].ToString();
                }
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("End->getting bulk list info by id");
            }
            return bulkListInfo;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the vendor history.
        /// </summary>
        internal static List<RVendorHistoryInfo> GetVendorHistory(string indentifier)
        {
            mLogger.Debug("Start->Insert into vendor History");
            RDBConnectionManager mDBConn = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
            try
            {
                List<RVendorHistoryInfo> historyList = new List<RVendorHistoryInfo>();
                DataSet ds = mDBConn.ExecuteQuery("select vh.*,pd.ID as vendor_pricing_id from dbo.ivp_rad_vendor_history vh with (nolock) inner join dbo.ivp_rad_vendor_pricing_details pd with (nolock) ON pd.time_stamp = vh.time_stamp where vh.request_identifier = '" + indentifier + "'", RQueryType.Select);
                RVendorHistoryInfo historyInfo = null;
                if (ds != null && (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0))
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        historyInfo = new RVendorHistoryInfo();
                        historyInfo.VendorHistoryId = Convert.ToInt32(dr["vendor_history_id"].ToString());
                        historyInfo.VendorName = Convert.ToString(dr["vendor_name"].ToString());
                        historyInfo.RequestType = Convert.ToString(dr["request_type"].ToString());
                        historyInfo.VendorInstruments = Convert.ToString(dr["requested_instruments"].ToString());
                        historyInfo.VendorFields = Convert.ToString(dr["requested_fields"].ToString());
                        historyInfo.requestIdentifier = Convert.ToString(dr["request_identifier"].ToString());
                        historyInfo.TimeStamp = Convert.ToString(dr["time_stamp"].ToString());
                        historyInfo.RequestStatus = Convert.ToString(dr["request_status"].ToString());
                        historyInfo.UserLoginName = Convert.ToString(dr["user_login_name"].ToString());
                        historyInfo.RequestedOn = Convert.ToDateTime(dr["requested_on"].ToString());
                        historyInfo.ExceptionMessage = Convert.ToString(dr["requested_on"].ToString());
                        historyInfo.IsBulkList = Convert.ToBoolean(dr["is_bulk_list"].ToString());
                        if (dr["module_id"] != null)
                            historyInfo.ModuleId = Convert.ToInt32(dr["module_id"].ToString());
                        historyInfo.VendorPricingId = dr.IsNull("vendor_pricing_id") ? 0 : dr.Field<int>("vendor_pricing_id");
                        historyList.Add(historyInfo);
                    }
                }
                return historyList;
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("End->Insert into vendor History");
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the formatted string.
        /// </summary>
        internal static string GetFormattedString(string value)
        {
            StringBuilder strBuilder = new StringBuilder();
            string[] strArr = value.Split(',');
            for (int i = 0; i < strArr.Length; i++)
            {
                if (strArr[i] != string.Empty)
                {
                    strBuilder.Append(",");
                    strBuilder.Append("'");
                    strBuilder.Append(strArr[i]);
                    strBuilder.Append("'");
                }
            }
            if (strBuilder != null && strBuilder.Length > 0)
                strBuilder.Remove(0, 1);
            return strBuilder.ToString();
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the vendor history exception.
        /// </summary>
        internal static void UpdateVendorHistoryException(string timestamp, string message)
        {
            mLogger.Debug("Start->Insert into vendor History");
            RDBConnectionManager mDBConn = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
            try
            {
                mDBConn.ExecuteQuery("Update ivp_rad_vendor_history set exception_message='" +
                    message + "'" + ",request_status ='" + RVendorStatusConstant.FAILED +
                    "' where time_stamp = '" + timestamp + "'", RQueryType.Update);
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("End->Insert into vendor History");
            }
        }


        internal static RVendorResponse ProcessResponseForBulkList(RSecurityReturnType returnType, object response, object unprocessed,
            RBbgSecurityInfo secInfo, string requestIdentifier, List<string> AdditionalInfo = null, List<string> ResponseAdditionalInfo = null)
        {
            RVendorResponse vendorResponse = new RVendorResponse();
            List<SecuritiesCollection> listResponseSecurities = null;
            SecuritiesCollection dictUnProcessedSecurities = null;
            DataSet ds = new DataSet();
            string[] fldNames = null;
            if (response is List<SecuritiesCollection>)
            {
                if (response != null)
                    listResponseSecurities = (List<SecuritiesCollection>)response;
                if (unprocessed != null)
                    dictUnProcessedSecurities = (SecuritiesCollection)unprocessed;
                if (listResponseSecurities.Count != 0)
                {
                    for (int key = 0; key < listResponseSecurities.Count; key++)
                    {
                        DataTable dtResult = RVendorUtils.ConvertToDataSetForBulkList
                                            (listResponseSecurities[key],
                                            dictUnProcessedSecurities, secInfo);

                        if (dtResult != null && dtResult.Rows.Count > 0)
                        {
                            List<string> processedSec = new List<string>();
                            //bool isDataSetNull = false;
                            for (int counter = 0; counter < dtResult.Rows.Count; counter++)
                            {
                                object[] objArray = dtResult.Rows[counter].ItemArray;
                                if (!processedSec.Contains(objArray[0].ToString()))
                                {
                                    processedSec.Add(objArray[0].ToString());
                                    bool isDataPresent = false;
                                    for (int count = 3; count < dtResult.Columns.Count; count++)
                                    {
                                        if (!Convert.ToString(objArray[count]).Equals("0") && !Convert.ToString(objArray[count]).Equals("1") && !Convert.ToString(objArray[count]).Equals("Not Available") && !string.IsNullOrEmpty(Convert.ToString(objArray[count])))
                                        {
                                            isDataPresent = true;
                                            break;
                                        }
                                    }
                                    if (!isDataPresent)
                                    {
                                        dtResult.Rows.RemoveAt(counter);
                                        counter--;
                                    }
                                    else
                                    {
                                        //Rename Columns 
                                        int id = Convert.ToInt32(dtResult.TableName);
                                        RBbgBulkListInfo bulkListInfo = RVendorUtils.GetBulkListInfo(id);
                                        fldNames = bulkListInfo.OutputFields.Split(',');
                                        if ((dtResult != null && dtResult.Rows.Count > 0))
                                        {
                                            foreach (DataColumn dc in dtResult.Columns)
                                            {
                                                mLogger.Error("column name=> " + dc.ColumnName);
                                                if (dc.Ordinal >= 3)
                                                    dc.ColumnName = fldNames[dc.Ordinal - 3];
                                            }
                                        }

                                    }
                                }
                            }
                            ds.Tables.Add(dtResult);
                        }
                    }
                    if (fldNames == null)
                        fldNames = new string[] { };
                    foreach (DataTable resp in ds.Tables)
                    {
                        if (resp.Columns.Count > fldNames.Length + 3)
                        {
                            for (int count = fldNames.Length + 3; count < resp.Columns.Count; count++)
                            {
                                resp.Columns.RemoveAt(count);
                                count--;
                            }
                        }
                    }
                    vendorResponse.SecurityResponse = ds;
                    if (listResponseSecurities.Count > 0)
                        vendorResponse.ResponseStatus = RVendorResponseStatus.Passed;
                    else
                        vendorResponse.ResponseStatus = RVendorResponseStatus.Failed;
                }
            }
            else
            {
                vendorResponse.SecurityResponse = response;
                vendorResponse.ResponseStatus = RVendorResponseStatus.RequestRegistered;
            }
            vendorResponse.RequestIdentifier = requestIdentifier;
            vendorResponse.ExceptionMessage = "";
            vendorResponse.AdditionalInfo = AdditionalInfo;
            vendorResponse.ResponseAdditionalInfo = ResponseAdditionalInfo;
            return vendorResponse;
        }

        /// <summary>
        /// Processes the response for bulk list.
        /// </summary>
        internal static RVendorResponse ProcessResponseForBulkList(RSecurityReturnType returnType, object response, object unprocessed
            , string fieldNames, RBbgSecurityInfo secInfo, string requestIdentifier)
        {
            RVendorResponse vendorResponse = new RVendorResponse();
            List<SecuritiesCollection> listResponseSecurities = null;
            SecuritiesCollection dictUnProcessedSecurities = null;
            DataSet ds = new DataSet();
            string[] fldNames = fieldNames.Split(',');
            if (response is List<SecuritiesCollection>)
            {
                if (response != null)
                    listResponseSecurities = (List<SecuritiesCollection>)response;
                if (unprocessed != null)
                    dictUnProcessedSecurities = (SecuritiesCollection)unprocessed;
                if (listResponseSecurities.Count != 0)
                {
                    //ds = RVendorUtils.ConvertToDataSetForBulkList
                    //        (listResponseSecurities[0], dictUnProcessedSecurities,secInfo);

                    DataTable dtResult = RVendorUtils.ConvertToDataSetForBulkList
                            (listResponseSecurities[0], dictUnProcessedSecurities, secInfo);

                    #region Check For Valid Response

                    if ((ds != null && ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows.Count == 1))
                    {
                        bool isDataSetNull = false;
                        object[] objArray = ds.Tables[0].Rows[0].ItemArray;
                        foreach (object obj in objArray)
                        {
                            if (obj.ToString().Equals("0") ||
                                obj.ToString().Equals("1") ||
                                obj.ToString().Equals("Not Available"))
                                isDataSetNull = true;
                        }
                        if (isDataSetNull)
                            ds = new DataSet();
                    }
                    #endregion
                    if ((ds != null && ds.Tables.Count > 0))
                    {
                        foreach (DataColumn dc in ds.Tables[0].Columns)
                        {
                            if (dc.Ordinal >= 3)
                                dc.ColumnName = fldNames[dc.Ordinal - 3];
                        }
                    }
                }
                vendorResponse.SecurityResponse = ds;
                if (listResponseSecurities.Count > 0)
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Passed;
                else
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Failed;
                vendorResponse.RequestIdentifier = requestIdentifier;
            }
            else
            {
                vendorResponse.SecurityResponse = response;
                vendorResponse.RequestIdentifier = requestIdentifier;
                vendorResponse.ResponseStatus = RVendorResponseStatus.RequestRegistered;
            }
            vendorResponse.ExceptionMessage = "";
            return vendorResponse;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Processes the response.
        /// </summary>
        internal static RVendorResponse ProcessResponse(string requestIdentifier, List<string> AdditionalInfo = null, List<string> ResponseAdditionalInfo = null)
        {
            RVendorResponse vendorResponse = new RVendorResponse();

            vendorResponse.SecurityResponse = requestIdentifier;
            vendorResponse.RequestIdentifier = requestIdentifier;
            vendorResponse.ResponseStatus = RVendorResponseStatus.RequestRegistered;
            vendorResponse.ExceptionMessage = "";

            vendorResponse.AdditionalInfo = AdditionalInfo;
            vendorResponse.ResponseAdditionalInfo = ResponseAdditionalInfo;
            return vendorResponse;
        }

        internal static RVendorResponse ProcessResponseSecurities(List<SecuritiesCollection> listResponseSecurities, RBbgSecurityInfo securityInfo, ref SecuritiesCollection unProcessedResponse, RSecurityReturnType returnType, string requestIdentifier, bool requireNotAvailableInField, int VendorPreferenceId, RVendorType vendorType, List<string> AdditionalInfo = null, List<string> ResponseAdditionalInfo = null)
        {
            RVendorResponse vendorResponse = new RVendorResponse();

            ExecuteCustomClass(listResponseSecurities.FirstOrDefault(), VendorPreferenceId, vendorType);

            if (returnType == RSecurityReturnType.DataSet)
            {
                var dsResult = new DataSet();
                var dtResult = new DataTable("SecurityInfo");
                DataColumn dcInstrument = new DataColumn("INSTRUMENT", Type.GetType("System.String"));
                DataColumn dcIsValid = new DataColumn("is_valid", Type.GetType("System.String"));
                DataColumn dcFailureReason = new DataColumn("failure_reason", Type.GetType("System.String"));
                dtResult.Columns.Add(dcInstrument);
                dtResult.Columns.Add(dcIsValid);
                dtResult.Columns.Add(dcFailureReason);

                var lstFieldNames = new HashSet<string>(securityInfo.InstrumentFields.Select(x => x.Mnemonic));
                if (listResponseSecurities != null && listResponseSecurities.Count > 0)
                {
                    var lstFieldNamesRes = listResponseSecurities.SelectMany(x => x.FirstOrDefault().Value.Values.Select(y => y.Name).ToList()).ToList();
                    foreach (string fieldName in lstFieldNamesRes)
                    {
                        lstFieldNames.Add(fieldName);
                    }
                }

                foreach (string fieldName in lstFieldNames)
                {
                    DataColumn dc = new DataColumn(fieldName.ToUpper(), typeof(string));
                    if (requireNotAvailableInField)
                        dc.DefaultValue = RVendorConstant.NOT_AVAILABLE;
                    dtResult.Columns.Add(dc);
                }

                foreach (RBbgInstrumentInfo info in securityInfo.Instruments)
                {
                    DataRow drRow = dtResult.NewRow();

                    string instrument = info.InstrumentID;
                    drRow["INSTRUMENT"] = instrument;

                    bool hasInstrument = false;
                    foreach (var response in listResponseSecurities)
                    {
                        if (response.ContainsKey(instrument))
                        {
                            hasInstrument = true;

                            foreach (var fieldInfo in response[instrument].Values)
                            {
                                drRow[fieldInfo.Name] = fieldInfo.Value;
                            }
                        }
                    }

                    if (hasInstrument)
                    {
                        drRow["is_valid"] = RVendorStatusConstant.PASSED;
                        dtResult.Rows.Add(drRow);
                    }
                    else
                    {
                        drRow["is_valid"] = RVendorStatusConstant.FAILED;
                        drRow["failure_reason"] = RVendorConstant.NOT_PROCESSED;
                    }
                }

                if (listResponseSecurities.Count > 0)
                {
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Passed;
                    vendorResponse.ExceptionMessage = "";
                }
                else
                {
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Failed;
                    vendorResponse.ExceptionMessage = "No data available from vendor";
                }

                dsResult.Tables.Add(dtResult);
                vendorResponse.SecurityResponse = dsResult;
            }
            else
            {
                SecuritiesCollection normalizedResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);

                if (listResponseSecurities.Count > 0)
                {
                    normalizedResponse = listResponseSecurities[0];
                }

                if (listResponseSecurities.Count > 1)
                {
                    for (int i = 1; i < listResponseSecurities.Count; i++)
                    {
                        var response = listResponseSecurities[i];
                        foreach (string instrument in response.Keys)
                        {
                            if (normalizedResponse.ContainsKey(instrument))
                            {
                                var dictVendorFieldInfo = normalizedResponse[instrument];

                                foreach (var fieldKeyVal in response[instrument])
                                {
                                    if (!dictVendorFieldInfo.ContainsKey(fieldKeyVal.Key))
                                        dictVendorFieldInfo.Add(fieldKeyVal.Key, fieldKeyVal.Value);
                                }
                                normalizedResponse[instrument] = dictVendorFieldInfo;
                            }
                            else
                                normalizedResponse[instrument] = response[instrument];
                        }
                    }
                }

                foreach (RBbgInstrumentInfo info in securityInfo.Instruments)
                {
                    if (securityInfo.InstrumentFields.Count > 0)
                    {
                        string instrument = info.InstrumentID;
                        if (normalizedResponse.ContainsKey(instrument))
                        {
                            var processedFieldInfo = normalizedResponse[instrument];
                            foreach (RBbgFieldInfo bbgFldInfo in securityInfo.InstrumentFields)
                            {
                                if (processedFieldInfo.ContainsKey(bbgFldInfo.Mnemonic))
                                {
                                    if (bbgFldInfo.IsSystemAdded)
                                        processedFieldInfo.Remove(bbgFldInfo.Mnemonic);
                                }
                                else
                                {
                                    RVendorFieldInfo vendorFldInfo = new RVendorFieldInfo();
                                    vendorFldInfo.Name = bbgFldInfo.Mnemonic;
                                    vendorFldInfo.Status = RVendorStatusConstant.FAILED;
                                    vendorFldInfo.Value = RVendorConstant.NOT_AVAILABLE;
                                    normalizedResponse[instrument].Add(vendorFldInfo.Name, vendorFldInfo);
                                }
                            }
                        }
                        else
                        {

                            unProcessedResponse.Add(instrument, securityInfo.InstrumentFields.ToDictionary(x => x.Mnemonic, y => new RVendorFieldInfo
                            {
                                Name = y.Mnemonic,
                                Status = RVendorStatusConstant.FAILED,
                                Value = RVendorConstant.NOT_AVAILABLE,
                                ExceptionMessage = RVendorConstant.NOT_PROCESSED
                            }, StringComparer.OrdinalIgnoreCase));

                        }
                    }
                }

                vendorResponse.SecurityResponse = normalizedResponse;

                if (normalizedResponse.Count > 0)
                {
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Passed;
                    vendorResponse.ExceptionMessage = "";
                }
                else
                {
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Failed;
                    vendorResponse.ExceptionMessage = "No data available from vendor";
                }
            }

            vendorResponse.RequestIdentifier = requestIdentifier;

            vendorResponse.AdditionalInfo = AdditionalInfo;
            vendorResponse.ResponseAdditionalInfo = ResponseAdditionalInfo;
            return vendorResponse;
        }

        private static void ExecuteCustomClass(SecuritiesCollection listResponseSecurities, int VendorPreferenceId, RVendorType vendorType)
        {
            mLogger.Debug("ExecuteCustomClass ==> Start");
            RDBConnectionManager mDBConn = null;

            try
            {
                if (listResponseSecurities == null)
                {
                    mLogger.Error("listResponseSecurities is null");
                    return;
                }
                else
                    mLogger.Error("listResponseSecurities count - " + listResponseSecurities.Count);

                int externalVendorId = 0;
                if (vendorType == RVendorType.Bloomberg)
                    externalVendorId = 1;
                else if (vendorType == RVendorType.Reuters)
                    externalVendorId = 2;

                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);

                DataTable dtRules = mDBConn.ExecuteQuery(DALWrapperAppend.Replace(string.Format(@"SELECT class_name, assembly_path, exec_sequence, vendor_management_id
                FROM IVPRefMaster.dbo.ivp_srm_bbg_custom_class
                WHERE is_active = 1 AND (vendor_management_id = -1 OR vendor_management_id = {0}) AND external_vendor_id = {1}", VendorPreferenceId, externalVendorId)), RQueryType.Select).Tables[0];

                if (dtRules != null && dtRules.Rows.Count > 0)
                {
                    RHashlist param = new RHashlist();
                    param.Add("data", listResponseSecurities);
                    foreach (DataRow drRule in dtRules.AsEnumerable().OrderBy(x => Convert.ToInt32(x["vendor_management_id"])).ThenBy(x => Convert.ToInt32(x["exec_sequence"])))
                    {
                        string assemblyPath = Convert.ToString(drRule["assembly_path"]);
                        string className = Convert.ToString(drRule["class_name"]);

                        mLogger.Error("Executing bbg custom class with assemblyPath : (" + assemblyPath + "), className : (" + className + ")");

                        RCustomCallExecutor.ExecuteCustomCall(assemblyPath, className, param);
                    }
                }
                else
                    mLogger.Debug("No custom class configured");
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("ExecuteCustomClass ==> End");
            }
        }

        internal static bool IsRealtimeDebugMode()
        {
            mLogger.Debug("IsRealtimeDebugMode -> Start");
            try
            {
                DataSet ds = ExecuteSelectQuery(DALWrapperAppend.Replace(@"DECLARE @script VARCHAR(MAX) = ''

                IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'IVPSecMaster')
	                SELECT @script = 'IF EXISTS (SELECT 1 FROM IVPSecMaster.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''ivp_secm_temp_table_config_master'')
	                SELECT functionality, persist_tables
	                FROM IVPSecMaster.dbo.ivp_secm_temp_table_config_master
	                WHERE functionality = ''RealtimeLegDebugger'''

                EXEC (@script)"), "RADDBConnectionId");

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    mLogger.Debug("IsRealtimeDebugMode -> Entry not found in database for functionality - RealtimeLegDebugger");
                    return false;
                }
                else
                    return Convert.ToBoolean(ds.Tables[0].Rows[0]["persist_tables"]);
            }
            catch (Exception ex)
            {
                mLogger.Error("IsRealtimeDebugMode -> Error occured while fetching entry for functionality - " + ex.ToString());
                return false;
            }
            finally
            {
                mLogger.Debug("IsRealtimeDebugMode -> End");
            }
        }

        internal static RVendorResponse ProcessResponseSecurities(List<SecuritiesCollection> listResponseSecurities, RBbgSecurityInfo securityInfo, ref SecuritiesCollection unProcessedResponse, RSecurityReturnType returnType, string requestIdentifier, bool requireNotAvailableInField, ref DataSet dsResponseData, ref object requestedBbgSecurities, List<string> lstBulkMnemonicsInMasterReq, Dictionary<string, List<string>> lstBulkMnemonicsInLegReq, int VendorPreferenceId, RVendorType vendorType, List<string> AdditionalInfo = null, List<string> ResponseAdditionalInfo = null)
        {
            RVendorResponse vendorResponse = new RVendorResponse();

            ExecuteCustomClass(listResponseSecurities.FirstOrDefault(), VendorPreferenceId, vendorType);

            if (returnType == RSecurityReturnType.DataSet)
            {
                //var dsResult = new DataSet();
                //var dtResult = new DataTable("SecurityInfo");

                if (!dsResponseData.Tables[0].Columns.Contains("INSTRUMENT"))
                {
                    DataColumn dcInstrument = new DataColumn("INSTRUMENT", Type.GetType("System.String"));
                    dsResponseData.Tables[0].Columns.Add(dcInstrument);
                }

                if (!dsResponseData.Tables[0].Columns.Contains("is_valid"))
                {
                    DataColumn dcIsValid = new DataColumn("is_valid", Type.GetType("System.String"));
                    dsResponseData.Tables[0].Columns.Add(dcIsValid);
                }

                if (!dsResponseData.Tables[0].Columns.Contains("failure_reason"))
                {
                    DataColumn dcFailureReason = new DataColumn("failure_reason", Type.GetType("System.String"));
                    dsResponseData.Tables[0].Columns.Add(dcFailureReason);
                }

                var lstBulkMnemonicsLegOnly = lstBulkMnemonicsInLegReq.Keys.Except(lstBulkMnemonicsInMasterReq, StringComparer.OrdinalIgnoreCase).ToList();

                var lstFieldNames = new HashSet<string>(securityInfo.InstrumentFields.Select(x => x.Mnemonic));
                var lstResponseSecuritiesVsFilteredFields = new List<KeyValuePair<SecuritiesCollection, List<List<string>>>>();
                if (listResponseSecurities != null && listResponseSecurities.Count > 0)
                {
                    var lstFieldNamesRes = listResponseSecurities.SelectMany(x => x.FirstOrDefault().Value.Values.Select(y => y.Name).ToList()).ToList();
                    foreach (string fieldName in lstFieldNamesRes)
                    {
                        lstFieldNames.Add(fieldName);
                    }

                    lstFieldNames = new HashSet<string>(lstFieldNames.Except(lstBulkMnemonicsLegOnly, StringComparer.OrdinalIgnoreCase));

                    for (int i = 0; i < listResponseSecurities.Count; i++)
                    {
                        var responseSecurities = listResponseSecurities[i];
                        var lstFieldsPerResponse = responseSecurities.Values.FirstOrDefault().Values.Select(y => y.Name).ToList();

                        var commonFields = lstFieldsPerResponse.Intersect(lstBulkMnemonicsInLegReq.Keys, StringComparer.OrdinalIgnoreCase).ToList();
                        lstFieldsPerResponse = lstFieldsPerResponse.Except(lstBulkMnemonicsLegOnly, StringComparer.OrdinalIgnoreCase).ToList();

                        lstResponseSecuritiesVsFilteredFields.Add(new KeyValuePair<SecuritiesCollection, List<List<string>>>(responseSecurities, new List<List<string>> { lstFieldsPerResponse, commonFields }));
                    }
                }

                foreach (string fieldName in lstFieldNames)
                {
                    DataColumn dc = new DataColumn(fieldName.ToUpper(), typeof(string));
                    if (requireNotAvailableInField)
                        dc.DefaultValue = RVendorConstant.NOT_AVAILABLE;
                    if (!dsResponseData.Tables[0].Columns.Contains(dc.ColumnName))
                    {
                        dsResponseData.Tables[0].Columns.Add(dc);
                    }
                }

                bool isSingleIdentifier = true;
                if (requestedBbgSecurities is List<RBbgSecurityInfo>)
                    isSingleIdentifier = false;

                var instrumentVsSecInfo = isSingleIdentifier ? new Dictionary<string, RBbgSecurityInfo>(StringComparer.OrdinalIgnoreCase) : ((List<RBbgSecurityInfo>)requestedBbgSecurities).ToDictionary(x => x.Instruments[0].InstrumentID, x => x, StringComparer.OrdinalIgnoreCase);//((RBbgSecurityInfo)requestedBbgSecurities).in

                bool IsRealtimeDebugMode = RVendorUtils.IsRealtimeDebugMode();

                foreach (RBbgInstrumentInfo info in securityInfo.Instruments)
                {
                    string instrument = info.InstrumentID;

                    DataRow drRow = dsResponseData.Tables[0].NewRow();
                    drRow["INSTRUMENT"] = instrument;
                    drRow["Request Identifier"] = requestIdentifier;
                    drRow["Security ID"] = string.Empty;
                    //RBbgSecurityInfo requestedBbgSecInfo = null;//todo:verify from request task identifier ranking if security id is empty or not
                    //if (isSingleIdentifier)
                    //{
                    //    drRow["Security ID"] = instrument;
                    //}
                    //else
                    //{
                    //    requestedBbgSecInfo = instrumentVsSecInfo[instrument];
                    //    drRow["Security ID"] = requestedBbgSecInfo.SecurityId;
                    //}

                    bool hasInstrument = false;
                    foreach (var responseKeyValPair in lstResponseSecuritiesVsFilteredFields)
                    {
                        var response = responseKeyValPair.Key;
                        if (response.ContainsKey(instrument))
                        {
                            var masterFields = responseKeyValPair.Value[0];
                            var legFields = responseKeyValPair.Value[1];

                            hasInstrument = true;

                            var securityResponse = response[instrument];
                            foreach (string fieldName in masterFields)
                            {
                                if (securityResponse.ContainsKey(fieldName))
                                    drRow[fieldName] = securityResponse[fieldName].Value;
                            }

                            foreach (string bulkMnemonic in legFields)
                            {
                                mLogger.Error("Processing bulkMnemonic : " + bulkMnemonic);
                                var bulk = securityResponse[bulkMnemonic].Value;
                                if (!string.IsNullOrWhiteSpace(bulk))
                                {
                                    var contents = bulk.Split(';');
                                    int rowCount = Convert.ToInt32(contents[2]);
                                    if (rowCount > 0)
                                    {
                                        var reponseLegData = dsResponseData.Tables[bulkMnemonic];

                                        var bulkMnemonicColNames = lstBulkMnemonicsInLegReq[bulkMnemonic];

                                        int noOfFields = Convert.ToInt32(contents[3]);
                                        for (int index = 5; index < contents.Length; index = index + noOfFields * 2)
                                        {
                                            DataRow drLegData = reponseLegData.NewRow();
                                            drLegData["INSTRUMENT"] = instrument;

                                            int fieldCounter = 0;
                                            while (fieldCounter < noOfFields)
                                            {
                                                string colValue = contents[index + fieldCounter * 2];
                                                string colName = bulkMnemonicColNames[fieldCounter];
                                                drLegData[colName] = RBbgUtils.IsInValidField(colValue) ? string.Empty : colValue;
                                                fieldCounter++;
                                            }

                                            drLegData["is_valid"] = "PASSED";
                                            reponseLegData.Rows.Add(drLegData);
                                        }
                                    }
                                }

                                if (IsRealtimeDebugMode)
                                {
                                    if (dsResponseData != null && dsResponseData.Tables.Count > 0 && dsResponseData.Tables.Contains(bulkMnemonic) && dsResponseData.Tables[bulkMnemonic].Rows.Count > 0)
                                    {
                                        StringWriter writer = new StringWriter();

                                        try
                                        {
                                            dsResponseData.Tables[bulkMnemonic].WriteXml(writer);
                                            mLogger.Error("Leg data for leg after fetch (" + dsResponseData.Tables[bulkMnemonic].TableName + ") : " + writer.ToString());
                                        }
                                        catch (Exception ex)
                                        {
                                            mLogger.Error("Exception occured while printing Leg data for leg after fetch (" + dsResponseData.Tables[bulkMnemonic].TableName + ") : " + ex.ToString());
                                        }
                                    }
                                    else
                                        mLogger.Error("Leg data for leg after fetch (" + dsResponseData.Tables[bulkMnemonic].TableName + ") is blank");
                                }
                            }

                            //foreach (var fieldInfo in response[instrument].Values)
                            //{
                            //    drRow[fieldInfo.Name] = fieldInfo.Value;
                            //}
                        }
                    }

                    if (hasInstrument)
                    {
                        drRow["is_valid"] = RVendorStatusConstant.PASSED;
                        dsResponseData.Tables[0].Rows.Add(drRow);
                        if (!isSingleIdentifier)
                            instrumentVsSecInfo.Remove(info.InstrumentID);
                    }
                    else
                    {
                        drRow["is_valid"] = RVendorStatusConstant.FAILED;
                        drRow["failure_reason"] = RVendorConstant.NOT_PROCESSED;
                        if (!isSingleIdentifier)
                        {
                            var requestedBbgSecInfo = instrumentVsSecInfo[instrument];
                            requestedBbgSecInfo.Instruments.RemoveAt(0);
                        }
                    }
                }

                if (!isSingleIdentifier)
                    requestedBbgSecurities = instrumentVsSecInfo.Values.ToList();

                if (listResponseSecurities.Count > 0)
                {
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Passed;
                    vendorResponse.ExceptionMessage = "";
                }
                else
                {
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Failed;
                    vendorResponse.ExceptionMessage = "No data available from vendor";
                }

                //dsResult.Tables.Add(dtResult);
                //vendorResponse.SecurityResponse = dsResult;
            }
            else
            {
                SecuritiesCollection normalizedResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);

                if (listResponseSecurities.Count > 0)
                {
                    normalizedResponse = listResponseSecurities[0];
                }

                if (listResponseSecurities.Count > 1)
                {
                    for (int i = 1; i < listResponseSecurities.Count; i++)
                    {
                        var response = listResponseSecurities[i];
                        foreach (string instrument in response.Keys)
                        {
                            if (normalizedResponse.ContainsKey(instrument))
                            {
                                var dictVendorFieldInfo = normalizedResponse[instrument];

                                foreach (var fieldKeyVal in response[instrument])
                                {
                                    if (!dictVendorFieldInfo.ContainsKey(fieldKeyVal.Key))
                                        dictVendorFieldInfo.Add(fieldKeyVal.Key, fieldKeyVal.Value);
                                }
                                normalizedResponse[instrument] = dictVendorFieldInfo;
                            }
                            else
                                normalizedResponse[instrument] = response[instrument];
                        }
                    }
                }

                foreach (RBbgInstrumentInfo info in securityInfo.Instruments)
                {
                    if (securityInfo.InstrumentFields.Count > 0)
                    {
                        string instrument = info.InstrumentID;
                        if (normalizedResponse.ContainsKey(instrument))
                        {
                            var processedFieldInfo = normalizedResponse[instrument];
                            foreach (RBbgFieldInfo bbgFldInfo in securityInfo.InstrumentFields)
                            {
                                if (processedFieldInfo.ContainsKey(bbgFldInfo.Mnemonic))
                                {
                                    if (bbgFldInfo.IsSystemAdded)
                                        processedFieldInfo.Remove(bbgFldInfo.Mnemonic);
                                }
                                else
                                {
                                    RVendorFieldInfo vendorFldInfo = new RVendorFieldInfo();
                                    vendorFldInfo.Name = bbgFldInfo.Mnemonic;
                                    vendorFldInfo.Status = RVendorStatusConstant.FAILED;
                                    vendorFldInfo.Value = RVendorConstant.NOT_AVAILABLE;
                                    normalizedResponse[instrument].Add(vendorFldInfo.Name, vendorFldInfo);
                                }
                            }
                        }
                        else
                        {

                            unProcessedResponse.Add(instrument, securityInfo.InstrumentFields.ToDictionary(x => x.Mnemonic, y => new RVendorFieldInfo
                            {
                                Name = y.Mnemonic,
                                Status = RVendorStatusConstant.FAILED,
                                Value = RVendorConstant.NOT_AVAILABLE,
                                ExceptionMessage = RVendorConstant.NOT_PROCESSED
                            }, StringComparer.OrdinalIgnoreCase));

                        }
                    }
                }

                vendorResponse.SecurityResponse = normalizedResponse;

                if (normalizedResponse.Count > 0)
                {
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Passed;
                    vendorResponse.ExceptionMessage = "";
                }
                else
                {
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Failed;
                    vendorResponse.ExceptionMessage = "No data available from vendor";
                }
            }

            vendorResponse.RequestIdentifier = requestIdentifier;

            vendorResponse.AdditionalInfo = AdditionalInfo;
            vendorResponse.ResponseAdditionalInfo = ResponseAdditionalInfo;
            return vendorResponse;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the un processed records.
        /// </summary>
        internal static SecuritiesCollection GetUnProcessedRecords(SecuritiesCollection processedRecords, string reqInstruments, string reqFields, ref string exceptionMessage, ref int processedInstrumentCount,
                       ref int processedFieldCount)
        {

            SecuritiesCollection dictUnProcessed = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);

            string[] actualInstruments = reqInstruments.Split(',');
            string[] actualFields = reqFields.Split(',');


            List<string> processedInstruments = new List<string>();
            List<string> processedFields = new List<string>();

            List<string> unProcessedInstruments = new List<string>();
            List<string> unProcessedFields = new List<string>();

            if (processedRecords == null)
                processedRecords = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);

            #region "Processed Records"
            processedInstruments.AddRange(processedRecords.Keys);
            processedInstrumentCount = processedInstruments.Count;

            foreach (string key in processedRecords.Keys)
            {
                foreach (RVendorFieldInfo fldInfo in processedRecords[key].Values)
                    processedFields.Add(fldInfo.Name);
            }
            processedFieldCount = processedFields.Count;
            #endregion

            #region "UnProcessed Records = Actual Records - Processed Records"
            foreach (string instrument in actualInstruments)
            {
                if (instrument.Trim() != string.Empty && !processedInstruments.Contains(instrument))
                    unProcessedInstruments.Add(instrument);
            }

            foreach (string field in actualFields)
            {
                if (field.Trim() != string.Empty && !processedFields.Contains(field))
                    unProcessedFields.Add(field);
            }

            #endregion

            #region "Generate Exception Message"
            //if (exceptionMessage == string.Empty)
            //{
            //    if (unProcessedInstruments.Count > 0 && unProcessedFields.Count > 0)
            //    {
            //        exceptionMessage = "All Instruments and Fields not processsed.";
            //    }
            //    else if (unProcessedInstruments.Count > 0)
            //    {
            //        exceptionMessage = "All Instruments not processsed.";
            //    }
            //    else if (unProcessedFields.Count > 0)
            //    {
            //        exceptionMessage = "All Fields not processsed.";
            //    }
            //}
            #endregion

            if (unProcessedInstruments.Count > 0)
            {
                foreach (string instrument in unProcessedInstruments)
                {
                    var lstVendorFieldInfo = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                    foreach (string field in unProcessedFields)
                    {
                        var vendorFieldInfo = new RVendorFieldInfo();
                        vendorFieldInfo.Name = field;
                        vendorFieldInfo.Value = "";
                        vendorFieldInfo.Status = RVendorStatusConstant.FAILED;
                        vendorFieldInfo.ExceptionMessage = exceptionMessage;
                        lstVendorFieldInfo.Add(vendorFieldInfo.Name, vendorFieldInfo);
                    }
                    foreach (string field in processedFields)
                    {
                        var vendorFieldInfo = new RVendorFieldInfo();
                        vendorFieldInfo.Name = field;
                        vendorFieldInfo.Value = "";
                        vendorFieldInfo.Status = RVendorStatusConstant.FAILED;
                        vendorFieldInfo.ExceptionMessage = exceptionMessage;
                        lstVendorFieldInfo.Add(vendorFieldInfo.Name, vendorFieldInfo);
                    }
                    dictUnProcessed[instrument] = lstVendorFieldInfo;
                }
            }


            foreach (string inst in processedRecords.Keys)
            {
                foreach (string unProcField in unProcessedFields)
                {
                    var vendorFieldInfo = new RVendorFieldInfo();
                    vendorFieldInfo.Name = unProcField;
                    vendorFieldInfo.Value = "";
                    vendorFieldInfo.Status = RVendorStatusConstant.FAILED;
                    vendorFieldInfo.ExceptionMessage = exceptionMessage;
                    processedRecords[inst].Add(vendorFieldInfo.Name, vendorFieldInfo);
                }
            }

            return dictUnProcessed;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the output request dictionary.
        /// </summary>
        //internal static SecuritiesCollection GetOutputRequestDictionary(SecuritiesCollection securitiesList)
        //{
        //    SecuritiesCollection securities = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);

        //    foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> security in securitiesList)
        //    {
        //        if (!securities.Keys.Contains(security.Key))
        //            securities.Add(security.Key, security.Value);
        //        else
        //        {
        //            foreach (var fieldKeyVal in security.Value)
        //            {
        //                securities[security.Key].Add(fieldKeyVal.Key, fieldKeyVal.Value);
        //            }
        //        }
        //    }
        //    return securities;
        //}
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the database exception.
        /// </summary>
        internal static RVendorResponse GetDatabaseException(bool immediateRequest, string message)
        {
            RVendorResponse vendorResponse = new RVendorResponse();
            vendorResponse.ExceptionMessage = message;
            vendorResponse.ResponseStatus = RVendorResponseStatus.Failed;
            vendorResponse.SecurityResponse = null;
            return vendorResponse;
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region Private Methods
        /// <summary>
        /// Validates the instruments.
        /// </summary>
        private SecuritiesCollection ValidateInstruments(SecuritiesCollection responseSecurities, RBbgSecurityInfo securityInfo)
        {
            RBbgSecurityInfo tempSecurityInfo = new RBbgSecurityInfo();
            SecuritiesCollection tempDict = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            tempSecurityInfo = securityInfo;
            tempDict = responseSecurities;
            List<RVendorFieldInfo> lstUnprocessInstr = new List<RVendorFieldInfo>();

            foreach (RBbgInstrumentInfo info in tempSecurityInfo.Instruments)
            {
                foreach (string instrument in tempDict.Keys)
                {
                    if (info.InstrumentID.Equals(instrument))
                    {
                        tempSecurityInfo.Instruments.Remove(info);
                        break;
                    }
                }
            }

            foreach (RBbgInstrumentInfo info in tempSecurityInfo.Instruments)
            {
                RVendorFieldInfo fld = new RVendorFieldInfo();
                fld.Name = info.InstrumentID;
                fld.Status = RVendorStatusConstant.FAILED;
                fld.Value = "Not Available";
                fld.ExceptionMessage = "Not Processed";
                lstUnprocessInstr.Add(fld);
            }
            return responseSecurities;
        }
        private static object GetMaxValue(List<string> matchingSec)
        {
            string maxLength = string.Empty;
            foreach (string sec in matchingSec)
            {
                if (sec.Length > maxLength.Length)
                    maxLength = sec;
            }
            return maxLength;
        }
        #endregion

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the vendor history.
        /// </summary>
        internal static void UpdateVendorHistory(string processedInstruments,
                            string processedFields, string timestamp, string status)
        {
            UpdateVendorHistory(processedInstruments, processedFields, timestamp, status, new StringBuilder());
        }

        internal static void UpdateVendorHistory(string instruments, string fields, string timeStamp, string status, StringBuilder responseStatus)
        {
            mLogger.Debug("Start->Insert into vendor History");
            RDBConnectionManager mDBConn = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
            try
            {
                mDBConn.ExecuteQuery("Update ivp_rad_vendor_history set request_status='" + status +
                    "'" + ",processed_instruments=" + "'" + instruments + "'" +
                    " ,processed_fields=" + "'" + fields + "'" + ",[response status]=" + "'" + responseStatus.ToString() + "'" +
                    " where time_stamp = '" + timeStamp + "'", RQueryType.Update);

            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("End->Insert into vendor History");
            }
        }

        internal static bool RegisterBBGAuditLock(string identifier)
        {
            SMDataUploadManagerClient cl = null;
            try
            {
                cl = new SMDataUploadManagerClient();
                return cl.RegisterBBGAuditLock(identifier);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                if (cl != null)
                    cl.Close();
            }

        }

        internal static void UnRegisterBBGAuditLock(string identifier)
        {
            SMDataUploadManagerClient cl = null;
            try
            {
                cl = new SMDataUploadManagerClient();
                cl.UnRegisterBBGAuditLock(identifier);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw;
            }
            finally
            {
                if (cl != null)
                    cl.Close();
            }
        }

        internal static void InsertBBGAudit(string timeStamp, DataTable dtSecInfo, DataTable dtMnemonicInfo)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("InsertBBGAudit -> Start");
            var dictTableNames = new Dictionary<string, string>();
            string uniqueIdentifier = Guid.NewGuid().ToString();
            try
            {
                while (!RegisterBBGAuditLock(uniqueIdentifier))
                {
                    Thread.Sleep(3000);
                }

                string uniqueId = RBbgUtils.GetTargetDateTime();
                string secTableName = "secBBGAudit_" + uniqueId;
                string mneTableName = "mneBBGAudit_" + uniqueId;

                dictTableNames.Add(secTableName, "[IVPRAD].[dbo].[" + secTableName + "]");
                dictTableNames.Add(mneTableName, "[IVPRAD].[dbo].[" + mneTableName + "]");

                var query = new List<string>();
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + secTableName + "] (identifier_value VARCHAR(900), identifier_name VARCHAR(200), yellow_key_name VARCHAR(100))");
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + mneTableName + "] (mnemonic_name VARCHAR(100))");

                ExecuteSelectQuery(string.Join(Environment.NewLine, query), "RADDBConnectionId");

                ExecuteBulkUpload("[IVPRAD].[dbo].[" + secTableName + "]", dtSecInfo, "RADDBConnectionId");
                if (dtMnemonicInfo.Rows.Count > 0)
                    ExecuteBulkUpload("[IVPRAD].[dbo].[" + mneTableName + "]", dtMnemonicInfo, "RADDBConnectionId");

                ExecuteSelectQuery("EXEC IVPRAD.dbo.RAD_InsertBBGAudit '" + timeStamp + "','" + secTableName + "','" + mneTableName + "'", "RADDBConnectionId");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                UnRegisterBBGAuditLock(uniqueIdentifier);
                try
                {
                    if (dictTableNames.Count > 0)
                        ExecuteSelectQuery(string.Join(Environment.NewLine, dictTableNames.Select(x => "IF EXISTS(SELECT 1 FROM sys.tables WHERE name = '" + x.Key + "') DROP TABLE " + x.Value)), "RADDBConnectionId");
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                }

                mLogger.Debug("InsertBBGAudit -> End");
            }
        }

        internal static void InsertBBGAuditHist(string timeStamp, DataTable dtSecInfo, DataTable dtMnemonicInfo)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("InsertBBGAuditHist -> Start");

            var dictTableNames = new Dictionary<string, string>();
            string uniqueIdentifier = Guid.NewGuid().ToString();
            try
            {
                while (!RegisterBBGAuditLock(uniqueIdentifier))
                {
                    Thread.Sleep(3000);
                }

                string uniqueId = RBbgUtils.GetTargetDateTime();
                string secTableName = "secBBGAudit_" + uniqueId;
                string mneTableName = "mneBBGAudit_" + uniqueId;

                dictTableNames.Add(secTableName, "[IVPRAD].[dbo].[" + secTableName + "]");
                dictTableNames.Add(mneTableName, "[IVPRAD].[dbo].[" + mneTableName + "]");

                var query = new List<string>();
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + secTableName + "] (identifier_value VARCHAR(900), identifier_name VARCHAR(200), yellow_key_name VARCHAR(100),[start_date] DATETIME,[end_date] DATETIME)");
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + mneTableName + "] (mnemonic_name VARCHAR(100))");

                ExecuteSelectQuery(string.Join(Environment.NewLine, query), "RADDBConnectionId");

                ExecuteBulkUpload("[IVPRAD].[dbo].[" + secTableName + "]", dtSecInfo, "RADDBConnectionId");
                if (dtMnemonicInfo.Rows.Count > 0)
                    ExecuteBulkUpload("[IVPRAD].[dbo].[" + mneTableName + "]", dtMnemonicInfo, "RADDBConnectionId");

                ExecuteSelectQuery("EXEC IVPRAD.dbo.RAD_InsertBBGAuditHist '" + timeStamp + "','" + secTableName + "','" + mneTableName + "'", "RADDBConnectionId");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                UnRegisterBBGAuditLock(uniqueIdentifier);
                try
                {
                    if (dictTableNames.Count > 0)
                        ExecuteSelectQuery(string.Join(Environment.NewLine, dictTableNames.Select(x => "IF EXISTS(SELECT 1 FROM sys.tables WHERE name = '" + x.Key + "') DROP TABLE " + x.Value)), "RADDBConnectionId");
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                }

                mLogger.Debug("InsertBBGAuditHist -> End");
            }
        }

        internal static void InsertBBGAuditCorpAction(string timeStamp, DataTable dtSecInfo, DataTable dtCorpActionTypeInfo)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("InsertBBGAuditCorpAction -> Start");

            var dictTableNames = new Dictionary<string, string>();
            string uniqueIdentifier = Guid.NewGuid().ToString();
            try
            {
                while (!RegisterBBGAuditLock(uniqueIdentifier))
                {
                    Thread.Sleep(3000);
                }

                string uniqueId = RBbgUtils.GetTargetDateTime();
                string secTableName = "secBBGAudit_" + uniqueId;
                string corpTableName = "corpBBGAudit_" + uniqueId;

                dictTableNames.Add(secTableName, "[IVPRAD].[dbo].[" + secTableName + "]");
                dictTableNames.Add(corpTableName, "[IVPRAD].[dbo].[" + corpTableName + "]");

                var query = new List<string>();
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + secTableName + "] (identifier_value VARCHAR(900), identifier_name VARCHAR(200), yellow_key_name VARCHAR(100),[start_date] DATETIME,[end_date] DATETIME)");
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + corpTableName + "] (corpaction_type_name VARCHAR(100))");

                ExecuteSelectQuery(string.Join(Environment.NewLine, query), "RADDBConnectionId");

                ExecuteBulkUpload("[IVPRAD].[dbo].[" + secTableName + "]", dtSecInfo, "RADDBConnectionId");
                ExecuteBulkUpload("[IVPRAD].[dbo].[" + corpTableName + "]", dtCorpActionTypeInfo, "RADDBConnectionId");

                ExecuteSelectQuery("EXEC IVPRAD.dbo.RAD_InsertBBGAuditCorpAction '" + timeStamp + "','" + secTableName + "','" + corpTableName + "'", "RADDBConnectionId");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                UnRegisterBBGAuditLock(uniqueIdentifier);
                try
                {
                    if (dictTableNames.Count > 0)
                        ExecuteSelectQuery(string.Join(Environment.NewLine, dictTableNames.Select(x => "IF EXISTS(SELECT 1 FROM sys.tables WHERE name = '" + x.Key + "') DROP TABLE " + x.Value)), "RADDBConnectionId");
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                }

                mLogger.Debug("InsertBBGAuditCorpAction -> End");
            }
        }

        internal static void InsertBBGAuditHeaders(string timeStamp, DataTable dtHeaderInfo)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("InsertBBGAudit -> Start");

            var dictTableNames = new Dictionary<string, string>();
            string uniqueIdentifier = Guid.NewGuid().ToString();
            try
            {
                while (!RegisterBBGAuditLock(uniqueIdentifier))
                {
                    Thread.Sleep(3000);
                }

                string uniqueId = RBbgUtils.GetTargetDateTime();
                string headerTableName = "headerBBGAudit_" + uniqueId;

                dictTableNames.Add(headerTableName, "[IVPRAD].[dbo].[" + headerTableName + "]");

                var lstColumns = dtHeaderInfo.Columns.Cast<DataColumn>().Where(x => !x.ColumnName.Equals("history_id", StringComparison.OrdinalIgnoreCase)).Select(x => x.ColumnName).ToList();
                if (dtHeaderInfo.Columns.Contains("history_id"))
                    dtHeaderInfo.Columns.Remove("history_id");

                var lstColumnsDb = ExecuteSelectQuery("SELECT * FROM IVPRAD.dbo.ivp_rad_requested_header_history WHERE 1=2", "RADDBConnectionId").Tables[0].Columns.Cast<DataColumn>().ToDictionary(x => x.ColumnName, y => y.ColumnName, StringComparer.OrdinalIgnoreCase);
                var missingColumns = lstColumns.Except(lstColumnsDb.Keys, StringComparer.OrdinalIgnoreCase).ToList();

                var query = new List<string>();
                if (missingColumns.Count > 0)
                {
                    query.AddRange(missingColumns.Select(x => "ALTER TABLE IVPRAD.dbo.ivp_rad_requested_header_history ADD [" + x + "] VARCHAR(900)"));

                    ExecuteSelectQuery(string.Join(Environment.NewLine, query), "RADDBConnectionId");

                    foreach (string colName in missingColumns)
                    {
                        lstColumnsDb.Add(colName, colName);
                    }
                }

                var dictColumnMappings = lstColumns.ToDictionary(x => x, y => lstColumnsDb[y]);

                query = new List<string>();
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + headerTableName + "] (" + string.Join(",", lstColumns.Select(x => "[" + x + "] VARCHAR(900)")) + ")");
                ExecuteSelectQuery(string.Join(Environment.NewLine, query), "RADDBConnectionId");

                ExecuteBulkUpload("[IVPRAD].[dbo].[" + headerTableName + "]", dtHeaderInfo, dictColumnMappings, "RADDBConnectionId");

                ExecuteSelectQuery("EXEC IVPRAD.dbo.RAD_InsertBBGAuditHeaders '" + timeStamp + "','" + headerTableName + "','" + string.Join("|", dictColumnMappings.Keys) + "'", "RADDBConnectionId");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                UnRegisterBBGAuditLock(uniqueIdentifier);
                try
                {
                    if (dictTableNames.Count > 0)
                        ExecuteSelectQuery(string.Join(Environment.NewLine, dictTableNames.Select(x => "IF EXISTS(SELECT 1 FROM sys.tables WHERE name = '" + x.Key + "') DROP TABLE " + x.Value)), "RADDBConnectionId");
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                }

                mLogger.Debug("InsertBBGAudit -> End");
            }
        }

        internal static void UpdateBBGAudit(string timeStamp, DataTable dtSecInfo, DataTable dtMnemonicInfo)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("UpdateBBGAudit -> Start");

            var dictTableNames = new Dictionary<string, string>();
            string uniqueIdentifier = Guid.NewGuid().ToString();
            try
            {
                while (!RegisterBBGAuditLock(uniqueIdentifier))
                {
                    Thread.Sleep(3000);
                }

                string uniqueId = RBbgUtils.GetTargetDateTime();
                string secTableName = "secBBGAudit_" + uniqueId;
                string mneTableName = "mneBBGAudit_" + uniqueId;

                dictTableNames.Add(secTableName, "[IVPRAD].[dbo].[" + secTableName + "]");
                dictTableNames.Add(mneTableName, "[IVPRAD].[dbo].[" + mneTableName + "]");

                var query = new List<string>();
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + secTableName + "] (identifier_value VARCHAR(900), asset_id VARCHAR(100))");
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + mneTableName + "] (mnemonic_name VARCHAR(100))");

                ExecuteSelectQuery(string.Join(Environment.NewLine, query), "RADDBConnectionId");

                ExecuteBulkUpload("[IVPRAD].[dbo].[" + secTableName + "]", dtSecInfo, "RADDBConnectionId");
                if (dtMnemonicInfo.Rows.Count > 0)
                    ExecuteBulkUpload("[IVPRAD].[dbo].[" + mneTableName + "]", dtMnemonicInfo, "RADDBConnectionId");

                ExecuteSelectQuery("EXEC IVPRAD.dbo.RAD_UpdateBBGAudit '" + timeStamp + "','" + secTableName + "','" + mneTableName + "'", "RADDBConnectionId");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                UnRegisterBBGAuditLock(uniqueIdentifier);
                try
                {
                    if (dictTableNames.Count > 0)
                        ExecuteSelectQuery(string.Join(Environment.NewLine, dictTableNames.Select(x => "IF EXISTS(SELECT 1 FROM sys.tables WHERE name = '" + x.Key + "') DROP TABLE " + x.Value)), "RADDBConnectionId");
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                }

                mLogger.Debug("UpdateBBGAudit -> End");
            }
        }

        internal static void UpdateBBGAuditHist(string timeStamp, DataTable dtSecInfo, DataTable dtMnemonicInfo)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("UpdateBBGAuditHist -> Start");

            var dictTableNames = new Dictionary<string, string>();
            string uniqueIdentifier = Guid.NewGuid().ToString();
            try
            {
                while (!RegisterBBGAuditLock(uniqueIdentifier))
                {
                    Thread.Sleep(3000);
                }

                string uniqueId = RBbgUtils.GetTargetDateTime();
                string secTableName = "secBBGAudit_" + uniqueId;
                string mneTableName = "mneBBGAudit_" + uniqueId;

                dictTableNames.Add(secTableName, "[IVPRAD].[dbo].[" + secTableName + "]");
                dictTableNames.Add(mneTableName, "[IVPRAD].[dbo].[" + mneTableName + "]");

                var query = new List<string>();
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + secTableName + "] (identifier_value VARCHAR(900))");
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + mneTableName + "] (mnemonic_name VARCHAR(100))");

                ExecuteSelectQuery(string.Join(Environment.NewLine, query), "RADDBConnectionId");

                ExecuteBulkUpload("[IVPRAD].[dbo].[" + secTableName + "]", dtSecInfo, "RADDBConnectionId");
                if (dtMnemonicInfo.Rows.Count > 0)
                    ExecuteBulkUpload("[IVPRAD].[dbo].[" + mneTableName + "]", dtMnemonicInfo, "RADDBConnectionId");

                ExecuteSelectQuery("EXEC IVPRAD.dbo.RAD_UpdateBBGAuditHist '" + timeStamp + "','" + secTableName + "','" + mneTableName + "'", "RADDBConnectionId");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                UnRegisterBBGAuditLock(uniqueIdentifier);
                try
                {
                    if (dictTableNames.Count > 0)
                        ExecuteSelectQuery(string.Join(Environment.NewLine, dictTableNames.Select(x => "IF EXISTS(SELECT 1 FROM sys.tables WHERE name = '" + x.Key + "') DROP TABLE " + x.Value)), "RADDBConnectionId");
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                }

                mLogger.Debug("UpdateBBGAuditHist -> End");
            }
        }

        internal static void UpdateBBGAuditCorpAction(string timeStamp, DataTable dtSecInfo)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("UpdateBBGAuditCorpAction -> Start");

            var dictTableNames = new Dictionary<string, string>();
            string uniqueIdentifier = Guid.NewGuid().ToString();
            try
            {
                while (!RegisterBBGAuditLock(uniqueIdentifier))
                {
                    Thread.Sleep(3000);
                }

                string uniqueId = RBbgUtils.GetTargetDateTime();
                string secTableName = "secBBGAudit_" + uniqueId;

                dictTableNames.Add(secTableName, "[IVPRAD].[dbo].[" + secTableName + "]");

                var query = new List<string>();
                query.Add("CREATE TABLE [IVPRAD].[dbo].[" + secTableName + "] (identifier_value VARCHAR(900), corpaction_type_name VARCHAR(MAX))");

                ExecuteSelectQuery(string.Join(Environment.NewLine, query), "RADDBConnectionId");

                ExecuteBulkUpload("[IVPRAD].[dbo].[" + secTableName + "]", dtSecInfo, "RADDBConnectionId");

                ExecuteSelectQuery("EXEC IVPRAD.dbo.RAD_UpdateBBGAuditCorpAction '" + timeStamp + "','" + secTableName + "'", "RADDBConnectionId");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                UnRegisterBBGAuditLock(uniqueIdentifier);
                try
                {
                    if (dictTableNames.Count > 0)
                        ExecuteSelectQuery(string.Join(Environment.NewLine, dictTableNames.Select(x => "IF EXISTS(SELECT 1 FROM sys.tables WHERE name = '" + x.Key + "') DROP TABLE " + x.Value)), "RADDBConnectionId");
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                }

                mLogger.Debug("UpdateBBGAuditCorpAction -> End");
            }
        }

        internal static void UpdateBBGAuditStatus(string timeStamp)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("UpdateBBGAuditStatus -> Start");
            string uniqueIdentifier = Guid.NewGuid().ToString();
            try
            {
                while (!RegisterBBGAuditLock(uniqueIdentifier))
                {
                    Thread.Sleep(3000);
                }

                ExecuteSelectQuery("EXEC IVPRAD.dbo.RAD_UpdateBBGAuditStatus '" + timeStamp + "'", "RADDBConnectionId");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                UnRegisterBBGAuditLock(uniqueIdentifier);

                mLogger.Debug("UpdateBBGAuditStatus -> End");
            }
        }

        internal static void UpdateBBGAuditCorpActionStatus(string timeStamp)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("UpdateBBGAuditCorpActionStatus -> Start");
            string uniqueIdentifier = Guid.NewGuid().ToString();
            try
            {
                while (!RegisterBBGAuditLock(uniqueIdentifier))
                {
                    Thread.Sleep(3000);
                }

                ExecuteSelectQuery("EXEC IVPRAD.dbo.RAD_UpdateBBGAuditCorpActionStatus '" + timeStamp + "'", "RADDBConnectionId");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                UnRegisterBBGAuditLock(uniqueIdentifier);

                mLogger.Debug("UpdateBBGAuditCorpActionStatus -> End");
            }
        }

        public static DataSet ExecuteSelectQuery(string queryText, string connectionType)
        {
            return CommonDALWrapper.ExecuteSelectQuery(queryText, connectionType);
        }

        public static void ExecuteBulkUpload(string tableName, DataTable dtTable, string connectionType)
        {
            CommonDALWrapper.ExecuteBulkUpload(tableName, dtTable, connectionType);
        }

        public static void ExecuteBulkUpload(string tableName, DataTable dtTable, Dictionary<string, string> dictColumnMappings, string connectionType)
        {
            CommonDALWrapper.ExecuteBulkUpload(tableName, dtTable, dictColumnMappings, connectionType);
        }

        internal static void UpdateVendorHistory(string instruments, string fields, string timeStamp, string status, StringBuilder responseStatus, string responseXml, int vendorPricingId)
        {
            mLogger.Debug("Start->Insert into vendor History");
            RDBConnectionManager mDBConn = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
            try
            {
                mDBConn.ExecuteQuery("Update ivp_rad_vendor_history set request_status='" + status +
                    "'" + ",processed_instruments=" + "'" + instruments + "'" +
                    " ,processed_fields=" + "'" + fields + "'" + ",[response status]=" + "'" + responseStatus.ToString() + "'" +
                    " where time_stamp = '" + timeStamp + "' update ivp_rad_vendor_pricing_details set response_structure = '" + responseXml + "' where time_stamp = '" + timeStamp + "' declare @requested_on datetime select @requested_on = requested_on from ivp_rad_vendor_pricing_details where ID = " + vendorPricingId + " insert into dbo.ivp_rad_vendor_pricing_identifiers_sync VALUES(@requested_on)", RQueryType.Update);
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("End->Insert into vendor History");
            }
        }

        internal static RVendorResponse ProcessResponseForCombinedRequest(List<SecuritiesCollection> response, SecuritiesCollection unprocessed, RBbgSecurityInfo secInfo, string requestID, List<string> AdditionalInfo = null, List<string> ResponseAdditionalInfo = null)
        {
            RVendorResponse vendorResponse = new RVendorResponse();
            List<Dictionary<string, Dictionary<string, RVendorFieldInfo>>> listResponseSecurities = null;
            Dictionary<string, Dictionary<string, RVendorFieldInfo>> dictUnProcessedSecurities = null;
            DataSet ds = new DataSet();

            if (response is List<Dictionary<string, Dictionary<string, RVendorFieldInfo>>>)
            {
                if (response != null)
                    listResponseSecurities = (List<SecuritiesCollection>)response;
                if (unprocessed != null)
                    dictUnProcessedSecurities = (SecuritiesCollection)unprocessed;
                if (listResponseSecurities.Count != 0)
                {
                    for (int key = 0; key < listResponseSecurities.Count; key++)
                    {
                        // if (listResponseSecurities[key].Keys.Any(q => q.Contains(',')))
                        // {
                        var processedNormalFields = response[key].Where(q => !q.Key.Contains(","));
                        ds.Tables.Add(ConvertProcessedFieldsToData(processedNormalFields, unprocessed));
                        SecuritiesCollection processedBulkFields = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                        foreach (string field in response[key].Keys)
                        {
                            if (field.Contains(","))
                                processedBulkFields.Add(field, response[key][field]);
                        }
                        DataTable dtResult = RVendorUtils.ConvertToDataSetForBulkList(processedBulkFields, dictUnProcessedSecurities, secInfo);

                        if (dtResult != null && dtResult.Rows.Count > 0)
                        {
                            List<string> processedSec = new List<string>();
                            //bool isDataSetNull = false;
                            for (int counter = 0; counter < dtResult.Rows.Count; counter++)
                            {
                                object[] objArray = dtResult.Rows[counter].ItemArray;
                                if (!processedSec.Contains(objArray[0].ToString()))
                                {
                                    processedSec.Add(objArray[0].ToString());
                                    bool isDataPresent = false;
                                    for (int count = 3; count < dtResult.Columns.Count; count++)
                                    {
                                        if (!Convert.ToString(objArray[count]).Equals("0") && !Convert.ToString(objArray[count]).Equals("1") && !Convert.ToString(objArray[count]).Equals("Not Available") && !string.IsNullOrEmpty(Convert.ToString(objArray[count])))
                                        {
                                            isDataPresent = true;
                                            break;
                                        }
                                    }
                                    if (!isDataPresent)
                                    {
                                        dtResult.Rows.RemoveAt(counter);
                                        counter--;
                                    }
                                    else
                                    {
                                        //Rename Columns 
                                        int id = Convert.ToInt32(dtResult.TableName);
                                        RBbgBulkListInfo bulkListInfo = RVendorUtils.GetBulkListInfo(id);
                                        string[] fldNames = bulkListInfo.OutputFields.Split(',');
                                        if ((dtResult != null && dtResult.Rows.Count > 0))
                                        {
                                            foreach (DataColumn dc in dtResult.Columns)
                                            {
                                                if (dc.Ordinal >= 3)
                                                    dc.ColumnName = fldNames[dc.Ordinal - 3];
                                            }
                                        }

                                    }
                                }
                            }
                            ds.Tables.Add(dtResult);
                        }
                        //}
                        //else
                        //{
                        //    ds = ConvertToDataSet(listResponseSecurities[key], unprocessed);
                        //}
                    }
                    vendorResponse.SecurityResponse = ds;
                    if (listResponseSecurities.Count > 0)
                        vendorResponse.ResponseStatus = RVendorResponseStatus.Passed;
                    else
                        vendorResponse.ResponseStatus = RVendorResponseStatus.Failed;
                }
            }
            else
            {
                vendorResponse.SecurityResponse = response;
                vendorResponse.ResponseStatus = RVendorResponseStatus.RequestRegistered;
            }
            vendorResponse.RequestIdentifier = requestID;
            vendorResponse.ExceptionMessage = "";
            vendorResponse.AdditionalInfo = AdditionalInfo;
            vendorResponse.ResponseAdditionalInfo = ResponseAdditionalInfo;
            return vendorResponse;
        }

        private static DataTable ConvertProcessedFieldsToData(IEnumerable<KeyValuePair<string, Dictionary<string, RVendorFieldInfo>>> processedSec, Dictionary<string, Dictionary<string, RVendorFieldInfo>> unprocessed)
        {
            DataTable response = new DataTable("SecurityInfo");
            try
            {
                //--- Create Instrument Column,Default Columnn ---
                DataColumn dcInstrument = new DataColumn("INSTRUMENT", Type.GetType("System.String"));
                DataColumn dcIsValid = new DataColumn("is_valid", Type.GetType("System.String"));
                DataColumn dcFailureReason = new DataColumn("failure_reason", Type.GetType("System.String"));
                // ---Add Column to DataTable ---
                response.Columns.Add(dcInstrument);
                response.Columns.Add(dcIsValid);
                response.Columns.Add(dcFailureReason);
                //--- Create Columns from security 
                foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyval in processedSec)
                {
                    foreach (RVendorFieldInfo val in keyval.Value.Values)
                    {
                        DataColumn dc = new DataColumn(val.Name.ToUpper(), Type.GetType("System.String"));
                        if (!response.Columns.Contains(dc.ColumnName))
                            response.Columns.Add(dc);
                    }
                    break;
                }
                foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyval in unprocessed)
                {
                    foreach (RVendorFieldInfo val in unprocessed[keyval.Key].Values)
                    {
                        if (!response.Columns.Contains(val.Name))
                        {
                            DataColumn dc = new DataColumn(val.Name.ToUpper(), Type.GetType("System.String"));
                            if (!response.Columns.Contains(dc.ColumnName))
                                response.Columns.Add(dc);
                        }
                    }
                    break;
                }

                if (processedSec.Count() > 0)
                {
                    foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyval in processedSec)
                    {
                        DataRow drRow = response.NewRow();
                        drRow["INSTRUMENT"] = keyval.Key;
                        drRow["is_valid"] = RVendorStatusConstant.PASSED;
                        foreach (var fieldInfo in keyval.Value.Values)
                        {
                            drRow[fieldInfo.Name] = fieldInfo.Value;
                        }
                        if (unprocessed.Count > 0)
                        {
                            foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> key in unprocessed)
                            {
                                if (drRow["INSTRUMENT"].ToString().Equals(key.Key))
                                {
                                    foreach (var fieldInfo in unprocessed[keyval.Key].Values)
                                    {
                                        drRow[fieldInfo.Name] = fieldInfo.Value;
                                        drRow["failure_reason"] = fieldInfo.ExceptionMessage;
                                    }
                                    unprocessed.Remove(key.Key);
                                    break;
                                }
                            }
                        }
                        response.Rows.Add(drRow);
                    }
                }
                if (unprocessed.Count > 0)
                {
                    foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyval in unprocessed)
                    {
                        DataRow drRow = response.NewRow();
                        drRow["INSTRUMENT"] = keyval.Key;
                        foreach (var fieldInfo in unprocessed[keyval.Key].Values)
                        {
                            drRow[fieldInfo.Name] = fieldInfo.Value;
                            drRow["failure_reason"] = fieldInfo.ExceptionMessage;
                        }
                        drRow["is_valid"] = RVendorStatusConstant.FAILED;
                        response.Rows.Add(drRow);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        internal static Dictionary<string, Dictionary<string, object>> GetSystemDefinedOverrides()
        {
            Dictionary<string, Dictionary<string, object>> overrides = new Dictionary<string, Dictionary<string, object>>();
            try
            {
                mLogger.Debug("GetSystemDefinedOverrides=> begin getting overrides");
                RDBConnectionManager mDBConn = null;
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                DataSet dsResult = new DataSet();
                dsResult = mDBConn.ExecuteQuery("select * from dbo.ivp_rad_bbg_overrides_mapping", RQueryType.Select);
                if (dsResult != null && (dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0))
                {
                    foreach (DataRow dr in dsResult.Tables[0].Rows)
                    {
                        Dictionary<string, object> overrideValue = new Dictionary<string, object>();
                        overrideValue.Add(dr["overrideField"].ToString(), dr["overrideValue"]);
                        overrides.Add(dr["request_field"].ToString(), overrideValue);
                    }
                }
                mLogger.Debug("GetSystemDefinedOverrides=> end getting overrides");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            return overrides;
        }

        internal static bool ValidateRequestLength(List<RBbgSecurityInfo> listRequestedSecurities)
        {
            mLogger.Debug("ValidateRequestLength=> begin ValidateRequestLength");
            string monthlyLimit = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\IVPVendor\\GetData", "Monthly Limit", string.Empty);
            mLogger.Debug("monthly=> " + monthlyLimit);
            string dailyLimit = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\IVPVendor\\GetData", "Daily Limit", string.Empty);
            mLogger.Debug("daily=> " + dailyLimit);
            string clientName = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\IVPVendor\\GetData", "Client Name", string.Empty);
            mLogger.Debug("clientName=> " + clientName);
            string mailId = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\IVPVendor\\GetData", "Mail Id", string.Empty);
            mLogger.Debug("mailId=> " + mailId);
            HashSet<string> requestedInstrument = new HashSet<string>();
            if (!string.IsNullOrWhiteSpace(monthlyLimit) && !string.IsNullOrWhiteSpace(dailyLimit))
            {
                mLogger.Debug("ValidateRequestLength=> got registry info");
                foreach (var security in listRequestedSecurities)
                {
                    security.Instruments.ForEach(q =>
                    {
                        string InstrumentID = q.InstrumentID.Split('|')[0];
                        if (!requestedInstrument.Contains(InstrumentID))
                            requestedInstrument.Add(InstrumentID);
                    });
                }
                DataSet data = GetRequestedCountFromDatabase(requestedInstrument);
                if (data != null && data.Tables.Count > 0)
                {
                    mLogger.Debug("ValidateRequestLength=> got data from database");
                    int monthlyRequestedData = Convert.ToInt32(data.Tables[0].Rows[0][0]);
                    int dailyRequestedData = Convert.ToInt32(data.Tables[1].Rows[0][0]);
                    string[] monthlyLimitArray = monthlyLimit.Split(new char[] { ',' });
                    string[] dailyLimitArray = dailyLimit.Split(new char[] { ',' });
                    bool raiseException = false;
                    string message = string.Empty;
                    if (monthlyRequestedData > Convert.ToInt32(monthlyLimitArray[1]))
                    {
                        //mothly hard alert reached
                        mLogger.Debug("ValidateRequestLength=> Hard alert for monthly request");
                        SendMail(clientName, mailId, RBbgAlertType.MonthlyHardAlert, monthlyLimitArray[1], monthlyRequestedData);
                        mLogger.Debug("ValidateRequestLength=> end ValidateRequestLength");
                        raiseException = true;
                        message = message + "Hard alert for monthly request limit has reached.";
                        //throw new RVendorException("Hard alert for monthly request limit has reached.");
                    }
                    if (monthlyRequestedData > Convert.ToInt32(monthlyLimitArray[0]))
                    {
                        //monthly soft alert reached
                        mLogger.Debug("ValidateRequestLength=> soft alert for monthly request");
                        SendMail(clientName, mailId, RBbgAlertType.MonthlySoftAlert, monthlyLimitArray[0], monthlyRequestedData);
                        mLogger.Debug("ValidateRequestLength=> begin ValidateRequestLength");
                        // return true;
                    }
                    if (dailyRequestedData > Convert.ToInt32(dailyLimitArray[1]))
                    {
                        //daily hard alert reached
                        mLogger.Debug("ValidateRequestLength=> Hard alert for daily request");
                        SendMail(clientName, mailId, RBbgAlertType.DailyHardAlert, dailyLimitArray[1], dailyRequestedData);
                        mLogger.Debug("ValidateRequestLength=> begin ValidateRequestLength");
                        raiseException = true;
                        message = message + "Hard alert for daily request limit has reached.";
                        // throw new RVendorException("Hard alert for daily request limit has reached.");
                    }
                    if (dailyRequestedData > Convert.ToInt32(dailyLimitArray[0]))
                    {
                        //daily hard alert reached
                        mLogger.Debug("ValidateRequestLength=> soft alert for daily request");
                        SendMail(clientName, mailId, RBbgAlertType.DailySoftAlert, dailyLimitArray[0], dailyRequestedData);
                        mLogger.Debug("ValidateRequestLength=> begin ValidateRequestLength");
                    }
                    if (raiseException)
                    {
                        throw new RVendorException(message);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    mLogger.Debug("ValidateRequestLength=> begin ValidateRequestLength");
                    throw new Exception("Failed to validate monthly/daily request limit.");
                }
            }
            else
            {
                mLogger.Debug("ValidateRequestLength=> no data found for alerts.");
                if (!string.IsNullOrWhiteSpace(mailId) && !string.IsNullOrWhiteSpace(clientName))
                    SendMail(clientName, mailId, RBbgAlertType.NoAlertDefined, "", 0);
                throw new Exception("No data found for alerts.");
            }
        }

        private static void SendMail(string clientName, string mailId, RBbgAlertType rBbgAlertType, string requestLimit, int requestCount)
        {
            try
            {
                mLogger.Debug("begin sending mail.");
                string instanceName = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["instanceName"]) ? string.Empty : ConfigurationManager.AppSettings["instanceName"];
                string subject = "BLOOMBERG REQUEST LIMIT ALERT! {0} | {1} | {2} Breach | {3} | {4}";
                string body = "The count of unique security identifiers requested by the {0} instance of {1} has breached the {2} .<br/>The count of unique securities requested in the {6} will become {3}, breaching the {5} limit of  {4} set by  {1}.";
                string alertType = string.Empty;
                string alertRequestType = string.Empty;
                string breachType = string.Empty;
                IRTransport transport = new RTransportManager().GetTransport("SMTPNOTIFY");
                RMailContent mailContent = new RMailContent();
                switch (rBbgAlertType)
                {
                    case RBbgAlertType.DailyHardAlert:
                    case RBbgAlertType.MonthlyHardAlert:
                        alertType = "Hard";
                        break;
                    case RBbgAlertType.DailySoftAlert:
                    case RBbgAlertType.MonthlySoftAlert:
                        alertType = "Soft";
                        break;
                }
                switch (rBbgAlertType)
                {
                    case RBbgAlertType.DailySoftAlert:
                    case RBbgAlertType.DailyHardAlert:
                        alertRequestType = "Daily Unique Identifier Limit";
                        breachType = "day";
                        break;
                    case RBbgAlertType.MonthlyHardAlert:
                    case RBbgAlertType.MonthlySoftAlert:
                        alertRequestType = "Monthly Unique Identifier Limit";
                        breachType = "month";
                        break;
                }
                if (rBbgAlertType == RBbgAlertType.NoAlertDefined)
                {
                    subject = "BLOOMBERG REQUEST LIMIT ALERT! {0} | {1} | {2}";
                    subject = string.Format(subject, clientName, instanceName, alertType);
                    body = "The hard/Soft Alert count for Bloomberg request is not defined.";
                }
                else
                {
                    subject = string.Format(subject, clientName, instanceName, alertType, alertRequestType, requestLimit);
                    body = string.Format(body, instanceName, clientName, alertRequestType, requestCount, requestLimit, alertType, breachType);
                }



                mailContent.Body = body;
                mailContent.IsBodyHTML = true;
                mailContent.Subject = subject;
                mailContent.To = mailId;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FromEmailIDForApp"]))
                    mailContent.From = ConfigurationManager.AppSettings["FromEmailIDForApp"].ToString();
                transport.SendMessage(mailContent);
                mLogger.Debug("end sending mail.");
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
        }

        internal static DataSet GetRequestedCountFromDatabase(HashSet<string> securities)
        {
            mLogger.Debug("Start->GetRequestedCountFromDatabase");
            RDBConnectionManager mDBConn = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);

            var dictTableNames = new Dictionary<string, string>();
            try
            {
                DataTable dtSecurities = new DataTable();
                dtSecurities.Columns.Add("SecurityId", typeof(string));
                foreach (string security in securities)
                {
                    var drSecurity = dtSecurities.NewRow();
                    drSecurity["SecurityId"] = security;
                    dtSecurities.Rows.Add(drSecurity);
                }

                string tempTableName = "tempValidateInstrument_" + RBbgUtils.GetTargetDateTime();

                dictTableNames.Add(tempTableName, ("[dbo].[" + tempTableName + "]"));

                tempTableName = ("dbo." + tempTableName);

                mDBConn.ExecuteQuery("CREATE TABLE " + tempTableName + "(SecurityId VARCHAR(200))", RQueryType.Select);
                mDBConn.ExecuteBulkCopy(tempTableName, dtSecurities);
                DataSet data = mDBConn.ExecuteQuery("EXEC dbo.RAD_ValidateRequestLength '" + tempTableName + "'", RQueryType.Select);
                mDBConn.ExecuteQuery("DROP TABLE " + tempTableName, RQueryType.Select);

                //StringBuilder MailInfoXml = new StringBuilder();
                //MailInfoXml.Append("<Details>");
                //securities.ForEach(q => MailInfoXml.Append("<Name><![CDATA[" + q + "]]></Name>"));
                //MailInfoXml.Append("</Details>");
                //RHashlist param = new RHashlist();
                //param.Add("securities", MailInfoXml.ToString());
                //RHashlist result = mDBConn.ExecuteProcedure("RAD:ValidateRequestLength", param);
                //DataSet data = (DataSet)result["DataSet"];
                mLogger.Debug("End->GetRequestedCountFromDatabase");
                return data;
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                try
                {
                    if (dictTableNames.Count > 0)
                        ExecuteSelectQuery(string.Join(Environment.NewLine, dictTableNames.Select(x => "IF EXISTS(SELECT 1 FROM sys.tables WHERE name = '" + x.Key + "') DROP TABLE " + x.Value)), "RADDBConnectionId");
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex.ToString());
                }

                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
            }
        }

        internal static DataSet GetBloombergPricing(DateTime startDate, DateTime endDate)
        {
            RDBConnectionManager mDBConn = null;
            try
            {
                mLogger.Debug("begin GetBloombergPricing");
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                RHashlist list = new RHashlist();
                list.Add("startDate", startDate);
                list.Add("endDate", endDate);
                DataSet data = (DataSet)mDBConn.ExecuteProcedure("RAD:GetBloombergPricingLatest", list)[0];
                mLogger.Debug("end GetBloombergPricing");
                return data;
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
            }
        }

        internal static DataSet GetBloombergPricingNew(DateTime? requestDate, bool isPrevMonth)
        {
            RDBConnectionManager mDBConn = null;
            try
            {
                mLogger.Debug("begin GetBloombergPricing new");
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);

                DataSet data = mDBConn.ExecuteQuery("EXEC dbo.RAD_GetBloombergPrices " + (isPrevMonth ? "null" : "'" + requestDate.Value.ToString("yyyy-MM-dd") + "'") + "," + (isPrevMonth ? 1 : 0), RQueryType.Select);
                mLogger.Debug("end GetBloombergPricing new");
                return data;
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
            }
        }

        internal static DataTable GetDrillTableData(DataSet CurrrentMonth, DataSet previous)
        {
            mLogger.Debug("begin GetDrillTableData");
            try
            {
                DataTable drillDownDetails = new DataTable();
                drillDownDetails.Columns.Add("Description");
                drillDownDetails.Columns.Add("Amount");
                if (CurrrentMonth != null && CurrrentMonth.Tables.Count > 0 && CurrrentMonth.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in CurrrentMonth.Tables[1].Rows)
                    {
                        bool matchFound = false;
                        DataRow drillRow = drillDownDetails.NewRow();
                        if (previous != null && previous.Tables.Count > 0)
                        {
                            foreach (DataRow drr in previous.Tables[1].Rows)
                            {
                                if (dr["DESCRIPTION"].ToString().Equals(drr["DESCRIPTION"].ToString()))
                                {
                                    drillRow["Description"] = dr["DESCRIPTION"];
                                    drillRow["Amount"] = Convert.ToDecimal(dr["AMOUNT"]) - Convert.ToDecimal(drr["AMOUNT"]);
                                    matchFound = true;
                                    break;
                                }
                            }
                        }
                        if (!matchFound)
                        {
                            drillRow["Description"] = dr["DESCRIPTION"];
                            drillRow["Amount"] = Convert.ToDecimal(dr["AMOUNT"]);
                        }
                        drillDownDetails.Rows.Add(drillRow);
                    }
                }
                mLogger.Debug("end  GetDrillTableData");
                return drillDownDetails;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
        }

        public static void InsertLogTable(string step, DateTime date1, DateTime date2)
        {
            mLogger.Error(step + " : " + date2.Subtract(date1).TotalMilliseconds);
            try
            {
                RDBConnectionManager mDBConn = null;
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);

                mDBConn.ExecuteQuery("INSERT INTO dbo.tblVendorAPILog values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "','" + step + "'," + date2.Subtract(date1).TotalMilliseconds + ")", RQueryType.Update);
            }
            catch (Exception ex) { }
        }
    }
}
