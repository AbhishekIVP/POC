using System;
using System.Text;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;

namespace com.ivp.commom.commondal
{
    public enum CommonQueryType
    {
        /// <summary>
        /// 
        /// </summary>
        Insert = 1,
        /// <summary>
        /// 
        /// </summary>
        Update = 2,
        /// <summary>
        /// 
        /// </summary>
        Select = 3,
        /// <summary>
        /// 
        /// </summary>
        Delete = 4,
        /// <summary>
        /// 
        /// </summary>
        SelectSchema = 5
    }

    public class ConnectionConstants
    {
        public const string SecMaster_Connection = "SecMDBConnectionId";
        public const string SecMasterArchive_Connection = "SecMArchiveDBConnectionId";
        public const string SecMasterVendor_Connection = "SecMVendorDBConnectionId";
        public const string SecMasterVendorArchive_Connection = "SecMVendorArchiveDBConnectionId";
        public const string CorpAction_Connection = "CorpActDBConnectionId";
        public const string CorpActionVendor_Connection = "CorpActVendorDBConnectionId";
        public const string RefMaster_Connection = "RefMDBConnectionId";
        public const string RefMasterArchive_Connection = "RefMDBArchiveConnectionId";
        public const string RefMasterVendor_Connection = "RefMDBVendorConnectionId";
        public const string RefMasterVendorArchive_Connection = "RefMDBVendorArchiveConnectionId";
    }

