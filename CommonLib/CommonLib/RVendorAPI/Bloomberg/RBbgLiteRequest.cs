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
 * 1            24-10-2008      Manoj          Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.srm.vendorapi.bloombergServices;
using com.ivp.srm.vendorapi.bloombergServices.lite;
using com.ivp.rad.common;
//using SecuritiesCollection = System.Collections.Generic.Dictionary<string,
//                                System.Collections.Generic.
//                                List<com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using InstrumentCollection = System.Collections.Generic.Dictionary<string,
                                com.ivp.srm.vendorapi.bloomberg.RBbgInstrumentInfo>;

namespace com.ivp.srm.vendorapi.bloomberg
{
    /// <summary>
    /// Bloomberg Execution engine for Lite Request
    /// </summary>
    internal class RBbgLiteRequest
    {
        #region Member Variables
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        string ViewName = string.Format("{0:MMddyyHHmmssfff}", System.DateTime.Now);
        GetDataService service = null;
        #endregion
        //------------------------------------------------------------------------------------------
        #region Private Methods
        /// <summary>
        /// Creates the view for request.
        /// </summary>
        /// <param name="service">The Bloomberg data service.</param>
        /// <param name="bbgFields">The BBG fields.</param>
        private void CreateView(GetDataService service, List<RBbgFieldInfo> bbgRequestedFields)
        {
            mLogger.Debug("Start-> creating custom view");
            //DeleteAllViews(service);//Delete all view if limit of views exceeds.
            try
            {
                List<RBbgFieldInfo> bbgFields = null;
                try
                {
                    bbgFields = new List<RBbgFieldInfo>();
                    bbgFields.AddRange(bbgRequestedFields);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                CreateOrUpdateViewDefinitionRequest createRequest = new CreateOrUpdateViewDefinitionRequest();
                ViewOptions viewOptions = new ViewOptions();
                viewOptions.dateFormat = DateFormat.yyyymmdd;
                createRequest.viewOptions = viewOptions;
                createRequest.viewName = ViewName;
                Field[] fields = GetFields(bbgFields);
                createRequest.fields = fields;
                CreateOrUpdateViewDefinitionResponse createResponse = null;
                try
                {
                    createResponse = service.createViewDefinition(createRequest);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                bool skip = false;
                for (int k = 0; k < createResponse.fields.Length; k++)
                {
                    if (createResponse.fields[k].statusCodes.internalCode == 102 || createResponse.fields[k].statusCodes.internalCode==103)
                    {
                        foreach (RBbgFieldInfo info in bbgFields)
                        {
                            if (info.Mnemonic.Equals(createResponse.fields[k].mnemonic))
                            {
                                bbgFields.Remove(info);
                                skip = true;
                                break;
                            }
                        }
                    }
                }
                if (skip)
                {
                    fields = GetFields(bbgFields);
                    createRequest.fields = fields;
                    createResponse = service.createViewDefinition(createRequest);
                }
                //loop through response
                //if found invalid field
                //remove from bbgFields and call this method again
                if (createResponse.statusCodes.externalCode != 0 || createResponse.statusCodes.internalCode != 0)
                {
                    RBbgUtils.HandleErrorCodes(createResponse.statusCodes.externalCode.ToString());
                    //throw new RVendorException(RBbgUtils.CreateExceptionMessage(createResponse.statusCodes.
                    //internalCode.ToString(), createResponse.statusCodes.externalCode.ToString(), 
                    //createResponse.statusCodes.description));
                }
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
                mLogger.Debug("End-> creating custom view");
            }
        }

        
        //------------------------------------------------------------------------------------------
        private static Field[] GetFields(List<RBbgFieldInfo> bbgFields)
        {
            Field[] fields = null;
            try
            {
                fields = new Field[bbgFields.Count];
                int i = 0;
                foreach (RBbgFieldInfo field in bbgFields)
                {
                    fields[i] = new Field();
                    fields[i].mnemonic = field.Mnemonic;
                    if (field.FieldOptions != null)
                    {
                        fields[i].fieldOptions.precision = field.FieldOptions.PrecisionField;
                        fields[i].fieldOptions.precisionSpecified = field.FieldOptions.PrecisionFieldSpecified;
                        fields[i].fieldOptions.realValue = field.FieldOptions.RealValueField;
                        fields[i].fieldOptions.realValueSpecified = field.FieldOptions.RealValueFieldSpecified;
                    }
                    fields[i].overrideSpecified = true;
                    fields[i].@override = false;
                    i++;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            return fields;
        }
        /// <summary>
        /// Deletes the view.
        /// </summary>
        /// <param name="service">The Bloomberg data service.</param>
        private void DeleteView(GetDataService service)
        {
            mLogger.Debug("Start-> deleting custom view");
            DeleteViewDefinitionRequest deleteRequest = new DeleteViewDefinitionRequest();
            //deleteRequest.viewName = "IVP View";
            deleteRequest.viewName = ViewName;
            Response deleteResponse = service.deleteViewDefinition(deleteRequest);
            if (deleteResponse.statusCodes.externalCode == 117 || deleteResponse.statusCodes.internalCode == 117)
                return;
            if (deleteResponse.statusCodes.externalCode != 0 || deleteResponse.statusCodes.internalCode != 0)
            {
                throw new RVendorException(RBbgUtils.CreateExceptionMessage(deleteResponse.statusCodes.internalCode.ToString(),
                    deleteResponse.statusCodes.externalCode.ToString(), deleteResponse.statusCodes.description));
            }
            mLogger.Debug("End-> deleting custom view");
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region Public Methods
        /// <summary>
        /// Gets the securities.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        public SecuritiesCollection GetSecurities(RBbgSecurityInfo securityInfo)
        {
            mLogger.Debug("Start -> retrieving retrieving secuities info");
            //GetDataService service=null;
            try
            {
                service = RBbgUtils.GetLiteService(securityInfo);
                CreateView(service, securityInfo.InstrumentFields);
                Instrument[] instruments = new Instrument[securityInfo.Instruments.Count];
                int i = 0;
                InstrumentCollection dictInstruments = new InstrumentCollection();
                SecuritiesCollection returnReponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                foreach (RBbgInstrumentInfo inst in securityInfo.Instruments)
                {
                    instruments[i] = new Instrument();
                    instruments[i].id = inst.InstrumentID;

                    instruments[i].idType = (InstrumentIdType)Enum.Parse(typeof(InstrumentIdType),
                                                inst.InstrumentIdType.ToString());
                    instruments[i].idTypeSpecified = inst.InstrumentIdTypeSpecified;
                    instruments[i].marketSector = (MarketSector)Enum.Parse(typeof(MarketSector),
                                                inst.MarketSector.ToString());
                    instruments[i].marketSectorSpecified = inst.MarketSectorSpecified;
                    dictInstruments[instruments[i].id] = inst;
                    i++;
                }
                getDataRequest request = new getDataRequest();
                request.instruments = instruments;
                //request.view = "IVP View";
                request.view = ViewName;

                getDataResponse dataResponse = service.getData(request);

                if (dataResponse.statusCode.externalCode == 100 || dataResponse.statusCode.internalCode == 100)
                {
                    return new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                }
                else if (dataResponse.statusCode.externalCode == 0 
                            || dataResponse.statusCode.externalCode == 107)
                {
                    //TODO::Check status codes here
                    foreach (InstrumentData instrumentData in dataResponse.instrumentData)
                    {
                        if (instrumentData.statusCode.externalCode == 0 || instrumentData.statusCode.externalCode == 115)
                        {
                            string key =Convert.ToString(dictInstruments.Keys.Where(x => x.ToLower().Equals(instrumentData.instrument.id.ToLower())).FirstOrDefault());
                            RBbgInstrumentInfo responseInstrument = dictInstruments[key];
                            var responseList = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                            foreach (DataItem item in instrumentData.data)
                            {
                                RVendorFieldInfo responseField = new RVendorFieldInfo();
                                responseField.Name = item.field;
                                responseField.Value = item.value;
                                responseField.Status = RVendorStatusConstant.PASSED;
                                responseList.Add(responseField.Name, responseField);
                            }
                            returnReponse[responseInstrument.InstrumentID] = responseList;
                        }
                        else
                            continue;
                    }
                }
                else if (dataResponse.statusCode.externalCode != 0 || dataResponse.statusCode.internalCode != 0)
                {
                    RBbgUtils.HandleErrorCodes(dataResponse.statusCode.externalCode.ToString());
                    //throw new RVendorException(RBbgUtils.CreateExceptionMessage(dataResponse.statusCode.
                    //    internalCode.ToString(), dataResponse.statusCode.externalCode.ToString(), 
                    //    dataResponse.statusCode.description));
                }
                
                return returnReponse;
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
                if (service != null)
                    DeleteView(service);
                mLogger.Debug("End -> retrieving retrieving secuities info");
            }
        }
        //------------------------------------------------------------------------------------------
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
            for (int i = 0; i < viewNames.Length; i++)
            {
                DeleteViewDefinitionRequest viewDeleteRequest = new DeleteViewDefinitionRequest();
                viewDeleteRequest.viewName = viewResponse.viewName[i];
                service.deleteViewDefinition(viewDeleteRequest);
            }
        }
        #endregion
        //------------------------------------------------------------------------------------------
    }
}
