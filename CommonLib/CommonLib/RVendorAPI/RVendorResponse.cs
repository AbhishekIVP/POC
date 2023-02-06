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
 * 1            31-12-2008      Manoj          Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.ivp.srm.vendorapi
{
    /// <summary>
    /// Class for vendor response
    /// </summary>
    public class RVendorResponse
    {
        /// <summary>
        /// Gets or sets the security response.
        /// </summary>
        /// <value>The security response.</value>
        public object SecurityResponse { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the exception message.
        /// </summary>
        /// <value>The exception message.</value>
        public string ExceptionMessage { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the response status.
        /// </summary>
        /// <value>The response status.</value>
        public RVendorResponseStatus ResponseStatus { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
        public string RequestIdentifier{ get; set; }
        /// <summary>
        /// request ids list for priority hit
        /// </summary>
        public List<string> RequestIdList { get; set; }

        public List<string> AdditionalInfo { get; set; }

        public List<string> ResponseAdditionalInfo { get; set; }

        public bool requireNotAvailableInField { get; set; }
    }
}
