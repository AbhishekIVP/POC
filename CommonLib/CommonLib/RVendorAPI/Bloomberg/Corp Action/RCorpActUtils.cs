using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using com.ivp.rad.utils;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using com.ivp.rad.dal;
using com.ivp.rad.configurationmanagement;
using com.ivp.srm.vendorapi.ServiceReference1;
using com.ivp.srm.vendorapi.bloombergServices.heavy;
using System.Security.Cryptography.X509Certificates;
using com.ivp.rad.cryptography;
using com.ivp.srmcommon;

namespace com.ivp.srm.vendorapi.bloomberg
{
    class RCorpActUtils
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        static RRSAEncrDecr encDec = new RRSAEncrDecr();
        internal static void WriteSecInfoOnFile(RCorpActInfo corpActInfo, string requestFileName, string responseFileName, string workingDirectory, ref Dictionary<string, string> dictHeaders)
        {
            StringBuilder str = null;
            FileStream fs = null;
            StreamWriter sw = null;
            mLogger.Debug("RCorpActUtils:WriteSecInfoOnFile=>Start writing security info on file.");
            try
            {
                str = new StringBuilder();
                str.Append(RCorpActionConstant.STARTOFFILE);
                str.AppendLine();
                str.Append(RCorpActionConstant.FIRMNAME);
                str.Append(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.USER_ID));
                str.AppendLine();
                str.AppendLine(RCorpActionConstant.REPLYFILENAME + responseFileName);

                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "FIRMNAME", RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.USER_ID));
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "REPLYFILENAME", responseFileName);
                if (corpActInfo.corpActions.Count > 0)
                {
                    StringBuilder actions = new StringBuilder();
                    foreach (CorpActions act in corpActInfo.corpActions)
                        actions.Append(act.ToString() + RCorpActionConstant.PIPE);
                    actions = actions.Remove(actions.Length - 1, 1);
                    str.AppendLine("ACTIONS=" + actions);
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "ACTIONS", actions.ToString());
                }
                if (corpActInfo.GetFutureAction)
                {
                    str.AppendLine("ACTIONS_DATE=both");
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "ACTIONS_DATE", "both");
                }
                else
                {
                    str.AppendLine("ACTIONS_DATE=entry");
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "ACTIONS_DATE", "entry");
                }
                str.AppendLine(RCorpActionConstant.DATERANGE + corpActInfo.StartDate.ToString(RCorpActionConstant.YYYYMMDD) + RCorpActionConstant.PIPE + corpActInfo.EndDate.ToString(RCorpActionConstant.YYYYMMDD));
                try
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "DATERANGE", corpActInfo.StartDate.ToString(RCorpActionConstant.YYYYMMDD) + RCorpActionConstant.PIPE + corpActInfo.EndDate.ToString(RCorpActionConstant.YYYYMMDD));
                }
                catch (Exception ex) { }
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "PROGRAMNAME", "getactions");

                HashSet<string> restrictedHeaders = RBbgUtils.GetRestrictedHeaders(RBbgHeaderType.GetActions);
                Dictionary<string, string> HeaderNamesVsValues = RVendorConfigLoader.GetVendorHeaders(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, "GetActions");

                if (HeaderNamesVsValues != null && HeaderNamesVsValues.Count > 0)
                {
                    foreach (var item in HeaderNamesVsValues.Keys)
                    {
                        if (!restrictedHeaders.Contains(item))
                        {
                            RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, item, (string.IsNullOrEmpty(HeaderNamesVsValues[item]) ? "yes" : HeaderNamesVsValues[item]));
                            str.Append(item + "=" + (string.IsNullOrEmpty(HeaderNamesVsValues[item]) ? "yes" : HeaderNamesVsValues[item]));
                            str.AppendLine();
                        }
                        else
                        {
                            mLogger.Error("Header " + item + " ignored as it is restricted");
                        }
                    }
                }

                str.AppendLine("PROGRAMNAME=getactions");
                str.AppendLine(RCorpActionConstant.STARTOFDATA);
                if (corpActInfo.SecurityIds.Count > 0)
                {
                    foreach (RCorpActSecurityInfo secInfo in corpActInfo.SecurityIds)
                    {
                        str.Append(secInfo.SecurityName);
                        if (secInfo.MarcketSector != CorpActMarketSector.None)
                        {
                            str.Append(RCorpActionConstant.WHITESPACE + secInfo.MarcketSector);
                        }
                        str.AppendLine();
                    }
                }
                str.AppendLine(RCorpActionConstant.ENDOFDATA);
                str.Append(RCorpActionConstant.ENDOFFILE);
                fs = new FileStream(Path.Combine(workingDirectory, requestFileName),
                                   FileMode.Create, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);
                sw.Write(str.ToString());
                mLogger.Debug("RCorpActUtils:WriteSecInfoOnFile=>End writing security info on file.");

            }
            catch (Exception ex)
            {
                mLogger.Error("RCorpActUtils:WriteSecInfoOnFile=>" + ex.ToString());
                throw new RVendorException(ex.Message);
            }
            finally
            {
                sw.Close();
                fs.Close();
                if (str != null)
                    str = null;
            }
        }

        internal static void RemoveDirtyEntries(RCorpActInfo corpActInfo)
        {
            try
            {
                mLogger.Error("RCorpActUtils:RemoveDirtyEntries=> Start removing dirty entries");
                Compare comparer = new Compare();
                corpActInfo.SecurityIds = corpActInfo.SecurityIds.Distinct(comparer).ToList();
                corpActInfo.corpActions = corpActInfo.corpActions.Distinct().ToList();
                if (DateTime.Now - corpActInfo.EndDate > new TimeSpan(7, 0, 0, 0) || corpActInfo.EndDate < corpActInfo.StartDate)
                    corpActInfo.EndDate = DateTime.Now;
                if (corpActInfo.StartDate > DateTime.Now)
                    corpActInfo.StartDate = DateTime.Now - new TimeSpan(7, 0, 0, 0);
                mLogger.Error("RCorpActUtils:RemoveDirtyEntries=>End removing dirty entries");
            }
            catch (Exception ex)
            {
                mLogger.Error("RCorpActUtils:RemoveDirtyEntries=>" + ex.ToString());
                throw ex;
            }
        }

        internal static string DecryptOutputFile(string workingDirectory, string responseFileName, string decryptFileName, RCorpActInfo corpActInfo)
        {
            mLogger.Debug("RCorpActUtils:DecryptOutputFile=>Start decrypting output file.");
            try
            {
                RCommandExecutor cmdExec = new RCommandExecutor();
                string decryptCmd = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.FTPDECRYPTCOMMAND);
                string decryptKey = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.FTPDECRPYPTKEY);
                string decryptToolPath = RVendorConfigLoader.GetDecryptToolPath(corpActInfo.VendorPreferenceId);
                if (string.IsNullOrEmpty(decryptToolPath))
                    return responseFileName;

                StringBuilder cmd = new StringBuilder();
                cmd.Append(RCorpActionConstant.WHITESPACE);
                cmd.Append(decryptCmd);
                cmd.Append(RCorpActionConstant.WHITESPACE);
                cmd.Append(RCorpActionConstant.SINGLEQOUTE);
                cmd.Append(decryptKey);
                cmd.Append(RCorpActionConstant.SINGLEQOUTE);
                cmd.Append(RCorpActionConstant.WHITESPACE);
                cmd.Append(responseFileName);
                cmd.Append(RCorpActionConstant.WHITESPACE);
                cmd.Append(decryptFileName);
                cmd.Append(RCorpActionConstant.WHITESPACE);
                cmdExec.ExecuteCommand(decryptToolPath, cmd.ToString(), workingDirectory);
                mLogger.Debug("RCorpActUtils:DecryptOutputFile=>End decrypting output file.");
                return decryptFileName;
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error("RCorpActUtils:DecryptOutputFile=>" + ex.ToString());
                if (ex.Message.ToLower().Contains("process must exit before requested information can be determined"))//&& retryAttemp < 10
                {
                    //Eat the exception
                    return decryptFileName;
                }
                else
                    throw new Exception(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->decrypting output file.");
            }
        }

        internal static void GetXML(string decryptFileName, RCorpActInfo corpActInfo)
        {
            StreamReader sr = null;
            bool startOfData = false;
            List<string> lstLines = null;
            XDocument resultant = new XDocument();
            XElement root = new XElement("CorpActionInfo");
            resultant.Add(root);
            mLogger.Debug("RCorpActUtils:GetXML =>Start reading output file.");

            Dictionary<string, string> DictInstrumentNameVsIdentifier = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (corpActInfo.SecurityIds != null && corpActInfo.SecurityIds.Count > 0)
            {
                foreach (var item in corpActInfo.SecurityIds)
                {
                    string InstrumentName = item.SecurityName;
                    if (item.MarcketSector != CorpActMarketSector.None)
                    {
                        InstrumentName += RCorpActionConstant.WHITESPACE + item.MarcketSector;
                    }
                    DictInstrumentNameVsIdentifier[InstrumentName] = item.SecurityName;
                }
            }
            try
            {
                XDocument corpDetails = LoadCorpActionDetails();
                lstLines = new List<string>();
                sr = new StreamReader(decryptFileName);
                while (!sr.EndOfStream)
                {
                    lstLines.Add(sr.ReadLine());
                }
                for (int j = 0; j < lstLines.Count; j++)
                {
                    if (lstLines[j] == RCorpActionConstant.STARTOFDATA)
                        startOfData = true;
                    if (lstLines[j] == RCorpActionConstant.ENDOFDATA)
                        break;
                    if (startOfData)
                    {
                        if (!lstLines[j].Trim().StartsWith("#"))
                        {
                            string[] data = lstLines[j].Split(new string[] { RCorpActionConstant.PIPE }, StringSplitOptions.None);
                            if (data.Length > 1)
                            {

                                string instrument = data[0];
                                string marketSector = Regex.Match(data[0], "[\\w]+$").Value;
                                if (DictInstrumentNameVsIdentifier.Count > 0 && DictInstrumentNameVsIdentifier.ContainsKey(instrument))
                                {
                                    instrument = DictInstrumentNameVsIdentifier[instrument];
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(marketSector))
                                        instrument = instrument.Replace(marketSector, string.Empty);
                                    instrument = instrument.Trim();
                                }
                                XElement instrumentName = null;
                                if (data.Length > 5)
                                    instrumentName = new XElement(RCorpActionConstant.INSTRUMENT, new XAttribute(RCorpActionConstant.NAME, instrument), new XAttribute(RCorpActionConstant.CORP_ACTION, data[5]));
                                else
                                    instrumentName = new XElement(RCorpActionConstant.INSTRUMENT, new XAttribute(RCorpActionConstant.NAME, instrument), new XAttribute(RCorpActionConstant.CORP_ACTION, string.Empty));
                                root.Add(instrumentName);
                                XElement field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERG_COMPANY_ID), new XAttribute(RCorpActionConstant.VALUE, data[1]));
                                instrumentName.Add(field);
                                field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERGSECURITYID), new XAttribute(RCorpActionConstant.VALUE, data[2]));
                                instrumentName.Add(field);
                                field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.RCODE), new XAttribute(RCorpActionConstant.VALUE, data[3]));
                                instrumentName.Add(field);
                                if (data[3].Equals("0"))
                                {
                                    CreateDataTable(corpActInfo, corpDetails, data[5]);
                                    DataRow corpDetailRow = corpActInfo.CorpActResultantDataset.Tables[data[5]].NewRow();
                                    corpDetailRow[RCorpActionConstant.INSTRUMENTNAME] = instrument;
                                    corpDetailRow[RCorpActionConstant.BLOOMBERG_COMPANY_ID] = data[1];
                                    corpDetailRow[RCorpActionConstant.BLOOMBERGSECURITYID] = data[2];
                                    corpDetailRow[RCorpActionConstant.RCODE] = data[3];

                                    if (corpActInfo.ProcessedInstruments.ContainsKey(instrument))
                                    {
                                        if (!corpActInfo.ProcessedInstruments[instrument].Contains(data[5]))
                                            corpActInfo.ProcessedInstruments[instrument].Add(data[5]);
                                    }
                                    else
                                        corpActInfo.ProcessedInstruments.Add(instrument, new List<string> { data[5] });

                                    corpDetailRow[RCorpActionConstant.ACTIONID] = data[4];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.ACTIONID), new XAttribute(RCorpActionConstant.VALUE, data[4]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.MNEMONIC] = data[5];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.MNEMONIC), new XAttribute(RCorpActionConstant.VALUE, data[5]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.FLAG] = data[6];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.FLAG), new XAttribute(RCorpActionConstant.VALUE, data[6]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.COMPANYNAME] = data[7];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.COMPANYNAME), new XAttribute(RCorpActionConstant.VALUE, data[7]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.SECIDTYPE] = data[8];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.SECIDTYPE), new XAttribute(RCorpActionConstant.VALUE, data[8]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.SECID] = data[9];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.SECID), new XAttribute(RCorpActionConstant.VALUE, data[9]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.CURRENCY] = data[10];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.CURRENCY), new XAttribute(RCorpActionConstant.VALUE, data[10]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.MARKET_SECTOR] = marketSector;// data[11];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.MARKET_SECTOR), new XAttribute(RCorpActionConstant.VALUE, data[11]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.BLOOMBERGUNIQUEID] = data[12];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERGUNIQUEID), new XAttribute(RCorpActionConstant.VALUE, data[12]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.ANNDATE] = data[13];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.ANNDATE), new XAttribute(RCorpActionConstant.VALUE, data[13]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.EFFDATE] = data[14];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.EFFDATE), new XAttribute(RCorpActionConstant.VALUE, data[14]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.AMDDATE] = data[15];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.AMDDATE), new XAttribute(RCorpActionConstant.VALUE, data[15]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.BLOOMBERGGLOBALID] = data[16];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERGGLOBALID), new XAttribute(RCorpActionConstant.VALUE, data[16]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.BLOOMBERGGLOBALCOMPANYID] = data[17];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERGGLOBALCOMPANYID), new XAttribute(RCorpActionConstant.VALUE, data[17]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.BLOOMBERGSECURITYIDNUMBERDESCRIPTION] = data[18];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.BLOOMBERGSECURITYIDNUMBERDESCRIPTION), new XAttribute(RCorpActionConstant.VALUE, data[18]));
                                    instrumentName.Add(field);
                                    corpDetailRow[RCorpActionConstant.FEEDSOURCE] = data[19];
                                    field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, RCorpActionConstant.FEEDSOURCE), new XAttribute(RCorpActionConstant.VALUE, data[19]));
                                    instrumentName.Add(field);
                                    for (int counter = 2; counter <= Convert.ToInt32(data[20]) * 2; counter += 2)
                                    {
                                        corpDetailRow[data[20 + counter - 1]] = data[20 + counter];
                                        field = new XElement(RCorpActionConstant.FIELD, new XAttribute(RCorpActionConstant.NAME, data[20 + counter - 1]), new XAttribute(RCorpActionConstant.VALUE, data[20 + counter]));
                                        instrumentName.Add(field);
                                    }
                                    corpActInfo.CorpActResultantDataset.Tables[data[5]].Rows.Add(corpDetailRow);
                                }
                            }
                        }
                    }
                }
                corpActInfo.CorpActResultantXml = resultant;
                mLogger.Debug("RCorpActUtils:GetXML =>End reading output file.");
            }
            catch (Exception ex)
            {
                corpActInfo.HasError = true;
                mLogger.Error("RCorpActUtils:GetXML =>" + ex.ToString());
                throw ex;
            }
        }

        internal static void CreateDataTable(RCorpActInfo corpActInfo, XDocument corpDetails, string corpAction)
        {
            if (corpActInfo.CorpActResultantDataset.Tables[corpAction] == null)
            {
                IEnumerable corpActions = (IEnumerable)corpDetails.XPathEvaluate("CorpActions/CorpAction");
                foreach (XElement action in corpActions)
                {
                    if (action.Attribute(RCorpActionConstant.NAME).Value.Equals(corpAction))
                    {
                        DataTable corpTable = new DataTable(corpAction);
                        IEnumerable<XElement> fields = action.Elements();
                        AddStaticColumns(corpTable);
                        foreach (XElement field in fields)
                            corpTable.Columns.Add(field.Attribute(RCorpActionConstant.NAME).Value);
                        corpActInfo.CorpActResultantDataset.Tables.Add(corpTable);
                        break;
                    }
                }
            }
        }

        private static void AddStaticColumns(DataTable corpTable)
        {
            corpTable.Columns.Add(RCorpActionConstant.INSTRUMENTNAME);
            corpTable.Columns.Add(RCorpActionConstant.BLOOMBERG_COMPANY_ID);
            corpTable.Columns.Add(RCorpActionConstant.BLOOMBERGSECURITYID);
            corpTable.Columns.Add(RCorpActionConstant.RCODE);
            corpTable.Columns.Add(RCorpActionConstant.ACTIONID);
            corpTable.Columns.Add(RCorpActionConstant.MNEMONIC);
            corpTable.Columns.Add(RCorpActionConstant.FLAG);
            corpTable.Columns.Add(RCorpActionConstant.COMPANYNAME);
            corpTable.Columns.Add(RCorpActionConstant.SECIDTYPE);
            corpTable.Columns.Add(RCorpActionConstant.SECID);
            corpTable.Columns.Add(RCorpActionConstant.CURRENCY);
            corpTable.Columns.Add(RCorpActionConstant.MARKET_SECTOR);
            corpTable.Columns.Add(RCorpActionConstant.BLOOMBERGUNIQUEID);
            corpTable.Columns.Add(RCorpActionConstant.ANNDATE);
            corpTable.Columns.Add(RCorpActionConstant.EFFDATE);
            corpTable.Columns.Add(RCorpActionConstant.AMDDATE);
            corpTable.Columns.Add(RCorpActionConstant.BLOOMBERGGLOBALID);
            corpTable.Columns.Add(RCorpActionConstant.BLOOMBERGGLOBALCOMPANYID);
            corpTable.Columns.Add(RCorpActionConstant.BLOOMBERGSECURITYIDNUMBERDESCRIPTION);
            corpTable.Columns.Add(RCorpActionConstant.FEEDSOURCE);
        }

        internal static XDocument LoadCorpActionDetails()
        {
            Stream stream = null;
            XmlReader reader = null;
            try
            {
                string masterConfigPath = RADConfigReader.GetServerPhysicalPath() + RADConfigReader.GetConfigAppSettings("MasterConfigPath");
                string directory = Path.GetDirectoryName(masterConfigPath);
                return XDocument.Load(Path.Combine(directory, "CorpActionDetails.xml"));
                //stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("com.ivp.srm.vendorapi.Bloomberg.Corp_Action.Resources.CorpActionDetails.xml");
                //reader = XmlReader.Create(stream);
                //return XDocument.Load(reader);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
                stream = null;
                if (reader != null)
                    reader.Close();
                reader = null;
            }
        }

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

        internal static void InsertIntoHistory(RCorpActInfo corpActInfo, string requestId, string timeStamp)
        {
            RDBConnectionManager mDBConn = null;
            try
            {
                mLogger.Debug("RCorpActUtils:InsertIntoHistory=>Start inserting into database");
                StringBuilder instruments = new StringBuilder();
                corpActInfo.SecurityIds.ForEach(q =>
                {
                    instruments.Append(q.SecurityName + RCorpActionConstant.COMMA);
                });
                StringBuilder corpAction = new StringBuilder();
                corpActInfo.corpActions.ForEach(q =>
                {
                    corpAction.Append(q.ToString() + RCorpActionConstant.COMMA);
                });
                string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
                mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                RHashlist mList = new RHashlist();
                mList.Add("vendor_name", RCorpActionConstant.BLOOMBERG);
                mList.Add("request_type", "CorpAction-" + corpActInfo.RequestType.ToString());
                mList.Add("requested_instruments", instruments.ToString());
                mList.Add("request_identifier", requestId);
                mList.Add("time_stamp", timeStamp);
                mList.Add("request_status", RVendorStatusConstant.PENDING);
                mList.Add("user_login_name", corpActInfo.UserLogin);
                mList.Add("module_id", corpActInfo.ModuleID);
                mList.Add("corp_action", corpAction.ToString());
                mDBConn.ExecuteQuery("RAD:InsertCorpActionVendorHistory", mList, true);
                mLogger.Debug("RCorpActUtils:InsertIntoHistory=>End inserting into database");

                if (RVendorUtils.EnableBBGAudit)
                {
                    mLogger.Debug("RCorpActUtils:InsertIntoHistory:InsertBBGAuditCorpAction -> Start");
                    try
                    {
                        var dtSecAuditInfo = new DataTable();
                        dtSecAuditInfo.Columns.Add("identifier_value", typeof(string));
                        dtSecAuditInfo.Columns.Add("identifier_name", typeof(string));
                        dtSecAuditInfo.Columns.Add("yellow_key_name", typeof(string));
                        dtSecAuditInfo.Columns.Add("start_date", typeof(DateTime));
                        dtSecAuditInfo.Columns.Add("end_date", typeof(DateTime));

                        var dtCorpActionTypeInfo = new DataTable();
                        dtCorpActionTypeInfo.Columns.Add("corpaction_type_name", typeof(string));

                        List<string> marketSectors = Enum.GetNames(typeof(RBbgMarketSector)).Select(x => x.ToLower()).ToList();
                        foreach (var instrumentInfo in corpActInfo.SecurityIds)
                        {
                            string marketSector = string.Empty;
                            string identifierValue = string.Empty;
                            if (instrumentInfo.MarcketSector == CorpActMarketSector.None)
                            {
                                RBbgUtils.GetMarketSectorFromInstrument(instrumentInfo.SecurityName, marketSectors, out identifierValue, out marketSector);
                            }
                            else
                            {
                                marketSector = instrumentInfo.MarcketSector.ToString();
                                identifierValue = instrumentInfo.SecurityName;
                            }

                            var drSecAudit = dtSecAuditInfo.NewRow();
                            drSecAudit["identifier_value"] = identifierValue;
                            drSecAudit["identifier_name"] = corpActInfo.RequestType == RBbgCorpActRequestType.FTP ? RBbgInstrumentIdType.BB_GLOBAL.ToString() : corpActInfo.InstrumentType.ToString();
                            drSecAudit["yellow_key_name"] = marketSector;
                            dtSecAuditInfo.Rows.Add(drSecAudit);
                        }

                        foreach (var corpActionType in corpActInfo.corpActions)
                        {
                            var drCorpActionType = dtCorpActionTypeInfo.NewRow();
                            drCorpActionType["corpaction_type_name"] = corpActionType.ToString();

                            dtCorpActionTypeInfo.Rows.Add(drCorpActionType);
                        }

                        if (dtSecAuditInfo.Rows.Count > 0 && dtCorpActionTypeInfo.Rows.Count > 0)
                        {
                            RVendorUtils.InsertBBGAuditCorpAction(timeStamp, dtSecAuditInfo, dtCorpActionTypeInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("RCorpActUtils:InsertIntoHistory:InsertBBGAuditCorpAction -> " + ex.ToString());
                    }
                    finally
                    {
                        mLogger.Debug("RCorpActUtils:InsertIntoHistory:InsertBBGAuditCorpAction -> End");
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("RCorpActUtils:InsertIntoHistory=> " + ex.Message);
                throw ex;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("End->Insert into vendor History");
            }
        }

        internal static void UpdateVendorHistory(string instruments, string timeStamp, string status)
        {
            mLogger.Debug("RCorpActUtils:UpdateVendorHistory->Insert into vendor History");
            RDBConnectionManager mDBConn = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
            try
            {
                mDBConn.ExecuteQuery("Update ivp_rad_vendor_Corp_Action_history set request_status='" + status +
                    "'" + ",processed_instruments=" + "'" + instruments + "'" +
                    "  where time_stamp = '" + timeStamp + "'", RQueryType.Update);
            }
            catch (RDALException dalEx)
            {
                mLogger.Error(dalEx.ToString());
                throw new RVendorException(dalEx);
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDBConn);
                mLogger.Debug("End->Insert into vendor History");
            }
        }

        public static PerSecurityWS GetHeavyService(RCorpActInfo corpActInfo)
        {
            PerSecurityWSClient ps = null;
            mLogger.Debug("Start -> retrieving Heavy Service");
            try
            {
                ps = new PerSecurityWSClient("PerSecurityWSPort");

                string serverPath = RADConfigReader.GetServerPhysicalPath();
                string filePath = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.CERTIFICATEPATH);
                //ps.Url = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg)[RVendorConstant.WEBSERVICEURLHEAVY];
                string password = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, RVendorConstant.PASSWORD);
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

        internal static void GenerateHeaderSection(ref GetActionsHeaders getDataHeaders, RCorpActInfo corpActInfo, ref Dictionary<string, string> dictHeaders)
        {
            getDataHeaders.actions = corpActInfo.corpActions.Select(x => x.ToString()).ToArray();
            getDataHeaders.actions_date = ActionsDate.entry;
            getDataHeaders.actions_dateSpecified = true;
            //getDataHeaders.dateformat = DateFormat.yyyymmdd;
            //getDataHeaders.dateformatSpecified = true;
            getDataHeaders.daterange = new DateRange() { period = new Period { start = corpActInfo.StartDate, end = corpActInfo.EndDate } };
            getDataHeaders.secid = (InstrumentType)Enum.Parse(typeof(InstrumentType), corpActInfo.InstrumentType.ToString());
            getDataHeaders.secidSpecified = true;

            try
            {
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "ACTIONS", string.Join("|", corpActInfo.corpActions.Select(x => x.ToString())));
            }
            catch (Exception ex) { }
            RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "ACTIONS_DATE", ActionsDate.entry.ToString());
            try
            {
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "DATERANGE", corpActInfo.StartDate.ToString(RCorpActionConstant.YYYYMMDD) + RCorpActionConstant.PIPE + corpActInfo.EndDate.ToString(RCorpActionConstant.YYYYMMDD));
            }
            catch (Exception ex) { }
            try
            {
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "SECID", ((InstrumentType)Enum.Parse(typeof(InstrumentType), corpActInfo.InstrumentType.ToString())).ToString());
            }
            catch (Exception ex) { }

            HashSet<string> restrictedHeaders = RBbgUtils.GetRestrictedHeaders(RBbgHeaderType.Corpaction);
            Dictionary<string, string> HeaderNamesVsValues = RVendorConfigLoader.GetVendorHeaders(RVendorType.Bloomberg, corpActInfo.VendorPreferenceId, "Corpaction");

            if (HeaderNamesVsValues != null && HeaderNamesVsValues.Count > 0)
            {
                var propInfoAll = typeof(GetActionsHeaders).GetProperties();
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
                                var propSpecified = typeof(GetActionsHeaders).GetProperty(specifiedPropertyName);
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

        internal static Instrument[] GenerateInstruments(RCorpActInfo corpActInfo)
        {
            //Setting Instrument information
            Instrument[] instruments = new Instrument[corpActInfo.SecurityIds.Count];
            int i = 0;
            foreach (RCorpActSecurityInfo inst in corpActInfo.SecurityIds)
            {
                instruments[i] = new Instrument();
                instruments[i].id = inst.SecurityName;
                instruments[i].type = (InstrumentType)Enum.Parse(typeof(InstrumentType), corpActInfo.InstrumentType.ToString());
                instruments[i].typeSpecified = true;
                if (inst.MarcketSector != CorpActMarketSector.None)
                {
                    instruments[i].yellowkey = (com.ivp.srm.vendorapi.bloombergServices.heavy.MarketSector)Enum.Parse(typeof(com.ivp.srm.vendorapi.bloombergServices.heavy.MarketSector), inst.MarcketSector.ToString());
                    instruments[i].yellowkeySpecified = true;
                }
                i++;
            }
            return instruments;
        }

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
    }

    class Compare : IEqualityComparer<RCorpActSecurityInfo>
    {

        public bool Equals(RCorpActSecurityInfo x, RCorpActSecurityInfo y)
        {
            return x.SecurityName.Equals(y.SecurityName);
        }

        public int GetHashCode(RCorpActSecurityInfo obj)
        {
            return base.GetHashCode();
        }

    }
}
