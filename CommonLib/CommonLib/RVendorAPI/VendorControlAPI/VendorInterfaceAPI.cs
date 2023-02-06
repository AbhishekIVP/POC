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
using System.Collections.Generic;
using System.Data;
using System.Collections;
using com.ivp.srm.vendorapi.bloomberg;
using com.ivp.srm.vendorapi;
using System.ServiceModel.Activation;
using System.ServiceModel;
using com.ivp.srm.vendorapi.reuters;
using com.ivp.rad.transport;
using System.Reflection;
using System.IO;
using System.Web.Script.Serialization;
using com.ivp.rad.utils;

namespace com.ivp.srm.vendorapi
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class VendorInterfaceAPI : RServiceBaseClass, IVendorInterfaceAPI
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the type of the BBG request.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetBbgRequestType()
        {
            return (EnumHelper(typeof(RBbgRequestType)));
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the type of the BBG instrument id.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetBbgInstrumentIdType()
        {
            return (EnumHelper(typeof(RBbgInstrumentIdType)));
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the type of BBG market sector.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetBbgMarketSector()
        {
            return (EnumHelper(typeof(RBbgMarketSector)));
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the vendor types.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetVendorTypes(string moduleId, string TypeOfControl)
        {
            ArrayList vendorTypes = (EnumHelper(typeof(RVendorType)));
            if (TypeOfControl != "API" || string.IsNullOrEmpty(moduleId) || moduleId != "3")
                vendorTypes.Remove(RVendorType.MarkitWSO.ToString());
            return vendorTypes;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Converts the enumeration  to an ArrayList.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns></returns>
        private ArrayList EnumHelper(Type enumType)
        {
            ArrayList nameArray = new ArrayList();
            nameArray.AddRange(Enum.GetNames(enumType));
            nameArray.Sort();
            return nameArray;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the HTML according to vendorType.
        /// </summary>
        /// <param name="ClientID">The client ID of the control to be populated.</param>
        /// <param name="type">The type of vendor.</param>
        /// <param name="hasCustomIdentifier">has custom identifier(sec master).</param>
        /// <returns>html according to type of control</returns>
        public string ReturnHTML(string ClientID, string type, string assemblyName, string htmlLoc)//, string hasCustomIdentifier)
        {
            AssemblyName vendorAssemblyName = null;
            Assembly vendorAssembly = null;
            StreamReader _textStreamReader = null;
            string dataRead = string.Empty;

            try
            {
                //vendorAssemblyName = new AssemblyName("SRMVendorAPIControl");
                vendorAssemblyName = new AssemblyName(assemblyName);
                vendorAssembly = Assembly.Load(vendorAssemblyName);
                _textStreamReader = new StreamReader(vendorAssembly.GetManifestResourceStream
                    //("com.ivp.srm.controls.vendor.Resources.HTML." + type.ToLower() + ".htm"));
                    (htmlLoc + "." + type.ToLower() + ".htm"));
                dataRead = _textStreamReader.ReadToEnd();
                switch (type.ToLower())
                {
                    #region "API"
                    case TypeOfVendorControl.API:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty);
                        break;
                    #endregion

                    #region "API_*"
                    case TypeOfVendorControl.API_Bloomberg:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty);
                        //hasCustomIdentifier);
                        break;
                    case TypeOfVendorControl.API_Reuters:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty);
                        //hasCustomIdentifier);
                        break;
                    case TypeOfVendorControl.API_MarkitWSO:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty);
                        break;
                    #endregion

                    #region "REALTIMEPREFERENCE_*"
                    case TypeOfVendorControl.REALTIMEPREFERENCE:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty);
                        break;

                    case TypeOfVendorControl.REALTIMEPREFERENCE_Bloomberg:
                        //dataRead = string.Format(
                        //    dataRead,
                        //    ClientID,
                        //    string.Empty,
                        //    string.Empty,
                        //    string.Empty,
                        //    string.Empty,
                        //    string.Empty,
                        //    string.Empty,
                        //    string.Empty,
                        //    string.Empty,
                        //    string.Empty);
                        //break;
                    case TypeOfVendorControl.REALTIMEPREFERENCE_Reuters:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty);
                        break;

                    #endregion

                    #region "FTP"
                    case TypeOfVendorControl.FTP:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            string.Empty,
                            string.Empty,
                            string.Empty);
                        break;
                    #endregion

                    #region "FTP_*"
                    case TypeOfVendorControl.FTP_Bloomberg:
                        dataRead = string.Format(
                        dataRead,
                        ClientID,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty);
                        //hasCustomIdentifier);
                        break;
                    case TypeOfVendorControl.FTP_Reuters:
                        dataRead = string.Format(
                        dataRead,
                        ClientID,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty);
                        //hasCustomIdentifier);
                        break;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_textStreamReader != null)
                {
                    _textStreamReader.Close();
                    _textStreamReader.Dispose();
                }
            }
            return dataRead;
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the type of the request reuter.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetRReuterRequestType()
        {
            return (EnumHelper(typeof(RReuterRequestType)));
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the type of the reuter instrument id.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetRReuterInstrumentIdType()
        {
            return (EnumHelper(typeof(RReuterInstrumentIdType)));
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the reuter asset types.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetRReuterAssetTypes()
        {
            return (EnumHelper(typeof(RReuterAssetTypes)));
        }

        public ArrayList GetWSOInstrumentIdType()
        {
            ArrayList list = new ArrayList { "CUSIP", "ISIN", "LIN", "LoanX", "WSODataFacilityID" };
            return list;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the type of transports.
        /// </summary>
        /// <returns></returns>
        public DataSet GetGetAllTransports()
        {
            DataSet ds = RTransportConfigLoader.GetTransportsByTransportName("FTP");
            DataSet ds1 = RTransportConfigLoader.GetTransportsByTransportName("SFTP");
            if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].Merge(ds1.Tables[0]);
                    }
                }
                else if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    return ds1;
                }
            return ds;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the type of transports.
        /// </summary>
        /// <returns></returns>
        public ArrayList GetAllTransportsNew()
        {
            ArrayList result = new ArrayList();
            DataSet ds = RTransportConfigLoader.GetTransportsByTransportName("FTP");
            DataSet ds1 = RTransportConfigLoader.GetTransportsByTransportName("SFTP");

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(Convert.ToString(dr["transport_name"]));
                }
            }
            if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds1.Tables[0].Rows)
                {
                    result.Add(Convert.ToString(dr["transport_name"]));
                }
            }

            return result;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the methods of the third party classes  
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="assembly">Name of the assembly.</param>
        /// <param name="id">The id of the control to which the data is bound.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <returns>javascript serialized object of ApplicationSpecificData class </returns>
        public object BindApplicationSpecificData(string className, string methodName, string assembly,
            string id, string nameSpace)
        {
            ApplicationSpecificData data = new ApplicationSpecificData();
            AssemblyName thirdPartyAssemblyName;
            Assembly thirdPartyAssembly;
            thirdPartyAssemblyName = new AssemblyName(assembly);
            thirdPartyAssembly = Assembly.Load(thirdPartyAssemblyName);
            Type t = thirdPartyAssembly.GetType(nameSpace + "." + className);

            MethodInfo methodInfo = t.GetMethod(methodName);
            object obj = Activator.CreateInstance(t);
            object result = methodInfo.Invoke(obj, new Object[] { });

            data.ControlId = id;
            if (methodInfo.ReturnType.Name == "DataSet")
            {
                data.ReturnData = ((DataSet)result).GetXml();
                data.DataType = DataType.DataSet;
            }
            else
            {
                data.ReturnData = result;
                data.DataType = DataType.ArrayList;
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(data);
        }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        /// <param name="vendorType">Type of the vendor.</param>
        /// <param name="licenseType">Type of the license.</param>
        /// <returns></returns>
        public ArrayList GetRequestType(RVendorType vendorType, RLicenseType licenseType)
        {
            switch (vendorType)
            {
                case RVendorType.Bloomberg:
                    switch (licenseType)
                    {
                        case RLicenseType.API:
                            return (EnumHelper(typeof(RBbgRequestTypeAPI)));
                        case RLicenseType.FTP:
                            return (EnumHelper(typeof(RBbgRequestTypeFTP)));
                        case RLicenseType.ALL:
                            return (EnumHelper(typeof(RBbgRequestType)));
                    }
                    break;
                case RVendorType.Reuters:
                    switch (licenseType)
                    {
                        case RLicenseType.API:
                            return (EnumHelper(typeof(RReuterRequestTypeAPI)));
                        case RLicenseType.FTP:
                            return (EnumHelper(typeof(RReuterRequestTypeFTP)));
                        case RLicenseType.ALL:
                            return (EnumHelper(typeof(RReuterRequestType)));
                    }
                    break;
            }
            return null;
        }
    }
    //------------------------------------------------------------------------------------------
    /// <summary>
    /// ReturnData is xml or arraylist
    /// DataType is of enum DataType
    /// ControlId is the id of the control to which data would be bound
    /// </summary>
    internal class ApplicationSpecificData
    {
        public object ReturnData;
        public DataType DataType;
        public string ControlId;
    }

    /// <summary>
    /// enum for DataType in ApplicationSpecificData class
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// 
        /// </summary>
        DataSet = 0,
        /// <summary>
        /// 
        /// </summary>
        ArrayList = 1
    }

    /// <summary>
    /// contains all the types of controls supported
    /// </summary>
    public class TypeOfVendorControl
    {
        public const string API = "api";
        public const string API_Bloomberg = "api_bloomberg";
        public const string API_Reuters = "api_reuters";
        public const string API_MarkitWSO = "api_markitwso";
        public const string FTP = "ftp";
        public const string FTP_Bloomberg = "ftp_bloomberg";
        public const string FTP_Reuters = "ftp_reuters";
        public const string REALTIMEPREFERENCE_Bloomberg = "realtimepreference_bloomberg";
        public const string REALTIMEPREFERENCE_Reuters = "realtimepreference_reuters";
        public const string REALTIMEPREFERENCE = "realtimepreference";
    }

    public enum RLicenseType
    {
        ALL = 0,
        API = 1,
        FTP = 2,
    }
}
