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
 * * Change History
 * Version      Date            Author          Comments
 * -------------------------------------------------------------------------------------------------
 * 1            24-10-2008      Manoj          Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.srm.vendorapi.bloombergServices;
using System.Security.Cryptography.X509Certificates;
using com.ivp.srm.vendorapi.bloombergServices.lite;
using com.ivp.srm.vendorapi.bloombergServices.heavy;
using com.ivp.rad.utils;
using com.ivp.rad.common;
using com.ivp.rad.BusinessCalendar;
using Bloomberglp.Blpapi;
//using HeavyRequest = com.ivp.srm.vendorapi.bloombergServices.heavy;
using com.ivp.rad.configurationmanagement;
using System.Xml.Linq;
using com.ivp.srm.vendorapi.AlterLoggerService;
using System.Configuration;
using com.ivp.srm.vendorapi.ServiceReference1;
using com.ivp.rad.cryptography;
using System.Data;
//using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using com.ivp.srmcommon;
//using com.ivp.srm.vendorapi.ServiceReference1;


namespace com.ivp.srm.vendorapi.bloomberg

{
    /// <summary>
    /// Bloomber Utilities
    /// </summary>
    public class RBbgUtils
    {
        #region Member Variables
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        static RRSAEncrDecr encDec = new RRSAEncrDecr();
        #endregion

        #region Internal Methods

