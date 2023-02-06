using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloomberglp.Blpapi;
using com.ivp.rad.common;
using com.ivp.srm.vendorapi.bloomberg;
//using SecuritiesCollection = System.Collections.Generic.Dictionary<string,
//                                System.Collections.Generic.
//                                List<com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;

namespace com.ivp.srm.vendorapi.Bloomberg
{
    internal class RBbgManagedBPipe
    {
        #region class member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("RBbgManagedBPipe");
        static Session session = null;
        private static Service serviceApiAuthSvc;
        private static Service serviceRefDataSvc;
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
        SecuritiesCollection dictVendor = null;
        RBbgSecurityInfo bbgSecurityInfo = null;
        #endregion

        public SecuritiesCollection GetSecurities(RBbgSecurityInfo securityInfo)
        {
            mLogger.Debug("Start ->getting securities");
            dictVendor = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            try
            {
                string authentication = RVendorConfigLoader.GetVendorConfig
                                           (RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.MANAGEDAUTHENTICATION);
                session = RBbgUtils.GetManagedBPipeSession(authentication, securityInfo);
                mLogger.Debug("GetSecurities=>starting sesion");
                if (session.Start())
                {
                    bool flag = false;
                    string token = "";
                    bbgSecurityInfo = securityInfo;
                    if (authentication.Equals("application_only", StringComparison.OrdinalIgnoreCase))
                    {
                        CorrelationID tokenCorrelator = new CorrelationID(99);
                        EventQueue tokenEventQueue = new EventQueue();
                        session.GenerateToken(tokenCorrelator, tokenEventQueue);

                        Event tokenEvent = tokenEventQueue.NextEvent();

                        foreach (Message message in tokenEvent.GetMessages())
                        {

                            if ("TokenGenerationFailure" == message.MessageType.ToString())
                            {
                                mLogger.Error(message.ToString());
                                flag = false; break;
                            }
                            if ("TokenGenerationSuccess" == message.MessageType.ToString())
                            {
                                flag = true;
                                token = message.GetElementAsString("token");
                                break;
                            }
                        }
                    }
                    else if (authentication.Equals("user_and_application", StringComparison.OrdinalIgnoreCase))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        mLogger.Debug("GetSecurities=> sesion started , opening service.");
                        OpenServices(securityInfo);//Open Authorization and reference data service.
                        mLogger.Debug("GetSecurities=>service opened.");
                        Identity identity = session.CreateIdentity();
                        Request authorizationRequest = serviceApiAuthSvc.CreateAuthorizationRequest();
                        CorrelationID identityCorrelator = new CorrelationID(99);
                        EventQueue identityEventQueue = new EventQueue();
                        if (authentication.Equals("APPLICATION_ONLY"))
                            authorizationRequest.Set("token", token);
                        else if (authentication.Equals("USER_AND_APPLICATION"))
                        {
                            if (string.IsNullOrWhiteSpace(securityInfo.SAPIUserName))
                                throw new RVendorException("UserNameNotProvided");
                            else
                            {
                                string emrsid = RVendorUtils.GetEMRSID(securityInfo.SAPIUserName);
                                authorizationRequest.Set("emrsId", emrsid);
                            }
                            if (securityInfo.IPAddresses == null || securityInfo.IPAddresses.Count == 0)
                                throw new RVendorException("IPAddressNotProvided");
                            else
                                authorizationRequest.Set("ipAddress", securityInfo.IPAddresses[0]);
                        }
                        session.SendAuthorizationRequest(authorizationRequest, identity, identityEventQueue, identityCorrelator);
                        Event authorizationEvent = identityEventQueue.NextEvent();
                        foreach (Message message in authorizationEvent.GetMessages())
                        {

                            if ("AuthorizationFailure" == message.MessageType.ToString())
                            {
                                mLogger.Error(message.ToString());
                                flag = false;
                                break;
                            }
                            if ("AuthorizationSuccess" == message.MessageType.ToString())
                            {
                                flag = true;
                                break;
                            }
                        }

                        if (flag)
                        {
                            //Generate Server API request.
                            Request request = GenerateServerAPIRequest(securityInfo, serviceRefDataSvc);

                            session.SendRequest(request, null);
                            dictVendor = GenerateVendorResponse(session);//Generate Response.
                        }
                        else
                            throw new RVendorException("Authorization Failed. User not logged In.");
                    }
                    else
                    {
                        throw new RVendorException("Token generation failed");
                    }
                }
                else
                    throw new RVendorException("Failed to start session.");
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
                session.Stop();
                mLogger.Debug("End ->getting securities");
            }
            return dictVendor;
        }

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

