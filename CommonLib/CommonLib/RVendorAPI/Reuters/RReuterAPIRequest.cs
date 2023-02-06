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
using System.Collections.Generic;
using com.ivp.rad.common;
using System;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;

namespace com.ivp.srm.vendorapi.reuters
{
    /// <summary>
    /// Implementation of Reuters Webservice API
    /// </summary>
    internal class RReuterAPIRequest
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RReuterAPIRequest");
        #region "Properties"
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>The service.</value>
        private ExtractionService Service;
        #endregion

        #region "Public Methods"
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the securities.
        /// </summary>
        /// <param name="secInfo">The sec info.</param>
        /// <returns></returns>
        public SecuritiesCollection GetSecurities(RReuterSecurityInfo secInfo)
        {
            mLogger.Debug("Start->Get Securities for Reuters using API Service");

            InstrumentValidationRequest valRequest = null;
            InstrumentValidationResponse valResponse = null;
            ExtractionResponse response = null;

            Service = RReuterUtils.GetService(secInfo.VendorPreferenceId);

            #region "Validate Instruments"
            valRequest = new InstrumentValidationRequest();
            try
            {
                valRequest.Identifiers = GetInstrumentIdentifiers(secInfo);
                valResponse = Service.ValidateInstruments(valRequest);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                RVendorException vEx = new RVendorException(ex.Message);
                throw vEx;
                //throw new RVendorException(ex);
            }
            #endregion

            #region "Extract Instruments"
            InstrumentExtractionRequestTermsAndConditions request =
                                                new InstrumentExtractionRequestTermsAndConditions();

            request.Instruments = valResponse.Instruments;
            try
            {
                request.OutputFields = GetFields(secInfo);
                response = Service.Extract(request);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex);
            }
            #endregion

            #region "Create Securities from response"
            SecuritiesCollection securities = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < request.Instruments.Length; i++)
            {
                var vendorFieldInfo = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                foreach (ExtractionColumn col in response.Columns)
                {
                    var fieldInfo = new RVendorFieldInfo();
                    fieldInfo.Name = col.Name;
                    if (col.Values[i] is com.ivp.srm.vendorapi.reuters.EmptyValue)
                        fieldInfo.Value = "";
                    else
                        fieldInfo.Value = col.Values[i].ToString();
                    fieldInfo.ExceptionMessage = "";
                    fieldInfo.Status = RVendorStatusConstant.PASSED;
                    vendorFieldInfo.Add(fieldInfo.Name, fieldInfo);
                }
                securities[request.Instruments[i].IdentifierValue] = vendorFieldInfo;
            }
            #endregion

            mLogger.Debug("End->Get Securities for Reuters using API Service");
            return securities;
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region "Private Methods"
        /// <summary>
        /// Gets the instrument identifiers.
        /// </summary>
        /// <param name="secInfo">The sec info.</param>
        /// <returns></returns>
        private InstrumentIdentifier[] GetInstrumentIdentifiers(RReuterSecurityInfo secInfo)
        {
            mLogger.Debug("Start-> Get Object of Reuter Instrument Identifiers");
            List<InstrumentIdentifier> instrumentsToValidate = null;
            instrumentsToValidate = new List<InstrumentIdentifier>();
            InstrumentIdentifier identifier = null;
            foreach (RReuterInstrumentInfo instrumentInfo in secInfo.Instruments)
            {
                identifier = new InstrumentIdentifier();
                identifier.IdentifierValue = instrumentInfo.InstrumentID;
                identifier.IdentifierType = Enum.GetName(typeof(RReuterInstrumentIdType), instrumentInfo.InstrumentIdType);
                instrumentsToValidate.Add(identifier);
            }
            mLogger.Debug("End-> Get Object of Reuter Instrument Identifiers");
            return instrumentsToValidate.ToArray();
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <param name="secInfo">The sec info.</param>
        /// <returns></returns>
        private string[] GetFields(RReuterSecurityInfo secInfo)
        {
            mLogger.Debug("Start-> Get Fields(Data Dictionary from Reuters");
            DataDictionaryRequestInstrument ddRequest = new DataDictionaryRequestInstrument();
            ddRequest.InstrumentExtractionType = "TNC";
            if (secInfo.AssetTypes != null)
            {
                List<string> assetTypes = new List<string>();
                foreach (RReuterAssetTypes asset in secInfo.AssetTypes)
                {
                    assetTypes.Add(Enum.GetName(typeof(RReuterAssetTypes), asset));
                }
                ddRequest.AssetTypes = assetTypes.ToArray();
            }
            DataDictionaryResponse response = Service.Define(ddRequest);
            DataDictionaryField[] fields = response.Fields;
            List<string> validFields = new List<string>();
            List<string> validInstrumentFields = new List<string>();


            foreach (DataDictionaryField field in fields)
            {
                validFields.Add(field.Name);
            }
            foreach (string instField in secInfo.InstrumentFields)
            {
                if (validFields.Contains(instField))
                {
                    validInstrumentFields.Add(instField);
                }
            }
            mLogger.Debug("End-> Get Fields(Data Dictionary from Reuters");
            return validInstrumentFields.ToArray();
        }
        #endregion
    }
}
