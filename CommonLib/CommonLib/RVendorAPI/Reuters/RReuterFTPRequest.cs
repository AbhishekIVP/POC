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
 * 1            01-12-2008      Manoj          Initial Version
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.transport;
using System.IO;
using System.Threading;
using com.ivp.rad.utils;
using System.Data;
using com.ivp.rad.common;
using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.configurationmanagement;
using SecuritiesCollection = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, com.ivp.srm.vendorapi.RVendorFieldInfo>>;

namespace com.ivp.srm.vendorapi.reuters
{
    /// <summary>
    /// Implementation of Reuters FTP
    /// </summary>
    internal class RReuterFTPRequest
    {
        #region "Member Variables"
        static IRLogger mLogger = RLogFactory.CreateLogger("RReuterFTPRequest");
        //string mCurrentTimeStamp = "";
        #endregion
        //public string RequestedInstruments { get; set; }
        //public string RequestedFields { get; set; }
        #region "Local Enum"
        /// <summary>
        /// File Type for generation of file name
        /// </summary>
        enum FileType
        {
            /// <summary>
            /// Input file name
            /// </summary>
            Input,
            /// <summary>
            /// Output file name
            /// </summary>
            Ouput
        }
        #endregion

        //#region "Constructor"
        ///// <summary>
        ///// Initializes a new instance of the <see cref="RReuterFTPRequest"/> class.
        ///// </summary>
        //private void SetTimeStamp()
        //{
        //    TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(
        //             RVendorConfigLoader.GetVendorConfig
        //            (RVendorType.Reuters)[RVendorConstant.FTPTIMEZONE]);
        //    DateTime dtEST = RFormatterUtils.ConvertDateToSpecifiedTimeZone
        //                    (System.DateTime.Now, tz.StandardName);
        //        mCurrentTimeStamp = string.Format(string.Format("{0:MMddyyHHmmss}",
        //                                                                    dtEST));
        //}
        //#endregion

        #region "Public Methods"
        /// <summary>
        /// Gets the securities.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        internal object GetSecurities(RReuterSecurityInfo securityInfo, bool immediateRequest, string timeStamp)
        {
            //SetTimeStamp();
            mLogger.Debug("Start->Get Securities using Reuters FTP Call");
            RFTPContent ftpContent = null;
            IRTransport transport = null;

            string mWorkingDirectory = RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
            GenerateXmlFile(timeStamp, securityInfo);
            transport = new RTransportManager().GetTransport(securityInfo.TransportName);
            //RequestedInstruments = RReuterUtils.GetCSVInstruments(securityInfo.Instruments);
            //RequestedFields = RReuterUtils.GetCSVFields(securityInfo.InstrumentFields);
            #region "Send File"
            ftpContent = new RFTPContent();
            //mLogger.Debug("mCurrentTimeStamp:" + mCurrentTimeStamp); 
            ftpContent.FileName = Path.Combine(mWorkingDirectory, GetFileName(timeStamp, FileType.Input));
            //mLogger.Debug("mCurrentTimeStamp:" + ftpContent.FileName);
            ftpContent.FolderName = RVendorConfigLoader.GetVendorConfig(RVendorType.Reuters, securityInfo.VendorPreferenceId, RVendorConstant.INPUT_DIRECTORY);
            ftpContent.Action = FTPAction.Upload;
            object[] ftpArgs = { ftpContent };
            try
            {
                mLogger.Debug("Start->Sending Input file to Reuters FTP Location");
                transport.SendMessage(null, ftpArgs);
            }
            catch (RTransportException transEx)
            {
                throw transEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex);
            }
            finally
            {
                mLogger.Debug("End->Sending Input file to Reuters FTP Location");
            }
            #endregion
            mLogger.Debug("End->Get Securities using Reuters FTP Call");
            if (immediateRequest)
                return GetResponse(timeStamp, securityInfo.TransportName, securityInfo);
            return timeStamp;
        }

