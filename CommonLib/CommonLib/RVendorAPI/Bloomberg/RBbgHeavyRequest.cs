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
 * 1            24-10-2008      Nitin Saxena    Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.srm.vendorapi.bloombergServices.heavy;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using com.ivp.rad.common;
//using SecuritiesCollection = System.Collections.Generic.Dictionary<string,
//                                System.Collections.Generic.
//                                List<com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using InstrumentCollection = System.Collections.Generic.Dictionary<string,
                                com.ivp.srm.vendorapi.bloomberg.RBbgInstrumentInfo>;
using System.Web.Script.Serialization;
using com.ivp.srmcommon;
//using com.ivp.srm.vendorapi.ServiceReference1;

namespace com.ivp.srm.vendorapi.bloomberg
{
    /// <summary>
    /// Class for Bloomberg Heavy Request
    /// </summary>
    internal class RBbgHeavyRequest
    {
        #region class member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        SecuritiesCollection dictVendor = null;
        submitGetDataRequestResponse sbmtDataResponse = null;
        submitGetDataRequestRequest sbmtDataRequest = null;
        retrieveGetDataResponseRequest rtrvDataRequest = null;
        PerSecurityWS service = null;
        internal OrderedDictionary _instrumentCode = new OrderedDictionary();
        internal SecuritiesCollection unprocessedDict = null;
        #endregion
        //------------------------------------------------------------------------------------------
        #region Class Properties
        internal OrderedDictionary InstrumentErrorCode
        {
            get { return _instrumentCode; }
            set { _instrumentCode = value; }
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region Public Methods
        /// <summary>
        /// Gets the securities.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        public SecuritiesCollection GetSecurities(string timeStamp, RBbgSecurityInfo securityInfo, ref Dictionary<string, string> dictHeaders)
        {
            mLogger.Debug("Start ->getting securities");
            try
            {
                dictVendor = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                sbmtDataRequest = new submitGetDataRequestRequest();
                rtrvDataRequest = new retrieveGetDataResponseRequest();
                sbmtDataResponse = new submitGetDataRequestResponse();
                service = RBbgUtils.GetHeavyService(securityInfo);
                sbmtDataRequest = GenerateDataRequest(securityInfo, ref dictHeaders);
                RBbgUtils.InsertBBGAuditHeaders(timeStamp, dictHeaders);
                mLogger.Debug("HEAVY REQUEST SUBMIT");
                sbmtDataResponse = service.submitGetDataRequest(sbmtDataRequest);
                rtrvDataRequest.retrieveGetDataRequest = new RetrieveGetDataRequest();
                rtrvDataRequest.retrieveGetDataRequest.responseId = sbmtDataResponse.submitGetDataResponse.responseId;
                try
                {
                    mLogger.Debug("HEAVY REQUEST HEADERS JSON--> " + new JavaScriptSerializer() { MaxJsonLength = 2147483647 }.Serialize(sbmtDataRequest));
                    mLogger.Debug("HEAVY RESPONSE JSON--> " + new JavaScriptSerializer() { MaxJsonLength = 2147483647 }.Serialize(sbmtDataResponse));
                }
                catch (Exception ex) { }
                mLogger.Debug("HEAVY REQUEST HEADERS --> START");
                mLogger.Debug("CLOSING VALUES : " + sbmtDataRequest.submitGetDataRequest.headers.closingvalues);
                mLogger.Debug("DERIVED : " + sbmtDataRequest.submitGetDataRequest.headers.derived);
                mLogger.Debug("USER NUMBER : " + sbmtDataRequest.submitGetDataRequest.headers.usernumber);
                mLogger.Debug("WS : " + sbmtDataRequest.submitGetDataRequest.headers.ws);
                mLogger.Debug("SN : " + sbmtDataRequest.submitGetDataRequest.headers.sn);
                mLogger.Debug("responseId : " + rtrvDataRequest.retrieveGetDataRequest.responseId);
                mLogger.Debug("HEAVY REQUEST HEADERS --> END");
                dictVendor = GetFieldsData(rtrvDataRequest, service, securityInfo);
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->getting securities");
                if (sbmtDataRequest != null)
                    sbmtDataRequest = null;
                if (rtrvDataRequest != null)
                    rtrvDataRequest = null;
                if (sbmtDataResponse != null)
                    sbmtDataResponse = null;
            }
            return dictVendor;
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region Private Methods
        /// <summary>
        /// Gets the fields data.
        /// </summary>
        /// <param name="rtrvDataRequest">The RTRV data request.</param>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        private SecuritiesCollection GetFieldsData(retrieveGetDataResponseRequest rtrvDataRequest,
                                       PerSecurityWS service, RBbgSecurityInfo securityInfo)
        {
            retrieveGetDataResponseResponse rtrvDataResponse = null;
            OrderedDictionary respErrorCode = null;
            bool isBulkArray = false;
            mLogger.Debug("Start ->getting fields data");
            try
            {
                string clientName = SRMMTConfig.GetClientName();
                SecuritiesCollection respDict = null;
                rtrvDataResponse = new retrieveGetDataResponseResponse();
                respDict = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                respErrorCode = new OrderedDictionary();

                Dictionary<string, List<string>> dictBulkListFields = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

                if (securityInfo.BulkListMapId != null && securityInfo.BulkListMapId.Count > 0)
                {
                    foreach (int bulkListMapId in securityInfo.BulkListMapId)
                    {
                        RBbgBulkListInfo bulkListInfo = RVendorUtils.GetBulkListInfo(bulkListMapId);
                        dictBulkListFields[bulkListInfo.RequestField] = bulkListInfo.OutputFields.Split(',').ToList();
                    }
                }
                do
                {
                    mLogger.Debug("HEAVY REQUEST POLLING");
                    System.Threading.Thread.Sleep(RVendorHeavyStatusConstant.POLLINTERVAL);
                    rtrvDataResponse = service.retrieveGetDataResponse(rtrvDataRequest);
                }
                while (rtrvDataResponse.retrieveGetDataResponse.statusCode.code == RVendorHeavyStatusConstant.DATANOTAVAILABLE);
                mLogger.Debug("HEAVY REQUEST RETRIEVE");
                //Check Response level code//
                if (rtrvDataResponse.retrieveGetDataResponse.statusCode.code == RVendorHeavyStatusConstant.SUCCESS)
                {
                    try
                    {
                        mLogger.Debug("HEAVY RESPONSE DATA JSON--> " + new JavaScriptSerializer() { MaxJsonLength = 2147483647 }.Serialize(rtrvDataResponse));
                    }
                    catch (Exception ex) { }
                    mLogger.Error("response length=> " + rtrvDataResponse.retrieveGetDataResponse.instrumentDatas.Length);
                    for (int i = 0; i < rtrvDataResponse.retrieveGetDataResponse.instrumentDatas.Length; i++)
                    {
                        var respList = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                        //Check Field Level Code//
                        if (rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].code == "0")
                        {
                            mLogger.Error("instrument data length=> " + rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].data.Length);
                            for (int j = 0; j < rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].data.Length; j++)
                            {
                                RVendorFieldInfo vInfo = new RVendorFieldInfo();
                                if (rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].data[j].isArray && securityInfo.IsBulkList)
                                {
                                    isBulkArray = true;
                                    int count = 0;
                                    string instrument = string.Empty;
                                    foreach (RBbgInstrumentInfo instInfo in securityInfo.Instruments)
                                    {
                                        if (instInfo.InstrumentID.Equals(rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].instrument.id, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            instrument = instInfo.InstrumentID;
                                            break;
                                        }
                                        else if (instInfo.InstrumentID.Equals(rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].instrument.id + " " + rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].instrument.yellowkey, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            instrument = instInfo.InstrumentID;
                                            break;
                                        }
                                    }
                                    //throw new RVendorException("More than one value is returned for the requested field");
                                    if (rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].data[j].bulkarray != null)
                                    {
                                        if (!dictBulkListFields.ContainsKey(sbmtDataRequest.submitGetDataRequest.fields[j]))
                                            continue;

                                        var lstFields = dictBulkListFields[sbmtDataRequest.submitGetDataRequest.fields[j]];
                                        foreach (BulkArray blkArray in rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].data[j].bulkarray)
                                        {
                                            respList = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                                            int fieldCount = 0;
                                            foreach (BulkArrayEntry blkArrEntry in blkArray.data)
                                            {
                                                if (lstFields.Count <= fieldCount)
                                                    break;

                                                RVendorFieldInfo fldInfo = new RVendorFieldInfo();
                                                fldInfo.Name = lstFields[fieldCount++];
                                                fldInfo.Status = RVendorStatusConstant.PASSED;
                                                fldInfo.Value = blkArrEntry.value;
                                                respList.Add(fldInfo.Name, fldInfo);
                                            }
                                            respDict[count.ToString() + "," + securityInfo.BulkListMapId[0].ToString() + "," + instrument] = respList;
                                            count++;
                                        }
                                    }
                                }
                                else if(!rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].data[j].isArray && !securityInfo.IsBulkList)
                                {
                                    mLogger.Error(sbmtDataRequest.submitGetDataRequest.fields[j]);
                                    vInfo.Name = sbmtDataRequest.submitGetDataRequest.fields[j];
                                    mLogger.Error(rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].data[j].value);
                                    vInfo.Value = rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].data[j].value;
                                    vInfo.Status = RVendorStatusConstant.PASSED;
                                    respList.Add(vInfo.Name, vInfo);
                                }
                                else if (rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].data[j].isArray && !securityInfo.IsBulkList)
                                {

                                    String name = sbmtDataRequest.submitGetDataRequest.fields[j];
                                    Data currentData = rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].data[j];
                                    processBulkField(currentData, name, respList);


                                }
                            }
                            if (!isBulkArray)
                            {
                                foreach (RBbgInstrumentInfo instInfo in securityInfo.Instruments)
                                {
                                    if (instInfo.InstrumentID.Equals(rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].instrument.id, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        respDict[instInfo.InstrumentID] = respList;
                                        break;
                                    }
                                    else if (instInfo.InstrumentID.Equals(rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].instrument.id + " " + rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].instrument.yellowkey, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        respDict[instInfo.InstrumentID] = respList;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            mLogger.Error("instrument error code=> " + rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].code);
                            if (RVendorConfigLoader.mVendorConfig[clientName]["BBGErrorCodes"].ContainsKey(
                                                           rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].code))
                            {
                                respErrorCode[rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].code] =
                                    RVendorConfigLoader.mVendorConfig[clientName]["BBGErrorCodes"][rtrvDataResponse.retrieveGetDataResponse.instrumentDatas[i].code];
                                InstrumentErrorCode = respErrorCode;
                            }
                        }
                    }
                }
                else if (rtrvDataResponse.retrieveGetDataResponse.statusCode.code == RVendorHeavyStatusConstant.REQUESTERROR)
                {
                    mLogger.Error("error code=> " + rtrvDataResponse.retrieveGetDataResponse.statusCode.code);
                    RBbgUtils.HandleErrorCodes(rtrvDataResponse.retrieveGetDataResponse.statusCode.code.ToString());
                }

                return respDict;
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw new RVendorException(rEx.Message);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->getting fields data");
                if (rtrvDataResponse != null)
                    rtrvDataResponse = null;
            }
        }

        private void processBulkField(Data responseData, string name, Dictionary<string, RVendorFieldInfo> respList)
        {
            BulkArray[] bulkArray = responseData.bulkarray;
            int rows = responseData.rows;

            RVendorFieldInfo vFldInfo = new RVendorFieldInfo();
            vFldInfo.Name = name;
            vFldInfo.Status = RVendorStatusConstant.PASSED;
            vFldInfo.Value = string.Empty;

            respList.Add(vFldInfo.Name, vFldInfo);
            if (bulkArray != null && rows > 0)
            {
                int columns = responseData.bulkarray[0].columns;

                int numofBulkValues = rows;
                vFldInfo.Value = ";2;" + numofBulkValues.ToString() + ";";
                for (int bvCtr = 0; bvCtr < numofBulkValues; bvCtr++)
                {
                    BulkArray currentBulkArray = bulkArray[bvCtr];
                    // Get the number of sub fields for each bulk data element
                    int numofBulkElements = columns;
                    if (bvCtr == 0)
                    {
                        vFldInfo.Value += numofBulkElements.ToString() + ";";
                    }

                    StringBuilder str = new StringBuilder();

                    // Read each field in Bulk data
                    for (int beCtr = 0; beCtr < numofBulkElements; beCtr++)
                    {

                        str.Append((((int)(currentBulkArray.data[beCtr].type)) + 1).ToString() + ";").Append(currentBulkArray.data[beCtr].value).Append(";");
                    }
                    vFldInfo.Value += str.ToString();
                }
            }
        }



        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Generates the data request.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        private submitGetDataRequestRequest GenerateDataRequest(RBbgSecurityInfo securityInfo, ref Dictionary<string, string> dictHeaders)
        {
            GetDataHeaders getDataHeaders = null;
            mLogger.Debug("Start ->generating data request.");
            try
            {
                //Setting request header information	
                getDataHeaders = new GetDataHeaders();
                sbmtDataRequest = new submitGetDataRequestRequest();
                sbmtDataRequest.submitGetDataRequest = new SubmitGetDataRequest();

                Instruments instrs = new Instruments();

                RBbgUtils.GenerateHeaderSection(ref getDataHeaders, securityInfo, ref dictHeaders);
                sbmtDataRequest.submitGetDataRequest.headers = getDataHeaders;
                sbmtDataRequest.submitGetDataRequest.fields = RBbgUtils.GenerateFields(securityInfo);
                instrs = RBbgUtils.GenerateMacroRequest(securityInfo);
                instrs.instrument = RBbgUtils.GenerateInstruments(securityInfo);
                sbmtDataRequest.submitGetDataRequest.instruments = instrs;

                HashSet<string> defaultHeadersToSkip = RBbgUtils.GetDefaultHeadersToSkip();
                #region Handle BulkList Request
                if (securityInfo.IsBulkList)
                {
                    if (!defaultHeadersToSkip.Contains("CLOSINGVALUES"))
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "CLOSINGVALUES", "true");
                        sbmtDataRequest.submitGetDataRequest.headers.closingvalues = true;
                        sbmtDataRequest.submitGetDataRequest.headers.closingvaluesSpecified = true;
                    }

                    if (!defaultHeadersToSkip.Contains("DERIVED"))
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "DERIVED", "true");
                        sbmtDataRequest.submitGetDataRequest.headers.derived = true;
                        sbmtDataRequest.submitGetDataRequest.headers.derivedSpecified = true;
                    }

                    if (!defaultHeadersToSkip.Contains("HISTORICAL"))
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "HISTORICAL", "true");
                        sbmtDataRequest.submitGetDataRequest.headers.historical = true;
                        sbmtDataRequest.submitGetDataRequest.headers.historicalSpecified = true;
                    }

                    if (!defaultHeadersToSkip.Contains("QUOTECOMPOSITE"))
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "QUOTECOMPOSITE", "true");
                        sbmtDataRequest.submitGetDataRequest.headers.quotecomposite = true;
                        sbmtDataRequest.submitGetDataRequest.headers.quotecompositeSpecified = true;
                    }

                    if (!defaultHeadersToSkip.Contains("SECMASTER"))
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "SECMASTER", "true");
                        sbmtDataRequest.submitGetDataRequest.headers.secmaster = true;
                        sbmtDataRequest.submitGetDataRequest.headers.secmasterSpecified = true;
                    }
                    if (securityInfo.Instruments.Count > 0)
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "SECID", ((InstrumentType)Enum.Parse(typeof(InstrumentType), securityInfo.Instruments[0].InstrumentIdType.ToString())).ToString());
                        sbmtDataRequest.submitGetDataRequest.headers.secid = (InstrumentType)Enum.Parse(typeof(InstrumentType), securityInfo.Instruments[0].InstrumentIdType.ToString());
                        sbmtDataRequest.submitGetDataRequest.headers.secidSpecified = true;
                    }
                }
                else
                {
                    bool boolResult;
                    int intResult;
                    if (securityInfo.Instruments.Count > 0)
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "SECID", ((InstrumentType)Enum.Parse(typeof(InstrumentType), securityInfo.Instruments[0].InstrumentIdType.ToString())).ToString());
                        sbmtDataRequest.submitGetDataRequest.headers.secid = (InstrumentType)Enum.Parse(typeof(InstrumentType), securityInfo.Instruments[0].InstrumentIdType.ToString());
                        sbmtDataRequest.submitGetDataRequest.headers.secidSpecified = true;
                    }

                    if (!string.IsNullOrEmpty(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.DERIVED))
                        && bool.TryParse(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.DERIVED), out boolResult))
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "DERIVED", boolResult.ToString());
                        getDataHeaders.derived = boolResult;
                        getDataHeaders.derivedSpecified = boolResult;
                    }

                    if (!string.IsNullOrEmpty(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.CLOSING_VALUES))
                        && bool.TryParse(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.CLOSING_VALUES), out boolResult))
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "CLOSINGVALUES", boolResult.ToString());
                        getDataHeaders.closingvalues = boolResult;
                        getDataHeaders.closingvaluesSpecified = boolResult;
                    }

                    if (!string.IsNullOrEmpty(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.USER_NUMBER))
                        && int.TryParse(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.USER_NUMBER), out intResult))
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "USERNUMBER", intResult.ToString());
                        getDataHeaders.usernumber = intResult;
                        getDataHeaders.usernumberSpecified = true;
                    }

                    if (!string.IsNullOrEmpty(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.WS))
                        && int.TryParse(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.WS), out intResult))
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "WS", intResult.ToString());
                        getDataHeaders.ws = intResult;
                        getDataHeaders.wsSpecified = true;
                    }

                    if (!string.IsNullOrEmpty(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.SERIAL_NUMBER))
                        && int.TryParse(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.SERIAL_NUMBER), out intResult))
                    {
                        RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "SN", intResult.ToString());
                        getDataHeaders.sn = intResult;
                        getDataHeaders.snSpecified = true;
                    }
                }

                ProgramFlag programflag;
                if (!string.IsNullOrEmpty(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.PROGRAMFLAG))
                        && Enum.TryParse<ProgramFlag>(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.PROGRAMFLAG), out programflag))
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "PROGRAMFLAG", programflag.ToString());
                    getDataHeaders.programflag = programflag;
                    getDataHeaders.programflagSpecified = true;
                }
                #endregion
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->generating data request.");
                if (getDataHeaders != null)
                    getDataHeaders = null;
            }
            return sbmtDataRequest;
        }
        #endregion
    }
}
