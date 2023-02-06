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
using Sys.UI;
using System.DHTML;

namespace com.ivp.srm.controls.vendor.script
{
    public class ManageHandlers
    {
        private static ArrayList _elementList;
        private static ArrayList ElementList
        {
            get
            {
                if (_elementList == null)
                    _elementList = new ArrayList();
                return _elementList;
            }
        }

        /// <summary>
        /// Adds the handler.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="clientID">The client ID.</param>
        /// <param name="instance">The instance.</param>
        public static void AddHandler(string methodName, string eventType, string clientID, VendorScript instance)
        {
            switch (methodName)
            {
                case ClientMethodNames.ClearHandler:
                    DomEvent.AddHandler(VendorControls.GetElementByID(clientID), eventType, instance.ClearControl);
                    break;                
                case ClientMethodNames.DisplayControl:
                    DomEvent.AddHandler(VendorControls.GetElementByID(clientID), eventType, instance.DisplayControl);
                    break;
                case ClientMethodNames.GetOutputXML:
                    DomEvent.AddHandler(VendorControls.GetElementByID(clientID), eventType, instance.GetOutputXML);
                    break;
                case ClientMethodNames.RequestClicked:
                    DomEvent.AddHandler(VendorControls.GetElementByID(clientID), eventType, instance.RequestClicked);
                    break;
                case ClientMethodNames.ResponseClicked:
                    DomEvent.AddHandler(VendorControls.GetElementByID(clientID), eventType, instance.ResponseClicked);
                    break;
                case ClientMethodNames.HideShowTransport:
                    DomEvent.AddHandler(VendorControls.GetElementByID(clientID), eventType, instance.HideShowTransport);
                    break;
            }
            if (clientID != null && clientID != "")
                ArrayList.Add(ElementList, clientID);
        }

        /// <summary>
        /// Removes the handler.
        /// </summary>
        /// <param name="clientID">The client ID.</param>
        public static void RemoveHandler(string clientID)
        {
            DomEvent.ClearHandlers(VendorControls.GetElementByID(clientID));
        }

        public static void RemoveHandlers()
        {
            DOMElement element = null;
            for (int i = 0; i < ElementList.Length; i++)
            {
                element = VendorControls.GetElementByID(ElementList[i].ToString());
                if (element != null)
                {
                    DomEvent.ClearHandlers(element);
                }
            }
        }
    }    
}
