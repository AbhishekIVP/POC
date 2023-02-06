using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.common;
using com.ivp.srm.vendorapi.bloomberg;
using com.ivp.rad.dal;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.utils;
using System.IO;
using System.Data;

namespace com.ivp.srm.vendorapi.Bloomberg.Fundamentals
{
    internal class RBbgFundamentalUtils
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RBbgFundamentalUtils");

        internal static void RemoveDirtyEntries(RBbgFundamentalInfo secInfo)
        {
            try
            {
                mLogger.Error("RBbgFundamentalUtils:RemoveDirtyEntries=> Start removing dirty entries");
                secInfo.Instruments = secInfo.Instruments.Distinct(new DistinctInstrument()).ToList();
                secInfo.InstrumentFields = secInfo.InstrumentFields.Distinct(new DistinctField()).ToList();
                for (int i = 0; i < secInfo.InstrumentFields.Count; i++)
                {
                    if (secInfo.InstrumentFields[i].Mnemonic.Equals("FISCAL_YEAR_PERIOD", StringComparison.InvariantCultureIgnoreCase))
                    {
                        secInfo.InstrumentFields.RemoveAt(i);
                        break;
                    }
                }
                RBbgFieldInfo fldInfo = new RBbgFieldInfo();
                fldInfo.Mnemonic = "FISCAL_YEAR_PERIOD";
                secInfo.InstrumentFields.Insert(0, fldInfo);
                if (secInfo.EndDate < secInfo.StartDate && secInfo.DateProvided)
                    secInfo.EndDate = secInfo.StartDate + new TimeSpan(1, 0, 0, 0);
                mLogger.Error("RBbgFundamentalUtils:RemoveDirtyEntries=>End removing dirty entries");
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgFundamentalUtils:RemoveDirtyEntries=>" + ex.ToString());
                throw ex;
            }
        }

        internal static void InsertIntoDatabase(RBbgFundamentalInfo secInfo, string requestId, string timeStamp)
        {
            try
            {
                mLogger.Debug("RBbgFundamentalUtils:InsertIntoHistory=>Start inserting into database");
                RVendorHistoryInfo info = new RVendorHistoryInfo();
                info.IsBulkList = false;
                info.ModuleId = secInfo.ModuleId;
                info.UserLoginName = secInfo.UserLogin;
                info.TimeStamp = timeStamp;
                info.requestIdentifier = requestId;
                info.RequestStatus = RVendorStatusConstant.PENDING;
                info.RequestType = "Fundamental-FTP";
                StringBuilder instruments = new StringBuilder();
                secInfo.Instruments.ForEach(q =>
                {
                    instruments.Append(q.InstrumentID + RCorpActionConstant.COMMA);
                });
                info.VendorFields = GetCSVFields(secInfo.InstrumentFields);
                info.VendorInstruments = instruments.ToString();
                if (info.VendorInstruments != string.Empty)
                    info.VendorInstruments = info.VendorInstruments.Remove(0, 1);
                info.VendorName = RCorpActionConstant.BLOOMBERG;
                InsertHistory(info);
                mLogger.Debug("RBbgFundamentalUtils:InsertIntoHistory=>End inserting into database");
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgFundamentalUtils:InsertIntoHistory=> " + ex.Message);
                throw ex;
            }
        }

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
                mDBConn.ExecuteQuery("RAD:InsertVendorHistory", mList, true);
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

