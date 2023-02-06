using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.srm.vendorapi.bloombergServices.heavy;
using com.ivp.srm.vendorapi.bloomberg;
using System.Collections.Specialized;
using com.ivp.rad.common;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Data;
using com.ivp.srmcommon;

namespace com.ivp.srm.vendorapi
{
    public class RCorpActHeavyHandler
    {
        #region class member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        internal OrderedDictionary _instrumentCode = new OrderedDictionary();
        #endregion

        submitGetActionsRequestRequest sbmtDataRequest = null;
        retrieveGetActionsResponseRequest rtrvDataRequest = null;
        submitGetActionsRequestResponse sbmtDataResponse = null;
        PerSecurityWS service = null;

        public void GetCorpAction(RCorpActInfo corpActInfo, string timeStamp, ref Dictionary<string, string> dictHeaders)
        {
            mLogger.Debug("Start ->getting securities");
            try
            {
                sbmtDataRequest = new submitGetActionsRequestRequest();
                rtrvDataRequest = new retrieveGetActionsResponseRequest();
                sbmtDataResponse = new submitGetActionsRequestResponse();

                service = RCorpActUtils.GetHeavyService(corpActInfo);
                sbmtDataRequest = GenerateDataRequest(corpActInfo, ref dictHeaders);
                RBbgUtils.InsertBBGAuditHeaders(timeStamp, dictHeaders);
                mLogger.Debug("HEAVY REQUEST SUBMIT");
                sbmtDataResponse = service.submitGetActionsRequest(sbmtDataRequest);

                rtrvDataRequest.retrieveGetActionsRequest = new RetrieveGetActionsRequest();
                rtrvDataRequest.retrieveGetActionsRequest.responseId = sbmtDataResponse.submitGetActionsResponse.responseId;
                try
                {
                    mLogger.Debug("HEAVY REQUEST HEADERS JSON--> " + new JavaScriptSerializer() { MaxJsonLength = 2147483647 }.Serialize(sbmtDataRequest));
                    mLogger.Debug("HEAVY RESPONSE JSON--> " + new JavaScriptSerializer() { MaxJsonLength = 2147483647 }.Serialize(sbmtDataResponse));
                }
                catch (Exception ex) { }
                mLogger.Debug("HEAVY REQUEST HEADERS --> START");
                mLogger.Debug("USER NUMBER : " + sbmtDataRequest.submitGetActionsRequest.headers.usernumber);
                mLogger.Debug("WS : " + sbmtDataRequest.submitGetActionsRequest.headers.ws);
                mLogger.Debug("SN : " + sbmtDataRequest.submitGetActionsRequest.headers.sn);
                mLogger.Debug("responseId : " + rtrvDataRequest.retrieveGetActionsRequest.responseId);
                mLogger.Debug("HEAVY REQUEST HEADERS --> END");
                GetFieldsData(rtrvDataRequest, service, corpActInfo);
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
                mLogger.Debug("End ->getting securities");
                if (sbmtDataRequest != null)
                    sbmtDataRequest = null;
                if (rtrvDataRequest != null)
                    rtrvDataRequest = null;
                if (sbmtDataResponse != null)
                    sbmtDataResponse = null;
            }
        }

        private submitGetActionsRequestRequest GenerateDataRequest(RCorpActInfo corpActInfo, ref Dictionary<string, string> dictHeaders)
        {
            GetActionsHeaders getDataHeaders = null;
            mLogger.Debug("Start ->generating data request.");
            try
            {
                //Setting request header information	
                getDataHeaders = new GetActionsHeaders();
                sbmtDataRequest = new submitGetActionsRequestRequest();
                sbmtDataRequest.submitGetActionsRequest = new SubmitGetActionsRequest();

                Instruments instrs = new Instruments();

                RCorpActUtils.GenerateHeaderSection(ref getDataHeaders, corpActInfo, ref dictHeaders);
                sbmtDataRequest.submitGetActionsRequest.headers = getDataHeaders;

                instrs.instrument = RCorpActUtils.GenerateInstruments(corpActInfo);
                sbmtDataRequest.submitGetActionsRequest.instruments = instrs;
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                mLogger.Debug("End ->generating data request.");
                if (getDataHeaders != null)
                    getDataHeaders = null;
            }
            return sbmtDataRequest;
        }

