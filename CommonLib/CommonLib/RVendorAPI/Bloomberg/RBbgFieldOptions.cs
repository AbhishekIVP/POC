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
    /// Bloomberg Field Options Info
    /// </summary>
    public class RBbgFieldOptionsInfo
    {
        #region "Private Variables"
        private bool _realValueField;
        private int _precisionField;
        #endregion

        #region "Public Properties"
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [real value field].
        /// </summary>
        /// <value><c>true</c> if [real value field]; otherwise, <c>false</c>.</value>

        public bool RealValueField
        {
            get
            {
                return _realValueField;
            }
            set
            {
                _realValueField = value;
                RealValueFieldSpecified = true;
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [real value field specified].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [real value field specified]; otherwise, <c>false</c>.
        /// </value>
        internal bool RealValueFieldSpecified { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the precision field.
        /// </summary>
        /// <value>The precision field.</value>
        public int PrecisionField
        {
            get
            {
                return _precisionField;
            }
            set
            {
                _precisionField = value;
                PrecisionFieldSpecified = true;
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether [precision field specified].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [precision field specified]; otherwise, <c>false</c>.
        /// </value>
        internal bool PrecisionFieldSpecified { get; set; }
        #endregion
    }
}
