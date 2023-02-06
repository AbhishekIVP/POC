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
 * Version      Date            Author        Comments
 * -------------------------------------------------------------------------------------------------
 * 1            24-10-2008      Manoj         Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using com.ivp.rad.common;
using System.Threading;
using com.ivp.srm.vendorapi.Bloomberg;
//using SecuritiesCollection = System.Collections.Generic.Dictionary<string,
//                                System.Collections.Generic.
//                                List<com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using InstrumentCollection = System.Collections.Generic.Dictionary<string,
                                com.ivp.srm.vendorapi.bloomberg.RBbgInstrumentInfo>;
using com.ivp.rad.utils;
using System.Xml;
using com.ivp.rad.configurationmanagement;
using System.IO;
using Microsoft.Win32;
using System.Net;
using System.Web.Script.Serialization;
using com.ivp.srmcommon;

namespace com.ivp.srm.vendorapi.bloomberg
{
    /// <summary>
    /// Bloomberg Data Service
    /// </summary>
    public class RBloomberg : IVendor
    {
        static RBloomberg()
        {
            ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
        }

        #region Member Variables
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        private object responseLock = new object();
        private object additionalInfoLock = new object();
        private object errorLock = new object();
        SecuritiesCollection dictUnprocessedInstruments = null;
        List<SecuritiesCollection> listResponseSecurities = null;
        List<string> AdditionalInfo = null;
        List<string> ResponseAdditionalInfo = null;
        RVendorResponse vendorResponse = null;
        ManualResetEvent[] manaulResetEvent = null;
        bool skipFlag = true;
        bool marketSectorSpecified = false;
        #endregion

        #region IVendor Members
        RVendorResponse IVendor.GetSecurities(object requestedSecurities, RSecurityReturnType returnType, bool immediateRequest)
        {
            mLogger.Debug("Start -> Get Bloomberg Securities");
            try
            {
                RBbgSecurityInfo requestedSec = (RBbgSecurityInfo)requestedSecurities;
                if (requestedSec.RequestType == RBbgRequestType.SAPI && requestedSec.Instruments[0].MarketSector == RBbgMarketSector.None)
                    marketSectorSpecified = false;
                else
                    marketSectorSpecified = true;
                string gd = Guid.NewGuid().ToString();

                string transportName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, requestedSec.VendorPreferenceId, RVendorConstant.TRANSPORTNAME);

                if (requestedSec.RequestType == RBbgRequestType.FTP && (string.IsNullOrEmpty(requestedSec.TransportName) || (!string.IsNullOrEmpty(transportName) && immediateRequest && requestedSec.ImmediateRequest)))
                {
                    requestedSec.TransportName = transportName;
                }

                var listRequestedSecurities = RBbgUtils.GetSecurityRequests(requestedSec);
                if (listRequestedSecurities[0].RequestType != RBbgRequestType.SAPI)
                    RVendorUtils.ValidateRequestLength(listRequestedSecurities);
                listResponseSecurities = new List<SecuritiesCollection>();
                dictUnprocessedInstruments = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                var bulkListInfo = new RBbgBulkListInfo();
                AdditionalInfo = new List<string>();
                ResponseAdditionalInfo = new List<string>();
                int VendorPreferenceId = listRequestedSecurities[0].VendorPreferenceId;

                if (listRequestedSecurities.Count == 0)
                    throw new RVendorException("No instruments to be processed");

                manaulResetEvent = new ManualResetEvent[listRequestedSecurities.Count];
                if (listRequestedSecurities[0].RequestType == RBbgRequestType.Lite)
                {
                    ExecuteRequestInThreadForLite(listRequestedSecurities, immediateRequest, gd);
                }
                else if (listRequestedSecurities[0].RequestType == RBbgRequestType.FTP)
                    ExecuteRequestInThreadForFTP(listRequestedSecurities, immediateRequest, gd);
                else
                    ExecuteRequestInThread(listRequestedSecurities, immediateRequest, gd);

                //Main Thread wait for all worker threads to complete their processing.
                WaitHandle.WaitAll(manaulResetEvent);

                if (listRequestedSecurities[0].RequestType == RBbgRequestType.Lite ||
                        listRequestedSecurities[0].RequestType == RBbgRequestType.Heavy ||
                        listRequestedSecurities[0].RequestType == RBbgRequestType.SAPI)
                    immediateRequest = true;

                if (!skipFlag)
                {
                    vendorResponse.RequestIdentifier = gd;
                    return vendorResponse;
                }

                #region Process Response
                if (immediateRequest)
                {
                    if (((RBbgSecurityInfo)requestedSecurities).IsBulkList)
                    {
                        if (listRequestedSecurities[0].RequestType == RBbgRequestType.FTP)
                        {
                            if (dictUnprocessedInstruments != null && dictUnprocessedInstruments.Count > 0)
                            {
                                foreach (string instrument in dictUnprocessedInstruments.Keys)
                                {
                                    foreach (var fieldInfo in dictUnprocessedInstruments[instrument].Values)
                                    {
                                        fieldInfo.Value = RVendorConstant.NOT_AVAILABLE;
                                        fieldInfo.Status = RVendorStatusConstant.FAILED;
                                    }
                                }
                            }
                        }

                        if (((RBbgSecurityInfo)requestedSecurities).IsCombinedFtpReq)
                            return RVendorUtils.ProcessResponseForCombinedRequest(listResponseSecurities, dictUnprocessedInstruments, (RBbgSecurityInfo)requestedSecurities, gd, AdditionalInfo, ResponseAdditionalInfo);
                        else
                            return RVendorUtils.ProcessResponseForBulkList(RSecurityReturnType.DataSet, listResponseSecurities, dictUnprocessedInstruments,
                                ((RBbgSecurityInfo)requestedSecurities), gd, AdditionalInfo, ResponseAdditionalInfo);
                    }
                    else
                    {
                        mLogger.Debug("Start -->begin ProcessResponse");
                        RVendorResponse objResponse = RVendorUtils.ProcessResponseSecurities(listResponseSecurities, (RBbgSecurityInfo)requestedSecurities, ref dictUnprocessedInstruments, returnType, gd, true, requestedSec.VendorPreferenceId, RVendorType.Bloomberg, AdditionalInfo, ResponseAdditionalInfo);
                        mLogger.Debug("End -->begin ProcessResponse");

                        //try
                        //{
                        //    //mLogger.Error("GetSecurities objResponse : " + new JavaScriptSerializer().Serialize(objResponse));
                        //    mLogger.Error("GetSecurities objResponse ResponseStatus : " + objResponse.ResponseStatus.ToString());
                        //    mLogger.Error("GetSecurities objResponse SecurityResponse : " + ((DataSet)objResponse.SecurityResponse).GetXml());
                        //}
                        //catch (Exception ex)
                        //{ }


                        if (((RBbgSecurityInfo)requestedSecurities).RequestType == RBbgRequestType.SAPI && !marketSectorSpecified && objResponse.ResponseStatus == RVendorResponseStatus.Failed)
                        {
                            List<string> mktSecotrList = Enum.GetNames(typeof(RBbgMarketSector)).ToList();
                            mktSecotrList.Remove("None");
                            //mktSecotrList.Remove("Equity");
                            foreach (string mktSector in mktSecotrList)
                            {
                                ((RBbgSecurityInfo)requestedSecurities).Instruments.ForEach(instr =>
                                {
                                    instr.MarketSector = (RBbgMarketSector)Enum.Parse(typeof(RBbgMarketSector), mktSector);
                                });
                                IVendor vendor = RVendorManager.GetVendor(RVendorType.Bloomberg, VendorPreferenceId);
                                RVendorResponse mktSectorResponse = vendor.GetSecurities(requestedSecurities, returnType, immediateRequest);
                                if (mktSectorResponse.ResponseStatus == RVendorResponseStatus.Passed)
                                    return mktSectorResponse;
                            }
                            return objResponse;
                        }
                        else
                        {
                            return objResponse;
                        }
                    }
                }
                else
                    return RVendorUtils.ProcessResponse(gd, AdditionalInfo, ResponseAdditionalInfo);
                #endregion
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                RVendorResponse vendorResponse = RVendorUtils.GetDatabaseException(immediateRequest, rEx.Message);
                return vendorResponse;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                RVendorResponse vendorResponse = RVendorUtils.GetDatabaseException(immediateRequest, ex.Message);
                return vendorResponse;
            }
            finally
            {
                mLogger.Debug("End -> creating request object.");
            }
        }

