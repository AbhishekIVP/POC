using com.ivp.commom.commondal;
using com.ivp.common.Migration;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.RUserManagement;
using com.ivp.rad.utils;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.ivp.common
{
    public class RMModelerDBManager
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMModelerDBManager");
        private RDBConnectionManager mDBCon = null;

        public RMModelerDBManager()
        {
        }
        public RMModelerDBManager(RDBConnectionManager conMgr)
        {
            this.mDBCon = conMgr;
        }

        #region Download entire modeler configuration
        public ObjectSet GetModelerConfiguration(int moduleId, List<int> entityType = null, bool runSelected = false)
        {
            try
            {
                mLogger.Debug("RMModelerDBManager -> GetModelerConfiguration -> Start");

                ObjectSet dsExcel = null;
                string entityTypes = string.Empty;

                if (entityType != null && entityType.Count > 0)
                    entityTypes = string.Join(",", entityType);

                if (entityType == null)
                {
                    dsExcel = CommonDALWrapper.ExecuteSelectQueryObject(@"
                DECLARE @moduleID INT = " + moduleId + @", @entityType VARCHAR(MAX) = ''" + @", @runSelected BIT = '" + runSelected + @"'
                EXEC [IVPRefmaster].[dbo].[REFM_DownloadModelerConfiguration] @entityType,@moduleID,@runSelected  
                ", ConnectionConstants.RefMaster_Connection);
                }
                else
                {
                    dsExcel = CommonDALWrapper.ExecuteSelectQueryObject(@"
                DECLARE @moduleID INT = " + moduleId + @", @entityType VARCHAR(MAX) = '" + entityTypes + @"' , @runSelected BIT = '" + runSelected + @"'
                EXEC [IVPRefmaster].[dbo].[REFM_DownloadModelerConfiguration] @entityType,@moduleID,@runSelected    
                ", ConnectionConstants.RefMaster_Connection);
                }

                return dsExcel;

            }
            catch
            {
                mLogger.Debug("RMModelerDBManager -> GetModelerConfiguration -> Error");
                throw;
            }
            finally
            {
                mLogger.Debug("RMModelerDBManager -> GetModelerConfiguration -> End");
            }
        }

        #endregion

        #region Insert and update entity types methods
        public RHashlist AddEntityType(RMEntityTypeInfo objEntityTypeInfo, RDBConnectionManager conMgr = null)
        {
            //bool result;
            RHashlist result = new RHashlist();

            mLogger.Debug("Start->AddEntityType");
            RHashlist mlist = new RHashlist();
            mlist.Add("structureTypeID", objEntityTypeInfo.StructureTypeID);
            mlist.Add("derivedFromEntityTypeID", objEntityTypeInfo.DerivedFromEntityTypeID);
            mlist.Add("entityTypeID", objEntityTypeInfo.EntityTypeID);
            mlist.Add("isOneToOne", objEntityTypeInfo.IsOneToOne);
            mlist.Add("hasParent", objEntityTypeInfo.HasParent);
            mlist.Add("entityTypeName", objEntityTypeInfo.EntityTypeName);
            mlist.Add("userName", objEntityTypeInfo.LastModifiedBy);
            mlist.Add("entityName", "NULL");
            mlist.Add("accountID", objEntityTypeInfo.AccountId);
            mlist.Add("dynamicName", RMModelerConstants.TABLE_START_NAME);
            mlist.Add("isvector", objEntityTypeInfo.IsVector);
            mlist.Add("allowed_users", objEntityTypeInfo.AllowedUsers);
            mlist.Add("allowed_groups", objEntityTypeInfo.AllowedGroups);
            mlist.Add("module_id", objEntityTypeInfo.ModuleID);

            try
            {
                RHashlist output = null;
                if (conMgr == null)
                {
                    output = CommonDALWrapper.ExecuteProcedure("REFM:Refm_CreateEntityType", mlist, ConnectionConstants.RefMaster_Connection);
                    if (objEntityTypeInfo.StructureTypeID == 3)
                        AddNewLeg(objEntityTypeInfo.DerivedFromEntityTypeID, Convert.ToInt32(output[1]));
                    if (output.Count == 4 && !string.IsNullOrEmpty(objEntityTypeInfo.Tags))
                    {
                        mlist.Clear();
                        mlist.Add("tags", objEntityTypeInfo.Tags);
                        mlist.Add("entityTypeId", output[1]);
                        mlist.Add("attributeId", DBNull.Value);
                        mlist.Add("username", objEntityTypeInfo.LastModifiedBy);
                        CommonDALWrapper.ExecuteProcedure("REFM:REFM_InsertTags", mlist, ConnectionConstants.RefMaster_Connection);
                    }
                }
                else
                {
                    output = CommonDALWrapper.ExecuteProcedure("REFM:Refm_CreateEntityType", mlist, conMgr);
                    if (objEntityTypeInfo.StructureTypeID == 3)
                        AddNewLeg(objEntityTypeInfo.DerivedFromEntityTypeID, Convert.ToInt32(output[1]), conMgr);
                    if (output.Count == 4 && !string.IsNullOrEmpty(objEntityTypeInfo.Tags))
                    {
                        mlist.Clear();
                        mlist.Add("tags", objEntityTypeInfo.Tags);
                        mlist.Add("entityTypeId", output[1]);
                        mlist.Add("attributeId", DBNull.Value);
                        mlist.Add("username", objEntityTypeInfo.LastModifiedBy);
                        CommonDALWrapper.ExecuteProcedure("REFM:REFM_InsertTags", mlist, conMgr);
                    }
                }

                result = output;
                              
            }
            catch (Exception ex)
            {
                result = null;
                mLogger.Error(ex.ToString());
                throw;
            }

            finally
            {
                mLogger.Debug("End-->AddEntityType");
            }
            return result;
        }
        public void AddNewLeg(int masterEntityId, int legId, RDBConnectionManager connMgr = null)
        {
            List<int> template_ids = new List<int>();
            DataTable dtLegDisplayOrder = new DataTable();
            //Dictionary<int, int> TemplateIdVsEntityId = new Dictionary<int, int>();

            bool connectionCreated = false;
            try
            {
                if (connMgr == null)
                {
                    connectionCreated = true;
                    string mDBConnectionId = RConfigReader.GetConfigAppSettings(ConnectionConstants.RefMaster_Connection);
                    connMgr = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                    connMgr.UseTransaction = true;
                }

                try
                {
                    template_ids = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT template_id FROM IVPRefMaster.dbo.ivp_refm_entity_template_master WHERE entity_type_id = {0} ", masterEntityId), connMgr).Tables[0].AsEnumerable().Select(x => x.Field<int>("template_id")).Distinct().ToList();
                    
                    dtLegDisplayOrder = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"SELECT * FROM IVPRefMaster.dbo.ivp_refm_entitytype_configured_legs_order WHERE template_id IN (" + string.Join(",", template_ids) + ")"), connMgr).Tables[0];
                    template_ids = dtLegDisplayOrder.AsEnumerable().Select(x => x.Field<int>("template_id")).Distinct().ToList();

                    foreach (int template_id in template_ids)
                    {
                        connMgr.ExecuteQuery(DALWrapperAppend.Replace(string.Format(@"DECLARE @max_order INT = 0;

                                            SELECT @max_order = MAX(display_order) FROM IVPRefMaster.dbo.ivp_refm_entitytype_configured_legs_order WHERE template_id = {0}

                                            INSERT INTO IVPRefMaster.dbo.ivp_refm_entitytype_configured_legs_order(template_id,entity_type_id,display_order)
                                            VALUES ({0},{1},@max_order+1)", template_id, legId)), RQueryType.Insert);

                    }
                    if (connectionCreated)
                        connMgr.CommitTransaction();
                }
                catch (Exception ex)
                {
                    if (connMgr != null)
                        connMgr.RollbackTransaction();
                }
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                dtLegDisplayOrder = null;
                if (connectionCreated)
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(connMgr);
                    connMgr = null;
                }
            }

        }

        public bool UpdateEntityType(RMEntityTypeInfo objEntityTypeInfo, RDBConnectionManager conMgr = null)
        {
            bool result;
            mLogger.Debug("Start->UpdateEntityType");
            RHashlist mlist = new RHashlist();

            mlist.Add(RMModelerConstants.STRUCTURE_TYPE_ID, objEntityTypeInfo.StructureTypeID);
            mlist.Add(RMModelerConstants.DERIVED_FROM_ENTITY_TYPE, objEntityTypeInfo.DerivedFromEntityTypeID);
            mlist.Add(RMModelerConstants.IS_ONE_TO_ONE, objEntityTypeInfo.IsOneToOne);
            mlist.Add(RMModelerConstants.HAS_PARENT, objEntityTypeInfo.HasParent);
            mlist.Add(RMModelerConstants.LAST_MODIFIED_BY, objEntityTypeInfo.LastModifiedBy);
            mlist.Add(RMModelerConstants.ENTITY_DISPLAY_NAME, objEntityTypeInfo.EntityDisplayName);
            mlist.Add(RMModelerConstants.ENTITY_TYPE_ID, objEntityTypeInfo.EntityTypeID);
            mlist.Add(RMModelerConstants.ALLOWED_USERS, objEntityTypeInfo.AllowedUsers);
            mlist.Add(RMModelerConstants.ALLOWED_GROUPS, objEntityTypeInfo.AllowedGroups);
            mlist.Add(RMModelerConstants.MODULE_ID, objEntityTypeInfo.ModuleID);

            try
            {
                if (conMgr == null)
                {
                    ModifyFeedMappingView(objEntityTypeInfo.EntityTypeID, 0, 0, true);
                    CommonDALWrapper.ExecuteQuery("REFM:UpdateEntityType", mlist, ConnectionConstants.RefMaster_Connection);
                    ModifyFeedMappingView(objEntityTypeInfo.EntityTypeID, 0, 0, false);
                    mlist.Clear();
                    mlist.Add("tags", objEntityTypeInfo.Tags);
                    mlist.Add("entityTypeId", objEntityTypeInfo.EntityTypeID);
                    mlist.Add("attributeId", DBNull.Value);
                    mlist.Add("username", objEntityTypeInfo.LastModifiedBy);
                    CommonDALWrapper.ExecuteProcedure("REFM:REFM_InsertTags", mlist, ConnectionConstants.RefMaster_Connection);
                }
                else
                {
                    ModifyFeedMappingView(objEntityTypeInfo.EntityTypeID, 0, 0, true, conMgr);
                    CommonDALWrapper.ExecuteQuery("REFM:UpdateEntityType", mlist, conMgr);
                    ModifyFeedMappingView(objEntityTypeInfo.EntityTypeID, 0, 0, false, conMgr);
                    mlist.Clear();
                    mlist.Add("tags", objEntityTypeInfo.Tags);
                    mlist.Add("entityTypeId", objEntityTypeInfo.EntityTypeID);
                    mlist.Add("attributeId", DBNull.Value);
                    mlist.Add("username", objEntityTypeInfo.LastModifiedBy);
                    CommonDALWrapper.ExecuteProcedure("REFM:REFM_InsertTags", mlist, conMgr);
                }

                result = true;
            }
            catch (CommonDALException ex)
            {
                result = false;
                mLogger.Error(ex.ToString());
                throw;
            }            
            finally
            {
                mLogger.Debug("End-->UpdateEntityType");
            }
            return result;
        }

        public static void ModifyFeedMappingView(int entityTypeID, int feedSummaryID, int dataSourceID, bool dropView, RDBConnectionManager mDbConMgr = null)
        {
            try
            {
                mLogger.Debug("RMModelerDBManager: ModifyFeedMappingView -> Start");

                string query = " EXEC REFM_ModifyFeedMappingViews " + dataSourceID + ", " + feedSummaryID + ", " + entityTypeID + ", " + dropView + " ";

                if (mDbConMgr != null)
                    CommonDALWrapper.ExecuteSelectQuery(query, mDbConMgr);
                else
                    CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);


            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("RMModelerDBManager: ModifyFeedMappingView -> End");
            }
        }

        #endregion

        #region Page configurations methods
        public string GetMaxAttributeDataLength(int entity_attribute_id)
        {
            mLogger.Debug("RMModelerDBManager : GetMaxAttributeDataLength -> Start");
            RHashlist hList = null;
            DataSet dsResult = null;
            string maxLength = "-1";
            try
            {
                hList = new RHashlist();
                hList.Add("entity_attribute_id", entity_attribute_id);

                dsResult = (DataSet)(CommonDALWrapper.ExecuteProcedure("REFM:GetMaxAttributeDataLength", hList, ConnectionConstants.RefMaster_Connection)["DataSet"]);

                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0] != null && dsResult.Tables[0].Rows.Count > 0)
                    maxLength = Convert.ToString(dsResult.Tables[0].Rows[0][0]);

                return maxLength;

            }
            catch (Exception ex)
            {
                mLogger.Error("RMModelerDBManager: GetMaxAttributeDataLength-> Error " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMModelerDBManager : GetMaxAttributeDataLength -> End");
                if (hList != null)
                {
                    hList.Clear();
                    hList = null;
                }
            }
        }

        public DataSet AddUpdateEntityTypeAttributes(List<RMEntityAttributeInfo> lstAttributesInfo, int entityTypeID, string userName,bool saveLookups = true, RDBConnectionManager objConnMgr = null)
        {
            mLogger.Debug("RMModelerDBManager : AddUpdateEntityTypeAttributes -> Start");
            string mDBConnectionId = RConfigReader.GetConfigAppSettings(ConnectionConstants.RefMaster_Connection);

            RDBConnectionManager connectionManager = null;
            try
            {
                if (objConnMgr == null)
                {
                    connectionManager = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                    connectionManager.UseTransaction = true;
                }
                else
                {
                    connectionManager = objConnMgr;
                }
                {

                    ModifyFeedMappingView(entityTypeID, 0, 0, true, connectionManager);

                    XElement sbATInfo = new RMModelerController().CreateXmlInfoFromAttributeInfo(lstAttributesInfo, entityTypeID);
                    RHashlist hList = new RHashlist();
                    hList.Add("valuexml", sbATInfo.ToString());
                    hList.Add("entityTypeId", entityTypeID);
                    hList.Add("userName", userName);
                    hList.Add("saveLookups", saveLookups);                    
                    DataSet outputDs = (DataSet)connectionManager.ExecuteProcedure("REFM:InsertUpdateAttributes", hList)["DataSet"];

                    ModifyFeedMappingView(entityTypeID, 0, 0, false, connectionManager);

                    if (objConnMgr == null)
                    {
                        connectionManager.CommitTransaction();
                    }
                    return outputDs;
                }
            }
            catch (Exception e)
            {
                if (objConnMgr == null)
                {
                    connectionManager.RollbackTransaction();
                }
                mLogger.Error(e.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RMModelerDBManager : AddUpdateEntityTypeAttributes -> End");
                if (objConnMgr == null)
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(connectionManager);
                }
            }
        }

        public DataSet AddUpdateLookups(List<RMEntityAttributeInfo> lstAttributesInfo, int entityTypeID, string entityTypeName, string userName,DateTime date, Dictionary<string,RMEntityDetailsInfo> entityDetailsInfo, RDBConnectionManager objConnMgr = null)
        {
            mLogger.Debug("RMModelerDBManager : AddUpdateLookups -> Start");
            string mDBConnectionId = RConfigReader.GetConfigAppSettings(ConnectionConstants.RefMaster_Connection);

            RDBConnectionManager connectionManager = null;
            try
            {
                if (objConnMgr == null)
                {
                    connectionManager = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                    connectionManager.UseTransaction = true;
                }
                else
                {
                    connectionManager = objConnMgr;
                }
                
                XElement sbATInfo = new RMModelerController().CreateLookupXmlInfoFromAttributeInfo(lstAttributesInfo, entityTypeName, entityDetailsInfo);
                RHashlist hList = new RHashlist();
                hList.Add("valuexml", sbATInfo.ToString());
                hList.Add("entityTypeId", entityTypeID);
                hList.Add("userName", userName);
                hList.Add("date", date);
                DataSet outputDs = (DataSet)connectionManager.ExecuteProcedure("REFM:REFM_SaveLookupDetails", hList)["DataSet"];
                    
                if (objConnMgr == null)
                {
                    connectionManager.CommitTransaction();
                }
                return outputDs;
            }
            catch (Exception e)
            {
                if (objConnMgr == null)
                {
                    connectionManager.RollbackTransaction();
                }
                mLogger.Error(e.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RMModelerDBManager : AddUpdateLookups -> End");
                if (objConnMgr == null)
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(connectionManager);
                }
            }
        }        

        #endregion

        #region Unique Keys 

        #endregion

        #region Layout Sheet methods

        //Add layout
        public Object SaveLayoutInfo(RMTemplateInfo LayoutInfo, action_type action, RDBConnectionManager objConnMgr = null)
        {
            mLogger.Debug("RMModelerDBManager:SaveLayoutInfo -> Start Saving Layout Data");

            RHashlist htParams = new RHashlist();
            CreateLayoutInfo layoutInfo = new CreateLayoutInfo();
            try
            {
                htParams = new RHashlist();
                htParams.Add("template_id", -1);
                htParams.Add("template_name", LayoutInfo.TemplateName.ToString());
                htParams.Add("entity_type_id", LayoutInfo.EntityTypeId);
                htParams.Add("template_type_id", LayoutInfo.TemplateTypeId);
                htParams.Add("dependent_id", LayoutInfo.DependentId);
                htParams.Add("user", LayoutInfo.CreatedBy);
                htParams.Add("action_type", action);           //insert data 
                htParams.Add("entity_states", LayoutInfo.EntityStates);
                if (objConnMgr == null)
                {
                    DataSet dtResult = (DataSet)CommonDALWrapper.ExecuteProcedure("REFM:ModifyUserBasedLayoutInfo", htParams, ConnectionConstants.RefMaster_Connection)["DataSet"];
                    if (dtResult.Tables[0].Rows.Count > 0)
                    {
                        layoutInfo.templateId = Convert.ToInt32(dtResult.Tables[0].Rows[0]["template_id"]);
                        layoutInfo.TabNameVsTabNameId = dtResult.Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["entity_tab_name"]), y => new TabDetails()
                        {
                            tabName = Convert.ToString(y["entity_tab_name"]),
                            tabNameId = Convert.ToInt32(y["entity_tab_name_id"]),
                            tabOrder = Convert.ToInt32(y["tab_index"])
                        });
                    }
                }
                else
                {
                    DataSet dtResult = (DataSet)CommonDALWrapper.ExecuteProcedure("REFM:ModifyUserBasedLayoutInfo", htParams, objConnMgr)["DataSet"];
                    if (dtResult.Tables[0].Rows.Count > 0)
                    {
                            layoutInfo.templateId = Convert.ToInt32(dtResult.Tables[0].Rows[0]["template_id"]);
                            layoutInfo.TabNameVsTabNameId = dtResult.Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["entity_tab_name"]), y => new TabDetails()
                            {
                                tabName = Convert.ToString(y["entity_tab_name"]),
                                tabNameId = Convert.ToInt32(y["entity_tab_name_id"]),
                                tabOrder = Convert.ToInt32(y["tab_index"])
                            }); 
                    }
                }
                
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                htParams.Clear();
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("RMModelerDBManager:SaveLayoutInfo -> End Saving Layout Data");
            }
            return layoutInfo;
        }

        //Update layout
        public void UpdateLayoutInfo(RMTemplateInfo LayoutInfo, action_type action, RDBConnectionManager objConnMgr = null)
        {
            mLogger.Debug("RMModelerDBManager:UpdateLayoutInfo -> Start Updating Layout Data");
            RHashlist htParams = new RHashlist();

            try
            {
                htParams = new RHashlist();
                htParams.Add("template_id", LayoutInfo.TemplateId);
                htParams.Add("template_name", LayoutInfo.TemplateName.ToString());
                htParams.Add("entity_type_id", LayoutInfo.EntityTypeId);
                htParams.Add("template_type_id", LayoutInfo.TemplateTypeId);
                htParams.Add("dependent_id", LayoutInfo.DependentId);
                htParams.Add("user", LayoutInfo.CreatedBy);
                htParams.Add("action_type", action);           //update data 
                htParams.Add("entity_states", LayoutInfo.EntityStates);
                if(objConnMgr == null)
                    CommonDALWrapper.ExecuteProcedure("REFM:ModifyUserBasedLayoutInfo", htParams, ConnectionConstants.RefMaster_Connection);
                else
                    CommonDALWrapper.ExecuteProcedure("REFM:ModifyUserBasedLayoutInfo", htParams, objConnMgr);

            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                htParams.Clear();
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("RMModelerDBManager:UpdateLayoutInfo -> End Updating Layout Data");
            }

        }

        public Dictionary<string,List<int>> GetPossibleLayoutStates()
        {
            Dictionary<string, List<int>> possStates = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

            DataTable dt = CommonDALWrapper.ExecuteSelectQuery("select entity_state, entity_type_id from IVPRefMaster.dbo.ivp_refm_entity_states; ", ConnectionConstants.RefMaster_Connection).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
                possStates = dt.AsEnumerable().GroupBy(x => Convert.ToString(x["entity_state"]), StringComparer.OrdinalIgnoreCase).ToDictionary(y => y.Key, y => y.Select( z=> Convert.ToInt32(z["entity_type_id"])).ToList(), StringComparer.OrdinalIgnoreCase);
            return possStates;
        }

        #endregion

        public void InsertUpdateLayoutPreference(List<RMTemplatePreferenceInfo> lstPrefInfo, List<int> lstEntityTypeIds, string userName, RDBConnectionManager mDbConn = null)
        {
            try
            {

                if (lstPrefInfo != null && lstPrefInfo.Count > 0)
                {
                    if (mDbConn == null)
                        mDbConn = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, System.Data.IsolationLevel.RepeatableRead);

                    string deleteRecords = @"delete pref 
									from IVPRefMaster.dbo.ivp_refm_entity_template_master mas
                                    inner join IVPRefMaster.dbo.ivp_refm_entity_template_preference pref
                                    on mas.template_id = pref.template_id
                                    Inner join IVPRefMaster.dbo.REFM_GetList2Table('" + string.Join(",", lstEntityTypeIds) + @"', ',') tab
                                    on tab.item = mas.entity_type_id;
                                    ";

                    CommonDALWrapper.ExecuteSelectQuery(deleteRecords, mDbConn);

                    //user_name,template_id,created_by,created_on,last_modified_by,last_modified_on
                    DataTable dtUpload = new DataTable();
                    dtUpload.Columns.Add("user_name");
                    dtUpload.Columns.Add("template_id");
                    dtUpload.Columns.Add("created_by");
                    dtUpload.Columns.Add("created_on");
                    dtUpload.Columns.Add("last_modified_by");
                    dtUpload.Columns.Add("last_modified_on");

                    foreach (var info in lstPrefInfo)
                    {
                        DataRow dr = dtUpload.NewRow();
                        dr["user_name"] = info.UserName;
                        dr["template_id"] = info.TemplateId;
                        dr["created_by"] = userName;
                        dr["last_modified_by"] = userName;
                        dr["created_on"] = DateTime.Now;
                        dr["last_modified_on"] = DateTime.Now;
                        dtUpload.Rows.Add(dr);
                    }

                    CommonDALWrapper.ExecuteBulkUpload("IVPRefMaster.dbo.ivp_refm_entity_template_preference", dtUpload, mDbConn);

                }

            }
            catch (Exception e)
            {
                //con.RollbackTransaction();
                throw e;
            }
            finally
            {
                //SMDALWrapper.PutConnectionManager(con);
            }
        }

        public Dictionary<string, Dictionary<string, string>> getAllGroupLayout(out Dictionary<string, Dictionary<string, List<string>>> dictEntityTypeVsGroupLayoutVsEntityStates)
        {
            Dictionary<string, Dictionary<string, string>> dictEntityTypeVsGroupLayoutVsUser = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            dictEntityTypeVsGroupLayoutVsEntityStates = new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);

            string query = string.Format(@"select etype.entity_display_name,mas.template_name, mas.dependent_id, mas.entity_states
                from IVPRefMaster.dbo.ivp_refm_entity_template_master mas
                inner join IVPRefMaster.dbo.ivp_refm_entity_type etype
                on etype.entity_type_id = mas.entity_type_id and etype.is_active = 1 and mas.is_active = 1 and mas.template_type_id = 2");

            ObjectTable ds = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection).Tables[0];

            if (ds != null && ds.Rows.Count() > 0)
            {
                dictEntityTypeVsGroupLayoutVsUser = ds.Rows.AsEnumerable().GroupBy(x => Convert.ToString(x["entity_display_name"]), StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.Key, y => y.ToDictionary(z => Convert.ToString(z["template_name"]), z => Convert.ToString(z["dependent_id"]), StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);

                dictEntityTypeVsGroupLayoutVsEntityStates = ds.Rows.AsEnumerable().GroupBy(x => Convert.ToString(x["entity_display_name"]), StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.Key, y => y.ToDictionary(z => Convert.ToString(z["template_name"]), z => Convert.ToString(z["entity_states"]).Split(',').ToList(), StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
            }

            return dictEntityTypeVsGroupLayoutVsUser;
        }

        #region Tab Management

        //sace new tab
        public int SaveEntityTabDetails(int entityTabId, RMTabManagementInfo objTabManagementInfo, RDBConnectionManager objConnMgr = null)
        {
            mLogger.Debug("RMModelerDBManager:SaveEntityTabDetails -> Start Saving Entity Tab Details");
            RHashlist htParams = new RHashlist();
            int tabNameId = 0;
            try
            {
                htParams = new RHashlist();
                htParams.Add(RMTableRefmTabManagement.ENTITY_TAB_ID, entityTabId);
                htParams.Add(RMTableRefmTabManagement.ENTITY_TAB_NAME, objTabManagementInfo.TabName);
                htParams.Add(RMTableRefmTabManagement.TAB_INDEX, objTabManagementInfo.TabIndex);
                htParams.Add(RMDBCommonConstantsInfo.CREATED_BY, objTabManagementInfo.CreatedBy);
                htParams.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objTabManagementInfo.LastModifiedBy);
                htParams.Add("template_id", objTabManagementInfo.TemplateId);

                if (objConnMgr == null)
                {
                    DataSet dtResult = CommonDALWrapper.ExecuteSelectQuery("REFM:SaveEntityTabDetails", htParams, ConnectionConstants.RefMaster_Connection);
                    if (dtResult.Tables[0].Rows.Count > 0)
                        tabNameId = Convert.ToInt32(dtResult.Tables[0].Rows[0]["entity_tab_name_id"]);
                }
                else
                {
                    DataSet dtResult = CommonDALWrapper.ExecuteSelectQuery("REFM:SaveEntityTabDetails", htParams, objConnMgr);
                    if (dtResult.Tables[0].Rows.Count > 0)
                        tabNameId = Convert.ToInt32(dtResult.Tables[0].Rows[0]["entity_tab_name_id"]);

                }
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                htParams.Clear();
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("RMModelerDBManager:SaveEntityTabDetails -> End Saving Entity Tab Details");
            }
            return tabNameId;
        }

        //update 
        public void UpdateEntityTabDetails(int entityTabId, RMTabManagementInfo objTabManagementInfo, RDBConnectionManager objConnMgr = null)
        {
            mLogger.Debug("RMModelerDBManager:UpdateEntityTabDetails -> Start Updateing Entity Tab Details");
            RHashlist htParams = new RHashlist();
            try
            {
                htParams = new RHashlist();
                htParams.Add(RMTableRefmTabManagement.ENTITY_TAB_ID, entityTabId);
                htParams.Add(RMTableRefmTabManagement.ENTITY_TAB_NAME_ID, objTabManagementInfo.TabNameId);
                htParams.Add(RMTableRefmTabManagement.ENTITY_TAB_NAME, objTabManagementInfo.TabName);
                htParams.Add(RMTableRefmTabManagement.TAB_INDEX, objTabManagementInfo.TabIndex);
                htParams.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objTabManagementInfo.LastModifiedBy);
                htParams.Add("template_id", objTabManagementInfo.TemplateId);
                if(objConnMgr == null)
                    CommonDALWrapper.ExecuteQuery("REFM:UpdateEntityTabDetails", htParams, ConnectionConstants.RefMaster_Connection);
                else
                    CommonDALWrapper.ExecuteQuery("REFM:UpdateEntityTabDetails", htParams, objConnMgr);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                htParams.Clear();
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("RMModelerDBManager:UpdateEntityTabDetails -> End Updateing Entity Tab Details");
            }
        }

        //get Tab Id
        public int GetEntityTabIdByTypeId(int entityTypeId, int templateId)
        {
            mLogger.Debug("RMModelerDBManager:UpdateEntityTabDetails -> GetEntityTabIdByTypeId");
            DataSet dsResult = new DataSet();
            RHashlist htParams = new RHashlist();
            try
            {
                htParams = new RHashlist();
                htParams.Add(RMModelerConstants.ENTITY_TYPE_ID, entityTypeId);
                htParams.Add("template_id", templateId);

                dsResult = CommonDALWrapper.ExecuteSelectQuery("REFM:GetEntityTabIdByTypeId", htParams, ConnectionConstants.RefMaster_Connection);
                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                {
                    return int.Parse(dsResult.Tables[0].Rows[0][0].ToString());
                }
                else return 0;

            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                htParams.Clear();
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("RMModelerDBManager:UpdateEntityTabDetails -> End Updating Entity Tab Details");
            }
        }
        #endregion

        #region Layout Attribute Management
        public void SaveModifiedUIXml(RMAttributeManagementInfo objAttributeManagementInfo, RDBConnectionManager connMgr = null)
        {            
            mLogger.Debug("RMModelerDBManager:SaveModifiedUIXml -> Start Saving Modified UIXml");
            RHashlist htParams = new RHashlist();
            try
            {
                htParams = new RHashlist();
                htParams.Add(RMModelerConstants.ENTITY_TYPE_ID, objAttributeManagementInfo.EntityTypeId);
                htParams.Add("ui_xml", objAttributeManagementInfo.UIXml);
                htParams.Add(RMDBCommonConstantsInfo.LAST_MODIFIED_BY, objAttributeManagementInfo.LastModifiedBy);
                htParams.Add("template_id", objAttributeManagementInfo.TemplateId);
                if (connMgr == null)
                    CommonDALWrapper.ExecuteProcedure("REFM:Refm_SaveModifiedUIXml", htParams, ConnectionConstants.RefMaster_Connection);
                else
                    CommonDALWrapper.ExecuteProcedure("REFM:Refm_SaveModifiedUIXml", htParams, connMgr);
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw ex;
            }
            finally
            {
                htParams.Clear();
                if (htParams != null)
                    htParams = null;
                mLogger.Debug("RMModelerDBManager:SaveModifiedUIXml -> End Saving Modified UIXml");
            }
        }
        #endregion

        #region Leg order
        public bool SaveEntityTypeLegsDisplayOrder(int templateID, object entityTypeId, DataTable dtSaveAllSectypeLegs, RDBConnectionManager objConnMgr = null)
        {
            int count = 0;
            bool flag = false;
            mLogger.Debug("RMModelerDBManager : Start -> SaveSectypeLegsDisplayOrder");
            string mDBConnectionId = RConfigReader.GetConfigAppSettings(ConnectionConstants.RefMaster_Connection);

            RDBConnectionManager connectionManager = null;
            try
            {
                if (objConnMgr == null)
                {
                    connectionManager = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                    connectionManager.UseTransaction = true;
                }
                else
                {
                    connectionManager = objConnMgr;
                }
                DataTable configured_legs_priority = new DataTable();
                configured_legs_priority.Columns.Add("template_id", typeof(int));
                configured_legs_priority.Columns.Add("entity_type_id", typeof(int));
                configured_legs_priority.Columns.Add("display_order", typeof(int));

                foreach (var dr in dtSaveAllSectypeLegs.AsEnumerable())
                {
                    configured_legs_priority.Rows.Add(Convert.ToInt32(templateID), Convert.ToInt32(dr["entity_type_id"]), Convert.ToInt32(++count));
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO @tab VALUES(").Append(templateID).Append(");");


                if (configured_legs_priority.Rows.Count > 0)
                {
                    connectionManager.ExecuteQuery(string.Format(@"DECLARE @tab TABLE(template_id INT)
						{0}

						DELETE c
						FROM dbo.ivp_refm_entitytype_configured_legs_order c
						INNER JOIN @tab tab ON c.template_id = tab.template_id", sb.ToString()), RQueryType.Delete);

                    connectionManager.ExecuteBulkCopy("dbo.ivp_refm_entitytype_configured_legs_order", configured_legs_priority);
                }

                connectionManager.CommitTransaction();
                flag = true;
            }
            catch (Exception ex)
            {
                if (connectionManager != null)
                    connectionManager.RollbackTransaction();
                //modalMessageError.Show();
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(connectionManager);
                connectionManager = null;

                mLogger.Debug("RMModelerDBManager : End -> SaveSectypeLegsDisplayOrder");
            }
            return flag;
        }
        #endregion

        #region Page Configuration
        public void InsertDataForPageConfig(RMManageAttributeConfigurationInfo objRMManageAttributeConfigurationInfo)
        {
            mLogger.Debug("RMModelerDBManager : InsertDataForPageConfig Start -> Inserting Right side shuttle data ");
            RHashlist mList = new RHashlist();
            try
            {
                mList.Add(RMModelerConstants.PAGE_IDENTIFIER, objRMManageAttributeConfigurationInfo.PageIdentifier);
                mList.Add(RMModelerConstants.FUNCTIONALITY_IDENTIFIER, objRMManageAttributeConfigurationInfo.FunctionalityIdentifier);
                mList.Add(RMModelerConstants.ATTRIBUTE_IDS, objRMManageAttributeConfigurationInfo.AttributeIds);
                mList.Add(RMModelerConstants.ENTITY_TYPE_ID, objRMManageAttributeConfigurationInfo.EntityTypeId);
                CommonDALWrapper.ExecuteQuery("REFM:InsertPageConfig", mList, ConnectionConstants.RefMaster_Connection);
            }
            catch (Exception e)
            {
                mLogger.Error(e.Message);
                throw new Exception(e.Message, e);
            }            
            finally
            {
                mLogger.Debug("RMModelerDBManager : InsertDataForPageConfig -> End");
                if (mList != null)
                {
                    mList.Clear();
                    mList = null;
                }
            }
        }

        internal void DeleteFailedLookupAttribute(List<RMEntityAttributeInfo> failedList, int entityTypeId, string entityTypeName, string userName, IEnumerable<ObjectRow> currentRows, RDBConnectionManager objConnMgr = null)
        {
            mLogger.Debug("RMModelerDBManager : DeleteFailedLookupAttribute -> Start");
            string mDBConnectionId = RConfigReader.GetConfigAppSettings(ConnectionConstants.RefMaster_Connection);

            RDBConnectionManager connectionManager = null;
            try
            {
                if (objConnMgr == null)
                {
                    connectionManager = RDALAbstractFactory.DBFactory.GetConnectionManager(mDBConnectionId);
                    connectionManager.UseTransaction = true;
                }
                else
                {
                    connectionManager = objConnMgr;
                }

                foreach (RMEntityAttributeInfo info in failedList)
                {
                    string query = string.Format("EXEC [dbo].[Refm_DeleteAttributeFromEntityType] '{0}',{1},'{2}'", info.DisplayName, entityTypeId, userName);
                    connectionManager.ExecuteQuery(query, RQueryType.Select);

                    currentRows.Where(x => Convert.ToString(x[RMModelerConstants.Attribute_Name]).SRMEqualWithIgnoreCase(info.DisplayName) && Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName)).ToList().ForEach(row => {
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMAttributeSetupConstants.INVALID_REFERENCE_ATTRIBUTE_ERROR }, false);
                    });
                }
                if (objConnMgr == null)
                {
                    connectionManager.CommitTransaction();
                }                
            }
            catch (Exception e)
            {
                if (objConnMgr == null)
                {
                    connectionManager.RollbackTransaction();
                }
                mLogger.Error(e.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("RMModelerDBManager : DeleteFailedLookupAttribute -> End");
                if (objConnMgr == null)
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(connectionManager);
                }
            }
        }
        #endregion

        public ObjectTable GetAllTabsForTempate(string templateIds)
        {

            string query = string.Format(@"SELECT 
                            entity_tab_name AS[Tab Name],
                            entity_tab_name_id,
                            etyp.entity_display_name AS[Entity Type Name],
                            tmas.template_id,
                            tmas.template_name AS[Layout Name],
							tab_index AS [Tab Order]
                        FROM IVPRefMaster.dbo.ivp_refm_entity_tab_name tname
                        INNER JOIN IVPRefMaster.dbo.REFM_GetList2Table('{0}', ',') tab
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_template_master tmas
                        ON(tmas.template_id = tab.item AND tmas.is_active = 1)
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_type etyp
                        ON(etyp.entity_type_id = tmas.entity_type_id)
                        ON(tab.item = tname.template_id AND tname.is_active = 1)", templateIds);

            ObjectTable ds = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection).Tables[0];

            return ds;
        }

        public void UpdateLookupDisplayAttributes(Dictionary<string, RMEntityDetailsInfo> entityDetails, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> lookupDisplayAttributes)
        {
            StringBuilder queryText = new StringBuilder();
            foreach (string entityTypeName in lookupDisplayAttributes.Keys)
            {
                if (entityDetails.ContainsKey(entityTypeName))
                {
                    foreach (string attributeName in lookupDisplayAttributes[entityTypeName].Keys)
                    {
                        StringBuilder attr = new StringBuilder();
                        if (entityDetails[entityTypeName].Attributes.ContainsKey(attributeName))
                        {
                            attr.Append("UPDATE [IVPRefMaster].[dbo].[ivp_refm_entity_attribute] SET lookup_display_attributes = '");
                            foreach (string lookupType in lookupDisplayAttributes[entityTypeName][attributeName].Keys)
                            {
                                if (entityDetails.ContainsKey(lookupType))
                                {
                                    foreach (string lookupAttribute in lookupDisplayAttributes[entityTypeName][attributeName][lookupType])
                                    {
                                        if (entityDetails[lookupType].Attributes.ContainsKey(lookupAttribute))
                                        {
                                            attr.Append(entityDetails[lookupType].Attributes[lookupAttribute].EntityAttributeID + ",");
                                        }
                                        else
                                            attr = new StringBuilder();
                                    }
                                }
                                else
                                    attr = new StringBuilder();                                
                            }
                            if(attr.Length > 0)
                            {
                                attr.Remove(attr.Length -1, 1);
                                attr.Append("' WHERE entity_attribute_id = " + entityDetails[entityTypeName].Attributes[attributeName].EntityAttributeID);
                            }
                        }                        
                        queryText.Append(attr);
                        queryText.AppendLine();
                    }
                }                
            }

            if(queryText.Length > 0)
                CommonDALWrapper.ExecuteSelectQuery(queryText.ToString(), ConnectionConstants.RefMaster_Connection);
        }

        public void UpdateAttributeDisplayConfigurations(HashSet<RMAttributeDisplayConfigurationInfo> lstAttrDispInfo)
        {
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.RefMaster_Connection, true, IsolationLevel.RepeatableRead);
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("attribute_id", typeof(Int32));
                dt.Columns.Add("append_percentage", typeof(bool));
                dt.Columns.Add("show_entity_code", typeof(bool));
                dt.Columns.Add("order_by_attribute_id", typeof(Int32));
                dt.Columns.Add("order_by_attribute_name", typeof(string));
                dt.Columns.Add("allow_comma_formatting", typeof(bool));
                dt.Columns.Add("append_multiplier", typeof(bool));

                foreach (var info in lstAttrDispInfo)
                {
                    DataRow dr = dt.NewRow();
                    dr["attribute_id"] = info.AttributeId;
                    dr["append_percentage"] = info.AppendPercentage;
                    dr["show_entity_code"] = info.ShowEntityCode;
                    dr["order_by_attribute_id"] = info.OrderByAttributeId;
                    dr["order_by_attribute_name"] = info.OrderByAttributeName;
                    dr["allow_comma_formatting"] = info.AllowCommaFormatting;
                    dr["append_multiplier"] = info.AppendMultiplier;
                    dt.Rows.Add(dr);
                }

                CommonDALWrapper.ExecuteQuery(@"IF(OBJECT_ID('tempdb..#tempAttributeDisplayConf') IS NOT NULL)
                DROP TABLE #tempAttributeDisplayConf
                CREATE TABLE #tempAttributeDisplayConf(attribute_id INT,append_percentage BIT, show_entity_code BIT,order_by_attribute_id INT,order_by_attribute_name VARCHAR(500),allow_comma_formatting BIT,append_multiplier BIT);", CommonQueryType.Insert, con);

                CommonDALWrapper.ExecuteBulkUpload("#tempAttributeDisplayConf", dt, con);
                var query = string.Format(@"CREATE TABLE #AttrIdsToInsert(attribute_id INT);

                INSERT INTO #AttrIdsToInsert
                SELECT temp.attribute_id
                FROM #tempAttributeDisplayConf temp
                EXCEPT Select attribute_id from IVPRefMaster.dbo.ivp_refm_attribute_display_configuration

                UPDATE disp
                SET disp.append_percentage = temp.append_percentage,disp.show_entity_code = temp.show_entity_code,disp.order_by_attribute_id = temp.order_by_attribute_id,
                disp.order_by_attribute_name = temp.order_by_attribute_name, disp.allow_comma_formatting = temp.allow_comma_formatting, disp.append_multiplier = temp.append_multiplier
                FROM IVPRefMaster.dbo.ivp_refm_attribute_display_configuration disp
                INNER JOIN #tempAttributeDisplayConf temp
                on temp.attribute_id = disp.attribute_id;

                INSERT INTO IVPRefMaster.dbo.ivp_refm_attribute_display_configuration
                SELECT temp.*
                FROM #tempAttributeDisplayConf temp 
                INNER JOIN #AttrIdsToInsert attrInsert
                on attrInsert.attribute_id = temp.attribute_id
                
                DROP TABLE #tempAttributeDisplayConf                
                DROP TABLE #AttrIdsToInsert ");

                CommonDALWrapper.ExecuteSelectQuery(query.ToString(), con);
                //CommonDALWrapper.ExecuteSelectQuery(query.ToString(), con);

                con.CommitTransaction();
            }
            catch (Exception ex)
            {
                con.RollbackTransaction();
                throw ex;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(con);
            }

        }
        public DataSet GetAttributesForEntityType(List<int> entityTypeIds, string userName, EMModule module)
        {
            try
            {
                DataSet result;
                string queryText = string.Empty;
                queryText = "EXEC [IVPRefMaster].[dbo].[REFM_GetAttributesForEntityType] '" + string.Join(",", entityTypeIds) + "','" + userName + "'";

                if (this.mDBCon != null)
                {
                    result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);

                }
                else
                {
                    result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                }
                return result;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetAttributesForEntityType -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        public DataSet GetEntityTypes(List<int> entityTypeIds = null)
        {
            try
            {
                DataSet result;
                string queryText = string.Empty;
                queryText = @"SELECT ET.*,ST.is_master,ST.is_derived,ST.is_leg 
                                FROM IVPRefMaster.dbo.ivp_refm_entity_type ET
                                INNER JOIN IVPRefMaster.dbo.ivp_refm_structure_type ST
                                ON(ET.structure_type_id = ST.structure_type_id AND ET.is_active = 1 AND ST.is_active = 1)" +
                                ((entityTypeIds == null || entityTypeIds.Count == 0) ? string.Empty : "WHERE entity_type_id IN(" + string.Join(",", entityTypeIds) + ")'");

                if (this.mDBCon != null)
                {
                    result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);

                }
                else
                {
                    result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                }
                return result;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetAttributesForEntityType -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }
    }

    public class CreateLayoutInfo
    {
        public string templateName;
        public int templateId;
        public Dictionary<string, TabDetails> TabNameVsTabNameId;
    }
    public class TabDetails
    {
        public int tabNameId { get; set; }
        public int tabOrder { get; set; }
        public string tabName { get; set; }
    }


}