        public SecuritiesCollection GetResponse(string timeStamp, string transportName, RReuterSecurityInfo securityInfo)
        {
            RFTPContent ftpContent = null;
            IRTransport transport = null;
            SecuritiesCollection dictResult = null;

            string mWorkingDirectory = RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
            transport = new RTransportManager().GetTransport(transportName);
            #region "Request File"
            ftpContent = new RFTPContent();
            ftpContent.FileName = Path.Combine(RVendorConfigLoader.GetVendorConfig(RVendorType.Reuters, securityInfo.VendorPreferenceId, RVendorConstant.REPORTS_DIRECTORY),
                                        GetFileName(timeStamp, FileType.Ouput));
            ftpContent.FolderName = mWorkingDirectory;
            object[] ftpArg = { ftpContent };
            try
            {
                mLogger.Debug("Start->Requesting Output file from Reuters FTP Location");
                transport.ReceiveMessage(null, ftpArg);
            }
            catch (RTransportException transEx)
            {
                throw transEx;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new RVendorException(ex);
            }
            finally
            {
                mLogger.Debug("End->Requesting Output file from Reuters FTP Location");
            }
            #endregion

            #region "Read File"
            RParserUtils parser = new RParserUtils();
            DataTable dt = parser.CSVToDataTable(Path.Combine(mWorkingDirectory, GetFileName(timeStamp, FileType.Ouput)), '0', '0', true);
            dictResult = GetDictionary(dt, securityInfo);
            #endregion

            //#region "Delete Files"
            //ParameterizedThreadStart prmThread = new ParameterizedThreadStart(DeleteFilesFromFTP);
            //Thread thread = new Thread(prmThread);
            //thread.Start(transport);
            //#endregion
            return dictResult;
        }
        #endregion

