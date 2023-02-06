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
 * 1            05-11-2009      Mukul Saini     Initial Version
 **************************************************************************************************/
using System;
using System.DHTML;

namespace com.ivp.srm.controls.vendor.script
{

    /// <summary>
    /// Custom implementation of XMLDOM for script sharp
    /// </summary>
    [IgnoreNamespace, Imported]
    public sealed class XMLDOM : DOMElement
    {
        #region "Public Methods"
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Loads the XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        public void LoadXML(string xml)
        {

        }
        [IntrinsicProperty]
        public new DOMElement[] ChildNodes
        {
            get
            {
                return null;
            }
        }

        public void SetProperty(string propertyName, string propertyValue)
        {

        }

        public ArrayList SelectNodes(string query)
        {
            return null;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="XMLDOM"/> class.
        /// </summary>
        private XMLDOM()
        {
            // Private to disallow direct creation. App code should use new ActiveXObject 
            // to create instances. 
        }

        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.ActiveXObject"/> to <see cref="com.ivp.srm.controls.vendor.script.XMLDOM"/>.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator XMLDOM(ActiveXObject o)
        {
            return null;
        }

        [IntrinsicProperty]
        public string Xml
        {
            get
            {
                return null;
            }
        }

        [IntrinsicProperty]
        public string Text
        {
            get
            {
                return null;
            }
        }
        #endregion
    }
}

