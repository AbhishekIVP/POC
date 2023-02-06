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
using com.ivp.srm.vendorapi.Bloomberg;
using com.ivp.srm.vendorapi.bloomberg;

namespace com.ivp.srm.vendorapi
{
    /// <summary>
    /// Vendor API Manager
    /// </summary>
    public class RVendorManager
    {
        /// <summary>
        /// Gets the vendor.
        /// </summary>
        /// <param name="vendorType">Type of the vendor.</param>
        /// <returns></returns>
        public static IVendor GetVendor(RVendorType vendorType, int VendorPreferenceId)
        {
            switch (vendorType)
            {
                case RVendorType.Bloomberg:
                    isUserAuthenticated = Convert.ToBoolean(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, VendorPreferenceId, RVendorConstant.SAPI_VALIDATE_SESSION));
                    return new com.ivp.srm.vendorapi.bloomberg.RBloomberg();
                case RVendorType.Reuters:
                    return new com.ivp.srm.vendorapi.reuters.RReuter();
            }
            return null;
        }

        public static IVendor GetVendor(RVendorType vendorType)
        {
            return GetVendor(vendorType, 1);
        }

        public static IVendorForCorpAction GetVendorForCorpAction(RVendorType vendorType)
        {
            switch (vendorType)
            {
                case RVendorType.Bloomberg:
                    return new RCorpActController();
                case RVendorType.Reuters:
                    return null;
            }
            return null;
        }

        public static IVendorForGetHistory GetVendorForHistory(RVendorType vendorType)
        {
            switch (vendorType)
            {
                case RVendorType.Bloomberg:
                    return new RBbgHistoryController();
                case RVendorType.Reuters:
                    return null;
            }
            return null;
        }

        public static bool IsUserAutherized(string userName, List<string> ipAddresses, RBbgSecurityInfo securityInfo)
        {
            RBbgServerRequest serverRequest = new RBbgServerRequest();
            return serverRequest.IsUserAutherized(userName, ipAddresses, securityInfo);
        }

        private static bool isUserAuthenticated;
        public static bool SapiAuthenticated
        {
            get { return isUserAuthenticated; }
        }
    }
}
