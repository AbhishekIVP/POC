using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.srm.vendorapi.bloomberg;
using com.ivp.srm.vendorapi.bloombergServices;
using System.IO;
using com.ivp.rad.transport;
using com.ivp.rad.common;
using com.ivp.rad.utils;
using System.Threading;
using System.Data;
//using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;
using com.ivp.rad.configurationmanagement;
using com.ivp.srm.vendorapi.AlterLoggerService;
using System.Diagnostics;
using System.Web.Script.Serialization;
using com.ivp.srm.vendorapi.BBFTPServiceLayer;
using System.Configuration;
using com.ivp.srmcommon;

namespace com.ivp.srm.vendorapi.bloomberg
{
    /// <summary>
    /// Bloomberg Class for FTP Request
    /// </summary>
    internal class RBbgFTPRequest
    {
        #region member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMVendorAPI");
        int count = 0;
        List<string> lstLines = null;
        private const string WHITESPACE = " ";
        #endregion

        void newProcess_ExitedFTPUpload(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(((Process)sender).StandardError.ReadToEnd().Trim()))
            {
                mLogger.Error("BBG FTP Upload Job =>" + ((Process)sender).StandardError.ReadToEnd().Trim());
            }
        }

        //------------------------------------------------------------------------------------------
        #region Public Methods
        /// <summary>
        /// Gets the securities.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        public object GetSecurities(RBbgSecurityInfo securityInfo, bool immediataeRequest,
                                                                            string timeStamp, ref Dictionary<string, string> dictHeaders, out string AdditionalInfo, out string ResponseAdditionalInfo, ref SecuritiesCollection unprocessedSecurities)
        {
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;

            //---Write Security Info on File-----
            mLogger.Debug("Start ->writing security info on file.");
            try
            {
                string appenTime = timeStamp;
                string clientName = SRMMTConfig.GetClientName();
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                workingDirectory = SRMMTCommon.GetClientFilePath(workingDirectory);
                string requestFileName = "req" + appenTime + ".req";
                string responseFileName = "rep" + appenTime + ".res";
                string outputFileName = "decrypt" + appenTime + ".txt";
                string decryptToolPath = RVendorConfigLoader.GetDecryptToolPath(securityInfo.VendorPreferenceId);
                if (string.IsNullOrEmpty(decryptToolPath))
                    ResponseAdditionalInfo = Path.Combine(workingDirectory, responseFileName);
                else
                    ResponseAdditionalInfo = Path.Combine(workingDirectory, outputFileName);
                AdditionalInfo = Path.Combine(workingDirectory, requestFileName);

                if (!Directory.Exists(workingDirectory))
                {
                    Directory.CreateDirectory(workingDirectory);
                }

                //Writing security info on files
                StringBuilder message = new StringBuilder();

                if (securityInfo.IsBulkList)
                {
                    message.AppendLine("Requesting following bulk identifiers though FTP:");
                    WriteSecInfoOnFileForBulkList(securityInfo, requestFileName, responseFileName, message, ref dictHeaders);
                }
                else if (securityInfo.IsGetCompany)
                {
                    message.AppendLine("Requesting following identifiers though FTP:");
                    WriteSecInfoOnFileForGetCompany(securityInfo, requestFileName, responseFileName, message);
                }
                else
                {
                    message.AppendLine("Requesting following identifiers though FTP:");

                    date1 = DateTime.Now;
                    WriteSecInfoOnFile(securityInfo, requestFileName, responseFileName, message, ref dictHeaders);
                    date2 = DateTime.Now;
                    RVendorUtils.InsertLogTable("WriteSecInfoOnFile", date1, date2);
                }

                RBbgUtils.InsertBBGAuditHeaders(timeStamp, dictHeaders);

                string ftpFolderName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.FTPFOLDERNAME);

                string useSingleBBGTransport = ConfigurationManager.AppSettings["UseSingleBBGTransport"];
                bool boolResult;
                if (!string.IsNullOrEmpty(useSingleBBGTransport) && bool.TryParse(useSingleBBGTransport, out boolResult) && boolResult)
                {
                    var fileInfo = new SMFTPFileInfo
                    {
                        FileName = requestFileName,
                        Identifier = appenTime,
                        FolderName = string.IsNullOrEmpty(ftpFolderName) ? "//" : ftpFolderName,
                        VendorPreferenceId = securityInfo.VendorPreferenceId
                    };

                    string uploadMessage;
                    bool isSuccess = false;
                    while (!isSuccess)
                    {
                        if (!SRMBBGFTPServiceLayer.RegisterUploadFile(fileInfo, out uploadMessage))
                        {
                            if (!string.IsNullOrEmpty(uploadMessage))
                                throw new Exception(uploadMessage);
                            Thread.Sleep(3000);
                        }
                        else
                            isSuccess = true;
                    }


                    uploadMessage = string.Empty;
                    isSuccess = false;
                    while (!isSuccess)
                    {
                        if (!SRMBBGFTPServiceLayer.GetUploadFileResponse(fileInfo.Identifier, out uploadMessage))
                        {
                            if (!string.IsNullOrEmpty(uploadMessage))
                                throw new Exception(uploadMessage);

                            Thread.Sleep(3000);
                        }
                        else
                            isSuccess = true;
                    }

                    if (immediataeRequest)
                        return GetResponse(appenTime, securityInfo.TransportName, securityInfo.IsBulkList, securityInfo, ref unprocessedSecurities);
                }
                else
                {
                    string fileName = Path.Combine(workingDirectory, requestFileName);

                    #region NEW PROCESS
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    if (!File.Exists(baseDirectory + "\\SRMBBGUploadDownload.exe"))
                        baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\bin";

                    ProcessStartInfo processInfo = new ProcessStartInfo(baseDirectory + "\\SRMBBGUploadDownload.exe", "bbgupload " + ("\"" + fileName + "\"") + " " + ("\"" + securityInfo.TransportName + "\"") + " " + ("\"" + appenTime + "\"") + " " + ("\"" + ftpFolderName + "\"") + " " + ("\"" + clientName + "\""));

                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;

                    processInfo.WorkingDirectory = baseDirectory;
                    processInfo.RedirectStandardError = true;
                    processInfo.RedirectStandardInput = true;
                    processInfo.RedirectStandardOutput = true;

                    Process newProcess = Process.Start(processInfo);
                    newProcess.EnableRaisingEvents = true;
                    newProcess.Exited += new EventHandler(newProcess_ExitedFTPUpload);
                    newProcess.WaitForExit();
                    int exitCode = newProcess.ExitCode;
                    switch (exitCode)
                    {
                        case 1:
                            if (immediataeRequest)
                                return GetResponse(appenTime, securityInfo.TransportName, securityInfo.IsBulkList, securityInfo, ref unprocessedSecurities);
                            break;
                        case -1:
                            throw new Exception("Unknown exception. Please check Bloomberg FTP log for details");
                            break;
                        case 0:
                            throw new Exception("Failed to upload request file. Request timed out.");
                            break;
                    }
                    #endregion
                }

                //RTransportManager.GetTransport(securityInfo.TransportName).SendMessage(null, new object[]
                //{
                //    new RFTPContent
                //    {
                //        FileName = fileName,
                //        FolderName = "//",
                //        Action = FTPAction.Upload
                //    }
                //});

                return appenTime;
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
            }

        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="requestIdentifier">The request identifier.</param>
        /// <param name="transportName">Name of the transport.</param>
        /// <returns></returns>
        public SecuritiesCollection GetResponse(string requestIdentifier, string transportName, bool IsBulkList, RBbgSecurityInfo securityInfo, ref SecuritiesCollection unprocessedSecurities)
        {
            SecuritiesCollection dictResult = null;
            mLogger.Debug("start ->reading security info from file.");
            try
            {
                string appenTime = requestIdentifier;
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                         (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                workingDirectory = SRMMTCommon.GetClientFilePath(workingDirectory);
                string clientName = SRMMTConfig.GetClientName();
                string requestFileName = "req" + appenTime + ".req";
                string responseFileName = "rep" + appenTime + ".res";
                string outputFileName = "decrypt" + appenTime + ".txt";

                string ftpFolderName = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.FTPFOLDERNAME);

                string useSingleBBGTransport = ConfigurationManager.AppSettings["UseSingleBBGTransport"];
                bool boolResult;
                if (!string.IsNullOrEmpty(useSingleBBGTransport) && bool.TryParse(useSingleBBGTransport, out boolResult) && boolResult)
                {
                    var fileInfo = new SMFTPFileInfo
                    {
                        FileName = string.IsNullOrEmpty(ftpFolderName) ? responseFileName : Path.Combine(ftpFolderName, responseFileName),
                        Identifier = appenTime,
                        FolderName = "//",
                        VendorPreferenceId = securityInfo.VendorPreferenceId
                    };

                    string downloadMessage;
                    bool isSuccess = false;
                    while (!isSuccess)
                    {
                        if (!SRMBBGFTPServiceLayer.RegisterDownloadFile(fileInfo, out downloadMessage))
                        {
                            if (!string.IsNullOrEmpty(downloadMessage))
                                throw new Exception(downloadMessage);

                            Thread.Sleep(3000);
                        }
                        else
                            isSuccess = true;
                    }

                    downloadMessage = string.Empty;
                    isSuccess = false;
                    while (!isSuccess)
                    {
                        if (!SRMBBGFTPServiceLayer.GetDownloadFileResponse(fileInfo.Identifier, out downloadMessage))
                        {
                            if (!string.IsNullOrEmpty(downloadMessage))
                                throw new Exception(downloadMessage);

                            Thread.Sleep(3000);
                        }
                        else
                            isSuccess = true;
                    }

                    //string decryptFileName = string.Empty;
                    //if (securityInfo.counter == 0)
                    //    decryptFileName = "decrypt022118031714583.txt";
                    //else if (securityInfo.counter == 1)
                    //    decryptFileName = "decrypt022118031711846.txt";
                    //decryptFileName = "decrypt031918043357095.txt";
                    string decryptFileName = DecryptOutputFile(workingDirectory, responseFileName, outputFileName, securityInfo.IsBVAL, securityInfo);
                    if (!IsBulkList)
                        //Read From output File and Return Dictionary
                        dictResult = ReadFromOutputFile(Path.Combine(workingDirectory, decryptFileName), securityInfo, ref unprocessedSecurities);
                    else
                    {
                        if (securityInfo.IsCombinedFtpReq)
                            dictResult = ReadFromOutputFileForBulkListAndNormal(Path.Combine(workingDirectory, decryptFileName), securityInfo.BulkListMapId[0], ref unprocessedSecurities);
                        else
                            dictResult = ReadFromOutputFileForBulkList(Path.Combine(workingDirectory, decryptFileName), securityInfo.BulkListMapId[0], ref unprocessedSecurities);
                    }
                }
                else
                {
                    #region NEW PROCESS
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    if (!File.Exists(baseDirectory + "\\SRMBBGUploadDownload.exe"))
                        baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\bin";

                    ProcessStartInfo processInfo = new ProcessStartInfo(baseDirectory + "\\SRMBBGUploadDownload.exe", "bbgdownload " + ("\"" + responseFileName + "\"") + " " + ("\"" + workingDirectory + "\"") + " " + ("\"" + transportName + "\"") + " " + ("\"" + appenTime + "\"") + " " + ("\"" + ftpFolderName + "\"") + " " + ("\"" + clientName + "\""));

                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = false;

                    processInfo.WorkingDirectory = baseDirectory;
                    processInfo.RedirectStandardError = true;
                    processInfo.RedirectStandardInput = true;
                    processInfo.RedirectStandardOutput = true;

                    Process newProcess = Process.Start(processInfo);
                    newProcess.EnableRaisingEvents = true;
                    newProcess.Exited += new EventHandler(newProcess_ExitedFTPUpload);
                    newProcess.WaitForExit();

                    int exitCode = newProcess.ExitCode;
                    switch (exitCode)
                    {
                        case 1:
                            string decryptFileName = DecryptOutputFile(workingDirectory, responseFileName, outputFileName, securityInfo.IsBVAL, securityInfo);
                            if (!IsBulkList)
                                //Read From output File and Return Dictionary
                                dictResult = ReadFromOutputFile(Path.Combine(workingDirectory, decryptFileName), securityInfo, ref unprocessedSecurities);
                            else
                            {
                                if (securityInfo.IsCombinedFtpReq)
                                    dictResult = ReadFromOutputFileForBulkListAndNormal(Path.Combine(workingDirectory, decryptFileName), securityInfo.BulkListMapId[0], ref unprocessedSecurities);
                                else
                                    dictResult = ReadFromOutputFileForBulkList(Path.Combine(workingDirectory, decryptFileName), securityInfo.BulkListMapId[0], ref unprocessedSecurities);
                            }
                            break;
                        case -1:
                            throw new Exception("Unknown exception. Please check Bloomberg FTP log for details");
                            break;
                        case 0:
                            throw new Exception("Failed to download response file. Request timed out.");
                            break;
                    }
                    #endregion
                }

                //RTransportManager.GetTransport(transportName).ReceiveMessage(null, new object[]
                //{
                //    new RFTPContent
                //    {
                //        FileName = responseFileName,
                //        FolderName = workingDirectory,
                //        UsePrivateKey = true
                //    }
                //});
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
                mLogger.Debug("end ->reading security info from file.");
            }
            return dictResult;
        }

        private SecuritiesCollection ReadFromOutputFileForBulkListAndNormal(string filePath, int bulkListId, ref SecuritiesCollection unprocessedSecurities)
        {
            SecuritiesCollection dictResult = null;
            StreamReader sr = null;
            bool startOfData = false;
            mLogger.Debug("Start ->reading output file.");
            try
            {
                int counter = 1;
                lstLines = new List<string>();
                dictResult = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                sr = new StreamReader(filePath);
                while (!sr.EndOfStream)
                {
                    lstLines.Add(sr.ReadLine());
                }
                List<string> lstFieldNames = StartReadingFieldNames();
                for (int j = count; j < lstLines.Count; j++)
                {
                    if (lstLines[j] == "START-OF-DATA")
                        startOfData = true;
                    if (lstLines[j + 1] == "END-OF-DATA")
                        break;
                    if (startOfData)
                    {
                        List<string> rowVal = lstLines[j + 1].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (rowVal.Count < 3)
                            continue;
                        Dictionary<string, RVendorFieldInfo> nLsit = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                        bool isBulkField = false;
                        for (int k = 3; k < rowVal.Count; k++)
                        {
                            if (lstFieldNames.Count > k - 2)
                            {
                                isBulkField = true;
                                RVendorFieldInfo obj = new RVendorFieldInfo();
                                obj.Name = lstFieldNames[k - 3];
                                obj.Value = rowVal[k];
                                obj.Status = RVendorStatusConstant.PASSED;
                                nLsit.Add(obj.Name, obj);
                            }
                            else
                            {
                                if (isBulkField)
                                {
                                    bool fieldHasValue = false;
                                    foreach (var fieldInfo in nLsit.Values)
                                    {
                                        if (RBbgUtils.IsInValidField(fieldInfo.Value))
                                        {
                                            fieldInfo.Value = RVendorConstant.NOT_AVAILABLE;
                                            fieldInfo.Status = RVendorStatusConstant.FAILED;
                                        }
                                        else
                                        {
                                            fieldHasValue = true;
                                            break;
                                        }
                                    }

                                    string instrument = rowVal[0];
                                    if (fieldHasValue)
                                    {
                                        if (!dictResult.ContainsKey(instrument))
                                            dictResult.Add(instrument, nLsit);
                                    }
                                    else
                                    {
                                        if (!unprocessedSecurities.ContainsKey(instrument))
                                            unprocessedSecurities.Add(instrument, nLsit);
                                        else
                                            unprocessedSecurities[instrument] = nLsit;
                                    }

                                    nLsit = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                                    isBulkField = true;
                                }
                                string[] bulkInfo = rowVal[k].Split(';');
                                int fieldCount = Convert.ToInt32(bulkInfo[3]);
                                for (int bulkField = 5; bulkField < bulkInfo.Count();)
                                {
                                    if (nLsit.Count == 7)
                                    {
                                        bool fieldHasValue = false;
                                        foreach (var fieldInfo in nLsit.Values)
                                        {
                                            if (RBbgUtils.IsInValidField(fieldInfo.Value))
                                            {
                                                fieldInfo.Value = RVendorConstant.NOT_AVAILABLE;
                                                fieldInfo.Status = RVendorStatusConstant.FAILED;
                                            }
                                            else
                                            {
                                                fieldHasValue = true;
                                                break;
                                            }
                                        }

                                        string instrument = rowVal[0] + " " + counter + "," + bulkListId.ToString();
                                        if (fieldHasValue)
                                        {
                                            if (!dictResult.ContainsKey(instrument))
                                                dictResult.Add(instrument, nLsit);
                                        }
                                        else
                                        {
                                            if (!unprocessedSecurities.ContainsKey(instrument))
                                                unprocessedSecurities.Add(instrument, nLsit);
                                            else
                                                unprocessedSecurities[instrument] = nLsit;
                                        }

                                        counter++;
                                        nLsit = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                                    }
                                    RVendorFieldInfo obj = new RVendorFieldInfo();
                                    obj.Name = bulkInfo[bulkField];
                                    obj.Value = bulkInfo[bulkField];
                                    obj.Status = RVendorStatusConstant.PASSED;
                                    nLsit.Add(obj.Name, obj);
                                    bulkField += 2;
                                }
                            }
                        }
                    }
                }
                if (lstLines != null)
                    lstLines.Clear();
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
                mLogger.Debug("End ->reading output file.");
                if (sr != null)
                    sr = null;

            }
            return dictResult;
        }
        #endregion
        //------------------------------------------------------------------------------------------
        #region Private Methods
        /// <summary>
        /// Reads from output file for bulk list.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private SecuritiesCollection ReadFromOutputFileForBulkList(string filePath, int bulkListId, ref SecuritiesCollection unprocessedSecurities)
        {
            SecuritiesCollection dictResult = null;
            StreamReader sr = null;
            bool startOfData = false;
            mLogger.Debug("Start ->reading output file.");
            try
            {
                RBbgBulkListInfo bulkListInfo = RVendorUtils.GetBulkListInfo(bulkListId);

                var lstFields = bulkListInfo.OutputFields.Split(',').ToList();

                int counter = 1;
                lstLines = new List<string>();
                dictResult = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                sr = new StreamReader(filePath);
                while (!sr.EndOfStream)
                {
                    lstLines.Add(sr.ReadLine());
                }
                List<string> lstFieldNames = StartReadingFieldNames();
                for (int j = count; j < lstLines.Count; j++)
                {
                    if (lstLines[j] == "START-OF-DATA")
                        startOfData = true;
                    if (lstLines[j + 1] == "END-OF-DATA")
                        break;
                    if (startOfData)
                    {
                        string[] rowVal = lstLines[j + 1].Split('|');
                        if (!rowVal[0].EndsWith("  "))// && rowVal[1] == "10" && rowVal[2] == "1"
                            continue;
                        bool fieldHasValue = false;
                        var nLsit = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                        for (int k = 1; k < lstFields.Count + 1; k++)
                        {
                            if (rowVal.Length <= k)
                                continue;

                            RVendorFieldInfo obj = new RVendorFieldInfo();
                            obj.Name = lstFields[k - 1];
                            obj.Value = rowVal[k];
                            if (RBbgUtils.IsInValidField(obj.Value))
                            {
                                obj.Value = RVendorConstant.NOT_AVAILABLE;
                                obj.Status = RVendorStatusConstant.FAILED;
                            }
                            else
                            {
                                obj.Status = RVendorStatusConstant.PASSED;
                                fieldHasValue = true;
                            }
                            nLsit.Add(obj.Name, obj);
                        }

                        string instrument = rowVal[0] + " " + counter + "," + bulkListId.ToString();
                        if (fieldHasValue)
                        {
                            dictResult.Add(instrument, nLsit);
                        }
                        else
                        {
                            if (!unprocessedSecurities.ContainsKey(instrument))
                                unprocessedSecurities.Add(instrument, nLsit);
                            else
                                unprocessedSecurities[instrument] = nLsit;
                        }
                        counter++;
                    }
                }
                if (lstLines != null)
                    lstLines.Clear();
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
                mLogger.Debug("End ->reading output file.");
                if (sr != null)
                    sr = null;

            }
            return dictResult;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Reads from output file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private SecuritiesCollection ReadFromOutputFile(string filePath, RBbgSecurityInfo secInfo, ref SecuritiesCollection unprocessedSecurities)
        {
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;

            SecuritiesCollection dictResult = null;
            StreamReader sr = null;
            bool startOfData = false;
            mLogger.Debug("Start ->reading output file.");
            try
            {
                Dictionary<string, string> DictInstrumentNameVsIdentifier = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (secInfo.Instruments != null && secInfo.Instruments.Count > 0)
                {
                    foreach (var item in secInfo.Instruments)
                    {
                        string InstrumentName = item.InstrumentID.Trim();
                        string instrumentNameWithoutInstrument = string.Empty;

                        if (!string.IsNullOrEmpty(InstrumentName) && !string.IsNullOrWhiteSpace(InstrumentName))
                            instrumentNameWithoutInstrument = InstrumentName.Split('|')[0];

                        if (item.MarketSector != RBbgMarketSector.None)
                        {
                            instrumentNameWithoutInstrument += RCorpActionConstant.WHITESPACE + item.MarketSector.ToString();
                        }
                        DictInstrumentNameVsIdentifier[instrumentNameWithoutInstrument] = item.InstrumentID;
                    }
                }

                lstLines = new List<string>();
                dictResult = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
                sr = new StreamReader(filePath);
                while (!sr.EndOfStream)
                {
                    lstLines.Add(sr.ReadLine());
                }
                List<string> lstFieldNames = StartReadingFieldNames();
                for (int j = count; j < lstLines.Count; j++)
                {
                    if (lstLines[j] == "START-OF-DATA")
                        startOfData = true;
                    if (lstLines[j + 1] == "END-OF-DATA")
                        break;
                    if (startOfData)
                    {
                        string[] rowVal = lstLines[j + 1].Remove(lstLines[j + 1].Length - 1, 1).Split('|');
                        if (rowVal.Length < 3)
                            continue;

                        bool fieldHasValue = false;
                        var nLsit = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                        for (int k = 0; k < lstFieldNames.Count; k++)
                        {
                            RVendorFieldInfo obj = new RVendorFieldInfo();
                            obj.Name = lstFieldNames[k];
                            obj.Value = rowVal[k + 3];
                            if (RBbgUtils.IsInValidField(obj.Value))
                            {
                                obj.Value = secInfo.requireNotAvailableInField ? RVendorConstant.NOT_AVAILABLE : string.Empty;
                                obj.Status = RVendorStatusConstant.FAILED;
                            }
                            else
                            {
                                obj.Status = RVendorStatusConstant.PASSED;
                                fieldHasValue = true;
                            }

                            nLsit.Add(obj.Name, obj);
                        }
                        if (secInfo.IsMarketSectorAppended)
                        {
                            dictResult.Add(rowVal[0], nLsit);
                        }
                        else
                        {
                            string instrument = rowVal[0].Trim();

                            int lastIndexOfSpace = instrument.LastIndexOf(' ');
                            string instrumentTrimmed = instrument;
                            if (lastIndexOfSpace > -1)
                                instrumentTrimmed = instrumentTrimmed.Substring(0, lastIndexOfSpace);
                            if (DictInstrumentNameVsIdentifier.Count > 0 && DictInstrumentNameVsIdentifier.ContainsKey(instrument))
                            {
                                instrument = DictInstrumentNameVsIdentifier[instrument];
                            }
                            else if (DictInstrumentNameVsIdentifier.Count > 0 && DictInstrumentNameVsIdentifier.ContainsKey(instrumentTrimmed))
                            {
                                instrument = DictInstrumentNameVsIdentifier[instrumentTrimmed];
                            }
                            else
                                instrument = instrumentTrimmed;
                            //if (secInfo.Instruments.Count > 0)
                            //{
                            //    foreach (RBbgInstrumentInfo instInfo in secInfo.Instruments)
                            //    {
                            //        if ((instInfo.InstrumentID.Trim() + RCorpActionConstant.WHITESPACE + instInfo.MarketSector.ToString().Trim()).Equals(rowVal[0].Trim()))
                            //        {
                            //            instrument = instInfo.InstrumentID;
                            //            break;
                            //        }
                            //        else if (instInfo.InstrumentID.Trim().Equals(rowVal[0].Trim()))
                            //        {
                            //            instrument = instInfo.InstrumentID;
                            //            break;
                            //        }
                            //        else if (rowVal[0].LastIndexOf(' ') > 0)
                            //        {
                            //            int lastIndexOfSpace = rowVal[0].LastIndexOf(' ');
                            //            mLogger.Debug("Row Instrument:" + rowVal[0] + "," + rowVal[0].Substring(0, lastIndexOfSpace) + "," + dictResult.ContainsKey(rowVal[0].Substring(0, lastIndexOfSpace)).ToString());
                            //            instrument = rowVal[0].Substring(0, lastIndexOfSpace);
                            //        }d
                            //    }
                            //}
                            //else
                            //{
                            //    if (rowVal[0].LastIndexOf(' ') > 0)
                            //    {
                            //        int lastIndexOfSpace = rowVal[0].LastIndexOf(' ');
                            //        mLogger.Debug("Row Instrument:" + rowVal[0] + "," + rowVal[0].Substring(0, lastIndexOfSpace) + "," + dictResult.ContainsKey(rowVal[0].Substring(0, lastIndexOfSpace)).ToString());
                            //        instrument = rowVal[0].Substring(0, lastIndexOfSpace);
                            //    }
                            //}

                            //int lastIndexOfSpace = rowVal[0].LastIndexOf(' ');
                            //mLogger.Debug("Row Instrument:" + rowVal[0] + "," + rowVal[0].Substring(0, lastIndexOfSpace) + "," + dictResult.ContainsKey(rowVal[0].Substring(0, lastIndexOfSpace)).ToString());
                            //dictResult.Add(rowVal[0].Substring(0, lastIndexOfSpace), nLsit);
                            if (fieldHasValue)
                            {
                                if (!dictResult.ContainsKey(instrument))
                                    dictResult.Add(instrument, nLsit);
                            }
                            else
                            {
                                if (!unprocessedSecurities.ContainsKey(instrument))
                                    unprocessedSecurities.Add(instrument, nLsit);
                                else
                                    unprocessedSecurities[instrument] = nLsit;
                            }
                        }
                    }
                }
                if (lstLines != null)
                    lstLines.Clear();
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
                mLogger.Debug("End ->reading output file.");
                if (sr != null)
                    sr = null;

                date2 = DateTime.Now;
                RVendorUtils.InsertLogTable("ReadFromOutputFile", date1, date2);
            }
            return dictResult;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Starts the reading field names.
        /// </summary>
        /// <returns></returns>
        private List<string> StartReadingFieldNames()
        {
            List<string> strFieldNames = null;
            bool startOfFields = false;
            mLogger.Debug("Start ->reading field names.");
            try
            {
                strFieldNames = new List<string>();
                for (int i = 0; i < lstLines.Count; i++)
                {
                    if (lstLines[i] == "START-OF-FIELDS")
                        startOfFields = true;
                    if (lstLines[i + 1] == "END-OF-FIELDS")
                    {
                        count = i + 1;
                        break;
                    }
                    if (startOfFields)
                        if (!strFieldNames.Contains(lstLines[i + 1]))
                            strFieldNames.Add(lstLines[i + 1]);
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
                mLogger.Debug("End ->reading field names.");
            }
            return strFieldNames;
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Writes the sec info on file for bulk list.
        /// </summary>
        /// <param name="secInfo">The sec info.</param>
        /// <param name="requestFileName">Name of the request file.</param>
        /// <param name="responseFileName">Name of the response file.</param>
        private void WriteSecInfoOnFileForBulkList(RBbgSecurityInfo secInfo, string requestFileName,
                                                                string responseFileName, StringBuilder message, ref Dictionary<string, string> dictHeaders)
        {
            StringBuilder str = null;
            FileStream fs = null;
            StreamWriter sw = null;
            mLogger.Debug("Start ->writing security info on file.");
            try
            {
                string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                         (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                workingDirectory = SRMMTCommon.GetClientFilePath(workingDirectory);

                str = new StringBuilder();
                str.Append("START-OF-FILE");
                str.AppendLine();

                HashSet<string> restrictedHeaders = RBbgUtils.GetRestrictedHeaders(RBbgHeaderType.BulkList);
                HashSet<string> defaultHeadersToSkip = RBbgUtils.GetDefaultHeadersToSkip();
                Dictionary<string, string> HeaderNamesVsValues = RVendorConfigLoader.GetVendorHeaders(RVendorType.Bloomberg, secInfo.VendorPreferenceId, "GetData");

                if (secInfo.HeaderNamesVsValues != null && secInfo.HeaderNamesVsValues.Count > 0)
                    foreach (var kvp in secInfo.HeaderNamesVsValues)
                        HeaderNamesVsValues[kvp.Key] = kvp.Value;

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

                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "FIRMNAME", RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.USER_ID));
                str.Append("FIRMNAME=");
                str.Append(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.USER_ID));

                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "FILETYPE", "pc");
                str.AppendLine();
                str.Append("FILETYPE=pc");

                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "REPLYFILENAME", responseFileName);
                str.AppendLine();
                str.Append("REPLYFILENAME=" + responseFileName);

                if (!defaultHeadersToSkip.Contains("CLOSINGVALUES"))
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "CLOSINGVALUES", "yes");
                    str.AppendLine();
                    str.Append("CLOSINGVALUES=yes");
                }

                if (!defaultHeadersToSkip.Contains("DERIVED"))
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "DERIVED", "yes");
                    str.AppendLine();
                    str.Append("DERIVED=yes");
                }

                if (!defaultHeadersToSkip.Contains("HISTORICAL"))
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "HISTORICAL", "yes");
                    str.AppendLine();
                    str.Append("HISTORICAL=yes");
                }

                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "OUTPUTFORMAT", "bulklist");
                str.AppendLine();
                str.Append("OUTPUTFORMAT=bulklist");
                str.AppendLine();
                if (secInfo.Instruments[0].InstrumentIdType != RBbgInstrumentIdType.NONE)
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "SECID", secInfo.Instruments[0].InstrumentIdType.ToString());
                    str.Append("SECID=" + secInfo.Instruments[0].InstrumentIdType);
                    str.AppendLine();
                }

                if (!defaultHeadersToSkip.Contains("SECMASTER"))
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "SECMASTER", "yes");
                    str.Append("SECMASTER=yes");
                    str.AppendLine();
                }
                if (secInfo.Instruments[0].MarketSector != RBbgMarketSector.None)
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "YELLOWKEY", secInfo.Instruments[0].MarketSector.ToString());
                    str.Append("YELLOWKEY=" + secInfo.Instruments[0].MarketSector);
                    //str.Append("YELLOWKEY="+secInfo.MarketSector);
                    str.AppendLine();
                }

                str.AppendLine();
                if (!defaultHeadersToSkip.Contains("QUOTECOMPOSITE"))
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "QUOTECOMPOSITE", "yes");
                    str.Append("QUOTECOMPOSITE=yes");
                    str.AppendLine();
                }
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "PROGRAMNAME", "getdata");
                str.Append("PROGRAMNAME=getdata");
                str.AppendLine();

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
                    message.AppendLine(objInfo.InstrumentID + " " + objInfo.InstrumentIdType + "for bulk field=> " + secInfo.InstrumentFields[0].Mnemonic);
                    str.Append(objInfo.InstrumentID);
                    str.AppendLine();
                    // if (secInfo.IsBulkList)
                    //   break;
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

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Writes the sec info on file.
        /// </summary>
        /// <param name="secInfo">The sec info.</param>
        /// <param name="requestFileName">Name of the request file.</param>
        /// <param name="responseFileName">Name of the response file.</param>
        private void WriteSecInfoOnFile(RBbgSecurityInfo secInfo, string requestFileName,
                                                                string responseFileName, StringBuilder message, ref Dictionary<string, string> dictHeaders)
        {
            StringBuilder str = null;
            FileStream fs = null;
            StreamWriter sw = null;
            mLogger.Debug("Start ->writing security info on file.");
            try
            {
                string clientName = SRMMTConfig.GetClientName();

                string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                         (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                workingDirectory = SRMMTCommon.GetClientFilePath(workingDirectory);

                str = new StringBuilder();
                str.Append("START-OF-FILE");
                str.AppendLine();

                //Pending for Headers

                HashSet<string> restrictedHeaders = null;
                Dictionary<string, string> HeaderNamesVsValues = null;
                HashSet<string> defaultHeadersToSkip = RBbgUtils.GetDefaultHeadersToSkip();

                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "FIRMNAME", RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.USER_ID));
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "FILETYPE", "pc");
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "REPLYFILENAME", responseFileName);
                str.Append("FIRMNAME=");
                str.Append(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.USER_ID));
                str.AppendLine();
                str.Append("FILETYPE=pc");
                str.AppendLine();
                str.Append("REPLYFILENAME=" + responseFileName);
                str.AppendLine();
                if (secInfo.IsBVALPricingSource)
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "PRICING_SOURCE", secInfo.Instruments[0].BVALPriceSourceValue);
                    str.Append("PRICING_SOURCE=" + secInfo.Instruments[0].BVALPriceSourceValue);
                    str.AppendLine();

                    restrictedHeaders = RBbgUtils.GetRestrictedHeaders(RBbgHeaderType.BVAL);
                    HeaderNamesVsValues = RVendorConfigLoader.GetVendorHeaders(RVendorType.Bloomberg, secInfo.VendorPreferenceId, "BVAL");
                }
                else
                {
                    restrictedHeaders = RBbgUtils.GetRestrictedHeaders(RBbgHeaderType.GetData);
                    HeaderNamesVsValues = RVendorConfigLoader.GetVendorHeaders(RVendorType.Bloomberg, secInfo.VendorPreferenceId, "GetData");
                }

                if (secInfo.HeaderNamesVsValues != null && secInfo.HeaderNamesVsValues.Count > 0)
                    foreach (var kvp in secInfo.HeaderNamesVsValues)
                        HeaderNamesVsValues[kvp.Key] = kvp.Value;

                bool isProgramFlagSet = false;
                if (HeaderNamesVsValues != null && HeaderNamesVsValues.Count > 0)
                {
                    foreach (var item in HeaderNamesVsValues.Keys)
                    {
                        if (!restrictedHeaders.Contains(item))
                        {
                            RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, item, (string.IsNullOrEmpty(HeaderNamesVsValues[item]) ? "yes" : HeaderNamesVsValues[item]));
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

                if (!defaultHeadersToSkip.Contains("DERIVED"))
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "DERIVED", "yes");
                    str.Append("DERIVED=yes");
                    str.AppendLine();
                }
                if (!defaultHeadersToSkip.Contains("SECMASTER"))
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "SECMASTER", "yes");
                    str.Append("SECMASTER=yes");
                    str.AppendLine();
                }
                RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "PROGRAMNAME", "getdata");
                str.Append("PROGRAMNAME=getdata");
                str.AppendLine();
                //str.Append("SPECIALCHAR=decimal:2");
                //str.AppendLine();
                //str.Append("SECID=");
                //str.Append(secInfo.InstrumentIdentifierType);
                //str.AppendLine();
                //str.Append("YELLOWKEY=");
                //str.Append(secInfo.MarketSector);
                //str.AppendLine();
                if (secInfo.Instruments.Count > 0 && secInfo.Instruments[0].InstrumentIdType != RBbgInstrumentIdType.NONE)
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "SECID", secInfo.Instruments[0].InstrumentIdType.ToString());
                    str.Append("SECID=");
                    str.Append(secInfo.Instruments[0].InstrumentIdType);
                }
                str.AppendLine();
                if (!isProgramFlagSet)
                {
                    RBbgUtils.AddBBGAuditHeaders(ref dictHeaders, "PROGRAMFLAG", "one-shot");
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
                    message.AppendLine(objInfo.InstrumentID + " " + objInfo.InstrumentIdType);
                    if (objInfo.MarketSector != RBbgMarketSector.None)
                    {
                        str.Append(WHITESPACE);
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
                    if (secInfo.Overrides.Count > 0)
                    {
                        str.Append("||" + secInfo.Overrides.Count + "|");
                        foreach (var overrides in secInfo.Overrides)
                        {
                            str.Append(overrides.Key + "|");
                            overridesValue.Append(overrides.Value.ToString() + "|");
                        }
                        str.Append(overridesValue.ToString());
                    }
                    str.AppendLine();
                }
                if (secInfo.MacroInfo != null && secInfo.MacroInfo.Count > 0)
                {
                    //Handle Primary Macro
                    if (secInfo.MacroInfo.ContainsKey(RBbgMacroType.Primary) &&
                        (secInfo.MacroInfo[RBbgMacroType.Primary] != null &&
                        secInfo.MacroInfo[RBbgMacroType.Primary].Count > 0))
                    {
                        List<RBbgMacroInfo> lstMacroInfo = secInfo.MacroInfo[RBbgMacroType.Primary];
                        for (int m = 0; m < lstMacroInfo.Count; m++)
                        {
                            if (RBbgUtils.IsValidPrimaryQualifierAndValue(lstMacroInfo[m],clientName))
                            {
                                str.Append(lstMacroInfo[m].QualifierType);
                                str.Append(" = ");
                                str.Append(lstMacroInfo[m].QualifierValue);
                                //handle secondary macro
                                if ((secInfo.MacroInfo.ContainsKey(RBbgMacroType.Secondary) &&
                                    secInfo.MacroInfo[RBbgMacroType.Secondary] != null) &&
                                    (secInfo.MacroInfo.ContainsKey(RBbgMacroType.Secondary)
                                    && secInfo.MacroInfo[RBbgMacroType.Secondary].Count > 0))
                                {
                                    List<RBbgMacroInfo> lstSecMacroInfo = secInfo.MacroInfo[RBbgMacroType.Secondary];
                                    for (int n = 0; n < lstSecMacroInfo.Count; n++)
                                    {
                                        if (RBbgUtils.IsSecondaryQualifierValidFTP(lstMacroInfo[m], lstSecMacroInfo[n],clientName))
                                        {
                                            RBbgMacroInfo macroInfo = lstSecMacroInfo[n];
                                            str.Append(" AND " + macroInfo.QualifierType + " = " +
                                                macroInfo.QualifierValue);
                                        }
                                    }
                                    str.Append(" | MACRO");
                                }
                                else
                                    str.Append(" | MACRO");
                                str.AppendLine();
                            }
                        }
                    }
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
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Writes the sec info on file.
        /// </summary>
        /// <param name="secInfo">The sec info.</param>
        /// <param name="requestFileName">Name of the request file.</param>
        /// <param name="responseFileName">Name of the response file.</param>
        private void WriteSecInfoOnFileForGetCompany(RBbgSecurityInfo secInfo, string requestFileName,
                                                                string responseFileName, StringBuilder message)
        {
            StringBuilder str = null;
            FileStream fs = null;
            StreamWriter sw = null;
            mLogger.Debug("Start ->writing security info on file.");
            try
            {
                string clientName = SRMMTConfig.GetClientName();

                string workingDirectory = RADConfigReader.GetServerPhysicalPath() +
                                         (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
                workingDirectory = SRMMTCommon.GetClientFilePath(workingDirectory);

                str = new StringBuilder();
                str.Append("START-OF-FILE");
                str.AppendLine();

                //Pending for Headers

                str.Append("FIRMNAME=");
                str.Append(RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.USER_ID));
                str.AppendLine();
                str.Append("FILETYPE=pc");
                str.AppendLine();
                str.Append("REPLYFILENAME=" + responseFileName);
                str.AppendLine();

                HashSet<string> restrictedHeaders = RBbgUtils.GetRestrictedHeaders(RBbgHeaderType.GetCompany);
                Dictionary<string, string> HeaderNamesVsValues = RVendorConfigLoader.GetVendorHeaders(RVendorType.Bloomberg, secInfo.VendorPreferenceId, "GetCompany");

                if (secInfo.HeaderNamesVsValues != null && secInfo.HeaderNamesVsValues.Count > 0)
                    foreach (var kvp in secInfo.HeaderNamesVsValues)
                        HeaderNamesVsValues[kvp.Key] = kvp.Value;

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
                if (secInfo.Instruments.Count > 0 && secInfo.Instruments[0].InstrumentIdType != RBbgInstrumentIdType.NONE)
                {
                    str.Append("SECID=");
                    str.Append(secInfo.Instruments[0].InstrumentIdType);
                    str.AppendLine();
                }

                str.Append("PROGRAMNAME=getcompany");
                str.AppendLine();
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
                    message.AppendLine(objInfo.InstrumentID + " " + objInfo.InstrumentIdType);
                    if (objInfo.MarketSector != RBbgMarketSector.None)
                    {
                        str.Append(WHITESPACE);
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
                    if (secInfo.Overrides.Count > 0)
                    {
                        str.Append("||" + secInfo.Overrides.Count + "|");
                        foreach (var overrides in secInfo.Overrides)
                        {
                            str.Append(overrides.Key + "|");
                            overridesValue.Append(overrides.Value.ToString() + "|");
                        }
                        str.Append(overridesValue.ToString());
                    }
                    str.AppendLine();
                }
                if (secInfo.MacroInfo != null && secInfo.MacroInfo.Count > 0)
                {
                    //Handle Primary Macro
                    if (secInfo.MacroInfo.ContainsKey(RBbgMacroType.Primary) &&
                        (secInfo.MacroInfo[RBbgMacroType.Primary] != null &&
                        secInfo.MacroInfo[RBbgMacroType.Primary].Count > 0))
                    {
                        List<RBbgMacroInfo> lstMacroInfo = secInfo.MacroInfo[RBbgMacroType.Primary];
                        for (int m = 0; m < lstMacroInfo.Count; m++)
                        {
                            if (RBbgUtils.IsValidPrimaryQualifierAndValue(lstMacroInfo[m], clientName))
                            {
                                str.Append(lstMacroInfo[m].QualifierType);
                                str.Append(" = ");
                                str.Append(lstMacroInfo[m].QualifierValue);
                                //handle secondary macro
                                if ((secInfo.MacroInfo.ContainsKey(RBbgMacroType.Secondary) &&
                                    secInfo.MacroInfo[RBbgMacroType.Secondary] != null) &&
                                    (secInfo.MacroInfo.ContainsKey(RBbgMacroType.Secondary)
                                    && secInfo.MacroInfo[RBbgMacroType.Secondary].Count > 0))
                                {
                                    List<RBbgMacroInfo> lstSecMacroInfo = secInfo.MacroInfo[RBbgMacroType.Secondary];
                                    for (int n = 0; n < lstSecMacroInfo.Count; n++)
                                    {
                                        if (RBbgUtils.IsSecondaryQualifierValidFTP(lstMacroInfo[m], lstSecMacroInfo[n], clientName))
                                        {
                                            RBbgMacroInfo macroInfo = lstSecMacroInfo[n];
                                            str.Append(" AND " + macroInfo.QualifierType + " = " +
                                                macroInfo.QualifierValue);
                                        }
                                    }
                                    str.Append(" | MACRO");
                                }
                                else
                                    str.Append(" | MACRO");
                                str.AppendLine();
                            }
                        }
                    }
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
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Decrypts the output file.
        /// </summary>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="responseFileName">Name of the response file.</param>
        /// <param name="decryptFileName">Name of the decrypt file.</param>
        /// <returns></returns>
        private string DecryptOutputFile(string workingDirectory, string responseFileName,
                                                                    string decryptFileName, bool isbval, RBbgSecurityInfo secInfo)
        {
            RCommandExecutor cmdExec = null;
            mLogger.Debug("Start ->decrypting output file.");
            try
            {
                cmdExec = new RCommandExecutor();
                string decryptCmd = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.FTPDECRYPTCOMMAND);
                string decryptKey = RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, secInfo.VendorPreferenceId, RVendorConstant.FTPDECRPYPTKEY);
                string decryptToolPath = RVendorConfigLoader.GetDecryptToolPath(secInfo.VendorPreferenceId); 
                if (string.IsNullOrEmpty(decryptToolPath))
                    return responseFileName;
                StringBuilder cmd = new StringBuilder();

                cmd.Append(WHITESPACE);
                cmd.Append(decryptCmd);
                cmd.Append(WHITESPACE);
                cmd.Append("\"");
                cmd.Append(decryptKey);
                cmd.Append("\"");
                cmd.Append(WHITESPACE);
                cmd.Append(responseFileName);
                cmd.Append(WHITESPACE);
                cmd.Append(decryptFileName);
                cmd.Append(WHITESPACE);
                cmdExec.ExecuteCommand(decryptToolPath, cmd.ToString(), workingDirectory);
                return decryptFileName;
            }
            catch (RVendorException rEx)
            {
                mLogger.Error(rEx.ToString());
                return string.Empty;
                throw rEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                if (ex.Message.ToLower().Contains("process must exit before requested information can be determined"))//&& retryAttemp < 10
                {
                    //Eat the exception
                    return decryptFileName;
                }
                else
                    throw new RVendorException(ex.Message);
            }
            finally
            {
                mLogger.Debug("End ->decrypting output file.");
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the files from FTP.
        /// </summary>
        /// <param name="transportDel">The transport del.</param>
        private void DeleteFilesFromFTP(object transportDel, string fileIdentifier)
        {
            mLogger.Debug("Start ->deleting files from FTP.");

            RFTPContent ftpContent = null;
            IRTransport transport = (IRTransport)transportDel;
            //DateTime dtEST = RBbgUtils.GetTargetDateTime();
            //string appenTime = string.Format(string.Format("{0:yyyyMMdd}",
            //                                                            dtEST));
            string appenTime = RBbgUtils.GetTargetDateTime();
            List<string> filesToDelete = new List<string>();
            filesToDelete.Add("req" + fileIdentifier + ".req.copied");
            filesToDelete.Add("rep" + fileIdentifier + ".res");
            filesToDelete.Add("rep" + fileIdentifier + ".res." + appenTime);
            try
            {
                ftpContent = new RFTPContent();
                foreach (string fileName in filesToDelete)
                {
                    ftpContent.FileName = fileName;
                    ftpContent.FolderName = "//";
                    ftpContent.Action = FTPAction.Delete;

                    object[] ftpArgs = { ftpContent };
                    transport.SendMessage(null, ftpArgs);
                }
            }
            catch (Exception ex)
            {

                mLogger.Error("Bloomberg service is working fine,the problem is with deleting" +
                              "the request and response files from FTP");
                mLogger.Error(ex.ToString());
                //throw ex;
            }
            finally
            {
                //if (transportDel != null)
                //    transportDel = null;
                if (transport != null)
                    transport = null;
                mLogger.Debug("End ->deleting files from FTP.");
            }
        }
        #endregion
    }
}
