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
 * 1            23-07-2009      Nitin Saxena    Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using com.ivp.rad.common;
using System.Threading;
using com.ivp.srm.vendorapi.bloomberg;
using com.ivp.rad.utils;
using System.Linq;
//using SecuritiesCollection = System.Collections.Generic.Dictionary<string,
//                                System.Collections.Generic.
//                                List<com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using com.ivp.srm.vendorapi.bloombergServices.lite;
using com.ivp.srmcommon;

namespace com.ivp.srm.vendorapi.Bloomberg
{
    /// <summary>
    /// Class for processing request.
    /// </summary>
    public class RBbgRequestProcess
    {
        #region Static Variables
        static IRLogger mLogger = RLogFactory.CreateLogger("RBbgRequestProcess");
        #endregion
        #region Member Variables
        SecuritiesCollection unprocessedSecurities = null;
        SecuritiesCollection responseSecurities = null;
        RBbgBulkListInfo bulkListInfo = null;
        delegate void delegateAddResponse(SecuritiesCollection dictResponse, SecuritiesCollection dictUnprocessed);
        delegate void delegateUpdateAdditionalInfo(List<string> FTPAdditionalInfo, List<string> FTPResponseAdditionalInfo);
        delegate void delegateGenerateError(RVendorResponse vendorResponse);
        delegateAddResponse responseDelegate = null;
        delegateUpdateAdditionalInfo additionalInfoDelegate = null;
        delegateGenerateError errorDelegate = null;
        private readonly static object timestamp = new object();//used for synchronization purpose.
        #endregion
        #region Methods
        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="requestInfo">The request info.</param>
        public void ProcessRequest(object requestInfo)
        {
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;
            RBbgSecurityInfo securityInfo = null;
            securityInfo = (RBbgSecurityInfo)requestInfo;
            SRMMTConfig.SetClientName(securityInfo.ClientName);
            mLogger.Debug("Start --> processing request");
            string requestId = string.Empty;
            string timeStamp = string.Empty;
            string status = string.Empty;
            bulkListInfo = new RBbgBulkListInfo();
            unprocessedSecurities = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            responseSecurities = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            
            responseDelegate = new delegateAddResponse(securityInfo.CurrentObject.AddVendorResponse);
            additionalInfoDelegate = new delegateUpdateAdditionalInfo(securityInfo.CurrentObject.UpdateAdditionalInfo);
            errorDelegate = new delegateGenerateError(securityInfo.CurrentObject.GenerateVendorResponseForError);
            var lstRequestIds = new List<string>();
            int vendorPricingId = 0;
            var dictHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                if (securityInfo.IsBulkList)
                {
                    bulkListInfo = RVendorUtils.GetBulkListInfo(securityInfo.BulkListMapId[0]);
                    if (securityInfo.IsCombinedFtpReq)
                        securityInfo.InstrumentFields.AddRange(RBbgUtils.GetRequestFieldForBulkList(bulkListInfo));
                    else
                        securityInfo.InstrumentFields = RBbgUtils.GetRequestFieldForBulkList(bulkListInfo);
                }
                #region Insert into Vendor History
                if (!securityInfo.IsBVALPricingSource)
                {
                    lock (timestamp)//Create this section a critical one for synchronization purpose.
                    {
                        //timeStamp = string.Format(string.Format("{0:MMddyyHHmmssfff}", RBbgUtils.GetTargetDateTime()));
                        //timeStamp = "031918043357095";//
                        timeStamp = RBbgUtils.GetTargetDateTime();
                        RBbgUtils.WriteVendorHistory(securityInfo, "Bloomberg", securityInfo.RequestIdentifier, timeStamp, out vendorPricingId);
                    }
                }
                #endregion

                #region "Process Request"
                switch (securityInfo.RequestType)
                {
                    case RBbgRequestType.Lite:
                        securityInfo.ImmediateRequest = true;
                        if (securityInfo.MacroInfo == null)
                        {
                            RBbgLiteRequest liteRequest = new RBbgLiteRequest();
                            responseSecurities = liteRequest.GetSecurities(securityInfo);
                            requestId = "Bloomberg Lite API Request";
                            break;
                        }
                        else
                            throw new RVendorException("Macros not supported by Bloomberg Lite API.");
                    case RBbgRequestType.Heavy:
                        securityInfo.ImmediateRequest = true;
                        RBbgHeavyRequest heavyRequest = new RBbgHeavyRequest();
                        responseSecurities = heavyRequest.GetSecurities(timeStamp, securityInfo, ref dictHeaders);
                        requestId = "Bloomberg Heavy API Request";
                        break;
                    case RBbgRequestType.FTP:
                        RBbgFTPRequest ftpRequest = new RBbgFTPRequest();
                        string AdditionalInfo = string.Empty;
                        string ResponseAdditionalInfo = string.Empty;
                        if (securityInfo.ImmediateRequest)
                        {
                            responseSecurities = (SecuritiesCollection)ftpRequest.GetSecurities(securityInfo, securityInfo.ImmediateRequest, timeStamp, ref dictHeaders, out AdditionalInfo, out ResponseAdditionalInfo, ref unprocessedSecurities);

                            securityInfo.AdditionalInfo.Add(AdditionalInfo);
                            securityInfo.ResponseAdditionalInfo.Add(ResponseAdditionalInfo);
                            requestId = "Bloomberg FTP Immediate Request";
                        }
                        else
                        {
                            if (securityInfo.IsBVALPricingSource)
                            {
                                var globalInstruments = securityInfo.Instruments;
                                var bvalPricingGroups = securityInfo.Instruments.GroupBy(x => ((string.IsNullOrEmpty(x.BVALPriceSourceValue) || x.BVALPriceSourceValue.Equals("Bloomberg", StringComparison.OrdinalIgnoreCase)) ? string.Empty : x.BVALPriceSourceValue), StringComparer.OrdinalIgnoreCase);
                                for (int i = 0; i < bvalPricingGroups.Count(); i++)
                                {
                                    lock (timestamp)
                                    {
                                        timeStamp = RBbgUtils.GetTargetDateTime();
                                        lstRequestIds.Add(timeStamp);
                                    }
                                }
                                lock (lstRequestIds)
                                {
                                    RBbgUtils.WriteVendorHistory(securityInfo, "Bloomberg", securityInfo.RequestIdentifier, string.Join("|", lstRequestIds), out vendorPricingId);
                                }
                                int groupCounter = 0;
                                foreach (var bvalPricingSourceGroup in bvalPricingGroups)
                                {
                                    string bvalPricingSourceValue = bvalPricingSourceGroup.Key;
                                    if (string.IsNullOrEmpty(bvalPricingSourceValue) || bvalPricingSourceValue.Equals("Bloomberg", StringComparison.OrdinalIgnoreCase))
                                        securityInfo.IsBVALPricingSource = false;
                                    var bvalInstruments = bvalPricingSourceGroup.Select(x => x).ToList();
                                    securityInfo.Instruments = bvalInstruments;

                                    ftpRequest.GetSecurities(securityInfo, securityInfo.ImmediateRequest, lstRequestIds[groupCounter], ref dictHeaders, out AdditionalInfo, out ResponseAdditionalInfo, ref unprocessedSecurities);

                                    securityInfo.AdditionalInfo.Add(AdditionalInfo);
                                    securityInfo.ResponseAdditionalInfo.Add(ResponseAdditionalInfo);
                                    securityInfo.IsBVALPricingSource = true;
                                    groupCounter++;
                                }
                                securityInfo.Instruments = globalInstruments;
                            }
                            else
                            {
                                requestId = (string)ftpRequest.GetSecurities(securityInfo, securityInfo.ImmediateRequest, timeStamp, ref dictHeaders, out AdditionalInfo, out ResponseAdditionalInfo, ref unprocessedSecurities);

                                securityInfo.AdditionalInfo.Add(AdditionalInfo);
                                securityInfo.ResponseAdditionalInfo.Add(ResponseAdditionalInfo);
                            }
                        }
                        break;
                    case RBbgRequestType.SAPI:
                        securityInfo.ImmediateRequest = true;
                        RBbgServerRequest serverRequest = new RBbgServerRequest();
                        responseSecurities = serverRequest.GetSecurities(securityInfo);
                        requestId = "Bloomberg SAPI Request";
                        break;
                    case RBbgRequestType.MANAGEDBPIPE:
                        securityInfo.ImmediateRequest = true;
                        RBbgManagedBPipe managedBPipeRequest = new RBbgManagedBPipe();
                        responseSecurities = managedBPipeRequest.GetSecurities(securityInfo);
                        requestId = "Bloomberg SAPI Request";
                        break;
                    case RBbgRequestType.GlobalAPI:
                        securityInfo.ImmediateRequest = true;
                        RBbgGlobalAPI globalAPIRequest = new RBbgGlobalAPI();
                        responseSecurities = globalAPIRequest.GetSecurities(securityInfo);
                        requestId = "Bloomberg SAPI Request";
                        break;
                }
                #endregion

                #region Handle NS,ND,NA,FLD Unknown
                if (responseSecurities != null)
                {
                    mLogger.Debug("Start --> RemoveInvalidFields");
                    if (securityInfo.RequestType != RBbgRequestType.FTP)
                        responseSecurities = RVendorUtils.RemoveInvalidFields(responseSecurities, ref unprocessedSecurities, securityInfo.requireNotAvailableInField);
                    mLogger.Debug("End --> RemoveInvalidFields");
                    if (responseSecurities.Count > 0 || unprocessedSecurities.Count > 0)
                    {
                        mLogger.Debug("Start --> invoking delegates");
                        responseDelegate.Invoke(responseSecurities, unprocessedSecurities);
                        mLogger.Debug("End --> invoking delegates");
                    }
                }
                #endregion

                #region Update vendor history
                if (securityInfo.ImmediateRequest)
                {
                    StringBuilder responseStatus = new StringBuilder();
                    string instruments = string.Empty; string fields = string.Empty;
                    if (securityInfo.IsBulkList)
                    {
                        ((List<RBbgInstrumentInfo>)securityInfo.Instruments).ForEach(q => instruments = instruments + "," + q.InstrumentID);
                        //  instruments = ((List<RBbgInstrumentInfo>)securityInfo.Instruments)[0].InstrumentID;
                        fields = bulkListInfo.OutputFields;
                        if (securityInfo.IsCombinedFtpReq)
                            securityInfo.InstrumentFields.ForEach(q => fields = fields + "," + q);
                        responseStatus.Append(securityInfo.Instruments[0].InstrumentIdType.ToString() + "=> " + fields);
                    }
                    else
                    {
                        date1 = DateTime.Now;
                        mLogger.Debug("Start --> getting instruments");
                        instruments = RBbgUtils.GetCSVInstruments(responseSecurities);
                        mLogger.Debug("Start --> getting  fields");
                        int fieldsCount;
                        fields = RBbgUtils.GetCSVFields(responseSecurities, out fieldsCount);
                        if (securityInfo.Instruments.Count > 0)
                        {
                            mLogger.Debug("Start --> preparing status");
                            //int fieldsCount = fields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Count();
                            var instrumentType = securityInfo.Instruments.Select(q => q.InstrumentIdType).Distinct();
                            foreach (var type in instrumentType)
                            {
                                responseStatus.Append(type);
                                responseStatus.Append("=> ");
                                responseStatus.Append(fieldsCount);
                                responseStatus.Append(",");
                            }
                            mLogger.Debug("End --> preparing status");
                        }

                        date2 = DateTime.Now;
                        RVendorUtils.InsertLogTable("GetCSVInstruments", date1, date2);
                    }
                    string responseXml = string.Empty;
                    if (instruments.Equals(string.Empty) && fields.Equals(string.Empty))
                        status = RVendorStatusConstant.FAILED;
                    else
                    {
                        status = RVendorStatusConstant.PROCESSED;
                    }
                    if (securityInfo.IsBulkList)
                    {
                        RVendorUtils.UpdateVendorHistory(instruments, fields, timeStamp, status, responseStatus);

                        if (RVendorUtils.EnableBBGAudit)
                        {
                            try
                            {
                                var dictBulkListResult = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                                var lstFieldInfo = new Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase) { { securityInfo.InstrumentFields[0].Mnemonic, new com.ivp.srm.vendorapi.RVendorFieldInfo { Name = securityInfo.InstrumentFields[0].Mnemonic } } };
                                foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyValue in responseSecurities)
                                {
                                    if (securityInfo.RequestType == RBbgRequestType.FTP)
                                    {
                                        List<string> matchingSec = new List<string>();
                                        foreach (RBbgInstrumentInfo info in securityInfo.Instruments)
                                        {
                                            if (keyValue.Key.StartsWith(info.InstrumentID))
                                            {
                                                matchingSec.Add(info.InstrumentID);
                                            }
                                        }
                                        string identifierValue = GetMaxValue(matchingSec);
                                        if (!dictBulkListResult.ContainsKey(identifierValue))
                                            dictBulkListResult.Add(identifierValue, lstFieldInfo);
                                    }
                                    else if (securityInfo.RequestType == RBbgRequestType.SAPI || securityInfo.RequestType == RBbgRequestType.Heavy)
                                    {
                                        List<string> matchingSec = new List<string>();
                                        foreach (RBbgInstrumentInfo info in securityInfo.Instruments)
                                        {
                                            if (keyValue.Key.Contains(info.InstrumentID))
                                            {
                                                matchingSec.Add(info.InstrumentID);
                                            }
                                        }
                                        string identifierValue = GetMaxValue(matchingSec);
                                        if (!dictBulkListResult.ContainsKey(identifierValue))
                                            dictBulkListResult.Add(identifierValue, lstFieldInfo);
                                    }
                                }
                                RBbgUtils.UpdateBBGAudit(timeStamp, dictBulkListResult);
                            }
                            catch (Exception ex)
                            {
                                mLogger.Error("Bulk List immediate : " + ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        //if (securityInfo.RequestType == RBbgRequestType.FTP)
                        //{
                        date1 = DateTime.Now;
                        responseXml = RBbgUtils.PrepareXML(responseSecurities);
                        date2 = DateTime.Now;
                        RVendorUtils.InsertLogTable("PrepareXML", date1, date2);

                        //}

                        date1 = DateTime.Now;
                        RVendorUtils.UpdateVendorHistory(instruments, fields, timeStamp, status, responseStatus, responseXml, vendorPricingId);
                        date2 = DateTime.Now;
                        RVendorUtils.InsertLogTable("UpdateVendorHistory", date1, date2);

                        RBbgUtils.UpdateBBGAudit(timeStamp, responseSecurities);
                    }
                }
                else
                {
                    RVendorUtils.UpdateVendorHistory(RBbgUtils.GetCSVInstruments(responseSecurities),
                        RBbgUtils.GetCSVFields(responseSecurities), (securityInfo.IsBVALPricingSource ? string.Join("|", lstRequestIds) : timeStamp),
                                RVendorStatusConstant.REQUEST_REGISTERED);
                    RBbgUtils.UpdateBBGAudit(timeStamp, responseSecurities);
                }
                #endregion
            }
            catch (Exception ex)
            {
                RVendorResponse vendorResponse = null;
                try
                {
                    if (securityInfo.RequestType == RBbgRequestType.Lite)
                        DeleteAllViews(RBbgUtils.GetLiteService(securityInfo));
                    mLogger.Error(ex.ToString());
                    vendorResponse = RVendorUtils.GetDatabaseException(securityInfo.ImmediateRequest, ex.Message);
                    RVendorUtils.UpdateVendorHistoryException(timeStamp, vendorResponse.ExceptionMessage);
                }
                catch
                { }
                finally
                {
                    errorDelegate.Invoke(vendorResponse);
                }
            }
            finally
            {
                mLogger.Debug("Start --> invoking additionalInfoDelegate");
                additionalInfoDelegate.Invoke(securityInfo.AdditionalInfo, securityInfo.ResponseAdditionalInfo);
                mLogger.Debug("End --> invoking additionalInfoDelegate");
                mLogger.Debug("End --> processing request");
                //Signalling Main thread,that processing is finished.
                if (securityInfo.ManualResetEvent != null && securityInfo.ManualResetEvent.SafeWaitHandle.IsClosed == false)
                    securityInfo.ManualResetEvent.Set();
            }
        }

