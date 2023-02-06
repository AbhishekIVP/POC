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
using System.Threading;

namespace com.ivp.srm.vendorapi.bloomberg
{
    /// <summary>
    /// Security Info for Bloomberg Data Request
    /// </summary>
    public class RBbgSecurityInfo
    {
        public int counter;
        public RBbgSecurityInfo()
        {
            Instruments = new List<RBbgInstrumentInfo>();
            InstrumentFields = new List<RBbgFieldInfo>();
            Overrides = new Dictionary<string, object>();
            IPAddresses = new List<string>();
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the instruments.
        /// </summary>
        /// <value>The instruments.</value>
        public List<RBbgInstrumentInfo> Instruments { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the instrument fields.
        /// </summary>
        /// <value>The instrument fields.</value>
        public List<RBbgFieldInfo> InstrumentFields { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the Macro info.
        /// </summary>
        /// <value>The Macro Info.</value>
        public Dictionary<RBbgMacroType, List<RBbgMacroInfo>> MacroInfo { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of the request.
        /// </summary>
        /// <value>The type of the request.</value>
        public RBbgRequestType RequestType { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the transport.
        /// </summary>
        /// <value>The name of the transport.</value>
        public string TransportName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the user login.
        /// </summary>
        /// <value>The name of the user login.</value>
        public string UserLoginName { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the market sector.
        /// </summary>
        /// <value>The market sector.</value>
        public RBbgMarketSector MarketSector { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of the instrument identifier.
        /// </summary>
        /// <value>The type of the instrument identifier.</value>
        public RBbgInstrumentIdType InstrumentIdentifierType { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether this instance is bulk list.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is bulk list; otherwise, <c>false</c>.
        /// </value>
        public bool IsBulkList { get; set; }
        /// <summary>
        /// for requesting both bulk and normal fields from FTP
        /// </summary>
        public bool IsCombinedFtpReq { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is BVAL.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is BVAL; otherwise, <c>false</c>.
        /// </value>
        public bool IsBVAL { get; set; }

        public bool IsBVALPricingSource { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is BVAL.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is BVAL; otherwise, <c>false</c>.
        /// </value>
        public bool IsGetCompany { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the module id.
        /// </summary>
        public int ModuleId { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the bulk list map id.
        /// </summary>
        public List<int> BulkListMapId { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        public string RequestIdentifier { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the manual reset event.
        /// </summary>
        internal ManualResetEvent ManualResetEvent { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [immediate request].
        /// </summary>
        public bool ImmediateRequest { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the SAPI User Name.
        /// </summary>
        public string SAPIUserName { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the IP addresses.
        /// </summary>
        /// <value>The IP addresses.</value>
        public List<string> IPAddresses { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the current object.
        /// </summary>
        /// <value>The current object.</value>
        internal RBloomberg CurrentObject { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Uniquely identify security from a collection
        /// </summary>
        public string SecurityId { get; set; }

        public Dictionary<string, object> Overrides { get; set; }

        public bool IsMarketSectorAppended { get; set; }

        public List<string> AdditionalInfo { get; set; }

        public List<string> ResponseAdditionalInfo { get; set; }

        public Dictionary<string, string> HeaderNamesVsValues { get; set; }

        public int VendorPreferenceId { get; set; }

        public bool requireNotAvailableInField { get; set; }

        public string ClientName{ get; set; }

    }

    //----------------------------------------------------------------------------------------------
    /// <summary>
    /// Macro Qualifier Types
    /// </summary>
    public enum RBbgMacroType
    {
        /// <summary>
        /// Primary Qualifier
        /// </summary>
        Primary,
        /// <summary>
        /// Secondary Qualifier
        /// </summary>
        Secondary
    }

    public enum RBbgAlertType
    {
        MonthlyHardAlert,
        MonthlySoftAlert,
        DailyHardAlert,
        DailySoftAlert,
        NoAlertDefined
    }

}
