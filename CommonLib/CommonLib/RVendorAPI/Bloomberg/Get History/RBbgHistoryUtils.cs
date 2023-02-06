using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using System.IO;
using com.ivp.rad.utils;
using System.IO.Compression;
using System.Data;
using com.ivp.rad.dal;
using com.ivp.rad.configurationmanagement;
using System.Xml.Linq;

namespace com.ivp.srm.vendorapi.bloomberg
{
    class RBbgHistoryUtils
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RBbgHistoryUtils");

        internal static void RemoveDirtyEntries(RBbgHistoryInfo historyInfo)
        {
            try
            {
                mLogger.Error("RBbgHistoryUtils:RemoveDirtyEntries=> Start removing dirty entries");
                historyInfo.SecurityIds = historyInfo.SecurityIds.Distinct(new Distinct()).ToList();
                historyInfo.Fields = historyInfo.Fields.Distinct().ToList();
                if (historyInfo.EndDate < historyInfo.StartDate)
                    historyInfo.EndDate = historyInfo.StartDate + new TimeSpan(1, 0, 0, 0);
                mLogger.Error("RBbgHistoryUtils:RemoveDirtyEntries=>End removing dirty entries");
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgHistoryUtils:RemoveDirtyEntries=>" + ex.ToString());
                throw ex;
            }
        }

