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
 * 1            29-04-2009      Nitin Saxena    Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace com.ivp.srm.vendorapi.bloomberg
{
    /// <summary>
    /// Class for Bulk List Info
    /// </summary>
    public class RBbgBulkListInfo
    {
        /// <summary>
        /// Gets or sets the bulk list map id.
        /// </summary>
        public int BulkListMapId { get; set; }
        /// <summary>
        /// Gets or sets the request field.
        /// </summary>
        public string RequestField{ get; set; }
        /// <summary>
        /// Gets or sets the output fields.
        /// </summary>
        public string OutputFields{ get; set; }
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public string CreatedBy{ get; set; }
        /// <summary>
        /// Gets or sets the market sector.
        /// </summary>
        public string MarketSector{ get; set; }
    }
}
