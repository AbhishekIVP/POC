using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.excellibrary;
using com.ivp.rad.xruleengine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using com.ivp.rad.data;
using com.ivp.rad.utils;
using System.Reflection;
using com.ivp.common;
using com.ivp.common.Migration;
using com.ivp.common.reporting;
using com.ivp.common.migration;
using com.ivp.common.DownstreamSystems;
using com.ivp.common.TransportTasks;
using com.ivp.rad.RFileParser;
using com.ivp.common.srmdwhjob;

namespace com.ivp.srmcommon
{
    public class SRMCommonMigration : ISRMMigrationInterface
    {
        public Dictionary<MigrationFeatureEnum, SRMCommonMigrationController> staticFileMetaData { get; set; }
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMCommonMigration");
        private List<string> lstAllUsers;
        private List<string> lstAllGroups;
        private Dictionary<int, HashSet<MigrationFeatureEnum>> ModuleIdVsFeatureList = new Dictionary<int, HashSet<MigrationFeatureEnum>>();

        #region  CONSTRUCTORS
        public SRMCommonMigration()
        {
            this.staticFileMetaData = new Dictionary<MigrationFeatureEnum, SRMCommonMigrationController>();
            this.lstAllUsers = SRMCommonRAD.GetAllUsersLoginNamevsDisplayName().Values.ToList();
            this.lstAllGroups = SRMCommonRAD.GetAllGroups();
            SetInitialUIConfigurationsData();
        }

        public SRMCommonMigration(List<MigrationFeatureEnum> lstFeatures, int moduleId) : this()
        {
            foreach (var item in lstFeatures)
            {
                var tempMigrationControllerObject = new SRMCommonMigrationController(item, moduleId);
                //List<CommonSheetInfo> tempItem = tempMigrationControllerObject.getLstCommonSheetInfo();
                if (tempMigrationControllerObject.getLstCommonSheetInfo() != null)
                    this.staticFileMetaData.Add(item, tempMigrationControllerObject);
                else
                    throw new Exception("File Meta Data Not Found for this Feature.");
            }
        }

        #endregion

        #region PRIVATE METHODS

        // Wrapper method over GetData to remove "Missing_" sheets from our dataset AND Convert 1st row into Column Names and remove that row.
        private ObjectSet parseExcelAndMassageDataSetForMigration(string filePath, List<string> lstSheets)
        {
            mLogger.Debug("SRMCommonMigration -> parseExcelAndMassageDataSetForMigration -> Start");
            try
            {
                //RExcelManager objRExcelManager = new RExcelManager();
                //DataSet ds = objRExcelManager.GetData(filePath, lstSheets);

                RParserManager ExcelManager = new RParserManager();
                var parserObj = ExcelManager.GetParser(FileType.Excel);
                DataSet ds = parserObj.GetData(new RParsingInfo() { FilePath = filePath, Sheetname = lstSheets });

                //Converts 1st row into Column Names and removes that row.
                MassageDatatables(ref ds);

                List<string> lstTableNames = ds.Tables.OfType<DataTable>().Select(dt => dt.TableName).ToList();

                foreach (var tableName in lstTableNames)
                {
                    if (tableName.Trim().ToLower().StartsWith("missing_"))
                    {
                        ds.Tables.Remove(tableName);
                    }

                    if (ds.Tables[tableName].Columns.Contains(SRMSpecialColumnNames.Delta_Action))
                    {
                        ds.Tables[tableName].Columns.Remove(SRMSpecialColumnNames.Delta_Action);
                    }

                    if (ds.Tables[tableName].Columns.Contains(SRMSpecialColumnNames.Remarks))
                    {
                        ds.Tables[tableName].Columns.Remove(SRMSpecialColumnNames.Remarks);
                    }

                    if (ds.Tables[tableName].Columns.Contains(SRMSpecialColumnNames.Sync_Status))
                    {
                        ds.Tables[tableName].Columns.Remove(SRMSpecialColumnNames.Sync_Status);
                    }
                }


                ObjectSet objSet = SRMCommon.ConvertDataSetToObjectSet(ds);

                return objSet;

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> parseExcelAndMassageDataSetForMigration -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> parseExcelAndMassageDataSetForMigration -> End");
            }
        }

        private ObjectSet parseXMLAndMassageDataSetForMigration(string filePath, List<string> lstSheets)
        {
            mLogger.Debug("SRMCommonMigration -> parseXMLAndMassageDataSetForMigration -> Start");
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(filePath, XmlReadMode.ReadSchema);

                //Converts 1st row into Column Names and removes that row.
                //MassageDatatables(ref ds);

                //keep only sheets present in the list of sheets
                HashSet<string> lstOfSheets = new HashSet<string>(lstSheets);
                List<string> lstTableNames = ds.Tables.OfType<DataTable>().Select(dt => dt.TableName).ToList();

                foreach (var tbl in lstTableNames)
                {
                    if (!lstOfSheets.Contains(tbl))
                        ds.Tables.Remove(tbl);
                }
                lstTableNames = null;

                lstTableNames = ds.Tables.OfType<DataTable>().Select(dt => dt.TableName).ToList();

                foreach (var tableName in lstTableNames)
                {
                    if (tableName.Trim().ToLower().StartsWith("missing_"))
                    {
                        ds.Tables.Remove(tableName);
                    }

                    if (ds.Tables[tableName].Columns.Contains(SRMSpecialColumnNames.Delta_Action))
                    {
                        ds.Tables[tableName].Columns.Remove(SRMSpecialColumnNames.Delta_Action);
                    }

                    if (ds.Tables[tableName].Columns.Contains(SRMSpecialColumnNames.Remarks))
                    {
                        ds.Tables[tableName].Columns.Remove(SRMSpecialColumnNames.Remarks);
                    }

                    if (ds.Tables[tableName].Columns.Contains(SRMSpecialColumnNames.Sync_Status))
                    {
                        ds.Tables[tableName].Columns.Remove(SRMSpecialColumnNames.Sync_Status);
                    }
                }


                ObjectSet objSet = SRMCommon.ConvertDataSetToObjectSet(ds);

                return objSet;

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> parseXMLAndMassageDataSetForMigration -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> parseXMLAndMassageDataSetForMigration -> End");
            }
        }

