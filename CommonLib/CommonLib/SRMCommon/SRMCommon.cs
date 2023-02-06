using com.ivp.rad.common;
using com.ivp.rad.data;
using com.ivp.rad.ExcelCreator;
using com.ivp.rad.excellibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.GAC;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using com.ivp.commom.commondal;
using System.Diagnostics;
using com.ivp.rad.xruleengine;
using com.ivp.rad.dal;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.controls.xruleeditor;
using Irony.Parsing;
using com.ivp.rad.controls.xruleeditor.grammar;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using com.ivp.rad.servicesecuritymanagement;
using com.ivp.rad.utils;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using JWT.Serializers;
using JWT;
using JWT.Algorithms;
using Newtonsoft.Json;
using JWT.Exceptions;
using com.ivp.rad.cryptography;
using System.Net;
using System.Xml;
using System.Web;
using System.Security.Principal;
using System.Collections.Concurrent;
using System.Threading;

namespace com.ivp.srmcommon
{
    public class SRMCommon
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SMAttributeSetup");

        private static HashSet<string> lstBooleanTrue = new HashSet<string>() { "yes", "y", "1", "true" };
        private static HashSet<string> lstBooleanFalse = new HashSet<string>() { "no", "n", "0", "false" };
        static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> dctClientVsConfigEntries = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        public static byte[] GetExcelByteArray(DataSet dsExcel)
        {
            try
            {
                mLogger.Debug("SMExcelManager : GetExcelByteArray -> Start");
                RCreateMultiSheetExcel objRCreateMultiSheetExcel = new RCreateMultiSheetExcel();
                RExcelMultiSheetInfo objRExcelMultiSheetInfo = new RExcelMultiSheetInfo();
                objRExcelMultiSheetInfo.BreakAfterTable = new Dictionary<string, bool>();
                objRExcelMultiSheetInfo.ReportDataSource = dsExcel;
                Dictionary<string, string> sheetNames = new Dictionary<string, string>();
                foreach (DataTable dt in dsExcel.Tables)
                {
                    sheetNames.Add(dt.TableName, dt.TableName);
                    objRExcelMultiSheetInfo.BreakAfterTable.Add(dt.TableName, true);
                }

                IAssemblyEnum ae = AssemblyCache.CreateGACEnum();
                IAssemblyName nameRef;
                AssemblyName name;
                Assembly ass = null;
                while (AssemblyCache.GetNextAssembly(ae, out nameRef) == 0)
                {
                    if (AssemblyCache.GetName(nameRef) == "Microsoft.Office.Interop.Excel")
                    {
                        name = new AssemblyName();
                        name.Name = AssemblyCache.GetName(nameRef);
                        name.Version = AssemblyCache.GetVersion(nameRef);
                        name.CultureInfo = AssemblyCache.GetCulture(nameRef);
                        name.SetPublicKeyToken(AssemblyCache.GetPublicKeyToken(nameRef));
                        ass = Assembly.Load(name);
                        break;
                    }
                }
                if (ass != null)
                {
                    mLogger.Error("Microsoft.Office.Interop.Excel found");
                    objRExcelMultiSheetInfo.ReportName = sheetNames;
                    return RCOMExcelCreator.CreateExcel(objRExcelMultiSheetInfo);
                }
                else
                {
                    mLogger.Error("Microsoft.Office.Interop.Excel not found");
                    string excelStr = objRCreateMultiSheetExcel.GetMultiSheetExcel(objRExcelMultiSheetInfo).ToString();
                    return Encoding.UTF8.GetBytes(excelStr);
                }

            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
            }
            finally
            {
                mLogger.Debug("SMExcelManager : GetExcelByteArray -> End");
            }

            return null;
        }

        //private static byte[] GetExcelByteArray(ObjectSet dsExcel)
        //{
        //    try
        //    {
        //        mLogger.Debug("SMExcelManager : GetExcelByteArray -> Start");
        //        RCreateMultiSheetExcel objRCreateMultiSheetExcel = new RCreateMultiSheetExcel();
        //        RExcelMultiSheetInfo objRExcelMultiSheetInfo = new RExcelMultiSheetInfo();
        //        objRExcelMultiSheetInfo.BreakAfterTable = new Dictionary<string, bool>();
        //        objRExcelMultiSheetInfo.ReportDataSource = ConvertObjectSetToDataSet(dsExcel);
        //        Dictionary<string, string> sheetNames = new Dictionary<string, string>();
        //        foreach (ObjectTable dt in dsExcel.Tables)
        //        {
        //            sheetNames.Add(dt.TableName, dt.TableName);
        //            objRExcelMultiSheetInfo.BreakAfterTable.Add(dt.TableName, true);
        //        }

        //        IAssemblyEnum ae = AssemblyCache.CreateGACEnum();
        //        IAssemblyName nameRef;
        //        AssemblyName name;
        //        Assembly ass = null;
        //        while (AssemblyCache.GetNextAssembly(ae, out nameRef) == 0)
        //        {
        //            if (AssemblyCache.GetName(nameRef) == "Microsoft.Office.Interop.Excel")
        //            {
        //                name = new AssemblyName();
        //                name.Name = AssemblyCache.GetName(nameRef);
        //                name.Version = AssemblyCache.GetVersion(nameRef);
        //                name.CultureInfo = AssemblyCache.GetCulture(nameRef);
        //                name.SetPublicKeyToken(AssemblyCache.GetPublicKeyToken(nameRef));
        //                ass = Assembly.Load(name);
        //                break;
        //            }
        //        }
        //        if (ass != null)
        //        {
        //            mLogger.Error("Microsoft.Office.Interop.Excel found");
        //            objRExcelMultiSheetInfo.ReportName = sheetNames;
        //            return RCOMExcelCreator.CreateExcel(objRExcelMultiSheetInfo);
        //        }
        //        else
        //        {
        //            mLogger.Error("Microsoft.Office.Interop.Excel not found");
        //            string excelStr = objRCreateMultiSheetExcel.GetMultiSheetExcel(objRExcelMultiSheetInfo).ToString();
        //            return Encoding.UTF8.GetBytes(excelStr);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        mLogger.Error(ex.ToString());
        //    }
        //    finally
        //    {
        //        mLogger.Debug("SMExcelManager : GetExcelByteArray -> End");
        //    }

        //    return null;
        //}

        public static byte[] GetExcelByteArrayNew(DataSet dsExcel)
        {
            mLogger.Debug("SRMCommon -> GetExcelByteArrayNew -> Start");
            try
            {
                RExcelSheetInfo objRExcelSheetInfo = new RExcelSheetInfo();

                objRExcelSheetInfo.reportData = dsExcel;
                List<RSheetInfo> sheetNames = new List<RSheetInfo>();

                foreach (DataTable dt in dsExcel.Tables)
                {
                    RSheetInfo objRSheetInfo = new RSheetInfo();
                    objRSheetInfo.tableName = dt.TableName;
                    objRSheetInfo.sheetName = dt.TableName;
                    sheetNames.Add(objRSheetInfo);
                }
                objRExcelSheetInfo.sheetInfo = sheetNames;
                return new RExcelManager().ExportExcel(objRExcelSheetInfo);
            }
            catch (Exception ex)
            {
                mLogger.Debug("SRMCommon -> GetExcelByteArrayNew -> Error : " + ex.Message.ToString());
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommon -> GetExcelByteArrayNew -> End");
            }
        }

        public static byte[] GetExcelByteArrayNew(ObjectSet dsExcel)
        {
            mLogger.Debug("SRMCommon -> GetExcelByteArrayNew -> Start");
            try
            {
                RExcelSheetInfo objRExcelSheetInfo = new RExcelSheetInfo();

                objRExcelSheetInfo.reportData = ConvertObjectSetToDataSet(dsExcel);
                List<RSheetInfo> sheetNames = new List<RSheetInfo>();

                foreach (ObjectTable dt in dsExcel.Tables)
                {
                    RSheetInfo objRSheetInfo = new RSheetInfo();
                    objRSheetInfo.tableName = dt.TableName;
                    objRSheetInfo.sheetName = dt.TableName;
                    sheetNames.Add(objRSheetInfo);
                }
                objRExcelSheetInfo.sheetInfo = sheetNames;
                return new RExcelManager().ExportExcel(objRExcelSheetInfo);
            }
            catch (Exception ex)
            {
                mLogger.Debug("SRMCommon -> GetExcelByteArrayNew -> Error : " + ex.Message.ToString());
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommon -> GetExcelByteArrayNew -> End");
            }
        }

