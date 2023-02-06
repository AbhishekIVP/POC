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

namespace com.ivp.srm.vendorapi.bloomberg
{
    /// <summary>
    /// Type of Bloomberg Instrument Id
    /// </summary>
    public enum RBbgInstrumentIdType
    {
        /// <remarks/>
        NONE,

        /// <remarks/>
        AUSTRIAN,

       /// <remarks/>
        BB_UNIQUE ,

        /// <remarks/>
        BELGIAN,

        /// <remarks/>
        BLOOMBERG,

        /// <remarks/>
        CATS,

        /// <remarks/>
        CEDEL,

        /// <remarks/>
        CINS,

        /// <remarks/>
        COMMON_NUMBER,

        /// <remarks/>
        COMMON_HEADER,

        /// <remarks/>
        CUSIP,

        /// <remarks/>
        CZECH,

        /// <remarks/>
        DUTCH,

        /// <remarks/>
        EUROCLEAR,

        /// <remarks/>
        FRENCH,

        /// <remarks/>
        IRISH,

        /// <remarks/>
        ISIN,

        /// <remarks/>
        ISRAELI,

        /// <remarks/>
        ITALY,

        /// <remarks/>
        JAPAN,

        /// <remarks/>
        LUXEMBOURG,

        /// <remarks/>
        SEDOL,

        /// <remarks/>
        SEDOL1,

        /// <remarks/>
        SEDOL2,

        /// <remarks/>
        SPAIN,

        /// <remarks/>
        TICKER,

        /// <remarks/>
        TSCUSIP,

        /// <remarks/>
        VALOREN,

        /// <remarks/>
        WPK,

        /// <remarks/>
        BB_GLOBAL,
        /// <summary>
        /// 
        /// </summary>
        UNIQUE_ID_FUT_OPT,

        BB_COMPANY
    }

    public enum RbbgPricingType
    {
        MTD,
        WTD,
        DTD
    }
}