        RVendorResponse GetSecurities(object requestedSecurities, RSecurityReturnType returnType, bool immediateRequest, ref DataSet dsResponseData, ref object requestedBbgSecurities, List<string> lstBulkMnemonicsInMasterReq, Dictionary<string, List<string>> lstBulkMnemonicsInLegReq)
        {
            mLogger.Debug("Start -> Get Bloomberg Securities");
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;
            try
            {
                RBbgSecurityInfo requestedSec = (RBbgSecurityInfo)requestedSecurities;
                if (requestedSec.RequestType == RBbgRequestType.SAPI && requestedSec.Instruments[0].MarketSector == RBbgMarketSector.None)
                    marketSectorSpecified = false;
                else
                    marketSectorSpecified = true;
                string gd = Guid.NewGuid().ToString();

                string transportName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, requestedSec.VendorPreferenceId, RVendorConstant.TRANSPORTNAME);

                if (requestedSec.RequestType == RBbgRequestType.FTP && (string.IsNullOrEmpty(requestedSec.TransportName) || (!string.IsNullOrEmpty(transportName) && immediateRequest && requestedSec.ImmediateRequest)))
                {
                    requestedSec.TransportName = transportName;
                }

                date1 = DateTime.Now;
                var listRequestedSecurities = RBbgUtils.GetSecurityRequests(requestedSec);
                date2 = DateTime.Now;
                RVendorUtils.InsertLogTable("GetSecurityRequests", date1, date2);

                date1 = DateTime.Now;
                if (listRequestedSecurities[0].RequestType != RBbgRequestType.SAPI)
                    RVendorUtils.ValidateRequestLength(listRequestedSecurities);
                date2 = DateTime.Now;
                RVendorUtils.InsertLogTable("ValidateRequestLength", date1, date2);


                listResponseSecurities = new List<SecuritiesCollection>();
                dictUnprocessedInstruments = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                var bulkListInfo = new RBbgBulkListInfo();
                AdditionalInfo = new List<string>();
                ResponseAdditionalInfo = new List<string>();
                int VendorPreferenceId = listRequestedSecurities[0].VendorPreferenceId;

                if (listRequestedSecurities.Count == 0)
                    throw new RVendorException("No instruments to be processed");

                manaulResetEvent = new ManualResetEvent[listRequestedSecurities.Count];

                date1 = DateTime.Now;
                if (listRequestedSecurities[0].RequestType == RBbgRequestType.Lite)
                {
                    ExecuteRequestInThreadForLite(listRequestedSecurities, immediateRequest, gd);
                }
                else if (listRequestedSecurities[0].RequestType == RBbgRequestType.FTP)
                    ExecuteRequestInThreadForFTP(listRequestedSecurities, immediateRequest, gd);
                else
                    ExecuteRequestInThread(listRequestedSecurities, immediateRequest, gd);

                //Main Thread wait for all worker threads to complete their processing.
                WaitHandle.WaitAll(manaulResetEvent);

                date2 = DateTime.Now;
                RVendorUtils.InsertLogTable("ExecuteRequestInThreadForFTP", date1, date2);

                if (listRequestedSecurities[0].RequestType == RBbgRequestType.Lite ||
                        listRequestedSecurities[0].RequestType == RBbgRequestType.Heavy ||
                        listRequestedSecurities[0].RequestType == RBbgRequestType.SAPI)
                    immediateRequest = true;

                if (!skipFlag)
                {
                    vendorResponse.RequestIdentifier = gd;
                    return vendorResponse;
                }

                #region Process Response
                if (immediateRequest)
                {
                    if (((RBbgSecurityInfo)requestedSecurities).IsBulkList)
                    {
                        if (listRequestedSecurities[0].RequestType == RBbgRequestType.FTP)
                        {
                            if (dictUnprocessedInstruments != null && dictUnprocessedInstruments.Count > 0)
                            {
                                foreach (string instrument in dictUnprocessedInstruments.Keys)
                                {
                                    foreach (var fieldInfo in dictUnprocessedInstruments[instrument].Values)
                                    {
                                        fieldInfo.Value = RVendorConstant.NOT_AVAILABLE;
                                        fieldInfo.Status = RVendorStatusConstant.FAILED;
                                    }
                                }
                            }
                        }

                        if (((RBbgSecurityInfo)requestedSecurities).IsCombinedFtpReq)
                            return RVendorUtils.ProcessResponseForCombinedRequest(listResponseSecurities, dictUnprocessedInstruments, (RBbgSecurityInfo)requestedSecurities, gd, AdditionalInfo, ResponseAdditionalInfo);
                        else
                            return RVendorUtils.ProcessResponseForBulkList(RSecurityReturnType.DataSet, listResponseSecurities, dictUnprocessedInstruments,
                                ((RBbgSecurityInfo)requestedSecurities), gd, AdditionalInfo, ResponseAdditionalInfo);
                    }
                    else
                    {
                        mLogger.Debug("Start -->begin ProcessResponse");

                        date1 = DateTime.Now;
                        bool requireNotAvailableInField = ((RBbgSecurityInfo)requestedSecurities).requireNotAvailableInField;
                        RVendorResponse objResponse = RVendorUtils.ProcessResponseSecurities(listResponseSecurities, (RBbgSecurityInfo)requestedSecurities, ref dictUnprocessedInstruments, returnType, gd, requireNotAvailableInField, ref dsResponseData, ref requestedBbgSecurities, lstBulkMnemonicsInMasterReq, lstBulkMnemonicsInLegReq, VendorPreferenceId, RVendorType.Bloomberg, AdditionalInfo, ResponseAdditionalInfo);
                        date2 = DateTime.Now;
                        RVendorUtils.InsertLogTable("ProcessResponseSecurities", date1, date2);

                        mLogger.Debug("End -->begin ProcessResponse");

                        if (((RBbgSecurityInfo)requestedSecurities).RequestType == RBbgRequestType.SAPI && !marketSectorSpecified && objResponse.ResponseStatus == RVendorResponseStatus.Failed)
                        {
                            List<string> mktSecotrList = Enum.GetNames(typeof(RBbgMarketSector)).ToList();
                            mktSecotrList.Remove("None");
                            //mktSecotrList.Remove("Equity");
                            foreach (string mktSector in mktSecotrList)
                            {
                                ((RBbgSecurityInfo)requestedSecurities).Instruments.ForEach(instr =>
                                {
                                    instr.MarketSector = (RBbgMarketSector)Enum.Parse(typeof(RBbgMarketSector), mktSector);
                                });
                                IVendor vendor = RVendorManager.GetVendor(RVendorType.Bloomberg, VendorPreferenceId);
                                RVendorResponse mktSectorResponse = this.GetSecurities(requestedSecurities, returnType, immediateRequest, ref dsResponseData, ref requestedBbgSecurities, lstBulkMnemonicsInMasterReq, lstBulkMnemonicsInLegReq);
                                if (mktSectorResponse.ResponseStatus == RVendorResponseStatus.Passed)
                                    return mktSectorResponse;
                            }
                            return objResponse;
                        }
                        else
                        {
                            return objResponse;
                        }
                    }
                }
                else
                    return RVendorUtils.ProcessResponse(gd, AdditionalInfo, ResponseAdditionalInfo);
                #endregion
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                RVendorResponse vendorResponse = RVendorUtils.GetDatabaseException(immediateRequest, rEx.Message);
                return vendorResponse;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                RVendorResponse vendorResponse = RVendorUtils.GetDatabaseException(immediateRequest, ex.Message);
                return vendorResponse;
            }
            finally
            {
                mLogger.Debug("End -> creating request object.");
            }
        }

        public RVendorResponse GetResponse(string requestIdentifier, string transportName, RSecurityReturnType returnType, int VendorPreferenceId)
        {
            SecuritiesCollection securityResponse = null;
            RBbgBulkListInfo bulkListInfo = null;
            //ManualResetEvent[] manualReset = null;
            RBbgSecurityInfo securityInfo = null;
            List<RBbgInstrumentInfo> lstInstrument = null;
            List<RBbgFieldInfo> lstFields = null;
            dictUnprocessedInstruments = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            securityInfo = new RBbgSecurityInfo() { VendorPreferenceId = VendorPreferenceId };
            lstInstrument = new List<RBbgInstrumentInfo>();
            listResponseSecurities = new List<SecuritiesCollection>();
            lstFields = new List<RBbgFieldInfo>();
            bulkListInfo = new RBbgBulkListInfo();
            try
            {
                string transportNameVendorPreference = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, VendorPreferenceId, RVendorConstant.TRANSPORTNAME);

                if (string.IsNullOrEmpty(transportName) && !string.IsNullOrEmpty(transportNameVendorPreference))
                {
                    transportName = transportNameVendorPreference;
                }

                string workingDirectory = RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                string fileName = requestIdentifier + ".xml";
                if (File.Exists(Path.Combine(workingDirectory, fileName)))
                {
                    DataTable responseData = new DataTable();
                    responseData.ReadXml(Path.Combine(workingDirectory, fileName));
                    RVendorResponse res = new RVendorResponse();
                    DataSet resultantDataset = new DataSet();
                    resultantDataset.Tables.Add(responseData);
                    res.SecurityResponse = resultantDataset;
                    return res;
                }
                else
                {
                    List<RVendorHistoryInfo> historyList = RVendorUtils.GetVendorHistory(requestIdentifier);
                    securityInfo = RBbgUtils.PopulateSecurityInfoFromVendorHist(historyList);
                    securityInfo.VendorPreferenceId = VendorPreferenceId;

                    if (historyList.Count > 0)
                    {
                        manaulResetEvent = new ManualResetEvent[historyList.Count];
                        RBbgRequestProcess objRequestProcess = new RBbgRequestProcess();
                        for (int i = 0; i < historyList.Count; i++)
                        {
                            manaulResetEvent[i] = new ManualResetEvent(false);
                            historyList[i].TransportName = transportName;
                            historyList[i].CurrentObject = this;
                            historyList[i].ManualReset = manaulResetEvent[i];
                            historyList[i].VendorPreferenceId = VendorPreferenceId;
                            historyList[i].ClientName = SRMMTConfig.GetClientName();
                            ThreadPool.QueueUserWorkItem(objRequestProcess.ProcessFTPResponse, historyList[i]);
                            //objRequestProcess.ProcessFTPResponse(historyList[i]);
                        }
                        //main thread waits for all worker threads to finish their processing.
                        WaitHandle.WaitAll(manaulResetEvent);

                        if (!skipFlag)
                        {
                            vendorResponse.RequestIdentifier = requestIdentifier;
                            return vendorResponse;
                        }

                        #region Process Response
                        if (securityInfo.IsBulkList)
                        {
                            if (dictUnprocessedInstruments != null && dictUnprocessedInstruments.Count > 0)
                            {
                                foreach (string instrument in dictUnprocessedInstruments.Keys)
                                {
                                    foreach (var fieldInfo in dictUnprocessedInstruments[instrument].Values)
                                    {
                                        fieldInfo.Value = RVendorConstant.NOT_AVAILABLE;
                                        fieldInfo.Status = RVendorStatusConstant.FAILED;
                                    }
                                }
                            }

                            return RVendorUtils.ProcessResponseForBulkList(RSecurityReturnType.DataSet, listResponseSecurities, dictUnprocessedInstruments, securityInfo, requestIdentifier);
                        }
                        else
                        {
                            //securityResponse = RBbgUtils.NormalizeSecurities(listResponseSecurities, securityInfo, ref dictUnprocessedInstruments);
                            //return RVendorUtils.ProcessResponse(returnType, securityResponse, dictUnprocessedInstruments, requestIdentifier);

                            return RVendorUtils.ProcessResponseSecurities(listResponseSecurities, securityInfo, ref dictUnprocessedInstruments, returnType, requestIdentifier, true, VendorPreferenceId, RVendorType.Bloomberg);
                        }
                        #endregion
                    }
                    else
                        throw new RVendorException("Invalid Request Identifier");
                }
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                RVendorResponse vendorResponse = RVendorUtils.GetDatabaseException(true, rEx.Message);
                return vendorResponse;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                RVendorResponse vendorResponse = RVendorUtils.GetDatabaseException(true, ex.Message);
                return vendorResponse;
            }
            finally
            {
                mLogger.Debug("End -> creating request object.");
            }
        }

        public RVendorResponse GetResponse(string requestIdentifier, string transportName, RSecurityReturnType returnType)
        {
            return GetResponse(requestIdentifier, transportName, returnType, 1);
        }

        public RVendorResponse GetSecurities(object requestedSecurities)
        {
            DataSet dsResponseData = null;
            List<RBbgSecurityInfo> requestedBbgSecurities = new List<RBbgSecurityInfo>();
            RBbgSecurityInfo info = null;
            RVendorResponse response = null;
            List<string> requestIDs = null;
            List<string> errors = null;
            //DataSet ds = new DataSet();
            List<string> AdditionalInfo = new List<string>();
            List<string> ResponseAdditionalInfo = new List<string>();
            try
            {
                mLogger.Debug("Start -> begin GetResponse.");
                if (((List<RBbgSecurityInfo>)requestedSecurities) != null && ((List<RBbgSecurityInfo>)requestedSecurities).Count > 0)
                {
                    requestIDs = new List<string>();
                    errors = new List<string>();
                    // ((List<RBbgSecurityInfo>)requestedSecurities).ForEach(q => requestedBbgSecurities.Add(q));

                    var instrumentFields = ((List<RBbgSecurityInfo>)requestedSecurities)[0].InstrumentFields;
                    var bulkListMapId = ((List<RBbgSecurityInfo>)requestedSecurities)[0].BulkListMapId;

                    var lstRequestedMasterFields = new HashSet<string>(instrumentFields.Select(x => x.Mnemonic));
                    var lstBulkMnemonicsInMasterReq = new List<string>();
                    var lstBulkMnemonicsInLegReq = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

                    if (bulkListMapId != null && bulkListMapId.Count > 0)
                    {
                        foreach (int bulkListId in bulkListMapId.Distinct().ToList())
                        {
                            var bulkListInfo = RVendorUtils.GetBulkListInfo(bulkListId);
                            string bulkMnemonic = bulkListInfo.RequestField.Trim();
                            var bulkMnemonicColNames = bulkListInfo.OutputFields.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                            lstBulkMnemonicsInLegReq.Add(bulkMnemonic, bulkMnemonicColNames);

                            if (lstRequestedMasterFields.Contains(bulkMnemonic))
                            {
                                lstBulkMnemonicsInMasterReq.Add(bulkMnemonic);
                            }
                            else
                            {
                                var bulkMnemonicfieldInfo = new RBbgFieldInfo { Mnemonic = bulkMnemonic };
                                instrumentFields.Add(bulkMnemonicfieldInfo);
                            }
                        }
                    }

                    foreach (RBbgSecurityInfo reqInfo in (List<RBbgSecurityInfo>)requestedSecurities)
                    {
                        reqInfo.InstrumentFields = instrumentFields;
                        List<RBbgInstrumentInfo> instInfo = new List<RBbgInstrumentInfo>();
                        foreach (RBbgInstrumentInfo insInfo in reqInfo.Instruments)
                            instInfo.Add((RBbgInstrumentInfo)insInfo.Clone());
                        RBbgSecurityInfo copyInfo = GetRbbgInfo(reqInfo, instInfo);
                        requestedBbgSecurities.Add(copyInfo);
                    }
                    int VendorPreferenceId = requestedBbgSecurities[0].VendorPreferenceId;
                    bool requireNotAvailableInField = requestedBbgSecurities[0].requireNotAvailableInField;

                    if (!requestedBbgSecurities[0].IsBulkList && requestedBbgSecurities[0].RequestType == RBbgRequestType.FTP)
                        dsResponseData = CreateResponseTableWithDefaultValues(requestedBbgSecurities[0], true, lstBulkMnemonicsInMasterReq, lstBulkMnemonicsInLegReq);
                    else
                        dsResponseData = CreateResponseTable(requestedBbgSecurities[0], lstBulkMnemonicsInMasterReq, lstBulkMnemonicsInLegReq);
                    bool workDone = false;
                    while (!workDone)
                    {
                        List<RBbgInstrumentInfo> instruments = GetInstrumentsFromSecurities(requestedBbgSecurities);
                        info = GetRbbgInfo(requestedBbgSecurities[0], instruments);
                        string InstrumentIdType = info.Instruments[0].InstrumentIdType.ToString();

                        if (info.IsBulkList)
                            response = ((IVendor)this).GetSecurities(info, RSecurityReturnType.DataSet, true);
                        else
                        {
                            object obj = requestedBbgSecurities;
                            response = this.GetSecurities(info, RSecurityReturnType.DataSet, true, ref dsResponseData, ref obj, lstBulkMnemonicsInMasterReq, lstBulkMnemonicsInLegReq);
                            requestedBbgSecurities = (List<RBbgSecurityInfo>)obj;
                        }

                        if (response.AdditionalInfo != null && response.AdditionalInfo.Count > 0)
                            AdditionalInfo.AddRange(response.AdditionalInfo);
                        if (response.ResponseAdditionalInfo != null && response.ResponseAdditionalInfo.Count > 0)
                            ResponseAdditionalInfo.AddRange(response.ResponseAdditionalInfo);
                        if (!string.IsNullOrWhiteSpace(response.ExceptionMessage))
                        {
                            errors.Add(response.ExceptionMessage);
                            //errors.Add(response.ExceptionMessage + " for => " + InstrumentIdType);
                        }
                        requestIDs.Add(response.RequestIdentifier);

                        if (info.IsBulkList)
                            FillDataIntoTable(response, dsResponseData.Tables[0], ref requestedBbgSecurities);
                        if (!(requestedBbgSecurities.Count != 0 && requestedBbgSecurities.Any(q => q.Instruments.Count > 0)))
                        {
                            workDone = true;
                        }
                    }
                    if (((List<RBbgSecurityInfo>)requestedSecurities).Any(q => q.ImmediateRequest == true))
                    {
                        mLogger.Debug("End -> End GetResponse.");
                        RVendorResponse res = new RVendorResponse();
                        if (dsResponseData.Tables[0].Rows.Count == 0)
                        {
                            res.ExceptionMessage = string.Join(".", errors);
                            res.SecurityResponse = null;
                            mLogger.Error("Output from bbg is blank");
                        }
                        else
                        {
                            //ds.Tables.Add(responseData);
                            res.SecurityResponse = dsResponseData;

                            if (RVendorUtils.IsRealtimeDebugMode())
                            {
                                StringWriter writer = new StringWriter();

                                try
                                {
                                    dsResponseData.WriteXml(writer);
                                    mLogger.Error("Output from bbg : " + writer.ToString());
                                }
                                catch (Exception ex)
                                {
                                    mLogger.Error("Exception occured while printing Output from bbg : " + ex.ToString());
                                }
                            }
                        }
                        res.RequestIdList = requestIDs;
                        res.AdditionalInfo = AdditionalInfo;
                        res.ResponseAdditionalInfo = ResponseAdditionalInfo;
                        res.requireNotAvailableInField = requireNotAvailableInField;
                        return res;
                    }
                    else
                    {
                        RVendorResponse res = new RVendorResponse();
                        if (dsResponseData.Tables[0].Rows.Count == 0)
                        {
                            res.ExceptionMessage = string.Join(".", errors);
                            res.SecurityResponse = null;
                        }
                        else
                        {
                            string workingDirectory = RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                            string guid = Guid.NewGuid().ToString();
                            string fileName = guid + ".xml";
                            dsResponseData.Tables[0].WriteXml(Path.Combine(workingDirectory, fileName), XmlWriteMode.WriteSchema);
                            res.RequestIdentifier = guid;
                            res.SecurityResponse = guid;
                        }
                        mLogger.Debug("End -> End GetResponse.");
                        res.RequestIdList = requestIDs;
                        res.AdditionalInfo = AdditionalInfo;
                        res.ResponseAdditionalInfo = ResponseAdditionalInfo;
                        return res;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                RVendorResponse res = new RVendorResponse();
                res.ExceptionMessage = "Data not available";
                res.ResponseStatus = RVendorResponseStatus.Failed;
                return res;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                RVendorResponse res = new RVendorResponse();
                res.ExceptionMessage = "Data not available";
                res.ResponseStatus = RVendorResponseStatus.Failed;
                return res;
            }
        }

        public RVendorResponse GetSecuritiesForSingleIdentifier(RBbgSecurityInfo requestedSecurity)
        {
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;

            DataSet dsResponseData = null;
            //List<RBbgSecurityInfo> requestedBbgSecurities = new List<RBbgSecurityInfo>();
            //RBbgSecurityInfo info = null;
            RVendorResponse response = null;
            List<string> requestIDs = null;
            List<string> errors = null;
            //DataSet ds = new DataSet();
            List<string> AdditionalInfo = new List<string>();
            List<string> ResponseAdditionalInfo = new List<string>();
            try
            {
                mLogger.Debug("Start -> begin GetResponse.");
                if (requestedSecurity != null && requestedSecurity.Instruments != null && requestedSecurity.Instruments.Count > 0)
                {
                    requestedSecurity.InstrumentIdentifierType = requestedSecurity.Instruments[0].InstrumentIdType;
                    var bulkListMapId = requestedSecurity.BulkListMapId;

                    var lstRequestedMasterFields = new HashSet<string>(requestedSecurity.InstrumentFields.Select(x => x.Mnemonic));
                    var lstBulkMnemonicsInMasterReq = new List<string>();
                    var lstBulkMnemonicsInLegReq = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

                    if (bulkListMapId != null && bulkListMapId.Count > 0)
                    {
                        foreach (int bulkListId in bulkListMapId.Distinct().ToList())
                        {
                            var bulkListInfo = RVendorUtils.GetBulkListInfo(bulkListId);
                            string bulkMnemonic = bulkListInfo.RequestField.Trim();
                            var bulkMnemonicColNames = bulkListInfo.OutputFields.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

                            lstBulkMnemonicsInLegReq.Add(bulkMnemonic, bulkMnemonicColNames);

                            if (lstRequestedMasterFields.Contains(bulkMnemonic))
                            {
                                lstBulkMnemonicsInMasterReq.Add(bulkMnemonic);
                            }
                            else
                            {
                                var bulkMnemonicfieldInfo = new RBbgFieldInfo { Mnemonic = bulkMnemonic };
                                requestedSecurity.InstrumentFields.Add(bulkMnemonicfieldInfo);
                            }
                        }
                    }

                    requestIDs = new List<string>();
                    errors = new List<string>();
                    //foreach (RBbgSecurityInfo reqInfo in (List<RBbgSecurityInfo>)requestedSecurities)
                    //{
                    //    List<RBbgInstrumentInfo> instInfo = new List<RBbgInstrumentInfo>();
                    //    foreach (RBbgInstrumentInfo insInfo in reqInfo.Instruments)
                    //        instInfo.Add((RBbgInstrumentInfo)insInfo.Clone());
                    //    RBbgSecurityInfo copyInfo = GetRbbgInfo(reqInfo, instInfo);
                    //    requestedBbgSecurities.Add(copyInfo);
                    //}
                    int VendorPreferenceId = requestedSecurity.VendorPreferenceId;

                    if (!requestedSecurity.IsBulkList && requestedSecurity.RequestType == RBbgRequestType.FTP)
                        dsResponseData = CreateResponseTableWithDefaultValues(requestedSecurity, requestedSecurity.requireNotAvailableInField, lstBulkMnemonicsInMasterReq, lstBulkMnemonicsInLegReq);
                    else
                        dsResponseData = CreateResponseTable(requestedSecurity, lstBulkMnemonicsInMasterReq, lstBulkMnemonicsInLegReq);
                    //bool workDone = false;
                    //while (!workDone)
                    //{
                    //List<RBbgInstrumentInfo> instruments = GetInstrumentsFromSecurities(requestedBbgSecurities);
                    //info = GetRbbgInfo(requestedBbgSecurities[0], instruments);

                    if (requestedSecurity.IsBulkList)
                        response = ((IVendor)this).GetSecurities(requestedSecurity, RSecurityReturnType.DataSet, true);
                    else
                    {
                        DateTime date1a = DateTime.Now;

                        object obj = requestedSecurity;
                        response = this.GetSecurities(requestedSecurity, RSecurityReturnType.DataSet, true, ref dsResponseData, ref obj, lstBulkMnemonicsInMasterReq, lstBulkMnemonicsInLegReq);
                        requestedSecurity = (RBbgSecurityInfo)obj;

                        DateTime date2a = DateTime.Now;
                        RVendorUtils.InsertLogTable("GetSecurities", date1a, date2a);
                    }

                    if (response.AdditionalInfo != null && response.AdditionalInfo.Count > 0)
                        AdditionalInfo.AddRange(response.AdditionalInfo);
                    if (response.ResponseAdditionalInfo != null && response.ResponseAdditionalInfo.Count > 0)
                        ResponseAdditionalInfo.AddRange(response.ResponseAdditionalInfo);
                    if (!string.IsNullOrWhiteSpace(response.ExceptionMessage))
                    {
                        errors.Add(response.ExceptionMessage);
                    }
                    requestIDs.Add(response.RequestIdentifier);

                    if (requestedSecurity.IsBulkList)
                        FillDataIntoTable(response, dsResponseData.Tables[0]);
                    //    if (!(requestedBbgSecurities.Count != 0 && requestedBbgSecurities.Any(q => q.Instruments.Count > 0)))
                    //    {
                    //        workDone = true;
                    //    }
                    //}
                    if (requestedSecurity.ImmediateRequest == true)
                    {
                        mLogger.Debug("End -> End GetResponse.");
                        RVendorResponse res = new RVendorResponse();
                        if (dsResponseData.Tables[0].Rows.Count == 0)
                        {
                            res.ExceptionMessage = string.Join(".", errors);
                            res.SecurityResponse = null;
                            mLogger.Error("Output from bbg is blank");
                        }
                        else
                        {
                            //ds.Tables.Add(responseData);
                            res.SecurityResponse = dsResponseData;

                            if (RVendorUtils.IsRealtimeDebugMode())
                            {
                                StringWriter writer = new StringWriter();

                                try
                                {
                                    dsResponseData.WriteXml(writer);
                                    mLogger.Error("Output from bbg : " + writer.ToString());
                                }
                                catch(Exception ex)
                                {
                                    mLogger.Error("Exception occured while printing Output from bbg : " + ex.ToString());
                                }
                            }
                        }
                        res.RequestIdList = requestIDs;
                        res.AdditionalInfo = AdditionalInfo;
                        res.ResponseAdditionalInfo = ResponseAdditionalInfo;
                        res.requireNotAvailableInField = requestedSecurity.requireNotAvailableInField;
                        return res;
                    }
                    else
                    {
                        RVendorResponse res = new RVendorResponse();
                        if (dsResponseData.Tables[0].Rows.Count == 0)
                        {
                            res.ExceptionMessage = string.Join(".", errors);
                            res.SecurityResponse = null;
                        }
                        else
                        {
                            string workingDirectory = RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                            string guid = Guid.NewGuid().ToString();
                            string fileName = guid + ".xml";
                            dsResponseData.Tables[0].WriteXml(Path.Combine(workingDirectory, fileName), XmlWriteMode.WriteSchema);
                            res.RequestIdentifier = guid;
                            res.SecurityResponse = guid;
                        }
                        mLogger.Debug("End -> End GetResponse.");
                        res.RequestIdList = requestIDs;
                        res.AdditionalInfo = AdditionalInfo;
                        res.ResponseAdditionalInfo = ResponseAdditionalInfo;
                        return res;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                RVendorResponse res = new RVendorResponse();
                res.ExceptionMessage = "Data not available";
                res.ResponseStatus = RVendorResponseStatus.Failed;
                return res;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                RVendorResponse res = new RVendorResponse();
                res.ExceptionMessage = "Data not available";
                res.ResponseStatus = RVendorResponseStatus.Failed;
                return res;
            }
            finally
            {
                date2 = DateTime.Now;
                RVendorUtils.InsertLogTable("GetSecuritiesForSingleIdentifier", date1, date2);
            }
        }

        public RVendorResponse GetResponse(string requestId, int VendorPreferenceId)
        {
            try
            {
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                string fileName = requestId + ".xml";
                if (File.Exists(Path.Combine(workingDirectory, fileName)))
                {
                    DataTable responseData = new DataTable();
                    responseData.ReadXml(Path.Combine(workingDirectory, fileName));
                    RVendorResponse res = new RVendorResponse();
                    DataSet resultantDataset = new DataSet();
                    resultantDataset.Tables.Add(responseData);
                    res.SecurityResponse = resultantDataset;
                    return res;
                }
                return null;
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            return null;
        }

        public RVendorResponse GetResponse(string requestId)
        {
            return GetResponse(requestId, 1);
        }

        private void FillDataIntoTable(RVendorResponse response, DataTable responseData, ref List<RBbgSecurityInfo> requestedBbgSecurities)
        {
            int maxInstrumentCount = 0;
            if (response != null && response.SecurityResponse != null)
            {
                if (requestedBbgSecurities != null && requestedBbgSecurities.Count > 0 && requestedBbgSecurities[0].Instruments.Count > 0)
                {
                    maxInstrumentCount = requestedBbgSecurities[0].Instruments.Count;
                }
                for (int counter = 0; counter < ((DataSet)response.SecurityResponse).Tables[0].Rows.Count;)
                {
                    var drSecurityResponse = ((DataSet)response.SecurityResponse).Tables[0].Rows[counter];
                    if (!drSecurityResponse["Failure_reason"].ToString().ToUpper().Equals("NOT PROCESSED"))
                    {
                        DataRow drResponseData = responseData.NewRow();
                        drResponseData["Request Identifier"] = response.RequestIdentifier;
                        drResponseData["Security ID"] = requestedBbgSecurities[counter].SecurityId;
                        foreach (DataColumn dc in ((DataSet)response.SecurityResponse).Tables[0].Columns)
                        {
                            drResponseData[dc.ColumnName] = drSecurityResponse[dc];
                        }
                        responseData.Rows.Add(drResponseData);
                        requestedBbgSecurities = requestedBbgSecurities.Where(q => q.Instruments[0].InstrumentID != drSecurityResponse["INSTRUMENT"].ToString()).ToList();
                    }
                    else
                    {
                        foreach (RBbgSecurityInfo secInfo in requestedBbgSecurities)
                        {
                            if (secInfo.Instruments.Count > 0 && secInfo.Instruments.Count == maxInstrumentCount && secInfo.Instruments[0].InstrumentID.Equals(drSecurityResponse["INSTRUMENT"].ToString()))
                                secInfo.Instruments.RemoveAt(0);
                        }
                    }
                    ((DataSet)response.SecurityResponse).Tables[0].Rows.RemoveAt(counter);
                }
            }
            else
            {
                requestedBbgSecurities.ForEach(q =>
                {
                    if (q.Instruments.Count > 0)
                        q.Instruments.RemoveAt(0);
                });
            }
        }

        private void FillDataIntoTable(RVendorResponse response, DataTable responseData)
        {
            if (response != null && response.SecurityResponse != null)
            {
                for (int counter = 0; counter < ((DataSet)response.SecurityResponse).Tables[0].Rows.Count;)
                {
                    var drSecurityResponse = ((DataSet)response.SecurityResponse).Tables[0].Rows[counter];
                    if (!drSecurityResponse["Failure_reason"].ToString().ToUpper().Equals("NOT PROCESSED"))
                    {
                        DataRow drResponseData = responseData.NewRow();
                        drResponseData["Request Identifier"] = response.RequestIdentifier;
                        drResponseData["Security ID"] = drSecurityResponse["INSTRUMENT"].ToString();
                        foreach (DataColumn dc in ((DataSet)response.SecurityResponse).Tables[0].Columns)
                        {
                            drResponseData[dc.ColumnName] = drSecurityResponse[dc];
                        }
                        responseData.Rows.Add(drResponseData);
                    }
                    ((DataSet)response.SecurityResponse).Tables[0].Rows.RemoveAt(counter);
                }
            }
        }

        private List<RBbgInstrumentInfo> GetInstrumentsFromSecurities(List<RBbgSecurityInfo> requestedBbgSecurities)
        {
            List<RBbgInstrumentInfo> instruments = new List<RBbgInstrumentInfo>();
            requestedBbgSecurities.ForEach(sec =>
            {
                if (sec.Instruments.Count > 0)
                    instruments.Add((RBbgInstrumentInfo)sec.Instruments[0].Clone());
            });
            return instruments;
        }

        private RBbgSecurityInfo GetRbbgInfo(object requestedSecurities, List<RBbgInstrumentInfo> instruments)
        {
            RBbgSecurityInfo info = CreateCopy((RBbgSecurityInfo)requestedSecurities, instruments[0]);
            info.Instruments.AddRange(instruments);
            info.InstrumentFields = ((RBbgSecurityInfo)requestedSecurities).InstrumentFields;
            if (((RBbgSecurityInfo)requestedSecurities).HeaderNamesVsValues != null && ((RBbgSecurityInfo)requestedSecurities).HeaderNamesVsValues.Count > 0)
                info.HeaderNamesVsValues = ((RBbgSecurityInfo)requestedSecurities).HeaderNamesVsValues;
            info.VendorPreferenceId = ((RBbgSecurityInfo)requestedSecurities).VendorPreferenceId;
            return info;
        }

        private DataSet CreateResponseTable(object requestedSecurities, List<string> lstBulkMnemonicsInMasterReq, Dictionary<string, List<string>> lstBulkMnemonicsInLegReq)
        {
            DataSet dsResponse = new DataSet();
            DataTable reponseData = new DataTable("ResponseTable");
            dsResponse.Tables.Add(reponseData);

            DataColumn column = new DataColumn("Request Identifier");
            reponseData.Columns.Add(column);
            column = new DataColumn("Security ID");
            reponseData.Columns.Add(column);
            column = new DataColumn("INSTRUMENT");
            reponseData.Columns.Add(column);
            column = new DataColumn("is_valid");
            reponseData.Columns.Add(column);
            column = new DataColumn("failure_reason");
            reponseData.Columns.Add(column);
            column = new DataColumn("DL_Asset_Class");
            reponseData.Columns.Add(column);
            column = new DataColumn("MARKET_SECTOR_DES");
            reponseData.Columns.Add(column);

            var lstMasterFields = ((RBbgSecurityInfo)requestedSecurities).InstrumentFields.Select(x => x.Mnemonic).Distinct(StringComparer.OrdinalIgnoreCase).Except(lstBulkMnemonicsInLegReq.Keys.Except(lstBulkMnemonicsInMasterReq, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase).ToList();
            lstMasterFields.ForEach(field =>
            {
                if (!reponseData.Columns.Contains(field))
                {
                    column = new DataColumn(field);
                    reponseData.Columns.Add(column);
                }
            });

            foreach (string bulkMnemonic in lstBulkMnemonicsInLegReq.Keys)
            {
                DataTable reponseLegData = new DataTable(bulkMnemonic);
                dsResponse.Tables.Add(reponseLegData);

                column = new DataColumn("INSTRUMENT");
                reponseLegData.Columns.Add(column);
                foreach (string bulkMnemonicColumn in lstBulkMnemonicsInLegReq[bulkMnemonic])
                {
                    column = new DataColumn(bulkMnemonicColumn, typeof(string));
                    column.DefaultValue = RVendorConstant.NOT_AVAILABLE;
                    reponseLegData.Columns.Add(column);
                }
                column = new DataColumn("is_valid");
                reponseLegData.Columns.Add(column);
            }
            return dsResponse;
        }

        private DataSet CreateResponseTableWithDefaultValues(object requestedSecurities, bool requireNotAvailableInField, List<string> lstBulkMnemonicsInMasterReq, Dictionary<string, List<string>> lstBulkMnemonicsInLegReq)
        {
            DataSet dsResponse = new DataSet();
            DataTable reponseData = new DataTable("ResponseTable");
            dsResponse.Tables.Add(reponseData);

            DataColumn column = new DataColumn("Request Identifier");
            reponseData.Columns.Add(column);
            column = new DataColumn("Security ID");
            reponseData.Columns.Add(column);
            column = new DataColumn("INSTRUMENT");
            reponseData.Columns.Add(column);
            column = new DataColumn("is_valid");
            reponseData.Columns.Add(column);
            column = new DataColumn("failure_reason");
            reponseData.Columns.Add(column);
            column = new DataColumn("DL_Asset_Class");
            reponseData.Columns.Add(column);
            column = new DataColumn("MARKET_SECTOR_DES");
            reponseData.Columns.Add(column);

            var lstMasterFields = ((RBbgSecurityInfo)requestedSecurities).InstrumentFields.Select(x => x.Mnemonic).Distinct(StringComparer.OrdinalIgnoreCase).Except(lstBulkMnemonicsInLegReq.Keys.Except(lstBulkMnemonicsInMasterReq, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase).ToList();
            lstMasterFields.ForEach(field =>
            {
                if (!reponseData.Columns.Contains(field))
                {
                    column = new DataColumn(field, typeof(string));

                    if (requireNotAvailableInField)
                        column.DefaultValue = RVendorConstant.NOT_AVAILABLE;

                    reponseData.Columns.Add(column);
                }
            });

            foreach (string bulkMnemonic in lstBulkMnemonicsInLegReq.Keys)
            {
                DataTable reponseLegData = new DataTable(bulkMnemonic);
                dsResponse.Tables.Add(reponseLegData);

                column = new DataColumn("INSTRUMENT");
                reponseLegData.Columns.Add(column);
                foreach (string bulkMnemonicColumn in lstBulkMnemonicsInLegReq[bulkMnemonic])
                {
                    column = new DataColumn(bulkMnemonicColumn, typeof(string));

                    if (requireNotAvailableInField)
                        column.DefaultValue = RVendorConstant.NOT_AVAILABLE;

                    reponseLegData.Columns.Add(column);
                }
                column = new DataColumn("is_valid");
                reponseLegData.Columns.Add(column);
            }

            return dsResponse;
        }

        private RBbgSecurityInfo CreateCopy(RBbgSecurityInfo rBbgSecurityInfo, RBbgInstrumentInfo rBbgInstrumentInfo)
        {
            RBbgSecurityInfo info = new RBbgSecurityInfo();
            info.BulkListMapId = rBbgSecurityInfo.BulkListMapId;
            info.CurrentObject = rBbgSecurityInfo.CurrentObject;
            info.ImmediateRequest = rBbgSecurityInfo.ImmediateRequest;
            info.InstrumentIdentifierType = rBbgInstrumentInfo.InstrumentIdType;
            info.IPAddresses = rBbgSecurityInfo.IPAddresses;
            info.IsBulkList = rBbgSecurityInfo.IsBulkList;
            info.MacroInfo = rBbgSecurityInfo.MacroInfo;
            info.ManualResetEvent = rBbgSecurityInfo.ManualResetEvent;
            info.MarketSector = rBbgSecurityInfo.MarketSector;
            info.ModuleId = rBbgSecurityInfo.ModuleId;
            info.RequestIdentifier = rBbgSecurityInfo.RequestIdentifier;
            info.RequestType = rBbgSecurityInfo.RequestType;
            info.SAPIUserName = rBbgSecurityInfo.SAPIUserName;
            info.TransportName = rBbgSecurityInfo.TransportName;
            info.UserLoginName = rBbgSecurityInfo.UserLoginName;
            info.requireNotAvailableInField = rBbgSecurityInfo.requireNotAvailableInField;
            info.VendorPreferenceId = rBbgSecurityInfo.VendorPreferenceId;
            return info;
        }
        #endregion

        #region Helper Methods
        private void ExecuteRequestInThreadForLite(List<RBbgSecurityInfo> listRequestedSecurities, bool immediateRequest, string gd)
        {
            int countIndex = 0;
            Dictionary<int, List<RBbgSecurityInfo>> dictLiteSec = RBbgUtils.
                                        GetListofRequestedSecuritiesForLite(listRequestedSecurities);
            for (int i = 0; i < dictLiteSec.Count; i++)
            {
                if (i > 0)
                    Thread.Sleep(60000);//causes next request to be processed after 1 min.
                for (int k = 0; k < dictLiteSec[i].Count; k++)
                {
                    manaulResetEvent[countIndex] = new ManualResetEvent(false);
                    #region set properties
                    dictLiteSec[i][k].ImmediateRequest = immediateRequest;
                    dictLiteSec[i][k].RequestIdentifier = gd;
                    dictLiteSec[i][k].ManualResetEvent = manaulResetEvent[countIndex];
                    dictLiteSec[i][k].CurrentObject = this;
                    dictLiteSec[i][k].ClientName = SRMMTConfig.GetClientName();
                    #endregion
                    ThreadPool.QueueUserWorkItem(new RBbgRequestProcess().ProcessRequest, dictLiteSec[i][k]);
                    countIndex++;
                }
            }
        }
        private void ExecuteRequestInThread(List<RBbgSecurityInfo> listRequestedSecurities, bool immediateRequest, string gd)
        {
            for (int i = 0; i < listRequestedSecurities.Count; i++)
            {
                manaulResetEvent[i] = new ManualResetEvent(false);
                #region set properties
                listRequestedSecurities[i].ImmediateRequest = immediateRequest;
                listRequestedSecurities[i].RequestIdentifier = gd;
                listRequestedSecurities[i].ManualResetEvent = manaulResetEvent[i];
                listRequestedSecurities[i].CurrentObject = this;
                listRequestedSecurities[i].ClientName = SRMMTConfig.GetClientName();

                #endregion
                ThreadPool.QueueUserWorkItem(new RBbgRequestProcess().ProcessRequest, listRequestedSecurities[i]);
            }
        }

        private void ExecuteRequestInThreadForFTP(List<RBbgSecurityInfo> listRequestedSecurities, bool immediateRequest, string gd)
        {
            //if (listRequestedSecurities[0].InstrumentFields.Count > listRequestedSecurities[1].InstrumentFields.Count)
            //{
            //    listRequestedSecurities[0].counter = 0;
            //    listRequestedSecurities[1].counter = 1;
            //}
            //else
            //{
            //    listRequestedSecurities[1].counter = 0;
            //    listRequestedSecurities[0].counter = 1;
            //}

            for (int i = 0; i < listRequestedSecurities.Count; i++)
            {
                manaulResetEvent[i] = new ManualResetEvent(false);
                #region set properties
                listRequestedSecurities[i].ImmediateRequest = immediateRequest;
                listRequestedSecurities[i].RequestIdentifier = gd;
                listRequestedSecurities[i].ManualResetEvent = manaulResetEvent[i];
                listRequestedSecurities[i].CurrentObject = this;
                listRequestedSecurities[i].AdditionalInfo = new List<string>();
                listRequestedSecurities[i].ResponseAdditionalInfo = new List<string>();
                listRequestedSecurities[i].ClientName = SRMMTConfig.GetClientName();

                #endregion
                ThreadPool.QueueUserWorkItem(new RBbgRequestProcess().ProcessRequest, listRequestedSecurities[i]);
                //objRequestProcess.ProcessRequest(listRequestedSecurities[i]);
            }
        }
        internal void UpdateAdditionalInfo(List<string> FTPAdditionalInfo, List<string> FTPResponseAdditionalInfo)
        {
            lock (additionalInfoLock)
            {
                if (FTPAdditionalInfo != null && FTPAdditionalInfo.Count > 0)
                {
                    AdditionalInfo.AddRange(FTPAdditionalInfo);
                }
                if (FTPResponseAdditionalInfo != null && FTPResponseAdditionalInfo.Count > 0)
                {
                    ResponseAdditionalInfo.AddRange(FTPResponseAdditionalInfo);
                }
            }
        }

        internal void AddVendorResponse(SecuritiesCollection response, SecuritiesCollection UnprocessedInstruments)
        {
            lock (responseLock)
            {
                if (response != null && response.Count > 0)
                    listResponseSecurities.Add(response);

                foreach (string instrument in UnprocessedInstruments.Keys)
                {
                    var dictFieldInfo = UnprocessedInstruments[instrument];
                    if (!dictUnprocessedInstruments.ContainsKey(instrument))
                        dictUnprocessedInstruments.Add(instrument, dictFieldInfo);
                    else
                    {
                        var dictFieldInfoPre = dictUnprocessedInstruments[instrument];
                        foreach (var fieldInfoKeyVal in dictFieldInfo)
                        {
                            if (!dictFieldInfoPre.ContainsKey(fieldInfoKeyVal.Key))
                                dictFieldInfoPre.Add(fieldInfoKeyVal.Key, fieldInfoKeyVal.Value);
                        }
                    }
                }
            }
        }

        internal void GenerateVendorResponseForError(RVendorResponse errorResponse)
        {
            lock (errorLock)
            {
                vendorResponse = errorResponse;
                skipFlag = false;
                foreach (ManualResetEvent manualEvent in manaulResetEvent)
                {
                    if (manualEvent != null && manualEvent.SafeWaitHandle.IsClosed == false)
                        manualEvent.Set();
                }
            }
        }
        #endregion

        #region Public Methods
        public List<RMacroConfigInfo> GetQualifiers(RBbgMacroType qualifierType)
        {
            string clientName = SRMMTConfig.GetClientName();

            List<RMacroConfigInfo> lstQualifier = null;
            try
            {
                lstQualifier = new List<RMacroConfigInfo>();
                if (RVendorConfigLoader.mMacroConfig.ContainsKey(clientName)  && RVendorConfigLoader.mMacroConfig[clientName].ContainsKey(qualifierType))
                    lstQualifier = RVendorConfigLoader.mMacroConfig[clientName][qualifierType];
                return lstQualifier;
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
                mLogger.Debug("End -> creating request object.");
            }
        }
        //------------------------------------------------------------------------------------------
        public List<RMacroConfigInfo> GetPrimaryQualifiers()
        {
            string clientName = SRMMTConfig.GetClientName();
            List<RMacroConfigInfo> lstQualifier = null;
            try
            {
                lstQualifier = new List<RMacroConfigInfo>();
                RVendorConfigLoader.LoadConfiguration(clientName);
                if (RVendorConfigLoader.mMacroConfig.ContainsKey(clientName) && RVendorConfigLoader.mMacroConfig[clientName].ContainsKey(RBbgMacroType.Primary))
                    lstQualifier = RVendorConfigLoader.mMacroConfig[clientName][RBbgMacroType.Primary];
                return lstQualifier;
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
                mLogger.Debug("End -> creating request object.");
            }
        }
        //------------------------------------------------------------------------------------------
        public List<RMacroConfigInfo> GetSecondaryQualifiers(string pqType, string pqValue)
        {
            string clientName = SRMMTConfig.GetClientName();
            List<RMacroConfigInfo> lstQualifier = null;
            List<RMacroConfigInfo> lstSecondaryQualifiers = null;
            Dictionary<string, string> dictCheckPQKey = null;
            try
            {
                lstQualifier = new List<RMacroConfigInfo>();
                lstSecondaryQualifiers = new List<RMacroConfigInfo>();
                dictCheckPQKey = new Dictionary<string, string>();
                if (RVendorConfigLoader.mMacroConfig.ContainsKey(clientName) && RVendorConfigLoader.mMacroConfig[clientName].ContainsKey(RBbgMacroType.Secondary))
                {
                    lstQualifier = RVendorConfigLoader.mMacroConfig[clientName][RBbgMacroType.Secondary];
                    if (lstQualifier.Count > 0)
                    {

                        foreach (RMacroConfigInfo macroConfigInfo in lstQualifier)
                        {
                            if (macroConfigInfo.PQType.Equals(pqType))
                            {
                                dictCheckPQKey.Clear();
                                if (macroConfigInfo.PQValues != null && ((List<string>)macroConfigInfo.PQValues).Count > 0)
                                {
                                    string[] values = macroConfigInfo.PQValues[0].Split(',');
                                    foreach (string key in values)
                                    {
                                        if (!dictCheckPQKey.ContainsKey(key))
                                            dictCheckPQKey.Add(key, key);
                                    }
                                    if (dictCheckPQKey.ContainsKey(pqValue))
                                        lstSecondaryQualifiers.Add(macroConfigInfo);
                                }
                                else
                                    lstSecondaryQualifiers.Add(macroConfigInfo);
                            }
                        }
                    }
                }
                return lstSecondaryQualifiers;
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
                mLogger.Debug("End -> creating request object.");
            }
        }

        public DataTable BloombergAlertDetails()
        {
            mLogger.Debug("begin BloombergAlertDetails");
            DataTable ResponseData = new DataTable();
            ResponseData.Columns.Add("Monthly Hard Limit");
            ResponseData.Columns.Add("Monthly Soft Limit");
            ResponseData.Columns.Add("Daily Hard Limit");
            ResponseData.Columns.Add("Daily Soft Limit");
            ResponseData.Columns.Add("Monthly Requested Securities");
            ResponseData.Columns.Add("Daily Requested Securities");
            string monthlyLimit = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\IVPVendor\\GetData", "Monthly Limit", string.Empty);
            mLogger.Debug("monthly=> " + monthlyLimit);
            string dailyLimit = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\IVPVendor\\GetData", "Daily Limit", string.Empty);
            mLogger.Debug("daily=> " + dailyLimit);
            DataSet data = RVendorUtils.GetRequestedCountFromDatabase(new HashSet<string>());
            int monthlyRequestedData = Convert.ToInt32(data.Tables[0].Rows[0][0]);
            int dailyRequestedData = Convert.ToInt32(data.Tables[1].Rows[0][0]);
            string[] monthlyLimitArray = (!string.IsNullOrEmpty(monthlyLimit) ? monthlyLimit.Split(new char[] { ',' }) : new string[0]);
            string[] dailyLimitArray = (!string.IsNullOrEmpty(dailyLimit) ? dailyLimit.Split(new char[] { ',' }) : new string[0]);
            DataRow AlertRow = ResponseData.NewRow();

            if (monthlyLimitArray.Length > 0)
            {
                AlertRow["Monthly Hard Limit"] = monthlyLimitArray[1];
                AlertRow["Monthly Soft Limit"] = monthlyLimitArray[0];
            }
            else
            {
                AlertRow["Monthly Hard Limit"] = 0;
                AlertRow["Monthly Soft Limit"] = 0;
            }
            if (dailyLimitArray.Length > 0)
            {
                AlertRow["Daily Hard Limit"] = dailyLimitArray[1];
                AlertRow["Daily Soft Limit"] = dailyLimitArray[0];
            }
            else
            {
                AlertRow["Daily Hard Limit"] = 0;
                AlertRow["Daily Soft Limit"] = 0;
            }
            AlertRow["Monthly Requested Securities"] = monthlyRequestedData;
            AlertRow["Daily Requested Securities"] = dailyRequestedData;
            ResponseData.Rows.Add(AlertRow);
            mLogger.Debug("end BloombergAlertDetails");
            return ResponseData;
        }

        public DataSet GetBloombergPricingDetailsOld(RbbgPricingType pricingType)
        {
            mLogger.Debug("begin GetBloombergPricingDetails");
            try
            {
                DataSet Result = new DataSet();
                DataTable totalDetails = new DataTable();
                totalDetails.Columns.Add("Cost");
                totalDetails.Columns.Add("Diff");
                DataTable drillDownDetails = new DataTable();
                drillDownDetails.Columns.Add("Description");
                drillDownDetails.Columns.Add("Amount");
                DateTime endDate = DateTime.Now;
                DateTime startDate = endDate.AddDays(-(endDate.Day - 1)).Date;
                DataSet CurrrentMonth = RVendorUtils.GetBloombergPricing(startDate, endDate);
                DataRow totalRow = null;
                DataSet previous = null;
                decimal totalCost = 0;
                switch (pricingType)
                {
                    case RbbgPricingType.MTD:
                        previous = RVendorUtils.GetBloombergPricing(startDate.AddMonths(-1), startDate.AddDays(-1));
                        totalRow = totalDetails.NewRow();
                        decimal prevCost = 0;
                        if (CurrrentMonth != null && CurrrentMonth.Tables.Count > 0 && CurrrentMonth.Tables[0].Rows.Count > 0 && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != null && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            totalCost = Convert.ToDecimal(CurrrentMonth.Tables[0].Rows[0]["Total Cost"]);
                        if (previous != null && previous.Tables.Count > 0 && previous.Tables[0].Rows.Count > 0 && previous.Tables[0].Rows[0]["Total Cost"] != null && previous.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            prevCost = Convert.ToDecimal(previous.Tables[0].Rows[0]["Total Cost"]);
                        totalRow[0] = totalCost.ToString();
                        totalRow[1] = prevCost - totalCost;
                        totalDetails.Rows.Add(totalRow);
                        drillDownDetails = RVendorUtils.GetDrillTableData(CurrrentMonth, new DataSet());
                        Result.Tables.Add(totalDetails);
                        Result.Tables.Add(drillDownDetails);
                        break;
                    case RbbgPricingType.WTD:
                        previous = RVendorUtils.GetBloombergPricing(startDate, endDate.AddDays(-(int)DateTime.Today.DayOfWeek + 1).Date);
                        totalRow = totalDetails.NewRow();
                        prevCost = 0;
                        if (CurrrentMonth != null && CurrrentMonth.Tables.Count > 0 && CurrrentMonth.Tables[0].Rows.Count > 0 && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != null && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            totalCost = Convert.ToDecimal(CurrrentMonth.Tables[0].Rows[0]["Total Cost"]);
                        if (previous != null && previous.Tables.Count > 0 && previous.Tables[0].Rows.Count > 0 && previous.Tables[0].Rows[0]["Total Cost"] != null && previous.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            prevCost = Convert.ToDecimal(previous.Tables[0].Rows[0]["Total Cost"]);
                        totalRow[0] = totalCost - prevCost;
                        totalDetails.Rows.Add(totalRow);
                        drillDownDetails = RVendorUtils.GetDrillTableData(CurrrentMonth, previous);
                        Result.Tables.Add(totalDetails);
                        Result.Tables.Add(drillDownDetails);
                        break;
                    case RbbgPricingType.DTD:
                        previous = RVendorUtils.GetBloombergPricing(startDate, endDate.Date);
                        totalRow = totalDetails.NewRow();
                        prevCost = 0;
                        if (CurrrentMonth != null && CurrrentMonth.Tables.Count > 0 && CurrrentMonth.Tables[0].Rows.Count > 0 && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != null && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            totalCost = Convert.ToDecimal(CurrrentMonth.Tables[0].Rows[0]["Total Cost"]);
                        if (previous != null && previous.Tables.Count > 0 && previous.Tables[0].Rows.Count > 0 && previous.Tables[0].Rows[0]["Total Cost"] != null && previous.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            prevCost = Convert.ToDecimal(previous.Tables[0].Rows[0]["Total Cost"]);
                        totalRow[0] = totalCost - prevCost;
                        totalDetails.Rows.Add(totalRow);
                        drillDownDetails = RVendorUtils.GetDrillTableData(CurrrentMonth, previous);
                        Result.Tables.Add(totalDetails);
                        Result.Tables.Add(drillDownDetails);
                        break;
                }
                mLogger.Debug("end GetBloombergPricingDetails");
                return Result;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }
        }

        public DataSet GetBloombergPricingDetails(RbbgPricingType pricingType)
        {
            mLogger.Debug("begin GetBloombergPricingDetails new");
            try
            {
                DateTime todaysDate = DateTime.Today;
                DataSet CurrrentMonth = RVendorUtils.GetBloombergPricingNew(todaysDate, false);
                DataSet previous = null;

                DataSet dsResult = new DataSet();

                DataTable totalDetails = new DataTable();
                totalDetails.Columns.Add("Cost");
                totalDetails.Columns.Add("Diff");
                dsResult.Tables.Add(totalDetails);
                DataRow totalRow = totalDetails.NewRow();
                totalDetails.Rows.Add(totalRow);

                decimal totalCost = 0;
                decimal prevCost = 0;

                switch (pricingType)
                {
                    case RbbgPricingType.MTD:
                        previous = RVendorUtils.GetBloombergPricingNew(null, true);

                        if (CurrrentMonth != null && CurrrentMonth.Tables.Count > 0 && CurrrentMonth.Tables[0].Rows.Count > 0 && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != null && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            totalCost = Convert.ToDecimal(CurrrentMonth.Tables[0].Rows[0]["Total Cost"]);
                        if (previous != null && previous.Tables.Count > 0 && previous.Tables[0].Rows.Count > 0 && previous.Tables[0].Rows[0]["Total Cost"] != null && previous.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            prevCost = Convert.ToDecimal(previous.Tables[0].Rows[0]["Total Cost"]);

                        totalRow[0] = totalCost.ToString();
                        totalRow[1] = prevCost - totalCost;
                        break;
                    case RbbgPricingType.WTD:
                        if (todaysDate.Day > 7)
                            previous = RVendorUtils.GetBloombergPricingNew(todaysDate.AddDays(-(int)todaysDate.DayOfWeek + 1).Date, false);

                        if (CurrrentMonth != null && CurrrentMonth.Tables.Count > 0 && CurrrentMonth.Tables[0].Rows.Count > 0 && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != null && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            totalCost = Convert.ToDecimal(CurrrentMonth.Tables[0].Rows[0]["Total Cost"]);
                        if (previous != null && previous.Tables.Count > 0 && previous.Tables[0].Rows.Count > 0 && previous.Tables[0].Rows[0]["Total Cost"] != null && previous.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            prevCost = Convert.ToDecimal(previous.Tables[0].Rows[0]["Total Cost"]);

                        totalRow[0] = totalCost - prevCost;
                        break;
                    case RbbgPricingType.DTD:
                        if (todaysDate.Day > 1)
                            previous = RVendorUtils.GetBloombergPricingNew(todaysDate.AddDays(-1), false);

                        if (CurrrentMonth != null && CurrrentMonth.Tables.Count > 0 && CurrrentMonth.Tables[0].Rows.Count > 0 && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != null && CurrrentMonth.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            totalCost = Convert.ToDecimal(CurrrentMonth.Tables[0].Rows[0]["Total Cost"]);
                        if (previous != null && previous.Tables.Count > 0 && previous.Tables[0].Rows.Count > 0 && previous.Tables[0].Rows[0]["Total Cost"] != null && previous.Tables[0].Rows[0]["Total Cost"] != DBNull.Value)
                            prevCost = Convert.ToDecimal(previous.Tables[0].Rows[0]["Total Cost"]);

                        totalRow[0] = totalCost - prevCost;
                        break;
                }

                DataTable drillDownDetails = RVendorUtils.GetDrillTableData(CurrrentMonth, (pricingType == RbbgPricingType.MTD ? new DataSet() : previous));
                dsResult.Tables.Add(drillDownDetails);

                mLogger.Debug("end GetBloombergPricingDetails new");
                return dsResult;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                return null;
            }
        }
        #endregion
    }
}
