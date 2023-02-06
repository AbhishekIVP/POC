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

namespace com.ivp.srm.controls.vendor.script
{
    /// <summary>
    /// an interface for client methods
    /// </summary>
    public interface IClientMethods
    {
        void ClearControl(DomEvent e);
        void DisplayControl(DomEvent e);
        void GetOutputXML(DomEvent e);
        void RequestClicked(DomEvent e);
        void ResponseClicked(DomEvent e);
        void HideShowTransport(DomEvent e);
    }

    /// <summary>
    /// info class for all the client methods
    /// </summary>
    public class ClientMethodNames
    {
        internal const string ClearHandler = "ClearControl";        
        internal const string DisplayControl = "DisplayControl";
        internal const string GetOutputXML = "GetOutputXML";        
        internal const string RequestClicked = "RequestClicked";
        internal const string ResponseClicked = "ResponseClicked";
        internal const string HideShowTransport = "HideShowTransport";
    }
}