    public class CoreQueryConstants
    {
        public const string GetMasterAndBasketInfoForSectype = "SECM:SECM_GetMasterAndBasketInfoForSectype";
        public const string GetUnderlyerMappedAttribute = "SECM:GetUnderlyerMappedAttribute";
        public const string GetDerivativeSecTypes = "SECM:GetDerivativeSecTypes";
        public const string GetEntitlementInfoForSectypeId = "SECM:GetEntitlementInfoForSectypeId";
        public const string GetGroupsAndUsersForEntitlement = "SECM:GetGroupsAndUsersForEntitlement";
        public const string GetAllAttributeDetailsForSectypeId = "SECM:GetAllAttributeDetailsForSectypeId";
        public const string GetPreferredDetails = "SECMV:GetPreferredDetails";
        public const string GetMnemonicValue = "SECM:GetSecurityMnemonic";
        public const string GetAllSecType = "SECM:GetAllSecType";
        public const string GetTabsAndSubTabsForTemplate = "SECM:GetTabsAndSubTabsForTemplate";
        public const string GetAttributeDetailsForTemplate = "SECM:GetAttributeDetailsForTemplate";
        public const string GetExistingXMLForTemplate = "SECM:GetExistingXMLForTemplate";
        public const string GetSecTypeAttributes = "SECM:GetSecTypeAttributes";
        public const string UpdatePreferedAttributeForCreation = "SECM:UpdatePreferedAttributeForCreation";
        public const string GetPreferedAttributesWithDefaultValues = "SECM:GetPreferedAttributesWithDefaultValues";
        public const string GetPreferenceDetails = "SECMV:GetPreferenceDetails";
        public const string GetSecurities = "SECM:GetSecurities";
        public const string AddModifySecurity = "SECM:SECM_AddModifySecurity";
        public const string GetSecurityDetails = "SECM:SECM_GetSecurityDetails";
        public const string GetDefaultValuesDetails = "SECM:GetDefaultValuesDetails";
        public const string GetCounterParties = "SECM:GetCounterParties";
        public const string GetAttributeDefaultValues = "SECM:GetAttributeDefaultValues";
        public const string GetAuditTrialValues = "SECM:SECM_GetAuditTrial";
        public const string GetSecTypeAndSecTypeTypeName = "SECM:GetSecTypeAndSecTypeTypeName";
        public const string GetSecTypeTypeName = "SECM:GetSecTypeTypeName";
        public const string GetVendorAPIDetails = "SECM:SECM_GetMappingInfo";
        public const string GetAllVendors = "SECMV:GetAllVendors";
        public const string GetSecTypeIdFromSecId = "SECM:GetSecTypeIdFromSecId";
        public const string GetMappedVendorsForSecType = "SECMV:GetMappedVendorsForSecType";
        public const string GetTabSubTabInfo = "SECM:GetTabSubTabInfo";
        public const string GetDraftsForSecurity = "SECM:GetDraftsForSecurity";
        public const string DeleteSecurityDrafts = "SECM:SECM_DeleteSecurity";
        public const string GetTemplateId = "SECM:GetTemplateId";
        public const string GetPreferredAttributes = "SECMV:GetPreferredAttributes";
        public const string GetSecurityForSecType = "SECM:SECM_GetAttributesDetailsForSecurity";
        public const string GetAttributeExceptionDetails = "SECMV:GetAttributeExceptionDetails";
        public const string GetSecIdFromVendorPrioritization = "SECMV:GetSecIdFromVendorPrioritization";
        public const string GetExcptionAttributeVendorValues = "SECMV:GetExcptionAttributeVendorValues";
        public const string InsertAttributeException = "SECM:SECM_InsertAttributeException";
        public const string GetSecIdFromException = "SECMV:GetSecIdFromException";
        public const string UpdateSecurityDetailsLater = "SECM:SECM_UpdateSecurityDetailsLater";
        public const string GetSchedulingDetails = "SECM:GetSchedulingDetails";
        public const string GetSchedulingXML = "SECM:GetSchedulingXML";
        public const string GetHierarchyNames = "SECM:GetHierarchyNames";
        public const string GetTagNames = "SECM:GetTagNames";
        public const string CreateSecurityRequest = "SECM:CreateSecurityRequest";
        public const string InsertTagName = "SECM:SECM_AddTagName";
        public const string CreateTags = "SECM:SECM_CreateTags";
        public const string CreateHierachy = "SECM:SECM_CreateHierachy";
        public const string GetAttributeNamesFromSecType = "SECM:GetArributeNamesForSecType";
        public const string CreateHierarchyFromExisting = "SECM:SECM_CreateHierarchyFromExisting";
        public const string GetCommonAttributes = "SECM:SECM_GetCommonAttributes";
        public const string SecurityRequestApproved = "SECM:SecurityRequestApproved";
        public const string GetLookupValues = "SECM:LookupQuery";
        public const string SecurityRequestRejected = "SECM:SecurityRequestRejected";
        public const string HiddenFieldAttrValue = "SECM:HiddenValueAttrValue";
        public const string ApproveCreation = "SECM:ApproveBtnCreateSecurity";
        public const string GetHierarchyNamesAndId = "SECM:GetHierarchyNamesAndId";
        public const string GetHierarchy = "SECM:SECM_GetHierarchy";
        public const string AddModifyDefaultValues = "SECM:SECM_AddModifyDefaultValues";
        public const string GetReferenceDetails = "SECM:GetReferenceDetails";
        public const string CreateHierarchyCustom = "SECM:SECM_CreateHierarchyCustom";
        public const string GetSecType = "SECM:GetSecTypes";
        public const string GetUnderliersForSecurity = "SECM:SECM_GetUnderliersForSecurity";
        public const string CreateWorkFlow = "SECM:CreateWorkFlow";
        public const string GetWorkFlowData = "SECM:GetWorkFlowData";
        public const string AssignUsers = "SECM:AddUsers";
        public const string SaveWorkFlow = "SECM:SaveWorkFlow";
        public const string ConvertNamesIds = "SECM:SECM_ConvertNamesIds";
        public const string SecuritiesToBeViewed = "SECM:SECM_SecuritiesToBeViewed";
        public const string GetAssociationDetails = "SECM:SECM_GetAssociationDetails";
        public const string SearchSecurities = "SECM:SECM_SearchSecurities";
        public const string GetHierarchyLeafNodes = "SECM:SECM_GetHierarchyLeafNodes";
        public const string SaveGraphHierarchyLeafNode = "SECM:SECM_SaveGraphHierarchyLeafNode";
        public const string GetLevelName = "SECM:GetLevelName";
        public const string GetOtherIdentifiers = "SECM:SECM_GetOtherIdentifiers";
        public const string GetReferenceKeyAttributes = "SECM:GetReferenceKeyAttribute";
        public const string SaveReferenceData = "SECM:SECM_SaveReferenceData";
        public const string DeleteReferenceData = "SECM:DeleteReferenceData";
        public const string GetGraphHierarchyXML = "SECM:GetGraphHierarchyXML";
        public const string GetAttributeHierarchyAttributes = "SECM:SECM_GetAttributeHierarchyAttributes";
        public const string UniqueHierarchyName = "SECM:UniqueHierarchyName";
        public const string UniqueRefData = "SECM:UniqueRefData";
        public const string GetInfoToGenerateXml = "SECM:GetInfoToGenerateXml";
        public const string SearchAuditTrail = "SECM:SECM_SearchAuditTrail";
        public const string LayoutManager = "SECM:SECM_LayoutManager";
        public const string DeletePreferenceData = "SECMV:DeletePreferenceData";
        public const string BindDataSourceForRealTimePreference = "SECMV:BindDataSourceForRealTimePreference";
        public const string BindShuttleForIdentifiers = "SECMV:BindShuttleForIdentifiers";
        public const string SaveRealTimePreference = "SECMV:SaveRealTimePreference";
        public const string BindGridForRealTimePreference = "SECMV:BindGridForRealTimePreference";
        public const string UpdateRealTimePreference = "SECMV:UpdateRealTimePreference";
        public const string InsertBgMultipleValueException = "SECM:InsertBgMultipleValueException";
        public const string InsertBgDataSetFailureException = "SECM:InsertBgDataSetFailureException";
        public const string GetPreferenceDetailTableDetails = "SECMV:GetPreferenceDetailTableDetails";
        public const string GetLegSection = "SECM:GetLegSection";
        public const string GetLegNames = "SECM:GetLegNames";
        // public const string GetLayoutDetails = "SECM:GetLayoutDetails";
        public const string GetLegAttributesList = "SECM:GetLegAttributesList";
        public const string AddModifySecTypeTable = "SECM:AddModifySecTypeTable";
        public const string UpdateLegName = "SECM:UpdateLegName";
        public const string SECM_AddCommonAttribute = "SECM:SECM_AddCommonAttribute";
        public const string GetSecTypeInfo = "SECM:GetSecTypeInfo";
        public const string SECM_CreateSectype = "SECM:SECM_CreateSectype";
        public const string CheckUnderLierMaster = "SECM:CheckUnderLierMaster";
        public const string UpdateSecTypeType = "SECM:UpdateSecTypeType";
        public const string SECM_TabManagement = "SECM:SECM_TabManagement";
        public const string GetAllSecurityTypes = "SECM:GetAllSecurityTypes";
        public const string GetAllAttributesForSecType = "SECM:GetAllAttributesForSecType";
        public const string GetRequestsForUser = "SECM:SECM_GetRequestsForUser";
        public const string DeleteSearchCriteria = "SECM:DeleteSearchCriteria";
        public const string GetSearchCriteria = "SECM:GetSearchCriteria";
        public const string SaveSearchCriteria = "SECM:SECM_SaveSearchCriteria";
        public const string GetDetailsForSavedSearch = "SECM:GetDetailsForSavedSearch";
        public const string GetSearchSecurityDetails = "SECM:GetSecurityDetails";
        public const string GetSecurityUnderliers = "SECM:GetSecurityUnderliers";
        public const string GetAttributeAuditHistory = "SECM:SECM_AuditTrailForSingleAttribute";
        public const string GetDataSetForCreateBulkSecurity = "SECM:SECM_GetDataSetForCreateBulkSecurities";
        public const string GetSecurityAuditHistory = "SECM:SECM_GetSecurityAuditHistory";
        public const string GetCompleteAuditHistoryBySecurityID = "SECM:GetCompleteAuditHistoryBySecID";
        public const string GetCommonAttributesExcludingReferenceData = "SECM:GetCommonAttributesExcludingReferenceData";
        public const string GetActionResult = "SECMV:SECMV_GetActionQuery";
        public const string GetSecurityActivationStatus = "SECM:GetSecurityActivationStatus";
        public const string DeleteSecurityActivationStatus = "SECM:DeleteSecurityActivationStatus";
        public const string UpdateSecurityActivationEffectiveDate = "SECM:UpdateSecurityActivationEffectiveDate";
        public const string GetMnemonicAttributes = "SECM:GetMnemonicAttributes";
        public const string DeleteSecurityActivationTask = "SECM:DeleteActivationTaskStatus";
        public const string GetSecurityImpact = "SECM:SECM_GetSecurityImpact";
        public const string GetRepositoryDetailsByName = "SECM:GetRepositoryDetailsByName";
        public const string GetSecTypeIdForSecTypeName = "SECM:GetSecTypeIdForSecTypeName";
        public const string GetSecIDforDrafts = "SECM:GetSecIDforDrafts";
        public const string UpdateSecTypeDescription = "SECM:UpdateSecTypeDescription";
        public const string GetSecurityDraftsForSecType = "SECM:GetSecurityDraftsForSecType";
        public const string GetSecTypeLegDetails = "SECM:GetSecTypeLegDetails";
        public const string GetMnemonicAttributeList = "SECM:GetMnemonicAttributeList";
        public const string SaveMnemonics = "SECM:SaveMnemonics";
        public const string GetPrefferedVendorIdetifierForSecType = "SECMV:GetPrefferedVendorIdetifierForSecType";
        public const string UploadFile = "SECM:SECM_UploadFile";
        public const string UploadFileInVendor = "SECMV:SECMV_UploadFile";
        public const string GetUploadedFile = "SECM:GetUploadedFile";
        public const string AddDefaultTab = "SECM:SECM_AddDefaultTab";
        public const string AddModifySecurityRealTime = "SECM:SECM_RealTimeAddModifySecurity";
        public const string GetMacroData = "SECMV:GetMacroData";
        public const string GetDetailsOfsecurities = "SECM:SECM_GetDetailsOfsecurities";
        public const string ActivateSecurity = "SECM:SECM_ActivateSecurity";
        public const string ActivateBulkSecurity = "SECM:SECM_ActivateBulkSecurity";
        public const string UpdateBulkSecurityStatus = "SECM:UpdateBulkSecurityStatus";
        public const string DeleteSecurityWithStatus = "SECM:SECM_DeleteSecurityWithStatus";
        public const string ReferenceDataSearch = "SECM:SECM_ReferenceDataSearch";
        public const string GetAllSecTypeNames = "SECM:GetAllSecTypeNames";
        public const string GetSecurityTypeAttributes = "SECM:GetSecurityTypeAttributes";
        public const string GetReferenceAttributeDetails = "SECM:GetReferenceAttributeDetails";
        public const string GetReferenceAttributeDetailsForSearch = "SECM:GetReferenceAttributeDetailsForSearch";
        public const string GetTemplateDetails = "SECM:SECM_GetTemplateDetails";
        public const string AddUserTemplate = "SECM:SECM_AddUserTemplate";
        public const string AddTemplatePreference = "SECM:AddTemplatePreference";
        public const string ActivateSecurityType = "SECM:SECM_CheckSectypeActivation";
        public const string GetGroupNamesForUser = "SECM:GetGroupNamesForUser";
        public const string GetUnderlyingSecurities = "SECM:GetUnderlyingSecurityDetails";
        public const string GetUserNamesForUserTemplate = "SECM:GetUserNamesForUserTemplate";
        public const string GetExternalSystemDetails = "SECM:GetExternalSystemDetails";
        public const string GetGenevaIdForSecuritesOnDate = "SECM:SECM_GetGenevaIdForSecurities";
        public const string GetPageIdentifiersAndFunctionality = "SECM:GetPageIdentifiersAndFunctionality";
        public const string GetDataForPageConfig = "SECM:SECM_GetDataForPageConfig";
        //public const string GetDataForPageConfig = "SECM:GetPageConfigurationDetails";
        public const string InsertPageConfig = "SECM:InsertPageConfig";
        public const string GetDetailsForUnderlyingSecurity = "SECM:GetDetailsForUnderlyingSecurity";
        public const string GetAttributeDefaultValuesForTemplate = "SECM:GetAttributeDefaultValuesForTemplate";
        public const string ViewAttributeLevelTimeSeriesEntry = "SECM:SECM_ViewAttributeLevelTimeSeriesEntry";
        public const string GetAttributeTypeInfo = "SECM:GetAttributeTypeInfo";
        public const string GetPageConfiguredAttributeDetails = "SECM:GetPageConfiguredAttributeDetails";
        public const string GetUnderlyerSecurityInfoFromPageConfig = "SECM:GetUnderlyerSecurityInfoFromPageConfig";
        public const string CheckForDefaultWorkflow = "SECM:CheckForDefaultWorkflow";
        public const string GetTasksByTaskType = "SECMV:GetTasksByTaskType";
        //public const string GetPrefDetails = "SECM:GetPrefDetails";
        public const string UpdateExVendorSecTypeName = "SECM:UpdateExVendorSecTypeName";
        public const string SavePreferenceLegMapping = "SECM:SavePreferenceLegMapping";
        public const string SaveWildCardTemplate = "SECM:SaveWildCardTemplate";
        public const string GetMapping = "SECM:GetLegMapping";
        public const string DeleteMacro = "SECM:DeleteMacro";
        public const string DeleteMapping = "SECM:DeleteMapping";
        public const string GetPrefLegNames = "SECM:GetPrefLegNames";
        public const string GetRealTimeData = "SECM:GetRealTimeData";
        public const string GetRealTimePreferenceDetails = "SECM:GetRealTimePreferenceDetails";
        public const string GetRealTimeAdditionalPreferenceDetails = "SECM:GetRealTimeAdditionalPreferenceDetails";
        public const string GetRTPSectypeList = "SECM:GetRTPSectypeList";
        public const string AddModifyPreferenceDetails = "SECM:AddModifyPreferenceDetails";
        public const string GetSecurityTypeName = "SECM:GetSecurityTypeName";
        public const string GetAllDateFormats = "SECMV:GetAllDateFormats";
        public const string CheckUnderlyerExists = "SECM:CheckUnderlyerExists";
        public const string GetSecMasterIdentifier = "SECM:GetSecMasterIdentifier";
        public const string GetMacroListForSectype = "SECM:GetMacroListForSectype";
        public const string GetMacroDetails = "SECM:GetMacroDetails";
        public const string GetLegsForSectype = "SECM:GetLegsForSectype";
        public const string UpdateCommonAttribute = "SECM:UpdateCommonAttribute";
        public const string GetAllCommonAttributeDetails = "SECM:GetAllCommonAttributeDetails";
        public const string UpdateAttributeDetails = "SECM:SECM_UpdateAttributeDetails";
        public const string GetDerivedSecurityDetails = "SECM:SECM_GetDerivedSecurityDetails";
        public const string GetAttributeIdForAttributeName = "SECM:GetAttributeIdForAttributeName";
        public const string GetAuditHistoryAtLegLevel = "SECM:GetAuditHistoryAtLegLevel";
        public const string GetLegAttributesByLegDisplayName = "SECM:GetLegAttributesByLegDisplayName";
        public const string GetLegAuditAndTimeSeriesInfo = "SECM:GetLegAuditAndTimeSeriesInfo";
        public const string GetRealTimePreferenceMasterId = "SECM:GetRealTimePreferenceMasterId";
        public const string GetDuplicateSecuritiesCreateUpdate = "SECM:GetDuplicateSecuritiesCreateUpdate";
        public const string CheckSectypeSupportLegAndUnderLyer = "SECM:CheckSectypeSupportLegAndUnderLyer";
        public const string GetAllAttributesBySecurityTypeName = "SECM:GetAllAttributesBySectypeName";
        public const string GetSecurityCreationDateForAsOfView = "SECM:GetSecurityCreationDateForAsOfView";
        public const string GetAllUsers = "SECM:SECM_GetAllUsers";
        public const string DeleteTemplate = "SECM:SECM_DeleteTemplate";
        public const string AddUpdateCloneability = "SECM:SECM_AddUpdateCloneability";
        public const string InsertTaskStatusDetails = "SECMV:SECMV_InsertTaskStatusDetails";
    }