        internal static void InsertIntoHistory(RBbgHistoryInfo historyInfo, string requestId, string timeStamp)
        {
            try
            {
                mLogger.Debug("RBbgHistoryUtils:InsertIntoHistory=>Start inserting into database");
                RVendorHistoryInfo info = new RVendorHistoryInfo();
                info.IsBulkList = false;
                info.ModuleId = historyInfo.ModuleID;
                info.UserLoginName = historyInfo.UserLogin;
                info.TimeStamp = timeStamp;
                info.requestIdentifier = requestId;
                info.RequestStatus = RVendorStatusConstant.PENDING;
                info.RequestType = "GetHistory-" + historyInfo.RequestType.ToString();// RCorpActionConstant.GETHISTORYFTP
                StringBuilder instruments = new StringBuilder();
                historyInfo.SecurityIds.ForEach(q =>
                {
                    instruments.Append(q.SecurityName + RCorpActionConstant.COMMA);
                });
                info.VendorInstruments = instruments.ToString();
                info.VendorName = RCorpActionConstant.BLOOMBERG;
                InsertHistory(info);

                if (RVendorUtils.EnableBBGAudit)
                {
                    mLogger.Debug("RBbgHistoryUtils:InsertIntoHistory:InsertBBGAuditHistory -> Start");
                    try
                    {
                        var dtSecAuditInfo = new DataTable();
                        dtSecAuditInfo.Columns.Add("identifier_value", typeof(string));
                        dtSecAuditInfo.Columns.Add("identifier_name", typeof(string));
                        dtSecAuditInfo.Columns.Add("yellow_key_name", typeof(string));
                        dtSecAuditInfo.Columns.Add("start_date", typeof(DateTime));
                        dtSecAuditInfo.Columns.Add("end_date", typeof(DateTime));

                        var dtMnemonicAuditInfo = new DataTable();
                        dtMnemonicAuditInfo.Columns.Add("mnemonic_name", typeof(string));

                        List<string> marketSectors = Enum.GetNames(typeof(RBbgMarketSector)).Select(x => x.ToLower()).ToList();
                        foreach (var instrumentInfo in historyInfo.SecurityIds)
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
                            drSecAudit["identifier_name"] = instrumentInfo.securityType == RBbgHistoryInstrumentIdType.NONE ? string.Empty : instrumentInfo.securityType.ToString();
                            drSecAudit["yellow_key_name"] = marketSector;
                            if (instrumentInfo.dateProvided)
                            {
                                if (instrumentInfo.EndDate < instrumentInfo.StartDate)
                                    instrumentInfo.EndDate = instrumentInfo.StartDate + new TimeSpan(1, 0, 0, 0);
                                drSecAudit["start_date"] = instrumentInfo.StartDate;
                                drSecAudit["end_date"] = instrumentInfo.EndDate;
                            }
                            dtSecAuditInfo.Rows.Add(drSecAudit);
                        }

                        foreach (string mnemonic in historyInfo.Fields)
                        {
                            var drMnemonic = dtMnemonicAuditInfo.NewRow();
                            drMnemonic["mnemonic_name"] = mnemonic;

                            dtMnemonicAuditInfo.Rows.Add(drMnemonic);
                        }

                        if (dtSecAuditInfo.Rows.Count > 0)
                        {
                            RVendorUtils.InsertBBGAuditHist(timeStamp, dtSecAuditInfo, dtMnemonicAuditInfo);
                        }
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("RBbgHistoryUtils:InsertIntoHistory:InsertBBGAuditHistory -> " + ex.ToString());
                    }
                    finally
                    {
                        mLogger.Debug("RBbgHistoryUtils:InsertIntoHistory:InsertBBGAuditHistory -> End");
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgHistoryUtils:InsertIntoHistory=> " + ex.Message);
                throw ex;
            }
        }

        internal static void InsertHistory(RVendorHistoryInfo info)
        {
            mLogger.Debug("Start->Insert into vendor History");
            RDBConnectionManager mDBConn = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
            RHashlist mList = new RHashlist();
            mList.Add("vendor_name", info.VendorName);
            mList.Add("request_type", info.RequestType);
            mList.Add("requested_instruments", info.VendorInstruments);
            mList.Add("requested_fields", info.VendorFields);
            mList.Add("request_identifier", info.requestIdentifier);
            mList.Add("time_stamp", info.TimeStamp);
            mList.Add("request_status", info.RequestStatus);
            mList.Add("user_login_name", info.UserLoginName);
            mList.Add("is_bulk_list", info.IsBulkList);
            mList.Add("module_id", info.ModuleId);
            //mList.Add("market_sector", info.MarketSector);
            //mList.Add("request_module", "Historical");
            try
            {
                mDBConn.ExecuteQuery("RAD:InsertVendorHistHistory", mList, true);
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

        internal static void WriteSecInfoOnFile(RBbgHistoryInfo historyInfo, string requestFileName, string responseFileName, string workingDirectory, ref Dictionary<string, string> dictHeaders)
        {
            StringBuilder str = null;
            FileStream fs = null;
            StreamWriter sw = null;
            mLogger.Debug("RBbgHistoryUtils:WriteSecInfoOnFile=>Start writing security info on file.");
            try
            {
                str = new StringBuilder();
                str.Append(RCorpActionConstant.STARTOFFILE);
                str.AppendLine();

                HashSet<string> restrictedHeaders = RBbgUtils.GetRestrictedHeaders(RBbgHeaderType.GetHistory);
                Dictionary<string, string> HeaderNamesVsValues = RVendorConfigLoader.GetVendorHeaders(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, "GetHistory");

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

                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "FIRMNAME", RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.USER_ID));
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "COMPRESS", "yes");
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "REPLYFILENAME", responseFileName);
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "DATERANGE", historyInfo.StartDate.ToString(RCorpActionConstant.YYYYMMDD) + RCorpActionConstant.PIPE + historyInfo.EndDate.ToString(RCorpActionConstant.YYYYMMDD));
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "HIST_FORMAT", "horizontal");
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "PROGRAMNAME", "gethistory");
                str.Append(RCorpActionConstant.FIRMNAME);
                str.Append(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.USER_ID));
                str.AppendLine();
                str.AppendLine("COMPRESS=yes");
                str.AppendLine(RCorpActionConstant.REPLYFILENAME + responseFileName);
                str.AppendLine(RCorpActionConstant.DATERANGE + historyInfo.StartDate.ToString(RCorpActionConstant.YYYYMMDD) + RCorpActionConstant.PIPE + historyInfo.EndDate.ToString(RCorpActionConstant.YYYYMMDD));
                str.AppendLine("HIST_FORMAT=horizontal");
                // str.AppendLine("DERIVED=yes");
                str.AppendLine("PROGRAMNAME=gethistory");
                str.AppendLine(RCorpActionConstant.STARTOFFIELDS);
                if (historyInfo.Fields.Count > 0)
                    historyInfo.Fields.ForEach(q => str.AppendLine(q));
                str.AppendLine(RCorpActionConstant.ENDOFFIELDS);
                str.AppendLine(RCorpActionConstant.STARTOFDATA);
                if (historyInfo.SecurityIds.Count > 0)
                {
                    foreach (RHistorySecurityInfo secInfo in historyInfo.SecurityIds)
                    {
                        if (secInfo.securityType == RBbgHistoryInstrumentIdType.TICKER)
                        {
                            if (secInfo.MarcketSector != CorpActMarketSector.None)
                                str.Append(secInfo.SecurityName + RCorpActionConstant.WHITESPACE + secInfo.MarcketSector);
                            else
                                str.Append(secInfo.SecurityName + RCorpActionConstant.WHITESPACE);
                            if (secInfo.dateProvided)
                            {
                                if (secInfo.EndDate < secInfo.StartDate)
                                    secInfo.EndDate = secInfo.StartDate + new TimeSpan(1, 0, 0, 0);
                                str.Append("||" + secInfo.StartDate.ToString("yyyMMdd") + "|" + secInfo.EndDate.ToString("yyyMMdd") + "|");
                            }
                        }
                        else
                        {
                            str.Append(secInfo.SecurityName + RCorpActionConstant.PIPE + secInfo.securityType.ToString());
                            if (secInfo.dateProvided)
                            {
                                if (secInfo.EndDate < secInfo.StartDate)
                                    secInfo.EndDate = secInfo.StartDate + new TimeSpan(1, 0, 0, 0);
                                str.Append("|" + secInfo.StartDate.ToString("yyyMMdd") + "|" + secInfo.EndDate.ToString("yyyMMdd") + "|");
                            }
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
                mLogger.Debug("RBbgHistoryUtils:WriteSecInfoOnFile=>End writing security info on file.");
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgHistoryUtils:WriteSecInfoOnFile=>" + ex.ToString());
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

        internal static string DecryptOutputFile(string workingDirectory, string responseFileName, string decryptFileName, RBbgHistoryInfo historyInfo)
        {
            mLogger.Debug("RBbgHistoryUtils:DecryptOutputFile=>Start decrypting and unzip output file.");
            try
            {
                decryptFileName = DecryptFile(workingDirectory, responseFileName, decryptFileName, historyInfo);
                UnZipFile(workingDirectory, decryptFileName);
                mLogger.Debug("RBbgHistoryUtils:DecryptOutputFile=>End decrypting and unzip output file.");
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgHistoryUtils:DecryptOutputFile=>" + ex.ToString());
                if (ex.Message.ToLower().Contains("process must exit before requested information can be determined"))//&& retryAttemp < 10
                {
                    //Eat the exception
                }
                else
                    throw new Exception(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->decrypting output file.");
            }
            return decryptFileName;
        }

        private static void UnZipFile(string workingDirectory, string decryptFileName)
        {
            //SevenZipExtractor extractor = null;
            //FileStream stream = null;
            try
            {
                mLogger.Debug("RBbgHistoryUtils:UnZipFile=>start unzipping file");
                //SevenZipExtractor.SetLibraryPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\7z.dll"));
                //extractor = new SevenZipExtractor(Path.Combine(workingDirectory, decryptFileName + ".gz"));
                //stream = new FileStream(Path.Combine(workingDirectory, decryptFileName + ".txt"), FileMode.Create);
                //extractor.ExtractFile(0, stream);

                FileInfo fInfo = new FileInfo(Path.Combine(workingDirectory, decryptFileName + ".gz"));
                Decompress(fInfo, Path.Combine(workingDirectory, decryptFileName + ".txt"));

                mLogger.Debug("RBbgHistoryUtils:UnZipFile=>end unzipping file");
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgHistoryUtils:UnZipFile=>" + ex.ToString());
                throw ex;
            }
            finally
            {
                //if (extractor != null)
                //    extractor.Dispose();
                //extractor = null;
                //if (stream != null)
                //    stream.Dispose();
                //stream = null;
            }
        }

        private static void Decompress(FileInfo fi, string decompressedFileName)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                //Create the decompressed file.
                using (FileStream outFile = File.Create(decompressedFileName))
                {
                    using (GZipStream Decompress = new GZipStream(inFile, System.IO.Compression.CompressionMode.Decompress))
                    {
                        //Copy the decompression stream into the output file.
                        byte[] buffer = new byte[4096];
                        int numRead;
                        while ((numRead = Decompress.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            outFile.Write(buffer, 0, numRead);
                        }
                        mLogger.Debug(string.Format("Decompressed: {0}", fi.Name));
                    }
                }
            }
        }

        internal static void GetData(string outputFilePath, RBbgHistoryInfo historyInfo)
        {
            StreamReader sr = null;
            bool startOfData = false;
            List<string> lstLines = null;

            mLogger.Debug("RBbgHistoryUtils:GetData =>Start reading output file.");
            try
            {
                lstLines = new List<string>();
                sr = new StreamReader(outputFilePath);
                DataTable resultantTable = CreateDataTable(historyInfo.Fields);
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
                        string[] data = lstLines[j].Split(new string[] { RCorpActionConstant.PIPE }, StringSplitOptions.None);
                        if (data.Length > 1)
                        {
                            string instrument = string.Empty;
                            string marketSector = string.Empty;
                            foreach (RHistorySecurityInfo info in historyInfo.SecurityIds)
                            {
                                if ((info.SecurityName.Trim() + RCorpActionConstant.WHITESPACE + info.MarcketSector.ToString().Trim()).Equals(data[0].Trim()))
                                {
                                    instrument = info.SecurityName;
                                    marketSector = info.MarcketSector.ToString();
                                    break;
                                }
                                else if (info.SecurityName.Trim().Equals(data[0].Trim()))
                                {
                                    instrument = info.SecurityName;
                                    marketSector = string.Empty;
                                }
                            }
                            DataRow row = resultantTable.NewRow();
                            row[0] = instrument;
                            row[1] = marketSector;
                            if (data[1].Equals("0"))
                            {
                                if (!historyInfo.ProcessedInstruments.Contains(instrument))
                                    historyInfo.ProcessedInstruments.Add(instrument);
                                row[2] = RVendorStatusConstant.PASSED;
                                row[3] = data[3];
                                for (int count = 4; count < historyInfo.Fields.Count + 4; count++)
                                {
                                    row[count] = data[count];
                                    if (!historyInfo.ProcessedFields.Contains(historyInfo.Fields[count - 4]) && !data[count].Equals(RCorpActionConstant.NA))
                                        historyInfo.ProcessedFields.Add(historyInfo.Fields[count - 4]);
                                }
                            }
                            else
                            {
                                row[2] = RVendorStatusConstant.FAILED;
                            }
                            resultantTable.Rows.Add(row);
                        }
                    }
                }
                historyInfo.ResultantData.Tables.Add(resultantTable);
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgHistoryUtils:GetData =>" + ex.ToString());
                throw ex;
            }
        }

        internal static string PrepareXML(RBbgHistoryInfo historyInfo, string timeStamp)
        {
            mLogger.Debug("begin prepare xml");
            XDocument responseXml = new XDocument();
            XElement root = new XElement("Response");
            responseXml.Add(root);
            string fields = string.Join(",", historyInfo.ProcessedFields);
            foreach (var instruments in historyInfo.ProcessedInstruments)
            {
                //StringBuilder fields = new StringBuilder();
                //instruments.Value.ForEach(q => fields.Append("," + q.Name));
                // if (fields.Length > 0)
                // fields = fields.Replace(',', ' ', fields.Length - 1, 1);
                XElement securityRow = new XElement("Security", new XAttribute("Name", instruments),
                                                         new XAttribute("SecurityType", "10"),
                                                         new XAttribute("Fields", fields.Trim()),
                                                         new XAttribute("RequestTime", timeStamp));
                root.Add(securityRow);
            }
            mLogger.Debug("end prepare xml");
            return responseXml.ToString();
        }

        private static DataTable CreateDataTable(List<string> Fields)
        {
            DataTable resultantTable = new DataTable();
            resultantTable.Columns.Add(RCorpActionConstant.INSTRUMENT);
            resultantTable.Columns.Add(RCorpActionConstant.MARKET_SECTOR);
            resultantTable.Columns.Add(RCorpActionConstant.IS_VALID);
            resultantTable.Columns.Add(RCorpActionConstant.DATEOFELEMENT);
            Fields.ForEach(q => resultantTable.Columns.Add(q));
            return resultantTable;
        }

        private static string DecryptFile(string workingDirectory, string responseFileName, string decryptFileName, RBbgHistoryInfo historyInfo)
        {
            try
            {
                mLogger.Debug("RBbgHistoryUtils:DecryptFile=>start decrypting file");
                RCommandExecutor cmdExec = new RCommandExecutor();
                string decryptCmd = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.FTPDECRYPTCOMMAND);
                string decryptKey = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, historyInfo.VendorPreferenceId, RVendorConstant.FTPDECRPYPTKEY);
                string decryptToolPath = RVendorConfigLoader.GetDecryptToolPath(historyInfo.VendorPreferenceId);
                if (string.IsNullOrEmpty(decryptToolPath))
                    return responseFileName.Substring(0, responseFileName.Length - 3);
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
                cmd.Append(decryptFileName + RCorpActionConstant.DOTGZ);
                cmd.Append(RCorpActionConstant.WHITESPACE);
                cmdExec.ExecuteCommand(decryptToolPath, cmd.ToString(), workingDirectory);
                mLogger.Debug("RBbgHistoryUtils:DecryptFile=>end decrypting file");
                return decryptFileName;
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgHistoryUtils:DecryptFile=>" + ex.ToString());
                throw ex;
            }
        }

        internal static void UpdateHistoryDetails(string xml, string appenTime)
        {
            mLogger.Debug("Start->Insert into vendor History");
            RDBConnectionManager mDBConn = null;
            string mDBConnectionId = RADConfigReader.GetConfigAppSettings("RADDBConnectionId");
            mDBConn = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
            try
            {
                mDBConn.ExecuteQuery(" update ivp_rad_vendor_historical_pricing_details set response_structure = '" + xml + "' where time_stamp = '" + appenTime + "'", RQueryType.Update);

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
    }

    class Distinct : IEqualityComparer<RHistorySecurityInfo>
    {

        #region IEqualityComparer<RHistorySecurityInfo> Members

        public bool Equals(RHistorySecurityInfo x, RHistorySecurityInfo y)
        {
            if (x.SecurityName == y.SecurityName)
                return true;
            else
                return false;
        }

        public int GetHashCode(RHistorySecurityInfo obj)
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
