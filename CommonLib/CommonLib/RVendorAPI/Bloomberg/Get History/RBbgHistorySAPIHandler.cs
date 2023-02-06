using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using Bloomberglp.Blpapi;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string,
                                System.Collections.Generic.
                                List<com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using System.Data;

namespace com.ivp.srm.vendorapi.bloomberg
{
    class RBbgHistorySAPIHandler
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        Session session = null;
        bool isValidateSession = false;
        private Service serviceApiAuthSvc;
        private Service serviceRefDataSvc;
        List<Identity> lstUserHandles = null;
        SecuritiesCollection dictVendor = null;
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
        internal void GetHistory(RBbgHistoryInfo historyInfo, string appenTime)
        {
            try
            {
                int UUID = 0;
                mLogger.Debug("RbbgHistorySAPIHandler:GetCorpAction=>start getting history data");
                isValidateSession = Convert.ToBoolean(RVendorConfigLoader.
                                              GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.SAPI_VALIDATE_SESSION));

                if (isValidateSession)
                {
                    if (historyInfo.SAPIUserName.Equals(string.Empty))
                        throw new RVendorException("No user name were specified.");
                    if (historyInfo.IPAddresses.Count == 0)
                        throw new RVendorException("No IPAddresses were specified.");
                }
                lstUserHandles = new List<Identity>();
                session = RBbgUtils.GetServerAPISession(historyInfo.VendorPreferenceId);
                if (session.Start())
                {
                    OpenServices(historyInfo);
                    if (isValidateSession)
                        UUID = RVendorUtils.GetUUID(historyInfo.SAPIUserName);//Fetch UUID from DB.
                    // Authorize user.
                    bool flag = RBbgUtils.AuthorizeSAPIUser(UUID,
                                historyInfo.IPAddresses, session, ref lstUserHandles,
                                serviceApiAuthSvc, isValidateSession);
                    if (flag)
                    {
                        Request request = GenerateServerAPIRequest(historyInfo, serviceRefDataSvc);
                        if (!isValidateSession)
                            session.SendRequest(request, null);
                        else
                        {
                            for (int i = 0; i < lstUserHandles.Count; ++i)
                            {
                                // Send the request for each user
                                Identity identity = (Identity)lstUserHandles[i];
                                session.SendRequest(request, identity, new CorrelationID(UUID));
                            }
                        }
                    }
                    dictVendor = GenerateVendorResponse(session,historyInfo);
                    Dictionary<string, List<RVendorFieldInfo>> unprocessed = new Dictionary<string, List<RVendorFieldInfo>>();
                    foreach (KeyValuePair<string, List<RVendorFieldInfo>> fields in dictVendor)
                    {
                        if (fields.Value.Any(q => q.ExceptionMessage.ToLower().Contains("invalid security")))
                        {
                            unprocessed.Add(fields.Key, fields.Value);
                        }
                    }
                    foreach (KeyValuePair<string, List<RVendorFieldInfo>> fields in unprocessed)
                    {
                        dictVendor.Remove(fields.Key);
                    }
                    historyInfo.ResultantData =  ConvertToDataSet(dictVendor, unprocessed);
                    mLogger.Debug("RbbgHistorySAPIHandler:GetCorpAction=>end getting history data");
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("RbbgHistorySAPIHandler:GetCorpAction=>" + ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
            }
        }
        
        private  void OpenServices(RBbgHistoryInfo historyInfo)
        {
            if (session != null)
            {
                string authorizationServiceName = RVendorConfigLoader.GetVendorConfig
                    (RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.API_AUTH_SVC_NAME);
                string referenceDataServiceName = RVendorConfigLoader.GetVendorConfig
                    (RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.REF_DATA_SVC_NAME);

                if (!session.OpenService(authorizationServiceName))
                    throw new RVendorException("Failed to open service: " + authorizationServiceName);
                serviceApiAuthSvc = session.GetService(authorizationServiceName);

                if (!session.OpenService(referenceDataServiceName))
                    throw new RVendorException("Failed to open service: " + referenceDataServiceName);
                serviceRefDataSvc = session.GetService(referenceDataServiceName);
            }
        }

        private Request GenerateServerAPIRequest(RBbgHistoryInfo securityInfo, Service service)
        {
            Request request = service.CreateRequest("HistoricalDataRequest");
            //Add securities
            Element secs = request.GetElement(RVendorConstant.SECURITIESELEMENTFORSAPI);
            foreach (RHistorySecurityInfo instrInfo in securityInfo.SecurityIds)
            {
                secs.AppendValue(instrInfo.SecurityName + " " + instrInfo.MarcketSector);
            }

            //Add Fields
            Element flds = request.GetElement(RVendorConstant.FIELDSELEMENTFORSAPI);
            foreach (string fldInfo in securityInfo.Fields)
            {
                flds.AppendValue(fldInfo);
            }
            request.Set("periodicityAdjustment", securityInfo.PeriodicAdjustment.ToString());
            request.Set("periodicitySelection", securityInfo.PeriodicSelection.ToString());
            request.Set("startDate", securityInfo.StartDate.ToString("yyyyMMdd"));
            request.Set("endDate", securityInfo.EndDate.ToString("yyyyMMdd"));
            //request.Set("maxDataPoints", 100);
            request.Set("nonTradingDayFillOption", "ALL_CALENDAR_DAYS");
            request.Set("nonTradingDayFillMethod", "PREVIOUS_VALUE");
         
            return request;
        }

        private SecuritiesCollection GenerateVendorResponse(Session session,RBbgHistoryInfo historyInfo)
        {
            SecuritiesCollection dictVendorResponse = null;
            List<RVendorFieldInfo> lstVendorResponse = null;
            dictVendorResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            lstVendorResponse = new List<RVendorFieldInfo>();
            bool done = false;
            mLogger.Debug("Start ->getting fields data");
            try
            {
                dictVendorResponse = new Dictionary<string, List<RVendorFieldInfo>>();
                while (!done)
                {
                    Event eventObj = session.NextEvent();
                    if (eventObj.Type == Event.EventType.PARTIAL_RESPONSE)
                    {
                        //Processing Partial Response

                        SecuritiesCollection Response = processResponseEvent(eventObj, historyInfo);
                        foreach (string key in Response.Keys)
                        {
                            if (!dictVendorResponse.ContainsKey(key))
                                dictVendorResponse[key] = new List<RVendorFieldInfo>();
                            Response[key].ForEach(q => dictVendorResponse[key].Add(q));
                        }
                    }
                    else if (eventObj.Type == Event.EventType.RESPONSE)
                    {
                        //Processing Response
                        SecuritiesCollection Response = processResponseEvent(eventObj, historyInfo);
                        foreach (string key in Response.Keys)
                        {
                            if (!dictVendorResponse.ContainsKey(key))
                                dictVendorResponse[key] = new List<RVendorFieldInfo>();
                            Response[key].ForEach(q => dictVendorResponse[key].Add(q));
                        }
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

        /// <summary>
        /// Processes the response event.
        /// </summary>
        private SecuritiesCollection processResponseEvent(Event eventObj,RBbgHistoryInfo historyInfo)
        {
            SecuritiesCollection dictResponse = null;
            RVendorFieldInfo vFldInfo = null;
            List<RVendorFieldInfo> lstVendorInfo = null;
            dictResponse = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            foreach (Bloomberglp.Blpapi.Message msg in eventObj.GetMessages())
            {

                if (msg.HasElement(RESPONSE_ERROR))
                {
                    throw new Exception(msg.GetElement(RESPONSE_ERROR).GetElementAsString(CATEGORY) +
                        " (" + msg.GetElement(RESPONSE_ERROR).GetElementAsString(MESSAGE) + ")");
                }

                Element securities = msg.GetElement(SECURITY_DATA);
                if (securities.HasElement(RVendorConstant.SECURITYERROR))
                {
                    Element securityError = securities.GetElement(RVendorConstant.SECURITYERROR);
                    string errorMessage = securityError.Elements.ElementAt(3).ToString();
                    lstVendorInfo = new List<RVendorFieldInfo>();
                    historyInfo.Fields.ForEach(q =>
                        {
                            vFldInfo = new RVendorFieldInfo();
                            vFldInfo.Name = q;
                            vFldInfo.Value = " ";
                            vFldInfo.Status = RVendorStatusConstant.FAILED;
                            vFldInfo.ExceptionMessage = errorMessage;
                            lstVendorInfo.Add(vFldInfo);
                        });
                    string instrument = securities.GetElementAsString(SECURITY).Substring(0,
                         securities.GetElementAsString(SECURITY).LastIndexOf(" "));
                    dictResponse[instrument] = lstVendorInfo;
                }
                
                for (int i = 0; i < securities.NumValues; ++i)
                {
                    Element fields = securities.GetElement(FIELD_DATA);
                   
                    for (int j = 0; j < fields.NumValues; ++j)
                    {
                        //Generate Vendor Field Data
                        Element field = fields.GetValueAsElement(j);
                        lstVendorInfo = new List<RVendorFieldInfo>();

                        processRefField(field, ref lstVendorInfo, historyInfo.Fields);//ReferenceData Request


                        Element fieldExceptions = securities.GetElement(FIELD_EXCEPTIONS);
                        if (fieldExceptions.NumValues > 0)
                        {
                            for (int k = 0; k < fieldExceptions.NumValues; ++k)
                            {
                                Element fieldException =
                                    fieldExceptions.GetValueAsElement(k);
                                vFldInfo = new RVendorFieldInfo();
                                vFldInfo.Name = fieldException.GetElementAsString(FIELD_ID);
                                vFldInfo.Value = " ";
                                vFldInfo.Status = RVendorStatusConstant.PASSED;
                                vFldInfo.ExceptionMessage = string.Empty;
                                lstVendorInfo.Add(vFldInfo);
                            }
                        }
                        string InstName = string.Empty;
                        foreach (RHistorySecurityInfo instInfo in historyInfo.SecurityIds)
                        {
                            string instrument = securities.GetElementAsString(SECURITY);
                            if ((instInfo.SecurityName.Trim() + RCorpActionConstant.WHITESPACE + instInfo.MarcketSector.ToString().Trim()).Equals(instrument.Trim()))
                            {
                                InstName = instInfo.SecurityName;
                                break;
                            }
                            else if (instInfo.SecurityName.Trim().Equals(instrument.Trim()))
                            {
                                InstName = instInfo.SecurityName;
                                break;
                            }
                        }
                        //string instrument = securities.GetElementAsString(SECURITY).Substring(0,
                        //  securities.GetElementAsString(SECURITY).LastIndexOf(" "));
                        dictResponse[InstName + " " + j] = lstVendorInfo;//Generate VendorRespone.
                    }
                }
            }
            return dictResponse;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Processes the ref field.
        /// </summary>
        private void processRefField(Element dataField, ref List<RVendorFieldInfo> lstVendorInfo,List<string> fields)
        {
            if (dataField.NumElements > 0)
            {
                RVendorFieldInfo vFldInfo = new RVendorFieldInfo();
                vFldInfo.Name = "DateOfElement";
                vFldInfo.Value = dataField.GetElementAsString(dataField.GetElement(0).Name.ToString());
                vFldInfo.Status = RVendorStatusConstant.PASSED;
                vFldInfo.ExceptionMessage = string.Empty;
                lstVendorInfo.Add(vFldInfo);
            }
            for (int processedField = 1; processedField < dataField.NumElements; processedField++)
            {
                RVendorFieldInfo vFldInfo = new RVendorFieldInfo();
                vFldInfo.Name = dataField.GetElement(processedField).Name.ToString();
                vFldInfo.Value = dataField.GetElementAsString(vFldInfo.Name);
                vFldInfo.Status = RVendorStatusConstant.PASSED;
                vFldInfo.ExceptionMessage = string.Empty;
                lstVendorInfo.Add(vFldInfo);
            }
            
        }

        public static DataSet ConvertToDataSet(IDictionary<string, List<RVendorFieldInfo>> securities,
           IDictionary<string, List<RVendorFieldInfo>> unprocessed)
        {
            DataSet dsResult = null;
            DataTable dtResult = null;
            List<RVendorFieldInfo> fieldInfo = null;
            mLogger.Debug("Start -> converting security info into dataset");
            try
            {
                dsResult = new DataSet();
                dtResult = new DataTable("SecurityInfo");
                fieldInfo = new List<RVendorFieldInfo>();
                //--- Create Instrument Column,Default Columnn ---
                DataColumn dcInstrument = new DataColumn("INSTRUMENT", Type.GetType("System.String"));
                DataColumn dcIsValid = new DataColumn("is_valid", Type.GetType("System.String"));
                DataColumn dcFailureReason = new DataColumn("failure_reason", Type.GetType("System.String"));
                // ---Add Column to DataTable ---
                dtResult.Columns.Add(dcInstrument);
                dtResult.Columns.Add(dcIsValid);
                dtResult.Columns.Add(dcFailureReason);
                //--- Create Columns from security 
                foreach (KeyValuePair<string, List<RVendorFieldInfo>> keyval in securities)
                {
                    fieldInfo = securities[keyval.Key];
                    foreach (RVendorFieldInfo val in fieldInfo)
                    {
                        DataColumn dc = new DataColumn(val.Name.ToUpper(), Type.GetType("System.String"));
                        dtResult.Columns.Add(dc);
                    }
                    break;
                }
                
                if (securities.Count > 0)
                {
                    foreach (KeyValuePair<string, List<RVendorFieldInfo>> keyval in securities)
                    {
                        fieldInfo = securities[keyval.Key];
                        DataRow drRow = dtResult.NewRow();
                        drRow["INSTRUMENT"] = keyval.Key.Substring(0,keyval.Key.LastIndexOf(' '));
                        drRow["is_valid"] = RVendorStatusConstant.PASSED;
                        for (int i = 0; i < fieldInfo.Count; i++)
                        {
                            drRow[fieldInfo[i].Name] = fieldInfo[i].Value;
                        }                        
                        dtResult.Rows.Add(drRow);
                    }
                }
                if (unprocessed != null && unprocessed.Count > 0)
                {
                    foreach (KeyValuePair<string, List<RVendorFieldInfo>> keyval in unprocessed)
                    {
                        DataRow drRow = dtResult.NewRow();
                        fieldInfo = unprocessed[keyval.Key];
                        drRow["INSTRUMENT"] = keyval.Key;
                        for (int i = 0; i < fieldInfo.Count; i++)
                        {
                            drRow[fieldInfo[i].Name] = fieldInfo[i].Value;
                            drRow["failure_reason"] = fieldInfo[i].ExceptionMessage;
                        }
                        drRow["is_valid"] = RVendorStatusConstant.FAILED;
                        dtResult.Rows.Add(drRow);
                    }
                }
                dsResult.Tables.Add(dtResult);
            }
            catch (RVendorException ex)
            {
                throw new RVendorException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End -> converting security info into dataset");
            }
            return dsResult;
        }
    }
}