        public static string WriteMultipleFiles(string basePath, Dictionary<string, ObjectSet> fileNameVsDataSet, int moduleId, bool isDownloadDiff, bool isUpload, string userName, bool IsXML, string zipFileNamebase)
        {
            try
            {
                string tempFileLocation = "MigrationUtilityFiles\\" + userName + "\\" + DateTime.Now.ToString("yyyy-MM-dd--hh--mm--ss") + "\\"; // + randomly generated suffix 

                //adding directories
                string actionDirectory = "Download";

                if (isDownloadDiff)
                {
                    actionDirectory = "Compare Result";
                    tempFileLocation += actionDirectory;
                }
                else if (isUpload)
                {
                    actionDirectory = "Upload Result";
                    tempFileLocation += actionDirectory;
                }
                else
                {
                    tempFileLocation += actionDirectory;
                    actionDirectory = "";
                }
            ;

                //single file
                if (fileNameVsDataSet.Count == 1)
                {
                    if (IsXML)
                        return WriteXMLFile(basePath + tempFileLocation, fileNameVsDataSet.First().Key + (string.IsNullOrEmpty(actionDirectory) ? "" : " - " + actionDirectory), "xml", fileNameVsDataSet.First().Value);
                    else
                        return WriteExcelFile(basePath + tempFileLocation, fileNameVsDataSet.First().Key + (string.IsNullOrEmpty(actionDirectory) ? "" : " - " + actionDirectory), "xlsx", fileNameVsDataSet.First().Value);
                }
                //zip file
                else
                {
                    string locationToWriteFiles = tempFileLocation + "\\" + "FilesToBeZipped";
                    string zipFileName = zipFileNamebase + (string.IsNullOrEmpty(actionDirectory) ? "" : " - " + actionDirectory) + ".7z";
                    string locationToPutZippedFile = tempFileLocation;
                    foreach (var item in fileNameVsDataSet)
                    {
                        string fileNameTemp = item.Key + (string.IsNullOrEmpty(actionDirectory) ? "" : " - " + actionDirectory);
                        if (IsXML)
                            WriteXMLFile(basePath + locationToWriteFiles, fileNameTemp, "xml", item.Value);
                        else
                            WriteExcelFile(basePath + locationToWriteFiles, fileNameTemp, "xlsx", item.Value);
                    }
                    //File.Delete(basePath + locationToPutZippedFile + "\\Test");
                    SRMCommonZip objSRMCommonZip = new SRMCommonZip();
                    string locationToReadFilesForZipping = tempFileLocation + "\\" + zipFileNamebase + (string.IsNullOrEmpty(actionDirectory) ? "" : " - " + actionDirectory);
                    if (CopyDirectoryContents(basePath + locationToWriteFiles, basePath + locationToReadFilesForZipping))
                        objSRMCommonZip.Zip(basePath + locationToReadFilesForZipping, basePath + locationToPutZippedFile + "\\" + zipFileName);
                    else
                        throw new Exception("Error while zipping");
                    return basePath + locationToPutZippedFile + "\\" + zipFileName;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CopyDirectoryContents(string sourcePath, string targetPath)
        {
            try
            {
                string fileName = string.Empty;
                string destFile = string.Empty;
                if (System.IO.Directory.Exists(sourcePath))
                {
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }
                    string[] files = System.IO.Directory.GetFiles(sourcePath);

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(targetPath, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        //file type? (zip add later)
        //returns file path
        //mlogger debugging
        private static string WriteExcelFile(string filePath, string fileName, string fileExtension, ObjectSet fileData)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    throw new Exception("Empty File Name");
                if (string.IsNullOrEmpty(filePath))
                    throw new Exception("No file path provided");
                if (string.IsNullOrEmpty(fileExtension))
                    throw new Exception("No file extension provided");

                if (!(fileExtension == "xls" || fileExtension == "xlsx"))
                    throw new Exception("Incorrect File Format provided");
                //fileData empty TODO

                byte[] byteData = GetExcelByteArrayNew(fileData);
                string fileFullName = fileName + "." + fileExtension;

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                filePath += "\\" + fileFullName;
                DeleteFile(filePath);

                //write file
                File.WriteAllBytes(filePath, byteData);
                return filePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string WriteXMLFile(string filePath, string fileName, string fileExtension, ObjectSet fileData)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    throw new Exception("Empty File Name");
                if (string.IsNullOrEmpty(filePath))
                    throw new Exception("No file path provided");
                if (string.IsNullOrEmpty(fileExtension))
                    throw new Exception("No file extension provided");

                if (!(fileExtension == "xml"))
                    throw new Exception("Incorrect File Format provided");
                //fileData empty TODO


                string fileFullName = fileName + "." + fileExtension;

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                filePath += "\\" + fileFullName;

                DeleteFile(filePath);
                //write file
                ConvertObjectSetToDataSet(fileData).WriteXml(filePath, XmlWriteMode.WriteSchema);
                return filePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string WriteSqlPlanFile(string filePath, string fileName, string fileExtension, string QueryPlan)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    throw new Exception("Empty File Name");
                if (string.IsNullOrEmpty(filePath))
                    throw new Exception("No file path provided");
                if (string.IsNullOrEmpty(fileExtension))
                    throw new Exception("No file extension provided");

                if (!(fileExtension == "sqlplan"))
                    throw new Exception("Incorrect File Format provided");
                XDocument doc = XDocument.Parse(QueryPlan);

                string fileFullName = fileName + "." + fileExtension;

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                filePath += "\\" + fileFullName;

                DeleteFile(filePath);
                //write file
                doc.Save(filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string WriteXDLFile(string filePath, string fileName, string fileExtension, string DeadLockXMLData)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    throw new Exception("Empty File Name");
                if (string.IsNullOrEmpty(filePath))
                    throw new Exception("No file path provided");
                if (string.IsNullOrEmpty(fileExtension))
                    throw new Exception("No file extension provided");

                if (!(fileExtension == "xml"))
                    throw new Exception("Incorrect File Format provided");
                //fileData empty TODO
                XDocument doc = XDocument.Parse(DeadLockXMLData);

                string fileFullName = fileName + "." + fileExtension;

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                filePath += "\\" + fileFullName;

                DeleteFile(filePath);
                //write file
                doc.Save(filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static DataSet parseExcel(XElement xe)
        {
            DataSet dsExcel = new DataSet();
            int i = 0;
            if (xe != null)
            {
                IEnumerable<XElement> worksheets = xe.Elements(XName.Get("{" + xe.Name.NamespaceName + "}Worksheet"));
                foreach (XElement ws in worksheets)
                {
                    dsExcel.Tables.Add(ws.Attribute(XName.Get("{" + xe.Name.NamespaceName + "}Name")).Value);

                    IEnumerable<XElement> rows = ws.Elements(XName.Get("{" + xe.Name.NamespaceName + "}Table")).Elements(XName.Get("{" + xe.Name.NamespaceName + "}Row"));

                    rows.ToList().ForEach(row =>
                    {
                        if (rows.First().Equals(row))
                        {
                            i = 0;
                            row.Elements(XName.Get("{" + xe.Name.NamespaceName + "}Cell")).ToList().ForEach(y =>
                            {
                                dsExcel.Tables[dsExcel.Tables.Count - 1].Columns.Add("Column" + i, typeof(string));
                                i++;
                            });
                        }
                        i = 0;
                        dsExcel.Tables[dsExcel.Tables.Count - 1].Rows.Add();
                        row.Elements(XName.Get("{" + xe.Name.NamespaceName + "}Cell")).ToList().ForEach(z =>
                        {
                            dsExcel.Tables[dsExcel.Tables.Count - 1].Rows[dsExcel.Tables[dsExcel.Tables.Count - 1].Rows.Count - 1][i] = z.Element(XName.Get("{" + xe.Name.NamespaceName + "}Data")).Value;
                            i++;
                        });

                    });
                }
            }

            return dsExcel;
        }



        /// <summary>
        /// Get Security Lookup Data
        /// </summary>
        /// <param name="inputInfo">Input info per security type. Will be 0 for All Sec Types (common)</param>
        /// <param name="isSecIdToValues">True if attribute values are needed from sec ids. False otherwise. If null, all securities should be fetched.</param>
        /// <returns></returns>
        public List<SecurityLookupOutputInfo> GetSecurityLookupData(SecurityLookupInputInfo inputInfo)
        {
            string tableNameOrg = string.Empty;
            string tableName = string.Empty;

            List<SecurityLookupOutputInfo> result = new List<SecurityLookupOutputInfo>();

            try
            {
                if (inputInfo != null && inputInfo.searchObjects != null && inputInfo.searchObjects.Count > 0)
                {
                    DataTable dtInput = new DataTable();
                    dtInput.Columns.Add(new DataColumn("row_id", typeof(int)));
                    dtInput.Columns.Add(new DataColumn("sectype_id", typeof(int)));
                    dtInput.Columns.Add(new DataColumn("attribute_id", typeof(int)));
                    dtInput.Columns.Add(new DataColumn("value", typeof(string)));
                    dtInput.Columns.Add(new DataColumn("original_value", typeof(string)));

                    string timePartFormat = string.Empty;

                    if (!string.IsNullOrEmpty(inputInfo.DateTimeFormat))
                    {
                        int index = inputInfo.DateTimeFormat.IndexOf(" ");
                        if (index > -1)
                            timePartFormat = inputInfo.DateTimeFormat.Substring(index + 1, inputInfo.DateTimeFormat.Length - (index + 1));
                    }

                    HashSet<int> attributeIds = new HashSet<int>(inputInfo.searchObjects.Select(x => x.SecurityAttributeID));

                    Dictionary<int, string> attributeIdVsDataType = new Dictionary<int, string>();
                    if (attributeIds != null && attributeIds.Count > 0)
                        attributeIdVsDataType = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT adet.attribute_id, data_type_name
                        FROM IVPSecMaster.dbo.ivp_secm_attribute_details adet
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_subtype styp
                        ON styp.attribute_subtype_id = adet.attribute_subtype_id
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_type atyp
                        ON atyp.attribute_type_id = styp.attribute_type_id
                        INNER JOIN IVPSecMaster.dbo.SECM_GetList2Table('{0}', ',') attrs
                        ON attrs.item = adet.attribute_id", string.Join(",", attributeIds)), ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToInt32(x["attribute_id"]), y => Convert.ToString(y["data_type_name"]));

                    int rowId = 1;
                    foreach (var input in inputInfo.searchObjects)
                    {
                        if (input.ValuesToSearch != null && input.ValuesToSearch.Count > 0)
                        {
                            if (!attributeIdVsDataType.ContainsKey(input.SecurityAttributeID))
                                continue;

                            //XElement xele = new XElement("securities");
                            List<KeyValuePair<string, string>> lstValues = new List<KeyValuePair<string, string>>();

                            bool isAnyValid = false;
                            foreach (var value in input.ValuesToSearch)
                            {
                                object val;
                                string errMessage;
                                bool isValid = false;
                                string attrValue = Convert.ToString(value);

                                if (string.IsNullOrEmpty(attrValue))
                                    continue;

                                if (inputInfo.isSecIdToValues)
                                {
                                    val = attrValue;
                                    isValid = true;
                                }
                                else
                                    isValid = ValidateDataType(string.Empty, attributeIdVsDataType[input.SecurityAttributeID], attrValue, inputInfo.DateTimeFormat, inputInfo.DateFormat, inputInfo.TimeFormat, timePartFormat, out val, out errMessage);

                                if (isValid)
                                {
                                    isAnyValid = true;
                                    //xele.Add(new XElement("security", new XAttribute("value", val), new XAttribute("original_value", value)));
                                    lstValues.Add(new KeyValuePair<string, string>(Convert.ToString(val), Convert.ToString(value)));
                                }
                            }

                            if (isAnyValid)
                            {
                                //XDocument xdoc = new XDocument(xele);

                                foreach (var item in lstValues)
                                {
                                    DataRow dr = dtInput.NewRow();
                                    dr["row_id"] = rowId;
                                    dr["sectype_id"] = input.SecurityTypeID;
                                    dr["attribute_id"] = input.SecurityAttributeID;
                                    dr["value"] = item.Key;
                                    dr["original_value"] = item.Value;

                                    dtInput.Rows.Add(dr);
                                }

                                rowId++;
                            }
                            else
                                AddBlankResultForLookupData(ref result, input.SecurityTypeID, input.SecurityAttributeID);
                        }
                        else
                            AddBlankResultForLookupData(ref result, input.SecurityTypeID, input.SecurityAttributeID);
                    }

                    if (dtInput.Rows.Count > 0)
                    {
                        tableNameOrg = "LookupSecurities_" + Guid.NewGuid().ToString().Replace("-", "_");
                        tableName = "IVPSecMaster.dbo." + tableNameOrg;

                        CommonDALWrapper.ExecuteSelectQuery(string.Format(@"CREATE TABLE {0} (row_id INT, sectype_id INT, attribute_id INT, value VARCHAR(MAX), original_value VARCHAR(MAX))", tableName), ConnectionConstants.SecMaster_Connection);
                        CommonDALWrapper.ExecuteBulkUpload(tableName, dtInput, ConnectionConstants.SecMaster_Connection);

                        DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_GetSecurityLookupData '{0}', {1}", tableName, inputInfo.isSecIdToValues ? 1 : 0), ConnectionConstants.RefMaster_Connection);

                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                int sectypeId = Convert.ToInt32(dr["sectype_id"]);
                                int attributeId = Convert.ToInt32(dr["attribute_id"]);
                                if (!string.IsNullOrEmpty(Convert.ToString(dr["fully_qualified_table_name"])) && !string.IsNullOrEmpty(Convert.ToString(dr["attribute_name"])))
                                {
                                    rowId = Convert.ToInt32(dr["row_id"]);

                                    if (ds.Tables.Count > rowId && ds.Tables[rowId].Rows.Count > 0)
                                    {
                                        result.Add(new SecurityLookupOutputInfo
                                        {
                                            SecurityTypeID = sectypeId,
                                            SecurityAttributeID = attributeId,
                                            ValuesToSearch = ds.Tables[rowId].AsEnumerable().Select(x => new SecurityLookupAttributeInfo { SecurityID = Convert.ToString(x["sec_id"]), AttributeValue = (inputInfo.isSecIdToValues ? x["value"] : x["original_value"]) }).ToList()
                                        });
                                    }
                                    else
                                        AddBlankResultForLookupData(ref result, sectypeId, attributeId);
                                }
                                else
                                    AddBlankResultForLookupData(ref result, sectypeId, attributeId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tableName))
                        CommonDALWrapper.ExecuteSelectQuery(string.Format(@"DROP TABLE {0}", tableName), ConnectionConstants.SecMaster_Connection);
                }
                catch { }
            }

            return result;
        }
        private static void AddBlankResultForLookupData(ref List<SecurityLookupOutputInfo> result, int sectypeId, int attributeId)
        {
            result.Add(new SecurityLookupOutputInfo
            {
                SecurityTypeID = sectypeId,
                SecurityAttributeID = attributeId
            });
        }

        private static bool ValidateDataType(string displayName, string datatype, string attrValue, string dateTimeFormat, string dateFormat, string timeFormat, string timePartFormat, out object val, out string errMessage)
        {
            try
            {
                attrValue = attrValue.Trim();
                errMessage = "";
                val = "";

                switch (datatype.ToUpper())
                {
                    case "STRING":
                    case "FILE":
                    case "TEXT":
                        val = attrValue;
                        return true;
                        break;

                    case "NUMERIC":
                        decimal temp;

                        //if (!attrValue.Equals(string.Empty))
                        {
                            if (decimal.TryParse(attrValue, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign, null, out temp))
                            {
                                val = temp.ToString();
                                return true;
                            }
                            else
                            {
                                errMessage = displayName + " (" + attrValue + ") is not a valid NUMERIC value.";
                                return false;
                            }
                        }
                        break;

                    case "BOOLEAN":
                        string value = string.Empty;

                        string boolString = attrValue.ToLower();

                        //if (!boolString.Equals(string.Empty))
                        {
                            if (lstBooleanTrue.Contains(boolString))
                                value = "True";
                            else if (lstBooleanFalse.Contains(boolString))
                                value = "False";
                            else
                            {
                                errMessage = displayName + " (" + attrValue + ") is not a valid BOOLEAN value";
                                return false;
                            }

                            val = value;
                            return true;
                        }

                        break;

                    case "DATE":
                        DateTime dd;

                        if (!string.IsNullOrEmpty(dateFormat))
                        {
                            if (DateTime.TryParseExact(attrValue, dateFormat, null, System.Globalization.DateTimeStyles.None, out dd))
                            {
                                val = dd.Date;
                                return true;
                            }
                            else if (DateTime.TryParseExact(attrValue.Split(' ')[0].ToString(), dateFormat, null, System.Globalization.DateTimeStyles.None, out dd))
                            {
                                val = dd.Date;
                                return true;
                            }
                            else
                            {
                                errMessage = displayName + " (" + attrValue + ") is not a valid DATE (" + dateFormat + ")";
                                return false;
                            }
                        }
                        else
                        {
                            if (DateTime.TryParse(attrValue, null, System.Globalization.DateTimeStyles.None, out dd))
                            {
                                val = dd.Date;
                                return true;
                            }
                            else
                            {
                                errMessage = displayName + " (" + attrValue + ") is not a valid DATE (" + dateFormat + ")";
                                return false;
                            }
                        }

                        break;

                    case "DATETIME":
                        DateTime dt;

                        if (!string.IsNullOrEmpty(dateTimeFormat))
                        {
                            if (DateTime.TryParseExact(attrValue, dateTimeFormat, null, System.Globalization.DateTimeStyles.None, out dt))
                            {
                                val = dt;
                                return true;
                            }
                            else if (DateTime.TryParseExact(attrValue + " " + DateTime.Today.ToString(timePartFormat), dateTimeFormat, null, System.Globalization.DateTimeStyles.None, out dt))
                            {
                                val = dt;
                                return true;
                            }
                            else
                            {
                                errMessage = displayName + " (" + attrValue + ") is not a valid DATETIME (" + dateTimeFormat + ")";
                                return false;
                            }
                        }
                        else
                        {
                            if (DateTime.TryParse(attrValue, null, System.Globalization.DateTimeStyles.None, out dt))
                            {
                                val = dt;
                                return true;
                            }
                            else
                            {
                                errMessage = displayName + " (" + attrValue + ") is not a valid DATETIME (" + dateTimeFormat + ")";
                                return false;
                            }
                        }

                        break;

                    case "TIME":
                        TimeSpan tt;
                        string timeValue = string.Empty;

                        if (!string.IsNullOrEmpty(timeValue))
                        {
                            if (TimeSpan.TryParseExact(attrValue, timeFormat, null, out tt))
                            {
                                val = tt.ToString(@"hh\:mm\:ss\.fffffff");
                                return true;
                            }
                            else
                            {
                                errMessage = displayName + " (" + attrValue + ") is not a valid TIME (" + timeFormat + ")";
                                return false;
                            }
                        }
                        else
                        {
                            if (TimeSpan.TryParse(attrValue, null, out tt))
                            {
                                val = tt.ToString(@"hh\:mm\:ss\.fffffff");
                                return true;
                            }
                            else
                            {
                                errMessage = displayName + " (" + attrValue + ") is not a valid TIME (" + timeFormat + ")";
                                return false;
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                val = "";
                errMessage = ex.ToString();
            }

            return false;
        }

        public static ObjectSet ConvertDataSetToObjectSet(DataSet ds)
        {
            mLogger.Debug("SRMCommon -> ConvertDataSetToObjectSet -> Start");
            try
            {
                ObjectSet objSet = new ObjectSet();

                foreach (DataTable dt in ds.Tables)
                {
                    ObjectTable objTable = ConvertDataTableToObjectTable(dt);

                    objSet.Tables.Add(objTable);
                }

                return objSet;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommon -> ConvertDataSetToObjectSet -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommon -> ConvertDataSetToObjectSet -> End");
            }
        }

        public static ObjectTable ConvertDataTableToObjectTable(DataTable dt)
        {
            mLogger.Debug("SRMCommon -> ConvertDataTableToObjectTable -> Start");
            try
            {
                ObjectTable objTable = new ObjectTable();

                //Copy Table Name
                objTable.TableName = dt.TableName;

                //Copy Schema
                foreach (DataColumn dc in dt.Columns)
                {
                    ObjectColumn objColumn = new ObjectColumn(dc.ColumnName, dc.DataType);
                    objTable.Columns.Add(objColumn);
                }

                //Copy Data
                foreach (DataRow dr in dt.Rows)
                {
                    ObjectRow objRow = objTable.NewRow();

                    foreach (DataColumn dc in dt.Columns)
                    {
                        objRow[dc.ColumnName] = dr[dc.ColumnName];
                    }

                    objTable.Rows.Add(objRow);
                }
                return objTable;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommon -> ConvertDataTableToObjectTable -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommon -> ConvertDataTableToObjectTable -> End");
            }
        }

        public static DataSet ConvertObjectSetToDataSet(ObjectSet objSet)
        {
            mLogger.Debug("SRMCommon -> ConvertObjectSetToDataSet -> Start");
            try
            {
                DataSet ds = new DataSet();

                if (objSet != null)
                {
                    foreach (ObjectTable objTable in objSet.Tables)
                    {
                        DataTable dt = objTable.ConvertToDataTable();
                        dt.TableName = objTable.TableName;
                        ds.Tables.Add(dt);
                    }
                }

                return ds;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommon -> ConvertObjectSetToDataSet -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommon -> ConvertObjectSetToDataSet -> End");
            }
        }

        public static DataTable ConvertObjectTableToDataTable(ObjectTable objTable)
        {
            mLogger.Debug("SRMCommon -> ConvertObjectTableToDataTable -> Start");
            try
            {
                DataTable dt = objTable.ConvertToDataTable();
                dt.TableName = objTable.TableName;

                return dt;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommon -> ConvertObjectTableToDataTable -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommon -> ConvertObjectTableToDataTable -> End");
            }
        }

        public static void deleteRule(int ruleId, string userName, string connectionId, RDBConnectionManager conn, out bool isSuccess, out string errorMessage)
        {
            isSuccess = true;
            errorMessage = string.Empty;
            try
            {
                RXRuleController objRXRuleController = new RXRuleController() { DBConnectionId = RADConfigReader.GetConfigAppSettings(connectionId) };

                if (conn == null)
                {
                    objRXRuleController.DeleteRule(ruleId, userName);
                    CommonDALWrapper.ExecuteSelectQuery("Delete  from [IVPSecMasterVendor].[dbo].[ivp_secmv_rule_mapping_details] where rule_id=" + ruleId, ConnectionConstants.SecMaster_Connection);
                }
                else
                {
                    objRXRuleController.DeleteRule(ruleId, userName, conn);
                    CommonDALWrapper.ExecuteSelectQuery("Delete  from [IVPSecMasterVendor].[dbo].[ivp_secmv_rule_mapping_details] where rule_id=" + ruleId, conn);
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                errorMessage = ex.Message;
            }
        }

        public static void deleteRuleFromSM(int ruleSetId, string connectionId, RDBConnectionManager conn)
        {

            try
            {
                RXRuleController objRXRuleController = new RXRuleController() { DBConnectionId = RADConfigReader.GetConfigAppSettings(connectionId) };

                if (conn == null)
                {
                    CommonDALWrapper.ExecuteSelectQuery("Delete  from [IVPSecMaster].[dbo].[ivp_secm_sectype_rule_mapping] where ruleset_id=" + ruleSetId, ConnectionConstants.SecMaster_Connection);

                }
                else
                {
                    CommonDALWrapper.ExecuteSelectQuery("Delete  from [IVPSecMaster].[dbo].[ivp_secm_sectype_rule_mapping] where ruleset_id=" + ruleSetId, conn);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int saveRule(RXRuleType ruleType, string ruleName, int priority, string ruleText, bool ruleState, int ruleId, int ruleSetId, string userName, RADXRuleGrammarInfo grammarInfo, string connectionId, RDBConnectionManager conn, out bool isSuccess, out string errorMessage)
        {
            isSuccess = true;
            errorMessage = string.Empty;
            try
            {
                RXRuleController objRXRuleController = new RXRuleController() { DBConnectionId = RADConfigReader.GetConfigAppSettings(connectionId) };

                RADXRuleHelperClass objRADXRuleHelperClass = new RADXRuleHelperClass();
                Parser parser = (Parser)objRADXRuleHelperClass.GetRuleParser(grammarInfo);
                RXRuleEditorResponse response = objRADXRuleHelperClass.GetRuleClass(ruleText, parser);

                RXRuleInfo mObjRuleInfo = new RXRuleInfo();

                mObjRuleInfo.RuleSetID = ruleSetId;
                mObjRuleInfo.RuleType = ruleType;
                mObjRuleInfo.RuleName = ruleName;
                mObjRuleInfo.RulePriority = priority;
                mObjRuleInfo.RuleText = ruleText;
                mObjRuleInfo.CreatedBy = userName;
                mObjRuleInfo.ClassText = response.ClassText;
                mObjRuleInfo.ClassDetails = response.ClassDocument;
                mObjRuleInfo.CreatedOn = DateTime.Now;
                mObjRuleInfo.RuleState = ruleState;
                if (ruleId > 0)
                {
                    mObjRuleInfo.RuleID = ruleId;
                    mObjRuleInfo.DBOperationType = RXOperationType.Update;
                }
                else
                {
                    mObjRuleInfo.RuleID = 0;
                    mObjRuleInfo.DBOperationType = RXOperationType.Insert;
                }
                mObjRuleInfo.IsActive = true;
                mObjRuleInfo.RuleExecutionMode = RXRuleExecutionMode.Priority;

                if (conn == null)
                    return objRXRuleController.SaveRule(mObjRuleInfo);
                else
                    return objRXRuleController.SaveRule(mObjRuleInfo, conn);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                errorMessage = ex.Message;
            }

            return 0;
        }

        public static string DecryptThroughCertificate(byte[] data, string certificateName)
        {
            byte[] ret = new byte[0];
            if (data != null)
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly | OpenFlags.IncludeArchived);
                X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, false);
                store.Close();

                if (col != null && col.Count == 1)
                {
                    var cert = col.Cast<X509Certificate2>().First();
                    using (var rsa = (RSACryptoServiceProvider)cert.PrivateKey)
                    {
                        ret = rsa.Decrypt(data, false);
                    }
                }
            }
            return Encoding.UTF8.GetString(ret);
        }

        public static byte[] EncryptThroughCertificate(string data, string certificateName)
        {
            byte[] ret = new byte[0];
            if (data != null)
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly | OpenFlags.IncludeArchived);
                X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, false);
                store.Close();

                if (col != null && col.Count == 1)
                {
                    var cert = col.Cast<X509Certificate2>().First();

                    var publicKeyParameters = RSACertificateExtensions.GetRSAPublicKey(cert).ExportParameters(false);

                    var datatoEncrypt = Encoding.UTF8.GetBytes(data);

                    using (var rsa = new RSACryptoServiceProvider())
                    {
                        rsa.ImportParameters(publicKeyParameters);
                        ret = rsa.Encrypt(datatoEncrypt, false);
                    }
                }
            }
            return ret;
        }

        public static void UpdateLastUseTime(int apiRequestId, bool enforceAuthentication, bool isTCP)
        {
            try
            {
                string sessionId, deviceKey;
                if (enforceAuthentication)
                {
                    sessionId = GetHeaderValue("SessionId", true, isTCP);
                    deviceKey = GetHeaderValue("DeviceKey", true, isTCP);

                    string certificateName = SRMCommon.GetConfigFromDB("CertificateName");
                    if (!string.IsNullOrEmpty(certificateName))
                    {
                        sessionId = DecryptThroughCertificate(Convert.FromBase64String(sessionId), certificateName);
                    }
                }
                else
                {
                    sessionId = GetHeaderValue("SessionId", false, isTCP);
                    deviceKey = GetHeaderValue("DeviceKey", false, isTCP);
                }

                if (!string.IsNullOrEmpty(sessionId) && !string.IsNullOrEmpty(deviceKey))
                {
                    RServiceAuthorizationManager rsAuthManager = new RServiceAuthorizationManager();

                    try
                    {
                        InsertAPILogs(apiRequestId, "Start -> RServiceAuthentication.UpdateLastUseTime");
                        rsAuthManager.UpdateLastUseTime(sessionId, deviceKey);
                        InsertAPILogs(apiRequestId, "End -> RServiceAuthentication.UpdateLastUseTime");
                    }
                    catch (Exception ex)
                    {
                        InsertAPILogs(apiRequestId, "Exception -> RServiceAuthentication.UpdateLastUseTime : " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                InsertAPILogs(apiRequestId, "Exception -> RServiceAuthentication.UpdateLastUseTime : " + ex.Message);
            }
        }

        public static void InsertAPILogs(int apiRequestId, string logMessage, RDBConnectionManager mDbConn = null)
        {
            if (apiRequestId == -1)
            {
                mLogger.Debug(logMessage);
                return;
            }

            try
            {
                if (mDbConn != null)
                    CommonDALWrapper.ExecuteSelectQuery("exec [IVPRefMaster].[dbo].[SRM_InsertAPIRequestLog] " + apiRequestId + ",'" + (string.IsNullOrEmpty(logMessage) ? "" : logMessage.Replace("'", "''")) + "'", mDbConn);
                else
                    CommonDALWrapper.ExecuteSelectQuery("exec [IVPRefMaster].[dbo].[SRM_InsertAPIRequestLog] " + apiRequestId + ",'" + (string.IsNullOrEmpty(logMessage) ? "" : logMessage.Replace("'", "''")) + "'", ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex);
            }
        }

        public static string GetHeaderValue(string headerName, bool throwException, bool isTCP)
        {
            string headerValue = string.Empty;
            try
            {
                bool headerFound = false;
                if (isTCP)
                {
                    foreach (var header in OperationContext.Current.IncomingMessageHeaders)
                    {
                        if (header.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase) && header.Namespace.Equals("IVP", StringComparison.OrdinalIgnoreCase))
                        {
                            headerValue = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(headerName, "IVP");
                            headerFound = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.Headers.AllKeys.Contains(headerName, StringComparer.OrdinalIgnoreCase))
                    {
                        headerValue = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.Headers[headerName];
                        headerFound = true;
                    }
                    else
                    {
                        foreach (var header in OperationContext.Current.IncomingMessageHeaders)
                        {
                            if (header.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase) && header.Namespace.Equals("IVP", StringComparison.OrdinalIgnoreCase))
                            {
                                headerValue = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(headerName, "IVP");
                                headerFound = true;
                                break;
                            }
                        }
                    }
                }

                if (throwException)
                {
                    if (!headerFound)
                        throw new Exception(headerName + " header is not available in request. You are not authorized to use.");

                    if (string.IsNullOrEmpty(headerValue))
                        throw new Exception(headerName + " header is empty. You are not authorized to use.");
                }
            }
            catch (Exception ee)
            {
                if (throwException)
                    throw;
            }
            return headerValue;
        }

        public static void SetErrorMessageInResponseHeader(string message, bool isTCP)
        {
            if (isTCP)
            {
                MessageHeader<string> header = new MessageHeader<string>(message);
                MessageHeader untypedHeader = header.GetUntypedHeader("ErrorDescription", "IVP");
                OperationContext.Current.OutgoingMessageHeaders.Add(untypedHeader);
            }

            OutgoingWebResponseContext ctx = WebOperationContext.Current.OutgoingResponse;
            ctx.StatusCode = System.Net.HttpStatusCode.Unauthorized;
            ctx.StatusDescription = message;
            ctx.Headers.Add("ErrorDescription", message);
        }

        static RRSAEncrDecr encDec = new RRSAEncrDecr();
        public static string FetchDataToken(string dataToken, int apiRequestId, ref string tokenKey, ref string creationTicks)
        {
            string UserEmail = string.Empty;

            try
            {
                string secret = SRMCommon.GetConfigFromDB("JWTSecretKey");
                if (!string.IsNullOrEmpty(secret))
                {
                    string tempPass = secret;
                    try
                    {
                        secret = encDec.DoDecrypt(secret);
                    }
                    catch (Exception ex)
                    {
                        secret = tempPass;
                        mLogger.Error("FetchDataToken => " + ex.ToString());
                    }
                }

                IJsonSerializer serializer = new JsonNetSerializer();
                var provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                string jsonDecoded;
                if (string.IsNullOrEmpty(secret))
                    jsonDecoded = decoder.Decode(dataToken);
                else
                    jsonDecoded = decoder.Decode(dataToken, secret, verify: true);

                SRMCommon.InsertAPILogs(apiRequestId, "Token decoded successfully");

                dynamic json = JsonConvert.DeserializeObject(jsonDecoded);
                UserEmail = Convert.ToString(json.email);
                tokenKey = Convert.ToString(json.nonce);
                creationTicks = Convert.ToString(json.createdAt);
            }
            catch (TokenExpiredException)
            {
                mLogger.Error("Exception while parsing token : Token has expired");
                throw new Exception("Token has expired. Please verify the token");
            }
            catch (SignatureVerificationException)
            {
                mLogger.Error("Exception while parsing token : Token has invalid signature");
                throw new Exception("Token has invalid signature. Please verify the token");
            }
            catch (Exception ex)
            {
                mLogger.Error("Exception while parsing token : " + ex.ToString());
                throw new Exception("Unable to parse token. Please verify the token");
            }

            if (string.IsNullOrEmpty(UserEmail))
                throw new Exception("UserEmail parsed from token is blank");
            if (string.IsNullOrEmpty(tokenKey))
                throw new Exception("tokenKey parsed from token is blank");

            return UserEmail;
        }
        public static ExceptionsConfig getExceptionConfigDetails(int moduleId, int instrumentTypeId)
        {
            string query = "SELECT * FROM [IVPRefMaster].[dbo].[ivp_srm_exception_config] where module_id = " + moduleId + " and instrument_type_id =" + instrumentTypeId;
            DataTable dt = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection).Tables[0];
            if (dt.Rows.Count == 0)
                return null;
            ExceptionsConfig result = new ExceptionsConfig();
            result.moduleId = moduleId;
            result.instrumentTypeId = instrumentTypeId;

            DataRow row = dt.Rows[0];
            result.Alert = Boolean.Parse(row["alerts"].ToString());
            result.Duplicate = Boolean.Parse(row["duplicates"].ToString());
            result.FirstVendorValueMissing = Boolean.Parse(row["first_vendor_value_missing"].ToString());
            result.InvalidData = Boolean.Parse(row["invalid_data"].ToString());
            result.NoVendorValue = Boolean.Parse(row["no_vendor_value"].ToString());
            result.RefDataMissing = Boolean.Parse(row["ref_data_missing"].ToString());
            result.ShowAsException = Boolean.Parse(row["show_as_exception"].ToString());
            result.Validation = Boolean.Parse(row["validations"].ToString());
            result.ValueChange = Boolean.Parse(row["value_change"].ToString());
            result.VendorMismatch = Boolean.Parse(row["vendor_mismatch"].ToString());
            result.UnderlierMissing = Boolean.Parse(row["underlier_missing"].ToString());
            return result;
        }

        public static string saveExceptionConfigDetails(List<ExceptionsConfig> configs, string user)
        {

            if (configs == null || configs.Count == 0)
                return "No Exception Config sent";
            int moduleId;
            int instrumentTypeId;
            int insertCount = 0;
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            ExceptionsConfig config;
            string insert = "INSERT INTO [IVPRefMaster].[dbo].[ivp_srm_exception_config] ([module_id],[instrument_type_id],[created_by],[created_on],[last_modified_by],[last_modified_on],[is_active],[alerts],[duplicates],[first_vendor_value_missing],[invalid_data],[no_vendor_value],[ref_data_missing],[show_as_exception],[underlier_missing],[validations],[value_change],[vendor_mismatch])" +
                            "  VALUES";
            try
            {
                for (int i = 0; i < configs.Count; i++)
                {
                    config = configs[i];
                    moduleId = config.moduleId;
                    instrumentTypeId = config.instrumentTypeId;
                    string query = "SELECT 1 FROM[IVPRefMaster].[dbo].[ivp_srm_exception_config] where module_id = " + moduleId + " and instrument_type_id =" + instrumentTypeId;
                    DataTable dt = CommonDALWrapper.ExecuteSelectQuery(query, con).Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        insert = insert + "(" + moduleId +
                            "," + instrumentTypeId +
                            ",'" + user +
                            "',GETDATE() " +
                            ",NULL,NULL,1" +
                            ",'" + config.Alert +
                            "','" + config.Duplicate +
                            "','" + config.FirstVendorValueMissing +
                            "','" + config.InvalidData +
                            "','" + config.NoVendorValue +
                            "','" + config.RefDataMissing +
                            "','" + config.ShowAsException +
                            "','" + config.UnderlierMissing +
                            "','" + config.Validation +
                            "','" + config.ValueChange +
                            "','" + config.VendorMismatch +
                            "'),";
                        insertCount++;
                        CommonDALWrapper.ExecuteSelectQuery(query, con);
                    }
                    else
                    {
                        query = "UPDATE [IVPRefMaster].[dbo].[ivp_srm_exception_config] SET " +
                            "[last_modified_by] = '" + user + "'," +
                            "[last_modified_on]= GetDate()," +
                            "[alerts] ='" + config.Alert + "'," +
                            "[duplicates] ='" + config.Duplicate + "'," +
                            "[first_vendor_value_missing] ='" + config.FirstVendorValueMissing + "'," +
                            "[invalid_data] ='" + config.InvalidData + "'," +
                            "[no_vendor_value] ='" + config.NoVendorValue + "'," +
                            "[ref_data_missing] ='" + config.RefDataMissing + "'," +
                            "[show_as_exception] ='" + config.ShowAsException + "'," +
                            "[underlier_missing] ='" + config.UnderlierMissing + "'," +
                            "[validations] ='" + config.Validation + "'," +
                            "[value_change] ='" + config.ValueChange + "'," +
                            "[vendor_mismatch] ='" + config.VendorMismatch + "'" +
                            "where [module_id]=" + moduleId + " AND [instrument_type_id]=" + instrumentTypeId;
                        CommonDALWrapper.ExecuteSelectQuery(query, con);
                    }
                }
                if (insertCount != 0)
                {

                    insert = insert.Remove(insert.Length - 1);
                    CommonDALWrapper.ExecuteQuery(insert, CommonQueryType.Insert, con);
                }
                con.CommitTransaction();
            }
            catch (Exception e)
            {
                con.RollbackTransaction();
                throw e;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }
            return "Success";
        }

        public static string InvokeRestAPI(string url, Dictionary<string, string> headers, byte[] requestBodyBytes, string methodName, string methodType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "//" + methodName);
            request.Method = methodType.ToUpper();
            request.ContentType = "application/json";
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            if (requestBodyBytes.Length > 0)
            {
                Stream postStream = request.GetRequestStream();
                postStream.Write(requestBodyBytes, 0, requestBodyBytes.Length);
                postStream.Flush();
                postStream.Close();
            }
            string response = string.Empty;

            WebResponse webResponse = request.GetResponse();
            Stream webStream = webResponse.GetResponseStream();
            StreamReader responseReader = new StreamReader(webStream);
            response = responseReader.ReadToEnd();
            responseReader.Close();

            return response;
        }

        public static void ReduceAndTransformData(ref ObjectTable dtData, List<string> columnsForPrimary, List<string> columnsToSkip, string dateColumnName)
        {
            if (!dtData.Columns.Contains("End Date"))
                dtData.Columns.Add(new ObjectColumn("End Date", typeof(System.DateTime)) { DefaultValue = DBNull.Value });

            if (dtData.Rows.Count > 0)
            {
                const string separator = "¦";
                List<string> columnsForHash = new List<string>();

                foreach (ObjectColumn dc in dtData.Columns)
                    if (!columnsToSkip.Contains(dc.ColumnName, StringComparer.OrdinalIgnoreCase))
                        columnsForHash.Add(dc.ColumnName);

                var orderedRows = dtData.AsEnumerable().OrderBy(dr =>
                {
                    string primaryKey = string.Empty;
                    StringBuilder primKeySB = new StringBuilder();
                    foreach (var colName in columnsForPrimary)
                    {
                        primKeySB.Append(Convert.ToString(dr[colName])).Append(separator);
                    }
                    primaryKey = primKeySB.ToString();

                    return primaryKey;
                }).ThenBy(x => Convert.ToDateTime(x[dateColumnName]));

                string previousPrimaryKey = string.Empty, previousHash = string.Empty;
                DateTime previousDate = DateTime.Now;
                ObjectRow previousRow = dtData.Rows[0];
                List<ObjectRow> rowsToDelete = new List<ObjectRow>();
                foreach (ObjectRow dr in orderedRows)
                {
                    string primaryKey = string.Empty;
                    StringBuilder primKeySB = new StringBuilder();
                    foreach (var colName in columnsForPrimary)
                    {
                        primKeySB.Append(Convert.ToString(dr[colName])).Append(separator);
                    }
                    primaryKey = primKeySB.ToString();

                    string hash = string.Empty;
                    StringBuilder hashSB = new StringBuilder();
                    foreach (var colName in columnsForHash)
                    {
                        hashSB.Append(Convert.ToString(dr[colName])).Append(separator);
                    }
                    hash = hashSB.ToString();

                    DateTime date = Convert.ToDateTime(dr[dateColumnName]).Date;
                    if (previousPrimaryKey.Equals(primaryKey, StringComparison.OrdinalIgnoreCase) && previousHash.Equals(hash, StringComparison.OrdinalIgnoreCase) && previousDate.AddDays(1).Equals(date))
                        rowsToDelete.Add(dr);
                    else
                    {
                        previousPrimaryKey = primaryKey;
                        previousHash = hash;
                        previousRow = dr;
                    }
                    if (!date.Equals(DateTime.Today))
                        previousRow["End Date"] = date;
                    else
                        previousRow["End Date"] = DBNull.Value;
                    previousDate = date;
                }
                if (rowsToDelete.Count > 0)
                {
                    foreach (var row in rowsToDelete)
                    {
                        row.DeleteDeferred();
                        //dtData.Rows.Remove(row);
                    }
                    dtData.CommitDelete();
                }
            }

            dtData.Columns[dateColumnName].ColumnName = "Start Date";
        }

        public static void ReduceAndTransformData(ref DataTable dtData, List<string> columnsForPrimary, List<string> columnsToSkip, string dateColumnName)
        {
            if (!dtData.Columns.Contains("End Date"))
                dtData.Columns.Add(new DataColumn("End Date", typeof(System.DateTime)) { DefaultValue = DBNull.Value });

            if (dtData.Rows.Count > 0)
            {
                const string separator = "¦";
                List<string> columnsForHash = new List<string>();

                foreach (DataColumn dc in dtData.Columns)
                    if (!columnsToSkip.Contains(dc.ColumnName, StringComparer.OrdinalIgnoreCase))
                        columnsForHash.Add(dc.ColumnName);

                var orderedRows = dtData.AsEnumerable().OrderBy(dr =>
                {
                    string primaryKey = string.Empty;
                    StringBuilder primKeySB = new StringBuilder();
                    foreach (var colName in columnsForPrimary)
                    {
                        primKeySB.Append(Convert.ToString(dr[colName])).Append(separator);
                    }
                    primaryKey = primKeySB.ToString();

                    return primaryKey;
                }).ThenBy(x => Convert.ToDateTime(x[dateColumnName]));

                string previousPrimaryKey = string.Empty, previousHash = string.Empty;
                DateTime previousDate = DateTime.Now;
                DataRow previousRow = dtData.Rows[0];
                List<DataRow> rowsToDelete = new List<DataRow>();
                foreach (DataRow dr in orderedRows)
                {
                    string primaryKey = string.Empty;
                    StringBuilder primKeySB = new StringBuilder();
                    foreach (var colName in columnsForPrimary)
                    {
                        primKeySB.Append(Convert.ToString(dr[colName])).Append(separator);
                    }
                    primaryKey = primKeySB.ToString();

                    string hash = string.Empty;
                    StringBuilder hashSB = new StringBuilder();
                    foreach (var colName in columnsForHash)
                    {
                        hashSB.Append(Convert.ToString(dr[colName])).Append(separator);
                    }
                    hash = hashSB.ToString();

                    DateTime date = Convert.ToDateTime(dr[dateColumnName]).Date;
                    if (previousPrimaryKey.Equals(primaryKey, StringComparison.OrdinalIgnoreCase) && previousHash.Equals(hash, StringComparison.OrdinalIgnoreCase) && previousDate.AddDays(1).Equals(date))
                        rowsToDelete.Add(dr);
                    else
                    {
                        previousPrimaryKey = primaryKey;
                        previousHash = hash;
                        previousRow = dr;
                    }
                    if (!date.Equals(DateTime.Today))
                        previousRow["End Date"] = date;
                    else
                        previousRow["End Date"] = DBNull.Value;
                    previousDate = date;
                }
                if (rowsToDelete.Count > 0)
                {
                    foreach (var row in rowsToDelete)
                    {
                        dtData.Rows.Remove(row);
                    }
                }
            }

            dtData.Columns[dateColumnName].ColumnName = "Start Date";
        }

        public static string RestAPICaller(string serviceURL, SRMRestMethod type, string data, string authUserName, string authPassword)
        {
            string output;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(serviceURL);
                req.ServicePoint.Expect100Continue = false;
                req.KeepAlive = true;
                req.ContentType = "application/json";

                if (!string.IsNullOrEmpty(authUserName) && !string.IsNullOrEmpty(authPassword))
                {
                    req.PreAuthenticate = true;
                    req.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(authUserName + ":" + authPassword)));
                }

                req.Method = type.ToString();

                //req.ContentLength = 5;
                if (type == SRMRestMethod.POST)
                {
                    StreamWriter requestStream = new StreamWriter(req.GetRequestStream());

                    requestStream.Write(data);
                    requestStream.Flush();
                    requestStream.Close();
                }

                using (HttpWebResponse responseStream = (HttpWebResponse)req.GetResponse())
                {
                    StreamReader sr = new StreamReader(responseStream.GetResponseStream());
                    output = sr.ReadToEnd();
                    responseStream.Close();
                }

            }
            //catch (WebException we)
            //{
            //    mLogger.Error(we.ToString());
            //    output = we.Message;
            //    try
            //    {
            //        using (HttpWebResponse response = (HttpWebResponse)we.Response)
            //        {
            //            using (Stream responseStream = response.GetResponseStream())
            //            {
            //                using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
            //                {
            //                    output = readStream.ReadToEnd();
            //                }
            //            }
            //        }
            //    }
            //    catch { }

            //    throw;
            //}
            catch (Exception e)
            {
                mLogger.Error("RestAPICaller method failed for uri - " + serviceURL + " with error:" + Environment.NewLine + e.ToString());
                //output = e.Message;

                throw;
            }
            return output;
        }


        public static int TriggerProcess(string exeName, string processName, string workingDirectory, Action ExitCallBack)
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo(exeName);

                processInfo.CreateNoWindow = false;
                processInfo.UseShellExecute = false;
                processInfo.WorkingDirectory = workingDirectory;
                processInfo.RedirectStandardError = false;
                processInfo.RedirectStandardInput = false;
                processInfo.RedirectStandardOutput = false;
                Process process = Process.Start(processInfo);
                process.EnableRaisingEvents = true;
                process.Exited += delegate (object sender, EventArgs e)
                {
                    try
                    {
                        process_Exited(sender, e);
                        if (ExitCallBack != null)
                            ExitCallBack();
                    }
                    catch { }
                };

                return process.Id;
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("TriggerProcessError_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt", string.Format("Process Name : {0} -> Exception Message: {1}", processName, ex));
            }
            return -1;
        }

        public static void process_Exited(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(((Process)sender).StandardError.ReadToEnd().Trim()))
                {
                    errorMessage = ((Process)sender).StandardError.ReadToEnd().Trim() + "      ";
                }
                if (!string.IsNullOrEmpty(((Process)sender).StandardOutput.ReadToEnd().Trim()))
                {
                    errorMessage += ((Process)sender).StandardOutput.ReadToEnd().Trim();
                }

            }
            catch (Exception ee)
            {
                errorMessage = ee.ToString();
            }
            finally
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    System.IO.File.AppendAllText("ProcessExitedError_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt", "Exception Message: " + errorMessage);
            }
        }

