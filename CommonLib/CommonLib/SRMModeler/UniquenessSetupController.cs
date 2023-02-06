using com.ivp.commom.commondal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.rad.common;
using com.ivp.common;
using System.Xml.Linq;
using System.Reflection;
using com.ivp.rad.data;

namespace SRMModelerController
{
    public class UniquenessSetupController
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("UniquenessSetupController");
        public List<SectypeInfo> UniquenessSetupGetAllSectypes()
        {
            mLogger.Debug("CommonService -> UniquenessSetupGetAllSectypes -> Start");

            List<SectypeInfo> result = new List<SectypeInfo>();

            try
            {
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT sectype_id, sectype_name FROM IVPSecMaster.dbo.SECM_GetUserSectypes('admin') ORDER BY sectype_name", ConnectionConstants.SecMaster_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {

                    int length = ds.Tables[0].Rows.Count;

                    for (int i = 0; i < length; i++)
                    {
                        SectypeInfo item = new SectypeInfo();
                        item.SectypeID = Convert.ToInt32(ds.Tables[0].Rows[i][0]);
                        item.SectypeName = Convert.ToString(ds.Tables[0].Rows[i][1]);

                        result.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> UniquenessSetupGetAllSectypes -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> UniquenessSetupGetAllSectypes -> End");
            }

            return result;
        }

        public List<UniquenessSetupKeyInfo> GetUniqueKeysForSelectedSectypes(List<int> selectedSectypes)
        {
            mLogger.Debug("CommonService -> GetUniqueKeysForSelectedSectypes -> Start");

            List<UniquenessSetupKeyInfo> result = new List<UniquenessSetupKeyInfo>();

            try
            {
                string selectedSectypesString = "";

                if (selectedSectypes.Count > 0)
                {
                    selectedSectypesString = Convert.ToString(selectedSectypes[0]);

                    if (selectedSectypes.Count > 1)
                    {
                        for (int i = 1; i < selectedSectypes.Count; i++)
                            selectedSectypesString += ("," + Convert.ToString(selectedSectypes[i]));
                    }
                }

                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_GetUniqueKeyData '{0}'", selectedSectypesString), ConnectionConstants.SecMaster_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    result = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Key_Name = x.Field<string>("Key_Name"), Is_Master = x.Field<bool>("Is_Master"), Is_Across_Securities = x.Field<bool>("Is_Across_Securities"), Check_In_Drafts = x.Field<bool>("Check_In_Drafts"), Check_In_Workflows = x.Field<bool>("Check_In_Workflows"), Null_As_Unique = x.Field<bool>("Null_As_Unique") }).Select(x => new UniquenessSetupKeyInfo() { KeyID = Convert.ToInt32(x.Key.Key_ID), KeyName = Convert.ToString(x.Key.Key_Name), IsMaster = Convert.ToBoolean(x.Key.Is_Master), IsAcrossSecurities = Convert.ToBoolean(x.Key.Is_Across_Securities), CheckInDrafts = Convert.ToBoolean(x.Key.Check_In_Drafts), CheckInWorkflows = Convert.ToBoolean(x.Key.Check_In_Workflows), NullAsUnique = Convert.ToBoolean(x.Key.Null_As_Unique), SelectedSectypes = x.Select(y => Convert.ToInt32(y["Sectype_ID"])).ToList() }).ToList();
                    //result = dt.AsEnumerable().GroupBy(x => x.Field<string>("Key_ID")).Select(x => new UniquenessSetupKeyInfo() { KeyID = Convert.ToInt32(x.Key) }).ToList();

                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].IsMaster == true)
                        {
                            result[i].SelectedAttributes = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Attribute_Name = x.Field<string>("Attribute_Name") }).Where(pp => Convert.ToInt32(pp.Key.Key_ID) == result[i].KeyID).Select(x => new UniquenessSetupAttributeInfo() { AttributeName = x.Key.Attribute_Name, AttributeIDs = string.Join(",", x.Select(y => Convert.ToString(y["Attribute_ID"]))), AreAdditionalLegAttributes = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).ToList();
                        }
                        else
                        {
                            //result[i].SelectedLeg = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Leg_Name = x.Field<string>("Leg_Name") }).Where(pp => Convert.ToInt32(pp.Key.Key_ID) == result[i].KeyID).Select(x => new UniquenessSetupLegInfo() { LegName = x.Key.Leg_Name, LegIDs = string.Join(",", x.Select(y => Convert.ToString(y["Additional_Leg_ID"]))), AreAdditionalLegs = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).First();
                            UniqueCompare comparer = new UniqueCompare();

                            result[i].SelectedLeg = dt.AsEnumerable().Where(x => Convert.ToInt32(x.Field<int>("Key_ID")) == result[i].KeyID).Distinct(comparer).GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Leg_Name = x.Field<string>("Leg_Name") }).Select(x => new UniquenessSetupLegInfo() { LegName = x.Key.Leg_Name, LegIDs = string.Join(",", x.Select(y => Convert.ToString(y["Additional_Leg_ID"]))), AreAdditionalLegs = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).First();

                            result[i].SelectedLegAttributes = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Leg_Name = x.Field<string>("Leg_Name"), Leg_Attribute_Name = x.Field<string>("Attribute_Name") }).Where(pp => Convert.ToInt32(pp.Key.Key_ID) == result[i].KeyID).Select(x => new UniquenessSetupAttributeInfo() { AttributeName = x.Key.Leg_Attribute_Name, AttributeIDs = string.Join(",", x.Select(y => Convert.ToString(y["Attribute_ID"]))), AreAdditionalLegAttributes = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetUniqueKeysForSelectedSectypes -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetUniqueKeysForSelectedSectypes -> End");
            }

            return result;
        }

        //Handled in UI Itself
        /*public List<UniquenessSetupKeyInfo> SearchUniqueKeys(List<int> selectedSectypes, string searchString)
        {
            mLogger.Debug("CommonService -> SearchUniqueKeys -> Start");

            List<UniquenessSetupKeyInfo> result = new List<UniquenessSetupKeyInfo>();

            try
            {
                string selectedSectypesString = "";

                if (selectedSectypes.Count > 0)
                {
                    selectedSectypesString = Convert.ToString(selectedSectypes[0]);

                    if (selectedSectypes.Count > 1)
                    {
                        for (int i = 1; i < selectedSectypes.Count; i++)
                            selectedSectypesString += ("," + Convert.ToString(selectedSectypes[i]));
                    }
                }

                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_SearchUniqueKeys '{0}', '{1}'", selectedSectypesString, searchString), ConnectionConstants.SecMaster_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    result = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Key_Name = x.Field<string>("Key_Name"), Is_Master = x.Field<bool>("Is_Master"), Is_Across_Securities = x.Field<bool>("Is_Across_Securities") }).Select(x => new UniquenessSetupKeyInfo() { KeyID = Convert.ToInt32(x.Key.Key_ID), KeyName = Convert.ToString(x.Key.Key_Name), IsMaster = Convert.ToBoolean(x.Key.Is_Master), IsAcrossSecurities = Convert.ToBoolean(x.Key.Is_Across_Securities), SelectedSectypes = x.Select(y => Convert.ToInt32(y["Sectype_ID"])).ToList() }).ToList();
                    //result = dt.AsEnumerable().GroupBy(x => x.Field<string>("Key_ID")).Select(x => new UniquenessSetupKeyInfo() { KeyID = Convert.ToInt32(x.Key) }).ToList();

                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].IsMaster == true)
                        {
                            result[i].SelectedAttributes = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Attribute_Name = x.Field<string>("Attribute_Name") }).Where(pp => Convert.ToInt32(pp.Key.Key_ID) == result[i].KeyID).Select(x => new UniquenessSetupAttributeInfo() { AttributeName = x.Key.Attribute_Name, AttributeIDs = string.Join(",", x.Select(y => Convert.ToString(y["Attribute_ID"]))), AreAdditionalLegAttributes = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).ToList();
                        }
                        else
                        {
                            result[i].SelectedLeg = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Leg_Name = x.Field<string>("Leg_Name") }).Where(pp => Convert.ToInt32(pp.Key.Key_ID) == result[i].KeyID).Select(x => new UniquenessSetupLegInfo() { LegName = x.Key.Leg_Name, LegIDs = string.Join(",", x.Select(y => Convert.ToString(y["Additional_Leg_ID"]))), AreAdditionalLegs = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).First();

                            result[i].SelectedLegAttributes = dt.AsEnumerable().GroupBy(x => new { Key_ID = x.Field<int>("Key_ID"), Leg_Name = x.Field<string>("Leg_Name"), Leg_Attribute_Name = x.Field<string>("Attribute_Name") }).Where(pp => Convert.ToInt32(pp.Key.Key_ID) == result[i].KeyID).Select(x => new UniquenessSetupAttributeInfo() { AttributeName = x.Key.Leg_Attribute_Name, AttributeIDs = string.Join(",", x.Select(y => Convert.ToString(y["Attribute_ID"]))), AreAdditionalLegAttributes = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> SearchUniqueKeys -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> SearchUniqueKeys -> End");
            }

            return result;
        }
        */

        public List<UniquenessSetupLegInfo> GetCommonLegsForSelectedSectypes(List<int> selectedSectypes)
        {
            mLogger.Debug("CommonService -> GetCommonLegsForSelectedSectypes -> Start");

            List<UniquenessSetupLegInfo> result = new List<UniquenessSetupLegInfo>();

            try
            {
                string selectedSectypesString = "";

                if (selectedSectypes.Count > 0)
                {
                    selectedSectypesString = Convert.ToString(selectedSectypes[0]);

                    if (selectedSectypes.Count > 1)
                    {
                        for (int i = 1; i < selectedSectypes.Count; i++)
                            selectedSectypesString += ("," + Convert.ToString(selectedSectypes[i]));
                    }
                }

                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_GetCommonLegsForSelectedSectypes '{0}'", selectedSectypesString), ConnectionConstants.SecMaster_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    //Return leg_ids and are_additional_leg comma seperated.

                    DataTable dt = ds.Tables[0];

                    result = dt.AsEnumerable().GroupBy(x => x.Field<string>("Leg_Name")).Select(x => new UniquenessSetupLegInfo() { LegName = x.Key, LegIDs = string.Join(",", x.Select(y => Convert.ToString(y["Leg_ID"]))), AreAdditionalLegs = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).ToList();
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetCommonLegsForSelectedSectypes -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetCommonLegsForSelectedSectypes -> End");
            }

            return result;
        }

        public UniquenessSetupCommonMasterAttributesOutputInfo GetCommonMasterAttributesForSelectedSectypes(string userName, List<int> selectedSectypes, int KeyID)
        {
            mLogger.Debug("CommonService -> GetCommonMasterAttributesForSelectedSectypes -> Start");

            UniquenessSetupCommonMasterAttributesOutputInfo result = new UniquenessSetupCommonMasterAttributesOutputInfo();
            result.KeyID = KeyID;
            //List<UniquenessSetupAttributeInfo> result = new List<UniquenessSetupAttributeInfo>();

            try
            {
                string selectedSectypesString = "";

                if (selectedSectypes.Count > 0)
                {
                    selectedSectypesString = Convert.ToString(selectedSectypes[0]);

                    if (selectedSectypes.Count > 1)
                    {
                        for (int i = 1; i < selectedSectypes.Count; i++)
                            selectedSectypesString += ("," + Convert.ToString(selectedSectypes[i]));
                    }
                }

                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_GetCommonMasterAttributesForSelectedSectypes '{0}', '{1}'", selectedSectypesString, userName), ConnectionConstants.SecMaster_Connection);

                //Manipulating the result retrieved from DB
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    result.CommonMasterAttributesList = dt.AsEnumerable().GroupBy(x => x.Field<string>("Attribute_Display_Name")).Select(x => new UniquenessSetupAttributeInfo() { AttributeName = x.Key, AttributeIDs = string.Join(",", x.Select(y => Convert.ToString(y["Attribute_ID"]))), AreAdditionalLegAttributes = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).ToList();
                }

            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetCommonMasterAttributesForSelectedSectypes -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetCommonMasterAttributesForSelectedSectypes -> End");
            }

            return result;
        }

        public UniquenessSetupCommonLegAttributesOutputInfo GetCommonLegAttributesForSelectedLegName(UniquenessSetupLegInfo InputObject, int KeyID)
        {
            mLogger.Debug("CommonService -> GetCommonLegAttributesForSelectedLegName -> Start");

            UniquenessSetupCommonLegAttributesOutputInfo result = new UniquenessSetupCommonLegAttributesOutputInfo();
            result.KeyID = KeyID;

            try
            {
                //NOTE : When Multi Info Leg support is added, the following Stored Procedure will need change.

                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_GetCommonLegAttributesForSelectedLegName '{0}', '{1}', '{2}'", InputObject.LegIDs, InputObject.LegName, InputObject.AreAdditionalLegs), ConnectionConstants.SecMaster_Connection);

                //Manipulating the result retrieved from DB
                //NOTE : When Multi Info Leg support is added, the following code will need change.
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    result.CommonLegAttributesList = dt.AsEnumerable().GroupBy(x => x.Field<string>("Attribute_Display_Name")).Select(x => new UniquenessSetupAttributeInfo() { AttributeName = x.Key, AttributeIDs = string.Join(",", x.Select(y => Convert.ToString(y["Attribute_ID"]))), AreAdditionalLegAttributes = string.Join(",", x.Select(y => Convert.ToString(y["Is_Additional_Leg"]))) }).ToList();
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> GetCommonLegAttributesForSelectedLegName -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> GetCommonLegAttributesForSelectedLegName -> End");
            }

            return result;
        }

        public UniquenessSetupOutputObject CreateUniqueKey(string userName, UniquenessSetupKeyInfo InputObject)
        {
            mLogger.Debug("CommonService -> CreateUniqueKey -> Start");

            UniquenessSetupOutputObject result = new UniquenessSetupOutputObject();
            // result.status 
            //  0 : Unique Key Validation failed
            // -1 : Data is NOT Unique
            //  1 : Key Successfully Inserted/Updated.
            //  2 : Key Successfully Validated (NOT Inserted/Updated)

            try
            {
                //Perform Unique Key Setup Validation before performing Uniqueness Check on Securities Data
                bool is_key_setup_validation_only = true;

                //Create Selected Sectypes String
                string selectedSectypesString = "";

                if (InputObject.SelectedSectypes.Count > 0)
                {
                    selectedSectypesString = Convert.ToString(InputObject.SelectedSectypes[0]);

                    if (InputObject.SelectedSectypes.Count > 1)
                    {
                        for (int i = 1; i < InputObject.SelectedSectypes.Count; i++)
                            selectedSectypesString += ("," + Convert.ToString(InputObject.SelectedSectypes[i]));
                    }
                }

                XDocument attributesXML;

                if (InputObject.IsMaster)
                {
                    //Find length of attributes
                    int selectedAttributesCount = InputObject.SelectedAttributes.Count;

                    //Create XML from the selected Attributes
                    attributesXML = new XDocument(
                                        new XElement("attributeList",
                                            Enumerable.Range(0, selectedAttributesCount).Select(i => new XElement("attributeListItem", new XAttribute("attribute_ids", InputObject.SelectedAttributes[i].AttributeIDs), new XAttribute("is_additional_leg", InputObject.SelectedAttributes[i].AreAdditionalLegAttributes), new XAttribute("sectype_ids", selectedSectypesString))))
                                        );
                }
                else
                {
                    //Find length of attributes
                    int selectedLegAttributesCount = InputObject.SelectedLegAttributes.Count;

                    //Create XML from the selected Leg Attributes
                    attributesXML = new XDocument(
                                        new XElement("attributeList",
                                            Enumerable.Range(0, selectedLegAttributesCount).Select(i => new XElement("attributeListItem", new XAttribute("attribute_ids", InputObject.SelectedLegAttributes[i].AttributeIDs), new XAttribute("is_additional_leg", InputObject.SelectedLegAttributes[i].AreAdditionalLegAttributes), new XAttribute("sectype_ids", selectedSectypesString))))
                                        );
                }

                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_InsertUniqueKey @key_name='{0}', @is_master={1}, @user_name='{2}', @attributes_xml='{3}', @is_across_securities={4}, @is_validation_only={5}, @check_in_drafts={6}, @check_in_workflows={7}, @null_as_unique={8}", InputObject.KeyName, InputObject.IsMaster, userName, attributesXML, InputObject.IsAcrossSecurities, is_key_setup_validation_only, InputObject.CheckInDrafts, InputObject.CheckInWorkflows, InputObject.NullAsUnique), ConnectionConstants.SecMaster_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    //////////////////////////////////////////////
                    //NOTE : ADD CONSTRAINTS ISSUE MESSAGES HERE//
                    //////////////////////////////////////////////
                    result.status = Convert.ToInt32(ds.Tables[0].Rows[0]["status"]);

                    if (result.status == 0)                //If Key Setup Validation Fails
                    {
                        result.message = Convert.ToString(ds.Tables[0].Rows[0]["message"]);
                    }
                    else
                    {
                        //If Key Setup is successfully validated, then perform Uniqueness Check on Securities Data.
                        //Check Uniqueness on Existing Data
                        bool saveKeyAttempt = false;

                        if (InputObject.IsMaster)
                        {
                            HashSet<string> tempVar = new HashSet<string>();
                            Dictionary<string, string> attributeNames = new Dictionary<string, string>();

                            foreach (var item in InputObject.SelectedAttributes)
                            {
                                attributeNames.Add(item.AttributeName, item.AttributeName);
                            }

                            Assembly SecMasterRuleManager = Assembly.Load("SecMasterRuleManager");
                            Type RulesExecutor = SecMasterRuleManager.GetType("com.ivp.secm.SecMasterRuleManager.RulesExecutor");

                            MethodInfo GetMasterDuplicates = RulesExecutor.GetMethod("GetMasterDuplicates");
                            var RulesExecutorObj = Activator.CreateInstance(RulesExecutor);
                            Dictionary<string, List<SMUniquenessFailureSecurityInfo>> UniquenessCheckDictionary = (Dictionary<string, List<SMUniquenessFailureSecurityInfo>>)GetMasterDuplicates.Invoke(RulesExecutorObj, new object[] { null, InputObject.SelectedSectypes, attributeNames, tempVar, null, true, InputObject.CheckInWorkflows, InputObject.CheckInDrafts, InputObject.NullAsUnique, false });


                            if (UniquenessCheckDictionary != null && UniquenessCheckDictionary.Count == 0)
                            {
                                saveKeyAttempt = true;
                            }
                            else
                            {
                                //dtUniquenessCheck = new DataTable("UniquenessCheckErrorInfo");

                                //DataColumn dcSecType = new DataColumn("Security Type");
                                //DataColumn dcSecID = new DataColumn("Security ID");
                                //DataColumn dcSecName = new DataColumn("Security Name");

                                //dtUniquenessCheck.Columns.Add(dcSecType);
                                //dtUniquenessCheck.Columns.Add(dcSecID);
                                //dtUniquenessCheck.Columns.Add(dcSecName);

                                //for (int i=0; i<InputObject.SelectedAttributes.Count; i++)
                                //{
                                //    DataColumn dcColName = new DataColumn(InputObject.SelectedAttributes[i].AttributeName);
                                //    dtUniquenessCheck.Columns.Add(dcColName);
                                //}

                                //string sec_ids = "";

                                //foreach (var item in UniquenessCheckDictionary)
                                //{
                                //    foreach (var item1 in item.Value)
                                //    {
                                //        sec_ids += sec_ids == "" ? item1.SecurityId : ", " + item1.SecurityId;
                                //    }
                                //}

                                //string sec_ids = "";
                                //HashSet<string> secIdList = new HashSet<string>();
                                //foreach (var item in UniquenessCheckDictionary)
                                //{
                                //    foreach (var item1 in item.Value)
                                //    {
                                //        if (!secIdList.Contains(item1.SecurityId))
                                //            secIdList.Add(item1.SecurityId);

                                //        //sec_ids += sec_ids == "" ? item1 : ", " + item1;
                                //    }
                                //}
                                //if (secIdList.Count() > 0)
                                //    sec_ids = string.Join(",", secIdList);

                                ////////////////////////////////////////////////////////////////////////
                                // Massage UniquenessCheckDictionary into List<UniquenessfailureInfo> //
                                ////////////////////////////////////////////////////////////////////////

                                List<UniquenessFailureInfo> LstUniquenessFailureInfo = new List<UniquenessFailureInfo>();

                                foreach (var failureInfo in UniquenessCheckDictionary)
                                {
                                    List<SMUniquenessFailureSecurityInfo> secIds = new List<SMUniquenessFailureSecurityInfo>(failureInfo.Value);

                                    //NOT Required
                                    //SMUniquenessFailureSecurityInfo secInfoToRemove = secIds.Where(secLevel => secLevel.SecurityId.Equals(x.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                    //if (secInfoToRemove != null)
                                    //    secIds.Remove(secInfoToRemove);

                                    int index = 0;
                                    List<string> attrValues = failureInfo.Key.Split(new string[] { "<@>" }, StringSplitOptions.None).ToList();
                                    Dictionary<string, string> attributeNamesVsValues = new Dictionary<string, string>();
                                    foreach (var attrName in InputObject.SelectedAttributes.Select(attr => attr.AttributeName))
                                    {
                                        attributeNamesVsValues[attrName] = attrValues[index++];
                                    }

                                    LstUniquenessFailureInfo.Add(new UniquenessFailureInfo { AttributeNamesVsValues = attributeNamesVsValues, UniqueKeyName = InputObject.KeyName, SecIds = secIds, SelectedSectypes = InputObject.SelectedSectypes, IsMaster = InputObject.IsMaster });
                                }


                                /////////////////////////////////////////////////////////////
                                // Massage List<UniquenessFailureInfo> into info for Popup //
                                /////////////////////////////////////////////////////////////

                                Assembly SecMasterDashboardServiceAss = Assembly.Load("SecMasterDashboardService");
                                Type SecMasterDashboardService = SecMasterDashboardServiceAss.GetType("com.ivp.secm.api.ui.SecMasterDashboardService");
                                MethodInfo SMPrepareUniquenessFailurePopupInfo = SecMasterDashboardService.GetMethod("SMPrepareUniquenessFailurePopupInfo");

                                object dashboardServiceObj = Activator.CreateInstance(SecMasterDashboardService);

                                var SMUniquenessOutputInfo = SMPrepareUniquenessFailurePopupInfo.Invoke(dashboardServiceObj, new object[] { LstUniquenessFailureInfo });

                                result.uniquenessFailurePopupInfo = SMUniquenessOutputInfo != null ? (List<KeyValuePair<string, List<SRMDuplicateInfo>>>)SMUniquenessOutputInfo : null;
                                result.message = "Data is not Unique on Selected Attributes.";
                                result.status = -1;
                                //message = "Data is not Unique on Selected Attributes";
                            }
                        }
                        else
                        {
                            HashSet<string> tempVar = new HashSet<string>();
                            HashSet<string> tempVar1 = new HashSet<string>();

                            Dictionary<string, string> attributeNames = new Dictionary<string, string>();

                            foreach (var item in InputObject.SelectedLegAttributes)
                            {
                                attributeNames.Add(item.AttributeName, item.AttributeName);
                            }

                            Assembly SecMasterRuleManager = Assembly.Load("SecMasterRuleManager");
                            Type RulesExecutor = SecMasterRuleManager.GetType("com.ivp.secm.SecMasterRuleManager.RulesExecutor");

                            MethodInfo GetLegDuplicates = RulesExecutor.GetMethod("GetLegDuplicates");
                            var RulesExecutorObj = Activator.CreateInstance(RulesExecutor);
                            Dictionary<string, Dictionary<string, List<int>>> UniquenessCheckDictionary = (Dictionary<string, Dictionary<string, List<int>>>)GetLegDuplicates.Invoke(RulesExecutorObj, new object[] { null, InputObject.SelectedSectypes, InputObject.SelectedLeg.LegName, attributeNames, tempVar, null, tempVar1, null, true, !InputObject.IsAcrossSecurities, InputObject.CheckInWorkflows, InputObject.CheckInDrafts, InputObject.NullAsUnique, false });

                            if (UniquenessCheckDictionary != null && UniquenessCheckDictionary.Count == 0)
                            {
                                saveKeyAttempt = true;
                            }
                            else
                            {
                                //string sec_ids = "";

                                //foreach (var item in UniquenessCheckDictionary)
                                //{
                                //    foreach (var item1 in item.Value)
                                //    {
                                //        sec_ids += sec_ids == "" ? item1.Key.Split(new string[] { "ž" }, StringSplitOptions.None)[0] : ", " + item1.Key.Split(new string[] { "ž" }, StringSplitOptions.None)[0];
                                //    }
                                //}

                                //message = "Data is not Unique on Selected Attributes in following Securities : " + sec_ids;

                                //string sec_ids = "";
                                //HashSet<string> secIdList = new HashSet<string>();
                                //foreach (var item in UniquenessCheckDictionary)
                                //{
                                //    string secId;
                                //    foreach (var item1 in item.Value)
                                //    {
                                //        secId = item1.Key.Split(new string[] { "ž" }, StringSplitOptions.None)[0];
                                //        if (!secIdList.Contains(secId))
                                //            secIdList.Add(secId);

                                //        //sec_ids += sec_ids == "" ? item1 : ", " + item1;
                                //    }
                                //}
                                //if (secIdList.Count() > 0)
                                //    sec_ids = string.Join(",", secIdList);
                                ////////////////////////////////////////////////////////////////////////
                                // Massage UniquenessCheckDictionary into List<UniquenessfailureInfo> //
                                ////////////////////////////////////////////////////////////////////////

                                List<UniquenessFailureInfo> LstUniquenessFailureInfo = new List<UniquenessFailureInfo>();

                                foreach (var failureInfo in UniquenessCheckDictionary)
                                {
                                    List<SMUniquenessFailureSecurityInfo> secIds = failureInfo.Value.Keys.Select(secLevel => new SMUniquenessFailureSecurityInfo { SecurityId = secLevel.Split('ž')[0], FailureType = (SMUniquenessFailureType)Enum.Parse(typeof(SMUniquenessFailureType), secLevel.Split('ž')[1], true) }).ToList();

                                    //SMUniquenessFailureSecurityInfo secInfoToRemove = secIds.Where(secLevel => secLevel.SecurityId.Equals(x.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                    //if (secInfoToRemove != null)
                                    //    secIds.Remove(secInfoToRemove);

                                    int index = 0;
                                    List<string> attrValues = failureInfo.Key.Split(new string[] { "<@>" }, StringSplitOptions.None).ToList();
                                    Dictionary<string, string> attributeNamesVsValues = new Dictionary<string, string>();
                                    foreach (var attrName in InputObject.SelectedLegAttributes.Select(attr => attr.AttributeName))
                                    {
                                        attributeNamesVsValues[attrName] = attrValues[index++];
                                    }

                                    LstUniquenessFailureInfo.Add(new UniquenessFailureInfo { AttributeNamesVsValues = attributeNamesVsValues, UniqueKeyName = InputObject.KeyName, SecIds = secIds, LegName = InputObject.SelectedLeg.LegName, SelectedSectypes = InputObject.SelectedSectypes, IsMaster = InputObject.IsMaster });
                                }

                                /////////////////////////////////////////////////////////////
                                // Massage List<UniquenessFailureInfo> into info for Popup //
                                /////////////////////////////////////////////////////////////

                                Assembly SecMasterDashboardServiceAss = Assembly.Load("SecMasterDashboardService");
                                Type SecMasterDashboardService = SecMasterDashboardServiceAss.GetType("com.ivp.secm.api.ui.SecMasterDashboardService");
                                MethodInfo SMPrepareUniquenessFailurePopupInfo = SecMasterDashboardService.GetMethod("SMPrepareUniquenessFailurePopupInfo");

                                object dashboardServiceObj = Activator.CreateInstance(SecMasterDashboardService);

                                var SMUniquenessOutputInfo = SMPrepareUniquenessFailurePopupInfo.Invoke(dashboardServiceObj, new object[] { LstUniquenessFailureInfo });

                                result.uniquenessFailurePopupInfo = SMUniquenessOutputInfo != null ? (List<KeyValuePair<string, List<SRMDuplicateInfo>>>)SMUniquenessOutputInfo : null;
                                result.message = "Data is not Unique on Selected Attributes.";
                                result.status = -1;
                            }
                        }


                        if (saveKeyAttempt)
                        {
                            //Executing the Stored Procedure
                            DataSet ds2 = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_InsertUniqueKey @key_name='{0}', @is_master={1}, @user_name='{2}', @attributes_xml='{3}', @is_across_securities={4},@check_in_drafts={5}, @check_in_workflows={6}, @null_as_unique={7}", InputObject.KeyName, InputObject.IsMaster, userName, attributesXML, InputObject.IsAcrossSecurities, InputObject.CheckInDrafts, InputObject.CheckInWorkflows, InputObject.NullAsUnique), ConnectionConstants.SecMaster_Connection);

                            if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0] != null && ds2.Tables[0].Rows.Count > 0)
                            {
                                result.status = Convert.ToInt32(ds2.Tables[0].Rows[0]["status"]);
                                if (result.status == 0)
                                {
                                    result.message = Convert.ToString(ds2.Tables[0].Rows[0]["message"]);
                                }
                                else
                                {
                                    //If successfully added.
                                    string last_inserted_key_id = Convert.ToString(ds2.Tables[0].Rows[0]["Last_Inserted_Key_ID"]);
                                    result.message = last_inserted_key_id;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> CreateUniqueKey -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> CreateUniqueKey -> End");
            }

            return result;
        }

        public UniquenessSetupOutputObject UpdateUniqueKey(string userName, UniquenessSetupKeyInfo InputObject)
        {
            mLogger.Debug("CommonService -> UpdateUniqueKey -> Start");

            UniquenessSetupOutputObject result = new UniquenessSetupOutputObject();
            // result.status 
            //  0 : Unique Key Validation failed
            // -1 : Data is NOT Unique
            //  1 : Key Successfully Inserted/Updated.
            //  2 : Key Successfully Validated (NOT Inserted/Updated)

            try
            {
                //Perform Unique Key Setup Validation before performing Uniqueness Check on Securities Data
                bool is_key_setup_validation_only = true;

                //Create Selected Sectypes String
                string selectedSectypesString = "";

                if (InputObject.SelectedSectypes.Count > 0)
                {
                    selectedSectypesString = Convert.ToString(InputObject.SelectedSectypes[0]);

                    if (InputObject.SelectedSectypes.Count > 1)
                    {
                        for (int i = 1; i < InputObject.SelectedSectypes.Count; i++)
                            selectedSectypesString += ("," + Convert.ToString(InputObject.SelectedSectypes[i]));
                    }
                }

                XDocument attributesXML;

                if (InputObject.IsMaster)
                {
                    //Find length of attributes
                    int selectedAttributesCount = InputObject.SelectedAttributes.Count;

                    //Create XML from the selected Attributes
                    attributesXML = new XDocument(
                                        new XElement("attributeList",
                                            Enumerable.Range(0, selectedAttributesCount).Select(i => new XElement("attributeListItem", new XAttribute("attribute_ids", InputObject.SelectedAttributes[i].AttributeIDs), new XAttribute("is_additional_leg", InputObject.SelectedAttributes[i].AreAdditionalLegAttributes), new XAttribute("sectype_ids", selectedSectypesString))))
                                        );
                }
                else
                {
                    //Find length of attributes
                    int selectedLegAttributesCount = InputObject.SelectedLegAttributes.Count;

                    //Create XML from the selected Leg Attributes
                    attributesXML = new XDocument(
                                        new XElement("attributeList",
                                            Enumerable.Range(0, selectedLegAttributesCount).Select(i => new XElement("attributeListItem", new XAttribute("attribute_ids", InputObject.SelectedLegAttributes[i].AttributeIDs), new XAttribute("is_additional_leg", InputObject.SelectedLegAttributes[i].AreAdditionalLegAttributes), new XAttribute("sectype_ids", selectedSectypesString))))
                                        );
                }

                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_UpdateUniqueKey @key_id={0}, @key_name='{1}', @is_master={2}, @user_name='{3}', @attributes_xml='{4}', @is_across_securities={5}, @is_validation_only={6}, @check_in_drafts={7}, @check_in_workflows={8}, @null_as_unique={9}", InputObject.KeyID, InputObject.KeyName, InputObject.IsMaster, userName, attributesXML, InputObject.IsAcrossSecurities, is_key_setup_validation_only, InputObject.CheckInDrafts, InputObject.CheckInWorkflows, InputObject.NullAsUnique), ConnectionConstants.SecMaster_Connection);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    /////////////////////////////////////////////////////////////////////////////////////
                    //NOTE : ADD CONSTRAINTS ISSUE MESSAGES HERE, i.e., when key not added successfully.
                    /////////////////////////////////////////////////////////////////////////////////////
                    result.status = Convert.ToInt32(ds.Tables[0].Rows[0]["status"]);

                    if (result.status == 0)
                    {
                        result.message = Convert.ToString(ds.Tables[0].Rows[0]["message"]);
                    }
                    else
                    {
                        //If Key Setup is successfully validated, then perform Uniqueness Check on Securities Data.
                        //Check Uniqueness on Existing Data
                        bool saveKeyAttempt = false;

                        if (InputObject.IsMaster)
                        {
                            HashSet<string> tempVar = new HashSet<string>();
                            Dictionary<string, string> attributeNames = new Dictionary<string, string>();

                            foreach (var item in InputObject.SelectedAttributes)
                            {
                                attributeNames.Add(item.AttributeName, item.AttributeName);
                            }

                            Assembly SecMasterRuleManager = Assembly.Load("SecMasterRuleManager");
                            Type RulesExecutor = SecMasterRuleManager.GetType("com.ivp.secm.SecMasterRuleManager.RulesExecutor");

                            MethodInfo GetMasterDuplicates = RulesExecutor.GetMethod("GetMasterDuplicates");
                            var RulesExecutorObj = Activator.CreateInstance(RulesExecutor);
                            Dictionary<string, List<SMUniquenessFailureSecurityInfo>> UniquenessCheckDictionary = (Dictionary<string, List<SMUniquenessFailureSecurityInfo>>)GetMasterDuplicates.Invoke(RulesExecutorObj, new object[] { null, InputObject.SelectedSectypes, attributeNames, tempVar, null, true, InputObject.CheckInWorkflows, InputObject.CheckInDrafts, InputObject.NullAsUnique, false });


                            if (UniquenessCheckDictionary != null && UniquenessCheckDictionary.Count == 0)
                            {
                                saveKeyAttempt = true;
                            }
                            else
                            {
                                ////////////////////////////////////////////////////////////////////////
                                // Massage UniquenessCheckDictionary into List<UniquenessfailureInfo> //
                                ////////////////////////////////////////////////////////////////////////
                                List<UniquenessFailureInfo> LstUniquenessFailureInfo = new List<UniquenessFailureInfo>();

                                foreach (var failureInfo in UniquenessCheckDictionary)
                                {
                                    List<SMUniquenessFailureSecurityInfo> secIds = new List<SMUniquenessFailureSecurityInfo>(failureInfo.Value);

                                    //NOT Required
                                    //SMUniquenessFailureSecurityInfo secInfoToRemove = secIds.Where(secLevel => secLevel.SecurityId.Equals(x.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                    //if (secInfoToRemove != null)
                                    //    secIds.Remove(secInfoToRemove);

                                    int index = 0;
                                    List<string> attrValues = failureInfo.Key.Split(new string[] { "<@>" }, StringSplitOptions.None).ToList();
                                    Dictionary<string, string> attributeNamesVsValues = new Dictionary<string, string>();
                                    foreach (var attrName in InputObject.SelectedAttributes.Select(attr => attr.AttributeName))
                                    {
                                        attributeNamesVsValues[attrName] = attrValues[index++];
                                    }

                                    LstUniquenessFailureInfo.Add(new UniquenessFailureInfo { AttributeNamesVsValues = attributeNamesVsValues, UniqueKeyName = InputObject.KeyName, SecIds = secIds, SelectedSectypes = InputObject.SelectedSectypes, IsMaster = InputObject.IsMaster });
                                }


                                /////////////////////////////////////////////////////////////
                                // Massage List<UniquenessFailureInfo> into info for Popup //
                                /////////////////////////////////////////////////////////////
                                Assembly SecMasterDashboardServiceAss = Assembly.Load("SecMasterDashboardService");
                                Type SecMasterDashboardService = SecMasterDashboardServiceAss.GetType("com.ivp.secm.api.ui.SecMasterDashboardService");
                                MethodInfo SMPrepareUniquenessFailurePopupInfo = SecMasterDashboardService.GetMethod("SMPrepareUniquenessFailurePopupInfo");

                                object dashboardServiceObj = Activator.CreateInstance(SecMasterDashboardService);

                                var SMUniquenessOutputInfo = SMPrepareUniquenessFailurePopupInfo.Invoke(dashboardServiceObj, new object[] { LstUniquenessFailureInfo });

                                result.uniquenessFailurePopupInfo = SMUniquenessOutputInfo != null ? (List<KeyValuePair<string, List<SRMDuplicateInfo>>>)SMUniquenessOutputInfo : null;
                                result.message = "Data is not Unique on Selected Attributes.";
                                result.status = -1;
                            }
                        }
                        else
                        {
                            HashSet<string> tempVar = new HashSet<string>();
                            HashSet<string> tempVar1 = new HashSet<string>();

                            Dictionary<string, string> attributeNames = new Dictionary<string, string>();

                            foreach (var item in InputObject.SelectedLegAttributes)
                            {
                                attributeNames.Add(item.AttributeName, item.AttributeName);
                            }

                            Assembly SecMasterRuleManager = Assembly.Load("SecMasterRuleManager");
                            Type RulesExecutor = SecMasterRuleManager.GetType("com.ivp.secm.SecMasterRuleManager.RulesExecutor");

                            MethodInfo GetLegDuplicates = RulesExecutor.GetMethod("GetLegDuplicates");
                            var RulesExecutorObj = Activator.CreateInstance(RulesExecutor);
                            Dictionary<string, Dictionary<string, List<int>>> UniquenessCheckDictionary = (Dictionary<string, Dictionary<string, List<int>>>)GetLegDuplicates.Invoke(RulesExecutorObj, new object[] { null, InputObject.SelectedSectypes, InputObject.SelectedLeg.LegName, attributeNames, tempVar, null, tempVar1, null, true, !InputObject.IsAcrossSecurities, InputObject.CheckInWorkflows, InputObject.CheckInDrafts, InputObject.NullAsUnique, false });

                            if (UniquenessCheckDictionary != null && UniquenessCheckDictionary.Count == 0)
                            {
                                saveKeyAttempt = true;
                            }
                            else
                            {
                                ////////////////////////////////////////////////////////////////////////
                                // Massage UniquenessCheckDictionary into List<UniquenessfailureInfo> //
                                ////////////////////////////////////////////////////////////////////////

                                List<UniquenessFailureInfo> LstUniquenessFailureInfo = new List<UniquenessFailureInfo>();

                                foreach (var failureInfo in UniquenessCheckDictionary)
                                {
                                    List<SMUniquenessFailureSecurityInfo> secIds = failureInfo.Value.Keys.Select(secLevel => new SMUniquenessFailureSecurityInfo { SecurityId = secLevel.Split('ž')[0], FailureType = (SMUniquenessFailureType)Enum.Parse(typeof(SMUniquenessFailureType), secLevel.Split('ž')[1], true) }).ToList();

                                    //SMUniquenessFailureSecurityInfo secInfoToRemove = secIds.Where(secLevel => secLevel.SecurityId.Equals(x.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                    //if (secInfoToRemove != null)
                                    //    secIds.Remove(secInfoToRemove);

                                    int index = 0;
                                    List<string> attrValues = failureInfo.Key.Split(new string[] { "<@>" }, StringSplitOptions.None).ToList();
                                    Dictionary<string, string> attributeNamesVsValues = new Dictionary<string, string>();
                                    foreach (var attrName in InputObject.SelectedLegAttributes.Select(attr => attr.AttributeName))
                                    {
                                        attributeNamesVsValues[attrName] = attrValues[index++];
                                    }

                                    LstUniquenessFailureInfo.Add(new UniquenessFailureInfo { AttributeNamesVsValues = attributeNamesVsValues, UniqueKeyName = InputObject.KeyName, SecIds = secIds, LegName = InputObject.SelectedLeg.LegName, SelectedSectypes = InputObject.SelectedSectypes, IsMaster = InputObject.IsMaster });
                                }

                                /////////////////////////////////////////////////////////////
                                // Massage List<UniquenessFailureInfo> into info for Popup //
                                /////////////////////////////////////////////////////////////

                                Assembly SecMasterDashboardServiceAss = Assembly.Load("SecMasterDashboardService");
                                Type SecMasterDashboardService = SecMasterDashboardServiceAss.GetType("com.ivp.secm.api.ui.SecMasterDashboardService");
                                MethodInfo SMPrepareUniquenessFailurePopupInfo = SecMasterDashboardService.GetMethod("SMPrepareUniquenessFailurePopupInfo");

                                object dashboardServiceObj = Activator.CreateInstance(SecMasterDashboardService);

                                var SMUniquenessOutputInfo = SMPrepareUniquenessFailurePopupInfo.Invoke(dashboardServiceObj, new object[] { LstUniquenessFailureInfo });

                                result.uniquenessFailurePopupInfo = SMUniquenessOutputInfo != null ? (List<KeyValuePair<string, List<SRMDuplicateInfo>>>)SMUniquenessOutputInfo : null;
                                result.message = "Data is not Unique on Selected Attributes.";
                                result.status = -1;
                            }
                        }

                        if (saveKeyAttempt)
                        {
                            //Executing the Stored Procedure
                            DataSet ds2 = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_UpdateUniqueKey @key_id={0}, @key_name='{1}', @is_master={2}, @user_name='{3}', @attributes_xml='{4}', @is_across_securities={5}, @check_in_drafts={6}, @check_in_workflows={7}, @null_as_unique={8}", InputObject.KeyID, InputObject.KeyName, InputObject.IsMaster, userName, attributesXML, InputObject.IsAcrossSecurities, InputObject.CheckInDrafts, InputObject.CheckInWorkflows, InputObject.NullAsUnique), ConnectionConstants.SecMaster_Connection);

                            if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0] != null && ds2.Tables[0].Rows.Count > 0)
                            {
                                result.status = Convert.ToInt32(ds2.Tables[0].Rows[0]["status"]);
                                result.message = Convert.ToString(ds2.Tables[0].Rows[0]["message"]);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> UpdateUniqueKey -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> UpdateUniqueKey -> End");
            }

            return result;
        }

        public int DeleteUniqueKey(string userName, int keyID)
        {
            mLogger.Debug("CommonService -> DeleteUniqueKey -> Start");

            int result = 0;

            try
            {
                //Executing the Stored Procedure
                DataSet ds = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"EXEC IVPSecMaster.dbo.SECM_DeleteUniqueKey {0}, '{1}'", keyID, userName), ConnectionConstants.SecMaster_Connection);

                result = 1;
            }
            catch (Exception ex)
            {
                mLogger.Error("CommonService -> DeleteUniqueKey -> Exception ->" + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("CommonService -> DeleteUniqueKey -> End");
            }

            return result;
        }

    }

    internal class UniqueCompare : IEqualityComparer<DataRow>
    {
        public bool Equals(DataRow x, DataRow y)
        {
            return x["Additional_Leg_ID"].Equals(y["Additional_Leg_ID"]);
        }

        public int GetHashCode(DataRow obj)
        {
            return base.GetHashCode();
        }

    }
}