        internal static void WriteSecInfoOnFile(RBbgFundamentalInfo secInfo, string requestFileName, string responseFileName, string workingDirectory)
        {
            StringBuilder str = null;
            FileStream fs = null;
            StreamWriter sw = null;
            mLogger.Debug("Start ->writing security info on file.");
            try
            {
                str = new StringBuilder();
                str.Append("START-OF-FILE");
                str.AppendLine();

                HashSet<string> restrictedHeaders = RBbgUtils.GetRestrictedHeaders(RBbgHeaderType.GetFundamentals);
                Dictionary<string, string> HeaderNamesVsValues = RVendorConfigLoader.GetVendorHeaders(RVendorType.Bloomberg, secInfo.VendorPreferenceId, "GetFundamentals");

                bool isProgramFlagSet = false;
                if (HeaderNamesVsValues != null && HeaderNamesVsValues.Count > 0)
                {
                    foreach (var item in HeaderNamesVsValues.Keys)
                    {
                        if (!restrictedHeaders.Contains(item))
                        {
                            str.Append(item + "=" + (string.IsNullOrEmpty(HeaderNamesVsValues[item]) ? "yes" : HeaderNamesVsValues[item]));
                            str.AppendLine();
                            if (item.Equals("PROGRAMFLAG", StringComparison.OrdinalIgnoreCase))
                                isProgramFlagSet = true;
                        }
                        else
                        {
                            mLogger.Error("Header " + item + " ignored as it is restricted");
                        }
                    }
                }

                str.Append("PROGRAMNAME=getfundamentals");
                str.AppendLine();
                str.Append("FIRMNAME=");
                str.Append(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.USER_ID));
                str.AppendLine();
                str.Append("FILETYPE=pc");
                str.AppendLine();
                str.Append("REPLYFILENAME=" + responseFileName);
                str.AppendLine();
                str.Append(RCorpActionConstant.DATERANGE + secInfo.StartDate.ToString(RCorpActionConstant.YYYYMMDD) + RCorpActionConstant.PIPE + secInfo.EndDate.ToString(RCorpActionConstant.YYYYMMDD));
                str.AppendLine();
                str.Append("PERIODICITY=" + secInfo.Periodicity.ToString());
                str.AppendLine();
                str.Append("YELLOWKEY=");
                str.Append(secInfo.MarketSector);
                str.AppendLine();
                if (secInfo.Instruments.Count > 0 && secInfo.Instruments[0].InstrumentIdType != RBbgInstrumentIdType.NONE)
                {
                    str.Append("SECID=");
                    str.Append(secInfo.Instruments[0].InstrumentIdType);
                }
                str.AppendLine();
                if (!isProgramFlagSet)
                {
                    str.Append("PROGRAMFLAG=one-shot");
                    str.AppendLine();
                }
                str.Append("START-OF-FIELDS");
                str.AppendLine();
                foreach (RBbgFieldInfo info in secInfo.InstrumentFields)
                {
                    str.Append(info.Mnemonic);
                    str.AppendLine();
                }
                str.Append("END-OF-FIELDS");
                str.AppendLine();
                str.AppendLine();
                str.Append("START-OF-DATA");
                str.AppendLine();
                foreach (RBbgInstrumentInfo objInfo in secInfo.Instruments)
                {
                    StringBuilder overridesValue = new StringBuilder();
                    str.Append(objInfo.InstrumentID);
                    if (objInfo.MarketSector != RBbgMarketSector.None)
                    {
                        str.Append(" ");
                        switch (objInfo.MarketSector)
                        {

                            case RBbgMarketSector.Comdty:
                                str.Append("Comdty");
                                break;
                            case RBbgMarketSector.Corp:
                                str.Append("Corp");
                                break;
                            case RBbgMarketSector.Curncy:
                                str.Append("Curncy");
                                break;
                            case RBbgMarketSector.Equity:
                                str.Append("Equity");
                                break;
                            case RBbgMarketSector.Govt:
                                str.Append("Govt");
                                break;
                            case RBbgMarketSector.Index:
                                str.Append("Index");
                                break;
                            case RBbgMarketSector.MMkt:
                                str.Append("MMkt");
                                break;
                            case RBbgMarketSector.Mtge:
                                str.Append("Mtge");
                                break;
                            case RBbgMarketSector.Muni:
                                str.Append("Muni");
                                break;
                            case RBbgMarketSector.Pfd:
                                str.Append("Pfd");
                                break;
                        }
                    }
                    if (objInfo.InstrumentIdTypeSpecified)
                    {
                        str.Append("|" + objInfo.InstrumentIdType.ToString());
                    }
                    if (objInfo.DateSpecified)
                    {
                        if (objInfo.EndDate < objInfo.StartDate)
                            objInfo.EndDate = objInfo.StartDate + new TimeSpan(1, 0, 0, 0);
                        if (objInfo.InstrumentIdTypeSpecified)
                            str.Append("|" + objInfo.StartDate.ToString("yyyyMMdd") + "|" + objInfo.EndDate.ToString("yyyyMMdd"));
                        else
                            str.Append("||" + objInfo.StartDate.ToString("yyyyMMdd") + "|" + objInfo.EndDate.ToString("yyyyMMdd"));
                    }
                    if (objInfo.Overrides.Count > 0)
                    {
                        if (objInfo.InstrumentIdTypeSpecified || objInfo.DateSpecified)
                            str.Append("|" + objInfo.Overrides.Count + "|");
                        else
                            str.Append("||" + objInfo.Overrides.Count + "|");
                        foreach (var overrides in objInfo.Overrides)
                        {
                            str.Append(overrides.Key + "|");
                            overridesValue.Append(overrides.Value.ToString() + "|");
                        }
                        str.Append(overridesValue.ToString());
                    }
                    str.AppendLine();
                }

                str.Append("END-OF-DATA");
                str.AppendLine();
                str.Append("END-OF-FILE");
                fs = new FileStream(Path.Combine(workingDirectory, requestFileName),
                                    FileMode.Create, FileAccess.Write, FileShare.Read);
                sw = new StreamWriter(fs);
                sw.Write(str.ToString());

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
                mLogger.Debug("End ->writing security info on file.");
                sw.Close();
                fs.Close();
                if (str != null)
                    str = null;
            }
        }

        internal static void DecryptOutputFile(string workingDirectory, string responseFileName, string outputFileName, RBbgFundamentalInfo secInfo)
        {
            mLogger.Debug("RBbgFundamentalUtils:DecryptOutputFile=>Start decrypting  output file.");
            try
            {
                DecryptFile(workingDirectory, responseFileName, outputFileName, secInfo);
                mLogger.Debug("RBbgFundamentalUtils:DecryptOutputFile=>End decrypting output file.");
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgFundamentalUtils:DecryptOutputFile=>" + ex.ToString());
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
        }

        internal static void GetData(string outputFilePath, RBbgFundamentalInfo secInfo)
        {
            StreamReader sr = null;
            bool startOfData = false;
            List<string> lstLines = null;

            mLogger.Debug("RBbgFundamentalUtils:GetData =>Start reading output file.");
            try
            {
                lstLines = new List<string>();
                sr = new StreamReader(outputFilePath);
                DataTable resultantTable = CreateDataTable(secInfo.InstrumentFields);
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
                            foreach (RBbgInstrumentInfo info in secInfo.Instruments)
                            {
                                if ((info.InstrumentID.Trim() + RCorpActionConstant.WHITESPACE + info.MarketSector.ToString().Trim()).Equals(data[0].Trim()))
                                {
                                    instrument = info.InstrumentID;
                                    marketSector = info.MarketSector.ToString();
                                    break;
                                }
                                else if (info.InstrumentID.Trim().Equals(data[0].Trim()))
                                {
                                    instrument = info.InstrumentID;
                                    marketSector = string.Empty;
                                    break;
                                }
                            }
                            DataRow row = resultantTable.NewRow();
                            row[0] = instrument;
                            row[1] = marketSector;
                            if (data[1].Equals("0"))
                            {
                                if (!secInfo.ProcessedInstruments.Contains(instrument))
                                    secInfo.ProcessedInstruments.Add(instrument);
                                row[2] = RVendorStatusConstant.PASSED;
                                for (int count = 3; count < secInfo.InstrumentFields.Count + 3; count++)
                                {
                                    row[count] = data[count];
                                    if (!secInfo.ProcessedFields.Contains(secInfo.InstrumentFields[count - 3].Mnemonic) && !data[count].Equals(RCorpActionConstant.NA))
                                        secInfo.ProcessedFields.Add(secInfo.InstrumentFields[count - 3].Mnemonic);
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
                secInfo.ResultantData.Tables.Add(resultantTable);
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgFundamentalUtils:GetData =>" + ex.ToString());
                throw ex;
            }
        }

        private static DataTable CreateDataTable(List<RBbgFieldInfo> list)
        {
            DataTable resultantTable = new DataTable();
            resultantTable.Columns.Add(RCorpActionConstant.INSTRUMENT);
            resultantTable.Columns.Add(RCorpActionConstant.MARKET_SECTOR);
            resultantTable.Columns.Add(RCorpActionConstant.IS_VALID);
            list.ForEach(q => resultantTable.Columns.Add(q.Mnemonic));
            return resultantTable;
        }

        private static void DecryptFile(string workingDirectory, string responseFileName, string decryptFileName, RBbgFundamentalInfo secInfo)
        {
            try
            {
                mLogger.Debug("RBbgFundamentalUtils:DecryptFile=>start decrypting file");
                RCommandExecutor cmdExec = new RCommandExecutor();
                string decryptCmd = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.FTPDECRYPTCOMMAND);
                string decryptKey = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.FTPDECRPYPTKEY);
                string decryptToolPath = RVendorConfigLoader.GetDecryptToolPath(secInfo.VendorPreferenceId);
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
                mLogger.Debug("RBbgFundamentalUtils:DecryptFile=>end decrypting file");
            }
            catch (Exception ex)
            {
                mLogger.Error("RBbgFundamentalUtils:DecryptFile=>" + ex.ToString());
                throw ex;
            }
        }
    }

    class DistinctInstrument : IEqualityComparer<RBbgInstrumentInfo>
    {
        #region IEqualityComparer<RBbgInstrumentInfo> Members

        public bool Equals(RBbgInstrumentInfo x, RBbgInstrumentInfo y)
        {
            if (x.InstrumentID.Equals(y.InstrumentID))
                return true;
            else
                return false;
        }

        public int GetHashCode(RBbgInstrumentInfo obj)
        {
            return base.GetHashCode();
        }

        #endregion

    }

    class DistinctField : IEqualityComparer<RBbgFieldInfo>
    {
        #region IEqualityComparer<RBbgInstrumentInfo> Members

        public bool Equals(RBbgFieldInfo x, RBbgFieldInfo y)
        {
            if (x.Mnemonic.Equals(y.Mnemonic))
                return true;
            else
                return false;
        }

        public int GetHashCode(RBbgFieldInfo obj)
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