        private void GetFieldsData(retrieveGetActionsResponseRequest rtrvDataRequest,
                                       PerSecurityWS service, RCorpActInfo corpActInfo)
        {
            retrieveGetActionsResponseResponse rtrvDataResponse = null;
            OrderedDictionary respErrorCode = null;
            bool isBulkArray = false;
            XDocument resultant = new XDocument();
            XElement root = new XElement("CorpActionInfo");
            resultant.Add(root);
            mLogger.Debug("Start ->getting fields data");
            try
            {
                string clientName = SRMMTConfig.GetClientName();
                XDocument corpDetails = RCorpActUtils.LoadCorpActionDetails();
                rtrvDataResponse = new retrieveGetActionsResponseResponse();
                respErrorCode = new OrderedDictionary();
                do
                {
                    mLogger.Debug("HEAVY REQUEST POLLING");
                    System.Threading.Thread.Sleep(RVendorHeavyStatusConstant.POLLINTERVAL);
                    rtrvDataResponse = service.retrieveGetActionsResponse(rtrvDataRequest);
                }
                while (rtrvDataResponse.retrieveGetActionsResponse.statusCode.code == RVendorHeavyStatusConstant.DATANOTAVAILABLE);
                mLogger.Debug("HEAVY REQUEST RETRIEVE");
                //Check Response level code//
                if (rtrvDataResponse.retrieveGetActionsResponse.statusCode.code == RVendorHeavyStatusConstant.SUCCESS)
                {
                    try
                    {
                        mLogger.Debug("HEAVY RESPONSE DATA JSON--> " + new JavaScriptSerializer() { MaxJsonLength = 2147483647 }.Serialize(rtrvDataResponse));
                    }
                    catch (Exception ex) { }
                    mLogger.Error("response length=> " + rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas.Length);
                    for (int i = 0; i < rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas.Length; i++)
                    {
                        string instrument = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].instrument.id;
                        XElement instrumentName = null;
                        if (rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data != null && rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data.Length > 0)
                            instrumentName = new XElement(RCorpActionConstant.INSTRUMENT, new XAttribute(RCorpActionConstant.NAME, instrument), new XAttribute(RCorpActionConstant.CORP_ACTION, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.mnemonic));
                        else
                            instrumentName = new XElement(RCorpActionConstant.INSTRUMENT, new XAttribute(RCorpActionConstant.NAME, instrument), new XAttribute(RCorpActionConstant.CORP_ACTION, string.Empty));
                        root.Add(instrumentName);
                        XElement field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERG_COMPANY_ID), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.companyId));
                        instrumentName.Add(field);
                        field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERGSECURITYID), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.securityId));
                        instrumentName.Add(field);
                        field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.RCODE), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].code));
                        instrumentName.Add(field);
                        //Check Field Level Code//
                        if (rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].code == "0")
                        {
                            RCorpActUtils.CreateDataTable(corpActInfo, corpDetails, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.mnemonic);
                            DataRow corpDetailRow = corpActInfo.CorpActResultantDataset.Tables[rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.mnemonic].NewRow();
                            corpDetailRow[RCorpActionConstant.INSTRUMENTNAME] = instrument;
                            corpDetailRow[RCorpActionConstant.BLOOMBERG_COMPANY_ID] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.companyId;
                            corpDetailRow[RCorpActionConstant.BLOOMBERGSECURITYID] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.securityId;
                            corpDetailRow[RCorpActionConstant.RCODE] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].code;

                            if (corpActInfo.ProcessedInstruments.ContainsKey(instrument))
                            {
                                if (!corpActInfo.ProcessedInstruments[instrument].Contains(rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.mnemonic))
                                    corpActInfo.ProcessedInstruments[instrument].Add(rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.mnemonic);
                            }
                            else
                                corpActInfo.ProcessedInstruments.Add(instrument, new List<string> { rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.mnemonic });

                            corpDetailRow[RCorpActionConstant.ACTIONID] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.actionId;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.ACTIONID), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.actionId));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.MNEMONIC] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.mnemonic;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.MNEMONIC), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.mnemonic));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.FLAG] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.flag;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.FLAG), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.flag));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.COMPANYNAME] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.companyName;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.COMPANYNAME), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.companyName));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.SECIDTYPE] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.secIdType;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.SECIDTYPE), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.secIdType));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.SECID] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.secId;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.SECID), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.secId));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.CURRENCY] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.currency;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.CURRENCY), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.currency));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.MARKET_SECTOR] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.marketSectorDes;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.MARKET_SECTOR), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.marketSectorDes));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.BLOOMBERGUNIQUEID] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.bbUnique;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERGUNIQUEID), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.bbUnique));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.ANNDATE] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.announceDate;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.ANNDATE), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.announceDate));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.EFFDATE] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.effectiveDate;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.EFFDATE), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.effectiveDate));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.AMDDATE] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.amendDate;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.AMDDATE), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.amendDate));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.BLOOMBERGGLOBALID] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.bbGlobal;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERGGLOBALID), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.bbGlobal));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.BLOOMBERGGLOBALCOMPANYID] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.bbGlobalCompany;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERGGLOBALCOMPANYID), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.bbGlobalCompany));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.BLOOMBERGSECURITYIDNUMBERDESCRIPTION] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.bbSecNumDes;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERGSECURITYIDNUMBERDESCRIPTION), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.bbSecNumDes));
                            instrumentName.Add(field);
                            corpDetailRow[RCorpActionConstant.FEEDSOURCE] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.feedSource;
                            field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.FEEDSOURCE), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.feedSource));
                            instrumentName.Add(field);

                            mLogger.Error("instrument data length=> " + rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data.Length);
                            for (int j = 0; j < rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data.Length; j++)
                            {
                                //To Do
                                //mLogger.Error(rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data[j].field);
                                //mLogger.Error(rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data[j].value);
                                //vInfo.Name = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data[j].field;
                                //vInfo.Value = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data[j].value;
                                corpDetailRow[rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data[j].field] = rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data[j].value;
                                field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data[j].field), new XAttribute(RCorpActionConstant.VALUE, rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].data[j].value));
                                instrumentName.Add(field);
                            }
                            corpActInfo.CorpActResultantDataset.Tables[rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].standardFields.mnemonic].Rows.Add(corpDetailRow);
                            //foreach (RCorpActSecurityInfo instInfo in corpActInfo.SecurityIds)
                            //{
                            //    if (instInfo.SecurityName.Equals(rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].instrument.id, StringComparison.InvariantCultureIgnoreCase))
                            //    {
                            //        respDict[instInfo.SecurityName] = respList;
                            //        break;
                            //    }
                            //    else if (instInfo.SecurityName.Equals(rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].instrument.id + " " + rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].instrument.yellowkey, StringComparison.InvariantCultureIgnoreCase))
                            //    {
                            //        respDict[instInfo.SecurityName] = respList;
                            //        break;
                            //    }
                            //}
                        }
                        else
                        {
                            mLogger.Error("instrument error code=> " + rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].code);
                            if (RVendorConfigLoader.mVendorConfig[clientName]["BBGErrorCodes"].ContainsKey(
                                                           rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].code))
                            {
                                respErrorCode[rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].code] =
                                    RVendorConfigLoader.mVendorConfig[clientName]["BBGErrorCodes"][rtrvDataResponse.retrieveGetActionsResponse.instrumentDatas[i].code];
                                //InstrumentErrorCode = respErrorCode;
                            }
                        }
                    }
                }
                else if (rtrvDataResponse.retrieveGetActionsResponse.statusCode.code == RVendorHeavyStatusConstant.REQUESTERROR)
                {
                    mLogger.Error("error code=> " + rtrvDataResponse.retrieveGetActionsResponse.statusCode.code);
                    RCorpActUtils.HandleErrorCodes(rtrvDataResponse.retrieveGetActionsResponse.statusCode.code.ToString());
                }
                corpActInfo.CorpActResultantXml = resultant;
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw new RVendorException(rEx.Message);
            }
            catch (Exception ex)
            {
                corpActInfo.HasError = true;
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->getting fields data");
                if (rtrvDataResponse != null)
                    rtrvDataResponse = null;
            }
        }
    }
}
