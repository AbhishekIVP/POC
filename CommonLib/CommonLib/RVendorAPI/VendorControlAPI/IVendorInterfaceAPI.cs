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
using System.Collections.Generic;
using System.ServiceModel;
using System.Data;
using System.Collections;


namespace com.ivp.srm.vendorapi
{
    /// <summary>
    /// Exposes the server side methods to the service
    /// </summary>
    [ServiceContract(Name = "VendorInterfaceAPI", Namespace = "com.ivp.srm.vendorapi")]
    public interface IVendorInterfaceAPI
    {
        [OperationContract]
        ArrayList GetBbgRequestType();
        
        [OperationContract]
        ArrayList GetBbgInstrumentIdType();
        
        [OperationContract]
        ArrayList GetBbgMarketSector();
        
        [OperationContract]
        ArrayList GetVendorTypes(string moduleId, string TypeOfControl);
        
        [OperationContract]
        string ReturnHTML(string cacheKey, string type, string assemblyName, string htmlLoc);//, string hasCustomIdentifier);
        
        [OperationContract]
        ArrayList GetRReuterRequestType();
        
        [OperationContract]
        ArrayList GetRReuterInstrumentIdType();
        
        [OperationContract]
        ArrayList GetRReuterAssetTypes();

        [OperationContract]
        ArrayList GetWSOInstrumentIdType();
        
        [OperationContract]
        DataSet GetGetAllTransports();

        [OperationContract]
        ArrayList GetAllTransportsNew();

        [OperationContract]
        object BindApplicationSpecificData(string className, string methodName, string assembly, 
            string id, string nameSpace);

        [OperationContract]
        ArrayList GetRequestType(RVendorType vendorType, RLicenseType licenseType);
    }
}
