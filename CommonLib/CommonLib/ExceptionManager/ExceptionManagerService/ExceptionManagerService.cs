using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using com.ivp.commom.commondal;
using System.Data;
using com.ivp.rad.common;
using com.ivp.rad.utils;
using com.ivp.refmaster.common;
using System.Threading.Tasks;

namespace com.ivp.common
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CommonExceptionManagerService : RServiceBaseClass, ICommonExceptionManagerService
    {
        //private Dictionary<string, string> exceptionTypesNameMapping = new Dictionary<string, string>();
        CommonExceptionManagerService()
        {
            /*this.exceptionTypesNameMapping.Add("Vendor Mismatch", "Compare And Show");
            this.exceptionTypesNameMapping.Add("Ref Data Missing", "Reference Data Missing");
            this.exceptionTypesNameMapping.Add("No Vendor Value Found", "No Vendor Value Found");
            this.exceptionTypesNameMapping.Add("Value Changed", "Value Changed");
            this.exceptionTypesNameMapping.Add("Show As Exception", "Show As Exception");
            this.exceptionTypesNameMapping.Add("1st Vendor Value Missing", "First Vendor Value Missing");
            this.exceptionTypesNameMapping.Add("Validations", "Custom Exception");
            this.exceptionTypesNameMapping.Add("Invalid Data", "Invalid Data Exception");
            this.exceptionTypesNameMapping.Add("Derived Entity Missing", "Underlyer Missing");
            this.exceptionTypesNameMapping.Add("Alert", "Alerts");
            this.exceptionTypesNameMapping.Add("Duplicates", "Uniqueness Failure");*/
        }

        public List<EMCountResponse> SMGetExceptionCountData(SMExceptionFilterInfo objSMExceptionFilterInfo)
        {
            List<EMCountResponse> resObj = new List<EMCountResponse>();
            try
            {
                DataSet getEMCountDS = EMController.GetExceptionCountDataSM(objSMExceptionFilterInfo);
                DataSet getEMType = EMController.GetSMExceptionTypes();
                foreach (DataRow row in getEMCountDS.Tables[0].Rows)
                {
                    EMCountResponse tempObj = new EMCountResponse();
                    if (Convert.ToString(row["Security Type"]) != "All Security Types")
                        tempObj.TypeId = Convert.ToInt32(row["sectype_id"]);
                    else
                        tempObj.TypeId = -1;

                    tempObj.TypeName = Convert.ToString(row["Security Type"]);
                    int eCount = Convert.ToInt32(row["All Exceptions"]);
                    tempObj.ExceptionsCount = eCount < 10 ? eCount.ToString("00") : eCount.ToString();
                    tempObj.Exceptions = new List<EMExceptions>();

                    //string underlierMissingExceptionTypeColumnName = "Derived Entity Missing";
                    string underlierMissingSecMasterColumnName = "Underlier Missing";

                    foreach (DataColumn col in getEMCountDS.Tables[0].Columns)
                    {
                        if (col.ColumnName != "sectype_id" && col.ColumnName != "Security Type" && col.ColumnName != "exception_type_id" && col.ColumnName != "All Exceptions")
                        {
                            if (getEMCountDS.Tables[0].Rows.Count > 0)
                            {
                                if (col.ColumnName == underlierMissingSecMasterColumnName && Convert.ToInt32(getEMCountDS.Tables[0].Rows[0][underlierMissingSecMasterColumnName]) != 0)
                                {
                                    EMExceptions tempEx = new EMExceptions();
                                    tempEx.ExceptionType = underlierMissingSecMasterColumnName;
                                    tempEx.ExceptionTypeID = string.Join(",", getEMType.Tables[0].AsEnumerable().Where(x => x.Field<string>("exception_type") == underlierMissingSecMasterColumnName).Select(x => x.Field<int>("exception_type_id")).ToList());
                                    int exCount = Convert.ToInt32(row[underlierMissingSecMasterColumnName]);
                                    tempEx.ExceptionCount = exCount < 10 ? exCount.ToString("00") : exCount.ToString();
                                    tempObj.Exceptions.Add(tempEx);
                                }
                                else if (Convert.ToInt32(getEMCountDS.Tables[0].Rows[0][col.ColumnName]) != 0)
                                {
                                    EMExceptions tempEx = new EMExceptions();
                                    tempEx.ExceptionType = col.ColumnName;
                                    tempEx.ExceptionTypeID = string.Join(",", getEMType.Tables[0].AsEnumerable().Where(x => x.Field<string>("exception_type") == col.ColumnName).Select(x => x.Field<int>("exception_type_id")).ToList());
                                    int exCount = Convert.ToInt32(row[col.ColumnName]);
                                    tempEx.ExceptionCount = exCount < 10 ? exCount.ToString("00") : exCount.ToString();
                                    tempObj.Exceptions.Add(tempEx);
                                }
                            }
                        }
                    }
                    resObj.Add(tempObj);
                }
            }
            catch (Exception ex)
            {

            }
            return resObj;
        }

        public List<EMCountResponse> RMGetExceptionCountData(RMExceptionFilterInfo objRMExceptionFilterInfo, int ModuleID = 0)
        {
            List<EMCountResponse> resObj = new List<EMCountResponse>();
            try
            {
                DataSet getEMCountDS = EMController.GetExceptionCountDataRM(objRMExceptionFilterInfo, ModuleID);
                DataSet getEMType = EMController.GetExceptionTypes();
                foreach (DataRow row in getEMCountDS.Tables[0].Rows)
                {
                    EMCountResponse tempObj = new EMCountResponse();
                    if (Convert.ToString(row["Entity Type"]) != "All Entity Types")
                        tempObj.TypeId = Convert.ToInt32(row["entity_type_id"]);
                    else
                        tempObj.TypeId = -1;

                    tempObj.TypeName = Convert.ToString(row["Entity Type"]);
                    int eCount = Convert.ToInt32(row["All Exceptions"]);
                    tempObj.ExceptionsCount = eCount < 10 ? eCount.ToString("00") : eCount.ToString();
                    tempObj.Exceptions = new List<EMExceptions>();
                    foreach (DataColumn col in getEMCountDS.Tables[0].Columns)
                    {
                        if (col.ColumnName != "entity_type_id" && col.ColumnName != "Entity Type" && col.ColumnName != "exception_type_id" && col.ColumnName != "All Exceptions")
                        {
                            if (getEMCountDS.Tables[0].Rows.Count > 0)
                            {
                                if (Convert.ToInt32(getEMCountDS.Tables[0].Rows[0][col.ColumnName]) != 0)
                                {
                                    EMExceptions tempEx = new EMExceptions();
                                    tempEx.ExceptionType = col.ColumnName;
                                    tempEx.ExceptionTypeID = string.Join(",", getEMType.Tables[0].AsEnumerable().Where(x => x.Field<string>("exception_type") == col.ColumnName).Select(x => x.Field<int>("exception_type_id")).ToList());
                                    int exCount = Convert.ToInt32(row[col.ColumnName]);
                                    tempEx.ExceptionCount = exCount < 10 ? exCount.ToString("00") : exCount.ToString();
                                    tempObj.Exceptions.Add(tempEx);
                                }
                            }
                        }
                    }
                    resObj.Add(tempObj);
                }
            }
            catch (Exception ex)
            {

            }
            return resObj;
        }

        public List<EMTags> SMGetExceptionTagsCountData(SMExceptionFilterInfo objSMExceptionFilterInfo)
        {
            List<EMTags> resObj = new List<EMTags>();
            try
            {
                DataSet getEMTagsCountDS = EMController.GetTagsCountSM(objSMExceptionFilterInfo);
                if (getEMTagsCountDS.Tables.Count > 1)
                {
                    foreach (DataRow row in getEMTagsCountDS.Tables[1].Rows)
                    {
                        foreach (DataColumn col in getEMTagsCountDS.Tables[1].Columns)
                        {
                            int tCount = Convert.ToInt32(row[col.ColumnName]);
                            if (tCount != 0)
                            {
                                EMTags tempObj = new EMTags();
                                tempObj.TagName = col.ColumnName;
                                tempObj.TagCount = tCount < 10 ? tCount.ToString("00") : tCount.ToString();
                                resObj.Add(tempObj);
                            }
                            else if (tCount == 0 && col.ColumnName.Equals("All Exceptions"))
                            {
                                EMTags tempObj = new EMTags();
                                tempObj.TagName = col.ColumnName;
                                tempObj.TagCount = "00";
                                resObj.Add(tempObj);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resObj;
        }

        public List<EMTags> RMGetExceptionTagsCountData(RMExceptionFilterInfo objRMExceptionFilterInfo, int ModuleID = 0)
        {
            List<EMTags> resObj = new List<EMTags>();
            try
            {
                DataSet getEMTagsCountDS = EMController.GetTagsCountRM(objRMExceptionFilterInfo, ModuleID);

                foreach (DataRow row in getEMTagsCountDS.Tables[0].Rows)
                {
                    foreach (DataColumn col in getEMTagsCountDS.Tables[0].Columns)
                    {
                        int tCount = Convert.ToInt32(row[col.ColumnName]);
                        if (tCount != 0)
                        {
                            EMTags tempObj = new EMTags();
                            if (col.ColumnName == "All")
                            {
                                tempObj.TagName = "All Exceptions";
                            }
                            else
                            {
                                tempObj.TagName = col.ColumnName;
                            }
                            tempObj.TagCount = tCount < 10 ? tCount.ToString("00") : tCount.ToString();
                            resObj.Add(tempObj);
                        }
                        else if (tCount == 0)
                        {
                            EMTags tempObj = new EMTags();
                            if (col.ColumnName == "All")
                            {
                                tempObj.TagName = "All Exceptions";
                            }
                            else
                            {
                                tempObj.TagName = col.ColumnName;
                            }
                            if(tempObj.TagName == "All Exceptions")
                            {
                                tempObj.TagCount = "00";
                                resObj.Add(tempObj);
                            }                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resObj;
        }

        public List<EMCommonData> GetCommonEMData(SMExceptionFilterInfo objSMExceptionFilterInfo, RMExceptionFilterInfo objRMExceptionFilterInfo,string ModuleID)
        {
            List<EMCommonData> resObj = new List<EMCommonData>();
            List<int> moduleIds = ModuleID.Split(',').Select(x => int.Parse(x)).ToList<int>();
            try
            {
                DataSet getEMSecMasterCountDS = new DataSet(), getEMRefMasterCountDS = new DataSet(), getEMPartyMasterCountDS = new DataSet(), getEMFundMasterCountDS = new DataSet();

                if (moduleIds.Contains(Convert.ToInt32(ExceptionConstantsSM.SEC_MODULE)))
                    getEMSecMasterCountDS = EMController.GetExceptionCountDataSM(objSMExceptionFilterInfo);
                if (moduleIds.Contains(Convert.ToInt32(ExceptionConstantsRM.REFMASTER_MODULE)))
                    getEMRefMasterCountDS = EMController.GetExceptionCountDataRM(objRMExceptionFilterInfo, Convert.ToInt32(ExceptionConstantsRM.REFMASTER_MODULE));
                if (moduleIds.Contains(Convert.ToInt32(ExceptionConstantsRM.PARTY_MODULE)))
                    getEMPartyMasterCountDS = EMController.GetExceptionCountDataRM(objRMExceptionFilterInfo, Convert.ToInt32(ExceptionConstantsRM.PARTY_MODULE));
                if (moduleIds.Contains(Convert.ToInt32(ExceptionConstantsRM.PARTY_MODULE)))
                    getEMFundMasterCountDS = EMController.GetExceptionCountDataRM(objRMExceptionFilterInfo, Convert.ToInt32(ExceptionConstantsRM.FUND_MODULE));

                DataSet getEMType = new DataSet();
                if (moduleIds.Contains(Convert.ToInt32(ExceptionConstantsSM.SEC_MODULE)))
                    getEMType = EMController.GetSMExceptionTypes();
                else
                    getEMType = EMController.GetExceptionTypes();

                int secCount = 0, refCount = 0,partyCount = 0, fundCount = 0;
                DataTable dt = getEMType.Tables[0].DefaultView.ToTable(true, "exception_type");
                foreach (DataRow row in dt.Rows)
                {
                    EMCommonData tempData = new EMCommonData();
                    //DataRow firstSecMRow = null;
                    //DataRow firstRefMRow = null;
                    //if (getEMSecMasterCountDS.Tables[0].Rows.Count > 0)
                    //{
                    //    firstSecMRow = getEMSecMasterCountDS.Tables[0].Rows[0];
                    //}
                    //if (getEMRefMasterCountDS.Tables[0].Rows.Count > 0)
                    //{
                    //    firstRefMRow = getEMRefMasterCountDS.Tables[0].Rows[0];
                    //}

                    tempData.ExceptionType = Convert.ToString(row["exception_type"]);// exceptionTypesNameMapping[Convert.ToString(row["exception_type"])];
                    tempData.ExceptionTypeID = String.Join(",", getEMType.Tables[0].AsEnumerable().Where(x => x.Field<string>("exception_type") == Convert.ToString(row["exception_type"])).Select(x => x.Field<int>("exception_type_id")).ToArray());
                    
                    if (Convert.ToString(row["exception_type"]).Equals("Underlier Missing", StringComparison.OrdinalIgnoreCase))
                    {
                        Int32 secMCount = 0;
                        if (getEMSecMasterCountDS != null && getEMSecMasterCountDS.Tables.Count > 0 && getEMSecMasterCountDS.Tables[0].Rows.Count > 0)
                        //if (firstSecMRow != null)
                        {
                            if (getEMSecMasterCountDS.Tables[0].Columns.Contains("Underlier Missing"))
                                secMCount = Convert.ToInt32(getEMSecMasterCountDS.Tables[0].Rows[0]["Underlier Missing"]);
                            else if(getEMSecMasterCountDS.Tables[0].Columns.Contains("Underlyer Missing"))
                                secMCount = Convert.ToInt32(getEMSecMasterCountDS.Tables[0].Rows[0]["Underlyer Missing"]);
                        }
                        tempData.SecmasterCount = Convert.ToInt32(secMCount) < 10 ? secMCount.ToString("00") : Convert.ToString(secMCount);
                        tempData.RefmasterCount = "00";
                        tempData.PartyCount = "00";
                        tempData.FundCount = "00";
                        //tempData.ExceptionType = "Derived Entity/ Underlier Missing";
                        tempData.ExceptionType = "Underlier Missing";

                        secCount = secCount + secMCount;
                        refCount = refCount + 0;
                        partyCount = partyCount + 0;
                        fundCount = fundCount + 0;
                    }
                    else
                    {
                        Int32 secMCount = 0, refMCount = 0,partyMCount = 0, fundMCount = 0;
                        if (getEMSecMasterCountDS != null && getEMSecMasterCountDS.Tables.Count > 0 && getEMSecMasterCountDS.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToString(row["exception_type"]).Equals("No Vendor Value Found",StringComparison.OrdinalIgnoreCase))
                            {
                                secMCount = Convert.ToInt32(getEMSecMasterCountDS.Tables[0].Rows[0]["No Vendor Value"]);
                            }
                            else if (Convert.ToString(row["exception_type"]) == "Alert")
                            {
                                secMCount = Convert.ToInt32(getEMSecMasterCountDS.Tables[0].Rows[0]["Alerts"]);
                            }
                            else
                            {
                                secMCount = Convert.ToInt32(getEMSecMasterCountDS.Tables[0].Rows[0][Convert.ToString(row["exception_type"])]);
                            }
                        }
                        if (getEMRefMasterCountDS != null && getEMRefMasterCountDS.Tables.Count > 0 && getEMRefMasterCountDS.Tables[0].Rows.Count > 0)
                        {
                            refMCount = Convert.ToInt32(getEMRefMasterCountDS.Tables[0].Rows[0][Convert.ToString(row["exception_type"])]);
                        }

                        if (getEMFundMasterCountDS != null && getEMFundMasterCountDS.Tables.Count > 0 && getEMFundMasterCountDS.Tables[0].Rows.Count > 0)
                        {
                            fundMCount = Convert.ToInt32(getEMFundMasterCountDS.Tables[0].Rows[0][Convert.ToString(row["exception_type"])]);
                        }

                        if (getEMPartyMasterCountDS != null && getEMPartyMasterCountDS.Tables.Count > 0 && getEMPartyMasterCountDS.Tables[0].Rows.Count > 0)
                        {
                            partyMCount = Convert.ToInt32(getEMPartyMasterCountDS.Tables[0].Rows[0][Convert.ToString(row["exception_type"])]);
                        }

                        tempData.SecmasterCount = Convert.ToInt32(secMCount) < 10 ? secMCount.ToString("00") : Convert.ToString(secMCount);
                        tempData.RefmasterCount = Convert.ToInt32(refMCount) < 10 ? refMCount.ToString("00") : Convert.ToString(refMCount);
                        tempData.FundCount = Convert.ToInt32(fundMCount) < 10 ? fundMCount.ToString("00") : Convert.ToString(fundMCount);
                        tempData.PartyCount = Convert.ToInt32(partyMCount) < 10 ? partyMCount.ToString("00") : Convert.ToString(partyMCount);

                        secCount = secCount + secMCount;
                        refCount = refCount + refMCount;
                        partyCount = partyCount + partyMCount;
                        fundCount = fundCount + fundMCount;
                    }
                    resObj.Add(tempData);
                }

                EMCommonData tempData2 = new EMCommonData();
                tempData2.ExceptionType = "All Exceptions";
                tempData2.ExceptionTypeID = "-1";
                tempData2.SecmasterCount = secCount < 10 ? secCount.ToString("00") : Convert.ToString(secCount);
                tempData2.RefmasterCount = refCount < 10 ? refCount.ToString("00") : Convert.ToString(refCount);

                tempData2.FundCount = fundCount < 10 ? fundCount.ToString("00") : Convert.ToString(fundCount);
                tempData2.PartyCount = partyCount < 10 ? partyCount.ToString("00") : Convert.ToString(partyCount);

                resObj.Insert(0, tempData2);
            }
            catch (Exception ex)
            {

            }
            return resObj;
        }

        public List<EMTags> GetCommonEMTags(SMExceptionFilterInfo objSMExceptionFilterInfo, RMExceptionFilterInfo objRMExceptionFilterInfo,string ModuleID)
        {
            List<EMTags> resObj = new List<EMTags>();
            try
            {
                DataSet dsSM = new DataSet(), dsRM = new DataSet(),dsPM = new DataSet(),dsFM = new DataSet();
                List<string> commonCols = new List<string>();

                List<int> moduleIds = ModuleID.Split(',').Select(x => int.Parse(x)).ToList<int>();
                
                if (moduleIds.Contains(Convert.ToInt32(ExceptionConstantsSM.SEC_MODULE)))
                    dsSM = EMController.GetTagsCountSM(objSMExceptionFilterInfo);
                if(moduleIds.Contains(Convert.ToInt32(ExceptionConstantsRM.REFMASTER_MODULE)))
                    dsRM = EMController.GetTagsCountRM(objRMExceptionFilterInfo, Convert.ToInt32(ExceptionConstantsRM.REFMASTER_MODULE));
                if (moduleIds.Contains(Convert.ToInt32(ExceptionConstantsRM.FUND_MODULE)))
                    dsFM = EMController.GetTagsCountRM(objRMExceptionFilterInfo, Convert.ToInt32(ExceptionConstantsRM.FUND_MODULE));
                if (moduleIds.Contains(Convert.ToInt32(ExceptionConstantsRM.PARTY_MODULE)))
                    dsPM = EMController.GetTagsCountRM(objRMExceptionFilterInfo, Convert.ToInt32(ExceptionConstantsRM.PARTY_MODULE));

                if (dsSM != null && dsSM.Tables.Count > 1)
                {
                    foreach (DataColumn col in dsSM.Tables[1].Columns)
                    {
                        EMTags tempObj = new EMTags();

                        int tCount = Convert.ToInt32(dsSM.Tables[1].Rows[0][col.ColumnName]);

                        if (dsRM != null && dsRM.Tables.Count > 0 && dsRM.Tables[0].Columns.Contains(col.ColumnName))
                            tCount += Convert.ToInt32(dsRM.Tables[0].Rows[0][col.ColumnName]);

                        if (dsFM != null && dsFM.Tables.Count > 0 && dsFM.Tables[0].Columns.Contains(col.ColumnName))
                            tCount += Convert.ToInt32(dsFM.Tables[0].Rows[0][col.ColumnName]);

                        if (dsPM != null && dsPM.Tables.Count > 0 && dsPM.Tables[0].Columns.Contains(col.ColumnName))
                            tCount += Convert.ToInt32(dsPM.Tables[0].Rows[0][col.ColumnName]);

                        if (tCount != 0)
                        {
                            tempObj.TagName = col.ColumnName;
                            tempObj.TagCount = tCount < 10 ? tCount.ToString("00") : tCount.ToString();
                            resObj.Add(tempObj);
                            commonCols.Add(col.ColumnName);
                        }
                        else if (tCount == 0 && col.ColumnName.Equals("All Exceptions"))
                        {
                            tempObj.TagName = col.ColumnName;
                            tempObj.TagCount = "00";
                            resObj.Add(tempObj);
                            commonCols.Add(col.ColumnName);
                        }
                    }
                }

                else if (dsRM != null && dsRM.Tables.Count > 0)
                {
                    foreach (DataColumn col in dsRM.Tables[0].Columns)
                    {
                        EMTags tempObj = new EMTags();

                        int tCount = Convert.ToInt32(dsRM.Tables[1].Rows[0][col.ColumnName]);
                        
                        if (dsFM != null && dsFM.Tables.Count > 0 && dsFM.Tables[0].Columns.Contains(col.ColumnName))
                            tCount += Convert.ToInt32(dsFM.Tables[0].Rows[0][col.ColumnName]);

                        if (dsPM != null && dsPM.Tables.Count > 0 && dsPM.Tables[0].Columns.Contains(col.ColumnName))
                            tCount += Convert.ToInt32(dsPM.Tables[0].Rows[0][col.ColumnName]);

                        if (tCount != 0)
                        {
                            tempObj.TagName = col.ColumnName.Equals("All") ? "All Exceptions" : col.ColumnName;
                            tempObj.TagCount = tCount < 10 ? tCount.ToString("00") : tCount.ToString();
                            resObj.Add(tempObj);
                            commonCols.Add(col.ColumnName);
                        }
                        else if (tCount == 0 && col.ColumnName.Equals("All Exceptions"))
                        {
                            tempObj.TagName = col.ColumnName.Equals("All") ? "All Exceptions" : col.ColumnName;
                            tempObj.TagCount = "00";
                            resObj.Add(tempObj);
                            commonCols.Add(col.ColumnName);
                        }
                    }
                }

                else if (dsFM != null && dsFM.Tables.Count > 0)
                {
                    foreach (DataColumn col in dsFM.Tables[0].Columns)
                    {
                        EMTags tempObj = new EMTags();

                        int tCount = Convert.ToInt32(dsFM.Tables[1].Rows[0][col.ColumnName]);

                        if (dsPM != null && dsPM.Tables.Count > 0 && dsPM.Tables[0].Columns.Contains(col.ColumnName))
                            tCount += Convert.ToInt32(dsPM.Tables[0].Rows[0][col.ColumnName]);

                        if (tCount != 0)
                        {
                            tempObj.TagName = col.ColumnName.Equals("All") ? "All Exceptions" : col.ColumnName;
                            tempObj.TagCount = tCount < 10 ? tCount.ToString("00") : tCount.ToString();
                            resObj.Add(tempObj);
                            commonCols.Add(col.ColumnName);
                        }
                        else if (tCount == 0 && col.ColumnName.Equals("All Exceptions"))
                        {
                            tempObj.TagName = col.ColumnName.Equals("All") ? "All Exceptions" : col.ColumnName;
                            tempObj.TagCount = "00";
                            resObj.Add(tempObj);
                            commonCols.Add(col.ColumnName);
                        }
                    }
                }

                else if (dsPM != null && dsPM.Tables.Count > 0)
                {
                    foreach (DataColumn col in dsPM.Tables[0].Columns)
                    {
                        EMTags tempObj = new EMTags();

                        int tCount = Convert.ToInt32(dsPM.Tables[1].Rows[0][col.ColumnName]);
                        
                        if (tCount != 0)
                        {
                            tempObj.TagName = col.ColumnName.Equals("All") ? "All Exceptions" : col.ColumnName;
                            tempObj.TagCount = tCount < 10 ? tCount.ToString("00") : tCount.ToString();
                            resObj.Add(tempObj);
                            commonCols.Add(col.ColumnName);
                        }
                        else if (tCount == 0 && col.ColumnName.Equals("All Exceptions"))
                        {
                            tempObj.TagName = col.ColumnName.Equals("All") ? "All Exceptions" : col.ColumnName;
                            tempObj.TagCount = "00";
                            resObj.Add(tempObj);
                            commonCols.Add(col.ColumnName);
                        }
                    }
                }
                
                if (dsRM != null && dsRM.Tables.Count > 0)
                {
                    foreach (DataColumn col in dsRM.Tables[0].Columns)
                    {
                        if (!commonCols.Contains(col.ColumnName) && (dsRM.Tables[0].Rows.Count > 0))
                        {
                            int tCount = Convert.ToInt32(dsRM.Tables[0].Rows[0][col.ColumnName]);

                            if (dsPM != null && dsPM.Tables.Count > 0 && dsPM.Tables[0].Columns.Contains(col.ColumnName))
                                tCount += Convert.ToInt32(dsPM.Tables[0].Rows[0][col.ColumnName]);

                            if (dsFM != null && dsFM.Tables.Count > 0 && dsFM.Tables[0].Columns.Contains(col.ColumnName))
                                tCount += Convert.ToInt32(dsFM.Tables[0].Rows[0][col.ColumnName]);

                            if (tCount != 0)
                            {
                                EMTags tempObj = new EMTags();
                                tempObj.TagName = col.ColumnName;
                                tempObj.TagCount = tCount < 10 ? tCount.ToString("00") : tCount.ToString();
                                resObj.Add(tempObj);
                            }
                            else if (tCount == 0 && col.ColumnName.Equals("All Exceptions"))
                            {
                                EMTags tempObj = new EMTags();
                                tempObj.TagName = col.ColumnName;
                                tempObj.TagCount = "00";
                                resObj.Add(tempObj);
                            }
                        }
                    }
                }

                else if (dsPM != null && dsPM.Tables.Count > 0)
                {
                    foreach (DataColumn col in dsPM.Tables[0].Columns)
                    {
                        if (!commonCols.Contains(col.ColumnName) && (dsPM.Tables[0].Rows.Count > 0))
                        {
                            int tCount = Convert.ToInt32(dsPM.Tables[0].Rows[0][col.ColumnName]);
                            
                            if (dsFM != null && dsFM.Tables.Count > 0 && dsFM.Tables[0].Columns.Contains(col.ColumnName))
                                tCount += Convert.ToInt32(dsFM.Tables[0].Rows[0][col.ColumnName]);

                            if (tCount != 0)
                            {
                                EMTags tempObj = new EMTags();
                                tempObj.TagName = col.ColumnName;
                                tempObj.TagCount = tCount < 10 ? tCount.ToString("00") : tCount.ToString();
                                resObj.Add(tempObj);
                            }
                            else if (tCount == 0 && col.ColumnName.Equals("All Exceptions"))
                            {
                                EMTags tempObj = new EMTags();
                                tempObj.TagName = col.ColumnName;
                                tempObj.TagCount = "00";
                                resObj.Add(tempObj);
                            }
                        }
                    }
                }

                else if (dsFM != null && dsFM.Tables.Count > 0)
                {
                    foreach (DataColumn col in dsFM.Tables[0].Columns)
                    {
                        if (!commonCols.Contains(col.ColumnName) && (dsFM.Tables[0].Rows.Count > 0))
                        {
                            int tCount = Convert.ToInt32(dsFM.Tables[0].Rows[0][col.ColumnName]);
                            
                            if (tCount != 0)
                            {
                                EMTags tempObj = new EMTags();
                                tempObj.TagName = col.ColumnName;
                                tempObj.TagCount = tCount < 10 ? tCount.ToString("00") : tCount.ToString();
                                resObj.Add(tempObj);
                            }
                            else if (tCount == 0 && col.ColumnName.Equals("All Exceptions"))
                            {
                                EMTags tempObj = new EMTags();
                                tempObj.TagName = col.ColumnName;
                                tempObj.TagCount = "00";
                                resObj.Add(tempObj);
                            }
                        }
                    }
                }
                
                resObj.RemoveAll(x => x.TagName == "All");
            }
            catch (Exception ex)
            {

            }
            return resObj;
        }

        public List<ListItems> GetAllSectypes(HashSet<string> selectedSectypes, string username)
        {
            List<ListItems> lstObj = new List<ListItems>();
            DataTable dt = EMController.GetAllSecTypeNames(username).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ListItems temp = new ListItems();
                    string value = dr.Field<int>("sectype_id").ToString();
                    string text = dr.Field<string>("sectype_name").ToString();
                    temp.value = value;
                    temp.text = text;
                    if (selectedSectypes != null && selectedSectypes.Contains(value))
                    {
                        temp.isSelected = "true";
                    }
                    else
                    {
                        temp.isSelected = "false";
                    }
                    lstObj.Add(temp);
                }
            }
            return lstObj;
        }

        public List<ListItems> GetAllEntityTypes(HashSet<string> selectedEntityTypes, string userName, string ModuleID = "0")
        {
            List<ListItems> lstObj = new List<ListItems>();
            List<int> moduleIds = ModuleID.Split(',').Select(x => int.Parse(x)).ToList<int>();
            foreach (int module in moduleIds)
            {
                DataTable dt = EMController.GetAllEntityTypes(userName, module).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {                        
                        string value = Convert.ToString(dr["entity_type_id"]);
                        string text = Convert.ToString(dr["entity_display_name"]);
                        if(!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(text))
                        {
                            ListItems temp = new ListItems();
                            temp.value = value;
                            temp.text = text;
                            if (selectedEntityTypes != null && selectedEntityTypes.Contains(value))
                            {
                                temp.isSelected = "true";
                            }
                            else
                            {
                                temp.isSelected = "false";
                            }
                            lstObj.Add(temp);
                        }                        
                    }
                }
            }            
            return lstObj;
        }

        public List<ListItems> GetAllAttributesBasedOnSectypeSelection(string secTypeIds, string userName, HashSet<string> selectedAttributes)
        {
            List<ListItems> lstObj = new List<ListItems>();
            DataSet ds = EMController.GetAttributeBasedOnSecTypeSelection(secTypeIds, userName);
            DataTable dt = ds.Tables[0];
            DataView dv = dt.DefaultView;
            dv.Sort = "display_name ASC";
            dt = dv.ToTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ListItems temp = new ListItems();
                    string value = dr.Field<int>("attribute_id").ToString();
                    string text = dr.Field<string>("display_text").ToString();
                    temp.value = value;
                    temp.text = text;
                    if(selectedAttributes != null)
                    {
                        if (selectedAttributes.Contains(value))
                        {
                            temp.isSelected = "true";
                        }
                        else
                        {
                            temp.isSelected = "false";
                        }
                    }
                    else
                        temp.isSelected = "true";

                    lstObj.Add(temp);
                }
            }
            return lstObj;
        }

        public List<ListItems> GetAllAttributesBasedOnEntityTypeSelection(string entityTypeIds, HashSet<string> selectedAttributes)
        {
            List<ListItems> lstObj = new List<ListItems>();
            DataSet ds = EMController.GetAttributeBasedOnEntityTypeSelection(entityTypeIds);
            DataTable dt = ds.Tables[0];
            DataView dv = dt.DefaultView;
            dv.Sort = "Attribute Display Name ASC";
            dt = dv.ToTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ListItems temp = new ListItems();
                    string value = dr.Field<int>("Attribute Id").ToString();
                    string text = dr.Field<string>("Attribute Display Name").ToString();
                    temp.value = value;
                    temp.text = text;
                    if (selectedAttributes != null)
                    {
                        if (selectedAttributes != null && selectedAttributes.Contains(value))
                        {
                            temp.isSelected = "true";
                        }
                        else
                        {
                            temp.isSelected = "false";
                        }
                    }
                    else
                        temp.isSelected = "true";

                    lstObj.Add(temp);
                }
            }
            return lstObj;
        }
    }

    internal static class EMController
    {
        #region private member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("EMController");
        #endregion

        internal static DataSet GetExceptionCountDataSM(SMExceptionFilterInfo objSMExceptionFilterInfo)
        {
            mLogger.Debug("EMController : Start -> GetExceptionsCountSM");
            RHashlist mHList = new RHashlist();
            try
            {
                if (objSMExceptionFilterInfo.ListSecurityType != null && objSMExceptionFilterInfo.ListSecurityType.Count > 0)
                    mHList.Add(ExceptionConstantsSM.LIST_SECURITY_TYPE, string.Join(",", objSMExceptionFilterInfo.ListSecurityType));
                else
                    mHList.Add(ExceptionConstantsSM.LIST_SECURITY_TYPE, ExceptionConstantsSM.ALL);

                if (objSMExceptionFilterInfo.ListAttribute != null && objSMExceptionFilterInfo.ListAttribute.Count > 0)
                    mHList.Add(ExceptionConstantsSM.LIST_ATTRIBUTES, string.Join(",", objSMExceptionFilterInfo.ListAttribute));
                else
                    mHList.Add(ExceptionConstantsSM.LIST_ATTRIBUTES, ExceptionConstantsSM.ALL);

                if (objSMExceptionFilterInfo.ListSecurityId != null && objSMExceptionFilterInfo.ListSecurityId.Count > 0)
                    mHList.Add(ExceptionConstantsSM.LIST_SECURITY_ID, string.Join(",", objSMExceptionFilterInfo.ListSecurityId));
                else
                    mHList.Add(ExceptionConstantsSM.LIST_SECURITY_ID, ExceptionConstantsSM.ALL);

                if (objSMExceptionFilterInfo.ListExceptionType != null && objSMExceptionFilterInfo.ListExceptionType.Count > 0)
                    mHList.Add(ExceptionConstantsSM.LIST_EXCEPTION_TYPE, string.Join(",", objSMExceptionFilterInfo.ListExceptionType));
                else
                    mHList.Add(ExceptionConstantsSM.LIST_EXCEPTION_TYPE, ExceptionConstantsSM.ALL);

                if (objSMExceptionFilterInfo.ListSectypeTableId != null && objSMExceptionFilterInfo.ListSectypeTableId.Count > 0)
                    mHList.Add(ExceptionConstantsSM.LIST_SECTYPE_TABLE_ID, string.Join(",", objSMExceptionFilterInfo.ListSectypeTableId));
                else
                    mHList.Add(ExceptionConstantsSM.LIST_SECTYPE_TABLE_ID, ExceptionConstantsSM.ALL);

                if (!String.IsNullOrEmpty(objSMExceptionFilterInfo.StartDate))
                    mHList.Add(ExceptionConstantsSM.START_DATE, Convert.ToDateTime(objSMExceptionFilterInfo.StartDate));
                else
                    mHList.Add(ExceptionConstantsSM.START_DATE, null);

                if (!String.IsNullOrEmpty(objSMExceptionFilterInfo.EndDate))
                    mHList.Add(ExceptionConstantsSM.END_DATE, Convert.ToDateTime(objSMExceptionFilterInfo.EndDate));
                else
                    mHList.Add(ExceptionConstantsSM.END_DATE, null);

                mHList.Add(ExceptionConstantsSM.USERNAME, objSMExceptionFilterInfo.UserName);
                mHList.Add(ExceptionConstantsSM.IS_SUPPRESSED, objSMExceptionFilterInfo.Suppressed);
                mHList.Add(ExceptionConstantsSM.IS_RESOLVED, objSMExceptionFilterInfo.Resolved);
                mHList.Add(ExceptionConstantsSM.TAG_TYPE_NAME_LIST, objSMExceptionFilterInfo.TagName);

                return (DataSet)CommonDALWrapper.ExecuteProcedure(ExceptionManagerConstants.GetExceptionsCountSM, mHList, ConnectionConstants.SecMaster_Connection)[0];
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("EMController : End -> GetExceptionsCountSM");
            }
        }

        internal static DataSet GetExceptionCountDataRM(RMExceptionFilterInfo objRMExceptionFilterInfo, int ModuleID = 0)
        {
            mLogger.Debug("EMController : Start -> GetExceptionsCountRM");
            RHashlist mHList = new RHashlist();
            try
            {
                if (objRMExceptionFilterInfo.ListEntityType != null && objRMExceptionFilterInfo.ListEntityType.Count > 0)
                    mHList.Add(ExceptionConstantsRM.LIST_ENTITY_TYPE, string.Join(",", objRMExceptionFilterInfo.ListEntityType));
                else
                    mHList.Add(ExceptionConstantsRM.LIST_ENTITY_TYPE, ExceptionConstantsRM.ALL);

                if (objRMExceptionFilterInfo.ListAttribute != null && objRMExceptionFilterInfo.ListAttribute.Count > 0)
                    mHList.Add(ExceptionConstantsRM.LIST_ATTRIBUTES, string.Join(",", objRMExceptionFilterInfo.ListAttribute));
                else
                    mHList.Add(ExceptionConstantsRM.LIST_ATTRIBUTES, ExceptionConstantsRM.ALL);

                if (objRMExceptionFilterInfo.ListEntityCode != null && objRMExceptionFilterInfo.ListEntityCode.Count > 0)
                    mHList.Add(ExceptionConstantsRM.LIST_ENTITY_CODE, string.Join(",", objRMExceptionFilterInfo.ListEntityCode));
                else
                    mHList.Add(ExceptionConstantsRM.LIST_ENTITY_CODE, ExceptionConstantsRM.ALL);

                if (objRMExceptionFilterInfo.ListExceptionType != null && objRMExceptionFilterInfo.ListExceptionType.Count > 0)
                    mHList.Add(ExceptionConstantsRM.LIST_EXCEPTION_TYPE, string.Join(",", objRMExceptionFilterInfo.ListExceptionType));
                else
                    mHList.Add(ExceptionConstantsRM.LIST_EXCEPTION_TYPE, ExceptionConstantsRM.ALL);

                if (!String.IsNullOrEmpty(objRMExceptionFilterInfo.StartDate))
                    mHList.Add(ExceptionConstantsRM.START_DATE, objRMExceptionFilterInfo.StartDate);
                else
                    mHList.Add(ExceptionConstantsRM.START_DATE, null);

                if (!String.IsNullOrEmpty(objRMExceptionFilterInfo.EndDate))
                    mHList.Add(ExceptionConstantsRM.END_DATE, objRMExceptionFilterInfo.EndDate);
                else
                    mHList.Add(ExceptionConstantsSM.END_DATE, null);

                mHList.Add(ExceptionConstantsRM.USERNAME, objRMExceptionFilterInfo.UserName);
                mHList.Add(ExceptionConstantsRM.IS_SUPPRESSED, objRMExceptionFilterInfo.Suppressed);
                mHList.Add(ExceptionConstantsRM.IS_RESOLVED, objRMExceptionFilterInfo.Resolved);
                mHList.Add(ExceptionConstantsRM.TAG_TYPE_NAME_LIST, objRMExceptionFilterInfo.TagName);
                if (objRMExceptionFilterInfo.ListLegs != null && objRMExceptionFilterInfo.ListLegs.Count > 0)
                    mHList.Add(ExceptionConstantsRM.LIST_LEGS, string.Join(",", objRMExceptionFilterInfo.ListLegs));
                else
                    mHList.Add(ExceptionConstantsRM.LIST_LEGS, ExceptionConstantsRM.ALL);

                mHList.Add(ExceptionConstantsRM.VIEW_USER_SPECIFIC, "false");

                //Added moduleID
                mHList.Add(ExceptionConstantsRM.MODULEID, ModuleID);

                return (DataSet)CommonDALWrapper.ExecuteProcedure(ExceptionManagerConstants.GetExceptionsCountRM, mHList,
                    ConnectionConstants.RefMaster_Connection)[0];
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("EMController : End -> GetExceptionsCountRM");
            }
        }

        internal static DataSet GetTagsCountSM(SMExceptionFilterInfo objSMExceptionFilterInfo)
        {
            mLogger.Debug("ExceptionController : Start -> GetTagsCount");
            RHashlist mHList = new RHashlist();
            try
            {
                if (objSMExceptionFilterInfo.ListSecurityType != null && objSMExceptionFilterInfo.ListSecurityType.Count > 0)
                    mHList.Add(ExceptionConstantsSM.LIST_SECURITY_TYPE, string.Join(",", objSMExceptionFilterInfo.ListSecurityType));
                else
                    mHList.Add(ExceptionConstantsSM.LIST_SECURITY_TYPE, ExceptionConstantsSM.ALL);

                if (objSMExceptionFilterInfo.ListAttribute != null && objSMExceptionFilterInfo.ListAttribute.Count > 0)
                    mHList.Add(ExceptionConstantsSM.LIST_ATTRIBUTES, string.Join(",", objSMExceptionFilterInfo.ListAttribute));
                else
                    mHList.Add(ExceptionConstantsSM.LIST_ATTRIBUTES, ExceptionConstantsSM.ALL);

                if (objSMExceptionFilterInfo.ListSecurityId != null && objSMExceptionFilterInfo.ListSecurityId.Count > 0)
                    mHList.Add(ExceptionConstantsSM.LIST_SECURITY_ID, string.Join(",", objSMExceptionFilterInfo.ListSecurityId));
                else
                    mHList.Add(ExceptionConstantsSM.LIST_SECURITY_ID, ExceptionConstantsSM.ALL);

                if (objSMExceptionFilterInfo.ListExceptionType != null && objSMExceptionFilterInfo.ListExceptionType.Count > 0)
                    mHList.Add(ExceptionConstantsSM.LIST_EXCEPTION_TYPE, string.Join(",", objSMExceptionFilterInfo.ListExceptionType));
                else
                    mHList.Add(ExceptionConstantsSM.LIST_EXCEPTION_TYPE, ExceptionConstantsSM.ALL);

                if (objSMExceptionFilterInfo.ListSectypeTableId != null && objSMExceptionFilterInfo.ListSectypeTableId.Count > 0)
                    mHList.Add(ExceptionConstantsSM.LIST_SECTYPE_TABLE_ID, string.Join(",", objSMExceptionFilterInfo.ListSectypeTableId));
                else
                    mHList.Add(ExceptionConstantsSM.LIST_SECTYPE_TABLE_ID, ExceptionConstantsSM.ALL);

                if (!String.IsNullOrEmpty(objSMExceptionFilterInfo.StartDate))
                    mHList.Add(ExceptionConstantsSM.START_DATE, Convert.ToDateTime(objSMExceptionFilterInfo.StartDate));
                else
                    mHList.Add(ExceptionConstantsSM.START_DATE, null);

                if (!String.IsNullOrEmpty(objSMExceptionFilterInfo.EndDate))
                    mHList.Add(ExceptionConstantsSM.END_DATE, Convert.ToDateTime(objSMExceptionFilterInfo.EndDate));
                else
                    mHList.Add(ExceptionConstantsSM.END_DATE, null);

                mHList.Add(ExceptionConstantsSM.USERNAME, objSMExceptionFilterInfo.UserName);
                mHList.Add(ExceptionConstantsSM.IS_SUPPRESSED, objSMExceptionFilterInfo.Suppressed);
                mHList.Add(ExceptionConstantsSM.IS_RESOLVED, objSMExceptionFilterInfo.Resolved);
                mHList.Add(ExceptionConstantsSM.TAG_TYPE_NAME_LIST, objSMExceptionFilterInfo.TagName);

                return (DataSet)CommonDALWrapper.ExecuteProcedure(ExceptionManagerConstants.GetExceptionsCountSM, mHList,
                    ConnectionConstants.SecMaster_Connection)[0];
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("ExceptionController : End -> GetExceptionsCount");
            }
        }

        internal static DataSet GetTagsCountRM(RMExceptionFilterInfo objRMExceptionFilterInfo, int ModuleID = 0)
        {
            mLogger.Debug("ExceptionController : Start->GetTagsCountRM");
            RHashlist mHList = new RHashlist();
            try
            {
                if (objRMExceptionFilterInfo.ListEntityType != null && objRMExceptionFilterInfo.ListEntityType.Count > 0)
                    mHList.Add(ExceptionConstantsRM.LIST_ENTITY_TYPE, string.Join(",", objRMExceptionFilterInfo.ListEntityType));
                else
                    mHList.Add(ExceptionConstantsRM.LIST_ENTITY_TYPE, ExceptionConstantsRM.ALL);

                if (objRMExceptionFilterInfo.ListAttribute != null && objRMExceptionFilterInfo.ListAttribute.Count > 0)
                    mHList.Add(ExceptionConstantsRM.LIST_ATTRIBUTES, string.Join(",", objRMExceptionFilterInfo.ListAttribute));
                else
                    mHList.Add(ExceptionConstantsRM.LIST_ATTRIBUTES, ExceptionConstantsRM.ALL);

                if (objRMExceptionFilterInfo.ListEntityCode != null && objRMExceptionFilterInfo.ListEntityCode.Count > 0)
                    mHList.Add(ExceptionConstantsRM.LIST_ENTITY_CODE, string.Join(",", objRMExceptionFilterInfo.ListEntityCode));
                else
                    mHList.Add(ExceptionConstantsRM.LIST_ENTITY_CODE, ExceptionConstantsRM.ALL);

                if (objRMExceptionFilterInfo.ListExceptionType != null && objRMExceptionFilterInfo.ListExceptionType.Count > 0)
                    mHList.Add(ExceptionConstantsRM.LIST_EXCEPTION_TYPE, string.Join(",", objRMExceptionFilterInfo.ListExceptionType));
                else
                    mHList.Add(ExceptionConstantsRM.LIST_EXCEPTION_TYPE, ExceptionConstantsRM.ALL);

                if (objRMExceptionFilterInfo.ListLegs != null && objRMExceptionFilterInfo.ListLegs.Count > 0)
                    mHList.Add(ExceptionConstantsRM.LIST_LEGS, string.Join(",", objRMExceptionFilterInfo.ListLegs));
                else
                    mHList.Add(ExceptionConstantsRM.LIST_LEGS, ExceptionConstantsRM.ALL);

                if (!String.IsNullOrEmpty(objRMExceptionFilterInfo.StartDate))
                    mHList.Add(ExceptionConstantsRM.START_DATE, objRMExceptionFilterInfo.StartDate);
                else
                    mHList.Add(ExceptionConstantsSM.START_DATE, null);

                if (!String.IsNullOrEmpty(objRMExceptionFilterInfo.EndDate))
                    mHList.Add(ExceptionConstantsRM.END_DATE, objRMExceptionFilterInfo.EndDate);
                else
                    mHList.Add(ExceptionConstantsSM.END_DATE, null);

                mHList.Add(ExceptionConstantsRM.USERNAME, objRMExceptionFilterInfo.UserName);
                mHList.Add(ExceptionConstantsRM.IS_SUPPRESSED, objRMExceptionFilterInfo.Suppressed);
                mHList.Add(ExceptionConstantsRM.IS_RESOLVED, objRMExceptionFilterInfo.Resolved);
                mHList.Add(ExceptionConstantsRM.TAG_TYPE_NAME_LIST, objRMExceptionFilterInfo.TagName);

                //Added moduleID
                mHList.Add(ExceptionConstantsRM.MODULEID, ModuleID);

                var result = CommonDALWrapper.ExecuteProcedure(ExceptionManagerConstants.GetTagCountRM, mHList,
                    ConnectionConstants.RefMaster_Connection);

                mLogger.Debug("ExceptionController : End -> GetTagsCountRM - > 123");

                if (result.Count == 1)
                {
                    mLogger.Debug("ExceptionController : GetTagsCountRM -> Data Returned");
                    return (DataSet)result[0];
                }

                mLogger.Debug("ExceptionController : GetTagsCountRM -> null Data Returned");
                return null;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("ExceptionController : End -> GetTagsCountRM");
            }
        }

        internal static DataSet GetExceptionTypes()
        {
            mLogger.Debug("ExceptionController : Start -> GetExceptionTypes");
            RHashlist mHList = new RHashlist();
            try
            {
                return (DataSet)CommonDALWrapper.ExecuteSelectQuery("SELECT exception_type_id, exception_type FROM [IVPRefMaster].[dbo].[ivp_refm_exception_types]", ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("ExceptionController : End -> GetExceptionTypes");
            }
        }

        internal static DataSet GetSMExceptionTypes()
        {
            mLogger.Debug("ExceptionController : Start -> GetExceptionTypes");
            RHashlist mHList = new RHashlist();
            try
            {
                return (DataSet)CommonDALWrapper.ExecuteSelectQuery("SELECT exception_type_id, exception_type FROM [IVPSecMaster].[dbo].[ivp_secm_exception_types]", ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("ExceptionController : End -> GetExceptionTypes");
            }
        }

        internal static DataSet GetAllSecTypeNames(string username)
        {
            mLogger.Debug("ExceptionController : Start -> GetAllSecTypeNames");
            RHashlist mHList = new RHashlist();
            try
            {
                return (DataSet)CommonDALWrapper.ExecuteSelectQuery("SELECT sectype_id, sectype_name FROM IVPSecMaster.dbo.SECM_GetUserSectypes('" + username + "')",
                    ConnectionConstants.SecMaster_Connection);
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("ExceptionController : End -> GetAllSecTypeNames");
            }
        }

        internal static DataSet GetAllEntityTypes(string userName, int moduleId = 0)
        {
            mLogger.Debug("ExceptionController : Start -> GetAllEntityTypes");
            RHashlist mHList = new RHashlist();
            try
            {
//                return (DataSet)CommonDALWrapper.ExecuteSelectQuery(
//                    @"select entity_type_id, entity_display_name 
//                    from [IVPRefMaster].[dbo].[REFM_GetUserEntityTypes] ('" + userName + @"') 
//                    order by entity_display_name",
//                    ConnectionConstants.RefMaster_Connection
//                );
                return (DataSet)CommonDALWrapper.ExecuteSelectQuery(
                    @"select 
                        et.entity_type_id, 
                        CASE WHEN et.structure_type_id IN(2,4) THEN et.entity_display_name ELSE etMaster.entity_code + '-' + et.entity_display_name END as 'entity_display_name'
                    from [IVPRefMaster].[dbo].[REFM_GetUserEntityTypes] ('" + userName + @"', " + moduleId + @") userEt
                    join ivp_refm_entity_type et
                    on userEt.entity_type_id = et.entity_type_id
                    left join ivp_refm_entity_type etMaster
                    on et.derived_from_entity_type_id = etMaster.entity_type_id
                    where et.is_active = 1
                    order by et.entity_display_name",
                    ConnectionConstants.RefMaster_Connection
                );
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("ExceptionController : End -> GetAllEntityTypes");
            }
        }

        internal static DataSet GetAttributeBasedOnSecTypeSelection(string secTypeIds, string userName)
        {
            mLogger.Debug("ExceptionController : Start -> GetAttributeBasedOnSecTypeSelection");
            RHashlist mHList = new RHashlist();
            try
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery("exec [SECM_SearchIntellisense] @sectype_ids='" + secTypeIds + "',@attribute_id=0,@search_text='',@is_attribute_search=1,@user_name='" + userName + "',@no_of_record_required=-1", ConnectionConstants.SecMaster_Connection);
                return ds;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("ExceptionController : End -> GetAttributeBasedOnSecTypeSelection");
            }
        }

        internal static DataSet GetAttributeBasedOnEntityTypeSelection(string entityTypeIds)
        {
            mLogger.Debug("ExceptionController : Start -> GetAttributeBasedOnEntityTypeSelection");
            RHashlist mHList = new RHashlist();
            try
            {
                DataSet tempDs = RMCommonUtils.GetAttributeByMultipleEntityTypeId(entityTypeIds);
                return tempDs;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("ExceptionController : End -> GetAttributeBasedOnEntityTypeSelection");
            }
        }
    }

    internal static class ExceptionManagerConstants
    {
        internal const string GetExceptionDetails = "SECM:GetExceptionDetails";
        internal const string GetExceptionsCountSM = "SECM:SECM_GetExceptionsCount";
        internal const string GetExceptionsCountRM = "REFM:GetExceptionsCount";
        internal const string GetTagCountRM = "REFM:REFM_GetTagsCount";
        internal const string GetTagsCount = "SECM:SECM_GetTagsCount";
        internal const string GetExceptionTypes = "SECM:Secm_GetExceptionTypeNames";
        internal const string GetSectypes = "";
    }

    internal class ExceptionConstantsSM
    {
        internal const string LIST_SECURITY_TYPE = "sectype_id_list";
        internal const string LIST_SECTYPE_TABLE_ID = "sectype_table_id_list";
        internal const string LIST_ATTRIBUTES = "attribute_id_list";
        internal const string LIST_SECURITY_ID = "sec_id_list";
        internal const string LIST_EXCEPTION_TYPE = "exception_type_id_list";
        internal const string EXCEPTION_XML = "exception_xml";
        internal const string REMARKS = "remarks";
        internal const string START_DATE = "start_date";
        internal const string END_DATE = "end_date";
        internal const string USERNAME = "user_name";
        internal const string IS_SUPPRESSED = "is_suppressed";
        internal const string IS_RESOLVED = "is_resolved";
        internal const string MAX_RESULTS = "max_results";
        internal const string ALL = "-1";
        internal const string ATTRIBUTE_XML = "attribute_xml";
        internal const string BASKET_ATTRIBUTE_XML = "basket_attribute_xml";
        internal const string ACTION_ID = "action_id";
        internal const string INPUT_XML = "input_xml";
        internal const string VENDOR_ID = "vendor_id";
        internal const string IS_USER_VALUE = "is_user_value";
        internal const string USER_VALUE = "user_value";
        internal const string PAGE_ID = "page_id";
        internal const string FUNCTIONALITY_ID = "functionality_id";
        internal const string TAG_TYPE_NAME_LIST = "tag_type_name_list";
        public const string SEC_MODULE = "3";
    }

    public class ExceptionConstantsRM
    {
        public const string LIST_ENTITY_TYPE = "entity_type_id_list";
        public const string LIST_LEGS = "leg_entity_type_id_list";
        public const string LIST_ATTRIBUTES = "attribute_id_list";
        public const string LIST_ENTITY_CODE = "entity_code_list";
        public const string LIST_EXCEPTION_TYPE = "exception_type_id_list";
        public const string EXCEPTION_XML = "exception_xml";
        public const string REMARKS = "remarks";
        public const string START_DATE = "start_date";
        public const string END_DATE = "end_date";
        public const string USERNAME = "user_name";
        public const string MODULEID = "ModuleId";                                     //Added moduleID
        public const string IS_SUPPRESSED = "is_suppressed";
        public const string IS_RESOLVED = "is_resolved";
        public const string MAX_RESULTS = "max_results";
        public const string ALL = "-1";
        public const string ATTRIBUTE_XML = "attribute_xml";
        public const string BASKET_ATTRIBUTE_XML = "basket_attribute_xml";
        public const string ACTION_ID = "action_id";
        public const string INPUT_XML = "input_xml";
        public const string VENDOR_ID = "vendor_id";
        public const string IS_USER_VALUE = "is_user_value";
        public const string USER_VALUE = "user_value";
        public const string PAGE_ID = "page_id";
        public const string FUNCTIONALITY_ID = "functionality_id";
        public const string TAG_TYPE_NAME_LIST = "tag_type_name_list";
        public const string VIEW_USER_SPECIFIC = "viewUserSpecific";
        public const string REFMASTER_MODULE = "6";
        public const string FUND_MODULE = "18";
        public const string PARTY_MODULE = "20";
    }
}
