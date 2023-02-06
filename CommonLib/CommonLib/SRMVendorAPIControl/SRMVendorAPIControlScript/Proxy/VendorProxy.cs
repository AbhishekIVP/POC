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
 * 1            05-11-2009      Mukul Saini     Initial Version
 **************************************************************************************************/
using System;

namespace com.ivp.srm.vendorapi
{
    [Imported]
    public class VendorInterfaceAPI
    {
        //[PreserveCase]
        //public void BindShuttleForIdentifiers(SuccessCallbackDelegate successClassbackHandler,
        //    FailureCallbackDelegate failureCallbackHandler)
        //{
        //}

       
        [PreserveCase]
        public void GetBbgRequestType(SuccessCallbackDelegate successClassbackHandler,
            FailureCallbackDelegate failureCallbackHandler)
        {
        }

        [PreserveCase]
        public void GetBbgInstrumentIdType(SuccessCallbackDelegate successClassbackHandler,
            FailureCallbackDelegate failureCallbackHandler)
        {
        }

        [PreserveCase]
        public void GetBbgMarketSector(SuccessCallbackDelegate successClassbackHandler,
            FailureCallbackDelegate failureCallbackHandler)
        {
        }

        [PreserveCase]
        public void GetVendorTypes(string moduleId, string TypeOfControl, SuccessCallbackDelegate successClassbackHandler,
            FailureCallbackDelegate failureCallbackHandler)
        {
        }

        [PreserveCase]
        public void ReturnHTML(string cacheKey, string type, string assemblyName, string htmlLoc,//string hasCustomIdentifier,
            SuccessCallbackDelegate successClassbackHandler, FailureCallbackDelegate failureCallbackHandler)
        {
        }

        [PreserveCase]
        public void GetRReuterRequestType(SuccessCallbackDelegate successClassbackHandler,
           FailureCallbackDelegate failureCallbackHandler)
        {
        }

        [PreserveCase]
        public void GetRReuterInstrumentIdType(SuccessCallbackDelegate successClassbackHandler,
          FailureCallbackDelegate failureCallbackHandler)
        {
        }

        [PreserveCase]
        public void GetRReuterAssetTypes(SuccessCallbackDelegate successClassbackHandler,
          FailureCallbackDelegate failureCallbackHandler)
        {
        }

        [PreserveCase]
        public void GetWSOInstrumentIdType(SuccessCallbackDelegate successClassbackHandler,
          FailureCallbackDelegate failureCallbackHandler)
        {
        }
        
        [PreserveCase]
        public void GetGetAllTransports(SuccessCallbackDelegate successClassbackHandler,
          FailureCallbackDelegate failureCallbackHandler)
        {
        }

        [PreserveCase]
        public void GetAllTransportsNew(SuccessCallbackDelegate successClassbackHandler,
          FailureCallbackDelegate failureCallbackHandler)
        {
        }

        //[PreserveCase]
        //public void GetBindDataSourceForRealTimePreference(SuccessCallbackDelegate successClassbackHandler,
        //  FailureCallbackDelegate failureCallbackHandler)
        //{
        //}



        [PreserveCase]
        public void BindApplicationSpecificData(string className, string methodName, 
            string assembly, string id, string nameSpace,
                SuccessCallbackDelegate successClassbackHandler,
                    FailureCallbackDelegate failureCallbackHandler)
        {
        }

        [PreserveCase]
        public void GetRequestType(int vendorType, int licenseType,
            SuccessCallbackDelegate successClassbackHandler, FailureCallbackDelegate failureCallbackHandler)
        {

        }

    }

    [Imported]
    public delegate void SuccessCallbackDelegate(object stringResult, object eventArgs);

    [Imported]
    public delegate void FailureCallbackDelegate(object eventArgs);

    public class RADWebServiceException
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Get_messages this instance.
        /// </summary>
        /// <returns></returns>
        public string Get_message()
        {
            return "";
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Get_statuses the code.
        /// </summary>
        /// <returns></returns>
        public int Get_statusCode()
        {
            return 0;
        }
    }

}