        #region "Get Service"
        /// <summary>
        /// Gets the Bloomberg service.
        /// </summary>
        /// <returns></returns>
        internal static GetDataService GetLiteService(RBbgSecurityInfo securityInfo)
        {
            GetDataService service = null;
            mLogger.Debug("Start -> retrieving Lite Service");
            try
            {
                service = new GetDataService();
                string serverPath = RADConfigReader.GetServerPhysicalPath();
                string filePath = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.CERTIFICATEPATH);
                service.Url = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.WEBSERVICEURLLITE);
                string password = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.PASSWORD);
                if (!string.IsNullOrEmpty(password))
                {
                    string tempPass = password;
                    try
                    {
                        password = encDec.DoDecrypt(password);
                    }
                    catch (Exception ex)
                    {
                        password = tempPass;
                        mLogger.Error("GetLiteService => " + ex.ToString());
                    }
                }
                X509Certificate2 clientCert = new X509Certificate2(serverPath + filePath, password);
                service.ClientCertificates.Add(clientCert);

            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End -> retrieving Lite Service");
            }
            return service;
        }

        //-------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the heavy service.
        /// </summary>
        /// <returns></returns>
        internal static PerSecurityWS GetHeavyService(RBbgSecurityInfo securityInfo)
        {
            PerSecurityWSClient ps = null;
            mLogger.Debug("Start -> retrieving Heavy Service");
            try
            {
                ps = new PerSecurityWSClient("PerSecurityWSPort");

                string serverPath = RADConfigReader.GetServerPhysicalPath();
                string filePath = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.CERTIFICATEPATH);
                //ps.Url = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg)[RVendorConstant.WEBSERVICEURLHEAVY];
                string password = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.PASSWORD);
                if (!string.IsNullOrEmpty(password))
                {
                    string tempPass = password;
                    try
                    {
                        password = encDec.DoDecrypt(password);
                    }
                    catch (Exception ex)
                    {
                        password = tempPass;
                        mLogger.Error("GetHeavyService => " + ex.ToString());
                    }
                }
                X509Certificate2 clientCert = new X509Certificate2(serverPath + filePath, password);
                ps.ClientCredentials.ClientCertificate.Certificate = (clientCert);
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End -> retrieving Heavy Service");
            }
            return ps;
        }
        //-------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the server API session.
        /// </summary>
        /// <returns></returns>
        internal static Session GetServerAPISession(int VendorPreferenceId)
        {
            mLogger.Debug("Start -> creating serverapi session");
            SessionOptions sessionOptions = null;
            Session session = null;
            try
            {
                sessionOptions = new SessionOptions();
                sessionOptions.ServerHost = RVendorConfigLoader.
                                            GetVendorConfig(RVendorType.Bloomberg, VendorPreferenceId, RVendorConstant.SERVERHOST);
                sessionOptions.ServerPort = Convert.ToInt32(RVendorConfigLoader.GetVendorConfig
                                            (RVendorType.Bloomberg, VendorPreferenceId, RVendorConstant.SERVERPORT));
                // Establish Session//
                mLogger.Debug("GetServerAPISession=>server host = " + sessionOptions.ServerHost);
                mLogger.Debug("GetServerAPISession=>server port = " + sessionOptions.ServerPort);
                session = new Session(sessionOptions);
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->  creating serverapi session");
            }
            return session;
        }

        internal static Session GetManagedBPipeSession(string authentication, RBbgSecurityInfo securityInfo)
        {
            mLogger.Debug("Start -> creating serverapi session");
            SessionOptions sessionOptions = null;
            Session session = null;
            try
            {
                sessionOptions = new SessionOptions();
                sessionOptions.ServerHost = RVendorConfigLoader.
                                            GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MANAGEDSERVERHOST);
                sessionOptions.ServerPort = Convert.ToInt32(RVendorConfigLoader.GetVendorConfig
                                            (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MANAGEDSERVERPORT));
                // Establish Session//
                string applicationName = RVendorConfigLoader.GetVendorConfig
                                            (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MANAGEDAPPNAME);

                switch (authentication)
                {
                    case "APPLICATION_ONLY":
                        sessionOptions.AuthenticationOptions = "AuthenticationMode=APPLICATION_ONLY;ApplicationAuthenticationType=APPNAME_AND_KEY;ApplicationName=" + applicationName; //ivp:Security Master";
                        break;
                    case "OS_LOGON":
                        sessionOptions.AuthenticationOptions = "AuthenticationType=OS_LOGON";
                        break;
                    case "DIRECTORY_SERVICE":
                        sessionOptions.AuthenticationOptions = "AuthenticationType=DIRECTORY_SERVICE;DirSvcProperty=mail";
                        break;
                    case "USER_AND_APPLICATION":
                        sessionOptions.AuthenticationOptions = "AuthenticationMode=USER_AND_APPLICATION;ApplicationAuthenticationType=APPNAME_AND_KEY;ApplicationName=" + applicationName + ";AuthenticationType=OS_LOGON";
                        break;
                }
                session = new Session(sessionOptions);
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->  creating serverapi session");
            }
            return session;
        }

        internal static Session GetGlobalAPISession(string authentication, RBbgSecurityInfo securityInfo)
        {
            mLogger.Debug("Start -> creating serverapi session");
            SessionOptions sessionOptions = null;
            Session session = null;
            try
            {
                sessionOptions = new SessionOptions();
                sessionOptions.ServerHost = RVendorConfigLoader.
                                            GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.GLOBALSERVERHOST);
                sessionOptions.ServerPort = Convert.ToInt32(RVendorConfigLoader.GetVendorConfig
                                            (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.GLOBALSERVERPORT));
                // Establish Session//
                string applicationName = RVendorConfigLoader.GetVendorConfig
                                            (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.GLOBALAPPNAME);

                switch (authentication)
                {
                    case "APPLICATION_ONLY":
                        sessionOptions.AuthenticationOptions = "AuthenticationMode=APPLICATION_ONLY;ApplicationAuthenticationType=APPNAME_AND_KEY;ApplicationName=" + applicationName;
                        break;
                    case "OS_LOGON":
                        sessionOptions.AuthenticationOptions = "AuthenticationType=OS_LOGON";
                        break;
                    case "DIRECTORY_SERVICE":
                        sessionOptions.AuthenticationOptions = "AuthenticationType=DIRECTORY_SERVICE;DirSvcProperty=mail";
                        break;
                    case "USER_AND_APPLICATION":
                        sessionOptions.AuthenticationOptions = "AuthenticationMode=USER_AND_APPLICATION;ApplicationAuthenticationType=APPNAME_AND_KEY;ApplicationName=" + applicationName + ";AuthenticationType=OS_LOGON";
                        break;
                }
                session = new Session(sessionOptions);
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->  creating serverapi session");
            }
            return session;
        }
        #endregion

        #region Heavy Request
        /// <summary>
        /// Generates the macro request.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        internal static Instruments GenerateMacroRequest(RBbgSecurityInfo securityInfo)
        {
            string clientName = SRMMTConfig.GetClientName();

            Instruments instrs = new Instruments();
            #region Primary Qualifier
            //Append Primary qualifiers with the request//
            if (securityInfo.MacroInfo != null && securityInfo.MacroInfo[RBbgMacroType.Primary].Count > 0
                && !securityInfo.IsBulkList)
            {
                RBbgMacroInfo mInfo = null;
                instrs.macro = new Macro[securityInfo.MacroInfo[RBbgMacroType.Primary].Count];
                List<RBbgMacroInfo> lstMacroInfo = securityInfo.MacroInfo[RBbgMacroType.Primary];
                for (int m = 0; m < lstMacroInfo.Count; m++)
                {
                    mInfo = lstMacroInfo[m];
                    if (mInfo.QualifierOperator == RBbgOperatorType.None)
                    {
                        if (RBbgUtils.IsValidPrimaryQualifierAndValue(mInfo, clientName))
                        {
                            instrs.macro[m] = new Macro();
                            instrs.macro[m].primaryQualifier = new PrimaryQualifier();
                            instrs.macro[m].primaryQualifier.primaryQualifierType = (MacroType)Enum.Parse(typeof(MacroType), mInfo.QualifierType);
                            instrs.macro[m].primaryQualifier.primaryQualifierValue = mInfo.QualifierValue;
                        }
                    }
                }
            }
            #endregion
            #region Secondary Qualifier
            //Append secondary qualifiers to the primary qualifiers//
            if ((instrs.macro != null && instrs.macro.Length > 0) && (securityInfo.MacroInfo.ContainsKey(RBbgMacroType.Secondary)
                                && securityInfo.MacroInfo[RBbgMacroType.Secondary].Count > 0))
            {
                for (int k = 0; k < instrs.macro.Length; k++)
                {
                    List<RBbgMacroInfo> lstMacroInfo = securityInfo.MacroInfo[RBbgMacroType.Secondary];
                    instrs.macro[k].secondaryQualifier = new SecondaryQualifier
                                [securityInfo.MacroInfo[RBbgMacroType.Secondary].Count];
                    for (int n = 0; n < lstMacroInfo.Count; n++)
                    {
                        RBbgMacroInfo macroInfo = lstMacroInfo[n];
                        PrimaryQualifier pq = instrs.macro[k].primaryQualifier;
                        if (RBbgUtils.IsValidSecondaryPQTypeAndValue(macroInfo, pq, clientName))
                        {
                            instrs.macro[k].secondaryQualifier[n] = new SecondaryQualifier();
                            instrs.macro[k].secondaryQualifier[n].secondaryQualifierOperator = (SecondaryQualifierOperator)
                                                                                            macroInfo.QualifierOperator;
                            instrs.macro[k].secondaryQualifier[n].secondaryQualifierType = (SecondaryQualifierType)Enum.
                                                        Parse(typeof(SecondaryQualifierType), macroInfo.QualifierType);
                            instrs.macro[k].secondaryQualifier[n].secondaryQualifierValue = macroInfo.QualifierValue;
                        }
                    }
                }
            }
            #endregion
            return instrs;
        }
        /// <summary>
        /// Generates the header section.
        /// </summary>
        /// <param name="getDataHeaders">The get data headers.</param>
        /// <param name="securityInfo">The security info.</param>
        internal static void GenerateHeaderSection(ref GetDataHeaders getDataHeaders,
                                                                RBbgSecurityInfo securityInfo, ref Dictionary<string, string> dictHeaders)
        {
            HashSet<string> defaultHeadersToSkip = RBbgUtils.GetDefaultHeadersToSkip();
            if (!defaultHeadersToSkip.Contains("SECMASTER"))
            {
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "SECMASTER", "true");
                getDataHeaders.secmaster = true;
                getDataHeaders.secmasterSpecified = true;
            }

            HashSet<string> restrictedHeaders = RBbgUtils.GetRestrictedHeaders(RBbgHeaderType.Security);
            Dictionary<string, string> HeaderNamesVsValues = RVendorConfigLoader.GetVendorHeaders(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, "Security");

            if (securityInfo.HeaderNamesVsValues != null && securityInfo.HeaderNamesVsValues.Count > 0)
                foreach (var kvp in securityInfo.HeaderNamesVsValues)
                    HeaderNamesVsValues[kvp.Key] = kvp.Value;

            if (HeaderNamesVsValues != null && HeaderNamesVsValues.Count > 0)
            {
                var propInfoAll = typeof(GetDataHeaders).GetProperties();
                foreach (var propInfo in propInfoAll)
                {
                    if (!restrictedHeaders.Contains(propInfo.Name) && HeaderNamesVsValues.Keys.Contains(propInfo.Name, StringComparer.OrdinalIgnoreCase))
                    {
                        string specifiedPropertyName = string.Format("{0}Specified", propInfo.Name);
                        bool flagValueSet = false;
                        try
                        {
                            switch (propInfo.PropertyType.Name)
                            {
                                case "Boolean":
                                    string boolValue = null;
                                    if (HeaderNamesVsValues[propInfo.Name].Equals("1") || HeaderNamesVsValues[propInfo.Name].Equals("true", StringComparison.OrdinalIgnoreCase) || HeaderNamesVsValues[propInfo.Name].Equals("yes", StringComparison.OrdinalIgnoreCase) || HeaderNamesVsValues[propInfo.Name].Equals("y", StringComparison.OrdinalIgnoreCase))
                                        boolValue = "True";
                                    else if (HeaderNamesVsValues[propInfo.Name].Equals("0") || HeaderNamesVsValues[propInfo.Name].Equals("false", StringComparison.OrdinalIgnoreCase) || HeaderNamesVsValues[propInfo.Name].Equals("no", StringComparison.OrdinalIgnoreCase) || HeaderNamesVsValues[propInfo.Name].Equals("n", StringComparison.OrdinalIgnoreCase))
                                        boolValue = "False";
                                    else
                                        boolValue = HeaderNamesVsValues[propInfo.Name];
                                    propInfo.SetValue(getDataHeaders, Convert.ToBoolean(boolValue), null);
                                    break;
                                case "Int32":
                                    propInfo.SetValue(getDataHeaders, Convert.ToInt32(HeaderNamesVsValues[propInfo.Name]), null);
                                    break;
                                case "Int64":
                                    propInfo.SetValue(getDataHeaders, Convert.ToInt64(HeaderNamesVsValues[propInfo.Name]), null);
                                    break;
                                case "DateFormat":
                                case "DiffFlag":
                                case "ProgramFlag":
                                case "InstrumentType":
                                case "SpecialChar":
                                case "Version":
                                case "MarketSector":
                                case "BvalSnapshot":
                                case "PortSecDes":
                                    if (!string.IsNullOrEmpty(HeaderNamesVsValues[propInfo.Name]))
                                    {
                                        object newEnumValue = Enum.Parse(propInfo.PropertyType, Convert.ToString(HeaderNamesVsValues[propInfo.Name]), true);
                                        propInfo.SetValue(getDataHeaders, newEnumValue, null);
                                    }
                                    break;
                                default:
                                    propInfo.SetValue(getDataHeaders, HeaderNamesVsValues[propInfo.Name], null);
                                    break;
                            }
                            flagValueSet = true;
                            RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, propInfo.Name, HeaderNamesVsValues[propInfo.Name]);
                        }
                        catch (Exception ex)
                        {
                            flagValueSet = false;
                            mLogger.Error(string.Format("Failed to insert header '{0}' with value '{1}'. Error String-{2}", propInfo.Name, HeaderNamesVsValues[propInfo.Name], ex.ToString()));
                        }
                        if (flagValueSet)
                        {
                            try
                            {
                                var propSpecified = typeof(GetDataHeaders).GetProperty(specifiedPropertyName);
                                if (propSpecified != null)
                                {
                                    propSpecified.SetValue(getDataHeaders, true, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                mLogger.Error(string.Format("Failed to set '{0}' true for header '{1}'. Error String-{2}", specifiedPropertyName, propInfo.Name, ex.ToString()));
                            }
                        }
                    }
                    else if (restrictedHeaders.Contains(propInfo.Name))
                    {
                        mLogger.Error("Header " + propInfo.Name + " ignored as it is restricted");
                    }
                }
            }
        }
        /// <summary>
        /// Generates the fields.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        internal static string[] GenerateFields(RBbgSecurityInfo securityInfo)
        {
            string[] field = new string[securityInfo.InstrumentFields.Count];
            int j = 0;
            foreach (RBbgFieldInfo bbgInfo in securityInfo.InstrumentFields)
            {
                field[j] = bbgInfo.Mnemonic;
                j++;
            }
            return field;
        }
        /// <summary>
        /// Generates the instruments.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        internal static com.ivp.srm.vendorapi.bloombergServices.heavy.Instrument[] GenerateInstruments(RBbgSecurityInfo securityInfo)
        {
            //Setting Instrument information
            com.ivp.srm.vendorapi.bloombergServices.heavy.Instrument[] instruments = new com.ivp.srm.vendorapi.bloombergServices.heavy.Instrument[securityInfo.Instruments.Count];
            int i = 0;
            foreach (RBbgInstrumentInfo inst in securityInfo.Instruments)
            {
                instruments[i] = new com.ivp.srm.vendorapi.bloombergServices.heavy.Instrument();
                instruments[i].id = inst.InstrumentID;
                instruments[i].type = (InstrumentType)Enum.Parse(typeof(InstrumentType), inst.InstrumentIdType.ToString());
                instruments[i].typeSpecified = inst.InstrumentIdTypeSpecified;
                if (inst.MarketSector != RBbgMarketSector.None)
                {
                    instruments[i].yellowkey = (com.ivp.srm.vendorapi.bloombergServices.heavy.MarketSector)Enum.Parse(typeof(com.ivp.srm.vendorapi.bloombergServices.heavy.MarketSector), inst.MarketSector.ToString());
                    instruments[i].yellowkeySpecified = inst.MarketSectorSpecified;
                }
                i++;
                //Only one instrument to be processed,if request is bulk list//
                //if (securityInfo.IsBulkList)
                //    break;
            }
            return instruments;
        }
        #endregion

        #region SAPI Methods
        /// <summary>
        /// Authorizes the SAPI user.
        /// </summary>
        internal static bool AuthorizeSAPIUser(int UUID, List<string> ipAddresses, Session session, ref List<UserHandle> d_userHandles, Service d_apiAuthSvc, bool validateSession)
        {
            mLogger.Debug("Start -> authorizing serverapi user");
            bool flag = false;
            try
            {
                if (!validateSession)
                    return true;
                for (int i = 0; i < ipAddresses.Count; i++)
                {
                    try
                    {
                        d_userHandles.Clear();
                        StringBuilder message = new StringBuilder();
                        message.AppendLine("Validating SAPI request with following details:");
                        message.AppendLine("UserName=> " + UUID);
                        message.AppendLine("IP Address=> " + ipAddresses[i]);
                        AddEvent("", message.ToString(), EType.Information, EventCategory.General);
                        UserHandle userHandle = session.CreateUserHandle();
                        d_userHandles.Add(userHandle);

                        Request authRequest = d_apiAuthSvc.CreateAuthorizationRequest();

                        authRequest.Set(RVendorConstant.SAPIUUID, UUID);
                        authRequest.Set(RVendorConstant.IPADDRESS, ipAddresses[i]);

                        CorrelationID correlator = new CorrelationID(i);
                        EventQueue eventQueue = new EventQueue();
                        session.SendAuthorizationRequest(authRequest, userHandle,
                            eventQueue, correlator);

                        Event eventObj = eventQueue.NextEvent();
                        flag = IsUserAuthorize(eventObj);
                        if (flag)
                            break;
                    }
                    catch (RVendorException rEx)
                    {
                        mLogger.Error(rEx.ToString());
                    }
                }
                if (flag)
                    AddEvent("", "SAPI validation succeed", EType.Information, EventCategory.General);
                else
                    AddEvent("", "SAPI validation Failed", EType.Information, EventCategory.General);
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End -> authorizing serverapi user");
            }
            return flag;
        }

        internal static bool AuthorizeSAPIUser(int UUID, List<string> ipAddresses, Session session, ref List<Identity> lstUserHandles, Service serviceApiAuthSvc, bool validateSession)
        {
            mLogger.Debug("Start -> authorizing serverapi user");
            bool flag = false;
            try
            {
                if (!validateSession)
                    return true;
                for (int i = 0; i < ipAddresses.Count; i++)
                {
                    Identity userHandle = session.CreateIdentity();
                    lstUserHandles.Add(userHandle);

                    Request authRequest = serviceApiAuthSvc.CreateAuthorizationRequest();

                    authRequest.Set(RVendorConstant.SAPIUUID, UUID);
                    authRequest.Set(RVendorConstant.IPADDRESS, ipAddresses[i]);

                    CorrelationID correlator = new CorrelationID(i);
                    EventQueue eventQueue = new EventQueue();
                    session.SendAuthorizationRequest(authRequest, userHandle,
                        eventQueue, correlator);

                    Event eventObj = eventQueue.NextEvent();
                    flag = IsUserAuthorize(eventObj);
                    if (flag)
                        break;
                }
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End -> authorizing serverapi user");
            }
            return flag;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Determines whether [is user authorize] [the specified event obj].
        /// </summary>
        internal static bool IsUserAuthorize(Event eventObj)
        {
            bool flag = false;
            foreach (Message msg in eventObj.GetMessages())
            {
                string message = msg.MessageType.ToString();
                switch (message.ToLower().ToString())
                {
                    case "authorizationfailure":
                        flag = false;
                        break;
                    case "authorizationsuccess":
                        flag = true;
                        break;

                }
                if (flag)
                    break;
            }
            return flag;
        }
        #endregion

        #region "Break and Generate Multiple Security Requests"

        /// <summary>
        /// Each request contains 10 requests to process in a minute for lite. 
        ///
        internal static Dictionary<int, List<RBbgSecurityInfo>> GetListofRequestedSecuritiesForLite
                               (List<RBbgSecurityInfo> listRequestedSecurities)
        {
            int count = 0;
            int counter = 0;
            List<RBbgSecurityInfo> tempList = new List<RBbgSecurityInfo>();
            Dictionary<int, List<RBbgSecurityInfo>> dictRequest = new Dictionary<int, List<RBbgSecurityInfo>>();
            for (int i = 0; i < listRequestedSecurities.Count; i++)
            {
                if (i >= count && i <= count + 9)//Only Ten request process in a minute.
                {
                    tempList = new List<RBbgSecurityInfo>();
                    tempList.Add(listRequestedSecurities[i]);
                    if (!dictRequest.ContainsKey(counter))
                        dictRequest.Add(counter, tempList);
                    else
                        dictRequest[counter].AddRange(tempList);

                }
                else
                {
                    count = --i + 1;
                    counter++;
                }
            }
            return dictRequest;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the security requests.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        internal static List<RBbgSecurityInfo> GetSecurityRequests(RBbgSecurityInfo bbgSecurityInfo)
        {
            bool invalidFieldCount = false;
            bool invalidInstrumentCount = false;
            RBbgSecurityInfo secInfo = null;
            List<RBbgInstrumentInfo> instrumentInfo = null;
            List<RBbgSecurityInfo> tempSecurities = new List<RBbgSecurityInfo>();
            List<RBbgSecurityInfo> securities = new List<RBbgSecurityInfo>();
            if (bbgSecurityInfo.Instruments != null && bbgSecurityInfo.Instruments.Count != 0)
            {
                var instrumentInfoTemp = new Dictionary<string, RBbgInstrumentInfo>(StringComparer.OrdinalIgnoreCase);
                foreach (RBbgInstrumentInfo instrument in bbgSecurityInfo.Instruments)
                {
                    if (!string.IsNullOrEmpty(instrument.InstrumentID.Trim()) && !instrumentInfoTemp.ContainsKey(instrument.InstrumentID))
                        instrumentInfoTemp.Add(instrument.InstrumentID, instrument);
                }

                bbgSecurityInfo.Instruments = instrumentInfoTemp.Values.ToList();
                //bbgSecurityInfo = RemoveDuplicateInstruments(bbgSecurityInfo);
                ////check for empty Instrument ID.
                //RemoveEmptyInstrument(bbgSecurityInfo);
            }
            else
                if (bbgSecurityInfo.MacroInfo.Count == 0)
                throw new RVendorException("No instruments/Macros to process.");

            bbgSecurityInfo = RemoveDuplicateFields(bbgSecurityInfo);
            //if (!bbgSecurityInfo.InstrumentFields.Exists(q => q.Mnemonic.Equals("SECURITY_TYP", StringComparison.OrdinalIgnoreCase)) && !bbgSecurityInfo.IsBulkList)
            //    bbgSecurityInfo.InstrumentFields.Add(new RBbgFieldInfo() { Mnemonic = "SECURITY_TYP", IsSystemAdded = true });
            //if (!bbgSecurityInfo.InstrumentFields.Exists(q => q.Mnemonic.Equals("SECURITY_TYP2", StringComparison.OrdinalIgnoreCase)) && !bbgSecurityInfo.IsBulkList)
            //    bbgSecurityInfo.InstrumentFields.Add(new RBbgFieldInfo() { Mnemonic = "SECURITY_TYP2", IsSystemAdded = true });
            //if (!bbgSecurityInfo.InstrumentFields.Exists(q => q.Mnemonic.Equals("MARKET_SECTOR_DES", StringComparison.OrdinalIgnoreCase)) && !bbgSecurityInfo.IsBulkList)
            //    bbgSecurityInfo.InstrumentFields.Add(new RBbgFieldInfo() { Mnemonic = "MARKET_SECTOR_DES", IsSystemAdded = true });
            bool isSystemAddedDlAssetClass = false;
            if (!bbgSecurityInfo.InstrumentFields.Exists(q => q.Mnemonic.Equals("DL_Asset_Class", StringComparison.OrdinalIgnoreCase)) && !bbgSecurityInfo.IsBulkList)
            {
                bbgSecurityInfo.InstrumentFields.Add(new RBbgFieldInfo() { Mnemonic = "DL_Asset_Class", IsSystemAdded = true });
                isSystemAddedDlAssetClass = true;
            }
            bool isSystemAddedIDBBCompany = false;
            if (!bbgSecurityInfo.InstrumentFields.Exists(q => q.Mnemonic.Equals("ID_BB_COMPANY", StringComparison.OrdinalIgnoreCase)) && bbgSecurityInfo.IsGetCompany)
            {
                bbgSecurityInfo.InstrumentFields.Add(new RBbgFieldInfo() { Mnemonic = "ID_BB_COMPANY", IsSystemAdded = true });
                isSystemAddedIDBBCompany = true;
            }
            #region Apply Market Sector And Instrument ID Type
            foreach (RBbgInstrumentInfo instrInfo in bbgSecurityInfo.Instruments)
            {
                if (instrInfo.InstrumentIdType == RBbgInstrumentIdType.NONE)
                {
                    //    if (bbgSecurityInfo.InstrumentIdentifierType == RBbgInstrumentIdType.NONE)
                    //        instrInfo.InstrumentIdType = RBbgInstrumentIdType.TICKER;
                    //    else
                    instrInfo.InstrumentIdType = bbgSecurityInfo.InstrumentIdentifierType;
                }
                //if (instrInfo.MarketSector == RBbgMarketSector.None)
                //{
                //    if (bbgSecurityInfo.MarketSector == RBbgMarketSector.None && !bbgSecurityInfo.IsMarketSectorAppended)
                //        instrInfo.MarketSector = RBbgMarketSector.Equity;
                //    else
                //        instrInfo.MarketSector = bbgSecurityInfo.MarketSector;
                //}
            }
            #endregion

            #region Handle Multiple Instrument ID Type
            if (bbgSecurityInfo.RequestType == RBbgRequestType.FTP)
                tempSecurities = HandleInputSecurityInfo(bbgSecurityInfo);
            else
                tempSecurities.Add(bbgSecurityInfo);
            #endregion


            #region Handle Multiple Bulk List Map ID
            if (bbgSecurityInfo.IsBulkList)
                tempSecurities = HandleMultipleBulkListIds(bbgSecurityInfo);
            #endregion

            foreach (RBbgSecurityInfo securityInfo in tempSecurities)
            {

                #region "Validate Lengths"
                int maxFields = 0;
                int maxInstruments = 0;
                GetValidLength(securityInfo.RequestType, securityInfo, ref maxFields, ref maxInstruments);
                if (securityInfo.InstrumentFields.Count > maxFields)
                    invalidFieldCount = true;
                if (securityInfo.Instruments.Count > maxInstruments)
                    invalidInstrumentCount = true;
                #endregion

                var lstLstFieldInfo = new List<List<RBbgFieldInfo>>();
                if (invalidFieldCount)
                {
                    List<string> lstFieldsToIgnore = new List<string>() { };

                    if (bbgSecurityInfo.IsGetCompany)
                        lstFieldsToIgnore.Add("ID_BB_COMPANY");

                    if (!bbgSecurityInfo.IsBulkList)
                        lstFieldsToIgnore.Add("DL_Asset_Class");

                    List<RBbgFieldInfo> lstFieldInfo = new List<RBbgFieldInfo>();
                    var maxLimit = (!bbgSecurityInfo.IsBulkList) ? maxFields - 1 : maxFields;
                    maxLimit = (bbgSecurityInfo.IsGetCompany) ? maxLimit - 1 : maxLimit;
                    foreach (var fieldInfo in securityInfo.InstrumentFields.Where(q => !lstFieldsToIgnore.Contains(q.Mnemonic, StringComparer.OrdinalIgnoreCase)))
                    {
                        if (lstFieldInfo.Count < maxLimit)
                        {
                            lstFieldInfo.Add(fieldInfo);
                        }
                        else
                        {
                            if (bbgSecurityInfo.IsGetCompany)
                                lstFieldInfo.Add(new RBbgFieldInfo() { Mnemonic = "ID_BB_COMPANY", IsSystemAdded = isSystemAddedIDBBCompany });

                            if (!bbgSecurityInfo.IsBulkList)
                                lstFieldInfo.Add(new RBbgFieldInfo() { Mnemonic = "DL_Asset_Class", IsSystemAdded = isSystemAddedDlAssetClass });
                            lstLstFieldInfo.Add(lstFieldInfo);
                            lstFieldInfo = new List<RBbgFieldInfo>();
                            lstFieldInfo.Add(fieldInfo);
                        }
                    }

                    if (lstFieldInfo.Count > 0)
                    {
                        if (bbgSecurityInfo.IsGetCompany)
                            lstFieldInfo.Add(new RBbgFieldInfo() { Mnemonic = "ID_BB_COMPANY", IsSystemAdded = isSystemAddedIDBBCompany });

                        if (!bbgSecurityInfo.IsBulkList)
                            lstFieldInfo.Add(new RBbgFieldInfo() { Mnemonic = "DL_Asset_Class", IsSystemAdded = isSystemAddedDlAssetClass });
                        lstLstFieldInfo.Add(lstFieldInfo);
                        lstFieldInfo = new List<RBbgFieldInfo>();
                    }
                }

                int maxInstrumentLoops = GetMaxLoops(securityInfo.Instruments.Count, maxInstruments);
                int maxFieldLoops = GetMaxLoops(securityInfo.InstrumentFields.Count, maxFields);

                if (!invalidFieldCount && !invalidInstrumentCount)
                    securities.Add(securityInfo);
                else if (invalidInstrumentCount && !invalidFieldCount)
                {
                    for (int i = 0; i < maxInstrumentLoops; i++)
                    {
                        instrumentInfo = securityInfo.Instruments.GetRange
                            (i * maxInstruments, GetItemCount(securityInfo.Instruments.Count,
                                maxInstruments, i));

                        secInfo = GetSecurityInfo(securityInfo);

                        secInfo.InstrumentFields = securityInfo.InstrumentFields;
                        secInfo.Instruments = instrumentInfo;

                        securities.Add(secInfo);
                    }
                }
                else if (!invalidInstrumentCount && invalidFieldCount)
                {
                    for (int i = 0; i < lstLstFieldInfo.Count; i++)
                    {
                        var lstFieldInfo = lstLstFieldInfo[i];

                        secInfo = GetSecurityInfo(securityInfo);

                        secInfo.Instruments = securityInfo.Instruments;
                        secInfo.InstrumentFields = lstFieldInfo;

                        securities.Add(secInfo);
                    }
                }
                else
                {
                    for (int i = 0; i < maxInstrumentLoops; i++)
                    {
                        instrumentInfo = securityInfo.Instruments.GetRange
                            (i * maxInstruments, GetItemCount(securityInfo.Instruments.Count,
                                maxInstruments, i));

                        for (int j = 0; j < lstLstFieldInfo.Count; j++)
                        {
                            var lstFieldInfo = lstLstFieldInfo[j];

                            secInfo = GetSecurityInfo(securityInfo);

                            secInfo.Instruments = instrumentInfo;
                            secInfo.InstrumentFields = lstFieldInfo;
                            securities.Add(secInfo);
                        }
                    }
                }
            }
            //securities[0].InstrumentFields = GetFieldInfo1(isSystemAddedDlAssetClass);
            //if (securities.Count > 1)
            //    securities[1].InstrumentFields = GetFieldInfo2(isSystemAddedDlAssetClass);
            return securities;
        }

        public static List<RBbgFieldInfo> GetFieldInfo1(bool isSystemAddedDlAssetClass)
        {
            var lstFieldInfo = new List<RBbgFieldInfo>();
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_FAIR_VAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INFLATION_ASSUMPTION" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CUR_FACTOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FAIR_VAL_UPPER_BOUND" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ADR_FX_UNDL_ADR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ADR_FX_ADR_UNDL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ASK_WORKOUT_DT_DAYS_TDY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ASK_WORKOUT_DT_YEARS_TDY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CAPITAL_GAIN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DAYS_TO_ASK_WORKOUT_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_PX_VAL_BP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_CUSIP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INT_ACC" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CASH_FLOW" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CDR_3M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_END_PRINC_WNDW_CALL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_END_PRINC_WNDW_MTY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_ABS_12M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_ABS_1M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_ABS_3M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_ABS_6M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_ABS_ISS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_CPR_12M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_CPR_1M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_CPR_3M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_CPR_6M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_CPR_ISS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_PSA_12M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_PSA_1M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_PSA_3M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_PSA_6M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PL_PSA_ISS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PREPAY_SPEED" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PREPAY_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_SEV_3M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_START_PRINC_WNDW_CALL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_START_PRINC_WNDW_MTY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OID_COUPON_INCOME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ORDINARY_INCOME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "TRADE_DT_ACC_INT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WORKOUT_DT_ASK" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WORKOUT_DT_BID" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WORKOUT_DT_MID" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WORKOUT_DT_YEARS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WORKOUT_DT_YEARS_MID" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WORKOUT_DT_YEARS_TDY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "YAS_WORKOUT_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "YRS_TO_ASK_WORKOUT_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "EXCH_CODE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_BB_GLOBAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_BB_SEC_NUM_DES" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_BB_SECURITY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_BB_UNIQUE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MARKET_SECTOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MARKET_SECTOR_DES" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SECURITY_DES" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SECURITY_NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SECURITY_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SECURITY_TYP2" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "TICKER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COMPOSITE_ID_BB_GLOBAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_POSITION_LIMIT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_CONT_VAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "BHIS_CLOSE_ON_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CONTRACT_VALUE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PX_ASK" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PX_BID" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PX_LAST" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "YLD_YTM_BID" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DUR_ADJ_OAS_MID" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_BMED" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_BMED_D100" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_BMED_D200" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_BMED_D300" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_BMED_D50" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_BMED_U100" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_BMED_U200" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_BMED_U300" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_BMED_U50" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FFIEC_TEST" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OAS_OPT_VAL_BID" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "UNDL_CONTRACT_VALUE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "VOLATILITY_260D" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_SEDOL1" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CRNCY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_CUSIP_ID_NUM" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_CUSIP_REAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_ISIN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "TRADE_CRNCY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_HIST_CPN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_HIST_FACT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "144A_FLAG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "AMT_ISSUED" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "AMT_OUTSTANDING" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ANNOUNCE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ARM_INITIAL_FIXED_PERIOD_MONTHS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "BANKRUPT_PCT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "BASE_ACC_RT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "BASE_ACC_RT_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "BASE_CPI" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "BASIC_SPREAD" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "BB_COMPOSITE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "BOND_TO_EQY_TICKER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "BOOK_ENTRY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALC_MATURITY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALC_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALC_TYP_DES" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALL_DAYS_NOTICE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALL_DISCRETE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALL_FEATURE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALL_FREQ" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALL_NOTIFICATION_MIN_DAYS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALL_SCHEDULE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALL_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALLABLE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALLED_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALLED_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CAPSEC_INDICATOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CASH_SETTLED" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CDS_DEBT_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CMBS_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CNTRY_ISSUE_ISO" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CNTRY_OF_DOMICILE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COLLAT_ARM_INDEX_1" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COLLAT_ARM_LOAN_RST_DT_1" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COLLAT_ARM_PERIODIC_CAP_ALL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COLLAT_ARM_WA_CAP_ALL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COLLAT_ARM_WA_FLOOR_ALL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COLLAT_ARM_WA_GROSS_MARGIN_1" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COLLAT_ARM_WA_NET_MARGIN_1" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COLLAT_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COMPLETELY_CALLED" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COMPOSITE_EFF_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CONVERTIBLE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COUNTRY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "COUPON_TYPE_RESET_DATE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CPN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CPN_ASOF_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CPN_CAP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CPN_CRNCY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CPN_FLOOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CPN_FREQ" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CPN_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CPN_TYP_SPECIFIC" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CREDIT_ENHANCEMENTS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CUR_CPN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CURR_CREDIT_SUPPORT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DAY_CNT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DAY_CNT_DES" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DAYS_TO_NXT_REFIX_TDY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DAYS_TO_SETTLE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DBRS_EFF_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DEFAULTED" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DELIVERY_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DES_NOTES" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DTC_ELIGIBLE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "EQY_DVD_YLD_12M" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "EQY_SIC_CODE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "EQY_SIC_NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FED_WIRE_BOOK_ENTRY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FINAL_MATURITY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FIRST_CALL_DT_ISSUANCE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FIRST_CPN_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FIRST_SETTLE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FITCH_EFF_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FLOATER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FLT_MAX_CPN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FLT_PAY_DAY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FLT_SPREAD" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_ACT_DAYS_EXP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_CTD" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_CTD_CUSIP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_DLV_DT_FIRST" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_DLV_DT_LAST" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_DLVRBLE_BNDS_ISINS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_EXCH_NAME_LONG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_EXCH_NAME_SHRT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_FIRST_TRADE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_INIT_HEDGE_ML" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_INIT_SPEC_ML" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_LONG_NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_MONTH_YR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_NOMINAL_CONTRACT_VALUE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_NOTICE_FIRST" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_OPT_TRADING" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_SEC_HEDGE_ML" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_SEC_SPEC_ML" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_TICK_SIZE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_TICK_VAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_TRADING_UNITS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_VAL_PT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUTURES_CATEGORY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "GUARANTOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_BB_COMPANY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ID_BB_PARENT_CO" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "IDX_RATIO" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INDUSTRY_GROUP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INDUSTRY_GROUP_NUM" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INDUSTRY_SECTOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INDUSTRY_SECTOR_NUM" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INDUSTRY_SUBGROUP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INDUSTRY_SUBGROUP_NUM" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INFLATION_LINKED_INDICATOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INT_ACC_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "INT_RATE_FUT_START_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "IS_EVER_TRACE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "IS_FIX_TO_FLOAT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "IS_FLOAT_TO_FIX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "IS_PERPETUAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "IS_TRACE_ELIGIBLE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ISO_COUNTRY_GUARANTOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ISSUE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ISSUE_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ISSUER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ISSUER_INDUSTRY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "LAST_REFIX_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "LAST_TRADEABLE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "LEAD_MGR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "LONG_COMP_NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MARKET_ISSUE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MATURITY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MIN_DENOMINATION" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MIN_INCREMENT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MIN_PIECE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MOST_RECENT_REPORTED_FACTOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_2_4_FAMILY_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_ACC_RT_START_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_AMORT_WAM" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_AOLS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_ARM_SUBTYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_BALLOON_WAM" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CALL_PCT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CARD_MPR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CMO_CLASS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CMO_GROUP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CMO_SERIES" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_COLLAT_GROUP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_COLLAT_SEASONING" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_COLLAT_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_COMPLIANCE_CODE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CONDOMINIUM_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CONV" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CREDIT_SCORE_WAVG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_CUR_PAY_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_DEAL_CUR_NET" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_DEAL_NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_DEAL_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_DELQ_60PLUS_CUR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_DELQ_90PLUS_CUR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_EST_ACC_RT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FACE_AMT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FACTOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FACTOR_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FACTOR_NUM_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FACTOR_SET_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FIRST_PAY_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FIRST_RESET_CAP_DOWN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FIRST_RESET_CAP_UP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FIRST_RST_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_FLT_NXT_RST" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_GEN_TICKER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_GEO" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_IS_AGENCY_BACKED" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_LIFE_CAP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_LIFE_FLOOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_LOAN_AGE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_LOAN_MRGN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_LOAN_PURPOSE_EQUITY_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_LOAN_PURPOSE_PURCHASE_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_LOAN_PURPOSE_REFINANCE_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_LOAN_SIZE_WAVG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_LOOKBACK" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_LTV_WAVG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_OCCUPANCY_INVESTMENT_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_OCCUPANCY_OWNER_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_OCCUPANCY_VACATION_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_ORIG_AMT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_ORIG_SERV_LINE1" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_ORIG_WAC" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_ORIG_WAM" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PAC_COLLARS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PAY_DELAY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PAY_RT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PER_RT_CAP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_POOL_ALPHA" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_POOL_NUMBER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_POOL_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PREV_CPN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PREV_FACTOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PREV_FACTOR_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PREV_FACTOR_NUM_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_PUD_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_RECORD_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_REM_IO_PERIOD" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_REMIC_CUSIP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_RT_CHG_FREQ" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_SERV" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_SINGLE_FAMILY_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_START_ACC_DT_SET_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_STATED_WALA" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_STATED_WARM" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_TRANCHE_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_TRANCHE_TYP_LONG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WA_ORIG_LOAN_SIZE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAC_ARM_CAP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAC_ARM_CPN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAC_ARM_MRGN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAC_ARM_PCT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAC_CALC" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAC_WAVG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WACPN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WALA_CALC" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAM" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAM_NXT_RST" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAM_SD" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAM_WAVG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAOCS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAOLT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WAOLTV" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_30DLQ" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_60DLQ" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_90DLQ" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_CRED_SUP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_FCLS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_GEO1" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_GEO2" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_GEO3" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_GEO4" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_LIM_DOC" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_LTV" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_NUMBER_LOAN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_REO" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_WHLN_WALTV" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTY_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MULTI_CPN_SCHEDULE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_ADV_RFND_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_ADV_RFND_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_ADV_RFND_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_BANK_QUAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_DATED_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_ESCROW_MTY_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_FED_TAX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_FORM" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_ISSUE_SIZE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_ISSUE_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_ISSUER_DES_2ND_LINE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_LONG_INDUSTRY_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_MIN_TAX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_MTY_SIZE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_OFFERING_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_OPT_CALL_TIMING_IND" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_ORIG_CALL_SCHEDULE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_PAYING_AGENT_CITY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_PURPOSE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_RECENT_REDEMP_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_RECENT_REDEMP_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_REFUND_STAT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_REVISED_ISSUE_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_SOURCE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_STATE_TAX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_TAX_PROV" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NOMINAL_PAYMENT_DAY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NXT_CALL_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NXT_CALL_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NXT_CPN_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NXT_FACTOR_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NXT_PUT_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NXT_REFIX_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NXT_REFUND_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OID_BOND" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_CONT_SIZE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_CONT_SIZE_REAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_EXER_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_EXPIRE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_FIRST_TRADE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_PUT_CALL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_STRIKE_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_TICK_VAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_UNDL_EXP_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_UNDL_TICKER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ORIG_CREDIT_SUPPORT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PAR_AMT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PARENT_COMP_NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PENULTIMATE_CPN_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PFD_DVD_PAY_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PREV_CPN_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PRINCIPAL_FACTOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PRPL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PUT_DAYS_NOTICE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PUT_DISCRETE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PUT_NOTIFICATION_MIN_DAYS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PUT_SCHEDULE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PUTABLE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "REDEMP_VAL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "REFIX_FREQ" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "REFIX_FREQ_FEATURE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RESET_IDX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_DBRS" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_DBRS_ST" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_DBRS_ST_DEBT_RTG_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_DBRS_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH_UNDL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SALE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SECOND_CALL_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SECOND_CALL_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SECOND_CPN_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SECURITY_FACTORABLE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SERIAL_MONTH_1" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SERIES" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SETTLE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SHORT_NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SINK_SCHEDULE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SINKABLE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SINKING_FUND_FACTOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "STATE_CODE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "STEPUP_CPN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "STEPUP_CPN_SCHEDULE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "STEPUP_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "STRUCTURED_NOTE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "STRUCTURED_NOTE_CPN_FORMULA" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "STRUCTURED_NOTE_RANGE_FORMULA" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "TRACE_SOURCE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "TRUST_PREFERRED_INDICATOR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ULTIMATE_BORROWER_NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "UNDERLYING_CUSIP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "UNDL_ID_BB_UNIQUE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "XO_REDEMP_FLAG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "YIELD_ON_ISSUE_DATE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ZERO_CPN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_UNDL_ISIN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_UNDL_CUSIP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_TICKER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_VAR_INIT_CPN_RT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_VAL_PT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_UNDL_TICKER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "REFERENCE_INDEX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_SETTLE_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_SH_PER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SEDOL1_COUNTRY_ISO" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_OUTLOOK" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_NO_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_ISSUER_EFF_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_ISSUER_RATING" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PUT_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_PUT_OR_CALL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PUT_FREQ" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PUT_FEATURE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PAYMENT_RANK" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_TICK_SIZE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NXT_SINK_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NXT_SINK_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NXT_PUT_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_PURPOSE_3" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_PURPOSE_2" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MULTI_CPN_SCHEDULE_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_UNDL_OUTLOOK" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MOODY_EFF_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MOODY_NO_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_OUTLOOK" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_LT_ENHANCED" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_ENHANCED_OUTLOOK" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH_UNDERLYING_OUTLOOK" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH_NO_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH_OUTLOOK" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_EXPIRE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_EXER_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "WRT_EXER_PX" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_ADJ_CONTRACT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_ADJ_CPN_MODE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "IS_STK_MARGINABLE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CV_UNTIL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CV_START_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CV_COMMON_TICKER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ADR_UNDL_TICKER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ADR_UNDL_CRNCY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_GEO4_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_GEO3_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_GEO2_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MTG_GEO1_CURR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_ST_UNDL_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_ST_UNDL_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_ST_UNDL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH_SHRT_RATING_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH_LONG_RATING_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_SHRT_RATING_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DL_Asset_Class" });

            return lstFieldInfo;
        }

        public static List<RBbgFieldInfo> GetFieldInfo2(bool isSystemAddedDlAssetClass)
        {
            var lstFieldInfo = new List<RBbgFieldInfo>();
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_SHRT_RATING_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_LT_MUNI_UNDERLYING_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_LONG_RATING_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_LT_UNDL_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_LONG_RATING_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH_UNDL_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_UNDL_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_LT_UNDL_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_UNDERLYING" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_LT_UNDL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_LONG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_SHRT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MOODY_SHRT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MOODY_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_SP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MOODY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PRE_RFND_TYP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH_SHRT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_FITCH_LONG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_DBRS_ST_DEBT_RTG_WATCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ADR_SH_PER_ADR" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ADR_ADR_PER_SH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "LEAP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "LETTER_OF_CREDIT_FLAG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "UNDERLYING_ISIN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_ISSUER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MOODY_LONG_ISSUE_LEVEL" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CV_CNVS_RATIO" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_UNDL_ISIN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_UNDL_CUSIP" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "FUT_LAST_TRADE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MDY_ISSUER_RATING" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "EQY_PRIM_EXCH" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SETTLE_DATE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "RTG_MOODY_LONG" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ACC_RT_START_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "IS_CAPSEC_OR_TRUST_PFD" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "MUNI_PRE_RFND_MTY_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "SP_EFF_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "CALLED" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PRIMARY_EXCHANGE_NAME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PX_CLOSE_1D" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "PX_DIRTY_CLEAN" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "LOCAL_TIME" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "LAST_UPDATE_DT" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "ADR_CRNCY" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "NY_TIME_OF_LAST_PRICE_UPDATE" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "OPT_MULTIPLIER" });
            lstFieldInfo.Add(new RBbgFieldInfo { Mnemonic = "DL_Asset_Class" });

            return lstFieldInfo;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Removes the duplicate fields.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        internal static RBbgSecurityInfo RemoveDuplicateFields(RBbgSecurityInfo securityInfo)
        {
            List<string> lstStr = null;
            List<RBbgFieldInfo> fldInfo = null;
            mLogger.Debug("Start -> validating security info.");
            try
            {
                lstStr = new List<string>();
                fldInfo = new List<RBbgFieldInfo>();
                if (securityInfo.InstrumentFields != null)
                {
                    foreach (RBbgFieldInfo info in securityInfo.InstrumentFields)
                    {
                        if (!lstStr.Contains(info.Mnemonic.ToLower()))
                        {
                            lstStr.Add(info.Mnemonic.ToLower());
                            fldInfo.Add(info);
                        }
                    }
                }
                securityInfo.InstrumentFields = fldInfo;
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End -> validating security info.");
                if (lstStr != null)
                    lstStr = null;
                if (fldInfo != null)
                    fldInfo = null;
            }
            return securityInfo;
        }

        //------------------------------------------------------------------------------------------
        public static void RemoveEmptyInstrument(RBbgSecurityInfo securityInfo)
        {
            securityInfo.Instruments.RemoveAll(instruments =>
            {
                return string.IsNullOrEmpty(instruments.InstrumentID.Trim());
            });
            if (securityInfo.Instruments.Count == 0)
                throw new RVendorException("No instruments to be processed");
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Removes the duplicate instruments.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        private static RBbgSecurityInfo RemoveDuplicateInstruments(RBbgSecurityInfo securityInfo)
        {
            List<string> lstStr = null;
            List<RBbgInstrumentInfo> instrumentInfo = null;
            mLogger.Debug("Start -> Remove duplicate instruments");
            try
            {
                lstStr = new List<string>();
                instrumentInfo = new List<RBbgInstrumentInfo>();
                foreach (RBbgInstrumentInfo instrument in securityInfo.Instruments)
                {
                    if (!lstStr.Contains(instrument.InstrumentID.ToLower()))
                    {
                        lstStr.Add(instrument.InstrumentID.ToLower());
                        instrumentInfo.Add(instrument);
                    }
                }
                securityInfo.Instruments = instrumentInfo;
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                if (lstStr != null)
                    lstStr = null;
                if (instrumentInfo != null)
                    instrumentInfo = null;
            }
            mLogger.Debug("End -> Remove duplicate instruments");
            return securityInfo;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the security info.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        private static RBbgSecurityInfo GetSecurityInfo(RBbgSecurityInfo securityInfo)
        {
            RBbgSecurityInfo secInfo = new RBbgSecurityInfo();
            secInfo.RequestType = securityInfo.RequestType;
            secInfo.TransportName = securityInfo.TransportName;
            secInfo.UserLoginName = securityInfo.UserLoginName;
            secInfo.IsBulkList = securityInfo.IsBulkList;
            secInfo.MacroInfo = securityInfo.MacroInfo;
            secInfo.ModuleId = securityInfo.ModuleId;
            secInfo.requireNotAvailableInField = securityInfo.requireNotAvailableInField;
            secInfo.VendorPreferenceId = securityInfo.VendorPreferenceId;
            secInfo.BulkListMapId = securityInfo.BulkListMapId;
            secInfo.HeaderNamesVsValues = securityInfo.HeaderNamesVsValues;
            secInfo.ImmediateRequest = securityInfo.ImmediateRequest;
            secInfo.InstrumentIdentifierType = securityInfo.InstrumentIdentifierType;
            secInfo.IPAddresses = securityInfo.IPAddresses;
            secInfo.SAPIUserName = securityInfo.SAPIUserName;
            secInfo.MarketSector = securityInfo.MarketSector;
            secInfo.IsBVAL = securityInfo.IsBVAL;
            secInfo.IsBVALPricingSource = securityInfo.IsBVALPricingSource;
            secInfo.IsCombinedFtpReq = securityInfo.IsCombinedFtpReq;
            secInfo.IsGetCompany = securityInfo.IsGetCompany;
            secInfo.IsMarketSectorAppended = securityInfo.IsMarketSectorAppended;
            secInfo.Overrides = securityInfo.Overrides.ToDictionary(x => x.Key, y => y.Value, StringComparer.OrdinalIgnoreCase);
            secInfo.RequestIdentifier = securityInfo.RequestIdentifier;
            secInfo.SecurityId = securityInfo.SecurityId;
            return secInfo;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <param name="itemCount">The item count.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static int GetItemCount(int itemCount, int maxValue, int index)
        {
            int usedItemCount = itemCount - (maxValue * index);
            int count = 0;
            if (usedItemCount > maxValue)
            {
                count = maxValue;
            }
            else
            {
                count = usedItemCount;
            }
            return count;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the max loops.
        /// </summary>
        /// <param name="itemCount">The item count.</param>
        /// <param name="maxValue">The max value.</param>
        /// <returns></returns>
        private static int GetMaxLoops(int itemCount, int maxValue)
        {
            int quotient = itemCount / maxValue;
            int remainder = itemCount % maxValue;
            if (remainder > 0)
                return quotient + 1;
            else
                return quotient;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the request field for bulk list.
        /// </summary>
        internal static List<RBbgFieldInfo> GetRequestFieldForBulkList(RBbgBulkListInfo bulkListInfo)
        {
            mLogger.Debug("Start -> getting request field.");
            List<RBbgFieldInfo> lstField = null;
            RBbgFieldInfo info = null;
            try
            {
                lstField = new List<RBbgFieldInfo>();
                info = new RBbgFieldInfo();
                info.Mnemonic = bulkListInfo.RequestField;
                lstField.Add(info);
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End -> getting request field");
            }
            return lstField;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the length of the valid.
        /// </summary>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="maxFields">The max fields.</param>
        /// <param name="maxInstruments">The max instruments.</param>
        internal static void GetValidLength(RBbgRequestType requestType, RBbgSecurityInfo securityInfo,
            ref int maxFields,
                ref int maxInstruments)
        {
            switch (requestType)
            {
                case RBbgRequestType.Lite:
                    maxFields = int.Parse(RVendorConfigLoader.GetVendorConfig
                        (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MAXFIELDSLITE));
                    maxInstruments = int.Parse(RVendorConfigLoader.GetVendorConfig
                        (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MAXSECLITE));
                    break;
                case RBbgRequestType.Heavy:
                    maxFields = int.Parse(RVendorConfigLoader.GetVendorConfig
                        (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MAXFIELDSHEAVY));
                    maxInstruments = int.Parse(RVendorConfigLoader.GetVendorConfig
                        (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MAXSECHEAVY));
                    break;
                case RBbgRequestType.FTP:
                    maxFields = int.Parse(RVendorConfigLoader.GetVendorConfig
                        (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MAXFIELDSFTP));
                    maxInstruments = int.Parse(RVendorConfigLoader.GetVendorConfig
                        (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MAXSECFTP));
                    break;
                case RBbgRequestType.MANAGEDBPIPE:
                case RBbgRequestType.GlobalAPI:
                case RBbgRequestType.SAPI:
                    maxFields = int.Parse(RVendorConfigLoader.GetVendorConfig
                        (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MAXFIELDSSERVER));
                    maxInstruments = int.Parse(RVendorConfigLoader.GetVendorConfig
                        (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MAXSECSERVER));
                    break;
            }
        }
        #endregion

        #region "Write Vendor History"
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Writes the vendor history.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="requestId">The request id.</param>
        internal static void WriteVendorHistory(RBbgSecurityInfo securityInfo, string vendorName,
                                                                string identifier, string timestamp, out int vendorPricingID)
        {
            RVendorHistoryInfo historyInfo = new RVendorHistoryInfo();
            historyInfo.requestIdentifier = identifier;
            historyInfo.RequestStatus = RVendorStatusConstant.PENDING;
            historyInfo.TimeStamp = timestamp;
            historyInfo.UserLoginName = securityInfo.UserLoginName;
            if (securityInfo.IsBulkList)
                historyInfo.VendorFields = GetCSVFields(securityInfo.InstrumentFields) +
                    "," + securityInfo.BulkListMapId[0];
            else
                historyInfo.VendorFields = GetCSVFields(securityInfo.InstrumentFields);
            if (securityInfo.MacroInfo != null &&
                (securityInfo.MacroInfo.ContainsKey(RBbgMacroType.Primary) &&
                securityInfo.MacroInfo[RBbgMacroType.Primary].Count > 0))
            {
                StringBuilder reqInstr = new StringBuilder();
                reqInstr.Append(",");
                reqInstr.Append("PrimaryMacro");
                if (securityInfo.MacroInfo.ContainsKey(RBbgMacroType.Secondary)
                    && securityInfo.MacroInfo[RBbgMacroType.Secondary].Count > 0)
                    reqInstr.Append(",SecondaryMacro");
                reqInstr.Remove(0, 1);
                historyInfo.VendorInstruments = reqInstr.ToString();
            }
            else
                historyInfo.VendorInstruments = GetCSVInstruments(securityInfo.Instruments);
            historyInfo.RequestType = securityInfo.RequestType.ToString();
            historyInfo.VendorName = vendorName;
            historyInfo.IsBulkList = securityInfo.IsBulkList;
            historyInfo.ModuleId = securityInfo.ModuleId;
            vendorPricingID = RVendorUtils.InsertHistory(historyInfo);

            if (RVendorUtils.EnableBBGAudit)
            {
                mLogger.Debug("WriteVendorHistory:InsertBBGAudit -> Start");
                try
                {
                    var dtSecAuditInfo = new DataTable();
                    dtSecAuditInfo.Columns.Add("identifier_value", typeof(string));
                    dtSecAuditInfo.Columns.Add("identifier_name", typeof(string));
                    dtSecAuditInfo.Columns.Add("yellow_key_name", typeof(string));

                    var dtMnemonicAuditInfo = new DataTable();
                    dtMnemonicAuditInfo.Columns.Add("mnemonic_name", typeof(string));

                    List<string> marketSectors = Enum.GetNames(typeof(RBbgMarketSector)).Select(x => x.ToLower()).ToList();
                    foreach (var instrumentInfo in securityInfo.Instruments)
                    {
                        string marketSector = string.Empty;
                        string identifierValue = string.Empty;
                        if (instrumentInfo.MarketSector == RBbgMarketSector.None)
                        {
                            GetMarketSectorFromInstrument(instrumentInfo.InstrumentID, marketSectors, out identifierValue, out marketSector);
                        }
                        else
                        {
                            marketSector = instrumentInfo.MarketSector.ToString();
                            identifierValue = instrumentInfo.InstrumentID;
                        }

                        var drSecAudit = dtSecAuditInfo.NewRow();
                        drSecAudit["identifier_value"] = identifierValue;
                        drSecAudit["identifier_name"] = instrumentInfo.InstrumentIdType == RBbgInstrumentIdType.NONE ? string.Empty : instrumentInfo.InstrumentIdType.ToString();
                        drSecAudit["yellow_key_name"] = marketSector;

                        dtSecAuditInfo.Rows.Add(drSecAudit);
                    }

                    foreach (var fieldInfo in securityInfo.InstrumentFields)
                    {
                        var drMnemonic = dtMnemonicAuditInfo.NewRow();
                        drMnemonic["mnemonic_name"] = fieldInfo.Mnemonic;

                        dtMnemonicAuditInfo.Rows.Add(drMnemonic);
                    }

                    if (dtSecAuditInfo.Rows.Count > 0)
                    {
                        RVendorUtils.InsertBBGAudit(timestamp, dtSecAuditInfo, dtMnemonicAuditInfo);
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error("WriteVendorHistory:InsertBBGAudit -> " + ex.ToString());
                }
                finally
                {
                    mLogger.Debug("WriteVendorHistory:InsertBBGAudit -> End");
                }
            }
        }

        public static void InsertBBGAuditHeaders(string timeStamp, Dictionary<string, string> dictHeaders)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("InsertBBGAuditHeaders -> Start");
            try
            {
                if (dictHeaders != null && dictHeaders.Count > 0)
                {
                    var dtHeadersAuditInfo = new DataTable();
                    foreach (string headerName in dictHeaders.Keys)
                    {
                        dtHeadersAuditInfo.Columns.Add(headerName.ToUpper(), typeof(string));

                    }

                    var drHeader = dtHeadersAuditInfo.NewRow();
                    foreach (string headerName in dictHeaders.Keys)
                    {
                        string headerValue = dictHeaders[headerName];
                        drHeader[headerName.ToUpper()] = headerValue;
                    }
                    dtHeadersAuditInfo.Rows.Add(drHeader);

                    if (dtHeadersAuditInfo.Rows.Count > 0)
                    {
                        RVendorUtils.InsertBBGAuditHeaders(timeStamp, dtHeadersAuditInfo);
                    }
                }
                else
                    mLogger.Error("InsertBBGAuditHeaders -> no headers to insert");
            }
            catch (Exception ex)
            {
                mLogger.Error("InsertBBGAuditHeaders -> " + ex.ToString());
            }
            finally
            {
                mLogger.Debug("InsertBBGAuditHeaders -> End");
            }
        }

        public static void UpdateBBGAudit(string timeStamp, SecuritiesCollection resDict)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("UpdateBBGAudit -> Start");
            try
            {
                if (resDict != null && resDict.Count > 0)
                {
                    List<string> marketSectors = Enum.GetNames(typeof(RBbgMarketSector)).Select(x => x.ToLower()).ToList();

                    var dtSecAuditInfo = new DataTable();
                    dtSecAuditInfo.Columns.Add("identifier_value", typeof(string));
                    dtSecAuditInfo.Columns.Add("asset_id", typeof(string));

                    var dtMnemonicAuditInfo = new DataTable();
                    dtMnemonicAuditInfo.Columns.Add("mnemonic_name", typeof(string));

                    foreach (string instrumentID in resDict.Keys)
                    {
                        string marketSector = string.Empty;
                        string identifierValue = string.Empty;
                        GetMarketSectorFromInstrument(instrumentID, marketSectors, out identifierValue, out marketSector);

                        string assetId = string.Empty;
                        var fieldInfo = resDict[instrumentID].Values.Where(q => q.Name.Equals("DL_Asset_Class", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (fieldInfo != null)
                            assetId = fieldInfo.Value;

                        var drSecAudit = dtSecAuditInfo.NewRow();
                        drSecAudit["identifier_value"] = identifierValue;
                        drSecAudit["asset_id"] = assetId;

                        dtSecAuditInfo.Rows.Add(drSecAudit);
                    }

                    var lstFieldInfo = resDict.Values.FirstOrDefault();
                    if (lstFieldInfo != null && lstFieldInfo.Count > 0)
                    {
                        foreach (string fieldName in lstFieldInfo.Values.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).ToList())
                        {
                            var drMnemonic = dtMnemonicAuditInfo.NewRow();
                            drMnemonic["mnemonic_name"] = fieldName;

                            dtMnemonicAuditInfo.Rows.Add(drMnemonic);
                        }
                    }
                    else
                        mLogger.Error("UpdateBBGAudit -> no fields to update");

                    if (dtSecAuditInfo.Rows.Count > 0)
                    {
                        RVendorUtils.UpdateBBGAudit(timeStamp, dtSecAuditInfo, dtMnemonicAuditInfo);
                    }
                }
                else
                {
                    RVendorUtils.UpdateBBGAuditStatus(timeStamp);
                    mLogger.Error("UpdateBBGAudit -> no securities to update");
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("UpdateBBGAudit -> " + ex.ToString());
            }
            finally
            {
                mLogger.Debug("UpdateBBGAudit -> End");
            }
        }

        public static void UpdateBBGAuditHist(string timeStamp, List<string> processedInstruments, List<string> processedFields)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("UpdateBBGAuditHist -> Start");
            try
            {
                if (processedInstruments != null && processedInstruments.Count > 0)
                {
                    List<string> marketSectors = Enum.GetNames(typeof(RBbgMarketSector)).Select(x => x.ToLower()).ToList();

                    var dtSecAuditInfo = new DataTable();
                    dtSecAuditInfo.Columns.Add("identifier_value", typeof(string));

                    var dtMnemonicAuditInfo = new DataTable();
                    dtMnemonicAuditInfo.Columns.Add("mnemonic_name", typeof(string));

                    foreach (string instrumentID in processedInstruments)
                    {
                        string marketSector = string.Empty;
                        string identifierValue = string.Empty;
                        GetMarketSectorFromInstrument(instrumentID, marketSectors, out identifierValue, out marketSector);

                        var drSecAudit = dtSecAuditInfo.NewRow();
                        drSecAudit["identifier_value"] = identifierValue;

                        dtSecAuditInfo.Rows.Add(drSecAudit);
                    }

                    if (processedFields != null && processedFields.Count > 0)
                    {
                        foreach (string fieldName in processedFields)
                        {
                            var drMnemonic = dtMnemonicAuditInfo.NewRow();
                            drMnemonic["mnemonic_name"] = fieldName;

                            dtMnemonicAuditInfo.Rows.Add(drMnemonic);
                        }
                    }
                    else
                        mLogger.Error("UpdateBBGAuditHist -> no fields to update");

                    if (dtSecAuditInfo.Rows.Count > 0)
                    {
                        RVendorUtils.UpdateBBGAuditHist(timeStamp, dtSecAuditInfo, dtMnemonicAuditInfo);
                    }
                }
                else
                {
                    RVendorUtils.UpdateBBGAuditStatus(timeStamp);
                    mLogger.Error("UpdateBBGAuditHist -> no securities to update");
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("UpdateBBGAuditHist -> " + ex.ToString());
            }
            finally
            {
                mLogger.Debug("UpdateBBGAuditHist -> End");
            }
        }

        public static void UpdateBBGAuditCorpAction(string timeStamp, Dictionary<string, List<string>> processedInstruments)
        {
            if (!RVendorUtils.EnableBBGAudit)
                return;

            mLogger.Debug("UpdateBBGAuditHist -> Start");
            try
            {
                if (processedInstruments != null && processedInstruments.Count > 0)
                {
                    List<string> marketSectors = Enum.GetNames(typeof(RBbgMarketSector)).Select(x => x.ToLower()).ToList();

                    var dtSecAuditInfo = new DataTable();
                    dtSecAuditInfo.Columns.Add("identifier_value", typeof(string));
                    dtSecAuditInfo.Columns.Add("corpaction_type_name", typeof(string));

                    foreach (string instrumentID in processedInstruments.Keys)
                    {
                        string marketSector = string.Empty;
                        string identifierValue = string.Empty;
                        GetMarketSectorFromInstrument(instrumentID, marketSectors, out identifierValue, out marketSector);

                        var drSecAudit = dtSecAuditInfo.NewRow();
                        drSecAudit["identifier_value"] = identifierValue;
                        if (processedInstruments[instrumentID] != null && processedInstruments[instrumentID].Count > 0)
                        {
                            var corpActionTypeNames = processedInstruments[instrumentID].Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                            if (corpActionTypeNames.Count > 0)
                                drSecAudit["corpaction_type_name"] = string.Join(",", corpActionTypeNames);
                        }

                        dtSecAuditInfo.Rows.Add(drSecAudit);
                    }

                    if (dtSecAuditInfo.Rows.Count > 0)
                    {
                        RVendorUtils.UpdateBBGAuditCorpAction(timeStamp, dtSecAuditInfo);
                    }
                }
                else
                {
                    RVendorUtils.UpdateBBGAuditStatus(timeStamp);
                    mLogger.Error("UpdateBBGAuditHist -> no securities to update");
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("UpdateBBGAuditHist -> " + ex.ToString());
            }
            finally
            {
                mLogger.Debug("UpdateBBGAuditHist -> End");
            }
        }

        public static void GetMarketSectorFromInstrument(string InstrumentID, List<string> marketSectors, out string identifierValue, out string marketSector)
        {
            marketSector = string.Empty;
            string[] arr = InstrumentID.Split(' ');
            if (arr != null && arr.Length > 0)
            {
                RBbgMarketSector marketSectorEnum;
                if (Enum.TryParse<RBbgMarketSector>(arr[arr.Length - 1], true, out marketSectorEnum) && marketSectors.Contains(arr[arr.Length - 1].ToLower()))
                    marketSector = marketSectorEnum.ToString();
            }

            if (!string.IsNullOrEmpty(marketSector))
                identifierValue = InstrumentID.Substring(0, InstrumentID.ToLower().IndexOf(marketSector.ToLower()) - 1).ToUpper();
            else
                identifierValue = InstrumentID.ToUpper();
        }

        public static void AddBBGAuditHeaders(ref Dictionary<string, string> dictHeaders, string key, string value)
        {
            try
            {
                if (!dictHeaders.ContainsKey(key))
                    dictHeaders.Add(key, value);
            }
            catch (Exception ex)
            {
                mLogger.Error("headerName : " + (key == null ? string.Empty : key));
                mLogger.Error("headerValue : " + (value == null ? string.Empty : value));
                mLogger.Error(ex.ToString());
            }
        }
        #endregion

        #region Helper Methods
        //internal static DateTime GetTargetDateTime()
        //{
        //    TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(
        //         RVendorConfigLoader.GetVendorConfig
        //        (RVendorType.Bloomberg)[RVendorConstant.FTPTIMEZONE]);
        //    DateTime dtEST = RFormatterUtils.ConvertDateToSpecifiedTimeZone
        //                (System.DateTime.Now, tz.StandardName);
        //    return dtEST;
        //}

        internal static string GetTargetDateTime()
        {
            SMDataUploadManagerClient cl = null;
            try
            {
                cl = new SMDataUploadManagerClient();
                return cl.GetGuid();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (cl != null)
                    cl.Close();
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the CSV fields.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        internal static string GetCSVFields(List<RBbgFieldInfo> list)
        {
            string strFields = "";
            foreach (RBbgFieldInfo fields in list)
            {
                strFields = strFields + "," + fields.Mnemonic;
            }
            if (strFields != string.Empty)
                strFields = strFields.Remove(0, 1);
            return strFields;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the CSV fields.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        internal static string GetCSVInstruments(SecuritiesCollection resDict)
        {
            if (resDict == null)
                return string.Empty;
            StringBuilder strFields = new StringBuilder();
            foreach (string key in resDict.Keys)
            {
                strFields.Append(",");
                strFields.Append(key);
            }
            if (strFields.Length > 0)
                strFields = strFields.Remove(strFields.Length - 1, 1);
            return strFields.ToString();
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the CSV fields.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        internal static string GetCSVFields(SecuritiesCollection resDict)
        {
            if (resDict == null || resDict.Count == 0)
                return string.Empty;
            StringBuilder strFields = new StringBuilder();
            var firstList = resDict.Values.First().Values.Select(d => d.Name).ToList();
            //foreach (var item in resDict)
            //{
            //    var data = item.Value.Values.Select(d => d.Name);
            //    var diff = data.Except(firstList);
            //    if (diff.Count() > 0)
            //        firstList.AddRange(diff);
            //}

            firstList.ForEach(q =>
            {
                strFields.Append(q);
                strFields.Append(",");
            });

            if (strFields.Length > 0)
                strFields = strFields.Remove(strFields.Length - 1, 1);
            return strFields.ToString();
        }

        internal static string GetCSVFields(SecuritiesCollection resDict, out int fieldCount)
        {
            fieldCount = 0;
            if (resDict == null || resDict.Count == 0)
                return string.Empty;
            StringBuilder strFields = new StringBuilder();
            var firstList = resDict.Values.First().Values.Select(d => d.Name).ToList();
            fieldCount = firstList.Count;
            //foreach (var item in resDict)
            //{
            //    var data = item.Value.Values.Select(d => d.Name);
            //    var diff = data.Except(firstList);
            //    if (diff.Count() > 0)
            //        firstList.AddRange(diff);
            //}

            firstList.ForEach(q =>
            {
                strFields.Append(q);
                strFields.Append(",");
            });

            if (strFields.Length > 0)
                strFields = strFields.Remove(strFields.Length - 1, 1);
            return strFields.ToString();
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the CSV instruments.
        /// </summary>
        /// <param name="instruments">The instruments.</param>
        /// <returns></returns>
        internal static string GetCSVInstruments(List<RBbgInstrumentInfo> instruments)
        {
            string strInstruments = "";
            foreach (RBbgInstrumentInfo instrument in instruments)
            {
                strInstruments = strInstruments + "," + instrument.InstrumentID;
            }
            if (strInstruments != string.Empty)
                strInstruments = strInstruments.Remove(0, 1);
            return strInstruments;
        }

        /// <summary>
        /// Normalizes the securities.
        /// </summary>
        /// <param name="listResponseSecurities">The list response securities.</param>
        /// <param name="securityInfo">The security info.</param>
        /// <param name="unProcessedResponse">The un processed response.</param>
        /// <returns></returns>
        internal static SecuritiesCollection NormalizeSecurities(List<SecuritiesCollection> listResponseSecurities, RBbgSecurityInfo securityInfo, ref SecuritiesCollection unProcessedResponse)
        {
            SecuritiesCollection normalizedResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);

            if (listResponseSecurities.Count > 0)
            {
                normalizedResponse = listResponseSecurities[0];
            }

            if (listResponseSecurities.Count > 1)
            {
                for (int i = 1; i < listResponseSecurities.Count; i++)
                {
                    var response = listResponseSecurities[i];
                    foreach (string instrument in response.Keys)
                    {
                        if (normalizedResponse.ContainsKey(instrument))
                        {
                            var dictVendorFieldInfo = normalizedResponse[instrument];

                            foreach (var fieldKeyVal in response[instrument])
                            {
                                if (!dictVendorFieldInfo.ContainsKey(fieldKeyVal.Key))
                                    dictVendorFieldInfo.Add(fieldKeyVal.Key, fieldKeyVal.Value);
                            }
                            normalizedResponse[instrument] = dictVendorFieldInfo;
                        }
                        else
                            normalizedResponse[instrument] = response[instrument];
                    }
                }
            }

            foreach (RBbgInstrumentInfo info in securityInfo.Instruments)
            {
                if (securityInfo.InstrumentFields.Count > 0)
                {
                    string instrument = info.InstrumentID;
                    if (normalizedResponse.ContainsKey(instrument))
                    {
                        var processedFieldInfo = normalizedResponse[instrument];
                        foreach (RBbgFieldInfo bbgFldInfo in securityInfo.InstrumentFields)
                        {
                            if (processedFieldInfo.ContainsKey(bbgFldInfo.Mnemonic))
                            {
                                if (bbgFldInfo.IsSystemAdded)
                                    processedFieldInfo.Remove(bbgFldInfo.Mnemonic);
                            }
                            else
                            {
                                RVendorFieldInfo vendorFldInfo = new RVendorFieldInfo();
                                vendorFldInfo.Name = bbgFldInfo.Mnemonic;
                                vendorFldInfo.Status = RVendorStatusConstant.FAILED;
                                vendorFldInfo.Value = RVendorConstant.NOT_AVAILABLE;
                                normalizedResponse[instrument].Add(vendorFldInfo.Name, vendorFldInfo);
                            }
                        }
                    }
                    else
                    {

                        unProcessedResponse.Add(instrument, securityInfo.InstrumentFields.ToDictionary(x => x.Mnemonic, y => new RVendorFieldInfo
                        {
                            Name = y.Mnemonic,
                            Status = RVendorStatusConstant.FAILED,
                            Value = RVendorConstant.NOT_AVAILABLE,
                            ExceptionMessage = RVendorConstant.NOT_PROCESSED
                        }, StringComparer.OrdinalIgnoreCase));

                    }
                }
            }


            return normalizedResponse;
        }

        //internal static SecuritiesCollection NormalizeSecurities(List<SecuritiesCollection> listResponseSecurities, RBbgSecurityInfo securityInfo, ref SecuritiesCollection unProcessedResponse, List<string> lstDuplicateFields)
        //{
        //    SecuritiesCollection normalizedResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
        //    normalizedResponse = listResponseSecurities[0];
        //    if (listResponseSecurities.Count > 1)
        //    {
        //        for (int i = 1; i < listResponseSecurities.Count; i++)
        //        {
        //            var response = listResponseSecurities[i];
        //            response.FirstOrDefault().Value.Values.Select(x => x.Name).ToList();

        //            foreach (string instrument in response.Keys)
        //            {
        //                if (normalizedResponse.ContainsKey(instrument))
        //                {
        //                    normalizedResponse[instrument].AddRange(response[instrument]);
        //                }
        //                else
        //                    normalizedResponse[instrument] = response[instrument];
        //            }
        //        }
        //    }


        //    foreach (IDictionary<string, List<RVendorFieldInfo>> response in listResponseSecurities)
        //    {
        //        foreach (string instrument in response.Keys)
        //        {
        //            if (normalizedResponse.ContainsKey(instrument))
        //            {
        //                //var dictVendorFieldInfo = normalizedResponse[instrument].ToDictionary(x => x.Name, y => y, StringComparer.OrdinalIgnoreCase);
        //                //var dictResponseFields = response[instrument].ToDictionary(x => x.Name, y => y, StringComparer.OrdinalIgnoreCase);

        //                //foreach (var dupFieldName in dictResponseFields.Keys.Intersect(dictVendorFieldInfo.Keys, StringComparer.OrdinalIgnoreCase).ToList())
        //                //{
        //                //    dictResponseFields.Remove(dupFieldName);
        //                //}

        //                //var listVendorFieldInfo = dictVendorFieldInfo.Values.ToList();
        //                //listVendorFieldInfo.AddRange(dictResponseFields.Values.ToList());

        //                normalizedResponse[instrument] = listVendorFieldInfo;
        //            }
        //            else
        //                normalizedResponse[instrument] = response[instrument];
        //        }
        //    }

        //    List<string> unProcessedFields = null;
        //    foreach (string instrument in normalizedResponse.Keys)
        //    {
        //        unProcessedFields = new List<string>();
        //        unProcessedFields = ValidateAllFields(securityInfo.InstrumentFields, normalizedResponse[instrument]);
        //        if (unProcessedFields.Count > 0)
        //        {
        //            foreach (string fld in unProcessedFields)
        //            {
        //                RVendorFieldInfo vendorFldInfo = new RVendorFieldInfo();
        //                vendorFldInfo.Name = fld;
        //                vendorFldInfo.Status = RVendorStatusConstant.FAILED;
        //                vendorFldInfo.Value = RVendorConstant.NOT_AVAILABLE;
        //                normalizedResponse[instrument].Add(vendorFldInfo);
        //            }
        //        }
        //    }

        //    #region "Add Unprocessed Securities"
        //    List<string> unProcessedInstruments = null;
        //    //Validate Fields for only one security as all securities returned will have equal # of fields
        //    unProcessedInstruments = ValidateAllInstruments(securityInfo.Instruments, normalizedResponse);

        //    if (unProcessedInstruments.Count > 0)
        //    {
        //        foreach (string instrument in unProcessedInstruments)
        //        {
        //            List<RVendorFieldInfo> vendorFlds = new List<RVendorFieldInfo>();
        //            foreach (RBbgFieldInfo fldInfo in securityInfo.InstrumentFields)
        //            {
        //                RVendorFieldInfo fieldInfo = new RVendorFieldInfo();
        //                fieldInfo.Name = fldInfo.Mnemonic;
        //                fieldInfo.Status = RVendorStatusConstant.FAILED;
        //                fieldInfo.Value = RVendorConstant.NOT_AVAILABLE;
        //                fieldInfo.ExceptionMessage = RVendorConstant.NOT_PROCESSED;
        //                vendorFlds.Add(fieldInfo);
        //            }
        //            unProcessedResponse[instrument] = vendorFlds;
        //        }
        //    }
        //    #endregion
        //    return normalizedResponse;

        //}
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates the exception message.
        /// </summary>
        /// <param name="internalCode">The internal code.</param>
        /// <param name="externalCode">The external code.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        internal static string CreateExceptionMessage(string internalCode, string externalCode,
                                                                            string description)
        {
            mLogger.Debug("Start -> creating exception message");
            StringBuilder sb = new StringBuilder();
            sb.Append("Internal Status Code: ");
            sb.Append(internalCode);
            sb.AppendLine("External Status Code: ");
            sb.Append(externalCode);
            sb.AppendLine("Description: ");
            sb.Append(description);
            mLogger.Debug("End -> creating exception message");
            return sb.ToString();
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Generates the failed response. This method generates the dictionary of all instruments 
        /// and fields with status as FAILED and Exception set as ExceptionMessage.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        internal static SecuritiesCollection GenerateFailedResponse(RBbgSecurityInfo securityInfo, Exception ex)
        {
            SecuritiesCollection failedResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);

            foreach (RBbgInstrumentInfo instrument in securityInfo.Instruments)
            {
                string instrumentId = instrument.InstrumentID;
                var fieldinfo = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                foreach (RBbgFieldInfo field in securityInfo.InstrumentFields)
                {
                    RVendorFieldInfo vendorFieldInfo = new RVendorFieldInfo();
                    vendorFieldInfo.Name = field.Mnemonic;
                    vendorFieldInfo.Status = "FAILED";
                    vendorFieldInfo.ExceptionMessage = ex.ToString();
                    fieldinfo.Add(vendorFieldInfo.Name, vendorFieldInfo);
                }
                failedResponse[instrumentId] = fieldinfo;
            }
            return failedResponse;
        }
        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Determines whether [is in valid field] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [is in valid field] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        //------------------------------------------------------------------------------------------
        internal static bool IsInValidField(string value)
        {
            if (value.Equals("N.S.") || value.Equals("N.D.") ||
               value.Equals("N.A.") || value.Equals("FLD UNKNOWN") || value.Equals(" "))
                return true;
            else
                return false;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Determines whether [is valid primary qualifier and value] [the specified macro info].
        /// </summary>
        internal static bool IsValidPrimaryQualifierAndValue(RBbgMacroInfo macroInfo,string clientName)
        {            
            bool flag = false;
            List<RMacroConfigInfo> lstMacroConfig = RVendorConfigLoader.mMacroConfig[clientName][RBbgMacroType.Primary];
            foreach (RMacroConfigInfo info in lstMacroConfig)
            {
                if (info.Name == macroInfo.QualifierType)
                {
                    string[] values = info.Value.Split(',');
                    if (values[0] == string.Empty)
                        return true;
                    else
                    {
                        foreach (string val in values)
                        {
                            if (val == macroInfo.QualifierValue)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                            break;
                    }
                }
            }
            return flag;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Determines whether [is valid secondary PQ type and value] [the specified macro info].
        /// </summary>
        internal static bool IsValidSecondaryPQTypeAndValue(RBbgMacroInfo macroInfo, PrimaryQualifier pq, string clientName)
        {
            bool result = false;
            List<RMacroConfigInfo> lstConfigInfo = new List<RMacroConfigInfo>();
            lstConfigInfo = RVendorConfigLoader.mMacroConfig[clientName][RBbgMacroType.Secondary];
            foreach (RMacroConfigInfo info in lstConfigInfo)
            {
                //Check whether QualifierType exist in config.
                if (macroInfo.QualifierType == info.Name)
                {
                    //Check whether macro pq type is valid.
                    if (pq.primaryQualifierType.ToString() == info.PQType)
                    {
                        string[] strValues = info.Value.Split(',');
                        if (strValues[0] != string.Empty)
                        {
                            foreach (string val in strValues)
                            {
                                //Check whether its value exist in config.
                                if (macroInfo.QualifierValue == val)
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                        else
                            result = true;
                        //check whether primary qualifier
                        List<string> pqValues = new List<string>();
                        if (info.PQValues != null)
                            pqValues = info.PQValues;
                        if (pqValues.Count > 0 && result)
                        {
                            foreach (string key in pqValues)
                            {
                                if (key == pq.primaryQualifierValue)
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (result)
                    break;
            }
            return result;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Determines whether [is secondary qualifier valid FTP] [the specified pq].
        /// </summary>
        internal static bool IsSecondaryQualifierValidFTP(RBbgMacroInfo pq, RBbgMacroInfo sq,string clientName)
        {
            bool flag = false;
            List<RMacroConfigInfo> lstMacroConfig = RVendorConfigLoader.mMacroConfig[clientName][RBbgMacroType.Secondary];
            foreach (RMacroConfigInfo configInfo in lstMacroConfig)
            {
                if (configInfo.Name == sq.QualifierType)
                {
                    if (configInfo.PQType == pq.QualifierType)
                    {
                        string[] values = configInfo.Value.Split(',');
                        if (values[0] != string.Empty)
                        {
                            foreach (string val in values)
                            {
                                //Check whether its value exist in config.
                                if (sq.QualifierValue == val)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        else
                            flag = true;
                        //check whether primary qualifier
                        List<string> pqValues = new List<string>();
                        if (configInfo.PQValues != null)
                            pqValues = configInfo.PQValues;
                        if (pqValues.Count > 0 && flag)
                        {
                            foreach (string key in pqValues)
                            {
                                if (key == pq.QualifierValue)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (flag)
                    break;
            }
            return flag;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the error codes.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        internal static void HandleErrorCodes(string errorCode)
        {
            string clientName = SRMMTConfig.GetClientName();

            if (RVendorConfigLoader.mVendorConfig[clientName]["BBGErrorCodes"].ContainsKey(errorCode))
            {
                RVendorException ex = new RVendorException
                        (RVendorConfigLoader.mVendorConfig[clientName]["BBGErrorCodes"][errorCode].ToString());
                throw ex;
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Populates the security info from vendor hist.
        /// </summary>
        /// <param name="historyList">The history list.</param>
        /// <returns></returns>
        internal static RBbgSecurityInfo PopulateSecurityInfoFromVendorHist(List<RVendorHistoryInfo>
                                                                                    historyList)
        {
            RBbgSecurityInfo securityInfo = new RBbgSecurityInfo();
            List<int> lstBulkList = new List<int>();
            securityInfo.Instruments = GenerateInstrumentFromVendorHistory(historyList);
            securityInfo.InstrumentFields = GenerateInstrumentFields(historyList);
            securityInfo.IsBulkList = historyList[0].IsBulkList;
            securityInfo.BulkListMapId = lstBulkList;
            securityInfo.RequestType = (RBbgRequestType)Enum.Parse(typeof(RBbgRequestType), historyList[0].RequestType);
            if (securityInfo.IsBulkList)
            {
                foreach (RVendorHistoryInfo histInfo in historyList)
                {
                    if (!lstBulkList.Contains(Convert.ToInt32(histInfo.VendorFields.Split(',')[1])))
                        lstBulkList.Add(Convert.ToInt32(histInfo.VendorFields.Split(',')[1]));
                }
                securityInfo.BulkListMapId = lstBulkList;
            }

            return securityInfo;
        }
        //------------------------------------------------------------------------------------------

        internal static HashSet<string> GetRestrictedHeaders(RBbgHeaderType requestType)
        {
            HashSet<string> defaultHeadersToSkip = RBbgUtils.GetDefaultHeadersToSkip();

            List<string> result = new List<string>() { "FIRMNAME", "FILETYPE", "REPLYFILENAME", "YELLOWKEY", "SECID", "PROGRAMNAME", "ACTIONS", "ACTIONS_DATE", "DATERANGE", "PERIODICITY", "COMPRESS", "HIST_FORMAT", "COMPRESS" };
            if (!defaultHeadersToSkip.Contains("SECMASTER"))
                result.Add("SECMASTER");
            switch (requestType)
            {
                case RBbgHeaderType.BulkList:
                    result.Add("OUTPUTFORMAT");
                    if (!defaultHeadersToSkip.Contains("HISTORICAL"))
                        result.Add("HISTORICAL");
                    if (!defaultHeadersToSkip.Contains("CLOSINGVALUES"))
                        result.Add("CLOSINGVALUES");
                    if (!defaultHeadersToSkip.Contains("QUOTECOMPOSITE"))
                        result.Add("QUOTECOMPOSITE");
                    break;
                case RBbgHeaderType.BVAL:
                    result.Add("PRICING_SOURCE");
                    if (!defaultHeadersToSkip.Contains("DERIVED"))
                        result.Add("DERIVED");
                    break;
                case RBbgHeaderType.GetData:
                    if (!defaultHeadersToSkip.Contains("DERIVED"))
                        result.Add("DERIVED");
                    break;
                case RBbgHeaderType.GetCompany:
                case RBbgHeaderType.GetActions:
                case RBbgHeaderType.GetHistory:
                case RBbgHeaderType.GetFundamentals:
                case RBbgHeaderType.Security:
                case RBbgHeaderType.Corpaction:
                    break;
            }
            return new HashSet<string>(result, StringComparer.OrdinalIgnoreCase);
        }

        internal static HashSet<string> GetDefaultHeadersToSkip()
        {
            string VendorDefaultHeadersToSkipStr = Convert.ToString(ConfigurationManager.AppSettings["VendorDefaultHeadersToSkip"]);

            HashSet<string> VendorDefaultHeadersToSkip = new HashSet<string>();
            if (!string.IsNullOrEmpty(VendorDefaultHeadersToSkipStr))
                VendorDefaultHeadersToSkip = new HashSet<string>(VendorDefaultHeadersToSkipStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()), StringComparer.OrdinalIgnoreCase);

            return VendorDefaultHeadersToSkip;
        }
        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the multiple bulk list ids.
        /// </summary>
        /// <param name="inputInfo">The input info.</param>
        /// <returns></returns>
        private static List<RBbgSecurityInfo> HandleMultipleBulkListIds(RBbgSecurityInfo inputInfo)
        {
            Dictionary<int, RBbgSecurityInfo> dict = new Dictionary<int, RBbgSecurityInfo>();
            foreach (int id in inputInfo.BulkListMapId)
            {

                if (!dict.ContainsKey(id))
                {
                    dict.Add(id, GenerateSecurityInfoByMultipleBulkId(id, inputInfo));
                }
            }

            List<RBbgSecurityInfo> normalizeSecurities = new List<RBbgSecurityInfo>();
            foreach (KeyValuePair<int, RBbgSecurityInfo> key in dict)
            {
                if (inputInfo.HeaderNamesVsValues != null && inputInfo.HeaderNamesVsValues.Count > 0)
                    key.Value.HeaderNamesVsValues = inputInfo.HeaderNamesVsValues;
                key.Value.VendorPreferenceId = inputInfo.VendorPreferenceId;
                normalizeSecurities.Add(key.Value);
            }
            if (normalizeSecurities.Count == 0)
                normalizeSecurities.Add(inputInfo);
            return normalizeSecurities;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Generates the security info by multiple bulk id.
        /// </summary>
        /// <param name="bulkListId">The bulk list id.</param>
        /// <param name="inputInfo">The input info.</param>
        /// <returns></returns>
        private static RBbgSecurityInfo GenerateSecurityInfoByMultipleBulkId(int bulkListId,
                                                                    RBbgSecurityInfo inputInfo)
        {
            RBbgSecurityInfo bbgSecurityInfo = new RBbgSecurityInfo();
            List<int> lstBulkList = new List<int>();
            List<RBbgInstrumentInfo> instrumentInfo = new List<RBbgInstrumentInfo>();
            bbgSecurityInfo = GetSecurityInfo(inputInfo);
            bbgSecurityInfo.Instruments = inputInfo.Instruments;
            bbgSecurityInfo.InstrumentFields = inputInfo.InstrumentFields;
            //Add BulkList Id 
            lstBulkList.Add(bulkListId);
            bbgSecurityInfo.BulkListMapId = lstBulkList;
            bbgSecurityInfo.IsCombinedFtpReq = inputInfo.IsCombinedFtpReq;
            bbgSecurityInfo.IsBVAL = inputInfo.IsBVAL;
            bbgSecurityInfo.IsGetCompany = inputInfo.IsGetCompany;
            bbgSecurityInfo.SAPIUserName = inputInfo.SAPIUserName;
            bbgSecurityInfo.IPAddresses.AddRange(inputInfo.IPAddresses);
            bbgSecurityInfo.VendorPreferenceId = inputInfo.VendorPreferenceId;
            return bbgSecurityInfo;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the input secrity info.
        /// </summary>
        private static List<RBbgSecurityInfo> HandleInputSecurityInfo(RBbgSecurityInfo inputInfo)
        {
            RBbgSecurityInfo bbgSecurityInfo = null;
            Dictionary<string, RBbgSecurityInfo> dict = new Dictionary<string, RBbgSecurityInfo>();
            foreach (RBbgInstrumentInfo instrInfo in inputInfo.Instruments)
            {

                if (dict.ContainsKey(instrInfo.InstrumentIdType.ToString()))
                {
                    bbgSecurityInfo = dict[instrInfo.InstrumentIdType.ToString()];
                    bbgSecurityInfo.Instruments.Add(GetBbgInstrumentInfo(instrInfo));
                }
                else
                {
                    dict.Add(instrInfo.InstrumentIdType.ToString(),
                                        GenerateBbgSecuityInfo(instrInfo, inputInfo));
                }
            }

            List<RBbgSecurityInfo> normalizeSecurities = new List<RBbgSecurityInfo>();
            foreach (KeyValuePair<string, RBbgSecurityInfo> key in dict)
            {
                if (inputInfo.HeaderNamesVsValues != null && inputInfo.HeaderNamesVsValues.Count > 0)
                    key.Value.HeaderNamesVsValues = inputInfo.HeaderNamesVsValues;
                key.Value.VendorPreferenceId = inputInfo.VendorPreferenceId;
                normalizeSecurities.Add(key.Value);
            }
            if (normalizeSecurities.Count == 0)
                normalizeSecurities.Add(inputInfo);
            return normalizeSecurities;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Generates the BBG secuity info.
        /// </summary>
        private static RBbgSecurityInfo GenerateBbgSecuityInfo(RBbgInstrumentInfo instrInfo,
            RBbgSecurityInfo inputInfo)
        {
            RBbgSecurityInfo bbgSecurityInfo = new RBbgSecurityInfo();
            List<RBbgInstrumentInfo> instrumentInfo = new List<RBbgInstrumentInfo>();
            instrumentInfo = GetInstrumentInfo(instrInfo);
            bbgSecurityInfo = GetSecurityInfo(inputInfo);
            bbgSecurityInfo.Instruments = instrumentInfo;
            bbgSecurityInfo.InstrumentFields = inputInfo.InstrumentFields;
            bbgSecurityInfo.BulkListMapId = inputInfo.BulkListMapId;
            bbgSecurityInfo.IsCombinedFtpReq = inputInfo.IsCombinedFtpReq;
            bbgSecurityInfo.IsBVAL = inputInfo.IsBVAL;
            bbgSecurityInfo.IsBVALPricingSource = inputInfo.IsBVALPricingSource;
            bbgSecurityInfo.IsGetCompany = inputInfo.IsGetCompany;
            bbgSecurityInfo.IsMarketSectorAppended = inputInfo.IsMarketSectorAppended;
            //foreach (var overide in inputInfo.Overrides)
            //    bbgSecurityInfo.Overrides.Add(overide.Key, overide.Value);
            bbgSecurityInfo.VendorPreferenceId = inputInfo.VendorPreferenceId;
            return bbgSecurityInfo;
        }
        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Generates the instrument fields.
        /// </summary>
        /// <param name="historyList">The history list.</param>
        /// <returns></returns>
        private static List<RBbgFieldInfo> GenerateInstrumentFields(List<RVendorHistoryInfo>
                                                                                    historyList)
        {
            Dictionary<string, string> dictFields = new Dictionary<string, string>();
            List<RBbgFieldInfo> lstFields = new List<RBbgFieldInfo>();

            foreach (RVendorHistoryInfo historyInfo in historyList)
            {
                string[] arrFields = historyInfo.VendorFields.Split(',');
                for (int i = 0; i < arrFields.Length; i++)
                {
                    if (!dictFields.ContainsKey(arrFields[i]))
                        dictFields.Add(arrFields[i], arrFields[i]);
                }
            }
            foreach (string key in dictFields.Keys)
            {
                RBbgFieldInfo fldInfo = new RBbgFieldInfo();
                fldInfo.Mnemonic = key;
                lstFields.Add(fldInfo);
                if (historyList[0].IsBulkList)
                    break;
            }
            return lstFields;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Generates the instrument from vendor history.
        /// </summary>
        /// <param name="historyList">The history list.</param>
        /// <returns></returns>
        private static List<RBbgInstrumentInfo> GenerateInstrumentFromVendorHistory(
                                                            List<RVendorHistoryInfo> historyList)
        {
            Dictionary<string, string> dictInstruments = new Dictionary<string, string>();
            List<RBbgInstrumentInfo> lstInstrument = new List<RBbgInstrumentInfo>();
            foreach (RVendorHistoryInfo historyInfo in historyList)
            {
                string[] arrInstruments = historyInfo.VendorInstruments.Split(',');
                for (int i = 0; i < arrInstruments.Length; i++)
                {
                    if (!dictInstruments.ContainsKey(arrInstruments[i]))
                        dictInstruments.Add(arrInstruments[i], arrInstruments[i]);
                }
            }
            foreach (string key in dictInstruments.Keys)
            {
                RBbgInstrumentInfo instrInfo = new RBbgInstrumentInfo();
                instrInfo.InstrumentID = key;
                lstInstrument.Add(instrInfo);
            }
            return lstInstrument;
        }

        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the BBG instrument info.
        /// </summary>
        private static RBbgInstrumentInfo GetBbgInstrumentInfo(RBbgInstrumentInfo instrumentInfo)
        {
            RBbgInstrumentInfo info = new RBbgInstrumentInfo();
            info.InstrumentID = instrumentInfo.InstrumentID;
            info.InstrumentIdType = instrumentInfo.InstrumentIdType;
            info.MarketSector = instrumentInfo.MarketSector;
            info.BVALPriceSourceValue = instrumentInfo.BVALPriceSourceValue;
            return info;
        }
        /// <summary>
        /// Gets the instrument info.
        /// </summary>
        private static List<RBbgInstrumentInfo> GetInstrumentInfo(RBbgInstrumentInfo instrumentInfo)
        {
            List<RBbgInstrumentInfo> lstInstrumentInfo = new List<RBbgInstrumentInfo>();
            RBbgInstrumentInfo info = new RBbgInstrumentInfo();
            info.InstrumentID = instrumentInfo.InstrumentID;
            info.InstrumentIdType = instrumentInfo.InstrumentIdType;
            info.MarketSector = instrumentInfo.MarketSector;
            info.BVALPriceSourceValue = instrumentInfo.BVALPriceSourceValue;
            lstInstrumentInfo.Add(info);
            return lstInstrumentInfo;
        }

        /// <summary>
        /// Validates all instruments.
        /// </summary>
        /// <param name="lstInstrInfo">The LST instr info.</param>
        /// <param name="responseSecurities">The response securities.</param>
        /// <returns></returns>
        private static List<string> ValidateAllInstruments(List<RBbgInstrumentInfo> lstInstrInfo, SecuritiesCollection responseSecurities)
        {
            List<string> unProcessedInstrments = new List<string>();
            foreach (RBbgInstrumentInfo info in lstInstrInfo)
            {
                if (!responseSecurities.ContainsKey(info.InstrumentID))
                    unProcessedInstrments.Add(info.InstrumentID);
            }
            return unProcessedInstrments;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Validates all fields.
        /// </summary>
        /// <param name="fldInfo">The FLD info.</param>
        /// <param name="processedFieldInfo">The processed field info.</param>
        /// <returns></returns>
        //private static List<string> ValidateAllFields(List<RBbgFieldInfo> fldInfo, Dictionary<string, RVendorFieldInfo> processedFieldInfo)
        //{
        //    List<string> unProcessedFields = new List<string>();
        //    Dictionary<string, RVendorFieldInfo> DictProcessedFieldInfo = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
        //    if (processedFieldInfo != null && processedFieldInfo.Count > 0)
        //    {
        //        foreach (var item in processedFieldInfo.Values)
        //        {
        //            DictProcessedFieldInfo[item.Name] = item;
        //        }
        //    }

        //    foreach (RBbgFieldInfo bbgFldInfo in fldInfo)
        //    {
        //        if (DictProcessedFieldInfo.ContainsKey(bbgFldInfo.Mnemonic))
        //        {
        //            if (bbgFldInfo.IsSystemAdded)
        //                processedFieldInfo.Remove(bbgFldInfo.Mnemonic);//DictProcessedFieldInfo[bbgFldInfo.Mnemonic]

        //        }
        //        else
        //            unProcessedFields.Add(bbgFldInfo.Mnemonic);
        //    }
        //    return unProcessedFields;
        //}
        #endregion



        //internal static string PrepareXML(SecuritiesCollection responseSecurities, string timeStamp)
        //{
        //    XDocument responseXml = new XDocument();
        //    XElement root = new XElement("Response");
        //    responseXml.Add(root);
        //    foreach (var security in responseSecurities)
        //    {
        //        StringBuilder fields = new StringBuilder();
        //        var lstFields = security.Value;
        //        lstFields.Values.ToList().ForEach(q => fields.Append("," + q.Name));
        //        if (fields.Length > 0)
        //            fields = fields.Replace(',', ' ', fields.Length - 1, 1);
        //        XElement securityRow = new XElement("Security", new XAttribute("Name", security.Key),
        //                                                 new XAttribute("SecurityType", lstFields["DL_Asset_Class"].Value),
        //                                                 new XAttribute("Fields", fields.ToString().Trim()),
        //                                                 new XAttribute("RequestTime", timeStamp));

        //        root.Add(securityRow);
        //    }
        //    return responseXml.ToString();
        //}

        internal static string PrepareXML(SecuritiesCollection responseSecurities)
        {
            XDocument responseXml = new XDocument();
            XElement root = new XElement("Response");
            responseXml.Add(root);
            foreach (var security in responseSecurities)
            {
                //StringBuilder fields = new StringBuilder();
                var lstFields = security.Value;
                //lstFields.Values.ToList().ForEach(q => fields.Append("," + q.Name));
                //if (fields.Length > 0)
                //    fields = fields.Replace(',', ' ', fields.Length - 1, 1);
                XElement securityRow = new XElement("Security", new XAttribute("Name", security.Key),
                                                         new XAttribute("SecurityType", lstFields["DL_Asset_Class"].Value)
                                                         );
                // ,new XAttribute("Fields", fields.ToString().Trim())
                //,new XAttribute("RequestTime", timeStamp)
                root.Add(securityRow);
            }
            return responseXml.ToString();
        }

        private const string TICKER = "ticker";
        private const string SEDOL1 = "sedol1";
        private const string SEDOL2 = "sedol2";
        private const string BB_GLOBAL = "bbgid";
        private const string BB_UNIQUE = "buid";
        private const string ISIN = "isin";
        private const string CUSIP = "cusip";
        private const string CATS = "cats";
        private const string CINS = "cins";

        internal static string GetInstrumentType(RBbgInstrumentIdType rBbgInstrumentIdType)
        {
            switch (rBbgInstrumentIdType)
            {
                case RBbgInstrumentIdType.TICKER: return TICKER;
                case RBbgInstrumentIdType.SEDOL1: return SEDOL1;
                case RBbgInstrumentIdType.SEDOL2: return SEDOL2;
                case RBbgInstrumentIdType.BB_GLOBAL: return BB_GLOBAL;
                case RBbgInstrumentIdType.BB_UNIQUE: return BB_UNIQUE;
                case RBbgInstrumentIdType.ISIN: return ISIN;
                case RBbgInstrumentIdType.CUSIP: return CUSIP;
                case RBbgInstrumentIdType.CATS: return CATS;
                case RBbgInstrumentIdType.CINS: return CINS;
                default:
                    return rBbgInstrumentIdType.ToString();
            }
        }


        public static void AddEvent(string userName, string message, EType eventType, EventCategory eventCategory)
        {
            Service1Client client = null;
            try
            {
                client = new Service1Client();
                string instanceName = Convert.ToString(ConfigurationManager.AppSettings["InstanceName"]);
                Info info = new Info() { EventID = eventCategory, EventStatus = EStatus.Pending, MessageDetails = message, NameDetails = userName, Product = "RAD - " + instanceName, Type = eventType };
                client.EventFired(info);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                if (client != null)
                    client.Close();
            }
        }
    }
}
