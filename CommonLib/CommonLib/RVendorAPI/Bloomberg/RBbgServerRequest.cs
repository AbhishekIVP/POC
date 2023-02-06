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
 * 1            23-09-2009      Nitin Saxena    Initial Version
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using com.ivp.rad.common;
using Bloomberglp.Blpapi;
using com.ivp.srm.vendorapi.bloomberg;
//using SecuritiesCollection = System.Collections.Generic.Dictionary<string,
//                                System.Collections.Generic.
//                                List<com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using com.ivp.srm.vendorapi.AlterLoggerService;

namespace com.ivp.srm.vendorapi.Bloomberg
{
    /// <summary>
    /// Class for processing server api request.
    /// </summary>
    internal class RBbgServerRequest
    {
        #region class member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("RBbgServerRequest");
        private static readonly Name SECURITY_DATA = new Name("securityData");
        private static readonly Name SECURITY = new Name("security");
        private static readonly Name FIELD_DATA = new Name("fieldData");
        private static readonly Name RESPONSE_ERROR = new Name("responseError");
        private static readonly Name SECURITY_ERROR = new Name("securityError");
        private static readonly Name FIELD_EXCEPTIONS = new Name("fieldExceptions");
        private static readonly Name FIELD_ID = new Name("fieldId");
        private static readonly Name ERROR_INFO = new Name("errorInfo");
        private static readonly Name CATEGORY = new Name("category");
        private static readonly Name MESSAGE = new Name("message");
        private Service serviceApiAuthSvc;
        private Service serviceRefDataSvc;
        SecuritiesCollection dictVendor = null;
        RBbgSecurityInfo bbgSecurityInfo = null;
        private List<UserHandle> lstUserHandles;
        Session session = null;
        bool isValidateSession = false;
        #endregion
        //------------------------------------------------------------------------------------------
        #region Public Methods
        /// <summary>
        /// Gets the securities.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        public SecuritiesCollection GetSecurities(RBbgSecurityInfo securityInfo)
        {
            mLogger.Debug("Start ->getting securities");
            dictVendor = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            int UUID = 0;
            try
            {

                isValidateSession = Convert.ToBoolean(RVendorConfigLoader.
                                               GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.SAPI_VALIDATE_SESSION));

                if (isValidateSession)
                {
                    if (string.IsNullOrEmpty(securityInfo.SAPIUserName))
                        throw new RVendorException("UserNameNotProvided");
                    if (securityInfo.IPAddresses == null || securityInfo.IPAddresses.Count == 0)
                        throw new RVendorException("IPAddressNotProvided");
                }

                lstUserHandles = new List<UserHandle>();
                bbgSecurityInfo = securityInfo;
                session = RBbgUtils.GetServerAPISession(securityInfo.VendorPreferenceId);
                mLogger.Debug("GetSecurities=>starting sesion");
                if (session.Start())
                {
                    mLogger.Debug("GetSecurities=> sesion started , opening service.");
                    OpenServices(securityInfo);//Open Authorization and reference data service.
                    mLogger.Debug("GetSecurities=>service opened.");
                    if (isValidateSession)
                        UUID = RVendorUtils.GetUUID(securityInfo.SAPIUserName);//Fetch UUID from DB.
                    mLogger.Debug(string.Format("UUID-{0} and IPAddress-{1}", UUID, string.Join(", ", securityInfo.IPAddresses)));
                    // Authorize user.
                    bool flag = RBbgUtils.AuthorizeSAPIUser(UUID,
                                securityInfo.IPAddresses, session, ref lstUserHandles,
                                serviceApiAuthSvc, isValidateSession);
                    if (flag)
                    {
                        //Generate Server API request.
                         StringBuilder message=new StringBuilder();
                            message.AppendLine("Requesting following identifiers though SAPI:");
                        Request request = GenerateServerAPIRequest(securityInfo, serviceRefDataSvc,message);
                        //RBbgUtils.AddEvent("", message.ToString(), EType.Information, EventCategory.General);

                        if (!isValidateSession)
                        {
                           
                            session.SendRequest(request, null);

                        }
                        else
                        {
                            for (int i = 0; i < lstUserHandles.Count; ++i)
                            {
                                // Send the request for each user
                                UserHandle identity = (UserHandle)lstUserHandles[i];
                                session.SendRequest(request, identity, new CorrelationID(UUID));
                            }
                        }
                        dictVendor = GenerateVendorResponse(session,securityInfo);//Generate Response.
                    }
                    else
                        throw new RVendorException("LoginFailed");
                }
                else
                    throw new RVendorException("CreateSessionFailed");
                mLogger.Debug("Start ->end getting securities");
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
                if (session != null)
                    session.Stop();
                mLogger.Debug("End ->getting securities");
            }
            return dictVendor;
        }