        //Also Removes Empty Tables
        internal static void MassageDatatables(ref DataSet ds)
        {
            if (ds != null && ds.Tables.Count > 0)
            {
                //Remove Empty Tables
                List<string> tablesToRemove = new List<string>();

                foreach (DataTable dt in ds.Tables)
                {
                    if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                    {
                        List<DataColumn> columnsToBeDeleted = new List<DataColumn>();
                        List<string> lstColumns = new List<string>();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            string columnName = Convert.ToString(dt.Rows[0][dc.ColumnName]);

                            if (!string.IsNullOrEmpty(columnName))
                            {
                                dc.ColumnName = columnName;
                                lstColumns.Add(columnName);
                            }
                            else
                                columnsToBeDeleted.Add(dc);
                        }
                        if (columnsToBeDeleted.Count > 0)
                        {
                            foreach (DataColumn dc in columnsToBeDeleted)
                            {
                                dt.Columns.Remove(dc);
                            }
                        }
                        dt.Rows.RemoveAt(0);

                        List<DataRow> rowsToDelete = new List<DataRow>();
                        if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                        {
                            foreach (DataRow dr in dt.AsEnumerable().Where(x => lstColumns.All(y => string.IsNullOrEmpty(Convert.ToString(x[y])))))
                            {
                                rowsToDelete.Add(dr);
                            }
                            foreach (DataRow dr in rowsToDelete)
                            {
                                dt.Rows.Remove(dr);
                            }
                        }
                    }

                    if (dt != null && dt.Rows.Count == 0)
                        tablesToRemove.Add(dt.TableName);
                }

                foreach (var tableName in tablesToRemove)
                    ds.Tables.Remove(tableName);
            }
        }

        // Validates Sheet(ObjectTable) Names in Excel(ObjectSet). 
        // RETURN : If valid -> Empty string is returned. If NOT valid, errorMessage is returned.
        private string validateSheets(MigrationFeatureEnum featureNameEnum, ObjectSet objSetFromExcel)
        {
            mLogger.Debug("SRMCommonMigration -> validateSheets -> Start");
            try
            {
                string errorMessage = "";

                foreach (ObjectTable objTable in objSetFromExcel.Tables)
                {
                    if (!staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Select(sheet => sheet.sheetName.ToLower().Trim()).Contains(objTable.TableName.ToLower().Trim()))
                        errorMessage += (errorMessage == "" ? "Incorrect Sheet Name(s) - " : ", ") + objTable.TableName;
                }
                return errorMessage;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateSheets -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateSheets -> End");
            }
        }

        // Validates Columns(Attributes) Names in all Sheets(ObjectTables) in Excel(ObjectSet) provided. 
        // RETURN : If valid -> Empty string is returned. If NOT valid, errorMessage is returned.
        private string validateColumnsInAllSheets(MigrationFeatureEnum featureNameEnum, ObjectSet objSetFromExcel)
        {
            mLogger.Debug("SRMCommonMigration -> validateColumnsInAllSheets -> Start");
            try
            {
                string errorMessage = "";
                string errorMessage1 = "";

                foreach (ObjectTable objTable in objSetFromExcel.Tables)
                {
                    CommonSheetInfo info = staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Where(sheet => sheet.sheetName == objTable.TableName).First();
                    if (info.allowExtraColumnsInSheet)
                    {
                        List<string> infoColumns = info.lstColumnInfo.AsEnumerable().Select(col => col.columnName.ToLower().Trim()).ToList<string>();
                        List<string> tableColumns = objTable.Columns.AsEnumerable().Select(col => col.ColumnName.ToLower().Trim()).ToList<string>();

                        if (!(infoColumns.All(tableColumns.Contains)))
                            errorMessage += (errorMessage == "" ? "Mandatory Columns missing in Sheet - " + objTable.TableName : ", ");
                    }
                    else
                    {
                        if (objTable.Columns.Count == info.lstColumnInfo.Count)
                        {
                            foreach (ObjectColumn objCol in objTable.Columns)
                            {
                                if (!info.lstColumnInfo.Select(col1 => col1.columnName.ToLower().Trim()).Contains(objCol.ColumnName.ToLower().Trim()))
                                    errorMessage1 += (errorMessage1 == "" ? "Incorrect Column Name(s) - " : ", ") + objCol.ColumnName + "(" + objTable.TableName + ")";
                            }
                        }
                        else
                        {
                            errorMessage += (errorMessage == "" ? "Column Count does NOT match in Sheet - " + objTable.TableName : ", ");
                        }
                    }
                }

                string finalErrorMsg = (string.IsNullOrEmpty(errorMessage) ? "" : errorMessage + " ") + errorMessage1;

                return finalErrorMsg.Trim();
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateColumnsInAllSheets -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateColumnsInAllSheets -> End");
            }
        }

        // This method returns whether dataset is valid. 
        // RETURN : If valid -> Empty string is returned. If NOT valid, errorMessage is returned.
        private string validateObjectSet(MigrationFeatureEnum featureNameEnum, ObjectSet objSetFromExcel)
        {
            mLogger.Debug("SRMCommonMigration -> validateObjectSet -> Start");
            try
            {
                string errorMessage = validateSheets(featureNameEnum, objSetFromExcel);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = validateColumnsInAllSheets(featureNameEnum, objSetFromExcel);
                }

                return errorMessage;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateObjectSet -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateObjectSet -> End");
            }
        }

        //will be called after validatecolumnorder - NOT REQUIRED in OBJECTTABLE
        //private DataSet changeColumnOrder(MigrationFeatureEnum featureNameEnum, DataSet dsExcel, DataSet dsFromDB)
        //{
        //    mLogger.Debug("SRMCommonMigration -> changeColumnOrder -> Start");

        //    //try
        //    //{
        //    // DataSet dsClone = new DataSet();
        //    //   DataTable dtclone = null;
        //    //foreach (DataTable dt in dsFromDB.Tables)
        //    //{
        //    //   dtclone = dsExcel.Tables[dt.TableName].Clone();
        //    //   dtclone.TableName = dt.TableName;
        //    //int col = 0;
        //    //foreach (DataColumn dc in dt.Columns)
        //    //{

        //    //    dtclone.Columns[dc.ColumnName].DataType = dc.DataType;
        //    //   dtclone.Columns[dc.ColumnName].SetOrdinal(col);
        //    //dsExcel.Tables[dt.TableName].Columns[dc.ColumnName].SetOrdinal(col);

        //    //col++;
        //    //     }
        //    //foreach (DataRow row in dsExcel.Tables[dt.TableName].Rows)
        //    //{
        //    //    dtclone.ImportRow(row);
        //    //}
        //    //     dtclone.AcceptChanges();
        //    //    dsClone.Tables.Add(dtclone);
        //    //        dsExcel.AcceptChanges();

        //    //    }
        //    //    return dsExcel;
        //    //}
        //    try
        //    {
        //        foreach (DataTable dt in dsFromDB.Tables)
        //        {
        //            int col = 0;
        //            foreach (DataColumn dc in dt.Columns)
        //            {
        //                dsExcel.Tables[dt.TableName].Columns[dc.ColumnName].SetOrdinal(col);
        //                col++;
        //            }
        //            dsExcel.Tables[dt.TableName].AcceptChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        mLogger.Error("SRMCommonMigration -> changeColumnOrder -> Error : " + ex.ToString());
        //        throw ex;
        //    }
        //    finally
        //    {
        //        mLogger.Debug("SRMCommonMigration -> changeColumnOrder -> End");
        //    }
        //    return dsExcel;

        //}

        private Dictionary<string, Dictionary<string, List<ObjectRow>>> getPrimaryAttrDictForObjectSet(MigrationFeatureEnum featureNameEnum, ObjectSet objSetInput, ObjectSet objSetGetDelta, bool isUploadedData)
        {
            mLogger.Debug("SRMCommonMigration -> getPrimaryAttrDictForObjectSet -> Start");
            try
            {
                Dictionary<string, Dictionary<string, List<ObjectRow>>> dictPrimaryAttrVSLstDataRows = new Dictionary<string, Dictionary<string, List<ObjectRow>>>(StringComparer.OrdinalIgnoreCase);
                foreach (ObjectTable objTableSheet in objSetInput.Tables)
                {
                    Dictionary<string,bool> dictColumnNameVsAllowblank = staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Where(sheet => sheet.sheetName == objTableSheet.TableName).First().lstColumnInfo.AsEnumerable().ToDictionary(x => x.columnName, x => x.allowBlanks);
                    var lstPrimary = staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Where(sheet => sheet.sheetName == objTableSheet.TableName).First().lstPrimaryAttr;
                    foreach (ObjectRow objRowSheet in objTableSheet.Rows)
                    {
                        if (isUploadedData)
                        {
                            objRowSheet.Table.Columns.ToList().ForEach(
                                objCol =>
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(objRowSheet[objCol])))
                                        objRowSheet[objCol] = Convert.ToString(objRowSheet[objCol]).Trim();
                                }
                            );
                        }

                        string consolidatedKey = "";
                        string sheetName = objTableSheet.TableName;
                        string primaryColVal = string.Empty;
                        bool isAnyPrimaryColValueEmpty = false;
                        foreach (string primaryColName in staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Where(sheet => sheet.sheetName == objTableSheet.TableName).First().lstPrimaryAttr)
                        {
                            primaryColVal = Convert.ToString(objRowSheet[primaryColName]);
                            if (!string.IsNullOrEmpty(primaryColVal))
                                primaryColVal = primaryColVal.Trim();
                            else
                            {
                                if(lstPrimary.Count() == 1)
                                {
                                    addErrorToRow(objRowSheet, primaryColName + " - " + SRMErrorMessages.Value_Can_Not_Be_Empty, true);
                                    //objRowSheet[SRMSpecialColumnNames.Remarks] +=
                                    //(!string.IsNullOrEmpty(Convert.ToString(objRowSheet[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") +
                                    //(primaryColName + " - " + SRMErrorMessages.Value_Can_Not_Be_Empty);
                                    //objRowSheet[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;

                                    isAnyPrimaryColValueEmpty = true;
                                }
                                else if (!dictColumnNameVsAllowblank[primaryColName])
                                {
                                    addErrorToRow(objRowSheet, primaryColName + " - " + SRMErrorMessages.Value_Can_Not_Be_Empty, true);
                                    //objRowSheet[SRMSpecialColumnNames.Remarks] +=
                                    //(!string.IsNullOrEmpty(Convert.ToString(objRowSheet[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") +
                                    //(primaryColName + " - " + SRMErrorMessages.Value_Can_Not_Be_Empty);
                                    //objRowSheet[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;

                                    isAnyPrimaryColValueEmpty = true;
                                }
                            }
                            consolidatedKey += (consolidatedKey == "" ? "" : SRMMigrationSeparators.Consolidated_Keys_Separator) + primaryColVal;
                        }

                        if (isAnyPrimaryColValueEmpty)
                        {
                            if (isUploadedData)
                            {
                                ObjectTable objTableGetDelta = new ObjectTable();
                                objTableGetDelta.TableName = sheetName;
                                objTableGetDelta = objSetInput.Tables[sheetName].Clone();
                                objTableGetDelta.Columns.Add(SRMSpecialColumnNames.Delta_Action, typeof(string));

                                //Adding those rows to Delta Table for which atleast one of the primary columns is empty.
                                if (!objSetGetDelta.Tables.Contains(sheetName))
                                {
                                    objSetGetDelta.Tables.Add(objTableGetDelta);
                                }
                                List<ObjectRow> lstObjRows = new List<ObjectRow>();
                                lstObjRows.Add(objRowSheet);

                                ObjectTable objTable = lstObjRows.CopyToObjectTable();
                                ObjectColumn objCol = new ObjectColumn(SRMSpecialColumnNames.Delta_Action, typeof(string));
                                objCol.DefaultValue = "";
                                objTable.Columns.Add(objCol);
                                objSetGetDelta.Tables[sheetName].Merge(objTable);
                            }
                            //break;
                        }

                        if (string.IsNullOrEmpty(consolidatedKey) && !isUploadedData)
                        {
                            throw new Exception("Invalid Data in DB (All primary columns are empty in atleast one case in DB) --> Sheet Name : " + sheetName);
                        }

                        if (!isAnyPrimaryColValueEmpty)
                        {
                            if (!dictPrimaryAttrVSLstDataRows.ContainsKey(objTableSheet.TableName))
                            {
                                dictPrimaryAttrVSLstDataRows[sheetName] = new Dictionary<string, List<ObjectRow>>(StringComparer.OrdinalIgnoreCase);
                                dictPrimaryAttrVSLstDataRows[sheetName].Add(consolidatedKey, new List<ObjectRow> { objRowSheet });
                            }
                            else if (!dictPrimaryAttrVSLstDataRows[sheetName].ContainsKey(consolidatedKey))
                            {
                                dictPrimaryAttrVSLstDataRows[sheetName][consolidatedKey] = new List<ObjectRow>();
                                dictPrimaryAttrVSLstDataRows[sheetName][consolidatedKey].Add(objRowSheet);
                            }
                            else
                            {
                                dictPrimaryAttrVSLstDataRows[sheetName][consolidatedKey].Add(objRowSheet);
                            }
                        }
                    }
                }

                return dictPrimaryAttrVSLstDataRows;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> getPrimaryAttrDictForObjectSet -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> getPrimaryAttrDictForObjectSet -> End");
            }
        }

        private bool matchObjectRows(ObjectRow objRowFromDB, ObjectRow objRowFromExcel, CommonSheetInfo cmnSheetInfo)
        {
            mLogger.Debug("SRMCommonMigration -> matchObjectRows -> Start");
            try
            {
                foreach (ObjectColumn objCol in objRowFromExcel.Table.Columns)
                {
                    if (!(objCol.ColumnName.SRMEqualWithIgnoreCase(SRMSpecialColumnNames.Sync_Status) || objCol.ColumnName.SRMEqualWithIgnoreCase(SRMSpecialColumnNames.Delta_Action) || objCol.ColumnName.SRMEqualWithIgnoreCase(SRMSpecialColumnNames.Remarks)))
                    {
                        string val1 = Convert.ToString(objRowFromDB[objCol.ColumnName]);
                        string val2 = Convert.ToString(objRowFromExcel[objCol.ColumnName]);

                        //if (val2 != null)
                        //{
                        //    objRowFromExcel[objCol.ColumnName] = val2.Trim();
                        //}

                        if (val1 == null && val2 == null)
                        {
                            return true;
                        }

                        if (val1 == null || val2 == null)
                        {
                            return false;
                        }

                        CommonColumnInfo cmnColumnInfo = cmnSheetInfo.lstColumnInfo.Where(x => x.columnName.SRMEqualWithIgnoreCase(objCol.ColumnName)).FirstOrDefault();

                        if (cmnColumnInfo != null && cmnColumnInfo.isCaseSensitive)
                        {
                            if (val1.Trim() != val2.Trim())
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (val1.ToLower().Trim() != val2.ToLower().Trim())
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> matchObjectRows -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> matchObjectRows -> End");
            }
        }

        private void modifyObjectTableForSync(ref ObjectSet objSet, bool isSync)
        {
            mLogger.Debug("SRMCommonMigration -> modifyObjectTableForSync -> Start");
            try
            {
                if (objSet != null)
                {
                    foreach (ObjectTable objTable in objSet.Tables)
                    {
                        //Remarks Column
                        if (!objTable.Columns.Contains(SRMSpecialColumnNames.Remarks))
                        {
                            ObjectColumn objColumn = new ObjectColumn(SRMSpecialColumnNames.Remarks, typeof(string));
                            objColumn.DefaultValue = "";
                            objTable.Columns.Add(objColumn);
                        }
                        else
                        {
                            throw new Exception(SRMErrorMessages.Remarks_Column_Exists);
                        }

                        if (isSync)
                        {
                            //Status Column
                            if (!objTable.Columns.Contains(SRMSpecialColumnNames.Sync_Status))
                            {
                                ObjectColumn objColumn = new ObjectColumn(SRMSpecialColumnNames.Sync_Status, typeof(string));
                                objColumn.DefaultValue = "";
                                objTable.Columns.Add(objColumn);
                            }
                            else
                            {
                                throw new Exception(SRMErrorMessages.Status_Column_Exists);
                            }
                        }
                    }
                }
                else
                    throw new Exception("Object set is null");
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> modifyObjectTableForSync -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> modifyObjectTableForSync -> End");
            }
        }

        #region  DATA TYPE Validations

        private void validateDataTypeForTable(MigrationFeatureEnum featureNameEnum, ObjectTable objTable)
        {
            mLogger.Debug("SRMCommonMigration -> validateDataTypeForTable -> Start");
            //List<string> lstCompostitePrimariesToFail = new List<string>();
            try
            {
                if (objTable != null)
                {
                    string sheetName = objTable.TableName;

                    if (staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Where(x => x.sheetName == sheetName).Count() == 1)
                    {
                        CommonSheetInfo cmnSheetInfo = staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Where(x => x.sheetName == sheetName).FirstOrDefault();
                        foreach (CommonColumnInfo cmnColInfo in cmnSheetInfo.lstColumnInfo)
                        {
                            string columnName = cmnColInfo.columnName;
                            //NOTE : Check Blanks only on Non-Primary Attributes. As blank for these have already been checked.
                            if (!cmnSheetInfo.lstPrimaryAttr.Contains(columnName, StringComparer.OrdinalIgnoreCase))
                            {
                                checkBlanks(cmnColInfo, objTable);
                            }

                            switch ((int)cmnColInfo.dataTypeName)
                            {
                                case 0: //VARCHAR
                                    validateStringValue(cmnColInfo, objTable);
                                    break;
                                case 1: //INT
                                    validateIntegerValue(cmnColInfo, objTable);
                                    break;
                                case 2: //DECIMAL
                                    validateDecimalValue(cmnColInfo, objTable);
                                    break;
                                case 3: //DATE
                                    validateDateValue(cmnColInfo, objTable);
                                    break;
                                case 4: //DATETIME
                                    validateDateTimeValue(cmnColInfo, objTable);
                                    break;
                                case 5: //BIT
                                    validateBooleanValue(cmnColInfo, objTable);
                                    break;
                                case 6: //ENUM
                                    validateEnumValue(cmnColInfo, objTable, featureNameEnum);
                                    break;
                                default:
                                    throw new Exception(SRMErrorMessages.Invalid_Data_Type);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Static Meta Data Not Correctly Initialised for Sheet : " + sheetName);
                    }
                }
                else
                {
                    throw new ArgumentNullException();
                }

                return;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateDataTypeForTable -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateDataTypeForTable -> End");
            }
        }

        //Gives Error Msg in Remarks if value in cell is blank and Column doesnt allow blanks.
        private void checkBlanks(CommonColumnInfo cmnColInfo, ObjectTable objTable)
        {
            mLogger.Debug("SRMCommonMigration -> checkBlanks -> Start");
            try
            {
                string columnName = cmnColInfo.columnName;
                if (!cmnColInfo.allowBlanks)
                {
                    objTable.AsEnumerable().Where(row => string.IsNullOrEmpty(Convert.ToString(row[columnName])) || string.IsNullOrWhiteSpace(Convert.ToString(row[columnName]))).ToList().ForEach(
                        row =>
                        {
                            addErrorToRow(row, columnName + " - " + SRMErrorMessages.Value_Can_Not_Be_Empty, false);
                            //row[SRMSpecialColumnNames.Remarks] +=
                            //    (!string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") +
                            //    (columnName + " - " + SRMErrorMessages.Value_Can_Not_Be_Empty);

                            //string compositePrimary = "";
                            //lstPrimaryAttr.ForEach(
                            //    primColName => compositePrimary += (Convert.ToString(row[primColName]) == null ? "" : Convert.ToString(row[primColName]))
                            //    );

                            //if (!lstCompostitePrimariesToFail.Contains(compositePrimary))
                            //    lstCompostitePrimariesToFail.Add(compositePrimary);
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> checkBlanks -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> checkBlanks -> End");
            }
        }

        //  1. STRING
        private void validateStringValue(CommonColumnInfo cmnColInfo, ObjectTable objTable)
        {
            mLogger.Debug("SRMCommonMigration -> validateStringValue -> Start");
            try
            {
                string columnName = cmnColInfo.columnName;
                if (!string.IsNullOrEmpty(columnName) && !string.IsNullOrEmpty(cmnColInfo.dataTypeLength))
                {
                    int precision;
                    try
                    {
                        precision = Convert.ToInt32(cmnColInfo.dataTypeLength);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(columnName + SRMErrorMessages.Meta_Data_Invalid_Data_Type_Length);
                    }

                    if (precision == -1)
                    {
                        return;
                    }
                    else if (precision >= 0)
                    {
                        objTable.AsEnumerable().Where(row => !string.IsNullOrEmpty(Convert.ToString(row[columnName]))).ToList().ForEach(
                                row =>
                                {
                                    if (Convert.ToString(row[columnName]).Length > precision)
                                    {
                                        addErrorToRow(row, columnName + " - " + SRMErrorMessages.Data_Precision_Incorrect, false);
                                    }

                                    if ((cmnColInfo.acceptorRegex != null && !cmnColInfo.acceptorRegex.IsMatch(Convert.ToString(row[columnName]))) || (cmnColInfo.rejectorRegex != null && cmnColInfo.rejectorRegex.IsMatch(Convert.ToString(row[columnName]))))
                                    {
                                        addErrorToRow(row, columnName + " - " + "Some characters are restricted.", false);
                                    }

                                    //row[SRMSpecialColumnNames.Remarks] +=
                                    //    (!string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") +
                                    //    (columnName + " - " + SRMErrorMessages.Data_Precision_Incorrect);

                                    //string compositePrimary = "";
                                    //lstPrimaryAttr.ForEach(
                                    //    primColName => compositePrimary += (Convert.ToString(row[primColName]) == null ? "" : Convert.ToString(row[primColName]))
                                    //    );

                                    //if (!lstCompostitePrimariesToFail.Contains(compositePrimary))
                                    //    lstCompostitePrimariesToFail.Add(compositePrimary);
                                }
                         );
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateStringValue -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateStringValue -> End");
            }
        }

        // 2. INT
        private void validateIntegerValue(CommonColumnInfo cmnColInfo, ObjectTable objTable)
        {
            mLogger.Debug("SRMCommonMigration -> validateIntegerValue -> Start");
            try
            {
                string columnName = cmnColInfo.columnName;
                if (!string.IsNullOrEmpty(columnName))
                {
                    int tryInt = -1;
                    objTable.AsEnumerable().Where(row => !string.IsNullOrEmpty(Convert.ToString(row[columnName])) && !Int32.TryParse(Convert.ToString(row[columnName]), out tryInt)).ToList().ForEach(
                        row =>
                        {
                            addErrorToRow(row, columnName + " - " + "Invalid Integer Value.", false);
                            //row[SRMSpecialColumnNames.Remarks] +=
                            //    (!string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") +
                            //    (columnName + " - " + SRMErrorMessages.Data_Precision_Incorrect);

                            //string compositePrimary = "";
                            //lstPrimaryAttr.ForEach(
                            //    primColName => compositePrimary += (Convert.ToString(row[primColName]) == null ? "" : Convert.ToString(row[primColName]))
                            //    );

                            //if (!lstCompostitePrimariesToFail.Contains(compositePrimary))
                            //    lstCompostitePrimariesToFail.Add(compositePrimary);
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateIntegerValue -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateIntegerValue -> End");
            }
        }

        // 2. DECIMAL
        private void validateDecimalValue(CommonColumnInfo cmnColInfo, ObjectTable objTable)
        {
            mLogger.Debug("SRMCommonMigration -> validateDecimalValue -> Start");
            try
            {
                string columnName = cmnColInfo.columnName;
                if (!string.IsNullOrEmpty(columnName))
                {
                    int decprecision;
                    int decscale;
                    decimal deccheck = -1;
                    try
                    {
                        decprecision = Convert.ToInt32(cmnColInfo.dataTypeLength.Split(',')[0]) - Convert.ToInt32(cmnColInfo.dataTypeLength.Split(',')[1]);
                        decscale = Convert.ToInt32(cmnColInfo.dataTypeLength.Split(',')[1]);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(columnName + SRMErrorMessages.Meta_Data_Invalid_Data_Type_Length);
                    }

                    objTable.AsEnumerable().Where(row => !string.IsNullOrEmpty(Convert.ToString(row[columnName])) && (Convert.ToString(row[columnName]).Split('.').Length > 2) || ((Convert.ToString(row[columnName]).Split('.')[0].Length > decprecision) || (Convert.ToString(row[columnName]).Split('.').Length == 2 && Convert.ToString(row[columnName]).Split('.')[1].Length > decscale)) || (!string.IsNullOrEmpty(Convert.ToString(row[columnName])) && !decimal.TryParse(Convert.ToString(row[columnName]), out deccheck))).ToList().ForEach(
                            row =>
                            {
                                addErrorToRow(row, columnName + " - " + SRMErrorMessages.Data_Precision_Incorrect, false);
                                //row[SRMSpecialColumnNames.Remarks] +=
                                //    (!string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") +
                                //    (columnName + " - " + SRMErrorMessages.Data_Precision_Incorrect);

                                //string compositePrimary = "";
                                //lstPrimaryAttr.ForEach(
                                //    primColName => compositePrimary += (Convert.ToString(row[primColName]) == null ? "" : Convert.ToString(row[primColName]))
                                //    );

                                //if (!lstCompostitePrimariesToFail.Contains(compositePrimary))
                                //    lstCompostitePrimariesToFail.Add(compositePrimary);
                            }
                     );
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateDecimalValue -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateDecimalValue -> End");
            }
        }

        // 3. DATE
        private void validateDateValue(CommonColumnInfo cmnColInfo, ObjectTable objTable)
        {
            mLogger.Debug("SRMCommonMigration -> validateDateValue -> Start");
            try
            {
                string columnName = cmnColInfo.columnName;
                if (!string.IsNullOrEmpty(columnName))
                {
                    objTable.AsEnumerable().Where(row => !string.IsNullOrEmpty(Convert.ToString(row[columnName]))).ToList().ForEach(
                        row =>
                        {
                            DateTime dateCheck;
                            if (!DateTime.TryParse(Convert.ToString(row[columnName]).Split(' ')[0], out dateCheck))
                            {
                                addErrorToRow(row, columnName + " - " + SRMErrorMessages.Incorrect_Date_Format, false);
                            }
                            else if ((DateTime)dateCheck < new DateTime(1753, 1, 1))
                            {
                                addErrorToRow(row, columnName + " - " + "Value must be greater than or equal to 1st Jan 1753. ", false);
                            }
                            //row[SRMSpecialColumnNames.Remarks] +=
                            //    (!string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") +
                            //    (columnName + " - " + SRMErrorMessages.Incorrect_Date_Format);

                            //    string compositePrimary = "";
                            //    lstPrimaryAttr.ForEach(
                            //        primColName => compositePrimary += (Convert.ToString(row[primColName]) == null ? "" : Convert.ToString(row[primColName]))
                            //        );

                            //    if (!lstCompostitePrimariesToFail.Contains(compositePrimary))
                            //        lstCompostitePrimariesToFail.Add(compositePrimary);
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateDateValue -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateDateValue -> End");
            }
        }

        // 4. DATETIME
        private void validateDateTimeValue(CommonColumnInfo cmnColInfo, ObjectTable objTable)
        {
            mLogger.Debug("SRMCommonMigration -> validateDateTimeValue -> Start");
            try
            {
                string columnName = cmnColInfo.columnName;
                if (!string.IsNullOrEmpty(columnName))
                {
                    objTable.AsEnumerable().Where(row => !string.IsNullOrEmpty(Convert.ToString(row[columnName]))).ToList().ForEach(
                        row =>
                        {
                            DateTime dateTimeCheck;
                            if (!DateTime.TryParse(Convert.ToString(row[columnName]), out dateTimeCheck))
                            {
                                addErrorToRow(row, columnName + " - " + SRMErrorMessages.Incorrect_DateTime_Format, false);
                            }
                            else if ((DateTime)dateTimeCheck < new DateTime(1753, 1, 1))
                            {
                                addErrorToRow(row, columnName + " - " + "Value must be greater than or equal to 1st Jan 1753. ", false);
                            }
                            //row[SRMSpecialColumnNames.Remarks] +=
                            //    (!string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") +
                            //    (columnName + " - " + SRMErrorMessages.Incorrect_DateTime_Format);

                            //string compositePrimary = "";
                            //lstPrimaryAttr.ForEach(
                            //    primColName => compositePrimary += (Convert.ToString(row[primColName]) == null ? "" : Convert.ToString(row[primColName]))
                            //    );
                            //if (!lstCompostitePrimariesToFail.Contains(compositePrimary))
                            //    lstCompostitePrimariesToFail.Add(compositePrimary);
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateDateTimeValue -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateDateTimeValue -> End");
            }
        }

        // 5. BOOLEAN
        private void validateBooleanValue(CommonColumnInfo cmnColInfo, ObjectTable objTable)
        {
            mLogger.Debug("SRMCommonMigration -> validateBooleanValue -> Start");
            try
            {
                string columnName = cmnColInfo.columnName;
                if (!string.IsNullOrEmpty(columnName))
                {
                    bool boolCheck;
                    objTable.AsEnumerable().Where(row => !string.IsNullOrEmpty(Convert.ToString(row[columnName])) && !bool.TryParse(Convert.ToString(row[columnName]), out boolCheck)).ToList().ForEach(
                        row =>
                        {
                            addErrorToRow(row, columnName + " - " + SRMErrorMessages.Incorrect_Boolean_Value, false);
                            //row[SRMSpecialColumnNames.Remarks] +=
                            //    (!string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") +
                            //    (columnName + " - " + SRMErrorMessages.Incorrect_Boolean_Value);

                            //string compositePrimary = "";
                            //lstPrimaryAttr.ForEach(
                            //    primColName => compositePrimary += (Convert.ToString(row[primColName]) == null ? "" : Convert.ToString(row[primColName]))
                            //    );

                            //if (!lstCompostitePrimariesToFail.Contains(compositePrimary))
                            //    lstCompostitePrimariesToFail.Add(compositePrimary);
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateBooleanValue -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateBooleanValue -> End");
            }
        }

        // 6. ENUM_VARCHAR
        private void validateEnumValue(CommonColumnInfo cmnColInfo, ObjectTable objTable, MigrationFeatureEnum featureNameEnum)
        {
            mLogger.Debug("SRMCommonMigration -> validateEnumValue -> Start");
            try
            {
                string columnName = cmnColInfo.columnName;

                if (!string.IsNullOrEmpty(columnName))
                {
                    objTable.AsEnumerable().Where(row => !string.IsNullOrEmpty(Convert.ToString(row[columnName]))).ToList().ForEach(
                    row =>
                        {
                            string remark = "";
                            string columnValue = Convert.ToString(row[columnName]).Trim();

                            if (!string.IsNullOrEmpty(columnValue))
                            {
                                if (cmnColInfo.lookupType == LookupType.RAD_USERS_OR_GROUPS)
                                {
                                    if (row.Table.Columns.Contains(cmnColInfo.lookupColumnName))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(row[cmnColInfo.lookupColumnName])))
                                        {
                                            //SYSTEM
                                            if (Convert.ToString(row[cmnColInfo.lookupColumnName]).Trim().SRMEqualWithIgnoreCase(SM_Layout_Type.System))
                                            {
                                                if (!columnValue.SRMEqualWithIgnoreCase(SM_Layout_Type.System))
                                                {
                                                    remark = columnName + " - " + SRMErrorMessages.Invalid_Data_In_Enum_Varchar;
                                                }
                                            }
                                            //USER
                                            else if (Convert.ToString(row[cmnColInfo.lookupColumnName]).Trim().SRMEqualWithIgnoreCase(SM_Layout_Type.User))
                                            {
                                                validateUsersOrGroups(this.lstAllUsers, columnName, columnValue, SRMErrorMessages.Invalid_User_In_Enum_Varchar, out remark);
                                            }
                                            //GROUP
                                            else if (Convert.ToString(row[cmnColInfo.lookupColumnName]).Trim().SRMEqualWithIgnoreCase(SM_Layout_Type.Group))
                                            {
                                                validateUsersOrGroups(this.lstAllGroups, columnName, columnValue, SRMErrorMessages.Invalid_Group_In_Enum_Varchar, out remark);
                                            }
                                        }
                                        else
                                        {
                                            remark = cmnColInfo.lookupColumnName + " - " + SRMErrorMessages.Invalid_Data_In_Enum_Varchar;
                                        }
                                    }
                                    else
                                    {
                                        remark = columnName + " - " + SRMErrorMessages.Lookup_Column_Does_Not_Exist;
                                    }
                                }
                                else if (cmnColInfo.lookupType == LookupType.RAD_USERS)
                                {
                                    validateUsersOrGroups(this.lstAllUsers, columnName, columnValue, SRMErrorMessages.Invalid_User_In_Enum_Varchar, out remark);
                                }
                                else if (cmnColInfo.lookupType == LookupType.RAD_GROUPS)
                                {
                                    validateUsersOrGroups(this.lstAllGroups, columnName, columnValue, SRMErrorMessages.Invalid_Group_In_Enum_Varchar, out remark);
                                }
                                else
                                {
                                    if (!cmnColInfo.lstPossibleVal.Contains(columnValue, StringComparer.OrdinalIgnoreCase))
                                    {
                                        remark = columnName + " - " + SRMErrorMessages.Invalid_Data_In_Enum_Varchar; ;
                                    }
                                }

                                addErrorToRow(row, remark, false);
                                //row[SRMSpecialColumnNames.Remarks] += (!string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") + remark;

                                //string compositePrimary = "";
                                //lstPrimaryAttr.ForEach(
                                //    primColName => compositePrimary += (Convert.ToString(row[primColName]) == null ? "" : Convert.ToString(row[primColName]))
                                //    );

                                //if (!lstCompostitePrimariesToFail.Contains(compositePrimary))
                                //    lstCompostitePrimariesToFail.Add(compositePrimary);
                            }
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateEnumValue -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateEnumValue -> End");
            }
        }

        //Validate User(s) or Group(s) which are seperated by SRMMigrationSeperators.Remarks_Users_Or_Groups_Seperator
        private void validateUsersOrGroups(List<string> lstPossibleValues, string columnName, string columnValue, string errorMsg, out string remark)
        {
            mLogger.Debug("SRMCommonMigration -> validateUsersOrGroups -> Start");
            try
            {
                string remark_1 = "";
                remark = "";

                foreach (string ug_1 in columnValue.Split(new string[] { SRMMigrationSeparators.Remarks_Users_Or_Groups_Separator }, StringSplitOptions.None))
                {
                    string ug = ug_1.Trim();
                    if (!lstPossibleValues.Contains(ug, StringComparer.OrdinalIgnoreCase))
                    {
                        remark_1 = (!string.IsNullOrEmpty(remark_1) ? SRMMigrationSeparators.Remarks_Users_Or_Groups_Separator : "") + ug;
                    }
                }
                if (!string.IsNullOrEmpty(remark_1))
                {
                    remark = columnName + " - " + errorMsg + " - " + remark_1;
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateUsersOrGroups -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateUsersOrGroups -> End");
            }
        }

        #endregion DATA TYPE Validations


        private List<CommonMigrationSelectionInfo> PopulateCommonMigrationSelectionInfoFromObjectTable(ObjectTable data, string textField, string valueField, string additionalTextField = null)
        {
            List<CommonMigrationSelectionInfo> lstInfo = new List<CommonMigrationSelectionInfo>();
            if (data != null && data.Rows.Count > 0)
            {
                foreach (ObjectRow row in data.Rows)
                {
                    lstInfo.Add(new CommonMigrationSelectionInfo() { Text = Convert.ToString(row[textField]), Value = Convert.ToString(row[valueField]), AdditionalText = string.IsNullOrEmpty(additionalTextField) ? string.Empty : Convert.ToString(row[additionalTextField]) });
                }
            }
            return lstInfo;
        }
        private List<CommonMigrationSelectionInfo> PopulateCommonMigrationSelectionInfoFromDataTable(DataTable data, string textField, string valueField, string additionalTextField = null)
        {
            List<CommonMigrationSelectionInfo> lstInfo = new List<CommonMigrationSelectionInfo>();
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow row in data.Rows)
                {
                    lstInfo.Add(new CommonMigrationSelectionInfo() { Text = Convert.ToString(row[textField]), Value = Convert.ToString(row[valueField]), AdditionalText = string.IsNullOrEmpty(additionalTextField) ? string.Empty : Convert.ToString(row[additionalTextField]) });
                }
            }
            return lstInfo;
        }

        #endregion PRIVATE METHODS


        #region PUBLIC METHODS

        #region Rohan
        private void SetInitialUIConfigurationsData()
        {
            HashSet<MigrationFeatureEnum> SecMasterFeatures = new HashSet<MigrationFeatureEnum>();
            HashSet<MigrationFeatureEnum> RefMasterFeatures = new HashSet<MigrationFeatureEnum>();
            HashSet<MigrationFeatureEnum> FundMasterFeatures = new HashSet<MigrationFeatureEnum>();
            HashSet<MigrationFeatureEnum> PartyMasterFeatures = new HashSet<MigrationFeatureEnum>();

            //SecMaster 
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_CommonAttributes);
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_SecurityTypeModeler);
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_CommonLegs);
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_LegsOrder);
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_CommonRules);
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_UniqueKeys);
            SecMasterFeatures.Add(MigrationFeatureEnum.SRM_WorkFlowModeler);
            //SecMasterFeatures.Add(MigrationFeatureEnum.SM_VendorSettings);
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_DataSource);
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_DataSourcePrioritization);

            SecMasterFeatures.Add(MigrationFeatureEnum.SM_RealtimePreference);
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_DownstreamReports);
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_DownstreamSystems);
            SecMasterFeatures.Add(MigrationFeatureEnum.SM_DownstreamTasks);

            SecMasterFeatures.Add(MigrationFeatureEnum.SM_TransportTasks);

            SecMasterFeatures.Add(MigrationFeatureEnum.SM_ValidationTasks);

            SecMasterFeatures.Add(MigrationFeatureEnum.SM_CommonConfig);

            ModuleIdVsFeatureList.Add(3, SecMasterFeatures);

            //Refmaster

            RefMasterFeatures.Add(MigrationFeatureEnum.RM_EntityTypeModeler);
            RefMasterFeatures.Add(MigrationFeatureEnum.SRM_WorkFlowModeler);
            RefMasterFeatures.Add(MigrationFeatureEnum.RM_DataSource);
            RefMasterFeatures.Add(MigrationFeatureEnum.RM_Prioritization);
            RefMasterFeatures.Add(MigrationFeatureEnum.RM_RealtimePreference);
            RefMasterFeatures.Add(MigrationFeatureEnum.RM_Reports);
            RefMasterFeatures.Add(MigrationFeatureEnum.RM_DownstreamSystems);
            //RefMasterFeatures.Add(MigrationFeatureEnum.RM_ValidationTasks);
            RefMasterFeatures.Add(MigrationFeatureEnum.RM_DownstreamTasks);
            RefMasterFeatures.Add(MigrationFeatureEnum.RM_TransportTasks);
            RefMasterFeatures.Add(MigrationFeatureEnum.RM_TimeSeriesUpdateTasks);
            RefMasterFeatures.Add(MigrationFeatureEnum.SRM_TaskManager);
            RefMasterFeatures.Add(MigrationFeatureEnum.SRM_VendorSettings);
            RefMasterFeatures.Add(MigrationFeatureEnum.SRM_DownstreamSync);
            ModuleIdVsFeatureList.Add(6, RefMasterFeatures);

            //Fundmaster
            FundMasterFeatures.Add(MigrationFeatureEnum.RM_EntityTypeModeler);
            FundMasterFeatures.Add(MigrationFeatureEnum.SRM_WorkFlowModeler);
            FundMasterFeatures.Add(MigrationFeatureEnum.RM_DataSource);
            FundMasterFeatures.Add(MigrationFeatureEnum.RM_Prioritization);
            FundMasterFeatures.Add(MigrationFeatureEnum.RM_RealtimePreference);
            FundMasterFeatures.Add(MigrationFeatureEnum.RM_Reports);
            //FundMasterFeatures.Add(MigrationFeatureEnum.RM_DownstreamSystems);
            FundMasterFeatures.Add(MigrationFeatureEnum.RM_DownstreamTasks);
            // FundMasterFeatures.Add(MigrationFeatureEnum.RM_TransportTasks);
            FundMasterFeatures.Add(MigrationFeatureEnum.RM_TimeSeriesUpdateTasks);
            ModuleIdVsFeatureList.Add(18, FundMasterFeatures);

            //Partymaster
            PartyMasterFeatures.Add(MigrationFeatureEnum.RM_EntityTypeModeler);
            PartyMasterFeatures.Add(MigrationFeatureEnum.SRM_WorkFlowModeler);
            PartyMasterFeatures.Add(MigrationFeatureEnum.RM_DataSource);
            PartyMasterFeatures.Add(MigrationFeatureEnum.RM_Prioritization);
            PartyMasterFeatures.Add(MigrationFeatureEnum.RM_RealtimePreference);
            PartyMasterFeatures.Add(MigrationFeatureEnum.RM_Reports);
            //PartyMasterFeatures.Add(MigrationFeatureEnum.RM_DownstreamSystems);
            PartyMasterFeatures.Add(MigrationFeatureEnum.RM_DownstreamTasks);
            //PartyMasterFeatures.Add(MigrationFeatureEnum.RM_TransportTasks);
            PartyMasterFeatures.Add(MigrationFeatureEnum.RM_TimeSeriesUpdateTasks);

            ModuleIdVsFeatureList.Add(20, PartyMasterFeatures);
        }

        public Dictionary<int, string> GetEnumValueVsFeatureDisplayName()
        {
            return Enum<MigrationFeatureEnum>.ValueVsDescription;
        }

        public HashSet<MigrationFeatureEnum> GetFeaturesList(int moduleId)
        {
            return ModuleIdVsFeatureList[moduleId];
        }

        public Dictionary<MigrationFeatureEnum, Tuple<string, string>> ValidateFileInfoAndGetFileEnumMapping(FileInfo[] fileInfo, int moduleId)
        {
            MigrationFeatureEnum? currMigEnum = null;
            var featuresList = GetFeaturesList(moduleId);
            var featureDescriptions = featuresList.ToDictionary(x => x, x => x.GetDescription());
            Dictionary<MigrationFeatureEnum, Tuple<string, string>> featureEnumVsFilePathVsFileExtension = new Dictionary<MigrationFeatureEnum, Tuple<string, string>>();

            foreach (var file in fileInfo)
            {
                if (!ValidationFileTypes(file.Extension))
                {
                    throw new Exception("File type: " + file.Name + " is not in supported format.");
                }

                SRMModuleNames selectedModuleEnum = (SRMModuleNames)moduleId;
                if (!file.Name.Contains(selectedModuleEnum.GetDescription() + " -"))
                {
                    throw new Exception("Filename: " + file.Name + " uploaded is invalid. It should start with " + selectedModuleEnum.GetDescription() + " - <Configuration Name>");
                }
                string featureName = file.Name.Replace(selectedModuleEnum.GetDescription(), "");
                try
                {
                    featureName = featureName.Split('-')[1].Trim();
                }
                catch
                {
                    throw new Exception("Filename entered is not in correct format");
                }
                foreach (var item in featureDescriptions)
                {
                    if (item.Value.Equals(featureName))
                    {
                        currMigEnum = item.Key;
                        break;
                    }
                }

                if (!currMigEnum.HasValue)
                {
                    throw new Exception("File " + file.Name + " contains wrong feature name.");
                }

                featureEnumVsFilePathVsFileExtension.Add(currMigEnum.Value, new Tuple<string, string>(file.FullName, file.Extension));

            }
            Dictionary<MigrationFeatureEnum, Tuple<string, string>> resultDict = new Dictionary<MigrationFeatureEnum, Tuple<string, string>>();

            var orderOfEnums = ModuleIdVsFeatureList[moduleId];
            foreach (var item in orderOfEnums)
            {
                foreach (var item2 in featureEnumVsFilePathVsFileExtension)
                {
                    if (item == item2.Key)
                    {
                        resultDict.Add(item2.Key, item2.Value);
                        break;
                    }
                }
            }

            return resultDict;
        }

        private bool ValidationFileTypes(string fileExt)
        {
            switch (fileExt.ToLower())
            {
                case ".xls":
                case ".xlsx":
                case ".xml": return true;
                default: return false;
            }
        }

        #endregion

        public List<MigrationFeatureInfo> UploadMigrationConfiguration(List<MigrationFeatureInfo> lstFeatureInfo, bool isSync, bool requireMissing, int moduleID, string userName, string dateFormat, string dateTimeFormat, string timeFormat, out string errorMessage)
        {
            mLogger.Debug("SRMCommonMigrationController: SyncConfiguration -> Start");
            try
            {
                SRMWorkflowMigrationController obj = new SRMWorkflowMigrationController();
                errorMessage = string.Empty;

                SRMMigrationUserAction userAction;
                if (isSync)
                    userAction = SRMMigrationUserAction.Sync;
                else
                    userAction = SRMMigrationUserAction.Diff;

                DownloadMigrationConfiguration(lstFeatureInfo, null, moduleID, userName, userAction, out errorMessage,obj);
                

                if (string.IsNullOrEmpty(errorMessage))
                {
                    foreach (MigrationFeatureInfo featureInfo in lstFeatureInfo)
                    {
                        errorMessage = string.Empty;
                        featureInfo.DeltaSet = this.getDeltaOfObjectSets(featureInfo.FeatureEnum, featureInfo.SourceSet, featureInfo.TargetSet, out errorMessage, requireMissing, isSync);

                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            if (isSync)
                            {
                                this.validateDataTypeForMigration(featureInfo.FeatureEnum, featureInfo.DeltaSet);

                                if (featureInfo.FeatureEnum != MigrationFeatureEnum.SRM_WorkFlowModeler && featureInfo.FeatureEnum != MigrationFeatureEnum.SRM_TaskManager)
                                {
                                    if (moduleID == 3)
                                    {
                                        try
                                        {
                                            Assembly SecMasterMigrationManager = Assembly.Load("SecMasterMigrationManager");
                                            Type SMMigration = SecMasterMigrationManager.GetType("com.ivp.secm.secmastermigrationmanager.SMMigration");
                                            MethodInfo SyncMigrationConfiguration = SMMigration.GetMethod("SyncMigrationConfiguration");

                                            object SMMigrationObj = Activator.CreateInstance(SMMigration);

                                            var properties = SMMigrationObj.GetType().GetProperties();

                                            PropertyInfo propinfo = properties.Where(x => x.Name.Equals("staticFileMetaData")).FirstOrDefault();
                                            propinfo.SetValue(SMMigrationObj, this.staticFileMetaData, null);

                                            var resultOfReflection = SyncMigrationConfiguration.Invoke(SMMigrationObj, new object[] { featureInfo, moduleID, userName, dateFormat, dateTimeFormat, timeFormat, errorMessage });
                                            errorMessage = Convert.ToString(resultOfReflection);
                                            if (!string.IsNullOrEmpty(errorMessage))
                                            {
                                                throw new Exception(errorMessage);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            //multiple
                                            if (lstFeatureInfo.Count > 1)
                                            {
                                                featureInfo.ErrorMsg = errorMessage;
                                                featureInfo.SyncStatus = "Failure";
                                                featureInfo.IsDownloadable = false;
                                            }
                                            else
                                                throw ex;
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            SyncMigrationConfiguration(featureInfo, moduleID, userName, dateFormat, dateTimeFormat, timeFormat, out errorMessage);
                                            if (!string.IsNullOrEmpty(errorMessage))
                                                throw new Exception(errorMessage);
                                        }
                                        catch (Exception ex)
                                        {
                                            //multiple
                                            if (lstFeatureInfo.Count > 1)
                                            {
                                                featureInfo.ErrorMsg = errorMessage;
                                                featureInfo.SyncStatus = "Failure";
                                                featureInfo.IsDownloadable = false;
                                            }
                                            else
                                                throw ex;
                                        }
                                    }
                                }
                                else if (featureInfo.FeatureEnum == MigrationFeatureEnum.SRM_WorkFlowModeler)
                                {
                                    try
                                    {
                                        obj.SRMWorkflowModeler_Sync(featureInfo.TargetSet, featureInfo.DeltaSet, moduleID, userName, out errorMessage);
                                        if (!string.IsNullOrEmpty(errorMessage))
                                            throw new Exception(errorMessage);
                                    }
                                    catch (Exception ex)
                                    {
                                        //multiple
                                        if (lstFeatureInfo.Count > 1)
                                        {
                                            featureInfo.ErrorMsg = errorMessage;
                                            featureInfo.SyncStatus = "Failure";
                                            featureInfo.IsDownloadable = false;
                                        }
                                        else
                                            throw ex;
                                    }
                                }
                                else if (featureInfo.FeatureEnum == MigrationFeatureEnum.SRM_TaskManager)
                                {
                                    try
                                    {
                                        SRMMigrationController.ValidateandSaveTaskManager(featureInfo.DeltaSet, featureInfo.TargetSet, userName);
                                        if (!string.IsNullOrEmpty(errorMessage))
                                            throw new Exception(errorMessage);
                                    }
                                    catch (Exception ex)
                                    {
                                        //multiple
                                        if (lstFeatureInfo.Count > 1)
                                        {
                                            featureInfo.ErrorMsg = errorMessage;
                                            featureInfo.SyncStatus = "Failure";
                                            featureInfo.IsDownloadable = false;
                                        }
                                        else
                                            throw ex;
                                    }
                                }

                                //Remove Remarks Column in case of Sync
                                if (featureInfo.DeltaSet != null && featureInfo.DeltaSet.Tables.Count > 0)
                                {
                                    foreach (ObjectTable objTable in featureInfo.DeltaSet.Tables)
                                    {
                                        if (objTable.Columns.Contains(SRMSpecialColumnNames.Delta_Action))
                                            objTable.Columns.Remove(SRMSpecialColumnNames.Delta_Action);
                                    }
                                }

                                //SETTING featureInfo.isSyncComplete
                                if (string.IsNullOrEmpty(errorMessage))
                                {
                                    if (featureInfo.DeltaSet != null && featureInfo.DeltaSet.Tables.Count > 0)
                                    {
                                        string statusMsg = string.Empty;
                                        int syncCount = 0;
                                        int alreadyInSyncCount = 0;
                                        int totalCount = 0;
                                        foreach (ObjectTable objTable in featureInfo.DeltaSet.Tables)
                                        {
                                            if (objTable.Rows.Count > 0)
                                            {
                                                foreach (ObjectRow objRow in objTable.Rows)
                                                {
                                                    totalCount++;
                                                    if ((Convert.ToString(objRow[SRMSpecialColumnNames.Sync_Status]).SRMEqualWithIgnoreCase(SRMMigrationStatus.Passed)))
                                                    {
                                                        syncCount++;
                                                    }
                                                    else if (Convert.ToString(objRow[SRMSpecialColumnNames.Sync_Status]).SRMEqualWithIgnoreCase(SRMMigrationStatus.Already_In_Sync))
                                                    {
                                                        alreadyInSyncCount++;
                                                    }
                                                }
                                                if (syncCount + alreadyInSyncCount < totalCount)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        if (syncCount > 0 && (syncCount + alreadyInSyncCount == totalCount))
                                            statusMsg = "Success";
                                        else if (alreadyInSyncCount == totalCount)
                                        {
                                            statusMsg = "Already in Sync";
                                        }
                                        else
                                        {
                                            statusMsg = "Failure";
                                        }
                                        //Set in featureInfo
                                        featureInfo.SyncStatus = statusMsg;
                                        featureInfo.IsDownloadable = true;
                                    }
                                }
                            }
                            else
                            {
                                featureInfo.ErrorMsg = errorMessage;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                mLogger.Debug("SRMCommonMigrationController: SyncConfiguration -> Error: " + ex.Message);
                errorMessage = ex.Message;
                throw;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationController: SyncConfiguration -> End");
            }
        }

        public string SyncMigrationConfiguration(MigrationFeatureInfo featureInfo, int moduleID, string userName, string dateFormat, string dateTimeFormat, string timeFormat, out string errorMessage)
        {
            //dateFormat hardcoded to MM/dd/yyyy
            dateFormat = "MM/dd/yyyy";
            dateTimeFormat = "MM/dd/yyyy HH:mm:ss";

            errorMessage = string.Empty;
            switch (featureInfo.FeatureEnum)
            {
                case MigrationFeatureEnum.RM_DataSource:
                    new RMDataSourceMigrationSync().StartSync(featureInfo.DeltaSet, moduleID, userName, DateTime.Now, out errorMessage);
                    break;

                case MigrationFeatureEnum.RM_EntityTypeModeler:
                    new EntityTypeModelerSync().StartSync(moduleID, featureInfo.DeltaSet, userName, DateTime.Now, dateFormat, out errorMessage);
                    break;

                case MigrationFeatureEnum.RM_Reports:
                    new EMReportingController().SyncReports(featureInfo.SourceSet, featureInfo.DeltaSet, featureInfo.TargetSet, dateFormat, userName, moduleID);
                    break;

                case MigrationFeatureEnum.RM_Prioritization:
                    new RMPrioritizationSync().StartSync(moduleID, featureInfo.DeltaSet, userName, DateTime.Now, dateFormat, out errorMessage);
                    break;
                case MigrationFeatureEnum.RM_RealtimePreference:
                    new RMRealTimePreferenceSync().StartSync(moduleID, featureInfo.DeltaSet, userName, DateTime.Now, dateFormat, out errorMessage);
                    break;
                case MigrationFeatureEnum.SRM_VendorSettings:
                    new SRMVendorSettingsController().SyncVendor(featureInfo.DeltaSet, userName);
                    break;

                case MigrationFeatureEnum.RM_DownstreamSystems:
                    new RMDownstreamSystemsController().RMDownstreamSystems_Sync(featureInfo.TargetSet, featureInfo.DeltaSet, userName, out errorMessage);
                    break;
                case MigrationFeatureEnum.RM_TransportTasks:
                    new RMTransportTasksController().RMTransportTasks_Sync(featureInfo.TargetSet, featureInfo.DeltaSet, userName, dateFormat, out errorMessage);
                    break;
                case MigrationFeatureEnum.RM_DownstreamTasks:
                    new EMReportingTaskController().SyncReportingTasks(featureInfo.SourceSet, featureInfo.DeltaSet, featureInfo.TargetSet, dateFormat, userName, moduleID);
                    break;
                case MigrationFeatureEnum.RM_TimeSeriesUpdateTasks:
                    new RMTimeSeriesTaskSync().StartSync(featureInfo.DeltaSet, moduleID, userName, DateTime.Now, out errorMessage);
                    break;
                case MigrationFeatureEnum.SRM_DownstreamSync:
                    new SRMDownstreamConfiguration().StartSync(featureInfo.DeltaSet, out errorMessage);
                    break;
                default:
                    break;
            }
            return errorMessage;
        }

        public string DownloadMigrationConfiguration(List<MigrationFeatureInfo> features, List<object> selectedItems, int moduleID, string userName, SRMMigrationUserAction userAction, out string errorMessage, SRMWorkflowMigrationController obj=null)
        {
            mLogger.Debug("SRMCommonMigrationController: DownloadConfiguration -> Start");
            try
            {
                errorMessage = string.Empty;
                ObjectSet objSet = null;
                bool isUpload = (userAction != SRMMigrationUserAction.Download);


                MigrationFeatureInfo downloadWorkFlowModelerFeature = features.FirstOrDefault(x => x.FeatureEnum == MigrationFeatureEnum.SRM_WorkFlowModeler);
                if (downloadWorkFlowModelerFeature != null)
                {
                    if (obj == null)
                        obj = new SRMWorkflowMigrationController();
                    if (selectedItems != null)
                        objSet = obj.GetAllWorkflowsConfiguration(moduleID, selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>());
                    else
                        objSet = obj.GetAllWorkflowsConfiguration(moduleID, null);

                    downloadWorkFlowModelerFeature.TargetSet = objSet;
                }

                if (moduleID == 3)
                {
                    Assembly SecMasterMigrationManager = Assembly.Load("SecMasterMigrationManager");
                    Type SMMigration = SecMasterMigrationManager.GetType("com.ivp.secm.secmastermigrationmanager.SMMigration");
                    MethodInfo DownloadMigrationConfiguration = SMMigration.GetMethod("DownloadMigrationConfiguration");

                    object SMMigrationObj = Activator.CreateInstance(SMMigration);

                    var properties = SMMigrationObj.GetType().GetProperties();

                    PropertyInfo propinfo = properties.Where(x => x.Name.Equals("staticFileMetaData")).FirstOrDefault();
                    propinfo.SetValue(SMMigrationObj, this.staticFileMetaData, null);

                    var resultOfReflection = DownloadMigrationConfiguration.Invoke(SMMigrationObj, new object[] { features, selectedItems, moduleID, userName, userAction, errorMessage,obj });
                    errorMessage = Convert.ToString(resultOfReflection);
                }
                else
                {
                    bool removeInternalColumn = true;
                    foreach (var feature in features)
                    {
                        if (feature.FeatureEnum == MigrationFeatureEnum.SRM_WorkFlowModeler)
                            continue;
                        switch (feature.FeatureEnum)
                        {
                            case MigrationFeatureEnum.RM_DataSource:
                                //objSet = RMGetDataSourceConfiguration(selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>(), moduleID);
                                objSet = new RMDataSourceControllerNew().GetDataSourceConfiguration((selectedItems != null && selectedItems.Count > 0) ? selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>() : null, moduleID, false, false);
                                break;
                            case MigrationFeatureEnum.RM_TimeSeriesUpdateTasks:
                                objSet = new RMTimeSeriesController().GetTimeSeriesTaskConfiguration((selectedItems != null && selectedItems.Count > 0) ? selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>() : null, moduleID, false);
                                break;
                            case MigrationFeatureEnum.RM_Reports:
                                removeInternalColumn = true;
                                if (userAction == SRMMigrationUserAction.Sync)
                                    removeInternalColumn = false;
                                objSet = new EMReportingController().GetReportMetadata(selectedItems == null ? null : selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>(), userName, moduleID, removeInternalColumn);
                                break;
                            case MigrationFeatureEnum.RM_EntityTypeModeler:
                                objSet = new RMModelerController().GetModelerConfigurationDetails(moduleID, selectedItems == null ? null : selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>());
                                break;
                            case MigrationFeatureEnum.RM_Prioritization:
                                objSet = new RMPrioritizationController().GetPrioritizationConfiguration(moduleID, selectedItems == null ? new List<int>() : selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>(), false);
                                break;
                            case MigrationFeatureEnum.SRM_TaskManager:
                                selectedItems = selectedItems ?? new List<object>();
                                objSet = SRMMigrationController.GetTaskManagerConfiguration(selectedItems.Select(s => Convert.ToString(s)).ToList());
                                break;
                            case MigrationFeatureEnum.SRM_VendorSettings:
                                objSet = new SRMVendorManagement().GetVendorSystemSettingsConfiguration(selectedItems == null ? null : selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>());
                                break;

                            case MigrationFeatureEnum.RM_RealtimePreference:
                                objSet = new RMPreferenceController().GetRealTimePreferences(moduleID, selectedItems == null ? new List<int>() : selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>(), false);
                                break;
                            case MigrationFeatureEnum.RM_DownstreamTasks:
                                removeInternalColumn = true;
                                if (userAction == SRMMigrationUserAction.Sync)
                                    removeInternalColumn = false;
                                objSet = new EMReportingTaskController().GetReportingTaskMetadata(selectedItems == null ? null : selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>(), userName, moduleID, removeInternalColumn);
                                break;
                            case MigrationFeatureEnum.RM_DownstreamSystems:
                                objSet = new RMDownstreamSystemsController().RMDownstreamSystemsDownload(moduleID, selectedItems == null ? null : selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>(), out errorMessage);
                                break;
                            case MigrationFeatureEnum.RM_TransportTasks:
                                objSet = new RMTransportTasksController().RMTransportTasksDownload(moduleID, selectedItems == null ? null : selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>(), out errorMessage);
                                break;
                            case MigrationFeatureEnum.SRM_DownstreamSync:
                                objSet = new SRMDownstreamConfiguration().GetConfiguredReports( selectedItems == null ? null : selectedItems.Select(s => Convert.ToInt32(s)).ToList<int>(), out errorMessage);
                                break;
                            default:
                                break;
                        }

                        feature.TargetSet = objSet;
                    }
                }
                return errorMessage;
            }
            catch (Exception ex)
            {
                mLogger.Debug("SRMCommonMigrationController: DownloadConfiguration -> Error: " + ex.Message);
                errorMessage = ex.Message;
                throw;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationController: DownloadConfiguration -> End");
            }
        }

        public List<CommonMigrationSelectionInfo> GetSelectableItemsForMigration(int moduleID, int feature, string userName)
        {
            mLogger.Debug("SRMCommonMigrationController: GetSelectableItemsForMigration -> Start");
            try
            {
                SRMWorkflowMigrationController obj = new SRMWorkflowMigrationController();
                mLogger.Debug("Module ID: " + moduleID.ToString());
                mLogger.Debug("Feature: " + feature.ToString());
                userName = "SYSTEM";
                List<CommonMigrationSelectionInfo> lstSelectionItems = null;
                MigrationFeatureEnum featureName = (MigrationFeatureEnum)Enum.ToObject(typeof(MigrationFeatureEnum), feature);

                if (featureName == MigrationFeatureEnum.SRM_WorkFlowModeler)
                {
                    lstSelectionItems = obj.SRMGetSelectableWorkflows(moduleID);
                }
                else if (moduleID == 3)
                {
                    Assembly SecMasterMigrationManager = Assembly.Load("SecMasterMigrationManager");
                    Type SMMigration = SecMasterMigrationManager.GetType("com.ivp.secm.secmastermigrationmanager.SMMigration");
                    MethodInfo GetSelectableItemsForMigration = SMMigration.GetMethod("GetSelectableItemsForMigration");

                    object SMMigrationObj = Activator.CreateInstance(SMMigration);

                    var properties = SMMigrationObj.GetType().GetProperties();
                    PropertyInfo propinfo = properties.Where(x => x.Name.Equals("staticFileMetaData")).FirstOrDefault();
                    propinfo.SetValue(SMMigrationObj, this.staticFileMetaData, null);

                    lstSelectionItems = (List<CommonMigrationSelectionInfo>)GetSelectableItemsForMigration.Invoke(SMMigrationObj, new object[] { moduleID, feature, userName });
                }
                else
                {
                    switch (featureName)
                    {
                        case MigrationFeatureEnum.RM_EntityTypeModeler:
                            lstSelectionItems = new SRMCommonMigrationDBController().RMGetSelectableEntityTypes(moduleID);
                            break;

                        case MigrationFeatureEnum.RM_DataSource:
                            lstSelectionItems = new SRMCommonMigrationDBController().RMGetSelectableFeeds(moduleID);
                            break;

                        case MigrationFeatureEnum.RM_Reports:
                            ObjectSet result = new EMReportingController().GetAllReports(userName, moduleID);
                            if (result != null && result.Tables.Count > 0 && result.Tables[0] != null)
                            {
                                lstSelectionItems = PopulateCommonMigrationSelectionInfoFromObjectTable(result.Tables[0], EMReportingConstants.REPORT_NAME, EMReportingConstants.REPORT_ID, EMReportingConstants.REPOSITORY_NAME);
                            }
                            break;

                        case MigrationFeatureEnum.SRM_TaskManager:
                            ObjectSet taskConfig = SRMMigrationController.GetTaskManagerConfiguration(null);
                            if (taskConfig != null && taskConfig.Tables.Count > 0 && taskConfig.Tables[0] != null)
                                lstSelectionItems = PopulateCommonMigrationSelectionInfoFromObjectTable(taskConfig.Tables[0], SM_ColumnNames.Chain_Name, SM_ColumnNames.Chain_Name);
                            break;

                        case MigrationFeatureEnum.RM_Prioritization:
                            lstSelectionItems = new SRMCommonMigrationDBController().RMGetSelectableEntityTypesForPrioritization(moduleID);
                            break;

                        case MigrationFeatureEnum.SRM_VendorSettings:
                            lstSelectionItems = new SRMCommonMigrationDBController().SRMGetSelectableVendorPreferenceNames();
                            break;

                        case MigrationFeatureEnum.RM_RealtimePreference:
                            lstSelectionItems = new SRMCommonMigrationDBController().RMGetSelectableEntityTypesForRealTimePreference(moduleID);
                            break;
                        case MigrationFeatureEnum.RM_DownstreamTasks:
                            ObjectSet downstreamTasks = new EMReportingTaskController().GetAllReportingTasks(userName, moduleID);
                            if (downstreamTasks != null && downstreamTasks.Tables.Count > 0 && downstreamTasks.Tables[0] != null)
                            {
                                lstSelectionItems = PopulateCommonMigrationSelectionInfoFromObjectTable(downstreamTasks.Tables[0], RMCommonConstants.TASK_NAME, RMCommonConstants.TASK_MASTER_ID);
                            }
                            break;
                        case MigrationFeatureEnum.RM_DownstreamSystems:
                            lstSelectionItems = new SRMCommonMigrationDBController().RMGetSelectableDownstremSystems();
                            break;
                        case MigrationFeatureEnum.RM_TransportTasks:
                            lstSelectionItems = new SRMCommonMigrationDBController().RMGetSelectableTasks();
                            break;
                        case MigrationFeatureEnum.RM_TimeSeriesUpdateTasks:
                            lstSelectionItems = new SRMCommonMigrationDBController().RMGetSelectableEntityTypesForTSTask(moduleID);
                            break;
                        case MigrationFeatureEnum.SRM_DownstreamSync:
                            lstSelectionItems = new SRMCommonMigrationDBController().SRMGetSelectableDownstreamSyncSystems();
                            break;
                        default:
                            break;
                    }
                }

                return lstSelectionItems;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationController: GetSelectableItemsForMigration -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationController: GetSelectableItemsForMigration -> End");
            }
        }


        private void ReconcileDynamicColumns(ObjectTable dbTable, ObjectTable uploadedTable, string sheetName, ref Dictionary<string, List<string>> sheetNameVsColumnsToRemove)
        {
            try
            {
                if (uploadedTable != null)
                {
                    foreach (ObjectColumn col in dbTable.Columns)
                    {
                        if (!uploadedTable.Columns.Contains(col.ColumnName))
                        {
                            if (!sheetNameVsColumnsToRemove.ContainsKey(sheetName))
                                sheetNameVsColumnsToRemove[sheetName] = new List<string>();
                            sheetNameVsColumnsToRemove[sheetName].Add(col.ColumnName);

                            ObjectColumn newCol = new ObjectColumn(col.ColumnName, col.DataType);
                            uploadedTable.Columns.Add(newCol);
                        }
                    }
                    foreach (ObjectColumn col in uploadedTable.Columns)
                    {
                        if (!dbTable.Columns.Contains(col.ColumnName))
                        {
                            ObjectColumn newCol = new ObjectColumn(col.ColumnName, col.DataType);
                            dbTable.Columns.Add(newCol);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }


        public ObjectSet getDeltaOfObjectSets(MigrationFeatureEnum featureNameEnum, ObjectSet objSetUploadedExcel, ObjectSet objSetDataFromDb, out string errorMsg, bool requireMissingTables, bool isSync)
        {
            mLogger.Debug("SRMCommonMigration -> getDeltaOfObjectSets -> Start");

            Dictionary<string, List<string>> sheetNameVsColumnsToRemove = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            try
            {
                errorMsg = "";
                string tempErrorMsg = validateObjectSet(featureNameEnum, objSetUploadedExcel);
                if (!string.IsNullOrEmpty(tempErrorMsg))
                {
                    errorMsg = "Uploaded Excel --> " + tempErrorMsg;
                }
                else
                {
                    //Extra Columns Might Be Returned
                    //string tempErrorMsg2 = validateObjectSet(featureNameEnum, objSetDataFromDb);

                    //if (!string.IsNullOrEmpty(tempErrorMsg2))
                    //{
                    //    errorMsg = "Data Fetched from DB --> " + tempErrorMsg2;
                    //}
                }

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    return null;
                }


                //Vikas Bhat, dynamic column handling
                foreach (CommonSheetInfo cmnSheetInfo in staticFileMetaData[featureNameEnum].getLstCommonSheetInfo())
                {
                    if (cmnSheetInfo.allowExtraColumnsInSheet)
                    {
                        ReconcileDynamicColumns(objSetDataFromDb.Tables[cmnSheetInfo.sheetName], objSetUploadedExcel.Tables[cmnSheetInfo.sheetName], cmnSheetInfo.sheetName, ref sheetNameVsColumnsToRemove);
                    }
                }

                //Add Remarks & Status Columns in each Object Table based on Sync               
                modifyObjectTableForSync(ref objSetDataFromDb, isSync);
                modifyObjectTableForSync(ref objSetUploadedExcel, isSync);

                ObjectSet objSetGetDelta = new ObjectSet();

                Dictionary<string, Dictionary<string, List<ObjectRow>>> dictSheetNameVSPrimaryAttrVSLstObjectRowsOSUploadedExcel = getPrimaryAttrDictForObjectSet(featureNameEnum, objSetUploadedExcel, objSetGetDelta, true);
                Dictionary<string, Dictionary<string, List<ObjectRow>>> dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB = getPrimaryAttrDictForObjectSet(featureNameEnum, objSetDataFromDb, objSetGetDelta, false);
                Dictionary<string, List<string>> sheetNameVSPrimaryAttr = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

                //Diff cases 

                //Case1: UploadedExcel has new row (primaryAttr values are new) that is not present in objectset from db

                //Case2: UploadedExcel has new row (primaryAttr values are present but the objctrow corresponding to it has some change) that is not present in objectset from db

                // dictPrimaryAttrVSLstObjectRowsOSUploadedExcel contains sheetItem (KEY) vs [PrimaryAttr (KEY) vs List of ObjectRows (VALUE)] (VALUE)
                foreach (KeyValuePair<string, Dictionary<string, List<ObjectRow>>> SheetItem in dictSheetNameVSPrimaryAttrVSLstObjectRowsOSUploadedExcel)
                {
                    // dictPrimaryAttrVSLstObjectRowsOSUploadedExcelSheetWise contains [sheet name (table name in dataset) as Key which further has value as - [ Primary Attr as Key and DataRows as value ]]
                    Dictionary<string, List<ObjectRow>> dictPrimaryAttrVSLstObjectRowsOSUploadedExcelSheetWise;
                    string sheetName = SheetItem.Key;
                    dictPrimaryAttrVSLstObjectRowsOSUploadedExcelSheetWise = SheetItem.Value;
                    ObjectTable objTableGetDelta = new ObjectTable();
                    objTableGetDelta.TableName = sheetName;
                    objTableGetDelta = objSetUploadedExcel.Tables[sheetName].Clone();
                    objTableGetDelta.Columns.Add(SRMSpecialColumnNames.Delta_Action, typeof(string));
                    //DataTable dtGetDeltaForSync = new DataTable();
                    List<string> LstPrimary = new List<string>();

                    foreach (KeyValuePair<string, List<ObjectRow>> primaryAttrItem in dictPrimaryAttrVSLstObjectRowsOSUploadedExcelSheetWise)
                    {
                        CommonSheetInfo cmnSheetInfo = staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Where(sheet => sheet.sheetName.Equals(sheetName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();


                        if (!cmnSheetInfo.allowMultiplesAgainstPrimary && primaryAttrItem.Value.Count() > 1)
                        {
                            string colList = string.Join(",", cmnSheetInfo.lstPrimaryAttr);

                            primaryAttrItem.Value.ForEach(
                                row =>
                                    {
                                        addErrorToRow(row, SRMErrorMessages.Primary_Value_Is_Duplicate + " (" + colList + ")", true);
                                        //row[SRMSpecialColumnNames.Remarks] +=
                                        //(!string.IsNullOrEmpty(Convert.ToString(row[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") +
                                        //(SRMErrorMessages.Primary_Value_Is_Duplicate + " (" + colList + ")");
                                        //row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                    }
                            );

                            //Adding those rows to Delta Table for which duplicate against primary wasn't allowed.
                            if (!objSetGetDelta.Tables.Contains(sheetName))
                            {
                                objSetGetDelta.Tables.Add(objTableGetDelta);
                            }
                            ObjectTable objTable = primaryAttrItem.Value.CopyToObjectTable();
                            ObjectColumn objCol = new ObjectColumn(SRMSpecialColumnNames.Delta_Action, typeof(string));
                            objCol.DefaultValue = "";
                            objTable.Columns.Add(objCol);
                            objSetGetDelta.Tables[sheetName].Merge(objTable);

                            //If this primary has been moved to delta set, then remove it from db collection
                            if (dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB.ContainsKey(sheetName) && dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName].ContainsKey(primaryAttrItem.Key))
                            {
                                dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName][primaryAttrItem.Key] = null;
                            }
                        }
                        else
                        {
                            bool isContinue = true;

                            if (cmnSheetInfo.allowMultiplesAgainstPrimary && primaryAttrItem.Value.Count() > 1)
                            {
                                Dictionary<string, List<ObjectRow>> dictDuplicateRowHashVsLstObjectRows = new Dictionary<string, List<ObjectRow>>(StringComparer.OrdinalIgnoreCase);
                                Dictionary<string, Dictionary<string, List<ObjectRow>>> dictUniqueKeyVSRowHashVsLstObjectRows = new Dictionary<string, Dictionary<string, List<ObjectRow>>>(StringComparer.OrdinalIgnoreCase);
                                Dictionary<string, Dictionary<string, Dictionary<string, List<ObjectRow>>>> dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows = new Dictionary<string, Dictionary<string, Dictionary<string, List<ObjectRow>>>>(StringComparer.OrdinalIgnoreCase);
                                string primaryColumnNames = "";

                                if (isSync)
                                {
                                    primaryColumnNames = string.Join(",", staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Where(x => x.sheetName == sheetName).FirstOrDefault().lstPrimaryAttr);
                                }

                                foreach (ObjectRow objRow in primaryAttrItem.Value)
                                {
                                    //DUPLICATE ROW
                                    string hashKeyDuplicateRow = "";
                                    foreach (ObjectColumn objCol in objRow.Table.Columns)
                                    {
                                        hashKeyDuplicateRow += !string.IsNullOrEmpty(Convert.ToString(objRow[objCol])) ? Convert.ToString(objRow[objCol]) : "";
                                    }
                                    if (!dictDuplicateRowHashVsLstObjectRows.ContainsKey(hashKeyDuplicateRow))
                                    {
                                        dictDuplicateRowHashVsLstObjectRows.Add(hashKeyDuplicateRow, new List<ObjectRow>());
                                    }
                                    dictDuplicateRowHashVsLstObjectRows[hashKeyDuplicateRow].Add(objRow);

                                    //UNIQUE KEYS
                                    if (isSync && cmnSheetInfo.lstUniqueKeys.Count() > 0)
                                    {
                                        foreach (var uniqueKey in cmnSheetInfo.lstUniqueKeys)
                                        {
                                            string hashKeyUniqueKey = "";
                                            string columnNamesUniqueKey = "";
                                            foreach (string colName in uniqueKey.lstUniqueColumns)
                                            {
                                                columnNamesUniqueKey += colName;
                                                hashKeyUniqueKey += (!string.IsNullOrEmpty(Convert.ToString(objRow[colName])) ? Convert.ToString(objRow[colName]) : "") + SRMMigrationSeparators.Consolidated_Keys_Separator;
                                            }

                                            if (!dictUniqueKeyVSRowHashVsLstObjectRows.ContainsKey(columnNamesUniqueKey))
                                                dictUniqueKeyVSRowHashVsLstObjectRows.Add(columnNamesUniqueKey, new Dictionary<string, List<ObjectRow>>(StringComparer.OrdinalIgnoreCase));

                                            if (!dictUniqueKeyVSRowHashVsLstObjectRows[columnNamesUniqueKey].ContainsKey(hashKeyUniqueKey))
                                                dictUniqueKeyVSRowHashVsLstObjectRows[columnNamesUniqueKey].Add(hashKeyUniqueKey, new List<ObjectRow>());

                                            dictUniqueKeyVSRowHashVsLstObjectRows[columnNamesUniqueKey][hashKeyUniqueKey].Add(objRow);
                                        }
                                    }

                                    //GROUP VALIDATION
                                    if (isSync && cmnSheetInfo.lstGroupValidations.Count() > 0)
                                    {
                                        foreach (var gv in cmnSheetInfo.lstGroupValidations)
                                        {
                                            string hashKeyGroupValidation = "";
                                            List<string> lstColumnNamesGroupValidation = new List<string>();
                                            string col1ColumnName = gv.lstGVColumns[0];
                                            foreach (string colName in gv.lstGVColumns)
                                            {
                                                lstColumnNamesGroupValidation.Add(colName);
                                                hashKeyGroupValidation += (!string.IsNullOrEmpty(Convert.ToString(objRow[colName])) ? Convert.ToString(objRow[colName]) : "") + SRMMigrationSeparators.Consolidated_Keys_Separator;
                                            }

                                            string col1Value = Convert.ToString(objRow[col1ColumnName]);

                                            string columnNamesGroupValidation = string.Join("ž", lstColumnNamesGroupValidation);

                                            if (!dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows.ContainsKey(columnNamesGroupValidation))
                                                dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows.Add(columnNamesGroupValidation, new Dictionary<string, Dictionary<string, List<ObjectRow>>>(StringComparer.OrdinalIgnoreCase));

                                            if (!dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows[columnNamesGroupValidation].ContainsKey(col1Value))
                                                dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows[columnNamesGroupValidation].Add(col1Value, new Dictionary<string, List<ObjectRow>>());

                                            if (!dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows[columnNamesGroupValidation][col1Value].ContainsKey(hashKeyGroupValidation))
                                                dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows[columnNamesGroupValidation][col1Value].Add(hashKeyGroupValidation, new List<ObjectRow>());

                                            dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows[columnNamesGroupValidation][col1Value][hashKeyGroupValidation].Add(objRow);
                                        }
                                    }
                                }

                                //Adding Errors for Duplicate Rows
                                if (dictDuplicateRowHashVsLstObjectRows.Values.Where(x => x.Count() > 1).Count() > 0)
                                {
                                    foreach (var duplicateRowList in dictDuplicateRowHashVsLstObjectRows.Values.Where(x => x.Count() > 1).ToList())
                                    {
                                        foreach (ObjectRow objRow in duplicateRowList)
                                        {
                                            addErrorToRow(objRow, SRMErrorMessages.Entire_Row_Is_Duplicate, true);
                                        }
                                    }

                                    if (isSync)
                                    {
                                        //Fail All in this Primary
                                        foreach (ObjectRow objRow in primaryAttrItem.Value)
                                        {
                                            objRow[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                        }
                                    }
                                    //Used to add those rows to delta set which are duplicate
                                    isContinue = false;
                                }

                                //Adding Errors for Unique Keys
                                if (isSync && cmnSheetInfo.lstUniqueKeys.Count() > 0)
                                {
                                    foreach (var columnNamesUniqueKey in dictUniqueKeyVSRowHashVsLstObjectRows.Keys)
                                    {
                                        var hashKeyWithMultipleRows = dictUniqueKeyVSRowHashVsLstObjectRows[columnNamesUniqueKey].Where(x => x.Value.Count() > 1);
                                        if (hashKeyWithMultipleRows.Count() > 0)
                                        {
                                            foreach (var hashKeyData in hashKeyWithMultipleRows.ToList())
                                            {
                                                string hashKeyValue = hashKeyData.Key;

                                                foreach (ObjectRow objRow in hashKeyData.Value)
                                                {
                                                    addErrorToRow(objRow, primaryColumnNames + "," + columnNamesUniqueKey + " - " + SRMErrorMessages.Unique_Columns_Within_Primary_Are_Duplicate, true);
                                                }
                                            }

                                            //Used to add those primaries' rows to delta set within which uniqueness has failed.
                                            isContinue = false;
                                        }
                                    }
                                }

                                //Adding Errors for Group Validation
                                if (isSync && cmnSheetInfo.lstGroupValidations.Count() > 0)
                                {
                                    foreach (var gvColumns in dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows.Keys)
                                    {
                                        List<string> lstColNamesGV = gvColumns.Split('ž').ToList();
                                        string colNamesGroupValidationExceptfirst = string.Join(",", lstColNamesGV.GetRange(1, lstColNamesGV.Count() - 1));

                                        foreach (var col1Value in dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows[gvColumns].Keys)
                                        {
                                            if (dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows[gvColumns][col1Value].Count > 1)
                                            {
                                                var multipleHashKeys = dictGroupValidationVSCol1ValueVSColsValuesHashVsLstObjectRows[gvColumns][col1Value].ToList();
                                                if (multipleHashKeys.Count() > 0)
                                                {
                                                    foreach (var hashKeyData in multipleHashKeys)
                                                    {
                                                        foreach (ObjectRow objRow in hashKeyData.Value)
                                                        {
                                                            addErrorToRow(objRow, colNamesGroupValidationExceptfirst + " - " + SRMErrorMessages.Values_Should_Be_Same_For_Same + " " + lstColNamesGV[0], true);
                                                        }
                                                    }

                                                    //Used to add those primaries' rows to delta set for which group validation has failed.
                                                    isContinue = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (isContinue)
                            {
                                //case 1
                                if (!dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB.ContainsKey(sheetName) || !dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName].ContainsKey(primaryAttrItem.Key))
                                {
                                    if (!objSetGetDelta.Tables.Contains(sheetName))
                                    {
                                        objSetGetDelta.Tables.Add(objTableGetDelta);
                                    }
                                    ObjectTable objTable = primaryAttrItem.Value.CopyToObjectTable();
                                    ObjectColumn objCol = new ObjectColumn(SRMSpecialColumnNames.Delta_Action, typeof(string));
                                    objCol.DefaultValue = "Insert";
                                    objTable.Columns.Add(objCol);
                                    objSetGetDelta.Tables[sheetName].Merge(objTable);
                                }

                                //case 2
                                else if (dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName].ContainsKey(primaryAttrItem.Key))
                                {
                                    bool isMatchRow = true;
                                    foreach (ObjectRow objRowFromExcel in primaryAttrItem.Value)
                                    {
                                        isMatchRow = true;
                                        if (dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName][primaryAttrItem.Key].Count() == 0)
                                        {
                                            isMatchRow = false;
                                        }
                                        else
                                        {
                                            for (int i = 0; i < dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName][primaryAttrItem.Key].Count; i++)
                                            {
                                                ObjectRow objRowFromDB = dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName][primaryAttrItem.Key][i];
                                                //isMatchRow = objRowFromExcel.ItemArray.EqualsOverride(objRowFromDB.ItemArray);
                                                isMatchRow = matchObjectRows(objRowFromDB, objRowFromExcel, cmnSheetInfo);

                                                if (isMatchRow)
                                                {
                                                    dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName][primaryAttrItem.Key].RemoveAt(i);
                                                    break;
                                                }
                                            }
                                        }
                                        if (!isMatchRow)
                                        {
                                            if (!objSetGetDelta.Tables.Contains(sheetName))
                                            {
                                                objSetGetDelta.Tables.Add(objTableGetDelta);
                                            }

                                            ObjectTable objTable = primaryAttrItem.Value.CopyToObjectTable();
                                            ObjectColumn objCol = new ObjectColumn(SRMSpecialColumnNames.Delta_Action, typeof(string));
                                            objCol.DefaultValue = "Update";
                                            objTable.Columns.Add(objCol);
                                            objSetGetDelta.Tables[sheetName].Merge(objTable);
                                            LstPrimary.Add(primaryAttrItem.Key);
                                            if (!sheetNameVSPrimaryAttr.ContainsKey(sheetName))
                                                sheetNameVSPrimaryAttr.Add(sheetName, LstPrimary);
                                            else
                                                sheetNameVSPrimaryAttr[sheetName] = LstPrimary;

                                            //If this primary has been moved to delta set, then remove it from db collection
                                            dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName][primaryAttrItem.Key] = null;

                                            break;
                                        }
                                    }

                                    //Already In Sync - All Rows matched & Count in DB Dictionary is now zero against that primary
                                    if (isSync && isMatchRow && dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName][primaryAttrItem.Key].Count() == 0)
                                    {
                                        ////////////////////////////////
                                        ////////////////////////////////
                                        ///ENTIRE PRIMARY HAS MATCHED///
                                        ////////////////////////////////
                                        ////////////////////////////////

                                        //Adding those rows to Delta Table for Already In Sync.
                                        if (!objSetGetDelta.Tables.Contains(sheetName))
                                        {
                                            objSetGetDelta.Tables.Add(objTableGetDelta);
                                        }
                                        ObjectTable objTable = primaryAttrItem.Value.CopyToObjectTable();

                                        foreach (ObjectRow objRow in objTable.Rows)
                                        {
                                            objRow[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Already_In_Sync;
                                        }

                                        ObjectColumn objCol = new ObjectColumn(SRMSpecialColumnNames.Delta_Action, typeof(string));
                                        objCol.DefaultValue = "";
                                        objTable.Columns.Add(objCol);
                                        objSetGetDelta.Tables[sheetName].Merge(objTable);
                                    }

                                }
                            }
                            else
                            {
                                //Adding those rows to Delta Table for completely duplicate rows or uniqueness or group validation within primary failed.
                                if (!objSetGetDelta.Tables.Contains(sheetName))
                                {
                                    objSetGetDelta.Tables.Add(objTableGetDelta);
                                }
                                ObjectTable objTable = primaryAttrItem.Value.CopyToObjectTable();
                                ObjectColumn objCol = new ObjectColumn(SRMSpecialColumnNames.Delta_Action, typeof(string));
                                objCol.DefaultValue = "";
                                objTable.Columns.Add(objCol);
                                objSetGetDelta.Tables[sheetName].Merge(objTable);

                                //If this primary has been moved to delta set, then remove it from db collection
                                if (dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB.ContainsKey(sheetName) && dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName].ContainsKey(primaryAttrItem.Key))
                                {
                                    dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName][primaryAttrItem.Key] = null;
                                }
                            }
                        }
                    }
                }

                //REMOVE Redundant Data from DB Dictionary
                foreach (string sheetName in dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB.Keys.ToList())
                {
                    //Remove Primaries from DB Dictionary, whose List<ObjectRows> is now empty.
                    foreach (string primaryAttrValue in dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName].Keys.ToList())
                    {
                        if (dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName][primaryAttrValue] == null || dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName][primaryAttrValue].Count == 0)
                        {
                            dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName].Remove(primaryAttrValue);
                        }
                    }

                    //Remove SheetNames from DB Dictionary, whose Dictionary<string,List<ObjectRows>> is now empty.
                    if (dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName] == null || dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB[sheetName].Count == 0)
                    {
                        dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB.Remove(sheetName);
                    }
                }

                //Case3: MISSING ->  Objectset from db has new row (primaryAttr values are new) that is not present in uploadedExcelDb

                ////////////////////////////////////////
                //CASE 4 wil NOT come in Delta or Sync//
                ////////////////////////////////////////
                //Case4: DELTA -> Objectset from db has new row (primaryAttr values are present but the objectrow corresponding to it has some change) that is not present in uploadedExcelDb

                // dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB contains sheetItem (KEY) vs [PrimaryAttr (KEY) vs List of ObjectRows (VALUE)] (VALUE)
                foreach (KeyValuePair<string, Dictionary<string, List<ObjectRow>>> SheetItem in dictSheetNameVSPrimaryAttrVSLstObjectRowsOSDataFromDB)
                {
                    // dictPrimaryAttrVSLstObjectRowsOSUploadedExcelSheetWise contains [sheet name (table name in dataset) as Key which further has value as - [ Primary Attr as Key and DataRows as value ]]
                    Dictionary<string, List<ObjectRow>> dictPrimaryAttrVSLstObjectRowsOSDataFromDBSheetWise;

                    dictPrimaryAttrVSLstObjectRowsOSDataFromDBSheetWise = SheetItem.Value;
                    string sheetName = SheetItem.Key;
                    ObjectTable objTableMissing = new ObjectTable();

                    if (!isSync && requireMissingTables)
                    {
                        objTableMissing = objSetDataFromDb.Tables[sheetName].Clone();
                        objTableMissing.TableName = "Missing_" + sheetName;
                        //Remove Remarks Column From Missing Tables
                        if (objTableMissing.Columns.Contains(SRMSpecialColumnNames.Remarks))
                        {
                            objTableMissing.Columns.Remove(SRMSpecialColumnNames.Remarks);
                        }
                    }

                    foreach (KeyValuePair<string, List<ObjectRow>> primaryAttrItem in dictPrimaryAttrVSLstObjectRowsOSDataFromDBSheetWise)
                    {
                        //case 3 - Merge in missing sheet
                        if (!dictSheetNameVSPrimaryAttrVSLstObjectRowsOSUploadedExcel.ContainsKey(sheetName) || !dictSheetNameVSPrimaryAttrVSLstObjectRowsOSUploadedExcel[sheetName].ContainsKey(primaryAttrItem.Key))
                        {
                            if (!isSync && requireMissingTables)
                            {
                                if (!objSetGetDelta.Tables.Contains(objTableMissing.TableName))
                                    objSetGetDelta.Tables.Add(objTableMissing);

                                //Remove Remarks Column From Missing Tables
                                ObjectTable objTableTemp = primaryAttrItem.Value.CopyToObjectTable();
                                if (objTableTemp.Columns.Contains(SRMSpecialColumnNames.Remarks))
                                {
                                    objTableTemp.Columns.Remove(SRMSpecialColumnNames.Remarks);
                                }

                                objSetGetDelta.Tables[objTableMissing.TableName].Merge(objTableTemp);
                            }
                        }

                        ////////////////////////////////////////////////////////////////////////////////////////////
                        //REQUIRED IN DELTA (Think of it as Sectype has been unmapped from Common Rules in Source)//
                        ////////////////////////////////////////////////////////////////////////////////////////////
                        // delta case 4 - Merge in delta sheet
                        else if (dictSheetNameVSPrimaryAttrVSLstObjectRowsOSUploadedExcel.ContainsKey(sheetName) && dictSheetNameVSPrimaryAttrVSLstObjectRowsOSUploadedExcel[sheetName].ContainsKey(primaryAttrItem.Key))
                        {
                            //bool isMatchRow = true;
                            //foreach (ObjectRow objRowFromDB in primaryAttrItem.Value)
                            //{
                            //    isMatchRow = true;
                            //    foreach (ObjectRow objRowFromExcel in dictSheetNameVSPrimaryAttrVSLstObjectRowsOSUploadedExcel[sheetName][primaryAttrItem.Key])
                            //{
                            //    //isMatchRow = objRowFromDB.ItemArray.EqualsOverride(objRowFromExcel.ItemArray);
                            //    isMatchRow = matchObjectRows(objRowFromDB, objRowFromExcel);
                            //    if (isMatchRow)
                            //        break;
                            //}
                            //if (!isMatchRow)
                            //{
                            if (!objSetGetDelta.Tables.Contains(sheetName))
                            {
                                ObjectTable objTableGetDelta = new ObjectTable();
                                objTableGetDelta.TableName = sheetName;
                                objTableGetDelta = objSetUploadedExcel.Tables[sheetName].Clone();

                                //Add Action Column
                                ObjectColumn objColAction = new ObjectColumn(SRMSpecialColumnNames.Delta_Action, typeof(string));
                                objColAction.DefaultValue = "";
                                objTableGetDelta.Columns.Add(objColAction);

                                objSetGetDelta.Tables.Add(objTableGetDelta);
                            }

                            //if (!sheetNameVSPrimaryAttr[sheetName].Contains(primaryAttrItem.Key))
                            //{
                            ObjectTable objTable = dictSheetNameVSPrimaryAttrVSLstObjectRowsOSUploadedExcel[sheetName][primaryAttrItem.Key].CopyToObjectTable();
                            ObjectColumn objCol = new ObjectColumn(SRMSpecialColumnNames.Delta_Action, typeof(string));
                            objCol.DefaultValue = "Update";
                            objTable.Columns.Add(objCol);
                            objSetGetDelta.Tables[sheetName].Merge(objTable);
                            //}
                            //}
                            //}
                        }
                    }
                }
                if (sheetNameVsColumnsToRemove != null && sheetNameVsColumnsToRemove.Count > 0)
                {
                    foreach (string sheetName in sheetNameVsColumnsToRemove.Keys)
                    {
                        if (objSetGetDelta.Tables.Contains(sheetName))
                        {
                            foreach (string columnName in sheetNameVsColumnsToRemove[sheetName])
                            {
                                if (objSetGetDelta.Tables[sheetName].Columns.Contains(columnName))
                                    objSetGetDelta.Tables[sheetName].Columns.Remove(columnName);
                            }
                        }
                    }
                }

                return objSetGetDelta;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> getDeltaOfObjectSets -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                if (sheetNameVsColumnsToRemove != null && sheetNameVsColumnsToRemove.Count > 0)
                {
                    foreach (string sheetName in sheetNameVsColumnsToRemove.Keys)
                    {
                        if (objSetUploadedExcel.Tables.Contains(sheetName))
                        {
                            foreach (string columnName in sheetNameVsColumnsToRemove[sheetName])
                            {
                                if (objSetUploadedExcel.Tables[sheetName].Columns.Contains(columnName))
                                    objSetUploadedExcel.Tables[sheetName].Columns.Remove(columnName);
                            }
                        }
                    }
                }
                mLogger.Debug("SRMCommonMigration -> getDeltaOfObjectSets -> End");
            }
        }

        public ObjectSet getObjectSetFromExcel(string filePath, string loginName, MigrationFeatureEnum featureNameEnum)
        {
            mLogger.Debug("SRMCommonMigration -> getObjectSetFromExcel -> Start");
            try
            {
                ObjectSet objSet = new ObjectSet();

                //string newfilePath = string.Empty;
                string dir = string.Empty;

                if (!File.Exists(filePath))
                    throw new Exception("Please Upload the File First");

                if (!(filePath.Split('.').ToArray().Last().ToString().ToLower().SRMEqualWithIgnoreCase("xls") ||
                    filePath.Split('.').ToArray().Last().ToString().ToLower().SRMEqualWithIgnoreCase("xlsx")))
                    throw new Exception("Please Upload Excel File Only.");

                //dir = RConfigReader.GetFullDirectoryPath("documentDirectory");
                //newfilePath = (dir + loginName + "\\" + "Diff " + featureNameEnum + "." + filePath.Split('.').ToArray().Last().ToString().ToLower()).Replace("\\\\", "\\");

                //if (File.Exists(newfilePath))
                //    File.Delete(newfilePath);

                //if (!(Directory.Exists(dir + loginName)))
                //    Directory.CreateDirectory(dir + loginName);

                //File.Move(filePath, newfilePath);

                var lstSheets = this.staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Select(x => x.sheetName).ToList();               

                objSet = parseExcelAndMassageDataSetForMigration(filePath, lstSheets);

                return objSet;

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> getObjectSetFromExcel -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> getObjectSetFromExcel -> End");
            }
        }

        public ObjectSet getObjectSetFromXML(string filePath, string loginName, MigrationFeatureEnum featureNameEnum)
        {
            mLogger.Debug("SRMCommonMigration -> getObjectSetFromXML -> Start");
            try
            {
                ObjectSet objSet = new ObjectSet();

                //string newfilePath = string.Empty;
                string dir = string.Empty;

                if (!File.Exists(filePath))
                    throw new Exception("Please Upload the File First");

                if (!(filePath.Split('.').ToArray().Last().ToString().ToLower().SRMEqualWithIgnoreCase("xml")))
                    throw new Exception("Please Upload XML File Only.");

                var lstSheets = this.staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Select(x => x.sheetName).ToList();

                objSet = parseXMLAndMassageDataSetForMigration(filePath, lstSheets);

                return objSet;

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> getObjectSetFromXML -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> getObjectSetFromXML -> End");
            }
        }

        public void validateDataTypeForMigration(MigrationFeatureEnum featureNameEnum, ObjectSet objSet)
        {
            mLogger.Debug("SRMCommonMigration -> validateDataTypeForMigration -> Start");
            try
            {
                if (!string.IsNullOrEmpty(featureNameEnum.ToString()))
                {
                    if (staticFileMetaData.ContainsKey(featureNameEnum))
                    {
                        foreach (ObjectTable objTable in objSet.Tables)
                        {
                            validateDataTypeForTable(featureNameEnum, objTable);

                            updateStatusColumn(objTable, staticFileMetaData[featureNameEnum].getLstCommonSheetInfo().Where(cmnSheetInfo => cmnSheetInfo.sheetName == objTable.TableName).FirstOrDefault().lstPrimaryAttr);
                        }
                    }
                    else
                    {
                        throw new Exception("Static Meta Data Not Initialised for Feature : " + featureNameEnum.ToString());
                    }
                }
                else
                {
                    throw new ArgumentNullException();
                }

                return;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> validateDataTypeForMigration -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigration -> validateDataTypeForMigration -> End");
            }
        }

        public static void addErrorToRow(ObjectRow objRow, string errorMsg, bool setStatusColumn)
        {
            if (objRow != null && objRow.Table.Columns.Contains(SRMSpecialColumnNames.Remarks) && !string.IsNullOrEmpty(errorMsg))
            {
                objRow[SRMSpecialColumnNames.Remarks] += (!string.IsNullOrEmpty(Convert.ToString(objRow[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") + errorMsg;
            }

            if (objRow != null && objRow.Table.Columns.Contains(SRMSpecialColumnNames.Sync_Status) && setStatusColumn)
            {
                objRow[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
            }
        }

        public static void addErrorToRow(DataRow objRow, string errorMsg, bool setStatusColumn)
        {
            if (objRow != null && objRow.Table.Columns.Contains(SRMSpecialColumnNames.Remarks) && !string.IsNullOrEmpty(errorMsg))
            {
                objRow[SRMSpecialColumnNames.Remarks] += (!string.IsNullOrEmpty(Convert.ToString(objRow[SRMSpecialColumnNames.Remarks])) ? SRMMigrationSeparators.Remarks_Separator : "") + errorMsg;
            }

            if (objRow != null && objRow.Table.Columns.Contains(SRMSpecialColumnNames.Sync_Status) && setStatusColumn)
            {
                objRow[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
            }
        }

        //Updates Status Column for Entire Primary, if any one of its rows' remarks has an error message
        public static void updateStatusColumn(ObjectTable objTable, List<string> lstPrimaryAttr)
        {
            try
            {
                if (objTable != null && lstPrimaryAttr != null && lstPrimaryAttr.Count() > 0 && objTable.Columns.Contains(SRMSpecialColumnNames.Remarks) && objTable.Columns.Contains(SRMSpecialColumnNames.Sync_Status))
                {
                    HashSet<string> hsCompostitePrimary = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    objTable.Rows.AsEnumerable().Where(objRow => !string.IsNullOrEmpty(Convert.ToString(objRow[SRMSpecialColumnNames.Remarks]))).ToList().ForEach(
                        row =>
                        {
                            string compositePrimary = "";
                            lstPrimaryAttr.ForEach(primColName =>
                                compositePrimary += !string.IsNullOrEmpty(Convert.ToString(row[primColName])) ? Convert.ToString(row[primColName]) : ""
                            );

                            hsCompostitePrimary.Add(compositePrimary);
                        }
                    );

                    if (hsCompostitePrimary.Count() > 0)
                    {
                        objTable.Rows.AsEnumerable().ToList().ForEach(
                            row =>
                            {
                                string compositePrimary = "";
                                lstPrimaryAttr.ForEach(primColName =>
                                    compositePrimary += !string.IsNullOrEmpty(Convert.ToString(row[primColName])) ? Convert.ToString(row[primColName]) : ""
                                );

                                if (hsCompostitePrimary.Contains(compositePrimary, StringComparer.OrdinalIgnoreCase))
                                {
                                    row[SRMSpecialColumnNames.Sync_Status] = SRMMigrationStatus.Failed;
                                    if (objTable.Columns.Contains(SRMSpecialColumnNames.Delta_Action))
                                    {
                                        row[SRMSpecialColumnNames.Delta_Action] = "";
                                    }
                                }
                            }
                        );
                    }
                }
                else
                {
                    throw new Exception("Invalid Input to updateStatusColumn Method");
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigration -> updateStatusColumn -> Error : " + ex.ToString());
                throw ex;
            }
        }

        #endregion PUBLIC METHODS
    }

    //public static class overrideEqualsMethod
    //{
    //    public static bool EqualsOverride(this Object[] obj, object[] target)
    //    {
    //        for (int i = 0; i < obj.Length; i++)
    //        {
    //            string str1 = Convert.ToString(obj[i]);
    //            string str2 = Convert.ToString(target[i]);
    //            //    .Equals(Convert.ToString(target[i]
    //            if (!string.IsNullOrEmpty(str1))
    //            {
    //                str1 = str1.Trim();
    //            }
    //            if (!string.IsNullOrEmpty(str2))
    //            {
    //                str2 = str2.Trim();
    //            }
    //            if (!str1.Equals(str2, StringComparison.OrdinalIgnoreCase))
    //                return false;
    //            //For handling case sensitivity in column matching
    //            //if (!(Convert.ToString(obj[i]).Equals(Convert.ToString(target[i]), StringComparison.OrdinalIgnoreCase)))
    //            //    return false;
    //        }
    //        return true;
    //    }
    //}

    public class SRMRulesImport
    {
        static string FileName = "RuleImports.txt";
        static int opType = 0;
        static IRLogger mLogger = RLogFactory.CreateLogger("RuleMigrationforRuleCatalog");
        const string BASKET_VALIDATION_RULE = "Basket Validation Rules";
        const string REPORTING_RULE_FILTER = "ReportRuleFilter";
        const string REPORTING_RULE_TRANSFORMATION = "ReportRuleTransformation";
        const string UPSTREAM_RULE_FILTER_SECURITY = "APIFTPRuleFilter";
        const string UPSTREAM_RULE_VALIDATION = "RuleValidation";
        const string UPSTREAM_RULE_FILTER = "RuleFilter";
        const string UPSTREAM_RULE_TRANSFORMATION = "RuleTransformation";

        //static Dictionary<int, int> reportSetupIdVsReportID = new Dictionary<int, int>();

        //static DataTable rule_mapping_details = new DataTable();
        private static DataSet GetSecMReportandUpstreamRuleDetails(string RulesOf)
        {
            DataSet ds = new DataSet();

            if (RulesOf.Equals("report"))
            {
                ds = commom.commondal.CommonDALWrapper.ExecuteSelectQuery(string.Format(@"
                    SELECT	rr.repository_name,
                    rs.report_name,
                    xr.rule_id,
                    xr.rule_set_id, 
                    rule_type_id,
                    rule_name, 
                    priority, 
                    rule_text,
                    xr.created_by, 
                    xr.created_on, 
                    xr.rule_state,
                    task_in_module,
                    rs.report_setup_id AS rule_dependent_id,
                    rs.report_id AS report_type_id,
                    is_additional_leg
            FROM IVPSecMasterVendor.dbo.ivp_secmv_rule_mapping srm
            INNER JOIN IVPSecMaster.dbo.ivp_rad_xrule xr ON srm.rule_set_id = xr.rule_set_id
            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_setup rs ON srm.rule_dependent_id = rs.report_setup_id
            INNER JOIN IVPSecMaster.dbo.ivp_secm_reports r ON rs.report_id = r.report_id
            INNER JOIN IVPSecMaster.dbo.ivp_secm_report_repository rr ON rr.repository_id = rs.repository_id
            WHERE srm.task_in_module = 'd' AND rs.is_active = 1 AND xr.is_active = 1
            ORDER BY rule_set_id, rule_id"), com.ivp.commom.commondal.ConnectionConstants.SecMasterVendor_Connection);

            }
            else
            {
                ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"
                    SELECT	vm.vendor_name,
                    fs.feed_name,
                    xr.rule_id,
                    xr.rule_set_id, 
                    rule_type_id, 
                    rule_name, 
                    priority, 
                    rule_text,
                    xr.created_by, 
                    xr.created_on, 
                    xr.rule_state,
                    task_in_module,
                    rule_dependent_id,
                    0 AS report_type_id,
                    is_additional_leg
            FROM IVPSecMasterVendor.dbo.ivp_secmv_rule_mapping srm
            INNER JOIN IVPSecMaster.dbo.ivp_rad_xrule xr ON srm.rule_set_id = xr.rule_set_id
            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_feed_summary fs ON srm.rule_dependent_id = fs.feed_id
            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_vendor_master vm ON vm.vendor_id = fs.vendor_id
            WHERE task_in_module = 'u' AND srm.rule_type_id <> 2 AND fs.is_active = 1 AND xr.is_active = 1 AND vm.is_active = 1
            UNION ALL
            SELECT	vm.vendor_name,
                    fs.feed_name,
                    xr.rule_id,
                    xr.rule_set_id, 
                    rule_type_id, 
                    rule_name, 
                    priority, 
                    rule_text,
                    xr.created_by, 
                    xr.created_on, 
                    xr.rule_state,
                    task_in_module,
                    CASE WHEN task_in_module = 'n' THEN sm.sectype_id WHEN task_in_module = 'u' THEN sfmm.feed_id END AS rule_dependent_id,
                    0 AS report_type_id,
                    is_additional_leg
            FROM IVPSecMasterVendor.dbo.ivp_secmv_rule_mapping srm
            INNER JOIN IVPSecMaster.dbo.ivp_rad_xrule xr ON srm.rule_set_id = xr.rule_set_id
            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_sectype_feed_mapping_master sfmm ON srm.rule_dependent_id = sfmm.sectype_feed_mapping_id
            INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master sm ON sfmm.sectype_id = sm.sectype_id
            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_feed_summary fs ON sfmm.feed_id = fs.feed_id
            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_vendor_master vm ON vm.vendor_id = fs.vendor_id
            WHERE (task_in_module = 'n' OR (srm.rule_type_Id = 2 AND task_in_module = 'u')) AND sm.is_active = 1 AND fs.is_active = 1 AND xr.is_active = 1 AND vm.is_active = 1"), com.ivp.commom.commondal.ConnectionConstants.SecMasterVendor_Connection);

            }

            return ds;
        }

        //public static void CompareAndSyncRules(DataSet RuleInfoFromExcel, string userName, int CompareWith)
        //{
        //    opType = CompareWith;
        //    DataSet RuleInfoFromDB = new DataSet();
        //    DataTable RuleInfoToProcess = new DataTable();
        //    RuleInfoToProcess.Columns.Add("rule_id", typeof(int));
        //    RuleInfoToProcess.Columns.Add("rule_set_id", typeof(int));
        //    RuleInfoToProcess.Columns.Add("rule_name", typeof(string));
        //    RuleInfoToProcess.Columns.Add("priority", typeof(int));
        //    RuleInfoToProcess.Columns.Add("rule_text", typeof(string));
        //    RuleInfoToProcess.Columns.Add("created_by", typeof(string));
        //    RuleInfoToProcess.Columns.Add("created_on", typeof(DateTime));
        //    RuleInfoToProcess.Columns.Add("rule_state", typeof(bool));
        //    //  RuleInfoToProcess.Columns.Add("task_in_module", typeof(char));
        //    //RuleInfoToProcess.Columns.Add("rule_dependent_id", typeof(int));
        //    //  RuleInfoToProcess.Columns.Add("report_type_id", typeof(int));
        //    RuleInfoToProcess.Columns.Add("is_additional_leg", typeof(bool));
        //    RuleInfoToProcess.Columns.Add("rule_type_id", typeof(int));
        //    RuleInfoToProcess.Columns.Add("to_insert", typeof(bool));


        //    DataTable ReportRulesFromExcel = RuleInfoFromExcel.Tables[0];
        //    for (int j = 0; j < ReportRulesFromExcel.Columns.Count; j++)
        //        ReportRulesFromExcel.Columns[j].ColumnName = ReportRulesFromExcel.Rows[0][j].ToString();

        //    ReportRulesFromExcel = ReportRulesFromExcel.Rows.Cast<DataRow>().Where(row => !row.ItemArray.Any(field => field is DBNull || string.IsNullOrWhiteSpace(field.ToString()))).CopyToDataTable();

        //    ReportRulesFromExcel.Rows.Remove(ReportRulesFromExcel.Rows[0]);

        //    Dictionary<string, Dictionary<string, DataTable>> ReportDictionaryDB = new Dictionary<string, Dictionary<string, DataTable>>();
        //    Dictionary<string, Dictionary<string, ReportRuleInfo>> ReportOrFeedNameVsRuleInfo = new Dictionary<string, Dictionary<string, ReportRuleInfo>>();
        //    Dictionary<string, List<string>> RepositoryVsReportName = new Dictionary<string, List<string>>();

        //    RuleInfoFromDB = GetSecMReportandUpstreamRuleDetails("report");

        //    ReportDictionaryDB = RuleInfoFromDB.Tables[0].AsEnumerable().GroupBy(x => x.Field<string>("repository_name")).ToDictionary(x => x.Key, y => y.GroupBy(z => z.Field<string>("report_name")).ToDictionary(a => a.Key, b => b.CopyToDataTable()));

        //    //Dictionary<string, Dictionary<string, DataTable>> ReportnameVsRuletypeVsPriorityExcel = ReportRulesFromExcel.AsEnumerable().GroupBy(x => x.Field<string>("Report Name")).ToDictionary(x => x.Key, y => y.GroupBy(z => z.Field<string>("Rule Type")).ToDictionary(a => a.Key, b => b.CopyToDataTable()));

        //    if (RuleInfoFromDB.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow row in RuleInfoFromDB.Tables[0].AsEnumerable())
        //        {
        //            //var RepositoryName = Convert.ToString(row["repository_name"]);
        //            ReportRuleInfo ruleobj = new ReportRuleInfo();
        //            ruleobj.rule_id = Convert.ToInt32(row["rule_id"]);
        //            ruleobj.rule_set_id = Convert.ToInt32(row["rule_set_id"]);
        //            ruleobj.priority = Convert.ToInt32(row["priority"]);
        //            ruleobj.rule_text = Convert.ToString(row["rule_text"]);
        //            ruleobj.rule_state = Convert.ToBoolean(row["rule_state"]);
        //            ruleobj.created_by = Convert.ToString(row["created_by"]);
        //            ruleobj.created_on = Convert.ToDateTime(row["created_on"]);
        //            ruleobj.task_in_module = Convert.ToString(row["task_in_module"]);
        //            ruleobj.rule_dependent_id = Convert.ToInt32(row["rule_dependent_id"]);
        //            ruleobj.report_type_id = Convert.ToInt32(row["report_type_id"]);
        //            ruleobj.is_additional_leg = Convert.ToBoolean(row["is_additional_leg"]);
        //            ruleobj.rule_type_id = Convert.ToInt32(row["rule_type_id"]);

        //            string ReportName = Convert.ToString(row["report_name"]);
        //            string ReposName = Convert.ToString(row["repository_name"]);
        //            if (!ReportOrFeedNameVsRuleInfo.ContainsKey(ReportName))
        //                ReportOrFeedNameVsRuleInfo[ReportName] = new Dictionary<string, ReportRuleInfo>();
        //            if (CompareWith == 1)
        //                ReportOrFeedNameVsRuleInfo[ReportName][Convert.ToString(row["priority"])] = ruleobj;
        //            else if (CompareWith == 2)
        //                ReportOrFeedNameVsRuleInfo[ReportName][Convert.ToString(row["rule_name"])] = ruleobj;

        //            if (!RepositoryVsReportName.ContainsKey(ReposName))
        //                RepositoryVsReportName.Add(ReposName, new List<string> { ReportName });
        //            if (!RepositoryVsReportName[ReposName].Contains(ReportName))
        //            {
        //                RepositoryVsReportName[ReposName].Add(ReportName);
        //            }

        //        }
        //        foreach (var row in ReportRulesFromExcel.AsEnumerable())
        //        {
        //            string ReportName = string.Empty;
        //            ReportName = Convert.ToString(row["Report Name"]);
        //            string Repository = Convert.ToString(row["Repository Name"]);
        //            string RuleName = Convert.ToString(row["Rule Name"]);
        //            int Priority = Convert.ToInt32(row["Priority"]);
        //            string ruleText = Convert.ToString(row["Rule Text"]);
        //            int ruleTypeId = 0;
        //            string ruleType = Convert.ToString(row["Rule Type"]).Split(' ')[0].ToLower();
        //            switch (ruleType)
        //            {
        //                case "filter":
        //                    ruleTypeId = 2;
        //                    break;
        //                case "transformation":
        //                    ruleTypeId = 4;
        //                    break;
        //            }

        //            if (CompareWith == 1)
        //            {
        //                if (ReportDictionaryDB.ContainsKey(Repository))
        //                {
        //                    if (ReportDictionaryDB[Repository].ContainsKey(ReportName))
        //                    {
        //                        foreach (var innerPair in ReportOrFeedNameVsRuleInfo[ReportName].Values)
        //                        {

        //                            if (innerPair.priority == Priority)
        //                            {
        //                                if (!string.IsNullOrEmpty(ruleText) && innerPair.rule_text != ruleText)
        //                                {
        //                                    RuleInfoToProcess.Rows.Add(innerPair.rule_id, innerPair.rule_set_id, RuleName, innerPair.priority, ruleText, innerPair.created_by, innerPair.created_on, innerPair.rule_state, innerPair.task_in_module, innerPair.rule_dependent_id, innerPair.report_type_id, innerPair.is_additional_leg, innerPair.rule_type_id, false);

        //                                    break;
        //                                }

        //                            }
        //                            else
        //                            {
        //                                if (ReportOrFeedNameVsRuleInfo[ReportName].ContainsKey(RuleName))
        //                                {
        //                                    RuleInfoToProcess.Rows.Add(innerPair.rule_id, innerPair.rule_set_id, RuleName, Priority, ruleText, innerPair.created_by, innerPair.created_on, innerPair.rule_state, innerPair.task_in_module, innerPair.rule_dependent_id, innerPair.report_type_id, innerPair.is_additional_leg, innerPair.rule_type_id, false);
        //                                }
        //                                else
        //                                {
        //                                    DataRow dr = RuleInfoToProcess.NewRow();
        //                                    dr["rule_name"] = RuleName;
        //                                    dr["priority"] = Priority;
        //                                    dr["rule_text"] = ruleText;
        //                                    dr["created_by"] = userName;
        //                                    dr["created_on"] = DateTime.Now;
        //                                    dr["rule_state"] = Convert.ToString(row["Rule State"]);
        //                                    dr["task_in_module"] = innerPair.task_in_module;
        //                                    dr["rule_dependent_id"] = innerPair.rule_dependent_id;
        //                                    dr["report_type_id"] = innerPair.report_type_id;
        //                                    dr["is_additional_leg"] = innerPair.is_additional_leg;
        //                                    dr["rule_type_id"] = ruleTypeId;
        //                                    dr["to_insert"] = true;
        //                                    RuleInfoToProcess.Rows.Add(dr);

        //                                }
        //                            }

        //                        }
        //                    }
        //                }
        //            }
        //            else if (CompareWith == 2)
        //            {
        //                if (ReportDictionaryDB.ContainsKey(Repository))
        //                {
        //                    if (ReportDictionaryDB[Repository].ContainsKey(ReportName))
        //                    {
        //                        if (!ReportOrFeedNameVsRuleInfo[ReportName].ContainsKey(RuleName))
        //                        {
        //                            Dictionary<string, ReportRuleInfo> tempDict = new Dictionary<string, ReportRuleInfo>();

        //                            if (RepositoryVsReportName.ContainsKey(Convert.ToString(row["Repository Name"])))
        //                            {
        //                                if (RepositoryVsReportName[Convert.ToString(row["Repository Name"])].Contains(ReportName))
        //                                {
        //                                    tempDict = ReportOrFeedNameVsRuleInfo[ReportName];
        //                                }
        //                            }

        //                            DataRow dr = RuleInfoToProcess.NewRow();
        //                            dr["rule_name"] = RuleName;
        //                            dr["priority"] = Priority;
        //                            dr["rule_text"] = ruleText;
        //                            dr["created_by"] = userName;
        //                            dr["created_on"] = DateTime.Now;
        //                            dr["rule_state"] = Convert.ToString(row["Rule State"]);
        //                            dr["task_in_module"] = tempDict.First().Value.task_in_module;
        //                            dr["rule_dependent_id"] = tempDict.First().Value.rule_dependent_id;
        //                            dr["report_type_id"] = tempDict.First().Value.report_type_id;
        //                            dr["is_additional_leg"] = tempDict.First().Value.is_additional_leg;
        //                            dr["rule_type_id"] = ruleTypeId;
        //                            dr["to_insert"] = true;
        //                            RuleInfoToProcess.Rows.Add(dr);
        //                        }
        //                    }
        //                }
        //            }

        //            else
        //            {
        //                var info = ReportDictionaryDB[Convert.ToString(row["Repository Name"])][ReportName];
        //                DataRow dr = RuleInfoToProcess.NewRow();
        //                dr["rule_name"] = RuleName;
        //                dr["priority"] = Priority;
        //                dr["rule_text"] = ruleText;
        //                dr["created_by"] = userName;
        //                dr["created_on"] = DateTime.Now;
        //                dr["rule_state"] = Convert.ToString(row["Rule State"]);
        //                dr["task_in_module"] = info.Rows[0]["task_in_module"];
        //                dr["rule_dependent_id"] = info.Rows[0]["rule_dependent_id"];
        //                dr["report_type_id"] = info.Rows[0]["report_type_id"];
        //                dr["is_additional_leg"] = info.Rows[0]["is_additional_leg"];
        //                dr["rule_type_id"] = ruleTypeId;
        //                dr["to_insert"] = true;
        //                RuleInfoToProcess.Rows.Add(dr);

        //            }

        //        }

        //    }
        //    FillCollections();
        //    ProcessSecMReportandUpstreamRules("SecMDB", "SM", RuleInfoToProcess);
        //}


        //private static void FillCollections()
        //{
        //    if (!rule_mapping_details.Columns.Contains("rule_set_id"))
        //        rule_mapping_details.Columns.Add("rule_set_id", typeof(int));
        //    if (!rule_mapping_details.Columns.Contains("rule_id"))
        //        rule_mapping_details.Columns.Add("rule_id", typeof(int));
        //    if (!rule_mapping_details.Columns.Contains("dependent_id"))
        //        rule_mapping_details.Columns.Add("dependent_id", typeof(string));
        //    if (!rule_mapping_details.Columns.Contains("attributes_in_rule"))
        //        rule_mapping_details.Columns.Add("attributes_in_rule", typeof(string));
        //    if (!rule_mapping_details.Columns.Contains("other_info"))
        //        rule_mapping_details.Columns.Add("other_info", typeof(string));
        //    if (!rule_mapping_details.Columns.Contains("is_additional_leg"))
        //        rule_mapping_details.Columns.Add("is_additional_leg", typeof(bool));
        //    if (!rule_mapping_details.Columns.Contains("task_in_module"))
        //        rule_mapping_details.Columns.Add("task_in_module", typeof(string));
        //    if (!rule_mapping_details.Columns.Contains("parent_type_id"))
        //        rule_mapping_details.Columns.Add("parent_type_id", typeof(int));

        //    RDBConnectionManager dbConnectionObj = RDALAbstractFactory.DBFactory.GetConnectionManager("SecMDB");
        //    try
        //    {

        //        reportSetupIdVsReportID = dbConnectionObj.ExecuteQuery(@"SELECT report_id, report_setup_id FROM dbo.ivp_secm_report_setup WHERE is_active = 1", RQueryType.Select).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToInt32(x["report_setup_id"]), y => Convert.ToInt32(y["report_id"]));
        //    }
        //    catch (Exception er)
        //    {
        //        throw er;
        //    }
        //    finally
        //    {
        //        RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnectionObj);
        //    }

        //}


        //private static void FetchDependentAttributes(string connectionId, DataTable rule_mapping_details, int ruleId, int ruleSetId, int columnID, bool is_additional_leg, bool isCorpAction, string moduleType = null)
        //{
        //    string dependingg = null;
        //    Dictionary<int, Dictionary<int, List<string>>> dependingAttributes = null;
        //    var depending = new List<string>();
        //    var lst = new List<string>();
        //    int reportTypeId = 0;

        //    try
        //    {
        //        if (moduleType == REPORTING_RULE_FILTER || moduleType == REPORTING_RULE_TRANSFORMATION)
        //            reportTypeId = reportSetupIdVsReportID[columnID];

        //        var ruleExecutorObject = new RXRuleExecutor() { DBConnectionId = connectionId };

        //        var dependentAttributes = ruleExecutorObject.GetRuleOnColumnForRulesets(new List<int> { ruleSetId });
        //        dependingAttributes = ruleExecutorObject.GetRuleUsedColumnForRulesets(new List<int>() { ruleSetId });

        //        if (moduleType == null)
        //            lst = new List<string> { columnID.ToString() };
        //        else if (dependentAttributes != null && dependentAttributes.ContainsKey(ruleSetId) && dependentAttributes[ruleSetId].ContainsKey(ruleId))
        //            lst = dependentAttributes[ruleSetId][ruleId];

        //        if (dependingAttributes != null && dependingAttributes.ContainsKey(ruleSetId) && dependingAttributes[ruleSetId].ContainsKey(ruleId))
        //        {
        //            depending = dependingAttributes[ruleSetId][ruleId];
        //            if (depending.Count > 0)
        //            {
        //                if (moduleType == UPSTREAM_RULE_FILTER || moduleType == UPSTREAM_RULE_VALIDATION || moduleType == UPSTREAM_RULE_TRANSFORMATION)
        //                    dependingg = string.Join("|", depending.Where(e => !string.IsNullOrEmpty(e) && !lst.Contains(e)).Select(e => e.Split('_')[1]));
        //                else
        //                    dependingg = string.Join("|", depending.Where(e => !string.IsNullOrEmpty(e) && !lst.Contains(e)));
        //            }
        //        }

        //        if ((lst != null && lst.Count > 0) || (depending.Count > 0))
        //        {
        //            if (lst.Count > 0)
        //            {
        //                foreach (var attr in lst)
        //                {
        //                    string id = columnID.ToString();

        //                    if (moduleType != null)
        //                    {

        //                        if (moduleType == UPSTREAM_RULE_FILTER || moduleType == UPSTREAM_RULE_VALIDATION || moduleType == UPSTREAM_RULE_TRANSFORMATION)
        //                        {
        //                            id = attr.Split('_')[1];
        //                        }
        //                        else if (moduleType == REPORTING_RULE_FILTER || moduleType == REPORTING_RULE_TRANSFORMATION)
        //                            id = attr;
        //                    }

        //                    if (string.IsNullOrEmpty(dependingg))
        //                    {
        //                        if (moduleType == REPORTING_RULE_FILTER || moduleType == REPORTING_RULE_TRANSFORMATION)
        //                            rule_mapping_details.Rows.Add(ruleSetId, ruleId, id, DBNull.Value, DBNull.Value, is_additional_leg, "d", reportTypeId);
        //                        else
        //                            rule_mapping_details.Rows.Add(ruleSetId, ruleId, id, DBNull.Value, DBNull.Value, is_additional_leg, DBNull.Value, DBNull.Value);
        //                    }
        //                    else
        //                    {
        //                        if (moduleType == null)
        //                            rule_mapping_details.Rows.Add(ruleSetId, ruleId, id, dependingg, DBNull.Value, is_additional_leg, DBNull.Value, DBNull.Value);
        //                        else if (moduleType == REPORTING_RULE_FILTER || moduleType == REPORTING_RULE_TRANSFORMATION)
        //                            rule_mapping_details.Rows.Add(ruleSetId, ruleId, id, dependingg, DBNull.Value, is_additional_leg, "d", reportTypeId);
        //                        else
        //                            rule_mapping_details.Rows.Add(ruleSetId, ruleId, id, dependingg, DBNull.Value, is_additional_leg, DBNull.Value, DBNull.Value);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (string.IsNullOrEmpty(dependingg))
        //                {
        //                    if (moduleType == REPORTING_RULE_FILTER || moduleType == REPORTING_RULE_TRANSFORMATION)
        //                        rule_mapping_details.Rows.Add(ruleSetId, ruleId, DBNull.Value, DBNull.Value, DBNull.Value, is_additional_leg, "d", reportTypeId);
        //                    else
        //                        rule_mapping_details.Rows.Add(ruleSetId, ruleId, DBNull.Value, DBNull.Value, DBNull.Value, is_additional_leg, DBNull.Value, DBNull.Value);
        //                }
        //                else
        //                {
        //                    if (moduleType == REPORTING_RULE_FILTER || moduleType == REPORTING_RULE_TRANSFORMATION)
        //                        rule_mapping_details.Rows.Add(ruleSetId, ruleId, DBNull.Value, dependingg, DBNull.Value, is_additional_leg, "d", reportTypeId);
        //                    else
        //                        rule_mapping_details.Rows.Add(ruleSetId, ruleId, DBNull.Value, dependingg, DBNull.Value, is_additional_leg, DBNull.Value, DBNull.Value);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (moduleType == null && columnID > 0)
        //                rule_mapping_details.Rows.Add(ruleSetId, ruleId, columnID, DBNull.Value, DBNull.Value, is_additional_leg, DBNull.Value, DBNull.Value);
        //            else
        //            {
        //                if (moduleType == REPORTING_RULE_FILTER || moduleType == REPORTING_RULE_TRANSFORMATION)
        //                    rule_mapping_details.Rows.Add(ruleSetId, ruleId, DBNull.Value, DBNull.Value, DBNull.Value, is_additional_leg, "d", reportTypeId);
        //                else
        //                    rule_mapping_details.Rows.Add(ruleSetId, ruleId, DBNull.Value, DBNull.Value, DBNull.Value, is_additional_leg, DBNull.Value, DBNull.Value);
        //            }
        //        }
        //    }
        //    catch (Exception ee)
        //    {
        //        mLogger.Error("FetchDependentAttributes : RuleSetId : " + ruleSetId + " RuleId : " + ruleId + " Error: " + ee.ToString());
        //        File.AppendAllText(FileName, "FetchDependentAttributes : RuleSetId : " + ruleSetId + " RuleId : " + ruleId + " Error: " + ee.Message + Environment.NewLine);
        //    }
        //}

        private static void DumpRuleMappingDetails(string connectionId, DataTable rule_mapping_details, bool isCorpAction)
        {
            RDBConnectionManager conn = RDALAbstractFactory.DBFactory.GetConnectionManager(connectionId);
            conn.UseTransaction = true;
            conn.IsolationLevel = IsolationLevel.RepeatableRead;

            try
            {
                rule_mapping_details = rule_mapping_details.Copy();
                rule_mapping_details.Columns.Remove("other_info");

                var sb = new StringBuilder();
                foreach (var row in rule_mapping_details.AsEnumerable())
                {
                    sb.Append("INSERT INTO @tab VALUES(").Append(Convert.ToString(row["rule_set_id"])).Append(",").Append(Convert.ToString(row["rule_id"])).Append(");");
                }

                if (rule_mapping_details.Rows.Count > 0)
                {
                    if (isCorpAction)
                    {
                        conn.ExecuteQuery(string.Format(@"DECLARE @tab TABLE(rule_set_id INT, rule_id INT)
                       {0}

                       DELETE c
                       FROM dbo.ivp_cav_rule_mapping_details c
                       INNER JOIN @tab tab ON c.rule_set_id = tab.rule_set_id AND c.rule_id = tab.rule_id", sb.ToString()), RQueryType.Delete);

                        conn.ExecuteBulkCopy("dbo.ivp_cav_rule_mapping_details", rule_mapping_details);
                    }
                    else

                    {
                        conn.ExecuteQuery(string.Format(@"DECLARE @tab TABLE(rule_set_id INT, rule_id INT)
                       {0}

                       DELETE c
                       FROM dbo.ivp_secmv_rule_mapping_details c
                       INNER JOIN @tab tab ON c.rule_set_id = tab.rule_set_id AND c.rule_id = tab.rule_id", sb.ToString()), RQueryType.Delete);

                        conn.ExecuteBulkCopy("dbo.ivp_secmv_rule_mapping_details", rule_mapping_details);
                    }
                }

                conn.CommitTransaction();
            }
            catch (Exception eer)
            {
                conn.RollbackTransaction();
                throw eer;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(conn);
            }
        }



    }

    public class SRMRulesInfo
    {
        public string RuleGroupName { get; set; }
        public string AttributeName { get; set; }
        public string RuleName { get; set; }
        public string Priority { get; set; }
        public string RuleType { get; set; }
        public ObjectRow drow { get; set; }

    }

    public class SRMSpecialColumnNames
    {
        public const string Delta_Action = "Action";
        public const string Remarks = "Remarks";
        public const string Sync_Status = "Status";
    }

    public class SRMMigrationSeparators
    {
        public const string Remarks_Separator = "\n";
        public const string Remarks_Users_Or_Groups_Separator = ",";
        public const string Consolidated_Keys_Separator = "¦";
    }

    public class SRMMigrationStatus
    {
        public const string Passed = "Success";
        public const string Failed = "Failure";
        public const string Not_Processed = "Not Processed";
        public const string Already_In_Sync = "Already in Sync";
    }
    public class SRMCommonMigrationDBController
    {
        public List<CommonMigrationSelectionInfo> RMGetSelectableFeeds(int moduleID, RDBConnectionManager mDBCon = null)
        {
            List<CommonMigrationSelectionInfo> lstSelectableFeeds = new List<CommonMigrationSelectionInfo>();
            CommonMigrationSelectionInfo feed = null;
            DataSet ds = null;

            string query = " EXEC IVPRefMasterVendor.dbo.REFM_GetSelectableFeedsForMigration " + moduleID + " ";

            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, com.ivp.commom.commondal.ConnectionConstants.RefMasterVendor_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    feed = new CommonMigrationSelectionInfo();
                    feed.Text = Convert.ToString(row["Text"]);
                    feed.Value = Convert.ToInt32(row["Value"]);
                    lstSelectableFeeds.Add(feed);
                });
            }

            return lstSelectableFeeds;
        }

        public List<CommonMigrationSelectionInfo> RMGetSelectableEntityTypesForTSTask(int moduleID, RDBConnectionManager mDBCon = null)
        {
            List<CommonMigrationSelectionInfo> lstSelectables = new List<CommonMigrationSelectionInfo>();
            CommonMigrationSelectionInfo entityType = null;
            DataSet ds = null;

            string query = " EXEC IVPRefMasterVendor.dbo.REFM_GetTimeSeriesTaskData '', " + moduleID + " ";

            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, com.ivp.commom.commondal.ConnectionConstants.RefMasterVendor_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    entityType = new CommonMigrationSelectionInfo();
                    entityType.Text = Convert.ToString(row["Entity Type Name"]);
                    entityType.Value = Convert.ToInt32(row["entity_type_id"]);
                    lstSelectables.Add(entityType);
                });
            }

            return lstSelectables;
        }

        public List<CommonMigrationSelectionInfo> SRMGetSelectableDownstreamSyncSystems(RDBConnectionManager mDBCon = null)
        {
            List<CommonMigrationSelectionInfo> lstDownstreamSyncSystems = new List<CommonMigrationSelectionInfo>();
            CommonMigrationSelectionInfo downstreamSyncSystem = null;
            DataSet ds = null;

            string query = @"select setup_id AS [Value], setup_name AS [Text] from  IVPRefMaster.dbo.ivp_srm_dwh_downstream_master WHERE is_active = 1";

            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    downstreamSyncSystem = new CommonMigrationSelectionInfo();
                    downstreamSyncSystem.Text = Convert.ToString(row["Text"]);
                    downstreamSyncSystem.Value = Convert.ToInt32(row["Value"]);
                    lstDownstreamSyncSystems.Add(downstreamSyncSystem);
                });
            }

            return lstDownstreamSyncSystems;
        }
        public List<CommonMigrationSelectionInfo> RMGetSelectableEntityTypes(int moduleId, RDBConnectionManager mDBCon = null)
        {
            List<CommonMigrationSelectionInfo> lstSelectableFeeds = new List<CommonMigrationSelectionInfo>();
            CommonMigrationSelectionInfo entityType = null;
            DataSet ds = null;

            string query = @"SELECT entity_display_name AS [Text], entity_type_id AS [Value] 
                            FROM IVPRefMaster.dbo.ivp_refm_entity_type etyp
                            INNER JOIN IVPRefMaster.dbo.ivp_refm_structure_type sty
                            ON(etyp.structure_type_id = sty.structure_type_id AND sty.is_active = 1 AND sty.is_master = 1)
                            WHERE etyp.is_active = 1 AND module_id = " + moduleId + @" AND derived_from_entity_type_id = 0
                            ORDER BY entity_display_name";

            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, com.ivp.commom.commondal.ConnectionConstants.RefMasterVendor_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    entityType = new CommonMigrationSelectionInfo();
                    entityType.Text = Convert.ToString(row["Text"]);
                    entityType.Value = Convert.ToInt32(row["Value"]);
                    lstSelectableFeeds.Add(entityType);
                });
            }

            return lstSelectableFeeds;
        }
        public List<CommonMigrationSelectionInfo> RMGetSelectableDownstremSystems(RDBConnectionManager mDBCon = null)
        {
            List<CommonMigrationSelectionInfo> lstDownstreamSystems = new List<CommonMigrationSelectionInfo>();
            CommonMigrationSelectionInfo DS_system = null;
            DataSet ds = null;

            string query = @"SELECT report_system_name AS [Text], report_system_id AS [Value] from IVPRefMaster.dbo.ivp_refm_report_system_configuration where is_active = 1";

            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    DS_system = new CommonMigrationSelectionInfo();
                    DS_system.Text = Convert.ToString(row["Text"]);
                    DS_system.Value = Convert.ToInt32(row["Value"]);
                    lstDownstreamSystems.Add(DS_system);
                });
            }

            return lstDownstreamSystems;

        }
        public List<CommonMigrationSelectionInfo> RMGetSelectableTasks(RDBConnectionManager mDBCon = null)
        {
            List<CommonMigrationSelectionInfo> lstTasks = new List<CommonMigrationSelectionInfo>();
            CommonMigrationSelectionInfo DS_task = null;
            DataSet ds = null;

            string query = @"SELECT DISTINCT task_name AS [Text], task_master_id AS [Value] FROM IVPRefMasterVendor.dbo.ivp_refm_task_summary TS
                            INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_task_type TT ON (TT.task_type_id = TS.task_type_id)
                             INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_transport_task_details TTD ON (TTD.transport_master_id = TS.task_master_id AND TTD.is_active = 1)
                             WHERE TT.task_type_id = 1 AND TS.is_active = 1";

            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMasterVendor_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    DS_task = new CommonMigrationSelectionInfo();
                    DS_task.Text = Convert.ToString(row["Text"]);
                    DS_task.Value = Convert.ToInt32(row["Value"]);
                    lstTasks.Add(DS_task);
                });
            }

            return lstTasks;

        }

        public List<CommonMigrationSelectionInfo> RMGetSelectableEntityTypesForPrioritization(int moduleId, RDBConnectionManager mDBCon = null)
        {
            List<CommonMigrationSelectionInfo> lstSelectableFeeds = new List<CommonMigrationSelectionInfo>();
            CommonMigrationSelectionInfo entityType = null;
            DataSet ds = null;

            string query = @"SELECT 
	                            etyp.entity_type_id AS [Value] , etyp.entity_display_name AS [Text]
                                FROM IVPRefMaster.dbo.ivp_refm_entity_type_preference pr
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                                ON (etyp.entity_type_id = pr.entity_type_id AND etyp.is_active = 1 AND module_id = " + moduleId + @" 
                                AND derived_from_entity_type_id = 0)
                                ORDER BY entity_display_name";

            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, com.ivp.commom.commondal.ConnectionConstants.RefMaster_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    entityType = new CommonMigrationSelectionInfo();
                    entityType.Text = Convert.ToString(row["Text"]);
                    entityType.Value = Convert.ToInt32(row["Value"]);
                    lstSelectableFeeds.Add(entityType);
                });
            }

            return lstSelectableFeeds;
        }

        public List<CommonMigrationSelectionInfo> SRMGetSelectableVendorPreferenceNames(RDBConnectionManager mDBCon = null)
        {
            List<CommonMigrationSelectionInfo> lstSelectableVendorPreferenceNames = new List<CommonMigrationSelectionInfo>();
            CommonMigrationSelectionInfo VendorPreferenceNames = null;
            DataSet ds = null;

            string query = @"SELECT vendor_management_name AS [Text], vendor_management_id AS [Value] 
                            FROM IVPRefMasterVendor.dbo.ivp_rad_vendor_management_master
                            WHERE is_active = 1
                            ORDER BY vendor_management_name";

            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, com.ivp.commom.commondal.ConnectionConstants.RefMasterVendor_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    VendorPreferenceNames = new CommonMigrationSelectionInfo();
                    VendorPreferenceNames.Text = Convert.ToString(row["Text"]);
                    VendorPreferenceNames.Value = Convert.ToInt32(row["Value"]);
                    lstSelectableVendorPreferenceNames.Add(VendorPreferenceNames);
                });
            }

            return lstSelectableVendorPreferenceNames;
        }

        public List<CommonMigrationSelectionInfo> RMGetSelectableEntityTypesForRealTimePreference(int moduleId, RDBConnectionManager mDBCon = null)
        {
            List<CommonMigrationSelectionInfo> lstSelectableEntityTypes = new List<CommonMigrationSelectionInfo>();
            CommonMigrationSelectionInfo entityType = null;
            DataSet ds = null;

            string query = @"SELECT DISTINCT etyp.entity_type_id AS [Value],etyp.entity_display_name AS [Text]
                                FROM IVPRefMasterVendor.dbo.ivp_refm_preference_master pm
                                INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_data_source ds
                                ON (pm.data_source_id = ds.data_source_id  AND pm.is_active =1 AND ds.is_active = 1)
                                INNER JOIN IVPRefMasterVendor.dbo.ivp_refm_feed_summary fsum
                                ON (fsum.data_source_id = ds.data_source_id AND fsum.is_active =1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type_feed_mapping fmap
                                ON (fmap.feed_summary_id = fsum.feed_summary_id AND fmap.is_active =1)
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                                ON (fmap.entity_type_id = etyp.entity_type_id AND etyp.is_active = 1 AND etyp.module_id = " + moduleId + @")
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_structure_type sty
								ON (sty.structure_type_id = etyp.structure_type_id AND sty.is_master = 1 AND sty.is_active = 1)";




            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, com.ivp.commom.commondal.ConnectionConstants.RefMasterVendor_Connection);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
            {
                ds.Tables[0].AsEnumerable().ToList().ForEach(row =>
                {
                    entityType = new CommonMigrationSelectionInfo();
                    entityType.Text = Convert.ToString(row["Text"]);
                    entityType.Value = Convert.ToInt32(row["Value"]);
                    lstSelectableEntityTypes.Add(entityType);
                });
            }

            return lstSelectableEntityTypes;
        }

    }
}
