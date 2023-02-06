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
namespace com.ivp.srm.vendorapi
{
    public class RVendorFieldInfo
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the mnemonic field.
        /// </summary>
        /// <value>The mnemonic field.</value>
        public string Name { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        /// <value>The field value.</value>
        public string Value { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the exception message.
        /// </summary>
        /// <value>The exception message.</value>
        public string ExceptionMessage { get; set; }
    }
}
