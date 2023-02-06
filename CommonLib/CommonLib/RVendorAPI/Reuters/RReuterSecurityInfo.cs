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
    /// <summary>
    /// Security Info for Reuter Data Request
    /// </summary>
    public class RReuterSecurityInfo
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the instruments.
        /// </summary>
        /// <value>The instruments.</value>
        public List<RReuterInstrumentInfo> Instruments { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the instrument fields.
        /// </summary>
        /// <value>The instrument fields.</value>
        public List<string> InstrumentFields { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the asset types.
        /// </summary>
        /// <value>The asset types.</value>
        public List<RReuterAssetTypes> AssetTypes { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of the request.
        /// </summary>
        /// <value>The type of the request.</value>
        public RReuterRequestType RequestType { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the transport.
        /// </summary>
        /// <value>The name of the transport.</value>
        public string TransportName { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of the instrument identifier.
        /// </summary>
        /// <value>The type of the instrument identifier.</value>
        public RReuterInstrumentIdType InstrumentIdentifierType { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the user login.
        /// </summary>
        /// <value>The name of the user login.</value>
        public string UserLoginName { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the module id.
        /// </summary>
        /// <value>The module id.</value>
        public int ModuleId{ get; set; }

        public Dictionary<string, string> HeaderNamesVsValues { get; set; }

        public int VendorPreferenceId { get; set; }
    }
}