        public bool IsUserAutherized(string userName, List<string> ipAddresses, RBbgSecurityInfo securityInfo)
        {
            try
            {
                isValidateSession = true;
                mLogger.Debug("Start ->start Autherizing user");
                int UUID = 0;
                if (userName.Equals(string.Empty))
                    throw new RVendorException("No user name were specified.");
                if (ipAddresses.Count == 0)
                    throw new RVendorException("No IPAddresses were specified.");
                lstUserHandles = new List<UserHandle>();
                session = RBbgUtils.GetServerAPISession(securityInfo.VendorPreferenceId);
                if (session.Start())
                {
                    mLogger.Debug("GetSecurities=> sesion started , opening service.");
                    OpenServices(securityInfo);//Open Authorization and reference data service.
                    mLogger.Debug("GetSecurities=>service opened.");
                    if (isValidateSession)
                        UUID = RVendorUtils.GetUUID(userName);//Fetch UUID from DB.
                    // Authorize user.
                    bool flag = RBbgUtils.AuthorizeSAPIUser(UUID,
                                ipAddresses, session, ref lstUserHandles,
                                serviceApiAuthSvc, isValidateSession);
                    mLogger.Debug("Start ->end Autherizing user");
                    return flag;
                }
                mLogger.Debug("Start ->end Autherizing user");
                return false;
            }
            catch (RVendorException ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                session.Stop();
            }
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region Private Methods
        /// <summary>
        /// Opens the services.
        /// </summary>
        private void OpenServices(RBbgSecurityInfo securityInfo)
        {
            if (session != null)
            {
                string authorizationServiceName = RVendorConfigLoader.GetVendorConfig
                    (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.API_AUTH_SVC_NAME);
                string referenceDataServiceName = RVendorConfigLoader.GetVendorConfig
                    (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.REF_DATA_SVC_NAME);
                if (serviceApiAuthSvc == null)
                {
                    if (!session.OpenService(authorizationServiceName))
                        throw new RVendorException("Failed to open service: " + authorizationServiceName);
                    serviceApiAuthSvc = session.GetService(authorizationServiceName);
                }
                if (serviceRefDataSvc == null)
                {
                    if (!session.OpenService(referenceDataServiceName))
                        throw new RVendorException("Failed to open service: " + referenceDataServiceName);
                    serviceRefDataSvc = session.GetService(referenceDataServiceName);
                }
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Generates the server API request.
        /// </summary>
        private Request GenerateServerAPIRequest(RBbgSecurityInfo securityInfo,Service service,StringBuilder message)
        {
            Request request = service.CreateRequest(RVendorConstant.OPERATIONNAMEREFDATA);
            //Add securities
            Element secs = request.GetElement(RVendorConstant.SECURITIESELEMENTFORSAPI);
            foreach (RBbgInstrumentInfo instrInfo in securityInfo.Instruments)
            {
                string instrumentType = string.Empty;
                instrumentType = RBbgUtils.GetInstrumentType(instrInfo.InstrumentIdType);
                if (instrInfo.MarketSector != RBbgMarketSector.None)
                    secs.AppendValue("/" + instrumentType + "/" + instrInfo.InstrumentID + " " + instrInfo.MarketSector);
                else
                    secs.AppendValue("/" + instrumentType + "/" + instrInfo.InstrumentID );
                message.AppendLine(instrInfo.InstrumentID + " " + instrInfo.InstrumentIdType);
                //secs.AppendValue(instrInfo.InstrumentID );
            }

            //Add Fields
            Element flds = request.GetElement(RVendorConstant.FIELDSELEMENTFORSAPI);
            foreach (RBbgFieldInfo fldInfo in securityInfo.InstrumentFields)
            {
                flds.AppendValue(fldInfo.Mnemonic);
            }
            
            if (securityInfo.Overrides != null && securityInfo.Overrides.Count > 0)
            {
                Element overrides = request.GetElement(new Name("overrides"));
                foreach (KeyValuePair<string, object> secOverride in securityInfo.Overrides)
                {
                    Element override1 = overrides.AppendElement();
                    override1.SetElement("fieldId", secOverride.Key);
                    //Type type = secOverride.Value.GetType();
                    //var a = Activator.CreateInstance(type);
                    //a= 
                    //dynamic d = Convert.ChangeType(secOverride.Value, type);
                    //GetValue < secOverride.Value.GetType() > (secOverride.Value);
                    override1.SetElement("value",Encoding.ASCII.GetBytes(secOverride.Value.ToString()) );
                }
            }
            var sysOverrides = RVendorUtils.GetSystemDefinedOverrides();
            if (sysOverrides != null && sysOverrides.Count > 0)
            {
                Element overrides = request.GetElement(new Name("overrides"));
                foreach (KeyValuePair<string, Dictionary<string,object>> secOverride in sysOverrides)
                {
                    if (securityInfo.InstrumentFields.Any(q => q.Mnemonic.Equals(secOverride.Key)))
                    {
                        var overrideValue = secOverride.Value.First();
                        Element override1 = overrides.AppendElement();
                        override1.SetElement("fieldId", overrideValue.Key);
                        override1.SetElement("value", Encoding.ASCII.GetBytes(overrideValue.Value.ToString()));
                    }
                }
                
            }
            if (isValidateSession)
                request.Set(RVendorConstant.ELEMENTEIDSFORSAPI, true);
            return request;
        }
        public t GetValue<t>(object data)
        {
            return (t)data;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Generates the vendor response.
        /// </summary>
        private SecuritiesCollection GenerateVendorResponse(Session session,RBbgSecurityInfo securityInfo)
        {
            SecuritiesCollection dictVendorResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            bool done = false;
            mLogger.Debug("Start ->getting fields data");
            try
            {
                while (!done)
                {
                    Event eventObj = session.NextEvent();
                    if (eventObj.Type == Event.EventType.PARTIAL_RESPONSE)
                    {
                        //Processing Partial Response
                        processResponseEvent(eventObj,dictVendorResponse, securityInfo);
                    }
                    else if (eventObj.Type == Event.EventType.RESPONSE)
                    {
                        //Processing Response
                        processResponseEvent(eventObj, dictVendorResponse, securityInfo);
                        done = true;
                    }
                    else
                    {
                        foreach (Bloomberglp.Blpapi.Message msg in eventObj.GetMessages())
                        {
                            if (eventObj.Type == Event.EventType.SESSION_STATUS)
                            {
                                if (msg.MessageType.Equals("SessionTerminated"))
                                    done = true;
                            }
                        }
                    }
                }
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw new RVendorException(rEx.Message);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->getting fields data");
            }
            return dictVendorResponse;
        }
        //------------------------------------------------------------------------------------------

        /// <summary>
        /// Processes the response event.
        /// </summary>
        private SecuritiesCollection processResponseEvent(Event eventObj, SecuritiesCollection dictResponse, RBbgSecurityInfo securityInfo)
        {
           // SecuritiesCollection dictResponse = null;
            //dictResponse = new SecuritiesCollection();
            foreach (Bloomberglp.Blpapi.Message msg in eventObj.GetMessages())
            {

                if (msg.HasElement(RESPONSE_ERROR))
                {
                    throw new Exception(msg.GetElement(RESPONSE_ERROR).GetElementAsString(CATEGORY) +
                        " (" + msg.GetElement(RESPONSE_ERROR).GetElementAsString(MESSAGE) + ")");
                }

                Element securities = msg.GetElement(SECURITY_DATA);
                for (int i = 0; i < securities.NumValues; ++i)
                {
                    var lstVendorInfo = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                    //Get Security Information
                    Element security = securities.GetValueAsElement(i);
                    string SecurityName = security.GetElementAsString("security");
                    if (security.HasElement(RVendorConstant.SECURITYERROR))
                        continue;
                    //Handle Fields processing
                    Element fields = security.GetElement(FIELD_DATA);
                    
                    Element fieldExceptions = security.GetElement(FIELD_EXCEPTIONS);
                    Dictionary<string, object> dictFailedFields = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    if (fieldExceptions.NumValues > 0)
                    {
                        for (int k = 0; k < fieldExceptions.NumValues; ++k)
                        {
                            Element fieldException = fieldExceptions.GetValueAsElement(k);
                            if (bbgSecurityInfo.IsBulkList)
                            {
                                continue;
                                //throw new RVendorException(fieldException.GetElementAsString(FIELD_ID) +
                                //    " " + fieldException.GetElement(ERROR_INFO).GetElementAsString(MESSAGE));
                            }
                            var vFldInfo = new RVendorFieldInfo();
                            vFldInfo.Name = fieldException.GetElementAsString(FIELD_ID);
                            vFldInfo.Value = " ";
                            vFldInfo.Status = RVendorStatusConstant.PASSED;
                            lstVendorInfo.Add(vFldInfo.Name, vFldInfo);

                            dictFailedFields.Add(vFldInfo.Name, new object());
                        }
                    }

                    if (fields.NumElements > 0)
                    {
                        for (int j = 0; j < fields.NumElements; ++j)
                        {
                            //Generate Vendor Field Data
                            Element field = fields.GetElement(j);
                            if (field.Datatype == Schema.Datatype.SEQUENCE && bbgSecurityInfo.IsBulkList)
                                processBulkField(field, ref dictResponse, bbgSecurityInfo.BulkListMapId, SecurityName);//Bulk Request
                            if (field.Datatype != Schema.Datatype.SEQUENCE && bbgSecurityInfo.IsBulkList == false)
                                processRefField(field, ref lstVendorInfo);//ReferenceData Request
                            if (field.Datatype == Schema.Datatype.SEQUENCE && !bbgSecurityInfo.IsBulkList)
                            {
                                string bulkMnemonicName = field.Name.ToString();
                                if (!dictFailedFields.ContainsKey(bulkMnemonicName))
                                    processBulkField(field, ref lstVendorInfo);
                            }
                        }
                    }

                    if (!bbgSecurityInfo.IsBulkList)
                    {
                        //string instrumentDet = security.GetElementAsString(SECURITY).Substring(0,
                        //  security.GetElementAsString(SECURITY).LastIndexOf(" "));
                        //string[] instrument = instrumentDet.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        //StringBuilder InstName = new StringBuilder();
                        //for (int count = 1; count < instrument.Length; count++)
                        //{
                        //    InstName.Append(instrument[count] + "/");
                        //}
                        //InstName = InstName.Remove(InstName.Length - 1, 1);
                        //dictResponse[InstName.ToString()] = lstVendorInfo;//Generate VendorRespone.

                        string instrumentDet = security.GetElementAsString(SECURITY);
                        string[] instrument = instrumentDet.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        StringBuilder InstName = new StringBuilder();
                        for (int count=1;count<instrument.Length;count++)
                        {
                            InstName.Append(instrument[count] + "/");
                        }
                        InstName = InstName.Remove(InstName.Length - 1, 1);
                        foreach (RBbgInstrumentInfo instInfo in securityInfo.Instruments)
                        {
                            if ((instInfo.InstrumentID.Trim() + RCorpActionConstant.WHITESPACE + instInfo.MarketSector.ToString().Trim()).Equals(InstName.ToString()))
                            {
                                InstName = new StringBuilder(InstName.ToString().Substring(0, InstName.ToString().LastIndexOf(" ")));
                                break;
                            }
                            else if (instInfo.InstrumentID.Trim().Equals(InstName))
                            {
                                break;
                            }
                            else
                            {

                            }
                        }
                        dictResponse[InstName.ToString()] = lstVendorInfo;//Generate VendorRespone.
                    }
                }
            } 
            return dictResponse;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Processes the ref field.
        /// </summary>
        private void processRefField(Element field, ref Dictionary<string, RVendorFieldInfo> lstVendorInfo)
        {
            RVendorFieldInfo vFldInfo = null;
            vFldInfo = new RVendorFieldInfo();
            vFldInfo.Name = field.Name.ToString();
            vFldInfo.Value = field.GetValueAsString();
            vFldInfo.Status = RVendorStatusConstant.PASSED;
            lstVendorInfo.Add(vFldInfo.Name, vFldInfo);
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Processes the bulk field.
        /// </summary>
        private void processBulkField(Element refBulkField, ref SecuritiesCollection dictResponse,List<int> bulkListId,string securityName)
        {
            RVendorFieldInfo vFieldInfo = null;
            int count=0;
            int numofBulkValues = refBulkField.NumValues;
            for (int bvCtr = 0; bvCtr < numofBulkValues; bvCtr++)
            {
                var lstVendorInfo = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                Element bulkElement = refBulkField.GetValueAsElement(bvCtr);
                // Get the number of sub fields for each bulk data element
                int numofBulkElements = bulkElement.NumElements;
                // Read each field in Bulk data
                for (int beCtr = 0; beCtr < numofBulkElements; beCtr++)
                {
                    Element elem = bulkElement.GetElement(beCtr);
                    //System.Console.WriteLine("\t\t" + elem.Name + " = "
                    //                        + elem.GetValueAsString());
                    vFieldInfo = new RVendorFieldInfo();
                    vFieldInfo.Name = elem.Name.ToString();
                    vFieldInfo.Value = elem.GetValueAsString();
                    vFieldInfo.Status = RVendorStatusConstant.PASSED;
                    lstVendorInfo.Add(vFieldInfo.Name, vFieldInfo);
                }
                dictResponse.Add(count.ToString() + "," + bulkListId[0].ToString() + "," + securityName, lstVendorInfo);
                //dictResponse[count.ToString()] = lstVendorInfo;
                count++;
            }
        }

        private void processBulkField(Element refBulkField, ref Dictionary<string, RVendorFieldInfo> lstMasterVendorInfo)
        {
            RVendorFieldInfo vFldInfo = null;
            vFldInfo = new RVendorFieldInfo();
            vFldInfo.Name = refBulkField.Name.ToString();
            vFldInfo.Status = RVendorStatusConstant.PASSED;
            lstMasterVendorInfo.Add(vFldInfo.Name, vFldInfo);

            int count = 0;
            int numofBulkValues = refBulkField.NumValues;
            vFldInfo.Value = ";2;" + numofBulkValues.ToString() + ";";
            for (int bvCtr = 0; bvCtr < numofBulkValues; bvCtr++)
            {
                Element bulkElement = refBulkField.GetValueAsElement(bvCtr);
                // Get the number of sub fields for each bulk data element
                int numofBulkElements = bulkElement.NumElements;
                if (bvCtr == 0)
                {
                    vFldInfo.Value += numofBulkElements.ToString() + ";";
                }

                StringBuilder str = new StringBuilder();
                // Read each field in Bulk data
                for (int beCtr = 0; beCtr < numofBulkElements; beCtr++)
                {
                    Element elem = bulkElement.GetElement(beCtr);
                    str.Append(((int)elem.Datatype).ToString()).Append(";").Append(elem.GetValueAsString()).Append(";");
                }
                vFldInfo.Value += str.ToString();
                count++;
            }
        }
        #endregion
    }
}