        public static string GetConfigFromDB(string key)
        {
            string value = string.Empty;
            string name = string.Empty;
            if (SRMMTConfig.isMultiTenantEnabled)
            {
                string clientName = SRMMTConfig.GetClientName();

                if (!dctClientVsConfigEntries.ContainsKey(clientName))
                {
                    dctClientVsConfigEntries.TryAdd(clientName, new ConcurrentDictionary<string, string>());

                    ObjectSet os = CommonDALWrapper.ExecuteSelectQueryObject("SELECT * from dbo.ivp_srm_app_settings", ConnectionConstants.RefMaster_Connection);
                    if (os != null && os.Tables.Count > 0 && os.Tables[0].Rows.Count > 0)
                    {
                        foreach (ObjectRow or in os.Tables[0].Rows)
                        {
                            name = Convert.ToString(or["name"]);
                            value = Convert.ToString(or["value"]);

                            dctClientVsConfigEntries[clientName][name] = value;
                        }
                    }
                }
                if (dctClientVsConfigEntries[clientName].ContainsKey(name))
                    value = dctClientVsConfigEntries[clientName][name];
                else
                    value = string.Empty;
            }
            else
                value = RConfigReader.GetConfigAppSettings(key);
            return value;
        }

        public static void SetReportsInMenu(ref XElement menu, List<string> lstReportPath, Dictionary<string, string> nameVsReportName, int moduleId)
        {
            var logMenuString = RConfigReader.GetConfigAppSettings("LogMenu");
            if (!string.IsNullOrWhiteSpace(logMenuString) && Boolean.TryParse(logMenuString, out bool logMenu))
            {
                mLogger.Debug("Menu -> " + menu.ToString());
                mLogger.Debug("lstReportPath -> " + string.Join(",", lstReportPath));
                mLogger.Debug("nameVsReportName -> " + string.Join(Environment.NewLine, nameVsReportName.Select(x => $"Name : {x.Key}, ReportName : {x.Value}")));
            }

            List<XElement> mainSections = menu.Elements("section").ToList();
            foreach (string path in lstReportPath)
            {
                string[] pathArr = path.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                int index = Convert.ToInt32(pathArr[pathArr.Length - 1]);
                string reportName = pathArr[pathArr.Length - 2];

                string[] pathArrNew = new string[pathArr.Length - 2];
                if (pathArr.Length > 2)
                {
                    Array.Copy(pathArr, pathArrNew, pathArr.Length - 2);
                    pathArr = pathArrNew;
                }
                else
                    pathArr = new string[0];

                if (pathArr.Length > 0)
                {
                    XElement prevNode = menu;
                    XElement temp = null;
                    for (int i = 0; i < pathArr.Length; i++)
                    {
                        var item = pathArr[i];
                        int intermediateNodeIndex = 0;

                        if (item.Contains("~~"))
                        {
                            var at = item.Split(new string[] { "~~" }, StringSplitOptions.None);
                            intermediateNodeIndex = Convert.ToInt32(at[at.Length - 1]);
                            item = at[0];
                        }

                        List<XElement> sections = null;

                        if (temp == null)
                            sections = mainSections;
                        else
                        {
                            prevNode = temp;
                            sections = temp.Elements("section").ToList();
                        }
                        temp = null;
                        foreach (var sec in sections)
                        {
                            XElement header = sec.Element("header");
                            if (header != null && header.Value == item)
                            {
                                temp = sec;
                                break;
                            }
                            else
                            {
                                XElement body = sec.Element("body");
                                if (body != null)
                                {
                                    bool isBroken = false;
                                    List<XElement> lxe = body.Descendants().ToList();
                                    if (lxe.Any())
                                    {
                                        XElement found = null;
                                        foreach (XElement xitem in lxe)
                                        {
                                            XAttribute xa = xitem.Attribute("name");
                                            if (xa != null && xa.Value == item)
                                            {
                                                isBroken = true;
                                                found = xitem;
                                                break;
                                            }
                                        }
                                        if (isBroken)
                                        {
                                            lxe.Remove(found);

                                            if (lxe.Count > 0)
                                                body.Add(new XElement("section",
                                                    new XElement("body", lxe)));
                                            else
                                            {
                                                var items = body.Elements("item");
                                                if (items != null && items.Count() > 0)
                                                {
                                                    foreach (var ite in items)
                                                        ite.Remove();
                                                }
                                            }
                                            temp = new XElement("section",
                                                new XElement("header", item), new XElement("body", found));

                                            if (header == null)
                                            {
                                                sec.AddFirst(new XElement("header", item));
                                                body.Add(found);
                                                temp = sec;
                                            }
                                            else
                                                body.Add(temp);
                                        }
                                    }
                                    if (isBroken)
                                        break;
                                }
                            }
                        }

                        //case : could not find node and it is the last intermediate node
                        if (temp == null && i == pathArr.Length - 1)
                        {
                            temp = new XElement("section",
                                new XElement("header", item));

                            XElement tt = new XElement("body",
                                new XElement("item",
                                 new XAttribute("name", reportName),
                                        new XAttribute("source", "replace#Common_GridWriter"),
                                        new XAttribute("sourcehref", "CommonUI/SRMGridWriter.aspx"),
                                        new XAttribute("ignoreprivilege", "true"),
                                new XElement("parameter",
                                new XAttribute("name", "ReportName"), nameVsReportName[reportName]),
                                new XElement("parameter",
                                new XAttribute("name", "ModuleId"), moduleId),
                                new XElement("parameter",
                                new XAttribute("name", "TabName"), reportName)));

                            temp.Add(tt);

                            if (prevNode == menu)
                            {
                                if (intermediateNodeIndex > mainSections.Count + 1)
                                    intermediateNodeIndex = mainSections.Count + 1;
                                mainSections.Insert(intermediateNodeIndex - 1, temp);
                            }
                        }
                        //case : could not find node and it is an intermediate node but not the last intermediate node
                        else if (temp == null && i < pathArr.Length - 1)
                        {
                            XElement body = prevNode.Element("body");
                            XElement secc = body.Element("section");
                            if (secc == null)
                            {
                                var bodyContent = new XElement("section", body);
                                body.RemoveNodes();
                                body.Add(bodyContent);
                            }
                            temp = new XElement("section", new XElement("header", item));
                            body.Add(temp);
                        }

                        //case : found parent node and it is the last intermediate node
                        else if (temp != null && i == pathArr.Length - 1)
                        {
                            XElement tt = new XElement("item",
                                new XAttribute("name", reportName),
                                        new XAttribute("source", "replace#Common_GridWriter"),
                                        new XAttribute("sourcehref", "CommonUI/SRMGridWriter.aspx"),
                                        new XAttribute("ignoreprivilege", "true"),
                                new XElement("parameter",
                                new XAttribute("name", "ReportName"), nameVsReportName[reportName]),
                                new XElement("parameter",
                                new XAttribute("name", "ModuleId"), moduleId),
                                new XElement("parameter",
                                new XAttribute("name", "TabName"), reportName));

                            XElement body = temp.Element("body");
                            if (body != null)
                            {
                                XElement secc = body.Element("section");
                                if (secc != null)
                                {
                                    int k = 1;
                                    foreach (var ele in body.Elements("section"))
                                    {
                                        if (index == 1)
                                        {
                                            ele.AddBeforeSelf(new XElement("section",
                                                new XElement("body", tt)));
                                            break;
                                        }
                                        else if (k == index - 1)//index is 1 based
                                        {
                                            ele.AddAfterSelf(new XElement("section",
                                                new XElement("body", tt)));
                                            break;
                                        }
                                        k++;
                                    }
                                }
                                else
                                {
                                    int k = 1;
                                    foreach (var ele in body.Elements("item"))
                                    {
                                        if (index == 1)
                                        {
                                            ele.AddBeforeSelf(tt);
                                            break;
                                        }
                                        else if (k == index - 1)//index is 1 based
                                        {
                                            ele.AddAfterSelf(tt);
                                            break;
                                        }
                                        k++;
                                    }
                                }
                            }
                            else
                                temp.Add(new XElement("body", tt));
                        }
                    }
                }
                else
                {
                    //case : No header.. Report in root node
                    XElement tt = new XElement("section",
                        new XElement("body",
                        new XElement("item",
                        new XAttribute("name", reportName),
                                new XAttribute("source", "replace#Common_GridWriter"),
                                new XAttribute("sourcehref", "CommonUI/SRMGridWriter.aspx"),
                                new XAttribute("ignoreprivilege", "true"),
                        new XElement("parameter",
                        new XAttribute("name", "ReportName"), nameVsReportName[reportName]),
                        new XElement("parameter",
                                new XAttribute("name", "ModuleId"), moduleId),
                        new XElement("parameter",
                        new XAttribute("name", "TabName"), reportName))));

                    menu.Add(tt);
                    if (index > mainSections.Count + 1)
                        index = mainSections.Count + 1;
                    mainSections.Insert(index - 1, tt);
                }
            }
            XElement mm = new XElement("root");
            foreach (var section in mainSections)
                mm.Add(section);

            menu = mm;
        }
        public static void WaitForScanForUploadedFile(string userName, string filePath, byte[] fileByteData, string fileExtension, out bool scanPassed)
        {
            scanPassed = true;
            string tempFolderLocation = SRMCommon.GetConfigFromDB("SRMFolderLocationForScanning");
            string waitTime = SRMCommon.GetConfigFromDB("SRMFileScanWaitTimeInMS");
            int timeInMS;
            if (!string.IsNullOrEmpty(tempFolderLocation) && !string.IsNullOrEmpty(waitTime) && Int32.TryParse(waitTime, out timeInMS) && timeInMS > 0)
            {
                try
                {
                    string tempFolderPath = Path.Combine(tempFolderLocation, userName);
                    if (!Directory.Exists(tempFolderPath))
                        Directory.CreateDirectory(tempFolderPath);

                    string fileName = "fileScan_" + Guid.NewGuid().ToString() + fileExtension;
                    string completeFilePath = Path.Combine(tempFolderPath, fileName);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        if (Path.HasExtension(filePath))
                        {
                            fileName = Path.GetFileName(filePath);
                            completeFilePath = Path.Combine(tempFolderPath, fileName);
                            try
                            {
                                File.Delete(completeFilePath);
                            }
                            catch { }
                        }
                        File.Move(filePath, completeFilePath);
                    }

                    if (fileByteData != null && fileByteData.Length > 0)
                    {
                        File.WriteAllBytes(completeFilePath, fileByteData);
                    }

                    Thread.Sleep(timeInMS);

                    if (File.Exists(completeFilePath))
                    {
                        if (!string.IsNullOrEmpty(filePath))
                            File.Move(completeFilePath, filePath);

                        if (fileByteData != null && fileByteData.Length > 0)
                        {
                            try
                            {
                                File.Delete(completeFilePath);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        scanPassed = false;
                        try
                        {
                            File.Delete(completeFilePath);
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error(ex);
                    throw;
                }
            }
        }
    }


    public enum SRMRestMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class SecurityLookupInputInfo
    {
        public bool isSecIdToValues { get; set; }
        public string TimeFormat { get; set; }
        public string DateFormat { get; set; }
        public string DateTimeFormat { get; set; }
        public List<SecurityLookupInfo> searchObjects { get; set; }
    }


    public class SecurityLookupInfo
    {
        public int SecurityTypeID { get; set; }
        public int SecurityAttributeID { get; set; }
        public List<object> ValuesToSearch { get; set; }
    }

    public class SecurityLookupOutputInfo
    {
        public SecurityLookupOutputInfo()
        {
            ValuesToSearch = new List<SecurityLookupAttributeInfo>();
        }
        public int SecurityTypeID { get; set; }
        public int SecurityAttributeID { get; set; }
        public List<SecurityLookupAttributeInfo> ValuesToSearch { get; set; }
    }

    public class SecurityLookupAttributeInfo
    {
        public string SecurityID { get; set; }
        public object AttributeValue { get; set; }
    }


    public class AttrInfo
    {
        public string AttrRealName { get; set; }
        public int AttrId { get; set; }
        public int Priority { get; set; }
        public int SubTypeId { get; set; }
        public int SectypeTableId { get; set; }
        public int OldSectypeTableId { get; set; }
        public string AttrType { get; set; }
        public string AttrDataType { get; set; }
        public int AttrReferenceType { get; set; }
        public string ReferenceAttr { get; set; }
        public string AttrDisplayName { get; set; }
        public string TableDisplayName { get; set; }
        public int SecTypeId { get; set; }
        public string SecTypeName { get; set; }
        public string TableName { get; set; }
        public bool ToShow { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsCommon { get; set; }
        public List<string> Tags { get; set; }
        public bool IsAdditionalLegAttribute { get; set; }
        public string DataLength { get; set; }
        public List<int> UnderlyingSectypeIds { get; set; }
        public string LegName { get; set; }

        public AttrInfo Clone()
        {
            return new AttrInfo
            {
                AttrRealName = this.AttrRealName,
                AttrId = this.AttrId,
                AttrType = this.AttrType,
                AttrDataType = this.AttrDataType,
                AttrReferenceType = this.AttrReferenceType,
                AttrDisplayName = this.AttrDisplayName,
                TableName = this.TableName,
                Priority = this.Priority,
                SubTypeId = this.SubTypeId,
                SectypeTableId = this.SectypeTableId,
                OldSectypeTableId = this.OldSectypeTableId,
                ReferenceAttr = this.ReferenceAttr,
                TableDisplayName = this.TableDisplayName,
                SecTypeId = this.SecTypeId,
                SecTypeName = this.SecTypeName,
                Tags = this.Tags,
                ToShow = this.ToShow,
                IsMandatory = this.IsMandatory,
                DataLength = this.DataLength,
                IsAdditionalLegAttribute = this.IsAdditionalLegAttribute,
                IsCommon = this.IsCommon,
                IsReadOnly = this.IsReadOnly,
                LegName = this.LegName,
                UnderlyingSectypeIds = this.UnderlyingSectypeIds
            };
        }
    }

    public class SecurityAttrInfo
    {
        public Dictionary<string, string> UnderlyingAttrNameVsRealName { get; set; }
        public Dictionary<string, AttrInfo> MasterAttrs { get; set; }
        public Dictionary<string, Dictionary<string, AttrInfo>> LegAttrs { get; set; }
        public Dictionary<string, Dictionary<string, AttrInfo>> SingleInfoLegAttrs { get; set; }
        public Dictionary<string, HashSet<string>> LegPrimaryAttr { get; set; }
        public Dictionary<string, string> LegVsUnderlyingAttribute { get; set; }
        public Dictionary<string, bool> LegsVsAdditionalOrNot { get; set; }
    }

    public class SecurityTypeMasterInfo
    {
        public int SectypeId { get; set; }
        public SecurityAttrInfo AttributeInfo { get; set; }
    }

    [DataContract]
    public enum SRMEncodingType
    {
        [EnumMember]
        Default = 1,
        [EnumMember]
        ASCII = 2,
        [EnumMember]
        Unicode = 3,
        [EnumMember]
        UTF32 = 4,
        [EnumMember]
        UTF7 = 5,
        [EnumMember]
        UTF8 = 6
    }

    #region MultiTenant
    public static class SRMMTCommon
    {
        public static IRLogger CreateJobLogger(bool isMultiTenant, string logFileName, string serviceName, string directoryName, out string loggingPath, string clientName)
        {
            loggingPath = string.Empty;
            IRLogger mLogger = null;
            List<XmlNode> nodeToRemove = new List<XmlNode>();
            try
            {
                if (isMultiTenant)
                {
                    Dictionary<string, string> appenderNameVsClientName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                    List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
                    foreach (XmlDocument loggerDoc in lstConfig)
                    {
                        XmlNodeList loggerList = loggerDoc.GetElementsByTagName("logger");
                        foreach (XmlNode node in loggerList)
                        {
                            string clientNameApp = node.Attributes["name"].Value;
                            string appenderName = node.SelectNodes("appender-ref")[0].Attributes["ref"].Value;
                            if (!appenderNameVsClientName.ContainsKey(appenderName))
                                appenderNameVsClientName.Add(appenderName, value: clientNameApp);

                            if (!string.IsNullOrEmpty(clientName) && !clientNameApp.Equals(clientName, StringComparison.OrdinalIgnoreCase))
                                nodeToRemove.Add(node);
                        }

                        XmlNodeList appenderList = loggerDoc.GetElementsByTagName("appender");
                        foreach (XmlNode node in appenderList)
                        {
                            string clientNameApp = node.Attributes["name"].Value;
                            if (appenderNameVsClientName.ContainsKey(clientNameApp) || string.IsNullOrEmpty(clientName))
                            {
                                XmlNodeList paramsList = node.SelectNodes("param");

                                foreach (XmlNode paramNode in paramsList)
                                {
                                    if (paramNode.Attributes["name"].Value.ToLower().Trim().Equals("file"))
                                    {
                                        string xmlFileName = paramNode.Attributes["value"].Value;
                                        if (string.IsNullOrEmpty(directoryName))
                                            directoryName = System.IO.Directory.GetParent(xmlFileName).FullName;
                                        if (string.IsNullOrEmpty(logFileName))
                                            logFileName = serviceName + "Log.txt";
                                        if (appenderNameVsClientName.ContainsKey(clientNameApp) && string.IsNullOrEmpty(clientName))
                                            paramNode.Attributes["value"].Value = directoryName + "\\" + appenderNameVsClientName[clientNameApp] + "\\" + logFileName;
                                        else if (!appenderNameVsClientName.ContainsKey(clientNameApp))
                                            paramNode.Attributes["value"].Value = directoryName + "\\Common\\" + logFileName;
                                        else
                                            paramNode.Attributes["value"].Value = directoryName + "\\" + logFileName;

                                        if (!string.IsNullOrEmpty(clientName) && appenderNameVsClientName.ContainsKey(clientNameApp) && clientName.Equals(appenderNameVsClientName[clientNameApp], StringComparison.OrdinalIgnoreCase))
                                            loggingPath = paramNode.Attributes["value"].Value;

                                        if (!string.IsNullOrEmpty(clientName) && appenderNameVsClientName.ContainsKey(clientNameApp) && !appenderNameVsClientName[clientNameApp].Equals(clientName, StringComparison.OrdinalIgnoreCase))
                                            nodeToRemove.Add(node);

                                        break;
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(clientName))
                        {
                            foreach (var node in nodeToRemove)
                                node.ParentNode.RemoveChild(node);
                        }
                    }
                    //string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CreateLogger_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt");
                    //System.IO.File.AppendAllText(fileName, string.Format("Logger XML : {0}", lstConfig[0].InnerXml));

                    mLogger = RLogFactory.CreateLogger(serviceName, lstConfig[0].InnerXml);
                }
                else
                {
                    string routingKey = string.Empty;
                    string customFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"CustomConfigFiles\LoggerConfigRoutingKeys.xml");
                    if (File.Exists(customFilePath))
                    {
                        XDocument rKeys = XDocument.Load(customFilePath);
                        if (rKeys != null)
                        {
                            foreach (XElement ele in rKeys.Root.Descendants("RoutingKey"))
                            {
                                if (ele.Attribute("name").Value.Equals(serviceName))
                                {
                                    routingKey = ele.Attribute("key").Value;
                                    break;
                                }
                            }
                        }
                    }

                    List<XmlDocument> lstConfig = RConfigurationManager.GetConfigDocument("logger");
                    foreach (XmlDocument loggerDoc in lstConfig)
                    {
                        XmlNodeList nodeList = loggerDoc.GetElementsByTagName("param");
                        foreach (XmlNode node in nodeList)
                        {
                            if (node.Attributes["name"].Value.ToLower().Trim().Equals("file"))
                            {
                                string xmlFileName = node.Attributes["value"].Value;
                                if (string.IsNullOrEmpty(directoryName))
                                    directoryName = System.IO.Directory.GetParent(xmlFileName).FullName;
                                if (string.IsNullOrEmpty(logFileName))
                                    logFileName = serviceName + "Log.txt";
                                node.Attributes["value"].Value = directoryName + "\\" + logFileName;
                            }
                        }
                        if (!string.IsNullOrEmpty(routingKey))
                        {
                            XmlNodeList RoutingKeyList = loggerDoc.GetElementsByTagName("RoutingKey");
                            if (RoutingKeyList != null)
                            {
                                foreach (XmlNode node in RoutingKeyList)
                                {
                                    node.Attributes["value"].Value = routingKey;
                                }
                            }
                        }
                    }

                    mLogger = RLogFactory.CreateLogger(serviceName, lstConfig[0].InnerXml);
                }
            }
            catch (Exception ex)
            {
                string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CreateLoggerError_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt");
                System.IO.File.AppendAllText(fileName, string.Format("Exception Message: {0}", ex));
            }
            return mLogger;
        }

        public static string GetClientFilePath(string filePath)
        {
            string fileName = string.Empty, directoryPath = filePath;
            if (Path.HasExtension(filePath))
            {
                directoryPath = Path.GetDirectoryName(filePath);
                fileName = Path.GetFileName(filePath);
            }
            if (!string.IsNullOrEmpty(RMultiTanentUtil.ClientName))
                directoryPath = Path.Combine(directoryPath, RMultiTanentUtil.ClientName);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            return Path.Combine(directoryPath, fileName);
        }
    }

    public class SRMMTConfig
    {
        internal static string MultiTenantEnabled;

        internal static bool isConfigMultiTenantRead;
        public static bool isMultiTenantEnabled
        {
            get
            {
                if (!isConfigMultiTenantRead)
                {
                    MultiTenantEnabled = RConfigReader.GetConfigAppSettings("MultiTenantEnabled");
                    if (!string.IsNullOrEmpty(MultiTenantEnabled) && MultiTenantEnabled.ToLower().Equals("true"))
                        MultiTenantEnabled = "true";
                    else
                        MultiTenantEnabled = "false";

                    isConfigMultiTenantRead = true;
                }
                return Convert.ToBoolean(MultiTenantEnabled);
            }
        }
        public static string ClientDBID = RConfigReader.GetConfigAppSettings("ClientDBID");

        public static string GetClientName()
        {
            string clientName = string.Empty;
            if (isMultiTenantEnabled)
                clientName = RMultiTanentUtil.ClientName;
            return clientName;
        }

        public static void SetClientName(string clientName)
        {
            if (isMultiTenantEnabled)
            {
                RMultiTanentUtil.ClientName = clientName;
                RMultiTanentClientUtil.ClientName = clientName;
            }
        }


        public static string SetClientNameFromHeaders(bool isTCP)
        {
            string clientName = string.Empty;
            clientName = SRMCommon.GetHeaderValue("ClientName", false, isTCP);
            if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
            {
                RMultiTanentUtil.ClientName = clientName;
                RMultiTanentClientUtil.ClientName = clientName;
            }

            return clientName;
        }

        public static void MethodCallPerClient(Action<string, int> methodToBeCalled, int moduleId)
        {
            HashSet<string> clientNames = new HashSet<string>();

            if (SRMMTConfig.isMultiTenantEnabled)
            {
                RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(SRMMTConfig.ClientDBID);
                string query = "SELECT client_real_name FROM [dbo].[ivp_ctrl_client_master] WHERE is_active = 1";
                DataSet ds = dbConnection.ExecuteQuery(query, RQueryType.Select);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        clientNames.Add(Convert.ToString(dr[0]));
                }
            }
            else
                clientNames.Add(string.Empty);

            Parallel.ForEach(clientNames, clientName =>
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                {
                    RMultiTanentUtil.ClientName = clientName;
                    RMultiTanentClientUtil.ClientName = clientName;
                }
                methodToBeCalled(clientName, moduleId);
            });
        }

        public static void MethodCallPerClient(Action<string> methodToBeCalled)
        {
            HashSet<string> clientNames = new HashSet<string>();

            if (SRMMTConfig.isMultiTenantEnabled)
            {
                RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(SRMMTConfig.ClientDBID);
                string query = "SELECT client_real_name FROM [dbo].[ivp_ctrl_client_master] WHERE is_active = 1";
                DataSet ds = dbConnection.ExecuteQuery(query, RQueryType.Select);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        clientNames.Add(Convert.ToString(dr[0]));
                }
            }
            else
                clientNames.Add(string.Empty);

            Parallel.ForEach(clientNames, clientName =>
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                {
                    RMultiTanentClientUtil.ClientName = clientName;
                    RMultiTanentUtil.ClientName = clientName;
                }

                methodToBeCalled(clientName);
            });
        }

        public static void MethodCallPerClientSequentially(Action<string> methodToBeCalled)
        {
            HashSet<string> clientNames = new HashSet<string>();

            if (SRMMTConfig.isMultiTenantEnabled)
            {
                RDBConnectionManager dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(SRMMTConfig.ClientDBID);
                string query = "SELECT client_real_name FROM [dbo].[ivp_ctrl_client_master] WHERE is_active = 1";
                DataSet ds = dbConnection.ExecuteQuery(query, RQueryType.Select);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        clientNames.Add(Convert.ToString(dr[0]));
                }
            }
            else
                clientNames.Add(string.Empty);

            foreach (string clientName in clientNames)
            {
                if (!string.IsNullOrEmpty(clientName) && SRMMTConfig.isMultiTenantEnabled)
                {
                    RMultiTanentClientUtil.ClientName = clientName;
                    RMultiTanentUtil.ClientName = clientName;
                }

                methodToBeCalled(clientName);
            }
        }

    }

