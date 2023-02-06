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
using com.ivp.rad.common;
using System.Text;
using com.ivp.srm.vendorapi.bloomberg;
//using SecuritiesCollection = System.Collections.Generic.
//                             Dictionary<string, System.Collections.
//                             Generic.List<com.ivp.srm.vendorapi.
//                             RVendorFieldInfo>>;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using System.Data;

namespace com.ivp.srm.vendorapi.reuters
{
    /// <summary>
    /// IVendor implementation for reuters api
    /// </summary>
    public class RReuter : IVendor
    {
        #region variables
        static IRLogger mLogger = RLogFactory.CreateLogger("RReuter");
        SecuritiesCollection dictUnprocessedInstruments = null;
        List<SecuritiesCollection> listSecurityResponse = null;
        #endregion
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the securities.
        /// </summary>
        /// <param name="requestedSecurities"></param>
        /// <param name="returnType">Type of the return.</param>
        /// <returns></returns>
        RVendorResponse IVendor.GetSecurities(object requestedSecurities, RSecurityReturnType returnType,
                                                                                    bool immediateRequest)
        {
            mLogger.Debug("Start -> Get Securities using Reuters");
            SecuritiesCollection securitiesResponse = null;
            string requestId = string.Empty;
            listSecurityResponse = new List<SecuritiesCollection>();
            dictUnprocessedInstruments = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            RVendorResponse vendorResponse = null;
            //string timeStamp = string.Format(string.Format("{0:MMddyyHHmmss}",
            //                                       RBbgUtils.GetTargetDateTime()));
            string timeStamp = RBbgUtils.GetTargetDateTime();
            string gd = Guid.NewGuid().ToString();
            try
            {
                vendorResponse = new RVendorResponse();
                RReuterSecurityInfo securityInfo = (RReuterSecurityInfo)requestedSecurities;
                securityInfo = RReuterUtils.ValidateRequest(securityInfo);
                if (securityInfo.Instruments.Count == 0)
                    throw new RVendorException("no instruments to be processed");

                string transportName = RVendorConfigLoader.GetVendorConfig(RVendorType.Reuters, securityInfo.VendorPreferenceId, RVendorConstant.TRANSPORTNAME);

                if (securityInfo.RequestType == RReuterRequestType.FTP && (string.IsNullOrEmpty(securityInfo.TransportName) || (!string.IsNullOrEmpty(transportName) && immediateRequest)))
                {
                    securityInfo.TransportName = transportName;
                }
                #region Insert VendorHistory
                //Insert into vendor history
                RReuterUtils.WriteVendorHistory(securityInfo, "Reuters", gd, timeStamp);
                #endregion
                #region Process Request
                switch (securityInfo.RequestType)
                {
                    case RReuterRequestType.FTP:
                        RReuterFTPRequest ftpRequest = new RReuterFTPRequest();
                        if (immediateRequest)
                        {
                            securitiesResponse = (SecuritiesCollection)ftpRequest.GetSecurities(securityInfo, immediateRequest, timeStamp);
                            requestId = "Reuters FTP Immediate Request";
                        }
                        else
                            requestId = (string)ftpRequest.GetSecurities(securityInfo, immediateRequest, timeStamp);
                        break;
                    case RReuterRequestType.API:
                    default:
                        immediateRequest = true;
                        RReuterAPIRequest apiRequest = new RReuterAPIRequest();
                        requestId = "Reuters API Request";
                        securitiesResponse = apiRequest.GetSecurities(securityInfo);
                        break;
                }
                #endregion
                #region Normalize Securities
                if (securitiesResponse != null)
                {
                    securitiesResponse = RVendorUtils.RemoveInvalidFields(securitiesResponse, ref dictUnprocessedInstruments, true);
                    if (securitiesResponse.Count > 0)
                        listSecurityResponse.Add(securitiesResponse);
                }
                #endregion
                #region Process Response
                if (immediateRequest)
                {
                    RVendorUtils.UpdateVendorHistory(RReuterUtils.GetCSVInstruments(securitiesResponse),
                                 RBbgUtils.GetCSVFields(securitiesResponse), timeStamp,
                                 RVendorStatusConstant.PROCESSED);
                    securitiesResponse = RReuterUtils.NormalizeSecurities(listSecurityResponse, (RReuterSecurityInfo)requestedSecurities, ref dictUnprocessedInstruments);
                    vendorResponse = RReuterUtils.ProcessResponse(returnType, securitiesResponse, dictUnprocessedInstruments, gd);
                }
                else
                {
                    RVendorUtils.UpdateVendorHistory(RReuterUtils.GetCSVInstruments(securitiesResponse), RBbgUtils.GetCSVFields(securitiesResponse), timeStamp, RVendorStatusConstant.REQUEST_REGISTERED);
                    vendorResponse = RReuterUtils.ProcessResponse(returnType, gd, dictUnprocessedInstruments, gd);
                }
                #endregion
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                vendorResponse = RVendorUtils.GetDatabaseException(immediateRequest, rEx.Message);
                vendorResponse.RequestIdentifier = gd;
                RVendorUtils.UpdateVendorHistoryException(timeStamp, vendorResponse.ExceptionMessage);
                if (vendorResponse != null) return vendorResponse;
            }
            catch (Exception rEx)
            {
                mLogger.Error(rEx.ToString());
                vendorResponse = RVendorUtils.GetDatabaseException(immediateRequest, rEx.Message);
                RVendorUtils.UpdateVendorHistoryException(timeStamp, vendorResponse.ExceptionMessage);
                if (vendorResponse != null) return vendorResponse;
            }
            finally
            {
                mLogger.Debug("End -> creating request object.");
            }
            vendorResponse.RequestIdentifier = gd;
            return vendorResponse;
        }
        //------------------------------------------------------------------------------------------
        #region IVendor Members
        public RVendorResponse GetResponse(string requestIdentifier, string transportName, RSecurityReturnType returnType, int VendorPreferenceId)
        {
            List<SecuritiesCollection> listSecuritiesResponse = null;
            SecuritiesCollection securitiesResponse = null;
            SecuritiesCollection dictUnprocessedInstruments = null;
            RReuterSecurityInfo securityInfo = null;
            List<RReuterInstrumentInfo> lstInstrument = null;
            List<string> lstFields = null;
            List<RVendorHistoryInfo> historyList = null;

            listSecuritiesResponse = new List<SecuritiesCollection>();
            dictUnprocessedInstruments = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            securityInfo = new RReuterSecurityInfo();
            lstInstrument = new List<RReuterInstrumentInfo>();
            lstFields = new List<string>();

            //Get Vendor History from Dbase
            historyList = RVendorUtils.GetVendorHistory(requestIdentifier);
            if (historyList.Count > 0)
            {
                string transportNameVendorPreference = RVendorConfigLoader.GetVendorConfig(RVendorType.Reuters, VendorPreferenceId, RVendorConstant.TRANSPORTNAME);

                if (string.IsNullOrEmpty(transportName) && !string.IsNullOrEmpty(transportNameVendorPreference))
                {
                    transportName = transportNameVendorPreference;
                }
                foreach (RVendorHistoryInfo vendorHistory in historyList)
                {
                    RReuterFTPRequest reuter = new RReuterFTPRequest();
                    try
                    {
                        securityInfo.Instruments = RReuterUtils.GetInstruments
                                                        (vendorHistory.VendorInstruments);
                        securityInfo.InstrumentFields = RReuterUtils.GetFields
                                                        (vendorHistory.VendorFields);
                        securitiesResponse = reuter.GetResponse(vendorHistory.TimeStamp, transportName, securityInfo);
                        if (securitiesResponse != null)
                        {
                            securitiesResponse = RVendorUtils.RemoveInvalidFields(securitiesResponse, ref dictUnprocessedInstruments, true);
                            listSecuritiesResponse.Add(securitiesResponse);
                            RVendorUtils.UpdateVendorHistory(RReuterUtils.GetCSVInstruments(securitiesResponse),
                                                                RReuterUtils.GetCSVFields(securitiesResponse),
                                                                vendorHistory.TimeStamp, RVendorStatusConstant.PROCESSED);
                        }
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("VendorAPIError: Error Updating Database");
                        mLogger.Error(ex.ToString());
                        RVendorResponse vendorResponse = GetDatabaseException(true, ex.Message);
                        RVendorUtils.UpdateVendorHistoryException(vendorHistory.TimeStamp, vendorResponse.ExceptionMessage);
                        if (vendorResponse != null) return vendorResponse;
                    }
                }
                securitiesResponse = RReuterUtils.NormalizeSecurities(listSecuritiesResponse, securityInfo, ref dictUnprocessedInstruments);
                return RReuterUtils.ProcessResponse(returnType, securitiesResponse, dictUnprocessedInstruments, requestIdentifier);
            }
            else
                throw new RVendorException("Invalid Request Identifier");
        }

        public RVendorResponse GetResponse(string requestIdentifier, string transportName, RSecurityReturnType returnType)
        {
            return GetResponse(requestIdentifier, transportName, returnType, 1);
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the database exception.
        /// </summary>
        private RVendorResponse GetDatabaseException(bool immediateRequest, string message)
        {
            if (immediateRequest == false)
            {
                RVendorResponse vendorResponse = new RVendorResponse();
                vendorResponse.ExceptionMessage = message;
                vendorResponse.ResponseStatus = RVendorResponseStatus.Failed;
                vendorResponse.SecurityResponse = null;
                return vendorResponse;
            }
            return null;
        }

        RVendorResponse IVendor.GetSecurities(object requestedSecurities)
        {
            throw new NotImplementedException();
        }

        public RVendorResponse GetResponse(string requestId, int VendorPreferenceId)
        {
            throw new NotImplementedException();
        }

        public RVendorResponse GetResponse(string requestId)
        {
            return GetResponse(requestId, 1);
        }

        #endregion
    }
}
