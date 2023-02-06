using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.commom.commondal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.rad.common;
using com.ivp.srmcommon;
using System.Data;
using System.Reflection;
using System.Xml.Linq;
using com.ivp.rad.controls.xruleeditor;
using com.ivp.rad.controls.xruleeditor.grammar;
using com.ivp.rad.utils;
using System.Text.RegularExpressions;
using com.ivp.SRMCommonModules;
using com.ivp.rad.transport;
using com.ivp.rad.RUserManagement;

namespace com.ivp.common.Migration
{
    public class RMEntityTypeModelerMigration
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMEntityTypeModelerMigration");
        const string COL_IS_INSERT = "@isInsert";
        const string COL_ENTITY_ID = "entity_type_id";
        ObjectSet dbEntityTypesDetails = null;

        public RMEntityTypeModelerMigration(ObjectSet dbEntityTypesDetails)
        {
            this.dbEntityTypesDetails = dbEntityTypesDetails;
        }

        public void SyncEntityTypes(ObjectSet deltaSet, ObjectTable existingEntityTypes, ObjectTable sourceTable, string userName, int moduleId, Dictionary<string, RMEntityDetailsInfo> entityDetails, Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo, Dictionary<string, string> hshUsers, List<string> hshGroups)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncEntityTypes->Start");
            bool isInsert = false;
            int entitytypeId = 0;
            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            RMEntityTypeSetupSync entityValidate = new RMEntityTypeSetupSync();
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.ENTITY_TYPE_ID, DataType = typeof(Int32), DefaultValue = -1 });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.ENTITY_TYPE_NAME, DataType = typeof(string), DefaultValue = string.Empty });
            new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);
            //Dictionary<string, string> hshUsers = SRMCommonRAD.GetAllDisplayNamevsUsersLoginName();
            //List<string> hshGroups = SRMCommonRAD.GetAllGroups();

            var assign = from db in existingEntityTypes.AsEnumerable()
                         join upl in sourceTable.AsEnumerable()
                         on ConvertToLower(db[RMModelerConstants.ENTITY_DISPLAY_NAME]) equals ConvertToLower(upl[RMModelerConstants.ENTITY_DISPLAY_NAME])
                         select AssignExistingEntityType(db, upl);

            assign.Count();

            sourceTable.AsEnumerable()
                .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList().ForEach(row =>
                {
                    RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                    string entityTypeName = string.Empty;
                    List<string> errorMessages = new List<string>();
                    try
                    {
                        bool isValid = true;
                        isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);
                        entityTypeName = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]);
                        string entityRealName = Convert.ToString(row[RMModelerConstants.ENTITY_TYPE_NAME]);
                        string tags = Convert.ToString(row[RMModelerConstants.Tags]);
                        string allowed_users = Convert.ToString(row[RMModelerConstants.Allowed_Users]);
                        string allowed_groups = Convert.ToString(row[RMModelerConstants.Allowed_Groups]);
                        string validationErrorMessage = string.Empty;
                        RHashlist NewEnitityDetails = new RHashlist();
                        List<string> lstUserNames = new List<string>();
                        List<string> lstGroupNames = new List<string>();

                        errorMessages.AddRange(entityValidate.validateEntityDisplayName(entityTypeName));
                        errorMessages.AddRange(entityValidate.validateTagName(tags));

                        if (errorMessages.Count > 0)
                        {
                            isValid = false;
                        }
                        else
                        {

                            string final_allowed_users = string.Empty;
                            string final_allowed_groups = string.Empty;
                            if (!string.IsNullOrEmpty(allowed_users))
                            {
                                lstUserNames = allowed_users.Split(',').ToList();
                                List<string> massageusers = new List<string>();
                                lstUserNames.ForEach(z =>
                                {
                                    if (hshUsers.ContainsKey(z.Trim()))
                                        massageusers.Add(hshUsers[z.Trim()]);
                                });

                                if (massageusers.Count > 0)
                                    final_allowed_users = string.Join(",", massageusers);
                            }
                            if (!string.IsNullOrEmpty(allowed_groups))
                            {
                                lstGroupNames = allowed_groups.Split(',').ToList();
                                List<string> massagegroups = new List<string>();
                                lstGroupNames.ForEach(z =>
                                {
                                    if (hshGroups.Contains(z.Trim()))
                                        massagegroups.Add(z.Trim());
                                });

                                if (massagegroups.Count > 0)
                                    final_allowed_groups = string.Join(",", massagegroups);
                            }
                            if (!entityDetails.ContainsKey(entityTypeName) && isInsert) //Add Entity type
                            {
                                if (allModulesEntityDetailsInfo.ContainsKey(entityTypeName))
                                {
                                    isValid = false;
                                    errorMessages.Add("Entity type name already exists");
                                }
                                else
                                {
                                    RMEntityTypeInfo entityInfo = new RMEntityTypeInfo();
                                    entityInfo.AccountId = 0;
                                    entityInfo.DerivedFromEntityTypeID = 0;
                                    entityInfo.EntityDisplayName = entityTypeName;
                                    entityInfo.EntityTypeName = entityTypeName;
                                    entityInfo.Tags = tags;
                                    entityInfo.AllowedUsers = final_allowed_users;
                                    entityInfo.AllowedGroups = final_allowed_groups;
                                    entityInfo.LastModifiedBy = userName;
                                    entityInfo.CreatedBy = userName;
                                    entityInfo.ModuleID = moduleId;
                                    entityInfo.EntityTypeID = 0;
                                    entityInfo.HasParent = false;
                                    entityInfo.IsOneToOne = false;
                                    entityInfo.IsVector = false;
                                    entityInfo.StructureTypeID = 2;
                                    NewEnitityDetails = new RMModelerDBManager().AddEntityType(entityInfo, mDBCon);
                                }

                            }
                            else //Update Entity type
                            {
                                entitytypeId = Convert.ToInt32(row[RMModelerConstants.ENTITY_TYPE_ID]);
                                RMEntityTypeInfo entityInfo = new RMEntityTypeInfo();
                                entityInfo.EntityTypeID = entitytypeId;
                                entityInfo.EntityDisplayName = entityTypeName;
                                entityInfo.Tags = tags;
                                entityInfo.AllowedUsers = final_allowed_users;
                                entityInfo.AllowedGroups = final_allowed_groups;
                                entityInfo.LastModifiedBy = userName;
                                entityInfo.ModuleID = moduleId;
                                entityInfo.HasParent = false;
                                entityInfo.IsOneToOne = false;
                                entityInfo.StructureTypeID = 2;
                                entityInfo.DerivedFromEntityTypeID = 0;
                                new RMModelerDBManager().UpdateEntityType(entityInfo, mDBCon);
                            }
                            if (isValid)
                            {
                                mDBCon.CommitTransaction();
                                new RMCommonMigration().SetPassedRow(row, string.Empty);
                                updateEntityInfo(entityDetails, entityTypeVstemplateInfo, entityTypeName, NewEnitityDetails, allModulesEntityDetailsInfo);
                            }
                            else
                            {
                                mDBCon.RollbackTransaction();
                                new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                            }
                        }



                    }
                    catch (Exception ex)
                    {
                        if (mDBCon != null)
                            mDBCon.RollbackTransaction();

                        mLogger.Error("RMEntityTypeModelerMigration-SyncEntityTypes->Error " + ex.Message);
                        //FailEntityDependencies(deltaSet, entityTypeName);
                        FailAllDependencies(deltaSet, entityTypeName, null, false, false, false, false, false, false, false, false, false, false, false);

                        //Code to fail dependent rows in other tables
                    }
                    finally
                    {
                        if (mDBCon != null)
                        {
                            RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                            mDBCon = null;
                        }
                    }
                });

            //Remove static columns
            List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMModelerConstants.ENTITY_TYPE_ID, RMModelerConstants.ENTITY_TYPE_NAME };
            new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);

            mLogger.Error("RMEntityTypeModelerMigration-SyncEntityTypes->End");
        }


        public void SyncAttributeRules(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingDBRules, ObjectTable sourceTable, string userName, Dictionary<string, RMEntityDetailsInfo> entityDetailsInfo)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->Start");

            RMCommonController controller = new RMCommonController();

            var ruleGroups = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS]))).GroupBy(x => new { ename = ConvertToLower(x[RMModelerConstants.ENTITY_DISPLAY_NAME]), aname = ConvertToLower(x[RMModelerConstants.Attribute_Name]), rtype = ConvertToLower(x[RMModelerConstants.Rule_Type]) });

            foreach (var group in ruleGroups)
            {
                List<string> errorMessages = new List<string>();
                RADXRuleGrammarInfo ruleGrammarInfo = null;
                RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                int ruleSetId = 0;
                string entityTypeName = group.Key.ename;
                string attributeName = group.Key.aname;
                string ruleType = group.Key.rtype;

                if (!entityDetailsInfo.Keys.SRMContainsWithIgnoreCase(entityTypeName))
                {
                    errorMessages.Add(RMAttributeSetupConstants.INVALID_ENITY_TYPE);
                }
                else if (!entityDetailsInfo[entityTypeName].Attributes.Keys.SRMContainsWithIgnoreCase(attributeName) && !entityDetailsInfo[entityTypeName].Legs.Any(x => x.Value.Attributes.Keys.SRMContainsWithIgnoreCase(attributeName)))
                {
                    errorMessages.Add(RMAttributeSetupConstants.INVALID_ATTRIBUTE);
                }

                if (errorMessages.Count == 0)
                {
                    int entityTypeId = entityDetailsInfo[entityTypeName].EntityTypeID;
                    int attributeId = -1;

                    attributeId = entityDetailsInfo[entityTypeName].Attributes.Keys.SRMContainsWithIgnoreCase(attributeName) ? entityDetailsInfo[entityTypeName].Attributes[attributeName].EntityAttributeID : -1;

                    if (attributeId == -1)
                    {
                        attributeId = entityDetailsInfo[entityTypeName].Legs.Where(x => x.Value.Attributes.Keys.SRMContainsWithIgnoreCase(attributeName)).Select(x => x.Value.Attributes[attributeName].EntityAttributeID).FirstOrDefault();
                        entityTypeId = entityDetailsInfo[entityTypeName].Legs.Where(x => x.Value.Attributes.Keys.SRMContainsWithIgnoreCase(attributeName)).Select(x => x.Value.EntityTypeID).FirstOrDefault();
                    }

                    List<int> ruleIds = existingDBRules.AsEnumerable().Where(x => Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) &&
                                       Convert.ToString(x[RMModelerConstants.Attribute_Name]).SRMEqualWithIgnoreCase(attributeName) &&
                                       Convert.ToString(x[RMModelerConstants.Rule_Type]).SRMEqualWithIgnoreCase(ruleType)).Select(y => Convert.ToInt32(y[RMModelerConstants.Rule_ID])).ToList<int>();

                    try
                    {
                        bool isValid = true;
                        foreach (ObjectRow row in group.ToList())
                        {
                            try
                            {
                                string ruleName = Convert.ToString(row[RMModelerConstants.Rule_Name]);
                                int priority = Convert.ToInt32(row[RMModelerConstants.Priority]);
                                bool state = Convert.ToBoolean(row[RMModelerConstants.Rule_State]);
                                string ruleText = Convert.ToString(row[RMModelerConstants.Rule_Text]);
                                int ruleId = 0;
                                int ruleTypeId = 8;
                                if (ruleType.SRMEqualWithIgnoreCase("Calculated Field"))
                                    ruleTypeId = 9;
                                else if (ruleType.SRMEqualWithIgnoreCase("Alert"))
                                    ruleTypeId = 11;

                                ruleSetId = controller.RMSaveRule(entityTypeId, attributeId, ruleTypeId, ruleName, priority, ruleText, state, ruleId, ruleSetId, userName, ConnectionConstants.RefMasterVendor_Connection, ref ruleGrammarInfo, mDBCon);
                                if (ruleSetId == 0)
                                    isValid = false;
                            }
                            catch (Exception ex)
                            {
                                isValid = false;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                            }
                        }
                        if (isValid)
                            controller.RMDeleteRulesByRuleID(ruleIds, mDBCon);

                        if (isValid)
                        {
                            mDBCon.CommitTransaction();
                            foreach (ObjectRow row in group.ToList())
                                new RMCommonMigration().SetPassedRow(row, string.Empty);
                        }
                        else
                        {
                            mDBCon.RollbackTransaction();
                            foreach (ObjectRow row in group.ToList().Where(t => string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList())
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { }, true);
                        }

                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->Error " + ex.Message);
                        foreach (ObjectRow row in group.ToList())
                        {
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, true);
                        }
                    }
                    finally
                    {
                        if (mDBCon != null)
                        {
                            RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                            mDBCon = null;
                        }
                    }
                }
                else
                {
                    foreach (ObjectRow row in group.ToList())
                    {
                        new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                    }
                }
            }
            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->End");
        }
        public void SyncBasketRules(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingDBRules, ObjectTable sourceTable, string userName, Dictionary<string, RMEntityDetailsInfo> entityDetailsInfo)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncBasketRules->Start");

            RMCommonController controller = new RMCommonController();

            var ruleGroups = sourceTable.AsEnumerable().Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS]))).GroupBy(x => new { ename = ConvertToLower(x[RMModelerConstants.ENTITY_DISPLAY_NAME]), lname = ConvertToLower(x[RMModelerConstants.Leg_Name]), rtype = ConvertToLower(x[RMModelerConstants.Rule_Type]) });

            foreach (var group in ruleGroups)
            {
                List<string> errorMessages = new List<string>();
                RADXRuleGrammarInfo ruleGrammarInfo = null;
                RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                int ruleSetId = 0;
                string entityTypeName = group.Key.ename;
                string legName = group.Key.lname;

                string ruleType = group.Key.rtype;

                if (!entityDetailsInfo.Keys.SRMContainsWithIgnoreCase(entityTypeName))
                {
                    errorMessages.Add(RMAttributeSetupConstants.INVALID_ENITY_TYPE);
                }

                if (entityDetailsInfo.Keys.SRMContainsWithIgnoreCase(entityTypeName) && !entityDetailsInfo[entityTypeName].Legs.Keys.SRMContainsWithIgnoreCase(legName))
                {
                    errorMessages.Add(RMAttributeSetupConstants.INVALID_LEG_ENITY_TYPE);
                }

                if (errorMessages.Count == 0)
                {
                    int entityTypeId = entityDetailsInfo[entityTypeName].EntityTypeID;
                    int legEntityTypeId = -1;
                    int attributeId = -1;

                    if (!string.IsNullOrEmpty(legName))
                    {
                        legEntityTypeId = entityDetailsInfo[entityTypeName].Legs[legName].EntityTypeID;
                    }

                    List<int> ruleIds = existingDBRules.AsEnumerable().Where(x => Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) &&
                                        Convert.ToString(x[RMModelerConstants.Leg_Name]).SRMEqualWithIgnoreCase(legName) &&
                                        Convert.ToString(x[RMModelerConstants.Rule_Type]).SRMEqualWithIgnoreCase(ruleType)).Select(y => Convert.ToInt32(y[RMModelerConstants.Rule_ID])).ToList<int>();

                    try
                    {
                        //controller.RMDeleteRulesByRuleID(ruleIds, mDBCon);
                        bool isValid = true;
                        foreach (ObjectRow row in group.ToList())
                        {
                            try
                            {
                                string ruleName = Convert.ToString(row[RMModelerConstants.Rule_Name]);
                                int priority = Convert.ToInt32(row[RMModelerConstants.Priority]);
                                bool state = Convert.ToBoolean(row[RMModelerConstants.Rule_State]);
                                string ruleText = Convert.ToString(row[RMModelerConstants.Rule_Text]);
                                int ruleId = 0;
                                int ruleTypeId = 10;

                                int rsId = controller.RMSaveRule(legEntityTypeId, attributeId, ruleTypeId, ruleName, priority, ruleText, state, ruleId, ruleSetId, userName, ConnectionConstants.RefMasterVendor_Connection, ref ruleGrammarInfo, mDBCon);
                                if (rsId == 0)
                                    isValid = false;
                            }
                            catch (Exception ex)
                            {
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                                isValid = false;
                                //Code to fail dependent rows in other tables
                            }
                        }
                        if (isValid)
                            controller.RMDeleteRulesByRuleID(ruleIds, mDBCon);
                        if (isValid)
                        {
                            mDBCon.CommitTransaction();
                            foreach (ObjectRow row in group.ToList())
                                new RMCommonMigration().SetPassedRow(row, string.Empty);
                        }
                        else
                        {
                            mDBCon.RollbackTransaction();
                            foreach (ObjectRow row in group.ToList().Where(t => string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList())
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { }, true);
                        }

                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("RMEntityTypeModelerMigration-SyncBasketRules->Error " + ex.Message);
                        foreach (ObjectRow row in group.ToList())
                        {
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, true);
                        }
                    }
                    finally
                    {
                        if (mDBCon != null)
                        {
                            RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                            mDBCon = null;
                        }
                    }
                }
                else
                {
                    foreach (ObjectRow row in group.ToList())
                    {
                        new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                    }
                }
            }
            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->End");
        }
        private void updateEntityInfo(Dictionary<string, RMEntityDetailsInfo> entityDetails, Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, string entityTypeName, RHashlist tempDets, Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo)
        {
            if (!entityDetails.ContainsKey(entityTypeName))
            {
                entityDetails.Add(entityTypeName, new RMEntityDetailsInfo());
                entityDetails[entityTypeName].EntityDisplayName = entityTypeName;
                entityDetails[entityTypeName].EntityTypeName = Convert.ToString(tempDets[3]) + Convert.ToString(tempDets[2]);
                entityDetails[entityTypeName].EntityTypeID = Convert.ToInt32(tempDets[1]);
                entityDetails[entityTypeName].Attributes = new Dictionary<string, RMEntityAttributeInfo>(StringComparer.OrdinalIgnoreCase);
                entityDetails[entityTypeName].Legs = new Dictionary<string, RMEntityDetailsInfo>(StringComparer.OrdinalIgnoreCase);
            }

            if (!allModulesEntityDetailsInfo.ContainsKey(entityTypeName))
            {
                allModulesEntityDetailsInfo.Add(entityTypeName, new RMEntityDetailsInfo());
                allModulesEntityDetailsInfo[entityTypeName].EntityDisplayName = entityTypeName;
                allModulesEntityDetailsInfo[entityTypeName].EntityTypeName = Convert.ToString(tempDets[3]) + Convert.ToString(tempDets[2]);
                allModulesEntityDetailsInfo[entityTypeName].EntityTypeID = Convert.ToInt32(tempDets[1]);
                allModulesEntityDetailsInfo[entityTypeName].Attributes = new Dictionary<string, RMEntityAttributeInfo>(StringComparer.OrdinalIgnoreCase);
                allModulesEntityDetailsInfo[entityTypeName].Legs = new Dictionary<string, RMEntityDetailsInfo>(StringComparer.OrdinalIgnoreCase);
            }

            if (!entityTypeVstemplateInfo.ContainsKey(entityTypeName))
            {
                entityTypeVstemplateInfo.Add(entityTypeName, new Dictionary<string, RMEntityTypeTemplateDetails>());
                string templateName = "SYSTEM";
                string tabName = entityTypeName;
                var layoutInfo = (DataSet)tempDets[0];
                entityTypeVstemplateInfo[entityTypeName].Add(templateName, new RMEntityTypeTemplateDetails());
                entityTypeVstemplateInfo[entityTypeName][templateName].TemplateName = templateName;
                entityTypeVstemplateInfo[entityTypeName][templateName].TemplateId = Convert.ToInt32(layoutInfo.Tables[0].Rows[0]["template_id"]);
                entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId = new Dictionary<string, RMEntityTabDetails>(StringComparer.OrdinalIgnoreCase);
                if (!entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId.ContainsKey(tabName))
                {
                    entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId.Add(tabName, new RMEntityTabDetails());
                    entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId[tabName].tabName = tabName;
                    entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId[tabName].tabNameId = Convert.ToInt32(layoutInfo.Tables[0].Rows[0]["entity_tab_name_id"]);
                    entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId[tabName].tabOrder = 1;
                }
            }
        }

        public void SyncLegConfiguration(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingLegs, ObjectTable sourceTable, string userName, int moduleId, Dictionary<string, RMEntityDetailsInfo> entityDetails, Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->Start");
            bool isInsert = false;

            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            RMEntityTypeSetupSync entityValidate = new RMEntityTypeSetupSync();
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.MASTER_ENTITY_TYPE_ID, DataType = typeof(Int32), DefaultValue = -1 });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.LEG_ENTITY_TYPE_ID, DataType = typeof(Int32), DefaultValue = -1 });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });
            new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

            var assign = from db in existingLegs.AsEnumerable()
                         join upl in sourceTable.AsEnumerable()
                         on ConvertToLower(db[RMModelerConstants.ENTITY_DISPLAY_NAME]) equals ConvertToLower(upl[RMModelerConstants.ENTITY_DISPLAY_NAME])
                         select AssignExistingEntityLegTypeInfo(db, upl);

            assign.Count();

            currentRows
               .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList().ForEach(row =>
               {
                   RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                   bool isentityType = false;
                   List<string> errorMessages = new List<string>();
                   string entityTypeName = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]);
                   string legName = Convert.ToString(row[RMModelerConstants.Leg_Name]);
                   RHashlist LegEntityDetails = new RHashlist();
                   try
                   {
                       isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);

                       string legRealName = string.Empty;
                       string validationErrorMessage = string.Empty;
                       int legId = 0;

                       errorMessages.AddRange(entityValidate.validateEntityDisplayName(entityTypeName));

                       errorMessages.AddRange(entityValidate.validateLegName(legName));

                       if (!entityDetails.ContainsKey(entityTypeName))
                       {
                           isentityType = true;
                           errorMessages.Add("Invalid Entity Type");
                       }

                       if (errorMessages.Count > 0)
                       {
                           new RMCommonMigration().SetFailedRow(row, errorMessages, true);
                           if (isentityType)
                               FailAllDependencies(deltaSet, entityTypeName, null, false, false, false, false, false, false, false, false, false, false, false);
                           else
                               FailAllDependencies(deltaSet, entityTypeName, null, true, true, false, false, false, false, false, false, false, false, false);
                       }

                       else
                       {
                           if (!entityDetails[entityTypeName].Legs.ContainsKey(legName)) //Add New Leg
                           {
                               RMEntityTypeInfo entityInfo = new RMEntityTypeInfo();
                               entityInfo.AccountId = 0;
                               entityInfo.EntityDisplayName = legName;
                               entityInfo.EntityTypeName = legName;
                               entityInfo.CreatedBy = userName;
                               entityInfo.ModuleID = moduleId;
                               entityInfo.StructureTypeID = 3;
                               entityInfo.HasParent = false;
                               entityInfo.IsOneToOne = false;
                               entityInfo.IsVector = false;
                               entityInfo.LastModifiedBy = userName;
                               entityInfo.DerivedFromEntityTypeID = Convert.ToInt32(entityDetails[entityTypeName].EntityTypeID);

                               LegEntityDetails = new RMModelerDBManager().AddEntityType(entityInfo, mDBCon);

                           }
                           else if (entityDetails[entityTypeName].Legs.ContainsKey(legName))
                           {
                               legId = entityDetails[entityTypeName].Legs[legName].EntityTypeID;
                               legRealName = entityDetails[entityTypeName].Legs[legName].EntityTypeName;
                           }
                           mDBCon.CommitTransaction();
                           new RMCommonMigration().SetPassedRow(row, string.Empty);
                           updateLegInfo(entityDetails, entityTypeVstemplateInfo, entityTypeName, legName, LegEntityDetails, allModulesEntityDetailsInfo);
                       }

                   }
                   catch (Exception ex)
                   {
                       if (mDBCon != null)
                           mDBCon.RollbackTransaction();

                       mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->Error " + ex.Message);

                       //FailLegDependencies(deltaSet, legName, true);
                       FailAllDependencies(deltaSet, entityTypeName, null, true, true, false, false, false, false, false, false, false, false, false);

                       //Code to fail dependent rows in other tables
                   }
                   finally
                   {
                       if (mDBCon != null)
                       {
                           RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                           mDBCon = null;
                       }
                   }
               });

            //Remove static columns
            List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMModelerConstants.MASTER_ENTITY_TYPE_ID, RMModelerConstants.LEG_ENTITY_TYPE_ID };

            new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);
            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->End");
        }

        private void updateLegInfo(Dictionary<string, RMEntityDetailsInfo> entityDetails, Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, string entityTypeName, string legName, RHashlist layoutDets, Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo)
        {
            if (!entityDetails[entityTypeName].Legs.ContainsKey(legName))
            {
                entityDetails[entityTypeName].Legs.Add(legName, new RMEntityDetailsInfo());
                entityDetails[entityTypeName].Legs[legName].EntityDisplayName = legName;
                entityDetails[entityTypeName].Legs[legName].EntityTypeName = Convert.ToString(layoutDets[3]) + Convert.ToString(layoutDets[2]); ;
                entityDetails[entityTypeName].Legs[legName].EntityTypeID = Convert.ToInt32(layoutDets[1]);
                entityDetails[entityTypeName].Legs[legName].Attributes = new Dictionary<string, RMEntityAttributeInfo>(StringComparer.OrdinalIgnoreCase);

                string tabName = legName;
                var layoutInfo = (DataSet)layoutDets[0];
                foreach (var info in layoutInfo.Tables[0].AsEnumerable())
                {
                    string templateName = Convert.ToString(info["template_name"]);
                    //if (!entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId.ContainsKey(tabName))
                    //    entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId.Add(tabName, Convert.ToInt32(info["entity_tab_name_id"]));
                    if (!entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId.ContainsKey(tabName))
                    {
                        entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId.Add(tabName, new RMEntityTabDetails());
                        entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId[tabName].tabName = tabName;
                        entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId[tabName].tabNameId = Convert.ToInt32(layoutInfo.Tables[0].Rows[0]["entity_tab_name_id"]);
                        entityTypeVstemplateInfo[entityTypeName][templateName].TabNameVsId[tabName].tabOrder = 1;
                    }
                }

            }

            if (!allModulesEntityDetailsInfo[entityTypeName].Legs.ContainsKey(legName))
            {
                allModulesEntityDetailsInfo[entityTypeName].Legs.Add(legName, new RMEntityDetailsInfo());
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].EntityDisplayName = legName;
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].EntityTypeName = Convert.ToString(layoutDets[3]) + Convert.ToString(layoutDets[2]); ;
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].EntityTypeID = Convert.ToInt32(layoutDets[1]);
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes = new Dictionary<string, RMEntityAttributeInfo>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public void SyncMasterAttributes(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingDBAttributes, ObjectTable sourceTable, Dictionary<string, RMEntityDetailsInfo> entityDetails, Dictionary<string, SecurityTypeMasterInfo> secTypeDetails, string dateFormat, string userName, DateTime syncDateTime, Dictionary<string, AttrInfo> commonAttributes, Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->Start");

            RMAttributeSetupMigrationValidations attrValidations = new RMAttributeSetupMigrationValidations(dbEntityTypesDetails, entityDetails, secTypeDetails, commonAttributes, allModulesEntityDetailsInfo);
            RMEntityTypeSetupSync entityValidate = new RMEntityTypeSetupSync();
            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            bool isInsert = false;
            Dictionary<string, List<RMEntityAttributeInfo>> lookupAttributes = new Dictionary<string, List<RMEntityAttributeInfo>>();
            Dictionary<string, Dictionary<string, RMAttributeDisplayConfigurationInfo>> dictEntityVsDisplayConfig = new Dictionary<string, Dictionary<string, RMAttributeDisplayConfigurationInfo>>();

            //entity type name, attribute name,lookup type, lookupdisplay attributes
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> lookupDisplayAttributes = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, List<string>> attributesFailed = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            ////Add static columns
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.ENTITY_ATTRIBUTE_ID, DataType = typeof(Int32), DefaultValue = -1 });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.Attribute_Real_Name, DataType = typeof(String), DefaultValue = DBNull.Value });

            new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

            var assign = from db in existingDBAttributes.AsEnumerable()
                         join upl in sourceTable.AsEnumerable()
                         on new { eName = ConvertToLower(db[RMModelerConstants.ENTITY_DISPLAY_NAME]), aName = ConvertToLower(db[RMModelerConstants.Attribute_Name]) }
                         equals new { eName = ConvertToLower(upl[RMModelerConstants.ENTITY_DISPLAY_NAME]), aName = ConvertToLower(upl[RMModelerConstants.Attribute_Name]) }

                         select AssignExistingAttributes(db, upl);

            assign.Count();

            var nonEmptyGroups = currentRows
                .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
            var attributeGroups = nonEmptyGroups.GroupBy(x => Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]));


            foreach (var group in attributeGroups)
            {
                List<string> errorMessages = new List<string>();
                Dictionary<string, RMAttributeDisplayConfigurationInfo> dictAttrDispInfo = new Dictionary<string, RMAttributeDisplayConfigurationInfo>();
                bool isValid = true;
                bool isEntityTypeValid = true;
                RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                string entityTypeName = Convert.ToString(group.Key);
                try
                {
                    int entityTypeId = 0;
                    List<RMEntityAttributeInfo> lstAttributesInfo = new List<RMEntityAttributeInfo>();
                    bool saveLookups = true;

                    if (!entityDetails.ContainsKey(entityTypeName))
                    {
                        errorMessages.Add(RMCommonConstants.ENTITY_TYPE_NOT_FOUND);
                        isEntityTypeValid = false;
                    }
                    else
                    {
                        foreach (var row in group.ToList())
                        {
                            errorMessages = new List<string>();
                            string lookupDisplayAttributesString = string.Empty;
                            RMAttributeDisplayConfigurationInfo attrDispConf = new RMAttributeDisplayConfigurationInfo();
                            isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);
                            try
                            {
                                entityTypeId = entityDetails[entityTypeName].EntityTypeID;
                                string attributeName = Convert.ToString(row[RMModelerConstants.Attribute_Name]);
                                string attributeRealName = isInsert ? attributeName.Replace(" ", "_") : Convert.ToString(row[RMModelerConstants.Attribute_Real_Name]);
                                if (isInsert && Regex.IsMatch(attributeName, @"^\d"))
                                    attributeRealName = "Z_" + attributeRealName;
                                int entityAttributeId = Convert.ToInt32(row[RMModelerConstants.ENTITY_ATTRIBUTE_ID]);
                                string dataType = Convert.ToString(row[RMModelerConstants.Data_Type]);
                                string attributeLength = Convert.ToString(row[RMModelerConstants.Attribute_Length]);
                                string lookupType = Convert.ToString(row[RMModelerConstants.Lookup_Type]);
                                string lookupAttribute = Convert.ToString(row[RMModelerConstants.Lookup_Attribute]);
                                string lookupDisplayAttribute = Convert.ToString(row[RMModelerConstants.Lookup_Display_Attributes]);
                                string defaultValue = Convert.ToString(row[RMModelerConstants.Default_Value]);
                                int searchViewPosition = string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Search_View_Position])) ? -1 : Convert.ToInt32(row[RMModelerConstants.Search_View_Position]);
                                string tags = Convert.ToString(row[RMModelerConstants.Tags]);
                                string restictedCharacters = Convert.ToString(row[RMModelerConstants.Restricted_Characters]);
                                bool isMandatory = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Mandatory])) ? Convert.ToBoolean(row[RMModelerConstants.Is_Mandatory]) : false;
                                bool visibleInSearch = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Visible_In_Search])) ? Convert.ToBoolean(row[RMModelerConstants.Visible_In_Search]) : false;
                                bool isCloneable = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Cloneable])) ? Convert.ToBoolean(row[RMModelerConstants.Is_Cloneable]) : false;
                                bool isEncrypted = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Encrypted])) ? Convert.ToBoolean(row[RMModelerConstants.Is_Encrypted]) : false;
                                bool isPII = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_PII])) ? Convert.ToBoolean(row[RMModelerConstants.Is_PII]) : false;
                                string orderByAttribute = Convert.ToString(row[RMModelerConstants.Order_By_Attribute]);
                                string showEntityCode = Convert.ToString(row[RMModelerConstants.Show_Entity_Code]);
                                string showPercentage = Convert.ToString(row[RMModelerConstants.Show_Percentage]);
                                string showMultiplier = Convert.ToString(row[RMModelerConstants.Show_Multiple]);
                                string showComma = Convert.ToString(row[RMModelerConstants.Comma_Formatting]);
                                bool dispMetaData = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Display_Meta_Data])) ? Convert.ToBoolean(row[RMModelerConstants.Display_Meta_Data]) : false;
                                string attrDescription = Convert.ToString(row[RMModelerConstants.Attribute_Description]);


                                if (!string.IsNullOrEmpty(showEntityCode) && (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)) || dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.SECURITY_LOOKUP))))
                                    attrDispConf.ShowEntityCode = Convert.ToBoolean(showEntityCode);

                                errorMessages.AddRange(attrValidations.ValidateAttributeName(entityTypeName, attributeName, dataType, isInsert, attributeRealName));

                                if (isEncrypted)
                                    errorMessages.AddRange(attrValidations.ValidateEncryptedAttributes(dataType));

                                if (!isInsert && entityDetails[entityTypeName].Attributes.ContainsKey(attributeName))
                                {
                                    var type = GetDataType(dataType);
                                    if (!entityDetails[entityTypeName].Attributes[attributeName].DataType.Equals(type))
                                        errorMessages.Add(RMCommonConstants.DATATYPE_CANNOT_BE_UPDATED);
                                }

                                errorMessages.AddRange(attrValidations.ValidateAttributeDataTypeLength(dataType, attributeLength));

                                if (!isInsert && (dataType == RMAttributeSetupConstants.DATA_TYPE_VARCHAR || dataType == RMAttributeSetupConstants.DATA_TYPE_DECIMAL))
                                    errorMessages.AddRange(attrValidations.ValidateAttributeDataLength(entityAttributeId, attributeLength));

                                if (!isInsert && isMandatory)
                                    errorMessages.AddRange(attrValidations.ValidateMandatory(entityTypeName, attributeName));

                                if (isPII && !isEncrypted)
                                    errorMessages.Add(RMCommonConstants.PII_NOT_ENCRYPTED);

                                errorMessages.AddRange(attrValidations.ValidateRestrictedChars(attributeName, restictedCharacters));
                                errorMessages.AddRange(entityValidate.validateTagName(tags));

                                if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)) && (string.IsNullOrEmpty(lookupType) || string.IsNullOrEmpty(lookupAttribute)))
                                    errorMessages.Add(RMCommonConstants.LOOKUP_NOT_MAPPED);

                                if (!string.IsNullOrEmpty(lookupType))
                                {
                                    if (isInsert && entityTypeName.SRMEqualWithIgnoreCase(lookupType) && dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)))
                                    {
                                        if (group.ToList().AsEnumerable().Where(x => Convert.ToString(x[RMModelerConstants.Attribute_Name]).SRMEqualWithIgnoreCase(lookupAttribute) &&
                                                Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(lookupType) &&
                                                !Convert.ToString(x[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Count() == 0 &&
                                                !(allModulesEntityDetailsInfo.ContainsKey(lookupType) && allModulesEntityDetailsInfo[lookupType].Attributes.ContainsKey(lookupAttribute)))
                                        {
                                            errorMessages.Add(RMAttributeSetupConstants.INVALID_REFERENCE_ATTRIBUTE_ERROR);
                                        }

                                        if (!string.IsNullOrEmpty(lookupDisplayAttribute.Trim()))
                                        {
                                            List<string> tempDisplayAttributes = lookupDisplayAttribute.Split(',').ToList();
                                            if (!tempDisplayAttributes.Contains(lookupAttribute))
                                                errorMessages.Add(RMCommonConstants.INVALID_LOOKUP_DISPLAY_ATTRS);
                                            foreach (string attr in tempDisplayAttributes)
                                            {
                                                if (group.ToList().AsEnumerable().Where(x => Convert.ToString(x[RMModelerConstants.Attribute_Name]).SRMEqualWithIgnoreCase(attr) &&
                                                Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(lookupType) &&
                                                !Convert.ToString(x[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Count() == 0 &&
                                                !(allModulesEntityDetailsInfo.ContainsKey(lookupType) && allModulesEntityDetailsInfo[lookupType].Attributes.ContainsKey(attr)))

                                                {
                                                    errorMessages.Add(attr + " - " + RMAttributeSetupConstants.INVALID_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);
                                                }
                                            }
                                            if (errorMessages.Count == 0)
                                            {
                                                if (!lookupDisplayAttributes.ContainsKey(entityTypeName))
                                                    lookupDisplayAttributes.Add(entityTypeName, new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase));

                                                lookupDisplayAttributes[entityTypeName].Add(attributeName, new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase));
                                                lookupDisplayAttributes[entityTypeName][attributeName].Add(lookupType, lookupDisplayAttribute.Split(',').ToList());
                                            }
                                        }
                                        else
                                        {
                                            errorMessages.Add(RMAttributeSetupConstants.MISSING_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);
                                        }
                                    }
                                    else
                                    {
                                        errorMessages.AddRange(attrValidations.ValidateReferenceType(isInsert, entityTypeName, attributeName, dataType, lookupType, lookupAttribute, lookupDisplayAttribute, string.Empty, nonEmptyGroups));
                                        if (errorMessages.Count == 0)
                                        {
                                            if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)))
                                            {
                                                StringBuilder lookupAttributeString = new StringBuilder();

                                                if (!lookupDisplayAttributes.ContainsKey(entityTypeName))
                                                    lookupDisplayAttributes.Add(entityTypeName, new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase));

                                                List<string> tempDisplayAttributes = lookupDisplayAttribute.Split(',').ToList();
                                                if (!tempDisplayAttributes.Contains(lookupAttribute))
                                                    errorMessages.Add(RMCommonConstants.INVALID_LOOKUP_DISPLAY_ATTRS);

                                                lookupDisplayAttributes[entityTypeName].Add(attributeName, new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase));
                                                lookupDisplayAttributes[entityTypeName][attributeName].Add(lookupType, tempDisplayAttributes);
                                            }
                                            else if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.SECURITY_LOOKUP)))
                                            {
                                                StringBuilder lookupAttributeString = new StringBuilder();
                                                if (!string.IsNullOrEmpty(lookupDisplayAttribute.Trim()))
                                                {
                                                    List<string> tempDisplayAttributes = lookupDisplayAttribute.Split(',').ToList();
                                                    if (!tempDisplayAttributes.Contains(lookupAttribute))
                                                        errorMessages.Add(RMCommonConstants.INVALID_LOOKUP_DISPLAY_ATTRS);
                                                    foreach (string attr in tempDisplayAttributes)
                                                    {
                                                        if (lookupAttributeString.Length == 0)
                                                        {
                                                            if (lookupType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES))
                                                                lookupAttributeString.Append(commonAttributes[attr].AttrId);
                                                            else
                                                                lookupAttributeString.Append(secTypeDetails[lookupType].AttributeInfo.MasterAttrs[attr].AttrId);
                                                        }
                                                        else
                                                        {
                                                            if (lookupType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES))
                                                                lookupAttributeString.Append("," + commonAttributes[attr].AttrId);
                                                            else
                                                                lookupAttributeString.Append("," + secTypeDetails[lookupType].AttributeInfo.MasterAttrs[attr].AttrId);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    errorMessages.Add(RMAttributeSetupConstants.MISSING_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);
                                                }
                                                lookupDisplayAttributesString = lookupAttributeString.ToString();
                                            }
                                        }
                                    }
                                }

                                errorMessages.AddRange(attrValidations.ValidateDefaultValue(dataType, ref defaultValue, dateFormat, attributeLength));

                                errorMessages.AddRange(attrValidations.ValidateAttributeDisplay(dataType, showEntityCode, orderByAttribute, showComma, showPercentage, showMultiplier));

                                if (!string.IsNullOrEmpty(orderByAttribute) && entityDetails.ContainsKey(lookupType))
                                {
                                    if (!orderByAttribute.SRMEqualWithIgnoreCase("entity code") && !entityDetails[lookupType].Attributes.ContainsKey(orderByAttribute))
                                    {
                                        errorMessages.Add("Invalid order by attribute.");
                                    }
                                    else if (!orderByAttribute.SRMEqualWithIgnoreCase("entity code") && !orderByAttribute.SRMEqualWithIgnoreCase(lookupAttribute) && entityDetails[lookupType].Attributes.ContainsKey(orderByAttribute) && entityDetails[lookupType].Attributes[orderByAttribute].DataType != RMDBDataTypes.INT)
                                        errorMessages.Add("Invalid order by attribute. Attribute can be entity code, primary attribute or attribute of INT datatype.");
                                }

                                if (errorMessages.Count > 0)
                                {
                                    new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                                    isValid = false;

                                    if (!attributesFailed.ContainsKey(entityTypeName))
                                        attributesFailed.Add(entityTypeName, new List<string>());

                                    attributesFailed[entityTypeName].Add(attributeName);
                                }
                                else
                                {
                                    RMDBDataTypes type = RMDBDataTypes.VARCHAR;
                                    switch (dataType.ToUpper())
                                    {
                                        case RMAttributeSetupConstants.DATA_TYPE_VARCHAR:
                                            type = RMDBDataTypes.VARCHAR;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_VARCHAR_MAX:
                                            type = RMDBDataTypes.VARCHARMAX;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_INT:
                                            type = RMDBDataTypes.INT;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_DATETIME:
                                            type = RMDBDataTypes.DATETIME;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_BIT:
                                            type = RMDBDataTypes.BIT;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_DECIMAL:
                                            type = RMDBDataTypes.DECIMAL;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_LOOKUP:
                                            saveLookups = false;
                                            type = RMDBDataTypes.LOOKUP;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_SECURITYLOOKUP:
                                        case RMAttributeSetupConstants.DATA_TYPE_SECURITY_LOOKUP:
                                            type = RMDBDataTypes.SECURITY_LOOKUP;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_FILE:
                                            type = RMDBDataTypes.FILE;
                                            break;
                                    }
                                    RMEntityAttributeInfo attrInfo = new RMEntityAttributeInfo()
                                    {
                                        DisplayName = attributeName,
                                        AttributeDescription = attrDescription,
                                        EntityAttributeID = entityAttributeId,
                                        EntityTypeID = entityTypeId,
                                        IsClonable = isCloneable,
                                        IsEncrypted = isEncrypted,
                                        IsNullable = !isMandatory,
                                        IsPII = isPII,
                                        DisplayMetaData = dispMetaData,
                                        IsPrimary = false,
                                        IsSearchView = visibleInSearch,
                                        IsInternal = false,
                                        DataTypeLength = attributeLength,
                                        DataType = type,
                                        DefaultValue = defaultValue,
                                        Tags = tags,
                                        RestrictedChars = restictedCharacters,
                                        SearchViewPosition = searchViewPosition,
                                        AttributeName = attributeRealName,
                                        LookupEntityTypeID = dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)) ? allModulesEntityDetailsInfo[lookupType].EntityTypeID : dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.SECURITY_LOOKUP)) ? lookupType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES) ? 0 : secTypeDetails[lookupType].SectypeId : -1,
                                        LookupAttributeID = dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)) ? (lookupType.SRMEqualWithIgnoreCase(entityTypeName) ? -1 : (allModulesEntityDetailsInfo[lookupType].Attributes.ContainsKey(lookupAttribute) ? allModulesEntityDetailsInfo[lookupType].Attributes[lookupAttribute].EntityAttributeID : -1)) : dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.SECURITY_LOOKUP)) ? lookupType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES) ? commonAttributes[lookupAttribute].AttrId : secTypeDetails[lookupType].AttributeInfo.MasterAttrs[lookupAttribute].AttrId : -1,                                        
                                        LookupEntityTypeName = lookupType,
                                        LookupAttributeName = lookupAttribute,
                                        LookupDisplayAttributes = lookupDisplayAttributesString,
                                        LastModifiedBy = userName,
                                        LastModifiedOn = syncDateTime
                                    };

                                    if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.INT)) || dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.DECIMAL)))
                                    {
                                        attrDispConf.AttributeId = entityAttributeId;
                                        attrDispConf.AllowCommaFormatting = Convert.ToBoolean(showComma);
                                        attrDispConf.AppendMultiplier = Convert.ToBoolean(showMultiplier);
                                        attrDispConf.AppendPercentage = Convert.ToBoolean(showPercentage);
                                        if (dictAttrDispInfo.ContainsKey(attributeName))
                                        {
                                            dictAttrDispInfo[attributeName] = attrDispConf;
                                        }
                                        else
                                            dictAttrDispInfo.Add(attributeName, attrDispConf);
                                    }
                                    else if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)))
                                    {
                                        attrDispConf.AttributeId = entityAttributeId;
                                        attrDispConf.ShowEntityCode = Convert.ToBoolean(showEntityCode);
                                        if (!string.IsNullOrEmpty(orderByAttribute) && errorMessages.Count == 0)
                                        {
                                            attrDispConf.OrderByAttributeName = orderByAttribute.ToLower().Equals("entity code") ? "entity_code" : entityDetails[lookupType].Attributes.ContainsKey(orderByAttribute) ? entityDetails[lookupType].Attributes[orderByAttribute].AttributeName : orderByAttribute;
                                            attrDispConf.OrderByAttributeId = entityDetails[lookupType].Attributes.ContainsKey(orderByAttribute) ? entityDetails[lookupType].Attributes[orderByAttribute].EntityAttributeID : -1;
                                        }
                                        if (dictAttrDispInfo.ContainsKey(attributeName))
                                        {
                                            dictAttrDispInfo[attributeName] = attrDispConf;
                                        }
                                        else
                                            dictAttrDispInfo.Add(attributeName, attrDispConf);
                                    }
                                    else if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.SECURITY_LOOKUP)))
                                    {
                                        attrDispConf.AttributeId = entityAttributeId;
                                        attrDispConf.ShowEntityCode = Convert.ToBoolean(showEntityCode);
                                        if (dictAttrDispInfo.ContainsKey(attributeName))
                                        {
                                            dictAttrDispInfo[attributeName] = attrDispConf;
                                        }
                                        else
                                            dictAttrDispInfo.Add(attributeName, attrDispConf);
                                    }

                                    lstAttributesInfo.Add(attrInfo);

                                    if (type == RMDBDataTypes.LOOKUP)
                                    {
                                        if (lookupAttributes.ContainsKey(entityTypeName))
                                            lookupAttributes[entityTypeName].Add(attrInfo);
                                        else
                                            lookupAttributes.Add(entityTypeName, new List<RMEntityAttributeInfo>() { attrInfo });
                                    }							
                                }
                            }
                            catch (Exception ex)
                            {
                                mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->Attribute Info Error " + ex.Message);
                                isValid = false;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                                //FailDependencies(deltaSet, entityTypeName, true);
                                FailAllDependencies(deltaSet, entityTypeName, null, true, false, false, false, false, false, false, false, false, false, false);
                                lookupAttributes.Remove(entityTypeName);
                                //dictEntityVsDisplayConfig.Remove(entityTypeName);
                            }
                        }
                    }
                    if (isValid && isEntityTypeValid)
                    {
                        DataSet attrOutputDs = new RMModelerDBManager().AddUpdateEntityTypeAttributes(lstAttributesInfo, entityTypeId, userName, saveLookups, mDBCon);
                        mDBCon.CommitTransaction();
                        //Update info with details
                        UpdateAttributes(entityDetails, attrOutputDs, lstAttributesInfo, entityTypeName, allModulesEntityDetailsInfo);
						if (dictEntityVsDisplayConfig.ContainsKey(entityTypeName))
                            dictEntityVsDisplayConfig[entityTypeName] = dictAttrDispInfo;
                        else
                            dictEntityVsDisplayConfig.Add(entityTypeName, dictAttrDispInfo);
                        foreach (var row in group.ToList())
                        {
                            new RMCommonMigration().SetPassedRow(row, string.Empty);
                        }
                    }
                    else
                    {
                        if (!isValid)
                            //FailDependencies(deltaSet, entityTypeName, true);
                            FailAllDependencies(deltaSet, entityTypeName, null, true, false, false, false, false, false, false, false, false, false, false);

                        if (!isEntityTypeValid)
                            //FailEntityDependencies(deltaSet, entityTypeName);
                            FailAllDependencies(deltaSet, entityTypeName, null, false, false, false, false, false, false, false, false, false, false, false);
                        lookupAttributes.Remove(entityTypeName);
						if(dictEntityVsDisplayConfig.ContainsKey(entityTypeName))
							dictEntityVsDisplayConfig.Remove(entityTypeName);

                        foreach (RMEntityAttributeInfo info in lstAttributesInfo)
                        {
                            if (!attributesFailed.ContainsKey(entityTypeName))
                                attributesFailed.Add(entityTypeName, new List<string>());

                            attributesFailed[entityTypeName].Add(info.DisplayName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->Save Attributes Error " + ex.Message);

                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();
                    //Code to fail dependent rows in other tables
                    foreach (var row in group.ToList())
                    {
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                    }
                    //FailDependencies(deltaSet, entityTypeName, true);
                    FailAllDependencies(deltaSet, entityTypeName, null, true, false, false, false, false, false, false, false, false, false, false);
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }
            }

            foreach (KeyValuePair<string, List<RMEntityAttributeInfo>> info in lookupAttributes)
            {
                int entityTypeId = 0;
                entityTypeId = entityDetails[info.Key].EntityTypeID;
                List<RMEntityAttributeInfo> passsedList = info.Value;
                List<RMEntityAttributeInfo> failedList = info.Value;

                var count = 1;
                while (count > 0)
                {
                    var nLevelFailed = failedList.AsEnumerable().Where(x => attributesFailed.ContainsKey(x.LookupEntityTypeName) && attributesFailed[x.LookupEntityTypeName].SRMContainsWithIgnoreCase(x.LookupAttributeName) && x.EntityAttributeID < 0);
                    count = nLevelFailed.Count();
                    int subInfoCount = 0;
                    foreach (var subInfo in nLevelFailed)
                    {
                        if (!attributesFailed.ContainsKey(entityDetails[info.Key].EntityDisplayName))
                            attributesFailed.Add(entityDetails[info.Key].EntityDisplayName, new List<string>());

                        if (subInfo.EntityAttributeID < 0 && !attributesFailed[entityDetails[info.Key].EntityDisplayName].SRMContainsWithIgnoreCase(subInfo.DisplayName))
                            attributesFailed[entityDetails[info.Key].EntityDisplayName].Add(subInfo.DisplayName);
                        else
                            subInfoCount++;
                    }
                    if (count == subInfoCount)
                        count = 0;
                }

                failedList = failedList.AsEnumerable().Where(x => attributesFailed.ContainsKey(x.LookupEntityTypeName) && attributesFailed[x.LookupEntityTypeName].SRMContainsWithIgnoreCase(x.LookupAttributeName) && x.EntityAttributeID < 0).ToList();

                passsedList = passsedList.AsEnumerable().Where(x => (!attributesFailed.ContainsKey(x.LookupEntityTypeName)) || (attributesFailed.ContainsKey(x.LookupEntityTypeName) && !attributesFailed[x.LookupEntityTypeName].SRMContainsWithIgnoreCase(x.LookupAttributeName))).ToList();

                new RMModelerDBManager().DeleteFailedLookupAttribute(failedList, entityTypeId, allModulesEntityDetailsInfo[info.Key].EntityDisplayName, userName, sourceTable.AsEnumerable());

                if (passsedList.Count > 0)
                    new RMModelerDBManager().AddUpdateLookups(passsedList, entityTypeId, info.Key, userName, syncDateTime, allModulesEntityDetailsInfo);
                else
                {
                    try
                    {
                        Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                        Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.common.RMCommonUtils");
                        MethodInfo CreateEntityTypeViews = objType.GetMethod("CreateEntityTypeViews");
                        CreateEntityTypeViews.Invoke(null, new object[] { entityTypeId });
                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("Error while creating views for entityTypeId : " + entityTypeId);
                        mLogger.Error(ex.Message);
                    }
                }

                new RMModelerDBManager().UpdateLookupDisplayAttributes(allModulesEntityDetailsInfo, lookupDisplayAttributes);
            }

            if (dictEntityVsDisplayConfig != null && dictEntityVsDisplayConfig.Count > 0)
            {
                HashSet<RMAttributeDisplayConfigurationInfo> lstAttrDispInfo = new HashSet<RMAttributeDisplayConfigurationInfo>();
                foreach (var entitytype in dictEntityVsDisplayConfig.Keys)
                {
                    if (entityDetails.ContainsKey(entitytype))
                    {
                        foreach (var attrDet in dictEntityVsDisplayConfig[entitytype])
                        {
                            if (entityDetails[entitytype].Attributes.ContainsKey(attrDet.Key))
                            {
                                var config = attrDet.Value;
								
                                var lookupType = entityDetails[entitytype].Attributes[attrDet.Key].LookupEntityTypeName;
                                if (config.AttributeId == -1)
                                    config.AttributeId = entityDetails[entitytype].Attributes[attrDet.Key].EntityAttributeID;
                                if (!string.IsNullOrEmpty(config.OrderByAttributeName) && config.OrderByAttributeId == -1 && lookupType != null && !string.IsNullOrEmpty(Convert.ToString(lookupType)) && entityDetails[lookupType].Attributes.ContainsKey(config.OrderByAttributeName) && config.OrderByAttributeName != "entity_code")
                                {
                                    config.OrderByAttributeId = entityDetails[lookupType].Attributes[config.OrderByAttributeName].EntityAttributeID;
                                    config.OrderByAttributeName = entityDetails[lookupType].Attributes[config.OrderByAttributeName].AttributeName;
                                }
                                if(!lstAttrDispInfo.Contains(config))
                                    lstAttrDispInfo.Add(config);
                            }
                        }
                    }
                }
                new RMModelerDBManager().UpdateAttributeDisplayConfigurations(lstAttrDispInfo);
            }


            //Remove static columns
            List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMModelerConstants.ENTITY_TYPE_ID, RMModelerConstants.ENTITY_ATTRIBUTE_ID, RMModelerConstants.Attribute_Real_Name };
            new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);

            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeRules->End");
        }

        public void SyncLegAttributes(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingDBAttributes, ObjectTable sourceTable, Dictionary<string, RMEntityDetailsInfo> entityDetails, Dictionary<string, SecurityTypeMasterInfo> secTypeDetails, string dateFormat, string userName, DateTime syncDateTime, Dictionary<string, AttrInfo> commonAttributes, Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncLegAttributes->Start");

            RMAttributeSetupMigrationValidations attrValidations = new RMAttributeSetupMigrationValidations(dbEntityTypesDetails, entityDetails, secTypeDetails, commonAttributes, allModulesEntityDetailsInfo);
            RMEntityTypeSetupSync entityValidate = new RMEntityTypeSetupSync();
            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            HashSet<RMAttributeDisplayConfigurationInfo> lstAttrDispInfo = new HashSet<RMAttributeDisplayConfigurationInfo>();
            bool isInsert = false;

            ////Add static columns
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.ENTITY_ATTRIBUTE_ID, DataType = typeof(Int32), DefaultValue = -1 });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.Attribute_Real_Name, DataType = typeof(String), DefaultValue = DBNull.Value });

            new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

            var assign = from db in existingDBAttributes.AsEnumerable()
                         join upl in sourceTable.AsEnumerable()
                         on new { eName = ConvertToLower(db[RMModelerConstants.ENTITY_DISPLAY_NAME]), lName = ConvertToLower(db[RMModelerConstants.Leg_Name]), aName = ConvertToLower(db[RMModelerConstants.Attribute_Name]) }
                         equals new { eName = ConvertToLower(upl[RMModelerConstants.ENTITY_DISPLAY_NAME]), lName = ConvertToLower(upl[RMModelerConstants.Leg_Name]), aName = ConvertToLower(upl[RMModelerConstants.Attribute_Name]) }

                         select AssignExistingLegAttributes(db, upl);

            assign.Count();

            var attributeGroups = currentRows
                .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS]))).GroupBy(x => new { eName = Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]), lName = Convert.ToString(x[RMModelerConstants.Leg_Name]) });

            foreach (var group in attributeGroups)
            {
                List<string> errorMessages = new List<string>();
                bool isValid = true, isEntityTypeValid = true;
                RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                Dictionary<string, RMAttributeDisplayConfigurationInfo> dictAttrDispInfo = new Dictionary<string, RMAttributeDisplayConfigurationInfo>();
                string entityTypeName = Convert.ToString(group.Key.eName);
                string legName = Convert.ToString(group.Key.lName);
                try
                {
                    int entityTypeId = 0;
                    int legEntityTypeId = 0;
                    List<RMEntityAttributeInfo> lstAttributesInfo = new List<RMEntityAttributeInfo>();

                    if (!entityDetails.ContainsKey(entityTypeName))
                    {
                        errorMessages.Add("Invalid Entity Type.");
                        isEntityTypeValid = false;
                    }
                    else if (!entityDetails[entityTypeName].Legs.ContainsKey(legName))
                    {
                        errorMessages.Add(RMCommonConstants.LEG_NOT_FOUND);
                    }
                    else
                    {
                        foreach (var row in group.ToList())
                        {
                            errorMessages = new List<string>();
                            RMAttributeDisplayConfigurationInfo attrDispConf = new RMAttributeDisplayConfigurationInfo();
                            isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);
                            try
                            {
                                entityTypeId = entityDetails[entityTypeName].EntityTypeID;
                                legEntityTypeId = entityDetails[entityTypeName].Legs[legName].EntityTypeID;
                                string attributeName = Convert.ToString(row[RMModelerConstants.Attribute_Name]);
                                string attributeRealName = isInsert ? attributeName.Replace(" ", "_") : Convert.ToString(row[RMModelerConstants.Attribute_Real_Name]);
                                if (isInsert && Regex.IsMatch(attributeName, @"^\d"))
                                    attributeRealName = "Z_" + attributeRealName;
                                int entityAttributeId = Convert.ToInt32(row[RMModelerConstants.ENTITY_ATTRIBUTE_ID]);
                                string dataType = Convert.ToString(row[RMModelerConstants.Data_Type]);
                                string attributeLength = Convert.ToString(row[RMModelerConstants.Attribute_Length]);
                                string lookupType = Convert.ToString(row[RMModelerConstants.Lookup_Type]);
                                string lookupAttribute = Convert.ToString(row[RMModelerConstants.Lookup_Attribute]);
                                string lookupDisplayAttribute = Convert.ToString(row[RMModelerConstants.Lookup_Display_Attributes]);
                                string defaultValue = Convert.ToString(row[RMModelerConstants.Default_Value]);
                                int searchViewPosition = -1;
                                bool isMandatory = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Mandatory])) ? Convert.ToBoolean(row[RMModelerConstants.Is_Mandatory]) : false;
                                bool visibleInSearch = false;
                                bool isCloneable = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Cloneable])) ? Convert.ToBoolean(row[RMModelerConstants.Is_Cloneable]) : false;
                                bool isEncrypted = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Encrypted])) ? Convert.ToBoolean(row[RMModelerConstants.Is_Encrypted]) : false;
                                bool isPrimary = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Primary])) ? Convert.ToBoolean(row[RMModelerConstants.Is_Primary]) : false;
                                bool isPII = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_PII])) ? Convert.ToBoolean(row[RMModelerConstants.Is_PII]) : false;
                                string tags = Convert.ToString(row[RMModelerConstants.Tags]);
                                string restictedCharacters = Convert.ToString(row[RMModelerConstants.Restricted_Characters]);

                                //bool showEntityCode = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Show_Entity_Code])) ? Convert.ToBoolean(row[RMModelerConstants.Show_Entity_Code]) : true;
                                string orderByAttribute = Convert.ToString(row[RMModelerConstants.Order_By_Attribute]);
                                //bool showPercentage = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Show_Percentage])) ? Convert.ToBoolean(row[RMModelerConstants.Show_Percentage]) : false;
                                //bool showMultiplier = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Show_Multiple])) ? Convert.ToBoolean(row[RMModelerConstants.Show_Multiple]) : false;
                                //bool showComma = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Comma_Formatting])) ? Convert.ToBoolean(row[RMModelerConstants.Comma_Formatting]) : true;
                                string showEntityCode = Convert.ToString(row[RMModelerConstants.Show_Entity_Code]);
                                string showPercentage = Convert.ToString(row[RMModelerConstants.Show_Percentage]);
                                string showMultiplier = Convert.ToString(row[RMModelerConstants.Show_Multiple]);
                                string showComma = Convert.ToString(row[RMModelerConstants.Comma_Formatting]);
                                bool dispMetaData = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Display_Meta_Data])) ? Convert.ToBoolean(row[RMModelerConstants.Display_Meta_Data]) : false;
                                string attrDescription = Convert.ToString(row[RMModelerConstants.Attribute_Description]);

                                errorMessages.AddRange(attrValidations.ValidateAttributeName(entityTypeName, attributeName, dataType, isInsert, attributeRealName));

                                if (isEncrypted)
                                    errorMessages.AddRange(attrValidations.ValidateEncryptedAttributes(dataType));

                                errorMessages.AddRange(attrValidations.ValidateAttributeDataTypeLength(dataType, attributeLength));

                                if (!isInsert && entityDetails[entityTypeName].Legs[legName].Attributes.ContainsKey(attributeName))
                                {
                                    var type = GetDataType(dataType);
                                    if (!entityDetails[entityTypeName].Legs[legName].Attributes[attributeName].DataType.Equals(type))
                                        errorMessages.Add(RMCommonConstants.DATATYPE_CANNOT_BE_UPDATED);
                                }


                                if (!isInsert && (dataType == RMAttributeSetupConstants.DATA_TYPE_VARCHAR || dataType == RMAttributeSetupConstants.DATA_TYPE_DECIMAL))
                                    errorMessages.AddRange(attrValidations.ValidateAttributeDataLength(entityAttributeId, attributeLength));

                                if (!isInsert && isMandatory)
                                    errorMessages.AddRange(attrValidations.ValidateMandatory(legName, attributeName));

                                if (!isInsert && isPrimary && !entityDetails[entityTypeName].Legs[legName].Attributes[attributeName].IsPrimary)
                                    errorMessages.AddRange(attrValidations.ValidateLegPrimary(entityTypeName, legName, attributeName));

                                errorMessages.AddRange(attrValidations.ValidateRestrictedChars(attributeName, restictedCharacters));
                                errorMessages.AddRange(entityValidate.validateTagName(tags));

                                if (isPII && !isEncrypted)
                                    errorMessages.Add(RMCommonConstants.PII_NOT_ENCRYPTED);


                                errorMessages.AddRange(attrValidations.ValidateReferenceType(isInsert, entityTypeName, attributeName, dataType, lookupType, lookupAttribute, lookupDisplayAttribute, legName, new List<ObjectRow>()));

                                errorMessages.AddRange(attrValidations.ValidateAttributeDisplay(dataType, showEntityCode, orderByAttribute, showComma, showPercentage, showMultiplier));

                                if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)) && (string.IsNullOrEmpty(lookupType) || string.IsNullOrEmpty(lookupAttribute)))
                                    errorMessages.Add(RMCommonConstants.LOOKUP_NOT_MAPPED);

                                if (!string.IsNullOrEmpty(orderByAttribute) && entityDetails.ContainsKey(lookupType))
                                {
                                    if (!orderByAttribute.SRMEqualWithIgnoreCase("entity code") && !entityDetails[lookupType].Attributes.ContainsKey(orderByAttribute))
                                    {
                                        errorMessages.Add("Invalid order by attribute.");
                                    }
                                    else if (!orderByAttribute.SRMEqualWithIgnoreCase("entity code") && !orderByAttribute.SRMEqualWithIgnoreCase(lookupAttribute) && entityDetails[lookupType].Attributes.ContainsKey(orderByAttribute) && entityDetails[lookupType].Attributes[orderByAttribute].DataType != RMDBDataTypes.INT)
                                        errorMessages.Add("Invalid order by attribute.");
                                }

                                string lookupDisplayAttributes = string.Empty;
                                if (errorMessages.Count == 0)
                                {
                                    if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)))
                                    {
                                        StringBuilder lookupAttributeString = new StringBuilder();
                                        if (!string.IsNullOrEmpty(lookupDisplayAttribute.Trim()))
                                        {
                                            List<string> tempDisplayAttributes = lookupDisplayAttribute.Split(',').ToList();
                                            if (!tempDisplayAttributes.Contains(lookupAttribute))
                                                errorMessages.Add(RMCommonConstants.INVALID_LOOKUP_DISPLAY_ATTRS);
                                            foreach (string attr in tempDisplayAttributes)
                                            {
                                                if (lookupAttributeString.Length == 0)
                                                    lookupAttributeString.Append(allModulesEntityDetailsInfo[lookupType].Attributes[attr].EntityAttributeID);
                                                else
                                                    lookupAttributeString.Append("," + allModulesEntityDetailsInfo[lookupType].Attributes[attr].EntityAttributeID);
                                            }
                                        }
                                        else
                                        {
                                            errorMessages.Add(RMAttributeSetupConstants.MISSING_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);
                                        }
                                        lookupDisplayAttributes = lookupAttributeString.ToString();
                                    }
                                    else if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.SECURITY_LOOKUP)))
                                    {
                                        StringBuilder lookupAttributeString = new StringBuilder();
                                        if (!string.IsNullOrEmpty(lookupDisplayAttribute.Trim()))
                                        {
                                            List<string> tempDisplayAttributes = lookupDisplayAttribute.Split(',').ToList();
                                            if (!tempDisplayAttributes.Contains(lookupAttribute))
                                                errorMessages.Add(RMCommonConstants.INVALID_LOOKUP_DISPLAY_ATTRS);

                                            foreach (string attr in tempDisplayAttributes)
                                            {
                                                if (lookupAttributeString.Length == 0)
                                                {
                                                    if (lookupType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES))
                                                        lookupAttributeString.Append(commonAttributes[attr].AttrId);
                                                    else
                                                        lookupAttributeString.Append(secTypeDetails[lookupType].AttributeInfo.MasterAttrs[attr].AttrId);
                                                }
                                                else
                                                {
                                                    if (lookupType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES))
                                                        lookupAttributeString.Append("," + commonAttributes[attr].AttrId);
                                                    else
                                                        lookupAttributeString.Append("," + secTypeDetails[lookupType].AttributeInfo.MasterAttrs[attr].AttrId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            errorMessages.Add(RMAttributeSetupConstants.MISSING_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);
                                        }
                                        lookupDisplayAttributes = lookupAttributeString.ToString();
                                    }
                                }

                                errorMessages.AddRange(attrValidations.ValidateDefaultValue(dataType, ref defaultValue, dateFormat, attributeLength));

                                if (errorMessages.Count > 0)
                                {
                                    new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                                    isValid = false;
                                }
                                else
                                {
                                    RMDBDataTypes type = RMDBDataTypes.VARCHAR;
                                    switch (dataType.ToUpper())
                                    {
                                        case RMAttributeSetupConstants.DATA_TYPE_VARCHAR:
                                            type = RMDBDataTypes.VARCHAR;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_VARCHAR_MAX:
                                            type = RMDBDataTypes.VARCHARMAX;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_INT:
                                            type = RMDBDataTypes.INT;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_DATETIME:
                                            type = RMDBDataTypes.DATETIME;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_BIT:
                                            type = RMDBDataTypes.BIT;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_DECIMAL:
                                            type = RMDBDataTypes.DECIMAL;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_LOOKUP:
                                            type = RMDBDataTypes.LOOKUP;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_SECURITYLOOKUP:
                                        case RMAttributeSetupConstants.DATA_TYPE_SECURITY_LOOKUP:
                                            type = RMDBDataTypes.SECURITY_LOOKUP;
                                            break;
                                        case RMAttributeSetupConstants.DATA_TYPE_FILE:
                                            type = RMDBDataTypes.FILE;
                                            break;
                                    }
                                    lstAttributesInfo.Add(new RMEntityAttributeInfo()
                                    {
                                        DisplayName = attributeName,
                                        AttributeDescription = attrDescription,
                                        EntityAttributeID = entityAttributeId,
                                        EntityTypeID = legEntityTypeId,
                                        IsClonable = isCloneable,
                                        IsEncrypted = isEncrypted,
                                        IsNullable = !isMandatory,
                                        IsPII = isPII,
                                        DisplayMetaData = dispMetaData,
                                        IsPrimary = isPrimary,
                                        IsSearchView = visibleInSearch,
                                        IsInternal = false,
                                        DataTypeLength = attributeLength,
                                        DataType = type,
                                        DefaultValue = defaultValue,
                                        Tags = tags,
                                        RestrictedChars = restictedCharacters,
                                        SearchViewPosition = searchViewPosition,
                                        AttributeName = attributeRealName,
                                        LookupEntityTypeID = dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)) ? allModulesEntityDetailsInfo[lookupType].EntityTypeID : dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.SECURITY_LOOKUP)) ? lookupType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES) ? 0 : secTypeDetails[lookupType].SectypeId : -1,
                                        LookupAttributeID = dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)) ? allModulesEntityDetailsInfo[lookupType].Attributes[lookupAttribute].EntityAttributeID : dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.SECURITY_LOOKUP)) ? lookupType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES) ? commonAttributes[lookupAttribute].AttrId : secTypeDetails[lookupType].AttributeInfo.MasterAttrs[lookupAttribute].AttrId : -1,
                                        LookupEntityTypeName = lookupType,
                                        LookupAttributeName = lookupAttribute,
                                        LookupDisplayAttributes = lookupDisplayAttributes,
                                        LastModifiedBy = userName,
                                        LastModifiedOn = syncDateTime
                                    });

                                    if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.INT)) || dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.DECIMAL)))
                                    {
                                        attrDispConf.AttributeId = entityAttributeId;
                                        attrDispConf.AllowCommaFormatting = Convert.ToBoolean(showComma);
                                        attrDispConf.AppendMultiplier = Convert.ToBoolean(showMultiplier);
                                        attrDispConf.AppendPercentage = Convert.ToBoolean(showPercentage);
                                        if (dictAttrDispInfo.ContainsKey(attributeName))
                                        {
                                            dictAttrDispInfo[attributeName] = attrDispConf;
                                        }
                                        else
                                            dictAttrDispInfo.Add(attributeName, attrDispConf);
                                    }
                                    else if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.LOOKUP)))
                                    {
                                        attrDispConf.AttributeId = entityAttributeId;
                                        attrDispConf.ShowEntityCode = Convert.ToBoolean(showEntityCode);
                                        if (!string.IsNullOrEmpty(orderByAttribute) && errorMessages.Count == 0)
                                        {
                                            attrDispConf.OrderByAttributeName = orderByAttribute.ToLower().Equals("entity code") ? "entity_code" : entityDetails[lookupType].Attributes.ContainsKey(orderByAttribute) ? entityDetails[lookupType].Attributes[orderByAttribute].AttributeName : orderByAttribute;
                                            attrDispConf.OrderByAttributeId = entityDetails[lookupType].Attributes.ContainsKey(orderByAttribute) ? entityDetails[lookupType].Attributes[orderByAttribute].EntityAttributeID : -1;
                                        }
                                        if (dictAttrDispInfo.ContainsKey(attributeName))
                                        {
                                            dictAttrDispInfo[attributeName] = attrDispConf;
                                        }
                                        else
                                            dictAttrDispInfo.Add(attributeName, attrDispConf);
                                    }
                                    else if (dataType.SRMEqualWithIgnoreCase(Enum.GetName(typeof(RMDBDataTypes), RMDBDataTypes.SECURITY_LOOKUP)))
                                    {
                                        attrDispConf.AttributeId = entityAttributeId;
                                        attrDispConf.ShowEntityCode = Convert.ToBoolean(showEntityCode);
                                        if (dictAttrDispInfo.ContainsKey(attributeName))
                                        {
                                            dictAttrDispInfo[attributeName] = attrDispConf;
                                        }
                                        else
                                            dictAttrDispInfo.Add(attributeName, attrDispConf);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                mLogger.Error("RMEntityTypeModelerMigration-SyncLegAttributes->Error Attribute Info " + ex.Message);

                                isValid = false;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                                //FailDependencies(deltaSet, entityTypeName, true);
                                FailAllDependencies(deltaSet, entityTypeName, null, true, true, true, false, false, false, false, false, false, false, true);
                            }
                        }
                    }
                    if (isValid && isEntityTypeValid)
                    {
                        DataSet attrOutputDs = new RMModelerDBManager().AddUpdateEntityTypeAttributes(lstAttributesInfo, legEntityTypeId, userName, true, mDBCon);
                        //Save
                        mDBCon.CommitTransaction();
                        //Update info with details
                        UpdateLegAttributes(entityDetails, attrOutputDs, lstAttributesInfo, entityTypeName, legName, allModulesEntityDetailsInfo);
                        foreach (KeyValuePair<string, RMAttributeDisplayConfigurationInfo> attrInfo in dictAttrDispInfo)
                        {
                            string attributeName = entityDetails[entityTypeName].Legs[legName].Attributes[attrInfo.Key].AttributeName;
                            int attributeId = entityDetails[entityTypeName].Legs[legName].Attributes[attrInfo.Key].EntityAttributeID;
                            RMAttributeDisplayConfigurationInfo AttrToConsider = attrInfo.Value;
                            AttrToConsider.AttributeId = attributeId;
                            if (!string.IsNullOrEmpty(AttrToConsider.OrderByAttributeName))
                            {
                                AttrToConsider.OrderByAttributeName = AttrToConsider.OrderByAttributeName;
                                AttrToConsider.OrderByAttributeId = AttrToConsider.OrderByAttributeId;
                            }

                            lstAttrDispInfo.Add(AttrToConsider);
                        }

                        foreach (var row in group.ToList())
                        {
                            new RMCommonMigration().SetPassedRow(row, string.Empty);
                        }
                    }
                    else
                    {
                        if (!isValid)
                            FailAllDependencies(deltaSet, entityTypeName, null, true, true, true, false, false, false, false, false, false, false, true);

                        if (!isEntityTypeValid)
                            //FailDependencies(deltaSet, entityTypeName, true);
                            FailAllDependencies(deltaSet, entityTypeName, null, false, false, false, false, false, false, false, false, false, false, false);
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error("RMEntityTypeModelerMigration-SyncLegAttributes->Error Save Attributes " + ex.Message);
                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();
                    //Code to fail dependent rows in other tables
                    foreach (var row in group.ToList())
                    {
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                    }
                    //FailDependencies(deltaSet, entityTypeName, true);
                    FailAllDependencies(deltaSet, entityTypeName, null, true, true, true, false, false, false, false, false, false, false, false);
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }
            }

            if (lstAttrDispInfo != null && lstAttrDispInfo.Count > 0)
            {
                new RMModelerDBManager().UpdateAttributeDisplayConfigurations(lstAttrDispInfo);
            }

            //Remove static columns
            List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMModelerConstants.ENTITY_TYPE_ID, RMModelerConstants.ENTITY_ATTRIBUTE_ID, RMModelerConstants.Attribute_Real_Name };
            new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);

            mLogger.Error("RMEntityTypeModelerMigration-SyncLegAttributes->End");
        }

        private void UpdateAttributes(Dictionary<string, RMEntityDetailsInfo> entityDetails, DataSet attributeOutputDs, List<RMEntityAttributeInfo> lstAttributesInfo, string entityTypeName, Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo)
        {
            int tableCount = attributeOutputDs.Tables.Count;
            foreach (RMEntityAttributeInfo info in lstAttributesInfo)
            {
                if (!entityDetails[entityTypeName].Attributes.ContainsKey(info.DisplayName))
                    entityDetails[entityTypeName].Attributes.Add(info.DisplayName, new RMEntityAttributeInfo());

                entityDetails[entityTypeName].Attributes[info.DisplayName].DisplayName = info.DisplayName;
                entityDetails[entityTypeName].Attributes[info.DisplayName].AttributeName = info.AttributeName;
                entityDetails[entityTypeName].Attributes[info.DisplayName].LookupAttributeID = info.EntityAttributeID;
                entityDetails[entityTypeName].Attributes[info.DisplayName].LookupAttributeName = info.LookupAttributeName;
                entityDetails[entityTypeName].Attributes[info.DisplayName].LookupEntityTypeName = info.LookupEntityTypeName;
                entityDetails[entityTypeName].Attributes[info.DisplayName].LookupEntityTypeID = info.LookupEntityTypeID;
                entityDetails[entityTypeName].Attributes[info.DisplayName].IsNullable = info.IsNullable;
                entityDetails[entityTypeName].Attributes[info.DisplayName].DataType = info.DataType;

                if (!allModulesEntityDetailsInfo[entityTypeName].Attributes.ContainsKey(info.DisplayName))
                    allModulesEntityDetailsInfo[entityTypeName].Attributes.Add(info.DisplayName, new RMEntityAttributeInfo());

                allModulesEntityDetailsInfo[entityTypeName].Attributes[info.DisplayName].DisplayName = info.DisplayName;
                allModulesEntityDetailsInfo[entityTypeName].Attributes[info.DisplayName].AttributeName = info.AttributeName;
                allModulesEntityDetailsInfo[entityTypeName].Attributes[info.DisplayName].LookupAttributeID = info.EntityAttributeID;
                allModulesEntityDetailsInfo[entityTypeName].Attributes[info.DisplayName].LookupAttributeName = info.LookupAttributeName;
                allModulesEntityDetailsInfo[entityTypeName].Attributes[info.DisplayName].LookupEntityTypeName = info.LookupEntityTypeName;
                allModulesEntityDetailsInfo[entityTypeName].Attributes[info.DisplayName].LookupEntityTypeID = info.LookupEntityTypeID;
                allModulesEntityDetailsInfo[entityTypeName].Attributes[info.DisplayName].IsNullable = info.IsNullable;
                allModulesEntityDetailsInfo[entityTypeName].Attributes[info.DisplayName].DataType = info.DataType;
            }

            //For New Attributes Only populate attribute id , rest all would be set from info above.
            foreach (DataRow attribute in attributeOutputDs.Tables[tableCount - 2].Rows)
            {
                string attributeName = Convert.ToString(attribute["display_name"]);
                entityDetails[entityTypeName].Attributes[attributeName].EntityAttributeID = Convert.ToInt32(attribute["entity_attribute_id"]);
                allModulesEntityDetailsInfo[entityTypeName].Attributes[attributeName].EntityAttributeID = Convert.ToInt32(attribute["entity_attribute_id"]);
                //entityDetails[entityTypeName].Attributes[attributeName].LookupAttributeID = Convert.ToInt32(attribute["parent_attribute_id"]);                
            }

            if (entityDetails.ContainsKey(entityTypeName))
            {
                //case of self lookup populate lookup attribute id
                foreach (string key in entityDetails[entityTypeName].Attributes.Keys)
                {
                    if (entityDetails[entityTypeName].Attributes.ContainsKey(key) && entityDetails[entityTypeName].Attributes[key].DataType == RMDBDataTypes.LOOKUP && entityDetails[entityTypeName].Attributes[key].EntityTypeID == entityDetails[entityTypeName].Attributes[key].LookupEntityTypeID)
                    {
                        entityDetails[entityTypeName].Attributes[key].LookupAttributeID = entityDetails[entityTypeName].Attributes[entityDetails[entityTypeName].Attributes[key].LookupAttributeName].EntityAttributeID;
                    }
                }

                foreach (string key in allModulesEntityDetailsInfo[entityTypeName].Attributes.Keys)
                {
                    if (allModulesEntityDetailsInfo[entityTypeName].Attributes.ContainsKey(key) && allModulesEntityDetailsInfo[entityTypeName].Attributes[key].DataType == RMDBDataTypes.LOOKUP && allModulesEntityDetailsInfo[entityTypeName].Attributes[key].EntityTypeID == allModulesEntityDetailsInfo[entityTypeName].Attributes[key].LookupEntityTypeID)
                    {
                        allModulesEntityDetailsInfo[entityTypeName].Attributes[key].LookupAttributeID = allModulesEntityDetailsInfo[entityTypeName].Attributes[allModulesEntityDetailsInfo[entityTypeName].Attributes[key].LookupAttributeName].EntityAttributeID;
                    }
                }
            }
        }

        private void UpdateLegAttributes(Dictionary<string, RMEntityDetailsInfo> entityDetails, DataSet attributeOutputDs, List<RMEntityAttributeInfo> lstAttributesInfo, string entityTypeName, string legName, Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo)
        {
            int tableCount = attributeOutputDs.Tables.Count;
            foreach (RMEntityAttributeInfo info in lstAttributesInfo)
            {
                if (!entityDetails[entityTypeName].Legs[legName].Attributes.ContainsKey(info.DisplayName))
                    entityDetails[entityTypeName].Legs[legName].Attributes.Add(info.DisplayName, new RMEntityAttributeInfo());

                entityDetails[entityTypeName].Legs[legName].Attributes[info.DisplayName].DisplayName = info.DisplayName;
                entityDetails[entityTypeName].Legs[legName].Attributes[info.DisplayName].AttributeName = info.AttributeName;
                entityDetails[entityTypeName].Legs[legName].Attributes[info.DisplayName].LookupAttributeID = info.EntityAttributeID;
                entityDetails[entityTypeName].Legs[legName].Attributes[info.DisplayName].LookupAttributeName = info.LookupAttributeName;
                entityDetails[entityTypeName].Legs[legName].Attributes[info.DisplayName].LookupEntityTypeName = info.LookupEntityTypeName;
                entityDetails[entityTypeName].Legs[legName].Attributes[info.DisplayName].LookupEntityTypeID = info.LookupEntityTypeID;
                entityDetails[entityTypeName].Legs[legName].Attributes[info.DisplayName].IsNullable = info.IsNullable;
                entityDetails[entityTypeName].Legs[legName].Attributes[info.DisplayName].DataType = info.DataType;
                entityDetails[entityTypeName].Legs[legName].Attributes[info.DisplayName].IsPrimary = info.IsPrimary;

                if (!allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes.ContainsKey(info.DisplayName))
                    allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes.Add(info.DisplayName, new RMEntityAttributeInfo());

                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[info.DisplayName].DisplayName = info.DisplayName;
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[info.DisplayName].AttributeName = info.AttributeName;
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[info.DisplayName].LookupAttributeID = info.EntityAttributeID;
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[info.DisplayName].LookupAttributeName = info.LookupAttributeName;
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[info.DisplayName].LookupEntityTypeName = info.LookupEntityTypeName;
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[info.DisplayName].LookupEntityTypeID = info.LookupEntityTypeID;
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[info.DisplayName].IsNullable = info.IsNullable;
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[info.DisplayName].DataType = info.DataType;
                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[info.DisplayName].IsPrimary = info.IsPrimary;
            }

            //For New Attributes Only populate attribute id , rest all would be set from info above.
            foreach (DataRow attribute in attributeOutputDs.Tables[tableCount - 1].Rows)
            {
                string attributeName = Convert.ToString(attribute["display_name"]);
                entityDetails[entityTypeName].Legs[legName].Attributes[attributeName].EntityAttributeID = Convert.ToInt32(attribute["entity_attribute_id"]);

                allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[attributeName].EntityAttributeID = Convert.ToInt32(attribute["entity_attribute_id"]);
            }

        }

        public void SyncUniqueKeys(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingUniqueKeys, ObjectTable sourceTable, string userName, Dictionary<string, RMEntityDetailsInfo> entityDetails)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncUniqueKeys->Start");

            bool isInsert = false;

            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.UNIQUE_KEY_ID, DataType = typeof(Int32), DefaultValue = -1 });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });
            new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

            //when existing key
            var assign = from db in existingUniqueKeys.AsEnumerable()
                         join upl in sourceTable.AsEnumerable()
                         on new { eName = ConvertToLower(db[RMModelerConstants.ENTITY_DISPLAY_NAME]), level = ConvertToLower(db[RMModelerConstants.Level]), keyName = ConvertToLower(db[RMModelerConstants.Key_Name]) }
                         equals new { eName = ConvertToLower(upl[RMModelerConstants.ENTITY_DISPLAY_NAME]), level = ConvertToLower(upl[RMModelerConstants.Level]), keyName = ConvertToLower(upl[RMModelerConstants.Key_Name]) }

                         select AssignExistingUniqueKeys(db, upl);

            assign.Count();

            var uniqueKeyGroups = currentRows
                .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS]))).GroupBy(x => new
                {
                    EntityTypeName = Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]),
                    Level = Convert.ToString(x[RMModelerConstants.Level]),
                    KeyName = Convert.ToString(x[RMModelerConstants.Key_Name])
                });

            foreach (var group in uniqueKeyGroups)
            {
                List<string> errorMessages = new List<string>();
                bool isValid = true;
                RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                try
                {
                    string entityTypeName = Convert.ToString(group.Key.EntityTypeName);
                    string keyName = Convert.ToString(group.Key.KeyName);
                    string level = Convert.ToString(group.Key.Level);
                    int entityTypeId = 0;
                    RMUniquenessSetupKeyInfo infoObj = new RMUniquenessSetupKeyInfo();
                    List<RMUniquenessSetupAttributeInfo> lstSeletcedAttr = new List<RMUniquenessSetupAttributeInfo>();
                    foreach (var row in group)
                    {

                        try
                        {
                            isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);
                            string legName = Convert.ToString(row[RMModelerConstants.Leg_Name]);
                            bool isAcrossEntities = string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Across_Entities])) ? false : Convert.ToString(row[RMModelerConstants.Is_Across_Entities]).ToLower().Equals("yes") ? true : false;
                            bool checkInDrafts = string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Check_In_Drafts])) ? false : Convert.ToString(row[RMModelerConstants.Check_In_Drafts]).ToLower().Equals("yes") ? true : false;
                            bool checkInWorkFlows = string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Check_In_Workflows])) ? false : Convert.ToString(row[RMModelerConstants.Check_In_Workflows]).ToLower().Equals("yes") ? true : false;
                            bool nullAsUnique = string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Consider_Null_As_Unique])) ? false : Convert.ToString(row[RMModelerConstants.Consider_Null_As_Unique]).ToLower().Equals("yes") ? true : false;
                            string attrName = Convert.ToString(row[RMModelerConstants.Attribute_Name]);
                            int uniqueKeyId = 0;
                            RMEntityTypeSetupSync uniquevalidate = new RMEntityTypeSetupSync();

                            if (!entityDetails.ContainsKey(entityTypeName))
                                errorMessages.Add("Invalid entity type " + entityTypeName);

                            else
                            {
                                errorMessages.AddRange(uniquevalidate.validateUniqueKeyName(keyName));
                                errorMessages.AddRange(uniquevalidate.validateUniqueKeyAttributes(attrName));
                                var lstAttributes = attrName.Trim().Split(',').ToList();

                                if (!string.IsNullOrEmpty(legName))
                                {
                                    if (!entityDetails[entityTypeName].Legs.ContainsKey(legName))
                                        errorMessages.Add("Invalid Leg " + legName);

                                    if (!level.Equals("Leg Level"))
                                        errorMessages.Add("Level must be set as Leg Level.");

                                    if (!isAcrossEntities && (checkInWorkFlows || checkInDrafts))
                                        errorMessages.Add("Check in Workflows and Check In Drafts column cannot be set 'Yes' if unique key is within entity.");
                                }
                                else
                                {
                                    if (!level.Equals("Attribute Level"))
                                        errorMessages.Add("Level must be set as Attribute Level.");

                                    if (!isAcrossEntities)
                                        errorMessages.Add("Is Acrosss Entity cannot be set as 'No' for attribute level unique key.");
                                }


                                if (!string.IsNullOrEmpty(legName) && entityDetails[entityTypeName].Legs.ContainsKey(legName))
                                    entityTypeId = entityDetails[entityTypeName].Legs[legName].EntityTypeID;

                                else if (string.IsNullOrEmpty(legName))
                                    entityTypeId = Convert.ToInt32(entityDetails[entityTypeName].EntityTypeID);

                                foreach (var elem in lstAttributes)
                                {
                                    RMUniquenessSetupAttributeInfo attrinfo = new RMUniquenessSetupAttributeInfo();
                                    if (string.IsNullOrEmpty(legName) && entityDetails[entityTypeName].Attributes.ContainsKey(elem))
                                        attrinfo.AttributeID = entityDetails[entityTypeName].Attributes[elem].EntityAttributeID;

                                    else if (entityDetails[entityTypeName].Legs.ContainsKey(legName) && entityDetails[entityTypeName].Legs[legName].Attributes.ContainsKey(elem))
                                        attrinfo.AttributeID = entityDetails[entityTypeName].Legs[legName].Attributes[elem].EntityAttributeID;

                                    else
                                        errorMessages.Add("Invalid Attribute " + elem);

                                    attrinfo.AttributeName = elem;
                                    lstSeletcedAttr.Add(attrinfo);
                                }



                                infoObj.EntityTypeID = entityTypeId;
                                infoObj.KeyName = keyName;
                                infoObj.IsLeg = string.IsNullOrEmpty(legName) ? false : true;
                                infoObj.IsAcrossEntities = isAcrossEntities;
                                infoObj.CheckInDrafts = checkInDrafts;
                                infoObj.CheckInWorkflows = checkInWorkFlows;
                                infoObj.NullAsUnique = nullAsUnique;
                                infoObj.SelectedAttributes = lstSeletcedAttr;

                                if (!isInsert)
                                {
                                    uniqueKeyId = Convert.ToInt32(row[RMModelerConstants.UNIQUE_KEY_ID]);
                                    infoObj.KeyID = uniqueKeyId;
                                }
                            }
                            if (errorMessages.Count > 0)
                            {
                                isValid = false;
                                new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            mLogger.Error("RMEntityTypeModelerMigration-SyncUniqueKeys->Error " + ex.Message);

                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                        }
                    }

                    if (isValid)
                    {
                        Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                        Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMUniquenessSetupController");
                        string result = string.Empty;

                        if (isInsert)
                        {
                            MethodInfo CreateUniqueKey = objType.GetMethod("RMCreateUniqueKey");
                            result = (string)CreateUniqueKey.Invoke(null, new object[] { userName, infoObj, mDBCon });
                        }
                        else
                        {
                            MethodInfo UpdateUniqueKey = objType.GetMethod("RMUpdateUniqueKey");
                            result = (string)UpdateUniqueKey.Invoke(null, new object[] { userName, infoObj, mDBCon });
                        }
                        if (!string.IsNullOrEmpty(result))
                        {
                            var toCommit = result.Split(new string[] { "@&@" }, StringSplitOptions.None);
                            if (Convert.ToInt32(toCommit[0]) == 1)
                            {
                                mDBCon.CommitTransaction();
                                foreach (var row in group.ToList())
                                {
                                    new RMCommonMigration().SetPassedRow(row, string.Empty);
                                }
                            }
                            else
                            {
                                mDBCon.RollbackTransaction();
                                foreach (var row in group.ToList())
                                {
                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { Convert.ToString(toCommit[1]) }, false);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                catch (Exception ex)
                {
                    mLogger.Error("RMEntityTypeModelerMigration-SyncUniqueKeys->Error " + ex.Message);

                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();
                    //Code to fail dependent rows in other tables
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }
            }

            //Remove static columns
            List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMModelerConstants.UNIQUE_KEY_ID };

            new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);
            mLogger.Error("RMEntityTypeModelerMigration-SyncUniqueKeys->End");
        }

        public void SyncLayouts(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingLayouts, ObjectTable sourceTable, string userName, Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, Dictionary<string, RMEntityDetailsInfo> entityDetails, Dictionary<string, string> hshUsers, List<string> hshGroups)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncLayouts->Start");

            RMLayoutMangementValidations layoutvalidation = new RMLayoutMangementValidations(dbEntityTypesDetails);
            bool isInsert = true;

            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.TEMPLATE_ID, DataType = typeof(Int32), DefaultValue = -1 });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });
            new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

            Dictionary<string, List<int>> dictPossibleLayoutStates = new RMModelerDBManager().GetPossibleLayoutStates();

            //existing layouts
            var assign = from db in existingLayouts.AsEnumerable()
                         join upl in sourceTable.AsEnumerable()
                         on new { eName = ConvertToLower(db[RMModelerConstants.ENTITY_DISPLAY_NAME]), layout = ConvertToLower(db[RMModelerConstants.Layout_Name]) }
                         equals new { eName = ConvertToLower(upl[RMModelerConstants.ENTITY_DISPLAY_NAME]), layout = ConvertToLower(upl[RMModelerConstants.Layout_Name]) }
                         select AssignExistingLayoutIds(db, upl);

            assign.Count();

            currentRows
               .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS]))).ToList().ForEach(row =>
               {
                   RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                   List<string> errorMessages = new List<string>();
                   bool isValid = true;
                   int template_id = 0;
                   string layoutName = string.Empty;
                   string entityTypeName = string.Empty;
                   try
                   {
                       isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);
                       entityTypeName = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]);
                       layoutName = Convert.ToString(row[RMModelerConstants.Layout_Name]);
                       string dependentName = Convert.ToString(row[RMModelerConstants.Dependent_Name]);
                       string layoutType = Convert.ToString(row[RMModelerConstants.Layout_Type]);
                       string entityStates = Convert.ToString(row[RMModelerConstants.Entity_States]);
                       int entityTypeId = 0;
                       int templatetypeId = 0;
                       string dependentId = string.Empty;
                       Dictionary<string, TabDetails> tabInfo = new Dictionary<string, TabDetails>();
                       //entityDetails[entityTypeName].Legs.ToList();
                       if (!entityDetails.ContainsKey(entityTypeName) || !entityTypeVstemplateInfo.ContainsKey(entityTypeName))
                           errorMessages.Add("Entity Type does not exist.");

                       else
                       {
                           errorMessages.AddRange(layoutvalidation.validateLayoutName(entityTypeName, layoutName));
                           errorMessages.AddRange(layoutvalidation.validateLayoutType(entityTypeName, layoutType, dependentName));
                           if (entityDetails.ContainsKey(entityTypeName))
                           {
                               errorMessages.AddRange(layoutvalidation.validateEntityStates(entityStates, entityDetails[entityTypeName].EntityTypeID, dictPossibleLayoutStates != null ? dictPossibleLayoutStates : new Dictionary<string, List<int>>()));
                           }
                       }

                       if (layoutType.ToLower().Equals(RMModelerConstants.USER))
                       {
                           if (!hshUsers.SRMContainsWithIgnoreCase(dependentName))
                               errorMessages.Add("Invalid dependent name - " + dependentName);
                       }
                       else if (layoutType.ToLower().Equals(RMModelerConstants.GROUP))
                       {
                           if (!hshGroups.SRMContainsWithIgnoreCase(dependentName))
                               errorMessages.Add("Invalid dependent name - " + dependentName);
                       }
                       if (errorMessages.Count > 0)
                       {
                           new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                           isValid = false;
                       }
                       else
                       {
                           RMTemplateInfo tempInfoObj = new RMTemplateInfo();
                           action_type action;
                           entityTypeId = Convert.ToInt32(entityDetails[entityTypeName].EntityTypeID);
                           if (layoutType.ToLower().Equals(RMModelerConstants.SYSTEM))
                           {
                               templatetypeId = 1;
                               dependentId = "SYSTEM";
                           }
                           else if (layoutType.ToLower().Equals(RMModelerConstants.GROUP))
                           {
                               templatetypeId = 2;
                               dependentId = dependentName;
                           }
                           else if (layoutType.ToLower().Equals(RMModelerConstants.USER))
                           {
                               templatetypeId = 3;
                               dependentId = hshUsers[dependentName];
                           }

                           tempInfoObj.DependentId = dependentId;
                           tempInfoObj.TemplateName = layoutName;
                           tempInfoObj.TemplateTypeId = templatetypeId;
                           tempInfoObj.EntityTypeId = entityTypeId;
                           tempInfoObj.CreatedBy = userName;
                           tempInfoObj.EntityStates = entityStates;

                           if (!entityTypeVstemplateInfo[entityTypeName].ContainsKey(layoutName) && isInsert)
                           {
                               action = action_type.insert;
                               CreateLayoutInfo layoutInfoObj = (CreateLayoutInfo)new RMModelerDBManager().SaveLayoutInfo(tempInfoObj, action, mDBCon);
                               template_id = layoutInfoObj.templateId;
                               tabInfo = layoutInfoObj.TabNameVsTabNameId;
                           }
                           else
                           {
                               template_id = Convert.ToInt32(row[RMModelerConstants.TEMPLATE_ID]);
                               tempInfoObj.TemplateId = template_id;
                               action = action_type.update;
                               new RMModelerDBManager().UpdateLayoutInfo(tempInfoObj, action, mDBCon);
                           }

                       }
                       if (isValid)
                       {
                           mDBCon.CommitTransaction();
                           //mDBCon.RollbackTransaction();
                           new RMCommonMigration().SetPassedRow(row, string.Empty);
                           updateLayoutInfo(entityTypeVstemplateInfo, layoutName, template_id, tabInfo, entityTypeName);
                       }
                       else
                       {
                           //FailLayoutDependencies(deltaSet, layoutName);
                           FailAllDependencies(deltaSet, entityTypeName, layoutName, true, true, true, true, true, true, true, false, false, false, true);
                       }

                   }
                   catch (Exception ex)
                   {
                       mLogger.Error("RMEntityTypeModelerMigration-SyncLayouts->Error " + ex.Message);

                       if (mDBCon != null)
                           mDBCon.RollbackTransaction();
                       //FailLayoutDependencies(deltaSet, layoutName);
                       FailAllDependencies(deltaSet, entityTypeName, layoutName, true, true, true, true, true, true, true, false, false, false, true);
                   }
                   finally
                   {
                       if (mDBCon != null)
                       {
                           RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                           mDBCon = null;
                       }
                   }
               });


            //Remove static columns
            List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMModelerConstants.TEMPLATE_ID };

            new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);

            mLogger.Error("RMEntityTypeModelerMigration-SyncLayouts->End");
        }

        public void SyncLayoutPreference(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingLayouts, ObjectTable sourceTable, string userName, Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, Dictionary<string, RMEntityDetailsInfo> entityDetails, Dictionary<string, string> hshUsers, List<RUserInfo> userInfo)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncLayoutPreference->Start");

            //RMLayoutMangementValidations layoutvalidation = new RMLayoutMangementValidations(dbEntityTypesDetails);
            Dictionary<string, Dictionary<string, List<string>>> dctEntityTypeVsUserVsGroupLayoutName = new Dictionary<string, Dictionary<string, List<string>>>();
            bool isInsert = true;
            Dictionary<string, HashSet<string>> groupVsUsernames = new Dictionary<string, HashSet<string>>();
            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.TEMPLATE_ID, DataType = typeof(Int32), DefaultValue = -1 });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });
            new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

            //Dictionary<string, string> hshUsers = SRMCommonRAD.GetAllDisplayNamevsUsersLoginName();
            //var userInfo = new RUserManagementService().GetAllUsersGDPR();

            if (userInfo != null && userInfo.Count > 0)
            {
                foreach (var info in userInfo)
                {
                    //radUsers.Add(info.UserName);
                    var fullusername = SRMCommonRAD.GetUserDisplayNameWithUserNameFromInfo(info);

                    foreach (var grp in info.Groups)
                    {
                        if (!groupVsUsernames.ContainsKey(grp))
                        {
                            groupVsUsernames.Add(grp, new HashSet<string>());
                        }
                        groupVsUsernames[grp].Add(fullusername);
                    }
                }
            }

            Dictionary<string, Dictionary<string, List<string>>> dictEntityTypeVsGroupLayoutVsEntityStates;
            Dictionary<string, Dictionary<string, string>> dictEntityTypeVsGroupLayoutVsUser = new RMModelerDBManager().getAllGroupLayout(out dictEntityTypeVsGroupLayoutVsEntityStates);
            List<RMTemplatePreferenceInfo> LstValidRowsInsert = new List<RMTemplatePreferenceInfo>();
            List<int> lstEntityTypeIds = new List<int>();
            RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);

            //existing layouts combination of entity type and user
            var assign = from db in existingLayouts.AsEnumerable()
                         join upl in sourceTable.AsEnumerable()
                         on new { eName = ConvertToLower(db[RMModelerConstants.ENTITY_DISPLAY_NAME]), layout = ConvertToLower(db[RMModelerConstants.Group_Level_Layout]) }
                         equals new { eName = ConvertToLower(upl[RMModelerConstants.ENTITY_DISPLAY_NAME]), layout = ConvertToLower(upl[RMModelerConstants.Group_Level_Layout]) }
                         select AssignExistingLayoutIds(db, upl);

            assign.Count();

            try
            {
                currentRows
               .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS]))).ToList().ForEach(row =>
               {

                   List<string> errorMessages = new List<string>();
                   bool isValid = true;
                   int template_id = 0;
                   string groupLayoutName = string.Empty;
                   string entityTypeName = string.Empty;

                   isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);
                   entityTypeName = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]);
                   groupLayoutName = Convert.ToString(row[RMModelerConstants.Group_Level_Layout]);
                   string LayoutUserName = Convert.ToString(row[RMModelerConstants.User_Name]);


                   if (!entityDetails.ContainsKey(entityTypeName) || !entityTypeVstemplateInfo.ContainsKey(entityTypeName))
                       errorMessages.Add("Entity Type does not exist.");

                   else if (!entityTypeVstemplateInfo[entityTypeName].ContainsKey(groupLayoutName))
                       errorMessages.Add("Layout does not exist.");

                   else
                   {
                       if (LayoutUserName.Split(',').Count() > 1)
                           errorMessages.Add("Multiple User Name present.");
                       if (groupLayoutName.Split(',').Count() > 1)
                           errorMessages.Add("Multiple group layout names present.");
                       if (!hshUsers.SRMContainsWithIgnoreCase(LayoutUserName))
                           errorMessages.Add("Invalid User Name.");
                       if (dictEntityTypeVsGroupLayoutVsUser != null && dictEntityTypeVsGroupLayoutVsUser.Count > 0 && dictEntityTypeVsGroupLayoutVsUser.ContainsKey(entityTypeName) && dictEntityTypeVsGroupLayoutVsUser[entityTypeName] != null)
                       {
                           if (!dictEntityTypeVsGroupLayoutVsUser[entityTypeName].ContainsKey(groupLayoutName))
                               errorMessages.Add("Incorrect group layout.");
                           else
                           {
                               string templateGrpDB = dictEntityTypeVsGroupLayoutVsUser[entityTypeName][groupLayoutName];
                               if (!groupVsUsernames.ContainsKey(templateGrpDB) || !groupVsUsernames[templateGrpDB].Contains(LayoutUserName))
                                   errorMessages.Add("User not a part of the Group.");
                           }
                       }

                   }

                   Dictionary<string, List<string>> dctTemp = new Dictionary<string, List<string>>();
                   List<string> lstTemp = new List<string>();
                   if (!dctEntityTypeVsUserVsGroupLayoutName.TryGetValue(entityTypeName, out dctTemp))
                   {
                       dctTemp = new Dictionary<string, List<string>>();
                       dctEntityTypeVsUserVsGroupLayoutName.Add(entityTypeName, dctTemp);
                   }

                   if (!dctTemp.TryGetValue(LayoutUserName, out lstTemp))
                   {
                       lstTemp = new List<string>() { groupLayoutName };
                       dctTemp.Add(LayoutUserName, lstTemp);
                   }
                   else
                   {
                       lstTemp.Add(groupLayoutName);
                       foreach (var layoutName in lstTemp)
                       {
                           if (dictEntityTypeVsGroupLayoutVsEntityStates.ContainsKey(entityTypeName) && dictEntityTypeVsGroupLayoutVsEntityStates[entityTypeName].ContainsKey(groupLayoutName) && groupLayoutName != layoutName)
                           {
                               if (dictEntityTypeVsGroupLayoutVsEntityStates[entityTypeName][groupLayoutName].Intersect(dictEntityTypeVsGroupLayoutVsEntityStates[entityTypeName][layoutName]).Count() > 0)
                                   errorMessages.Add("Layout already prioritized for this user in the input sheet");
                           }
                       }
                   }

                   if (errorMessages.Count > 0)
                   {
                       new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                       isValid = false;
                   }
                   else
                   {
                       RMTemplatePreferenceInfo tempInfoObj = new RMTemplatePreferenceInfo();

                       tempInfoObj.TemplateId = entityTypeVstemplateInfo[entityTypeName][groupLayoutName].TemplateId;
                       tempInfoObj.UserName = hshUsers[LayoutUserName];
                       //if (!LstValidRowsInsert.Contains(tempInfoObj))
                       LstValidRowsInsert.Add(tempInfoObj);

                       if (!lstEntityTypeIds.Contains(entityDetails[entityTypeName].EntityTypeID))
                           lstEntityTypeIds.Add(entityDetails[entityTypeName].EntityTypeID);

                       new RMCommonMigration().SetPassedRow(row, string.Empty);

                   }
               });
                if (LstValidRowsInsert != null && LstValidRowsInsert.Count > 0)
                {
                    new RMModelerDBManager().InsertUpdateLayoutPreference(LstValidRowsInsert, lstEntityTypeIds, userName, mDBCon);
                    mDBCon.CommitTransaction();
                }
            }

            catch (Exception ex)
            {
                mLogger.Error("RMEntityTypeModelerMigration-SyncLayouts->Error " + ex.Message);

                if (mDBCon != null)
                    mDBCon.RollbackTransaction();
                //FailLayoutDependencies(deltaSet, layoutName);
                //FailAllDependencies(deltaSet, entityTypeName, layoutName, true, true, true, true, true, true, true, false, false, false, true);
            }
            finally
            {
                if (mDBCon != null)
                {
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                    mDBCon = null;
                }
            }


            //Remove static columns
            List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMModelerConstants.TEMPLATE_ID };

            new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);

            mLogger.Error("RMEntityTypeModelerMigration-SyncLayouts->End");
        }

        private void updateLayoutInfo(Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, string layoutName, int templateId, Dictionary<string, TabDetails> tabinfo, string entityTypeName)
        {
            if (!entityTypeVstemplateInfo[entityTypeName].ContainsKey(layoutName))
            {
                entityTypeVstemplateInfo[entityTypeName].Add(layoutName, new RMEntityTypeTemplateDetails());
                entityTypeVstemplateInfo[entityTypeName][layoutName].TemplateId = templateId;
                entityTypeVstemplateInfo[entityTypeName][layoutName].TemplateName = layoutName;
                entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId = new Dictionary<string, RMEntityTabDetails>(StringComparer.OrdinalIgnoreCase);

                //tabinfo.Keys.Except(entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId.Keys).ToList().ForEach(k => entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId.Add(k, tabinfo[k]));
                tabinfo.Keys.Except(entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId.Keys).ToList().ForEach(k => entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId.Add(k, new RMEntityTabDetails() { tabName = tabinfo[k].tabName, tabNameId = tabinfo[k].tabNameId, tabOrder = tabinfo[k].tabOrder }));
            }

        }

        public void SyncTabs(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingLayouts, ObjectTable sourceTable, string userName, Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, Dictionary<string, RMEntityDetailsInfo> entityDetails)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncTabs->Start");

            RMLayoutMangementValidations layoutvalidation = new RMLayoutMangementValidations(dbEntityTypesDetails);
            bool isInsert = false;

            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.TAB_NAME_ID, DataType = typeof(Int32), DefaultValue = -1 });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });
            new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

            var assign = from db in existingLayouts.AsEnumerable()
                         join upl in sourceTable.AsEnumerable()
                         on new { eName = ConvertToLower(db[RMModelerConstants.ENTITY_DISPLAY_NAME]), layout = ConvertToLower(db[RMModelerConstants.Layout_Name]), tabName = ConvertToLower(db[RMModelerConstants.Tab_Name]) }
                         equals new { eName = ConvertToLower(upl[RMModelerConstants.ENTITY_DISPLAY_NAME]), layout = ConvertToLower(upl[RMModelerConstants.Layout_Name]), tabName = ConvertToLower(upl[RMModelerConstants.Tab_Name]) }

                         select AssignExistingTabs(db, upl);

            assign.Count();

            //var nonEmptyGroups = currentRows
            //    .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS])));
            //var layoutGroups = nonEmptyGroups.GroupBy(x => Convert.ToString(x[RMModelerConstants.Layout_Name]));

            var layoutGroups = currentRows
                .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS]))).GroupBy(x => new
                {
                    EntityTypeName = Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]),
                    Layout = Convert.ToString(x[RMModelerConstants.Layout_Name])

                });

            foreach (var group in layoutGroups)
            {
                RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                List<string> errorMessages = new List<string>();
                bool isValid = true;
                bool entityNameTabExist = true;
                string tabName = string.Empty;
                string layoutName = group.Key.Layout;
                string entityTypeName = group.Key.EntityTypeName;
                int tabOrder = 0;
                int tabNameId = 0;
                List<string> tabNamesInSheet = group.Select(x => x.Field<string>(RMModelerConstants.Tab_Name)).Distinct().ToList();
                try
                {
                    foreach (var row in group.ToList())
                    {
                        isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);
                        //entityTypeName = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]);
                        //layoutName = Convert.ToString(row[RMModelerConstants.Layout_Name]);
                        tabName = Convert.ToString(row[RMModelerConstants.Tab_Name]);
                        tabOrder = Convert.ToInt32(row[RMModelerConstants.Tab_Order]);
                        int entityTypeId = 0;
                        int templateId = 0;
                        int tabId = 0;
                        //string Action = Convert.ToString(row["Action"]);

                        if (!entityTypeVstemplateInfo.ContainsKey(entityTypeName))
                            errorMessages.Add("Entity Type does not exist.");

                        else if (!entityTypeVstemplateInfo[entityTypeName].ContainsKey(layoutName))
                            errorMessages.Add("Layout does not exist.");

                        else if (!entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId.ContainsKey(tabName) && !isInsert)
                            errorMessages.Add("Duplicate tab order present.");

                        else
                        {
                            errorMessages.AddRange(layoutvalidation.validateTabName(layoutName, tabName));
                            if (entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId.ContainsKey(tabName))
                                entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId[tabName].tabOrder = tabOrder;
                            //errorMessages.AddRange(layoutvalidation.validateTabOrder(entityTypeVstemplateInfo, entityTypeName, layoutName, tabName, tabOrder));
                        }

                        if (errorMessages.Count > 0)
                        {
                            new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                            isValid = false;
                        }
                        else
                        {
                            RMTabManagementInfo objTabManagementInfo = new RMTabManagementInfo();
                            RMModelerDBManager objTabManagementController = new RMModelerDBManager();
                            entityTypeId = Convert.ToInt32(entityDetails[entityTypeName].EntityTypeID);
                            templateId = entityTypeVstemplateInfo[entityTypeName][layoutName].TemplateId;// Convert.ToInt32(row[RMModelerConstants.TEMPLATE_ID]);
                            tabId = objTabManagementController.GetEntityTabIdByTypeId(entityTypeId, templateId);
                            objTabManagementInfo.CreatedBy = userName;
                            objTabManagementInfo.LastModifiedBy = userName;
                            objTabManagementInfo.TemplateId = templateId;
                            objTabManagementInfo.TabEntityTypeId = entityTypeId;
                            objTabManagementInfo.TabId = tabId;
                            objTabManagementInfo.TabIndex = tabOrder;

                            if (!tabName.Equals(entityTypeName) && !tabNamesInSheet.Contains(entityTypeName))
                            {
                                if (tabOrder == 1 && entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId.ContainsKey(entityTypeName))
                                {
                                    entityNameTabExist = false;
                                    isInsert = false;
                                }
                            }

                            objTabManagementInfo.TabName = tabName;


                            if (!entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId.ContainsKey(tabName) && isInsert)
                            {
                                tabNameId = objTabManagementController.SaveEntityTabDetails(objTabManagementInfo.TabId, objTabManagementInfo, mDBCon);
                            }
                            else
                            {
                                if (!entityNameTabExist)
                                    tabNameId = Convert.ToInt32(entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId[entityTypeName].tabNameId);
                                else
                                    tabNameId = Convert.ToInt32(entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId[tabName].tabNameId);
                                //tabNameId = Convert.ToInt32(entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId[tabName]);
                                objTabManagementInfo.TabNameId = tabNameId;
                                objTabManagementController.UpdateEntityTabDetails(objTabManagementInfo.TabId, objTabManagementInfo, mDBCon);
                            }

                        }
                        if (isValid)
                        {
                            new RMCommonMigration().SetPassedRow(row, string.Empty);
                            updateTabInfo(entityTypeVstemplateInfo, tabName, tabNameId, tabOrder, layoutName, entityTypeName);
                        }
                    }
                    if (!layoutvalidation.checkAllOrderValuesUnderMaxExist(group.AsEnumerable(), RMModelerConstants.Tab_Order, true))
                    {
                        isValid = false;
                        errorMessages.Add(RMCommonConstants.INVALID_TAB_ORDER);
                        foreach (var row in group.ToList())
                            new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                    }
                    if (isValid)
                    {
                        mDBCon.CommitTransaction();

                    }
                    else
                    {
                        //FailTabDependencies(deltaSet, layoutName);
                        if (mDBCon != null)
                            mDBCon.RollbackTransaction();
                        FailAllDependencies(deltaSet, entityTypeName, layoutName, true, true, true, true, true, true, true, true, false, true, true);
                    }

                }
                catch (Exception ex)
                {
                    mLogger.Error("RMEntityTypeModelerMigration-SyncTabs->Error " + ex.Message);

                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();
                    //FailTabDependencies(deltaSet, layoutName);
                    FailAllDependencies(deltaSet, entityTypeName, layoutName, true, true, true, true, true, true, true, true, false, true, true);
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }

            }


            //Remove static columns
            List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMModelerConstants.TAB_NAME_ID };

            new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);

            mLogger.Error("RMEntityTypeModelerMigration-SyncTabs->End");
        }

        private bool AssignExistingEntityTypeIds(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[RMModelerConstants.ENTITY_TYPE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.ENTITY_TYPE_ID]);
            return true;
        }

        private bool AssignExistingRules(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[RMModelerConstants.ENTITY_TYPE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.ENTITY_TYPE_ID]);
            sourceRow[RMModelerConstants.Rule_ID] = Convert.ToInt32(dbRow[RMModelerConstants.Rule_ID]);
            sourceRow[RMModelerConstants.Rule_Set_ID] = Convert.ToInt32(dbRow[RMModelerConstants.Rule_Set_ID]);

            sourceRow[COL_IS_INSERT] = false;
            return true;
        }

        private void updateTabInfo(Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, string tabName, int tabNameId, int tabOrder, string layoutName, string entityTypeName)
        {
            if (!entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId.ContainsKey(tabName))
            {
                entityTypeVstemplateInfo[entityTypeName][layoutName].TabNameVsId.Add(tabName, new RMEntityTabDetails()
                {
                    tabName = tabName,
                    tabNameId = tabNameId,
                    tabOrder = tabOrder
                });
            }
        }

        public void SyncAttributeManagement(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingLayoutAttributes, ObjectTable sourceTable, string userName, Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, Dictionary<string, RMEntityDetailsInfo> entityDetails)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeManagement-> Start");

            RMLayoutMangementValidations layoutvalidation = new RMLayoutMangementValidations(dbEntityTypesDetails);

            var layoutAttrGroups = currentRows
                .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS]))).GroupBy(x => new
                {
                    EntityTypeName = Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]),
                    Layout = Convert.ToString(x[RMModelerConstants.Layout_Name])
                });


            foreach (var group in layoutAttrGroups)
            {
                List<string> errorMessages = new List<string>();
                bool isValid = true;
                RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                try
                {
                    string entitytypeName = Convert.ToString(group.Key.EntityTypeName);
                    string layoutName = Convert.ToString(group.Key.Layout);
                    RMAttributeManagementInfo objTemplateManagerInfo = new RMAttributeManagementInfo();
                    List<string> attrToValidate = new List<string>();
                    XElement leftSection = new XElement("Left");
                    XElement middleSection = new XElement("Middle");
                    XElement rightSection = new XElement("Right");
                    XElement accordianSection = new XElement("Accordian");
                    XElement attributeXML = new XElement("Template");
                    XElement serverControlsXML = new XElement("ServerControls");
                    XElement detailsXML = new XElement("Details");
                    attributeXML.Add(serverControlsXML);
                    attributeXML.Add(detailsXML);
                    string workingTab = string.Empty;
                    string legName = string.Empty;
                    int workingTabId = 0;
                    string workingLegName = string.Empty;
                    int entitytypeId = 0;
                    int templateId = 0;

                    List<ObjectRow> attributeRows = group.ToList();
                    //MergeExistingAttributes(attributeRows, existingLayoutAttributes, layoutName, entitytypeName); //merging attributes

                    attributeRows.OrderBy(x => new { LegName = x[RMModelerConstants.Leg_Name], TabName = x[RMModelerConstants.Tab_Name] });

                    foreach (var row in attributeRows)
                    {
                        errorMessages = new List<string>();
                        legName = Convert.ToString(row[RMModelerConstants.Leg_Name]);
                        string tabName = Convert.ToString(row[RMModelerConstants.Tab_Name]);
                        string attrName = Convert.ToString(row[RMModelerConstants.Attribute_Name]);
                        string panel = Convert.ToString(row[RMModelerConstants.Panel]);
                        int Order = Convert.ToInt32(row[RMModelerConstants.Order]);
                        bool isVisible = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Visible])) ? Convert.ToBoolean(row[RMModelerConstants.Is_Visible]) : false;
                        bool isMandatory = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Mandatory])) ? Convert.ToBoolean(row[RMModelerConstants.Is_Mandatory]) : false;
                        bool isReadOnly = !string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Is_Read_Only])) ? Convert.ToBoolean(row[RMModelerConstants.Is_Read_Only]) : false;
                        bool isNameModified = false;

                        if (!entityDetails.ContainsKey(entitytypeName) && !entityTypeVstemplateInfo.ContainsKey(entitytypeName))
                            errorMessages.Add("Entity Type does not exist.");

                        else if (!entityTypeVstemplateInfo[entitytypeName].ContainsKey(layoutName))
                            errorMessages.Add("Layout does not exist.");

                        else
                        {
                            if (string.IsNullOrEmpty(legName))
                            {
                                if (!entityDetails[entitytypeName].Attributes.ContainsKey(attrName))
                                    errorMessages.Add("Attribute does not exist.");

                                if (entityDetails[entitytypeName].Attributes.ContainsKey(attrName))
                                {
                                    if (Convert.ToBoolean(entityDetails[entitytypeName].Attributes[attrName].IsNullable) == false && !isMandatory)
                                        errorMessages.Add("Mandatory attribute cannot be marked as non mandatory");

                                    errorMessages.AddRange(layoutvalidation.validateCheckBoxList(isMandatory, isVisible, isReadOnly, false));

                                    attrToValidate.Add(attrName);
                                }
                            }
                            if (!string.IsNullOrEmpty(legName))
                            {
                                if (!entityDetails[entitytypeName].Legs.ContainsKey(legName))
                                    errorMessages.Add("Leg does not exist.");

                                if (!entityDetails[entitytypeName].Legs[legName].Attributes.ContainsKey(attrName))
                                    errorMessages.Add("Leg Attribute does not exist.");

                                if (entityDetails[entitytypeName].Legs[legName].Attributes.ContainsKey(attrName))
                                {
                                    if (Convert.ToBoolean(entityDetails[entitytypeName].Legs[legName].Attributes[attrName].IsNullable) == false && !isMandatory)
                                        errorMessages.Add("Mandatory attribute cannot be marked as non mandatory");

                                    errorMessages.AddRange(layoutvalidation.validateCheckBoxList(isMandatory, isVisible, isReadOnly, entityDetails[entitytypeName].Legs[legName].Attributes[attrName].IsPrimary));

                                    if (Convert.ToBoolean(entityDetails[entitytypeName].Legs[legName].Attributes[attrName].IsPrimary) == true && !isVisible)
                                        errorMessages.Add("Primary attribute cannot be marked as hidden");

                                    attrToValidate.Add(attrName);
                                }
                            }

                            if (!entityTypeVstemplateInfo[entitytypeName][layoutName].TabNameVsId.ContainsKey(tabName))
                                errorMessages.Add("Invalid tab name.");

                        }


                        if (errorMessages.Count > 0)
                        {
                            new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                            isValid = false;
                        }
                        else
                        {

                            if (string.IsNullOrEmpty(legName))
                                entitytypeId = entityDetails[entitytypeName].EntityTypeID;
                            else
                                entitytypeId = entityDetails[entitytypeName].Legs[legName].EntityTypeID;

                            templateId = entityTypeVstemplateInfo[entitytypeName][layoutName].TemplateId;
                            int tabNameId = Convert.ToInt32(entityTypeVstemplateInfo[entitytypeName][layoutName].TabNameVsId[tabName].tabNameId);

                            if (string.IsNullOrEmpty(workingTab))
                            {
                                workingTabId = tabNameId;
                                workingTab = tabName;
                                workingLegName = legName;
                            }

                            if (tabName != workingTab)
                            {
                                XElement tab = new XElement("Tab",
                                        new XAttribute("tabNameId", workingTabId),
                                        new XAttribute("headerName", workingTab)
                                        );

                                // Add to main
                                if (string.IsNullOrEmpty(workingLegName))
                                {
                                    accordianSection.Add(leftSection);
                                    accordianSection.Add(middleSection);
                                    accordianSection.Add(rightSection);
                                    tab.Add(accordianSection);
                                    serverControlsXML.Add(tab);
                                }

                                else
                                {
                                    tab.Add(leftSection);
                                    tab.Add(middleSection);
                                    tab.Add(rightSection);
                                    detailsXML.Add(tab);
                                }


                                leftSection = new XElement("Left");
                                middleSection = new XElement("Middle");
                                rightSection = new XElement("Right");
                                accordianSection = new XElement("Accordian");
                                workingTab = tabName;
                                workingTabId = tabNameId;
                                workingLegName = legName;
                            }

                            XElement controlElement = null;
                            if (string.IsNullOrEmpty(legName))
                            {
                                controlElement = new XElement("Control");
                                controlElement.Add(new XAttribute("id", entityDetails[entitytypeName].Attributes[attrName].AttributeName));
                                controlElement.Add(new XAttribute("control_id", entityDetails[entitytypeName].Attributes[attrName].EntityAttributeID));
                            }
                            else
                            {
                                controlElement = new XElement("DetailControl");
                                controlElement.Add(new XAttribute("id", entityDetails[entitytypeName].Legs[legName].Attributes[attrName].AttributeName));
                                controlElement.Add(new XAttribute("control_id", entityDetails[entitytypeName].Legs[legName].Attributes[attrName].EntityAttributeID));
                            }

                            controlElement.Add(new XAttribute("display_name", attrName));
                            controlElement.Add(new XAttribute("is_mandatory", isMandatory));
                            controlElement.Add(new XAttribute("to_show", isVisible));
                            controlElement.Add(new XAttribute("is_name_modified", isNameModified));
                            controlElement.Add(new XAttribute("is_read_only", isReadOnly));

                            if (panel.SRMEqualWithIgnoreCase("Left"))
                                leftSection.Add(controlElement);
                            if (panel.SRMEqualWithIgnoreCase("Right"))
                                rightSection.Add(controlElement);
                            if (panel.SRMEqualWithIgnoreCase("Center"))
                                middleSection.Add(controlElement);
                        }
                    }

                    XElement newTab = new XElement("Tab",
                                        new XAttribute("tabNameId", workingTabId),
                                        new XAttribute("headerName", workingTab));

                    if (string.IsNullOrEmpty(workingLegName))
                    {
                        accordianSection.Add(leftSection);
                        accordianSection.Add(middleSection);
                        accordianSection.Add(rightSection);
                        newTab.Add(accordianSection);
                        serverControlsXML.Add(newTab);
                    }

                    else
                    {
                        newTab.Add(leftSection);
                        newTab.Add(middleSection);
                        newTab.Add(rightSection);
                        detailsXML.Add(newTab);
                    }


                    leftSection = new XElement("Left");
                    middleSection = new XElement("Middle");
                    rightSection = new XElement("Right");

                    string xml = attributeXML.ToString();
                    if (entityDetails.ContainsKey(entitytypeName))
                        errorMessages.AddRange(layoutvalidation.ValidateAttributesInTemplate(entityDetails[entitytypeName], attrToValidate));//, legName));
                    if (errorMessages.Count > 0)
                    {
                        //new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                        foreach (var row in group.ToList())
                        {
                            new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                        }
                        isValid = false;
                    }
                    if (isValid)
                    {
                        objTemplateManagerInfo.EntityTypeId = entitytypeId;
                        objTemplateManagerInfo.TemplateId = templateId;
                        objTemplateManagerInfo.UIXml = xml;
                        objTemplateManagerInfo.LastModifiedBy = userName;

                        new RMModelerDBManager().SaveModifiedUIXml(objTemplateManagerInfo, mDBCon);
                        mDBCon.CommitTransaction();
                        foreach (var row in group.ToList())
                        {
                            new RMCommonMigration().SetPassedRow(row, string.Empty);
                        }
                    }
                    else
                    {
                        foreach (ObjectRow row in group.ToList().Where(t => string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))).ToList())
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { }, true);
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeManagement-> Error " + ex.Message);

                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();
                    //Code to fail dependent rows in other tables
                    foreach (var row in group.ToList())
                    {
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                    }
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }
            }
            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeManagement-> End");
        }

        public void SyncLegOrder(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingLegOrderInfo, ObjectTable sourceTable, string userName, Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, Dictionary<string, RMEntityDetailsInfo> entityDetails)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncLegOrder-> Start ");

            var legOrderGroups = currentRows
                            .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS]))).GroupBy(x => new
                            {
                                EntityTypeName = Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]),
                                Layout = Convert.ToString(x[RMModelerConstants.Layout_Name])
                            });

            foreach (var group in legOrderGroups)
            {
                List<string> errorMessages = new List<string>();
                bool isValid = true;
                RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                try
                {
                    string entitytypeName = Convert.ToString(group.Key.EntityTypeName);
                    string layoutName = Convert.ToString(group.Key.Layout);
                    int entitytypeId = 0;
                    int templateId = 0;
                    int legId = 0;
                    DataTable dt = new DataTable();
                    dt.Columns.Add("entity_display_name", typeof(string));
                    dt.Columns.Add("entity_type_id", typeof(int));

                    foreach (var row in group.ToList().OrderBy(x => Convert.ToInt32(x[RMModelerConstants.Display_Order])))
                    {
                        string legName = Convert.ToString(row[RMModelerConstants.Leg_Name]);
                        int displayOrder = Convert.ToInt32(row[RMModelerConstants.Display_Order]);

                        if (!entityDetails.ContainsKey(entitytypeName) && !entityTypeVstemplateInfo.ContainsKey(entitytypeName))
                            errorMessages.Add("Entity Type does not exist.");

                        else if (!entityTypeVstemplateInfo[entitytypeName].ContainsKey(layoutName))
                            errorMessages.Add("Layout does not exist.");

                        else if (!entityDetails[entitytypeName].Legs.ContainsKey(legName))
                            errorMessages.Add("Invalid leg. Leg does not exist.");

                        if (errorMessages.Count() > 0)
                        {
                            new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                            isValid = false;
                        }
                        else
                        {
                            entitytypeId = Convert.ToInt32(entityDetails[entitytypeName].EntityTypeID);
                            templateId = Convert.ToInt32(entityTypeVstemplateInfo[entitytypeName][layoutName].TemplateId);
                            legId = Convert.ToInt32(entityDetails[entitytypeName].Legs[legName].EntityTypeID);
                            dt.Rows.Add(legName, legId);
                        }
                    }
                    if (isValid)
                    {
                        new RMModelerDBManager().SaveEntityTypeLegsDisplayOrder(templateId, entitytypeId, dt, mDBCon);
                        foreach (var row in group.ToList())
                        {
                            new RMCommonMigration().SetPassedRow(row, string.Empty);
                        }
                        mDBCon.CommitTransaction();
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error("RMEntityTypeModelerMigration-SyncLegOrder-> Error " + ex.Message);

                    if (mDBCon != null)
                        mDBCon.RollbackTransaction();
                    //Code to fail dependent rows in other tables
                    foreach (var row in group.ToList())
                    {
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                    }
                }
                finally
                {
                    if (mDBCon != null)
                    {
                        RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                        mDBCon = null;
                    }
                }
            }
            mLogger.Error("RMEntityTypeModelerMigration-SyncLegOrder-> End ");
        }

        public void SyncAttributeConfiguration(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingAttrConfigInfo, ObjectTable sourceTable, string userName, Dictionary<string, RMEntityDetailsInfo> entityDetails)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeConfiguration-> Start");
            int entitytypeId = 0;

            RMAttributeConfigMgmtValidations attrConfigvalidation = new RMAttributeConfigMgmtValidations();

            currentRows
                .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS]))).ToList().ForEach(row =>
                {
                    List<string> errorMessages = new List<string>();
                    bool isValid = true;
                    RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                    RMManageAttributeConfigurationInfo objRMManageAttributeConfigurationInfo = new RMManageAttributeConfigurationInfo();
                    try
                    {

                        string entityTypeName = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]);
                        string pageIdentifier = Convert.ToString(row[RMModelerConstants.Page_Identifier]);
                        string functionalityIdentifier = Convert.ToString(row[RMModelerConstants.Functionality_Identifier]);
                        string attrName = Convert.ToString(row[RMModelerConstants.Attribute_Name]);
                        string attrIds = string.Empty;
                        List<string> attrNamesLst = attrName.Split(',').ToList();
                        List<int> id = new List<int>();

                        if (!entityDetails.ContainsKey(entityTypeName))
                            errorMessages.Add("Entity Type does not exist.");

                        if (attrNamesLst.Count > 1)
                            errorMessages.Add("You cannot select more than one attribute");

                        else if (entityDetails.ContainsKey(entityTypeName))
                        {
                            if (!string.IsNullOrEmpty(attrName))
                            {
                                foreach (var names in attrNamesLst)
                                {
                                    if (!entityDetails[entityTypeName].Attributes.ContainsKey(names))
                                        errorMessages.Add("Attribute " + names + " does not exist.");

                                    else
                                        id.Add(Convert.ToInt32(entityDetails[entityTypeName].Attributes[names].EntityAttributeID));
                                }
                            }
                            errorMessages.AddRange(attrConfigvalidation.validatePageAndFuncIdentifier(pageIdentifier, functionalityIdentifier));
                        }


                        if (errorMessages.Count > 0)
                        {
                            new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                            isValid = false;
                        }
                        else
                        {
                            if (id.Count > 0)
                                attrIds = string.Join(",", id);
                            entitytypeId = Convert.ToInt32(entityDetails[entityTypeName].EntityTypeID);
                            objRMManageAttributeConfigurationInfo.EntityTypeId = Convert.ToString(entitytypeId);
                            objRMManageAttributeConfigurationInfo.AttributeIds = string.IsNullOrEmpty(attrName) ? "" : attrIds;
                            objRMManageAttributeConfigurationInfo.PageIdentifier = pageIdentifier;
                            objRMManageAttributeConfigurationInfo.FunctionalityIdentifier = functionalityIdentifier;
                        }
                        if (isValid)
                        {
                            new RMModelerDBManager().InsertDataForPageConfig(objRMManageAttributeConfigurationInfo);
                            new RMCommonMigration().SetPassedRow(row, string.Empty);
                            mDBCon.CommitTransaction();
                        }

                    }
                    catch (Exception ex)
                    {
                        mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeConfiguration-> Error " + ex.Message);

                        if (mDBCon != null)
                            mDBCon.RollbackTransaction();

                        new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                    }
                    finally
                    {
                        if (mDBCon != null)
                        {
                            RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                            mDBCon = null;
                            objRMManageAttributeConfigurationInfo = null;
                        }
                    }
                });
            mLogger.Error("RMEntityTypeModelerMigration-SyncAttributeConfiguration-> End");
        }

        public void SyncExceptionPreferences(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingAttrConfigInfo, ObjectTable sourceTable, string userName, Dictionary<string, RMEntityDetailsInfo> entityDetails, int moduleId)
        {
            bool isValid = true;
            List<ExceptionsConfig> lstValidConfig = new List<ExceptionsConfig>();
            List<ObjectRow> ValidRows = new List<ObjectRow>();
            List<string> errorMessages = new List<string>();
            currentRows
                .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS]))).ToList().ForEach(row =>
                {

                    RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                    ExceptionsConfig objExceptionConfig = new ExceptionsConfig();
                    string entityTypeName = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]);
                    bool alerts = Convert.ToBoolean(row[RMModelerConstants.ALERTS]);
                    bool validation = Convert.ToBoolean(row[RMModelerConstants.VALIDATION]);
                    bool duplicates = Convert.ToBoolean(row[RMModelerConstants.Duplicates]);
                    bool vendorMismatch = Convert.ToBoolean(row[RMModelerConstants.VENDOR_MISMATCH]);
                    bool refDataMissing = Convert.ToBoolean(row[RMModelerConstants.REF_DATA_MISSING]);
                    bool noVendorValue = Convert.ToBoolean(row[RMModelerConstants.NO_VENDOR_VALUE]);
                    bool valueChanged = Convert.ToBoolean(row[RMModelerConstants.VALUE_CHANGED]);
                    bool showAsException = Convert.ToBoolean(row[RMModelerConstants.SHOW_AS_EXCEPTION]);
                    bool vendorValueMissing = Convert.ToBoolean(row[RMModelerConstants.VENDOR_VALUE_MISSING]);
                    bool invalidData = Convert.ToBoolean(row[RMModelerConstants.INVALID_DATA]);

                    if (!entityDetails.ContainsKey(entityTypeName))
                    {
                        errorMessages.Add("Entity type does not exist");
                        isValid = false;
                    }
                    else
                    {
                        objExceptionConfig.instrumentTypeId = entityDetails[entityTypeName].EntityTypeID;
                        objExceptionConfig.moduleId = moduleId;
                        objExceptionConfig.Alert = alerts;
                        objExceptionConfig.Duplicate = duplicates;
                        objExceptionConfig.FirstVendorValueMissing = vendorValueMissing;
                        objExceptionConfig.InvalidData = invalidData;
                        objExceptionConfig.NoVendorValue = noVendorValue;
                        objExceptionConfig.RefDataMissing = refDataMissing;
                        objExceptionConfig.ShowAsException = showAsException;
                        objExceptionConfig.Validation = validation;
                        objExceptionConfig.ValueChange = valueChanged;
                        objExceptionConfig.VendorMismatch = vendorMismatch;
                    }
                    if (errorMessages.Count > 0)
                    {
                        new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                        isValid = false;
                    }
                    else
                    {
                        if (!lstValidConfig.Contains(objExceptionConfig))
                            lstValidConfig.Add(objExceptionConfig);
                        ValidRows.Add(row);
                    }
                });

            try
            {
                if (lstValidConfig != null && lstValidConfig.Count > 0)
                {   /// final save in db -> if no value then save false
                    string status = SRMCommon.saveExceptionConfigDetails(lstValidConfig, userName);
                    foreach (var row in ValidRows.ToList())
                    {
                        new RMCommonMigration().SetPassedRow(row, string.Empty);
                    }
                    // fill success
                }
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.Message.ToString());
                foreach (var row in ValidRows.ToList())
                {
                    new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                }
            }
        }

        public void SyncActionNotifications(IEnumerable<ObjectRow> currentRows, ObjectSet deltaSet, ObjectTable existingActionConfigInfo, ObjectTable sourceTable, string userName, Dictionary<string, RMEntityDetailsInfo> entityDetails, int moduleId)
        {
            mLogger.Error("RMEntityTypeModelerMigration-SyncUniqueKeys->Start");

            bool isInsert = false;

            List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMModelerConstants.CONFIG_MASTER_ID, DataType = typeof(Int32), DefaultValue = -1 });
            columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });
            new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

            List<string> allConfiguredTransports = new RTransportService().GetAllQueueTransportName();

            //when existing key
            var assign = from db in existingActionConfigInfo.AsEnumerable()
                         join upl in sourceTable.AsEnumerable()
                         on new { eName = ConvertToLower(db[RMModelerConstants.ENTITY_DISPLAY_NAME]), atName = ConvertToLower(db[RMModelerConstants.Attribute_Name]), legName = ConvertToLower(db[RMModelerConstants.Leg_Name]), eventLevel = ConvertToLower(db[RMModelerConstants.Action_Level]) }
                         equals new { eName = ConvertToLower(upl[RMModelerConstants.ENTITY_DISPLAY_NAME]), atName = ConvertToLower(upl[RMModelerConstants.Attribute_Name]), legName = ConvertToLower(upl[RMModelerConstants.Leg_Name]), eventLevel = ConvertToLower(upl[RMModelerConstants.Action_Level]) }

                         select AssignExistingActionConfigs(db, upl);

            assign.Count();

            var eventConfigGroupsPerEntityType = currentRows
                .Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS]))).GroupBy(x => new
                {
                    EntityTypeName = Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME])
                });

            foreach (var group in eventConfigGroupsPerEntityType)
            {

                bool isValid = true;
                //RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMaster_Connection);
                try
                {

                    SRMEventsAllLevelActions eventActionObj = new SRMEventsAllLevelActions();
                    eventActionObj.AttributeLevel = new List<SRMEventsAttributeLegActions>();
                    eventActionObj.LegLevel = new List<SRMEventsAttributeLegActions>();
                    //List<SRMEventsAttributeLegActions> attributeEventInfo = new List<SRMEventsAttributeLegActions>();
                    //List<SRMEventsAttributeLegActions> legEventInfo = new List<SRMEventsAttributeLegActions>();
                    //var eventConfigGroups = currentRows
                    //.Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_REMARKS]))).GroupBy(x => new
                    //{
                    //    AttrName = Convert.ToString(x[RMModelerConstants.Attribute_Name]),
                    //    LegName = Convert.ToString(x[RMModelerConstants.Leg_Name]),
                    //    EventLevel = Convert.ToString(x[RMModelerConstants.Action_Level])
                    //});

                    string entityTypeName = Convert.ToString(group.Key.EntityTypeName);
                    int instrumentTypeId = 0;
                    foreach (var row in group)
                    {
                        try
                        {
                            List<string> errorMessages = new List<string>();
                            string AttrName = Convert.ToString(row[RMModelerConstants.Attribute_Name]);
                            string LegName = Convert.ToString(row[RMModelerConstants.Leg_Name]);
                            string EventLevel = Convert.ToString(row[RMModelerConstants.Action_Level]);
                            int dependent_id = 0;
                            isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);
                            string queues = Convert.ToString(row[RMModelerConstants.Queues]);
                            string actions = Convert.ToString(row[RMModelerConstants.Actions]);
                            List<string> lstQueues = queues.Split(',').ToList();
                            List<string> lstActions = actions.Split(',').ToList();
                            List<int> lstActionIds = new List<int>();
                            SRMEventsAttributeLegActions attributeLegDetails = new SRMEventsAttributeLegActions();
                            RMEventnotificationValidations eventvalidations = new RMEventnotificationValidations();
                            //int uniqueKeyId = 0;
                            //RMEntityTypeSetupSync uniquevalidate = new RMEntityTypeSetupSync();

                            if (!entityDetails.ContainsKey(entityTypeName))
                                errorMessages.Add("Invalid entity type " + entityTypeName);

                            else
                            {
                                instrumentTypeId = entityDetails[entityTypeName].EntityTypeID;

                                if (EventLevel.ToLower().Equals("entity"))
                                {
                                    dependent_id = entityDetails[entityTypeName].EntityTypeID;
                                    if (lstActions.Contains("Attribute_Update", StringComparer.OrdinalIgnoreCase))
                                        errorMessages.Add("Incorrect Action Attribute_Update defined for Entity level.");

                                    if (!string.IsNullOrEmpty(AttrName))
                                        errorMessages.Add("Attribute Name cannot be populated for Entity level.");

                                    if (!string.IsNullOrEmpty(LegName))
                                        errorMessages.Add("Leg Name cannot be populated for Entity level.");
                                }

                                else if ((EventLevel.ToLower().Equals("attribute") || EventLevel.ToLower().Equals("leg")))
                                {
                                    if (lstActions.Contains("Create", StringComparer.OrdinalIgnoreCase) || lstActions.Contains("Update", StringComparer.OrdinalIgnoreCase))
                                        errorMessages.Add("Incorrect Action defined for Attribute/Leg level.");

                                    if (EventLevel.ToLower().Equals("attribute"))
                                    {
                                        if (!entityDetails[entityTypeName].Attributes.ContainsKey(AttrName))
                                            errorMessages.Add("Attribute does not exist.");
                                        else
                                            dependent_id = entityDetails[entityTypeName].Attributes[AttrName].EntityAttributeID;
                                        if (!string.IsNullOrEmpty(LegName))
                                            errorMessages.Add("Leg Name cannot be populated for Attribute level.");
                                    }

                                    if (EventLevel.ToLower().Equals("leg"))
                                    {
                                        if (!entityDetails[entityTypeName].Legs.ContainsKey(LegName))
                                            errorMessages.Add("Leg does not exist.");
                                        else
                                            dependent_id = entityDetails[entityTypeName].Legs[LegName].EntityTypeID;

                                        if (!string.IsNullOrEmpty(AttrName))
                                            errorMessages.Add("Attribute Name cannot be populated for Leg level.");
                                    }
                                }
                                errorMessages.AddRange(eventvalidations.validateQueues(queues));
                                errorMessages.AddRange(eventvalidations.validateActions(actions));
                            }
                            if (errorMessages.Count > 0)
                            {
                                isValid = false;
                                new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                            }
                            else
                            {
                                if (lstActions.Count > 0)
                                {
                                    foreach (var action in lstActions)
                                    {
                                        var id = (SRMEventActionType)Enum.Parse(typeof(SRMEventActionType), action, true);
                                        lstActionIds.Add((int)id);
                                    }
                                }
                                if (EventLevel.ToLower().Equals("entity"))
                                {
                                    if (!isInsert)
                                        eventActionObj.ConfigId = Convert.ToInt32(row[RMModelerConstants.CONFIG_MASTER_ID]);
                                    else
                                        eventActionObj.ConfigId = -1;
                                    eventActionObj.SelectedInstrumentQueue = allConfiguredTransports.Select(i => i.ToString()).Intersect(lstQueues, StringComparer.OrdinalIgnoreCase).ToList();
                                    //eventActionObj.SelectedInstrumentQueue = allConfiguredTransports.Select( x=> x.Equals(lstQueues));
                                    eventActionObj.SelectedInstrumentActions = lstActionIds;
                                }
                                else if (EventLevel.ToLower().Equals("attribute"))
                                {
                                    if (!isInsert)
                                        attributeLegDetails.ConfigId = Convert.ToInt32(row[RMModelerConstants.CONFIG_MASTER_ID]);
                                    else
                                        eventActionObj.ConfigId = -1;
                                    attributeLegDetails.DependentId = dependent_id;
                                    attributeLegDetails.DisplayName = AttrName;
                                    attributeLegDetails.IsAdditional = false;
                                    attributeLegDetails.SelectedAttributeLegActions = lstActionIds;
                                    attributeLegDetails.SelectedAttributeLegQueue = allConfiguredTransports.Select(i => i.ToString()).Intersect(lstQueues, StringComparer.OrdinalIgnoreCase).ToList();
                                    eventActionObj.AttributeLevel.Add(attributeLegDetails);
                                }
                                else if (EventLevel.ToLower().Equals("leg"))
                                {
                                    if (!isInsert)
                                        attributeLegDetails.ConfigId = Convert.ToInt32(row[RMModelerConstants.CONFIG_MASTER_ID]);
                                    attributeLegDetails.DependentId = dependent_id;
                                    attributeLegDetails.DisplayName = LegName;
                                    attributeLegDetails.IsAdditional = false;
                                    attributeLegDetails.SelectedAttributeLegActions = lstActionIds;
                                    attributeLegDetails.SelectedAttributeLegQueue = allConfiguredTransports.Select(i => i.ToString()).Intersect(lstQueues, StringComparer.OrdinalIgnoreCase).ToList();
                                    eventActionObj.LegLevel.Add(attributeLegDetails);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            mLogger.Error("RMEntityTypeModelerMigration-SyncActionNotifications->Error " + ex.Message);
                            isValid = false;
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
                        }

                    }
                    if (isValid)
                    {
                        try
                        {
                            SRMEventController.SaveEventConfigForInstrumentType(moduleId, instrumentTypeId, eventActionObj, userName);
                            foreach (var row in group.ToList())
                            {
                                new RMCommonMigration().SetPassedRow(row, string.Empty);
                            }
                        }
                        catch (Exception ex)
                        {
                            foreach (var row in group.ToList())
                            {
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { Convert.ToString(ex.Message) }, false);
                            }
                        }
                    }
                    else
                    {
                        //foreach (var row in group.ToList())
                        //{
                        //    new RMCommonMigration().SetFailedRow(row, err, false);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Error("RMEntityTypeModelerMigration-SyncActionNotifications->Error " + ex.Message);
                }
                finally
                {
                }
            }

            //Remove static columns
            List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMModelerConstants.CONFIG_MASTER_ID };

            new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);
            mLogger.Error("RMEntityTypeModelerMigration-SyncActionNotifications->End");

        }
        private bool AssignExistingEntityType(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMModelerConstants.ENTITY_TYPE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.ENTITY_TYPE_ID]);
            sourceRow[RMModelerConstants.ENTITY_TYPE_NAME] = Convert.ToString(dbRow[RMModelerConstants.ENTITY_TYPE_NAME]);
            return true;
        }

        private bool AssignExistingEntityLegTypeInfo(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            return true;
        }

        private bool AssignExistingAttributes(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMModelerConstants.ENTITY_ATTRIBUTE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.ENTITY_ATTRIBUTE_ID]);
            sourceRow[RMModelerConstants.Attribute_Real_Name] = Convert.ToString(dbRow[RMModelerConstants.Attribute_Real_Name]);
            return true;
        }

        private bool AssignExistingLegAttributes(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMModelerConstants.ENTITY_ATTRIBUTE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.ENTITY_ATTRIBUTE_ID]);
            sourceRow[RMModelerConstants.Attribute_Real_Name] = Convert.ToString(dbRow[RMModelerConstants.Attribute_Real_Name]);
            return true;

        }
        private bool AssignExistingUniqueKeys(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMModelerConstants.UNIQUE_KEY_ID] = Convert.ToInt32(dbRow[RMModelerConstants.UNIQUE_KEY_ID]);
            return true;
        }

        private bool AssignExistingActionConfigs(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMModelerConstants.CONFIG_MASTER_ID] = Convert.ToInt32(dbRow[RMModelerConstants.CONFIG_MASTER_ID]);
            return true;
        }

        private bool AssignExistingLayoutIds(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[RMModelerConstants.TEMPLATE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.TEMPLATE_ID]);
            sourceRow[COL_IS_INSERT] = false;
            return true;
        }

        private bool AssignExistingTabs(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMModelerConstants.TAB_NAME_ID] = Convert.ToInt32(dbRow[RMModelerConstants.TAB_NAME_ID]);
            return true;
        }

        private bool AssignExistingLayoutAttributes(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMModelerConstants.MASTER_ENTITY_TYPE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.MASTER_ENTITY_TYPE_ID]);
            sourceRow[RMModelerConstants.LEG_ENTITY_TYPE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.LEG_ENTITY_TYPE_ID]);
            sourceRow[RMModelerConstants.TEMPLATE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.TEMPLATE_ID]);
            sourceRow[RMModelerConstants.TAB_NAME_ID] = Convert.ToInt32(dbRow[RMModelerConstants.TAB_NAME_ID]);
            sourceRow[RMModelerConstants.ENTITY_ATTRIBUTE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.ENTITY_ATTRIBUTE_ID]);
            return true;
        }

        private bool AssignExistingLegOrderDetails(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMModelerConstants.MASTER_ENTITY_TYPE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.MASTER_ENTITY_TYPE_ID]);
            sourceRow[RMModelerConstants.LEG_ENTITY_TYPE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.LEG_ENTITY_TYPE_ID]);
            sourceRow[RMModelerConstants.TEMPLATE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.TEMPLATE_ID]);
            return true;
        }

        private bool AssignExistingAttrConfigInfo(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[RMModelerConstants.ENTITY_TYPE_ID] = Convert.ToInt32(dbRow[RMModelerConstants.ENTITY_TYPE_ID]);
            return true;
        }
        private string ConvertToLower(object ob)
        {
            if (string.IsNullOrEmpty(Convert.ToString(ob)))
                return string.Empty;
            else
                return ob.ToString().ToLower();
        }
        private RMDBDataTypes GetDataType(string dataType)
        {
            RMDBDataTypes type = RMDBDataTypes.VARCHAR;
            switch (dataType.ToUpper())
            {
                case RMAttributeSetupConstants.DATA_TYPE_VARCHAR:
                    type = RMDBDataTypes.VARCHAR;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_VARCHAR_MAX:
                    type = RMDBDataTypes.VARCHARMAX;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_INT:
                    type = RMDBDataTypes.INT;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_DATETIME:
                    type = RMDBDataTypes.DATETIME;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_BIT:
                    type = RMDBDataTypes.BIT;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_LOOKUP:
                    type = RMDBDataTypes.LOOKUP;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_SECURITYLOOKUP:
                case RMAttributeSetupConstants.DATA_TYPE_SECURITY_LOOKUP:
                    type = RMDBDataTypes.SECURITY_LOOKUP;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_FILE:
                    type = RMDBDataTypes.FILE;
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_DECIMAL:
                    type = RMDBDataTypes.DECIMAL;
                    break;
            }

            return type;
        }

        //private void FailDependencies(ObjectSet deltaSet, string entityTypeName, bool isAttribute)
        //{
        //    Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
        //    foreach (ObjectTable table in deltaSet.Tables)
        //    {
        //        if (table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEGS_CONFIGURATION) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ATTRIBUTES)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_BASKET_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_UNIQUE_KEYS)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ORDER) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION))
        //        {
        //            dependencies.Add(table.TableName, null);

        //            if (!isAttribute)
        //            {
        //                dependencies.Add(RMModelerConstants.TABLE_MASTER_ATTIRBUTES, null);
        //            }

        //            dependencies[table.TableName] =
        //            table.AsEnumerable()
        //                .Where
        //                (
        //                    t => Convert.ToString(t[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS]))
        //                     && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))
        //                );
        //        }

        //    }
        //    if (isAttribute)
        //        new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ATTR_NOT_PROCESSED, true);
        //    else
        //        new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ET_NOT_PROCESSED, true);
        //}

        //private void FailEntityDependencies(ObjectSet deltaSet, string entityName)
        //{
        //    Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
        //    foreach (ObjectTable table in deltaSet.Tables)
        //    {
        //        if (table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_MASTER_ATTIRBUTES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEGS_CONFIGURATION) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ATTRIBUTES)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_BASKET_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_UNIQUE_KEYS)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_TAB_MANAGEMENT) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ORDER) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION))
        //        {
        //            dependencies.Add(table.TableName, null);

        //            dependencies[table.TableName] =
        //            table.AsEnumerable()
        //            .Where
        //            (
        //                t => Convert.ToString(t[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityName) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))
        //            );
        //        }

        //    }

        //    new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ET_NOT_PROCESSED, true);

        //}
        //private void FailLegDependencies(ObjectSet deltaSet, string entityTypeName, bool isleg)
        //{
        //    Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
        //    foreach (ObjectTable table in deltaSet.Tables)
        //    {
        //        if (table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ATTRIBUTES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_BASKET_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_UNIQUE_KEYS) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ORDER))
        //        {
        //            dependencies.Add(table.TableName, null);

        //            if (!isleg)
        //            {
        //                dependencies.Add(RMModelerConstants.TABLE_LEGS_ATTRIBUTEs, null);
        //            }

        //            dependencies[table.TableName] =
        //            table.AsEnumerable()
        //                .Where
        //                (
        //                    t => Convert.ToString(t[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))
        //                );
        //        }
        //    }
        //    if (isleg)
        //        new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.LEG_NOT_PROCESSED, true);
        //    else
        //        new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ET_NOT_PROCESSED, true);
        //}

        //private void FailLegAttributeDependencies(ObjectSet deltaSet, string legName, string entityTypeName, bool isleg)
        //{
        //    Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
        //    foreach (ObjectTable table in deltaSet.Tables)
        //    {
        //        if (table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_BASKET_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_UNIQUE_KEYS)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ORDER))
        //        {
        //            dependencies.Add(table.TableName, null);

        //            if (!isleg)
        //            {
        //                dependencies.Add(RMModelerConstants.TABLE_LEGS_ATTRIBUTEs, null);
        //            }

        //            dependencies[table.TableName] =
        //            table.AsEnumerable()
        //                .Where
        //                (
        //                    t => Convert.ToString(t[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS]))
        //                     && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))
        //                );
        //        }

        //    }
        //    if (isleg)
        //        new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ATTR_NOT_PROCESSED, true);
        //    else
        //        new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ET_NOT_PROCESSED, true);
        //}

        //private void FailLayoutDependencies(ObjectSet deltaSet, string layoutName)
        //{
        //    Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
        //    foreach (ObjectTable table in deltaSet.Tables)
        //    {
        //        if (table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_MASTER_ATTIRBUTES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEGS_CONFIGURATION) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ATTRIBUTES)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_BASKET_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_UNIQUE_KEYS)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_TAB_MANAGEMENT) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ORDER) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION))
        //        {
        //            dependencies.Add(table.TableName, null);

        //            //if(i)
        //            //dependencies[table.TableName] =
        //            //table.AsEnumerable()
        //            //.Where
        //            //(
        //            //    t => Convert.ToString(t[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityName) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))
        //            //);
        //        }

        //    }

        //    new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.LAYOUT_NOT_PROCESSED, true);
        //}

        //private void FailTabDependencies(ObjectSet deltaSet, string layoutName)
        //{
        //    Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
        //    foreach (ObjectTable table in deltaSet.Tables)
        //    {
        //        if (table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT))
        //            dependencies.Add(table.TableName, null);

        //        {
        //            dependencies[table.TableName] =
        //          table.AsEnumerable()
        //              .Where
        //              (

        //                  t => Convert.ToString(t[RMModelerConstants.Layout_Name]).SRMEqualWithIgnoreCase(layoutName) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS]))
        //              );
        //        }
        //    }

        //    new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.TAB_NOT_PROCESSED, true);
        //}

        //private void FailLegEntityTypeDependencies(ObjectSet deltaSet, string entityTypeName, string legName)
        //{
        //    Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
        //    foreach (ObjectTable table in deltaSet.Tables)
        //    {
        //        if (table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ATTRIBUTES)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_UNIQUE_KEYS) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_ATTRIBUTE_RULES) || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_BASKET_RULES)
        //            || table.TableName.SRMEqualWithIgnoreCase(RMModelerConstants.TABLE_LEG_ORDER))
        //            dependencies.Add(table.TableName, null);

        //        {
        //            dependencies[table.TableName] =
        //          table.AsEnumerable()
        //              .Where
        //              (
        //                  t => Convert.ToString(t[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) && Convert.ToString(t[RMModelerConstants.Leg_Name]).SRMEqualWithIgnoreCase(legName)
        //                  && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS]))
        //              );
        //        }
        //    }
        //    new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ET_NOT_PROCESSED, true);
        //}

        public void FailAllDependencies(ObjectSet deltaSet, string entityTypeName, string layoutName, bool isAttribute, bool isLeg, bool isLegAttr, bool isunique, bool isattrRule, bool isbasketRule, bool isLayout, bool istab, bool isattrMgmt, bool isLegOrder, bool isattrConfig)
        {
            Dictionary<string, IEnumerable<ObjectRow>> dependencies = new Dictionary<string, IEnumerable<ObjectRow>>();
            foreach (ObjectTable table in deltaSet.Tables)
            {

                if (!isAttribute && table.TableName.Contains(RMModelerConstants.TABLE_MASTER_ATTIRBUTES))
                    dependencies.Add(RMModelerConstants.TABLE_MASTER_ATTIRBUTES, null);
                if (!isLeg && table.TableName.Contains(RMModelerConstants.TABLE_LEGS_CONFIGURATION))
                    dependencies.Add(RMModelerConstants.TABLE_LEGS_CONFIGURATION, null);
                if (!isLegAttr && table.TableName.Contains(RMModelerConstants.TABLE_LEG_ATTRIBUTES))
                    dependencies.Add(RMModelerConstants.TABLE_LEG_ATTRIBUTES, null);
                if (!isunique && table.TableName.Contains(RMModelerConstants.TABLE_UNIQUE_KEYS))
                    dependencies.Add(RMModelerConstants.TABLE_UNIQUE_KEYS, null);
                if (!isattrRule && table.TableName.Contains(RMModelerConstants.TABLE_ATTRIBUTE_RULES))
                    dependencies.Add(RMModelerConstants.TABLE_ATTRIBUTE_RULES, null);
                if (!isbasketRule && table.TableName.Contains(RMModelerConstants.TABLE_BASKET_RULES))
                    dependencies.Add(RMModelerConstants.TABLE_BASKET_RULES, null);
                if (!isLayout && table.TableName.Contains(RMModelerConstants.TABLE_LAYOUTS))
                    dependencies.Add(RMModelerConstants.TABLE_LAYOUTS, null);
                if (!istab && table.TableName.Contains(RMModelerConstants.TABLE_TAB_MANAGEMENT))
                    dependencies.Add(RMModelerConstants.TABLE_TAB_MANAGEMENT, null);
                if (!isattrMgmt && table.TableName.Contains(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT))
                    dependencies.Add(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT, null);
                if (!isLegOrder && table.TableName.Contains(RMModelerConstants.TABLE_LEG_ORDER))
                    dependencies.Add(RMModelerConstants.TABLE_LEG_ORDER, null);
                if (!isattrConfig && table.TableName.Contains(RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION))
                    dependencies.Add(RMModelerConstants.TABLE_ATTRIBUTE_CONFIGURATION, null);

                if (isLayout && !string.IsNullOrEmpty(layoutName) && (table.TableName.Contains(RMModelerConstants.TABLE_TAB_MANAGEMENT) || table.TableName.Contains(RMModelerConstants.TABLE_ATTRIBUTE_MANAGEMENT) || table.TableName.Contains(RMModelerConstants.TABLE_LEG_ORDER)))
                {
                    dependencies[table.TableName] =
                                        table.AsEnumerable()
                                            .Where
                                            (
                                                t => Convert.ToString(t[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) && Convert.ToString(t[RMModelerConstants.Layout_Name]).SRMEqualWithIgnoreCase(layoutName) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))
                                            );
                }

                else
                {
                    dependencies[table.TableName] =
                  table.AsEnumerable()
                      .Where
                      (
                          t => Convert.ToString(t[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_REMARKS])) && string.IsNullOrEmpty(Convert.ToString(t[RMCommonConstants.MIGRATION_COL_STATUS]))
                      );
                }


            }

            if (isAttribute && !isLeg && !isLegAttr && !isLayout && !istab)
                new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ATTR_NOT_PROCESSED, true);
            else if (isLeg && !isLegAttr && !isLayout && !istab)
                new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.LEG_NOT_PROCESSED, true);
            else if (isLegAttr && !isLayout && !istab)
                new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ATTR_NOT_PROCESSED, true);
            else if (isLayout && !istab)
                new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.LAYOUT_NOT_PROCESSED, true);
            else if (istab)
                new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.TAB_NOT_PROCESSED, true);
            else
                new RMCommonMigration().FailDependencies(dependencies, RMCommonConstants.ET_NOT_PROCESSED, true);
        }

        //private void MergeExistingAttributes(List<ObjectRow> attributeRows, ObjectTable dbTable, string layoutName, string entitytypeName)
        //{
        //    List<string> attributesInFile = attributeRows.Select(c => RMCommonStatic.ConvertToLower(c[RMModelerConstants.Attribute_Name])).Distinct().ToList();

        //    List<ObjectRow> dbRows = dbTable.AsEnumerable()
        //        .Where(r => RMCommonStatic.ConvertToLower(r[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entitytypeName)
        //            && RMCommonStatic.ConvertToLower(r[RMModelerConstants.Layout_Name]).SRMEqualWithIgnoreCase(layoutName)
        //            && !attributesInFile.SRMContainsWithIgnoreCase(RMCommonStatic.ConvertToLower(r[RMModelerConstants.Attribute_Name]))
        //            ).ToList();

        //    if (dbRows != null)
        //    {
        //        attributeRows.AddRange(dbRows);
        //        //foreach (ObjectRow row in dbRows)
        //        //    new RMCommonMigration().SetInSyncRow(row, string.Empty);
        //    }

        //}
    }
}