    public class SRMMTHttpAuthenticateModule : IHttpModule
    {
        bool isClientNameSet = false;
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {

            context.AuthenticateRequest += context_AuthenticateRequest;

            context.PreRequestHandlerExecute += Context_PreRequestHandlerExecute;
        }

        private void Context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            ValidateHttpRequest();
        }

        void context_AuthenticateRequest(object sender, EventArgs e)
        {
            ValidateHttpRequest();
        }

        public void ValidateHttpRequest()
        {
            string clientName = string.Empty;
            string clientDisplayName = string.Empty;
            string timeZone = string.Empty;
            string token = string.Empty;
            string licensePath = string.Empty;
            StringBuilder sb = new StringBuilder();

            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["ClientName"] != null)
            {
                clientName = HttpContext.Current.Session["ClientName"].ToString();
                if (!string.IsNullOrEmpty(clientName) && !clientName.ToLower().Equals("null") && RMultiTanentUtil.ClientName != clientName)
                {
                    clientDisplayName = HttpContext.Current.Session["ClientDisplayName"].ToString();
                    timeZone = HttpContext.Current.Session["TimeZone"].ToString();
                    licensePath = HttpContext.Current.Session["LicensePath"].ToString();

                    RMultiTanentClientUtil.ClientName = clientName;
                    RMultiTanentUtil.ClientName = clientName;
                    RMultiTanentUtil.ClientDisplayName = clientDisplayName;
                    RMultiTanentUtil.TimeZone = timeZone;
                    RMultiTanentUtil.LicensePath = licensePath;

                }
            }
        }

        public static IPrincipal ProcessAuthentication()
        {
            IIdentity identity = new GenericIdentity("authenticated User");
            return (IPrincipal)identity;
        }
    }

    #endregion
}