        private Request GenerateServerAPIRequest(RBbgSecurityInfo securityInfo, Service service)
        {
            Request request = service.CreateRequest(RVendorConstant.OPERATIONNAMEREFDATA);
            //Add securities
            Element secs = request.GetElement(RVendorConstant.SECURITIESELEMENTFORSAPI);
            foreach (RBbgInstrumentInfo instrInfo in securityInfo.Instruments)
            {
                secs.AppendValue(instrInfo.InstrumentID + " " + instrInfo.MarketSector);
            }

            //Add Fields
            Element flds = request.GetElement(RVendorConstant.FIELDSELEMENTFORSAPI);
            foreach (RBbgFieldInfo fldInfo in securityInfo.InstrumentFields)
            {
                flds.AppendValue(fldInfo.Mnemonic);
            }
            //if (securityInfo.Overrides != null && securityInfo.Overrides.Count > 0)
            //{
            //    Element overrides = request.GetElement(new Name("overrides"));
            //    foreach (KeyValuePair<string, object> secOverride in securityInfo.Overrides)
            //    {
            //        Element override1 = overrides.AppendElement();
            //        override1.SetElement("fieldId", secOverride.Key);
            //        //Type type = secOverride.Value.GetType();
            //        //var a = Activator.CreateInstance(type);
            //        //a= 
            //        //dynamic d = Convert.ChangeType(secOverride.Value, type);
            //        //GetValue < secOverride.Value.GetType() > (secOverride.Value);
            //        override1.SetElement("value", Encoding.ASCII.GetBytes(secOverride.Value.ToString()));
            //    }
            //}
            //if (isValidateSession)
            //    request.Set(RVendorConstant.ELEMENTEIDSFORSAPI, true);
            return request;
        }

        private SecuritiesCollection GenerateVendorResponse(Session session)
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
                        processResponseEvent(eventObj, dictVendorResponse);
                    }
                    else if (eventObj.Type == Event.EventType.RESPONSE)
                    {
                        //Processing Response
                        processResponseEvent(eventObj, dictVendorResponse);
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

        private SecuritiesCollection processResponseEvent(Event eventObj, SecuritiesCollection dictResponse)
        {
            // SecuritiesCollection dictResponse = null;
            RVendorFieldInfo vFldInfo = null;
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
                        }
                    }

                    Element fieldExceptions = security.GetElement(FIELD_EXCEPTIONS);
                    if (fieldExceptions.NumValues > 0)
                    {
                        for (int k = 0; k < fieldExceptions.NumValues; ++k)
                        {
                            Element fieldException =
                                fieldExceptions.GetValueAsElement(k);
                            if (bbgSecurityInfo.IsBulkList)
                            {
                                throw new RVendorException(fieldException.GetElementAsString(FIELD_ID) +
                                    " " + fieldException.GetElement(ERROR_INFO).GetElementAsString(MESSAGE));
                            }
                            vFldInfo = new RVendorFieldInfo();
                            vFldInfo.Name = fieldException.GetElementAsString(FIELD_ID);
                            vFldInfo.Value = " ";
                            vFldInfo.Status = RVendorStatusConstant.PASSED;
                            lstVendorInfo.Add(vFldInfo.Name, vFldInfo);
                        }
                    }
                    if (!bbgSecurityInfo.IsBulkList)
                    {
                        string instrument = security.GetElementAsString(SECURITY).Substring(0,
                          security.GetElementAsString(SECURITY).LastIndexOf(" "));
                        dictResponse[instrument] = lstVendorInfo;//Generate VendorRespone.
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
        private void processBulkField(Element refBulkField, ref SecuritiesCollection dictResponse, List<int> bulkListId, string securityName)
        {
            RVendorFieldInfo vFieldInfo = null;
            int count = 0;
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
    }
}
