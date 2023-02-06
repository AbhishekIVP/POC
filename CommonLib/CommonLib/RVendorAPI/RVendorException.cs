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
 * 1            31-10-2008      Manoj           Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using com.ivp.rad.common;

namespace com.ivp.srm.vendorapi
{
    /// <summary>
    /// Provide methods related to Vendor Exception
    /// </summary>
    [Serializable]
    public class RVendorException : RException
    {
        private string _errorCode = null;
        //------------------------------------------------------------------------------------------

        public string ErrorCode
        { get { return _errorCode; } set { _errorCode = value; } }
        /// <summary>
        /// Initializes a new instance of the <see cref="RVendorException"/> class.
        /// </summary>
        public RVendorException()
            : base()
        {

        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="RVendorException"/> class.
        /// </summary>
        public RVendorException(Exception ex)
            : base(ex)
        {

        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="RVendorException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RVendorException(string message)
            : base(message)
        {
        }
    }
}
