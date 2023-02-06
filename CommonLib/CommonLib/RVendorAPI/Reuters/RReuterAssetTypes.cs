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
using System.Linq;
using System.Text;

namespace com.ivp.srm.vendorapi.reuters
{
    public enum RReuterAssetTypes
    {
        
        /// <summary>
        /// Equity
        /// </summary>
        EQTY,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Convertible
        /// </summary>
        CONV,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Corporate (Invest Grade)
        /// </summary>
        CORP,

        

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Futures & Options
        /// </summary>
        FOPT,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gov’t/Agency
        /// </summary>
        GORP,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Corporate (High Yield)
        /// </summary>
        HIYD,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Loans
        /// </summary>
        LOAN,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Money Market
        /// </summary>
        MMKT,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// MBS
        /// </summary>
        MORT,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Municipal
        /// </summary>
        MUNI,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Mutual Funds
        /// </summary>
        MUTL,

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// CMO/ABS
        /// </summary>
        TRNC
    }
}