        #region "Private Methods"
        /// <summary>
        /// Generates the XML file.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        private void GenerateXmlFile(string timeStamp, RReuterSecurityInfo securityInfo)
        {
            mLogger.Debug("Start-> Generate XML File for Reuters Request");
            FileStream fs = null;
            StreamWriter sw = null;

            string mWorkingDirectory = RADConfigReader.GetServerPhysicalPath() + (RVendorConfigLoader.GetVendorConfig(RVendorType.Bloomberg, securityInfo.VendorPreferenceId, RVendorConstant.WORKINGDIRECTORY));
            string xml = GenerateXml(timeStamp, securityInfo);

            fs = new FileStream(Path.Combine(mWorkingDirectory, GetFileName(timeStamp, FileType.Input)),
                                    FileMode.Create, FileAccess.Write, FileShare.Read);
            sw = new StreamWriter(fs);
            try
            {
                sw.Write(xml);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
                throw new RVendorException(ex);
            }
            finally
            {
                sw.Close();
                fs.Close();
                sw.Dispose();
                fs.Dispose();
                mLogger.Debug("End-> Generate XML File for Reuters Request");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        private SecuritiesCollection GetDictionary(DataTable dt, RReuterSecurityInfo securityInfo)
        {
            mLogger.Debug("Start-> Get reponse object of Reuters from Datatable");
            SecuritiesCollection dict = new SecuritiesCollection(StringComparer.OrdinalIgnoreCase);
            int i = 0;
            string[] instruments = securityInfo.Instruments.Select(x => x.InstrumentID).ToArray();
            foreach (DataRow dr in dt.Rows)
            {
                var vendorFieldInfo = new Dictionary<string, RVendorFieldInfo>(StringComparer.OrdinalIgnoreCase);
                foreach (DataColumn dc in dt.Columns)
                {
                    var fieldInfo = new RVendorFieldInfo();
                    fieldInfo.Name = dc.ColumnName;
                    fieldInfo.Value = dr[dc.ColumnName].ToString();
                    if (fieldInfo.Value != null)
                    {
                        fieldInfo.Value = fieldInfo.Value.Trim();
                        if (fieldInfo.Value.StartsWith("\""))
                            fieldInfo.Value = fieldInfo.Value.Substring(1, fieldInfo.Value.Length - 1);
                        if (fieldInfo.Value.EndsWith("\""))
                            fieldInfo.Value = fieldInfo.Value.Substring(0, fieldInfo.Value.Length - 1);
                    }
                    fieldInfo.Status = RVendorStatusConstant.PASSED;
                    fieldInfo.ExceptionMessage = "";
                    vendorFieldInfo.Add(fieldInfo.Name, fieldInfo);
                }
                dict.Add(instruments[i], vendorFieldInfo);
                i++;
            }
            mLogger.Debug("End-> Get reponse object of Reuters from Datatable");
            return dict;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Generates the XML.
        /// </summary>
        /// <param name="securityInfo">The security info.</param>
        /// <returns></returns>
        private string GenerateXml(string timeStamp, RReuterSecurityInfo securityInfo)
        {
            Dictionary<string, string> HeaderNamesVsValues = RVendorConfigLoader.GetVendorHeaders(RVendorType.Reuters, securityInfo.VendorPreferenceId, RReuterHeaderType.FTP.ToString());
            string reportTemplateName = "IVP Report Template";
            string inputListName = "IVP Input List";
            string reportTemplateTag = "TermsandConditions";
            string scheduleName = "ImmediateSchedule";
            string scheduleTag = "TermsandConditionsSchedule";

            if (HeaderNamesVsValues != null && HeaderNamesVsValues.ContainsKey(RVendorConstant.REPORT_TEMPLATE_NAME))
                reportTemplateName = HeaderNamesVsValues[RVendorConstant.REPORT_TEMPLATE_NAME];
            if (HeaderNamesVsValues != null && HeaderNamesVsValues.ContainsKey(RVendorConstant.INPUT_LIST_NAME))
                inputListName = HeaderNamesVsValues[RVendorConstant.INPUT_LIST_NAME];
            if (HeaderNamesVsValues != null && HeaderNamesVsValues.ContainsKey(RVendorConstant.REPORT_TEMPLATE_TAG))
                reportTemplateTag = HeaderNamesVsValues[RVendorConstant.REPORT_TEMPLATE_TAG];
            if (HeaderNamesVsValues != null && HeaderNamesVsValues.ContainsKey(RVendorConstant.SCHEDULE_NAME))
                scheduleName = HeaderNamesVsValues[RVendorConstant.SCHEDULE_NAME];
            if (HeaderNamesVsValues != null && HeaderNamesVsValues.ContainsKey(RVendorConstant.SCHEDULE_TAG))
                scheduleTag = HeaderNamesVsValues[RVendorConstant.SCHEDULE_TAG];

            mLogger.Debug("Start->Generate XML for Reuters Request. This XML will be used to write into the file");
            StringBuilder builder = new StringBuilder();
            builder.Append("<?xml version=\"1.0\"?>");
            builder.Append("<ReportRequest xmlns=\"http://www.reuters.com/Datascope/ReportRequest.xsd\">");

            #region "Generate InputList"
            builder.Append("<InputList>");
            builder.Append("<InputListAction>Replace</InputListAction>");
            builder.Append("<Name>" + inputListName + "</Name>");
            builder.Append(GetInstruments(securityInfo.Instruments));
            builder.Append("</InputList>");
            #endregion

            #region "Generate Report Template"
            builder.Append("<ReportTemplate>");
            builder.Append("<ReportAction>Replace</ReportAction>");
            builder.Append("<Name>" + reportTemplateName + "</Name>");
            builder.Append("<" + reportTemplateTag + ">");
            builder.Append("<OutputFormat>CSV</OutputFormat>");
            builder.Append("<Delimiter>None</Delimiter>");
            builder.Append("<Delivery>None</Delivery>");
            builder.Append("<Destination> </Destination>");
            builder.Append("<Compression>None</Compression>");
            builder.Append("<ColumnHeaders>Yes</ColumnHeaders>");
            builder.Append("<Body>");
            builder.Append(GetDataFields(securityInfo.InstrumentFields));
            builder.Append("</Body>");
            builder.Append("</" + reportTemplateTag + ">");
            builder.Append("</ReportTemplate>");
            #endregion

            #region "Generate Schedule"
            builder.Append("<Schedule>");
            builder.Append("<ScheduleAction>Replace</ScheduleAction>");
            builder.Append("<Name>" + scheduleName + "</Name>");
            builder.Append("<" + scheduleTag + ">");
            builder.Append("<InputListName>" + inputListName + "</InputListName>");
            builder.Append("<ReportTemplateName>" + reportTemplateName + "</ReportTemplateName>");
            builder.Append("<OutputFileName>");
            builder.Append(GetFileName(timeStamp, FileType.Ouput));
            builder.Append("</OutputFileName>");
            builder.Append("<ScheduleImmediate>");
            builder.Append("<Type>LastUpdate</Type>");
            builder.Append("</ScheduleImmediate>");
            builder.Append("</" + scheduleTag + ">");
            builder.Append("</Schedule>");
            #endregion

            builder.Append("</ReportRequest>");
            mLogger.Debug("End->Generate XML for Reuters Request. This XML will be used to write into the file");
            return builder.ToString();
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <param name="fileType">Type of the file.</param>
        /// <returns></returns>
        private string GetFileName(string timeStamp, FileType fileType)
        {
            string fileDirection = "";
            string fileExtension = "";
            switch (fileType)
            {
                case FileType.Input:
                    fileDirection = "req";
                    fileExtension = ".xml";
                    break;
                case FileType.Ouput:
                    fileDirection = "rep";
                    fileExtension = ".csv";
                    break;
            }
            //fileName = appenTime;
            return fileDirection + timeStamp + fileExtension;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the data fields.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private string GetDataFields(List<string> list)
        {
            StringBuilder builder = new StringBuilder();
            int fieldIndex = 0;
            foreach (string field in list)
            {
                builder.Append("<DataField>");
                builder.Append("<Name>");
                builder.Append(GetXmlFormattedText(field));
                builder.Append("</Name>");

                builder.Append("<FieldNumber>");
                builder.Append(fieldIndex);
                builder.Append("</FieldNumber>");

                builder.Append("<DataFieldType>Text</DataFieldType>");
                builder.Append("<Justification>Left</Justification>");
                builder.Append("<Capitalization>None</Capitalization>");

                builder.Append("</DataField>");
                fieldIndex++;
            }
            return builder.ToString();
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the instruments.
        /// </summary>
        /// <param name="instruments">The instruments.</param>
        /// <returns></returns>
        private string GetInstruments(List<RReuterInstrumentInfo> instruments)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RReuterInstrumentInfo inst in instruments)
            {
                builder.Append("<Instrument>");
                builder.Append("<IdentifierType>");
                builder.Append(inst.InstrumentIdType);
                builder.Append("</IdentifierType>");
                builder.Append("<Identifier>");
                builder.Append(GetXmlFormattedText(inst.InstrumentID));
                builder.Append("</Identifier>");
                builder.Append("</Instrument>");
            }
            return builder.ToString();
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the XML formatted text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string GetXmlFormattedText(string text)
        {
            if (text == null || text.Trim() == "")
                return text.Trim();
            text = text.Replace("&", "&amp;");
            text = text.Replace("'", "&#39;");
            text = text.Replace("<", "&lt;");
            text = text.Replace(">", "&gt;");
            text = text.Replace("\"", "&quot;");
            return text.Trim();
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the files from FTP.
        /// </summary>
        /// <param name="transportDel">The transport del.</param>
        private void DeleteFilesFromFTP(object transportDel, string timeStamp, int VendorPreferenceId)
        {
            mLogger.Debug("Start ->Delete files from Reuter FTP.");

            RFTPContent ftpContentRequestFile = null;
            IRTransport transport = (IRTransport)transportDel;
            try
            {

                ftpContentRequestFile = new RFTPContent();
                ftpContentRequestFile.Action = FTPAction.Delete;

                Dictionary<string, string> filesToDelete = GetFilesToDelete(timeStamp, VendorPreferenceId);
                foreach (string fileName in filesToDelete.Keys)
                {
                    ftpContentRequestFile.FileName = fileName;
                    ftpContentRequestFile.FolderName = filesToDelete[fileName];
                    object[] ftpArgs = { ftpContentRequestFile };
                    transport.SendMessage(null, ftpArgs);
                }
            }
            catch (Exception ex)
            {

                mLogger.Error("Reuter service is working fine,the problem is with deleting" +
                              "the request and response files from FTP");
                throw new RVendorException(ex);
            }
            finally
            {
                if (transport != null)
                    transport = null;
                mLogger.Debug("End ->Delete files from Reuter FTP.");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the files to delete.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetFilesToDelete(string timeStamp, int VendorPreferenceId)
        {
            Dictionary<string, string> filesToDelete = new Dictionary<string, string>();
            string inputFileName = GetFileName(timeStamp, FileType.Input);
            string inputLocation = RVendorConfigLoader.GetVendorConfig(RVendorType.Reuters, VendorPreferenceId, RVendorConstant.COPY_DIRECTORY);

            string outputLocation = RVendorConfigLoader.GetVendorConfig(RVendorType.Reuters, VendorPreferenceId, RVendorConstant.REPORTS_DIRECTORY);
            string ouputFileName = GetFileName(timeStamp, FileType.Ouput);
            filesToDelete.Add(inputFileName, inputLocation);
            filesToDelete.Add(inputFileName + ".notes.txt", inputLocation);
            filesToDelete.Add(ouputFileName, outputLocation);
            filesToDelete.Add(ouputFileName + ".notes.txt", outputLocation);
            filesToDelete.Add(ouputFileName.Substring(0, ouputFileName.LastIndexOf(".")) + ".ric.csv", outputLocation);
            return filesToDelete;
        }
        #endregion
    }
}