        private static string GetMaxValue(List<string> matchingSec)
        {
            string maxLength = string.Empty;
            foreach (string sec in matchingSec)
            {
                if (sec.Length > maxLength.Length)
                    maxLength = sec;
            }
            return maxLength;
        }

        /// <summary>
        /// Deletes all views.
        /// </summary>
        /// <param name="service">The service.</param>
        private void DeleteAllViews(GetDataService service)
        {
            GetViewNamesRequest viewRequest = new GetViewNamesRequest();
            GetViewNamesResponse viewResponse = new GetViewNamesResponse();
            viewResponse = service.getViewNames(viewRequest);
            string[] viewNames = viewResponse.viewName;
            if (viewNames != null)
            {
                for (int i = 0; i < viewNames.Length; i++)
                {
                    DeleteViewDefinitionRequest viewDeleteRequest = new DeleteViewDefinitionRequest();
                    viewDeleteRequest.viewName = viewResponse.viewName[i];
                    service.deleteViewDefinition(viewDeleteRequest);
                }
            }
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Processes the FTP response,if request is asynchronous.
        /// </summary>
        /// <param name="historyInfo">The history info.</param>
        public void ProcessFTPResponse(object historyInfo)
        {
            mLogger.Debug("Start --> processing reponse for FTP");
            List<RBbgInstrumentInfo> lstInstrument = null;
            List<RBbgFieldInfo> lstFields = null;
            RBbgSecurityInfo securityInfo = null;
            RBbgFTPRequest bloomberg = null;
            RBbgBulkListInfo bulkListInfo = null;
            SecuritiesCollection securityResponse = null;
            RVendorHistoryInfo vendorHistory = null;
            string status = string.Empty;
            lstInstrument = new List<RBbgInstrumentInfo>();
            lstFields = new List<RBbgFieldInfo>();
            securityInfo = new RBbgSecurityInfo();
            bloomberg = new RBbgFTPRequest();
            bulkListInfo = new RBbgBulkListInfo();
            securityResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            unprocessedSecurities = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            try
            {
                vendorHistory = (RVendorHistoryInfo)historyInfo;
                SRMMTConfig.SetClientName(vendorHistory.ClientName);

                securityInfo.IsBulkList = vendorHistory.IsBulkList;
                securityInfo.BulkListMapId = new List<int>();
                securityInfo.RequestType = RBbgRequestType.FTP;
                securityInfo.TransportName = vendorHistory.TransportName;
                securityInfo.IsMarketSectorAppended = vendorHistory.IsMarketSectorAppended;
                securityInfo.CurrentObject = (RBloomberg)vendorHistory.CurrentObject;
                responseDelegate = new delegateAddResponse(securityInfo.CurrentObject.AddVendorResponse);
                errorDelegate = new delegateGenerateError(securityInfo.CurrentObject.GenerateVendorResponseForError);
                #region Instruments
                string[] instrumentsInfo = vendorHistory.VendorInstruments.Split(',');
                for (int i = 0; i < instrumentsInfo.Length; i++)
                {
                    RBbgInstrumentInfo info = new RBbgInstrumentInfo();
                    info.InstrumentID = instrumentsInfo[i];
                    if (!lstInstrument.Contains(info))
                        lstInstrument.Add(info);
                }
                #endregion
                #region Fields
                string[] fieldsInfo = vendorHistory.VendorFields.Split(',');
                for (int i = 0; i < fieldsInfo.Length; i++)
                {
                    RBbgFieldInfo info = new RBbgFieldInfo();
                    info.Mnemonic = fieldsInfo[i];
                    if (!lstFields.Contains(info))
                        lstFields.Add(info);
                    if (securityInfo.IsBulkList)
                    {
                        bulkListInfo = RVendorUtils.GetBulkListInfo(int.Parse(fieldsInfo[1]));
                        securityInfo.BulkListMapId.Add(bulkListInfo.BulkListMapId);
                        break;
                    }
                }
                #endregion
                securityInfo.Instruments = lstInstrument;
                securityInfo.InstrumentFields = lstFields;
                securityInfo.VendorPreferenceId = vendorHistory.VendorPreferenceId;
                var requestIds = vendorHistory.TimeStamp.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var requestId in requestIds)
                {
                    if (securityResponse == null || securityResponse.Count == 0)
                        securityResponse = bloomberg.GetResponse(requestId, securityInfo.TransportName, securityInfo.IsBulkList, securityInfo, ref unprocessedSecurities);
                    else
                    {
                        var tempSecurityResponse = bloomberg.GetResponse(requestId, securityInfo.TransportName, securityInfo.IsBulkList, securityInfo, ref unprocessedSecurities);
                        foreach (var secResKey in tempSecurityResponse.Keys)
                        {
                            var secResValue = tempSecurityResponse[secResKey];
                            securityResponse.Add(secResKey, secResValue);
                        }
                    }
                }

                if (securityResponse != null)
                {
                    //securityResponse = RVendorUtils.RemoveInvalidFields(securityResponse, ref unprocessedSecurities);
                    if (securityResponse.Count > 0 || unprocessedSecurities.Count > 0)
                        responseDelegate.Invoke(securityResponse, unprocessedSecurities);
                    string instruments = string.Empty;
                    string fields = string.Empty;
                    if (securityInfo.IsBulkList)
                    {
                        instruments = ((List<RBbgInstrumentInfo>)securityInfo.Instruments)[0].InstrumentID;
                        fields = bulkListInfo.OutputFields;
                    }
                    else
                    {
                        instruments = RBbgUtils.GetCSVInstruments(securityResponse);
                        fields = RBbgUtils.GetCSVFields(securityResponse);
                    }

                    if (instruments.Equals(string.Empty) && fields.Equals(string.Empty))
                        status = RVendorStatusConstant.FAILED;
                    else
                        status = RVendorStatusConstant.PROCESSED;
                    if (securityInfo.IsBulkList)
                    {
                        RVendorUtils.UpdateVendorHistory(instruments, fields, vendorHistory.TimeStamp, status);

                        if (securityInfo.IsBulkList)
                        {
                            try
                            {
                                var dictBulkListResult = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                                var lstFieldInfo = new Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo> { { securityInfo.InstrumentFields[0].Mnemonic, new com.ivp.srm.vendorapi.RVendorFieldInfo { Name = securityInfo.InstrumentFields[0].Mnemonic } } };
                                foreach (KeyValuePair<string, Dictionary<string, RVendorFieldInfo>> keyValue in securityResponse)
                                {
                                    if (securityInfo.RequestType == RBbgRequestType.FTP)
                                    {
                                        List<string> matchingSec = new List<string>();
                                        foreach (RBbgInstrumentInfo info in securityInfo.Instruments)
                                        {
                                            if (keyValue.Key.StartsWith(info.InstrumentID))
                                            {
                                                matchingSec.Add(info.InstrumentID);
                                            }
                                        }
                                        string identifierValue = GetMaxValue(matchingSec);
                                        if (!dictBulkListResult.ContainsKey(identifierValue))
                                            dictBulkListResult.Add(identifierValue, lstFieldInfo);
                                    }
                                    else if (securityInfo.RequestType == RBbgRequestType.SAPI || securityInfo.RequestType == RBbgRequestType.Heavy)
                                    {
                                        List<string> matchingSec = new List<string>();
                                        foreach (RBbgInstrumentInfo info in securityInfo.Instruments)
                                        {
                                            if (keyValue.Key.Contains(info.InstrumentID))
                                            {
                                                matchingSec.Add(info.InstrumentID);
                                            }
                                        }
                                        string identifierValue = GetMaxValue(matchingSec);
                                        if (!dictBulkListResult.ContainsKey(identifierValue))
                                            dictBulkListResult.Add(identifierValue, lstFieldInfo);
                                    }
                                }
                                RBbgUtils.UpdateBBGAudit(vendorHistory.TimeStamp, dictBulkListResult);
                            }
                            catch (Exception ex)
                            {
                                mLogger.Error("Bulk List response : " + ex.ToString());
                            }
                        }
                    }
                    else
                    {
                        string responseXml = RBbgUtils.PrepareXML(securityResponse);//, vendorHistory.TimeStamp
                        RVendorUtils.UpdateVendorHistory(instruments, fields, vendorHistory.TimeStamp, status, new StringBuilder(), responseXml, vendorHistory.VendorPricingId);
                        RBbgUtils.UpdateBBGAudit(vendorHistory.TimeStamp, securityResponse);
                    }
                    RVendorUtils.UpdateVendorHistory(instruments, fields, vendorHistory.TimeStamp, status);
                }
            }
            catch (Exception ex)
            {
                RVendorResponse vendorResponse = null;
                try
                {
                    mLogger.Error("VendorAPIError: Error Writting into Database");
                    mLogger.Error(ex.ToString());
                    vendorResponse = RVendorUtils.GetDatabaseException(true, ex.Message);
                    RVendorUtils.UpdateVendorHistoryException(vendorHistory.TimeStamp, vendorResponse.ExceptionMessage);
                }
                catch
                {
                }
                finally
                {
                    errorDelegate.Invoke(vendorResponse);
                }
            }
            finally
            {
                mLogger.Debug("End --> processing reponse for FTP");
                //Signalling Main thread,that processing is finished.
                if (vendorHistory.ManualReset != null && vendorHistory.ManualReset.SafeWaitHandle.IsClosed == false)
                {
                    mLogger.Error("End --> setting reset event");
                    vendorHistory.ManualReset.Set();
                }
            }
        }
        #endregion
    }
}
