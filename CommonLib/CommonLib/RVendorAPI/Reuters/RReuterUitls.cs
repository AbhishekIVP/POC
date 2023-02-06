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
 * 1            01-12-2008      Manoj          Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.srm.vendorapi.reuters;
using com.ivp.rad.common;
using com.ivp.rad.cryptography;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;

namespace com.ivp.srm.vendorapi.reuters
{
    /// <summary>
    /// Utils for Reuter
    /// </summary>
    internal class RReuterUtils
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RReuterUitls");
        static RRSAEncrDecr encDec = new RRSAEncrDecr();
        /// <summary>
        /// Gets the instance reuter service.
        /// </summary>
        internal static ExtractionService GetService(int VendorPreferenceId)
        {
            mLogger.Debug("Start->Get Reuters Service Instance");
            try
            {
                CredentialsHeader CH = new CredentialsHeader();
                CH.Username = RVendorConfigLoader.GetVendorConfig(RVendorType.Reuters, VendorPreferenceId, RVendorConstant.USERID);
                string password = RVendorConfigLoader.GetVendorConfig(RVendorType.Reuters, VendorPreferenceId, RVendorConstant.PASSWORD);
                if (!string.IsNullOrEmpty(password))
                {
                    string tempPass = password;
                    try
                    {
                        password = encDec.DoDecrypt(password);
                    }
                    catch (Exception ex)
                    {
                        password = tempPass;
                        mLogger.Error("GetService => " + ex.ToString());
                    }
                }
                CH.Password = password;
                CH.AuthenticationToken = "";
                //Create Extraction Service
                ExtractionService service = new ExtractionService();
                service.CredentialsHeaderValue = CH;
                service.AllowAutoRedirect = false;
                service.Url = RVendorConfigLoader.GetVendorConfig(RVendorType.Reuters, VendorPreferenceId, RVendorConstant.SERVICEURL);
                return service;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex);
            }
            finally
            {
                mLogger.Debug("End->Get Reuters Service Instance");
            }

        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the instruments.
        /// </summary>
        /// <param name="instrInfo">The instr info.</param>
        /// <returns></returns>
        internal static List<RReuterInstrumentInfo> GetInstruments(string instrInfo)
        {
            mLogger.Debug("Start->creating list of instrument.");
            List<RReuterInstrumentInfo> lstInstrument = null;
            try
            {
                lstInstrument = new List<RReuterInstrumentInfo>();
                string[] instrumentsInfo = instrInfo.Split(',');
                for (int i = 0; i < instrumentsInfo.Length; i++)
                {
                    RReuterInstrumentInfo info = new RReuterInstrumentInfo();
                    info.InstrumentID = instrumentsInfo[i];
                    if (!lstInstrument.Contains(info))
                        lstInstrument.Add(info);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex);
            }
            finally
            {
                mLogger.Debug("End-> creating list of instrument.");
            }
            return lstInstrument;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        internal static List<string> GetFields(string fields)
        {
            mLogger.Debug("Start->creating list of instrument.");
            List<string> lstFields = null;
            try
            {
                lstFields = new List<string>();
                string[] fieldsInfo = fields.Split(',');
                for (int i = 0; i < fieldsInfo.Length; i++)
                {
                    if (!lstFields.Contains(fieldsInfo[i]))
                        lstFields.Add(fieldsInfo[i]);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex);
            }
            finally
            {
                mLogger.Debug("End-> creating list of instrument.");
            }
            return lstFields;
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Validates the request.
        /// </summary>
        internal static RReuterSecurityInfo ValidateRequest(RReuterSecurityInfo secInfo)
        {
            List<string> lstInstrumentIds = new List<string>();
            List<string> lstFields = new List<string>();
            List<RReuterInstrumentInfo> lstInstruments = new List<RReuterInstrumentInfo>();
            foreach (RReuterInstrumentInfo inst in secInfo.Instruments)
            {
                if (!lstInstrumentIds.Contains(inst.InstrumentID))
                {
                    lstInstrumentIds.Add(inst.InstrumentID);
                    lstInstruments.Add(inst);
                }
            }

            foreach (string str in secInfo.InstrumentFields)
            {
                if (!lstFields.Contains(str))
                    lstFields.Add(str);
            }

            secInfo.InstrumentFields = lstFields;
            secInfo.Instruments = lstInstruments;
            return secInfo;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Writes the vendor history.
        /// </summary>
        internal static void WriteVendorHistory(RReuterSecurityInfo securityInfo, string identifier,
                                                    string requestId, int instCount, int fldCount)
        {
            RVendorHistoryInfo historyInfo = new RVendorHistoryInfo();

            historyInfo.requestIdentifier = identifier;
            if (requestId.Contains("Request"))
                historyInfo.RequestStatus = RVendorStatusConstant.PASSED;
            else
                historyInfo.RequestStatus = RVendorStatusConstant.PENDING;
            historyInfo.TimeStamp = requestId;
            historyInfo.UserLoginName = securityInfo.UserLoginName;
            historyInfo.VendorFields = GetCSVFields(securityInfo.InstrumentFields);
            historyInfo.VendorInstruments = GetCSVInstruments(securityInfo.Instruments);
            historyInfo.ProcessedInstrumentCount = instCount;
            historyInfo.ProcessedFieldCount = fldCount;
            RVendorUtils.InsertHistory(historyInfo);
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Writes the vendor history.
        /// </summary>
        internal static void WriteVendorHistory(RReuterSecurityInfo securityInfo, string vendorName,
                                                            string identifier, string timestamp)
        {
            RVendorHistoryInfo historyInfo = new RVendorHistoryInfo();
            historyInfo.requestIdentifier = identifier;
            historyInfo.RequestType = securityInfo.RequestType.ToString();
            historyInfo.VendorName = vendorName;
            historyInfo.TimeStamp = timestamp;
            historyInfo.UserLoginName = securityInfo.UserLoginName;
            historyInfo.VendorFields = GetCSVFields(securityInfo.InstrumentFields);
            historyInfo.VendorInstruments = GetCSVInstruments(securityInfo.Instruments);
            historyInfo.RequestStatus = RVendorStatusConstant.PENDING;
            historyInfo.ModuleId = securityInfo.ModuleId;
            RVendorUtils.InsertHistory(historyInfo);
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Normalizes the securities.
        /// </summary>
        internal static SecuritiesCollection NormalizeSecurities(List<SecuritiesCollection> listResponseSecurities, RReuterSecurityInfo securityInfo, ref SecuritiesCollection unProcessedResponse)
        {
            SecuritiesCollection normalizedResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            unProcessedResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);

            foreach (Dictionary<string, Dictionary<string, RVendorFieldInfo>> response in listResponseSecurities)
            {
                foreach (string instrument in response.Keys)
                {
                    if (normalizedResponse.ContainsKey(instrument))
                    {
                        Dictionary<string, RVendorFieldInfo> listVendorFieldInfo = normalizedResponse[instrument];
                        foreach (var fieldInfoKeyVal in listVendorFieldInfo)
                        {
                            listVendorFieldInfo.Add(fieldInfoKeyVal.Key, fieldInfoKeyVal.Value);
                        }
                        
                        normalizedResponse[instrument] = listVendorFieldInfo;
                    }
                    else
                        normalizedResponse[instrument] = response[instrument];
                }
            }

            List<string> unProcessedFields = null;
            foreach (string instrument in normalizedResponse.Keys)
            {
                unProcessedFields = new List<string>();
                unProcessedFields = ValidateAllFields(securityInfo.InstrumentFields, normalizedResponse[instrument]);
                if (unProcessedFields.Count > 0)
                {
                    foreach (string fld in unProcessedFields)
                    {
                        RVendorFieldInfo vendorFldInfo = new RVendorFieldInfo();
                        vendorFldInfo.Name = fld;
                        vendorFldInfo.Status = RVendorStatusConstant.FAILED;
                        vendorFldInfo.Value = RVendorConstant.NOT_AVAILABLE;
                        normalizedResponse[instrument].Add(vendorFldInfo.Name, vendorFldInfo);
                    }
                }
            }
            //------------------------------------------------------------------------------------------
            #region "Add Unprocessed Securities"
            List<string> unProcessedInstruments = null;
            //Validate Fields for only one security as all securities returned will have equal # of fields
            unProcessedInstruments = ValidateAllInstruments(securityInfo.Instruments, normalizedResponse);

            if (unProcessedInstruments.Count > 0)
            {
                foreach (string instrument in unProcessedInstruments)
                {
                    var vendorFlds = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                    foreach (string fields in securityInfo.InstrumentFields)
                    {
                        RVendorFieldInfo fieldInfo = new RVendorFieldInfo();
                        fieldInfo.Name = fields;
                        fieldInfo.Status = RVendorStatusConstant.FAILED;
                        fieldInfo.Value = RVendorConstant.NOT_AVAILABLE;
                        fieldInfo.ExceptionMessage = RVendorConstant.NOT_PROCESSED;
                        vendorFlds.Add(fieldInfo.Name, fieldInfo);
                    }
                    unProcessedResponse[instrument] = vendorFlds;
                }
            }
            #endregion
            return normalizedResponse;

        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Validates all instruments.
        /// </summary>
        private static List<string> ValidateAllInstruments(List<RReuterInstrumentInfo> lstInstrInfo, SecuritiesCollection responseSecurities)
        {
            List<string> unProcessedInstrments = new List<string>();
            foreach (RReuterInstrumentInfo info in lstInstrInfo)
            {
                bool fldFound = false;
                foreach (string instrument in responseSecurities.Keys)
                {
                    if (info.InstrumentID.ToLower() == instrument.ToLower())
                    {
                        fldFound = true;
                        break;
                    }
                }
                if (!fldFound)
                {
                    unProcessedInstrments.Add(info.InstrumentID);
                }
            }
            return unProcessedInstrments;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Validates all fields.
        /// </summary>
        private static List<string> ValidateAllFields(List<string> lstFldInfo, Dictionary<string, RVendorFieldInfo> processedFieldInfo)
        {
            List<string> unProcessedFields = new List<string>();
            foreach (string fields in lstFldInfo)
            {
                bool fldFound = false;
                foreach (RVendorFieldInfo processfld in processedFieldInfo.Values)
                {
                    if (processfld.Name.ToLower() == fields.ToLower())
                    {
                        fldFound = true;
                        break;
                    }
                }
                if (!fldFound)
                {
                    unProcessedFields.Add(fields);
                }
            }
            return unProcessedFields;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the CSV fields.
        /// </summary>
        internal static string GetCSVInstruments(Dictionary<string, Dictionary<string, RVendorFieldInfo>> resDict)
        {
            if (resDict == null)
                return string.Empty;
            string strFields = "";
            foreach (string key in resDict.Keys)
            {
                strFields = strFields + "," + key;
            }
            if (strFields != string.Empty)
                strFields = strFields.Remove(0, 1);
            return strFields;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the CSV fields.
        /// </summary>
        internal static string GetCSVFields(Dictionary<string, Dictionary<string, RVendorFieldInfo>> resDict)
        {
            if (resDict == null)
                return string.Empty;
            string strFields = "";
            foreach (string key in resDict.Keys)
            {
                foreach (RVendorFieldInfo fields in resDict[key].Values)
                {
                    if (fields.Status == RVendorStatusConstant.PASSED)
                        strFields = strFields + "," + fields.Name;
                }
            }
            if (strFields != string.Empty)
                strFields = strFields.Remove(0, 1);
            return strFields;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the CSV fields.
        /// </summary>
        internal static string GetCSVFields(List<string> list)
        {
            string strFields = "";
            foreach (string fields in list)
            {
                strFields = strFields + "," + fields;
            }
            if (strFields != string.Empty)
                strFields = strFields.Remove(0, 1);
            return strFields;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the CSV instruments.
        /// </summary>
        internal static string GetCSVInstruments(List<RReuterInstrumentInfo> instruments)
        {
            string strInstruments = "";
            foreach (RReuterInstrumentInfo instrument in instruments)
            {
                strInstruments = strInstruments + "," + instrument.InstrumentID;
            }
            if (strInstruments != string.Empty)
                strInstruments = strInstruments.Remove(0, 1);
            return strInstruments;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the vendor history.
        /// </summary>
        internal static void UpdateVendorHistory(string requestIdentifier, string timeStamp, int instCount, int fldCount)
        {
            RVendorHistoryInfo historyInfo = new RVendorHistoryInfo();
            historyInfo.requestIdentifier = requestIdentifier;
            historyInfo.RequestStatus = RVendorStatusConstant.PASSED;
            historyInfo.TimeStamp = timeStamp;
            historyInfo.ProcessedInstrumentCount = instCount;
            historyInfo.ProcessedFieldCount = fldCount;
            //RVendorUtils.UpdateVendorHistory("aa", "bb", historyInfo.TimeStamp);
        }

        internal static RVendorResponse ProcessResponse(RSecurityReturnType returnType, object response,
                                            object unprocessed, string requestIdentifier, List<string> AdditionalInfo = null, List<string> ResponseAdditionalInfo = null)
        {
            RVendorResponse vendorResponse = new RVendorResponse();
            SecuritiesCollection dictResponseSecurities = null;
            SecuritiesCollection dictUnProcessedSecurities = null;
            SecuritiesCollection securities = null;

            if (response is SecuritiesCollection)
            {
                securities = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                if (response != null)
                    dictResponseSecurities = (SecuritiesCollection)response;
                if (unprocessed != null)
                    dictUnProcessedSecurities = (SecuritiesCollection)unprocessed;
                if (dictResponseSecurities.Count != 0)
                {
                    securities = GetOutputRequestDictionary(dictResponseSecurities);
                }
                if (returnType == RSecurityReturnType.DataSet)
                    vendorResponse.SecurityResponse = RVendorUtils.ConvertToDataSet(securities, dictUnProcessedSecurities);
                else
                    vendorResponse.SecurityResponse = securities;
                if (securities.Count > 0)
                {
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Passed;
                    vendorResponse.ExceptionMessage = "";
                }
                else
                {
                    vendorResponse.ResponseStatus = RVendorResponseStatus.Failed;
                    vendorResponse.ExceptionMessage = "No data available from vendor";
                }
                vendorResponse.RequestIdentifier = requestIdentifier;
            }
            else
            {
                vendorResponse.SecurityResponse = response;
                vendorResponse.RequestIdentifier = requestIdentifier;
                vendorResponse.ResponseStatus = RVendorResponseStatus.RequestRegistered;
                vendorResponse.ExceptionMessage = "";
            }
            vendorResponse.AdditionalInfo = AdditionalInfo;
            vendorResponse.ResponseAdditionalInfo = ResponseAdditionalInfo;
            return vendorResponse;
        }

        internal static SecuritiesCollection GetOutputRequestDictionary(SecuritiesCollection securitiesList)
        {
            SecuritiesCollection securities = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> security in securitiesList)
            {
                if (!securities.Keys.Contains(security.Key))
                    securities.Add(security.Key, security.Value);
                else
                {
                    foreach (var fieldKeyVal in security.Value)
                    {
                        securities[security.Key].Add(fieldKeyVal.Key, fieldKeyVal.Value);
                    }
                }
            }
            return securities;
        }
    }
}