    public partial class CommonDALWrapper
    {
        #region private member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("CommonDALWrapper");
        #endregion

        public static RDBConnectionManager GetConnectionManager(string connectionType, bool useTransaction, IsolationLevel isolationLevel)
        {
            mLogger.Debug("Start-> GetConnectionManager");
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));

                if (useTransaction)
                {
                    mDbConn.UseTransaction = true;
                    mDbConn.IsolationLevel = isolationLevel;
                }

            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End-> GetConnectionManager");
            }
            return mDbConn;
        }

        public static RDBConnectionManager PutConnectionManager(RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start-> PutConnectionManager");
            try
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);

            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End-> PutConnectionManager");
            }
            return mDbConn;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the select query.
        /// </summary>
        /// <param name="queryID">The query ID.</param>
        /// <param name="paramaters">The paramaters.</param>
        /// <param name="connectionType">Type of the connection.</param>
        /// <returns></returns>
        public static DataSet ExecuteSelectQuery(string queryID, RHashlist paramaters, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing SELECT Query");
            DataSet dsDataSet = null;

            try
            {
                if (paramaters != null && paramaters.Count > 0)
                {
                    for (var i = 0; i < paramaters.Keys.Count; i++)
                    {
                        if (paramaters[i] != null && paramaters[i] != DBNull.Value && paramaters[i].GetType() == typeof(string))
                            paramaters[i] = DALWrapperAppend.Replace(Convert.ToString(paramaters[i]));
                    }
                }

                dsDataSet = mDbConn.ExecuteQuery(queryID, paramaters);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing SELECT Query");
            }
            return dsDataSet;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the select query.
        /// </summary>
        /// <param name="queryText">The query text.</param>
        /// <param name="connectionType">Type of the connection.</param>
        /// <returns></returns>
        public static DataSet ExecuteSelectQuery(string queryText, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing SELECT Query");
            DataSet dsDataSet = null;

            try
            {
                dsDataSet = mDbConn.ExecuteQuery(DALWrapperAppend.Replace(queryText), RQueryType.Select);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing SELECT Query");
            }
            return dsDataSet;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="queryID">The query ID.</param>
        /// <param name="paramaters">The paramaters.</param>
        /// <param name="connectionType">Type of the connection.</param>
        public static void ExecuteQuery(string queryID, RHashlist paramaters, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing Query");

            try
            {
                if (paramaters != null && paramaters.Count > 0)
                {
                    for (var i = 0; i < paramaters.Keys.Count; i++)
                    {
                        if (paramaters[i] != null && paramaters[i] != DBNull.Value && paramaters[i].GetType() == typeof(string))
                            paramaters[i] = DALWrapperAppend.Replace(Convert.ToString(paramaters[i]));
                    }
                }

                mDbConn.ExecuteQuery(queryID, paramaters);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing Query");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the query for Modify,Delete,Create
        /// </summary>
        /// <param name="queryID">The query ID.</param>
        /// <param name="paramaters">The paramaters.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <param name="connectionType">Type of the connection.</param>
        public static void ExecuteQuery(string queryID, RHashlist paramaters, bool state, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing Query");

            try
            {
                if (paramaters != null && paramaters.Count > 0)
                {
                    for (var i = 0; i < paramaters.Keys.Count; i++)
                    {
                        if (paramaters[i] != null && paramaters[i] != DBNull.Value && paramaters[i].GetType() == typeof(string))
                            paramaters[i] = DALWrapperAppend.Replace(Convert.ToString(paramaters[i]));
                    }
                }

                mDbConn.ExecuteQuery(queryID, paramaters, state);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing Query");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="queryText">The query text.</param>
        /// <param name="queryType">Type of the query.</param>
        /// <param name="connectionType">Type of the connection.</param>
        public static void ExecuteQuery(string queryText, CommonQueryType queryType, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing Query");
            RQueryType radQueryType;

            try
            {
                radQueryType = (RQueryType)Enum.Parse(typeof(RQueryType), queryType.ToString());
                mDbConn.ExecuteQuery(DALWrapperAppend.Replace(queryText), radQueryType);

                //if (queryType == RQueryType.Insert)
                //    mDbConn.ExecuteQuery(queryText, RQueryType.Insert);
                //else if (queryType == RQueryType.Update)
                //    mDbConn.ExecuteQuery(queryText, RQueryType.Update);
                //else if (queryType == RQueryType.Delete)
                //    mDbConn.ExecuteQuery(queryText, RQueryType.Delete);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing Query");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the proceedure
        /// </summary>
        /// <param name="queryID">The query ID.</param>
        /// <param name="paramaters">The paramaters.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static RHashlist ExecuteProcedure(string queryID, RHashlist paramaters, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing Procedure");

            try
            {
                if (paramaters != null && paramaters.Count > 0)
                {
                    for (var i = 0; i < paramaters.Keys.Count; i++)
                    {
                        if (paramaters[i] != null && paramaters[i] != DBNull.Value && paramaters[i].GetType() == typeof(string))
                            paramaters[i] = DALWrapperAppend.Replace(Convert.ToString(paramaters[i]));
                    }
                }

                return mDbConn.ExecuteProcedure(queryID, paramaters);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing Procedure");
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="connectionType">Type of the connection.</param>
        /// <returns></returns>
        public static DataSet GetTableSchema(string tableName, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start -> Getting Table Schema");

            try
            {
                return mDbConn.ExecuteQueryToGetSchema(DALWrapperAppend.Replace(tableName));
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                mLogger.Debug("End -> Getting Table Schema");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the bulk upload.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="dtTable">The dt table.</param>
        /// <param name="connectionType">Type of the connection.</param>
        public static void ExecuteBulkUpload(string tableName, DataTable dtTable, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start -> Bulk Uploading");

            try
            {
                mDbConn.ExecuteBulkCopy(DALWrapperAppend.Replace(tableName), dtTable, 100000);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                mLogger.Debug("End -> Bulk Uploading");
            }
        }

        public static void ExecuteBulkUpload(string tableName, DataTable dtTable, bool keepIdentity, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start -> Bulk Uploading");

            try
            {
                mDbConn.ExecuteBulkCopy(DALWrapperAppend.Replace(tableName), dtTable, 100000, keepIdentity);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                mLogger.Debug("End -> Bulk Uploading");
            }
        }

        public static void ExecuteBulkUpload(string tableName, DataTable dtTable, bool keepIdentity, Dictionary<string, string> columnMappings, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start -> Bulk Uploading");

            try
            {
                mDbConn.ExecuteBulkCopy(DALWrapperAppend.Replace(tableName), dtTable, 100000, keepIdentity, columnMappings);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                mLogger.Debug("End -> Bulk Uploading");
            }
        }

        public static void ExecuteBulkUpload(string tableName, DataTable dtTable, Dictionary<string, string> columnMappings, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start -> Bulk Uploading");

            try
            {
                mDbConn.ExecuteBulkCopy(DALWrapperAppend.Replace(tableName), dtTable, 100000, columnMappings);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                mLogger.Debug("End -> Bulk Uploading");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the select query.
        /// </summary>
        /// <param name="queryID">The query ID.</param>
        /// <param name="paramaters">The paramaters.</param>
        /// <param name="connectionType">Type of the connection.</param>
        /// <returns></returns>
        public static DataSet ExecuteSelectQuery(string queryID, RHashlist paramaters, string connectionType)
        {
            mLogger.Debug("Start->Executing SELECT Query");
            DataSet dsDataSet = null;
            RDBConnectionManager mDbConn = null;

            try
            {
                if (paramaters != null && paramaters.Count > 0)
                {
                    for (var i = 0; i < paramaters.Keys.Count; i++)
                    {
                        if (paramaters[i] != null && paramaters[i] != DBNull.Value && paramaters[i].GetType() == typeof(string))
                            paramaters[i] = DALWrapperAppend.Replace(Convert.ToString(paramaters[i]));
                    }
                }

                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));

                dsDataSet = mDbConn.ExecuteQuery(queryID, paramaters);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End->Executing SELECT Query");
            }
            return dsDataSet;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the select query.
        /// </summary>
        /// <param name="queryText">The query text.</param>
        /// <param name="connectionType">Type of the connection.</param>
        /// <returns></returns>
        public static DataSet ExecuteSelectQuery(string queryText, string connectionType)
        {
            mLogger.Debug("Start->Executing SELECT Query");
            DataSet dsDataSet = null;
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));

                dsDataSet = mDbConn.ExecuteQuery(DALWrapperAppend.Replace(queryText), RQueryType.Select);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End->Executing SELECT Query");
            }
            return dsDataSet;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="queryID">The query ID.</param>
        /// <param name="paramaters">The paramaters.</param>
        /// <param name="connectionType">Type of the connection.</param>
        public static void ExecuteQuery(string queryID, RHashlist paramaters, string connectionType)
        {
            mLogger.Debug("Start->Executing Query");
            RDBConnectionManager mDbConn = null;

            try
            {
                if (paramaters != null && paramaters.Count > 0)
                {
                    for (var i = 0; i < paramaters.Keys.Count; i++)
                    {
                        if (paramaters[i] != null && paramaters[i] != DBNull.Value && paramaters[i].GetType() == typeof(string))
                            paramaters[i] = DALWrapperAppend.Replace(Convert.ToString(paramaters[i]));
                    }
                }

                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));

                mDbConn.ExecuteQuery(queryID, paramaters);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End->Executing Query");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the query for Modify,Delete,Create
        /// </summary>
        /// <param name="queryID">The query ID.</param>
        /// <param name="paramaters">The paramaters.</param>
        /// <param name="state">if set to <c>true</c> [state].</param>
        /// <param name="connectionType">Type of the connection.</param>
        public static void ExecuteQuery(string queryID, RHashlist paramaters, bool state, string connectionType)
        {
            mLogger.Debug("Start->Executing Query");
            RDBConnectionManager mDbConn = null;

            try
            {
                if (paramaters != null && paramaters.Count > 0)
                {
                    for (var i = 0; i < paramaters.Keys.Count; i++)
                    {
                        if (paramaters[i] != null && paramaters[i] != DBNull.Value && paramaters[i].GetType() == typeof(string))
                            paramaters[i] = DALWrapperAppend.Replace(Convert.ToString(paramaters[i]));
                    }
                }

                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));

                mDbConn.ExecuteQuery(queryID, paramaters, state);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End->Executing Query");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="queryText">The query text.</param>
        /// <param name="queryType">Type of the query.</param>
        /// <param name="connectionType">Type of the connection.</param>
        public static void ExecuteQuery(string queryText, CommonQueryType queryType, string connectionType)
        {
            mLogger.Debug("Start->Executing Query");
            RDBConnectionManager mDbConn = null;
            RQueryType radQueryType;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));

                radQueryType = (RQueryType)Enum.Parse(typeof(RQueryType), queryType.ToString());
                mDbConn.ExecuteQuery(DALWrapperAppend.Replace(queryText), radQueryType);

                //if (queryType == RQueryType.Insert)
                //    mDbConn.ExecuteQuery(queryText, RQueryType.Insert);
                //else if (queryType == RQueryType.Update)
                //    mDbConn.ExecuteQuery(queryText, RQueryType.Update);
                //else if (queryType == RQueryType.Delete)
                //    mDbConn.ExecuteQuery(queryText, RQueryType.Delete);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End->Executing Query");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the proceedure
        /// </summary>
        /// <param name="queryID">The query ID.</param>
        /// <param name="paramaters">The paramaters.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static RHashlist ExecuteProcedure(string queryID, RHashlist paramaters, string connectionType)
        {
            mLogger.Debug("Start->Executing Procedure");
            RDBConnectionManager mDbConn = null;

            try
            {
                if (paramaters != null && paramaters.Count > 0)
                {
                    for (var i = 0; i < paramaters.Keys.Count; i++)
                    {
                        if (paramaters[i] != null && paramaters[i] != DBNull.Value && paramaters[i].GetType() == typeof(string))
                            paramaters[i] = DALWrapperAppend.Replace(Convert.ToString(paramaters[i]));
                    }
                }

                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));

                return mDbConn.ExecuteProcedure(queryID, paramaters);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End->Executing Procedure");
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="connectionType">Type of the connection.</param>
        /// <returns></returns>
        public static DataSet GetTableSchema(string tableName, string connectionType)
        {
            mLogger.Debug("Start -> Getting Table Schema");
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));
                return mDbConn.ExecuteQueryToGetSchema(DALWrapperAppend.Replace(tableName));

            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End -> Getting Table Schema");
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Executes the bulk upload.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="dtTable">The dt table.</param>
        /// <param name="connectionType">Type of the connection.</param>
        public static void ExecuteBulkUpload(string tableName, DataTable dtTable, string connectionType)
        {
            mLogger.Debug("Start -> Bulk Uploading");
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));
                mDbConn.ExecuteBulkCopy(DALWrapperAppend.Replace(tableName), dtTable, 100000);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End -> Bulk Uploading");
            }
        }

        public static void ExecuteBulkUpload(string tableName, DataTable dtTable, bool hasTableLock, string connectionType)
        {
            mLogger.Debug("Start -> Bulk Uploading");
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(RConfigReader.GetConfigAppSettings(connectionType));
                mDbConn.ExecuteBulkCopy(DALWrapperAppend.Replace(tableName), dtTable, 100000, hasTableLock, false);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End -> Bulk Uploading");
            }
        }

        public static void ExecuteBulkUpload(string tableName, DataTable dtTable, Dictionary<string, string> columnMappings, string connectionType)
        {
            mLogger.Debug("Start -> Bulk Uploading");
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));
                mDbConn.ExecuteBulkCopy(DALWrapperAppend.Replace(tableName), dtTable, 100000, columnMappings);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                if (mDbConn != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDbConn);
                mLogger.Debug("End -> Bulk Uploading");
            }
        }
    }

    public class CommonDALScriptsExecutor
    {
        #region private member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("CommonDALScriptsExecutor");
        #endregion

        public void RunDBScriptsFromFolder(string DirectoryLocation, string connectionId)
        {
            mLogger.Debug("Start -> RunDBScriptsFromFolder");

            try
            {
                mLogger.Debug("DirectoryLocation - " + DirectoryLocation + ", connectionId - " + connectionId);

                if (!Directory.Exists(DirectoryLocation))
                {
                    mLogger.Debug("Directory " + DirectoryLocation + " does not exist");
                    return;
                }

                List<FileInfo> currFileInfos = Directory.CreateDirectory(DirectoryLocation).GetFiles("*", SearchOption.AllDirectories).OrderBy(x => x.FullName).ToList();

                if (currFileInfos.Count == 0)
                {
                    mLogger.Debug("No files exist in directory");
                    return;
                }

                foreach (FileInfo fileInfo in currFileInfos)
                {
                    try
                    {
                        string currScriptName = fileInfo.FullName;
                        string currScriptText = string.Empty;

                        mLogger.Debug("Executing Script : " + currScriptName);

                        StreamReader scriptReader = null;

                        try
                        {
                            scriptReader = new StreamReader(currScriptName, Encoding.Default);

                            currScriptText = scriptReader.ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            if (scriptReader != null)
                            {
                                scriptReader.Close();
                                scriptReader.Dispose();
                            }
                        }

                        bool useTransaction = false;
                        if (currScriptText.IndexOf("CREATE DATABASE", StringComparison.OrdinalIgnoreCase) < 0 && currScriptText.IndexOf("ALTER DATABASE", StringComparison.OrdinalIgnoreCase) < 0)
                            useTransaction = true;

                        currScriptText = "IF ( (16 & @@OPTIONS) <> 16 ) SET ANSI_PADDING ON" + Environment.NewLine + "GO" + Environment.NewLine + Environment.NewLine + currScriptText + Environment.NewLine + Environment.NewLine + "GO" + Environment.NewLine + "IF ( (16 & @@OPTIONS) = 16 ) SET ANSI_PADDING OFF";

                        var connection = CreateDBConnection(connectionId);
                        ServerConnection serverConn = new ServerConnection(connection);
                        Server server = new Server(serverConn);
                        using (SqlConnection sqlConn = server.ConnectionContext.SqlConnectionObject)
                        {
                            if (!sqlConn.State.Equals(ConnectionState.Open))
                            {
                                sqlConn.Open();
                            }
                            try
                            {
                                if (useTransaction)
                                    server.ConnectionContext.BeginTransaction();

                                server.ConnectionContext.StatementTimeout = 0;
                                server.ConnectionContext.ExecuteNonQuery(currScriptText);

                                mLogger.Debug("Successfully executed script " + currScriptName);

                                if (useTransaction)
                                    server.ConnectionContext.CommitTransaction();
                            }
                            catch (Exception ex)
                            {
                                if (useTransaction)
                                    server.ConnectionContext.RollBackTransaction();

                                throw;
                            }
                            finally
                            {
                                sqlConn.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("Error while executing scripts -> " + Environment.NewLine + ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("End -> RunDBScriptsFromFolder");
            }
        }

        public SqlConnection CreateDBConnection(string connectionId)
        {
            SqlConnection connection = null;
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(connectionId);

                connection = new SqlConnection(mDbConn.ConnectionString);

                connection.Open();
            }
            catch (SqlException ex)
            {
                throw;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }

                if (mDbConn != null)
                    CommonDALWrapper.PutConnectionManager(mDbConn);
            }
            return connection;
        }
    }
}
