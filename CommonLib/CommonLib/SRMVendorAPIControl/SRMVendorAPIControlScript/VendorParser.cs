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
    public class VendorParser
    {
        /// <summary>
        /// parsing through XMLDOM
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static XMLDOM GenericParser(string xml)
        {
            #region "Parsing through XMLDOM"

            XMLDOM xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
            xmlDoc.LoadXML(xml);
            xmlDoc.SetProperty("SelectionLanguage", "XPath");
            return xmlDoc;

            #endregion
        }
    }
}
